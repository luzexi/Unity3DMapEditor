using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public enum ASK_BAG_MODE
    {
        ASK_ALL,
        ASK_SET
    };

    public class CGAskMyBagList : PacketBase
	{

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKMYBAGLIST;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(Byte) + sizeof(Byte) * m_AskCount;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int mode = 0;
            buff.ReadInt(ref mode);
            m_Mode = (ASK_BAG_MODE)mode;

            buff.ReadByte(ref m_AskCount);
            if (m_AskCount > GAMEDEFINE.MAX_BAG_SIZE) 
                m_AskCount = GAMEDEFINE.MAX_BAG_SIZE;

            for (int i = 0; i < m_AskCount; i++)
            {
                buff.ReadByte(ref m_ItemIndex[i]);

            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt((int)m_Mode);
            buff.WriteByte(m_AskCount);
            int count = m_AskCount > (Byte)GAMEDEFINE.MAX_BAG_SIZE ? (Byte)GAMEDEFINE.MAX_BAG_SIZE: m_AskCount;
            for (int i = 0; i < count; i++)
            {
                buff.WriteByte(m_ItemIndex[i]);
            }
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        private ASK_BAG_MODE m_Mode;
        public Network.Packets.ASK_BAG_MODE Mode
        {
            get { return m_Mode; }
            set
            {
                m_Mode = value;
                if (m_Mode == ASK_BAG_MODE.ASK_ALL)
                {
                    m_AskCount = 0;
                }
            }
        }
        private Byte m_AskCount;
        public System.Byte AskCount
        {
            get { return m_AskCount; }
            set { m_AskCount = value; }
        }
        private Byte[] m_ItemIndex = new Byte[GAMEDEFINE.MAX_BAG_SIZE];


        public void SetAskItemIndex(Byte AskIndex, Byte index) { m_ItemIndex[index] = AskIndex; }
        public Byte GetAskItemIndex(Byte index) { return m_ItemIndex[index]; }
    }


    public class CGAskMyBagListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskMyBagList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKMYBAGLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(Byte) + sizeof(Byte) + sizeof(Byte) * GAMEDEFINE.MAX_BAG_SIZE;
        }
    };
}
