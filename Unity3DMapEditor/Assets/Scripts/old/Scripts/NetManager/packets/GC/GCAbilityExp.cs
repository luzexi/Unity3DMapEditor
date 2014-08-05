using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCAbilityExp : PacketBase
    {

        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            buff.ReadShort(ref m_AbilityID);
            buff.ReadInt(ref m_Exp);

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ABILITYEXP;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(short);
        }

        public short AbilityID
        {
            get { return m_AbilityID; }
        }

        public int Exp
        {
            get { return this.m_Exp; }
            set { m_Exp = value; }
        }

        //数据
        int m_Exp;
        short m_AbilityID;
    };

    public class GCAbilityExpFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCAbilityExp(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ABILITYEXP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(short);
        }
    };
}