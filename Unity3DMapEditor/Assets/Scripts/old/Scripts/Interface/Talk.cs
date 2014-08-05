using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Packets;
using UnityEngine;
public class Talk
{
	static readonly Talk sInstance = new Talk();
	public static Talk Instance
	{
		get
		{
			return sInstance;
		}
	}
    public  void SendTalk(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        CGChat msg = new CGChat();
		string myName = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Name();
        int myLevel = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level();
        //判断聊天类型

//             int posSpace = message.IndexOf(" ");
//             if (posSpace == -1) return;
//             string targetName = message.Substring(1, posSpace - 1);
//             string messageString = message.Substring(posSpace + 1, message.Length - posSpace - 1);
        msg.SetTalkContent(message);
        msg.ChatType = (byte)mCurrentChatType;
        if(mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_TELL)
        {
            msg.SetTarget(m_targetName);
            
        }
        else if (mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL)//近聊
        {
            
        }
        else if (mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_WORLD)//世界聊天
        {
        }
        else if(mCurrentChatType ==ENUM_CHAT_TYPE.CHAT_TYPE_MAP)//地图聊天
        {
        }
        else if(mCurrentChatType ==ENUM_CHAT_TYPE.CHAT_TYPE_TEAM)//队伍
        {
            msg.SetTeamID(m_TeamID);
        }
        else if (mCurrentChatType ==ENUM_CHAT_TYPE.CHAT_TYPE_GUILD)//工会
        {
            msg.SetGuildID(m_GuildID);
        }
        else if(mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM)//系统消息
        {

        }
        else if(mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_CAMP)//阵营
        {
            
        }
        else if (mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_LIE)//流言
        {
        }
        else if(mCurrentChatType == ENUM_CHAT_TYPE.CHAT_TYPE_INVALID)//无效
        {
            return;
        }
        ShowChat(myName + ":" + message,mCurrentChatType);
        NetManager.GetNetManager().SendPacket(msg);
    }
    public  void ReceiveTalk(GCChat msg)
    {
        ENUM_CHAT_TYPE chatType = (ENUM_CHAT_TYPE)msg.ChatType;
        string name = EncodeUtility.Instance.GetUnicodeString(msg.SourName) + ":";
        string text = Encoding.UTF8.GetString(msg.Contex);
		// 删除不必要的多余字符
        string showText = (name + text).Replace("\0", "");
        if ( chatType == ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL )
        {
            ShowChat(showText, chatType);
            CObject_Character talker = (CObject_Character)CObjectManager.Instance.FindServerObject((int)msg.SourObject);
            talker.ShowTalk(text);
        }
        else if (chatType == ENUM_CHAT_TYPE.CHAT_TYPE_TELL)//私聊
        {
            ShowChat(showText, chatType);
        }
    }
    void ShowChat(string msg, ENUM_CHAT_TYPE chatType)
    {
        Color color = Color.black;
        switch(chatType)
        {
            case ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL:
                color = Color.white;
                break;
            case ENUM_CHAT_TYPE.CHAT_TYPE_SYSTEM:
                color = Color.red;
                break;
            case ENUM_CHAT_TYPE.CHAT_TYPE_TELL:
                color = new Color(1.0f, 0.65f, 0.0f, 1.0f);
                break;
        }
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, color + msg);
    }

    //队伍号，仅在队伍聊天时有效
    short m_TeamID;
    public short TeamID
    {
        set { m_TeamID = value; }
        get { return m_TeamID; }
    }
    //频道号，仅在自建聊天频道聊天时有效
    short m_ChannelID;
    public short ChannelID
    {
        set { m_ChannelID = value; }
        get { return m_ChannelID; }
    }
    //帮派号，仅属于此帮派的成员有效
    short m_GuildID;
    public short GuildID
    {
        set { m_GuildID = value; }
        get { return m_GuildID;  }
    }
    //门派值，仅此门派内的成员有效
    byte m_MenpaiID;
    public byte MenpaiID
    {
        set { m_MenpaiID = value; }
        get { return m_MenpaiID;  }
    }
    string m_targetName;//私聊目标玩家
    public string TargetName
    {
        set { m_targetName = value; }
        get { return m_targetName; }
    }
    ENUM_CHAT_TYPE mCurrentChatType = ENUM_CHAT_TYPE.CHAT_TYPE_NORMAL;//当前的聊天类型
    public void SetChatType(ENUM_CHAT_TYPE type)
    {
        mCurrentChatType = type;
    }
}