using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DBSystem;

public class SObject_BusInit : SObjectInit
{
	public int			m_nDataID;
    public int m_nPassengerCount;		// 乘客数目
    public uint[] m_anPassengerIDs = new uint[GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT];	// 乘客列表

	public SObject_BusInit()
	{
		m_nDataID			= MacroDefine.INVALID_ID;
		m_nPassengerCount	= 0;
		int i;
		for(i = 0; i < GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT; i++)
		{
			m_anPassengerIDs[i] = 0;
		}
	}

    public override void Reset()
	{
		m_nDataID			= MacroDefine.INVALID_ID;
		m_nPassengerCount	= 0;
		int i;
		for(i = 0; i < GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT; i++)
		{
			m_anPassengerIDs[i] = 0;
		}

		base.Reset();
	}
};

public class CObject_Bus : CObject_Dynamic
{
	public CObject_Bus()
    {
	    m_nDataID			= MacroDefine.INVALID_ID;
	    m_nPassengerCount	= 0;
	    int i;
	    for(i = 0; i < GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT; i++)
	    {
		    m_anPassengerIDs[i] = 0;
		    m_abPassengerAttacheds[i] = false;
	    }

	    m_nCurrentAnimationIndex = MacroDefine.INVALID_ID;

	    m_pBusData			= null;

	    m_fvMoveTargetPos	= new Vector3(-1f, -1f, -1f);
	    m_bMoving			= false;

    }

	public override void Initial(object pParam)
    {
	    base.Initial(pParam);

	    RemoveAllPassengers();

	    m_fvMoveTargetPos	= new Vector3(-1.0f, -1.0f, -1.0f);
	    m_bMoving			= false;

	    m_pBusData			= null;
	    m_nCurrentAnimationIndex = MacroDefine.INVALID_ID;

        SObject_BusInit pBusInit = pParam as SObject_BusInit;
	    if(pBusInit != null)
	    {
		    m_nDataID			= pBusInit.m_nDataID;
		    int i;
		    for(i = 0; i < pBusInit.m_nPassengerCount; i++)
		    {
			    AddPassenger(i, pBusInit.m_anPassengerIDs[i]);
		    }
	    }
	    else
	    {
		    m_nDataID			= MacroDefine.INVALID_ID;
	    }
	    UpdateData();

	    UpdateAnimation();
    }
    public override void Release()
    {
	    RemoveAllPassengers();
	    ReleaseRenderInterface();
	    m_nDataID			= MacroDefine.INVALID_ID;
	    m_pBusData			= null;
	    m_fvMoveTargetPos	= new Vector3(-1.0f, -1.0f, -1.0f);
	    m_bMoving			= false;
	    m_nCurrentAnimationIndex = MacroDefine.INVALID_ID;
	    
        base.Release();
    }
    public override void Tick()
    {
        base.Tick();

	    if(m_bMoving)
	    {
		    uint uElapseTime = GameProcedure.s_pTimeSystem.GetDeltaTime();
		    Tick_Move(uElapseTime);
	    }

	    UpdatePassengerAttached();
    }

	// 阴影大小
	public virtual float GetShadowRange()  { return 100.0f; }

    public override void SetPosition(Vector3 fvPosition)
    {
        base.SetPosition(fvPosition);

	    // 如果自己在bus上面，则更新bus的时候需要更新摄像机 [9/2/2011 Ivan edit]
// 	    if( CObjectManager::GetMe()->GetMySelf()->IsInBus(GetServerID()) )
// 	    {
// 		    if(CWorldManager::GetMe()->GetCameraAnimation() == INVALID_ID)
// 		    {
// 			    CGameProcedure::s_pGfxSystem->Camera_SetLookAt(fvPosition);
// 		    }
// 	    }

	    int i;
	    for(i = 0; i < m_nPassengerCount; i++)
	    {
		    if(m_anPassengerIDs[i] != 0)
		    {
			    CObject pObj = CObjectManager.Instance.FindServerObject((int)m_anPassengerIDs[i]);
			    if(pObj != null)
			    {
                    pObj.SetMapPosition(fvPosition.x,fvPosition.z);
				    pObj.SetFootPosition(fvPosition);
			    }
		    }
	    }
    }
    public void SetRotation(Vector3 fvRotation)
    {
// 	    m_fvRotation = fvRotation;
// 	    base.SetRotation( fvRotation );

    }
	// 压入一条指令
    public override bool PushCommand(SCommand_Object pCmd)
    {
	    switch((OBJECTCOMMANDDEF)pCmd.m_wID)
	    {
	    case OBJECTCOMMANDDEF.OC_BUS_MOVE:
		    {
			    float x = pCmd.GetValue<float>(0);
			    float y = pCmd.GetValue<float>(1);
			    float z = pCmd.GetValue<float>(2);

			    Vector3 fvGame =new Vector3(x, y, z);
			    StartMove(fvGame);
		    }
		    break;
	    case OBJECTCOMMANDDEF.OC_BUS_STOP_MOVE:
		    {
			    float x = pCmd.GetValue<float>(0);
			    float y = pCmd.GetValue<float>(1);
			    float z = pCmd.GetValue<float>(2);

			    Vector3 fvGame =new Vector3(x, y, z);

			    SetPosition(fvGame);
			    StopMove();
		    }
		    break;
	    case OBJECTCOMMANDDEF.OC_BUS_ADD_PASSENGER:
		    {
                int nIndex = pCmd.GetValue<int>(0);
                int nPassengerID = pCmd.GetValue<int>(1);

			    AddPassenger(nIndex, (uint)nPassengerID);
		    }
		    break;
	    case OBJECTCOMMANDDEF.OC_BUS_REMOVE_PASSENGER:
		    {
                int nPassengerID = pCmd.GetValue<int>(0);

			    RemovePassenger((uint)nPassengerID);
		    }
		    break;
	    case OBJECTCOMMANDDEF.OC_BUS_REMOVE_ALL_PASSENGER:
		    {
			    RemoveAllPassengers();
		    }
		    break;
	    default:
		    break;
	    }
	    return true;
    }

    bool StartMove(Vector3 pTargetPos)
    {
        if (pTargetPos == null)
        {
            return false;
        }

        m_fvMoveTargetPos = pTargetPos;
        m_fvSaveStartPos = GetPosition();

        m_fTargetDir = Utility.GetYAngle(new Vector2(GetPosition().x, GetPosition().z), 
                                new Vector2(m_fvMoveTargetPos.x, m_fvMoveTargetPos.z));
        if (m_fTargetDir < 0.0f)
        {
            m_fTargetDir = Mathf.PI * 2.0f + m_fTargetDir;
        }

        if (m_nCurrentAnimationIndex != (int)ENUM_BASE_ACTION.BASE_ACTION_N_RUN)
        {
            ChangeAction((int)ENUM_BASE_ACTION.BASE_ACTION_N_RUN, 1.0f, true);
        }

        m_bMoving = true;
        return true;
    }

    void StopMove()
    {
        m_bMoving = false;

        ChangeActionLoop(false);
    }

    bool Tick_Move(uint uElapseTime)
    {
        float fElapseTime = (float)uElapseTime / 1000.0f;
        float fMoveDist = fElapseTime * GetMoveSpeed();
        float fDist = Utility.GetDistance(GetPosition().x, GetPosition().z,
                                            m_fvMoveTargetPos.x, m_fvMoveTargetPos.z);
        if (fMoveDist >= fDist)
        {
            if (m_fvMoveTargetPos.y <= GameStruct_Bus.DEF_BUS_PATH_NODE_INVALID_Y)
            {
                SetMapPosition(m_fvMoveTargetPos.x, m_fvMoveTargetPos.z);
            }
            else
            {
                SetPosition(m_fvMoveTargetPos);
            }
            StopMove();
        }
        else
        {
            if (fDist > 0.0f)
            {
                Vector3 fvThisPos = GetPosition();
                Vector3 fvLen = m_fvMoveTargetPos - fvThisPos;
                if (m_fvMoveTargetPos.y <= GameStruct_Bus.DEF_BUS_PATH_NODE_INVALID_Y)
                {
                    fvThisPos.x += (fvLen.x / fDist) * fMoveDist;
                    fvThisPos.z += (fvLen.z / fDist) * fMoveDist;
                    SetMapPosition(fvThisPos.x, fvThisPos.z);
                }
                else
                {
                    fvThisPos.x += (fvLen.x / fDist) * fMoveDist;
                    fvThisPos.y += (fvLen.y / fDist) * fMoveDist;
                    fvThisPos.z += (fvLen.z / fDist) * fMoveDist;
                    SetPosition(fvThisPos);
                }
            }
        }

        float fFaceDir = GetFaceDir();
        if (Math.Abs(m_fTargetDir - fFaceDir) > 0.01f)
        {
            if (Math.Abs(m_fTargetDir - fFaceDir) < Mathf.PI * 5.0f / 4.0f
                && Math.Abs(m_fTargetDir - fFaceDir) > Mathf.PI * 3.0f / 4.0f)
            {
                SetFaceDir(m_fTargetDir);
            }
            else
            {
                float fElapseDir = fElapseTime * Mathf.PI;
                if (m_fTargetDir - fFaceDir > Mathf.PI
                    || (m_fTargetDir - fFaceDir < 0.0f && m_fTargetDir - fFaceDir > -Mathf.PI))
                {// 正转
                    fFaceDir -= fElapseDir;
                    if (fFaceDir < 0.0f)
                    {
                        fFaceDir += 2.0f * Mathf.PI;
                    }
                    SetFaceDir(fFaceDir);
                }
                else
                {// 反转
                    fFaceDir += fElapseDir;
                    if (fFaceDir > m_fTargetDir)
                    {
                        fFaceDir = m_fTargetDir;
                    }
                    SetFaceDir(fFaceDir);
                }
            }
        }

        return true;
    }

	Vector3		m_fvMoveTargetPos;
	Vector3		m_fvSaveStartPos;
	float			m_fTargetDir;
	bool			m_bMoving;

    public int GetMaxPassengerCount()
    {
        if (m_pBusData != null)
        {
            return m_pBusData.m_nMaxPassengerCount;
        }
        return 0;
    }
    public float GetMoveSpeed()
    {
        if (m_pBusData != null)
        {
            return m_pBusData.m_fMoveSpeed;
        }
        return 0.0f;
    }

	public int GetPassengerCount()
	{
		return m_nPassengerCount;
	}

    public bool IsPassenger(uint nObjID)
    {
        if (nObjID == 0)
            return false;

        int i;
        for (i = 0; i < m_nPassengerCount; i++)
        {
            if (nObjID == m_anPassengerIDs[i])
            {
                return true;
            }
        }
        return false;
    }

	public int GetDataID()
	{
		return m_nDataID;
	}

	public _BUS_INFO GetBusData()
	{
		return m_pBusData;
	}

    public int GetPassengerActionSetFileIndex(uint nObjID)
    {
        if (nObjID == 0)
            return -1;

        if (m_pBusData == null)
            return -1;

        int i;
        for (i = 0; i < m_nPassengerCount; i++)
        {
            if (nObjID == m_anPassengerIDs[i])
            {
                return m_pBusData.m_aSeatInfo[i].m_nActionSetIndex;
            }
        }
        return -1;
    }

    public void DetachCharacterInterface(uint nObjID)
    {
	    if(nObjID == 0)
		    return ;

	    int nCount = GetPassengerCount();
	    if(nCount > 0)
	    {
		    int i;
		    for(i = 0; i < nCount; i++)
		    {
			    if(m_anPassengerIDs[i] == nObjID)
			    {
				    if(m_abPassengerAttacheds[i] && GetRenderInterface() != null)
				    {
					    CObject pObj = CObjectManager.Instance.FindServerObject((int)m_anPassengerIDs[i]);
                        if (pObj != null && pObj.GetRenderInterface() != null)
					    {
						    GetRenderInterface().Detach_Object(pObj.GetRenderInterface());
						    pObj.SetMapPosition(pObj.GetPosition().x, pObj.GetPosition().z);//角色离开飞行坐骑后刷新自己的位置  [8/22/2011 zzy]
					    }
				    }
				    m_anPassengerIDs[i] = m_anPassengerIDs[nCount - 1];
				    m_abPassengerAttacheds[i] = m_abPassengerAttacheds[nCount - 1];
				    m_anPassengerIDs[nCount - 1] = 0;
				    m_abPassengerAttacheds[nCount - 1] = false;
				    return ;
			    }
		    }
	    }
    }

    bool AddPassenger(int nIndex, uint nPassengerID)
    {
        if (nPassengerID == 0 || nIndex < 0 || nIndex >= GetMaxPassengerCount())
        {
            return false;
        }

        if (m_anPassengerIDs[nIndex] != 0)
        {
            RemovePassenger(m_anPassengerIDs[nIndex]);
        }

        m_anPassengerIDs[nIndex] = nPassengerID;
        m_abPassengerAttacheds[nIndex] = false;
        m_nPassengerCount++;
        UpdateMemberRenderInterfaceByIndex(nIndex);
        return true;
    }
    bool RemovePassenger(uint nPassengerID)
    {
	    if(nPassengerID == 0)
		    return false;

	    int nCount = GetPassengerCount();
	    if(nCount > 0)
	    {
		    int i;
		    for(i = 0; i < nCount; i++)
		    {
			    if(m_anPassengerIDs[i] == nPassengerID)
			    {
				    if(m_abPassengerAttacheds[i] && GetRenderInterface() != null)
				    {
					    CObject pObj = CObjectManager.Instance.FindServerObject((int)m_anPassengerIDs[i]);
                        if (pObj != null && pObj.GetRenderInterface() != null)
					    {
                            CObject_Character player = pObj as CObject_Character;
                            if (player != null)
                                player.GetCharacterData().Set_BusObjID(0);
						    GetRenderInterface().Detach_Object(pObj.GetRenderInterface());
						    pObj.SetMapPosition(pObj.GetPosition().x, pObj.GetPosition().z);//角色离开飞行坐骑后刷新自己的位置  [8/22/2011 zzy]
					    }
				    }
				    m_anPassengerIDs[i] = m_anPassengerIDs[nCount - 1];
				    m_abPassengerAttacheds[i] = m_abPassengerAttacheds[nCount - 1];
                    m_anPassengerIDs[nCount - 1] = 0;
				    m_abPassengerAttacheds[nCount - 1] = false;
				    // 减少乘客 [8/26/2011 ivan edit]
				    --m_nPassengerCount;
				    return true;
			    }
		    }
	    }
	    return false;
    }
    void RemoveAllPassengers()
    {
	    int i;
	    for(i = 0; i < GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT; i++)
	    {
		    if(m_anPassengerIDs[i] != MacroDefine.INVALID_ID)
		    {
			    if(m_abPassengerAttacheds[i] && GetRenderInterface() != null)
			    {
				    CObject pObj = CObjectManager.Instance.FindServerObject((int)m_anPassengerIDs[i]);
                    if (pObj != null && pObj.GetRenderInterface() != null)
				    {
                        CObject_Character player = pObj as CObject_Character;
                        if (player != null)
                            player.GetCharacterData().Set_BusObjID(0);
					    GetRenderInterface().Detach_Object(pObj.GetRenderInterface());
					    pObj.SetMapPosition(pObj.GetPosition().x, pObj.GetPosition().z);//角色离开飞行坐骑后刷新自己的位置  [8/22/2011 zzy]
				    }
			    }
                m_anPassengerIDs[i] = 0;
			    m_abPassengerAttacheds[i] = false;
		    }
	    }
	    m_nPassengerCount = 0;
    }

    void UpdateMemberRenderInterfaceByIndex(int nIndex)
    {
	    if(m_anPassengerIDs[nIndex] == 0)
		    return;

	    if(m_abPassengerAttacheds[nIndex])
		    return;

	    if(GetRenderInterface() != null && m_pBusData != null && m_pBusData.m_aSeatInfo[nIndex].m_pszLocator.Length > 0)
	    {
		    CObject pObj = CObjectManager.Instance.FindServerObject((int)m_anPassengerIDs[nIndex]);
            if (pObj != null && pObj.GetRenderInterface() != null)
		    {
			    GetRenderInterface().Attach_Object(pObj.GetRenderInterface(),m_pBusData.m_aSeatInfo[nIndex].m_pszLocator);
                CObject_Character player = pObj as CObject_Character;
                if (player != null)
                    player.GetCharacterData().Set_BusObjID((uint)ServerID);
			    m_abPassengerAttacheds[nIndex] = true;
			    UpdateAnimation();
			    SetPosition(GetPosition());
		    }
	    }
    }

	int			m_nDataID;
	int			m_nPassengerCount;										// 乘客数目
	uint[]		m_anPassengerIDs= new uint[GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT];			// 乘客列表
	bool[]		m_abPassengerAttacheds = new bool[GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT];	// 乘客是否已经绑定到了车上


    public void ChangeAction(int nSetID, float fSpeed, bool bLoop/*, float fFuseParam = 0.3*/)
    {
        float fFuseParam = 0.3f;
        ChangeAction(nSetID, fSpeed, bLoop, fFuseParam);
    }

    public void ChangeAction(int nSetID, float fSpeed, bool bLoop, float fFuseParam)
    {
	    string lpszCharActionName = "";
	    if(GetRenderInterface()  != null)
	    {
		    lpszCharActionName = GetCharActionNameByActionSetID(nSetID);
		    if(lpszCharActionName == null || lpszCharActionName.Length == 0)
			    return;
	    }

	    m_nCurrentAnimationIndex = nSetID;

	    if(GetRenderInterface()  != null)
	    {
            GetRenderInterface().EnterSkill(true, lpszCharActionName, bLoop, fFuseParam);
            GetRenderInterface().ChangeActionRate(fSpeed);
	    }

	    int i;
	    for(i = 0; i < m_nPassengerCount; i++)
	    {
		    if(m_abPassengerAttacheds[i])
		    {
		        CObject_Character pObj = CObjectManager.Instance.FindServerObject((int)m_anPassengerIDs[i]) as CObject_Character;
                if (pObj != null && pObj.GetRenderInterface() != null)
                {
                    //pObj->Bus_ChangeAction(nSetID, fSpeed, bLoop, fFuseParam);
				    pObj.ChangeAction(nSetID, fSpeed, bLoop, fFuseParam);
                }
		    }
	    }

    }
    public void ChangeActionLoop(bool bLoop)
    {
        if (GetRenderInterface() != null)
        {
            GetRenderInterface().ChangeActionLoop(bLoop);
        }
    }
    public virtual string GetCharActionNameByActionSetID(int nActionSetID)
    {
	    int nCalcWeaponType = (int)ENUM_WEAPON_TYPE.WEAPON_TYPE_NPC;
	    if(m_pCharActionSetFile != null && nActionSetID != -1 && nCalcWeaponType >= 0 && nCalcWeaponType < DBC_DEFINE.MAX_WEAPON_TYPE_NUMBER)
	    {
		    _DBC_CHARACTER_ACTION_SET pActionSet = m_pCharActionSetFile.Search_Index_EQU(nActionSetID);
            if (pActionSet != null)
		    {
			    return pActionSet.pWeapon_Set[nCalcWeaponType];
		    }
	    }
        return null;
    }

    static bool _OnAnimationEnd(string szAnimationName, uint dwParam)
    {
	    //取得该Object
        CObject_Bus bus = CObjectManager.Instance.FindObject((int)dwParam) as CObject_Bus;

        if (bus != null)
		    //调用相应的对象函数
            return bus.OnAnimationEnd(szAnimationName);
	    else
		    //该对象已经销毁?
		    return false;
    }
    public virtual bool OnAnimationEnd(string szAnimationName)
    {
        UpdateAnimation();
        return true;
    }

    void UpdateAnimation()
    {
        if (m_bMoving)
        {
            ChangeAction((int)ENUM_BASE_ACTION.BASE_ACTION_N_RUN, 1.0f, true);
        }
        else
        {
            ENUM_BASE_ACTION nActIndex = ENUM_BASE_ACTION.BASE_ACTION_N_IDLE;
            int nRandValue = UnityEngine.Random.Range(0,100);
            if (nRandValue > 90)
            {
                nActIndex = ENUM_BASE_ACTION.BASE_ACTION_N_IDLE_EX0;
            }
            else if (nRandValue > 80)
            {
                nActIndex = ENUM_BASE_ACTION.BASE_ACTION_N_IDLE_EX1;
            }

            ChangeAction((int)nActIndex, 1.0f, true);
        }
    }

	int m_nCurrentAnimationIndex;

    void UpdateData()
    {
	    if(m_pBusData != null && m_pBusData.m_nDataID == m_nDataID)
		    return;

	    ReleaseRenderInterface();

	    m_pBusData = null;
	    if(m_nDataID != MacroDefine.INVALID_ID)
	    {
            DBC.COMMON_DBC<_BUS_INFO> allBus = 
                CDataBaseSystem.Instance.GetDataBase<_BUS_INFO>((int)DataBaseStruct.DBC_BUS_INFO);
            if (allBus == null)
            {
                LogManager.LogError("无法找到Bus配置表。");
                return;
            }
            m_pBusData = allBus.Search_Index_EQU(m_nDataID);
	    }

	    CreateRenderInterface();
    }
    void UpdatePassengerAttached()
    {
        if (GetRenderInterface() != null && m_pBusData != null)
        {
            int i;
            for (i = 0; i < m_nPassengerCount; i++)
            {
                if (!m_abPassengerAttacheds[i] && m_pBusData.m_aSeatInfo[i].m_pszLocator.Length > 0)
                {
                    UpdateMemberRenderInterfaceByIndex(i);
                }
            }
        }
    }
    void CreateRenderInterface()
    {
	    if(m_pBusData != null && m_pBusData.m_nModelID != MacroDefine.INVALID_ID)
	    {
            DBC.COMMON_DBC<_DBC_CHARACTER_MODEL> s_pCharModelDBC = 
                CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MODEL>((int)DataBaseStruct.DBC_CHARACTER_MODEL);
		    string lpszModelFileName = null;
		    _DBC_CHARACTER_MODEL pCharModel = s_pCharModelDBC.Search_Index_EQU(m_pBusData.m_nModelID);
		    if (pCharModel != null)
		    {
			    lpszModelFileName = pCharModel.m_pszModelName;
		    }

		    if (lpszModelFileName != null)
		    {
			    mRenderInterface = GFX.GFXObjectManager.Instance.createObject(lpszModelFileName, GFX.GFXObjectType.ACTOR);
                mRenderInterface.useTempAsset();//资源未加载完全时使用临时模型
			    mRenderInterface.SetData(ID);

			    //mRenderInterface.Actor_SetFile(lpszModelFileName);

			    setVisible(isVisible());

			    mRenderInterface.SetRayQuery(false);

                mRenderInterface.SetAnimationEndEvent(new GFX.OnAnimationEvent(OnAnimationEnd));

			    UpdatePassengerAttached();

			    UpdateCharActionSetFile();

			    SetPosition(GetPosition());
			    SetFaceDir( GetFaceDir() );
			    setVisible(isVisible());
		    }
	    }
    }
    public override void ReleaseRenderInterface()
    {
	    if(mRenderInterface != null)
	    {
            RemoveAllPassengers();
            GFX.GFXObjectManager.Instance.DestroyObject(mRenderInterface);
            mRenderInterface = null;
	    }
    }
    void UpdateCharActionSetFile()
    {
	    m_pCharActionSetFile	= null;

	    if(m_pBusData != null)
	    {
		    int nCharModelID	= m_pBusData.m_nModelID;
            
            DBC.COMMON_DBC<_DBC_CHARACTER_MODEL> s_pCharModelDBC = 
                CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MODEL>((int)DataBaseStruct.DBC_CHARACTER_MODEL);
		    if(nCharModelID != MacroDefine.INVALID_ID)
            {
                _DBC_CHARACTER_MODEL pCharModel = s_pCharModelDBC.Search_Index_EQU(m_pBusData.m_nModelID);
                if (pCharModel != null)
                {
                    string pszActionSetFileName = pCharModel.m_pszActionSetName_None;

				    if(pszActionSetFileName.Length > 0)
				    {
                        m_pCharActionSetFile = CActionSetMgr.Instance.GetActionSetFile(pszActionSetFileName);
				    }
			    }
		    }
	    }
    }
    
    protected DBC.COMMON_DBC<_DBC_CHARACTER_ACTION_SET> m_pCharActionSetFile;
	_BUS_INFO		m_pBusData;

};