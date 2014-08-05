using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;

public class CommonSkill : MonoBehaviour {

    public delegate void EventHandler();

    //public List<GameObject> goButtons;
    public List<GameObject> items;
    private List<ActionButton> buttons = new List<ActionButton>();
    private List<SpriteText> txtNames = new List<SpriteText>();
    private List<SpriteText> txtLevels = new List<SpriteText>();
    private List<UIButton> levelUpBtns = new List<UIButton>();
    private List<UIButton> bgBtns = new List<UIButton>();
    private List<string> toolText = new List<string>();
    private SpriteText txtExp = null;
    private SpriteText txtMoney = null;

    float fLastUpdateCDTime = 0.0f;
    bool bAutoClearCDTime = false;

    TAB_MODE tabMode = TAB_MODE.TAB_SKILL;
    enum TAB_MODE
    {
        TAB_SKILL,
        TAB_JINGJIE,
		TAB_USUALSKILL,
    }

   // private int nBeginIndex = 0;
    void Awake()
    {
        foreach ( GameObject item in items)
        {
            SpriteText[] texts = item.GetComponentsInChildren<SpriteText>();
            for (int i = 0; i < texts.Length; i++ )
            {
                if (texts[i].gameObject.name == "NameLabel")
                    txtNames.Add(texts[i]);
                else if (texts[i].gameObject.name == "LevelLabel")
                    txtLevels.Add(texts[i]);
            }
             ActionButton action = item.GetComponentInChildren<ActionButton>();
             if (action != null)
                 buttons.Add(action);
             UIButton[] btns = item.GetComponentsInChildren<UIButton>();
             for (int i = 0; i < btns.Length; i++)
             {
                 if (btns[i].gameObject.name == "LevelUp")
                 {
                     levelUpBtns.Add(btns[i]);
                     
                 }
                 else if (btns[i].gameObject.name == "Bg")
                 {
                     bgBtns.Add(btns[i]);
                     // 屏蔽背景的Tooltip [4/10/2012 Ivan]
                     //btns[i].SetInputDelegate(handlerInput);
                     toolText.Add(null);
                 }
       
             }
             
        }
        txtExp = transform.FindChild("exp_text").GetComponent<SpriteText>();
        txtMoney = transform.FindChild("money_text").GetComponent<SpriteText>();

        //gameObject.SetActiveRecursively(false);
        UpdateSkill();
        UpdateMoney();
        UpdateExp();

        registerHandler();
    }
    //void Start()
    //{
    //    //UpdateSkill();
    //}
    //void Update()
    //{
    //    if (updateCDEvent != null)
    //    {
    //        if (Time.time >= fLastUpdateCDTime + 1.0)
    //        {
    //            fLastUpdateCDTime = Time.time;
    //            updateCDEvent();
    //        }
    //    }
    //}

    void registerHandler()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_SKILL_UPDATE, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_EXP, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_COMMONSKILL_PAGE, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MONEY, OnEvent);
    }
    void UpdateMoney()
    {
        int money = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money();

        txtMoney.Text = "可用的金钱: " + money;
    }
    void UpdateExp()
    {
        int exp = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Exp();
        txtExp.Text = "可用的经验: " + exp;
    }
    void UpdateSkill()
    {
        if (!gameObject.active) return;

        ResetUI();
        if (tabMode == TAB_MODE.TAB_SKILL)
        {
            int num = CActionSystem.Instance.GetSkillActionNum();
            int showIndex = 0;
            for (int i = 0; i < num; i++)
            {
                if (showIndex >= buttons.Count) break;
                CActionItem_Skill skill = CActionSystem.Instance.EnumAction(i, ActionNameType.Skill) as CActionItem_Skill;
                if (skill != null && skill.GetXinfaID() >= SCLIENT_SKILLCLASS.XINFATYPE_SKILLBENGIN)
                {
                    //LogManager.LogWarning("Xinfa = " + skill.GetXinfaID() + " skill = " + skill.GetDefineID());
                    //这里可能有问题
                    items[showIndex].SetActiveRecursively(true);
                    SCLIENT_SKILL skillData = skill.GetImpl() as SCLIENT_SKILL;
                    buttons[showIndex].UpdateItemFromAction(skill);
                    if (skillData != null && skillData.IsCanUse_CheckLevel(CObjectManager.Instance.getPlayerMySelf().ID, (int)skillData.m_nLevel) == OPERATE_RESULT.OR_OK)
                    {
                        buttons[showIndex].EnableDrag();
                        txtLevels[showIndex].Text = "等级：" + skill.GetSkillLevel();
                    }
                    else
                    {
                        buttons[showIndex].DisableDrag();
                        txtLevels[showIndex].Text = "未学习";
                    }

                    buttons[showIndex].EnableDoAction = true;

                    //update text
                    txtNames[showIndex].Text = skill.GetName();
                    

                    SCLIENT_SKILLCLASS skillClass = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(skill.GetXinfaID());
                    //升级可操作
                    string error;
                    if (!CDataPool.Instance.isCanSkillLevelUp(skillClass.m_pDefine.nID, skillClass.m_nLevel + 1, out error))
                    {
                        levelUpBtns[showIndex].Hide(true);
                        //levelUpBtns[showIndex].controlIsEnabled = false;
                        toolText[showIndex] = UIString.Instance.ParserString_Runtime(error);
                    }
                    else
                    {
                       // levelUpBtns[showIndex].controlIsEnabled = true;
                        levelUpBtns[showIndex].Hide(false);
                        toolText[showIndex] = null;
                    }
                    
                    showIndex++;
                }

            }
        }
        else if(tabMode == TAB_MODE.TAB_JINGJIE)
        {
            int num = CActionSystem.Instance.GetSkillActionNum();
            int showIndex = 0;
            for (int i = 0; i < num; i++)
            {
                if (showIndex >= buttons.Count) break;
                CActionItem_Skill skill = CActionSystem.Instance.EnumAction(i, ActionNameType.Skill) as CActionItem_Skill;
                //LogManager.LogWarning("Xinfa = " + skill.GetXinfaID() + " skill = " + skill.GetDefineID());
                if (skill != null && ( skill.GetXinfaID() >= SCLIENT_SKILLCLASS.XINFATYPE_JINGJIEBEGIN && skill.GetXinfaID() < SCLIENT_SKILLCLASS.XINFATYPE_SKILLBENGIN))
                {
                    //这里可能有问题
                    items[showIndex].SetActiveRecursively(true);

                    buttons[showIndex].UpdateItemFromAction(skill);
                    buttons[showIndex].DisableDrag();
                    buttons[showIndex].EnableDoAction = false;

                    //update text
                    txtNames[showIndex].Text = skill.GetName();
                    txtLevels[showIndex].Text = "等级：" + skill.GetSkillLevel();

                    SCLIENT_SKILLCLASS skillClass = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(skill.GetXinfaID());
                    //升级可操作
                    string error;
                    if (!CDataPool.Instance.isCanSkillLevelUp(skillClass.m_pDefine.nID, skillClass.m_nLevel + 1, out error))
                    {
                        levelUpBtns[showIndex].controlIsEnabled = false;
                        toolText[showIndex] = UIString.Instance.ParserString_Runtime(error);
                    }
                    else
                    {
                        levelUpBtns[showIndex].controlIsEnabled = true;
                        toolText[showIndex] = null;
                    }
                    showIndex++;
                }

            }
        }
		else if(tabMode == TAB_MODE.TAB_USUALSKILL)
		{
			int num = CActionSystem.Instance.GetSkillActionNum();
            //LogManager.LogWarning("Skill count = " + num);
			int showIndex =0;
			for(int i = 0; i < num; i++)
			{
                CActionItem_Skill skill = CActionSystem.Instance.EnumAction(i, ActionNameType.Skill) as CActionItem_Skill;
             

				if(showIndex >= buttons.Count) break;
				
				//超过51不属于普通技能
				if(skill !=null && (skill.GetXinfaID() == -1 && skill.GetIDTable() < 51))
				{
                   // LogManager.LogWarning("Xinfa = " + skill.GetXinfaID() + "skill = " + skill.GetDefineID() + "index=" + i);
					//这里可能有问题
					items[showIndex].SetActiveRecursively(true);
					
					buttons[showIndex].UpdateItemFromAction(skill);
                    buttons[showIndex].EnableDrag();
                    buttons[showIndex].EnableDoAction = true;
					
					//update text
					txtNames[showIndex].Text = skill.GetName();
					txtLevels[showIndex].Text = "等级：" + skill.GetSkillLevel();

                    levelUpBtns[showIndex].gameObject.SetActiveRecursively( false);
           
					showIndex++;
				}
			}
		}
    }
	
    public void OnEvent(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (gAME_EVENT_ID == GAME_EVENT_ID.GE_SKILL_UPDATE)
            UpdateSkill();
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_EXP)
        {
            UpdateExp();
            UpdateSkill();
        }
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_TOGLE_COMMONSKILL_PAGE)
        {
            if (!gameObject.active)
            {
                //gameObject.SetActiveRecursively(true);
                UIWindowMng.Instance.ShowWindow("RoleStudyWindow");
                UpdateSkill();
                UpdateExp();
                UpdateMoney();
            }
            else
                CloseBtn_Clicked();

        }
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_MONEY)
        {
            UpdateMoney();
            UpdateSkill();
        }
    }
    void handlerUplevel(int nIndex)
    {
        if (nIndex < 0 || nIndex >= buttons.Count)
            return;
        CActionItem_Skill skillAction = buttons[nIndex].CurrActionItem as CActionItem_Skill;
        if(skillAction != null)
        {
            SCLIENT_SKILL skill = skillAction.GetImpl() as SCLIENT_SKILL;
            if(skill != null)
                CDataPool.Instance._StudySkill.SendStudySkillEvent(skill.m_pDefine.m_nSkillClass, skill.m_pDefine.m_nMenPai, 1 );
            else
                LogManager.LogWarning("handle uplevel failed index="+nIndex);
        }
    }
    void handlerChangeTab(TAB_MODE mode)
    {
        if (tabMode != mode)
        {
            tabMode = mode;

            UpdateSkill();
            UpdateExp();
            UpdateMoney();
        }
    }

#region ui handler

    public void OnUpLevel1() { handlerUplevel(0); }
    public void OnUpLevel2() { handlerUplevel(1); }
    public void OnUpLevel3() { handlerUplevel(2); }
    public void OnUpLevel4() { handlerUplevel(3); }
    public void OnUpLevel5() { handlerUplevel(4); }
    public void OnUpLevel6() { handlerUplevel(5); }
    public void OnUpLevel7() { handlerUplevel(6); }
    public void OnUpLevel8() { handlerUplevel(7); }
    public void OnUpLevel9() { handlerUplevel(8); }
    public void OnUpLevel10() { handlerUplevel(9); }
    public void OnUpLevel11() { handlerUplevel(10); }
    public void OnUpLevel12() { handlerUplevel(11); }
    public void OnUpLevel13() { handlerUplevel(12); }
    public void OnUpLevel14() { handlerUplevel(13); }
    public void OnUpLevel15() { handlerUplevel(14); }
    public void OnChangeToSkill()
    {
        handlerChangeTab(TAB_MODE.TAB_SKILL);
    }
    public void OnChangeToJingjie()
    {
        handlerChangeTab(TAB_MODE.TAB_JINGJIE);
    }
	public void OnChangeToUsualSkill()
	{
		handlerChangeTab(TAB_MODE.TAB_USUALSKILL);
	}
    public void OnChangeAutoResetCDState()
    {
        bAutoClearCDTime = !bAutoClearCDTime;
    }
#endregion


	
	//关闭按钮
    void CloseBtn_Clicked()
	{
        //gameObject.SetActiveRecursively(false);
        UIWindowMng.Instance.HideWindow("RoleStudyWindow");
	}
    void ResetUI()
    {
        foreach (GameObject item in items)
        {
            item.SetActiveRecursively(false);
        }
    }


    public void handlerInput(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE)
        {
            for (int i = 0; i < bgBtns.Count; i++ )
            {
                if (bgBtns[i].gameObject == ptr.targetObj.gameObject)
                {
                    GameObject bufferToolTip = UIWindowMng.Instance.GetWindowGo("BufferTooltipWindow");
                    if (bufferToolTip != null)
                    {
                        Vector2 ptMouse = GameProcedure.s_pInputSystem.GetMouseUIPos();
                        UIBufferToolTip toolTip = bufferToolTip.GetComponent<UIBufferToolTip>();
                        if(toolText[i] != null)
                            toolTip.ShowTooltip(ptMouse.x, ptMouse.y, toolText[i]);
                    }
                    break;
                }
            }
        }
        else if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
        {
            GameObject bufferToolTip = UIWindowMng.Instance.GetWindowGo("BufferTooltipWindow");
            if (bufferToolTip != null)
            {
                UIBufferToolTip toolTip = bufferToolTip.GetComponent<UIBufferToolTip>();
                toolTip.Hide();
            }
        }
    }
}
