using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using Interface;

public class CDetailAttrib_Player : CDetailAttrib_Base
{
    readonly static CDetailAttrib_Player instance = new CDetailAttrib_Player();
    public static CDetailAttrib_Player Instance
    {
        get { return instance; }
    }

    // 任务?
    public _MISSION_DB_LOAD		    m_listMission = new _MISSION_DB_LOAD();

    //protected uint				    mTraceMissionNum;
    //protected int[]                 mTraceMissionList = new int[GAMEDEFINE.MAX_CHAR_MISSION_NUM];
    protected List<int> mTraceMissionList = new List<int>();

    // �可以被本客户端所能看见怪物掉落的任务物品ID列表
    //protected uint				    m_dwCanPickMissionItemIDNum;
    //protected uint[]			    m_adwCanPickMissionItemList = new uint [GAMEDEFINE.MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM];
    protected List<uint> m_adwCanPickMissionItemList = new List<uint>();

    public override bool Init()
    {
        return true;
    }

    public override void Term()
    {
        //m_dwCanPickMissionItemIDNum = 0;
        //for (int i = 0; i < GAMEDEFINE.MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM; i++)
        //{
        //    m_adwCanPickMissionItemList[i] = MacroDefine.UINT_MAX;
        //}
        m_adwCanPickMissionItemList.Clear();
    }

    public override void Tick()
    {
    }

    public override void AddMission(_OWN_MISSION pMission)
    {
        if (m_listMission.m_Count == (byte)GAMEDEFINE.MAX_CHAR_MISSION_NUM)
        {
            return;
        }
        int dwIndex = GetMissionIndexByID(pMission.m_idMission);
        if (dwIndex != MacroDefine.INVALID_ID)
        {
            return;
        }

        dwIndex = MacroDefine.INVALID_ID;
        for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
        {
            if (m_listMission.m_aMission[i].m_idMission == MacroDefine.INVALID_ID)
            {
                dwIndex = i;
                break;
            }
        }

        if (dwIndex == MacroDefine.INVALID_ID)
        {
            return;
        }

        m_listMission.m_aMission[dwIndex] = pMission;
        m_listMission.m_Count++;

        // 添加到任务追?
        AddTraceMission(pMission.m_idMission);

        // �解析任务
        DataPool.Instance.ParseMission(pMission.m_idMission, pMission.m_idScript);

        // 自动执行任务
        AutoExecuteMission(pMission.m_idMission);

        ////任务增加，播放声音?
        //CSoundSystemFMod::_PlayUISoundFunc(70);

        //if(CGameProcedure::s_pUISystem && CGameProcedure::s_pUISystem->IsWindowShow("QuestLog"))
        //    CEventSystem::GetMe()->PushEvent(GE_UPDATE_MISSION);
    }

    // �自动执行任务 [4/17/2012 Ivan]
    public void AutoExecuteMission(int missId)
    {
        _MISSION_INFO missInfo = MissionStruct.Instance.GetMissionInfo(missId);
        HyperLink link = new HyperLink();
        UIString.Instance.ParseHyperLink(missInfo.acceptLink,ref link);
        // 如果名字是超链接的话，完成任务后自动执行 [4/16/2012 Ivan]
        if (link.allItems.Count != 0)
        {
            link.allItems[0].Click();
            //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MISSION_TUTORIALTIPHIDE);
        }
    }

    public override void ModifyMission(_OWN_MISSION pMission)
    {
        int dwIndex = GetMissionIndexByID(pMission.m_idMission);
        if (dwIndex == MacroDefine.INVALID_ID)
        {
            return;
        }
        m_listMission.m_aMission[dwIndex] = pMission;
        
        //// 如果境界任务处于可完成状态则给客户端发送消?[8/18/2011 edit by ZL]
        //if (DataPool::GetMe()->GetAmbitMissionPojieByScriptID(pMission->m_idScript) != -1 &&
        //    m_listMission.m_aMission[dwIndex].m_anParam[0] == 0 ) {
        //    CEventSystem::GetMe()->PushEvent(GE_AMBITMISSION_FINISHED);
        //    }

        AutoReplyMission(pMission.m_idMission);

	    //�删除只限QuestLog窗口显示时才发送消息的限制，任务跟踪系统也需要这个消?
        if (GameProcedure.s_pUISystem != null)
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_MISSION, pMission.m_idMission);
        }
    }

    /// <summary>
    /// �如果任务完成了，自动回复任务
    /// </summary>
    /// <param name="pMission"></param>
    private void AutoReplyMission(int missId)
    {
        if (!IsMissionCanCommit(missId))
            return;

        // 刷新采集?[5/11/2011 ivan edit]
        CObjectManager.Instance.FlashAllTripperObjs();

        _MISSION_INFO missInfo = MissionStruct.Instance.GetMissionInfo(missId);
        HyperLink link = new HyperLink();
        UIString.Instance.ParseHyperLink(missInfo.m_finishNpcName, ref link);
        // �如果名字是超链接的话，完成任务后自动执行 [4/16/2012 Ivan]
        if (link.allItems.Count != 0)
        {
            // 如果在挂机状态，要先停止 [4/21/2012 Ivan]
            GameInterface.Instance.StopAutoHit();
            AutoReleaseSkill.Instance.SetTargetObject(-1);//停止自动释放技能
            link.allItems[0].Click();

            //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MISSION_TUTORIALTIPHIDE);
        }
    }

    public override void ModifyMissionData(int[] pMissionData)
    {
        for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM; i++)
        {
            m_listMission.m_aMissionData[i] = pMissionData[i];
        }

        //if(GameProcedure.s_pUISystem && GameProcedure.s_pUISystem.IsWindowShow("QuestLog"))
        //{
        //    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_MISSION);
        //}
    }

    public override void RemoveMission(int idMission)
    {
        int dwIndex = GetMissionIndexByID(idMission);
        if (dwIndex != MacroDefine.INVALID_ID && m_listMission.m_aMission[dwIndex].m_idMission != MacroDefine.INVALID_ID)
        {
            MissionStruct.Instance.RemoveMissionInfo(dwIndex);
            m_listMission.m_aMission[dwIndex].Cleanup();
            m_listMission.m_Count--;

            // 从任务追踪里面删除掉 [1/18/2011 ivan edit]
            DelTraceMission(idMission);

            //if (GameProcedure.s_pUISystem && GameProcedure.s_pUISystem.IsWindowShow("QuestLog"))
            //    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_MISSION);
        }
    }

    public override void UpdateCanPickMissionItemList(uint dwItemCount, uint[] paItemList)
    {
        //m_dwCanPickMissionItemIDNum = dwItemCount;
        //m_adwCanPickMissionItemList = paItemList;
        for (uint ui = 0; ui < dwItemCount; ui++)
        {
            m_adwCanPickMissionItemList.Add(paItemList[ui]);
        }
    }

    public override void AddCanPickMissionItem(uint dwItemDataID)
    {
        if (m_adwCanPickMissionItemList.Contains(dwItemDataID) && m_adwCanPickMissionItemList.Count < GAMEDEFINE.MAX_CHAR_CAN_PICK_MISSION_ITEM_NUM)
        {
            m_adwCanPickMissionItemList.Add(dwItemDataID);
        }
    }

    public override void RemoveCanPickMissionItem(uint dwItemDataID)
    {
        //for (uint i = 0; i < m_dwCanPickMissionItemIDNum; i++)
        //{
        //    if (m_adwCanPickMissionItemList[i] == dwItemDataID)
        //    {
        //        m_dwCanPickMissionItemIDNum--;
        //        m_adwCanPickMissionItemList[i] = m_adwCanPickMissionItemList[m_dwCanPickMissionItemIDNum];
        //        m_adwCanPickMissionItemList[m_dwCanPickMissionItemIDNum] = MacroDefine.UINT_MAX;
        //        return;
        //    }
        //}
        foreach (uint item in m_adwCanPickMissionItemList)
        {
            if (item == dwItemDataID)
            {
                m_adwCanPickMissionItemList.Remove(item);
            }
        }
    }

    public _OWN_MISSION GetMissionByID(uint dwMissionID)
    {
        _OWN_MISSION a = new _OWN_MISSION();
        return a;
    }

    // 通过任务ID取该任务在任务表表的索引值， 无该任务时返回UINT_MAX
    public int GetMissionIndexByID( int idMission )
    {
        for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
        {
            if (m_listMission.m_aMission!= null && m_listMission.m_aMission[i].m_idMission == idMission)
            {
                return i;
            }
        }
        return MacroDefine.INVALID_ID;
    }

    // 设置任务参数
    public void SetMissionParam(int dwIndexMission, int dwIndexParam, int nValue)
    {
        if (dwIndexMission < GAMEDEFINE.MAX_MISSION_PARAM_NUM && dwIndexParam < GAMEDEFINE.MAX_MISSION_PARAM_NUM)
        {
            m_listMission.m_aMission[dwIndexMission].GetValue<int>(nValue);
            if (dwIndexParam == GAMEDEFINE.MAX_MISSION_PARAM_NUM - 1)
                CDataPool.Instance.QuestTimeGroup_UpdateList(dwIndexMission, nValue);
        }
    }

    // 取任务参?
    public int GetMissionParam(int dwIndexMission, int dwIndexParam)
    {
        if (dwIndexMission < GAMEDEFINE.MAX_MISSION_PARAM_NUM && dwIndexParam < GAMEDEFINE.MAX_MISSION_PARAM_NUM)
        {
            return m_listMission.m_aMission[dwIndexMission].GetValue<int>(dwIndexParam);
        }
        else
        {
            return 0;
        }
    }

    public _OWN_MISSION GetMission( int dwIndex )
    {
        if (dwIndex < GAMEDEFINE.MAX_CHAR_MISSION_NUM)
            return m_listMission.m_aMission[dwIndex];
        else
            return null;
    }

    // �设置任务自定义数?
    public void SetMissionData(int dwIndexMission, int nValue)
    {
        if (dwIndexMission < GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM)
        {
            m_listMission.m_aMissionData[dwIndexMission] = nValue;
        }
    }

    // �取任务自定义数据
    public int GetMissionData(int dwIndexMission)
    {
        if (dwIndexMission < GAMEDEFINE.MAX_CHAR_MISSION_DATA_NUM)
            return m_listMission.m_aMissionData[dwIndexMission];
        else
            return 0;
    }

    //任务相关----------------------------------------------
    //player的当前任务数?
    public int GetMission_Num()
    {
        return m_listMission.m_Count;
    }

    // �保存任务完成标志 [6/14/2011 ivan edit]
    public void SetMissionHaveDone(uint[] pHaveDone)
    {
        m_listMission.m_aMissionHaveDoneFlags = pHaveDone;
        // 初始化可接任务列表，这个只会设置一?[4/6/2012 Ivan]
        MissionList.Instance.Initial();

        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UI_INFOS, "MissionDoneTag");
    }

    // �设置单个任务完成 [6/15/2011 ivan edit]
    public void SetMissionComplete(int nID, int bComplete)
    {
        if (bComplete > 0)
        {
            m_listMission.m_aMissionHaveDoneFlags[nID >> 5] |= (uint)(0x00000001 << (nID & 0x0000001F));
        }
        else
        {
            m_listMission.m_aMissionHaveDoneFlags[nID >> 5] &= ~((uint)(0x00000001 << (nID & 0x0000001F)));
        }
    }

    /*
    *	是否已经做过了某个任?[6/14/2011 ivan edit]
    *	�任务号转换成二进制，?50->111000010
    *	�前面?110�代表任务类型，就?4�号类型，?�?27�个类?
    *	�后面?0010�代表位移，是否完成这个任务就靠这个标?
    *	�所以说，一共可以标?28*32�?096�个任?
    */
    public bool IsMissionHaveDone(uint idMission)
    {
        int idIndex = ((int)idMission >> 5);

        if (idIndex < GAMEDEFINE.MAX_CHAR_MISSION_FLAG_LEN)
        {
            if ((m_listMission.m_aMissionHaveDoneFlags[idIndex] & (0x00000001 << (int)(idMission & 0x0000001F))) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

//    // �判断该任务是否已经是完成可交付状?[6/16/2011 ivan edit]
    public bool IsMissionCanCommit(int nID)
    {
        int nIndex;
        int nMissionParaIndex; // �任务已完成的标志位，需要脚本在任务完成时设?

        nMissionParaIndex = 0;

        nIndex = GetMissionIndexByID(nID);

        //�任务完成
        if ((nIndex != MacroDefine.INVALID_ID) && (GetMissionParam(nIndex, nMissionParaIndex) == 0))
            return true;
        else
            return false;
    }

    public int GetTraceMissionNum()
    {
        return mTraceMissionList.Count;
    }

    public int GetTraceMissionId(int index)
    {
        return mTraceMissionList[index];
    }

    public void AddTraceMission(int misId)
    {
        //if (mTraceMissionList.Count < (uint)GAMEDEFINE.MAX_CHAR_MISSION_NUM)
        //{
        //    for (uint i= 0; i < mTraceMissionNum; i++ )
        //{
        //    if ( mTraceMissionList[i] == misId )
        //    {
        //        return ;
        //    }
        //}
        //mTraceMissionList[mTraceMissionNum++] = misId;
        if (!mTraceMissionList.Contains(misId) && mTraceMissionList.Count < GAMEDEFINE.MAX_CHAR_MISSION_NUM)
        {
            mTraceMissionList.Add(misId);

            // 追踪任务，参数任务Id [8/12/2011 Sun]
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TRACE_MISSION, misId);
        }
    }

    public void DelTraceMission(int misId)
    {
        //for (uint i = 0; i < mTraceMissionNum; i++)
        //{
        //    if (mTraceMissionList[i] == misId)
        //    {
        //        mTraceMissionNum--;
        //        mTraceMissionList[i] = mTraceMissionList[mTraceMissionNum];
        //        mTraceMissionList[mTraceMissionNum] = -1;
        //        return;
        //    }
        //}
        if (mTraceMissionList.Contains(misId))
        {
            mTraceMissionList.Remove(misId);
        }
    }

    public void ClearTraceMission()
    {
        //mTraceMissionNum = 0;
        //for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
        //{
        //    mTraceMissionList[i] = -1;
        //}
        mTraceMissionList.Clear();
    }

    protected void OnMissionChanged(uint dwIndex){}
}