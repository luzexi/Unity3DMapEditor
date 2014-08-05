// 用于测试与简易服务端相连 [12/15/2011 ZL]
using System;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCManualAttrResult : PacketBase
    {
        //聊天内容数据
        private ATTR_RESUlT m_Result;

        private _ATTR_LEVEL1 m_Attr;
        private byte         m_Index; //0 为主角，其他为宠物

        public ATTR_RESUlT Result
        {
            get { return m_Result; }
        }

        public int this[int index]
        {
            get {
                if (index < 0 || index > (int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER)
                    throw new ArgumentOutOfRangeException("GCManualAttrResult : " + index);
                return m_Attr[index]; 
            }
        }

        public byte Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public _ATTR_LEVEL1 Attrib
        {
            get { return m_Attr; }
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MANUALATTRRESULT;
        }

        public override int getSize()
        {
            return sizeof(LEVELUP_RESULT) + sizeof(uint) + sizeof(byte);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            int result = 0;
            if (buff.ReadInt(ref result) != sizeof(int)) return false;
            m_Result = (ATTR_RESUlT) result;
            m_Attr.readFromBuff(ref buff);
            if (buff.ReadByte(ref m_Index) != sizeof(byte)) return false;
            return true;
        }

    }


    public class GCManualAttrResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCManualAttrResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_MANUALATTRRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(ATTR_RESUlT) + _ATTR_LEVEL1.getMaxSize() + sizeof(byte);
        }
    };
}