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
    public class GCCooldownUpdate : PacketBase
    {
        public GCCooldownUpdate()
		{
            for (int nIdx = 0; (int)CHAR_ATTR_CONSTANT1.MAX_COOLDOWN_LIST_SIZE_FOR_HUMAN > nIdx; ++nIdx)
            {
                if(m_aCooldowns[nIdx] == null)
				{
					m_aCooldowns[nIdx] = new Cooldown_T();
				}
            }
		}
		
		public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (!m_guidPet.readFromBuff(ref buff)) return false;
            if (buff.ReadShort(ref m_nNumCooldown) != sizeof(short)) return false;
            for (int nIdx = 0; m_nNumCooldown > nIdx; ++nIdx)
            {
                if (buff.ReadShort(ref m_aCooldowns[nIdx].m_nID) != sizeof(short)) return false;
                if (buff.ReadInt(ref m_aCooldowns[nIdx].m_nCooldown) != sizeof(int)) return false;
                if (buff.ReadInt(ref m_aCooldowns[nIdx].m_nCooldownElapsed) != sizeof(int)) return false;
            }
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_COOLDOWN_UPDATE;
        }

        public override int getSize()
        {
            CalcNumCooldown();
            return m_guidPet.getSize() + sizeof(short) +
                m_aCooldowns[0].getSize()*m_nNumCooldown;
        }

        public PET_GUID_t GuidPet
        {
            get { return this.m_guidPet; }
        }

        public short numCoolDown
        {
            get { return this.m_nNumCooldown; }
        }

        public Cooldown_T[] CoolDowns
        {
            get { return m_aCooldowns; }
        }
        private void CalcNumCooldown()
        {
            m_nNumCooldown = 0;
            for (int nIdx = 0; (int)CHAR_ATTR_CONSTANT1.MAX_COOLDOWN_LIST_SIZE_FOR_HUMAN > nIdx; ++nIdx)
            {
                if(m_aCooldowns[nIdx] == null)
				{
					m_aCooldowns[nIdx] = new Cooldown_T();
				}
				
                if (-1 != m_aCooldowns[nIdx].m_nID)
                {
                    ++m_nNumCooldown;
                }
            }
        }
        private PET_GUID_t	m_guidPet;
	    private short		m_nNumCooldown;
        private Cooldown_T[] m_aCooldowns = new Cooldown_T[(int)CHAR_ATTR_CONSTANT1.MAX_COOLDOWN_LIST_SIZE_FOR_HUMAN];
    };

    public class GCCooldownUpdateFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCooldownUpdate(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_COOLDOWN_UPDATE; }
        public override int GetPacketMaxSize()
        {
            return PET_GUID_t.getMaxSize() + Cooldown_T.getMaxSize() * (int)CHAR_ATTR_CONSTANT1.MAX_COOLDOWN_LIST_SIZE_FOR_HUMAN + sizeof(short);
        }
    };
}