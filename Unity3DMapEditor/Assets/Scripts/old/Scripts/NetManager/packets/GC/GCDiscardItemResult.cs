
using System;
using System.Collections.Generic;

using Network;

namespace Network.Packets
{
    public class GCDiscardItemResult : PacketBase
    {
        public enum Operator
		{
			FromBag = 0,
			FromBank,
		};
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DISCARDITEMRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte)*3 + sizeof(uint);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            if (buff.ReadByte(ref m_Result) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_BagIndex) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_Opt) != sizeof(byte)) return false;
            if (buff.ReadUint(ref m_ItemTableIndex) != sizeof(uint)) return false;
            return true;
        }

        public uint Itemtable
        {
            get { return m_ItemTableIndex; }
        }
        public byte Operate
        {
            get { return m_Opt; }
        }
        public byte Result
        {
            get { return m_Result; }
        }
        public byte BagIndex
        {
            get { return m_BagIndex; }
        }
        byte m_Opt;		     //是否成功，不成功包含错误信息
        byte m_Result;		     //是否成功，不成功包含错误信息
        byte m_BagIndex;		    //成功后，丢弃物品的位置
        uint m_ItemTableIndex;
    }


    public class GCDiscardItemResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDiscardItemResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DISCARDITEMRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3 + sizeof(uint);
        }
    };
}