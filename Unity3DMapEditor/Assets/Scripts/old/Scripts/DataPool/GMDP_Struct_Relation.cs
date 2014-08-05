using System;
using System.Collections;
using System.Collections.Generic;
public class SDATA_RELATION_MEMBER
{
        // 名单人数
    public const int LIST_MEMBER_COUNT		=	20;
    public const int  RELATION_DESC_SIZE	=		32;
    public  const int 	LOCATION_DESC_SIZE	=		32;
    public const int 	TEAM_DESC_SIZE		=		32;
	public uint								m_GUID;								// GUID
	public string								m_szName;		// 名字

	public RELATION_TYPE						m_RelationType;						// 关系类型
	public int									m_nFriendPoint;						// 好友度

	public int									m_nLevel;							// 角色等级
	public int									m_nMenPai;							// 五行属性 MENPAI_ATTRIBUTE
	public int									m_nPortrait;						// 头像
	public short							    m_GuildID;							// 帮会ID
	public string								m_szGuildName;	// 帮会名称
	public bool								m_bOnlineFlag;						// 是否在线
	public string								m_szMood;			// 心情
	public string								m_szTitle;		// 称号
	public short							    m_SceneID;							// 所在场景
	public string								m_szRelation;	// 具体关系描述
	public string								m_szLocation;	// 具体位置（离线或者场景）
	public int									m_nTeamSize;						// 队伍人数（0 表示未组队）
	public string								m_szTeamDesc;		// 组队描述
	public List<int>					m_vMailList = new List<int>();						// 邮件索引列表

	public int									m_nEnterOrder;						// 临时好友进入的顺序

	public SDATA_RELATION_MEMBER()				{ CleanUp(); }
	public void								CleanUp()
    {
        m_GUID = MacroDefine.UINT_MAX;
	    m_szName = null;

	    m_RelationType = RELATION_TYPE.RELATION_TYPE_NONE;
	    m_nFriendPoint = 0;

	    m_nLevel = 0;
	    m_nMenPai = 9;
	    m_nPortrait = -1;
	    m_GuildID = MacroDefine.INVALID_ID;
	    m_szGuildName = null;
	    m_bOnlineFlag = false;
	    m_szMood = "";
	    m_szTitle = null;
	    m_SceneID = MacroDefine.INVALID_ID;
        m_szRelation = null;
	    m_szLocation = null;
	    
	    m_nTeamSize = 0;
        m_szTeamDesc  = null;
	    m_nEnterOrder = 0;
    }
};

// 名单
public class RelationList
{
	 ~RelationList()
    {
        CleanUp();
    }

	public void								CleanUp()
    {
	    m_vecRelationMember.Clear();
    }

	// 加入名单
	public virtual bool						Add( SDATA_RELATION_MEMBER pMember )
    {
	    bool bAddedFlag = false;

	    if ( m_vecRelationMember.Count >= SDATA_RELATION_MEMBER.LIST_MEMBER_COUNT )
	    { // 人数太多
		    return false;
	    }

	    if ( !IsFitType( pMember ) )
	    { // 不应该放到这类名单里
		    return false;
	    }

	    for( int i=0; i<(int)m_vecRelationMember.Count; ++i )
	    { // 按拼音顺序插入现有名单
		    int nRet;

		    nRet = Compare(pMember, m_vecRelationMember[i]);
		    if ( nRet < 0 )
		    { // 优先级低
			    continue;
		    }
		    else
		    {
			    m_vecRelationMember.Insert( i, pMember);
			    bAddedFlag = true;
			    break;
		    }
	    }

	    if ( !bAddedFlag )
	    {
		    m_vecRelationMember.Add(pMember);
	    }

	    return true;
    }

	// 从名单里移除，只移除指针，不释放数据
	public virtual bool						Remove( int idx )
    {
        if ( m_vecRelationMember.Count <= idx || idx < 0 )
	    {
		    return false;
	    }

	    m_vecRelationMember.RemoveAt( idx );

	    return true;
    }

	// 从名单里移除，只移除指针，不释放数据
	public virtual bool						RemoveByGUID( uint guid )
    {
	    int idx;
	    idx = GetMemberIndex( guid );

	    if ( idx == -1 )
	    {
		    return false;
	    }

	    return Remove( idx );
    }

	// 从名单里移除，并释放数据
	public virtual bool						Erase( int idx )
    {
        if ( m_vecRelationMember.Count <= idx || idx < 0 )
	    {
		    return false;
	    }

	    // 更新好友信息 [9/26/2011 Ivan edit]
	    CObject_Character player = (CObject_Character)(CObjectManager.Instance.FindServerObject(
		    (int)m_vecRelationMember[idx].m_GUID));
	    if (player != null)
	    {
		    player.GetCharacterData().RefreshName();
	    }

	 //   SAFE_DELETE( m_vecRelationMember[idx] );
	    m_vecRelationMember.RemoveAt(idx );

	    return true;
    }

	// 从名单里移除，并释放数据
	public virtual bool						EraseByGUID( uint guid )
    {
        int idx;
	    idx = GetMemberIndex( guid );

	    if ( idx == -1 )
	    {
		    return false;
	    }

	    return Erase( idx );
    }

	// 得到当前名单人数
	public  int							GetMemberCount()
    {
        return m_vecRelationMember.Count;
    }

	// 得到一个关系数据（不可修改数据）
	public  SDATA_RELATION_MEMBER		GetMember( int idx )
    {
        if ( m_vecRelationMember.Count <= idx || idx < 0 )
	    {
		    return null;
	    }

	    return m_vecRelationMember[idx];
    }

	// 得到一个关系数据（可以修改数据）
	public SDATA_RELATION_MEMBER				GetMemberByGUID( uint guid )
    {
        for( int i=0; i<m_vecRelationMember.Count; ++i )
	    {
		    if ( m_vecRelationMember[i].m_GUID == guid )
		    {
			    return m_vecRelationMember[i];
		    }
	    }

	    return null;
    }

	// 通过名字得到一个关系数据（可以修改数据）
	public SDATA_RELATION_MEMBER				GetMemberByName( string szName )
    {
        for( int i=0; i<m_vecRelationMember.Count; ++i )
	    {
		    if ( m_vecRelationMember[i].m_szName == szName )
		    {
			    return m_vecRelationMember[i];
		    }
	    }

	    return null;
    }
	
	// 通过名字得到在名单中的位置（可以修改数据）
	public int									GetMemberIndexByName( string szName )
    {
       for( int i=0; i<m_vecRelationMember.Count; ++i )
	    {
		    if ( m_vecRelationMember[i].m_szName == szName )
		    {
			    return i;
		    }
	    }

	    return -1;
    }
	// 判断当前名单是否已满
	public bool								IsFull()
    {
        return (m_vecRelationMember.Count >= SDATA_RELATION_MEMBER.LIST_MEMBER_COUNT);
    }
	// 得到某个 GUID 在名单中的位置，如果不存在，则返回 -1
	public  int							GetMemberIndex(uint guid)
    {
       for( int i=0; i<m_vecRelationMember.Count; ++i )
	    {
		    if ( m_vecRelationMember[i].m_GUID == guid )
		    {
			    return i;
		    }
	    }

	    return -1;
    }

	// 判断是否名单需要的类型
	protected virtual bool						IsFitType( SDATA_RELATION_MEMBER pMember ) {return false;}

	// 返回值：CSTR_LESS_THAN, CSTR_EQUAL, CSTR_GREATER_THAN,
	// 错误时返回 0
	protected static int							CompareByName(string szName1, string szName2)
    {
        return string.Compare(szName1, szName2);
    }

	// 比较两个关系人的优先排放顺序（<0表示优先级低，0表示相等，>0表示优先级高）
	protected virtual int							Compare( SDATA_RELATION_MEMBER pMember1,  SDATA_RELATION_MEMBER pMember2)
    {
        return CompareByName(pMember1.m_szName, pMember2.m_szName);
    }


	// 有社会关系的人员
	protected List<SDATA_RELATION_MEMBER>	m_vecRelationMember = new List<SDATA_RELATION_MEMBER>();
};

// 好友名单
public class FriendList :  RelationList
{

	// 判断是否名单需要的类型
    protected	override bool						IsFitType( SDATA_RELATION_MEMBER pMember )
    {
    	    return (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_FRIEND)
		    || (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_BROTHER)
		    || (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_MARRY)
		    || (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_MASTER)
		    || (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_PRENTICE);
    }

	// 计算一个好友的优先级，用于排序，目前仅用于 Compare 使用，所以返回值不定义
    protected	int									CalcPriority(  SDATA_RELATION_MEMBER pMember )
    {
        if( pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_MARRY )
	    {
		    return 10;
	    }
	    else if( pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_BROTHER )
	    {
		    return 9;
	    }
	    else if( pMember.m_nFriendPoint >= GAMEDEFINE.FRIEND_POINT_ENOUGH_NOTIFY )
	    { // 亲密好友
		    if( pMember.m_bOnlineFlag )
		    { // 如果在线
			    return 8;
		    }
		    else
		    {
			    return 7;
		    }
	    }
	    else
	    {
		    return 6;
	    }
    }

	// 比较两个关系人的优先排放顺序（<0表示优先级低，0表示相等，>0表示优先级高）
    protected	override int							Compare( SDATA_RELATION_MEMBER pMember1,  SDATA_RELATION_MEMBER pMember2)
    {
        int nPriority1;
        int nPriority2;

	    nPriority1 = CalcPriority( pMember1 );
	    nPriority2 = CalcPriority( pMember2 );

	    if( nPriority1 > nPriority2 )
	    {
		    return 1;
	    }
	    else if( nPriority1 < nPriority2 )
	    {
		    return -1;
	    }
	    else
	    {
		    return base.Compare( pMember1, pMember2 );
	    }
    }
	// 最后一次获取详细信息的时间，这个变量的使用与否取决于好友界面（或 Tip）需要显示的信息量
	//DWORD								m_dwTimeStamp;
};

// 临时关系名单
public class TempRelationList :  RelationList
{

	// 加入名单
    public	override bool						Add( SDATA_RELATION_MEMBER pMember )
    {
        if ( m_vecRelationMember.Count < 1 )
	    { // 初始化最大 Order
		    m_nMaxOrder = 0;
	    }

	    while ( m_vecRelationMember.Count >= SDATA_RELATION_MEMBER.LIST_MEMBER_COUNT )
	    { // 人数太多
		    int nSize;

		    nSize = m_vecRelationMember.Count;
		    for( int i=0; i<nSize; ++i )
		    {
			    if ( m_vecRelationMember[i].m_nEnterOrder == 1 )
			    {
				    m_vecRelationMember.RemoveAt(i);
				    --m_nMaxOrder;
			    }
			    else
			    {
				    --(m_vecRelationMember[i].m_nEnterOrder);
			    }
		    }
	    }

	    pMember.m_nEnterOrder = ++m_nMaxOrder; // 所以最小 Order 为 1

	    return base.Add( pMember );
    }


	// 判断是否名单需要的类型
    protected	override bool						IsFitType( SDATA_RELATION_MEMBER pMember )
    {
        return (pMember.m_RelationType == RELATION_TYPE .RELATION_TYPE_TEMPFRIEND);
    }

    int									m_nMaxOrder;

}

// 黑名单
public class BlackList :  RelationList
{

	// 判断是否名单需要的类型
    protected	override bool						IsFitType( SDATA_RELATION_MEMBER pMember )
    {
        return (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_TEMPFRIEND);
    }
}


public class Relation
{
	public bool								AddRelation(  _FRIEND_INFO pFriend )
    {

	    RelationList pRelationList;

	    switch( (RELATION_GROUP)pFriend.m_uGroup )
	    {
	    case RELATION_GROUP.RELATION_GROUP_F1:
	    case RELATION_GROUP.RELATION_GROUP_F2:
	    case RELATION_GROUP.RELATION_GROUP_F3:
	    case RELATION_GROUP.RELATION_GROUP_F4:
		    pRelationList = GetRelationList( (RELATION_GROUP)pFriend.m_uGroup );
		    break;
	    default:
		    return false;
	    }

	    if( pRelationList == null )
	    {
		    return false;
	    }

	    SDATA_RELATION_MEMBER pNewRelation;
	    pNewRelation = pRelationList.GetMemberByGUID( pFriend.m_GUID );

	    if( pNewRelation != null )
	    {
		    pNewRelation.m_szName= UIString.Instance.GetUnicodeString(pFriend.m_szTargetName);
		    pNewRelation.m_RelationType = (RELATION_TYPE)pFriend.m_uRelationType;
		    pNewRelation.m_nFriendPoint = pFriend.m_nFriendpoint;
		    return true;
	    }
	    else
	    {
		    pNewRelation = new SDATA_RELATION_MEMBER();

		    pNewRelation.m_GUID = pFriend.m_GUID;
		    pNewRelation.m_szName= UIString.Instance.GetUnicodeString(pFriend.m_szTargetName);
		    pNewRelation.m_RelationType = (RELATION_TYPE)pFriend.m_uRelationType;
		    pNewRelation.m_nFriendPoint = pFriend.m_nFriendpoint;
		    return pRelationList.Add(pNewRelation);
	    }
    }

	// 从服务器端接收一个黑名单玩家
	public bool								AddRelation(  _BLACKNAME_INFO pBlackName )
    {
        RelationList pRelationList;

	    pRelationList = GetRelationList(RELATION_GROUP.RELATION_GROUP_BLACK);
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    SDATA_RELATION_MEMBER pNewRelation;
	    pNewRelation = pRelationList.GetMemberByGUID( pBlackName.m_GUID );

	    if( pNewRelation != null )
	    {
		    pNewRelation.m_szName = UIString.Instance.GetUnicodeString(pBlackName.m_szTargetName);
		    pNewRelation.m_RelationType = RELATION_TYPE.RELATION_TYPE_BLACKNAME;
		    return true;
	    }
	    else
	    {
		    pNewRelation = new SDATA_RELATION_MEMBER();

		    if( pNewRelation == null )
		    {
			    return false;
		    }

		    pNewRelation.m_GUID = pBlackName.m_GUID;
		    pNewRelation.m_szName = UIString.Instance.GetUnicodeString(pBlackName.m_szTargetName);
		    pNewRelation.m_RelationType = RELATION_TYPE.RELATION_TYPE_BLACKNAME;
		    return pRelationList.Add(pNewRelation);
	    }
    }
	// 获得一个名单的人数
	public int									GetListCount( RELATION_GROUP RelationGroup )
    {
        RelationList pRelationList;

	    pRelationList = GetRelationList(RelationGroup);
	    if ( pRelationList == null )
	    {
		    return 0;
	    }

	    return pRelationList.GetMemberCount();
    }

	//清理临时好友
	public void								ClearTempFriend(  )
    {
        RelationList pRelationList;

	    pRelationList = GetRelationList(RELATION_GROUP.RELATION_GROUP_TEMPFRIEND);
	    if ( pRelationList == null )
	    {
		    return ;
	    }
	    pRelationList.CleanUp();
    }

	// 客户端获得关系人信息，供界面使用
	public SDATA_RELATION_MEMBER		GetRelationInfo( RELATION_GROUP RelationGroup, int idx )
    {
        RelationList pRelationList;

	    pRelationList = GetRelationList(RelationGroup);
	    if ( pRelationList == null )
	    {
		    return null;
	    }

	    return pRelationList.GetMember( idx );
    }

	// 加入一个关系，用于游戏过程中，加入时需要指定将要加入的组
	public bool								AddRelation( RELATION_GROUP RelationGroup, SDATA_RELATION_MEMBER pMember )
    {
	    RelationList pRelationList;

	    pRelationList = GetRelationList(RelationGroup);
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    // 不让重复加好友，不能加自己为好友
	    if( pMember.m_GUID != MacroDefine.UINT_MAX )
	    {
		    if( CObjectManager.Instance.getPlayerMySelf().GetServerGUID() == pMember.m_GUID )
		    {
			    return false;
		    }

		    RELATION_GROUP rg = RELATION_GROUP.RELATION_GROUP_F4;
            int idx = 0 ;

		    if( GetRelationPosition( pMember.m_GUID, ref rg, ref idx ) != RELATION_TYPE.RELATION_TYPE_STRANGER )
		    {
			    return false;
		    }
		    else if( pRelationList.GetMemberByGUID(pMember.m_GUID) != null )
		    {
			    return false;
		    }
	    }
	    else if(pMember.m_szName.Length > 0 )
	    {
		    string str1 = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Name();
		    string str2 = pMember.m_szName;
		    if( str1 == str2 )
		    {
			    return false;
		    }

            int nrg = 0;
            int idx =0;

		    if( GetRelationByName(pMember.m_szName, ref nrg, ref idx ) != RELATION_TYPE.RELATION_TYPE_STRANGER )
		    {
			    return false;
		    }
		    else if( pRelationList.GetMemberByName(pMember.m_szName) != null )
		    {
			    return false;
		    }
	    }

	    SDATA_RELATION_MEMBER pNewRelation = new SDATA_RELATION_MEMBER();

	    pNewRelation.m_GUID = pMember.m_GUID;
	     pNewRelation.m_szName = ( pMember.m_szName);
	    pNewRelation.m_RelationType = pMember.m_RelationType;
	    pNewRelation.m_nFriendPoint = pMember.m_nFriendPoint;
	    pNewRelation.m_nLevel = pMember.m_nLevel;
	    pNewRelation.m_nMenPai = pMember.m_nMenPai;
	    pNewRelation.m_nPortrait = pMember.m_nPortrait;
	    pNewRelation.m_GuildID = pMember.m_GuildID;
	    pNewRelation.m_szGuildName= (pMember.m_szGuildName);
	    pNewRelation.m_bOnlineFlag = pMember.m_bOnlineFlag;
	    SetRelationDesc( pNewRelation );

	    if( pMember.m_bOnlineFlag )
	    {
		    pNewRelation.m_szMood = pMember.m_szMood;
		    pNewRelation.m_szTitle = pMember.m_szTitle;
		    pNewRelation.m_SceneID = pMember.m_SceneID;
		    GetLocationName( pNewRelation.m_SceneID, out pNewRelation.m_szLocation );
		    pNewRelation.m_nTeamSize = pMember.m_nTeamSize;
		    SetTeamDesc( pNewRelation );
	    }
	    else
	    {
		    pNewRelation.m_szLocation =  "离线";
	    }

	    pRelationList.Add(pNewRelation);
	    UpdateUIList( RelationGroup );

	    return true;
    }

	// 服务器端发消息来更新关系人信息
	public bool								UpdateRelationInfo( RETURN_RELATION_INFO pRelationInfo )
    {
        RelationList pRelationList;
	    RELATION_GROUP grp;

	    switch( (RELATION_TYPE)pRelationInfo.GetRelationType() )
	    {
	    case RELATION_TYPE.RELATION_TYPE_BLACKNAME:
		    grp = RELATION_GROUP.RELATION_GROUP_BLACK;
		    break;
	    case RELATION_TYPE.RELATION_TYPE_TEMPFRIEND:
		    grp = RELATION_GROUP.RELATION_GROUP_TEMPFRIEND;
		    break;
	    default:
		    grp = (RELATION_GROUP)pRelationInfo.GetGroup();
            break;
	    }

	    _RELATION pRelationData;
	    pRelationData = pRelationInfo.GetRelationData();

	    pRelationList = GetRelationList( grp );
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    uint guid = pRelationData.GetGUID();
	    SDATA_RELATION_MEMBER pMember;
	    pMember = pRelationList.GetMemberByGUID( guid );
	    if( pMember == null )
	    {
		    pMember = pRelationList.GetMemberByName( UIString.Instance.GetUnicodeString(pRelationData.GetName()) );
	    }
	    if( pMember == null )
	    { // 陌生人，查看玩家
		    m_ViewPlayer.m_GUID = pRelationData.GetGUID();
		    m_ViewPlayer.m_szName = UIString.Instance.GetUnicodeString( pRelationData.GetName());
		    m_ViewPlayer.m_RelationType = (RELATION_TYPE)pRelationInfo.GetRelationType();
		    m_ViewPlayer.m_nFriendPoint = pRelationInfo.GetFriendPoint();
		    m_ViewPlayer.m_nLevel = pRelationData.GetLevel();
		    m_ViewPlayer.m_nMenPai = pRelationData.GetMenPai();
		    m_ViewPlayer.m_nPortrait = pRelationData.GetPortrait();
		    m_ViewPlayer.m_GuildID = pRelationData.GetGuildID();
		     m_ViewPlayer.m_szGuildName = UIString.Instance.GetUnicodeString(pRelationData.GetGuildName());
		    m_ViewPlayer.m_bOnlineFlag = pRelationData.GetOnlineFlag() != 0;
		    //		SetRelationDesc( &m_ViewPlayer );

		    if( m_ViewPlayer.m_bOnlineFlag )
		    {
			    m_ViewPlayer.m_szMood= UIString.Instance.GetUnicodeString( pRelationData.GetMood() );
			    m_ViewPlayer.m_szTitle =UIString.Instance.GetUnicodeString(pRelationData.GetTitle());
			    m_ViewPlayer.m_SceneID = pRelationData.GetSceneID();
			    GetLocationName( m_ViewPlayer.m_SceneID, out m_ViewPlayer.m_szLocation );
			    m_ViewPlayer.m_nTeamSize = pRelationData.GetTeamSize();
			    SetTeamDesc( m_ViewPlayer );
		    }

		    // TODO: Push_Event here
		    CEventSystem.Instance.PushEvent( GAME_EVENT_ID.GE_CHAT_SHOWUSERINFO);
		    return true;
	    }

	    pMember.m_GUID = pRelationData.GetGUID();
	    pMember.m_szName= UIString.Instance.GetUnicodeString(pRelationData.GetName());
	    pMember.m_RelationType = (RELATION_TYPE)pRelationInfo.GetRelationType();
	    pMember.m_nFriendPoint = pRelationInfo.GetFriendPoint();
	    pMember.m_nLevel = pRelationData.GetLevel();
	    pMember.m_nMenPai = pRelationData.GetMenPai();
	    pMember.m_nPortrait = pRelationData.GetPortrait();
	    pMember.m_GuildID = pRelationData.GetGuildID();
	    pMember.m_szGuildName =UIString.Instance.GetUnicodeString( pRelationData.GetGuildName() );
	    pMember.m_bOnlineFlag = pRelationData.GetOnlineFlag() != 0;
	    SetRelationDesc( pMember );

	    if( pMember.m_bOnlineFlag )
	    {
		    pMember.m_szMood = UIString.Instance.GetUnicodeString(pRelationData.GetMood());
		    pMember.m_szTitle =  UIString.Instance.GetUnicodeString(pRelationData.GetTitle());
		    pMember.m_SceneID = pRelationData.GetSceneID();
		    GetLocationName( pMember.m_SceneID, out pMember.m_szLocation );
		    pMember.m_nTeamSize = pRelationData.GetTeamSize();
		    SetTeamDesc( pMember );
	    }
	    else
	    {
		    pMember.m_szLocation = "离线";
	    }

	    UpdateUIInfo( grp, pRelationList.GetMemberIndex(guid) );
	    return true;
    }

	// 客户端移除一个关系，通常用于移除临时好友
	public bool								RemoveRelation( RELATION_GROUP RelationGroup, int idx )
    {
        RelationList pRelationList;

	    pRelationList = GetRelationList( RelationGroup );
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    if( idx < 0 || idx >= pRelationList.GetMemberCount() )
	    {
		    return false;
	    }

	    // keep name [10/9/2011 Ivan edit]
	    string m_szName = pRelationList.GetMember(idx).m_szName;

	    // 刷新头顶名字颜色 [9/26/2011 Ivan edit]
	    CObject_Character player = (CObject_Character)(CObjectManager.Instance.FindCharacterByName(m_szName));
	    if (player != null)
	    {
		    player.GetCharacterData().RefreshName();
	    }
        pRelationList.Erase( idx );
	    UpdateUIList( RelationGroup );
	    return true;
    }

	// 客户端移除一个关系，通常用于移除临时好友
	public bool								RemoveRelation( RELATION_GROUP RelationGroup, string szName )
    { 
        RelationList pRelationList;

	    pRelationList = GetRelationList( RelationGroup );
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    int nIndex;
	    nIndex = pRelationList.GetMemberIndexByName(szName);
	    if( nIndex >= 0 )
	    {
		    RemoveRelation( RelationGroup, nIndex );
		    return true;
	    }

	    return false;
    }
	// 从服务器端收到一条移除关系人的消息
	public bool								RemoveRelation( uint guid )
    {
        RELATION_GROUP RelationGroup = RELATION_GROUP.RELATION_GROUP_FRIEND_ALL;
	    int idx =-1;

	    if( GetRelationPosition( guid, ref RelationGroup, ref idx ) != RELATION_TYPE.RELATION_TYPE_STRANGER )
	    {
		    return RemoveRelation( RelationGroup, idx );
	    }

	    return false;
    }

	// 移动一个关系，Srg 是原来的组，guid 是要移动的玩家 GUID，Dtype 是移动后的关系类型，Drg 是移动后的组
	public bool								MoveRelation( RELATION_GROUP Srg, RELATION_TYPE Dtype, RELATION_GROUP Drg, uint guid )
    {
        RelationList pSRelationList;

	    pSRelationList = GetRelationList(Srg);
	    if ( pSRelationList == null )
	    {
		    return false;
	    }

	    RelationList pDRelationList;

	    pDRelationList = GetRelationList(Drg);
	    if ( pDRelationList == null )
	    {
		    return false;
	    }

	    SDATA_RELATION_MEMBER pMember;
	    pMember = pSRelationList.GetMemberByGUID( guid );
	    if ( pMember == null )
	    {
		    return false;
	    }

	    if ( pSRelationList.RemoveByGUID( guid ) )
	    {
		    UpdateUIList( Srg );

		    // 修改关系信息
		    pMember.m_RelationType = Dtype;

		    string m_szName = pMember.m_szName;
		    bool isFinish = false;
		    if ( pDRelationList.Add( pMember ) )
		    {
			    UpdateUIList( Drg );
			    isFinish = true;
		    }
		    else
		    {
			    pMember = null;
			    isFinish = false;
		    }

		    // 必须等add完成后再刷新 [9/26/2011 Ivan edit]
		     CObject_Character player = (CObject_Character)(CObjectManager.Instance.FindCharacterByName(m_szName));
		    if (player != null)
		    {
			    player.GetCharacterData().RefreshName();
		    }
		    return isFinish;
	    }

	    return false;
    }
	// 通知 UI 刷新列表，默认更新所有列表
	public	void							UpdateUIList( RELATION_GROUP RelationGroup  )
    {
        CEventSystem.Instance.PushEvent( GAME_EVENT_ID.GE_UPDATE_FRIEND );
    }
	// 通知 UI 刷新 RelationGroup 中第 idx 号关系人的详细信息
	public void								UpdateUIInfo( RELATION_GROUP RelationGroup, int idx )
    {
        List<string> param = new List<string>();
        int temp = (int)( RelationGroup );
        param.Add(temp.ToString());
        param.Add(idx.ToString());
        CEventSystem.Instance.PushEvent( GAME_EVENT_ID.GE_UPDATE_FRIEND_INFO, param );
    }

	// 得到一个 GUID 所在的组以及在组里的索引，没有找到返回 RELATION_TYPE_STRANGER，这个功能不考虑临时好友
	public RELATION_TYPE						GetRelationPosition( uint guid, ref RELATION_GROUP RelationGroup, ref int idx )
    {
        RelationList pRelationList;

	    for( int i = (int)RELATION_GROUP.RELATION_GROUP_F1; i < (int)RELATION_GROUP.RELATION_GROUP_NUMBER; ++i )
	    {
		    pRelationList = GetRelationList( (RELATION_GROUP)i );
		    if( pRelationList == null )
		    {
			    return RELATION_TYPE .RELATION_TYPE_STRANGER;
		    }

		    idx = pRelationList.GetMemberIndex(guid);
		    if( idx != -1 )
		    {
			    RelationGroup = (RELATION_GROUP)i;
			    return pRelationList.GetMember(idx).m_RelationType;
		    }
	    }

	    return RELATION_TYPE.RELATION_TYPE_STRANGER;
    }

	// 得到一个 名字 所在的组以及在组里的索引，没有找到返回 RELATION_TYPE_STRANGER，这个功能不考虑临时好友
	public RELATION_TYPE						GetRelationByName( string szName, ref int RelationGroup, ref int idx )
    {
        RelationList pRelationList;

	    for( int i = (int)RELATION_GROUP.RELATION_GROUP_F1; i < (int)RELATION_GROUP.RELATION_GROUP_NUMBER; ++i )
	    {
		    pRelationList = GetRelationList( (RELATION_GROUP)i );
		    if( pRelationList == null )
		    {
			    return RELATION_TYPE.RELATION_TYPE_STRANGER;
		    }

		    idx = pRelationList.GetMemberIndexByName( szName );
		    if( idx != -1 )
		    {
			    RelationGroup = i;
			    return pRelationList.GetMember(idx).m_RelationType;
		    }
	    }

	    return RELATION_TYPE.RELATION_TYPE_STRANGER;
    }

	// 更新在线好友
	public bool								UpdateOnlineFriend(  _RELATION_ONLINE pOnlineRelation )
    {
        RELATION_GROUP RelationGroup = RELATION_GROUP.RELATION_GROUP_FRIEND_ALL;
	    int idx = -1;
	    uint guid = 0;

	    guid = pOnlineRelation.GetGUID();
        if (GetRelationPosition(guid, ref RelationGroup, ref idx) == RELATION_TYPE.RELATION_TYPE_STRANGER)
	    { // 数据池还为空
		    return true;
	    }

	    RelationList pRelationList;

	    pRelationList = GetRelationList( RelationGroup );
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    SDATA_RELATION_MEMBER pMember;
	    pMember = pRelationList.GetMemberByGUID( guid );
	    if( pMember == null )
	    {
		    return false;
	    }

	    pMember.m_bOnlineFlag = true;
	    pMember.m_szMood = UIString.Instance.GetUnicodeString( pOnlineRelation.GetMood() );

	    UpdateUIList( RelationGroup ); // 刷新关系人在线状态
	    return true;
    }

	// 关系人上线
	public bool								RelationOnline(string szName, string szMood)
    {
        int nGroup =0;
	    int idx=0;

	    if( GetRelationByName( szName, ref nGroup, ref idx ) == RELATION_TYPE.RELATION_TYPE_STRANGER )
	    { // 数据池还为空
		    return true;
	    }

	    RELATION_GROUP RelationGroup;
	    RelationList  pRelationList;

	    RelationGroup = (RELATION_GROUP)nGroup;

	    pRelationList = GetRelationList( RelationGroup );
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    SDATA_RELATION_MEMBER  pMember;
	    pMember = pRelationList.GetMemberByName( szName );
	    if( pMember == null )
	    {
		    return false;
	    }

	    pMember.m_bOnlineFlag = true;
	    pMember.m_szMood = szMood;

	    UpdateUIList( RelationGroup ); // 刷新关系人在线状态

	    // 只显示好友提示 [6/23/2011 edit by ZL]
	    if(szName != null && pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_FRIEND)
	    {
		    string szText = "你的好友#Y" + szName + "#R进入游戏了";
		    //ADDTALKMSG(szText); //todo
	    }

	    return true;
    }

	// 关系人下线
	public bool								RelationOffLine(uint guid)
    {
        RELATION_GROUP RelationGroup = RELATION_GROUP.RELATION_GROUP_FRIEND_ALL;
	    int idx = 0;

	    GetRelationPosition( guid, ref RelationGroup, ref idx );

	    RelationList  pRelationList;

	    pRelationList = GetRelationList( RelationGroup );
	    if( pRelationList == null )
	    {
		    return false;
	    }

	    SDATA_RELATION_MEMBER  pMember;
	    pMember = pRelationList.GetMemberByGUID( guid );
	    if( pMember == null )
	    {
		    return false;
	    }

	    pMember.m_bOnlineFlag = false;
	    //pMember.m_szMood =  pMember.m_szMood;

	    
	
	    // 只显示好友提示 [6/23/2011 edit by ZL]
	    if (pMember.m_RelationType == RELATION_TYPE.RELATION_TYPE_FRIEND)
	    {
            string szText = "你的好友#Y" + pMember.m_szName+ "#R离开游戏了";
		    //ADDTALKMSG(szText); todo
	    }
	
	    UpdateUIList( RelationGroup ); // 刷新关系人在线状态
	    return true;

    }

	// 判断是否在自己黑名单中
	public bool								IsBlackName( string szName )
    {

        for( int i=0; i<m_BlackListGroup.GetMemberCount(); ++i )
	    {
		    if (  m_BlackListGroup.GetMember(i).m_szName ==  szName )
		    {
			    return true;
		    }
	    }

	    return false;
    }

	// 给某个好友增加一条邮件（邮件池中的索引号）
    public bool AddMail(string szName, uint uIndex)
    {
/*
        	int aInt[] = {RELATION_GROUP_F1, RELATION_GROUP_F2, RELATION_GROUP_F3, RELATION_GROUP_F4, RELATION_GROUP_TEMPFRIEND};
	RelationList  pRelationList = null;
	SDATA_RELATION_MEMBER  pRelationMember = null;

	for( INT i = 0; i < sizeof(aInt)/sizeof(INT); ++i )
	{
		pRelationList = GetRelationList( (RELATION_GROUP)aInt[i] );
		if( pRelationList == null )
		{
			Assert( pRelationList );
			return false;
		}

		pRelationMember = pRelationList.GetMemberByName(szName);
		if( pRelationMember != null )
		{
			break;
		}
	}

	if( pRelationMember == null )
	{ // 没有找到这个好友
		return false;
	}

	pRelationMember.m_vMailList.push_back(uIndex);*/
	return true;
    }

	// 得到某个好友的历史邮件数量
    public uint GetMailCount(RELATION_GROUP RelationGroup, int idx)
    {
        SDATA_RELATION_MEMBER  pRelationMember;

	    pRelationMember = GetRelationInfo( RelationGroup, idx );
	    if( pRelationMember == null )
	    {
		    return 0;
	    }

	    return (uint)pRelationMember.m_vMailList.Count;
    }

	// 得到某个好友的第 uIndex 封历史邮件的邮件池索引号
    public uint GetMailIndex(RELATION_GROUP RelationGroup, int idx, uint uIndex)
    {
        SDATA_RELATION_MEMBER  pRelationMember;

	    pRelationMember = GetRelationInfo( RelationGroup, idx );
	    if( pRelationMember == null )
	    {
		    return 0;
	    }

	    if( pRelationMember.m_vMailList.Count <= uIndex )
	    {
		    return 0;
	    }

        List<int> mailList = pRelationMember.m_vMailList;
        return (uint)mailList[(int)uIndex];
    }

	// 给某个（临时）好友重建历史信息
    public void RebuildMailHistory(RELATION_GROUP RelationGroup, string szName)
    {
        //todo
    /*    RelationList  pRelationList = null;
	    SDATA_RELATION_MEMBER  pRelationMember = null;

	    pRelationList = GetRelationList( RelationGroup );
	    if( pRelationList == null )
	    {
		    return;
	    }

	    pRelationMember = pRelationList.GetMemberByName(szName);
	    if( pRelationMember == null )
	    { // 没有找到这个好友
		    return;
	    }

	    CMailPool  pMailPool = CDataPool::GetMe().GetMailPool();

	    for( INT i=0; i<pMailPool.GetMailCount(); ++i )
	    {
		    const SMail  pMail;

		    pMail = pMailPool.GetMail(i);
		    if( pMail != null )
		    {
			    if( strcmp(pMail.m_szSourName, pRelationMember.m_szName) == 0
			     || strcmp(pMail.m_szDestName, pRelationMember.m_szName) == 0
			     )
			    {
				    pRelationMember.m_vMailList.push_back(i);
			    }
		    }
	    }*/
    }

	// 得到心情
    public string GetMood()
    {
        return m_szMood;
    }

	// 设置心情
    public void SetMood(string szMood)
    {
        if( szMood == null )
	    {
		    return;
	    }

	    m_szMood = szMood;
	    CEventSystem.Instance.PushEvent( GAME_EVENT_ID.GE_MOOD_CHANGE );
    }

	// 查看玩家界面的信息
    public SDATA_RELATION_MEMBER GetPlayerCheckInfo()
    {
        return m_ViewPlayer;
    }

	// 获得一个名单
	protected RelationList				GetRelationList( RELATION_GROUP RelationGroup )
    {
        switch( RelationGroup )
	    {
	    case RELATION_GROUP.RELATION_GROUP_F1:
		    {
			    return m_FriendGroup1;
		    }
		    //break;
	    case RELATION_GROUP.RELATION_GROUP_F2:
		    {
			    return m_FriendGroup2;
		    }
		    //break;
	    case RELATION_GROUP.RELATION_GROUP_F3:
		    {
			    return m_FriendGroup3;
		    }
		    //break;
	    case RELATION_GROUP.RELATION_GROUP_F4:
		    {
			    return m_FriendGroup4;
		    }
		    //break;
	    case RELATION_GROUP.RELATION_GROUP_TEMPFRIEND:
		    {
			    return m_TeamFriendGroup;
		    }
		    //break;
	    case RELATION_GROUP.RELATION_GROUP_BLACK:
		    {
			    return m_BlackListGroup;
		    }
		    //break;
	    default:
		    break;
	    }

	    return null;
    }

	// 根据场景 ID 获得场景名字，并存入 szSceneName
	 protected bool							GetLocationName( short sid, out string szSceneName )
     {
        if( sid != MacroDefine.INVALID_ID )
	    {
            szSceneName = GameProcedure.s_pWorldManager.GetSceneName(sid);
	    }
	    else
	    {
		    szSceneName =  "未知场景" ;
	    }

	    return true;
     }

	// 根据关系类型或者友好度确定双方关系，并存入 m_szRelation
	 protected bool							SetRelationDesc( SDATA_RELATION_MEMBER pMember )
     {
        switch( pMember.m_RelationType )
	    {
	    case RELATION_TYPE.RELATION_TYPE_FRIEND:						//好友
		    {
			    if( pMember.m_nFriendPoint < 10 )
			    {
				     pMember.m_szRelation= "一面之缘" ;
			    }
			    else if( pMember.m_nFriendPoint <= 200 )
			    {
				   pMember.m_szRelation="泛泛之交" ;
			    }
			    else if( pMember.m_nFriendPoint <= 500 )
			    {
				    pMember.m_szRelation="君子之交";
			    }
			    else if( pMember.m_nFriendPoint <= 1000 )
			    {
				    pMember.m_szRelation="莫逆之交" ;
			    }
			    else if( pMember.m_nFriendPoint > 1000 )
			    {
                    pMember.m_szRelation = "刎颈之交";
			    }
			    else
			    {
				    pMember.m_szRelation = "普通朋友";
			    }
		    }
		    break;
	    case RELATION_TYPE.RELATION_TYPE_BROTHER:						//结拜
		     pMember.m_szRelation = "金兰之好" ;
		    break;
	    case RELATION_TYPE.RELATION_TYPE_MARRY:						//结婚
		    pMember.m_szRelation = "夫妻" ;
		    break;
	    case RELATION_TYPE.RELATION_TYPE_BLACKNAME:					//黑名单
		    pMember.m_szRelation="交恶" ;
		    break;
	    case RELATION_TYPE.RELATION_TYPE_TEMPFRIEND:					//临时好友
		    pMember.m_szRelation = "临时好友" ;
		    break;
    //	case RELATION_TYPE_MASTER:						//师傅关系
    //	case RELATION_TYPE_PRENTICE:					//徒弟关系
	    default:
		    return false;
	    }

	    return true;
        
    }

	// 根据队伍人数设置组队显示数据，并存入 m_szTeamDesc
	 protected bool							SetTeamDesc( SDATA_RELATION_MEMBER pMember )
     {
        if( pMember.m_nTeamSize == 0 )
	    {
		    pMember.m_szTeamDesc= "未组队" ;
	    }
	    else
	    {
            pMember.m_szTeamDesc = "已组队"+pMember.m_nTeamSize +"人";
	    }

	    return true;
    }

	// MAX_MOOD_SIZE 才是允许输入的字符长度
	string								m_szMood;// 心情数据
	FriendList							m_FriendGroup1 = new FriendList();			// 好友 1 组
	FriendList							m_FriendGroup2 = new FriendList();			// 好友 2 组
	FriendList							m_FriendGroup3 = new FriendList();			// 好友 3 组
	FriendList							m_FriendGroup4 = new FriendList();			// 好友 4 组
	TempRelationList					m_TeamFriendGroup = new TempRelationList();		// 临时好友组
	BlackList							m_BlackListGroup = new BlackList();		// 黑名单组
	SDATA_RELATION_MEMBER				m_ViewPlayer = new SDATA_RELATION_MEMBER();			// 查看玩家
}
