using UnityEngine;
using System;
using System.Collections.Generic;
using DBSystem;
using GFX;

//角色obj的类型
public enum CHARACTER_TYPE
{
    CT_INVALID = -1,

    CT_PLAYERMYSELF,		//玩家自己
    CT_PLAYEROTHER,			//其他玩家
    CT_MONSTER,				//怪物/NPC
    CT_SpecialBus			// 载具 [8/22/2011 ivan edit]
}

//当前角色类Object的逻辑状态
public enum ENUM_CHARACTER_LOGIC
{
    CHARACTER_LOGIC_INVALID = -1,
    CHARACTER_LOGIC_IDLE,
    CHARACTER_LOGIC_ACTION,
    CHARACTER_LOGIC_MOVE,
    CHARACTER_LOGIC_SKILL_GATHER,
    CHARACTER_LOGIC_SKILL_LEAD,
    CHARACTER_LOGIC_SKILL_SEND,
    CHARACTER_LOGIC_ABILITY_ACTION,
    CHARACTER_LOGIC_DEAD,
    CHARACTER_LOGIC_STALL,
    CHARACTER_LOGIC_SKILL_JUMP_SEND,// 跳斩技能，需要移动和跳跃 [10/31/2011 Ivan edit]

    CHARACTER_LOGIC_NUMBERS
}

public class CObject_Character : CObject_Dynamic
{
    public static float sDefaultActionFuseTime = 0.3f;//默认的动作混合时间为0.3秒
    //----------------------------------------------------------
    //基本接口
    //----------------------------------------------------------
    //在某种逻辑状态下的参数
    public struct SLogicParam_Idle
    {
        public uint m_uIdleInterval;				// 间隔多少时间可以随机一次休闲动作
        public uint m_uLastIdleMotionTime;			// 上一次做休闲动作的时间
        public uint m_uStartTime;					// 待命逻辑的起始时间
    };

    public struct SLogicParam_Move
    {
        public int m_nCurrentNodeIndex;			// 当前节点的索引值
        public Vector2 m_posSaveStart;					// 保存当前的起始点
        public Vector2 m_posSaveTarget;				// 保存当前的目标点
    };

    public struct SLogicParam_MagicCharge
    {
        public uint m_uCurrentTime;					// 当前进度时间
    };

    public struct SLogicParam_MagicChannel
    {
        public uint m_uCurrentTime;					// 当前进度时间
    };

    public struct SLogicParam_MagicSend
    {
        public int m_nSaveMagicID;					// 当前法术ID
        public bool m_bBeAttackEffectShowed;		// 因此次攻击所产生的被攻击者的表现是否已经表现
        public bool m_bCanBreak;					// 是否可以结束该逻辑了
        public int m_nActionIndex;					// 为了让动作按顺序播放
        public int m_nActionCount;					// 动作数量
        public bool m_bDoNextAction_Concatenation;	// 当技能的招式类型为SKILL_ACTION_TYPE_CONCATENATION时所用
        public uint m_uAnimationTime;				// 动已经播放的时间
        public uint m_uAnimationEndElapseTime;		// 动画停止的时间间隔
		public bool m_hitGrounded;//使用跳斩的时候是否已经落地了
        public ENUM_SKILL_ACTION_TYPE m_actionType;//招式类型
    };

    public struct SLogicParam_Ability
    {
        public uint m_uTotalTime;
        public uint m_uCurrentTime;
    };

    public struct SLogicParam_Sit
    {
        public bool m_bStandUp;
    };

    public struct SLogicParam_Stall
    {
        public enum ENUM_STALL_ANIMATION
        {
            STALL_ANIMATION_INVALID = -1,
            STALL_ANIMATION_SITDOWN,
            STALL_ANIMATION_IDLE,
            STALL_ANIMATION_STANDUP,

            STALL_ANIMATION_NUMBER
        };
        public int m_nCurrentAnimation;			// ENUM_STALL_ANIMATION
    };

    public struct SParam_RandomSay
    {
        public uint m_uSayInterval;				// 间隔多少时间
        public uint m_uStartTime;				// 待命逻辑的起始时间
    };

    // 跳跃相关
    public CObject_Character()
    {
        m_pTheAI = null;

        m_pInfoBoard		= null;

        ////m_pCharacterData	= null;

        ////m_uTime_LogicEventListTick	= 0;

        m_bJumping = false;
		m_CImpact_Character = new CImpact_Character(this);
		m_CLogicEvent_Character = new CLogicEvent_Character(this);

        ////m_pCurrentLogicCommand		= null;
        m_paramLogic_MagicSend.m_nSaveMagicID = -1;
        m_paramLogic_MagicSend.m_bBeAttackEffectShowed = false;
        m_paramLogic_MagicSend.m_nActionIndex = -1;

        //////m_nIdleIntervalTime = (UINT)-1;
        //////m_nLastIdleMotionTime = 0;

        m_uFightStateTime = 0;
        m_fLogic_Speed = 1.0f;


        ////m_pWlakSound = 0;
        ////m_pLifeAbilitySound = 0;

        ////m_bIsChatMoodPlaying		= false;
        ////m_bNeedStandUp				= false;
        ////m_bStanding					= false;

        m_bDropBox_HaveData			= false;
        m_nDropBox_ItemBoxID		= MacroDefine.UINT_MAX;
        m_DropBox_OwnerGUID =           MacroDefine.UINT_MAX;
        m_posDropBox_CreatePos.m_fX = -1.0f;
        m_posDropBox_CreatePos.m_fZ = -1.0f;

        //////  [9/8/2010 Sun]
        ////m_TestActionType = -1;

        ////m_CreateCreatureBoard  = true;

        //////  [8/29/2011 Ivan edit]
        ////isDriver = false;
    }

    public override void Initial(object pInit)
    {
        //////AxProfile::AxProfile_PopNode("ObjInit");
        base.Initial(pInit);
        ////m_bAnimationEnd				= false;
        m_uFightStateTime	= 0;

        SetCurrentLogicCommand(null);
        m_fLogic_Speed				= 1.0f;

        SetMapPosition(GetPosition().x, GetPosition().z);

        //////创建逻辑信息
        ////m_pCharacterData = CDataPool::GetMe().CharacterData_Create(this);

        //创建信息板
        //AxProfile::AxProfile_PushNode("ObjInit_CreateBoard");
        //if(!GetFakeObjectFlag()&& CGameProcedure::s_pUISystem&& m_CreateCreatureBoard )
        if (GetFakeObjectFlag() == false &&m_pInfoBoard == null)
        {
            m_pInfoBoard = UISystem.Instance.CreateCreatureBoard();
            m_pInfoBoard.SetElement_ObjId(ServerID);
            m_pInfoBoard.SetElement_Name("");
            m_pInfoBoard.Show(false);
        }
        //AxProfile::AxProfile_PopNode("ObjInit_CreateBoard");

        ////m_uTime_LogicEventListTick	= CGameProcedure::s_pTimeSystem.GetTimeNow();

        ////Enable(OSF_VISIABLE);
        ////Disalbe(OSF_OUT_VISUAL_FIELD);

        ////m_bDropBox_HaveData			= false;
        ////m_nDropBox_ItemBoxID		= INVALID_ID;
        ////m_DropBox_OwnerGUID			= INVALID_ID;
        ////m_posDropBox_CreatePos		= WORLD_POS(-1.f, -1.f);

        ////m_bHideWeapon = false;
        ////UpdateCharRace();
        ////UpdateCharModel();
        ////UpdateMountModel();

        SObjectInit pCharacterInit = (SObjectInit)pInit;
        if (pCharacterInit != null)
        {
            // 状态信息
            RemoveAllImpact();
        }
        Start_Idle();
        ////m_param_RandomSay.m_uStartTime = CGameProcedure::s_pTimeSystem.GetTimeNow();
        ////m_param_RandomSay.m_uSayInterval = CalcIdleRandInterval();
       

    }

    public override void Release()
    {
        ////DeleteObjectCommand(m_pCurrentLogicCommand);
        m_pCurrentLogicCommand = null;

        CleanupLogicCommandList();
        if (m_CLogicEvent_Character != null)
        {
            m_CLogicEvent_Character.RemoveAllLogicEvent();
        }


        ////if ( !m_mapImpactEffect.empty() )
        ////{
        ////    SImpactEffect *pImpactEffect;
        ////    CImpactEffectMap::iterator itCur, itEnd;
        ////    itEnd = m_mapImpactEffect.end();
        ////    for ( itCur = m_mapImpactEffect.begin(); itCur != itEnd; itCur++ )
        ////    {
        ////        pImpactEffect = (itCur.second);
        ////        if ( pImpactEffect != null )
        ////        {
        ////            delete pImpactEffect;
        ////        }
        ////    }
        ////    m_mapImpactEffect.erase( m_mapImpactEffect.begin(), itEnd );
        ////}

        //////删除逻辑信息
        ////CDataPool::GetMe().CharacterData_Destroy(this);
        ////m_pCharacterData = null;

        //////从加载队列中清除
        ////CObjectManager::GetMe().GetLoadQueue().ClearTask(GetID());

        ////ReleaseRenderInterface();
        ////ReleaseMountRenderInterface();

        if(m_pInfoBoard != null)
        {
            m_pInfoBoard.Destroy();
            m_pInfoBoard = null;
        }

        ////m_uTime_LogicEventListTick	= 0;

        //////释放声音资源
        ////CGameProcedure::s_pSoundSystem.Source_Destroy(m_pWlakSound);
        ////m_pWlakSound = 0;
        ////CGameProcedure::s_pSoundSystem.Source_Destroy(m_pLifeAbilitySound);
        ////m_pLifeAbilitySound = 0;

        ////Disalbe(OSF_RAY_QUERY);

        ////m_pCharActionSetFile		= null;
        ////m_pMountActionSetFile		= null;
        ////m_nCharModelID				= INVALID_ID;
        ////m_nMountModelID				= INVALID_ID;
        ////m_fMountAddHeight			= 0.0F;
        ////m_uFightStateTime		= 0;

        ////m_bDropBox_HaveData			= false;
        ////m_nDropBox_ItemBoxID		= INVALID_ID;
        ////m_DropBox_OwnerGUID			= INVALID_ID;
        ////m_posDropBox_CreatePos		= WORLD_POS(-1.f, -1.f);

        base.Release();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    #region LogicEvent处理

    private CLogicEvent_Character m_CLogicEvent_Character;

    public void ShowLogicEvent(int iServerObjID, int iLogicCount, bool bShowAll)
    {
        m_CLogicEvent_Character.ShowLogicEvent(iServerObjID, iLogicCount, bShowAll);
    }
    public void DoLogicEvent(_LOGIC_EVENT pLogicEvent)
    {
        m_CLogicEvent_Character.DoLogicEvent(pLogicEvent);
    }
    public void AddLogicEvent(_LOGIC_EVENT pLogicEvent)
    {
        m_CLogicEvent_Character.AddLogicEvent(pLogicEvent);
    }

    public void RemoveAllLogicEvent()
    {
        m_CLogicEvent_Character.RemoveAllLogicEvent();
    }

    #endregion LogicEvent处理

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    #region 角色附加效果类
    private CImpact_Character m_CImpact_Character;
    public void ChangeImpact(uint idImpact, uint uCreatorID, bool bEnable, bool bShowEnableEffect)
    {
        m_CImpact_Character.ChangeImpact(idImpact, uCreatorID, bEnable, bShowEnableEffect);
    }
    public void RemoveAllImpact()
    {
        m_CImpact_Character.RemoveAllImpact();
    }
    public void Tick_UpdateEffect()
    {
        m_CImpact_Character.Tick_UpdateEffect();
    }

    protected void UpdateBuffEffect()
    {
        m_CImpact_Character.UpdateBuffEffect();
    }
    public CImpact_Character  Impact
    {
        get { return m_CImpact_Character; }
    }
    #endregion 角色附加效果类

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // 对象判断
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public bool IsMySelf(int objID)
    {
        CObject pMySelf = CObjectManager.Instance.getPlayerMySelf();
        if (pMySelf != null)
        {
            return (objID == pMySelf.ServerID);
        }
        return false;
    }

    public bool IsMyPet(int objID)
    {
        PET_GUID_t[] CurrentPetGuid = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_CurrentPetGUID();
        for (int i = 0; i < GAMEDEFINE.MAX_CURRENT_PET; i++)
        {
            SDATA_PET pPet = CDataPool.Instance.Pet_GetPet(CurrentPetGuid[i]);
            if (pPet != null)
            {
                return ((uint)(objID) == pPet.idServer);
            }
        }
        //PET_GUID_t CurrentPetGuid = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_CurrentPetGUID();
        //SDATA_PET pPet = CDataPool.Instance.Pet_GetPet(CurrentPetGuid);
        //if (pPet != null)
        //{
        //    return ((uint)(objID) == pPet.idServer);
        //}
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // 跳跃相关
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetbJumping(bool bJumping) { m_bJumping = bJumping; }
    public bool GetbJumping() { return m_bJumping; }
    public bool Jump()
    {
        if (IsDie())
        {
            return false;
        }

        CCharacterData pCharacterData = GetCharacterData();
        if (pCharacterData == null)
        {
            return false;
        }

        if (pCharacterData.Get_ModelID() != MacroDefine.INVALID_ID)
        {
            return false;
        }

        if (GetCharacterType() == CHARACTER_TYPE.CT_PLAYERMYSELF || GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            //待实现组队
            //if (pCharacterData.Get_TeamFollowFlag())
            //    return false.;

            //if (pCharacterData.Get_IsInStall())
            //    return false;
        }

        int nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_JUMP_R;

        if (CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
        {
            nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_JUMP_N;
        }
        else
        {
            if (GetCharacterData().Get_MountID() == -1)
            {
                System.Random rand = new System.Random();
                int nRet = rand.Next() % 4;
                if (nRet == 0)
                    nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_JUMP_R;
                else
                    nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_JUMP_N;
            }
            else
            {
                nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_JUMP_R;
            }
        }

        ChangeAction(nBaseAction, 1.0f, false,sDefaultActionFuseTime);

        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE || GetCharacterData().Get_MountID() == -1)
        {
            SetbJumping(true);
            RegisterPhyEvent(PHY_EVENT_ID.PE_COLLISION_WITH_GROUND);
            //由物理层去计算
            PhyEnable(true);
            float fTEST = 12.0f;
            AddLinearSpeed(new Vector3(0.0f, fTEST, 0.0f));
        }
        StandUp();
        return true;
        //todo
    }
    bool m_bJumping;

    //坐下/站起
    public bool SitDown()
    { 
        //todo
/*        if(IsDie())
	    {
		    return false;
	    }

	    CCharacterData pCharacterData = GetCharacterData();
	    if(pCharacterData == null)
	    {
		    return false;
	    }

	    if(pCharacterData.Get_MountID() != MacroDefine.INVALID_ID)
	    {
		    return false;
	    }

	    if(pCharacterData.Get_ModelID() != MacroDefine.INVALID_ID)
	    {
		    return false;
	    }

	    if(GetCharacterType() == CHARACTER_TYPE.CT_PLAYERMYSELF || GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
	    {
		    if(pCharacterData.Get_TeamFollowFlag())
			    return false;

		    if(pCharacterData.Get_IsInStall())
			    return false;
	    }

	    if (CharacterLogic_Get() == CHARACTER_LOGIC_IDLE && GetCharacterData() != null)
	    {
		    if(!GetCharacterData().IsSit() && GetCharacterData().Get_MountID() == MacroDefine.INVALID_ID)
		    {
			    GetCharacterData().Set_Sit(true);
			    if(CObjectManager::GetMe().GetMySelf() == this)
			    {
				    CGCharSit cmdSit;
				    cmdSit.setObjID(GetServerID());
				    cmdSit.setSit(GetCharacterData().IsSit());

				    CNetManager::GetMe().SendPacket(&cmdSit);
			    }
		    }
	    }*/
	    return true;
    }
    public bool StandUp()
    {
        //todo
    	/*if(IsDie())
	    {
		    return false;
	    }

	    CCharacterData *pCharacterData = GetCharacterData();
	    if(pCharacterData == null)
	    {
		    return false;
	    }

	    if(pCharacterData.Get_MountID() != MacroDefine.INVALID_ID)
	    {
		    return false;
	    }

	    if(pCharacterData.Get_ModelID() != MacroDefine.INVALID_ID)
	    {
		    return false;
	    }

	    if(GetCharacterType() == CHARACTER_TYPE.CT_PLAYERMYSELF || GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
	    {
		    if(pCharacterData.Get_TeamFollowFlag())
			    return false;

		    if(pCharacterData.Get_IsInStall())
			    return false;
	    }

	    if(GetCharacterData().IsSit())
	    {
		    GetCharacterData().Set_Sit(false);
		    if(CObjectManager::GetMe().GetMySelf() == this)
		    {
			    CGCharSit cmdSit;
			    cmdSit.setObjID(GetServerID());
			    cmdSit.setSit(GetCharacterData().IsSit());

			    CNetManager::GetMe().SendPacket(&cmdSit);
		    }
	    }*/
	    return true;
    }
    //当前的逻辑状态
    protected ENUM_CHARACTER_LOGIC m_eCharacterLogic = ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE;
    //当前逻辑是否停止
    protected bool m_bCharacterLogicStopped;

    protected int m_nLastServerLogicCount;		// 最后得到的服务器端的逻辑计数
    protected int m_uLastServerTime;				// 最后得到的服务器端时间
    protected int m_nLastModifyPosLogicCount;		// 最后执行的改变位置的指令
    protected CObjectCommand_Logic m_pCurrentLogicCommand;		// 当前正在执行的指令
    protected SLogicParam_Idle m_paramLogic_Idle;
    protected  SLogicParam_Move m_paramLogic_Move;

    protected  SLogicParam_MagicCharge m_paramLogic_MagicCharge;
    protected  SLogicParam_MagicChannel m_paramLogic_MagicChannel;
    protected SLogicParam_MagicSend m_paramLogic_MagicSend;
    protected SLogicParam_Ability m_paramLogic_Ability;
    protected SLogicParam_Sit m_paramLogic_Sit;
    protected SLogicParam_Stall m_paramLogic_Stall;
    protected CAI_Base m_pTheAI;

    protected SParam_RandomSay m_param_RandomSay;
    // 得到当前的逻辑状态
    public ENUM_CHARACTER_LOGIC CharacterLogic_Get()
    {
        return m_eCharacterLogic;
    }

    public CAI_Base CharacterLogic_GetAI()
    {
        return m_pTheAI;
    }

    // 当前逻辑是否停止
    public bool CharacterLogic_IsStopped()
    {
        return m_bCharacterLogicStopped;
    }

    //返回角色类型
    public virtual CHARACTER_TYPE GetCharacterType() { return CHARACTER_TYPE.CT_INVALID; }
    //是否能够被作为主目标选择
    public override bool CanbeSelect() { return true; }
    //
    public virtual uint GetIdleInterval() { return MacroDefine.UINT_MAX; }
    public uint CalcIdleRandInterval()
    {
        uint uInterval = GetIdleInterval();
        if (uInterval == MacroDefine.UINT_MAX)
        {
            uInterval = MacroDefine.UINT_MAX;
        }
        else
        {
            uInterval = (uint)((float)(GetIdleInterval()) * ((float)(64 - (GameProcedure.s_random.Next() % 16))) / 64.0f);
        }
        return uInterval;
    }
    public bool IsDie()
    {
        if (m_pCharacterData != null)
        {
            return m_pCharacterData.IsDie();
        }
        else
        {
            return false;
        }
    }

    //掉落包事件
	protected bool			m_bDropBox_HaveData;
    protected uint m_nDropBox_ItemBoxID;
    protected uint m_DropBox_OwnerGUID;
    protected WORLD_POS m_posDropBox_CreatePos;
    public bool AddDropBoxEvent(uint idItemBox, uint idOwner, ref WORLD_POS pCreatePos)
    {
	    m_bDropBox_HaveData		= true;
	    m_nDropBox_ItemBoxID	= idItemBox;
	    m_DropBox_OwnerGUID		= idOwner;
	    m_posDropBox_CreatePos.m_fX	= pCreatePos.m_fX;
        m_posDropBox_CreatePos.m_fZ = pCreatePos.m_fZ;
	    return true;
    }

    	// 选中环大小
	public virtual float				GetProjtexRange()
    {
        return 1.0f;
    }
	// 阴影大小
	public virtual float				GetShadowRange()
    {
        return 1.0f;
    }
    //--------------------------------------------------------
	//头顶信息板相关
	//--------------------------------------------------------
	//UI接口
    public CreatureBoard m_pInfoBoard;
    //位置
    Vector2 m_fvInfoBoardPos;

    //----------------------------------------------------------
    //角色逻辑数据相关
    //----------------------------------------------------------
    //得到逻辑数据
    //角色逻辑数据
    protected CCharacterData m_pCharacterData;
    public CCharacterData GetCharacterData() { return m_pCharacterData; }

    public virtual void OnDataChanged_RaceID()
    {
        UpdateCharRace();
        UpdateCharModel();
    }
    public virtual void OnDataChanged_ModelID()
    {
        UpdateCharModel();
    }
    public virtual void OnDataChanged_MountID()
    {
        UpdateMountModel();
    }
    public virtual void OnDataChanged_Name()
    {
        if (m_pInfoBoard != null)
        {
            //设置
            m_pInfoBoard.SetElement_Name(GetNameWithColor());
            Tick_UpdateInfoBoard();
        }
    }
    //获取增加颜色后的名称
    public string GetNameWithColor()
    {
        string roleName = GetCharacterData().Get_Name();
        if (roleName.Length == 0)
            return "";

        //...TODO
        //添加临时使用颜色标示
        string nameColor = Color.white.ToString();
        switch (GetCharacterType())
        {
            case CHARACTER_TYPE.CT_PLAYERMYSELF:
                nameColor = "[#e6c8c8]";
                break;
            case CHARACTER_TYPE.CT_PLAYEROTHER:
                nameColor = "[#d1d287]";
                break;
            case CHARACTER_TYPE.CT_MONSTER:
                {
		            ENUM_RELATION eCampType = GameProcedure.s_pGameInterface.GetCampType( CObjectManager.Instance.getPlayerMySelf(), this );
                    switch (eCampType)
                    {
                        case ENUM_RELATION.RELATION_FRIEND:
                            nameColor = "[#63b95f]";
                            break;
                        case ENUM_RELATION.RELATION_ENEMY:
                            {
                                CObject_PlayerNPC monster = this as CObject_PlayerNPC;
                                if (monster != null && monster.IsBoss())
                                    nameColor = "[#b74fd9]";
                                else
                                    nameColor = "[#bd5050]";

                            }
                            break;
                        default:
                            nameColor = "[#bd5050]";
                            break;
                    }
                }
                break;
            default:
                break;
        }

        return nameColor + roleName;
    }

    public virtual void OnDataChanged_Ambit() { }
    public virtual void OnDataChanged_CurTitles() { }
    public virtual void OnDataChanged_MoveSpeed()
    {
        //     float fSpeedRate = GetCharacterData().Get_SpeedRate();
        // 
        //     if (CHARACTER_LOGIC_MOVE == CharacterLogic_Get())
        //     {
        //         //ChangeActionSpeed( fSpeedRate );
        //     }
    }
    public virtual void OnDataChanged_StallName() { }
    public virtual void OnDataChanged_IsInStall() { }
    public virtual void OnDataChanged_TeamLeaderFlag() { }
    public virtual void OnDataChanged_Die() 
    {
        if (GetCharacterData().IsDie())
        {
            if (m_CLogicEvent_Character.isEmptyLogicEvent())
            {
                LogManager.Log("Start_Dead ID"+ ServerID);
				Start_Dead(true);
            }
        }
    }
    public virtual void OnDataChanged_StealthLevel() { }
    public virtual void OnDataChanged_FightState() { }
    public virtual void OnDataChanged_Sit() { }
    public virtual void OnDataChanged_Level() { }
    public virtual void OnDataChanged_BusObjID() { }		// BUS相关接口 [8/15/2011 ivan edit]
    public ENUM_WEAPON_TYPE GetWeaponType() { return m_theWeaponType; }
    // 武器类型
    protected void SetWeaponType(ENUM_WEAPON_TYPE type) 
    { 
        m_theWeaponType = type;
        //if (GetCharacterData() != null)
        //    FreshAnimation();//更换武器类型会导致动作变化  [3/8/2012 ZZY]
    }
    //当前武器类型
    protected ENUM_WEAPON_TYPE m_theWeaponType = ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE;

    public virtual void OnDataChanged_Equip(HUMAN_EQUIP point) { }
    public virtual void OnDataChanged_EquipVer() { }

    public virtual void UpdateCharModel() { }//更新角色模型
    public virtual void UpdateMountModel() { }//更新坐骑模型
    public virtual void UpdateCharRace() { }//更新角色门派

    //刷新当前动作 由派生类PlayerNPC实现
    public virtual void FreshAnimation() { }

    //根据目标点创建行走路径
    public virtual void OnMoveStop()
    {
        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
            CharacterLogic_Stop(true);
    }
    public virtual void ChangeAction(int nSetID, float fSpeed, bool bLoop, float fFuseParam) { }//切换动作 由派生类实现
    //--------------------------------------------------------
    // 角色聊天动作相关
    //--------------------------------------------------------
    public void SetChatMoodAction(string strOrder) {/*todo*/}	//设置动作列表

    protected void ClearAllChatMoodAction() {/*todo*/}				//清除队列里剩余的动作
    protected void PlayChatMoodAction() { /*todo*/}					//每次从队列里弹出一个动作进行播放，角色动作逻辑必须处于idle状态才行
    protected bool IsHaveChatMoodAction() { /*todo*/ return false; }					// 是否有聊天动作

    protected bool IsChatMoodActionPlaying() { return m_bIsChatMoodPlaying; }
    protected void SetChatMoodActionPlaying(bool bPlaying) { m_bIsChatMoodPlaying = bPlaying; }

    protected bool m_bIsChatMoodPlaying;
    protected bool m_bNeedStandUp;
    protected bool m_bStanding;
    protected class CHAT_MOOD_DATA
    {
        int m_ActionId;
        bool m_bLoop;

        CHAT_MOOD_DATA()
        {
            m_ActionId = -1;
            m_bLoop = false;
        }
    }

    protected List<CHAT_MOOD_DATA> m_listChatMoodAction;
    uint m_uFightStateTime = 0;
    public bool IsFightState() { return m_uFightStateTime > 0; }
    public virtual void SetFightState(bool bSet)
    {
       
	    // 只有自己才提示 [8/13/2010 Sun]
        //todo
	    if (CObjectManager.Instance.getPlayerMySelf() == this
		    && IsFightState() != bSet )
	    {

		 //   int nState = OtherMsgInfo::OM_ENTER_FIGHT;
		    GAME_EVENT_ID eventID = GAME_EVENT_ID.GE_MYSELF_ENTER_FIGHTSTATE ;
		    //string strMsg = "进入战斗";
		    if(!bSet)
		    {
			  //  nState = OtherMsgInfo::OM_LEAVE_FIGHT;
			    eventID  = GAME_EVENT_ID.GE_MYSELF_LEAVE_FIGHTSTATE;
			    //strMsg = "脱离战斗";
		    }

		    // 显示提示 [8/13/2010 Sun]
		//    this.DisplayOtherMsg(nState);

		 //   ADDTALKMSG(strMsg);
    		CEventSystem.Instance.PushEvent(eventID);

		    if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)//避免跑动中切换战斗时，动作不匹配  [11/8/2011 zzy]
		    {
			    if(bSet)
			    {
				    m_uFightStateTime = ObjectDef.FIGHT_STATE_DURATION_TIME;
			    }
			    else
			    {
				    m_uFightStateTime = 0;
			    }
			    FreshAnimation();
		    }
	    }

	    if(bSet)
	    {
            m_uFightStateTime = ObjectDef.FIGHT_STATE_DURATION_TIME;
	    }
	    else
	    {
		    m_uFightStateTime = 0;
	    }
    }

    public virtual ENUM_RELATION GetCampType(CObject_Character pChar)
    {
        return ENUM_RELATION.RELATION_INVALID;
    }


    public virtual bool CannotBeAttack() { return false; }

    protected List<CObjectCommand_Logic> m_listLogicCommand = new List<CObjectCommand_Logic>();
    protected float m_fLogic_Speed = 1.0f;					// 当前逻揖执行的速度
    protected void SetLogicSpeed(float fSpeed)
    {
        if (Mathf.Abs(m_fLogic_Speed - fSpeed) > 0.1f)
        {
            m_fLogic_Speed = fSpeed;
            OnLogicSpeedChanged();
        }
    }
    protected float GetLogicSpeed() { return m_fLogic_Speed; }
    protected void OnLogicSpeedChanged()
    {
        ChangeActionSpeed(GetLogicSpeed());
    }
    protected virtual void ChangeActionSpeed(float fSpeed) { }//动作速率,由父类实现
    //设置当前的逻辑状态
    protected void CharacterLogic_Set(ENUM_CHARACTER_LOGIC eLogic)
    {
        m_bCharacterLogicStopped = false;
        m_eCharacterLogic = eLogic;

        // 清空上次保存的寻路数据 [9/1/2011 Sun]
        //if(eLogic != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE && eLogic != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
        //CWorldManager::GetMe().SetPathNodesDirty();
    }

    void SetLastModifyPosLogicCount(int nLogicCount)
    {
        m_nLastModifyPosLogicCount = nLogicCount;
    }

    //进入某一种逻辑状态
    protected bool Start_Idle()
    {
        if (m_bJumping)
        {
        }
        else if (IsHaveChatMoodAction())
        {
            PlayChatMoodAction();
        }
        else
        {
            bool bFightState, bSit;
            int nBaseAction;
            bFightState = IsFightState();
            if (GetCharacterData() != null)
                bSit = GetCharacterData().IsSit();
            else
                bSit = false;
            if (bSit)
            {
                nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_SIT_IDLE;
            }
            else
            {
                nBaseAction = (int)((bFightState) ? (ENUM_BASE_ACTION.BASE_ACTION_F_IDLE) : (ENUM_BASE_ACTION.BASE_ACTION_N_IDLE));
            }

            ChangeAction(nBaseAction, GetLogicSpeed(), false, sDefaultActionFuseTime);
        }

        m_paramLogic_Idle.m_uIdleInterval		= CalcIdleRandInterval();
        m_paramLogic_Idle.m_uLastIdleMotionTime = GameProcedure.s_pTimeSystem.GetTimeNow();
        m_paramLogic_Idle.m_uStartTime = GameProcedure.s_pTimeSystem.GetTimeNow();

        CharacterLogic_Set(ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE);

        return true;
    }
    bool Start_Action(CObjectCommand_Logic pLogicCommand)
    {
        if (pLogicCommand == null)
            return false;

        if (pLogicCommand.GetCommandID() != ENUM_OBJECT_COMMAND.OBJECT_COMMAND_ACTION)
            return false;

        CObjectCommand_Action pActionCommand = (CObjectCommand_Action)pLogicCommand;
        if (pActionCommand.GetActionID() == MacroDefine.INVALID_ID)
            return false;

        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
            return false;

        if (!m_bJumping)
        {
            ChangeAction(pActionCommand.GetActionID(), GetLogicSpeed(), false, sDefaultActionFuseTime);
        }

        SetCurrentLogicCommand(pLogicCommand);
        SetLogicCount(pLogicCommand.GetLogicCount());
        CharacterLogic_Set(ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ACTION);
        return true;
    }
    bool Start_Move(CObjectCommand_Logic pLogicCommand)
    {
        if (pLogicCommand == null)
            return false;

        if (pLogicCommand.GetCommandID() != ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MOVE)
            return false;

        CObjectCommand_Move pMoveCommand = (CObjectCommand_Move)pLogicCommand;
        if (pMoveCommand.GetNodeCount() <= 0)
            return false;

        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
            return false;


        ENUM_CHARACTER_LOGIC ePrevLogic = CharacterLogic_Get();
        if (ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE != ePrevLogic)
        {
            if (!m_bJumping)
            {
                bool bFightState;
                int nBaseAction;
                bFightState = IsFightState();
                if (bFightState)
                {
                    nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_F_RUN;
                }
                else
                {
                    nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_RUN;
                }
                ChangeAction(nBaseAction, GetLogicSpeed()/*  GetCharacterData().Get_SpeedRate()  */, true, sDefaultActionFuseTime);
            }
        }

        SetCurrentLogicCommand(pLogicCommand);
        SetLogicCount(pLogicCommand.GetLogicCount());

        SetLastModifyPosLogicCount(pLogicCommand.GetLogicCount());

        m_paramLogic_Move.m_nCurrentNodeIndex = 0;
        m_paramLogic_Move.m_posSaveStart = new Vector2(GetPosition().x, GetPosition().z);
        m_paramLogic_Move.m_posSaveTarget = new Vector2(pMoveCommand.GetNodeList()[0].m_fX, pMoveCommand.GetNodeList()[0].m_fZ);

        CharacterLogic_Set(ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE);
        //DispatchPhyEvent(PE_OBJECT_BEGIN_MOVE);

        return true;
    }
    protected virtual bool Start_MagicCharge(CObjectCommand_Logic pLogicCommand)//在父类实现
    {return true;}
    protected virtual bool Start_MagicChannel(CObjectCommand_Logic pLogicCommand) { return true; }//在父类实现
    protected virtual bool Start_MagicSend(CObjectCommand_Logic pLogicCommand) { return true; }//在父类实现
    protected virtual bool Start_Dead(bool bPlayDieAni) { return true; }//在父类实现
    bool Start_Ability(CObjectCommand_Logic pLogicCommand) 
    { 
        if(pLogicCommand == null) return false;

	    if(pLogicCommand.GetCommandID() != ENUM_OBJECT_COMMAND.OBJECT_COMMAND_ABILITY)
		    return false;

	    if ( CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD )
		    return false;

	    CObjectCommand_Ability pAbilityCommand = (CObjectCommand_Ability)pLogicCommand;

	    _DBC_LIFEABILITY_DEFINE pAbilityDef =  CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_DEFINE>((int)DataBaseStruct.DBC_LIFEABILITY_DEFINE).Search_Index_EQU(pAbilityCommand.GetAbilityID());
	    if(pAbilityDef == null)
		    return false;

	    int uTotalTime = 0;
	    if(pAbilityCommand.GetPrescriptionID() >= 0)
	    {
 		    _DBC_LIFEABILITY_ITEMCOMPOSE pDefine =
            CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_ITEMCOMPOSE>((int)DataBaseStruct.DBC_LIFEABILITY_ITEMCOMPOSE).Search_Index_EQU(pAbilityCommand.GetPrescriptionID());
     
 		    //记录起始时间和总共需要的时间
 		    if(pDefine != null && pDefine.nProficiencyTime > 0)
 			    uTotalTime = (pDefine.nProficiencyTime);
	    }

	    if(uTotalTime == 0)
	    {
		    //记录起始时间和总共需要的时间
		    if(pAbilityDef.nTimeOperation > 0)
			    uTotalTime = (pAbilityDef.nTimeOperation);
	    }

	    if(uTotalTime == 0)
		    return false;

	    // 方向
	    CObject pTarget = (CObject)((CObjectManager.Instance).FindServerObject((int) pAbilityCommand.GetTargetObjID() ));
	    if(pTarget != null && pTarget != this)
	    {
		    Vector3  fvTarget = pTarget.GetPosition();
		    Vector2 fvThis;
		    Vector2 fvTarget2;

		    fvThis.x = GetPosition().x;
		    fvThis.y = GetPosition().z;
		    fvTarget2.x = fvTarget.x;
		    fvTarget2.y = fvTarget.z;

		    float fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget2);
		    SetFaceDir( fDir );

	    }

	    //// 隐藏武器
	    //if(GetCharacterType() == CT_PLAYEROTHER || GetCharacterType() == CT_PLAYERMYSELF)
	    //	((CObject_PlayerOther*)this).EquipItem_BodyLocator( HEQUIP_WEAPON, pAbilityDef.nItemVisualLocator);

	    // 更改人物动作
	    ChangeAction( pAbilityDef.nAnimOperation, GetLogicSpeed(), true , sDefaultActionFuseTime);

	    //更新生活技能声音
         //todo
	/*    if(CObjectManager::GetMe().GetMySelf() == this)
	    {
		    //创建声音
		    if(!m_pLifeAbilitySound)
		    {
			    m_pLifeAbilitySound = CGameProcedure::s_pSoundSystem.Source_Create(CSoundSourceFMod::type_skill, FALSE, FALSE);
			    TDAssert(m_pLifeAbilitySound);

		    }

		    // 屏蔽声音 [7/16/2011 ivan edit]
		    //根据生活技能开始播放不同声音
    // 		switch(pAbilityCommand.GetAbilityID())
    // 		{
    // 		case 1:			//烹饪
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(16+59));
    // 			m_pLifeAbilitySound.SetLooping(TRUE);
    // 			m_pLifeAbilitySound.Play();
    // 		case 2:			//中医
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(17+59));
    // 			m_pLifeAbilitySound.SetLooping(TRUE);
    // 			m_pLifeAbilitySound.Play();
    // 			break;
    // 		case 3:			//加工
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(20+59));//还有18
    // 			m_pLifeAbilitySound.SetLooping(TRUE);
    // 			m_pLifeAbilitySound.Play();
    // 			break;
    // 		case 4:			//铸剑
    // 		case 5:			//制衣
    // 		case 6:			//艺术制作
    // 		case 7:			//工程学
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(18+59));
    // 			m_pLifeAbilitySound.SetLooping(TRUE);
    // 			m_pLifeAbilitySound.Play();
    // 			break;
    // 		case 8:			//采矿
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(12+59));
    // 			m_pLifeAbilitySound.SetLooping(TRUE);
    // 			m_pLifeAbilitySound.Play();
    // 			break;
    // 		case 9:			//采药
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(10+59));
    // 			m_pLifeAbilitySound.SetLooping(TRUE);
    // 			m_pLifeAbilitySound.Play();
    // 			break;
    // 		case 10:		//钓鱼
    // 			m_pLifeAbilitySound.SetBuffer(CGameProcedure::s_pSoundSystem.Buffer_Create(14+59));
    // 			m_pLifeAbilitySound.SetLooping(FALSE);
    // 			m_pLifeAbilitySound.Play();
    // 			break;
    // 		default:		//...
    // 			break;
    // 		}

	    }*/

	    SetCurrentLogicCommand(pLogicCommand);
	    SetLogicCount(pLogicCommand.GetLogicCount());

	    m_paramLogic_Ability.m_uCurrentTime		= 0;
	    m_paramLogic_Ability.m_uTotalTime		= (uint)uTotalTime;

	    //进度条
	    if ( CObjectManager.Instance.getPlayerMySelf() == this )
	    {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_SHOW, pAbilityDef.szProgressbarName);
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH, 0);
	    }

	    //设置人物逻辑为“生活技能进行中...”
        CharacterLogic_Set(ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ABILITY_ACTION);

        return true; 
    
    }
    bool Start_Stall(bool bPlayAni) { return true; }


    protected CObjectCommand_Logic GetCurrentLogicCommand()
    {
        return m_pCurrentLogicCommand;
    }

    protected void SetCurrentLogicCommand(CObjectCommand_Logic pLogicCommand)
    {
        if (m_pCurrentLogicCommand != null)
        {
            ObjectCommandGenerator.DeleteObjectCommand(m_pCurrentLogicCommand);
        }
        m_pCurrentLogicCommand = pLogicCommand;
    }
    protected void CheckMoveStop()
    {
        if (CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
            return;

        CObjectCommand_Move pMoveCommand = (CObjectCommand_Move)GetCurrentLogicCommand();
        if (pMoveCommand != null && pMoveCommand.GetNodeCount() > 0)
        {
            // 主角不走回头路，不瞬移
            bool bAdjustPos = false;
            WORLD_POS posAdjust;
            posAdjust.m_fX = 0;
            posAdjust.m_fZ = 0;
            if (m_paramLogic_Move.m_nCurrentNodeIndex >= pMoveCommand.GetNodeCount())
            {
                WORLD_POS[] paPos = pMoveCommand.GetNodeList();
                int nEndNodeIndex = pMoveCommand.GetNodeCount() - 1;
                posAdjust = paPos[nEndNodeIndex];

                bAdjustPos = true;
            }
            else if (m_paramLogic_Move.m_nCurrentNodeIndex == pMoveCommand.GetNodeCount() - 1)
            {
                WORLD_POS[] paPos = pMoveCommand.GetNodeList();
                int nEndNodeIndex = pMoveCommand.GetNodeCount() - 1;
                WORLD_POS posCommandTarget = paPos[nEndNodeIndex];

                float fLenCSTarget = Mathf.Abs(m_paramLogic_Move.m_posSaveTarget.x - posCommandTarget.m_fX)
                    + Mathf.Abs(m_paramLogic_Move.m_posSaveTarget.y - posCommandTarget.m_fZ);

                // 目标点变动了
                if (fLenCSTarget > 0.01f)
                {
                    float fSaveToServerDist = Utility.GetDistance(m_paramLogic_Move.m_posSaveTarget.x, m_paramLogic_Move.m_posSaveTarget.y, posCommandTarget.m_fX, posCommandTarget.m_fZ);
                    float fSaveToCurrentDist = Utility.GetDistance(m_paramLogic_Move.m_posSaveTarget.x, m_paramLogic_Move.m_posSaveTarget.y, GetPosition().x, GetPosition().z);

                    // 这里忽略了服务器传过来的目标点不在路径上的情况
                    if (fSaveToServerDist - fSaveToCurrentDist >= 0.0f)
                    {
                        posAdjust = posCommandTarget;
                        bAdjustPos = true;
                    }

                    m_paramLogic_Move.m_posSaveTarget.x = posCommandTarget.m_fX;
                    m_paramLogic_Move.m_posSaveTarget.y = posCommandTarget.m_fZ;
                }
            }

            if (bAdjustPos)
            {
                if (CObjectManager.Instance.getPlayerMySelf() == this)
                {
                    // 当位置差大于某个值时
                    float fAdjustToCurrentDist = Utility.GetDistance(posAdjust.m_fX, posAdjust.m_fZ, GetPosition().x, GetPosition().z);
                    if (fAdjustToCurrentDist > ObjectDef.DEF_CLIENT_ADJUST_POS_WARP_DIST)
                    {
                        // 瞬移到当前服务器对应的位置
                        SetMapPosition(posAdjust.m_fX, posAdjust.m_fZ);
                    }
                    else
                    {
                        // 调整服务器位置到当前主角客户端对应的位置
                        Network.Packets.CGCharPositionWarp msgWarp = new Network.Packets.CGCharPositionWarp();
                        WORLD_POS posCur = new WORLD_POS();
                        posCur.m_fX = GetPosition().x;
                        posCur.m_fZ = GetPosition().z;
                        msgWarp.ObjectID = ServerID;
                        msgWarp.ServerPos = posAdjust;
                        msgWarp.ClientPos = posCur;
                        NetManager.GetNetManager().SendPacket(msgWarp);
                    }
                }
                else
                {
                    float fAdjustToCurrentDist = Utility.GetDistance(posAdjust.m_fX, posAdjust.m_fZ, GetPosition().x, GetPosition().z);

                    if (fAdjustToCurrentDist > ObjectDef.DEF_CHARACTER_POS_ADJUST_DIST)
                    {
                        // 瞬移到当前服务器对应的位置
                        SetMapPosition(posAdjust.m_fX, posAdjust.m_fZ);
                    }
                }
                CharacterLogic_Stop(true);
            }
        }
    }
    // 处理缓存的指令
    bool ProcessLogicCommand()
    {
        // 此处代码用来控制逻辑命令同步，防止出现堆积大量逻辑命令 [2/18/2011 ivan edit]
        float fSpeed;
        int nLogicCommandCount = (int)(m_listLogicCommand.Count);
        if (nLogicCommandCount > 1)
        {
            fSpeed = (float)(nLogicCommandCount-1) * 0.5f + 1.0f;
        }
        else
        {
            fSpeed = 1.0f;
        }
        SetLogicSpeed(fSpeed);

        if (CharacterLogic_IsStopped())
        {
            bool bResult = DoNextLogicCommand();
            if (!bResult)
            {
                Start_Idle();
            }
        }
        else if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE)
        {
            DoNextLogicCommand();
        }
        return true;
    }

    bool DoNextLogicCommand()
    {
        if (!IsLogicCommandListEmpty())
        {
            CObjectCommand_Logic pLogicCommand = NextLogicCommand();

            bool bResult = DoLogicCommand(pLogicCommand);

            if (!bResult)
            {
                ObjectCommandGenerator.DeleteObjectCommand(pLogicCommand);
            }

            return bResult;
        }
        return false;
    }
    bool DoLogicCommand(CObjectCommand_Logic pLogicCmd)
    {
        bool bResult = false;
        switch (pLogicCmd.GetCommandID())
        {
            case ENUM_OBJECT_COMMAND.OBJECT_COMMAND_ACTION:
                {
                    bResult = Start_Action(pLogicCmd);
                }
                break;
            case ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MOVE:
                {
                    bResult = Start_Move(pLogicCmd);
                }
                break;
            case ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_SEND:
                {
                    bResult = Start_MagicSend(pLogicCmd);
                }
                break;
            case ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_CHARGE:
                {
                    bResult = Start_MagicCharge(pLogicCmd);
                }
                break;
            case ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_CHANNEL:
                {
                    bResult = Start_MagicChannel(pLogicCmd);
                }
                break;
            case ENUM_OBJECT_COMMAND.OBJECT_COMMAND_ABILITY:
                {
                    bResult = Start_Ability(pLogicCmd);
                }
                break;
            default:
                break;
        }
        return bResult;
    }
    protected bool IsLogicCommandListEmpty()
    {
        return m_listLogicCommand.Count == 0;
    }
    bool PushLogicCommand(CObjectCommand_Logic pCmd)
    {
        int nLogicCount = pCmd.GetLogicCount();
        if (!IsLogicCommandListEmpty())
        {
            int count = m_listLogicCommand.Count;
            for (int i = 0; i < count; ++i)
            {
                if (m_listLogicCommand[i].GetLogicCount() > nLogicCount)
                {
                    m_listLogicCommand.Insert(i, pCmd);
                    return true;
                }

            }
        }
        m_listLogicCommand.Add(pCmd);
        return true;
    }

    CObjectCommand_Logic GetNextLogicCommand()
    {
        if (!IsLogicCommandListEmpty())
        {
            CObjectCommand_Logic pCmd = m_listLogicCommand[0];
            return pCmd;
        }
        return null;
    }

    CObjectCommand_Logic NextLogicCommand()
    {
        if (!IsLogicCommandListEmpty())
        {
            CObjectCommand_Logic pCmd = m_listLogicCommand[0];
            m_listLogicCommand.Remove(pCmd);
            return pCmd;
        }
        return null;
    }

    CObjectCommand_Logic FindLogicCommand(int nLogicCount)
    {
        // 当前正在执行的指令
        if (m_pCurrentLogicCommand != null && m_pCurrentLogicCommand.GetLogicCount() == nLogicCount)
        {
            return m_pCurrentLogicCommand;
        }

        // 缓存的指令
        int count = m_listLogicCommand.Count;
        for (int i = 0; i < count; ++i)
        {
            if (m_listLogicCommand[i].GetLogicCount() == nLogicCount)
            {
                return m_listLogicCommand[i];
            }
        }
        return null;
    }

    protected bool CleanupLogicCommandList()
    {
        int count = m_listLogicCommand.Count;
        for (int i = 0; i < count; ++i)
        {
            ObjectCommandGenerator.DeleteObjectCommand(m_listLogicCommand[i]);
        }

        m_listLogicCommand.Clear();
        return true;
    }

    protected bool IsBeAttackEffectShowed()
	{
		return m_paramLogic_MagicSend.m_bBeAttackEffectShowed;
	}

    public override bool IsLogicReady(int nLogicCount)
    {
        if (GetLogicCount() > nLogicCount)
        {
            return true;
        }
        else if (GetLogicCount() == nLogicCount)
        {
            if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND)
            {
                if(IsBeAttackEffectShowed())
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public int GetLastModifyPosLogicCount()
    {
        return m_nLastModifyPosLogicCount;
    }

    public bool IsIdle()
    {
        return CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE;
    }

    public bool IsMoving()
    {
        return CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE && !CharacterLogic_IsStopped();
    }

    // 逻辑停止方法
    protected void CharacterLogic_Stop(bool bFinished)
    {
        SetCurrentLogicCommand(null);

        CharacterLogic_OnStopped(bFinished);
    }

    /// <summary>
    /// 强制停止所有逻辑
    /// </summary>
    public void StopLogic()
    {
        CharacterLogic_Stop(true);
        if (m_pTheAI != null)
            m_pTheAI.Reset();
    }

    // 逻辑停止事件，任何逻辑的任何条件停止都得调用(这个函数的调用是在设置下一逻辑之前)
    // bFinished	:	是否为成功执行完毕
    protected void CharacterLogic_OnStopped(bool bFinished)
    {
 	    m_bCharacterLogicStopped = true;
 		if(GetCharacterData() == null)
		{
 	    	LogManager.Log("CharacterLogic_OnStopped " + ServerID);
		}
        if (GetCharacterData() != null)
        {
            if (GetCharacterData().IsSit())
            {
                StandUp();
            }
        }
        else
        {
            LogManager.LogError("CharacterLogic_OnStopped " + ServerID);
        }
//      
//      	    //设置进度条
      	    if ( CObjectManager.Instance.getPlayerMySelf() == this )
      	    {
      		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_HIDE);
      	    }
//      
//      	    if(CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ABILITY_ACTION)
//      	    {
//      		    if(m_pLifeAbilitySound != null)
//      		    {
//      			    //停止生活技能声音
//      			    m_pLifeAbilitySound.Stop();
//      			    CGameProcedure::s_pSoundSystem.Source_Destroy(m_pLifeAbilitySound);
//      			    m_pLifeAbilitySound = 0;
//      		    }
//      	    }
    }
    public override void Tick()
    {
        base.Tick();
        ProcessLogicCommand();

        //AI逻辑Tick
        if(m_pTheAI != null) 
            m_pTheAI.Tick();

        if (!CharacterLogic_IsStopped())
        {
            uint uElapseTime = (uint)(GameProcedure.s_pTimeSystem.GetDeltaTime());
            uElapseTime = (uint)((float)uElapseTime * GetLogicSpeed());
            bool bResult = false;
            switch (CharacterLogic_Get())
            {
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE:
                    bResult = Tick_Idle(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ACTION:
                    bResult = Tick_Action(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE:
                    bResult = Tick_Move(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_GATHER:
                    bResult = Tick_MagicCharge(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_LEAD:
                    bResult = Tick_MagicChannel(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND:
                    bResult = Tick_MagicSend(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_JUMP_SEND:
                    // 增加跳斩逻辑 [10/31/2011 Ivan edit]
                    bResult = Tick_JumpMagicSend(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ABILITY_ACTION:
                    bResult = Tick_Ability(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD:
                    bResult = Tick_Dead(uElapseTime);
                    break;
                case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_STALL:
                    bResult = Tick_Stall(uElapseTime);
                    break;
                default:
                    bResult = false;
                    break;
            }
            if (!bResult)
            {
                Start_Idle();
            }
        }
        // 摄像机位置更新需要放在这里，否则会出现抖动的问题 [4/28/2012 SUN]
        if (CObjectManager.Instance.getPlayerMySelf() == this)
        {
            GFX.SceneCamera.Instance.lookAt(ref m_fvFootPosition);//根据主角位置更新摄像机位置
        }

        Tick_UpdateInfoBoard();

        //特效刷新
        Tick_UpdateEffect();

        // 更新伤害信息
        m_CLogicEvent_Character.UpdateLogicEvent();
        
        // 战斗状态
	    if(IsFightState())
	    {
		    uint uET = (uint)(GameProcedure.s_pTimeSystem.GetDeltaTime());
		    if(uET >= m_uFightStateTime)
		    {
			    //m_uFightStateTime = 0;
			    SetFightState(false);
		    }
		    else
		    {
			    m_uFightStateTime -= uET;
		    }
	    }

    }

    // 更新信息面板 [1/31/2012 Ivan]
    protected virtual void Tick_UpdateInfoBoard()
    {
        // 转移到npc类实现 [3/27/2012 Ivan]
    }

    //在一种逻辑状态下的逻辑Tick
    protected virtual bool Tick_Idle(uint uElapseTime) 
	{ 
		m_paramLogic_Idle.m_uStartTime += uElapseTime;
		// 有mood action 就可以做，原来的坐下状态去掉 [11/4/2010 Sun]
		if(/*GetCharacterData().IsSit() &&*/ IsHaveChatMoodAction())
		{
			FreshAnimation();
		}
		return true; 
	}
    protected virtual bool Tick_Action(uint uElapseTime) { return true; }
    protected virtual bool Tick_Move(uint uElapseTime)
    {
        CObjectCommand_Move pMoveCommand = (CObjectCommand_Move)GetCurrentLogicCommand();
        if (pMoveCommand == null)
        {
            return false;
        }

        if (pMoveCommand.GetNodeCount() <= 0)
        {
            return false;
        }
        WORLD_POS[] paPos = pMoveCommand.GetNodeList();

        Vector2 fvStartPos2D = new Vector2(m_paramLogic_Move.m_posSaveStart.x, m_paramLogic_Move.m_posSaveStart.y);
        Vector2 fvCurrentPos2D = new Vector2(GetPosition().x, GetPosition().z);
        Vector2 fvTargetPos2D = new Vector2(paPos[m_paramLogic_Move.m_nCurrentNodeIndex].m_fX, paPos[m_paramLogic_Move.m_nCurrentNodeIndex].m_fZ);
        // 当前位置与当前的目标路径点路径长度的平方
        Vector2 fvDistToTarget = fvTargetPos2D - fvCurrentPos2D;
        float fDistToTargetSq = fvDistToTarget.x * fvDistToTarget.x + fvDistToTarget.y * fvDistToTarget.y;
        float fDistToTarget = Mathf.Sqrt(fDistToTargetSq);

        // 这一帧可移动的路径长度
        float fElapseTime = ((float)(uElapseTime)) / 1000.0f;
        float fSpeed = GetCharacterData().Get_MoveSpeed();
        //temp code for test 
        if (fSpeed <= 0)
            fSpeed = 4;
        float fMoveDist = fSpeed * fElapseTime;

        if (fMoveDist < 0.01f)
        {
            return true;
        }

        if (fDistToTarget < 0.01f)
        {
            CharacterLogic_Stop(true);
            return true;
        }

        bool bStopMove = false;
        Vector2 fvSetToPos = fvCurrentPos2D;
        float fSetToDir = GetFaceDir();
        while (true)
        {
            if (fMoveDist > fDistToTarget)
            {
                m_paramLogic_Move.m_nCurrentNodeIndex++;
                if (m_paramLogic_Move.m_nCurrentNodeIndex >= pMoveCommand.GetNodeCount())
                {
                    // 走到了
                    bStopMove = true;
                    fvSetToPos = fvTargetPos2D;
                    fSetToDir = GFX.GfxUtility.GetYAngle(new Vector2(fvCurrentPos2D.x, fvCurrentPos2D.y), new Vector2(fvTargetPos2D.x, fvTargetPos2D.y));
                    break;
                }
                else
                {
                    // 改变m_paramLogic_Move中的各个值
                    fMoveDist -= fDistToTarget;
                    fvStartPos2D = fvTargetPos2D;
                    fvCurrentPos2D = fvTargetPos2D;
                    fvTargetPos2D = new Vector2(paPos[m_paramLogic_Move.m_nCurrentNodeIndex].m_fX, paPos[m_paramLogic_Move.m_nCurrentNodeIndex].m_fZ);

                    m_paramLogic_Move.m_posSaveStart.x = fvStartPos2D.x;
                    m_paramLogic_Move.m_posSaveStart.y = fvStartPos2D.y;
                    m_paramLogic_Move.m_posSaveTarget.x = fvTargetPos2D.x;
                    m_paramLogic_Move.m_posSaveTarget.y = fvTargetPos2D.y;
                }
            }
            else
            {
                float fDistX = (fMoveDist * (fvTargetPos2D.x - fvCurrentPos2D.x)) / fDistToTarget;
                float fDistZ = (fMoveDist * (fvTargetPos2D.y - fvCurrentPos2D.y)) / fDistToTarget;

                fvSetToPos.x = fvCurrentPos2D.x + fDistX;
                fvSetToPos.y = fvCurrentPos2D.y + fDistZ;

                //需要位置修正，防止因为误差走入阻挡区内部 
                //todo
                // 			    if( CObjectManager.Instance.getPlayerMySelf() == this && CPath::IsPointInUnreachRegion(fvSetToPos))
                // 			    {
                // 				    fvSetToPos = TDU_GetReflect(fvStartPos2D, fvTargetPos2D, fvSetToPos);
                // 
                // 				    // 如果修正后的位置还在阻挡区内，则停止移动 [11/4/2011 Ivan edit]
                // 				    if (CPath::IsPointInUnreachRegion(fvSetToPos))
                // 				    {
                // 					    bStopMove = true;
                // 					    fvSetToPos = fvCurrentPos2D;
                // 					    //Assert( !bStopMove && "Wrong position" );
                // 				    }
                // 			    }

                fSetToDir = GFX.GfxUtility.GetYAngle(fvCurrentPos2D, fvTargetPos2D);
                break;
            }
        }

        SetFaceDir(fSetToDir);
        SetMapPosition(fvSetToPos.x, fvSetToPos.y);

        if (bStopMove)
        {
            CharacterLogic_Stop(true);

            // 如果停止移动后，则需要判断上次寻路是否没有走到终点 [8/4/2011 ivan edit]
            if (GetCharacterType() == CHARACTER_TYPE.CT_PLAYERMYSELF)
            {
                // 只有玩家需要判断寻路是否没走完 [8/4/2011 ivan edit]
                //CGameInterface::GetMe().CheckNeedContinueMove(fvSetToPos);
            }
        }
        return true;
    }
    protected virtual bool Tick_MagicCharge(uint uElapseTime) { return true; }
    protected virtual bool Tick_MagicChannel(uint uElapseTime) { return true; }
    protected virtual bool Tick_MagicSend(uint uElapseTime) { return true; }
    protected virtual bool Tick_Dead(uint uElapseTime) { return true; }
    protected virtual bool Tick_Ability(uint uElapseTime)
    {
        CObjectCommand_Ability pAbilityCommand = (CObjectCommand_Ability)GetCurrentLogicCommand();
	    if(pAbilityCommand == null)
	    {
		    return false;
	    }

	    m_paramLogic_Ability.m_uCurrentTime += uElapseTime;

	    uint uEndTime = pAbilityCommand.GetEndTime();
	    if(uEndTime == MacroDefine.UINT_MAX)
	    {
		    uEndTime = m_paramLogic_Ability.m_uTotalTime;
	    }

	    if(m_paramLogic_Ability.m_uCurrentTime > uEndTime)
	    {
		    m_paramLogic_Ability.m_uCurrentTime = uEndTime;
	    }

	    if ( CObjectManager.Instance.getPlayerMySelf() == this )
	    {
		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH, (float)m_paramLogic_Ability.m_uCurrentTime/(float)m_paramLogic_Ability.m_uTotalTime);
	    }

	    if(m_paramLogic_Ability.m_uCurrentTime >= uEndTime)
	    {
		    if(m_paramLogic_Ability.m_uTotalTime <= uEndTime)
		    {
			    CharacterLogic_Stop(true);

                // 结束采集操作 [4/18/2012 Ivan]
                {
                    CAI_MySelf pMySelfAI = (CAI_MySelf)CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
                    pMySelfAI.FinishTripperActive();
                }
		    }
		    else
		    {
			    CharacterLogic_Stop(false);
		    }
	    }
	    return true; 
    
    }

    protected virtual bool Tick_Stall(uint uElapseTime) { return true; }
    protected virtual bool Tick_JumpMagicSend(uint uElapseTime) { return true; }

    public bool ModifyMoveCommand(int nLogicCount, byte numPos, WORLD_POS[] targetPos)
    {
        CObjectCommand_Logic pLogicHandle = FindLogicCommand(nLogicCount);
        if (pLogicHandle != null && IsMoving())
        {
            CObjectCommand_Move pLoginCommandMove = (CObjectCommand_Move)pLogicHandle;
            return pLoginCommandMove.ModifyTargetPos(numPos, targetPos);
        }
        return false;
    }
    // 压入一条指令
    public override bool PushCommand(SCommand_Object pCmd)
    {
        switch ((OBJECTCOMMANDDEF)pCmd.m_wID)
        {
            case OBJECTCOMMANDDEF.OC_LOGIC_EVENT:
                {
                    CObjectManager pObjectManager = CObjectManager.Instance;

                    _LOGIC_EVENT pLogicEvent = (_LOGIC_EVENT)(pCmd.GetValue<_LOGIC_EVENT>(0));

                    int nCreaterID = (int)pLogicEvent.m_nSenderID;
                    int nCreaterLogicCount = pLogicEvent.m_nSenderLogicCount;

                    CObject pCreater = (CObject)(pObjectManager.FindServerObject(nCreaterID));
                    if (pCreater != null && pCreater.IsLogicReady(nCreaterLogicCount))
                    {
                        DoLogicEvent(pLogicEvent);
                    }
                    else
                    {// 如施法者无效或传来的Impact是前面Skill对应的则直接显示
                        AddLogicEvent(pLogicEvent);
                    }
                }
                break;
            case OBJECTCOMMANDDEF.OC_STOP_ACTION:
            case OBJECTCOMMANDDEF.OC_STOP_MOVE:
            case OBJECTCOMMANDDEF.OC_STOP_SIT:
                {
                    CObjectCommand_StopLogic pStopCommand = (CObjectCommand_StopLogic)(ObjectCommandGenerator.NewObjectCommand(pCmd));

                    if (pStopCommand != null)
                    {
                        CObjectCommand_Logic pLogicCommand = FindLogicCommand(pStopCommand.GetLogicCount());
                        if (pLogicCommand != null)
                        {
                            pLogicCommand.Modify(pStopCommand);

                            if (pStopCommand.GetCommandID() == ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_MOVE
                                && pStopCommand.GetLogicCount() == GetLogicCount())
                            {
                                CheckMoveStop();
                            }
                        }
                        else
                        {
                            if (pStopCommand.GetCommandID() == ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_MOVE && pStopCommand.GetLogicCount() == GetLastModifyPosLogicCount())
                            {
                                if (CharacterLogic_IsStopped() || CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
                                {
                                    CObjectCommand_StopMove pStopMoveCommand = (CObjectCommand_StopMove)pStopCommand;

                                    float fStopToCurrentDist = Utility.GetDistance(pStopMoveCommand.GetEndPos().m_fX, pStopMoveCommand.GetEndPos().m_fZ, GetPosition().x, GetPosition().z);
                                    if (fStopToCurrentDist > ObjectDef.DEF_CHARACTER_POS_ADJUST_DIST)
                                    {
                                        if (CObjectManager.Instance.getPlayerMySelf() == this)
                                        {
                                            // 当位置差大于某个值时
                                            if (fStopToCurrentDist > ObjectDef.DEF_CLIENT_ADJUST_POS_WARP_DIST)
                                            {
                                                // 瞬移到当前服务器对应的位置
                                                SetMapPosition(pStopMoveCommand.GetEndPos().m_fX, pStopMoveCommand.GetEndPos().m_fZ);
                                            }
                                            else
                                            {
                                                // 调整服务器位置到当前主角客户端对应的位置
                                                Network.Packets.CGCharPositionWarp msgWarp = new Network.Packets.CGCharPositionWarp();
                                                WORLD_POS posCur = new WORLD_POS();
                                                posCur.m_fX = GetPosition().x;
                                                posCur.m_fZ = GetPosition().z;

                                                msgWarp.ObjectID = ServerID;
                                                msgWarp.ServerPos = pStopMoveCommand.GetEndPos();

                                                msgWarp.ClientPos = posCur;

                                                NetManager.GetNetManager().SendPacket(msgWarp);
                                            }
                                        }
                                        else
                                        {
                                            // 瞬移到当前服务器对应的位置
                                            SetMapPosition(pStopMoveCommand.GetEndPos().m_fX, pStopMoveCommand.GetEndPos().m_fZ);
                                        }
                                    }
                                }
                            }
                        }
                        ObjectCommandGenerator.DeleteObjectCommand(pStopCommand);
                    }
                }
                break;
            case OBJECTCOMMANDDEF.OC_ACTION:
            case OBJECTCOMMANDDEF.OC_MOVE:
            case OBJECTCOMMANDDEF.OC_MAGIC_SEND:
            case OBJECTCOMMANDDEF.OC_MAGIC_CHARGE:
            case OBJECTCOMMANDDEF.OC_MAGIC_CHANNEL:
            case OBJECTCOMMANDDEF.OC_ABILITY:
                {
                    CObjectCommand_Logic pLogicCommand = (CObjectCommand_Logic)(ObjectCommandGenerator.NewObjectCommand(pCmd));
                    if (pLogicCommand != null)
                    {
                        // 丢弃
                        if (pLogicCommand.GetLogicCount() <= GetLogicCount())
                        {
                            ObjectCommandGenerator.DeleteObjectCommand(pLogicCommand);
                        }
                        else
                        {
                            CObjectCommand_Logic pFindCommand = FindLogicCommand(pLogicCommand.GetLogicCount());
                            // 没有找到对应的指令才加入，主要是防止与客户端主角的预测指令相冲突
                            if (pFindCommand == null)
                            {
                                PushLogicCommand(pLogicCommand);
                                //if(GetCurrentLogicCommand() != null && pLogicCommand.GetLogicCount() < GetCurrentLogicCommand().GetLogicCount())
                                //{
                                //	// 停止客户端预测指令，执行服务器指令
                                //	CharacterLogic_Stop(false);
                                //}
                            }
                            else
                            {
                                ObjectCommandGenerator.DeleteObjectCommand(pLogicCommand);
                            }
                        }
                    }

                    if (CharacterLogic_IsStopped())
                    {
                        ProcessLogicCommand();
                    }
                }
                break;
            case OBJECTCOMMANDDEF.OC_MODIFY_ACTION:
                {
                    int nLogicCount = pCmd.GetValue<int>(0);
                    int nModifyTime = pCmd.GetValue<int>(1);
                    if (GetLogicCount() == nLogicCount && !CharacterLogic_IsStopped())
                    {
                        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_GATHER)
                        {
                            CObjectCommand_MagicCharge pMagicChargeCommand = (CObjectCommand_MagicCharge)(GetCurrentLogicCommand());
                            if (nModifyTime >= 0)
                            {
                                uint uModifyTime = (uint)nModifyTime;
                                if (m_paramLogic_MagicCharge.m_uCurrentTime > uModifyTime)
                                {
                                    m_paramLogic_MagicCharge.m_uCurrentTime -= uModifyTime;
                                }
                                else
                                {
                                    m_paramLogic_MagicCharge.m_uCurrentTime = 0;
                                }
                            }
                            else
                            {
                                uint uModifyTime = (uint)(Mathf.Abs(nModifyTime));
                                if (m_paramLogic_MagicCharge.m_uCurrentTime + uModifyTime > pMagicChargeCommand.GetTotalTime())
                                {
                                    m_paramLogic_MagicCharge.m_uCurrentTime = pMagicChargeCommand.GetTotalTime();
                                }
                                else
                                {
                                    m_paramLogic_MagicCharge.m_uCurrentTime += uModifyTime;
                                }
                            }
                        }
                        else if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_LEAD)
                        {
                            CObjectCommand_MagicChannel pMagicChannelCommand = (CObjectCommand_MagicChannel)(GetCurrentLogicCommand());
                            if (nModifyTime >= 0)
                            {
                                uint uModifyTime = (uint)nModifyTime;
                                if (m_paramLogic_MagicChannel.m_uCurrentTime > uModifyTime)
                                {
                                    m_paramLogic_MagicChannel.m_uCurrentTime -= uModifyTime;
                                }
                                else
                                {
                                    m_paramLogic_MagicChannel.m_uCurrentTime = 0;
                                }
                            }
                            else
                            {
                                uint uModifyTime = (uint)(Mathf.Abs(nModifyTime));
                                if (m_paramLogic_MagicChannel.m_uCurrentTime + uModifyTime > pMagicChannelCommand.GetTotalTime())
                                {
                                    m_paramLogic_MagicChannel.m_uCurrentTime = pMagicChannelCommand.GetTotalTime();
                                }
                                else
                                {
                                    m_paramLogic_MagicChannel.m_uCurrentTime += uModifyTime;
                                }
                            }
                        }
                    }
                }
                break;
            default:
                {
                    DoCommand(pCmd);
                }
                break;
        }
        return true;
    }
    // 
    // 	// 执行客气端的模拟指令，只充许AI调用，其他地方勿用，这个将清空缓存的指令表
    public RC_RESULT DoSimulantCommand(SCommand_Object pCmd)
    {
        CleanupLogicCommandList();
        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND)
        {
            if (!m_paramLogic_MagicSend.m_bCanBreak && !CharacterLogic_IsStopped())
            {
                return RC_RESULT.RC_WAIT;
            }
        }

        CharacterLogic_Stop(false);
        PushCommand(pCmd);
        return RC_RESULT.RC_OK;
    }
    // 立即执行此指令
    protected virtual RC_RESULT DoCommand(SCommand_Object pCmd)
    {
        return OnCommand(pCmd);
    }
    protected override RC_RESULT OnCommand(SCommand_Object pCmd)
    {
        RC_RESULT rcResult = RC_RESULT.RC_SKIP;

        switch ((OBJECTCOMMANDDEF)pCmd.m_wID)
        {
            case OBJECTCOMMANDDEF.OC_TELEPORT:
                {
                    float x = pCmd.GetValue<float>(0);
                    float z = pCmd.GetValue<float>(1);

                    SetMapPosition(x, z);
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case OBJECTCOMMANDDEF.OC_JUMP:
                {
                    Jump();
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case OBJECTCOMMANDDEF.OC_DEATH:
                {
                    bool bPlayDieAni;
                    bPlayDieAni = pCmd.GetValue<bool>(0);
                    bool bResult = Start_Dead(bPlayDieAni);
                    if (bResult)
                        rcResult = RC_RESULT.RC_OK;
                    else
                        rcResult = RC_RESULT.RC_ERROR;
                }
                break;
            case OBJECTCOMMANDDEF.OC_UPDATE_IMPACT:
                {
                    short idImpact;
                    bool bEnable;
                    uint nCreatorID;
                    idImpact = pCmd.GetValue<short>(0);
                    bEnable = (pCmd.GetValue<int>(1)!=0);
                    nCreatorID = (uint)pCmd.GetValue<int>(2);
                    LogManager.Log("idImpact " + idImpact + " bEnable " + bEnable + " CreateId " + nCreatorID);
                    ChangeImpact((uint)idImpact, nCreatorID, bEnable, true);
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case OBJECTCOMMANDDEF.OC_UPDATE_IMPACT_EX:
                {
                    short idImpact;
                    idImpact = pCmd.GetValue<short>(0);
                    ChangeImpact((uint)idImpact, (uint)MacroDefine.UINT_MAX, true, false);
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case OBJECTCOMMANDDEF.OC_LEVEL_UP:
                {
                    // string levelUpStr = ((int)(ENUM_BASE_ACTION.BASE_ACTION_LEVEL_UP).ToString();
                    // SetChatMoodAction(levelUpStr);
					//按策划的需求升级时只播放特效 [2012/4/17 ZZY]
					if(GetRenderInterface() != null )
					{
                        string shengjiEffectName = "shengji";
                        string locatorName = "FootEffectLocator";
                        GetRenderInterface().PlayEffect(locatorName, shengjiEffectName);
					}
                    rcResult = RC_RESULT.RC_OK;
                }
                break;

            default:
                rcResult = RC_RESULT.RC_SKIP;
                break;
        }

        return rcResult;

    }

    //动画事件
    public bool OnAnimationShakeEvent(string szAnimationName)
    {
        GFX.SceneCamera.Instance.OnShake();
	    return true;
    }
    public bool OnAnimationEnd(string szAnimationName)
    {
        FreshAnimation();
	    return true;
    }

    public bool OnAnimationCanBreak(string szAnimationName)
    {
	    if(CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND)
	    {
		    m_paramLogic_MagicSend.m_bCanBreak	= true;
	    }
	    //if(!IsLogicCommandListEmpty())
	    //{
	    //	CharacterLogic_Stop(TRUE);
	    //}
	    return true;
    }

    static int nHitCount=0;// 记录连击招的第几次Hit
    public bool OnAnimationHit(string szAnimationName)
    {
 	    if ( CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND )
 	    {
 		    CObjectCommand_MagicSend pMagicSendCommand = (CObjectCommand_MagicSend)GetCurrentLogicCommand();
            _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)m_paramLogic_MagicSend.m_nSaveMagicID);
 		    if ( skillData != null && pMagicSendCommand != null )
 		    {
 			    //设置当前目标位置数据
 			    uint idTarget = pMagicSendCommand.GetTargetObjID();
                CObject pTarget = CObjectManager.Instance.FindServerObject((int)idTarget);
 			    if (pTarget != null) 
 			    {
 				    bool bShowAll = false;
                    if (skillData.m_nHitsOrinterval > 0)
 				    {// 如果是连招
 					    ++nHitCount;
                        if (nHitCount == skillData.m_nHitsOrinterval)
 					    {// 如果是最后一招
 						    m_paramLogic_MagicSend.m_bBeAttackEffectShowed = true;	
 						    bShowAll = true;
 						    nHitCount = 0;
 					    }
 				    }
 				    else
 				    {
                        m_paramLogic_MagicSend.m_bBeAttackEffectShowed = true;
 				    }
 			    }
 			    else
 			    {
                    m_paramLogic_MagicSend.m_bBeAttackEffectShowed = true;
 			    }
 		    }
 	    }
 	    return true;
    }

    public override _CAMP_DATA GetCampData()
    {
        if (GetCharacterData() != null)
            return GetCharacterData().Get_CampData();
        else
            LogManager.LogWarning("Character.GetCampData():CampData is null.");
        return new _CAMP_DATA();
    }

    //外部调用相应死亡事件
    public void OnDead(bool bPlayDieAni)
    {
        Start_Dead(bPlayDieAni);
    }

    // 添加测试用冒泡功能 [2/7/2012 Ivan]
    public void ShowTalk(string text)
    {
        Vector3 fvPos = m_pInfoBoard != null ? m_pInfoBoard.GetPosition() : Vector3.zero;

        UISystem.Instance.AddNewBeHitBoard(false, text,
            fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_INVALID, ENUM_DMG_MOVE_TYPE.MOVE_SCENE_NAME);
    }

    /// <summary>
    /// 显示获得的经验值
    /// </summary>
    /// <param name="num"></param>
    public void ShowExp(int num)
    {
        Vector3 fvPos = m_pInfoBoard != null ? m_pInfoBoard.GetPosition() : Vector3.zero;

        UISystem.Instance.AddNewBeHitBoard(false, num.ToString(),
            fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_Exp, ENUM_DMG_MOVE_TYPE.MOVE_SCENE_NAME);
    }

    	// 随机说话 [4/26/2011 Sun]
	public virtual void	Tick_RandomSay()
    {
        //todo
//         UINT uTimeNow = CGameProcedure::s_pTimeSystem.GetTimeNow();
// 	    if ( m_param_RandomSay.m_uSayInterval < uTimeNow - m_param_RandomSay.m_uStartTime )
// 	    {
// 		    m_param_RandomSay.m_uStartTime = uTimeNow;
// 		    m_param_RandomSay.m_uSayInterval = CalcIdleRandInterval();//使用休闲动作的时间
// 
// 		    if(this.GetInfoBoard() && this.GetInfoBoard().isShow())
// 		    {
//     			
// 			    DBC_DEFINEHANDLE(s_pNpcPaoPaoIndex, DBC_NPC_PAOPAO_INDEX);
// 			    const _DBC_NPC_PAOPAO_INDEX* pPaopao = (const _DBC_NPC_PAOPAO_INDEX*)s_pNpcPaoPaoIndex.Search_Index_EQU( this.GetCharacterData().Get_RaceID() );
// 
// 			    INT idMsg = INVALID_ID;
// 			    if(pPaopao != NULL)
// 			    {
// 				    idMsg = pPaopao.nBaseMsgId + rand()%pPaopao.nStep;
// 
// 				    DBC_DEFINEHANDLE(s_pMonsterPaopao, DBC_MONSTER_PAOPAO);
// 				    if(s_pMonsterPaopao)
// 				    {
// 					    const _DBC_MONSTER_PAOPAO* pLine = (const _DBC_MONSTER_PAOPAO*)s_pMonsterPaopao.Search_Index_EQU(idMsg);
// 					    if(pLine)
// 					    {
// 						    this.Say(STRING(pLine.szPaoPaoTxt));
// 					    }
// 				    }
// 			    }
// 		    }
// 	    }
    }	

	public void			Say( string str )
    {
        //todo
       // m_pInfoBoard.SetElement_PaoPaoText(str);
       
    }

    public int GetOwnerId()
    {
        if (GetCharacterData() != null)
            return GetCharacterData().Get_OwnerID();
        else
            return -1;
    }
	
	public void OnHitGroundedEvent()
    {
        m_paramLogic_MagicSend.m_hitGrounded = true;
    }
}
