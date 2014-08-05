using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

public enum ENUM_DIE_RESULT_CODE
{
    DIE_RESULT_CODE_INVALID = -1,
    DIE_RESULT_CODE_OUT_GHOST,		// 释放灵魂
    DIE_RESULT_CODE_RELIVE,			// 接受复活
    DIE_RESULT_CODE_NUMBERS
};

namespace Network.Packets
{
    public class CGPlayerDieResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_PLAYER_DIE_RESULT;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int enumindex = (int)dieResultCode;
            if (buff.ReadInt(ref enumindex) != sizeof(int)) return false;
            dieResultCode = (ENUM_DIE_RESULT_CODE)enumindex;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            int enumindex = (int)dieResultCode;
            buff.WriteInt(enumindex);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private ENUM_DIE_RESULT_CODE dieResultCode;
        public ENUM_DIE_RESULT_CODE DieResultCode
        {
            get { return dieResultCode; }
            set { dieResultCode = value; }
        }

        public CGPlayerDieResult()
        {
            DieResultCode = ENUM_DIE_RESULT_CODE.DIE_RESULT_CODE_INVALID;
        }
    }

    class CGPlayerDieResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGPlayerDieResult(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_PLAYER_DIE_RESULT; }
        public override int GetPacketMaxSize() { return sizeof(int); }
    }
}