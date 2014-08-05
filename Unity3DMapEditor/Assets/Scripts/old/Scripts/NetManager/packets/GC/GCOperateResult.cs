using System;
using System.Collections.Generic;

namespace Network.Packets
{
    public class GCOperateResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_OPERATE_RESULT;
        }

        int m_nResult;		// 返回值 OPERATE_RESULT
        public int Result
        {
            get { return m_nResult; }
        }
        public override int getSize()
        {
            return sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadInt(ref m_nResult);
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }
    public class GCOperateResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCOperateResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_OPERATE_RESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}
