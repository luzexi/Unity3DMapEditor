using UnityEngine;
using System.Collections;
using Network;
using System.Runtime.InteropServices;
using System;

public enum ENUM_SCRIPT_COMMAND
{
    SCRIPT_COMMAND_INVALID = -1,
    SCRIPT_COMMAND_EVENT_LIST_RESPONSE,			// 事件列表返回
    SCRIPT_COMMAND_MISSION_RESPONSE,			// 任务事件的查询返回
    SCRIPT_COMMAND_MISSION_REGIE,				// 漕运任务查询返回
    SCRIPT_COMMAND_MISSION_DEMAND_RESPONSE,		// 任务需求的查询返回
    SCRIPT_COMMAND_MISSION_CONTINUE_RESPONSE,	// 任务的继续按钮事件返回
    SCRIPT_COMMAND_MISSION_TIPS,				// 任务提示
    SCRIPT_COMMAND_TRADE,						// 交易
    SCRIPT_COMMAND_SKILL_STUDY,					// 技能学习
};

public struct SCRIPT_COMMAND
{
    public const int DEF_SCRIPT_STRING_LEN = (256);
    public const int MAX_EVENT_LIST_ITEM_COUNT = (16);
    public const int MAX_MISSION_TEXT_COUNT = (8);
    public const int MAX_MISSION_BONUS_COUNT = (16);
    public const int MAX_MISSION_DEMAND_COUNT = (8);
    public const int MAX_TRADE_ITEM_COUNT = (128);
    public const int MAX_SKILL_ITEM_COUNT = (128);
}

public class ScriptString
{
    public short      m_Size;
    public string   m_szString;

    public void Reset()
    {
        m_Size = 0;
        m_szString = "";
    }

    public uint GetBufSize()
    {
        uint usize = Convert.ToUInt32(sizeof(short) + m_Size * sizeof(char));
        return usize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadShort(ref m_Size) != sizeof(short))
	    {
            return false;
	    }

        if (m_Size > 0)
        {
            byte[] byarrayTemp = new byte[m_Size];
            if (iStream.Read(ref byarrayTemp, m_Size) == 0)
            {
                return false;
            }
            m_szString = EncodeUtility.Instance.GetUnicodeString(byarrayTemp);
        }
        else
        {
            m_Size = 0;
            m_szString = "";
        }

        return true;
    }

    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    if (oStream.WriteInt(m_Size) != sizeof(int))
    //    {
    //        return false;
    //    }
    //    if (m_Size > 0)
    //    {
    //        if (oStream.Write(ref m_szString, m_Size) == 0)
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}
}

// 事件列表 
public enum ENUM_EVENT_ITEM_TYPE
{
    EVENT_ITEM_TYPE_INVALID = -1,	// 无效
    EVENT_ITEM_TYPE_SECTION,		// 节点
    EVENT_ITEM_TYPE_SCRIPT_ID,		// 选项
    EVENT_ITEM_TYPE_TEXT,			// 文本
};

public class ScriptEventItem
{
    public ENUM_EVENT_ITEM_TYPE m_nType;
    public int                  m_state;
    public int                  m_index;
    public ScriptString         m_strString = new ScriptString();
    public int                  m_idScript;

    public void Reset()
    {
        m_nType = ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_INVALID;
        m_strString.Reset();
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(int);

        switch (m_nType)
        {
            case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT:
                uSize += m_strString.GetBufSize();
                break;
            case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SECTION:
                uSize += m_strString.GetBufSize();
                break;
            case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID:
                uSize += m_strString.GetBufSize();
                uSize += sizeof(int);
                uSize += sizeof(int);
                uSize += sizeof(int);
                break;
            default:
                break;
        }

        return uSize;
    }

    public void SetSection(ScriptString pstrSection)
    {
		m_nType	= ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SECTION;
		m_strString = pstrSection;
	}

	public void SetScriptID(int index,int state,int idScript, ScriptString pstrSection)
    {
		m_nType	= ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID;
		m_state = state;
		m_index = index;
		m_idScript = idScript;
		m_strString = pstrSection;
    }

	public void SetText(ScriptString pstrSection)
    {
		m_nType	= ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT;
		m_strString = pstrSection;
	}

	public bool Read(ref NetInputBuffer iStream)
    {
        int nType = Convert.ToInt32(m_nType);
        iStream.ReadInt(ref nType);
        m_nType = (ENUM_EVENT_ITEM_TYPE)nType;
        switch (m_nType)
        {
            case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT:
                if(!m_strString.Read(ref iStream))
                {
                    return false;
                }
                break;
            case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SECTION:
                if(!m_strString.Read(ref iStream))
                {
                    return false;
                }
                break;
            case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID:
                if (iStream.ReadInt(ref m_index) != sizeof(int))
                {
                    return false;
                }
                if (iStream.ReadInt(ref m_state) != sizeof(int))
                {
                    return false;
                }
                if (iStream.ReadInt(ref m_idScript) != sizeof(int))
                {
                    return false;
                }
                if(!m_strString.Read(ref iStream))
                {
                    return false;
                }
                break;
            default:
                break;
        }

        return true;
    }

    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    int nType = Convert.ToInt32(m_nType);
    //    if (oStream.WriteInt(nType) != sizeof(int))
    //    {
    //        return false;
    //    }

    //    switch (m_nType)
    //    {
    //        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT:
    //            if(!m_strString.Write(ref oStream))
    //            {
    //                return false;
    //            }
    //            break;
    //        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SECTION:
    //            if(!m_strString.Write(ref oStream))
    //            {
    //                return false;
    //            }
    //            break;
    //        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID:
    //            if(oStream.WriteInt(m_index) != sizeof(int))
    //            {
    //                return false;
    //            }
    //            if(oStream.WriteInt(m_state) != sizeof(int))
    //            {
    //                return false;
    //            }
    //            if(oStream.WriteInt(m_idScript) != sizeof(int))
    //            {
    //                return false;
    //            }
    //            if(!m_strString.Write(ref oStream))
    //            {
    //                return false;
    //            }
    //            break;
    //        default:
    //            break;
    //    }

    //    return true;
    //}
}

public class ScriptParam_EventList
{
    public uint                 m_idNPC;
    public byte                 m_yItemCount;
    public ScriptEventItem[]    m_seiItem = new ScriptEventItem[SCRIPT_COMMAND.MAX_EVENT_LIST_ITEM_COUNT];

    public void Reset()
    {
        m_idNPC = MacroDefine.UINT_MAX;
        m_yItemCount = 0;
        for (int i = 0; i < SCRIPT_COMMAND.MAX_EVENT_LIST_ITEM_COUNT; i++)
        {
            m_seiItem[i].Reset();
        }
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(uint);
        uSize += sizeof(byte);
        for (byte i = 0; i < m_yItemCount; i++)
        {
            uSize += m_seiItem[i].GetBufSize();
        }

        return uSize;
    }

    public void AddItem(ScriptEventItem pItem)
    {
		if (m_yItemCount < SCRIPT_COMMAND.MAX_EVENT_LIST_ITEM_COUNT)
		{
			m_seiItem[m_yItemCount] = pItem;
			m_yItemCount++;
		}
	}

    public ScriptEventItem GetItem( byte yIndex )
    {
        return (yIndex < m_yItemCount) ? (m_seiItem[yIndex]) : (null);
	}

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadUint(ref m_idNPC) != sizeof(uint))
        {
            return false;
        }
        if (iStream.ReadByte(ref m_yItemCount) != sizeof(byte))
        {
            return false;
        }
        for (byte i = 0; i < m_yItemCount; i++)
        {
            m_seiItem[i] = new ScriptEventItem();
            if (!m_seiItem[i].Read(ref iStream))
            {
                return false;
            }
        }

        return true;
    }

    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    if (oStream.WriteUint(m_idNPC) != sizeof(uint))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteByte(m_yItemCount) != sizeof(byte))
    //    {
    //        return false;
    //    }
    //    for (byte i = 0; i < m_yItemCount; i++)
    //    {
    //        if (!m_seiItem[i].Write(ref oStream))
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}
}

// 任务信息
public class SMissionBonusItem
{
	public int      m_yCount;
	public uint		m_uItemID;

	public void Reset()
    {
		m_yCount	= 0;
		m_uItemID	= MacroDefine.UINT_MAX;
	}

	public uint GetBufSize()
    {
        uint uBufSize = sizeof(int) + sizeof(uint);
        return uBufSize;
    }

	public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadInt(ref m_yCount) != sizeof(int))
        {
            return false;
        }

        if (iStream.ReadUint(ref m_uItemID) != sizeof(uint))
        {
            return false;
        }
        
        return true;
    }

	public bool Write(ref NetOutputBuffer oStream)
    {
        if(oStream.WriteInt(m_yCount) != sizeof(int))
        {
            return false;
        }

        if (oStream.WriteUint(m_uItemID) != sizeof(uint))
        {
            return false;
        }

        return true;
    }
}

//任务需要杀死的NPC
public class SMissionDemandKill
{
    public byte m_yCount;
    public uint m_uNPCID;

	public void Reset()
    {
		m_yCount	= 0;
		m_uNPCID	= MacroDefine.UINT_MAX;
	}

	public uint GetBufSize()
    {
        uint uBufSize = sizeof(byte) + sizeof(uint);
        return uBufSize;
    }

	public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadByte(ref m_yCount) != sizeof(byte))
        {
            return false;
        }
        
        if (iStream.ReadUint(ref m_uNPCID) != sizeof(uint))
        {
            return false;
        }

        return true;
    }

	public bool Write(ref NetOutputBuffer oStream)
    {
        if(oStream.WriteByte(m_yCount) != sizeof(byte))
        {
            return false;
        }
        
        if (oStream.WriteUint(m_uNPCID) != sizeof(uint))
        {
            return false;
        }

        return true;
    }
}

public enum ENUM_MISSION_BONUS_TYPE
{
    MISSION_BONUS_TYPE_INVALID = -1,
    MISSION_BONUS_TYPE_MONEY,			// 金钱
    MISSION_BONUS_TYPE_ITEM,			// 物品
    MISSION_BONUS_TYPE_ITEM_RAND,		// 随机物品
    MISSION_BONUS_TYPE_ITEM_RADIO,		// 多选1物品
    MISSION_BONUS_TYPE_EXP,				// 奖励经验
};

// 奖励的结构
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class SMissionBonus
{
	public ENUM_MISSION_BONUS_TYPE      m_nType;		// ENUM_MISSION_BONUS_TYPE
    public uint                         m_uMoney;
    public uint					        m_uExp;
    public SMissionBonusItem            m_ItemBonus = new SMissionBonusItem();

	public void Reset()
    {
		m_nType		= ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_INVALID;
	}
    
// 	public void SetMoney(uint uMoney)
//     {
// 		m_nType		= ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY;
// 		m_uMoney	= m_data0;
// 	}
// 
//     public void SetExp(uint uExp)
//     {
//         m_nType     = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP;
//         m_uExp      = m_data0;
//     }
// 
// 	public void SetItem(uint uItemID,int yItemCount)
//     {
// 		m_nType		= ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM;
//         m_ItemBonus.m_yCount    = Convert.ToInt32(m_data0);
//         m_ItemBonus.m_uItemID   = m_data1;
// 	}

// 	public void SetItemRand()
//     {
// 		m_nType		= ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND;
// 	}
// 
// 	public void SetItemRadio(uint uItemID, int yItemCount)
//     {
// 		m_nType		= ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO;
// 		m_ItemBonus.m_yCount    = Convert.ToInt32(m_data0);
//         m_ItemBonus.m_uItemID   = m_data1;
// 	}

	public uint GetBufSize()
    {
        uint uSize;
        uSize = sizeof(ENUM_MISSION_BONUS_TYPE);
        switch (m_nType)
	    {
		    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY:
                uSize += sizeof(uint);
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM:
                uSize += m_ItemBonus.GetBufSize();
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND:
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO:
                uSize += m_ItemBonus.GetBufSize();
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP:
                uSize += sizeof(uint);
                break;
            default:
                break;
	    }

        return uSize;
    }

	public bool Read(ref NetInputBuffer iStream)
    {
        int nType = Convert.ToInt32(m_nType);
        if (iStream.ReadInt(ref nType) != sizeof(int))
        {
            return false;
        }
        m_nType = (ENUM_MISSION_BONUS_TYPE)nType;

        switch (m_nType)
        {
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY:
                if(iStream.ReadUint(ref m_uMoney) != sizeof(uint))
                {
                    return false;
                }
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM:
                m_ItemBonus = new SMissionBonusItem();
                if (!m_ItemBonus.Read(ref iStream))
                {
                    return false;
                }
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND:
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO:
                m_ItemBonus = new SMissionBonusItem();
                if (!m_ItemBonus.Read(ref iStream))
                {
                    return false;
                }
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP:
                if (iStream.ReadUint(ref m_uExp) != sizeof(uint))
                {
                    return false;
                }
                break;
            default:
                break;
        }

        return true;
    }

	public bool Write(ref NetOutputBuffer oStream)
    {
//         int nType = Convert.ToInt32(m_nType);
//         if (oStream.WriteInt(nType) != sizeof(int))
//         {
//             return false;
//         }
// 
//         switch (m_nType)
//         {
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY:
//                 if (oStream.WriteUint(m_uMoney) != sizeof(uint))
//                 {
//                     return false;
//                 }
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM:
//                 if (!m_ItemBonus.Write(ref oStream))
//                 {
//                     return false;
//                 }
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND:
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO:
//                 if (!m_ItemBonus.Write(ref oStream))
//                 {
//                     return false;
//                 }
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP:
//                 if (oStream.WriteUint(m_uExp) != sizeof(uint))
//                 {
//                     return false;
//                 }
//                 break;
//             default:
//                 break;
//         }

        return true;
    }
};

public class ScriptParam_MissionInfo
{
    public uint             m_idNPC;			// 向服务器返回时的参数
    public int              m_idScript;			// 用于向服务器返回操作，如：接受任务（传到服务器的不是任务ID，而是脚本ID）
    public int              m_idMission;		// 用于客户端显示信息的查询
    public byte             m_yTextCount;
    public ScriptString[]   m_aText = new ScriptString[SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT];
    public byte             m_yBonusCount;
    public SMissionBonus[]  m_aBonus = new SMissionBonus[SCRIPT_COMMAND.MAX_MISSION_BONUS_COUNT];
    public int              m_IsPush;           //原本为BOOL值

    public void Reset()
    {
        m_idNPC         = MacroDefine.UINT_MAX;
        m_idScript      = MacroDefine.INVALID_ID;
        m_idMission     = MacroDefine.INVALID_ID;
        m_yTextCount    = 0;
        m_yBonusCount   = 0;
        m_IsPush        = 0;

        for (int i = 0; i < SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT; i++)
        {
            m_aText[i].Reset();
        }

        for (int i = 0; i < SCRIPT_COMMAND.MAX_MISSION_BONUS_COUNT; i++)
        {
            m_aBonus[i].Reset();
        }
    }

    public void AddText(ScriptString pText)
    {
        if (m_yTextCount < SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT)
        {
            m_aText[m_yTextCount] = pText;
            m_yTextCount++;
        }
    }

    public void AddBouns(SMissionBonus pBouns)
    {
        if (m_yBonusCount < SCRIPT_COMMAND.MAX_MISSION_BONUS_COUNT)
        {
            m_aBonus[m_yBonusCount] = pBouns;
            m_yBonusCount++;
        }
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(uint);
        uSize += sizeof(int);
        uSize += sizeof(int);
        uSize += sizeof(int);
        uSize += sizeof(byte);

        for (byte i = 0; i < m_yTextCount; i++)
        {
            uSize += (uint)m_aText[i].GetBufSize();
        }

        uSize += sizeof(byte);

        for (byte i = 0; i < m_yBonusCount; i++)
        {
            uSize += m_aBonus[i].GetBufSize();
        }

        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadUint(ref m_idNPC) != sizeof(uint))
        {
            return false;
        }
        
        if (iStream.ReadInt(ref m_idScript) != sizeof(int))
        {
            return false;
        }
        
        if (iStream.ReadInt(ref m_idMission) != sizeof(int))
        {
            return false;
        }
        
        if (iStream.ReadByte(ref m_yTextCount) != sizeof(byte))
        {
            return false;
        }
        
        if (iStream.ReadInt(ref m_IsPush) != sizeof(int))
        {
            return false;
        }

        for (byte i = 0; i < m_yTextCount; i++)
        {
			m_aText[i] = new ScriptString();	
            if (!m_aText[i].Read(ref iStream))
            {
                return false;
            }
        }

        if (iStream.ReadByte(ref m_yBonusCount) != sizeof(byte))
        {
            return false;
        }

        for (byte i = 0; i < m_yBonusCount; i++)
        {
			m_aBonus[i] = new SMissionBonus();
            if (!m_aBonus[i].Read(ref iStream))
            {
                return false;
            }
        }

        return true;
    }

    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    if (oStream.WriteUint(m_idNPC) != sizeof(uint))
    //    {
    //        return false;
    //    }
        
    //    if (oStream.WriteInt(m_idScript) != sizeof(int))
    //    {
    //        return false;
    //    }
        
    //    if (oStream.WriteInt(m_idMission) != sizeof(int))
    //    {
    //        return false;
    //    }

    //    if (oStream.WriteInt(m_IsPush) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    if (oStream.WriteByte(m_yTextCount) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    for (byte i = 0; i < m_yTextCount; i++)
    //    {
    //        if (!m_aText[i].Write(ref oStream))
    //        {
    //            return false;
    //        }
    //    }

    //    if (oStream.WriteByte(m_yBonusCount) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    for (byte i = 0; i < m_yBonusCount; i++)
    //    {
    //        if (!m_aBonus[i].Write(ref oStream))
    //        {
    //            return false;
    //        }
    //    }

    //    return false;
    //}
}

public class ScriptParam_MissionDemandInfo
{
    public enum ScriptMissionDamandInfo
	{
		MISSION_NODONE	=0,
		MISSION_DONE,
		MISSION_CHECK,
	};
    public uint                 m_idNPC;			// 向服务器返回时的参数
    public int                  m_idScript;			// 向服务器返回时的参数
    public int                  m_idMission;		// 用于客户端显示信息的查询
    public int                  m_bDone;			// 完成标志 0:未完成，1:完成，2:需要二次判定
    public byte                 m_yTextCount;
    public ScriptString[]       m_aText = new ScriptString[SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT];
    public byte                 m_yDemandCount;
    public SMissionBonusItem[]  m_aDemandItem = new SMissionBonusItem[SCRIPT_COMMAND.MAX_MISSION_DEMAND_COUNT];

    public void Reset()
    {
        m_idNPC = MacroDefine.UINT_MAX;
        m_idScript = MacroDefine.INVALID_ID;
        m_idMission = MacroDefine.INVALID_ID;
        m_bDone = 0;
        m_yTextCount = 0;

        for (int i = 0; i < SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT; i++)
        {
            m_aText[i].Reset();
        }

        m_yDemandCount = 0;

        for (int i = 0; i < SCRIPT_COMMAND.MAX_MISSION_DEMAND_COUNT; i++)
        {
            m_aDemandItem[i].Reset();
        }
    }

    public void AddText(ScriptString pText)
    {
        if (m_yTextCount < SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT)
        {
            m_aText[m_yTextCount] = pText;
            m_yTextCount++;
        }        
    }

    public void AddDemandItem(SMissionBonusItem pDemandItem)
    {
        if (m_yDemandCount < SCRIPT_COMMAND.MAX_MISSION_DEMAND_COUNT)
        {
            m_aDemandItem[m_yDemandCount] = pDemandItem;
            m_yDemandCount++;
        }
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(uint);
        uSize += sizeof(int);
        uSize += sizeof(int);
        uSize += sizeof(int);
        uSize += sizeof(byte);
        for (byte i = 0; i < m_yTextCount; i++)
        {
            uSize += m_aText[i].GetBufSize();
        }
        uSize += sizeof(byte);
        for (byte i = 0; i < m_yDemandCount; i++)
        {
            uSize += m_aDemandItem[i].GetBufSize();
        }

        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadUint(ref m_idNPC) != sizeof(uint))
        {
            return false;
        }
        if (iStream.ReadInt(ref m_idScript) != sizeof(int))
        {
            return false;
        }
        if (iStream.ReadInt(ref m_idMission) != sizeof(int))
        {
            return false;
        }
        if (iStream.ReadInt(ref m_bDone) != sizeof(int))
        {
            return false;
        }
        if (iStream.ReadByte(ref m_yTextCount) != sizeof(byte))
        {
            return false;
        }

        for (int i = 0; i < m_yTextCount; i++)
        {
            m_aText[i] = new ScriptString();
            m_aText[i].Read(ref iStream);
        }

        if (iStream.ReadByte(ref m_yDemandCount) != sizeof(byte))
        {
            return false;
        }

        for (int i = 0; i < m_yDemandCount; i++)
        {
            m_aDemandItem[i] = new SMissionBonusItem();
            m_aDemandItem[i].Read(ref iStream);
        }

        return true;
    }

    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    if (oStream.WriteUint(m_idNPC) != sizeof(uint))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteInt(m_idScript) != sizeof(int))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteInt(m_idMission) != sizeof(int))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteInt(m_bDone) != sizeof(int))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteByte(m_yTextCount) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    for (int i = 0; i < m_yTextCount; i++)
    //    {
    //        m_aText[i].Write(ref oStream);
    //    }

    //    if (oStream.WriteByte(m_yDemandCount) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    for (int i = 0; i < m_yDemandCount; i++)
    //    {
    //        m_aDemandItem[i].Write(ref oStream);
    //    }

    //    return false;
    //}
}

// 任务的继续按钮事件返回(返回任务的奖励等等)
public class ScriptParam_MissionContinueInfo
{
    public uint             m_idNPC;                // 向服务器返回时的参数
    public int              m_idScript;	            // 向服务器返回时的参数
    public int              m_idMission;            // 用于客户端显示信息的查询
    public byte             m_yTextCount;           // 说明性文本的数目
    public ScriptString[]   m_aText = new ScriptString[SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT];      // 说明性文本
    public byte             m_yBonusCount;						                                    // 任务道具奖励的数目
    public SMissionBonus[]  m_aBonus = new SMissionBonus[SCRIPT_COMMAND.MAX_MISSION_BONUS_COUNT];   // 任务道具列表

    public void Reset()
    {
        m_idNPC         = MacroDefine.UINT_MAX;
        m_idScript      = MacroDefine.INVALID_ID;
        m_idMission     = MacroDefine.INVALID_ID;
        m_yTextCount    = 0;
        m_yBonusCount   = 0;

        for (int i = 0; i < SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT; i++)
        {
            m_aText[i].Reset();
        }

        for (int i = 0; i < SCRIPT_COMMAND.MAX_MISSION_BONUS_COUNT; i++)
        {
            m_aBonus[i].Reset();
        }
    }

    public void AddText(ScriptString pstrText)
    {
		if (m_yTextCount < SCRIPT_COMMAND.MAX_MISSION_TEXT_COUNT)
		{
			m_aText[m_yTextCount] = pstrText;
			m_yTextCount++;
		}
	}

    public void AddBonus(SMissionBonus pBonus)
    {
        if (m_yBonusCount < SCRIPT_COMMAND.MAX_MISSION_DEMAND_COUNT)
        {
            m_aBonus[m_yBonusCount] = pBonus;
            m_yBonusCount++;
        }
    }

    public uint GetSelectItemID(int nIndex)
    {
        byte i;
        for (i = 0; i < m_yBonusCount; i++)
        {
            if (ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO == m_aBonus[i].m_nType)
                break;
        }
        return m_aBonus[i + nIndex].m_ItemBonus.m_uItemID;
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(uint);
        uSize += sizeof(int);
        uSize += sizeof(int);
        uSize += sizeof(byte);
        for (byte i = 0; i < m_yTextCount; i++)
        {
            uSize += m_aText[i].GetBufSize();
        }
        uSize += sizeof(byte);
        for (byte i = 0; i < m_yBonusCount; i++)
        {
            uSize += m_aBonus[i].GetBufSize();
        }

        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadUint(ref m_idNPC) != sizeof(uint))
        {
            return false;
        }
        if (iStream.ReadInt(ref m_idScript) != sizeof(int))
        {
            return false;
        }
        if (iStream.ReadInt(ref m_idMission) != sizeof(int))
        {
            return false;
        }
        if (iStream.ReadByte(ref m_yTextCount) != sizeof(byte))
        {
            return false;
        }

        for (byte i = 0; i < m_yTextCount; i++)
        {
            m_aText[i] = new ScriptString();
            if (!m_aText[i].Read(ref iStream))
            {
                return false;
            }
        }

        if (iStream.ReadByte(ref m_yBonusCount) != sizeof(byte))
        {
            return false;
        }

        for (byte i = 0; i < m_yBonusCount; i++)
        {
            m_aBonus[i] = new SMissionBonus();
            if (!m_aBonus[i].Read(ref iStream))
            {
                return false;
            }
        }

        return true;
    }

    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    if (oStream.WriteUint(m_idNPC) != sizeof(uint))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteInt(m_idScript) != sizeof(int))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteInt(m_idMission) != sizeof(int))
    //    {
    //        return false;
    //    }
    //    if (oStream.WriteByte(m_yTextCount) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    for (int i = 0; i < m_yTextCount; i++)
    //    {
    //        if (!m_aText[i].Write(ref oStream))
    //        {
    //            return false;
    //        }
    //    }

    //    if (oStream.WriteByte(m_yBonusCount) != sizeof(byte))
    //    {
    //        return false;
    //    }

    //    for (int i = 0; i < m_yBonusCount; i++)
    //    {
    //        if (!m_aBonus[i].Write(ref oStream))
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}
}

// 任务提示
public class ScriptParam_MissionTips
{
    public ScriptString m_strText = new ScriptString();

	public void Reset()
    {
		m_strText.Reset();
	}

    public uint GetBufSize()
    {
        uint uSize = m_strText.GetBufSize();
        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (!m_strText.Read(ref iStream))
        {
            return false;
        }
        return true;
    }
    
    //public bool Write(ref NetOutputBuffer oStream)
    //{
    //    if (!m_strText.Write(ref oStream))
    //    {
    //        return false;
    //    }
    //    return true;
    //}
}

// 交易
public class STradeItem
{
    public uint         m_uDataID;                      // Excel中的Index
    public _ITEM_TYPE   m_typeItem = new _ITEM_TYPE();  // 道具类型
    public byte         m_yCount;                       // 道具数量

    public void Reset()
    {
        m_uDataID = MacroDefine.UINT_MAX;
        m_typeItem.CleanUp();
        m_yCount = 0;
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(uint) + sizeof(uint) + sizeof(byte);
        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadUint(ref m_uDataID) != sizeof(uint))
        {
            return false;
        }
        if (iStream.ReadUint(ref m_typeItem.uTemp) != sizeof(uint))
        {
            return false;
        }
        if (iStream.ReadByte(ref m_yCount) != sizeof(byte))
        {
            return false;
        }
        return true;
    }
    
    public bool Write(ref NetOutputBuffer oStream)
    {
        if (oStream.WriteUint(m_uDataID) != sizeof(uint))
        {
            return false;
        }
        if (oStream.WriteUint(m_typeItem.FourToOne()) != sizeof(uint))
        {
            return false;
        }
        if (oStream.WriteByte(m_yCount) != sizeof(byte))
        {
            return false;
        }
        return true;
    }
}

// 交易消息所用到的结构
public class ScriptParam_Trade
{
    public byte         m_yItemCount;   // 道具数目
    public STradeItem[] m_aTradeItem = new STradeItem[SCRIPT_COMMAND.MAX_TRADE_ITEM_COUNT];	    // 道具列表
    public byte         m_bRepair;      // 是否有修理功能

	public void Reset()
    {
        m_yItemCount = 0;
        for (int i = 0; i < SCRIPT_COMMAND.MAX_TRADE_ITEM_COUNT; i++)
		{
            m_aTradeItem[i].Reset();
		}
	}

    public uint GetBufSize()
    {
        uint uSize = sizeof(byte);
        for (byte i = 0; i < m_yItemCount; i++)
        {
            uSize += m_aTradeItem[i].GetBufSize();
        }
        uSize += sizeof(byte);

        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadByte(ref m_yItemCount) != sizeof(byte))
        {
            return false;
        }

        for (byte i = 0; i < m_yItemCount; i++)
        {
            if (!m_aTradeItem[i].Read(ref iStream))
            {
                return false;
            }
        }

        if (iStream.ReadByte(ref m_bRepair) != sizeof(byte))
        {
            return false;
        }

        return true;
    }

    public bool Write(ref NetOutputBuffer oStream)
    {
        if (oStream.WriteByte(m_yItemCount) != sizeof(byte))
        {
            return false;
        }
        for (byte i = 0; i < m_yItemCount; i++)
        {
            if (!m_aTradeItem[i].Write(ref oStream))
            {
                return false;
            }
        }
        if (oStream.WriteByte(m_bRepair) != sizeof(byte))
        {
            return false;
        }
        return true;
    }
}

//技能学习消息的结构体
public class SSkillItem
{
    public int  nSkillID;
    public byte nLevel;
    public byte SkillType;

    public void Init()
    {
        nSkillID    = MacroDefine.INVALID_ID;
        nLevel      = 0;
        SkillType   = 0;
    }

	public uint GetBufSize()
    {
        uint uSize = sizeof(int);
        uSize += sizeof(byte);
        uSize += sizeof(byte);
        return uSize;
    }

	public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadInt(ref nSkillID) != sizeof(int))
        {
            return false;
        }
        if (iStream.ReadByte(ref nLevel) != sizeof(byte))
        {
            return false;
        }
        if (iStream.ReadByte(ref SkillType) != sizeof(byte))
        {
            return false;
        }
        return true;
    }

	public bool Write(ref NetOutputBuffer oStream)
    {
        if (oStream.WriteInt(nSkillID) != sizeof(int))
        {
            return false;
        }
        if (oStream.WriteByte(nLevel) != sizeof(byte))
        {
            return false;
        }
        if (oStream.WriteByte(SkillType) != sizeof(byte))
        {
            return false;
        }
        return true;
    }
}

public class ScriptParam_SkillStudy
{
    public byte         m_yStudyCount;  // 技能数目
    public SSkillItem[] m_aSkillItem = new SSkillItem[SCRIPT_COMMAND.MAX_SKILL_ITEM_COUNT];  // 技能列表
    public int          m_nReserve;

    public void Reset()
    {
        m_yStudyCount = 0;
        m_nReserve = MacroDefine.INVALID_ID;
        for (int i = 0; i < SCRIPT_COMMAND.MAX_SKILL_ITEM_COUNT; i++)
        {
            m_aSkillItem[i].Init();
        }
    }

    public uint GetBufSize()
    {
        uint uSize = sizeof(byte);
        for (int i = 0; i < SCRIPT_COMMAND.MAX_SKILL_ITEM_COUNT; i++)
        {
            uSize += m_aSkillItem[i].GetBufSize();
        }
        return uSize;
    }

    public bool Read(ref NetInputBuffer iStream)
    {
        if (iStream.ReadByte(ref m_yStudyCount) != sizeof(byte))
        {
            return false;
        }
        for (byte i = 0; i < m_yStudyCount; i++)
        {
            if (!m_aSkillItem[i].Read(ref iStream))
            {
                return false;
            }
        }
        if (iStream.ReadInt(ref m_nReserve) != sizeof(int))
        {
            return false;
        }
        return true;
    }

    public bool Write(ref NetOutputBuffer oStream)
    {
        if (oStream.WriteByte(m_yStudyCount) != sizeof(byte))
        {
            return false;
        }
        for (byte i = 0; i < m_yStudyCount; i++)
        {
            if (!m_aSkillItem[i].Write(ref oStream))
            {
                return false;
            }
        }
        if (oStream.WriteInt(m_nReserve) != sizeof(int))
        {
            return false;
        }
        return true;
    }
}