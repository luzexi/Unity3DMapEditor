using UnityEngine;
using DBSystem;
public class SObject_BulletInit:SObject_EffectInit
{
	public uint		m_idSend;			// 发射者
    public int      m_nBulletID;		// 子弹ID
    public int      m_nSendLogicCount;	// 发起者的逻辑计数

    public bool     m_bSingleTarget;	// 单目标？
    public int      m_idTarget;
    public Vector3  m_fvTargetPos;

	public SObject_BulletInit()
    {
        m_idSend            = MacroDefine.UINT_MAX;
		m_nBulletID			= MacroDefine.INVALID_ID;
        m_nSendLogicCount   = MacroDefine.INVALID_ID;

		m_bSingleTarget		= false;
		m_idTarget			= MacroDefine.INVALID_ID;
		m_fvTargetPos		= new Vector3( -1.0f, -1.0f, -1.0f );
	}

	public override void Reset(){
		base.Reset();
        m_idSend            = MacroDefine.UINT_MAX;
		m_nBulletID			= MacroDefine.INVALID_ID;
        m_nSendLogicCount   = MacroDefine.INVALID_ID;

		m_bSingleTarget		= false;
		m_idTarget			= MacroDefine.INVALID_ID;
		m_fvTargetPos		= new Vector3( -1.0f, -1.0f, -1.0f );
	}
};

public class CObject_Bullet:CObject_Effect
{
    public CObject_Bullet()
    {
        m_idSend            = MacroDefine.UINT_MAX;
        m_pBulletData       = null;
        m_nSendLogicCount   = -1;

        m_bSingleTarget = false;
        m_idTarget      = MacroDefine.INVALID_ID;
        m_fvTargetPos   = new Vector3(-1.0f, -1.0f, -1.0f);
        m_fvStartPos    = new Vector3(-1.0f, -1.0f, -1.0f);
        m_fStartToEndDist = -1.0f;

        m_bAlreadyHit = false;
    }

    public override void Initial( object pInit )
    {
        base.Initial(pInit);
	    SObject_BulletInit pBulletInit= (SObject_BulletInit)(pInit);
	    if ( pBulletInit != null )
	    {
		    m_idSend			= pBulletInit.m_idSend;
		    if ( pBulletInit.m_nBulletID != -1 )
		    {
                m_pBulletData = CDataBaseSystem.Instance.GetDataBase<_DBC_BULLET_DATA>((int)DataBaseStruct.DBC_BULLET_DATA).Search_Index_EQU((int)pBulletInit.m_nBulletID);
		    }
		    else
		    {
			    m_pBulletData	= null;
		    }
            m_nSendLogicCount   = pBulletInit.m_nSendLogicCount;
            m_bSingleTarget     = pBulletInit.m_bSingleTarget;
            m_idTarget          = pBulletInit.m_idTarget;
            m_fvTargetPos       = pBulletInit.m_fvTargetPos;
	    }
	    else
	    {
            m_idSend            = MacroDefine.UINT_MAX;
		    m_pBulletData		= null;
		    m_nSendLogicCount	= -1;

		    m_bSingleTarget		= false;
            m_idTarget          = MacroDefine.INVALID_ID;
		    m_fvTargetPos		= new Vector3( -1.0f, -1.0f, -1.0f );
	    }

	    if(m_pBulletData != null)
	    {
            switch ((ENUM_BULLET_CONTRAIL_TYPE)m_pBulletData.m_nContrailType)
		    {
                case ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_BEELINE:
                case ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_PARABOLA:
                    {
                        m_fvStartPos = GetPosition();
                        if (m_pBulletData.m_szFlyEffect.Length != 0)
                        {
                            ChangEffect(m_pBulletData.m_szFlyEffect, false);
                        }
                    }
                    break;
                case ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_NONE:
                default:
                    {
						//[无轨迹子弹击中效果需要由m_idTarget计算得来 2012/4/18 ZZY]
                        //m_fvStartPos = m_fvTargetPos;
                        if ( m_idTarget != MacroDefine.INVALID_ID )
                        {
                            CObject_Character character = (CObject_Character)CObjectManager.Instance.FindServerObject(m_idTarget);
                            if(character != null && character.GetRenderInterface() != null)
                                character.GetRenderInterface().GetLocator(m_pBulletData.m_szHitEffectLocator, ref m_fvStartPos);
                        }
                        else 
                        {
                            m_fvStartPos = m_fvTargetPos;
                        }
                        SetPosition(m_fvStartPos);
                    }
                    break;
		    }

	    }
	    else
	    {
		    m_fvStartPos	= GetPosition();
	    }

        m_fStartToEndDist   = (m_fvTargetPos - m_fvStartPos).magnitude;
	    m_bAlreadyHit	    = false;
    }

    public override void Tick()
    {
        if ( m_bAlreadyHit )
	    {
            base.Tick();
	    }
	    else
	    {
		    if ( m_pBulletData == null )
		    {
			    m_bAlreadyHit = true;
			    return ;
		    }

		    switch((ENUM_BULLET_CONTRAIL_TYPE)m_pBulletData.m_nContrailType)
		    {
		        case ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_BEELINE:
		        case ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_PARABOLA:
		        {
			        Vector3 fvTargetPos;
			        if ( m_bSingleTarget )
			        {
				        CObject pObj = CObjectManager.Instance.FindServerObject(m_idTarget);
				        if ( pObj != null )
				        {
					        fvTargetPos = pObj.GetPosition();

                            if (pObj.GetRenderInterface() != null &&
                                (m_pBulletData.m_szHitEffectLocator.Length != 0))
                            {
                                pObj.GetRenderInterface().GetLocator(m_pBulletData.m_szHitEffectLocator, ref fvTargetPos);
                            }
                            else
                            {
								fvTargetPos = pObj.GetPosition();
                            }
				        }
				        else
				        {
							fvTargetPos = new Vector3( -1.0f, -1.0f, -1.0f );
                            CObjectManager.Instance.DestroyObject(this);
                            return;
				        }
			        }
			        else
			        {
				        fvTargetPos = m_fvTargetPos;
			        }

                    float fCurTickFlyDist = m_pBulletData.m_fSpeed * GameProcedure.s_pTimeSystem.GetDeltaTime()/ 1000.0f;
                    float fDistSq 			= (fvTargetPos - GetPosition()).sqrMagnitude;
                    if (GFX.GfxUtility.IsLessEqual(fDistSq, fCurTickFlyDist * fCurTickFlyDist))
			        {
                        if (m_pBulletData.m_nContrailType == (int)ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_PARABOLA)
				        {
					        float fStartToEndDist       = m_fStartToEndDist;
                            float fTargetToStartDist    = (fvTargetPos - m_fvStartPos).magnitude;
                            float fCenterToTargetDist   = Mathf.Abs(fTargetToStartDist - fStartToEndDist / 2.0f);
                            float fTemp = (fCenterToTargetDist * 2.0f) / fStartToEndDist;
                            float fAddY = (1.0f - (fTemp * fTemp)) * m_pBulletData.m_fContrailParam;
					        fvTargetPos.y += fAddY;
				        }
                        
                        Vector2 curPos  = new Vector2(GetPosition().x, GetPosition().z);
                        Vector2 targetPos = new Vector2(fvTargetPos.x, fvTargetPos.z);
                        float fDir = GFX.GfxUtility.GetYAngle(curPos, targetPos);
				        SetPosition( fvTargetPos );
				        SetFaceDir(fDir);
				        AlreadyHit();
			        }
			        else
			        {
				        Vector3 vDir = fvTargetPos - GetPosition();
                        vDir.Normalize();
                        Vector3 vFlyLength;
				        vFlyLength.x = vDir.x * fCurTickFlyDist;
				        vFlyLength.y = vDir.y * fCurTickFlyDist;
				        vFlyLength.z = vDir.z * fCurTickFlyDist;
				        fvTargetPos = vFlyLength + GetPosition();
                        if (m_pBulletData.m_nContrailType == (int)ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_PARABOLA)
				        {
					        float fStartToEndDist = m_fStartToEndDist;
                            float fTargetToStartDist = (fvTargetPos - m_fvStartPos).magnitude;
                            float fCenterToTargetDist = Mathf.Abs(fTargetToStartDist - fStartToEndDist / 2.0f);
                            float fTemp = (fCenterToTargetDist * 2.0f) / fStartToEndDist;
                            float fAddY = (1.0f - (fTemp * fTemp)) * m_pBulletData.m_fContrailParam;
					        fvTargetPos.y += fAddY;
				        }
                        Vector2 curPos      = new Vector2(GetPosition().x, GetPosition().z);
                        Vector2 targetPos   = new Vector2(fvTargetPos.x, fvTargetPos.z);
                        float fDir = GFX.GfxUtility.GetYAngle(curPos, targetPos);
				        SetPosition( fvTargetPos );
				        SetFaceDir(fDir);
			        }
		        }
			        break;
		        case ENUM_BULLET_CONTRAIL_TYPE.BULLET_CONTRAIL_TYPE_NONE:
		        default:
		        {
			        AlreadyHit();
		        }
		        break;
		    }
	    }
    }

    protected void AlreadyHit()
    {   
        if(m_idTarget != MacroDefine.INVALID_ID)
	    {
            CObject_Character pChar = (CObject_Character)CObjectManager.Instance.FindServerObject(m_idTarget);
		    if(pChar != null)
		    {
                pChar.ShowLogicEvent((int)m_idSend, m_nSendLogicCount, true);
		    }
	    }
	    m_bAlreadyHit = true;
	    if ( m_pBulletData != null && m_pBulletData.m_szHitEffect.Length > 0 )
	    {
            ChangEffect(m_pBulletData.m_szHitEffect, false);
	    }
	    else
	    {
            Release();
	    }
    }

    protected uint				m_idSend;			// 发射者
    protected _DBC_BULLET_DATA	m_pBulletData;		// 子弹数据
    protected int				m_nSendLogicCount;	// 发起者的逻辑计数

    protected bool				m_bSingleTarget;	// 单目标？
    protected int				m_idTarget;
    protected Vector3			m_fvTargetPos;
    protected Vector3			m_fvStartPos;		// 起始的坐标
    protected float				m_fStartToEndDist;	// 起始点到结束点的距离
    protected bool				m_bAlreadyHit;		// 已经击中

};