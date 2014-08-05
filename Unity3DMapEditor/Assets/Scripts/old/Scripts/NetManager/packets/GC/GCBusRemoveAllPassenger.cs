using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;
namespace Network.Packets
{
	public class GCBusRemoveAllPassenger : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BUSREMOVEALLPASSENGER;
        }

        public override int getSize()
        {
            return sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
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
    }

    public class GCBusRemoveAllPassengerFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCBusRemoveAllPassenger(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_BUSREMOVEALLPASSENGER; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint);
        }
    }
}
