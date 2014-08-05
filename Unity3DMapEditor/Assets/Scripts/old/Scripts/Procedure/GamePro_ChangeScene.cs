
/****************************************\
*										*
* 			   登录流程					*
*										*
\****************************************/

using System;
using System.Text;

using UnityEngine;

using Network;
using Network.Packets;

public class GamePro_ChangeScene: GameProcedure
{

	//登录状态
	public enum PLAYER_CHANGE_SERVER_STATUS
	{
		CHANGESCENE_DISCONNECT,					// 断开状态
		CHANGESCENE_CONNECTING,					// 连接中
		CHANGESCENE_CONNECT_SUCCESS,			// 连接成功
		CHANGESCENE_SENDING_CGCONNECT,			// 正在发送握手消息
		CHANGESCENE_RECEIVE_CGCONNECT_SUCCESS,	// 允许进入服务器
		CHANGESCENE_RECEIVE_CGCONNECT_FAIL,		// 不允许进入服务器
	};

	// 流程状态
	public PLAYER_CHANGE_SERVER_STATUS m_Status;

	// 设置状态
    public void SetStatus(PLAYER_CHANGE_SERVER_STATUS status) { m_Status = status; }

	// 得到状态
    public PLAYER_CHANGE_SERVER_STATUS GetStatus() { return m_Status; }

	//
	public void ConnectToGameServer(){

        //STRING strIp = CGameProcedure::s_pVariableSystem->GetAs_String("GameServer_Address");
        //INT			iPort = CGameProcedure::s_pVariableSystem->GetAs_Int("GameServer_Port");
        //CNetManager::GetMe()->ConnectToServer(strIp.c_str(), iPort);
        //SetStatus(CHANGESCENE_CONNECTING);

        string strIP = GameProcedure.s_pVariableSystem.GetAs_String("GameServer_Address");
        int nPort = GameProcedure.s_pVariableSystem.GetAs_Int("GameServer_Port");
        if(nPort != 0) //exist
        {

            NetManager.GetNetManager().ConnectToServer(strIP, nPort);
            SetStatus(PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_CONNECTING);
           // LogManager.Log("Connect to Server");
        }

    }

	// 发送cgconnect
	public void SendCGConnectMsg(){
        CGConnect msg = (CGConnect) NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_CONNECT);

        msg.Key = GameProcedure.s_pVariableSystem.GetAs_Int("GameServer_Key");
        msg.ServerID = -1;
        msg.GUID = GameProcedure.s_pVariableSystem.GetAs_Int("User_GUID");
        byte[] temp = Encoding.ASCII.GetBytes(GameProcedure.s_pVariableSystem.GetAs_String("User_NAME"));
        Array.Copy(temp, msg.SzAccount, temp.Length);
        msg.Gender = GameProcedure.s_pVariableSystem.GetAs_Int("User_GENDER");
        msg.CheckVer = 0x00000005;
        NetManager.GetNetManager().SendPacket(msg);

        SetStatus(PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_SENDING_CGCONNECT);
        //LogManager.Log("Send Connect to Server");
      
    }

	// 切换到enter game 流程
	public void ChangeToEnterGameProdure(){
        //切换到进入场景流程
	    GameProcedure.SetActiveProc(GameProcedure.s_ProcEnter);
	    GameProcedure.s_ProcEnter.SetStatus(GamePro_Enter.ENTER_STATUS.ENTERSCENE_READY);
    }


	public GamePro_ChangeScene(){
        m_Status = PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_DISCONNECT;
    }
	~GamePro_ChangeScene(){}

//////////////////////////////////////////////////////////////////////////
//protected:
    public override void Init(){}
    public override void Tick()
    {
        base.Tick();

        switch (GetStatus())
        {

            case PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_DISCONNECT:
                {

                    // 连接到 game server(socket) 
                    ConnectToGameServer();
                    break;
                }
            case PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_CONNECT_SUCCESS:
                {
                    //SetStatus(ENTERSCENE_REQUESTING);

                    // 发送CGConnect 消息
                    SendCGConnectMsg();
                    break;
                }
            case PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_SENDING_CGCONNECT:
                {
                    break;
                }
            case PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_RECEIVE_CGCONNECT_SUCCESS:
                {
                    ChangeToEnterGameProdure();
                    break;
                }
            case PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_RECEIVE_CGCONNECT_FAIL:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }

    }
    public override void Render(){}
    public override void Release(){}
};
