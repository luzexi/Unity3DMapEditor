/****************************************\
*										*
* 		  进入场景前的等待流程			*
*										*
\****************************************/
using UnityEngine;

using Network;
using Network.Packets;

public class GamePro_Enter : GameProcedure
{

	//--------------------------------------------------------------
	//登录状态
	public enum ENTER_STATUS
	{
		ENTERSCENE_CONNECTING,	//!< 与game server进行连接
		ENTERSCENE_READY,		//!< 初始化状态

		ENTERSCENE_REQUESTING,	//!< 发送进入场景的请求...
		ENTERSCENE_OK,			//!< 进入场景成功，会转入下一个循环
		ENTERSCENE_FAILED,		//!< 进入场景失败
	};
	public ENTER_STATUS		GetStatus() { return m_Status; }
	public void				SetStatus(ENTER_STATUS status) { m_Status = status; }

	//--------------------------------------------------------------
	//设置所要进入的场景的信息(场景ID/城市id, 城市等级)
	public void SetSceneID(int nSceneID, int nCityLevel) 
	{ 
		m_nSceneID = nSceneID; 
		m_nCityLevel = nCityLevel;
	}
	// 设置场景ID，和资源ID [10/24/2011 Sun]
	public void SetSceneID(int nSceneID, int nResID, int nCityLevel)
	{
		m_nSceneResID = nResID;
		SetSceneID(nSceneID, nCityLevel);
	}

	public void			SetEnterType(uint dwEnterType) { m_dwEnterType = dwEnterType; }
	public uint			GetEnterType()  { return m_dwEnterType; }

	//进入指定场景
	public void				EnterScene(){

        //todo load scene
        //LogManager.Log("Enter Scene");

        if (!WorldManager.Instance.EnterScene(m_nSceneID, m_nSceneResID, m_nCityLevel)) return;
        AssetBundleManager.AssetBundleRequestManager.releaseAssetBundles();//切换场景时释放已下载的www对象
        //转入主循环
	    GameProcedure.SetActiveProc(GameProcedure.s_ProcMain);

        //创建玩家自身
	    int idMySelfServer = s_pVariableSystem.GetAs_Int("MySelf_ID");
	    CObject_PlayerMySelf pMySelf = s_pObjectManager.getPlayerMySelf();
	    if(pMySelf == null)
	    {
            pMySelf = s_pObjectManager.NewPlayerMySelf(idMySelfServer);
	    }
        s_pObjectManager.SetObjectServerID(pMySelf.ID, idMySelfServer);
        pMySelf.SetServerGUID(s_pVariableSystem.GetAs_Int("User_GUID"));
        
        fVector2 MySelfPos = s_pVariableSystem.GetAs_Vector2("MySelf_Pos");
        pMySelf.Initial(null);//初始化玩家，创建角色数据
		string posMsg = "玩家位置：";
		posMsg += MySelfPos.x.ToString()+" "+MySelfPos.y.ToString();
		//LogManager.Log(posMsg);
        pMySelf.SetMapPosition(MySelfPos.x, MySelfPos.y);

        //设置摄像机参数
        // TODO

        //////////////////////////////////////////////////////////////////////////
        // 检查是否在自动寻路 [8/9/2011 ivan edit]
        Interface.GameInterface.Instance.AutoMovePlayerReady();
		
		//产生事件
		CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PLAYER_ENTER_WORLD);
    }


//////////////////////////////////////////////////////////////////////////
// protected:
	// 进入游戏服务器的ip 地址和端口号
	char[]	m_szLoginServerAddr = new char[64];
	int		m_nLoginServerPort;

	// 进入游戏服务器的场景号，如果是玩家城市，则是玩家城市ID
	int		m_nSceneID;
	int		m_nSceneResID; // 新增场景资源ID [10/24/2011 Sun]
	//如果是玩家城市，城市等级
	int		m_nCityLevel;

	uint			m_dwEnterType;
	ENTER_STATUS	m_Status;


    public override void Init(){
        //((CObjectManager*)(s_pObjectManager))->SetLoadNPCDirect(TRUE);
    }
    public override void Tick(){
        base.Tick();
        
        
	switch(GetStatus())
	{

	case ENTER_STATUS.ENTERSCENE_CONNECTING:
		{
			//等待服务器的回应
			SetStatus(ENTER_STATUS.ENTERSCENE_READY);
			break;
		}
	//发送进入场景的请求
	case ENTER_STATUS.ENTERSCENE_READY:
		{
            int nSceneID = GameProcedure.s_pVariableSystem.GetAs_Int("Scene_ID");

			//发送进入场景的请求
			CGEnterScene msg = (CGEnterScene) NetManager.GetNetManager().CreatePacket((int) PACKET_DEFINE.PACKET_CG_ENTERSCENE);
            msg.EnterType = (byte) m_dwEnterType;
            msg.SceneID = (short)nSceneID;
            fVector2 pos = GameProcedure.s_pVariableSystem.GetAs_Vector2("Scene_EnterPos");

            WORLD_POS Pos;
            Pos.m_fX = pos.x;
            Pos.m_fZ = pos.y;
            msg.Position = Pos;
			
			NetManager.GetNetManager().SendPacket(msg);

            //LogManager.Log("Send EnterScene: " + nSceneID);
			//等待服务器的回应
			SetStatus(ENTER_STATUS.ENTERSCENE_REQUESTING);
		}
		break;

	//发送进入场景的请求,等待服务器的回应...
	case ENTER_STATUS.ENTERSCENE_REQUESTING:
		break;

	//服务器允许进入场景
	case ENTER_STATUS.ENTERSCENE_OK:
		{
			// 关闭系统界面
			//CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_CLOSE_SYSTEM_INFO);
			// 关闭帐号输入界面
			//CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_CLOSE_COUNT_INPUT);
			// 关闭人物选择界面
			//CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_CLOSE_SELECT_CHARACTOR);
			//设置场景摄像机
			//CGameProcedure::s_pGfxSystem->Camera_SetCurrent(CRenderSystem::SCENE);
			
			// 在外层已经调用，这里屏蔽 [6/2/2011 Sun]
			//EnterScene();
		}
		break;

	//服务器不允许进入场景
	case ENTER_STATUS.ENTERSCENE_FAILED:
		{
			//CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO_CLOSE_NET, "服务器不允许进入场景");	
		}
		break;
	}
    }
    public override void Render(){}
    public override void Release(){}
    //public override void MouseInput();


	public GamePro_Enter(){
        m_Status = ENTER_STATUS.ENTERSCENE_READY;
        //m_dwEnterType = ENTER_TYPE_FIRST;
    }
	~GamePro_Enter(){}
};
