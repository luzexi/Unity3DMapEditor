using System.Collections;
using UnityEngine;
public class CObject_PlayerMySelf : CObject_PlayerOther
{
    private tActionItem m_pActiveSkill;
    public override CHARACTER_TYPE GetCharacterType() { return CHARACTER_TYPE.CT_PLAYERMYSELF; }
    public void SetActiveSkill(tActionItem pSkill)
    {
        m_pActiveSkill = pSkill;
        CActionSystem.Instance.SetDefaultAction(pSkill);
    }
    public tActionItem GetActiveSkill() { return m_pActiveSkill; }
    public CObject_PlayerMySelf()
    {
        m_pTheAI = new CAI_MySelf(this);
    }
    CObject_PlayerOther m_pAvatar;//用于在UI显示
    public override void Initial(object pInit)
    {
        //解决部分情况下默认技能丢失的问题
        SetActiveSkill(null);
        base.Initial(pInit);
        SetFaceDir(3.1416f);//初始创建的玩家朝向x轴
        m_pAvatar = CObjectManager.Instance.NewFakePlayerOther();//创建用于UI显示的人物
    }
    public CObject getAvatar()
    {
        return m_pAvatar;
    }

    public override void SetPosition(Vector3 pos)
    {
	    base.SetPosition( pos );
        GameProcedure.m_bNeedFreshMinimap = true;
    }

    public override void Tick()
    {
        Tick_AutoOperator();
        base.Tick();
        
    }
    void Tick_AutoOperator()
    {
        if ( isRiding )//已经骑上了坐骑，继续之前的移动操作
        {
            if (Ride || ridingTime > 2000)//超过2秒就继续
            {
                isRiding = false;
                ridingTime = 0;
                if (mContinueMove)
                {
                    mContinueMove = false;
                    ((CAI_MySelf)CharacterLogic_GetAI()).Enter_Move(mDest.x, mDest.z, mAutoMove);
                }
            }
            else
            {
                ridingTime += GameProcedure.s_pTimeSystem.GetDeltaTime();
            }

        }
        if (isUnRiding && Ride == false)//已经下了坐骑，继续之前使用物品的操作
        {
            isUnRiding = false;
            if (mContinueUseItem)
            {
                mContinueUseItem = false;
                Interface.GameInterface.Instance.PacketItem_UserItem(mActionItem, mTargetServerID, mFvPos);
            }
        }
    }
    int m_nSimulantLogicCount;
    public int SimulantLogicCount
    {
        get { return m_nSimulantLogicCount; }
        set { m_nSimulantLogicCount = value; }
    }

    public int GetRelationOther(CObject_Character pCharObj)
    {
        if (pCharObj.GetCharacterType() != CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            return -1;
        }
            _CAMP_DATA playerCampData = GetCharacterData().Get_CampData();
            
            // 目前只有和平全体组队模式 [8/19/2011 edit by ZL]
            switch ( playerCampData.m_nPKModeID ) {
                case (byte)PK_MODE.PK_MODE_PEACE: 
                    return 1;
                case (byte)PK_MODE.PK_MODE_GOODBAD: 
                    return 0;
                case (byte)PK_MODE.PK_MODE_PERSONAL: 
                    return 0;
                case (byte)PK_MODE.PK_MODE_GUILD: 
        //			return pCharObj->GetCharacterData()->Get_Guild() == 
        //				CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_Guild() ? 0 : 1;
                    return 0;
                case (byte)PK_MODE.PK_MODE_TEAM:
                    //todo
//                     CTeamOrGroup playerTeam = null;
//                     playerTeam = CUIDataPool::GetMe()->GetTeamOrGroup();
//                     for ( int i = 0; i < playerTeam->GetTeamMemberCount(1); ++i ) {
//                         if ( playerTeam->GetMemberByIndex(i, 1)->m_szNick && strcmp(playerTeam->GetMemberByIndex(i, 1)->m_szNick,
//                                     pCharObj->GetCharacterData()->Get_Name()) == 0 ) {
//                             return 1;
//                         }
//                     }
                    return 0;
                case (byte)PK_MODE.PK_MODE_FAMILY: 
                    return 0;
                default: return 1;
            }
        return -1;
    }

    public void Player_UseSkill(int idSkill)
    {
        CActionItem pAction = (CActionItem)CActionSystem.Instance.GetAction_SkillID(idSkill);
        if (pAction == null)
            return;

        //取得技能数据
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)pAction.GetImpl();
        if (pSkill == null) return;

        CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();

        //发送消息
        SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
        cmdTemp.SetValue(0, pSkill.m_pDefine.m_nID);
        cmdTemp.SetValue(1, pSkill.m_nLevel);
        cmdTemp.SetValue(2, MacroDefine.INVALID_ID);
        cmdTemp.SetValue(3, -1.0f);
        cmdTemp.SetValue(4, -1.0f);
        cmdTemp.SetValue(5, -1.0f);
        cmdTemp.SetValue(6, MacroDefine.INVALID_GUID);
        pMySelfAI.PushCommand(cmdTemp);

        //发送事件
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, idSkill);
    }

    public void Player_UseSkill(int idSkill, int idObj)
    {
        //判断是否为合法的目标技能
        CActionItem pAction = (CActionItem)CActionSystem.Instance.GetAction_SkillID(idSkill);
        if (pAction == null)
            return;

        //取得技能数据
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)pAction.GetImpl();
        if (pSkill == null) return;


        //检测目标是否合法

        //	if(!(pSkill->IsValidTarget(idObj))) return;


        CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();

        //发送消息
        SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
        cmdTemp.SetValue(0, pSkill.m_pDefine.m_nID);
        cmdTemp.SetValue(1, pSkill.m_nLevel);
        cmdTemp.SetValue(2, idObj);
        cmdTemp.SetValue(3, -1.0f);
        cmdTemp.SetValue(4, -1.0f);
        cmdTemp.SetValue(5, -1.0f);
        cmdTemp.SetValue(6, MacroDefine.INVALID_GUID);
        pMySelfAI.PushCommand(cmdTemp);

        //发送事件
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, idSkill);
    }

    public void Player_UseSkill(int idSkill, float fDir)
    {
        //判断是否为合法的范围技能
        CActionItem pAction = (CActionItem)CActionSystem.Instance.GetAction_SkillID(idSkill);
        if (pAction != null)
            return;

        //取得技能数据
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)pAction.GetImpl();
        if (pSkill != null) return;


        //检测目标是否合法

        //	if(!(pSkill->IsValidTarget(idObj))) return;


        CAI_Base pMySelfAI = CharacterLogic_GetAI();

        //发送消息
        SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
        cmdTemp.SetValue(0, pSkill.m_pDefine.m_nID);
        cmdTemp.SetValue(1, pSkill.m_nLevel);
        cmdTemp.SetValue(2, MacroDefine.INVALID_ID);
        cmdTemp.SetValue(3, -1.0f);
        cmdTemp.SetValue(4, -1.0f);
        cmdTemp.SetValue(5, fDir);
        cmdTemp.SetValue(6, -1.0f);
        cmdTemp.SetValue(7, MacroDefine.INVALID_GUID);
        pMySelfAI.PushCommand(cmdTemp);

        //发送事件
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, idSkill);

    }

    public void Player_UseSkill(int idSkill, uint guid)
    {

        CActionItem pAction = (CActionItem)CActionSystem.Instance.GetAction_SkillID(idSkill);
        if (pAction != null)
            return;

        //取得技能数据
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)pAction.GetImpl();
        if (pSkill != null) return;

        CAI_Base pMySelfAI = CharacterLogic_GetAI();

        //发送消息
        SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;

        cmdTemp.SetValue(0, pSkill.m_pDefine.m_nID);
        cmdTemp.SetValue(1, pSkill.m_nLevel);
        cmdTemp.SetValue(2, MacroDefine.INVALID_ID);
        cmdTemp.SetValue(3, -1.0f);
        cmdTemp.SetValue(4, -1.0f);
        cmdTemp.SetValue(5, -1.0f);
        cmdTemp.SetValue(6, guid);
        pMySelfAI.PushCommand(cmdTemp);

        //发送事件
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, idSkill);
    }

    public override void FillMouseCommand_Left(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        if(pActiveSkill != null && pActiveSkill.GetType() == ACTION_OPTYPE.AOT_ITEM)
		{
			//鼠标上挂的是一个物品
			base.FillMouseCommand_Left(ref pOutCmd, pActiveSkill);
		}
		else
		{
			pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
		}
		//pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_SELECT;
       // pOutCmd.SetValue(0, ServerID);
    }

    public override void OnDataChanged_Die()
    {
        base.OnDataChanged_Die();
        if (GetCharacterData().IsDie())
        {
            //进入死亡效果
            //CGameProcedure::s_pGfxSystem->Scene_SetPostFilter_Death(true);
        }
        else
        {
            // CGameProcedure::s_pGfxSystem->Scene_SetPostFilter_Death(FALSE);
        }
    }
    public override void OnDataChanged_Level()
    {
        //Talk::s_Talk.SetTalkRule(); todo
        GetCharacterData().UpdateAllSkillState();
        GetCharacterData().UpdateAllSkillClassState();
        
    }
    public override void OnDataChanged_RaceID()
    {
        base.OnDataChanged_RaceID();
        m_pAvatar.GetCharacterData().Set_RaceID(GetCharacterData().Get_RaceID());
        m_pAvatar.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
    }
    public override void OnDataChanged_HairMesh()
    {
        base.OnDataChanged_HairMesh();
        m_pAvatar.GetCharacterData().Set_HairMesh(GetCharacterData().Get_HairMesh());
        m_pAvatar.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
    }
    public override void OnDataChanged_FaceMesh()
    {
        base.OnDataChanged_FaceMesh();
        m_pAvatar.GetCharacterData().Set_FaceMesh(GetCharacterData().Get_FaceMesh());
        m_pAvatar.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
    }
    public override void OnDataChanged_HairColor()
    {
        base.OnDataChanged_HairColor();
        m_pAvatar.GetCharacterData().Set_HairColor(GetCharacterData().Get_HairColor());
    }
    public override void OnDataChanged_Equip(HUMAN_EQUIP point)
    {
        base.OnDataChanged_Equip(point);
        m_pAvatar.GetCharacterData().Set_EquipGem(point, GetCharacterData().Get_EquipGem(point));
        m_pAvatar.GetCharacterData().Set_Equip(point, GetCharacterData().Get_Equip(point));
        m_pAvatar.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
    }
    public override void Release()
    {
        base.Release();
        CObjectManager.Instance.DestroyObject(m_pAvatar);
        m_pAvatar = null;
    }

    const int RideSkill = 21;//骑马技能ID：21
    //获取坐骑技能id
	int getRideSkillID()
    {
        if (GetCharacterData() == null)
        {
            return -1;
        }
        int RideSkillID = -1;
        System.Collections.Generic.Dictionary<int, SCLIENT_SKILL> skills = GetCharacterData().Get_Skill();
        foreach (int k in skills.Keys)
        {
            SCLIENT_SKILL skill = skills[k];
            if (skill.m_pDefine != null && skill.m_pDefine.m_nID == RideSkill)
            {
                RideSkillID = k;
                break;
            }
        }
        return RideSkillID;
    }
    bool isRiding = false;
    bool isUnRiding = false;
    uint ridingTime = 0;//乘坐骑花费的时间
    //上下坐骑
    public bool Ride
    {
        set
        {
            //当前是否骑马
            bool currRide = ( m_nMountModelID != MacroDefine.INVALID_ID );
            if (currRide != value)
            {
                int rideSkillID = getRideSkillID();
                if ( rideSkillID  != -1)
                {
                    Player_UseSkill(rideSkillID);
                }
                if (value)
                {
                    ridingTime = 0;
                    isRiding = true;
                }
                else
                    isUnRiding = true;
            }

        }
        get
        {
            return m_nMountModelID != MacroDefine.INVALID_ID;
        }
    }

    UnityEngine.Vector3 mDest;//保存上坐骑之前请求的移动目标
    bool mAutoMove = false;
    bool mContinueUseItem = false;
    CActionItem_Item mActionItem;//保存下坐骑前请求的使用物品
    int mTargetServerID;
    UnityEngine.Vector2 mFvPos;
    //下坐骑使用物品
    public bool UnRideUseItem(CActionItem_Item ActionItemm, int targetServerID, UnityEngine.Vector2 fvPos)
    {
        if (isUnRiding)
        {
            return true;
        }
        if (Ride)
        {
            mActionItem = ActionItemm;
            mTargetServerID = targetServerID;
            mFvPos = fvPos;
            Ride = false;
            mContinueUseItem = true;//下坐骑后使用物品
            return true;
        }
        return false;
    }
    const float LengthNeedRide = 20.0f;//移动超过这个距离需要上坐骑
    bool mContinueMove = false;
    //使用坐骑移动
    public bool RideToMove(UnityEngine.Vector3 Dest, bool autoMove)   
    {
        if (CDataPool.Instance.UserEquip_GetItem(HUMAN_EQUIP.HEQUIP_RIDER) == null)
        {
            return false;
        }
        if (isRiding)
        {
            return true;
        }
        if (Ride ==false)
        {
            UnityEngine.Vector2 vec = new UnityEngine.Vector2(mPosition.x - Dest.x, mPosition.z - Dest.z);
            if (vec.magnitude > LengthNeedRide)
            {
                Ride = true;
                mContinueMove = true;
                mDest = Dest;
                mAutoMove = autoMove;
                return true;
            }
        }

        return false;
    }

    public bool IsAutoFight()
    {
        CAI_MySelf mySelfAI = m_pTheAI as CAI_MySelf;
        return mySelfAI.AutoFight == 1;
    }
    public override void  NotifyPhyEvent(PHY_EVENT_ID eventid, object pParam)
    {
	    switch(eventid)
	    {
	    case PHY_EVENT_ID.PE_OBJECT_BEGIN_MOVE:
		    {
			  //  GameProcedure.s_pGfxSystem.Camera_BeginTrace();
		    }
		    break;
	    case PHY_EVENT_ID.PE_OBJECT_TURN_AROUND:
		    {

		    }
		    break;
	    default:
		    {
			    base.NotifyPhyEvent(eventid, pParam);
		    }
		    break;
	    }
	    return;
    }
}
