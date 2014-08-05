using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//武器刀光  [3/16/2012 ZZY@SG]
public class RibbonTrackData 
{
    public Vector3 pos;
    public Color color;
    public float bornTime;
}
public class WeaponRibbonScript : MonoBehaviour {

	// Use this for initialization
    const string StartLocatorName = "StartLocator";
    const string EndLocatorName = "EndLocator";
    Transform mRibbonStart;
    Transform mRibbonEnd;
    List<RibbonTrackData> mRibbonStartTrack = new List<RibbonTrackData>();
    List<RibbonTrackData> mRibbonEndTrack = new List<RibbonTrackData>();

    List<RibbonTrackData> mRibbonStartTrackTess = new List<RibbonTrackData>();
    List<RibbonTrackData> mRibbonEndTrackTess = new List<RibbonTrackData>();
    GameObject mRibbonGO;
    Mesh mRibbonMesh;
	public bool StartRibbon = false;
    public Material RibbonMaterial;
    public float LifeTime = 0.2f;
	public bool  AlphaBlend = true;
	public int   Tessellation = 3;
    public bool enable
    {
        set
        {
            StartRibbon = value;
            mRibbonStartTrack.Clear();
            mRibbonEndTrack.Clear();
			if(mRibbonMesh != null)
            	mRibbonMesh.Clear();
        }
        get
        {
            return StartRibbon;
        }
    }
	void Start () 
    {
        mRibbonGO = new GameObject("Ribbon");
        MeshFilter meshfilter = mRibbonGO.AddComponent<MeshFilter>();
        mRibbonGO.AddComponent<MeshRenderer>().material = RibbonMaterial;
        mRibbonMesh = meshfilter.mesh;
        Transform[] locators = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in locators)
        {
            if (t.name == StartLocatorName)
            {
                mRibbonStart = t;
            }
            else if (t.name == EndLocatorName)
            {
                mRibbonEnd = t;
            }
        }
        if (mRibbonStart == null || mRibbonEnd == null)
        {
            LogManager.LogError("Can not find Ribbon Locator");
        }
	}
    void OnEnable()
    {
        
    }
    void OnDisable()
    {
        
    }
	// Update is called once per frame
	void Update () {

        

	}
	void OnWillRenderObject()
	{
		UpdateRibbonMesh();
	}
    void UpdateRibbonMesh()
    {
        if (StartRibbon == false || mRibbonStart == null || mRibbonEnd == null ) return;
        RibbonTrackData curRibbonTrackStart = new RibbonTrackData();
        curRibbonTrackStart.pos = mRibbonStart.transform.position;   
        curRibbonTrackStart.bornTime = 0;
        curRibbonTrackStart.color = new Color(1,1,1,1);
        mRibbonStartTrack.Add(curRibbonTrackStart);

        RibbonTrackData curRibbonTrackEnd = new RibbonTrackData();
        curRibbonTrackEnd.pos = mRibbonEnd.transform.position;
        curRibbonTrackEnd.bornTime = 0;
        curRibbonTrackEnd.color = new Color(1, 1, 1, 1);
        mRibbonEndTrack.Add(curRibbonTrackEnd);
        int numTrackVertex = mRibbonStartTrack.Count;
        List<RibbonTrackData> removeStartList = new List<RibbonTrackData>();
		List<RibbonTrackData> removeEndList = new List<RibbonTrackData>();
        for (int i = 0; i < numTrackVertex; ++i )
        {
            float alpha = (LifeTime - mRibbonStartTrack[i].bornTime) / LifeTime;
            if (alpha <0 )
            {
                removeStartList.Add(mRibbonStartTrack[i]);
				removeEndList.Add(mRibbonEndTrack[i]);
            }
			
			if(AlphaBlend)
			{
				mRibbonStartTrack[i].color = new Color(1, 1, 1, alpha);
				mRibbonEndTrack[i].color = new Color(1, 1, 1, alpha);
			}
			else
			{
				mRibbonStartTrack[i].color = new Color(alpha, alpha, alpha, 1);
				mRibbonEndTrack[i].color = new Color(alpha, alpha, alpha, 1);
			}

            mRibbonStartTrack[i].bornTime += Time.deltaTime;
            
            mRibbonEndTrack[i].bornTime += Time.deltaTime;
        }
        for (int j=0; j< removeStartList.Count; ++j)//销毁到期的点
        {
            mRibbonStartTrack.Remove(removeStartList[j]);
            mRibbonEndTrack.Remove(removeEndList[j]);
        }
   
        mRibbonMesh.Clear();
        if (mRibbonStartTrack.Count<=1)//不能构成刀光
        {
            return;
        }
		tessellation( mRibbonStartTrack, ref mRibbonStartTrackTess);
		tessellation( mRibbonEndTrack, ref mRibbonEndTrackTess);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
		List<Vector3> normals = new List<Vector3>();
        numTrackVertex = mRibbonStartTrackTess.Count;
		if(numTrackVertex<=1)
		{
			return;
		}
        for ( int i = 0; i < (numTrackVertex); ++i )
        {
			float uvx = 1-((float)i)/(float)(numTrackVertex-1);
            vertices.Add(mRibbonStartTrackTess[i].pos);
            colors.Add(mRibbonStartTrackTess[i].color);
            uvs.Add(new Vector2(uvx, 0));
			normals.Add(Vector3.up);
            vertices.Add(mRibbonEndTrackTess[i].pos);
            colors.Add(mRibbonEndTrackTess[i].color);
			uvs.Add(new Vector2(uvx,1) );
			normals.Add(Vector3.up);
        }
        for (int i = 0; i < (numTrackVertex - 1); ++i)
        {
            triangles.Add(2 * i);//三角形1
            triangles.Add(2 * i+1);
            triangles.Add(2 * (i+1));

            triangles.Add(2 * i + 1);
            triangles.Add(2 * (i + 1));
            triangles.Add(2 * (i + 1) + 1);
        }
        mRibbonMesh.vertices = vertices.ToArray();
        mRibbonMesh.colors = colors.ToArray();
        mRibbonMesh.uv = uvs.ToArray();
		mRibbonMesh.normals = normals.ToArray();
        mRibbonMesh.triangles = triangles.ToArray();
        mRibbonMesh.RecalculateBounds();
    }
	
	void tessellation(List<RibbonTrackData> ribbon, ref List<RibbonTrackData> ribbonTess)//镶嵌三角形，形成光滑的条带
	{
		ribbonTess.Clear();
        //ribbonTess.AddRange(ribbon);
       // return; //temp code
		int count = ribbon.Count-1;
		
		if(count<=0) return;
        for (int i = 0; i < count; ++i)
        {
            RibbonTrackData t0 = new RibbonTrackData();
            if (i == 0)
            {
                t0.pos = ribbon[i + 1].pos - ribbon[i].pos;
                t0.color = ribbon[i + 1].color - ribbon[i].color;
                t0.bornTime = ribbon[i + 1].bornTime - ribbon[i].bornTime;
            }
            else
            {
                t0.pos = ribbon[i + 1].pos - ribbon[i - 1].pos;
                t0.color = ribbon[i + 1].color - ribbon[i - 1].color;
                t0.bornTime = ribbon[i + 1].bornTime - ribbon[i - 1].bornTime;
            }
            RibbonTrackData t1 = new RibbonTrackData();
            if (i == (count - 1))
            {
                t1.pos = ribbon[i + 1].pos - ribbon[i].pos;
                t1.color = ribbon[i + 1].color - ribbon[i].color;
                t1.bornTime = ribbon[i + 1].bornTime - ribbon[i].bornTime;
            }
            else
            {
                t1.pos = ribbon[i + 2].pos - ribbon[i].pos;
                t1.color = ribbon[i + 2].color - ribbon[i].color;
                t1.bornTime = ribbon[i + 2].bornTime - ribbon[i].bornTime;
            }

            RibbonTrackData p0 = ribbon[i];
            RibbonTrackData p1 = ribbon[i + 1];
            //Q(s) = (2s3 - 3s2 + 1)v1 + (-2s3 + 3s2)v2 + (s3 - 2s2 + s)t1 + (s3 - s2)t2.
            float step = 1.0f / (Tessellation + 1);
            for (int j = 0; j < (Tessellation + 1); ++j)
            {
                RibbonTrackData p = new RibbonTrackData();
                float s = j * step;
                float a = (2 * s * s * s - 3 * s * s + 1);
                float b = (-2 * s * s * s + 3 * s * s);
                float c = (s * s * s - 2 * s * s + s);
                float d = (s * s * s - s * s);
                p.pos = a * p0.pos + b * p1.pos + c * t0.pos + d * t1.pos;
                p.color = (float)a * p0.color + (float)b * p1.color + (float)c * t0.color + (float)d * t1.color;
                p.bornTime = (float)a * p0.bornTime + (float)b * p1.bornTime + (float)c * t0.bornTime + (float)d * t1.bornTime;
                ribbonTess.Add(p);
            }
        }
        ribbonTess.Add(ribbon[ribbon.Count - 1]);
	}
    void OnDestroy()
    {
        mRibbonMesh = null;
        Object.Destroy(mRibbonGO);
    }
}
