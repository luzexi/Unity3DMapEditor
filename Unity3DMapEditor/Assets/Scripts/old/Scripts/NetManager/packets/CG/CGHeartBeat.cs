
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets {
	

public class CGHeartBeat :  PacketBase 
{

	//公用继承接口
		public override bool	readFromBuff( ref NetInputBuffer buff )
		{
            return true;
		}
		public override int writeToBuff( ref NetOutputBuffer buff ) {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
		}

		public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_HEARTBEAT;
        }
        public override int getSize() { return 0; }
			
};
	 public class CGHeartBeatFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGHeartBeat(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_HEARTBEAT; }
        public override int GetPacketMaxSize()
        {
            return 0;
        }
    };
}