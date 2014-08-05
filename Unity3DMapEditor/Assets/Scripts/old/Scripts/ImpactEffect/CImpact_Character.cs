using System;
using System.Collections.Generic;
using UnityEngine;
using DBSystem;
/// <summary>
/// 角色附加效果类
/// </summary>
public class CImpact_Character
{
    #region CImpact_Character Interface
    CObject_Character m_Character;
    public CImpact_Character(CObject_Character pChar)
    {
        m_Character = pChar;
    }
    
    public void ChangeImpact(uint idImpact, uint uCreatorID, bool bEnable, bool bShowEnableEffect)
    {
        // ???? [4/19/2012 SUN]
        //_DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU((int)idImpact);
        //object objTemp = (object)pBuffImpact;
        //if (objTemp == null)
        //{
        //    return;
        //}

        if (bEnable)
        {
            InsertImpact(idImpact, uCreatorID, bShowEnableEffect);
        }
        else
        {
            RemoveImpact(idImpact);
        }
    }

    public void RemoveAllImpact()
    {
        foreach (SImpactEffect pImpactEffect in m_mapImpactEffect.Values)
        {
            uint uEffect = pImpactEffect.GetEffect();
            if (uEffect != 0 && m_Character.GetRenderInterface() != null)
            {
                m_Character.GetRenderInterface().DelEffect(uEffect);
            }
        }
        m_mapImpactEffect.Clear();
    }

    public void Tick_UpdateEffect()
    {
        if (m_mapImpactEffect.Count == 0)
        {
            return;
        }

        foreach (SImpactEffect pImpactEffect in m_mapImpactEffect.Values)
        {
            if(pImpactEffect.IsLineEffect())
            {
                UpdateEffectTargetPos(pImpactEffect);
            }
        }
    }

    public void UpdateBuffEffect()
    {
        if (m_Character.GetRenderInterface() != null)
        {
            foreach (KeyValuePair<uint, SImpactEffect> subItem in m_mapImpactEffect)
            {
                uint uImpactID = subItem.Key;
                SImpactEffect pImpactEffect = subItem.Value;
                _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU((int)uImpactID);
                object objTemp = (object)pBuffImpact;
                if (objTemp == null)
                {
                    continue;
                }
                if(pBuffImpact.m_lpszEffect_Continuous.Length > 0 && 
                    m_Character.GetRenderInterface() != null)
                {
                    uint uEffect = m_Character.GetRenderInterface().AddEffect(pBuffImpact.m_lpszEffect_Continuous, pBuffImpact.m_lpszBind_Continuous);
                    pImpactEffect.SetEffect(uEffect);
                }
            }
        }
    }
    #endregion

    private Dictionary<uint, SImpactEffect> m_mapImpactEffect = new Dictionary<uint, SImpactEffect>();

    ////public Dictionary<uint, SImpactEffect> GetImpactEffectMap()
    ////{
    ////    return m_mapImpactEffect;
    ////}

    private bool InsertImpact(uint idImpact, uint uCreatorID, bool bShowEnableEffect)
    {
        SImpactEffect pImpactEffect;
        if (!m_mapImpactEffect.ContainsKey(idImpact))
        {
            pImpactEffect = new SImpactEffect();
            pImpactEffect.SetImpactID(idImpact);
            pImpactEffect.SetCreatorID(uCreatorID);
            m_mapImpactEffect[idImpact] = pImpactEffect;

            _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU((int)idImpact);
            object objTemp = (object)pBuffImpact;
            if (objTemp != null)
            {
                bool bLineEffect = pBuffImpact.m_bLineEffect;
                string strCreatorLocator = pBuffImpact.m_pszCreatorLocator;

                pImpactEffect.SetLineEffect(bLineEffect);
                pImpactEffect.SetCreatorLocator(strCreatorLocator);

                if (bShowEnableEffect)
                {
                    // 效果触发的特效（RenderInterface自动会删除）
                    if (pBuffImpact.m_lpszEffect_Active.Length > 0 && m_Character.GetRenderInterface() != null)
                        m_Character.GetRenderInterface().AddEffect(pBuffImpact.m_lpszEffect_Active, pBuffImpact.m_lpszBind_Active);

                    if (pBuffImpact.m_lpszSound_Active.Length > 0)
                    {
                        //Vector3 fvPos = m_Character.getPosition();
                        ////sunyu此接口是提供给fairy调用的音效接口，所以传入坐标必须是gfx坐标
                        //Vector3 fvGame;
                        //if (!GameProcedure.s_pGfxSystem.Axis_Trans(CRenderSystem.AX_GAME, fvPos,
                        //    CRenderSystem.AX_GFX, fvGame))
                        //{
                        //    return false;
                        //}
                        //CSoundSystemFMod._PlaySoundFunc(pBuffImpact.m_lpszSound_Active, &fvGame.x, false);
                    }
                }
                //需要我们控制的特效
                if (pBuffImpact.m_lpszBind_Continuous.Length > 0 && pBuffImpact.m_lpszEffect_Continuous.Length > 0 && m_Character.GetRenderInterface() != null)
                {
                    uint uEffect = m_Character.GetRenderInterface().AddEffect(pBuffImpact.m_lpszEffect_Continuous, pBuffImpact.m_lpszBind_Continuous);
                    pImpactEffect.SetEffect(uEffect);
                    UpdateEffectTargetPos(pImpactEffect);
                }
            }
        }
        else
        {
            pImpactEffect = m_mapImpactEffect[idImpact];
        }

        pImpactEffect.AddRefCount();

        //更新宠物Frame
        //UpdatePetFrame();

        return true;
    }

    private void RemoveImpact(uint idImpact)
    {
        SImpactEffect pImpactEffect = new SImpactEffect();
        if (m_mapImpactEffect.ContainsKey(idImpact))
        {
            pImpactEffect = m_mapImpactEffect[idImpact];
            pImpactEffect.DecRefCount();
            if (pImpactEffect.GetRefCount() == 0)
            {
                uint uEffect = pImpactEffect.GetEffect();
                if (uEffect != 0 && m_Character.GetRenderInterface() != null)
                {
                    m_Character.GetRenderInterface().DelEffect(uEffect);
                }

                m_mapImpactEffect.Remove(idImpact);
                pImpactEffect = null;
            }
        }
    }

    private void UpdateEffectTargetPos(SImpactEffect pImpactEffect)
    {
        if (pImpactEffect == null)
        {
            return;
        }
        if (pImpactEffect.IsLineEffect())
        {
            uint uEffect = pImpactEffect.GetEffect();
            if ( uEffect != 0 && m_Character.GetRenderInterface() != null && pImpactEffect.GetCreatorID() != MacroDefine.INVALID_ID )
		    {
			    CObject_Character pCreator = (CObject_Character)(CObjectManager.Instance.FindServerObject((int)pImpactEffect.GetCreatorID()));
			    if(pCreator != null)
			    {
				    Vector3 fvPos = Vector3.zero;
				    if ( pCreator.GetRenderInterface() != null && pImpactEffect.GetCreatorLocator().Length > 0 )
				    {
					    pCreator.GetRenderInterface().GetLocator(pImpactEffect.GetCreatorLocator(),ref fvPos );
				    }
				    else
				    {
					    fvPos = pCreator.GetPosition();
				    }
				    m_Character.GetRenderInterface().SetEffectExtraTransformInfos(uEffect, ref fvPos);
			    }
		    }
        }
    }
    public bool isExistEffect(uint idImpact)
    {
        return m_mapImpactEffect.ContainsKey(idImpact);
    }

    public SImpactEffect BuffImpact_GetByIndex(int index)
    {
        if (m_mapImpactEffect.Count > index)
        {
            int curIndex = 0;
            //这样遍历貌似有点问题？
            foreach (uint key in m_mapImpactEffect.Keys)
            {
                if (curIndex == index)
                {
                    return m_mapImpactEffect[key];
                }
                curIndex++;
            }
        }
        return null;
    }
}