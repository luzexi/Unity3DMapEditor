
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCBankBegin : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_BankID) != sizeof(byte)) return false;
            if (buff.ReadUint(ref m_NPCID) != sizeof(uint)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BANKBEGIN;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(byte);
        }

        public uint NPCID
        {
            get { return this.m_NPCID; }
            set { m_NPCID = value; }
        }

        public byte BankID
        {
            get { return this.m_BankID; }
            set { m_BankID = value; }
        }


        byte m_BankID;		//成功与否
        uint m_NPCID;
    };

    public class GCBankBeginFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBankBegin(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BANKBEGIN; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(byte);
        }
    };
}