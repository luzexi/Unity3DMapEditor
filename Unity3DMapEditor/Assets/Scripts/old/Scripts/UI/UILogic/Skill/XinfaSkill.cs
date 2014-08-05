using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;

public class XinfaSkill : MonoBehaviour
{

    public UIScrollList list;
    public GameObject itemPrefab;
    private List<ActionButton> buttons = new List<ActionButton>();
    private List<SpriteText> txtNames = new List<SpriteText>();
    private List<SpriteText> txtLevels = new List<SpriteText>();
    private List<UIButton> levelUpBtns = new List<UIButton>();
    private List<UIButton> bgBtns = new List<UIButton>();
    private List<string> toolText = new List<string>();
    private SpriteText txtExp = null;
    private SpriteText txtMoney = null;


    TAB_MODE tabMode = TAB_MODE.TAB_SELFXINFA;
    enum TAB_MODE
    {
        TAB_SELFXINFA,
        TAB_PETXINFA,
    }

    void Awake()
    {
        txtExp = transform.FindChild("exp_text").GetComponent<SpriteText>();
        txtMoney = transform.FindChild("money_text").GetComponent<SpriteText>();

        UpdateSkill();
        UpdateMoney();
        UpdateExp();

        registerHandler();
		
		if(gameObject.active)
		{
			CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_XINFASHOW);
		}
    }

    void registerHandler()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_SKILL_UPDATE, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_EXP, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_XINFASKILL_PAGE, OnEvent);
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
        
        if (tabMode == TAB_MODE.TAB_SELFXINFA)
        {
            int num = CActionSystem.Instance.GetSkillActionNum();
            int nXinfaIndex = -1;
            IUIListObject listItem = null;
            for (int i = 0; i < num; i++)
            {
                CActionItem_Skill skill = CActionSystem.Instance.EnumAction(i, ActionNameType.Skill) as CActionItem_Skill;
                if (skill != null && (skill.GetXinfaID() >= SCLIENT_SKILLCLASS.XINFATYPE_JINGJIEBEGIN && skill.GetXinfaID() < SCLIENT_SKILLCLASS.XINFATYPE_SKILLBENGIN))
                {
                    nXinfaIndex++;
                    int nOffset = nXinfaIndex % 3;
                    if (nOffset == 0)
                    {
                        listItem = list.CreateItem(itemPrefab);
						listItem.gameObject.name = "XinfaListItem" +nXinfaIndex;
                        for (int j = 0; j < 3; j++ )
                        {
                            listItem.gameObject.transform.FindChild("Item" + j).gameObject.SetActiveRecursively(false);
                        }
                    }
                    GameObject item = listItem.gameObject.transform.FindChild("Item" + nOffset).gameObject;
                    item.SetActiveRecursively(true);
                    ActionButton skillAction = item.GetComponentInChildren<ActionButton>();
                    if (skillAction != null)
                    {
                        skillAction.SetActionItemByActionId(skill.GetID());
                        skillAction.DisableDrag();
                        skillAction.EnableDoAction = false;
                    }
                    UIButton bg = item.transform.FindChild("Bg").gameObject.GetComponent<UIButton>();
                    if (bg != null)
                    {
                        bg.SetInputDelegate(handlerInput);
                    }

                    SpriteText txt = item.transform.FindChild("NameLabel").gameObject.GetComponent<SpriteText>();
                    if(txt != null)
                        txt.Text = skill.GetName() + "\n" + "等级：" + skill.GetSkillLevel();

                    UIButton levelBtn = item.transform.FindChild("LevelUp").gameObject.GetComponent<UIButton>();
                    if (levelBtn != null)
                    {
                        levelBtn.SetInputDelegate(levelUpInputDelegate);
                        //LogManager.LogWarning("Set Delegate : " + levelBtn.gameObject.name);
                    }
        

                    SCLIENT_SKILLCLASS skillClass = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(skill.GetXinfaID());
                    //升级可操作
                    string error;
                    if (!CDataPool.Instance.isCanSkillLevelUp(skillClass.m_pDefine.nID, skillClass.m_nLevel + 1, out error))
                    {
                        item.transform.FindChild("LevelUp").gameObject.GetComponent<UIButton>().controlIsEnabled = false;
                        //toolText[showIndex] = UIString.Instance.ParserString_Runtime(error);
                    }
                    else
                    {
                        item.transform.FindChild("LevelUp").gameObject.GetComponent<UIButton>().controlIsEnabled = true;
                        //toolText[showIndex] = null;
                    }
                }

            }
        }
        else if (tabMode == TAB_MODE.TAB_PETXINFA)
        {
            //int num = CActionSystem.Instance.GetSkillActionNum();
            ////LogManager.LogWarning("Skill count = " + num);
            //int showIndex = 0;
            //for (int i = 0; i < num; i++)
            //{
            //    CActionItem_Skill skill = CActionSystem.Instance.EnumAction(i, ActionNameType.Skill) as CActionItem_Skill;


            //    if (showIndex >= buttons.Count) break;

            //    //超过51不属于普通技能
            //    if (skill != null && (skill.GetXinfaID() == -1 && skill.GetIDTable() < 51))
            //    {
            //        // LogManager.LogWarning("Xinfa = " + skill.GetXinfaID() + "skill = " + skill.GetDefineID() + "index=" + i);
            //        //这里可能有问题
            //        items[showIndex].SetActiveRecursively(true);

            //        buttons[showIndex].UpdateItemFromAction(skill);
            //        buttons[showIndex].EnableDrag();
            //        buttons[showIndex].EnableDoAction = true;

            //        //update text
            //        txtNames[showIndex].Text = skill.GetName();
            //        txtLevels[showIndex].Text = "等级：" + skill.GetSkillLevel();

            //        levelUpBtns[showIndex].gameObject.SetActiveRecursively(false);

            //        showIndex++;
            //    }
            //}
        }
        if (list.UnviewableArea > 0)
            list.slider.Hide(false);
        else
            list.slider.Hide(true);
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
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_TOGLE_XINFASKILL_PAGE)
        {
            if (!gameObject.active)
            {
                //gameObject.SetActiveRecursively(true);
                UIWindowMng.Instance.ShowWindow("XinfaWindow");
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
        if (skillAction != null)
        {
            SCLIENT_SKILL skill = skillAction.GetImpl() as SCLIENT_SKILL;
            if (skill != null)
                CDataPool.Instance._StudySkill.SendStudySkillEvent(skill.m_pDefine.m_nSkillClass, skill.m_pDefine.m_nMenPai, 1);
            else
                LogManager.LogWarning("handle uplevel failed index=" + nIndex);
        }
    }
    void levelUpInputDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            Transform parent = ptr.targetObj.gameObject.transform.parent;
            ActionButton actionBtn = parent.FindChild("Icon").gameObject.GetComponent<ActionButton>();
            if (actionBtn != null)
            {
                CActionItem_Skill skillAction = actionBtn.CurrActionItem as CActionItem_Skill;
                if (skillAction != null)
                {
                    SCLIENT_SKILL skill = skillAction.GetImpl() as SCLIENT_SKILL;
                    if (skill != null)
                        CDataPool.Instance._StudySkill.SendStudySkillEvent(skill.m_pDefine.m_nSkillClass, skill.m_pDefine.m_nMenPai, 1);

                }
            }
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
 
    public void OnChangeToSelfXinfa()
    {
        handlerChangeTab(TAB_MODE.TAB_SELFXINFA);
    }
    public void OnChangeToPetXinfa()
    {
        handlerChangeTab(TAB_MODE.TAB_PETXINFA);
    }
 
    #endregion



    //关闭按钮
    void CloseBtn_Clicked()
    {
        //gameObject.SetActiveRecursively(false);
        UIWindowMng.Instance.HideWindow("XinfaWindow");
    }
    void ResetUI()
    {
        list.ClearList(true);
    }


    public void handlerInput(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE)
        {
            Transform parent = ptr.targetObj.gameObject.transform.parent;
            ActionButton actionBtn = parent.FindChild("Icon").gameObject.GetComponent<ActionButton>();
            if (actionBtn != null && actionBtn.CurrActionItem != null)
            {
                actionBtn.CurrActionItem.NotifyTooltipsShow();
            }
        }
        else if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
        {
            Transform parent = ptr.targetObj.gameObject.transform.parent;
            ActionButton actionBtn = parent.FindChild("Icon").gameObject.GetComponent<ActionButton>();
            if (actionBtn != null && actionBtn.CurrActionItem != null)
            {
                actionBtn.CurrActionItem.NotifyTooltipsHide();
            }
        }
    }
}
