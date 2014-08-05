
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
//								主角相关								//
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//

/////////////////////////////////////////////////////////////////////
//主角种族							(CHAR_01)
// 种族编号规则 [12/13/2010 ivan edit]
// nID奇数为男，偶数为女
/////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;

public struct DBC_DEFINE
{
    public const int CHAR_RACE_NUM = 2;

    public const int MAX_WEAPON_TYPE_NUMBER = 9;
    public const int MAX_MOUNT_NUMBER = 32;
    //物品定义表_装备表					(ITEM_03)
    // [2010-11-4] by: cfp+ 
    public const int MAX_BASE_ATTR = (5);	//装备基础属性（如：物攻，魔攻等）
    ////#define MAX_ADD_ATTR	(54)	//装备表最大附加属性
    public const int MAX_SOUL_ATTR = (7);		// 装备的魂印属性数量 [10/11/2011 edit by ZL]
    public const int MAX_GEM_ATTR = (0);	// 装备的魂印属性数量 [10/11/2011 edit by ZL]
    public const int MAX_ADD_ATTR = (22);	//装备表最大附加属性 /*28-6*/

    public const int MAX_PINJI_NUMBER = (5);//// 装备表修改 [9/15/2011 zzh+]
    public const int MAX_DANGCI_NUMBER = (10);//// 装备表修改 [9/15/2011 zzh+]
    public const int MAX_QUALITY_NUMBER = (5);//// 装备表修改 [9/15/2011 zzh+]
    public const int LEVEL_NUMER_PER_DANGCI = (10);//// 装备表修改 [9/15/2011 zzh+]
}

public class _DBC_CHAR_RACE
{
    public int nID;
    public int nGender;
    public int nIsPlayer;
    public int nModelID;
    public int nDefHeadGeo;
    public int nDefHairGeo;
    public int nDefBody;
    public int nDefArm;
    public int nDefFoot;
    public int nIdleInterval;
    public int nDefRWeapon;
}

/////////////////////////////////////////////////////////////////////
//主角头发模型						(CHAR_02)
/////////////////////////////////////////////////////////////////////

public class _DBC_CHAR_HAIR_GEO
{
    public int nID;
    public int nRace;
    public string pMeshFile;
    public string pShowName;
    public string pMaterialName;
    public int nItemID;
    public int nItemNum;
    public int nSelect;
    public string pIcon;
    public int nMoney;
}

/////////////////////////////////////////////////////////////////////
//主角脸部模型						(CHAR_04)
/////////////////////////////////////////////////////////////////////

public class _DBC_CHAR_HEAD_GEO
{
    public int nID;
    public int nRace;
    public string pMeshFile;
    public string pShowName1;//modify by RM 2012-1-4
    public string pShowName2;//added by RM 2012-1-4
}

/////////////////////////////////////////////////////////////////////
//主角动作组						(CHAR_06)
/////////////////////////////////////////////////////////////////////

public class _DBC_CHARACTER_ACTION_SET
{
    public int nID;
    public string[] pWeapon_Set = new string[DBC_DEFINE.MAX_WEAPON_TYPE_NUMBER];
    public bool bHideWeapon;
    public int nAppointedWeaponID;		// 指定的武器ID
    public string pszDesc;
}

/////////////////////////////////////////////////////////////////////
//主角特效							(CHAR_07)
/////////////////////////////////////////////////////////////////////

public class _DBC_CHARACTER_EFFECT
{
    public int nID;
    public int nEffect1;
    public int nEffect2;
    public int nSoundID;
    public string pLocator;
    public string pWeaponLocator;
}

/////////////////////////////////////////////////////////////////////
//主角升级经验值						(CHAR_08)
/////////////////////////////////////////////////////////////////////

public class _DBC_CHARACTER_EXPLEVEL
{
    public int nID;
    public int nEffectNeed;
}

/////////////////////////////////////////////////////////////////////
//外形ID对应模型名称					(CHAR_09)
/////////////////////////////////////////////////////////////////////

public class _DBC_CHARACTER_MODEL
{

    public int m_nID;
    public string m_pszModelName;
    public float m_fFuseTime;				// 动作熔合时间
    public float m_fFuseTime1;				////ZZH+
    public float m_fFuseTime2;				////ZZH+
    public float m_fFuseTime3;				////ZZH+
    public ulong[] reserve = new ulong[3];/**********************************/
    public float m_scale;//zzy+
    public string m_materialName;//zzy+
    public string m_pszActionSetName_None;
    public string[] m_apszActionSetName_Mount = new string[DBC_DEFINE.MAX_MOUNT_NUMBER];
}

/////////////////////////////////////////////////////////////////////
//主角头像
/////////////////////////////////////////////////////////////////////

public class _DBC_CHAR_FACE
{
    public int nID;
    public int nRace;
    public string pImageSetName;

    public int n3or2; ////zzh+
}



/************************************************************************/
/* 
	//  [12/14/2010 ivan edit]
	主角初始套装
*/
/************************************************************************/

public class _DBC_CHARACTER_EQUIPSET
{
    public int nID;			//ID	
    public string pName;			//套装名称
    public int nSex;			//套装归属性别,-1通用
    public int nWeapon;		//武器
    public int nCap;			//帽子
    public int nArmor;			//盔甲
    public int nCuff;			//护腕
    public int nBoot;			//鞋子
    public int nSash;			//腰带
    public int nRing;			//戒指
    public int nNeckLace;		//项链
    public int nRider;			//骑乘
    public int nBack;			//背饰
}
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
//								生物相关								//
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//

/////////////////////////////////////////////////////////////////////
//生物定义总表						(CREATURE_01)
/////////////////////////////////////////////////////////////////////

public class _DBC_CREATURE_ATT
{
    public int nID;				//怪物编号
    public string pName;				//名称
    public int nSex;				//性别
    public int nLevel;				//等级	
    public int nExp;				//基础经验获得
    public int nFriendValue;		//友好值
    public int nIdleInterval;		//休闲间隔时间
    public int nBaseAI;			//基础AI
    public int nExtAI;				//扩展AI
    public int nCamp;				//阵营
    public int nDropSearchRange;	//掉落分配半径(米)
    public int nDropTeamCount;		//掉落最大有效组
    public int nMinDamagePercent;	//最小伤血百分比
    public int nCanHandle;			//是否可以交互
    public int nMonsterBossFlag;	//BOSS标记
    public int nAttackRate_P;		//物理攻击
    public int nDefence_P;			//物理防御
    public int nAttackRate_M;		//魔法攻击
    public int nDefence_M;			//魔法防御
    public int nMaxHP;				//HP上限
    public int nMaxMP;				//MP上限
    public int nRestoreHP;			//HP回复
    public int nRestoreMP;			//MP回复
    public int nHit;				//命中率
    public int nMiss;				//闪避
    public int nCritrate;			//会心率
    public int nCritrate_Defence;	//会心防御
    public int nSpeed;				//移动速度
    public int nMonsterWalkSpeed;	//步行速度
    public int nAttackSpeed;		//攻击速度

    public int nAttackCold;		//冰攻击
    public int nDefenceCold;		//冰防御

    public int nAttackFire;		//火攻击
    public int nDefenceFire;		//火防御

    public int nAttackLight;		//电攻击
    public int nDefenceLight;		//电防御

    public int nAttackPoison;		//毒攻击
    public int nDefencePoison;		//毒防御

    public int nAttackEarth;		//土攻击
    public int nDefenceEarth;		//土防御
    public int nMonsterBossSnapImmID;      //瞬时效果免疫ID
    public int nMonsterBossDurationImmID;  //持续效果免疫ID
    public int nModelID;				// 外型ID 以前是string	szModeleFile;
    public int nIsDisplayerName;		// 是否显示头顶信息板
    public int nIsCanSwerve;			// 是否随玩家点选转向
    public float fBoardHeight;			// 头顶名字板高度
    public float fProjTexRange;			// 选中环大小
    public float fShadowRange;			// 是否显示阴影，阴影大小
    public string szIconName;				// 头像图标

    public int nReserve1;				// 攻击动作时间
    public int nReserve2;				// 攻击冷却时间
    public int nReserve3;				// MaxLv
    public int nReserve4;				// MaxExp
    public int nReserve5;				// MaxAtt
    public int nReserve6;				// MaxDef
    public int nReserve7;				// MaxMag
    public int nReserve8;				// MaxRes
    public int nReserve9;				// MaxHP
    public int nReserve10;				// 是否在小地图上显示
    public int nReserve11;				// 势力ID
    public int nReserve12;				// 攻击特性ID
    public int nReserve13;				// 是否攻击NPC
    public int nReserve14;				// 是否播放融合，闪避，搁挡动作
    public int nReserve15;				// 外观类型
    public int nReserve16;				// 强度类型
    public int nReserve17;				// 交互方式
    public int nReserve18;				// 激活时间
    public int nCannotBeAttack;		// 战斗方式
    public float effectHeight;			//特效挂接点所占的高度
    public string szEffectName;			//特效名称
}


//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
//								物品相关								//
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//

/////////////////////////////////////////////////////////////////////
//装备类物品显示定义				(ITEM_01)
/////////////////////////////////////////////////////////////////////

public class _DBC_ITEM_VISUAL_CHAR
{
    public int nID;
    public string[] pVisualEntity = new string[2 * DBC_DEFINE.CHAR_RACE_NUM];
    public string[] pEffectName = new string[7];// 增加装备的宝石和强化特效，前7个对应宝石特效，后3个强化特效 [10/25/2011 Sun]
    public string[] pStrengthenEffectName = new string[3];
}

/////////////////////////////////////////////////////////////////////
//挂接类物品显示定义				(ITEM_02)
/////////////////////////////////////////////////////////////////////

public class _DBC_ITEM_VISUAL_LOCATOR
{
    public int nID;
    public string[] pVisvalLocatorR = new string[2 * DBC_DEFINE.CHAR_RACE_NUM]; //右手：0，女obj；1，女材质；2，男obj；3，男材质
    public string[] pVisvalLocatorL = new string[2 * DBC_DEFINE.CHAR_RACE_NUM]; //左手

    public string[] pEffectName = new string[7];// 增加宝石和强化特效，宝石特效7个，强化特效3个 [10/25/2011 Sun]
    public string[] pStrendthenEffectName = new string[3];

}
//游戏中用到的投影贴花表
public class _DBC_EFFECT
{
    public int nID;
    public string effectName ;
    public string descStr;
}
//{----------------------------------------------------------------------------



public class _DBC_ITEM_EQUIP_BASE
{
    public int nID;						// 物品的唯一标示由class、Quality、Type、Index组合而成
    public int nClass;						// 物品种类enum ITEM_CLASS
    public int nQuality;					// 品质enum EQUIP_QUALITY
    public int nType;						// 详细的类型WEAPIN_TYPE, DEFENCE_TYPE, ADORN_TYPE
    public int nIndex;						// nID的最后3位
    public int nEquipPoint;				// HUMAN_EQUIP 装备格
    public int nVisualID;					// 对应的外观索引itemvisual*.txt
    public int nRule;						// 对应_DBC_ITEM_RULE中的id
    public int nSetID;						// 套装编号
    public int nSetNum;					// 套装数量
    public string szName;						// 物品名称
    public int nLevelRequire;				// 需求等级
    public int nMenPai;					// 职业需求
    public string szDesc;						// 描述
    public string szIcon;						// 图标
    public string szTypeDesc;					// 物品的类型描述
    public int nBasePrice;					// 基本价格
    public int nSellPrice;					// 出售价格
    public int nBMaxDur;					// 耐久
    public int nBMaxRepair;				// 可修理次数
    public int nBMaxGem;					// 可装备宝石数量
    public int nSkillID;					// 技能ID
    public int nScriptID;					// 脚本ID
    public int nDangCi;					//// 装备表修改 [9/15/2011 zzh+]
    public int nNextDangCiItemSN;					//// 当前装备升到下一档的物品的SN [9/15/2011 zzh+]
    public int[] nBaseAttr = new int[DBC_DEFINE.MAX_BASE_ATTR];	// 基础属性
}


/*
 * 
 * /////////////////////////////////////////////////////////////////////
//坐骑表
/////////////////////////////////////////////////////////////////////*/

public class _DBC_CHARACTER_MOUNT
{
// 	int		m_nID;
// 	int		m_nModelID;
// 	float	m_fAddHeight;
// 	int		m_nPassLevel;
    public int nID;
    public string pName;
    public int nPeopleNum;
    public int m_nModelID;
    public float m_fAddHeight;
    public int nRideRecipe;
    public int nMainActionIndex;
    public string pMainBindName;
    public int nSubActionIndex;
    public string pSubBindName;
    public int nSoundID;
    public int nJumpAble;
}
// /////////////////////////////////////////////////////////////////////
// //宠物附加属性						(DBC_PET_EX_ATT)
// /////////////////////////////////////////////////////////////////////
// 
public class _DBC_PET_EX_ATT
{
    public int m_ID;						// 宠物编号
    public string m_Name;						// 名称
    public int m_Type;						//宠物类型
    public int m_TakeLevel;				// 可携带等级
    public int m_Camp;						// 阵营
    public int m_Reserve1;					// 保留
    public int m_Reserve2;					// 保留
    public int m_FoodType;					// 食物类
    public int m_SkillLearnNum;			// 所能学的技能数
    public int m_PositiveSkill;			// 主动技能
    public int m_PassiveSkill1;			// 被动技能1
    public int m_PassiveSkill2;			// 被动技能2
    public int m_PassiveSkill3;			// 被动技能3
    public int m_PassiveSkill4;			// 被动技能4
    public int m_Life;						// 标准寿命
    public int m_StrPerception;			// 标准力量资质
    public int m_ConPerception;			// 标准体质资质
    public int m_DexPerception;			// 标准灵气资质
    public int m_SprPerception;			// 标准身法资质
    public int m_IntPerception;			// 标准定力资质
    public int m_GrowRate0;				// 成长率1
    public int m_GrowRate1;				// 成长率2
    public int m_GrowRate2;				// 成长率3
    public int m_GrowRate3;				// 成长率4
    public int m_GrowRate4;				// 成长率5
    public int m_CowardiceRate;			// 胆小几率
    public int m_WarinessRate;				// 谨慎几率	
    public int m_LoyalismRate;				// 忠诚几率
    public int m_CanninessRate;			// 精明几率
    public int m_ValourRate;				// 勇猛几率
    public int m_FanZhi_Time;			//宠物繁殖时间(ms)

    //public int[] m_HaveProblemsZZH = new int[20];	////zzh+
}
// 
// /////////////////////////////////////////////////////////////////////
// //生物声音定义表					(CREATURE_03)
// /////////////////////////////////////////////////////////////////////
// 
// class _DBC_CREATURE_SOUND
// {
// 	int		nID;
// 	int		nAttack;
// 	int		nUnderAttack;
// 	int		nDeath;
// }
// 
// ///////////////////////////////////////////////////////////////////////
// ////物品定义表_白装					(ITEM_03)
// ///////////////////////////////////////////////////////////////////////
// #define	DBC_ITEM_EQUIP_WHITE		(303)
// //#define MAX_BASE_ATTR	(12)	//装备基础属性（如：物攻，魔攻等）
// //#define MAX_ADD_ATTR	(54) //装备表最大附加属性
// //
// //class _DBC_ITEM_EQUIP_WHITE
// //{
// //	int		nID;						//物品的唯一标示由class、Quality、Type、Index组合而成
// //	int		nClass;						//物品种类enum ITEM_CLASS
// //	int		nQuality;					//品质enum EQUIP_QUALITY
// //	int		nType;						//详细的类型WEAPIN_TYPE, DEFENCE_TYPE, ADORN_TYPE
// //	int		nIndex;						//nID的最后3位
// //	int		nEquipPoint;				//HUMAN_EQUIP 装备格
// //	int		nVisualID;					//对应的外观索引itemvisual*.txt
// //	int		nRule;						//对应_DBC_ITEM_RULE中的id
// //	int		nSetID;						//套装编号
// //	int		nSetNum;					//套装数量
// //	//int		nJob;					// temp fix [10/22/2010 ivan edit]
// //	string	szName;						//物品名称
// //	int		nLevelRequire;				//需求等级
// //	string	szDesc;						//描述
// //	int		nBasePrice;					//基本价格
// //	int		nSellPrice;					//出售价格
// //	int		nBMaxDur;					//耐久
// //	int		nBMaxRepair;				//可修理次数
// //	int		nBMaxGem;					//可装备宝石数量
// //	//...NOT CARE
// //	//int		NOTCARE[70];	//这些应该找server
// //	int		NOTCARE[4];					//技能ID，脚本ID，行囊，格箱
// //	int		NOTCARE1[MAX_ADD_ATTR];	//这些应该找server
// //	int		nBaseAttr[MAX_BASE_ATTR];	//基础属性
// //	string szIcon;						//图标
// //	string szTypeDesc;					// 物品的类型描述2006－4－28
// //	int nNameID;
// //	int nColor;							//显示颜色
// //	int nIsPublic;						//是否广播
// //}
// //
// ///////////////////////////////////////////////////////////////////////
// ////物品定义表_绿装					(ITEM_04)
// ///////////////////////////////////////////////////////////////////////
// #define	DBC_ITEM_EQUIP_GREEN		(304)
// //#define MAX_ADD_ATTR_G		(3) //绿装表最大附加属性
// //
// //class _DBC_ITEM_EQUIP_GREEN
// //{
// //	int		nID;						//物品的唯一标示由class、Quality、Type、Index组合而成
// //	int		nClass;						//物品种类enum ITEM_CLASS
// //	int		nQuality;					//品质enum EQUIP_QUALITY
// //	int		nType;						//详细的类型WEAPIN_TYPE, DEFENCE_TYPE, ADORN_TYPE
// //	int		nIndex;						//nID的最后3位
// //	int		nEquipPoint;				//HUMAN_EQUIP 装备格
// //	int		nVisualID;					//对应的外观索引itemvisual*.txt
// //	int		nRule;						//对应_DBC_ITEM_RULE中的id
// //	int		nSetID;						//套装编号
// //	int		nSetNum;					//套装数量
// ////	int		nJob;						//  [10/27/2010 Sun]
// //	string	szName;						//物品名称
// //	int		nLevelRequire;				//需求等级
// //	string	szDesc;						//描述
// //	int		nBasePrice;					//基本价格
// //	int		nSellPrice;					//出售价格
// //	int		nBMaxDur;					//耐久
// //	int		nBMaxRepair;				//可修理次数
// //	int		nBMaxGem;					//可装备宝石数量
// //	int		nSkillID;					//技能ID
// //	int		nScriptID;					//脚本ID
// //	string szIcon;
// //	string szTypeDesc;					// 物品的类型描述2006－4－28
// //	//...NOT CARE
// //	int		NOTCARE[MAX_ADD_ATTR_G];	//1. NameID\ 2. 是否紫色装备\ 3. 飘带id
// //	int		NOTCARE1[MAX_ADD_ATTR];		//附加属性
// //	int		nBaseAttr[MAX_BASE_ATTR];	//基础属性
// //	int nColor;							//显示颜色
// //	int nIsPublic;						//是否广播
// //}
// //
// //
// ///////////////////////////////////////////////////////////////////////
// ////物品定义表_蓝装					(ITEM_05)
// ///////////////////////////////////////////////////////////////////////
// #define	DBC_ITEM_EQUIP_BLUE		(305)
// //#define MAX_ADD_ATTR_B		(66) //蓝装表最大附加属性
// //
// //class _DBC_ITEM_EQUIP_BLUE
// //{
// //	int		nID;
// //	int		nClass;
// //	int		nQuality;
// //	int		nType;
// //	int		nIndex;
// //	int		nEquipPoint;
// //	int		nVisualID;
// //	int		nRule;
// //	int		nSetID;
// //	int		nSetNum;
// ////	int		nJob;
// //	string	szName;
// //	int		nLevelRequire;
// //	string	szDesc;
// //	int		nBasePrice;
// //	int		nSellPrice;
// //	int		nBMaxDur;
// //	int		nBMaxRepair;
// //	int		nBMaxGem;
// //	int		nSkillID;
// //	int		nScriptID;
// //	//...NOT CARE
// //	int		NOTCARE1[MAX_ADD_ATTR];		//附加属性
// //	int		nBaseAttr[MAX_BASE_ATTR];	//基础属性
// //	string szIcon;
// //	string szTypeDesc;		// 物品的类型描述2006－4－28
// //	int nNameID;
// //	int nColor;
// //	int nIsPublic;
// //}
// //----------------------------------------------------------------------------}
// 
 /////////////////////////////////////////////////////////////////////
 //物品定义表_药瓶				(ITEM_06)
 /////////////////////////////////////////////////////////////////////
 //#define	DBC_ITEM_MEDIC			(306)
 public class _DBC_ITEM_MEDIC
 {
 	public int		nID;
 
 	//// _ITEM_TYPE [12/31/2010 zzh?] 应该改成_ITEM_TYPE
    public int nClass;
    public int nQuality;
    public int nType;
    public int nIndex;

    public string szIcon;
    public string szName;
    public string szDesc;
    public int nLevelRequire;
    public int nCurPinJiValue;			//// 材料品级值 [9/16/2011 zzh+]
    public int nBasePrice;			// 基本价格 [8/31/2010 Sun]
    public int nSalePrice;			//出售价格
    public int nRule;				//适应规则
    public int nPileCount;			//叠放数量
    public int nScriptID;			//脚本编号
    public int nSkillID;			//技能编号(使用对应技能的冷却时间)
    public int[] NOTCARE = new int[3];			//需求技能等级
    public int nMaxHold;			//最大持有数量
    public int nTargetType;		// 针对对象类型 ENUM_ITEM_TARGET_TYPE
    public string szTypeDesc;			// 物品的类型描述2006－4－28
    public int[] NOTCARE2 = new int[5];		//档次等级，配方ID，颜色，subclass，是否广播
 }
 
// /////////////////////////////////////////////////////////////////////
// //物品定义表_宝石				(ITEM_07)
// /////////////////////////////////////////////////////////////////////
 //// 在geminfo.txt中引入宝石套装（GemSet）属性和需求等级属性，可以让Gem和Equip的属性统一
 
 //#define	DBC_ITEM_GEM			(307)
 public class _DBC_ITEM_GEM
 {
 	public int		nID;
 
 	//// _ITEM_TYPE [12/31/2010 zzh?] 应该改成_ITEM_TYPE
 	public int		nClass;
 	public int		nQuality;
 	public int		nType;
 	public int		nGemIndex;
 
 	public string	szIcon;
 	public int		nRule;
 	public string	szName;
 	public string szDesc;
 	public int		nPrice;		// 宝石的价格
 	public int		nSellPrice;		// 宝石的出售价格
    public int      m_GemAttrType;
    public int      m_GemAttrValue;
 	public string	szColor;	// 附加属性的颜色
 	public string   szTypeDesc; // 类型描述
 	public int		nEffectIndex;//特效索引
 	public int		nColor;
 	public int		nIsPublic;
 }
 public class _DBC_ITEM_SYMBOL // 符印数据表结构 [3/22/2012 ZZY]
 {
     public int nID;
     public int nClass;
     public int nQuality;
     public int nType;
     public int nSymbolIndex;

     public string szIcon;
     public int nRule;
     public string szName;
     public string szDesc;
     public int nPrice;		// 宝石的价格
     public int nSellPrice;		// 宝石的出售价格
     public int m_SymbolAttrType;
     public int m_SymbolAttrValue;
 }
//物品定义表_法宝				(ITEM_09)
/////////////////////////////////////////////////////////////////////
//#define	DBC_ITEM_TALISMAN			(309)
public class _DBC_ITEM_TALISMAN
{
     public int nTableIndex;    //index
     public int nClass;
     public int nQuality;       //品质
     public int nType;          //等级
     public int nIndex;
     public int nRule;          //适应规则
     public uint uBasePrice;     //基本价格
     public uint uBaseExp;           //被吃所得经验
     public uint uLevelrequireExp;   //升级需求经验
     public int m_AttrType;     //效果类型
     public int m_Value;        //效果值
     public string szIcon;         //
     public string szName;         //名称
     public string szDesc;         //说明信息
}
// 
// /////////////////////////////////////////////////////////////////////
// //物品定义表_地图				(ITEM_08)
// /////////////////////////////////////////////////////////////////////
// #define	DBC_ITEM_STOREMAP			(308)
// class _DBC_ITEM_STOREMAP
// {
// 	int		nID;
// 
// 	//// _ITEM_TYPE [12/31/2010 zzh?] 应该改成_ITEM_TYPE
// 	int		nClass;
// 	int		nQuality;
// 	int		nType;
// 	int		nIndex;
// 
// 	string	szIcon;
// 	string	szName;
// 	string	szDesc;
// 	int		nLevelRequire;
// 	int		nRule;
// 	float	fNOTCARE[2];
// 	int		nNOTCARE[3];
// 	//.......................
// 	//... NOT CARE
// }
// 
// /////////////////////////////////////////////////////////////////////
// //套装组合后附加属性			(ITEM_15)
// /////////////////////////////////////////////////////////////////////
// 
// /* Allan 4/1/2011 
// #define	DBC_ITEM_SETATT			(315)
// class _DBC_ITEM_SETATT
// {
// int		nID;
// string	szName;
// int		nAtt[35];
// }
// ------------------*/
// 
// #define	DBC_ITEM_SETATT			(315)
// class _DBC_ITEM_SETATT             // Allan 4/1/2011 
// {
//     int		nID;
//     string	szName;
//     int     nSetNum;                //套装最大数量
//     int     nAttrNum;               //套装属性数量
//     string nSetEquipID;            //构成套装的装备ID
//     int     nSkillID;               //心法ID
//     int     nEffectIndex;           //特效索引
//     int     nBindPoint;             //特效挂载点
//     int     nActivityNum1;          //本条属性激活需要的最低套装件数
//     int     nAttrType1;             //属性枚举
//     int     nAttrValue1;            //属性数值
//     string nAttrDescribe1;         //属性描述
//     int     nActivityNum2;
//     int     nAttrType2;
//     int     nAttrValue2;
//     string nAttrDescribe2;
//     int     nActivityNum3;
//     int     nAttrType3;
//     int     nAttrValue3;
//     string nAttrDescribe3;
//     int     nActivityNum4;
//     int     nAttrType4;
//     int     nAttrValue4;
//     string nAttrDescribe4;
//     int     nActivityNum5;
//     int     nAttrType5;
//     int     nAttrValue5;
//     string nAttrDescribe5;
//     int     nActivityNum6;
//     int     nAttrType6;
//     int     nAttrValue6;
//     string nAttrDescribe6;
// }
// 
// 
// 
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// //								声音相关								//
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// 
// /////////////////////////////////////////////////////////////////////
// //声音文件定义						(SOUND_01)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SOUND_INFO				(401)
// class _DBC_SOUND_INFO
// {
// 	int		nID;
// 	string	pSoundFile;
// 	string	pNOTCARE;
// }
// 
// 
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// //								特效相关								//
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// 
// /////////////////////////////////////////////////////////////////////
// //特效定义							(EFFECT_01)
// /////////////////////////////////////////////////////////////////////
// #define DBC_EFFECT_DEFINE			(501)
// class _DBC_EFFECT_DEFINE
// {
// 	int		nID;
// 	string	pEffectType;
// 	string	pParam1;
// 	string	pParam2;
// 	////ZZH- string	pParam3;
// 	////ZZH- string	pParam4;
// 	////ZZH- string	pParam5;
// 	////ZZH- string	pParam6;
// }
// 
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// //								技能相关								//
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// /////////////////////////////////////////////////////////////////////
// //心法定义表						(SKILL_01)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SKILL_XINFA				(601)
public class _DBC_SKILL_XINFA
{
    public int nID;
    public int nMenpaiID;
    public string pszName;
    public string pszDesc;
    public string pszIconName;
    public int nCDTime;
    public int nLevelLimit;
   
};

//基础属性表
public class _DBC_XINFA_SKILL_DATA
{
    public int nID;
    public int[] nMenPaiValue = new int[6];

}
// /////////////////////////////////////////////////////////////////////
// //附加效果							(SKILL_02)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SKILL_ADDIMPACT			(602)
// class _DBC_SKILL_ADDIMPACT
// {
// 	int		nID;
// 	string	pDesc;
// 	int		nParamNum;
// 	int		nAddAttributeNum;
// 	string	pAddAttribute1;
// 	string	pAddAttribute2;
// 	string	pAddAttribute3;
// 	string	pAddAttribute4;
// 	int		nSpliceID;
// 	string	pIcon;
// }
// //
// ///////////////////////////////////////////////////////////////////////
// ////子弹效果定义						(SKILL_03)
// ///////////////////////////////////////////////////////////////////////
// //#define DBC_SKILL_BULLET			(603)
// //class _DBC_SKILL_BULLET
// //{
// //	int		nID;							//子弹类型
// //	int		nType;							/*
// //											|	0 - 瞬间到达
// //											|	1 - 移动到达
// //											*/
// //	int		nEffect;						//特效ID
// //	float	fSpeed;							//移动速度	(m/s)
// //	int		nHitEffect;						//击中特效
// //	int		nHitEffectLiveTime;				//击中特效维持时间
// //}
// 
// /////////////////////////////////////////////////////////////////////
// //BUFF附加效果						(BUFF_IMPACT)
// /////////////////////////////////////////////////////////////////////
// #define DBC_BUFF_IMPACT					(604)
// //_DBC_BUFF_IMPACT
// 
// /////////////////////////////////////////////////////////////////////
// //DIRECTLY附加效果						(DIRECTLY_IMPACT)
// /////////////////////////////////////////////////////////////////////
// #define DBC_DIRECTLY_IMPACT				(605)
// //_DBC_DIRECTLY_IMPACT
// 
// /////////////////////////////////////////////////////////////////////
// //子弹						(BULLET)
// /////////////////////////////////////////////////////////////////////
// #define DBC_BULLET_DATA					(606)
// //_DBC_BULLET_DATA
public class _DBC_BULLET_DATA
{
    public int          m_nID;					// ID
    public int          m_nContrailType;		// 轨迹类型 ENUM_BULLET_CONTRAIL_TYPE
    public int	        m_fContrailParam;		// 轨迹参数(对抛物线为上升的最大高度)
    public string       m_szFlyEffect;			// 飞行特效
    public string       m_szFlySound;			// 飞行音效
    public float        m_fSpeed;				// 速度(m/s)
    public string	    m_szHitEffect;			// 击中特效
    public string 	    m_szHitSound;			// 击中音效
    public string	    m_szHitEffectLocator;	// 击中特效的绑定点
    public string 	    m_szReserved1;			// 预留1
    public string 	    m_szReserved2;			// 预留1
    public string 	    m_szReserved3;			// 预留1
    public string 	    m_szReserved4;			// 预留1
    public string 	    m_szReserved5;			// 预留1
    public string       m_szDesc;				// 说明和描述
};
// /////////////////////////////////////////////////////////////////////
// //技能						(SKILL)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SKILL_DATA					(607)
// //_DBC_SKILL_DATA
public class _DBC_SKILL_DATA
{
    public int m_nID;							// ID
    public int m_nIDForManagement;				// 策划内部使用的管理性ID
    public int m_nMenPai;						// 门派ID
    public string m_lpszName;					// 名称
    public string m_lpszIconName;				// Icon名称
    public int m_nLevelRequirement;				// 技能的等级要求
    public int m_nSkillActionType;				// 招式类型ENUM_SKILL_ACTION_TYPE
    public int m_bMustUseWeapon; 				// 是否是必须使用武器的技能
    public int m_nDisableByFlag1; 				// 受限于限制标记1, 用于昏迷,魅惑等
    public int m_nDisableByFlag2; 				// 受限于限制标记2, 用于沉默类似
    public int m_nDisableByFlag3; 				// 受限于限制标记3, 用于变身骑乘
    public int m_nSkillClass;					// 技能系
    public int m_nXinFaParam_Nouse;				// 心法修正参数
    public int m_nRangeSkillFlag;				// 是否是远程技能
    public bool m_bBreakPreSkill;				// 是否中断自己当前使用的技能
    public int m_nType;							// 技能类型 ENUM_SKILL_TYPE
    public int m_nCooldownID;					// 冷却组ID
    public string m_lpBeginEffect;				// 起手特效
    public string m_lpBeginEffectLocator;		// 起手特效绑定点
    public string m_lpBeginSound;				// 起手音效
    public string m_lpszGatherLeadActionSetID;	// 引导/聚气动作组ID 
    public string m_lpszSendActionSetID;		// 发招招式ID
    public int m_nEnvironmentSpecialEffect;		// 环境特效ID
    public int m_nTargetMustInSpecialState;		// 目标必须是， 0:活的；1:死的; -1:没有要求
    public int m_nClassByUser;					// 按使用者类型分类，0:玩家, 1:怪物, 2:宠物, 3:物品,
    public int m_nPassiveFlag;					// 主动还是被动技能，0:主动技能,1:被动技能;
    public int m_nSelectType;					// 点选类型 ENUM_SELECT_TYPE
    public int m_nOperateModeForPetSkill;		// 宠物技能专用，操作模式: PET_SKILL_OPERATEMODE
    public int m_nPetRateOfSkill; 				// 技能发动几率,只对宠物技能有效
    public int m_nTypeOfPetSkill; 				// 宠物技能类型,0:物功,1:法功,2:护主,3:防御,4:复仇;
    public short m_nImpactIDOfSkill;			// 宠物技能产生的效果ID
    public int m_nGroupID;						// 技能组ID
    public int m_nGroupLevel;					// 技能组等级
    public int m_nLearnDir;						// 是否直接学会
    public int m_nUseInLearned;					// 使用前是否需要学会
    public int m_nBulletID;						// 子弹ID
    public string m_pszBulletSendLocator;		// 子弹发出的绑定点
    public int m_nSpeedOfBullet;				// 子弹飞行速度 [4/12/2012 Adomi]
    public int m_nTargetingLogic;				// 目标选择逻辑 ENUM_TARGET_LOGIC
    public int m_nSendTime;						// 发招时间(ms),动作时间
    public float m_fMinAttackRange;				// 最小攻击距离(m)
    public float m_fMaxAttackRange;				// 最大攻击距离(m)
    public int m_nFriendness;					// 技能友好度，=0为中性技能，>0为正面技能，<0为 负面技能
    public int m_nRelationshipLogic;			// 目标和使用者关系的合法性判断ID，参考相关子表格。
    public int m_nTargetCheckByObjType;			// 目标必须是某一指定的obj_type的角色
    public int m_nIsPartyOnly;					// 是否仅对队友有效。注：队友宠物算作队友。1为只对队友有效，0为无此限制。
    public int m_nHitsOrinterval;				// 连击的攻击次数或引导的时间间隔
    public bool m_bAutoRedo;					// 自动连续释放
    public int m_nHitRate;						// 命中率
    public int m_nCriticalRate; 				// 会心率
    public bool m_bUseNormalAttackRate;			// 冷却时间是否受攻击速度影响
    public int m_nActiveTime;					// 激活时间
    public float m_fDamageRange;				// 杀伤范围(m)
    public int m_nDamageAngle;					// 杀伤角度(0~360)
    public int m_nTargetNum;					// 影响目标的最大数
    public int m_nCaninterruptPetAttack;		// 中断或激活珍兽攻击
    public int m_nCaninterruptAutoShot; 		// 是否能打断自动技能的连续释放
    public int m_nDelayTime; 					// 延迟时间
    public int m_nHurtDelayTime;                // 伤害通用延迟时间 [4/12/2012 Adomi]
    public int[] m_anSkillByLevel = new int[12];// 心法级别对应的技能ID
    public string m_pszDesc;					// 技能描述
    public int m_nShowDetail;					// 是否显示打击名
    //public int[] m_nUnKnow = new int[3];		//干扰时间百分比\干扰几率\干扰时间浮动百分比
    //暂时修改
    public float m_fUnknow;
    public int[] m_iUnknow = new int[2];
};

// 客户端与服务器共用的DBC结构
// 技能伤害结构
public class _DBC_DIRECT_IMPACT
{
    public uint   m_uID;					// ID
    public string m_pszEffect;			// 特效ID
    public string m_pszSound;			// 音效ID
    public string m_pszEffectLocator;	// 特效绑定点
    public string m_pszEffect2;			// 特效ID
    public string m_pszSound2;			// 音效ID
    public string m_pszEffectLocator2;	// 特效绑定点
    public string m_pszEffect3;			// 特效ID
    public string m_pszSound3;			// 音效ID
    public string m_pszEffectLocator3;	// 特效绑定点
    public string m_pszEffect4;			// 特效ID
    public string m_pszSound4;			// 音效ID
    public string m_pszEffectLocator4;	// 特效绑定点
    public string m_pszEffect5;			// 特效ID
    public string m_pszSound5;			// 音效ID
    public string m_pszEffectLocator5;	// 特效绑定点
    public string m_pszReserved1;		// 预留1
    public string m_pszReserved2;		// 预留2
    public string m_pszInfo;				// 效果描述
};
// /////////////////////////////////////////////////////////////////////
// //技能						(SKILL_DEPLETE)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SKILL_DEPLETE					(608)
public class _DBC_SKILL_DEPLETE
{
    public int m_nID;
    public int m_nHP;
    public int m_nMP;
    public int m_nSP;
    public int m_nStrikePoint;
    public int m_nItemID;
};
// 
// /////////////////////////////////////////////////////////////////////
// //任务						(MISSION)
// /////////////////////////////////////////////////////////////////////
// #define DBC_MISSION_LIST				(610)
public class _DBC_MISSION_LIST
{
    public int nScriptID;
    public int nMissionID;
    public int nReward;
    public int nPunish;
    public int nDialog;
    public int nPromulgatorScene;
    public int nPromulgatorID;
    public int nSubmitorScene;
    public int nSubmitorID;
    public int nDestScene;
    public int nDestX;
    public int nDestZ;
}
// 
// #define DBC_MISSION_DATA				(611)
public class _DBC_MISSION_DATA
{
    public int nMissionID;
    public int nMissionClass;
    public int nMissionSubClass;
    public int nIsExistArea;           // Allan_Tao 14/3/2011 新加一个字段,用于判断当前任务是否存在区域类的限制, 0表是没有,1表示有
    public int nMissionType;
    public int nMissionLevel;
    public int nLevel;
    public int nMaxLeve;
    public int nCamp;
    public int nIdentity; //no use
    public int nAcceptItem1ID;
    public int nAcceptItem1Num;
    public int nAcceptItem2ID;
    public int nAcceptItem2Num;
    public int nFinishItem1ID;
    public int nFinishItem1Num;
    public int nFinishItem1UseNum;       // Allan_Tao 14/3/2011 物品使用次数
    public int nFinishItem2ID;
    public int nFinishItem2Num;
    public int nFinishItem3ID;
    public int nFinishItem3Num;
    public int nFinishItem4ID;
    public int nFinishItem4Num;
    public int nFinishItem5ID;
    public int nFinishItem5Num;
    public int nMonster1ID;
    public int nMonster1Num;
    public int nMonster2ID;
    public int nMonster2Num;
    public int nMonster3ID;
    public int nMonster3Num;
    public int nMonster4ID;
    public int nMonster4Num;
    public int nMoney1;
    public int nMoney2;
    public int nMenPai;
    public int nGuildLevel; //no use
    public int nMissionKind;
    public int nTotalTimes;
    public int nDayTimes;
    public int nTimeDist;
    public int nMaxCycle;
    public int nPositionTag;
    public int nPreMission;
    public int nAftMission;
    public int nDestScene;             // Allan_Tao 14/3/2011 区域所在场景
    public int nDestX;                 // Allan_Tao 14/3/2011 坐标代表区域来处理
    public int nDestZ;                 // Allan_Tao 14/3/2011 坐标代表区域来处理
    public int nDistance;              // Allan_Tao 14/3/2011 坐标+距离成为区域, 距离的平方
    public int nNeedBuffId;                // Allan_Tao 14/3/2011 完成任务需要的buff
    public string nTargetTraceOne;
    public string nTargetTraceTwo;
    public string nTargetTraceThree;
    public string nTargetTraceFour;
    public string nTargetTraceFive;
    public string nTargetTraceComplete;
    //int		nNotCare[33]; not used -ivan
}
// 
// #define DBC_MISSION_REWARD				(612)
public class _DBC_MISSION_REWARD
{
    public int nRewardID;
    public int nMoney;
    public int nAmbitExp;
    public int nItem1ID;
    public int nItem1Num;
    public int nItem2ID;
    public int nItem2Num;
    public int nRandomItem1ID;
    public int nRandomItem1Num;
    public int nRandomItem2ID;
    public int nRandomItem2Num;
    public int nRandomItem3ID;
    public int nRandomItem3Num;
    public int nRandomItem4ID;
    public int nRandomItem4Num;
    public int nExp;
    public int nLevel;
    public int nGuildExp1;
    public int nGuildExp2;
    public int nGuildExp3;
    public int nBuff1ID;
    public int nBuff2ID;
    public int nBuff3ID;
}
// 
// #define DBC_MISSION_PUNISH				(613)
// class _DBC_MISSION_PUNISH
// {
// 	int		nPunishID;
// 	int		nMoney;
// 	int		nItem1ID;
// 	int		nItem1Num;
// 	int		nItem2ID;
// 	int		nItem2Num;
// 	int		nItem3ID;
// 	int		nItem3Num;
// 	int		nExp;
// 	int		nLevel;
// 	int		nMissionTag;
// }
// 
// #define DBC_MISSION_DIALOG				(614)
public class _DBC_MISSION_DIALOG
{
    public int nDialogID;
    public string szMissionName;		//任务名称
    public string szMissionDesc;	//任务描述
    public string szMissionTarget;	//任务目标
    public string szMissionContinue;	//继续任务
    public string szMissionAbandon;	//放弃任务
    public string szMissionSuccess;	//完成任务
}
// 
// #define DBC_MISSION_NPC				(615)
// class _DBC_MISSION_NPC
// {
// 	int			nScriptID;
// 	int			nSceneID;
// 	string		szNpcName;
// 	int			nDialogNum;
// 	string		szDialog1;
// 	string		szDialog2;
// 	string		szDialog3;
// 	string		szDialog4;
// 	string		szDialog5;
// }
// 
// #define DBC_MISSION_LOOTITEM				(616)
// class _DBC_MISSION_LOOTITEM
// {
// 	int			nMissionIndex;
// 	int			nMissionID;
// 	int			nMissionType;
// 	int			nMissionNOTCARE1[6];
// 	string		szMissionName;
// 	string		szMissionTarget;
// 	string		szMissionDesc1;
// 	string		szMissionDesc2;
// 	string		szMissionDesc3;
// 	int			nMissionSceneID;
// 	string		szAcceptNpcName;
// 	int			nMissionNOTCARE2[2];
// 	string		szFinishNpcName;
// 	int			nMissionNOTCARE3[6];
// 	string		szEnemyName1;
// 	int			nMissionNOTCARE4[4];
// 	string		szEnemyName2;
// 	int			nMissionNOTCARE5[4];
// 	string		szEnemyName3;
// 	int			nMissionNOTCARE6[4];
// 	string		szEnemyName4;
// 	int			nMissionNOTCARE7[4];
// 	string		szEnemyName5;
// 	////zzh- int			nMissionNOTCARE8[8];
// 	int			nMissionNOTCARE8[9];
// 	string		szMissionNOTCARE1;
// 	int			nMissionNOTCARE9;
// 	string		szMissionNOTCARE2;
// 	int			nMissionNOTCARE10[2];
// 	string		szMissionNOTCARE3;
// 	int			nMissionNOTCARE11[26];
// }
// 
// #define DBC_MISSION_ENTER_AREA				(617)
// class _DBC_MISSION_ENTER_AREA
// {
// 	int			nMissionIndex;
// 	int			nMissionID;
// 	int			nNOTCARE1[6];
// 	string		szNOTCARE1[5];
// 	int			nHasDialog;
// 	string		szDialog;
// 	string		szTips;
// 	int			nAcceptSceneID;
// 	string		szAcceptNpcName;
// 	int			nAcceptNpcGUID;
// 	int			nFinishSceneID;
// 	string		szFinishNpcName;
// 	////zzh- int			nNOTCARE[40];
// 	int			nNOTCARE[41];////zzh+
// }
// 
// #define DBC_MISSION_NPC_HASHTABLE				(618)
// class _DBC_MISSION_NPC_HASHTABLE
// {
// 	int			nNpcIndex;
// 	string		szNpcName;
// 	string		szSceneName;
// 	int			nNOTCARE1[3];
// 	string		szNOTCARE1;
// 	int			nNOTCARE2[52];
// 	int			nNOTCARE3[307/*366-59*/];////zzh+
// }
// 
// //-------- Allan 13/1/2011 
// //Npc EventList 数量
public struct NPC_MISSION
{
    public const int MAX_NPC_EVENT = 25;
}
// //npc事件表
// #define DBC_NPC_MISSION				(619)
public class _DBC_NPC_MISSION
{
     public int		m_Type;
     public string  m_NpcName;
     public string  m_DefaultDialog;
     public int		m_ScriptID;
     public int[]   m_EventList = new int[NPC_MISSION.MAX_NPC_EVENT];
 }
// //-------- Allan End ------------------*/
// 
 
 /************************************************************************/
 /* 任务所需条件列表 [6/14/2011 ivan edit]                               */
 /************************************************************************/
 //#define DBC_MISSION_DEMAND_LIST (620)
public class _DBC_MISSION_DEMAND_LIST
{
    public int m_nID;					//ID
    public int n_ScriptID;
    public int n_MissionID;
    public int n_MissKind;
    public int n_AcceptNpcID;
    public int n_AcceptSceneId; // NPC所在场景
    public int n_AcceptPosX;
    public int n_AcceptPosZ;
    public int n_FinishNpcID;
    public int n_FinishSceneId; // NPC所在场景
    public int n_FinishPosX;
    public int n_FinishPosZ;
    public int n_MinLevel;
    public int n_MaxLevel;
    public int n_PreMissionID;	// 前置任务 [8/1/2011 ivan edit]
}
// 
// 
// /////////////////////////////////////////////////////////////////////
// //法术OBJ数据						(SPECIAL_OBJ_DATA)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SPECIAL_OBJ_DATA			(630)
public class _DBC_SPECIAL_OBJ_DATA
{
    public int m_nID;					//ID
    public string m_lpszName;				//名称
    public string m_lpszToolTips;			//ToolTips
    public int m_nType;				//类别（服务器专用）
    public int m_nLogicID;				//逻辑ID（服务器专用）
    public int m_nStealthLevel;		//隐形级别（服务器专用，陷阱专用）
    public int m_nTrapFlags;			//陷阱标记位集合（服务器专用，陷阱专用）
    public int m_nActiveTimes;			//可以激发的次数（服务器专用）
    public string m_lpszEffect_Normal;	//生存期持续特效
    public string m_lpszSound_Normal;		//生存期持续音效
    public string m_lpszEffect_Active;	//激发特效
    public string m_lpszSound_Active;		//激发音效
    public string m_lpszEffect_Die;		//死亡特效
    public string m_lpszSound_Die;		//死亡音效
    public int m_nReserve1;			//预留
    public int m_nReserve2;			//预留
    public int m_nReserve3;			//预留
    public int m_nBulletID;			//子弹ID
    public int m_nDuration;			//持续时间（服务器专用）
    public int m_nInterval;			//激活或触发时间间隔（服务器专用）
    public float m_nTriggerRadius;		//触发半径（服务器专用，陷阱专用）
    public float m_nEffectRadius;		//影响半径（服务器专用，陷阱专用）
    public int m_nEffectUnitNum;		//影响个体数目（服务器专用，陷阱专用）
    public int m_nReserve4;			//预留
    public int m_nReserve5;			//预留
    public int m_nReserve6;			//预留
    public int m_nReserve7;			//预留
    public int m_nReserve8;			//预留
    public string m_lpszParamDesc0;		//参数说明（服务器专用）
    public int m_nParam0;				//参数值（服务器专用）
    public string m_lpszParamDesc1;		//参数说明（服务器专用）
    public int m_nParam1;				//参数值（服务器专用）
    public string m_lpszParamDesc2;		//参数说明（服务器专用）
    public int m_nParam2;				//参数值（服务器专用）
    public string m_lpszParamDesc3;		//参数说明（服务器专用）
    public int m_nParam3;				//参数值（服务器专用）
    public string m_lpszParamDesc4;		//参数说明（服务器专用）
    public int m_nParam4;				//参数值（服务器专用）
    public string m_lpszParamDesc5;		//参数说明（服务器专用）
    public int m_nParam5;				//参数值（服务器专用）
    public string m_lpszParamDesc6;		//参数说明（服务器专用）
    public int m_nParam6;				//参数值（服务器专用）
    public string m_lpszParamDesc7;		//参数说明（服务器专用）
    public int m_nParam7;				//参数值（服务器专用）
    public string m_lpszParamDesc8;		//参数说明（服务器专用）
    public int m_nParam8;				//参数值（服务器专用）
    public string m_lpszParamDesc9;		//参数说明（服务器专用）
    public int m_nParam9;				//参数值（服务器专用）
    public string m_lpszParamDesc10;		//参数说明（服务器专用）
    public int m_nParam10;				//参数值（服务器专用）
    public string m_lpszParamDesc11;		//参数说明（服务器专用）
    public int m_nParam11;				//参数值（服务器专用）
    public string m_lpszParamDesc12;		//参数说明（服务器专用）
    public int m_nParam12;				//参数值（服务器专用）
    public string m_lpszParamDesc13;		//参数说明（服务器专用）
    public int m_nParam13;				//参数值（服务器专用）
    public string m_lpszParamDesc14;		//参数说明（服务器专用）
    public int m_nParam14;				//参数值（服务器专用）
    public string m_lpszParamDesc15;		//参数说明（服务器专用）
    public int m_nParam15;				//参数值（服务器专用）
    public string m_lpszInfo;				//详细说明(内部使用)
}
// 
// 
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// //								场景相关相关							//
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// 
// /////////////////////////////////////////////////////////////////////
// //场景定义							(SCENE_01)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SCENE_DEFINE			(701)
public class _DBC_SCENE_DEFINE
{
    //static const int SCENE_SERVER_ID_COLUMN = 0;////zzh-  1;

    public int nSceneResID;

    public string szName;
    public int campId;						// 场景的阵营id，通用场景填-1 [11/16/2011 Ivan edit]
    public int nXSize;
    public int nZSize;
    public string szWXObjectName;
    public string szRegionFile;
    public string szCollisionfile;	// 建筑物行走面文件。
    public string szMiniMap;
    public int nBackSound;
    public int nCityLevel;
    public string szSceneMap;
    public int nWroldMapPosX;
    public int nWroldMapPosY;				// 场景图标所在得位置
    public int nNameWroldMapPosX;
    public int nNameWroldMapPosY;			// 场景名字所在得位置

    public string szSceneType;				// 场景得图标等级
    public string szCityNameNormalImageSet;		// 场景正常名字所用得图片资源
    public string szCityNameNormalImage;		// 场景正常名字所用得图片资源
    public string szCityNameHoverImageSet;		// 场景高亮名字所用得图片资源
    public string szCityNameHoverImage;		// 场景高亮名字所用得图片资源
    public string szDesc;
    public string szTemp;
}
// 
// /////////////////////////////////////////////////////////////////////
// //场景建筑物定义					(SCENE_02)
// /////////////////////////////////////////////////////////////////////
// #define DBC_BUILDING_DEINFE			(702)
// class _DBC_BUILDING_DEFINE
// {
// 	static const int MAX_LEVEL = 5;
// 
// 	int		nID;				//资源id
// 	string	szResType;			//资源类型
// 	string	szLevel[MAX_LEVEL];	//等级1-5资源
// 	string	szMeshName;
// }
// 
// /////////////////////////////////////////////////////////////////////
// //城市建筑物定义					(SCENE_03)
// /////////////////////////////////////////////////////////////////////
// #define DBC_CITY_BUILDING			(703)
// class _DBC_CITY_BUILDING
// {
// 	int		nID;					//id
// 	int		nCityID;				//城市id(DBC_CITY_DEINFE)
// 	string	szName;					//城市名称
// 	int		nBuildingType;
// 	int		nBuildingID;			//建筑物id(DBC_BUILDING_DEINFE)
// 	int		nInitLevel;				//初始等级
// 	string	szGfxPosition;			//位置(gfx)
// 	string	szGfxOrientation;		//旋转四元数(gfx)
// 	int		nNotCare[15];
// }
// 
// /////////////////////////////////////////////////////////////////////
// //场景传送点定义							(SCENE_02)
// /////////////////////////////////////////////////////////////////////
// #define DBC_SCENE_POS_DEFINE			(917)
public class _DBC_SCENE_POS_DEFINE
{
    public int nID;				    // id
    public string szSceneName;		// 场景的名字
    public int nSceneID;			// 场景的id
    public int nXPos;				// 传送点的位置
    public int nZPos;				// 传送点的位置
    public string szAimName;		// 传送到哪个场景
    public int nDestSceneID;		// 目标场景ID [8/3/2011 ivan edit]
}
// 
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// //								UI相关									//
// //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
// /////////////////////////////////////////////////////////////////////
// //UI组定义						(UI_01)
// /////////////////////////////////////////////////////////////////////
// #define DBC_UI_LAYOUTDEFINE		(801)
public class _DBC_UI_LAYOUTDEFINE
{
	public int nID;
	public string	szName;
	public string	szLayout;
	public int		nDemise;
    public string   poss;//UI窗口坐标,eg: 22,10,0
    public string   desc;
    public int downImmediately;// 是否马上下载 [4/1/2012 Ivan]
}
 
 //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
 //								生活技能相关							//
 //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-//
 /////////////////////////////////////////////////////////////////////
 //资源生长点							(LIFE_01)
 /////////////////////////////////////////////////////////////////////
 public class _DBC_LIFEABILITY_GROWPOINT
 {
 	public int		nID;
    public string szName;
    public string szMeshFile;
    public int nLifeAbility;
    public string szMissionID; // int --> string 采集物品要求支持多个任务id,用逗号隔开，eg: 1901,1902 [4/27/2011 ivan edit]
    public string szTips;
    public int nTimeNeed;
 	//string	szNotcare[4];
    public int nScriptID;
    public int nAnimOpen;
 }
// 
// /////////////////////////////////////////////////////////////////////
// //生活技能								(LIFE_02)
// /////////////////////////////////////////////////////////////////////
 public  class _DBC_LIFEABILITY_DEFINE
 {
    public int nID;
    public string szName;
    public int nLevelNeed;
    public int nLevelMax;
    public string szLevelDesc;//temp fix, 描述升级信息的文本文件
    public int nTimeOperation;
    public int nToolNeed;
    public float fPlatformDist;
    public int nPlatformID;
    public int nAnimOperation;
    public string szIconName;
    public int nItemVisualLocator; //动作挂接
    public string szExplain;			//详细解释
    public int nPopup;				//是否弹出下级界面
    public string szProgressbarName;	//进度条名称
    public int[] nNOTCARE = new int[5];
 }
// 
// /////////////////////////////////////////////////////////////////////
// //生活技能								(LIFE_03)
// /////////////////////////////////////////////////////////////////////
 public class Stuff
 {
     public int nID;
     public int nNum;

 }
 public class _DBC_LIFEABILITY_ITEMCOMPOSE
 {
     public int nID;
     public string szName;
     public int nResultID;
     public int nResultNum;
     public int nLifeAbility;
     public int nLifeAbility_Level;
     public Stuff[] mStuffs = new Stuff[5];
     public int nVigorRequire;			// 活力需求
     public int nEnergyRequire;         // 精力需求
     public int nAttribute_Level1;		// 属性需求的数量？
     public int nAttribute_Level2;		// 属性需求2的数量？
     public int nContriAttrRequire;     // 门派贡献度需求     
     public int nXianShiRequire;		// 仙石需求
     public int nBindLingShi;			// 灵石
     public int nExpRequire;            // 经验
     public int nCoolDown_Time;         // 冷却时间
     public int nFacility;				// 需要的工具	
     public int nCoolDoon_Troop;		// 冷却组
     public int nProficiencyRequire;	// 熟练度需求
     public int nProficiencyIncrease;	// 熟练度增加
     public int nGroup;				    // 功能分组
     public int nMenPai;				// 门派，对应五行 [10/8/2011 Sun]
     public int nProficiencyTime;		// 配方时间
     public int nScriptID;				// 脚本ID
     public int nFailItem;				// 失败后的替代品
     public string szExplain;				// 详细描述
     public int nQualityRule;			// 品质规则
     //NOT CARE...
     public int nSpecialItem;			// 特殊需求物品 commonItem.txt里面的物品子类


 }
// 
// /////////////////////////////////////////////////////////////////////
// //生活技能								(LIFE_04)
// /////////////////////////////////////////////////////////////////////
// #define DBC_LIFEABILITY_PLATFORM		(904)
// class _DBC_LIFEABILITY_PLATFORM
// {
// 	int		nID;
// 	int		nType;
// 	string	szName;
// 	string	szMeshFile;
// 	int		nClickType;
// }
// /////////////////////////////////////////////////////////////////////
// // 表情和颜色转换字符
// /////////////////////////////////////////////////////////////////////
// #define DBC_CODE_CONVERT			( 905 )
// class _DBC_CODE_CONVERT
// {
// 	int		nID;		// ID
// 	string  szName;		// 介绍
// 	string  szCode;		// 字符串形式的十六进制数字
// }
// 
// /////////////////////////////////////////////////////////////////////
// //心法技能								(XINFA_02)
// /////////////////////////////////////////////////////////////////////
// #define DBC_XINFA_XINFADEFINE	(906)
// class _DBC_XINFA_XINFADEFINE
// {
// 	int		nID;			//心法ID
// 	int		nIDMenpai;		//门派ID
// 	string	szName;			//心法名称
// 	string	szTips;			//心法描述
// 	string	szIconName;		//图标名称
// }
 /////////////////////////////////////////////////////////////////////
 //心法升级花费						(XINFA_02)
 /////////////////////////////////////////////////////////////////////
 public class Spend
 {
 	public int		dwSpendMoney;			// 需要花费的金钱数
 	public int		dwSpendExperience;		// 需要花费的经验值
 }
 //#define DBC_XINFA_STUDYSPEND	(907)
 public class _DBC_XINFA_STUDYSPEND
 {
 	public int		nLevel;			            // 心法的等级 [2/18/2012 SUN]
 	public Spend[]	StudySpend = new Spend[82];	// 每个心法对应的消耗 [2/18/2012 SUN]
 }
//////////////////////////////////////////////////////////////////////////
//心法CD重置消耗 XINFA
//////////////////////////////////////////////////////////////////////////
 public class _DBC_XINFA_RESETCD_SPEND
 {
     public int nID;
     public int nMoney; //cd清零消耗, per min
 }
 
// /////////////////////////////////////////////////////////////////////
// //称号列表						(TITLE)
// /////////////////////////////////////////////////////////////////////
// #define DBC_TITLE_NAME			(908)
// class _DBC_TITLE_NAME
// {
// 	int		nTitleIndex;			// 称号的索引
// 	uint	nReserve;				// 类型保留
// 	string	szTitle;				// 称号的名字
// 	string	szTitleDesc;			// 称号的描述
// 
// 	int		nValue[3];////zzh+
// 
// 	string	szTitleDesc2;			////ZZH+
// }
// /////////////////////////////////////////////////////////////////////
// //阵营数据	
// /////////////////////////////////////////////////////////////////////
// #define DBC_CAMP_DATA	(909)
// 
// /////////////////////////////////////////////////////////////////////
// //技能	skillData_v1.txt(读取表)	
// //[2010-10-27] cfp+
// /////////////////////////////////////////////////////////////////////
// #define DBC_SKILLDATA_V1_DEPLETE					(910)
// //旧表结构
// //class _TempStructForSringAndInt
// //{
// //	string	szName;
// //	int		nValue;
// //}
// //class _DBC_SKILLDATA_V1_DATA_PAIR
// //{
// //	int		nID;
// //	int		nCeHuaID;				//策划用ID
// //	string  szDesc;
// //	string  szDesc2;
// //	string  szDesc3;
// //	int		nNotCare1[4];
// //	string	szNotCare1;
// //	int		nNotCare2[3];
// //	string	szNotCare2;
// //	int		nNotCare3[3];
// //	string	szNotCar3;
// //	int		nNotCare4[6];
// //	_TempStructForSringAndInt	nNotCare[12];
// //}
// //
////class _DBC_SKILLDATA_V1_DEPLETE
////{

////    int		nId;				//  ID	  
////    int		nCeHuaId;			//	策划专用ID	
////    string	szEffectDesc;		//	效果的简要逻辑说明
////    int		nNeedLevel;			//	
////    int		nNeedMoney;			//
////    int		nSkillLogicid;		//	技能逻辑ID	

////    int		nCooldownTime;		//	冷却时间	
////    int		nJuqiTime;			//	聚气时间(ms)	
////    int		nDirectTime;		//	引导时间(ms)	
////    string  szCondition1Desc;	//	条件或消耗参数说明
////    int		nCondition11;		//	条件或消耗参数值	
////    int	    nCondition12;		//	条件或消耗参数值	
////    string  szCondition2Desc;	//	条件或消耗参数说明
////    int		nCondition21;		//	条件或消耗参数值	
////    int		nCondition22;		//	条件或消耗参数值	
////    string  szCondition3Desc;	//	条件或消耗参数说明
////    int		nCondition31;		//	条件或消耗参数值	
////    int		nCondition32;		//	条件或消耗参数值	
////    string	szCondition4Desc;	//	条件或消耗参数说明
////    int		nCondition41;		//	条件或消耗参数值	
////    int		nCondition42;		//	条件或消耗参数值	
////    string  szCondition5Desc;	//	条件或消耗参数说明
////    int		nCondition51;		//	条件或消耗参数值	
////    int		nCondition52;		//	条件或消耗参数值	
////    string  szCondition6Desc;	//	参数说明

////    // 参数对
////    _DBC_SKILLDATA_V1_DATA_PAIR	paraPair[12];

////    string Skill_Desc_Interface;

////}
// 
 //新表结构
public class _SringAndInt
{
    public string szName;
    public int nValue;
};

public class _ConDepTerm
{
    public string szConditionDesc;	//	条件或消耗参数说明
    public int nType;				//消耗类型
    public int nParam0;			//参数1
    public int nParam1;			//参数2
};

public class _DBC_SKILLDATA_V1_DEPLETE
{
 
 	public int		    nId;				//  ID	  
 	public int		    nCeHuaId;			//	策划专用ID	
 	public string	    szEffectDesc;		//	效果的简要逻辑说明
 	public string       szDesc2;			//  技能描述(FOR TIP)
 	public int		    nNeedLevel;			//	学习等级要求
 	public int		    nNeedMoney;			//	学习金钱要求
 	public int		    nSkillLogicid;		//	技能逻辑ID
 	public int		    nCooldownTime;		//	冷却时间	
 	public int		    nJuqiTime;			//	聚气时间(ms)	
 	public int		    nDirectTime;		//	引导时间(ms)	
    public _ConDepTerm[] aCondition    = new _ConDepTerm[3];	//	条件或消耗参数说明(3组)
 	public int		    nTargetLevel;		//	目标等级    
 	public int		    nReserve1;			// 预留1
 	public int		    nReserve2;			// 预留2
 	// 参数对
    public _SringAndInt[] aDescriptors = new _SringAndInt[12];//数值和对应说明 
};
// 
// /////////////////////////////////////////////////////////////////////
// //称号列表						(MissionKind)
// /////////////////////////////////////////////////////////////////////
// #define DBC_MISSION_KIND			(911)
// class _DBC_MISSION_KIND
// {
// 	int		nIndex;					// 任务种类的索引
// 	string	szKindName;				// 任务种类的名字
// }
// 
 /////////////////////////////////////////////////////////////////////
 // 表情和颜色转换字符
 /////////////////////////////////////////////////////////////////////
 //#define DBC_STRING_DICT			( 912 )
 public class _DBC_STRING_DICT
 {
 	public int		nID;			// ID
 	public string  szKey;			// 关键字
 	public string  szString;		// 字符串
 }
// 
// 
// /////////////////////////////////////////////////////////////////////
// // 环境音效列表
// /////////////////////////////////////////////////////////////////////
// #define DBC_ENV_SOUND			( 913 )
// class _DBC_ENV_SOUND
// {
// 	int		nID;			// ID
// 	uint	nSoundID;		// 音效ID
// 	uint	nXPos;			// 声源的X坐标
// 	uint	nZPos;			// 声源的Z坐标
// 	uint	nDistance;		// 能听到声音的最大距离
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 物品规则表
// /////////////////////////////////////////////////////////////////////
// #define DBC_ITEM_RULE			( 914 )
 public class _DBC_ITEM_RULE
 {
 	public int		nID;			// ID
 	public int		bCanDrop;		// 是否可丢弃
 	public int		bCanOverLay;	// 是否可重叠
 	public int		bCanQuick;		// 是否可放入快捷栏
 	public int		bCanSale;		// 是否可以出售给NPC商店
 	public int		bCanExchange;	// 是否可以交易
 	public int		bCanUse;		// 是否可以使用
 	public int		bPickBind;		// 是否是拾取邦定
 	public int		bEquipBind;		// 是否是装备邦定
 	public int		bUnique;		// 是否是唯一
 	////zzh- int		nNotCare[2];
 	public int[]		nNotCare = new int[4];////zzh+
 }
 
// /////////////////////////////////////////////////////////////////////
// // 过滤词词汇表
// /////////////////////////////////////////////////////////////////////
// #define DBC_TALK_FILTER			( 915 )
// class _DBC_TALK_FILTER
// {
// 	int		nID;			// ID
// 	string	szString;		// 过滤词汇（即：不能说的词汇）
// 	int		nNotCare[4];
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 等级金钱对应表
// /////////////////////////////////////////////////////////////////////
// #define DBC_LV_MAXMONEY			( 916 )
// class _DBC_LV_MAXMONEY
// {
// 	int		nID;			//ID
// 	int		nLv;			//等级
// 	int		nMaxMoney;		//最大金钱
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 聊天动作命令表
// /////////////////////////////////////////////////////////////////////
// #define DBC_TALK_ACT			( 918 )
// class _DBC_TALK_ACT
// {
// 	int		nID;			//ID
// 	string	szCmd;			//命令
// 	string	szNobody;		//没有目标时的字串
// 	string	szMyself;		//目标自己时的字串
// 	string	szOther;		//目标是其他玩家时的字串
// 	int		nNotCare;
// 	string	szAct;
// 	//string	szNotCare[2];
// 	string	szIconName;		//图标
// 	string	szToolTip;		//提示信息
// 	//string	szAct;			//收到聊天信息时需要做的动作命令串
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 聊天限制配置表
// /////////////////////////////////////////////////////////////////////
// #define DBC_TALK_CONFIG			( 919 )
public class _DBC_TALK_CONFIG
 {
     public int nID;			//ID 依据ENUM_CHAT_TYPE
     public string szChannelName;	//频道名称
     public string szChannelHeader;	//频道前导字符
     public int nTimeSlice;		//时间间隔（单位：秒）
     public int nNeedType1;		//消耗类型1 依据CHAT_NEED_TYPE
     public int nNeedValue1;	//消耗数值1
     public int nNeedType2;		//消耗类型2 依据CHAT_NEED_TYPE
     public int nNeedValue2;	//消耗数值2
     public int nNeedType3;		//消耗类型3 依据CHAT_NEED_TYPE
     public int nNeedValue3;	//消耗数值3
     public int nNeedLv;		//等级限制，这里标称的是最低多少级才能在此频道发言
 }
// 
// /////////////////////////////////////////////////////////////////////
// // 发型改变消耗表
// /////////////////////////////////////////////////////////////////////
// #define DBC_HAIR_STYLE			( 920 )
// class _DBC_HAIR_STYLE
// {
// 	int		nID;			//ID索引
// 	int		nRaceID;		//性别区分
// 	int		nItemTableID;	//消耗物品ID
// 	int		nItemCount;		//消耗物品数量
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 怪物头顶泡泡表
// /////////////////////////////////////////////////////////////////////
// #define DBC_MONSTER_PAOPAO		( 921 )
// class _DBC_MONSTER_PAOPAO
// {
// 	int		nID;			//ID索引
// 	string	szPaoPaoTxt;	//怪物要说的头顶文字
// }
// 
// 
//#define DBC_CAMP_AND_STAND		(922)	//阵营
public class _DBC_CAMP_AND_STAND
{
    public int campId;
    public int[] campData = new int[16];
}

// /////////////////////////////////////////////////////////////////////
// //字体信息颜色表
// /////////////////////////////////////////////////////////////////////
// #define DBC_SYSTEM_COLOR		(923)
// class _DBC_SYSTEM_COLOR
// {
// 	const static int NAME_PLAYER_MYSELF		 = 1;	//名字_主角
// 	const static int NAME_PLAYER_OTHER		 = 2;	//名字_其他玩家
// 	const static int NAME_NPC_PEACE			 = 3;	//名字_和平NPC
// 	const static int NAME_NPC_ATTACK_ACTIVE	 = 4;	//名字_主动攻击NPC
// 	const static int NAME_NPC_ATTACK_PASS	 = 5;	//名字_不主动攻击NPC
// 	const static int NAME_NPC_CANNT_ATTACK	 = 6;	//不可攻击的NPC
// 	const static int NAME_PET				 = 7;	//宠物名字
// 	const static int NANE_SELF_PET			 = 8;	//自己的宠物
// 
// 	const static int NANE_ISACK_NOTRETORT_PLAYER	 = 10;	//可以攻击_不还击的玩家
// 	const static int NANE_ISACK_ISTRETORT_PLAYER	 = 11;	//可以攻击_会还击的玩家
// 	const static int NANE_NOTACK_ISTRETORT_PLAYER	 = 12;	//不可攻击_会还击的玩家
// 	const static int NANE_NOTACK_NOTRETORT_PLAYER	 = 13;	//不可攻击_不还击的玩家
// 	const static int NANE_ISACK_NOTRETORT_MONSTER	 = 14;	//可攻击、不还击、不主动
// 	const static int NANE_ISACK_ISRETORT_MONSTER	 = 15;	//可攻击、会还击、不主动
// 	const static int NANE_ISACK_ISACK_MONSTER		 = 16;	//可攻击、会还击、会主动
// 	const static int NANE_NOTACK_ISACK_MONSTER		 = 17;	//不可攻击、会还击、会主动
// 	const static int NANE_NOTACK_NOTRETORT_MONSTER	 = 18;	//不可攻击、不会还击、不会主动
// 
// 	const static int TITLE_NORMAL			 = 100;	//称号_普通
// 	const static int TITLE_BANGPAI			 = 101;	//称号_帮派职称
// 	const static int TITLE_MOOD				 = 102;	//称号_玩家自定义心情
// 	const static int TITLE_PLAYER_SHOP		 = 103;	//称号_玩家商店
// 
// 	// 势力相关 [9/26/2011 Ivan edit]
// 	//const static int SHILI_NAME		 = 104;	//势力名称的颜色
// 	const static int RELATION_TEAM		 = 104;	//队友的颜色
// 	const static int RELATION_FRIEND		 = 105;	//好友的颜色
// 
// 	const static int UI_DEBUGLISTBOX		 = 1000; //debuglistbox的颜色
// 
// 	int		m_nID;
// 	string  strDesc;
// 	string	strFontColor;
// 	string	strExtenColor;
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 完全匹配过滤表，用在创建帮会、起名字之类的地方，避免使用系统词组
// /////////////////////////////////////////////////////////////////////
// #define DBC_FULLCMP_FILTER		( 924 )
// class _DBC_FULLCMP_FILTER
// {
// 	int		nID;			//ID索引
// 	string	szFilterType;	//用在什么地方，例如：all,guild,team,.....表示用在所有地方，仅帮会，仅组队等等。
// 	string	szFullTxt;		//完全屏蔽词
// }
// 
// /////////////////////////////////////////////////////////////////////
// // 宠物升级需要的经验表
// /////////////////////////////////////////////////////////////////////
// #define DBC_PET_LEVELUP			( 925 )
public class _DBC_PET_LEVELUP
{
    public int nID;			//ID索引
    public int nExp;			//所需要经验
}
// 
// ///////////////////////////////////////////////////////////////////////
// //// 商店结构表  .\Config\ShopTable.txt
// //// cfp+ [2010-10-26]
// ///////////////////////////////////////////////////////////////////////
// //#define DBC_SHOP_TABLE			( 926 )
// //
// ////商店中的商品结构
// //class _MERCHANDISE_LIST_TABLE
// //{
// //	uint			nPID;			// PID(商品编号)
// //	int				nPNUM;			// PNUM(每组多少个)
// //	int				nPMAX;			// PMAX(有限商品的数量上限)
// //	int				nPrice;			// 价格，当该商店货币单位不是 CU_MONEY 时生效
// //	int				nPriceScale;	// 商品折扣
// //	string			nShopColor;		// 商品颜色
// //}
// //
// ////商店结构表
// //class _DBC_SHOP_TABLE
// //{
// //	int						nIndex;					// 商店ID
// //	string					nShopName;				// 商店名称
// //	int						nShopType;				// 商店类型，0普通商店， 1元宝商店
// //
// //	int						nRepairLevel;			// 修理等级
// //	int						nBuyLevel;				// 收购等级
// //	int						nRepairType;			// RepairType(修理类型)
// //	int						nBuyType;				// 商店的收购类型
// //	float					nRepairSpend;			// 修理花费
// //	float					nRepairOkProb;			// 修理成功几率
// //	int						nCurrencyUnit;			// 货币单位 enum CURRENCY_UNIT
// //	int						nCanBuyBack;			// 是否能回购
// //	float					nPriceScale;			// Scale(价格系数)
// //	int						nRefreshTime;			// Time(刷新时间)
// //
// //	int						nShopMaxNum;			// Num(商品数量)
// //	// 随机商店属性
// //	int						nIsRandShop;			// 是否随机商店
// //	string					nShopConfigpath;		// 随机商店配置文件路径
// //	int						nCountForSell;			// 随机库中挑选出来出售的商品数量
// //	int						nCanMultiBuy;			//是否能够购买多个商品
// //	int						nIsRandSort;			// 是否随机商店
// //
// //	int						nAffectByPKValue;		// 是否受杀气值影响
// //	int						nPKValueScale[7];		// 杀气折扣
// //
// //	//_MERCHANDISE_LIST_TABLE		nItemList[50];		// 商品结构
// //
// //}
// 
// /************************************************************************/
// /* 生活技能配方类型表
//    [11/3/2010 Sun]
// */
// /************************************************************************/
// #define  DBC_LIFEABILITY_PRESCR_KIND	(927)
// class _DBC_LIFEABILITY_PRESCR_KIND
// {
// 	int nID;
// 	string szKindName;
// }
// 
// 
// /************************************************************************/
// /* 黄色装备
// //  [11/4/2010 Sun]
// */
// /************************************************************************/
// #define  DBC_ITEM_EQUIP_YELLOW	(928)
// 
// /************************************************************************/
// /* 紫色装备
// // [11/4/2010 Sun]
// */
// /************************************************************************/
// #define DBC_ITEM_EQUIP_PURPLE	(929)
// 
// #define DBC_ITEM_EQUIP	(930)
// 
// #define DBC_NPC_PAOPAO_INDEX (931)
// class _DBC_NPC_PAOPAO_INDEX
// {
// 	int nID;		//	npc id
// 	int nBaseMsgId; //	起始字符串id
// 	int nStep;		//	字符串数量
// }
// 
// /************************************************************************/
// /* NPC列表，用于地图显示npc和怪物等。[5/19/2011 ivan edit]
// /************************************************************************/
// #define DBC_NPC_LIST (932)
// class _DBC_NPC_LIST
// {
// 	int		nID;
// 	int		nSceneID;
// 	int		nDataType;// 数据类型，如 功能：1；商店：2；任务：3；怪物：4 [5/19/2011 ivan edit]
// 	string	szName;
// 	string	szDesc;
// 	int		nLevel;
// 	string	szType;// 怪物类型,如普通 精英 BOSS [5/19/2011 ivan edit]
// 	int		nPosX;
// 	int		nPosY;// 其实是z轴 [5/19/2011 ivan edit]
// }
// 
// /************************************************************************/
// /* 装备打孔消耗表 [7/14/2011 ivan edit]
// /************************************************************************/
// #define DBC_ITEM_SLOT_COST (933)
// class _DBC_ITEM_SLOT_COST
// {
// 	int		ID;
// 	int		roleRate; // 打孔概率 [7/15/2011 ivan edit]
// 	int		moneyCost;
// 	int		needItem1ID;
// 	int		needItem1Num;
// 	int		needItem2ID;
// 	int		needItem2Num;
// }
// 
// /************************************************************************/
// /* 装备强化 [7/19/2011 ivan edit]
// /************************************************************************/
// #define DBC_ITEM_ENHANCE (934)
 public class _DBC_ITEM_ENHANCE
 {
 	public int		ID;
 	public int		equipPoint;
 	public int		strengthenLevel; // 强化等级 [7/19/2011 ivan edit]
    public int		successRate;	 // 成功率 [7/19/2011 ivan edit]
 	public int		needMoney;
 	public int[]   attribute = new int[5];	//  对应目前的基础属性提升的值[9/29/2011 edit by ZL]
 }
 public class _DBC_ITEM_ENCHANCE_RATE
 {
     public int nId;
     public int nDangCiRatio;
     public int nEquipPointRatio;
}
/***************************************************************************/
/*装备升档*/
/***************************************************************************/
 public class _DBC_ITEM_UPLEVEL
 {
     public int ID;  //EquipPoint * 10000 + dangCi
     public int nMoney;
     public int needItem1ID;
     public int needItem1Num;
     public int needItem2ID;
     public int needItem2Num;
 }
// /************************************************************************/
// /* 摄像机动画
// //  [8/4/2011 Sun]
// */
// /************************************************************************/
// #define DBC_CAMERA_ANIMATION	(935)
// class _DBC_CAMERA_ANIMATION
// {
// 	int	ID;
// 	string	szAnimation;	//动画名字
// 	string  szScene;				//场景ID
// }
// 
// /************************************************************************/
// /* 角色境界表
// //  [8/12/2011 ivan edit]
// */
// /************************************************************************/
// #define DBC_CHAR_AMBIT	(936)
// class _DBC_CHAR_AMBIT
// {
// 	int		ID;//ambitLevel
// 	string	ambitName;
// }
// 
// /************************************************************************/
// /* 
// // Bus相关数据表 [8/15/2011 ivan edit]
// */
// /************************************************************************/
// #define BUS_INFO		(937)
// 
// /////////////////////////////////////////////////////////////////////
// //心法升级需求
// /////////////////////////////////////////////////////////////////////
 public class XinfaRequirement
 {
    public int needRoleLevel;			// 角色等级需求 [10/18/2011 Ivan edit]
    public int dependsSkillId;			// 依赖技能id [10/18/2011 Ivan edit]
    public int dependsSkillLevel;		// 依赖技能等级 [10/18/2011 Ivan edit]
 };
 //#define DBC_XINFA_REQUIREMENTS	(938)
 public class _DBC_XINFA_REQUIREMENTS
 {
 	public int					    nXinFaId;			// 要学习的心法的id(SkillTemplate_V1.txt中的ID) [10/18/2011 Ivan edit]
    //public XinfaRequirement[] StudySpend = new XinfaRequirement[12];		// 一个心法现在只有12级 [10/18/2011 Ivan edit]
    public int                      nStepLevel; //心法等级步长，学习需求等级=nStepLevel*xinfaStudyLevel
 };
// 
// /************************************************************************/
// /* 活动配置表
// //  [10/20/2011 sqy edit]
// */
// /************************************************************************/
// #define DBC_ACTIVITY_INFO   (939)
 public class _DBC_ACTIVITY_INFO
 {
     public int nID;
     public int nActivityType;    //活动类型（需要判断分类标签页和副本、非副本）
     public string szActivityName;    //活动名称
     public int IsDayActivity;     //活动的时间类型（分为全天与非全天）
     public string StartTime;     //活动开始时间
     public string EndTime;       //结束时间
     public string nCondition;        //进行活动所需的条件
     public int nLevel;            //等级
     public string szActivityIcon;    //活动图标
     public int szActivityNum;    //活动次数
     public string szDetail;         //详情链接
     public int szHeadTo;         //前往链接
     public int ProfitType;       //收益类型（物品，金钱，经验）
     public int nMark;            //积分
     public int nRemind;          //提示
 }
// /************************************************************************/
// /*
// // 用于视频动画 [11/8/2011 Sun]
// */
// /************************************************************************/
// #define DBC_VIDEO	(940)
// class _DBC_VIDEO
// {
// 	int		nID;
// 	string	szFileName;	//视频文件名 
// }

// 通用编码类 [2/6/2012 Ivan]
public class EncodeBase
{
 public int orgCode;
 public int tarCode;
}

// 剧情对白配置表 [4/24/2012 Ivan]
public class _DBC_Dialogue
{
    public int id;
    public string diglogueGroup;
}

// 随机名字配置表 [5/2/2012 Ivan]
public class _DBC_RANDOM_NAME
{
    public int id;
    public string surname; // 姓
    public int sex; // 名字的性别(1男,0女，通用-1)
    public int repeat;// 名字是否可以重复，重复填1，否则填0
    public string personalName;// 名字
}

// 功能开放配置表 [5/15/2012 Ivan]
public class _DBC_FUNC_OPEN_LIST
{
    public int id;
    public string name;
    public string uiName;
    public int needLevel;
    public int receiveMission;
    public int finishMission;
    public string desc;
    public int specialTag;
}
