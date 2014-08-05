
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGDispelBuffReq : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_nSkillID) != sizeof(short)) return false;
            if (buff.ReadShort(ref m_nImpactID) != sizeof(short)) return false;
            if (buff.ReadInt(ref m_nSN) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteShort(m_nSkillID);
            buff.WriteShort(m_nImpactID);
            buff.WriteInt(m_nSN);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_DISPEL_BUFF;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(short)*2;
        }

        //public interface
        public int SN
        {
            get { return this.m_nSN; }
            set { m_nSN = value; }
        }

        private short m_nSkillID;
        private short m_nImpactID;
        private int   m_nSN;
    };

    public class CGDispelBuffReqFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGDispelBuffReq(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_DISPEL_BUFF; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(short) * 2;
        }
    };
}