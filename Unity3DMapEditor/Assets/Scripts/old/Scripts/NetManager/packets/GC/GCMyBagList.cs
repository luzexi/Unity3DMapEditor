using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
    public struct _BAG_ITEM : ClassCanbeSerialized
    {
        public Byte m_nndex;
        public _ITEM_GUID m_ItemID;
        public uint m_ItemTableIndex;
        public Byte m_Count;

        #region ClassCanbeSerialized Members

        public int getSize()
        {
            return sizeof(Byte) * 2 + sizeof(uint) + m_ItemID.getSize();
        }

        public static int GetMaxSize()
        {
            return sizeof(Byte) * 2 + sizeof(uint) + _ITEM_GUID.GetMaxSize();
        }

        public bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_nndex);
            m_ItemID.readFromBuff(ref buff);
            buff.ReadUint(ref m_ItemTableIndex);
            buff.ReadByte(ref m_Count);
            
            return true;
        }

        public int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_nndex);
            m_ItemID.writeToBuff(ref buff);
            buff.WriteUint(m_ItemTableIndex);
            buff.WriteByte(m_Count);

            return getSize();
        }

        #endregion
    };

    public class GCMyBagList : PacketBase
	{

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MYBAGLIST;
        }

        public override int getSize()
        {
            return sizeof(int) +
            sizeof(Byte) +
            _BAG_ITEM.GetMaxSize() * m_AskCount;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int bMode = 0;
            buff.ReadInt(ref bMode);
            m_Mode = (ASK_BAG_MODE)bMode;

            buff.ReadByte(ref m_AskCount);
            if (AskCount > GAMEDEFINE.MAX_BAG_SIZE)
            {
                AskCount = GAMEDEFINE.MAX_BAG_SIZE;
            }
            for (int i = 0; i < AskCount; i++)
            {
                m_ItemData[i].readFromBuff(ref buff);
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt((int)m_Mode);
            buff.WriteByte(m_AskCount);
            if(AskCount > GAMEDEFINE.MAX_BAG_SIZE)
            {
                AskCount = GAMEDEFINE.MAX_BAG_SIZE;
            }
            for (int i = 0; i < AskCount; i++)
            {
                m_ItemData[i].writeToBuff(ref buff);
            }

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        
		ASK_BAG_MODE		m_Mode;
        public Network.Packets.ASK_BAG_MODE Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
		Byte				m_AskCount;
        public System.Byte AskCount
        {
            get { return m_AskCount; }
            set { m_AskCount = value; }
        }
		_BAG_ITEM[]			m_ItemData = new _BAG_ITEM[GAMEDEFINE.MAX_BAG_SIZE];
        public void SetAskItemData(_BAG_ITEM pAskData, Byte index)
        {
            m_ItemData[index] = pAskData;
        }
        public _BAG_ITEM[] GetItemData() { return m_ItemData; }
    }

    public class GCMyBagListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCMyBagList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_MYBAGLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) +
            sizeof(Byte) +
            _BAG_ITEM.GetMaxSize() * GAMEDEFINE.MAX_BAG_SIZE;
        }
    }
}
