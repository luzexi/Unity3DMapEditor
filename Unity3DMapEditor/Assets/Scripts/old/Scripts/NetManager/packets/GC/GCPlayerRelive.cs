using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCPlayerRelive : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_PLAYER_RELIVE;
        }

        public override int getSize()
        {
            return 0;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class GCPlayerReliveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCPlayerRelive(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_PLAYER_RELIVE; }
        public override int GetPacketMaxSize() { return 0; }
    }
}