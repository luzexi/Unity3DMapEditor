
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCManipulatePetRet : PacketBase
    {
        public enum ENUM_MANIPULATEPET_RET
        {
            MANIPULATEPET_RET_INVALID = -1,
            MANIPULATEPET_RET_CAPTUREFALID,		// 捕捉失败
            MANIPULATEPET_RET_CAPTURESUCC,		// 捕捉成功
            MANIPULATEPET_RET_CALLUPFALID,		// 召唤失败
            MANIPULATEPET_RET_CALLUPSUCC,		// 召唤成功
            MANIPULATEPET_RET_FREEFALID,		// 放生失败
            MANIPULATEPET_RET_FREESUCC,			// 放生成功
            MANIPULATEPET_RET_RECALLFALID,		// 收回失败
            MANIPULATEPET_RET_RECALLSUCC,		// 收回成功

        };

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MANIPULATEPETRET;
        }

        public override int getSize()
        {
            return m_GUID.getSize() + sizeof(int)*2;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            m_GUID.readFromBuff(ref buff);
            buff.ReadInt(ref m_bFighting);
            buff.ReadInt(ref m_Ret);
            return true;
        }
        public PET_GUID_t GUID
        {
            get { return m_GUID; }
        }
        public int IsFighting
        {
            get { return m_bFighting; }
        }
        public int Result
        {
            get { return m_Ret; }
        }
        private PET_GUID_t m_GUID;
        private int m_bFighting;	// 是否处于参战状态
        private int m_Ret;			// 返回结果
    }


    public class GCManipulatePetRetFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCManipulatePetRet(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_MANIPULATEPETRET; }
        public override int GetPacketMaxSize()
        {
            return PET_GUID_t.getMaxSize() + sizeof(int)*2;
        }
    };
}