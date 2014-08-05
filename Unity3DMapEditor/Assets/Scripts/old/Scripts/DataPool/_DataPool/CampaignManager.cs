using System.Collections.Generic;

using DBSystem;
using System;

public class CampaignManager
{
    //队伍列表
    List< RECRUIT_INFO> mTeamInfo = new List< RECRUIT_INFO>();

    DateTime mSystemTime;

    public DateTime SystemTime
    {
        get { return mSystemTime; }
        set
        {
            mSystemTime = value;
        }
    }
    //活动数据
    static Dictionary<int, List<_DBC_ACTIVITY_INFO>> s_ActivityDBC = new Dictionary<int, List<_DBC_ACTIVITY_INFO>>();
    public void Initial()
    {
        DBC.COMMON_DBC<_DBC_ACTIVITY_INFO> activityDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_ACTIVITY_INFO>((int)DataBaseStruct.DBC_ACTIVITY_INFO);

        int nCount = activityDBC.StructDict.Count;
        for (int i =0; i < nCount; i++)
        {
            _DBC_ACTIVITY_INFO info = activityDBC.Search_Index_EQU(i);
            //按照类型区分活动
            if (s_ActivityDBC.ContainsKey(info.nActivityType))
                s_ActivityDBC[info.nActivityType].Add(info);
            else
            {
                List<_DBC_ACTIVITY_INFO> infoList = new List<_DBC_ACTIVITY_INFO>();
                infoList.Add(info);
                s_ActivityDBC.Add(info.nActivityType, infoList);
            }
            
        }
    }
    public int GetCampaignCount()
    {
        int nCount = 0;
        for (int i = 0; i < s_ActivityDBC.Count; i++ )
        {
            nCount += s_ActivityDBC[i].Count;
        }
        return nCount;
    }
    public int GetCampaignCount(int nType)
    {
        if (s_ActivityDBC.ContainsKey(nType))
            return s_ActivityDBC[nType].Count;
        return 0;
    }
    public _DBC_ACTIVITY_INFO GetCampaignInfo(int nIndex, int nType)
    {
        List<_DBC_ACTIVITY_INFO> listInfo;
        if (s_ActivityDBC.TryGetValue(nType, out listInfo))
        {
            if (nIndex >= 0 && nIndex < listInfo.Count)
                return listInfo[nIndex];
        }

        return null;
    }
    public _DBC_ACTIVITY_INFO GetCampaignInfo(int nID)
    {
        for (int i = 0; i < s_ActivityDBC.Count; i++)
        {
            List<_DBC_ACTIVITY_INFO> listInfo = s_ActivityDBC[i];
            foreach (_DBC_ACTIVITY_INFO info in listInfo)
            {
                if (info.nID == nID)
                {
                    return info;
                }
            }
        }
        return null;
    }
    public bool CheckCampaignTime(int nId)
    {
        for (int i = 0; i < s_ActivityDBC.Count; i++ )
        {
            List<_DBC_ACTIVITY_INFO> listInfo = s_ActivityDBC[i];
            foreach (_DBC_ACTIVITY_INFO info in listInfo)
            {
                if (info.nID == nId)
                {
                    DateTime time;
                    if (DateTime.TryParse(info.StartTime, out time))
                    {
                        if (DateTime.Now < time)
                            return false;
                        if (DateTime.TryParse(info.EndTime, out time))
                        {
                            if (DateTime.Now > time)
                                return false;
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool CheckMySelf(int nId)
    {
        _DBC_ACTIVITY_INFO info = GetCampaignInfo(nId);
        if (info != null)
        {
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level() >= info.nLevel)
                return true;
        }
        return false;
    }


#region  Current Campaign Team

    //当前打开的活动id
    short mCampaignId;
    public short CampaignID
    {
        set { mCampaignId = value; }
        get { return mCampaignId; }
    }
    public void AddTeamInfo(RECRUIT_INFO info)
    {
        bool bFind = false;
        for (int i = 0; i < mTeamInfo.Count; i++)
        {
            if (mTeamInfo[i].m_TeamID == info.m_TeamID)
            {
                mTeamInfo[i].m_TeamNum = info.m_TeamNum;
                mTeamInfo[i].m_szUserName = info.m_szUserName;
                mTeamInfo[i].m_NameLen = info.m_NameLen;

                bFind = true;
                break;
            }
        }
        if(!bFind)
            mTeamInfo.Add(info);
    }

    public int GetTeamCount()
    {
        return mTeamInfo.Count;
    }
    public RECRUIT_INFO GetTeamInfo(int nIndex)
    {
        if (nIndex < 0 || mTeamInfo.Count <= nIndex)
            return null;

        return mTeamInfo[nIndex];
    }
    public void ClearTeamInfo()
    {
        mTeamInfo.Clear();
    }
#endregion
}