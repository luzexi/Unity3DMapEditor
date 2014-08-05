
using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskDetailAbilityInfo : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKDETAILABILITYINFO;
        }
        public override int getSize()
        {
            return sizeof(int);
        }

        //public interface
        public int ObjID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        private int m_ObjID;
    };
    public class CGAskDetailAbilityInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskDetailAbilityInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKDETAILABILITYINFO; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}