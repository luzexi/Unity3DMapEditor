using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class LCRetDeleteCharHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCRetDeleteChar msg = pPacket as LCRetDeleteChar;
            if(msg != null)
		{
			ASKDELETECHAR_RESULT res = msg.Result;
			switch(res)
			{
			case ASKDELETECHAR_RESULT.ASKDELETECHAR_SUCCESS:		//成功
				{
					GameProcedure.s_ProcCharSel.SetCurSelDel(-1);
					GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_Delete_ROLE_OK, "删除角色成功! ");	
					break;
				}
			case ASKDELETECHAR_RESULT.ASKDELETECHAR_SERVER_BUSY://服务器忙，重试
				{
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "服务器忙，重试! ");	
					break;
				}
			case ASKDELETECHAR_RESULT.ASKDELETECHAR_OP_TIMES:		//操作过于频繁
				{
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "再次删除一个角色需要15秒钟！");
					break;
				}
			case ASKDELETECHAR_RESULT.ASKDELETECHARR_EMPTY:		//没有角色删除
				{
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "没有角色删除! ");	
					break;
				}
			case ASKDELETECHAR_RESULT.ASKDELETECHAR_OP_ERROR:		//错误操作流程
				{
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "错误操作流程! ");	
					break;
				}
			case ASKDELETECHAR_RESULT.ASKDELETECHAR_INTERNAL_ERROR: //内部错误
				{
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "内部错误! ");	
					break;
				}
			default:
				{
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "未知错误! ");	
					break;
				}
			}
		}
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_RETDELETECHAR;
        }
    }
}