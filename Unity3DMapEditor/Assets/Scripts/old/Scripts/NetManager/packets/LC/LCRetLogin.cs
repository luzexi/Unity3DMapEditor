using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class LCRetLogin : PacketBase
    {

        private byte[] szAccount = new byte[NET_DEFINE.MAX_ACCOUNT + 1];
        private NET_RESULT_DEFINE.LOGIN_RESULT Result;

        public NET_RESULT_DEFINE.LOGIN_RESULT M_Result
        {
            get { return Result; }
            set { Result = value; }
        }

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

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_RETLOGIN;
        }

        public override int getSize()
        {
            return NET_DEFINE.MAX_ACCOUNT + sizeof(int);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            buff.Write(ref szAccount, NET_DEFINE.MAX_ACCOUNT);
            buff.WriteInt((int)Result);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.Read(ref szAccount, NET_DEFINE.MAX_ACCOUNT) != NET_DEFINE.MAX_ACCOUNT) return false;
            int temp = -1;
            if (buff.ReadInt(ref temp) != 4) return false;
            Result = (NET_RESULT_DEFINE.LOGIN_RESULT)temp;
            return true;
        }

    }

    public class LCRetLoginFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new LCRetLogin(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_RETLOGIN; }
        public override int GetPacketMaxSize()
        {
            return NET_DEFINE.MAX_ACCOUNT + sizeof(int);
        }
    };
}