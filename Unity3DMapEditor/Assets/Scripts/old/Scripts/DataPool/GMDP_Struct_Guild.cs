//可任命的帮派职位
public class GuildAppointPos_t
{
public 	string		m_szPosName;			//职位名
public int m_PosID;				//职位ID
public GuildAppointPos_t()
	{
		CleanUp();
	}
public void CleanUp()
	{
		m_PosID			= -1;
		m_szPosName		= "";
	}
};

//帮派的详细信息
public class GuildDetailInfo_t
{
    public string m_GuildName;
    public string m_GuildCreator;
    public string m_GuildChairMan;
    public string m_CityName;
    public byte m_nLevel;
    public int m_nPortSceneID;				//入口场景
    public int m_MemNum;					//人数
    public int m_Longevity;				//资历 
    public int m_Contribute;				//贡献度
    public int m_Honor;					//人气
    public int m_FoundedMoney;				//帮派资金
    public int m_nIndustryLevel;			//工业度
    public int m_nAgrLevel;				//农业度
    public int m_nComLevel;				//商业度
    public int m_nDefLevel;				//防卫度
    public int m_nTechLevel;				//科技度
    public int m_nAmbiLevel;				//扩张度

    public GuildDetailInfo_t()
	{
		CleanUp();
	}

    public void CleanUp()
	{
		m_GuildName			= "";
		m_GuildCreator		= "";
		m_GuildChairMan		= "";
		m_CityName			= "";
		m_nLevel			=	0;
		m_nPortSceneID		=	0;			//入口场景
		m_MemNum			=	0;			//人数
		m_Longevity			=	0;			//资历 
		m_Contribute		=	0;			//贡献度
		m_Honor				=	0;			//人气
		m_FoundedMoney		=	0;			//帮派资金
		m_nIndustryLevel	=	0;			//工业度
		m_nAgrLevel			=	0;			//农业度
		m_nComLevel			=	0;			//商业度
		m_nDefLevel			=	0;			//防卫度
		m_nTechLevel		=	0;			//科技度
		m_nAmbiLevel		=	0;			//扩张度
	}

};

//帮众信息
public class GuildMemberInfo_t
{
    public string m_szName;
    public uint m_Guid;
    public byte m_bLevel;
    public byte m_bMenPaiID;
    public int m_iCurContribute;
    public int m_iMaxContribute;
    public int m_iJointime;
    public int m_iLogOutTime;
    public byte m_bIsOnline;
    public byte m_bPosition;

    public GuildMemberInfo_t()
	{
		CleanUp();
	}
    public void CleanUp()
	{
		m_szName			=	"";
        m_Guid = MacroDefine.UINT_MAX;  
		m_bLevel			=	0;
		m_bMenPaiID			=	0;
		m_iCurContribute	=	0;
		m_iMaxContribute	=	0;
		m_iJointime			=	0;
		m_iLogOutTime		=	0;
		m_bIsOnline			=	0;
		m_bPosition			=	0;
	}

};

//工会列表中每一项的内容
public class GuildInfo_t
{
    public int m_GuildID;
    public string m_ChiefName;
    public string m_GuildName;
    public string m_GuildDesc;
    public string m_CityName;
    public int m_PortSceneID;
    public byte m_uGuildStatus;
    public byte m_uGuildUserCount;
    public byte m_bGuildLevel;
    public int m_nFoundedTime;

    public GuildInfo_t()
	{
		CleanUp();
	}

    public void CleanUp()
	{
		m_GuildID			= MacroDefine.INVALID_ID;
		m_ChiefName			=	"";
		m_GuildName			=	"";
		m_GuildDesc			=	"";
		m_CityName			=	"";
		m_uGuildStatus		=	0;
		m_uGuildUserCount	=	0;
		m_bGuildLevel		=	0;
		m_PortSceneID		=	-1;
		m_nFoundedTime		=	0;
	}
};