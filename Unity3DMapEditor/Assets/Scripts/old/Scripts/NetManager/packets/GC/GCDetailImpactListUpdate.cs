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
    public class GCDetailImpactListUpdate : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nOwnerID) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_nNumOfImpacts) != sizeof(short)) return false;
            for (short i = 0; i < m_nNumOfImpacts; i++)
            {
                if (!m_aImpact[i].readFromBuff(ref buff)) return false;
            }
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nOwnerID);
            buff.WriteShort(m_nNumOfImpacts);
            for (short i = 0; i < m_nNumOfImpacts; i++)
            {
                m_aImpact[i].writeToBuff(ref buff);
            }
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAIL_IMPACT_LIST_UPDATE;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(short) + DetailImpactStruct_T.GetMaxSize() * m_nNumOfImpacts;
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

        public DetailImpactStruct_T[] Impacts
        {
            get { return this.m_aImpact; }
            set { m_aImpact = value; }
        }

        private int			m_nOwnerID;
		private short		m_nNumOfImpacts;
        private DetailImpactStruct_T[] m_aImpact = new DetailImpactStruct_T[GAMEDEFINE.MAX_IMPACT_NUM];
    };

    public class GCDetailImpactListUpdateFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailImpactListUpdate(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAIL_IMPACT_LIST_UPDATE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(short) + DetailImpactStruct_T.GetMaxSize() * GAMEDEFINE.MAX_IMPACT_NUM;
        }
    };
}