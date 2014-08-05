using System;
using Network;
using Network.Packets;
using System.Runtime.InteropServices;
// 询问帮会列表 
public class GUILD_CGW_ASKLIST : ClassCanbeSerialized
{
	byte					m_SortType;
	ushort					m_uStart;

	short				m_Camp; //查询者的阵营

	public GUILD_CGW_ASKLIST()
	{
		m_SortType = 0;
		m_uStart = 0;
		m_Camp = -1;
	}

	public int getSize(){ return sizeof(byte) + sizeof(ushort) + sizeof(short); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadByte(ref m_SortType);
        buff.ReadUShort(ref m_uStart);
        buff.ReadShort(ref m_Camp);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteByte(m_SortType);
        buff.WriteUShort(m_uStart);
        buff.WriteShort(m_Camp);
         return getSize();
    }
};

// 创建帮会

public class GUILD_CGW_CREATE : ClassCanbeSerialized
{
	byte					m_NameSize;
	byte[]					m_szGuildName = new byte[GAMEDEFINE.MAX_GUILD_NAME_SIZE];

	byte					m_DescSize;
	byte[]					m_szGuildDesc = new byte[GAMEDEFINE.MAX_GUILD_DESC_SIZE];

	short				m_Camp; //帮主的阵营

	GUILD_CGW_CREATE()
	{
		m_NameSize = 0;
		m_DescSize = 0;
		m_Camp = -1;
	}

	public int getSize() { return sizeof(byte)*(m_NameSize+1)  + sizeof(byte)*(m_DescSize + 1)  + sizeof(short); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadByte(ref m_NameSize);
        if(m_NameSize > 0)
        {
            buff.Read(ref m_szGuildName,m_NameSize);
        }

        buff.ReadByte(ref m_DescSize);
        if(m_DescSize > 0)
        {
            buff.Read(ref m_szGuildDesc, m_DescSize);
        }

        buff.ReadShort(ref m_Camp);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteByte(m_NameSize);
        if( m_NameSize > 0 )
        {
            buff.Write(ref m_szGuildName,m_NameSize);
        }
        buff.WriteByte(m_DescSize);
        if( m_DescSize > 0 )
        {
             buff.Write(ref m_szGuildDesc, m_DescSize);
        }

        buff.WriteShort(m_Camp);
         return getSize();
    }
};

// 加入帮会
public class GUILD_CGW_JOIN : ClassCanbeSerialized
{
	short				m_GuildGUID;

	short				m_Camp; //加入者的阵营

	GUILD_CGW_JOIN()
	{
		m_GuildGUID = 0;
		m_Camp = -1;
	}

	public int getSize() { return sizeof(short)*2; }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadShort(ref m_GuildGUID);
        buff.ReadShort(ref m_Camp);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteShort(m_GuildGUID);
        buff.WriteShort(m_Camp);
         return getSize();
    }
};

// 询问帮会信息
public class GUILD_CGW_ASKINFO : ClassCanbeSerialized
{

	public const int GUILD_MEMBER_INFO = 0;	//帮众信息
	public const int  GUILD_INFO =1;			//帮会信息
	public const int  GUILD_APPOINT_POS = 2;	//帮会中可任命的职位
	public const int  GUILD_SELF_INFO = 3;	//本人帮派信息

	
	short				m_GuildGUID;
	byte					m_Type;				//帮众信息，还是帮会信息

	public int getSize() { return sizeof(short)+sizeof(byte); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadShort(ref m_GuildGUID);
        buff.ReadByte(ref m_Type);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteShort(m_GuildGUID);
        buff.WriteByte(m_Type);
        return getSize();
    }
};

// 帮会任免职务
public class GUILD_CGW_APPOINT : ClassCanbeSerialized
{
	short				m_GuildGUID;
	uint					m_CandidateGUID;
	byte					m_NewPosition;

	public int getSize()
	{
        return sizeof(short) + sizeof(uint) + sizeof(byte);
	}
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadShort(ref m_GuildGUID);
        buff.ReadUint(ref m_CandidateGUID);
        buff.ReadByte(ref m_NewPosition);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteShort(m_GuildGUID);
        buff.WriteUint(m_CandidateGUID);
        buff.WriteByte(m_NewPosition);
        return getSize();
    }
};

// 帮会调整权限
public class GUILD_CGW_ADJUSTAUTH : ClassCanbeSerialized
{
	short				m_GuildGUID;
	uint					m_CandidateGUID;
	byte					m_NewAuthority;

	public int getSize()
	{
		return sizeof(short) + sizeof(uint) + sizeof(byte);
	}
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadShort(ref m_GuildGUID);
        buff.ReadUint(ref m_CandidateGUID);
        buff.ReadByte(ref m_NewAuthority);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteShort(m_GuildGUID);
        buff.WriteUint(m_CandidateGUID);
        buff.WriteByte(m_NewAuthority);
        return getSize();
    }
};

// 招收新帮众
public class GUILD_CGW_RECRUIT : ClassCanbeSerialized
{
	uint					m_ProposerGUID;

	public int getSize(){ return sizeof(uint); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadUint(ref m_ProposerGUID);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteUint(m_ProposerGUID);
         return getSize();
    }
};

// 开除帮众
public class GUILD_CGW_EXPEL : ClassCanbeSerialized
{
	uint					m_GuildUserGUID;

	public int getSize(){ return sizeof(uint); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadUint(ref m_GuildUserGUID);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteUint(m_GuildUserGUID);
         return getSize();
    }
};

// 提取帮资
public class GUILD_CGW_WITHDRAW : ClassCanbeSerialized
{
	uint					m_MoneyAmount;

	public int getSize(){ return sizeof(uint); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadUint(ref m_MoneyAmount);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteUint(m_MoneyAmount);
         return getSize();
    }
};

// 存入帮资
public class GUILD_CGW_DEPOSIT : ClassCanbeSerialized
{
	uint					m_MoneyAmount;

	public int getSize(){ return sizeof(uint); }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadUint(ref m_MoneyAmount);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteUint(m_MoneyAmount);
        return getSize();
    }
};

// 离开帮会
public class GUILD_CGW_DEMISE : ClassCanbeSerialized
{
	public int getSize(){ return 0; }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
         return getSize();
    }
};

// 离开帮会
public class GUILD_CGW_LEAVE : ClassCanbeSerialized
{
	public int getSize(){ return 0; }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
         return getSize();
    }
};

// 更改帮会宗旨
public class GUILD_CGW_CHANG_DESC : ClassCanbeSerialized
{
    byte					m_GuildDescLength;
	byte[]					m_GuildDesc = new byte[GAMEDEFINE.MAX_GUILD_DESC_SIZE];
	

	GUILD_CGW_CHANG_DESC()
	{
		m_GuildDescLength	=	0;
	}

	public int getSize(){  return sizeof(byte) *( m_GuildDescLength + 1 ) ; }
	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadByte(ref m_GuildDescLength);
        if(m_GuildDescLength > 0)
        {
            buff.Read(ref m_GuildDesc, m_GuildDescLength);
        }
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteByte(m_GuildDescLength);
        if(m_GuildDescLength > 0)
        {
            buff.Write(ref m_GuildDesc, m_GuildDescLength);
        }
        return getSize();
    }
};


//帮会的可任命信息
public class GUILD_WGC_APPOINT_INFO : ClassCanbeSerialized
{
	public class s
	{
		public byte[]	m_PosName = new byte[GAMEDEFINE.MAX_GUILD_POS_NAME_SIZE];
		public byte	m_PosID;
		public s()
		{
			m_PosID			=	0;
		}	
	}
    int iPosNum;
    s[] m_PosList = new s[(int)GUILD_POSITION.GUILD_POSITION_SIZE];
	
	public GUILD_WGC_APPOINT_INFO()
	{
		iPosNum = 0;
        for(int i=0; i<m_PosList.Length; ++i)
        {
            m_PosList[i] = new s();
        }
	}

	public int getSize()
	{
        return sizeof(int) +
                sizeof(byte) * (GAMEDEFINE.MAX_GUILD_POS_NAME_SIZE + 1) * iPosNum;
	}

	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadInt(ref iPosNum);
        for(int i=0; i<iPosNum; ++i)
        {
            buff.Read(ref m_PosList[i].m_PosName, GAMEDEFINE.MAX_GUILD_POS_NAME_SIZE);
            buff.ReadByte(ref m_PosList[i].m_PosID);
        }
		return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteInt(iPosNum);
        for (int i = 0; i < iPosNum; ++i)
        {
            buff.Write(ref m_PosList[i].m_PosName, GAMEDEFINE.MAX_GUILD_POS_NAME_SIZE);
            buff.WriteByte(m_PosList[i].m_PosID);
        }
		return getSize();
    }
}

//帮会信息
public class GUILD_WGC_GUILD_INFO : ClassCanbeSerialized
{
	byte[]			m_GuildName = new byte[GAMEDEFINE.MAX_GUILD_NAME_SIZE];
	byte[]			m_GuildCreator = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];
	byte[]			m_GuildChairMan = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];
	byte[]			m_CityName = new byte[GAMEDEFINE_CITY.MAX_CITY_NAME_SIZE];
	byte			m_nLevel;
	int				m_nPortSceneID;				//入口场景
	int				m_MemNum;					//人数
	int				m_Longevity;				//资历 
	int				m_Contribute;				//贡献度
	int				m_Honor;					//人气
	int				m_FoundedMoney;				//帮派资金
	int				m_nIndustryLevel;			//工业度
	int				m_nAgrLevel;				//农业度
	int				m_nComLevel;				//商业度
	int				m_nDefLevel;				//防卫度
	int				m_nTechLevel;				//科技度
	int				m_nAmbiLevel;				//扩张度
	byte			m_bAccess;

	public GUILD_WGC_GUILD_INFO()
	{
		CleanUp();
	}
	public void CleanUp()
	{
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
		m_bAccess			=	0;
	}

	public int getSize()
	{
        return sizeof(int) * 12 +
            sizeof(byte) *
            (2 + GAMEDEFINE.MAX_CHARACTER_NAME
            + GAMEDEFINE.MAX_CHARACTER_NAME
            + GAMEDEFINE_CITY.MAX_CITY_NAME_SIZE
            + GAMEDEFINE.MAX_GUILD_NAME_SIZE);
	}

	public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.Read(ref m_GuildName, GAMEDEFINE.MAX_GUILD_NAME_SIZE);
        buff.Read(ref (m_GuildCreator), GAMEDEFINE.MAX_CHARACTER_NAME);
        buff.Read(ref (m_GuildChairMan), GAMEDEFINE.MAX_CHARACTER_NAME);
        buff.Read(ref  (m_CityName), GAMEDEFINE_CITY.MAX_CITY_NAME_SIZE);
        buff.ReadByte( ref m_nLevel);
        buff.ReadInt(ref (m_nPortSceneID));
        buff.ReadInt(ref (m_MemNum));
        buff.ReadInt(ref (m_Longevity));
        buff.ReadInt(ref (m_Contribute));
        buff.ReadInt(ref (m_Honor));
        buff.ReadInt(ref (m_FoundedMoney));
        buff.ReadInt(ref (m_nIndustryLevel));
        buff.ReadInt(ref (m_nAgrLevel));
        buff.ReadInt(ref (m_nComLevel));
        buff.ReadInt(ref (m_nDefLevel));
        buff.ReadInt(ref (m_nTechLevel));
        buff.ReadInt(ref (m_nAmbiLevel));
        buff.ReadByte(ref m_bAccess);
        return true;
    }
	public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.Write(ref m_GuildName, GAMEDEFINE.MAX_GUILD_NAME_SIZE);
        buff.Write(ref (m_GuildCreator), GAMEDEFINE.MAX_CHARACTER_NAME);
        buff.Write(ref (m_GuildChairMan), GAMEDEFINE.MAX_CHARACTER_NAME);
        buff.Write(ref  (m_CityName), GAMEDEFINE_CITY.MAX_CITY_NAME_SIZE);
        buff.WriteByte( m_nLevel);
        buff.WriteInt( (m_nPortSceneID) );
        buff.WriteInt( (m_MemNum));
        buff.WriteInt( (m_Longevity));
        buff.WriteInt( (m_Contribute));
        buff.WriteInt( (m_Honor));
        buff.WriteInt( (m_FoundedMoney));
        buff.WriteInt( (m_nIndustryLevel));
        buff.WriteInt( (m_nAgrLevel));
        buff.WriteInt( (m_nComLevel));
        buff.WriteInt( (m_nDefLevel));
        buff.WriteInt( (m_nTechLevel));
        buff.WriteInt( (m_nAmbiLevel));
        buff.WriteByte( m_bAccess);
        return getSize();
    }
};
/*
// 帮众列表
public class GUILD_WGC_MEMBER_LIST : ClassCanbeSerialized
{
	public class s
	{
		byte		m_szName[MAX_CHARACTER_NAME];
		uint		m_Guid;
		BYTE		m_bLevel;
		byte		m_bMenPaiID;
		int			m_iCurContribute;
		int			m_iMaxContribute;
		int			m_iJoinTime;
		int			m_iLogOutTime;
		BYTE		m_bIsOnline;
		BYTE		m_bPosition;
		s()
		{
			memset(m_szName, 0, MAX_CHARACTER_NAME);
			m_Guid				= UINT_MAX;
			m_bLevel			=	0;
			m_bMenPaiID			=	-1;
			m_iCurContribute	=	0;
			m_iMaxContribute	=	0;
			m_iJoinTime			=	0;
			m_iLogOutTime		=	0;
			m_bIsOnline			=	0;
			m_bPosition			=	0;	
		}
	}	m_GuildMemberData[USER_ARRAY_SIZE];

	ushort			m_uValidMemberCount;
	ushort			m_uMemberCount;
	ushort			m_uMemberMax;
	byte			m_GuildDesc[MAX_GUILD_DESC_SIZE];
	byte			m_GuildName[MAX_GUILD_NAME_SIZE];
	BYTE			m_bPosition;
	BYTE			m_bAccess;

	GUILD_WGC_MEMBER_LIST()
	{
		m_uValidMemberCount	=	0;
		m_uMemberCount	=	0;
		m_uMemberMax	=	0;
		m_bPosition		=	0;
		m_bAccess		=	0;
		memset(m_GuildDesc, 0, MAX_GUILD_DESC_SIZE);
		memset(m_GuildName, 0, MAX_GUILD_NAME_SIZE);
	}

	virtual uint			GetPacketSize() const
	{
		return	sizeof(m_uValidMemberCount) +
				sizeof(m_uMemberCount) + 
				sizeof(m_uMemberMax) + 
				sizeof(m_bPosition) +
				sizeof(m_bAccess) +
				MAX_GUILD_DESC_SIZE + 
				MAX_GUILD_NAME_SIZE + 
				m_uMemberCount * sizeof(s);
	}

	virtual void			Read(SocketInputStream& iStream);
	virtual void			Write(SocketOutputStream& oStream) const;

};

// 帮会列表
public class GUILD_WGC_LIST : ClassCanbeSerialized
{
	ushort					m_uStartIndex;
	ushort					m_uGuildCount;
	byte					m_uGuildListCount;
	short				m_Camp;

	public class s
	{
		s()
		{
			m_GuildID = INVALID_ID;
			memset((void*)m_GuildName, 0, sizeof(m_GuildName));
			memset((void*)m_GuildDesc, 0, sizeof(m_GuildDesc));
			memset((void*)m_CityName, 0, sizeof(m_CityName));
			memset((void*)m_ChiefName, 0, sizeof(m_ChiefName));
			m_uGuildStatus		= 0;
			m_uGuildUserCount	= 0;
			m_bGuildLevel		= 0;
			m_PortSceneID		= -1;
			m_nFoundedTime		=	0;
		}

		short			m_GuildID;
		byte				m_ChiefName[MAX_CHARACTER_NAME];
		byte				m_GuildName[MAX_GUILD_NAME_SIZE];
		byte				m_GuildDesc[MAX_GUILD_DESC_SIZE];
		byte				m_CityName[MAX_CITY_NAME_SIZE];
		int					m_PortSceneID;
		byte				m_uGuildStatus;
		byte				m_uGuildUserCount;
		BYTE				m_bGuildLevel;
		int					m_nFoundedTime;
	}		m_uGuild[MAX_GUILD_LIST_COUNT];

	GUILD_WGC_LIST() 
	{
		m_uStartIndex = 0;
		m_uGuildCount = 0;
		m_uGuildListCount = 0;
		m_Camp = -1;
	}

	virtual uint			GetPacketSize() const
	{
		return sizeof(m_uStartIndex) + sizeof(m_uGuildCount) + sizeof(m_uGuildListCount) + sizeof(m_Camp)
			 + m_uGuildListCount * sizeof(s);
	}

	virtual void			Read(SocketInputStream& iStream);
	virtual void			Write(SocketOutputStream& oStream) const;
};

//个人帮派数据
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class GUILD_WGC_SELF_GUILD_INFO : ClassCanbeSerialized
{
	byte			m_GuildName[MAX_GUILD_NAME_SIZE];
	BYTE			m_GuildNameSize;
	BYTE			m_bAccess;

	GUILD_WGC_SELF_GUILD_INFO()
	{
		CleanUp();
	}
	void CleanUp()
	{
		memset(m_GuildName, 0, MAX_GUILD_NAME_SIZE);
		m_GuildNameSize = 0;
		m_bAccess = 0;
	}

	virtual uint			GetPacketSize() const
	{
		return	sizeof(BYTE) + sizeof(BYTE) + m_GuildNameSize;
	}

	virtual void			Read(SocketInputStream& iStream);
	virtual void			Write(SocketOutputStream& oStream) const;
};
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class _GUILD_PACKET
{
	byte					m_uPacketType;			// 消息包类型
	virtual GUILD_PACKET*	GetPacket(int nPacketType) const = 0;

	uint					GetPacketSize() const
	{
		GUILD_PACKET*		pGuildPacket;

		pGuildPacket = GetPacket(m_uPacketType);
		if( pGuildPacket == NULL )
		{
			Assert( FALSE );
			return 0;
		}

		return (sizeof(m_uPacketType) + pGuildPacket->GetPacketSize());
	}

	void					Read(SocketInputStream& iStream);
	void					Write(SocketOutputStream& oStream) const;
};
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class _GUILD_CGW_PACKET : public _GUILD_PACKET
{
	GUILD_CGW_ASKLIST				m_PacketAskList;
	GUILD_CGW_CREATE				m_PacketCreate;
	GUILD_CGW_JOIN					m_PacketJoin;
	GUILD_CGW_ASKINFO				m_PacketAskInfo;
	GUILD_CGW_APPOINT				m_PacketAppoint;
	GUILD_CGW_ADJUSTAUTH			m_PacketJustAuthority;
	GUILD_CGW_RECRUIT				m_PacketRecruit;
	GUILD_CGW_EXPEL					m_PacketExpel;
	GUILD_CGW_WITHDRAW				m_PacketWithdraw;
	GUILD_CGW_DEPOSIT				m_PacketDeposit;
	GUILD_CGW_LEAVE					m_PacketLeave;
	GUILD_CGW_DEMISE				m_PacketDemise;
	GUILD_CGW_CHANG_DESC			m_PacketChangeDesc;

	union u
	{ // 比最大的消息包长了一个虚表长度
		byte				u_AskListSize[sizeof(GUILD_CGW_ASKLIST)];
		byte				u_CreateSize[sizeof(GUILD_CGW_CREATE)];
		byte				u_JoinSize[sizeof(GUILD_CGW_JOIN)];
		byte				u_AskInfoSize[sizeof(GUILD_CGW_ASKINFO)];
		byte				u_AppointSize[sizeof(GUILD_CGW_APPOINT)];
		byte				u_JustAuthoritySize[sizeof(GUILD_CGW_ADJUSTAUTH)];
		byte				u_RecruitSize[sizeof(GUILD_CGW_RECRUIT)];
		byte				u_ExpelSize[sizeof(GUILD_CGW_EXPEL)];
		byte				u_WithdrawSize[sizeof(GUILD_CGW_WITHDRAW)];
		byte				u_DepositSize[sizeof(GUILD_CGW_DEPOSIT)];
		byte				u_LeaveSize[sizeof(GUILD_CGW_LEAVE)];
		byte				u_DemiseSize[sizeof(GUILD_CGW_DEMISE)];
		byte				u_ChangeDescSize[sizeof(GUILD_CGW_CHANG_DESC)];
	};

	_GUILD_CGW_PACKET() {}

	virtual GUILD_PACKET*	GetPacket(int nPacketType) const
	{
		GUILD_PACKET*		pGuildPacket;

		switch(nPacketType)
		{
		case GUILD_PACKET_CG_ASKLIST:
		case GUILD_PACKET_GW_ASKLIST: pGuildPacket = (GUILD_PACKET*)&m_PacketAskList; break;

		case GUILD_PACKET_CG_CREATE:
		case GUILD_PACKET_GW_CREATE: pGuildPacket = (GUILD_PACKET*)&m_PacketCreate; break;
		
		case GUILD_PACKET_CG_JOIN:
		case GUILD_PACKET_GW_JOIN: pGuildPacket = (GUILD_PACKET*)&m_PacketJoin; break;
		
		case GUILD_PACKET_CG_ASKINFO:
		case GUILD_PACKET_GW_ASKINFO: pGuildPacket = (GUILD_PACKET*)&m_PacketAskInfo; break;
		
		case GUILD_PACKET_CG_APPOINT:
		case GUILD_PACKET_GW_APPOINT: pGuildPacket = (GUILD_PACKET*)&m_PacketAppoint; break;
		
		case GUILD_PACKET_CG_ADJUSTAUTHORITY:
		case GUILD_PACKET_GW_ADJUSTAUTHORITY: pGuildPacket = (GUILD_PACKET*)&m_PacketJustAuthority; break;
		
		case GUILD_PACKET_CG_RECRUIT:
		case GUILD_PACKET_GW_RECRUIT: pGuildPacket = (GUILD_PACKET*)&m_PacketRecruit; break;
		
		case GUILD_PACKET_CG_EXPEL:
		case GUILD_PACKET_GW_EXPEL: pGuildPacket = (GUILD_PACKET*)&m_PacketExpel; break;
		
		case GUILD_PACKET_CG_WITHDRAW:
		case GUILD_PACKET_GW_WITHDRAW: pGuildPacket = (GUILD_PACKET*)&m_PacketWithdraw; break;
		
		case GUILD_PACKET_CG_DEPOSIT:
		case GUILD_PACKET_GW_DEPOSIT: pGuildPacket = (GUILD_PACKET*)&m_PacketDeposit; break;
		
		case GUILD_PACKET_CG_LEAVE:
		case GUILD_PACKET_GW_LEAVE: pGuildPacket = (GUILD_PACKET*)&m_PacketLeave; break;
		
		case GUILD_PACKET_CG_DISMISS:
		case GUILD_PACKET_GW_DISMISS: pGuildPacket = (GUILD_PACKET*)&m_PacketLeave; break;
		
		case GUILD_PACKET_CG_DEMISE:
		case GUILD_PACKET_GW_DEMISE: pGuildPacket = (GUILD_PACKET*)&m_PacketDemise; break;
		
		case GUILD_PACKET_CG_CHANGEDESC:
		case GUILD_PACKET_GW_CHANGEDESC: pGuildPacket = (GUILD_PACKET*)&m_PacketChangeDesc; break;
		
		default:
			Assert( FALSE );
			pGuildPacket = NULL;
		}

		return pGuildPacket;
	}

	static uint				GetPacketMaxSize()
	{
		return (sizeof(byte) + sizeof(u) - sizeof(GUILD_PACKET));
	}

};
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class _GUILD_WGC_PACKET : public _GUILD_PACKET
{
	GUILD_WGC_LIST				m_PacketList;
	GUILD_WGC_MEMBER_LIST		m_PacketMemberList;
	GUILD_WGC_GUILD_INFO		m_PacketGuildInfo;
	GUILD_WGC_APPOINT_INFO		m_PacketAppointList;
	GUILD_WGC_SELF_GUILD_INFO	m_PacketSelfGuildInfo;

	union u
	{ // 比最大的消息包长了一个虚表长度
		byte				u_ListSize[sizeof(GUILD_WGC_LIST)];
		byte				u_MemberListSize[sizeof(GUILD_WGC_MEMBER_LIST)];
		byte				u_GuildInfoSize[sizeof(GUILD_WGC_GUILD_INFO)];
		byte				u_AppointInfoSize[sizeof(GUILD_WGC_APPOINT_INFO)];
		byte				u_SelfGuildInfoSize[sizeof(GUILD_WGC_SELF_GUILD_INFO)];
	};

	_GUILD_WGC_PACKET() {}

	virtual GUILD_PACKET*	GetPacket(int nPacketType) const
	{
		GUILD_PACKET*		pGuildPacket;

		switch(nPacketType)
		{
		case GUILD_PACKET_WG_LIST:
		case GUILD_PACKET_GC_LIST: pGuildPacket = (GUILD_PACKET*)&m_PacketList; break;

		case GUILD_PACKET_WG_MEMBER_LIST:
		case GUILD_PACKET_GC_MEMBER_LIST: pGuildPacket = (GUILD_PACKET*)&m_PacketMemberList; break;

		case GUILD_PACKET_WG_GUILD_INFO:
		case GUILD_PACKET_GC_GUILD_INFO: pGuildPacket = (GUILD_PACKET*)&m_PacketGuildInfo; break;

		case GUILD_PACKET_WG_APPOINT_INFO:
		case GUILD_PACKET_GC_APPOINT_INFO: pGuildPacket = (GUILD_PACKET*)&m_PacketAppointList; break;

		case GUILD_PACKET_WG_SELF_GUILD_INFO:
		case GUILD_PACKET_GC_SELF_GUILD_INFO: pGuildPacket = (GUILD_PACKET*)&m_PacketSelfGuildInfo; break;

		default:
			Assert( FALSE );
			pGuildPacket = NULL;
		}

		return pGuildPacket;
	}

	static uint				GetPacketMaxSize()
	{
		return (sizeof(byte) + sizeof(u) - sizeof(GUILD_PACKET));
	}

};

public class _GUILD_RETURN
{
	byte					m_ReturnType;
	short				m_GuildID;
	uint					m_GUID; // 需要重设 GuildID 的玩家的 GUID
	uint					m_GUIDChanged; // 需要重设 GuildID 的玩家的 GUID
	BYTE					m_PosID;
	byte					m_SourNameSize;
	byte					m_SourName[MAX_CHARACTER_NAME];
	byte					m_DestNameSize;
	byte					m_DestName[MAX_CHARACTER_NAME];
	byte					m_GuildNameSize;
	byte					m_GuildName[MAX_GUILD_NAME_SIZE];
	byte					m_PositionNameSize;
	byte					m_PositionName[MAX_GUILD_POS_NAME_SIZE];
	byte					m_ChangedPositionNameSize;
	byte					m_ChangedPositionName[MAX_GUILD_POS_NAME_SIZE];

	_GUILD_RETURN()
	{
		m_ReturnType	=	GUILD_RETURN_INVALID;
		m_GuildID		=	INVALID_ID;
		m_GUID			=	UINT_MAX;
		m_GUIDChanged	=	UINT_MAX;
		m_PosID			=	0;
		m_SourNameSize	=	0;
		memset((void*)m_SourName, 0, sizeof(m_SourName));
		m_DestNameSize = 0;
		memset((void*)m_DestName, 0, sizeof(m_DestName));
		m_GuildNameSize = 0;
		memset((void*)m_GuildName, 0, sizeof(m_GuildName));
		m_PositionNameSize	=	0;
		memset((void*)m_PositionName, 0, sizeof(m_PositionName));	
		m_ChangedPositionNameSize	=	0;
		memset((void*)m_ChangedPositionName, 0, sizeof(m_ChangedPositionNameSize));	

	}

	uint					GetPacketSize() const
	{
		uint uSize = sizeof(m_ReturnType);

		switch( m_ReturnType )
		{
		case GUILD_RETURN_CREATE:
		case GUILD_RETURN_JOIN:
			{
				uSize +=	sizeof(m_GuildID)
							+ sizeof(m_GuildNameSize) + m_GuildNameSize;
			}
			break;
		case GUILD_RETURN_RESPONSE:
			{
				uSize += sizeof(m_GuildID) + sizeof(m_GUID) + sizeof(m_SourNameSize) + m_SourNameSize;
			}
			break;
		case GUILD_RETURN_EXPEL:
			{
				uSize +=  sizeof(m_SourNameSize) + m_SourNameSize
						+ sizeof(m_DestNameSize) + m_DestNameSize
						+ sizeof(m_GUID);
			}
			break;
		case GUILD_RETURN_REJECT:
			{
				uSize +=  sizeof(m_GuildNameSize) + m_GuildNameSize
					+ sizeof(m_DestNameSize) + m_DestNameSize
					+ sizeof(m_GUID);
			}
			break;
		case GUILD_RETURN_RECRUIT:
			{
				uSize += sizeof(m_GuildID)
						+ sizeof(m_GUIDChanged)
						+ sizeof(m_SourNameSize) + m_SourNameSize
						+ sizeof(m_DestNameSize) + m_DestNameSize
						+ sizeof(m_GuildNameSize) + m_GuildNameSize
						+ sizeof(m_PositionNameSize) + m_PositionNameSize
						+ sizeof(m_GUID);
			}
			break;
		case GUILD_RETURN_LEAVE:
			{
				uSize +=  sizeof(m_GUID) 
						+ sizeof(m_GuildNameSize) + m_GuildNameSize
						+ sizeof(m_DestNameSize) + m_DestNameSize;
			}
			break;
		case GUILD_RETURN_FOUND:
			{

			}
			break;
		case GUILD_RETURN_DISMISS:
			{
			
			}
			break;
		case GUILD_RETURN_PROMOTE:
			{
				uSize +=  sizeof(m_SourNameSize) + m_SourNameSize
						+ sizeof(m_DestNameSize) + m_DestNameSize
						+ sizeof(m_PositionNameSize) + m_PositionNameSize
						+ sizeof(m_GuildNameSize) + m_GuildNameSize
						+ sizeof(m_PosID)
						+ sizeof(m_GUID)
						+ sizeof(m_GuildID);
			}
			break;
		case GUILD_RETURN_DEMOTE:
			{
				uSize +=  sizeof(m_SourNameSize) + m_SourNameSize
						+ sizeof(m_DestNameSize) + m_DestNameSize
						+ sizeof(m_PositionNameSize) + m_PositionNameSize
						+ sizeof(m_GuildNameSize) + m_GuildNameSize
						+ sizeof(m_PosID)
						+ sizeof(m_GUID)
						+ sizeof(m_GuildID);
			}
			break;
		case GUILD_RETURN_AUTHORIZE:
		case GUILD_RETURN_DEPRIVE_AUTHORITY:
			{
			}
			break;
		case GUILD_RETURN_DEMISE:
			{
				uSize += sizeof(m_GuildID)
					+ sizeof(m_GUID)
					+ sizeof(m_GUIDChanged)
					+ sizeof(m_SourNameSize) + m_SourNameSize
					+ sizeof(m_DestNameSize) + m_DestNameSize
					+ sizeof(m_GuildNameSize) + m_GuildNameSize
					+ sizeof(m_PositionNameSize) + m_PositionNameSize
					+ sizeof(m_ChangedPositionNameSize) + m_ChangedPositionNameSize;
			}
			break;
		case GUILD_RETURN_NAME:
			{

			}
			break;
		case GUILD_RETURN_WITHDRAW:
		case GUILD_RETURN_DEPOSIT:
		default:
			Assert(FALSE);
			return 0;
		}

		return uSize;
	}

	void					Read(SocketInputStream& iStream);
	void					Write(SocketOutputStream& oStream) const;
};

public class GUILD_POS_t
{
	BYTE	NumPos;
	BYTE	MaxNumPos;
	byte	szPosName[MAX_GUILD_POS_NAME_SIZE];
	int		Posidx[MAX_NUM_PER_POSITION];

	void CleanUp()
	{
		NumPos = 0;
		MaxNumPos = 0;
		memset(szPosName, 0, MAX_GUILD_POS_NAME_SIZE);
		for(int i = 0; i<MAX_NUM_PER_POSITION; i++)
		{
			Posidx[i] = -1;
		}
	}
};

//不同级别帮会对应的不同的行政结构
public class GUILD_ADMIN_t
{
	enum ORESULT
	{
		RET_SUCCESS = 0,
		RET_NOT_EXIST,	//此人在本职位不存在
		RET_POS_FULL,	//职位人都满了
		RET_NO_USER,	//玩家不存在

		RET_UNKOWN,
	};
	enum
	{
		POS_LEVEL0_BEGIN = 0,

		POS_CHAIR_MAN,				//帮主
		POS_ASS_CHAIR_MAN ,			//副帮主
		POS_MEMBER,					//帮众

		POS_LEVEL1_BEGIN,

		POS_ELITE_MEMBER,			//精英帮众
		POS_COM,					//商业官员
		POS_AGRI,					//农业官员
		POS_INDUSTRY,				//工业官员
		POS_HR,						//人事官员

		POS_LEVEL2_BEGIN,

		POS_LEVEL_END,						
	};

	GUILD_POS_t		m_PosList[POS_LEVEL_END];

	GUILD_POS_t*	Get(int iPos)
	{
		return &m_PosList[iPos];
	}

	ORESULT	Remove(int iPos, int iIndex)
	{
		GUILD_POS_t* pGuildPos = Get(iPos);

		for(int i = 0; i<pGuildPos->MaxNumPos; i++)
		{
			if(pGuildPos->Posidx[i] == iIndex)
			{
				pGuildPos->Posidx[i] = pGuildPos->Posidx[pGuildPos->NumPos-1];
				pGuildPos->Posidx[pGuildPos->NumPos-1] = -1;
				pGuildPos->NumPos--;
				return RET_SUCCESS;
			}
		}
		return RET_NOT_EXIST;
	}


	ORESULT	Add(int iPos, int iIndex)
	{
		GUILD_POS_t* pGuildPos = Get(iPos);
		//{----------------------------------------------------------------------------
		// [2010-12-10] by: cfp+ 
		Assert(pGuildPos);
		if ( NULL == pGuildPos )
		{
			return RET_UNKOWN;
		}
		//----------------------------------------------------------------------------}

		if(pGuildPos->NumPos >= MAX_NUM_PER_POSITION || pGuildPos->NumPos == pGuildPos->MaxNumPos)
			return RET_POS_FULL;
		pGuildPos->Posidx[pGuildPos->NumPos] = iIndex;
		pGuildPos->NumPos++;
		return RET_SUCCESS;
	}

	void CleanUp(int iLevelBegin = POS_LEVEL0_BEGIN)
	{
		for(int i = iLevelBegin; i<POS_LEVEL_END; i++)
		{
			m_PosList[i].CleanUp();
		}
	}
};*/
