using System;

using Network;


namespace Network.Packets
{
    public class CGShopBuy : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_SHOPBUY;
        }

        public override int getSize()
        {
            return sizeof(byte)*2 + sizeof(uint)*2 + m_ItemGuid.getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteByte(m_nndex);
            buff.WriteByte(m_uSerialNum);
            buff.WriteUint(m_UniqueID);
            m_ItemGuid.writeToBuff(ref buff);
            buff.WriteUint(m_BuyNum);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public byte Index
        {
            set { m_nndex = value; }
        }
        public byte SerialNum
        {
            set { m_uSerialNum = value; }
        }
        public uint UniqueID
        {
            set { m_UniqueID = value; }
        }
        public _ITEM_GUID ItemGuid
        {
            set { m_ItemGuid = value; }
        }
        public uint BuyNum
        {
            set { m_BuyNum = value; }
        }

        byte m_nndex;		//位置
        byte m_uSerialNum;	//商店流水号
        uint m_UniqueID;
        _ITEM_GUID m_ItemGuid;
        uint m_BuyNum = 1;		//购买的数量
    }

    public class CGShopBuyFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGShopBuy(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_SHOPBUY; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 2 + sizeof(uint) * 2 + _ITEM_GUID.GetMaxSize();
        }
    }
}