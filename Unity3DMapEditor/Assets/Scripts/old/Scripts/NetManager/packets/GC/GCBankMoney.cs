
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCBankMoney : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_Save) != sizeof(byte)) return false;
            if (buff.ReadInt(ref m_Amount) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_AmountRMB) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BANKMONEY;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 + sizeof(byte);
        }

        public int Amount
        {
            get { return this.m_Amount; }
            set { m_Amount = value; }
        }
        public int AmountRMB
        {
            get { return this.m_AmountRMB; }
            set { m_AmountRMB = value; }
        }

        public byte SaveType
        {
            get { return this.m_Save; }
            set { m_Save = value; }
        }


        byte m_Save;
        int m_Amount;
        int m_AmountRMB;
    };

    public class GCBankMoneyFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBankMoney(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BANKMONEY; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int)*2 + sizeof(byte);
        }
    };
}