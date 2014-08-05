// Obj_Special.h
//
// 作用：技能所产生的OBJ， 类似于原来的SKILLOBJ，
//		但由于服务器端以这个名字来命名，那么客户端也就这样吧
// 
///////////////////////////////////////////////////////////
using System.Collections.Generic;
using DBSystem;
public class SObject_SpecialInit :SObject_EffectInit
{
	public int			m_idOwner;		// 所有者
	public int				m_nDataID;		// 资源ID

	public SObject_SpecialInit()
    {
		m_idOwner			= MacroDefine.INVALID_ID;
		m_nDataID			= MacroDefine.INVALID_ID;
	}

	public override void Reset()
    {
		base.Reset();
		m_idOwner			= MacroDefine.INVALID_ID;
		m_nDataID			= MacroDefine.INVALID_ID;
	}
};

public class CObject_Special:CObject_Effect
{
    public	enum ENUM_SPECIAL_STATE
	{
		SPECIAL_STATE_INVALID	= -1,
		SPECIAL_STATE_NORMAL,
		SPECIAL_STATE_DIE,
		SPECIAL_STATE_NUMBERS
	};

	private     ENUM_SPECIAL_STATE			m_eSpecialState;
	private     int							m_idOwner;			// 所有者
	private     _DBC_SPECIAL_OBJ_DATA		m_pSpecialObjData = null;
	// 消息缓存
	private     List<SCommand_Object>		m_listCommand = new List<SCommand_Object>();
    public CObject_Special()
    {
        m_eSpecialState = ENUM_SPECIAL_STATE.SPECIAL_STATE_INVALID;
        m_idOwner = MacroDefine.INVALID_ID;
        m_pSpecialObjData = null;
    }
	

	//-----------------------------------------------------
	///根据初始化物体，并同步到渲染层
    public override void Initial(object pInit)
    {
        base.Initial(pInit);
        m_eSpecialState		= ENUM_SPECIAL_STATE.SPECIAL_STATE_NORMAL;

	    SObject_SpecialInit pSpecialInit = (SObject_SpecialInit)(pInit);
	    if ( pSpecialInit != null )
	    {
		    m_idOwner		= pSpecialInit.m_idOwner;
		    if ( pSpecialInit.m_nDataID != MacroDefine.INVALID_ID )
		    {
			    
			   m_pSpecialObjData = CDataBaseSystem.Instance.GetDataBase<_DBC_SPECIAL_OBJ_DATA>((int)DataBaseStruct.DBC_SPECIAL_OBJ_DATA).Search_Index_EQU(pSpecialInit.m_nDataID);
		    }
		    else
		    {
			    m_pSpecialObjData = null;
		    }
	    }
	    else
	    {
		    m_idOwner			= MacroDefine.INVALID_ID;
		    m_pSpecialObjData	= null;
	    }

	    if ( GetSpecialObjData() != null && GetSpecialObjData().m_lpszEffect_Normal != null && GetSpecialObjData().m_lpszEffect_Normal.Length > 0 )
	    {
		    ChangEffect( GetSpecialObjData().m_lpszEffect_Normal,true );
	    }

    }
    public override void Release()
    {
        ReleaseCommandList();
        m_eSpecialState     = ENUM_SPECIAL_STATE.SPECIAL_STATE_INVALID;
        m_idOwner           = MacroDefine.INVALID_ID;
	    m_pSpecialObjData	= null;

	    base.Release();
    }

    public override void Tick()
    {
        switch ( GetSpecialState() )
	    {
	        case ENUM_SPECIAL_STATE.SPECIAL_STATE_NORMAL:
		        {
			        // 如果有消息就执行消息， 否则就是等待状态
			        SCommand_Object pCommand = PopCommand();
			        if ( pCommand != null )
			        {
				        OnCommand( pCommand );
			        }
		        }
		        break;
	        case ENUM_SPECIAL_STATE.SPECIAL_STATE_DIE:
		        break;
	        default:
		        break;
	    }
	    base.Tick();
    }

	// 左键指令的分析
    public override void FillMouseCommand_Left(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
	    if ( pActiveSkill != null
		    && pActiveSkill.GetType() == ACTION_OPTYPE.AOT_SKILL )
	    {
		    SCLIENT_SKILL pSkillImpl = (SCLIENT_SKILL)pActiveSkill.GetImpl();
		    if(pSkillImpl != null
			    && pSkillImpl.m_pDefine.m_nSelectType == (int)ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER)
		    {
			    ENUM_RELATION eCampType = GameProcedure.s_pGameInterface.GetCampType((CObject) CObjectManager.Instance.getPlayerMySelf(), (CObject) this );
			    switch ( eCampType )
			    {
			        case ENUM_RELATION.RELATION_FRIEND:
				        break;
			        case ENUM_RELATION.RELATION_ENEMY:
			        default:
				    {
                        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_SKILL_OBJ;
					    pOutCmd.SetValue<object>(0,(object)pActiveSkill);
					    pOutCmd.SetValue<int>(1,ServerID);
				    }
				    break;
			    }
		    }
	    }
    }
	// 右键指令的分析
    public override void FillMouseCommand_Right(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_SELECT;
        pOutCmd.SetValue<int>(0,ServerID);
    }

	// 压入一条指令
    public override bool PushCommand(SCommand_Object pCmd)
    {
        m_listCommand.Add(pCmd);
	    return true;
    }


    protected override RC_RESULT OnCommand(SCommand_Object pCmd)
    {
        switch ((OBJECTCOMMANDDEF)pCmd.m_wID)
        {
            case OBJECTCOMMANDDEF.OC_SPECIAL_OBJ_TRIGGER:
                {
                    int nLogicCount, nTargetCount;
                    int[] paTargetIDs;
                    nLogicCount = pCmd.GetValue<int>(0);
                    nTargetCount = pCmd.GetValue<int>(1);
                    paTargetIDs = (int[])(pCmd.GetValue<object>(2));

                    DoTrigger(nLogicCount, nTargetCount, paTargetIDs);
                }
                break;
            case OBJECTCOMMANDDEF.OC_SPECIAL_OBJ_DIE:
                {
                    DoDie();
                }
                break;
            default:
                break;
        }
        return RC_RESULT.RC_SKIP;
    }

    protected void DoTrigger(int nLogicCount, int nTargetCount, int[] paTargetIDs)
    {
        SetLogicCount( nLogicCount );

	    if ( GetSpecialObjData() != null && GetSpecialObjData().m_lpszEffect_Active != null && GetSpecialObjData().m_lpszEffect_Active.Length > 0 )
	    {
		    CObject_Effect pEffectObj = (CObject_Effect)(CObjectManager.Instance.NewEffect(-1));
		    if ( pEffectObj != null )
		    {
			    SObject_EffectInit initEffect = new SObject_EffectInit();
			    initEffect.m_fvPos 			= GetPosition();
			    initEffect.m_fvRot 			= new UnityEngine.Vector3( 0.0f, 0.0f, 0.0f );
			    initEffect.m_pszEffectName	= GetSpecialObjData().m_lpszEffect_Active;
			    initEffect.m_bLoopEffect	= false;
			    pEffectObj.Initial(initEffect );
		    }
	    }


	    // 有子弹
	    if ( GetSpecialObjData() != null && GetSpecialObjData().m_nBulletID != MacroDefine.INVALID_ID )
	    {
		    SObject_BulletInit initBullet   = new SObject_BulletInit();
		    initBullet.m_fvPos				= GetPosition();

            UnityEngine.Vector3 fvRot = UnityEngine.Vector3.zero;
            fvRot.y   = GetFaceDir();
		    initBullet.m_fvRot				= fvRot;
		    initBullet.m_idSend				= (uint)ServerID;
		    initBullet.m_nSendLogicCount	= GetLogicCount();
		    initBullet.m_nBulletID			= m_pSpecialObjData.m_nBulletID;
		    initBullet.m_bSingleTarget		= true;
		    initBullet.m_fvTargetPos		= new UnityEngine.Vector3( -1.0f, -1.0f, -1.0f );
		    for (int i = 0; i < nTargetCount; i++ )
		    {
			    initBullet.m_idTarget	= paTargetIDs[i];
			    CObject_Bullet pBullet = (CObject_Bullet)CObjectManager.Instance.NewBullet(-1);
			    pBullet.Initial(initBullet );
		    }
	    }
	    else
	    {
		    CObject_Character pCharacter = null;
		    for (int i = 0; i < nTargetCount; i++ )
		    {
			    int idCharacter = paTargetIDs[i];
			    pCharacter = (CObject_Character)(CObjectManager.Instance.FindServerObject( idCharacter ));
			    if ( pCharacter != null)
			    {
				    pCharacter.ShowLogicEvent( ServerID, nLogicCount, true );
			    }
		    }
	    }
    }

    protected void DoDie()
    {
        if (GetSpecialObjData() != null && GetSpecialObjData().m_lpszEffect_Die != null && GetSpecialObjData().m_lpszEffect_Die.Length > 0)
        {
            ChangEffect(GetSpecialObjData().m_lpszEffect_Die, false);
        }

        SetSpecialState(ENUM_SPECIAL_STATE.SPECIAL_STATE_DIE);
    }

    public override bool IsEffectStopped()
    {
        if ( ENUM_SPECIAL_STATE.SPECIAL_STATE_DIE == GetSpecialState() )
	    {
		    return base.IsEffectStopped();
	    }
	    return false;
    }

	public ENUM_SPECIAL_STATE GetSpecialState( )
	{
		return m_eSpecialState;
	}

	public _DBC_SPECIAL_OBJ_DATA GetSpecialObjData()
	{
		return m_pSpecialObjData;
	}

	public virtual object GetCampData(){ return null; }
	public virtual int	GetOwnerID(){ return m_idOwner; }
	protected void SetSpecialState( ENUM_SPECIAL_STATE eState )
	{
		m_eSpecialState = eState;
	}

    protected void ReleaseCommandList()
    {
        m_listCommand.Clear();
    }
    protected SCommand_Object PopCommand()
    {
        if (m_listCommand.Count == 0)
        {
            return null;
        }
        SCommand_Object pResult = m_listCommand[0];
        m_listCommand.RemoveAt(0);
        return pResult;
    }

    protected SCommand_Object GetNextCommand()
    {
        if (m_listCommand.Count == 0)
        {
            return null;
        }
        return m_listCommand[0];
    }
};

