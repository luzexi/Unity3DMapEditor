using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCRemoveCanPickMissionItem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_REMOVECANPICKMISSIONITEM;
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

    public class GCRemoveCanPickMissionItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRemoveCanPickMissionItem(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_REMOVECANPICKMISSIONITEM; }
        public override int GetPacketMaxSize() { return sizeof(uint); }
    }
}