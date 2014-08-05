
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class CGCharSetMask : PacketBase
    {
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_uHelpMask);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHARSETMASK;
        }
        public override int getSize()
        {
            return sizeof(uint);
        }
        public uint HelpMask
        {
            get { return m_uHelpMask; }
            set { m_uHelpMask = value; }
        }
        //数据
        uint m_uHelpMask;	// HelpMask

    };
    public class CGCharSetMaskFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGCharSetMask(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARSETMASK; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint);
        }
    };
}