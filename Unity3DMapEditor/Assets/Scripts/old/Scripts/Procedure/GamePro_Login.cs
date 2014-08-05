using System;
using System.Text;
using System.Collections.Generic;

using UnityEngine;
// net work
using Network;
using Network.Packets;
using Network.Handlers;

public class GamePro_Login : GameProcedure
{

	//登录状态
	public enum PLAYER_LOGIN_STATUS
	{
		LOGIN_DEBUG_SETTING,			//!< -- FOR DEBUG 用户参数

		LOGIN_SELECT_SERVER,			// 选择服务器界面.
		LOGIN_DISCONNECT,				//!< 尚未登录
		
		LOGIN_CONNECTING,				//!< 连接服务器中...
		LOGIN_CONNECTED_OK,				//!< 成功连接到服务器
		LOGIN_CONNECT_FAILED,			//!< 连接到服务器失败

		LOGIN_ACCOUNT_BEGIN_REQUESTING,	// 发送密码之前状态
		LOGIN_ACCOUNT_REQUESTING,		//!< 发送帐号信息数据包到服务器...
		LOGIN_ACCOUNT_OK,				//!< 帐号验证成功
		LOGIN_ACCOUNT_FAILED,			//!< 帐号验证失败

		LOGIN_WAIT_FOR_LOGIN,			// 排队进入游戏状态.

		LOGIN_FIRST_LOGIN,				// 首次登录
		LOGIN_CHANGE_SCENE,				// 切换场景的重登录
	};

	public PLAYER_LOGIN_STATUS	GetStatus() { return m_Status; }
	public void					SetStatus(PLAYER_LOGIN_STATUS status) { m_Status = status; }
	public void					SetRelogin(bool bReLogin) { m_bReLogin = bReLogin; }

	// 设置登录状态为首次登录（以区分切换场景的登录状态）
	public void					FirstLogin(){
        GameProcedure.s_pVariableSystem.SetAs_Int("Login_Mode", (int)PLAYER_LOGIN_STATUS.LOGIN_FIRST_LOGIN);
    }

	// 设置登录状态为切换场景的登录状态
	public void					LoginForChangeScene(){

        GameProcedure.s_pVariableSystem.SetAs_Int("Login_Mode", (int)PLAYER_LOGIN_STATUS.LOGIN_CHANGE_SCENE);
    }

	//-----------------------------------------------------------------------------------------------------------
	//
	// 从配置文件得到, login server 的ip地址和端口号
	//
	//

	// login server 的ip地址和端口号.
	public string	mLoginServerAddr;

	public int		mLoginServerPort;

	// 设置端口号.
	//public void SetPort(int iPort){}

	// 设置ip地址
	//public void SetIpAddr(string szServerAddr){}


	// 区域信息
    public List<AREA_INFO> mAreaInfo;

	// 区域个数
	public int			m_iAreaCount;

	//推荐服务器信息
	//vector<pair<int, int>> m_serverComment;

	//推荐服务器个数
	public int			m_sCommentCount;

	// 读取服务器配置文件的错误信息状态
	public enum LOAD_LOGIN_SERVER_ERR
	{
		LOAD_LOGIN_SERVER_SUCCESS = 0,		// 读取成功.
		LOAD_LOGIN_SERVER_FILE_NOT_FINDED,	// 找不到文件
		LOAD_LOGIN_SERVER_AREA_NOT_FINDED,	// 没有区域
		LOAD_LOGIN_SERVER_OTHER_ERR,		// 其他错误

	};

    const int PROVIDE_COUNT = 3;

	// 读取login server信息
	//
	// 返回值: 
	//			0 -- 读取成功
	//			1 -- 失败找不到配置文件
	//          2 -- 其他错误    
    public int LoadLoginServerInfo(string strLoginServerInfoFile) {
        if (mAreaInfo == null)
            mAreaInfo = new List<AREA_INFO>();

        //load server info

        return 0;
    }


	// 读取last login server信息
	//
	// 返回值: 
	//			0 -- 读取成功
	//			1 -- 失败找不到配置文件
	//          2 -- 其他错误    
	// 2011-5-5 add by ZL
    public int LoadLastLoginServerInfo(string strLastLoginServerInfoFile) { return -1; }

	// 保存last login server信息
	//
	// 返回值: 
	//			0 -- 保存成功
	//			1 -- 失败找不到配置文件
	//          2 -- 其他错误    
	// 2011-5-5 add by ZL
    public int SaveLastLoginServerInfo(string strLastLoginServerInfoFile) { return -1; }
	// 读取区域信息.
	//int ReadAreaInfo(std::string& strLoginServer, int iAreaIndex);

	// 读取login server信息.
	//int ReadLoginInfo(FILE* pFile, int iAreaIndex, int iLoginServerIndex);

	// 初始化区域信息
	public void InitLoginAreaInfo(){}

	//
	// 
	// 从配置文件得到, login server 的ip地址和端口号
	//
	//-----------------------------------------------------------------------------------------------------------------------------
	



	//-----------------------------------------------------------------------------------------------------------------------------
	//
	// 逻辑数据操作
	//
	//

	// 得到区域的个数
    public int GetAreaCount() { return 0; }

	// 得到区域名字
    public string GetAreaName(int iAreaIndex) { return "FR"; }

	// 得到区域信息
    public AREA_INFO GetAreaInfo(int iAreaIndex) { return new AREA_INFO(); }

	// 得到区域中login server 的个数
    public int GetAreaLoginServerCount(int iAreaIndex) { return 0; }

	// 得到区域中login server 的信息.
    public LOGINSERVER_INOF GetAreaLoginServerInfo(int iAreaIndex, int iLoginServerIndex) { return new LOGINSERVER_INOF(); }

	// 推荐服务器个数
    public int getCommendCount() { return 0; }

	// 推荐服务器区域编号
    public int getCommendArea(int idx) { return -1; }

	// 推荐服务器编号
    public int getCommendServer(int idx) { return - 1; }

	// 连接到login server
    public int ConnectToLoginServer(int iAreaIndex, int iLoginServerIndex)
    {
        if (iAreaIndex < 0 || iAreaIndex >= m_iAreaCount)
            return 1;

        if (iLoginServerIndex >= (int)mAreaInfo[iAreaIndex].LoginInfo.Count)
            return 1;

        // 设置ip地址和端口号.
        //SetIpAddr( m_pAreaInfo[iAreaIndex].LoginInfo[iLoginServerIndex].szIp.c_str() );
        //SetPort( m_pAreaInfo[iAreaIndex].LoginInfo[iLoginServerIndex].iPort );
        mLoginServerAddr = mAreaInfo[iAreaIndex].LoginInfo[iLoginServerIndex].szIp;
        mLoginServerPort = mAreaInfo[iAreaIndex].LoginInfo[iLoginServerIndex].iPort;

        // 设置当前的状态为非连接状态
        SetStatus(PLAYER_LOGIN_STATUS.LOGIN_DISCONNECT);

        // 通知界面显示系统提示信息, 正在连接服务器.
        //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, "正在连接到服务器.....");

        return 0;
    }

	// 连接到login server
    public int ConnectToLoginServer() { 
        // 设置当前的状态为非连接状态
	    SetStatus(PLAYER_LOGIN_STATUS.LOGIN_DISCONNECT);

	    // 通知界面显示系统提示信息, 正在连接服务器.
	    //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, "正在连接到服务器.....");
	    return 0;
    }

	// 当前选择的区域id
	public int m_iCurSelAreaId;

	// 当前选择的login server id
	public int m_iCurSelLoginServerId;

	// 记录选择的服务器
    public int SelectLoginServer(int iAreaIndex, int iLoginServerIndex) { return 0; }

	// 在界面上选择旧的服务器
	public void SelectOldServer(){}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// 自动选择区域数据
	//

	// 当前选择的区域
	public int[] m_iAutoAreaIndex = new int[3];

	// 当前选择的login server 索引
	public int[] m_iAutoLoginServerIndex = new int[3];


	// 前一次找到网络状态空闲的索引.
	public int m_iPreFindIdle;

	// 前一次找到网络状态正常的索引.
	public int m_iPreFindNor;

	// 前一次找到网络状态拥挤的索引.
	public int m_iPreFindBusy;

	// 前一次找到网络状态爆满的索引.
	public int m_iPreFindFull;

	// 电信供应商
	//PROVIDE_INFO m_Provide0;

	// 网通供应商
	//PROVIDE_INFO m_Provide1;

	// 其他供应商
	//PROVIDE_INFO m_Provide2;

	// 通过网络供应商选择一个login server
    public int AutoSelLoginServer(int iProvide) { return -1; }

	// 构造自动选择login server需要的数据表
    public int BuildAutoSelTable() { return -1; }


	// 
	//  逻辑数据操作
	//
	//-------------------------------------------------------------------------------------------------------------------------------




	//-------------------------------------------------------------------------------------------------------------------------------
	//
	// 界面操作
	//

	// 打开帐号输入界面
    public int OpenCountInputDlg() { return 0; }

	// 打开服务器选择界面
    public int OpenSelectServerDlg() { return 0; }

	// 打开人物选择界面
    public int OpenSelectCharacter() { return 0; }

	// 切换到人物选择界面
	public int ChangeToCharacterSel(){
        // 保存选择的服务器
	    //CGameProcedure::s_pVariableSystem->SetAs_Int("Login_Area",   CGameProcedure::s_pProcLogIn->m_iCurSelAreaId, FALSE);
	    //CGameProcedure::s_pVariableSystem->SetAs_Int("Login_Server", CGameProcedure::s_pProcLogIn->m_iCurSelLoginServerId, FALSE);

	    // 关闭系统界面
	    //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_CLOSE_SYSTEM_INFO);

	    // 关闭帐号输入界面
	    //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_CLOSE_COUNT_INPUT);

	    //切换到人物选择界面
	    GameProcedure.SetActiveProc(GameProcedure.s_ProcCharSel);

	    // 设置用户名
	    GameProcedure.s_ProcCharSel.SetUserName(m_strUserName);

	    // 进入到人物选择界面， 清空ui模型。
	    GameProcedure.s_ProcCharSel.m_bClearUIModel = true;

	    // 发送得到角色信息消息.
		CLAskCharList msg = (CLAskCharList) NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_ASKCHARLIST);
		byte[] temp = Encoding.ASCII.GetBytes(m_strUserName);
        Array.Copy(temp, msg.SzAccount, temp.Length);
		NetManager.GetNetManager().SendPacket(msg);
        LogManager.Log("Send Packet: CLAskCharList. ID:" + msg.getPacketID());

        // 显示人物选择界面
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_OPEN_SELECT_CHARACTOR);
	    
        return 0;

    }

	//
	// 界面操作
	//
	//-------------------------------------------------------------------------------------------------------------------------------


	//-------------------------------------------------------------------------------------------------------------------------------
	//
	// 网络命令消息操作
	//

	// 发送同步消息
	public int SendClConnectMsg(){

        LogManager.Log("Send CLConnect");

        CLConnect msg = (CLConnect)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_CONNECT);

	    // 发送网络连接消息
	    NetManager.GetNetManager().SendPacket(msg);
	    return 0;
    }

	// 用户名
	public string m_strUserName;

	// 密码
	public string m_strPassword;

	// 检查用户名和密码.
    public int CheckAccount(string szUserName, string szPassword) { 
		
		m_strUserName = szUserName;
		m_strPassword = szPassword;
		
		ConnectToLoginServer();
		
		return 0;
	}

	// 发送check msg 消息
	public int SendCheckAccountMsg()
    {
        // 发送验证密码消息
	    CLAskLogin msg = (CLAskLogin)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_ASKLOGIN);
        byte[] temp = Encoding.ASCII.GetBytes(m_strUserName);
        Array.Copy(temp, msg.SzAccount, temp.Length);
        temp = Encoding.ASCII.GetBytes(m_strPassword);
        Array.Copy(temp, msg.SzPassWord, temp.Length);
        msg.UVersion = LCRetConnectHandler.verson;
	    NetManager.GetNetManager().SendPacket(msg);
        LogManager.Log("Send CLAskLogin");
		//LogManager.Log("Send Packet: CLAskLogin");
        //todo
        //std::string isGameReLogin = CVariableSystem::GetMe()->GetVariable("IsGameReLogin");
        //if (isGameReLogin.empty() || isGameReLogin == "0")
        //{
        //    CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, "正在验证密码....");
        //}
        //// 只有返回登录的第一次才不显示这个ui [9/5/2011 Ivan edit]
        //CVariableSystem::GetMe()->SetVariable("IsGameReLogin", "0");

        //s_pVariableSystem->SetVariable( "User_NAME", m_strUserName.c_str() );
        s_pVariableSystem.SetVariable("User_NAME", m_strUserName);
	    return 0;
    }

	//
	// 网络命令消息操作
	//
	//-------------------------------------------------------------------------------------------------------------------------------


	//处理输入
	public override void	ProcessInput(){}


    public override void Init() {

        //打开登录界面
        if (PLAYER_LOGIN_STATUS.LOGIN_SELECT_SERVER == m_Status)
        {
            //s_pEventSystem->PushEvent(GE_GAMELOGIN_OPEN_SELECT_SERVER);
        }
        //加载登录场景
        UnityEngine.GameObject.Find("Main Camera").GetComponent<LoadScene>().EnterSceneWithoutNav("chuangjian/chuangjian.ab");
        //设置摄像机参数
        UseLoginCamera();
    }
     public override void Tick()
     {

         base.Tick();

         switch (m_Status)
         {
             case PLAYER_LOGIN_STATUS.LOGIN_DEBUG_SETTING:
                 {
                     
                     SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_DISCONNECT);
                    
                 }
                 break;

             case PLAYER_LOGIN_STATUS.LOGIN_SELECT_SERVER:// 选择服务器状态
                 {
                     //--- for debug
                     //if(CGameProcedure::s_pVariableSystem->GetAs_Int("GameServer_ConnectDirect") == 1)
                     //{
                     //    //直接切换到Change-Server流程
                     //    CGameProcedure::SetActiveProc((CGameProcedure*)CGameProcedure::s_pProcChangeScene);
                     //    return;
                     //}
                     if(GameProcedure.s_pVariableSystem.GetAs_Int("GameServer_ConnectDirect") == 1)
                     {
                           GameProcedure.SetActiveProc(GameProcedure.s_ProcChangeScene);
                     }
                     //--- for debug
                     break;
                 }
             case PLAYER_LOGIN_STATUS.LOGIN_DISCONNECT:
                 {
                     //s_pGfxSystem->PushDebugString("Connect to login server %s:%d...", m_szLoginServerAddr, m_nLoginServerPort);
                     LogManager.Log("GamePro_Login ip:" + mLoginServerAddr + " port:" + mLoginServerPort);   
                     //开始登录
                     SetStatus(PLAYER_LOGIN_STATUS.LOGIN_CONNECTING);
                     NetManager.GetNetManager().ConnectToServer(mLoginServerAddr, mLoginServerPort);
                     

                 }
                 break;

             case PLAYER_LOGIN_STATUS.LOGIN_CONNECTING:

                 break;

             //连接成功
             case PLAYER_LOGIN_STATUS.LOGIN_CONNECTED_OK:
                 {

                     // 设置正在验证密码
                     SetStatus(PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_REQUESTING);

                 }
                 break;

             //连接失败
             case PLAYER_LOGIN_STATUS.LOGIN_CONNECT_FAILED:
				 LogManager.Log("Can not connect to server!");
                 NetManager.GetNetManager().Disconnect();
                 SetStatus(PLAYER_LOGIN_STATUS.LOGIN_SELECT_SERVER);
                 break;

             case PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_BEGIN_REQUESTING:
                 {
                     break;
                 }
             // 正在验证用户名和密码.
             case PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_REQUESTING:
                 {

                     // 判断是否超时, 超时就提示错误信息.
                     break;
                 }
             //登录信息验证成功
             case PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_OK:
                 {
                     // 保存选择的服务器
                     //CGameProcedure::s_pVariableSystem->SetAs_Int("Login_Area",   CGameProcedure::s_pProcLogIn->m_iCurSelAreaId, FALSE);
                     //CGameProcedure::s_pVariableSystem->SetAs_Int("Login_Server", CGameProcedure::s_pProcLogIn->m_iCurSelLoginServerId, FALSE);

                     //if(m_bReLogin)
                     //{
                     //    //直接进入场景
                     //    CGameProcedure::s_pProcEnter->SetStatus(CGamePro_Enter::ENTERSCENE_READY);
                     //    CGameProcedure::s_pProcEnter->SetEnterType(ENTER_TYPE_FIRST);
                     //    CGameProcedure::SetActiveProc((CGameProcedure*)CGameProcedure::s_pProcEnter);
                     //}
                     //else
                     {
                         // 设置登录状态为首次登录（以区分切换场景的登录状态）
                         GameProcedure.s_ProcLogIn.FirstLogin();

                         //转入人物选择循环
                         GameProcedure.SetActiveProc(s_ProcCharSel);

                     }
                 }
                 break;
             default:
                 break;
         }
    }
	public override void	Render(){}
	public override void	Release(){}


	// 与服务器的网络连接状态.
	PLAYER_LOGIN_STATUS	m_Status;
	//是否是从从新连接新的服务器
	bool				m_bReLogin;
	//背景音乐
	//tSoundSource	m_pSoundSource;


	public GamePro_Login(){
        //m_Status = PLAYER_LOGIN_STATUS.LOGIN_DEBUG_SETTING;
        m_Status = PLAYER_LOGIN_STATUS.LOGIN_SELECT_SERVER;
        m_bReLogin = false;
        //m_pSoundSource = NULL;

        // 读取服务器配置信息.
        //LoadLoginServerInfo();

        // 读取最后登录信息
       // LoadLastLoginServerInfo();

        // 构造自动选择服务器查找表.
        //BuildAutoSelTable();

        // 用户名
        m_strUserName = "";

        // 密码
        m_strPassword = "";

        for (int i = 0; i < PROVIDE_COUNT; i++)
        {
            // 当前选择的区域
            m_iAutoAreaIndex[i] = -1;

            // 当前选择的login server 索引
            m_iAutoLoginServerIndex[i] = -1;
        }

        // 前一次找到网络状态空闲的索引.
        m_iPreFindIdle = -1;

        // 前一次找到网络状态正常的索引.
        m_iPreFindNor = -1;

        // 前一次找到网络状态拥挤的索引.
        m_iPreFindBusy = -1;

        // 前一次找到网络状态爆满的索引.
        m_iPreFindFull = -1;
    }
	~GamePro_Login(){}
};