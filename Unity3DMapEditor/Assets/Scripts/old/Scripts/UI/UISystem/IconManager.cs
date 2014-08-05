using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IconManager
{
    static readonly IconManager sInstance = new IconManager();
    static public IconManager Instance { get { return sInstance; } }

    public void Init(string iconResPath)
    {
        ResourceManager.Me.DownloadAsync(iconResPath, null, CommonWindowDownload);
    }

    AssetBundle allIcons;
    Texture2D emptyIcon;
    public void CommonWindowDownload(System.Object obj, AssetBundle asset)
    {
        if (asset != null)
        {
            allIcons = asset;
            emptyIcon = GetIcon("wuqi_4");// 修改默认 [3/9/2012 SUN]
        }
        else
            LogManager.LogError("allIcons res is empty.");
    }

    public Texture2D GetIcon(string iconName)
    {
		if (iconName == "") {
			return emptyIcon;
		}
        UnityEngine.Object res = allIcons.Load(iconName);
        Texture2D icon = res as Texture2D;
        if (icon == null)
        { 
            LogManager.LogWarning("icon:" + iconName + " is null");
            return emptyIcon;
        }
        else
            return icon;
    }
}
