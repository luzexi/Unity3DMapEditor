using UnityEngine;
using System;
using System.Collections.Generic;
using DBSystem;
using Network;
using Network.Packets;

public enum ENUM_MYSELF_AI
{
	MYSELF_AI_INVALID	= -1,
	MYSELF_AI_IDLE,					//空闲中
	MYSELF_AI_MOVE,					//移动中
	MYSELF_AI_USE_SKILL,			//技能使用中
	MYSELF_AI_ACTIVE_TRIPPEROBJ,	//对TripperObj激活中
	MYSELF_AI_DEFAULT_EVENT,		//对npc谈话请求中
	MYSELF_AI_FOLLOW,				//跟随
	MYSELF_AI_GUAJI,				//挂机
	MYSELF_AI_MOVE_ALONG,			//直线行走
	MYSELF_AI_NUMBERS
};

public class CAI_MySelf : CAI_Base
{
    public override bool Tick()
    {
        AutoReleaseSkill.Instance.Update();
		if(m_uForbidTime > 0)
	    {
            if (GameProcedure.s_pTimeSystem.GetTimeNow() - m_uForbidStartTime >= m_uForbidTime)
		    {
			    m_uForbidTime = 0;
		    }
	    }
		
	    if ( m_SaveCommand.m_wID != (int)AICommandDef.AIC_NONE )
	    {
		    if(m_uForbidTime == 0)
		    {
			    if ( !IsLimitCmd(m_SaveCommand ) )
			    {
				    RC_RESULT rcResult = OnCommand(m_SaveCommand );
				    if ( RC_RESULT.RC_WAIT != rcResult )
				    {
					    m_SaveCommand.m_wID = (int)AICommandDef.AIC_NONE;
				    }
			    }
			    else
			    {
				    m_SaveCommand.m_wID = (int)AICommandDef.AIC_NONE;
			    }
		    }
	    }

	    bool bResult;
	    switch( GetMySelfAI() )
	    {
            case ENUM_MYSELF_AI.MYSELF_AI_IDLE:
                {
                    bResult = Tick_Idle();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_MOVE:
                {
                    bResult = Tick_Move();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_USE_SKILL:
                {
                    bResult = Tick_UseSkill();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_ACTIVE_TRIPPEROBJ:
                {
                    bResult = Tick_ActiveTripperObj();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_DEFAULT_EVENT:
                {
                    bResult = Tick_DefaultEvent();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_FOLLOW:
                {
                    bResult = Tick_Follow();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_GUAJI:
                {
                    bResult = Tick_AutoHit();
                }
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_MOVE_ALONG:
                {
                    bResult = Tick_MoveAlong();
                }
                break;
		    default:
			    bResult = false;
			    break;
	    }
	    return bResult;
    }
  
    //重置
    public override void Reset()
    {
        OnAIStopped();
    }

    public override bool PushCommand_MoveTo(float fDestX, float fDestZ, bool bAutoMove)
    {
        if (StartGuaJi == 1)
            StartGuaJi = GameProcedure.s_pGameInterface.AutoHitState;
        SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_MOVE;
        cmdTemp.SetValue<float>(0, fDestX);
        cmdTemp.SetValue<float>(1, fDestZ);
        if (bAutoMove) 
            cmdTemp.SetValue<float>(2, 1.0f);
        else
            cmdTemp.SetValue<float>(2, 0.0f);
        return PushCommand(cmdTemp);
    }

    public override bool PushCommand_AutoHit(int isAutoHit)
    {
        CObjectManager.Instance.ResetNearestTarget();
		SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_GUAJI;
        cmdTemp.SetValue<int>(0, isAutoHit);
        return PushCommand(cmdTemp);
    }
   
    public override bool PushCommand_Jump()
    {
        if (StartGuaJi == 1)
            StartGuaJi = GameProcedure.s_pGameInterface.AutoHitState;
        if (RC_RESULT.RC_OK == Jump())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool PushCommand_UseSkill(int idSkill, uint guidTarget, int idTargetObj, float fTargetX, float fTargetZ, float fDir)
    {
        if (StartGuaJi == 1)
            StartGuaJi = GameProcedure.s_pGameInterface.AutoHitState;
        SCommand_AI cmdTemp = new SCommand_AI();
        cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
        cmdTemp.SetValue<int>(0, idSkill);
        cmdTemp.SetValue<int>(1, idTargetObj);
        cmdTemp.SetValue<float>(2, fTargetX);
        cmdTemp.SetValue<float>(3, fTargetZ);
        cmdTemp.SetValue<float>(4, fDir);
        cmdTemp.SetValue<uint>(5, guidTarget);
        return PushCommand(cmdTemp);
    }

	    // 压入一条指令
    public override bool PushCommand(SCommand_AI cmd )
    {
        // 先清空，不管此次操作是否为空
        m_SaveCommand.m_wID = (int)AICommandDef.AIC_NONE;
        if (!IsLimitCmd(cmd))
        {

            if (m_uForbidTime == 0)
            {
                RC_RESULT rcResult = OnCommand(cmd);

                if (rcResult != RC_RESULT.RC_WAIT)
                {// 如果执行成功则将缓存清空

                    m_SaveCommand.m_wID = (int)AICommandDef.AIC_NONE;

                    // 如果执行成功，那么直接返回，不再保存，以防重复执行 [8/4/2010 Sun]
                    return true;
                }
            }

            // 先将pCmd缓存起来
            m_SaveCommand = cmd;

        }
        return true;
    }

    public bool IsMoving()
    {
	    return GetMySelfAI() == ENUM_MYSELF_AI.MYSELF_AI_MOVE;
    }

    public bool IsFollow()
    {
	    return GetMySelfAI() == ENUM_MYSELF_AI.MYSELF_AI_FOLLOW;
    }

	    //当前AI逻辑状态
    public ENUM_MYSELF_AI GetMySelfAI()
    {
	    return m_eMySelfAI;
    }

    protected void				SetForbidTime(uint nForbidTime)
    {
        m_uForbidTime = nForbidTime;
        m_uForbidStartTime = GameProcedure.s_pTimeSystem.GetTimeNow();
    }

    protected SCommand_AI		m_SaveCommand;
    protected uint				m_uForbidTime;		// 禁止操作的时间
    protected uint				m_uForbidStartTime;	// 禁止操作的起始时间
    protected uint				StartGuaJi;	// 禁止操作的起始时间

    protected override RC_RESULT	OnCommand(SCommand_AI cmd )
    {
        RC_RESULT rcResult;

	    switch ( (AICommandDef)cmd.m_wID )
	    {
	        case AICommandDef.AIC_MOVE:
		        {
			        fVector2 fv2TargetPos;
			        fv2TargetPos.x	= cmd.GetValue<float>(0);
                    fv2TargetPos.y = cmd.GetValue<float>(1);
                    bool bAutoMove = ((cmd.GetValue<float>(2)) == 1.0f);
                    bool requestMount = CObjectManager.Instance.getPlayerMySelf().RideToMove(new Vector3(fv2TargetPos.x,0, fv2TargetPos.y),bAutoMove);
                    if (requestMount == false)//如果正在请求坐骑则跳过移动  [3/13/2012 ZZY]
                    rcResult = Enter_Move(fv2TargetPos.x, fv2TargetPos.y, bAutoMove);
                    else rcResult = RC_RESULT.RC_OK;
		        }
		        break;

	        case AICommandDef.AIC_USE_SKILL:
		        {
			        int dwTargetID, dwSkillID, dwSkillLevel;
			        WORLD_POS posTarget;
			        float     fDir;
                    uint guidTarget;
                    dwSkillID       = cmd.GetValue<int>(0);
                    dwSkillLevel    = (int)cmd.GetValue<byte>(1);
                    dwTargetID      = cmd.GetValue<int>(2);
                    posTarget.m_fX  = cmd.GetValue<float>(3);
                    posTarget.m_fZ  = cmd.GetValue<float>(4);
                    fDir            = cmd.GetValue<float>(5);
                    guidTarget      = cmd.GetValue<uint>(6);
			        rcResult        = Enter_UseSkill( dwSkillID, dwSkillLevel, guidTarget, dwTargetID, posTarget.m_fX, posTarget.m_fZ, fDir );
		        }
		        break;

	        case AICommandDef.AIC_TRIPPER_ACTIVE:
		        {
			        int	idItemBox;
			        idItemBox	= cmd.GetValue<int>(0);
			        rcResult    = Enter_ActiveTripperObj( idItemBox );
		        }
		        break;

	        case AICommandDef.AIC_COMPOSE_ITEM:
		        {
			        int	idPrescription;
                    idPrescription = cmd.GetValue<int>(0);
                    uint uFlag = cmd.GetValue<uint>(1);
			        rcResult = ComposeItem( idPrescription, uFlag );
		        }
		        break;

	        case AICommandDef.AIC_DEFAULT_EVENT:
		        {
			        int dwTargetID;
                    dwTargetID = cmd.GetValue<int>(0);
			        rcResult = Enter_DefaultEvent( dwTargetID );
		        }
		        break;
	        case AICommandDef.AIC_JUMP:
		        {
			        rcResult = Jump();
		        }
		        break;

	        case AICommandDef.AIC_FOLLOW:
		        {
			        uint idTargetObj;
                    idTargetObj = cmd.GetValue<uint>(0);
			        bool bResult = Enter_Follow(idTargetObj);
			        if ( bResult )
			        {
				        rcResult = RC_RESULT.RC_OK;
			        }
			        else
			        {
				        rcResult = RC_RESULT.RC_ERROR;
			        }
		        }
		        break;
	        case AICommandDef.AIC_GUAJI:
		        {
			        if (cmd.GetValue<int>(0) == 1)
			        {
				        SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_GUAJI );
				        StartGuaJi = 1;
			        }
                    else
			        {
				        SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_IDLE );
				        StartGuaJi = 0;
                      
				      //  CObjectManager::GetMe().m_pNearestTargetOLD = NULL;
			        }
			        rcResult = RC_RESULT.RC_OK;
		        }
		        break;
	        case AICommandDef.AIC_MOVE_ALONG:
		        {
			        float fSpeed = cmd.GetValue<float>(0);
			        rcResult = Enter_MoveAlong(fSpeed, cmd.GetValue<float>(1));
		        }
		        break;
	        case AICommandDef.AIC_CMD_AFTERMOVE:
		        {
                    Vector2 pos = cmd.GetValue<Vector2>(2);
                    rcResult = Enter_CmdAfterMove((CMD_AFTERMOVE_TYPE)cmd.GetValue<int>(0), cmd.GetValue<int>(1), pos);
		        }
		        break;
	        default:
		        rcResult = RC_RESULT.RC_SKIP;
		        break;
	    };
	    return rcResult;
    }

    protected RC_RESULT	AI_MoveTo( float fDestX, float fDestZ, bool bAutoFind, bool bCircle, int nCircleCount)
    {
        CObject_PlayerMySelf mySelf = CObjectManager.Instance.getPlayerMySelf();
        CCharacterData pCharData = mySelf.GetCharacterData();

        if (pCharData.IsLimitMove())
            return RC_RESULT.RC_ERROR;

        if (pCharData.IsDie())
            return RC_RESULT.RC_ERROR;

        Vector3 MyselPos = mySelf.GetPosition();
        Vector2 myPos = new Vector2(MyselPos.x, MyselPos.z);
        Vector2 tarPos = new Vector2(fDestX, fDestZ);

        bool result = CPath.Instance.CreateMovePath(myPos, tarPos);
        if (result)
        {
            // 发送到服务器
            int nSimulantLogicCount = mySelf.SimulantLogicCount;
            if (mySelf.GetLogicCount() > nSimulantLogicCount)
            {
                nSimulantLogicCount = mySelf.GetLogicCount();
            }
            int nLogicCount = nSimulantLogicCount + ObjectDef.DEF_CLIENT_LOGIC_COUNT_ADD_STEP;
            mySelf.SimulantLogicCount = nLogicCount;

            SCommand_Object cmd = new SCommand_Object();
            WORLD_POS[] posTarget = new WORLD_POS[1];
            posTarget[0].m_fX = CPath.Instance.PosStack[0].fvTarget.x;
            posTarget[0].m_fZ = CPath.Instance.PosStack[0].fvTarget.y;

            cmd.m_wID = (int)OBJECTCOMMANDDEF.OC_MOVE;
            cmd.SetValue(0, (uint)0);
            cmd.SetValue(1, nLogicCount);
            cmd.SetValue(2, 1);
            cmd.SetValue(3, posTarget);
            mySelf.DoSimulantCommand(cmd);

            SendMoveMessage(CPath.Instance, nLogicCount);
        }
        else
        {
            LogManager.LogWarning("目标点无法到达!");
        }

        return RC_RESULT.RC_OK;
    }

    //向服务器发送命令
    public void SendMoveMessage(CPath pPath, int nLogicCount)
    {
        CObject_Character pCharacter = CObjectManager.Instance.getPlayerMySelf();
        WORLD_POS posCurrent;
        posCurrent.m_fX = pCharacter.GetPosition().x;
        posCurrent.m_fZ = pCharacter.GetPosition().z;

        //向服务器发送请求消息
        CGCharMove msg = new CGCharMove();
        msg.ObjID = (uint)pCharacter.ServerID;
        msg.HandleID = nLogicCount;
        msg.PosWorld = posCurrent;

        uint dwNumPathNode = (uint)(pPath.PosStack.Count);
        if (dwNumPathNode == 0)
            return;

        int i;
        WORLD_POS posTarget;
        for (i = 0; i < dwNumPathNode; i++)
        {
            PathUnit puTemp = pPath.PosStack[i];

            posTarget.m_fX = puTemp.fvTarget.x;
            posTarget.m_fZ = puTemp.fvTarget.y;

            //检测是否合法
            // 		    if(!CWorldManager::GetMe().GetActiveScene().IsValidPosition(puTemp.fvTarget))
            // 		    {
            // 			    return;
            // 		    }

            msg.addTargetPos(posTarget);
        }
        GameProcedure.s_NetManager.SendPacket(msg);
    }


    const float TDU_PI	                = 3.1415926535f;
    const float MOVE_AROUND_ANGLE_STEP  = 5 * TDU_PI / 180.0f;
    const float TURN_ANGLESPEED         = 200*TDU_PI/180.0f;
    protected int GetCirclePos(float fEndDir, float fAngleStep)
    {
        Vector2 fvEnd;
	    float fCurDir = CObjectManager.Instance.getPlayerMySelf().GetFaceDir();
	    Vector2 fvCurPos;
        fvCurPos.x = CObjectManager.Instance.getPlayerMySelf().GetPosition().x;
        fvCurPos.y = CObjectManager.Instance.getPlayerMySelf().GetPosition().z;
	    float fDis = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_MoveSpeed()*fAngleStep/TURN_ANGLESPEED;
	    int i;
	    Vector2 fNode;
	    m_fv2CirclePath.Clear();
	    for (i = 0; i < Mathf.Abs(fEndDir - fCurDir)/Mathf.Abs(fAngleStep); i++)
	    {
            fvEnd.x = Mathf.Sin(fCurDir + fAngleStep * i) * fDis;
		    fvEnd.y = Mathf.Cos(fCurDir + fAngleStep * i) * fDis;

		    fNode.x = fvCurPos.x + fvEnd.x;
		    fNode.y = fvCurPos.y + fvEnd.y;
            m_fv2CirclePath.Add(fNode);
		    fvCurPos = fNode;
	    }
	    return i;
    }

	    // 跳跃相关
    public bool IsCanJump()
    {
        CObject_Character Character = GetCharacter();
	    CCharacterData CharData = Character.GetCharacterData();
        if (CharData.IsLimitMove())
            return false;

        if (CharData.IsDie())
            return false;

        if (Character.GetbJumping())
            return false;

        bool bCharacterLogicCanJump = false;
        ENUM_CHARACTER_LOGIC eCharacterLogic = Character.CharacterLogic_Get();
        switch (eCharacterLogic)
        {
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_GATHER:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_LEAD:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ABILITY_ACTION:
                bCharacterLogicCanJump = true;
                break;
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_ACTION:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_SEND:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD:
            case ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_STALL:
            default:
                bCharacterLogicCanJump = false;
                break;
        }
        if (!bCharacterLogicCanJump)
            return false;

	    return true;
    }
    public RC_RESULT Jump()
    {
        if ( IsCanJump() )
        {
            CObject_Character Character = GetCharacter();
            bool bResult = Character.Jump();
            if (!bResult)
                return RC_RESULT.RC_ERROR;
            // send jump msg to server.
            CGCharJump msg = new CGCharJump();
            msg.ObjID = Character.ServerID;
            NetManager.GetNetManager().SendPacket(msg);
            return RC_RESULT.RC_OK;
        }

        return RC_RESULT.RC_ERROR;
    }

    public int Find_Platform(int idAbility)
    {
        int guidPlatform = MacroDefine.INVALID_ID;
        //int find_platform = 0;
        //CObject_Character Character = GetCharacter();

        //static const tDataBase* pLifeAbilityDBC = CDataBaseSystem::GetMe().GetDataBase(DBC_LIFEABILITY_DEFINE);
        //const _DBC_LIFEABILITY_DEFINE* m_pAbilityDef = (const _DBC_LIFEABILITY_DEFINE*)pLifeAbilityDBC.Search_Index_EQU(idAbility);

        //INT idPlatform	= m_pAbilityDef.nPlatformID;
        //FLOAT distPlatform	= m_pAbilityDef.fPlatformDist;

        //INT my_level =  CObjectManager::GetMe().GetMySelf().GetCharacterData().Get_Level();
        //if( my_level < m_pAbilityDef.nLevelNeed ) 
        //{
        //    ADDTALKMSG("人物等级不够。");
        //    return INVALID_ID;
        //}

        //if(idPlatform >=0)
        //{
        //    CScene* pActiveScene = (CScene*)CWorldManager::GetMe().GetActiveScene();
        //    if(!pActiveScene) return FALSE;

        //    fVector3 fvTarget,fvPlayerMySelf;

        //    fvPlayerMySelf	= pCharacter.GetPosition();

        //    INT zone_x = pActiveScene.GetZoneX(fvPlayerMySelf.x);
        //    INT zone_z = pActiveScene.GetZoneX(fvPlayerMySelf.z);
        //    CZone* pZone;

        //    INT final_x = zone_x + 1;
        //    INT final_z = zone_z + 1;
        //    //在当前zone和周围8个zone之中，寻找这个platform
        //    for( zone_x = final_x - 2; zone_x < final_x; zone_x++ )
        //        for( zone_z=final_z - 2; zone_z < final_z; zone_z++ )
        //            {
    	
        //                pZone = pActiveScene.GetZone(zone_x,zone_z);
        //                if(!pZone) continue;

        //                FLOAT fDistSq = 0.0f;
        //                for (std::set<INT>::iterator it = pZone.m_setRegisterObject.begin();it!=pZone.m_setRegisterObject.end();it++)
        //                {		
        //                    CObject *pTarget = (CObject*)(CObjectManager::GetMe().FindObject((INT)*it));
        //                    if(!pTarget) 
        //                    {
        //                        continue;
        //                    }
        //                    if(  ( tObject::TOT_PLATFORM == pTarget.Tripper_GetType() ) && ( idPlatform == ((CTripperObject_Platform*)(CTripperObject*)pTarget).GetPlatformID() ) )
        //                    {
        //                        fvTarget	= pTarget.GetPosition();
        //                        fDistSq		= TDU_GetDistSq( fvTarget, fvPlayerMySelf );
    							
        //                        if ( fDistSq < distPlatform * distPlatform ) 
        //                        {
        //                            guidPlatform = pTarget.GetServerID();
        //                            find_platform = 1;				
        //                            break;
        //                        }
        //                    }
        //                }
        //                //此处应检查是否满足合成之条件，如原材料，技能，配方，平台等 
        //                //
        //                //距离不够，不会走到平台前
        //                //if ( fDistSq > distPlatform )
        //                //{

        //                //	BOOL bResult = MoveTo( fvTarget.x, fvTarget.z );
        //                //	if ( !bResult )
        //                //		return FALSE;
        //                //}
        //            }
        //            if(!find_platform) 
        //            {
        //                ADDTALKMSG("附近没有操作平台。");
        //                return INVALID_ID;
        //            }
        //}
	    return guidPlatform;

    }

    public RC_RESULT ComposeItem(int idPrescription, uint uFlag)
    {
        CObject_Character Character = GetCharacter();
        CCharacterData CharData = Character.GetCharacterData();
        //if ( pCharData.IsLimitHandle() )
        //	return RC_ERROR;

        if (CharData.IsDie())
            return RC_RESULT.RC_ERROR;

        if (Character.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
            return RC_RESULT.RC_ERROR;

        int guidPlatform = MacroDefine.INVALID_ID;
        int find_platform = 0;
	    //此处应检查是否满足合成之条件，如原材料，技能，配方，平台等 
    /////////////////////////
        DBC.COMMON_DBC<_DBC_LIFEABILITY_ITEMCOMPOSE> prescrDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_ITEMCOMPOSE>((int)DataBaseStruct.DBC_LIFEABILITY_ITEMCOMPOSE);
        _DBC_LIFEABILITY_ITEMCOMPOSE prescrDef = prescrDBC.Search_Index_EQU(idPrescription);
        if (prescrDef == null)
            throw new NullReferenceException("Prescr define id null id=" + idPrescription);
        int idAbility = prescrDef.nLifeAbility;

        DBC.COMMON_DBC<_DBC_LIFEABILITY_DEFINE> lifeAbilityDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_DEFINE>((int)DataBaseStruct.DBC_LIFEABILITY_DEFINE);
        _DBC_LIFEABILITY_DEFINE abilityDef = lifeAbilityDBC.Search_Index_EQU(idAbility);
        if (abilityDef == null)
            throw new NullReferenceException("lifeability define is null id=" +idAbility);

        int idPlatform = abilityDef.nPlatformID;
        float distPlatform = abilityDef.fPlatformDist;

        int my_level =  CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level();
        if( my_level < abilityDef.nLevelNeed ) 
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "#{LowLevel}");

            return RC_RESULT.RC_ERROR;
        }

        SCLIENT_LIFEABILITY player_ability = 
            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_LifeAbility(idAbility);
        if (player_ability == null)
            throw new NullReferenceException("The ability is not learn id=" + idAbility);

        int player_ability_level = player_ability.m_nLevel;
        if( player_ability_level < prescrDef.nLifeAbility_Level) 
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "#{Skill LowLevel}");
            return RC_RESULT.RC_ERROR;
        }
        if(idPlatform>=0)
        {
            guidPlatform = Find_Platform(idAbility);
            if(guidPlatform == MacroDefine.INVALID_ID)
                return RC_RESULT.RC_ERROR;
        }
        // 站在原地即可开始合成，无需移动 

        // 参数赋值
        SendComposeItemMessage( idAbility, idPrescription, (uint)guidPlatform, uFlag);
        //CGameProcedure::s_pGfxSystem.PushDebugString("CGCharComposeItem[%d],[%d],[%d]", idAbility, idPrescription, guidPlatform);
        return RC_RESULT.RC_OK;

    }

    public bool IsLimitCmd(SCommand_AI cmd)
    {
         CObject_PlayerMySelf MySelf = (CObject_PlayerMySelf)GetCharacter();
         CCharacterData CharData = MySelf.GetCharacterData();
        //CUIDataPool* pUIDataPool = CUIDataPool::GetMe();
        //// 实际上这一步不应该放以这里检测，应该放在UI里
        //// 检测是否为组队跟随状态
        //{
        //    BOOL bTeamFollowMode = pCharData.Get_TeamFollowFlag();
        //    BOOL bRet = FALSE;

        //    if ( bTeamFollowMode )
        //    { // 组队跟随状态
        //        CTeamOrGroup* pTeam = pUIDataPool.GetTeamOrGroup();
        //        BOOL bIsLeader = (pTeam.GetLeaderGUID() == pMySelf.GetServerGUID());

        //        switch ( pCmd.m_wID )
        //        {
        //        case AIC_MOVE:
        //        case AIC_DEFAULT_EVENT:
        //        case AIC_FOLLOW:
        //        case AIC_MOVE_ALONG:
        //            {
        //                if ( bIsLeader==FALSE )
        //                {
        //                    bRet = TRUE;
        //                }
        //            }
        //            break;
        //        case AIC_USE_SKILL:
        //        case AIC_TRIPPER_ACTIVE:
        //        case AIC_COMPOSE_ITEM:
        //        case AIC_GUAJI:
        //        case AIC_CMD_AFTERMOVE:
        //            {
        //                bRet = TRUE;
        //            }
        //            break;
        //        default:
        //            break;
        //        };

        //        if ( bRet==TRUE )
        //        {
        //            CGameProcedure::s_pEventSystem.PushEvent(GE_INFO_SELF,"组队跟随状态,不允许进行这种操作");
        //        }
        //    }

        //    if ( bRet )
        //        return TRUE;
        //}

        //// 人在载具上的时候不能行动 [8/16/2011 ivan edit]
        //// 角色在执行跳斩技能的时候禁止其他行为 [10/31/2011 Ivan edit]
        if (/*CharData.IsInBusAndLimitMove() ||*/
            MySelf.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_SKILL_JUMP_SEND)
        {
            bool bRet = false;
            switch ((AICommandDef)cmd.m_wID)
            {
                case AICommandDef.AIC_MOVE:
                case AICommandDef.AIC_DEFAULT_EVENT:
                case AICommandDef.AIC_FOLLOW:
                case AICommandDef.AIC_MOVE_ALONG:
                case AICommandDef.AIC_USE_SKILL:
                case AICommandDef.AIC_TRIPPER_ACTIVE:
                case AICommandDef.AIC_COMPOSE_ITEM:
                case AICommandDef.AIC_GUAJI:
                case AICommandDef.AIC_CMD_AFTERMOVE:
                    {
                        bRet = true;
                    }
                    break;
                default:
                    break;
            };
            if ( bRet==true )
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "此时不允许进行这种操作。");
                return true;
            }
        }

        CDataPool pDataPool = CDataPool.Instance;
        switch ((AICommandDef)cmd.m_wID)
        {
            case AICommandDef.AIC_MOVE:
            case AICommandDef.AIC_MOVE_ALONG:
                {
                    // 是否有限制移动的状态
                    int nLimitMoveBuffID = MacroDefine.INVALID_ID;
                    _BUFF_IMPACT_INFO pBuffImpactInfo = pDataPool.BuffImpact_GetByID( nLimitMoveBuffID );
                    if ( pBuffImpactInfo != null )
                    {
                        return true;
                    }
                }
                return false;
            case AICommandDef.AIC_USE_SKILL:
                // 是否有限制施法的状态
                return false;
            case AICommandDef.AIC_TRIPPER_ACTIVE:
            case AICommandDef.AIC_DEFAULT_EVENT:
            case AICommandDef.AIC_COMPOSE_ITEM:
            //case AIC_DIE:
            case AICommandDef.AIC_JUMP:
            case AICommandDef.AIC_FOLLOW:
            case AICommandDef.AIC_GUAJI:
            case AICommandDef.AIC_CMD_AFTERMOVE:
                return false;
            default:
                return true;
        }
        return false;
    }

    protected RC_RESULT Enter_Idle()
    {
        CObject_Character Character = GetCharacter();

        SCommand_Object cmd = new SCommand_Object();

        cmd.m_wID = (int)OBJECTCOMMANDDEF.OC_IDLE;
        cmd.SetValue<float>(0,Character.GetPosition().x);
        cmd.SetValue<float>(1,Character.GetPosition().z);

        Character.DoSimulantCommand(cmd);
        //RC_RESULT rcResult = pCharacter.DoCommand(&cmd);
        //if ( rcResult != RC_OK )
        //	return RC_ERROR;

        if (ENUM_MYSELF_AI.MYSELF_AI_IDLE != GetMySelfAI())
        {
            SetMySelfAI(ENUM_MYSELF_AI.MYSELF_AI_IDLE);
        }
        return RC_RESULT.RC_OK;
    }

    private uint moveSaveTime = 0;
    public RC_RESULT Enter_Move(float fDestX, float fDestZ, bool bAutoMove)
    {
	        uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
	        uint uSleepTime = 500;
	        //if(CGameProcedure::s_pProcMain.IsAutoRun())
	        //{
	        //	uSleepTime = 500;
	        //}
	        if(moveSaveTime + uSleepTime > uTimeNow)
	        {
		        return RC_RESULT.RC_WAIT;
	        }

	        //if ( GetMySelfAI() == MYSELF_AI_DEAD )
	        //	return RC_ERROR;

        // 	if(TRUE == CObjectManager::GetMe().GetMySelf().GetCharacterData().Get_IsInStall())
        // 	{
        // 		CEventSystem::GetMe().PushEvent(GE_INFO_SELF,"摆摊时不能移动");
        // 		return RC_ERROR;
        // 	}
        // 	
        // 	CObject_Character *pCharacter = GetCharacter();
        // 	CCharacterData* pCharData = pCharacter.GetCharacterData();
        // 	if ( pCharData.IsLimitMove() )
        // 	{
        // 		return RC_ERROR;
        // 	}
        // 
        // 	if ( pCharData.IsDie() )
        // 		return RC_ERROR;

	        //  [8/6/2010 Sun]
	        if(IsCanMove() == false)
		        return RC_RESULT.RC_ERROR;

          
	        RC_RESULT rcResult = AI_MoveTo( fDestX, fDestZ, bAutoMove ,false,0);
	        if ( rcResult != RC_RESULT.RC_OK )
	        {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "无法到达目的地");
		        return rcResult;
	        }
           
	        m_paramAI_Move.m_posTarget.m_fX	= fDestX;
	        m_paramAI_Move.m_posTarget.m_fZ	= fDestZ;
	        if ( ENUM_MYSELF_AI.MYSELF_AI_MOVE != GetMySelfAI() )
	        {
		        SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_MOVE );
	        }

	        moveSaveTime = uTimeNow;
            
	        return RC_RESULT.RC_OK;
    }
    private uint skillSaveTime = 0;
    protected RC_RESULT Enter_UseSkill(int idSkill, int nLevel, uint guidTarget, int idTargetObj, float fTargetX, float fTargetZ, float fDir)
    {
            
	        uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
	        uint uSleepTime = 500;
	        if(skillSaveTime + uSleepTime > uTimeNow)
	        {
		        return RC_RESULT.RC_WAIT;
	        }


	        //限制摆摊的时候人物不能使用技能
            //if(true == CObjectManager::GetMe().GetMySelf().GetCharacterData().Get_IsInStall())
            //{
            //    CEventSystem::GetMe().PushEvent(GE_INFO_SELF,"摆摊时不能使用技能");
            //    return RC_RESULT.RC_ERROR;
            //}

	        CObject_Character Character = GetCharacter();
	        CCharacterData CharData = Character.GetCharacterData();
	        //if ( pCharData.IsLimitUseSkill() )
	        //	return RC_ERROR;

	        bool bNeedMove = false;
	        RC_RESULT rcResult = IsCanUseSkill( idSkill, nLevel,guidTarget, idTargetObj, fTargetX, fTargetZ, fDir,ref bNeedMove );
	        if (rcResult != RC_RESULT.RC_OK)
		        return rcResult;

            //如果该技能需要下坐骑，则先下坐骑  [3/13/2012 ZZY]
            GetOffIfNeed(idSkill, nLevel);

	        if (!bNeedMove)
	        {// 就在攻击范围内
		        SetForbidTime(USE_SKILL_FORBID_TIME);
		        SendUseSkillMessage( idSkill, nLevel, guidTarget, idTargetObj, fTargetX, fTargetZ, fDir );
	        }
	        else
	        {
		        SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_USE_SKILL );
	        }

	        skillSaveTime = uTimeNow;
	        return RC_RESULT.RC_OK;

    }
    //使用技能前是否需要下坐骑  [3/13/2012 ZZY]
    protected void GetOffIfNeed(int idSkill, int nLevel)
    {   
        CObject_PlayerMySelf    myself = (CObject_PlayerMySelf)GetCharacter();
        SCLIENT_SKILL  skill = myself.GetCharacterData().Get_Skill(idSkill * 100 + nLevel);
        //如果正在骑乘，并且该技能不允许骑乘
        if (skill != null && 
            myself.Ride && 
            skill.m_pDefine != null && 
            skill.m_pDefine.m_nDisableByFlag3 == 1)
        {
            myself.Ride = false;//下坐骑
        }
    }
    static uint uActiveTripperSaveTime = 0;
    protected RC_RESULT Enter_ActiveTripperObj(int idTripperObj)
    {
        uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
        uint uSleepTime = 500;
        if (uActiveTripperSaveTime + uSleepTime > uTimeNow)
        {
            return RC_RESULT.RC_WAIT;
        }

        //if ( GetMySelfAI() == MYSELF_AI_DEAD )
        //	return RC_ERROR;

        CObject_Character pCharacter = GetCharacter();
        CCharacterData CharData = pCharacter.GetCharacterData();
        //if ( pCharData.IsLimitHandle() )
        //	return RC_ERROR;
        if (CharData.IsDie())
            return RC_RESULT.RC_ERROR;


        if (pCharacter.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
            return RC_RESULT.RC_ERROR;

        // 判断物体是否存在
        Vector3 fvTarget;
        CObject pTarget = (CObject)(CObjectManager.Instance.FindServerObject(idTripperObj));
        if (pTarget == null)
            return RC_RESULT.RC_ERROR;

        //	if(g_theKernel.IsKindOf(pTarget.GetClass(), GETCLASS(CTripperObject_Resource)))
        // 是否是tripperobj_resource
        if (CObject.TRIPPER_OBJECT_TYPE.TOT_RESOURCE == pTarget.Tripper_GetType())
        {
            if (!((CTripperObject_Resource)(CTripperObject)pTarget).Resource_CanOperation())
                return RC_RESULT.RC_ERROR;
        }

        fvTarget = pTarget.GetPosition();

        float fOperationDistance;
        //	if ( fDistSq > MAX_OPEN_ITEMBOX_DIST_SQ )
        if (CObject.TRIPPER_OBJECT_TYPE.TOT_RESOURCE == pTarget.Tripper_GetType())
        {
            CTripperObject_Resource pResource = (CTripperObject_Resource)(CTripperObject)pTarget;
            fOperationDistance = pResource.Resource_GetOperationDistance();

            m_paramAI_ActiveTripperObj.tableId = pResource.GetTableId();
        }
        else
        {
            fOperationDistance = GAMEDEFINE.MAX_OPEN_ITEMBOX_DIST;
        }

        bool bCanDo = true;
        if (fOperationDistance > 0)
        {
            float fOperationDistanceSq = fOperationDistance * fOperationDistance;
            float fDistSq = (fvTarget - pCharacter.GetPosition()).sqrMagnitude;
            if (fOperationDistanceSq < fDistSq)
            {
                bCanDo = false;
            }
        }

        if (bCanDo)
        {
            SendActiveTripperObjMessage(idTripperObj);
            //Enter_Idle();
            //GameProcedure.s_pGfxSystem.PushDebugString("CGCharTripperObj[%d]", idTripperObj); //todo
        }
        else
        {
            //如果是钓鱼的话，就不用走到河里去了。
            //但是也需要距离合适，如果距离不够，也不能开始钓。
            if (CObject.TRIPPER_OBJECT_TYPE.TOT_RESOURCE == pTarget.Tripper_GetType() && ((CTripperObject_Resource)pTarget).Resource_IsFish())
            {
                //ADDTALKMSG("距离太远！"); //todo
                return RC_RESULT.RC_ERROR;
            }
            else
            {
                Vector3 fvTargetPos = GetTargetPos(pCharacter.GetPosition(), fvTarget, fOperationDistance);
                RC_RESULT rcResult = AI_MoveTo(fvTargetPos.x, fvTargetPos.z, false, false, 0);
                if (rcResult != RC_RESULT.RC_OK)
                    return rcResult;

                // 参数付值
                m_paramAI_ActiveTripperObj.serverId = idTripperObj;
                //GameProcedure.s_pGfxSystem.PushDebugString("CGCharTripperObj[%d]", idTripperObj); //todo
            }
        }
        SetMySelfAI(ENUM_MYSELF_AI.MYSELF_AI_ACTIVE_TRIPPEROBJ);

        uActiveTripperSaveTime = uTimeNow;
        return RC_RESULT.RC_OK;
    }
    private uint defaultEventSaveTime = 0;
    protected RC_RESULT Enter_DefaultEvent(int idTargetObj)
    {
        defaultEventSaveTime = 0;
        uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
        uint uSleepTime = 500;
        if(defaultEventSaveTime + uSleepTime > uTimeNow)
        {
            return RC_RESULT.RC_WAIT;
        }

        CObject_Character Character = GetCharacter();
        CCharacterData CharData = Character.GetCharacterData();
        if ( CharData.IsDie() )
            return RC_RESULT.RC_ERROR;

        if ( Character.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD )
            return RC_RESULT.RC_ERROR;

        CObject Target = (CObject)CObjectManager.Instance.FindServerObject(idTargetObj);
        if ( Target == null )
            return RC_RESULT.RC_ERROR;

        Vector3 fvTargetPos = Target.GetPosition();

        // 设置角色朝向NPC [9/7/2011 edit by ZL]
        float    fDir;
        Vector2 fvThis;
        Vector2 fvTarget2;
        // 玩家的位置
        CObject_PlayerMySelf MySelf = CObjectManager.Instance.getPlayerMySelf();
        fvThis.x = MySelf.GetPosition().x;
        fvThis.y = MySelf.GetPosition().z;
        // fvTargent2为NPC的位置
        fvTarget2.x = Target.GetPosition().x;
        fvTarget2.y = Target.GetPosition().z;
        fDir = GFX.GfxUtility.GetYAngle(fvThis, fvTarget2);
        MySelf.SetFaceDir(fDir);

        float fDistSq = Utility.TDU_GetDistSq(fvTargetPos, GetCharacter().GetPosition());
        if (fDistSq <= GAMEDEFINE.MAX_CLIENT_DEFAULT_EVENT_DIST_SQ)
        {
            SendDefaultEventMessage(idTargetObj);

            // 如果是任务NPC，设置其自身的方向
            if(Target is CObject_PlayerNPC)
            {
                if (((CObject_PlayerNPC)Target).IsCanFaceToPlayer())
                {// 判断该NPC是否可以转向
                    ((CObject_PlayerNPC)Target).NeedFaceToPlayer = true;
                }
            }
        }
        else
        {
            // 参数付值
            m_paramAI_DefaultEvent.m_idTargetObj	= idTargetObj;
            m_paramAI_DefaultEvent.m_posPrevTarget  = fvTargetPos;
            SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_DEFAULT_EVENT );
        }

        defaultEventSaveTime = uTimeNow;
	    return RC_RESULT.RC_OK;
    }
	    //RC_RESULT Enter_Dead( VOID );
    protected bool Enter_Follow(uint idTargetObj)
    {
        // 如果不符合跟随条件 return FALSE;
	    if ( IsCanFollow() == false )
	    {
		    return false;
	    }

	    if ( GetMySelfAI() == ENUM_MYSELF_AI.MYSELF_AI_FOLLOW
	      && m_paramAI_Follow.m_idTargetObj == idTargetObj
	      )
	    { // 不重复对一个对象进行跟随操作
		    return true;
	    }

	    SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_FOLLOW );

	    CObject Obj = (CObject)CObjectManager.Instance.getPlayerMySelf();
        m_paramAI_Follow.m_nTickCount = SAIParam_Follow.m_nMaxTickCount;
	    m_paramAI_Follow.m_idTargetObj = idTargetObj;
	    m_paramAI_Follow.m_LastPos = Obj.GetPosition();

	    if ( Tick_Follow( ) )
	    {
		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"进入跟随状态。");
		    return true;
	    }
	    else
	    {
		    return false;
	    }
    }

    protected RC_RESULT Enter_MoveAlong(float fSpeed, float fTurnSpeed)
    {
        if(!IsCanMove())
		return RC_RESULT.RC_ERROR;

	    m_paramAI_MoveAlong.m_fSpeed     = fSpeed;
	    m_paramAI_MoveAlong.m_fTurnSpeed = fTurnSpeed;

	    m_paramAI_MoveAlong.m_fvTargetPos   = CObjectManager.Instance.getPlayerMySelf().GetPosition();
	    m_paramAI_MoveAlong.m_fOldDirection = CObjectManager.Instance.getPlayerMySelf().GetFaceDir();

	    if(fTurnSpeed != 0.0f)
	    {
		    m_paramAI_MoveAlong.m_fOldDirection -= fTurnSpeed ;//> 0 ? MOVE_AROUND_ANGLE_STEP : -MOVE_AROUND_ANGLE_STEP;
	    }

	    SetMySelfAI(ENUM_MYSELF_AI.MYSELF_AI_MOVE_ALONG);

	    return RC_RESULT.RC_OK;
    }
    private uint cmdAfterMoveSaveTime  = 0;
    private uint cmdAfterMoveStartTime = 0;
    // 定义超时最大时间 [6/1/2011 ivan edit]
    const uint uMaxTime = 50000;
    protected RC_RESULT Enter_CmdAfterMove(CMD_AFTERMOVE_TYPE type, int id, Vector2 targetPos)
    {
        if (cmdAfterMoveStartTime == 0)
            cmdAfterMoveStartTime = GameProcedure.s_pTimeSystem.GetTimeNow();

        // 间隔一段时间再执行 [6/1/2011 ivan edit]
        uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
        uint uSleepTime = 500;
        if (cmdAfterMoveSaveTime + uSleepTime > uTimeNow)
        {
            return RC_RESULT.RC_WAIT;
        }

        CObject_Character Character = GetCharacter();

        // 移动时不处理这个逻辑，延后处理 [4/27/2011 ivan edit]
        if (Character.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
        {
            cmdAfterMoveSaveTime = uTimeNow;
            return RC_RESULT.RC_WAIT;
        }

        // 防止服务器延时的时候，角色在移动队列的中间进入IDLE状态再处理这个命令会找不到目标。 [6/1/2011 ivan edit]
        // 现在只有玩家在靠近目的地的时候才会执行命令
        Vector2 myPos = new Vector2(GetCharacter().GetPosition().x, GetCharacter().GetPosition().z);
        float fDistSq = (targetPos - myPos).magnitude;
        if (fDistSq > GAMEDEFINE.MAX_CLIENT_DEFAULT_EVENT_DIST_SQ)
        {
            // 超时了还没移动到目的地则撤销这个命令,否则继续等待 [6/1/2011 ivan edit]
            if (uTimeNow - cmdAfterMoveStartTime > uMaxTime)
            {
                cmdAfterMoveStartTime = 0;
                return RC_RESULT.RC_ERROR;
            }
            else
            {
                cmdAfterMoveSaveTime = uTimeNow;
                return RC_RESULT.RC_WAIT;
            }
        }

	    RC_RESULT rcResult = RC_RESULT.RC_ERROR;

        CObject Obj = null;
        switch ((CMD_AFTERMOVE_TYPE)type)
        {
            case CMD_AFTERMOVE_TYPE.CMD_AFMV_SPEAK:
                Obj = CObjectManager.Instance.FindCharacterByTableID(id);
                if (Obj == null || Obj == Character)
                {
                    rcResult = RC_RESULT.RC_ERROR;
                }
                else
                {
                    rcResult = Enter_DefaultEvent(Obj.ServerID);
                }
                break;
            case CMD_AFTERMOVE_TYPE.CMD_AFMV_Tripper_ACTIVE:
                Obj = CObjectManager.Instance.FindTripperResource(id);
                if (Obj == null)
                {
                    rcResult = RC_RESULT.RC_ERROR;
                }
                else
                {
                    m_paramAI_ActiveTripperObj.serverId = -1;
                    m_paramAI_ActiveTripperObj.tableId = ((CTripperObject_Resource)Obj).GetTableId();
                    rcResult = Enter_ActiveTripperObj(Obj.ServerID);
                }
                break;
            case CMD_AFTERMOVE_TYPE.CMD_AFMV_INVALID:
                rcResult = RC_RESULT.RC_ERROR;
                break;
            case CMD_AFTERMOVE_TYPE.CMD_AFMV_ENTER_SPECIALBUS:
                SendEnterSpecialBusMessage(id);
                rcResult = RC_RESULT.RC_OK;
                break;
            case CMD_AFTERMOVE_TYPE.CMD_AFMV_AutoHit:
                PushCommand_AutoHit(1);
                rcResult = RC_RESULT.RC_OK;
                break;
            default:
                //  Assert( FALSE && "CAI_MySelf::Enter_CmdAfterMove  unknow CMD_AFTERMOVE_TYPE" );
                rcResult = RC_RESULT.RC_ERROR;
                break;
        }

        if (rcResult == RC_RESULT.RC_ERROR || rcResult == RC_RESULT.RC_OK)
            cmdAfterMoveStartTime = 0;// 结束的时候重置起始时间 [6/1/2011 ivan edit]

        //发送取消显示寻路消息（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE, 0);
        cmdAfterMoveSaveTime = uTimeNow;
	    return rcResult;
    }

    protected bool Tick_Idle()
    {
        if (StartGuaJi !=0)
        {
            Tick_AutoHit();
        }
        return true;
    }

    protected bool Tick_Move()
    {
        CObject_Character Character = GetCharacter();

        if (Character.CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE || Character.CharacterLogic_IsStopped())
	    {
		    //OnAIStopped( );
            SetMySelfAI(ENUM_MYSELF_AI.MYSELF_AI_IDLE);
	    }

	    return true;
    }

    protected bool Tick_UseSkill()
    {
	    _DBC_SKILL_DATA skillData = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).Search_Index_EQU((int)m_paramAI_UseSkill.m_idSkill);
        if ( skillData == null )
		    return false;

	    float fMaxSkillRange		= skillData.m_fMaxAttackRange;
	    float fMoveRange			= fMaxSkillRange;
	    float fMoveRangeForecast	= 1.0f;

	    fMoveRange -= fMoveRangeForecast;
	    if ( fMoveRange < 0.0f )
		    fMoveRange = 0.0f;

	    CObject_Character Character = GetCharacter();

	    // 对单体
	    Vector3 fvTarget;
	    if ( skillData.m_fDamageRange <= 0.0f )
	    {
		   
            CObject_Character TargetObj = (CObject_Character)CObjectManager.Instance.FindServerObject(m_paramAI_UseSkill.m_idTarget);
		    if ( TargetObj == null ||
			    TargetObj.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
		    {
			    OnAIStopped( );
			    return false;
		    }
		    fvTarget = TargetObj.GetPosition();
	    }
	    else
	    {
		    fvTarget = m_paramAI_UseSkill.m_posTarget;
	    }

	    float fDist = (fvTarget - m_paramAI_UseSkill.m_posPrevTarget).magnitude;
	    if ( fDist > fMoveRange+1.0f )
	    {
            // 设置移动到的目标位置
		    Vector3 fvTargetPos = GetTargetPos(Character.GetPosition(), fvTarget, fMoveRange);
		    m_paramAI_UseSkill.m_posPrevTarget = fvTargetPos;

		    RC_RESULT rcResult = AI_MoveTo( fvTargetPos.x, fvTargetPos.z,false,false,0);
            if (rcResult == RC_RESULT.RC_ERROR)
		    {
			    OnAIStopped();
			    return false;
		    }
	    }
        else if (Character.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE)
	    {
		    float fMTDist = (Character.GetPosition() - fvTarget).magnitude;
		    if (fMTDist > fMaxSkillRange)
		    {
			    // 设置移动到的目标位置
                Vector3 fvTargetPos = GetTargetPos(Character.GetPosition(), fvTarget, fMoveRange);
			    m_paramAI_UseSkill.m_posPrevTarget = fvTargetPos;

			    RC_RESULT rcResult = AI_MoveTo( fvTargetPos.x, fvTargetPos.z,false,false,0);
                if (rcResult == RC_RESULT.RC_ERROR)
			    {
				    OnAIStopped( );
				    return false;
			    }
		    }
		    else
		    {
			    SetForbidTime(USE_SKILL_FORBID_TIME);
			    SendUseSkillMessage( m_paramAI_UseSkill.m_idSkill,
				    m_paramAI_UseSkill.m_SkillLevel,
				    m_paramAI_UseSkill.m_guidTarget,
				    m_paramAI_UseSkill.m_idTarget,
				    m_paramAI_UseSkill.m_posTarget.x,
				    m_paramAI_UseSkill.m_posTarget.z,
				    m_paramAI_UseSkill.m_fDir );
			    //只有不是连续攻击的技能才转入IDLE状态
			    //if (pSkillData)
			    //{
			    //	if (FALSE == pSkillData.m_bAutoRedo)
			    //	{
			    if ( ENUM_MYSELF_AI.MYSELF_AI_IDLE != GetMySelfAI() )
			    {
				    SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_IDLE );
			    }
			    //	}
			    //}
		    }
	    }

	    return true;
    }
	
	public uint AutoFight
	{
		get {return this.StartGuaJi;}
	}
	
    protected bool Tick_ActiveTripperObj()
    {
        // 正在采集，跳过 [4/18/2012 Ivan]
        if (IsTripperActive())
            return true;

        CObject_Character pCharacter = GetCharacter();

        if (pCharacter.CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_MOVE)
        {
            CObject pTarget = null;
            if (m_paramAI_ActiveTripperObj.serverId != -1)
            {
                pTarget = (CObject)CObjectManager.Instance.FindServerObject(m_paramAI_ActiveTripperObj.serverId);
            }
            else
            {
                pTarget = (CObject)CObjectManager.Instance.FindTripperResource(m_paramAI_ActiveTripperObj.tableId);
                m_paramAI_ActiveTripperObj.serverId = pTarget.ServerID;
            }
            if (pTarget == null)
            {
                OnAIStopped();
                return false;
            }

            Vector2 tripperPos = new Vector2(pTarget.GetPosition().x, pTarget.GetPosition().z);
            Vector2  charPos    = new Vector2(GetCharacter().GetPosition().x, GetCharacter().GetPosition().z);
            float fDistSq = (tripperPos - charPos).sqrMagnitude;

            // 得到采集物的距离 [6/15/2011 ivan edit]
            float fOperationDistance;
            if(	CObject.TRIPPER_OBJECT_TYPE.TOT_RESOURCE == pTarget.Tripper_GetType() )
            {
                CTripperObject_Resource pResource = (CTripperObject_Resource)(CTripperObject)pTarget;
                fOperationDistance = pResource.Resource_GetOperationDistance();
            }
            else
            {
                fOperationDistance = GAMEDEFINE.MAX_OPEN_ITEMBOX_DIST;
            }
            float fOperationDistanceSq = fOperationDistance * fOperationDistance;

            if (fDistSq <= fOperationDistanceSq)
            {
                SendActiveTripperObjMessage(pTarget.ServerID);
                m_paramAI_ActiveTripperObj.serverId = -1;
            }
            else
            {
                Vector3 fvTargetPos = GetTargetPos(pCharacter.GetPosition(), pTarget.GetPosition(), fOperationDistance);
                RC_RESULT rcResult = AI_MoveTo(fvTargetPos.x, fvTargetPos.z, false, false, 0);
                if (rcResult != RC_RESULT.RC_OK)
                {
                    OnAIStopped();
                    return false;
                }
            }

        }

	    return true;
    }

    protected bool Tick_DefaultEvent()
    {
        CObject_Character Character = GetCharacter();

        CObject Target = (CObject)CObjectManager.Instance.FindServerObject(m_paramAI_DefaultEvent.m_idTargetObj);
        if ( Target == null )
        {
            OnAIStopped( );
            return false;
        }
    	
        Vector3 fvTarget;
        fvTarget = Target.GetPosition();

        float fDistSq = Utility.TDU_GetDistSq( fvTarget , GetCharacter().GetPosition());
        if (fDistSq <= GAMEDEFINE.MAX_CLIENT_DEFAULT_EVENT_DIST_SQ)
        {
            SendDefaultEventMessage( m_paramAI_DefaultEvent.m_idTargetObj );

            // 如果是任务NPC，设置其自身的方向
            if(Target is CObject_PlayerNPC)
            {
                if (((CObject_PlayerNPC)Target).IsCanFaceToPlayer())
                {// 判断该NPC是否可以转向
                    ((CObject_PlayerNPC)Target).NeedFaceToPlayer = true;
                }
            }
            OnAIStopped( );
            return true;
        }

        float fPrevTargetToTargetSq = Utility.TDU_GetDistSq( fvTarget, m_paramAI_DefaultEvent.m_posPrevTarget );
        if ( Character.CharacterLogic_Get() ==  ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE
            || fPrevTargetToTargetSq > GAMEDEFINE.MAX_CLIENT_DEFAULT_EVENT_DIST_SQ + 1)
        {
            Vector3 fvTargetPos = GetTargetPos(Character.GetPosition(), fvTarget, GAMEDEFINE.MAX_CLIENT_DEFAULT_EVENT_DIST);
            RC_RESULT rcResult = AI_MoveTo(fvTargetPos.x, fvTargetPos.z, false, false, 0);
            if ( rcResult != RC_RESULT.RC_OK )
            {
                OnAIStopped( );
                return false;
            }
            m_paramAI_DefaultEvent.m_posPrevTarget = fvTargetPos;
        }
	    return true;
    }

	    //BOOL Tick_Dead( VOID );
    protected bool Tick_Follow()
    {
        return true;
    }

    protected bool Tick_AutoHit()
    {
        if (CObjectManager.Instance.getPlayerMySelf().CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
	    {
		    StartGuaJi = 0;
	    }
	    CObjectManager.Instance.LockNearestTargetToHit();
	    return true;
    }
    private float fstepMoveTime = 0.0f;
    protected bool Tick_MoveAlong()
    {
        
        float fCurDir = CObjectManager.Instance.getPlayerMySelf().GetFaceDir();
	    float fTargetDir = fCurDir;

	    // 直线行走时，本次移动与目标点小于一定距离时，才开始新的一次寻路 [8/20/2010 Sun]
	    // 提高效率
	    float fDistance = 0.0f;
	    float fMoveDistance = 0.0f;
	    //if(abs(fCurDir - m_paramAI_MoveAlong.m_fOldDirection) < 0.01)
	    if(Mathf.Abs(m_paramAI_MoveAlong.m_fTurnSpeed) < 0.0001f)
	    {
		    fDistance = 4.0f;
		    fMoveDistance = m_paramAI_MoveAlong.m_fSpeed;

	 	    Vector3 fCurPos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
	 	    fVector2 fv;
	 	    fv.x = m_paramAI_MoveAlong.m_fvTargetPos.x - fCurPos.x;
	 	    fv.y = m_paramAI_MoveAlong.m_fvTargetPos.z - fCurPos.z;
	 	    if (fv.x * fv.x + fv.y * fv.y > fDistance)
	 	    {
	 		    return true;
	 	    }

	    }
	    else 
	    {
		    fDistance = 1.00f;
		    fMoveDistance = m_paramAI_MoveAlong.m_fSpeed/8.0f;
		    float fDetailTime = GameProcedure.s_pTimeSystem.GetDeltaTime();
		    //fStepTime += fDetailTime;
		    fTargetDir += m_paramAI_MoveAlong.m_fTurnSpeed * fstepMoveTime;
	    //	fTargetDir += m_paramAI_MoveAlong.m_fTurnSpeed > 0 ? MOVE_AROUND_ANGLE_STEP : -MOVE_AROUND_ANGLE_STEP;

		    if (Mathf.Abs(fTargetDir - m_paramAI_MoveAlong.m_fOldDirection ) < MOVE_AROUND_ANGLE_STEP * 2)
		    {
			    fstepMoveTime += fDetailTime;
			    return true;
		    }
		    fstepMoveTime = 0.0f;
	    }
    	
    	
	    Vector2 fvTarget;
	    fvTarget.x = Mathf.Sin(fTargetDir) * fMoveDistance;
	    fvTarget.y = Mathf.Cos(fTargetDir) * fMoveDistance;

	    Vector2 fvEnd;
        fvEnd.x = CObjectManager.Instance.getPlayerMySelf().GetPosition().x + fvTarget.x;
        fvEnd.y = CObjectManager.Instance.getPlayerMySelf().GetPosition().z + fvTarget.y;
    	
	    m_paramAI_MoveAlong.m_fvTargetPos.x = fvEnd.x;
	    m_paramAI_MoveAlong.m_fvTargetPos.z = fvEnd.y;
	    m_paramAI_MoveAlong.m_fOldDirection = fTargetDir;


	    RC_RESULT Result = AI_MoveTo(fvEnd.x, fvEnd.y,false,false,0);
	    if(Result != RC_RESULT.RC_OK)
		    return false;
	    return true;
    }

    protected override void OnAIStopped()
    {
        switch ((ENUM_MYSELF_AI)GetMySelfAI())
        {
            case ENUM_MYSELF_AI.MYSELF_AI_IDLE:
            case ENUM_MYSELF_AI.MYSELF_AI_MOVE:
            case ENUM_MYSELF_AI.MYSELF_AI_USE_SKILL:
            case ENUM_MYSELF_AI.MYSELF_AI_ACTIVE_TRIPPEROBJ:
            case ENUM_MYSELF_AI.MYSELF_AI_DEFAULT_EVENT:
            case ENUM_MYSELF_AI.MYSELF_AI_GUAJI:
            case ENUM_MYSELF_AI.MYSELF_AI_MOVE_ALONG:
                break;
            case ENUM_MYSELF_AI.MYSELF_AI_FOLLOW:
                {
                    StopFollow();
                }
                break;
            default:
                //Assert( FALSE && "CAI_MySelf::OnAIStopped  unknow GetMySelfAI" );
                break;
        }

        Enter_Idle();
    }

    protected void StopFollow()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "退出跟随状态");
    }

    protected override void OnAIStopMove()
    {
        Vector3 fCurPos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
	    AI_MoveTo(fCurPos.x, fCurPos.y,false,false,0);
	    OnAIStopped();  
    }

    protected Vector3 GetTargetPos(Vector3 MePos, Vector3 TargetPos, float fDist)
    {
        Vector3 fvTarget = Vector3.zero;
        float fMTDist = Utility.TDU_GetDist(MePos, TargetPos);
        float fGoDist = (fMTDist - fDist + 0.5f) > 0 ? (fMTDist - fDist + 0.5f) : 0;

        fvTarget.x = MePos.x + fGoDist * (TargetPos.x - MePos.x) / fMTDist;
        fvTarget.z = MePos.z + fGoDist * (TargetPos.z - MePos.z) / fMTDist;
        fvTarget.y = MePos.y + fGoDist * (TargetPos.y - MePos.y) / fMTDist;

        //// 防止出现距离回退后出现在不可行走区域里面 [5/9/2011 ivan edit]
        //if(CPath::IsPointInUnreachRegion(fVector2(fvTarget.x, fvTarget.z)))
        //{
        //    // 为了加快速度，取短一点的距离 [5/9/2011 ivan edit]
        //    FLOAT nearDist = fDist / 3 >= 1? fDist / 3 : 1.0f;

        //    for (int i =1; i <= 9; i++)
        //    {
        //        // 重置位置，从原点开始计算 [5/9/2011 ivan edit]
        //        fvTarget = TargetPos;
        //        // 计算九宫格偏移 [5/9/2011 ivan edit]
        //        // 计算X轴偏移 [5/9/2011 ivan edit]
        //        if (i%3 == 1)
        //            fvTarget.x -= nearDist;
        //        else if (i%3 == 0)
        //            fvTarget.x += nearDist;

        //        // 计算Y轴偏移 [5/9/2011 ivan edit]
        //        if (i <= 3)
        //            fvTarget.z -= nearDist;
        //        else if (i >= 7)
        //            fvTarget.z += nearDist;

        //        // 找到一个可行走的坐标就返回 [5/9/2011 ivan edit]
        //        if(!CPath::IsPointInUnreachRegion(fVector2(fvTarget.x, fvTarget.z)))
        //            return fvTarget;
        //    }

        //    // 如果遍历周围都找不到可走的点则返回npc的坐标 [5/9/2011 ivan edit]
        //    return TargetPos;
        //}

        return fvTarget;
    }

    protected void			        SetMySelfAI(ENUM_MYSELF_AI eAI)	{ m_eMySelfAI = eAI; }

	    //向服务器发送命令 暂时不用
   // protected void			SendMoveMessage( CPath path, int nLogicCount );
    protected void SendUseSkillMessage(int idSkill, int nLevel, uint guidTarget, int idTargetObj, float fTargetX, float fTargetZ, float fDir)
    {
         CObject_Character Character = GetCharacter();

	    // 不要打开下面的代码，打开会出现连招攻击的技能第一招发不出来
	    //// temp code {
	    //pCharacter.m_paramLogic_SkillSend.m_fCurTime = 0;
	    //pCharacter.CharacterLogic_Set(CObject_Character::CHARACTER_LOGIC_SKILL_SEND);
	    //// }

	    WORLD_POS posCurrent, posTarget;
	    posCurrent.m_fX	= Character.GetPosition().x;
	    posCurrent.m_fZ	= Character.GetPosition().z;
	    posTarget.m_fX	= fTargetX;
	    posTarget.m_fZ	= fTargetZ;

	    // 发往服务器
        LogManager.LogWarning("SendUseSkillMessage " + idSkill);
        CGCharUseSkill msg = (CGCharUseSkill) NetManager.GetNetManager().CreatePacket((int) PACKET_DEFINE.PACKET_CG_CHARUSESKILL);
        msg.ObjectID    = Character.ServerID;
	    msg.SkillDataID = idSkill*100+nLevel;
	    msg.GuidTarget  = (int)guidTarget;
        msg.TargetID    = idTargetObj;
        msg.PosTarget   = posTarget;
        msg.Dir         = fDir;

	    NetManager.GetNetManager().SendPacket(msg);
        CActionSystem.Instance.UpdateCommonCoolDown(idSkill);

	   // AxTrace(0, 0, "%s...%d", "Send Skill", idSkill);
        //CGameProcedure::s_pGfxSystem.PushDebugString("CGCharUseSkill[%d].[%d]", 
        //    idSkill, idTargetObj);
    }


    bool startTripper = false;

    protected void SendActiveTripperObjMessage(int idTripperObj)
    {
        CTripperObject TripperObj = (CTripperObject)CObjectManager.Instance.FindServerObject(idTripperObj);
        if (TripperObj == null) return;

        TripperObj.Tripper_Active();

        startTripper = true;
        LogManager.LogWarning("start tripper");
    }

    public bool IsTripperActive()
    {
        return startTripper;
    }

    public void FinishTripperActive()
    {
        startTripper = false;
        LogManager.LogWarning("finish tripper");
    }

    protected void SendComposeItemMessage(int idAbility, int idPrescription, uint guidPlatform, uint uFlag)
    {
        CObject_Character Character = GetCharacter();

        // 发往服务器
        CGUseAbility msg = (CGUseAbility)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_USEABILITY);
        msg.SetAbilityID( (short)idAbility );
        msg.SetPrescriptionID(idPrescription);
        msg.SetPlatformGUID( guidPlatform );
        msg.SetSpecialFlag( uFlag);
        NetManager.GetNetManager().SendPacket(msg);

        //CGameProcedure::s_pGfxSystem.PushDebugString("CGCharUseAbility[%d],[%d],[%d]", 
        //    idAbility, idPrescription,	guidPlatform);
    }

    protected void SendDefaultEventMessage(int idTargetObj)
    {
        LogManager.LogWarning("SendDefaultEventMessage " + idTargetObj);
        CGCharDefaultEvent msg = new CGCharDefaultEvent();
        msg.ObjId = (uint)idTargetObj;
        NetManager.GetNetManager().SendPacket(msg);

        // 设置当前对话npc [5/16/2012 Ivan]
        CUIDataPool.Instance.SetCurDialogNpcId(idTargetObj);
    }
	    // 进入载具 [8/26/2011 ivan edit]
    protected void SendEnterSpecialBusMessage(int idBus)
    {

    }

	    //判断现在是否可以使用技能
    protected RC_RESULT IsCanUseSkill(int idSkill, int idLevel, uint guidTarget, int idTargetObj, float fTargetX, float fTargetZ, float fDir,ref bool bNeedMove)
    {
         if ( GetMySelfAI( ) == ENUM_MYSELF_AI.MYSELF_AI_USE_SKILL
		&& m_paramAI_UseSkill.m_idSkill == idSkill
		&& m_paramAI_UseSkill.m_idTarget == idTargetObj
		&& Mathf.Abs( m_paramAI_UseSkill.m_posTarget.x - fTargetX ) < 0.1f
		&& Mathf.Abs( m_paramAI_UseSkill.m_posTarget.z - fTargetZ ) < 0.1f
		&& Mathf.Abs( m_paramAI_UseSkill.m_fDir - fDir ) < 0.1f )
	    {
		    return RC_RESULT.RC_ERROR;
	    }

	    CObject_Character Character = GetCharacter();
	    CCharacterData CharData     = Character.GetCharacterData();
	    if ( CharData.IsDie() )
		    return RC_RESULT.RC_ERROR;

	    if ( Character.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD )
		    return RC_RESULT.RC_ERROR;

	    _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)idSkill);
        if ( skillData == null )
		    return RC_RESULT.RC_ERROR;
		    

	    //////////////////////////////////////////////////////////////////////////////////////////
	    SCLIENT_SKILL skill = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Skill( idSkill * 100 + idLevel );
	    if ( skill == null)
		    return RC_RESULT.RC_ERROR;

	    CObject_PlayerMySelf MySelf = CObjectManager.Instance.getPlayerMySelf();
        
	    int idUser = (MySelf!= null)?(MySelf.ID):(MacroDefine.INVALID_ID);

	    OPERATE_RESULT oResult = skill.IsCanUse(idUser, idLevel, (int)idTargetObj, fTargetX, fTargetZ, fDir );
        if (oResult != OPERATE_RESULT.OR_OK)
	    {
		    //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "退出跟随状态");
           // GameProcedure::s_pEventSystem.PushEvent( GE_INFO_SELF, GetOResultText( oResult ) );
		    return RC_RESULT.RC_ERROR;
	    }

	    // 不需要Character目标的技能则直接返回成功
	    if (skill.m_pDefine.m_nSelectType != (int)ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER)
		    return RC_RESULT.RC_OK;

	    Vector3 fvTarget;
	    float fMaxSkillRange, fMaxSkillRangeSq, fDistToTargetSq;
	    CObject Target = (CObject)(CObjectManager.Instance.FindServerObject( idTargetObj ));
	    if ( Target == null )
		    return RC_RESULT.RC_ERROR;

	    fvTarget = Target.GetPosition();

	    fMaxSkillRange      = skillData.m_fMaxAttackRange;
	    fMaxSkillRangeSq    = fMaxSkillRange * fMaxSkillRange;
        fDistToTargetSq     = Utility.TDU_GetDistSq(fvTarget, Character.GetPosition());
	    if ( fMaxSkillRangeSq > fDistToTargetSq )
	    {
		    bNeedMove = false;
	    }
	    else
	    {
            // 设置移动到的目标位置
		    Vector3 fvTargetPos = GetTargetPos(Character.GetPosition(), fvTarget, fMaxSkillRange);

		    RC_RESULT rcResult = AI_MoveTo( fvTargetPos.x, fvTargetPos.z ,false,false,0);
		    if ( rcResult != RC_RESULT.RC_OK )
		    {
			    return rcResult;
		    }

		    // 参数付值
		    m_paramAI_UseSkill.m_idSkill				= (short)idSkill;
		    m_paramAI_UseSkill.m_SkillLevel				= (byte)idLevel;
		    m_paramAI_UseSkill.m_guidTarget				= guidTarget;
		    m_paramAI_UseSkill.m_idTarget				= idTargetObj;
		    m_paramAI_UseSkill.m_posTarget				= fvTarget;
		    m_paramAI_UseSkill.m_fDir					= fDir;
		    m_paramAI_UseSkill.m_posPrevTarget			= fvTargetPos;

		    SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_USE_SKILL );

		    bNeedMove = true;
	    }

	    return RC_RESULT.RC_OK;

    }

    protected bool IsCanFollow()
    {
//        // 判断玩家当前的状态
//	    if( true == CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_IsInStall() )
//	    {
//		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "摆摊时不能移动。");
//		    return false;
//	    }

	    // 判断当前的 AI
	    switch( GetMySelfAI() )
	    {
	        case ENUM_MYSELF_AI.MYSELF_AI_MOVE:
	        case ENUM_MYSELF_AI.MYSELF_AI_IDLE:
	        case ENUM_MYSELF_AI.MYSELF_AI_DEFAULT_EVENT:
	        case ENUM_MYSELF_AI.MYSELF_AI_FOLLOW:
		        return true;
	        case ENUM_MYSELF_AI.MYSELF_AI_ACTIVE_TRIPPEROBJ:
	        case ENUM_MYSELF_AI.MYSELF_AI_USE_SKILL:
	        case ENUM_MYSELF_AI.MYSELF_AI_GUAJI:
	        //case MYSELF_AI_DEAD:
		        {
			        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "无法进入跟随状态。!");
		        }
		        return false;
	        default:
		        return false;
	    }

	    return true;
    }

    protected bool IsCanMove()
    {
//        if(true == CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_IsInStall())
//	    {
//		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "摆摊时不能移动。");
//		    return false;
//	    }

	    CObject_Character Character = GetCharacter();
	    CCharacterData CharData     = Character.GetCharacterData();
	    if ( CharData.IsLimitMove() )
	    {
		    return false;
	    }

	    if ( CharData.IsDie() )
		    return false;

	    // 人在载具上的时候不能使用移动 [8/16/2011 ivan edit]
//	    if (CharData.IsInBusAndLimitMove())
//	    {
//		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "摆摊时不能移动。");
//		    return false;
//	    }

	    return true;
    }

    private	ENUM_MYSELF_AI	m_eMySelfAI;

    protected struct SAIParam_Move
	{
        public WORLD_POS m_posTarget;
	};

	protected struct SAIParam_UseSkill
	{
		public short	m_idSkill;
        public byte     m_SkillLevel;
        public uint     m_guidTarget;
        public int      m_idTarget;
        public Vector3  m_posTarget;
        public float    m_fDir;
        public Vector3  m_posPrevTarget;
	};

	protected struct SAIParam_ActiveTripperObj
	{
        public int serverId;
        public int tableId;
        // 如果设置为true，则一直执行采集动作 [4/18/2012 Ivan]
        //public bool keepDoing;
	};

	protected struct SAIParam_DefaultEvent
	{
        public int      m_idTargetObj;
        public Vector3  m_posPrevTarget;
	};

	protected struct SAIParam_Follow
	{
		public const int    m_nMaxTickCount = 10;
        public int          m_nTickCount;
        public uint         m_idTargetObj;
        public Vector3      m_LastPos;
	};
	
	protected struct SAIParam_MoveAlong
	{
        public float    m_fSpeed;
        public float    m_fTurnSpeed;
        public Vector3  m_fvTargetPos;
        public float    m_fOldDirection;
	};

	protected SAIParam_Move				    m_paramAI_Move;
	protected SAIParam_UseSkill			    m_paramAI_UseSkill;
	protected SAIParam_ActiveTripperObj	    m_paramAI_ActiveTripperObj;
	protected SAIParam_DefaultEvent		    m_paramAI_DefaultEvent;
	protected SAIParam_Follow				m_paramAI_Follow;
	protected SAIParam_MoveAlong			m_paramAI_MoveAlong;

	List<Vector2>		m_fv2CirclePath;
    const int  USE_SKILL_FORBID_TIME	= 500;

    public CAI_MySelf(CObject_Character CharObj):base(CharObj)
    {
        SetMySelfAI( ENUM_MYSELF_AI.MYSELF_AI_IDLE );
        m_SaveCommand = new SCommand_AI();
	    m_SaveCommand.CleanUp();
        m_fv2CirclePath = new List<Vector2>();
	    m_uForbidTime		= 0;
	    m_uForbidStartTime	= 0;
	    StartGuaJi = 0;
		AutoReleaseSkill autoSkill = AutoReleaseSkill.Instance;
    }
};