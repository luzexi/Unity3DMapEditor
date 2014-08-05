/****************************************\
*										*
* 			   输入管理器				*
*										*
\****************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using GFX;

//-----------------------------------------------------
//输入捕获状态
public enum InputCapture
{
    ICS_NONE = 0,	//没有被捕获
    ICS_UI,		    //被UI捕获
    ICS_GAME,	    //被GAME捕获
};

public class CInputSystem
{
    private static readonly CInputSystem instance = new CInputSystem();

    // 禁止在外面创建实例 [1/30/2012 Ivan]
    private CInputSystem() { }

    public static CInputSystem Instance
    {
        get
        {
            return instance;
        }
    }

    public bool Tick()
    {
        // 	    //键盘
        // 	    KeybordTick();
        // 
        // 	    //鼠标，主要处理长时间按键的事件
        // 	    MouseOISTick();

        ExcuteInputEvent();
        return true;
    }

    /************************************************************/
    //捕获状态
    /************************************************************/
    //输入捕获
    InputCapture m_InputCapture;
    public InputCapture InputCapture
    {
        get { return m_InputCapture; }
        set { m_InputCapture = value; }
    }
    public void ExcuteInputEvent()
    {
        // 屏蔽输入 [8/4/2011 Sun]
        if (!IsAcceptInput())
        {
            //ClearInputQueue();
            return;
        }

        //调用相应输入处理,
        switch (InputCapture)
        {
            case InputCapture.ICS_NONE:
                {
                    GameProcedure.ProcessActiveInput();
                    UISystem.Instance.InjectInput();
                }
                break;

            case InputCapture.ICS_UI:
                {
                    UISystem.Instance.InjectInput();
                }
                break;

            case InputCapture.ICS_GAME:
                {
                    GameProcedure.ProcessActiveInput();
                }
                break;
        }
    }

    public bool IsKeyDown(KeyCode code)
    {
        // 处于输入状态不激活事件 [3/27/2012 Ivan]
        if (UIManager.instance.FocusObject == null)
        {
            return Input.GetKeyDown(code);
        }
        return false;
    }

    public bool IsKeyUp(KeyCode code)
    {
        // 处于输入状态不激活事件 [3/27/2012 Ivan]
        if (UIManager.instance.FocusObject == null)
        {
            return Input.GetKeyUp(code);
        }
        return false;
    }

    public bool IsLeftMouseClick()
    {
        return Input.GetMouseButtonUp(0);
    }

    public bool IsRightMouseClick()
    {
        return Input.GetMouseButtonUp(1);
    }

    public bool isMouseRightHold()
    {
        return Input.GetMouseButton(1);
    }
    public bool IsMouseLeftHold()
    {
        return Input.GetMouseButton(0);
    }
    public float GetAxis(string axisName )
    {
        return Input.GetAxis(axisName);
    }

    public Vector3 GetMousePos()
    {
        return Input.mousePosition;
    }

    bool m_bDiscardAllInout;// 是否屏蔽所有输入 [8/4/2011 Sun]
    public bool DiscardAllInout
    {
        get { return m_bDiscardAllInout; }
        set { m_bDiscardAllInout = value; }
    }
    public bool IsAcceptInput() { return !m_bDiscardAllInout; }

    internal Vector2 GetMouseUIPos()
    {
        //主摄像机的坐标转换到屏幕坐标
        Vector3 viewPos = SceneCamera.Instance.UnityMainCamera.ScreenToViewportPoint(GetMousePos());

        //屏幕坐标转换到UI摄像机内的世界坐标
        Vector3 uiPos = UISystem.Instance.UiCamrea.ViewportToWorldPoint(viewPos);

        return new Vector2(uiPos.x, uiPos.y);
    }

    internal bool IsLeftMouseDown()
    {
        return Input.GetMouseButtonDown(0);
    }
}
