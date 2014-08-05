using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCMissionRemove : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONREMOVE;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_idMission) != sizeof(int))
            { 
                return false; 
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_idMission);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private int m_idMission;
        public int MissionID
        {
            get { return m_idMission; }
            set { m_idMission = value; }
        }
    }

    public class GCMissionRemoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMissionRemove(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_MISSIONREMOVE; }
        public override int GetPacketMaxSize() { return sizeof(int); }
    }
}