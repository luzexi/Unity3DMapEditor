using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGCharMove : PacketBase
	{

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_CHARMOVE;
        }

        public override int getSize()
        {
            return sizeof(uint) +
                sizeof(int) +
                WORLD_POS.GetMaxSize() +
                sizeof(Byte) +
                WORLD_POS.GetMaxSize() * m_yNumTargetPos;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_ObjID);
            buff.ReadInt(ref m_nHandleID);
            PosWorld.readFromBuff(ref buff);
            buff.ReadByte(ref m_yNumTargetPos);

            if (m_yNumTargetPos > 0 && m_yNumTargetPos <= GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER)
            {
                for (int i = 0; i < m_yNumTargetPos; i++)
                {
                    m_aTargetPos[i].readFromBuff(ref buff);
                }
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(ObjID);
            buff.WriteInt(HandleID);
            PosWorld.writeToBuff(ref buff);
            buff.WriteByte(m_yNumTargetPos);
            if (m_yNumTargetPos > 0 && m_yNumTargetPos <= GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER)
            {
                for (int i = 0; i < m_yNumTargetPos; i++)
                {
                    m_aTargetPos[i].writeToBuff(ref buff);
                }
            }

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        // 数据 [1/10/2012 Ivan]
        uint				m_ObjID;			// ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }
		int					m_nHandleID;		// 操作ID
        public int HandleID
        {
            get { return m_nHandleID; }
            set { m_nHandleID = value; }
        }
		WORLD_POS			m_posWorld;			// 当前位置
        public WORLD_POS PosWorld
        {
            get { return m_posWorld; }
            set { m_posWorld = value; }
        }
		Byte				m_yNumTargetPos;
		WORLD_POS[]			m_aTargetPos = new WORLD_POS[GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER];		// 目标位置
        public WORLD_POS[] TargetPos
        {
            get { return m_aTargetPos; }
        }
        public void addTargetPos( WORLD_POS pPos )
		{
            if (m_yNumTargetPos < GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER)
			{
				m_aTargetPos[m_yNumTargetPos] = pPos;
				m_yNumTargetPos++;
			}
		}
    };

    public class CGCharMoveFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGCharMove(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARMOVE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) +
                sizeof(int) +
                WORLD_POS.GetMaxSize() +
                sizeof(Byte) +
                WORLD_POS.GetMaxSize() * GAMEDEFINE.MAX_CHAR_PATH_NODE_NUMBER;
        }
    };
}
