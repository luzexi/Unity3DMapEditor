using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCSystemMsg : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_GUID);
            buff.ReadByte(ref m_MessageType);
            buff.ReadByte(ref m_ContexSize);
            if (m_ContexSize > 0 && m_ContexSize <= 255)
                buff.Read(ref m_Contex, m_ContexSize);
            buff.ReadUint(ref m_CommonField);
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SYSTEMMSG;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(byte) * 2 + sizeof(byte) * m_ContexSize + sizeof(uint);
        }

        byte[] m_Contex = new byte[GAMEDEFINE.MAX_SYSTEM_MSG_SIZE];
        public byte[] Contex
        {
            get { return m_Contex; }
        }
		byte	m_ContexSize;
        public byte ContexSize
        {
            get { return m_ContexSize; }
        }
		byte	m_MessageType;
        public byte MessageType
        {
            get { return m_MessageType; }
        }
		uint	m_GUID;
        public uint GUID
        {
            get { return m_GUID; }
        }
        uint    m_CommonField;
        public uint CommonField
        {
            get { return m_CommonField; }
        }
    };

    public class GCSystemMsgFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCSystemMsg(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_SYSTEMMSG; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(byte) * 2 + sizeof(byte) * GAMEDEFINE.MAX_SYSTEM_MSG_SIZE + sizeof(uint);
        }
    };
}