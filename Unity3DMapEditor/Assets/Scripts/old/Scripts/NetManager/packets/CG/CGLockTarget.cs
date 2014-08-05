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
    public class CGLockTarget : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_TargetID) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_TargetID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_LOCK_TARGET;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public int TargetID
        {
            get { return this.m_TargetID; }
            set { m_TargetID = value; }
        }
        private int m_TargetID;			// ObjID

    };

    public class CGLockTargetFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGLockTarget(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_LOCK_TARGET; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}