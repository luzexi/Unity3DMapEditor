using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using Network;
using Network.Packets;
public class MainEquip : MonoBehaviour {

    public const int MAX_GEM = 4; 
    public GameObject EnchanceWindow;
	public GameObject ShengDangWindow;
	public GameObject EnchaseWindow;
	public GameObject CombineWindow;
	//public GameObject StoneContainer;

    public List<UIRadioBtn> mRoleMenus;
    public List<ActionButton> mEquips;
    public List<SpriteText> mEquipsText;
    public List<UIRadioBtn> mOperateMenus;
    public List<UIRadioBtn> mEquipMenus;
    public UIButton mPrePage;
    public UIButton mNextPage;
    public SpriteText mPageText;

    Vector3 mPacketMenuPos;

    //升档界面
    public ActionButton mSDSelectedAction;
    public SpriteText mSDSelectedAttriDesc;
    public SpriteText mSDSelectedAttriDesc2;
    public ActionButton mSDPreviewAction;
    public SpriteText mSDPreviewAttriDesc;
    public SpriteText mSDPreviewAttriDesc2;
    public List<ActionButton> mSDMaterials;
    public List<SpriteText> mSDMaterialTexts;
    public UIButton mSDOK;
    public UIButton mSDPrescrLink;
    bool mIsEnableSD = false;
    int mSearchPrescrItem = -1;

    //强化界面
    public ActionButton mEnchanceSelectedAction;
    public SpriteText mEnchanceSelectedAttri1;
    public SpriteText mEnchanceSelectedAttri2;
    public SpriteText mEnchanceResultAttri1;
    public SpriteText mEnchanceResultAttri2;
    public SpriteText mEnchanceMoney;
    public UIButton mEnchanceOK;
    bool mIsEnableEnchance = false;

    //宝石公用
    List<ActionButton> mGemContainer;
    Dictionary<string, int> mGemIndexs;
    SpriteText mGemNumText;
    int mGemBeginIndex = 0;
    const int MAX_GEMCONTAINER_COUNT = 18;
    UIButton mGemContainerPre;
    UIButton mGemContainerNext;
    SpriteText mGemContainerPage;

    //宝石合成
    public List<ActionButton> mCombineContainer;
    public SpriteText mCombineNumText;
    public List<ActionButton> mGemCombineMaterials;
    public SpriteText mCombineRatio;
    public UIButton mCombineOK;
    public UIButton mCombinePre;
    public UIButton mCombineNext;
    public SpriteText mCombineContainerPage;
    List<int> mGemCombineSelectedIDs = new List<int>();
    Dictionary<string, int> mCombineGemIndexs = new Dictionary<string, int>();
    Dictionary<string, int> mCombineGemMatsIndex = new Dictionary<string, int>();


    //宝石镶嵌
    public List<ActionButton> mEnchanseContainer;
    public SpriteText mEnchanseNumText;
    public List<ActionButton> mEnchanseGems;
    public SpriteText mEnchanseSelectItemName;
    public ActionButton mEnchanseSelectedAction;
    public UIButton mEnchansePre;
    public UIButton mEnchanseNext;
    public SpriteText mEnchanseContainerPage;
    List<int> mEnchanseSelectedIDs = new List<int>();
    Dictionary<string, int> mEnchanseGemIndexs = new Dictionary<string, int>();
    struct GemEnchanseInfo
    {
        public int nGemType;
        public byte bFlag; //0,不变；1，镶嵌；2，摘除
        public ActionButton enchanseGem;
    }
    const byte GEM_NONE = 0;//不变
    const byte GEM_ENCHANSE = 1;//镶嵌
    const byte GEM_TAKEOFF = 2;//摘除

    GemEnchanseInfo[] mEnchanseGemInfo = new GemEnchanseInfo[MAX_GEM];

    Dictionary<string, int> mRoleTypes = new Dictionary<string, int>();
    Dictionary<string, int> mOperateTypes = new Dictionary<string, int>();
    Dictionary<string, int> mEquipIndexs = new Dictionary<string, int>();

    List<CObject_Item> mLockedItem = new List<CObject_Item>();
    CObject_Item mSelectedItem = null;
    bool mbSelectedItemChanged = false;
    // 最大十件装备 [4/10/2012 SUN]
    const int MAX_EQUIPGRID = 10;
    int mCurrentEquipIndex = 0;//用于翻页
    int mMaxPage = 0;
    int mCurrentSelectedEquip = 0;//当前选择的装备索引

    RoleType mCurrentRoleType;
    //装备位置
    enum RoleType
    {
        ROLE_SELF = 0,
        ROLE_PET1,
        ROLE_PET2,
        ROLE_PET3,
        ROLE_PET4,
        ROLE_PET5,
        ROLE_PET6,
        ROLE_PACKET,
    }

    OperateType mCurrentOperate;
	//操作界面
	enum OperateType
	{
		ENCHANCE =0,
		SHENGDANG,
		ENCHASE,
		COMBINE,
	}

	
	void Awake()
	{
		gameObject.SetActiveRecursively(true);
			
		registerHandler();
        registerWidgets();
        petWidgetsShow();
        //动态加载，初始化
        mCurrentEquipIndex = 0;
        OnChangedRoleType(mCurrentRoleType);
        OnChangedOperator(mCurrentOperate);
		
		RegisterEnhanceHandle(OperateType.ENCHANCE);
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

        if (UIWindowMng.Instance.IsWindowShow("RoleTipWindow"))
        {
            GameObject selfEquipt = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
            UISelfEquip selfEquiptComponent = selfEquipt.GetComponent<UISelfEquip>();
            PET_EQUIP activePart = selfEquiptComponent.ActivePetEquiptPart;
            if (activePart != PET_EQUIP.PEQUIP_INVALID)
            {
                int roleIndex = selfEquiptComponent.ActivePage;
                List<string> Params = new List<string>();
                Params.Add(roleIndex.ToString());
                int temp = (int)(activePart);
                Params.Add(temp.ToString());
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_PETEQUIPLEVELUP, Params);
            }
        }
	}

    void registerWidgets()
    {
        for (int i = 0; i < mRoleMenus.Count; i++ )
        {
            mRoleTypes.Add(mRoleMenus[i].gameObject.name, i);
            mRoleMenus[i].SetValueChangedDelegate(RoleMenuValueChanged);
            if (mRoleMenus[i].defaultValue)
            {
                mCurrentRoleType = (RoleType)i;
            }
        }
        mPacketMenuPos = mRoleMenus[(int)RoleType.ROLE_PACKET].gameObject.transform.localPosition;
        for (int i = 0; i < mOperateMenus.Count; i++ )
        {
            mOperateTypes.Add(mOperateMenus[i].gameObject.name, i);
            mOperateMenus[i].SetValueChangedDelegate(OperateMenuValueChanged);
            if (mOperateMenus[i].defaultValue)
            {
                mCurrentOperate = (OperateType)i;
            }
        }
        for (int i = 0; i < mEquipMenus.Count; i++ )
        {
            mEquipIndexs.Add(mEquipMenus[i].gameObject.name, i);
            mEquipMenus[i].SetValueChangedDelegate(OnEquipSelectedChanged);
            if(mEquipMenus[i].defaultValue)
                mCurrentSelectedEquip = i;
            
        }
        foreach (ActionButton equip in mEquips)
        {
            equip.AddInputDelegate(OnEquipActionButtonClicked);
            equip.EnableDoAction = false;
            equip.DisableDrag();
        }
        mSDSelectedAction.DisableDrag();
        mSDPreviewAction.DisableDrag();
        foreach (ActionButton action in mSDMaterials)
        {
            action.DisableDrag();
        }
        mEnchanceSelectedAction.DisableDrag();

        int index = 0;
        foreach (ActionButton gemAction in mCombineContainer)
        {
            gemAction.AddInputDelegate(OnGemActionClicked);
            gemAction.DisableDrag();
            gemAction.EnableDoAction = false;
            mCombineGemIndexs.Add(gemAction.Name(), index++);
        }
        index = 0;
        foreach (ActionButton matAction in mGemCombineMaterials)
        {
            matAction.DisableDrag();
            matAction.AddInputDelegate(OnGemCombineMatRelease);
            mCombineGemMatsIndex.Add(matAction.Name(), index++);
            matAction.EnableDoAction = false;
        }

        //宝石镶嵌
        for (int i = 0; i < mEnchanseContainer.Count; i++ )
        {
            mEnchanseGemIndexs.Add(mEnchanseContainer[i].Name(), i);
            mEnchanseContainer[i].AddInputDelegate(OnGemEnchanseActionClicked);
            mEnchanseContainer[i].DisableDrag();
            mEnchanseContainer[i].EnableDoAction = false;
        }
        index = 0;
        foreach (ActionButton gemAction in mEnchanseGems)
        {
            gemAction.DisableDrag();
            gemAction.EnableDoAction = false;
            gemAction.AddInputDelegate(OnEnchanseTakeOff);
            mEnchanseGemInfo[index++].enchanseGem = gemAction;
        }
    }
	
	void registerHandler()
	{
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_EQUIPWINDOW, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_EQUIP, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_PET_PAGE, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_PETEQUIP, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_PETEQUIPLEVELUP, OnEvent);
	}
    void OperateMenuValueChanged(IUIObject obj)
    {
        UIRadioBtn radio = obj as UIRadioBtn;
        if (radio != null && radio.Value)
        {
            if (mOperateTypes.ContainsKey(radio.gameObject.name))
            {
                mCurrentOperate = (OperateType)(mOperateTypes[radio.gameObject.name]);
                OnChangedOperator(mCurrentOperate);
                SelectedEquipChanged(mCurrentSelectedEquip);
            }
        }
    }
    void OnChangedOperator(OperateType ot)
    {
        widgetsShow(ot);
        //SelectedEquipChanged(mCurrentSelectedEquip);
    }
    void UpdateOperatorContext(OperateType ot)
    {
        if (ot == OperateType.SHENGDANG)
        {
            UpdateShengDang();
        }
        else if (ot == OperateType.ENCHANCE)
        {
            UpdateEnchance();
        }
        else if (ot == OperateType.ENCHASE)
        {
            UpdateEnchase();
        }
        else if (ot == OperateType.COMBINE)
        {
            UpdateCombine();
        }
    }
	
	void RegisterEnhanceHandle(OperateType ot)
	{
		if(ot == OperateType.ENCHANCE)
		{
			if(gameObject.active)
			{
				CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGGLE_EQUIP_ENHANCE);
			}
		}
	}
	
    void petWidgetsShow()
    {
        bool bChanged = false;
        int nCount = CDataPool.Instance.Pet_GetCount();
        //for (int i = (int)RoleType.ROLE_PET1; i <= (int)RoleType.ROLE_PET6; i++ )
        //{
            //if (i < nCount + (int)RoleType.ROLE_PET1)
            //{
            //    mRoleMenus[i].Hide(false);
            //    SDATA_PET pet = CDataPool.Instance.Pet_GetPet(i - 1);
            //    mRoleMenus[i].Text = pet.Name;
            //}
            //else
            //{
            //    if ((int)mCurrentRoleType == i)
            //        bChanged = true;
            //    mRoleMenus[i].Hide(true);
            //}
       // }
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

        int nPacketIndex = nCount + 1;
        if (nPacketIndex > (int)RoleType.ROLE_PET6)
            mRoleMenus[(int)RoleType.ROLE_PACKET].gameObject.transform.localPosition = mPacketMenuPos;
        else
            mRoleMenus[(int)RoleType.ROLE_PACKET].gameObject.transform.localPosition = mRoleMenus[nPacketIndex].gameObject.transform.localPosition;

        if (bChanged)
        {
            mCurrentRoleType = RoleType.ROLE_SELF;
            OnChangedRoleType(mCurrentRoleType);
        }
    }
    void widgetsShow(OperateType ot)
    {
        bool bShengDang = false;//升档
        bool bEnchance = false;//强化
        bool bCombine = false; //合成
        bool bEnchase = false; //镶嵌
        if (ot == OperateType.SHENGDANG)
        {
            bShengDang = true;
        }
        else if (ot == OperateType.ENCHANCE)
        {
            bEnchance = true;
        }
        else if (ot == OperateType.COMBINE)
        {
            bCombine = true;
        }
        else if (ot == OperateType.ENCHASE)
        {
            bEnchase = true;
        }
        ShengDangWindow.SetActiveRecursively(bShengDang);

        EnchanceWindow.SetActiveRecursively(bEnchance);
        EnchaseWindow.SetActiveRecursively(bEnchase);
        CombineWindow.SetActiveRecursively(bCombine);
		
    }
	
    /// <summary>
    /// 升档
    /// </summary>
    void UpdateShengDang()
    {
        ClearShengDangContext();
        mIsEnableSD = false;
        if (mSelectedItem != null)
        {
            mSDSelectedAction.SetActionItem(mSelectedItem.GetID());
            string text = mSelectedItem.GetWhiteAttribute(0);
            mSDSelectedAttriDesc.Text = UIString.Instance.ParserString_Runtime(text);
            text = mSelectedItem.GetWhiteAttribute(1);
            if(text == "")
            {
                mSDSelectedAttriDesc2.Hide(true);
            }
            else
            {
                mSDSelectedAttriDesc2.Hide(false);
                mSDSelectedAttriDesc2.Text = UIString.Instance.ParserString_Runtime(text);
            }
            

            CActionItem actionItem = LifeAbility.Instance.GetEquipUpdatePreView(mSDSelectedAction.CurrActionItem.GetID());
            if (actionItem != null)
            {
                mSDPreviewAction.SetActionItemByActionId(actionItem.GetID());

                CObject_Item previewItem = mSDPreviewAction.CurrActionItem.GetImpl() as CObject_Item;
                if (previewItem != null)
                {
                    text = previewItem.GetWhiteAttribute(0);
                    mSDPreviewAttriDesc.Text = UIString.Instance.ParserString_Runtime(text);
                    text = previewItem.GetWhiteAttribute(1);
                    if (text == "")
                        mSDPreviewAttriDesc2.Hide(true);
                    else
                    {
                        mSDPreviewAttriDesc2.Text = UIString.Instance.ParserString_Runtime(text);
                        mSDPreviewAttriDesc2.Hide(false);
                    }
                }            
            }

            int nPrescrId = LifeAbility.Instance.GetPrescrID(mSelectedItem.GetIdTable());
            if (nPrescrId == -1)
            {
                NoPrescrHandler(mSelectedItem.GetIdTable());
                mSDOK.controlIsEnabled = false;
                return;
            }
            NoPrescrHandler(-1);
            //材料
            int nCount = LifeAbility.Instance.GetPrescrStuffCount(nPrescrId);
            //int nMaterial = -1;
            for (int i = 1; i < nCount; i++ )//第一个材料为当前装备，跳过不显示
            {
                Stuff stuff = LifeAbility.Instance.GetPrescrStuff(i, nPrescrId);
                if (stuff.nID == -1)
                {
                    mSDMaterials[i - 1].SetActionItem(-1);
                    mSDMaterialTexts[i - 1].Text = "";
                    continue;
                }
                bool bCreateNew = true;
                if (mSDMaterials[i - 1].CurrActionItem != null)
                {
                    if (mSDMaterials[i - 1].CurrActionItem.GetIDTable() == stuff.nID)
                        bCreateNew = false;
                    else
                        mSDMaterials[i - 1].CurrActionItem.DestroyImpl();
                }
                if (bCreateNew)
                {
                    CObject_Item pItemObj1 = ObjectSystem.Instance.NewItem((uint)stuff.nID);
                    CActionItem_Item actionItem1 = CActionSystem.Instance.GetAction_ItemID(pItemObj1.GetID(), false);

                    mSDMaterials[i - 1].SetActionItemByActionId(actionItem1.GetID());
                }

                int count = CDataPool.Instance.UserBag_CountItemByIDTable(stuff.nID);
                if (count >= stuff.nNum)
                {
                    count = stuff.nNum;
                    mIsEnableSD = true;
                }
                else
                    mIsEnableSD = false;
                mSDMaterialTexts[i - 1].Text = count + "/" + stuff.nNum;
            }

            mSDOK.controlIsEnabled = mIsEnableSD;
        }

    }
    void ClearShengDangContext()
    {
        mSDSelectedAction.SetActionItem(-1);
        mSDSelectedAttriDesc.Text = "";
        mSDSelectedAttriDesc2.Text = "";
        foreach (ActionButton action in mSDMaterials)
        {
            action.SetActionItem(-1);
        }
        foreach (SpriteText text in mSDMaterialTexts)
        {
            text.Text = "";
        }
        mSDPreviewAction.SetActionItem(-1);
        mSDPreviewAttriDesc.Text = "";
        mSDPreviewAttriDesc2.Text = "";

    }
    void NoPrescrHandler(int nTableId)
    {
        if (nTableId != -1)
            mSDPrescrLink.Hide(false);
        else
            mSDPrescrLink.Hide(true);
        mSearchPrescrItem = nTableId;
    }

    /// <summary>
    /// 强化
    /// </summary>
    void UpdateEnchance()
    {
        ClearEnchanceContext();
        mIsEnableEnchance = false;
        if (mSelectedItem != null)
        {
            //属性
            mEnchanceSelectedAction.SetActionItem(mSelectedItem.GetID());
            string text = mSelectedItem.GetWhiteAttribute(0);
            mEnchanceSelectedAttri1.Text = UIString.Instance.ParserString_Runtime(text);
            text = mSelectedItem.GetWhiteAttribute(1);
            if(text == "")
                mEnchanceSelectedAttri2.Hide(true);
            else
            {
                mEnchanceSelectedAttri2.Hide(false);
                mEnchanceSelectedAttri2.Text = UIString.Instance.ParserString_Runtime(text);
            }

            //效果
            int nEnchanceLevel = mSelectedItem.GetStrengthLevel() + 1;
            CObject_Item_Equip equip = mSelectedItem as CObject_Item_Equip;
            if(equip != null)
            {
                text = equip.GetStrengthAttributeValue(nEnchanceLevel, 0);
                mEnchanceResultAttri1.Text = UIString.Instance.ParserString_Runtime(text);
                text = equip.GetStrengthAttributeValue(nEnchanceLevel, 1);
                if(text == "")
                    mEnchanceResultAttri2.Hide(true);
                else
                {  
                    mEnchanceResultAttri2.Hide(false);
                    mEnchanceResultAttri2.Text = UIString.Instance.ParserString_Runtime(text);
                 }
            }
            Item_Enhance itemEnhance = LifeAbility.Instance.Get_Equip_EnhanceLevel(mSelectedItem);
            mEnchanceMoney.Text = itemEnhance.needMoney + "    金钱";

            //是否可强化
            if (mSelectedItem.GetStrengthLevel() < CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level()
                && itemEnhance.needMoney <= CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money())
            {
                mEnchanceOK.controlIsEnabled = true;
            }
        }

    }
    void ClearEnchanceContext()
    {
        mEnchanceSelectedAction.SetActionItem(-1);
        mEnchanceSelectedAttri1.Text = "";
        mEnchanceSelectedAttri2.Text = "";
        mEnchanceResultAttri1.Text = "";
        mEnchanceResultAttri2.Text = "";
        mEnchanceMoney.Text = "";

        mEnchanceOK.controlIsEnabled = false;
    }
    void ClearGemContainer()
    {
        foreach (ActionButton gemAction in mGemContainer)
        {
            gemAction.SetActionItem(-1);
        }
        mGemNumText.Text = "";
    }
    void UpdateGemContainer()
    {
        ClearGemContainer();
        int nIndex = 0;
        for (int i = GAMEDEFINE.MATERIAL_BAG_BEGIN + mGemBeginIndex; i < GAMEDEFINE.MATERIAL_BAG_END; i++)
        {
            CObject_Item_Gem item = CDataPool.Instance.UserBag_GetItem(i) as CObject_Item_Gem;

            if (item != null )
            {
                if(nIndex < MAX_GEMCONTAINER_COUNT)
                    mGemContainer[nIndex].SetActionItem(item.GetID());
       
                nIndex++;
            }

        }
        nIndex += mGemBeginIndex;
        //more than one page
        if (nIndex >= MAX_GEMCONTAINER_COUNT)
        {
            mGemContainerPre.Hide(false);
            mGemContainerNext.Hide(false);

            mGemContainerPre.controlIsEnabled = mGemBeginIndex > 0 ? true : false;
            bool benable = nIndex - mGemBeginIndex > MAX_GEMCONTAINER_COUNT;
            mGemContainerNext.controlIsEnabled = benable;
        }
        else
        {
            mGemContainerPre.Hide(true);
            mGemContainerNext.Hide(true);
        }
        int curpage = mGemBeginIndex / MAX_GEMCONTAINER_COUNT + 1;

        int totalpage = nIndex / MAX_GEMCONTAINER_COUNT;
        if (nIndex % MAX_GEMCONTAINER_COUNT != 0)
            totalpage++;
        mGemContainerPage.Text = curpage + "/" + totalpage;

        mGemNumText.Text = "";//nIndex + "/" + mGemContainer.Count;

        
    }
    
    void ClearGemCombineMaterials()
    {
        foreach (ActionButton gemMat in mGemCombineMaterials)
        {
            gemMat.SetActionItem(-1);
        }
        mCombineRatio.Text = "";
    }
    void ClearInvalidGemId()
    {
        List<int>.Enumerator enumerator = mGemCombineSelectedIDs.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int id = enumerator.Current;
            if (CDataPool.Instance.UserBag_GetItemById(id) == null)
            {
                mGemCombineSelectedIDs.Remove(enumerator.Current);
                enumerator = mGemCombineSelectedIDs.GetEnumerator();
            }
        }
    }
    //更新宝石合成材料格
    void UpdateGemMaterials()
    {
        ClearGemCombineMaterials();
        int nIndex = 0;
        foreach (int Id in mGemCombineSelectedIDs)
        {
            mGemCombineMaterials[nIndex].SetActionItem(Id);
            nIndex++;
        }
        if (nIndex >= 3)
        {
            mCombineRatio.Text = 25 * (nIndex - 1) + "%";

            mCombineOK.controlIsEnabled = true;
        }
        else
        {
            mCombineOK.controlIsEnabled = false;
        }
    }
    bool CheckGem(CObject_Item gem)
    {
        bool bSuc = true;
        foreach (int id in mGemCombineSelectedIDs)
        {
            CObject_Item matItem = CDataPool.Instance.UserBag_GetItemById(id);
            if (matItem != null)
            {
                if (matItem.GetIdTable() != gem.GetIdTable())
                {
                    bSuc = false;
                    break;
                }
            }
        }
        return bSuc;
    }
    void AddGemCombineMat(int nIndex)
    {
        if (mGemContainer[nIndex].CurrActionItem != null)
        {
            CObject_Item item = mGemContainer[nIndex].CurrActionItem.GetImpl() as CObject_Item;
            if(item != null)
            {
                //检测材料合法性
                //todo
                if (!CheckGem(item))
                    return;
                if (!mGemCombineSelectedIDs.Contains(item.GetID()))
                {
                    mGemCombineSelectedIDs.Add(item.GetID());
                    item.isLocked = true;
                    UpdateGemMaterials();
                }
            }
        }
    }
    void UpdateEnchanseGemStates()
    {
        for (int i = 0; i < mEnchanseGemInfo.Length; i++ )
        {
            if (mEnchanseGemInfo[i].bFlag == 2)
            {
                mEnchanseGemInfo[i].enchanseGem.SetMaskTexture("maskout");
            }
            else if (mEnchanseGemInfo[i].bFlag == 1)
            {
                //todo add
                if (mEnchanseGemInfo[i].nGemType != -1)
                {
                    CObject_Item gemItem = CDataPool.Instance.UserBag_GetItemById(mEnchanseGemInfo[i].nGemType);
                    CObject_Item newGemItem = ObjectSystem.Instance.NewItem((uint)gemItem.GetIdTable());
                    CActionItem action = CActionSystem.Instance.GetAction_ItemID(newGemItem.GetID(), false);
                    mEnchanseGemInfo[i].enchanseGem.SetActionItemByActionId(action.GetID());
                    mEnchanseGemInfo[i].enchanseGem.SetMaskTexture("maskin");
                }
            }
        }
    }
    void OnGemActionClicked(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            string parentName = ptr.targetObj.gameObject.name;
            if (mGemIndexs.ContainsKey(parentName))
            {
                int nIndex = mGemIndexs[parentName];
                AddGemCombineMat(nIndex);

            }
        }
    }
    void ClearAllGemMat()
    {
        foreach (int nId in mGemCombineSelectedIDs)
        {
            CObject_Item gem = CDataPool.Instance.UserBag_GetItemById(nId);
            if (gem != null)
                gem.isLocked = false;
        }
        mGemCombineSelectedIDs.Clear();
    }
    void OnGemCombineMatRelease(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            string parentName = ptr.targetObj.gameObject.name;
            if (mCombineGemMatsIndex.ContainsKey(parentName))
            {
                int nIndex = mCombineGemMatsIndex[parentName];
                int nId = mGemCombineSelectedIDs[nIndex];
                CObject_Item gemItem = CDataPool.Instance.UserBag_GetItemById(nId);
                if (gemItem != null)
                    gemItem.isLocked = false ;
                mGemCombineSelectedIDs.RemoveAt(nIndex);
                UpdateGemMaterials();
            }
        }
    }
    void OnGemEnchanseActionClicked(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            string parentName = ptr.targetObj.gameObject.name;
            if (mGemIndexs.ContainsKey(parentName))
            {
                int nIndex = mGemIndexs[parentName];
                CObject_Item_Equip equip = mSelectedItem as CObject_Item_Equip;
                if (equip != null)
                {
                    //if (equip.GetCurrentGemCount() >= equip.GetGemMaxCount())
                    //    return;
                    CObject_Item_Gem gem = mGemContainer[nIndex].CurrActionItem.GetImpl() as CObject_Item_Gem;
                    if (gem != null)
                    {
 
                        if(InsertGem(gem, equip))
                            gem.isLocked = true;
                        UpdateEnchanseGemStates();
                    }
                }

            }
        }
    }
    bool InsertGem(CObject_Item_Gem gem, CObject_Item_Equip equip)
    {
        for (int i = 0; i < GAMEDEFINE.MAX_ITEM_GEM; i++ )
        {
            if (mEnchanseGemInfo[i].bFlag == 1 )
            {
                CObject_Item_Gem gemold = CDataPool.Instance.UserBag_GetItemById(mEnchanseGemInfo[i].nGemType) as CObject_Item_Gem;
                if (gemold != null && gem.GetItemTableType() == gemold.GetItemTableType())
                    return false;
            }
            if (mEnchanseGemInfo[i].bFlag == 0)
            {
                //当前没有宝石
                if (equip.GetGemTableId(i) == 0)
                {
                    byte oldFlag = 2;
                    int index = equip.IsGemmy(gem.GetItemTableType());
                    if (index >=0)
                        oldFlag = mEnchanseGemInfo[index].bFlag;
                    if (oldFlag == 2)
                    {
          
                        mEnchanseGemInfo[i].nGemType = gem.GetID();
                        mEnchanseGemInfo[i].bFlag = 1;

                        return true;
                    }
                }
            }
           
        }
        return false;
    }
    void OnEnchanseTakeOff(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            string name = ptr.targetObj.name;
            string index = name.Remove(0, 2);
            int nIndex = int.Parse(index) - 1;
            if (nIndex >= 0 && nIndex < mEnchanseGemInfo.Length)
            {
                if (mEnchanseGemInfo[nIndex].bFlag == GEM_ENCHANSE)
                {

                    CObject_Item gem = CDataPool.Instance.UserBag_GetItemById(mEnchanseGemInfo[nIndex].nGemType);
                    if (gem != null)
                        gem.isLocked = false;

                    // 直接清除显示 [5/3/2012 SUN]
                    ActionButton gemAction = mEnchanseGemInfo[nIndex].enchanseGem;
                    if (gemAction.CurrActionItem != null)
                    {
                        gemAction.CurrActionItem.DestroyImpl();
                    }
                    gemAction.SetActionItem(-1);
                    gemAction.SetMaskTexture("");

                    mEnchanseGemInfo[nIndex].bFlag = GEM_NONE;
                    mEnchanseGemInfo[nIndex].nGemType = -1;
                }
                else if(mEnchanseGemInfo[nIndex].enchanseGem.CurrActionItem != null)
                {
                    mEnchanseGemInfo[nIndex].bFlag = GEM_TAKEOFF;
                    mEnchanseGemInfo[nIndex].nGemType = -1;
                }

                UpdateEnchanseGemStates();
            }

            //todo update takeoff selected
        }
    }
    void ClearEnchanseMat()
    {
        for (int i = 0; i < mEnchanseGemInfo.Length; i++ )
        {
            if (mEnchanseGemInfo[i].bFlag == GEM_ENCHANSE)
            {
                CObject_Item gem = CDataPool.Instance.UserBag_GetItemById(mEnchanseGemInfo[i].nGemType);
                if (gem != null)
                    gem.isLocked = false;
            }
        }
    }
    void ClearEnchanseSelected()
    {
        mEnchanseSelectedAction.SetActionItem(-1);
        mEnchanseSelectItemName.Text = "";

        for (int i = 0; i < mEnchanseGemInfo.Length; i++ )
        {
            ActionButton gemAction = mEnchanseGemInfo[i].enchanseGem;
            if (gemAction.CurrActionItem != null)
            {
                gemAction.CurrActionItem.DestroyImpl();
            }
            gemAction.SetActionItem(-1);
            gemAction.SetMaskTexture("");

            if (mEnchanseGemInfo[i].bFlag == 1)
            {

                CObject_Item gem = CDataPool.Instance.UserBag_GetItemById(mEnchanseGemInfo[i].nGemType);
                // 需要保留部分数据 [4/25/2012 SUN]
                if (gem != null && mbSelectedItemChanged)
                {
                    gem.isLocked = false;
                    mEnchanseGemInfo[i].nGemType = -1;
                    mEnchanseGemInfo[i].bFlag = 0;
                }
                else if (gem == null)
                {
                    mEnchanseGemInfo[i].nGemType = -1;
                    mEnchanseGemInfo[i].bFlag = 0;
                }
            }
            else if (mEnchanseGemInfo[i].bFlag == 2)
            {
                if (mbSelectedItemChanged)
                {
                    mEnchanseGemInfo[i].nGemType = -1;
                    mEnchanseGemInfo[i].bFlag = 0;
                }
            }

            
        }
       
    }
    void UpdateEnchanseSelectedEquip()
    {
        ClearEnchanseSelected();
        if (mSelectedItem != null)
        {
            mEnchanseSelectedAction.SetActionItem(mSelectedItem.GetID());

            CObject_Item_Equip equip = mSelectedItem as CObject_Item_Equip;
            if (equip != null)
            {
                int gemCount = equip.GetGemCount();
                for (int i = 0; i < gemCount; i++)
                {
                    int nIdTable = equip.GetGemTableId(i);
                    if (nIdTable != 0)
                    {
                        CObject_Item gemItem = ObjectSystem.Instance.NewItem((uint)nIdTable);
                        CActionItem action = CActionSystem.Instance.GetAction_ItemID(gemItem.GetID(), false);
                        mEnchanseGemInfo[i].enchanseGem.SetActionItemByActionId(action.GetID());
                    }
                }
            }
        }
        UpdateEnchanseGemStates();
    }
    //宝石镶嵌
    void UpdateEnchase()
    {
        mGemContainer = mEnchanseContainer;
        mGemNumText = mEnchanseNumText;
        mGemIndexs = mEnchanseGemIndexs;
        mGemContainerPre = mEnchansePre;
        mGemContainerNext = mEnchanseNext;
        mGemContainerPage = mEnchanseContainerPage;

        UpdateGemContainer();
        UpdateEnchanseSelectedEquip();
    }
    //宝石合成
    void UpdateCombine()
    {
        mGemContainer = mCombineContainer;
        mGemNumText = mCombineNumText;
        mGemIndexs = mCombineGemIndexs;
        mGemContainerPre = mCombinePre;
        mGemContainerNext = mCombineNext;
        mGemContainerPage = mCombineContainerPage;

        UpdateGemContainer();
        //刷新合成材料
        ClearInvalidGemId();
        UpdateGemMaterials();
    }

    void RoleMenuValueChanged(IUIObject obj)
    {
        UIRadioBtn radio = obj as UIRadioBtn;
        if (radio != null && radio.Value)
        {
            if (mRoleTypes.ContainsKey(radio.gameObject.name))
            {
                mCurrentRoleType = (RoleType)(mRoleTypes[radio.gameObject.name]);
                OnChangedRoleType(mCurrentRoleType);
            }
        }
    }
    void OnChangedRoleType(RoleType rt)
    {
        Clear();
        bool bResetSelectIndex = true;
        switch (rt)
        {
            case RoleType.ROLE_SELF:
                {
                    // 装备点的最大个数.
                    int nBeginIndex = mCurrentEquipIndex * MAX_EQUIPGRID;
                    int nCount = 0; //总装备数量
                    for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
                    {
                        if (i == (int)HUMAN_EQUIP.HEQUIP_RIDER)
                            continue;
                        //取得玩家身上的装备
                        CObject_Item pItem = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)i);
                        if (pItem != null)
                        {

                            if (nCount < nBeginIndex || nCount - nBeginIndex >= MAX_EQUIPGRID)
                            {
                                nCount++;
                                continue;
                            }
                            //不重置当前选择的索引
                            if (nCount - nBeginIndex >= mCurrentSelectedEquip)
                                bResetSelectIndex = false;

                            mEquips[nCount-nBeginIndex].SetActionItem(pItem.GetID());
                            mEquipsText[nCount-nBeginIndex].Text = pItem.GetName() + "\n" + "强化 " + pItem.GetStrengthLevel() + "级";
                            
                            nCount++;
                        }
                    }
                    mMaxPage = nCount / MAX_EQUIPGRID;
                    if (nCount % MAX_EQUIPGRID != 0)
                        mMaxPage++;
                    UpatePageNum();
                }
                break;
            case RoleType.ROLE_PACKET:
                {
                    int nCount = 0;
                    int nBeginIndex = mCurrentEquipIndex * MAX_EQUIPGRID;
                    for (int i = GAMEDEFINE.EQUIP_BAG_BEGIN; i < GAMEDEFINE.EQUIP_BAG_END; i++)
                    {
      
                        CObject_Item_Equip item = CDataPool.Instance.UserBag_GetItem(i) as CObject_Item_Equip;

                        if (item != null && item.GetItemType() != HUMAN_EQUIP.HEQUIP_RIDER)
                        {
                            if (nCount < nBeginIndex || nCount - nBeginIndex >= MAX_EQUIPGRID)
                            {
                                nCount++;
                                continue;
                            }
                            //不重置当前选择的索引
                            if (nCount - nBeginIndex >= mCurrentSelectedEquip)
                                bResetSelectIndex = false;

                            mEquips[nCount-nBeginIndex].SetActionItem(item.GetID());
                            mEquipsText[nCount-nBeginIndex].Text = item.GetName() + "\n" + "强化 " + item.GetStrengthLevel() + "级";

                            nCount++;
                        }
                        
                    }
                    mMaxPage = nCount / MAX_EQUIPGRID;
                    if (nCount % MAX_EQUIPGRID != 0)
                        mMaxPage++;
                    UpatePageNum();
                }
                break;
            case RoleType.ROLE_PET1:
                {
                    RefreshRolePetPage(0, ref bResetSelectIndex);
                }
                break;
            case RoleType.ROLE_PET2:
                {
                    RefreshRolePetPage(1, ref bResetSelectIndex);
                }
                break;
            case RoleType.ROLE_PET3:
                {
                    RefreshRolePetPage(2, ref bResetSelectIndex);
                }
                break;
            case RoleType.ROLE_PET4:
                {
                    RefreshRolePetPage(3, ref bResetSelectIndex);
                }
                break;
            case RoleType.ROLE_PET5:
                {
                    RefreshRolePetPage(4, ref bResetSelectIndex);
                }
                break;
            case RoleType.ROLE_PET6:
                {
                    RefreshRolePetPage(5, ref bResetSelectIndex);
                }
                break;
            default:
                break;
        }

        //切换回归第一件装备
        if (bResetSelectIndex)
        {
            mCurrentSelectedEquip = 0;
            mEquipMenus[mCurrentSelectedEquip].Value = true;
        }
        SelectedEquipChanged(mCurrentSelectedEquip);
    }

    void RefreshRolePetPage(int petIndex, ref bool bResetSelectIndex)
    {
        int nBeginIndex = mCurrentEquipIndex * MAX_EQUIPGRID;
        int nCount = 0; //总装备数量
        SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(petIndex);//CDataPool.Instance.Pet_GetPet(petIndex);
        if (curPet != null)
        {
            for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
            {
                CObject_Item curItem = CDataPool.Instance.UserPetEquipt_GetItem(curPet.GUID, (PET_EQUIP)i);
                if (curItem != null)
                {
                    if (nCount < nBeginIndex || nCount - nBeginIndex >= MAX_EQUIPGRID)
                    {
                        nCount++;
                        continue;
                    }
                    //不重置当前选择的索引
                    if (nCount - nBeginIndex >= mCurrentSelectedEquip)
                        bResetSelectIndex = false;

                    mEquips[nCount - nBeginIndex].SetActionItem(curItem.GetID());
                    mEquipsText[nCount - nBeginIndex].Text = curItem.GetName() + "\n" + "强化 " + curItem.GetStrengthLevel() + "级";
                    nCount++;
                }
            }
        }
        mMaxPage = nCount / MAX_EQUIPGRID;
        if (nCount % MAX_EQUIPGRID != 0)
            mMaxPage++;
        UpatePageNum();
    }

    public void OnEquipSelectedChanged(IUIObject obj)
    {
        UIRadioBtn radio = obj as UIRadioBtn;
        if (radio != null && radio.Value)
        {
            if (mEquipIndexs.ContainsKey(radio.gameObject.name))
            {
                int nSelected = mEquipIndexs[radio.gameObject.name];
                if (nSelected != mCurrentSelectedEquip)
                {
                    mCurrentSelectedEquip = nSelected;
                    SelectedEquipChanged(mCurrentSelectedEquip);
                }
            }
        }
    }
    public void OnEquipActionButtonClicked(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            Transform parent = ptr.targetObj.gameObject.GetComponent<Transform>().parent;
            string parentName = parent.gameObject.name;
            if (mEquipIndexs.ContainsKey(parentName))
            {
                int nIndex = mEquipIndexs[parentName];
                mEquipMenus[nIndex].Value = true;

            }
        }
    }
    void SelectedEquipChanged(int nIndex)
    {
        if (nIndex < 0 )
            return;
        if (nIndex < mEquips.Count && mEquips[nIndex].CurrActionItem != null)
        {
            CObject_Item newSelectItem = mEquips[nIndex].CurrActionItem.GetImpl() as CObject_Item;
            if (newSelectItem != mSelectedItem)
                mbSelectedItemChanged = true;
            else
                mbSelectedItemChanged = false;
            if (mbSelectedItemChanged)
            {
                if (mSelectedItem != null)
                    mSelectedItem.isLocked = false;
                mSelectedItem = newSelectItem;
                mSelectedItem.isLocked = true;
            }
        }
        else
        {
            if (mSelectedItem != null)
                mSelectedItem.isLocked = false;

            mSelectedItem = null;
        }
        UpdateOperatorContext(mCurrentOperate);
    }
    void OnChangePageNext()
    {
        mCurrentEquipIndex++;
        OnChangedRoleType(mCurrentRoleType);
        OnChangedOperator(mCurrentOperate);
    }
    void OnChangePagePre()
    {
        mCurrentEquipIndex--;
        OnChangedRoleType(mCurrentRoleType);
        OnChangedOperator(mCurrentOperate);
    }
    void UpatePageNum()
    {
        if (mMaxPage > mCurrentEquipIndex + 1)
            mNextPage.controlIsEnabled = true;
        else
            mNextPage.controlIsEnabled = false;
        if (mCurrentEquipIndex > 0)
            mPrePage.controlIsEnabled = true;
        else
            mPrePage.controlIsEnabled = false;
        int npage = mCurrentEquipIndex + 1;

        mPageText.Text = npage + "/" + mMaxPage;

    }
    void Clear()
    {
        for (int i = 0; i < MAX_EQUIPGRID; i++)
        {
            mEquips[i].SetActionItem(-1);
            mEquipsText[i].Text = "";
        }
    }
	public void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_TOGGLE_EQUIPWINDOW)
		{
            if (!gameObject.active)
            {
                petWidgetsShow();
                UIWindowMng.Instance.ShowWindow("EquipWindow");
                mCurrentEquipIndex = 0;
                OnChangedRoleType(mCurrentRoleType);
                OnChangedOperator(mCurrentOperate);

            }
            else
                CloseBtnClick();
		}
        int flag = -1;
        if (vParam.Count > 0)
            flag = int.Parse(vParam[0]);
        if (eventId == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED && flag !=-1)
        {
            if (gameObject.active)
            {
                OnChangedRoleType(mCurrentRoleType);
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_EQUIP && vParam.Count == 0 )
        {
            if (gameObject.active)
            {
                OnChangedRoleType(mCurrentRoleType);
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_PET_PAGE)
        {
            if (gameObject.active)
                petWidgetsShow();
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_PETEQUIP)
        {
            if (gameObject.active)
            {
                OnChangedRoleType(mCurrentRoleType);
            }
        }
        else if (eventId == GAME_EVENT_ID.GE_TOGGLE_PETEQUIPLEVELUP)
        {
            if (vParam.Count == 2)
            {
                int RoleType = Convert.ToInt32(vParam[0]);
                int PetEquipIndex = Convert.ToInt32(vParam[1]);
                if (!gameObject.active)
                {
                    petWidgetsShow();
                    UIWindowMng.Instance.ShowWindow("EquipWindow");
                }
                mCurrentEquipIndex = PetEquipIndex;

                mRoleMenus[(int)mCurrentRoleType].Value = false;
                mCurrentRoleType = (RoleType)RoleType;
                mRoleMenus[(int)mCurrentRoleType].Value = true;

                mOperateMenus[(int)mCurrentOperate].Value = false;
                mCurrentOperate = OperateType.SHENGDANG;
                mOperateMenus[(int)mCurrentOperate].Value = true;
            }
        }
	}
	

	//关闭装备界面
	void CloseBtnClick()
	{
		UIWindowMng.Instance.HideWindow("EquipWindow");

        //clear
        ClearAllSelectItem();
	}
    void ClearAllSelectItem()
    {
        ClearAllGemMat();
        ClearEnchanseSelected();
        ClearEnchanseMat();
        if (mSelectedItem != null)
        {
            mSelectedItem.isLocked = false;
            mSelectedItem = null;
        }
    }

    public void OnShengDangBtnClicked()
    {
        if (mSelectedItem != null)
        {
            LifeAbility.Instance.setOperaterRole((int)mCurrentRoleType); 
            LifeAbility.Instance.equipOperate(mSelectedItem, EquipUpLevelOp.Name);
        }
    }
    //强化
    public void OnEnchanceBtnClicked()
    {
        if (mSelectedItem != null)
        {
            LifeAbility.Instance.setOperaterRole((int)mCurrentRoleType); 
            LifeAbility.Instance.equipOperate(mSelectedItem, EquipStrengthenOp.Name);
        }
    }
    //镶嵌
    public void OnEnchaseBtnClicked()
    {
        CObject_Item_Equip equipTarget = mSelectedItem as CObject_Item_Equip;
        if (equipTarget != null)
        {
            short[] gemIndexs = new short[GAMEDEFINE.MAX_ITEM_GEM];
            for (int i = 0; i < mEnchanseGemInfo.Length; i++ )
            {
                if (mEnchanseGemInfo[i].bFlag == 0)
                    gemIndexs[i] = -2;
                else if(mEnchanseGemInfo[i].bFlag == 2)
                    gemIndexs[i] = -1;
                else if (mEnchanseGemInfo[i].bFlag == 1)
                {
                    CObject_Item gem = CDataPool.Instance.UserBag_GetItemById(mEnchanseGemInfo[i].nGemType);
                    if (gem != null)
                        gemIndexs[i] = gem.PosIndex;
                    else
                        gemIndexs[i] = -2;
                }

            }
            LifeAbility.Instance.setOperaterRole((int)mCurrentRoleType);
            LifeAbility.Instance.Do_Enchase(equipTarget, gemIndexs);
        }
    }
    //合成
    public void OnCombineBtnClicked()
    {
        List<CObject_Item> stuffs = new List<CObject_Item>();
        foreach (int id in mGemCombineSelectedIDs)
        {
            CObject_Item item = CDataPool.Instance.UserBag_GetItemById(id);
            if (item != null)
                stuffs.Add(item);
        }
        LifeAbility.Instance.Do_Combine(stuffs.ToArray());
    }
    public void OnLearnPrescrBtnClicked()
    {
        //search item 
        LogManager.LogWarning("Search item= " + mSearchPrescrItem);
    }

    public void OnGemContainerPre()
    {
        if (mGemBeginIndex >= MAX_GEMCONTAINER_COUNT)
            mGemBeginIndex -= MAX_GEMCONTAINER_COUNT;
        UpdateGemContainer();
    }
    public void OnGemContainerNext()
    {
        mGemBeginIndex += MAX_GEMCONTAINER_COUNT;
        UpdateGemContainer();
    }
}
