
public class CScene 
{
    //public enum  { SIZE_OF_ZONE = 10  };	//Zone的大小

	//取得场景定义
    public _DBC_SCENE_DEFINE GetSceneDefine() { return m_pTheDefine; }
	//场景位置是否合法
	public bool	IsValidPosition(ref fVector2 fvPos){return true;}
	//根据逻辑坐标取得地形高度，考虑行走面，如果传入坐标非法，返回FALSE
	//virtual BOOL	GetMapHeight(const fVector2& fvMapPos, FLOAT& fHeight);
	//
	//加载不可行走区域Region数据
	//  1. 从二进制文件szRegionFile中读取所有Region定义，并贮存到m_theRegion结构中
	//  2. 根据每个Region的位置，将其注册到相应的CZone中
	//  3. 如果发生错误，则直接抛出异常
	//  上一个函数的二进制版本.
	public void			LoadRegionBinary(string szRegionFile){}

    //-
    // 现在没有Zone概念 [12/29/2011 Administrator]
	//得到指定网格，如果位置非法，返回NULL
    //CZone*			GetZone(INT nXPos, INT nZPos);
    ////根据世界X坐标得到所在的Zone的x坐标
    //INT				GetZoneX(FLOAT fx)const { return (INT)(fx/SIZE_OF_ZONE); }
    ////根据世界Z坐标得到所在的Zone的z坐标
    //INT				GetZoneZ(FLOAT fz)const { return (INT)(fz/SIZE_OF_ZONE); }

    ////得到大小
    //INT				GetSizeX(VOID) const { return m_nZoneXSize; }
    //INT				GetSizeZ(VOID) const { return m_nZoneZSize; }


	//控制背景音乐
	public void			ControlBkgSound(bool bEnable){}
	public void			ControlBkgSoundVol(){}
	//控制环境音
	public void			ControlEnvSound(bool bEnable){}
	public void			ControlEnvSoundVol(){}

	/***************************************************
		玩家城市相关
	****************************************************/
    //bool IsUserCity(void) const { return m_bUserCity; }
    ////玩家城市升级
    //bool UpdateUserCity(int nNewLevel);
    ////更新建筑物
    //bool UpdateCityBuilding(int nBuildingID, int nBuildingLevel);

	public bool IsFirstEnter() {return m_bFirstEnterScene;}
	public void SetFirstEnter(bool bFirst) { m_bFirstEnterScene = bFirst;}

	//场景初始化，加载地形，静态物品等
	public void	Initial(){

        //加载阻挡区域信息
        RegisteAllRegion();

        LoadNavMesh();

        LoadEnvSound();

       
    }
	public void	EnterScene(){

       if(mLoadSceneBehaviour)
           mLoadSceneBehaviour.EnterScene(m_pTheDefine.szWXObjectName);
    }
	public void	LeaveScene(){

        DestroyEnvSound();
        //清楚其他场景对象等信息
        m_bFirstEnterScene = false;
        m_pTheDefine = null;
        //删除CObject派生对象  [3/7/2012 ZZY]
        CObjectManager.Instance.Clear();

        // 删除老的导航数据 [3/14/2012 Ivan]
        NavMesh.PathFinder.Instance.NavMeshData = null;
    }
	public void	Tick(){

    }

    public string GetActiveSceneName(){
        return mSceneName;
    }


	//public CScene(const _DBC_SCENE_DEFINE* pSceneDef, bool bUserCity);
    public CScene(string name, LoadScene loadBehaviour){

        mSceneName = name;
        mLoadSceneBehaviour = loadBehaviour;
    }
    public CScene(_DBC_SCENE_DEFINE pSceneDef, LoadScene loadBehaviour)
    {
        m_pTheDefine = pSceneDef;
        mLoadSceneBehaviour = loadBehaviour;
    }
	~CScene(){

    }


	//将所有的Region注册到Zone
	void						RegisteAllRegion(){}

	void						LoadDijk(){}

	//环境音效
	bool						LoadEnvSound(){ return true;}
	//销毁所有环境音效
	void						DestroyEnvSound(){}

	// 加载导航网格 [3/21/2011 ivan edit]
	void						LoadNavMesh(){}

    // 场景加载 [12/29/2011 Administrator]
    LoadScene mLoadSceneBehaviour;

    string mSceneName;

	//场景定义结构, 从文件中读取
	_DBC_SCENE_DEFINE 	m_pTheDefine;

	//Zone数据
// 	INT							m_nZoneXSize;
// 	INT							m_nZoneZSize;
// 	std::vector< CZone >		m_theZoneBuf;	// 大小为 m_nZoneXSize*m_nZoneZSize

	//背景音乐
	//tSoundSource*				m_pBackSound;

	//环境音效列表
    //struct EnvSound_t
    //{
    //    INT				nID;
    //    INT				nSoundID;
    //    UINT			nPosx;
    //    UINT			nPosz;
    //    UINT			nDistance;
    //    tSoundSource*	pSoundSource;
    //};

	//std::vector<EnvSound_t>		m_pEnvSoundList;	

	bool	m_bFirstEnterScene;

	/***************************************************
		玩家城市相关
	****************************************************/
	bool m_bUserCity;	//是否是玩家城市

	//建筑物
    //struct BUILDING
    //{
    //    const _DBC_CITY_BUILDING*	pDefine;	//资源定义
    //    INT							nLevel;		//当前等级
    //    INT							nObjID;		//运行时实例ID
    //};

	//当前场景中建筑物(资源定义id为索引)
    //typedef std::map< INT, BUILDING >	BUILDING_REGISTER;
    //BUILDING_REGISTER				m_allBuilding;	//所有建筑物

//public:
//    //Region链表	
//    std::vector< CRegion >		m_theRegion;
//    // 建筑物行走面管理
//    CBuildingCollisionMng		m_WalkCollisionMng;

//    FLOAT*						m_Weights;
//    FLOAT*						m_Dist;
//    std::vector<fVector2>		m_Points;

//    // 导航网格 [3/21/2011 ivan edit]
//    WX::TriangleList triNavMesh;
};

