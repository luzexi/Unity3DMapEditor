
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets {
	

public class CGAskFlushCharmInfo:  PacketBase 
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
            return (short)PACKET_DEFINE.PACKET_CG_ASK_FLUSH_CHARM_INFO;
        }
        public override int getSize() { return 0; }
			
};
	 public class CGAskFlushCharmInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskFlushCharmInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASK_FLUSH_CHARM_INFO; }
        public override int GetPacketMaxSize()
        {
            return 0;
        }
    };
}