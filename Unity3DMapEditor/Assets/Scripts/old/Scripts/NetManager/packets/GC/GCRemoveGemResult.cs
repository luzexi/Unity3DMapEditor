using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCRemoveGemResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_REMOVEGEMRESULT;
        }

        public override int getSize()
        {
            return sizeof(REMOVEGEM_RESULT);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int r = 0;
            if (buff.ReadInt(ref r) != sizeof(int)) return false;
            m_Result = (REMOVEGEM_RESULT)r;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public REMOVEGEM_RESULT Result
        {
            get { return m_Result; }
        }
       private REMOVEGEM_RESULT m_Result;
    }

    public class GCRemoveGemResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRemoveGemResult(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_REMOVEGEMRESULT; }
        public override int GetPacketMaxSize() { return sizeof(REMOVEGEM_RESULT); }
    }
}