
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCResetTalismanBagSize : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_RESETTALISMANBAGSIZE;
        }

        public override int getSize()
        {
            return sizeof(byte)*2;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            if (buff.ReadByte(ref m_Type) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_TMBagSize) != sizeof(byte)) return false;
            return true;
        }
        public byte Type
        {
            get { return m_Type; }
        }

        public byte BagSize
        {
            get { return m_TMBagSize; }
        }
        byte m_Type;             //类型：0 法宝装备包，1 法宝存放包（未装备的法宝）
        byte m_TMBagSize;       //法宝存放包大小
    }

    public class GCResetTalismanBagSizeFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCResetTalismanBagSize(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_RESETTALISMANBAGSIZE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte)*2;
        }
    };
}