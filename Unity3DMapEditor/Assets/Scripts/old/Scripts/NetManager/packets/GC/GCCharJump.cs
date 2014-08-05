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
    public class GCCharJump : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARJUMP;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        int m_ObjID;		
    };

    public class GCCharJumpFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharJump(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARJUMP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}