using System;
using System.Collections.Generic;
using UnityEngine;

public class UISystem
{
    // 更改为单例模式
    static readonly UISystem sInstance = new UISystem();
    static public UISystem Instance { get { return sInstance; } }

    readonly Camera uiCamrea;
    public Camera UiCamrea
    {
        get { return uiCamrea; }
    }

    UIManager uiManager;
    private UISystem()
    {
        foreach (Camera camera in Camera.allCameras)
        {
            if (camera.name == "UICamera")
            {
                uiCamrea = camera;
                break;
            }
        }

        GameObject goUiMng = GameObject.Find("UIManager");
        if (goUiMng != null)
        {
            uiManager = goUiMng.GetComponent<UIManager>();
            uiManager.AddMouseTouchPtrListener(InputDelegate);
        }
    }

    UICreatureBoardSystem creatureBoardSystem;
    UIWindowMng uiWindowMng;
    public void Initial()
    {
        //初始化角色信息板管理器
        creatureBoardSystem = UICreatureBoardSystem.Instance;
        creatureBoardSystem.Initial();

        //----------------------------------------------------------------------
        //初始化布局管理器
        uiWindowMng = UIWindowMng.Instance;
        uiWindowMng.Init();

        //----------------------------------------------------------------------
        //文本管理器
        UIString.Instance.Init();
    }

    public void Release()
    {
        if (creatureBoardSystem != null)
            creatureBoardSystem.Release();
    }

    public bool IsWindowHollow { get; set; }

    public bool IsMouseHover()
    {
        if (uiManager != null)
        {
            if (CurrHyperlink != null)
                return true;

            if (uiManager.DidAnyPointerHitUI()
                && !IsWindowHollow)
            {
                return true;
            }
        }
        return false;
    }

    public CreatureBoard CreateCreatureBoard()
    {
        return creatureBoardSystem.CreateCreatureBoard();
    }

    internal void AddNewBeHitBoard(bool bDouble, string dmgText, float x, float y, ENUM_DAMAGE_TYPE dmgType, ENUM_DMG_MOVE_TYPE moveType)
    {
        BeHitBoardManager.Instance.AddNewBeHit(bDouble, dmgText, x, y, dmgType, moveType);
    }

    public void Tick()
    {
        BeHitBoardManager.Instance.Update();
        UIBufferManager.Instance.Tick();
    }

    ActionButton actionStarter = null;
    public bool IsDragging()
    {
        return actionStarter != null;
    }
    internal void OnDragBeging(ActionButton actionButton)
    {
        if (actionButton == null)
            return;
        CInputSystem.Instance.InputCapture = InputCapture.ICS_UI;
        actionStarter = actionButton;
        Texture icon = actionButton.GetIcon();
        CursorMng.Instance.EnterUICursor(icon);
    }

    internal void OnDragEnd(ActionButton btnTar)
    {
        CInputSystem.Instance.InputCapture = InputCapture.ICS_NONE;
        CursorMng.Instance.LeaveUICursor();
        CursorMng.Instance.SetCursor(ENUM_CURSOR_TYPE.CURSOR_NORMAL);

        if (IsDragging())
        {
            // 跳过拖拽到自己身上 [4/12/2012 Ivan]
            if (btnTar != actionStarter)
            {
                bool beDestroy = btnTar == null;

                if (actionStarter.CurrActionItem != null)
                    actionStarter.CurrActionItem.NotifyDragDropDragged(beDestroy,
                        !beDestroy ? btnTar.Name() : "",
                        actionStarter.Name());
            }
        }

        actionStarter = null;
    }

    internal bool IsWindowShow(string winName)
    {
        return uiWindowMng.IsWindowShow(winName);
    }

    internal void InjectInput()
    {
        //if (CInputSystem.Instance.IsLeftMouseDown())
        //{
        //    Ray ray = uiCamrea.ScreenPointToRay(CInputSystem.Instance.GetMousePos());
        //    RaycastHit hitInfo;
        //    bool hit = Physics.Raycast(ray, out hitInfo, LayerManager.UIMask);
        //    if (hit)
        //    {
        //        GameObject hitGo = hitInfo.collider.gameObject.transform.root.gameObject;
        //        UIWindowMng.Instance.BringWindowForward(hitGo.name);

        //        // 检测是否有超链接点击到
        //        CheckHyperClick(hitInfo);
        //    }
        //}
    }

    public HyperItemBase CurrHyperlink { get; set; }

    public void InputDelegate(POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
                break;
            //case POINTER_INFO.INPUT_EVENT.DRAG:
            case POINTER_INFO.INPUT_EVENT.PRESS:
            case POINTER_INFO.INPUT_EVENT.TAP:
                {
                    if (ptr.hitInfo.collider != null)
                    {
                        GameObject hitGo = ptr.hitInfo.collider.gameObject.transform.root.gameObject;
                        UIWindowMng.Instance.BringWindowForward(hitGo.name);
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                { 
                    // 检测是否有超链接点击到
                    CurrHyperlink = CheckHyperHover(ptr.hitInfo);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                break;
            default:
                break;
        }
    }

    // 检测是否有超链接在
    HyperItemBase CheckHyperHover(RaycastHit hitInfo)
    {
        if (hitInfo.collider == null)
            return null;

        // 防止移动到容器上误判为超链接之上
        UIScrollList list = hitInfo.collider.gameObject.GetComponent<UIScrollList>();
        if (list != null)
            return null;

        SpriteText currTextItem = hitInfo.collider.gameObject.GetComponentInChildren<SpriteText>();
        if (currTextItem != null)
        {
            //string showText = currTextItem.Text.Replace("\0", "");
            int charIndex = currTextItem.GetNearestInsertionPoint(hitInfo.point);

            return HyperLinkManager.Instance.GetHyperHover(currTextItem.DisplayString, charIndex);
        }
        return null;
    }

    internal void AddHollowWindow(GameObject window)
    {
        if (window == null)
            return;
        UIScrollList list = window.GetComponentInChildren<UIScrollList>();
        if (list != null)
            list.AddInputDelegate(HollowWindowInput);

        AutoSpriteControlBase baseControl = window.GetComponentInChildren<AutoSpriteControlBase>();
        if (baseControl != null)
            baseControl.AddInputDelegate(HollowWindowInput);
    }

    public void HollowWindowInput(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                break;
            case POINTER_INFO.INPUT_EVENT.PRESS:
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
                break;
            case POINTER_INFO.INPUT_EVENT.TAP:
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                IsWindowHollow = true;
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                IsWindowHollow = false;
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                break;
            case POINTER_INFO.INPUT_EVENT.DRAG:
                break;
            default:
                break;
        }
    }
}
