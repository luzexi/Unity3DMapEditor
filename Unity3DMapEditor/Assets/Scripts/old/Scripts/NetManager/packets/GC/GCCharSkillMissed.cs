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
    public class GCCharSkillMissed : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nReceiverID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nSenderID) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_nSkillID) != sizeof(short)) return false;
            if (buff.ReadShort(ref m_nFlag) != sizeof(short)) return false;
            if (buff.ReadInt(ref m_nSenderLogicCount) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nReceiverID);
            buff.WriteInt(m_nSenderID);
            buff.WriteShort(m_nSkillID);
            buff.WriteShort(m_nFlag);
            buff.WriteInt(m_nSenderLogicCount);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_MISSED;
        }

        public override int getSize()
        {
            return sizeof(int) * 4;
        }

        public int ReceiverID
        {
            get { return this.m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        public int SenderID
        {
            get { return this.m_nSenderID; }
            set { m_nSenderID = value; }
        }

        public short Flag
        {
            get { return this.m_nFlag; }
            set { m_nFlag = value; }
        }

        public int SenderLogicCount
        {
            get { return this.m_nSenderLogicCount; }
            set { m_nSenderLogicCount = value; }
        }
		
		public short SkillID
        {
            get { return this.m_nSkillID; }
            set { m_nSkillID = value; }
        }


        private int m_nReceiverID;	// 效果接受者的ID
        private int m_nSenderID;	// 效果释放者的ID
        private short m_nSkillID;		// 技能的ID
        private short m_nFlag;		// 未击中，免疫，吸收，转移的标记
        private int m_nSenderLogicCount;
    };

    public class GCCharSkillMissedFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharSkillMissed(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_MISSED; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 4;
        }
    };
}