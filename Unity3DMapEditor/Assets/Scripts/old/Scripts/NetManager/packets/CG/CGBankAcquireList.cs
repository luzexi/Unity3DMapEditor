
using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CGBankAcquireList : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_BankID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_BANKACQUIRELIST;
        }
        public override int getSize()
        {
            return sizeof(byte);
        }

        public byte BankId
        {
            set { m_BankID = value; }
        }

        //数据
        byte m_BankID;		//成功与否
    };

    public class CGBankAcquireListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGBankAcquireList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_BANKACQUIRELIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte);
        }
    };
}