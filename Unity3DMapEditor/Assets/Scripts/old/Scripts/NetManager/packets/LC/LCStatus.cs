using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class LCStatus : PacketBase
    {

        private short TurnNumber;
        private NET_RESULT_DEFINE.CLIENT_TURN_STATUS ClientStatus;

        public short M_TurnNumber
        {
            get { return TurnNumber; }
            set { TurnNumber = value; }
        }
        public NET_RESULT_DEFINE.CLIENT_TURN_STATUS M_ClientStatus
        {
            get { return ClientStatus; }
            set { ClientStatus = value; }
        }
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_STATUS;
        }

        public override int getSize()
        {
            return sizeof(short) + sizeof(int);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            buff.WriteShort(TurnNumber);
            buff.WriteInt((int)ClientStatus);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref TurnNumber) != 2) return false;
            int temp = -1;
            if (buff.ReadInt(ref temp) != 4) return false;
            ClientStatus = (NET_RESULT_DEFINE.CLIENT_TURN_STATUS)temp;
            return true;
        }

    }


    public class LCStatusFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new LCStatus(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_STATUS; }
        public override int GetPacketMaxSize()
        {
            return sizeof(short) + sizeof(int);
        }
    };
}