using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
	public class GCPickResult:PacketBase
	{
		public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            m_ItemID.readFromBuff(ref buff);
            buff.ReadByte(ref m_Result);
            buff.ReadByte(ref m_BagIndex);
            buff.ReadUShort(ref m_ItemTableIndex);
            m_BagItemGUID.readFromBuff(ref buff);
            return true;
        }
		public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            m_ItemID.writeToBuff(ref buff);
            buff.WriteByte(m_Result);
            buff.WriteByte(m_BagIndex);
            buff.WriteUShort(m_ItemTableIndex);
            m_BagItemGUID.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
		public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_GC_PICKRESULT; }

		 public override int getSize() 
		{
            return sizeof(uint)+sizeof(byte)*2+_ITEM_GUID.GetMaxSize()*2+sizeof(ushort); 
		}

		public void			setItemID(_ITEM_GUID itemIndex){m_ItemID = itemIndex;}
		public _ITEM_GUID		getItemID()	{return m_ItemID;}
		public void			setItemBoxId(uint	id){m_ObjID = id;}
		public uint			getItemBoxId(){return m_ObjID;}

		public void			setResult(byte Result){m_Result = Result;}
		public byte			getResult(){return m_Result;}

		public void			setBagIndex(byte index){m_BagIndex = index;}
		public byte			getBagIndex(){return m_BagIndex;}

		public void			setItemTableIndex(ushort tableIndex){m_ItemTableIndex= tableIndex;}
		public ushort			getItemTableIndex(){return m_ItemTableIndex;}

		public void			setBagItemGUID(_ITEM_GUID itemGUID){m_BagItemGUID = itemGUID;}
		public _ITEM_GUID		getBagItemGUID()	{return m_BagItemGUID;}

		uint			m_ObjID;
		_ITEM_GUID		m_ItemID;
		_ITEM_GUID		m_BagItemGUID;		//存放位置的物品的GUID
		byte			m_Result;			//是否成功，不成功包含错误信息
		byte			m_BagIndex;			//成功后，存放的位置
		ushort			m_ItemTableIndex;

	};


	public class GCPickResultFactory: PacketFactory
	{
		public override PacketBase CreatePacket() { return new GCPickResult(); }
		public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_PICKRESULT; }
		public override int GetPacketMaxSize()
        {
            return sizeof(uint)+sizeof(byte)*2+_ITEM_GUID.GetMaxSize()*2+sizeof(ushort); 
        }
	};






}
