using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGMissionTrace : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONTRACE;
        }

        public override int getSize()
        {
            return sizeof(byte) + sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_index) != sizeof(byte)) return false;
            if (buff.ReadInt(ref m_bSet) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_index);
            buff.WriteInt(m_bSet);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private byte m_index;// 任务在任务数组中的索引
        private int m_bSet;// 任务追踪状态, 0为不追踪，1为追踪

        public byte Index
        {
            get { return m_index; }
            set { m_index = value; }
        }
        private int TraceSet
        {
            get { return m_bSet; }
            set { m_bSet = value; }
        }
    }

    public class CGMissionTraceFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGMissionTrace(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONTRACE; }
        public override int GetPacketMaxSize() { return sizeof(int); }
    }
}