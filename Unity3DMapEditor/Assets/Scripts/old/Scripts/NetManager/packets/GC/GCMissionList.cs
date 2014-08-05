using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCMissionList : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONLIST;
        }

        public override int getSize()
        {
            return sizeof(uint) * 2 + m_listMission.getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadUint(ref m_uMissionListFlags) != sizeof(uint)) return false;
            if (!m_listMission.readFromBuff(ref buff)) return false;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            buff.WriteUint(m_uMissionListFlags);
            m_listMission.writeToBuff(ref buff);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private uint                m_ObjID;
        private uint                m_uMissionListFlags;		// 1位表示1条要刷新的
        private _MISSION_DB_LOAD m_listMission = new _MISSION_DB_LOAD();

        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        public uint MissionListFlags
        {
            get { return m_uMissionListFlags; }
        }

        public void AddMission( byte yIndex, _OWN_MISSION pMission )
        {
            if (yIndex < GAMEDEFINE.MAX_CHAR_MISSION_NUM)
            {
                m_uMissionListFlags |= (uint)(0x00000001 << yIndex);
                m_listMission.m_aMission[yIndex] = pMission;
            }
		}

        public _OWN_MISSION GetMission(byte yIndex)
        {
            if (yIndex < GAMEDEFINE.MAX_CHAR_MISSION_NUM)
            {
                return m_listMission.m_aMission[yIndex];
            }
            else
            {
                return null;
            }
        }

        public _OWN_MISSION[] GetMissionBuf()
        {
            return m_listMission.m_aMission;
        }

        public void SetMissionHaveDone(uint[] pHaveDone)
        {
            m_listMission.m_aMissionHaveDoneFlags = pHaveDone;
        }

        public uint[] GetMissionHaveDone()
        {
            return m_listMission.m_aMissionHaveDoneFlags;
        }

        public void SetMissionList(_MISSION_DB_LOAD pList)
        {
            m_listMission = pList;
        }

        public _MISSION_DB_LOAD GetMissionList()
        {
            return m_listMission;
        }
    }

    public class GCMissionListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMissionList(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_MISSIONLIST; }
        public override int GetPacketMaxSize() { return sizeof(uint) * 2 + _MISSION_DB_LOAD.GetMaxSize(); }
    }
}