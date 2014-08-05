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
    public class GCCharModifyAction : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_logicCount) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nModifyTime) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_logicCount);
            buff.WriteInt(m_nModifyTime);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARMODIFYACTION;
        }

        public override int getSize()
        {
            return sizeof(int) * 3;
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int LogicCount
        {
            get { return this.m_logicCount; }
            set { m_logicCount = value; }
        }

        public int ModifyTime
        {
            get { return this.m_nModifyTime; }
            set { m_nModifyTime = value; }
        }


        private int m_ObjID;			// ObjID
        private int m_logicCount;
        private int m_nModifyTime;	//坐标
    };

    public class GCCharModifyActionFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharModifyAction(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARMODIFYACTION; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 3;
        }
    };
}