using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Network;
using Network.Handlers;
using UnityEngine;
namespace Network.Packets
{
    public class GCCharSkillLead : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nLogicCount) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_SkillDataID) != sizeof(short)) return false;
            if (!m_posUser.readFromBuff(ref buff)) return false;
            if (buff.ReadInt(ref m_TargetID) != sizeof(int)) return false;
            if (!m_posTarget.readFromBuff(ref buff)) return false;
            if (buff.ReadFloat(ref m_fDir) != sizeof(float)) return false;
            if (buff.ReadInt(ref m_nTotalTime) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_nLogicCount);
            buff.WriteShort(m_SkillDataID);
            m_posUser.writeToBuff(ref buff);
            buff.WriteInt(m_TargetID);
            m_posTarget.writeToBuff(ref buff);
            buff.WriteFloat(m_fDir);
            buff.WriteInt(m_nTotalTime);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_LEAD;
        }

        public override int getSize()
        {
            return sizeof(int) * 4 +
                m_posUser.getSize() +
                m_posTarget.getSize() +
                sizeof(float) + sizeof(short);
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int LogicCount
        {
            get { return this.m_nLogicCount; }
            set { m_nLogicCount = value; }
        }

        public short SkillDataID
        {
            get { return this.m_SkillDataID; }
            set { m_SkillDataID = value; }
        }

        public WORLD_POS PosUser
        {
            get { return this.m_posUser; }
            set { m_posUser = value; }
        }

        public int TargetID
        {
            get { return this.m_TargetID; }
            set { m_TargetID = value; }
        }

        public WORLD_POS PosTarget
        {
            get { return this.m_posTarget; }
            set { m_posTarget = value; }
        }

        public float Dir
        {
            get { return this.m_fDir; }
            set { m_fDir = value; }
        }

        public int TotalTime
        {
            get { return this.m_nTotalTime; }
            set { m_nTotalTime = value; }
        }


        private int       m_ObjID;			// ObjID
        private int       m_nLogicCount;		// 逻辑计数
        private short     m_SkillDataID;		// 技能的资源ID
        private WORLD_POS m_posUser;			// 使用都坐标

        private int       m_TargetID;			// 目标角色
        private WORLD_POS m_posTarget;		// 目标坐标
        private float     m_fDir;				// 技能的方向
        private int       m_nTotalTime;		// 总时间
    };

    public class GCCharSkillLeadFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharSkillSend(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_LEAD; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 4 + WORLD_POS.GetMaxSize() * 2 + sizeof(float) + sizeof(short);
        }
    };
}