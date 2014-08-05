using Network;
using Network.Packets;

public class SimpleImpactList
{
	public int						m_nNumOfImpacts;				// BUFF 数量
	public short[]				    m_aBuffID = new short[GAMEDEFINE.MAX_IMPACT_NUM];		// BUFF 列表

    public SimpleImpactList()		{ CleanUp(); }

    public void CleanUp() { m_nNumOfImpacts = 0; }

    public int GetImpactsCount() { return m_nNumOfImpacts; }

    public void AddImpact(short impact)
	{
		//Assert( m_nNumOfImpacts <= MAX_IMPACT_NUM );
		m_aBuffID[m_nNumOfImpacts++] = impact;
	}

    public void RemoveImpact(short impact)
	{
		//Assert( m_nNumOfImpacts > 0 );
		for( short i=0; i<m_nNumOfImpacts; ++i )
		{
			if( m_aBuffID[i] == impact )
			{
				--m_nNumOfImpacts;

				for( int j=i; j<m_nNumOfImpacts; ++j )
				{
					m_aBuffID[j] = m_aBuffID[j+1];
				}

				return;
			}
		}
	}

    public void SetImpactList(SimpleImpactList pSimpleImpactList)
	{
		m_nNumOfImpacts = pSimpleImpactList.GetImpactsCount();
        for( short i=0; i<m_nNumOfImpacts; ++i )
        {
            m_aBuffID[i] = pSimpleImpactList.m_aBuffID[i];
        }
	}

    public void SetImpactList(_IMPACT_LIST pImpactList)
	{
		m_nNumOfImpacts = pImpactList.m_Count;
		for( short i=0; i<m_nNumOfImpacts; ++i )
		{
			m_aBuffID[i] = pImpactList.m_aImpacts[i].ImpactID;
		}
	}

	public static bool	operator==( SimpleImpactList pSimpleImpactList,_IMPACT_LIST ImpactList )
	{
		if( pSimpleImpactList.m_nNumOfImpacts != ImpactList.m_Count )
		{
			return false;
		}

		for( short i=0; i<ImpactList.m_Count; ++i )
		{ // 查找是否 Buff 列表已经改变，每个当前 ID 拿去 Cache 中比较一下是否存在
			short j;
			for( j=0; j<pSimpleImpactList.m_nNumOfImpacts; ++j )
			{
                if (ImpactList.m_aImpacts[i].ImpactID == pSimpleImpactList.m_aBuffID[j])
				{
					break;
				}
			}

			if ( j >= pSimpleImpactList.m_nNumOfImpacts )
			{ // 这个 ID 不存在
				return false;
			}
		}

		return true;
	}

	public static bool	operator!=(SimpleImpactList pSimpleImpactList ,_IMPACT_LIST ImpactList )
	{
		return (pSimpleImpactList == ImpactList) == false;
	}
};

//玩家请求得到结果，返回给该玩家的消息结构
public class RECRUIT_INFO: ClassCanbeSerialized
{
   public RECRUIT_INFO()
   {
        m_NameLen   = 0;
        m_TeamID    = -1;
        m_TeamNum   =0;
    }

    public static int getMaxSize()
   {
       return sizeof(short) + sizeof(int) + sizeof(byte) * (GAMEDEFINE.MAX_CHARACTER_NAME + 2);
   }
    #region ClassCanbeSerialized Members

    public int  getSize()
    {
 	    return sizeof(short) + sizeof(int) + sizeof(byte) * (m_NameLen + 2);
    }

    public bool  readFromBuff(ref NetInputBuffer buff)
    {
 	    buff.ReadShort(ref m_TeamID);
        buff.ReadInt(ref m_TeamNum);
        buff.ReadByte(ref m_NeedTeamNum);
        buff.ReadByte(ref m_NameLen);
        buff.Read(ref m_szUserName, m_NameLen);

        return true;
    }

    public int  writeToBuff(ref NetOutputBuffer buff)
    {
 	    throw new System.NotImplementedException();
    }

    #endregion

    public short               m_TeamID;
    public int                 m_TeamNum;
    public byte                m_NeedTeamNum;
    //队长名字
    public byte	               m_NameLen;
    public byte[]	           m_szUserName = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];

};

//////////////////////////////////////////////////////////////////////////
//自动组队所用到的数据结构 [2011-11-10] by: cfp+

//消息类型
//enum RECRUIT_TYPE
//{
//    MEMBER_RECRUIT = 0,     //个人报名
//    LEADER_RECRUIT,         //队长报名
//};

////组队目的
//enum TEAME_TARGET
//{
//    EXERCISE_LEVEL = 0,		//是否练级
//    DO_MISSION,				//是否做任务
//    BOUT,					//是否切磋
//    KILL_BOSS,				//是否打BOSS
//    KILL_ENEMY,				//是否敌国杀人
//    HANG,					//是否挂机
//    TARGET_NUMBER,
//};

////副本难度
//enum COPYSCENE_DIFFICULTY
//{
//    BRIEFNESS   =   0 , //简单
//    GENERAL,            //一般
//    HARD,               //难
//    HELL,               //地狱级
//    DIFFICULTY_NUMBER,
//};


////报名
//struct RECRUIT_ATTRIB
//{
//    RECRUIT_ATTRIB()
//    {
//        //iMinLevel = 0;
//        //iMaxLevel = MAX_PLAYER_EXP_LEVEL;
//    }

//    //INT					iMinLevel;      //匹配的最小等级
//    //INT					iMaxLevel;      //匹配的最大等级
//    INT                 iLevel;         //副本难度
//    SceneID_t           SceneID;        //场景ID


//    /////////////////////////////////////////////////////////////////////////
//    //消息相关
//    UINT	GetSize()	const{
//        return  sizeof(INT) + sizeof(SceneID_t) ;
//    }

//    BOOL Read(SocketInputStream& iStream ){
//        __ENTER_FUNCTION

//            iStream.Read( (CHAR*)(&iLevel), sizeof(iLevel) ) ;
//            iStream.Read( (CHAR*)(&SceneID), sizeof(SceneID) ) ;
//            //iStream.Read( (CHAR*)(&iMinLevel), sizeof(iMinLevel) ) ;
//            //iStream.Read( (CHAR*)(&iMaxLevel), sizeof(iMaxLevel) ) ;

//            return TRUE;

//        __LEAVE_FUNCTION

//            return FALSE;
//    }

//    BOOL Write(SocketOutputStream& oStream )	const{
//        __ENTER_FUNCTION

//            oStream.Write( (CHAR*)(&iLevel), sizeof(iLevel) ) ;
//            oStream.Write( (CHAR*)(&SceneID), sizeof(SceneID) ) ;
//            //oStream.Write( (CHAR*)(&iMinLevel), sizeof(iMinLevel) ) ;
//            //oStream.Write( (CHAR*)(&iMaxLevel), sizeof(iMaxLevel) ) ;

//            return TRUE;

//        __LEAVE_FUNCTION

//            return FALSE;
//    }
//};

////自动组队返回信息
//struct RETURN_AUTOTEAM_INFO
//{
//    UCHAR	    m_NameLen;
//    CHAR	    m_szUserName[MAX_CHARACTER_NAME];
//    INT		    m_Level;
//    UCHAR	    m_MenPai;
//    CampID_t    m_Camp;	                // 阵营编号
//    INT			m_nPortrait;			// 头像
//    USHORT		m_uDataID;              // 队员的性别

//    RETURN_AUTOTEAM_INFO()
//    {
//        m_Level	    = INVALID_ID;
//        m_MenPai	= UCHAR_MAX;
//        m_Camp	    = INVALID_ID;
//        m_NameLen   = 0;
//        m_nPortrait = INVALID_ID;
//        m_uDataID   = 0;
//    }

//    //////////////////////////////////////////////////////////////////////////
//    //基本函数接口
//    VOID                SetLevel( INT iLevel ){
//        m_Level = iLevel;
//    };
//    INT                 GetLevel( ){
//        return m_Level;
//    };

//    VOID                SetMenpai( UCHAR ucMenPai ){
//        m_MenPai = ucMenPai;
//    };
//    UCHAR               GetMenpai( ){
//        return m_MenPai;
//    };

//    CampID_t			GetCamp(){
//        return m_Camp;
//    }
//    VOID				SetCamp(CampID_t id){
//        m_Camp = id;
//    }

//    VOID				SetName( const CHAR* pName ){
//        strncpy			( m_szUserName, pName, MAX_CHARACTER_NAME-1 );
//        m_NameLen		= (UCHAR)strlen(m_szUserName);
//    }
//    const CHAR*			GetName( ) const { 
//        return m_szUserName; 
//    }

//    VOID				SetIcon( INT icon ) { 
//        m_nPortrait = icon; 
//    }
//    INT					GetIcon( ) const { 
//        return m_nPortrait; 
//    }

//    VOID				SetDataID(USHORT dataid) { 
//        m_uDataID = dataid; 
//    }
//    USHORT				GetDataID() const { 
//        return m_uDataID; 
//    }

//    //////////////////////////////////////////////////////////////////////////
//    //消息处理
//    UINT	GetSize()	const
//    {
//        UINT size = sizeof(m_Level) + sizeof(m_MenPai) + 
//                    sizeof(m_Camp) + sizeof(m_nPortrait)
//                    +sizeof(m_uDataID)
//                    + sizeof(m_NameLen) 
//                    + m_NameLen * sizeof(CHAR);
//        return size;
//    }

//    BOOL Read(SocketInputStream& iStream )
//    {
//        __ENTER_FUNCTION

//            iStream.Read( (CHAR*)(&m_Level), sizeof(m_Level) ) ;
//            iStream.Read( (CHAR*)(&m_MenPai), sizeof(m_MenPai) ) ;
//            iStream.Read( (CHAR*)(&m_Camp), sizeof(m_Camp) ) ;
//            iStream.Read( (CHAR*)(&m_nPortrait), sizeof(m_nPortrait) ) ;
//            iStream.Read( (CHAR*)(&m_uDataID), sizeof(m_uDataID) ) ;
//            iStream.Read( (CHAR*)(&m_NameLen), sizeof(m_NameLen) ) ;
//            iStream.Read( (CHAR*)m_szUserName,  m_NameLen*sizeof(CHAR) ) ;

//            return TRUE;

//        __LEAVE_FUNCTION

//            return FALSE;
//    }

//    BOOL Write(SocketOutputStream& oStream )	const
//    {
//        __ENTER_FUNCTION

//            oStream.Write( (CHAR*)(&m_Level), sizeof(m_Level) ) ;
//            oStream.Write( (CHAR*)(&m_MenPai), sizeof(m_MenPai) ) ;
//            oStream.Write( (CHAR*)(&m_Camp), sizeof(m_Camp) ) ;
//            oStream.Write( (CHAR*)(&m_nPortrait), sizeof(m_nPortrait) ) ;
//            oStream.Write( (CHAR*)(&m_uDataID), sizeof(m_uDataID) ) ;
//            oStream.Write( (CHAR*)(&m_NameLen), sizeof(m_NameLen) ) ;
//            oStream.Write( (CHAR*)m_szUserName,  m_NameLen*sizeof(CHAR) ) ;
//            return TRUE;

//        __LEAVE_FUNCTION

//            return FALSE;
//    }

//};
