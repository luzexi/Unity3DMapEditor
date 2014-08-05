using System;
using DBSystem;

public class CActionItem_PetSkill : CActionItem
{
	//得到操作类型
    public override ACTION_OPTYPE GetType() { return ACTION_OPTYPE.AOT_PET_SKILL; }
	//类型字符串
	public override ActionNameType	GetType_String()	{ return ActionNameType.PetSkill;	}
	//得到定义ID
	public override int		GetDefineID()
    {
        PET_SKILL pSkill = GetPetSkillImpl() as PET_SKILL;	
	    if(pSkill == null) return -1;

	    return pSkill.m_pDefine.m_nID;
    }
	//得到数量
	public override int		GetNum()			{ return 1;	}
    public int GetPetIndex() { return m_nPetNum; }
	//得到内部数据
	public override object	GetImpl()			{ return GetPetSkillImpl();	}
	//得到解释
	public override string			GetDesc()
    {
        PET_SKILL pSkill = GetPetSkillImpl();

	    if(pSkill != null)
	    {
		    if(pSkill.m_pDefine!=null)
		    {
			    // 得到技能对应的心法数据
			    int iPetLevel = GetMyPetLevel();
			    if(iPetLevel>=0)
			    {
				    // 技能数据id
				    int iSkillData_V1ID = pSkill.m_pDefine.m_anSkillByLevel[0];
    				
				    //搜索纪录
				    _DBC_SKILLDATA_V1_DEPLETE pSkillData_V1 = 
                        CDataBaseSystem.Instance.GetDataBase<_DBC_SKILLDATA_V1_DEPLETE>((int)DataBaseStruct.DBC_SKILLDATA_V1_DEPLETE).Search_Index_EQU(iSkillData_V1ID);
    			
				    if(pSkillData_V1 != null)
					    return pSkillData_V1.szDesc2;
			    }
		    }
		    return "ERROR";
	    }
	    else
		    return "ERROR";//
    }
	//得到冷却状组ID
	public override int				GetCoolDownID()
    {
        PET_SKILL pSkill = GetPetSkillImpl();
	    if(pSkill == null) return -1;

	    return pSkill.m_pDefine.m_nCooldownID;
    }
	//得到所在容器的索引
	//	技能			- 第几个技能
	public override int				GetPosIndex()
    {
       // PET_SKILL pSkill = GetPetSkillImpl();
	   // if(pSkill == null) return -1;

        return m_idPetSkillImpl;
    }
	//是否能够自动继续进行
	public  bool			AutoKeepOn() { return false; }
	//激活动作
	public override void 			DoAction()
    {
        //检查冷却是否结束
	    if(!CoolDownIsOver()) return;

	    PET_SKILL pSkill = GetPetSkillImpl();
	    if(pSkill==null) return;

	    SDATA_PET pThisOwner = CDataPool.Instance.Pet_GetPet(m_nPetNum);

        if (!CObjectManager.Instance.getPlayerMySelf().GetCharacterData().isFightPet(pThisOwner.GUID))
	    {
		    GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, "你不能使用未放出宠物的技能");	
		    return;
	    }
        
	    if(GetPetSkillImpl() != null && GetPetSkillImpl().m_pDefine != null)
	    {
		    //POINT pt = CGameProcedure::s_pInputSystem->MouseGetPos();
		    //fVector3 fvMouseHitPlan;
		    //CObjectManager::GetMe()->GetMouseOverObject(pt.x, pt.y, fvMouseHitPlan);

		    CObject pTargetObj = CObjectManager.Instance.GetMainTarget();

		    //根据操作类型
		    switch((ENUM_SELECT_TYPE)GetPetSkillImpl().m_pDefine.m_nSelectType)
		    {
			    case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:	
			    case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:	
				    {
                        CDataPool.Instance.Pet_SendUseSkillMessage(GetPetIndex(),(short)GetPetSkillImpl().m_pDefine.m_nID, 
                            ((pTargetObj != null)?(pTargetObj.ServerID):(MacroDefine.INVALID_ID)),-1, -1);
                        //恢复激活Action
                        CActionSystem.Instance.SetDefaultAction(GameProcedure.s_pGameInterface.Skill_GetActive());
				    }
				    break;

			    case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
				    {
					    CActionSystem.Instance.SetDefaultAction(this);
				    }
				    break;
			    case ENUM_SELECT_TYPE.SELECT_TYPE_SELF:
			    default:
				    {
					    throw new Exception("Invalid pet skill select type:"+ GetPetSkillImpl().m_pDefine.m_nSelectType);
				    }
				    break;
		    }
	    }
    }
	//是否有效
	public override bool			IsValidate() { return true; }
	//检查冷却是否结束
	public override bool			CoolDownIsOver()
    {
        int nCoolDownID = GetCoolDownID();

	    //对于没有冷却的Action
	    if(MacroDefine.INVALID_ID == nCoolDownID) return true;
	    //取得冷却组数据
	    COOLDOWN_GROUP pCoolDownGroup =
		    CDataPool.Instance.PetSkillCoolDownGroup_Get(nCoolDownID,m_nPetNum);

	    if(pCoolDownGroup != null && pCoolDownGroup.nTime <= 0) 
		    return true;
	    else 
		    return false;
    }
	//拖动结束
	public override void	NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName)
    {
        if( bDestory )
	    {
		    char cSourceType = szSourceName[0];
		    switch(cSourceType)
		    {
		    case 'M':		//主菜单
			    {
				    int nIndex = szSourceName[1]-'0';
				    nIndex = szSourceName[2]-'0' + nIndex*10 ;
				    CActionSystem.Instance.MainMenuBar_Set(nIndex, -1);
			    }
			    break;
		    default:
			    break;
		    }
	    }

	    //拖动到空白地方
	    if(szTargetName == null || szTargetName[0]=='\0') return;
    	
	    char cSourceName = szSourceName[0];
	    char cTargetType = szTargetName[0];

	    //如果不是拖到快捷栏，返回
	    if( cSourceName == 'M' && cTargetType != 'M' )
		    return;

	    int nOldTargetId = -1;
	    int nPet_Num = -1;

	    PET_SKILL pPetSkill = GetPetSkillImpl();

	    switch(cTargetType)
	    {
	        case 'M':		//主菜单
		    {
			    //如果不是手动的技能，不能放如快捷栏
			    if(pPetSkill.m_pDefine.m_nOperateModeForPetSkill != (int)PET_SKILL_OPERATEMODE.PET_SKILL_OPERATE_NEEDOWNER)
				    break;
    ///////////////////////////////////////////////////
			    //要是以后策划改主意了，就把这段注释掉。
                nPet_Num = GetPetIndex();//modified by ss 将getnum 接口换成 getpetindex
			    if(nPet_Num >= 0 && nPet_Num < GAMEDEFINE.HUMAN_PET_MAX_COUNT )
			    {
				    SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPet_Num);
				    //如果不是放出的宠物，就不要放在快捷
                    if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().isFightPet(My_Pet.GUID))
					    break;
			    }
    ///////////////////////////////////////////////////
			    int nIndex = szTargetName[1]-'0';
			    nIndex = szTargetName[2]-'0' + nIndex*10 ;
			    //查询目标位置原来的内容
			    nOldTargetId = CActionSystem.Instance.MainMenuBar_Get(nIndex);
			    CActionSystem.Instance.MainMenuBar_Set(nIndex, GetID() );
		    }
		    break;

	        default:
		        break;
	    }

	    switch(cSourceName)
	    {
	        case 'M':
		    {
			    int nIndex = szSourceName[1]-'0';
			    nIndex = szSourceName[2]-'0' + nIndex*10 ;
			    CActionSystem.Instance.MainMenuBar_Set(nIndex, nOldTargetId);
		    }
		        break;
	        default:
		        break;
	    }
	    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, "0");
    }

	public override void	UpdateCoolDown()
    {
       
	    int nCoolDownID = GetCoolDownID();

	    //对于没有冷却的Action
	    if(-1 == nCoolDownID)
	    {
		    foreach(tActionReference tRef in m_setReference)
		    {
			    tRef.EnterCoolDown(-1, 0.0f);
		    }
		    return;
	    }

	    //取得冷却组数据
	    COOLDOWN_GROUP pCoolDownGroup =
			    CDataPool.Instance.PetSkillCoolDownGroup_Get(nCoolDownID,m_nPetNum);

	    int nTimeNow   = pCoolDownGroup.nTime;
	    int nTimeTotal = pCoolDownGroup.nTotalTime;

	    foreach(tActionReference tRef in m_setReference)
	    {
		    if(nTimeNow <= 0 || nTimeTotal <= 0)
		    {
			    tRef.EnterCoolDown(-1, 0.0f);
		    }
		    else
		    {
			    tRef.EnterCoolDown(nTimeNow, 1.0f - (float)nTimeNow/(float)nTimeTotal);
		    }
	    }
    }

	public CActionItem_PetSkill(int nID)
        :base(nID)
    {

    }

	//根据技能跟新
	public void	Update_PetSkill(PET_SKILL pPetSkill)
    {
        if(pPetSkill == null)
            throw new NullReferenceException("petskill is null in Update_PetSkill()");

	    //引用

	    m_idPetSkillImpl =	pPetSkill.m_nPosIndex;
	    //此技能是属于第几只宠物的。
	    m_nPetNum = pPetSkill.m_nPetNum;

    //	m_idPetSkillImpl =	pPetSkill->m_pDefine->m_nID;
	    //名称
	    m_strName = pPetSkill.m_pDefine.m_lpszName;
	    //图标
	    m_strIconName = pPetSkill.m_pDefine.m_lpszIconName;

    //	SetCheckState(pPetSkill->m_bCanUse);//此设计为2005年邱果口述，无文档
    	
	    SetCheckState(false);//根据BUG 4396杨婷所提而更改

	    //通知UI
	    UpdateToRefrence();
    }


	//得到技能数据
	PET_SKILL	GetPetSkillImpl()
    {
        if(m_nPetNum == PET_SKILL.MENPAI_PETSKILLSTUDY_PETNUM)
		    return CDataPool.Instance.PetSkillStudy_MenPaiList_Get(m_idPetSkillImpl);
	    else if(m_nPetNum == (int)PET_INDEX.TARGETPET_INDEX)
		    return CDataPool.Instance.TargetPet_GetSkill(m_idPetSkillImpl);
	    else
		    return CDataPool.Instance.Pet_GetSkill(m_nPetNum,m_idPetSkillImpl);
    }

	public int	GetMyPetLevel()
    {
        if(m_nPetNum == (int)PET_INDEX.TARGETPET_INDEX)
	    {
		    SDATA_PET MyPet = CDataPool.Instance.TargetPet_GetPet();

		    return MyPet.Level;
	    }
	    else if(m_nPetNum == PET_SKILL.MENPAI_PETSKILLSTUDY_PETNUM)
		    return -1;
	    else
	    {
		    SDATA_PET MyPet = CDataPool.Instance.Pet_GetPet(m_nPetNum);
		    return MyPet.Level;
	    }
    }
    // 宠物技能类型,0:物功,1:法功,2:护主,3:防御,4:复仇;
    public int GetPetSkillType()
    {
        PET_SKILL pSkill = GetPetSkillImpl();
	    if(pSkill != null && pSkill.m_pDefine!=null)
	    {
            return pSkill.m_pDefine.m_nTypeOfPetSkill;
        }
        return -1;
    }


	//用于技能操作时，技能数据
	int		m_idPetSkillImpl = -1;
	int		m_nPetNum;

};