using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCMissionResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_IsFinished) != sizeof(byte))
            {
                return false;
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_IsFinished);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private byte m_IsFinished;
        public byte IsFinished
        {
            get { return m_IsFinished; }
            set { m_IsFinished = value; }
        }
    }

    public class GCMissionResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMissionResult(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_MISSIONRESULT; }
        public override int GetPacketMaxSize() { return sizeof(byte); }
    }
}