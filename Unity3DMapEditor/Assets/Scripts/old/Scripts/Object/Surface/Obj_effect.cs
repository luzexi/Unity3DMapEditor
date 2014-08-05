// Obj_Effect.h
// 
// 不属于地表层的短期物体，例如闪电，魔法，射出的箭
// 这类物体不受场景管理
// 
/////////////////////////////////////////////////////
using UnityEngine;
public class SObject_EffectInit :SObjectInit
{
	public string      m_pszEffectName;
	public bool        m_bLoopEffect;

	public SObject_EffectInit(){
		m_pszEffectName		= "";
		m_bLoopEffect		= false;
	}

	public override void Reset()
    {
		base.Reset();
		m_pszEffectName		= "";
		m_bLoopEffect		= false;
	}
};

//
public class CObject_Effect :CObject_Surface
{
    public CObject_Effect()
    {
        m_bLoopEffect = false;
        m_fEffectTime = 0.0f;
    }

    public override void Initial( object pInit )
    {
        SObject_EffectInit pEffectInit = (SObject_EffectInit)(pInit);
        if (pEffectInit != null)
        {
            if (pEffectInit.m_pszEffectName.Length != 0)
            {
                ChangEffect(pEffectInit.m_pszEffectName, pEffectInit.m_bLoopEffect);
            }
        }
        m_fEffectTime = 0;
        base.Initial(pInit);
    }

    public override void Release()
    {
	    base.Release();
    }

    public override void Tick()
    {
        m_fEffectTime += GameProcedure.s_pTimeSystem.GetDeltaTime()/1000.0f;
        if (mRenderInterface == null)
	    {
            Destroy();
		    return;
	    }
	    else if ( !m_bLoopEffect )
	    {
		    bool bEffectDie = IsEffectStopped();
		    if ( bEffectDie )
		    {
			    Destroy();
			    return;
		    }
	    }
    }

    public virtual bool IsEffectStopped()
    {
        if (!m_bLoopEffect)
        {
            GFX.GfxEffect effect = (GFX.GfxEffect)mRenderInterface;
            if(effect.hasLifeTime())
            {
                if( m_fEffectTime > effect.getLifeTime() )
                {
                    return true;
                }
            }
            else
            {
                if(m_fEffectTime > 5.0f)
                {
                    return true;
                }
            }
            
        }
        return false;
    }

    protected bool	m_bLoopEffect;
    protected float	m_fEffectTime;

    protected void ChangEffect( string pszEffectName, bool bLoop )
    {
        if (pszEffectName.Length != 0)
        {
            if (mRenderInterface != null)
            {
                GFX.GFXObjectManager.Instance.DestroyObject(mRenderInterface);
                
            }
            mRenderInterface = GFX.GFXObjectManager.Instance.createObject(pszEffectName, GFX.GFXObjectType.EFFECT);
        }
        else
        {
            if (mRenderInterface != null)
            {
                GFX.GFXObjectManager.Instance.DestroyObject(mRenderInterface);
                mRenderInterface = null;
            }
        }
        m_bLoopEffect = bLoop;
        m_fEffectTime = 0;
        SetPosition(GetPosition());
    }
};
//--------------------------------------------------
//鼠标点击时的特效
public class CObject_Effect_MouseTarget :CObject_Effect
{
    protected enum Effect_MouseTarget_Type
    {
        Reachable = 6,
        UnReachable= 7
    }
    protected static string mReachMouseTargetName;
    protected static string mUnReachMouseTargetName;
    public	CObject_Effect_MouseTarget() 
    {
        //读取鼠标点击特效名字
        if(mReachMouseTargetName.Length==0)
        {
            _DBC_EFFECT pEffect = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_EFFECT>((int)DataBaseStruct.DBC_EFFECT).Search_Index_EQU((int)Effect_MouseTarget_Type.Reachable);
            mReachMouseTargetName = pEffect.effectName;
        }
        if(mUnReachMouseTargetName.Length == 0)
        {
            _DBC_EFFECT pEffect = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_EFFECT>((int)DataBaseStruct.DBC_EFFECT).Search_Index_EQU((int)Effect_MouseTarget_Type.UnReachable);
            mUnReachMouseTargetName = pEffect.effectName;
        }
       
    }
    public void SetReachAble(bool bReachAble)
    {
        string effectName;
        if (bReachAble)
        {
            effectName = mReachMouseTargetName; 

        }
        else
        {
            effectName = mUnReachMouseTargetName;
        }
        ChangEffect(effectName, false);   
    }
	public void	UpdateAsCursor()
    {
        Vector3 pt = GameProcedure.s_pInputSystem.GetMousePos();
	    Vector3 fvMousePos;
        CObjectManager.Instance.GetMouseOverObject(pt,out fvMousePos);
        SetMapPosition(fvMousePos.x, fvMousePos.z);
    }

    public override void Tick()
    {

    }
};
