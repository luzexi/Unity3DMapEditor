using UnityEngine;
using System;
using System.Collections.Generic;

public class UIPacket : MonoBehaviour
{
    public List<ActionButton> buttons;

    const int MAX_PERPACKET = 20;

    UIButton ezButton1;

    void Awake()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, PackItemChanged);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MONEY, PackItemChanged);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_RMB, PackItemChanged);

        //gameObject.transform.root.gameObject.SetActiveRecursively(false);
        
    }
    void Start()
    {
        UpdateByTab();
    }

    public void PackItemChanged(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED)
        {
            if (!gameObject.active)
                return;
            RefreshTabs();
            if (vParam.Count == 0)
            {
                UpdateByTab();
            }
            else if (vParam.Count == 1)
            {
                int bagIndex = int.Parse(vParam[0]);
                if (bagIndex == -1)
                    UpdateBagByIndex(bagIndex);
                else
                    UpdateByTab();
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_UNIT_MONEY ||
            eventId == GAME_EVENT_ID.GE_UNIT_RMB)
        {
            UpdateMoney();
        }
    }

    public SpriteText txtMoney;
    public SpriteText txtRmb;
    private void UpdateMoney()
    {
        if (txtMoney != null)
        {
            int money = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money();
            txtMoney.Text = money.ToString();
        }

        if (txtRmb != null)
        {
            int rmb = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_RMB();
            txtRmb.Text = rmb.ToString();
        }
    }

    void UpdateBagByIndex(int index)
    {
        int nIndex = index - currTab * MAX_PERPACKET;
        CObject_Item packItem = CDataPool.Instance.UserBag_GetItemByIndex(index);
        if (nIndex < 0 || nIndex >= buttons.Count)
        {
            LogManager.LogError("packet index is invalid index=" + nIndex);
            return;
        }
        if (packItem != null)
        {
            buttons[nIndex].SetActionItem(packItem.GetID());
            if (packItem.isLocked)
                buttons[nIndex].Disable();
            else
                buttons[nIndex].Enable();
        }
        else
            buttons[nIndex].SetActionItem(-1);
    }

    public void CloseWindow()
    {
        UIWindowMng.Instance.HideWindow("BagWindow");
    }

    public GameObject[] panels;
    int currTab = 0;
    public void ChangeTab(int tab)
    {
        currTab = tab;
		RefreshTabs();
        UpdateByTab();
    }

    public void ChangeTabEquip()
    {
        ChangeTab(0);
    }
    public void ChangeTabMaterial()
    {
        ChangeTab(1);
    }
    public void ChangeTabTask()
    {
        ChangeTab(2);
    }
    public void ChangeTabRider()
    {
        ChangeTab(3);
    }

    void RefreshTabs()
    {
        //for (int j = 0; j < 3; j++)
        //{
        //    if (currTab == j)
        //        panels[j].transform.localPosition = new Vector3(0, 0, 0);
        //    else
        //        panels[j].transform.localPosition = new Vector3(0, 0, 1000);
        //}
    }

    public void UpdateByTab()
    {
        int i = currTab * MAX_PERPACKET;
        int nMax = i + MAX_PERPACKET;
        for (; i <nMax; i++)
        {
            UpdateBagByIndex((int)i);
        }

        UpdateMoney();
    }
}