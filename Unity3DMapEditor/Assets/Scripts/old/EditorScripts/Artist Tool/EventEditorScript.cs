using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
public class EventEditorScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}
	// Update is called once per frame
	void Update () 
    {
		RaycastHit hitInfo;
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		if( Physics.Raycast(ray, out hitInfo, Mathf.Infinity) == false )
			return;
		
        bool leftButtonDown = Input.GetMouseButtonDown(0);
        bool rightButtonDown = Input.GetMouseButtonDown(1);
		
        if (rightButtonDown)//create area
        { 
            if(AreaBeginning)
			{
                if (mBeginGo != null)
                    UnityEngine.Object.DestroyImmediate(mBeginGo);
				AreaBeginning = false;
				createArea(mEventAreaBegin, hitInfo.point);

			}
			else
			{
				AreaBeginning = true;
				mEventAreaBegin = hitInfo.point;
                mBeginGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mBeginGo.transform.position = mEventAreaBegin;
                mBeginGo.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			}
        }
		
		if(leftButtonDown)//select area
		{
            if (hitInfo.collider != null && hitInfo.collider.gameObject != null)
            {
                setSelect(hitInfo.collider.gameObject);
            }
		}

        //´¦ÀíÉ¾³ý
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleteCurrent();
        }
	}
	public void createArea(Vector3 point1, Vector3 point2)
	{
		GameObject boxGo =  GameObject.CreatePrimitive(PrimitiveType.Cube);
        boxGo.name = "EventArea";
		Vector3 boxCenter = 0.5f*(point1 + point2);
		boxGo.transform.position = boxCenter;
		Vector3 boxSize = point1-point2;
		boxSize.x = Mathf.Abs(boxSize.x);
		boxSize.y = Mathf.Abs(boxSize.y)+0.1f;
		boxSize.z = Mathf.Abs(boxSize.z);
		boxGo.transform.localScale = boxSize;
        BoxCollider boxCollider = (BoxCollider)boxGo.collider;
		boxGo.AddComponent<Rigidbody>();
        boxGo.rigidbody.isKinematic = true;
		EventInfo info = new EventInfo();
		EventInfo.copy(mCurrentEventInfo, info);
		mAreaMap.Add(boxGo, info);
		setSelect(boxGo);
	}
    public void clearAll()
    {
        foreach (GameObject go in mAreaMap.Keys)
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
        mSelectedGO = null;
        mAreaMap.Clear();
    }
	void setSelect(GameObject go)
	{
        if (!mAreaMap.ContainsKey(go)) return;
        // UnityEditor.Selection.activeGameObject = GameObject.Find("SGEditor");
		if(mSelectedGO != null && mSelectedGO.renderer != null)
		{
			mSelectedGO.renderer.material.color = Color.white;
		}

		mSelectedGO = go;
        if (mSelectedGO != null)
        {
            EventInfo.copy(mAreaMap[mSelectedGO], mCurrentEventInfo);
        }

		if(mSelectedGO != null&& mSelectedGO.renderer != null)
		{
			mSelectedGO.renderer.material.color = Color.green;
		}
	}
    public void modifyCurrent()
    {
        if (mSelectedGO != null)
        {
            EventInfo.copy( mCurrentEventInfo, mAreaMap[mSelectedGO] );
            
        }
    }

	public void deleteCurrent()
	{
		if(mSelectedGO != null)
		{
			mAreaMap.Remove(mSelectedGO);
            UnityEngine.Object.DestroyImmediate(mSelectedGO);
			mSelectedGO = null;
		}
	}
    public void saveToFile(string path)
    {
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine("[area_info]");
        sw.WriteLine("area_count=" + mAreaMap.Count.ToString());
        int i = 0;
        foreach(GameObject go in mAreaMap.Keys)
        {
            EventInfo info = mAreaMap[go];
            sw.WriteLine("[area" + i.ToString() + "]");
            sw.WriteLine("guid=" + info.mGuid);
            sw.WriteLine("script_id=" + info.mScript_id) ;
            sw.WriteLine("left=" + go.renderer.bounds.min.x);
            sw.WriteLine("top=" + go.renderer.bounds.min.z);
            sw.WriteLine("right=" + go.renderer.bounds.max.x);
            sw.WriteLine("bottom=" + go.renderer.bounds.max.z);
            sw.WriteLine("transSceneId=" + info.mTransSceneId);
            sw.WriteLine("transX=" + info.mTransX);
            sw.WriteLine("transY=" + info.mTransY);
            sw.WriteLine("transMinLevel=" + info.mTransMinLevel);
            sw.WriteLine("transMaxLevel=" + info.mTransMaxLevel);
            ++i;
        }
        sw.Close();
    }
    Vector3 mEventAreaBegin;
    bool AreaBeginning = false;
    GameObject mBeginGo = null;
	Dictionary<GameObject, EventInfo> mAreaMap = new  Dictionary<GameObject, EventInfo>();
    GameObject mSelectedGO = null;
	public EventInfo	mCurrentEventInfo = new  EventInfo();
    public bool mUIDirty = true;
}

public class EventInfo
{
    public int mGuid = 0;
	public int mScript_id = -1;
	//public int mLeft;
	//public int mTop;
	//public int mRight;
	//public int mBottom;
	public int mTransSceneId = -1;
	public int mTransX=-1;
	public int mTransY=-1;
	public int mTransMinLevel=-1;
	public int mTransMaxLevel=-1;
	public static void copy(EventInfo src, EventInfo dest)
	{
		dest.mGuid = src.mGuid;
		dest.mScript_id = src.mScript_id;
		dest.mTransSceneId = src.mTransSceneId;
		dest.mTransX = src.mTransX;
		dest.mTransY = src.mTransY;
		dest.mTransMinLevel = src.mTransMinLevel;
		dest.mTransMaxLevel = src.mTransMaxLevel;
	}
}