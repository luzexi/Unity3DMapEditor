using System;

using Network;
using Network.Handlers;



namespace Network.Packets
{
    class GCShopBuy : PacketBase
    {
        public enum BuyResult
		{
			BUY_OK = 0,				// 购买成功
			BUY_BACK_OK,			// 回购成功
			BUY_MONEY_FAIL,			// 没钱了
			BUY_RMB_FAIL,			// 没钱了
			BUY_NO_MERCH,			// 已经卖完了
			BUY_BAG_FULL,			// 放不下了
		};

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SHOPBUY;
        }

        public override int getSize()
        {
            return sizeof(byte)*2 + sizeof(uint);
        }


        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_IsBuyOk);
            buff.ReadUint(ref m_ItemIndex);
            buff.ReadByte(ref m_ItemNum);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public byte IsBuyOk
        {
            get { return m_IsBuyOk; }
        }
        public uint ItemIndex
        {
            get { return m_ItemIndex; }
        }
        public byte ItemNum
        {
            get { return m_ItemNum; }
        }

        byte m_IsBuyOk;		//成功与否
        uint m_ItemIndex;	//购买的物品索引
        byte m_ItemNum;		//购买的物品数量
    }

    public class GCShopBuyFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCShopBuy(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SHOPBUY; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 2 + sizeof(uint);
        }
    };
}