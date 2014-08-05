using DBSystem;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CSkillDataMgr
{
    static readonly CSkillDataMgr sInstance = new CSkillDataMgr();//唯一的实例
    static public CSkillDataMgr Instance
    {
        get
        {
            return sInstance;
        }
    }

    private Dictionary<uint,List<int>>    m_mapSkillSendAnim;
    private Dictionary<uint, List<int>>   m_mapSkillLeadAnim;
    public CSkillDataMgr()
    {
        m_mapSkillSendAnim = new Dictionary<uint, List<int>>();
        m_mapSkillLeadAnim = new Dictionary<uint, List<int>>();
    }

    public _DBC_SKILL_DATA GetSkillData( uint skillID )
    {
        return CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).Search_Index_EQU((int)skillID);
    }

	public int	GetRandAnim(uint skillID, int nActionIndex, bool bSendAnim)
    {
        List<int> pvTheAnimID = GetAnimList( skillID, bSendAnim );
	    if ( pvTheAnimID == null )
	    {
		    return 0;
	    }

	    if ( pvTheAnimID.Count <= 0 )
		    return 0;

	    int nRandIndex;
	    if ( nActionIndex <= 0 )
		    nRandIndex = 0;
	    else
		    nRandIndex = nActionIndex%(int)pvTheAnimID.Count;
    		
	    return pvTheAnimID[nRandIndex];
    }

	public int	GetAnimCount(uint skillID, bool bSendAnim)
    {
        List<int> pvTheAnimID = GetAnimList(skillID,bSendAnim);
	    if ( pvTheAnimID == null )
	    {
		    return 0;
	    }
    		
	    return pvTheAnimID.Count;
    }

    public List<int> GetAnimList(uint skillID, bool bSendAnim)
    {
        Dictionary<uint, List<int>> skillAnimMap;
        if ( bSendAnim )
	    {
		    skillAnimMap = m_mapSkillSendAnim;
	    }
	    else
	    {
		    skillAnimMap = m_mapSkillLeadAnim;
	    }
        
        if(skillAnimMap.ContainsKey(skillID))
        {
            return skillAnimMap[skillID];
        }
        else
        {
            _DBC_SKILL_DATA skillData = GetSkillData(skillID);
            if(skillData == null)
            {
                return null;
            }

            string[] vAnim;
            char[] splitter= new char[1]{';'};
		    if ( bSendAnim )
		    {
			    
                vAnim = skillData.m_lpszSendActionSetID.Split(splitter,StringSplitOptions.RemoveEmptyEntries);
               
		    }
		    else
		    {
                vAnim = skillData.m_lpszGatherLeadActionSetID.Split(splitter,StringSplitOptions.RemoveEmptyEntries);
		    }

            if(vAnim.Length == 0)
            {
                LogManager.Log("CSkillDataMgr::GetAnimList Invalid skill anim set");
                return null;
            }

            List<int> animateIndexes = new List<int>();
            foreach(string animateIndex in vAnim)
            {
                animateIndexes.Add(System.Convert.ToInt32(animateIndex));
            }
            skillAnimMap.Add(skillID,animateIndexes);
            return skillAnimMap[skillID];
        }
    }    
};