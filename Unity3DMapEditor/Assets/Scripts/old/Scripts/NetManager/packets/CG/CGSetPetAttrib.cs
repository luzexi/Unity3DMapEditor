using System;
using Network;
using GameUtil;
namespace Network.Packets
{
    enum ENUM_SET_PET_ATTRIB
    {
        SET_PET_ATTRIB_INVALID = -1,
        SET_PET_ATTRIB_NAME,
        SET_PET_ATTRIB_STR_INCREMENT,
        SET_PET_ATTRIB_CON_INCREMENT,
        SET_PET_ATTRIB_DEX_INCREMENT,
        SET_PET_ATTRIB_SPR_INCREMENT,
        SET_PET_ATTRIB_INT_INCREMENT,

        SET_PET_ATTRIB_NUMBERS
    };
    public class CGSetPetAttrib : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            m_GUID.writeToBuff(ref buff);
            buff.WriteShort(m_Flags.m_wFlags);
            buff.WriteByte(m_byNameSize);
            buff.Write(ref m_szName, m_byNameSize);
            buff.WriteInt(m_StrIncrement);
            buff.WriteInt(m_ConIncrement);
            buff.WriteInt(m_DexIncrement);
            buff.WriteInt(m_SprIncrement);
            buff.WriteInt(m_IntIncrement);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_SET_PETATTRIB;
        }

        public override int getSize()
        {
            return sizeof(int) * 5 + m_GUID.getSize() + sizeof(byte)*(m_byNameSize + 1) + sizeof(short);
        }
       
        public PET_GUID_t GUID
        {
            get { return m_GUID; }
            set { m_GUID = value; }
        }
        public Flag16 Flag
        {
            get { return m_Flags; }
            set { m_Flags = value; }
        }

        public int StrIncrement
        {
            get { return m_StrIncrement; }
            set { m_StrIncrement = value; }
        }

        public int ConIncrement
        {
            get { return m_ConIncrement; }
            set { m_ConIncrement = value; }
        }
        public int DexIncrement
        {
            get { return m_DexIncrement; }
            set { m_DexIncrement = value; }
        }

        public int SprIncrement
        {
            get { return m_SprIncrement; }
            set { m_SprIncrement = value; }
        }

        public int IntIncrement
        {
            get { return m_IntIncrement; }
            set { m_IntIncrement = value; }
        }
        public byte NameSize
        {
            get { return m_byNameSize; }
            set { m_byNameSize = value; }
        }

        public byte[] Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
        PET_GUID_t		m_GUID;							// ID
        GameUtil.Flag16 m_Flags = new GameUtil.Flag16();						// 每个位表示一个属性是否要刷新 ENUM_DETAIL_ATTRIB

		byte			m_byNameSize;					// 名称长度,不包括最后的'\0'
		byte[]			m_szName = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];	// 名称
		int				m_StrIncrement;					// 力量增量
		int				m_ConIncrement;					// 体力增量
		int 			m_DexIncrement;					// 身法增量
		int				m_SprIncrement;					// 灵气增量
		int 			m_IntIncrement;					// 定力增量
    };

    public class CGSetPetAttribFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGSetPetAttrib(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_SET_PETATTRIB; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 5 + PET_GUID_t.getMaxSize() + sizeof(byte) * (GAMEDEFINE.MAX_CHARACTER_NAME + 1) + sizeof(short);
        }
    };
}