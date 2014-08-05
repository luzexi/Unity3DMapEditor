
using System.Collections.Generic;

//////////////////////////////////////////////////////////////////////////
//交易系统，源自CDataPool

//------------
//交易盒
//------------
struct CLIENT_EXCHANGE_BOX
{
    public bool m_IsLocked;						//标示界面上lock选项是否钩上
    public bool m_CanConform;					//标示是否显示确定按钮
    public uint m_Money;						//标示交易的金钱
    public List<CObject_Item> m_ItemList;						//标示交易栏中自己的物品
    //public List<SDATA_PET> m_Petlist;						//标示交易栏中自己的宠物

    public void CleanUp(){}
};

/// <summary>
/// 交易的双方
/// </summary>
public class ExchangePlayer
{

    private CLIENT_EXCHANGE_BOX mExchangeBox;
    //------------------------------------------------------
	// 交易盒数据访问
	//------------------------------------------------------
	void				Clear(){}
	void                SetItem(int nExBoxIndex, CObject_Item pItem, bool bClearOld){}
	CObject_Item		GetItem(int nExBoxIndex)
    {
        //Todo:
        return null;
    }
	void				SetItemExtraInfo(int nExBoxIndex, bool bEmpty, ref _ITEM pItem)
    {

    }
    void                SetMoney(uint Money) { mExchangeBox.m_Money = Money; }
    uint                 GetMoney() { return mExchangeBox.m_Money; }
    void                SetLock(bool Setlock) { mExchangeBox.m_IsLocked = Setlock; }
    bool                GetLock() { return mExchangeBox.m_IsLocked; }
    void                SetConfirm(bool Confirm) { mExchangeBox.m_CanConform = Confirm; }
    bool                GetConfirm() { return mExchangeBox.m_CanConform; }
    int                 GetItemNum() { return mExchangeBox.m_ItemList.Count; }

	int							GetPetByGuid(PET_GUID_t PetGuid)
    {
        //Todo:
        return 0;
    }
	//void						SetPet(int nIndex, SDATA_PET* pPetData, bool bClearOld);
	//SDATA_PET*					GetPet(int nIndex);
	int							GetCount()
    {
        //Todo:
        return 0;
    }
	void						SetSkill(int nIndex,int nSkillIndex,ref _OWN_SKILL Skill){}
	//PET_SKILL*					GetSkill(int nIndex,int nSkillIndex);


	// 查询自己的交易框里头是否有空格
	bool				IsEmpty()
    {
        //Todo:
        return true;
    }
}
/// <summary>
/// 玩家交易管理器，原CDataPool的交易接口
/// </summary>
public class ExchangeManager
{

    private int m_CurExID;
    private List<int> m_listApplicantsID;				//申请交易序列
    private short m_pOut;							    //从队列中取出的位置指针
    private short m_pIn;							    //加入队列的位置指针

    private ExchangePlayer mExchangeMySelf;
    private ExchangePlayer mExchangeOther;

    public void SetExchangObjID(int ObjID) { m_CurExID = ObjID; }				//当前交易对方ID
    public int GetExchangObjID() { return m_CurExID; }				//当前交易对方ID

    public void AddToAppList(int appid)				//加入到申请人列表中
    {
        
    }
    public int GetFromAppList()						//从申请人列表中取出一个
    {
        //Todo:
        return 0;
    }
    public bool IsStillAnyAppInList()					//申请人列表是否空
    {
        //Todo:
        return true;
    }
}