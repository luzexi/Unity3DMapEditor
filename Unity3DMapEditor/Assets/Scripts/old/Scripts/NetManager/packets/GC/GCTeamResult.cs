using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCTeamResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_TEAMRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte) * (3 + m_NameSize) + sizeof(short)* 3 + sizeof(uint) * 3;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            buff.ReadByte(ref m_Return);
            buff.ReadUint(ref m_GUID);
            buff.ReadShort(ref m_TeamID);
            buff.ReadUint(ref m_GUIDEx);
            buff.ReadShort(ref m_SceneID);
            buff.ReadByte(ref m_NameSize);
            if (m_NameSize > 0)
                buff.Read(ref m_Name, m_NameSize);
            buff.ReadInt(ref m_nPortrait);
            buff.ReadShort(ref m_uDataID);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
        public byte Return
        {
            get { return m_Return; }
        }
        public uint GUID
        {
            get { return m_GUID; }
        }
        public short TeamId
        {
            get { return m_TeamID; }
        }
        public uint GUIDEx
        {
            get { return m_GUIDEx; }
        }
        public short SceneId
        {
            get { return m_SceneID; }
        }
        public byte[] Name
        {
            get { return m_Name; }
        }
        public int Portrait
        {
            get { return m_nPortrait; }
        }
        public short DataID
        {
            get { return m_uDataID; }
        }
        public byte AllocRuler
        {
            get { return m_TeamAllocationRuler; }
        }

        byte				m_Return ;
		uint				m_GUID ;
		short			    m_TeamID ;
		uint				m_GUIDEx ;
		short			    m_SceneID ;
		byte				m_NameSize;
		byte[]				m_Name = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];			// 队员的名字，在队员入队的时候用
		int					m_nPortrait;						// 头像
		short				m_uDataID;							// 队员的性别

		//// 分配模式 [8/23/2011 zzh+]
		byte                m_TeamAllocationRuler;
    }

    public class GCTeamResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCTeamResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_TEAMRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * (3 + GAMEDEFINE.MAX_CHARACTER_NAME) + sizeof(short) * 3 + sizeof(uint) * 3;
        }
    }
}