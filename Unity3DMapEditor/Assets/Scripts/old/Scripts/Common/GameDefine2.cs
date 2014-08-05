//门派类别
public enum MENPAI_ATTRIBUTE
{
	MATTRIBUTE_SHAOLIN			=0,	//少林-战士
	MATTRIBUTE_MINGJIAO			=1,	//明教
	MATTRIBUTE_GAIBANG			=2,	//丐帮
	MATTRIBUTE_WUDANG			=3,	//武当-法师
	MATTRIBUTE_EMEI				=4,	//峨嵋-道士
	MATTRIBUTE_XINGXIU			=5,	//星宿
	MATTRIBUTE_DALI				=6,	//大理（天龙寺）
	MATTRIBUTE_TIANSHAN			=7,	//天山
	MATTRIBUTE_XIAOYAO			=8,	//逍遥
	MATTRIBUTE_WUMENPAI			=9,
	MATTRIBUTE_NUMBER,				//门派总数
}

// 属性攻击类型 [11/14/2011 edit by ZL]
public enum EXTEND_ATTACK_TYPE
{
	ATTACKTYPE_JIN,
	ATTACKTYPE_MU,
	ATTACKTYPE_SHUI,
	ATTACKTYPE_HUO,
	ATTACKTYPE_TU,
	ATTACKTYPE_NUMBER,		//属性攻击类型
}

// 属性修正类型 [11/14/2011 edit by ZL]
public enum RECTIFY_ATTR_TYPE
{
	RECTIFY_ATTR_HIT,			//命中	
	RECTIFY_ATTR_MISS,			//闪避
	RECTIFY_ATTR_CRITICAL,		//暴击
	RECTIFY_ATTR_DECRITICAL,	//抗暴
	RECTIFY_ATTR_DEFENCE,		//抗性
	RECTIFY_ATTR_DECDEFENCE,	//减抗
	RECTIFY_ATTR_NUMBER,		//属性攻击类型
}

// 关系
public enum ENUM_RELATION
{
	RELATION_INVALID	= -1,
	RELATION_ENEMY,			// 敌对
	RELATION_FRIEND,			// 友好

	RELATION_NUMBERS
}

public enum ENUM_NPC_AI_ATTR
{
	NPC_AI_TYPE_INVALID			= -1,	// INVALID
	NPC_AI_TYPE_SCANNPC			= 0,	// 主动攻击，会随机移动，可以攻击（巡逻），会还击
	NPC_AI_TYPE_NOTSCANNPC		= 1,	// 不主动攻击，会随机移动，可以攻击，会还击
	NPC_AI_TYPE_CANNOTATTACK	= 2,	// 不主动攻击，会随机移动，不可以攻击，不会还击
	NPC_AI_TYPE_IS_3			= 3,	//不主动攻击，不随机移动，不可以攻击，不会还击
	NPC_AI_TYPE_IS_4			= 4,	//主动攻击，会随机移动，可以攻击，会还击（不巡逻）
	NPC_AI_TYPE_IS_5			= 5,	//主动攻击，不会随机移动，可以攻击，会还击
	NPC_AI_TYPE_IS_6			= 6,	//不主动攻击，不会随机移动，可以攻击，会还击
	NPC_AI_TYPE_IS_7			= 7,	//不主动攻击，不会随机移动，可以攻击，不会还击
	NPC_AI_TYPE_IS_8			= 8,	//不主动攻击，会随机移动，可以攻击，不会还击
	NPC_AI_TYPE_IS_9			= 9,	//主动攻击，不会随机移动，可以攻击，会还击（副本专用，小扫敌半径）
}


//属性信息类别
public enum ATTRIBUTEINFO_TYPE
{
	AINFOTYPE_BASE_HP			=0,		//初始生命
	AINFOTYPE_CON_HP			=1,		//体制对生命影响系数
	AINFOTYPE_LEVEL_HP			=2,		//等级对生命影响系数
	AINFOTYPE_BASE_HPRESTORE	=3,		//初始生命回复
	AINFOTYPE_CON_HPRESTORE		=4,		//体制对生命回复影响系数
	AINFOTYPE_LEVEL_HPRESTORE	=5,		//等级对生命回复影响系数
	AINFOTYPE_BASE_MP			=6,		//初始内力
	AINFOTYPE_INT_MP			=7,		//定力对内力影响系数
	AINFOTYPE_LEVEL_MP			=8,		//等级对内力影响系数
	AINFOTYPE_BASE_MPRESTORE	=9,		//初始内力回复
	AINFOTYPE_INT_MPRESTORE		=10,	//定力对内力回复影响系数
	AINFOTYPE_LEVEL_MPRESTORE	=11,	//等级对内力回复影响系数
	AINFOTYPE_BASE_ATTACK_P		=12,	//初始物理攻击
	AINFOTYPE_STR_ATTACK_P		=13,	//力量对物理攻击的影响系数
	AINFOTYPE_LEVEL_ATTACK_P	=14,	//等级对物理攻击的影响系数
	AINFOTYPE_BASE_ATTACK_M		=15,	//初始魔法攻击
	AINFOTYPE_SPR_ATTACK_M		=16,	//灵力对魔法攻击的影响系数
	AINFOTYPE_LEVEL_ATTACK_M	=17,	//等级对魔法攻击的影响系数
	AINFOTYPE_BASE_DEFENCE_P	=18,	//初始物理防御
	AINFOTYPE_CON_DEFENCE_P		=19,	//体制对物理防御的影响系数
	AINFOTYPE_LEVEL_DEFENCE_P	=20,	//等级对物理防御的影响系数
	AINFOTYPE_BASE_DEFENCE_M	=21,	//初始魔法防御
	AINFOTYPE_INT_DEFENCE_M		=22,	//定力对魔法防御的影响系数
	AINFOTYPE_LEVEL_DEFENCE_M	=23,	//等级对魔法防御的影响系数
	AINFOTYPE_BASE_HIT			=24,	//初始命中
	AINFOTYPE_DEX_HIT			=25,	//身法对命中的影响系数
	AINFOTYPE_LEVEL_HIT			=26,	//等级对命中的影响系数
	AINFOTYPE_BASE_MISS			=27,	//初始闪避
	AINFOTYPE_DEX_MISS			=28,	//身法对闪避的影响系数
	AINFOTYPE_LEVEL_MISS		=29,	//等级对闪避的影响系数
	AINFOTYPE_BASE_CRITRATE		=30,	//初始会心率
	AINFOTYPE_DEX_CRITRATE		=31,	//身法对会心率的影响系数
	AINFOTYPE_LEVEL_CRITRATE	=32,	//等级对会心率的影响系数
    AINFOTYPE_BASE_DEFENCE_C    =33,    //初始会心防御                  Allan   12/14/2010
    AINFOTYPE_DEX_DEFENCE_C     =34,    //身法对会心防御的影响系数      Allan   12/14/2010
    AINFOTYPE_LEVEL_DEFENCE_C   =35,    //等级对会心防御的影响系数      Allan   12/14/2010
	AINFOTYPE_ATTACK_FLUCTUATION=36,	//攻击力浮动                    Allan   12/14/2010
    AINFOTYPE_HURT_RP           =37,    //受伤的怒气增长率，千分之怒气每一点伤害        Allan   12/14/2010
    AINFOTYPE_ATTACK_RP         =38,    //攻击的怒气增长率，千分之怒气每一点伤害
    AINFOTYPE_BASE_RP           =39,    //怒气的自然增长率，每恢复心跳恢复多少点怒气    Allan   12/14/2010
    AINFOTYPE_CAMP_ID           =40,    //势力ID                        Allan   12/14/2010
    AINFOTYPE_ATTACK_PROPERTY   =41,    //攻击特性ID                    Allan   12/14/2010
	AINFOTYPE_NUMBER,					//属性信息总数
}

public enum ATTACK_ERROR
{
	AERROR_NOTLIVE		=-1,	//目标死了，攻击无效		
	AERROR_NOTSCENE		=-2,	//目标不在当前场景
	AERROR_OUTRANGE		=-3,	//攻击者与目标距离太远
	AERROR_INVALID		=-4,	//无效目标
	AERROR_HIT			=-5,	//没有命中
	AERROR_IAMDEAD		=-6,	//攻击者是个死人
	AERROR_BREAK		=-7,	//攻击被打断
	AERROR_CANNOTATTACK =-8,	//不能攻击的目标

}

public enum ESPECIAL_ABILITY_NUMBER
{
    EAN_GEM_COMPOUNDED = GAMEDEFINE.MAX_CHAR_ABILITY_NUM + 1,	// 宝石合成
	EAN_GEM_EMBEDDED,								// 宝石镶嵌
}


public enum COMMONCOOLDOWN
{
    COOLDOWN_LIST_SIZE = 255,
	COMMONCOOLDOWN_TIME = 750,
}
public enum SKILL_DEPLETE_LOGIC
{
	DEPLETE_INVALID = -1,
	DEPLETE_MANA = 0,
	DEPLETE_RAGE,
	DEPLETE_STRIKE_POINT,
	DEPLETE_ITEM_STD,
}
public enum EFFECT_STATUS
{
	ESTATUS_COLD		=0,	//迟缓
	ESTATUS_FIRE		=1, //弱化
	ESTATUS_LIGHT		=2,	//眩晕
	ESTATUS_POISON		=3,	//中毒

	ESTATUS_NUMBER, //人物状态数量
}

public enum SKILL_ATTRIBUTE
{
	SATTRIBUTE_ATTACK_P			=0, //增加物理攻击点数
	SATTRIBUTE_RATE_ATTACK_P	=1, //增加物理攻击百分比

	SATTRIBUTE_NUMBER, //技能影响属性的数量
}

//怪物AI的元素
public enum MONSTER_AI_PRAM
{
	AIPARAM_SCANTIME			=0,	//如果此值大于0，则为主动攻击，值的含义就是扫描间隔
									//如果小于等于0，则为非主动攻击
	AIPARAM_RANDMOVETIME		=1,	//随机移动位置的时间间隔（毫秒）
	AIPARAM_CANNOTATTACK		=2,	//无法攻击的对象(已经废弃, 是否无敌是以MonsterAttrExTable.txt表中为主)
	AIPARAM_RETURN				=3, //如果此值大于0，则当怪物距离出生地大于此值时，怪物放弃追赶
	AIPARAM_SCANENEMYDIST		=4, //扫描敌人的最大距离	
	AIPARAM_SCANTEAMMATEDIST	=5, //扫描队友的最大距离	
	AIPARAM_RESETTARGET_DIST	=6, //如果当前的移动目标和敌人的位置之间的距离大于此数值
									//则需要重新设定移动目标
	AIPARAM_PATROLTIME			=7,//巡逻的时间间隔，若小于等于0，则不进行巡逻
	AIPARAM_STRIKEBACK			=8,//是否会还击

	AIPARAM_NUMBER,
}

public enum ENUM_UPDATE_CHAR_ATT
{
	UPDATE_CHAR_ATT_DATA_ID	= 0,		// 数据资源ID
	UPDATE_CHAR_ATT_NAME,				// 姓名
	UPDATE_CHAR_ATT_TITLE,				// 头衔
	UPDATE_CHAR_ATT_LEVEL,				// 等级
	UPDATE_CHAR_ATT_HP_PERCENT,			// HP百分比
	UPDATE_CHAR_ATT_MP_PERCENT,			// MP百分比
	UPDATE_CHAR_ATT_RAGE,				// 怒气
	UPDATE_CHAR_ATT_STEALTH_LEVEL,		// 隐身级别
	UPDATE_CHAR_ATT_SIT,				// 是否为坐下状态
	UPDATE_CHAR_ATT_MOVE_SPEED,			// 移动速度
	UPDATE_CHAR_ATT_ATTACK_SPEED,		// 攻击速度
	UPDATE_CHAR_ATT_CAMP_ID,			// 阵营ID
	UPDATE_CHAR_ATT_PORTRAIT_ID,		// 头像ID
	UPDATE_CHAR_ATT_MODEL_ID,			// 模型ID
	UPDATE_CHAR_ATT_MOUNT_ID,			// 座骑ID
	UPDATE_CHAR_ATT_AITYPE,				// AI类型
	UPDATE_CHAR_ATT_PLAYER_DATA,		// 00|0000|0000|0000|0000
										//     |    |    |    |  
										//    头发 脸型 头发 脸型
										//    颜色 颜色 模型 模型
	UPDATE_CHAR_IS_IN_STALL,			// 是否摆摊状态
	UPDATE_CHAR_STALL_NAME,				// 摊位名
	UPDATE_CHAR_OCCUPANT,				// 占有者(是谁打的，死了算谁的)
	UPDATE_CHAR_OWNER,					// 拥有者(是谁控制的)
	UPDATE_CHAR_ISNPC,					// 是否是NPC


	UPDATE_CHAR_ATT_NUMBERS //基础信息数量,(目前不能超过32)
}

public enum ENUM_DETAIL_ATTRIB
{
	DETAIL_ATTRIB_LEVEL		= 0,		//等级
    DETAIL_ATTRIB_AMBIT,		        //角色境界 [2011-8-10] by: cfp+
	DETAIL_ATTRIB_HP,					//生命值
	DETAIL_ATTRIB_MP,					//魔法值
	DETAIL_ATTRIB_EXP,					//经验
	DETAIL_ATTRIB_MONEY,				//货币

	DETAIL_ATTRIB_STR,					//力量 力量
	DETAIL_ATTRIB_SPR,					//灵气 智慧
	DETAIL_ATTRIB_CON,					//体制 体质
	DETAIL_ATTRIB_INT,					//定力 韧性
	DETAIL_ATTRIB_DEX,					//身法 敏捷

    //// 基础一级战斗属性的培养点数[02/23/2012 zzh+]
    DETAIL_ATTRIB_STR_RANDOM_POINT,					//力量 力量
    DETAIL_ATTRIB_SPR_RANDOM_POINT,					//灵气 灵力
    DETAIL_ATTRIB_CON_RANDOM_POINT,					//体制 体制
    DETAIL_ATTRIB_INT_RANDOM_POINT,					//定力 智力
    DETAIL_ATTRIB_DEX_RANDOM_POINT,					//身法 敏捷
    DETAIL_ATTRIB_POINT_REMAIN,			            //剩余点数

	DETAIL_ATTRIB_ATT_PHYSICS,			//物理攻击力
	DETAIL_ATTRIB_DEF_PHYSICS,			//物理防御力
	DETAIL_ATTRIB_ATT_MAGIC,			//魔法攻击力
	DETAIL_ATTRIB_DEF_MAGIC,			//魔法防御力
	DETAIL_ATTRIB_MAXHP,				//最大生命值
	DETAIL_ATTRIB_MAXMP,				//最大魔法值
	DETAIL_ATTRIB_HP_RESPEED,			//HP恢复速度  点/秒
	DETAIL_ATTRIB_MP_RESPEED,			//MP恢复速度  点/秒
	DETAIL_ATTRIB_HIT,					//命中率
	DETAIL_ATTRIB_MISS,					//闪避率
	DETAIL_ATTRIB_CRITRATE,				//会心率
	DETAIL_ATTRIB_DEFENCE_C,			//抗暴率 2011-11-16 ZL+

	DETAIL_ATTRIB_RAGE,					//怒气
	DETAIL_ATTRIB_STRIKE_POINT,			//连技点

	DETAIL_ATTRIB_MOVESPEED,			//移动速度
	DETAIL_ATTRIB_ATTACKSPEED,			//攻击速度
	// 增加土攻土防 [11/9/2011 edit by ZL]
	DETAIL_ATTRIB_ATTACKCOLD,			//冰攻击 //水
	DETAIL_ATTRIB_DEFENCECOLD,			//冰防御
	DETAIL_ATTRIB_ATTACKFIRE,			//火攻击 //火
	DETAIL_ATTRIB_DEFENCEFIRE,			//火防御
	DETAIL_ATTRIB_ATTACKLIGHT,			//电攻击 //金
	DETAIL_ATTRIB_DEFENCELIGHT,			//电防御
	DETAIL_ATTRIB_ATTACKPOISON,			//毒攻击 //木
	DETAIL_ATTRIB_DEFENCEPOISON,		//毒防御
	DETAIL_ATTRIB_ATTACKEARTH,			//土攻击
	DETAIL_ATTRIB_DEFENCEEARTH,			//土防御

	DETAIL_ATTRIB_MENPAI,				//门派
	DETAIL_ATTRIB_GUILD,				//帮派
	DETAIL_ATTRIB_CAMP,					//阵营
	DETAIL_ATTRIB_DATAID,				//DataID
	DETAIL_ATTRIB_PORTRAITID,			//头像
	DETAIL_ATTRIB_MODELID,				//外形
	DETAIL_ATTRIB_MOUNTID,				//座骑
	DETAIL_ATTRIB_CURRENT_PET_GUID,		//当前的宠物GUID

	DETAIL_ATTRIB_LIMIT_MOVE,			//是否限制不能移动
	DETAIL_ATTRIB_CAN_ACTION1,			//技能限制标记1.
	DETAIL_ATTRIB_CAN_ACTION2,			//技能限制标记2.	
	//DETAIL_ATTRIB_LIMIT_HANDLE,			//是否限制不能进行一切操作

	DETAIL_ATTRIB_VIGOR,				// [2010-12-1] by: cfp+ 活力
	DETAIL_ATTRIB_MAX_VIGOR,			// [2010-12-1] by: cfp+ 活力上限
	DETAIL_ATTRIB_ENERGY,				//精力
	DETAIL_ATTRIB_MAX_ENERGY,			//精力上限
	DETAIL_ATTRIB_RMB,					//元宝
	DETAIL_ATTRIB_BANK_RMB,				//存在银行的元宝
	DETAIL_ATTRIB_DOUBLEEXPTIME,		// [2010-12-1] by: cfp+ 双倍经验时间
	DETAIL_ATTRIB_GMRIGHT,				// [2010-12-1] by: cfp+ 双倍经验时间
    DETAIL_ATTRIB_HELPMASK,             //新手引导掩码 [2011-8-10] by: cfp+

	//DETAIL_ATTRIB_GOODBADVALUE,			//善恶值

	//DETAIL_ATTRIB_MISSION_HAVEDONE_FLAGS,	//任务的完成标志
	//DETAIL_ATTRIB_MISSION_KILLOBJECT_FLAGS,	//任务的杀怪标志
	//DETAIL_ATTRIB_MISSION_ENTERAREA_FLAGS,	//任务的区域标志
	//DETAIL_ATTRIB_MISSION_ITEMCHANGED_FLAGS,	//任务的道具更新标志


	DETAIL_ATTRIB_NUMBERS				
}

//伤害类型
public enum DamageType_T
{
	//DAMAGE_NOSPECIALTYPE = 0,
	DAMAGE_TYPE_P,
	DAMAGE_TYPE_M,
	DAMAGE_TYPE_COLD,		//对应目前金伤害
	DAMAGE_TYPE_FIRE,		//对应目前木伤害
	DAMAGE_TYPE_LIGHT,		//对应目前水伤害
	DAMAGE_TYPE_POISON,		//对应目前火伤害
	DAMAGE_TYPE_EARTH,		//对应目前土伤害
	DAMAGE_TYPE_NUMBER,	//伤害类型数据
} 


// 角色属性
public enum ENUM_CHAR_ATT
{
	CHAR_ATT_INVALID	= -1,
	CHAR_ATT_STR,				// 力量 力量
	CHAR_ATT_SPR,				// 灵气 灵力
	CHAR_ATT_CON,				// 体制 体制
	CHAR_ATT_INT,				// 定力 智力
	CHAR_ATT_DEX,				// 身法 敏捷

	CHAR_ATT_HP,				// HP
	CHAR_ATT_MP,				// MP
	CHAR_ATT_MAX_HP,			// HP上限
	CHAR_ATT_MAX_MP,			// MP上限

	CHAR_ATT_RESTORE_HP,		// HP回复
	CHAR_ATT_RESTORE_MP,		// MP回复

	CHAR_ATT_HIT,				// 命中率
	CHAR_ATT_MISS,				// 闪避
	CHAR_ATT_CRIT_RATE,			// 会心率

	CHAR_ATT_SPEED,				// 移动速度
	CHAR_ATT_ATTACK_SPEED,		// 攻击速度

	CHAR_ATT_ATTACK_PHYSICS,	// 物理攻击
	CHAR_ATT_DEFENCE_PHYSICS,	// 物理防御

	CHAR_ATT_ATTACK_MAGIC,		// 魔法攻击
	CHAR_ATT_DEFENCE_MAGIC,		// 魔法防御

	CHAR_ATT_ATTACK_COLD,		// 水攻击
	CHAR_ATT_DEFENCE_COLD,		// 水防御

	CHAR_ATT_ATTACK_FIRE,		// 火攻击
	CHAR_ATT_DEFENCE_FIRE,		// 火防御

	CHAR_ATT_ATTACK_LIGHT,		// 金攻击
	CHAR_ATT_DEFENCE_LIGHT,		// 金防御

	CHAR_ATT_ATTACK_POISON,		// 木攻击
	CHAR_ATT_DEFENCE_POISON,	// 木防御

	CHAR_ATT_ATTACK_EARTH,		// 土攻击
	CHAR_ATT_DEFENCE_EARTH,		// 土防御

	CHAR_ATT_ANGER,				// 怒气值
	CHAR_ATT_SKILL_POINT,		// 连技点

	CHAR_ATT_NUMBERS
}

public enum TEAM_ERROR
{
	TEAM_ERROR_INVITEDESTHASTEAM=0 ,	//邀请对象已经属于某个组了
	TEAM_ERROR_INVITEREFUSE ,			//被邀请人拒绝加入
	TEAM_ERROR_INVITETEAMFULL ,			//邀请人的队伍人数已经满了
	TEAM_ERROR_INVITELEADERREFUSE ,		//队长拒绝新成员被邀请加入
	TEAM_ERROR_DISMISSNOTLEADER ,		//解散队伍的人不是队长
	TEAM_ERROR_KICKNOTLEADER ,			//踢人者不是队长
	TEAM_ERROR_APPLYSOURHASTEAM ,		//申请人已经属于某个组了
	TEAM_ERROR_APPLYDESTHASNOTTEAM ,	//被申请者不属于某个组
	TEAM_ERROR_APPLYLEADERREFUSE ,		//队长不同意申请人加入队伍
	TEAM_ERROR_APPLYTEAMFULL ,			//被申请人的队伍人数已经满了
	TEAM_ERROR_APPLYLEADERGUIDERROR ,	//被申请人所在队伍的队长GUID发生变化
	TEAM_ERROR_APPOINTSOURNOTEAM ,		//旧队长不是队伍成员
	TEAM_ERROR_APPOINTDESTNOTEAM ,		//新队长不是队伍成员
	TEAM_ERROR_APPOINTNOTSAMETEAM ,		//任命时两个人不属于同一个队伍
	TEAM_ERROR_APPOINTSOURNOLEADER ,	//旧队长不是队伍的队长了
	TEAM_ERROR_APPLYLEADERCANTANSWER,	//队长目前无法答复
	TEAM_ERROR_INVITERNOTINTEAM,		//邀请人不在队长的队伍中
	TEAM_ERROR_APPLYWHENINTEAM,			//申请人在有队伍的情况下申请入队
	TEAM_ERROR_TEAMFULL,				//队伍人数已满。
	TEAM_ERROR_REFUSEINVITESETTING,		//被邀请人设置了拒绝邀请
	TEAM_ERROR_TARGETNOTONLINE,			//对方已经离线，加入失败。

	TEAM_ERROR_ALLOCATIONRULERNOTLEADER ,		//// 改变分配模式的人不是队长 [8/23/2011 zzh+]
	TEAM_ERROR_INVALIDALLOCATIONRULER,		//// 改变分配模式的人不是队长 [8/23/2011 zzh+]


	TEAM_ERROR_NUMBER ,	//队伍功能错误号的最大值
}

public enum TEAM_RESULT
{
	TEAM_RESULT_MEMBERENTERTEAM=0 ,		//成员加入队伍
	TEAM_RESULT_MEMBERLEAVETEAM ,		//普通成员离开队伍
	TEAM_RESULT_LEADERLEAVETEAM ,		//队长离开队伍
	TEAM_RESULT_TEAMDISMISS ,			//队伍解散
	TEAM_RESULT_TEAMKICK ,				//踢除队员
	TEAM_RESULT_TEAMAPPOINT ,			//任命新队长
	TEAM_RESULT_TEAMREFRESH,			// 重新请求队伍信息的回复
	TEAM_RESULT_STARTCHANGESCENE,		// 开始切换场景
	TEAM_RESULT_ENTERSCENE,				// 队友进入新场景
	TEAM_RESULT_REFRESHSERVERINFO,		// 玩家跳转场景后，给服务器刷新队伍消息
	TEAM_RESULT_MEMBEROFFLINE,			// 玩家下线

	TEAM_RESULT_TEAMALLOCATIONRULER,	//// 改变分配模式[8/23/2011 zzh+]

	TEAM_RESULT_NUMBER , //队伍结果类型的最大值
}

//// 改变分配模式[8/23/2011 zzh+]
public enum TEAM_ALLOCATION_RULER
{
	TEAM_ALLOCATION_RULER_AVERAGE=0 , // 平均分配
	TEAM_ALLOCATION_RULER_PERSON=1 , // 个人分配(拥有者全部,非拥有者按比例)

	TEAM_ALLOCATION_RULER_NUMBER=2 , //类型的最大值
}

public enum NOTIFY_TEAMINFO
{
	NOTIFY_USERCHANGESCENE=0 ,	//队员转移场景
	NOTIFY_USERENTERSCENE,		//队员进入场景

}

//{----------------------------------------------------------------------------
// [2011-1-13] by: cfp+ 系统聊天时指定的显示位置
public enum ENUM_SYSTEM_CHAR_SHOW_POS
{
    CHAT_MAIN_WINDOW = 0,	//显示在主界面的聊天窗口内
    CHAT_LEFTDOWN,			//左下角个人的系统提示内
    CHAT_RIGHTDOWN,			//右下角系统的系统提示内
    CHAT_PLUMB_SCROLL,		//垂直滚动提示内
    CHAT_PLANE_SCROLL,		//水平滚动提示内
    CHAR_SHOW_POS_NUM

}
//----------------------------------------------------------------------------}

public enum ENUM_CHAT_TYPE
{
	CHAT_TYPE_INVALID = -1,

	CHAT_TYPE_NORMAL =0 ,	//普通说话消息
	CHAT_TYPE_TEAM ,		//队聊消息
	CHAT_TYPE_WORLD ,		//场景消息          Allan_Tao 改为世界了
	CHAT_TYPE_TELL ,		//私聊消息
	CHAT_TYPE_SYSTEM ,		//系统消息
	CHAT_TYPE_CHANNEL ,		//自建聊天频道消息
	CHAT_TYPE_GUILD ,		//帮派消息
	CHAT_TYPE_CAMP ,		//门派消息              Allan_Tao 改为阵营
	CHAT_TYPE_SELF,			//仅客户端使用的消息
    CHAT_TYPE_LIE,          // Allan_Tao 10/5/2011 谣言,暂时就加载这个后面
    CHAT_TYPE_MAP,          // 地图聊天                 Allan_Tao

	CHAT_TYPE_NUMBER,			//聊天类型的最大值
}

public enum CHAT_NEED_TYPE
{
	CHAT_NEED_NONE		= -1,					
	CHAT_NEED_MP		= 0 ,				//需要消耗MP
	CHAT_NEED_ENERGY	= 1 ,				// [2010-12-1] by: cfp+ 需要精力
	CHAT_NEED_VIGOR		= 2 ,				// [2010-12-1] by: cfp+ 需要活力

	CHAT_NEED_NUMBER,					//消耗类型的最大值
}

public enum CHANNEL_ERROR
{
	CHANNEL_ERROR_HASCHANNEL = 0 ,		//此用户已经创建了一个聊天频道了
	CHANNEL_ERROR_CHANNELFULL ,			//聊天频道创建失败
	CHANNEL_ERROR_NOCHANNEL ,			//没有频道信息
	CHANNEL_ERROR_CHANNELMEMBERFULL ,	//聊天频道内人数满了
	CHANNEL_ERROR_MEMBEREXIST ,			//用户已经存在聊天频道内了
	CHANNEL_ERROR_NOTINCHANNEL ,		//用户不在此聊天频道内


	CHANNEL_ERROR_NUMBER , //自建聊天频道错误号的最大值
}

public enum CHANNEL_RESULT
{
	CHANNEL_RESULT_CREATE = 0 ,			//创建聊天频道
	CHANNEL_RESULT_ADDMEMBER ,			//聊天频道内添加了新成员
	CHANNEL_RESULT_DISMISS ,			//自建聊天频道解散
	CHANNEL_RESULT_KICKMEMBER ,			//踢除成员


	CHANNEL_RESULT_NUMBER , //自建聊天频道结果类型的最大值
}

// 队员信息
public enum ENUM_TEAM_MEMBER_ATT
{
	TEAM_MEMBER_ATT_INVALID	= -1,

	TEAM_MEMBER_ATT_FAMILY,			// 门派
	TEAM_MEMBER_ATT_LEVEL,			// 等级
	TEAM_MEMBER_ATT_POSITION,		// 位置

	TEAM_MEMBER_ATT_HP,				// HP
	TEAM_MEMBER_ATT_MAX_HP,			// HP上限
	TEAM_MEMBER_ATT_MP,				// MP
	TEAM_MEMBER_ATT_MAX_MP,			// MP上限
	TEAM_MEMBER_ATT_ANGER,			// 怒气值

	TEAM_MEMBER_ATT_WEAPON,			// 武器
	TEAM_MEMBER_ATT_CAP,			// 帽子
	TEAM_MEMBER_ATT_ARMOR,			// 衣服
	TEAM_MEMBER_ATT_CUFF,			// 护腕
	TEAM_MEMBER_ATT_BOOT,			// 靴子

	TEAM_MEMBER_ATT_BUFF,			// 有益 buff

	TEAM_MEMBER_ATT_DEADLINK,		// 断线
	TEAM_MEMBER_ATT_DEAD,			// 死亡
	TEAM_MEMBER_ATT_FACEMESH,		// 面部模型
	TEAM_MEMBER_ATT_HAIRMESH,		// 头发模型
	TEAM_MEMBER_ATT_HAIRCOLOR,		// 头发颜色
	TEAM_MEMBER_ATT_BACK,			//背饰

	TEAM_MEMBER_ATT_NUMBERS
}


public enum	SHAREMEM_TYPE
{
	ST_INVAILD		=  -1,
	ST_HUMAN_SMU	=	1,
	ST_GUILD_SMU	=	2,
	ST_MAIL_SMU		=	3,
	ST_PSHOP_SMU	=	4,
	ST_USERDATA_SMU	=	5,
	ST_HUMANEXT_SMU	=	6,
	ST_ITEMSERIAL_SMU = 7,
}


//共享内存使用状态
public enum   SHAREMEM_USE_STATUS
{
	SUS_NEVER_USE		= 0,				//没有被使用过
    SUS_SERVER_USED				= 1,		//Server 占用
	SUS_SERVER_HAVE_CLEANED		= 2,		//Server 正常ShutDown ，已经清除
	SUS_WORLD_USED				= 3,		//World 占用
	SUS_WORLD_HAVE_CLEANED		= 4,		//World 正常ShutDown ，已经清除
}

public enum UDRETURN
{
	UDR_NOTFINDUSERDATA = 0 ,	//没有发现用户数据
	UDR_USERDATA ,				//拥有用户信息
	UDR_USERDATALIVING ,		//用户信息还在目的服务器的缓存中
	UDR_USERSERVERCRASH,		//用户Server已经Crash,可能还在缓存中,Server正在处理这块缓存
	UDR_KEYERROR ,				//验证码错误
}

public enum ENUM_COMBAT_EVENT //技能和效果关心的事件
{
	EVENT_ON_DAMAGE = 0,
	EVENT_ON_HIT_BY_SKILL,
	EVENT_ON_HIT_TARGET,
	EVENT_ON_BE_CRITICAL_HIT,
	EVENT_ON_CRITICAL_HIT_TARGET,
	EVENT_ON_HEAL_TARGET,
	EVENT_ON_BE_HEAL,
	EVENT_ON_DEAD,
	NUMBER_OF_EVENTS
}

// 杀气值
public enum ENUM_PK_VALUE_RANGE
{
	PK_VALUE_RANGE_1	= 1,
	PK_VALUE_RANGE_20	= 20,
	PK_VALUE_RANGE_40	= 40,
	PK_VALUE_RANGE_60	= 60,
	PK_VALUE_RANGE_80	= 80,
	PK_VALUE_RANGE_MAX	= 1000,
}

public enum TEAM_FOLLOW_RESULT
{
	TF_RESULT_REFUSE_FOLLOW = 0,	// 队员拒绝跟随队长
	TF_RESULT_ENTER_FOLLOW,			// 队员进入组队跟随状态
	TF_RESULT_STOP_FOLLOW,			// 队员退出组队跟随状态
	TF_RESULT_FOLLOW_FLAG,			// 通知客户端进入组队跟随状态的标记（ENTER_FOLLOW 的 quiet 模式）
}

public enum TEAM_FOLLOW_ERROR
{
	TF_ERROR_TOO_FAR = 0,			// 离队长太远了（而不能跟随）
	TF_ERROR_IN_TEAM_FOLLOW,		// 已经处于组队跟随状态（而不能做某些操作）
	TF_ERROR_STALL_OPEN,			// 正在摆摊
	TF_ERROR_NOT_IN_FOLLOW_MODE,	// 队伍目前不处于跟随状态
}

public enum SERVER_TYPE			//服务器端程序类型
{
	SERVER_GAME = 0 ,		//游戏服务器端程序
	SERVER_LOGIN = 1 ,		//登陆服务器端程序
	SERVER_SHAREMEN = 2 ,	//共享存储端程序
	SERVER_WORLD = 3,
	SERVER_BILLING = 4,
	SERVER_NUMBER ,
}

public enum RELATION_GROUP
{
	RELATION_GROUP_FRIEND_ALL		= 0,		//全部好友
	RELATION_GROUP_F1,							//好友1组
	RELATION_GROUP_F2,							//好友2组
	RELATION_GROUP_F3,							//好友3组
	RELATION_GROUP_F4,							//好友4组
	RELATION_GROUP_BLACK,						//黑名单组

	RELATION_GROUP_NUMBER ,						//存储组数量

	RELATION_GROUP_TEMPFRIEND,					//临时好友
}

public enum RELATION_TYPE //联系人关系
{
	RELATION_TYPE_NONE				= 0,		//空
	RELATION_TYPE_FRIEND,						//好友
	RELATION_TYPE_BROTHER,						//结拜
	RELATION_TYPE_MARRY,						//结婚
	RELATION_TYPE_BLACKNAME,					//黑名单
	RELATION_TYPE_TEMPFRIEND,					//临时好友
	RELATION_TYPE_STRANGER,						//陌生人关系
	RELATION_TYPE_MASTER,						//师傅关系
	RELATION_TYPE_PRENTICE,						//徒弟关系
	RELATION_TYPE_SIZE,							//关系种类
}

public enum RELATION_DEFINE
{
  RELATION_FRIEND_OFFSET = 0,
  RELATION_BLACKNAME_OFFSET =  80
}



public enum ENUM_SKILL_CLASS_BY_USER_TYPE//技能的使用者分类
{
	A_SKILL_FOR_PLAYER 	= 0,
	A_SKILL_FOR_MONSTER = 1,
	A_SKILL_FOR_PET 	= 2,
	A_SKILL_FOR_ITEM 	= 3,
}
public enum CONSTANT_VALUE //一些常量
{
	DEFAULT_ATTACK_FLUCTUATION = 8, //攻击浮动 +/- 8%
}



//GM指令类型
public enum COMMAND_TYPE
{
	COMMAND_TYPE_NONE = 0 ,				//空指令
	COMMAND_TYPE_TELEPORT ,				//将一个OBJ移动到某地
	COMMAND_TYPE_MODIFYMENPAI ,			//修改角色门派属性
	COMMAND_TYPE_MODIFYXINFA ,			//修改角色心法等级
	COMMAND_TYPE_LEVELUPALLXINFA,		//所有心法等级修改
	COMMAND_TYPE_CREATEITEM ,			//创建物品
	COMMAND_TYPE_RELIVE ,				//复活自己
	COMMAND_TYPE_CHAT ,					//发送Chat消息
	COMMAND_TYPE_ENABLEGOD ,			//充许无敌
	COMMAND_TYPE_DISABLEGOD ,			//禁止无敌
	COMMAND_TYPE_IAMGOD ,				//变成无敌
	COMMAND_TYPE_IAMDOG ,				//取消无敌
	COMMAND_TYPE_DEBUG_RELOADCONFIG ,	//重新读取Config配置文件
	COMMAND_TYPE_SYSCHAT ,				//系统弹出消息
	COMMAND_TYPE_DEBUG_SHUTDOWN ,		//关闭服务器
	COMMAND_TYPE_TEAM_INVITE ,			//队伍-邀请
	COMMAND_TYPE_TEAM_RETINVITE ,		//队伍-邀请回应
	COMMAND_TYPE_TEAM_LEADERRETINVITE , //队伍-队长邀请回应
	COMMAND_TYPE_TEAM_APPLY ,			//队伍-申请
	COMMAND_TYPE_TEAM_RETAPPLY ,		//队伍-申请回应
	COMMAND_TYPE_TEAM_LEAVE ,			//队伍-离开
	COMMAND_TYPE_TEAM_KICK ,			//队伍-踢人
	COMMAND_TYPE_TEAM_DISMISS ,			//队伍-解散
	COMMAND_TYPE_TEAM_APPOINT ,			//队伍-任命
	COMMAND_TYPE_ABILITY_DETAIL,		//请求生活技能详细信息
	COMMAND_TYPE_USE_ABILITY,			//使用生活技能
	COMMAND_TYPE_COMBOUND_GEM,			//合成宝石
	COMMAND_TYPE_USE_GEM,				//宝石镶嵌
	COMMAND_TYPE_CHANNEL_CREATE,		//自建聊天频道-创建
	COMMAND_TYPE_CHANNEL_INVITE,		//自建聊天频道-邀请
	COMMAND_TYPE_CHANNEL_DISMISS,		//自建聊天频道-解散
	COMMAND_TYPE_CHANNEL_KICK,			//自建聊天频道-踢除成员
	COMMAND_TYPE_GOTO ,					//传送玩家本身到目标位置（或新场景）
	COMMAND_TYPE_FULLRECOVER ,			//回复满血、魔
	COMMAND_TYPE_CREATEPET ,			//创建宠物
	COMMAND_TYPE_DELETEPET ,			//删除宠物
	COMMAND_TYPE_CAPTUREPET ,			//捕捉宠物
	COMMAND_TYPE_CALLUPPET ,			//召唤宠物
	COMMAND_TYPE_RECALLPET ,			//收回宠物
	COMMAND_TYPE_FREEPETTONATURE ,		//放生宠物
	COMMAND_TYPE_RETURNTOCHILD ,		//宠物还童
	COMMAND_TYPE_SKILL_RELIVE,			//技能复活
	COMMAND_TYPE_CREATESCENE ,			//创建一个副本场景
	COMMAND_TYPE_CLOSESCENE ,			//强制关闭一个副本场景
	COMMAND_TYPE_SET_SCENE_TYPE,		//设置场景类型
	COMMAND_TYPE_SET_PK_VALUE,			//设置杀气
	COMMAND_TYPE_SET_MODEL_ID,			//设置模型ID
	COMMAND_TYPE_SET_EQUIP_DUR,			//设置装备耐久度
	COMMAND_TYPE_SETDAMAGE,				//设置伤害
	COMMAND_TYPE_KILLOBJ,				//杀死一个OBJ
	COMMAND_TYPE_CREATEMONSTER,			//生成一只怪物
	COMMAND_TYPE_DELETEMONSTER,			//删除一只生成的怪物
	COMMAND_TYPE_SETAITYPE,				//修改AI类型
	COMMAND_TYPE_MODIFYMONEY,			//修改金钱
	COMMAND_TYPE_MAKETEAMFOLLOW,		//模拟队伍跟随
	COMMAND_TYPE_SENDMAIL,				//发送邮件
	COMMAND_TYPE_RECVMAIL,				//接收邮件
	COMMAND_TYPE_CREATEGUILD,			// 创建帮会
	COMMAND_TYPE_JOINGUILD,				// 加入帮会
	COMMAND_TYPE_GUILDRECRUIT,			// 帮会收人
	COMMAND_TYPE_GUILDEXPEL,			// 帮会踢人
	COMMAND_TYPE_LEAVEGUILD,			// 离开帮会
	COMMAND_TYPE_GUILDDISMISS,			// 解散帮会
	COMMAND_TYPE_ASKGUILDLIST,			// 帮会列表
	COMMAND_TYPE_SETMOVEMODE,			// 设置移动模式 走/跑/疾跑
	COMMAND_TYPE_ASKRELATIONLIST,		// 关系列表
	COMMAND_TYPE_ADDFRIEND,				// 加入一个好友
	COMMAND_TYPE_SETFRIENDPOINT,		// 设置友好度
	COMMAND_TYPE_ADDBLACKNAME,			// 加入黑名单
	COMMAND_TYPE_DELFRIEND,				// 删除一个好友
	COMMAND_TYPE_DELBLACK,				// 从黑名单里删除一个玩家
	COMMAND_TYPE_TRANSITION,			// 组间转移
	COMMAND_TYPE_SETCAMPID,				// 设置阵营
	COMMAND_TYPE_SAVE,					// 保存用户档案
	COMMAND_TYPE_GETOBJINFO,			// 取得某个obj的服务器端数据
	COMMAND_TYPE_LEVELUP,				// 升级
	COMMAND_TYPE_ABILITYUP,				// 生活技能升级
	COMMAND_TYPE_WHO,					// 当前场景
	COMMAND_TYPE_ALLWHO,				// 所有场景
	COMMAND_TYPE_CATCHPLAYER,			// 取得世界中某个玩家的基本信息
	COMMAND_TYPE_PLAYERINFO,			// 取得场景中某个玩家的详细信息
	COMMAND_TYPE_SETPETHAPPINESS,		// 设置宠物的快乐度
	COMMAND_TYPE_SETPETHP,				// 设置宠物HP
	COMMAND_TYPE_SETPETLIFE,			// 设置宠物的寿命
	COMMAND_TYPE_PETLEVELUP,			// 升级当前宠物
	COMMAND_TYPE_SETPETSKILL,			// 设置宠物的技能
	COMMAND_TYPE_USEPETSKILL,			// 使用宠物的技能
	COMMAND_TYPE_LEVELDOWN,				// 降级，只降低级别数值，不管其他相关的属性
	COMMAND_TYPE_ADDEXP,				// 增加经验
	COMMAND_TYPE_CREATECITY,			// 建立城市
	COMMAND_TYPE_DELETECITY,			// 摧毁城市
	COMMAND_TYPE_LOADMONSTER,			// 像场景增加一批怪物
	COMMAND_TYPE_SETHAIRCOLOR,			// 设置玩家头发颜色
	COMMAND_TYPE_SETHAIRMODEL,			// 设置玩家头发模型
	COMMAND_TYPE_CREATEGUARD,			// 创建分身
	COMMAND_TYPE_CREATETRAP,			// 创建陷阱
	COMMAND_TYPE_USEITEM,				// 使用物品
	COMMAND_TYPE_FORGETRECIPE,			// 忘记配方
	COMMAND_TYPE_SEND_IMPACT_TO_UNIT,	// 给某个角色一个效果
	COMMAND_TYPE_RELOAD_COMBAT_TABLES,	// 重新装载战斗数据表
	COMMAND_TYPE_USE_SKILL,				// 施放魔法
	COMMAND_TYPE_SAVEPACKETINFO,		// 保存消息分配信息
	COMMAND_TYPE_SETVIGOR,				// 设置活力值
	COMMAND_TYPE_SETENERGY,				// 设置精力值
	COMMAND_TYPE_PETPULLULATE,			// 宠物成长率
	COMMAND_TYPE_ADD_SKILL,				// 添加技能
	COMMAND_TYPE_REMOVE_SKILL,			// 删除技能
	COMMAND_TYPE_FINGERGUID,			// 用 GUID 找玩家信息
	COMMAND_TYPE_FINGERNAME,			// 用名字找玩家信息
	COMMAND_TYPE_ADVANCEDFINGER,		// 高级查找
	COMMAND_TYPE_SETABILITYEXP,			// 设置生活技能熟练度
	COMMAND_TYPE_SERMBMONEY,			// 设置RMB(原善恶值)
	COMMAND_TYPE_DOUBLEEXP,             //双倍经验
    COMMAND_TYPE_SETGMRIGHT,            //设置gm权限
    COMMAND_TYPE_SHOWGUID,            //显示guid
    COMMAND_TYPE_GMTEST,                //测试用
	COMMAND_TYPE_NUMBER , //指令数量
}

// 宠物技能列表
public enum ENUM_PET_SKILL_INDEX
{
	PET_SKILL_INDEX_INVALID = -1,
	PET_SKILL_INDEX_CONTROL_BY_PLAYER,
	PET_SKILL_INDEX_CONTROL_BY_AI0,
	PET_SKILL_INDEX_CONTROL_BY_AI1,
	PET_SKILL_INDEX_CONTROL_BY_AI2,
	PET_SKILL_INDEX_CONTROL_BY_AI3,
	PET_SKILL_INDEX_MENPAI,

	PET_SKILL_INDEX_NUMBERS
}

// 宠物技能操作模式
public enum PET_SKILL_OPERATEMODE
{
	PET_SKILL_OPERATE_INVALID			= -1, // INVALID
	PET_SKILL_OPERATE_NEEDOWNER			= 0,  // 需要主人触发
	PET_SKILL_OPERATE_AISTRATEGY		= 1,  // 由AI来触发	
	PET_SKILL_OPERATE_INCEACEATTR		= 2,  // 增强宠物属性

	PET_SKILL_OPERATE_NUMBERS,
	
}

public enum GM_EXECUTE_LEVEL
{	
	GM_EXECUTE_GM =1,
	GM_EXECUTE_GMADMIN,
	GM_EXECUTE_ADMIN,
	GM_EXECUTE_ALL,

}

// 邮件类型
public enum MAIL_TYPE
{
	MAIL_TYPE_NORMAL = 0 ,//普通邮件
	MAIL_TYPE_SYSTEM = 1 ,//系统邮件
	MAIL_TYPE_SCRIPT = 2 ,//脚本邮件，服务器端接收到此邮件后会调用一个脚本
	MAIL_TYPE_NORMAL2 = 3 , // 系统给玩家代发
}

// 可执行邮件功能
//!!!!此定义必须与脚本ScriptGlobal.lua中的定义完全相同，
//!!!!切勿在不了解整个可执行邮件流程的情况下随便添加
public enum MAIL_DEFINE
{
    MAIL_REPUDIATE		=	1,			//强制离婚
    MAIL_BETRAYMASTER	=	2,	//叛师
    MAIL_EXPELPRENTICE	=	3,			//开除徒弟
    MAIL_UPDATE_ATTR	=	4			//更新不在线玩家属性(通过可执行邮件系统自动在玩家上线时更新)
}


public enum MAIN_ATTR_DEFINE
{
	MAIL_ATTR_GUILD = 1,					//工会ID
	MAIL_ATTR_MONEY						//身上的金钱
}

//请求邮件的请求类型
public enum ASK_TYPE
{
	ASK_TYPE_LOGIN = 0 ,//用户刚登陆游戏时发送的邮件检查消息,
						//如果有邮件，服务器发送通知消息
	ASK_TYPE_MAIL 	//用户向服务器提出需要邮件的消息
					//如果有邮件则发送邮件数据
}

// 游戏设置标志
// 低 16 位是需要传到 World 的，高 16 位是只需要 Server 保存的
public enum GAME_SETTING_FLAG
{
	GSF_CANNOT_ADD_FRIEND			= 0,		// 拒绝添加好友
	GSF_CANNOT_RECV_MAIL			= 1,		// 拒绝接收邮件
	GSF_CANNOT_RECV_STRANGER_MAIL	= 2,		// 拒绝接收陌生人邮件
	GSF_REFUSE_TEAM_INVITE			= 3,		// 拒绝组队邀请

	GSF_REFUSE_LOOK_SPOUSE_INFO		= 16,		// 拒绝查看配偶资料
	GSF_REFUSE_TRADE				= 17,		// 拒绝交易
	GSF_CLOSE_POPUP					= 18,		// 关闭当前泡泡框
	GSF_MANUAL_LEVEL_UP				= 19,		// 手动升级
}
public enum SETTING_TYPE
{
	SETTING_TYPE_NONE = 0 ,
	SETTING_TYPE_GAME ,		//联系人设置, bit含义见 public enum GAME_SETTING_FLAG
	SETTING_TYPE_K0 ,		//0号快捷栏设置
	SETTING_TYPE_K1 ,		//1号快捷栏设置
	SETTING_TYPE_K2 ,		//2号快捷栏设置
	SETTING_TYPE_K3 ,		//3号快捷栏设置
	SETTING_TYPE_K4 ,		//4号快捷栏设置
	SETTING_TYPE_K5 ,		//5号快捷栏设置
	SETTING_TYPE_K6 ,		//6号快捷栏设置
	SETTING_TYPE_K7 ,		//7号快捷栏设置
	SETTING_TYPE_K8 ,		//8号快捷栏设置
	SETTING_TYPE_K9 ,		//9号快捷栏设置
	SETTING_TYPE_K10 ,	//右边0号快捷栏设置
	SETTING_TYPE_K11 ,	//右边1号快捷栏设置
	SETTING_TYPE_K12 ,	//右边2号快捷栏设置
	SETTING_TYPE_K13 ,	//右边3号快捷栏设置
	SETTING_TYPE_K14 ,	//右边4号快捷栏设置
	SETTING_TYPE_K15 ,	//右边5号快捷栏设置
	SETTING_TYPE_K16 ,	//右边6号快捷栏设置
	SETTING_TYPE_K17 ,	//右边7号快捷栏设置
	SETTING_TYPE_K18 ,	//右边8号快捷栏设置
	SETTING_TYPE_K19 ,	//右边9号快捷栏设置

	SETTING_TYPE_CHAT_TAB1_PART1,	//聊天自定义tab1设置第一部分
	SETTING_TYPE_CHAT_TAB1_PART2,	//聊天自定义tab1设置第二部分
	SETTING_TYPE_CHAT_TAB2_PART1,	//聊天自定义tab2设置第一部分
	SETTING_TYPE_CHAT_TAB2_PART2,	//聊天自定义tab2设置第二部分
	SETTING_TYPE_CHAT_TAB3_PART1,	//聊天自定义tab3设置第一部分
	SETTING_TYPE_CHAT_TAB3_PART2,	//聊天自定义tab3设置第二部分
	SETTING_TYPE_CHAT_TAB4_PART1,	//聊天自定义tab4设置第一部分
	SETTING_TYPE_CHAT_TAB4_PART2,	//聊天自定义tab4设置第二部分

	// 修改自定义快捷键 [8/26/2011 edit by ZL]
	// 暂时取消 ZL-
// 	SETTING_TYPE_K0_HOTKEY ,		//自定义0号快捷栏快捷键
// 	SETTING_TYPE_K1_HOTKEY ,		//自定义1号快捷栏快捷键
// 	SETTING_TYPE_K2_HOTKEY ,		//自定义2号快捷栏快捷键
// 	SETTING_TYPE_K3_HOTKEY ,		//自定义3号快捷栏快捷键
// 	SETTING_TYPE_K4_HOTKEY ,		//自定义4号快捷栏快捷键
// 	SETTING_TYPE_K5_HOTKEY ,		//自定义5号快捷栏快捷键
// 	SETTING_TYPE_K6_HOTKEY ,		//自定义6号快捷栏快捷键
// 	SETTING_TYPE_K7_HOTKEY ,		//自定义7号快捷栏快捷键
// 	SETTING_TYPE_K8_HOTKEY ,		//自定义8号快捷栏快捷键
// 	SETTING_TYPE_K9_HOTKEY ,		//自定义9号快捷栏快捷键
// 	SETTING_TYPE_K10_HOTKEY ,	//自定义右边0号快捷栏快捷键
// 	SETTING_TYPE_K11_HOTKEY ,	//自定义右边1号快捷栏快捷键
// 	SETTING_TYPE_K12_HOTKEY ,	//自定义右边2号快捷栏快捷键
// 	SETTING_TYPE_K13_HOTKEY ,	//自定义右边3号快捷栏快捷键
// 	SETTING_TYPE_K14_HOTKEY ,	//自定义右边4号快捷栏快捷键
// 	SETTING_TYPE_K15_HOTKEY ,	//自定义右边5号快捷栏快捷键
// 	SETTING_TYPE_K16_HOTKEY ,	//自定义右边6号快捷栏快捷键
// 	SETTING_TYPE_K17_HOTKEY ,	//自定义右边7号快捷栏快捷键
// 	SETTING_TYPE_K18_HOTKEY ,	//自定义右边8号快捷栏快捷键
// 	SETTING_TYPE_K19_HOTKEY ,	//自定义右边9号快捷栏快捷键

	SETTING_TYPE_NUMBER ,//用户设置数据的类型数量
}

// 性别
public enum ENUM_SEX
{
	SEX_INVALID	= -1,
	SEX_FEMALE,			// 雌
	SEX_MALE,			// 雄

	SEX_NUMBERS
}

// 玩家死亡类型
public enum ENUM_HUMAN_DIE_TYPE
{
	HUMAN_DIE_TYPE_INVALID = -1,
	HUMAN_DIE_TYPE_INTERCHANGE,		// 切磋
	HUMAN_DIE_TYPE_MONSTER_KILL,	// 被怪杀死
	HUMAN_DIE_TYPE_PLAYER_KILL,		// 被玩家杀死

	HUMAN_DIE_TYPE_NUMBERS
}

public enum TASK_EVENT
{
	TASK_EVENT_KILLOBJ		= 0 ,	//杀死怪物
	TASK_EVENT_ENTERAREA	= 1 ,	//进入事件区
	TASK_EVENT_ITEMCHANGED	= 2 ,	//物品变化
	TASK_EVENT_PETCHANGED	= 3 ,	//宠物变化
	TASK_EVENT_LOCKEDTARGET	= 4 ,	//// 目标锁定 [11/4/2010 zzh+]
	TASK_EVENT_ENTERCOPY	= 5 ,	//// 进入副本 [11/4/2010 zzh+]
	TASK_EVENT_CONVEY	= 6 ,	//// 进入副本 [11/4/2010 zzh+]
}

public enum	CMD_MODE
{
	CMD_MODE_CLEARALL	=	1,		//清除模式
	CMD_MODE_LOADDUMP	=	2,		//载入dump模式
}


public enum	WORLD_TIME
{
	WT_IND_1	=	0,	//子时
	WT_IND_2,			//丑时
	WT_IND_3,			//寅时
	WT_IND_4,			//卯时
	WT_IND_5,			//辰时
	WT_IND_6,			//巳时
	WT_IND_7,			//午时
	WT_IND_8,			//未时
	WT_IND_9,			//申时
	WT_IND_10,			//酉时
	WT_IND_11,			//戌时
	WT_IND_12			//亥时
}

public enum	NPC_SHOP_TYPE
{
	SHOP_All		= 1,	//
	SHOP_DEFENCE	= 2,	//防具
	SHOP_ADORN		= 3,	//饰物
	SHOP_WEAPON		= 4,	//武器

	SHOP_FOOD		= 5,	//食物
	SHOP_MATERIAL	= 6,	//材料
	SHOP_COMITEM	= 7,	//药品

	////zzh+: 根据ShopTable.txt中的收购类型字段发现,商店都是设置为8 or 9.
	////zzh+: 我先假设所有商店都收购各种类型的东东(以后再考虑特殊商店和特殊物品)
	SHOP_ZZH_8		= 8,
	SHOP_ZZH_9		= 9,
}

public enum SystemUseSkillID_T
{
	MELEE_ATTACK = 0,
	CAPTURE_PET = 1,
	CALL_UP_PET = 2,
	SYSTEM_RESERVED_3	= 3,
	SYSTEM_RESERVED_4	= 4,
	SYSTEM_RESERVED_5	= 5,
	SYSTEM_RESERVED_6	= 6,
	SYSTEM_RESERVED_7	= 7,
	SHADOW_GUARD 		= 8,
	XIAOYAO_TRAPS		= 9,
} 

public enum SystemUseImpactID_T
{
	IMP_DAMAGES_OF_ATTACKS = 0,
	IMP_NOTYPE_DAMAGE,
} 

 //战斗距离计算，可接受的距离误差

public enum AcceptableDistanceError_NS
{
	ADE_FOR_HUMAN = 2,
	ADE_FOR_NPC = 1,
}


////zzh+ 根据ShopTable.txt调整 public enum CURRENCY_UNIT
////zzh+ 货币单位(1代表金币，2代表善恶值，4,帮贡，3代表师德点,5元宝，6赠点,7师门贡献度)
////zzh+ 参考客户端的shop.ua
/**** local CU_MONEY			= 1	-- 钱
local CU_GOODBAD		= 2	-- 善恶值
local CU_MORALPOINT	= 3	-- 师德点
local CU_TICKET			= 4 -- 官票钱
local CU_YUANBAO		= 5	-- 元宝
local CU_ZENGDIAN		= 6 -- 赠点
local CU_MENPAI_POINT		= 7 -- 门派贡献度
****/
public enum CURRENCY_UNIT	// 货币单位
{
	CU_INVALID,		// 0:无效
	CU_MONEY,		// 钱
	CU_GOODBAD,		// 善恶值
	CU_TICKET,		// 帮贡( 官票钱 )
	CU_MORALPOINT,	// 师德点
	CU_YUANBAO,		// 元宝
	CU_ZENGDIAN,	// 赠点
	CU_MENPAI_POINT, // 门派贡献度
}

// 跑环任务类型 [11/16/2010 ivan edit]
public enum MissionType
{
	MissionType_XunWu = 1,			//寻物
	MissionType_SongXin,			//送信
	MissionType_DingDianYinDao,		//定点引导
	MissionType_FuBenZhanDou,		//副本战斗
	MissionType_BuZhuo,				//捕捉
	MissionType_ShouJi,				//收集
	MissionType_KaiGuang,			//开光
	MissionType_OtherMenpaiFuben,	//其他门派副本
	MissionType_killMonster			//杀怪
}

public enum CMD_AFTERMOVE_TYPE
{
	CMD_AFMV_INVALID	= -1,
	CMD_AFMV_SPEAK,
	CMD_AFMV_USESKILL,
	CMD_AFMV_Tripper_ACTIVE,
    CMD_AFMV_ENTER_SPECIALBUS,	// 进入载具 [8/26/2011 ivan edit]
    CMD_AFMV_AutoHit,           // 进入自动打怪 [3/14/2012 Ivan]
}


public enum BULLETIN_SEND_TYPE
{
    BULLETIN_TYPE_SYSTEM    = 11,                // Allan_Tao 11/5/2011 系统公告

    BULLETIN_TYPE_ALL       = 21,                // Allan_Tao 11/5/2011 全服公告
    BULLETIN_TYPE_CAMP      = 22,                // Allan_Tao 11/5/2011 指定阵营公告
    BULLETIN_TYPE_GUILD     = 23,                // Allan_Tao 11/5/2011 指定帮派公告
    BULLETIN_TYPE_MAP       = 24,                // Allan_Tao 11/5/2011 指定地图公告
    BULLETIN_TYPE_DUP       = 25,                // Allan_Tao 11/5/2011 指定副本公告
}
public struct GameDefine2
{
    //防沉迷系统定义
    const float WALLOW_NORMAL		= 1.0f; //正常游戏
    const float WALLOW_HALF_INCOME	= 0.5f; //收益减半
    const float WALLOW_NONE			= 0.0f; //无收益
}


