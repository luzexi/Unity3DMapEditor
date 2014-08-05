using System;

using Network;
using Network.Handlers;
namespace Network.Packets
{
    class GCShopUpdateMerchandiseList : PacketBase
    {

        //商人所卖商品结构
        public struct _MERCHANDISE_ITEM:ClassCanbeSerialized
        {
            public uint idTable;		//资源id
            public byte byNumber;		//数量


            public static int getMaxSize()
            {
                return sizeof(uint) + sizeof(byte); 
            }
            #region ClassCanbeSerialized Members

            public int getSize()
            {
                return sizeof(uint) + sizeof(byte);
            }

            public bool readFromBuff(ref NetInputBuffer buff)
            {
                buff.ReadUint(ref idTable);
                buff.ReadByte(ref byNumber);
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
            return (short)PACKET_DEFINE.PACKET_GC_SHOPUPDATEMERCHANDISELIST;
        }

        public override int getSize()
        {
            return sizeof(byte) + m_MerchadiseList[0].getSize() * m_nMerchadiseNum;
        }


        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_nMerchadiseNum);
            for (byte i = 0; i < m_nMerchadiseNum; i++)
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
		_MERCHANDISE_ITEM[]			m_MerchadiseList=new _MERCHANDISE_ITEM[GAMEDEFINE.MAX_BOOTH_NUMBER];
    }

    public class GCShopUpdateMerchandiseListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCShopUpdateMerchandiseList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SHOPUPDATEMERCHANDISELIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + GCShopUpdateMerchandiseList._MERCHANDISE_ITEM.getMaxSize();
        }
    };
}