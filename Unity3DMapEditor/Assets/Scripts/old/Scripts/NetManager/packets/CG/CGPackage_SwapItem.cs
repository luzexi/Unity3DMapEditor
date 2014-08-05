using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGPackage_SwapItem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_PACKAGE_SWAPITEM;
        }

        public override int getSize()
        {
            return sizeof(byte) * 2;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        byte pIndex1;
        public byte PIndex1
        {
            get { return pIndex1; }
            set { pIndex1 = value; }
        }
        byte pIndex2;
        public byte PIndex2
        {
            get { return pIndex2; }
            set { pIndex2 = value; }
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(pIndex1);
            buff.WriteByte(pIndex2);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }
    public class CGPackage_SwapItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGPackage_SwapItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_PACKAGE_SWAPITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 2;
        }
    };
}
