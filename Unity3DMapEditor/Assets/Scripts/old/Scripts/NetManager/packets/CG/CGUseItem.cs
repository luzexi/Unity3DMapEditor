using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGUseItem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_USEITEM;
        }

        public override int getSize()
        {
            return sizeof(byte) + sizeof(uint) + WORLD_POS.GetMaxSize() + sizeof(float) + PET_GUID_t.getMaxSize() + sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        byte m_BagIndex = byte.MaxValue;
        public byte BagIndex
        {
            get { return m_BagIndex; }
            set { m_BagIndex = value; }
        }
        uint m_Target = 0;
        public uint Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }
        WORLD_POS m_posTarget;
        public WORLD_POS PosTarget
        {
            get { return m_posTarget; }
            set { m_posTarget = value; }
        }
        float m_fDir = -1f;
        public float Dir
        {
            get { return m_fDir; }
            set { m_fDir = value; }
        }
        PET_GUID_t m_TargetPetGUID;
        public PET_GUID_t TargetPetGUID
        {
            get { return m_TargetPetGUID; }
            set { m_TargetPetGUID = value; }
        }
        byte m_TargetItemIndex = byte.MaxValue;
        public byte TargetItemIndex
        {
            get { return m_TargetItemIndex; }
            set { m_TargetItemIndex = value; }
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_BagIndex);
            buff.WriteUint(m_Target);
            m_posTarget.writeToBuff(ref buff);
            buff.WriteFloat(m_fDir);
            m_TargetPetGUID.writeToBuff(ref buff);
            buff.WriteByte(m_TargetItemIndex);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class CGUseItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGUseItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_USEITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(uint) + WORLD_POS.GetMaxSize() + sizeof(float) + PET_GUID_t.getMaxSize() + sizeof(byte);
        }
    }
}