using System;
using Network;
namespace Network.Packets
{
    public enum ENUM_MANIPULATE_TYPE
    {
        MANIPULATE_INVALID = -1,	// 无效
        MANIPULATE_CREATEPET,		// 召唤宠物
        MANIPULATE_DELETEPET,		// 收回宠物
        MANIPULATE_FREEPET,			// 放生宠物
        MANIPULATE_RESETSKILL,		// 重置宠物技能
        MANIPULATE_ASKOWNPETINFO,
        MANIPULATE_ASKOTHERPETINFO, // 察看其他玩家的宠物信息(宠物征友等用...)


        MANIPULATE_NUMBERS,
    };
    public class CGManipulatePet : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            m_PetGUID.writeToBuff(ref buff);
            buff.WriteInt(m_nType);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MANIPULATEPET;
        }

        public override int getSize()
        {
            return sizeof(uint) *2 + m_PetGUID.getSize();
        }

        public uint ObjectID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        public PET_GUID_t PetGUID
        {
            get { return m_PetGUID; }
            set { m_PetGUID = value;}
        }
        public int Type
        {
            get { return m_nType; }
            set { m_nType = value;}
        }
        private uint m_ObjID;	// 宠物的ObjID;
        private PET_GUID_t m_PetGUID;
        private int m_nType;

    };

    public class CGManipulatePetFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGManipulatePet(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_MANIPULATEPET; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int)*2 + PET_GUID_t.getMaxSize();
        }
    };
}