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
    public class GCCharDirectImpact : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nReceiverID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nSenderID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nSenderLogicCount) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_nImpactID) != sizeof(short)) return false;
            if (buff.ReadShort(ref m_nSkillID) != sizeof(short)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nReceiverID);
            buff.WriteInt(m_nSenderID);
            buff.WriteInt(m_nSenderLogicCount);
            buff.WriteShort(m_nImpactID);
            buff.WriteShort(m_nSkillID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHAR_DIRECT_IMPACT;
        }

        public override int getSize()
        {
            return sizeof(int) * 3 + sizeof(short) * 2;
        }

        public int SenderID
        {
            get { return this.m_nSenderID; }
            set { m_nSenderID = value; }
        }

        public int RecieverID
        {
            get { return this.m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        public int SenderLogicCount
        {
            get { return this.m_nSenderLogicCount; }
            set { m_nSenderLogicCount = value; }
        }

        public short ImpactID
        {
            get { return this.m_nImpactID; }
            set { m_nImpactID = value; }
        }

        public short SkillID
        {
            get { return this.m_nSkillID; }
            set { m_nSkillID = value; }
        }

        private int m_nReceiverID;		// 效果接受者的ID
        private int m_nSenderID;			// 效果施放者的ID
        private int m_nSenderLogicCount;		// 攻击者的逻辑计数
        private short m_nImpactID;			// 效果ID
        private short m_nSkillID;		// 技能的ID
    };

    public class GCCharDirectImpactFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharDirectImpact(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHAR_DIRECT_IMPACT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 3 + sizeof(short)*2;
        }
    };
}