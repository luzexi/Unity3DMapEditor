using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{

    /// <summary>
    /// LCRetCharLoginHandler
    /// </summary>
    public class LCRetCharLoginHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCRetCharLogin msg = (LCRetCharLogin)pPacket;
            LogManager.Log("RECV LCRetCharLogin: " + msg.Result);
            if (msg != null)
            {

                switch (msg.Result)
                {
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_SUCCESS:		//成功
                        {
                            // 记录服务器ip地址在全局变量
                            string ip = Encoding.ASCII.GetString(msg.ServerIP);
                            GameProcedure.s_pVariableSystem.SetVariable("GameServer_Address", ip);

                            // 记录服务器端口号在全局变量
                            GameProcedure.s_pVariableSystem.SetAs_Int("GameServer_Port", msg.ServerPort);
                            //CGameProcedure::s_pVariableSystem->SetAs_Int("GameServer_Port", msg.ServerPort);

                            // 记录进入游戏角色的guid
                            GameProcedure.s_pVariableSystem.SetAs_Int("User_GUID", GameProcedure.s_ProcCharSel.m_EnterGameGUID);
                            //CGameProcedure::s_pVariableSystem->SetAs_Int("User_GUID", CGameProcedure::s_pProcCharSel->m_EnterGameGUID);

                            // 进入场景的user key
                            GameProcedure.s_pVariableSystem.SetAs_Int("GameServer_Key", msg.Key);
                            //CGameProcedure::s_pVariableSystem->SetAs_Int("GameServer_Key", msg.Key);

                            // 切换到服务器连接流程
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_CLEAR_ACCOUNT );
                            GameProcedure.SetActiveProc(GameProcedure.s_ProcChangeScene);
                            GameProcedure.s_ProcChangeScene.SetStatus(GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_DISCONNECT);

                            LogManager.Log("登陆成功");

                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLIST_WORLD_FULL:		//世界满了
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "服务器满了!");	
                            LogManager.Log("服务器满了!");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_SERVER_BUSY:	//服务器忙，重试
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "服务器忙，重试! ");
                            LogManager.Log("服务器忙，重试!");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_LOADDB_ERROR:	//角色载入出错
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "角色载入出错! ");	
                            LogManager.Log("角色载入出错!");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_OP_TIMES:		//操作过于频繁
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "操作过于频繁!");	
                            LogManager.Log("操作过于频繁!");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_NOT_OWNER:	//不是角色的所有者
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "不是角色的所有者! ");	
                            LogManager.Log("不是角色的所有者!");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_SERVER_STOP:	//服务器维护
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "服务器维护! ");	
                            LogManager.Log("服务器维护!");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT.ASKCHARLOGIN_CHANGE_SCENE:	//切换场景
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "角色正切换场景! ");	
                            LogManager.Log("角色正切换场景!");
                            break;
                        }
                    default:
                        {
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "未知错误! ");	
                            LogManager.Log("未知错误!");
                            break;
                        }
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_RETCHARLOGIN;
        }
    }
}