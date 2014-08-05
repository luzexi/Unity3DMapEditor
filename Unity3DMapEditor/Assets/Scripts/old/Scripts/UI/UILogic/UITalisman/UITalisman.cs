using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Network.Packets;
public class UITalisman : MonoBehaviour
{
    public List<ActionButton> inventory_;
    public List<ActionButton> equipts_;
    public UIButton compoundBtn_;
    bool autoCompound = false;
    void Awake()
    {
        //gameObject.SetActiveRecursively(false);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_NEW_TALISMANITEM, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_OPEN_TALISMANITEM, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_TALISMANITEM, OnEvent);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_TALISMANITEM_CHANGED, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_COMPOUND_TALISMANITEM_RESULT, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_TALISMANEQUIPT_CHANGED, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_FABAOWINDOW, OnEvent);

        for (int i = 0; i < inventory_.Count; i++)
        {
            inventory_[i].AddInputDelegate(ClickDelegate);
        }

        for (int i = 0; i < equipts_.Count; i++)
        {
            equipts_[i].AddInputDelegate(ClickDelegate);
        }

        Refresh();
    }
	
    void Close()
    {
        gameObject.SetActiveRecursively(false);
    }

    void EnableButton(bool enabled)
    {
        int unlockCount = CDataPool.Instance.TalismanInventory_UnLockCount();
        for (int i = 0; i < unlockCount; i++)
        {
            if (!enabled)
            {
                inventory_[i].SetDisableColor();
                inventory_[i].Disable();
            }
            else
            {
                inventory_[i].SetEnableColor();
                inventory_[i].Enable();
            }
        }

        unlockCount = CDataPool.Instance.TalismanEquipment_UnLockCount();
        for (int i = 0; i < unlockCount; i++)
        {
            if (!enabled)
            {
                equipts_[i].SetDisableColor();
                equipts_[i].Disable();

            }
            else
            {
                equipts_[i].SetEnableColor();
                equipts_[i].Enable();
            }
        }
    }

    void Compound()
    {
        
        GameObject mbox = UIWindowMng.Instance.GetWindowGo("MessageBoxSelf");
        MessageBoxSelf messagebox = mbox.GetComponent<MessageBoxSelf>();
        if (messagebox != null)
        {
            messagebox.Show("提示", "是否需要合成", StartCompound, null);
        }
       
    }

    void StartCompound()
    {
        EnableButton(false);
        autoCompound = true;
        CompoundSingle();
    }

    void CompoundSingle()
    {
        bool compound = false;
        int unlockCount = CDataPool.Instance.TalismanInventory_UnLockCount();
        CTalisman_Item item1 = null;
        CTalisman_Item item2 = null;
        for (int i = 0; i < unlockCount; i++)
        {
            item1 = CDataPool.Instance.TalismanInventory_GetItem(i);
            int lv = item1.GetLV();
            int quality = item1.GetItemTableQuality();
            item2 = CDataPool.Instance.TalismanInventory_GetSuitableCompoundItem(lv, quality);
            if (item2 != null)
            {
                CGOperateTalisman operateTalisman = new CGOperateTalisman();
                operateTalisman.Type = 2;
                operateTalisman.SrcIndex = (byte)(item1.GetPosIndex() + GAMEDEFINE.MAX_BAG_SIZE);
                operateTalisman.DstIndex = (byte)(item2.GetPosIndex() + GAMEDEFINE.MAX_BAG_SIZE);
                NetManager.GetNetManager().SendPacket(operateTalisman);
                compound = true;
                return;
            }
        }

        if (autoCompound)
        {
            autoCompound = compound;
        }

        if (!autoCompound)
        {
            EnableButton(true);
        }
    }

    void Search()
    {
        
    }

    void RefreshInventory()
    {
        int unlockCount = CDataPool.Instance.TalismanInventory_UnLockCount();
        int lockCount = CTalisman_Inventory.MAX_TALISMAN_ITEM_NUMBER - unlockCount;
        for (int i = 0; i < unlockCount; i++)
        {
            RefreshInventoryByIndex(i);
        }
        for (int i = unlockCount; i < CTalisman_Inventory.MAX_TALISMAN_ITEM_NUMBER; i++)
        {
            inventory_[i].SetIcon("fabaodai-feng");
        }
    }

    void RefreshEquiptmentByIndex(int index)
    {
        if(index < 0 || index >= CDataPool.Instance.TalismanEquipment_UnLockCount())
        {
            LogManager.LogError("Refresh Fabao Equipt index out of range");
            return;
        }
        CTalisman_Item item = CDataPool.Instance.TalismanEquipment_GetItem(index);
        if (item == null)
        {
            equipts_[index].SetActionItem(-1);
        }
        else
        {
            equipts_[index].SetActionItem(item.GetID());
        }
    }

    void RefreshEquiptment()
    {
        int unlockCount = CDataPool.Instance.TalismanEquipment_UnLockCount();
        int lockCount = CTalisman_Equipments.MAX_TALISMAN_EQUIPT_NUMBER - unlockCount;
        for (int i = 0; i < unlockCount; i++)
        {
            RefreshEquiptmentByIndex(i);
        }
        for (int i = unlockCount; i < CTalisman_Equipments.MAX_TALISMAN_EQUIPT_NUMBER; i++)
        {
            equipts_[i].SetIcon("fabaodai-feng");
        }
    }

    void RefreshInventoryByIndex(int index)
    {
        if (index < 0 || index >= CDataPool.Instance.TalismanInventory_UnLockCount())
        {
            LogManager.LogError("Refresh Fabao Inventory index out of range");
            return;
        }
        CTalisman_Item item = CDataPool.Instance.TalismanInventory_GetItem(index);
        if (item == null)
        {
            inventory_[index].SetActionItem(-1);
        }
        else
        {
            inventory_[index].SetActionItem(item.GetID());
        }
    }

    void Refresh()
    {
        RefreshInventory();
        RefreshEquiptment();
    }

    void OnEvent(GAME_EVENT_ID eventId, List<string> vParams)
    {
        switch(eventId)
        {
            case GAME_EVENT_ID.GE_NEW_TALISMANITEM:
                {
                    if (!gameObject.active)
                        return;
                    if (vParams.Count == 0)
                    {
                        RefreshInventory();
                    }
                    else if (vParams.Count == 1)
                    {
                        int bagIndex = int.Parse(vParams[0]);
                        RefreshInventoryByIndex(bagIndex);
                    }
                }
                break;
            case GAME_EVENT_ID.GE_OPEN_TALISMANITEM:
                {
                    gameObject.SetActiveRecursively(true);
                    Refresh();
                }
                break;
            case GAME_EVENT_ID.GE_UPDATE_TALISMANITEM:
                {
                    if (!gameObject.active)
                        return;
                    Refresh();
                }
                break;
            case GAME_EVENT_ID.GE_PACKAGE_TALISMANITEM_CHANGED:
                {
                    if (!gameObject.active)
                        return;
                    if (vParams.Count == 0)
                    {
                        RefreshInventory();
                    }
                    else if (vParams.Count == 1)
                    {
                        int bagIndex = int.Parse(vParams[0]);
                        RefreshInventoryByIndex(bagIndex);
                    }
                }
                break;
            case GAME_EVENT_ID.GE_PACKAGE_TALISMANEQUIPT_CHANGED:
                {
                    if (!gameObject.active)
                        return;
                    if (vParams.Count == 0)
                    {
                        RefreshEquiptment();
                    }
                    else if (vParams.Count == 1)
                    {
                        int Index = int.Parse(vParams[0]);
                        RefreshEquiptmentByIndex(Index);
                    }
                }
                break;
            case GAME_EVENT_ID.GE_COMPOUND_TALISMANITEM_RESULT:
                {
                    if (autoCompound)
                        CompoundSingle();
                }
                break;
            case GAME_EVENT_ID.GE_TOGGLE_FABAOWINDOW:
                {
                    if (!gameObject.active)
                    {
                        UIWindowMng.Instance.ShowWindow("FaBaoWindow");
                        Refresh();
                    }
                    else
                        Close();
                }
                break;
        }
    }

    void ClickDelegate(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                break;
            case POINTER_INFO.INPUT_EVENT.PRESS:
                break;
            case POINTER_INFO.INPUT_EVENT.TAP:
                if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
                {
                	string actionName = ptr.hitInfo.collider.gameObject.name;
					if(actionName.Length > 1)
					{
						char type = actionName[0];
					    int  Index = Convert.ToInt32(actionName.Substring(1, actionName.Length-1)) -1;
						//if((type == 'A'))
						//if(type == 'A')
						//{
						//
						//}
						//else if(type == 'B')
						//{
						
						//}
					}
				//LogManager.Log("ptr.hitInfo.collider.gameObject" + ptr.hitInfo.collider.gameObject.name);
                }
                break;
            default:
                break;
        }
    }

}