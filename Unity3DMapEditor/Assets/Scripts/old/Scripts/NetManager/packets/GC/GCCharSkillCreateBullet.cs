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
    public class GCCharSkillCreateBullet : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_SkillDataID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_TargetID) != sizeof(int)) return false;
            if (!m_posTarget.readFromBuff(ref buff)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_SkillDataID);
            buff.WriteInt(m_TargetID);
            m_posTarget.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_CREATEBULLET;
        }

        public override int getSize()
        {
            return sizeof(int) * 3 + m_posTarget.getSize();
        }

        public int ObjID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int SkillID
        {
            get { return this.m_SkillDataID; }
            set { m_SkillDataID = value; }
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


        private int         m_ObjID;			// ObjID
        private int         m_SkillDataID;		// 技能的资源ID
        //union{
        private int         m_TargetID;			// 目标角色
        private WORLD_POS   m_posTarget;		// 目标坐标
    };

    public class GCCharSkillCreateBulletFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharSkillCreateBullet(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_CREATEBULLET; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 3 + WORLD_POS.GetMaxSize();
        }
    };
}