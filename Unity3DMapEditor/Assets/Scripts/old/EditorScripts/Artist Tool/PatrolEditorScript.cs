using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class PatrolEditorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        bool leftButtonDown = Input.GetMouseButtonDown(0);
        bool rightButtonDown = Input.GetMouseButtonDown(1);
        if (rightButtonDown || leftButtonDown)//鼠标左键或右键按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(ray, out hitInfo);
            if (!hit) return;
            Vector3 hitPos = hitInfo.point;

            if (rightButtonDown)//摆放模式
            {
                add(hitInfo.point, mCurrentSettleTime);
            }
            if (leftButtonDown && hitInfo.collider != null)//选择模式
            {
                if (hitInfo.collider.gameObject.name == patrolPointName)
                    select(hitInfo.collider.gameObject);
            }
        }
        //处理删除
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleteCurrent();
        }
	}

    void select(GameObject go)
    {
        if (mCurrentPatrolPoint != null)
            mCurrentPatrolPoint.renderer.material.color = (Color.white);

       // UnityEditor.Selection.activeGameObject = GameObject.Find("SGEditor");
        mCurrentPatrolPoint = go;
        
        if (mCurrentPatrolPoint != null)
            mCurrentPatrolPoint.renderer.material.color = (Color.green);
        foreach (patrol p in mAllPatrols)
        {
            patrolPoint pp = p.findPatrolPoint(go);
            if ( pp != null )
            {
                mCurrentSettleTime = pp.mSettletime;
                mCurrentPatrol = p;
                return;
            }
        }
    }

    public void add(Vector3 pos, int settleTime)
    {
        if(mCurrentPatrol == null)//开始一个新的路线
        {
            mCurrentPatrol = new patrol();
            mAllPatrols.Add(mCurrentPatrol);
        }
        if (mCurrentPatrol != null)
        {
            GameObject go = mCurrentPatrol.addPatrolPoint(pos, settleTime);
            select(go);
        }
    }
    public void clearAll()
    {
        mCurrentPatrol = null;
        mCurrentPatrolPoint = null;
        foreach (patrol p in mAllPatrols)
        {
            p.clear();
        }
        mAllPatrols.Clear();
    }
    public void modifyCurrent()
    {
        if (mCurrentPatrol == null || mCurrentPatrolPoint == null)
            return;
        mCurrentPatrol.findPatrolPoint(mCurrentPatrolPoint).mSettletime = mCurrentSettleTime ;

    }
    public void finishCurrentLine()
    {
        mCurrentPatrol = null;
        select(null);
    }
    void deleteCurrent()
    {
        if (mCurrentPatrolPoint == null)
            return;
        foreach (patrol p in mAllPatrols)
        {
            patrolPoint pp = p.findPatrolPoint(mCurrentPatrolPoint);
            if ( pp != null)
            {
                p.removePatrolPoint(mCurrentPatrolPoint);
                if (p.mPatrolPoints.Count == 0)
                {
                    mAllPatrols.Remove(p);
                    mCurrentPatrol = null;
                }
                break;
            }
        }
        select(null);
    }
    public void refresh()
    {
        foreach (patrol p in mAllPatrols)
        {
            p.updatePatrolLine();
        }
    }
    public void saveToFile(string path)
    {
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine("[INFO]");
        sw.WriteLine("PATROLNUMBER=" + mAllPatrols.Count);
        if (mAllPatrols.Count >0)
        {
            for (int i = 0; i < mAllPatrols.Count; ++i )
            {
                sw.WriteLine("[PATROL" + i + "]");
                sw.WriteLine("PATROLPOINTNUM=" + mAllPatrols[i].mPatrolPoints.Count);
                for (int j = 0; j < mAllPatrols[i].mPatrolPoints.Count; ++j)
                { 
                    Vector3 pos = mAllPatrols[i].mPatrolPoints[j].mGo.transform.position;
                    sw.WriteLine("POSX" + j + "=" + pos.x);
                    sw.WriteLine("POSY" + j + "=" + pos.y);
                    sw.WriteLine("POSZ" + j + "=" + pos.z);
                    sw.WriteLine("settletime" + j + "=" + mAllPatrols[i].mPatrolPoints[j].mSettletime);
                }
            }
        }
        sw.Close();
    }
    public int mCurrentSettleTime = 0;
    public bool mGUIDirty = true;
    GameObject mCurrentPatrolPoint = null;
    patrol mCurrentPatrol = null;
    List<patrol> mAllPatrols =  new List<patrol>();
    public static string patrolPointName = "patrolPoint";
}

public class patrolPoint
{
    public GameObject mGo;
    public int mSettletime = 0;
}

public class patrol
{
    public List<patrolPoint> mPatrolPoints = new List<patrolPoint>();
    GameObject mLinesGo = null;
	static int LinesCount = 0;
    public GameObject addPatrolPoint(Vector3 pos, int settleTime)
    {
        patrolPoint pp = new patrolPoint();
        pp.mGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pp.mGo.name = PatrolEditorScript.patrolPointName;
        pp.mGo.transform.position = pos;
        pp.mGo.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        pp.mSettletime = settleTime;
        mPatrolPoints.Add(pp);
        updatePatrolLine();
        return pp.mGo;
    }
    public void removePatrolPoint(GameObject go)
    {
        patrolPoint pp = findPatrolPoint(go);
        if (pp == null) return;
        mPatrolPoints.Remove(pp);
        UnityEngine.Object.DestroyImmediate(go);
        updatePatrolLine();
    }
    public void updatePatrolLine()
    {
        if ( mPatrolPoints.Count == 0 )
		{
			UnityEngine.Object.DestroyImmediate(mLinesGo);
			return;
		}
        if (mLinesGo == null)
        {
            mLinesGo = new GameObject("lines" + LinesCount);
			LinesCount++;
            mLinesGo.AddComponent(typeof(LineRenderer));
        }
        LineRenderer lr = (LineRenderer)mLinesGo.GetComponent(typeof(LineRenderer));
        lr.SetWidth(0.2f, 0.2f);
        lr.SetVertexCount(mPatrolPoints.Count);
        for (int i = 0; i < mPatrolPoints.Count; ++i)
        {
            lr.SetPosition(i, mPatrolPoints[i].mGo.transform.position);
        }
    }
    public patrolPoint findPatrolPoint(GameObject go)
    {
        foreach (patrolPoint pp in mPatrolPoints)
        {
            if (pp.mGo == go)
            {
                return pp;
            }
        }
        return null;
    }
    public void clear()
    {
        foreach (patrolPoint pp in mPatrolPoints)
        {
            UnityEngine.Object.DestroyImmediate(pp.mGo);
        }
        mPatrolPoints.Clear();
        UnityEngine.Object.DestroyImmediate(mLinesGo);
    }
}