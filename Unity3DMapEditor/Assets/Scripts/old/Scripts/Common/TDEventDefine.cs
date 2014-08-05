
public enum GAME_EVENT_ID
{
    GE_APPLICATION_INITED,			//游戏程序初始化
    GE_ON_SCENE_TRANS,				//场景转移
    GE_SCENE_TRANSED,				//场景转移后
    // arg0- 小地图的名称
    //
    GE_ON_SCENE_UPDATED,			//场景更新(一般用于玩家城市)
    // arg0- 小地图的名称
    //
    GE_ON_SERVER_TRANS,				//服务器转移
    GE_PLAYER_ENTER_WORLD,			//进入世界
    GE_PLAYER_LEAVE_WORLD,			//退出世界
    GE_TEAM_CHANG_WORLD,			// 队友改变场景了
    GE_SKILL_CHANGED,				//玩家的某项技能发生改变,
    // arg0 - 技能id
    GE_PACKAGE_ITEM_CHANGED,		//包裹中的某个物品发生改变
    // arg0 - 物品在包裹中的编号

    GE_MAINTARGET_CHANGED,			//主选择对象更改
    // arg0 - 对象ServerId


    GE_MAINTARGET_OPEND,			// 当target 窗口选择是队友的情况， 打开窗口 2006-4-12


    GE_TOGLE_LIFETALISMAN_BOOK,			// 打开/关闭技能书
    GE_CLOSE_SKILL_BOOK,			// 关闭技能书

    GE_TOGLE_PET_PAGE,				// 打开宠物界面
    GE_TOGLE_RIDE_PAGE,				////zzh+ 打开骑乘界面
    GE_TOGLE_OTHER_INFO_PAGE,				////zzh+ 打开骑乘界面

    //GE_TOGLE_NATURALSKILL_PAGE,				// 打开生活技能界面
    GE_TOGLE_XINFASKILL_PAGE,       // 心法技能
    GE_TOGLE_COMMONSKILL_PAGE,		// 打开普通技能界面
    GE_SKILL_UPDATE,				//玩家技能数据发生改变

    GE_TOGLE_CONTAINER,				// 打开/关闭包裹

    GE_TOGLE_MISSION_TRACE,

    //-----------------------------------------------------
    //角色属性
    GE_UNIT_HP,						//某个单元的HP发生变化
    // arg0 - 单元名称
    //		"player"	- 玩家自己
    //		"target"	- 主目标
    //		"pet"		- 自己的宠物
    //		"party*"	- 队友(1-4)
    //		"partypet*"	- 队友的宠物(1-4)
    //		...
    GE_UNIT_MP,						//某个单元的MANA发生变化
    // arg0 - 单元名称(同上)

    GE_UNIT_RAGE,					//某个单元的怒气发生变化
    // arg0 - 单元名称(同上)

    GE_UNIT_XINFA,					//某个单元的心法等级发生变化
    // arg0 - 单元名称(同上)
    // arg1 - 心法名称

    GE_UNIT_EXP,					//某个单元的经验值发生变化
    // arg0 - 单元名称(同上)
    GE_UNIT_AMBIT_EXP,				//某个单元的仙缘值发生变化
    // arg0 - 单元名称(同上)
    GE_UNIT_AMBIT,					// 境界

    GE_UNIT_MONEY,					//某个单元的钱发生变化
    // arg0 - 单元名称(同上)

    GE_UNIT_RACE_ID,				// 数据表中的ID
    GE_UNIT_NAME,					// 角色的名字，从服务器传来
    GE_UNIT_CAMP_ID,				// 阵营ID
    GE_UNIT_LEVEL,					// 等级
    GE_UNIT_MOVE_SPEED,				// 移动的速度
    GE_UNIT_FIGHT_STATE,			// 战斗状态	
    GE_UNIT_MAX_EXP,				// 最大EXP
    GE_UNIT_TITLE,					// 称号
    GE_UNIT_STRIKEPOINT,			// 连击点
    GE_UNIT_RELATIVE,				// 归属问题

    //-------------
    //一级战斗属性
    GE_UNIT_STR,					// 外功
    GE_UNIT_SPR,					// 内功
    GE_UNIT_CON,					// 身法
    GE_UNIT_INT,					// 体魄
    GE_UNIT_DEX,					// 智慧
    GE_UNIT_POINT_REMAIN,			// 剩余待分配点数

    //培养点数
    GE_UNIT_BRING_STR,
    GE_UNIT_BRING_SPR,
    GE_UNIT_BRING_CON,
    GE_UNIT_BRING_INT,
    GE_UNIT_BRING_DEX,

    //-------------
    //二级战斗属性
    GE_UNIT_ATT_PHYSICS,			// 物理攻击力			
    GE_UNIT_ATT_MAGIC,				// 魔法攻击力			
    GE_UNIT_DEF_PHYSICS,			// 物理防御力			
    GE_UNIT_DEF_MAGIC,				// 魔法防御力			
    GE_UNIT_MAX_HP,					// 最大生命点			
    GE_UNIT_MAX_MP,					// 最大魔法点			
    GE_UNIT_HP_RE_SPEED,			// HP恢复速度  点/秒	
    GE_UNIT_MP_RE_SPEED,			// MP恢复速度  点/秒	
    GE_UNIT_HIT,					// 命中率				
    GE_UNIT_MISS,					// 闪避率				
    GE_UNIT_CRIT_RATE,				// 会心率				
    GE_UNIT_DEF_CRIT_RATE,			// 会心防御 (新增 ivan 2010年12月24日)				
    GE_UNIT_ATT_SPEED,				// 攻击速度	
    GE_UNIT_ATT_COLD,				// 冰攻击	
    GE_UNIT_DEF_COLD,				// 冰防御	
    GE_UNIT_ATT_FIRE,				// 火攻击	
    GE_UNIT_DEF_FIRE,				// 火防御	
    GE_UNIT_ATT_LIGHT,				// 电攻击
    GE_UNIT_DEF_LIGHT,				// 电防御
    GE_UNIT_ATT_POSION,				// 毒攻击
    GE_UNIT_DEF_POSION,				// 毒防御
    GE_UNIT_ATT_METAL,				// 金攻击
    GE_UNIT_DEF_METAL,				// 金防御
    GE_UNIT_ATT_WOOD,				// 木攻击
    GE_UNIT_DEF_WOOD,				// 木防御
    GE_UNIT_ATT_WATER,				// 水攻击
    GE_UNIT_DEF_WATER,				// 水防御
    GE_UNIT_ATT_EARTH,				// 土攻击	
    GE_UNIT_DEF_EARTH,				// 土防御	
    GE_UINT_ATT_WIND,				// 风攻击
    GE_UINT_DEF_WIND,				// 风防御
    GE_UINT_ATT_THUNDER,			// 雷攻击
    GE_UINT_DEF_THUNDER,			// 雷防御	
    GE_UNIT_RMB,					// 元宝		//GE_UNIT_VIGOR,					// 活力值
    //GE_UNIT_MAX_VIGOR,				// 活力值上限
    GE_UNIT_ENERGY,					// 精力值
    GE_UNIT_MAX_ENERGY,				// 精力值上限
    GE_UNIT_GOODBADVALUE,

    GE_UNIT_MENPAI,					// 五行属性
    GE_UNIT_HAIR_MESH,				// -> DBC_CHARACTER_HAIR_GEO
    GE_UNIT_HAIR_COLOR,				// 头发颜色
    GE_UNIT_FACE_MESH,				// -> DBC_CHARACTER_HEAD_GEO
    GE_UNIT_FACE_IMAGE,				// 头像信息改变 2006-3-9
    GE_UNIT_EQUIP_VER,				// 角色的装备状态版本号，用于和服务器同步数据的依据
    GE_UNIT_EQUIP,					// 装备表(含武器)
    GE_UNIT_EQUIP_WEAPON,			// 武器


    //--------------------------------------------------------------------------------

    GE_SHOW_CONTEXMENU,				//显示右键菜单
    // arg0 - 菜单类
    //		"player"		- 玩家自己
    //		"other_player"  - 其他玩家
    //		"npc"			- npc
    // arg1 - 鼠标位置_x
    // arg2 - 鼠标位置_y

    GE_TOGLE_COMPOSE_WINDOW,		// 打开关闭合成界面

    GE_TOGLE_CONSOLE,				// 显示控制台

    GE_ON_SKILL_ACTIVE,				// 某个技能开始使用
    // arg0 - 技能ID

    GE_POSITION_MODIFY,				// 人物得位置改变了

    GE_CHAT_MESSAGE,				// 得到聊天信息
    /*
    |  arg0 - 聊天类型
    |		"near"		- 附近玩家
    |		"scene"		- 场景
    |		"sys"		- 系统
    |		"team"		- 队伍
    |		"guild"		- 帮会
    |		"user"		- 自建
    |		"private"	- 私聊
    |
    |  arg1 - 说话者的名字
    |
    |  arg2 - 说话内容
    |
    */

    GE_CHAT_CHANNEL_CHANGED,		//聊天频道发生改变
    GE_CHAT_CHANGE_PRIVATENAME,		//修改聊天中的密语对象
    GE_CHAT_TAB_CREATE,				//创建聊天页面
    GE_CHAT_TAB_CREATE_FINISH,		//创建聊天页面完成
    GE_CHAT_TAB_CONFIG,				//配置聊天页面
    GE_CHAT_TAB_CONFIG_FINISH,		//配置聊天页面完成
    GE_CHAT_FACEMOTION_SELECT,		//聊天表情选择
    GE_CHAT_TEXTCOLOR_SELECT,		//聊天文字颜色选择
    GE_CHAT_CONTEX_MENU,			//聊天相关的快捷菜单
    GE_CHAT_ACTSET,					//聊天动作命令
    GE_CHAT_ACT_SELECT,				//聊天动作命令界面显示
    GE_CHAT_ACT_HIDE,				//聊天动作命令界面关闭，因为动作命令界面比较特殊，需要支持托拽。
    GE_CHAT_PINGBI_LIST,			//聊天屏蔽界面
    GE_CHAT_PINGBI_UPDATE,			//屏蔽列表更新
    GE_CHAT_ADJUST_MOVE_CTL,		//通知聊天窗口调整大小
    GE_CHAT_INPUTLANGUAGE_CHANGE,	//输入法变更
    GE_CHAT_SHOWUSERINFO,			//聊天查看玩家信息界面
    GE_CHAT_LOAD_TAB_CONFIG,		//聊天页面配置信息
    GE_CHAT_MENUBAR_ACTION,			//聊天按钮命令 --ivan
    GE_CHAT_HISTORY_ACTION,			//聊天历史命令 --ivan

    GE_ACCELERATE_KEYSEND,			//键盘快捷键发送

    GE_LOOT_OPENED,					// 箱子打开
    GE_LOOT_SLOT_CLEARED,			// 箱子中的某个位置清空
    // arg0 - 箱子中的位置

    GE_LOOT_CLOSED,					// 箱子关闭

    GE_PROGRESSBAR_SHOW,			// 进度条显示
    GE_PROGRESSBAR_HIDE,			// 进度条隐藏
    GE_PROGRESSBAR_WIDTH,			// 进度条宽度调整
    // arg0 - 调整到的百分比 1 < x < 100

    GE_CHANGE_BAR,					//改变工具条上的Action
    // arg0 - Bar类型
    // arg1 - Index
    // arg2 - ActionItem ID

    GE_TOGLE_MISSION,				//接收任务界面
    GE_UPDATE_MISSION,				//刷新任务列表
    GE_REPLY_MISSION,				//提交任务界面
    GE_UPDATE_REPLY_MISSION,		//刷新提交任务界面

    GE_TOGLE_LARGEMAP,				// 关闭打开大地图
    GE_TOGLE_SCENEMAP,				// 关闭打开大地图
    GE_OPEN_MINIMAP,				// 打开小地图
    GE_OPEN_MINIMAPEXP,				// 打开扩展小地图

    GE_OPEN_AUTO_SEARCH_LIST,				////ZZH+打开扩展小地图
    GE_OPEN_YUANBAOSHOP, ////ZZH+

    GE_QUEST_EVENTLIST,				//打开和npc第一次对话时的可执行脚本列表
    GE_QUEST_INFO,					//打开和npc第二次对话(在接任务时，看到的任务信息，等待接受)
    GE_QUEST_REGIE,					//漕运任务
    GE_QUEST_CONTINUE_DONE,			//接受任务后，再次和npc对话，所得到的任务需求信息，(任务完成)
    GE_QUEST_CONTINUE_NOTDONE,		//接受任务后，再次和npc对话，所得到的任务需求信息，(任务未完成)
    GE_QUEST_AFTER_CONTINUE,		//点击“继续之后”，奖品选择界面
    GE_QUEST_TIPS,					//游戏过程中的任务进展提示信息

    GE_TOGLE_COMPOSEITEM,			// 打开/关闭 合成界面
    // arg0 - 生活技能的名称
    GE_TOGLE_COMPOSEGEM,			// 打开/关闭 宝石合成界面
    // 
    GE_ON_LIFEABILITY_ACTIVE,		// 某个生活技能开始使用
    // arg0 - 技能ID
    // arg1 - 配方
    // arg2 - 平台
    GE_NEW_DEBUGMESSAGE,			//新的debug string弹出，在屏幕上用listbox显示。

    //-------------------------------------------------------------------------------------------------------
    // 人物属性界面
    GE_UPDATE_EQUIP,				// 更新装备
    GE_OPEN_BOOTH,					// 打开货架栏
    GE_CLOSE_BOOTH,					// 关闭货架栏
    GE_MANUAL_ATTR_SUCCESS_EQUIP,	// 手动调节属性成功.
    GE_CUR_TITLE_CHANGEED,			// 当前人物的称号改变了.

    GE_UPDATE_BOOTH,				// 货架更新
    GE_SHOP_PILIANG_GOUMAI,				// 玩家批量购买 ADD ZL
    GE_SHOP_PILIANG_CHUSHOU,			// 玩家批量出售 ADD ZL
    GE_SHOP_CHUSHOU_QUEREN,				// 玩家确认出售 ADD ZL

    GE_OPEN_CHARACTOR,				// 打开人物属性栏
    GE_OPEN_EQUIP,					// 打开装备栏
    GE_TOGLE_JOINSCHOOL,			// TODO 检查是否需要保留 打开加入门派的对话框

    GE_UPDATE_CONTAINER,			// 包裹状态更新
    GE_IMPACT_SELF_UPDATE,			// 自己的特效更新 
    GE_IMPACT_SELF_UPDATE_TIME,		// 自己的特效时间的更新 
    GE_TOGLE_SKILLSTUDY,			// 技能心法的学习和升级
    GE_SKILLSTUDY_SUCCEED,			// 技能心法学习成功

    GE_TOGLE_ABILITY_STUDY,			// 生活技能学习界面
    GE_OPEN_AGNAME,					// 打开选择称号界面
    GE_CLOSE_AGNAME,				// 关闭选择称号界面

    GE_TOGLE_BANK,					// 打开银行界面
    GE_UPDATE_BANK,					// 更新银行界面
    GE_TOGLE_INPUT_MONEY,			// 打开输入钱的界面
    GE_CLOSE_INPUT_MONEY,			// 关闭输入金钱界面

    GE_RECEIVE_EXCHANGE_APPLY,		// 收到交易请求
    GE_STOP_EXCHANGE_TIP,			// 停止交易请求提示
    GE_OPEN_EXCHANGE_FRAME,			// 打开交易对话筐
    GE_UPDATE_EXCHANGE,				// 更新交易界面
    GE_CANCEL_EXCHANGE,				// 取消交易
    GE_SUCCEED_EXCHANGE_CLOSE,		// 交易成功，通知关闭交易界面（可能还会关闭相关界面）
    GE_UPDATE_PET_PAGE,				// 刷新宠物界面
    GE_UPDATE_LIFETALISMAN_PAGE,		// 刷新生活技能界面
    GE_OPEN_COMPOSE_ITEM,			// 打开制作物品界面
    GE_UPDATE_COMPOSE_ITEM,			// 刷新制作物品界面
    GE_OPEN_COMPOSE_GEM,			// 打开宝石合成/镶嵌界面
    GE_UPDATE_COMPOSE_GEM,			// 刷新宝石合成/镶嵌界面
    GE_AFFIRM_SHOW,					// 打开放弃任务确认界面


    GE_OPEN_STALL_SALE,				// 摆摊(卖)
    GE_OPEN_STALL_BUY,				// 摆摊(买)
    GE_UPDATE_STALL_BUY,			// 更新(卖)
    GE_UPDATE_STALL_SALE,			// 更新(买)
    GE_OPEN_STALL_RENT_FRAME,		// 摆摊(租金税率提示)
    GE_STALL_SALE_SELECT,			// 摆摊(卖)时有选中摊位上的物品
    GE_STALL_BUY_SELECT,			// 摆摊(买)时有选中摊位上的物品
    GE_OPEN_STALL_MESSAGE,			// 摆摊(信息发布界面)
    GE_CLOSE_STALL_MESSAGE,			// 摆摊(关系消息界面)

    GE_OPEN_DISCARD_ITEM_FRAME,		// 销毁物品
    GE_OPEN_CANNT_DISCARD_ITEM,		// 不能销毁物品

    GE_SHOW_SPLIT_ITEM,				// 打开拆分武平对话框
    GE_HIDE_SPLIT_ITEM,				// 关闭拆分武平对话框

    GE_TOGLE_FRIEND_INFO,			// 打开关闭好友列表对话框
    GE_TOGLE_FRIEND,				// 打开关闭好友对话框
    GE_UPDATE_FRIEND,				// 跟新好友数据了
    GE_UPDATE_FRIEND_INFO,			// 好友数据界面打开
    GE_OPEN_EMAIL,					// 打开信件界面
    GE_OPEN_EMAIL_WRITE,			// 打开信件界面
    GE_HAVE_MAIL,					// 有邮件
    GE_SEND_MAIL,					// 发送邮件
    GE_UPDATE_EMAIL,				// 刷新当前邮件
    GE_OPEN_HISTROY,				// 玩家历史消息

    //系统设置相关
    GE_TOGLE_SYSTEMFRAME,			// 打开系统主界面
    GE_TOGLE_VIEWSETUP,				// 打开视频设置界面
    GE_TOGLE_SOUNDSETUP,			// 打开声音设置界面
    GE_TOGLE_UISETUP,				// 打开界面设置
    GE_TOGLE_INPUTSETUP,			// 打开按键设置
    GE_TOGLE_GAMESETUP,				// 游戏性设置

    //玩家商店
    GE_PS_OPEN_OTHER_SHOP,			// 打开别人的商店
    GE_PS_OPEN_MY_SHOP,				// 打开自己的商店
    GE_PS_OPEN_CREATESHOP,			// 打开创建商店界面
    GE_PS_CLOSE_CREATESHOP,			// 关闭创建商店界面
    GE_PS_OPEN_SHOPLIST,			// 打开商店列表界面
    GE_PS_SELF_ITEM_CHANGED,		// 自己商店内物品发生改变
    GE_PS_OTHER_SELECT,				// 选中一个物品
    GE_PS_SELF_SELECT,				// 选中一个商品

    GE_PS_UPDATE_MY_SHOP,			// 更新自己商店的数据
    GE_PS_UPDATE_OTHER_SHOP,		// 更新商店的数据

    GE_PS_OPEN_OTHER_TRANS,			// 打开自己的盘出状态中的商店
    GE_PS_UPDATE_OTHER_TRANS,		// 更新
    GE_PS_OTHER_TRANS_SELECT,		// 选中

    GE_OPEN_PS_MESSAGE_FRAME,		// 玩家商店操作中需要弹出的确认对话框

    GE_PS_OPEN_MESSAGE,				// 打开玩家商店的消息框
    GE_PS_UPDATE_MESSAGE,			// 更新玩家商店的消息框

    GE_OPEN_PET_LIST,				// 交易等过程使用的宠物列表
    GE_VIEW_EXCHANGE_PET,			// 交易等过程中显示宠物
    GE_CLOSE_PET_LIST,				// 交易等过程结束的时候关闭宠物列表界面
    GE_UPDATE_PET_LIST,				// 更新宠物列表界面
    GE_OPEN_PETJIAN_DLG,			////zzh+ 打开宠物图鉴

    GE_OPEN_PRIVATE_INFO,			// 打开玩家信息对话框

    GE_OPEN_CALLOF_PLAYER,			// 打开有人要拉你的对话框
    GE_NET_CLOSE,					// 断开连接

    GE_OPEN_ITEM_COFFER,			// 打开锁定物品界面

    GE_PS_INPUT_MONEY,				// 弹出玩家商店的输入钱的框

    //--------------------------------------------------------------------------------------------------------------------
    // 组队相关事件

    GE_TEAM_OPEN_TEAMINFO_DLG,			// 打开队伍信息对话框.
    // 
    // arg0 - 正整数, 从0开始
    // 0 : 有人邀请你加入队伍
    // 1 : 有人申请加入队伍
    // 2 : 打开队伍信息
    // -1: 关闭对话框


    GE_TEAM_NOTIFY_APPLY,				// 通知队长, 有人申请加入队伍.
    // 
    // arg0 - 正整数, 从0开始
    // 0 : 有人邀请你加入队伍
    // 1 : 有人申请加入队伍
    // 申请的人的具体信息从申请的信息队列中获得.


    GE_TEAM_APPLY,						// 有人申请你加入队伍.
    // 
    // arg0 - 字符串
    // 申请的人.


    GE_TEAM_INVITE,						// 有人邀请你加入队伍.
    // 
    // arg0 - 字符串
    // 邀请你的人.


    GE_TEAM_CLEAR_UI,					// 清空界面
    //
    // 无参数


    GE_TEAM_REFRESH_UI,					// 刷新界面
    //
    // 无参数


    GE_TEAM_MEMBER_ENTER,				// 有新的队员进入
    //
    // arg0 - 正整数, 从1 开始
    // 在ui界面中的显示位置


    GE_TEAM_UPTEDATA_MEMBER_INFO,		// 更新队员信息
    // arg0 - 正整数, 从1 开始
    // 在ui界面中的显示位置


    GE_TEAM_MESSAGE,					// 队伍信息提示信息
    // arg0 - 字符串
    // 需要提示的信息.

    // 注意, 这条消息包含了提示给界面的信息
    // 如: xxx离开队伍
    //     xxx已经在一个队伍中.


    GE_TEAM_MEMBER_INVITE,				// 队员邀请某一个人加入队伍请求队长同意
    //
    // arg0 - 队员名字
    // arg1 - 被邀请人的名字

    GE_TEAM_FOLLOW_INVITE,				// 队长邀请队员进入组队跟随模式
    //
    // arg0 - 队长名字

    GE_TEAM_REFRESH_MEMBER,				// 刷新队员信息
    //
    // arg0 - 队员的位置索引

    GE_TEAM_STATE,						// 是否在组队状态中

    GE_TEAM_ADMEASURE_SHOW,				// 打开组队分配界面 [6/20/2011 edit by ZL]

    /**********************************************************************************************************
    **
    ** 以下以后需要删除
    **
    **
    **
    ***********************************************************************************************************/

    GE_ON_TEAM_ENTER_MEMBER,		// 有新队员入队

    GE_SHOW_TEAM_YES_NO,			// 打开同意组队窗口.

    GE_SHOW_TEAM_MEMBER_INFO,		// 显示队员的详细信息.

    GE_SHOW_TEAM_MEMBER_NAME,		// 在左边的队友列表框中显示队友的名字

    GE_HIDE_ALL_PLAYER,				// 自己离开队伍后, 隐藏所有的队友界面

    // 队员的详细信息
    GE_SHOW_TEAM_MEMBER_NICK,		// 名字

    GE_SHOW_TEAM_MEMBER_FAMILY,		// 五行属性
    GE_SHOW_TEAM_MEMBER_LEVEL,		// 等级
    GE_SHOW_TEAM_MEMBER_POS,		// 位置
    GE_SHOW_TEAM_MEMBER_HP,			// hp
    GE_SHOW_TEAM_MEMBER_MP,			// mp
    GE_SHOW_TEAM_MEMBER_ANGER,		// 怒气
    GE_SHOW_TEAM_MEMBER_DEAD_LINK,  // 连线信息
    GE_SHOW_TEAM_MEMBER_DEAD,		// 死亡信息.

    GE_UPDATE_TEAM_MEMBER,			// 更新队员信息
    // arg0 - 队员序号（队伍中，自己不占用序号）
    // arg1 - 队员 guid（用于取得该队员信息）

    GE_SHOW_TEAM_FUNC_MENU_MEMBER,	// 显示队员的功能菜单
    GE_SHOW_TEAM_FUNC_MENU_LEADER,	// 显示队长的功能菜单

    //
    // 组队相关事件
    //------------------------------------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------------------------------------
    //
    // 登录流程相关
    //

    GE_GAMELOGIN_SHOW_SYSTEM_INFO_CLOSE_NET,	// 显示系统信息
    //
    // arg0 - 字符串 : 需要提示的系统信息.
    //
    // 点击确认按钮后断开网络。

    GE_GAMELOGIN_SHOW_SYSTEM_INFO,				// 显示系统信息
    //
    // arg0 - 字符串 : 需要提示的系统信息.


    GE_GAMELOGIN_CLOSE_SYSTEM_INFO,				// 显示队长的功能菜单


    GE_GAMELOGIN_OPEN_SELECT_SERVER,			// 关闭选择服务器界面

    GE_GAMELOGIN_LASTSELECT_AREA_AND_SERVER,    // 最后登录服务器


    GE_GAMELOGIN_CLOSE_SELECT_SERVER,			// 关闭选择服务器界面


    GE_GAMELOGIN_OPEN_COUNT_INPUT,				// 打开帐号输入界面


    GE_GAMELOGIN_CLOSE_COUNT_INPUT,				// 关闭帐号输入界面

    GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON,	// 显示系统信息, 不显示按钮
    //
    // arg0 - 字符串 : 需要提示的系统信息.

    GE_GAMELOGIN_OPEN_SELECT_CHARACTOR,			// 显示人物选择界面

    GE_GAMELOGIN_CLOSE_SELECT_CHARACTOR,		// 关闭人物选择界面

    GE_GAMELOGIN_OPEN_CREATE_CHARACTOR,			// 显示人物创建界面

    GE_GAMELOGIN_CLOSE_CREATE_CHARACTOR,		// 关闭人物创建界面

    GE_GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR,	// 刷新人物信息

    GE_GAMELOGIN_CLOSE_BACK_GROUND,				// 关闭背景界面

    GE_GAMELOGIN_SYSTEM_INFO_YESNO,				// 系统信息提示 yes_no 界面.
    //
    // 参数0, 提示的字符串
    // 参数1,	对话框的类型
    //			0 -- 是否退出游戏
    //			1 -- 是否删除角色
    //			2 -- 是否更换帐号
    //			3 -- 是否重新选择角色
    //			4 -- 是否开启自动打怪

    GE_GAMELOGIN_SELECT_LOGIN_SERVER,			// 选择一个login server
    //
    // 参数0, iAreaIndex   区域索引
    // 参数1, iLoginServer 区域索引

    GE_GAMELOGIN_CLEAR_ACCOUNT,					// 清空帐号显示.
    //

    GE_GAMELOGIN_SELECT_AREA,					// 清空帐号显示.
    //
    GE_GAMELOGIN_SELECT_LOGINSERVER,			// 清空帐号显示.
    //

    GE_GAMELOGIN_CREATE_CLEAR_NAME,				// 清空创建角色的名字。
    //

    GE_GAMELOGIN_CREATE_ROLE_OK,				// 创建角色成功。
    //

    GE_GAMELOGIN_Delete_ROLE_OK,				//删除角色成功



    //
    // 登录流程相关
    //
    //------------------------------------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------------------------------------
    //
    // 是否设置二级保护密码
    //

    GE_MINORPASSWORD_OPEN_SET_PASSWORD_DLG,			// 打开设置二级密码界面
    //


    GE_MINORPASSWORD_OPEN_UNLOCK_PASSWORD_DLG,		// 打开unlock密码界面。

    GE_MINORPASSWORD_OPEN_CHANGE_PASSWORD_DLG,		// 更改密码界面

    GE_OPEN_SYSTEM_TIP_INFO_DLG,					// 提示系统信息对话框。


    //
    // 是否设置二级保护密码
    //
    //------------------------------------------------------------------------------------------------------------------------

    GE_SUPERTOOLTIP,				//Tooltips
    /*
    | arg0 - 显示/隐藏 1,0
    | arg1 - 类型 "skill", "lifeability", "item", "xinfa", "pet_skill"
    | arg2, arg3 - 鼠标位置
    |
    */

    // 复活界面相关
    GE_RELIVE_SHOW,					// 显示复活界面
    // arg0 - 是否有复活的按钮
    // arg1 - 剩余时间

    GE_RELIVE_HIDE,					// 隐藏复活界面

    GE_RELIVE_REFESH_TIME,			// 刷新灵魂出窍的时间
    // 显示时间

    GE_OBJECT_CARED_EVENT,			//某逻辑对象的某些发生改变
    /*
    |  arg0 - Object的ID
    |  arg1 - 类型
    |			distance: 距离发生改变 (arg2距离)
    |			destroy:  被销毁
    |
    */
    GE_UPDATE_MAP,					// 更新地图

    GE_UPDATE_SUPERTOOLTIP,
    GE_UI_COMMAND,
    GE_OTHERPLAYER_UPDATE_EQUIP,

    GE_VARIABLE_CHANGED,			//某个全局变量发生改变， 
    /*
    | arg0 - 变量名
    | arg1 - 新的变量值
    */

    GE_TIME_UPDATE,					//由时间系统定时触发的事件			
    GE_FRIEND_INFO,					// 打开好友详细信息

    GE_UPDATE_TARGETPET_PAGE,		//打开targetpet ui
    GE_UPDATE_PETSKILLSTUDY,		//更新宠物技能学习界面

    GE_UPDATE_PETINVITEFRIEND,		//更新宠物征友板界面
    GE_REPLY_MISSION_PET,			//宠物刷新

    GE_GUILD_CREATE,				//帮会创建事件
    GE_GUILD_CREATE_CONFIRM,		//帮会创建确认事件
    GE_GUILD_SHOW_LIST,				//显示帮会列表事件
    GE_GUILD_SHOW_MEMBERINFO,		//显示自己帮会成员管理界面
    GE_GUILD_UPDATE_MEMBERINFO,		//更新帮会成员的信息
    GE_GUILD_SHOW_DETAILINFO,		//显示自己帮会详细信息界面
    GE_GUILD_SHOW_APPOINTPOS,		//显示自己帮会可用职位界面
    GE_GUILD_DESTORY_CONFIRM,		//删除帮会确认事件
    GE_GUILD_QUIT_CONFIRM,			//退出帮会确认事件
    GE_GUILD_FORCE_CLOSE,			//强制帮会相关界面关闭事件

    GE_CLOSE_MISSION_REPLY,			//关闭任务提交UI
    GE_CLOSE_TARGET_EQUIP,			//关闭查看对方角色属性UI
    GE_VIEW_RESOLUTION_CHANGED,		//屏幕分辨率变化时的通知消息
    GE_CLOSE_SYNTHESIZE_ENCHASE,	//关闭合成UI和镶嵌UI
    GE_TOGGLE_PETLIST,				//打开和关闭宠物列表
    GE_PET_FREE_CONFIRM,			//放生宠物确认

    GE_TOGLE_CONTAINER1,				// 打开/关闭包裹  //fujia 2007.10.24
    GE_TOGLE_CONTAINER2,				// 打开/关闭包裹
    GE_CHAT_OPEN,                       //聊天框得到焦点  fujia 2007.12.29
    GE_CHAT_CLOSED,
    GE_NEW_GONGGAOMESSAGE,			//系统滚屏消息
    GE_MYSELF_ENTER_FIGHTSTATE,		// 进入战斗状态 [8/13/2010 Sun]
    GE_MYSELF_LEAVE_FIGHTSTATE,		// 脱离战斗状态 [8/13/2010 Sun]

    GE_TOGLE_ZHENFA_SKILL_PAGE,     // 打开/关闭阵法界面 [9/29/2010 Sun]
    GE_UPDATE_ZHANFA_SKILL,         // 更新阵法 [9/29/2010 Sun]

    GE_TOGLE_LIFE_PAGE,             // temp fix 开启生活技能 [10/26/2010 Sun]
    GE_TOGLE_SKILL_BOOK,			// temp fix 开启门派技能 [10/26/2010 Sun]
    GE_UPDATE_STILETTO,				// temp fix  [11/17/2010 Sun]
    GE_SHOW_MISSION_BY_ID,			// 弹出任务界面显示指定任务 [1/19/2011 ivan edit]

    GE_CANNOT_FIND_NEARESTTARGET,   // 找不到最近的敌对目标 [4/6/2011 Sun]
    GE_SUPERTOOLTIP2,				//Tooltips
    /*
    | arg0 - 显示/隐藏 1,0
    | arg1 - 类型 "skill", "lifeability", "item", "xinfa", "pet_skill"
    | arg2, arg3 - 鼠标位置
    |
    */
    GE_UPDATE_SUPERTOOLTIP2,
    GE_INPUT_ITEM_LINK,             // 输入物品超链接，参数为超链接字符串 [4/12/2011 Sun]

    //-------------------------------------------------------------------------------------------------------
    // 游戏内提示信息分类
    GE_INFO_GAME,					// "game" --  游戏信息
    GE_INFO_ACTIVITY,				// "activity" -- 活动信息
    GE_INFO_SELF,					// "self" -- 个人信息
    GE_INFO_INTERCOURSE,			// "intercourse" -- 交互信息
    GE_INFO_FIGHT,					// "fight" -- 战斗信息
    GE_INFO_ERROR,					// "error" -- 错误信息

    GE_MISSION_FINISH,				// 任务完成标志 [6/16/2011 ivan edit]
    GE_ENABLE_MISSION_UPDATE,		// 刷新可接任务列表 [6/16/2011 ivan edit]

    //--------------------------------------------------------------------------------------------------------
    // 好友事件分类 [6/21/2011 edit by ZL]
    GE_FRIEND_MSG_CONFIRM,			// 好友界面确认
    /*
    | arg0 - 显示确认事件类型 
    | arg1 - 参数一（好友人名或群名）
    | arg2 - 参数二（好友人名或群名）

        --1 你确定要将XXX从黑名单中删除，并将其加入好友列表？
        --2 你确定要将XXX从仇人中删除，并将其加入好友列表？
        --3 你确定要将XXX从好友中删除，并将其加入黑名单？
        --4 你确定要将XXX从临时好友中删除，并将其加入黑名单？
        --5 你确定要将XXX从你的好友列表中删除？
        --6 你确定要将XXX从你的临时好友中删除？
        --7 你确定要将XXX从临时好友中删除，并将其加入好友列表？
        --8 你确定要将XXX从黑名单中删除？
        --9 你确定要将XXX从群中踢出吗？
        --10 你确定要将XXX提升为管理员吗？
        --11 你确定要将XXX贬为普通群众吗？
        --12 你确定要将XXX提升为群主吗？
        --200 你确定要离开群XXX？
        --201 XXX已加你为好友，是否将对方加为好友？
        --202 XXX邀请你加入群XXX，是否加入？

    */
    GE_MOOD_SET,					// 设置签名界面开关
    GE_MOOD_CHANGE,					// 签名改变了
    GE_FRIEND_ADD_FRIEND,			// 增加好友界面开关
    GE_FRIEND_SEARCH_FRIEND,		// 查找好友界面开关
    GE_FRIEND_SET_GROUP,			// 设置分组界面开关
    GE_FRIEND_CREATE_GROUP,			// 创建分组界面开关
    GE_FRIEND_FRIEND_DETAIL,		// 好友详细界面开关
    GE_FRIEND_MSG_MANAGER,			// 好友消息记录开关开关
    GE_FRIEND_MSG_SEND,				// 好友消息发送界面开关
    GE_FRIEND_SET_NICK,				// 好友备注设置
    GE_FRIEND_SHOW_TIP,				// 显示好友TIP
    GE_FRIEND_HIDE_TIP,				// 关闭好友TIP
    GE_ADD_SLOT_SUCCESS,			// 打孔成功 [7/15/2011 ivan edit]
    GE_UPDATE_SPLITGEM,				// 宝石摘除 [7/16/2011 ivan edit]
    GE_REMOVE_GEM_SUCCESS,			// 宝石摘除成功 [7/16/2011 ivan edit]
    GE_STENGTHEN_UPDATE,			// 更新强化界面 [7/18/2011 ivan edit]
    GE_STENGTHEN_SUCCESS,			// 强化成功 [7/18/2011 ivan edit]
    GE_CAMERAANIMATION_START,		// 摄像机动画开始 [8/4/2011 Sun]
    GE_CAMERAANIMATION_END,			// 动画结束 [8/4/2011 Sun]
    GE_OPEN_AMBIT,					// 打开境界任务界面 [8/10/2011 edit by ZL]
    GE_UPDATE_ALL_TITLE,			// 刷新所有玩家称号 [8/9/2011 ivan edit]
    GE_USER_FIRST_LOGIN,			// 角色创建人物后首次登陆
    GE_TRACE_MISSION,				// 开始追踪一个任务 [8/12/2011 Sun]
    GE_DROP_ITEMBOX,				// 物品掉落
    GE_CLOSE_EQUIP,					// 关闭装备界面 [8/15/2011 Sun]
    GE_GET_NEWEQUIP,				// 得到新的装备 [8/15/2011 Sun]
    GE_GET_NEWAMBITMISSION,			// 能接到新的境界任务 [8/18/2011 edit by ZL]
    GE_AMBITMISSION_FINISHED,		// 境界任务完成 [8/18/2011 edit by ZL]
    GE_TEAM_RULER_CHANGE,			// 队伍分配方式改变 [8/24/2011 edit by ZL]
    GE_WINDOW_POSITION_CHANGED,		// 通知其他界面某个控件位置改变
    // arg0-----窗体名字
    // arg1-----x
    // arg2-----y [8/29/2011 Sun]
    GE_AUTO_RUNING_CHANGE,			// 自动寻路相关 [8/31/2011 edit by ZL]
    // arg0----（0:取消自动寻路，1:开始自动寻路）
    GE_NEW_ITEM,					// 获取新的道具 [8/31/2011 Sun]
    GE_MESSAGEBOX,					// messagebox专用 [9/16/2011 Ivan edit]
    // arg0 确定后回调函数
    // arg1 取消后回调函数
    // arg2 messageBox Type eg:okcancel
    GE_ASK_ENCHASE_MSG,				// 询问镶嵌消息 [9/16/2011 edit by ZL]
    GE_YES_ENCHASE_MSG,				// 确定镶嵌消息 [9/16/2011 edit by ZL]
    GE_UPDATE_EPUIP_QUALITY_UP,		// 装备升品消息 [9/19/2011 edit by ZL]
    GE_UPDATE_EPUIP_LEVEL_UP,		// 确定升级消息 [9/19/2011 edit by ZL]
    GE_UPDATE_EPUIP_REPRODUCE,		// 装备重铸消息 [10/9/2011 edit by ZL]
    GE_UPDATE_EPUIP_PRINTSOUL,		// 装备魂印消息 [10/9/2011 edit by ZL]

    GE_TOGLE_ACTIVITYDETAIL,       // 活动详情界面 [10/31/2011 edit by sqy]

    GE_PLAYVIDEO_START,				// 播放视频开始 [11/8/2011 Sun]
    GE_PLAYVIDEO_STOP,				// 结束视频播放 [11/8/2011 Sun]

    GE_BUFF_UPDATE,					// 更新buff [11/10/2011 Ivan edit]
    GE_NEW_SKILL,					// 学会新技能 [11/15/2011 Ivan edit]
    GE_UPDATE_CDTIME,               // 刷新cd时间 [2/21/2012 SUN]
    GE_TOGGLE_ATTRIRANDOM,          // 属性培养打开 [2/22/2012 SUN]
    GE_UPDATE_ATTRIRANDOM,          // 刷新培养属性 [2/24/2012 Ivan]

	GE_TOGGLE_EQUIPWINDOW,          //暂时测试用[2/22/2012 shen]
    GE_EQUIP_MOUNT,                 // 可以使用骑术 [2/24/2012 SUN]
	GE_ROLE_TIPWINDOW,              //打开角色信息界面[3/9/2012]
    GE_TOGGLE_PETEQUIPLEVELUP,       //打开宠物装备生档界面[5/9/2012 shens]

    GE_ADD_MISSION,             // 新增一个任务 [3/20/2012 SUN]
    GE_UPDATE_CAMPAIGN_TEAMINFO, // 刷新活动队伍列表 [3/23/2012 SUN]

    GE_NEW_TALISMANITEM,
    GE_OPEN_TALISMANITEM,
    GE_UPDATE_TALISMANITEM,
    GE_PACKAGE_TALISMANITEM_CHANGED,
    GE_PACKAGE_TALISMANEQUIPT_CHANGED,
    GE_COMPOUND_TALISMANITEM_RESULT,
    GE_TOGGLE_FABAOWINDOW,

    GE_TOGLE_CHARMWINDOW,
    GE_UI_INFOS,
    GE_UPDATE_PRESCR, // 学会配方 [4/9/2012 SUN]
    GE_SHORTKEY,                // 快捷键 [4/11/2012 Ivan]
	
	GE_ROLE_TIPWINDOWSHOWN = 2000,
	//从2000开始为UI打开以后触发的事件
	GE_ATTRIRANDOMSHOWN,
	GE_TOGGLE_STONE_COMBINE,        //打开宝石合成界面[3/27/2012 shen]
	GE_TOGGLE_STONE_ENCHASE,        //打开宝石镶嵌界面[3/27/2012 shen]
	GE_TOGGLE_EQUIP_SHENGDANG,        //打开装备升档界面[3/27/2012 shen]
	GE_TOGGLE_EQUIP_ENHANCE,        //打开装备强化界面[3/27/2012 shen]
    GE_MISSION_TUTORIALTIPHIDE,     //自动做任务时新手引导显示
	GE_TOGGLE_XINFASHOW,          //打开心法界面[5/14/2012 shen]

    GE_NEW_PET,                 //获得新宠物
    GE_PLAY_DIALOGUE,           // 播放剧情 [4/24/2012 Ivan]
    GE_STOP_DIALOGUE,
    GE_UPDATE_PETEQUIP,//更新宠物装备
    GE_OTHERPLAYER_UPDATE_PETEQUIP,//更新其他玩家宠物装备
	GE_OPEN_FUBEN,   //打开副本界面[5/3/2012 shen]
    GE_FUNC_OPEN,   // 系统功能开放提示 [5/16/2012 Ivan]
};
