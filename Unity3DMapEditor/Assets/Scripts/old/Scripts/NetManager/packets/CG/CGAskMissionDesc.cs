using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskMissionDesc : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONDESC;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nMissionIndex) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nMissionIndex);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private int m_nMissionIndex;
        public int MissionIndex
        {
            get { return m_nMissionIndex; }
            set { m_nMissionIndex = value; }
        }
    }

    public class CGAskMissionDescFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskMissionDesc(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONDESC; }
        public override int GetPacketMaxSize() { return sizeof(int); }
    }
}