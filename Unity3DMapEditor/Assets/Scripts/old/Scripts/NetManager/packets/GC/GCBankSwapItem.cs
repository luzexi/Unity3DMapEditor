
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCBankSwapItem : PacketBase
    {
        public enum PosType
        {
            EQUIP_POS = 0,
            BAG_POS,
            BANK_POS,
            ERROR_POS,
        };
  

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_ToType) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_FromType) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_indexFrom) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_indexTo) != sizeof(byte)) return false;
            
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BANKSWAPITEM;
        }

        public override int getSize()
        {
            return sizeof(byte) * 4;
        }

        public byte ToType
        {
            get { return m_ToType; }
            set { m_ToType = value; }
        }
        public byte IndexFrom
        {
            get { return this.m_indexFrom; }
            set { m_indexFrom = value; }
        }

        public byte IndexTo
        {
            get { return this.m_indexTo; }
            set { m_indexTo = value; }
        }
        public byte FromType
        {
            get { return this.m_FromType; }
            set { m_FromType = value; }
        }


        byte m_ToType=1;
        byte m_indexFrom;
        byte m_indexTo;
        byte m_FromType=1;
    };

    public class GCBankSwapItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBankSwapItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BANKSWAPITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 4;
        }
    };
}