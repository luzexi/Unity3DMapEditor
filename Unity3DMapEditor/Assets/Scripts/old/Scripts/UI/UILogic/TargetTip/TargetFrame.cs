using UnityEngine;
using System;
using System.Collections.Generic;

using Interface;

public class TargetFrame : MonoBehaviour {

    public GameObject NameButton;
    public GameObject LevelButton;
    public GameObject WuXingButton;
    public GameObject AmbitButton;
    public GameObject HPButton;
    public GameObject MPButton;
    public UIButton headIcon;


    SpriteText button1;
    SpriteText button2;
    ActionButton button3;
    ActionButton button4;
    UIProgressBar button5;
    UIProgressBar button6;

    void Awake()
    {
        button1 = NameButton.GetComponent<SpriteText>();
        button2 = LevelButton.GetComponent<SpriteText>();
        button3 = WuXingButton.GetComponent<ActionButton>();
        button4 = AmbitButton.GetComponent<ActionButton>();
        button5 = HPButton.GetComponent<UIProgressBar>();
        button6 = MPButton.GetComponent<UIProgressBar>();
		
		//gameObject.SetActiveRecursively(false);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_MAINTARGET_CHANGED, TargetFrame_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MP, TargetFrame_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HP, TargetFrame_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_LEVEL, TargetFrame_Update);

        // Init Camera [4/1/2012 Ivan]
        EZScreenPlacement ScreenPlacement = gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
            ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
        updateTargetTip();
    }

    public void TargetFrame_Update(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (Character.Instance.IsPresent())
        {
			gameObject.SetActiveRecursively(true);
            updateTargetTip();
        }
        else
        {
            this.gameObject.SetActiveRecursively(false);
        }
    }

    public void updateTargetTip()
    {
        TargetFrame_Update_Name();
        TargetFrame_Update_HP();
        TargetFrame_Update_MP();
        TargetFrame_Update_Level();
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        
        string iconName = Character.Instance.GetHeadIcon();
        headIcon.SetTexture(IconManager.Instance.GetIcon(iconName));
    }

    public void TargetFrame_Update_Name()
    {
        button1.Text = Character.Instance.GetName();
    }

    public void TargetFrame_Update_HP()
    {
        button5.Value = Character.Instance.GetHPPercent();
        button5.Text = "";
    }

    public void TargetFrame_Update_MP()
    {
        button6.Value = Character.Instance.GetMPPercent();
        button6.Text = "";
    }

    public void TargetFrame_Update_Level()
    {
        button2.Text = Character.Instance.GetData("LEVEL",-1);
    }
}
