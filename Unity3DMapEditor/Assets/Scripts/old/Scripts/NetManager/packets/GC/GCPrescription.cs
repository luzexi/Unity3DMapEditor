using System;
using System.Collections.Generic;

namespace Network.Packets
{
    public class GCPrescription : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_PRESCRIPTION;
        }

        public override int getSize()
        {
            return sizeof(int)*2;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_Prescription);
            buff.ReadInt(ref m_LearnOrAbandon);
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public uint Prescription
        {
            get { return m_Prescription; }
        }
        public int LearnOrAbandom
        {
            get { return m_LearnOrAbandon; }
        }
        private uint m_Prescription;
        private int  m_LearnOrAbandon;
    }
    public class GCPrescriptionFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCPrescription(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_PRESCRIPTION; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int)*2;
        }
    };
}
