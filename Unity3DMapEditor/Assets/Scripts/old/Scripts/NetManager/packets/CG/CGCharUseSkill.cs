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
    public class CGCharUseSkill : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_SkillDataID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_guidTarget) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_TargetID) != sizeof(int)) return false;
            if (!m_posTarget.readFromBuff(ref buff)) return false;
            if (buff.ReadFloat(ref m_fDir) != sizeof(float)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_SkillDataID);
            buff.WriteInt(m_guidTarget);
            buff.WriteInt(m_TargetID);
            m_posTarget.writeToBuff(ref buff);
            buff.WriteFloat(m_fDir);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHARUSESKILL;
        }

        public override int getSize()
        {
            return sizeof(int) * 4 + m_posTarget.getSize() + sizeof(float);
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int SkillDataID
        {
            get { return this.m_SkillDataID; }
            set { m_SkillDataID = value; }
        }

        public int GuidTarget
        {
            get { return this.m_guidTarget; }
            set { m_guidTarget = value; }
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

        private int m_ObjID;			// ObjID
        private int m_SkillDataID;		// 技能的资源ID
        //union{
        private int m_guidTarget;		// 目标角色的GUID
        private int m_TargetID;			// 目标角色
        private WORLD_POS m_posTarget;		// 目标坐标
        private float m_fDir;				// 技能的方向
    };

    public class CGCharUseSkillFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGCharUseSkill(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARUSESKILL; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 4 + WORLD_POS.GetMaxSize() + sizeof(float);
        }
    };
}