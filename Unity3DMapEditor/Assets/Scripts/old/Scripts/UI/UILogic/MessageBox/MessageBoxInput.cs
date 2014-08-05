using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageBoxInput : MonoBehaviour {

    public SpriteText descText;
    public UIButton buttonOK;
    public UIButton buttonCancel;
    public UITextField inputText;

    static string WindowName = "MessageBoxInput";

    enum InputType
    {
        INPUT_ADDFRIEND, //ÃÌº”∫√”—
    }
    InputType inputType;
    void Awake()
    {
        gameObject.SetActiveRecursively(false);

        GameProcedure.s_pEventSystem.RegisterEventHandle(GAME_EVENT_ID.GE_FRIEND_ADD_FRIEND, OnEvent);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_FRIEND_ADD_FRIEND)
        {
            showAddFriendFrame();
        }
    }
    void resetWidgets()
    {
       inputText.Text = "";
    }
    void showAddFriendFrame()
    {

        inputType = InputType.INPUT_ADDFRIEND;
        descText.Text = UIString.Instance.ParserString_Runtime("#{INTERFACE_TEXT_1}");
        UIManager.instance.FocusObject = inputText;

        UIWindowMng.Instance.ShowWindow(MessageBoxInput.WindowName);
    }
    void closeWindow()
    {
        UIWindowMng.Instance.HideWindow(MessageBoxInput.WindowName);
        UIManager.instance.FocusObject = null;
        resetWidgets();
    }
    void addFriend()
    {
        string name = inputText.Text;
        if (string.IsNullOrEmpty(name))
        {
            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "#{INTERFACE_TEXT_2}");
            return;
        }
        Interface.GameInterface.Instance.AddFriendByName(name, RELATION_GROUP.RELATION_GROUP_F1);
    }

    void OnOKBtnClicked()
    {
        if (inputType == InputType.INPUT_ADDFRIEND)
        {
            addFriend();
        }

        closeWindow();
    }
    void OnCancelBtnClicked()
    {
        closeWindow();
    }

}
