public enum OBJECTCOMMANDDEF
{
    OC_NONE						 = 0xFFFF,	// 无效ID

    // 休闲
    // F0	:	角色当前X坐标
    // F1	:	角色当前Z坐标
    OC_IDLE						 = 0,

    // 跳跃
    OC_JUMP						 = 1,
    //
    //// 寻路移动
    //// F0	:	角色当前X坐标
    //// F1	:	角色当前Z坐标
    //// P2	:	CPath指针
    //// D3	:	路径ID
    //#define OC_MOVE_USE_PATH			(2)
    //
    //// 移动
    //// F0	:	角色当前X坐标
    //// F1	:	角色当前Z坐标
    //// D2	:	路径点数目
    //// P3	:	路径点列表指针
    //// D4	:	路径ID
    //#define OC_MOVE_USE_NODE_LIST		(3)

    // 死亡
    // B0	:	是否播放死亡动画
     OC_DEATH					  = 4,

    // 瞬移
    // F0	:	角色当前X坐标
    // F1	:	角色当前Z坐标
     OC_TELEPORT				  = 5,
    //
    //// 技能(聚气)
    //// D0	:	技能ID
    //// D1	:	目标ID
    //// F2	:	目标点X坐标
    //// F3	:	目标点Z坐标
    //// F4	:	方向
    //// F5	:	聚气时间
    //// F6	:	使用者X坐标
    //// F7	:	使用者Z坐标
    //#define OC_SKILL_GATHER				(11)

    // 技能(聚气)
    // F0	:	减少的时间
     OC_SKILL_GATHER_MODIFY		 = 12,
    //
    //// 技能(引导)
    //// D0	:	技能ID
    //// D1	:	目标ID
    //// F2	:	目标点X坐标
    //// F3	:	目标点Z坐标
    //// F4	:	方向
    //// F5	:	引导时间
    //// F6	:	使用者X坐标
    //// F7	:	使用者Z坐标
    //#define OC_SKILL_LEAD				(13)

    // 技能(引导)
    // F0	:	减少的时间
     OC_SKILL_LEAD_MODIFY		 = 14,
    //
    //// 技能(发招)
    //// D0	:	技能ID
    //// D1	:	目标ID
    //// F2	:	目标点X坐标
    //// F3	:	目标点Z坐标
    //// F4	:	方向
    //// F5	:	发招时间
    //// F6	:	使用者X坐标
    //// F7	:	使用者Z坐标
    //// N8	:	逻辑计数
    //#define OC_SKILL_SEND				(15)

    // 技能(子弹)
    // D0	:	技能ID
    // D1	:	目标ID
    // F2	:	目标点X坐标
    // F3	:	目标点Z坐标
    // F4	:	方向
     OC_SKILL_CREATE_BULLET		    = 16,

    // 逻辑执行时产生的事件
    // P0	:	_LOGIC_EVENT指针
     OC_LOGIC_EVENT				    = 18,

    // 属性刷新
    // D0	:	更改了哪些属性
    // P1	:	所更改属性的值列表
    //#define OC_UPDATE_ATTRIB			(20)

    // 属性装备
    // D0	:	更改了哪些装备
    // P1	:	所更改装备的ID列表
     OC_UPDATE_EQUIPMENT			= 21,

    // 属性附加效果
    // D0	:	附加效果ID
    // B1	:	加或减(1=加，0=减)
    // N2	:	BUFF的创建者ID
     OC_UPDATE_IMPACT			    =22,

    // 属性附加效果(无需表现触发特效)
    // D0	:	附加效果ID
     OC_UPDATE_IMPACT_EX			=23,
    //
    //// 进入某个生活技能的操作中
    //// D0	:   技能ID
    //#define OC_LIFE_ABILITE_ACTION		(24)

    // 更新队伍标记
    // B0	:	是否有队伍的标记
    // B1	:	是否是队长的标记
    // B2	:	队伍是否满员的标记
     OC_UPDATE_TEAM_FLAG		 = 25,

    // 队伍跟随列表
    // D0	:	列表人数
    // D1..7:	列表中存放的 GUID
     OC_TEAM_FOLLOW_MEMBER		 = 26,

    // 更新自己的队伍跟随标记
    // B0	:	是否正在跟随队长
     OC_UPDATE_TEAM_FOLLOW_FLAG	 = 27,
    //
    //// 中断移动
    //// F0	:	坐标X
    //// F1	:	坐标Z
    //// D3	:	路径点ID
    //#define OC_ARRIVE_MOVE				(28)

    // 升级
     OC_LEVEL_UP				= 29,

    // 法术OBJ触发
    // N0		:		逻辑计数
    // N1		:		目标数目
    // P2		:		目标对象的列表
     OC_SPECIAL_OBJ_TRIGGER		= 30,

    // 法术OBJ灭亡
     OC_SPECIAL_OBJ_DIE			= 31,

    //////////////////////////////////////////////////////////////////////////
    // 添加BUS相关命令 [8/15/2011 ivan edit]

    // 往BUS上加入一个角色
    // NO		:		坐位索引
    // N1		:		乘客ID
     OC_BUS_ADD_PASSENGER		= 32,

    // 移除BUS上某个角色
    // N0		:		乘客ID
     OC_BUS_REMOVE_PASSENGER		=33,

    // 移除BUS上所有角色
     OC_BUS_REMOVE_ALL_PASSENGER	=34,

    // BUS移动
    // FO		:		目标点x坐标
    // F1		:		目标点y坐标
    // F2		:		目标点z坐标
     OC_BUS_MOVE		 = 35,

    // BUS移动停止
    // FO		:		中止点x坐标
    // F1		:		中止点y坐标
    // F2		:		中止点z坐标
     OC_BUS_STOP_MOVE	 = 36,
    //////////////////////////////////////////////////////////////////////////

    // 中断动作
    // N0		:	逻辑计数
    // U1		:	中断点
     OC_STOP_ACTION		 = 100,


    // 中断移动
    // N0		:	逻辑计数
    // N1		:	结束的路径节点索引
    // F2		:	结束的X位置
    // F3		:	结束的Z位置
     OC_STOP_MOVE		 = 101,

    // 中断坐下
    // N0		:	逻辑计数
     OC_STOP_SIT		 = 104,


    // 动作
    // U0		:	起始时间
    // N1		:	逻辑计数
    // N2		:	动作ID
     OC_ACTION			 = 200,

    // 移动
    // U0		:	起始时间
    // N1		:	逻辑计数
    // N2		:	节点数目
    // P3		:	节点列表指针(WORLD_POS*)
     OC_MOVE			 =201,

    // 法术发招
    // U0		:	起始时间
    // N1		:	逻辑计数
    // N2		:	法术ID
    // N3		:	目标角色ID
    // F4		:	目标X坐标
    // F5		:	目标Z坐标
    // F6		:	目标方向
     OC_MAGIC_SEND		 = 202,

    // 法术聚气
    // U0		:	起始时间
    // N1		:	逻辑计数
    // N2		:	法术ID
    // N3		:	目标角色ID
    // F4		:	目标X坐标
    // F5		:	目标Z坐标
    // F6		:	目标方向
    // U7		:	总进度时间
     OC_MAGIC_CHARGE	 = 203,

    // 法术引导
    // U0		:	起始时间
    // N1		:	逻辑计数
    // N2		:	法术ID
    // N3		:	目标角色ID
    // F4		:	目标X坐标
    // F5		:	目标Z坐标
    // F6		:	目标方向
    // U7		:	总进度时间
     OC_MAGIC_CHANNEL	 = 204 ,

    // 生活技能
    // U0		:	起始时间
    // N1		:	逻辑计数
    // N2		:	生活技能ID
    // N3		:	配方ID
    // N4		:	目标角色ID
     OC_ABILITY			= 205,

    // 干扰动作
    // N0		:	逻辑计数
    // N1		:	干扰时间值(增加或减少的时间)
     OC_MODIFY_ACTION	= 300
};