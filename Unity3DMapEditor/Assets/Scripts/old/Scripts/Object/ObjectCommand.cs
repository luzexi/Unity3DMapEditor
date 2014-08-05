public enum ENUM_OBJECT_COMMAND
{
	OBJECT_COMMAND_INVALID	= -1,
	OBJECT_COMMAND_ACTION,				// 动作
	OBJECT_COMMAND_STOP_ACTION,			// 停止动作
	OBJECT_COMMAND_MOVE,				// 移动
	OBJECT_COMMAND_STOP_MOVE,			// 中止移动
	OBJECT_COMMAND_MAGIC_SEND,			// 法术发招
	OBJECT_COMMAND_MAGIC_CHARGE,		// 法术聚气
	OBJECT_COMMAND_MAGIC_CHANNEL,		// 法术引导
	OBJECT_COMMAND_ABILITY,				// 生活技能

	OBJECT_COMMAND_NUMBERS
};

/////////////////////////////////////////////
// CObjectCommand
/////////////////////////////////////////////
public class CObjectCommand
{
    public CObjectCommand()
    {
        m_nCommandID = ENUM_OBJECT_COMMAND.OBJECT_COMMAND_INVALID;
    }

    public virtual void CleanUp()
	{
		m_nCommandID = ENUM_OBJECT_COMMAND.OBJECT_COMMAND_INVALID;
	}

    public ENUM_OBJECT_COMMAND GetCommandID()
	{
		return m_nCommandID;
	}

	protected void SetCommandID(ENUM_OBJECT_COMMAND nCommandID)
	{
		m_nCommandID = nCommandID;
	}

	private	ENUM_OBJECT_COMMAND		m_nCommandID;		// ENUM_OBJECT_COMMAND
};



/////////////////////////////////////////////
// CObjectCommand_StopLogic 中断逻辑指令
/////////////////////////////////////////////
public class CObjectCommand_StopLogic : CObjectCommand 
{
    public CObjectCommand_StopLogic()
    {
	    m_nLogicCount	= -1;
    }

    public override void CleanUp()
    {
        m_nLogicCount	= -1;
    }

    public int GetLogicCount()
	{
		return m_nLogicCount;
	}

    protected void SetLogicCount(int nLogicCount)
	{
		m_nLogicCount = nLogicCount;
	}

    private	int			m_nLogicCount;		// 逻辑计数
};


/////////////////////////////////////////////
// CObjectCommand_Logic 逻辑指令
/////////////////////////////////////////////
public class CObjectCommand_Logic : CObjectCommand
{
    public CObjectCommand_Logic()
    {
	    m_uStartTime	= MacroDefine.UINT_MAX;
	    m_nLogicCount	= -1;
    }
	public override void CleanUp()
    {
        m_uStartTime	= MacroDefine.UINT_MAX;
	    m_nLogicCount	= -1;
    }

	public virtual bool Modify(CObjectCommand_StopLogic cmd)
    {
        return true;
    }

	public uint GetStartTime()
	{
		return m_uStartTime;
	}

	public int GetLogicCount()
	{
		return m_nLogicCount;
	}

	protected void SetStartTime(uint uTime)
	{
		m_uStartTime = uTime;
	}

	protected void SetLogicCount(int nLogicCount)
	{
		m_nLogicCount = nLogicCount;
	}
	private uint		m_uStartTime;		// 起始时间
	private int			m_nLogicCount;		// 逻辑计数
};

/////////////////////////////////////////////
// CObjectCommand_Action 动作
/////////////////////////////////////////////
public class CObjectCommand_Action :CObjectCommand_Logic
{
    public CObjectCommand_Action()
    {
        m_nActionID = MacroDefine.INVALID_ID;
    }

	public bool Init(uint uStartTime, int nLogicCount, int nActionID)
    {
        SetStartTime(uStartTime);
	    SetLogicCount(nLogicCount);
	    m_nActionID		= MacroDefine.INVALID_ID;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_ACTION);
        return true;
    }

	public override void CleanUp()
    {
        m_nActionID		= MacroDefine.INVALID_ID;
	    base.CleanUp();
    }

	public int GetActionID()
	{
		return m_nActionID;
	}

	private	int			m_nActionID;		// 动作ID
};

/////////////////////////////////////////////
// CObjectCommand_StopAction 中止动作
/////////////////////////////////////////////
public class CObjectCommand_StopAction :CObjectCommand_StopLogic
{
    public CObjectCommand_StopAction()
    {
        m_uEndTime			= MacroDefine.UINT_MAX;
    }

	public bool Init(int nLogicCount, uint uEndTime)
    {
        SetLogicCount(nLogicCount);
	    m_uEndTime			= uEndTime;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_ACTION);
        return true;
    }

	public override void CleanUp()
    {
        m_uEndTime			= MacroDefine.UINT_MAX;
	    base.CleanUp();
    }

	public uint GetEndTime()
	{
		return m_uEndTime;
	}

    private	uint		m_uEndTime;			// 结束时间
};

/////////////////////////////////////////////
// CObjectCommand_StopMove 中止移动
/////////////////////////////////////////////
public class CObjectCommand_StopMove :CObjectCommand_StopLogic
{
    public CObjectCommand_StopMove()
    {
        m_nEndNodeIndex		= -1;
    }

	public bool Init(int nLogicCount, int nEndNodeIndex, WORLD_POS Pos)
    {
        SetLogicCount(nLogicCount);
	    m_nEndNodeIndex		= nEndNodeIndex;
	    m_posEndPos			= Pos;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_MOVE);
	    return true;
    }

	public override void CleanUp()
    {
        m_nEndNodeIndex		= -1;
	    m_posEndPos.m_fX = -1.0f;
		m_posEndPos.m_fZ = -1.0f;
	    base.CleanUp();
    }

	public int GetEndNodeIndex()
	{
		return m_nEndNodeIndex;
	}

    public WORLD_POS GetEndPos()
	{
		return m_posEndPos;
	}

    private	int			m_nEndNodeIndex;		// 中止的节点索引(如果总节点为1，那么中止节点只可能为0)
    private	WORLD_POS	m_posEndPos;			// 中止的位置(在中止的节点索引后面的出现的坐标)
};

/////////////////////////////////////////////
// CObjectCommand_Move 移动
/////////////////////////////////////////////
public class CObjectCommand_Move :CObjectCommand_Logic
{
    public CObjectCommand_Move()
    {
        m_nNodeCount	= 0;
	    m_paposNode		= null;
    }


    public bool Init(uint uStartTime, int nLogicCount, int nNodeCount, WORLD_POS[] paposNode)
    {
	    SetStartTime(uStartTime);
	    SetLogicCount(nLogicCount);
	    m_nNodeCount	= nNodeCount;
	    if(m_nNodeCount > 0)
	    {
		    m_paposNode	= new WORLD_POS[m_nNodeCount];
            m_paposNode = paposNode;
	    }
	    else
	    {
		    m_paposNode	= null;
	    }

	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MOVE);
	    return true;
    }
    public override void CleanUp()
    {
        m_nNodeCount	= 0;
        m_paposNode     = null;
	    base.CleanUp();
    }

    public override bool Modify(CObjectCommand_StopLogic cmd)
    {
        if(cmd != null && cmd.GetCommandID() == ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_MOVE)
	    {
		    CObjectCommand_StopMove stopMoveCmd = (CObjectCommand_StopMove)cmd;
		    int nEndNodeIndex	= stopMoveCmd.GetEndNodeIndex();
		    if(m_nNodeCount > nEndNodeIndex)
		    {
			    m_nNodeCount = nEndNodeIndex + 1;
			    m_paposNode[nEndNodeIndex]	= stopMoveCmd.GetEndPos();
		    }
		    return true;
	    }
	    return false;
    }

    public int GetNodeCount()
	{
		return m_nNodeCount;
	}

	public WORLD_POS[] GetNodeList()
	{
		return m_paposNode;
	}

	public bool ModifyTargetPos(byte numPos, WORLD_POS[] targetPos)
    {
        if( m_nNodeCount == 0 || numPos == 0 || m_nNodeCount < numPos ) return false;

	    m_nNodeCount = numPos;
	    m_paposNode[numPos-1] = targetPos[numPos-1];
	    return true;
    }

    private	int	        m_nNodeCount;		// 节点数目
    private	WORLD_POS[] m_paposNode;		// 节点数据
};



/////////////////////////////////////////////
// CObjectCommand_MagicSend 法术发招
/////////////////////////////////////////////
public class CObjectCommand_MagicSend :CObjectCommand_Logic
{
    public CObjectCommand_MagicSend()
    {
        m_nMagicID			= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    m_fTargetDir		= -1.0f;
    }


    public bool Init(uint uStartTime,int nLogicCount,int nMagicID,uint nTargetID,WORLD_POS TargetPos,float fTargetDir)
    {
        SetStartTime(uStartTime);
	    SetLogicCount(nLogicCount);
	    m_nMagicID			= nMagicID;
	    m_nTargetObjID		= nTargetID;
	    m_posTarget			= TargetPos;
	    m_fTargetDir		= fTargetDir;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_SEND);
	    return true;
    }

    public override void CleanUp()
    {
        m_nMagicID			= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    m_posTarget.m_fX    = -1.0f;
		m_posTarget.m_fZ    = -1.0f;
	    m_fTargetDir		= -1.0f;
	    base.CleanUp();
    }

    public	int GetMagicID()
	{
		return m_nMagicID;
	}

    public  uint GetTargetObjID()
	{
		return m_nTargetObjID;
	}

	public WORLD_POS GetTargetPos()
	{
		return m_posTarget;
	}

	// 支持保存计算后的落点 [10/31/2011 Ivan edit]
	public void	SetTargetPos(float x, float z)
	{
		m_posTarget.m_fX = x;
		m_posTarget.m_fZ = z;
	}

	public float GetTargetDir()
	{
		return m_fTargetDir;
	}

    private	int			m_nMagicID;			// 法术ID
    private	uint		m_nTargetObjID;		// 目标角色
    private	WORLD_POS	m_posTarget;		// 目标位置
    private	float		m_fTargetDir;		// 目标方向
};

/////////////////////////////////////////////
// CObjectCommand_MagicCharge 法术聚气
/////////////////////////////////////////////
public class CObjectCommand_MagicCharge :CObjectCommand_Logic
{
    public CObjectCommand_MagicCharge()
    {
	    m_nMagicID			= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    m_fTargetDir		= -1.0f;
	    m_uTotalTime		= MacroDefine.UINT_MAX;
	    m_uEndTime			= MacroDefine.UINT_MAX;
    }

	public bool Init(uint uStartTime,int nLogicCount,int nMagicID,uint nTargetID,
                     WORLD_POS TargetPos,float fTargetDir,uint uTotalTime)
    {
        SetStartTime(uStartTime);
	    SetLogicCount(nLogicCount);
	    m_nMagicID			= nMagicID;
	    m_nTargetObjID		= nTargetID;
	    m_posTarget			= TargetPos;
	    m_fTargetDir		= fTargetDir;
	    m_uTotalTime		= uTotalTime;
	    m_uEndTime			= m_uTotalTime;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_CHARGE);
		return true;
    }

	public override void CleanUp()
    {
        m_nMagicID			= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	   	m_posTarget.m_fX    = -1.0f;
		m_posTarget.m_fZ    = -1.0f;
	    m_fTargetDir		= -1.0f;
	    m_uTotalTime		= MacroDefine.UINT_MAX;
	    m_uEndTime			= MacroDefine.UINT_MAX;
	    base.CleanUp();
    }

	public override bool Modify(CObjectCommand_StopLogic cmd)
    {
        if(cmd != null && cmd.GetCommandID() == ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_ACTION)
	    {
		    CObjectCommand_StopAction StopCmd = (CObjectCommand_StopAction)cmd;
		    uint uEndTime = 0;
		    if(m_uTotalTime > StopCmd.GetEndTime())
		    {
			    uEndTime = m_uTotalTime - StopCmd.GetEndTime();
		    }
		    if(uEndTime < m_uEndTime)
		    {
			    m_uEndTime = uEndTime;
		    }
		    return true;
	    }
	    return false;
    }

	public int GetMagicID()
	{
		return m_nMagicID;
	}

    public uint GetTargetObjID()
	{
		return m_nTargetObjID;
	}

    public WORLD_POS GetTargetPos()
	{
		return m_posTarget;
	}

    public float GetTargetDir()
	{
		return m_fTargetDir;
	}

    public uint GetTotalTime()
	{
		return m_uTotalTime;
	}

    public uint GetEndTime()
	{
		return m_uEndTime;
	}

    private	int			m_nMagicID;			// 法术ID
    private	uint		m_nTargetObjID;		// 目标角色
    private	WORLD_POS	m_posTarget;		// 目标位置
    private	float		m_fTargetDir;		// 目标方向
    private	uint		m_uTotalTime;		// 总时间
    private	uint		m_uEndTime;			// 结束时间
};

/////////////////////////////////////////////
// CObjectCommand_MagicChannel 法术引导
/////////////////////////////////////////////
public class CObjectCommand_MagicChannel:CObjectCommand_Logic
{
    public CObjectCommand_MagicChannel()
    {
        m_nMagicID			= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    m_fTargetDir		= -1.0f;
	    m_uTotalTime		= MacroDefine.UINT_MAX;
	    m_uEndTime			= MacroDefine.UINT_MAX;
    }

    public bool Init(uint uStartTime,int nLogicCount,int nMagicID,uint nTargetID,
		             WORLD_POS TargetPos,float fTargetDir,uint uTotalTime)
    {
        SetStartTime(uStartTime);
	    SetLogicCount(nLogicCount);
	    m_nMagicID			= nMagicID;
	    m_nTargetObjID		= nTargetID;
	    m_posTarget			= TargetPos;
	    m_fTargetDir		= fTargetDir;
	    m_uTotalTime		= uTotalTime;
	    m_uEndTime			= 0;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_MAGIC_CHANNEL);
        return true;
    }

	public override void CleanUp()
    {
        m_nMagicID			= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    m_posTarget.m_fX    = -1.0f;
		m_posTarget.m_fZ    = -1.0f;
	    m_fTargetDir		= -1.0f;
	    m_uTotalTime		= MacroDefine.UINT_MAX;
	    m_uEndTime			= MacroDefine.UINT_MAX;
	    base.CleanUp();
    }

	public override bool Modify(CObjectCommand_StopLogic cmd)
    {
        if(cmd != null && cmd.GetCommandID() == ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_ACTION)
	    {
		    CObjectCommand_StopAction StopCmd = (CObjectCommand_StopAction)cmd;
		    uint uEndTime = StopCmd.GetEndTime();
		    if(uEndTime > m_uEndTime)
		    {
			    m_uEndTime = uEndTime;
		    }
		    return true;
	    }
	    return false;
    }

	public int GetMagicID()
	{
		return m_nMagicID;
	}

	public uint GetTargetObjID()
	{
		return m_nTargetObjID;
	}

    public WORLD_POS GetTargetPos()
	{
		return m_posTarget;
	}

    public float GetTargetDir()
	{
		return m_fTargetDir;
	}

    public uint GetTotalTime()
	{
		return m_uTotalTime;
	}

    public uint GetEndTime()
	{
		return m_uEndTime;
	}

    private	int			m_nMagicID;			// 法术ID
    private	uint		m_nTargetObjID;		// 目标角色
    private	WORLD_POS	m_posTarget;		// 目标位置
    private	float		m_fTargetDir;		// 目标方向
    private	uint		m_uTotalTime;		// 总时间
    private	uint		m_uEndTime;			// 结束时间
};

/////////////////////////////////////////////
// CObjectCommand_Ability 生活技能
/////////////////////////////////////////////
public class CObjectCommand_Ability :CObjectCommand_Logic
{
    public CObjectCommand_Ability()
    {
        m_nAbilityID		= MacroDefine.INVALID_ID;
	    m_nPrescriptionID	= MacroDefine.INVALID_ID;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    m_uEndTime			= MacroDefine.UINT_MAX;
    }


    public bool Init(uint uStartTime, int nLogicCount, int nAbilityID, int nPrescriptionID, uint nTargetObjID)
    {
        SetStartTime(uStartTime);
	    SetLogicCount(nLogicCount);
	    m_nAbilityID		= nAbilityID;
	    m_nPrescriptionID	= nPrescriptionID;
	    m_nTargetObjID		= nTargetObjID;
	    m_uEndTime			= MacroDefine.UINT_MAX;
	    SetCommandID(ENUM_OBJECT_COMMAND.OBJECT_COMMAND_ABILITY);
	    return true;
    }

    public override void CleanUp()
    {
        m_nAbilityID		= MacroDefine.INVALID_ID;
	    m_nPrescriptionID	= MacroDefine.INVALID_ID;
	    m_uEndTime			= MacroDefine.UINT_MAX;
	    m_nTargetObjID		= MacroDefine.UINT_MAX;
	    base.CleanUp();
    }

    public override bool Modify(CObjectCommand_StopLogic cmd)
    {
        if(cmd != null && cmd.GetCommandID() == ENUM_OBJECT_COMMAND.OBJECT_COMMAND_STOP_ACTION)
	    {
		    CObjectCommand_StopAction StopCmd = (CObjectCommand_StopAction)cmd;
		    uint uEndTime = StopCmd.GetEndTime();
		    if(uEndTime < m_uEndTime)
		    {
			    m_uEndTime = uEndTime;
		    }
		    return true;
	    }
	    return false;
    }

    public int GetAbilityID()
    {
	    return m_nAbilityID;
    }

    public int GetPrescriptionID()
    {
	    return m_nPrescriptionID;
    }

    public uint GetTargetObjID()
    {
		return m_nTargetObjID;
    }

    public uint GetEndTime()
	{
		return m_uEndTime;
	}

    private int			m_nAbilityID;		// 生活技能ID
    private int			m_nPrescriptionID;	// 配方ID
    private uint		m_nTargetObjID;		// 目标角色ID
    private uint		m_uEndTime;			// 结束时间
};


/////////////////////////////////////////////////////////////////////////////////////////
// 转换函数
/////////////////////////////////////////////////////////////////////////////////////////
public class ObjectCommandGenerator
{
    static public CObjectCommand NewObjectCommand(SCommand_Object cmd)
    {
	    CObjectCommand retCmd = null;
	    if(cmd != null)
	    {
            switch((OBJECTCOMMANDDEF)cmd.m_wID)
		    {
		        case OBJECTCOMMANDDEF.OC_STOP_ACTION:
			    {
				    CObjectCommand_StopAction NewCmd = new CObjectCommand_StopAction();
				    int nLogicCount;
				    uint uEndTime;
                    nLogicCount = cmd.GetValue<int>(0);
                    uEndTime    = cmd.GetValue<uint>(1);

				    bool bResult = NewCmd.Init(nLogicCount, uEndTime);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
                        NewCmd = null;
				    }
			    }
			    break;
		        case OBJECTCOMMANDDEF.OC_STOP_MOVE:
			    {
				    CObjectCommand_StopMove NewCmd = new CObjectCommand_StopMove();
				    int nLogicCount, nEndNodeIndex;
				    WORLD_POS posEnd;
                    nLogicCount     = cmd.GetValue<int>(0);
                    nEndNodeIndex   = cmd.GetValue<int>(1);
                    posEnd.m_fX     = cmd.GetValue<float>(2);
                    posEnd.m_fZ     = cmd.GetValue<float>(3);

				    bool bResult = NewCmd.Init(nLogicCount, nEndNodeIndex,posEnd);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		        case OBJECTCOMMANDDEF.OC_ACTION:
			    {
				    CObjectCommand_Action NewCmd = new CObjectCommand_Action();
				    uint uStartTime;
				    int nLogicCount, nActionID;
                    uStartTime      = cmd.GetValue<uint>(0);
                    nLogicCount     = cmd.GetValue<int>(1);
                    nActionID       = cmd.GetValue<int>(2);

				    bool bResult = NewCmd.Init(uStartTime, nLogicCount, nActionID);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		        case OBJECTCOMMANDDEF.OC_MOVE:
			    {
				    CObjectCommand_Move NewCmd = new CObjectCommand_Move();
				    uint uStartTime;
				    int nLogicCount, nNodeCount;
				    WORLD_POS[] paposNode;
                    uStartTime      = cmd.GetValue<uint>(0);
                    nLogicCount     = cmd.GetValue<int>(1);
                    nNodeCount      = cmd.GetValue<int>(2);
				    paposNode		= (cmd.GetValue<WORLD_POS[]>(3));

				    bool bResult = NewCmd.Init(uStartTime, nLogicCount, nNodeCount, paposNode);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		        case OBJECTCOMMANDDEF.OC_MAGIC_SEND:
			    {
				    CObjectCommand_MagicSend NewCmd = new CObjectCommand_MagicSend();
				    uint		uStartTime;
				    int			nLogicCount;
				    int			nMagicID;
				    uint		nTargetObjID;
				    WORLD_POS	posTarget;
				    float		fTargetDir;

				    uStartTime		= cmd.GetValue<uint>(0);
				    nLogicCount		= cmd.GetValue<int>(1);
                    nMagicID        = cmd.GetValue<short>(2);
                    nTargetObjID    = cmd.GetValue<uint>(3);
				    posTarget.m_fX	= cmd.GetValue<float>(4);
                    posTarget.m_fZ  = cmd.GetValue<float>(5);
                    fTargetDir      = cmd.GetValue<float>(6);

				    bool bResult = NewCmd.Init(uStartTime, nLogicCount, nMagicID, nTargetObjID, posTarget, fTargetDir);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		        case OBJECTCOMMANDDEF.OC_MAGIC_CHARGE:
			    {
				    CObjectCommand_MagicCharge NewCmd = new CObjectCommand_MagicCharge();
				    uint		uStartTime;
				    int			nLogicCount;
				    int			nMagicID;
				    uint		nTargetObjID;
				    WORLD_POS	posTarget;
				    float		fTargetDir;
				    uint		uTotalTime;

                    uStartTime      = cmd.GetValue<uint>(0);
                    nLogicCount     = cmd.GetValue<int>(1);
                    nMagicID        = cmd.GetValue<short>(2);
                    nTargetObjID    = (uint)cmd.GetValue<int>(3);
                    posTarget.m_fX  = cmd.GetValue<float>(4);
                    posTarget.m_fZ  = cmd.GetValue<float>(5);
                    fTargetDir      = cmd.GetValue<float>(6);
                    uTotalTime      = (uint)cmd.GetValue<int>(7);

				    bool bResult = NewCmd.Init(uStartTime, nLogicCount, nMagicID, nTargetObjID, posTarget, fTargetDir, uTotalTime);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		    case OBJECTCOMMANDDEF.OC_MAGIC_CHANNEL:
			    {
				    CObjectCommand_MagicChannel NewCmd = new CObjectCommand_MagicChannel();
				    uint		uStartTime;
				    int			nLogicCount;
				    int			nMagicID;
				    uint		nTargetObjID;
				    WORLD_POS	posTarget;
				    float		fTargetDir;
				    uint		uTotalTime;

                    uStartTime      = cmd.GetValue<uint>(0);
                    nLogicCount     = cmd.GetValue<int>(1);
                    nMagicID        = cmd.GetValue<short>(2);
                    nTargetObjID    = (uint)cmd.GetValue<int>(3);
                    posTarget.m_fX  = cmd.GetValue<float>(4);
                    posTarget.m_fZ  = cmd.GetValue<float>(5);
                    fTargetDir      = cmd.GetValue<float>(6);
                    uTotalTime      = (uint)cmd.GetValue<int>(7);

				    bool bResult = NewCmd.Init(uStartTime, nLogicCount, nMagicID, nTargetObjID, posTarget, fTargetDir, uTotalTime);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		    case OBJECTCOMMANDDEF.OC_ABILITY:
			    {
				    CObjectCommand_Ability NewCmd = new CObjectCommand_Ability();
				    int		uStartTime;
				    int			nLogicCount;
				    int			nAbilityID;
				    int			nPrescriptionID;
				    uint			nTargetID;
                    uStartTime = cmd.GetValue<int>(0);
                    nLogicCount = cmd.GetValue<int>(1);
                    nAbilityID = cmd.GetValue<short>(2);
                    nPrescriptionID = cmd.GetValue<int>(3);
                    nTargetID = cmd.GetValue<uint>(4);

				    bool bResult = NewCmd.Init((uint)uStartTime, nLogicCount, nAbilityID, nPrescriptionID, nTargetID);
				    if(bResult)
				    {
					    retCmd = NewCmd;
				    }
				    else
				    {
					    NewCmd.CleanUp();
					    NewCmd = null;
				    }
			    }
			    break;
		        case OBJECTCOMMANDDEF.OC_MODIFY_ACTION:
		            default:
			    break;
		    }
	    }
	    return retCmd;
    }

   static public void DeleteObjectCommand(CObjectCommand pCmd)
    {
        if (pCmd != null)
        {
            pCmd.CleanUp();
            pCmd = null;
        }
    }
};