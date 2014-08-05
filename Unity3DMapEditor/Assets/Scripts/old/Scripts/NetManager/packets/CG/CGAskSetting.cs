
using Network;


namespace Network.Packets
{
    public class CGAskSetting : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKSETTING;
        }
        public override int getSize()
        {
            return 0;
        }


    };

    public class CGAskSettingFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskSetting(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKSETTING; }
        public override int GetPacketMaxSize()
        {
            return 0;
        }
    };
}