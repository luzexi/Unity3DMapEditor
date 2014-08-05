
using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CGBankRemoveItem : PacketBase
    {
        public enum RemoveType
        {
            EQUIP_POS = 0,
            BAG_POS,
        };
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_ToType);
            buff.WriteByte(m_indexFrom);
            buff.WriteByte(m_indexTo);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_BANKREMOVEITEM;
        }
        public override int getSize()
        {
            return sizeof(byte) * 3;
        }

        public byte ToType
        {
            set { m_ToType = value; }
        }
        public byte IndexFrom
        {
            set { m_indexFrom = value; }
        }
        public byte IndexTo
        {
            set { m_indexTo = value; }
        }
        byte m_ToType = 1; //默认为背包
        byte m_indexFrom;
        byte m_indexTo;
    };

    public class CGBankRemoveItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGBankRemoveItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_BANKREMOVEITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3;
        }
    };
}