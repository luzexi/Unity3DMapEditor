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
    public class GCLevelUpResult : PacketBase
    {
        //聊天内容数据
        private int             m_Result;
        private uint            m_nRemainExp;

        public LEVELUP_RESULT LevelUpResut
        {
            get { return (LEVELUP_RESULT)m_Result; }
        }

        public uint RemainExp
        {
            get { return m_nRemainExp; }
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_LEVELUPRESULT;
        }

        public override int getSize()
        {
            return sizeof(LEVELUP_RESULT) + sizeof(uint);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            if (buff.ReadInt(ref m_Result) != sizeof(int)) return false;
            if (buff.ReadUint(ref m_nRemainExp) != sizeof(uint)) return false;
            return true;
        }

    }


    public class GCLevelUpResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCLevelUpResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_LEVELUPRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(LEVELUP_RESULT) + sizeof(uint);
        }
    };
}