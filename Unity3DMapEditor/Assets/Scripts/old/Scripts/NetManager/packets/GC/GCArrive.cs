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
    public class GCArrive : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nHandleID) != sizeof(int)) return false;
            if (!m_posWorld.readFromBuff(ref buff)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_nHandleID);
            m_posWorld.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ARRIVE;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 +
                m_posWorld.getSize();
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public WORLD_POS PosWorld
        {
            get { return this.m_posWorld; }
            set { m_posWorld = value; }
        }

        public int HandleID
        {
            get { return this.m_nHandleID; }
            set { m_nHandleID = value; }
        }


        private int       m_ObjID;			// ObjID
        private int       m_nHandleID;		
        private WORLD_POS m_posWorld;	//坐标
    };

    public class GCArriveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCArrive(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ARRIVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2 + WORLD_POS.GetMaxSize();
        }
    };
}