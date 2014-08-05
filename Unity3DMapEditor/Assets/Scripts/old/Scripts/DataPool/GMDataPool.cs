/****************************************\
*										*
* 			   数据池					*
*										*
\****************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Network.Packets;
/// <summary>
/// 重新构架DataPool，将大部分功能拆分到具体的类中
/// CharacterData----------------不变
/// CoolDownGroup----------|
/// PetSkillCoolDownGroup--|
/// CommonCoolDown---------|-----mCoolDownManager
/// QuestTimeGroup---------|
/// 
/// UserEquip--------------------mMyEquip
/// OtherPlayerEquip-------------mOtherEquip
/// UserBag----------------------mUserBange
/// ItemBox----------------------mDropBox;
/// Booth------------------------mBooth;
/// Bank-------------------------mBank;
/// Relation---------------------不变;
/// MailPool---------------------不变；
/// MyExBox----------------|-----mExchangeManager;
/// OtExBox----------------|
/// 
/// MissionBox-------------------不变
/// QuestLogBox------------------不变
/// MyStallBox-------------|
/// MyStallMsg-------------|
/// OtStallBox-------------|-----mStallManager;
/// OtStallMsg-------------|
/// 
/// StudySkill-------------|-----mStudySkill;
/// StudyAbility-----------|
/// 
/// Pet--------------------|------mPetDataPool;
/// PetSkillStudy----------|
/// PetPlaCard-------------|
/// BuffImpact--------------------mPlayerBuffImpact;
/// DisCard-----------------------不变；
/// X_SCRIPT----------------------不变；
/// Split-------------------------mSliptOp;
/// PlayerShop--------------------mPlayerShopManager;
/// Guild-------------------------mGuildManager;
/// </summary>
public class CDataPool
{
    static readonly CDataPool instance = new CDataPool();
    public static CDataPool Instance
    {
        get
        {
            return instance;
        }
    }
    public CDataPool()
    {
        mCoolDownManager = new CoolDownManager();
        mPlayerBuffImpact = new PlayerBuffImpact();
    }

    public void Tick()
    {
        uint uDeltaTime = GameProcedure.s_pTimeSystem.GetDeltaTime();
        mCoolDownManager.Tick(uDeltaTime);
        uint uTimeNow = GameProcedure.s_pTimeSystem.GetTimeNow();
        mPlayerBuffImpact.Tick(uTimeNow);
    }

    public void Initial()
    {
        mPlayerBuffImpact.ImpactTime = GameProcedure.s_pTimeSystem.GetTimeNow();

        mCampaignManager.Initial();
        mCoolDownManager.Initial();
    }

    public void Release()
    {
        mPlayerBuffImpact.BuffImpact_RemoveAll();
    }
    //------------
    //角色属性创建/销毁
    //------------
    public CCharacterData CharacterData_Create(CObject_Character character)
    {
        //创建新的角色数据
        CCharacterData pNewCharacterData = new CCharacterData(character);

        m_mapAllCharacterData[character.ID] = pNewCharacterData;

        return pNewCharacterData;
    }
    public void CharacterData_Destroy(CObject_Character character)
    {
        if (character == null) throw new NullReferenceException("character is null in CharacterData_Destroy()");
        if (m_mapAllCharacterData.ContainsKey(character.ID))
        {
            m_mapAllCharacterData.Remove(character.ID);

            //是否能成功？
            if (character.IsMySelf(character.ServerID))
                CActionSystem.Instance.CleanInvalidAction();
        }
    }
    public CCharacterData CharacterData_Get(int nID)
    {
        if (m_mapAllCharacterData.ContainsKey(nID))
        {
            return m_mapAllCharacterData[nID];
        }
        else
            return null;
    }

    public _ATTR_LEVEL1[] RandomAttrs 
    { 
        get { return mRandomAttrs; } 
    }
    //_ATTR_LEVEL1    mRandomAttr;
    _ATTR_LEVEL1[] mRandomAttrs = new _ATTR_LEVEL1[GAMEDEFINE.HUMAN_PET_MAX_COUNT + 1];

    protected Dictionary<int, CCharacterData> m_mapAllCharacterData = new Dictionary<int, CCharacterData>();
    protected uint m_nBuffImpactTime = 0;

    //------------
    //查看对方角色属性界面
    //------------
    //对方的Obj
    protected CObject m_pTargetEquip;
    //------------
    //冷却组
    //------------
    CoolDownManager mCoolDownManager;

    //取得冷却组
    public virtual COOLDOWN_GROUP CoolDownGroup_Get(int nCoolDownID)
    {
        if (mCoolDownManager != null)
            return mCoolDownManager.CoolDownGroup_Get(nCoolDownID);
        else
            return null;
    }

    public int CommonCoolDown_Get()
    {
        return mCoolDownManager.CommonCoolDown_Get();
    }

    public virtual void CommonCoolDown_Update() { mCoolDownManager.CommonCoolDown_Update(); }

    public void CoolDownGroup_UpdateList(Cooldown_T[] pCoolDownList, int nUpdateNum)
    {
        mCoolDownManager.CoolDownGroup_UpdateList(pCoolDownList, nUpdateNum);
    }

    public void PetSkillCoolDownGroup_UpdateList(Cooldown_T[] pCoolDownList, int nUpdateNum, PET_GUID_t nPet_GUID)
    {
        mCoolDownManager.PetSkillCoolDownGroup_UpdateList(pCoolDownList, nUpdateNum, nPet_GUID);
    }


    //--------
    //玩家装备
    //--------
    PlayerEquip mMyEquip = new PlayerEquip();
    PlayerEquip mOtherEquip = new PlayerEquip();

    //////////////////////////////////////////////////////////////////////////
    //------------
    //主角背包列表
    //------------
    Inventory mUserInventory = new Inventory();

    public int UserBag_CountItemByIDTable(int id)
    {
        return mUserInventory.CountItemByIDTable(id);
    }

    //根据id来获取某种宝石的数量
    public int UserBag_CountGemByIDTable(int id)
    {
        return mUserInventory.CountGemByIDTable(id);
    }

    public CObject_Item UserBag_GetItemById(int id)
    {
        return mUserInventory.GetItemByID(id);
    }

    public CObject_Item UserBag_GetItemByIndex(int index)
    {
        return mUserInventory.GetItem(index);
    }

    public CObject_Item GetMedicial()
    {
        return mUserInventory.GetMedicialItem();
    }


    public int UserBag_GetItemIndexByID(int nID)
    {

        return mUserInventory.GetItemIndexByID(nID);
    }

    public void UserBag_Clear()
    {
        mUserInventory.Clear();
    }

    public void UserBag_SetItem(short nBagIndex, CObject_Item pItem, bool bClearOld, bool bNew)
    {
        mUserInventory.SetItem(nBagIndex, pItem, bClearOld, bNew);
    }
    public virtual CObject_Item UserBag_GetItem(int nBagIndex)
    {
        return mUserInventory.GetItem(nBagIndex);
    }
    //重新设置物品属性
    public void UserBag_SetItemInfo(int nBagIndex, bool bEmpty, _ITEM pItem)
    {
        mUserInventory.SetItemInfo(nBagIndex, bEmpty, ref pItem);
    }

    public int UserBag_FindFirstEmptyPlace()
    {
        return mUserInventory.FindFirstEmptyPlace();
    }

    //------------
    //掉落盒子
    //-----------
    ItemBox mDropBox;

    //------------
    //npc商店
    //------------
    Booth mBooth = new Booth();
    public void Booth_Clear()
    {
        mBooth.Clear();
    }
    public void Booth_Sold_Clear()
    {
        mBooth.Sold_Clear();
    }
    public void Booth_SetItem(int nBoothIndex, CObject_Item pItem)
    {
        mBooth.SetItem(nBoothIndex, pItem);
    }
    public void Booth_SetItemPrice(int nBoothIndex, uint nPrice)
    {
        mBooth.SetItemPrice(nBoothIndex, nPrice);
    }
    public void Booth_SetSoldItem(int nBoothIndex, CObject_Item pItem)
    {
        mBooth.SetSoldItem(nBoothIndex, pItem);
    }
    public void Booth_SetSoldPrice(int nSoldIndex, uint uPrice)
    {
        mBooth.SetSoldPrice(nSoldIndex, uPrice);
    }
    public void Booth_SetNumber(int nBoothNumber) { mBooth.SetNumber(nBoothNumber); }
    public void Booth_SetSoldNumber(int nBoothNumber) { mBooth.SetSoldNumber(nBoothNumber); }
    public CObject_Item Booth_GetItem(int nBoothIndex)
    {
        return mBooth.GetItem(nBoothIndex);
    }
    public uint Booth_GetItemPrice(int nBoothIndex)
    {
        return mBooth.GetItemPrice(nBoothIndex);
    }
    public uint Booth_GetSoldPrice(int nSoldIndex)
    {
        return mBooth.GetSoldPrice(nSoldIndex);
    }
    public CObject_Item Booth_GetSoldItem(int nBoothIndex)
    {
        return mBooth.GetSoldItem(nBoothIndex);
    }
    public CObject_Item Booth_GetItemByID(int IDtable)
    {
        return mBooth.GetItemByID(IDtable);
    }
    public int Booth_GetNumber() { return mBooth.GetNumber(); }
    public int Booth_GetSoldNumber() { return mBooth.GetSoldNumber(); }
    //	virtual	BOOL				Booth_IsCursorRepairing(void)	const {return	m_bIsRepairing;}
    //	virtual	void				Booth_SetCursorRepairing(BOOL flag)	{m_bIsRepairing = flag;}
    public bool Booth_IsClose() { return mBooth.IsClose(); }
    public void Booth_Open() { mBooth.Open(); }
    public void Booth_Close() { mBooth.Close(); }
    public void Booth_SetBuyType(int nBuyType) { mBooth.SetBuyType(nBuyType); }
    public int Booth_GetBuyType() { return mBooth.GetBuyType(); }
    public void Booth_SetRepairType(int nRepairType) { mBooth.SetRepairType(nRepairType); }
    public int Booth_GetRepairType() { return mBooth.GetRepairType(); }

    public void Booth_SetRepairLevel(int nRepairLevel) { mBooth.SetRepairLevel(nRepairLevel); }
    public int Booth_GetRepairLevel() { return mBooth.GetRepairLevel(); }
    public void Booth_SetBuyLevel(int nBuyLevel) { mBooth.SetBuyLevel(nBuyLevel); }
    public int Booth_GetBuyLevel() { return mBooth.GetBuyLevel(); }
    public void Booth_SetRepairSpend(float nRepairSpend) { mBooth.SetRepairSpend(nRepairSpend); }
    public float Booth_GetRepairSpend() { return mBooth.GetRepairSpend(); }
    public void Booth_SetRepairOkProb(float nRepairOkProb) { mBooth.SetRepairOkProb(nRepairOkProb); }
    public float Booth_GetRepairOkProb() { return mBooth.GetRepairOkProb(); }
    public void Booth_SetScale(float fScale) { mBooth.SetScale(fScale); }
    public float Booth_GetScale() { return mBooth.GetScale(); }

    public void Booth_SetShopNpcId(int nShopNpcId) { mBooth.SetShopNpcId(nShopNpcId); }
    public int Booth_GetShopNpcId() { return mBooth.GetShopNpcId(); }

    public void Booth_SetShopUniqueId(uint nShopUniqueId) { mBooth.SetShopUniqueId(nShopUniqueId); }
    public uint Booth_GetShopUniqueId() { return mBooth.GetShopUniqueId(); }

    public bool Booth_IsCanRepair(CObject_Item pItem)
    {
        return mBooth.IsCanRepair(pItem);
    }
    public bool Booth_IsCanBuy(CObject_Item pItem)
    {
        return mBooth.IsCanBuy(pItem);
    }

    public void Booth_SetCurrencyUnit(int nCurrencyUnit) { mBooth.SetCurrencyUnit(nCurrencyUnit); }
    public int Booth_GetCurrencyUnit() { return mBooth.GetCurrencyUnit(); }

    public void Booth_SetSerialNum(int nSerialNum) { mBooth.SetSerialNum(nSerialNum); }
    public int Booth_GetSerialNum() { return mBooth.GetSerialNum(); }

    public void Booth_SetbCanBuyMult(int bBuyMulti) { mBooth.SetbCanBuyMult(bBuyMulti); }
    public int Booth_GetbCanBuyMult() { return mBooth.GetbCanBuyMult(); }

    public void Booth_SetCallBack(bool bCallBack) { mBooth.SetCallBack(bCallBack); }
    public bool Booth_GetCallBack() { return mBooth.GetCallBack(); }

    public void Booth_SetShopType(int nType) { mBooth.SetShopType(nType); }
    public int Booth_GetShopType() { return mBooth.GetShopType(); }

    public int Booth_GetShopNpcCamp() { return mBooth.GetShopNpcCamp(); }

    //---------------
    //银行
    //---------------
    Bank mBank = new Bank();
    //------------------------------------------------------
    // 银行数据的访问
    //------------------------------------------------------
    public void UserBank_Clear()
    {
        mBank.Clear();
    }
    public void UserBank_SetItem(int nBankIndex, CObject_Item pItem, bool bClearOld)
    {
        mBank.SetItem(nBankIndex, pItem, bClearOld);
    }
    public CObject_Item UserBank_GetItem(int nBankIndex)
    {
        return mBank.GetItem(nBankIndex);
    }
    public void UserBank_SetBankEndIndex(int endindex) { mBank.SetBankEndIndex(endindex); }
    public int UserBank_GetBankEndIndex() { return mBank.GetBankEndIndex(); }
    public void UserBank_SetBankMoney(int Money) { mBank.SetBankMoney(Money); }
    public int UserBank_GetBankMoney() { return mBank.GetBankMoney(); }
    public void UserBank_SetBankRMB(int Money) { mBank.SetBankRMB(Money); }
    public int UserBank_GetBankRMB() { return mBank.GetBankRMB(); }
    public void UserBank_SetItemExtraInfo(int nBankIndex, bool bEmpty, _ITEM pItem)
    {
        mBank.SetItemExtraInfo(nBankIndex, bEmpty, ref pItem);
    }
    //查询是否解封
    public bool UserBank_IsValid(int nIndex)
    {
        return mBank.IsValid(nIndex);
    }
    // 查询银行nIndex编号的租赁箱子是不是有空格
    public bool UserBank_IsEmpty(int nIndex) { return mBank.IsEmpty(nIndex); }
    public void UserBank_SetNpcId(int nNpcId) { mBank.SetNpcId(nNpcId); }
    public int UserBank_GetNpcId() { return mBank.GetNpcId(); }

    //------------
    // 关系系统部分
    //------------
    Relation m_pRelation = new Relation();
    public  virtual Relation GetRelation() { return m_pRelation; }
    //virtual CMailPool* GetMailPool() { return m_pMailPool; }

    //-----------------
    //交易管理
    //-----------------
    ExchangeManager mExchangeManager;

    //----------
    //任务道具
    //----------
    //virtual void				MissionBox_Clear(void);					//清空递交任务物品的列表
    //virtual void				MissionBox_SetItem(int nMissionBoxIndex, tObject_Item* pItem, BOOL bClearOld=TRUE);//设置递交任务物品的列表
    //virtual tObject_Item*		MissionBox_GetItem(int nMissionBoxIndex);//取得递交任务物品的列表

    //virtual void				QuestLogBox_Clear(void);							//清空任务物品的列表
    //virtual void				QuestLogBox_SetItem(int nQuestIndex, int nItemIndex, tObject_Item* pItem, BOOL bClearOld=TRUE);//设置任务物品的列表
    //virtual tObject_Item*		QuestLogBox_GetItem(int nQuestIndex, int nItemIndex);	//取得任务物品的列表

    //void						Copy_To_TargetEquip(CObject* pObj){ m_pTargetEquip = pObj; }
    //CObject*					GetTargetEquip(void) { return m_pTargetEquip;}
    public CObject GetTargetEquip() { return m_pTargetEquip; }

    //-------------
    //技能学习
    //-------------
    public StudySkill _StudySkill
    {
        get
        {
            if (mStudySkill == null)
                mStudySkill = new StudySkill();
            return mStudySkill;
        }
    }

    StudySkill mStudySkill;

    public bool isCanSkillLevelUp(int nID, int nLevel, out string error)
    {
        return _StudySkill.isCanUplevel(nID, nLevel, out error);
    }
    public int GetUpLevelXinfaCDTime()
    {
        return _StudySkill.GetUpLevelXinfaCDTime();
    }

    //-----------
    //宠物
    //-----------
    PetDataPool mPetDataPool = new PetDataPool();
    PetDataPool mOtherPlayerPetDataPool = new PetDataPool();

    public int Pet_GetCount()
    {
        return mPetDataPool.Pet_GetCount();
    }
    public SDATA_PET Pet_GetPet(int index)
    {
        return mPetDataPool.Pet_GetPet(index);
    }

    public SDATA_PET Pet_GetValidPet(int index)
    {
        return mPetDataPool.Pet_GetValidPet(index);
    }
    public SDATA_PET Pet_GetPet(PET_GUID_t guidPet)
    {
        return mPetDataPool.Pet_GetPet(guidPet);
    }
    public void Pet_SetSkill(int nIndex, int nSkillIndex, ref _OWN_SKILL Skill)
    {
        mPetDataPool.Pet_SetSkill(nIndex, nSkillIndex, ref Skill);
    }
    public void Pet_SetSkill(int nIndex, int nSkillIndex, PET_SKILL pPetSkillData, bool bClearOld)
    {
        mPetDataPool.Pet_SetSkill(nIndex, nSkillIndex, pPetSkillData, bClearOld);
    }
    public bool Pet_SendUseSkillMessage(int petIndex,short idSkill, int idTargetObj, float fTargetX, float fTargetY)
    {
        return mPetDataPool.Pet_SendUseSkillMessage(petIndex,idSkill, idTargetObj, fTargetX, fTargetY);
    }
    public PET_SKILL Pet_GetSkill(int nIndex, int nSkillIndex)
    {
        return mPetDataPool.Pet_GetSkill(nIndex, nSkillIndex);
    }
    public PET_SKILL TargetPet_GetSkill(int nSkillIndex)
    {
        return mPetDataPool.TargetPet_GetSkill(nSkillIndex);
    }
    public COOLDOWN_GROUP PetSkillCoolDownGroup_Get(int nCoolDownID, int nPetNum)
    {
        return mCoolDownManager.PetSkillCoolDownGroup_Get(nCoolDownID, nPetNum);
    }
    public PET_SKILL PetSkillStudy_MenPaiList_Get(int idx)
    {
        return mPetDataPool.PetSkillStudy_MenPaiList_Get(idx);
    }
    public SDATA_PET TargetPet_GetPet()
    {
        return mPetDataPool.TargetPet_GetPet();
    }
     public int  PetPlacard_GetItemCount()
    {
        return mPetDataPool.PetPlacard_GetItemCount();
    }

    public _PET_PLACARD_ITEM	PetPlacard_GetItem(int nIndex)
    {
        return mPetDataPool.PetPlacard_GetItem(nIndex);
    }

    public void PetPlacard_AddItem(_PET_PLACARD_ITEM pItem)
    {
        mPetDataPool.PetPlacard_AddItem(pItem);
    }

    public void PetPlacard_CleanUp()
    {
        mPetDataPool.PetPlacard_CleanUp();
    }

    public void Pet_CopyToTarget(SDATA_PET pSourcePet)
    {
        mPetDataPool.Pet_CopyToTarget(pSourcePet);
    }

    public int Pet_GetPetFoodType(PET_GUID_t guidPet)
    {
        return mPetDataPool.Pet_GetPetFoodType(guidPet);
    }
    public CObject_Item Pet_GetLowestLevel_Food_From_Package(PET_GUID_t pg, ref int idxPackage)	
    {
        return mPetDataPool.Pet_GetLowestLevel_Food_From_Package(pg, ref idxPackage);
    }

    public CObject_Item Pet_GetLowestLevel_Dome_From_Package(PET_GUID_t pg, ref int idxPackage)
    {
        return mPetDataPool.Pet_GetLowestLevel_Dome_From_Package(pg, ref idxPackage);
    }

    //--------------
    //玩家buff列表
    //--------------
    PlayerBuffImpact mPlayerBuffImpact;

    // 需要销毁的物品的一个临时记忆区
    struct DISCARD_ITEM
    {
        public int m_nContainer;
        public int m_nPosition;
        void CleanUp()
        {
            m_nContainer = -1;
            m_nPosition = -1;
        }
    };
    DISCARD_ITEM m_CurDisCardItem;


    //当前需要销毁物品
    public int DisCard_GetItemConta() { return m_CurDisCardItem.m_nContainer; }
    public int DisCard_GetItemPos() { return m_CurDisCardItem.m_nPosition; }
    public void DisCard_SetItemConta(int nContainer) { m_CurDisCardItem.m_nContainer = nContainer; }
    public void DisCard_SetItemPos(int nPosition) { m_CurDisCardItem.m_nPosition = nPosition; }


    X_PARAM m_X_PARAM;
    X_SCRIPT m_X_SCRIPT;

    public void X_PARAM_Set(X_PARAM value, int uUIIndex)
    {
        m_X_PARAM = value;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UI_COMMAND, uUIIndex);
    }

    public X_PARAM _X_PARAM
    {
        get
        {
            if (m_X_PARAM == null)
                m_X_PARAM = new X_PARAM();
            return m_X_PARAM;
        }

    }
    public X_SCRIPT _X_SCRIPT
    {
        get
        {
            if (m_X_SCRIPT == null)
                m_X_SCRIPT = new X_SCRIPT();
            return m_X_SCRIPT;
        }
    }

    //------------
    //拆分操作
    //------------
    SplitOperation mSplitOp;

    ////活动界面
    //float ConvertTimeToFloat(STRING time);
    //string ConvertIntToString(int intValue);

    //---------
    //玩家商店
    //---------
    PlayerShopManager mPlayerShopManager;

    //------------
    //帮会
    //------------
    GuildManager mGuildManager;


    //---------------------------------------------------------------------------------------------------------------------------------
    // 鉴定相关
    //----------------------------------------------------------------------------------------------------------------------------------
    //// 锁定鉴定轴2006-4-15
    //void Identify_Lock(int iBagPos);

    //// 取消鉴定轴锁定2006－4－15
    //void Identify_UnLock();

    //// 当前鉴定卷轴的位置
    //int m_nIdentifyPosition;
    //// 使用卷轴的背包索引位置
    //int m_iIdentifyInBagIndex;
    ////鉴定卷轴等级。
    //int m_iIdentifyLevel;

    //---------------
    //装备打孔
    //---------------
    AddHoleOperation mAddHoleOp;
    public void QuestTimeGroup_UpdateList(int nQuestTime, int nUpdateNum)
    {
        //if (nUpdateNum <= 0) return;
        //if (nUpdateNum > m_vQuestTimeGroup.Count) nUpdateNum = m_vQuestTimeGroup.Count;

        //m_vCoolDownGroup[nUpdateNum].nTime = m_vCoolDownGroup[nUpdateNum].nTotalTime = (int)(nQuestTime);

    }


    public _BUFF_IMPACT_INFO BuffImpact_GetByIndex(int nIndex)
    {
        return mPlayerBuffImpact.BuffImpact_GetByIndex(nIndex);
    }

    public _BUFF_IMPACT_INFO BuffImpact_GetByID(int nID)
    {
        return mPlayerBuffImpact.BuffImpact_GetByID(nID);
    }
    public bool BuffImpact_Add(_BUFF_IMPACT_INFO pBuffImpact)
    {
        return mPlayerBuffImpact.BuffImpact_Add(pBuffImpact);
    }

    public void BuffImpact_Remove(int nSN)
    {
        mPlayerBuffImpact.BuffImpact_Remove(nSN);
    }

    public void BuffImpact_RemoveByIndex(int nIndex)
    {
        mPlayerBuffImpact.BuffImpact_RemoveByIndex(nIndex);
    }

    public void BuffImpact_RemoveAll()
    {
        mPlayerBuffImpact.BuffImpact_RemoveAll();
    }

    public string BuffImpact_GetToolTips(int nSN)
    {
        return mPlayerBuffImpact.BuffImpact_GetToolTips(nSN);
    }

    public string BuffImpact_GetToolTipsByIndex(int nIndex)
    {
        return mPlayerBuffImpact.BuffImpact_GetToolTipsByIndex(nIndex);
    }

    public string BuffImpact_GetIconNameByIndex(int nIndex)
    {
        return mPlayerBuffImpact.BuffImpact_GetIconNameByIndex(nIndex);
    }

    public bool BuffImpact_Dispel(int nSN)
    {
        return mPlayerBuffImpact.BuffImpact_Dispel(nSN);
    }

    public bool BuffImpact_DispelByIndex(int nIndex)
    {
        return mPlayerBuffImpact.BuffImpact_DispelByIndex(nIndex);
    }

    public int BuffImpact_GetTime(int nIndex)
    {
        return mPlayerBuffImpact.BuffImpact_GetTime(nIndex);
    }

    public int BuffImpact_GetTimeByIndex(int nIndex)
    {
        return mPlayerBuffImpact.BuffImpact_GetTimeByIndex(nIndex);
    }

    //详细请求队列
    List<int> m_setAskBagExtra = new List<int>();
    internal void UserBag_AskExtraInfo(short nBagIndex)
    {
        if (nBagIndex >= GAMEDEFINE.MAX_BAG_SIZE) return;

        //不重复提交
        if (m_setAskBagExtra.IndexOf(nBagIndex) != -1)
            return;

        CGAskItemInfo msg = new CGAskItemInfo();
        msg.BagIndex = nBagIndex;
        NetManager.GetNetManager().SendPacket(msg);

        //记录请求号
        m_setAskBagExtra.Add(nBagIndex);
    }

    public void OtherPlayerEquip_SetItem(HUMAN_EQUIP ptEquip, CObject_Item pEquipItem, bool bClearOld)
    {
        mOtherEquip.SetItem((short)ptEquip, pEquipItem, bClearOld);
    }


    internal void UserEquip_SetItem(HUMAN_EQUIP ptEquip, CObject_Item pEquipItem, bool bClearOld)
    {
        mMyEquip.SetItem((short)ptEquip, pEquipItem, bClearOld);
    }

    public CObject_Item UserEquip_GetItem(HUMAN_EQUIP ptEquip)
    {
        if ((int)ptEquip < 0 && (int)ptEquip >= (int)HUMAN_EQUIP.HEQUIP_NUMBER)
            return null;

        return mMyEquip.GetItem((int)ptEquip);
    }

    public CObject_Item UserPetEquipt_GetItem(PET_GUID_t guid,PET_EQUIP ptEquipt)
    {
        return mPetDataPool.GetEquiptItem(guid,ptEquipt);
    }

    public void UserPetEquipt_SetItem(PET_GUID_t guid, PET_EQUIP ptEquipt,CObject_Item pEquiptItem, bool bClearOld)
    {
        mPetDataPool.SetEquiptItem(guid,ptEquipt,pEquiptItem,bClearOld);
    }

    public void OtherPlayerPetEquip_SetItem(PET_GUID_t guid, PET_EQUIP ptEquipt, CObject_Item pEquiptItem, bool bClearOld)
    {
        mOtherPlayerPetDataPool.SetEquiptItem(guid, ptEquipt, pEquiptItem, bClearOld);
    }

    public CObject_Item OtherPlayerPetEquip_GetItem(PET_GUID_t guid, PET_EQUIP ptEquipt)
    {
        return mOtherPlayerPetDataPool.GetEquiptItem(guid, ptEquipt);
    }

    //得到当前装备的强化等级
    public string GetStrengthLevelDesc(int nLevel)
    {
        string text = "";
        if (nLevel == 0)
        {
            text = "未强化";
        }
        else if (nLevel >= 1 && nLevel <= 10)
        {
            text = "凡人" + nLevel + "级";
        }
        else if (nLevel >= 11 && nLevel <= 20)
        {
            nLevel = nLevel - 10;
            text = "炼气" + nLevel + "级";
        }
        else if (nLevel >= 21 && nLevel <= 30)
        {
            nLevel = nLevel - 20;
            text = "筑基" + nLevel + "级";
        }
        else if (nLevel >= 31 && nLevel <= 40)
        {
            nLevel = nLevel - 30;
            text = "结丹" + nLevel + "级";

        }
        else if (nLevel >= 41 && nLevel <= 50)
        {
            nLevel = nLevel - 40;
            text = "元婴" + nLevel + "级";
        }
        else
            return "";

        return text;

    }

    //得到装备下一级的强化等级
    public string GetStrengthNextLevelDesc(int nLevel)
    {
        string text = "";
        if (nLevel >= 0 && nLevel <= 9)
        {
            nLevel = nLevel + 1;
            text = "凡人" + nLevel + "级";
        }
        else if (nLevel >= 10 && nLevel <= 19)
        {
            nLevel = nLevel - 10 + 1;
            text = "炼气" + nLevel + "级";
        }
        else if (nLevel >= 20 && nLevel <= 29)
        {
            nLevel = nLevel - 20 + 1;
            text = "筑基" + nLevel + "级";
        }
        else if (nLevel >= 30 && nLevel <= 39)
        {
            nLevel = nLevel - 30 + 1;
            text = "结丹" + nLevel + "级";

        }
        else if (nLevel >= 40 && nLevel <= 49)
        {
            nLevel = nLevel - 40 + 1;
            text = "元婴" + nLevel + "级";
        }
        else
            return "";

        return text;
    }

    CObject_Item_Equip mPreviewEquip;
    public CObject_Item_Equip PreviewEquip
    {
        get { return mPreviewEquip; }
        set { mPreviewEquip = value; }
    }


    internal CObject_Item OtherPlayerEquip_GetItem(HUMAN_EQUIP ptEquip)
    {
        if ((int)ptEquip < 0 && (int)ptEquip >= (int)HUMAN_EQUIP.HEQUIP_NUMBER)
            return null;

        return mOtherEquip.GetItem((int)ptEquip);
    }

    internal int GetPlayerMission_Num()
    {
        int nNum = CDetailAttrib_Player.Instance.GetMission_Num();
        return nNum;
    }

    internal _MISSION_INFO GetPlayerMissionByIndex(int index)
    {
        _OWN_MISSION miss = CDetailAttrib_Player.Instance.GetMission(index);
        return GetPlayerMissionById(miss.m_idMission);
    }


    internal _MISSION_INFO GetPlayerMissionById(int id)
    {
        _MISSION_INFO missData = MissionStruct.Instance.GetMissionInfo(id);
        return missData;
    }

    internal int GetMissionIdByIndex(int index)
    {
        _OWN_MISSION miss = CDetailAttrib_Player.Instance.GetMission(index);
        if (miss == null)
            return -1;
        else
            return miss.m_idMission;
    }
    byte m_bCharFirstLogin;
    public void SetCharFirstLogin(byte bFirst)
    {
        m_bCharFirstLogin = bFirst;

    }

    //------------
    //当前打开的箱子
    //------------
    //箱子ID
    int m_idItemBox;
    //设置箱子ID
	public virtual void			ItemBox_SetID(int id)		{ m_idItemBox = id; }
	//取得箱子ID
	public virtual int			ItemBox_GetID() 	{ return m_idItemBox; }

    //箱子物品实例数组
    CObject_Item[] m_vItemBox = new CObject_Item[GAMEDEFINE.MAX_BOXITEM_NUMBER];
    public virtual void ItemBox_Clear()
    {
        for (int i = 0; i < GAMEDEFINE.MAX_BOXITEM_NUMBER; i++)
        {
            if (m_vItemBox[i] != null)
            {
                ObjectSystem.Instance.DestroyItem(m_vItemBox[i]);
                m_vItemBox[i] = null;
            }
        }

        m_idItemBox = MacroDefine.INVALID_ID;
    }

    public virtual void ItemBox_SetItem(int nBoxIndex, CObject_Item pItem, bool bClearOld)
    {
        if (nBoxIndex < 0 || nBoxIndex >= GAMEDEFINE.MAX_BOXITEM_NUMBER) return;
        if (m_vItemBox[nBoxIndex] != null && bClearOld)
        {
            ObjectSystem.Instance.DestroyItem(m_vItemBox[nBoxIndex]);
            m_vItemBox[nBoxIndex] = null;
        }

        if (pItem != null)
        {
            pItem.SetTypeOwner(ITEM_OWNER.IO_ITEMBOX);
            pItem.SetPosIndex((short)nBoxIndex);
        }

        m_vItemBox[nBoxIndex] = pItem;
    }

    public virtual CObject_Item ItemBox_GetItem(int nBoxIndex)
    {
        if (nBoxIndex < 0 || nBoxIndex >= GAMEDEFINE.MAX_BOXITEM_NUMBER) return null;

        return m_vItemBox[nBoxIndex];
    }

    public virtual CObject_Item ItemBox_GetItem(ushort idWorld, ushort idServer, int idSerial, ref int nIndex)
    {
        for (int i = 0; i < GAMEDEFINE.MAX_BOXITEM_NUMBER; i++)
        {
            if (m_vItemBox[i] != null)
            {
                ushort wWorld = 0;
                ushort wServer = 0;
                uint dwSerial = 0;
                m_vItemBox[i].GetGUID(ref wWorld, ref wServer, ref dwSerial);

                if (wWorld == idWorld && wServer == idServer && dwSerial == idSerial)
                {
                    nIndex = i;
                    return m_vItemBox[i];
                }
            }
        }
        return null;
    }

    public virtual int ItemBox_GetNumber()
    {
        int nNumber = 0;
        for (int i = 0; i < GAMEDEFINE.MAX_BOXITEM_NUMBER; i++)
        {
            if (m_vItemBox[i] != null) nNumber++;
        }
        return nNumber;
    }
    //玩家符印数据
    byte[,] mCharmInfo = new byte[GAMEDEFINE.CHARM_ATTR_NUM, GAMEDEFINE.CHARM_LEVEL_NUM];
    public byte[,] PlayerCharmInfo
    {
        set
        {
            mCharmInfo = value;
			if(CharmScript.sSelf != null)
            	CharmScript.sSelf.flush();
        }
        get
        {
            return mCharmInfo;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //活动列表管理
    CampaignManager mCampaignManager = new CampaignManager();

    public void Campaign_Clear()
    {
        mCampaignManager.ClearTeamInfo();
    }
    public void Campaign_AddTeamInfo(RECRUIT_INFO info)
    {
        mCampaignManager.AddTeamInfo(info);
    }
    public int Campaign_GetTeamCount()
    {
        return mCampaignManager.GetTeamCount();
    }
    public RECRUIT_INFO Campaign_GetTeamInfo(int nIndex)
    {
        return mCampaignManager.GetTeamInfo(nIndex);
    }
    public int Campaign_GetCampaignCount(int nType)
    {
        return mCampaignManager.GetCampaignCount(nType);
    }
    public int Campaign_GetCampaignCount()
    {
        return mCampaignManager.GetCampaignCount();
    }
    public _DBC_ACTIVITY_INFO Campaign_GetCamaignInfo(int nIndex, int nType)
    {
        return mCampaignManager.GetCampaignInfo(nIndex, nType);
    }
	public _DBC_ACTIVITY_INFO Campaign_GetCampaignInfo(int nId)
	{
		return mCampaignManager.GetCampaignInfo(nId);
	}
    public bool Campaign_CheckTime(int nId)
    {
        return mCampaignManager.CheckCampaignTime(nId);
    }
    public bool Campaign_CheckMySelf(int nId)
    {
        return mCampaignManager.CheckMySelf(nId);
    }
    public void Campaign_SetSystemTime(DateTime time)
    {
        mCampaignManager.SystemTime = time;
    }
  
    CTalisman_Equipments mTalismanEquip = new CTalisman_Equipments();
    CTalisman_Inventory  mTalismanInventory = new CTalisman_Inventory();
    public int TalismanInventory_UnLockCount()
    {
        return mTalismanInventory.UnLockCount; 
    }

    public void TalismanInventory_UnLockCount(int count)
    {
        mTalismanInventory.UnLockCount = count;
    }

    public void TalismanInventory_SetItem(int index, ref _ITEM item)
    {
        mTalismanInventory.setItem(index, ref item);
    }
	public void TalismanInventory_SetItem(short nBagIndex, CObject_Item pItem, bool bClearOld, bool bNew)
    {
        mTalismanInventory.SetItem(nBagIndex, pItem,  bClearOld,  bNew);
    }

    public void TalismanEquip_SetItem(int index, ref _ITEM item)
    {
        mTalismanEquip.setItem(index, ref item);
    }

    public void TalismanEquip_SetItem(short nBagIndex, CObject_Item pItem, bool bClearOld, bool bNew)
    {
        mTalismanEquip.SetItem(nBagIndex, pItem, bClearOld, bNew);
    }

    public CTalisman_Item TalismanInventory_GetItem(int index)
    {
        return mTalismanInventory.getItem(index);
    }

    public void TalismanInventory_SetItemInfo(int nBagIndex, bool bEmpty, _ITEM pItem)
    {
        mTalismanInventory.SetItemInfo(nBagIndex, bEmpty, ref pItem);
    }

    public int TalismanEquipment_UnLockCount()
    {
        return mTalismanEquip.UnLockCount;
    }

    public void TalismanEquipment_UnLockCount(int count)
    {
        mTalismanEquip.UnLockCount = count;
    }

    public CTalisman_Item TalismanEquipment_GetItem(int index)
    {
        return mTalismanEquip.getItem(index);
    }

    public int FindTalismanEquiptEmptyPlace()
    {
        return mTalismanEquip.FindFirstEmptyItemPlace();
    }

    public int FindTalismanInventoryEmptyPlace()
    {
        return mTalismanInventory.FindFirstEmptyItemPlace();
    }

    public CTalisman_Item TalismanInventory_GetSuitableCompoundItem(int lv,int quality)
    {
        return mTalismanInventory.GetSuitableCompoundItem(lv, quality);
    }

    //---------------------------------------------
    //帮派列表
    //---------------------------------------------
    //客户端显示的帮派列表
	GuildInfo_t[]						m_GuildList = new GuildInfo_t[GAMEDEFINE.MAX_GUILD_SIZE];
	int									m_GuildNum;

	//帮众列表
	GuildMemberInfo_t[]					m_GuildMemList = new GuildMemberInfo_t[GAMEDEFINE.USER_ARRAY_SIZE];
	int									m_GuildMaxMemNum;
	int									m_GuildMemNum;
	string								m_GuildDesc;
	string								m_GuildName;
	byte								m_uPosition;
	byte								m_uAccess;

	//帮派的详细信息
	GuildDetailInfo_t					m_GuildDetailInfo = new GuildDetailInfo_t();

	//可任命帮派的职位信息
	GuildAppointPos_t[]					m_GuildPosAvail =new GuildAppointPos_t[(int)GUILD_POSITION.GUILD_POSITION_SIZE];
    public void Guild_ClearAllInfo()//清空所有工会信息
    {
        if(m_GuildNum == 0)
		    return;

	    for(int i = 0; i<m_GuildNum; i++)
	    {
		    m_GuildList[i].CleanUp(); 
	    }
	    m_GuildNum = 0;
	    return;
    }
    public GuildInfo_t Guild_GetInfoByIndex(int nIndex)//通过索引获得工会信息
    {
        if (nIndex >= 0 && nIndex < GAMEDEFINE.MAX_GUILD_SIZE)
        {
            return m_GuildList[nIndex];
        }
        return null;
    }
    void Guild_SetInfoByIndex(int nIndex, GuildInfo_t pGuildInfo)//通过索引设置工会信息
    {
        if (nIndex >= 0 && nIndex < GAMEDEFINE.MAX_GUILD_SIZE)
        {
            m_GuildList[nIndex] = pGuildInfo;
        }
    }

    public int Guild_GetInfoNum() { return m_GuildNum; }//获得工会数量
    public void Guild_SetInfoNum(int iGuildNum) { m_GuildNum = iGuildNum; }//设置工会数量

    //---------------------------------------------
    //帮派成员列表
    //---------------------------------------------
    public  void Guild_ClearMemInfo()//清空所有帮众信息
    {
        if(m_GuildMemNum == 0)
		return;

	    for(int i = 0; i<m_GuildMemNum; i++)
	    {
		    m_GuildMemList[i].CleanUp(); 
	    }
	    m_GuildMemNum		= 0;
	    m_GuildMaxMemNum	= 0;
	    m_GuildDesc = "";
	    m_GuildName = "";
	    m_uAccess	=	0;
	    m_uPosition	=	0;
	    return;
    }
    public GuildMemberInfo_t Guild_GetMemInfoByIndex(int nIndex)//通过索引获得帮众信息
    {
        if (nIndex >= 0 && nIndex < GAMEDEFINE.USER_ARRAY_SIZE)
        {
            return m_GuildMemList[nIndex];
        }
        return null;
    }
    public void Guild_SetMemInfoByIndex(int nIndex, GuildMemberInfo_t pMemberInfo)//通过索引设置帮众信息
    {
        if (nIndex >= 0 && nIndex < GAMEDEFINE.USER_ARRAY_SIZE)
        {
            m_GuildMemList[nIndex] = pMemberInfo;
        }
    }
    public int Guild_GetMemInfoNum() { return m_GuildMemNum; }//获得帮众信息
    public void Guild_SetMemInfoNum(int iMemNum) { m_GuildMemNum = iMemNum; }//设置帮众信息

    public int Guild_GetMaxMemNum() { return m_GuildMaxMemNum; }//获得帮众的最大数量
    public void Guild_SetMaxMemNum(int iMaxMemNum) { m_GuildMaxMemNum = iMaxMemNum; }//设置帮众的最大数量

    public string Guild_GetDesc() { return m_GuildDesc; }//获得帮派宗旨
    public void Guild_SetDesc(string pGuildDesc) { m_GuildDesc = pGuildDesc; }//设置帮派宗旨

    public string Guild_GetName() { return m_GuildName; }//获得帮派宗旨
    public void Guild_SetName(string pGuildName) { m_GuildName = pGuildName; }//设置帮派宗旨

    public byte Guild_GetCurAccess() { return m_uAccess; }//获得当前人物权限
    public void Guild_SetCurAccess(byte uAccess) { m_uAccess = uAccess; }//设置当前人物权限

    public byte Guild_GetCurPosition() { return m_uPosition; }//获得当前人物职位
    public void Guild_SetCurPosition(byte uPosition) { m_uPosition = uPosition; }//设置当前人物职位


    //---------------------------------------------
    //帮派的详细信息
    //---------------------------------------------
    public void Guild_ClearDetailInfo()//清空帮派的详细信息
    {
        m_GuildDetailInfo.CleanUp();
    }
    public GuildDetailInfo_t Guild_GetDetailInfo()//获得帮派的详细信息
    {
        return m_GuildDetailInfo;
    }
    public void Guild_SetDetailInfo(GuildDetailInfo_t pDetailInfo)//设置帮派的详细信息
    {
        m_GuildDetailInfo = pDetailInfo;
    }

    //---------------------------------------------
    //帮派可任命职位
    //---------------------------------------------
    public void Guild_ClearAppointInfo()//清空帮派可任命职位
    {
        for (int i = 0; i < (int)GUILD_POSITION.GUILD_POSITION_SIZE; i++)
        {
            m_GuildPosAvail[i].CleanUp();
        }
    }
    public GuildAppointPos_t Guild_GetAppointInfoByIndex(int nIndex)//获得帮派可任命职位
    {
        if (nIndex >= 0 && nIndex < (int)GUILD_POSITION.GUILD_POSITION_SIZE)
        {
            return m_GuildPosAvail[nIndex];
        }
        return null;
    }
    public void Guild_SetAppointInfoByIndex(int nIndex, GuildAppointPos_t pAppointInfo)//设置帮派可任命职位
    {
        if (nIndex >= 0 && nIndex < (int)GUILD_POSITION.GUILD_POSITION_SIZE)
        {
            m_GuildPosAvail[nIndex] = pAppointInfo;
        }
    }
}
