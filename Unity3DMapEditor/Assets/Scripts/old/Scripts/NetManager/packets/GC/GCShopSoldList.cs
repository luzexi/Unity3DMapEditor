using System;

using Network;
using Network.Handlers;
namespace Network.Packets
{
    class GCShopSoldList : PacketBase
    {
        public GCShopSoldList()
        {

        }
         //商人所卖商品结构
        public struct _MERCHANDISE_ITEM:ClassCanbeSerialized
        {
            public _ITEM item_data;
            public uint iPrice;
        
            public static int getMaxSize()
            {
                return sizeof(int) + _ITEM.GetMaxSize();
            }
            #region ClassCanbeSerialized Members

            public int getSize()
            {
                if (item_data == null)
                    item_data = new _ITEM();
                return sizeof(uint) + item_data.getSize();
            }

            public bool readFromBuff(ref NetInputBuffer buff)
            {
                if (item_data == null)
                    item_data = new _ITEM();
                item_data.readFromBuff(ref buff);
                buff.ReadUint(ref iPrice);
                return true;
            }

            public int writeToBuff(ref NetOutputBuffer buff)
            {
                throw new NotImplementedException();
            }

            #endregion
        };

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SHOPSOLDLIST;
        }

        public override int getSize()
        {
            return sizeof(byte) + m_MerchadiseList[0].getSize() * m_nMerchadiseNum;
        }


        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_nMerchadiseNum);
            for (byte i = 0; i < m_nMerchadiseNum; i++ )
            {
                m_MerchadiseList[i].readFromBuff(ref buff);
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public byte MerchadiseNum
        {
            get { return m_nMerchadiseNum; }
        }
        public _MERCHANDISE_ITEM[] MerchadiseList
        {
            get { return m_MerchadiseList; }
        }

        //商品个数
		byte						m_nMerchadiseNum;
		//所有商品列表
		_MERCHANDISE_ITEM[]			m_MerchadiseList = new _MERCHANDISE_ITEM[GAMEDEFINE.MAX_BOOTH_NUMBER];
    }

    public class GCShopSoldListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCShopSoldList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SHOPSOLDLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + GCShopSoldList._MERCHANDISE_ITEM.getMaxSize()*GAMEDEFINE.MAX_BOOTH_NUMBER;
        }
    };
}