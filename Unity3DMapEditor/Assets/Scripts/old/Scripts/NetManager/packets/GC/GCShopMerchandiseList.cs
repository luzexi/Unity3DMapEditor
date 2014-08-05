using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    class GCShopMerchandiseList : PacketBase
    {
        public struct _MERCHANDISE_ITEM : ClassCanbeSerialized   //学习后的结果（改变了什么）
        {
            public uint idTable;		//资源id
            public byte byNumber;		//数量
            public uint nPrice;			//价格
            public int  MaxNumber;		//有限商品的数量

            public static int getMaxSize()
            {
                return sizeof(uint) * 3 + sizeof(byte);
            }
            public Boolean readFromBuff(ref Network.NetInputBuffer buff)
            {
                buff.ReadUint(ref idTable);
                buff.ReadByte(ref byNumber);
                buff.ReadUint(ref nPrice);
                buff.ReadInt(ref MaxNumber);

                return true;
            }

            public Int32 getSize()
            {
                return sizeof(uint) * 3 + sizeof(byte) ;
            }

            public Int32 writeToBuff(ref Network.NetOutputBuffer buff)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        };

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SHOPMERCHANDISELIST;
        }

        public override int getSize()
        {
            return sizeof(uint)*6 + sizeof(byte) * 6 + sizeof(float)*3 + m_MerchadiseList[0].getSize()*GAMEDEFINE.MAX_BOOTH_NUMBER;
        }


        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            buff.ReadUint(ref m_nObjID);
            buff.ReadByte(ref m_nShopType);
            buff.ReadByte(ref m_nMerchadiseNum);
            buff.ReadInt(ref m_nRepairLevel);
            buff.ReadInt(ref m_nBuyLevel);
            buff.ReadInt(ref m_nRepairType);
            buff.ReadInt(ref m_nBuyType);
            buff.ReadFloat(ref m_nRepairSpend);
            buff.ReadFloat(ref m_nRepairOkProb);
            buff.ReadUint(ref m_UniqueID);
            buff.ReadByte(ref m_bBuyBack);
            buff.ReadFloat(ref m_fScale);
            buff.ReadByte(ref m_uCurrencyUnit);
            buff.ReadByte(ref m_uSerialNum);
            buff.ReadByte(ref m_bCanBuyMulti);
            for (int i = 0; i < m_nMerchadiseNum; i++)
                m_MerchadiseList[i].readFromBuff(ref buff);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
        public uint ObjectID
        {
            get { return m_nObjID; }
        }
        public byte ShopType
        {
            get { return m_nShopType; }
        }
        public byte MerchadiseNum
        {
            get { return m_nMerchadiseNum; }
        }
        public int RepairLevel
        {
            get { return m_nRepairLevel; }
        }
        public int RepairType
        {
            get { return m_nRepairType; }
        }
        public int BuyLevel
        {
            get { return m_nBuyLevel; }
        }
        public int BuyType
        {
            get { return m_nBuyType; }
        }
        public float RepairSpend
        {
            get { return m_nRepairSpend; }
        }
        public float RepairOkProb
        {
            get { return m_nRepairOkProb; }
        }
        public uint UniqueID
        {
            get { return m_UniqueID; }
        }
        public byte bBuyBack
        {
            get { return m_bBuyBack; }
        }
        public float fScale
        {
            get { return m_fScale; }
        }
        public byte CurrencyUnit
        {
            get { return m_uCurrencyUnit; }
        }
        public byte SerialNum
        {
            get { return m_uSerialNum; }
        }
        public byte CanBuyMulti
        {
            get { return m_bCanBuyMulti; }
        }
        public _MERCHANDISE_ITEM[] MerchadiseList
        {
            get { return m_MerchadiseList; }
        }
        //商人的id
		uint				m_nObjID;
		byte				m_nShopType;		// 商店类型
		//商品个数
		byte				m_nMerchadiseNum;
		int							m_nRepairLevel;		// 修理等级
		int							m_nBuyLevel;		// 收购等级
		int							m_nRepairType;		// 修理类型
		int							m_nBuyType;			// 商店的收购类型
		float						m_nRepairSpend;		// 修理花费
		float						m_nRepairOkProb;	// 修理成功几率
		uint						m_UniqueID;
		byte						m_bBuyBack;			//	是否支持回购

		float						m_fScale;			// 商店系数

		byte						m_uCurrencyUnit;
		byte						m_uSerialNum;
		byte						m_bCanBuyMulti;		//是否能够指定购买数量

		//所有商品列表
		_MERCHANDISE_ITEM[]			m_MerchadiseList = new _MERCHANDISE_ITEM[GAMEDEFINE.MAX_BOOTH_NUMBER];
    }

    public class GCShopMerchandiseListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCShopMerchandiseList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SHOPMERCHANDISELIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) * 6 + sizeof(byte) * 6 + sizeof(float) * 3 + GCShopMerchandiseList._MERCHANDISE_ITEM.getMaxSize() * GAMEDEFINE.MAX_BOOTH_NUMBER;
        }
    };
}