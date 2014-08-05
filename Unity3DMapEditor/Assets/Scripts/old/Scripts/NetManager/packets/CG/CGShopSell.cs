using System;

using Network;


namespace Network.Packets
{
    public class CGShopSell : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_SHOPSELL;
        }

        public override int getSize()
        {
            return sizeof(byte)  + sizeof(uint) ;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteByte(m_nndex);
            buff.WriteUint(m_UniqueID);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public byte Index
        {
            set { m_nndex = value; }
        }

        public uint UniqueID
        {
            set { m_UniqueID = value; }
        }


        byte m_nndex;		//位置
        uint m_UniqueID;

    }

    public class CGShopSellFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGShopSell(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_SHOPSELL; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte)  + sizeof(uint) ;
        }
    }
}