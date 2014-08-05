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
    public class GCSpecialObjFadeOut : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nObjID) != sizeof(int)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nObjID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SPECIAL_OBJ_FADE_OUT;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        public int ObjID
        {
            get { return this.m_nObjID; }
            set { m_nObjID = value; }
        }

        private int m_nObjID;	// 特殊对象的ID
    };

    public class GCSpecialObjFadeOutFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCSpecialObjFadeOut(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SPECIAL_OBJ_FADE_OUT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}