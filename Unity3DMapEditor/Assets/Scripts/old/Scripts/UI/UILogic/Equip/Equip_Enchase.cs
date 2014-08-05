using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class Equip_Enchase : MonoBehaviour {
	
	public GameObject EnchaseWindow;
	public List<GameObject> EnchaseItems; //镶嵌Item
	public List<UIRadioBtn> EnchaseRadios;
	public GameObject StoneContainer;//宝石侧栏
    public GameObject EquipContainer;//装备侧栏
	public ActionButton EnchaseEquipIcon; //宝石镶嵌时的装备Icon
	public List<ActionButton> StoneIcons; //宝石Icon
	public List<UIButton> SelectStonesBtn; //选择宝石按钮
	public List<UIButton> RemoveBtns;  //摘除按钮
	public SpriteText EnchasePageNum;
	public List<SpriteText> StoneInfo;//宝石的信息
	
	private List<CObject_Item_Equip> eItem; //宝石镶嵌界面的装备
	private List<SpriteText> EnchaseNames = new List<SpriteText>(); //镶嵌装备
    private List<SpriteText> EnchaseNums = new List<SpriteText>(); //宝石的数量
	private List<CObject_Item_Gem> GemItem;
    private List<CObject_Item_Gem> cItem; //宝石合成界面的宝石材料
	
	private CObject_Item_Gem[] StoneItem = new CObject_Item_Gem[3];
	private CObject_Item_Equip EnchaseItem;
	
	MODE EnchaseMode = MODE.TAB_Role;
	WINDOWTYPE m_Type = WINDOWTYPE.enchase;
	
	int nTheIndex = 0;
	int[] EnchaseIndex = new int[2];
	int[] EnchasePage = new int[2];
	int[] EnchasePageRadio = new int[2];
	
    int StonePage = 0;
	int StoneIndex = 0;
    int StoneBtn = 0;  //哪个选择宝石按钮按下
	TAB_SELECT[] EnchaseSelect = new TAB_SELECT[2];
	TAB_SELECT tabSelect = TAB_SELECT.TAB_1;
	
	
	
	enum WINDOWTYPE
	{
		enchase,
		stone,
	}
	
	void Awake()
	{
		foreach(GameObject item in EnchaseItems)
		{
			SpriteText[] texts = item.GetComponentsInChildren<SpriteText>();
			for(int i = 0; i < texts.Length; i++)
			{
				if(texts[i].gameObject.name == "Name")
					EnchaseNames.Add(texts[i]);
				else if(texts[i].gameObject.name == "Num")
					EnchaseNums.Add(texts[i]);
			}
		}
		
		for(int i = 0; i < 2; i++)
		{
            EnchasePage[i] = 0;
            EnchaseIndex[i] = 0;
            EnchasePageRadio[i] = 0;
            EnchaseSelect[i] = TAB_SELECT.TAB_1;        

		}
		
		registerHandler();

	}
	
	void registerHandler()
	{
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_STONE_ENCHASE, EquipEnchase_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, EquipEnchase_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_EQUIP, EquipEnchase_Update);
		
	}
		
    public void EquipEnchase_Update(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_TOGGLE_STONE_ENCHASE)
		{
			if(!gameObject.active)
			{
				EnchaseWindow.SetActiveRecursively(true);
			    ShowWhichWindow();
			}
		}
		else if(eventId == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED)
		{
			//ShowWhichWindow();
			if(m_Type == WINDOWTYPE.enchase)
				Update_EnchaseEquip();
			else if(m_Type == WINDOWTYPE.stone)
				Update_Stone();
		}
		else if(eventId == GAME_EVENT_ID.GE_UPDATE_EQUIP)
		{
			if(m_Type == WINDOWTYPE.enchase)
				Update_EnchaseEquip();
			else if(m_Type == WINDOWTYPE.stone)
				Update_Stone();
		}
	}
	
	//更新宝石镶嵌界面
	void Update_EnchaseEquip()
	{
		if(!gameObject.active)
			return;
		eItem = new List<CObject_Item_Equip>();
		if(EnchaseMode == MODE.TAB_Role)
		{
			int index = 0;
			for(int i = 0; i < 10; i++)
			{
				if(index >= EnchaseNames.Count) break;
				CObject_Item_Equip Item = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)i) as CObject_Item_Equip;
				if(Item != null)
				{
					int EquipPoint = (int)Item.GetItemType();
                    if (EquipPoint == 8 || EquipPoint == 9 || EquipPoint == 10)
					{
						continue;
					}
					else
					{
						eItem.Add(Item);
						index++;
					}
				}
			}
		}
		else if(EnchaseMode == MODE.TAB_Bag)
		{
			int index = 0;
			for(int i = 0; i < 60; i++)
			{
				CObject_Item_Equip Item = CDataPool.Instance.UserBag_GetItemByIndex(i) as CObject_Item_Equip;
				if(Item != null)
				{
					int EquipPoint = (int)Item.GetItemType();
                    if (EquipPoint == 8 || EquipPoint == 9 || EquipPoint == 10)
					{
						continue;
					}
					else
					{
						eItem.Add(Item);
						index++;
					}
				}
			}
		}
		
		nTheIndex = (int)EnchaseMode;
        IsShow(EnchaseIndex[nTheIndex]);
	}
	
	//更新宝石信息
	void Update_Stone()
	{
		if(m_Type == WINDOWTYPE.stone)
		{
			int j = 0;
			if(!gameObject.active)
				return;
			GemItem = new List<CObject_Item_Gem>();
			for(int i = 0; i < 60; i++)
			{
				CObject_Item_Gem item = CDataPool.Instance.UserBag_GetItemByIndex(i) as CObject_Item_Gem;
				if(item != null)
				{
					int count = CDataPool.Instance.UserBag_CountGemByIDTable(item.GetIdTable());
					if(count != 0)
					{
						if(GemItem.Count == 0)
							GemItem.Add(item);
						else
						{
							for(j = 0; j < GemItem.Count; j++)
							{
								if(item.GetIdTable() == GemItem[j].GetIdTable())
									break;
								else
								{
									if(j == GemItem.Count-1)
									{
										GemItem.Add(item);
									}
								}
							}
						}
					}
				}
			}
		}
		
		IsShow(EnchaseIndex[0]);
	}
	
	void IsShow(int nIndex)
	{
		if(m_Type == WINDOWTYPE.enchase)
		{
			if(nIndex < eItem.Count)
				ShowEnchaseInfo(nIndex);
			else
				ClearEnchase();
		}
		else if(m_Type == WINDOWTYPE.stone)
		{
			if(nIndex < GemItem.Count)
				ShowStone(nIndex,StoneBtn);
			else
				ClearEnchase();
		}
	}
	
	void ShowWhichWindow()
	{
		if(m_Type == WINDOWTYPE.enchase)
		{
			EquipContainer.SetActiveRecursively(true);
			StoneContainer.SetActiveRecursively(false);
			Update_EnchaseEquip();
		}
		else if(m_Type == WINDOWTYPE.stone)
		{
			StoneContainer.SetActiveRecursively(true);
			EquipContainer.SetActiveRecursively(false);
			ShowStone(StoneIndex,StoneBtn);
		}
	}
	
	//显示宝石信息
	void ShowStone(int index,int pos)
	{
		StoneItem[pos] = GemItem[index];
		int j = StonePage * 10;

        for (int i = 0; i < 10; i++)
        {
            if (i + j >= GemItem.Count)
            {
                EnchaseNames[i].Text = "";
                EnchaseNums[i].Text = "";
                EnchaseRadios[i].Hide(true);
            }
            else
            {
                EnchaseNames[i].Text = GemItem[i + j].GetName();
				EnchaseNums[i].Text = CDataPool.Instance.UserBag_CountGemByIDTable(GemItem[i+j].GetIdTable()).ToString();
                EnchaseRadios[i].Hide(false);
            }
        }

        StoneIcons[pos].UpdateItem(StoneItem[pos].GetID());
		//StoneInfo[pos].Text = StoneItem[pos].GetItemLevelDesc().ToString();
		StoneInfo[pos].Text = StoneItem[pos].GetGemArribInfo();
	}
	
	//清空镶嵌界面
	void ClearEnchase()
	{
		 for (int i = 0; i < 10; i++)
        {
            EnchaseNames[i].Text = "";
            EnchaseNums[i].Text = "";
            EnchaseRadios[i].Hide(true);
        }
		EnchaseEquipIcon.SetActionItem(-1);
		for(int i = 0; i < 3; i++)
		{
			SelectStonesBtn[i].Hide(false);
			RemoveBtns[i].Hide(true);
		}
	}
	
	//显示镶嵌信息
	void ShowEnchaseInfo(int index)
	{
		EnchaseItem = eItem[index];
		EnchaseEquipIcon.UpdateItem(EnchaseItem.GetID());
		int count = EnchaseItem.GetGemCount();
		int i = 0;
		//ShowStoneInfo(count);

        int j = EnchasePage[nTheIndex] * 10;
        for (i = 0; i < 10; i++)
        {
            if (i + j >= eItem.Count)
            {
                EnchaseNames[i].Text = "";
                EnchaseNums[i].Text = "";
                EnchaseRadios[i].Hide(true);
            }
            else
            {
                EnchaseNames[i].Text = eItem[i+j].GetName();
                EnchaseNums[i].Text = eItem[i+j].GetGemCount().ToString();
                EnchaseRadios[i].Hide(false);
            }
        }
		
		 int selectIndex = (int)EnchaseSelect[nTheIndex];
            if (EnchasePage[nTheIndex] == EnchasePageRadio[nTheIndex])
            {
                EnchaseRadios[selectIndex].Value = true;
            }
            else
                EnchaseRadios[selectIndex].Value = false;

            EnchasePageNum.Text = (EnchasePage[nTheIndex] + 1) + "/" + GetPageNum();

		int k = 0;
		for(k = 0; k < 3; k++)
		{
			if(StoneItem[k] == null)
			{
				StoneIcons[k].SetActionItem(-1);
				StoneInfo[k].Text = "未镶嵌";
				SelectStonesBtn[k].Hide(false);
				RemoveBtns[k].Hide(true);
			}
		}
	}
	
	int GetPageNum()
	{
		int num;
		if(m_Type == WINDOWTYPE.enchase)
		{
			num = eItem.Count;
		}
		else
		{
			num = GemItem.Count;
		}
		int page = num/10;
		if(num > page*10 && num <= (page+1)*10)
			page++;
		return page;
	}
	
	//选择装备按钮
    void SelectEquip()
    {
		//m_Window = Equip_Window.ENCHASE;
		m_Type = WINDOWTYPE.enchase;
		EnchaseMode = MODE.TAB_Role;
		//RoleBtn.Value = true;
        EquipContainer.SetActiveRecursively(true);
        StoneContainer.SetActiveRecursively(false);
        Update_EnchaseEquip();
    }
	
	void BagRadioClicked()   //背包按钮点击
	{
		EnchaseMode = MODE.TAB_Bag;
		Update_EnchaseEquip();
	}
	
	void RoleRadioClicked()  //角色按钮点击
	{
		EnchaseMode = MODE.TAB_Role;
		Update_EnchaseEquip();
	}
	
	void ChangeToFirstStone()
	{
		HandlerStonePage(0);
	}
	
	void ChangeToSecondStone()
	{
		HandlerStonePage(1);
	}
	
	void ChangeToThirdPage()
	{
		HandlerStonePage(2);
	}
	
	void HandlerStonePage(int index)
	{
		if(index < 0 || index > 2)
			return;

        StoneBtn = index;
		//m_Window = Equip_Window.STONE;
		m_Type = WINDOWTYPE.stone;
        EquipContainer.SetActiveRecursively(false);
        StoneContainer.SetActiveRecursively(true);
        Update_Stone();	
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
		if(m_Type == WINDOWTYPE.enchase)
		{
			EnchasePageRadio[nTheIndex] = EnchasePage[nTheIndex];
			EnchaseSelect[nTheIndex] = m_tab;
			EnchaseIndex[nTheIndex] = (int)m_tab + EnchasePage[nTheIndex]*10;
			IsShow(EnchaseIndex[nTheIndex]);
		}
		else if(m_Type == WINDOWTYPE.stone)
		{
			StoneIndex = (int)m_tab + StonePage *10;
			ShowStone(StoneIndex,StoneBtn);
		}
	}
	
	//切换到上一页
	void ChangeToPrePage()
	{
		int PageNum = GetPageNum();
		if(EnchasePage[nTheIndex] == 0)
		{
			LogManager.LogWarning("已经是第一页了");
		}
		else
		{
			EnchasePage[nTheIndex] = EnchasePage[nTheIndex]-1;
			ShowEnchaseInfo(EnchaseIndex[nTheIndex]);
		}
	}
	
	//切换到下一页
	void ChangeToNextPage()
	{
		int PageNum = GetPageNum();
		
		if(EnchasePage[nTheIndex] == PageNum-1)
		{
			LogManager.LogWarning("已经是最后一页了");
		}
		else
		{
			EnchasePage[nTheIndex]++;
			ShowEnchaseInfo(EnchaseIndex[nTheIndex]);
		}
	}
	
	//宝石镶嵌
	void Begin_Enchase()
	{
        //for(int i = 0; i < 3; i++)
        //{
        //    if(StoneItem[i] != null)
        //    {
        //        LifeAbility.Instance.Do_Enchase(EnchaseItem,StoneItem[i],(byte)i);
        //    }
        //}
	}
}
