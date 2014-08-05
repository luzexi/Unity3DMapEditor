using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;

public class StorageFrame : MonoBehaviour {

    public List<ActionButton> mActionItems = new List<ActionButton>();

    int mObjID = -1;
    int mOldEndIndex = -1;
#region MonoBehaviour

    void Awake()
    {
        mOldEndIndex = -1;
        //gameObject.SetActiveRecursively(false);
        RegisterEvent();

        UIWindowMng.Instance.HideWindow("QuestDialog");
        mObjID = CDataPool.Instance.UserBank_GetNpcId();
        Show();
    }

#endregion

    void RegisterEvent()
    {
        GameProcedure.s_pEventSystem.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_BANK, OnEvent);
        GameProcedure.s_pEventSystem.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_BANK, OnEvent);
        GameProcedure.s_pEventSystem.RegisterEventHandle(GAME_EVENT_ID.GE_OBJECT_CARED_EVENT, OnEvent);
    }
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParams)
    {
        if (eventId == GAME_EVENT_ID.GE_TOGLE_BANK)
        {
            mObjID = Interface.Bank.Instance.GetNpcId();
            
            Show();
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_BANK)
        {
            UpdateItem();
        }
        else if (eventId == GAME_EVENT_ID.GE_OBJECT_CARED_EVENT)
        {
            if (mObjID == int.Parse(vParams[0]))
            {
                if (float.Parse(vParams[1]) > 3.0)
                {
                    Close();

                }
            }
        }
    }
    void Show()
    {
        UIWindowMng.Instance.ShowWindow("StoreWindow");
        CObjectManager.Instance.CareObject(mObjID, true, "Bank");
        UpdateItem();
    }
    void Close()
    {
        CObjectManager.Instance.CareObject(mObjID, false, "Bank");
        UIWindowMng.Instance.HideWindow("StoreWindow");
        Interface.Bank.Instance.Close();
    }
    void UpdateItem()
    {
        int nCurMaxGrid = CDataPool.Instance.UserBank_GetBankEndIndex();
        for (int i = 0; i < mActionItems.Count; i++ )
        {
            if (nCurMaxGrid < i)
                break;
            ActionButton action = mActionItems[i];
            if (action != null)
            {
                ActionItem actionItem = Interface.Bank.Instance.EnumItem(i);
                if (actionItem.actionItem != null)
                {

                    if (action.CurrActionItem != null && action.CurrActionItem.GetID() == actionItem.actionItem.GetID())
                        continue;

                    action.SetActionItemByActionId(actionItem.actionItem.GetID());
                }
                else
                    action.SetActionItemByActionId(-1);
            }
        }
    }
    void UpdateLockGrid()
    {
        if (mOldEndIndex == -1)
        {
            mOldEndIndex = CDataPool.Instance.UserBank_GetBankEndIndex();
            for (int i = mOldEndIndex + 1; i < mActionItems.Count; i++)
            {
                mActionItems[i].SetIcon("fabaodai-feng");
                mActionItems[i].DisableDrag();
            }
        }
        else
        {
            int nNewEndIndex = CDataPool.Instance.UserBank_GetBankEndIndex();
            for (int i = mOldEndIndex + 1; i <= nNewEndIndex; i++ )
            {
                if (mActionItems.Count > i)
                {
                    mActionItems[i].SetActionItem(-1);
                    mActionItems[i].EnableDrag();
                }
            }
            mOldEndIndex = nNewEndIndex;
        }
    }
    void Clear()
    {

    }
#region UI Events
    void OnCloseClicked()
    {
        Close();
    }
#endregion
}
