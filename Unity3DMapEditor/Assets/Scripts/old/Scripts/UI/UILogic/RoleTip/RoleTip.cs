using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class RoleTip : MonoBehaviour
{
	public GameObject NameButton;
	public GameObject LevelButton;
	public GameObject WuXingButton;
    public GameObject AmbitButton;
	public GameObject HPButton;
    public GameObject MPButton;
	public UIProgressBar EnergyButton;
    public UIButton headIcon;
	public SpriteText txtMoney;
    public SpriteText txtRmb;
	
	
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
		
		
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_NAME, PlayerFrame_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_LEVEL,PlayerFrame_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_AMBIT, PlayerFrame_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HP,PlayerFrame_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MP,PlayerFrame_Update);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_ENERGY,PlayerFrame_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MONEY, PlayerFrame_Update);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_RMB, PlayerFrame_Update);
    }
	
	public void PlayerFrame_Update(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
	
		string msg;
		string m_WuXing;
		
		//显示玩家名字
		button1.Text = PlayerMySelf.Instance.GetName();
		msg="玩家名字:"+button1.Text;
	    //LogManager.Log(msg);
		
		//显示玩家等级
		button2.Text = PlayerMySelf.Instance.GetData("LEVEL");
		msg="玩家等级:"+button2.Text;
		//LogManager.Log(msg);
		
		m_WuXing = PlayerMySelf.Instance.GetData("MENPAI");
		if(m_WuXing == "0")
		{
			button3.SetIcon("角色--金");
			//LogManager.LogError("五行属性为金");
		}
		else if(m_WuXing == "4")
		{
			button3.SetIcon("角色--木");
			//LogManager.LogError("五行属性为木");
		}
		else if(m_WuXing == "1")
		{
			button3.SetIcon("角色--水");
			//LogManager.LogError("五行属性为水");
		}
		else if(m_WuXing == "2")
		{
			button3.SetIcon("角色--火");
			//LogManager.LogError("五行属性为火");
		}
		else if(m_WuXing == "3")
		{
			button3.SetIcon("角色--土");
			//LogManager.LogError("五行属性为土");
		}

        string nAmbit;
        nAmbit = PlayerMySelf.Instance.GetData("AMBIT");
        if (nAmbit == "凡人期")
        {
            button4.SetIcon("icon3");
            //LogManager.LogError("境界为凡人期");
        }
        else if (nAmbit == "炼气初期")
        {
            button4.SetIcon("icon4");
            //LogManager.LogError("境界为炼气初期");
        }
        else if (nAmbit == "炼气中期")
        {
            button4.SetIcon("icon5");
            //LogManager.LogError("境界为炼气中期");
        }
        else if (nAmbit == "炼气后期")
        {
            button4.SetIcon("30008002i");
            //LogManager.LogError("境界为炼气后期");
        }
        else if (nAmbit == "筑基初期")
        {
            button4.SetIcon("packet");
            //LogManager.LogError("境界为筑基初期");
        }
        else if (nAmbit == "筑基中期")
        {
            button4.SetIcon("icon4");
            //LogManager.LogError("境界为筑基中期");
        }
        else if (nAmbit == "筑基后期")
        {
            button4.SetIcon("icon5");
            //LogManager.LogError("境界为筑基后期");
        }
        else if (nAmbit == "结丹初期")
        {
            button4.SetIcon("30008002i");
            //LogManager.LogError("境界为结丹初期");
        }
        else if (nAmbit == "结丹中期")
        {
            button4.SetIcon("packet");
            //LogManager.LogError("境界为结丹中期");
        }
        else if (nAmbit == "结丹后期")
        {
            button4.SetIcon("packet");
            //LogManager.LogError("境界为结丹后期");
        }
        else if (nAmbit == "元婴初期")
        {
            button4.SetIcon("icon4");
            //LogManager.LogError("境界为元婴初期");
        }
        else if (nAmbit == "元婴中期")
        {
            button4.SetIcon("icon5");
            //LogManager.LogError("境界为元婴中期");
        }
        else if (nAmbit == "元婴后期")
        {
            button4.SetIcon("30008002i");
            //LogManager.LogError("境界为元婴后期");
        }

		
		//显示玩家的生命值
        string PlayerHP = PlayerMySelf.Instance.GetData("HP");
        string PlayerMaxHP = PlayerMySelf.Instance.GetData("MAXHP");
		//LogManager.LogError("HP: " + PlayerHP + " MaxHP: " + PlayerMaxHP);
        button5.Value = PlayerMySelf.Instance.GetHPPercent();
		//LogManager.LogError("生命值：" + button5.Value);
        button5.Text = PlayerHP + "/" + PlayerMaxHP;
        
        //显示玩家的魔法值
        string PlayerMP = PlayerMySelf.Instance.GetData("MP");
        string PlayerMaxMP = PlayerMySelf.Instance.GetData("MAXMP");
        button6.Value = PlayerMySelf.Instance.GetMPPercent();
        button6.Text = PlayerMP + "/" + PlayerMaxMP;
		
		//显示玩家的精力
        int PlayerEnergy = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_DoubleExpTime_Num();
        int PlayerMaxEnergy = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_MaxEnergy();
        EnergyButton.Value = PlayerEnergy/PlayerMaxEnergy;
		EnergyButton.Text = PlayerEnergy + "/" + PlayerMaxEnergy;

        // 显示头像 [3/30/2012 Ivan]
        string iconName = PlayerMySelf.Instance.GetData("PORTRAIT");
        headIcon.SetTexture(IconManager.Instance.GetIcon(iconName));
		
		UpdateMoney();
	}
	
	private void UpdateMoney()
    {
        if (txtMoney != null)
        {
            int money = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money();
            txtMoney.Text = money.ToString();
        }

        if (txtRmb != null)
        {
            int rmb = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_RMB();
            txtRmb.Text = rmb.ToString();
        }
    }
}