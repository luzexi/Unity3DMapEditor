using System;
using System.Collections.Generic;

using Network.Handlers;

namespace Network.Packets
{
    public class GCCharMoveResult : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARMOVERESULT;
        }

        public override int getSize()
        {
            return sizeof(int) * 2 + sizeof(byte) + m_aTargetPos[0].getSize() * m_yNumTargetPos;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadInt(ref m_nHandleID);
            buff.ReadInt(ref m_nResult);
            buff.ReadByte(ref m_yNumTargetPos);
            for (byte i = 0; i < m_yNumTargetPos; i++)
            {
                m_aTargetPos[i].readFromBuff(ref buff);
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        // 数据 [1/10/2012 Ivan]
        public int HandleID
        {
            get { return m_nHandleID; }
        }
        public int Result
        {
            get { return m_nResult; }
        }
        public byte NumTargetPos
        {
            get { return m_yNumTargetPos; }
        }
        public WORLD_POS[] TargetPos
        {
            get { return m_aTargetPos; }
        }

        private int					m_nHandleID;	// 操作ID
		private int					m_nResult;		// ORESULT
		private byte				m_yNumTargetPos;
		private WORLD_POS[]			m_aTargetPos = new WORLD_POS[GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER];		// 目标位置
    };

    public class GCCharMoveResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharMoveResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARMOVERESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(byte) + WORLD_POS.GetMaxSize() * GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER;
        }
    };
}
