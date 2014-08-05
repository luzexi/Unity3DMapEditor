
using System;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
    public struct _STUDYXINFA_INFO : ClassCanbeSerialized
    {
        public short m_idMenpai;
        public short m_idXinfa;
        public short m_NowLevel;

        public static int GetMaxSize() { return sizeof(short) * 3; }
        public int getSize() { return sizeof(short) * 3; }
        public bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteShort(m_idMenpai);
            buff.WriteShort(m_idXinfa);
            buff.WriteShort(m_NowLevel);
            return getSize();
        }
    }
    public class CGAskStudyXinfa : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return m_UplevelInfo.readFromBuff(ref buff);
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            m_UplevelInfo.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKSTUDYXINFA;
        }
        public override int getSize()
        {
            return m_UplevelInfo.getSize();
        }

        public _STUDYXINFA_INFO UplevelInfo
        {
            get { return m_UplevelInfo; }
            set
            {
                m_UplevelInfo = value;
            }
        }

        //数据
        private _STUDYXINFA_INFO m_UplevelInfo;
    };

    public class CGAskStudyXinfaFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskStudyXinfa(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKSTUDYXINFA; }
        public override int GetPacketMaxSize()
        {
            return _STUDYXINFA_INFO.GetMaxSize();
        }
    };
}