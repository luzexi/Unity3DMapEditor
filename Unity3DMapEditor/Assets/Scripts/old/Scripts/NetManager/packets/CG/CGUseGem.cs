using System;
using System.Collections.Generic;

using Network;

namespace Network.Packets
{
    
    public class CGUseGem : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_USEGEM;
        }

        public override int getSize()
        {
            return sizeof(byte) * 3 + GAMEDEFINE.MAX_ITEM_GEM * sizeof(short);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_isInBag);
            buff.WriteByte(m_EquipBagIndex);
            for (int i = 0; i < GAMEDEFINE.MAX_ITEM_GEM; i++ )
            {
                buff.WriteShort(m_MatBagIndex[i]);
            }
            buff.WriteByte(m_petIndex);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public void SetGemState(short[] gems)
        {
            if(gems.Length == m_MatBagIndex.Length)
                m_MatBagIndex = gems;
        }

        public byte EquipBagIndex
        {
            set { m_EquipBagIndex = value; }
        }
        public byte isInBag
        {
            set { m_isInBag = value; }
        }

        public byte petIndex
        {
            set { m_petIndex = value; }
        }

        byte m_isInBag;// 是否在背包里，0为在装备点上，1为在背包里   2为在宠物上`
        short[] m_MatBagIndex = new short[GAMEDEFINE.MAX_ITEM_GEM]; // 按宝石孔存储的 宝石背包里的位置
                                                                  // 若 第 0 位为 UCHAR_MAX，则装备的第0个宝石孔为摘除
		byte					m_EquipBagIndex;
        byte                    m_petIndex;
    }

    public class CGUseGemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGUseGem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_USEGEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3 + GAMEDEFINE.MAX_ITEM_GEM * sizeof(short);
        }
    }
}