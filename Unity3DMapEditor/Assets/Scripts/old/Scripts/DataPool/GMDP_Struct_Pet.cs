using System;
using System.Collections.Generic;
//-------------------------------------------
//	1) 宠物技能数据结构
//-------------------------------------------
public class PET_SKILL
{
    //宠物门派技能学习时，只有宠物技能号，没有宠物编号，现在用-4444表示这样的
    public static readonly int MENPAI_PETSKILLSTUDY_PETNUM	= -4444;
	public _DBC_SKILL_DATA		m_pDefine;		//技能表定义
	public int					m_nPetNum;		//第几只宠物的技能
	public int					m_nPosIndex;	//第几个技能 (UI同步)
	public bool					m_bCanUse;		//是否可用，开关被动技能

	public void CleanUp( )
	{
		m_pDefine		= null;
		m_nPetNum		= -1;
		m_nPosIndex		= -1;
		m_bCanUse		= false;
	}
};

//-------------------------------------------
//	宠物数据结构
//------------------------------------------
public class SDATA_PET
{

	public SDATA_PET()
	{
		m_nIsPresent	= -1;
		m_GUID.Reset();
		m_idServer		= MacroDefine.UINT_MAX;
		m_nDataID		= MacroDefine.INVALID_ID;
		m_nAIType		= -1;
		m_szName		= "";
		m_nLevel		= -1;
		m_nExp			= -1;
		m_nHP			= -1;
		m_nHPMax		= -1;
		m_nAge			= -1;
		m_nEraCount		= -1;
		m_nHappiness	= -1;
		m_SpouseGUID.Reset();
		m_nModelID		= MacroDefine.INVALID_ID;
		m_nMountID		= MacroDefine.INVALID_ID;
		m_nAttPhysics	= -1;
		m_nAttMagic		= -1;
		m_nDefPhysics	= -1;
		m_nDefMagic		= -1;
		m_nHit			= -1;
		m_nMiss			= -1;
		m_nCritical		= -1;
		m_nAttrStrApt	= -1;
		m_nAttrConApt	= -1;
		m_nAttrDexApt	= -1;
		m_nAttrSprApt	= -1;
		m_nAttrIntApt	= -1;
		m_nAttrStr		= -1;
		m_nAttrCon		= -1;
		m_nAttrDex		= -1;
		m_nAttrSpr		= -1;
		m_nAttrInt		= -1;
		m_nBasic		= -1;
		m_nPot			= -1;
        m_nDefCritical  = -1;
        m_nAttrStrBring = -1;
        m_nAttrConBring = -1;
        m_nAttrDexBring = -1;
        m_nAttrIntBring = -1;
		m_aSkill        = new List<PET_SKILL>();
        m_FakeObj       = null;
	}
    public int IsPresent { get { return m_nIsPresent; } set { m_nIsPresent = value;} }
    public PET_GUID_t GUID { get { return m_GUID; } set { m_GUID = value; } }
    public uint idServer { get { return m_idServer; } set { m_idServer = value; } }
    public int DataID 
    { 
        get { return m_nDataID; } 
        set {
                if (m_nDataID != value)
                {
                    m_nDataID = value;
                    m_FakeObj = CObjectManager.Instance.NewFakePlayerNPC();
                    m_FakeObj.GetCharacterData().Set_RaceID(m_nDataID);
                    m_FakeObj.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
                }
            } 
    }
    public int AIType { get { return m_nAIType; } set { m_nAIType = value; } }
    public string Name { get { return m_szName; } set { m_szName = value;} }
    public int Level { get { return m_nLevel; } set { m_nLevel = value; } }
    public int TakeLevel { get { return m_nTakeLevel; } set { m_nTakeLevel = value; } }
    public int Exp { get { return m_nExp; } set { m_nExp = value; } }
    public int HP { get { return m_nHP; } set { m_nHP = value; } }
    public int HPMax { get { return m_nHPMax; } set { m_nHPMax = value; } }
    public int Age { get { return m_nAge; } set { m_nAge = value; } }
    public int EraCount { get { return m_nEraCount; } set { m_nEraCount = value; } }
    public int Happiness { get { return m_nHappiness; } set { m_nHappiness = value; } }
    public int Savvy { get { return m_nSavvy; } set { m_nSavvy = value; } }
    public PET_GUID_t SpouseGUID { get { return m_SpouseGUID; } set { m_SpouseGUID = value; } }
    public int ModelID { get { return m_nModelID; } set { m_nModelID = value; } }
    public int MountID { get { return m_nMountID; } set { m_nMountID = value; } }
    public int AttPhysics { get { return m_nAttPhysics; } set { m_nAttPhysics = value; } }
    public int AttMagic { get { return m_nAttMagic; } set { m_nAttMagic = value; } }
    public int DefPhysics { get { return m_nDefPhysics; } set { m_nDefPhysics = value; } }
    public int DefMagic { get { return m_nDefMagic; } set { m_nDefMagic = value; } }
    public int Hit { get { return m_nHit; } set { m_nHit = value; } }
    public int Miss { get { return m_nMiss; } set { m_nMiss = value; } }
    public int Critical { get { return m_nCritical; } set { m_nCritical = value; } }
    public int DefCritical { get { return m_nDefCritical; } set { m_nDefCritical = value; } }
    public int AttrStrApt { get { return m_nAttrStrApt; } set { m_nAttrStrApt = value; } }
    public int AttrConApt { get { return m_nAttrConApt; } set { m_nAttrConApt = value; } }
    public int AttrDexApt { get { return m_nAttrDexApt; } set { m_nAttrDexApt = value; } }
    public int AttrSprApt { get { return m_nAttrSprApt; } set { m_nAttrSprApt = value; } }
    public int AttrIntApt { get { return m_nAttrIntApt; } set { m_nAttrIntApt = value; } }
    public int AttrStr { get { return m_nAttrStr; } set { m_nAttrStr = value; } }
    public int AttrCon { get { return m_nAttrCon; } set { m_nAttrCon = value; } }
    public int AttrDex { get { return m_nAttrDex; } set { m_nAttrDex = value; } }
    public int AttrSpr { get { return m_nAttrSpr; } set { m_nAttrSpr = value; } }
    public int AttrInt { get { return m_nAttrInt; } set { m_nAttrInt = value; } }
    public int AttrStrBring{ get{return m_nAttrStrBring;} set {m_nAttrStrBring = value;}}
    public int AttrConBring{ get{return m_nAttrConBring;} set {m_nAttrConBring = value;}}
    public int AttrDexBring{ get{return m_nAttrDexBring;} set {m_nAttrDexBring = value;}}
    public int AttrIntBring { get { return m_nAttrIntBring; } set { m_nAttrIntBring = value; } }
    public int AttrSprBring { get { return m_nAttrSprBring; } set { m_nAttrSprBring = value; } }
    public int Basic { get { return m_nBasic; } set { m_nBasic = value; } }
    public int Pot { get { return m_nPot; } set { m_nPot = value; } }
    public CObject_PlayerNPC FakeObject { get { return m_FakeObj; } set { m_FakeObj = value; } }
    public CObject_Item[] Equipts { get { return m_Equipts; } set { m_Equipts = value; } }
    public PET_SKILL this[int index]
    {
        get {

            if (index < 0)
                throw new NullReferenceException("Out of range: Get petskill:" + index);
            if (index >= m_aSkill.Count)
                return null;
            return m_aSkill[index];
        }
        set
        {
            if (index < 0 )
                throw new NullReferenceException("Out of range: Set petskill:" + index);
			
            if (index >= m_aSkill.Count)
                m_aSkill.Add(value);
			else
				m_aSkill[index] = value;
         
        }
    }

	 int	    m_nIsPresent;	// 宠物是否存在
								// -1:不存在
								// 1:存在	（只知道名字）
								// 2:数据已知（知道详细信息）

	 PET_GUID_t	m_GUID;			// 宠物的GUID
	 uint		m_idServer;		// server端的objID

	 int			m_nDataID;		// 数据表中的ID
	 int			m_nAIType;		// 性格

	 string		m_szName;		// 名称
	 int			m_nLevel;		// 等级
	 int			m_nTakeLevel;		////zzh+ 携带等级
	 int 		m_nExp;			// 经验
	 int 		m_nHP;			// 血当前值
	 int 		m_nHPMax;		// 血最大值

	 int 		m_nAge;			// 当前寿命
	 int 		m_nEraCount;	// 几代宠
	 int 		m_nHappiness;	// 快乐度
	 int 		m_nSavvy;	////zzh+ 悟性

	 PET_GUID_t	m_SpouseGUID;	// 配偶

	 int			m_nModelID;		// 外形
	 int			m_nMountID;		// 座骑ID

	 int 		m_nAttPhysics;	// 物理攻击力
	 int 		m_nAttMagic;	// 魔法攻击力
	 int 		m_nDefPhysics;	// 物理防御力
	 int 		m_nDefMagic;	// 魔法防御力

	 int 		m_nHit;			// 命中率
	 int 		m_nMiss;		// 闪避率
	 int			m_nCritical;	// 会心率
     int        m_nDefCritical; //抗暴

	int			m_nAttrStrApt;	// 力量资质
	int			m_nAttrConApt;	// 体力资质
	int			m_nAttrDexApt;	// 身法资质
	int			m_nAttrSprApt;	// 灵气资质
	int			m_nAttrIntApt;	// 定力资质

	int			m_nAttrStr;		// 力量
	int			m_nAttrCon;		// 体力
	int			m_nAttrDex;		// 身法
	int			m_nAttrSpr;		// 灵气
	int			m_nAttrInt;		// 定力

    int         m_nAttrStrBring;
    int         m_nAttrConBring;
    int         m_nAttrDexBring;
    int         m_nAttrIntBring;
    int         m_nAttrSprBring;
	
    int			m_nBasic;		// 根骨

	int			m_nPot;			// 潜能点
    CObject_PlayerNPC m_FakeObj;

	List<PET_SKILL>			m_aSkill;	// 技能列表
    CObject_Item[] m_Equipts = new CObject_Item[(int)PET_EQUIP.PEQUIP_NUMBER];

    public void CleanUp( )
	{
		m_nIsPresent	= -1;
		m_GUID.Reset();
		m_idServer		= MacroDefine.UINT_MAX;
        m_nDataID = MacroDefine.INVALID_ID;
		m_nAIType		= -1;
		m_szName		= "";
		m_nLevel		= -1;
		m_nExp			= -1;
		m_nHP			= -1;
		m_nHPMax		= -1;
		m_nAge			= -1;
		m_nEraCount		= -1;
		m_nHappiness	= -1;
		m_SpouseGUID.Reset();
        m_nModelID = MacroDefine.INVALID_ID;
        m_nMountID = MacroDefine.INVALID_ID;
		m_nAttPhysics	= -1;
		m_nAttMagic		= -1;
		m_nDefPhysics	= -1;
		m_nDefMagic		= -1;
		m_nHit			= -1;
		m_nMiss			= -1;
		m_nCritical		= -1;
		m_nAttrStrApt	= -1;
		m_nAttrConApt	= -1;
		m_nAttrDexApt	= -1;
		m_nAttrSprApt	= -1;
		m_nAttrIntApt	= -1;
		m_nAttrStr		= -1;
		m_nAttrCon		= -1;
		m_nAttrDex		= -1;
		m_nAttrSpr		= -1;
		m_nAttrInt		= -1;
		m_nBasic		= -1;
		m_nPot			= -1;
        m_nDefCritical  = -1;

        m_nAttrStrBring = -1;
        m_nAttrConBring = -1;
        m_nAttrDexBring = -1;
        m_nAttrIntBring = -1;

        CObjectManager.Instance.DestroyObject(m_FakeObj);
        m_FakeObj       = null;

		for(int i = 0; i<(int)m_aSkill.Count; i++ )
		{
			if(m_aSkill[i] != null)
				m_aSkill[i].CleanUp();
		}

        for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
        {
            m_Equipts[i] = null;
        }
       
	
	}

};