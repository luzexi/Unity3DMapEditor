using UnityEngine;
using System;
using System.Collections.Generic;
public class AutoReleaseSkill
{
    const int MAX_SHORTCUT_COUNT = 10;
    int shortCutTokenID_ = 0;
    bool execute_ = false;
    bool canUseNextSkill_ 	= false;  //收到服务器回包，说明可以使用技能
    bool isMovingStat_ 		= false;
    int  targetID_ 			= -1;
	bool enterFightStat_    = false;
    static readonly AutoReleaseSkill sInstance = new AutoReleaseSkill();//唯一的实例
    const uint MAX_WAIT_NEXT_SKILL_TIME = 1000;
    const uint MIN_ACTION_TIME = 1000;//最小发包时间
    static public AutoReleaseSkill Instance
    {
        get
        {
            return sInstance;
        }
    }
    CActionItem[] mItems = new CActionItem[MAX_SHORTCUT_COUNT];
    uint waitNextSkillTime_ = 0;
    uint waitSendSkillTime_ = 0;
    bool needWaitSendSkill_ = false;
    public CActionItem getSuitAbleSkill(CObject_Character target)
    {
        if (!canUseNextSkill_)
        {
            return null;
        }

        for (int i = shortCutTokenID_; i < MAX_SHORTCUT_COUNT; i++)
        {
            CActionItem curItem = mItems[i];
            if (CanUseSkill(curItem, target))
            {
                shortCutTokenID_++;
                waitNextSkillTime_ = GameProcedure.s_pTimeSystem.GetTimeNow();
                shortCutTokenID_ %= MAX_SHORTCUT_COUNT;
                canUseNextSkill_ = false;
                return curItem;
            }
        }

        for (int i = 0; i < shortCutTokenID_; i++)
        {
            CActionItem curItem = mItems[i];
            if (CanUseSkill(curItem,target))
            {
                waitNextSkillTime_ = GameProcedure.s_pTimeSystem.GetTimeNow();
                shortCutTokenID_ = i + 1; 
                shortCutTokenID_ %= MAX_SHORTCUT_COUNT;
                canUseNextSkill_ = false;
                return curItem;
            }
        }
        return null;
    }
	
	public CActionItem getAttackableSkill()
	{
		for (int i = shortCutTokenID_; i < MAX_SHORTCUT_COUNT; i++)
        {
            CActionItem curItem = mItems[i];
            if(isAttackableSkill(curItem))
            {
                shortCutTokenID_++;
				shortCutTokenID_ %= MAX_SHORTCUT_COUNT;
				return curItem;
            }
        }
		
		for (int i = 0; i < shortCutTokenID_; i++)
        {
            CActionItem curItem = mItems[i];
            if (isAttackableSkill(curItem))
            {
                shortCutTokenID_ = i + 1; 
                shortCutTokenID_ %= MAX_SHORTCUT_COUNT;
				return curItem;
            }
        }
		return null;
	}
	
	bool isAttackableSkill(CActionItem item)
	{
		if(item == null)return false;
		bool result = false;
		SCLIENT_SKILL pSkill = item.GetImpl() as SCLIENT_SKILL;
		switch (item.GetType())
        {
            case ACTION_OPTYPE.AOT_SKILL:
			{
                if (pSkill == null) break;
                ENUM_SELECT_TYPE typeSel = (ENUM_SELECT_TYPE)pSkill.m_pDefine.m_nSelectType;
                switch(typeSel)
				{
					case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
                    case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
                    case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
                    case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
				     result = true;
					break;
					default:
						break;
				}
			}
			break;
			default:
			break;
		}

		if(result)
		{	
			OPERATE_RESULT  retVal = pSkill.IsCanUse_CheckCoolDown();
			if(retVal!= OPERATE_RESULT.OR_OK)return false;
			
			retVal = pSkill.IsCanUse_CheckPassive();
			if(retVal!= OPERATE_RESULT.OR_OK)return false;
			
			retVal = pSkill.IsCanUse_CheckLevel(CObjectManager.Instance.getPlayerMySelf().ID,pSkill.m_nLevel);
			if(retVal!= OPERATE_RESULT.OR_OK)return false;
			
			retVal = pSkill.IsCanUse_CheckFightState(CObjectManager.Instance.getPlayerMySelf().ID);
			if(retVal!= OPERATE_RESULT.OR_OK)return false;
			
			retVal = pSkill.IsCanUse_CheckDeplete(CObjectManager.Instance.getPlayerMySelf().ID);
			if(retVal!= OPERATE_RESULT.OR_OK)return false;
			
			retVal = pSkill.IsCanUse_Leaned();
			if(retVal!= OPERATE_RESULT.OR_OK)return false;
		}
		return result;
	}
	
    public bool isMovingState()
    {
        bool curStat = isMovingStat_;
        isMovingStat_ = false;
        return curStat;
    }

    public void StartMove()
    {
        isMovingStat_ = true;
    }
    
    public void waitNextSkill()
    {
        //超时处理
        uint curTime = GameProcedure.s_pTimeSystem.GetTimeNow();
        uint deltaTime = curTime - waitNextSkillTime_;
        if (deltaTime >= MAX_WAIT_NEXT_SKILL_TIME)
        {
            canUseNextSkill_ = true;
            waitNextSkillTime_ = curTime;
        }
    }

    public AutoReleaseSkill()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_CHANGE_BAR, OnEvent);
    }
    
    public void ToggleAutoSkill()
    {
        execute_ = !execute_;
        if (execute_)
        {
            canUseNextSkill_ = true;
        }
    }

    public bool isAutoSkill()
    {
        return execute_;
    }

    public void EndUseSkill()
    {
        enterFightStat_  = true;
		canUseNextSkill_ = true;
    }

    public bool CanUseNextSkill()
    {
        return canUseNextSkill_;
    }

    bool CanUseSkill(CActionItem skillItem,CObject_Character target)
    {
        if (skillItem == null) return false;
		if(target == null)return false;
		if(target.IsDie())return false;
        switch (skillItem.GetType())
        {
            case ACTION_OPTYPE.AOT_SKILL:
                {
                    SCLIENT_SKILL pSkill = skillItem.GetImpl() as SCLIENT_SKILL;
                    if (pSkill == null) break;
                    
					Vector3 avatarPos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
                    Vector3 avatarPosOnPlane = new Vector3(avatarPos.x, 0, avatarPos.z);
            		Vector3   targetPos = new Vector3(-1,0,-1);	
					int ServerID = -1;
					targetPos = new Vector3(target.GetPosition().x, 0, target.GetPosition().z);
					ServerID  = target.ServerID;
					Vector3 PosOnPlane = new Vector3(targetPos.x, 0, targetPos.z);
                    float dir = Vector3.Angle(avatarPosOnPlane, PosOnPlane);
					OPERATE_RESULT result = pSkill.IsCanUse(CObjectManager.Instance.getPlayerMySelf().ID,
                                                              pSkill.m_nLevel,
                                                              ServerID,
                                                              targetPos.x,
                                                              targetPos.z,
                                                              dir);
                    
                    if (result == OPERATE_RESULT.OR_OK) return true;
                }
                break;
            case ACTION_OPTYPE.AOT_PET_SKILL:
                {
                    PET_SKILL pPetSkill = skillItem.GetImpl() as PET_SKILL;
                    if (pPetSkill == null) break;
                    if(Interface.Pet.Instance.IsFighting(pPetSkill.m_nPetNum))
                    {
                        return true;
                    }
                }
                break;
        }    
        return false;
    }

    void OnEvent(GAME_EVENT_ID eventid,List<string> vParam)
    {
        switch(eventid)
		{
			case GAME_EVENT_ID.GE_CHANGE_BAR:
			{
				for (int i = 0; i < MAX_SHORTCUT_COUNT; i++)
		        {
		            int actionID = CActionSystem.Instance.MainMenuBar_Get(i);
		            mItems[i] = null;
		            CActionItem pItem = CActionSystem.Instance.GetActionByActionId(actionID);
		            if (pItem != null)
		            {
		                switch (pItem.GetType())
		                {
		                    case ACTION_OPTYPE.AOT_SKILL:
		                        {
		                            SCLIENT_SKILL pSkill = pItem.GetImpl() as SCLIENT_SKILL;
		                            if (pSkill == null) break;
		                            mItems[i] = pItem;
		                        }
		                        break;
		                    case ACTION_OPTYPE.AOT_PET_SKILL:
		                        {
		                            PET_SKILL pPetSkill = pItem.GetImpl() as PET_SKILL;
		                            if (pPetSkill == null) break;
		                            mItems[i] = pItem;
		                        }
		                        break;
		                }
		            }
		        }
			}
			break;
			
		}
		
    }
    public void SetTargetObject(int ServerID)
    {
        if (targetID_ != ServerID)
        {
            shortCutTokenID_ = 0;
        }
        targetID_ = ServerID;
        waitSendSkillTime_ = 0;
    }

    public void Update()
    {
        if (!execute_) return;
        if (targetID_ == MacroDefine.INVALID_ID) return;
        if (CObjectManager.Instance.getPlayerMySelf().IsAutoFight()) return;
        CObject_Character pChar = CObjectManager.Instance.FindServerObject(targetID_) as CObject_Character;
        if (pChar == null ||pChar.IsDie()) 
		{
			enterFightStat_     = false;
            waitSendSkillTime_  = 0;
			return;
		}
        if (!enterFightStat_) return;
        if (pChar.CannotBeAttack()) return;
        
        if (!canUseNextSkill_)
        {
            waitNextSkill();
            return;
        }
        
        bool needSend = false;
        if (needWaitSendSkill_)
        {
            uint curTime = GameProcedure.s_pTimeSystem.GetTimeNow();
            if (curTime - waitSendSkillTime_ >= MIN_ACTION_TIME)
            {
                needSend = true;
                waitSendSkillTime_ = curTime;
            }
        }
        else
        {
            needSend = true;
        }

        if (needSend)
        {
            tActionItem curSkill = getSuitAbleSkill(pChar);
            if (curSkill == null)
            {
                curSkill = CActionSystem.Instance.GetDefaultAction();
            }
            CActionItem_Skill skill = curSkill as CActionItem_Skill;
            needWaitSendSkill_ = !skill.AutoKeepOn();
            Vector3 fvMouseHitPlan = pChar.GetPosition();
            CursorMng.Instance.MouseCommand_Set(false, pChar, fvMouseHitPlan, curSkill);
            CursorMng.Instance.MouseCommand_Active(CursorMng.Instance.MouseCommand_GetLeft()); 
        }
    }
}