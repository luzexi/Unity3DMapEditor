using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class CLAskCreateChar : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CL_ASKCREATECHAR;
        }

        public override int getSize()
        {
            return sizeof(byte) * (NET_DEFINE.MAX_CHARACTER_NAME + 6)
                + sizeof(uint) + sizeof(short) * 2;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.Write(ref m_Name, sizeof(byte) * NET_DEFINE.MAX_CHARACTER_NAME);
            buff.WriteByte(m_Sex); // [2010-12-14] by: cfp+ BOOL
            buff.WriteByte(m_HairColor);
            buff.WriteByte(m_FaceColor);
            buff.WriteByte(m_HairModel);
            buff.WriteByte(m_FaceModel);
            buff.WriteByte(m_HeadID);
            buff.WriteUint(m_GUID);
            buff.WriteShort(m_Camp);
            buff.WriteShort(m_MenPai);

            //包内容, 10为包头偏移 [12/9/2011 ZL]
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }



        public override bool readFromBuff(ref NetInputBuffer buff)
        {
           
            return true;
        }

        //public interface
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
        public byte[] Name{
            get { return this.m_Name; }
            set {  m_Name = value;}
        }
        public byte Sex
        {
            get { return this.m_Sex; }
            set { m_Sex = value; }
        }

        public byte HairColor
        {
            get { return this.m_HairColor; }
            set { m_HairColor = value; }
        }
        public byte FaceColor
        {
            get { return this.m_FaceColor; }
            set { m_FaceColor = value; }
        }
        public byte HairModel
        {
            get { return this.m_HairModel; }
            set { m_HairModel = value; }
        }
        public byte FaceModel
        {
            get { return this.m_FaceModel; }
            set { m_FaceModel = value; }
        }
        public byte HeadID
        {
            get { return this.m_HeadID; }
            set { m_HeadID = value; }
        }
        public short Camp
        {
            get { return this.m_Camp; }
            set { m_Camp = value; }
        }
        public short MenPai
        {
            get { return this.m_MenPai; }
            set { m_MenPai = value; }
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
		private byte[]			m_Name= new byte[NET_DEFINE.MAX_CHARACTER_NAME];		//新角色名字
		private byte			m_Sex;							//新角色性别 替换为种族，奇数代表男性，偶数代表女性 [12/14/2010 ivan edit]
		private byte			m_HairColor;					//新角色头发颜色	
		private byte			m_FaceColor;					//新角色脸形颜色
		private byte			m_HairModel;					//新角色头发模型
		private byte			m_FaceModel;					//新角色脸形模型
		private byte			m_HeadID;						//新角色头部编号
		private uint			m_GUID;							//
		private short   		m_Camp;							//新角色的阵营
		private short   		m_MenPai;
		
		//玩家池id，客户端不用填写
		private short				playerID;
		private byte[]				szAccount = new byte[NET_DEFINE.MAX_ACCOUNT+1] ;	//用户名称

    }


    public class CLAskCreateCharFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CLAskCreateChar(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CL_ASKCREATECHAR; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * (NET_DEFINE.MAX_CHARACTER_NAME + 6)
                + sizeof(uint) + sizeof(short) * 2;
        }
    };
}