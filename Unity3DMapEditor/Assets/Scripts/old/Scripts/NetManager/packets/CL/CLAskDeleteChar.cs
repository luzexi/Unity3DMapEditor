using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Network;
using Network.Handlers;

namespace Network.Packets
{
	public class CLAskDeleteChar  : PacketBase
	{
		//公用继承接口
		public override bool readFromBuff(ref NetInputBuffer buff){return true;}
		public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_GUID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

		public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_CL_ASKDELETECHAR ; }
		public override int getSize()  
		{
			return 	sizeof(uint);

		}

		//使用数据接口
        public byte[] SzAccount
        {
            get
            {
                return this.szAccount;
            }
            set
            {
                szAccount = value;
            }
        }
        public short PlayerID
        {
            get { return this.playerID; }
            set { playerID = value; }
        }
        public uint GUID
        {
            get { return this.m_GUID; }
            set { m_GUID = value;}
        }
		//数据
	
		private uint					m_GUID;

		//玩家池id，客户端不用填写
		private short				playerID;
		private byte[]				szAccount = new byte[NET_DEFINE.MAX_ACCOUNT+1] ;	//用户名称

	}

	public class CLAskDeleteCharFactory :  PacketFactory 
	{
		public override PacketBase CreatePacket()  { return new CLAskDeleteChar() ; }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CL_ASKDELETECHAR; }
		public override int GetPacketMaxSize()
		{ 
			return 	sizeof(uint);
		}
	}

}