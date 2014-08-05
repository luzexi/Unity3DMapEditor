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
    public class GCDetailSkillList : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadShort(ref m_wNumSkill) != sizeof(short)) return false;

            for (short i = 0; i < m_wNumSkill; i++)
            {
                if (!m_aSkill[i].readFromBuff(ref buff))
                    return false;   
            }
			for (short i = 0; i < m_wNumSkill; i++)
            {
				if (buff.ReadByte(ref m_aSkillLevel[i]) != sizeof(byte)) return false;
			}
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            buff.WriteShort(m_wNumSkill);

            for (short i = 0; i < m_wNumSkill; i++)
            {
                m_aSkill[i].writeToBuff(ref buff);   
            }
			
			for (short i = 0; i < m_wNumSkill; i++)
			{
				buff.WriteByte(m_aSkillLevel[i]);
			}
			
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAILSKILLLIST;
        }

        public override int getSize()
        {
            return sizeof(uint) + 
                   sizeof(short) + 
                   _OWN_SKILL.GetMaxSize() * m_wNumSkill +
                   sizeof(byte) * m_wNumSkill;
        }

        public int ObjectID
        {
            get { return (int)this.m_ObjID; }
            set { m_ObjID = (uint)value; }
        }   
        public short numSkill
        {
            get { return m_wNumSkill; }
            set { m_wNumSkill = value; }
        }

        public _OWN_SKILL[] Skill
        {
            get { return m_aSkill; }
        }
        public byte[] SkillLevel
        {
            get { return m_aSkillLevel; }
        }

        uint            m_ObjID;	// 所有Obj类型的ObjID
	    short		    m_wNumSkill;
        _OWN_SKILL[]    m_aSkill = new _OWN_SKILL[GAMEDEFINE.MAX_CHAR_SKILL_NUM];
        byte[] m_aSkillLevel     = new byte[GAMEDEFINE.MAX_CHAR_SKILL_NUM];
    };

    public class GCDetailSkillListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailSkillList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAILSKILLLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(short) +
                   _OWN_SKILL.GetMaxSize() * GAMEDEFINE.MAX_CHAR_SKILL_NUM +
                   sizeof(byte) * GAMEDEFINE.MAX_CHAR_SKILL_NUM;
        }
    };
}