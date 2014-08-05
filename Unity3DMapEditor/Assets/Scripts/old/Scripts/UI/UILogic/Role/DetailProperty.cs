using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class DetailProperty : MonoBehaviour {
	
    public GameObject HPButton;
    public GameObject MPButton;
    public GameObject PowerButton;
    public GameObject ZhiHuiButton;
    public GameObject TiZhiButton;
    public GameObject RenXingButton;
    public GameObject MinJieButton;
    public GameObject WuLiButton;
    public GameObject MagicButton;
    public GameObject WuLiAffendButton;
    public GameObject MagicAffendButton;
    public GameObject HitButton;
    public GameObject EscapeButton;
    public GameObject BaoJiButton;
    public GameObject KangBaoButton;


    UIProgressBar hpBtn;
    UIProgressBar mpBtn;
    UIButton powerBtn;
    UIButton zhihuiBtn;
    UIButton tizhiBtn;
    UIButton renxingBtn;
    UIButton minjieBtn;
    UIButton wuliBtn;
    UIButton magicBtn;
    UIButton wuliaffendBtn;
    UIButton magicaffendBtn;
    UIButton hitBtn;
    UIButton escapeBtn;
    UIButton baojiBtn;
    UIButton kangbaoBtn;



	void Awake()
    {
    //    hpBtn = HPButton.GetComponent<UIProgressBar>();
    //    mpBtn = MPButton.GetComponent<UIProgressBar>();
    //    powerBtn = PowerButton.GetComponent<UIButton>();
    //    zhihuiBtn = ZhiHuiButton.GetComponent<UIButton>();
    //    tizhiBtn = TiZhiButton.GetComponent<UIButton>();
    //    renxingBtn = RenXingButton.GetComponent<UIButton>();
    //    minjieBtn = MinJieButton.GetComponent<UIButton>();
    //    wuliBtn = WuLiButton.GetComponent<UIButton>();
    //    magicBtn = MagicButton.GetComponent<UIButton>();
    //    wuliaffendBtn = WuLiAffendButton.GetComponent<UIButton>();
    //    magicaffendBtn = MagicAffendButton.GetComponent<UIButton>();
    //    hitBtn = HitButton.GetComponent<UIButton>();
    //    escapeBtn = EscapeButton.GetComponent<UIButton>();
    //    baojiBtn = BaoJiButton.GetComponent<UIButton>();
    //    kangbaoBtn = KangBaoButton.GetComponent<UIButton>();

    //    //gameObject.SetActiveRecursively(false);
		
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HP, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MP, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_STR, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_SPR, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_CON, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_INT, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEX, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_STR, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_SPR, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_CON, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_INT, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_DEX, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_ATT_PHYSICS,Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_ATT_MAGIC, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEF_PHYSICS, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEF_MAGIC, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HIT, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MISS, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_CRIT_RATE, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEF_CRIT_RATE, Equip_OnUpdateShow);
    //    CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_ROLE_TIPWINDOW, Equip_OnUpdateShow);
	}
		

    public void Equip_OnUpdateShow(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if(gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_HP)
            hpBtn.Value = PlayerMySelf.Instance.GetHPPercent();
        else if(gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_MP)
            mpBtn.Value = PlayerMySelf.Instance.GetMPPercent();
        else if( gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_STR || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_STR)
            powerBtn.Text = PlayerMySelf.Instance.GetData("STR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_STR");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_SPR || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_SPR)
            zhihuiBtn.Text = PlayerMySelf.Instance.GetData("SPR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_SPR");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_CON || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_CON)
            tizhiBtn.Text = PlayerMySelf.Instance.GetData("CON") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_CON");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_INT || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_INT)
            renxingBtn.Text = PlayerMySelf.Instance.GetData("INT") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_INT");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_DEX || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_DEX)
            minjieBtn.Text = PlayerMySelf.Instance.GetData("DEX") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_DEX");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_ATT_PHYSICS)
            wuliBtn.Text = PlayerMySelf.Instance.GetData("ATT_PHYSICS");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_ATT_MAGIC)
            magicBtn.Text = PlayerMySelf.Instance.GetData("ATT_MAGIC");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_DEF_PHYSICS)
            wuliaffendBtn.Text = PlayerMySelf.Instance.GetData("DEF_PHYSICS");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_DEF_MAGIC)
            magicaffendBtn.Text = PlayerMySelf.Instance.GetData("DEF_MAGIC");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_HIT)
            hitBtn.Text = PlayerMySelf.Instance.GetData("HIT");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_MISS)
            escapeBtn.Text = PlayerMySelf.Instance.GetData("MISS");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_CRIT_RATE)
            baojiBtn.Text = PlayerMySelf.Instance.GetData("CRITRATE");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_DEF_CRIT_RATE)
            kangbaoBtn.Text = PlayerMySelf.Instance.GetData("DEFCRITRATE");
		else if(gAME_EVENT_ID == GAME_EVENT_ID.GE_ROLE_TIPWINDOW)
		{
			hpBtn.Value = PlayerMySelf.Instance.GetHPPercent();
			mpBtn.Value = PlayerMySelf.Instance.GetMPPercent();
			powerBtn.Text = PlayerMySelf.Instance.GetData("STR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_STR");
			zhihuiBtn.Text = PlayerMySelf.Instance.GetData("SPR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_SPR");
			tizhiBtn.Text = PlayerMySelf.Instance.GetData("CON") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_CON");
			renxingBtn.Text = PlayerMySelf.Instance.GetData("INT") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_INT");
			minjieBtn.Text = PlayerMySelf.Instance.GetData("DEX") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_DEX");
			magicBtn.Text = PlayerMySelf.Instance.GetData("ATT_MAGIC");
			wuliBtn.Text = PlayerMySelf.Instance.GetData("ATT_PHYSICS");
			magicaffendBtn.Text = PlayerMySelf.Instance.GetData("DEF_MAGIC");
			hitBtn.Text = PlayerMySelf.Instance.GetData("HIT");
			baojiBtn.Text = PlayerMySelf.Instance.GetData("CRITRATE");
			kangbaoBtn.Text = PlayerMySelf.Instance.GetData("DEFCRITRATE");
		}

    }

    public GameObject peiyangWinGo;
    public void OnOpenPeiYang()
    {
        //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_ATTRIRANDOM);
        if(peiyangWinGo == null)
        {
            peiyangWinGo = UIWindowMng.Instance.GetWindowGo("PeiYangWindow");
        }
        bool toggle = !peiyangWinGo.active;
        if (toggle)
        {
            peiyangWinGo.transform.parent = gameObject.transform;
            Vector3 pos = Vector3.zero;
            // ÁÙÊ±Ð´ËÀ×ø±ê [2/23/2012 Ivan]
            pos.x = 61;
            peiyangWinGo.transform.localPosition = pos;
        }
        else
        {
            peiyangWinGo.transform.parent = null;
        }
        peiyangWinGo.SetActiveRecursively(toggle);
        //UIWindowMng.Instance.ShowWindow("PeiYangWindow");
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_ATTRIRANDOM);
    }
}
