////////////////////////////////////////////////
// 附加宠物属性定义
////////////////////////////////////////////////
using System;
using System.Runtime.InteropServices;
using Network;
using Network.Packets;
enum ENUM_PET_FOOD_TYPE
{
    PET_FOOD_TYPE_INVALID = 0,
    PET_FOOD_TYPE_MEAT,				//肉食宠粮
    PET_FOOD_TYPE_GRASS,			//草类宠粮
    PET_FOOD_TYPE_WORM,				//虫类宠粮
    PET_FOOD_TYPE_PADDY,			//谷类宠粮

    PET_FOOD_TYPE_NUMBER,
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _PET_PLACARD_ITEM
{
    public bool IsInvalid()
	{
		return (m_HumanGUID == MacroDefine.INVALID_ID)?(true):(false);
	}
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        m_szHumanName = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];
        m_szHumanGuildName = new byte[GAMEDEFINE.MAX_GUILD_NAME_SIZE];
        m_szMessage = new byte[GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE];
        if (buff.ReadUint(ref m_uCreateTime) != sizeof(uint)) return false;
        if (buff.ReadUint(ref m_HumanGUID) != sizeof(short)) return false;
        if (buff.Read(ref m_szHumanName, GAMEDEFINE.MAX_CHARACTER_NAME) != GAMEDEFINE.MAX_CHARACTER_NAME) return false;
        if (buff.ReadInt(ref m_nHumanLevel) != sizeof(uint)) return false;
        if (buff.Read(ref m_szHumanGuildName, GAMEDEFINE.MAX_GUILD_NAME_SIZE) != GAMEDEFINE.MAX_GUILD_NAME_SIZE) return false;
        if (buff.ReadInt(ref m_nHumanMenPai) != sizeof(int)) return false;
        if (!m_PetAttr.readFromBuff(ref buff)) return false;
        if (buff.Read(ref m_szMessage, GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE) != GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE) return false;
        return true;
    }

    public void CleanUp()
    {
        m_uCreateTime = 0;

        // 宠主信息
        m_HumanGUID = MacroDefine.INVALID_GUID;
      
        m_nHumanLevel = -1;
        m_nHumanMenPai = -1;

        // 宠物信息
        m_PetAttr.CleanUp();
    }

    static public int getMaxSize()
    {
        return sizeof(uint) * 2 +
               sizeof(int) +
               GAMEDEFINE.MAX_CHARACTER_NAME +
               GAMEDEFINE.MAX_GUILD_NAME_SIZE +
               GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE +
               _PET_DETAIL_ATTRIB.getMaxSize();
    }

    public int getSize()
    {
        return sizeof(uint) * 2 +
               sizeof(int) +
               GAMEDEFINE.MAX_CHARACTER_NAME +
               GAMEDEFINE.MAX_GUILD_NAME_SIZE +
               GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE +
               m_PetAttr.getSize();
    }
	// 创建信息
	public uint		m_uCreateTime;
	// 宠主信息
    public uint     m_HumanGUID;
    public byte[]   m_szHumanName;
    public int      m_nHumanLevel;
    public byte[]   m_szHumanGuildName;
    public int      m_nHumanMenPai;
	// 宠物信息
    public _PET_DETAIL_ATTRIB m_PetAttr;
	// 留言
    public byte[] m_szMessage;
};