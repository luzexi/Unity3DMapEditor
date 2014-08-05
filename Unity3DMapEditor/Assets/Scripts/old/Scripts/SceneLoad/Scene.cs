using UnityEngine;

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NavMesh;
using System.IO;
using System;

public class SceneObject
{
	public GameObject go;
	public AssetBundle asset;
	public AssetBundle texture;
    public string assetFile;
	public string textureName;
    public bool isLoading;

	
	public SceneObject()
	{
		go = null;
		asset = null;
		texture = null;
        isLoading = false;
	}
}

public class SceneInfo{
	
	
	static public SceneInfo Instance()
	{
        if (singleton == null)
            singleton = new SceneInfo();
		return singleton;
	}
    static SceneInfo singleton;

	private SceneInfo()
	{
		loadSceneBehaviour = GameObject.Find("Main Camera").GetComponent<LoadScene>();
	}
	
	//
	LoadScene loadSceneBehaviour;
	
	public Scene mScene;

  
	public int  AssetVersion = 3;
	
	List<SceneNode> objects = new List<SceneNode>();
	
	List<SceneObject> mObjects;
    List<SceneObject> mObjectsCopy;
	
	public bool mLoading =false;
	
	bool mLoaded =false;
	public bool IsObjectDownload
	{
		get{
			return mLoaded;
		}
	}
	
	bool mTextureLoaded = false;
	public bool IsTextureDownload
	{
		get
		{
			return mTextureLoaded;
		}
	}

    int DownloadCount = 5;
	public bool bUseCach = false;

    private void LoadNavMesh(string filePath)
    {
        ResourceManager.Me.DownloadAsync(filePath, null, HandleNavMeshDownload);
    }
    
    public void HandleNavMeshDownload(System.Object obj, AssetBundle assetSrc)
    {
        List<Triangle> allNavMeshData = new List<Triangle>();
		if(assetSrc == null)
			return;

        TextAsset asset = assetSrc.Load(currSceneName + ".NavMesh") as TextAsset;
        MemoryStream stream = new MemoryStream(asset.bytes);
        BinaryReader binReader = new BinaryReader(stream);

        NavResCode res = NavResCode.Failed;
        try
        {
            res = NavMeshGen.Instance.LoadNavMeshFromFile(ref allNavMeshData, binReader);
        }
        finally
        {
            binReader.Close();
            stream.Close();
        }

        if (res != NavResCode.Success)
        {
            LogManager.LogError("加载导航网格失败");
        }
        else
        {
            PathFinder.Instance.NavMeshData = allNavMeshData;
            LogManager.Log("加载导航网格成功!");
        }

        //////////////////////////////////////////////////////////////////////////
        // 检查是否在自动寻路 [8/9/2011 ivan edit]
        Interface.GameInterface.Instance.AutoMoveNavReady();
    }

    string currSceneName;
    public void EnterScene(string name)
    {
        //销毁旧数据
        DestoryScene();
        
        string file = SceneRoot + name;
        ResourceManager.Me.DownloadAsync(file, null, HandleSceneDownload);

        string[] scenes = name.Split('.');
        currSceneName = scenes[0].Split('/')[0];
        // 加载导航数据 [1/10/2012 Ivan]
        file = SceneRoot + currSceneName + "/" + currSceneName + ".NavMesh";
        LoadNavMesh(file);

    }

    public void EnterSceneWithoutNav(string name)
    {
        //销毁旧数据
        DestoryScene();
        //LogManager.Log("Enter Scene :" + name);

        string file = SceneRoot + name;
        ResourceManager.Me.DownloadAsync(file, null, HandleSceneDownload);
    }

    public void HandleSceneDownload(System.Object obj, AssetBundle asset)
    {
        if (asset == null)
            throw new NullReferenceException("Download scene json file failed  asset is null");
     
        StringHolder holder = asset.mainAsset as StringHolder;
        if (holder == null)
            throw new NullReferenceException("The Scene json is failed");
        LoadFromJson(holder.content[0]);

        //load terrain
        string path = SceneRoot + mScene.terrainFile;
        ResourceManager.Me.DownloadAsync(path, null, HandleTerrainDownload);
    }
    public void HandleTerrainDownload(System.Object obj, AssetBundle asset)
    {
        if (asset == null) return;
        //LogManager.Log("Load Terrain file");
        Application.LoadLevelAdditive(mScene.name);
        //给地形设置ground layer
        GameObject terrainGo = GameObject.Find(mScene.name);
        if (terrainGo != null)
        {
            Transform[] allTrans = terrainGo.GetComponentsInChildren<Transform>();
            foreach (Transform t in allTrans)
            {
                t.gameObject.layer = LayerManager.GroundLayer;
            }
        }

        //开始加载对象
        loadSceneBehaviour.UpdateObjects += BuildObjectSingleNew;
    }

    public void LoadFromJson(string strJson)
    {
        mObjects = new List<SceneObject>();
        mObjectsCopy = new List<SceneObject>();

        mScene = SceneJsonSerializer.JsonToObject(strJson);
        if(mScene == null)
        {
            throw new NullReferenceException("Parse scene json failed!"+strJson);
        }
        BuildScene();
        //LogManager.Log("Create Scene Sucess:terrain=" + mScene.terrainFile + ", objectcount="+mScene.objects.Count);
    }
	public void BuildScene()
	{
        foreach (SceneNode node in mScene.objects )
        {
            objects.Add(node);
        }
	}

	void DestoryScene()
    {
        try
        {
            //销毁原有的地形
            if (mScene != null)
            {
                GameObject terrain = GameObject.Find(mScene.name);
                GameObject.Destroy(terrain);
            }

            if (mObjects != null)
            {
                //destory gameobject
                foreach (SceneObject obj in mObjects)
                {
                    //LogManager.LogWarning("destroy asset " + obj.assetFile);
                    GameObject.Destroy(obj.go);
                    ResourceManager.Me.Unload(SceneRoot + obj.assetFile);
                }
                mObjects.Clear();
            }
        }
        catch(Exception e)
        {
            LogManager.LogError("Destroy scene object failed! exception: " + e.Message);
        }
	}

    GameObject CreateObject(UnityEngine.Object asset, SceneNode node)
    {
        if(asset == null)
        {
            LogManager.Log("Create gameobject: " + node.name + " asset is " + node.assetPath);
            return null;
        }
        Vector3 pos = new Vector3((float)node.position[0], (float)node.position[1], (float)node.position[2]);
        Quaternion qu = new Quaternion((float)node.rotation[0], (float)node.rotation[1], (float)node.rotation[2], (float)node.rotation[3]);
        GameObject inst = UnityEngine.Object.Instantiate(asset, pos, qu) as GameObject;
        if(inst == null)
        {
            LogManager.Log("Create object failed: " + node.name);
            return null;
        }
		inst.name = node.name;
        inst.transform.localScale = new Vector3((float)node.scale[0], (float)node.scale[1], (float)node.scale[2]);
		
        return inst;
    }
    public void BuildObjectSingleNew(GameObject go)
    {
        if (objects.Count == 0)
            return;
        GameObject camera = GameObject.Find("Main Camera");

        SceneNode node = GetBestObjectToLoad(camera.transform);//如果对象很多，这里会很影响效率，需要改进

        if (node != null)
        {
            ResourceManager.Me.DownloadAsync(SceneRoot + node.assetPath, node, HandleObjectDownload);
        }
        objects.Remove(node);

        if (objects.Count == 0)
        {
            loadSceneBehaviour.UpdateObjects -= BuildObjectSingleNew;
        }
       
    }
    public void HandleObjectDownload(System.Object obj, AssetBundle asset)
    {
        if(obj == null || asset == null)
        {
            LogManager.Log("Invalid object");
            return;
        }
        try
        {

            SceneNode node = (SceneNode)obj;

            SceneObject so = new SceneObject();
            so.asset = asset;
            so.textureName = node.texture;
            so.assetFile = node.assetPath;

            UnityEngine.Object res = so.asset.Load(node.name, typeof(GameObject));
            so.go = CreateObject(res, node);
            if (so.go == null)
            {
				LogManager.LogError("Create Object Failed: " + node.name + " in HandleObjectDownload()");
                return;
            }

            mObjects.Add(so);
            mObjectsCopy.Add(so);

            if (mObjectsCopy.Count >= DownloadCount)
                loadSceneBehaviour.UpdateTextures += UpdateTexturesSingle_v4;
            else if (mScene.objects.Count - mObjects.Count < DownloadCount)
                loadSceneBehaviour.UpdateTextures += UpdateTexturesSingle_v4;
  
            
        }
        catch (System.Exception e)
        {
            LogManager.LogError(e.ToString());
        }

        

    }
    public void UpdateChildTextures_v4(SceneObject node)
    {
        if (node.texture == null && !node.isLoading)
        {
            if (node.textureName != "none")
            {
                string[] t = node.textureName.Split('@');
                ResourceManager.Me.DownloadAsync(SceneRoot + t[0]/*node.textureName*/, node, HandleTextureDownload);
            }
            node.isLoading = true;
     
        }
    }
    public void HandleTextureDownload(System.Object obj, AssetBundle asset)
    {
        if (obj == null)
        {
            LogManager.LogError("Invalid object : HandleTextureDownload()");
            return;
        }
        SceneObject node = (SceneObject) obj;
        if (node == null)
        {
            LogManager.LogError("Invalid node: HandleTextureDownload()");
            return;
        }
        if (node.go == null)
        {
            LogManager.LogError("GameObject " + node.textureName + "is null in:HandleTextureDownload()");
            return;
        }
        node.texture = asset;

        int npos = node.textureName.IndexOf("@");
        if (npos == -1)
        {
            LogManager.LogError("Update texture failed: " + node.go.name);
            return;
        }

        Renderer[] renders = node.go.GetComponentsInChildren<Renderer>();

        string[] r = node.textureName.Substring(npos + 1).Split(';');
        if (renders.Length > r.Length)return;
        for (int i = 0; i < renders.Length; i++)
        {
            string[] m = r[i].Split(',');
            if (renders[i].materials.Length > m.Length)
            {
                //LogManager.LogError("The material num is not equal:" + "materials=" + renders[i].materials.Length + "string=" + r[i]);
                return;
            }
            for (int j = 0; j < renders[i].materials.Length; j++ )
            {
                //Texture tex = renders[i].materials[j].mainTexture;
                //if (!tex)
                //{
                //    LogManager.Log("Update " + node.go.name + " texture is null");
                //    continue;
                //}
                
                //if (node.texture.Contains(tex.name))
                //{
                //    //renders[i].materials[j].mainTexture = node.texture.Load(tex.name) as Texture;
                //}
                //else
                //{
                //    LogManager.Log("No texture in asssetbundle: " + tex.name + " go:" + node.go.name);
                //}
                string[] t = m[j].Split('=');
                if(renders[i].materials[j].name.Contains(t[0]))
                {
                    if(node.texture.Contains(t[1]))
                    {
                        renders[i].materials[j].mainTexture = node.texture.Load(t[1]) as Texture;
                    }
                    else
                    {
                        LogManager.LogError("The texture was not found: " + t[1]);
                    }
                }
                else
                {
                    LogManager.LogError("The material was not found: " + t[0]);
                }
            }

        }
    }


    public void UpdateTexturesSingle_v4(GameObject go)
    {
        if (mObjectsCopy.Count == 0 || mObjectsCopy[mObjectsCopy.Count - 1].isLoading)
        {
            //LogManager.Log("Remove Update Texture: count=" + mObjectsCopy.Count);
            mObjectsCopy.Clear();
            
            loadSceneBehaviour.UpdateTextures -= UpdateTexturesSingle_v4;
            return;
        }

        foreach (SceneObject node in mObjectsCopy)
        {

            UpdateChildTextures_v4(node);
        }
    }


	SceneNode GetBestObjectToLoad(Transform transCamera){
		//通常选择最近的加载
		float bestDistance = 0;
		SceneNode bestNode = null ;
		foreach (SceneNode node in objects)
		{
            Vector3 nodepos = new Vector3((float)node.position[0], (float)node.position[1], (float)node.position[2]);

            float distance = (nodepos - transCamera.position).sqrMagnitude;
			if(bestNode == null || distance < bestDistance)
			{
				bestNode = node;
				bestDistance = distance;
				
				if(distance == 0)
					break;
			}
		}
		
		return bestNode;
	}
	public static string GetBaseUrl(){
		
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            return "file://" + Application.dataPath + "/../Build/Maps/";
		if(Application.platform == RuntimePlatform.WindowsWebPlayer)
			return "Build/Maps/"; //todo temp
		else
			return "file://" + Application.dataPath + "/../Build/Maps/";
	}
    public static string SceneRoot
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                return "Build/Maps/";
            if (Application.platform == RuntimePlatform.WindowsWebPlayer)
                return "Build/Maps/"; //todo temp
            else
                return "Build/Maps/";
        }
    }
}