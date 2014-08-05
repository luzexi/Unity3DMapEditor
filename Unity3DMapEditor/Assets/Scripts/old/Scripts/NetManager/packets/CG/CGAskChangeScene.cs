
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class CGAskChangeScene : PacketBase
    {
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteShort(m_SourSceneID);
            buff.WriteShort(m_DestSceneID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKCHANGESCENE;
        }
        public override int getSize()
        {
            return sizeof(short) *2;
        }


        //public
        public short SourSceneID
        {
            get { return this.m_SourSceneID; }
            set { m_SourSceneID = value; }
        }
        public short DestSceneID
        {
            get { return this.m_DestSceneID; }
            set { m_DestSceneID = value; }
        }

        //数据
        private short m_SourSceneID;
        private short m_DestSceneID;

    };
    public class CGAskChangeSceneFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskChangeScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKCHANGESCENE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(short) * 2;
        }
    };
}