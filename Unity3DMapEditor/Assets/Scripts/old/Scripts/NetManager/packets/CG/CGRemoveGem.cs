using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGRemoveGem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_REMOVEGEM;
        }

        public override int getSize()
        {
            return sizeof(byte) * 3;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_EquipBagIndex);
            buff.WriteByte(m_MatBagIndex);
            buff.WriteByte(m_GemIndex);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public byte EquipBagIndex
        {
            set { m_EquipBagIndex = value; }
        }
        public byte MatBagIndex
        {
            set { m_MatBagIndex = value; }
        }
        public byte GemIndex
        {
            set { m_GemIndex = value; }
        }
       

        private byte m_EquipBagIndex;    //装备在包中的位置
        private byte m_MatBagIndex;      //材料在包中的位置 [2011-7-16] by: cfp+
        private byte m_GemIndex;		 //宝石孔编号
    }
    public class CGRemoveGemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGRemoveGem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_REMOVEGEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3;
        }
    };
}
