
using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CGBankSwapItem : PacketBase
    {
        public enum AddType
        {
            EQUIP_POS = 0,
            BAG_POS,
            BANK_POS,
        };
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_ToType);
            buff.WriteByte(m_FromType);
            buff.WriteByte(m_indexFrom);
            buff.WriteByte(m_indexTo);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_BANKSWAPITEM;
        }
        public override int getSize()
        {
            return sizeof(byte) * 4;
        }

        public byte ToType
        {
            set { m_ToType = value; }
        }
        public byte FromType
        {
            set { m_FromType = value; }
        }
        public byte IndexFrom
        {
            set { m_indexFrom = value; }
            get { return m_indexFrom; }
        }
        public byte IndexTo
        {
            set { m_indexTo = value; }
            get { return m_indexTo; }
        }
        byte m_ToType = 1;
        byte m_FromType = 1; //默认为背包
        byte m_indexFrom;
        byte m_indexTo;
    };

    public class CGBankSwapItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGBankSwapItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_BANKSWAPITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 4;
        }
    };
}