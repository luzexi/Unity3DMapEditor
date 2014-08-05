using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;
namespace Network.Packets
{
	public class GCBusRemovePassenger : PacketBase
	{
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BUSREMOVEPASSENGER;
        }

        public override int getSize()
        {
            return sizeof(uint)
                + sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            buff.ReadUint(ref m_nPassengerID);
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
        uint m_nPassengerID;		// 乘客ID
        public uint PassengerID
        {
            get { return m_nPassengerID; }
            set { m_nPassengerID = value; }
        }
    }

    public class GCBusRemovePassengerFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBusRemovePassenger(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_BUSREMOVEPASSENGER; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint)
                + sizeof(uint);
        }
    }
}
