using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DBSystem;
using DBC;

public class GrowPointEditorScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        if (mDBCSystem == null)
        {
            mDBCSystem = CDataBaseSystem.Instance;
            mDBCSystem.Initial(DBStruct.s_dbToLoad, DBStruct.GetResources);
        }
        mGrowPointDBC = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_GROWPOINT>((int)DataBaseStruct.DBC_LIFEABILITY_GROWPOINT);
	    mAllGrwoPintInfo = new string[mGrowPointDBC.StructDict.Count];
        int j = 0;
        foreach(int i in mGrowPointDBC.StructDict.Keys)
        {
            _DBC_LIFEABILITY_GROWPOINT growPoint = mGrowPointDBC.Search_Index_EQU(i);
            mAllGrwoPintInfo[j] = growPoint.nID.ToString() + growPoint.szName;
            mIDToIndex.Add(i, j);
            mIndexToID.Add(j, i);
            j++;
        }
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

            if (rightButtonDown)//摆放模式
            {
                addGrowPoint(mIndexToID[mCurrentIndex], hitPos);
            }
            if(leftButtonDown)//选择模式
            {
                if( hitInfo.collider != null && mGoToActorMap.ContainsKey(hitInfo.collider.gameObject) )
                {
                    GFX.GfxActor actor = mGoToActorMap[hitInfo.collider.gameObject];
                    setActiveActor(actor);
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
    public void addGrowPoint(int id, Vector3 pos)
    {
        _DBC_LIFEABILITY_GROWPOINT growPoint = mGrowPointDBC.Search_Index_EQU(id);
        GFX.GfxActor actor = (GFX.GfxActor)GFX.GFXObjectManager.Instance.createObject(growPoint.szMeshFile, GFX.GFXObjectType.ACTOR);
        actor.position = pos;
        mAllGrowPoint.Add(actor,id);
        actor.setRenderInterfaceCreateEvt( new GFX.OnRenderInterfaceEvent(OnActorCreate) );
    }
    void deleteCurrent()
    {
        if(mActiveActor == null) return;
        mAllGrowPoint.Remove(mActiveActor);
        mGoToActorMap.Remove(mActiveActor.getGameObject());
        GFX.GFXObjectManager.Instance.DestroyObject(mActiveActor);
        setActiveActor(null);
    }
    void OnActorCreate(GFX.GfxActor actor)
    {
        if ( actor != null && actor.getGameObject() != null )
        {
             mGoToActorMap.Add(actor.getGameObject(), actor);
            setActiveActor(actor);
        }
        
    }
    void setActiveActor(GFX.GfxActor actor)
    {
        if(mActiveActor != null)
        {
            mActiveActor.SetMouseHover(false);
        }
        //UnityEditor.Selection.activeGameObject = GameObject.Find("SGEditor");
        mActiveActor = actor;
        if(mActiveActor != null)
        {
            mActiveActor.SetMouseHover(true);
            if(mAllGrowPoint.ContainsKey(mActiveActor))
            {
                int id = mAllGrowPoint[mActiveActor];
                mCurrentIndex = mIDToIndex[id];
            }
        }
        mGUIDirty = true;
    }
    public void clearAll()
    {
        mGoToActorMap.Clear();
        foreach (GFX.GfxActor actor in mAllGrowPoint.Keys)
        {
            GFX.GFXObjectManager.Instance.DestroyObject(actor);
        }
        mAllGrowPoint.Clear();
    }
    public void saveToFile(string path)
    {
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine("INT\tINT\tFLOAT\tFLOAT");
        sw.WriteLine("INT\tINT\tFLOAT\tFLOAT");
        int i=1;
        foreach(GFX.GfxActor actor in mAllGrowPoint.Keys)
        {
            if(actor.getGameObject() == null) continue;
            Vector3 pos = actor.getGameObject().transform.position;
            sw.WriteLine(i.ToString() + "\t" + mAllGrowPoint[actor].ToString() + "\t" + pos.x.ToString() + "\t" + pos.z.ToString());
            ++i;
        }
        sw.Close();
    }
    public string[] mAllGrwoPintInfo ;
    COMMON_DBC<_DBC_LIFEABILITY_GROWPOINT> mGrowPointDBC;
    Dictionary<int, int> mIDToIndex = new Dictionary<int,int>();
    Dictionary<int, int> mIndexToID = new Dictionary<int,int>();
    GFX.GfxActor mActiveActor = null;
    public int mCurrentIndex = 0;
    public bool mGUIDirty = true;
    public Dictionary<GFX.GfxActor, int> mAllGrowPoint = new Dictionary<GFX.GfxActor,int>();
    public Dictionary<GameObject, GFX.GfxActor> mGoToActorMap = new Dictionary<GameObject,GFX.GfxActor>();

    public CDataBaseSystem mDBCSystem;
}
