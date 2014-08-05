
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGDiscardItem : PacketBase
    {

        public enum Operator
		{
			FromBag = 0,
			FromBank,
		};
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_BagIndex);
            buff.WriteByte(m_Opt);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_DISCARDITEM;
        }

        public override int getSize()
        {
            return sizeof(byte) * 2;
        }

        //public interface
        public byte Operate
        {
            set { m_Opt = value; }
        }
        public byte BagIndex
        {
            set { m_BagIndex = value; }
        }

        byte m_Opt;
        byte m_BagIndex;
    };

    public class CGDiscardItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGDiscardItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_DISCARDITEM; }
        public override int GetPacketMaxSize()
        {
            return  sizeof(byte) * 2;
        }
    };
}