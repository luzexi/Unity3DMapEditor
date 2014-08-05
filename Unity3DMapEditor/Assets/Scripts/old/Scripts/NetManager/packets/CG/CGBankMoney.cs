
using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CGBankMoney : PacketBase
    {
 
        public enum OPtype
		{
			SAVE_MONEY		= 1,
			PUTOUT_MONEY	= 2,
			UPDATE_MONEY	= 4,
			SAVE_RMB		= 8,
			PUTOUT_RMB		= 16,
			UPDATA_RMB		= 32,
		};

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_Save);
            buff.WriteInt(m_AmountMoney);
            buff.WriteInt(m_AmountRMB);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_BANKMONEY;
        }
        public override int getSize()
        {
            return sizeof(byte)  + sizeof(int)*2;
        }

        public byte SaveType
        {
            set { m_Save = value; }
        }
        public int AmountMoney
        {
            set { m_AmountMoney = value; }
        }
        public int AmountRMB
        {
            set { m_AmountRMB = value; }
        }
        byte m_Save;
        int m_AmountMoney;
        int m_AmountRMB;
    };

    public class CGBankMoneyFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGBankMoney(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_BANKMONEY; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(int) * 2;
        }
    };
}