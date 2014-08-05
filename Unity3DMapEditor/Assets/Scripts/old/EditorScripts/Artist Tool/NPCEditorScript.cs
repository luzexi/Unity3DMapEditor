using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DBSystem;
using DBC;
public class NPCEditorScript : MonoBehaviour {
    public static  bool NPCEditorScriptEnable = false;
	// Use this for initialization
	void Start () {
        NPCEditorScriptEnable = true;
		InitAI(); 
		if(mDBCSystem == null)
		{
			mDBCSystem  = CDataBaseSystem.Instance; 
        	mDBCSystem.Initial(DBStruct.s_dbToLoad, DBStruct.GetResources);
		}
         mMonsterTable =  mDBCSystem.GetDataBase<_DBC_CREATURE_ATT>((int)DataBaseStruct.DBC_CREATURE_ATT);
        int lineCount =  mMonsterTable.StructDict.Count;
         AllMonsterNpcNames = new string[lineCount];
         AllMonsterNpcID = new int[lineCount];
		int j=0;
        foreach(int i in mMonsterTable.StructDict.Keys )
        {
            _DBC_CREATURE_ATT monsterAttr =  mMonsterTable.Search_Index_EQU(i);
            if (monsterAttr != null)
            {
                 AllMonsterNpcNames[j] = monsterAttr.nID.ToString() + "=" + monsterAttr.pName;
                 AllMonsterNpcID[j] = monsterAttr.nID;
            }
            else
            {
                 AllMonsterNpcNames[j] = "none";
                 AllMonsterNpcID[j] = -1;
            }
            IDToPopupIndex[AllMonsterNpcID[j]]= j;
			j++;
        }
         mNPCType[0] = "Monster";
         mNPCType[1] = "NPC";
         mCurrentSelectMonsterIndex = 0;
		resetCurrentMonsterInfo();
	}
    void OnDestroy()
    {
        NPCEditorScriptEnable = false;
    }

	// Update is called once per frame
	void Update () 
    {
        bool leftButtonDown = Input.GetMouseButtonDown(0);
        bool rightButtonDown =  Input.GetMouseButtonDown(1);
        if (rightButtonDown || leftButtonDown)//鼠标左键或右键按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(ray, out hitInfo);
            if (!hit) return;
            Vector3 hitPos = hitInfo.point;
            if (rightButtonDown)//摆放怪物模式
            {
                mCurrentMonsterInfo.m_dwMonsterGUID = getGuid();
                AddActor(AllMonsterNpcID[mCurrentSelectMonsterIndex], mCurrentMonsterInfo, hitPos, Quaternion.identity);
            }
            if(leftButtonDown)//选择模式
            {
                if ( mActorPhyMap.ContainsKey(hitInfo.collider.gameObject))
                {
                    NotifyActiveActor( mActorPhyMap[hitInfo.collider.gameObject]);
                }
            }
        }

        //处理删除
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleteCurrent();
        }
       GFX.GfxSystem.Instance.Tick();
	}
    uint  getGuid()
    {
        //起的自1970.1.1日至今的秒数
        System.DateTime dt = new System.DateTime(1970, 1, 1);
        System.TimeSpan ts = System.DateTime.Now - dt;
        uint LocalTime = (uint)(0x00ffffff &  (ts.Ticks/1000000));
        string ipStr = UnityEngine.Network.player.ipAddress;//将IP字符转换到数字
        string[] arrayIP = ipStr.Split('.');
        uint ulIp = (uint)(Int32.Parse(arrayIP[0])*256*256*256 + Int32.Parse(arrayIP[1])*256*256 + Int32.Parse(arrayIP[2])*256 +  Int32.Parse(arrayIP[3]));
        uint dwId = ulIp << 24;
	    dwId = dwId | LocalTime;
        return dwId;
    }
    public void AddActor(int ID,CMonsterInstanceInfo theMonsterInfo,  Vector3 pos, Quaternion rot)
    {
        _DBC_CREATURE_ATT monsterAttr = mMonsterTable.Search_Index_EQU(ID);
        if (monsterAttr == null) return;
        _DBC_CHARACTER_MODEL CharModel = CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_MODEL>((int)DataBaseStruct.DBC_CHARACTER_MODEL).Search_Index_EQU(monsterAttr.nModelID);
        if(CharModel == null) return;
		GFX.GfxActor actor = (GFX.GfxActor)GFX.GFXObjectManager.Instance.createObject(CharModel.m_pszModelName, GFX.GFXObjectType.ACTOR);
        actor.position = pos;
        actor.rotation = rot;
        actor.setRenderInterfaceCreateEvt(new GFX.OnRenderInterfaceEvent(OnActorCreate));
        CMonsterInstanceInfo monsterInfo = new CMonsterInstanceInfo();
        CMonsterInstanceInfo.Copy(theMonsterInfo, monsterInfo);
        mAllActor.Add(actor, monsterInfo); 
    }
    public void clear()
    {
        mActorPhyMap.Clear();
        foreach (KeyValuePair<GFX.GfxActor, CMonsterInstanceInfo> kv in mAllActor )
        {
            GFX.GFXObjectManager.Instance.DestroyObject(kv.Key);
        }
        mAllActor.Clear();
        mCurrentActor = null;
        resetCurrentMonsterInfo();
    }
	void InitAI()
	{
        string dataPath = "Assets/Editor/data/";
        List<string> monsterAIString = new List<string>();
        StreamReader monsterAITable = new StreamReader(dataPath + "MonsterAITable.ini");
        string readLine = null;
        while ((readLine = monsterAITable.ReadLine()) != null)
        {
            if( readLine.Contains("DESC=") )
            {
                monsterAIString.Add(readLine);
            }
        }

        mBaseAIString = monsterAIString.ToArray();

        List<string> advanceAIString = new List<string>();
        StreamReader advanceAIStringSR = new StreamReader(dataPath + "AIScript.dat");
        readLine = null;
        while ((readLine = advanceAIStringSR.ReadLine()) != null)
        {
            if (readLine.Contains("="))
            {
                advanceAIString.Add(readLine);
            }
        }
        mAdvanceAIString = advanceAIString.ToArray();

        List<string> scriptString = new List<string>();
		scriptString.Add("-1=无脚本");
        StreamReader scriptStringSR = new StreamReader(dataPath + "Script.dat");
        readLine = null;
        while ((readLine = scriptStringSR.ReadLine()) != null)
        {
            if (readLine.Contains("="))
            {
                scriptString.Add(readLine); 
            }
        }
        mScriptFileString = scriptString.ToArray();
	}
    void OnActorCreate(GFX.GfxActor actor)
    {
        if (actor != null && actor.getGameObject() != null)
        {
             mActorPhyMap.Add(actor.getGameObject(), actor);
             NotifyActiveActor(actor);
        }
    }
    void NotifyActiveActor(GFX.GfxActor actor)
    {
       // UnityEditor.Selection.activeGameObject = GameObject.Find("SGEditor");
        CMonsterInstanceInfo MonsterInfo =  mAllActor[actor];
        CMonsterInstanceInfo.Copy(MonsterInfo,  mCurrentMonsterInfo);
        if (mCurrentActor != null) mCurrentActor.SetMouseHover(false);
        mCurrentActor = actor;
       // UnityEditor.Selection.activeGameObject = actor.getGameObject();
        if (mCurrentActor != null) mCurrentActor.SetMouseHover(true);
        mCurrentSelectMonsterIndex = mCurrentMonsterInfo.m_popupIndex;
        mInspectorDirty = true;
    }
    public  void resetCurrentMonsterInfo()
    {
         mCurrentMonsterInfo = new CMonsterInstanceInfo();
        if ( mMonsterTable == null) return;
        _DBC_CREATURE_ATT monsterAttr = mMonsterTable.Search_Index_EQU(AllMonsterNpcID[mCurrentSelectMonsterIndex]);
        if (monsterAttr == null) return;
        mCurrentMonsterInfo.m_popupIndex = mCurrentSelectMonsterIndex;
        mCurrentMonsterInfo.m_iCampId = monsterAttr.nCamp;
        mCurrentMonsterInfo.m_iLevel = monsterAttr.nLevel;
        mCurrentMonsterInfo.m_dwObjectId = (uint)AllMonsterNpcID[mCurrentSelectMonsterIndex];
    }
    public void deleteCurrent()
    {
        if (mCurrentActor == null) return;
        mAllActor.Remove(mCurrentActor);
        mActorPhyMap.Remove(mCurrentActor.getGameObject());
        GFX.GFXObjectManager.Instance.DestroyObject(mCurrentActor);
        mCurrentActor = null;
        resetCurrentMonsterInfo();
    }
    public void save()
    {
        if(mAllActor.ContainsKey(mCurrentActor))
        {
            CMonsterInstanceInfo monsterInfo = mAllActor[mCurrentActor];
            CMonsterInstanceInfo.Copy(mCurrentMonsterInfo, monsterInfo);
        }
    }
    public void saveToFile(string path)
    {
        StreamWriter sw = new StreamWriter(path);
        int actorCount = mAllActor.Count;
        sw.WriteLine("[info]");//ini头
        sw.WriteLine("monstercount=" + actorCount.ToString());
        int i=0;
        foreach (KeyValuePair<GFX.GfxActor, CMonsterInstanceInfo> kv in mAllActor )
        {
            GFX.GfxActor actor = kv.Key;
            if( actor== null || actor.getGameObject() == null ) continue;
            CMonsterInstanceInfo monsterInfo = kv.Value;
            sw.WriteLine("");
            sw.WriteLine("[monster" + i.ToString() + "]");//section
            sw.WriteLine("guid=" + monsterInfo.m_dwMonsterGUID.ToString());
            sw.WriteLine("type=" + monsterInfo.m_dwObjectId.ToString());
            sw.WriteLine("name=" + monsterInfo.m_strInstanceName);
            sw.WriteLine("title=" + monsterInfo.m_strTitleName);
            sw.WriteLine("pos_x=" + actor.getGameObject().transform.position.x.ToString());
            //sw.WriteLine("pos_y=" + actor.position.y.ToString());
            sw.WriteLine("pos_z=" + actor.getGameObject().transform.position.z.ToString());
            //float dir =  GFX.GfxUtility.GetYAngle(Vector3.zero, actor.getGameObject().transform.forward)*180.0f/3.14f;
            float dir = (actor.getGameObject().transform.rotation.eulerAngles.y / 180.0f) * 3.14f;
            int iDir  = (int)((dir / (6.28f)) * 36);
            iDir = iDir + 27;
            iDir = iDir % 36;
            sw.WriteLine("dir=" + iDir.ToString());
            sw.WriteLine("script_id=" + monsterInfo.m_EvenId.ToString());
            sw.WriteLine("respawn_time=" + monsterInfo.m_iRefreshTime.ToString());
            sw.WriteLine("group_id=" + monsterInfo.m_iGroupId);
            sw.WriteLine("team_id=" + monsterInfo.m_iTeamId);
            sw.WriteLine("base_ai=" + monsterInfo.m_iBaseAIId);
            sw.WriteLine("ai_file=" + monsterInfo.m_iAdvanceAIId);
            sw.WriteLine("patrol_id=" + monsterInfo.m_iLineid);
            sw.WriteLine("shop0=" + monsterInfo.m_iShopArray[0]);
            sw.WriteLine("shop1=" + monsterInfo.m_iShopArray[1]);
            sw.WriteLine("shop2=" + monsterInfo.m_iShopArray[2]);
            sw.WriteLine("shop3=" + monsterInfo.m_iShopArray[3]);
            sw.WriteLine("ReputationID=" + monsterInfo.m_iReputationID);
            sw.WriteLine("level=" + monsterInfo.m_iLevel);
            sw.WriteLine("npc=" + monsterInfo.m_iType);
            sw.WriteLine("camp=" + monsterInfo.m_iCampId);
            i++;
        }
        sw.Close();
    }
    public bool mInspectorDirty = false;
	public bool mPut = false;
    public CDataBaseSystem mDBCSystem;
    public string[] AllMonsterNpcNames;
    public int[] AllMonsterNpcID;
    public Dictionary<int, int> IDToPopupIndex = new Dictionary<int, int>();
    public int mCurrentSelectMonsterIndex = 0;
    public COMMON_DBC<_DBC_CREATURE_ATT> mMonsterTable;
    public CMonsterInstanceInfo mCurrentMonsterInfo;
    public GFX.GfxActor mCurrentActor;
    public string[] mNPCType = new string[2];
    public string[] mBaseAIString;
    public string[] mAdvanceAIString;
    public string[] mScriptFileString;

    public Dictionary<GFX.GfxActor, CMonsterInstanceInfo> mAllActor = new Dictionary<GFX.GfxActor,CMonsterInstanceInfo>();
    public Dictionary<GameObject, GFX.GfxActor> mActorPhyMap = new Dictionary<GameObject,GFX.GfxActor>();
}
public class CMonsterInstanceInfo
{
    public static int SHOP_COUNT =	4;
	public CMonsterInstanceInfo()
	{
		for(int i = 0; i < SHOP_COUNT; i++)
		{
			m_iShopArray[i] = -1;
		}
        m_iRefreshTime = 1000;
        m_ActiveType = 0;
        m_fAreaRadius = 2;

        m_dwObjectId = 0xffffffff;
        m_iBaseAIId = 0;
        m_iAdvanceAIId = 0;
        m_EvenId = -1;
        m_strAdvanceAIFile = "";
        m_strEvent = "";
        m_dwMonsterGUID = 0xffffffff;
        m_iGroupId = -1;
        m_iTeamId = -1;
        m_iReputationID = -1;
        m_iLevel = -1;
	    m_iLineid = -1;
	}
    public static void Copy(CMonsterInstanceInfo src, CMonsterInstanceInfo dest)
    {
        if(src == null || dest == null) return;
        dest.m_popupIndex = src.m_popupIndex;
        dest.m_iRefreshTime = src.m_iRefreshTime;
        dest.m_ActiveType = src.m_ActiveType;
        dest.m_dwObjectId = src.m_dwObjectId;
        dest.m_iBaseAIId = src.m_iBaseAIId;
        dest.m_iAdvanceAIId = src.m_iAdvanceAIId;
        dest.m_EvenId = src.m_EvenId;
        dest.m_strInstanceName = src.m_strInstanceName;
        dest.m_strTitleName = src.m_strTitleName;
        dest.m_strAdvanceAIFile = src.m_strAdvanceAIFile;
        dest.m_strEvent = src.m_strEvent;
        dest.m_dwMonsterGUID = src.m_dwMonsterGUID;
        dest.m_iGroupId = src.m_iGroupId;
        dest.m_iTeamId = src.m_iTeamId;
        dest.m_iReputationID = src.m_iReputationID;
        dest.m_iLevel = src.m_iLevel;
        dest.m_iType = src.m_iType;
        dest.m_iCampId = src.m_iCampId;
        dest.m_iLineid = src.m_iLineid;
        for(int i=0; i<SHOP_COUNT; ++i)
        {
            dest.m_iShopArray[i] = src.m_iShopArray[i];
        }
    }
    public int m_popupIndex;
	public int				m_iRefreshTime;			
	public int				m_ActiveType;			

	public float			m_fAreaRadius;			
	public int				m_iLineid;
	
	
	public ulong	m_dwObjectId;			
    public int				m_iBaseAIId;			
	public int             m_iAdvanceAIId;			
	public int             m_EvenId;			
	public string     m_strInstanceName="";		
	public string     m_strTitleName="";			

	public string		m_strAdvanceAIFile;		
	public string		m_strEvent;				
	public uint   m_dwMonsterGUID;		

	public int				m_iGroupId;				
	public int				m_iTeamId;				
	public int				m_iReputationID;		

	public int				m_iLevel;				
	public int				m_iType;				
	public int				m_iCampId;				

	public int[]             m_iShopArray = new int[SHOP_COUNT];

};