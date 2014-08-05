using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCAbilitySucc : PacketBase
    {

        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            buff.ReadShort(ref m_Ability);
            buff.ReadInt(ref m_Prescription);
            buff.ReadUint(ref m_Obj);

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ABILITY_SUCC;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 + sizeof(short);
        }

        public short AbilityID
        {
            get { return m_Ability; }
        }
        public int PrescriptionID
        {
            get { return m_Prescription; }
        }
        public uint ObjectID
        {
            get { return this.m_Obj; }
        }


        //数据
        short m_Ability;
        int m_Prescription;
        uint m_Obj;
    };

    public class GCAbilitySuccFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCAbilitySucc(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ABILITY_SUCC; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2 + sizeof(short);
        }
    };
}