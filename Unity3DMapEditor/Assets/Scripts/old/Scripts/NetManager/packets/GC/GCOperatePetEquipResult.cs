using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCOperatePetEquipResult : PacketBase
    {
        public enum UseOperatePetEquipResultCode
        {
            USE_PETEQUIP_FAIL = 0,  //失败
            USE_PETEQUIP_SUCCESS = 1,  //成功
            USE_PETEQUIP_BAG_FULL = 2,  //存放包已满
            USE_PETEQUIP_HAS_ITEM = 3,  //存放包已有物品
        };

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_Type) != sizeof(byte)) return false;
            if(buff.ReadByte(ref m_Result)!= sizeof(byte))return false;
            if(buff.ReadByte(ref m_EquipBagIndex)!= sizeof(byte)) return false;
            if (buff.ReadByte(ref m_BagIndex) != sizeof(byte)) return false;
            if(!m_PetGUID.readFromBuff(ref buff))return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_OPERATE_PET_EQUIP;
        }

        public override int getSize()
        {
            return sizeof(byte) * 4 + PET_GUID_t.getMaxSize();
        }

        public PET_GUID_t GUID
        {
            get { return this.m_PetGUID; }
            set { m_PetGUID = value; }
        }

        public byte OperatorType
        {
            get { return this.m_Type; }
            set { m_Type = value; }
        }
        public byte Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
        public byte EquiptIndex
        {
            get { return m_EquipBagIndex; }
            set { m_EquipBagIndex = value;}
        }
        public byte BagIndex
        {
            get { return this.m_BagIndex; }
            set { m_BagIndex = value; }
        }
        //数据
        byte m_Type;             //类型：0 穿上法宝，1 脱下法宝
        byte m_Result;           //结果信息:UseOperatePetEquipResultCode
        byte m_EquipBagIndex;    //法宝装配点
        byte m_BagIndex;         //存放的位置
        PET_GUID_t m_PetGUID;          //宠物的GUID
    };

    public class GCOperatePetEquipResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCOperatePetEquipResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_OPERATE_PET_EQUIP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 4 + PET_GUID_t.getMaxSize();
        }
    };
}