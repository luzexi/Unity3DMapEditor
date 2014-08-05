using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CLConnect : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CL_CONNECT;
        }

        public override int getSize()
        {
            return 0;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }

    }


    public class CLConnectFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CLConnect(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CL_CONNECT; }
        public override int GetPacketMaxSize()
        {
            return 0;
        }
    };
}