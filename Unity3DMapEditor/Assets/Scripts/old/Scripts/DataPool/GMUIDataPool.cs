using System.Collections;
using System.Collections.Generic;
using System;
using DBSystem;
using Network.Packets;
using Interface;


////------------------------------------------------------
////任务奖励物品
//public class QuestRewardItem
//{
//    public SMissionBonus pItemData;
//    public CObject_Item pItemImpl;
//    public int bSelected;
//};

////------------------------------------------------------
////任务需要物品
//public class QuestDemandItem
//{
//    public SMissionBonusItem pDemandItem;
//    public CObject_Item pItemImpl;
//};

public class CUIDataPool
{
    static readonly CUIDataPool instance = new CUIDataPool();
    public static CUIDataPool Instance
    {
        get
        {
            return instance;
        }
    }

    public void Initial()
    {
        m_bOutGhostTimerWorking = false;
        m_nOutGhostTimer = 0;
        m_nCurDialogNpcId = -1;
        m_nCurShopNpcId = -1;
    }

    public void Tick()
    {
        CDetailAttrib_Player.Instance.Tick();

        if (m_bOutGhostTimerWorking)
        {
            int nTempOutGhostTimer = m_nOutGhostTimer - (int)(GameProcedure.s_pTimeSystem.GetDeltaTime());
            if (nTempOutGhostTimer < 0)
                nTempOutGhostTimer = 0;

            if (m_nOutGhostTimer != nTempOutGhostTimer)
            {
                // 精确到秒的级别
                int nSOld = m_nOutGhostTimer / 1000;
                int nSNew = nTempOutGhostTimer / 1000;
                m_nOutGhostTimer = nTempOutGhostTimer;
            }
        }
        return;
    }

    public RC_RESULT OnCommand_(SCommand_DPC pCmd)
    {
        RC_RESULT rcResult = RC_RESULT.RC_SKIP;
        bool bLogValid = (GameProcedure.s_pUISystem != null) ? true : false;
        switch (pCmd.m_wID)
        {
            case DPC_SCRIPT_DEFINE.DPC_SCRIPT_COMMAND:
                {
                    ENUM_SCRIPT_COMMAND nCmdID = pCmd.GetValue<ENUM_SCRIPT_COMMAND>(0);
                    object pBuf = pCmd.GetValue<object>(1);
                    switch (nCmdID)
                    {
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_EVENT_LIST_RESPONSE: // 事件列表返回
                            m_pEventList = (ScriptParam_EventList)pBuf;
                            OnEventListResponse();
                            break;
                        //打开任务信息
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_RESPONSE: // 任务事件的查询返回
                            m_pMissionInfo = (ScriptParam_MissionInfo)pBuf;
                            OnMissionInfoResponse();
                            break;
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_REGIE: // 漕运任务查询返回
                            m_pEventList = (ScriptParam_EventList)pBuf;
                            OnMissionRegie();
                            break;
                        //任务需求信息（完成任务的文字描述，完成需要的物品，奖励的物品）
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_DEMAND_RESPONSE: // 任务需求的查询返回
                            m_pMissionDemandInfo = (ScriptParam_MissionDemandInfo)pBuf;
                            OnMissionDemandInfoResponse();
                            break;
                        //任务继续信息（包含了奖励信息，在点击continue之后再去显示）
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_CONTINUE_RESPONSE: // 任务的继续按钮事件返回
                            m_pMissionContinueInfo = (ScriptParam_MissionContinueInfo)pBuf;
                            OnMissionContinueInfoResponse();
                            break;
                        //任务完成情况提示信息
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_TIPS: // 任务提示
                            m_pMissionTips = (ScriptParam_MissionTips)pBuf;
                            OnMissionTips();
                            break;
                        //技能学习信息
                        case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_SKILL_STUDY: // 技能信息
                            m_pSkillStudy.Reset();
                            m_pSkillStudy = (ScriptParam_SkillStudy)pBuf;
                            OnSkillStudy();
                            break;
                        default:
                            break;
                    }
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_LIST://名字似乎不再合适了
                {
                    uint dwObjID = pCmd.GetValue<uint>(0);
                    uint dwModifyFlags = pCmd.GetValue<uint>(1);
                    _OWN_MISSION[] paMissionBuf = pCmd.GetValue<_OWN_MISSION[]>(2);

                    CDetailAttrib_Player.Instance.ClearTraceMission();
                    for (int i = 0; i < GAMEDEFINE.MAX_CHAR_MISSION_NUM; i++)
                    {
                        if ((dwModifyFlags & (0x00000001 << i)) != 0)
                        {
                            CDetailAttrib_Player.Instance.m_listMission.m_Count += 1;
                            CDetailAttrib_Player.Instance.m_listMission.m_aMission[i] = paMissionBuf[i];

                            // 解析任务
                            DataPool.Instance.ParseMission(paMissionBuf[i].m_idMission, paMissionBuf[i].m_idScript);

                            CGAskMissionDesc msg = new CGAskMissionDesc();
                            msg.MissionIndex = paMissionBuf[i].m_idMission;
                            NetManager.GetNetManager().SendPacket(msg);

                            // 将有标志的任务添加到追踪列表
                            if (paMissionBuf[i].IsFlags_Trace())
                            {
                                CDetailAttrib_Player.Instance.AddTraceMission(paMissionBuf[i].m_idMission);
                            }
                        }
                    }
                    // 刷新面板 [1/19/2011 ivan edit]
                    ToggleMissionTrace();

                    // 刷新采集物 [5/11/2011 ivan edit]
                    CObjectManager.Instance.FlashAllTripperObjs();

                    //// 刷新可接任务列表 [6/17/2011 ivan edit]
                    //QuestListDataMgr.GetMe().ReFilter();

                    //// 必须先刷新可接任务列表 [6/20/2011 ivan edit]
                    //for (INT i =0; i < playData.m_listMission.m_Count; i++)
                    //{
                    //    pMission = &(playData.m_listMission.m_aMission[i]);
                    //    if(pMission)
                    //    {
                    //        // 刷新有任务的npc状态 [6/17/2011 ivan edit]
                    //        QuestListDataMgr.GetMe().UpdateNpcByMissId(pMission.m_idMission);
                    //    }
                    //}
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_ADD:
                {
                    _OWN_MISSION pMission = pCmd.GetValue<_OWN_MISSION>(0);
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ADD_MISSION, pMission.m_idMission);
                    CDetailAttrib_Player.Instance.AddMission(pMission);

                    CGAskMissionDesc msg = new CGAskMissionDesc();
                    msg.MissionIndex = pMission.m_idMission;
                    NetManager.GetNetManager().SendPacket(msg);

                    //// 将有标志的任务添加到追踪列表 [1/19/2011 ivan edit]
                    //// 默认追踪所有新任务
                    ////if (pMission.IsFlags_Trace())
                    //{
                    //    // 绑定追踪操作转移到AddMission函数中 [2/28/2011 ivan edit]
                    //    //m_pPlayerData.AddTraceMission(pMission.m_idMission);
                    //    // 刷新面板 [1/19/2011 ivan edit]
                    //    ToggleMissionTrace();
                    //}

                    //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ADD_MISSION, pMission.m_idMission);

                    //刷新面板 [1/19/2011 ivan edit]
                    ToggleMissionTrace();

                    // 刷新采集物 [5/11/2011 ivan edit]
                    CObjectManager.Instance.FlashAllTripperObjs();

                    MissionList.Instance.Sort();

                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_MODIFY:
                {
                    int missId = MacroDefine.INVALID_ID;
                    if (pCmd.GetValue<int>(1) == (int)GCMissionModify.MISSIONMODIFY.MISSIONMODIFY_MISSION)
                    {
                        _OWN_MISSION pMission = pCmd.GetValue<_OWN_MISSION>(0);
                        missId = pMission.m_idMission;

                        CDetailAttrib_Player.Instance.ModifyMission(pMission);

                        CGAskMissionDesc msg = new CGAskMissionDesc();
                        msg.MissionIndex = pMission.m_idMission;
                        NetManager.GetNetManager().SendPacket(msg);
                    }
                    else if (pCmd.GetValue<int>(1) == (int)GCMissionModify.MISSIONMODIFY.MISSIONMODIFY_MISSIONDATA)
                    {
                        CDetailAttrib_Player.Instance.ModifyMissionData(pCmd.GetValue<int[]>(0));
                    }
                    MissionList.Instance.UpdateNpcState();

                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_REMOVE:
                {
                    int idMission = pCmd.GetValue<int>(0);
                    CDetailAttrib_Player.Instance.RemoveMission(idMission);

                    //// 删除追踪操作转移到RemoveMission函数中 [2/28/2011 ivan edit]
                    ////m_pPlayerData.DelTraceMission(idMission);

                    // 刷新面板 [1/19/2011 ivan edit]
                    ToggleMissionTrace();

                    // 刷新采集物 [5/11/2011 ivan edit]
                    CObjectManager.Instance.FlashAllTripperObjs();
                    // 

                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_HAVEDOWN_FALG: // 修改任务是否完成标志位 [6/15/2011 ivan edit]
                {
                    int nMissionID;
                    int bHaveDone;
                    nMissionID = pCmd.GetValue<int>(0);
                    bHaveDone = pCmd.GetValue<int>(1);
                    if (CDetailAttrib_Player.Instance != null)
                    {
                        CDetailAttrib_Player.Instance.SetMissionComplete(nMissionID, bHaveDone);

                        // 广播任务完成 [6/16/2011 ivan edit]
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MISSION_FINISH, nMissionID);

                        // 刷新有任务的npc状态 [6/17/2011 ivan edit]
                        MissionList.Instance.Sort();
                    }
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_CAN_PICK_MISSION_ITEM_LIST:
                {
                    uint dwItemCount = pCmd.GetValue<uint>(0);
                    uint[] paItemList = pCmd.GetValue<uint[]>(1);
                    CDetailAttrib_Base pAttribData = CDetailAttrib_Player.Instance;
                    pAttribData.UpdateCanPickMissionItemList(dwItemCount, paItemList);

                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_ADD_CAN_PICK_MISSION_ITEM:
                {
                    uint dwItemDataID = pCmd.GetValue<uint>(0);
                    CDetailAttrib_Base pAttribData = CDetailAttrib_Player.Instance;
                    pAttribData.AddCanPickMissionItem(dwItemDataID);
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            case DPC_SCRIPT_DEFINE.DPC_REMOVE_CAN_PICK_MISSION_ITEM:
                {
                    uint dwItemDataID = pCmd.GetValue<uint>(0);
                    CDetailAttrib_Base pAttribData = CDetailAttrib_Player.Instance;
                    pAttribData.RemoveCanPickMissionItem(dwItemDataID);
                    rcResult = RC_RESULT.RC_OK;
                }
                break;
            /*
         case DPC_UPDATE_COOL_DOWN_LIST:
              {
                 UINT dwUpdateNum	= pCmd.m_adwParam[0];
                 const UINT *pdwSkillCoolDownList	= (const UINT*)(pCmd.m_apConstParam[1]);
                 CDetailAttrib_Base	*pAttribData = m_pPlayerData;
                 pAttribData.UpdateCoolDownList( pdwSkillCoolDownList, dwUpdateNum );
                 rcResult = RC_OK;
               }
               break;
            */
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_TEAM_OR_GROUP:
                {
                    ExcuteTeamCmd(pCmd);
                }
                break;
            // /*
            // 	case DPC_UI_COMMAND:
            // 		{
            // 			INT nUIIndex	= pCmd.m_anParam[0];
            // 			VOID *pBuf	= pCmd.m_apParam[1];
            // 			*pX_PARAM = *((X_PARAM*)(pBuf)); 
            // 
            // 			CEventSystem.GetMe().PushEvent(GE_UI_COMMAND, nUIIndex);
            // 		}
            // 		break;
            // */
            case DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_DESC:
                {
                    GCRetMissionDesc pPacket = pCmd.GetValue<GCRetMissionDesc>(0);
                    _OWN_MISSION OwnMission = CDetailAttrib_Player.Instance.GetMissionByID((uint)pPacket.GetMissionIndex());
                    if (OwnMission != null)
                    {
                        //    //_MISSION_INFO* misInfo = &(GetMissionInfo(OwnMission.m_idScript));
                        _MISSION_INFO misInfo = MissionStruct.Instance.GetMissionInfo(OwnMission.m_idMission);
                        if (pPacket.GetMissionNameLen() != null)
                        {
                            misInfo.m_misName = pPacket.GetMissionName() + pPacket.GetMissionNameLen();
                            ConvertServerString(ref misInfo.m_misName);
                        }
                        if (pPacket.GetMissionDescLen() != null)
                        {
                            misInfo.m_misDesc = pPacket.GetMissionDesc() + pPacket.GetMissionDescLen();
                            ConvertServerString(ref misInfo.m_misDesc);
                        }
                        if (pPacket.GetMissionTargetLen() != null)
                        {
                            //misInfo.tar
                        }
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_MISSION);
                    }
                }
                break;
            default:
                rcResult = RC_RESULT.RC_SKIP;
                break;
        }
        return rcResult;
    }

    private void ToggleMissionTrace()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_MISSION_TRACE);
    }

    private void OnEventListResponse()
    {
        CObject pNPC = CObjectManager.Instance.FindServerObject((int)m_pEventList.m_idNPC);

        if (pNPC != null)
        {
            SetCurDialogNpcId(pNPC.ServerID);
        }
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_EVENTLIST, (pNPC != null) ? pNPC.ID : -1);
    }

    private void OnMissionInfoResponse()
    {
        //清空原有数据
        QuestReward_Clear();

        //生成奖励物品信息
        int nItemCount = m_pMissionInfo.m_yBonusCount;
        for (int i = 0; i < nItemCount; i++)
        {
            SMissionBonus pRewardItem = m_pMissionInfo.m_aBonus[i];

            if (pRewardItem.m_nType == ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM ||
                pRewardItem.m_nType == ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO)
            {
                CObject_Item pItem = ObjectSystem.Instance.NewItem(pRewardItem.m_ItemBonus.m_uItemID);
                if (pItem == null)
                {
                    throw new System.ArgumentNullException();
                }
                pItem.SetNumber(pRewardItem.m_ItemBonus.m_yCount);
                // 创建物品action [2/15/2012 Ivan]
                CActionSystem.Instance.UpdateAction_FromItem(pItem);

                //增加到数据池中
                QuestRewardItem newRewardItem = new QuestRewardItem();
                newRewardItem.pItemData = pRewardItem;
                newRewardItem.pItemImpl = pItem;
                newRewardItem.bSelected = 0;
                m_vecQuestRewardItem.Add(newRewardItem);
            }
            else
            {
                //增加到数据池中
                QuestRewardItem newRewardItem = new QuestRewardItem();
                newRewardItem.pItemData = pRewardItem;
                newRewardItem.pItemImpl = null;
                newRewardItem.bSelected = 0;
                m_vecQuestRewardItem.Add(newRewardItem);
            }
        }
        for (int i = 0; i < m_pMissionInfo.m_yTextCount; i++)
        {
            ConvertServerString(ref m_pMissionInfo.m_aText[i].m_szString);
        }

        //通知UI
        CObject pNPC = CObjectManager.Instance.FindServerObject((int)m_pMissionInfo.m_idNPC);
        if (pNPC != null)
        {
            SetCurDialogNpcId(pNPC.ServerID);
        }

        List<string> LSTemp = new List<string>();
        LSTemp.Add(Convert.ToString((pNPC != null) ? pNPC.ID : -1));
        LSTemp.Add(Convert.ToString(m_pMissionInfo.m_IsPush));
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_INFO, LSTemp);
    }

    private bool ConvertServerString(ref string strConvert)
    {
        //bool bConvert = false;
        //int nPos = strConvert.IndexOf("@itemid_");
        //while (nPos >= 0)
        //{
        //    string strItemIndex = strConvert.Substring(nPos + 8);
        //    int nItemIndex = Convert.ToInt32(strItemIndex);
        //    string strItemName = CObjectManager.Instance.ItemNameByTBIndex(nItemIndex);
        //    strConvert = strConvert.Substring(0, nPos) + strItemName + strItemIndex.Substring(strItemName.Length);
        //    nPos = strConvert.IndexOf("@itemid_");
        //    bConvert = true;
        //}
        //nPos = strConvert.IndexOf("@monsterid_");
        //while (nPos >= 0)
        //{
        //    string strItemIndex = strConvert.Substring(nPos + 11);
        //    int nItemIndex = Convert.ToInt32(strItemIndex);

        //    _DBC_CREATURE_ATT pMonsterAttr = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU(nItemIndex);

        //    if (pMonsterAttr != null)
        //    {
        //        string strItemName = pMonsterAttr.pName;
        //        strConvert = strConvert.Substring(0, nPos) + strItemName + strItemIndex.Substring(strItemName.Length);
        //        bConvert = true;
        //    }
        //    else
        //    {
        //        break;
        //    }
        //    nPos = strConvert.IndexOf("@monsterid_");
        //}

        //return bConvert;

        //暂不实现，已true代替
        return true;
    }

    private void OnMissionRegie()
    {
        CObject pNPC = CObjectManager.Instance.FindServerObject((int)m_pEventList.m_idNPC);
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_REGIE, (pNPC != null) ? pNPC.ID : -1);
    }

    private void OnMissionDemandInfoResponse()
    {
        //清空原有数据
        QuestDemand_Clear();
        //生成需求物品信息
        int nItemCount = m_pMissionDemandInfo.m_yDemandCount;
        for (int i = 0; i < nItemCount; i++)
        {
            CObject_Item pItem = ObjectSystem.Instance.NewItem(m_pMissionDemandInfo.m_aDemandItem[i].m_uItemID);//.m_itemBonus.m_dwItemID);
            pItem.SetNumber(m_pMissionDemandInfo.m_aDemandItem[i].m_yCount);
            // 创建物品action [2/15/2012 Ivan]
            CActionSystem.Instance.UpdateAction_FromItem(pItem);
            //增加到数据池中
            QuestDemandItem newDemandItem = new QuestDemandItem();
            newDemandItem.pDemandItem = m_pMissionDemandInfo.m_aDemandItem[i];
            newDemandItem.pItemImpl = pItem;
            m_vecQuestDemandItem.Add(newDemandItem);
        }
        for (int i = 0; i < m_pMissionDemandInfo.m_yTextCount; i++)
        {
            ConvertServerString(ref m_pMissionDemandInfo.m_aText[i].m_szString);
        }

        //通知UI
        CObject pNPC = CObjectManager.Instance.FindServerObject((int)m_pMissionDemandInfo.m_idNPC);
        if (m_pMissionDemandInfo.m_bDone == (int)ScriptParam_MissionDemandInfo.ScriptMissionDamandInfo.MISSION_DONE)
        {
            //任务已经完成
            if (pNPC != null)
            {
                SetCurDialogNpcId(pNPC.ServerID);
            }
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_CONTINUE_DONE, (pNPC != null) ? pNPC.ID : -1);
        }
        else if (m_pMissionDemandInfo.m_bDone == (int)ScriptParam_MissionDemandInfo.ScriptMissionDamandInfo.MISSION_NODONE)
        {
            //任务没有完成
            if (pNPC != null)
            {
                SetCurDialogNpcId(pNPC.ServerID);
            }
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_CONTINUE_NOTDONE, (pNPC != null) ? pNPC.ID : -1);
        }
        else if (m_pMissionDemandInfo.m_bDone == (int)ScriptParam_MissionDemandInfo.ScriptMissionDamandInfo.MISSION_CHECK)
        {
            //任务提交界面
            if (pNPC != null)
            {
                SetCurDialogNpcId(pNPC.ServerID);
            }
            //需要二次判定，需要提交物品
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_REPLY_MISSION, (pNPC != null) ? pNPC.ID : -1);//这个事件同时也会打开背包界面
        }
    }

    private void OnMissionContinueInfoResponse()
    {
        //清空原有数据
        QuestReward_Clear();

        //生成需求物品信息
        int nItemCount = m_pMissionContinueInfo.m_yBonusCount;
        for (int i = 0; i < nItemCount; i++)
        {
            CObject_Item pItem = null;
            if (m_pMissionContinueInfo.m_aBonus[i].m_nType == ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM ||
                m_pMissionContinueInfo.m_aBonus[i].m_nType == ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO)
            {
                pItem = ObjectSystem.Instance.NewItem(m_pMissionContinueInfo.m_aBonus[i].m_ItemBonus.m_uItemID);
                pItem.SetNumber(m_pMissionContinueInfo.m_aBonus[i].m_ItemBonus.m_yCount);
                // 创建物品action [2/15/2012 Ivan]
                CActionSystem.Instance.UpdateAction_FromItem(pItem);
            }

            //增加到数据池中
            QuestRewardItem newRewardItem = new QuestRewardItem();
            newRewardItem.pItemData = m_pMissionContinueInfo.m_aBonus[i];
            newRewardItem.pItemImpl = pItem;
            newRewardItem.bSelected = 0;
            m_vecQuestRewardItem.Add(newRewardItem);
        }
        for (int i = 0; i < m_pMissionContinueInfo.m_yTextCount; i++)
        {
            ConvertServerString(ref m_pMissionContinueInfo.m_aText[i].m_szString);
        }

        //点击“继续”之后
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE);
    }

    private void OnMissionTips()
    {
        ConvertServerString(ref m_pMissionTips.m_strText.m_szString);
        if (!IsDialogue(m_pMissionTips.m_strText.m_szString))
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, m_pMissionTips.m_strText.m_szString);
    }

    /// <summary>
    /// 判断是否播放剧情动画的指令
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public bool IsDialogue(string content)
    {
        int id;
        if (UIString.Instance.ParseDialogue(content, out id))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PLAY_DIALOGUE, id);
            return true;
        }
        return false;
    }

    private void OnSkillStudy()
    {
        CObject pNPC = CObjectManager.Instance.FindServerObject((int)m_pEventList.m_idNPC);

        if (pNPC != null)
        {
            SetCurDialogNpcId(pNPC.ServerID);
        }
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_SKILLSTUDY, (pNPC != null) ? pNPC.ID : -1);
    }

    //public CDetailAttrib_Player GetMySelfDetailAttrib()
    //{
    //    return m_pPlayerData;
    //}

    //  当前对话NpcId;
    public int GetCurDialogNpcId()
    {
        return m_nCurDialogNpcId;
    }

    public void SetCurDialogNpcId(int nCurDialogNpcId)
    {
        m_nCurDialogNpcId = nCurDialogNpcId;
    }

    // 当前商人的Id
    public int GetCurShopNpcId()
    {
        return m_nCurShopNpcId;
    }

    public void SetCurShopNpcId(int nCurShopNpcId)
    {
        m_nCurShopNpcId = nCurShopNpcId;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // 复活相关的信息
    //
    // 这里只是提供一个复活的倒计时间功能，因没有相对应的控件支持，所以只能临时用这种方法实现
    //

    // 开始灵魂出窍时间的计时
    public void BeginOutGhostTimer(int nOutGhostTimer)
    {
        m_bOutGhostTimerWorking = true;
        m_nOutGhostTimer = nOutGhostTimer;
    }

    // 停止灵魂出窍时间的计时
    public void EndOutGhostTimer()
    {
        m_bOutGhostTimerWorking = false;
    }

    // 设置SOURCE_Id
    public void SetSourceGUID(uint Id)
    {
        m_IdSource = Id;
    }

    // 得到SOURCE_Id
    public uint GetSourceGUID()
    {
        return m_IdSource;
    }

    CTeamOrGroup m_TeamOrGroup = new CTeamOrGroup();

    // 设置队伍邀请者的Id
    public void SetTeamInvitorGUID(uint Id)
    {
        m_IdSource = Id;
    }
    // 得到队伍信息
    public CTeamOrGroup GetTeamOrGroup() { return m_TeamOrGroup; }

    // 得到队伍邀请者的id
    public uint GetTeamInvitorGUID()
    {
        return m_IdSource;
    }

    // 自动允许加入队伍标记 [6/20/2011 edit by ZL]
    public void SetAutoAddTeam(bool autoFlag)
    {
        m_AutoAddTeam = autoFlag;
    }
    // 自动允许加入队伍标记 [6/20/2011 edit by ZL]
    public bool GetAutoAddTeam()
    {
        return m_AutoAddTeam;
    }

    // 设置队员邀请者和被邀请者guid2006－4－28
    public int SetMemberInviteGUID(uint SId, uint DId)
    {
        int iPos = -1;
        for (int i = 0; i < UIDATAPOLL_DEFINE.MAX_INVITE_COUNT; i++)
        {
            if (m_MemberInviteInfo[i].bHave == false)
            {
                m_MemberInviteInfo[i].bHave = true;
                m_MemberInviteInfo[i].SourceId = SId;
                m_MemberInviteInfo[i].DestinId = DId;
                iPos = i;
                break;
            }
        }

        return iPos;
    }

    //	得到队员邀请者和被邀请者guid， 并删除2006－4－28
    public bool GetMemberInviteGUID(int iPos, ref uint SId, ref uint DId, bool bDel)
    {
        if ((iPos < 0) || (iPos >= UIDATAPOLL_DEFINE.MAX_INVITE_COUNT))
        {
            return false;
        }

        SId = m_MemberInviteInfo[iPos].SourceId;
        DId = m_MemberInviteInfo[iPos].DestinId;

        if (bDel)
        {
            m_MemberInviteInfo[iPos].bHave = false;
        }

        return true;
    }

    // 设置DESTINATION_Id
    public void SetDestinationGUID(uint Id)
    {
        m_IdDestination = Id;
    }

    // 得到DESTINATION_Id
    public uint GetDestinationGUID()
    {
        return m_IdDestination;
    }

    // 设置当前队伍的类型.
    public void SetCurTeamType(int iTeamType)
    {
        m_iCurTeamType = iTeamType;
    }

    // 得到当前的队伍的类型.
    public int GetCurTeamType()
    {
        return m_iCurTeamType;
    }
    RC_RESULT ExcuteTeamCmd(SCommand_DPC pCmd)
    {
        byte res = pCmd.GetValue<byte>(0);
        CTeamOrGroup pTeamOrGroup = GetTeamOrGroup();
        int MyServerID = CObjectManager.Instance.getPlayerMySelf().ServerID;
        string strTemp = "";

        // 如果得到队伍信息出现异常, 保存并且返回.
        if (null == pTeamOrGroup)
        {
            return RC_RESULT.RC_ERROR;
        }

        TeamMemberInfo tmInfo = new TeamMemberInfo();

        switch ((TEAM_RESULT)res)
        {
            case TEAM_RESULT.TEAM_RESULT_TEAMREFRESH:
            case TEAM_RESULT.TEAM_RESULT_MEMBERENTERTEAM:
                {
                    //	MessageBox(NULL, "有新队员加入！", "BEEP(got it))", MB_OK|MB_ICONHAND);
                    tmInfo.m_GUID = pCmd.GetValue<uint>(1);
                    tmInfo.m_OjbID = (int)pCmd.GetValue<uint>(3);
                    tmInfo.m_SceneID = pCmd.GetValue<short>(4);
                    tmInfo.m_nPortrait = pCmd.GetValue<int>(5);
                    tmInfo.m_uDataID = pCmd.GetValue<short>(6);
                    tmInfo.m_szNick = UIString.Instance.GetUnicodeString(pCmd.GetValue<byte[]>(8));
                    // ...

                    pTeamOrGroup.AddMember(tmInfo, pCmd.GetValue<short>(2), 0);

                    //CGameProcedure.s_pEventSystem.PushEvent( GE_INFO_SELF, "有队员加入队伍========");
                    int iUiIndex = pTeamOrGroup.GetMemberUIIndex(tmInfo.m_GUID, 0);
                    if (MacroDefine.INVALID_ID != iUiIndex)
                    {
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MEMBER_ENTER, iUiIndex);
                        if (pTeamOrGroup.GetLeaderGUID() == CObjectManager.Instance.getPlayerMySelf().GetServerGUID())
                        {
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 3);
                        }
                        else
                        {
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 0);
                        }

                        // 刷新左边的队伍窗口界面。
                        // CGameProcedure.s_pEventSystem.PushEvent( GE_TEAM_REFRESH_UI );

                    }

                    if (res == (byte)TEAM_RESULT.TEAM_RESULT_MEMBERENTERTEAM)
                    {
                        if (tmInfo.m_OjbID != MyServerID)
                        { // 别人加入了队伍
                            //                            if(bLogValid)
                            //                            {
                            //                                strTemp = NOCOLORMSGFUNC("TEAM_JOIN_OTHER", tmInfo.m_szNick);
                            //                                ADDNEWDEBUGMSG(strTemp);
                            ////								ADDTALKMSG(strTemp);
                            //                            }


                            // 刷新左边的队伍窗口界面。
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);
                            //CSoundSystemFMod._PlayUISoundFunc(62);
                        }
                        else
                        {
                            if (tmInfo.m_GUID != pTeamOrGroup.GetLeaderGUID())
                            { // 加入了别人的队伍
                                string pLeaderName = pTeamOrGroup.GetMember(pTeamOrGroup.GetLeaderGUID()).m_szNick;

                                //msg
                                //                                if(bLogValid)
                                //                                {
                                //                                    strTemp = NOCOLORMSGFUNC("TEAM_JOIN", pLeaderName);
                                //                                    ADDNEWDEBUGMSG(strTemp);
                                ////									ADDTALKMSG(strTemp);
                                //                                }
                                // 刷新左边的队伍窗口界面。
                                GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);
                                //CSoundSystemFMod._PlayUISoundFunc(62);
                            }
                            else
                            {
                                //if(bLogValid)
                                //{
                                //    strTemp = NOCOLORMSGFUNC("TEAM_CREATE_OK");
                                //    ADDNEWDEBUGMSG(strTemp);
                                //}
                                //CSoundSystemFMod._PlayUISoundFunc(62);

                            }

                            pTeamOrGroup.ClearInviteTeam();
                            pTeamOrGroup.ClearProposer();
                        }

                        if (IsTeamLeader_Me())
                        {
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 3);
                        }
                        else
                        {
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 0);
                        }

                        //  参数4表示, 界面打开就刷新, 界面关闭不刷新.
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);
                        ClearAllInviteTeam();
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MESSAGE, strTemp);

                        // 刷新左边的队伍窗口界面。2006-4-15
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);
                    }
                }
                break;
            case TEAM_RESULT.TEAM_RESULT_MEMBERLEAVETEAM:
                {
                    TeamMemberInfo pTMInfo;
                    pTMInfo = pTeamOrGroup.GetMember(pCmd.GetValue<uint>(1));

                    if (pTMInfo.m_OjbID != MyServerID)
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = NOCOLORMSGFUNC("TEAM_LEAVE_OTHER", pTMInfo.m_szNick);
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }

                        pTeamOrGroup.DelMember(pTMInfo.m_GUID);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);
                        //  参数4表示, 界面打开就刷新, 界面关闭不刷新.
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);
                    }
                    else
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = NOCOLORMSGFUNC("TEAM_LEAVE_MYSELF");
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }

                        pTeamOrGroup.DismissTeam();
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 4);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_CLEAR_UI);
                        // 参数大于2就行.
                        //CGameProcedure.s_pEventSystem.PushEvent( GE_TEAM_OPEN_TEAMINFO_DLG, 4 );
                        //Talk.s_Talk.TeamDestory();
                    }

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MESSAGE, strTemp);
                    //CSoundSystemFMod._PlayUISoundFunc(61);
                }
                break;
            case TEAM_RESULT.TEAM_RESULT_LEADERLEAVETEAM:
                {
                    TeamMemberInfo pTMInfo;
                    pTMInfo = pTeamOrGroup.GetMember(pCmd.GetValue<uint>(1));

                    if (pTMInfo.m_OjbID != MyServerID)
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = NOCOLORMSGFUNC("TEAM_LEAVE_OTHER", pTMInfo.m_szNick);
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }
                        pTeamOrGroup.SetLeaderGUID(pCmd.GetValue<uint>(3));
                        pTeamOrGroup.DelMember(pTMInfo.m_GUID);

                        if (IsTeamLeader_Me())
                        {
                            // 如果自己变成队长不刷新界面.打开队长界面.
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 3);

                            //  参数4表示, 界面打开就刷新, 界面关闭不刷新.
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);

                        }
                        else
                        {
                            // 如果自己变成队长不刷新界面.打开队长界面.
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 0);

                            // 刷新队员信息.
                            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);
                        }
                    }
                    else
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = NOCOLORMSGFUNC("TEAM_LEAVE_MYSELF");
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }

                        pTeamOrGroup.DismissTeam();
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 4);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_CLEAR_UI);
                        //Talk.s_Talk.TeamDestory();

                        // 清空邀请者
                        pTeamOrGroup.ClearInviteTeam();

                        // 清空申请者
                        pTeamOrGroup.ClearProposer();

                    }

                    // 刷新左边的队伍窗口界面。2006-4-15
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MESSAGE, strTemp);
                    //CSoundSystemFMod._PlayUISoundFunc(61);
                }
                break;
            case TEAM_RESULT.TEAM_RESULT_TEAMDISMISS:
                {
                    pTeamOrGroup.DismissTeam();
                    strTemp = /*NOCOLORMSGFUNC*/("TEAM_DISMISS");

                    // 删除界面.
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_CLEAR_UI);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 4);

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MESSAGE, strTemp);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, -1);

                    //msg
                    //                    if(bLogValid)
                    //                    {
                    //                        ADDNEWDEBUGMSG(strTemp);
                    ////						ADDTALKMSG(strTemp);
                    //                    }

                    // 清空邀请者
                    pTeamOrGroup.ClearInviteTeam();

                    // 清空申请者
                    pTeamOrGroup.ClearProposer();

                    //CSoundSystemFMod._PlayUISoundFunc(61);
                    //Talk.s_Talk.TeamDestory();



                }
                break;
            case TEAM_RESULT.TEAM_RESULT_TEAMKICK:
                {
                    TeamMemberInfo pTMInfo;
                    pTMInfo = pTeamOrGroup.GetMember(pCmd.GetValue<uint>(1));

                    if (pTMInfo.m_OjbID != MyServerID)
                    {
                        //msg
                        //                    if(bLogValid)
                        //                    {
                        //                        strTemp = NOCOLORMSGFUNC("TEAM_LEAVE_FORCE_OTHER", pTMInfo.m_szNick);
                        //                        ADDNEWDEBUGMSG(strTemp);
                        ////						ADDTALKMSG(strTemp);
                        //                    }
                        pTeamOrGroup.DelMember(pTMInfo.m_GUID);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);

                        //  参数4表示, 界面打开就刷新, 界面关闭不刷新.
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);
                    }
                    else
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = "你被请离了队伍";//NOCOLORMSGFUNC("team_leave_force");
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }
                        pTeamOrGroup.DismissTeam();
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_CLEAR_UI);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 4);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, -1);
                        //Talk.s_Talk.TeamDestory();

                    }

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MESSAGE, strTemp);
                    //CSoundSystemFMod._PlayUISoundFunc(61);

                }
                break;
            case TEAM_RESULT.TEAM_RESULT_TEAMAPPOINT:
                {
                    TeamMemberInfo pTMInfo;
                    uint NewLeaderGUID;

                    NewLeaderGUID = pCmd.GetValue<uint>(3);

                    pTeamOrGroup.Appoint(NewLeaderGUID);

                    pTMInfo = pTeamOrGroup.GetMember(NewLeaderGUID);

                    if (pTMInfo.m_OjbID != MyServerID)
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = NOCOLORMSGFUNC("TEAM_LEADER_CHANGE_OTHER", pTMInfo.m_szNick);
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 0);
                        // 参数4表示, 界面打开就刷新, 界面关闭不刷新.
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);
                    }
                    else
                    {
                        //msg
                        //                        if(bLogValid)
                        //                        {
                        //                            strTemp = "你被任命为队伍的队长了";//NOCOLORMSGFUNC("team_leader_change_myself");
                        //                            ADDNEWDEBUGMSG(strTemp);
                        ////							ADDTALKMSG(strTemp);
                        //                        }
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_NOTIFY_APPLY, 3);
                        // 参数4表示, 界面打开就刷新, 界面关闭不刷新.
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_OPEN_TEAMINFO_DLG, 4);
                    }

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_UI);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_MESSAGE, strTemp);
                    //CSoundSystemFMod._PlayUISoundFunc(61);
                }
                break;
            case TEAM_RESULT.TEAM_RESULT_STARTCHANGESCENE:
                { // 队友切换场景，改变头像状态
                    TeamMemberInfo pTMInfo;

                    pTMInfo = pTeamOrGroup.GetMember(pCmd.GetValue<uint>(1));

                    if (pTMInfo == null)
                    { // 这种情况可能会在组队跟随的切换场景过程中出现
                        // Assert( FALSE && "Team info is not found when a team member start change scene(ignore this)." );
                        return RC_RESULT.RC_OK;
                    }

                    pTMInfo.m_SceneID = pCmd.GetValue<short>(4);

                    if (MyServerID == pTMInfo.m_OjbID)
                    {
                        break;
                    }

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_UPTEDATA_MEMBER_INFO,
                        pTeamOrGroup.GetMemberUIIndex(pTMInfo.m_GUID, 0));
                    //	CGameProcedure.s_pEventSystem.PushEvent( GE_INFO_SELF, "开始切换场景。" );
                }
                break;
            case TEAM_RESULT.TEAM_RESULT_ENTERSCENE:
                { // 有队友进入新场景，更新场景信息，如果和自己相同场景，则查询他的信息
                    TeamMemberInfo pTMInfo;

                    pTMInfo = pTeamOrGroup.GetMember(pCmd.GetValue<uint>(1));

                    if (pTMInfo == null)
                    { // 这种情况可能会在组队跟随的切换场景过程中出现
                        // Assert( FALSE && "Team info is not found when a team member enter scene(ignore this)." );
                        return RC_RESULT.RC_OK;
                    }

                    if (MyServerID == pTMInfo.m_OjbID)
                    { // 自己进入新场景，不管，因为会自动获取队友信息
                        break;
                    }

                    pTMInfo.m_SceneID = pCmd.GetValue<short>(4);
                    pTMInfo.m_OjbID = (int)pCmd.GetValue<uint>(3);
                    pTMInfo.m_bDeadLink = false; // 进入场景必然非断线状态，免去对队友是否断线重连的判断

                    //if( CGameProcedure.s_pVariableSystem.GetAs_Int("Scene_ID") == pTMInfo.m_SceneID )
                    //{
                    CGAskTeamMemberInfo tmMsg = new CGAskTeamMemberInfo();
                    tmMsg.SceneID = (pTMInfo.m_SceneID);
                    tmMsg.GUID = (pTMInfo.m_GUID);
                    tmMsg.ObjID = (uint)(pTMInfo.m_OjbID);

                    NetManager.GetNetManager().SendPacket(tmMsg);
                    //}

                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_UPTEDATA_MEMBER_INFO,
                        pTeamOrGroup.GetMemberUIIndex(pTMInfo.m_GUID, 0));

                }
                break;
            case TEAM_RESULT.TEAM_RESULT_TEAMALLOCATIONRULER:
                { // 队伍分配模式改变了 [8/24/2011 edit by ZL]
                    byte ruler = pCmd.GetValue<byte>(7);
                    if (ruler < 0 || ruler >= (byte)TEAM_ALLOCATION_RULER.TEAM_ALLOCATION_RULER_NUMBER)
                    {
                        //这个情况出现就不更新了。
                        return RC_RESULT.RC_OK;
                    }
                    pTeamOrGroup.SetTeamAllocation(ruler);
                    // 通知UI组队分配模式改变了 [8/24/2011 edit by ZL]
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_RULER_CHANGE);
                }
                break;
            default:
                {

                }
                break;
        }

        return RC_RESULT.RC_OK;
    }
    public bool IsTeamLeader_Me()
    {
        if (GetTeamOrGroup().HasTeam())
        {
            if (GetTeamOrGroup().GetLeaderGUID() == CObjectManager.Instance.getPlayerMySelf().GetServerGUID()
             )
            {
                return true;
            }
        }

        return false;
    }
    public void ClearAllInviteTeam()
    {
        if (GetTeamOrGroup() != null)
        {
            GetTeamOrGroup().ClearInviteTeam();
        }
    }
    // 离开队伍
    public void LeaveTeam()
    {
	    CGTeamLeave msg = new CGTeamLeave();
	    //BOOL bLogValid = (CGameProcedure::s_pUISystem != NULL)?TRUE:FALSE;
    	
	    msg.GUID = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();
	    NetManager.GetNetManager().SendPacket(msg);
	    //if(bLogValid)	ADDNEWDEBUGMSG(STRING("发送离开队伍消息!"));
    }
    public void DismisstTeam()
    {
        CGTeamDismiss msg = new CGTeamDismiss();
	
	    msg.GUID = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();
	    NetManager.GetNetManager().SendPacket(msg);
    }


    public bool IsHadScriptEvent()
    {
        int listCount = m_pEventList.m_yItemCount;

        for (int i = 0; i < listCount; i++)
        {
            ScriptEventItem item = m_pEventList.GetItem((byte)i);
            if (item != null)
            {
                switch (item.m_nType)
                {
                    case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID:
                        return true;
                    default:
                        break;
                }
            }
        }
        return false;
    }
    //// Player attrib
    //protected CDetailAttrib_Player m_pPlayerData;
    //第一次和npc对话，得到npc所能激活的操作
    public ScriptParam_EventList m_pEventList;
    //在接任务时，看到的任务信息
    public ScriptParam_MissionInfo m_pMissionInfo;
    //接受任务后，再次和npc对话，所得到的任务需求信息
    public ScriptParam_MissionDemandInfo m_pMissionDemandInfo;
    //完成任务后，点击继续，出现的任务信息
    public ScriptParam_MissionContinueInfo m_pMissionContinueInfo;
    //完成任务过程中，阶段性提示信息
    public ScriptParam_MissionTips m_pMissionTips;
    //技能学习
    public ScriptParam_SkillStudy m_pSkillStudy;

    //  当前对话的NpcId
    protected int m_nCurDialogNpcId;
    //  当前商人的Id
    protected int m_nCurShopNpcId;
    //  灵魂出窍的剩余时间计数是否为工作状态
    protected bool m_bOutGhostTimerWorking;
    //  灵魂出窍的剩余时间
    protected int m_nOutGhostTimer;

    //----------------------------------------------------------------------------------------------------------------------------
    // 组队相关的操作
    //
    // 由于队伍信息只与当前玩家有关系, 所以在游戏客户端只存在一份, 应该写在数据池中.

    // 当前显示的队伍的类型.
    public int m_iCurTeamType;

    //---------------------------------------------
    //  邀请, 申请, 踢人使用.

    // 队伍邀请者id
    public uint m_IdSource;

    // 被邀请者id
    public uint m_IdDestination;

    // 自动允许加入队伍标记 [6/20/2011 edit by ZL]
    public bool m_AutoAddTeam;

    // 队员要求其他队员的数据结构2006－4－28
    public MEMBERINVITEINFO[] m_MemberInviteInfo = new MEMBERINVITEINFO[UIDATAPOLL_DEFINE.MAX_INVITE_COUNT];

    public struct MEMBERINVITEINFO
    {
        public uint SourceId;
        public uint DestinId;
        public bool bHave;

        //MEMBERINVITEINFO()
        // {
        //    bHave = FALSE;
        // }
        void Reset()
        {
            bHave = false;
        }
    }

    public struct UIDATAPOLL_DEFINE
    {
        public const int MAX_INVITE_COUNT = 5;
    }

    private void QuestReward_Clear()
    {
        for (int i = 0; i < m_vecQuestRewardItem.Count; i++)
        {
            ObjectSystem.Instance.DestroyItem(m_vecQuestRewardItem[i].pItemImpl);
        }
        m_vecQuestRewardItem.Clear();
    }

    private void QuestDemand_Clear()
    {
        for (int i = 0; i < m_vecQuestDemandItem.Count; i++)
        {
            ObjectSystem.Instance.DestroyItem(m_vecQuestDemandItem[i].pItemImpl);
        }
        m_vecQuestDemandItem.Clear();
    }

    //任务奖励物品
    public List<QuestRewardItem> m_vecQuestRewardItem = new List<QuestRewardItem>();

    //任务需求物品
    public List<QuestDemandItem> m_vecQuestDemandItem = new List<QuestDemandItem>();

    //向服务器发送从列表中选取的任务	
    internal void SendSelectEvent(int localIndex, int m_index, int script)
    {
        ScriptEventItem item = m_pEventList.GetItem((byte)localIndex);
        if (item.m_index == m_index && item.m_idScript == script)
        {
            CGEventRequest msg = new CGEventRequest();
            msg.ExIndex = item.m_index;
            msg.Script = item.m_idScript;
            msg.NPC = m_pEventList.m_idNPC;
            NetManager.GetNetManager().SendPacket(msg);
        }
    }

    internal void SendRefuseEvent()
    {
        CGMissionRefuse msg = new CGMissionRefuse();
        msg.ScriptID = m_pMissionInfo.m_idScript;
        msg.NPCID = m_pMissionInfo.m_idNPC;
        NetManager.GetNetManager().SendPacket(msg);
    }

    internal void SendAcceptEvent()
    {
        CGMissionAccept msg = new CGMissionAccept();
        msg.ScriptID = m_pMissionInfo.m_idScript;
        msg.NPCID = m_pMissionInfo.m_idNPC;
        NetManager.GetNetManager().SendPacket(msg);
    }

    internal void SendContinueEvent()
    {
        CGMissionContinue msg = new CGMissionContinue();
        msg.ScriptID = m_pMissionInfo.m_idScript;
        msg.NPCID = m_pMissionInfo.m_idNPC;
        NetManager.GetNetManager().SendPacket(msg);
    }

    internal void SendCompleteEvent(int rewardItemSel)
    {
        CGMissionSubmit msg = new CGMissionSubmit();
        msg.ScriptID = m_pMissionContinueInfo.m_idScript;
        msg.NPCID = m_pMissionContinueInfo.m_idNPC;
        //需要传入物品ID
        if (rewardItemSel != -1)
            msg.SelectRadio = m_pMissionContinueInfo.GetSelectItemID(rewardItemSel);
        NetManager.GetNetManager().SendPacket(msg);

        //清除跟踪列表里的对应项
        // 	    CDetailAttrib_Player* playData = const_cast<CDetailAttrib_Player*>(CUIDataPool.GetMe().GetMySelfDetailAttrib());
        // 	    playData.DelTraceMission(m_pMissionContinueInfo.m_idMission);
    }
}

