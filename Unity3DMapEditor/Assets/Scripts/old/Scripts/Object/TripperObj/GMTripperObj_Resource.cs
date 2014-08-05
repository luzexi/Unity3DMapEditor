
/**
	生活技能_矿物资源
*/
//生活技能定义
public enum LIFE_ABILITY
{
      LIFE_ABILITY_MINE	=	7,
      LIFE_ABILITY_HERBS	=	8,
      LIFE_ABILITY_FISH	=	9,
      LIFE_ABILITY_INTERACT =	10,
      LIFE_ABILITY_SUBDUE	 =	21
}
 

public class CTripperObject_Resource :	 CTripperObject
{
//Tripper 类型
    public override TRIPPER_OBJECT_TYPE Tripper_GetType() { return TRIPPER_OBJECT_TYPE.TOT_RESOURCE; }
//能否鼠标操作
    public override bool Tripper_CanOperation()
{
    	// 采集物支持多个任务ID [4/27/2011 ivan edit]
 	if (m_pResourceDef.szMissionID ==null || m_pResourceDef.szMissionID == "")
 		return false;
 
 	string[] allMisId = m_pResourceDef.szMissionID.Split(',');
    
 	for (uint i =0; i<allMisId.Length; i++)
 	{
 		int misId = System.Int32.Parse(allMisId[i]);

        if (CDetailAttrib_Player.Instance.IsMissionCanCommit(misId))
            continue;

        int misIndex = CDetailAttrib_Player.Instance.GetMissionIndexByID(misId);
 		if (misIndex != MacroDefine.INVALID_ID)
 			return true;
 	}
 	return false;
}
	//鼠标类型
public override ENUM_CURSOR_TYPE Tripper_GetCursor()
{
    	// 增加任务识别判断 [4/25/2011 ivan edit]
	if(m_pResourceDef == null || !Tripper_CanOperation()) return ENUM_CURSOR_TYPE.CURSOR_NORMAL;

	int idAbility = m_pResourceDef.nLifeAbility;
	//如果没有该生活技能，返回普通光标
/*	if(!(CObjectManager.GetMe().GetMySelf().GetCharacterData().Get_LifeAbility(idAbility)))
	{
		return CURSOR_NORMAL;
	}
*/
	//如果此生长点对应的生活技能是-1时，对鼠标不响应。
	if(idAbility == MacroDefine.INVALID_ID)
		return ENUM_CURSOR_TYPE.CURSOR_NORMAL;

	switch(idAbility)
	{
	    // 采矿技能
	    case (int)LIFE_ABILITY.LIFE_ABILITY_MINE:		return ENUM_CURSOR_TYPE.CURSOR_MINE;
	    // 采药
        case (int)LIFE_ABILITY.LIFE_ABILITY_HERBS: return ENUM_CURSOR_TYPE.CURSOR_HERBS;
	    // 钓鱼
        case (int)LIFE_ABILITY.LIFE_ABILITY_FISH: return ENUM_CURSOR_TYPE.CURSOR_INTERACT;
	    // 操作
        case (int)LIFE_ABILITY.LIFE_ABILITY_INTERACT: return ENUM_CURSOR_TYPE.CURSOR_INTERACT;
	    // 驱鬼
        case (int)LIFE_ABILITY.LIFE_ABILITY_SUBDUE: return ENUM_CURSOR_TYPE.CURSOR_INTERACT;

	    default:					return ENUM_CURSOR_TYPE.CURSOR_INTERACT;
	}
}

//进入激活
public override void Tripper_Active()
{
    if(m_pResourceDef == null) return;

    Network.Packets.CGUseAbility msg = new Network.Packets.CGUseAbility();

	int idAbility = m_pResourceDef.nLifeAbility;

	//if( CObjectManager.Instance.getPlayerMySelf().CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ABILITY_ACTION )
	//	return;

	//如果没有该生活技能，返回
	if((CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_LifeAbility(idAbility)) == null)
	{
		_DBC_LIFEABILITY_DEFINE pAbilityDef = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_DEFINE>((int)DataBaseStruct.DBC_LIFEABILITY_DEFINE).Search_Index_EQU(idAbility);

		if( pAbilityDef == null)
			return;
		string szMsg = "需要"  + pAbilityDef.szName+  "生活技能";
        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, szMsg);
		return;
	}

	msg.SetAbilityID((short)idAbility);
	msg.SetPlatformGUID((uint)ServerID);
	msg.SetPrescriptionID(MacroDefine.INVALID_ID);
	msg.SetSpecialFlag(MacroDefine.UINT_MAX);
	GameProcedure.s_NetManager.SendPacket(msg);
}


	// 设置矿物资源ID Ref[DBC_LIFEABILITY_GROWPOINT]
	// 返回是否是合法的资源
public	bool							SetResourceID(int nID)
{
    m_pResourceDef = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_GROWPOINT>((int)DataBaseStruct.DBC_LIFEABILITY_GROWPOINT).Search_Index_EQU(nID);

	if( m_pResourceDef == null) return false;

	return true;
}

public int GetTableId()
{
    if (m_pResourceDef == null) 
        return -1;
    return m_pResourceDef.nID;
}

	// 查看资源的种类
public	int								GetResourceID()
{
    if(m_pResourceDef == null) return 0;
	return m_pResourceDef.nLifeAbility;
}
	// 是否是钓鱼需要的鱼群
public bool Resource_IsFish()
{
    if (m_pResourceDef.nLifeAbility == (int)LIFE_ABILITY.LIFE_ABILITY_FISH) return true;
    return false;
}
	// 能否可以开采资源
public bool Resource_CanOperation()
{ 
       if(m_pResourceDef == null) return false;

      SCLIENT_LIFEABILITY pSclientAbilityDef = (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_LifeAbility(m_pResourceDef.nLifeAbility));

      if (pSclientAbilityDef == null)
       {
           _DBC_LIFEABILITY_DEFINE pAbilityDef = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_DEFINE>((int)DataBaseStruct.DBC_LIFEABILITY_DEFINE).Search_Index_EQU(m_pResourceDef.nLifeAbility);

           if(pAbilityDef == null)
               return false;
           string szMsg = "Ability_Requiement" + pAbilityDef.szName;
           GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,szMsg);
	
           return false;
       }	

       int my_level = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level();
       if (my_level < pSclientAbilityDef.m_pDefine.nLevelNeed) 
       {
           GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "等级不够");
           return false;
       }
       int idNeedFacility = pSclientAbilityDef.m_pDefine.nToolNeed;

       if( idNeedFacility >= 0 )
       {
           bool bHaveTool = false;

           CObject_Item Facility;
           Facility = CDataPool.Instance.UserEquip_GetItem(HUMAN_EQUIP.HEQUIP_WEAPON);
           if(Facility == null || idNeedFacility != Facility.GetParticularID())
           {
               for( int i = 0; i < GAMEDEFINE.MAX_BAG_SIZE; i++ )
               {
                   if( (Facility = CDataPool.Instance.UserBag_GetItem(i)) != null )
                   {
                       if( idNeedFacility == Facility.GetParticularID() )
                       {
                           bHaveTool = true;
                           break;
                       }
                   }
               }

              string szTemp="";
               //从资源表中去查找工具名称
              DBC.COMMON_DBC<_DBC_ITEM_EQUIP_BASE> equipBaseDBC = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_EQUIP_BASE>((int)DataBaseStruct.DBC_ITEM_EQUIP_BASE);
		
             _DBC_ITEM_EQUIP_BASE pWhiteEquip = equipBaseDBC.Search_Index_EQU(idNeedFacility);
 
               if(pWhiteEquip != null)
               {
                   if(bHaveTool)
                   {
                       szTemp = "你需要先将" + pWhiteEquip.szName + "装备上";
                   }
                   else
                   {
                       szTemp ="你还没有" + pWhiteEquip.szName;
                   }
               }
               else
               {
                   szTemp = "你需要装备工具。";
               }
               GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,szTemp);

               //todo
              // ADDTALKMSG(szTemp);
               return false;
           }
       }
    return true;
}
	// 是否可钓鱼
public bool Resource_CanFish(float distance)
{
    if(m_pResourceDef == null) return false;
	
	SCLIENT_LIFEABILITY pAbilityDef = 
		CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_LifeAbility(m_pResourceDef.nLifeAbility);
	if(pAbilityDef == null) return false;

    if (pAbilityDef.m_pDefine.nID == (int)LIFE_ABILITY.LIFE_ABILITY_FISH)
	{
		float fishdistance = pAbilityDef.m_pDefine.fPlatformDist;
		//需要距离合适，如果距离不够，也不能开始钓。
		if ( distance > fishdistance ) 
		{
		//	CGameProcedure::s_pEventSystem->PushEvent(GE_INFO_SELF,"The distance is too far.");
			//SCRIPT_SANDBOX::Talk::s_Talk.AddSelfMessage("距离太远！");
			/*ADDTALKMSG("距离太远！");*/ //todo
			return false; 
		}
	}

	return true;
}
	// 查看资源所需要的操作距离
public	float							Resource_GetOperationDistance()
{
    if(m_pResourceDef != null)
	{
		SCLIENT_LIFEABILITY pAbilityDef = 
			CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_LifeAbility(m_pResourceDef.nLifeAbility);
		if(pAbilityDef != null)
		{
			return pAbilityDef.m_pDefine.fPlatformDist;
		}
	}
	return 0.0f;
}

	// 增加特效显示是否可采集 [5/11/2011 ivan edit]
public void Flash(bool isShow)
{
    if (mRenderInterface == null) return;
    string triperLocator = "jh_test3";
    string tripperActiveEffect = "cai_ji";
    if (isShow)
    {
        mRenderInterface.attachEffect(triperLocator, tripperActiveEffect);
    }
    else
    {
        mRenderInterface.attachEffect(triperLocator, "");
    }
}
	// 策划要求可交互物体支持说话 [8/12/2011 ivan edit]
public	void							Say(  string str )
{
    //todo
}

public	void							StopSay()
{
    //todo
}

public	bool							IsShowTutorial()
{
    //todo
    return false;
}
public void SetTutorialMask()
{
    //todo
}
	//-----------------------------------------------------
	///根据初始化物体，并同步到渲染层
public	override	void				Initial(object pInit)
{
    if ( mRenderInterface == null && m_pResourceDef != null && m_pResourceDef.szMeshFile != null)
	{
        mRenderInterface = GFX.GFXObjectManager.Instance.createObject(m_pResourceDef.szMeshFile, GFX.GFXObjectType.ACTOR);

		//设置选择优先级
		mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_TRIPPEROBJ);

		mRenderInterface.SetData(ID);

		// 刷新是否显示特效 [5/12/2011 ivan edit]
		Flash(Tripper_CanOperation());
	}
    base.Initial(pInit);
    base.CreateRenderInterface();//
}
	///逻辑函数
public override void Tick()
{
    SetMapPosition(mPosition.x, mPosition.z);
    base.Tick();
    //头顶信息板Tick
    Tick_UpdateInfoBoard();
}
	//信息板Tick
public	virtual void				Tick_UpdateInfoBoard()
{
    //todo
}

public	 _DBC_LIFEABILITY_GROWPOINT  GetLifeAbility(){ return m_pResourceDef; }

public CTripperObject_Resource()
{ 
    //todo
}

~CTripperObject_Resource()
{
    //todo
}


	//资源定义
protected	  _DBC_LIFEABILITY_GROWPOINT	m_pResourceDef;
	//--------------------------------------------------------
	//头顶信息板相关UI接口
	//--------------------------------------------------------
protected	CreatureBoard			m_pInfoBoard;
protected	bool					isTalking;
};
