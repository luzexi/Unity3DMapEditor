//角色基本属性(一级属性)
public enum CHAR_ATTR_LEVEL1
{
	CATTR_LEVEL1_STR			=0, //力量 力量
	CATTR_LEVEL1_SPR			=1,	//灵气 灵力
	CATTR_LEVEL1_CON			=2,	//体制 体制
	CATTR_LEVEL1_INT			=3,	//定力 智力
	CATTR_LEVEL1_DEX			=4,	//身法 敏捷

	CATTR_LEVEL1_NUMBER, //角色基本属性总数
};



//角色二级属性
public enum CHAR_ATTR_LEVEL2
{
	CATTR_LEVEL2_ATTACKRATE_P =0,	//物理攻击
	CATTR_LEVEL2_DEFENCE_P,			//物理防御
	CATTR_LEVEL2_ATTACKRATE_M,		//魔法攻击
	CATTR_LEVEL2_DEFENCE_M,			//魔法防御
	CATTR_LEVEL2_MAXHP,				//HP上限
	CATTR_LEVEL2_MAXMP,				//MP上限
	CATTR_LEVEL2_RESTOREHP,			//HP回复
	CATTR_LEVEL2_RESTOREMP,			//MP回复
	CATTR_LEVEL2_HIT,				//命中率
	CATTR_LEVEL2_MISS,				//闪避
	CATTR_LEVEL2_CRITRATE,			//会心率
	CATTR_LEVEL2_SPEED,				//移动速度
	CATTR_LEVEL2_ATTACKSPEED,		//攻击速度
	CATTR_LEVEL2_ATTACKCOLD	,		//冰攻击
	CATTR_LEVEL2_DEFENCECOLD,		//冰防御
	CATTR_LEVEL2_ATTACKFIRE,		//火攻击
	CATTR_LEVEL2_DEFENCEFIRE,		//火防御
	CATTR_LEVEL2_ATTACKLIGHT,		//电攻击
	CATTR_LEVEL2_DEFENCELIGHT,		//电防御
	CATTR_LEVEL2_ATTACKPOISON,		//毒攻击
	CATTR_LEVEL2_DEFENCEPOISON,		//毒防御
	CATTR_LEVEL2_ATTACKEARTH,		//// 土攻击 [9/15/2011 zzh+]
	CATTR_LEVEL2_DEFENCEEARTH,		////土防御[9/15/2011 zzh+]

	CATTR_LEVEL2_NUMBER,			//二级属性数量
};



//角色三级属性
public enum CHAR_ATTR_LEVEL3
{
	CATTR_LEVEL3_NUQI			=0,	//怒气值
	CATTR_LEVEL3_SKILLPOINT		=1,	//连技点
	CATTR_LEVEL3_XIANJIN		=2,	//陷阱信息
	CATTR_LEVEL3_YINSHEN		=3,	//隐身信息

	CATTR_LEVEL3_NUMBER, //三级属性数量
};

//角色在数据库和ShareMem中对应的属性
public enum	CHAR_ATTR_DB
{
	//////////////////////////////////////////////////////////////////////////
	//最容易变化基本数据
	CATTR_DB_HP					=	0,				//生命
	CATTR_DB_MP					,					//魔法
	CATTR_DB_STRIKEPOINT		,					//连击
	CATTR_DB_VIGOR				,					// [2010-12-1] by: cfp+ 活力
	CATTR_DB_MAX_VIGOR			,					// [2010-12-1] by: cfp+ 活力上限
	CATTR_DB_VIGOR_REGENE_RATE	,					//活力恢复速度
	CATTR_DB_ENERGY				,					//精力
	CATTR_DB_MAX_ENERGY			,					//精力上限
	CATTR_DB_ENERGY_REGENE_RATE	,					//精力恢复速度
	CATTR_DB_RAGE				,					//怒气
	CATTR_DB_LEVEL				,					//等级
    CATTR_DB_AMBIT,                                 //角色境界 [2011-8-10] by: cfp+
	CATTR_DB_PKVALUE,								//杀气
	CATTR_DB_CURRENTPET,							//当前宠物GUID
	CATTR_DB_EXP,									//经验
	CATTR_DB_AT_POSITION,							//玩家位置
	CATTR_DB_BK_POSITION,							//玩家备份位置
	CATTR_DB_AT_SCENE,								//玩家场景
	CATTR_DB_BK_SCENE,								//玩家备份场景
	//////////////////////////////////////////////////////////////////////////
	//一级属性
	CATTR_DB_STR,									//力量 力量
	CATTR_DB_SPR,									//灵气 灵力
	CATTR_DB_CON,									//体制 体制
	CATTR_DB_INT,									//定力 智力
	CATTR_DB_DEX,									//身法 敏捷
	CATTR_DB_REMAINPOINT,							//剩余点数
	//////////////////////////////////////////////////////////////////////////
	//较少变化数据
	CATTR_DB_CAMP,									//阵营编号
	CATTR_DB_MENPAI,								//门派编号
	CATTR_DB_GUILDID,								//帮会ID
	CATTR_DB_TEAMID,								//队伍号
	CATTR_DB_GUID,									//角色唯一号
	CATTR_DB_PORTRAITID,							//角色头像
	CATTR_DB_NAME,									//角色名称
	CATTR_DB_SEX,									//角色性别
	CATTR_DB_CREATETIME,							//创建日期
	CATTR_DB_TITLE,									//角色称号
	CATTR_DB_PASSWORD,								//角色二级密码
	CATTR_DB_PWDDELTIME,							//设置强制解除密码的时间
	CATTR_DB_HAIR_COLOR,							//头发颜色	
	CATTR_DB_FACE_COLOR,							//脸形颜色
	CATTR_DB_HAIR_MODEL,							//头发模型
	CATTR_DB_FACE_MODEL,							//脸形模型
	CATTR_DB_ONLINETIME,							//总在线时间
	CATTR_DB_LASTLOGINTIME,							//最后一次登入时间
	CATTR_DB_LASTLOGOUTTIME,						//最后一次登出时间
	CATTR_DB_DBVERSION,								//存档版本
    CATTR_DB_HELPMASK,								//新手引导掩码 [2011-8-10] by: cfp+
	
	CATTR_DB_MONEY,									//金钱
	//CATTR_DB_GOODBAD,								//善恶值
	CATTR_DB_BANK_MONEY,							//银行金钱
	CATTR_DB_BANK_OFFSIZE,							//银行end
	CATTR_DB_RMB,									// [2010-12-1] by: cfp+ 元宝
	CATTR_DB_BANK_RMB,								// [2010-12-1] by: cfp+ 银行中的元宝
	CATTR_DB_DOUBLE_EXP_TIME,						// [2010-12-1] by: cfp+ 双倍经验时间
	CATTR_DB_GMRIGHT,								// [2010-12-1] by: cfp+ GM权限

	//////////////////////////////////////////////////////////////////////////
	//玩家商店数据
	CATTR_DB_PLAYER_SHOP_BEGIN,						//玩家商店开始的位置
	CATTR_DB_PLAYER_SHOP_END	=					//玩家商店结束位置
	CATTR_DB_PLAYER_SHOP_BEGIN+GAMEDEFINE.MAX_SHOP_NUM_PER_PLAYER-1,
	CATTR_DB_PLAYER_SHOP_FAVORITE_BEGIN,			//玩家商店收藏夹起始
	CATTR_DB_PLAYER_SHOP_FAVORITE_END =
    CATTR_DB_PLAYER_SHOP_FAVORITE_BEGIN + GAMEDEFINE.MAX_FAVORITE_SHOPS,//玩家商店收藏夹结束

	//////////////////////////////////////////////////////////////////////////
	//背包数据
    CATTR_DB_BAG_BEGIN,								//背包开始位置
	CATTR_DB_BAG_END			=					//背包结束位置
    CATTR_DB_BAG_BEGIN + GAMEDEFINE.MAX_BAG_SIZE + GAMEDEFINE.MAX_EXTBAG_SIZE - 1,
	//////////////////////////////////////////////////////////////////////////
	//装备数据
	CATTR_DB_EQUIP_BEGIN,							//装备开始位置
	CATTR_DB_EQUIP_END		=						//装备结束位置
    CATTR_DB_EQUIP_BEGIN + HUMAN_EQUIP.HEQUIP_NUMBER - 1,	
	//////////////////////////////////////////////////////////////////////////
	//银行数据
	CATTR_DB_BANK_BEGIN,							//银行开始位置
	CATTR_DB_BANK_END			=					//银行结束位置
	CATTR_DB_BANK_BEGIN+GAMEDEFINE.MAX_BANK_SIZE-1,	
	//////////////////////////////////////////////////////////////////////////
	//技能数据
	CATTR_DB_SKILL,									//技能
	//////////////////////////////////////////////////////////////////////////
	//CoolDown数据
	CATTR_DB_COOLDOWN,								//冷却时间
	CATTR_DB_XINFA,									//心法数据
	CATTR_DB_IMPACT,								//效果数据
	CATTR_DB_ABILITY,								//生活技能
	CATTR_DB_MISSION,								//任务数据
	CATTR_DB_SETTING,								//角色设置数据
	CATTR_DB_PET,									//宠物数据
	CATTR_DB_RELATION,								//联系人数据（好友、黑名单等）
	CATTR_DB_PRIVATEINFO,							//私人信息
	CATTR_DB_TITLEINFO,								//称号列表
	CATTR_DB_RESERVE,								//保留信息

	CATTR_DB_NUMBER,
};

//常量: 一些角色属性的最大值
public enum CHAR_ATTR_CONSTANT1
{
	MAX_STRIKE_POINT = 9,
	BASE_MAX_STRIKE_POINT = 6,
	BASE_MAX_RAGE = 1000,
	MAX_RAGE = 1500,
	BASE_ATTACK_SPEED = 100,
	BASE_VISION_RANGE = 16,
	MAX_EXP_REFIX_IN_CHAR = 400,
	MAX_EXP_REFIX_IN_SYSTEM = 400,
	MAX_COOLDOWN_LIST_SIZE_FOR_HUMAN = 32,
	MAX_COOLDOWN_LIST_SIZE_FOR_PET = 8,
};
//常量: 一些角色属性的最大值
public enum CHAR_ATTR_CONSTANT2
{
	MAX_EFFECTIVE_RESIST = 100,
	LL_GENERAL_ATTACK_DEFENCE = 1,
	LL_TRAIT_ATTACK = 0,
	LL_RESIST = -200,
	UL_RESIST = 200,
};
public enum CHAR_ATTR_CONSTANT3
{
	LL_CRITICAL = 0,
	LL_HIT_MISS = 1,
	UL_CRITICAL = 100,
	UL_HIT_MISS = 1000,
};

//宠物属性相关
public enum ENUM_PET_ATTR
{
	PET_ATTR_TAKELEVEL = 0,
	PET_ATTR_PETTYPE,
	PET_ATTR_FOODTYPE,
	PET_ATTR_LIFE,
	PET_ATTR_STRPERCEPTION,
	PET_ATTR_CONPERCEPTION,
	PET_ATTR_DEXPERCEPTION,
	PET_ATTR_SPRPERCEPTION,
	PET_ATTR_INTPERCEPTION,
	PET_ATTR_GROWRATE1,
	PET_ATTR_GROWRATE2,
	PET_ATTR_GROWRATE3,
	PET_ATTR_GROWRATE4,
	PET_ATTR_GROWRATE5,

	PET_ATTR_NUM,
};
//常量: 一些宠物属性的最大值
public enum PET_ATTR_CONSTANT1
{
	PET_BASE_MAX_HAPPINESS = 100,
};
public class GAMEDEFEIN_ATTR
{
    // 表情状态
    public const int INVALID_MOOD_STATE = -1;// 无效的表情状态
    public const int MAX_IMPACT_LEVEL = 12;//心法level/10
    public const int STRIKE_POINT_SEGMENT_SIZE = 1;//常量: 连击点段的连击点数目
    public const int MAX_STRIKE_POINT_SEGMENT = (int)CHAR_ATTR_CONSTANT1.MAX_STRIKE_POINT/(int)STRIKE_POINT_SEGMENT_SIZE;
    public const int HUMAN_DB_ATTR_FLAG_LENGTH = (int)(((int)(CHAR_ATTR_DB.CATTR_DB_NUMBER)>>3)+1);
    public static int Attr_VerifyCritical(int nAttr)
    {
	    if((int)CHAR_ATTR_CONSTANT3.LL_CRITICAL > nAttr)
	    {
		    nAttr = (int)CHAR_ATTR_CONSTANT3.LL_CRITICAL;
	    }
    // 当前数值不再限制最大值 [11/15/2011 edit by ZL]
    // 	if(UL_CRITICAL<nAttr)
    // 	{
    // 		nAttr = UL_CRITICAL;
    // 	}
	    return nAttr;
    }

    public static int Attr_VerifyHitMiss(int nAttr)
    {
	    if((int)CHAR_ATTR_CONSTANT3.LL_HIT_MISS>nAttr)
	    {
		    nAttr = (int)CHAR_ATTR_CONSTANT3.LL_HIT_MISS;
	    }
    // 当前数值不再限制最大值 [11/15/2011 edit by ZL]
    // 	if(UL_HIT_MISS<nAttr)
    // 	{
    // 		nAttr = UL_HIT_MISS;
    // 	}
	    return nAttr;
    }

    public static int Attr_VerifyResist(int nAttr)
    {
	    if((int)CHAR_ATTR_CONSTANT2.LL_RESIST>nAttr)
	    {
		    nAttr = (int)CHAR_ATTR_CONSTANT2.LL_RESIST;
	    }
    // 当前数值不再限制最大值 [11/15/2011 edit by ZL]
    // 	if(UL_RESIST<nAttr)
    // 	{
    // 		nAttr = UL_RESIST;
    // 	}
	    return nAttr;
    }

    //工具函数，校验属性的有效值
    public static int Attr_VerifyGeneralAttack(int nAttr)
    {
	    if((int)CHAR_ATTR_CONSTANT2.LL_GENERAL_ATTACK_DEFENCE>nAttr)
	    {
		    nAttr = (int)CHAR_ATTR_CONSTANT2.LL_GENERAL_ATTACK_DEFENCE;
	    }
	    return nAttr;
    }
    public static int Attr_VerifyTraitAttack(int nAttr)
    {
	    if((int)CHAR_ATTR_CONSTANT2.LL_TRAIT_ATTACK>nAttr)
	    {
		    nAttr = (int)CHAR_ATTR_CONSTANT2.LL_TRAIT_ATTACK;
	    }
	    return nAttr;
    }
    public static int Attr_VerifyDefence(int nAttr)
    {
	    if((int)CHAR_ATTR_CONSTANT2.LL_GENERAL_ATTACK_DEFENCE>nAttr)
	    {
		    nAttr = (int)CHAR_ATTR_CONSTANT2.LL_GENERAL_ATTACK_DEFENCE;
	    }
	    return nAttr;
    }

}

