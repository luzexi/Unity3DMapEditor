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
    public class GCObjTeleport : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (!m_posWorld.readFromBuff(ref buff)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_OBJ_TELEPORT;
        }

        public override int getSize()
        {
            return sizeof(int) + m_posWorld.getSize();
        }

        public int ObjID
        {
            get { return this.m_ObjID; }
        }

        public WORLD_POS PosWorld
        {
            get { return this.m_posWorld; }
        }

        int       m_ObjID;	//ID
        WORLD_POS m_posWorld;	//坐标
    };

    public class GCObjTeleportFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCObjTeleport(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_OBJ_TELEPORT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + WORLD_POS.GetMaxSize();
        }
    };
}