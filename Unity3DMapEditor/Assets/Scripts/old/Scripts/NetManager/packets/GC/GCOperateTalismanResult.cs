using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Network;
using Network.Handlers;
using UnityEngine;
namespace Network.Packets
{
    public class GCOperateTalismanResult : PacketBase
    {
        public enum UseOperateTalismanResultCode
        {
            USEOPERATETALISMAN__FAIL = 0,  //失败
            USEOPERATETALISMAN_SUCCESS = 1,  //成功
            USEOPERATETALISMAN_BAG_FULL = 2,  //法宝存放包已满
            USEOPERATETALISMAN_HAS_ITEM = 3,  //法宝存放包已有物品
            USEOPERATETALISMAN_CANNOT_EAT = 4,//法宝不能吃
            USEOPERATETALISMAN_MAX_LEVE = 5, //法宝已经达到最大等级
        };

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_Type) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_Result) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_TMEquipBagIndex) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_TMBagIndex) != sizeof(byte)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_OPERATETALISMANRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte) * 4;
        }

        public byte Type
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
            get { return this.m_TMEquipBagIndex; }
            set { m_TMEquipBagIndex = value; }
        }

        public byte BagIndex
        {
            get { return this.m_TMBagIndex; }
            set { m_TMBagIndex = value; }
        }

        byte m_Type;             //类型：0 穿上法宝，1 脱下法宝
        byte m_Result;           //结果信息:0 成功，1失败
        byte m_TMEquipBagIndex;  //法宝装配点
        byte m_TMBagIndex;       //存放的位置
    };

    public class GCOperateTalismanResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCOperateTalismanResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_OPERATETALISMANRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 4;
        }
    };
}