using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILogin : MonoBehaviour
{
    public Camera mainCamera;

    public UITextField loginName;
    public UITextField loginIp;

    MainBehaviour main;
    void Awake()
    {
        main = GameObject.Find("MainLoop").GetComponent<MainBehaviour>();//mainCamera.GetComponent<MainBehaviour>();
        loginName.Text = main.defaultAccount;
        loginIp.Text = main.defaultServerIP;

        gameObject.SetActiveRecursively(false);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UI_INFOS, HandelUIInfos);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_GAMELOGIN_OPEN_SELECT_CHARACTOR, HandelUIInfos);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (loginName.Text.Length<=0)
            {
                SetNameFocus();
            }
            else
            {
                Login();
            }
        }
    }

    public void HandelUIInfos(GAME_EVENT_ID eventID, List<string> vParam)
    {
        if (eventID == GAME_EVENT_ID.GE_UI_INFOS)
        {
            if (vParam.Count != 0)
            {
                string info = vParam[0];
                if (info == "LoginMapDownload")
                {
                    gameObject.SetActiveRecursively(true);
                    SetNameFocus();
                }
            }
        }
        else if (eventID == GAME_EVENT_ID.GE_GAMELOGIN_OPEN_SELECT_CHARACTOR)
        {
            gameObject.SetActiveRecursively(false);
        }
    }

    void SetNameFocus()
    {
        UIManager.instance.FocusObject = loginName;
    }

    public void Login()
    {
        main.defaultAccount = loginName.Text;
        main.defaultServerIP = loginIp.Text;

        main.OnLogin();
    }
}