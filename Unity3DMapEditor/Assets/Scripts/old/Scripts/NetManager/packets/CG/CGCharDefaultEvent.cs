using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGCharDefaultEvent : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHARDEFAULTEVENT;
        }

        public override int getSize()
        {
            return sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        uint objId;
        public uint ObjId
        {
            get { return objId; }
            set { objId = value; }
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(objId);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class CGCharDefaultEventFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGCharDefaultEvent(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARDEFAULTEVENT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint);
        }
    }
}
