using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCPlayerDie : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_PLAYER_DIE;
        }

        public override int getSize()
        {
            return sizeof(int) * 2;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_bCanRelive) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_uTime) != sizeof(int)) return false;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_bCanRelive);
            buff.WriteInt(m_uTime);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private int m_bCanRelive;
        public int IfCanRelive
        {
            get { return m_bCanRelive; }
            set { m_bCanRelive = value; }
        }

        private int m_uTime;
        public int ReliveTime
        {
            get { return m_uTime; }
            set { m_uTime = value; }
        }
    }

    public class GCPlayerDieFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCPlayerDie(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_PLAYER_DIE; }
        public override int GetPacketMaxSize() { return sizeof(uint) * 2; }
    }
}