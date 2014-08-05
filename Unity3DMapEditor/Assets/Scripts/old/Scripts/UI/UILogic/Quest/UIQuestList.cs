using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQuestList : MonoBehaviour {

    public GameObject cmpGo;
    public GameObject treeGo;

    ComplexListWindow cmpWin;
    TreeControl treeNode;

    void Awake()
    {
        InitClass();
        RegistAllEvents();

        //Hide();
        UpdateCurrList();
    }

    void InitClass()
    {
        cmpWin = cmpGo.GetComponent<ComplexListWindow>();
        if (cmpWin == null)
        {
            LogManager.LogError("Can't found ComplexListWindow in QuestList.");
        }
        cmpWin.maxTextLength = 85;
        treeNode = treeGo.GetComponent<TreeControl>();
        if (treeNode == null)
        {
            LogManager.LogError("Can't found TreeControl in QuestList.");
        }
        treeNode.AddItemInputDelegate(TreeItemClickDelegate);
    }

    public void Show()
    {
        gameObject.transform.root.gameObject.SetActiveRecursively(true);
    }

    public void Hide()
    {
        gameObject.transform.root.gameObject.SetActiveRecursively(false);
    }

    void RegistAllEvents()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_MISSION, UpdateQuestList);
    }


    public void UpdateQuestList(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_TOGLE_MISSION)
        {
            UIWindowMng.Instance.ToggleWindow("QuestListWindow", !gameObject.active);
            UpdateCurrList();
        }
    }

    void UpdateCurrList()
    {
        if (!gameObject.active)
            return;

        treeNode.Clear();
        cmpWin.CleanAll();

        int missNum = CDataPool.Instance.GetPlayerMission_Num();
        if (missNum <= 0)
        {
            treeNode.AddItem("[#D2BE8C]没有任何任务。", "0");
            return;
        }

        int firstMissId = -1;
        for (int i = 0; i < GAMEDEFINE.MAX_MISSION_PARAM_NUM; i++)
        {
            int missid = CDataPool.Instance.GetMissionIdByIndex(i);
            if(missid == -1)
                continue;

            _MISSION_INFO miss = CDataPool.Instance.GetPlayerMissionByIndex(i);
            if (!miss.m_bFill)
                continue;

            if (firstMissId == -1)
                firstMissId = missid;

            string name = UIString.Instance.ParserString_Runtime(miss.m_misName);
            treeNode.AddItem(name, missid.ToString(), KindName(miss.m_nKind));
        }

        SelectMission(firstMissId);
    }

    string KindName(int kind)
    {
        string name;
        switch (kind)
        {
            case 2:
                name = "主线任务";
                break;
            case 101:
                name = "日常任务";
                break;
            default:
                name = "普通任务";
                break;
        }
        return name;
    }

    public void TreeItemClickDelegate(string missIdText)
    {
        int missId = int.Parse(missIdText);
        SelectMission(missId);
    }

    ////////////////////////////////////////////////////////////////////
	// MissionParam_Index使用 :ivan 2011-3-29
	//Index 0 --------------------------------		任务是否完成
    //Index 1 --------------------------------		杀怪
    //Index 2 --------------------------------		需求物品
    //Index 3 --------------------------------		使用物品
    //Index 4 --------------------------------		需求BUFF
    //Index 5 --------------------------------		交互物件
    //Index 6 --------------------------------		区域
    //Index 7 --------------------------------		暂留
	////////////////////////////////////////////////////////////////////
    void SelectMission(int missId)
    {
        _MISSION_INFO miss = CDataPool.Instance.GetPlayerMissionById(missId);
        int missIndex = CDetailAttrib_Player.Instance.GetMissionIndexByID(missId);
        int missParamIndex = 1;//第0位用于判断任务是否完成

        cmpWin.CleanAll();

        string name = UIString.Instance.ParserString_Runtime(miss.m_misName);
        string desc = UIString.Instance.ParserString_Runtime(miss.m_misDesc);
        cmpWin.AddText(name);
        cmpWin.AddText(desc);

        // 任务需求物品
        if (miss.m_vecQuestDemandItem.Count != 0)
        {
            cmpWin.AddSpaceLine();
            cmpWin.AddText("需求物品:");

            foreach (QuestDemandItem item in miss.m_vecQuestDemandItem)
            {
                int itemNum = CDataPool.Instance.UserBag_CountItemByIDTable(item.pItemImpl.GetIdTable());
                string text = item.pItemImpl.GetName() + "(" + itemNum + "/"+ item.pDemandItem.m_yCount + ")";
                cmpWin.AddText(text);
            }
        }

        // 任务需求杀怪
        if (miss.m_vecQuestDemandKill.Count != 0)
        {
            cmpWin.AddSpaceLine();
            cmpWin.AddText("已杀死:");

            foreach (QuestDemandKill monster in miss.m_vecQuestDemandKill)
            {
                int paramValue = CDetailAttrib_Player.Instance.GetMissionParam(missIndex,missParamIndex++);
                if (paramValue == 0)
                    paramValue = monster.pDemandKill.m_yCount;
                else
                    paramValue -= 1;
                string text = monster.szNPCName + "(" + paramValue + "/"+ monster.pDemandKill.m_yCount + ")";
                cmpWin.AddText(text);
            }
        }

        // 自定义物品
        if (miss.m_vecQuestCustom.Count != 0)
        {
            cmpWin.AddSpaceLine();
            cmpWin.AddText("其他物品:");

            foreach (QuestCustom item in miss.m_vecQuestCustom)
            {
                int paramValue = CDetailAttrib_Player.Instance.GetMissionParam(missIndex, missParamIndex++);
                if (paramValue == 0)
                    paramValue = item.nCount;
                else
                    paramValue -= 1;
                string text = item.szCustomString;
                if (paramValue != item.nCount && item.nCount > 0)
                    text += "(" + paramValue + "/" + item.nCount + ")";
                cmpWin.AddText(text);
            }
        }

        // 奖励物品
        bool alreadyShowItem = false;
        if (miss.m_vecQuestRewardItem.Count != 0)
        {
            cmpWin.AddSpaceLine();

            foreach (QuestRewardItem rewardItem in miss.m_vecQuestRewardItem)
            {
                if (rewardItem == null)
                {
                    LogManager.LogError("GetMissionContinue_Bonus Empty Event Item");
                    continue;
                }
                switch (rewardItem.pItemData.m_nType)
                {
                    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_INVALID:
                        break;
                    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY:// 金钱
                        cmpWin.AddMoney(rewardItem.pItemData.m_uMoney);
                        break;
                    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM:// 物品
                        if (!alreadyShowItem)
                        {
                            cmpWin.AddText("奖励物品:");
                            alreadyShowItem = true;
                        }
                        cmpWin.AddItem((uint)rewardItem.pItemImpl.GetID(), rewardItem.pItemData.m_ItemBonus.m_yCount);
                        break;
                    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND:// 随机物品
                        break;
                    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO:// 多选1物品
                        cmpWin.AddItem((uint)rewardItem.pItemImpl.GetID(), rewardItem.pItemData.m_ItemBonus.m_yCount);
                        break;
                    case ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP:// 经验
                        cmpWin.AddExp(rewardItem.pItemData.m_uExp);
                        break;
                    default:
                        break;
                }
            }
        }

        cmpWin.ReLayout();
    }
}
