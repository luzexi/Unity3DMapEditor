
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCPetPlacardList : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_PETPLACARDLIST;
        }

        public override int getSize()
        {
            return sizeof(int) + m_nCount * _PET_PLACARD_ITEM.getMaxSize();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            if(buff.ReadInt(ref m_nCount) != sizeof(int))return false;
            for (int i = 0; i < m_nCount; i++)
            {
                if(!m_aItems[i].readFromBuff(ref buff))return false;
            }
            return true;
        }
        public int Count
        {
            get { return m_nCount; }
        }

        public _PET_PLACARD_ITEM[] Item
        {
            get { return m_aItems; }
        }
        private int m_nCount;
        private _PET_PLACARD_ITEM[] m_aItems = new _PET_PLACARD_ITEM[GAMEDEFINE.MAX_PETPLACARD_LIST_ITEM_NUM];	
		
    }

    public class GCPetPlacardListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCPetPlacardList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_PETPLACARDLIST; }
        public override int GetPacketMaxSize()
        {
            return _PET_PLACARD_ITEM.getMaxSize() + sizeof(int);
        }
    };
}