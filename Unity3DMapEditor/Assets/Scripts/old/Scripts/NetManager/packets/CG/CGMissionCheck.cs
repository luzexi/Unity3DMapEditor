using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGMissionCheck : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONCHECK;
        }

        public override int getSize()
        {
            return sizeof(byte) * 4 + sizeof(uint) + sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            for (int i = 0; i < ITEM_MAX_NUM; i++)
            {
                if (buff.ReadByte(ref m_ItemIndexList[i]) != sizeof(byte)) return false;
            }
            if (buff.ReadByte(ref m_PetIndex) != sizeof(byte)) return false;
            if (buff.ReadUint(ref m_idNPC) != sizeof(uint)) return false;
            if (buff.ReadInt(ref m_idScript) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            for (int i = 0; i < ITEM_MAX_NUM; i++)
            {
                buff.WriteByte(m_ItemIndexList[i]);
            }
            buff.WriteByte(m_PetIndex);
            buff.WriteUint(m_idNPC);
            buff.WriteInt(m_idScript);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public CGMissionCheck()
        {
            for (int i = 0; i < ITEM_MAX_NUM; i++)
            {
                m_ItemIndexList[i] = 0;
            }
            m_PetIndex = 0;
        }

        public static int ITEM_MAX_NUM = 3;

        private byte[]  m_ItemIndexList = new byte[ITEM_MAX_NUM];
		private byte    m_PetIndex;
		private uint    m_idNPC;
		private int     m_idScript;		// 任务的脚本ID

        public byte[] ItemIndexList
        {
            get { return m_ItemIndexList; }
            set { m_ItemIndexList = value; }
        }
        public byte PetIndex
        {
            get { return m_PetIndex; }
            set { m_PetIndex = value; }
        }
        public uint NPCID
        {
            get { return m_idNPC; }
            set { m_idNPC = value; }
        }
        public int ScriptID
        {
            get { return m_idScript; }
            set { m_idScript = value; }
        }
    }

    public class CGMissionCheckFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGMissionCheck(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONCHECK; }
        public override int GetPacketMaxSize() { return sizeof(byte) * 4 + sizeof(uint) + sizeof(int); }
    }
}