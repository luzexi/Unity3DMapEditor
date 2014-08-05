using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCAbilityResult : PacketBase
    {

        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            buff.ReadShort(ref m_Ability);
            buff.ReadInt(ref m_Prescription);
            buff.ReadInt(ref m_nResult);
       
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ABILITY_RESULT;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 + sizeof(short) ;
        }
 
        public short AbilityID
        {
            get { return m_Ability; }
        }
        public int PrescriptionID
        {
            get { return m_Prescription; }
        }
        public int Result
        {
            get { return this.m_nResult; }
            set { m_nResult = value; }
        }

  
        //数据
        short m_Ability;
        int m_Prescription;
        int m_nResult;		// 返回值 OPERATE_RESULT
    };

    public class GCAbilityResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCAbilityResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ABILITY_RESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2 + sizeof(short) ;
        }
    };
}