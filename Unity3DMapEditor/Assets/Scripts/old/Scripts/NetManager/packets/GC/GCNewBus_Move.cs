using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;
namespace Network.Packets
{
	public class GCNewBus_Move : PacketBase
	{

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWBUS_MOVE;
        }

        public override int getSize()
        {
            return sizeof(uint)
                + sizeof(int)
                + WORLD_POS.GetMaxSize()
                + sizeof(float)
                + WORLD_POS.GetMaxSize()
                + sizeof(float)
                + sizeof(int)
                + sizeof(uint) * m_nPassengerCount;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            buff.ReadInt(ref m_nDataID);
            m_posWorld.readFromBuff(ref buff);
            buff.ReadFloat(ref m_fDir);
            m_posTarget.readFromBuff(ref buff);
            buff.ReadFloat(ref m_fTargetHeight);
            buff.ReadInt(ref m_nPassengerCount);
            if (m_nPassengerCount > 0 && m_nPassengerCount <= GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT)
            {
                for (int i = 0; i < m_nPassengerCount; i++)
                {
                    buff.ReadUint(ref m_anPassengerIDs[i]);
                }
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }

        uint m_ObjID;		// ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        int m_nDataID;		// 数据ID（万位存储的是type）
        public int DataID
        {
            get { return m_nDataID <= 0 ? m_nDataID : m_nDataID % 10000; }
            set { m_nDataID = value; }
        }
        WORLD_POS m_posWorld;		// 位置
        public WORLD_POS PosWorld
        {
            get { return m_posWorld; }
            set { m_posWorld = value; }
        }
        float m_fDir;			// 方向
        public float Dir
        {
            get { return m_fDir; }
            set { m_fDir = value; }
        }
        WORLD_POS m_posTarget;	// 目标位置
        public WORLD_POS PosTarget
        {
            get { return m_posTarget; }
            set { m_posTarget = value; }
        }
        float m_fTargetHeight;	// 目标点高度
        public float TargetHeight
        {
            get { return m_fTargetHeight; }
            set { m_fTargetHeight = value; }
        }
        int m_nPassengerCount;		// 乘客数目
        public int PassengerCount
        {
            get { return m_nPassengerCount; }
            set { m_nPassengerCount = value; }
        }
        uint[] m_anPassengerIDs = new uint[GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT];	// 乘客列表
        public uint[] PassengerIDs
        {
            get { return m_anPassengerIDs; }
            set { m_anPassengerIDs = value; }
        }
    }
    public class GCNewBus_MoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewBus_Move(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_NEWBUS_MOVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint)
                + sizeof(int)
                + WORLD_POS.GetMaxSize()
                + sizeof(float)
                + WORLD_POS.GetMaxSize()
                + sizeof(float)
                + sizeof(int)
                + sizeof(uint) * GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT;
        }
    }
}
