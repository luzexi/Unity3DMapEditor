using System;
using System.Collections.Generic;
using Network.Packets;
using UnityEngine;

namespace Network.Handlers
{
    public class GCOperateResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCOperateResult packResult = (GCOperateResult)pPacket;
            OPERATE_RESULT oCode = (OPERATE_RESULT)(packResult.Result);
            string resultText = GameDefineResult.Instance.GetOResultText(oCode);
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, resultText);
			//LogManager.LogWarning("OperateResult " + packResult.Result + " " + resultText);
            switch( oCode )
            {
                case OPERATE_RESULT.OR_NEED_SETMINORPASSWORD:
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MINORPASSWORD_OPEN_SET_PASSWORD_DLG);
                    break;
                case OPERATE_RESULT.OR_NEED_UNLOCKMINORPASSWORD:
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MINORPASSWORD_OPEN_UNLOCK_PASSWORD_DLG);
                    break;
                default:
                    break;
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_OPERATE_RESULT;
        }
    }
}
