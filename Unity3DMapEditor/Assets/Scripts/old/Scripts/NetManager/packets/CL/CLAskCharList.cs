using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets{
	
 public class CLAskCharList : PacketBase
    {
		
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CL_ASKCHARLIST;
        }

        public override int getSize()
        {
            return sizeof(byte) * NET_DEFINE.MAX_ACCOUNT;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
	        buff.Write(ref szAccount, NET_DEFINE.MAX_ACCOUNT);

			
            //包内容, 10为包头偏移 [12/9/2011 ZL]
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
		
		

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.Read(ref szAccount, sizeof(byte) * NET_DEFINE.MAX_ACCOUNT);
                
            return true;
        }
	
		//public interface
		public byte[] SzAccount{
			get{
				return this.szAccount;
			}
			set{
				szAccount = value;
			}
		}
		
        public short PlayerID{
            get { return this.playerID; }
            set { playerID = value; }
        }
			
	
		private byte[] szAccount 			 = new byte[NET_DEFINE.MAX_ACCOUNT+1]; //用户名称
        private short playerID;
		
    }


    public class CLAskCharListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CLAskCharList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CL_ASKCHARLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * NET_DEFINE.MAX_ACCOUNT;
        }
    };
}