using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskMissionList : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKMISSIONLIST;
        }

        public override int getSize()
        {
            return sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private uint m_ObjID;
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
    }

    public class CGAskMissionListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskMissionList(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_ASKMISSIONLIST; }
        public override int GetPacketMaxSize() { return sizeof(uint); }
    }
}