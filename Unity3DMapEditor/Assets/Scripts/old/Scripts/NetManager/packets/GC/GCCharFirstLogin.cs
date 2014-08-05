using System;
using System.Collections.Generic;

using Network;

namespace Network.Packets
{
    public class GCCharFirstLogin : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_bFirstLogin) != sizeof(byte)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_FIRSTLOGIN;
        }

        public override int getSize()
        {
            return sizeof(byte);
        }

        public byte FirstLogin
        {
            get { return m_bFirstLogin; }
        }

        private byte m_bFirstLogin;
    };

    public class GCCharFirstLoginFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharFirstLogin(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_FIRSTLOGIN; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte);
        }
    };
}