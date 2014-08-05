using System;
using Network;

using UnityEngine;
using DBSystem;
using Interface;

// 为了打包一些特殊的版本，加个开关控制 [3/17/2012 Ivan]
public enum CurrentVersion 
{
    Debug,
    Release,
//    CeHua,
}

/// <summary>
/// 取消客户端原有的mainloop方式
/// 将每个独立的更新，交由MainBehaviour调用，不再自身调用
/// </summary>
/// 
public class GameProcedure{

    static CurrentVersion version = CurrentVersion.Debug;
    public static CurrentVersion Version
    {
        get { return version; }
        set { version = value; }
    }
    /// 初始化静态变量
	public static void		InitStaticMemeber(){

        //-------------------------------------------------------------------
        //初始化所有的循环实例
        s_ProcLogIn         = new GamePro_Login();			//!< 登录循环
        s_ProcCharSel       = new GamePro_CharSel();		//!< 人物选择流程
        s_ProcCharCreate    = new GamePro_CharCreate();	//!< 人物创建流程
        s_ProcEnter         = new GamePro_Enter();			//!< 等待进入场景流程
        s_ProcMain          = new GamePro_Main();			//!< 主游戏循环
        s_ProcChangeScene   = new GamePro_ChangeScene();	//!< 服务器切换流程

        //////////////////////////////////////////////////////////////////////////
        s_NetManager = new NetManager();
        s_pTimeSystem = new TimeSystem();
        s_pWorldManager = new WorldManager();
        s_pVariableSystem = new VariableSystem();
        s_pGfxSystem = GFX.GfxSystem.Instance;//GfxSystem是一个单例
        s_pGameInterface = GameInterface.Instance;
        s_pObjectManager = CObjectManager.Instance;//物体管理器
        s_pDataBaseSystem = CDataBaseSystem.Instance;//数据库
        s_pDataPool = CDataPool.Instance;
        s_pUIDataPool = CUIDataPool.Instance;
        s_pInputSystem = CInputSystem.Instance;
        s_pUISystem = UISystem.Instance;
        s_random = new System.Random((int)(UnityEngine.Time.time*1000));
        s_pEventSystem = CEventSystem.Instance;
        s_pScriptSystem = CScriptSvm.Instance;
        /////////////////////////////////////////  /////////////////////////////////
        //initial
        //s_pDataBaseSystem.Initial(DBStruct.s_dbToLoad, DBStruct.GetResources);//初始化数据库，打开所有数据表

        s_NetManager.init((int)PACKET_DEFINE.PACKET_MAX, 30 );
        s_pTimeSystem.Initial();
        s_pWorldManager.Initial();
        s_pUISystem.Initial();

        s_pActionSetMgr = CActionSetMgr.Instance;//初始动作集合管理器
        s_pActionSetMgr.Init("Private/ActionSet/");
        s_pGameInterface.Initial();
        s_pDataPool.Initial();
        s_pUIDataPool.Initial();
        s_pScriptSystem.Initial();
        MissionList.Instance.Initial();
        RandomName.Instance.Init();
        OpenFuncSystem.Instance.Initial();
        //////////////////////////////////////////////////////////////////////////
        mainBehaviour = GameObject.Find("MainLoop").GetComponent<MainBehaviour>();

        // 控制打印等级 [2/8/2012 Ivan]
        LogManager.EnableWarning();
    }

	///	将一个游戏循环激活
    public static void SetActiveProc(GameProcedure ToActive)
    {
        //LogManager.Log("Change Procedure: " + ToActive.GetType());

        if (null == ToActive || s_ProcActive == ToActive)
            return;

        s_ProcPrev = s_ProcActive;
        s_ProcActive = ToActive;
    }
	/// 进入当前游戏循环的数据逻辑函数
    public static void TickActive() {
        //如果要转入新的游戏循环..
        if (s_ProcActive != s_ProcPrev)
        {
            //调用旧循环的释放函数
            if (s_ProcPrev != null)
                s_ProcPrev.Release();
            //调用新循环的初始化函数
            if (s_ProcActive != null) 
                s_ProcActive.Init();
            //开始新的循环
            s_ProcPrev = s_ProcActive;
        }
        //执行激活循环的数据逻辑
        if (s_ProcActive != null) 
            s_ProcActive.Tick();
    }
	/// 让事件系统处理游戏事件
    public static void ProcessGameEvent() {

    }
	/// 键盘事件和鼠标事件的处理
    public static void ProcessActiveInput() 
    {
        //执行激活循环的键盘函数
        if (s_ProcActive == s_ProcPrev && s_ProcActive != null)
        {
            s_ProcActive.ProcessInput();
        }
    }
	/// 玩家请求退出程序事件
    public static void ProcessCloseRequest() { }
	/// 进入当前游戏循环的渲染函数
    //public static void RenderActive() { }
	/// 释放静态变量
    public static void ReleaseStaticMember() {
        s_NetManager.Release();
    }
	//为了返回登录，释放场景渲染
    public static void ReleaseForReLogin() { }


	/// 得到当前激活的循环
    public static GameProcedure GetActiveProcedure() { return s_ProcActive; }
	/// 主窗口是否处于激活状态
   // public static bool IsWindowActive() { return m_bActive; }
	// 是否正在播放视频 [11/15/2011 Sun]
    public static bool IsPlayVideo() { return false; }

	//  [12/13/2010 ivan edit]
    public static void InitNodesForReLogin(){}
    public static void InitEventForReLogin(){}
    public static void ReInitMembersForReLogin(){}

    public static void UseLoginCamera()
    {
        Camera cam = Camera.mainCamera;
        if (cam != null)
        {
            //设置摄像机参数，由美术提供
            Transform trasnform = cam.gameObject.transform;
            trasnform.position = new Vector3(25.457f, 5.26f, 4.0f);
			trasnform.rotation = Quaternion.Euler(10.652f, 36.33f, 0.789f);
        }
    }
    //根据美术提供的参数设置人物位置和朝向
    public static void setLoginPlayerTransform(CObject_PlayerOther player)
    {
        if (player != null)
        {
            player.SetPosition(new Vector3(27.41f, 3.37f, 6.48f));
            player.SetFaceDir( (216.02f)* Mathf.Deg2Rad);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // 游戏运行的流程.
    //

    /// 登录游戏循环
    public static GamePro_Login s_ProcLogIn;
    /// 人物选择流程
    public static GamePro_CharSel s_ProcCharSel;
    /// 人物创建流程
    public static GamePro_CharCreate s_ProcCharCreate;
    /// 等待进入场景流程
    public static GamePro_Enter s_ProcEnter;
    /// 主游戏循环
    public static GamePro_Main s_ProcMain;
    /// 当前激活的流程.
    public static GameProcedure s_ActiveProcedure;

    // 切换服务器流程
    public static GamePro_ChangeScene s_ProcChangeScene;

    public static MainBehaviour mainBehaviour;

    public static bool s_isRote = false;//当前登陆界面角色是否在旋转  [8/26/2011 zzy]
    public static float s_roteSpeed = 1.0f;//当前旋转速度
    //
    // 游戏运行的过程
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
    /************************************************************************/
    /* 
     * [12/21/2011 Sun]
     * 游戏管理器先屏蔽
     *
     * */
    /************************************************************************/
	///网络管理器
    public static NetManager s_NetManager;

    ///场景管理器
    public static WorldManager s_pWorldManager;

    // 变量管理器
    public static tVariableSystem s_pVariableSystem;

    /// 计时器
    public static TimeSystem s_pTimeSystem;

    // 渲染层指针
    public static  GFX.GfxSystem s_pGfxSystem;

    // UI系统
    public static  UISystem  s_pUISystem;

    //物体管理器
    public static  CObjectManager s_pObjectManager;

    // 数据库管理器
    public static  CDataBaseSystem s_pDataBaseSystem;

    //数据池
    public static  CDataPool s_pDataPool;

    //UI数据池
    public static CUIDataPool s_pUIDataPool;

    //输入管理器
    public static CInputSystem s_pInputSystem;

    //脚本管理器
    static CScriptSvm s_pScriptSystem;

    // 逻辑接口管理器
    public static GameInterface s_pGameInterface;

    public static System.Random s_random;//随机数

    //游戏事件管理器
    public static CEventSystem s_pEventSystem;
   /* /
    
    
    // 声音管理器
    static tSoundSystem* s_pSoundSystem;

    // 光标管理器
    static tCursorSystem* s_pCursorMng;

    //UI模型显示管理
    static tFakeObjSystem* s_pFakeObjSystem;
    //外接帮助系统
    static tHelperSystem* s_pHelperSystem;

    static CBuffImpactMgr* s_pBuffImpactMgr;
    static CDirectlyImpactMgr* s_pDirectlyImpactMgr;
    static CBulletDataMgr* s_pBulletDataMgr;
    static CSkillDataMgr* s_pSkillDataMgr;
    static CMissionDataMgr* s_pMissionDataMgr;
    //阵营
    static CampAndStandDataMgr_T* s_pCampDataMgr;

    

    // 任务需求列表 [6/14/2011 ivan edit]
    static QuestListDataMgr* s_pQuestListDataMgr;
    */

    static CActionSetMgr s_pActionSetMgr;

    /// 当前激活的循环
    static GameProcedure s_ProcActive;
    /// 上一个激活的循环，只在切换过程中使用
    static GameProcedure s_ProcPrev;

    // 是否需要马上刷新地图 [3/19/2012 Ivan]
    public static bool m_bNeedFreshMinimap;
    // 这个刷新频率低，每秒一下 [3/19/2012 Ivan]
    public static bool m_bWaitNeedFreshMinimap;
    // 得到前一个游戏流程.
    public static GameProcedure GetPreProcedure(){
        return s_ProcPrev;
    }

    public virtual void Init(){}
    public virtual void Tick(){

        s_pTimeSystem.Tick();//更新时间
        s_pInputSystem.Tick();
        
        //net manager
        s_NetManager.Tick();
        s_pObjectManager.Tick();//物体更新
        s_pGfxSystem.Tick();//图形更新
        s_pDataPool.Tick();//数据池更新
        s_pUISystem.Tick();

        // 放于最后处理逻辑事件 [5/3/2012 SUN]
        s_pEventSystem.Tick();
    }
    public virtual void Render(){}
    public virtual void Release(){}
    public virtual void ProcessInput(){}
    public virtual void CloseRequest(){}
}