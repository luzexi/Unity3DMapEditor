using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class MainMenuBar : MonoBehaviour {

    public List<ActionButton> buttons;
	public UIProgressBar ExperienceBtn;
    void Start()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_CHANGE_BAR,MainMenuBar_ActionUpdate);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_EXP,MainMenuBar_ActionUpdate);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MAX_EXP, MainMenuBar_ActionUpdate);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_NEW_SKILL, MainMenuBar_ActionUpdate);

        KeyCode[] keys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
                         KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0};

        for (int i = 0; i < buttons.Count; i++)
        {
            if ((int)keys[i] >= (int)KeyCode.Alpha0 &&
                (int)keys[i] <= (int)KeyCode.Alpha9)
                buttons[i].RegistShortKey(keys[i], Convert.ToString((int)keys[i] - (int)KeyCode.Alpha0));
            else
                buttons[i].RegistShortKey(keys[i]);
        }
    }

    public void MainMenuBar_ActionUpdate(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_CHANGE_BAR)
        {
            string arg0 = vParam[0];
            if (arg0 == "main")
            {
                int nIndex = int.Parse(vParam[1]);
                if (nIndex <= 0 || nIndex > 10)
                    return;
                int actionId = int.Parse(vParam[2]);
                buttons[nIndex-1].SetActionItemByActionId(actionId);
            }
        }
		else if(eventId == GAME_EVENT_ID.GE_UNIT_EXP)
		{
			ShowExperience();
		}
        else if (eventId == GAME_EVENT_ID.GE_UNIT_MAX_EXP)
        {
            ShowExperience();
        }
        else if (eventId == GAME_EVENT_ID.GE_NEW_SKILL)
        {
            int skillID = int.Parse(vParam[0]);
            CActionItem_Skill  actionSkill = CActionSystem.Instance.GetAction_SkillID(skillID);
            if (actionSkill == null) return;
            for (int i = 0; i < 10; i++)
            {
                int actionID = CActionSystem.Instance.MainMenuBar_Get(i);
                if (actionID == MacroDefine.INVALID_ID)
                {
                    CActionSystem.Instance.MainMenuBar_Set(i,actionSkill.GetID());
                    return;
                }
            }
        }
    }
	
	//显示经验
	void ShowExperience()
	{
		//得到当前经验
        float CurExperience = PlayerMySelf.Instance.GetFloatData("EXP");

        //得到升级需要的经验
        float RequireExperience = PlayerMySelf.Instance.GetFloatData("NEEDEXP");

        //显示进度
        ExperienceBtn.Value = CurExperience / RequireExperience;
        ExperienceBtn.Text = CurExperience.ToString("0") + "/" + RequireExperience.ToString("0"); 
	}

    void ToggleAutoSkill()
    {
        AutoReleaseSkill.Instance.ToggleAutoSkill();
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, AutoReleaseSkill.Instance.isAutoSkill() ? "开始自动释放技能" : "结束自动释放技能");
    }
}
