using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCAddCanPickMissionItem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ADDCANPICKMISSIONITEM;
        }

        public override int getSize()
        {
            return sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_uItemDataID) != sizeof(uint)) return false;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_uItemDataID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private uint m_uItemDataID;
        public uint ItemDataID
        {
            get { return m_uItemDataID; }
            set { m_uItemDataID = value; }
        }
    }

    public class GCAddCanPickMissionItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCAddCanPickMissionItem(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_ADDCANPICKMISSIONITEM; }
        public override int GetPacketMaxSize() { return sizeof(uint); }
    }
}