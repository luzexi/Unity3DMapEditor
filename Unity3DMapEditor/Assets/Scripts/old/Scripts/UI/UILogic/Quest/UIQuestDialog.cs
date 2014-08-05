using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;

public class UIQuestDialog : MonoBehaviour
{
	public GameObject cartoonBtn1;
	public GameObject cartoonBtn2;
	
    void Awake()
    {
        InitGo();
        InitClass();

        RegistAllEvents();
        Hide();
    }

    void RegistAllEvents()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_OBJECT_CARED_EVENT, NpcDistanceChanged);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_QUEST_EVENTLIST, NpcEventChange);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE, NpcEventChange);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_QUEST_INFO, NpcEventChange);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_QUEST_CONTINUE_NOTDONE, NpcEventChange);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_STOP_DIALOGUE, FinishDialogue);
    }

    void InitGo()
    {
        EZScreenPlacement ScreenPlacement = gameObject.transform.root.gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
            //ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
			ScreenPlacement.RenderCamera = UISystem.Instance.UiCamrea;
		
		PackedSprite sprite1 = cartoonBtn1.GetComponent<PackedSprite>();
		PackedSprite sprite2 = cartoonBtn2.GetComponent<PackedSprite>();
		if(sprite1 != null)
			//sprite1.SetCamera(UISystem.Instance.UiCamrea);
			sprite1.RenderCamera = UISystem.Instance.UiCamrea;
		
		if(sprite2 != null)
			sprite2.RenderCamera = UISystem.Instance.UiCamrea;
    }

    public ComplexWindow cmpWin = null;
    public SpriteText stName;
    public UIButton acceptBtn;
    public UIButton continueBtn;
	public SpriteText rewardTip;
	public SpriteText TaskReward;
    public UIButton refuseBtn;
	
    void InitClass()
    {
        if (cmpWin == null)
        {
            LogManager.LogError("Can't found ComplexWindow in questDialog.");
            return;
        }
        cmpWin.OpInputDelegate += EventSelect;
        cmpWin.maxTextLength = 65;
    }

    void AcceptClick()
    {
        if (currEvent == GAME_EVENT_ID.GE_QUEST_INFO)
            CUIDataPool.Instance.SendAcceptEvent();

        CloseWindow();
    }

    int rewardItemSel = -1;
    void ContinueClick()
    {
        if (currEvent == GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE)
        {
            CUIDataPool.Instance.SendCompleteEvent(rewardItemSel);
        }

        CloseWindow();
    }

    void RefuseClick()
    {
        if (currEvent == GAME_EVENT_ID.GE_QUEST_INFO)
            CUIDataPool.Instance.SendRefuseEvent();
        CloseWindow();
    }

    void CloseWindow()
    {
        CObjectManager.Instance.CareObject(npcId, false, "Quest");
        Hide();
    }

    void ShowWindow()
    {
        npcId = InterfaceTarget.Instance.GetDialogNpcID();
        CObjectManager.Instance.CareObject(npcId, true, "Quest");
        Show();
    }

    void ShowNpcName()
    {
        string name = InterfaceTarget.Instance.GetDialogNpcName();
        stName.Text = name + "：";
    }

    string dialogueFeedback = "";
    public void FinishDialogue(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_STOP_DIALOGUE)
        {
            if (dialogueFeedback != "")
            {
                EventSelect(dialogueFeedback);
                dialogueFeedback = "";
            }
        }
    }

    int npcId = -1;
    GAME_EVENT_ID currEvent;
    public void NpcEventChange(GAME_EVENT_ID eventId, List<string> vParam)
    {
        ShowWindow();
        ShowNpcName();

        currEvent = eventId;
        if (eventId == GAME_EVENT_ID.GE_QUEST_EVENTLIST)
        {
            if (!CheckIsDialogue())
            {
                ShowEventList();
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE)
        {
            // 查看奖励物品的对话框
            MissionRewardUpdate();
        }
        else if (eventId == GAME_EVENT_ID.GE_QUEST_INFO)
        {
            //在接任务时，看到的任务信息
            //ShowWindow();
            QuestInfoUpdate();
        }
        else if (eventId == GAME_EVENT_ID.GE_QUEST_CONTINUE_NOTDONE)
        {
            //接受任务后，再次和npc对话，所得到的任务需求信息，(任务未完成)
            MissionContinueUpdate(false);
        }
        else if (eventId == GAME_EVENT_ID.GE_QUEST_CONTINUE_DONE)
        {
            //接受任务后，再次和npc对话，所得到的任务需求信息，(任务未完成)
            MissionContinueUpdate(true);
        }
    }

    /// <summary>
    /// 注意，如果要用npc对话的流程来播放剧情，并且获得反馈的话，
    /// 必须确保服务器发送过来的只有开始标记和一个任务可选项，
    /// 剧情结束后，会自动回馈服务器这个可选项。
    /// </summary>
    /// <returns></returns>
    private bool CheckIsDialogue()
    {
        int listCount = CUIDataPool.Instance.m_pEventList.m_yItemCount;
        if (listCount != 2)
            return false;

        // 1)check startTag;
        ScriptEventItem item = CUIDataPool.Instance.m_pEventList.GetItem((byte)0);
        if (item.m_nType != ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT ||
            !CUIDataPool.Instance.IsDialogue(item.m_strString.m_szString))
        {
            return false;
        }
        //2) mark finishTag
        item = CUIDataPool.Instance.m_pEventList.GetItem((byte)1);
        dialogueFeedback = item.m_idScript + "|" + item.m_index + "|" + 1;
        //3) 播放剧情任务的时候不需要显示npc对话界面
        CloseWindow();

        return true;
    }

    private void MissionContinueUpdate(bool missIsDone)
    {
        cmpWin.CleanAll();
		
		string parseRes = "";
        int txtNum = CUIDataPool.Instance.m_pMissionDemandInfo.m_yTextCount;
        for (int i = 0; i < txtNum; i++)
        {
			int index;
            string txt = CUIDataPool.Instance.m_pMissionDemandInfo.m_aText[i].m_szString;
            if(UIString.Instance.ParseAcceptQuest(txt,out index))
			{
				string mark = "#{M_MUBIAO}";
				string TargetText = txt.Substring(index + mark.Length, txt.Length - mark.Length);
                UIString.Instance.ParserString_RuntimeNoHL(TargetText,out parseRes);
			}
			else
				cmpWin.AddText(txt);
        }

        int demandNum = CUIDataPool.Instance.m_vecQuestDemandItem.Count;
        //if(demandNum > 0)
            //cmpWin.AddText("需要物品:");
        for (int i = 0; i < demandNum; i++)
        {
//             SMissionBonus pBonus = new SMissionBonus();
//             pBonus.m_ItemBonus.m_uItemID = CUIDataPool.Instance.m_pMissionDemandInfo.m_aDemandItem[i].m_uItemID;
//             pBonus.m_ItemBonus.m_yCount = CUIDataPool.Instance.m_pMissionDemandInfo.m_aDemandItem[i].m_yCount;
            QuestDemandItem demandItem = CUIDataPool.Instance.m_vecQuestDemandItem[i];
            CommonAddDemand(demandItem);
        }
        cmpWin.ReLayout();
		
        acceptBtn.gameObject.SetActiveRecursively(false);
        continueBtn.gameObject.SetActiveRecursively(missIsDone);
        continueBtn.Text = parseRes + " (继续)";
        //refuseBtn.Text = "取消";
    }

    private void CommonAddDemand(QuestDemandItem demandItem)
    {
        if (demandItem == null)
        {
            LogManager.LogError("GetMissionContinue_Bonus Empty Event Item");
            return;
        }

        cmpWin.AddItem((uint)demandItem.pItemImpl.GetID(), demandItem.pDemandItem.m_yCount);
    }

    private void QuestInfoUpdate()
    {
        cmpWin.CleanAll();
		string parseRes = "";

        int txtNum = CUIDataPool.Instance.m_pMissionInfo.m_yTextCount;
        for (int i = 0; i < txtNum; i++)
        {
			int index;
            string txt = CUIDataPool.Instance.m_pMissionInfo.m_aText[i].m_szString;
			if(UIString.Instance.ParseAcceptQuest(txt,out index))
			{
				string mark = "#{M_MUBIAO}";
				string TargetText = txt.Substring(index + mark.Length, txt.Length - mark.Length);
                UIString.Instance.ParserString_RuntimeNoHL(TargetText,out parseRes);
			}
			else
				cmpWin.AddText(txt);
        }

        //cmpWin.AddText("奖励内容:");

        int bonusNum = CUIDataPool.Instance.m_vecQuestRewardItem.Count;
        for (int i = 0; i < bonusNum; i++)
        {
            QuestRewardItem rewardItem = CUIDataPool.Instance.m_vecQuestRewardItem[i];
            CommonAddBonus(rewardItem);
        }
		
		if(rewardtext != "")
		{
			rewardtext = "RGBA(255,255,0,1)任务奖励： " + rewardtext;
			TaskReward.Text = rewardtext;
		}
        cmpWin.ReLayout();
		
		rewardtext = "";

        acceptBtn.gameObject.SetActiveRecursively(true);
        acceptBtn.Text = parseRes + " (接受任务)";
        continueBtn.gameObject.SetActiveRecursively(false);
        //refuseBtn.gameObject.SetActiveRecursively(true);
        //refuseBtn.Text = "再见";
    }
	
	string rewardtext = "";
    private void CommonAddBonus(QuestRewardItem rewardItem)
    {
        if (rewardItem == null)
        {
            LogManager.LogError("GetMissionContinue_Bonus Empty Event Item");
            return;
        }
        switch (rewardItem.pItemData.m_nType)
        {
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_INVALID:
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY:// 金钱
                //cmpWin.AddMoney(rewardItem.pItemData.m_uMoney);
			    TaskReward.gameObject.SetActiveRecursively(true);
		        rewardtext += " RGBA(255,255,0,1)金钱： " + "RGBA(255,255,255,255)" + rewardItem.pItemData.m_uMoney;
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM:// 物品
                //cmpWin.AddText("物品奖励:");
			    rewardTip.gameObject.SetActiveRecursively(true);
                cmpWin.AddItem((uint)rewardItem.pItemImpl.GetID(), rewardItem.pItemData.m_ItemBonus.m_yCount);
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND:// 随机物品
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO:// 多选1物品
                cmpWin.AddItem((uint)rewardItem.pItemImpl.GetID(), rewardItem.pItemData.m_ItemBonus.m_yCount);
                break;
            case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP:// 经验
                //cmpWin.AddExp(rewardItem.pItemData.m_uExp);
			    TaskReward.gameObject.SetActiveRecursively(true);
			    rewardtext += " RGBA(255,255,0,1)经验： " + "RGBA(255,255,255,255)" + rewardItem.pItemData.m_uExp;
                break;
            default:
                break;
        }
    }
// 
//     void CommonAddBonus(SMissionBonus pBonus)
//     {
//         if (pBonus == null)
//         {
//             LogManager.LogError("GetMissionContinue_Bonus Empty Event Item");
//             return;
//         }
//         switch (pBonus.m_nType)
//         {
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_INVALID:
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY:// 金钱
//                 cmpWin.AddMoney(pBonus.m_uMoney);
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM:// 物品
//                 cmpWin.AddItem(pBonus.m_ItemBonus.m_uItemID, pBonus.m_ItemBonus.m_yCount);
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND:// 随机物品
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO:// 多选1物品
//                 cmpWin.AddItem(pBonus.m_ItemBonus.m_uItemID, pBonus.m_ItemBonus.m_yCount);
//                 break;
//             case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP:// 经验
//                 cmpWin.AddExp(pBonus.m_uExp);
//                 break;
//             default:
//                 break;
//         }
//     }
    
    const float MAX_OBJ_DISTANCE = 3.0F;
    public void NpcDistanceChanged(GAME_EVENT_ID eventId, List<string> vParam)
    {
        int id = int.Parse(vParam[0]);
        if (id == npcId)
        {

            if (vParam[1] == "destroy")
            {
                CloseWindow();
            }
            else
            {
                float distance = float.Parse(vParam[1]);
                if (distance > MAX_OBJ_DISTANCE)
                {
                    CloseWindow();
                }
            }
        }
    }

    private void MissionRewardUpdate()
    {
        cmpWin.CleanAll();
		
		string parseRes = "";
        int txtNum = CUIDataPool.Instance.m_pMissionContinueInfo.m_yTextCount;
        for (int i = 0; i < txtNum; i++)
        {
			int index;
            string txt = CUIDataPool.Instance.m_pMissionContinueInfo.m_aText[i].m_szString;
			if(UIString.Instance.ParseFinishQuest(txt, out index))
			{
				string mark = "#{M_FINISH}";
				string TargetText = txt.Substring(index + mark.Length, txt.Length - mark.Length);
                UIString.Instance.ParserString_RuntimeNoHL(TargetText,out parseRes);
			}
			else
				cmpWin.AddText(txt);
        }

        //cmpWin.AddText("奖励内容:");

        int bonusNum = CUIDataPool.Instance.m_vecQuestRewardItem.Count;
        for (int i = 0; i < bonusNum; i++)
        {
            QuestRewardItem rewardItem = CUIDataPool.Instance.m_vecQuestRewardItem[i];
            CommonAddBonus(rewardItem);
        }
			
		if(rewardtext != "")
		{
			rewardtext = "RGBA(255,255,0,1)任务奖励: " + rewardtext;
			TaskReward.Text = rewardtext;
		}
        cmpWin.ReLayout();
		
		rewardtext = "";

        acceptBtn.gameObject.SetActiveRecursively(false);

        continueBtn.gameObject.SetActiveRecursively(true);
        continueBtn.Text = parseRes + "(完成任务)";
		
		CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MISSION_TUTORIALTIPHIDE, CUIDataPool.Instance.m_pMissionContinueInfo.m_idMission);
        //refuseBtn.gameObject.SetActiveRecursively(true);
        //refuseBtn.Text = "取消";
    }
	
	UIWindowItem fuben;
	
    /// <summary>
    /// 显示任务列表
    /// </summary>
    public void ShowEventList()
    {
		
		ScriptEventItem IsFuBenItem = CUIDataPool.Instance.m_pEventList.GetItem((byte)0);
		if(!UIString.Instance.ParseIsFuBen(IsFuBenItem.m_strString.m_szString))
		{
			cmpWin.CleanAll();
			CObjectManager.Instance.CareObject(npcId, true, "Quest");
			
			int listCount = CUIDataPool.Instance.m_pEventList.m_yItemCount;
			
			for(int i = 0; i < listCount; i++)
			{
				ScriptEventItem item = CUIDataPool.Instance.m_pEventList.GetItem((byte)i);
				if(item != null)
				{
					switch(item.m_nType)
					{
					    case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_INVALID:
                            break;
                        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SECTION:
                            break;
                        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID:
                            string content = item.m_strString.m_szString;
                            string opName = item.m_idScript + "|" + item.m_index + "|" + i;
                            cmpWin.AddOption(opName, content);
                            break;
                        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT:
                            string content2 = item.m_strString.m_szString;
                            cmpWin.AddText(content2);
                            break;
                       default:
                            break;
					}
				}
			}
			cmpWin.ReLayout();
			
			acceptBtn.gameObject.SetActiveRecursively(false);
            continueBtn.gameObject.SetActiveRecursively(false);
			
			//rewardTip.gameObject.SetActiveRecursively(false);
            //refuseBtn.gameObject.SetActiveRecursively(true);
            //refuseBtn.Text = "再见";		
		}
		else
		{
			gameObject.transform.root.gameObject.SetActiveRecursively(false);
			
			UIWindowMng.Instance.GetWindow("FuBenWindow");
			CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OPEN_FUBEN);
		}
	}
  

    public void EventSelect(string opName)
    {
        string[] ops = opName.Split('|');
        if (ops.Length != 3)
        {
            LogManager.LogWarning("event name's count is wrong.");
            return;
        }
        int script = int.Parse(ops[0]);
        int m_index = int.Parse(ops[1]);
        int localIndex = int.Parse(ops[2]);
        CUIDataPool.Instance.SendSelectEvent(localIndex, m_index, script);
    }

    public void Show()
    {
        gameObject.transform.root.gameObject.SetActiveRecursively(true);
		rewardTip.gameObject.SetActiveRecursively(false);
		TaskReward.gameObject.SetActiveRecursively(false);
    }

    public void Hide()
    {
        gameObject.transform.root.gameObject.SetActiveRecursively(false);
    }
}
