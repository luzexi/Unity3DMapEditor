
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCBankItemInfo : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_BankIndex) != sizeof(byte)) return false;
            if (buff.ReadInt(ref m_nsNull) != sizeof(uint)) return false;
            if (m_ITEM == null)
                m_ITEM = new _ITEM();
            m_ITEM.readFromBuff(ref buff);
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BANKITEMINFO;
        }

        public override int getSize()
        {
            return sizeof(short) + sizeof(int) + m_ITEM.getSize();
        }

        public short BankIndex
        {
            get { return this.m_BankIndex; }
            set { m_BankIndex = value; }
        }

        public int IsNull
        {
            get { return this.m_nsNull; }
            set { m_nsNull = value; }
        }
        public _ITEM Item
        {
            get { return m_ITEM; }
        }

        short m_BankIndex;		//item 的BagIndex
        int m_nsNull;		//物品是否为空		TRUE 代表没有Item,FALSE 代表有Item	
        _ITEM m_ITEM;
    };

    public class GCBankItemInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBankItemInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BANKITEMINFO; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(short) + _ITEM.GetMaxSize();
        }
    };
}