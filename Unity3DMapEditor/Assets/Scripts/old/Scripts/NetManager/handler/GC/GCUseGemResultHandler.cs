using System;

using Network.Packets;

namespace Network.Handlers
{
    public class GCUseGemResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            //if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            //{

            //}
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USEGEMRESULT;
        }
    }
}