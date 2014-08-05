
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class CGCharJump : PacketBase
    {
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHARJUMP;
        }
        public override int getSize()
        {
            return sizeof(int);
        }
        public int ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
        //数据
        private int m_ObjID;

    };
    public class CGCharJumpFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGCharJump(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARJUMP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}