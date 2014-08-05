
using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CGBankAddItem : PacketBase
    {
        public enum AddType
		{
			EQUIP_POS = 0,
			BAG_POS,
		};
        public enum AutoPosBox
		{
			AUTO_POS_BOX1 = 255,
			AUTO_POS_BOX2 = 254,
			AUTO_POS_BOX3 = 253,
			AUTO_POS_BOX4 = 252,
			AUTO_POS_BOX5 = 251,
		};
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_FromType);
            buff.WriteByte(m_indexFrom);
            buff.WriteByte(m_indexTo);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_BANKADDITEM;
        }
        public override int getSize()
        {
            return sizeof(byte) * 3;
        }

        public byte FromType
        {
            set { m_FromType = value; }
        }
        public byte IndexFrom
        {
            set { m_indexFrom = value; }
        }
        public byte IndexTo
        {
            set { m_indexTo = value; }
        }
        byte m_FromType = 1; //默认为背包
        byte m_indexFrom;
        byte m_indexTo;
    };

    public class CGBankAddItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGBankAddItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_BANKADDITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3;
        }
    };
}