using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class Equip_Combine : MonoBehaviour {
	
	public List<GameObject> CombineItems; //宝石合成Item
	public GameObject CombineWindow;
	public List<UIRadioBtn> CombineRadios;
	public List<ActionButton> CombineIcons; //宝石合成界面的Icons
	public CObject_Item_Gem[] IconItems;
	
	private List<SpriteText> CombineNames = new List<SpriteText>(); //宝石合成材料
    private List<SpriteText> CombineNums = new List<SpriteText>(); //宝石合成数量
	private List<CObject_Item_Gem> cItem; //宝石合成界面的宝石材料
	private CObject_Item_Gem combineItem;
	
	int CombinePage = 0;
	int CombineIndex = 0;
	TAB_SELECT tabSelect = TAB_SELECT.TAB_1;
	
	void Awake()
	{
		foreach (GameObject item in CombineItems)
        {
            SpriteText[] texts = item.GetComponentsInChildren<SpriteText>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].gameObject.name == "Name")
                    CombineNames.Add(texts[i]);
                else if (texts[i].gameObject.name == "Num")
                    CombineNums.Add(texts[i]);
            }
        }
		
		registerHandler();
	}
	
	void registerHandler()
	{
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_STONE_COMBINE, StoneCombine_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, StoneCombine_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_EQUIP, StoneCombine_Update);	
	}
	
	public void StoneCombine_Update(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_TOGGLE_STONE_COMBINE)
		{
			if(!gameObject.active)
			{
				CombineWindow.SetActiveRecursively(true);
			    Update_CombineStone();
			}
		}
		else if(eventId == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED)
			Update_CombineStone();
		else if(eventId == GAME_EVENT_ID.GE_UPDATE_EQUIP)
			Update_CombineStone();
	}
	
	//更新宝石合成界面
	void Update_CombineStone()
	{
		if(!gameObject.active)
			return;
		int j = 0;
		cItem = new List<CObject_Item_Gem>();
		for(int i = 0; i < 60; i++)
		{
			CObject_Item_Gem item = CDataPool.Instance.UserBag_GetItemByIndex(i) as CObject_Item_Gem;
			if(item != null)
			{
				int count = CDataPool.Instance.UserBag_CountGemByIDTable(item.GetIdTable());
				if(count != 0)
				{
					if(cItem.Count == 0)
						cItem.Add(item);
					else
					{
						for(j = 0; j < cItem.Count; j++)
						{
							if(item.GetIdTable() == cItem[j].GetIdTable())
								break;
							else
							{
								if(j == cItem.Count -1)
									cItem.Add(item);
							}
						}
					}
				}
			}
		}
		
		IsShow(CombineIndex);
	}
	
	void IsShow(int nIndex)
	{
		if(nIndex < cItem.Count)
			ShowCombineInfo(nIndex);
		else
			ClearCombine();
	}
	
	//清空宝石合成界面
	void ClearCombine()
	{
		for (int i = 0; i < 10; i++)
        {
            CombineNames[i].Text = "";
            CombineNums[i].Text = "";
            CombineRadios[i].Hide(true);
        }
        for (int i = 0; i < 5; i++)
            CombineIcons[i].SetActionItem(-1);
	}
	
	// 清空宝石合成界面
    void ShowCombineInfo(int index)
    {
        combineItem = cItem[index];
        int j = CombinePage * 10;
        for (int i = 0; i < 10; i++)
        {
            if (i + j >= cItem.Count)
            {
                CombineNames[i].Text = "";
                CombineNums[i].Text = "";
                CombineRadios[i].Hide(true);
            }
            else
            {
                CombineNames[i].Text = cItem[i + j].GetGemLevel() +"级"+ cItem[i + j].GetName();
                CombineNums[i].Text = CDataPool.Instance.UserBag_CountGemByIDTable(cItem[i + j].GetIdTable()).ToString();
                CombineRadios[i].Hide(false);
            }
        }
        int count = CDataPool.Instance.UserBag_CountGemByIDTable(combineItem.GetIdTable());
        if (count == 0)
        {
            for (int i = 0; i < 5; i++)
                CombineIcons[i].SetActionItem(-1);
        }
        else
        {
            if (count >= 5)
                count = 5;
			for(int i = 0; i < 5; i++)
			{
				CombineIcons[i].SetActionItem(-1);
			}
			
			IconItems = new CObject_Item_Gem[count];
            for (int i = 0; i < count; i++)
			{
                CombineIcons[i].UpdateItem(combineItem.GetID());
				IconItems[i] = combineItem;
			}
        }

    }
	
	void Tab1_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_1);
	}
	
    void Tab2_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_2);
	}
	
	void Tab3_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_3);
	}
	
	void Tab4_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_4);
	}
	
	void Tab5_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_5);
	}
	
	void Tab6_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_6);
	}
	
	void Tab7_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_7);
	}
	
	void Tab8_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_8);
	}
	
	void Tab9_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_9);
	}
	
	void Tab10_Click()
	{
		HandlerChangeTab(TAB_SELECT.TAB_10);
	}
	
	void HandlerChangeTab(TAB_SELECT m_tab)
	{
		tabSelect = m_tab;
		CombineIndex = (int)m_tab + CombinePage * 10;
		ShowCombineInfo(CombineIndex);
	}
	
	//切换到上一页
	void ChangeToPrePage()
	{
		int PageNum = GetPageNum();
		if(CombinePage == 0)
		{
			LogManager.LogWarning("已经是第一页了");
		}
		else
		{
			CombinePage = CombinePage-1;
			ShowCombineInfo(CombineIndex);
		}
	}
	
	//切换到下一页
	void ChangeToNextPage()
	{
		int PageNum = GetPageNum();
		
		if(CombinePage == PageNum-1)
		{
			LogManager.LogWarning("已经是最后一页了");
		}
		else
		{
			CombinePage++;
			ShowCombineInfo(CombineIndex);
		}
	}
	
	int GetPageNum()
	{
		int num;
		num = cItem.Count;
		int page = num/10;
		if(num > page*10 && num <= (page+1)*10)
			page++;
		return page;
	}
	
	//宝石合成
	void Begin_Combine()
	{
		LifeAbility.Instance.Do_Combine(IconItems);
	}
}
