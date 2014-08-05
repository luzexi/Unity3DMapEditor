/***********************************\
*									*
*		帮会相关接口				*
*		2006-03-24					*
*									*
\***********************************/
using System.Collections;
using System.Collections.Generic;


// 	//only for tolua++
// 	// 为了导出结构给lua，需要建立一个原数据结构的映射，用于导出
// 	// 这样定义后同时需要修改lua脚本的代码[6/8/2010 Sun]
// 	// 更多信息搜索[struct for tolua++]
// 	struct Lua_GuildInfo{//tolua_export
// 	
// 		//tolua_begin
// 		int					m_GuildID;
// 		std::string			m_ChiefName;
// 		std::string			m_GuildName;
// 		std::string			m_GuildDesc;
// 		std::string			m_CityName;
// 		int					m_PortSceneID;
// 		UCHAR				m_uGuildStatus;
// 		int					m_uGuildUserCount;
// 		BYTE				m_bGuildLevel;
// 		std::string			m_FoundedTime;
// 		//tolua_end
// 
// 		Lua_GuildInfo()
// 		{
// 			CleanUp();
// 		}
// 
// 		void CleanUp()
// 		{
// 			m_GuildID			= INVALID_ID;
// 			m_ChiefName			=	"";
// 			m_GuildName			=	"";
// 			m_GuildDesc			=	"";
// 			m_CityName			=	"";
// 			m_uGuildStatus		=	0;
// 			m_uGuildUserCount	=	0;
// 			m_bGuildLevel		=	0;
// 			m_PortSceneID		=	-1;
// 			m_FoundedTime		=	"";
// 		}
// 
// 	};//tolua_export
// 
// 	//only for tolua++
// 	// 更多信息搜索[struct for tolua++]
// 	struct Lua_GuildMemberInfo{//tolua_export
// 	
// 		//tolua_begin
// 		std::string	m_szName;
// 		UINT		m_Guid;
// 		BYTE		m_bLevel;
// 		BYTE		m_bMenPaiID;
// 		int			m_iCurContribute;
// 		int			m_iMaxContribute;
// 		std::string	m_JoinTime;
// 		std::string	m_LogOutTime;
// 		BYTE		m_bIsOnline;
// 		BYTE		m_bPosition;
// 		std::string	m_ShowColour;
// 		//tolua_end
// 
// 		Lua_GuildMemberInfo()
// 		{
// 			CleanUp();
// 		}
// 		void CleanUp()
// 		{
// 			m_szName			=	"";
// 			m_Guid				=	INVALID_ID;
// 			m_bLevel			=	0;
// 			m_bMenPaiID			=	0;
// 			m_iCurContribute	=	0;
// 			m_iMaxContribute	=	0;
// 			m_JoinTime			=	"";
// 			m_LogOutTime		=	"";
// 			m_bIsOnline			=	0;
// 			m_bPosition			=	0;
// 			m_ShowColour		=   "";
// 		}
// 
// 	};//tolua_export
// 
// 	//帮派的详细信息
// 	//only for tolua++
// 	// 更多信息搜索[struct for tolua++]
// 	struct Lua_GuildDetailInfo{//tolua_export
// 
// 		//tolua_begin
// 		std::string			m_GuildName;
// 		std::string			m_GuildCreator;
// 		std::string			m_GuildChairMan;
// 		std::string			m_CityName;
// 		BYTE			m_nLevel;
// 		int				m_nPortSceneID;				//入口场景
// 		int				m_MemNum;					//人数
// 		int				m_Longevity;				//资历 
// 		int				m_Contribute;				//贡献度
// 		int				m_Honor;					//人气
// 		int				m_FoundedMoney;				//帮派资金
// 		int				m_nIndustryLevel;			//工业度
// 		int				m_nAgrLevel;				//农业度
// 		int				m_nComLevel;				//商业度
// 		int				m_nDefLevel;				//防卫度
// 		int				m_nTechLevel;				//科技度
// 		int				m_nAmbiLevel;				//扩张度
// 		//tolua_end
// 
// 		Lua_GuildDetailInfo()
// 		{
// 			CleanUp();
// 		}
// 
// 		void CleanUp()
// 		{
// 			m_GuildName			= "";
// 			m_GuildCreator		= "";
// 			m_GuildChairMan		= "";
// 			m_CityName			= "";
// 			m_nLevel			=	0;
// 			m_nPortSceneID		=	0;			//入口场景
// 			m_MemNum			=	0;			//人数
// 			m_Longevity			=	0;			//资历 
// 			m_Contribute		=	0;			//贡献度
// 			m_Honor				=	0;			//人气
// 			m_FoundedMoney		=	0;			//帮派资金
// 			m_nIndustryLevel	=	0;			//工业度
// 			m_nAgrLevel			=	0;			//农业度
// 			m_nComLevel			=	0;			//商业度
// 			m_nDefLevel			=	0;			//防卫度
// 			m_nTechLevel		=	0;			//科技度
// 			m_nAmbiLevel		=	0;			//扩张度
// 		}
// 
// 	};//tolua_export
    public enum ERR_GUILD
	{
		ERR_GUILD_ALREADYIN_MSG = 0,	//玩家已经在一个帮会中
		ERR_GUILD_NOTIN_MSG,			//玩家不在帮会中
		ERR_GUILD_NOPOW_MSG,			//你的权限不够
		ERR_GUILD_NOTHAVEASSCHIEF_MSG,	//没有副帮主

		ERR_GUILD_CREATE_LEVEL_TOO_LOW,	//创建等级过低
		ERR_GUILD_NAME_EMPTY,			//帮会名为空
		ERR_GUILD_NAME_INVALID,			//名字里含有敏感词
		ERR_GUILD_NAME_CANTUSE,			//名字是完全过滤表中的词
		ERR_GUILD_DESC_EMPTY,			//描述为空
		ERR_GUILD_DESC_INVALID,			//描述里含有敏感词
		ERR_GUILD_MONEY_NOT_ENOUGH,		//建立帮会金钱不足

		ERR_GUILD_JOIN_LEVEL_TOO_LOW,	//加入等级过低
		ERR_GUILD_POW_NORECRUIT,		//没有权限收人
		ERR_GUILD_POW_NOEXPEL,			//没有权限踢人
	};

 	public class Guild
    {
 		//创建帮会
		public int	CreateGuild(string lpGuildName, string lpGuildDesc)
        {
            const int E_FALSE = -1;
		    string szGuildName = lpGuildName;
		    string szGuildDesc = lpGuildDesc;

		    if(szGuildName.Length>0 && szGuildDesc.Length>0)
		    {
			    //帮会创建资格检查
			    if( CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level() < 40 )
			    {
                    ShowMsg(ERR_GUILD.ERR_GUILD_CREATE_LEVEL_TOO_LOW, false, true);
				    return E_FALSE;
			    }

			    if(MacroDefine.INVALID_ID == CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Guild())
			    {
				    //完全匹配过滤
// 				    if(!GameProcedure.s_pUISystem.CheckStringFullCompare(szGuildName, "guild"))
// 				    {
// 					    ShowMsg(ERR_GUILD.ERR_GUILD_NAME_CANTUSE);
// 					    return E_FALSE;
// 				    }
                    
				    //敏感字符过滤
// 				    if(!GameProcedure.s_pUISystem.CheckStringFilter(szGuildName))
// 				    {
// 					    ShowMsg(ERR_GUILD.ERR_GUILD_NAME_INVALID);
// 					    return E_FALSE;
// 				    }

// 				    //敏感字符过滤
// 				    if(!GameProcedure.s_pUISystem.CheckStringFilter(szGuildDesc))
// 				    {
// 					    ShowMsg(ERR_GUILD.ERR_GUILD_DESC_INVALID);
// 					    return E_FALSE;
// 				    }

// 				    //名称非法字符过滤
// 				    if(!TDU_CheckStringValid(szGuildName.c_str()))
// 				    {
// 					    ShowMsg(ERR_GUILD.ERR_GUILD_NAME_INVALID);
// 					    return E_FALSE;
// 				    }

				    m_MsgArray.Add(szGuildName);
				    m_MsgArray.Add(szGuildDesc);

				    //显示确认框
				    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GUILD_CREATE_CONFIRM, szGuildName);

				    return 1;
			    }
			    else
			    {
				    //已经拥有一个帮会MSG
                    ShowMsg(ERR_GUILD.ERR_GUILD_ALREADYIN_MSG, false, true);
				    return	-1;
			    }
		    }
		    else
		    {
			    if(szGuildName.Length == 0)
			    {
                    ShowMsg(ERR_GUILD.ERR_GUILD_NAME_EMPTY, false, true);
			    }

                if (szGuildDesc.Length == 0)
			    {
                    ShowMsg(ERR_GUILD.ERR_GUILD_DESC_EMPTY, false, true);
			    }
		    }

		    return E_FALSE;
        }
// 
// 		//创建帮会确认
 		public void CreateGuildConfirm(int nConfirmId)
        {
         /*   if(1 == nConfirmId && m_MsgArray.Count == 2) //create
		    {
    			
			    if(CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money()<500000)
			    {
				    ShowMsg(ERR_GUILD.ERR_GUILD_MONEY_NOT_ENOUGH);
				    return ;
			    }
			    //发送创建帮会信息包
                CGGuildApply pk = new CGGuildApply();
			    pk.SetGuildNameSize((BYTE)m_MsgArray[0].size());
			    pk.SetGuildName((CHAR*)m_MsgArray[0].c_str());
			    pk.SetGuildDescSize((BYTE)m_MsgArray[1].size());
			    pk.SetGuildDesc((CHAR*)m_MsgArray[1].c_str());
    			
			    NetManager.GetNetManager().SendPacket(pk);
		    }
		    else if(nConfirmId == 2)	//destory
		    {
			    //发送帮会删除包
                CGGuild ck = new CGGuild();
			    ck.GetGuildPacket().m_uPacketType = GUILD_PACKET_TYPE.GUILD_PACKET_CG_DISMISS;

			    NetManager.GetNetManager().SendPacket(&ck);
		    }
		    else if(nConfirmId == 3) //quit
		    {
			    //发送退出帮会包
                CGGuild dk = new CGGuild();
    			
			    dk.GetGuildPacket().m_uPacketType = GUILD_PACKET_TYPE.GUILD_PACKET_CG_LEAVE;
			    GUILD_CGW_LEAVE pLeave = (GUILD_CGW_LEAVE)dk.GetGuildPacket().GetPacket(GUILD_PACKET_TYPE.GUILD_PACKET_CG_LEAVE);

			    if(pLeave)
			    {
                    NetManager.GetNetManager().SendPacket(&dk);
			    }
		    }

		    m_MsgArray.clear();*/
        }
// 
// 		//向World申请帮会详细信息
// 		public void AskGuildDetailInfo();
// 
// 		//向World申请帮会成员信息
// 		public void AskGuildMembersInfo();
// 
// 		//向World申请帮会职位信息
// 		public void AskGuildAppointPosInfo(int nMemberBak);
// 
// 		//获得现有帮会的总数
// 		public int	GetGuildNum();
// 
// 		//获得其他帮会的信息
// 		// 修改了返回值和参数，因此lua脚本中需要修改 [6/8/2010 Sun]
// 		public Lua_GuildInfo GetGuildInfo(int nIndex);
// 
// 		//加入帮会
// 		public void JoinGuild(int nIndex);
// 
// 		//退出帮会
// 		public void	QuitGuild();
// 
// 		//踢出帮会,拒绝申请
// 		public void KickGuild(int nIndex);
// 
// 		//接纳会员
// 		public void RecruitGuild(int nIndex);
// 
// 		//获得自己的帮众信息
// 		public int GetMembersNum(int nType);
// 
// 		//Lua显示里list控件的位置[4/16/2006]
// 		public int GetShowMembersIdx(int nShowIdx);
// 		public int GetShowTraineesIdx(int nShowIdx);
// 
// 		//获得自己的帮众详细信息
// 		public Lua_GuildMemberInfo	GetMembersInfo(int nIndex);
// 
// 		//获得自己的帮派信息
// 		public string GetMyGuildInfo(string lpOp, int nIndex /*= 0*/);
// 
// 		//获得自己帮派的详细信息
// 		public Lua_GuildDetailInfo GetMyGuildDetailInfo();
// 
// 		//获得自己的帮派权利
// 		public string GetMyGuildPower();
// 
//         //修改现有帮众职位
// 		public void	AdjustMemberAuth(int nIndex);
// 
// 		//帮会让位给别人
// 		public void ChangeGuildLeader();
// 
// 		//帮会删除
// 		public void	DestoryGuild();
// 
// 		//修改帮会信息
// 		public void FixGuildInfo();
// 
// 		//帮会禅让
// 		public void DemisGuild();
// 
// 		//准备帮会成员数据[4/16/2006]
// 		public void PrepareMembersInfomation();
// 
// 		// 显示帮会界面 [2/27/2010 Sun]
// 		public void ToggleGuildDetailInfo();
// 		//tolua_end
// 
// 		//显示提示信息
// 		// msgType		消息号，用来在字典里获取相应的文字
// 		// bTalk		需要显示在聊天窗口
// 		// bTip			需要显示在屏幕中间的提示
        public void ShowMsg(ERR_GUILD msgType, bool bTalk /*= FALSE*/, bool bTip/* = TRUE*/)
        {

        }
// 
// 		public struct Name_Idx
// 		{
// 			string	m_MemberName;		//对应DataPool中GuildMemberInfo_t结构里的m_szName
// 			int		m_MemberIdx;		//对应DataPool中GuildMemberInfo_t的索引值
// 			int		m_Position;			//在帮中的职位
// 
// 			Name_Idx()
// 			{
// 				m_MemberName.erase();
// 				m_MemberIdx = -1;
// 				m_Position = GUILD_POSITION.GUILD_POSITION_INVALID;
// 			}
// 
// 			void	CleanUp()
// 			{
// 				m_MemberName.erase();
// 				m_MemberIdx = -1;
// 				m_Position = GUILD_POSITION.GUILD_POSITION_INVALID;
// 			}
// 		};
// 
// 		public class ShowColor
// 		{
// 			public string	m_OnlineLeaderColor;		//在线领导的显示颜色
// 			public string	m_LeaveLeaderColor;			//离线领导的显示颜色
// 			public string	m_OnlineMemberColor;		//在线帮众显示颜色
// 			public string	m_LeaveMemberColor;			//离线帮众显示颜色
// 			public string	m_OnlineTraineeColor;		//在线申请人显示颜色
// 			public string	m_LeaveTraineeColor;		//离线申请人显示颜色
// 
// 			ShowColor()
// 			{
// 				m_OnlineLeaderColor = "#B";			//蓝色
// 				m_OnlineMemberColor = "#W";			//白色
// 				m_OnlineTraineeColor = "#W";
// 
// 				m_LeaveLeaderColor = "#c808080";	//灰色
// 				m_LeaveMemberColor = "#c808080";
// 				m_LeaveTraineeColor = "#c808080";
// 			}
// 		};
// 		//帮会成立时间转换
// 		void ConvertServerTime(int dTime, string  strTime);
// 
// 		//更新帮会相应的显示信息
// 		void PerpareMembersBeforeShow();
// 
// 		//选择显示颜色
// 		string GetShowColor_For_Lua(int idx);
// 
		List<string>		m_MsgArray = new List<string>();			//配合ShowMsg函数使用
		int						m_iMemberBak;		//改变职位时的备份是要修改哪个玩家的职位信息

		//成员列表
		List<string>						m_AllMembers = new List<string>();			//所有成员
		List<string>						m_AllTrainees = new List<string>();			//所有预备成员
//		ShowColor								m_LuaShowColors = new ShowColor();		//Lua里显示的颜色
// 		public Guild();
// 		public ~Guild();
// 
 		public static Guild s_Guild;
 	}

