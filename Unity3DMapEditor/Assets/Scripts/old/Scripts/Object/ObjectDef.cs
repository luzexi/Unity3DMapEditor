// ObjectDef.h
//
//////////////////////////////////////////////////////

public struct  ObjectDef 
{
    // 客户端预测指令的逻辑计数加值
 public   const int DEF_CLIENT_LOGIC_COUNT_ADD_STEP =		(100);

// 角色位置校正的距离
// 这个值作用不够明确 [8/1/2011 Sun]
  public  const float DEF_CHARACTER_POS_ADJUST_DIST	=	(2.0f);

// 客户端发角色位置较正消息的距离阀值  //fujia 2008.1.15   0.08f
  public  const float DEF_CLIENT_ADJUST_POS_WARP_DIST	=	(GAMEDEFINE.DEF_SERVER_ADJUST_POS_WARP_DIST * 0.02f);

// 进入战斗状态后要持续的时间
  public const int FIGHT_STATE_DURATION_TIME = (6000);
}


// 性别
public enum ENUM_GENDER
{
	GENDER_INVALID	= -1,
	GENDER_MALE,
	GENDER_FEMALE
}

// 武器类型 旧
// enum ENUM_WEAPON_TYPE
// {
// 	WEAPON_TYPE_INVALID	= -1,
// 	WEAPON_TYPE_NONE,		// 空手
// 	WEAPON_TYPE_SPEAR,		// 长枪
// 	WEAPON_TYPE_LONG,		// 大刀
// 	WEAPON_TYPE_DSHORT,		// 双短
// 	WEAPON_TYPE_SHORT,		// 单短
// 	WEAPON_TYPE_FAN,		// 扇类
// 	WEAPON_TYPE_RING,		// 环类
// 	WEAPON_TYPE_MONSTER,	// 怪物
// 	WEAPON_TYPE_NPC,		// NPC，就是不可以被攻击的 Monster
// 
// 	WEAPON_TYPE_NUMBERS
// }
// 武器类型新 [9/8/2011 Sun]
public enum ENUM_WEAPON_TYPE
{
	WEAPON_TYPE_INVALID	= -1,
	WEAPON_TYPE_NONE,		// 空手
	WEAPON_TYPE_SPEAR,		// 长枪
	WEAPON_TYPE_BOW,		// 弓箭
	WEAPON_TYPE_SHORT,		// 单短
	WEAPON_TYPE_STAFF,		// 法杖
	WEAPON_TYPE_DSHORT,		// 双短
	
	WEAPON_TYPE_RING,		// 环类
	WEAPON_TYPE_MONSTER,	// 怪物
	WEAPON_TYPE_NPC,		// NPC，就是不可以被攻击的 Monster

	WEAPON_TYPE_NUMBERS
}

// 动作组
public enum ENUM_BASE_ACTION
{
	BASE_ACTION_INVALID	= -1,
	BASE_ACTION_N_IDLE			=  0,			// (和平)休息
	BASE_ACTION_N_IDLE_EX0		=  1,			// (和平)休息小动作1
	BASE_ACTION_N_IDLE_EX1		=  2,			// (和平)休息小动作2
	BASE_ACTION_N_WALK			=  3,			// (和平)行走
	BASE_ACTION_N_RUN			=  4,			// (和平)跑步
	BASE_ACTION_N_SIT_DOWN		=  5,			// (和平)坐下
	BASE_ACTION_N_SIT_IDLE		=  6,			// (和平)坐下休息
	BASE_ACTION_N_SIT_STAND		=  7,			// (和平)坐下起身
	BASE_ACTION_N_RESERVE_00	=  8,			// 保留
	BASE_ACTION_N_JUMP_N		=  9,			// (和平)正常跳跃中
	BASE_ACTION_N_RESERVE_01	= 10,			// 保留
	BASE_ACTION_N_RESERVE_02	= 11,			// 保留
	BASE_ACTION_N_JUMP_R		= 12,			// (和平)移动中跳跃
	BASE_ACTION_N_RESERVE_03	= 13,			// 保留
	BASE_ACTION_F_IDLE			= 14,			// (战斗)休息
	BASE_ACTION_F_WALK			= 15,			// (战斗)行走
	BASE_ACTION_F_RUN			= 16,			// (战斗)跑步
	BASE_ACTION_F_RESERVE_04	= 17,			// 保留
	BASE_ACTION_F_JUMP_N		= 18,			// (战斗)正常跳跃中
	BASE_ACTION_F_RESERVE_05	= 19,			// 战斗1
	BASE_ACTION_F_RESERVE_06	= 20,			// 战斗2
	BASE_ACTION_F_JUMP_R		= 21,			// (战斗)移动中跳跃
	BASE_ACTION_F_RESERVE_07	= 22,			// 保留
	BASE_ACTION_F_BE_HIT		= 23,			// (战斗)被击中
	BASE_ACTION_F_DIE			= 24,			// (战斗)死亡
	BASE_ACTION_F_CADAVER		= 25,			// (战斗)尸体
	//BASE_ACTION_GATHER_MINE		= 26,			// 采矿
	//BASE_ACTION_GATHER_HERBS	= 27,			// 采草药
	//BASE_ACTION_FISH			= 28,			// 钓鱼

	//BASE_ACTION_M_IDLE_0			= 29,			// (坐骑上)休息
	//BASE_ACTION_M_IDLE_EX0_0		= 30,			// (坐骑上)休息小动作1
	//BASE_ACTION_M_IDLE_EX1_0		= 31,			// (坐骑上)休息小动作2
	//BASE_ACTION_M_WALK_0			= 32,			// (坐骑上)行走
	//BASE_ACTION_M_RUN_0				= 33,			// (坐骑上)跑步
	//BASE_ACTION_M_RESERVE_08		= 34,			// 保留
	//BASE_ACTION_M_JUMP_N_0			= 35,			// (坐骑上)正常跳跃中
	//BASE_ACTION_M_RESERVE_09		= 36,			// (保留
	//BASE_ACTION_M_RESERVE_10		= 37,			// 保留
	//BASE_ACTION_M_JUMP_R_0			= 38,			// (坐骑上)移动中跳跃
	//BASE_ACTION_M_RESERVE_11		= 39,			// 保留

	//BASE_ACTION_M_IDLE_1			= 40,			// (坐骑上)休息
	//BASE_ACTION_M_IDLE_EX0_1		= 41,			// (坐骑上)休息小动作1
	//BASE_ACTION_M_IDLE_EX1_1		= 42,			// (坐骑上)休息小动作2
	//BASE_ACTION_M_WALK_1			= 43,			// (坐骑上)行走
	//BASE_ACTION_M_RUN_1				= 44,			// (坐骑上)跑步
	//BASE_ACTION_M_RESERVE_12		= 45,			// 保留
	//BASE_ACTION_M_JUMP_N_1			= 46,			// (坐骑上)正常跳跃中
	//BASE_ACTION_M_RESERVE_13		= 47,			// 保留
	//BASE_ACTION_M_RESERVE_14		= 48,			// 保留
	//BASE_ACTION_M_JUMP_R_1			= 49,			// (坐骑上)移动中跳跃
	//BASE_ACTION_M_RESERVE_15		= 50,			// 保留

	BASE_ACTION_ABILITY_LEVEL_UP	= 92,			// 生活技能升级
	BASE_ACTION_XINFA_LEVEL_UP		= 93,			// 心法升级
	BASE_ACTION_LEVEL_UP			= 99,			// 升级

	BASE_ACTION_NUMBERS
}

// 特效绑定点
public enum ENUM_LOCATOR
{
	LOCATOR_INVALID	= -1,
	LOCATOR_CENTER,			// 身体中心
	LOCATOR_HEAD,			// 头部
	LOCATOR_HAND_L,			// 左手
	LOCATOR_HAND_R,			// 右手
	LOCATOR_WEAPON_L,		// 左武器
	LOCATOR_WEAPON_R,		// 右武器
	LOCATOR_FOOT_L,			// 左脚
	LOCATOR_FOOT_R,			// 右脚
	LOCATOR_FOOT_CENTER,	// 脚底中心
	LOCATOR_CALVARIA,		// 头顶偏上
	LOCATOR_HAND_L_AND_R,	// 左手和右手
	LOCATOR_HAND_CENTER,	// 两手中心

	LOCATOR_NUMBERS
}
