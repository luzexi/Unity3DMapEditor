using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    class GCNewMonster_Move : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            buff.ReadInt(ref m_nHandleID);
            m_posWorld.readFromBuff(ref buff);
            buff.ReadFloat(ref m_fMoveSpeed);
            m_posTarget.readFromBuff(ref buff);
            buff.ReadByte(ref m_IsNPC);
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            buff.WriteInt(m_nHandleID);
            m_posWorld.writeToBuff(ref buff);
            buff.WriteFloat(m_fMoveSpeed);
            m_posTarget.writeToBuff(ref buff);
            buff.WriteByte(m_IsNPC);
            return (int)NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWMONSTER_MOVE; }
        public override int getSize()
        {
            return sizeof(uint) + sizeof(int) + WORLD_POS.GetMaxSize() * 2 + sizeof(float) + sizeof(byte);
        }

        //使用数据接口
        public void setObjID(uint id) { m_ObjID = id; }
        public uint getObjID() { return m_ObjID; }

        public void setHandleID(int nID) { m_nHandleID = nID; }
        public int getHandleID() { return m_nHandleID; }

        public void setWorldPos(ref WORLD_POS pos) { m_posWorld = pos; }
        public WORLD_POS getWorldPos() { return m_posWorld; }

        public void setMoveSpeed(float fMoveSpeed) { m_fMoveSpeed = fMoveSpeed; }
        public float getMoveSpeed() { return m_fMoveSpeed; }

        public void setTargetPos(ref WORLD_POS pos) { m_posTarget = pos; }
        public WORLD_POS getTargetPos() { return m_posTarget; }

        public void setIsNPC(bool bNPC) { m_IsNPC = (byte)(bNPC ? 1 : 0); }
        public bool getIsNPC() { return m_IsNPC == 1; }

        uint m_ObjID;		// ObjID
        int m_nHandleID;
        WORLD_POS m_posWorld;		// 位置

        float m_fMoveSpeed;	// 移动速度
        WORLD_POS m_posTarget;	// 移动的目标点
        byte m_IsNPC;
    }


    class GCNewMonster_MoveFactory : PacketFactory
    {

        public override PacketBase CreatePacket() { return new GCNewMonster_Move(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWMONSTER_MOVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(int) + WORLD_POS.GetMaxSize() * 2 + sizeof(float) + sizeof(byte);
        }
    }
}