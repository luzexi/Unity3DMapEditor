using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCMissionModify : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONMODIFY;
        }

        public override int getSize()
        {
            if (m_nFlag == (int)MISSIONMODIFY.MISSIONMODIFY_MISSION)
            {
                return sizeof(int) + m_Mission.getSize();
            }
            else if (m_nFlag == (int)MISSIONMODIFY.MISSIONMODIFY_MISSIONDATA)
            {
                return sizeof(int) +
                        sizeof(int) * GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM;
            }
            return 0;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nFlag) != sizeof(int)) return false;

            if (m_nFlag == (int)MISSIONMODIFY.MISSIONMODIFY_MISSION)
            {
                if (!m_Mission.readFromBuff(ref buff))
                {
                    return false;
                }
            }
            else if (m_nFlag == (int)MISSIONMODIFY.MISSIONMODIFY_MISSIONDATA)
            {
                for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM; i++)
                {
                    if (buff.ReadInt(ref m_aMissionData[i]) != sizeof(int))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nFlag);

            if (m_nFlag == (int)MISSIONMODIFY.MISSIONMODIFY_MISSION)
            {
                m_Mission.writeToBuff(ref buff);
            }
            else if (m_nFlag == (int)MISSIONMODIFY.MISSIONMODIFY_MISSIONDATA)
            {
                for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM; i++)
                {
                    buff.WriteInt(m_aMissionData[i]);
                }
            }

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private int m_nFlag;
        public int Flag
        {
            get { return m_nFlag; }
            set { m_nFlag = value; }
        }

        private _OWN_MISSION m_Mission = new _OWN_MISSION();

        public void SetMission(_OWN_MISSION pMission) { m_Mission = pMission; }
        public _OWN_MISSION GetMission() { return m_Mission; }

        private int[] m_aMissionData = new int[GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM];

        public void SetMIssionData(int[] aMissionData) { m_aMissionData = aMissionData; }
        public int[] GetMissionData() { return m_aMissionData; }

        public enum MISSIONMODIFY
        {
            MISSIONMODIFY_MISSION = 0,//修改Mission
            MISSIONMODIFY_MISSIONDATA,//修改MissonData
        };
    }

    public class GCMissionModifyFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMissionModify(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_MISSIONMODIFY; }
        public override int GetPacketMaxSize() { return sizeof(int) + _OWN_MISSION.getMaxSize() + sizeof(int) * GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM; }
    }
}