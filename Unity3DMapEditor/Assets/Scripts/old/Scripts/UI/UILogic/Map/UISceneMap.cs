using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class UISceneMap : MonoBehaviour
{

    public MapControl mapControl;

    void Start()
    {
        InitUi();

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_SCENEMAP, UpdateMap);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_SCENE_TRANSED, UpdateMap);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_MAP, UpdateMap);

        // 初始化刷新地图 [4/10/2012 Ivan]
        mapControl.ChangeMap(WorldManager.Instance.ActiveScene.GetSceneDefine().szSceneMap);
        lblName.Text = WorldManager.Instance.ActiveScene.GetSceneDefine().szName;
        ToggleWorldMap();
    }

    private void InitUi()
    {
        mapControl.InitialControls();
        mapControl.EnableAll();
        mapControl.AlwaysInCenter = false;

        EZScreenPlacement ScreenPlacement = gameObject.transform.root.gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
            ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);

        //gameObject.transform.root.gameObject.SetActiveRecursively(false);


        inputX.SetValidationDelegate(MyValidator);
        inputY.SetValidationDelegate(MyValidator);
    }

    string MyValidator(UITextField field, string text,ref int pos)
    {
        // Remove non-numeric characters:
        return Regex.Replace(text, "[^0-9]", "");
    }

    public SpriteText lblName;
    public void UpdateMap(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_TOGLE_SCENEMAP)
        {
            ToggleWindow();
        }
        else if (eventId == GAME_EVENT_ID.GE_SCENE_TRANSED)
        {
            if (vParam.Count > 0)
            {
                string mapName = vParam[0];
                mapControl.ChangeMap(mapName);

                lblName.Text = WorldManager.Instance.ActiveScene.GetSceneDefine().szName;
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_MAP)
        {
            if (mapControl.gameObject.active)
            {
                UpdatePos();
            }
        }
    }

    void UpdatePos()
    {
        Vector3 pos = CObjectManager.Instance.getPlayerMySelf().GetPosition();

        mapControl.SetPlayerPos(pos);
    }

    string windowName = "SceneMapWindow";
    void ToggleWindow()
    {
        if (gameObject.active)
        {
            CloseWindow();
        }
        else
        {
            UIWindowMng.Instance.ShowWindow(windowName);
            mapControl.ClearOldWindows();

            showWorldmap = !showWorldmap;
            ToggleWorldMap();
        }
    }

    public UITextField inputX;
    public UITextField inputY;
    public void AutoNav()
    {
        int x = int.Parse(inputX.text);
        int y = int.Parse(inputY.text);

        Interface.GameInterface.Instance.Player_MoveTo(new Vector3(x, 0, y));
    }

    public UIButton worldMapBtn;
    public UIButton worldMap;
    bool showWorldmap = true;
    public void ToggleWorldMap()
    {
        showWorldmap = !showWorldmap;
        if (showWorldmap)
        {
            worldMapBtn.Text = "场景地图";
            worldMap.gameObject.SetActiveRecursively(true);
            mapControl.gameObject.SetActiveRecursively(false);
        }
        else
        {
            worldMapBtn.Text = "世界地图";
            worldMap.gameObject.SetActiveRecursively(false);
            mapControl.gameObject.SetActiveRecursively(true);
        }
    }

    bool showFull = false;
    public void ToggleMapSize()
    {
        showFull = !showFull;
        mapControl.ShowFullMap = showFull;
    }

    public void CloseWindow()
    {
        UIWindowMng.Instance.HideWindow(windowName);
    }
}
