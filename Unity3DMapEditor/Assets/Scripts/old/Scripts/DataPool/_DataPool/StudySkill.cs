using DBSystem;
using Network;
using Network.Packets;

public class StudySkill
{
    //心法升级消耗表
    private DBC.COMMON_DBC<_DBC_XINFA_STUDYSPEND> xinfaStudySpendDBC;
    public DBC.COMMON_DBC<_DBC_XINFA_STUDYSPEND> XinfaStudySpendDBC
    {
        get {
            if(xinfaStudySpendDBC == null)
                xinfaStudySpendDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_XINFA_STUDYSPEND>((int)DataBaseStruct.DBC_XINFA_STUDYSPEND);
            return xinfaStudySpendDBC;
        }
    }
    //心法升级需求
    private DBC.COMMON_DBC<_DBC_XINFA_REQUIREMENTS> xinfaRequirementDBC;
    public DBC.COMMON_DBC<_DBC_XINFA_REQUIREMENTS> XinfaRequirementDBC
    {
        get {
            if (xinfaRequirementDBC == null)
                xinfaRequirementDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_XINFA_REQUIREMENTS>((int)DataBaseStruct.DBC_XINFA_REQUIREMENTS);
            return xinfaRequirementDBC;
        }
    }
    //心法CD清零需求
    private DBC.COMMON_DBC<_DBC_XINFA_RESETCD_SPEND> xinfaResetCDSpendDBC;
    public DBC.COMMON_DBC<_DBC_XINFA_RESETCD_SPEND> XinfaResetCDSpendDBC
    {
        get {
            if(xinfaResetCDSpendDBC == null)
                xinfaResetCDSpendDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_XINFA_RESETCD_SPEND>((int)DataBaseStruct.DBC_XINFA_RESETCD_SPEND);
            return xinfaResetCDSpendDBC;
        }
    }
    //导出心法升级消耗,传入的参数为要学习的心法等级
    public Spend GetUplevelXinfaSpend(int nXinfaId, int nXinfaLevel)
    {
        _DBC_XINFA_STUDYSPEND pSpend = XinfaStudySpendDBC.Search_Index_EQU(nXinfaLevel);
	    if (pSpend != null)
            return pSpend.StudySpend[nXinfaId];

	    return null;
    }
    //心法升级需求
    public bool isUpLevelXinfaRequirement(int nID, int nLevel)
    {
        _DBC_XINFA_REQUIREMENTS require = XinfaRequirementDBC.Search_Index_EQU(nID);
        SCLIENT_SKILLCLASS pSkillClass = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(nID);
        if (pSkillClass == null)
            return false;
        return pSkillClass.m_bCanLevelUp;
    }
    //心法CD重置消耗
    public int GetUpLevelXinfaResetCDSpend(int nID)
    {
        int nMillisceond = GetUpLevelXinfaCDTime();
        int nMin = nMillisceond / (1000 * 60) + nMillisceond % (1000 * 60) > 0 ? 1 : 0; 
        _DBC_XINFA_RESETCD_SPEND spend = XinfaResetCDSpendDBC.Search_Index_EQU(nID);
        if(spend != null)
            return spend.nMoney * nMin;
        return 0;
    }

    //------------					   
	//学习（升级）门派心法相关 五行属性
	//------------
    public void	SendStudySkillEvent(int nSkillIDandLEVEL, int NPCID, int HaveUPLevelSkill)
    {
        int nXinfaID = nSkillIDandLEVEL;
	    SCLIENT_SKILLCLASS pXinFa= CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(nXinfaID);
	    int nXinFaLevel = pXinFa.m_nLevel;
	    int nSelfLevel = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level();
	    CObject_Character pMySelf = CObjectManager.Instance.getPlayerMySelf();
       
        // 判断人物等级是否达到
        if (!isUpLevelXinfaRequirement(nXinfaID, nXinFaLevel + HaveUPLevelSkill))
	    {
		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"Role levle Low");
		    return;
	    }

        //// 获得升级心法需要的金钱
        Spend needSpend = GetUplevelXinfaSpend(nXinfaID, nXinFaLevel + HaveUPLevelSkill);

        // 获得升级心法需要的经验 [10/13/2011 Ivan edit]
        int myExp = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Exp();
        if (needSpend.dwSpendExperience > myExp)
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "Not enough Exp");
            return ;
        }

        ////使用客户端的数据来判断升级的金钱和经验是否够升级
        int nCurMoney = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money();
        if (needSpend.dwSpendMoney != -1)
        {
            if (needSpend.dwSpendMoney > nCurMoney)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "Not enough money");
                //ADDTALKMSG("升级需要的金钱不足");
                return ;
            }
        }

        if (GetUpLevelXinfaCDTime() > 0)
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "CD Time");
            return;
        }
	    
        _STUDYXINFA_INFO studyInfo;
	     
	     CGAskStudyXinfa msg = (CGAskStudyXinfa)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_ASKSTUDYXINFA);
	     studyInfo.m_idXinfa	= (short)nXinfaID;			//技能ID
	     studyInfo.m_NowLevel	= (short)nXinFaLevel;		//技能等级
	     studyInfo.m_idMenpai	= (short)NPCID;		//师父ID
         msg.UplevelInfo = studyInfo;

	     NetManager.GetNetManager().SendPacket( msg );
    }
    // 重置CD [2/17/2012 SUN]
    public void ResetXinfaCDTime(int nID)
    {
        SCLIENT_SKILLCLASS pXinFa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(nID);
        if(pXinFa == null)
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "No xinfa");
            return;
        }
        //不需重置

        if(GetUpLevelXinfaCDTime() == 0)
        {
            return;
        }
        int resetSpend = GetUpLevelXinfaResetCDSpend(nID);
        if (resetSpend != -1)
        {
            int nMoney = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money();
            if (resetSpend > nMoney)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "Not enough money");
                return;
            }
        }


        CGAskClearCDTime msg = (CGAskClearCDTime)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_ASKCLEARCDTIME);
        msg.CoolDownID = (short)nID;

        NetManager.GetNetManager().SendPacket(msg);
    }
    //心法升级是否在CD中,
    public int GetUpLevelXinfaCDTime()
    {
        //心法cd的id是固定的
        COOLDOWN_GROUP cdGroup = CDataPool.Instance.CoolDownGroup_Get((int)CDGROUPID.GROUPID_XINFA_UOLEVEL);
        if (cdGroup == null)
            return 0;
        return cdGroup.nTime;
    }
    //
    public bool isCanUplevel_deplete(int nID, int nLevel, out string error)
    {
        error = null;
        //// 获得升级心法需要的金钱
        Spend needSpend = GetUplevelXinfaSpend(nID, nLevel /*nXinFaLevel + HaveUPLevelSkill*/);

        // 获得升级心法需要的经验 [10/13/2011 Ivan edit]
        int myExp = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Exp();
        if (needSpend.dwSpendExperience > myExp)
        {
            //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "Not enough Exp");
            error = "#{Exp_Not_Enough}";
            return false;
        }

        ////使用客户端的数据来判断升级的金钱和经验是否够升级
        int nCurMoney = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money();
        if (needSpend.dwSpendMoney != -1)
        {
            if (needSpend.dwSpendMoney > nCurMoney)
            {
                error = "#{Money_Not_Enough}";
                return false;
            }
        }
        return true;
    }
    public bool isCanUplevel(int nID, int nLevel, out string error)
    {
        if (!isUpLevelXinfaRequirement(nID, nLevel))
        {
            error = "#{LowLevel}";
            return false;
        }

        if ( !isCanUplevel_deplete(nID, nLevel, out error))
            return false;
        return true;
    }
    //INT						MStudySkill(INT nSkillID,INT nSkillLEVEL);

    ////zzh+ 心法的学习
    //VOID StudyXinfa_SetXinfaTeacherId(INT nXinfaTeacherId);

    //////zzh+ 心法的学习
    //VOID StudyXinfa_SetMenpaiInfo(INT idMenpai);

    ////------------
    ////生活技能教师
    ////------------
    //INT							StudyAbility_GetID(VOID) { return m_nStudyAbility_ID; }
    //VOID						StudyAbility_SetID(INT nAbilityID) { m_nStudyAbility_ID = nAbilityID; }

    //INT							StudyAbility_GetNeedMoney(VOID) { return m_uStudyAbility_NeedMoney; }
    //VOID						StudyAbility_SetNeedMoney(INT uNeedMoney) { m_uStudyAbility_NeedMoney = uNeedMoney; }

    //INT							StudyAbility_GetNeedExp(VOID) { return m_uStudyAbility_NeedExp; }
    //VOID						StudyAbility_SetNeedExp(INT uNeedExp) { m_uStudyAbility_NeedExp = uNeedExp; }

    //INT							StudyAbility_GetSkillExp(VOID) { return m_uStudyAbility_SkillExp; }
    //VOID						StudyAbility_SetSkillExp(INT uSkillExp) { m_uStudyAbility_SkillExp = uSkillExp; }

    //INT							StudyAbility_GetLevelLimit(VOID) { return m_nStudyAbility_LevelLimit;}
    //VOID						StudyAbility_SetLevelLimit(INT nStudyAbility_LevelLimit) { m_nStudyAbility_LevelLimit = nStudyAbility_LevelLimit;}

    //INT							StudyAbility_GetScriptId(VOID) { return m_uStudyAbility_ScriptId;}
    //VOID						StudyAbility_SetScriptId(INT uStudyAbility_ScriptId) { m_uStudyAbility_ScriptId = uStudyAbility_ScriptId;}

    //INT							StudyAbility_GetNpcId(VOID) { return m_StudyAbility_NpcId;}
    //VOID						StudyAbility_SetNpcId(INT StudyAbility_NpcId) { m_StudyAbility_NpcId = StudyAbility_NpcId;}
}