using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    //请求邮件的请求类型
    public enum ASK_TYPE
    {
        ASK_TYPE_LOGIN = 0,//用户刚登陆游戏时发送的邮件检查消息,
        //如果有邮件，服务器发送通知消息
        ASK_TYPE_MAIL,	//用户向服务器提出需要邮件的消息
        //如果有邮件则发送邮件数据
    };

    public class CGAskMail : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKMAIL;
        }

        public override int getSize()
        {
            return sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_AskType) != sizeof(byte))
            {
                return false;
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_AskType);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private byte m_AskType ;// enum ASK_TYPE
        public byte AskType
        {
            get { return m_AskType; }
            set { m_AskType = value; }
        }
    }

    public class CGAskMailFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskMail(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_ASKMAIL; }
        public override int GetPacketMaxSize() { return sizeof(byte); }
    }
}