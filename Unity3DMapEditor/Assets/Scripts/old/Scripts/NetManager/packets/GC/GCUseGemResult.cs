using System;


using Network;


namespace Network.Packets
{
    class GCUseGemResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USEGEMRESULT;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        USEGEM_RESULT m_Result;
        public USEGEM_RESULT Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int iTemp = 0;
            buff.ReadInt(ref iTemp);
            m_Result = (USEGEM_RESULT)iTemp;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCUseGemResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCUseGemResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_USEGEMRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}