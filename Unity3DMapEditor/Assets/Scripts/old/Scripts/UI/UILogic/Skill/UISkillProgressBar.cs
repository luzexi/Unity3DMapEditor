using UnityEngine;
using System.Collections.Generic;
using System;
public class UISkillProgressBar : MonoBehaviour 
{
    UIProgressBar progress_;
    void Awake()
    {
        progress_ = GetComponentInChildren<UIProgressBar>();
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PROGRESSBAR_SHOW,SkillProgressBar_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PROGRESSBAR_HIDE,SkillProgressBar_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH,SkillProgressBar_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PLAYER_LEAVE_WORLD,SkillProgressBar_Update);
		progress_.Text = "";
		gameObject.SetActiveRecursively(false);
    }
	void Start()
	{
		EZScreenPlacement ScreenPlacement = gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
        {
			ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
        }
	}

    public void SkillProgressBar_Update(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        switch (gAME_EVENT_ID)
        {
            case GAME_EVENT_ID.GE_PROGRESSBAR_SHOW:
                {
                    
                    gameObject.SetActiveRecursively(true);        
					//progress_.Hide(false);
                }
                break;
            case GAME_EVENT_ID.GE_PROGRESSBAR_HIDE:
                {
                    gameObject.SetActiveRecursively(false);      
					//progress_.Hide(true);
                }
                break;
            case GAME_EVENT_ID.GE_PROGRESSBAR_WIDTH:
                {
                    progress_.Value = (float)(Convert.ToDouble(vParam[0]));
                   // progress_.Text = "" + progress_.Value * 100 + "/100";
                }
                break;
            case GAME_EVENT_ID.GE_PLAYER_LEAVE_WORLD:
                {
                    gameObject.SetActiveRecursively(false);        
					//progress_.Hide(false);
                }
                break;
        }
    }
}