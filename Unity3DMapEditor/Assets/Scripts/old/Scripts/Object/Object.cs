using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
	游戏概念中的物体基类
*/
public class SObjectInit
{
    public Vector3 m_fvPos;
    public Vector3 m_fvRot;

    public SObjectInit()
    {
        m_fvPos.x = -1.0f;
        m_fvPos.y = -1.0f;
        m_fvPos.z = -1.0f;

        m_fvRot.x = -1.0f;
        m_fvRot.y = -1.0f;
        m_fvRot.z = -1.0f;
    }

    public virtual void Reset()
    {
        m_fvPos.x = -1.0f;
        m_fvPos.y = -1.0f;
        m_fvPos.z = -1.0f;

        m_fvRot.x = -1.0f;
        m_fvRot.y = -1.0f;
        m_fvRot.z = -1.0f;
    }
};
public enum ObjectStatusFlags
{
    OSF_NONE			=	(0x0),		//空状态
    OSF_VISIABLE		=	(0x1),		//是否可见
    OSF_OUT_VISUAL_FIELD=	(0x2),		//已经不再玩家的视野范围,如果该值维持一定时间,则会被删除
    OSF_RAY_QUERY		=	(0x4)		//鼠标能否选中
}
public class CObject:CNode
{
    	// NPC的任务状态，按照显示规则排序，排序后面的优先于前面的 [6/16/2011 ivan edit]
	// 		根据优先级由高到低有以下顺序显示：
	// 		1、	史诗任务可交
	// 		2、	普通任务可交
	// 		3、	循环任务（每日或每周的日常）可交
	// 		4、	史诗任务可接
	// 		5、	普通任务可接
	// 		6、	循环任务（每日或每周的日常）可接
	// 		7、	所有任务类型的未完成
	// 		8、	没有任何任务标签显示（这是第八种情况）

	public enum MISSION_STATE
	{
		MS_INVALID = -1,

		MS_MISS_CONTINUE,			// 任务未完成
		MS_MISS_ACCEPT_CIRCLE,		// 循环可接任务
		MS_MISS_ACCEPT_NORMAL,		// 普通可接任务
		MS_MISS_ACCEPT_HISTORY,		// 史诗可接任务
		MS_MISS_DONE_CIRCLE,		// 循环已完成任务
		MS_MISS_DONE_NORMAL,		// 普通已完成任务
		MS_MISS_DONE_HISTORY,		// 史诗已完成任务
	};
	public virtual MISSION_STATE GetNpcMissionState(){return MISSION_STATE.MS_INVALID;}

//-----------------------------------------------------
//针对与Tripper物体
    public	enum TRIPPER_OBJECT_TYPE
	{
		TOT_NONE,			//非Tripper物体
		TOT_TRANSPORT,		//转送点
		TOT_ITEMBOX,		//掉落的箱子
		TOT_RESOURCE,		//生活技能中的矿物资源
		TOT_PLATFORM,		//生活技能中的合成所需要的平台
							//...
	};
    	//TripperObj
	//物体类型
	public virtual TRIPPER_OBJECT_TYPE		Tripper_GetType() 	{ return TRIPPER_OBJECT_TYPE.TOT_NONE; }
	//能否鼠标操作
	public virtual bool					Tripper_CanOperation()  { return false; }
	//鼠标类型
	public virtual ENUM_CURSOR_TYPE		Tripper_GetCursor()  { return ENUM_CURSOR_TYPE.CURSOR_NORMAL; }
	//进入激活
	public virtual void					Tripper_Active() {}
    public override void Initial(object pInit)
    {
        SObjectInit ObjectInit = (SObjectInit)pInit;
        if (ObjectInit != null)
        {
            SetPosition(ObjectInit.m_fvPos);
            SetFaceDir(ObjectInit.m_fvRot.y);
        }
        m_nLogicCount = -1;
    }
    public void OnRenderInterfaceCreatedEvt(GFX.GfxActor actor)
    {
        //建立RenderInterface GameObject和 该Object之间的映射，用于射线查询 [1/4/2012 ZZY]
        if (mRenderInterface != null && mRenderInterface.getGameObject() != null )
        {
            CObjectManager.Instance.insertPhyObject(mRenderInterface.getGameObject(), this);
        }
    }
    public virtual void CreateRenderInterface() 
    {
        //如果是actor对象，需要把renderInterface和Ojbect建立映射，用于射线查询
        if (mRenderInterface != null && (mRenderInterface is GFX.GfxActor))
        {
            ((GFX.GfxActor)mRenderInterface).setRenderInterfaceCreateEvt(new GFX.OnRenderInterfaceEvent(OnRenderInterfaceCreatedEvt));
        }
    }
    public virtual void ReleaseRenderInterface()
    {
        if (mRenderInterface != null && mRenderInterface.getGameObject() != null)
        {
            CObjectManager.Instance.removePhyObject(mRenderInterface.getGameObject());
        }
        GFX.GFXObjectManager.Instance.DestroyObject(mRenderInterface);
        mRenderInterface = null;
    }
    public override void Release()
    {
        base.Release();
        ReleaseRenderInterface();
    }

    public virtual void	 Destroy()
    {
        Release();
	    CObjectManager.Instance.DestroyObject(this);
	    return;
    }

    ~CObject()
    {
    }
	protected GFX.GfxObject	mRenderInterface;
    protected Vector3 mPosition = Vector3.zero;
    protected float mRotation=0;//绕Y轴旋转角度
    protected Vector3 mScale = Vector3.one;


    //该物体在客户端的唯一ID,在Object生成时动态生成
    protected int mID;
    public int ID
	{
		set{
			mID = value;
		}
		get{
			return mID; 
		}
	}
    // 服务器端的逻辑计数
//     int m_nLogicCount;
//     public int LogicCount
//     {
//         get { return m_nLogicCount; }
//         set { m_nLogicCount = value; }
//     }
    //该物体在服务器上的ID，由服务器传回，
    //对于完全不受服务器控制的静态物体，该值为INVALID_ID
    protected int mServerID;
    public int ServerID 
	{
		set{
			mServerID = value;
		}
		get{
			return mServerID; 
		}
    }
    //-----------------------------------------------------
    //--------------------------------------------------------
	//基本状态标志
	/*
    |    xxxxxxxx xxxxxxxx xxxxxxxx xxxxx000
    |                                    |||_____ 1:可见  0:不可见
    |                                    ||______ 1:渲染名字 0:不渲染名字
    |                                    |_______ 1:已经不在玩家的视野范围,如果该值维持一定时间,则会被删除 0:仍在玩家视野范围
    |           
	*/
	uint			m_dwStatusFlag;
	
	//--------------------------------------------------------

	///设置某项基本状态为Enable
    public virtual void Enable(uint dwFlag)
    {
        m_dwStatusFlag |= dwFlag;
        //附加处理
        switch (dwFlag)
        {
            case (uint)ObjectStatusFlags.OSF_VISIABLE:
                if (mRenderInterface != null) mRenderInterface.SetVisible(true);
                break;

            case (uint)ObjectStatusFlags.OSF_RAY_QUERY:
                if (mRenderInterface != null) mRenderInterface.SetRayQuery(true);
                break;

            default:
                break;
        }
    }
	///设置某项基本状态为Disable
    public virtual void Disalbe(uint dwFlag)
    { 
        m_dwStatusFlag &= (~dwFlag); 
	    //附加处理
	    switch(dwFlag)
	    {
            case (uint)ObjectStatusFlags.OSF_VISIABLE:
		    {
			    if(mRenderInterface != null)mRenderInterface.SetVisible(false);
			    if(this == CObjectManager.Instance.GetMainTarget())
			    {
                    CObjectManager.Instance.SetMainTarget(MacroDefine.INVALID_ID, CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT);
			    }
		    }
		    break;

            case (uint)ObjectStatusFlags.OSF_RAY_QUERY:
		    if(mRenderInterface != null )mRenderInterface.SetRayQuery(false);
		    break;
	    default:
		    break;
	    }
    }
	///察看某项状态是否为Enable
	public virtual	bool				IsEnable(uint dwFlag){ return (m_dwStatusFlag&dwFlag)!=0 ? true : false; }
	///察看某项状态是否为Disable
	public virtual	bool				IsDisable(uint dwFlag){ return (m_dwStatusFlag&dwFlag)!=0 ? false : true; }

	public bool	isVisible() { return IsEnable((uint)ObjectStatusFlags.OSF_VISIABLE) ;}
    public void setVisible(bool setting) { Enable((uint)ObjectStatusFlags.OSF_VISIABLE); }

    protected uint m_timeMsg;
    	//--------------------------------------------------------
	//最近一次接受服务器消息时间
    public void SetMsgTime(uint dwTime) { m_timeMsg = dwTime; }
    public uint GetMsgTime() { return m_timeMsg; }
    public GFX.GfxObject GetRenderInterface()
    {
        return mRenderInterface;    
    }
   
    //如果有该标志，表明该物体只是用于UI渲染，受FakeObjectManager管理
    bool m_bFakeObject =false;
     //是否属于UI渲染的对象
	public bool						GetFakeObjectFlag()   { return m_bFakeObject; }
	public void						SetFakeObjectFlag(bool bVisibleFlag) { m_bFakeObject = bVisibleFlag; }
    //for debug
    public void PushDebugString(string szDebugString)
    {
        //todo
    }
    public virtual string GetDebugDesc() { return ""; }

    int m_nLogicCount;
    	//服务器端的当前OBJ逻缉号
	public int							GetLogicCount(  ){ return m_nLogicCount; }
    public void SetLogicCount(int nLogicCount) { m_nLogicCount = nLogicCount; }
	public virtual bool				IsLogicReady( int nLogicCount ){ return (m_nLogicCount >= nLogicCount)?(true):(false); }
    public virtual void ResetLogicCount() { m_nLogicCount = -1; }

    public virtual void SetMapPosition(float x, float z) { }
    //位置朝向和旋转渲染属性
    public virtual void SetPosition(Vector3 pos)
    {
        mPosition = pos;
        GameProcedure.m_bWaitNeedFreshMinimap = true;
        if (mRenderInterface != null )
        {
            mRenderInterface.position = mPosition;
        }
    }
    public Vector3 GetPosition() {  return mPosition;  }

    //摄像机跟踪的点 
    protected Vector3 m_fvFootPosition;
    public virtual void SetFootPosition(Vector3 fvPosition)
    {
        m_fvFootPosition = fvPosition;
    }
	public Vector3				GetFootPosition(){ return m_fvFootPosition;		}

    public virtual void SetScale( Vector3 scale)
    {
        mScale = scale;
        if (mRenderInterface != null )
        {
            mRenderInterface.scale = mScale;
        }
    }
    public Vector3 GetScale()
    {
        return mScale;
    }

    public virtual void SetFaceDir(float dir)//朝向
    {

        mRotation = dir;
        if (mRenderInterface != null)
        {
            mRenderInterface.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg*mRotation, Vector3.up);
        }
    }
    public float GetFaceDir()
    {
        return mRotation;
    }
    public virtual void FillMouseCommand_Left(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
    }
    public virtual void FillMouseCommand_Right(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
    }

    public virtual bool	CanbeSelect(){ return false; }
    public virtual bool PushCommand(SCommand_Object pCmd )
    {
        OnCommand(pCmd);
        return true;
    }
	// 指令接收, 外界控制角色的唯一接口
    protected virtual RC_RESULT OnCommand(SCommand_Object pCmd) { return RC_RESULT.RC_SKIP; }
	
    public bool IsSpecialObject(ref string strReturn)
	{
		if(this == (CObject)CObjectManager.Instance.getPlayerMySelf())
		{
			strReturn = "player";
			return true;
		}
        else if (this == (CObject)CObjectManager.Instance.GetMainTarget())
        {
            strReturn = "target";
            return true;
        }
        else
        {
            return false;
        }
			
	}

    public virtual _CAMP_DATA GetCampData() { return new _CAMP_DATA(); }
}	


