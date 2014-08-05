/****************************************\
*										*
* 			   数据池数据结构			*
*					-角色数据			*
*										*
\****************************************/
using System.Collections;
using System.Collections.Generic;

//=================================================================
//-->0. Character 基本属性
//=================================================================
public class SDATA_CHARACTER
{
    public int m_nRaceID;					// 数据表中的ID						

    public int m_nPortraitID;				// 头像ID		
    public string m_strName;					// 角色的名字，从服务器传来			
    public string m_strTitle;					//当前称号
    public byte m_TitleType;				//当前称号类型
    public float m_fHPPercent;				// 生命值百分比						
    public float m_fMPPercent;				// 魔法值百分比						
    public int m_nRage;					// 怒气
    public float m_fMoveSpeed;				// 移动的速度						
    public _CAMP_DATA m_CampData;			// 阵营ID	
    public int m_nOwnerID;					// 所有者ID
    public uint m_OccupantGUID;				// 占有者(是谁打的，死了算谁的)
    public int m_nRelative;				// 相对关系，这个人是否可被攻击，是否主动攻击
    public int m_nModelID;					// 模型ID
    public int m_nMountID;					// 坐骑ID
    public int m_nLevel;					// 等级								
    public bool m_bFightState;				// 战斗状态							
    public int m_nStealthLevel;			// 隐身级别
    public bool m_bSit;						// 坐下状态
    public int m_nAIType;					// AI类型
    public int m_nAmbit;					// 境界
}

//=================================================================
//-->1. Character_NPC 对于NPC
//=================================================================
public class SDATA_NPC :  SDATA_CHARACTER
{

}

//=================================================================
//-->2. Character_PlayerOther 对于其他玩家
//=================================================================
public class SDATA_PLAYER_OTHER :  SDATA_NPC
{
    public int m_nMenPai;					// 五行属性ID									
    //头发外形
    public int m_nHairMeshID;					//-> DBC_CHARACTER_HAIR_GEO`					
    //脸部外形
    public int m_nFaceMeshID;					//-> DBC_CHARACTER_HEAD_GEO					
    public uint m_uHairColor;					//头发颜色
    public int m_nEquipVer;					//角色的装备状态版本号，用于和服务器同步数据的依据		
    public int[] m_nEquipmentID = new int[(int)HUMAN_EQUIP.HEQUIP_NUMBER];	//装备表;用于外形显示				
    public int[] m_nEquipmentGemID = new int[(int)HUMAN_EQUIP.HEQUIP_NUMBER];	//装备宝石表;用于外形显示	
    public uint[] m_nEquipmentEffect = new uint[(int)HUMAN_EQUIP.HEQUIP_NUMBER];// 用于存储装备的特效信息，高16位是强化特效，低位是宝石特效 [10/25/2011 Sun]

    public bool m_bTeamFlag;					// 是否有队伍
    public bool m_bTeamLeaderFlag;				// 是否是队长
    public bool m_bTeamFullFlag;				// 是否是满队
    public bool m_bTeamFollowFlag;				// 是否组队跟随
    public int m_nTeamFollowListCount;			// 跟随列表里的人数
    public uint[] m_aTeamFollowList = new uint[GAMEDEFINE.MAX_TEAM_MEMBER];	// 跟随列表

    public int m_nTitleNum;
    public int m_nCurTitleIndex;
    public _TITLE_[] m_nTitleList = new _TITLE_[GAMEDEFINE.MAX_TITLE_SIZE];	//称号表
    public bool m_bIsInStall;					//是否摆摊中
    public string m_strStallName;					//摊位名
    public uint m_BusObjID;						// 添加BusServerId [8/15/2011 ivan edit]
}


//=================================================================
//-->3. Character_PlayerMySelf 对于玩家自己
//=================================================================

public class SDATA_PLAYER_MYSELF :  SDATA_PLAYER_OTHER
{
    //-----------------------------------------------------
    public int m_nHP;				// 生命点						
    public int m_nMP;				// 魔法点						
    public int m_nExp;				// 经验值	
    public int m_nAmbitExp;
    //int			m_nMaxExp;			// 经验值上限(根据等级查表获得)					
    public int m_nMoney;			// 游戏币数	
    public int m_nStrikePoint;		// 连技点
    public int m_nRMB;				//m_nVigor;			// 活力值
    //int				m_nMaxVigor;		// 活力值上限
    public int m_nDoubleExpTime_Num;			// 双倍经验时间和倍数
    public int m_nMaxEnergy;		// 精力值上限
    public int m_nGoodBadValue;	// 善恶值 [11/30/2010 Sun] 
  //  public PET_GUID_t[] m_guidCurrentPet = new PET_GUID_t[GAMEDEFINE.MAX_CURRENT_PET];	// 当前宠物GUID
    public PET_GUID_t[] m_guidCurrentPet = new PET_GUID_t[GAMEDEFINE.MAX_CURRENT_PET];
    //-----------------------------------------------------
    public int m_nGuild;			// 帮派ID 

    //-----------------------------------------------------
    //一级战斗属性
    public int m_nSTR;				// 外功-金							
    public int m_nSPR;				// 内功-木							
    public int m_nCON;				// 身法-水							
    public int m_nINT;				// 体魄-火							
    public int m_nDEX;				// 智慧-土	
    public int m_nPoint_Remain;	// 剩余待分配点数		
    public int m_nBringSTR;         // 培养点数
    public int m_nBringSPR;
    public int m_nBringCON;
    public int m_nBringINT;
    public int m_nBringDEX;	

    //-----------------------------------------------------
    //二级战斗属性
    public int m_nAtt_Physics;			// 物理攻击力		
    public int m_nDef_Physics;			// 物理防御力					
    public int m_nMaxHP;				// 最大生命点					
    public int m_nMaxMP;				// 最大魔法点					
    public int m_nHP_ReSpeed;			// HP恢复速度  点/秒			
    public int m_nMP_ReSpeed;			// MP恢复速度  点/秒			
    public int m_nHit;					// 命中率						
    public int m_nMiss;				// 闪避率						
    public int m_nCritRate;			// 会心率						
    public int m_nDefCritRate;			// 会心防御 新增 ivan 2010年12月24日						
    //FLOAT			m_fMoveSpeed;			// 移动速度(在基本属性中)					
    public int m_nAttackSpeed;			// 攻击速度		

    // 最新对应关系更改，增加土属性 [11/9/2011 Ivan edit]
    //五行战斗属性
    public int m_nAtt_Magic;			// 	仙术攻击			m_nAtt_Magic;		
    public int m_nDef_Magic;			// 	仙术防御			m_nDef_Magic;		
    public int m_nAtt_Cold;			// 冰攻击-水 			m_nAtt_Cold;				
    public int m_nDef_Cold;			// 冰防御-水 			m_nDef_Cold;				
    public int m_nAtt_Light;			// 电攻击-金 			m_nAtt_Light;
    public int m_nDef_Light;			// 电防御-金 			m_nDef_Light;
    public int m_nAtt_Fire;				// 火攻击-火 			m_nAtt_Fire;	
    public int m_nDef_Fire;				// 火防御-火 			m_nDef_Fire;	
    public int m_nAtt_Posion;			// 毒攻击-木			m_nAtt_Posion;
    public int m_nDef_Posion;		// 毒防御-木			m_nDef_Posion;
    public int m_nAtt_Earth;			// 攻击  -土
    public int m_nDef_Earth;			// 防御  -土
    public int m_nAtt_Wind;			// 攻击  -风
    public int m_nDef_Wind;			// 防御  -风
    public int m_nAtt_Thunder;			// 攻击	 -雷
    public int m_nDef_Thunder;			// 防御  -雷
// 
 	public bool		m_bLimitMove;			//是否限制不能移动
    public bool m_bCanActionFlag1;		//技能受限标记1,用于昏迷催眠
    public bool m_bCanActionFlag2;		//技能受限标记2,用于沉默
    //bool			m_bLimitUseSkill;		//是否限制不能施法
    //bool			m_bLimitHandle;			//是否限制不能进行一切操作

    public bool m_bIsMinorPwdSetup;		// 是否已设置二级密码
    public bool m_bIsMinorPwdUnlocked;	// 是否已经解锁二级密码

    public uint m_nTutorialMask;		//新手引导掩码
// 
// 	//-----------------------------------------------------
// 	//其他属性
    public Dictionary<int, SCLIENT_SKILLCLASS> m_theSkillClass = new Dictionary<int, SCLIENT_SKILLCLASS>();	//技能系
    public Dictionary<int, SCLIENT_SKILL> m_theSkill = new Dictionary<int, SCLIENT_SKILL>();			//技能数据
    public Dictionary<int, SCLIENT_LIFEABILITY> m_theLifeAbility = new Dictionary<int,SCLIENT_LIFEABILITY>();	//生活技能数据
    public Dictionary<int, SCLIENT_PRESCR> m_theSprescr = new Dictionary<int,SCLIENT_PRESCR>();		//配方数据
// 
// 	// 根据新需求，保存一份生活技能配方的数据 [10/8/2011 Sun]
// 	// 这里的数据是客户端读表而得，和服务器没联系
// 	LIFEABILITY_PRESCR_MAP	m_theLifeAbilitySprescr;
// 	//宠物数据
// //	SDATA_PET			m_thePet;			//宠物
}

public class SDATA_SPECIAL_BUS :  SDATA_CHARACTER
 {
// 	uint				busID;				// 索引到BUSINFO表里的ID [8/22/2011 ivan edit]
// 	INT					busHP;					// 生命点
// 	INT					busMaxHP;				// 最大生命点
// 	INT					busDef_Physics;			// 物理防御力
// 	//-----------------------------------------------------
// 	//技能数据
// 	SKILLCLASS_MAP		busSkillClass;		//技能系
// 	SSKILL_MAP			busSkill;				//技能数据
}