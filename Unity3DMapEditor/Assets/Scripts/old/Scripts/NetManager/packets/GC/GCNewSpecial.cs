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
    public class GCNewSpecial : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (!m_posWorld.readFromBuff(ref buff)) return false;
            if (buff.ReadFloat(ref m_Dir) != sizeof(float)) return false;
            if (buff.ReadInt(ref m_nDataID) != sizeof(int)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            m_posWorld.writeToBuff(ref buff);
            buff.WriteFloat(m_Dir);
            buff.WriteInt(m_nDataID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWSPECIAL;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 + m_posWorld.getSize() + sizeof(float) ;
        }

        public int ObjID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public WORLD_POS posWorld
        {
            get { return this.m_posWorld; }
            set { m_posWorld = value; }
        }

        public float Dir
        {
            get { return m_Dir; }
            set { m_Dir = value; }
        }

        public int DataID
        {
            get { return m_nDataID; }
            set { m_nDataID = value; }
        }

        private int         m_ObjID;		// ObjID
        private WORLD_POS   m_posWorld;		// 位置
        private float       m_Dir;			// 方向
        private int         m_nDataID;
    };

    public class GCNewSpecialFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewSpecial(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWSPECIAL; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2 + WORLD_POS.GetMaxSize() + sizeof(float);
        }
    };
}