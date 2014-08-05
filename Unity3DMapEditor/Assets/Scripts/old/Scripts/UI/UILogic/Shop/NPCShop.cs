using UnityEngine;
using Interface;
using System.Collections.Generic;

/// <summary>
/// npc shop ui
/// </summary>
public class NPCShop : MonoBehaviour
{
    public List<ActionButton> itemList;
    public List<SpriteText> itemNames;
    public List<SpriteText> itemPriceGold;
    public List<SpriteText> itemPriceSilver;
    public List<SpriteText> itemPriceCopper;

    public SpriteText pageNumText;
    public UIButton buyBtn;
    public UIButton sellBtn;
    public UIButton redeemBtn;
    public UIButton prePageBtn;
    public UIButton nextPageBtn;

    const int MAX_ITEM_PERPAGE = 16;
    enum TabMode
    {
        TAB_BUY, //购买

        TAB_REDEEM, //回购
    }
    TabMode tabMode = TabMode.TAB_BUY;

    byte pageIndex = 1;
    int selectIndex = -1;

    void Awake()
    {
        //gameObject.SetActiveRecursively(false);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_OPEN_BOOTH, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_BOOTH, OnEvent);

        UIWindowMng.Instance.HideWindow("QuestDialog");
        UpdateShop();
    }

    void RestUI()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].BeDestroyed();
            itemNames[i].Text = "";
            itemPriceGold[i].Text = "";
            itemPriceSilver[i].Text = "";
            itemPriceCopper[i].Text = "";
        }
    }
    void UpdateWidgetState(TabMode tab, byte pageindex)
    {
        if (tab == TabMode.TAB_BUY)
        {
            redeemBtn.Hide(true);
            buyBtn.Hide(false);
            sellBtn.Hide(false);
        }
        else if (tab == TabMode.TAB_REDEEM)
        {
            redeemBtn.Hide(false);
            buyBtn.Hide(true);
            sellBtn.Hide(true);
        }
        prePageBtn.controlIsEnabled = false;
        nextPageBtn.controlIsEnabled = false;

        pageNumText.Text = pageIndex.ToString();
    }
    void UpdateItems(TabMode tab, byte pageindex)
    {
        if (tab == TabMode.TAB_BUY)
        {
            int nTotalNum = UIInfterface.Instance.GetActionNum(ActionNameType.boothItem);
            int maxPage = nTotalNum / MAX_ITEM_PERPAGE + 1;
            if (nTotalNum % MAX_ITEM_PERPAGE != 0)
                maxPage++;

            if (pageIndex < 1 || pageIndex > maxPage)
                return;

            int nUnit = Interface.NPCShop.Instance.GetShopType("unit");

            int startIndex = (pageIndex - 1) * MAX_ITEM_PERPAGE;

            for (int i = 0; i < MAX_ITEM_PERPAGE; i++)
            {
                CActionItem action = CActionSystem.Instance.EnumAction(i + startIndex, ActionNameType.boothItem);
                if (action == null) continue;
                if (itemList[i] != null )
                    itemList[i].SetActionItemByActionId(action.GetID());

                uint price = Interface.NPCShop.Instance.EnumItemPrice(i);
                if (nUnit == 1)
                    itemPriceCopper[i].Text = price.ToString();

                itemNames[i].Text = action.GetName();
            }

            if (pageIndex < maxPage)
                nextPageBtn.controlIsEnabled = true;
            if(pageIndex > 1)
                prePageBtn.controlIsEnabled = true;

        }
        else if (tab == TabMode.TAB_REDEEM)
        {
            int ntype = Interface.NPCShop.Instance.GetShopType("callback");
            if (ntype <= 0)
                return;
            int nNum = Interface.NPCShop.Instance.GetCallBackNum();
            int nUnit = Interface.NPCShop.Instance.GetShopType("unit");

            int maxPage = nNum / MAX_ITEM_PERPAGE + 1;
            if (nNum % MAX_ITEM_PERPAGE != 0)
                maxPage++;

            if (pageIndex < 1 || pageIndex > maxPage)
                return;

            int startIndex = (pageIndex - 1) * MAX_ITEM_PERPAGE;
            for (int i = 0; i < MAX_ITEM_PERPAGE; i++ )
            {
                CActionItem action = Interface.NPCShop.Instance.EnumCallBackItem(i + startIndex);
                if (action != null)
                {
                    if (itemList[i] != null)
                        itemList[i].SetActionItemByActionId(action.GetID());

                    uint price = Interface.NPCShop.Instance.EnumItemSoldPrice(i);
                    if (nUnit == 1)
                        itemPriceCopper[i].Text = price.ToString();

                    itemNames[i].Text = action.GetName();
                }
            }
            if (pageIndex < maxPage)
                nextPageBtn.controlIsEnabled = true;
            if (pageIndex > 1)
                prePageBtn.controlIsEnabled = true;
        }
    }
    void UpdateShop()
    {
        RestUI();
        UpdateWidgetState(tabMode, pageIndex);
        UpdateItems(tabMode, pageIndex);
    }
    public void OnEvent(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (gAME_EVENT_ID == GAME_EVENT_ID.GE_OPEN_BOOTH)
        {
            if (!gameObject.active)
            {
                UIWindowMng.Instance.ShowWindow("ShopWindow");
                UpdateShop();
            }
            else
                OnCloseClicked();
        }
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UPDATE_BOOTH)
        {
            UpdateShop();
        }
    }
    
#region Handle UI Event

    public void OnCloseClicked()
    {
        UIWindowMng.Instance.HideWindow("ShopWindow");
    }
    public void OnBuyRadioClicked()
    {
        tabMode = TabMode.TAB_BUY;
        pageIndex = 1;
        UpdateShop();
    }
    public void OnRedeemRadioClicked()
    {
        tabMode = TabMode.TAB_REDEEM;
        pageIndex = 1;
        UpdateShop();
    }
    public void OnPrePageClicked()
    {
        if (pageIndex > 1)
        {
            pageIndex--;
            UpdateShop();
        }
    }
    public void OnNextPageClicked()
    {
        pageIndex++;
        UpdateShop();
    }
    public void OnSellClicked()
    {
        
    }
    public void OnRedeemClicked()
    {
        LogManager.LogWarning("redeem");
    }
    public void OnBuyClicked()
    {
        if (selectIndex >= 0 || selectIndex < MAX_ITEM_PERPAGE)
            itemList[selectIndex].CurrActionItem.DoSubAction();
    }

    public void OnClickedActionBtn1()
    {
        selectIndex = 0;
    }
    public void OnClickedActionBtn2()
    {
        selectIndex = 1;
    }
    public void OnClickedActionBtn3()
    {
        selectIndex = 2;
    }
    public void OnClickedActionBtn4()
    {
        selectIndex = 3;
    }
    public void OnClickedActionBtn5()
    {
        selectIndex = 4;

    }
    public void OnClickedActionBtn6()
    {
        selectIndex = 5;

    }
    public void OnClickedActionBtn7()
    {
        selectIndex = 6;

    }
    public void OnClickedActionBtn8()
    {
        selectIndex = 7;

    }
    public void OnClickedActionBtn9()
    {
        selectIndex = 8;

    }
    public void OnClickedActionBtn10()
    {
        selectIndex = 9;

    }
    public void OnClickedActionBtn11()
    {
        selectIndex = 10;

    }
    public void OnClickedActionBtn12()
    {
        selectIndex = 11;

    }
    public void OnClickedActionBtn13()
    {
        selectIndex = 12;

    }
    public void OnClickedActionBtn14()
    {
        selectIndex = 13;

    }
    public void OnClickedActionBtn15()
    {
        selectIndex = 14;

    }
    public void OnClickedActionBtn16()
    {
        selectIndex = 15;

    }

#endregion
}