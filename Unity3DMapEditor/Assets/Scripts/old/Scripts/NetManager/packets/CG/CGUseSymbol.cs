using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGUseSymbol : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PAKCKET_CG_USESYMBOL;
        }

        byte m_BagIndex;		//使用Bag中的位置存放的位置
        public byte BagIndex
        {
            get { return m_BagIndex; }
            set { m_BagIndex = value; }
        }
        public override int getSize()
        {
            return sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_BagIndex);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class CGUseSymbolFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGUseSymbol(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PAKCKET_CG_USESYMBOL; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte);
        }
    };
}