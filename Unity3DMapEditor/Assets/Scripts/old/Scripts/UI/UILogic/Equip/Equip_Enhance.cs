using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;


public enum MODE
    {
	    TAB_Role,
	    TAB_Bag,
    }
public 	enum TAB_SELECT
	{
		TAB_1 = 0,
		TAB_2,
		TAB_3,
		TAB_4,
		TAB_5,
		TAB_6,
		TAB_7,
		TAB_8,
		TAB_9,
		TAB_10,
	}


public class Equip_Enhance : MonoBehaviour {
	
	
	public List<GameObject> Enhanceitems; //强化Item
	public GameObject EnhanceWindow; //强化界面
	public SpriteText EnchancePreLevel; //装备当前的强化等级
	public SpriteText EnchanceCurrentLevel; //装备的强化等级
	public SpriteText EnchanceNextLevel; //装备下一个强化等级
    public SpriteText EnhanceMoney;//装备强化所需的金钱
	public List<UIRadioBtn> EnchanceRadios;
	public SpriteText EnchancePageNum;
	public ActionButton Enchancebutton;
	public ActionButton QiangHuaButton;
	
	private List<CObject_Item_Equip> pItem;  //强化界面的装备
	private CObject_Item_Equip EnchanceItem;
	private List<SpriteText> EnhanceNames = new List<SpriteText>();  //强化装备
    private List<SpriteText> EnchanceLevels = new List<SpriteText>(); //装备强化等级
	
	MODE EnhanceMode = MODE.TAB_Role;
	int nTheIndex;
	int[] EnchanceIndex = new int[2];
	int[] EnhancePage = new int[2];   //保存装备强化界面上的背包，角色当前页
	int[] EnhancePageRadio = new int[2];  //判断哪一页的radioBtn点击
	
	TAB_SELECT[] EnchanceSelect = new TAB_SELECT[2];
	TAB_SELECT tabSelect = TAB_SELECT.TAB_1;
	
	void Awake()
	{
		foreach (GameObject item in Enhanceitems)
        {
            SpriteText[] texts = item.GetComponentsInChildren<SpriteText>();
            for (int i = 0; i < texts.Length; i++)
            {
				if(texts[i].gameObject.name == "Name")
                    EnhanceNames.Add(texts[i]);
                else if (texts[i].gameObject.name == "Level")
                    EnchanceLevels.Add(texts[i]);
            }
        }
		
		for(int i = 0; i < 2; i++)
		{
			EnhancePage[i] = 0;
			EnchanceIndex[i] = 0;
			EnhancePageRadio[i] = 0;
			EnchanceSelect[i] = TAB_SELECT.TAB_1;
		}
		
		//gameObject.SetActiveRecursively(false);
			
		registerHandler();
	}
	
	void registerHandler()
	{
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_EQUIP_ENHANCE, EquipStrengthen_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, EquipStrengthen_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_EQUIP, EquipStrengthen_Update);	
	}
	
	public void EquipStrengthen_Update(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_TOGGLE_EQUIP_ENHANCE)
	    {
			//if(!gameObject.active)
			//{
				EnhanceWindow.SetActiveRecursively(true);
			    Update_EnchanceEquip();
			//}
		}
		 else if(eventId == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED)
			 Update_EnchanceEquip();
		 else if(eventId == GAME_EVENT_ID.GE_UPDATE_EQUIP)
			 Update_EnchanceEquip();		
	}
	
    void Update_EnchanceEquip()          //更新强化界面
    {
		if (!gameObject.active)
			return;
		pItem = new List<CObject_Item_Equip>();
		if (EnhanceMode == MODE.TAB_Role)
		{
			int index = 0;
			for (int i = 0; i<10; i++)
			{
				if (index >= EnhanceNames.Count) break;
				CObject_Item_Equip equipItem = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)i) as CObject_Item_Equip;
				if (equipItem != null)
				{
					int EquipPoint = (int)equipItem.GetItemType();
					if (EquipPoint == 8 || EquipPoint == 9 || EquipPoint == 10)
					{
						continue; 
					}
					else
					{
						pItem.Add(equipItem);
                        index++;
                     }
                  }
			}
         }
         else if (EnhanceMode == MODE.TAB_Bag) 
		 {
			int index = 0;
			for (int i = 0; i < 60; i++)
			{
				CObject_Item_Equip packItem = CDataPool.Instance.UserBag_GetItemByIndex(i) as CObject_Item_Equip;
				if (packItem != null)
				{
					int EquipPoint = (int)packItem.GetItemType();
					if (EquipPoint == 8 || EquipPoint == 9 || EquipPoint == 10)
					{
						continue;
					}
					else
                    {
                        pItem.Add(packItem);

                        index++;
                    }
                 }
            }
          }
		
		nTheIndex = (int)EnhanceMode;
		IsShow(EnchanceIndex[nTheIndex]);
    }
	
	//清空强化界面
	void ClearEnhance()
	{
		for(int i = 0; i < 10; i++)
		  {
		    EnhanceNames[i].Text = "";
			EnchanceLevels[i].Text = "";
		    EnchanceRadios[i].Hide(true);
		  }
		EnchancePageNum.Text = "1/1";
        Enchancebutton.SetActionItem(-1);
        EnchancePreLevel.Text = "强化等级:";
        EnchanceCurrentLevel.Text = "";
        EnchanceNextLevel.Text = "";
	}
	
	void IsShow(int nIndex)
	{
		if(nIndex < pItem.Count)
			ShowEnhanceInfo(nIndex);
		else
			ClearEnhance();
	}
	
	//显示强化信息
	void ShowEnhanceInfo(int index)
	{
		 EnchanceItem =pItem[index];
		 Enchancebutton.UpdateItem(EnchanceItem.GetID());
		 int j = EnhancePage[nTheIndex] * 10;
		 for(int i = 0; i < 10; i++)
		 {
			if(i+j >= pItem.Count)
			{
				EnhanceNames[i].Text = "";
			    EnchanceLevels[i].Text = "";
				EnchanceRadios[i].Hide(true);
			}
			else
			{			
			   EnhanceNames[i].Text = pItem[i +j].GetName();
			   EnchanceLevels[i].Text = CDataPool.Instance.GetStrengthLevelDesc(pItem[i+j].GetStrengthLevel());
			   EnchanceRadios[i].Hide(false);
			}
		 }
		 
		 EnchancePreLevel.Text = "强化等级:" +CDataPool.Instance.GetStrengthLevelDesc(EnchanceItem.GetStrengthLevel());
		 EnchanceCurrentLevel.Text =  CDataPool.Instance.GetStrengthLevelDesc(EnchanceItem.GetStrengthLevel());
		 EnchanceNextLevel.Text =  CDataPool.Instance.GetStrengthNextLevelDesc(EnchanceItem.GetStrengthLevel());
		 int selectIndex = (int)EnchanceSelect[nTheIndex];
		 if(EnhancePage[nTheIndex] == EnhancePageRadio[nTheIndex])
		 {
		    EnchanceRadios[selectIndex].Value = true;
		 }
		 else
			EnchanceRadios[selectIndex].Value = false;
			
		 EnchancePageNum.Text = (EnhancePage[nTheIndex] +1) +"/" + GetPageNum();

         Item_Enhance itemEnhance = LifeAbility.Instance.Get_Equip_EnhanceLevel((CObject_Item)EnchanceItem);
         int NeedMoney = itemEnhance.needMoney;
         int Property = itemEnhance.addProperty;

         if (NeedMoney <= 0 || Property < 0)
         {
             if (EnchanceItem != null)
             {
                 EnhanceMoney.Text = "0";
                 //QiangHuaButton.Disable();
             }
             else
             {
                 LogManager.LogError("此装备无法强化。");
                 return;
             }
         }
         else
             QiangHuaButton.Enable();
	}
	
	//装备页数
	int GetPageNum()
	{
		int num;
		num = pItem.Count;
		int page = num/10;
		if(num > page*10 && num <= (page+1)*10)
			page++;
		return page;
	}
	
	
	void BagRadioClicked()   //背包按钮点击
	{
		EnhanceMode = MODE.TAB_Bag;
		//ShowEnhanceInfo(EnchanceIndex[nTheIndex]);
		Update_EnchanceEquip();
        //WhichWindowIsClick(MODE.TAB_Bag);
	}
	
	void RoleRadioClicked()  //角色按钮点击
	{
		EnhanceMode = MODE.TAB_Role;
		//ShowEnhanceInfo(EnchanceIndex[nTheIndex]);
		Update_EnchanceEquip();
        //WhichWindowIsClick(MODE.TAB_Role);
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
		EnhancePageRadio[nTheIndex] = EnhancePage[nTheIndex];
		EnchanceSelect[nTheIndex] = m_tab;
		EnchanceIndex[nTheIndex] = (int)m_tab + EnhancePage[nTheIndex]*10;
		IsShow(EnchanceIndex[nTheIndex]);
	}
	
	//切换到上一页
	void ChangeToPrePage()
	{
		int PageNum = GetPageNum();
		if(EnhancePage[nTheIndex] == 0)
		{
			LogManager.LogWarning("已经是第一页了");
		}
		else
		{
			EnhancePage[nTheIndex] = EnhancePage[nTheIndex]-1;
			ShowEnhanceInfo(EnchanceIndex[nTheIndex]);
		}
	}
	
	//切换到下一页
	void ChangeToNextPage()
	{
		int PageNum = GetPageNum();
		
		if(EnhancePage[nTheIndex] == PageNum-1)
		{
			LogManager.LogWarning("已经是最后一页了");
		}
		else
		{
			EnhancePage[nTheIndex]++;
			ShowEnhanceInfo(EnchanceIndex[nTheIndex]);
		}
	}
	
	//装备强化
	void StartStrength()
	{
        if (EnchanceItem == null)
        {
            LogManager.LogError("请放入一件装备");
            return;
        }
		LifeAbility.Instance.testStrengthen(EnchanceItem);
	}
}
