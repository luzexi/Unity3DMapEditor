/********************************************************************
	created:	2012/03/21
	created:	21:3:2012   19:32
	filename: 	E:\U3D_Proj\SG\Assets\Scripts\UI\UIWindow\MapControl.cs
	file path:	E:\U3D_Proj\SG\Assets\Scripts\UI\UIWindow
	file base:	MapControl
	file ext:	cs
	author:		Ivan
	
	purpose:	注意，如果要居中显示的话，
                ui布局的时候原点必须在中心，否则原点在左上角
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapControl : Window
{
    public GameObject iconPrefab;

    public override void Initial()
    {
        UIButton render = gameObject.GetComponent<UIButton>();
        if (render != null)
            render.AddInputDelegate(InputDelegate);
    }

    void InputDelegate(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.TAP:
                MouseClick();
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                DragMove();
                break;
            default:
                break;
        }
    }

    private void DragMove()
    {
        if (CInputSystem.Instance.isMouseRightHold())
        {
            float mouseHorMoveX = CInputSystem.Instance.GetAxis("Mouse X");
            if (Mathf.Abs(mouseHorMoveX) > 0.01)
            {
                float x = uiLeftTopPos.x - mouseHorMoveX*10;
                if (x >= 0 && x+ResShowWidth <= uiWidth)
                {
                    uiLeftTopPos.x = x;
                }
            }
            float mouseHorMoveY = CInputSystem.Instance.GetAxis("Mouse Y");
            if (Mathf.Abs(mouseHorMoveY) > 0.01)
            {
                float y = uiLeftTopPos.y + mouseHorMoveY * 10;
                if (y >= 0 && y+ResShowHeight <= uiHeight)
                {
                    uiLeftTopPos.y = y;
                }
            }
        }
    }

    void MouseClick()
    {
        Vector2 mousePos = CInputSystem.Instance.GetMouseUIPos();
        if (IsPointIn(mousePos))
        {
            Vector2 distance = Vector2.zero;
            Vector2 controlPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            distance = mousePos - controlPos;
            distance.x /= xUi2ScreenPer;
            distance.y /= yUi2ScreenPer;

            if (AlwaysInCenter)
            {
                // 如果用于处于地图中心的话，需要改变计算方式 [3/22/2012 Ivan]
                distance /= WorldScale2Pixel;
                Vector3 myPos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
                distance.x += myPos.x;
                distance.y += myPos.z;
            }
            else
            {
                distance.x += uiLeftTopPos.x;
                distance.y -= uiLeftTopPos.y;

                distance /= WorldScale2Pixel;

                distance.y += terrainHeight;
            }

            Interface.GameInterface.Instance.Player_MoveTo(new Vector3(distance.x, 0, distance.y));
        }
    }

    bool showFullMap = false;
    public bool ShowFullMap
    {
        get { return showFullMap; }
        set { 
            showFullMap = value;
            uiLeftTopPos = Vector2.zero;
            CalcPosPercent();
        }
    }
    // 标注可以显示目标上多少个像素的纹理区域 [3/20/2012 Ivan]
    public int showWidth;
    public int ResShowWidth
    {
        get {
            if(!showFullMap)
                return showWidth * scaleSize;
            else
                return uiWidth; 
        }
    }
    public int showHeight;
    public int ResShowHeight
    {
        get {
            if (!showFullMap)
                return showHeight * scaleSize;
            else
                return uiHeight; 
        }
    }
    public void SetPlayerPos(Vector3 posInWorld)
    {
        Rect rect = GetRectByPos(posInWorld);
        if (ezButton != null)
            ezButton.SetUVsFromPixelCoords(rect);

        // 更新坐标后需要刷新flag [3/20/2012 Ivan]
        UpdateFlags();
    }

    // 这个缩放只用来控制显示多少个像素的纹理区域 [3/20/2012 Ivan]
    int scaleSize = 1;
    public int ScaleSize
    {
        get { return scaleSize; }
        set 
        { 
            scaleSize = value;
            CalcPosPercent();
        }
    }

    bool alwaysInCenter = true;
    public bool AlwaysInCenter
    {
        get { return alwaysInCenter; }
        set { alwaysInCenter = value; }
    }

    Vector2 uiLeftTopPos = Vector2.zero;
    // 根据玩家在游戏中的坐标，计算出要显示的地图区域 [3/20/2012 Ivan]
    Rect GetRectByPos(Vector3 posInWorld)
    {
        if (AlwaysInCenter)
        {
            // 玩家在地图中心位置 [3/21/2012 Ivan]
            float wPercent = posInWorld.x / terrainWidth;
            float hPercent = 1 - posInWorld.z / terrainHeight;

            float mapX = wPercent * uiWidth;
            float mapY = hPercent * uiHeight;

            mapX -= ResShowWidth / 2;
            mapY -= ResShowHeight / 2;

            return new Rect(mapX, mapY, ResShowWidth, ResShowHeight);
        }
        else
        {
            // 显示区域在变化 [3/21/2012 Ivan]
            return new Rect(uiLeftTopPos.x, uiLeftTopPos.y, ResShowWidth, ResShowHeight);
        }
    }

    int terrainWidth = 0;
    int terrainHeight = 0;
    // 要显示的地图图片的大小 [3/21/2012 Ivan]
    int uiWidth = 0;
    int uiHeight = 0;
    internal void ChangeMap(string mapName)
    {
        // 暂时把地图保存到icon里面 [3/20/2012 Ivan]
        SetIcon(mapName);

        uiWidth = GetIcon().width;
        uiHeight = GetIcon().height;

        terrainWidth = WorldManager.Instance.ActiveScene.GetSceneDefine().nXSize;
        terrainHeight = WorldManager.Instance.ActiveScene.GetSceneDefine().nZSize;

        CalcPosPercent();

        Clear();
    }


    float xUi2ScreenPer;
    float yUi2ScreenPer;
    /// <summary>
    /// 计算出纹理坐标转换到屏幕坐标时候的百分比
    /// </summary>
    private void CalcPosPercent()
    {
        Bounds bound = gameObject.renderer.bounds;
        xUi2ScreenPer = (bound.max.x - bound.min.x) / ResShowWidth;
        yUi2ScreenPer = (bound.max.y - bound.min.y) / ResShowHeight;
    }

    public bool ShowNpc { get; set; }
    public bool ShowPlayer { get; set; }
    public bool ShowMonster { get; set; }

    public void EnableAll()
    {
        ShowNpc = true;
        ShowPlayer = true;
        ShowMonster = true;
    }

    /// <summary>
    /// 刷新所有的标记,eg:npc,player.
    /// </summary>
    public void UpdateFlags()
    {
        ClearOldWindows();

        UpdateMyself();
        UpdatePlayer();
        //UpdateNpc();
        UpdateMonster();
        //UpdateMission();
    }

    public void ClearOldWindows()
    {
        foreach (UIButton win in unUsedWindows)
        {
            if (win != null)
            {
                win.gameObject.SetActiveRecursively(false);
            }
        }
        foreach (UIButton win in usedWindows)
        {
            if (win != null)
            {
                win.gameObject.SetActiveRecursively(false);
                unUsedWindows.Add(win);
            }
        }
        usedWindows.Clear();

        if (xUi2ScreenPer == 0)
            CalcPosPercent();
    }

    void Clear()
    {
        usedWindows.Clear();
        unUsedWindows.Clear();
    }

    const string npcIconName = "npcB";
    const string meIconName = "me";
    const string enemyIconName = "monsterB";
    const string playerIconName = "player";
    const string missAcceptIconName = "gantanhao-huang";
    const string missContinueIconName = "wenhao-hui";
    const string missFinishIconName = "wenhao-huang";

    void UpdateMission()
    {
        List<int> npcIds = new List<int>();
        foreach (_DBC_MISSION_DEMAND_LIST miss in MissionList.Instance.SceneMissions)
        {
            if (!npcIds.Contains(miss.n_AcceptNpcID))
                npcIds.Add(miss.n_AcceptNpcID);
            if (!npcIds.Contains(miss.n_FinishNpcID))
                npcIds.Add(miss.n_FinishNpcID);
        }
        
        int currentScene = GameProcedure.s_pWorldManager.GetActiveSceneID();
        foreach (int id in npcIds)
        {
            _DBC_MISSION_DEMAND_LIST currMiss;
            NpcMissState state = MissionList.Instance.GetNpcMissState(id,out currMiss);
            if (state == NpcMissState.None || currMiss == null)
                continue;

            if ((state == NpcMissState.AcceptNormal && currMiss.n_AcceptSceneId != currentScene) ||
                ((state == NpcMissState.ContinueNormal || state == NpcMissState.FinishNormal) && currMiss.n_FinishSceneId != currentScene))
            {
                continue;
            }

            _MISSION_INFO info = MissionStruct.Instance.ParseMissionInfo(currMiss.n_MissionID, currMiss.n_ScriptID);
            
            MAP_POS_DEFINE item = new MAP_POS_DEFINE();
            item.nServerID = currMiss.n_MissionID + (int)state * 100;
            string iconName = "";
            string tip = "";
            if (state == NpcMissState.AcceptNormal)
            {
                iconName = missAcceptIconName;
                tip = "可接任务：" + info.m_misName;
                item.pos = new Vector2(currMiss.n_AcceptPosX, currMiss.n_AcceptPosZ);
            }
            else if (state == NpcMissState.ContinueNormal)
            {
                iconName = missContinueIconName;
                tip = "已接任务：" + info.m_misName;
                item.pos = new Vector2(currMiss.n_FinishPosX, currMiss.n_FinishPosZ);
            }
            else if (state == NpcMissState.FinishNormal)
            {
                iconName = missFinishIconName;
                tip = "可交任务：" + info.m_misName;
                item.pos = new Vector2(currMiss.n_FinishPosX, currMiss.n_FinishPosZ);
            }
            bool alreadyExist;
            UIButton newWin = AddNewWin(item, iconName, out alreadyExist);
            if (newWin == null || alreadyExist)
                continue;

            newWin.data = UIString.Instance.ParserString_Runtime(tip);
        }
    }
    void UpdateNpc()
    {
        if (ShowNpc)
        {
            foreach (MAP_POS_DEFINE item in WorldManager.Instance.ExpNPClistObj)
            {
                bool alreadyExist;
                UIButton newWin = AddNewWin(item, npcIconName, out alreadyExist);
                if (newWin == null || alreadyExist)
                    continue;

                HyperNpc hyper = new HyperNpc();
                hyper.Name = item.name;
                hyper.NpcId = item.nServerID;
                hyper.SceneId = WorldManager.Instance.GetActiveSceneID();
                hyper.PosTarget = new Vector3(item.pos.x, 0, item.pos.y);

                newWin.data = hyper;
            }
        }
    }
    void UpdateMonster()
    {
        if (ShowMonster)
        {
            foreach (MAP_POS_DEFINE item in WorldManager.Instance.AnimylistObj)
            {
                bool alreadyExist;
                UIButton newWin = AddNewWin(item, enemyIconName, out alreadyExist);
                if (newWin == null || alreadyExist)
                    continue;

                HyperAttack hyper = new HyperAttack();
                hyper.Name = item.name;
                hyper.TargetId = item.nServerID;
                hyper.SceneId = WorldManager.Instance.GetActiveSceneID();
                hyper.PosTarget = new Vector3(item.pos.x, 0, item.pos.y);

                newWin.data = hyper;
            }
        }
    }
    void UpdatePlayer()
    {
        if (ShowPlayer)
        {
            foreach (MAP_POS_DEFINE item in WorldManager.Instance.PlayerlistObj)
            {
                bool alreadyExist;
                UIButton newWin = AddNewWin(item, playerIconName, out alreadyExist);
                if (newWin == null || alreadyExist)
                    continue;
                newWin.data = item.name;
            }
        }
    }

    Vector3 myScreenPos = Vector3.zero;
    private void UpdateMyself()
    {
        bool alreadyExist;
        UIButton win = NextFreeWin(CObjectManager.Instance.getPlayerMySelf().ServerID, out alreadyExist);

        if (!alreadyExist || win.PixelSize == Vector2.zero)
        {
            Texture2D icon = IconManager.Instance.GetIcon(meIconName);
            win.SetTexture(icon);
            win.PixelSize = new Vector2(icon.width, icon.height);
        }

        if (AlwaysInCenter)
        {
            myScreenPos = new Vector3(0, 0, -0.11f);
        }
        else
        {
            // 如果主角没有恒定在原点的话，需要重新计算偏移的坐标 [3/21/2012 Ivan]
            Vector3 mePos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
            Vector3 lefTop = new Vector3(0, 0, terrainHeight);
            Vector3 distance = mePos - lefTop;
            // 转换成纹理坐标 [3/21/2012 Ivan]
            distance *= WorldScale2Pixel;
            // 加入偏移 [3/21/2012 Ivan]
            distance.x -= uiLeftTopPos.x;
            distance.z += uiLeftTopPos.y;

            myScreenPos = new Vector3(distance.x * xUi2ScreenPer,distance.z * yUi2ScreenPer,-0.11f);
        }
        win.gameObject.transform.localPosition = myScreenPos;
        if (!IsPointIn(win.gameObject.transform.position))
            return;
        win.gameObject.SetActiveRecursively(true);
    }

    UIButton AddNewWin(MAP_POS_DEFINE item, string iconName, out bool alreadyExist)
    {
        UIButton win = NextFreeWin(item.nServerID, out alreadyExist);
		if(win.PixelSize == Vector2.zero || win.renderer.material.mainTexture.name != iconName)
			alreadyExist = false;
		
        if (!alreadyExist )
        {
            Texture2D icon = IconManager.Instance.GetIcon(iconName);
            win.SetTexture(icon);
            win.PixelSize = new Vector2(icon.width, icon.height);
        }

        Vector3 newPos = WorldPos2UiPos(item.pos);
        win.gameObject.transform.localPosition = newPos;
        if (!IsPointIn(win.gameObject.transform.position))
            return null;
        win.gameObject.SetActiveRecursively(true);

        return win;
    }

    bool IsPointIn(Vector3 pos)
    {
        Bounds bound = gameObject.renderer.bounds;
        // 忽略深度值 [3/20/2012 Ivan]
        pos.z = bound.center.z;
        return bound.Contains(pos);
    }

    // 标注一个单位的场景坐标对应几个单位的像素 [3/20/2012 Ivan]
    const int WorldScale2Pixel = 4;
    Vector3 WorldPos2UiPos(Vector2 worldPos)
    {
        Vector3 otherPos = new Vector3(worldPos.x, 0, worldPos.y);
        Vector3 mePos = CObjectManager.Instance.getPlayerMySelf().GetPosition();
        // 算出真实世界中，玩家和主角间的距离 [3/20/2012 Ivan]
        Vector3 distance = otherPos - mePos;
        distance *= WorldScale2Pixel;

        Vector3 newPos;
        newPos = new Vector3(distance.x * xUi2ScreenPer + myScreenPos.x, 
                            distance.z * yUi2ScreenPer + myScreenPos.y,
                            - 0.1f);

        return newPos;
    }

    List<UIButton> usedWindows = new List<UIButton>();
    List<UIButton> unUsedWindows = new List<UIButton>();

    const int maxPoolNum = 30;
    UIButton NextFreeWin(int id,out bool alreadyExist)
    {
        alreadyExist = false;

        UIButton freeWin = null;
        if (unUsedWindows.Count != 0)
        {
            foreach (UIButton btn in unUsedWindows)
            {
                if (btn.name == "mapIcon" + id)
                {
                    freeWin = btn;
                    unUsedWindows.Remove(freeWin);
                    alreadyExist = true ;
                    break;
                }
            }
            if (freeWin == null && unUsedWindows.Count >= maxPoolNum)
            {
                freeWin = unUsedWindows[0];
                freeWin.data = null;
                freeWin.name = "mapIcon" + id;
                unUsedWindows.RemoveAt(0);
            }
        }
        
        if(freeWin == null)
        {
            GameObject win = UnityEngine.Object.Instantiate(iconPrefab,
                Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
            win.SetActiveRecursively(false);

            win.name = "mapIcon" + id;
            win.layer = LayerManager.UILayer;
            win.transform.parent = gameObject.transform;

            freeWin = win.GetComponent<UIButton>();
            freeWin.AddInputDelegate(ItemDragDelegate);
        }

        usedWindows.Add(freeWin);
        freeWin.gameObject.SetActiveRecursively(false);
        return freeWin;
    }

    void ItemDragDelegate(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.TAP:
                {
                    ItemClick(ptr.hitInfo.collider.gameObject);
                    HideTooltip();
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                {
                    ShowTooltip(ptr.hitInfo.collider.gameObject);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.DRAG:
                {
                    HideTooltip();
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                {
                    if (ptr.hitInfo.collider == null|| ptr.hitInfo.collider.gameObject.name != lastWinName)
                        HideTooltip();
                }
                break;
            default:
                break;
        }
    }

    private void ItemClick(GameObject go)
    {
        UIButton btn = go.GetComponent<UIButton>();
        if (btn != null)
        {
            if (btn.data is HyperNpc)
            {
                HyperNpc npc = (HyperNpc)btn.data;
                npc.Click();
            }
            else if (btn.data is HyperAttack)
            {
                HyperAttack attack = (HyperAttack)btn.data;
                attack.Click();
            }
        }
    }

    string lastWinName = "";
    void ShowTooltip(GameObject go)
    {
        UIButton btn = go.GetComponent<UIButton>();
        if (btn != null)
        {
            if (btn.data != null)
            {
                string text = "";
                if (btn.data is string)
                {
                    text = (string)btn.data;
                }
                else if (btn.data is HyperNpc)
                {
                    HyperNpc npc = (HyperNpc)btn.data;
                    text = npc.Name;
                }
                else if (btn.data is HyperAttack)
                {
                    HyperAttack attack = (HyperAttack)btn.data;
                    text = attack.Name;
                }

                if (text != "")
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, text);
                    lastWinName = go.name;
                }
            }
            
        }
    }

    private void HideTooltip()
    {
        lastWinName = "";
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, "0");
    }
}
