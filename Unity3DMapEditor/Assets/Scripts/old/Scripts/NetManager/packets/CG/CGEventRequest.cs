using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGEventRequest : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_EVENTREQUEST;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(int) * 2;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        uint m_idNPC;
        public uint NPC
        {
            get { return m_idNPC; }
            set { m_idNPC = value; }
        }
        int m_idScript;
        public int Script
        {
            get { return m_idScript; }
            set { m_idScript = value; }
        }
        int m_idExIndex;
        public int ExIndex
        {
            get { return m_idExIndex; }
            set { m_idExIndex = value; }
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_idNPC);
            buff.WriteInt(m_idScript);
            buff.WriteInt(m_idExIndex);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class CGEventRequestFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGEventRequest(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_EVENTREQUEST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(int) * 2;
        }
    }
}
