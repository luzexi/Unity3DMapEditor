using UnityEngine;
using System.Collections.Generic;
using Interface;
/// <summary>
/// 弹出消息框
/// </summary>
public class MessageBoxSelf : MonoBehaviour
{
    public delegate void OnEventCallback();
    public UIButton mBtnOk;
    public UIButton mBtnCancel;
    public SpriteText mText;
    public SpriteText mTitle;
    OnEventCallback mOkCallback = null;
    OnEventCallback mCancelCallback = null;
    public enum EMUN_MSG_RESULT
    {
        OK,
        CANCLE,
    }
    public bool mClose = true;
    private EMUN_MSG_RESULT mResult = EMUN_MSG_RESULT.CANCLE;
    public EMUN_MSG_RESULT Result
    {
        get { return mResult; }
    }
    MessageType messageType;
    enum MessageType
    {
        MT_NONE = -1,
        MT_NETCLOSE, //网络关闭
        MT_DISCARD_ITEM_FRAME,//丢弃道具
    }
#region MonoBehaviour

    void Awake()
    {
        gameObject.SetActiveRecursively(false);

        RegistEvent();
    }
#endregion
#region LogicCode
    void RegistEvent()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_NET_CLOSE, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_MESSAGEBOX, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_OPEN_DISCARD_ITEM_FRAME, OnEvent);
    }

    public void Show()
    {
        mClose = false;
		UIWindowMng.Instance.ShowWindow("MessageBoxSelf");
    }

    public void Show(string title, string desc, OnEventCallback callback1, OnEventCallback callback2)
    {
        mClose = false;
        mOkCallback     = callback1;
        mCancelCallback = callback2;
        UIWindowMng.Instance.ShowWindow("MessageBoxSelf");
        mTitle.Text = title;
        mText.Text = desc;
    }

    void Close()
    {
        UIWindowMng.Instance.HideWindow("MessageBoxSelf");
    }
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_NET_CLOSE)
        {
            mText.Text = UIString.Instance.ParserString_Runtime(vParam[0]);
            messageType = MessageType.MT_NETCLOSE;

            Show();
        }
        else if (eventId == GAME_EVENT_ID.GE_GAMELOGIN_SHOW_SYSTEM_INFO)
        {
            mText.Text = UIString.Instance.ParserString_Runtime(vParam[0]);
            Show();
        }
        else if (eventId == GAME_EVENT_ID.GE_MESSAGEBOX)
        {
            mText.Text = UIString.Instance.ParserString_Runtime(vParam[0]);
            Show();
        }
        else if (eventId == GAME_EVENT_ID.GE_OPEN_DISCARD_ITEM_FRAME)
        {
            cancelLastOp(MessageType.MT_DISCARD_ITEM_FRAME);
            messageType = MessageType.MT_DISCARD_ITEM_FRAME;
            mText.Text = "确实要丢弃 " + vParam[0] + " ?";
            Show();
        }
    }
    void cancelLastOp(MessageType mt)
    {
        if (gameObject.active && mt != messageType)
            OnCancel();
    }
    void handleOK()
    {
        if (messageType == MessageType.MT_DISCARD_ITEM_FRAME)
        {
            UIInfterface.Instance.DiscardItem();
        }

        messageType = MessageType.MT_NONE;
    }
    void handleCancel()
    {
        if (messageType == MessageType.MT_DISCARD_ITEM_FRAME)
        {
            UIInfterface.Instance.DiscardItemCancelLocked();
        }
        messageType = MessageType.MT_NONE;
    }
#endregion
#region UIEvent
    void OnOK()
    {
        Close();
        mResult = EMUN_MSG_RESULT.OK;
        mClose = true;
        if (mOkCallback != null)
        {
            mOkCallback();
        }
        mOkCallback = null;
        handleOK();
    }
    void OnCancel()
    {
        Close();
        mResult = EMUN_MSG_RESULT.CANCLE;
        mClose = true;
        if (mCancelCallback != null)
        {
            mCancelCallback();
        }
        mCancelCallback = null;
        handleCancel();
    }
#endregion
}