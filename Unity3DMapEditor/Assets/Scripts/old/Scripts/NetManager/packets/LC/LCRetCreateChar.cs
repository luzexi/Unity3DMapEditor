using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class LCRetCreateChar : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_RETCREATECHAR;
        }

        public override int getSize()
        {
            return sizeof(NET_RESULT_DEFINE.ASKCREATECHAR_RESULT) ;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
 
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int temp = -1;
            if (buff.ReadInt(ref temp) != sizeof(int)) return false;
            result = (NET_RESULT_DEFINE.ASKCREATECHAR_RESULT)temp;
      
            return true;
        }

        //public interface
        public NET_RESULT_DEFINE.ASKCREATECHAR_RESULT Result
        {
            get { return this.result; }
            set { result = value; }
        }
        public byte[] Account
        {
            get { return this.szAccount; }
            set { szAccount = value; }
        }

  
        //数据
		private NET_RESULT_DEFINE.ASKCREATECHAR_RESULT		result;
		private byte[]						szAccount = new byte[NET_DEFINE.MAX_ACCOUNT+1] ;	//用户名称
    }


    public class LCRetCreateCharFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new LCRetCreateChar(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_RETCREATECHAR; }
        public override int GetPacketMaxSize()
        {
            return sizeof(NET_RESULT_DEFINE.ASKCREATECHAR_RESULT);
        }
    };
}