using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;

public class MainMenuToolbar : MonoBehaviour {

    // 快捷键处理 [4/27/2012 SUN]
    delegate void KeyDelgate();
    struct HotKey
    {
        public int nID;
        public KeyDelgate keyDelegate;
    }
    List<HotKey> mHotKeyList = new List<HotKey>();
    //按键代码为KeyCode，如是组合键按照"Shift + Ctrl + Alt + key "，方式组合，每个键占3位数字
    //例如：LeftShift + F = 304102；
    Dictionary<int, HotKey> mHotKeyDelegate = new Dictionary<int, HotKey>();

    void Awake()
    {
        RegisterHotKey();
        defaultHotkey();
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_ACCELERATE_KEYSEND, OnEvent);
    }

    void Start()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_FUNC_OPEN, "-1");
    }

    void RegisterHotKey()
    {
        HotKey hotKey = new HotKey();
        hotKey.nID = 0; ;
        hotKey.keyDelegate = KeyDelegate_GuaJI;
        mHotKeyList.Add(hotKey);

        hotKey.nID = 1;
        hotKey.keyDelegate = KeyDelegate_LockNearest;
        mHotKeyList.Add(hotKey);

        hotKey.nID = 2;
        hotKey.keyDelegate = KeyDelegate_OpenSceneMap;
        mHotKeyList.Add(hotKey);

        hotKey.nID = 3;
        hotKey.keyDelegate = KeyDelegate_OpenTalisman;
        mHotKeyList.Add(hotKey);
        
    }
    void defaultHotkey()
    {
        RegisterKeyDelegate((int)KeyCode.K, 0);
        RegisterKeyDelegate((int)KeyCode.Tab, 1);
        RegisterKeyDelegate((int)KeyCode.M, 2);
        RegisterKeyDelegate((int)KeyCode.O, 3);
    }
    void RegisterKeyDelegate(int key, int nHotkeyID)
    {
        RegisterKeyDelegate(0, key, nHotkeyID);
    }
    void RegisterKeyDelegate(int cmdKey, int key, int nHotkeyID)
    {

        int nKey = cmdKey * 1000 + key;
        if (!mHotKeyDelegate.ContainsKey(nKey))
        {
            foreach (HotKey hotKey in mHotKeyList)
            {
                if(hotKey.nID == nHotkeyID)
                    mHotKeyDelegate.Add(nKey, hotKey);
            }
        }
    }
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_ACCELERATE_KEYSEND)
        {
            if(vParam.Count == 0)
                return;
            int key = int.Parse(vParam[0]);
            HotKey keyDelegate ;
            if(mHotKeyDelegate.TryGetValue(key, out keyDelegate))
            {
                if(keyDelegate.keyDelegate != null)
                    keyDelegate.keyDelegate();
            }
        }
    }

//     void RegisToMain(string windowName)
//     {
//         GameObject Window = UIWindowMng.Instance.GetWindowGo(windowName);
//         if (Window != null)
//         {
//             if (windowName == "BagWindow")
//                 packWindow = Window;
// //             else if (windowName == "RoleStudyWindow")
// //                 skillWindow = Window;
//             else if (windowName == "RoleTipWindow")
//                 RoleWindow = Window;
//             else
//                 return;
//         }
//     }
//     public void FirstEnter(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
//     {
// 		EZScreenPlacement ScreenPlacement = gameObject.GetComponent<EZScreenPlacement>();
// 		if(ScreenPlacement != null)
// 			ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
//         if (this.gameObject.transform.position.z >= 1000)
//             this.gameObject.transform.position -= new UnityEngine.Vector3(0, 0, 1000);
//     }

    public GameObject packWindow;
	public void TogglePackWindow()
	{
        if (packWindow == null)
        {
            packWindow = UIWindowMng.Instance.GetWindowGo("BagWindow");
        }

        // 有可能还没有下载，所以需要再次判断 [4/1/2012 Ivan]
        if (packWindow != null)
        {
            UIWindowMng.Instance.ToggleWindow("BagWindow", !packWindow.active);
            // 强制刷新背包 [2/22/2012 Ivan]
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
        }

        //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OPEN_BOOTH);

	}

    UIWindowItem skillWin;
    public void ToggleSkillWindow()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_COMMONSKILL_PAGE);
        if (skillWin == null)
        {
            skillWin = UIWindowMng.Instance.GetWindow("RoleStudyWindow");
        }
    }


    UIWindowItem roleTipWin;
	public void ToggleRoleWindow()
    { 
		CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ROLE_TIPWINDOW);
        if (roleTipWin == null)
        {
            roleTipWin = UIWindowMng.Instance.GetWindow("RoleTipWindow");
        }
	}
    //bool toggleEquipWindow = false;
    //public GameObject EquipWindow;
    // 
    GameObject EquipWindow;
    public void ToggleEquipWindow()
	{
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_EQUIPWINDOW);
        if (EquipWindow == null)
        {
            EquipWindow = UIWindowMng.Instance.GetWindowGo("EquipWindow");
        }
	}

    UIWindowItem questList;
    public void ToggleQuestList()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_MISSION);
        if (questList == null)
        {
            questList = UIWindowMng.Instance.GetWindow("QuestListWindow");
        }
    }

#region keyDelegate
    void KeyDelegate_GuaJI()
    {
        CAI_MySelf aiSelf = (CAI_MySelf)(CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI());
        if (aiSelf.GetMySelfAI() == ENUM_MYSELF_AI.MYSELF_AI_GUAJI)
        {
            GameProcedure.s_pGameInterface.StopAutoHit();
        }
        else
        {
            GameProcedure.s_pGameInterface.StartAutoHit();
        }
    }
    void KeyDelegate_OpenTalisman()
    {
        UIWindowMng.Instance.GetWindow("FaBaoWindow");
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OPEN_TALISMANITEM);
    }
    void KeyDelegate_LockNearest()
    {
        CObjectManager.Instance.LockNearestEnemy();
    }
    void KeyDelegate_OpenSceneMap()
    {
        UIWindowMng.Instance.GetWindow("SceneMapWindow");
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_SCENEMAP);
    }

#endregion
}
