using System.Collections.Generic;
/// <summary>
/// 冷却组管理器，原CDataPool里的冷却组管理
/// </summary>
/// 
//------------
//冷却组
//------------
public class COOLDOWN_GROUP
{
    public int nTime;
    public int nTotalTime;
    public int nTimeElapsed;
	public COOLDOWN_GROUP()
	{
		nTime 		 = 0;
		nTotalTime   = 0;
		nTimeElapsed = 0;
	}
};
public enum CDGROUPID
{
    GROUPID_XINFA_UOLEVEL = 0, //心法升级队列
    GROUPID_EQUIPCOMPOUND = 1, //装备强化队列
}
public class CoolDownManager
{

    private int m_nCommonCoolDown;
    private COOLDOWN_GROUP[] m_vCoolDownGroup = new COOLDOWN_GROUP[(int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE];
    private COOLDOWN_GROUP[] m_vPetSkillCoolDownGroup = new COOLDOWN_GROUP[(int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE * GAMEDEFINE.HUMAN_PET_MAX_COUNT];
   
    //取得冷却组
    public CoolDownManager()
    {
        for(int i = 0; i < m_vCoolDownGroup.Length;i++)
        {
            if(m_vCoolDownGroup[i] == null)
            {
                m_vCoolDownGroup[i] = new COOLDOWN_GROUP();
            }
        }

        for (int i = 0; i < m_vPetSkillCoolDownGroup.Length; i++)
        {
            if (m_vPetSkillCoolDownGroup[i] == null)
            {
                m_vPetSkillCoolDownGroup[i] = new COOLDOWN_GROUP();
            }
        }
    }

    public void Initial()
    {
        for (int i = 0; i < (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE; i++)
        {
            m_vCoolDownGroup[i].nTime = -1;
            m_vCoolDownGroup[i].nTotalTime = -1;
        }
        //初始化任务时间组
        //for (int i = 0; i < QUESTTIME_LIST_SIZE; i++)
        //{
        //    m_vQuestTimeGroup[i] = -1;
        //}
        m_nCommonCoolDown = -1;

        for (int i = 0; i < (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE * GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++)
        {
            m_vPetSkillCoolDownGroup[i].nTime = -1;
            m_vPetSkillCoolDownGroup[i].nTotalTime = -1;
        }

    }

    public COOLDOWN_GROUP CoolDownGroup_Get(int nCoolDownID)
    {
        if (nCoolDownID < m_vCoolDownGroup.Length)
            return m_vCoolDownGroup[nCoolDownID];
        else
            return null;
    }
	//更新冷却组
    public void CoolDownGroup_UpdateList(Cooldown_T[] pCoolDownList, int nUpdateNum) 
    {
        if(nUpdateNum<=0) return;
     
        if(nUpdateNum > m_vCoolDownGroup.Length) nUpdateNum = m_vCoolDownGroup.Length;
        int nCooldownID = MacroDefine.INVALID_ID;
        for(int i=0; nUpdateNum>i; i++)
        {
            nCooldownID = (int)pCoolDownList[i].m_nID;
            if(0> nCooldownID || (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE <= nCooldownID) continue;

            //更新冷却组
            COOLDOWN_GROUP rCooldown = m_vCoolDownGroup[nCooldownID];
            rCooldown.nTotalTime = pCoolDownList[i].m_nCooldown;
            rCooldown.nTimeElapsed = pCoolDownList[i].m_nCooldownElapsed;
            rCooldown.nTime = rCooldown.nTotalTime - rCooldown.nTimeElapsed;

            //刷新的ActionSystem
            CActionSystem.Instance.UpdateCoolDown(nCooldownID);
        }
    }
	//取得宠物冷却组
    public COOLDOWN_GROUP PetSkillCoolDownGroup_Get(int nCoolDownID, int nPetNum)
    {
        if (nCoolDownID >= 0 && nCoolDownID < m_vCoolDownGroup.Length && nPetNum >= 0 && nPetNum < GAMEDEFINE.HUMAN_PET_MAX_COUNT)
            return m_vPetSkillCoolDownGroup[nCoolDownID + (int)(COMMONCOOLDOWN.COOLDOWN_LIST_SIZE) * nPetNum];
        return null;
    }
	//更新宠物冷却组
    public void PetSkillCoolDownGroup_UpdateList(Cooldown_T[] pCoolDownList, int nUpdateNum, PET_GUID_t nPet_GUID)
    {
        if(nUpdateNum<=0) return;
	    if(nUpdateNum > (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE) nUpdateNum = (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE;
	    int nCooldownID = MacroDefine.INVALID_ID;
	    int nFind = -1;
        for(int i = 0; i < GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++)
        {
            SDATA_PET My_Pet= CDataPool.Instance.Pet_GetPet(i);
            if( My_Pet == null || (MacroDefine.INVALID_ID == My_Pet.IsPresent) )
                continue;
    		
            if( CObjectManager.Instance.getPlayerMySelf().GetCharacterData().isFightPet(My_Pet.GUID))
            {
                nFind = i;
                break;
            }
        }
        if(nFind<0) return;
        for(int i = 0;  nUpdateNum > i; i++)
        {
            nCooldownID = pCoolDownList[i].m_nID;
            if(0> nCooldownID || (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE<=nCooldownID) continue;

            //更新冷却组
            COOLDOWN_GROUP rCooldown = m_vPetSkillCoolDownGroup[(nFind*(int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE)+nCooldownID]; 
            rCooldown.nTotalTime = pCoolDownList[i].m_nCooldown;
            rCooldown.nTimeElapsed = pCoolDownList[i].m_nCooldownElapsed;
            rCooldown.nTime = rCooldown.nTotalTime - rCooldown.nTimeElapsed;

            //刷新的ActionSystem
            CActionSystem.Instance.UpdateCoolDown(nCooldownID);
        }
    }


	//------------
	//公共冷却组
	//------------
    public int CommonCoolDown_Get() { return m_nCommonCoolDown; }
    public void CommonCoolDown_Update() { m_nCommonCoolDown = (int)COMMONCOOLDOWN.COMMONCOOLDOWN_TIME; }
	
	//------------
	//任务时间
	//------------
	//取得任务时间
    public int QuestTimeGroup_Get(int nQuestTimeID)
    {
        //Todo:
        return 0;
    }
    public void Tick(uint nTickTime)
    {
                // 待优化
	    for(int i=0; i< (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE; i++)
	    {
		    COOLDOWN_GROUP rCoolDown = m_vCoolDownGroup[i];
		    if(0<rCoolDown.nTime)
		    {
			    rCoolDown.nTime -= (int)nTickTime;
			    if(rCoolDown.nTime < 0)
			    {
				    rCoolDown.nTime = 0;
			    }
		    }
		    // 有用否？ [11/9/2011 Sun]
            //if(i < QUESTTIME_LIST_SIZE &&  m_vQuestTimeGroup[i] > 0 )
            //{
            //    m_vQuestTimeGroup[i] -= nTickTime;
            //    if( m_vQuestTimeGroup[i] <= 0 )
            //        m_vQuestTimeGroup[i] = 0;
            //}
	    }
	    //宠物冷却组更新
        for (int i = 0; i < GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++)
        {
            SDATA_PET pMyPet = CDataPool.Instance.Pet_GetPet(i);
            if (pMyPet != null && pMyPet.IsPresent != MacroDefine.INVALID_ID)
            {
                for (int j = 0; j < (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE; j++)
                {
                    COOLDOWN_GROUP rCoolDown = m_vPetSkillCoolDownGroup[i * (int)COMMONCOOLDOWN.COOLDOWN_LIST_SIZE + j];
                    if(0<rCoolDown.nTime)
                    {
                        rCoolDown.nTime -= (int)nTickTime;
                        if(rCoolDown.nTime < 0)
                        {
                            rCoolDown.nTime = 0;
                        }
                    }
                }
            }
        }
    }
	//设置任务时间
    public void QuestTimeGroup_UpdateList(int nQuestTime, int nUpdateNum){}
}