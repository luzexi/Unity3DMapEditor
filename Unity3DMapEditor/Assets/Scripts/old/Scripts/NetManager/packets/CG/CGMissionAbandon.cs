using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGMissionAbandon : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONABANDON;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_idMissionScript) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_idMissionScript);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private int m_idMissionScript;
        public int MissionScriptID
        {
            get { return m_idMissionScript; }
            set { m_idMissionScript = value; }
        }
    }

    public class CGMissionAbandonFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGMissionAbandon(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONABANDON; }
        public override int GetPacketMaxSize() { return sizeof(int); }
    }
}