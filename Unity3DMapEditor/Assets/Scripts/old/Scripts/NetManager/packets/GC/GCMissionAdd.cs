using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCMissionAdd : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONADD;
        }

        public override int getSize()
        {
            return m_Mission.getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return m_Mission.readFromBuff(ref buff);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + m_Mission.writeToBuff(ref buff);
        }

        private _OWN_MISSION m_Mission = new _OWN_MISSION();
        public void SetMission(_OWN_MISSION Mission) { m_Mission = Mission; }
        public _OWN_MISSION GetMission() { return m_Mission; }
    }

    public class GCMissionAddFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMissionAdd(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_MISSIONADD; }
        public override int GetPacketMaxSize() { return _OWN_MISSION.getMaxSize(); }
    }
}