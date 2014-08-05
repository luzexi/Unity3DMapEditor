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
    public class GCCharSkillGatherModify : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nSubTime) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_nSubTime);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_GATHER_MODIFY;
        }

        public override int getSize()
        {
            return sizeof(int) * 2;
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int SubTime
        {
            get { return this.m_nSubTime; }
            set { m_nSubTime = value; }
        }


        private int m_ObjID;			// ObjID
        private int m_nSubTime;			// 缩短的引导时间
    };

    public class GCCharSkillGatherModifyFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharSkillGatherModify(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_GATHER_MODIFY; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2;
        }
    };
}