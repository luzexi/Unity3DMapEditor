using System.Collections.Generic;
using DBSystem;
public class SCLIENT_SKILLCLASS
{
    public static readonly int XINFATYPE_JINGJIEBEGIN = 0;
    public static readonly int XINFATYPE_SKILLBENGIN = 30;
    public _DBC_SKILL_XINFA		        m_pDefine;		//心法表定义
    public _DBC_XINFA_REQUIREMENTS      m_pRequire;  // 心法升级需求表 [10/18/2011 Ivan edit]
    public int m_nPosIndex;	//第几个心法 (UI同步)
    public bool m_bLeaned;		//是否已经学会
    public int m_nLevel;		//心法等级
    public bool m_bCanLevelUp;  // 心法是否可以升级了 [10/18/2011 Ivan edit]
    public bool enableLearnEvent;	// 控制是否可以广播技能可用消息 [11/15/2011 Ivan edit]
    public int nNeedMyLevel;
    public SCLIENT_SKILLCLASS()
    {
        m_nPosIndex      = 0;
        m_bLeaned        = false;
        m_nLevel         = 0;
        m_bCanLevelUp    = false;
        enableLearnEvent = false;
    }
// 不再需要刷新学习标志，默认所有发过来的技能都是已经学习过的 [2/18/2012 SUN]
//     public void UpdateLearnState()
//     {
// 	    const int UN_LEARN_LEVEL = 1;
// 	    const int UN_LEARN_INDEX = UN_LEARN_LEVEL - 1;
// 	    // 技能等级为1级的需要判断玩家是否达到使用条件 [10/27/2011 Ivan edit]
// 	    if (m_nLevel <= UN_LEARN_LEVEL )
// 	    {
// 		    m_bLeaned = false;
// 		    if (m_pRequire != null)
// 		    {
// 			    int myLevel = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level();
// 			    // 判断学会等级 [10/18/2011 Ivan edit]
// 			    if (m_pRequire.StudySpend[UN_LEARN_INDEX].needRoleLevel <= myLevel)
// 			    {
// 				    // 判断依赖心法等级 [10/18/2011 Ivan edit]
// 				    if (m_pRequire.StudySpend[UN_LEARN_INDEX].dependsSkillId != MacroDefine.INVALID_ID)
// 				    {
// 					    SCLIENT_SKILLCLASS pSkillClass =  CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(
// 																    m_pRequire.StudySpend[UN_LEARN_INDEX].dependsSkillId);
// 					    if (pSkillClass != null&& 
// 						    m_pRequire.StudySpend[UN_LEARN_INDEX].dependsSkillLevel <= pSkillClass.m_nLevel)
// 					    {
// 						    m_bLeaned = true;
// 					    }
// 				    }
// 				    else
// 				    {
// 					    m_bLeaned = true;
// 				    }
// 			    }
// 		    }
// 		    // 学会新技能的时候 [11/15/2011 Ivan edit]
// 		    if ( m_bLeaned && enableLearnEvent)
// 		    {
// 			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_SKILL, m_pDefine.nID);
// 		    }
// 	    }
// 	    else
// 	    {
// 		    m_bLeaned = true;
// 	    }
//     }
    // 是否境界心法 [4/19/2012 SUN]
    public bool IsJingJie()
    {
        if (m_pDefine == null)
            return false;
        if (m_pDefine.nID >= XINFATYPE_JINGJIEBEGIN && m_pDefine.nID < XINFATYPE_SKILLBENGIN)
            return true;
        return false;
    }
    public void UpdateLevelUpState()
    {
        m_bCanLevelUp = false;
        int myLevel = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level();

        // 索引从0开始 [10/28/2011 Ivan edit]
        nNeedMyLevel = GetNeedLevel();

        // 判断学会等级 [10/18/2011 Ivan edit]
        if (nNeedMyLevel <= myLevel)
        {
            m_bCanLevelUp = true;
        }
    }
    public int GetNeedLevel()
    {
        int nNeedLevel = m_nLevel * m_pRequire.nStepLevel;
        if (nNeedLevel < m_pDefine.nLevelLimit)
            return m_pDefine.nLevelLimit;
        return  nNeedLevel;
    }
};


//-------------------------------------------
//	1) 技能数据结构
//-------------------------------------------
public class SCLIENT_SKILL
{
	public _DBC_SKILL_DATA		        m_pDefine;		//技能表定义
	public _DBC_SKILL_DEPLETE	        m_pDeplete;	//技能的消耗
	public int							m_nPosIndex;	//第几个技能 (UI同步)
 	public byte						    m_nLevel;		//技能等级
    public bool                         m_IsActive = false;//是否已经激活
	// 不再保存是否学会技能和等级，根据心法走 [10/18/2011 Ivan edit]

	public OPERATE_RESULT IsCanUse( int idUser, int idLevel, int idTargetObj, float fTargetX, float fTargetZ, float fDir )
    {
	    OPERATE_RESULT oResult;
        oResult = IsCanUse_CheckPassive();
        if (GameDefineResult.Instance.OR_FAILED(oResult))
            return oResult;
	    oResult = IsCanUse_CheckLevel(idUser, idLevel);
	    if ( GameDefineResult.Instance.OR_FAILED( oResult ) )
		    return oResult;

	    oResult = IsCanUse_Leaned();
	    if ( GameDefineResult.Instance.OR_FAILED( oResult ) )
		    return oResult;

	    oResult = IsCanUse_CheckCoolDown();
        if (GameDefineResult.Instance.OR_FAILED(oResult))
        {
            LogManager.LogWarning("in the cd time " + m_pDefine.m_nID);
            return oResult;
        }

	    oResult = IsCanUse_CheckDeplete( idUser );
	    if ( GameDefineResult.Instance.OR_FAILED( oResult ) )
		    return oResult;

	    oResult = IsCanUse_CheckTarget( idUser, idTargetObj, fTargetX, fTargetZ, fDir );
	    if ( GameDefineResult.Instance.OR_FAILED( oResult ) )
		    return oResult;

	    oResult = IsCanUse_CheckFightState(idUser);
	    if(GameDefineResult.Instance.OR_FAILED(oResult))
		    return oResult;

	    return OPERATE_RESULT.OR_OK;
    }

	public OPERATE_RESULT IsCanUse_Leaned()
    {
        if (m_pDefine != null)
	    {
		    if (m_pDefine.m_nMenPai != MacroDefine.INVALID_ID)
		    {
			    SCLIENT_SKILLCLASS pSkillClass =  CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(
				    m_pDefine.m_nSkillClass);
			    if ( pSkillClass != null && pSkillClass.m_bLeaned)
				    return OPERATE_RESULT.OR_OK;
		    }
		    else
		    {
			    // 没有五行的技能是通用技能，可以直接使用 [10/18/2011 Ivan edit]
			    return OPERATE_RESULT.OR_OK;
		    }
	    }

	    return OPERATE_RESULT.OR_CHAR_DO_NOT_KNOW_THIS_SKILL;
    }

	public OPERATE_RESULT IsCanUse_CheckCoolDown()
    {
        int nCoolDownID = m_pDefine.m_nCooldownID;
        if (MacroDefine.INVALID_ID != nCoolDownID)
        {
            COOLDOWN_GROUP pCoolDownGroup = CDataPool.Instance.CoolDownGroup_Get(nCoolDownID);
            if (pCoolDownGroup == null || pCoolDownGroup.nTime > 0)
                return OPERATE_RESULT.OR_COOL_DOWNING;
        }
	    return OPERATE_RESULT.OR_OK;
    }

	public OPERATE_RESULT IsCanUse_CheckDeplete( int idUser )
    {
        
        CObject_Character pUser = (CObject_Character)CObjectManager.Instance.FindObject(idUser);
	    if ( pUser == null )
	    {
		   
		    return OPERATE_RESULT.OR_ERROR;
	    }

	    CCharacterData pCharData = CDataPool.Instance.CharacterData_Get( idUser );
	    if ( pCharData == null )
	    {
		    return OPERATE_RESULT.OR_ERROR;
	    }

	    // 心法要求
	    //const SCLIENT_XINFA* pXinfa = CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_XinFa(m_pDefine->m_nXinFaID);
	    //if ( pXinfa && pXinfa->m_nLevel <m_pDefine->m_nXinFaLevelRequirement )
	    //{
	    //	return OR_NEED_HIGH_LEVEL_XINFA;
	    //}

	    // 武器判断
	    if(m_pDefine.m_bMustUseWeapon !=0 )
	    {
		    if(pUser.GetCharacterData().Get_Equip(HUMAN_EQUIP.HEQUIP_WEAPON) == MacroDefine.INVALID_ID)
			    return OPERATE_RESULT.OR_NEED_A_WEAPON;
	    }

	    // 消耗判断
	    if ( m_pDeplete == null )
	    {
		    //Assert( m_pDeplete != NULL && "SCLIENT_SKILL::IsCanUse_CheckDeplete" );
		    return OPERATE_RESULT.OR_OK;
	    }
		//待实现
        CImpact_Character impactChar = pUser.Impact;
        uint nDerateAllImpactID = MacroDefine.UINT_MAX;
        if(impactChar.isExistEffect(nDerateAllImpactID))
        {
            return OPERATE_RESULT.OR_OK;
        }

	    // 是否有减免当前技能消耗的BUFF
        uint nDerateCurImpactID = MacroDefine.UINT_MAX;
        if (impactChar.isExistEffect(nDerateCurImpactID))
        {
            return OPERATE_RESULT.OR_OK;
        }
//
        if (m_pDeplete.m_nHP > 0)
        {
            // 是否有减免HP消耗的BUFF
            uint nDerateHPImpactID = MacroDefine.UINT_MAX;
            if (!impactChar.isExistEffect(nDerateHPImpactID))
            {
                if (pCharData.Get_HP() < m_pDeplete.m_nHP)
                    return OPERATE_RESULT.OR_NOT_ENOUGH_HP;
            }
        }
//
        if (m_pDeplete.m_nMP > 0)
        {
            // 是否有减免MP消耗的BUFF
            uint nDerateMPImpactID = MacroDefine.UINT_MAX;
            if (!impactChar.isExistEffect(nDerateMPImpactID))
            {
                if (pCharData.Get_MP() < m_pDeplete.m_nMP)
                    return OPERATE_RESULT.OR_LACK_MANA;
            }
        }
//
        //if (m_pDeplete.m_nSP > 0)
        //{
        //    // 是否有减免SP消耗的BUFF
        //    uint nDerateSPImpactID = MacroDefine.UINT_MAX;
        //    if (!impactChar.isExistEffect(nDerateSPImpactID))
        //    {
        //        if (pCharData.Get_Rage() < m_pDeplete.m_nSP)
        //            return OPERATE_RESULT.OR_NOT_ENOUGH_RAGE;
        //    }
        //}
//
        if (m_pDeplete.m_nStrikePoint > 0)
        {
            // 是否有减免StrikePoint消耗的BUFF
            // 这里的m_pDeplete->m_nStrikePoint为连技段
            uint nDerateStrikePointImpactID = MacroDefine.UINT_MAX;
            if (!impactChar.isExistEffect(nDerateStrikePointImpactID))
            {
                int nStrikePoint = m_pDeplete.m_nStrikePoint * 3;
                if (pCharData.Get_StrikePoint() < nStrikePoint)
                    return OPERATE_RESULT.OR_NOT_ENOUGH_STRIKE_POINT;
            }
        }
//
        if (m_pDeplete.m_nItemID != MacroDefine.INVALID_ID)
        {
            // 是否有减免Item消耗的BUFF
            uint nDerateItemImpactID = MacroDefine.UINT_MAX;
            if (!impactChar.isExistEffect(nDerateItemImpactID))
            {
                int nIndex = CDataPool.Instance.UserBag_GetItemIndexByID(m_pDeplete.m_nItemID);
                if (nIndex == -1)
                    return OPERATE_RESULT.OR_NOT_ENOUGH_ITEM;
            }
        }
	    return OPERATE_RESULT.OR_OK;
    }

	public OPERATE_RESULT IsCanUse_CheckTarget( int idUser, int idTargetObj, float fTargetX, float fTargetZ, float fDir )
    {
        switch((ENUM_SELECT_TYPE)m_pDefine.m_nSelectType)
	    {
	        case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:	
		        break;
	        case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:	
		        {
			        if(idTargetObj==MacroDefine.INVALID_ID)
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_FIGHT,"没有目标");
				        return OPERATE_RESULT.OR_NO_TARGET;
			        }
			        // m_nTargetMustInSpecialState--> 0:活的；1:死的; -1:没有要求
			        bool TargetMustAliveFlag = (m_pDefine.m_nTargetMustInSpecialState == 0);
                    
			        CObject_Character pTargetObj = (CObject_Character)CObjectManager.Instance.FindServerObject(idTargetObj);
			        if (pTargetObj == null)
			        {
				        return OPERATE_RESULT.OR_ERROR;
			        }
			        else if (pTargetObj.CannotBeAttack())// 敌对状态时，也要判断npc是否可以被攻击 [9/26/2011 Ivan edit]
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_FIGHT,"无效目标");
				        return OPERATE_RESULT.OR_INVALID_TARGET;
			        }
			        CObject_Character pThisObj = (CObject_Character)CObjectManager.Instance.FindObject(idUser);
			        if (pThisObj == null)
			        {
				        return OPERATE_RESULT.OR_ERROR;
			        }

			        //阵营判断
                    _CAMP_DATA cap_a = pThisObj.GetCampData();
                    _CAMP_DATA cap_b = pTargetObj.GetCampData();
                    ENUM_RELATION eCampType = CampStand.Instance.CalcRelationType(cap_a.m_nCampID, cap_b.m_nCampID);

			        // 通过PK模式判断是否为敌人 [8/19/2011 edit by ZL]
			        if (eCampType != ENUM_RELATION.RELATION_ENEMY) {
				        int tempRelation = CObjectManager.Instance.getPlayerMySelf().GetRelationOther(pTargetObj);
				        if ( tempRelation != -1 ) 
					        eCampType = (ENUM_RELATION)tempRelation;
			        }
                    if (m_pDefine.m_nFriendness < 0 && eCampType == ENUM_RELATION.RELATION_ENEMY)
                    { }
                    else if (m_pDefine.m_nFriendness > 0 && eCampType == ENUM_RELATION.RELATION_FRIEND)
                    { }
                    else if (m_pDefine.m_nFriendness == 0)
                    { }
                    else
                    {
                        return OPERATE_RESULT.OR_INVALID_TARGET;
                    }

			        if( !pTargetObj.IsDie()
                        && TargetMustAliveFlag)
			        {// 该技能只对活目标有效
			        }
			        else if( pTargetObj.IsDie()
                        && !TargetMustAliveFlag)
			        {// 该技能只对死目标有效
			        }
			        else
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_FIGHT,"无效目标");
				        return OPERATE_RESULT.OR_INVALID_TARGET;
			        }
		        }
		        break;
	        case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
		        {
			        if( !(fTargetX>=0.0f && fTargetZ>=0.0f) )
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_FIGHT,"无效目标");
				        return OPERATE_RESULT.OR_INVALID_TARGET_POS;
			        }
		        }
		        break;
	        case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
		        break;
	        case ENUM_SELECT_TYPE.SELECT_TYPE_SELF:		
		        break;
	    }
	    return OPERATE_RESULT.OR_OK;
    }

	public OPERATE_RESULT IsCanUse_CheckLevel( int idUser, int idLevel )
    {
        CObject_Character pUser = (CObject_Character)(CObjectManager.Instance.FindObject( idUser ));
        if (pUser == null)
        {
            return OPERATE_RESULT.OR_ERROR;
        }
        int iSkillClass = m_pDefine.m_nSkillClass;
        int iLevel = m_nLevel - 1;
        if (iLevel < 0) iLevel = 0;
        // 得到技能对应的心法数据
        SCLIENT_SKILLCLASS pSkillClass = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(iSkillClass);
        if (pSkillClass != null)
        {
            // 技能数据id
            int iSkillData_V1ID = 0;
            if (iLevel < 12)// 境界心法超过12级 [4/19/2012 SUN]
                iSkillData_V1ID = m_pDefine.m_anSkillByLevel[iLevel];
            else if (pSkillClass.IsJingJie())
                iSkillData_V1ID = m_pDefine.m_anSkillByLevel[11];
            //打开数据表
            _DBC_SKILLDATA_V1_DEPLETE pSkillData_V1 = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILLDATA_V1_DEPLETE>((int)DataBaseStruct.DBC_SKILLDATA_V1_DEPLETE).Search_Index_EQU(iSkillData_V1ID);
            //搜索纪录
            if (pSkillData_V1 != null)
            {
                if (pSkillData_V1.nNeedLevel > pUser.GetCharacterData().Get_Level())
                    return OPERATE_RESULT.OR_NEED_HIGH_LEVEL_XINFA;
            }
        }
        return OPERATE_RESULT.OR_OK;
    }

    public OPERATE_RESULT IsCanUse_CheckFightState(int idUser)
    {
        if (m_pDefine != null)
	    {
		    if (m_pDefine.m_nMenPai != MacroDefine.INVALID_ID)
		    {
			    SCLIENT_SKILLCLASS pSkillClass =  CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(
				    m_pDefine.m_nSkillClass);
			    if ( pSkillClass != null && pSkillClass.m_bLeaned)
				    return OPERATE_RESULT.OR_OK;
		    }
		    else
		    {
			    // 没有五行的技能是通用技能，可以直接使用 [10/18/2011 Ivan edit]
			    return OPERATE_RESULT.OR_OK;
		    }
	    }

	    return OPERATE_RESULT.OR_CHAR_DO_NOT_KNOW_THIS_SKILL;
    }
    public OPERATE_RESULT IsCanUse_CheckPassive()
    {
        if (m_pDefine != null)
        {
            if (m_pDefine.m_nPassiveFlag == 0)
                return OPERATE_RESULT.OR_OK;
        }
        return OPERATE_RESULT.OR_SKILL_PASSIVE_LIMIT;
    }

	public string GetSkillDesc()					// 得到技能的描述
    {
        if(m_pDefine != null)
	    {
		    // 得到技能心法id
		    int	iSkillClass    = m_pDefine.m_nSkillClass;

		    int iLevel = m_nLevel - 1;
		    if( iLevel < 0 ) iLevel = 0;

		    // 得到技能对应的心法数据
		    SCLIENT_SKILLCLASS pSkillClass =  CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(iSkillClass);
		    if(pSkillClass != null)
		    {
			    // 技能数据id
                if (iLevel > 11)
                    iLevel = 11;// 境界技能超过12级 [5/9/2012 SUN]
			    int iSkillData_V1ID = m_pDefine.m_anSkillByLevel[iLevel];
    			
			    //打开数据表
			    _DBC_SKILLDATA_V1_DEPLETE pSkillData_V1 = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILLDATA_V1_DEPLETE>((int)DataBaseStruct.DBC_SKILLDATA_V1_DEPLETE).Search_Index_EQU(iSkillData_V1ID);
			    //搜索纪录
			    if(pSkillData_V1 != null)
			    {
				    // temp fix [7/8/2010 Sun]
				    //return pSkillData_V1->paraPair[11].szDesc;
				    return pSkillData_V1.szDesc2;
			    }
		    }
		    else
		    {
			    return m_pDefine.m_pszDesc;
		    }
	    }
	    return "";
    }

	public SCLIENT_SKILL()
	{
		m_nLevel = 0;
	}
};

//-------------------------------------------
//	2) 客户端生活技能数据结构
//-------------------------------------------
public class SCLIENT_LIFEABILITY
{
    public _DBC_LIFEABILITY_DEFINE m_pDefine;		//生活技能表中的定义
    public int m_nPosIndex;	// 第几个技能 (UI同步)
    public int m_nLevel;		// 技能等级
    public int m_nExp;			// 技能熟练度
};
//typedef std::map< INT, SCLIENT_LIFEABILITY >	SLIFEABILITY_MAP;




//-------------------------------------------
//	3) 客户端配方数据结构
//-------------------------------------------
public class SCLIENT_PRESCR
{
	public  _DBC_LIFEABILITY_ITEMCOMPOSE		m_pDefine;		//在配方表中的定义
};
//typedef std::map< INT, SCLIENT_PRESCR >			SPRESCR_MAP;
// 根据生活技能索引配方 [10/8/2011 Sun]
//typedef std::map< INT, SPRESCR_MAP>				LIFEABILITY_PRESCR_MAP;