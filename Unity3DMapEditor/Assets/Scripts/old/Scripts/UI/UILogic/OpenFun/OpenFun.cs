using System;
using System.Collections.Generic;
using UnityEngine;
using Interface;

public class OpenFun : MonoBehaviour {
	
	public _DBC_FUNC_OPEN_LIST list = null;
	public UIButton tubiaoicon;
	
	void Awake()
	{
		gameObject.SetActiveRecursively(false);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_ADD_MISSION, OnEvent);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_LEVEL, OnEvent);
	}
	
	public void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
	{
		int count = OpenFuncSystem.Instance.funcList.Count;
		if(eventId == GAME_EVENT_ID.GE_ADD_MISSION)
		{
			int id = int.Parse(vParam[0]);
			if(id != -1)
			{
				for(int i = 0; i < count; i++)
				{
					if(id == OpenFuncSystem.Instance.funcList[i].receiveMission)
					{
						list = OpenFuncSystem.Instance.funcList[i];
						gameObject.SetActiveRecursively(true);
						Texture2D icon = IconManager.Instance.GetIcon(list.desc);
						tubiaoicon.SetTexture(icon);
						tubiaoicon.width = icon.width;
						tubiaoicon.height = icon.height;
					}
				}
			}
		}
		else if(eventId == GAME_EVENT_ID.GE_UNIT_LEVEL)
		{
			int nLevel = int.Parse(PlayerMySelf.Instance.GetData("LEVEL"));
			if(nLevel != -1)
			{
				for(int j = 0; j < count; j++)
				{
					if(nLevel == OpenFuncSystem.Instance.funcList[j].needLevel)
					{
						list = OpenFuncSystem.Instance.funcList[j];
						gameObject.SetActiveRecursively(true);
						Texture2D icon = IconManager.Instance.GetIcon(list.desc);
						tubiaoicon.SetTexture(icon);
						tubiaoicon.width = icon.width;
						tubiaoicon.height = icon.height;
					}
				}
			}
		}
	}
	
	void OKBtnClick()
	{
		CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_FUNC_OPEN, list.id);
		gameObject.SetActiveRecursively(false);
	}
}
