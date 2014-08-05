using System;
using System.Collections.Generic;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGOperateTalisman : PacketBase
    {
        public enum EOPTMType
        {
            EOP_TM_EQUIP   = 0,
            EOP_TM_UNEQUIP = 1,
            EOP_TM_EAT     = 2,
        };
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_OPERATETALISMAN;
        }

        public override int getSize()
        {
            return sizeof(byte)*3;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(ucType);
            buff.WriteByte(ucSourBagIndex);
            buff.WriteByte(ucDestBagIndex);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public byte Type
        {
            set { ucType = value; }
        }
        public byte SrcIndex
        {
            set { ucSourBagIndex = value; }
        }

        public byte DstIndex
        {
            set { ucDestBagIndex = value; }
        }

         //数据
        //按策划设计，穿法宝为拖拽，脱法宝为点击！
        //1.若ucType为0，且ucBagIndex>=0 && ucEquipBagIndex>=0，则为穿法宝（从装备栏到存放栏，ucEquipBagIndex->ucBagIndex）
        //2.若ucType为1，且ucBagIndex>=0 && ucEquipBagIndex>=0，则为拖拽穿法宝（从存放栏到装备栏，ucBagIndex->ucEquipBagIndex）
        //3.若ucType为2，且ucBagIndex>=0 && ucEquipBagIndex>=0，则为吃法宝（按规则吃）
        byte            ucType;             //0：穿法宝，1：脱法宝，2：吃法宝
        byte            ucSourBagIndex;		//原位置
        byte            ucDestBagIndex;	//目标位置
    }
    public class CGOperateTalismanFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGOperateTalisman(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_OPERATETALISMAN; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte)*3;
        }
    };
}
