using System;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskClearCDTime : PacketBase
    {
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteShort(m_idCD);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKCLEARCDTIME;
        }
        public override int getSize()
        {
            return sizeof(short);
        }

        //public interface
        public short CoolDownID
        {
            get { return this.m_idCD; }
            set { m_idCD = value; }
        }
        private short			m_idCD;
    };

    public class CGAskClearCDTimeFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskClearCDTime(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKCLEARCDTIME; }
        public override int GetPacketMaxSize()
        {
            return sizeof(short);
        }
    };
}