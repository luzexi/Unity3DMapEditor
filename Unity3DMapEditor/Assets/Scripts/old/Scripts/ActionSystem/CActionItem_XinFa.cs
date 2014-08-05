/****************************************\
*										*
* 			  操作管理器-心法			*
*										*
\****************************************/


public class CActionItem_XinFa : CActionItem
{
	//得到操作类型
	public override ACTION_OPTYPE	GetType() { return ACTION_OPTYPE.AOT_XINFA;	}
	//类型字符串
    public override ActionNameType GetType_String() { return ActionNameType.Xinfa; }
	//得到定义ID
	public override int				GetDefineID()
    {
        SCLIENT_SKILLCLASS pSkillClass = GetSkillClassImpl();
	    if(pSkillClass== null) return -1;
	
	    return pSkillClass.m_pDefine.nID;
    }
	//得到数量
	public override int				GetNum()			{ return 1;	}
	//得到内部数据
    public override object GetImpl() { return (object)GetSkillClassImpl(); }
	//得到解释
	public override string			GetDesc()
    {
        SCLIENT_SKILLCLASS pSkillClass = GetSkillClassImpl();
        if(pSkillClass== null) return "ERROR";

	    return pSkillClass.m_pDefine.pszDesc;
    }
	//得到冷却状组ID
	public override int				GetCoolDownID()		{ return -1; }
	//得到所在容器的索引
	//	技能			- 第几个技能
	public override int				GetPosIndex()
    {
        SCLIENT_SKILLCLASS pSkillClass = GetSkillClassImpl();
	    if(pSkillClass== null) return -1;

	    return pSkillClass.m_nPosIndex;
    }
	//是否能够自动继续进行
	public virtual bool			AutoKeepOn()		{ return false; }
	//激活动作
	public override void			DoAction()			{ }
	//是否有效
	public override bool			IsValidate() { return true; }
	//刷新
	public virtual  void			Update(tActionReference pRef) { }
	//检查冷却是否结束
	public override bool			CoolDownIsOver() { return true; }
	//拖动结束
	public override void			NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName)
    {

    }
	// 得到心法等级
	public virtual int				GetXinfaLevel()
    {
        SCLIENT_SKILLCLASS pXinfa = (SCLIENT_SKILLCLASS)GetSkillClassImpl();
	    if(pXinfa == null) 
	    {
		    return 0;
	    };

	    return pXinfa.m_nLevel;
    }

    public CActionItem_XinFa(int nID):base(nID)
    {
    }
	
	//更新数据
	public void	Update_SkillClass(SCLIENT_SKILLCLASS pXinfaDef)
    {
        m_idSkillClass = pXinfaDef.m_pDefine.nID;
	    //名称
	    m_strName = pXinfaDef.m_pDefine.pszName;
	    //图标
	    m_strIconName = pXinfaDef.m_pDefine.pszIconName;

	    //通知UI
	    UpdateToRefrence();
    }


	//返回技能系
	protected SCLIENT_SKILLCLASS	GetSkillClassImpl()
    {
        return CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass(m_idSkillClass);
    }


	//心法ID
	protected int		m_idSkillClass;

};