using System;
using System.Text.RegularExpressions;
using DBSystem;
using UnityEngine;
using System.Collections.Generic;

public struct MISSIONSCRIPT
{
    public static int MAX_COUNT = 10;
};

//------------------------------------------------------
//任务奖励物品
public class QuestRewardItem
{
    public SMissionBonus pItemData = new SMissionBonus();
    public CObject_Item pItemImpl;
    public int bSelected;
}

//------------------------------------------------------
//任务需要物品
public class QuestDemandItem
{
    public SMissionBonusItem pDemandItem = new SMissionBonusItem();
    public CObject_Item pItemImpl;
    //		std::string				pItemDesc;
    //		std::string				pItemPos;
}

//------------------------------------------------------
//任务需要杀死的NPC
public class QuestDemandKill
{
    public SMissionDemandKill pDemandKill = new SMissionDemandKill();
    public string szNPCName;
    //		std::string				szNPCDesc;
    //		std::string				szNPCPos;
}
//------------------------------------------------------
//任务需要的自定义
public struct QuestCustom
{
    public string szCustomString;	//任务自定义的字符串
    public int nCount;			//需要这个东东的数量
}

public class _MISSION_INFO
{
    public bool m_bFill = false;
    public int m_nLevel;        //任务等级
    public int m_nKind;         //任务类型
    //INT   m_nType;			//属于哪种类型的任务
    public int m_nElite;        //任务是否较难或者要多人才可以完成

    //任务是否需要显示某些值

    public int m_nOKFail;       // 任务是否完成
    public int m_nLimitTime;    // 任务剩余时间
    public int m_nRound;	    // 任务环数
    public int m_nBill;			// 任务的银票数
    public int m_nStrForePart;  // 自定义数据起始索引

    //	INT		m_TalkNpcID;
    //	std::string m_TalkNpcName;
    //	std::string m_TalkNpcDesc;
    //	std::string m_TalkNpcPos;


    public List<QuestRewardItem> m_vecQuestRewardItem = new List<QuestRewardItem>();
    public List<QuestDemandItem> m_vecQuestDemandItem = new List<QuestDemandItem>();
    public List<QuestDemandKill> m_vecQuestDemandKill = new List<QuestDemandKill>();
    public List<QuestCustom> m_vecQuestCustom = new List<QuestCustom>();
    public List<string> m_vecFormatList = new List<string>();
    public List<string> m_vecStrList = new List<string>();
    
    // 任务追踪面板描述 [3/12/2012 Ranger]
    public List<string> m_vecTraceDescList = new List<string>();
    //	std::vector< _ExtraInfo >		m_vecExtraInfos;

    public string m_misName;
    public string m_misDesc;
    //	std::string m_misTargetDesc;
    public string m_misState;
    public string m_misBillName;
    public string m_finishNpcName;
    // 玩家接受任务后自动执行的超链接 [4/17/2012 Ivan]
    public string acceptLink;
	//完成任务目标[5/9/2012 shen]
	public string m_finishTarget;
    //	INT			n_finishSceneId;
    //	FLOAT		n_finishPosX;
    //	FLOAT		n_finishPosY;
    //	std::string m_acceptNpcName;
    //	INT			n_acceptSceneId;
    //	FLOAT		n_acceptPosX;
    //	FLOAT		n_acceptPosY;
}

public class MissionStruct
{
    static readonly MissionStruct instance = new MissionStruct();
    public static MissionStruct Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// 存放解析脚本后的信息。以脚本中参数名_g_后面的为map索引 [2/20/2012 Ruanmin]
    /// </summary>
    private Dictionary<string, string> ScriptInfoMap = new Dictionary<string, string>();

    /// <summary>
    /// 临时存放读取脚本中得非注释行
    /// </summary>
    private List<string> ListTemp = new List<string>();

    /// <summary>
    /// 要用到的正则表达式
    /// </summary>
    private Regex StringRegex = new Regex(@"(?<={)[^{}]+(?=})");
    private Regex StringRegex2 = new Regex(@"(?<=<)[^<>]+(?=>)");

    /// <summary>
    /// 存放所有任务信息的map
    /// </summary>
    public Dictionary<int, _MISSION_INFO> MisInfo = new Dictionary<int, _MISSION_INFO>();

    public _MISSION_INFO GetMissionInfo(int missionId)
    {
        if (!MisInfo.ContainsKey(missionId))
        {
            _MISSION_INFO misInfo = new _MISSION_INFO();
            misInfo.m_bFill = false;
            misInfo.m_misBillName = "";
            misInfo.m_misDesc = "";
            misInfo.m_misName = "";
            misInfo.m_misState = "";
            misInfo.m_nBill = -1;
            misInfo.m_nElite = -1;
            misInfo.m_nKind = 1;
            misInfo.m_nLevel = -1;
            misInfo.m_nLimitTime = -1;
            misInfo.m_nOKFail = -1;
            misInfo.m_nRound = -1;

            MisInfo.Add(missionId, misInfo);
        }

        return MisInfo[missionId];
    }

    public void RemoveMissionInfo(int missIndex)
    {
        _OWN_MISSION OwnMission = CDetailAttrib_Player.Instance.GetMission(missIndex);

        // 统一修改为使用任务ID来管理任务信息 [7/4/2011 ivan edit]
        int missId = OwnMission.m_idMission;
        _MISSION_INFO MissionInfo = GetMissionInfo(missId);

        for (int i = 0; i < MissionInfo.m_vecQuestDemandItem.Count; i++)
        {
            if (MissionInfo.m_vecQuestDemandItem[i].pItemImpl != null)
                ObjectSystem.Instance.DestroyItem(MissionInfo.m_vecQuestDemandItem[i].pItemImpl);
        }
        for (int i = 0; i < MissionInfo.m_vecQuestRewardItem.Count; i++)
        {
            if (MissionInfo.m_vecQuestRewardItem[i].pItemImpl != null)
                ObjectSystem.Instance.DestroyItem(MissionInfo.m_vecQuestRewardItem[i].pItemImpl);
        }

        MissionInfo.m_misName = "";
        MissionInfo.m_misDesc = "";

        if (MisInfo.ContainsKey(missId))
        {
            MisInfo.Remove(missId);
        }
    }

    public void ClearAllMissionInfo()
    {
        foreach (int MisInfoKey in MisInfo.Keys)
        {
            _MISSION_INFO MissionInfoValue = MisInfo[MisInfoKey];

            for (int i = 0; i < MissionInfoValue.m_vecQuestDemandItem.Count; i++)
            {
                if (MissionInfoValue.m_vecQuestDemandItem[i].pItemImpl != null)
                    ObjectSystem.Instance.DestroyItem(MissionInfoValue.m_vecQuestDemandItem[i].pItemImpl);
            }
            for (int i = 0; i < MissionInfoValue.m_vecQuestRewardItem.Count; i++)
            {
                if (MissionInfoValue.m_vecQuestRewardItem[i].pItemImpl != null)
                    ObjectSystem.Instance.DestroyItem(MissionInfoValue.m_vecQuestRewardItem[i].pItemImpl);
            }
        }
        MisInfo.Clear();
    }

    private bool GetScriptInfoMap(ref Dictionary<string, string> ScriptInfoMap, List<string> LString)
    {
        try
        {
            for (int i = 0; i < LString.Count; i++)
            {
                int pos = LString[i].IndexOf("=");
                if (pos == -1)
                {
                    LogManager.LogError("--------------------要打开的文件名为空!--------------------");
                    return false;
                }

                string StringTempLeft = LString[i].Substring(0, pos);
                string StringTempRight = LString[i].Substring(pos + 1);

                pos = StringTempRight.IndexOf("--");
                if (pos != -1)
                {
                    StringTempRight = StringTempRight.Substring(0, pos).Trim();
                }

                pos = StringTempLeft.IndexOf("_g_");
                if (pos == -1)
                {
                    LogManager.LogError("--------------------脚本属性名称出错!--------------------");
                    return false;
                }

                StringTempLeft = StringTempLeft.Substring(pos + 3);
                ScriptInfoMap.Add(StringTempLeft.Trim(), StringTempRight.Trim());
            }
            return true;
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------从map中读取脚本数据出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    public bool OpenScript(string filename)
    {
        try
        {
            if (filename == string.Empty)
            {
                LogManager.LogError("--------------------要读取的名为: " + filename + " 的Lua脚本为空。--------------------");
                return false;
            }

//             UnityEngine.Object FileObject = Resources.Load(filename);
// 
//             if (FileObject == null)
//             {
//                 LogManager.LogError("\r\n----------------------------------------------------\r\n");
//                 LogManager.LogError("Load Resources名为: " + filename + " 的Lua脚本为空。");
//                 LogManager.LogError("\r\n----------------------------------------------------\r\n");
//                 return false;
//             }
// 
            //             TextAsset asset = (TextAsset)FileObject;
            // 对文件名特殊处理 [3/16/2012 Ivan]
            filename = filename.Replace(".lua", ".txt");
            filename = filename.Replace("\\", "/");
            string fileContent = DBStruct.GetResources(filename);

            string[] strsTemp = fileContent.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < strsTemp.Length; i++)
            {
                strsTemp[i] = strsTemp[i].Trim();
                if (!strsTemp[i].Equals(""))
                {
                    if (!strsTemp[i].StartsWith("--"))
                    {
                        if (i >= 1)
                        {
                            if (!strsTemp[i].StartsWith("x"))
                            {
                                int j = 1;
                                while (strsTemp[i - j].Equals(""))
                                {
                                    j++;
                                }
                                strsTemp[i - j] += strsTemp[i];
                                strsTemp[i] = string.Empty;
                            }
                        }
                        else
                        {
                            LogManager.LogError("--------------------脚本第一个属性出错，不是以x开头!--------------------");
                            return false;
                        }
                    }
                }
            }
			
			ListTemp.Clear();
            for (int i = 0; i < strsTemp.Length; i++)
            {
                if (!strsTemp[i].Equals(""))
                {
                    if (!strsTemp[i].StartsWith("--"))
                    {
                        ListTemp.Add(strsTemp[i]);
                    }
                }
            }
			
			ScriptInfoMap.Clear();
            if (GetScriptInfoMap(ref ScriptInfoMap, ListTemp))
            {
                if (!ParseScript(ScriptInfoMap))
                {
                    LogManager.LogError("--------------------解析脚本出错。出错脚本名为:" + filename + " 。--------------------");
                    return false;
                }
            }
            else
            {
                LogManager.LogError("--------------------从ScriptInfoMap中获取脚本信息出错。出错脚本名为:" + filename + " 。--------------------");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------读取名为: " + filename + " 的Lua脚本出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private bool ParseScript(Dictionary<string, string> ScriptInfoMap)
    {
        try
        {
            _MISSION_INFO MissionInfo = new _MISSION_INFO();
            int MissionID = MacroDefine.INVALID_ID;
            foreach (string stringkey in ScriptInfoMap.Keys)
            {
                if (ScriptInfoMap[stringkey].Equals(""))
                {
                    LogManager.LogError("--------------------参数 " + stringkey + " 属性未赋值!--------------------");
                    return false;
                }
                if (stringkey == "ScriptId")
                {
                    continue;
                }
                if (stringkey == "MissionId")
                {
                    MissionID = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MissionIdPre")
                {
                    continue;
                }
                if (stringkey == "MissionIdNext")
                {
                    continue;
                }
                if (stringkey == "AccomplishNPC_Name")
                {
                    continue;
                }
                if (stringkey == "NPC_Name")
                {
                    MissionInfo.m_finishNpcName = FormatString(ScriptInfoMap[stringkey]);
                    continue;
                }
                if (stringkey == "AcceptLink")
                {
                    MissionInfo.acceptLink = UIString.Instance.parseStringDictionary(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MissionKind")
                {
                    MissionInfo.m_nKind = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MissionLevel")
                {
                    MissionInfo.m_nLevel = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "IfMissionElite")
                {
                    MissionInfo.m_nElite = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MisParamIndex_KillObjOne")
                {
                    continue;
                }
                if (stringkey == "MissionName")
                {
                    MissionInfo.m_misName = FormatString(ScriptInfoMap[stringkey]);
                    continue;
                }
                if (stringkey == "MissionOKFail")
                {
                    MissionInfo.m_nOKFail = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MissionLimitTime")
                {
                    MissionInfo.m_nLimitTime = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MissionRound")
                {
                    MissionInfo.m_nRound = Convert.ToInt32(FormatString(ScriptInfoMap[stringkey]));
                    continue;
                }
                if (stringkey == "MissionBill")
                {
                    continue;
                }
                if (stringkey == "MissionBillName")
                {
                    continue;
                }
                if (stringkey == "MissionStrForePart")
                {
                    continue;
                }
                if (stringkey == "MissionInfo" || stringkey == "MissionInfo2")
                {
                    MissionInfo.m_misDesc = FormatString(ScriptInfoMap[stringkey]);
                    continue;
                }
                if (stringkey == "MissionComplete")
                {
                    continue;
                }
                if (stringkey == "MissionInfo1")
                {
                    continue;
                }
                if (stringkey == "MissionTarget")
                {
                    continue;
                }
                if (stringkey == "MissionBriefTarget")
                {
                    continue;
                }
                if (stringkey == "MissionFormatList")
                {
                    continue;
                }
                if (stringkey == "MissionStrList")
                {
                    continue;
                }
                if (stringkey == "MissionTraceDesc" || stringkey == "TraceDesc")
                {
                    parseTraceDesc(ScriptInfoMap[stringkey], ref MissionInfo);
                    continue;
                }
                if (stringkey == "MoneyBonus")
                {
                    SMissionBonus rewardItem = new SMissionBonus();
                    rewardItem.m_uMoney = Convert.ToUInt32(FormatString(ScriptInfoMap[stringkey]));
                    rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_MONEY;

                    QuestRewardItem newRewardItem = new QuestRewardItem();

                    newRewardItem.pItemData = rewardItem;
                    newRewardItem.bSelected = 0;

                    MissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                    continue;
                }
                if (stringkey == "ExpBonus")
                {
                    SMissionBonus rewardItem = new SMissionBonus();
                    rewardItem.m_uExp = Convert.ToUInt32(FormatString(ScriptInfoMap[stringkey]));
                    rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_EXP;

                    QuestRewardItem newRewardItem = new QuestRewardItem();

                    newRewardItem.pItemData = rewardItem;
                    newRewardItem.bSelected = 0;

                    MissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                    continue;
                }
                if (stringkey == "NeedItem")
                {
                    parseDemandItem(ScriptInfoMap[stringkey], ref MissionInfo);
                    continue;
                }
                if (stringkey == "ItemBonus")
                {
                    parseItemBonus(ScriptInfoMap[stringkey], ref MissionInfo);
                    continue;
                }
                if (stringkey == "MustDeleteItem")
                {
                    continue;
                }
                if (stringkey == "DemandItem")
                {
                    parseDemandItem(ScriptInfoMap[stringkey], ref MissionInfo);
                    continue;
                }
                if (stringkey == "KillObj")
                {
                    parseDemandKill(ScriptInfoMap[stringkey], ref MissionInfo);
                    continue;
                }
                if (stringkey == "CustomItem")
                {
                    parseCustomItem(ScriptInfoMap[stringkey], ref MissionInfo);
                    continue;
                }
				if(stringkey == "MissionFinish")
				{
					MissionInfo.m_finishTarget = FormatString(ScriptInfoMap[stringkey]);
					continue;
				}
            }

            if (MissionID == MacroDefine.INVALID_ID)
            {
                return false;
            }

            MissionInfo.m_bFill = true;
            MisInfo[MissionID] = MissionInfo;

            return true;
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------解析脚本出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    // 除去掉等号和多余的空格,引号等等 [3/13/2012 Ranger]
    private string FormatString(string TempString)
    {
		//去除lua的拼接符
        TempString = TempString.Replace("\"..\"", "\"\"");
        //去引号
        TempString = TempString.Replace("\"", "");

        //去等号并Trim
        int pos = TempString.IndexOf("=");
        if (pos != -1)
        {
            string StringTempRight = TempString.Substring(pos + 1).Trim();
            return StringTempRight;
        }
        return TempString.Trim();
    }

    private void parseDemandItem(string DemandItem, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!DemandItem.Equals("nil"))
            {
                MatchCollection matches = StringRegex.Matches(DemandItem);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的需求物品项。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;
                    string[] DemandItemstr = matchstr.Split(new string[1] { "," }, StringSplitOptions.None);
                    uint itemId = Convert.ToUInt32(FormatString(DemandItemstr[0]));
                    int itemCount = Convert.ToInt32(FormatString(DemandItemstr[1]));

                    CObject_Item pItem = ObjectSystem.Instance.NewItem(itemId);
                    if (pItem == null)
                    {
                        LogManager.LogError("--------------------非法的奖励物品ID: " + itemId + "--------------------");
                    }
                    pItem.SetNumber(itemCount);
                    CActionSystem.Instance.UpdateAction_FromItem(pItem);

                    QuestDemandItem newDemandItem = new QuestDemandItem();
                    newDemandItem.pDemandItem = new SMissionBonusItem();
                    newDemandItem.pDemandItem.m_uItemID = itemId;
                    newDemandItem.pDemandItem.m_yCount = itemCount;
                    pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                    newDemandItem.pItemImpl = pItem;
                    MissionInfo.m_vecQuestDemandItem.Add(newDemandItem);
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseDemandItem方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private void parseItemBonus(string ItemBonus, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!ItemBonus.Equals("nil"))
            {
                MatchCollection matches = StringRegex.Matches(ItemBonus);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的奖励物品项。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;
                    string[] ItemBonusstr = matchstr.Split(new string[1] { "," }, StringSplitOptions.None);
                    uint itemId = Convert.ToUInt32(FormatString(ItemBonusstr[0]));
                    int itemCount = Convert.ToInt32(FormatString(ItemBonusstr[1]));

                    SMissionBonus rewardItem = new SMissionBonus();
                    rewardItem.m_ItemBonus.m_uItemID = itemId;
                    rewardItem.m_ItemBonus.m_yCount = itemCount;
                    rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM;

                    CObject_Item pItem = ObjectSystem.Instance.NewItem(itemId);
                    if (pItem == null)
                    {
                        LogManager.LogError("--------------------非法的奖励物品ID: " + itemId + "--------------------");
                    }
                    pItem.SetNumber(itemCount);
                    CActionSystem.Instance.UpdateAction_FromItem(pItem);

                    QuestRewardItem newRewardItem = new QuestRewardItem();
                    newRewardItem.pItemData = rewardItem;
                    newRewardItem.bSelected = 0;
                    pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                    newRewardItem.pItemImpl = pItem;
                    MissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseItemBonus方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private void parseRandItemBonus(string RandItemBonus, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!RandItemBonus.Equals("nil"))
            {
                MatchCollection matches = StringRegex.Matches(RandItemBonus);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的RandItemBonu项。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;
                    string[] RandItemBonusstr = matchstr.Split(new string[1] { "," }, StringSplitOptions.None);
                    uint itemId = Convert.ToUInt32(FormatString(RandItemBonusstr[0]));
                    int itemCount = Convert.ToInt32(FormatString(RandItemBonusstr[1]));

                    SMissionBonus rewardItem = new SMissionBonus();
                    rewardItem.m_ItemBonus.m_uItemID = itemId;
                    rewardItem.m_ItemBonus.m_yCount = itemCount;
                    rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RAND;

                    //只显示为“问号”型物品
                    //tObject_Item* pItem = CObject_Item::NewItem(rewardItem->m_ItemBonus.m_uItemID);
                    //CActionSystem::GetMe()->UpdateAction_FromItem(pItem);
                    CObject_Item pItem = ObjectSystem.Instance.NewItem(20005020);//只显示为“问号”型物品
                    CActionSystem.Instance.UpdateAction_FromItem(pItem);

                    QuestRewardItem newRewardItem = new QuestRewardItem();

                    newRewardItem.pItemData = rewardItem;
                    newRewardItem.bSelected = 0;
                    pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                    newRewardItem.pItemImpl = pItem;

                    MissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseRandItemBonu方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private void parseRadioItemBonus(string RadioItemBonus, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!RadioItemBonus.Equals("nil"))
            {
                MatchCollection matches = StringRegex.Matches(RadioItemBonus);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的RadioItem项。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;
                    string[] RadioItemBonusstr = matchstr.Split(new string[1] { "," }, StringSplitOptions.None);
                    uint itemId = Convert.ToUInt32(FormatString(RadioItemBonusstr[0]));
                    int itemCount = Convert.ToInt32(FormatString(RadioItemBonusstr[1]));

                    SMissionBonus rewardItem = new SMissionBonus();
                    rewardItem.m_ItemBonus.m_uItemID = itemId;
                    rewardItem.m_ItemBonus.m_yCount = itemCount;
                    rewardItem.m_nType = ENUM_MISSION_BONUS_TYPE.MISSION_BONUS_TYPE_ITEM_RADIO;

                    CObject_Item pItem = ObjectSystem.Instance.NewItem(itemId);
                    if (pItem == null)
                    {
                        LogManager.LogError("--------------------非法的奖励物品ID: " + itemId + "--------------------");
                    }
                    pItem.SetNumber(itemCount);
                    CActionSystem.Instance.UpdateAction_FromItem(pItem);

                    QuestRewardItem newRewardItem = new QuestRewardItem();
                    //CActionSystem::GetMe()->UpdateAction_FromItem(pItem);
                    newRewardItem.pItemData = rewardItem;
                    pItem.SetTypeOwner(ITEM_OWNER.IO_QUESTVIRTUALITEM);
                    newRewardItem.pItemImpl = pItem;
                    newRewardItem.bSelected = 0;
                    MissionInfo.m_vecQuestRewardItem.Add(newRewardItem);
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseRadioItem方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private void parseDemandKill(string DemandKill, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!DemandKill.Equals("nil"))
            {
                MatchCollection matches = StringRegex.Matches(DemandKill);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的所需杀怪。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;
                    string[] DemandItemstr = matchstr.Split(new string[1] { "," }, StringSplitOptions.None);
                    uint npcId = Convert.ToUInt32(FormatString(DemandItemstr[0]));
                    int npcCount = Convert.ToInt32(FormatString(DemandItemstr[1]));

                    string npcName = FormatString(DemandItemstr[2]);

                    _DBC_CREATURE_ATT MissionNPC = CDataBaseSystem.Instance.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT).Search_Index_EQU((int)npcId);
                    QuestDemandKill newDemandKill = new QuestDemandKill();
                    newDemandKill.pDemandKill.m_uNPCID = npcId;
                    newDemandKill.pDemandKill.m_yCount = (byte)npcCount;
                    if (MissionNPC.pName == npcName)
                    {
                        newDemandKill.szNPCName = MissionNPC.pName;
                    }
                    else
                    {
                        newDemandKill.szNPCName = npcName;
                        LogManager.LogWarning("--------------------数据库NPC名称与脚本名称不一致。暂以数据库为准。--------------------");
                    }

                    MissionInfo.m_vecQuestDemandKill.Add(newDemandKill);
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseDemandKill方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private void parseTraceDesc(string TraceDesc, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!TraceDesc.Equals("nil"))
            {
                MatchCollection matches = StringRegex2.Matches(TraceDesc);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的任务追踪信息。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;

                    if (!matchstr.Equals(""))
                    {
                        MissionInfo.m_vecTraceDescList.Add(matchstr.Trim());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseTraceDesc方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    private void parseCustomItem(string CustomItem, ref _MISSION_INFO MissionInfo)
    {
        try
        {
            if (!CustomItem.Equals("nil"))
            {
                MatchCollection matches = StringRegex2.Matches(CustomItem);

                if (matches.Count == 0)
                {
                    LogManager.LogError("--------------------客户端配置中不存在匹配的自定义物品项。--------------------");
                }

                foreach (Match match in matches)
                {
                    string matchstr = match.Value;
                    int pos = matchstr.LastIndexOf(",");
                    QuestCustom newQuestCustom = new QuestCustom();
                    newQuestCustom.szCustomString = FormatString(matchstr.Substring(0, pos));
                    newQuestCustom.nCount = Convert.ToInt32(FormatString(matchstr.Substring(pos + 1)));

                    MissionInfo.m_vecQuestCustom.Add(newQuestCustom);
                }
            }
        }
        catch (Exception ex)
        {
            CDataBaseSystem.StrError += "--------------------parseCustomItem方法出错。--------------------";
            CDataBaseSystem.StrError += ex.ToString();
            throw new Exception(CDataBaseSystem.StrError);
        }
    }

    /// <summary>
    /// 获得强制解析后的任务信息
    /// </summary>
    /// <param name="missionId"></param>
    /// <param name="missScriptId"></param>
    /// <returns></returns>
    internal _MISSION_INFO ParseMissionInfo(int missionId,int scriptId)
    {
        _MISSION_INFO info = GetMissionInfo(missionId);
        if (!info.m_bFill)
        {
            Interface.DataPool.Instance.ParseMission(missionId, scriptId);
            info = GetMissionInfo(missionId);
        }
        return info;
    }
}