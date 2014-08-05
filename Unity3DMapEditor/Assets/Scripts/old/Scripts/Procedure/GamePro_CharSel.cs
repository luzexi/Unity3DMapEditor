/****************************************\
*										*
* 			   人物选择流程				*
*										*
\****************************************/
using Network;
using Network.Packets;

public class ModelShowInLoginUI
{


	//-----------------------------------------------------------------------------------------------------------------------
	//
	// 用于在ui界面上显示的信息.
	//
	public static int				m_TeamNumCount;					// 模型计数
	public CObject_PlayerOther	m_pAvatar=null;						// 用于UI显示的逻辑对象.
	public char[]					m_szBufModel = new char[128];				// 模型名字.

	public ModelShowInLoginUI(){

    }
	~ModelShowInLoginUI(){}


	// 创建新的模型
    public void CreateNewUIModel()
    {
        if (m_pAvatar != null)
        {
            DestroyUIModel();
        }
        m_pAvatar = CObjectManager.Instance.NewFakePlayerOther();
        m_pAvatar.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
    }

	// 删除ui模型
    public void DestroyUIModel() 
    {
        if (m_pAvatar != null)
        {
            CObjectManager.Instance.DestroyObject(m_pAvatar);
        }
        m_pAvatar = null;
    }

	// 设置模型信息
	public void SetUIModelInfo(HUMAN_EQUIP point, int nID)
    {
        if (m_pAvatar != null) 
        {
            m_pAvatar.GetCharacterData().Set_Equip(point, nID);
        }
    }

	
};

//
// 在界面上显示的模型.
//
//--------------------------------------------------------------------------------------------------------------------------------

public class GamePro_CharSel : GameProcedure
{

	//登录状态
	public enum PLAYER_CHARSEL_STATUS
	{
		CHARSEL_CHARSHOW,
		CHARSEL_SELDONE,
		CHARSEL_CONNNECT_GAME_SERVER_READY,	// 与游戏服务器的连接准备好, 可以连接.
	};

    const int MAX_SHOW_IN_UI = 3;

	// 切换流程是否清空界面。
	public bool m_bClearUIModel = true;

	// 清空角色数据
	public void ClearUIModel()
    {
        for (int i = 0; i < MAX_SHOW_IN_UI; i++)
        {
            m_Character[i].DestroyUIModel();
        }
        m_iCharacterCount = 0;
    }

	// 当前选中的要删除的角色
	public int	m_iCurSelRole=0;


	// 用户名
	public string m_strUserName;

	// 当前得到的角色的个数
	public int m_iCharacterCount;

	// 进入到游戏的guid
	public int	m_EnterGameGUID;	

	// 在ui上显示的模型
	ModelShowInLoginUI[] m_Character = new ModelShowInLoginUI[MAX_SHOW_IN_UI];
	DB_CHAR_BASE_INFO[]	m_CharacterInfo= new DB_CHAR_BASE_INFO[MAX_SHOW_IN_UI];

	// 添加一个人物
    public int AddCharacter(ref DB_CHAR_BASE_INFO CharacterInfo)
    {
        if (m_iCharacterCount < MAX_SHOW_IN_UI)
        {
            m_CharacterInfo[m_iCharacterCount].m_Ambit = CharacterInfo.m_Ambit;
            m_CharacterInfo[m_iCharacterCount].m_Camp = CharacterInfo.m_Camp;
            m_CharacterInfo[m_iCharacterCount].m_FaceColor = CharacterInfo.m_FaceColor;
            m_CharacterInfo[m_iCharacterCount].m_FaceModel = CharacterInfo.m_FaceModel;
            m_CharacterInfo[m_iCharacterCount].m_GUID = CharacterInfo.m_GUID;
            m_CharacterInfo[m_iCharacterCount].m_HairColor = CharacterInfo.m_HairColor;
            m_CharacterInfo[m_iCharacterCount].m_HairModel = CharacterInfo.m_HairModel;
            m_CharacterInfo[m_iCharacterCount].m_HeadID = CharacterInfo.m_HeadID;
            m_CharacterInfo[m_iCharacterCount].m_Level = CharacterInfo.m_Level;
            m_CharacterInfo[m_iCharacterCount].m_Menpai = CharacterInfo.m_Menpai;
            m_CharacterInfo[m_iCharacterCount].m_Name = CharacterInfo.m_Name;
            m_CharacterInfo[m_iCharacterCount].m_nClientResID = CharacterInfo.m_nClientResID;
            m_CharacterInfo[m_iCharacterCount].m_Sex = CharacterInfo.m_Sex;
            m_CharacterInfo[m_iCharacterCount].m_StartScene = CharacterInfo.m_StartScene;

            if(m_CharacterInfo[m_iCharacterCount].m_EquipList == null)
                m_CharacterInfo[m_iCharacterCount].m_EquipList = new uint[(int)HUMAN_EQUIP.HEQUIP_NUMBER];
            m_CharacterInfo[m_iCharacterCount].m_EquipList = CharacterInfo.m_EquipList;

            m_Character[m_iCharacterCount].CreateNewUIModel();
            int iRaceId = m_CharacterInfo[m_iCharacterCount].m_Sex;
            int iFaceMeshId = m_CharacterInfo[m_iCharacterCount].m_FaceModel;
            int iFaceHairId = m_CharacterInfo[m_iCharacterCount].m_HairModel;
            uint iHairColor = m_CharacterInfo[m_iCharacterCount].m_HairColor;
            int iMenPai = m_CharacterInfo[m_iCharacterCount].m_Menpai;
            _CAMP_DATA TempCampData;
            TempCampData.m_nPKModeID = (byte)PK_MODE.PK_MODE_PEACE;
            TempCampData.m_uActiveFlags = 0;
            TempCampData.m_uRelationFlags = 0;
            TempCampData.m_nCampID = m_CharacterInfo[m_iCharacterCount].m_Camp;

            //门派，五行
            m_Character[m_iCharacterCount].m_pAvatar.GetCharacterData().Set_MenPai(iMenPai);

            // 设置性别
            m_Character[m_iCharacterCount].m_pAvatar.GetCharacterData().Set_RaceID(iRaceId);

            // 设置脸的模型
            m_Character[m_iCharacterCount].m_pAvatar.GetCharacterData().Set_FaceMesh(iFaceMeshId);

            // 设置发型的模型.
            m_Character[m_iCharacterCount].m_pAvatar.GetCharacterData().Set_HairMesh(iFaceHairId);

            //头发颜色
            m_Character[m_iCharacterCount].m_pAvatar.GetCharacterData().Set_HairColor(iHairColor);

            //阵营
            m_Character[m_iCharacterCount].m_pAvatar.GetCharacterData().Set_CampData(TempCampData);

            //设置默认朝向为0 正对屏幕
            m_Character[m_iCharacterCount].m_pAvatar.SetFaceDir(0);
            m_Character[m_iCharacterCount].m_pAvatar.SetPosition(new UnityEngine.Vector3(0, 0, 0));
            setLoginPlayerTransform(m_Character[m_iCharacterCount].m_pAvatar);
            uint[] equipInfo = m_CharacterInfo[m_iCharacterCount].m_EquipList;
            for (int i = (int)HUMAN_EQUIP.HEQUIP_WEAPON; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
            {
                m_Character[m_iCharacterCount].SetUIModelInfo((HUMAN_EQUIP)i, (int)equipInfo[i]);
            }
            m_Character[m_iCharacterCount].m_pAvatar.Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
            m_iCharacterCount++;
            return 0;
        }
        else
        {
            return -1;
        }
    }

	// 切换到帐号输入界面
	public void ChangeToAccountInput()
    {
        // 关闭系统界面
	    s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_SYSTEM_INFO);

	    // 打开帐号输入界面
	    s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_OPEN_COUNT_INPUT);

	    // 关闭人物选择界面
	    s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_SELECT_CHARACTOR);

	    //切换到人物选择界面
	    SetActiveProc(s_ProcLogIn);
	    if(s_ProcLogIn != null)
	    {
		    s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_BEGIN_REQUESTING);
	    }

	    m_iCharacterCount = 0;
    }

	// 切换到创建人物界面
	public void ChangeToCreateRole()
    {
        if(s_ProcCharSel.GetCurRoleCount() >= 3)
		{
            s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "角色已经满了! ");	
			return;
		}
        // 打开人物创建界面
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_OPEN_CREATE_CHARACTOR);

	    // 关闭系统界面
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_SYSTEM_INFO);

	    // 关闭人物选择界面
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_SELECT_CHARACTOR);

	    //切换到人物选择界面
	    SetActiveProc(s_ProcCharCreate);

	    if(s_ProcCharCreate  != null)
	    {
		    s_ProcCharCreate.CallInit();
	    }

    }

	// 切换到服务器连接流程
	public void ChangeToServerConnect()
    {
        // 切换到连接game server流程
	    SetActiveProc(s_ProcChangeScene);

	    // 设置game server流程为断开状态
        s_ProcChangeScene.SetStatus(GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_DISCONNECT);
    }

	// 得到当前角色的个数
    public int GetCurRoleCount() { return m_iCharacterCount; }

	// 通过索引得到角色的信息
	public  DB_CHAR_BASE_INFO GetRoleInfo(int iIndex)
    {
        if((iIndex < MAX_SHOW_IN_UI)&&(iIndex >= 0))
	    {

		    return (m_CharacterInfo[iIndex]);
	    }
    	
	    return new DB_CHAR_BASE_INFO();
    }


	// 删除一个角色
    public int DelRole(int iRoleIndex) 
    {
        if((iRoleIndex < m_iCharacterCount)&&(iRoleIndex >= 0))
	    {
		    if(iRoleIndex < MAX_SHOW_IN_UI)
		    {
                CLAskDeleteChar msg = (CLAskDeleteChar)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_ASKDELETECHAR);

			    msg.GUID = (uint)m_CharacterInfo[iRoleIndex].m_GUID;

			    // 删除角色
			    NetManager.GetNetManager().SendPacket(msg);
			    s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, "正在删除角色.....");	
			    return 0;
    			
		    }
	    }
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "请选择一个角色");	
	    return 1;
    }

	// 删除选择的角色
    public int DelSelRole() 
    {
        return DelRole(m_iCurSelRole);
    }
    public void resetSel()
    {
        if (m_iCharacterCount>0)
        {
            SetCurSel(0);
        }
    }
	// 进入游戏
    public int SendEnterGameMsg(int iRoleIndex)
    {
         if((iRoleIndex < m_iCharacterCount)&&(iRoleIndex >= 0))
        {
            if(iRoleIndex < MAX_SHOW_IN_UI)
            {
                CLAskCharLogin msg = (CLAskCharLogin)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_ASKCHARLOGIN);
                

                m_EnterGameGUID = m_CharacterInfo[iRoleIndex].m_GUID;
                msg.CharGuid = m_EnterGameGUID;

                 
                NetManager.GetNetManager().SendPacket(msg);

                //LogManager.Log("Send CLAskCharLogin: " + iRoleIndex);

                s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, "准备进入游戏.....");	
                return 0;
            }
        }

        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "请选择一个角色");	
        return 1;
    }

	// 位置是否合法
    public bool IsValidIndex(int iRoleIndex) 
    {
        if (iRoleIndex < m_iCharacterCount && iRoleIndex >= 0)
        {
            return true;
        }
        else return false;
    }

	// 设置当前要的角色
    public void SetCurSel(int iIndex) 
    {
        m_iCurSelRole = iIndex;
        ShowRole(iIndex);
    }

    int m_iCurSelRoleDel;
    public void SetCurSelDel(int iIndex)
    {
	    m_iCurSelRoleDel = -1;
    }

	// 重置场景加载状态 [10/27/2010 Sun]
	public void SetSceneLoadStatus(bool bLoaded) {m_bIsLoadScene = bLoaded ;}


	// 设置用户名字
	public void SetUserName(string pUserName){}
	//显示角色  [8/25/2011 zzy]
	public void ShowRole(int iRoleIndex)
    {
        mCurrentRoleIndex = iRoleIndex;
        for (int index = 0; index < MAX_SHOW_IN_UI; ++index)
        {
            if (m_Character[index].m_pAvatar != null)
            {
                if (index == iRoleIndex)
                {
                    m_Character[index].m_pAvatar.Enable((int)ObjectStatusFlags.OSF_VISIABLE);
                }
                else
                    m_Character[index].m_pAvatar.Disalbe((int)ObjectStatusFlags.OSF_VISIABLE);

            }
        }
    }
	public virtual void RoteRole()
    {
        if (!s_isRote)
        {
            return;
        }
        if (mCurrentRoleIndex >= 0 && mCurrentRoleIndex < MAX_SHOW_IN_UI)
        {
            //旋转当前的角色  [8/26/2011 zzy]
            if (m_Character[mCurrentRoleIndex].m_pAvatar != null)
            {
                float rotY = m_Character[mCurrentRoleIndex].m_pAvatar.GetFaceDir();
                rotY += s_pTimeSystem.GetDeltaTime() / 1000.0f * s_roteSpeed;
                m_Character[mCurrentRoleIndex].m_pAvatar.SetFaceDir(rotY);
            }
        }
    }

	//处理输入
	public override void	ProcessInput(){}

    public PLAYER_CHARSEL_STATUS M_Status { 
        get{
            return this.m_Status;
        }
    }
	PLAYER_CHARSEL_STATUS	m_Status;

	bool m_bIsLoadScene;
	int mCurrentRoleIndex;

    public void SetStatus(PLAYER_CHARSEL_STATUS status) { m_Status = status; }

	//主角在渲染层中的接口
//	CObject*		m_pObject;


	public override void Init(){
        if (m_bClearUIModel)
        {
            ClearUIModel();
            m_bClearUIModel = false;
        }
        UseLoginCamera();
        resetSel();
    }
    public override void Tick(){
        base.Tick();

        RoteRole();
    }
    public override void Render(){}
    public override void Release(){
        ShowRole(-1);//隐藏所有可选择角色
    }
    public override void CloseRequest(){}


	public GamePro_CharSel(){
        m_Status = PLAYER_CHARSEL_STATUS.CHARSEL_CHARSHOW;
	    m_bIsLoadScene		= false;
	    m_iCharacterCount	= 0;

	    m_bClearUIModel = true;
        for (int i = 0; i < MAX_SHOW_IN_UI; i++)
        {
            m_Character[i] = new ModelShowInLoginUI();
        }
    }
	~GamePro_CharSel(){}
//     public override void OnGUI()
//     {
//         for(int i=0; i < GetCurRoleCount(); ++i)//角色选择UI
//         {
//             string charInfo = EncodeUtility.Instance.GetUnicodeString(GetRoleInfo(i).m_Name) + " " + GetRoleInfo(i).m_Level + "级";
//             if (UnityEngine.GUI.Button(new UnityEngine.Rect(100, 100 + i * 30, 120, 25), charInfo))
//             {
//                 SetCurSel(i);
//             }
//         }
//         //返回UI
//         if (UnityEngine.GUI.Button(new UnityEngine.Rect(50, 550, 80, 25), "返回登录"))
//         {
//             ChangeToAccountInput(); 
//         }
//         //进入游戏
//         if (UnityEngine.GUI.Button(new UnityEngine.Rect(400, 550, 80, 25), "进入游戏"))
//         {
//             SendEnterGameMsg(mCurrentRoleIndex);
//         }
//         //删除角色
//         if (UnityEngine.GUI.Button(new UnityEngine.Rect(500, 550, 80, 25), "删除角色"))
//         {
//             DelSelRole();
//             resetSel();
//         }
//         //创建角色UI
//         if (UnityEngine.GUI.Button(new UnityEngine.Rect(650, 550, 80, 25), "创建角色"))
//         {
//             ChangeToCreateRole();
//         }
//     }
};
