using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class LCRetConnect : PacketBase
    {

        private int Result;
        private byte[] LoginIP = new byte[NET_DEFINE.IP_SIZE];
        private int LoginPort;

        public int M_Result
        {
            get { return Result; }
            set { Result = value; }
        }
        public byte[] M_LoginIp
        {
            get { return LoginIP; }
            set { LoginIP = value; }
        }
        public int M_LoginPort
        {
            get { return LoginPort; }
            set { LoginPort = value; }
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_RETCONNECT;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(int) + NET_DEFINE.IP_SIZE;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            buff.WriteInt(Result);
            buff.WriteInt(LoginPort);
            buff.Write(ref LoginIP, NET_DEFINE.IP_SIZE);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref Result) != 4) return false;
            if (buff.ReadInt(ref LoginPort) != 4) return false;
            if (buff.Read(ref LoginIP, NET_DEFINE.IP_SIZE) != NET_DEFINE.IP_SIZE) return false;
            return true;
        }

    }


    public class LCRetConnectFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new LCRetConnect(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_RETCONNECT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(int) + NET_DEFINE.IP_SIZE;
        }
    };
}