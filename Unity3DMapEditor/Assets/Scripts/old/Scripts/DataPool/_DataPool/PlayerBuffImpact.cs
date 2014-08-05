using System.Collections.Generic;
using Network.Packets;
using Network;
using DBSystem;
/// <summary>
/// 玩家的buff列表，源自：CDataPool
/// </summary>
public class PlayerBuffImpact
{
    ////------------
    //// 技能的BUFF数据
    ////------------
    protected Dictionary<int, _BUFF_IMPACT_INFO> m_mapBuffImpact = new Dictionary<int, _BUFF_IMPACT_INFO>();
    protected uint                               m_nBuffImpactTime = 0;

    public int				    BuffImpact_GetCount(){ return m_mapBuffImpact.Count; }
    public _BUFF_IMPACT_INFO	BuffImpact_Get(int nSN )
    {
        if (m_mapBuffImpact.ContainsKey(nSN))
        {
            return m_mapBuffImpact[nSN];
        }
        return null;
    }
    public uint ImpactTime
    {
        get { return m_nBuffImpactTime; }
        set { m_nBuffImpactTime = value; }
    }
    public _BUFF_IMPACT_INFO	BuffImpact_GetByIndex( int nIndex )
    {
        if (m_mapBuffImpact.Count > nIndex)
        {
            int curIndex = 0;
            //这样遍历貌似有点问题？
            foreach (int key in m_mapBuffImpact.Keys)
            {
                if (curIndex == nIndex)
                {
                    return m_mapBuffImpact[key];
                }
                curIndex++;
            }
        }
        return null;
    }
    public _BUFF_IMPACT_INFO	BuffImpact_GetByID( int nID )
    {
        //这样判定合乎逻辑么？
        if (m_mapBuffImpact.Count > nID)
        {
            foreach (_BUFF_IMPACT_INFO buffImpact in m_mapBuffImpact.Values)
            {
                if (buffImpact.m_nBuffID == nID)
                {
                    return buffImpact;
                }
            }
        }
        return null;
    }
    public bool					BuffImpact_Add(_BUFF_IMPACT_INFO pBuffImpact )
    {
        if (pBuffImpact == null)
            return false;

        int nSN = (int)pBuffImpact.m_nSN;
        if (!m_mapBuffImpact.ContainsKey(nSN))
        {
            m_mapBuffImpact.Add(nSN, pBuffImpact);
        }
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE);
        return true;
    }

    public void					BuffImpact_Remove( int nSN )
    {
        if (m_mapBuffImpact.ContainsKey(nSN))
        {
            m_mapBuffImpact.Remove(nSN);
        }
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE);
    }

    public void					BuffImpact_RemoveByIndex( int nIndex )
    {
        if (m_mapBuffImpact.Count > nIndex)
        {
            int curIndex = 0;
            //这样遍历貌似有点问题？
            foreach (int key in m_mapBuffImpact.Keys)
            {
                if (curIndex == nIndex)
                {
                    m_mapBuffImpact.Remove(key);
                    return;
                }
                curIndex++;
            }
        }
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE);
    }

    public void					BuffImpact_RemoveAll()
    {
        m_mapBuffImpact.Clear();
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE);
    }

    public string			    BuffImpact_GetToolTips( int nSN )
    {
        _BUFF_IMPACT_INFO pBuffImpactInfo = BuffImpact_Get(nSN);
        if (pBuffImpactInfo != null)
        {
            _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU(pBuffImpactInfo.m_nBuffID);
            if (pBuffImpact != null)
            {
                return pBuffImpact.m_pszInfo;
            }
        }
        return "";

    }
    public string			    BuffImpact_GetToolTipsByIndex( int nIndex )
    {
        _BUFF_IMPACT_INFO pBuffImpactInfo = BuffImpact_GetByIndex(nIndex);
        if (pBuffImpactInfo != null)
        {
            _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU(pBuffImpactInfo.m_nBuffID);
            if (pBuffImpact != null)
            {
                return pBuffImpact.m_pszInfo;
            }
        }
        return "";
    }

    public string               BuffImpact_GetIconNameByIndex(int nIndex)
    {
        _BUFF_IMPACT_INFO pBuffImpactInfo = BuffImpact_GetByIndex(nIndex);
        if (pBuffImpactInfo != null)
        {
            _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU(pBuffImpactInfo.m_nBuffID);
            if (pBuffImpact != null)
            {
                return pBuffImpact.m_szIconName;
            }
        }
        return "";
    }


    public bool					BuffImpact_Dispel( int nSN )
    {
        _BUFF_IMPACT_INFO BuffImpactInfo = BuffImpact_Get(nSN);
        if (BuffImpactInfo != null)
        {
            CGDispelBuffReq msgDispelBuffReq = new CGDispelBuffReq();
            msgDispelBuffReq.SN = (int)nSN;
            NetManager.GetNetManager().SendPacket(msgDispelBuffReq);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool					BuffImpact_DispelByIndex( int nIndex )
    {
        _BUFF_IMPACT_INFO BuffImpactInfo = BuffImpact_GetByIndex(nIndex);
        if (BuffImpactInfo != null)
        {
            CGDispelBuffReq msgDispelBuffReq = new CGDispelBuffReq();
            msgDispelBuffReq.SN = (int)BuffImpactInfo.m_nSN;
            NetManager.GetNetManager().SendPacket(msgDispelBuffReq);
            return true;
        }
        return false;
    }
    public int					BuffImpact_GetTime( int nIndex )
    {
        _BUFF_IMPACT_INFO BuffImpactInfo = BuffImpact_Get(nIndex);
        if (BuffImpactInfo != null)
        {
            return BuffImpactInfo.m_nTimer;
        }
        else
        {
            return -1;
        }
    }
    public int					BuffImpact_GetTimeByIndex( int nIndex )
    {
        _BUFF_IMPACT_INFO BuffImpactInfo = BuffImpact_GetByIndex(nIndex);
        if (BuffImpactInfo != null)
        {
            return BuffImpactInfo.m_nTimer;
        }
        else
        {
            return -1;
        }
    }

    public void Tick(uint timeNow)
    {
        uint nIntervalTime = timeNow - ImpactTime;
        if (nIntervalTime >= 1000)
        {
            m_nBuffImpactTime = timeNow;
            if (m_mapBuffImpact.Count > 0)
            {
                //可以用另外的m_nTimer = 0 和m_nTimer ！=0 两个列表来遍历节省时间，但是如果这个列表量少的话，其实是可以接受这种简单一种列表
                foreach (_BUFF_IMPACT_INFO buffImpact in m_mapBuffImpact.Values)
                {
                    if (buffImpact.m_nTimer > 0)
                    {
                        buffImpact.m_nTimer -= (int)nIntervalTime;
                        if (buffImpact.m_nTimer < 0)
                        {
                            buffImpact.m_nTimer = 0;
                        }
                    }
                }
            }
        }
    }
}