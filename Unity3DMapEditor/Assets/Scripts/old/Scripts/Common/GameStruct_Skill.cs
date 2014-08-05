///////////////////////////////////////////////////////////////////////////////////
//// 客户端与服务器共用的DBC结构

//// 技能伤害结构
//typedef _DBC_DIRECT_IMPACT	SDirectImpact;
//typedef _DBC_DIRECT_IMPACT	_DIRECT_IMPACT;
using System;
using System.Runtime.InteropServices;

using Network;
using Network.Packets;
// 技能BUFF结构
public class _DBC_BUFF_IMPACT
{
    public uint   m_uID;						// ID
    public uint   m_uMutexID;					// 互斥标记
    public int    m_nPri;						// 优先级参数
    public string m_szIconName;				    // 图标的文件名
    public string m_lpszEffect_Active;		    // 激活特效ID
    public string m_lpszSound_Active;			// 激活特效的音效ID
    public string m_lpszBind_Active;			// 激活特效的绑定点
    public string m_lpszEffect_Continuous;	    // 持续特效ID
    public string m_lpszSound_Continuous;		// 持续特效的音效ID
    public string m_lpszBind_Continuous;		// 持续特效的绑定点
    public bool   m_bStillOnWhenOwnerDead;	    // 主人死后是否保留
    public bool   m_bCanBeDispeled;			    // 是否可以被驱散
    public bool   m_bHostileFlag;				// 是否是负面效果
    public bool   m_bCanBeManualCancel;		    // 是否可以被手动取消
    public bool   m_bLineEffect;				// 是否为线性特效
    public string m_pszCreatorLocator;		    // 线性特效的目标绑定点
    public string m_pszInfo;					// 效果描述
};
//typedef _DBC_BUFF_IMPACT SBuffImpact;
//typedef _DBC_BUFF_IMPACT _BUFF_IMPACT;

//// 子弹轨迹类型
public enum ENUM_BULLET_CONTRAIL_TYPE
{
    BULLET_CONTRAIL_TYPE_INVALID = -1,
    BULLET_CONTRAIL_TYPE_BEELINE,			// 直线
    BULLET_CONTRAIL_TYPE_PARABOLA,			// 抛物线
    BULLET_CONTRAIL_TYPE_NONE,				// 无轨迹，直接爆炸

    BULLET_CONTRAIL_TYPE_NUMBERS
};

//// 子弹

//typedef _DBC_BULLET_DATA SSkillObjData;
//typedef _DBC_BULLET_DATA _SKILLOBJ_DATA;

//enum ENUM_SKILL_TYPE
//{
//    SKILL_TYPE_INVALID	= -1,
//    SKILL_TYPE_GATHER,	//技能释放前需要维持
//    SKILL_TYPE_LEAD,	//释放技能后需要维持的技能,吸血吸蓝....
//    SKILL_TYPE_LAUNCH,	//立即释放技能
//    SKILL_TYPE_PASSIVE,

//    SKILL_TYPE_NUMBERS
//};

public enum ENUM_SELECT_TYPE
{
    SELECT_TYPE_INVALID = -1,
    SELECT_TYPE_NONE,			// 无需选择
    SELECT_TYPE_CHARACTER,		// 角色
    SELECT_TYPE_POS,			// 位置
    SELECT_TYPE_DIR,			// 方向
    SELECT_TYPE_SELF,			// 对自己进行操作
    SELECT_TYPE_HUMAN_GUID,		// 玩家
    SELECT_TYPE_NUMBERS
};

//enum ENUM_TARGET_LOGIC
//{
//    TARGET_LOGIC_INVALID	= -1,
//    TARGET_SELF,	//只对自己有效
//    TARGET_MY_PET,  //只对自己的宠物有效
//    TARGET_MY_SHADOW_GUARD, //只对自己的分身有效
//    TARGET_MY_MASTER, //只对自己的主人有效，宠物专用
//    TARGET_AE_AROUND_SELF, //以自己为中心，范围有效
//    TARGET_SPECIFIC_UNIT, //瞄准的对象有效
//    TARGET_AE_AROUND_UNIT, //以瞄准的对象为中心，范围有效
//    TARGET_AE_AROUND_POSITION, //以瞄准的位置点为中心，范围有效
//    TARGET_LOGIC_NUMBERS //逻辑总数
//};

//enum ENUM_BEHAVIOR_TYPE
//{
//    BEHAVIOR_TYPE_HOSTILITY = -1, //敌对行为
//    BEHAVIOR_TYPE_NEUTRALITY = 0, //中立行为
//    BEHAVIOR_TYPE_AMITY = 1, //友好行为
//};
//// 招式类型
public enum ENUM_SKILL_ACTION_TYPE
{
    SKILL_ACTION_TYPE_INVALID = -1,
    SKILL_ACTION_TYPE_NONE,					// 普通招式(无序随机)
    SKILL_ACTION_TYPE_CONCATENATION_EX,		// 连招(1,2,2,2,2,3)
    SKILL_ACTION_TYPE_CONCATENATION,		// 连招(1,2,3,1,2,3)
    SKILL_ACTION_TYPE_JUMP,                 //需要跳跃
    SKILL_ACTION_TYPE_NUMBERS
};

//////技能模板结构
//#define MAX_SKILL_FRIEND_TMPACT		(2)
//#define MAX_SKILL_ENEMY_TMPACT		(2)

////{----------------------------------------------------------------------------
//// [2010-10-27] by: cfp+  "../../Public/ConfigSkillTemplate_V1.txt"表结构

////旧表
////// 技能 只是供客户端使用的技能模板结构
////struct _DBC_SKILL_DATA
////{
////	// 综合
////	INT				m_nID;								// ID
////	INT				m_nIDForManagement;					// 策划内部使用的管理性ID
////	INT				m_nMenPai;							// 门派ID
////	const CHAR		*m_lpszName;						// 名称
////	const CHAR		*m_lpszIconName;					// Icon名称
////	INT				m_nLevelRequirement;				// 技能的等级要求
////	INT				m_nSkillActionType;					// 招式类型ENUM_SKILL_ACTION_TYPE
////	INT				m_bMustUseWeapon; 					// 是否是必须使用武器的技能
////	INT				m_nDisableByFlag1; 					// 受限于限制标记1, 用于昏迷,魅惑等
////	INT				m_nDisableByFlag2; 					// 受限于限制标记2, 用于沉默类似
////	INT				m_nDisableByFlag3; 					// 受限于限制标记3, 用于变身骑乘
////	INT				m_nSkillClass;						// 技能系
////	INT				m_nXinFaParam_Nouse;				// 心法修正参数
////	INT				m_nRangeSkillFlag;					// 是否是远程技能
////	BOOL			m_bBreakPreSkill;					// 是否中断自己当前使用的技能
////	INT				m_nType;							// 技能类型 ENUM_SKILL_TYPE
////	INT				m_nCooldownID;						// 冷却组ID
////	const CHAR*     m_lpBeginEffect;					// 起手特效
////	const CHAR*     m_lpBeginEffectLocator;				// 起手特效绑定点
////	const CHAR*     m_lpBeginSound;						// 起手音效
////	const CHAR*		m_lpszGatherLeadActionSetID;		// 引导/聚气动作组ID 
////	const CHAR*		m_lpszSendActionSetID;				// 发招招式ID
////	INT				m_nEnvironmentSpecialEffect;		// 环境特效ID
////	INT				m_nTargetMustInSpecialState;		// 目标必须是， 0:活的；1:死的; -1:没有要求
////	INT				m_nClassByUser;						// 按使用者类型分类，0:玩家, 1:怪物, 2:宠物, 3:物品,
////	INT				m_nPassiveFlag;						// 主动还是被动技能，0:主动技能,1:被动技能;
////	INT				m_nSelectType;						// 点选类型 ENUM_SELECT_TYPE
////	INT				m_nOperateModeForPetSkill;			// 宠物技能专用，操作模式: PET_SKILL_OPERATEMODE
////	INT				m_nPetRateOfSkill; 					// 技能发动几率,只对宠物技能有效
////	INT				m_nTypeOfPetSkill; 					// 宠物技能类型,0:物功,1:法功,2:护主,3:防御,4:复仇;
////	ID_t			m_nImpactIDOfSkill;					// 宠物技能产生的效果ID
////	INT				m_nReserved1;						// 预留数据域1
////	INT				m_nReserved2;						// 预留数据域2
////	INT				m_nReserved3;						// 预留数据域3
////	INT				m_nReserved4;						// 预留数据域4
////	INT				m_nBulletID;						// 子弹ID
////	const CHAR		*m_pszBulletSendLocator;			// 子弹发出的绑定点
////	INT				m_nReserved5;						// 预留
////	INT				m_nTargetingLogic;					// 目标选择逻辑
////	INT				m_nSendTime;						// 发招时间(ms)
////	FLOAT			m_fMinAttackRange;					// 最小攻击距离(m)
////	FLOAT			m_fMaxAttackRange;					// 最大攻击距离(m)
////	INT				m_nFriendness;						// 技能友好度，=0为中性技能，>0为正面技能，<0为 负面技能
////	INT				m_nRelationshipLogic;				// 目标和使用者关系的合法性判断ID，参考相关子表格。
////	INT				m_nTargetCheckByObjType;			// 目标必须是某一指定的obj_type的角色
////	INT				m_nIsPartyOnly;						// 是否仅对队友有效。注：队友宠物算作队友。1为只对队友有效，0为无此限制。
////	INT				m_nHitsOrINTerval;					// 连击的攻击次数或引导的时间间隔
////	BOOL			m_bAutoRedo;						// 自动连续释放
////	INT				m_nHitRate;							// 命中率
////	INT				m_nCriticalRate; 					// 会心率
////	BOOL			m_bUseNormalAttackRate;				// 冷却时间是否受攻击速度影响
////	INT				m_nActiveTime;						// 激活时间
////	FLOAT			m_fDamageRange;						// 杀伤范围(m)
////	INT				m_nDamageAngle;						// 杀伤角度(0~360)
////	INT				m_nTargetNum;						// 影响目标的最大数
////	INT				m_nReserved7;						// 预留数据域5
////	INT				m_nCanInterruptAutoShot; 			// 是否能打断自动技能的连续释放
////	Time_t			m_nDelayTime; 						// 延迟时间
////	INT				m_anSkillByLevel[12];				// 级别对应的技能ID
////	CHAR			*m_pszDesc;							// 技能描述
////	INT             m_nUnKnow1;							//temp fix by sun
////	INT             m_nUnKnow2;
////	INT             m_nUnKnow3;
////	INT             m_nUnKnow4;
////};

////新表
//// 技能 客户端服务器公用的技能模板结构
//struct _DBC_SKILL_DATA
//{
//    // 综合
//    INT				m_nID;								// ID
//    INT				m_nIDForManagement;					// 策划内部使用的管理性ID
//    INT				m_nMenPai;							// 门派ID
//    const CHAR		*m_lpszName;						// 名称
//    const CHAR		*m_lpszIconName;					// Icon名称
//    INT				m_nLevelRequirement;				// 技能的等级要求
//    INT				m_nSkillActionType;					// 招式类型ENUM_SKILL_ACTION_TYPE
//    INT				m_bMustUseWeapon; 					// 是否是必须使用武器的技能
//    INT				m_nDisableByFlag1; 					// 受限于限制标记1, 用于昏迷,魅惑等
//    INT				m_nDisableByFlag2; 					// 受限于限制标记2, 用于沉默类似
//    INT				m_nDisableByFlag3; 					// 受限于限制标记3, 用于变身骑乘
//    INT				m_nSkillClass;						// 技能系
//    INT				m_nXinFaParam_Nouse;				// 心法修正参数
//    INT				m_nRangeSkillFlag;					// 是否是远程技能
//    BOOL			m_bBreakPreSkill;					// 是否中断自己当前使用的技能
//    INT				m_nType;							// 技能类型 ENUM_SKILL_TYPE
//    INT				m_nCooldownID;						// 冷却组ID
//    const CHAR*     m_lpBeginEffect;					// 起手特效
//    const CHAR*     m_lpBeginEffectLocator;				// 起手特效绑定点
//    const CHAR*     m_lpBeginSound;						// 起手音效
//    const CHAR*		m_lpszGatherLeadActionSetID;		// 引导/聚气动作组ID 
//    const CHAR*		m_lpszSendActionSetID;				// 发招招式ID
//    INT				m_nEnvironmentSpecialEffect;		// 环境特效ID
//    INT				m_nTargetMustInSpecialState;		// 目标必须是， 0:活的；1:死的; -1:没有要求
//    INT				m_nClassByUser;						// 按使用者类型分类，0:玩家, 1:怪物, 2:宠物, 3:物品,
//    INT				m_nPassiveFlag;						// 主动还是被动技能，0:主动技能,1:被动技能;
//    INT				m_nSelectType;						// 点选类型 ENUM_SELECT_TYPE
//    INT				m_nOperateModeForPetSkill;			// 宠物技能专用，操作模式: PET_SKILL_OPERATEMODE
//    INT				m_nPetRateOfSkill; 					// 技能发动几率,只对宠物技能有效
//    INT				m_nTypeOfPetSkill; 					// 宠物技能类型,0:物功,1:法功,2:护主,3:防御,4:复仇;
//    ID_t			m_nImpactIDOfSkill;					// 宠物技能产生的效果ID
//    INT				m_nGroupID;							// 技能组ID
//    INT				m_nGroupLevel;						// 技能组等级
//    INT				m_nLearnDir;						// 是否直接学会
//    INT				m_nUseInLearned;					// 使用前是否需要学会
//    INT				m_nBulletID;						// 子弹ID
//    const CHAR		*m_pszBulletSendLocator;			// 子弹发出的绑定点
//    INT				m_nReserved5;						// 预留
//    INT				m_nTargetingLogic;					// 目标选择逻辑 ENUM_TARGET_LOGIC
//    INT				m_nSendTime;						// 发招时间(ms),动作时间
//    FLOAT			m_fMinAttackRange;					// 最小攻击距离(m)
//    FLOAT			m_fMaxAttackRange;					// 最大攻击距离(m)
//    INT				m_nFriendness;						// 技能友好度，=0为中性技能，>0为正面技能，<0为 负面技能
//    INT				m_nRelationshipLogic;				// 目标和使用者关系的合法性判断ID，参考相关子表格。
//    INT				m_nTargetCheckByObjType;			// 目标必须是某一指定的obj_type的角色
//    INT				m_nIsPartyOnly;						// 是否仅对队友有效。注：队友宠物算作队友。1为只对队友有效，0为无此限制。
//    INT				m_nHitsOrINTerval;					// 连击的攻击次数或引导的时间间隔
//    BOOL			m_bAutoRedo;						// 自动连续释放
//    INT				m_nHitRate;							// 命中率
//    INT				m_nCriticalRate; 					// 会心率
//    BOOL			m_bUseNormalAttackRate;				// 冷却时间是否受攻击速度影响
//    INT				m_nActiveTime;						// 激活时间
//    FLOAT			m_fDamageRange;						// 杀伤范围(m)
//    INT				m_nDamageAngle;						// 杀伤角度(0~360)
//    INT				m_nTargetNum;						// 影响目标的最大数
//    INT				m_nCanInterruptPetAttack;			// 中断或激活珍兽攻击
//    INT				m_nCanInterruptAutoShot; 			// 是否能打断自动技能的连续释放
//    Time_t			m_nDelayTime; 						// 延迟时间
//    INT				m_anSkillByLevel[12];				// 心法级别对应的技能ID
//    CHAR			*m_pszDesc;							// 技能描述
//    INT             m_nShowDetail;						// 是否显示打击名
//    INT             m_nUnKnow[3];						//干扰时间百分比\干扰几率\干扰时间浮动百分比

//};

//typedef _DBC_SKILL_DATA SSkillData;
//typedef _DBC_SKILL_DATA _SKILL_DATA;
////----------------------------------------------------------------------------}

/////////////////////////////////////////////////////////////////////////
//// 心法
//struct _DBC_XINFA_DATA
//{
//    //
//    UINT		m_uID;							// ID
//    UINT		m_uIDMenpai;					// 门派ID
//    const CHAR	*m_lpszName;					// 名称
//    CHAR		*m_pszDesc;						// 技能描述
//    const CHAR	*m_lpszIconName;				// Icon名称
//};
/////////////////////////////////////////////////////////////////////////
//struct _SKILL_EXPERIENCE
//{
//    BYTE		m_SkillLevel;	//技能等级
//    BYTE		m_SkillExp;		//技能熟练度
//    USHORT		m_SkillPoints;	//技能使用次数
//};
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _OWN_SKILL
{
    public short		m_nSkillID;			//拥有的技能ID
    public uint		m_nSkillTime;		//一个INT，但是目前没有使用，可以存盘
    public void SetSkillLevel(byte nLevel)
    {
    //    Assert(nLevel<=MAX_SKILL_LEVEL);
        m_nSkillTime |= nLevel;
    }
    public byte	GetSkillLevel()
    {
        return (byte)(m_nSkillTime & 0x000000ff );
    }

    public void	SetSkillExp(byte nSkillExp)
    {
        uint temp = (uint)nSkillExp;
        temp = temp << 8;
        m_nSkillTime |= temp;
    }

    public byte	GetSkillExp()
    {
        byte ret = (byte)((m_nSkillTime & 0x0000ff00) >> 8);
        return ret;
    }

    public void	SetSkillPoints(ushort nSkillPoints)
    {
        uint temp = (uint)nSkillPoints;
        temp = temp << 16;
        m_nSkillTime |= temp;
    }
    public ushort	GetSkillPoints()
    {
        ushort ret = (ushort)((m_nSkillTime & 0xffff0000) >> 16);
        return ret;
    }

    public int getSize()
    {
        return sizeof(short) + sizeof(uint);
    }
    public static int GetMaxSize()
    {
        return sizeof(short) + sizeof(uint);
    }
    public int writeToBuff(ref NetOutputBuffer buff)
    {

        buff.WriteFloat(m_nSkillID);
        buff.WriteFloat(m_nSkillTime);

        return getSize();
    }
    public bool readFromBuff(ref NetInputBuffer buff)
    {

        if (buff.ReadShort(ref m_nSkillID) != sizeof(short)) return false;
        if (buff.ReadUint(ref m_nSkillTime) != sizeof(uint)) return false;
        return true;
    }
};

////这个结构暂时不用了。。。。
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _OWN_XINFA
{
    
    public short m_nXinFaID ;		//拥有的心法ID
    public short m_nXinFaLevel;			//心法等级
    public int getSize()
    {
        return sizeof(short) + sizeof(short);
    }
    public static int GetMaxSize()
    {
        return sizeof(short) + sizeof(short);
    }
    public int writeToBuff(ref NetOutputBuffer buff)
    {

        buff.WriteFloat(m_nXinFaID);
        buff.WriteFloat(m_nXinFaLevel);
        return getSize();
    }
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (buff.ReadShort(ref m_nXinFaID) != sizeof(short)) return false;
        if (buff.ReadShort(ref m_nXinFaLevel) != sizeof(short)) return false;
        return true;
    }
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct DetailImpactStruct_T
{
    public int m_nSenderID;	// 效果释放者的ID
    public short m_nBuffID;		// 特效数据的ID(索引)
    public short m_nSkillID;  	// Skill ID
    public uint m_nSN;			// 效果序列号
    public int m_nContinuance; // 效果的持续时间
    public int getSize()
    {
        return sizeof(short) * 2 + sizeof(int) * 3;
    }
    public static int GetMaxSize()
    {
        return sizeof(short) * 2 + sizeof(int) * 3;
    }
    public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteInt(m_nSenderID);
        buff.WriteShort(m_nBuffID);
        buff.WriteShort(m_nSkillID);
        buff.WriteUint(m_nSN);
        buff.WriteInt(m_nContinuance);
        return getSize();
    }
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (buff.ReadInt(ref m_nSenderID) != sizeof(int)) return false;
        if (buff.ReadShort(ref m_nBuffID) != sizeof(short)) return false;
        if (buff.ReadShort(ref m_nSkillID) != sizeof(short)) return false;
        if (buff.ReadUint(ref m_nSN) != sizeof(uint)) return false;
        if (buff.ReadInt(ref m_nContinuance) != sizeof(int)) return false;
        return true;
    }
};



//////////////////////////////////////////////
//// 伤害的结构
//#define DAMAGE_INFO_PARAM_NUMBER	(8)

public class _DAMAGE_INFO
{
    public enum DAMAGETYPE
    {
        TYPE_INVALID         = MacroDefine.INVALID_ID,
        TYPE_EFFECT          = 0,
        TYPE_HEAL_AND_DAMAGE = 1,
        TYPE_DROP_BOX        = 2,
        TYPE_SKILL_TEXT      = 3,
        TYPE_DIE             = 4
    };
    public short        m_nSkillID;				// 技能ID
    public int          m_nBulletID;			// 子弹ID
    public uint         m_nTargetID;			// 目标对象的ID
    public uint         m_nSenderID;			// 攻击者的ID
    public int          m_nSenderLogicCount;	// 攻击者的逻辑计数
    public short        m_nImpactID;			// 效果ID //参考GameStruct_Impact.h的DIRECT_IMPACT_SEID
    public DAMAGETYPE   m_nType;				// 效果、伤害和治疗数值、掉落盒
    public int          m_nHealthIncrement;
    public bool         m_bHealthDirty;
    public int          m_nManaIncrement;
    public bool         m_bManaDirty;
    public int          m_nRageIncrement;
    public bool         m_bRageDirty;
    public int          m_nStrikePointIncrement;
    public bool         m_bStrikePointDirty;
    public bool         m_bIsCriticalHit;
    const int           DAMAGE_INFO_PARAM_NUMBER = 8;
    public int[]        m_aAttachedParams = new int[_DAMAGE_INFO.DAMAGE_INFO_PARAM_NUMBER]; // 附加的参数

    public _DAMAGE_INFO()
    {
        Reset();
    }

    public void Reset()
    {
        m_nSkillID				= MacroDefine.INVALID_ID;
        m_nBulletID				= MacroDefine.INVALID_ID;
        m_nTargetID				= MacroDefine.UINT_MAX;
        m_nSenderID				= MacroDefine.UINT_MAX;
        m_nSenderLogicCount		= -1;
        m_nImpactID				= MacroDefine.INVALID_ID;
        m_nType					= DAMAGETYPE.TYPE_INVALID;
        m_nHealthIncrement		= 0;
        m_bHealthDirty			= false;
        m_nManaIncrement		= 0;
        m_bManaDirty			= false;
        m_nRageIncrement		= 0;
        m_bRageDirty			= false;		
        m_nStrikePointIncrement	= 0;
        m_bStrikePointDirty		= false;
        m_bIsCriticalHit		= false;
    }
};

//////////////////////////////////////////////
//// 技能BUFF的结构
//#define BUFF_IMPACT_INFO_PARAM_NUMBER	(8)

public class _BUFF_IMPACT_INFO
{
    public uint     m_nReceiverID;										// 效果接受者的ID
    public uint     m_nSenderID;										// 效果释放者的ID
    public short    m_nBuffID;											// 特效数据的ID(索引)
    public short    m_nSkillID;  										// Skill ID
    public int      m_nSenderLogicCount;								// 效果创建者的逻辑计数
    public uint     m_nSN;												// 效果序列号
    public int      m_nTimer;											// 剩余时间

    public _BUFF_IMPACT_INFO()
    {
        Reset();
    }

    public void Reset()
    {
        m_nReceiverID			= MacroDefine.UINT_MAX;
        m_nSenderID				= MacroDefine.UINT_MAX;
        m_nBuffID				= MacroDefine.INVALID_ID;
        m_nSkillID				= MacroDefine.INVALID_ID;
        m_nSenderLogicCount		= -1;
        m_nSN					= MacroDefine.UINT_MAX;
        m_nTimer				= 0;
    }
};

//// 传说中的拉人的信息结构
//struct _CALLOF_INFO
//{
//    GUID_t			m_guidCaller;	// 召唤者的GUID
//    SceneID_t		m_SceneID;		// 场景ID
//    WORLD_POS		m_Pos;			// 场景位置
//    UINT			m_uDuration;	// 持续时间

//    _CALLOF_INFO( VOID )
//    {
//        m_guidCaller		= INVALID_GUID;
//        m_SceneID			= INVALID_ID;
//        m_Pos				= WORLD_POS( -1.f, -1.f );
//        m_uDuration			= 0;
//    }

//    VOID Reset( VOID )
//    {
//        m_guidCaller		= INVALID_GUID;
//        m_SceneID			= INVALID_ID;
//        m_Pos				= WORLD_POS( -1.f, -1.f );
//        m_uDuration			= 0;
//    }
//};

//cooldown
public class Cooldown_T
{
//public:
//    Cooldown_T(VOID): m_nID(INVALID_ID), m_nCooldown(0), m_nCooldownElapsed(0){};
//    Cooldown_T(Cooldown_T const& rhs)
//    {
//        m_nID = rhs.GetID();
//        m_nCooldown = rhs.GetCooldownTime();
//        m_nCooldownElapsed = rhs.GetCooldownElapsed();
//    };
//    ~Cooldown_T(VOID){}
//    Cooldown_T& operator=(Cooldown_T const& rhs)
//    {
//        m_nID = rhs.GetID();
//        m_nCooldown = rhs.GetCooldownTime();
//        m_nCooldownElapsed = rhs.GetCooldownElapsed();
//        return *this;
//    };
//    VOID HeartBeat(Time_t nDeltaTime)
//    {
//        if(0>m_nID)
//        {
//            return;
//        }
//        if(m_nCooldown<=m_nCooldownElapsed)
//        {
//            return;
//        }
//        m_nCooldownElapsed +=nDeltaTime;
//        if(m_nCooldown<m_nCooldownElapsed)
//        {
//            m_nCooldown=m_nCooldownElapsed;
//        }
//    };
//    BOOL IsCooldowned(VOID) const
//    {
//        return m_nCooldown<=m_nCooldownElapsed;
//    };
//    Time_t GetRemainTime(VOID) const
//    {
//        return m_nCooldown - m_nCooldownElapsed;
//    };
//    VOID Reset(VOID)
//    {
//        m_nID = INVALID_ID;
//        m_nCooldown = 0;
//        m_nCooldownElapsed = 0;
//    };
//    ID_t GetID(VOID) const {return m_nID;}
//    VOID SetID(ID_t nID) {m_nID = nID;}
//    Time_t GetCooldownTime(VOID) const {return m_nCooldown;}
//    VOID SetCooldownTime(Time_t nTime) {m_nCooldown = nTime;}
//    Time_t GetCooldownElapsed(VOID) const {return m_nCooldownElapsed;}
//    VOID SetCooldownElapsed(Time_t nTime){m_nCooldownElapsed = nTime;}
	public Cooldown_T()
	{
		m_nID = MacroDefine.INVALID_ID;
		m_nCooldown = 0;
		m_nCooldownElapsed = 0;
	}
    public int getSize()
    {
        return sizeof(short) + sizeof(int) * 2;
    }

    public static int getMaxSize()
    {
        return sizeof(short) + sizeof(int) * 2;
    }


    public short m_nID;
    public int m_nCooldown;
    public int m_nCooldownElapsed;
};
//template <INT nSize>
//class CooldownList_T
//{
//public:
//    enum
//    {
//        LIST_SIZE = nSize,
//    };
//    CooldownList_T(VOID){}
//    CooldownList_T(CooldownList_T const& rhs)
//    {
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            m_aCooldown[nIdx] = rhs.GetCooldownInfoByIndex(nIdx);
//        }
//    }
//    ~CooldownList_T(VOID){}
//    VOID CleanUp(VOID) {Reset();}
//    VOID Reset(VOID)
//    {
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            m_aCooldown[nIdx].Reset();
//        }
//    }
//    CooldownList_T& operator=(CooldownList_T const& rhs)
//    {
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            m_aCooldown[nIdx] = rhs.GetCooldownInfoByIndex(nIdx);
//        }
//        return *this;
//    }
//    Cooldown_T const& GetCooldownInfoByIndex(INT nIdx) const
//    {
//        if(0>nIdx || LIST_SIZE<=nIdx)
//        {
//            AssertEx(FALSE,"[CooldownList_T::GetCooldownByIndex]:Illegal index found!!");
//            return m_aCooldown[0];
//        }
//        return m_aCooldown[nIdx];
//    }
//    Time_t GetRemainTimeByID(CooldownID_t nCooldown) const
//    {
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            if(m_aCooldown[nIdx].GetID()==nCooldown)
//            {
//                return m_aCooldown[nIdx].GetRemainTime();
//            }
//        }
//        return TRUE;
//    }
//    INT GetSlotSize(VOID)
//    {
//        return LIST_SIZE;
//    }
//    INT GetByteSize(VOID)
//    {
//        return sizeof(CooldownList_T);
//    }
//    VOID HeartBeat(Time_t nDeltaTime)
//    {
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            m_aCooldown[nIdx].HeartBeat(nDeltaTime);
//        }
//    }
//    BOOL IsSpecificSlotCooldownedByID(CooldownID_t nCooldown) const
//    {
//        if(0 > nCooldown)
//        {
//            return TRUE;
//        }
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            if(m_aCooldown[nIdx].GetID()==nCooldown)
//            {
//                return m_aCooldown[nIdx].IsCooldowned();
//            }
//        }
//        return TRUE;
//    }
//    VOID RegisterCooldown(CooldownID_t nCooldown, Time_t nCooldownTime)
//    {
//        INT nEmptySlot = INVALID_ID;
//        if(0 > nCooldown)
//        {
//            return;
//        }
//        for(INT nIdx=0;LIST_SIZE>nIdx;++nIdx)
//        {
//            if(INVALID_ID==nEmptySlot)
//            {
//                if(INVALID_ID==m_aCooldown[nIdx].GetID())
//                {
//                    nEmptySlot = nIdx;
//                }
//                else if(TRUE==m_aCooldown[nIdx].IsCooldowned())
//                {
//                    nEmptySlot = nIdx;
//                }
//            }
//            if(m_aCooldown[nIdx].GetID()==nCooldown)
//            {
//                m_aCooldown[nIdx].Reset();
//                m_aCooldown[nIdx].SetID(nCooldown);
//                m_aCooldown[nIdx].SetCooldownTime(nCooldownTime);
//                return;
//            }
//        }
//        if(INVALID_ID!=nEmptySlot)
//        {
//            m_aCooldown[nEmptySlot].Reset();
//            m_aCooldown[nEmptySlot].SetID(nCooldown);
//            m_aCooldown[nEmptySlot].SetCooldownTime(nCooldownTime);
//            return;
//        }
//        AssertEx(FALSE, "[CooldownList_T::RegisterCooldown]: Cooldown list full!!");
//    }
//protected:
//private:
//    Cooldown_T m_aCooldown[LIST_SIZE];
//};

//enum MissFlag_T //empty class for wrap constants
public enum MissFlag_T
{
    FLAG_NORMAL = 0,
    FLAG_MISS,
    FLAG_IMMU,
    FLAG_ABSORB,
    FLAG_COUNTERACT,
    FLAG_TRANSFERED,
};

//enum OtherMsgInfo

public enum OtherMsgInfo
{
    OM_ENTER_FIGHT = 0,
    OM_LEAVE_FIGHT = 1,
    OM_ENTER_WORLD = 2,//进入地图显示地图名字
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _PET_DETAIL_ATTRIB
{
    public PET_GUID_t		m_GUID;							// ID
	public int			    m_ObjID;						// 所有Obj类型的ObjID
	public int				m_nDataID;						// 模型ID,宠物类型
	public byte[]			m_szName;	// 名称
	public int				m_nAIType;						// 性格
	public PET_GUID_t		m_SpouseGUID;					// 配偶的GUID
	public int				m_nLevel;						// 等级
	public int				m_nExp;							// 经验
	public int				m_nHP;							// 血当前值
	public int				m_nHPMax;						// 血最大值

	public int				m_nLife;						// 当前寿命
	public byte			    m_byGeneration;					// 几代宠
    public byte             m_byHappiness;					// 快乐度

	public int				m_nAtt_Physics;					// 物理攻击力
	public int				m_nAtt_Magic;					// 魔法攻击力
	public int				m_nDef_Physics;					// 物理防御力
	public int				m_nDef_Magic;					// 魔法防御力

	public int				m_nHit;							// 命中率
	public int				m_nMiss;						// 闪避率
	public int				m_nCritical;					// 会心率

	public int				m_nModelID;						// 外形
	public int				m_nMountID;						// 座骑ID

	public int				m_StrPerception;				// 力量资质
	public int				m_ConPerception;				// 体力资质
	public int 			    m_DexPerception;				// 身法资质
	public int				m_SprPerception;				// 灵气资质
	public int 			    m_IntPerception;				// 定力资质

	public int				m_Str;							// 力量
	public int				m_Con;							// 体力
	public int 			    m_Dex;							// 身法
	public int				m_Spr;							// 灵气
	public int 			    m_Int;							// 定力
	public int 			    m_GenGu;						// 根骨

	public int				m_nRemainPoint;					// 潜能点

    public _OWN_SKILL[] m_aSkill;	// 技能列表
    static public int getMaxSize()
    {
        return sizeof(int) * 29 + 
               sizeof(byte) * (2 + GAMEDEFINE.MAX_CHARACTER_NAME) + 
               PET_GUID_t.getMaxSize() * 2 + 
               _OWN_SKILL .GetMaxSize()* GAMEDEFINE.MAX_PET_SKILL_COUNT;
    }

    public void CleanUp()
    {
        m_GUID.Reset();
        m_ObjID     = MacroDefine.INVALID_ID;
        m_nDataID   = MacroDefine.INVALID_ID;
        m_SpouseGUID.Reset();
        for (int i = 0; i < GAMEDEFINE.MAX_PET_SKILL_COUNT; i++)
        {
            m_aSkill[i].m_nSkillID = MacroDefine.INVALID_ID;
        }
    }

    public bool readFromBuff(ref NetInputBuffer buff)
    {
        m_szName = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];
        m_aSkill = new _OWN_SKILL[GAMEDEFINE.MAX_PET_SKILL_COUNT];
        if (!m_GUID.readFromBuff(ref buff)) return false;
        if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
        if (buff.ReadInt(ref m_nDataID) != sizeof(int)) return false;
        if (buff.Read(ref m_szName, GAMEDEFINE.MAX_CHARACTER_NAME) != GAMEDEFINE.MAX_CHARACTER_NAME) return false;
        if (buff.ReadInt(ref m_nAIType) != sizeof(int)) return false;
        if (!m_SpouseGUID.readFromBuff(ref buff)) return false;
        if (buff.ReadInt(ref m_nLevel) != sizeof(int)) return false;					// 等级
	    if (buff.ReadInt(ref m_nExp)!= sizeof(int)) return false;							// 经验
	    if (buff.ReadInt(ref m_nHP)!= sizeof(int)) return false;							// 血当前值
	    if (buff.ReadInt(ref m_nHPMax)!= sizeof(int)) return false;
        if (buff.ReadInt(ref m_nLife) != sizeof(int)) return false;
        if (buff.ReadByte(ref m_byGeneration) != sizeof(byte)) return false;
        if (buff.ReadByte(ref m_byHappiness) != sizeof(byte)) return false;
        if (buff.ReadInt(ref m_nAtt_Physics) != sizeof(int)) return false;
	    if (buff.ReadInt(ref m_nAtt_Magic) != sizeof(int)) return false;
	    if (buff.ReadInt(ref m_nDef_Physics)!= sizeof(int)) return false;
	    if (buff.ReadInt(ref m_nDef_Magic)!= sizeof(int)) return false;

	    if (buff.ReadInt(ref m_nHit)!= sizeof(int)) return false;
	    if (buff.ReadInt(ref m_nMiss)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_nCritical)!=  sizeof(int)) return false;

	    if (buff.ReadInt(ref m_nModelID)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_nMountID)!=  sizeof(int)) return false;

	    if (buff.ReadInt(ref m_StrPerception)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_ConPerception)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_DexPerception)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_SprPerception)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_IntPerception)!=  sizeof(int)) return false;

	    if (buff.ReadInt(ref m_Str)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_Con)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_Dex)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_Spr)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_Int)!=  sizeof(int)) return false;
	    if (buff.ReadInt(ref m_GenGu)!=  sizeof(int)) return false;
        if (buff.ReadInt(ref m_nRemainPoint) != sizeof(int)) return false;
        for (int i = 0; i < GAMEDEFINE.MAX_PET_SKILL_COUNT; i++)
        {
            if(!m_aSkill[i].readFromBuff(ref buff))return false;
        }
        return true;
    }

    public int getSize()
    {
        return sizeof(int) * 29 +
               sizeof(byte) * (2 + GAMEDEFINE.MAX_CHARACTER_NAME) +
               PET_GUID_t.getMaxSize() * 2 +
               _OWN_SKILL.GetMaxSize() * GAMEDEFINE.MAX_PET_SKILL_COUNT;
    }
};

//public:
//    INT m_nType;
//};
//#endif
