using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
	public class CGCharPositionWarp :PacketBase
    {
        public CGCharPositionWarp()
		{
           
		}

		//公用继承接口
		public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (!m_posServer.readFromBuff(ref buff)) return false;
            if (!m_posClient.readFromBuff(ref buff)) return false;
            return true;
        }
		public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            m_posServer.writeToBuff(ref buff);
            m_posClient.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHARPOSITIONWARP;
        }


		public override int getSize()
        {
            return sizeof(int) +
                m_posServer.getSize() +
                m_posClient.getSize(); 
        }

		//使用数据接口
	    public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
    
        public WORLD_POS ServerPos
        {
            get { return this.m_posServer; }
            set { m_posServer = value; }
        }

        public WORLD_POS ClientPos
        {
            get { return this.m_posClient; }
            set { m_posClient = value; }
        }

	    private	int		            m_ObjID;			// ObjID
	    private	WORLD_POS			m_posServer;		// 服务器当前位置
	    private	WORLD_POS			m_posClient;		// 客户端当前位置
	};

    public class CGCharPositionWarpFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskChangeScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARPOSITIONWARP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + WORLD_POS.GetMaxSize()*2;
        }
    };
}