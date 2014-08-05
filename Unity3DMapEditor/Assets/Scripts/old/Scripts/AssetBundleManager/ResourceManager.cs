using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 资源下载完成后的回调函数
/// </summary>
/// <param name="obj">需要资源的对象</param>
/// <param name="asset">下载完成后的资源对象</param>

public delegate void DownloadHandler(System.Object obj, AssetBundle asset);

/************************************************************************/
/* 资源下载管理器
 * 由于暂时无法解决下载列表同步问题，因此不再用yield，是用轮询检测的机制
 * 使用接口函数DownloadAsyn提出下载需求
 * 如果资源已经下载完成，不再下载；如果正在等待下载，不再下载；
 */
/************************************************************************/

public class ResourceManager : MonoBehaviour {

    public struct VersionInfo
    {
        public int nVersionDBC;
        public int nVersionScene;
        public int nVersionUI;
    }
  

    void Init()
    {
        requestList = new List<Resource>();
        bufferList = new List<Resource>();
    }
	// Update is called once per frame
	void Update () {
        ProcessDownload();
	}
    //when the application exit
    void OnApplicationQuit()
    {
        s_Singleton = null;

        Release();
    }


    static ResourceManager s_Singleton;

    public static ResourceManager Me
    {
        get {
            if (s_Singleton == null)
            {
                s_Singleton = FindObjectOfType(typeof(ResourceManager)) as ResourceManager;
                s_Singleton.Init();
            }
            if(s_Singleton == null)
            {
                throw new NullReferenceException("Cannot find ResourceManager");
            }
            return s_Singleton; 
        }
    }

    public bool DownloadAsync(string file, System.Object obj, DownloadHandler handler, AskType askType, ResourceType resType)
    {
        //已经加载
        AssetBundle asset = GetFromMemory(file);
        if (asset != null)
        {
            if (handler != null)
                handler(obj, asset);
            return true;
        }
        Resource res = GetInDownloadList(file);
        if (res != null)
        {
            OwnerInfo info;
            info.obj = obj;
            info.handler = handler;
            res.objects.Add(info);
            //LogManager.Log("Add Handler to res :" + res.FilePath);
            return true;
        }

        res = new Resource();
        OwnerInfo inf;
        inf.obj = obj;
        inf.handler = handler;
        res.objects.Add(inf);
        res.FilePath = file;
        res.type = askType;
        res.ResType = resType;

        //这里和ProcessDownload数据同步是否有问题？
        requestList.Add(res);

        return true;
    }
    public bool DownloadAsync(string file, System.Object obj, DownloadHandler handler)
    {
        return DownloadAsync(file, obj, handler, AskType.Normal, ResourceType.RT_Normal);
    }

    // 由于资源被打包后无法直接判断出是否加密，暂时由使用者来判断 [3/27/2012 Ivan]
    public bool DownloadAsync(string file, System.Object obj, DownloadHandler handler, AskType askType)
    {
        return DownloadAsync(file, obj, handler, askType, ResourceType.RT_Normal);
    }
    public AssetBundle Download(string file)
    {

        return null;
    }
    public void Unload(string file)
    {
        if(resources.ContainsKey(file))
        {
            //卸载所有资源，包括现在已经被引用的
            resources[file].Unload(false);

            resources.Remove(file);
        }
    }
    public void UnloadAll()
    {
        foreach (KeyValuePair<string, AssetBundle> keyPair in resources)
        {
            keyPair.Value.Unload(false);
        }
        resources.Clear();
    }
    public void Release()
    {
        UnloadAll();
        ClearRequestList();
    }
    Resource GetInDownloadList(string file)
    {
        foreach(Resource res in requestList)
        {
            if (res.FilePath == file)
                return res;
        }

        return null;
    }
    void ProcessDownload()
    {
        foreach (Resource res in requestList)
        {
            if (res.Loaded) continue;
            if (res.Loading)
            {
                if (res.www.isDone)
                {
                    // 判断是否加密过 [3/27/2012 Ivan]
                    if (res.type == AskType.Encrypt)
                    {
                        DecryptFile(res);
                    }
                    else
                    {
                        res.Loaded = true;
                        res.Loading = false;
                        InvokeHandler(res);


                        if (res.www.error != null)
                        {
                            LogManager.LogError(res.www.error);
                            res.www.Dispose();
                            res.www = null;
                            return;
                        }

                        resources.Add(res.FilePath, res.www.assetBundle);
                        res.www.Dispose();
                        res.www = null;
                    }

                }
                return;
            }
            else
            {
                string url = BaseURL + res.FilePath;
                if (LoadFromCache)
                    res.www = WWW.LoadFromCacheOrDownload(url, GetResourceVersion(res.ResType));
                else
                    res.www = new WWW(url);
                res.Loading = true;
                return;
            }
        }
        requestList.Clear();
    }

    private void DecryptFile(Resource res)
    {
        if (res.request == null)
        {
            // 由于加密打包的过程中，多封装了一步，所以要先拆解出来 [3/27/2012 Ivan]
            string fileName = GetFileName(res.FilePath);
            TextAsset asset = res.www.assetBundle.Load(fileName) as TextAsset;
            if (asset != null)
            {
                // 需要先解密 [3/28/2012 Ivan]
                byte[] result = Encrypt.SGEncrypt.DecryptByte(asset.bytes);
                res.request = AssetBundle.CreateFromMemory(result);
            }
            else
            {
                res.Loaded = true;
                res.Loading = false;
            }
            return;
        }
        else if (res.request.isDone)
        {
            res.Loaded = true;
            res.Loading = false;
            foreach (OwnerInfo info in res.objects)
            {
                if (info.handler != null)
                    info.handler(info.obj, res.request.assetBundle);
            }
            resources.Add(res.FilePath, res.request.assetBundle);
            res.www.Dispose();
            res.www = null;
        }
    }

    private string GetFileName(string path)
    {
        string[] names = path.Split('/');
        string fileName = names[names.Length - 1];
        if (fileName.Contains("."))
        {
            fileName = fileName.Substring(0, fileName.IndexOf('.'));
        }
        return fileName;
    }

    void InvokeHandler(Resource res)
    {
        foreach (OwnerInfo info in res.objects)
        {
            if(info.handler != null)
                info.handler(info.obj, res.www.assetBundle);
            //LogManager.Log("Download res: " + res.FilePath);
        }
    }

  
    AssetBundle GetFromMemory(string file)
    {
        if(resources.ContainsKey(file))
            return resources[file];

        return null;
    }
    void ClearRequestList()
    {
        if(requestList != null)
            requestList.Clear();
    }


    //////////////////////////////////////////////////////////////////////////
    public string BaseURL
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                return "file://" + Application.dataPath + "/../";
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                return "http://192.168.1.173:8080/";
            if (Application.platform == RuntimePlatform.WindowsWebPlayer)
                return ""; //todo temp
            else
                return "file://" + Application.dataPath + "/../Build/Maps/";
        }
    }
    public int Version
    {
        get { return mVersion; }
        set { mVersion = value;}
    }

    int GetResourceVersion(ResourceType resType)
    {
        int version = 0;
        switch (resType)
        {
            case ResourceType.RT_DBC:
                version = versionInfo.nVersionDBC;
                break;
            case ResourceType.RT_Scene:
                version = versionInfo.nVersionScene;
                break;
            case ResourceType.RT_UI:
                version = versionInfo.nVersionUI;
                break;
            default:
                break;
        }
        return version;
    }
    static VersionInfo mVersionInfo;
    public static VersionInfo versionInfo
    {
        get { return mVersionInfo; }
        set { mVersionInfo = value; }
    }
    //////////////////////////////////////////////////////////////////////////
    List<Resource> requestList;
    List<Resource> bufferList;
    
 //   bool isLoading;
  //  bool isbufferLoading;

    bool LoadFromCache = false;
    WWW download;
    int mVersion=1;

    Dictionary<string, AssetBundle> resources = new Dictionary<string, AssetBundle>();
}
