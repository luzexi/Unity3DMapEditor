
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCBankAcquireList : PacketBase
    {
        public struct _BANK_ITEM
        {
            public byte bankindex;
            public byte isBlueEquip;
            public byte byNumber;
            public uint item_guid;
            public _ITEM item_data;
      
        };

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_ItemNum);
            buff.ReadByte(ref m_CurBankSize);
            buff.ReadInt(ref m_Money);
            buff.ReadInt(ref m_RMB);
            for (int i = 0; i < m_ItemNum; i++)
            {
                buff.ReadByte(ref m_ItemList[i].bankindex);
                buff.ReadByte(ref m_ItemList[i].isBlueEquip);
                buff.ReadByte(ref m_ItemList[i].byNumber);
                if (m_ItemList[i].isBlueEquip != 0)
                {
                    if (m_ItemList[i].item_data == null)
                        m_ItemList[i].item_data = new _ITEM();
                    m_ItemList[i].item_data.readFromBuff(ref buff);
                }
                else
                {
                    buff.ReadUint(ref m_ItemList[i].item_guid);
                }
            }
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BANKACQUIRELIST;
        }

        public override int getSize()
        {
            int length = 0;
            for (int i = 0; i < m_ItemNum; i++)
            {
                length += sizeof(byte) * 3;
                if (m_ItemList[i].isBlueEquip != 0)
                {
                    length += _ITEM.GetMaxSize();
                }
                else
                {
                    length += sizeof(uint);
                }

            }
            return sizeof(byte) * 2 + sizeof(int) * 2 + length;
        }

        public int Money
        {
            get { return this.m_Money; }
            set { m_Money = value; }
        }

        public int RMB
        {
            get { return this.m_RMB; }
            set { m_RMB = value; }
        }
        public byte CurBankSize
        {
            get { return m_CurBankSize; }
            set { m_CurBankSize = value; }
        }

        public byte ItemNum
        {
            get { return m_ItemNum; }
            set { m_ItemNum = value; }
        }
        public _BANK_ITEM[] ItemList
        {
            get { return m_ItemList; }
        }

        _BANK_ITEM[]			m_ItemList = new _BANK_ITEM[GAMEDEFINE.MAX_BANK_SIZE];
		byte					m_ItemNum;
		byte					m_CurBankSize;
		int						m_Money;
		int						m_RMB;
    };

    public class GCBankAcquireListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBankAcquireList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BANKACQUIRELIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 2 + sizeof(int) * 2 + GAMEDEFINE.MAX_BANK_SIZE * (sizeof(byte) * 3 + sizeof(uint) + _ITEM.GetMaxSize());
        }
    };
}