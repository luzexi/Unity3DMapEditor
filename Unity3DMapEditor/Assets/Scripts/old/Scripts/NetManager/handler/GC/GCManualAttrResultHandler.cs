using System;
using System.Collections.Generic;
using Network.Packets;

namespace Network.Handlers
{
    public class GCManualResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.LogWarning("Receive GCManualResult Packet");
                GCManualAttrResult packet = pPacket as GCManualAttrResult;
                int index = packet.Index;
                if (index < 0 || index >= GAMEDEFINE.HUMAN_PET_MAX_COUNT + 1)
                {
                    LogManager.LogError("GCManualResult index incorrect " + index);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
                CDataPool.Instance.RandomAttrs[index] = packet.Attrib;
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_ATTRIRANDOM);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MANUALATTRRESULT;
        }
    }
}