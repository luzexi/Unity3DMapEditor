using UnityEngine;
using System.Collections;
using DBC;
using DBSystem;
using System.Collections.Generic;

namespace Interface
{
    public class DataPool
    {
        static readonly DataPool instance = new DataPool();
        public static DataPool Instance
        {
            get
            {
                return instance;
            }
        }

        _MISSION_INFO ScriptMissionInfo = new _MISSION_INFO();
        QuestDemandItem newDemandItem = new QuestDemandItem();
        QuestRewardItem newRewardItem = new QuestRewardItem();
        QuestDemandKill newDemandKill = new QuestDemandKill();

        public void ParseMission(int missionId, int scriptId)
        {
            ScriptMissionInfo = MissionStruct.Instance.GetMissionInfo(missionId);

            if (!ScriptMissionInfo.m_bFill)
            {
                string FileName = CScriptSvm.Instance.GetMissionScriptFile(scriptId);
                if (FileName != string.Empty)
                {
                    MissionStruct.Instance.OpenScript(FileName);
                }
                else
                {
                    _DBC_MISSION_LIST MissionList = CDataBaseSystem.Instance.GetDataBase<_DBC_MISSION_LIST>((int)DataBaseStruct.DBC_MISSION_LIST).Search_Index_EQU(scriptId);
                    if (MissionList != null)
                    {
                        _DBC_MISSION_DATA MissionData = CDataBaseSystem.Instance.GetDataBase<_DBC_MISSION_DATA>((int)DataBaseStruct.DBC_MISSION_DATA).Search_Index_EQU(MissionList.nMissionID);
                        if (MissionData != null)
                        {
                            _DBC_MISSION_REWARD MissionReward = CDataBaseSystem.Instance.GetDataBase<_DBC_MISSION_REWARD>((int)DataBaseStruct.DBC_MISSION_REWARD).Search_Index_EQU(MissionList.nReward);
                            if (MissionReward != null)
                            {
                                _DBC_MISSION_DIALOG MissionDialog = CDataBaseSystem.Instance.GetDataBase<_DBC_MISSION_DIALOG>((int)DataBaseStruct.DBC_MISSION_DIALOG).Search_Index_EQU(MissionList.nDialog);
                                if (MissionDialog != null)
                                {
                                    ScriptMissionInfo.m_nLevel = MissionData.nMissionLevel;
                                    ScriptMissionInfo.m_nKind = MissionData.nMissionKind;
                                    ScriptMissionInfo.m_nElite = 0;
                                    ScriptMissionInfo.m_nOKFail = 1;
                                    ScriptMissionInfo.m_nLimitTime = -1;
                                    ScriptMissionInfo.m_nRound = -1;
                                    ScriptMissionInfo.m_nBill = -1;

                                    //固定奖励
                                    if (MissionReward.nItem1ID != -1 && MissionReward.nItem1Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionReward.nItem1ID);
                                        if (pItem != null)
                                        {
                                            SMissionBonus rewardItem = new SMissionBonus();
                                            rewardItem.m_ItemBonus.m_uItemID = (uint)MissionReward.nItem1ID;
                                            rewardItem.m_ItemBonus.m_yCount = MissionReward.nItem1Num;
                                            rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM;

                                            newRewardItem.pItemData = rewardItem;
                                            newRewardItem.bSelected = 0;
                                            pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                            newRewardItem.pItemImpl = pItem;
                                            ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                        }
                                    }
                                    if (MissionReward.nItem2ID != -1 && MissionReward.nItem2Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionReward.nItem2ID);
                                        if (pItem != null)
                                        {
                                            SMissionBonus rewardItem = new SMissionBonus();
                                            rewardItem.m_ItemBonus.m_uItemID = (uint)MissionReward.nItem2ID;
                                            rewardItem.m_ItemBonus.m_yCount = MissionReward.nItem2Num;
                                            rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM;

                                            newRewardItem.pItemData = rewardItem;
                                            newRewardItem.bSelected = 0;
                                            pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                            newRewardItem.pItemImpl = pItem;
                                            ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                        }
                                    }

                                    //选择奖励物品
                                    if (MissionReward.nRandomItem1ID != -1 && MissionReward.nRandomItem1Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionReward.nRandomItem1ID);
                                        if (pItem != null)
                                        {
                                            SMissionBonus rewardItem = new SMissionBonus();
                                            rewardItem.m_ItemBonus.m_uItemID = (uint)MissionReward.nRandomItem1ID;
                                            rewardItem.m_ItemBonus.m_yCount = MissionReward.nRandomItem1Num;
                                            rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO;

                                            newRewardItem.pItemData = rewardItem;
                                            newRewardItem.bSelected = 0;
                                            pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                            newRewardItem.pItemImpl = pItem;
                                            ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                        }
                                    }
                                    if (MissionReward.nRandomItem2ID != -1 && MissionReward.nRandomItem2Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionReward.nRandomItem2ID);
                                        if (pItem != null)
                                        {
                                            SMissionBonus rewardItem = new SMissionBonus();
                                            rewardItem.m_ItemBonus.m_uItemID = (uint)MissionReward.nRandomItem2ID;
                                            rewardItem.m_ItemBonus.m_yCount = MissionReward.nRandomItem2Num;
                                            rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO;

                                            newRewardItem.pItemData = rewardItem;
                                            newRewardItem.bSelected = 0;
                                            pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                            newRewardItem.pItemImpl = pItem;
                                            ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                        }
                                    }
                                    if (MissionReward.nRandomItem3ID != -1 && MissionReward.nRandomItem3Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionReward.nRandomItem3ID);
                                        if (pItem != null)
                                        {
                                            SMissionBonus rewardItem = new SMissionBonus();
                                            rewardItem.m_ItemBonus.m_uItemID = (uint)MissionReward.nRandomItem3ID;
                                            rewardItem.m_ItemBonus.m_yCount = MissionReward.nRandomItem3Num;
                                            rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO;

                                            newRewardItem.pItemData = rewardItem;
                                            newRewardItem.bSelected = 0;
                                            pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                            newRewardItem.pItemImpl = pItem;
                                            ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                        }
                                    }
                                    if (MissionReward.nRandomItem4ID != -1 && MissionReward.nRandomItem4Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionReward.nRandomItem4ID);
                                        if (pItem != null)
                                        {
                                            SMissionBonus rewardItem = new SMissionBonus();
                                            rewardItem.m_ItemBonus.m_uItemID = (uint)MissionReward.nRandomItem4ID;
                                            rewardItem.m_ItemBonus.m_yCount = MissionReward.nRandomItem4Num;
                                            rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO;

                                            newRewardItem.pItemData = rewardItem;
                                            newRewardItem.bSelected = 0;
                                            pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                            newRewardItem.pItemImpl = pItem;
                                            ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                        }
                                    }
                                    if (MissionReward.nMoney > 0)
                                    {
                                        SMissionBonus rewardItem = new SMissionBonus();
                                        rewardItem.m_uMoney = (uint)MissionReward.nMoney;
                                        rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY;

                                        newRewardItem.pItemData = rewardItem;
                                        newRewardItem.bSelected = 0;
                                        ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                    }
                                    if (MissionReward.nExp > 0)
                                    {
                                        SMissionBonus rewardItem = new SMissionBonus();
                                        rewardItem.m_uExp = (uint)MissionReward.nExp;
                                        rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP;

                                        newRewardItem.pItemData = rewardItem;
                                        newRewardItem.bSelected = 0;
                                        ScriptMissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                                    }

                                    //交任务需要的物品
                                    if (MissionData.nFinishItem1ID != -1 && MissionData.nFinishItem1Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionData.nFinishItem1ID);

                                        newDemandItem.pDemandItem = new SMissionBonusItem();
                                        newDemandItem.pDemandItem.m_uItemID = (uint)MissionData.nFinishItem1ID;
                                        newDemandItem.pDemandItem.m_yCount = MissionData.nFinishItem1Num;
                                        pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                        newDemandItem.pItemImpl = pItem;
                                        ScriptMissionInfo.m_vecQuestDemandItem.Add(newDemandItem);
                                    }
                                    if (MissionData.nFinishItem2ID != -1 && MissionData.nFinishItem2Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionData.nFinishItem2ID);

                                        newDemandItem.pDemandItem = new SMissionBonusItem();
                                        newDemandItem.pDemandItem.m_uItemID = (uint)MissionData.nFinishItem2ID;
                                        newDemandItem.pDemandItem.m_yCount = MissionData.nFinishItem2Num;
                                        pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                        newDemandItem.pItemImpl = pItem;
                                        ScriptMissionInfo.m_vecQuestDemandItem.Add(newDemandItem);
                                    }
                                    if (MissionData.nFinishItem3ID != -1 && MissionData.nFinishItem3Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionData.nFinishItem3ID);

                                        newDemandItem.pDemandItem = new SMissionBonusItem();
                                        newDemandItem.pDemandItem.m_uItemID = (uint)MissionData.nFinishItem3ID;
                                        newDemandItem.pDemandItem.m_yCount = MissionData.nFinishItem3Num;
                                        pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                        newDemandItem.pItemImpl = pItem;
                                        ScriptMissionInfo.m_vecQuestDemandItem.Add(newDemandItem);
                                    }
                                    if (MissionData.nFinishItem4ID != -1 && MissionData.nFinishItem4Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionData.nFinishItem4ID);

                                        newDemandItem.pDemandItem = new SMissionBonusItem();
                                        newDemandItem.pDemandItem.m_uItemID = (uint)MissionData.nFinishItem4ID;
                                        newDemandItem.pDemandItem.m_yCount = MissionData.nFinishItem4Num;
                                        pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                        newDemandItem.pItemImpl = pItem;
                                        ScriptMissionInfo.m_vecQuestDemandItem.Add(newDemandItem);
                                    }
                                    if (MissionData.nFinishItem5ID != -1 && MissionData.nFinishItem5Num > 0)
                                    {
                                        CObject_Item pItem = ObjectSystem.Instance.NewItem((uint)MissionData.nFinishItem5ID);

                                        newDemandItem.pDemandItem = new SMissionBonusItem();
                                        newDemandItem.pDemandItem.m_uItemID = (uint)MissionData.nFinishItem5ID;
                                        newDemandItem.pDemandItem.m_yCount = MissionData.nFinishItem5Num;
                                        pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                                        newDemandItem.pItemImpl = pItem;
                                        ScriptMissionInfo.m_vecQuestDemandItem.Add(newDemandItem);
                                    }

                                    //交任务需要杀怪
                                    if (MissionData.nMissionType != (int)GAMEDEFINE.MISSION_TYPE.MISSION_TYPE_SHOUJI)
                                    {
                                        if (MissionData.nMonster1ID != -1 && MissionData.nMonster1Num > 0)
                                        {
                                            _DBC_CREATURE_ATT MissionNpc = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU(MissionData.nMonster1ID);
                                            if (MissionNpc != null)
                                            {
                                                newDemandKill.pDemandKill = new SMissionDemandKill();
                                                newDemandKill.pDemandKill.m_uNPCID = (uint)MissionData.nMonster1ID;
                                                newDemandKill.pDemandKill.m_yCount = (byte)MissionData.nMonster1Num;
                                                newDemandKill.szNPCName = MissionNpc.pName;
                                                ScriptMissionInfo.m_vecQuestDemandKill.Add(newDemandKill);
                                            }
                                        }
                                        if (MissionData.nMonster2ID != -1 && MissionData.nMonster2Num > 0)
                                        {
                                            _DBC_CREATURE_ATT MissionNpc = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU(MissionData.nMonster2ID);
                                            if (MissionNpc != null)
                                            {
                                                newDemandKill.pDemandKill = new SMissionDemandKill();
                                                newDemandKill.pDemandKill.m_uNPCID = (uint)MissionData.nMonster2ID;
                                                newDemandKill.pDemandKill.m_yCount = (byte)MissionData.nMonster2Num;
                                                newDemandKill.szNPCName = MissionNpc.pName;
                                                ScriptMissionInfo.m_vecQuestDemandKill.Add(newDemandKill);
                                            }
                                        }
                                        if (MissionData.nMonster3ID != -1 && MissionData.nMonster3Num > 0)
                                        {
                                            _DBC_CREATURE_ATT MissionNpc = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU(MissionData.nMonster3ID);
                                            if (MissionNpc != null)
                                            {
                                                newDemandKill.pDemandKill = new SMissionDemandKill();
                                                newDemandKill.pDemandKill.m_uNPCID = (uint)MissionData.nMonster3ID;
                                                newDemandKill.pDemandKill.m_yCount = (byte)MissionData.nMonster3Num;
                                                newDemandKill.szNPCName = MissionNpc.pName;
                                                ScriptMissionInfo.m_vecQuestDemandKill.Add(newDemandKill);
                                            }
                                        }
                                        if (MissionData.nMonster4ID != -1 && MissionData.nMonster4Num > 0)
                                        {
                                            _DBC_CREATURE_ATT MissionNpc = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU(MissionData.nMonster4ID);
                                            if (MissionNpc != null)
                                            {
                                                newDemandKill.pDemandKill = new SMissionDemandKill();
                                                newDemandKill.pDemandKill.m_uNPCID = (uint)MissionData.nMonster4ID;
                                                newDemandKill.pDemandKill.m_yCount = (byte)MissionData.nMonster4Num;
                                                newDemandKill.szNPCName = MissionNpc.pName;
                                                ScriptMissionInfo.m_vecQuestDemandKill.Add(newDemandKill);
                                            }
                                        }
                                    }

                                    // 添加任务追踪时的描述
                                    //if (MissionData.nTargetTraceComplete != null && MissionData.nTargetTraceComplete != "")
                                    //{
                                    //    ScriptMissionInfo.m_vecTraceDescList.Add(MissionData.nTargetTraceComplete);
                                    //}
                                    //if (MissionData.nTargetTraceOne != null && MissionData.nTargetTraceOne != "")
                                    //{
                                    //    ScriptMissionInfo.m_vecTraceDescList.Add(MissionData.nTargetTraceOne);
                                    //}
                                    //if (MissionData.nTargetTraceTwo != null && MissionData.nTargetTraceTwo != "")
                                    //{
                                    //    ScriptMissionInfo.m_vecTraceDescList.Add(MissionData.nTargetTraceTwo);
                                    //}
                                    //if (MissionData.nTargetTraceThree != null && MissionData.nTargetTraceThree != "")
                                    //{
                                    //    ScriptMissionInfo.m_vecTraceDescList.Add(MissionData.nTargetTraceThree);
                                    //}
                                    //if (MissionData.nTargetTraceFour != null && MissionData.nTargetTraceFour != "")
                                    //{
                                    //    ScriptMissionInfo.m_vecTraceDescList.Add(MissionData.nTargetTraceFour);
                                    //}
                                    //if (MissionData.nTargetTraceFive != null && MissionData.nTargetTraceFive != "")
                                    //{
                                    //    ScriptMissionInfo.m_vecTraceDescList.Add(MissionData.nTargetTraceFive);
                                    //}

                                    if (ScriptMissionInfo.m_misName == string.Empty)
                                    {
                                        ScriptMissionInfo.m_misName = MissionDialog.szMissionName;
                                    }
                                    if (ScriptMissionInfo.m_misDesc == string.Empty)
                                    {
                                        ScriptMissionInfo.m_misDesc = MissionDialog.szMissionDesc;
                                    }
                                }
                            }
                        }
                    }
                }
                ScriptMissionInfo.m_bFill = true;
            }
        }

        //// 因为任务追踪数据依赖于玩家数据,如位置等,需要重新解析 [6/29/2011 ivan edit]
        //public void ReParseMission(int missIndex)
        //{
        //    _OWN_MISSION OwnMission = CDetailAttrib_Player.Instance.GetMission(missIndex);
        //    if (OwnMission == null)
        //    {
        //        return;
        //    }

        //    int scriptId = OwnMission.m_idScript;
        //    int missionId = OwnMission.m_idMission;
        //    //ScriptMissionInfo = CDetailAttrib_Player.Instance.GetMissionInfo(missionId, ref CDetailAttrib_Player.Instance.MisInfo);

        //    // 因为任务追踪数据依赖于玩家数据,如位置等,需要重新解析 [6/29/2011 ivan edit]
        //    MissionStruct.Instance.RemoveMissionInfo(missIndex);

        //    //填充任务描述数据
        //    ParseMission(missionId, scriptId);
        //}

        //取得当前对话npc数据
        public byte GetNPCEventList_Num()
        {
            return CUIDataPool.Instance.m_pEventList.m_yItemCount;
        }
    }
}