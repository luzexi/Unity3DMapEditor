using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CLAskCharLogin : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CL_ASKCHARLOGIN;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(short);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(charGuid);
            buff.WriteShort(playerID);

            //包内容, 10为包头偏移 [12/9/2011 ZL]
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }



        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref charGuid) != sizeof(int)) return false;
            if (buff.ReadShort(ref playerID) != sizeof(short)) return false;
            return true;
        }

        //public interface
        public int CharGuid{
            get{ return this.charGuid;}
            set{ charGuid = value; }
        }
        public short PlayerID{
            get{ return this.playerID; }
            set{ playerID = value; }
        }

        //member data
        private int  charGuid;
        private short playerID;
    }


    public class CLAskCharLoginFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CLAskCharLogin(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CL_ASKCHARLOGIN; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(short);
        }
    };
}