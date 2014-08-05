/****************************************\
*										*
* 			   人物创建流程				*
*										*
\****************************************/
using System;
using System.Text;

using Network;
using Network.Packets;

public class GamePro_CharCreate : GameProcedure
{
	

	//登录状态
	public enum PLAYER_CHAR_CREATE_STATUS
	{
		CHAR_CREATE_CREATE_OK = 0 ,	// 创建角色成功
		CHAR_CREATE_CREATEING,		// 创建角色
	};


    PLAYER_CHAR_CREATE_STATUS	status;
    public PLAYER_CHAR_CREATE_STATUS Status {
        get { return this.status; }
        set { status = value; }

    }
	public void					CallInit(){ Init(); }

    CObject_PlayerOther[]	m_pAvatar;						// 用于UI显示的逻辑对象.
    //TCHAR					m_szBufModelName1[128];				// 女模型名字.
    //TCHAR					m_szBufModelName2[128];				// 男模型名字.


    string					m_szCreateRoleName;			// 要创建的角色的名字
    public string CreateRoleName
    {
        get { return m_szCreateRoleName; }
        set { m_szCreateRoleName = value; }
    }
    //int						m_iFaceColor1;						// 设置面部颜色
    //int 					m_iFaceModle1;						// 设置面部模型
    //int 					m_iHairColor1;						// 设置头发颜色
    int 					m_iHairModle1;						// 设置头发模型
    int 					m_iSex;								// 设置性别
    public int Sex
    {
        get { return m_iSex; }
        set { m_iSex = value; }
    }
    int						m_iFaceId;							// 设置头像
    public short				m_iCamp;
    int m_iMenpai;
    public int Menpai
    {
        get { return m_iMenpai; }
        set { m_iMenpai = value; }
    }
    string[] mSexString;
    string[] mMenpaiString;
    string[] mHairStyleString;
    string[] mCampString;

    // 最大主角的头像头发数量，因为男女都在一张表里面，所以用这个做偏移 [3/30/2012 Ivan]
    public const int MaxMaleCount = 9;
	// 创建角色
    public int CreateRole()
    {
        
        string szInvalid = " #\\/`~!@~$%^&*()-_+=|{}[];:'\"<>,.?";
	    string szHZBlank = "　"; //中文空格
    
	    if(m_szCreateRoleName.Length == 0)
	    {
		    s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, "名字不能为空");	
		    return 1;
	    }
        
	    /*//查找是否有非法ascii字符
	    STRING::size_type nInvalidPos = strName.find_first_of(szInvalid);
	    STRING strInvalidChar;
	    if(nInvalidPos == STRING::npos)
	    {
		    //中文空格
		    nInvalidPos = strName.find(szHZBlank);
		    strInvalidChar = szHZBlank;
	    }
	    else
	    {
		    //非法字符
		    strInvalidChar = strName.substr(nInvalidPos, 1);
	    }

	    if(nInvalidPos != STRING::npos)
	    {
		    STRING strTemp = "";
		    strTemp = NOCOLORMSGFUNC("ERRORSpecialString");
		    CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, strTemp.c_str());	
		    return 0;
	    }

	    if(CGameProcedure::s_pUISystem)
	    {
		    if(!CGameProcedure::s_pUISystem->CheckStringFilter(strName)||(!CGameProcedure::s_pUISystem->CheckStringFullCompare(strName)))//LM修改
		    {
			    STRING strTemp = "";
			    strTemp = NOCOLORMSGFUNC("ERRORSpecialString");
			    CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, strTemp.c_str());
			    return 0;
		    }
	    }*/
        CLAskCreateChar msg = (CLAskCreateChar)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_ASKCREATECHAR);

        Random rdm = new Random((int)GameProcedure.s_pTimeSystem.GetTickCount());
        byte[] temp = EncodeUtility.Instance.GetGbBytes(m_szCreateRoleName);
        Array.Copy(temp, msg.Name, temp.Length);
        msg.FaceColor = 0;    //设置面部颜色
        msg.FaceModel = 0;    //设置面部模型
        msg.HairColor = 0;    //设置头发颜色
        msg.HairModel = (byte)(m_iHairModle1 + MaxMaleCount * (1 - m_iSex));    //设置头发模型
        msg.Sex = (byte)(m_iMenpai * 2 + m_iSex);		  //设置性别
        msg.HeadID = (byte)(m_iFaceId + MaxMaleCount * (1 - m_iSex));		  //设置头像id 
        msg.GUID = 0;		  //目前支持角色guid客户端计算，以后改为Login 
        msg.Camp = m_iCamp;		  //设置阵营
        msg.MenPai = (short)m_iMenpai;		  //设置门派

        NetManager.GetNetManager().SendPacket(msg);
        s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, "正在操作中");	
        return 0;
    }


	// 切换到角色选择界面
    public void ChangeToRoleSel() 
    {
        // 显示人物选择界面
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_OPEN_SELECT_CHARACTOR);

	    // 关闭系统界面
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_SYSTEM_INFO);

	    // 关闭人物创建界面
        s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_CREATE_CHARACTOR);
		
	    //切换到人物选择界面
	    SetActiveProc(s_ProcCharSel);
    }


	//////////////////////////////////////////////////////////////////////////////
	//
	//
	// 头像管理器.
	//CCharacterFaceMng	m_FaceMng;

	// 是否读取头像信息.
	public int					m_iIsLoadFace;

	// 设置头像id
	public void	SetFace(int iFaceId)
    {
        m_iFaceId = iFaceId;
    }	

	// 通过性别和索引设置头像id
	public void SetFaceByRaceAndIndex(int iRace, int iIndex){}	

	/////////////////////////////////////////////////////////////////////////////
	//
	// 
	//脸形信息管理器
	//CFaceModelMng		m_FaceModelmng;

	// 是否读取脸形模型信息.
	public int					m_iIsLoadFaceModel;

	// 设置脸形id
	public void	SetFaceModel(int iFaceModelId){}

	// 通过性别和索引设置头像id
	public void SetFaceModelByRaceAndIndex(int iRace, int iIndex){}	


	////////////////////////////////////////////////////////////////////////////
	//
	//
	// 发型信息管理器
	//CHairModelMng		m_HairModelMng;

	// 是否读取发形模型信息.
	public int					m_iIsLoadHairModel;

	// 设置脸形id
    public void SetHairModel(int iHairModelId)
    {
        m_iHairModle1 = iHairModelId;
    }

	// 通过性别和索引设置头像id
	public void SetHairModelByRaceAndIndex(int iRace, int iIndex){}	

	//通过性别和颜色值来设置发色
	public void SetHairColor(int iRace, int hairColor){}

	//设置五行
	public void SetMenPai(int menpai){}


    public override void Init()
    {
        status = PLAYER_CHAR_CREATE_STATUS.CHAR_CREATE_CREATEING;
        m_iSex = 0;
        m_iHairModle1 = -1;
        m_iMenpai = 1;
        m_iCamp = 0;
        Release();
        CreateModel();
        OnRoleChanged();
        UseLoginCamera();
    }
    public override void Tick()
    {
        base.Tick();
        RoteRole();
    }
    public override void Render(){}
    public override void Release()
    {
        if (m_pAvatar[0] != null)
        {
            CObjectManager.Instance.DestroyObject(m_pAvatar[0]);
            m_pAvatar[0] = null;
        }
        if (m_pAvatar[1] != null)
        {
            CObjectManager.Instance.DestroyObject(m_pAvatar[1]);
            m_pAvatar[1] = null;
        }
    }
    //public override void MouseInput() { }
	//处理输入
    public override void ProcessInput(){}


	public GamePro_CharCreate(){
        m_pAvatar = new CObject_PlayerOther[2];

        m_szCreateRoleName = "";
        mSexString = new string[2];
        mSexString[0] = "女";
        mSexString[1] = "男";
        mMenpaiString = new string[4];//4个职业
        mMenpaiString[0] = "职业1";
        mMenpaiString[1] = "职业2";
        mMenpaiString[2] = "职业3";
        mMenpaiString[3] = "职业4";

        mHairStyleString = new string[4];//4种发型
        mHairStyleString[0] = "发型1";
        mHairStyleString[1] = "发型2";
        mHairStyleString[2] = "发型3";
        mHairStyleString[3] = "发型4";

        mCampString = new string[3];//3个阵营
        mCampString[0] = "阵营1";
        mCampString[1] = "阵营2";
        mCampString[2] = "阵营3";
    }
	~GamePro_CharCreate(){
        Release();
    }

	// 创建界面模型
    public bool CreateModel() 
    {
        m_pAvatar[0] = CObjectManager.Instance.NewFakePlayerOther();
        m_pAvatar[0].Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);

        m_pAvatar[1] = CObjectManager.Instance.NewFakePlayerOther();
        m_pAvatar[1].Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
        // 女模型
        m_pAvatar[0].SetFaceDir(0);
        m_pAvatar[0].GetCharacterData().Set_RaceID(0);

        // 男模型 
        m_pAvatar[1].SetFaceDir(0);
        m_pAvatar[1].GetCharacterData().Set_RaceID(1);

        setLoginPlayerTransform(m_pAvatar[0]);
        setLoginPlayerTransform(m_pAvatar[1]);
        return true; 
    }

	// 切换到选择人物界面
	public void ChangeToSelectRole(){}
	public void RoteRole()
    {
        if (!s_isRote)
        {
            return;
        }
        for (int i = 0; i < 2; ++i)
        {
            if (m_pAvatar[i]!= null && m_pAvatar[i].IsEnable((int)ObjectStatusFlags.OSF_VISIABLE))
            {
                float rotY = m_pAvatar[i].GetFaceDir();
                rotY += s_pTimeSystem.GetDeltaTime() / 1000.0f * s_roteSpeed;
                m_pAvatar[i].SetFaceDir(rotY);
            }
        }
    }
    public void OnRoleChanged()
    {
        m_pAvatar[m_iSex].Enable((uint)ObjectStatusFlags.OSF_VISIABLE);
        m_pAvatar[1-m_iSex].Disalbe((uint)ObjectStatusFlags.OSF_VISIABLE);
        m_pAvatar[m_iSex].GetCharacterData().Set_HairMesh(m_iHairModle1 + MaxMaleCount * (1 - m_iSex));
        m_pAvatar[m_iSex].GetCharacterData().Set_MenPai(m_iMenpai);
        m_pAvatar[m_iSex].GetCharacterData().Set_RaceID(m_iMenpai * 2 + m_iSex);
    }
//     public override void OnGUI()
//     {
//         int y = 20;
//         for (int i = 0; i < mSexString.Length; ++i )
//         {
//             y += 30;
//             if (UnityEngine.GUI.Button(new UnityEngine.Rect(100,y , 50, 25), mSexString[i]))
//             {
//                 m_iSex = i;
//                 OnRoleChanged();
//             }
//         }
//         y += 20;
//         for (int i = 1; i <= mHairStyleString.Length; ++i )
//         {
//             y += 30;
//             if (UnityEngine.GUI.Button(new UnityEngine.Rect(100, y, 50, 25), mHairStyleString[i-1]))
//             {
//                 m_iHairModle1 = i;
//                 OnRoleChanged();
//             }
//         }
// 
//         y += 20;
// 
//         for (int i = 0; i < mMenpaiString.Length; ++i )
//         {
//             y += 30;
//             if (UnityEngine.GUI.Button(new UnityEngine.Rect(100, y, 50, 25), mMenpaiString[i]))
//             {
//                 m_iMenpai = i;
//                 OnRoleChanged();
//             }
//         }
// 
//         y += 20;
//         for (int i = 0; i < mCampString.Length; ++i)
//         {
//             y += 30;
//             if (UnityEngine.GUI.Button(new UnityEngine.Rect(100, y, 50, 25), mCampString[i]))
//             {
//                 m_iCamp = (short)i;
//                 OnRoleChanged();
//             }
//         }
//         //进入游戏
//         m_szCreateRoleName = UnityEngine.GUI.TextField(new UnityEngine.Rect(500, 550, 80, 25), m_szCreateRoleName);
//         
//         //删除角色
//         if (UnityEngine.GUI.Button(new UnityEngine.Rect(600, 550, 80, 25), "创建角色"))
//         {
//             if (m_szCreateRoleName.Length>0)
//             {
//                 CreateRole();
//             } 
//         }
// 
//         //返回
//         if (UnityEngine.GUI.Button(new UnityEngine.Rect(100, 550, 80, 25), "返回"))
//         {
//             ChangeToRoleSel();
//         }
//     }
};
