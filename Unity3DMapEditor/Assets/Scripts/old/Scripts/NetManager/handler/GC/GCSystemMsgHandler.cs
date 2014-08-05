using System;
using System.Collections.Generic;

using Network.Packets;
using System.Text;

namespace Network.Handlers
{
    public class GCSystemMsgHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LogManager.LogWarning("Recv GCSystemMsg");
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCSystemMsg msg = (GCSystemMsg)pPacket;
                string text = EncodeUtility.Instance.GetUnicodeString(msg.Contex);
                List<string> vParam = new List<string>();
                vParam.Add(msg.MessageType.ToString());
                vParam.Add(text);
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_GONGGAOMESSAGE,vParam);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SYSTEMMSG;
        }
    }
}