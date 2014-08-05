using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;
namespace Network.Packets
{
	public class GCBusMove : PacketBase
	{

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BUSMOVE;
        }

        public override int getSize()
        {
            return sizeof(uint)
                + WORLD_POS.GetMaxSize() * 2
                + sizeof(float);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            m_posWorld.readFromBuff(ref buff);
            m_posTarget.readFromBuff(ref buff);
            buff.ReadFloat(ref m_fTargetHeight);
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }

        uint m_ObjID;			// ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        WORLD_POS m_posWorld;	// 位置
        public WORLD_POS PosWorld
        {
            get { return m_posWorld; }
            set { m_posWorld = value; }
        }
        WORLD_POS m_posTarget;	// 目标位置
        public WORLD_POS PosTarget
        {
            get { return m_posTarget; }
            set { m_posTarget = value; }
        }
        float m_fTargetHeight;	// 目标高度
        public float TargetHeight
        {
            get { return m_fTargetHeight; }
            set { m_fTargetHeight = value; }
        }
    }

    public class GCBusMoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBusMove(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_BUSMOVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint)
                + WORLD_POS.GetMaxSize() * 2
                + sizeof(float);
        }
    }
}
