using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;
public class FunyinInfo
{
	public GameObject Locator;
	public GameObject charm;
}
public class CharmScript : MonoBehaviour {
    const string charmWindowName = "FuYinWindow";
    public static CharmScript sSelf;
	//public GameObject CharmInfo;
	public GameObject CharmName;
	public GameObject percentInfo;
	GameObject fuyinGo;
    GameObject addFuyinGo;
    FunyinInfo[] fuyinInfoLevel1 = new FunyinInfo[5];
    FunyinInfo[] fuyinInfoLevel2 = new FunyinInfo[6];
    FunyinInfo[] fuyinInfoLevel3 = new FunyinInfo[7];
    FunyinInfo[] fuyinInfoLevel4 = new FunyinInfo[8];
    FunyinInfo[] fuyinInfoLevel5 = new FunyinInfo[9];
    int currentAttr = 0;
    void Awake()
    {
        gameObject.SetActiveRecursively(true);
        sSelf = this;
		fuyinGo = getChildGO("FuYin");
		fuyinGo.renderer.enabled = false;
		addFuyinGo = getChildGO("AddFuYin");
		addFuyinGo.renderer.enabled = false;
        int fNum = 1;
        for (int i = 0; i < 5; ++i )
        {
            fuyinInfoLevel1[i] = new FunyinInfo();
            
            fuyinInfoLevel1[i].Locator = getChildGO("F" + fNum.ToString());
            fuyinInfoLevel1[i].charm = null;
            fNum++;
        }
        for (int i = 0; i < 6; ++i)
        {
            fuyinInfoLevel2[i] = new FunyinInfo();
           
            fuyinInfoLevel2[i].Locator = getChildGO("F" + fNum.ToString());
            fuyinInfoLevel2[i].charm = null;
            fNum++;
        }
        for (int i = 0; i < 7; ++i)
        {
            fuyinInfoLevel3[i] = new FunyinInfo();
           
            fuyinInfoLevel3[i].Locator = getChildGO("F" + fNum.ToString());
            fuyinInfoLevel3[i].charm = null;
            fNum++;
        }
        for (int i = 0; i < 8; ++i)
        {
            fuyinInfoLevel4[i] = new FunyinInfo();
           
            fuyinInfoLevel4[i].Locator = getChildGO("F" + fNum.ToString());
            fuyinInfoLevel4[i].charm = null;
            fNum++;
        }
        for (int i = 0; i < 9; ++i)
        {
            fuyinInfoLevel5[i] = new FunyinInfo();
            
            fuyinInfoLevel5[i].Locator = getChildGO("F" + fNum.ToString());
            fuyinInfoLevel5[i].charm = null;
            fNum++;
        }
		//gameObject.SetActiveRecursively(false);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_CHARMWINDOW, OnEvent);
        OnClickLiliang();
    }
    void OnEnable()
    {
        
    }
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParams)
    {
        if (eventId == GAME_EVENT_ID.GE_TOGLE_CHARMWINDOW)
        {
			
        }
    }

    void ShowWindow()
    {
        UIWindowMng.Instance.ShowWindow(charmWindowName);
        flush();
    }
    void HideWindow()
    {
        UIWindowMng.Instance.HideWindow(charmWindowName);
    }
	public void CloseWindow()
    {
        UIWindowMng.Instance.HideWindow(charmWindowName);
    }
	GameObject getChildGO(string name)
	{
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
		foreach(Transform t in trans)
		{
			if(t.gameObject.name == name)
				return t.gameObject;
		}
		return null;
	}
	GameObject newFuyinGo(GameObject GO)
	{
		GameObject newFuYin = (GameObject)Object.Instantiate(GO);
		newFuYin.renderer.enabled = true;
		return newFuYin;
	}
    void showCharm(int attr)
    {
        showCharmLevel(attr, fuyinInfoLevel1, 0);
        showCharmLevel(attr, fuyinInfoLevel2, 1);
        showCharmLevel(attr, fuyinInfoLevel3, 2);
        showCharmLevel(attr, fuyinInfoLevel4, 3);
        showCharmLevel(attr, fuyinInfoLevel5, 4);
    }
    void showCharmLevel(int attr, FunyinInfo[] fuyinInfoLevel, int level)
    {
        int countLevel = CDataPool.Instance.PlayerCharmInfo[attr,level];
        for (int i = 0; i < fuyinInfoLevel.Length; ++i)
        {
            if (i < countLevel)
            {
				if(fuyinInfoLevel[i].charm != null) Object.Destroy(fuyinInfoLevel[i].charm);
                fuyinInfoLevel[i].charm = newFuyinGo(fuyinGo);
				
                GFX.GfxUtility.attachGameObject(fuyinInfoLevel[i].charm, fuyinInfoLevel[i].Locator);
            }
			else if(i==countLevel)
			{
				if(fuyinInfoLevel[i].charm != null) Object.Destroy(fuyinInfoLevel[i].charm);
                fuyinInfoLevel[i].charm = newFuyinGo(addFuyinGo);
                GFX.GfxUtility.attachGameObject(fuyinInfoLevel[i].charm, fuyinInfoLevel[i].Locator);
                AddCharmScript addCharmScript = fuyinInfoLevel[i].charm.GetComponent<AddCharmScript>();
				addCharmScript.attr = currentAttr + 1;//从1开始
				addCharmScript.level = level +1 ;
			}
            else
            {
                if (fuyinInfoLevel[i].charm != null)
                {
                    Object.Destroy(fuyinInfoLevel[i].charm);
                    fuyinInfoLevel[i].charm = null;
                }
            }
        }
    }
    int getAdditionAttr(int attr)
    {
		int n = CDataPool.Instance.PlayerCharmInfo[attr,0];
       int totalAttr = (5+ 5 + n -1)*n/2;
		n = CDataPool.Instance.PlayerCharmInfo[attr,1];
		totalAttr += (10 + 10 + n -1 )*n/2;
		n = CDataPool.Instance.PlayerCharmInfo[attr,2];
		totalAttr += (15 + 15 + n-1 )*n/2;
		
		n = CDataPool.Instance.PlayerCharmInfo[attr,3];
		totalAttr += (20 + +20 + n -1 )*n/2;
		
		n = CDataPool.Instance.PlayerCharmInfo[attr,4];
		totalAttr += (25 + 25 + n -1)*n/2;
		return totalAttr;
    }
    int getPercentAttr(int attr)
    {
        int total = (5+9)*5/2 + (10+15)*6/2 + (15 + 21)*7/2 + (20 + 27)*8/2 + (25+33)*9/2;
		float percent = getAdditionAttr(attr)/(float)total;
		return (int)(percent*100);
    }
    public void OnClickLiliang()
	{
		//CharmInfo.GetComponent<SpriteText>().Text = "力量符印";
        CharmName.GetComponent<SpriteText>().Text = "力量: " + getAdditionAttr(0);
		percentInfo.GetComponent<SpriteText>().Text = "完成度：" + getPercentAttr(0) + " %";
        currentAttr = 0;
        showCharm(0);
	}
	
	public void OnClickZhiHui()
	{
		//CharmInfo.GetComponent<SpriteText>().Text = "智慧符印";
		CharmName.GetComponent<SpriteText>().Text = "智慧: " + getAdditionAttr(1);
		percentInfo.GetComponent<SpriteText>().Text = "完成度：" + getPercentAttr(1) + " %";
        currentAttr = 1;
        showCharm(1);
	}
	public void OnClickTiZhi()
	{
		//CharmInfo.GetComponent<SpriteText>().Text = "体质符印";
		CharmName.GetComponent<SpriteText>().Text = "体质: " + getAdditionAttr(2);
		percentInfo.GetComponent<SpriteText>().Text = "完成度：" + getPercentAttr(2) + " %";
        currentAttr = 2;
        showCharm(2);
	}
	public void OnClickRenXing()
	{
		//CharmInfo.GetComponent<SpriteText>().Text = "韧性符印";
		CharmName.GetComponent<SpriteText>().Text = "韧性: " + getAdditionAttr(3);
		percentInfo.GetComponent<SpriteText>().Text = "完成度：" + getPercentAttr(3) + " %";
        currentAttr = 3;
        showCharm(3);
	}
	public void OnClickMinJie()
	{
		//CharmInfo.GetComponent<SpriteText>().Text = "敏捷符印";
		CharmName.GetComponent<SpriteText>().Text = "敏捷: " + getAdditionAttr(4);
		percentInfo.GetComponent<SpriteText>().Text = "完成度：" + getPercentAttr(4) + " %";
        currentAttr = 4;
        showCharm(4);
	}
	public void OnShowMakeCharmWindow()
	{
		GameObject MakeFuyinWindow =  UIWindowMng.Instance.GetWindowGo("FuYinMaker");
        if(MakeFuyinWindow != null)
        {
            UIWindowMng.Instance.ToggleWindow("FuYinMaker", !MakeFuyinWindow.active);
        }
	}
    public void flush()
    {
        if (gameObject.active == false) return;
        switch(currentAttr)
        {
            case 0:
                OnClickLiliang();
                break;
            case 1:
                OnClickZhiHui();
                break;
            case 2:
                OnClickTiZhi();
                break;
            case 3:
                OnClickRenXing();
                break;
            case 4:
                OnClickMinJie();
                break;
        }
    }

}
