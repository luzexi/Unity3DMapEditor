using System.Collections.Generic;
using System;
using UnityEngine;


public class CLogicEvent_Character
{
    #region CLogicEvent_Character Interface
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="character"></param>
    public CLogicEvent_Character(CObject_Character character)
    {
        this.m_Character = character;
    }

    public void ShowLogicEvent(int iServerObjID, int iLogicCount, bool bShowAll)
    {
        _LOGIC_EVENT pLogicEvent;

        int iterator = 0;
        while (iterator < m_listLogicEvent.Count && m_listLogicEvent.Count != 0)
        {
            pLogicEvent = m_listLogicEvent[iterator];
            if (pLogicEvent.m_nSenderID == iServerObjID
                && pLogicEvent.m_nSenderLogicCount <= iLogicCount)
            {
                DoLogicEvent(pLogicEvent);
                m_listLogicEvent.RemoveAt(iterator);
                if (m_listLogicEvent.Count == 0 && m_Character.IsDie() && m_Character.CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
                {
                    m_Character.OnDead(true);
                }

                if (!bShowAll)
                {
                    break;
                }
            }
            else
            {
                iterator++;
            }
        }
    }

    public void DoLogicEvent(_LOGIC_EVENT pLogicEvent)
    {
        if (pLogicEvent != null)
        {
            switch (pLogicEvent.m_nEventType)
            {
                case ENUM_LOGIC_EVENT_TYPE.LOGIC_EVENT_TYPE_BULLET:
                    DoLogicEvent_Bullet(pLogicEvent);
                    break;
                case ENUM_LOGIC_EVENT_TYPE.LOGIC_EVENT_TYPE_DAMAGE:
                    DoLogicEvent_Damage(pLogicEvent);
                    break;
                default:
                    break;
            }
        }
    }

    public void UpdateLogicEvent()
    {
        if(m_listLogicEvent.Count == 0)
	    {
		    return;
	    }

	    
	    _LOGIC_EVENT pLogicEvent;
        
	    uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
        int  iterator =0;
        while(iterator < m_listLogicEvent.Count && 
		      m_listLogicEvent.Count !=0)
        {
			pLogicEvent = m_listLogicEvent[iterator];
            bool bMustRemove = false;
		    bool bMustDo     = false;
            if(pLogicEvent.m_nSenderID != MacroDefine.UINT_MAX)
            {
                CObject pObject = CObjectManager.Instance.FindServerObject((int)pLogicEvent.m_nSenderID);
                if(pObject!= null)
                {
                    uint elapseTime = pLogicEvent.m_uBeginTime + pLogicEvent.m_uRemoveTime;
					if(pObject.IsLogicReady(pLogicEvent.m_nSenderLogicCount))
				    {
					    LogManager.Log("UpdateLogicEvent IsLogicReady" + this.m_Character.ServerID);
						bMustDo = true;
				    }
				    else if(elapseTime < uTimeNow)
				    {
					    LogManager.Log("UpdateLogicEvent elapseTime" + this.m_Character.ServerID);
						bMustRemove = true;
				    }
                }
                else
                {
                    LogManager.Log("UpdateLogicEvent pObject== null" + this.m_Character.ServerID);
					bMustDo = true;
                }
            }
            else
            {
                LogManager.Log("UpdateLogicEvent pLogicEvent.m_nSenderID == MacroDefine.UINT_MAX" + this.m_Character.ServerID);
				bMustDo = true;
            }

            if(bMustDo || bMustRemove)
            {
                if (pLogicEvent.m_nEventType == ENUM_LOGIC_EVENT_TYPE.LOGIC_EVENT_TYPE_DAMAGE && 
                    pLogicEvent.m_damage.m_nBulletID != MacroDefine.INVALID_ID)
			    {
				    bMustDo = false;
                    if (!bMustRemove)// 超时删除 [6/30/2011 Sun]
                    {
                        break;
                    }
			    }
			    //AxTrace(0, 0, "%s", "Show Damage in Update_LogicEvent!");
			    if(bMustDo)
			    {
				    LogManager.Log("UpdateLogicEvent DoLogicEvent" + this.m_Character.ServerID);
					DoLogicEvent(pLogicEvent);
			    }
				else
				{
					 LogManager.Log("Enter not must do");
				}

                if (!m_listLogicEvent.Remove(pLogicEvent))
                {
                    LogManager.LogWarning("m_listLogicEvent remove failed");
                }
                LogManager.Log("m_Character.IsDie()" + m_Character.IsDie() + " ServerID " + this.m_Character.ServerID + "m_listLogicEvent.Count " + m_listLogicEvent.Count + "m_Character.CharacterLogic_Get() " + m_Character.CharacterLogic_Get());
			    if(m_listLogicEvent.Count == 0 && 
                    m_Character.IsDie() && 
                    m_Character.CharacterLogic_Get() != ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
			    {
                    m_Character.OnDead(true);
			    }
            }
            else
            {
                iterator++;
            }
        }
    }

    public void AddLogicEvent(_LOGIC_EVENT pLogicEvent)
    {
        if (pLogicEvent != null)
        {
            _LOGIC_EVENT pNewLogicEvent = new _LOGIC_EVENT();
            pNewLogicEvent = pLogicEvent;
            pNewLogicEvent.m_uBeginTime = (uint)GameProcedure.s_pTimeSystem.GetTimeNow();
            pNewLogicEvent.m_uRemoveTime = 10000;
            m_listLogicEvent.Add(pNewLogicEvent);
        }
    }

    public void RemoveAllLogicEvent()
    {
		m_listLogicEvent.Clear();
    }

    #endregion

    //// 没用到 public void RemoveLogicEvent(int nLogicCount)
    ////{
    ////    _LOGIC_EVENT pLogicEvent;
    ////    for (int i = 0; i < m_listLogicEvent.Count; i++)
    ////    {
    ////        pLogicEvent = m_listLogicEvent[i];
    ////        if (pLogicEvent.m_nSenderLogicCount == nLogicCount)
    ////        {
    ////            m_listLogicEvent.RemoveAt(i);
    ////            break;
    ////        }
    ////    }
    ////}

    private void DoLogicEvent_Bullet(_LOGIC_EVENT pLogicEvent)
    {
        if (pLogicEvent != null)
        {
            _LOGIC_EVENT_BULLET pBulletInfo = pLogicEvent.m_bullet;
            CObject pSender = CObjectManager.Instance.FindServerObject((int)pLogicEvent.m_nSenderID);
            if (pSender != null)
            {
                UnityEngine.Vector3 fvSenderPos = new UnityEngine.Vector3();
                UnityEngine.Vector3 fvSenderRot = UnityEngine.Vector3.zero;
                CObject_Character pCharacterSender = (CObject_Character)pSender;
                fvSenderPos     = pSender.GetPosition();
				fvSenderPos.y += 1.0f;//temp code
                fvSenderRot.y   = pSender.GetFaceDir();
                if (pCharacterSender != null)
                {
                    if (pBulletInfo.m_pszSenderLocator.Length > 0 &&
                        pCharacterSender.GetRenderInterface() != null)
                    {
                        pCharacterSender.GetRenderInterface().GetLocator(pBulletInfo.m_pszSenderLocator, ref fvSenderPos);
                    }
                }

                SObject_BulletInit initBullet   = new SObject_BulletInit();
                initBullet.m_fvPos              = fvSenderPos;
                initBullet.m_fvRot              = fvSenderRot;
                initBullet.m_idSend             = pLogicEvent.m_nSenderID;
                initBullet.m_nSendLogicCount    = pLogicEvent.m_nSenderLogicCount;
                initBullet.m_nBulletID          = pBulletInfo.m_nBulletID;
                initBullet.m_bSingleTarget      = pBulletInfo.m_bHitTargetObj;
                if (pBulletInfo.m_bHitTargetObj)
                {
                    initBullet.m_fvTargetPos    = new Vector3(-1.0f, -1.0f, -1.0f);
                    initBullet.m_idTarget       = (int)pBulletInfo.m_nTargetID;
                }
                else
                {
                    initBullet.m_fvTargetPos.x	= pBulletInfo.m_fTargetX;
                    initBullet.m_fvTargetPos.z	= pBulletInfo.m_fTargetZ;
                    initBullet.m_fvTargetPos.y = GFX.GfxUtility.getSceneHeight(pBulletInfo.m_fTargetX, pBulletInfo.m_fTargetZ);
                }

                CObject_Bullet pBullet = CObjectManager.Instance.NewBullet(-1);
                pBullet.Initial(initBullet);
            }
        }
    }

    private void DoLogicEvent_Damage(_LOGIC_EVENT pLogicEvent)
    {
        _DAMAGE_INFO pDamageInfo = pLogicEvent.m_damage;
        switch (pDamageInfo.m_nType)
        {
            case _DAMAGE_INFO.DAMAGETYPE.TYPE_INVALID:
                break;
            case _DAMAGE_INFO.DAMAGETYPE.TYPE_EFFECT:
                {
                    LogManager.Log("_DAMAGE_INFO.DAMAGETYPE.TYPE_EFFECT " + m_Character.ServerID);
                    if (pDamageInfo.m_nImpactID != MacroDefine.INVALID_ID)
                    {
                        _DBC_DIRECT_IMPACT pDirectImpact = CDirectlyImpactMgr.Instance.GetDirectlyImpact((uint)pDamageInfo.m_nImpactID);
                        if (pDirectImpact != null)
                        {
                            Vector3 fvPos = new Vector3();

                            if (m_Character.GetRenderInterface() != null && pDirectImpact.m_pszEffectLocator.Length != 0)
                            {
                                m_Character.GetRenderInterface().GetLocator(pDirectImpact.m_pszEffectLocator, ref fvPos);
                            }
                            else
                            {
                                fvPos = m_Character.GetPosition();
                            }

                            if (pDirectImpact.m_pszEffect.Length > 0)
                            {
                                CObject_Effect pObjEffect = CObjectManager.Instance.NewEffect(-1);
                                if (pObjEffect != null)
                                {
                                    SObject_EffectInit initEffect = new SObject_EffectInit();
                                    initEffect.m_fvPos = fvPos;
                                    initEffect.m_fvRot = new Vector3(0.0f, 0.0f, 0.0f);
                                    initEffect.m_pszEffectName = pDirectImpact.m_pszEffect;
                                    initEffect.m_bLoopEffect = false;
                                    pObjEffect.Initial(initEffect);
                                }
                            }

                            if (pDirectImpact.m_pszSound.Length > 0)
                            {
                                fvPos = m_Character.GetPosition();
                               // Vector3 fvGame = new Vector3();
                                
                                //if(!CGameProcedure::s_pGfxSystem.Axis_Trans(CRenderSystem::AX_GAME, fvPos, 
                                //    CRenderSystem::AX_GFX, fvGame))
                                //{
                                //    return;
                                //}
                                //CSoundSystemFMod::_PlaySoundFunc( pDirectlyImpact.m_pszSound, &fvGame.x, false );
                            }
                        }
                    }
                }
                break;
            case _DAMAGE_INFO.DAMAGETYPE.TYPE_HEAL_AND_DAMAGE:
                {
                    LogManager.Log("_DAMAGE_INFO.DAMAGETYPE.TYPE_HEAL_AND_DAMAGE " + this.m_Character.ServerID + " pDamageInfo.m_nHealthIncrement " + pDamageInfo.m_nHealthIncrement);
					if (pDamageInfo.m_nHealthIncrement < 0)
                    {
                        this.m_Character.SetFightState(true);
						LogManager.Log("pDamageInfo.m_nHealthIncrement " + this.m_Character.ServerID + " CharacterLogic_Get " + m_Character.CharacterLogic_Get());
                        if (m_Character.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_IDLE)
                        {
                            LogManager.Log("pDamageInfo.BASE_ACTION_F_BE_HIT " + this.m_Character.ServerID);
							m_Character.ChangeAction((int)ENUM_BASE_ACTION.BASE_ACTION_F_BE_HIT, 1.0f, false, CObject_Character.sDefaultActionFuseTime);
                        }
                    }
                    // 显示伤血信息
                    DisplayDamageBoard(pDamageInfo);
                }
                break;
            case _DAMAGE_INFO.DAMAGETYPE.TYPE_DROP_BOX:
                {
                    uint ObjID = (uint)pDamageInfo.m_aAttachedParams[0];
                    int idOwner = pDamageInfo.m_aAttachedParams[1];
                    float fX = pDamageInfo.m_aAttachedParams[2] / 1000.0f;
                    float fZ = pDamageInfo.m_aAttachedParams[3] / 1000.0f;
                    //创建ItemBox
			        CTripperObject_ItemBox pBox = CObjectManager.Instance.NewTripperItemBox((int)ObjID);
                    pBox.Initial(null);
                    //设置位置
                    pBox.SetMapPosition(fX, fZ);
                    //设置掉落箱的归属
                    pBox.SetOwnerGUID((uint)idOwner);
                }
                break;
            case _DAMAGE_INFO.DAMAGETYPE.TYPE_SKILL_TEXT:
                {
                    // 显示未击中和免疫信息
                    DisplayMissImmuAndSoOn(pDamageInfo);
                }
                break;
            case _DAMAGE_INFO.DAMAGETYPE.TYPE_DIE:
                {
                    m_Character.OnDead(true);
                }
                break;
            default:
                break;
        }
    }

    private void DisplayDamageBoard(_DAMAGE_INFO pDamageInfo)
    {
        if (pDamageInfo == null || m_Character.GetRenderInterface() == null)
		    return;
	    CObject pMySelf = CObjectManager.Instance.getPlayerMySelf();
	    if (pMySelf == null)
		    return;
	    if (m_Character.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER && ( ((CObject_PlayerNPC)m_Character).IsDisplayBoard() == false))
		    return;

	    Vector3 fvPos = m_Character.m_pInfoBoard != null ? m_Character.m_pInfoBoard.GetPosition() : Vector3.zero;
	    uint nCasterID = pDamageInfo.m_nSenderID;
	    uint nReceiverID = pDamageInfo.m_nTargetID;
    	
	    //bool bNeedDisplay = true;
	    bool bDouble = pDamageInfo.m_bIsCriticalHit;

	    // 是否暴击 [12/28/2010 ivan edit]
	    ENUM_DAMAGE_TYPE isCritical = pDamageInfo.m_bIsCriticalHit ? ENUM_DAMAGE_TYPE.DAMAGE_CRITICAL : ENUM_DAMAGE_TYPE.DAMAGE_NORMAL;

	    if (IsMySelf(nReceiverID))
	    {
		    if(0 < pDamageInfo.m_nHealthIncrement)//治疗
		    {
                UISystem.Instance.AddNewBeHitBoard(bDouble, pDamageInfo.m_nHealthIncrement.ToString(),
                    fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_NORMAL, ENUM_DMG_MOVE_TYPE.MOVE_HEAL_HP);
		    }
		    if(0 > pDamageInfo.m_nHealthIncrement)//伤害
		    {
                UISystem.Instance.AddNewBeHitBoard(bDouble, pDamageInfo.m_nHealthIncrement.ToString(),
                    fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_NORMAL, ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_ME);
		    }
		    if(0 < pDamageInfo.m_nManaIncrement)//mana增加
		    {
                UISystem.Instance.AddNewBeHitBoard(bDouble, pDamageInfo.m_nManaIncrement.ToString(),
                    fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_NORMAL, ENUM_DMG_MOVE_TYPE.MOVE_HEAL_MP);
		    }
	    }
	    else if (IsMySelf(nCasterID))
	    {
		    if(0 > pDamageInfo.m_nHealthIncrement)//伤害
		    {
                UISystem.Instance.AddNewBeHitBoard(bDouble, pDamageInfo.m_nHealthIncrement.ToString(),
                    fvPos.x, fvPos.y, isCritical, ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_OTHER);
		    }
	    }
        else if (IsMyPet(nCasterID))
        {
            if (0 > pDamageInfo.m_nHealthIncrement)//伤害
            {
                UISystem.Instance.AddNewBeHitBoard(bDouble, pDamageInfo.m_nHealthIncrement.ToString(),
                    fvPos.x, fvPos.y, isCritical, ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_OTHER);
            }
        }
        else if (IsMyPet(nReceiverID))
        {
            if (0 > pDamageInfo.m_nHealthIncrement)//伤害
            {
                UISystem.Instance.AddNewBeHitBoard(bDouble, pDamageInfo.m_nHealthIncrement.ToString(),
                    fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_NORMAL, ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_ME);
            }
        }
    }

    bool IsMyPet(uint serverId)
    {
        CObject_PlayerMySelf mySelf = CObjectManager.Instance.getPlayerMySelf();
        return mySelf.GetCharacterData().IsMyPet(serverId);
    }

    private bool IsMySelf(uint objServerId)
    {
        CObject mySelf = CObjectManager.Instance.getPlayerMySelf();
        if (mySelf == null)
        {
            return false;
        }
        else if (mySelf.ServerID == objServerId)
        {
            return true;
        }
        return false;
    }

    private void DisplayMissImmuAndSoOn(_DAMAGE_INFO pDamageInfo)
    {
        if (pDamageInfo == null)
        {
            return;
        }

        if (m_Character.GetRenderInterface() == null /*|| GameProcedure.s_pUISystem == null*/)
        {
            return;
        }

        //if (this.GetCharacterType() == CT_MONSTER && ( ((CObject_PlayerNPC*)this).IsDisplayBoard() == FALSE))
        //{
        //    return;
        //}

        Vector3 fvPos = m_Character.m_pInfoBoard != null ? m_Character.m_pInfoBoard.GetPosition() : Vector3.zero;

        //if (m_pMountRenderInterface != null)
        //{
        //    fvCurPos.y += m_fMountAddHeight;
        //}

        //m_cObject.GetRenderInterface().Actor_GetInfoBoardPos(fvPos, fvCurPos);

        CObject pMyself = CObjectManager.Instance.getPlayerMySelf();
        if (pMyself == null)
        {
            return;
        }

        //bool bFlag = false;
        string strMsg = string.Empty;
        switch ((MissFlag_T)pDamageInfo.m_nImpactID)
        {
            case MissFlag_T.FLAG_MISS:
                //strMsg = ChangeDamgeByRule(ENUM_DAMAGE_TYPE.DAMAGE_MISS,0,ENUM_DAMAGE_OPERATION.OPERATION_INVALID);
                //bFlag = true;
                UISystem.Instance.AddNewBeHitBoard(false, "",
                    fvPos.x, fvPos.y, ENUM_DAMAGE_TYPE.DAMAGE_MISS, ENUM_DMG_MOVE_TYPE.MOVE_STATUS);
                break;
            //case MissFlag_T.FLAG_IMMU:
            //    strMsg = ChangeDamgeByRule(ENUM_DAMAGE_TYPE.DAMAGE_MISS, 0, ENUM_DAMAGE_OPERATION.OPERATION_INVALID);
            //    bFlag = true;
            //    break;
            //case MissFlag_T.FLAG_ABSORB:
            //    strMsg = ChangeDamgeByRule(ENUM_DAMAGE_TYPE.DAMAGE_MISS, 0, ENUM_DAMAGE_OPERATION.OPERATION_INVALID);
            //    bFlag = true;
            //    break;
            //case MissFlag_T.FLAG_COUNTERACT:
            //    strMsg = ChangeDamgeByRule(ENUM_DAMAGE_TYPE.DAMAGE_MISS, 0, ENUM_DAMAGE_OPERATION.OPERATION_INVALID);
            //    bFlag = true;
            //    break;
            //case MissFlag_T.FLAG_TRANSFERED:
            //    strMsg = ChangeDamgeByRule(ENUM_DAMAGE_TYPE.DAMAGE_MISS, 0, ENUM_DAMAGE_OPERATION.OPERATION_INVALID);
            //    bFlag = true;
            //    break;
            default:
                break;
        } 
        //m_Character.ShowTalk(strMsg);
        //if (bFlag)
        //{
        //    //GameProcedure.s_pUISystem.AddNewBeHitBoard(false, m_szDamage, fvPos.x, fvPos.y, 0, MOVE_STATUS );
        //}
    }
    
    public bool isEmptyLogicEvent()
    {
        return (m_listLogicEvent.Count == 0);
    }

    private string ChangeDamgeByRule(ENUM_DAMAGE_TYPE dmgType, int dmgNum, ENUM_DAMAGE_OPERATION dmgOperation)
    {
        if (dmgType == ENUM_DAMAGE_TYPE.DAMAGE_INVALID && dmgNum == 0)
        {
            return "";
        }

        //string showImageSet = "FR_ShowText01";
        string showImageSet = string.Empty;
		string resultText = string.Empty;

        //添加所使用的imageset
        resultText += showImageSet;

        //添加伤害数值
        resultText += " ";

        if (dmgNum != 0)
        {
            switch (dmgType)
            {
//                 case ENUM_DAMAGE_TYPE.DAMAGE_ADD_HEALTH:
//                     resultText += "H";
//                     break;
//                 case ENUM_DAMAGE_TYPE.DAMAGE_ADD_MANA:
//                     resultText += "M";
//                     break;
                //case ENUM_DAMAGE_TYPE.DAMAGE_ADD_RAGE:
                //case ENUM_DAMAGE_TYPE.DAMAGE_SUB_RAGE:
                case ENUM_DAMAGE_TYPE.DAMAGE_CRITICAL:
                    resultText += "C";
                    break;
                case ENUM_DAMAGE_TYPE.DAMAGE_NORMAL:
                    resultText += "N";
                    break;
//                 case ENUM_DAMAGE_TYPE.DAMAGE_MYSELF:
//                     resultText += "I";
//                     break;
                case ENUM_DAMAGE_TYPE.DAMAGE_INVALID:
                default:
                    return "";
                    //break;
            }

            //判断调用操作符
            switch (dmgOperation)
            {
                case ENUM_DAMAGE_OPERATION.OPERATION_ADD:
                    resultText += "a";
                    break;
                case ENUM_DAMAGE_OPERATION.OPERATION_SUB:
                    resultText += "s";
                    break;
                case ENUM_DAMAGE_OPERATION.OPERATION_INVALID:
                default:
                    break;
            }

            //操作符在上面显示了，所以取绝对值
            if (dmgNum < 0)
            {
                dmgNum = -dmgNum;
            }
            //else
            //{
            //    dmgNum = dmgNum;
            //}

            resultText += dmgNum.ToString();
        }
        else
        {
            //为了统一格式，没有伤害数值的时候，中间填0 
          //  resultText += "0";
        }

        //添加显示文本
        resultText += " ";
        switch (dmgType)
        {
            
            case ENUM_DAMAGE_TYPE.DAMAGE_CRITICAL:
                resultText += "致命一击";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_MISS:
                resultText += "未击中";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_IMMU:
                resultText += "免疫";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_ABSORB:
                resultText += "吸收";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_COUNTERACT:
                resultText += "抵消";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_TRANSFERED:
                resultText += "转移";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_ENTER_FIGHT:
                resultText += "进入战斗";
                break;
            case ENUM_DAMAGE_TYPE.DAMAGE_LEAVE_FIGHT:
                resultText += "离开战斗";
                break;
            default:
                break;
        }

        return resultText;
    }

    private CObject_Character m_Character;

    private List<_LOGIC_EVENT> m_listLogicEvent = new List<_LOGIC_EVENT>();
}