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
    public class LCRetCreateCharHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCRetCreateChar msg = (LCRetCreateChar)pPacket;
            LogManager.Log("RECV LCRetCreateChar: " + msg.Result);
            if (msg != null)
            {
                switch (msg.Result)
                {
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_SUCCESS:			//成功
                        {
                            //CGameProcedure.s_pProcCharCreate.ChangeToRoleSel();

                            GameProcedure.s_ProcCharCreate.Status = GamePro_CharCreate.PLAYER_CHAR_CREATE_STATUS.CHAR_CREATE_CREATE_OK;
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CREATE_CLEAR_NAME);
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CREATE_ROLE_OK);
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_SERVER_BUSY:		//服务器忙，重试
                        {
                            //LogManager.Log("服务器忙，重试! ");
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "服务器忙，重试! ");	
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_OP_TIMES:		//操作过于频繁
                        {
                            //LogManager.Log("再次创建一个角色需要15秒钟！ ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "再次创建一个角色需要15秒钟！");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_FULL:			//角色已经满了
                        {
                            //LogManager.Log("角色已经满了! ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "角色已经满了! ");	
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_OP_ERROR:		//错误操作流程
                        {
                            //LogManager.Log("错误操作流程 ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "错误操作流程! ");	
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_INTERNAL_ERROR:  //内部错误
                        {
                            //LogManager.Log("内部错误 ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "内部错误! ");	
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_SAME_NAME:
                        {
                            //LogManager.Log("角色名字已经存在 ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "角色名字已经存在");
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCREATECHAR_RESULT.ASKCREATECHAR_INVALID_NAME:
                        {
                            //LogManager.Log("角色名错误 ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "角色名错误");
                            break;
                        }
                    default:							// 未知错误
                        {
                           // LogManager.Log("未知错误! ");
                            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "未知错误! ");	
                            break;
                        }
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_RETCREATECHAR;
        }
    }
}