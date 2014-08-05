using System;
using System.Collections.Generic;
using System.Text;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCPackage_SwapItem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_PACKAGE_SWAPITEM;
        }

        public override int getSize()
        {
            return sizeof(byte) * 3;
        }

        //结果
        // 0 - 成功
        // 1 - 失败
        byte result;
        public byte Result
        {
            get { return result; }
            set { result = value; }
        }
        //要求交换的两个Item的索引
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
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref result);
            buff.ReadByte(ref pIndex1);
            buff.ReadByte(ref pIndex2);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCPackage_SwapItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCPackage_SwapItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_PACKAGE_SWAPITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3;
        }
    }
}
