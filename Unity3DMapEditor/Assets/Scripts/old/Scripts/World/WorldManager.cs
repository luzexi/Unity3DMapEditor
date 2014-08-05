/****************************************\
*										*
*				游戏世界				*
*										*
\****************************************/
using System;
using System.Text;
using UnityEngine;
using Network;
using Network.Packets;

using DBSystem;
using System.Collections.Generic;
using Interface;


public class ActivePosManager
{

    public ActivePosManager()
    {
        s_pMe = this;
        m_nDistance = 25;
    }
    ~ActivePosManager() { }

    public static ActivePosManager GetMe() { return s_pMe; }

    public void Initial() { }
    public void Release() { }

    public void AddActivePos(bool bOnNPC, string pPosName, float fX, float fY, int nSceneID) { }
    public void AddFlashPos(bool bOnNPC, string pPosName, float fX, float fY, int nSceneID) { }

    public void update() { }



    protected static ActivePosManager s_pMe;
    // 	typedef std::list< MAP_POS_DEFINE >   POS_LIST;
    // 	POS_LIST   m_listActiveObj;
    // 	POS_LIST   m_listFlashObj;

    protected int m_nDistance;

    //public void   GetActiveList( int nSceneID, POS_LIST* pReturnList );
    //public void   GetFlashList( int nSceneID, POS_LIST* pReturnList );

    //private void   update( POS_LIST* pList );


};

public struct MAP_POS_DEFINE
{
    public Vector2 pos;
    public int dwSceneID; // 所在场景id
    public string name;
    public int nServerID;
    public byte otherData;  // 保存额外信息，比如任务类型等
}

public class WorldManager
{

    public enum WORLD_STATION
    {
        WS_NOT_ENTER,				//没有进入任何场景
        WS_ENTER_ING,				//正在进入一个场景(已经发送了CGEnterScene)
        WS_ENTER,					//成功进入一个场景
        WS_RELIVE,					//死亡复活
        WS_ASK_CHANGE_SCANE,		//要求切换到一个新的场景(已经发送了CGAskChangeScene)
    };
    /**
        进入某场景
        \param nSceneID	
            场景ID/玩家城市id
        \param nResID
            场景的客户端资源ID

        \param nCityLevel
            玩家城市等级，如果小于0，表示是普通场景!
    */
    public bool EnterScene(int nSceneID, int nResID, int nCityLevel)
    {
        //LogManager.Log("EnterScene(): int WorldManager");
        //必须在进入场景流程中
        if (GameProcedure.GetActiveProcedure() != GameProcedure.s_ProcEnter)
        {
            LogManager.Log("Must enter scene at ENTER procedure");
            return false;
        }

        _DBC_SCENE_DEFINE pSceneDef = null;

        //如果是玩家城市
        bool bUserCity = (nCityLevel >= 0);
        if (bUserCity)
        {
            //查找城市场景
            for (int i = 0; i < (int)DBC.COMMON_DBC<_DBC_SCENE_DEFINE>.DataNum; i++)
            {
                _DBC_SCENE_DEFINE pTempSceneDef = SceneDBC.Search_Index_EQU(i);

                //Id和等级相同
                if (pTempSceneDef.nSceneResID == nResID && pTempSceneDef.nCityLevel == nCityLevel)
                {
                    pSceneDef = pTempSceneDef;
                    break;
                }
            }
        }
        else
        {
            //查找场景定义
            pSceneDef = SceneDBC.Search_Index_EQU(nResID);
        }

        if (pSceneDef == null)
        {
            LogManager.LogError("(CWorldManager::EnterScene) Invalid scene ID:" + nResID);
            return false;
        }

        //首先离开现有的场景
        if (ActiveScene != null)
        {
            //相同的场景
            if (GetActiveSceneID() == nSceneID)
            {
                //LogManager.Log("Enter The Same Scene");
                return false;
            }

            // 移除寻路轨迹 [9/21/2011 Sun]
            //ClearAllFreeProjTex();

            //删除当前场景
            m_pActiveScene.LeaveScene();

            m_pActiveScene = null;
        }

        m_nActiveSceneID = (short)nSceneID;
        //创建新的场景
        m_pActiveScene = new CScene(pSceneDef, mLoadSceneBehaviour);

        //加载新的场景， 加载静态物体定义
        m_pActiveScene.Initial();

        ////进入场景
        m_pActiveScene.EnterScene();

        m_Station = WORLD_STATION.WS_ENTER;
        ////产生进入场景的事件
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SCENE_TRANSED, ActiveScene.GetSceneDefine().szSceneMap);

        return true;
    }

    //升级玩家城市
    public bool UpdateUserCity(int nNewLevel) { return false; }
    ///取得当前激活的场景
    public CScene ActiveScene
    {
        get { return this.m_pActiveScene; }
    }
    ///游戏过程中切换场景
    public void ChangeScene(short idTargetScene, short nSceneResID, ref fVector2 fvPos, int nDirection, byte bFlag)
    {
        if (GameProcedure.GetActiveProcedure() != GameProcedure.s_ProcMain)
        {
            //TDThrow("Must change scene at MAIN procedure");
            return;
        }

        //发送请求消息
        CGAskChangeScene msg = (CGAskChangeScene)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_ASKCHANGESCENE);
        msg.SourSceneID = GetActiveSceneID();
        msg.DestSceneID = idTargetScene;

        NetManager.GetNetManager().SendPacket(msg);


        //目标场景名
        //DBC_DEFINEHANDLE(s_pSceneDBC, DBC_SCENE_DEFINE);
        //const _DBC_SCENE_DEFINE* pSceneDef = 
        //    (const _DBC_SCENE_DEFINE*)s_pSceneDBC->Search_First_Column_Equ(
        //    _DBC_SCENE_DEFINE::SCENE_SERVER_ID_COLUMN, nSceneResID);

        ////通知UI,关闭相应窗口
        //CGameProcedure::s_pEventSystem->PushEvent(GE_PLAYER_LEAVE_WORLD, 
        //    pSceneDef ? pSceneDef->szName : "");

        _DBC_SCENE_DEFINE pSceneDef = m_SceneDBC.Search_Index_EQU(nSceneResID);
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PLAYER_LEAVE_WORLD, pSceneDef != null ? pSceneDef.szName : "");

        //---
        //保存状态
        m_idNextScene = idTargetScene;
        m_fvNextPos = fvPos;
        //	m_fNextDir = (FLOAT)((nDirection%36)*2.0f*TDU_PI/36.0f);
        // 	INT idDefaultSkill = CActionSystem::GetMe()->GetDefaultAction() ? CActionSystem::GetMe()->GetDefaultAction()->GetDefineID() : 0;
        // 
        // 	CGameProcedure::s_pVariableSystem->SetAs_Int("DefaultSkill",idDefaultSkill);
        //	CActionSystem::GetMe()->SaveAction();


        m_Station = bFlag == 1 ? WORLD_STATION.WS_RELIVE : WORLD_STATION.WS_ASK_CHANGE_SCANE;
    }
    ///取得当前状态
    public WORLD_STATION GetStation() { return m_Station; }
    //对外提供的地形高度查询接口(使用渲染系统坐标，考虑地形和行走面)
    //virtual FUNC_GETTERRAINHEIGHT			GetTerrainHeightFunc(void) { return _GetTerrainHeight; }
    //当前场景的ServerID
    public short GetActiveSceneID() { return m_nActiveSceneID; }

    //---- for debug
    //取得即将要去的场景ID
    public int GetNextSenceID() { return m_idNextScene; }
    public fVector2 GetNextScenePos() { return m_fvNextPos; }
    public float GetNextSceneFaceDir() { return m_fNextDir; }

    //bool	IsFirstEnter() {return m_pActiveScene->IsFirstEnter();}
    //void	SetFirstEnter(bool bFirst) {m_pActiveScene->SetFirstEnter(bFirst);}

    //public void	StartCameraAnimation(int nAnimation, bool bResume);
    //public void	ClearCameraAnimation();
    //public int	GetCameraAnimation() {return m_nCameraAnimationID;}
    //public static  void	HandlerEndCameraAnimation();

    //public void	StartPlayVideo(int nVideoID);
    //public static  void HandleStopPlayVideo();

    //public void   SetPathNodes(const CPath* pPath, bool bAutoFind);
    //public void   SetPathNodesDirty() {m_bMySelfPathDirty = true;}
    //public bool   IsPathNodesDirty() {return m_bMySelfPathDirty; }


    //节点初始化
    public void Initial()
    {

        mLoadSceneBehaviour = GameObject.Find("Main Camera").GetComponent<LoadScene>();

        // 初始化场景传送点信息 [3/13/2012 Ivan]
        InitSceneTransportData();

        GameProcedure.s_pEventSystem.RegisterEventHandle(GAME_EVENT_ID.GE_ON_SCENE_TRANS, OnSceneTransEvent);
        GameProcedure.s_pEventSystem.RegisterEventHandle(GAME_EVENT_ID.GE_ON_SERVER_TRANS, OnSceneTransEvent);
    }

    static bool alreadyLoad = false;
    void InitSceneTransportData()
    {
        // 初始化传送点数据 [8/3/2011 ivan edit]
        if (!alreadyLoad)
        {
            DBC.COMMON_DBC<_DBC_SCENE_POS_DEFINE> allPos =
                CDataBaseSystem.Instance.GetDataBase<_DBC_SCENE_POS_DEFINE>((int)DataBaseStruct.DBC_SCENE_POS_DEFINE);

            SceneTransportPath.Instance.Initailize(allPos.StructDict.Count * 2 + 1, GAMEDEFINE.MAX_SCENE);

            foreach (_DBC_SCENE_POS_DEFINE scene in allPos.StructDict.Values)
            {
                SceneTransferData data = new SceneTransferData();
                data.nSceneID = scene.nSceneID;
                data.xPos = scene.nXPos;
                data.yPos = scene.nZPos;
                data.nDestSceneID = scene.nDestSceneID;
                SceneTransportPath.Instance.AddSceneTransData(data);
            }

            alreadyLoad = true;
        }
    }

    //逻辑轮循函数
    public void Tick() { }
    //释放自己所所拥有的资源
    public void Release()
    {

        if (m_pActiveScene != null)
        {
            m_pActiveScene.LeaveScene();

            m_pActiveScene = null;
        }

        m_SceneDBC = null;
    }

    //virtual std::list< MAP_POS_DEFINE >* GetObjectListByClass( INT nType );
    //virtual void setMinimapSerachRange( int nRange ){ m_nSearchRange = nRange; };
    //virtual void MinimapNeedFresh();
    //virtual fVector3 GetMyselfPos();
    //virtual int GetMyselfDir();
    public virtual string GetSceneName( int nSceneID )
    {
       return "not impl";
    }


    // 用于获得寻路轨迹 [8/31/2011 Sun]
    //virtual int GetMoveNodesCount();
    //virtual fVector2 GetMoveNode(INT nIndex);
    //virtual int GetMoveNodeIndex();
    //virtual BOOL IsMoveStep(VOID);
    //public:
    //    typedef struct 
    //    {
    //        std::vector<CObject_ProjTex_MoveTrack*> vMoveTrack;
    //    }MoveTrackProjTex;
    //public:
    //    //系统级别场景转移事件处理
    //    static VOID	WINAPI	_OnSceneTransEvent(const EVENT* pEvent, UINT dwOwnerData);
    //    //对外提供的地形高度查询接口(使用渲染系统坐标，考虑地形和行走面)
    //    static bool WINAPI 	_GetTerrainHeight(float fX, float fZ, float& fY);
    //    //给GAME层提供的高度查询接口
    //    static bool WINAPI 	GetTerrainHeight_GamePos(float fX, float fZ, float& fY);
    //    //---------------------------------------------------------
    //    //变量截获函数，用于控制各种音效的开关
    //    static VOID	WINAPI	_OnVariableChangedEvent(const EVENT* pEvent, UINT dwOwnerData);

    // 事件系统没有实现，因此先手动调用 [12/29/2011 Administrator]
    public static void OnSceneTransEvent(GAME_EVENT_ID idEvent, List<string> vParam)
    {
        switch (idEvent)
        {
            case GAME_EVENT_ID.GE_ON_SCENE_TRANS:
                //转入进入场景流程
                {
                    // 在同一个服务器中切换场景.
                    GameProcedure.s_ProcEnter.SetStatus(GamePro_Enter.ENTER_STATUS.ENTERSCENE_READY);
                    GameProcedure.s_ProcEnter.SetEnterType((int)ENTER_TYPE.ENTER_TYPE_FROM_OTHER);

                    GameProcedure.SetActiveProc(GameProcedure.s_ProcEnter);
                }
                break;

            case GAME_EVENT_ID.GE_ON_SERVER_TRANS:
                {
                    GameProcedure.s_ProcChangeScene.SetStatus(GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_DISCONNECT);

                    // 切换服务器.
                    GameProcedure.SetActiveProc(GameProcedure.s_ProcChangeScene);
                }
                break;
        }
        GameProcedure.s_ProcLogIn.LoginForChangeScene();
    }


    public WorldManager()
    {
        ClearMapData();

        s_pMe = this;
    }
    ~WorldManager() { }
    public static WorldManager Instance {
        get
        {
            return s_pMe;
        }
    }
    static WorldManager s_pMe;

    //  [12/29/2011 Administrator]
    LoadScene mLoadSceneBehaviour;

    ///当前激活的场景
    public CScene m_pActiveScene = null;
    //	当前场景对应服务器ID [10/24/2011 Sun]
    short m_nActiveSceneID = -1;
    //当前状态
    WORLD_STATION m_Station;
    string m_strPath; //地图前缀path

    // 当前场景动画 [8/4/2011 Sun]
    int m_nCameraAnimationID;

    //场景数据表
    DBC.COMMON_DBC<_DBC_SCENE_DEFINE> m_SceneDBC;
    public DBC.COMMON_DBC<_DBC_SCENE_DEFINE> SceneDBC
    {
        get
        {
            if (m_SceneDBC == null)
                m_SceneDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_SCENE_DEFINE>((int)DataBaseStruct.DBC_SCENE_DEFINE);
            return m_SceneDBC;
        }
    }
    //--- for debug
    //即将要去的场景ID
    int m_idNextScene;
    //即将要去的场景位置
    fVector2 m_fvNextPos;
    //即将要去的场景方向
    float m_fNextDir;
    //--- for debug
    bool m_bAnimyNeedFresh;
    List<MAP_POS_DEFINE> m_AnimylistObj;		// 敌人列表
    public List<MAP_POS_DEFINE> AnimylistObj
    {
        get { return m_AnimylistObj; }
    }
    List<MAP_POS_DEFINE> m_ExpNPClistObj;		// NPC列表
    public List<MAP_POS_DEFINE> ExpNPClistObj
    {
        get { return m_ExpNPClistObj; }
    }
    List<MAP_POS_DEFINE> m_FriendlistObj;		// 队友列表
    public List<MAP_POS_DEFINE> FriendlistObj
    {
        get { return m_FriendlistObj; }
    }
    List<MAP_POS_DEFINE> m_PlayerlistObj;		// 别的玩家
    public List<MAP_POS_DEFINE> PlayerlistObj
    {
        get { return m_PlayerlistObj; }
    }
    List<MAP_POS_DEFINE> m_ExpObjlistObj;		// 特殊道具列表
    //List<MAP_POS_DEFINE> m_ActivelistObj;		// 激活点列表
    //List<MAP_POS_DEFINE> m_ScenePoslistObj;	// 场景点列表
    //List<MAP_POS_DEFINE> m_FlashlistObj;		// 激活点列表
    List<MAP_POS_DEFINE> m_PetlistObj;			// 宠物点列表
    List<MAP_POS_DEFINE> m_MisslistObj;			// 任务列表

    //std::vector<MoveTrackProjTex> m_MoveTrackProjTex; // 寻路轨迹 [9/5/2011 Sun]
    //std::list<CObject_ProjTex_MoveTrack*> m_FreeMoveTrackProjTex;

    // 需要存储寻路点 [9/1/2011 Sun]
    //std::vector< fVector2 > m_MySelfPath;
    //bool					m_bMySelfPathDirty;

    ActivePosManager m_pActiveManager;
    int m_nSearchRange;

    private bool CheckIsFriend(int id) { return false; }
    //private void    AddFriend( CObject_PlayerOther* pObj );

    //创建轨迹特效
    //private void	CreateMoveTrackProjTex();
    //private void	Tick_MoveTrack();
    //CObject_ProjTex_MoveTrack* GetFreeProjTex();
    //void	FreeMoveTrackProjTex(CObject_ProjTex_MoveTrack* pMoveTrack);
    //private void	FreeAllMoveTrackProjTex();
    //private void	ClearAllFreeProjTex();

    // 刷新地图信息 [3/19/2012 Ivan]
    public void UpdateMinimapData()
    {
        ClearMapData();

        // 更新队友列表
        //if(CUIDataPool::GetMe()->GetTeamOrGroup())
        //{
        //    for( int i = 0; i < CUIDataPool::GetMe()->GetTeamOrGroup()->GetTeamMemberCount(); i ++ )
        //    {
        //        const TeamMemberInfo* pInfo = CUIDataPool::GetMe()->GetTeamOrGroup()->GetMemberByIndex( i );
        //        if( pInfo )
        //        {
        //            /*if( pInfo->m_SceneID != m_pActiveScene->GetSceneDefine()->nID )
        //                continue;*/
        //            if( pInfo->m_OjbID == CObjectManager::GetMe()->GetMySelf()->GetServerID() )
        //                continue; 
        //            data.pos.x = pInfo->m_WorldPos.m_fX;
        //            data.pos.z = pInfo->m_WorldPos.m_fZ;
        //            _snprintf( data.name, 128, "%s", pInfo->m_szNick );
        //            data.nServerID = pInfo->m_OjbID;
        //            data.dwSceneID = pInfo->m_SceneID;
        //            m_FriendlistObj.push_back( data );
        //        }
        //    }
        //} 

        MAP_POS_DEFINE data;
        CObject_PlayerMySelf mySelf = CObjectManager.Instance.getPlayerMySelf();
        // 遍历所有的对象 [3/19/2012 Ivan]
        foreach (KeyValuePair<int, CObject> obj in CObjectManager.Instance.ServerObjects)
        {
            data = new MAP_POS_DEFINE();
            // 如果是角色 [3/19/2012 Ivan]
            if (obj.Value is CObject_Character)
            {
                CObject curObj = obj.Value;
                if (curObj == mySelf)
                    continue;
                ENUM_RELATION sCamp = GameInterface.Instance.GetCampType(curObj, mySelf);

                // 如果已经死了，就不加如列表了。
                if (((CObject_Character)curObj).CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD)
                    continue;
                if (((CObject_Character)curObj).GetFakeObjectFlag() == true)
                    continue;
                // 判断是不是宠物

                // 此处还有判断m_FlashlistObj和m_ActivelistObj的逻辑，没有使用，屏蔽 [3/19/2012 Ivan]

                data.pos.x = curObj.GetPosition().x;
                data.pos.y = curObj.GetPosition().z;
                data.name = ((CObject_Character)curObj).GetCharacterData().Get_Name();
                data.nServerID = ((CObject_PlayerNPC)curObj).ServerID;

                if (curObj is CObject_PlayerOther) // 如果是玩家
                {
                    if (sCamp == ENUM_RELATION.RELATION_FRIEND) // 如果是同一阵营的
                    {
                        // 如果是队友,就替掉本来已经存在的位置
                        // 						if( CheckIsFriend( pCurObj->GetServerID() ) )
                        // 						{
                        // 							AddFriend( (CObject_PlayerOther*)pCurObj );
                        // 							continue;
                        // 						}
                        m_PlayerlistObj.Add(data);
                    }
                    else if (sCamp == ENUM_RELATION.RELATION_ENEMY) // 敌人
                    {
                        m_AnimylistObj.Add(data);
                    }
                }
                else if (curObj is CObject_PlayerNPC) // 如果是npc
                {
                    //data.nServerID = ((CObject_PlayerNPC)curObj).GetCharacterData().Get_RaceID();
                    if (((CObject_PlayerNPC)curObj).IsDisplayBoard() == false)
                    {
                        continue;
                    }
                    // 如果是宠物
                    if (((CObject_PlayerNPC)curObj).GetNpcType() == ENUM_NPC_TYPE.NPC_TYPE_PET)
                    {
                        if (((CObject_PlayerNPC)curObj).GetOwnerId() == -1) // 还没有主人
                        {
                            m_PetlistObj.Add(data);
                        }
                    }
                    else if (sCamp == ENUM_RELATION.RELATION_FRIEND) // 如果是同阵营的，是npc,否则是敌人
                    {
                        // TODO 以后要判断该npc身上是否带了任务，有的话需要放到任务列表里面去 [3/19/2012 Ivan]
                        m_ExpNPClistObj.Add(data);
                    }
                    else if (sCamp == ENUM_RELATION.RELATION_ENEMY)
                    {
                        m_AnimylistObj.Add(data);
                    }
                }
            }
            else if (obj.Value is CTripperObject_Resource)
            {
                CTripperObject_Resource resource = obj.Value as CTripperObject_Resource;
                data.pos.x = resource.GetPosition().x;
                data.pos.y = resource.GetPosition().z;
                data.name = resource.GetLifeAbility().szName;
                data.nServerID = resource.ServerID;
                m_ExpObjlistObj.Add(data);
            }
            // 			else if( obj.Value is CTripperObject_Transport  )
            // 			{
            // 				data.pos.x = (( CObject*)pCurObj)->GetPosition().x;
            // 				data.pos.z = (( CObject*)pCurObj)->GetPosition().z;
            // 				_snprintf( data.name, 128, "传送点" );
            // 			}
        }
    }

    void ClearMapData()
    {
        if (m_AnimylistObj != null)
            m_AnimylistObj.Clear();
        else
            m_AnimylistObj = new List<MAP_POS_DEFINE>();

        if (m_ExpNPClistObj != null)
            m_ExpNPClistObj.Clear();
        else
            m_ExpNPClistObj = new List<MAP_POS_DEFINE>();

        if (m_FriendlistObj != null)
            m_FriendlistObj.Clear();
        else
            m_FriendlistObj = new List<MAP_POS_DEFINE>();

        if (m_PlayerlistObj != null)
            m_PlayerlistObj.Clear();
        else
            m_PlayerlistObj = new List<MAP_POS_DEFINE>();

        if (m_PetlistObj != null)
            m_PetlistObj.Clear();
        else
            m_PetlistObj = new List<MAP_POS_DEFINE>();

        if (m_MisslistObj != null)
            m_MisslistObj.Clear();
        else
            m_MisslistObj = new List<MAP_POS_DEFINE>();

        if (m_ExpObjlistObj != null)
            m_ExpObjlistObj.Clear();
        else
            m_ExpObjlistObj = new List<MAP_POS_DEFINE>();
    }
};

