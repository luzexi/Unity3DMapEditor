using UnityEngine;
using System.Collections;
using Network.Packets;
using Network;
using System;
using System.Collections.Generic;
using System.Text;

public class GmInput : MonoBehaviour {

    public UITextField txtField;
    public UIScrollList list;
    public UISlider listSlider;
    public GameObject chatItem;
    public GameObject channelMenus;
    public UIButton changeChannelBtn;
    public List<UIButton> talkChannelMenu;
    public UIPanel panel;

    bool panelIsShowing = false;
    //玩家可选发言频道
    static string[] changeType = {
        "near",
        "world",
        "guild",
    };
    List<Vector3> mChannelMenuPos = new List<Vector3>();
    bool showChannelMenu = false;
    int[,] channel_config = 
    {
        {1,1,1,1,0,0,1,1,0,0,1},
        {0,0,1,1,0,0,1,0,0,0,0},
        {0,0,0,1,0,0,0,0,0,0,0},
        {0,0,0,0,1,0,0,0,0,0,0},
    };
    int currentSelectTab = 0;


    int[,] lastTime = {
                          {0,0,0},
                          {0,0,0},
                          {0,0,0},
                          {0,0,0},
                       };
    int currentChannel = 0; //当前选择发言频道
    string defaultColor = "#W";
    void Awake()
    {
        txtField.SetCommitDelegate(MyCommitDelegate);
        UISystem.Instance.AddHollowWindow(list.gameObject);
        list.AddInputDelegate(scrollListInputDelegate);
        listSlider.AddInputDelegate(scrollListInputDelegate);
        for (int i = 0; i < talkChannelMenu.Count; i++ )
        {
            mChannelMenuPos.Add(talkChannelMenu[i].gameObject.transform.localPosition);
        }

        channelMenus.SetActiveRecursively(false);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UI_INFOS, HandelUIInfos);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_CHAT_MESSAGE, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PLAYER_ENTER_WORLD, OnEvent);
    }

    bool inFocus = false;
    public void HandelUIInfos(GAME_EVENT_ID eventID, List<string> vParam)
    {
        if (eventID == GAME_EVENT_ID.GE_UI_INFOS)
        {
            if (vParam.Count != 0)
            {
                string info = vParam[0];
                if (info == "KeyDown_Enter")
                {
                    if (!inFocus && UIManager.instance.FocusObject != txtField)
                    {
                        UIManager.instance.FocusObject = txtField;
                    }
                    inFocus = !inFocus;
                }
            }
        }
    }
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_INFO_SELF)
        {
            if (vParam.Count != 0)
            {
                //string text = UIString.Instance.ParserString_Runtime(vParam[0]);
                //AddNewItem(vParam[0]);
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_CHAT_MESSAGE)
        {

            AddChatContext(vParam);
        }
        else if (eventId == GAME_EVENT_ID.GE_PLAYER_ENTER_WORLD)
        {
            if (panelIsShowing)
                panel.BringIn();
            else
                panel.Dismiss();
        }
    }
    // A sample commit delegate.
    // Prints a string to the console when the player
    // commits input to a text field.
    void MyCommitDelegate(IKeyFocusable field)
    {
        if (field.Content.Length == 0)
            return;

        SendChatMessage(field.Content);
		
        txtField.Text = "";
    }
    void scrollListInputDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE)
        {
            if (!panelIsShowing)
            {
                panel.BringIn();
                panelIsShowing = true;
            }
        }
        else if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
        {
            if (panelIsShowing)
            {
                panel.Dismiss();
                panelIsShowing = false;
            }
        }
    }
    void SendChatMessage(string message)
    {
        	// tempTest for hyperLink
        if (message.IndexOf("testskill") == 0)
        {
            string[] skillIDArray = message.Split(new string[1] { " " }, StringSplitOptions.None);
            if (skillIDArray.Length == 3)
            {
                int skillID = Convert.ToInt32(skillIDArray[1]);
				int isDirectUse = Convert.ToInt32(skillIDArray[2]);
				Dictionary<int, SCLIENT_SKILL> mapSkill = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Skill();
	            foreach (KeyValuePair<int, SCLIENT_SKILL> skill in mapSkill)
	            {
	                
					if(skill.Value.m_pDefine.m_nID == skillID)
					{
						int defineID = skill.Value.m_pDefine.m_nID * 100 + skill.Value.m_nLevel;
						CActionItem_Skill curSkill = CActionSystem.Instance.GetAction_SkillID(defineID);
						if(isDirectUse !=0)
						{
							curSkill.DoAction();
						}	
						else
						{
                			CObjectManager.Instance.getPlayerMySelf().SetActiveSkill(curSkill);
						}
						break;
					}
	            }
                
            }
            return;
        }
// 		if(field.Content == "testPos")
// 		{
//             SendCommonTalk("测试坐标超链接{#pos[50,50]坐标超链接}asd测试物品超链接123{#item[100000]物品超链接}");
//         	txtField.Text = "";
// 			return;
// 		}

        if (message.IndexOf("!!") == -1)
        {
            SendCommonTalk(message);
        }
        else
        {
            SendGmCommand(message.Substring(2, message.Length - 2));
        }
    }
    void SendGmCommand(string cmd)
    {
        CGCommand msg = new CGCommand();
        byte[] temp = EncodeUtility.Instance.GetGbBytes(cmd);
        Array.Copy(temp, msg.Command, temp.Length);
        msg.CommandSize = (byte)temp.Length;

        NetManager.GetNetManager().SendPacket(msg);
    }

    void SendCommonTalk(string text)
    {
        Interface.Talk.Instance.SendChatMessage(changeType[currentChannel], text);
    }
    void AddNewItem(string text)
    {
        // Instantiates a new item from the item object and
        // sets any attached text object to "Level 1":

        IUIListObject newItem = list.CreateItem(chatItem);
        SpriteText spriteText = newItem.gameObject.transform.FindChild("text").gameObject.GetComponent<SpriteText>();
        if(spriteText != null)
            spriteText.Text = UIString.Instance.ParserString_Runtime(text);

        UISystem.Instance.AddHollowWindow(newItem.gameObject);

        list.ScrollToItem(newItem, 0.0f, EZAnimation.EASING_TYPE.Linear);
        if (list.UnviewableArea > 0)
            list.slider.Hide(false);
        else
            list.slider.Hide(true);
    }
    void AddChatContext(List<string> vParams)
    {
        if (vParams.Count < 5) return;
        string chatType = vParams[0];
        string talkerName = vParams[1];
        string chatContent = vParams[2];
        string sendByMe = vParams[3];
        string sendTime = vParams[4];

        string strFinal = "";
        string strHeader = Interface.Talk.Instance.GetChannelHeader(chatType, talkerName);
        if (strHeader == null)
        {
            LogManager.LogError("Invalid channel type!");
            return;
        }
        defaultColor = getColorType(chatType);
        if (string.IsNullOrEmpty(talkerName) && chatType != "self")
        {
            strFinal = defaultColor;
            strFinal += "[" + strHeader + "]" + chatContent;
        }
        else
        {
            if (chatType != "self")
            {
                strFinal = defaultColor;
                if (chatContent[0] != '*' && chatContent[0] != '@')
                {
                    strFinal += "[" + strHeader + "]";
                    strFinal += ParseName(chatType, talkerName, sendByMe);
                    strFinal += defaultColor + "：" + chatContent;
                }
                else
                {
                    //TODO:
                    strFinal += "[" + strHeader + "]";
                    strFinal += ParseName(chatType, talkerName, sendByMe);
                    strFinal += defaultColor + "：" + chatContent;
                }
            }
        }
        bool bShow = checkShow(chatType);
        if (chatType != "self" && bShow)
        {
            AddNewItem(strFinal);
            saveLastTime(int.Parse(sendTime));
        }
    }
    string getColorType(string chatType)
    {
        if (chatType == "near" )
		    return "#W";
	    else if (chatType == "map")
		    return "#Y";
	    else if (chatType == "private")
		    return "#W";
	    else if (chatType == "guild")
		    return "#B";
	    else if (chatType == "team" )
		    return "#Y";
	    else if (chatType == "camp")
		    return "#W";
	    else if (chatType == "world")
		    return "#W";

        return "";
    }
    string ParseName(string channelType, string name, string sendByMe)
	{
        string strFinal;
	    if(channelType != "private") 
        {  
            if(isMySelf(name))
			    strFinal = "#cyan[" + name + "]" ;
		    else
                strFinal = "#cyan[" + name + "]";
		    
        }
        else
		if(sendByMe != null && int.Parse(sendByMe) == 1)

            strFinal = "发送给#cyan[" + name + "]";
		else
            strFinal = "#cyan[" + name + "]" + defaultColor + "悄悄地说";

	    return strFinal;
    }
    bool isMySelf(string name)
    {
        if (name == Interface.PlayerMySelf.Instance.GetName())
            return true;
        return false;
    }
    bool checkShow(string chatType)
    {
        int index = 0;
	    //频道名//颜色     //附近//队聊//世界//私聊//系统//自定义//帮派//阵营//仅客户端使用//谣言//区域
	    if(chatType == "near") 
		    index = 0;
	    else if(chatType == "world") 
		    index = 2;
	    else if(chatType == "system") 
		    index = 4;
	    else if(chatType == "team") 
		    index = 1;
	    else if(chatType == "private") 
		    index = 2;
	    else if(chatType == "camp") 
		    index = 7;
	    else if(chatType == "guild") 
		    index = 6;
	    else if(chatType == "map") 
		    index = 10;
	    else if(chatType == "lie") 
		    index = 9;
        if(channel_config[currentSelectTab,index] == 1) 
		    return true;
	    else
		    return false;
    }
    void saveLastTime(int time)
    {
        lastTime[currentSelectTab,2] = time;
    }
    void updateTalkChannelWidgets()
    {
        if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Guild() != MacroDefine.INVALID_ID)
        {
            talkChannelMenu[2].Hide(false);
            for (int i = 0; i < 2; i++ )
            {
                talkChannelMenu[i].gameObject.transform.localPosition = mChannelMenuPos[i];
            }
        }
        else
        {
            talkChannelMenu[2].Hide(true);
            for (int i = 0; i < 2; i++)
            {
                talkChannelMenu[i].gameObject.transform.localPosition = mChannelMenuPos[i+1];
            }
        }
    }
    void closeChannelMenus()
    {
        showChannelMenu = false;
        channelMenus.SetActiveRecursively(showChannelMenu);
    }
#region WidgetsEvent
    void OnChangeChannelBtnClicked()
    {
        showChannelMenu = !showChannelMenu;
        channelMenus.SetActiveRecursively(showChannelMenu);
        if(showChannelMenu)
            updateTalkChannelWidgets();
    }
    void OnGuildBtnClicked()
    {
        currentChannel = 2;

        closeChannelMenus();
        changeChannelBtn.Text = "宗门";
    }
    void OnNearBtnClicked()
    {
        currentChannel = 0;

        closeChannelMenus();
        changeChannelBtn.Text = "附近";
    }
    void OnWorldBtnClicked()
    {
        currentChannel = 1;

        closeChannelMenus();
        changeChannelBtn.Text = "世界";
    }
    void OnListEndBtnClicked()
    {

    }
    void OnFaceBtnClicked()
    {

    }
    void OnInputBtnClicked()
    {
        MyCommitDelegate(txtField);
    }
    void OnFilterChannelAll()
    {

    }
    void OnFilterChannelWorld()
    {

    }
    void OnFilterChannelGuild()
    {

    }
    void OnFilterChannelPrivate()
    {

    }
    void OnFilterChannelSystem()
    {

    }
#endregion
}
