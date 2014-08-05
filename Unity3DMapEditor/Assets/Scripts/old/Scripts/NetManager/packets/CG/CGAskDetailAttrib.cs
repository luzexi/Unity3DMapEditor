using System;
using System.Collections.Generic;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskDetailAttrib : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKDETAILATTRIB; 
        }

        public override int getSize()
        {
            return sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        // all Data
        uint m_ObjID;	//对方的ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
    }

    class CGAskDetailAttribFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskDetailAttrib(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKDETAILATTRIB; }
        public override int GetPacketMaxSize() { return sizeof(uint); }
    }
}
