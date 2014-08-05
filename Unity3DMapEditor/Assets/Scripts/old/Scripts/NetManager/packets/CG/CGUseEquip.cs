using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGUseEquip : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_USEEQUIP;
        }

        byte m_BagIndex;		//使用Bag中的位置存放的位置
        public byte BagIndex
        {
            get { return m_BagIndex; }
            set { m_BagIndex = value; }
        }
        public override int getSize()
        {
            return sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_BagIndex);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class CGUseEquipFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGUseEquip(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_USEEQUIP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte);
        }
    };
}