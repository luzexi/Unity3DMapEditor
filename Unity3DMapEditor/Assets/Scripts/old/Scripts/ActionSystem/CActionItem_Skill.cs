using System;
using System.Collections.Generic;
using DBSystem;

public class CActionItem_Skill : CActionItem
{
    //得到操作类型
    public override ACTION_OPTYPE GetType() { return ACTION_OPTYPE.AOT_SKILL; }
    //类型字符串
    public override ActionNameType GetType_String() { return ActionNameType.Skill; }
    //得到定义ID
    public override int GetDefineID()
    {
        SCLIENT_SKILL pSkill = GetSkillImpl();
        if (pSkill == null) return -1;

        return pSkill.m_pDefine.m_nID * 100 + pSkill.m_nLevel;
    }
    public override int GetIDTable()
    {
        SCLIENT_SKILL pSkill = GetSkillImpl();
        if (pSkill == null) return -1;

        return pSkill.m_pDefine.m_nID;
    }
    //得到数量
    public override int GetNum() { return 1; }
    //得到内部数据
    public override object GetImpl() { return (object)GetSkillImpl(); }
    //得到解释
    public override string GetDesc()
    {
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)(GetSkillImpl());
        if (pSkill != null)
        {

            return pSkill.GetSkillDesc();
        }
        return "";
    }
    // 心法特殊描述 [5/9/2012 SUN]
    public string GetDesc1()
    {
        SCLIENT_SKILL pSkill = GetSkillImpl() as SCLIENT_SKILL;
        if (pSkill != null)
        {
            SCLIENT_SKILLCLASS pXinfa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(pSkill.m_pDefine.m_nSkillClass);
            DBC.COMMON_DBC<_DBC_XINFA_SKILL_DATA> dbc = CDataBaseSystem.Instance.GetDataBase<_DBC_XINFA_SKILL_DATA>((int)DataBaseStruct.DBC_XINFA_SKILL_DATA);
            _DBC_XINFA_SKILL_DATA define = dbc.Search_Index_EQU(pXinfa.m_pDefine.nID);
            int nMenPai = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_MenPai();
            int nStepValue = define.nMenPaiValue[nMenPai] / 100;
            int curValue = pXinfa.m_nLevel * nStepValue;
            return string.Format(pSkill.GetSkillDesc(), nStepValue, curValue);
        }
        return "";
    }
    //得到冷却状组ID
    public override int GetCoolDownID()
    {
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)GetSkillImpl();
        if (pSkill == null) return -1;

        return pSkill.m_pDefine.m_nCooldownID;
    }
    //得到所在容器的索引
    //	技能			- 第几个技能
    public override int GetPosIndex()
    {
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)GetSkillImpl();
        if (pSkill != null) return -1;

        return pSkill.m_nPosIndex;
    }
    //是否能够自动继续进行
    public virtual bool AutoKeepOn()
    {
        SCLIENT_SKILL pSkillImpl = GetSkillImpl();
        if (pSkillImpl != null) return false;

        //是否能够能够自动进行
        if (pSkillImpl.m_pDefine.m_bAutoRedo)
        {
            CObjectManager.Instance.getPlayerMySelf().SetActiveSkill(this);
            return true;
        }

        return false;
    }
    //激活动作
    public override void DoAction()
    {
        SCLIENT_SKILL pSkill = (SCLIENT_SKILL)GetSkillImpl();
        if (pSkill == null) return;

        OPERATE_RESULT oResult;
        // 
        oResult = pSkill.IsCanUse_Leaned();
        if (GameDefineResult.Instance.OR_FAILED(oResult))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, GameDefineResult.Instance.GetOResultText(oResult));
            return;
        }
        ////检查冷却是否结束
        oResult = pSkill.IsCanUse_CheckCoolDown();
        if (GameDefineResult.Instance.OR_FAILED(oResult))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, GameDefineResult.Instance.GetOResultText(oResult));
            return;
        }

        

        // 消耗检测
        int idUser;
        CObject_PlayerMySelf pMySelf = CObjectManager.Instance.getPlayerMySelf();
        idUser = (pMySelf != null) ? (pMySelf.ID) : (MacroDefine.INVALID_ID);
        oResult = pSkill.IsCanUse_CheckDeplete(idUser);
        if (GameDefineResult.Instance.OR_FAILED(oResult))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, GameDefineResult.Instance.GetOResultText(oResult));
            return;
        }

        oResult = pSkill.IsCanUse_CheckLevel(idUser, pSkill.m_nLevel);
        if (GameDefineResult.Instance.OR_FAILED(oResult))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, GameDefineResult.Instance.GetOResultText(oResult));
            return;
        }

        //如果是连续的那么就换为选中状态。
        AutoKeepOn();
        //根据操作类型
        switch ((ENUM_SELECT_TYPE)pSkill.m_pDefine.m_nSelectType)
        {
            case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
                {
                    CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(GetDefineID());
                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
                {
                    //取得技能数据
                    int idMyself = CObjectManager.Instance.getPlayerMySelf().ServerID;

                    if (CObjectManager.Instance.GetMainTarget() != null)
                    {
                        int idTarget = CObjectManager.Instance.GetMainTarget().ServerID;
                        // 陈治要求使用增益型技能的时候，选中为敌人的情况下，强制改为自己 [11/14/2011 Ivan edit]
                        if (pSkill.m_pDefine.m_nFriendness >= 0 &&
                            idTarget != idMyself)// 减少判断
                        {
                            CObject_Character pTargetObj = (CObject_Character)CObjectManager.Instance.FindServerObject(idTarget);
                            //阵营判断
                            ENUM_RELATION eCampType = pTargetObj.GetCampType(CObjectManager.Instance.getPlayerMySelf());

                            // 通过PK模式判断是否为敌人 [8/19/2011 edit by ZL]
                            if (eCampType != ENUM_RELATION.RELATION_ENEMY)
                            {
                                int tempRelation = CObjectManager.Instance.getPlayerMySelf().GetRelationOther(pTargetObj);
                                eCampType = (ENUM_RELATION)tempRelation;
                            }
                            // 增益型技能:目标不是友好对象的情况下必须强制对自己使用 [11/14/2011 Ivan edit]
                            if (eCampType != ENUM_RELATION.RELATION_FRIEND)
                            {
                                idTarget = idMyself;
                            }
                        }
                        CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(GetDefineID(), idTarget);
                    }
                    else
                    {
                        // 陈治要求如果增益型技能没有选中对象，则默认目标为自己 [11/14/2011 Ivan edit]
                        if (pSkill.m_pDefine.m_nFriendness >= 0)
                        {
                            CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(GetDefineID(), idMyself);
                        }
                        else
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, GameDefineResult.Instance.GetOResultText(OPERATE_RESULT.OR_NO_TARGET));
                            CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(GetDefineID(), (uint)idMyself);
                        }

                    }
                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
                CActionSystem.Instance.SetDefaultAction(this);
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
            	CActionSystem.Instance.SetDefaultAction(this);    
			//CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                //		CGameProcedure::s_pGameInterface->Skill_SetActive(this);
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_SELF:
                {
                    CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(GetDefineID(), CObjectManager.Instance.getPlayerMySelf().ServerID);
                    //恢复激活Action
                    //  CActionSystem.Instance.SetDefaultAction(GameProcedure.Skill_GetActive());
                }
                break;
            case ENUM_SELECT_TYPE.SELECT_TYPE_HUMAN_GUID:
                {
                    //待实现
                    //当前是否已经选择了一个队友
                    //uint guid;
                    //CObjectManager::GetMe()->GetMainTarget(guid);
                    //if(guid == (GUID_t)-1)
                    //{
                    //    //尚未选择合法的对象
                    //    CEventSystem::GetMe()->PushEvent(GE_INFO_SELF, "无效目标");
                    //    return;
                    //}

                    //CGameProcedure::s_pGameInterface->Player_UseSkill(GetID(), (GUID_t)guid);
                    ////恢复激活Action
                    //CActionSystem::GetMe()->SetDefaultAction(CGameProcedure::s_pGameInterface->Skill_GetActive());
                }
                break;
        }
    }
    //是否有效
    public override bool IsValidate() { return true; }
    //检查冷却是否结束
    public override bool CoolDownIsOver()
    {
        int nCoolDownID = GetCoolDownID();

        //对于没有冷却的Action
        if (MacroDefine.INVALID_ID == nCoolDownID) return true;
        //取得冷却组数据
        COOLDOWN_GROUP pCoolDownGroup = CDataPool.Instance.CoolDownGroup_Get(nCoolDownID);

        if (pCoolDownGroup != null && pCoolDownGroup.nTime <= 0 && CDataPool.Instance.CommonCoolDown_Get() <= 0)
            return true;
        else
            return false;
    }

    // 得到技能等级
    public virtual int GetSkillLevel()
    {
        SCLIENT_SKILL pImpl = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Skill(m_idSkillImpl);
        if (pImpl != null)
        {
            //  [11/4/2010 ivan edit]
            //return pImpl->m_pDefine->m_nLevelRequirement;
            //return pImpl->m_nLevel;

            // 没有技能等级，只有心法等级 [10/13/2011 Ivan edit]
            SCLIENT_SKILLCLASS pClass = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(pImpl.m_pDefine.m_nSkillClass);
            if (pClass != null)
            {
                return pClass.m_nLevel;
            }
        }
        return 0;
    }

    // 得到是否学习了技能
    public virtual bool GetIsLearnedSkill()
    {
        SCLIENT_SKILL pImpl = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Skill(m_idSkillImpl);
        if (pImpl != null)
        {
            return (pImpl.IsCanUse_Leaned() == OPERATE_RESULT.OR_OK);
        }
        return false;
    }

    // 得到类型描述
    public virtual string GetTypeDesc()
    {
        return "#{ActionType_Skill}";
    }

    // 得到技能等级
    public int GetSkillXinfaLevel()
    {
        SCLIENT_SKILL impl = GetSkillImpl();
        if (impl == null) return -1;

        SCLIENT_SKILLCLASS pXinfa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(impl.m_pDefine.m_nSkillClass);
        if (pXinfa != null)
        {
            return pXinfa.m_nLevel;

        }
        return -1;
    }
    public int GetXinfaID()
    {
        SCLIENT_SKILL impl = GetSkillImpl();
        if (impl == null) return -1;
        SCLIENT_SKILLCLASS pXinfa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(impl.m_pDefine.m_nSkillClass);
        if (pXinfa != null)
        {
            return pXinfa.m_pDefine.nID;

        }
        return -1;
    }
    public bool IsJingJieSkill()
    {
        SCLIENT_SKILL impl = GetSkillImpl();
        if (impl == null) return false;
        SCLIENT_SKILLCLASS pXinfa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(impl.m_pDefine.m_nSkillClass);
        if (pXinfa != null && pXinfa.IsJingJie())
        {
            return true;

        }
        return false;
    }
    public string GetUpLevelDesc()
    {
        string strDesc = null;
        SCLIENT_SKILL impl = GetSkillImpl();
        if (impl == null) return strDesc;
        SCLIENT_SKILLCLASS pXinfa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(impl.m_pDefine.m_nSkillClass);
        if (pXinfa == null) return strDesc;

        int needLevel = pXinfa.GetNeedLevel();
        if (needLevel > CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level())
            strDesc = "#R人物等级 " + needLevel;
        else
            strDesc = "人物等级 " + "#G" + needLevel;

        Spend spend = CDataPool.Instance._StudySkill.GetUplevelXinfaSpend(pXinfa.m_pDefine.nID, pXinfa.m_nLevel + 1);
        int myExp = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Exp();
        if (spend.dwSpendExperience > myExp)
        {
            strDesc += "\n" + "#R历练值 " + spend.dwSpendExperience;
        }
        else
        {
            strDesc += "\n" + "#W历练值 " + "#G" + spend.dwSpendExperience;
        }

        return strDesc;

    }


    public CActionItem_Skill(int nID)
        : base(nID)
    {

    }
    //根据技能跟新
    public void Update_Skill(SCLIENT_SKILL pSkill)
    {
        m_idSkillImpl = pSkill.m_pDefine.m_nID * 100 + pSkill.m_nLevel;
        //名称
        m_strName = pSkill.m_pDefine.m_lpszName;
        //图标
        m_strIconName = pSkill.m_pDefine.m_lpszIconName;

        bool bOldEnable = IsEnabled();
        bool bNewEnable = false;
        OPERATE_RESULT oResult = pSkill.IsCanUse_CheckDeplete(CObjectManager.Instance.getPlayerMySelf().ID);
        if (GameDefineResult.Instance.OR_SUCCEEDED(oResult))
        {
            oResult = pSkill.IsCanUse_Leaned();
            if (GameDefineResult.Instance.OR_SUCCEEDED(oResult))
            {
                bNewEnable = true;
            }
        }

        if (bNewEnable != bOldEnable)
        {
            if (bNewEnable)
            {
                Enable();
            }
            else
            {
                Disable();
            }
            //通知UI
            UpdateToRefrence();
        }
    }
    public override void NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName)
    {
        if (bDestory)
        {
            char cSourceType = szSourceName[0];
            switch (cSourceType)
            {
                case 'M':		//主菜单
                    {
                        int nIndex = szSourceName[1] - '0';
                        nIndex = szSourceName[2] - '0' + nIndex * 10;
                        CActionSystem.Instance.MainMenuBar_Set(nIndex, -1 );
                    }
                    break;
                default:
                    break;
            }
        }

        //拖动到空白地方
        if (szTargetName == null || szTargetName == "" || szTargetName[0] == '\0') return;

        // 陈治文档要求只要技能学会了就可以拖拽，不管是否有使用限制(如必须装备武器等) [11/7/2011 Ivan edit]
        /*if(!IsEnabled()) return;*/

        char cSourceName = szSourceName[0];
        char cTargetType = szTargetName[0];

        //如果不是拖到快捷栏，返回
        if (cSourceName == 'M' && cTargetType != 'M')
            return;

        int nOldTargetId = -1;

        switch (cTargetType)
        {
            case 'M':		//主菜单
                {
                    int nIndex = szTargetName[1] - '0';
                    nIndex = szTargetName[2] - '0' + nIndex * 10;
                    //查询目标位置原来的内容
                    //nOldTargetId = CActionSystem::GetMe()->MainMenuBar_Get(nIndex);
                    //CActionSystem::GetMe()->MainMenuBar_Set(nIndex, GetID() );
                    nOldTargetId = CActionSystem.Instance.MainMenuBar_Get(nIndex);
                    CActionSystem.Instance.MainMenuBar_Set(nIndex, GetID());


                }
                break;

            default:
                break;
        }

        switch (cSourceName)
        {
            case 'M':
                {
                    int nIndex = szSourceName[1] - '0';
                    nIndex = szSourceName[2] - '0' + nIndex * 10;
                    CActionSystem.Instance.MainMenuBar_Set(nIndex, nOldTargetId);
                }
                break;
            default:
                break;
        }
        //	CEventSystem::GetMe()->PushEvent(GE_UPDATE_SUPERTOOLTIP);
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, "0");
    }

    protected SCLIENT_SKILL GetSkillImpl()
    {
        return CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Skill(m_idSkillImpl);
    }

    //得到技能数据
    //用于技能操作时，技能数据
    protected int m_idSkillImpl;

    // 获得玩家想要升级技能所需的人物等级 [3/29/2012 Ivan]
    internal int GetNextRoleLevel()
    {
        SCLIENT_SKILL impl = GetSkillImpl();
        SCLIENT_SKILLCLASS pXinfa = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(impl.m_pDefine.m_nSkillClass);
        if (pXinfa != null)
        {
            return pXinfa.nNeedMyLevel;
        }

        return -1;
    }
};