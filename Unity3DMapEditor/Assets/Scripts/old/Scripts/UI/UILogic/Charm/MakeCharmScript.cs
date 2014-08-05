using UnityEngine;
using System.Collections;

public class MakeCharmScript : MonoBehaviour {
	
	int attr = -1;
	int level = -1;
	int[,] charmPrescrID = new int[,] { {161,162,163,164,165}, {166,167,168,169,170}, {171,172,173,174,175}, {176,177,178,179,180}, {181,182,183,184,185}};
	// Use this for initialization
	void Start () {
	}
	const string windowName = "FuYinMaker";
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		attr = 0;
		level = 0;
	}
	public void CloseWindow()
    {
        UIWindowMng.Instance.HideWindow(windowName);
    }

	public  void OnClickLiliang()
	{
		attr = 0;
	}
	public void OnClickZhiHui()
	{
		attr = 1;
	}
	public void OnClickTiZhi()
	{
		attr = 2;
	}
	public void OnClickRenXing()
	{
		attr = 3;
	}
	public void OnClickMinJie()
	{
		attr = 4;
	}
	public void OnLevel1()
	{
		level = 0;
	}
	public void OnLevel2()
	{
		level = 1;
	}
	public void OnLevel3()
	{
		level = 2;
	}
	public void OnLevel4()
	{
		level = 3;
	}
	public void OnLevel5()
	{
		level = 4;
	}

    string updateStuffInfo()
    { 
        int stuffCount = level+1;
        string stuffString = "需要材料： " + "符印石x" + stuffCount.ToString() + " " + "血精石x" + stuffCount.ToString() + " " + "育灵水x" + stuffCount.ToString();
        string specialStuff = "";
        switch(attr)
        {
            case 0:
                specialStuff = "力量之魂";
                break;
            case 1:
                specialStuff = "智慧之魂";
                break;
            case 2:
                specialStuff = "体质之魂";
                break;
            case 3:
                specialStuff = "韧性之魂";
                break;
            case 4:
                specialStuff = "敏捷之魂";
                break;
        }
        stuffString += " " + specialStuff + "x" + stuffCount.ToString();
        return stuffString;
    }
	void OnMakeCharm()
	{
        if (Interface.LifeAbility.Instance.ComposeItem_Begin(charmPrescrID[attr, level], 1) == COMPOSE_ITEM_RESULT.COMPOSE_NO_ENOUGHSTUFF)
        {
            string stuffString = updateStuffInfo();
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, stuffString);
        }
	}
}
