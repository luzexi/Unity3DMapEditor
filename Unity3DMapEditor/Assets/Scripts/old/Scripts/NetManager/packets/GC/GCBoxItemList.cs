using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
	public class GCBoxItemList: PacketBase
	{
	     public override bool readFromBuff(ref NetInputBuffer buff)
         {
             buff.ReadUint(ref m_ItemBoxID);
             buff.ReadByte(ref m_ItemNumber);
             buff.ReadUShort(ref m_ItemBoxType);
		    if(m_ItemNumber>GAMEDEFINE.MAX_BOXITEM_NUMBER)	m_ItemNumber	= GAMEDEFINE.MAX_BOXITEM_NUMBER;

		    for(int	i =0;i<m_ItemNumber;i++)
		    {
				m_ItemList[i] = new _ITEM();
			    m_ItemList[i].readFromBuff(ref buff);
		    }
		    return true ;
         }
		public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ItemBoxID);
		    buff.WriteByte(m_ItemNumber);
		    buff.WriteUShort(m_ItemBoxType);				
    	
		    for(int	i =0;i<m_ItemNumber;i++)
		    {
			    m_ItemList[i].writeToBuff(ref buff);
		    }
		    return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

		public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BOXITEMLIST; }
		public override int getSize()
		{ 

			int	isize	=	sizeof(uint)+sizeof(byte)+sizeof(ushort);
			for(int	i = 0;i<m_ItemNumber;i++)
			{
		
				isize+= m_ItemList[i].getSize();

			}
			return isize ;
		}
		//使用数据接口
		public void			setItemNum(byte num) { m_ItemNumber = num; }
		public byte			getItemNum()  { return m_ItemNumber; }

// 		public void			setItemList(_ITEM pItemList)
// 		{
// 			memcpy(m_ItemList,pItemList,sizeof(_ITEM)*MAX_BOXITEM_NUMBER);
// 		}
// 
// 		public void			setItemData(_ITEM	pItem,byte uIndex)
// 		{
// 			memcpy(&m_ItemList[uIndex] ,pItem,sizeof(_ITEM));
// 		}
		public _ITEM[]			getItemList()	
		{
			return  m_ItemList;
		}
		
		public void			setItemBoxId(uint ObjId){m_ItemBoxID = ObjId;}
		public uint			getItemBoxId(){return m_ItemBoxID;}

		public void			setItemBoxType(ushort IBType)
		{
			m_ItemBoxType	=	IBType;
		}

		public ushort			getItemBoxType(){return m_ItemBoxType;}

		 ushort       m_ItemBoxType;
		 uint	m_ItemBoxID;
		 byte		m_ItemNumber;
		_ITEM[]		m_ItemList = new _ITEM[GAMEDEFINE.MAX_BOXITEM_NUMBER];

	};

	public class GCBoxItemListFactory:PacketFactory
	{
		public override PacketBase CreatePacket() { return new GCBoxItemList() ; }
		public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_BOXITEMLIST; }
		public override int GetPacketMaxSize()
        {
            return	sizeof(uint)+sizeof(byte)+sizeof(ushort)+
												_ITEM.GetMaxSize()*GAMEDEFINE.MAX_BOXITEM_NUMBER; 
        }
	}

}
