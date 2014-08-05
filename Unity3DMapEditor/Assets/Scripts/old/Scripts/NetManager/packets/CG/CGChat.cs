// 用于测试与简易服务端相连 [12/15/2011 ZL]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGChat : PacketBase
    {
		//数据
        byte m_ChatType = (byte)ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL;//聊天消息类型 enum CHAT_TYPE
        public byte ChatType
        {
            get { return m_ChatType; }
            set { m_ChatType = value; }
        }
		
        //聊天内容数据
        private byte m_ContexSize;
        private byte[] m_Contex;

        public void SetTalkContent(string text)
        {
            m_Contex = EncodeUtility.Instance.GetBytes(text);
            m_ContexSize = (byte)m_Contex.Count();
        }
		
		//私聊对象的角色名字，仅在CHAT_TYPE_TELL时有效
		byte					m_TargetSize ;
		byte[]					m_TargetName ;
        public void SetTarget(string target)
        {
            m_TargetName = EncodeUtility.Instance.GetBytes(target);
            m_TargetSize = (byte)m_TargetName.Count();
        }
		//队伍号，仅在队伍聊天时有效
		short				    m_TeamID ;
		//频道号，仅在自建聊天频道聊天时有效
        short                   m_ChannelID;
		//帮派号，仅属于此帮派的成员有效
        short                   m_GuildID;
		//门派值，仅此门派内的成员有效
        byte                    m_MenpaiID;

        public void SetTeamID(short tid) { m_TeamID = tid; }
        public short GetTeamID() { return m_TeamID; }

        public void SetChannelID(short cid) { m_ChannelID = cid; }
        public short GetChannelID() { return m_ChannelID; }

        public void SetGuildID(short tid) { m_GuildID = tid; }
        public short GetGuildID() { return m_GuildID; }

        public void SetMenpaiID(byte menpai) { m_MenpaiID = menpai; }
        public byte GetMenpaiID() { return m_MenpaiID; } 
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHAT;
        }

        public override int getSize()
        {
            int iSize = sizeof(byte) + sizeof(byte) + m_ContexSize * sizeof(byte);
            switch ((ENUM_CHAT_TYPE)m_ChatType)
            {
                case ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL:
                case ENUM_CHAT_TYPE.CHAT_TYPE_WORLD:
                case ENUM_CHAT_TYPE.CHAT_TYPE_LIE:
                case ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM:
                case ENUM_CHAT_TYPE.CHAT_TYPE_MAP:
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TEAM:
                    iSize += sizeof(short);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TELL:
                    //Assert(m_TargetSize <= MAX_CHARACTER_NAME);
                    iSize += (sizeof(byte) + m_TargetSize * sizeof(byte));
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CHANNEL:
                    iSize += sizeof(short);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_GUILD:
                    iSize += sizeof(short);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CAMP:
                    iSize += sizeof(byte);
                    break;
                default:
                    break;
            }

            return iSize; 
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移 [12/9/2011 ZL]
            buff.WriteByte(m_ChatType);
            buff.WriteByte(m_ContexSize);
            buff.Write(ref m_Contex, m_ContexSize);
            switch ((ENUM_CHAT_TYPE)m_ChatType)
            {
                case ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL:
                case ENUM_CHAT_TYPE.CHAT_TYPE_WORLD:
                case ENUM_CHAT_TYPE.CHAT_TYPE_LIE:
                case ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM:
                case ENUM_CHAT_TYPE.CHAT_TYPE_MAP:
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TEAM:
                    buff.WriteShort(m_TeamID);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TELL:
                    buff.WriteByte(m_TargetSize);
                    buff.Write(ref m_TargetName, m_TargetSize);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CHANNEL:
                    buff.WriteShort(m_ChannelID);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_GUILD:
                    buff.WriteShort(m_GuildID);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CAMP:
                    buff.WriteByte(m_MenpaiID);
                    break;
                default:
                    break;
            }
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            if (buff.ReadByte(ref m_ContexSize) != 1) return false;
            m_Contex = new byte[m_ContexSize];
            if (buff.Read(ref m_Contex, m_ContexSize) != m_ContexSize) return false;
            switch ((ENUM_CHAT_TYPE)m_ChatType)
            {
                case ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL:
                case ENUM_CHAT_TYPE.CHAT_TYPE_WORLD:
                case ENUM_CHAT_TYPE.CHAT_TYPE_LIE:
                case ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM:
                case ENUM_CHAT_TYPE.CHAT_TYPE_MAP:
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TEAM:
                    buff.ReadShort(ref m_TeamID);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TELL:
                    buff.ReadByte(ref m_TargetSize);
                    buff.Read(ref m_TargetName, m_TargetSize);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CHANNEL:
                    buff.ReadShort(ref m_ChannelID);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_GUILD:
                    buff.ReadShort(ref m_GuildID);
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CAMP:
                    buff.ReadByte(ref m_MenpaiID);
                    break;
                default:
                    break;
            }
            return true;
        }

    }

    public class CGChatFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGChat(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHAT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) +
                    sizeof(byte) +
                    sizeof(byte) * GAMEDEFINE.MAX_CHAT_SIZE +
                    sizeof(byte) +
                    sizeof(byte) * GAMEDEFINE.MAX_CHARACTER_NAME +
                    sizeof(short) +
                    sizeof(short) +
                    sizeof(short) +
                    sizeof(byte);
        }
    };
}