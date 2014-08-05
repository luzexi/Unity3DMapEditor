using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class UIQuestTrace : MonoBehaviour
{
    public TreeControl tree;

    void Awake()
    {
        if (tree == null)
        {
            LogManager.LogError("UIQuestTrace init failed.");
            return;
        }

        tree.Clear();
        // 允许鼠标穿透 [3/12/2012 Ivan]
        if (tree.list != null)
            UISystem.Instance.AddHollowWindow(tree.transform.root.gameObject);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_MISSION_TRACE, UpdateMissionTrace);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_MISSION, UpdateMissionTrace);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, UpdateMissionTrace);
    }

    public void UpdateMissionTrace(GAME_EVENT_ID eventId, List<string> vParam)
    {
        UpdateAllMission();
    }

    void UpdateAllMission()
    {
        tree.Clear();
        if (missionTab == 0)
        {
            UpdateCurrentMission();
        }
        else if (missionTab == 1)
        {
            UpdateEnableMission();
        }
    }

    void UpdateEnableMission()
    {
        foreach (_DBC_MISSION_DEMAND_LIST miss in MissionList.Instance.SortMissions)
        {
            // 只显示没有接过的任务 [4/7/2012 Ivan]
            if ((CDetailAttrib_Player.Instance.GetMissionIndexByID(miss.n_MissionID) == MacroDefine.INVALID_ID))
            {
                _MISSION_INFO info = MissionStruct.Instance.ParseMissionInfo(miss.n_MissionID, miss.n_ScriptID);
                tree.AddItem(info.m_misName, miss.n_MissionID.ToString());
            }
        }
    }

    private void UpdateCurrentMission()
    {
        int missNum = CDataPool.Instance.GetPlayerMission_Num();
        if (missNum <= 0)
            return;

        for (int i = 0; i < GAMEDEFINE.MAX_MISSION_PARAM_NUM; i++)
        {
            int missid = CDataPool.Instance.GetMissionIdByIndex(i);
            if (missid == -1)
                continue;

            _MISSION_INFO miss = CDataPool.Instance.GetPlayerMissionByIndex(i);
            if (!miss.m_bFill)
                continue;

            AddNewMission(miss,missid,i);
        }
    }

    private void AddNewMission(_MISSION_INFO miss,int missId,int missIndex)
    {
        string missName = miss.m_misName;

        int missParamIndex = 1;//第0位用于判断任务是否完成
        int traceDescIndex = 1;
        if (missionTab == 0 )
        {
            // 已接任务，完成的直接显示完成内容
            int isFinish = CDetailAttrib_Player.Instance.GetMissionParam(missIndex, 0);
            if (isFinish == 0)
            {
                string finishContent = "";
                if(miss.m_vecTraceDescList.Count != 0)
                    finishContent = miss.m_vecTraceDescList[0];
                tree.AddItem(finishContent, missId.ToString(), missName);
                return;
            }
        }
        // 任务需求物品
        if (miss.m_vecQuestDemandItem.Count != 0)
        {
            foreach (QuestDemandItem item in miss.m_vecQuestDemandItem)
            {
                int itemNum = 0;
                if (missionTab == 0)// 只有当前任务需要解析任务完成度，可接任务不用
                {
                    itemNum = CDataPool.Instance.UserBag_CountItemByIDTable(item.pItemImpl.GetIdTable());
                }
                string text = "";
                if (miss.m_vecTraceDescList.Count > traceDescIndex)
                    text = miss.m_vecTraceDescList[traceDescIndex];
                text += "(" + itemNum + "/" + item.pDemandItem.m_yCount + ")";
                tree.AddItem(text, missId.ToString(), missName);
                traceDescIndex++;
            }
        }

        // 任务需求杀怪
        if (miss.m_vecQuestDemandKill.Count != 0)
        {
            foreach (QuestDemandKill monster in miss.m_vecQuestDemandKill)
            {
                int paramValue = 0;
                if (missionTab == 0)// 只有当前任务需要解析任务完成度，可接任务不用
                {
                    paramValue = CDetailAttrib_Player.Instance.GetMissionParam(missIndex, missParamIndex++);
                    if (paramValue == 0)
                        paramValue = monster.pDemandKill.m_yCount;
                    else
                        paramValue -= 1;
                }
                string text = "";
                if (miss.m_vecTraceDescList.Count > traceDescIndex)
                    text = miss.m_vecTraceDescList[traceDescIndex];
                text += "(" + paramValue + "/" + monster.pDemandKill.m_yCount + ")";
                tree.AddItem(text, missId.ToString(), missName);
                traceDescIndex++;
            }
        }

        // 自定义物品
        if (miss.m_vecQuestCustom.Count != 0)
        {
            foreach (QuestCustom item in miss.m_vecQuestCustom)
            {
                int paramValue = 0;
                if (missionTab == 0)// 只有当前任务需要解析任务完成度，可接任务不用
                {
                    paramValue = CDetailAttrib_Player.Instance.GetMissionParam(missIndex, missParamIndex++);
                    if (paramValue == 0)
                        paramValue = item.nCount;
                    else
                        paramValue -= 1;
                }
                string text = "";
                if (miss.m_vecTraceDescList.Count > traceDescIndex)
                    text = miss.m_vecTraceDescList[traceDescIndex];
                if (paramValue != item.nCount && item.nCount > 0)
                    text += "(" + paramValue + "/" + item.nCount + ")";
                tree.AddItem(text, missId.ToString(), missName);
                traceDescIndex++;
            }
        }

        //如果都不属于以上类型，则默认添加第一条
        if (traceDescIndex == 1)
        {
			if(miss.m_vecTraceDescList.Count != 0)
			{
	            string text = miss.m_vecTraceDescList[0];
	            tree.AddItem(text, missId.ToString(), missName);
			}
        }
    }

    int missionTab = 0;
    public void ToggleCurrent()
    {
        missionTab = 0;
        UpdateAllMission();
    }

    public void ToggleEnable()
    {
        missionTab = 1;
        UpdateAllMission();
    }
}