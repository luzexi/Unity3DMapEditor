using System;

using Network;
using Network.Handlers;



namespace Network.Packets
{
    class GCShopSell : PacketBase
    {
        public enum SellResult
        {
            SELL_OK = 0,		// 卖出成功
            SELL_FAIL,		// 卖出失败
        };

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SHOPSELL;
        }

        public override int getSize()
        {
            return sizeof(byte) ;
        }


        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_IsSellOk);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }


        public byte IsSellOk
        {
            get { return m_IsSellOk; }
        }

        byte m_IsSellOk;		//成功与否
    }

    public class GCShopSellFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCShopSell(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SHOPSELL; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte);
        }
    };
}