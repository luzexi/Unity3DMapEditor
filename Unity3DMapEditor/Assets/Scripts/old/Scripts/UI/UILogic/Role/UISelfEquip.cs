using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;
using DBSystem;
using Network;
using Network.Packets;
public class UISelfEquip : MonoBehaviour{
	

	//SpriteText nameBtn;
    //UIProgressBar expButton;

    public SpriteText nameBtn;
    public UIProgressBar expBtn;

    public UIProgressBar hpBtn;
    public UIProgressBar mpBtn;
    //public UIButton powerBtn;
    //public UIButton zhihuiBtn;
    //public UIButton tizhiBtn;
    //public UIButton renxingBtn;
    //public UIButton minjieBtn;
   // public UIButton wuliBtn;
    //public UIButton magicBtn;
    //public UIButton wuliaffendBtn;
    //public UIButton magicaffendBtn;
    //public UIButton hitBtn;
    //public UIButton escapeBtn;
    //public UIButton baojiBtn;
    //public UIButton kangbaoBtn;
	public SpriteText powerBtn;
	public SpriteText zhihuiBtn;
	public SpriteText tizhiBtn;
	public SpriteText renxingBtn;
	public SpriteText minjieBtn;
	public SpriteText wuliBtn;
	public SpriteText magicBtn;
	public SpriteText wuliaffendBtn;
	public SpriteText magicaffendBtn;
	public SpriteText hitBtn;
	public SpriteText escapeBtn;
	public SpriteText baojiBtn;
	public SpriteText kangbaoBtn;

    public UIButton turnLeft_;
    public UIButton turnRight_;
    
    public List<UIRadioBtn> mRoleMenus;
    public GameObject mSelfEquiptWindow;
    public GameObject mPetEquiptWindow;
	
	bool toggle = false;

    private float mRotateSpeed;
    private bool  mRotating;
    RoleType  mCurrentRoleType = RoleType.ROLE_SELF;
    PET_EQUIP mCurrentPetEquipt = PET_EQUIP.PEQUIP_INVALID;
    enum RoleType
    {
        ROLE_SELF = 0,
        ROLE_PET1,
        ROLE_PET2,
        ROLE_PET3,
        ROLE_PET4,
        ROLE_PET5,
        ROLE_PET6,
    }

    public PET_EQUIP ActivePetEquiptPart
    {
        get 
        {
            PET_EQUIP resultEquipt = mCurrentPetEquipt;
            mCurrentPetEquipt = PET_EQUIP.PEQUIP_INVALID;
            return resultEquipt; 
        }
    }

	void Awake()
	{
        gameObject.SetActiveRecursively(true);
		peiyangWinGo.SetActiveRecursively(false);
		//nameBtn = NameButton.GetComponent<SpriteText>();
        //expButton = ExpButton.GetComponent<UIProgressBar>();
		
		//gameObject.transform.root.gameObject.SetActiveRecursively(true);
		
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_ROLE_TIPWINDOW,RefreshEquip);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_NAME,Equip_OnUpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_EXP, Equip_OnUpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_EQUIP, RefreshEquip);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HP, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MP, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_STR, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_SPR, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_CON, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_INT, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEX, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_STR, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_SPR, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_CON, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_INT, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_BRING_DEX, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_ATT_PHYSICS, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_ATT_MAGIC, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEF_PHYSICS, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEF_MAGIC, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HIT, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_MISS, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_CRIT_RATE, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_DEF_CRIT_RATE, Equip_UpdateShow);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_PRESCR, Equip_UpdatePrescr);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, UpdateEquiptLevelUpButton);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_PETEQUIP, UpdatePetEquipt);

        
        for (int i = 0; i < mRoleMenus.Count; i++)
        {
            mRoleMenus[i].SetValueChangedDelegate(RoleMenuValueChanged);
            if (mRoleMenus[i].defaultValue)
            {
                mCurrentRoleType = (RoleType)i;
            }
        }  

        petTurnLeft_.AddInputDelegate(OnMouseEvent);
        petTurnRight_.AddInputDelegate(OnMouseEvent);
        turnLeft_.AddInputDelegate(OnMouseEvent);
        turnRight_.AddInputDelegate(OnMouseEvent);

        petHeadLevelUp_.AddInputDelegate(OnMouseLevelUpEquipt);
        petSpurLevelUp_.AddInputDelegate(OnMouseLevelUpEquipt);
        petClawLevelUp_.AddInputDelegate(OnMouseLevelUpEquipt);
        petRingLevelUp_.AddInputDelegate(OnMouseLevelUpEquipt);
        petBodyLevelUp_.AddInputDelegate(OnMouseLevelUpEquipt);
        petTattooLevelUp_.AddInputDelegate(OnMouseLevelUpEquipt);



        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_PET_PAGE, OnPetEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_PET_PAGE, OnPetEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_ACCELERATE_KEYSEND, OnPetEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_LEVEL, OnPetEvent);

        RefreshRolePageCaption();
        //Equip_RefreshEquip();
        //PlayerInfo_Update();
        //Tip_Refresh();
        OnChangedRoleType(RoleType.ROLE_SELF);

        //查询自己宠物的装备信息
        for (int i = 0; i < GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++)
        {
            SDATA_PET curPet = CDataPool.Instance.Pet_GetPet(i);
            if (!curPet.GUID.IsNull())
            {
                CGAskDetailPetEquipList queryPet = new CGAskDetailPetEquipList();
                queryPet.GUID = curPet.GUID;
                queryPet.EquiptType = ASK_PET_EQUIP_TYPE.ASK_PET_EQUIP_MYSELF;
                queryPet.ObjID = -1;//(int)curPet.idServer;
                NetManager.GetNetManager().SendPacket(queryPet);
            }
        }

		if(gameObject.active)
		{
			CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ROLE_TIPWINDOWSHOWN);
		}
	}

    void RefreshPage(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        RefreshRolePageCaption();
    }

    void SetActivePage(RoleType type)
    {
        if (mCurrentRoleType != type)
        {
            mRoleMenus[(int)mCurrentRoleType].Value = true;
            mCurrentRoleType = type;
            mRoleMenus[(int)mCurrentRoleType].Value = true;
        }
    }

    void RefreshRolePageCaption()
    {
        int count = CDataPool.Instance.Pet_GetCount();
        for (int i = 1; i < mRoleMenus.Count; i++)
        {
            mRoleMenus[i].Hide(true);
        }
        for (int i = 1; i <= count; i++)
        {

            mRoleMenus[i].Hide(false);
            SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(i - 1);
            mRoleMenus[i].Text = curPet.Name;
        }
    }

	//更新主角基本信息
	public void Equip_OnUpdateShow(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
	{
		PlayerInfo_Update();
	}
	
	void PlayerInfo_Update()
	{
		string strName = PlayerMySelf.Instance.GetName();
		string nLevel = PlayerMySelf.Instance.GetData("LEVEL");
        float exp = PlayerMySelf.Instance.GetFloatData("EXP");
        float max_Exp = PlayerMySelf.Instance.GetFloatData("NEEDEXP");
		
		nameBtn.Text = strName + " :" + nLevel + "级";
        expBtn.Value = exp / max_Exp;
        expBtn.Text = exp.ToString("0") + "/" + max_Exp.ToString("0"); 
	}
    //更新装备信息
    public void RefreshEquip(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
		if(gAME_EVENT_ID == GAME_EVENT_ID.GE_ROLE_TIPWINDOW)
		{
			if(!gameObject.active)
			{
               // mRoleMenus[(int)mCurrentRoleType].Value = false;
             //   mCurrentRoleType = RoleType.ROLE_SELF;
                UIWindowMng.Instance.ShowWindow("RoleTipWindow");
				peiyangWinGo.SetActiveRecursively(toggle);
              //  mRoleMenus[(int)mCurrentRoleType].Value = true;
                OnChangedRoleType(mCurrentRoleType);
			}
			else 
				CloseWindow();				
		}
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UPDATE_EQUIP && vParam.Count == 0)
        {
            if(gameObject.active)
			{
                mRoleMenus[(int)mCurrentRoleType].Value = false;
                mCurrentRoleType = RoleType.ROLE_SELF;
				mRoleMenus[(int)mCurrentRoleType].Value = true;
				//OnChangedRoleType(RoleType.ROLE_SELF);
			}
        }
        
        //PlayerInfo_Update();
        //Equip_RefreshEquip();
        //Tip_Refresh();
    }

    //更新属性界面信息
    public void Equip_UpdateShow(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_HP)
            hpBtn.Value = PlayerMySelf.Instance.GetHPPercent();
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_MP)
            mpBtn.Value = PlayerMySelf.Instance.GetMPPercent();
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_STR || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_STR)
            powerBtn.Text = PlayerMySelf.Instance.GetData("STR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_STR");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_SPR || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_SPR)
            zhihuiBtn.Text = PlayerMySelf.Instance.GetData("SPR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_SPR");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_CON || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_CON)
            tizhiBtn.Text = PlayerMySelf.Instance.GetData("CON") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_CON");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_INT || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_INT)
            renxingBtn.Text = PlayerMySelf.Instance.GetData("INT") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_INT");
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_DEX || gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_BRING_DEX)
        {
            minjieBtn.Text = PlayerMySelf.Instance.GetData("DEX") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_DEX");
           
        }
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
    }
    public void Equip_UpdatePrescr(GAME_EVENT_ID eventId, List<string> vParam)
    {

    }
    //bool toggleXiangXiWindow = false;
    //public GameObject XiangXiWindow;
    //public void XiangXiWindowOpen()
    //{
		//if(XiangXiWindow == null)
		//{
            //RegisToMain();
		//}
        //toggleXiangXiWindow = !toggleXiangXiWindow;

        //if (toggleXiangXiWindow)
        //{
            //XiangXiWindow.transform.parent = gameObject.transform.root;
            //Vector3 pos = Vector3.zero;
            // 临时写死坐标 [2/23/2012 Ivan]
           // pos.x = 78;
            //XiangXiWindow.transform.localPosition = pos;
        //}
        //else
        //{
            //XiangXiWindow.transform.parent = null;
       // }
       // XiangXiWindow.SetActiveRecursively(toggleXiangXiWindow);
    //}

    //void RegisToMain()
    //{
        //GameObject Window = UIWindowMng.Instance.GetWindowGo("RoleInfoWindow");
        //if (Window != null)
        //{
			//XiangXiWindow = Window;
        //}
    //}
    public void CloseWindow()
    {
        //LogManager.Log("Enter fucking Close Window");
        UIWindowMng.Instance.HideWindow("RoleTipWindow");
      // gameObject.SetActiveRecursively(false);
    }

    public ActionButton capBtn;
    public ActionButton armorBtn;
    public ActionButton shoulderBtn;
    public ActionButton bootBtn;
    public ActionButton backBtn; //腰带
    public ActionButton weaponBtn;
    public ActionButton ringBtn;//戒指
    public ActionButton wristBtn;//手镯
    public ActionButton necklaceBtn;//项链
    public ActionButton yuPeiBtn;//玉佩
    void Equip_RefreshEquip()
    {
        CActionItem weapon = CActionSystem.Instance.EnumAction(0, ActionNameType.Equip);
        if (weaponBtn != null)
        {
            if (weapon != null)
            {
                weaponBtn.UpdateItemFromAction(weapon);
                SetLocked(weaponBtn, weapon);
            }
            else
                weaponBtn.SetActionItem(-1);
        }
        CActionItem cap = CActionSystem.Instance.EnumAction(1, ActionNameType.Equip);
        if (capBtn != null)
        {
            if (cap != null)
            {
                capBtn.UpdateItemFromAction(cap);
                SetLocked(capBtn, cap);
            }
            else
                capBtn.SetActionItem(-1);
        }
        CActionItem armor = CActionSystem.Instance.EnumAction(2, ActionNameType.Equip);
        if (armorBtn != null)
        {
            if (armor != null)
            {
                armorBtn.UpdateItemFromAction(armor);
                SetLocked(armorBtn, armor);
            }
            else
                armorBtn.SetActionItem(-1);
        }
        CActionItem boot = CActionSystem.Instance.EnumAction(4, ActionNameType.Equip);
        if (bootBtn != null)
        {
            if (boot != null)
            {
                bootBtn.UpdateItemFromAction(boot);
                SetLocked(bootBtn, boot);
            }
            else
                bootBtn.SetActionItem(-1);
        }
        CActionItem shoulder = CActionSystem.Instance.EnumAction(15, ActionNameType.Equip);
        if (shoulderBtn != null)
        {
            if (shoulder != null)
            {
                shoulderBtn.UpdateItemFromAction(shoulder);
                SetLocked(shoulderBtn, shoulder);
            }
            else
                shoulderBtn.SetActionItem(-1);
        }
        CActionItem ring = CActionSystem.Instance.EnumAction(6, ActionNameType.Equip);
        if (ringBtn != null)
        {
            if (ring != null)
            {
                ringBtn.UpdateItemFromAction(ring);
                SetLocked(ringBtn, ring);
            }
            else
                ringBtn.SetActionItem(-1);
        }
        CActionItem necklace = CActionSystem.Instance.EnumAction(7, ActionNameType.Equip);
        if (necklaceBtn != null)
        {
            if (necklace != null)
            {
                necklaceBtn.UpdateItemFromAction(necklace);
                SetLocked(necklaceBtn, necklace);
            }
            else
                necklaceBtn.SetActionItem(-1);
        }
        CActionItem back = CActionSystem.Instance.EnumAction(5, ActionNameType.Equip);
        if (backBtn != null)
        {
            if (back != null)
            {
                backBtn.UpdateItemFromAction(back);
                SetLocked(backBtn, back);
            }
            else
                backBtn.SetActionItem(-1);
        }
        CActionItem yupei = CActionSystem.Instance.EnumAction(13, ActionNameType.Equip);
        if (yuPeiBtn != null)
        {
            if (yupei != null)
            {
                yuPeiBtn.UpdateItemFromAction(yupei);
                SetLocked(yuPeiBtn, yupei);
            }
            else
                yuPeiBtn.SetActionItem(-1);
        }
        CActionItem wrist = CActionSystem.Instance.EnumAction(3, ActionNameType.Equip);
        if (wristBtn != null)
        {
            if (wrist != null)
            {
                wristBtn.UpdateItemFromAction(wrist);
                SetLocked(wristBtn, wrist);
            }
            else
                wristBtn.SetActionItem(-1);
        }

    }
    void SetLocked(ActionButton actionBtn, CActionItem actionItem)
    {
        CObject_Item item = actionItem.GetImpl() as CObject_Item;
        if (item.isLocked)
            actionBtn.Disable();
        else
            actionBtn.Enable();
    }
	void Tip_Refresh()
	{
		hpBtn.Value = PlayerMySelf.Instance.GetHPPercent();
		string nHp = PlayerMySelf.Instance.GetData("HP");
		string nMp = PlayerMySelf.Instance.GetData("MP");
        string MaxHp = PlayerMySelf.Instance.GetData("MAXHP");
		string MaxMp = PlayerMySelf.Instance.GetData("MAXMP");
        
		hpBtn.Text = nHp + "/" + MaxHp;
		mpBtn.Text = nMp + "/" + MaxMp;
		mpBtn.Value = PlayerMySelf.Instance.GetMPPercent();
		powerBtn.Text = PlayerMySelf.Instance.GetData("STR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_STR");
		zhihuiBtn.Text = PlayerMySelf.Instance.GetData("SPR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_SPR");
		tizhiBtn.Text = PlayerMySelf.Instance.GetData("CON") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_CON");
		renxingBtn.Text = PlayerMySelf.Instance.GetData("INT") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_INT");
		minjieBtn.Text = PlayerMySelf.Instance.GetData("DEX") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_DEX");
		magicBtn.Text = PlayerMySelf.Instance.GetData("ATT_MAGIC");
		wuliBtn.Text = PlayerMySelf.Instance.GetData("ATT_PHYSICS");
		wuliaffendBtn.Text = PlayerMySelf.Instance.GetData("DEF_PHYSICS");
		magicaffendBtn.Text = PlayerMySelf.Instance.GetData("DEF_MAGIC");
		hitBtn.Text = PlayerMySelf.Instance.GetData("HIT");
		baojiBtn.Text = PlayerMySelf.Instance.GetData("CRITRATE");
		kangbaoBtn.Text = PlayerMySelf.Instance.GetData("DEFCRITRATE");
		escapeBtn.Text = PlayerMySelf.Instance.GetData("MISS");
	}
	
	public GameObject peiyangWinGo;
    public GameObject equiptWinGo;
	public void OnOpenPeiYang()
    {
        //if(peiyangWinGo == null)
        //{
        //    peiyangWinGo = UIWindowMng.Instance.GetWindowGo("PeiYangWindow");
        //    if (peiyangWinGo == null)
        //        return;
       // }
        toggle = !peiyangWinGo.active;
        if (toggle)
        {
            peiyangWinGo.transform.parent = gameObject.transform;
            Vector3 pos = Vector3.zero;
            pos.x = 78;
            peiyangWinGo.transform.localPosition = pos;
        }
        else
        {
            peiyangWinGo.transform.parent = null;
        }
        peiyangWinGo.SetActiveRecursively(toggle);
        //UIWindowMng.Instance.ShowWindow("PeiYangWindow");
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_ATTRIRANDOM,0);
    }

    void RoleMenuValueChanged(IUIObject obj)
    {
        UIRadioBtn radio = obj as UIRadioBtn;
        if (radio != null && radio.Value)
        {
            for (int i = 0; i < mRoleMenus.Count; i++)
            {
                if (radio == mRoleMenus[i])
                {
                    mCurrentRoleType = (RoleType)i;
                    OnChangedRoleType(mCurrentRoleType);
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_ATTRIRANDOM);
                    return;
                }
            }
        }
    }

    void OnChangedRoleType(RoleType rt)
    {
        bool bResetSelectIndex = true;
        RefreshRolePageCaption();
        switch (rt)
        {
            case RoleType.ROLE_SELF:
                {
                    if (mPetEquiptWindow.active)
                        mPetEquiptWindow.SetActiveRecursively(false);

                    if (!mSelfEquiptWindow.active)
                    {
                        mSelfEquiptWindow.SetActiveRecursively(true);
                    }
                    Equip_RefreshEquip();
                    PlayerInfo_Update();
                    Tip_Refresh();
                }
                break;
            case RoleType.ROLE_PET1:
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_PET_PAGE,1);
                    mSelfEquiptWindow.SetActiveRecursively(false);
                    if (!mPetEquiptWindow.active)
                    {
                        mPetEquiptWindow.SetActiveRecursively(true);
                    }
                    RefreshPet();
                }
                break;
            case RoleType.ROLE_PET2:
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_PET_PAGE, 2);
                    mSelfEquiptWindow.SetActiveRecursively(false);
                    if (!mPetEquiptWindow.active)
                    {
                        mPetEquiptWindow.SetActiveRecursively(true);
                    }
                    RefreshPet();
                }
                break;
            case RoleType.ROLE_PET3:
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_PET_PAGE, 3);
                    mSelfEquiptWindow.SetActiveRecursively(false);
                    if (!mPetEquiptWindow.active)
                    {
                        mPetEquiptWindow.SetActiveRecursively(true);
                    }
                    RefreshPet();
                }
                break;
            case RoleType.ROLE_PET4:
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_PET_PAGE, 4);
                    mSelfEquiptWindow.SetActiveRecursively(false);
                    if (!mPetEquiptWindow.active)
                    {
                        mPetEquiptWindow.SetActiveRecursively(true);
                    }
                    RefreshPet();
                }
                break;
            case RoleType.ROLE_PET5:
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_PET_PAGE, 5);
                    mSelfEquiptWindow.SetActiveRecursively(false);
                    if (!mPetEquiptWindow.active)
                    {
                        mPetEquiptWindow.SetActiveRecursively(true);
                    }
                    RefreshPet();
                }
                break;
            case RoleType.ROLE_PET6:
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_PET_PAGE, 6);
                    mSelfEquiptWindow.SetActiveRecursively(false);
                    if (!mPetEquiptWindow.active)
                    {
                        mPetEquiptWindow.SetActiveRecursively(true);
                    }
                    RefreshPet();
                }
                break;
            default:
                break;
        }
    }

    void OnOpenPetPeiYang()
    {
        if(peiyangWinGo == null)
        {
            peiyangWinGo = UIWindowMng.Instance.GetWindowGo("PeiYangWindow");
            if (peiyangWinGo == null)
                return;
        }
        bool toggle = !peiyangWinGo.active;
        if (toggle)
        {
            peiyangWinGo.transform.parent = gameObject.transform;
            Vector3 pos = Vector3.zero;
            pos.x = 78;
            peiyangWinGo.transform.localPosition = pos;
        }
        else
        {
            peiyangWinGo.transform.parent = null;
        }
        peiyangWinGo.SetActiveRecursively(toggle);
        //UIWindowMng.Instance.ShowWindow("PeiYangWindow");
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_ATTRIRANDOM,(int)mCurrentRoleType);
    }

    public UIProgressBar petexp_;
    public UIProgressBar petHP_;
    public ActionButton petHead_;//头
    public ActionButton petSpur_;//刺
    public ActionButton petClaw_;//爪
    public ActionButton petRing_; //兽环
    public ActionButton petBody_; //躯干
    public ActionButton petTattoo_;//纹身
    
    public UIButton petHeadLevelUp_;
    public UIButton petSpurLevelUp_;
    public UIButton petClawLevelUp_;
    public UIButton petRingLevelUp_;
    public UIButton petBodyLevelUp_;
    public UIButton petTattooLevelUp_;

    public SpriteText petLevelName_;
    public ActionButton petSkill1_;
    public ActionButton petSkill2_;
    public ActionButton petSkill3_;
    public ActionButton petSkill4_;
    public SpriteText petStrength_;//力量
    public SpriteText petWisdom_;//智慧
    public SpriteText petToughness_;//韧性
    public SpriteText petAgility_;//敏捷
    public SpriteText petConstituition_;//体格
    public SpriteText petPhysicAttk_;//物理攻击
    public SpriteText petMagicAttk_;//法术攻击
    public SpriteText petPhysicDef_;//物理防御
    public SpriteText petMagicDef_;//法术防御
    public SpriteText petName_;//宠物名称
    public SpriteText petBillBoard_;//排行榜
    public SpriteText petHitRate_;//命中
    public SpriteText petDuck_;//闪避
    public SpriteText petCritical_;//暴击
    public SpriteText petCriticalDef_;//抗暴
    public UIButton petTurnLeft_;
    public UIButton petTurnRight_;
    public UIButton fight_;

    void UpdateEquiptLevelUpButton(GAME_EVENT_ID eventID, List<string> vParam)
    {
        switch (eventID)
        {
            case GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED:
                RefreshLevelUpButton();
                break;
        }
    }

    void UpdatePetEquipt(GAME_EVENT_ID eventID, List<string> vParam)
    {
        switch (eventID)
        {
            case GAME_EVENT_ID.GE_UPDATE_PETEQUIP:
                {
                    RefreshPetEquipt();
                }
                break;
        }
    }

    void RefreshPetEquipt()
    {
        SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(ActivePet);//CDataPool.Instance.Pet_GetPet(this.ActivePet);
        if (curPet == null)
        {
            UpdatePetEquiptActionButton(null, petClaw_);
            UpdatePetEquiptActionButton(null, petHead_);
            UpdatePetEquiptActionButton(null, petSpur_);
            UpdatePetEquiptActionButton(null, petRing_);
            UpdatePetEquiptActionButton(null, petBody_);
            UpdatePetEquiptActionButton(null, petTattoo_);
        }
        else
        {
            UpdatePetEquiptActionButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_CLAW],petClaw_);
            UpdatePetEquiptActionButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_HORN], petHead_);
            UpdatePetEquiptActionButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_SPUR], petSpur_);
            UpdatePetEquiptActionButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_KNOCKER], petRing_);
            UpdatePetEquiptActionButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_TRUNK], petBody_);
            UpdatePetEquiptActionButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_VEINS], petTattoo_);
        }
    }

    void UpdatePetEquiptActionButton(CObject_Item item, ActionButton btn)
    {
        if (item != null)
        {
            btn.UpdateItem(item.GetID());
        }
        else
        {
            btn.UpdateItem(-1);
        }
    }

    void RefreshLevelUpButton()
    {
        SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(ActivePet);//CDataPool.Instance.Pet_GetPet((int)mCurrentRoleType - 1);
        if (curPet == null)
        {
            petHeadLevelUp_.Hide(true);
            petSpurLevelUp_.Hide(true);
            petClawLevelUp_.Hide(true);
            petRingLevelUp_.Hide(true);
            petBodyLevelUp_.Hide(true);
            petTattooLevelUp_.Hide(true);
        }
        else
        {
            HideLevelUpEquiptButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_CLAW],petClawLevelUp_);
            HideLevelUpEquiptButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_HORN], petHeadLevelUp_);
            HideLevelUpEquiptButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_SPUR], petSpurLevelUp_);
            HideLevelUpEquiptButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_KNOCKER], petRingLevelUp_);
            HideLevelUpEquiptButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_TRUNK], petBodyLevelUp_);
            HideLevelUpEquiptButton(curPet.Equipts[(int)PET_EQUIP.PEQUIP_VEINS], petTattooLevelUp_);
        }
    }

    void HideLevelUpEquiptButton(CObject_Item item, UIButton levelupButton)
    {
        bool hide = true;
        if (item != null)
        {
            hide = !CanLevelUpEquipt(item.GetIdTable());
        }
        levelupButton.Hide(hide);
    }

    bool CanLevelUpEquipt(int itemID)
    {
        int nPrescrId = LifeAbility.Instance.GetPrescrID(itemID);
        if (nPrescrId == -1)
        {
            return false;
        }
        bool result = true;
        int nCount = LifeAbility.Instance.GetPrescrStuffCount(nPrescrId);
        for (int i = 1; i < nCount; i++)
        {
            Stuff stuff = LifeAbility.Instance.GetPrescrStuff(i, nPrescrId);
            if (stuff.nID == -1)
            {
                continue;
            }
            int count = CDataPool.Instance.UserBag_CountItemByIDTable(stuff.nID);
            if (count < stuff.nNum)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    void OnPetEvent(GAME_EVENT_ID eventID, List<string> vParam)
    {
        switch (eventID)
        {
            case GAME_EVENT_ID.GE_TOGLE_PET_PAGE:
                {
                    if (vParam.Count != 0)
                    {
                        int index = Convert.ToInt32(vParam[0]);
                        int petCount = CDataPool.Instance.Pet_GetCount();
                        if (petCount <= 0)
                        {
                            SetActivePage(RoleType.ROLE_SELF);
                        }
                        else
                        {

                            SetActivePage((RoleType)index);
                        }
                    }
                }
                break;
            case GAME_EVENT_ID.GE_UPDATE_PET_PAGE:
                {
                    RefreshRolePageCaption();
                    int petCount = CDataPool.Instance.Pet_GetCount();
                    if (petCount <= 0 || petCount < ActivePage)
                    {
                        SetActivePage(RoleType.ROLE_SELF);
                    }
                    else
                    {
                        RefreshPet();
                    }
                }
                break;
            case GAME_EVENT_ID.GE_ACCELERATE_KEYSEND:
                {

                }
                break;
            case GAME_EVENT_ID.GE_UNIT_LEVEL:
                {

                }
                break;
        }
    }

    void RefreshPetState()
    {
        if (Interface.Pet.Instance.IsFighting((int)mCurrentRoleType -1))
        {
            fight_.Text = "休息";
        }
        else
        {
            fight_.Text = "出战";
        }
    }

    void SetPetState()
    {
        if (Interface.Pet.Instance.IsFighting((int)mCurrentRoleType - 1))
            Interface.Pet.Instance.Go_Relax((int)mCurrentRoleType - 1);
        else
            Interface.Pet.Instance.Go_Fight((int)mCurrentRoleType - 1);
    }

    void SetPetFree()
    {
        Interface.Pet.Instance.Go_Free((int)mCurrentRoleType - 1);
    }

    void RefreshPet()
    {
        RefreshLevelUpButton();
        RefreshPetEquipt();
        SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet((int)mCurrentRoleType - 1);//Pet_GetPet((int)mCurrentRoleType -1);
        if (curPet != null)
        {
            RefreshPetState();
            RefreshPetAttr(curPet);
            RefreshPetSkill((int)mCurrentRoleType -1);
        }
        else
        {
            LogManager.LogWarning("PetIndex is wrong,maybe some mistake occur " + mCurrentRoleType);
        }
    }

    void RefreshPetAttr(SDATA_PET curPet)
    {
        petName_.Text = curPet.Name + " " + "等级：" + curPet.Level;
        petHP_.Text = curPet.HP + "/" + curPet.HPMax;
        petHP_.Value = (float)curPet.HP / (float)curPet.HPMax;
        petStrength_.Text = curPet.AttrStr + " + " + UnityEngine.Color.green + curPet.AttrStrBring;
        petWisdom_.Text = curPet.AttrSpr + " + " + UnityEngine.Color.green + curPet.AttrSprBring;
        petConstituition_.Text = curPet.AttrCon + " + " + UnityEngine.Color.green + curPet.AttrConBring;
        petAgility_.Text = curPet.AttrDex + " + " + UnityEngine.Color.green + curPet.AttrDexBring;
        petToughness_.Text = curPet.AttrInt + " + " + UnityEngine.Color.green + curPet.AttrIntBring;
        petPhysicAttk_.Text = curPet.AttPhysics.ToString();
        petMagicAttk_.Text = curPet.AttMagic.ToString();
        petPhysicDef_.Text = curPet.DefPhysics.ToString();
        petMagicDef_.Text = curPet.DefMagic.ToString();
        petHitRate_.Text = curPet.Hit.ToString();
        petDuck_.Text = curPet.Miss.ToString();
        petCritical_.Text = curPet.Critical.ToString();
        petCriticalDef_.Text = curPet.DefCritical.ToString();
        _DBC_PET_LEVELUP curPetLevelup = CDataBaseSystem.Instance.GetDataBase<_DBC_PET_LEVELUP>((int)DataBaseStruct.DBC_PET_LEVELUP).Search_Index_EQU(curPet.Level);
        petexp_.Text = curPet.Exp.ToString() + "/" + curPetLevelup.nExp.ToString();
        petexp_.Value = (float)curPet.Exp / (float)curPetLevelup.nExp;
    }

    void ResetPetSkill()
    {
        Interface.Pet.Instance.ResetPetSkill((int)mCurrentRoleType - 1);
    }

    void RefreshPetSkill(int petIndex)
    {
        CActionItem curItem = CActionSystem.Instance.EnumPetAction(petIndex, 0);
        petSkill1_.UpdateItemFromAction(curItem);
        curItem = CActionSystem.Instance.EnumPetAction(petIndex, 1);
        petSkill2_.UpdateItemFromAction(curItem);
        curItem = CActionSystem.Instance.EnumPetAction(petIndex, 2);
        petSkill3_.UpdateItemFromAction(curItem);
        curItem = CActionSystem.Instance.EnumPetAction(petIndex, 3);
        petSkill4_.UpdateItemFromAction(curItem);
    }

    void OnMouseEvent(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                break;
            case POINTER_INFO.INPUT_EVENT.PRESS:
                {
                    if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
                    {
                        if ("PetLeftButton" == ptr.hitInfo.collider.gameObject.name)
                        {
                            beginRotate(false);
                        }
                        else if ("PetRightButton" == ptr.hitInfo.collider.gameObject.name)
                        {
                            beginRotate(true);
                        }
                        else if ("LeftButton" == ptr.hitInfo.collider.gameObject.name)
                        {
                            beginRotate(false);
                        }
                        else if ("RightButton" == ptr.hitInfo.collider.gameObject.name)
                        {
                            beginRotate(true);
                        }

                        LogManager.Log("POINTER_INFO.INPUT_EVENT.PRESS " + ptr.hitInfo.collider.gameObject.name);
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.TAP:
                {
                    if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
                    {
                        stopRotate();
                    }
                    else
                    {
                        LogManager.Log("POINTER_INFO.INPUT_EVENT.TAP");
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
                {
                    if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
                    {
                        LogManager.Log("POINTER_INFO.INPUT_EVENT.RELEASE " + ptr.hitInfo.collider.gameObject.name);
                    }
                    else
                    {
                        LogManager.Log("POINTER_INFO.INPUT_EVENT.RELEASE");
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                {
                    stopRotate();
                }
                break;
        }
    }

    void OpenLevelUpEquipt(RoleType role ,PET_EQUIP part)
    {
        mCurrentPetEquipt = part; 
        if (equiptWinGo == null)
        {
            equiptWinGo = UIWindowMng.Instance.GetWindowGo("EquipWindow");
            if (equiptWinGo == null)
                return;
        }
        List<string> Params = new List<string>();
        int temp = (int)(role);
        Params.Add(temp.ToString());
        temp = (int)(part);
        Params.Add(temp.ToString());
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_PETEQUIPLEVELUP, Params);
    }
    
    void OnMouseLevelUpEquipt(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.TAP:
            {
                if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
                {
                    if (ptr.hitInfo.collider.gameObject.name == "BodyLevelUp")
                    {
                        OpenLevelUpEquipt(mCurrentRoleType, PET_EQUIP.PEQUIP_KNOCKER);
                    }
                    else if (ptr.hitInfo.collider.gameObject.name == "ClawLevelUp")
                    {
                        OpenLevelUpEquipt(mCurrentRoleType, PET_EQUIP.PEQUIP_CLAW);
                    }
                    else if (ptr.hitInfo.collider.gameObject.name == "HeadLevelUp")
                    {
                        OpenLevelUpEquipt(mCurrentRoleType, PET_EQUIP.PEQUIP_HORN);
                    }
                    else if (ptr.hitInfo.collider.gameObject.name == "SpurLevelUp")
                    {
                        OpenLevelUpEquipt(mCurrentRoleType, PET_EQUIP.PEQUIP_SPUR);
                    }
                    else if (ptr.hitInfo.collider.gameObject.name == "TattooLevelUp")
                    {
                        OpenLevelUpEquipt(mCurrentRoleType, PET_EQUIP.PEQUIP_VEINS);
                    }
                    else if (ptr.hitInfo.collider.gameObject.name == "RingLevelUp")
                    {
                        OpenLevelUpEquipt(mCurrentRoleType, PET_EQUIP.PEQUIP_KNOCKER);
                    }
                }
            }
               break;
        }
    }
    

    void beginRotate(bool clockwise)
    {
        mRotateSpeed = clockwise ? 2.0f * -1.0f : 2.0f;
        mRotating = true;
    }

    public int ActivePet
    {
        get { return (int)mCurrentRoleType-1; }
    }

    public int ActivePage
    {
        get { return (int)mCurrentRoleType; }
    }

    void stopRotate()
    {
        mRotating = false;
    }

    void Update()
    {
        if (mRotating)
        {
            if (mCurrentRoleType == RoleType.ROLE_SELF)
            {
                CObject fakeObject = CObjectManager.Instance.getPlayerMySelf().getAvatar();
                if (fakeObject != null)
                {
                    float curDir = fakeObject.GetFaceDir() + GameProcedure.s_pTimeSystem.GetDeltaTime() / 1000.0f * mRotateSpeed;
                    fakeObject.SetFaceDir(curDir);
                }
            }
            else
            {
                SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(ActivePet);//CDataPool.Instance.Pet_GetPet((int)mCurrentRoleType - 1);
                if (curPet != null && curPet.FakeObject != null)
                {
                    float curDir = curPet.FakeObject.GetFaceDir() + GameProcedure.s_pTimeSystem.GetDeltaTime() / 1000.0f * mRotateSpeed;
                    curPet.FakeObject.SetFaceDir(curDir);
                }
            }
        }
    }
}
