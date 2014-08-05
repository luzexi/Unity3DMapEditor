
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;
//公用继承接口
public enum ASK_PET_EQUIP_TYPE
{
    ASK_PET_EQUIP_MYSELF,   //请求自己的宠物装备列表
    ASK_PET_EQUIP_OTHER     //请求别人的宠物装备列表
};
namespace Network.Packets
{
    public class CGAskDetailPetEquipList : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nType);
            buff.WriteInt(m_ObjID);
            m_PetGUID.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKDETAIL_PET_EQUIPLIST;
        }
        public override int getSize()
        {
            return sizeof(int) * 2 + PET_GUID_t.getMaxSize();
        }

        public int ObjID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        public PET_GUID_t GUID
        {
            get { return this.m_PetGUID; }
            set { m_PetGUID = value; }
        }

        public ASK_PET_EQUIP_TYPE EquiptType
        {
            get { return (ASK_PET_EQUIP_TYPE)this.m_nType; }
            set { m_nType = (int)value; }
        }

        //数据
        private int m_nType;    //操作类型 enum ENUM_ASK_PET_EQUIP_TYPE
        private int m_ObjID;            //对方的Pet的ObjID //只有在查询其他玩家宠物的时候ObjID才有效 查询自己的时候写-1
        private PET_GUID_t m_PetGUID;          //宠物的GUID

    };
    public class CGAskDetailPetEquipListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskDetailPetEquipList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKDETAIL_PET_EQUIPLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2 + PET_GUID_t.getMaxSize();
        }
    };
}