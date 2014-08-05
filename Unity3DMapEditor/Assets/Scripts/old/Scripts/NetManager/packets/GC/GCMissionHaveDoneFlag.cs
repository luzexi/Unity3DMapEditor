using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCMissionHaveDoneFlag : PacketBase
    {
        public bool Init(int nMissionID,int bHaveDoneFlag)
        {
            m_nMissionID = nMissionID;
            m_bHaveDone = bHaveDoneFlag;
            return true;
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONHAVEDONEFLAG;
        }

        public override int getSize()
        {
            return sizeof(int) * 2;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nMissionID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_bHaveDone) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nMissionID);
            buff.WriteInt(m_bHaveDone);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private int m_nMissionID;
        private int m_bHaveDone;
        public int MissionID
        {
            get { return m_nMissionID; }
            set { m_nMissionID = value; }
        }
        public int IsHaveDone
        {
            get { return m_bHaveDone; }
            set { m_bHaveDone = value; }
        }
    }

    public class GCMissionHaveDoneFlagFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMissionHaveDoneFlag(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_MISSIONHAVEDONEFLAG; }
        public override int GetPacketMaxSize() { return sizeof(int) * 2; }
    }
}