using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCNewPlayer_Move : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWPLAYER_MOVE;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(int) + WORLD_POS.GetMaxSize() * 2 + sizeof(short) + sizeof(float);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadInt(ref m_nHandleID) != sizeof(int)) return false;

            if (!m_posWorld.readFromBuff(ref buff)) return false;
            short equipVer = 0;
            if (buff.ReadShort(ref equipVer) != sizeof(short)) return false;


            m_wEquipVer = (ushort)EquipVer;
            if (buff.ReadFloat(ref m_fMoveSpeed) != sizeof(float)) return false;


            if (!m_posTarget.readFromBuff(ref buff)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return (int)NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        /////////////////////////////////////////////////////////////
        uint m_ObjID;		// ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        int m_nHandleID;
        public int HandleID
        {
            get { return m_nHandleID; }
            set { m_nHandleID = value; }
        }
        WORLD_POS m_posWorld;		// 位置
        public WORLD_POS PosWorld
        {
            get { return m_posWorld; }
            set { m_posWorld = value; }
        }
        ushort m_wEquipVer;	// 玩家的装备版本号
        public ushort EquipVer
        {
            get { return m_wEquipVer; }
            set { m_wEquipVer = value; }
        }
        float m_fMoveSpeed;	// 移动速度
        public float MoveSpeed
        {
            get { return m_fMoveSpeed; }
            set { m_fMoveSpeed = value; }
        }
        WORLD_POS m_posTarget;	// 移动的目标点
        public WORLD_POS PosTarget
        {
            get { return m_posTarget; }
            set { m_posTarget = value; }
        }
    }

    public class GCNewPlayer_MoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewPlayer_Move(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWPLAYER_MOVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(int) + WORLD_POS.GetMaxSize() * 2 + sizeof(short) + sizeof(float);
        }
    };
}
