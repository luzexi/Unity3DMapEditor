using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;
namespace Network.Packets
{
	public class GCBusStopMove : PacketBase
	{
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BUSSTOPMOVE;
        }

        public override int getSize()
        {
            return sizeof(uint)
                + WORLD_POS.GetMaxSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            m_posWorld.readFromBuff(ref buff);
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
        WORLD_POS m_posWorld;		// 位置
        public WORLD_POS PosWorld
        {
            get { return m_posWorld; }
            set { m_posWorld = value; }
        }
    }

    public class GCBusStopMoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBusStopMove(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_BUSSTOPMOVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint)
                + WORLD_POS.GetMaxSize();
        }
    }
}
