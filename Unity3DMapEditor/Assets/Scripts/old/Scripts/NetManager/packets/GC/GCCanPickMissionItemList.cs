using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCCanPickMissionItemList : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CANPICKMISSIONITEMLIST;
        }

        public override int getSize()
        {
            return sizeof(byte) + sizeof(uint) * m_yItemCount;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_yItemCount) != sizeof(byte)) return false;
            for (int i = 0; i < m_yItemCount; i++)
            {
                if (buff.ReadUint(ref m_aCanPickMissionItemList[i]) != sizeof(uint)) return false;
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_yItemCount);
            for (int i = 0; i < m_yItemCount; i++)
            {
                buff.WriteUint(m_aCanPickMissionItemList[i]);
            }
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private byte    m_yItemCount;
		private uint[]  m_aCanPickMissionItemList = new uint[GAMEDEFINE.MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM];

        public byte ItemCount
        {
            get { return m_yItemCount; }
            set { m_yItemCount = value; }
        }
        public uint[] CanPickMissionItemList
        {
            get { return m_aCanPickMissionItemList; }
            set { m_aCanPickMissionItemList = value; }
        }
    }

    public class GCCanPickMissionItemListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCanPickMissionItemList(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_CANPICKMISSIONITEMLIST; }
        public override int GetPacketMaxSize() { return sizeof(byte) + sizeof(uint) * GAMEDEFINE.MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM; }
    }
}