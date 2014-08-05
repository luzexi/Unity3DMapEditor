//////////////////////////////////////////////////////////////////////////
//玩家商店，源自：CDataPool

struct PLAYERSHOP_BOX												//标示商店中的所有柜台
{
    //std::vector<PLAYERSHOP_STALL_BOX> m_StallList;
    //UINT m_BaseMoney;
    //UINT m_ProfitMoney;
    //_PLAYERSHOP_GUID m_ShopID;
    //STRING m_szShopName;				//标示商店名
    //STRING m_szShopDesc;				//标示商店描述
    //STRING m_szOwnerName;				//标示店主名
    //UINT m_OwnerGuid;				//店主GUID
    //BYTE m_bIsSaleOut;				//此商店已被盘出
    //UINT m_uSaleOutPrice;			//商店的盘出价
    //BYTE m_Serial;					//商店的序列号
    //INT m_nShopType;				//商店的类型
    //BYTE m_ExRecListNum;				//交易记录数量
    //BYTE m_MaRecListNum;				//管理记录数量
    //BYTE m_StallOpenNum;				//已经拥有的柜台数
    //BYTE m_ShopSubType;				//商店子类型
    //std::vector<PlayerShopPartners_t> m_PartnerList;				//合伙人列表

    //PLAYERSHOP_BOX()
    //{
    //    m_BaseMoney = 0;
    //    m_ProfitMoney = 0;
    //    m_szShopName = "";
    //    m_szOwnerName = "";
    //    m_szShopDesc = "";
    //    m_OwnerGuid = 0;
    //    m_bIsSaleOut = FALSE;
    //    m_uSaleOutPrice = 0;
    //    m_Serial = 0;
    //    m_ExRecListNum = 0;
    //    m_MaRecListNum = 0;
    //    m_StallOpenNum = 0;
    //    PLAYERSHOP_STALL_BOX StallInit;
    //    m_StallList.resize(MAX_STALL_NUM_PER_SHOP, StallInit);
    //    PlayerShopPartners_t PartnerInit;
    //    m_PartnerList.resize(MAX_PARTNER_PER_SHOP, PartnerInit);
    //    m_nShopType = NO_SHOP;
    //    m_ShopSubType = 0;
    //}
    //VOID CleanUp()
    //{
    //    m_BaseMoney = 0;
    //    m_ProfitMoney = 0;
    //    m_szShopName = "";
    //    m_szOwnerName = "";
    //    m_OwnerGuid = 0;
    //    m_bIsSaleOut = FALSE;
    //    m_uSaleOutPrice = 0;
    //    m_Serial = 0;
    //    m_ExRecListNum = 0;
    //    m_MaRecListNum = 0;
    //    m_StallOpenNum = 0;
    //    m_ShopID.Reset();
    //    m_nShopType = NO_SHOP;
    //    m_ShopSubType = 0;
    //    for (INT i = 0; i < MAX_STALL_NUM_PER_SHOP; i++)
    //    {
    //        m_StallList[i].CleanUp();
    //    }
    //    for (INT i = 0; i < MAX_PARTNER_PER_SHOP; i++)
    //    {
    //        m_PartnerList[i].CleanUp();
    //    }
    //}
};

public class PlayerShop
{
    private PLAYERSHOP_BOX mShopBox;


    //------------------------------------------------------
	// 玩家商店数据访问
	//------------------------------------------------------
    //BOOL						PlayerShop_GetMeIsOwner(){return m_bMeIsOwner;}
    //VOID						PlayerShop_SetMeIsOwner(BOOL bOwner){m_bMeIsOwner = bOwner;}

    //INT							PlayerShop_GetNpcID(){return m_nPSNpcID;}
    //VOID						PlayerShop_SetNpcID(INT nPSNpcID){m_nPSNpcID = nPSNpcID;}

    //INT							PlayerShop_GetShopNum(){return m_nShopNum;}
    //VOID						PlayerShop_SetShopNum(INT nShopNum){m_nShopNum = nShopNum;}

    //FLOAT						PlayerShop_GetCommercialFactor(){return m_CommerFactor;}
    //VOID						PlayerShop_SetCommercialFactor(FLOAT fComFactor){ m_CommerFactor = fComFactor;}

    //UINT						PlayerShop_GetCost(){return m_PlayerShopApplyInfo.m_Cost;}
    //VOID						PlayerShop_SetCost(UINT uCost){m_PlayerShopApplyInfo.m_Cost = uCost;}

    //BYTE						PlayerShop_GetType(){return m_PlayerShopApplyInfo.m_Type;}
    //VOID						PlayerShop_SetType(BYTE nType){m_PlayerShopApplyInfo.m_Type = nType;}

    //LPCTSTR						PlayerShop_GetShopNameByIndex(UINT uIndex);
    //VOID						PlayerShop_SetShopNameByIndex(UINT uIndex, LPCTSTR pszShopName);

    //LPCTSTR						PlayerShop_GetShopDescByIndex(UINT uIndex);
    //VOID						PlayerShop_SetShopDescByIndex(UINT uIndex, LPCTSTR pszShopDesc);

    //LPCTSTR						PlayerShop_GetShopFoundedTimeByIndex(UINT uIndex);
    //VOID						PlayerShop_SetShopFoundedTimeByIndex(UINT uIndex, LPCTSTR pszTime);

    //BYTE						PlayerShop_GetStallNumOpened(UINT uIndex);
    //VOID						PlayerShop_SetStallNumOpened(UINT uIndex, BYTE nStallNumOpened);

    //BYTE						PlayerShop_GetStallNumOnSale(UINT uIndex);
    //VOID						PlayerShop_SetStallNumOnSale(UINT uIndex, BYTE nStallNumOnSale);

    //_PLAYERSHOP_GUID			PlayerShop_GetIDByIndex(UINT uIndex);
    //VOID						PlayerShop_SetIDByIndex(UINT uIndex, _PLAYERSHOP_GUID nId);

    //BYTE						PlayerShop_GetIsFavorByIndex(UINT uIndex);
    //VOID						PlayerShop_SetIsFavorByIndex(UINT uIndex, BYTE bIsFavor);

    //INT							PlayerShop_GetStallEnableByIndex(BOOL bIsMine, UINT nStallIndex);
    //VOID						PlayerShop_SetStallEnableByIndex(BOOL bIsMine, UINT nStallIndex, BYTE bIsEnable);

    //BYTE						PlayerShop_GetTypeByIndex(UINT uIndex);
    //VOID						PlayerShop_SetTypeByIndex(UINT uIndex, BYTE nType);

    //LPCTSTR						PlayerShop_GetOwnerNameByIndex(UINT uIndex);
    //VOID						PlayerShop_SetOwnerNameByIndex(UINT uIndex, LPCTSTR pszOwnerName);

    //UINT						PlayerShop_GetOwnerGuidByIndex(UINT uIndex);
    //VOID						PlayerShop_SetOwnerGuidByIndex(UINT uIndex, UINT uOwnerGuid);

    //VOID						PlayerShop_CleanUp(BOOL bIsMine);//清空玩家商店

    //VOID						PlayerShop_SetShopID(BOOL bIsMine, _PLAYERSHOP_GUID nShopID);//设置商店ID
    //_PLAYERSHOP_GUID			PlayerShop_GetShopID(BOOL bIsMine);//获得商店ID

    //VOID						PlayerShop_SetShopSerial(BOOL bIsMine, BYTE nShopSerial);//设置商店序列号
    //BYTE						PlayerShop_GetShopSerial(BOOL bIsMine);//获得商店序列号

    //VOID						PlayerShop_SetItem(BOOL bIsMine, INT nStallIndex, INT nBoxIndex, tObject_Item* pItem, BOOL bClearOld=TRUE);//设置摊位物品
    //tObject_Item*				PlayerShop_GetItem(BOOL bIsMine, INT nStallIndex, INT nBoxIndex);//获得制定位置的物品

    //VOID						PlayerShop_SetItemPrice(BOOL bIsMine, INT nStallIndex, INT nBoxIndex, UINT nPrice);//设置物品价格
    //UINT						PlayerShop_GetItemPrice(BOOL bIsMine, INT nStallIndex, INT nBoxIndex);//获得物品价格

    //VOID						PlayerShop_SetItemSerial(BOOL bIsMine, INT nStallIndex, INT nBoxIndex, UINT nSerial);//设置物品序列号
    //UINT						PlayerShop_GetItemSerial(BOOL bIsMine, INT nStallIndex, INT nBoxIndex);//获得物品序列号

    //VOID						PlayerShop_SetItemOnSale(BOOL bIsMine, INT nStallIndex, INT nBoxIndex, BYTE bIsEnable);//此物体是否上架
    //BYTE						PlayerShop_GetItemOnSale(BOOL bIsMine, INT nStallIndex, INT nBoxIndex);//此物体是否上架

    //VOID						PlayerShop_SetBaseMoney(BOOL bIsMine, UINT uBaseMoney);//设置商店本金
    //UINT						PlayerShop_GetBaseMoney(BOOL bIsMine);//设置商店本金

    //VOID						PlayerShop_SetProfitMoney(BOOL bIsMine, UINT uProfitPrice);//设置商店盈利金
    //UINT						PlayerShop_GetProfitMoney(BOOL bIsMine);//设置商店盈利金

    ////只有自己的店可以看到
    //INT							PlayerShop_GetPartnerNum(){return m_nPartnerNum;}
    //VOID						PlayerShop_SetPartnerNum(INT nPartnerNum){m_nPartnerNum = nPartnerNum;}

    //VOID						PlayerShop_SetPartnerByIndex(UINT uIndex, GUID_t Guid, LPCTSTR szName);//设置指定位置的合作伙伴
    //PlayerShopPartners_t* 		PlayerShop_GetPartnerByIndex(UINT uIndex);//获得指定位置的合作伙伴

    //VOID						PlayerShop_SetShopType(BOOL bIsMine, INT nShopType);//设置商店类型
    //UINT						PlayerShop_GetShopType(BOOL bIsMine);//设置商店类型

    //VOID						PlayerShop_SetIsSaleOut(BOOL bIsMine, BOOL bIsSaleOut);//设置商店此商店是否是出售状态
    //BOOL						PlayerShop_GetIsSaleOut(BOOL bIsMine);//设置商店此商店是否是出售状态

    //VOID						PlayerShop_SetSaleOutPrice(BOOL bIsMine, UINT uSaleOutPrice);//设置商店此商店是否是出售状态
    //UINT						PlayerShop_GetSaleOutPrice(BOOL bIsMine);//获得商店的盘出价

    //VOID						PlayerShop_SetOwnerGuid(BOOL bIsMine, UINT uOwnerGuid);//设置店主guid
    //UINT						PlayerShop_GetOwnerGuid(BOOL bIsMine);//获得店主guid

    //VOID						PlayerShop_SetExRecNum(BOOL bIsMine, UINT uExRecNum);//设置交易记录数量
    //UINT						PlayerShop_GetExRecNum(BOOL bIsMine);//获得交易记录数量

    //VOID						PlayerShop_SetMaRecNum(BOOL bIsMine, UINT uMaRecNum);//设置管理记录数量
    //UINT						PlayerShop_GetMaRecNum(BOOL bIsMine);//获得管理记录数量

    //VOID						PlayerShop_SetOpenStallNum(BOOL bIsMine, UINT uOpenStallNum);//设置已经拥有的柜台数
    //UINT						PlayerShop_GetOpenStalldNum(BOOL bIsMine);//获得已经拥有的柜台数

    //VOID						PlayerShop_SetShopSubType(BOOL bIsMine, UINT uShopSubType);//设置商店的类型
    //UINT						PlayerShop_GetShopSubType(BOOL bIsMine);//获得商店的类型

    //VOID						PlayerShop_SetOwnerName(BOOL bIsMine, LPCTSTR pszOwnerName);//设置店主名
    //LPCTSTR						PlayerShop_GetOwnerName(BOOL bIsMine);//获得店主名

    //VOID						PlayerShop_SetShopName(BOOL bIsMine, LPCTSTR pszShopName);//设置商店名
    //LPCTSTR						PlayerShop_GetShopName(BOOL bIsMine);//获得商店名

    //VOID						PlayerShop_SetShopDesc(BOOL bIsMine, LPCTSTR pszShopDesc);//设置商店描述
    //LPCTSTR						PlayerShop_GetShopDesc(BOOL bIsMine);//获得商店名

    //VOID						PlayerShop_SetItemExtraInfo(BOOL bIsMine, INT nStallIndex, INT nBoxIndex, BOOL bEmpty, const _ITEM* pItem);//设置物品详细信息
    //INT							PlayerShop_GetItemNum(BOOL bIsMine, INT nStallIndex);//获得摊位中的物品数量
    //INT							PlayerShop_GetItemIndexByGUID(BOOL bIsMine, INT nStallIndex, __int64 ItemGUID);//根据GUID查找物品位置

    //SDATA_PET*					PlayerShop_GetPet(BOOL bIsMine, INT nStallIndex, INT nIndex);

    //INT							PlayerShop_GetPetByGuid(BOOL bIsMine, INT nStallIndex, PET_GUID_t PetGuid);//根据GUID在玩家商店指定柜台中查找物品
    //VOID						PlayerShop_SetPet(BOOL bIsMine, INT nStallIndex, INT nIndex, SDATA_PET* pPetData, BOOL bClearOld = TRUE);//在指定柜台中设置宠物
    //VOID						PlayerShop_SetPetSerial(BOOL bIsMine, INT nStallIndex, INT nIndex, INT nSerial);//设置指定柜台的格子序列号
    //INT							PlayerShop_GetPetSerial(BOOL bIsMine, INT nStallIndex, INT nIndex);//获得指定柜台的格子序列号
    //VOID						PlayerShop_SetPetPrice(BOOL bIsMine, INT nStallIndex, INT nIndex, INT nPetPrice);
    //INT							PlayerShop_GetPetPrice(BOOL bIsMine, INT nStallIndex, INT nIndex);
    //VOID						PlayerShop_SetSkill(BOOL bIsMine, INT nStallIndex, INT nIndex, INT nSkillIndex, PET_SKILL* pPetSkillData, BOOL bClearOld = TRUE);	
    //VOID						PlayerShop_SetSkill(BOOL bIsMine, INT nStallIndex, INT nIndex,INT nSkillIndex, const _OWN_SKILL *Skill);
    //PET_SKILL*					PlayerShop_GetSkill(BOOL bIsMine, INT nStallIndex, INT nIndex,INT nSkillIndex);

    ////清除
    //VOID						PlayerShop_CleanObjItem(BOOL bIsMine);

    ////玩家商店上选中的物品
    //VOID						PlayerShop_MyClearSelect(){m_MyPsSelectItem.m_nConIndex = -1;m_MyPsSelectItem.m_nPosition = -1;}
    //VOID						PlayerShop_OtClearSelect(){m_OtPsSelectItem.m_nConIndex = -1;m_OtPsSelectItem.m_nPosition = -1;}

    //INT							PlayerShop_GetMySelectConTa(){return m_MyPsSelectItem.m_nConIndex;}
    //INT							PlayerShop_GetMySelectPos(){return m_MyPsSelectItem.m_nPosition;}
    //INT							PlayerShop_GetOtSelectConTa(){return m_OtPsSelectItem.m_nConIndex;}
    //INT							PlayerShop_GetOtSelectPos(){return m_OtPsSelectItem.m_nPosition;}

    //VOID						PlayerShop_SetMySelectConTa(INT nConIndex) {m_MyPsSelectItem.m_nConIndex = nConIndex;}
    //VOID						PlayerShop_SetMySelectPos(INT nPosition){m_MyPsSelectItem.m_nPosition = nPosition;}
    //VOID						PlayerShop_SetOtSelectConTa(INT nConIndex) {m_OtPsSelectItem.m_nConIndex = nConIndex;}
    //VOID						PlayerShop_SetOtSelectPos(INT nPosition){m_OtPsSelectItem.m_nPosition = nPosition;}
}

public class MySelfShop :  PlayerShop
{
    
}

public class PlayerShopManager
{
    PlayerShop mOtherPlayerShop;
    MySelfShop mMyPlayerShop;

    //RecordList_t* GetRecordList() { return &m_RecordList; }
}