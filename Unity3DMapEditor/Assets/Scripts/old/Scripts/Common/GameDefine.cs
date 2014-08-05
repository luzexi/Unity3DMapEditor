public struct  GAMEDEFINE
{
    public const int INVALID_GUILD_ID = -1;
    public const float DEF_SERVER_ADJUST_POS_WARP_DIST = (5.0f);
    public const int MAX_TEAM_MEMBER = 5;
    public const int MAX_TEAMS_IN_GROUP = 8;
     // 角色接受的最大路径点数量
    public const int MAX_CHAR_PATH_NODE_NUMBER = 16;
    public const int MAX_TITLE_SIZE = 20;
    public const int MAX_CHAR_ABILITY_NUM = 64;
   
    //技能配方表的条目数
    public const int MAX_ABILITY_PRESCRIPTION_NUM = 4 * 256;
    //技能配方表的存档字节占用数
    public const int MAX_CHAR_PRESCRIPTION_BYTE = MAX_ABILITY_PRESCRIPTION_NUM >> 3;

    //背包的最大容量，索引为 begin<= index <end,
    public const int MAX_BAG_SIZE = 80;
    public const int EQUIP_BAG_BEGIN = 0;
    public const int EQUIP_BAG_END = 20;
    public const int MATERIAL_BAG_BEGIN = 20;
    public const int MATERIAL_BAG_END = 40;
    public const int MAX_EXTBAG_SIZE = 140;

    //第一个租赁箱起始索引
    public const int RENTBOX1_START_INDEX  = 0;
    //第二个租赁箱起始索引
    public const int RENTBOX2_START_INDEX = 20;
    //第三个租赁箱起始索引
    public const int RENTBOX3_START_INDEX  = 40;
    //第四个租赁箱起始索引
    public const int RENTBOX4_START_INDEX = 60;
    //第五个租赁箱起始索引
    public const int RENTBOX5_START_INDEX = 80;
    public const int MAX_BANK_SIZE = 100;

    public const int MAX_CHAR_SKILL_NUM = 256;
    public const int MAX_CHAR_XINFA_NUM = 128;
    public const int MAX_IMPACT_NUM = 20;
    public const int DEF_IMPACT_DIRECT_PARAM_NUMBERS = 4;
    public const int MAX_SHOP_NUM_PER_PLAYER = 2;//一个玩家最多可以拥有的玩家商店个数
    public const int MAX_FAVORITE_SHOPS = 10;

    public const int MAX_MISSION_PARAM_NUM = 8;
    // 总任务数量
    public const int MAX_MISSION_NUM = 4096;
    public const int MAX_CHAR_MISSION_FLAG_LEN = ((MAX_MISSION_NUM + 31) / 32);
    //角色所拥有任务的数量
    public const int MAX_CHAR_MISSION_NUM = 20;
    //角色所拥有任务数据
    public const int MAX_CHAR_MISSION_DATA_NUM = 256;
    //名字长度

    //用户角色名字结构 |CharacterName|@|WorldID|00|
    //					角色名字      1  3      2个零
    //注意：角色名字后面的后缀：“@WorldID”是在用户创建角色时候自动添加进的；
    //		服务器端处理时候都是使用完整名字；
    //		客户端显示的时候，如果当前角色是本世界创建，则需去掉后面的后缀；
    //用户角色名称的最大值
    public const int MAX_CHARACTER_NAME = 30;
     //任务类型
    public enum MISSION_TYPE
     {
     	MISSION_TYPE_ZHAOREN = 1,	//找人，找物
     	MISSION_TYPE_SONGXIN,		//送任务道具
     	MISSION_TYPE_SHAGUAI,		//杀怪
     	MISSION_TYPE_SHOUJI	,		//杀怪，然后得到任务物品
     	//MISSION_TYPE_XILIE		= 0x10,
     	//MISSION_TYPE_HUSONG		= 0x20,
     	//MISSION_TYPE_CANGBAOTU	= 0x40,
     	//MISSION_TYPE_BAOWEI		= 0x80,
     	//MISSION_TYPE_ANSHA		= 0x100,
     };

    //每个角色最多能看见多少种任务物品的掉落
    public const int MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM = MAX_CHAR_MISSION_NUM * 4;

    public const int MAX_ITEM_GEM = 4;
    public const int MAX_ITEM_ATTR = (6 + 4 + MAX_ITEM_GEM);

     //最大可触发脚本事件的距离(客户端)
     public const int  MAX_CLIENT_DEFAULT_EVENT_DIST = 3;
     
     //最大可触发脚本事件的距离(服务器)
     public const int  MAX_SERVER_DEFAULT_EVENT_DIST	=(MAX_CLIENT_DEFAULT_EVENT_DIST * 2);
     
     //最大可触发脚本事件的距离平方(客户端)
     public const int  MAX_CLIENT_DEFAULT_EVENT_DIST_SQ =(MAX_CLIENT_DEFAULT_EVENT_DIST * MAX_CLIENT_DEFAULT_EVENT_DIST);
     
     //最大可触发脚本事件的距离平方(服务器)
     public const int MAX_SERVER_DEFAULT_EVENT_DIST_SQ = (MAX_SERVER_DEFAULT_EVENT_DIST * MAX_SERVER_DEFAULT_EVENT_DIST);

     // 宠物最多拥有的技能数目
     public const int MAX_PET_SKILL_COUNT = 6;
     public const int HUMAN_PET_MAX_COUNT = 6;
     public const int MAX_CURRENT_PET = 3;
     public const int MAX_EXTRAINFO_LENGTH = 256;

     public const int MAX_CHAT_SIZE = 256;

     //帮会名字长度
     public const int MAX_GUILD_NAME_SIZE = 24;

    // 玩家心情档案的长度
     public const int MOOD_DATA_SIZE = 32;

    //商人可以卖的最大货物个数
     public const int  MAX_BOOTH_NUMBER	=   96;

    //商人可以回购的最大货物个数
     public const int MAX_BOOTH_SOLD_NUMBER = 5;

     //场景管理模块可以管理的最大场景数量
     public const int MAX_SCENE = 1024;

     //最大可打开ITEM BOX的距离
     public const float MAX_OPEN_ITEMBOX_DIST	=	(2.0f);
    
     //最大可打开ITEM BOX的距离平方
     public const float MAX_OPEN_ITEMBOX_DIST_SQ = (MAX_OPEN_ITEMBOX_DIST * MAX_OPEN_ITEMBOX_DIST);

     //Obj_ItemBox 最大掉落物品个数
     public const int MAX_BOXITEM_NUMBER = 10;
     
     //Obj_ItemBox 最大参与拾取距离
     public const float MAX_ITEMBOX_PICK_DIST = (20.0f);

     public const int MAX_PETPLACARD_LIST_ITEM_NUM = 2;
     public const int PET_PLACARD_ITEM_MESSAGE_SIZE = 64;
     public const int MENPAI_PETSKILLSTUDY_PETNUM = -4444;
     public const int BASE_CONTAINER_OFFSET = 0;

     public const int CHARM_ATTR_NUM = 5;//5个属性的符印
     public const int CHARM_LEVEL_NUM = 5;//每个符印5个等级
     
     //帮会名字长度
     public const int MAX_GUILD_NAME_SIZE_DB = 32;
     
     //帮会职位名字长度
     public const int MAX_GUILD_POS_NAME_SIZE = 24;
    
    // //帮会职位个数
     public const int MAX_GUILD_POS_NUM = 8;
     
     //帮会描述长度
     public const int MAX_GUILD_DESC_SIZE = 60;
     
     //帮会描述长度
     public const int MAX_GUILD_DESC_SIZE_DB = 62;
    
     //帮会列表每次得到的最大个数
     public const int MAX_GUILD_LIST_COUNT = 50;
    
     public const int MAX_PROPOSER_IN_GUILD = 10;
     public const int MAX_USER_IN_GUILD = 160;
     public const int USER_ARRAY_SIZE = (MAX_USER_IN_GUILD + MAX_PROPOSER_IN_GUILD);
    
     public const int MAX_SYSTEM_MSG_SIZE = 256;
     // 友好度上限
     public const int MAX_FRIEND_POINT = 9999;
  
     // 友好度达到上下线提示
     public const int FRIEND_POINT_ENOUGH_NOTIFY = 10;
  
     // 友好度大到需要输入二级密码
     public const int FRIEND_POINT_NEED_REMOVE_VERIFY = 500;
 
     //联系人上限（包括好友、黑名单）
     public const int MAX_RELATION_SIZE = 100;
 
     //黑名单人数上限
     public const int MAX_BLACK_SIZE = 64;

     public const int MAX_TALISMAN_SIZE_UNEQUIP   = 24;    //未装备法宝包的大小
     public const int MAX_TALISMAN_SIZE_EQUIP = 10;    //已装备法宝包的大小

     //工会上限
     public const int MAX_GUILD_SIZE = 1024;

     //最大快捷栏数量
     public const int MAX_MAINMENUBAR_NUMBER = 30;
}


// /////////////////////////////////////////////////////////////////////////////////
// //游戏基本信息宏定义
// /////////////////////////////////////////////////////////////////////////////////
// 
// //世界号最大值
// #define MAX_WORLD 255
// 
// #define MAX_WORLD_USER	3000
// 
// //用户角色名字可以输入的最大值
// #define MAX_CHARACTER_INPUTNAME 24
// 
// //GamePlayer管理器的上限
// #define MAX_PLAYER 1024
// 
// //玩家池上限
// #define MAX_POOL_SIZE 1280

// 
// //没节的邮件数
// #define MAX_MAILNODE_SIZE 102400
// 
// //用户角色名字结构 |CharacterName|@|WorldID|00|
// //					角色名字      1  3      2个零
// //注意：角色名字后面的后缀：“@WorldID”是在用户创建角色时候自动添加进的；
// //		服务器端处理时候都是使用完整名字；
// //		客户端显示的时候，如果当前角色是本世界创建，则需去掉后面的后缀；
// //用户角色名称的最大值
// #define MAX_CHARACTER_NAME 30
// 
// //// 添加有关国家的咚咚 [8/15/2011 zzh+]
// #define MAX_COUNTRY_COLLECT_DESC 68
// 
// //玩家称号长度
// #define MAX_CHARACTER_TITLE 34
// 
// //NPC（或怪物）名称长度
// #define NPC_NAME_LEN 32
// 
// //NPC（或怪物）称号长度
// #define NPC_TITLE_LEN 32
// 
// //角色的昵称
// #define MAX_NICK_NAME 34
// 
// //商店的名字
// #define MAX_SHOP_NAME 34
// 
// //摊位的名字
// #define MAX_STALL_NAME 34
// 
// //留言的数目
// #define MAX_BBS_MESSAGE_NUM 20
// 
// //留言的长度
// #define MAX_BBS_MESSAGE_LENGTH 70
// 
// //留言作者名长度
// #define MAX_BBS_MESSAGE_AUTHORLENGTH 40
// 
// //二级密码最短长度
// #define MIN_PWD 4
// 
// //二级密码的最大长度 + 1
// #define MAX_PWD 17
// 
// //用户帐号的最大值
// #define MAX_ACCOUNT 16
// 
// //用户密码的最大值
// #define MAX_PASSWORD 32
// 
// //职业信息最大值
// //#define MAX_JOBINFO_LENGTH	50
// //毕业院校信息最大值
// //#define MAX_SCHOOLINFO_LENGTH	50
// //省份信息最大值
// //#define MAX_PROVINCE_LENGTH		16
// //City信息最大值
// //#define MAX_CITY_LENGTH		16
// //Email信息最大值
// //#define MAX_EMAIL_LENGTH	50
// //心情寄语最大值
// //#define MAX_LUCKWORD_LENGTH	255
// 
// //最多能创建角色的数量
// #define MAX_CREATE_PLAYER_NUM 3
// 
// //CharacterManager中缺省的ObjID缓存大小
// #define DEFAULT_OBJ_COUNT 1024
// 
// //
// #define ZONE_SIZE	10
// 
// //
// #define MAX_SEND_TIME 1
// 
// //搜索Zone格子的范围,0=1格(仅仅包含自己在的那个格子)，1=9格;2=25格;3=49格;
// #define MAX_RADIUS 2
// 
// #define MAX_REFESH_OBJ_ZONE_RADIUS		2
// 
// #define MAX_REFESH_OBJ_ZONE_ARRAY_SIZE	((MAX_REFESH_OBJ_ZONE_RADIUS - 1) * (MAX_REFESH_OBJ_ZONE_RADIUS - 1) + 1 )
// 
// //掉落包拾取的最大距离
// #define MAX_BOX_OP_DISTANCE				3.0f
// //背包的最大容量
// #define MAX_BAG_SIZE 60
// 
// #define MAX_EXTBAG_SIZE 140
// 
// #define MAX_NPC_DISTANCE	5.0
// 
// //Container 偏移
// 
// //基本包偏移
// #define BASE_CONTAINER_OFFSET	0
// 
// //材料包偏移
// #define MAT_CONTAINER_OFFSET	20
// 
// //任务包偏移
// #define TASK_CONTAINER_OFFSET	40
// 
// //装备偏移量
// #define EQUIP_CONTAINER_OFFSET	200
// 
// //银行偏移量
// #define BANK_CONTAINER_OFFSET	220
// 
// 
// #define DB_BAG_POS_START			(BASE_CONTAINER_OFFSET)					// 0
// #define	DB_BAG_POS_END				(MAX_BAG_SIZE+MAX_EXTBAG_SIZE-1)		// 199
// 
// #define DB_EQUIP_POS_START			(MAX_BAG_SIZE+MAX_EXTBAG_SIZE)			// 200
// #define DB_EQUIP_POS_END			(DB_EQUIP_POS_START+HEQUIP_NUMBER-1)	// 208
// 
// #define DB_BANK_POS_START			(DB_EQUIP_POS_START+HEQUIP_TOTAL)		// 220
// #define DB_BANK_POS_END				(MAX_BANK_SIZE+DB_BANK_POS_START-1)		// 319
// 
// 
// #define MAX_BANK_MONEY		10000000
// 
// //错误包格表示
// #define	INVALID_BAG_INDEX (-1)
// 
// //一个场景最多可以有多少个玩家商店
// #define MAX_SHOP_NUM_PER_SCENE 512
// 
// //一个玩家最多可以拥有的玩家商店个数
// #define MAX_SHOP_NUM_PER_PLAYER 2
// 
// //商店收藏夹大小
// #define MAX_FAVORITE_SHOPS		10
// 
// //一个玩家商店的柜台数
// #define MAX_STALL_NUM_PER_SHOP 10
// 
// //一个玩家商店的合作伙伴数
// #define MAX_PARTNER_PER_SHOP 6
// 
// //交易记录最大数量
// #define MAX_EXCHANGE_RECORD 40
// 
// //管理记录最大数量
// #define MAX_MANAGER_RECORD 128
// 
// //最大记录长度
// #define MAX_RECORD_LEN_ENTRY	128		
// 
// //最大记录数
// #define MAX_RECORD_NUM_ENTRY	128		
// 
// //每页记录数
// #define MAX_RECORD_NUM_PER_PAGE	10	
// 
// //一个玩家商店的名字长度
// #define MAX_SHOP_NAME_SIZE 12
// 
// //一个玩家商店的描述内容长度
// #define MAX_SHOP_DESC_SIZE 82
// 
// //商店的税率
// #define	SHOP_TAX 0.03
// 
// //银行的最大容量
// #define MAX_BANK_SIZE 100
// 
// //银行最多可以存的钱数
// #define MAX_BANK_MONEY 10000000
// 
// //银行最多可以存的元宝
// #define MAX_BANK_RMB 100000
// 
// //第一个租赁箱起始索引
// #define RENTBOX1_START_INDEX 0
// 
// //第二个租赁箱起始索引
// #define RENTBOX2_START_INDEX 20
// 
// //第三个租赁箱起始索引
// #define RENTBOX3_START_INDEX 40
// 
// //第四个租赁箱起始索引
// #define RENTBOX4_START_INDEX 60
// 
// //第五个租赁箱起始索引
// #define RENTBOX5_START_INDEX 80
// 
// //交易盒的最大容量
// #define	MISSION_BOX_SIZE	4
// 
// //任务列表界面物品的最大数量
// #define	QUESTLOG_BOX_SIZE	255
// 
// //交易盒的最大容量
// #define	EXCHANGE_BOX_SIZE	5
// 
// //交易盒中宠物的最大容量
// #define	EXCHANGE_PET_BOX_SIZE	5
// 
// //交易双方允许的最大相距距离
// #define EXCHANGE_MAX_DISTANCE	3
// 
// //摊位盒的最大容量
// #define	STALL_BOX_SIZE		20
// 
// //摊位盒的最大容量(一定要跟STALL_BOX_SIZE一样大！！！)
// #define	STALL_PET_BOX_SIZE		20
// 
// //缓存的申请者队列的长度
// #define	MAX_EXCHANGE_APPLICANTS_NUM	10
// 
// //称号的最大容量
// #define MAX_STRING_TITLE_SIZE 4
// 
// //动态称号的最大容量
// #define MAX_DYNAMIC_TITLE_SIZE 10
// 
// //静态态称号的最大容量
// #define MAX_STATIC_TITLE_SIZE 6
// 
// //静态态称号的最大容量
// #define MAX_TITLE_ID_SIZE 16
// 
// //称号的最大容量
// #define MAX_TITLE_SIZE 20
// 
// //物品可镶嵌宝石的最大个数
// #define MAX_ITEM_GEM 3
// 
// //最大蓝色装备随机属性个数
// #define		MAX_BLUE_ITEM_EXT_ATTR				4
// 
// //// 最大装备随机属性个数 [9/16/2011 zzh+]
// #define		MAX_ITEM_EXT_ATTR				4
// 
// //最大物品属性
// ////  [9/21/2011 zzh-]#define MAX_ITEM_ATTR (9+MAX_ITEM_GEM)
//  #define MAX_ITEM_ATTR (6+4+MAX_ITEM_GEM)
// 
// //最大强化等级 [2011-7-19] by: cfp+
// #define MAX_ITEM_ENHANCELEVEL 9
// 
// //共享内存ItemSerial固定key
// #define ITEM_SERIAL_KEY		536081
// 
// //宝石的最高级别
// #define MAX_GEM_QUALITY 9
// 
// //生活技能最大使用背包物品数
// #define MAX_ABILITY_ITEM 5
// 
// //最大装备套装属性
// #define MAX_ITEM_SET_ATTR 6             // Allan 7/1/2011 
// 
// //最大药品效果
// #define	MAX_MEDIC_EFFECT_NUM 3
// 
// #define MAX_GROW_POINT_RATE_NUM		4
// #define MAX_LOGIC_FUNC_LEN			128
// #define ITEM_LOGIC_FUNC_LEN			32
// #define MAX_SCENE_GROW_POINT_NUM	128
// 
// #define MAX_SCENE_GP_OWNER			255
// 
// // 宠物最多拥有的技能数目
// #define MAX_PET_SKILL_COUNT		(6)
// 

// 
// //商人可以卖的最大货物个数
// #define MAX_BOOTH_NUMBER	   96
// 
// //商人可以回购的最大货物个数
// #define MAX_BOOTH_SOLD_NUMBER   5
// 
// //角色最大等级 [2010-12-28] by: cfp+
// #define MAX_PLAYER_EXP_LEVEL	100 
// 
// //
// #define MAX_100_PLUS_DELTA_LEVEL	201
// 
// //
// #define DEFAULT_WASHPOINT_LEVEL		60
// 
// //最大表定义级别属性点分配
// #define MAX_TABLE_DEFINE_LEVEL	 150
// 
// //断开网络后，服务器继续保留数据的时间（毫秒）
// #define TIME_TO_TRUE_QUIT 10000
// 
// //怪物的种类数量最大值
// #define	MAXTYPE_NUMBER 1024 * 20
// 
// //怪物AI种类的数量最大值
// #define MAXAI_NUMBER 256
// 
// //NPC谈话的类型最大值
// #define MAXCHATTYPE_NUMBER 64
// 
// //NPC谈话记录的最大值
// #define MAXCHAT_NUMBER 64
// 
// //角色所拥有技能的数量
// #define MAX_CHAR_SKILL_NUM 256
// 
// //技能最大级别
// #define MAX_CHAR_SKILL_LEVEL 12
// 
// //角色身上可以拥有的附加效果最大数量
// #define MAX_IMPACT_NUM 20
// 
// //角色所拥有技能心法的数量，这里被用来描述技能等级了
// #define MAX_CHAR_XINFA_NUM 128
// 
// //角色所拥有生活技能的数量
// #define MAX_CHAR_ABILITY_NUM 64
// 
// //技能配方表的条目数
// #define MAX_ABILITY_PRESCRIPTION_NUM (4 * 256)
// 
// //技能配方表的存档字节占用数
// #define MAX_CHAR_PRESCRIPTION_BYTE (MAX_ABILITY_PRESCRIPTION_NUM>>3)
// 
// #define ABILITY_GAMBLE_NUM 5
// 
// 
// //门派数量
// #define MAX_MENPAI_NUM 9
// 
// //每个门派所拥有的技能心法的数量
// #define MAX_MENPAI_XINFA_NUM 16
// 
// //总心法数量
// //// #define MAX_ALL_XINFA_NUM	64
// #define MAX_ALL_XINFA_NUM	1024  //// 要等于MAX_SKILL_NUMBER [10/17/2011 zzh+]
// 
// //每个心法所能够达到的最大的等级
// //// #define MAX_XINFA_LEVEL_NUM	120
// #define MAX_XINFA_LEVEL_NUM	12
// 
// //每个生活技能能够达到的最大等级
// #define MAX_ABILITY_LEVEL_NUM 60
// 
// //技能最大数量
// #define MAX_SKILL_NUMBER 1024
// 
// //技能最大等级
// #define MAX_SKILL_LEVEL 12
// 
// // 总任务数量
// #define MAX_MISSION_NUM				(4096)
// #define MAX_CHAR_MISSION_FLAG_LEN	((MAX_MISSION_NUM+31)/32)
// 
// //角色所拥有任务的数量
// #define MAX_CHAR_MISSION_NUM		(20)
// enum
// {
// 	QUESTTIME_LIST_SIZE = MAX_CHAR_MISSION_NUM,
// };
// 
// //每个任务Notify数量
// #define MAX_MISSION_NOTIFY_NUM      (4)
// 
// //角色所拥有任务数据
// #define MAX_CHAR_MISSION_DATA_NUM	(256)
// 
// //任务类型
// enum
// {
// 	MISSION_TYPE_ZHAOREN = 1,	//找人，找物
// 	MISSION_TYPE_SONGXIN,		//送任务道具
// 	MISSION_TYPE_SHAGUAI,		//杀怪
// 	MISSION_TYPE_SHOUJI	,		//杀怪，然后得到任务物品
// 	//MISSION_TYPE_XILIE		= 0x10,
// 	//MISSION_TYPE_HUSONG		= 0x20,
// 	//MISSION_TYPE_CANGBAOTU	= 0x40,
// 	//MISSION_TYPE_BAOWEI		= 0x80,
// 	//MISSION_TYPE_ANSHA		= 0x100,
// };
// 
// // 每个角色最多能看见多少种任务物品的掉落
// #define MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM	(MAX_CHAR_MISSION_NUM*4)
// 
// //技能OBJ最大数量
// #define MAX_SKILLOBJ_NUMBER			1024
// 
// #define MAX_WEBSHOPINFO_NUMBER		3
// 

// 
// //最大可触发脚本事件的距离(客户端)
// #define MAX_CLIENT_DEFAULT_EVENT_DIST (3.f)
// 
// //最大可触发脚本事件的距离(服务器)
// #define MAX_SERVER_DEFAULT_EVENT_DIST	(MAX_CLIENT_DEFAULT_EVENT_DIST * 2.f)
// 
// //最大可触发脚本事件的距离平方(客户端)
// #define MAX_CLIENT_DEFAULT_EVENT_DIST_SQ (MAX_CLIENT_DEFAULT_EVENT_DIST * MAX_CLIENT_DEFAULT_EVENT_DIST)
// 
// //最大可触发脚本事件的距离平方(服务器)
// #define MAX_SERVER_DEFAULT_EVENT_DIST_SQ (MAX_SERVER_DEFAULT_EVENT_DIST * MAX_SERVER_DEFAULT_EVENT_DIST)
// 
// //
// #define MAX_ATOM_OPT_NUM 8
// 
// // 角色接受的最大路径点数量
// #define MAX_CHAR_PATH_NODE_NUMBER	(16)
// 
// //场景管理模块可以管理的最大场景数量
// #define MAX_SCENE 1024
// 
// // 一个 zone 里面可以加入的最大 Area 数量
// #define MAX_AREA_IN_ZONE		10
// 
// //一个副本可以配置的场景最大值
// #define MAX_COPYSCENE 128
// 
// 
// // 最大队伍人数现改为5 [6/14/2011 edit by ZL]
// #define MAX_TEAM_MEMBER 5
// 
// // 团队中最大队伍数量
// #define MAX_TEAMS_IN_GROUP 8
// 
// //队伍数量
// #define MAX_TEAMS 3000
// 
// //自建聊天频道内最多人数
// #define MAX_CHATCHANNEL_MEMBER 11
// 
// //频道数量
// #define MAX_CHANNELS 3000
// 
// //聊天频道 系统名字 [2011-1-18] by: cfp+
// #define SYSTEM_NAME "系统"
// 
// // 最多保存多少条离线邮件
// #define MAX_MAIL 20
// 
// //邮件内容的最长尺寸
// #define MAX_MAIL_CONTEX 256
// 
// 
// 
// //怪物泡泡说话的最大长度
// #define MAX_MONSTERSPEAK_CONTENT 64
// 
// //最小可以分配物品伤血比例
// #define MIN_DAMAGE_PERCENT	(0.2f)
// 

// 
// // 玩家心情档案的长度
// #define MOOD_DATA_SIZE 32
// 
// // 玩家心情的最大支持长度
// #define MAX_MOOD_SIZE 25
// 
// // 最大收徒个数
// #define MAX_PRENTICE_COUNT 2
// 
// // 最大师德点数
// #define MAX_MORAL_POINT 1000000
// 
// //数据库名称长度
// #define		DATABASE_STR_LEN			128
// //数据库用户名长度
// #define		DB_USE_STR_LEN				32
// //数据库密码长度
// #define		DB_PASSWORD_STR_LEN			32
// //DB 操作时间间隔
// #define		DB_OPERATION_TIME			500
// //账号保存角色个数
// #define		DB_CHAR_NUMBER				5
// //角色中非即时刷新属性同步到ShareMemory的时间间隔
// #define		DB_2_SM_TIME				60000 //60秒
// 
// //数据库连接池默认连接个数
// #define		DB_CONNECTION_COUNT			40
// 
// // 服务器能接受的客气端调整位置与服务器位的差距
// #define		DEF_SERVER_ADJUST_POS_WARP_DIST	(5.f)
// 
// // [2010-12-1] by: cfp+  玩家为一个物品（宠物）加锁需要消耗的精力
// #define		LOCK_A_OBJ_NEED_ENERGY		10
// 
// // 
// //一共有几个方面的阵营
// #define MAX_CAMP_PLAYER_NUM		3   
// #define INVALID_CAMP		-1		//无效阵营
// //#define CAMP_1				0		//第一方面的阵营
// //#define CAMP_2				1		//第二方面的阵营
// //#define CAMP_3				10		//中立类型的阵营，从10号开始
// //#define CAMP_4				20		//敌对类型的阵营，从20号开始
// 
// ////zzh- 阵营--对象类型
// ////#define CAMP1_PLAYER		0
// ////#define CAMP1_PET			1
// ////#define CAMP1_MONSTER		2
// ////#define CAMP1_NPC			3
// ////#define CAMP2_PLAYER		4
// ////#define CAMP2_PET			5
// ////#define CAMP2_MONSTER		6
// ////#define CAMP2_NPC			7
// ////#define CAMP3_MONSTER		8	//友好
// ////#define CAMP3_NPC			9
// ////#define CAMP4_MONSTER		10	//敌对
// ////#define CAMP4_NPC			11
// 
// ////  阵营类型 [12/10/2010 zzh+]
// #define CAMP_PLAYER_0		0 //// 阵营一，天机
// #define CAMP_PLAYER_1		1 //// 阵营二，逍遥
// #define CAMP_PLAYER_2		2 //// 阵营三，无极
// #define CAMP_PLAYER_3		3 //// 和2对立
// #define CAMP_PLAYER_4		4 //// 
// #define CAMP_PLAYER_5		5 //// 
// #define CAMP_PLAYER_6		6 //// 
// #define CAMP_PLAYER_7		7 //// 
// 
// #define CAMP_NPC_8		8 //// 标准
// #define CAMP_NPC_9		9 //// 自由
// #define CAMP_NPC_10		10 //// 和8对立
// #define CAMP_NPC_11		11 //// 
// #define CAMP_NPC_12		12 //// 
// #define CAMP_NPC_13		13 //// 
// #define CAMP_NPC_14		14 //// 
// #define CAMP_NPC_15		15 //// 
// 
// /////////////////////////////////////////////////////////////////////////////////
// //游戏基本数据操作宏
// /////////////////////////////////////////////////////////////////////////////////
// 
// //从GUID_t中取得世界号
// #define GETWORLD(u) ((u)/10000000)
// //从GUID_t中取得用户部分序列号
// #define GETUSER(u) ((u)%10000000)
// //判断当前是否是测试世界
// #define ISTESTWORLD(w) ((w)>200?1:0)
// //判断世界号是否合法
// #define ISVALIDWORLD(w) (((w)>0)?(((w)<MAX_WORLD)?1:0):(0))
// 
// 
// #define ABS(m) ((m)>0?(m):(m)*(-1))
// 
// #define LENGTH(x0,z0,x1,z1)  (ABS((x0)-(x1))+ABS((z0)-(z1)))
// 
// #define MAX_FILE_PATH  260
// 
// #define MAKE_COLOR(r,g,b,a)		(((r&0xFF)<<24)|((g&0xFF)<<16)|((b&0xFF)<<8)|(a&0xFF))
// #define COLOR_R(color)			((color>>24)&0x000000FF)
// #define COLOR_G(color)			((color>>16)&0x000000FF)
// #define COLOR_B(color)			((color>>8)&0x000000FF)
// #define COLOR_A(color)			((color)&0x000000FF)
// 
// #define GETHIGH(x) (x>>16)
// #define GETLOW(x)  ((x<<16)>>16)
