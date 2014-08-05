/****************************************\
*										*
* 		  操作管理器-生活技能			*
*										*
\****************************************/

public  class CActionItem_LifeAbility : CActionItem
{

// 	//得到操作类型
// public	virtual ACTION_OPTYPE	GetType(VOID) const		{ return AOT_SKILL_LIFEABILITY;	}
// 	//类型字符串
// public	virtual	LPCTSTR			GetType_String(VOID)	{ return NAMETYPE_LIFESKILL; }
// 	//得到定义ID
// public	virtual INT				GetDefineID(VOID);
// 	//得到数量
// public	virtual INT				GetNum(VOID)			{ return -1;	}
// 	//得到内部数据
// public	virtual VOID*			GetImpl(VOID)			{ return (VOID*)GetLifeAbilityImpl();	}
// 	//得到解释
// public	virtual LPCTSTR			GetDesc(VOID);
// 	//得到冷却状组ID
// public	virtual INT				GetCoolDownID(VOID)		{ return -1; }
// 	//得到所在容器的索引
// public	virtual INT				GetPosIndex(VOID);
// 	//是否能够自动继续进行
// public	virtual BOOL			AutoKeepOn(VOID)		{ return FALSE; }
// 	//激活动作
// public	virtual VOID			DoAction(VOID);
// 	//是否有效
// public	virtual BOOL			IsValidate(VOID)		{ return TRUE; }
// 	//刷新
// public	virtual VOID			Update(tActionReference* pRef) {}
// 	//检查冷却是否结束
// public	virtual BOOL			CoolDownIsOver(VOID)	{ return TRUE; }
// 	//拖动结束
// public	virtual VOID			NotifyDragDropDragged(BOOL bDestory, LPCSTR szTargetName, LPCSTR szSourceName);
// 
 public	CActionItem_LifeAbility(int nID):base(nID)
 {
     //todo
 }
// public	 ~CActionItem_LifeAbility();
// 
// 	//更新生活技能
 public	void	Update_LifeAbility(SCLIENT_LIFEABILITY pAbility)
 {
     //todo
 }
// 	//返回生活技能
// protected	 SCLIENT_LIFEABILITY	GetLifeAbilityImpl(VOID);
// 	//生活技能ID
// protected	int		m_idLifeAbilityImpl;

};