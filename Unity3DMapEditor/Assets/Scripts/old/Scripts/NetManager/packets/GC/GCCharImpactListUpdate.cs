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
    public class GCCharImpactListUpdate : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nOwnerID) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_nNumOfImpacts) != sizeof(short)) return false;
            for (short i = 0; i < m_nNumOfImpacts; i++)
            {
                if (buff.ReadShort(ref m_aImpactID[i]) != sizeof(short)) return false;
            }
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nOwnerID);
            buff.WriteShort(m_nNumOfImpacts);
            for (short i = 0; i < m_nNumOfImpacts; i++)
            {
                buff.WriteShort(m_aImpactID[i]);
            }
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHAR_IMPACT_LIST_UPDATE;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(short) * (m_nNumOfImpacts+1);
        }

        public int OwnerID
        {
            get { return this.m_nOwnerID; }
            set { m_nOwnerID = value; }
        }

        public short NumImpact
        {
            get { return this.m_nNumOfImpacts; }
            set { m_nNumOfImpacts = value; }
        }

        public short[] ImpactID
        {
            get { return this.m_aImpactID; }
            set { m_aImpactID = value; }
        }

        private int m_nOwnerID;		// 效果接受者的ID
        private short m_nNumOfImpacts;			// 效果ID
        private short[] m_aImpactID = new short[GAMEDEFINE.MAX_IMPACT_NUM];		// 技能的ID
    };

    public class GCCharImpactListUpdateFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharImpactListUpdate(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHAR_IMPACT_LIST_UPDATE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(short) * (GAMEDEFINE.MAX_IMPACT_NUM + 1);
        }
    };
}