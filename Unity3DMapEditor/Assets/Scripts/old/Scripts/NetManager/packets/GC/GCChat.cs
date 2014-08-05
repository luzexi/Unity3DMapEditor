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
    public class GCChat : PacketBase
    {
        //聊天内容数据
        private byte m_ContexSize;
        private byte[] m_Contex = new byte[GAMEDEFINE.MAX_CHAT_SIZE];
        //数据
		private byte			m_ChatType ;//聊天消息类型 enum CGChat::CHAT_TYPE

		//说话者名字
		byte					m_SourNameSize ;
        byte[] m_SourName = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];

		//说话者的ObjID, 普通说话中有效
		uint					m_SourID ;
		short				    m_CampID;	//发送者阵营
		uint					m_uWorldChatID ; //聊天消息的序列号，用于纠正玩家转移场景消息丢失问题

        public byte[] Contex
        {
            get { return m_Contex; }
        }
        public byte ContexSize
        {
            get { return m_ContexSize; }
        }
        public byte SourNameSize
        {
            get { return m_SourNameSize; }
        }
        public byte[] SourName
        {
            get { return m_SourName; }
        }
        public byte ChatType
        {
            get { return m_ChatType; }
        }
        public uint SourObject
        {
            get { return m_SourID; }
        }
        public short CampID
        {
            get { return m_CampID; }
        }
        public uint WorldChatID
        {
            get { return m_uWorldChatID; }
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHAT;
        }

        public override int getSize()
        {
            if (m_ChatType == (byte)ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL ||
               m_ChatType == (byte)ENUM_CHAT_TYPE.CHAT_TYPE_MAP)
            {
                return sizeof(byte) +
                        sizeof(byte) +
                        m_ContexSize * sizeof(byte) +
                        sizeof(byte) +
                        m_SourNameSize * sizeof(byte) +
                        sizeof(uint) +
                        sizeof(short) +
                        sizeof(uint);
            }
            else
            {
                return sizeof(byte) +
                        sizeof(byte) +
                        m_ContexSize * sizeof(byte) +
                        sizeof(byte) +
                        m_SourNameSize * sizeof(byte) +
                        sizeof(short) +
                        sizeof(uint);
            }
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            buff.ReadByte(ref m_ChatType);

            buff.ReadByte(ref m_ContexSize);
            if (m_ContexSize > 0 && m_ContexSize <= 255)
                buff.Read(ref m_Contex, m_ContexSize);

            buff.ReadByte(ref m_SourNameSize);
            if (m_SourNameSize > 0 && m_SourNameSize <= GAMEDEFINE.MAX_CHARACTER_NAME)
                buff.Read(ref m_SourName, m_SourNameSize);

            if (m_ChatType == (int)ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL ||
                m_ChatType == (int)ENUM_CHAT_TYPE.CHAT_TYPE_MAP)
                buff.ReadUint(ref m_SourID);

            buff.ReadShort(ref m_CampID);
            buff.ReadUint(ref m_uWorldChatID);
            return true;
        }

    }


    public class GCChatFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCChat(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHAT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(byte) + sizeof(byte) * 256 +
                                                         sizeof(byte) +
                                                         sizeof(byte) * GAMEDEFINE.MAX_CHARACTER_NAME +
                                                         sizeof(uint) +
                                                         sizeof(short) +
                                                         sizeof(uint);
        }
    };
}