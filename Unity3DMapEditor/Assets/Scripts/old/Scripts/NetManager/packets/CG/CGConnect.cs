
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class CGConnect : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_Key) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_GUID) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_ServerID) != sizeof(short)) return false;
            if (buff.Read(ref m_szAccount, NET_DEFINE.MAX_ACCOUNT) != NET_DEFINE.MAX_ACCOUNT) return false;
            m_szAccount[NET_DEFINE.MAX_ACCOUNT] = 0;
            if (buff.ReadInt(ref m_nGender) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nCheckVer) != sizeof(int)) return false;
                       
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_Key);
            buff.WriteInt(m_GUID);
            buff.WriteShort(m_ServerID);
            buff.Write(ref m_szAccount, NET_DEFINE.MAX_ACCOUNT);
            buff.WriteInt(m_nGender);
            buff.WriteInt(m_nCheckVer);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CONNECT;
        }
        public override int getSize()
        {
            return sizeof(int) * 4 +
                sizeof(short) +
                sizeof(byte) * NET_DEFINE.MAX_ACCOUNT;
        }


        //public
        public int Key{
            get { return this.m_Key; }
            set { m_Key = value; }
        }
        public int GUID{
            get { return this.m_GUID; }
            set { m_GUID = value; }
        }
        public short ServerID{
            get { return this.m_ServerID; }
            set { m_ServerID = value; }
        }
        public byte[] SzAccount{
            get { return this.m_szAccount; }
            set { m_szAccount = value; }
        }
        public int Gender{
            get { return this.m_nGender; }
            set { m_nGender = value; }
        }
        public int CheckVer {
            get { return this.m_nCheckVer; }
            set { m_nCheckVer = value; }
        }
        //数据
	    private int					m_Key ;
	    private int					m_GUID ;
	    private short				m_ServerID ;

	    //测试用
	    private byte[]				m_szAccount = new byte[NET_DEFINE.MAX_ACCOUNT+1] ;
	    private int					m_nGender;			//性别
	    private int					m_nCheckVer ;

    };
    public class CGConnectFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGConnect(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CONNECT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 4 +
                sizeof(short) +
                sizeof(byte) * NET_DEFINE.MAX_ACCOUNT;
        }
    };
}