using System.Collections;
using System.Collections.Generic;
using DBSystem;
using UnityEngine;
using System;
public enum ENUM_NPC_TYPE
{
    NPC_TYPE_INVALID = -1,
    NPC_TYPE_MONSTER,
    NPC_TYPE_NPC,
    NPC_TYPE_PET,

    NPC_TYPE_NUMBERS
}
public class CObject_PlayerNPC : CObject_Character
{
    //返回角色类型
    public override CHARACTER_TYPE GetCharacterType() { return CHARACTER_TYPE.CT_MONSTER; }

    public ENUM_CHARACTER_LOGIC GetSkillLogicCmd(uint skillClassId)
    {
        // 跳斩技能逻辑id写死了 [10/31/2011 Ivan edit]
        const uint JUMP_SKILL_ID = 4;

        _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)m_paramLogic_MagicSend.m_nSaveMagicID);
        if (skillData == null)
            return ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND;

        // 心法对应的技能应该跟等级走，因为这里只需要读取技能逻辑，所以用一级的就可以了 [10/31/2011 Ivan edit]
        int skillId = skillData.m_anSkillByLevel[0];
        if (skillId <= 0)
            return ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND;

        // 跳斩必须需要目标落点，所以必须是这两种选择类型 [10/31/2011 Ivan edit]
        if (skillData.m_nSelectType != (int)ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER &&
            skillData.m_nSelectType != (int)ENUM_SELECT_TYPE.SELECT_TYPE_POS)
            return ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND;

        //搜索纪录

        _DBC_SKILLDATA_V1_DEPLETE pSkillData_V1 = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILLDATA_V1_DEPLETE>((int)DataBaseStruct.DBC_SKILLDATA_V1_DEPLETE).Search_Index_EQU((int)skillId);
        if (pSkillData_V1 != null)
        {
            if (pSkillData_V1.nSkillLogicid == JUMP_SKILL_ID)
            {
                return ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_JUMP_SEND;
            }
        }
        return ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND;
    }

    //初始和释放
    public override void Initial(object pInit)
    {
        SetWeaponType(ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE);
        m_pCharacterData = CDataPool.Instance.CharacterData_Create(this);
        UpdateCharRace();
        UpdateCharModel();
        UpdateMountModel();
        base.Initial(pInit);
    }

    public override void Release()
    {
        ReleaseMountRenderInterface();
        base.Release();
        
        //删除逻辑信息
        CDataPool.Instance.CharacterData_Destroy(this);
        m_pCharacterData = null;
    }
    protected virtual int getCharModelID()
    {
        //经过变身
        if (GetCharacterData().Get_ModelID() != -1)
        {
            //直接从模型定义表中读取模型名
            return GetCharacterData().Get_ModelID();
        }
        else
        {
            if (mCreatureInfo != null)
                return mCreatureInfo.nModelID;
            else
                return -1;
        }
    }

    /// <summary>
    /// 获取额外配置的特效名字
    /// </summary>
    /// <returns></returns>
    public string GetExtraEffectName()
    {
        if (mCreatureInfo != null)
            return mCreatureInfo.szEffectName;

        return "";
    }

    public float GetEffectHeight()
    {
        if (mCreatureInfo != null)
            return mCreatureInfo.effectHeight;
        
        return 0;
    }

    public override void UpdateCharModel()//更新角色模型
    {
        int charModelID = getCharModelID();
        if (charModelID == -1)
        {
            return;
        }

        mCharModel = CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MODEL>((int)DataBaseStruct.DBC_CHARACTER_MODEL).Search_Index_EQU(charModelID);
        if (mCharModel == null)
            LogManager.LogError("Can't not found charmodel with modelID:" + charModelID);
//         if (mCharModel != null) UnityEngine.LogManager.Log("char model not null");
//         else UnityEngine.LogManager.Log("char model is null");
        UpdateCharActionSetFile();//更新动作集合
        CreateRenderInterface();//创建渲染对象
        UpdateTransparent();
        UpdateBuffEffect();
    }
    public override void UpdateMountModel()//更新坐骑模型
    {
	    int	nMountModelID = MacroDefine.INVALID_ID;
	    m_fMountAddHeight = 0.0f;
	    if(GetCharacterData() != null)
	    {
		    int nMountID = GetCharacterData().Get_MountID();
		    if(nMountID != MacroDefine.INVALID_ID)
		    {
			    _DBC_CHARACTER_MOUNT pCharMount =  CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MOUNT>((int)DataBaseStruct.DBC_CHARACTER_MOUNT).Search_Index_EQU(nMountID);
			    if(pCharMount != null)
			    {
				    nMountModelID = pCharMount.m_nModelID;
				    m_fMountAddHeight = pCharMount.m_fAddHeight;
				    //npasslevel = pCharMount.m_nPassLevel;
			    }
		    }
	    }

	    if(m_nMountModelID != nMountModelID)
	    {
		    m_nMountModelID = nMountModelID;

		    UpdateCharActionSetFile();
		    UpdateMountActionSetFile();
		    ReleaseMountRenderInterface();
		    if(m_nMountModelID != MacroDefine.INVALID_ID)
		    {
			    CreateMountRenderInterface();
		    }
		    SetPosition(GetPosition());//调用自己的SetPositon zzy
		    SetFaceDir(GetFaceDir());
            Enable( (uint)(IsEnable((uint)ObjectStatusFlags.OSF_VISIABLE)?1:0));
            Disalbe((uint)(IsEnable((uint)ObjectStatusFlags.OSF_OUT_VISUAL_FIELD)?1:0));
            UpdateTransparent();
		    // 这里可能有问题
		    if ( CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD
			    && CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE )
		    {
			    Start_Idle();
		    }
		    else
		    {
			    FreshAnimation();
		    }
	    }
    }
    public override void UpdateCharRace()//更新Race
    {
        mCreatureInfo = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU(m_pCharacterData.Get_RaceID());
    }
    //在渲染层创建渲染指针
    public override void CreateRenderInterface()
    {
        if (mCharModel == null)
        {
            LogManager.LogError("CharModel is Null, can not CreateRenderInterface.");
            return;
        }
        //LogManager.Log("CreateRenderinterface");
        //LogManager.Log(mCharModel.m_pszModelName);
        if (mRenderInterface != null)
        {
            ReleaseRenderInterface();
        }
        if (mCharModel != null && mCharModel.m_pszModelName.Length > 0)
        {
            mRenderInterface = GFX.GFXObjectManager.Instance.createObject(mCharModel.m_pszModelName, GFX.GFXObjectType.ACTOR);
            mRenderInterface.useTempAsset();//资源未加载完全时使用临时模型
        }
        base.CreateRenderInterface();
        SetPosition(GetPosition());//设定位置、旋转和朝向charModelID
        SetFaceDir (GetFaceDir());
        SetScale(GetScale());

        Enable((uint)ObjectStatusFlags.OSF_VISIABLE);
        //根据是否是FakeObject作一定处理
	    if(GetFakeObjectFlag())
	    {
		    //禁止鼠标选择
		    mRenderInterface.SetRayQuery(false); 
		    //设置特殊的VisibleFlag
		    mRenderInterface.SetUIVisibleFlag();
	    }
	    else
	    {
		    //设置选择优先级
		    if(CHARACTER_TYPE.CT_PLAYERMYSELF == GetCharacterType()) 
			    mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_PLAYERMYSLEF);
		    else if(CHARACTER_TYPE.CT_PLAYEROTHER == GetCharacterType()) 
			    mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_PLAYEROTHER);
		    else if(ENUM_NPC_TYPE.NPC_TYPE_PET == ((CObject_PlayerNPC)this).GetNpcType())
			    mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_PET);
		    else
                mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_CREATURE);
	    }

        //设置骨骼动画回调
        mRenderInterface.SetAnimationEndEvent(new GFX.OnAnimationEvent(OnAnimationEnd));
         mRenderInterface.SetAnimationCanBreakEvent(new GFX.OnAnimationEvent(OnAnimationCanBreak));
         mRenderInterface.SetAnimationHitEvent(new GFX.OnAnimationEvent(OnAnimationHit));
         mRenderInterface.SetAnimationShakeEvent(new GFX.OnAnimationEvent(OnAnimationShakeEvent));
         mRenderInterface.SetHitGroundedEvent(new GFX.OnHitGroundedEvent(OnHitGroundedEvent));
         mRenderInterface.SetData(ID);
         FreshAnimation();
    }

    protected virtual void CreateMountRenderInterface()
    { 
        if(mRenderInterface != null)
	    {
		    int nMountModelID = m_nMountModelID;
		    if ( nMountModelID != MacroDefine.INVALID_ID )
		    {
			    string lpszModelFileName = null;
			    _DBC_CHARACTER_MODEL pCharModel = CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MODEL>((int)DataBaseStruct.DBC_CHARACTER_MODEL).Search_Index_EQU(nMountModelID);
			    if (pCharModel != null)
			    {
				    lpszModelFileName = pCharModel.m_pszModelName;
			    }

			    if (lpszModelFileName != null)
			    {
                    //创建渲染对象
				    mMountRenderInterface = GFX.GFXObjectManager.Instance.createObject(lpszModelFileName, GFX.GFXObjectType.ACTOR);
				    mMountRenderInterface.SetData(ID);

				    // 设置缩放
				   // mMountRenderInterface.scale = new Vector3(pCharModel.m_scale, pCharModel.m_scale, pCharModel.m_scale);

                    mMountRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_PLAYERMYSLEF);
                    //重置mRenderInterface的位置、旋转、朝向
                    mRenderInterface.position = Vector3.zero;
                    mRenderInterface.rotation = Quaternion.identity;
                    mRenderInterface.scale = Vector3.one;
				    mMountRenderInterface.Attach_Object(mRenderInterface, "Attach1Locator" );
                    mMountRenderInterface.position = GetPosition();
			    }
		    }
	    }
    }

    public override void OnDataChanged_StealthLevel() 
    {
        UpdateTransparent();
    }

    protected void UpdateTransparent()
    {
        if (GetCharacterData().Get_StealthLevel() > 0)
        {
            SetTransparent(0.6f,1);
        }
        else
        {
            SetTransparent(0,1);
        }
    }

    void SetTransparent(float fTransparency, float fTime/* = 1.f*/)
    {
        if (GetFakeObjectFlag())
        {
            return;
        }

        if (mRenderInterface != null)
        {
            mRenderInterface.SetTransparent(fTransparency,fTime);
        }

        if (mMountRenderInterface != null)
        {
            mMountRenderInterface.SetTransparent(fTransparency, fTime);
        }
    }

	protected virtual void	ReleaseMountRenderInterface( )
    {
        if (mMountRenderInterface != null)
        {
            if (mRenderInterface != null)
            {
                mMountRenderInterface.Detach_Object(mRenderInterface);
            }
            GFX.GFXObjectManager.Instance.DestroyObject(mMountRenderInterface);
            mMountRenderInterface = null;
        }
    }
    //动作集合
    protected void UpdateCharActionSetFile()
    {

            if (mCharModel == null) return;
            int MountID = GetCharacterData().Get_MountID();

            string actionSetFileName = null;
            if (MountID >= 0 && MountID < DBC_DEFINE.MAX_MOUNT_NUMBER)//有骑宠的情况
            {
                actionSetFileName = mCharModel.m_apszActionSetName_Mount[MountID];
            }
            else//无骑宠
            {
                actionSetFileName = mCharModel.m_pszActionSetName_None;
            }
            if (actionSetFileName != null && actionSetFileName.Length > 0)
            {
				try
				{
					mCharActionSetDBC = CActionSetMgr.Instance.GetActionSetFile(actionSetFileName);
				}
				catch(System.Exception e)
				{
					LogManager.LogWarning(e.ToString());
					mCharActionSetDBC = null;
				}
                
            }
        
    }

    protected void UpdateMountActionSetFile()
    {
        mMountActionSetDBC	= null;
	    if(GetCharacterData() != null)
	    {
		    int nMountModelID	= m_nMountModelID;
		    if(nMountModelID != MacroDefine.INVALID_ID)
		    {
                _DBC_CHARACTER_MODEL pMountModel = CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MODEL>((int)DataBaseStruct.DBC_CHARACTER_MODEL).Search_Index_EQU(nMountModelID);
			    if (pMountModel != null && pMountModel.m_pszActionSetName_None.Length>0)
			    {
					try
					{
						mMountActionSetDBC =  CActionSetMgr.Instance.GetActionSetFile(pMountModel.m_pszActionSetName_None);
					}
					catch(System.Exception e)
					{
						mMountActionSetDBC = null;
						LogManager.LogWarning(e.ToString());
					}

			    }
		    }
	    }
    }

    // nActionSetID	:	ENUM_BASE_ACTION or 其它
    // nWeaponType	:	ENUM_WEAPON_TYPE
    protected virtual string GetCharActionNameByActionSetID(int nActionSetID, int nWeaponType, ref bool pbHideWeapon, ref int pnAppointedWeaponID)
    {
        int nCalcWeaponType = nWeaponType;
	    switch ( GetCharacterType() )
	    {
	        case CHARACTER_TYPE.CT_MONSTER:
	        case CHARACTER_TYPE.CT_SpecialBus:// 增加载具 [8/30/2011 Ivan edit]
		        nCalcWeaponType = CannotBeAttack() ? (int)ENUM_WEAPON_TYPE.WEAPON_TYPE_NPC : (int)ENUM_WEAPON_TYPE.WEAPON_TYPE_MONSTER;
		        break;
	        case CHARACTER_TYPE.CT_PLAYEROTHER:
	        case CHARACTER_TYPE.CT_PLAYERMYSELF:
	        default:
		        if(GetCharacterData() != null)
		        {
			        // 如果model有值则以这个动作为主，否则就调用五行动作 [1/5/2011 ivan edit]
			        if (GetCharacterData().Get_ModelID() != MacroDefine.INVALID_ID) //// 在变身后,ModelID为变身ID
                    {
                        nCalcWeaponType = (int)ENUM_WEAPON_TYPE.WEAPON_TYPE_MONSTER;
			        }
			        else
			        {

			        }
		        }
		        break;
	    }
        if (mCharActionSetDBC != null && nActionSetID != -1 && nCalcWeaponType >= 0 && nCalcWeaponType < DBC_DEFINE.MAX_WEAPON_TYPE_NUMBER)
	    {
            _DBC_CHARACTER_ACTION_SET pActionSet = mCharActionSetDBC.Search_Index_EQU(nActionSetID);
		    if(pActionSet != null)
		    {   
				pbHideWeapon = pActionSet.bHideWeapon;
			    pnAppointedWeaponID	= pActionSet.nAppointedWeaponID;
			    
			    return pActionSet.pWeapon_Set[nCalcWeaponType];
		    }
	    }
	    return null;
    }
    protected virtual string GetMountActionNameByActionSetID(int nActionSetID, int nWeaponType)
    {
	    int nCalcWeaponType = nWeaponType;
	    switch ( GetCharacterType() )
	    {
	    case CHARACTER_TYPE.CT_MONSTER:
		    nCalcWeaponType = CannotBeAttack() ? (int)ENUM_WEAPON_TYPE.WEAPON_TYPE_NPC : (int)ENUM_WEAPON_TYPE.WEAPON_TYPE_MONSTER;
		    break;
	    case CHARACTER_TYPE.CT_PLAYEROTHER:
	    case CHARACTER_TYPE.CT_PLAYERMYSELF:
		    break;
	    default:
		    break;
	    }
        if (mMountActionSetDBC != null && nActionSetID != -1 && nCalcWeaponType >= 0 && nCalcWeaponType < DBC_DEFINE.MAX_WEAPON_TYPE_NUMBER)
	    {
		    _DBC_CHARACTER_ACTION_SET pActionSet = mMountActionSetDBC.Search_Index_EQU(nActionSetID);
		    if(pActionSet != null)
		    {
			    return pActionSet.pWeapon_Set[nCalcWeaponType];
		    }
	    }
	    return null;
    }
    public override void ChangeAction(int nSetID, float fSpeed, bool bLoop, float fFuseParam)
    {
        SetAnimationEnd(false);
//         bool bMustDoCharAction, bMustDoMountAction;
//         if(mRenderInterface != null)
//             bMustDoCharAction = true;
//         else 
//             bMustDoCharAction = false;
// 
//         if (mMountRenderInterface != null)
//             bMustDoMountAction = true;
//         else
//             bMustDoMountAction = false;
      
        bool bHideWeapon = false;
        int nAppointedWeaponID = MacroDefine.INVALID_ID;
        string lpszCharActionName = null;
        if (mRenderInterface != null)
        {
            lpszCharActionName = GetCharActionNameByActionSetID(nSetID, (int)GetWeaponType(), ref bHideWeapon, ref nAppointedWeaponID);
        }

        string lpszMountActionName = null;
        if (mMountRenderInterface != null)
        {
            
			lpszMountActionName = GetMountActionNameByActionSetID(nSetID, (int)GetWeaponType());
        }

        if (bHideWeapon && !m_bHideWeapon)
        {
            m_bHideWeapon = bHideWeapon;
            OnHideWeapon(nAppointedWeaponID);
        }
        else if (!bHideWeapon && m_bHideWeapon)
        {
            m_bHideWeapon = bHideWeapon;
            OnShowWeapon();
        }

        ChangeActionSpeed(fSpeed);
        if (lpszCharActionName != null && mRenderInterface != null)
        {
            if(IsAnimationEnd()) SetAnimationEnd(false);
			mRenderInterface.EnterSkill(false, lpszCharActionName, bLoop, fFuseParam);
        }

        if (lpszMountActionName != null && mMountRenderInterface != null)
        {
            if(IsAnimationEnd()) SetAnimationEnd(false);
            mMountRenderInterface.EnterSkill(false, lpszMountActionName, bLoop, fFuseParam);
        }
    }
    public override void Tick()
    {
        SetMapPosition(mPosition.x, mPosition.z);
        base.Tick();

        Tick_UpdateEffect();
    }
    // 服务器GUID
    public virtual void SetServerGUID(int guid) { m_GUIDServer = guid; }
    public virtual int GetServerGUID() { return m_GUIDServer; }
    public void SetNpcType(ENUM_NPC_TYPE eType)
    {
        m_eNpcType = eType;
        if (mRenderInterface != null)
        {
             		    if(ENUM_NPC_TYPE.NPC_TYPE_PET == m_eNpcType)
             			    mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_PET);
             		    else
                            mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_CREATURE);
        }
    }
    public ENUM_NPC_TYPE GetNpcType()
    {
	    return m_eNpcType;
    }
    public override bool CannotBeAttack()
    {
        if(mCreatureInfo != null)
            return (mCreatureInfo.nCannotBeAttack!=0); 
	    else 
		    return  base.CannotBeAttack();
    }
    //显示/隐藏武器
    protected virtual  void OnHideWeapon(int nAppointedWeaponID){}
    protected virtual void	OnShowWeapon(){}
    //动画是否结束
    protected bool m_bAnimationEnd =false;
    protected bool IsAnimationEnd() { return m_bAnimationEnd; }
    protected void SetAnimationEnd(bool bSet) { m_bAnimationEnd = bSet; }

    protected override void ChangeActionSpeed(float fSpeed)//动作速率
    {
        if (mRenderInterface != null)
        {
            mRenderInterface.ChangeActionRate(fSpeed);
        }
        if (mMountRenderInterface != null)
        {
            mMountRenderInterface.ChangeActionRate(fSpeed);
        }
    }
    public override void FreshAnimation()
    {
        SetAnimationEnd(true);
        switch (CharacterLogic_Get())
        {
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE:
                {
                    bool bPlayIdleMotion = false;
                    uint nTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
                    if (m_paramLogic_Idle.m_uLastIdleMotionTime + m_paramLogic_Idle.m_uIdleInterval < nTimeNow)
                    {
                        bPlayIdleMotion = true;
                        m_paramLogic_Idle.m_uLastIdleMotionTime = nTimeNow;
                        m_paramLogic_Idle.m_uIdleInterval = CalcIdleRandInterval();
                    }

                    if (IsHaveChatMoodAction())
                    {
                        if (GetCharacterData().IsSit())
                        {
                            StandUp();
                        }
                        else
                        {
                            PlayChatMoodAction();
                        }
                    }
                    else
                    {
                        bool bFightState, bSit;
                        int nBaseAction;
                        bFightState = IsFightState();
                        bSit = GetCharacterData().IsSit();
                        if (bSit)
                        {
                            nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_SIT_IDLE;
                        }
                        else
                        {
                            if (bFightState)
                            {
                                nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_F_IDLE;
                            }
                            else
                            {
                                if (bPlayIdleMotion)
                                {
                                    nBaseAction = GameProcedure.s_random.Next() % 2 + (int)ENUM_BASE_ACTION.BASE_ACTION_N_IDLE;//(int)ENUM_BASE_ACTION.BASE_ACTION_N_IDLE_EX0;
                                }
                                else
                                {
                                    nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_IDLE;
                                }
                            }
                        }
                        ChangeAction(nBaseAction, GetLogicSpeed(), false, sDefaultActionFuseTime);

                    }
                }
                break;
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ACTION:
                break;
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE:
                {
                    bool bFightState;
                    int nBaseAction;
                    bFightState = IsFightState();
                    if (bFightState /*GetWeaponType() != ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE*/)//如果带武器，播放战斗跑步动作  [3/8/2012 ZZY])
                    {
                        nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_F_RUN;
                    }
                    else
                    {
                        nBaseAction = (int)ENUM_BASE_ACTION.BASE_ACTION_N_RUN;
                    }
                    ChangeAction( nBaseAction, GetLogicSpeed()/*GetCharacterData().Get_SpeedRate()*/, true, sDefaultActionFuseTime);
                }
                break;
             	    case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_GATHER:
             		    break;
             	    case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_LEAD:
             		    break;
             	    case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND:
             		    {
                             //todo
                            if (m_paramLogic_MagicSend.m_bDoNextAction_Concatenation)
                            {
                                m_paramLogic_MagicSend.m_bDoNextAction_Concatenation = false;

                                _DBC_SKILL_DATA ClientSkill = CSkillDataMgr.Instance.GetSkillData((uint)m_paramLogic_MagicSend.m_nSaveMagicID);
                                if (ClientSkill != null)
                                {
                                    // 如果是连招的特殊处理
                                    bool bPlayAction = true;
                                    int nSkillActionType = ClientSkill.m_nSkillActionType;
                                    switch ((ENUM_SKILL_ACTION_TYPE)nSkillActionType)
                                    {
                                        case ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_CONCATENATION:
                                            {
                                                m_paramLogic_MagicSend.m_nActionIndex++;
                                                if (m_paramLogic_MagicSend.m_nActionIndex >= m_paramLogic_MagicSend.m_nActionCount)
                                                {
                                                    m_paramLogic_MagicSend.m_nActionIndex = 0;
                                                }
                                                bPlayAction = true;
                                            }
                                            break;
                                        case ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_CONCATENATION_EX:
                                            {
                                                m_paramLogic_MagicSend.m_nActionIndex++;
                                                if (m_paramLogic_MagicSend.m_nActionIndex >= m_paramLogic_MagicSend.m_nActionCount - 1)
                                                {
                                                    m_paramLogic_MagicSend.m_nActionIndex = 1;
                                                }
                                                bPlayAction = true;
                                            }
                                            break;
                                        case ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_NONE:
                                        default:
                                            break;
                                    }

                                    if (bPlayAction)
                                    {
                                        int nRandAnimID = CSkillDataMgr.Instance.GetRandAnim((uint)m_paramLogic_MagicSend.m_nSaveMagicID, m_paramLogic_MagicSend.m_nActionIndex, true);
                                        ChangeAction(nRandAnimID, GetLogicSpeed(), false, sDefaultActionFuseTime);
                                    }
                                }
                            }
             		    }
             		    break;
             	    case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ABILITY_ACTION:
             		    break;
             	    case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD:
             		    // 暂时屏蔽因为没有尸体动作 [11/2/2010 Sun]
             		    //ChangeAction( BASE_ACTION_F_CADAVER, GetLogicSpeed(), FALSE, 0.0f );
             		    break;
             	    case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_STALL:
                        if (m_paramLogic_Stall.m_nCurrentAnimation == (int)SLogicParam_Stall.ENUM_STALL_ANIMATION.STALL_ANIMATION_IDLE)
             		    {
             			    ChangeAction( (int)ENUM_BASE_ACTION.BASE_ACTION_N_SIT_IDLE, GetLogicSpeed(), true, sDefaultActionFuseTime);
             		    }
             		    break;
            default:
                break;
        }
    }

    public override uint			GetIdleInterval()
    {
        if(mCreatureInfo != null && mCreatureInfo.nIdleInterval > 0)
	    {
		    return (uint)mCreatureInfo.nIdleInterval * 1000;
	    }
	    else
	    {
		     return MacroDefine.UINT_MAX;
	    }
    }

    protected override bool Start_Dead(bool bPlayDieAni)
    {
        //死亡时禁用鼠标选择npc[ 2012/4/11 ZZY]
        if(GetRenderInterface() != null && GetRenderInterface().getGameObject() != null)
        {
            GameObject go = GetRenderInterface().getGameObject();
            if(go.collider != null && (this is CObject_PlayerOther) == false)
            {
                go.collider.enabled = false;
            }
        }

        CharacterLogic_Stop(false);

	    CleanupLogicCommandList();

        if(m_bDropBox_HaveData)
        {
            //创建ItemBox
            CTripperObject_ItemBox pBox = CObjectManager.Instance.NewTripperItemBox((int)m_nDropBox_ItemBoxID);
            pBox.Initial(null);	
            //设置位置
            pBox.SetMapPosition( m_posDropBox_CreatePos.m_fX, m_posDropBox_CreatePos.m_fZ);
            //设置掉落箱的归属
            pBox.SetOwnerGUID(m_DropBox_OwnerGUID);

            m_bDropBox_HaveData = false;

            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_DROP_ITEMBOX);
        }

	    // 必需下马
	    ReleaseMountRenderInterface();

	    CharacterLogic_Set( ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD );

	    if ( bPlayDieAni )
	    {
		    ChangeAction( (int)ENUM_BASE_ACTION.BASE_ACTION_F_DIE, GetLogicSpeed(), false ,sDefaultActionFuseTime);
	    }
	    else
	    {
		    ChangeAction( (int)ENUM_BASE_ACTION.BASE_ACTION_F_CADAVER, GetLogicSpeed(), false, 0.0f );
	    }

	    SetFightState( false );
	    //GameProcedure.m_bWaitNeedFreshMinimap = true;

	    //设置选择优先级
	    if(GetRenderInterface() != null )
	    {
            if(CHARACTER_TYPE.CT_PLAYEROTHER == GetCharacterType())
			    GetRenderInterface().RayQuery_SetLevel(RAYQUERY_LEVEL.RL_PLAYER_DEADBODY);
		    else
			    GetRenderInterface().RayQuery_SetLevel(RAYQUERY_LEVEL.RL_CREATURE_DEADBODY);
	    }

	    RemoveAllImpact();

	    RemoveAllLogicEvent();
        
	    if(this == CObjectManager.Instance.getPlayerMySelf())
	    {
		    string strTemp = "你死亡了";//COLORMSGFUNC("DIE_YOU_DIED_MSG");
            ShowTalk(strTemp);
	    }
	    //选中目标怪物死亡
        
	    if(this == CObjectManager.Instance.GetMainTarget() && GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
	    {
            CObjectManager.Instance.SetMainTarget(MacroDefine.INVALID_ID, CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT);
	    }
	    return true;
    }

    protected override bool Start_MagicCharge(CObjectCommand_Logic LogicCommand)
    {
        if (LogicCommand == null)
            return false;

        if (LogicCommand.GetCommandID() != ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_CHARGE)
            return false;

        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
            return false;

        CObjectCommand_MagicCharge MagicChargeCommand = (CObjectCommand_MagicCharge)LogicCommand;
        if (MagicChargeCommand.GetTotalTime() == 0)
            return false;

        _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)MagicChargeCommand.GetMagicID());
        if (skillData == null)
            return false;

        switch ((ENUM_SELECT_TYPE)skillData.m_nSelectType)
        {
            case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
            case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
                {
                    bool bCalcDir;
                    Vector2 fvTarget = new Vector2();
                    bCalcDir = false;
                    if (MagicChargeCommand.GetTargetObjID() != MacroDefine.UINT_MAX)
                    {
                        if (MagicChargeCommand.GetTargetObjID() != ServerID)
                        {
                            CObject Target = CObjectManager.Instance.FindServerObject((int)MagicChargeCommand.GetTargetObjID());
                            if (Target != null)
                            {
                                fvTarget.x = Target.GetPosition().x;
                                fvTarget.y = Target.GetPosition().z;
                                bCalcDir = true;
                            }
                        }
                    }
                    else
                    {
                        fvTarget.x = MagicChargeCommand.GetTargetPos().m_fX;
                        fvTarget.y = MagicChargeCommand.GetTargetPos().m_fZ;
                        bCalcDir = true;
                    }

                    if (bCalcDir)
                    {
                        float fDir;
                        Vector2 fvThis = new Vector2();
                        fvThis.x = GetPosition().x;
                        fvThis.y = GetPosition().z;
                        fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget);
                        SetFaceDir(fDir);
                    }
                }
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
                SetFaceDir(MagicChargeCommand.GetTargetDir());
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
                break;
        }

        // 起手特效 [7/22/2010 Sun]
        // 效果触发的特效（RenderInterface自动会删除）
        if (skillData.m_lpBeginEffect.Length > 0 && mRenderInterface != null)
        {
            mRenderInterface.attachEffect(skillData.m_lpBeginEffectLocator, skillData.m_lpBeginEffect);
        }

        if (skillData.m_lpBeginSound.Length > 0)
        {
            Vector3 fvPos = GetPosition();
            //sunyu此接口是提供给fairy调用的音效接口，所以传入坐标必须是gfx坐标
            Vector3 fvGame;
            //if(!CGameProcedure::s_pGfxSystem.Axis_Trans(CRenderSystem::AX_GAME, fvPos, 
            //    CRenderSystem::AX_GFX, fvGame))
            //{
            //    return FALSE;
            //}
            //CSoundSystemFMod::_PlaySoundFunc( pClientSkill.m_lpBeginSound, &fvGame.x, false );
        }

        int nRandAnimID = CSkillDataMgr.Instance.GetRandAnim((uint)MagicChargeCommand.GetMagicID(), 0, false);
        if (nRandAnimID != -1)
        {
            ChangeAction(nRandAnimID, GetLogicSpeed(), true,sDefaultActionFuseTime);
        }

        if (CObjectManager.Instance.getPlayerMySelf() == this)
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_SHOW, skillData.m_lpszName);
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH, 0);
        }

        SetCurrentLogicCommand(LogicCommand);
        SetLogicCount(LogicCommand.GetLogicCount());

        m_paramLogic_MagicCharge.m_uCurrentTime = 0;

        if (skillData.m_nFriendness < 0)
        {
            SetFightState(true);
        }
        CharacterLogic_Set(ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_GATHER);
        return true;
    }

    protected override bool Start_MagicChannel(CObjectCommand_Logic pLogicCommand) 
    { 
        if(pLogicCommand == null)
		    return false;

	    if(pLogicCommand.GetCommandID() != ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_CHANNEL)
		    return false;

	    if ( CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD )
		    return false;

	    CObjectCommand_MagicChannel pMagicChannelCommand = (CObjectCommand_MagicChannel)pLogicCommand;
	    if(pMagicChannelCommand.GetTotalTime() == 0)
		    return false;

	    
        _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)pMagicChannelCommand.GetMagicID());
	    if ( skillData == null )
		    return false;

	    switch ( (ENUM_SELECT_TYPE)skillData.m_nSelectType )
	    {
	    case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
	    case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
		    {
			    bool bCalcDir;
			    Vector2 fvTarget = Vector2.zero;
			    bCalcDir = false;
			    if ( pMagicChannelCommand.GetTargetObjID() != MacroDefine.UINT_MAX )
			    {
				    if ( pMagicChannelCommand.GetTargetObjID() != ServerID )
				    {
                        CObject pTarget = CObjectManager.Instance.FindServerObject((int)pMagicChannelCommand.GetTargetObjID());
					    if ( pTarget != null )
					    {
						    fvTarget.x = pTarget.GetPosition().x;
						    fvTarget.y = pTarget.GetPosition().z;
						    bCalcDir = true;
					    }
				    }
			    }
			    else
			    {
				    fvTarget.x = pMagicChannelCommand.GetTargetPos().m_fX;
				    fvTarget.y = pMagicChannelCommand.GetTargetPos().m_fZ;
				    bCalcDir = true;
			    }
			    if ( bCalcDir )
			    {
				    float fDir;
				    Vector2 fvThis = new Vector2();
				    fvThis.x = GetPosition().x;
				    fvThis.y = GetPosition().z;
                    fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget);
                    SetFaceDir(fDir);
			    }
		    }
		    break;
	    case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
            SetFaceDir(pMagicChannelCommand.GetTargetDir());
		    break;
	    case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
		    break;
	    }

	    int nRandAnimID = CSkillDataMgr.Instance.GetRandAnim((uint)pMagicChannelCommand.GetMagicID(), 0, false);
	    if ( nRandAnimID != -1 )
	    {
		    ChangeAction( nRandAnimID, GetLogicSpeed(), true,sDefaultActionFuseTime );
	    }
        
	    if ( CObjectManager.Instance.getPlayerMySelf() == this )
	    {
		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_SHOW,skillData.m_lpszName);
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH,1.0f);
	    }

	    SetCurrentLogicCommand(pLogicCommand);
	    SetLogicCount(pLogicCommand.GetLogicCount());

	    m_paramLogic_MagicChannel.m_uCurrentTime		= pMagicChannelCommand.GetTotalTime();

        if (skillData.m_nFriendness < 0)
	    {
		    SetFightState(true);
	    }
	    CharacterLogic_Set( ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_LEAD );
        return true; 
    }

    protected override bool Start_MagicSend(CObjectCommand_Logic LogicCommand) 
    {
        if (LogicCommand == null)
            return false;

        if (LogicCommand.GetCommandID() != ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_SEND)
            return false;

        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
            return false;

        CObjectCommand_MagicSend MagicSendCommand = (CObjectCommand_MagicSend)LogicCommand;

        _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)MagicSendCommand.GetMagicID());
        if (skillData == null)
            return false;

        switch ((ENUM_SELECT_TYPE)skillData.m_nSelectType)
        {
            case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
            case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
                {
                    bool bCalcDir;
                    Vector2 fvTarget = Vector2.zero;
                    bCalcDir = false;
                    if (MagicSendCommand.GetTargetObjID() != MacroDefine.UINT_MAX &&
                        MagicSendCommand.GetTargetObjID() != ServerID)
                    {

                        {
                            CObject Target = CObjectManager.Instance.FindServerObject((int)MagicSendCommand.GetTargetObjID());
                            if (Target != null)
                            {
                                fvTarget.x = Target.GetPosition().x;
                                fvTarget.y = Target.GetPosition().z;
                                bCalcDir = true;
                                // 保存落点 [10/31/2011 Ivan edit]
                                MagicSendCommand.SetTargetPos(fvTarget.x, fvTarget.y);
                            }
                        }
                    }
                    else
                    {
                        fvTarget.x = MagicSendCommand.GetTargetPos().m_fX;
                        fvTarget.y = MagicSendCommand.GetTargetPos().m_fZ;
                        bCalcDir = true;
                    }
                    if (bCalcDir)
                    {
                        float fDir;
                        Vector2 fvThis;
                        fvThis.x = GetPosition().x;
                        fvThis.y = GetPosition().z;
                        fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget);
                        SetFaceDir(fDir);
                    }
                }
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
                SetFaceDir(MagicSendCommand.GetTargetDir());
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
                break;
        }

        bool bOldSkillID;
        if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND
            && m_paramLogic_MagicSend.m_nSaveMagicID == MagicSendCommand.GetMagicID())
        {
            bOldSkillID = true;
        }
        else
        {
            bOldSkillID = false;
        }

        m_paramLogic_MagicSend.m_nSaveMagicID = MagicSendCommand.GetMagicID();
        m_paramLogic_MagicSend.m_bDoNextAction_Concatenation = false;
        m_paramLogic_MagicSend.m_bBeAttackEffectShowed = false;
        m_paramLogic_MagicSend.m_bCanBreak = false;
        m_paramLogic_MagicSend.m_nActionCount = CSkillDataMgr.Instance.GetAnimCount((uint)MagicSendCommand.GetMagicID(), true);
        m_paramLogic_MagicSend.m_uAnimationTime = 0;
        m_paramLogic_MagicSend.m_uAnimationEndElapseTime = 0;
		m_paramLogic_MagicSend.m_hitGrounded    = false;
        m_paramLogic_MagicSend.m_actionType = (ENUM_SKILL_ACTION_TYPE)skillData.m_nSkillActionType;
        // 如果是连招的特殊处理
        bool bPlayAction = true;
        int nSkillActionType = skillData.m_nSkillActionType;
        switch ((ENUM_SKILL_ACTION_TYPE)nSkillActionType)
        {
            case ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_CONCATENATION:
                {
                    if (bOldSkillID)
                    {
                        m_paramLogic_MagicSend.m_nActionIndex++;
                        if (m_paramLogic_MagicSend.m_nActionIndex >= m_paramLogic_MagicSend.m_nActionCount)
                        {
                            m_paramLogic_MagicSend.m_nActionIndex = 0;
                        }
                        bPlayAction = false;
                    }
                    else
                    {
                        m_paramLogic_MagicSend.m_nActionIndex = 0;
                    }
                }
                break;
            case ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_CONCATENATION_EX:
                if (bOldSkillID)
                {
                    m_paramLogic_MagicSend.m_nActionIndex++;
                    if (m_paramLogic_MagicSend.m_nActionIndex >= m_paramLogic_MagicSend.m_nActionCount - 1)
                    {
                        m_paramLogic_MagicSend.m_nActionIndex = 1;
                    }
                    bPlayAction = false;
                }
                else
                {
                    m_paramLogic_MagicSend.m_nActionIndex = 0;
                }
                break;
            case ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_NONE:
            default:
                {
                    if (m_paramLogic_MagicSend.m_nActionCount > 0)
                    {
                        System.Random rand = new System.Random();
                        m_paramLogic_MagicSend.m_nActionIndex = (rand.Next()) % m_paramLogic_MagicSend.m_nActionCount;
                    }
                    else
                        m_paramLogic_MagicSend.m_nActionIndex = 0;
                }
                break;
        }

        if (bPlayAction || IsAnimationEnd())
        {
            int nRandAnimID = CSkillDataMgr.Instance.GetRandAnim((uint)m_paramLogic_MagicSend.m_nSaveMagicID, m_paramLogic_MagicSend.m_nActionIndex, true);
            if (nRandAnimID != MacroDefine.INVALID_ID)
                ChangeAction(nRandAnimID, GetLogicSpeed(), false,sDefaultActionFuseTime);
            else
                m_paramLogic_MagicSend.m_nActionCount = 0;//  [4/26/2011 Sun]
        }
        else
        {
            m_paramLogic_MagicSend.m_bDoNextAction_Concatenation = true;
        }

        if (CObjectManager.Instance.getPlayerMySelf() == this)
        {
            // 物品使用不触发其他的技能公共cooldowm 
            if (!skillData.m_bAutoRedo && skillData.m_nClassByUser != 3)
            {
                // CActionSystem::GetMe().UpdateCommonCoolDown(MacroDefine.INVALID_ID);
            }
        }

        SetCurrentLogicCommand(LogicCommand);
        SetLogicCount(LogicCommand.GetLogicCount());
        // 需要处理额外的技能需求，如跳斩等 [10/31/2011 Ivan edit]
        //CharacterLogic_Set(roleLogicCmd);
        ENUM_CHARACTER_LOGIC roleLogicCmd = GetSkillLogicCmd((uint)MagicSendCommand.GetMagicID());
        DealSkillLogicCmd(roleLogicCmd);

        if (skillData.m_nFriendness < 0)
        {
            SetFightState(true);
        }
        return true;
    }

    protected void DealSkillLogicCmd(ENUM_CHARACTER_LOGIC logicCmd)
    {
	    if (logicCmd == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_JUMP_SEND)
	    {
		    //////////////////////////////////////////////////////////////////////////
            if (m_paramLogic_MagicSend.m_actionType != ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_JUMP)
            {
                m_paramLogic_MagicSend.m_hitGrounded = true;
                CharacterLogic_Set(logicCmd);
                return;
            }
            CObjectCommand_MagicSend pMagicSendCommand = (CObjectCommand_MagicSend)GetCurrentLogicCommand();
            Vector2 fvCurrentPos2D = new Vector2(GetPosition().x, GetPosition().z);
            Vector2 fvTargetPos2D  = new Vector2(pMagicSendCommand.GetTargetPos().m_fX,
			    pMagicSendCommand.GetTargetPos().m_fZ);

		    // 如果目标落点是0 0，则判断为异常 [10/31/2011 Ivan edit]
		    if (fvTargetPos2D.x == 0.0f && 
			    fvTargetPos2D.y == 0.0f )
		    {
			    return;
		    }

		    // 当前位置与当前的目标路径点路径长度的平方
		    Vector2 fvDistToTarget = fvTargetPos2D - fvCurrentPos2D;
            float fDistToTarget = fvDistToTarget.magnitude;

		    float fSpeed		= GetCharacterData().Get_MoveSpeed();
		    // 跳斩需要加速 [11/1/2011 Ivan edit]
		    fSpeed *= 1.8f;
		    float spendTime = fDistToTarget / fSpeed;

		    double jumpYSpeed = GetJumpSpeedY(spendTime);
		    //////////////////////////////////////////////////////////////////////////

		    // 需要设置跳跃 [10/31/2011 Ivan edit]
            SetbJumping(true);
            RegisterPhyEvent(PHY_EVENT_ID.PE_COLLISION_WITH_GROUND);
            //由物理层去计算
            PhyEnable(true);
            //FLOAT fTEST = 12.0; 
            AddLinearSpeed(new Vector3(0.0f, (float)jumpYSpeed, 0.0f));
	    }

	    CharacterLogic_Set(logicCmd);
    }

    protected float GetJumpSpeedY(float spendTime)
    {
        float speedY = 12.0f * spendTime;
	    return speedY;
    }

    protected override bool Tick_MagicCharge(uint uElapseTime)
    {
        CObjectCommand_MagicCharge MagicChargeCommand = (CObjectCommand_MagicCharge)GetCurrentLogicCommand();
        if (MagicChargeCommand == null)
        {
            return false;
        }

        m_paramLogic_MagicCharge.m_uCurrentTime += uElapseTime;
        if (m_paramLogic_MagicCharge.m_uCurrentTime >= MagicChargeCommand.GetEndTime())
        {
            m_paramLogic_MagicCharge.m_uCurrentTime = MagicChargeCommand.GetEndTime();
        }

        if (CObjectManager.Instance.getPlayerMySelf() == this)
        {
            float fCurTime = (float)(m_paramLogic_MagicCharge.m_uCurrentTime);
            float fTotalTime = (float)(MagicChargeCommand.GetTotalTime());
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH, fCurTime / fTotalTime);
        }

        if (m_paramLogic_MagicCharge.m_uCurrentTime >= MagicChargeCommand.GetEndTime())
        {
            bool bFinished;
            if (MagicChargeCommand.GetEndTime() == MagicChargeCommand.GetTotalTime())
            {
                bFinished = true;
            }
            else
            {
                bFinished = false;
            }
            CharacterLogic_Stop(bFinished);
        }

        // 人物的方向
        uint nTargetObjID = MagicChargeCommand.GetTargetObjID();
        if (nTargetObjID != MacroDefine.UINT_MAX)
        {
            CObject pObj = (CObject)(CObjectManager.Instance.FindServerObject((int)nTargetObjID));
            if (pObj != null && pObj != this)
            {
                Vector2 fvThis = new Vector2(GetPosition().x, GetPosition().z);
                Vector2 fvTarget = new Vector2(pObj.GetPosition().x, pObj.GetPosition().z);
                float fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget);
                SetFaceDir (fDir);
            }
        }
        return true;
    }

    protected override bool Tick_MagicChannel(uint uElapseTime)
    {
        CObjectCommand_MagicChannel MagicChannelCommand = (CObjectCommand_MagicChannel)GetCurrentLogicCommand();
        if (MagicChannelCommand == null)
        {
            return false;
        }

        if (m_paramLogic_MagicChannel.m_uCurrentTime > uElapseTime)
        {
            m_paramLogic_MagicChannel.m_uCurrentTime -= uElapseTime;
        }
        else
        {
            m_paramLogic_MagicChannel.m_uCurrentTime = 0;
        }

        if (m_paramLogic_MagicChannel.m_uCurrentTime < MagicChannelCommand.GetEndTime())
        {
            m_paramLogic_MagicChannel.m_uCurrentTime = MagicChannelCommand.GetEndTime();
        }

        if (CObjectManager.Instance.getPlayerMySelf() == this)
        {
            float fCurTime = (float)(m_paramLogic_MagicChannel.m_uCurrentTime);
            float fTotalTime = (float)(MagicChannelCommand.GetTotalTime());
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH, fCurTime / fTotalTime);
        }

        if (m_paramLogic_MagicChannel.m_uCurrentTime <= MagicChannelCommand.GetEndTime())
        {
            bool bFinished;
            if (MagicChannelCommand.GetEndTime() == 0)
            {
                bFinished = true;
            }
            else
            {
                bFinished = false;
            }
            CharacterLogic_Stop(bFinished);
        }

        // 人物的方向
        uint nTargetObjID = MagicChannelCommand.GetTargetObjID();
        if (nTargetObjID != MacroDefine.UINT_MAX)
        {
            CObject pObj = (CObject)(CObjectManager.Instance.FindServerObject((int)nTargetObjID));
            if (pObj != null && pObj != this)
            {
                Vector2 fvThis = new Vector2(GetPosition().x, GetPosition().z);
                Vector2 fvTarget = new Vector2(pObj.GetPosition().x, pObj.GetPosition().z);
                float fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget);
                SetFaceDir(fDir);
            }
        }
        return true;
    }

    protected override bool Tick_MagicSend(uint uElapseTime)
    {
        _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)m_paramLogic_MagicSend.m_nSaveMagicID);
        if (skillData == null)
            return false;

        m_paramLogic_MagicSend.m_uAnimationTime += uElapseTime;
        if (m_paramLogic_MagicSend.m_uAnimationTime > 5000)
        {
            LogManager.LogWarning("m_paramLogic_MagicSend.m_uAnimationTime > 5000");
            CharacterLogic_Stop(true);
            return true;
        }

        // 如果没有动作直接结束 [4/26/2011 Sun]
        if (IsAnimationEnd() || m_paramLogic_MagicSend.m_nActionCount == 0)
        {
            m_paramLogic_MagicSend.m_uAnimationEndElapseTime += uElapseTime;

            bool bStop = true;
            int nSkillActionType = skillData.m_nSkillActionType;
            if (!IsLogicCommandListEmpty() && (/*nSkillActionType == SKILL_ACTION_TYPE_CONCATENATION || */nSkillActionType == (int)ENUM_SKILL_ACTION_TYPE.SKILL_ACTION_TYPE_CONCATENATION_EX))
            {
                if (m_paramLogic_MagicSend.m_uAnimationEndElapseTime < 500)
                {
                    bStop = false;
                }
            }

            if (bStop)
            {
                CharacterLogic_Stop(true);
                if (CObjectManager.Instance.getPlayerMySelf() == this)
                {
                    AutoReleaseSkill.Instance.EndUseSkill();
                }
            }
        }

        if (m_paramLogic_MagicSend.m_bCanBreak && !IsLogicCommandListEmpty())
        {
            CharacterLogic_Stop(true);
            if (CObjectManager.Instance.getPlayerMySelf() == this)
            {
                AutoReleaseSkill.Instance.EndUseSkill();
            }
        }
        return true;
    }

    protected override bool Tick_Dead(uint uElapseTime)
    {
	    CCharacterData pCharacterData = GetCharacterData();
	    if(pCharacterData == null)
	    {
		    return false;
	    }

	    if(!pCharacterData.IsDie())
	    {
		    CharacterLogic_Stop(true);
	    }
	    return true;
    }

    //坐骑模型ID
    protected int m_nMountModelID = MacroDefine.INVALID_ID;
    //骑上坐骑后头顶信息板高度偏移
    protected float m_fMountAddHeight = 0.0f;
    protected bool m_bHideWeapon = false;

    //char model
    protected _DBC_CHARACTER_MODEL mCharModel;

    //monster attribute 
    protected _DBC_CREATURE_ATT mCreatureInfo;

    protected GFX.GfxObject mMountRenderInterface;//骑宠渲染接口
   
    //action set
    protected DBC.COMMON_DBC<_DBC_CHARACTER_ACTION_SET> mCharActionSetDBC;
    protected DBC.COMMON_DBC<_DBC_CHARACTER_ACTION_SET> mMountActionSetDBC;

   
    int m_GUIDServer;
    ENUM_NPC_TYPE m_eNpcType = ENUM_NPC_TYPE.NPC_TYPE_INVALID;


    public override void FillMouseCommand_Left(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;

        if ( CanbeSelect() )
	    {
		    ENUM_RELATION eCampType = GameProcedure.s_pGameInterface.GetCampType( CObjectManager.Instance.getPlayerMySelf(), this );

		    if(CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
		    {
			    pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
                return;
		    }

		    switch ( eCampType )
		    {
		    case ENUM_RELATION.RELATION_FRIEND:
			    if ( CannotBeAttack())
			    {
				    pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_SPEAK;
				    pOutCmd.SetValue(0,ServerID);
			    }
			    break;
		    case ENUM_RELATION.RELATION_ENEMY:
		    default:
			    {
				    if ( pActiveSkill != null
					    && pActiveSkill.GetType() == ACTION_OPTYPE.AOT_SKILL
					    && !CannotBeAttack() )
				    {
					    SCLIENT_SKILL pSkillImpl = (SCLIENT_SKILL)pActiveSkill.GetImpl();
                        if (pSkillImpl != null
                            && pSkillImpl.m_pDefine.m_nSelectType == (int)ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER)
					    {
						    pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_SKILL_OBJ;
                            pOutCmd.SetValue(0, pActiveSkill);
                            pOutCmd.SetValue(1, ServerID);
					    }
                    }
                    else
                    {
                        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_SELECT;
                        pOutCmd.SetValue(0, ServerID);
                    }
			    }
			    break;
		    }
	    }
    }
    public override void FillMouseCommand_Right(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;

        if ( CanbeSelect() )
	    {
		    //是物品
		    if(pActiveSkill != null && pActiveSkill.GetType() == ACTION_OPTYPE.AOT_ITEM)
		    {
			    //鼠标上挂的是一个物品

			    //空物品
// 			    if(!pActiveSkill || pActiveSkill.GetType() != AOT_ITEM) return TRUE;
// 			    CObject_Item* pItem = (CObject_Item*)(((CActionItem_Item*)pActiveSkill).GetItemImpl());
// 			    if(!pItem) return  TRUE;
// 			    //必须是能够使用的物品
// 			    if(pItem.GetItemClass() != ICLASS_COMITEM) return  TRUE;
//     			
// 			    //是否能够使用
// 			    int objID;
// 			    PET_GUID_t petID;
// 			    bool bCanuseDir = ((CObject_Item_Medicine*)pItem).IsValidTarget(this, 
// 				    fVector2(-1, -1), objID, petID);
// 
// 			    if(bCanuseDir)
// 			    {
// 				    pOutCmd.m_typeMouse = SCommand_Mouse::MCT_USE_ITEM;
// 				    pOutCmd.m_apParam[0] = (VOID*)pActiveSkill;
// 				    pOutCmd.m_adwParam[1] = GetServerID();
// 				    pOutCmd.m_adwParam[4] = FALSE;
// 			    }
		    }
		    else
		    {
			    pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_SELECT;
                pOutCmd.SetValue(0, ServerID);

// 			    ENUM_NPC_TYPE ot = GetNpcType();
// 			    switch(ot)
// 			    {
// 			    case NPC_TYPE_PET:
// 				    pOutCmd.m_typeMouse	= SCommand_Mouse::MCT_CONTEXMENU;
// 				    pOutCmd.m_adwParam[0]	= GetServerID();
// 				    break;
// 			    default:
// 				    pOutCmd.m_typeMouse	= SCommand_Mouse::MCT_PLAYER_SELECT;
// 				    pOutCmd.m_adwParam[0]	= GetServerID();
// 				    break;
// 			    }
		    }
	    }
    }

    internal float GetBoardHeight()
    {
        if (mCreatureInfo != null)
        {
            return mCreatureInfo.fBoardHeight;
        }
        else
        {
            return 0;
        }
    }

    internal bool IsDisplayBoard()
    {
        if (mCreatureInfo != null)
        {
            return mCreatureInfo.nIsDisplayerName != 0;
        }
        else
        {
            return false;
        }
    }


    // 选中环大小
    public override float GetProjtexRange()
    {
        if (GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            return mCreatureInfo != null ? mCreatureInfo.fProjTexRange : 0.0f;
        }
        else return base.GetProjtexRange();
    }
    // 阴影大小
    public override float GetShadowRange()
    {
        if (GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            return mCreatureInfo !=null ? mCreatureInfo.fShadowRange : 0.0f;
        }
        else return base.GetShadowRange();
    }

    internal bool IsCanFaceToPlayer()
    {
        if (mCreatureInfo != null)
        {
            return mCreatureInfo.nIsCanSwerve != 0;
        }
        else
        {
            return false;
        }
    }

    bool needFaceToPlayer;
    public bool NeedFaceToPlayer
    {
        get { return needFaceToPlayer; }
        set { needFaceToPlayer = value; }
    }

     public override void SetPosition(Vector3 pos)//有坐骑的时候设置坐骑的位置
     {
         if(mMountRenderInterface != null )
         {
             mMountRenderInterface.position = pos;
         }
         base.SetPosition(pos);
     }
     public override void SetScale(Vector3 scale)
     {
         if (mMountRenderInterface != null)
         {
             mMountRenderInterface.scale = scale;
         }
         base.SetScale(scale);
     }
     public override void SetFaceDir(float dir)//朝向
     {
         if (mMountRenderInterface != null)
         {
             mMountRenderInterface.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * dir, Vector3.up); ;
         }
         base.SetFaceDir(dir);
     }
     public override void Enable(uint dwFlag)
     {
         base.Enable(dwFlag);

         switch (dwFlag)
         {
             case (uint)ObjectStatusFlags.OSF_VISIABLE:
                 if (mMountRenderInterface != null) mMountRenderInterface.SetVisible(true);
                 break;

             case (uint)ObjectStatusFlags.OSF_RAY_QUERY:
                 if (mMountRenderInterface != null) mMountRenderInterface.SetRayQuery(true);
                 break;

             default:
                 break;
         }
     }
     public override void Disalbe(uint dwFlag)
     {
         base.Disalbe(dwFlag);
         switch (dwFlag)
         {
             case (uint)ObjectStatusFlags.OSF_VISIABLE:
                 if (mMountRenderInterface != null ) mMountRenderInterface.SetVisible(false);
                 break;

             case (uint)ObjectStatusFlags.OSF_RAY_QUERY:
                 if (mMountRenderInterface != null ) mMountRenderInterface.SetRayQuery(false);
                 break;
             default:
                 break;
         }
     }

     // 更新信息面板 [1/31/2012 Ivan]
     protected override void Tick_UpdateInfoBoard()
     {
         if (m_pInfoBoard == null || GetRenderInterface() == null) return;

         //隐藏设置
         if (!CObjectManager.Instance.IsAllObjectInfoBoardShow())
         {
             m_pInfoBoard.Show(false);
             return;
         }
         else if (GetCharacterData().Get_Name().Length == 0)
         {
             m_pInfoBoard.Show(false);
             return;
         }
         else if (CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD && GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
         {
             m_pInfoBoard.Show(false);
             return;
         }
         else if(GetCharacterType() == CHARACTER_TYPE.CT_MONSTER && !this.IsDisplayBoard())
         {
             m_pInfoBoard.Show(false);
             return;
         }
      

         //计算位置
         Vector3 fvPos = new Vector3();
         Vector3 fvCurPos = GetPosition();
         if (mMountRenderInterface != null)
         {
             fvCurPos.y += m_fMountAddHeight;
         }

         bool bVisible;

         if (GetCharacterType() == CHARACTER_TYPE.CT_MONSTER &&
             (((CObject_PlayerNPC)this).GetBoardHeight() > 0))
             bVisible = GetRenderInterface().GetInfoBoardPos(ref fvPos, ref fvCurPos, ((CObject_PlayerNPC)this).GetBoardHeight());
         else
             bVisible = GetRenderInterface().GetInfoBoardPos(ref fvPos, ref fvCurPos);

         if (!bVisible)
         {
             m_pInfoBoard.Show(false);
             return;
         }

         //if (IsShaking(fvPos))
         //    return;

         // 设置离主角的距离
         if (CObjectManager.Instance.getPlayerMySelf() != null)
         {
             Vector3 fMyselfPos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
             float fDistance = Utility.TDU_GetDistSq(fMyselfPos, GetPosition());
             m_pInfoBoard.SetInfoDistance(fDistance);
         }

         m_pInfoBoard.SetPosition(new Vector2(fvPos.x, fvPos.y));
     }

     protected override bool Tick_JumpMagicSend(uint uElapseTime) 
     { 
        CObjectCommand_MagicSend pMagicSendCommand = (CObjectCommand_MagicSend)GetCurrentLogicCommand();
	    if(pMagicSendCommand == null)
	    {
		    return false;
	    }
        _DBC_SKILL_DATA skillData  = CSkillDataMgr.Instance.GetSkillData((uint)m_paramLogic_MagicSend.m_nSaveMagicID);
	    if ( skillData == null )
		    return false;

	    m_paramLogic_MagicSend.m_uAnimationTime += uElapseTime;
	    // 设置一个结束时间，以防止异常 [10/31/2011 Ivan edit]
	    if(m_paramLogic_MagicSend.m_uAnimationTime > 5000)
	    {
            CharacterLogic_Stop(true);
            if (CObjectManager.Instance.getPlayerMySelf() == this)
            {
                AutoReleaseSkill.Instance.EndUseSkill();
            }
		    return true;
	    }

	    // 进入跳斩移动逻辑(和移动逻辑类似) [10/31/2011 Ivan edit]
	    
       
	    Vector2 fvTargetPos2D = new Vector2( pMagicSendCommand.GetTargetPos().m_fX,pMagicSendCommand.GetTargetPos().m_fZ);
        Vector2 fvCurrentPos2D = new Vector2( GetPosition().x,GetPosition().z);
	    // 如果目标落点是0 0，则判断为异常 [10/31/2011 Ivan edit]
	    if (fvTargetPos2D.x == 0.0f && 
		    fvTargetPos2D.y == 0.0f )
	    {
		    CharacterLogic_Stop(true);
            if (CObjectManager.Instance.getPlayerMySelf() == this)
            {
                AutoReleaseSkill.Instance.EndUseSkill();
            }
            return false;
	    }
	    // 当前位置与当前的目标路径点路径长度的平方
	    Vector2 fvDistToTarget = fvTargetPos2D - fvCurrentPos2D;
	    float fDistToTargetSq = fvDistToTarget.x * fvDistToTarget.x + fvDistToTarget.y * fvDistToTarget.y;
        float fDistToTarget = Mathf.Sqrt(fDistToTargetSq);

	    // 这一帧可移动的路径长度
        float fElapseTime = ((float)(uElapseTime)) / 1000.0f;
	    float fSpeed		= GetCharacterData().Get_MoveSpeed();
	    // 跳斩需要加速 [11/1/2011 Ivan edit]
	    fSpeed *= 1.8f;
        float fMoveDist = fSpeed * fElapseTime;

	    if(fMoveDist < 0.01f)
	    {
		    // 帧数太高时由于timeGetTime()函数精度不够，会出现ElapseTime为0的情况
		    // 因此返回TRUE[11/29/2010 Sun]
		    return true;
	    }

        if (fDistToTarget < 0.01f && m_paramLogic_MagicSend.m_hitGrounded && IsAnimationEnd())
	    {
		    CharacterLogic_Stop(true);
            if (CObjectManager.Instance.getPlayerMySelf() == this)
            {
                AutoReleaseSkill.Instance.EndUseSkill();
            }
            return true;
	    }

	    bool bStopMove = false;
	    Vector2 fvSetToPos = fvCurrentPos2D;
        float fSetToDir = GetFaceDir();
	    while(true)
	    {
		    if(fMoveDist > fDistToTarget)
		    {
			    // 走到了
			    bStopMove	= true;
			    fvSetToPos	= fvTargetPos2D;
                fSetToDir = GFX.GfxUtility.GetYAngle(fvCurrentPos2D, fvTargetPos2D);
			    break;
		    }
		    else
		    {
			    float fDistX = (fMoveDist*(fvTargetPos2D.x-fvCurrentPos2D.x))/fDistToTarget;
			    float fDistZ = (fMoveDist*(fvTargetPos2D.y-fvCurrentPos2D.y))/fDistToTarget;

			    fvSetToPos.x = fvCurrentPos2D.x + fDistX;
			    fvSetToPos.y = fvCurrentPos2D.y + fDistZ;

			    // 跳斩应该可以跳过障碍物
    // 			if( CObjectManager::GetMe()->GetMySelf() == this && CPath::IsPointInUnreachRegion(fvSetToPos))
    // 			{
    // 				// 如果即将进入不可行走区域，则立即停止 [10/31/2011 Ivan edit]
    // 				fvSetToPos = fvCurrentPos2D;
    // 				bStopMove = TRUE;
    // 			}

			    fSetToDir = GFX.GfxUtility.GetYAngle(fvCurrentPos2D, fvTargetPos2D);
			    break;
		    }
	    }

	    SetFaceDir(fSetToDir);
	    SetMapPosition( fvSetToPos.x, fvSetToPos.y);
	    if(bStopMove && m_paramLogic_MagicSend.m_hitGrounded && IsAnimationEnd())
	    {
		    CharacterLogic_Stop(true);
            if (CObjectManager.Instance.getPlayerMySelf() == this)
            {
                AutoReleaseSkill.Instance.EndUseSkill();
            }
	    }
        return true; 
     }


     Vector3 lastPos = Vector3.zero;
     int lastXDir = 0;
     int lastYDir = 0;
     bool IsShaking(Vector3 newPos)
     {
         if (lastPos == Vector3.zero)
         {
             // 第一次强制更新
             lastPos = newPos;
             return false;
         }

         if (this == CObjectManager.Instance.getPlayerMySelf())
         {
             bool isScaleCamera = Math.Abs(newPos.y - lastPos.y) > 5 || Math.Abs(newPos.x - lastPos.x) > 5;
             lastPos = newPos;
             // 自身的坐标永远在屏幕中间，只有缩放镜头的时候需要刷新，防止震颤
             if (isScaleCamera)
                 return false;
             else
                 return true;
         }

         //         if (lastXDir == 0 && lastYDir == 0)
         //         {
         //             lastXDir = newPos.x > lastPos.x ? 1 : -1;
         //             lastYDir = newPos.y > lastPos.y ? 1 : -1;
         //             return false;
         //         }

         int newXDir = newPos.x > lastPos.x ? 1 : -1;
         int newYDir = newPos.y > lastPos.y ? 1 : -1;

         lastPos = newPos;

         if (lastXDir == newXDir && lastYDir == newYDir)
         {
             return false;
         }
         else
         {
             lastXDir = newXDir;
             lastYDir = newYDir;
             return true;
         }
     }

     internal bool IsBoss()
     {
         return mCreatureInfo.nMonsterBossFlag == 1;
     }

     internal string GetPortrait()
     {
         if (mCreatureInfo != null)
         {
             return mCreatureInfo.szIconName;
         }
         else
         {
             return "";
         }
     }

     NpcMissState currMissState = NpcMissState.None;
     bool missStateChanged = false;
     internal void UpdateMissionState()
     {
         NpcMissState old = currMissState;
         currMissState = MissionList.Instance.GetNpcMissState(GetCharacterData().Get_RaceID());

         if (old != currMissState)
             missStateChanged = true;
     }

     void Tick_UpdateEffect()
     {
         if (missStateChanged)
         {
             m_pInfoBoard.ShowMissState(currMissState);

             missStateChanged = false;
         }
     }
		public override void NotifyPhyEvent(PHY_EVENT_ID eventid, object pParam)
	    {
	        switch (eventid)
	        {
	            case PHY_EVENT_ID.PE_COLLISION_WITH_GROUND:
	                {
	                    if (GetbJumping())
	                    {//落地了
	                        SetbJumping(false);

	                        PhyEnable(false);
	                        UnRegisterPhyEvent(PHY_EVENT_ID.PE_COLLISION_WITH_GROUND);
	                        m_paramLogic_MagicSend.m_uAnimationTime = 0;
	                        if (mRenderInterface != null)
	                        {
	                            mRenderInterface.OnHitGroundEvent();
	                        }
	                        SetAnimationEnd(false);
	                    }
	                }
	                break;
	            default:
	                break;
	        }
	        return;
	    }
}
