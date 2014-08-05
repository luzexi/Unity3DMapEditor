using System.Collections;
using DBSystem;

public class CObject_PlayerOther : CObject_PlayerNPC
{
    //返回角色类型
    public override CHARACTER_TYPE GetCharacterType() { return CHARACTER_TYPE.CT_PLAYEROTHER; }

    public override void Initial(object pInit)
    {
        base.Initial(pInit);
        SetWeaponType(ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE);
    }
    public override void OnDataChanged_Equip(HUMAN_EQUIP point)
    {
	    UpdateEquip(point);
    }
    public override void OnDataChanged_EquipVer()
    {
	    //发送装备请求
	    SendEuipRequest();
    }
    public virtual void OnDataChanged_FaceMesh()
    {
        UpdateFaceMesh();
    }

    public virtual void OnDataChanged_HairMesh()
    {
        UpdateHairMesh();
    }
    public virtual void  OnDataChanged_HairColor()
    {
        //todo
        //UpdateHairColor();
    }
    //发出请求更详细信息的请求
    public virtual void SendEuipRequest()
    {
	    if(ServerID == -1) return;

        Network.Packets.CGCharAskEquipment msg = new Network.Packets.CGCharAskEquipment();
	    msg.setObjID((uint)ServerID);

	    NetManager.GetNetManager().SendPacket(msg);
    }

    protected override int getCharModelID()
    {
        //经过变身
        if (GetCharacterData().Get_ModelID() != -1)
        {
            LogManager.Log("主角变身：" + GetCharacterData().Get_ModelID());
            return GetCharacterData().Get_ModelID();
        }
        else
        {
            if (m_pCharRace != null)
                return m_pCharRace.nModelID;
            else
                return MacroDefine.INVALID_ID;
        }
    }
    public override void CreateRenderInterface()
    {
        base.CreateRenderInterface();
        if (GetCharacterData().Get_ModelID() == MacroDefine.INVALID_ID)//如果没有变身则换装
        {
            //重新刷新装备
            UpdateEquip(HUMAN_EQUIP.HEQUIP_WEAPON);
            UpdateEquip(HUMAN_EQUIP.HEQUIP_CAP);
            UpdateEquip(HUMAN_EQUIP.HEQUIP_ARMOR);
            UpdateEquip(HUMAN_EQUIP.HEQUIP_CUFF);
            UpdateEquip(HUMAN_EQUIP.HEQUIP_BOOT);
            UpdateEquip(HUMAN_EQUIP.HEQUIP_SASH);
            UpdateEquip(HUMAN_EQUIP.HEQUIP_BACK);

            UpdateFaceMesh();
            UpdateHairMesh();
        }

        SendEuipRequest();
    }
    protected void UpdateFaceMesh()
    {
		return;//暂时不换脸
	    if(mRenderInterface != null)
	    {
		    int nFaceMeshID = GetCharacterData().Get_FaceMesh();
		    if(nFaceMeshID == MacroDefine.INVALID_ID)
		    {
			    nFaceMeshID = m_pCharRace.nDefHeadGeo;
		    }
		    if(nFaceMeshID != MacroDefine.INVALID_ID)
		    {
			    //-----------------------------------------------------------
			    //角色头部模型数据库
			    //查找相应记录
                _DBC_CHAR_HEAD_GEO pFaceGeo =  CDataBaseSystem.Instance.GetDataBase<_DBC_CHAR_HEAD_GEO>((int)DataBaseStruct.DBC_CHAR_HEAD_GEO).Search_Index_EQU(nFaceMeshID);
			    if(pFaceGeo != null)
			    {
                    mRenderInterface.changePart(sFacePart, pFaceGeo.pMeshFile);
			    }
			    //如果是UI物体，重新设置VisibleFlag
// 			    if(GetFakeObjectFlag())
// 			    {
// 				    mRenderInterface->Actor_SetUIVisibleFlag();
// 			    }
		    }
	    }
    }

    protected void UpdateHairMesh()
    {
	    if(mRenderInterface !=null)
	    {
		    int nHairMesh = GetCharacterData().Get_HairMesh();
		    if(nHairMesh == MacroDefine.INVALID_ID)
		    {
			    nHairMesh = m_pCharRace.nDefHairGeo;
		    }
		    if(nHairMesh != MacroDefine.INVALID_ID)
		    {
			    //-----------------------------------------------------------
			    //角色头发模型数据库
			    //查找相应记录
                _DBC_CHAR_HAIR_GEO pHairMesh = CDataBaseSystem.Instance.GetDataBase<_DBC_CHAR_HAIR_GEO>((int)DataBaseStruct.DBC_CHAR_HAIR_GEO).Search_Index_EQU(nHairMesh);
			    if(pHairMesh != null)
			    {
				    mRenderInterface.changePart((int)HUMAN_EQUIP.HEQUIP_CAP, pHairMesh.pMeshFile);
			    }
			    //如果是UI物体，重新设置VisibleFlag
// 			    if(GetFakeObjectFlag())
// 			    {
// 				    mRenderInterface->Actor_SetUIVisibleFlag();
// 			    }
		    }
	    }
    }

    protected void UpdateEquip(HUMAN_EQUIP point)
    {
    	CCharacterData pCharacterData = GetCharacterData();
 	    if(pCharacterData == null)
 	    {
 		    return ;
 	    }
     
 	    //生成一个临时的Item对象
        int nID = pCharacterData.Get_Equip(point);
//  	    CObject_Item_Equip pTempEquip = null;
//  	    
//  	    if(nID != MacroDefine.INVALID_ID)
//  	    {
//  		    pTempEquip = (CObject_Item_Equip)CObject_Item.NewItem(nID);
 //               visualID = pTempEquip.GetVisualID();
//  	    }
        //因为和物品相关的代码还没有完成，暂时直接访问数据库  [1/4/2012 ZZY]
		DBC.COMMON_DBC<_DBC_ITEM_EQUIP_BASE> equipBaseDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_EQUIP_BASE>((int)DataBaseStruct.DBC_ITEM_EQUIP_BASE);
		
		_DBC_ITEM_EQUIP_BASE pEquip = equipBaseDBC.Search_Index_EQU(nID);
 	    //如果是装备根据装配点装备物品
 	    bool result = false;
        if (/*pTempEquip != null && ITEM_CLASS.ICLASS_EQUIP == pTempEquip.GetItemClass()*/pEquip!= null && pEquip.nVisualID != MacroDefine.INVALID_ID)
 	    {
			if(point == HUMAN_EQUIP.HEQUIP_WEAPON)
			{
				if(m_bHideWeapon == true && mRenderInterface != null)//如果当前正隐藏武器，那么不装备武器 [2012/4/18 ZZY]
				{
					    mRenderInterface.changeRightWeapon("");
            			mRenderInterface.changeLeftWeapon("");
					   SetWeaponType(ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE);
				}
				else
				{
					result = EquipItem_BodyLocator(/*pTempEquip.GetItemType()*/point, pEquip.nVisualID);
               	 	SetWeaponType((ENUM_WEAPON_TYPE)pEquip.nType);
				}
			}
			else
			{
                result = EquipItem_BodyPart(point, pEquip.nVisualID);
			}

 	    }
 	    else
 	    {
 		    //卸载装备
 		    UnEquipItem(point);
 	    }
 	    //  [5/6/2011 ivan edit]
 	    if (result == false)
 		    UnEquipItem(point);
    
        
 	    //CObject_Item.DestroyItem(pTempEquip);
    }

bool EquipItem_BodyPart(HUMAN_EQUIP nPart, int nID)
{
	//-----------------------------------------------------------
	//渲染层设置
	if( mRenderInterface == null )
	{
		return false;
	}

	//-----------------------------------------------------------
	//角色装备类数据库
    _DBC_ITEM_VISUAL_CHAR pEquipVisual = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_VISUAL_CHAR>((int)DataBaseStruct.DBC_ITEM_VISUAL_CHAR).Search_Index_EQU(nID);
	if(pEquipVisual==null)
	{
		return false;
	}

	//种族(男，女)
	int nRaceID = GetCharacterData().Get_RaceID();

	// 种族编号规则 [12/13/2010 ivan edit]
	// 奇数为男，偶数为女
	nRaceID = nRaceID % 2;

	if(nRaceID < 0 || nRaceID >= DBC_DEFINE.CHAR_RACE_NUM)
	{
		return false;
	}
	string szLocator="";
	switch(nPart)
	{
        case HUMAN_EQUIP.HEQUIP_CAP:				//帽子
		{
			// capmesh . headmesh . hairmesh [4/26/2011 ivan edit]
			mRenderInterface.changePart(
                (int)HUMAN_EQUIP.HEQUIP_CAP, pEquipVisual.pVisualEntity[nRaceID * 2 + 0]);
		}
		break;
        case HUMAN_EQUIP.HEQUIP_ARMOR:				//身体
		{
            mRenderInterface.changePart(
                (int)HUMAN_EQUIP.HEQUIP_ARMOR, pEquipVisual.pVisualEntity[nRaceID * 2 + 0]);
		}
		break;
        case HUMAN_EQUIP.HEQUIP_CUFF:				//手臂
		{
            mRenderInterface.changePart(
                (int)HUMAN_EQUIP.HEQUIP_CUFF, pEquipVisual.pVisualEntity[nRaceID * 2 + 0]);
		}
		break;
        case HUMAN_EQUIP.HEQUIP_BOOT:				//鞋
		{
            mRenderInterface.changePart(
                (int)HUMAN_EQUIP.HEQUIP_BOOT, pEquipVisual.pVisualEntity[nRaceID * 2 + 0]);
		}
		break;
        case HUMAN_EQUIP.HEQUIP_BACK:				//背部
		{
            mRenderInterface.changePart(
                (int)HUMAN_EQUIP.HEQUIP_BACK, pEquipVisual.pVisualEntity[nRaceID * 2 + 0]);

		}
		break;
        case HUMAN_EQUIP.HEQUIP_SASH:				// 增加肩甲换装 [4/26/2011 ivan edit]
		{
            mRenderInterface.changePart(
                (int)HUMAN_EQUIP.HEQUIP_SASH, pEquipVisual.pVisualEntity[nRaceID * 2 + 0]);
		}
		break;
	}
// 	//更新特效
// 	if(szLocator != NULL)
// 	{
// 		//-强化特效
// 		UINT uGemType = GetCharacterData().Get_EquipGem(nPart);
// 		int nStrengthenIndex = GetCharacterData().Get_EquipStrengthenEffect(nPart);
// 		int nGemEffectIndex = GetCharacterData().Get_EquipGemEffect(nPart);
// 		mRenderInterface.DelEffect(szLocator);
// 		if (uGemType != 0)
// 		{
// 			//强化特效
// 			if (nStrengthenIndex > 0)
// 			{
// 				if(pEquipVisual.pStrengthenEffectName 
// 					&& pEquipVisual.pStrengthenEffectName[nStrengthenIndex-1]
// 					&& pEquipVisual.pStrengthenEffectName[nStrengthenIndex-1][0] !='\0'
// 				)
// 				{
// 					mRenderInterface.AddEffect(pEquipVisual.pStrengthenEffectName[nStrengthenIndex-1],
// 						szLocator);
// 				}
// 			}
// 
// 			//宝石特效
// 			if(nGemEffectIndex > 0)
// 			{
// 				if(pEquipVisual.pEffectName
// 					&& pEquipVisual.pEffectName[nGemEffectIndex-1]
// 					&& pEquipVisual.pEffectName[nGemEffectIndex-1][0] !='\0' 
// 						)
// 				{
// 					mRenderInterface.AddEffect(pEquipVisual.pEffectName[nGemEffectIndex-1],
// 						szLocator);
// 				}
// 			}
// 		}
// 
// 	}
	return true;
}

bool EquipItem_BodyLocator(HUMAN_EQUIP nPart, int nID)
{
	//-----------------------------------------------------------
	//渲染层设置
	if(mRenderInterface == null)
	{
		return false;
	}

	//-----------------------------------------------------------
	//角色挂接类数据库
	//DBC_DEFINEHANDLE(s_pWeaponItem, DBC_ITEM_VISUAL_LOCATOR);
    _DBC_ITEM_VISUAL_LOCATOR pEquipVisual = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_VISUAL_LOCATOR>((int)DataBaseStruct.DBC_ITEM_VISUAL_LOCATOR).Search_Index_EQU(nID); 
	if(pEquipVisual == null)
	{
		return false;
	}

	//  [5/16/2011 Sun]
	int nGender = GetCharacterData().Get_Gender();

	//右手
	int nOjbFileIndex = nGender*DBC_DEFINE.CHAR_RACE_NUM;
	int nMatFileIndex = nOjbFileIndex + 1;

	bool rightWeapon = pEquipVisual.pVisvalLocatorR[nOjbFileIndex] != "" && pEquipVisual.pVisvalLocatorR[nOjbFileIndex][0] != '\0';
	bool leftWeapon = pEquipVisual.pVisvalLocatorL[nOjbFileIndex]!="" && pEquipVisual.pVisvalLocatorL[nOjbFileIndex][0] != '\0';

	if(rightWeapon)
	{
		mRenderInterface.changeRightWeapon(pEquipVisual.pVisvalLocatorR[nOjbFileIndex]);
// 		uint uGemType = GetCharacterData().Get_EquipGem(HEQUIP_WEAPON);
// 		int nStrengthenIndex = GetCharacterData().Get_EquipStrengthenEffect(HEQUIP_WEAPON);
// 		int nGemEffectIndex = GetCharacterData().Get_EquipGemEffect(HEQUIP_WEAPON);
// 
//  		if(0 != uGemType)
//  		{
// 			// 强化特效 [10/25/2011 Sun]
// 			if(nStrengthenIndex > 0  
// 				&& pEquipVisual.pStrendthenEffectName[nStrengthenIndex-1]
// 				&& pEquipVisual.pStrendthenEffectName[nStrengthenIndex-1][0] != '\0')
// 			{
// 			
// 				mRenderInterface.setRightWeaponEffect(pEquipVisual.pStrendthenEffectName[nStrengthenIndex-1]);
// 			}
// 			else
// 				mRenderInterface.setRightWeaponEffect("");
//  		}
//  		else
// 		{
// 			if(pEquipVisual.pEffectName && pEquipVisual.pEffectName[0] != '\0')
// 			{
// 				mRenderInterface.SetRightWeaponEffect('\0',0);
// 			}
// 			mRenderInterface.SetRightWeaponEffect('\0', szIDSTRING_STRENTHEND_LOCSTOR, 0);
// 			mRenderInterface.SetRightWeaponEffect('\0', szIDSTRING_GEM_LOCATOR, 0);
// 		}
	}
	else
	{
		mRenderInterface.changeRightWeapon("");
	}

	//左手
	if(leftWeapon)
	{
		mRenderInterface.changeLeftWeapon(pEquipVisual.pVisvalLocatorL[nOjbFileIndex]);

// 		uint uGemType = GetCharacterData().Get_EquipGem(HEQUIP_WEAPON);
// 		int nStrengthenIndex = GetCharacterData().Get_EquipStrengthenEffect(HEQUIP_WEAPON);
// 		int nGemEffectIndex = GetCharacterData().Get_EquipGemEffect(HEQUIP_WEAPON);
//  		if(0 != uGemType)
//  		{
// 			// 强化特效 [10/25/2011 Sun]
// 			if(nStrengthenIndex > 0 
// 				&& pEquipVisual.pStrendthenEffectName[nStrengthenIndex - 1]
// 				&& pEquipVisual.pStrendthenEffectName[nStrengthenIndex - 1][0] != '\0'
// 			)
// 			{
// 				mRenderInterface.SetLeftWeaponEffect(pEquipVisual.pStrendthenEffectName[nStrengthenIndex - 1], szIDSTRING_STRENTHEND_LOCSTOR, 0xFFFFFFFF);
// 			}
// 			else
// 				mRenderInterface.SetLeftWeaponEffect('\0', szIDSTRING_STRENTHEND_LOCSTOR, 0);
// 			//宝石特效
// 			if(nGemEffectIndex > 0 
// 				&& pEquipVisual.pEffectName[nGemEffectIndex - 1]
// 				&& pEquipVisual.pEffectName[nGemEffectIndex - 1][0] != '\0'
// 			)
// 				mRenderInterface.SetLeftWeaponEffect(pEquipVisual.pEffectName[nGemEffectIndex-1], szIDSTRING_GEM_LOCATOR, 0xFFFFFFFF);
// 			else
// 				mRenderInterface.SetLeftWeaponEffect('\0', szIDSTRING_GEM_LOCATOR, 0);
//  		}
//  		else
// 		{
// 			if(pEquipVisual.pEffectName && pEquipVisual.pEffectName[0] != '\0')
// 			{
// 				mRenderInterface.SetLeftWeaponEffect('\0',0);
// 			}
// 			mRenderInterface.SetRightWeaponEffect('\0', szIDSTRING_STRENTHEND_LOCSTOR, 0);
// 			mRenderInterface.SetRightWeaponEffect('\0', szIDSTRING_GEM_LOCATOR, 0);
// 		}
	}
	else
	{
		mRenderInterface.changeLeftWeapon("");
	}

    FreshAnimation();//刚刚装备上武器时，需要刷新下动作，动作由武器决定 [2012/4/26 ZZY]
	return true;
}
public void UnEquipItem(HUMAN_EQUIP nPart)
{
    if (nPart == HUMAN_EQUIP.HEQUIP_WEAPON)
	{
        SetWeaponType(ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE);
	}

	if(mRenderInterface == null)
	{
		return;
	}

	switch(nPart)
	{
        case HUMAN_EQUIP.HEQUIP_WEAPON:
            mRenderInterface.changeRightWeapon("");
            mRenderInterface.changeLeftWeapon("");
            if (m_bHideWeapon == false)
            {
                EquipItem_BodyLocator(HUMAN_EQUIP.HEQUIP_WEAPON, m_pCharRace.nDefRWeapon);
            }
                
		break;

	case HUMAN_EQUIP.HEQUIP_CAP:
        UpdateHairMesh();
		break;
    case HUMAN_EQUIP.HEQUIP_BACK:
		{
		}
		break;

    case HUMAN_EQUIP.HEQUIP_ARMOR:
		//设置缺省装备
        EquipItem_BodyPart(HUMAN_EQUIP.HEQUIP_ARMOR, m_pCharRace.nDefBody);		//衣服
		break;

    case HUMAN_EQUIP.HEQUIP_CUFF:
		//设置缺省装备
        EquipItem_BodyPart(HUMAN_EQUIP.HEQUIP_CUFF, m_pCharRace.nDefArm);		//手
		break;

    case HUMAN_EQUIP.HEQUIP_BOOT:
		//设置缺省装备
        EquipItem_BodyPart(HUMAN_EQUIP.HEQUIP_BOOT, m_pCharRace.nDefFoot);		//脚
		break;

    case HUMAN_EQUIP.HEQUIP_SASH:
        mRenderInterface.changePart((int)HUMAN_EQUIP.HEQUIP_SASH, "");
		break;
    case HUMAN_EQUIP.HEQUIP_RING:
    case HUMAN_EQUIP.HEQUIP_NECKLACE:
// 	case HUMAN_EQUIP.HEQUIP_JADE:
// 	case HUMAN_EQUIP.HEQUIP_AMULET:
		//Not Care...
		break;
	default:
		break;
	}
}

public override void UpdateCharRace()
{
	m_pCharRace = null;
	CCharacterData pCharacterData = GetCharacterData();
	if(pCharacterData != null && pCharacterData.Get_RaceID() != MacroDefine.INVALID_ID)
	{
		DBC.COMMON_DBC<_DBC_CHAR_RACE> raceDBC = 	CDataBaseSystem.Instance.GetDataBase<_DBC_CHAR_RACE>((int)DataBaseStruct.DBC_CHAR_RACE);
		
		if(raceDBC !=null)
		{
			 m_pCharRace = raceDBC.Search_Index_EQU(pCharacterData.Get_RaceID());
			 if(m_pCharRace != null)
             	LogManager.Log("Race ID:" + pCharacterData.Get_RaceID().ToString() + "Model ID:" + m_pCharRace.nModelID.ToString());
		}
	}
}
//显示/隐藏武器
protected override void OnHideWeapon(int nAppointedWeaponID)
{
    if (nAppointedWeaponID != MacroDefine.INVALID_ID)
    {
        EquipItem_BodyLocator(HUMAN_EQUIP.HEQUIP_WEAPON, nAppointedWeaponID);
    }
    else
    {
        //卸载装备
        UnEquipItem(HUMAN_EQUIP.HEQUIP_WEAPON);
    }
}
protected override void OnShowWeapon() 
{
    UpdateEquip(HUMAN_EQUIP.HEQUIP_WEAPON);
}
public override uint			GetIdleInterval()
{
    if(m_pCharRace != null && m_pCharRace.nIdleInterval > 0)
	{
		return (uint)m_pCharRace.nIdleInterval * 1000;
	}
	else
	{
		return MacroDefine.UINT_MAX;
	}
}
protected override RC_RESULT OnCommand(SCommand_Object pCmd)
{
    RC_RESULT rcResult = RC_RESULT.RC_SKIP;

	switch ((OBJECTCOMMANDDEF)pCmd.m_wID )
	{
	case OBJECTCOMMANDDEF.OC_UPDATE_EQUIPMENT:
		{
			uint	dwModifyFlags	= pCmd.GetValue<uint>(0);
			uint[]  pData			= pCmd.GetValue<uint[]>(1);
			int	i;
            uint    dwIndex =0;
			for ( i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++ )
			{
				if ( (dwModifyFlags & (1<<i)) != 0 )
				{
					GetCharacterData().Set_Equip((HUMAN_EQUIP)i, (int)(pData[dwIndex]));
					dwIndex++;
				}
			}
            rcResult = RC_RESULT.RC_OK;
		}
		break;
 	case OBJECTCOMMANDDEF.OC_UPDATE_TEAM_FLAG:
 		{
 			if(this != CObjectManager.Instance.getPlayerMySelf())
 			{
 				CCharacterData pData = GetCharacterData();
 				pData.Set_HaveTeamFlag( pCmd.GetValue<byte>(0) == 1 );
 				pData.Set_TeamLeaderFlag( pCmd.GetValue<byte>(1) == 1 );
 				pData.Set_TeamFullFlag( pCmd.GetValue<byte>(2) == 1 );
 			}
     		rcResult = RC_RESULT.RC_OK;
 		}
 		break;
// 	case OBJECTCOMMANDDEF.OC_TEAM_FOLLOW_MEMBER:
// 		{
// 			CCharacterData* pData = GetCharacterData();
// 
// 			pData->Set_TeamFollowListCount( pCmd->m_adwParam[0] );
// 
// 			for( INT i=0; i<(INT)(pCmd->m_adwParam[0]); ++i )
// 			{
// 				pData->Set_TeamFollowedMember(i, pCmd->m_adwParam[i+1]);
// 			}
// 
    // 			rcResult = RC_RESULT.RC_OK;
// 		}
// 		break;
// 	case OBJECTCOMMANDDEF.OC_UPDATE_TEAM_FOLLOW_FLAG:
// 		{
// 			CCharacterData* pData = GetCharacterData();
// 
// 			pData->Set_TeamFollowFlag( pCmd->m_abParam[0] );
// 			CharacterLogic_GetAI()->OnAIStopped();
// 
    // 			rcResult = RC_RESULT.RC_OK;
// 		}
// 		break;
	default:
		return base.OnCommand(pCmd);
	}

	return rcResult;
}
protected _DBC_CHAR_RACE m_pCharRace;
protected static int sFacePart = -1;//装备脸部模型所需的索引
}
