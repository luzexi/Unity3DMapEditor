using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBC;
using DBSystem;

public enum NpcMissState
{
    // 注意，后面的数字对应effect.txt表里面的特效编号 [4/6/2012 Ivan]
    None=-1,
    AcceptNormal=8,
    FinishNormal = 10,
    ContinueNormal = 11,
}

public class MissionList
{
    static readonly MissionList instance = new MissionList();
    public static MissionList Instance { get { return instance; } }

    List<_DBC_MISSION_DEMAND_LIST> allMissions = new List<_DBC_MISSION_DEMAND_LIST>();

    /// <summary>
    /// 删除已经做过的任务
    /// </summary>
    public void Initial()
    {
        allMissions.Clear();

        COMMON_DBC<_DBC_MISSION_DEMAND_LIST> orgMissions = CDataBaseSystem.Instance.GetDataBase<_DBC_MISSION_DEMAND_LIST>
                                                                ((int)DataBaseStruct.DBC_MISSION_DEMAND_LIST);
        foreach (_DBC_MISSION_DEMAND_LIST miss in orgMissions.StructDict.Values)
        {
            // 现在没有重复性任务 [4/6/2012 Ivan]
            if (!CDetailAttrib_Player.Instance.IsMissionHaveDone((uint)miss.n_MissionID))
            {
                allMissions.Add(miss);
            }
        }

        Sort();
    }

    // 过滤后的可交接任务 [4/6/2012 Ivan]
    List<_DBC_MISSION_DEMAND_LIST> sortMissions = new List<_DBC_MISSION_DEMAND_LIST>();
    public List<_DBC_MISSION_DEMAND_LIST> SortMissions
    {
        get { return sortMissions; }
    }

    // 当前场景的任务 [4/6/2012 Ivan]
    List<_DBC_MISSION_DEMAND_LIST> sceneMissions = new List<_DBC_MISSION_DEMAND_LIST>();
    public List<_DBC_MISSION_DEMAND_LIST> SceneMissions
    {
        get { return sceneMissions; }
    }

    /// <summary>
    /// 过滤任务
    /// </summary>
    public void Sort()
    {
        oldNpcIds.Clear();
        foreach (_DBC_MISSION_DEMAND_LIST miss in sceneMissions)
        {
            if (!oldNpcIds.Contains(miss.n_AcceptNpcID))
                oldNpcIds.Add(miss.n_AcceptNpcID);
            if (!oldNpcIds.Contains(miss.n_FinishNpcID))
                oldNpcIds.Add(miss.n_FinishNpcID);
        }

        sortMissions.Clear();
        sceneMissions.Clear();

        CObject_PlayerMySelf mySelf = CObjectManager.Instance.getPlayerMySelf();
        CDetailAttrib_Player myData = CDetailAttrib_Player.Instance;
        // 没有玩家数据无法过滤 [6/14/2011 ivan edit]
        if (mySelf == null || myData == null)
            return;

        int currentScene = GameProcedure.s_pWorldManager.GetActiveSceneID();

        foreach (_DBC_MISSION_DEMAND_LIST miss in allMissions)
        {
            // 是否已经接了该任务
//             if (myData.GetMissionIndexByID(miss.n_MissionID) != MacroDefine.INVALID_ID)
//                 continue;
            // 是否已经完成
            if (myData.IsMissionHaveDone((uint)miss.n_MissionID))
                continue;
            // 最低等级
            if (miss.n_MinLevel != MacroDefine.INVALID_ID && !(mySelf.GetCharacterData().Get_Level() >= miss.n_MinLevel))
                continue;
            // 最高等级
            if (miss.n_MaxLevel != MacroDefine.INVALID_ID && !(mySelf.GetCharacterData().Get_Level() < miss.n_MaxLevel))
                continue;
            // 前置任务
            if (miss.n_PreMissionID != MacroDefine.INVALID_ID && !myData.IsMissionHaveDone((uint)miss.n_PreMissionID))
                continue;

            sortMissions.Add(miss);

            // 整理出当前场景相关的任务 [4/6/2012 Ivan]
            if (miss.n_AcceptSceneId == currentScene || miss.n_FinishSceneId == currentScene)
            {
                sceneMissions.Add(miss);
            }
        }

        UpdateNpcState();

        oldNpcIds.Clear();

        // 刷新地图和可接任务列表 [4/6/2012 Ivan]
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UI_INFOS, "SortMission");
    }


    List<int> oldNpcIds = new List<int>();
    // 更新npc的任务状态 [4/6/2012 Ivan]
    public void UpdateNpcState()
    {
        List<int> npcIds = new List<int>();
        foreach (_DBC_MISSION_DEMAND_LIST miss in sceneMissions)
        {
            if (!npcIds.Contains(miss.n_AcceptNpcID))
                npcIds.Add(miss.n_AcceptNpcID);
            if (!npcIds.Contains(miss.n_FinishNpcID))
                npcIds.Add(miss.n_FinishNpcID);
        }

        foreach (int id in oldNpcIds)
        {
            if (!npcIds.Contains(id))
                npcIds.Add(id);
        }

        foreach (int id in npcIds)
        {
            UpdateNpcState(id);
        }
    }

    void UpdateNpcState( int npcID )
    {
        CObject_PlayerNPC pObj = (CObject_PlayerNPC)CObjectManager.Instance.FindCharacterByTableID(npcID);
	    if(pObj != null)
		    pObj.UpdateMissionState();
    }

    public NpcMissState GetNpcMissState(int npcId)
    {
        _DBC_MISSION_DEMAND_LIST currMiss;
        NpcMissState state = GetNpcMissState(npcId,out currMiss);

        return state;
    }

    public NpcMissState GetNpcMissState(int npcId, out _DBC_MISSION_DEMAND_LIST currMiss)
    {
        currMiss = null;

        if (npcId == MacroDefine.INVALID_ID)
            return NpcMissState.None;

        CDetailAttrib_Player myData = CDetailAttrib_Player.Instance;

        NpcMissState state = NpcMissState.None;
        foreach (_DBC_MISSION_DEMAND_LIST miss in sceneMissions)
        {
            bool alreadyHave = myData.GetMissionIndexByID(miss.n_MissionID) != MacroDefine.INVALID_ID;

            if (miss.n_AcceptNpcID == npcId &&
                state == NpcMissState.None && !alreadyHave)
            {
                state = NpcMissState.AcceptNormal;
                currMiss = miss;
            }
            else if (miss.n_FinishNpcID == npcId)
            {
                if (myData.IsMissionCanCommit(miss.n_MissionID))
                {
                    state = NpcMissState.FinishNormal;
                    currMiss = miss;
                    break;
                }
                else if (alreadyHave)
                {
                    state = NpcMissState.ContinueNormal;
                    currMiss = miss;
                }
            }
        }

        return state;
    }
}