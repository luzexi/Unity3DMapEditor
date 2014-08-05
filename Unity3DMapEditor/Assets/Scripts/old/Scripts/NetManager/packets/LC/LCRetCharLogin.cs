using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class LCRetCharLogin : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_RETCHARLOGIN;
        }

        public override int getSize()
        {
            return sizeof(NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT) +
                sizeof(byte) * NET_DEFINE.IP_SIZE +
                sizeof(int) * 2;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt((int)result);
            buff.WriteInt(serverPort);
            buff.Write(ref serverIP, NET_DEFINE.IP_SIZE);
            buff.WriteInt(key);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int temp = -1;
            if (buff.ReadInt(ref temp) != sizeof(int)) return false;
            result = (NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT)temp;
            if (buff.ReadInt(ref serverPort) != sizeof(int)) return false;
            if (buff.Read(ref serverIP, NET_DEFINE.IP_SIZE) != NET_DEFINE.IP_SIZE) return false;
            if (buff.ReadInt(ref key) != sizeof(int)) return false;

            return true;
        }

        //public interface
        public NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT Result{
            get{ return this.result; }
            set{ result = value;     }
        }
        public byte[] ServerIP{
            get { return this.serverIP; }
            set { serverIP = value; }
        }
        public int ServerPort{
            get { return this.serverPort; }
            set { serverPort = value; }
        }
        public int Key{
            get { return this.key; }
            set { key = value; }
        }

        //数据
        private NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT		result;	
		private byte[]			    serverIP = new byte[NET_DEFINE.IP_SIZE];
		private int					serverPort;
		private int					key;
    }


    public class LCRetCharLoginFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new LCRetCharLogin(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_RETCHARLOGIN; }
        public override int GetPacketMaxSize()
        {
            return sizeof(NET_RESULT_DEFINE.ASKCHARLOGIN_RESULT) +
                 sizeof(byte) * NET_DEFINE.IP_SIZE +
                 sizeof(int) * 2;
        }
    };
}