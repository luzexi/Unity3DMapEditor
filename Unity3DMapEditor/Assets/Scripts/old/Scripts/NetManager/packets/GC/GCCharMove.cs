using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCCharMove : PacketBase
	{

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARMOVE;
        }

        public override int getSize()
        {
            int uSize = 0;
            uSize += sizeof(uint) * 2 + sizeof(int) + WORLD_POS.GetMaxSize() + sizeof(Byte);
            if (IsHaveStopInfo())
            {
                uSize += sizeof(int) + WORLD_POS.GetMaxSize();
            }
            return uSize;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID );
            buff.ReadUint(ref m_uStartTime);
            buff.ReadInt(ref m_nHandleID);
            m_posTarget.readFromBuff(ref buff);
            buff.ReadByte(ref m_byStopMove);
            if (IsHaveStopInfo())
            {
                PosStop.readFromBuff(ref buff);
                buff.ReadInt(ref m_nStopLogicCount);
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(ObjID);
            buff.WriteUint(StartTime);
            buff.WriteInt(HandleID);
            m_posTarget.writeToBuff(ref buff);
            buff.WriteByte(m_byStopMove);
            if (IsHaveStopInfo())
            {
                PosStop.writeToBuff(ref buff);
                buff.WriteInt(StopLogicCount);
            }

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        // 数据 [1/10/2012 Ivan]
        uint m_ObjID;		// ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        uint m_uStartTime;	// 起始时间
        public uint StartTime
        {
            get { return m_uStartTime; }
            set { m_uStartTime = value; }
        }
        int m_nHandleID;	// 操作ID
        public int HandleID
        {
            get { return m_nHandleID; }
            set { m_nHandleID = value; }
        }
        WORLD_POS m_posTarget;	// 目标点
        public WORLD_POS PosTarget
        {
            get { return m_posTarget; }
            set { m_posTarget = value; }
        }
        Byte m_byStopMove;		// 是否要停止上次移动
        int m_nStopLogicCount;	// 停止的逻辑计数
        public int StopLogicCount
        {
            get { return m_nStopLogicCount; }
        }
        WORLD_POS m_posStop;			// 停止的坐标
        public WORLD_POS PosStop
        {
            get { return m_posStop; }
        }
        public void SetStopInfo(int nLogicCount, WORLD_POS pPos)
		{
			m_posStop			= pPos;
			m_nStopLogicCount	= nLogicCount;
			m_byStopMove		= 1;
		}
        public bool IsHaveStopInfo() { return (m_byStopMove != 0)?(true):(false); }
    };

    public class GCCharMoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharMove(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARMOVE; }
        public override int GetPacketMaxSize()
        {
            int uSize = 0;
            uSize += sizeof(uint) * 2 + sizeof(int) + WORLD_POS.GetMaxSize() + sizeof(Byte);
            uSize += sizeof(int) + WORLD_POS.GetMaxSize();
            return uSize;
        }
    };
}
