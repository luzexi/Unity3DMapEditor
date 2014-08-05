using UnityEngine;
using System.Collections.Generic;

public enum AskType
{
    Normal,
    Encrypt
}
public enum ResourceType
{
    RT_Normal,
    RT_DBC,    //数据表
    RT_UI,     //UI界面
    RT_Scene,  //场景数据
    RT_Actor,  //对象
}

public struct OwnerInfo
{
    public System.Object obj;
    public DownloadHandler handler;
}
public class Resource
{
    //public delegate void DownloadHandler(System.Object obj, AssetBundle asset);

    public string FilePath
    {
        get { return path;}
        set {path = value;}
    }
    public bool Loading
    {
        get { return isloading; }
        set { isloading = value;}
    }
    public bool Loaded
    {
        get { return isloaded; }
        set { isloaded = value;}
    }
    public ResourceType ResType
    {
        get { return mResType; }
        set { mResType = value; }
    }


    public Resource()
    {
        isloaded = false;
        isloading = false;
        objects = new List<OwnerInfo>();
    }

    public List<OwnerInfo> objects;
    public WWW www;

    ResourceType mResType;
    string path;
    bool isloading;
    bool isloaded;
    // 判断是否加密类型 [3/27/2012 Ivan]
    public AskType type;
    public UnityEngine.AssetBundleCreateRequest request;
}