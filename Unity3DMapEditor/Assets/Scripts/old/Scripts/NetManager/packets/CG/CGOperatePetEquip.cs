
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGOperatePetEquip : PacketBase
    {
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(ucType);
            buff.WriteByte(ucSourceBagIndex) ;
            buff.WriteByte(ucDestBagIndex) ;
            m_PetGUID.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_OPERATE_PET_EQUIP;
        }
        public override int getSize()
        {
            return sizeof(byte) * 3 + PET_GUID_t.getMaxSize();
        }


        //public
        public byte SourecBagIndex
        {
            get { return this.ucSourceBagIndex; }
            set { ucSourceBagIndex = value; }
        }
        public byte DestBagIndex
        {
            get { return this.ucDestBagIndex; }
            set { ucDestBagIndex = value; }
        }

        public byte OperatorType
        {
            get {return ucType;}
            set {ucType = value;}
        }
        public PET_GUID_t GUID
        {
            get {return m_PetGUID;}
            set {m_PetGUID = value;} 
        }
        //数据
        byte ucType;             //0：穿装备，1：脱装备
        byte ucSourceBagIndex;		//原位置  背包栏的物品位置
        byte ucDestBagIndex;	    //目标位置  宠物装备栏物品位置
        PET_GUID_t m_PetGUID;          //宠物的GUID

    };
    public class CGOperatePetEquipFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGOperatePetEquip(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_OPERATE_PET_EQUIP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3 + PET_GUID_t.getMaxSize();
        }
    };
}