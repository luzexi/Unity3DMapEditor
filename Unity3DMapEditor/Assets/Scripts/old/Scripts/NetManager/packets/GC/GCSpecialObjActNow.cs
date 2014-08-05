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
    public class GCSpecialObjActNow : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nLogicCount) != sizeof(int)) return false;
            if (!m_TargetList.readFromBuff(ref buff)) return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nObjID);
            buff.WriteInt(m_nLogicCount);
            m_TargetList.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SPECIAL_OBJ_ACT_NOW;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 + m_TargetList.getSize();
        }

        public int ObjID
        {
            get { return this.m_nObjID; }
            set { m_nObjID = value; }
        }

        public int LogicCount
        {
            get { return this.m_nLogicCount; }
            set { m_nLogicCount = value; }
        }

        public _ObjID_List ObjIDList
        {
            get { return this.m_TargetList; }
            set { m_TargetList = value; }
        }
        private int m_nObjID;	// 特殊对象的ID
        private int m_nLogicCount; //逻辑计数 
        _ObjID_List m_TargetList; // 目标列表.
    };

    public class GCSpecialObjActNowFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCSpecialObjActNow(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SPECIAL_OBJ_ACT_NOW; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 2 + _ObjID_List.getMaxSize();
        }
    };
}