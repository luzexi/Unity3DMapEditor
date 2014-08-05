using UnityEngine;
using System;
using System.Collections.Generic;

using Interface;

public class UIRelive : MonoBehaviour
{
    void Awake()
    {
        //Hide();

        EZScreenPlacement ScreenPlacement = gameObject.transform.root.gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
            ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_RELIVE_SHOW, ToggleRelive);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_RELIVE_HIDE, ToggleRelive);
    }

    public void ToggleRelive(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_RELIVE_SHOW)
        {
            Show();
        }
        else if (eventId == GAME_EVENT_ID.GE_RELIVE_HIDE)
        {
            Hide();
        }
    }

    void Hide()
    {
        gameObject.transform.root.gameObject.SetActiveRecursively(false);
    }

    void Show()
    {
        UIWindowMng.Instance.ShowWindow("ReliveWindow");
    }

    public void ReliveYuanDi()
    {
        Interface.PlayerMySelf.Instance.AskReliveYuanDi();
    }

    public void ReliveChangJing()
    {
        Interface.PlayerMySelf.Instance.AskReliveChangJing();
    }

    public void ReliveZhuCheng()
    {
        Interface.PlayerMySelf.Instance.AskReliveZhuCheng();
    }
}
