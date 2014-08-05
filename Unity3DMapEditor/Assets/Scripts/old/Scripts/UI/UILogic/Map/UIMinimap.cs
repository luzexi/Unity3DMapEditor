using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMinimap : MonoBehaviour {

    public MapControl mapControl;

    void Awake()
    {
        mapControl.EnableAll();

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_MAP, UpdateMap);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_SCENE_TRANSED, UpdateMap);
    }

    void Start()
    {
        GameObject bg = GameObject.Find("MiniMapBg");
        if (bg != null)
        {
            Destroy(bg.GetComponent<BoxCollider>());
        }

        //mapControl.ChangeMap(WorldManager.Instance.ActiveScene.GetSceneDefine().szSceneMap);
    }

    public SpriteText rolePos;
    public SpriteText lblName;
    public void UpdateMap(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_UPDATE_MAP)
        {
            UpdatePos();
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
    }

    void UpdatePos()
    {
        Vector3 pos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
        rolePos.Text =  (int)pos.x + "," + (int)pos.z;

        mapControl.SetPlayerPos(pos);
    }

    public void ZoomIn()
    {
        int newScale = mapControl.ScaleSize - 1;
        if (newScale > 0)
            mapControl.ScaleSize = newScale;
    }

    public void ZoomOut()
    {
        int newScale = mapControl.ScaleSize + 1;
        if (newScale <= 3)
            mapControl.ScaleSize = newScale;
    }

    UIWindowItem sceneMap = null;
    public void ToggleSceneMap()
    {
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_SCENEMAP);
        if (sceneMap == null)
        {
            sceneMap = UIWindowMng.Instance.GetWindow("SceneMapWindow");
        }
    }
	
   // GameObject campWindow;
    public void OnTeamClicked()
    {
		UIWindowMng.Instance.GetWindow("ActivityWindow");
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_ACTIVITYDETAIL);
        
        //if (campWindow == null)
        //{
        //    campWindow = UIWindowMng.Instance.GetWindowGo("DairyActivityWindow");
        //}
    }
}
