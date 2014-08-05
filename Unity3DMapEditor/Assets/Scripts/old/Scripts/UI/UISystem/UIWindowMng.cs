using System;
using System.Collections.Generic;
using System.Text;
using DBSystem;
using UnityEngine;

public class UIWindowMng
{

    static readonly UIWindowMng sInstance = new UIWindowMng();
    static public UIWindowMng Instance { get { return sInstance; } }
    
    List<UIWindowItem> allWindows = new List<UIWindowItem>();
    DBC.COMMON_DBC<_DBC_UI_LAYOUTDEFINE> allLayouts;

    public void Init()
    {
        allLayouts = CDataBaseSystem.Instance.GetDataBase<_DBC_UI_LAYOUTDEFINE>((int)DataBaseStruct.DBC_UI_LAYOUTDEFINE);

        if (allLayouts == null || allLayouts.StructDict.Count == 0)
        {
            LogManager.LogWarning("layouts is empty!");
            return;
        }
        foreach (_DBC_UI_LAYOUTDEFINE layout in allLayouts.StructDict.Values)
        {
            if (layout.downImmediately == 1)
                needDownList.Add(layout);
        }

        _DBC_UI_LAYOUTDEFINE common = allLayouts.Search_Index_EQU(0);
        ResourceManager.Me.DownloadAsync(UIWindowItem.UIRoot + common.szLayout, null, CommonWindowDownload);

        _DBC_UI_LAYOUTDEFINE fonts = allLayouts.Search_Index_EQU(2);
        ResourceManager.Me.DownloadAsync(UIWindowItem.UIRoot + fonts.szLayout, null, FontsDownload);

        // 这里是为了初始化Icon管理器 [2/17/2012 Ivan]
        _DBC_UI_LAYOUTDEFINE icon = allLayouts.Search_Index_EQU(1);
        IconManager.Instance.Init(UIWindowItem.UIRoot + icon.szLayout);
    }

    // 因为前两项是通用部分和Icon部分，所以特殊处理 [2/17/2012 Ivan]
    const int IgnoreNum = 3;
    AssetBundle uiCommonAsset;
    public void CommonWindowDownload(System.Object obj, AssetBundle asset)
    {
        uiCommonAsset = asset;

        InitTextPrefabs(asset);
    }

    public void FontsDownload(System.Object obj, AssetBundle asset)
    {
        AskDownNextWin();
    }

    List<_DBC_UI_LAYOUTDEFINE> needDownList = new List<_DBC_UI_LAYOUTDEFINE>();
    int currentIndex = IgnoreNum;
    public void AskDownNextWin()
    {
        if (currentIndex < needDownList.Count)
        {
            _DBC_UI_LAYOUTDEFINE layout = needDownList[currentIndex++];
            AskWindowDown(layout.szName);
        }
    }

    UnityEngine.Object contentTextPrefab = null;
    public UnityEngine.Object ContentTextPrefab
    {
        get { return contentTextPrefab; }
    }
    UnityEngine.Object captionTextPrefab = null;
    public UnityEngine.Object CaptionTextPrefab
    {
        get { return captionTextPrefab; }
    }
    // 为了加快初始化速度，保存字体的prefab [3/5/2012 Ivan]
    void InitTextPrefabs(AssetBundle asset)
    {
        contentTextPrefab = GetObjFromCommon("ContentTextPre");
        captionTextPrefab = GetObjFromCommon("CaptionTextPre");
    }

    public UnityEngine.Object GetObjFromCommon(string name)
    {
        return uiCommonAsset.Load(name);
    }

    public void BringWindowForward(string windowName)
    {
        UIWindowItem win = GetWindow(windowName);
        if (win != null)
            win.BringForward();
    }

    public void ShowWindow(string windowName)
    {
        UIWindowItem win = GetWindow(windowName);
        if (win != null)
            win.Show();
    }

    public void HideWindow(string windowName)
    {
        UIWindowItem win = GetWindow(windowName);
        if (win != null)
            win.Hide();
    }

    public void ToggleWindow(string name,bool toggle)
    {
        if (toggle == true)
            ShowWindow(name);
        else
            HideWindow(name);
    }

    void AskWindowDown(string winName)
    {
        // 如果找不到该窗口，则请求下载 [4/1/2012 Ivan]
        foreach (_DBC_UI_LAYOUTDEFINE layout in allLayouts.StructDict.Values)
        {
            if (layout.szName == winName)
            {
                UIWindowItem window = new UIWindowItem(layout);
                //预加载窗口
                window.PreLoadWindow();

                allWindows.Add(window);
            }
        }
    }


    public UIWindowItem GetWindow(string windowName,bool withDownload)
    {
        foreach (UIWindowItem item in allWindows)
        {
            if (item.m_strWindowName == windowName)
            {
                return item;
            }
        }

        if (withDownload)
        {
            // 如果找不到该窗口，则请求下载 [4/1/2012 Ivan]
            AskWindowDown(windowName);
        }

        return null;
    }

    public UIWindowItem GetWindow(string windowName)
    {
        return GetWindow(windowName,true);
    }

    public GameObject GetWindowGo(string windowName)
    {
        UIWindowItem win = GetWindow(windowName);
        if (win != null)
            return win.winGo;
        return null;
    }

    public void HideAllWindow()
    {
        HideDemiseWindow(0);
    }

    public bool IsWindowShow(string winName)
    {
        UIWindowItem win = GetWindow(winName,false);
        if (win == null)
            return false;
        return win.winGo.active;
    }

    /// <summary>
    /// 关闭对应类型的窗口
    /// </summary>
    /// <param name="winType">对应配置表里面的Type</param>
    void HideDemiseWindow(int demiseType)
    {
        foreach (UIWindowItem item in allWindows)
        {
            if (item.DemiseType == demiseType)
            {
                if (item.IsWindowShow())
                {
                    item.Hide();
                }
            }
        }
    }

    internal void DemiseWindow(UIWindowItem window)
    {
        if (window.IsCanDemise())
        {
            switch (window.DemiseType)
            {
                case 2:
                    {
                        HideDemiseWindow(window.DemiseType);
                    }
                    break;
                case 3:
                    {
                        HideDemiseWindow(window.DemiseType);
                    }
                    break;
                case 4:
                case 6:
                    {
                        CloseAllWindow();
                        break;
                    }
                case 5: // 自适应窗口
                    {
                        //pItem->PositionSelf();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void CloseAllWindow()
    {
        foreach (UIWindowItem item in allWindows)
        {
            if (item.DemiseType != 0 && item.DemiseType !=6)
            {
                if (item.IsWindowShow())
                {
                    item.Hide();
                }
            }
        }

    }


    static MessageBoxSelf messageboxself;
    public static MessageBoxSelf messageBoxSelf
    {
        get
        {
            if (messageboxself == null)
            {
                GameObject mbox = Instance.GetWindowGo("MessageBoxSelf");
                messageboxself = mbox.GetComponent<MessageBoxSelf>();
            }
            Instance.BringWindowForward("MessageBoxSelf");
            return messageboxself;
        }
    }
}