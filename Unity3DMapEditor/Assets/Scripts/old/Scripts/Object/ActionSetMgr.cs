using UnityEngine;
using System.Collections;
using DBSystem;
using System.Collections.Generic;
using DBC;

public class CActionSetMgr
{
    protected static readonly CActionSetMgr sInstance = new CActionSetMgr();
    protected Dictionary<string, object> m_mapActionSet = new Dictionary<string, object>();
    protected string m_strResPath = string.Empty;

    static public CActionSetMgr Instance
    {
        get
        {
            return sInstance;
        }
    }
    public bool Init(string strResPath)
    {
        m_strResPath = strResPath;
        return true;
    }

    public void CleanUp()
    {
        m_mapActionSet.Clear();
    }

    public _DBC_CHARACTER_ACTION_SET GetActionSet(string strFileName, uint dwID)
    {
        bool bFind = false;
        COMMON_DBC<_DBC_CHARACTER_ACTION_SET> pActionDBCFile = new COMMON_DBC<_DBC_CHARACTER_ACTION_SET>();
        if (!m_mapActionSet.ContainsKey(strFileName))
        {
            string strPathName = m_strResPath + strFileName;
            bool bResult = pActionDBCFile.OpenFromTXT(strPathName, DBStruct.GetResources);
            if (bResult)
            {
                m_mapActionSet.Add(strFileName, pActionDBCFile);
                bFind = m_mapActionSet.ContainsKey(strFileName);
            }
        }

        if (!bFind)
        {
            return null;
        }

        pActionDBCFile = (COMMON_DBC<_DBC_CHARACTER_ACTION_SET>)m_mapActionSet[strFileName];
        _DBC_CHARACTER_ACTION_SET pActionSet = (_DBC_CHARACTER_ACTION_SET)pActionDBCFile.Search_Index_EQU((int)dwID);
        return pActionSet;
    }

    public COMMON_DBC<_DBC_CHARACTER_ACTION_SET> GetActionSetFile(string strFileName)
    {
        bool bFind = false;
        COMMON_DBC<_DBC_CHARACTER_ACTION_SET> pActionDBCFile = new COMMON_DBC<_DBC_CHARACTER_ACTION_SET>();
        if (!m_mapActionSet.ContainsKey(strFileName))
        {
            string strPathName = m_strResPath + strFileName;
            //LogManager.LogWarning("Open action file : " + strPathName);
            bool bResult = pActionDBCFile.OpenFromTXT(strPathName, DBStruct.GetResources);
            if (bResult)
            {
                m_mapActionSet.Add(strFileName, pActionDBCFile);
                bFind = m_mapActionSet.ContainsKey(strFileName);
            }
        }
		else
		{
			bFind = true;
		}
		
        if (!bFind)
        {
            return null;
        }

        return (COMMON_DBC<_DBC_CHARACTER_ACTION_SET>)m_mapActionSet[strFileName];
    }
}
