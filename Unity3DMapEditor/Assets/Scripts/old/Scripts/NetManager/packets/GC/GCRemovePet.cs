
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCRemovePet : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_REMOVEPET;
        }

        public override int getSize()
        {
            return m_GUID.getSize();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            m_GUID.readFromBuff(ref buff);
            return true;
        }
        public PET_GUID_t GUID
        {
            get { return m_GUID; }
        }
        private PET_GUID_t m_GUID;
    }

    public class GCRemovePetFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRemovePet(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_REMOVEPET; }
        public override int GetPacketMaxSize()
        {
            return PET_GUID_t.getMaxSize();
        }
    };
}