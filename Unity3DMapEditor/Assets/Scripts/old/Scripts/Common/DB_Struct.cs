using System;
using System.Runtime.InteropServices;

using Network;
using Network.Packets;

// HighSection是玩家的GUID,LowSection是当前的系统时间
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct PET_GUID_t:ClassCanbeSerialized
{
    public uint m_uHighSelection;
    public uint m_uLowSelection;
    public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteUint(m_uHighSelection);
        buff.WriteUint(m_uLowSelection);
        return getSize();
    }
    public int getSize()
    {
        return sizeof(uint)* 2;
    }

    public static int getMaxSize()
    {
        return sizeof(uint) * 2;
    }
    // 将byte流转换为包内容
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (buff.ReadUint(ref m_uHighSelection) != sizeof(uint)) return false;
        if (buff.ReadUint(ref m_uLowSelection) != sizeof(uint)) return false;
        return true;
    }

    public bool IsNull()
	{
        if (m_uHighSelection == 0 && m_uLowSelection == 0)
		{
			return true;
		}
		return false;
	}
    public void Reset()
	{
        m_uHighSelection = 0;
        m_uLowSelection = 0;
	}


    //override operate
    public static bool operator==(PET_GUID_t lV, PET_GUID_t rV)  
	{
		if (lV.m_uHighSelection == rV.m_uHighSelection &&
			lV.m_uLowSelection == rV.m_uLowSelection)
		{
			return true;
		}
		return false;
	}

	public static bool operator!=( PET_GUID_t lV,  PET_GUID_t rV) 
	{
		if (lV.m_uHighSelection != rV.m_uHighSelection ||
			lV.m_uLowSelection != rV.m_uLowSelection)
		{
			return true;
		}
		return false;
	}
    public override bool Equals(object obj)
    {
        if(this.GetType() == obj.GetType())
        {
            PET_GUID_t petID = (PET_GUID_t)obj;
            return petID == this;
        }

        return base.Equals(obj);
    }
}
// 阵营数据
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _CAMP_DATA : ClassCanbeSerialized
{
	public short	m_nCampID;
    public byte m_nPKModeID;	//// PK MODE [8/17/2011 zzh+]
    public uint m_uActiveFlags;
    public uint m_uRelationFlags;

    //public _CAMP_DATA( )
    //{
    //    m_nCampID			= -1;
    //    m_uActiveFlags		= 0;
    //    m_uRelationFlags	= 0;
    //    m_nPKModeID = (byte)PK_MODE.PK_MODE_PEACE;//// PK MODE [8/17/2011 zzh+]
    //}

    // 取得包大小
    public int getSize(){
        return sizeof(short) + sizeof(byte) + sizeof(uint) * 2;
    }

    // 将包内容转换为byte流 return byte流长度
    public int writeToBuff(ref NetOutputBuffer buff){

        return getSize();
    }

    // 将byte流转换为包内容
    public bool readFromBuff(ref NetInputBuffer buff){
        if (buff.ReadShort(ref m_nCampID) != sizeof(short)) return false;
        if (buff.ReadByte(ref m_nPKModeID) != sizeof(byte)) return false;
        if (buff.ReadUint(ref m_uActiveFlags) != sizeof(uint)) return false;
        if (buff.ReadUint(ref m_uRelationFlags) != sizeof(uint)) return false;

        return true;
    }

    public static int getMaxSize() {
        return sizeof(short) + sizeof(byte) + sizeof(uint) * 2; 
    }

	public void CleanUp( )
	{
		m_nCampID			= -1;
		m_uActiveFlags		= 0;
		m_uRelationFlags	= 0;
		m_nPKModeID =(byte)PK_MODE.PK_MODE_PEACE;;//// PK MODE [8/17/2011 zzh+]
	}

	public bool IsActiveFlag_Dirty( int nID )
	{
		if ( nID < 0 || nID >= 32 )
		{
			return false;
		}
		return (m_uActiveFlags & (1<<nID)) != 0;
	}

	void SetActiveFlag_Dirty( int nID, bool bDirty )
	{
		if ( nID < 0 || nID >= 32 )
		{
			return ;
		}
		if ( bDirty )
			m_uActiveFlags |= (uint)(1<<nID);
		else
			m_uActiveFlags &= (uint)~(1<<nID);
	}

	bool IsRelationFlag_Dirty( int nID )
	{
		if ( nID < 0 || nID >= 32 )
		{
			return false;
		}
		return (m_uRelationFlags & (1<<nID)) != 0;
	}

	void SetRelationFlag_Dirty( int nID, bool bDirty )
	{
		if ( nID < 0 || nID >= 32 )
		{
			return ;
		}
		if ( bDirty )
			m_uRelationFlags |= (uint)(1<<nID);
		else
			m_uRelationFlags &= (uint)~(1<<nID);
	}

	public static bool operator==( _CAMP_DATA lhs, _CAMP_DATA rhs )
	{
		if ( lhs.m_nCampID != rhs.m_nCampID
			|| lhs.m_nPKModeID != rhs.m_nPKModeID //// PK MODE [8/17/2011 zzh+]
			|| lhs.m_uActiveFlags != rhs.m_uActiveFlags
			|| lhs.m_uRelationFlags != rhs.m_uRelationFlags )
		{
			return false;
		}
		return true;
	}

	public static bool operator != (  _CAMP_DATA lhs, _CAMP_DATA rhs )
	{
		if ( lhs.m_nCampID != rhs.m_nCampID
			|| lhs.m_nPKModeID != rhs.m_nPKModeID //// PK MODE [8/17/2011 zzh+]
			|| lhs.m_uActiveFlags != rhs.m_uActiveFlags
			|| lhs.m_uRelationFlags != rhs.m_uRelationFlags )
		{
			return true;
		}
		return false;
	}
    public override bool Equals(object obj)
    {
        if(obj == null) return false;

        _CAMP_DATA c = (_CAMP_DATA)obj;
        if((object)c == null) return false;

        return (m_nCampID == c.m_nCampID) && (m_nPKModeID == c.m_nPKModeID) &&
            (m_uRelationFlags == c.m_uRelationFlags) && (m_uActiveFlags == c.m_uActiveFlags);

       
    }
    public override int GetHashCode()
    {
        //return m_nCampID ^ m_nPKModeID ^ m_uActiveFlags ^ m_uRelationFlags;
        return base.GetHashCode();
    }


}

public class _IMPACT_LIST
{
	public byte			    m_Count;
	public _OWN_IMPACT[]	m_aImpacts = new _OWN_IMPACT[GAMEDEFINE.MAX_IMPACT_NUM];

	public _IMPACT_LIST( )
	{
		CleanUp();
	}
	public void	CleanUp()
    {
        m_Count = 0;
    }
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class _OWN_MISSION
{
	public int		m_idMission;
    public int      m_idScript;
	// 0x0000|PetChanged事件|ItemChanged事件|EnterZone事件|KillObject事件|
    public byte     m_yFlags;		

    // 0x0000|任务追踪|
    public byte     m_logicFlags;               // Allan 17/1/2011 

    private object[] m_aParam = new object[GAMEDEFINE.MAX_MISSION_PARAM_NUM];

    public void SetValue<Type>(int index, Type fValue)
    {
        m_aParam[index] = fValue;
    }

    public T GetValue<T>(int index)
    {
        try
        {
            T value = (T)m_aParam[index];
            return value;
        }
        catch (InvalidCastException ex)
        {
            LogManager.LogError(ex.ToString());
            throw ex;
        }
    }

    // 取得包大小
    public int getSize()
    {
        return sizeof(int) * +sizeof(uint) * GAMEDEFINE.MAX_MISSION_PARAM_NUM + sizeof(byte) * 2;
    }

    // 将包内容转换为byte流 return byte流长度
    public int writeToBuff(ref NetOutputBuffer buff)
    {
        return getSize();
    }

    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (buff.ReadInt(ref m_idMission) != sizeof(int)) return false;
        if (buff.ReadInt(ref m_idScript) != sizeof(int)) return false;
        if (buff.ReadByte(ref m_yFlags) != sizeof(byte)) return false;
        if (buff.ReadByte(ref m_logicFlags) != sizeof(byte)) return false;

        int n = -1;
        for (int i = 0; i < GAMEDEFINE.MAX_MISSION_PARAM_NUM; i++)
        {
            if (buff.ReadInt(ref n) != sizeof(int))
            {
                return false;
            }
            m_aParam[i] = (object)n;
        }

        return true;
    }

    public static int getMaxSize()
    {
        return sizeof(int) * +sizeof(uint) * GAMEDEFINE.MAX_MISSION_PARAM_NUM + sizeof(byte) * 2;
    }

	public void Cleanup()
	{
		m_idScript	    = MacroDefine.INVALID_ID;
		m_idMission     = MacroDefine.INVALID_ID;
		m_yFlags	    = 0;
        m_logicFlags    = 0;
        for (int i = 0; i < GAMEDEFINE.MAX_MISSION_PARAM_NUM; i++)
        {
            m_aParam[i] = -1;
        }
	}

    public bool IsActiveMission()
    {
        if ((m_idMission != MacroDefine.INVALID_ID) &&
            (m_idScript != MacroDefine.INVALID_ID))
            return true;
        else
            return false;
    }

    //kill obj event
    public void SetFlags_KillObject(bool bSet)
    {
        if (bSet)
        {
           m_yFlags |= 1;
        }
        else
        {
            m_yFlags &= 0xfe;
        }
    }

    public bool IsFlags_KillObject()
    {
        return (m_yFlags & 0x01) != 0;
    }

    //enter area event
    public void SetFlags_EnterArea(bool bSet)
    {
        if (bSet)
        {
            m_yFlags |= 0x02;
        }
        else
        {
            m_yFlags &= 0xfd;
        }
    }

    public bool IsFlags_EnterArea()
    {
        return (m_yFlags & 0x02) != 0;
    }

    //item changed event
    public void SetFlags_ItemChanged(bool bSet)
    {
        if (bSet)
        {
            m_yFlags |= 0x04;
        }
        else
        {
            m_yFlags &= 0xfb;
        }
    }

    public bool IsFlags_ItemChanged()
    {
        return (m_yFlags & 0x04) != 0;
    }

    //pet changed event
    public void SetFlags_PetChanged(bool bSet)
    {
        if (bSet)
        {
            m_yFlags |= 0x08;
        }
        else
        {
            m_yFlags &= 0xf7;
        }
    }

    public bool IsFlags_PetChanged()
    {
        return (m_yFlags & 0x08) != 0;
    }

    public void SetFlags_LockedTarget(bool bSet)
    {
        if (bSet)
        {
            m_yFlags |= 0x10;
        }
        else
        {
            m_yFlags &= 0xef;
        }
    }

    public bool IsFlags_LockedTarget()
    {
        return (m_yFlags & 0x10) != 0;
    }

    public void SetFlags_EnterCopy(bool bSet)
    {
        if (bSet)
        {
            m_yFlags |= 0x20;
        }
        else
        {
            m_yFlags &= 0xdf;
        }
    }

    public bool IsFlags_EnterCopy()
    {
        return (m_yFlags & 0x20) != 0;
    }

    public void SetFlags_Convey(bool bSet)
    {
        if (bSet)
        {
            m_yFlags |= 0x40;
        }
        else
        {
            m_yFlags &= 0xbf;
        }
    }

    public bool IsFlags_Convey()
    {
        return (m_yFlags & 0x40) != 0;
    }


    //-------- Allan 17/1/2011 
    public void SetFlags_Trace(bool bSet)
    {
        if (bSet)
        {
            m_logicFlags |= 0x01;
        }
        else
        {
            m_logicFlags &= 0xfe;
        }
    }

    public bool IsFlags_Trace()
    {
        return (m_logicFlags & 0x01) != 0;
    }
    //-------- Allan End ------------------*/
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class _MISSION_DB_LOAD
{
	public byte			    m_Count;
	public _OWN_MISSION[]	m_aMission = new _OWN_MISSION[GAMEDEFINE.MAX_CHAR_MISSION_NUM];             //角色所接的任务信息
    public uint[]			m_aMissionHaveDoneFlags = new uint[GAMEDEFINE.MAX_CHAR_MISSION_FLAG_LEN];   //角色的任务完成标志
    public int[]			m_aMissionData = new int[GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM] ;            //角色身上的任务自定义数据

    public _MISSION_DB_LOAD()
    {
        for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
        {
            m_aMission[i] = new _OWN_MISSION();
        }
        CleanUp();
    }
    public void CleanUp( )
    {
        m_Count = 0;
        for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
        {
            m_aMission[i].Cleanup();
        }
    }

    // 取得包大小
    public int getSize()
    {
        return sizeof(int) * +sizeof(uint) * GAMEDEFINE.MAX_MISSION_PARAM_NUM + sizeof(byte) * 2;
    }

    // 将包内容转换为byte流 return byte流长度
    public int writeToBuff(ref NetOutputBuffer buff)
    {
        return getSize();
    }

    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (buff.ReadByte(ref m_Count) != sizeof(byte)) return false;

        for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
        {
            m_aMission[i] = new _OWN_MISSION();
            if (!m_aMission[i].readFromBuff(ref buff))
            {
                return false;
            }
        }
        for (int j = 0; j < GAMEDEFINE.MAX_CHAR_MISSION_FLAG_LEN; j++)
        {
            if (buff.ReadUint(ref m_aMissionHaveDoneFlags[j]) != sizeof(uint))
            {
                return false;
            }
        }
        for (int k = 0; k < GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM; k++)
        {
            if (buff.ReadInt(ref m_aMissionData[k]) != sizeof(int))
            {
                return false;
            }
        }

        return true;
    }

    public static int GetMaxSize()
    {
        return sizeof(byte) + (sizeof(uint) + sizeof(int) + _OWN_MISSION.getMaxSize()) * GAMEDEFINE.MAX_CHAR_MISSION_FLAG_LEN;
    }
}

enum PET_INDEX
{
	PET_INDEX_INVALID	= -1,
	PET_INDEX_SELF_BEGIN,	// 自己身上的第一只
	PET_INDEX_SELF_2,		// 自己身上第二只
	PET_INDEX_SELF_3,		// 自己身上第三只
	PET_INDEX_SELF_4,		// 自己身上第四只
	PET_INDEX_SELF_5,		// 自己身上第五只
	PET_INDEX_SELF_6,		// 自己身上第六只

	PET_INDEX_SELF_NUMBERS,

	TARGETPET_INDEX		=100,		//
/*
	PET_INDEX_EXCHANGE_BEGIN  = 100,	// 交易时的第一只
	PET_INDEX_EXCHANGE_2,			// 交易时的第二只
	PET_INDEX_EXCHANGE_3,			// 交易时的第三只
	PET_INDEX_EXCHANGE_4,			// 交易时的第四只
	PET_INDEX_EXCHANGE_5,			// 交易时的第五只

	PET_INDEX_EXCHANGE_NUMBERS,

	PET_INDEX_STALL_BEGIN  = 200,	// 摆摊时的第一只
	PET_INDEX_STALL_2,				// 摆摊时的第二只
	PET_INDEX_STALL_3,				// 摆摊时的第三只
	PET_INDEX_STALL_4,				// 摆摊时的第四只
	PET_INDEX_STALL_5,				// 摆摊时的第五只

	PET_INDEX_STALL_NUMBERS,

	PET_INDEX_PLAYERSHOP_BEGIN  = 300,		// 玩家商店中的
	PET_INDEX_PLAYERSHOP_2,					// 玩家商店的第二只
	PET_INDEX_PLAYERSHOP_3,					// 玩家商店的第三只
	PET_INDEX_PLAYERSHOP_4,					// 玩家商店的第四只
	PET_INDEX_PLAYERSHOP_5,					// 玩家商店的第五只

	PET_INDEX_PLAYERSHOP_NUMBERS,
*/
};
/// <summary>
/// 设置保存
/// </summary>
//[Serializable]
//[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _OWN_SETTING : ClassCanbeSerialized
{
    public byte m_SettingType;
    public int m_SettingData;


    public static int getMaxSize()
    {
        return sizeof(byte) + sizeof(int);
    }
    #region ClassCanbeSerialized 成员

    public int getSize()
    {
        return sizeof(byte) + sizeof(int);
    }

    public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadByte(ref m_SettingType);
        buff.ReadInt(ref m_SettingData);

        return true;
    }

    public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteByte(m_SettingType);
        buff.WriteInt(m_SettingData);

        return getSize();
    }

    #endregion
};
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class _OWN_RELATION
{
	public RELATION_MEMBER		m_Member = new RELATION_MEMBER();
    public int m_FriendPoint;		//友好度
    public byte m_Type;				//关系标志, 见enum RELATION_TYPE
    public byte m_Group;			//所在的组

	public _OWN_RELATION( )
	{
		CleanUp( ) ;
	}
	public void CleanUp( )
	{
		m_Member.CleanUp( ) ;
		m_FriendPoint = 0 ;
        m_Type = (byte)RELATION_TYPE.RELATION_TYPE_NONE;
		m_Group = 0;
	}
	public void Init( _OWN_RELATION pRelation)
	{
        RELATION_MEMBER.copy(pRelation.m_Member, m_Member);
		m_FriendPoint = pRelation.m_FriendPoint;
		m_Type = pRelation.m_Type;
		m_Group = pRelation.m_Group;
	}
};

