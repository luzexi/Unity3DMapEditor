using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GFX;

public class UIWindowItem
{
    public  UIWindowItem():this(null) {}

    int id;					// ID
    public string m_strWindowName;		// 窗口名字，独一无二的
    string m_strLayoutFileName;	// layout文件名
    int demiseType;			// 是否可以自动滚动显示,屏幕最多可以显示两个可禅让窗口
    public int DemiseType
    {
        get { return demiseType; }
    }
    public GameObject winGo;           // 窗口指针
    Vector3 winPos;             // 保存窗口显示位置
    float WinDefaultZDepth;

    public UIWindowItem(_DBC_UI_LAYOUTDEFINE layout):this(layout,null)
    {
    }

    public UIWindowItem(_DBC_UI_LAYOUTDEFINE layout,AssetBundle asset)
    {
        if (layout == null)
        {
            LogManager.LogError("UI Layout Can not be null.");
            return;
        }

        id = layout.nID;
        m_strWindowName = layout.szName;
        m_strLayoutFileName = layout.szLayout;
        demiseType = layout.nDemise;

        if (!ParsePos(layout.poss))
            return;

        winGo = null;

        if (asset == null)
            RequestAssets(layout.szLayout);
        else
            HandleWindowDownload(null, asset);
    }

    bool ParsePos(string postion)
    {
        string[] poss = postion.Replace("\"","").Split(',');
        if (poss.Length != 2)
        {
            winPos = Vector3.zero;
            LogManager.LogError("UI window pos error, window name:" + m_strWindowName);
            return false;
        }
        WinDefaultZDepth = UIZDepthManager.GetIdByDemiseType(DemiseType);

        winPos = new Vector3(float.Parse(poss[0]), float.Parse(poss[1]), WinDefaultZDepth);
        winPos += UISystem.Instance.UiCamrea.transform.position;
        
        return true;
    }

    private void RequestAssets(string path)
    {
        ResourceManager.Me.DownloadAsync(UIRoot + path, null, HandleWindowDownload);
    }

    public void HandleWindowDownload(System.Object obj, AssetBundle asset)
    {
        if (asset == null)
            return;

        
        UnityEngine.Object res = asset.Load(m_strWindowName,typeof(GameObject));
        winGo = CreateObject(res);
        if (winGo == null)
        {
            LogManager.Log("Create UI Failed: " + m_strWindowName + " in HandleWindowDownload()");
            return;
        }
        // 不再挂接到摄像机节点上 [2/21/2012 Ivan]
        //GFX.GfxUtility.attachGameObject(winGo, UISystem.Instance.UiCamrea.gameObject);
        
        // 初始化一个ui后需要继续处理剩下的UI [4/1/2012 Ivan]
        UIWindowMng.Instance.AskDownNextWin();

        // 动态下载后强制置顶 [5/9/2012 Ivan]
        BringForward();
    }

    private GameObject CreateObject(UnityEngine.Object prefab)
    {
        if (prefab == null)
        {
            LogManager.LogError("UIPrefab is null.");
            return null;
        }
        GameObject inst = UnityEngine.Object.Instantiate(prefab, winPos, new Quaternion(0,0,0,0)) as GameObject;
        if (inst == null)
        {
            LogManager.Log("Create UI failed: " +m_strWindowName);
            return null;
        }
        inst.name = m_strWindowName;
        return inst;
    }

    public static string UIRoot
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                return "Build/UI/AssetBundles/";
            if (Application.platform == RuntimePlatform.WindowsWebPlayer)
                return "Build/UI/AssetBundles/"; //todo temp
            else
                return "Build/UI/AssetBundles/";
        }
    }

    internal void PreLoadWindow()
    {
    }

    public bool Show()
    {
        if (winGo == null)
            return false;
        // 同级窗口只能显示一个 [2/7/2012 Ivan]
        UIWindowMng.Instance.DemiseWindow(this);
        return ToggleWindow(true);
    }

    public bool Hide()
    {
        if (winGo == null)
            return false;
        return ToggleWindow(false);
    }

    bool ToggleWindow(bool isShow)
    {
        winGo.SetActiveRecursively(isShow);
        if (isShow)
            BringForward();

        return true;
    }

    static UIWindowItem lastWin = null;
    public void BringForward()
    {
        if (lastWin == this || this.DemiseType == 0)
            return;
        Vector3 oldPos = winGo.transform.localPosition;
        winGo.transform.localPosition = new Vector3(oldPos.x,oldPos.y,UIZDepthManager.NearWinZ);

        if (lastWin != null && lastWin != this)
            lastWin.ResetZDepth();

        lastWin = this;
    }

    public void ResetZDepth()
    {
        Vector3 oldPos = winGo.transform.localPosition;
        winGo.transform.localPosition = new UnityEngine.Vector3(oldPos.x, oldPos.y, WinDefaultZDepth);
    }

    internal bool IsCanDemise()
    {
        return DemiseType > 0;
    }

    internal bool IsWindowShow()
    {
        return winGo.active;
    }
}