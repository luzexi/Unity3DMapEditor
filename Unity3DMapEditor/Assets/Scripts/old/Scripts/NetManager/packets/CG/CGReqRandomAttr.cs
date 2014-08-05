using System;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public enum RANDOM_RANK
    {
        RANK_1, //普通培养
        RANK_2, //黄金培养
        RANK_3, //钻石培养
        RANK_4, //至尊培养
    }
    public class CGReqRandomAttr : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_REQRANDOMATTR;
        }

        public override int getSize()
        {
            return sizeof(byte)*2;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_Index);
            buff.WriteSByte(m_nRandomLevel);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        private byte m_Index; //0：表示自己，其他表示宠物
        private sbyte m_nRandomLevel;// enum ASK_TYPE
        public sbyte Level
        {
            get { return m_nRandomLevel; }
            set { m_nRandomLevel = value; }
        }

        public byte Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }
    }

    public class CGReqRandomAttrFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGReqRandomAttr(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_REQRANDOMATTR; }
        public override int GetPacketMaxSize() { return sizeof(byte)*2; }
    }
}