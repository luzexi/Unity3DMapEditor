using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;
using System;
using System.Globalization;

public class Campaign : MonoBehaviour {
	
	public UIScrollList List;
	public GameObject listItem;
	public SpriteText CurTime;
	
	bool bNeedUpdateList = true;
	short mSelectActivityId = -1;
	
	void Awake()
	{
		gameObject.SetActiveRecursively(true);
		
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_ACTIVITYDETAIL, OnEvent);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TEAM_REFRESH_UI, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_CAMPAIGN_TEAMINFO, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TEAM_CLEAR_UI, OnEvent);
		
		ShowWindow();
		
		CamInputDelegate += JoinActivity;
	}
	
	void ShowWindow()
	{
		UIWindowMng.Instance.ShowWindow("ActivityWindow");
		UpdateActivityList();
	}
	

    //UI Event
    void OnEvent(GAME_EVENT_ID eventId, List<string> vParams)
    {
        if (eventId == GAME_EVENT_ID.GE_TOGLE_ACTIVITYDETAIL)
        {
            if (gameObject.active)
                HideWindow();
            else
                ShowWindow();
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_CAMPAIGN_TEAMINFO)
        {
            if (!gameObject.active)
            {
                CloseActivityConnect();
                return;
            }
            //UpdateTeamList();
            //UpdateMyTeamList();
        }
        //else if (eventId == GAME_EVENT_ID.GE_TEAM_REFRESH_UI)
        //{
         //   if (gameObject.active)
         //   {
        //        UpdateMyTeamList();
        //    }
        //}
        //else if (eventId == GAME_EVENT_ID.GE_TEAM_CLEAR_UI)
       // {
       //     if (gameObject.active)
       //     {
       //         UpdateMyTeamList();
        //    }
       // }
    }
	
	//刷新活动列表
	void UpdateActivityList()
	{
		if(!bNeedUpdateList)
			return;
		
		AddActivityInfoToList();
		
		bNeedUpdateList = false;
	}
	

	public delegate void CamOpClick(string opName);
	protected CamOpClick camInputDelegate;
	public Campaign.CamOpClick CamInputDelegate
	{
		get { return camInputDelegate; }
		set { camInputDelegate = value;}
	}
	
	void AddActivityInfoToList()
	{
		List.ClearList(true);
		
		CultureInfo ci;
		ci = new CultureInfo("de-DE");
		DateTime time;
		
		int nCount = CDataPool.Instance.Campaign_GetCampaignCount();
		for(int i = 0; i < nCount; i++)
		{
			_DBC_ACTIVITY_INFO info = CDataPool.Instance.Campaign_GetCampaignInfo(i);
			IUIListObject item = List.CreateItem(listItem);
			
			item.Data = (short)info.nID;
			item.gameObject.name = item.Data.ToString();
			item.gameObject.SetActiveRecursively(true);
			ActionButton action = item.gameObject.GetComponentInChildren<ActionButton>();
			if(action != null)
				action.SetIcon(info.szActivityIcon);
			SpriteText[] texts = item.gameObject.GetComponentsInChildren<SpriteText>();
			for(int j = 0; j < texts.Length; j++)
			{
				if(texts[j].gameObject.name == "Name")
					texts[j].Text = info.szActivityName;
				else if(texts[j].gameObject.name == "Time")
				{
					if(info.IsDayActivity == 1) //全天型活动
						texts[j].Text = "全天";
					else
					{
						if(DateTime.TryParse(info.StartTime, out time))
							texts[j].Text = time.ToString("HH:mm",ci);
						if(DateTime.TryParse(info.EndTime, out time))
							texts[j].Text += "-" + time.ToString("HH:mm",ci);
					}
				}
					
			}
			UIButton[] btns = item.gameObject.GetComponentsInChildren<UIButton>();
			for(int k = 0; k < btns.Length; k++)
			{
				if(btns[k].gameObject.name == "Join")
					btns[k].AddInputDelegate(OnJoinActivityClicked);
			}
			
			CurTime.Text = DateTime.Now.ToString("HH:mm");
			
			item.gameObject.SetActiveRecursively(false);
			
		}
	}
	
	void HideWindow()
	{
        UIWindowMng.Instance.HideWindow("ActivityWindow");
		CloseActivityConnect();
	}
	
	void CloseActivityConnect()
	{
		UIInfterface.Instance.CloseActivityTeamInfo(mSelectActivityId);
	}
	
	//参加活动
	void OnJoinActivityClicked(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(camInputDelegate != null)
			{
				camInputDelegate(ptr.hitInfo.collider.gameObject.transform.parent.name);
			}
		}
	}
	
	void JoinActivity(string opName)
	{
		mSelectActivityId = short.Parse(opName);
		if(!CDataPool.Instance.Campaign_CheckTime(mSelectActivityId))
			mSelectActivityId = -1;
		
		if(mSelectActivityId != -1 && !CUIDataPool.Instance.GetTeamOrGroup().HasTeam())
			UIInfterface.Instance.AutoJoinActivityTeam(mSelectActivityId);
	}
}
