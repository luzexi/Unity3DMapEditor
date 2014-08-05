//////////////////////////////////////////////////////////////////////////
//摆摊管理器，源自CDataPool的摆摊接口
//------------
//摊位盒
//------------
enum STALL_DEFAULT_PAGE
{
    ITEM_PAGE = 0,
    PET_PAGE,
};

struct STALL_ITEM
{

    CObject_Item pItem;							//物品指针
    uint nPrice;							//价格
    uint nSerial;						//序列号
    byte nbIsEnable;						//这个物品是否上架
};

struct STALL_PET
{
 //   STALL_PET()
 //   {
 //       pPet = NULL;
 //       nPrice = 0;
 //       nSerial = 0;
 //       nbIsEnable = true;
 //   }
 //   SDATA_PET* pPet;							//宠物指针
 //   UINT nPrice;							//价格
 //   UINT nSerial;						//序列号
 //   BYTE nbIsEnable;						//这个物品是否上架
};

struct CLIENT_STALL_BOX
{
    //INT m_nDefaultPage;					//缺省的页面
    //UINT m_ObjID;
    //BOOL m_bIsOpen;						//自己摊位是否已经打开（是否处于交易状态）
    //UINT m_PosTax;						//摊位费
    //UINT m_TradeTax;						//每次交易税收
    //STRING m_szStallName;					//标示摊位名
    //STRING m_szOwnerName;					//标示摊主名
    //UINT m_nFirstPage;					//起始页
    //std::vector<STALL_ITEM> m_ItemList;						//标示摊位中的所有物品
    //std::vector<STALL_PET> m_PetList;						//标示摊位中的所有宠物
    //UINT m_OtGUID;
    //VOID CleanUp();
};
struct BBS_t
{
    //BBS_t()
    //{
    //    CleanUp();
    //}
    //VOID CleanUp();

    //STRING m_szTitle;
    //UINT m_nSerial;
    //INT m_nMessageNum;
    //std::vector<MessageEntry_t> m_MessageList;

};
public class StallMsg
{
    private BBS_t bbsMsg;
    //VOID				ClearUp(VOID);
    //VOID				SetSerial( UINT nSerial) {m_MyStallBox.m_BBS.m_nSerial = nSerial;}
    //INT					SetIDByIndex(UINT nIndex, UINT nID);
    //INT					SetTimeByIndex(UINT	nIndex, BYTE nHour, BYTE nMin);
    //INT					SetHasReplyByIndex(UINT	nIndex, BYTE bhasReply);
    //INT					SetReTimeByIndex(UINT nIndex, BYTE nHour, BYTE nMin);
    //VOID				SetTitle(LPCTSTR szMsg);
    //INT					SetAuthorNameByIndex(UINT	nIndex, LPCTSTR szMsg);
    //INT					SetProposedMessageByIndex(UINT	nIndex, LPCTSTR szMsg);
    //INT					SetReplyMessageByIndex(UINT nIndex, LPCTSTR szMsg);
    //UINT				GetSerial() {return m_MyStallBox.m_BBS.m_nSerial;}
    //INT					GetIDByIndex(UINT nIndex);
    //BYTE				GetHasReplyByIndex(UINT nIndex);
    //LPCTSTR				GetTitle();
    //LPCTSTR				GetAuthorNameByIndex(UINT nIndex);
    //LPCTSTR				GetProposedMessageByIndex(UINT nIndex );
    //LPCTSTR				GetReplyMessageByIndex(UINT nIndex);
    //INT					GetMessageNum();
    //VOID				SetMessageNum(INT nNum);
    //INT					GetHourByIndex(UINT	nIndex);
    //INT					GetMinByIndex(UINT	nIndex);
    //INT					GetReHourByIndex(UINT	nIndex);
    //INT					GetReMinByIndex(UINT	nIndex);
}
public class StallBox
{
    private CLIENT_STALL_BOX mStallBox;
    private StallMsg mStallMsg;
    //virtual VOID				SetDefaultPage(INT nPage){m_MyStallBox.m_nDefaultPage = nPage;}
    //virtual INT					GetDefaultPage(){return m_MyStallBox.m_nDefaultPage;}
    //virtual VOID				SetPosTax(UINT unPosTax){m_MyStallBox.m_PosTax = unPosTax;}//设置摊位费
    //virtual UINT				GetPosTax(VOID){return m_MyStallBox.m_PosTax;}//获得摊位费
    //virtual VOID				SetTradeTax(UINT unTradeTax){m_MyStallBox.m_TradeTax = unTradeTax;}//设置交易税
    //virtual UINT				GetTradeTax(VOID){return m_MyStallBox.m_TradeTax;}//获得交易税

    //virtual VOID				Clear(VOID);					//清空自己摊位盒
    //virtual VOID				SetIsOpen(BOOL IsOpen){ m_MyStallBox.m_bIsOpen = IsOpen; }//设置摊主ID
    //virtual BOOL				GetIsOpen(){ return m_MyStallBox.m_bIsOpen; }//获得摊主ID
    //virtual VOID				SetObjID(UINT nObjID){ m_MyStallBox.m_ObjID = nObjID; }//设置摊主ID
    //virtual UINT				GetObjID(){ return m_MyStallBox.m_ObjID; }//获得摊主ID
    //virtual VOID				SetStallName(LPCTSTR szName) {m_MyStallBox.m_szStallName = szName;}//设置摊位名
    //virtual LPCTSTR				GetStallName(){ return m_MyStallBox.m_szStallName.c_str(); }//获得摊位名
    //virtual VOID				SetOwnerName(LPCTSTR szName) {m_MyStallBox.m_szOwnerName = szName;}//设置摊主名
    //virtual LPCTSTR				GetOwnerName(){ return m_MyStallBox.m_szOwnerName.c_str(); }//获得摊主名
    //virtual VOID				SetFirstPage(UINT nFirstPage) {m_MyStallBox.m_nFirstPage = nFirstPage;}//设置首页
    //virtual UINT				GetFirstPage(){ return m_MyStallBox.m_nFirstPage; }//获得首页
    //virtual VOID				SetItem(INT nStallBoxIndex, tObject_Item* pItem, BOOL bClearOld=TRUE);//设置摊位中的物品
    //virtual VOID				SetItemPrice(INT nStallBoxIndex, UINT nPrice);//设置摊位物品价格
    //virtual VOID				SetItemSerial(INT nStallBoxIndex, UINT nSerial);//设置摊位物品序列号
    //virtual tObject_Item*		GetItem(INT nStallBoxIndex);//获得摊位中指定位置的物品
    //virtual UINT				GetItemPrice(INT nStallBoxIndex);//获得物品价格
    //virtual UINT				GetItemSerial(INT nStallBoxIndex);//获得物品序列号
    //virtual	VOID				SetItemExtraInfo(INT nStallBoxIndex, BOOL bEmpty, const _ITEM* pItem);//设置物品详细信息
    //virtual	INT					GetItemNum(){return (INT)m_MyStallBox.m_ItemList.size(); }//获得物品数量
    //virtual	BOOL				IsOpen(){return (INT)m_MyStallBox.m_bIsOpen; }//查询自己是否处于摆摊状态
    //virtual VOID				SetCurSelectItem(tObject_Item* pItem){m_CurSelectItem.pItem = pItem;}//当前选中的物品
    //virtual tObject_Item*		GetCurSelectItem(){return m_CurSelectItem.pItem;}//当前选中的物品
    //virtual VOID				SetSelectItemInPos(INT nSelectItemInPos){m_nSelectItemInPos = nSelectItemInPos;}//当前选中的物品所在的容器
    //virtual INT					GetSelectItemInPos(){return m_nSelectItemInPos;}//当前选中的物品所在的容器
    //virtual	INT					GetItemIndexByGUID(__int64 ItemGUID);//根据GUID查找物品位置
    //virtual	UINT				GetOwnerGUID() {return m_MyStallBox.m_OtGUID;}
    //virtual	VOID				SetOwnerGUID(UINT GUID){m_MyStallBox.m_OtGUID = GUID;}

    //virtual INT					GetnSelectPetOnStall(){return m_nSelectPetOnStall;}//当前选中的物品所在的容器
    //virtual	VOID				SetnSelectPetOnStall(INT nSelectPetOnStall){m_nSelectPetOnStall = nSelectPetOnStall;};//根据GUID查找物品位置
    //virtual	BOOL				IsHaveEmpty(INT nType);

    ////摆摊宠物
    //INT							GetPetByGuid(PET_GUID_t PetGuid);
    //VOID						SetPet(INT nIndex, SDATA_PET* pPetData, BOOL bClearOld = TRUE);
    //VOID						SetPetPrice(INT nIndex, INT nPetPrice);
    //VOID						SetPetSerial(INT nIndex, INT nSerial);
    //INT							GetPetPrice(INT nIndex);
    //INT							GetPetSerial(INT nIndex);
    //SDATA_PET*					GetPet(INT nIndex);
    //INT							GetCount(VOID);
    //VOID						SetSkill(INT nIndex,INT nSkillIndex,const _OWN_SKILL *Skill);
    //PET_SKILL*					GetSkill(INT nIndex,INT nSkillIndex);
    //VOID						GetSelectpetGuid(PET_GUID_t &CurSelectpetGuid)
    //{
    //                            CurSelectpetGuid.m_uHighSection = m_CurSelectpetGuid.m_uHighSection;
    //                            CurSelectpetGuid.m_uLowSection  = m_CurSelectpetGuid.m_uLowSection;
    //};           
    //VOID						SetSelectpetGuid(PET_GUID_t CurSelectpetGuid)
    //{
    //                            m_CurSelectpetGuid.m_uHighSection = CurSelectpetGuid.m_uHighSection;
    //                            m_CurSelectpetGuid.m_uLowSection  = CurSelectpetGuid.m_uLowSection;
    //}
    //virtual	BOOL				IsPetOnStall(PET_GUID_t PetGuid);
}

public class StallManager
{
    private StallBox m_MyStallBox;					//自己的摊位盒
    private StallBox m_OtStallBox;					//对方的摊位盒
    private STALL_ITEM m_CurSelectItem;				//玩家选中的物品
    private int m_nSelectItemInPos;				    //选中的这个物品所在的容器
    private PET_GUID_t m_CurSelectpetGuid;			//选中等待上架的宠物的GUID
    private int m_nSelectPetOnStall;			    //在摊位上被选中的宠物编号
}