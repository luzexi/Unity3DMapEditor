using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CLAskLogin : PacketBase
    {

        private byte[] szAccount = new byte[NET_DEFINE.MAX_ACCOUNT + 1];
        private byte[] szPassWord = new byte[NET_DEFINE.MAX_PASSWORD + 1];
        private int uVersion;

        public byte[] SzAccount
        {
            get
            {
                return this.szAccount;
            }
            set
            {
                szAccount = value;
            }
        }

        public byte[] SzPassWord
        {
            get
            {
                return this.szPassWord;
            }
            set
            {
                szPassWord = value;
            }
        }

        public int UVersion
        {
            get
            {
                return this.uVersion;
            }
            set
            {
                uVersion = value;
            }
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CL_ASKLOGIN;
        }

        public override int getSize()
        {
            return NET_DEFINE.MAX_ACCOUNT + NET_DEFINE.MAX_PASSWORD + sizeof(int);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移 [12/9/2011 ZL]
            buff.Write(ref szAccount, NET_DEFINE.MAX_ACCOUNT);
            buff.Write(ref szPassWord, NET_DEFINE.MAX_PASSWORD);
            buff.WriteInt(uVersion);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.Read(ref szAccount, NET_DEFINE.MAX_ACCOUNT) != NET_DEFINE.MAX_ACCOUNT) return false;
            if (buff.Read(ref szPassWord, NET_DEFINE.MAX_PASSWORD) != NET_DEFINE.MAX_ACCOUNT) return false;
            if (buff.ReadInt(ref uVersion) != 4) return false;
            return true;
        }

    }


    public class CLAskLoginFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CLAskLogin(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CL_ASKLOGIN; }
        public override int GetPacketMaxSize()
        {
            return NET_DEFINE.MAX_ACCOUNT + NET_DEFINE.MAX_PASSWORD + sizeof(int);
        }
    };
}