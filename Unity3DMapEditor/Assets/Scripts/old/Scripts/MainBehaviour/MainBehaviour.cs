using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBSystem;
using LitJson;

public class MainBehaviour : MonoBehaviour {

    public string defaultServerIP;
    public int defaultServerPort;
	public string defaultAccount;
	public string defaultPassword;
	
	//Initial static member
	void Awake(){

        //后台运行标志
        Application.runInBackground = true;
        // 注意，这里加入一个开关控制打包的不同版本，比如调试版，策划版等 [3/17/2012 Ivan]
        GameProcedure.Version = CurrentVersion.Debug;

        //在非编辑器下使用打包资源
        if (Application.platform != RuntimePlatform.WindowsEditor)
            GameProcedure.Version = CurrentVersion.Release;
	}

    static void FirstInitial()
    {
        CDataBaseSystem.Instance.Initial(DBStruct.s_dbToLoad, DBStruct.GetResources);//初始化数据库，打开所有数据表
        if (CDataBaseSystem.StrError != "")
            LogManager.LogError(CDataBaseSystem.StrError);
        GameProcedure.InitStaticMemeber();
        GameProcedure.SetActiveProc(GameProcedure.s_ProcLogIn);
    }

    // 初始化配置文件 [3/16/2012 Ivan]
    public static void HandleConfigFileDownload(System.Object obj, AssetBundle assetSrc)
    {
        DBStruct.AllConfigAsset = assetSrc;
        FirstInitial();
    }

    //初始化技能配置文件[4/19/2012 shens]
    public static void HandleSkillFileDownload(System.Object obj, AssetBundle assetSrc)
    {
        GFX.GfxSkillManager.AllSkillAsset = assetSrc;
    }
    

	// Use this for initialization
    IEnumerator Start()
    {
        //资源版本文件最先加载
        WWW wwwVersion = new WWW(ResourceManager.Me.BaseURL + "version.txt");
        yield return wwwVersion;

        if (wwwVersion.error != null)
        {
            wwwVersion.Dispose();
            wwwVersion = null;
        }
        if(wwwVersion != null)
            ResourceManager.versionInfo = JsonMapper.ToObject<ResourceManager.VersionInfo>(wwwVersion.text);

        if (GameProcedure.Version != CurrentVersion.Debug )
        {
            string configPath = "Build/Config/all.config";
            ResourceManager.Me.DownloadAsync(configPath, null, HandleConfigFileDownload, AskType.Encrypt);
            
            string skillPath = "Build/Skill/skill.config";
            ResourceManager.Me.DownloadAsync(skillPath, null, HandleSkillFileDownload, AskType.Encrypt);
        }
        else
        {
            FirstInitial();
        }
	}

	// Update is called once per frame
	void Update () {

        //客户端中mainloop是主循环,
        //取消了客户端的调用方式，因为不同部分的更新，不在一个函数

        //逻辑轮询
        GameProcedure.TickActive();

        //处理游戏逻辑事件
        GameProcedure.ProcessGameEvent();
	
	}

    void OnApplicationQuit()
    {
        GameProcedure.ReleaseStaticMember();
    }
    public void OnLogin()
    {
        GameProcedure.s_ProcLogIn.mLoginServerAddr = defaultServerIP;
        GameProcedure.s_ProcLogIn.mLoginServerPort = defaultServerPort;
        GameProcedure.s_ProcLogIn.CheckAccount(defaultAccount, defaultPassword);
    }
}

