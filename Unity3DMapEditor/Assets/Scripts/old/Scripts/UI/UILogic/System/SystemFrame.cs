using UnityEngine;
using System;
using System.Collections.Generic;

using Interface;

public class SystemFrame : MonoBehaviour
{

    public void TargetFrame_Update(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {


    }
    public void OnClickedPetButton()
    {
        if (Interface.Pet.Instance.IsFighting(0))
            Interface.Pet.Instance.Go_Relax(0);
        else
            Interface.Pet.Instance.Go_Fight(0);
    }

    GameObject EquipWindow;
    public void ToggleEquipWindow()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_EQUIPWINDOW);
        if (EquipWindow == null)
        {
            EquipWindow = UIWindowMng.Instance.GetWindowGo("EquipWindow");
        }
    }

    GameObject campWindow;
    public void OnTeamClicked()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_ACTIVITYDETAIL);
        if (campWindow == null)
        {
            campWindow = UIWindowMng.Instance.GetWindowGo("DairyActivityWindow");
        }
    }
    public void OnCharmClicked()//点击符印
    {
        GameObject fuyinWindow =  UIWindowMng.Instance.GetWindowGo("FuYinWindow");
        if(fuyinWindow != null)
        {
            UIWindowMng.Instance.ToggleWindow("FuYinWindow", !fuyinWindow.active);
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_CHARMWINDOW);
        }
       
    }
    GameObject friendWindow;
    public void OnFriendClicked()
    {
        if(friendWindow == null)
            friendWindow = UIWindowMng.Instance.GetWindowGo("FriendWindow");
        else
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_FRIEND_INFO);
        }
    }
    GameObject xinfaWindow;
    public void ToggleXinfaWindow()
    {
        if (xinfaWindow == null)
            xinfaWindow = UIWindowMng.Instance.GetWindowGo("XinfaWindow");
        else
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_XINFASKILL_PAGE);
        
    }

    GameObject faBaoWindow;
    public void ToggleFaBaoWindow()
    {
        if (faBaoWindow == null)
        {
            faBaoWindow = UIWindowMng.Instance.GetWindowGo("FaBaoWindow");
        }
        else
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_FABAOWINDOW);
        }
    }
}
