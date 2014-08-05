public enum HUMAN_EQUIP
{
	HEQUIP_WEAPON		=0,		//武器	WEAPON
	HEQUIP_CAP			=1,		//帽子	DEFENCE
	HEQUIP_ARMOR		=2,		//盔甲	DEFENCE
	HEQUIP_CUFF			=3,		//手套	DEFENCE
	HEQUIP_BOOT			=4,		//鞋	DEFENCE
	HEQUIP_SASH			=5,		//腰带	ADORN, //// 肩胛 [4/25/2011 Sun]
	HEQUIP_RING			=6,		//戒子	ADORN
	HEQUIP_NECKLACE		=7,		//项链	ADORN
	HEQUIP_RIDER		=8,		//骑乘	ADORN
	HEQUIP_BACK			=9,		//背饰  DEFENCE
	HEQUIP_FABAO		= 10,	//// HEQUIP_UNKNOW2		=10, [9/9/2011 zzh*]
	HEQUIP_RING2		=11,	//戒指2
	HEQUIP_CHARM		=12,    //护符
	HEQUIP_JADE			=13,	// 玉佩
	HEQUIP_WRIST		=14,	//护腕
	HEQUIP_SHOULDER		=15,	//护肩
	HEQUIP_UNKNOW8		=16,	//时装
	HEQUIP_UNKNOW9		=17,
	HEQUIP_UNKNOW10		=18,
	HEQUIP_UNKNOW11		=19,
	HEQUIP_TOTAL		=20,
	HEQUIP_NUMBER		=18,		// 主角装备点总数 // [2010-12-7] by: cfp+ 修改大小
}

public enum ITEM_ATTRIBUTE
{
    IATTRIBUTE_BASE_ATTACK_P,	//基础物理攻击
    IATTRIBUTE_BASE_ATTACK_M,	//基础魔法攻击
    IATTRIBUTE_BASE_DEFENCE_P,	//基础物理防御
    IATTRIBUTE_BASE_DEFENCE_M,	//基础魔法防御
    //IATTRIBUTE_BASE_HIT,		//基础命中 [2010-11-04] cfp+
    IATTRIBUTE_BASE_MAXHP,		// 基础最大血量 [3/29/2012 Adomi]
    IATTRIBUTE_BASE_MISS,		//基础闪避

    IATTRIBUTE_POINT_MAXHP,		//按点数增加HP的上限
    IATTRIBUTE_POINT_MAXMP,		//按点数增加MP的上限

    IATTRIBUTE_COLD_ATTACK = 8,		//// 保留
    IATTRIBUTE_POISON_ATTACK = 9,		//// 保留
    IATTRIBUTE_WATER_ATTACK = 10,
    IATTRIBUTE_FIRE_ATTACK = 11,


    IATTRIBUTE_EARTH_ATTACK = 12,

    IATTRIBUTE_ATTACK_P = 13,	//物理攻击
    IATTRIBUTE_DEFENCE_P,	//物理防御

    IATTRIBUTE_ATTACK_M = 15,	//魔法攻击
    IATTRIBUTE_DEFENCE_M,	//魔法防御

    IATTRIBUTE_HIT,	//命中
    IATTRIBUTE_MISS,	//闪避
    IATTRIBUTE_2ATTACK_RATE,	//会心一击（双倍攻击）的百分比
    IATTRIBUTE_2ATTACK_DEFENCE = 20,	//会心一击（双倍攻击）的抗

    //// 以下为魂印属性
    IATTRIBUTE_NO_P_DEFENCE_RATE,	//无视对方武防
    IATTRIBUTE_NO_M_DEFENCE_RATE,	//无视对方仙防

    IATTRIBUTE_DAMAGE_RET,	//伤害反射
    IATTRIBUTE_DAMAGE_SCALE,	//伤害增幅

    IATTRIBUTE_IMMUNITY_P = 25,	//武力免伤
    IATTRIBUTE_IMMUNITY_M,	//仙术免伤

    IATTRIBUTE_RESIST_ALL,	// 增加五行全抗性数值

    IATTRIBUTE_NUMBER, //物品属性类型总数
};

public enum ITEM_CLASS
{
    ICLASS_EQUIP = 1,	//武器WEAPON、防具DEFENCE、饰物ADORN
    ICLASS_MATERIAL = 2,	//原料
    ICLASS_COMITEM = 3,	//普通物品（如：药品）
    ICLASS_TASKITEM = 4,	//任务物品
    ICLASS_GEM = 5, //宝石
    ICLASS_STOREMAP = 6,	//藏宝图
    ICLASS_TALISMAN = 7,	//法宝---???
    ICLASS_GUILDITEM = 8,	//帮会物品
    ICLASS_SYMBOLITEM = 9,  //符印
    ICLASS_NUMBER, //物品的类别数量
};
// 目标类型
public enum ENUM_ITEM_TARGET_TYPE
{
	ITEM_TARGET_TYPE_INVALID	= -1,
	ITEM_TARGET_TYPE_NONE,				// 无需目标		:	无
	ITEM_TARGET_TYPE_POS,				// 位置			:	TargetPos
	ITEM_TARGET_TYPE_DIR,				// 方向			:	TargetDir
	ITEM_TARGET_TYPE_ITEM,				// 道具			:	TargetItemIndex
	ITEM_TARGET_TYPE_SELF,				// 自已			:	TargetObj
	ITEM_TARGET_TYPE_SELF_PET,			// 自已的宠物	:	TargetObj,TargetPetGUID
	ITEM_TARGET_TYPE_FRIEND,			// 友好目标		:	TargetObj
	ITEM_TARGET_TYPE_FRIEND_PLAYER,		// 友好玩家		:	TargetObj
	ITEM_TARGET_TYPE_FRIEND_MONSTER,	// 友好怪物		:	TargetObj
	ITEM_TARGET_TYPE_FRIEND_PET,		// 友好宠物		:	TargetObj
	ITEM_TARGET_TYPE_ENEMY,				// 敌对目标		:	TargetObj
	ITEM_TARGET_TYPE_ENEMY_PLAYER,		// 敌对玩家		:	TargetObj
	ITEM_TARGET_TYPE_ENEMY_MONSTER,		// 敌对怪物		:	TargetObj
	ITEM_TARGET_TYPE_ENEMY_PET,			// 敌对宠物		:	TargetObj
	ITEM_TARGET_TYPE_ALL_CHARACTER,		// 所有角色		:	TargetObj

	ITEM_TARGET_TYPE_NUMBERS
};
//物品信息位定义
public enum ITEM_EXT_INFO
{
    IEI_BIND_INFO = 0x00000001,	//绑定信息
    IEI_IDEN_INFO = 0x00000002, //鉴定信息
    IEI_PLOCK_INFO = 0x00000004, //二级密码已经处理
    IEI_BLUE_ATTR = 0x00000008, //是否有蓝属性
    IEL_CREATOR = 0x00000010,	//是否有创造者
};
// 武器类型 [9/8/2011 Sun]
public enum WEAPON_TYPE
{
	WTYPE_DAO			=0,	//大刀
	WTYPE_QIANG			=1,	//长枪
	WTYPE_BOW			=2, //弓箭
	WTYPE_1DUAN			=3,	//单短
	WTYPE_STAFF			=4, //法杖
	WTYPE_2DUAN			=5,	//双短

	WTYPE_NUMBER, //武器种类总数 WEAPON
};

// 宠物装备点类型 [4/27/2012 Adomi]
public enum PET_EQUIP
{
    PEQUIP_INVALID = -1,
    PEQUIP_CLAW = 0,    //利爪
    PEQUIP_SPUR = 1,    //骨刺
    PEQUIP_HORN = 2,    //头角
    PEQUIP_KNOCKER = 3,    //兽环
    PEQUIP_TRUNK = 4,    //躯干
    PEQUIP_VEINS = 5,    //兽纹
    PEQUIP_NUMBER,          //最大宠物装备点类型
};
//#define  WEAPON_TYPE_START WTYPE_DAO
//#define  WEAPON_TYPE_END WTYPE_NUMBER - 1

public enum ENUM_UPDATE_EQIPMENT
{
    UPDATE_EQIPMENT_WEAPON_ID = 0,	// 武器
    UPDATE_EQIPMENT_CAP_ID,				// 帽子
    UPDATE_EQIPMENT_ARMOUR_ID,			// 衣服
    UPDATE_EQIPMENT_WRIST_ID,			// 护腕
    UPDATE_EQIPMENT_FOOT_ID,			// 靴子
    UPDATE_EQIPMENT_NUMBERS
};

public enum ITEMBOX_TYPE
{
    ITYPE_DROPBOX = -1,
    ITYPE_GROWPOINT

};

//拾取规则
public enum PICK_RULER
{
    IPR_FREE_PICK,	//自由拾取
    IPR_BET_PICK,	//投塞子拾取
    IPR_TURN_PICK,	//轮流拾取

};

//分配规则
public enum BOX_DISTRIBUTE_RULER
{
    BDR_COMMON = 0,
    BDR_BOSS = 1,
    BDR_UNKNOW = 0xFFFF
};
// 道具分类编号

public struct GAMEITEMDEFINE
{
    public const int ITEM_PET_SKILL_STUDY_BEGIN		= 30402000;	//宠物技能书最小编号
    public const int ITEM_PET_SKILL_STUDY_END		= 30403000;	//宠物技能书最大编号

    public const int ITEM_PET_RETURN_BABAY_BEGIN	= 30503011;	//宠物还童丹最小编号（注意30503011本身也是合法的还童丹编号）
    public const int ITEM_PET_RETURN_BABAY_END		= 30503020;	//宠物还童丹最大编号（注意30503020本身也是合法的还童丹编号）

    public const int ITEM_DOME_MEDICINE				= 30601000;	//驯养道具 
    public const int ITEM_MEAT_MEDICINE				= 30602000;	//肉食宠粮 
    public const int ITEM_GRASS_MEDICINE			= 30603000;	//草类宠粮 
    public const int ITEM_WORM_MEDICINE				= 30604000;	//虫类宠粮 
    public const int ITEM_PADDY_MEDICINE				= 30605000;	//谷类宠粮

    public const int ITEM_PET_FEED_MEDICINE_MAX		= ITEM_PADDY_MEDICINE+1000;

    public const int ITEM_PET_ADD_LIFE_BEGIN			= ITEM_PET_FEED_MEDICINE_MAX;	//宠物延长寿命最小编号
    public const int ITEM_PET_ADD_LIFE_END			= 30607000;	//宠物延长寿命最大编号

    //跑商
    //最大官票金额
    public const int MAX_TICKET_VALUE	= 10000000;	

    //跑商官票索引(此种物品是一种特殊的任务物品)
    public const int TICKET_ITEM_INDEX	 = 40002000;

    //漕运官票索引(此种物品是一种特殊的任务物品)
    public const int TICKET_RIVER_TRANSPORTATION_INDEX	 = 30001000;

    //跑商商品索引开始
    public const int TICKET_MERCHANDISE_INDEX_BEGIN	 = 20400001;

    //跑商商品索引结尾
    public const int TICKET_MERCHANDISE_INDEX_END	 = 20400200;

    //索引是否属于跑商商品索引区间
    public bool IS_TICKET_MERCHANDISE_INDEX(int idx)	
    {
        return ((idx>=TICKET_MERCHANDISE_INDEX_BEGIN && idx<TICKET_MERCHANDISE_INDEX_END)? true : false);
    }

    //此物品是否是跑商官票
   // public bool  ITEM_IS_TICKET(pitem)	((pitem != NULL) ? ((pitem->GetItemTableIndex() == TICKET_ITEM_INDEX) ? TRUE:FALSE):FALSE)

    //身上的官票数量是否合法
   // #define	TICKET_NUM_IS_IN_LAW(phuman) ((HumanItemLogic::CalcBagItemCount(phuman,TICKET_ITEM_INDEX) == 1)? TRUE:FALSE)

    //存在跑商货物上的价格数据的start
    public const int TICKET_MERCHANDISE_ITEM_PARAM_BUY_PRICE_START	= 0;		

    //存在跑商货物上的价格数据的类型
    public const int TICKET_MERCHANDISE_ITEM_PARAM_BUY_PRICE_TYPE	= (int)ItemParamValue.IPV_INT;

    //存在跑商货物上的出售价格的start
    public const int TICKET_MERCHANDISE_ITEM_PARAM_SALE_PRICE_START	= 4	;	

    //存在跑商货物上的出售价格的类型
    public const int TICKET_MERCHANDISE_ITEM_PARAM_SALE_PRICE_TYPE	= (int)ItemParamValue.IPV_INT;	

    //存在跑商货物上的场景ID数据的start
    public const int TICKET_MERCHANDISE_ITEM_PARAM_SCENE_START	= 8;		

    //存在跑商货物上的价格数据的类型
    public const int TICKET_MERCHANDISE_ITEM_PARAM_SCENE_TYPE	= (int)ItemParamValue.IPV_SHORT;

    //存在跑商银票上的当前现金数据的start
    public const int TICKET_ITEM_PARAM_CUR_MONEY_START = 0;		

    //存在跑商银票上的当前现金数据的type
    public const int TICKET_ITEM_PARAM_CUR_MONEY_TYPE	= (int)ItemParamValue.IPV_INT;

    //存在跑商银票上的当前现金上限的start
    public const int TICKET_ITEM_PARAM_MAX_MONEY_START	= 4;		

    //存在跑商银票上的当前现金上限的type
    public const int TICKET_ITEM_PARAM_MAX_MONEY_TYPE	= (int)ItemParamValue.IPV_INT;
}