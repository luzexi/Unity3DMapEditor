using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCharmInfoFlushHandler: HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCCharmInfo Packet");
                GCCharmInfoFlush CharmInfoFlushPacket = (GCCharmInfoFlush)pPacket;
                CObjectManager pObjectManager = CObjectManager.Instance;
                CDataPool.Instance.PlayerCharmInfo = CharmInfoFlushPacket.CharmInfo;
            } 
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARMINFOFLUSH;
        }
    }
}