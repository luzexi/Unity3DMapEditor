using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;

public class FriendScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_FRIEND, UpdateFriendUI);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_LEVEL, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_FRIEND_INFO, OnEvent);
		updateFriendUI();
        updateMySelfInfo();
	}
	public GameObject[] item;
	public GameObject pageGO;
    public SpriteText friendNumber;
    public SpriteText nameText;
    public UIButton headIcon;
	const int itemCount = 11;
	
	public void closeWindow()
	{
		 UIWindowMng.Instance.HideWindow("FriendWindow");
	}
	public void AddFriend()
	{
        //CObject selObj = CObjectManager.Instance.GetMainTarget();
        //if(selObj == null) return;
        //if( (selObj is  CObject_PlayerOther) == false )return ;
        //CObject_Character character = (CObject_Character)(selObj);
        //Interface.GameInterface.Instance.addFriend(RELATION_GROUP.RELATION_GROUP_F1,  character.GetCharacterData().Get_Name());
        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_FRIEND_ADD_FRIEND);
	}
	public void DelFriend(int index)
	{
		Interface.GameInterface.Instance.delFriend(RELATION_GROUP.RELATION_GROUP_F1, index);
	}
	GameObject getChildGO(GameObject go, string name)
	{
		Transform[]  allTrans = go.GetComponentsInChildren<Transform>();
		foreach(Transform t in allTrans)
		{
			if(t.gameObject.name == name)
				return t.gameObject;
		}
		return null;
	}
	int page = 0;
	public void AddPage()
	{
		page++;
		updateFriendUI();
	}
	public void DecPage()
	{
		page--;
		if(page<0)
			page = 0;
		updateFriendUI();
	}
    void updateMySelfInfo()
    {
        //显示玩家名字
        nameText.Text = PlayerMySelf.Instance.GetName() + "  " + PlayerMySelf.Instance.GetData("LEVEL") + "级";

        string iconName = PlayerMySelf.Instance.GetData("PORTRAIT");
        headIcon.SetTexture(IconManager.Instance.GetIcon(iconName));
    }
	void updateFriendUI()
	{
		pageGO.GetComponent<SpriteText>().Text = (page+1).ToString();
		Relation relation = CDataPool.Instance.GetRelation();
		int friendCount = relation.GetListCount(RELATION_GROUP.RELATION_GROUP_F1);
		for(int i=0; i<itemCount; ++i)
		{
			int iFriend = i + page*itemCount;
			SDATA_RELATION_MEMBER friend = relation.GetRelationInfo( RELATION_GROUP.RELATION_GROUP_F1, iFriend );
			GameObject nameGo = getChildGO(item[i],"name" );
			GameObject levelGo = getChildGO(item[i],"level" );
			if(iFriend <friendCount)
			{
				nameGo.GetComponent<SpriteText>().Text = friend.m_szName;
				levelGo.GetComponent<SpriteText>().Text = friend.m_nLevel.ToString();
                if (!item[i].active)
                    item[i].SetActiveRecursively(true);
			}
			else	
			{
                item[i].SetActiveRecursively(false);
			}
		}
        friendNumber.Text = "好友数量 " + friendCount.ToString();
	}
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_UNIT_LEVEL)
        {
            updateMySelfInfo();
        }
        else if (eventId == GAME_EVENT_ID.GE_TOGLE_FRIEND_INFO)
        {
            if (!gameObject.active)
            {
                UIWindowMng.Instance.ShowWindow("FriendWindow");
                updateMySelfInfo();
                updateFriendUI();
            }
            else
                UIWindowMng.Instance.HideWindow("FriendWindow");
        }
    }
	public void UpdateFriendUI(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
	{
        if(gameObject.active)
		    updateFriendUI();
	}
	public void DelFriend1()
	{
		DelFriend(0+page*itemCount);
	}
	public void DelFriend2()
	{
		DelFriend(1+page*itemCount);
	}
	public void DelFriend3()
	{
		DelFriend(2+page*itemCount);
	}
	public void DelFriend4()
	{
		DelFriend(3+page*itemCount);
	}
	public void DelFriend5()
	{
		DelFriend(4+page*itemCount);
	}
	public void DelFriend6()
	{
		DelFriend(5+page*itemCount);
	}
	public void DelFriend7()
	{
		DelFriend(6+page*itemCount);
	}
	public void DelFriend8()
	{
		DelFriend(7+page*itemCount);
	}
	public void DelFriend9()
	{
		DelFriend(8+page*itemCount);
	}
	public void DelFriend10()
	{
		DelFriend(9+page*itemCount);
	}
	public void DelFriend11()
	{
		DelFriend(10+page*itemCount);
	}
	
	void chat(int index)
	{
		Relation relation = CDataPool.Instance.GetRelation();
		int friendCount = relation.GetListCount(RELATION_GROUP.RELATION_GROUP_F1);
		if(index < friendCount)
		{
			SDATA_RELATION_MEMBER friend = relation.GetRelationInfo( RELATION_GROUP.RELATION_GROUP_F1, index );
			Talk.Instance.TargetName = friend.m_szName;
			Talk.Instance.SetChatType(ENUM_CHAT_TYPE.CHAT_TYPE_TELL);
		}
	}
	public void Chat1()
	{
		chat(0 +page*itemCount);
	}
	public void Chat2()
	{
		chat(1 +page*itemCount);
	}
	public void Chat3()
	{
		chat(2 +page*itemCount);
	}
	public void Chat4()
	{
		chat(3 +page*itemCount);
	}
	public void Chat5()
	{
		chat(4 +page*itemCount);
	}
	public void Chat6()
	{
		chat(5 +page*itemCount);
	}
	public void Chat7()
	{
		chat(6 +page*itemCount);
	}
	public void Chat8()
	{
		chat(7 +page*itemCount);
	}
	public void Chat9()
	{
		chat(8 +page*itemCount);
	}
	public void Chat10()
	{
		chat(9 +page*itemCount);
	}
	public void Chat11()
	{
		chat(10 +page*itemCount);
	}
	
}
