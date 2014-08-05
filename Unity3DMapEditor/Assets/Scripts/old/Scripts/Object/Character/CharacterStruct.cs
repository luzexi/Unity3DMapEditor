// 特效组
public class SEffectSet
{
    protected uint	m_uEffect;

    public SEffectSet()
    {
	    m_uEffect = 0;
    }

    public virtual void Reset()
	{
		m_uEffect = 0;
	}

    public void SetEffect(uint uEffect)
	{
		m_uEffect = uEffect;
	}

    public uint GetEffect()
	{
		return m_uEffect;
	}
};

public class SImpactEffect:SEffectSet
{
    protected uint		m_dwRefCount;
    protected uint		m_dwImpactID;
    protected bool		m_bLineEffect;			// 连线的特效
    protected uint		m_CreatorID;			// 创建者ID
    protected string	m_lpszCreatorLocator;	// 创建者的绑定点

    public	SImpactEffect()
	{
		m_dwRefCount			= 0;
		m_dwImpactID			= MacroDefine.UINT_MAX;
		m_bLineEffect			= false;
		m_CreatorID				= MacroDefine.UINT_MAX;
		m_lpszCreatorLocator	= "";
	}

	public override void Reset()
	{
		m_dwRefCount			= 0;
		m_dwImpactID			= MacroDefine.UINT_MAX;
		m_bLineEffect			= false;
		m_CreatorID				= MacroDefine.UINT_MAX;
		m_lpszCreatorLocator	= "";
		base.Reset();
	}

    public void SetImpactID(uint dwImpactID) { m_dwImpactID = dwImpactID; }
    public uint GetImpactID() { return m_dwImpactID; }
    public uint GetRefCount() { return m_dwRefCount; }
    public void AddRefCount() { m_dwRefCount++; }
    public void DecRefCount()
	{
		if ( m_dwRefCount > 0 )
			m_dwRefCount--;
	}
    public bool IsLineEffect() { return m_bLineEffect; }
    public void SetLineEffect(bool bSet) { m_bLineEffect = bSet; }
    public uint GetCreatorID() { return m_CreatorID; }
    public void SetCreatorID(uint nCreatorID) { m_CreatorID = nCreatorID; }
    public string GetCreatorLocator() { return m_lpszCreatorLocator; }
    public void SetCreatorLocator(string lpszCreatorLocator) { m_lpszCreatorLocator = lpszCreatorLocator; }
};
//typedef std::map< UINT, SImpactEffect*> CImpactEffectMap;


public class _LOGIC_EVENT_BULLET
{
    public int    m_nBulletID;
    public string m_pszSenderLocator;
    public bool   m_bHitTargetObj;		// 目标是否为角色
    public uint   m_nTargetID;
    public float  m_fTargetX, m_fTargetZ;

	public _LOGIC_EVENT_BULLET()
	{
		m_nBulletID			= MacroDefine.INVALID_ID;
		m_pszSenderLocator	= "";
		m_bHitTargetObj		= true;
		m_nTargetID			= MacroDefine.UINT_MAX;
		m_fTargetX			= -1.0f;
		m_fTargetZ			= -1.0f;
	}

    public void Reset()
	{
		m_nBulletID			= MacroDefine.INVALID_ID;
		m_pszSenderLocator	= "";
		m_bHitTargetObj		= true;
		m_nTargetID			= MacroDefine.UINT_MAX;
		m_fTargetX			= -1.0f;
		m_fTargetZ			= -1.0f;
	}
};

public enum ENUM_LOGIC_EVENT_TYPE
{
	LOGIC_EVENT_TYPE_INVALID	= -1,
	LOGIC_EVENT_TYPE_BULLET,
	LOGIC_EVENT_TYPE_DAMAGE,

	LOGIC_EVENT_TYPE_NUMBERS
};

public class _LOGIC_EVENT
{
    public uint                 m_uBeginTime;			// 技能效果作用的开始时间
    public uint                 m_uRemoveTime;			// 技能效果作用的持续时间
    public uint                 m_nSenderID;			// 攻击者的ID
    public int                  m_nSenderLogicCount;	// 攻击者的逻辑计数

    public ENUM_LOGIC_EVENT_TYPE m_nEventType;			// 类型 ENUM_LOGIC_EVENT_TYPE
	//union
	//{
    public _DAMAGE_INFO         m_damage;			// 机能效果所产生的伤害信息
    public _LOGIC_EVENT_BULLET  m_bullet;			// 子弹
	//};

	public _LOGIC_EVENT()
	{
		m_uBeginTime	= 0;
		m_uRemoveTime	= 1000;
        m_bullet        = new _LOGIC_EVENT_BULLET();
	}

    public void Init(uint nSenderID, int nSenderLogicCount, _DAMAGE_INFO DamageInfo)
    {
        m_nSenderID = nSenderID;
        m_nSenderLogicCount = nSenderLogicCount;

        m_nEventType        = ENUM_LOGIC_EVENT_TYPE.LOGIC_EVENT_TYPE_DAMAGE;
        m_damage = DamageInfo;
    }

	public void Init(uint nSenderID, int nSenderLogicCount, _LOGIC_EVENT_BULLET BulletInfo)
	{
		m_nSenderID			= nSenderID;
		m_nSenderLogicCount	= nSenderLogicCount;

		m_nEventType		= ENUM_LOGIC_EVENT_TYPE.LOGIC_EVENT_TYPE_BULLET;
		m_bullet			= BulletInfo;
	}

	public void Reset()
	{
		m_uBeginTime	= 0;
		m_uRemoveTime	= 1000;

		m_damage.Reset();
		m_bullet.Reset();
	}
};

//typedef std::list< _LOGIC_EVENT* >		CLogicEventList;