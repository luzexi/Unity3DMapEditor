/****************************************\
*										*
* 			   游戏主流程				*
*										*
\****************************************/
using System;
using UnityEngine;
using Network.Packets;

public class GamePro_Main : GameProcedure
{

    public void SetSceneID(int nSceneID) { m_SceneID = nSceneID; }

    //设置目的地指示环
    public void			SetMouseTargetProjTex( float x, float z)
    {
        if(m_pMouseTarget == null)
	    {
		    //创建
		    m_pMouseTarget = (CObject_ProjTex_MouseTarget)CObjectManager.Instance.NewProjTexMouseTarget(-1);
		    m_pMouseTarget.Initial(null);
	    }
	    //判断目标是否不可走
	    bool bUnReachAble = CPath.IsPointInUnreachRegion(x, z);
	    m_pMouseTarget.SetReachAble(!bUnReachAble);
	    if(bUnReachAble)
	    {
// 		    STRING strWarn = COLORMSGFUNC("UNREACHABLE");
// 		    ADDTALKMSG(strWarn);
	    }
	    //设置位置
	    m_pMouseTarget.SetMapPosition(x, z);
    }

    //设置鼠标点击粒子特效
    //public void			SetMouseClickedEffect(const fVector2& fvClickedPos);

    //鼠标操作技能
	public tActionItem ActiveSkill
	{
		get{return m_pActiveSkill;}
		set{m_pActiveSkill = value;}
	}


    //public CObject_Effect_MouseTarget* GetMouseTarget(void) { return m_pMouseClickedEffect; }


    public bool CanUseKeyBoardMove() { return m_bUseKeyboardMove; }

    public bool IsKeepMoving() { return m_bKeepMoving; }
    public bool IsKeepturning() { return m_bKeepturning; }

    public void StartMoving(bool bBack) { }
    public void StopMoving() { }

    public void StartTurning(bool bLeft) { }
    public void StopTurning() { }

    //设置鼠标
    //public void			OnSetCursor(ENUM_CURSOR_TYPE nType);

    //  [6/28/2011 ivan edit]
    public int GetLoginState() { return loginState; }
    public void SetLoginState(int nState) { loginState = nState; }

    public bool IsAutoRun() { return m_bAutoRun; }
    public void SetAutoRun(bool bStart) { }

    //////////////////////////////////////////////////////////////////////////
    //protected:
    //当前进入的场景id
    int m_SceneID;
    //玩家自己
    CObject_PlayerMySelf m_pMySelf;
    //鼠标拖拽摄像机中
    //bool					m_bCameraDraging;

    //拖动行走中
    bool m_bAutoRun;

    //
    //bool					m_bAutoMoveAlong;

    //开启键盘操作
    bool m_bUseKeyboardMove;

    //保持移动
    bool m_bKeepMoving;
    //保持旋转
    bool m_bKeepturning;

    //bool m_bAutoHit = false;

    //当前旋转速度
    //float					m_fTurnSpeed;
    //float					m_fKeepMoveSpeed;

    // 判断是否已经登录过 [6/29/2011 ivan edit]
    // 0 不需要处理
    // 1 返回登录窗口
    // 2 返回角色选择窗口
    int loginState;

    public override void Init()
    {
        base.Init();
        m_pMySelf = s_pObjectManager.getPlayerMySelf();
        int m_nID = m_pMySelf.ServerID;
        // 	    m_pActiveSkill = NULL;
        // 	    m_pSkillArea = NULL;
        // 	    m_pMouseTarget = NULL;
        // 	    m_bAutoRun = FALSE;
        // 	    m_bAutoMoveAlong = FALSE;
        //------------------------------------------------------------------
        //重置LogicCount
        //	    m_pMySelf->ResetLogicCount();

        //------------------------------------------------------------------
        //清空加载队列
        //	    CObjectManager::GetMe()->GetLoadQueue()->ClearAllTask();

        //------------------------------------------------------------------
        //发送环境请求
        //	    CGEnvRequest msgEnvRequest;
        //	    CNetManager::GetMe()->SendPacket(&msgEnvRequest);

        //------------------------------------------------------------------
        //发送自身数据请求

        //自身基本信息
        CGCharAskBaseAttrib msgMyBaseAttrib = new CGCharAskBaseAttrib();
        msgMyBaseAttrib.setTargetID((uint)m_nID);
        s_NetManager.SendPacket(msgMyBaseAttrib);

        //基本属性
        CGAskDetailAttrib msgMyAttrib = new CGAskDetailAttrib();
        msgMyAttrib.ObjID = (uint)m_nID;
        s_NetManager.SendPacket(msgMyAttrib);

        ////自身装备(用于创建渲染层外形)
        //CGCharAskEquipment msgAskMyEquip;
        //msgAskMyEquip.setObjID(m_nID);
        //CNetManager::GetMe()->SendPacket(&msgAskMyEquip);

        //生活技能
        CGAskDetailAbilityInfo msgAbility = new CGAskDetailAbilityInfo();
        msgAbility.ObjID = m_nID;
        NetManager.GetNetManager().SendPacket(msgAbility);
        // 
        // 	    //技能系
        CGAskSkillClass msgMySkillClass = new CGAskSkillClass();
        msgMySkillClass.ObjID = m_nID;
        s_NetManager.SendPacket(msgMySkillClass);

        //技能
        CGAskDetailSkillList msgMyDetailSkill = new CGAskDetailSkillList();
        msgMyDetailSkill.ObjID = m_nID;
        s_NetManager.SendPacket(msgMyDetailSkill);
        // 
        // 	    //组队数据
        // 	    CUIDataPool::GetMe()->ClearTeamInfo();
        // 	    CGAskTeamInfo msgTeamInfo;
        // 	    msgTeamInfo.SetObjID( m_nID );
        // 	    CNetManager::GetMe()->SendPacket(&msgTeamInfo);
        // 
        // 	    //如果是城市向服务器请求城市数据
        // 	    CGCityAskAttr	MsgCityBuilding;
        // 	    CNetManager::GetMe()->SendPacket( &MsgCityBuilding );
        // 
        // 	    //所有称号
        // 	    CGCharAllTitles	msgAllTitles;
        // 	    msgAllTitles.setTargetID( m_nID );
        // 	    CNetManager::GetMe()->SendPacket(&msgAllTitles);

        //是第一次登陆
        if (GameProcedure.s_ProcEnter.GetEnterType() == (uint)ENTER_TYPE.ENTER_TYPE_FIRST)
        {
            //详细装备(用于在数据池中存储物品详细属性)
            CGAskDetailEquipList msgAskEquip = new CGAskDetailEquipList();
            msgAskEquip.ObjId = (uint)m_nID;
            msgAskEquip.Mode = ASK_EQUIP_MODE.ASK_EQUIP_ALL;
            s_NetManager.SendPacket(msgAskEquip);

            //背包
            CGAskMyBagList msgMyBag = new CGAskMyBagList();
            msgMyBag.Mode = ASK_BAG_MODE.ASK_ALL;
            s_NetManager.SendPacket(msgMyBag);

            CGAskMail msgAskMail = new CGAskMail();
            msgAskMail.AskType = (byte)ASK_TYPE.ASK_TYPE_LOGIN;
            s_NetManager.SendPacket(msgAskMail);

            //MissionList
            CGAskMissionList msgMissionList = new CGAskMissionList();
            msgMissionList.ObjID = (uint)m_nID;
            s_NetManager.SendPacket(msgMissionList);

            //向服务器请求快捷栏数据
		    CGAskSetting msgSetting = new CGAskSetting();
		    s_NetManager.SendPacket( msgSetting );
            //请求法宝栏数据
            CGAskTalismanBag AskTalismanBag = new CGAskTalismanBag();
            s_NetManager.SendPacket(AskTalismanBag);



            //向服务器请求关系人列表
		    CGRelation MsgRelation = new CGRelation();
		    MsgRelation.GetRelation().m_Type = (byte)RELATION_REQUEST_TYPE.REQ_RELATIONLIST;
		    NetManager.GetNetManager().SendPacket( MsgRelation );

            //想服务器请求符印信息
            CGAskFlushCharmInfo msgFlushCharmInfo = new CGAskFlushCharmInfo();
            NetManager.GetNetManager().SendPacket(msgFlushCharmInfo);

            //广播第一次进入游戏，用来初始化ui或者一些组件
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_APPLICATION_INITED);
        }

    }
    public override void Tick()
    {
        base.Tick();

        if (m_pMySelf != null)
        {
            if (GameProcedure.m_bNeedFreshMinimap) // 如果立马需要刷新小地图
            {
                if (GameProcedure.s_pTimeSystem.GetTimeNow() - currTime > 100)
                {
                    UpdateMap();
                    GameProcedure.m_bNeedFreshMinimap = GameProcedure.m_bWaitNeedFreshMinimap = false;
                }
            }
            else if (GameProcedure.m_bWaitNeedFreshMinimap)
            {
                if (GameProcedure.s_pTimeSystem.GetTimeNow() - currTime > 1000)
                {
                    UpdateMap();
                    GameProcedure.m_bWaitNeedFreshMinimap = false;
                }
            }
        }
    }

    static uint currTime = 0;
    private void UpdateMap()
    {
        currTime = GameProcedure.s_pTimeSystem.GetTimeNow();
        WorldManager.Instance.UpdateMinimapData();
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_MAP);
    }
    public override void Render() { }
    public override void Release() { }
    public override void CloseRequest() { }

    //virtual LRESULT	MainWndProc(HWND, UINT, WPARAM, LPARAM);


    //处理输入
    public override void ProcessInput()
    {
        ProcessInput_Keybord();
        ProcessInput_Mouse();
    }
    //处理鼠标输入
    public void ProcessInput_Mouse()
    {
        CInputSystem input = CInputSystem.Instance;
        bool bLBtnClick = input.IsLeftMouseClick();
        bool bRBtnClick = input.IsRightMouseClick();

        //-----------------------------------------------------------
        //鼠标碰撞上的物体
        Vector3 fvMouseHitPlan = Vector3.zero;
        CObject pSelectObj = CObjectManager.Instance.GetMouseOverObject(input.GetMousePos(), out fvMouseHitPlan);
        //鼠标是否在ui上空
        bool bInUIHover = s_pUISystem != null && s_pUISystem.IsMouseHover() && input.InputCapture != InputCapture.ICS_GAME;

        //-----------------------------------------------------------
        //如果左右键都没有按下,计算鼠标命令
        ENUM_CURSOR_TYPE LastCursor = ENUM_CURSOR_TYPE.CURSOR_WINBASE;
        MOUSE_CMD_TYPE mouseType = MOUSE_CMD_TYPE.MCT_NULL;
        if (!bLBtnClick && !bRBtnClick)
        {
            CursorMng.Instance.MouseCommand_Set(bInUIHover, pSelectObj, fvMouseHitPlan, CActionSystem.Instance.GetDefaultAction());
            //设置光标类型
            SCommand_Mouse cmd = CursorMng.Instance.MouseCommand_GetLeft();

            if (bInUIHover)
            {
                // 如果在UI上，且没有处于拖拽状态时，默认显示普通鼠标 [2/15/2012 Ivan]
                if (CursorMng.Instance.m_hUICursor == null)
                {
                    if (s_pUISystem.CurrHyperlink != null)
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_HYPERLINK_HOVER);
                    else
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_NORMAL);
                }
            }
            else
            {
                LastCursor = CursorMng.Instance.GetCursor();
                mouseType = cmd.m_typeMouse;
                switch (cmd.m_typeMouse)
                {
                    case MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO:	//移动
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_NORMAL);
                        break;

                    case MOUSE_CMD_TYPE.MCT_PLAYER_SELECT:	//选择目标
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_NORMAL);
                        break;

                    case MOUSE_CMD_TYPE.MCT_SKILL_OBJ:		//目标攻击
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_ATTACK);
                        break;
                        
			        case MOUSE_CMD_TYPE.MCT_SKILL_AREA:	//区域攻击
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_ATTACK);
				        break;

			        case MOUSE_CMD_TYPE.MCT_SKILL_DIR:		//方向型技能
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_ATTACK);
				        break;

			        case MOUSE_CMD_TYPE.MCT_HIT_TRIPPEROBJ:	//TripperObj
				    
					    CTripperObject pTripperObj = (CTripperObject)CObjectManager.Instance.FindServerObject(cmd.GetValue<int>(0));
					    if(pTripperObj != null)
						    CursorMng.Instance.SetCursor( pTripperObj.Tripper_GetCursor() );
					    else
                            CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_NORMAL);
                        break;
                    case MOUSE_CMD_TYPE.MCT_SPEAK:		//对话
                        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_SPEAK);
                        break;
                }

                //显示/隐藏技能范围指示器
			    if(MOUSE_CMD_TYPE.MCT_SKILL_AREA == cmd.m_typeMouse || 
			       (MOUSE_CMD_TYPE.MCT_USE_ITEM == cmd.m_typeMouse &&
			         cmd.GetValue<bool>(4)))
			    {
				    if(m_pSkillArea == null)
				    {
                        m_pSkillArea = (CObject_ProjTex_AuraDure)CObjectManager.Instance.NewProjTexAuraDure(-1);
					    m_pSkillArea.Initial(null);
                        
				    }
				    m_pSkillArea.SetShowEnable(true);
				    m_pSkillArea.SetPosition(fvMouseHitPlan);
                    if (MOUSE_CMD_TYPE.MCT_SKILL_AREA == cmd.m_typeMouse)
                    {
                        m_pSkillArea.SetRingRange(cmd.GetValue<float>(5));
                    }
			    }
			    else if(m_pSkillArea != null && m_pSkillArea.GetShowEnable())
			    {
				    m_pSkillArea.SetShowEnable(false);
			    }
            }
        }

        //-----------------------------------------------------------
	    //分析事件队列
        if (input.IsLeftMouseClick())
        {
            //激活鼠标命令
            // 			if(s_pUISystem && s_pUISystem->IsDragFlag())
            // 			{
            // 				s_pUISystem->ChangeDragFlag(false);
            // 			} 
            // 			else if (m_bCameraDraging)
            // 			{
            // 				//显示鼠标
            // 				CCursorMng::GetMe()->ShowCursor(true);
            // 				s_pInputSystem->SetCapture(ICS_NONE);
            // 			}
            // 			else
            {
                if (CursorMng.Instance.MouseCommand_GetLeft().m_typeMouse == MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO)
                {
                    AutoReleaseSkill.Instance.SetTargetObject(-1);
                    s_pGameInterface.StopAutoHit();
                }
                CursorMng.Instance.MouseCommand_Active(CursorMng.Instance.MouseCommand_GetLeft());
            }
        }
        else if (input.IsRightMouseClick())
        {
            CursorMng.Instance.MouseCommand_Active(CursorMng.Instance.MouseCommand_GetRight());
        }

        //鼠标滚轮操作
        float scrollWheel = input.GetAxis("Mouse ScrollWheel");
        GFX.SceneCamera.Instance.setScrollWheel(scrollWheel);
        
        if (input.isMouseRightHold() && !bInUIHover)//摄像机旋转操作
        {
            float mouseHorMove = input.GetAxis("Mouse X");
            if (Mathf.Abs(mouseHorMove) > 0.0001)
            {
                GFX.SceneCamera.Instance.AddDirection(mouseHorMove);
            }
        }

      
        //-----------------------------------------------------------

        // 显示鼠标
        if (CursorMng.Instance.GetCursor() != LastCursor)
        {
            CursorMng.Instance.OnSetCursor();
        }
    }

    private void UpdateClickShortKey()
    {
        CInputSystem input = CInputSystem.Instance;
        if (input.IsKeyDown(KeyCode.Alpha0))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha0);
        else if (input.IsKeyDown(KeyCode.Alpha1))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha1);
        else if (input.IsKeyDown(KeyCode.Alpha2))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha2);
        else if (input.IsKeyDown(KeyCode.Alpha3))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha3);
        else if (input.IsKeyDown(KeyCode.Alpha4))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha4);
        else if (input.IsKeyDown(KeyCode.Alpha5))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha5);
        else if (input.IsKeyDown(KeyCode.Alpha6))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha6);
        else if (input.IsKeyDown(KeyCode.Alpha7))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha7);
        else if (input.IsKeyDown(KeyCode.Alpha8))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha8);
        else if (input.IsKeyDown(KeyCode.Alpha9))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.Alpha9);
        else if (input.IsKeyDown(KeyCode.F1))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F1);
        else if (input.IsKeyDown(KeyCode.F2))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F2);
        else if (input.IsKeyDown(KeyCode.F3))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F3);
        else if (input.IsKeyDown(KeyCode.F4))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F4);
        else if (input.IsKeyDown(KeyCode.F5))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F5);
        else if (input.IsKeyDown(KeyCode.F6))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F6);
        else if (input.IsKeyDown(KeyCode.F7))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F7);
        else if (input.IsKeyDown(KeyCode.F8))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F8);
        else if (input.IsKeyDown(KeyCode.F9))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F9);
        else if (input.IsKeyDown(KeyCode.F10))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F10);
        else if (input.IsKeyDown(KeyCode.F11))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F11);
        else if (input.IsKeyDown(KeyCode.F12))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SHORTKEY, (int)KeyCode.F12);
    }
    //处理键盘输入
    public void ProcessInput_Keybord() 
    {
        CInputSystem input = CInputSystem.Instance;
        if (CInputSystem.Instance.IsKeyDown(KeyCode.Space))//空格键复位摄像机
        {
            GFX.SceneCamera.Instance.ResetCamera();
        }
        if (input.IsKeyDown(KeyCode.K))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ACCELERATE_KEYSEND, (int)KeyCode.K);
        }

        if (input.IsKeyDown(KeyCode.O))
        {
            // 这个只是为了检查是否已经下载窗口
            //UIWindowMng.Instance.GetWindow("FaBaoWindow");
            //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OPEN_TALISMANITEM);

            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ACCELERATE_KEYSEND, (int)KeyCode.O);
        }

        // 选定最近的目标 [4/11/2012 Ivan]
        if (input.IsKeyDown(KeyCode.Tab))
        {
            //CObjectManager.Instance.LockNearestEnemy();
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ACCELERATE_KEYSEND, (int)KeyCode.Tab);
        }

        // m键显示大地图 [3/31/2012 Ivan]
        if (input.IsKeyDown(KeyCode.M))
        {
            // 这个只是为了检查是否已经下载窗口
            //UIWindowMng.Instance.GetWindow("SceneMapWindow");
            //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_SCENEMAP);
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ACCELERATE_KEYSEND, (int)KeyCode.M);
        }

        if (input.IsKeyDown(KeyCode.R))
        {
            s_pObjectManager.getPlayerMySelf().Ride = !s_pObjectManager.getPlayerMySelf().Ride;

        }

        // 广播回车键按下 [4/23/2012 Ivan]
        if (input.IsKeyUp(KeyCode.Return))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UI_INFOS, "KeyDown_Enter");

        }

        UpdateClickShortKey();
    }

    public static int m_nResetCamera;
    //////////////////////////////////////////////////////////////////////////
    //private:
    ////-------------------------------------------
    ////当前激活的技能
    tActionItem					m_pActiveSkill;
    ////-------------------------------------------
    ////技能范围指示环
    CObject_ProjTex_AuraDure	m_pSkillArea;
    //鼠标目的地指示环
    CObject_ProjTex_MouseTarget	m_pMouseTarget;
    ////鼠标点击特效
    //CObject_Effect_MouseTarget*		m_pMouseClickedEffect;

    public GamePro_Main()
    {
        //m_bCameraDraging = false;
        // m_pSkillArea = NULL;
        //m_pActiveSkill = NULL;
        //m_pMouseTarget = NULL;
        m_bAutoRun = false;
        m_bUseKeyboardMove = false;
        m_bKeepMoving = false;
        //m_fKeepMoveSpeed = 0.0f;
        m_bKeepturning = false;
        //m_fTurnSpeed = 0.0f;
        //m_bAutoMoveAlong = false;
        //m_pMouseClickedEffect = NULL;

        //-------------------------------------------------------------------
        // 设置登录模式
        loginState = 0;
    }
    ~GamePro_Main() { }

};
