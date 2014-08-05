using System;
using System.Collections.Generic;

public class CEventSystem
{
    public delegate void EventHandler(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam);

    Dictionary<GAME_EVENT_ID, EVENT_DEFINE> allEventHandlers = new Dictionary<GAME_EVENT_ID, EVENT_DEFINE>();

    List<EVENT> delayEvtList = new List<EVENT>();
    List<EVENT> quickEvtList = new List<EVENT>();

    const uint WORK_STEP = 2;
	uint    m_dwLastTickCount;


    static readonly CEventSystem instance = new CEventSystem();
    public static CEventSystem Instance
    {
        get { return instance; }
    }

    public CEventSystem()
    {
        //allEventHandlers = new Dictionary<GAME_EVENT_ID, EventHandler>();
        InitialAllEvent();
    }

    void InitialAllEvent()
    {
        foreach (EVENT_DEFINE evtDef in EventConfig.sAllEventDefines)
        {
            allEventHandlers[evtDef.idEvent] = evtDef;
        }
    }

    //处理快速队列和慢速
    public void Tick()
    {
        if (delayEvtList.Count != 0)
        {
            uint nTickCountNow = GameProcedure.s_pTimeSystem.GetTickCount();
		    uint nTickCountStep =m_dwLastTickCount - nTickCountNow;
		    if(nTickCountStep >= WORK_STEP) 
		    {
			    m_dwLastTickCount = nTickCountNow;

                try
                {
                    EVENT evt = delayEvtList[0];
                    
                    if (evt.eventDefine.FuncNotify == null)
                    {
                        LogManager.LogWarning("evt.eventDefine.FuncNotify evt.eventDefine.idEvent " + evt.eventDefine.idEvent);
                    }
                    else
                    {
                      
                        if (evt.eventDefine.FuncNotify != null)
                            evt.eventDefine.FuncNotify(evt.eventDefine.idEvent, evt.Params);
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(ex.ToString());
                }
                finally
                {
                    delayEvtList.RemoveAt(0);
                }
		    }
        }
        
        if (quickEvtList.Count != 0)
        {
            try
            {
                List<EVENT>.Enumerator enumerator = quickEvtList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    EVENT evt = enumerator.Current;

                    if(evt.eventDefine.FuncNotify != null)
                        evt.eventDefine.FuncNotify(evt.eventDefine.idEvent, evt.Params);

                    quickEvtList.Remove(evt);
                    enumerator = quickEvtList.GetEnumerator();
                }

            }
            catch (System.Exception ex)
            {
                LogManager.LogError(ex.ToString());
            }
            finally
            {
                quickEvtList.Clear();
            }
        }
    }

    //注册事件处理函数
    public void RegisterEventHandle(GAME_EVENT_ID eventId, EventHandler funHandle)
    {
        if (allEventHandlers.ContainsKey(eventId))
        {
            allEventHandlers[eventId].RegistEvent(funHandle);
        }
        else
        {
            //默认自动添加不在config里面的事件
            allEventHandlers[eventId] = new EVENT_DEFINE(eventId);
            allEventHandlers[eventId].RegistEvent(funHandle);
        }
    }
    public void UnRegistEventHandle(GAME_EVENT_ID eventId, EventHandler funHandle)
    {
        if (allEventHandlers.ContainsKey(eventId))
        {
            allEventHandlers[eventId].UnRegistEvent(funHandle);
        }
    }
    public void PushEvent(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (allEventHandlers.ContainsKey(gAME_EVENT_ID))
        {
            EVENT newEvent = new EVENT();
            newEvent.eventDefine = allEventHandlers[gAME_EVENT_ID];
            newEvent.Params = vParam;
            //如果不是延时事件，则添加到快速队列
            if (!allEventHandlers[gAME_EVENT_ID].delayProcess)
            {
                //allEventHandlers[gAME_EVENT_ID].QuickFuncNotify(gAME_EVENT_ID,vParam);
                quickEvtList.Add(newEvent);
            }
            else
            {
                delayEvtList.Add(newEvent);
            }
        }
    }
    public void PushEvent(GAME_EVENT_ID gAME_EVENT_ID, string sParam)
    {
        List<String> vParam = new List<String>();
        vParam.Add(sParam);
        PushEvent(gAME_EVENT_ID, vParam);
    }
    public void PushEvent(GAME_EVENT_ID gAME_EVENT_ID, int iParam)
    {
        List<String> vParam = new List<String>();
        vParam.Add(Convert.ToString(iParam));
        PushEvent(gAME_EVENT_ID, vParam);
    }

    public void PushEvent(GAME_EVENT_ID gAME_EVENT_ID, float param)
    {
        List<String> vParam = new List<String>();
        vParam.Add(Convert.ToString(param));
        PushEvent(gAME_EVENT_ID, vParam);
    }

    public void PushEvent(GAME_EVENT_ID gAME_EVENT_ID)
    {
        List<String> vParam = new List<String>();
        PushEvent(gAME_EVENT_ID, vParam);
    }
}

public class EVENT
{
    public EVENT_DEFINE eventDefine;
    List<String> lParams = new List<String>();
    public List<String> Params
    {
        get { return lParams; }
        set { lParams = value; }
    }
}

public class EVENT_DEFINE
{
    public GAME_EVENT_ID idEvent;
    public bool delayProcess;
    List<CEventSystem.EventHandler> funcNotify; //仅用来记录是否重复注册
    CEventSystem.EventHandler funcHandler;
    public CEventSystem.EventHandler FuncNotify
    {
        get { return funcHandler; }
    }
    public void RegistEvent(CEventSystem.EventHandler evtHandler)
    {
       
        if (funcNotify != null)
        {
            if (funcNotify.Contains(evtHandler))
                return;
            funcNotify.Add(evtHandler);
            funcHandler += evtHandler;
        }
        else
        {
            funcNotify = new List<CEventSystem.EventHandler>();
            funcNotify.Add(evtHandler);
            funcHandler += evtHandler;
        }
    }
    public void UnRegistEvent(CEventSystem.EventHandler evtHanlder)
    {
        if (funcNotify != null)
        {
            if (funcNotify.Contains(evtHanlder))
                funcNotify.Remove(evtHanlder);
            funcHandler -= evtHanlder;
        }
    }

    public EVENT_DEFINE(GAME_EVENT_ID id): this(id, false)
    {
    }
    public EVENT_DEFINE(GAME_EVENT_ID id, bool delay)
    {
        idEvent = id;
        delayProcess = delay;
    }
}

public class EventConfig
{

    public static EVENT_DEFINE[] sAllEventDefines =
	    {
            new EVENT_DEFINE( GAME_EVENT_ID.GE_APPLICATION_INITED ),
// 	        { GE_ON_SCENE_TRANS,			"ON_SCENE_TRANS",			},
// 	        { GE_ON_SERVER_TRANS,			"ON_SERVER_TRANS",			},
// 	        { GE_ON_SCENE_UPDATED,			"ON_SCENE_UPDATED",			},
// 	        { GE_SCENE_TRANSED,				"SCENE_TRANSED",			},
// 	        { GE_PLAYER_ENTER_WORLD,		"PLAYER_ENTERING_WORLD",	},
// 	        { GE_PLAYER_LEAVE_WORLD,		"PLAYER_LEAVE_WORLD",		},
// 	        { GE_TEAM_CHANG_WORLD,			"TEAM_CHANGE_WORLD",			},	// 队友改变场景了
// 	        { GE_SKILL_CHANGED,				"SKILL_CHANGED",			},
// 	        { GE_MAINTARGET_CHANGED,		"MAINTARGET_CHANGED",		},
// 	        { GE_MAINTARGET_OPEND,			"MAINTARGET_OPEN",			},		//打开主窗口
// 	        { GE_PACKAGE_ITEM_CHANGED,		"PACKAGE_ITEM_CHANGED",		},
// 	        { GE_TOGLE_LIFETALISMAN_BOOK,			"TOGLE_LIFETALISMAN_BOOK",			},
// 	        { GE_CLOSE_SKILL_BOOK,			"CLOSE_SKILL_BOOK",			},
// 	        { GE_SKILL_UPDATE,				"SKILL_UPDATE",				},
// 	        { GE_TOGLE_PET_PAGE,			"TOGLE_PET_PAGE",			},
// 	        { GE_TOGLE_RIDE_PAGE,			"OPEN_RIDE_PAGE",			}, ////ZZH+:骑乘
// 	        { GE_TOGLE_OTHER_INFO_PAGE,			"OPEN_PRIVATE_INFO",			}, ////ZZH+:私人信息
// 	        { GE_TOGLE_NATURALSKILL_PAGE,			"TOGLE_NATURALSKILL_PAGE",			},
// 	        { GE_TOGLE_COMMONSKILL_PAGE,	"TOGLE_COMMONSKILL_PAGE"	},
// 	        { GE_TOGLE_CONTAINER,			"TOGLE_CONTAINER",			},
// 	        {GE_TOGLE_MISSION_TRACE,		"TOGLE_MISSION_TRACE",		},
//         //---------------------------------------------------------------------------	
// 	        //------------------------------
// 	        //角色基本属性
// 	        { GE_UNIT_HP,					"UNIT_HP",					  FALSE},
// 	        { GE_UNIT_MP,					"UNIT_MP",					  FALSE},
            new EVENT_DEFINE( GAME_EVENT_ID.GE_UNIT_RAGE,true ),//延时广播
// 	        { GE_UNIT_XINFA,				"UNIT_XINFA",				  TRUE},
// 	        { GE_UNIT_EXP,					"UNIT_EXP",					  FALSE},
// 	        { GE_UNIT_MONEY,				"UNIT_MONEY",				  TRUE},
// 	        { GE_UNIT_RACE_ID,				"UNIT_RACE_ID",				  TRUE},
// 	        { GE_UNIT_NAME,					"UNIT_NAME",				  TRUE},
// 	        { GE_UNIT_CAMP_ID,				"UNIT_CAMP_ID",				  TRUE},
// 	        { GE_UNIT_LEVEL,				"UNIT_LEVEL",				  FALSE},
// 	        { GE_UNIT_MOVE_SPEED,			"UNIT_MOVE_SPEED",			  TRUE},
// 	        { GE_UNIT_FIGHT_STATE,			"UNIT_FIGHT_STATE",			  TRUE},
// 	        { GE_UNIT_MAX_EXP,				"UNIT_MAX_EXP",				  FALSE},
// 	        //------------------------------"
// 	        //一级战斗属性					"
// 	        { GE_UNIT_STR,					"UNIT_STR",					  TRUE},
// 	        { GE_UNIT_SPR,					"UNIT_SPR",					  TRUE},
// 	        { GE_UNIT_CON,					"UNIT_CON",					  TRUE},
// 	        { GE_UNIT_INT,					"UNIT_INT",					  TRUE},
// 	        { GE_UNIT_DEX,					"UNIT_DEX",					  TRUE},
// 	        { GE_UINT_WIND,					"UINT_WIND",				  TRUE},
// 	        { GE_UINT_THUNDER,				"UINT_THUNDER",				  TRUE},
// 	        { GE_UNIT_POINT_REMAIN,			"UNIT_POINT_REMAIN",		  TRUE},
// 	        //------------------------------"
// 	        //二级战斗属性					"
// 	        { GE_UNIT_ATT_PHYSICS,			"UNIT_ATT_PHYSICS",			  TRUE},
// 	        { GE_UNIT_ATT_MAGIC,			"UNIT_ATT_MAGIC",			  TRUE},
// 	        { GE_UNIT_DEF_PHYSICS,			"UNIT_DEF_PHYSICS",			  TRUE},
// 	        { GE_UNIT_DEF_MAGIC,			"UNIT_DEF_MAGIC",			  TRUE},
// 	        { GE_UNIT_MAX_HP,				"UNIT_MAX_HP",				  TRUE},
// 	        { GE_UNIT_MAX_MP,				"UNIT_MAX_MP",				  TRUE},
// 	        { GE_UNIT_HP_RE_SPEED,			"UNIT_HP_RE_SPEED",			  TRUE},
// 	        { GE_UNIT_MP_RE_SPEED,			"UNIT_MP_RE_SPEED",			  TRUE},
// 	        { GE_UNIT_HIT,					"UNIT_HIT",					  TRUE},
// 	        { GE_UNIT_MISS,					"UNIT_MISS",				  TRUE},
// 	        { GE_UNIT_CRIT_RATE,			"UNIT_CRIT_RATE",			  TRUE},
// 	        { GE_UNIT_DEF_CRIT_RATE,		"UNIT_DEF_CRIT_RATE",		  TRUE},
// 	        { GE_UNIT_ATT_SPEED,			"UNIT_ATT_SPEED",			  TRUE},
// 	        { GE_UNIT_ATT_COLD,				"UNIT_ATT_COLD",			  TRUE},
// 	        { GE_UNIT_DEF_COLD,				"UNIT_DEF_COLD",			  TRUE},
// 	        { GE_UNIT_ATT_FIRE,				"UNIT_ATT_FIRE",			  TRUE},
// 	        { GE_UNIT_DEF_FIRE,				"UNIT_DEF_FIRE",			  TRUE},
// 	        { GE_UNIT_ATT_LIGHT,			"UNIT_ATT_LIGHT",			  TRUE},
// 	        { GE_UNIT_DEF_LIGHT,			"UNIT_DEF_LIGHT",			  TRUE},
// 	        { GE_UNIT_ATT_POSION,			"UNIT_ATT_POSION",			  TRUE},
// 	        { GE_UNIT_DEF_POSION,			"UNIT_DEF_POSION",			  TRUE},
// 	        { GE_UNIT_ATT_METAL,			"UNIT_ATT_METAL",			  TRUE},// 为了显示方便，重新封装 [11/9/2011 Ivan edit]
// 	        { GE_UNIT_DEF_METAL,			"UNIT_DEF_METAL",			  TRUE},
// 	        { GE_UNIT_ATT_WOOD,			"UNIT_ATT_WOOD",			  TRUE},
// 	        { GE_UNIT_DEF_WOOD,			"UNIT_DEF_WOOD",			  TRUE},
// 	        { GE_UNIT_ATT_WATER,			"UNIT_ATT_WATER",			  TRUE},
// 	        { GE_UNIT_DEF_WATER,		"UNIT_DEF_WATER",			  TRUE},
// 	        { GE_UNIT_ATT_EARTH,			"UNIT_ATT_EARTH",			  TRUE},
// 	        { GE_UNIT_DEF_EARTH,			"UNIT_DEF_EARTH",			  TRUE},
// 	        { GE_UNIT_MENPAI,				"UNIT_MENPAI",				  TRUE},
// 	        { GE_UNIT_HAIR_MESH,			"UNIT_HAIR_MESH",			  TRUE},
// 	        { GE_UNIT_HAIR_COLOR,			"UNIT_HAIR_COLOR",			  TRUE},
// 	        { GE_UNIT_FACE_IMAGE,			"UNIT_FACE_IMAGE",			  TRUE},// 头像信息改变 2006-3-9
// 	        { GE_UNIT_EQUIP_VER,			"UNIT_EQUIP_VER",			  TRUE},
// 	        { GE_UNIT_EQUIP,				"UNIT_EQUIP",				  TRUE},
// 	        { GE_UNIT_EQUIP_WEAPON,			"UNIT_EQUIP_WEAPON",		  TRUE},
// 	        { GE_UNIT_TITLE,				"UNIT_TITLE",				  TRUE},
// 	        { GE_UNIT_STRIKEPOINT,			"UNIT_STRIKEPOINT",			  TRUE},
// 	        { GE_UNIT_RELATIVE,				"UNIT_RELATIVE",			  TRUE},
// 	        { GE_UNIT_RMB,					"UNIT_RMB",					  TRUE},
// 	        { GE_UINT_ATT_WIND,				"UINT_ATT_WIND",			  TRUE},// 风攻击
// 	        { GE_UINT_DEF_WIND,				"UINT_DEF_WIND",			  TRUE},// 风防御
// 	        { GE_UINT_ATT_THUNDER,			"UINT_ATT_THUNDER",			  TRUE},// 雷攻击
// 	        { GE_UINT_DEF_THUNDER,			"UINT_DEF_THUNDER",			  TRUE},// 雷防御	
// 	        //{ GE_UNIT_MAX_VIGOR,			"UNIT_MAX_VIGOR",			  TRUE},
// 	        //{ GE_UNIT_ENERGY,				"UNIT_ENERGY",				  TRUE},
// 	        //{ GE_UNIT_MAX_ENERGY,			"UNIT_MAX_ENERGY",			  TRUE},
// 	        { GE_UNIT_GOODBADVALUE,			"UNIT_GOODBADVALUE",		  TRUE},
// 	        {GE_UNIT_AMBIT,					"UNIT_AMBIT",				  TRUE},
// 	        {GE_UNIT_AMBIT_EXP,				"UNIT_AMBITEXP",			  TRUE},
//         //
//         //---------------------------------------------------------------------------
// 	        { GE_SHOW_CONTEXMENU,			"SHOW_CONTEXMENU",			},
// 	        { GE_TOGLE_COMPOSE_WINDOW,		"TOGLE_COMPOSE_WINDOW",		},
// 	        { GE_TOGLE_CONSOLE,				"TOGLE_CONSOLE",			},
// 	        { GE_ON_SKILL_ACTIVE,			"ON_SKILL_ACTIVE",			},
// 	        { GE_POSITION_MODIFY,			"POSITION_MODIFY",			},
// 	        { GE_CHAT_MESSAGE,				"CHAT_MESSAGE",				},
// 	        { GE_CHAT_CHANGE_PRIVATENAME,	"CHAT_CHANGE_PRIVATENAME",	},
// 	        { GE_CHAT_CHANNEL_CHANGED,		"CHAT_CHANNEL_CHANGED",		},
// 
// 	        { GE_CHAT_TAB_CREATE,			"CHAT_TAB_CREATE",			},
// 	        { GE_CHAT_TAB_CONFIG,			"CHAT_TAB_CONFIG",			},
// 	        { GE_CHAT_TAB_CREATE_FINISH,	"CHAT_TAB_CREATE_FINISH",	},
// 	        { GE_CHAT_TAB_CONFIG_FINISH,	"CHAT_TAB_CONFIG_FINISH",	},
// 	        { GE_CHAT_FACEMOTION_SELECT,	"CHAT_FACEMOTION_SELECT",	},
// 	        { GE_CHAT_TEXTCOLOR_SELECT,		"CHAT_TEXTCOLOR_SELECT",	},
// 	        { GE_CHAT_CONTEX_MENU,			"CHAT_CONTEX_MENU",			},
// 	        { GE_CHAT_ACTSET,				"CHAT_ACTSET",				},
// 	        { GE_CHAT_ACT_SELECT,			"CHAT_ACT_SELECT",			},
// 	        { GE_CHAT_ACT_HIDE,				"CHAT_ACT_HIDE",			},
// 	        { GE_CHAT_PINGBI_LIST,			"CHAT_PINGBI_LIST",			},
// 	        { GE_CHAT_PINGBI_UPDATE,		"CHAT_PINGBI_UPDATE",		},
// 	        { GE_ACCELERATE_KEYSEND,		"ACCELERATE_KEYSEND",		},
// 	        { GE_CHAT_ADJUST_MOVE_CTL,		"CHAT_ADJUST_MOVE_CTL",		},
// 	        { GE_CHAT_INPUTLANGUAGE_CHANGE,	"CHAT_INPUTLANGUAGE_CHANGE",	TRUE},
// 	        { GE_CHAT_SHOWUSERINFO,			"CHAT_SHOWUSERINFO",		},
// 	        { GE_CHAT_LOAD_TAB_CONFIG,		"CHAT_LOAD_TAB_CONFIG",		},
// 	        { GE_CHAT_MENUBAR_ACTION,		"CHAT_MENUBAR_ACTION",		},
// 	        { GE_CHAT_HISTORY_ACTION,		"CHAT_HISTORY_ACTION",		},
// 
// 	        { GE_LOOT_OPENED,				"LOOT_OPENED",				},
// 	        { GE_LOOT_SLOT_CLEARED,			"LOOT_SLOT_CLEARED",		},
// 	        { GE_LOOT_CLOSED,				"LOOT_CLOSED",				},
// 	        { GE_PROGRESSBAR_SHOW,			"PROGRESSBAR_SHOW",			},
// 	        { GE_PROGRESSBAR_HIDE,			"PROGRESSBAR_HIDE",			},
// 	        { GE_PROGRESSBAR_WIDTH,			"PROGRESSBAR_WIDTH",		},
// 	        { GE_CHANGE_BAR,				"CHANGE_BAR",				},
// 	        { GE_TOGLE_MISSION,				"TOGLE_MISSION",			},
// 	        { GE_UPDATE_MISSION,			"UPDATE_MISSION",			},
// 	        { GE_REPLY_MISSION,				"REPLY_MISSION",			},
// 	        { GE_UPDATE_REPLY_MISSION,		"UPDATE_REPLY_MISSION",		},
// 	        { GE_TOGLE_LARGEMAP,			"TOGLE_LARGEMAP",			},
// 	        { GE_TOGLE_SCENEMAP,			"TOGLE_SCENE_MAP",			},
// 	        { GE_OPEN_MINIMAP,				"OPEN_MINIMAP",				},
// 	        { GE_OPEN_MINIMAPEXP,			"OPEN_MINIMAPEXP",			},
// 
// 	        { GE_OPEN_AUTO_SEARCH_LIST,			"OPEN_AUTOSEARCH",			}, ////ZZH+
// 	        { GE_OPEN_YUANBAOSHOP,			"OPEN_YUANBAOSHOP",			}, ////ZZH+
// 
// 
// 	        { GE_QUEST_EVENTLIST,			"QUEST_EVENTLIST",			},
// 	        { GE_QUEST_INFO,				"QUEST_INFO",				},
// 	        { GE_QUEST_REGIE,				"QUEST_REGIE",				},
// 	        { GE_QUEST_CONTINUE_DONE,		"QUEST_CONTINUE_DONE",		},
// 	        { GE_QUEST_CONTINUE_NOTDONE,	"QUEST_CONTINUE_NOTDONE",	},
// 	        { GE_QUEST_AFTER_CONTINUE,		"QUEST_AFTER_CONTINUE",		},
// 	        { GE_QUEST_TIPS,				"QUEST_TIPS",				},
// 	        { GE_TOGLE_COMPOSEITEM,			"TOGLE_COMPOSEITEM",		},
// 	        { GE_TOGLE_COMPOSEGEM,			"TOGLE_COMPOSEGEM",			},
// 	        { GE_ON_LIFEABILITY_ACTIVE,		"ON_LIFEABILITY_ACTIVE",	},
// 	        { GE_NEW_DEBUGMESSAGE,			"NEW_DEBUGMESSAGE",			},
// 	        { GE_OPEN_BOOTH,				"OPEN_BOOTH",				},
// 	        { GE_CLOSE_BOOTH,				"CLOSE_BOOTH",				},
// 
// 	        //------------------------------------------------------------------------------------------------------------
// 	        // 装备属性界面消息
// 	        { GE_UPDATE_EQUIP,				"REFRESH_EQUIP",				},	// 更新装备, 无参数.
// 	        { GE_MANUAL_ATTR_SUCCESS_EQUIP,	"MANUAL_ATTR_SUCCESS_EQUIP",	},	// 手动调节属性成功.
// 	        { GE_CUR_TITLE_CHANGEED,		"CUR_TITLE_CHANGED",			},	// 当前标题改变了.
// 
// 
// 	        { GE_UPDATE_BOOTH,				"UPDATE_BOOTH",				},
// 	        { GE_SHOP_PILIANG_GOUMAI,		"SHOP_PILIANG_GOUMAI",				},
// 	        { GE_SHOP_PILIANG_CHUSHOU,		"SHOP_PILIANG_CHUSHOU"				},
// 	        { GE_SHOP_CHUSHOU_QUEREN,		"SHOP_CHUSHOU_QUERE",				},
// 
// 	        { GE_UPDATE_TEAM_MEMBER,		"ON_UPDATE_TEAM_MEMBER",	},
// 
// 	        { GE_OPEN_CHARACTOR,			"OPEN_CHARACTOR",			},
// 	        { GE_OPEN_EQUIP,				"OPEN_EQUIP",				},
// 	        { GE_TOGLE_JOINSCHOOL,			"TOGLE_JOINSCHOOL"			},
// 	        { GE_UPDATE_CONTAINER,			"UPDATE_CONTAINER",			},
// 	        { GE_IMPACT_SELF_UPDATE,		"IMPACT_SELF_UPDATE",		},
// 	        { GE_IMPACT_SELF_UPDATE_TIME,	"IMPACT_SELF_UPDATE_TIME",	},
// 	        { GE_OPEN_AGNAME,				"OPEN_AGNAME",				},
// 	        { GE_CLOSE_AGNAME,				"CLOSE_AGNAME",				},
// 	        { GE_TOGLE_BANK,				"TOGLE_BANK",				},
// 	        { GE_UPDATE_BANK,				"UPDATE_BANK",				},
// 	        { GE_TOGLE_INPUT_MONEY,			"TOGLE_INPUT_MONEY",		},
// 	        { GE_CLOSE_INPUT_MONEY,			"CLOSE_INPUT_MONEY",		},
// 
// 	        { GE_RECEIVE_EXCHANGE_APPLY,	"RECEIVE_EXCHANGE_APPLY",	},
// 	        { GE_STOP_EXCHANGE_TIP,			"STOP_EXCHANGE_TIP",		},
// 	        { GE_OPEN_EXCHANGE_FRAME,		"OPEN_EXCHANGE_FRAME",		},
// 	        { GE_UPDATE_EXCHANGE,			"UPDATE_EXCHANGE",			},
// 
// 	        { GE_CANCEL_EXCHANGE,			"CANCEL_EXCHANGE",			},
// 	        { GE_SUCCEED_EXCHANGE_CLOSE,	"SUCCEED_EXCHANGE_CLOSE",	},
// 	        { GE_UPDATE_PET_PAGE,			"UPDATE_PET_PAGE",			},
// 	        { GE_UPDATE_LIFETALISMAN_PAGE,		"UPDATE_LIFETALISMAN_PAGE",	},
// 	        { GE_OPEN_COMPOSE_ITEM,			"OPEN_COMPOSE_ITEM",		},
// 	        { GE_UPDATE_COMPOSE_ITEM,		"UPDATE_COMPOSE_ITEM",		},
// 	        { GE_OPEN_COMPOSE_GEM,			"OPEN_COMPOSE_GEM",			},
// 	        { GE_UPDATE_COMPOSE_GEM,		"UPDATE_COMPOSE_GEM",		},
// 	        { GE_AFFIRM_SHOW,				"AFFIRM_SHOW",				},
// 
// 	        // 摆摊相关
// 	        { GE_OPEN_STALL_SALE,			"OPEN_STALL_SALE",			},
// 	        { GE_OPEN_STALL_BUY,			"OPEN_STALL_BUY",			},
// 	        { GE_UPDATE_STALL_BUY,			"UPDATE_STALL_BUY",			},
// 	        { GE_UPDATE_STALL_SALE,			"UPDATE_STALL_SALE",		},
// 	        { GE_OPEN_STALL_RENT_FRAME,		"OPEN_STALL_RENT_FRAME",	},
// 	        { GE_STALL_SALE_SELECT,			"STALL_SALE_SELECT",		},
// 	        { GE_STALL_BUY_SELECT,			"STALL_BUY_SELECT",			},
// 	        { GE_OPEN_STALL_MESSAGE,		"OPEN_STALL_MESSAGE",		},
// 	        { GE_CLOSE_STALL_MESSAGE,		"CLOSE_STALL_MESSAGE",		},
// 
// 	        // 销毁物品
// 	        { GE_OPEN_DISCARD_ITEM_FRAME,	"OPEN_DISCARD_ITEM_FRAME",	},
// 	        // 不能销毁物品
// 	        { GE_OPEN_CANNT_DISCARD_ITEM,	"OPEN_CANNT_DISCARD_ITEM",	},
// 
// 	        { GE_SHOW_SPLIT_ITEM,			"SHOW_SPLIT_ITEM",			},
// 	        { GE_HIDE_SPLIT_ITEM,			"HIDE_SPLIT_ITEM",			},
// 	        { GE_TOGLE_FRIEND_INFO,			"TOGLE_FRIEND_INFO",		},
// 	        { GE_TOGLE_FRIEND,				"TOGLE_FRIEND",				},
// 	        { GE_OPEN_EMAIL,				"OPEN_EMAIL",				},
// 	        { GE_OPEN_EMAIL_WRITE,			"OPEN_EMAIL_WRITE",			},
// 	        { GE_HAVE_MAIL,					"HAVE_MAIL",				},
// 	        { GE_SEND_MAIL,					"SEND_MAIL",				},
// 	        { GE_CHAT_OPEN,					"CHAT_OPEN",				},
// 	        { GE_CHAT_CLOSED,				"CHAT_CLOSED",				},
// 	        { GE_UPDATE_EMAIL,				"UPDATE_EMAIL",				},
// 	        { GE_MOOD_SET,					"MOOD_SET",					},
// 	        { GE_MOOD_CHANGE,				"MOOD_CHANGE",				},
// 	        { GE_OPEN_HISTROY,				"OPEN_HISTROY",				},
// 
// 	        //系统设置相关
// 	        { GE_TOGLE_SYSTEMFRAME,			"TOGLE_SYSTEMFRAME",		},
// 	        { GE_TOGLE_VIEWSETUP,			"TOGLE_VIEWSETUP",			},
// 	        { GE_TOGLE_SOUNDSETUP,			"TOGLE_SOUNDSETUP",			},
// 	        { GE_TOGLE_UISETUP,				"TOGLE_UISETUP",			},
// 	        { GE_TOGLE_INPUTSETUP,			"TOGLE_INPUTSETUP",			},
// 	        { GE_TOGLE_GAMESETUP,			"TOGLE_GAMESETUP",			},
// 
// 	        //玩家商店
// 	        { GE_PS_OPEN_OTHER_SHOP,		"PS_OPEN_OTHER_SHOP",		},
// 	        { GE_PS_OPEN_MY_SHOP,			"PS_OPEN_MY_SHOP",			},
// 	        { GE_PS_OPEN_CREATESHOP,		"PS_OPEN_CREATESHOP",		},
// 	        { GE_PS_CLOSE_CREATESHOP,		"PS_CLOSE_CREATESHOP",		},
// 	        { GE_PS_OPEN_SHOPLIST,			"PS_OPEN_SHOPLIST",			},
// 	        { GE_PS_SELF_ITEM_CHANGED,		"PS_SELF_ITEM_CHANGED",		},
// 	        { GE_PS_OTHER_SELECT,			"PS_OTHER_SELECT",			},
// 	        { GE_PS_SELF_SELECT,			"PS_SELF_SELECT",			},
// 
// 	        { GE_PS_UPDATE_MY_SHOP,			"PS_UPDATE_MY_SHOP",		},
// 	        { GE_PS_UPDATE_OTHER_SHOP,		"PS_UPDATE_OTHER_SHOP",		},
//         									
// 	        { GE_PS_OPEN_OTHER_TRANS,		"PS_OPEN_OTHER_TRANS",		},
// 	        { GE_PS_UPDATE_OTHER_TRANS,		"PS_UPDATE_OTHER_TRANS",	},
// 	        { GE_PS_OTHER_TRANS_SELECT,		"PS_OTHER_TRANS_SELECT",	},
// 
// 	        { GE_OPEN_PS_MESSAGE_FRAME,		"OPEN_PS_MESSAGE_FRAME",	},
// 
// 	        { GE_PS_OPEN_MESSAGE,			"PS_OPEN_MESSAGE",			},
// 	        { GE_PS_UPDATE_MESSAGE,			"PS_UPDATE_MESSAGE",		},
// 
// 	        { GE_OPEN_PET_LIST,				"OPEN_PET_LIST",			},
// 	        { GE_VIEW_EXCHANGE_PET,			"VIEW_EXCHANGE_PET",		},
// 	        { GE_CLOSE_PET_LIST,			"CLOSE_PET_LIST",			},
// 	        { GE_UPDATE_PET_LIST,			"UPDATE_PET_LIST",			},
// 	        { GE_OPEN_PETJIAN_DLG,			"OPEN_PETJIAN_DLG",			},			////zzh+ 打开宠物图鉴
// 
// 	        { GE_OPEN_PRIVATE_INFO,			"OPEN_PRIVATE_INFO",		},
// 
// 	        { GE_OPEN_CALLOF_PLAYER,		"OPEN_CALLOF_PLAYER",		},
// 	        { GE_NET_CLOSE,					"NET_CLOSE",				},
// 
// 	        { GE_OPEN_ITEM_COFFER,			"OPEN_ITEM_COFFER",			},
// 	        { GE_PS_INPUT_MONEY,			"PS_INPUT_MONEY",			},
// 	        //------------------------------------------------------------------------------------------------------------
// 	        //
// 	        // 组队相关事件
//         	
// 	        { GE_TEAM_OPEN_TEAMINFO_DLG,	"TEAM_OPEN_TEAMINFO_DLG",	},		// 打开队伍信息对话框.
// 																		        // 
// 																		        // arg0 - 正整数, 从0开始
// 																		        // 0 : 打开队伍信息
// 																		        // 1 : 有人申请加入队伍
// 																		        // 2 : 有人邀请你加入队伍
// 
// 	        { GE_TEAM_NOTIFY_APPLY,			"TEAM_NOTIFY_APPLY",		},		// mainmenubar, 闪烁按钮. 
// 																		        // arg0 - 正整数, 从0开始
// 																		        // 0 : 表示已经加入队伍, 不需要闪烁按钮, 打开界面为队伍信息
// 																		        // 1 : 有人申请加入队伍
// 																		        // 2 : 有人邀请你加入队伍
// 																		        // 申请的人的具体信息从申请的信息队列中获得.
// 
// 	        { GE_TEAM_APPLY,				"TEAM_APPLY",				},		// 有人申请你加入队伍.
// 																		        // 
// 																		        // arg0 - 字符串
// 																		        // 申请的人.
// 
// 	        { GE_TEAM_INVITE,				"TEAM_INVITE",				},		// 有人邀请你加入队伍.
// 																		        // 
// 																		        // arg0 - 字符串
// 																		        // 邀请你的人.
//         	
// 	        { GE_TEAM_CLEAR_UI,				"TEAM_HIDE_ALL_PLAYER",		},		// 清空界面
//         										
//         	
// 	        { GE_TEAM_REFRESH_UI,			"TEAM_REFRESH_DATA",		},		// 刷新界面
//         	
//         	
// 	        { GE_TEAM_MEMBER_ENTER,			"TEAM_ENTER_MEMBER",		},		// 有新的队员进入
// 																		        //
// 																		        // arg0 - 正整数, 从1 开始
// 																		        // 在ui界面中的显示位置
//         	
// 	        { GE_TEAM_UPTEDATA_MEMBER_INFO,	"TEAM_UPDATE_MEMBER",		},		// 更新队员信息
// 																		        // arg0 - 正整数, 从1 开始
// 																		        // 在ui界面中的显示位置
//         	
// 	        { GE_TEAM_MESSAGE,				"TEAM_TEAM_MESSAGE",		},		// 队伍信息提示信息
// 																		        // arg0 - 字符串
// 																		        // 需要提示的信息.
//         	
// 	        { GE_TEAM_MEMBER_INVITE,		"TEAM_MEMBER_INVITE",      },		// 队员邀请某一个人加入队伍请求队长同意
// 																		        //
// 																		        // arg0 - 队员名字
// 																		        // arg1 - 被邀请人的名字
// 
// 	        { GE_TEAM_FOLLOW_INVITE,		"TEAM_FOLLOW_INVITE",		},		// 队长邀请队员进入组队跟随模式
// 																		        //
// 																		        // arg0 - 队长名字
// 
// 	        { GE_TEAM_REFRESH_MEMBER,		"TEAM_REFRESH_MEMBER",		},		// 刷新队员信息
// 																		        //
// 																		        // arg0 - 队员的位置索引
//         	
// 	        { GE_TEAM_STATE,				"TEAM_STATE",				},		//是否在组队中
// 
// 	        {GE_TEAM_ADMEASURE_SHOW,		"TEAM_ADMEASURE_SHOW",		},		// 打开组队分配界面 [6/20/2011 edit by ZL]
//         	
//             /*****************************************************************************************************************************
// 	        **
// 	        ** 以下以后需要删除.
// 	        **
// 	        **
// 	        ******************************************************************************************************************************/
// 	        //{ GE_ON_TEAM_ENTER_MEMBER,		"TEAM_ENTER_MEMBER",		},// 有新的队员进入
// 	        { GE_UPDATE_TEAM_MEMBER,		"ON_UPDATE_TEAM_MEMBER",	},// 更新队员信息
// 	        { GE_SHOW_TEAM_YES_NO,				"SHOW_TEAM_YES_NO",		},// 有人邀请你加入队伍.
// 	        { GE_SHOW_TEAM_MEMBER_INFO,			"SHOW_PLAYER_INFO",		},// 显示队员的详细信息
// 	        { GE_SHOW_TEAM_MEMBER_NAME,			"SHOW_PLAYER_NAME",		},// 在左边的队友列表框中显示队友的名字
// 	        { GE_HIDE_ALL_PLAYER,				"HIDE_ALL_PLAYER",		},// 自己离开队伍后, 清空左边的队伍列表.
// 	        { GE_SHOW_TEAM_MEMBER_NICK,			"SHOW_TEAM_NICK",		},// 昵称
// 	        { GE_SHOW_TEAM_MEMBER_FAMILY,		"SHOW_TEAM_FAMILY",		},// 五行属性
// 	        { GE_SHOW_TEAM_MEMBER_LEVEL,		"SHOW_TEAM_LEVEL",		},// 等级
// 	        { GE_SHOW_TEAM_MEMBER_POS,			"SHOW_TEAM_POS",		},// 位置
// 	        { GE_SHOW_TEAM_MEMBER_HP,			"SHOW_TEAM_HP",			},// hp
// 	        { GE_SHOW_TEAM_MEMBER_MP,			"SHOW_TEAM_MP",			},// mp
// 	        { GE_SHOW_TEAM_MEMBER_ANGER,		"SHOW_TEAM_ANGER",		},// 怒气
// 	        { GE_SHOW_TEAM_MEMBER_DEAD_LINK,	"SHOW_TEAM_DEAD_LINK",	},// 连线信息
// 	        { GE_SHOW_TEAM_MEMBER_DEAD,			"SHOW_TEAM_DEAD",		},// 死亡信息.
// 	        { GE_SHOW_TEAM_FUNC_MENU_MEMBER,	"SHOW_TEAM_CONTEXMENU_MEMBER", },// 显示队员的功能菜单
// 	        { GE_SHOW_TEAM_FUNC_MENU_LEADER,	"SHOW_TEAM_CONTEXMENU_LEADER", },// 显示队长的功能菜单
// 
// 	        // 组队相关事件
// 	        //---------------------------------------------------------------------------------------------------------------------------------------
// 
// 
// 	        //------------------------------------------------------------------------------------------------------------------------
// 	        //
// 	        // 登录流程相关
// 	        //
//         	
// 	        { GE_GAMELOGIN_SHOW_SYSTEM_INFO,		"GAMELOGIN_SHOW_SYSTEM_INFO",	},		// 显示系统信息
// 																					        //
// 																					        // arg0 - 字符串 : 需要提示的系统信息.
// 
// 
// 	        { GE_GAMELOGIN_SHOW_SYSTEM_INFO_CLOSE_NET,		"GAMELOGIN_SHOW_SYSTEM_INFO_CLOSE_NET",	},		// 显示系统信息
// 																									        //
// 																									        // arg0 - 字符串 : 需要提示的系统信息.
// 																									        //
// 																									        // 点击确认按钮后断开网络。
// 
// 
// 	        { GE_GAMELOGIN_CLOSE_SYSTEM_INFO,		"GAMELOGIN_CLOSE_SYSTEM_INFO",	},		// 显示队长的功能菜单
// 
//         	
// 	        { GE_GAMELOGIN_OPEN_SELECT_SERVER,		"GAMELOGIN_OPEN_SELECT_SERVER",	},		// 关闭选择服务器界面
//         																					
// 
// 	        { GE_GAMELOGIN_LASTSELECT_AREA_AND_SERVER, "GE_GAMELOGIN_LASTSELECT_AREA_AND_SERVER", }, //最后登录服务器
// 
// 
// 	        { GE_GAMELOGIN_CLOSE_SELECT_SERVER,		"GAMELOGIN_CLOSE_SELECT_SERVER",},		// 关闭选择服务器界面
// 
// 
// 	        { GE_GAMELOGIN_OPEN_COUNT_INPUT,		"GAMELOGIN_OPEN_COUNT_INPUT",	},		// 打开帐号输入界面
//         																					
// 
// 	        { GE_GAMELOGIN_CLOSE_COUNT_INPUT,		"GAMELOGIN_CLOSE_COUNT_INPUT",},		// 关闭帐号输入界面
// 
// 	        { GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON,		"GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON",	},	// 显示系统信息, 不显示按钮
// 																								        //
// 																								        // arg0 - 字符串 : 需要提示的系统信息.
// 
// 	        { GE_GAMELOGIN_OPEN_SELECT_CHARACTOR,		"GAMELOGIN_OPEN_SELECT_CHARACTOR",	},			// 显示人物选择界面
// 
// 	        { GE_GAMELOGIN_CLOSE_SELECT_CHARACTOR,		"GAMELOGIN_CLOSE_SELECT_CHARACTOR",	},			// 关闭人物选择界面
// 
// 
// 	        { GE_GAMELOGIN_OPEN_CREATE_CHARACTOR,		"GAMELOGIN_OPEN_CREATE_CHARACTOR", },			// 显示人物创建界面
// 
// 	        { GE_GAMELOGIN_CLOSE_CREATE_CHARACTOR,		"GAMELOGIN_CLOSE_CREATE_CHARACTOR",},			// 关闭人物关闭界面
// 
// 	        { GE_GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR,"GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR",},	// 刷新人物信息
// 
// 
// 	        { GE_GAMELOGIN_CLOSE_BACK_GROUND,			"GAMELOGIN_CLOSE_BACK_GROUND",},				// 关闭背景界面
// 
// 
// 	        { GE_GAMELOGIN_SYSTEM_INFO_YESNO,			"GAMELOGIN_SYSTEM_INFO_YESNO",},				// 系统信息提示 yes_no 界面.
// 																								        //
// 																								        // 参数0,	提示的字符串
// 																								        // 参数1,	对话框的类型
// 																								        //			0 -- 是否退出游戏
// 																								        //			1 -- 是否删除角色
// 																								        //			2 -- 是否更换帐号
// 
// 
// 
// 	        { GE_GAMELOGIN_SELECT_LOGIN_SERVER,			"GAMELOGIN_SELECT_LOGIN_SERVER",},				// 选择一个login server
// 																								        //
// 																								        // 参数0, iAreaIndex   区域索引
// 																								        // 参数1, iLoginServer 区域索引
// 
// 	        { GE_GAMELOGIN_CLEAR_ACCOUNT,				"GAMELOGIN_CLEAR_ACCOUNT"},						// 清空帐号显示.
// 
// 	        { GE_GAMELOGIN_SELECT_AREA,					"GAMELOGIN_SELECT_AREA"},						// 选择区域
//         												
// 	        { GE_GAMELOGIN_SELECT_LOGINSERVER,			"GAMELOGIN_SELECT_LOGINSERVER"},				// 选择Login Server
// 
// 	        { GE_GAMELOGIN_CREATE_CLEAR_NAME,			"GAMELOGIN_CREATE_CLEAR_NAME"},				// 清空创建角色名字。
// 
// 	        { GE_GAMELOGIN_CREATE_ROLE_OK,				"GAMELOGIN_CREATE_ROLE_OK"},				// 创建角色成功。2006－4－17
// 
// 	        { GE_GAMELOGIN_Delete_ROLE_OK,				"GAMELOGIN_Delete_ROLE_OK"},				// 删除角色成功。2010-9-16
//         	
//         												
// 
//         	
// 	        //
// 	        // 登录流程相关
// 	        //
// 	        //------------------------------------------------------------------------------------------------------------------------
// 
// 
// 	        { GE_TOGLE_SKILLSTUDY,				"TOGLE_SKILLSTUDY",		},		//TODO 检查是否需要保留 五行属性技能学习界面
// 	        { GE_SKILLSTUDY_SUCCEED,			"SKILLSTUDY_SUCCEED",	},		//TODO 检查是否需要保留 五行属性技能学习界面
// 	        { GE_TOGLE_ABILITY_STUDY,			"TOGLE_ABILITY_STUDY"	},		// 生活技能学习界面
// 
// 
// 	        { GE_SUPERTOOLTIP,					"SHOW_SUPERTOOLTIP" }, //显示超级tooltip
// 	        { GE_UPDATE_SUPERTOOLTIP,			"UPDATE_SUPERTOOLTIP" }, //刷新超级tooltip
// 
// 
// 	        //
// 	        // 复活界面相关
// 	        //
// 	        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 	        { GE_RELIVE_SHOW,					"RELIVE_SHOW",			},		// 显示复活界面
// 	        { GE_RELIVE_HIDE,					"RELIVE_HIDE",			},		// 隐藏复活界面
// 	        { GE_RELIVE_REFESH_TIME,			"RELIVE_REFESH_TIME",	},		// 刷新灵魂出窍的时间
// 
// 	        { GE_OBJECT_CARED_EVENT,			"OBJECT_CARED_EVENT",	},
// 
// 	        { GE_UPDATE_MAP,					"UPDATE_MAP",	},
// 
// 	        { GE_UI_COMMAND,					"UI_COMMAND",	},
// 	        { GE_OTHERPLAYER_UPDATE_EQUIP,		"OTHERPLAYER_UPDATE_EQUIP", },
// 	        { GE_VARIABLE_CHANGED,				"VARIABLE_CHANGED" , TRUE }, //全局变量发生改变
// 	        { GE_TIME_UPDATE,					"TIME_UPDATE", },
// 
// 	        { GE_UPDATE_FRIEND,					"UPDATE_FRIEND",	},	// 好友数据更新了
// 	        { GE_UPDATE_FRIEND_INFO,			"UPDATE_FRIEND_INFO", },
//         	
// 
// 	        { GE_UPDATE_TARGETPET_PAGE,			"UPDATE_TARGETPET_PAGE", },
// 	        { GE_UPDATE_PETSKILLSTUDY,			"UPDATE_PETSKILLSTUDY", },
// 	        { GE_UPDATE_PETINVITEFRIEND,		"UPDATE_PETINVITEFRIEND", },
// 	        { GE_REPLY_MISSION_PET,				"REPLY_MISSION_PET", },
//         	
// 	        //------------------------------------------------------------------------------------------------------------------------
// 	        //
// 	        // 是否设置二级保护密码
// 	        //
// 
// 	        { GE_MINORPASSWORD_OPEN_SET_PASSWORD_DLG,		"MINORPASSWORD_OPEN_SET_PASSWORD_DLG", },		// 打开设置二级密码界面
// 	        { GE_MINORPASSWORD_OPEN_UNLOCK_PASSWORD_DLG,	"MINORPASSWORD_OPEN_UNLOCK_PASSWORD_DLG", },	// 打开unlock密码界面。
// 	        { GE_MINORPASSWORD_OPEN_CHANGE_PASSWORD_DLG,	"MINORPASSWORD_OPEN_CHANGE_PASSWORD_DLG", },	// 打开更改密码界面。
// 
// 	        { GE_OPEN_SYSTEM_TIP_INFO_DLG,	"OPEN_SYSTEM_TIP_INFO_DLG", },									// 提示系统信息对话框。
// 
// 	        { GE_GUILD_CREATE,					"GUILD_CREATE", },
// 	        { GE_GUILD_CREATE_CONFIRM,			"GUILD_CREATE_CONFIRM", },
// 	        { GE_GUILD_SHOW_LIST,				"GUILD_SHOW_LIST", },
// 	        { GE_GUILD_SHOW_MEMBERINFO,			"GUILD_SHOW_MEMBERINFO", },
// 	        { GE_GUILD_SHOW_DETAILINFO,			"GUILD_SHOW_DETAILINFO", },
// 	        { GE_GUILD_SHOW_APPOINTPOS,			"GUILD_SHOW_APPOINTPOS", },
// 	        { GE_GUILD_DESTORY_CONFIRM,			"GUILD_DESTORY_CONFIRM", },
// 	        { GE_GUILD_QUIT_CONFIRM,			"GUILD_QUIT_CONFIRM", },
// 	        { GE_GUILD_UPDATE_MEMBERINFO,		"GUILD_UPDATE_MEMBERINFO", },
// 	        { GE_GUILD_FORCE_CLOSE,				"GUILD_FORCE_CLOSE", },
// 
// 	        { GE_CLOSE_MISSION_REPLY,			"CLOSE_MISSION_REPLY", },
// 	        { GE_CLOSE_TARGET_EQUIP,			"CLOSE_TARGET_EQUIP",},
// 	        { GE_VIEW_RESOLUTION_CHANGED,		"VIEW_RESOLUTION_CHANGED", },
// 	        { GE_CLOSE_SYNTHESIZE_ENCHASE,		"CLOSE_SYNTHESIZE_ENCHASE", },
// 	        { GE_TOGGLE_PETLIST,				"TOGGLE_PETLIST", },
// 	        { GE_PET_FREE_CONFIRM,				"PET_FREE_CONFIRM", },
// 
// 	        { GE_TOGLE_CONTAINER1,				"TOGLE_CONTAINER1",			},//fujia 2007.10.24
// 	        { GE_TOGLE_CONTAINER2,				"TOGLE_CONTAINER2",			},
// 	        { GE_NEW_GONGGAOMESSAGE,			"NEW_GONGGAOMESSAGE",		},
// 	        { GE_MYSELF_ENTER_FIGHTSTATE,		"MYSELF_ENTER_FIGHTSTATE",  },//  [8/13/2010 Sun]
// 	        { GE_MYSELF_LEAVE_FIGHTSTATE,		"MYSELF_LEAVE_FIGHTSTATE",  },
// 	        { GE_TOGLE_ZHENFA_SKILL_PAGE,		"TOGLE_ZHENFASKILL_PAGE",  },
// 	        { GE_UPDATE_ZHANFA_SKILL,            "UPDATE_ZHANFASKILL",     },
// 	        { GE_TOGLE_LIFE_PAGE,				 "TOGLE_LIFE_PAGE",			},
// 	        { GE_TOGLE_SKILL_BOOK,				 "TOGLE_SKILL_BOOK",        }, 	
// 	        { GE_UPDATE_STILETTO,				 "UPDATE_STILETTO",			},
// 	        { GE_SHOW_MISSION_BY_ID,			 "SHOW_MISSION_BY_ID",		},
// 	        { GE_CANNOT_FIND_NEARESTTARGET,      "CANNOT_FIND_NEARESTTARGET",},
// 	        { GE_SUPERTOOLTIP2,					"SHOW_SUPERTOOLTIP2" }, //显示超级tooltip
// 	        { GE_UPDATE_SUPERTOOLTIP2,			"UPDATE_SUPERTOOLTIP2" }, //刷新超级tooltip
// 	        { GE_INPUT_ITEM_LINK,				"INPUT_ITEM_LINK"},//加入item链接
// 
// 	        { GE_INFO_GAME,						"INFO_GAME"},//游戏信息
// 	        { GE_INFO_ACTIVITY,					"INFO_ACTIVITY"},//活动信息
// 	        { GE_INFO_SELF,						"INFO_SELF"},//个人信息
// 	        { GE_INFO_INTERCOURSE,				"INFO_INTERCOURSE"},//交互信息
// 	        { GE_INFO_FIGHT,					"INFO_FIGHT"},//战斗信息
// 	        { GE_INFO_ERROR,					"INFO_ERROR"},//错误信息
// 
// 	        { GE_MISSION_FINISH,				"MISSION_FINISH"},// 任务完成标志 [6/16/2011 ivan edit]
// 	        { GE_ENABLE_MISSION_UPDATE,			"ENABLE_MISSION_UPDATE"},// 刷新可接任务列表 [6/16/2011 ivan edit]
// 	        { GE_FRIEND_MSG_CONFIRM,			"FRIEND_MSG_CONFIRM"}, //好友界面消息确认
// 
// 	        { GE_FRIEND_ADD_FRIEND,				"FRIEND_ADD_FRIEND"},// 增加好友界面开关
// 	        { GE_FRIEND_SEARCH_FRIEND,			"FRIEND_SEARCH_FRIEND"},// 查找好友界面开关
// 	        { GE_FRIEND_SET_GROUP,				"FRIEND_SET_GROUP"},// 设置分组界面开关
// 	        { GE_FRIEND_CREATE_GROUP,			"FRIEND_CREATE_GROUP"},// 创建分组界面开关
// 	        { GE_FRIEND_FRIEND_DETAIL,			"FRIEND_FRIEND_DETAIL"},// 好友详细界面开关
// 	        { GE_FRIEND_MSG_MANAGER,			"FRIEND_MSG_MANAGER"},// 好友消息记录开关开关
// 	        { GE_FRIEND_MSG_SEND,				"FRIEND_MSG_SEND"},// 好友消息发送界面开关
// 	        { GE_FRIEND_SET_NICK,				"FRIEND_SET_NICK"},// 好友备注界面开关
// 	        { GE_FRIEND_SHOW_TIP,				"FRIEND_SHOW_TIP"},// 好友tip
// 	        { GE_FRIEND_HIDE_TIP,				"FRIEND_HIDE_TIP"},// 好友tip
// 	        { GE_ADD_SLOT_SUCCESS,				"ADD_SLOT_SUCCESS"},// 打孔成功 [7/15/2011 ivan edit]
// 	        { GE_UPDATE_SPLITGEM,				"UPDATE_SPLITGEM"},// 宝石摘除 [7/16/2011 ivan edit]
// 	        { GE_REMOVE_GEM_SUCCESS,			"REMOVE_GEM_SUCCESS"},// 宝石摘除成功 [7/16/2011 ivan edit]
// 	        { GE_STENGTHEN_UPDATE,				"STENGTHEN_UPDATE"},// 更新强化界面 [7/18/2011 ivan edit]
// 	        { GE_STENGTHEN_SUCCESS,				"STENGTHEN_SUCCESS"},// 强化成功 [7/18/2011 ivan edit]
// 	        { GE_CAMERAANIMATION_START,         "CAMERAANIMATION_START"},
// 	        { GE_CAMERAANIMATION_END,			"CAMERAANIMATION_END"},
// 	        { GE_OPEN_AMBIT,					"OPEN_AMBIT"},// 打开境界任务 [8/10/2011 edit by ZL]
// 	        { GE_UPDATE_ALL_TITLE,				"UPDATE_ALL_TITLE"},// 刷新所有玩家称号 [8/9/2011 ivan edit]
// 	        { GE_USER_FIRST_LOGIN,				"USER_FIRST_LOGIN"},
// 	        { GE_TRACE_MISSION,					"TRACE_MISSION"},
// 	        { GE_DROP_ITEMBOX,					"DROP_ITEMBOX"	},
// 	        { GE_CLOSE_EQUIP,					"CLOSE_EQUIP"},
// 	        { GE_GET_NEWEQUIP,					"GET_NEWEQUIP"	},
// 	        { GE_GET_NEWAMBITMISSION,			"GET_NEWAMBITMISSION"},// 得到新的境界任务 arg0：为脚本编号 [8/18/2011 edit by ZL]
// 	        { GE_AMBITMISSION_FINISHED,			"AMBITMISSION_FINISHED"},// 境界任务处于可完全状态 [8/18/2011 edit by ZL]
// 	        { GE_TEAM_RULER_CHANGE,				"TEAM_RULER_CHANGE"},// 队伍分配方式改变 [8/24/2011 edit by ZL]
// 	        { GE_WINDOW_POSITION_CHANGED,		"WINDOW_POSITION_CHANGED"},
// 	        { GE_AUTO_RUNING_CHANGE,			"AUTO_RUNING_CHANGE"},
// 	        { GE_NEW_ITEM,						"NEW_ITEM"},
// 	        { GE_MESSAGEBOX,					"MESSAGEBOX"},
// 	        { GE_ASK_ENCHASE_MSG,				"ASK_ENCHASE_MSG"},// 询问镶嵌消息 [9/16/2011 edit by ZL]
// 	        { GE_YES_ENCHASE_MSG,				"YES_ENCHASE_MSG"},// 确定镶嵌消息 [9/16/2011 edit by ZL]
// 	        { GE_UPDATE_EPUIP_QUALITY_UP,		"UPDATE_EPUIP_QUALITY_UP"},// 装备升品消息 [9/19/2011 edit by ZL]
// 	        { GE_UPDATE_EPUIP_LEVEL_UP,			"UPDATE_EPUIP_LEVEL_UP"},// 装备升级消息 [9/19/2011 edit by ZL]
// 	        { GE_UPDATE_EPUIP_REPRODUCE,		"UPDATE_EPUIP_REPRODUCE"},// 装备重铸消息 [10/9/2011 edit by ZL]
// 	        { GE_UPDATE_EPUIP_PRINTSOUL,		"UPDATE_EPUIP_PRINTSOUL"},// 装备魂印消息 [10/9/2011 edit by ZL]
// 
// 	        { GE_TOGLE_ACTIVITYDETAIL,		"TOGLE_ACTIVITYDETAIL"},// 活动详情 [10/31/2011 edit by sqy]
// 
// 	        { GE_BUFF_UPDATE,					"BUFF_UPDATE"},	// 广播更新BUFF [11/10/2011 Ivan edit]
// 	        { GE_NEW_SKILL,						"NEW_SKILL"},	// 学会新技能 [11/15/2011 Ivan edit]
        };
}
