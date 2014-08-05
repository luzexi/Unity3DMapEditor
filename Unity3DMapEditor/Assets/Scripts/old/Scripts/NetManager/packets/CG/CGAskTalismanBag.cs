using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskTalismanBag : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKTALISMANBAG;
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

    public class CGAskTalismanBagFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskTalismanBag(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_ASKTALISMANBAG; }
        public override int GetPacketMaxSize() { return 0; }
    }
}