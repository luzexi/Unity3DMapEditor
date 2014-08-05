using System;
using System.Collections.Generic;
using Network.Packets;
using System.Text;

public class TALK_ACT_STRUCT
{
    public  List<string> m_actMsg;			//聊天显示信息
    public string m_actOrder;				//人物动作命令串
    public int m_actIdx;				    //动作索引，为界面的actbutton显示用
    public string m_iconName;				//图标
    public string m_tip;					//提示信息
};

namespace Interface
{
    public struct TALK_DEFINE
    {
        //当前所在处的频道
	    public const string		NAMETYPE_TALK_NEAR		= "near";
	    public const string		NAMETYPE_TALK_WORLD		= "world";
	    public const string		NAMETYPE_TALK_SYSTEM	= "system";
	    public const string		NAMETYPE_TALK_TEAM		= "team";
	    public const string		NAMETYPE_TALK_GUILD		= "guild";
	    public  const string		NAMETYPE_TALK_USER		= "user";
	    public  const string		NAMETYPE_TALK_PRIVATE	= "private";
	    public const string	    NAMETYPE_TALK_SELF		= "self";
	    public const string		NAMETYPE_TALK_CAMP		= "camp";
	    public const string		NAMETYPE_TALK_MAP		= "map";
	    public  const string		NAMETYPE_TALK_LIE		= "lie";

	    //保存密语对象的最大数量
	    public const int		TALK_TO_USER_MAXNUM				= 3;

	    //自建频道可以添加的玩家最大数量
	    public const int		CHANNEL_INVITE_USER_MAXNUM		=11;

	    //说话的时间差（单位：秒）
	    public const int		CHAT_TALK_SLICE					= 5;

	    //10级以下人物对话时的说话间隔（单位：秒）
	    public const int		CHAT_TALK_LV10_SLICE			= 10;

	    public const int		CHAT_PINGBI_MAXUSER_NUMBERS		= 20;
	    //GM命令的前导字符,长度必须是2个的字符，因为发送函数里只检测头2两个字符
	    public const string	GM_TALK_HEADER			= "!!";

	    //聊天动作的前导字符
	    public const string		CHAT_ACT_HEADER			="*";
    }
    //发送状态
    public enum CHAT_SEND_STATE
    {
        SEND_CHAT_OK = 1,
        SEND_FAILED_TIME_SLICE = -1,
        SEND_FAILED_LV10 = -2,
        SEND_FAILED_TEMPLATE = -3,
        SEND_FAILED_NEED = -4,
        SEND_FAILED_NOREASON = -5,
        SEND_FAILED_PINGBI = -6,
    };
    //字符串分析相关
    public enum TALKANALYZE_STR_ENUM
    {
	    ORGINAL_STR,
	    SEND_STR,
	    TALK_STR,
	    PAOPAO_STR,
	    HISTORY_STR,

	    PRIVATE_STR,
    };
    public enum TALKANALYZE_STRTYPE_ENUM
    {
	    STRINGTYPE_INVALID = -1,

	    STRINGTYPE_NORMAL = 0,			//普通消息
	    STRINGTYPE_GM,					//GM命令
	    STRINGTYPE_TXTACT,				//文字表情
	    STRINGTYPE_ACT,					//人物休闲动作
    // ----------------------------------------------------------------- [4/3/2006] @*特殊命令请往下添加
	    STRINGTYPE_FLAGADD,				//地图指示点填加	[4/13/2006]废弃
	    STRINGTYPE_FLASHADD,			//地图闪烁点填加	[4/13/2006]废弃
	    STRINGTYPE_FLAGDEL,				//地图指示点删除	[4/13/2006]废弃
	    STRINGTYPE_FLASHDEL,			//地图闪烁点删除	[4/13/2006]废弃

	    STRINGTYPE_FLAG_NPC_ADD,		//用NPC名字来添加指示点
	    STRINGTYPE_FLAG_POS_ADD,		//按x,z来添加指示点
	    STRINGTYPE_FLASH_NPC_ADD,		//用NPC名字来添加黄色指示点
	    STRINGTYPE_FLASH_POS_ADD,		//用x,z来添加黄色指示点

	    STRINGTYPE_MONSTER_PAOPAO,		//怪物泡泡
	    STRINGTYPE_SERVERMSG_TALK,		//Server端发送过来的提示信息消息
    // ----------------------------------------------------------------- [4/3/2006] @*特殊命令请往上添加
	    STRINGTYPE_NOUSER,				//私聊目标不在线，服务器传回来的提示信息
	    STRINGTYPE_DEBUG,				// debug版调试用 [8/13/2010 Sun]
	    STRINGTYPE_HYBRID,             //hybrid版调试用[10/13/2011 Sun]

	    STRINGTYPE_NUMBERS,
    };

    //字符串分析
	public class TalkAnalyze
	{
        public TalkAnalyze(ENUM_CHAT_TYPE ty, string str)
        {
            m_OrInputStr = str;
            m_OrChannelTye = ty;

            m_ChannelType = ENUM_CHAT_TYPE.CHAT_TYPE_INVALID;
            m_Ok = false;
            m_StrType = TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_INVALID;

            m_HeaderStr = null;
        }
		
		public virtual void	doAnalyze(string strTalker)	//分析
        {
            m_PrevResult = m_OrInputStr;

            JudgeChannelType();
            MoveHeader();
            JudgeStringType();
            RulerCheck();
            GenAllStr();

            m_PrevResult = null;
        }
        public ENUM_CHAT_TYPE ChannelType
        {
            get{return m_ChannelType;}
        }
        public ENUM_CHAT_TYPE OrChannelType
        {
            get{return m_OrChannelTye;}
        }
        public TALKANALYZE_STRTYPE_ENUM StrType
        {
            get{return m_StrType;}
        }
        public string ErrorStr
        {
            get{return m_ErrStr;}
        }

		public string	getStr(TALKANALYZE_STR_ENUM type)
        {
            string strRet = "";
		    if(!m_Ok) return strRet;

		    switch(type) 
		    {
		    case TALKANALYZE_STR_ENUM.ORGINAL_STR:
			    strRet = m_OrInputStr;
			    break;
		    case TALKANALYZE_STR_ENUM.SEND_STR:
			    strRet = m_SendStr;
			    break;
		    case TALKANALYZE_STR_ENUM.TALK_STR:
			    strRet = m_TalkStr;
			    break;
		    case TALKANALYZE_STR_ENUM.PAOPAO_STR:
			    strRet = m_PaoPaoStr;
			    break;
		    case TALKANALYZE_STR_ENUM.HISTORY_STR:
			    strRet = m_HistoryStr;
			    break;
		    case TALKANALYZE_STR_ENUM.PRIVATE_STR:
			    strRet = m_TargetStr;
			    break;
		    default:
			    break;
		    }

		    return strRet;
        }
		public virtual bool	IsOk()
        {
            m_Ok = true;

		    switch(m_StrType)
		    {
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL:			//普通消息
			    {
				    if(string.IsNullOrEmpty(m_SendStr) && string.IsNullOrEmpty(m_TalkStr)) 
                        m_Ok = false;
                    //check暂时不做
                    //if(!(CGameProcedure::s_pUISystem.CheckStringFilter(m_SendStr)))
                    //{
                    //    m_ErrStr = COLORMSGFUNC("InvalidSpecialString");
                    //    m_Ok = FALSE;
                    //}
			    }
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT:			//文字表情
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT:			//人物休闲动作
			    {
				    //if(m_ChannelType != CHAT_TYPE_NORMAL) m_Ok = FALSE; //现在有强制切换玩家频道的要求，暂不判断这个要求
				    if(string.IsNullOrEmpty(m_SendStr) && string.IsNullOrEmpty(m_PaoPaoStr)) 
                        m_Ok = false;
			    }
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NOUSER:			//私聊目标不在线，服务器传回来的提示信息
			    {
				    if(m_ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_SELF) m_Ok = false;
                    if(string.IsNullOrEmpty(m_TalkStr) && string.IsNullOrEmpty(m_HistoryStr)) 
                        m_Ok = false;
			    }
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_GM:				//GM命令
			    {
				    if(m_OrInputStr.Length < 2) m_Ok = false;
			    }
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_SERVERMSG_TALK:	//Server端发送过来的提示信息消息
			    {
				    if(m_ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_SELF && m_ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM) m_Ok = false;
				    if(m_OrInputStr.Length < 2) m_Ok = false;
			    }
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_MONSTER_PAOPAO:	//怪物泡泡
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAGADD:		//地图指示点填加
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASHADD:		//地图闪烁点填加
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAGDEL:		//地图指示点删除
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASHDEL:		//地图闪烁点删除

		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAG_NPC_ADD:	//用NPC名字来添加指示点
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAG_POS_ADD:	//按x,z来添加指示点
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASH_NPC_ADD:	//用NPC名字来添加黄色指示点
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASH_POS_ADD:	//用x,z来添加黄色指示点
			    {
				    if(m_ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_SELF) m_Ok = false;
				    if(m_OrInputStr.Length < 2) m_Ok = false;
			    }
			    break;
		    default:
			    m_Ok = false;
			    break;
		    }

		    if(m_ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_INVALID) m_Ok = false;
		    return m_Ok;
        }
        //根据字符串获得频道
		void			JudgeChannelType()
        {
            if(string.IsNullOrEmpty(m_PrevResult))
                return;

		    m_ChannelType = Talk.GetChatTypeFromMessage(m_PrevResult);
		    if(ENUM_CHAT_TYPE.CHAT_TYPE_INVALID == m_ChannelType)
		    {
			    m_ChannelType = m_OrChannelTye;
		    }

            if (m_ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_TELL)
            {
                Talk.Instance.UserCreate();
            }
        }
		void    MoveHeader()
        {
            if(string.IsNullOrEmpty(m_PrevResult)) 
                return;

		    int sz = m_PrevResult.Length;
		    char cH = '\0';

		    if(sz>=1) 
                cH = m_PrevResult[0];
		    if(cH == '/')
		    {
		
                int pos = m_PrevResult.IndexOf(" ");

			    if(pos != -1 && sz >= 2)
			    {
                    if (m_ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_TELL)
                    {
                        m_TargetStr = m_PrevResult.Substring(1, pos - 1);
                        if (m_TargetStr.Length > GAMEDEFINE.MAX_CHARACTER_NAME) m_TargetStr = "";
                    }

				    m_HeaderStr = m_PrevResult.Substring(0, pos+1);
				    m_PrevResult = m_PrevResult.Substring(pos+1);
				    if(string.IsNullOrEmpty(m_PrevResult))
				    {
					    m_HeaderStr = string.Empty;
                        m_PrevResult= string.Empty;
				    }
			    }
			    else
			    {
		            m_PrevResult = string.Empty;
                    m_HeaderStr  = string.Empty;
			    }
		    }
        }
		protected void    JudgeStringType()
        {
            if(string.IsNullOrEmpty(m_PrevResult)) return;
		
		    int sz = m_PrevResult.Length;
		    char[] cH = new char[2];

		    if(sz>=1) 
                cH[0] = m_PrevResult[0];
		    if(sz>=2) 
                cH[1] = m_PrevResult[1];

            //struct st{
            //    TALKANALYZE_STRTYPE_ENUM	ty;
            //    const CHAR*					pCmd;
            //    INT							num;
            //};

            ////@*特殊类型表
            //static st typeTable[] = {
            //    { STRINGTYPE_FLAGADD,			"flagadd",		6 },
            //    { STRINGTYPE_FLASHADD,			"flashadd",		6 },
            //    { STRINGTYPE_FLAGDEL,			"flagdel",		6 },
            //    { STRINGTYPE_FLASHDEL,			"flashdel",		6 },

            //    { STRINGTYPE_FLAG_NPC_ADD,		"flagNPC",		6 },
            //    { STRINGTYPE_FLAG_POS_ADD,		"flagPOS",		6 },
            //    { STRINGTYPE_FLASH_NPC_ADD,		"flashNPC",		6 },
            //    { STRINGTYPE_FLASH_POS_ADD,		"flashPOS",		6 },
            //    { STRINGTYPE_MONSTER_PAOPAO,	"npcpaopao",	4 },
            //    { STRINGTYPE_SERVERMSG_TALK,	"SrvMsg",		3 },	//个数不定，最少3个
            //};
    		
		    //@@	目标不在线
		    //@*	其他控制命令
		    //@		文字表情
		    //*		人物动作
		    if(cH[0] == '@')
		    {
                //if(cH[1] == '@') 
                //{
                //    m_StrType = STRINGTYPE_NOUSER;
                //}
                //else if(cH[1] == '*')
                //{
                //    std::vector<std::string> vCmd;
                //    INT nNumber = DBC::DBCFile::_ConvertStringToVector(m_PrevResult.c_str(),vCmd,";",TRUE,TRUE);

                //    for(INT i = 0; i < sizeof(typeTable)/sizeof(st); ++i)
                //    {
                //        if(nNumber >= typeTable[i].num && 0 == strcmp(vCmd[1].c_str(), typeTable[i].pCmd))
                //            m_StrType = typeTable[i].ty;
                //    }
                //}
                //else
                //{
                //    m_StrType = STRINGTYPE_TXTACT;
                //}
		    }
		    else if(cH[0] == '*')
		    {
			    m_StrType = TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT;
		    }
		    else if(cH[0] == '!' && cH[1] == '!')
		    {
			    m_StrType = TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_GM;
		    }
		    else
		    {
			    m_StrType = TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL;
		    }
        }
		protected void	GenAllStr()
        {
            if(string.IsNullOrEmpty(m_PrevResult))
                return;

		    switch((TALKANALYZE_STRTYPE_ENUM)m_StrType)
		    {
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL:			//普通消息
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT:			//文字表情
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT:			//人物休闲动作
			    GenSendStr();
			    GenTalkStr();
			    GenPaoPaoStr();
			    GenHistoryStr();
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NOUSER:			//私聊目标不在线，服务器传回来的提示信息
			    GenTalkStr();
			    GenHistoryStr();
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_MONSTER_PAOPAO:	//怪物泡泡
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_SERVERMSG_TALK:	//Server端发送过来的提示信息消息
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_GM:				//GM命令
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAGADD:		//地图指示点填加
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASHADD:		//地图闪烁点填加
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAGDEL:		//地图指示点删除
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASHDEL:		//地图闪烁点删除
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAG_NPC_ADD:	//用NPC名字来添加指示点
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAG_POS_ADD:	//按x,z来添加指示点
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASH_NPC_ADD:	//用NPC名字来添加黄色指示点
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASH_POS_ADD:	//用x,z来添加黄色指示点
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_DEBUG:			// 调试用命令 [8/13/2010 Sun]
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_HYBRID:		
			    {
				    GenSendStr();
			    }
			    break;
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_INVALID:
		    default:
			    break;
		    }
        }

		public virtual void	    GenSendStr(){}
		public virtual	void	GenTalkStr(){}
		public virtual	void	GenPaoPaoStr(){}
		public virtual void	    GenHistoryStr(){}

		void	RulerCheck()
        {

        }
		protected string	RemoveInvalidControlChar(string strIn)
        {
            return strIn;
        }
		public string			RemoveSpaceChar(string strIn)
        {
            string result = strIn.Replace(" ", "");
            return result.Replace("　", "");
        }

		public ENUM_CHAT_TYPE m_ChannelType;		//分析后的实际频道类型
		protected string	m_SendStr;		//网络传输用的字串
		protected string	m_TalkStr;		//界面上聊天窗口显示用的字串
		protected string	m_PaoPaoStr;	//泡泡上显示用的字串
		protected string	m_HistoryStr;	//历史记录应使用的字串
		protected string	m_TargetStr;	//私聊对象的名称
		protected string  m_HeaderStr;	//被剪切掉的文字头，用来构造发送历史字串使用

		protected string	m_OrInputStr;	//玩家输入的原始完整的的消息字串
		public ENUM_CHAT_TYPE	m_OrChannelTye;	//默认的当前频道类型

		public bool	m_Ok;			//是否分析成功
        public TALKANALYZE_STRTYPE_ENUM m_StrType;		//字符串的类型
		public string	m_ErrStr;		//获得分析结果

		protected string	m_PrevResult;
	};
    class SendTalkAnalyze : TalkAnalyze
	{
        public SendTalkAnalyze(ENUM_CHAT_TYPE ty, string str) : base(ty, str) { }
        public override bool IsOk()
        {
            base.IsOk();
            if (m_Ok)
            {
			    //文字表情、人物动作必须在“当前”频道中使用
			    if(StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT || StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT)
			    {
				    if(ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL)
				    {
					    if(ChannelType == OrChannelType)
					    {
  
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_CHAT_CHANNEL_CHANGED, "force_near");

						    m_ChannelType = ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL;
					    }
					    else
					    {
						    m_ErrStr = "#R不能在此频道中使用";//COLORMSGFUNC("GMGameInterface_Script_Talk_Info_CantUseInThisChannel");
						    m_Ok = false;
					    }
				    }
			    }

			    //密聊却没有玩家名称
                if (StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL && string.IsNullOrEmpty(getStr(TALKANALYZE_STR_ENUM.PRIVATE_STR)))
			    {
				    if(ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_TELL)
				    {

						GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_CHAT_CHANNEL_CHANGED);
					    m_ChannelType = ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL;
				    }
			    }

			    //密聊玩家的名称里含有非法字符
                if (StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL && !string.IsNullOrEmpty(getStr(TALKANALYZE_STR_ENUM.PRIVATE_STR)))
			    {
				    if(ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_TELL)
				    {
                        //if(!TDU_CheckStringValid(getStr(PRIVATE_STR).c_str()))
                        //{
                        //    m_ErrStr = "#R密聊对象名非法";//COLORMSGFUNC("GMGameInterface_Script_Talk_Info_InvalidTargetUser");
                        //    m_Ok = FALSE;
                        //}

                        //if(0 == strcmp(getStr(PRIVATE_STR).c_str(),CObjectManager::GetMe().GetMySelf().GetCharacterData().Get_Name()))
                        //{
                        //    m_ErrStr = "不能跟自己私聊";//COLORMSGFUNC("GMGameInterface_Script_Talk_Info_InvalidCantMySelf");
                        //    m_Ok = FALSE;
                        //}

    // 					if(!(CGameProcedure::s_pUISystem.CheckStringFilter(getStr(PRIVATE_STR))))
    // 					{
    // 						m_ErrStr = COLORMSGFUNC("InvalidSpecialString");
    // 						m_Ok = FALSE;
    // 					}
				    }
			    }


			    //只有颜色控制符的字串不发送
                if (m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL || m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT || m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT)
			    {
                    //STRING strNoColor;
                    //if(CGameProcedure::s_pUISystem)		CGameProcedure::s_pUISystem.ParserString_NoColor(m_SendStr, strNoColor, TRUE);
                    //if(strNoColor.empty())
                    //{
                    //    m_ErrStr = "#R没有可以发送的内容";//COLORMSGFUNC("GMGameInterface_Script_Talk_Info_InvalidTalkContent");
                    //    m_Ok = FALSE;
                    //}
			    }

			    //只有空格的字串不发送
                if (m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL || m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT || m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT)
			    {
				    string strNoColor = RemoveSpaceChar(m_SendStr);
    				
				    if(string.IsNullOrEmpty(strNoColor))
				    {
					    m_ErrStr = ("ERRORSpecialString");
					    m_Ok = false;
				    }
			    }
            }
            return m_Ok;
        }

        public override void GenSendStr()
        {
            if (m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL)
            {
                m_SendStr = m_PrevResult;
            }
            m_SendStr = RemoveInvalidControlChar(m_SendStr);
        }
        public override void GenHistoryStr()
        {
            if(m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL || 
                m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT ||
                m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT)
            {
                m_HistoryStr = m_HeaderStr + m_PrevResult;
            }
        }
	};

    class RecvTalkAnalyze : TalkAnalyze
    {

        public RecvTalkAnalyze(ENUM_CHAT_TYPE ty, string str) : base(ty, str) { }

        public override void doAnalyze(string strTalker)	//分析
        {
            m_PrevResult = m_OrInputStr;
            m_ChannelType = m_OrChannelTye;

            m_Talker = strTalker;
            JudgeStringType();
            GenAllStr();

            m_PrevResult = null;
        }
		public string			getTalker(){return m_Talker;}

        public override void GenTalkStr()
        {
            //普通类型字串
            if (m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL)
            {
                m_TalkStr = m_PrevResult;
            }
        }
        public override void GenPaoPaoStr()
        {
            if (ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL == m_ChannelType || ENUM_CHAT_TYPE.CHAT_TYPE_TEAM == m_ChannelType)
            {
                m_PaoPaoStr = (ENUM_CHAT_TYPE.CHAT_TYPE_TEAM == m_ChannelType) ? "#cCC99FF[队伍]#W" : "";
                //普通类型字串
                if (m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL)
                {
                    m_PaoPaoStr += m_PrevResult;
                }

                //文字表情
                //if (m_StrType == STRINGTYPE_TXTACT)
                //{
                //    m_PaoPaoStr = Talk::s_Talk.GetTalkTemplateString(m_PrevResult, m_Talker);
                //}

                //休闲动作
                //if (m_StrType == STRINGTYPE_ACT)
                //{
                //    m_PaoPaoStr = Talk::s_Talk.GetTalkActString(m_PrevResult, m_Talker);
                //}
            }
        }
        public override void GenHistoryStr()
        {
            //普通类型字串&文字表情&休闲动作
            if (m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_NORMAL 
                || m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_TXTACT 
                || m_StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT)
            {
                m_HistoryStr = m_PrevResult;
            }
        }

		string			m_Talker;	//说话的人
    };

    //聊天接口
    public class Talk
    {
        //字符串头标识检测
        public static string   MsgType_Near
        {
            get{ return "/n ";}
        }
        public static string MsgType_World
        {
            get{ return "/w ";}
        }
        public static string MsgType_Map
        {
            get{ return "/m ";}
        }
        public static string MsgType_Team
        {
            get{ return "/t ";}
        }
        public static string MsgType_Guild
        {
            get{ return "/g ";}
        }
        public static string MsgType_Camp
        {
            get{ return "/c ";}
        }
        public static string MsgType_Lie
        {
            get{ return "/l ";}
        }
        public Talk()
        {
            Channel newChannel = new Channel();

            newChannel.ChannelType = (ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL);
            m_listChannel.Add(newChannel);
    		
		    newChannel = new Channel();
            newChannel.ChannelType = (ENUM_CHAT_TYPE.CHAT_TYPE_WORLD);
		    m_listChannel.Add(newChannel);

		    newChannel = new Channel();
            newChannel.ChannelType = (ENUM_CHAT_TYPE.CHAT_TYPE_MAP);
            m_listChannel.Add(newChannel);

		    newChannel = new Channel();
            newChannel.ChannelType = (ENUM_CHAT_TYPE.CHAT_TYPE_CAMP);
            m_listChannel.Add(newChannel);

            m_UsrCh = null;
        }
        static readonly Talk sInstance = new Talk();
        public static Talk Instance
        {
            get
            {
                return sInstance;
            }
        }
        public class HistoryMsg
        {
            public HistoryMsg()
            {
                m_Name = "";
                m_ChannelType = ENUM_CHAT_TYPE.CHAT_TYPE_INVALID;
                m_Data = "";
                m_Time = 0;
                m_sendByMe = false;
            }

            public HistoryMsg(ENUM_CHAT_TYPE type)
            {
                m_Name = "";
                m_ChannelType = type;
                m_Data = "";
                m_Time = GameProcedure.s_pTimeSystem.GetTimeNow();
            }
            public string MsgName
            {
                get { return m_Name; }
                set { m_Name = value; }
            }
            public ENUM_CHAT_TYPE ChannelType
            {
                get { return m_ChannelType; }
                set { m_ChannelType = value; }
            }



            public void SetMsgData(string data, string file, int line)
            {
                m_Data = data;
            }
            public string MsgData
            {
                get { return m_Data; }
            }
            public uint MsgTime
            {
                get { return m_Time; }
                set { m_Time = value; }
            }
            public bool MsgSendByMe
            {
                get { return m_sendByMe; }
                set { m_sendByMe = value; }
            }


            public int SetByPacket(Network.Packets.GCChat packet)	// GCChat Packet	.	HistoryMsg Msg
            {
                if (null == packet)
                {
                    throw new NullReferenceException("GCChat is null");
                }

                //talker
                m_Name = EncodeUtility.Instance.GetUnicodeString(packet.SourName);

                //talk contex
                //m_Data = EncodeUtility.Instance.GetUnicodeString(packet.Contex);
                m_Data = Encoding.UTF8.GetString(packet.Contex);
                m_Data.Replace("\0", "");

                //channel type
                m_ChannelType = (ENUM_CHAT_TYPE)packet.ChatType;

                //saveTime
                m_Time = GameProcedure.s_pTimeSystem.GetTimeNow();

                return 0;
            }

            string m_Name;			//talker
            ENUM_CHAT_TYPE m_ChannelType;	//channel
            string m_Data;			//content

            uint m_Time;			//recvtime or sendtime
            bool m_sendByMe;		// 判断是否是自己发送的 [5/30/2011 ivan edit]
        };
        //历史记录队列
        protected Queue<HistoryMsg> m_SendHisQue = new Queue<HistoryMsg>();	//送出的聊天消息
        protected Queue<HistoryMsg> m_RecvHisQue = new Queue<HistoryMsg>();	//收到的聊天消息（包括客户端自己创建的消息）
        protected Queue<HistoryMsg> m_GMCmdHisQue= new Queue<HistoryMsg>();	//GM命令历史消息

        struct Talk_Need_Struct
        {
            CHAT_NEED_TYPE type;	//消耗类型
            int value;				//消耗数量

        };

        public class Channel
		{
		    public Channel()
			{
				m_channelType = ENUM_CHAT_TYPE.CHAT_TYPE_INVALID;
				m_channelName = "";
				m_channelHeader = "";
				m_TimeSlice = 0;
				m_close = false;
				m_need = new List<Talk_Need_Struct>();
				m_lastSendTime = 0;
			}
            public ENUM_CHAT_TYPE ChannelType
            {
                get { return m_channelType; }
                set { m_channelType = value; }
            }
            public string ChannelName
            {
                set { m_channelName = value; }
                get { return m_channelName; }
            }
            public string ChannelHeader
            {
                get { return m_channelHeader; }
            }
            public uint SliceTime
            {
                set { m_TimeSlice = value; }
                get { return m_TimeSlice; }
            }
            public bool Close
            {
                set { m_close = value; }
                get { return m_close; }
            }

            public bool TalkNeedCheck()	//TRUE 检查通过 FALSE 检查不通过
            {
                //todo:
                return true;
            }
            public bool TalkTimeCheck(uint dwTimeNow)	//TRUE 检查通过 FALSE 检查不通过
            {
                bool bOk = true;

		        if(0 == m_lastSendTime)
		        {
			        m_lastSendTime = dwTimeNow;
		        }
		        else
		        {
			        uint dwTime = GameProcedure.s_pTimeSystem.CalSubTime(m_lastSendTime,dwTimeNow);
			        if(dwTime/1000 > SliceTime)
			        {
				        m_lastSendTime = dwTimeNow;
			        }
			        else
			        {
				        bOk = false;
			        }
		        }

		        return bOk;
            }
            public void SetNeed(int tp, int value)
            {

            }

            public void LoadChannelConfig()
            {

            }
            public virtual void SetSendPacket(CGChat packet, TalkAnalyze pAna)	//设置本频道需要设置的相关内容
            {
                if (packet == null || pAna == null) return;
                packet.ChatType = (byte)pAna.ChannelType;
                packet.SetTalkContent(pAna.getStr(TALKANALYZE_STR_ENUM.SEND_STR));

            }

			protected ENUM_CHAT_TYPE		m_channelType;	//频道类型
			protected string				m_channelName;	//频道名称
			protected string				m_channelHeader;	//频道的前导字符
			protected uint					m_TimeSlice;	//频道的默认时间控制
			protected bool					m_close;		//频道是否关闭
			List<Talk_Need_Struct>			m_need;			//频道消耗
			uint					        m_lastSendTime;	//最后一次发送信息的时间
		};
        public class UserChannel :  Channel
		{
			public UserChannel()
			{
				m_channelType = ENUM_CHAT_TYPE.CHAT_TYPE_TELL;
				//m_channelName = "密聊频道";

				m_curSelPos = -1;
			}

            public int AddUser(string user)
            {
                if (user == null || user.Length == 0)
                {
                    return -1;
                }

                if (0 <= IsInQue(user))
                {
                    return 2;	//已经在密语对象队列中
                }

                m_UserQue.Add(user);

                if (m_UserQue.Count > TALK_DEFINE.TALK_TO_USER_MAXNUM)
                {
                    m_UserQue.RemoveAt(0);	//人数满，弹出旧对象
                }

                return 0;
            }
            public int IsInQue(string user)
            {
                for (int i = 0; i < m_UserQue.Count; ++i)
                {
                    if (user == m_UserQue[i]) return i;
                }
                return -1;
            }
            public void Clean()
            {
                m_UserQue.Clear();

                m_curSelPos = -1;
            }
            public string GetUserByIndex(uint idx)
            {
                string strR = "";
                if (idx >= m_UserQue.Count)
                {
                    return strR;
                }

                strR = m_UserQue[(int)idx];
                return strR;
            }
            public uint GetUserNum() { return (uint)m_UserQue.Count; }

            public string SelectFirstUser()
            {
                string strR = "";
                if (m_UserQue.Count == 0)
                {
                    return strR;
                }

                m_curSelPos = 0;
                strR = m_UserQue[0];
                return strR;
            }
            public string SelectLastUser()
            {
                string strR = "";
                if (m_UserQue.Count == 0)
                {
                    return strR;
                }

                m_curSelPos = (int)m_UserQue.Count - 1;
                strR = m_UserQue[m_curSelPos];
                return strR;
            }
            public string SelectNextUser(bool dir)
            {
                string strR = "";
                if (m_UserQue.Count == 0 || m_curSelPos < 0)
                {
                    return strR;
                }

                if (dir)
                {
                    m_curSelPos++;
                    if (m_curSelPos == (int)m_UserQue.Count) m_curSelPos = 0;
                }
                else
                {
                    m_curSelPos--;
                    if (0 > m_curSelPos) m_curSelPos = m_UserQue.Count - 1;
                }

                strR = m_UserQue[m_curSelPos];
                return strR;
            }
            public bool IsFirstUserSelect() { return (0 == m_curSelPos); }
            public bool IsLastUserSelect() { return (m_curSelPos == (int)m_UserQue.Count - 1); }
            public bool SetSelectByUser(string name)
            {
                if (m_UserQue.Count==0 || name.Length == 0)
                {
                    return false;
                }

                int i = 0;
                for (; i < m_UserQue.Count; ++i)
                {
                    if (name == GetUserByIndex((uint)i)) break;
                }

                if (i != m_UserQue.Count)
                {
                    m_curSelPos = i;
                    return true;
                }
                else
                    return false;    
            }
			public override void			SetSendPacket(CGChat packet, TalkAnalyze pAna)
            {
                if( packet == null || pAna == null ) return;
                if (pAna.ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_TELL) return;

		        base.SetSendPacket(packet, pAna);
		        //设置密聊对象
		        packet.SetTarget(pAna.getStr(TALKANALYZE_STR_ENUM.PRIVATE_STR));
		       
		        //填加私聊对象
                AddUser(pAna.getStr(TALKANALYZE_STR_ENUM.PRIVATE_STR));
            }

			protected List< string >	m_UserQue;

			protected int						m_curSelPos;	//当前选中位置
		};
        public class TeamChannel :  Channel
		{
			public TeamChannel()
			{
				m_channelType = ENUM_CHAT_TYPE.CHAT_TYPE_TEAM;
				//m_channelName = "组队频道";
			}
			
			~TeamChannel(){}

			public void					SetTeamID(short id){m_teamID = id;}
			public short				GetTeamID(){return m_teamID;}
            public override void SetSendPacket(CGChat packet, TalkAnalyze pAna)
            { 
            }
			short	m_teamID;
		};

        public class MenPaiChannel :  Channel
		{
			public MenPaiChannel()
			{
				m_channelType = ENUM_CHAT_TYPE.CHAT_TYPE_CAMP;
				//m_channelName = "门派频道";
			}
			~MenPaiChannel(){}

			public void					SetMenPaiID(short id){m_menpaiID = id;}
			public short				GetMenPaiID(){return m_menpaiID;}
			public override void			SetSendPacket(CGChat packet, TalkAnalyze pAna)
            {

            }
			protected short	m_menpaiID;
		};
        public class GuildChannel :  Channel
		{
			public GuildChannel()
			{
				m_channelType =  ENUM_CHAT_TYPE.CHAT_TYPE_GUILD;
				//m_channelName = "帮派频道";
			}

			~GuildChannel(){}

			public void					SetGuildID(short id){m_guildID = id;}
			public short				GetGuildID(){return m_guildID;}
			public override void			SetSendPacket(CGChat packet, TalkAnalyze pAna)
            {

            }
			protected short	m_guildID;
		};
		//密聊频道
        UserChannel m_UsrCh;
        //组队频道
		TeamChannel				m_TeamCh = new TeamChannel(); 
        Channel				    m_SysCh = new Channel();
		//客户端自用频道
		Channel				    m_SelfCh = new Channel();
        //门派频道
		MenPaiChannel			m_MenPaiCh = new MenPaiChannel();
        //帮派频道
		GuildChannel			m_GuildCh = new GuildChannel();
        //根据消息头获得频道类型
		public static ENUM_CHAT_TYPE GetChatTypeFromMessage(string strMsg)
        {
            ENUM_CHAT_TYPE retType = ENUM_CHAT_TYPE.CHAT_TYPE_INVALID;

		    if(strMsg.StartsWith(MsgType_Near))
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL;
		    else if(strMsg.StartsWith(MsgType_World))			//拆分场景和区域
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_WORLD;
		    else if(strMsg.StartsWith(MsgType_Map))			//区域聊天
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_MAP;
		    else if(strMsg.StartsWith(MsgType_Team))
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_TEAM;
		    else if(strMsg.StartsWith(MsgType_Guild))
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_GUILD;
		    else if(strMsg.StartsWith(MsgType_Camp))
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_CAMP;
		    else if(strMsg.StartsWith(MsgType_Lie))
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_LIE;
		    else if(strMsg.Length < MsgType_World.Length)		//不足MESSAGE_HEAD_LENGTH个字符的消息，当作普通消息
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_INVALID;
		    else if(strMsg.StartsWith("//"))
			    retType = ENUM_CHAT_TYPE.CHAT_TYPE_TELL;

		    return retType;
        }
        public  string SendChatMessage(string channel, string Msg)
        {
            if(GameProcedure.GetActiveProcedure() != GameProcedure.s_ProcMain)//当前是否处于游戏中
            {
                return "";
            }

            string strTalkCh = channel;
		    string strTalkMsg = Msg;
            //公用的聊天信息发送速度控制变量
		    int bSend = (int)CHAT_SEND_STATE.SEND_FAILED_NOREASON;	//是否发送

		    //输入字符串分析
            SendTalkAnalyze saN = new SendTalkAnalyze(String2ChannelType(strTalkCh), strTalkMsg);
		    saN.doAnalyze( CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Name() );
		    if(!saN.IsOk())
		    {
//                 if(!(saN.getErrStr().empty()))
// 			    {
// 				    ADDTALKMSG(saN.getErrStr());
// 			    }
			    return "";
		    }

		    //GM命令解析
		    if(saN.StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_GM)
		    {
			    SendGMCommand(saN.getStr(TALKANALYZE_STR_ENUM.ORGINAL_STR));
			    return "";
		    }

            //查找频道是否存在
		    Talk.Channel pChannel = null;
		    foreach( Talk.Channel cha in m_listChannel )
		    {
			    if( cha.ChannelType == saN.ChannelType)
			    {
				    pChannel = cha;
				    break;
			    }
		    }

		    if(null == pChannel) 
			    return "";
            //根据类型构造不同的消息发送

		    CGChat   msg = new CGChat();
		    pChannel.SetSendPacket(msg, saN);

            NetManager.GetNetManager().SendPacket(msg);
			return "";
        }
        void SendGMCommand(string strCmd)
        {

        }
        ENUM_CHAT_TYPE String2ChannelType(string strType)
	    {
            ENUM_CHAT_TYPE type = ENUM_CHAT_TYPE.CHAT_TYPE_INVALID;

            if (strType == TALK_DEFINE.NAMETYPE_TALK_NEAR)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_WORLD)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_WORLD;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_MAP)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_MAP;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_LIE)		// 谣言聊天 [5/11/2011 ivan edit]
                type = ENUM_CHAT_TYPE.CHAT_TYPE_LIE;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_TEAM)
			    type = ENUM_CHAT_TYPE.CHAT_TYPE_TEAM;					//??? TEAM, GUILD
		    else if(strType ==TALK_DEFINE.NAMETYPE_TALK_GUILD)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_GUILD;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_USER)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_TELL;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_PRIVATE)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_TELL;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_CAMP)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_CAMP;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_SELF)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_SELF;
            else if (strType == TALK_DEFINE.NAMETYPE_TALK_SYSTEM)
                type = ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM;

		    return type;
	    }
        string ChannelType2String(ENUM_CHAT_TYPE type)
        {
            string strType;
            switch (type)
            {
                case ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL:
                    strType = TALK_DEFINE.NAMETYPE_TALK_NEAR;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_WORLD:
                    strType = TALK_DEFINE.NAMETYPE_TALK_WORLD;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_MAP:// 增加区域聊天，即地图 [5/10/2011 ivan edit]
                    strType = TALK_DEFINE.NAMETYPE_TALK_MAP;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CHANNEL:
                    strType = TALK_DEFINE.NAMETYPE_TALK_PRIVATE;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM:
                    strType = TALK_DEFINE.NAMETYPE_TALK_SYSTEM;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TELL:
                    strType = TALK_DEFINE.NAMETYPE_TALK_PRIVATE;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_GUILD:
                    strType = TALK_DEFINE.NAMETYPE_TALK_GUILD;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_SELF:
                    strType = TALK_DEFINE.NAMETYPE_TALK_SELF;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TEAM:
                    strType = TALK_DEFINE.NAMETYPE_TALK_TEAM;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CAMP:
                    strType = TALK_DEFINE.NAMETYPE_TALK_CAMP;
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_LIE:// 增加谣言聊天 [5/11/2011 ivan edit]
                    strType = TALK_DEFINE.NAMETYPE_TALK_LIE;
                    break;
                default:
                    strType = "";
                    break;
            }

            return strType;
        }
        //私聊频道相关接口
        public void UserCreate()
        {
            if(m_UsrCh == null)
            {
                m_UsrCh = new UserChannel();
                m_UsrCh.LoadChannelConfig();
                m_listChannel.Add(m_UsrCh);
            }
        }
        //所有频道(给界面选择默认频道显示使用)
        List<Talk.Channel> m_listChannel = new List<Channel>();

        //保存聊天信息的最大值
        int m_MaxSaveNum = 100;

        //上次处理历史发送信息的位置
        int m_PrevSendPos;

        //上次处理GM命令发送信息的位置
        int m_PrevGMCmdPos;

        //上次处理菜单的link内容
        string m_PrevMenuLink;

        //泡泡文字信息是否限制
        bool m_bPaoPaoRule;

        List<string> m_PingBiQue = new List<string>();

        public int GetPingBiNum()
	    {
		    return m_PingBiQue.Count;
	    }
        public string GetPingBiName(int nIndex)
	    {

		    if(nIndex >= m_PingBiQue.Count|| nIndex < 0) 
			    return null;

		    return m_PingBiQue[nIndex];

	    }
        public void DelPingBi(int nIndex)
	    {

		    if(nIndex >= m_PingBiQue.Count || nIndex < 0)
			    return ;

		    DelPingBi(m_PingBiQue[nIndex]);

		    if(CEventSystem.Instance!=null)
		    {
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CHAT_PINGBI_UPDATE);
		    }
	    }
        public void AddPingBi(string strUser)
	    {
		    if(!IsInPingBi(strUser))
		    {
			    m_PingBiQue.Add(strUser);
			    if(m_PingBiQue.Count > TALK_DEFINE.CHAT_PINGBI_MAXUSER_NUMBERS) m_PingBiQue.RemoveAt(0);

			    if(CEventSystem.Instance != null)
			    {
				    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CHAT_PINGBI_UPDATE);
			    }
		    }
	    }

	    public void DelPingBi(string strUser)
	    {
		    if(m_PingBiQue.Count == 0) return;

		    for (int i = 0; i < m_PingBiQue.Count; i++)
		    {
                if(strUser == m_PingBiQue[i])
                {
                    m_PingBiQue.RemoveAt(i);
                    break;
                }
		    }
	    }

	    public bool IsInPingBi(string strUser)
	    {
		    if(m_PingBiQue.Count == 0) return false;

		    foreach(string name in m_PingBiQue)
            {
                if(name == strUser)
                    return true;
            }

		    return false;
	    }

	    public void ClearPingBi()
	    {
            m_PingBiQue.Clear();
	    }

        //网络处理相关接口
	
		//聊天信息包 G->C
        public int HandleRecvTalkPacket(GCChat pPacket)
        {
            
		    if( null == pPacket)
		    {
			    return -1;
		    }

            string sourName = EncodeUtility.Instance.GetUnicodeString(pPacket.SourName);
		    // 如果发言人在黑名单里，则不显示
            if (CDataPool.Instance.GetRelation().IsBlackName(sourName))
		    {
			    return 0;
		    }

		    // 如果发言人在屏蔽列表里，不显示
            if (IsInPingBi(sourName))
            {
                return 0;
            }

		    HistoryMsg msg = new HistoryMsg();
		    if( 0 == msg.SetByPacket(pPacket))
		    {

			    string strType = ChannelType2String(msg.ChannelType);
    			
			    RecvTalkAnalyze reN = new RecvTalkAnalyze(msg.ChannelType, msg.MsgData);
			    reN.doAnalyze(msg.MsgName);

			    if(!reN.IsOk())
			    {
                    //if(!(reN.getErrStr().empty())) 
                    //{
                    //    ADDTALKMSG(reN.getErrStr());
                    //}
				    return 0;
			    }

			    if(!HandleSpecialSelfString(reN))
			    {
				    // 小白说只有map和camp频道需要过滤阵营 [5/11/2011 ivan edit]
				    if (reN.ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_MAP ||
					    reN.ChannelType == ENUM_CHAT_TYPE.CHAT_TYPE_CAMP)
				    {

					    short sourceCamp = pPacket.CampID;
					    short myCamp = CObjectManager.Instance.getPlayerMySelf().GetCampData().m_nCampID;
					    // 由于策划要求把红名和非红名的看做一个势力，所以硬编码了阵营编号 [9/26/2011 Ivan edit]
					    switch(sourceCamp)
					    {
					    case 0:// 天元 [9/26/2011 Ivan edit]
					    case 3:// 红名天元 [9/26/2011 Ivan edit]
						    if (myCamp != 0 && myCamp != 3)
							    return 0;
						    break;
					    case 1:
					    case 4:
						    if (myCamp != 1 && myCamp != 4)
							    return 0;
						    break;
					    case 2:
					    case 5:
						    if (myCamp != 2 && myCamp != 5)
							    return 0;
						    break;
					    }
				    }

				    List<string> strParam = new List<string>();
				    strParam.Add(ChannelType2String(reN.ChannelType));
				    strParam.Add(reN.getTalker());
				    strParam.Add(reN.getStr(TALKANALYZE_STR_ENUM.TALK_STR));
				    // 标注消息不是自己发出的 [5/30/2011 ivan edit]
				    strParam.Add("0");
				    strParam.Add(msg.MsgTime.ToString());
    				
				    if(CEventSystem.Instance != null)
				    {
					    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CHAT_MESSAGE, strParam);
  					
				    }

				    msg.ChannelType = reN.ChannelType;
				    msg.SetMsgData(reN.getStr(TALKANALYZE_STR_ENUM.HISTORY_STR), null, 0);
				    //保存历史消息
				    AddToRecvHistoryQue(msg);

				    //泡泡信息显示
                    //if(!(reN.getStr(PAOPAO_STR).empty()))
                    //{
                    //    INT extLength = 0;
                    //    STRING strContext = "#cCC99FF[队伍]#W";
                    //    switch(reN.getChannelType()) {
                    //    case CHAT_TYPE_TEAM:
                    //        extLength += (INT)(strContext.size());
                    //        break;
                    //    default:
                    //        break;
                    //    }
                    //    SetPaoPaoTxt(reN.getTalker(), reN.getStr(PAOPAO_STR), reN.getStrType(), extLength);
                    //}

				    //聊天动作命令还要进行做具体的动作
				    if(reN.StrType == TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_ACT)
				    {
					    string strBackUp = reN.getStr( TALKANALYZE_STR_ENUM.ORGINAL_STR);
                        string strAct = strBackUp.Substring(TALK_DEFINE.CHAT_ACT_HEADER.Length, strBackUp.IndexOf(" ") - TALK_DEFINE.CHAT_ACT_HEADER.Length);
					    DoAct(strAct, reN.getTalker());
				    }
			    }
		    }

		    return 0;
        }
        bool HandleSpecialSelfString(TalkAnalyze pAna)
	    {
		    if(pAna==null) return false;
		    if(pAna.ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_SELF && pAna.ChannelType != ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM) 
                return false;
    		
		    bool bHandle = true;

		    switch(pAna.StrType) 
		    {
		    case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAGADD:
			    //填加方向点
			    //HandleFlagAdd(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAGDEL:
			    //删除方向点
			    //HandleFlagDel(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASHADD:
			    //填加闪光点
			    //HandleFlashAdd(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASHDEL:
			    //删除闪光点
			    //HandleFlashDel(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAG_NPC_ADD:	
			    //用NPC名字来添加指示点
			    //HandleFlagNPCAdd(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLAG_POS_ADD:
			    //按x,z来添加指示点
			    //HandleFlagPOSAdd(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASH_NPC_ADD:	
			    //用NPC名字来添加黄色指示点
			    //HandleFlashNPCAdd(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_FLASH_POS_ADD:	
			    //用x,z来添加黄色指示点
			    //HandleFlashPOSAdd(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_MONSTER_PAOPAO:
			    //怪物泡泡
			    //HandleMonsterPaoPao(pAna->getStr(ORGINAL_STR).c_str());
			    break;
            case TALKANALYZE_STRTYPE_ENUM.STRINGTYPE_SERVERMSG_TALK:
			    //服务器端传送过来的提示信息
			    //HandleServerMsgTalk(pAna->getStr(ORGINAL_STR).c_str(),pAna->getChannelType());
			    break;
		    default:
			    bHandle = false;
			    break;
		    }

		    return bHandle;
	    }

        int	AddToRecvHistoryQue(HistoryMsg msg)
	    {
		    m_RecvHisQue.Enqueue(msg);

		    if(m_RecvHisQue.Count > m_MaxSaveNum)
		    {
			    m_RecvHisQue.Dequeue();
		    }

		    return 0;
	    }

        void DoAct(string act, string user)
	    {
		    if(!string.IsNullOrEmpty(act) && !string.IsNullOrEmpty(user))
		    {
			    CObject_Character pObj = CObjectManager.Instance.FindCharacterByName(user);
			    if(pObj != null)
			    {
                    string strOrder = GetTalkActOrder(act);
				    //分拆动作命令
				    if(strOrder != "besit")
				    {
					    pObj.SetChatMoodAction(strOrder);
				    }
				    else
				    {
					    //坐下需要单独处理
					    pObj.SitDown();
				    }
				    //播放动作命令
				    //pObj->PlayChatMoodAction();
			    }
		    }
	    }
        string GetTalkActOrder(string act)
        {
            //TODO:
            return "";
        }

        public string GetChannelHeader(string strType, string chatTalkerName)
	    {

            string strHeader = "";
		    ENUM_CHAT_TYPE type = String2ChannelType(strType);

            switch (type)
            {
                case ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL:
                    strHeader = "附近";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TEAM:
                    strHeader = "队伍";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_WORLD:
                    strHeader = "世界";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_TELL:
                    strHeader = "私聊";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM:
                    strHeader = "#-09";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CHANNEL:
                    strHeader = "自建";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_GUILD:
                    strHeader = "帮派";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_CAMP:
                    strHeader = "势力";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_SELF:
                    strHeader = "自用";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_LIE:
                    strHeader = "谣言";
                    break;
                case ENUM_CHAT_TYPE.CHAT_TYPE_MAP:
                    strHeader = "区域";
                    break;
            }
            //const tDataBase* pTalkConfig = CDataBaseSystem::GetMe()->GetDataBase(DBC_TALK_CONFIG);
            //if(pTalkConfig)
            //{
            //    const _DBC_TALK_CONFIG* pLine = (const _DBC_TALK_CONFIG*)pTalkConfig->Search_LineNum_EQU(type);
            //    if(pLine)
            //    {
            //        return pLine->szChannelHeader;
            //    }
            //}

		    return strHeader;
	    }
    }
}