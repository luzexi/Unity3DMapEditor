using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGUnEquip : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_UNEQUIP;
        }

        public override int getSize()
        {
            return sizeof(byte) + sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        byte m_EquipPoint;		//装配点信息
        public byte EquipPoint
        {
            get { return m_EquipPoint; }
            set { m_EquipPoint = value; }
        }
        byte m_BagIndex = byte.MaxValue;			//取下来放置在包中的位置
        public byte BagIndex
        {
            get { return m_BagIndex; }
            set { m_BagIndex = value; }
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_EquipPoint);
            buff.WriteByte(m_BagIndex);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    public class CGUnEquipFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGUnEquip(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_UNEQUIP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(byte);
        }
    }
}