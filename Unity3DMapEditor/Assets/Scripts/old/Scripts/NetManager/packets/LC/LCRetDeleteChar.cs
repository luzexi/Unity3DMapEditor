using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class LCRetDeleteChar : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_RETDELETECHAR;
        }

        public override int getSize()
        {
            return sizeof(ASKDELETECHAR_RESULT);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            int temp = -1;
            if (buff.ReadInt(ref temp) != 4) return false;
            result = (ASKDELETECHAR_RESULT)temp;
            return true;
        }

        public ASKDELETECHAR_RESULT Result
        {
            get { return result; }
        }
        public byte[] Account
        {
            get { return szAccount; }
        }
        //数据
		private ASKDELETECHAR_RESULT		result;
		private byte[]						szAccount = new byte[NET_DEFINE.MAX_ACCOUNT+1];	//用户名称	
    }


    public class LCRetDeleteCharFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new LCRetDeleteChar(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_RETDELETECHAR; }
        public override int GetPacketMaxSize()
        {
            return  sizeof(ASKDELETECHAR_RESULT);
        }
    };
}