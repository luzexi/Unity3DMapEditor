using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCUnEquipResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_UNEQUIPRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte) * 3 + _ITEM_GUID.GetMaxSize()  + sizeof(uint) ;
        }

        byte m_EquipPoint;			//装备点
        public byte EquipPoint
        {
            get { return m_EquipPoint; }
            set { m_EquipPoint = value; }
        }
        byte m_BagIndex;				//卸下来得装备在Bag中存放的位置
        public byte BagIndex
        {
            get { return m_BagIndex; }
            set { m_BagIndex = value; }
        }
        uint m_ItemTableIndex;		//BagIndex上装备的资源号
        public uint ItemTableIndex
        {
            get { return m_ItemTableIndex; }
            set { m_ItemTableIndex = value; }
        }
        _ITEM_GUID m_ItemId;				//BagIndex上装备的GUID
        public _ITEM_GUID ItemId
        {
            get { return m_ItemId; }
            set { m_ItemId = value; }
        }
        byte m_Result;				//是否成功，不成功则是错误号	
        public byte Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
	
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_EquipPoint);
            buff.ReadByte(ref m_BagIndex);
            buff.ReadUint(ref m_ItemTableIndex);
            m_ItemId.readFromBuff(ref buff);
            buff.ReadByte(ref m_Result);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCUnEquipResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCUnEquipResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_UNEQUIPRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3 + _ITEM_GUID.GetMaxSize() + sizeof(uint);
        }
    }
}