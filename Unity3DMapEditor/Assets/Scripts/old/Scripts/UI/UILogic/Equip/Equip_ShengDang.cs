using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class Equip_ShengDang : MonoBehaviour {
	
	
	public GameObject ShengDangWindow;
	public List<GameObject> ShengDangItems;//升档Item
	public List<UIRadioBtn> ShengDangRadios;
	public List<SpriteText> MaterialName;  //装备升档消耗的材料名字
	public List<SpriteText> MaterialNum; //装备升档消耗的材料数量
	public SpriteText ShengDangPageNum;
	public List<ActionButton> ShengDangImageButton;
	public List<ActionButton> MaterialButton;
	public SpriteText ShengDangPreEnchanceLevel; //装备当前的强化等级
	public SpriteText ShengDangCurrentDangCi; //装备当前的档次
	public SpriteText ShengDangNextDangCi; //装备的下一个档次
	public SpriteText ShengDangMoney;
	
	private List<CObject_Item_Equip> sItem;  //升档界面的装备
	private List<SpriteText> ShengDangNames = new List<SpriteText>();
    private List<SpriteText> ShengDangLevels = new List<SpriteText>();//装备档次
	private CObject_Item_Equip ShengDangItem;
	
	
	int nTheIndex = 0;
	int[] ShengDangIndex = new int[2];
	int[] ShengDangPage = new int[2];
	int[] ShengDangPageRadio = new int[2];
	TAB_SELECT[] ShengDangSelect = new TAB_SELECT[2];
	TAB_SELECT tabSelect = TAB_SELECT.TAB_1;
	
	MODE ShengDangMode = MODE.TAB_Role;
	
	void Awake()
	{
		foreach (GameObject item in ShengDangItems)
		{
			SpriteText[] texts = item.GetComponentsInChildren<SpriteText>();
			for(int i =0; i < texts.Length; i++)
			{
                if (texts[i].gameObject.name == "Name")
                    ShengDangNames.Add(texts[i]);
                else if (texts[i].gameObject.name == "Level")
                    ShengDangLevels.Add(texts[i]);					
			}
		}
		
		
		for(int i = 0; i < 2; i++)
		{
			ShengDangPage[i] = 0;
			ShengDangIndex[i] = 0;
			ShengDangPageRadio[i] = 0;
			ShengDangSelect[i] = TAB_SELECT.TAB_1;
		}
		
		//gameObject.SetActiveRecursively(false);
			
		registerHandler();
	}
	
	void registerHandler()
	{
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_EQUIP_SHENGDANG, EquipLevelUp_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, EquipLevelUp_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_EQUIP, EquipLevelUp_Update);
		
	}
		
    public void EquipLevelUp_Update(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_TOGGLE_EQUIP_SHENGDANG)
		{
			if(!gameObject.active)
			{
				ShengDangWindow.SetActiveRecursively(true);
			    Update_ShengDangEquip();
			}
		}
	    else if(eventId == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED)
			Update_ShengDangEquip();
		else if(eventId == GAME_EVENT_ID.GE_UPDATE_EQUIP)
			Update_ShengDangEquip();
	}
	
	//更新装备升档界面
	void Update_ShengDangEquip()
	{
		if(!gameObject.active)
			return;
		sItem = new List<CObject_Item_Equip>();
		if(ShengDangMode == MODE.TAB_Role)
		{
			int index = 0;
			for(int i = 0; i < 10; i++)
			{
				if(index >= ShengDangNames.Count) break;
				CObject_Item_Equip equipItem = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)i) as CObject_Item_Equip;
				if(equipItem != null)
				{
					int EquipPoint = (int)equipItem.GetItemType();
                    if (EquipPoint == 8 || EquipPoint == 9 || EquipPoint == 10)
					{
						continue;
                    }
					else
					{
                        sItem.Add(equipItem);
                        index++;
					}
				}
			}
		}
		else if(ShengDangMode == MODE.TAB_Bag)
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
						sItem.Add(packItem);
                        index++;
					}
                }
           }
		}
		
		nTheIndex = (int)ShengDangMode;
		IsShow(ShengDangIndex[nTheIndex]);
	}
	
	void IsShow(int nIndex)
	{
		if(nIndex < sItem.Count)
			ShowShengDangInfo(nIndex);
		else
			ClearShengDang();
	}
	
	//清空升档信息
	void ClearShengDang()
	{
		for(int i = 0; i < 10; i++)
		{
			ShengDangNames[i].Text = "";
			ShengDangLevels[i].Text = "";
		    ShengDangRadios[i].Hide(true);
		}
		for(int i = 0; i < 2; i++)
		{
			MaterialName[i].Text = "";
			MaterialNum[i].Text = "";
			MaterialButton[i].SetActionItem(-1);
		}
		ShengDangPageNum.Text = "1/1";
		ShengDangImageButton[0].SetActionItem(-1);
		ShengDangPreEnchanceLevel.Text = "强化等级:";
		ShengDangCurrentDangCi.Text = "";
		ShengDangNextDangCi.Text = "";
		ShengDangMoney.Text = "";
	}
	
	void ShowShengDangInfo(int index)
	{
		ShengDangItem = sItem[index];
		ShengDangImageButton[0].UpdateItem(ShengDangItem.GetID());

        int j = ShengDangPage[nTheIndex] * 10;
		for(int i = 0; i < 10; i++)
		 {
			if(i+j >= sItem.Count)
			{
                ShengDangNames[i].Text = "";
                ShengDangLevels[i].Text = "";
                ShengDangRadios[i].Hide(true);
			}
			else
			{			
                ShengDangNames[i].Text = sItem[i + j].GetName();
                ShengDangLevels[i].Text = "档次:" + sItem[i + j].GetCurrentDangCi();
                ShengDangRadios[i].Hide(false);
			}
		 }

		int selectIndex = (int)ShengDangSelect[nTheIndex];
		if(ShengDangPage[nTheIndex] == ShengDangPageRadio[nTheIndex])
        {
            ShengDangRadios[selectIndex].Value = true;
        }

        else
            ShengDangRadios[selectIndex].Value = false;

        ShengDangPageNum.Text = (ShengDangPage[nTheIndex] + 1) + "/" + GetPageNum();

        ShengDangPreEnchanceLevel.Text = "强化等级:" + CDataPool.Instance.GetStrengthLevelDesc(ShengDangItem.GetStrengthLevel());
		if(ShengDangItem.GetCurrentDangCi() == -1)
		{
			ShengDangCurrentDangCi.Text = "";
			ShengDangNextDangCi.Text = "档次1";
		}
		else
		{
			ShengDangCurrentDangCi.Text = "档次:" + ShengDangItem.GetCurrentDangCi();
            ShengDangNextDangCi.Text = "档次:" + (ShengDangItem.GetCurrentDangCi()+1);
		}

        Lua_Item_LevelUp item = LifeAbility.Instance.Get_Equip_LevelUpMaterial(ShengDangItem.GetCurrentDangCi());
		CObject_Item pItemObj1 = ObjectSystem.Instance.NewItem((uint)item.needItemID);
		CObject_Item pItemObj2 = ObjectSystem.Instance.NewItem((uint)item.needItemID2);
		
		 CActionItem_Item actionItem1 = CActionSystem.Instance.GetAction_ItemID(pItemObj1.GetID());
         CActionItem_Item actionItem2 = CActionSystem.Instance.GetAction_ItemID(pItemObj2.GetID());
		MaterialName[0].Text = actionItem1.GetName();
		MaterialName[1].Text = actionItem2.GetName();
		MaterialButton[0].SetActionItemByActionId(actionItem1.GetID());
		MaterialButton[1].SetActionItemByActionId(actionItem2.GetID());

        int count1 = CDataPool.Instance.UserBag_CountItemByIDTable(item.needItemID);
        int count2 = CDataPool.Instance.UserBag_CountItemByIDTable(item.needItemID2);

        if (count1 >= item.needItemNum)
            count1 = item.needItemNum;
		else if(count1 == -1)
			count1 = 0;
		
        if (count2 >= item.needItem2Num)
            count2 = item.needItem2Num;
		else if(count2 == -1)
			count2 = 0;
     
		MaterialNum[0].Text = count1 + "/" + item.needItemNum;
		MaterialNum[1].Text = count2 + "/" + item.needItem2Num;
        ShengDangMoney.Text = item.money.ToString();
	}
	
	//得到装备的页数
	int GetPageNum()
	{
		int num;
		num = sItem.Count;
		int page = num/10;
		if(num > page*10 && num <= (page+1)*10)
			page++;
		return page;
	}
	
	void BagRadioClicked()   //背包按钮点击
	{
		ShengDangMode = MODE.TAB_Bag;
		//ShowShengDangInfo(ShengDangIndex[nTheIndex]);
		Update_ShengDangEquip();
        //WhichWindowIsClick(MODE.TAB_Bag);
	}
	
	void RoleRadioClicked()  //角色按钮点击
	{
		ShengDangMode = MODE.TAB_Role;
		//ShowShengDangInfo(ShengDangIndex[nTheIndex]);
		Update_ShengDangEquip();
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
		ShengDangPageRadio[nTheIndex] = ShengDangPage[nTheIndex];
		ShengDangSelect[nTheIndex] = m_tab;
		ShengDangIndex[nTheIndex] = (int)m_tab + ShengDangPage[nTheIndex]*10;
		IsShow(ShengDangIndex[nTheIndex]);
	}
	
	//切换到上一页
	void ChangeToPrePage()
	{
		int PageNum = GetPageNum();
		if(ShengDangPage[nTheIndex] == 0)
		{
			LogManager.LogWarning("已经是第一页了");
		}
		else
		{
			ShengDangPage[nTheIndex] = ShengDangPage[nTheIndex]-1;
			ShowShengDangInfo(ShengDangIndex[nTheIndex]);
		}
	}
	
	//切换到下一页
	void ChangeToNextPage()
	{
		int PageNum = GetPageNum();
		
		if(ShengDangPage[nTheIndex] == PageNum-1)
		{
			LogManager.LogWarning("已经是最后一页了");
		}
		else
		{
			ShengDangPage[nTheIndex]++;
			ShowShengDangInfo(ShengDangIndex[nTheIndex]);
		}
	}
	
	//装备升档
	void EquipUpLevelOnClicked()
    {
        //LifeAbility.Instance.equipOperate(ShengDangItem, "UpLevel");
        LifeAbility.Instance.setOperaterRole(0);
        LifeAbility.Instance.equipOperate(ShengDangItem,EquipUpLevelOp.Name);
		
    }
}
