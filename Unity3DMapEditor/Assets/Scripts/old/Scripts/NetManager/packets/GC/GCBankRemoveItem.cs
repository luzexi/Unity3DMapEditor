
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCBankRemoveItem : PacketBase
    {
        public enum PosType
		{
			EQUIP_POS = 0,
			BAG_POS,
			ERROR_POS,
		};
		public enum OPtype
		{
			OPERATE_ERROR	=0,		// 错误操作
			OPERATE_MOVE,			// 移动到空格
			OPERATE_SWAP,			// 交换物品
			OPERATE_SPLICE,			// 叠加
			OPERATE_SPLIT,			// 拆分
		};

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_ToType) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_indexFrom) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_indexTo) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_OperateType) != sizeof(byte)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BANKREMOVEITEM;
        }

        public override int getSize()
        {
            return sizeof(byte)*4;
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
        public byte operateType
        {
            get { return this.m_OperateType; }
            set { m_OperateType = value; }
        }


        byte m_ToType;
        byte m_indexFrom;
        byte m_indexTo;
        byte m_OperateType;
    };

    public class GCBankRemoveItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBankRemoveItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BANKREMOVEITEM; }
        public override int GetPacketMaxSize()
        {
            return  sizeof(byte)*4;
        }
    };
}