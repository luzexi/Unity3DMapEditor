
using System.Collections.Generic;
/// <summary>
/// 商店，原CDataPool中的商店接口
/// </summary>
public class Booth
{
    private int m_nBoothNumber;
    private int m_nBoothSoldNumber;
    private List<CObject_Item> m_listBooth = new List<CObject_Item>();
    private List<CObject_Item> m_listBoothSold = new List<CObject_Item>();
    private List<uint> m_listSoldPrice = new List<uint>();
    private bool m_bBoothWindowIsDirty;
    //	BOOL								m_bIsRepairing;
    private List<uint> m_nlistPrice = new List<uint>();
    private int m_nRepairLevel;		// 修理等级
    private int m_nBuyLevel;		// 收购等级
    private int m_nRepairType;		// 修理类型
    private int m_nBuyType;			// 商店的收购类型
    private float m_nRepairSpend;		// 修理花费
    private float m_nRepairOkProb;	// 修理成功几率
    private float m_fScale;			// 价格系数
    private int m_nShopType;		// 商店类型

    private int m_nCurrencyUnit;	// 
    private int m_nSerialNum;
    private int m_nBuyMulti;
    private bool m_bCallBack;

    // 当前Shop的NpcId
    private int m_nShopNpcId;		// 

    // 当前Shop的NpcId
    private uint m_nShopUniqueId;	// 

    public Booth()
    {
        for (int i = 0; i < GAMEDEFINE.MAX_BOOTH_NUMBER; i++ )
        {
            m_listBooth.Add(null);
            m_nlistPrice.Add(0);
        }
        for (int i = 0; i < GAMEDEFINE.MAX_BOOTH_SOLD_NUMBER; i++ )
        {
            m_listBoothSold.Add(null);
            m_listSoldPrice.Add(0);
        }
 
    }

    public void Clear()
    {

        for (int i = 0; i < m_listBooth.Count; i++)
        {
            if (m_listBooth[i] != null)
            {
                ObjectSystem.Instance.DestroyItem(m_listBooth[i]);
                m_listBooth[i] = null;
                m_nlistPrice[i] = 0;
            }
        }
        m_nBoothNumber = 0;
    }
    public void Sold_Clear()
    {
        for (int i = 0; i < m_listBoothSold.Count; i++)
        {
            if (m_listBoothSold[i] != null)
            {
                ObjectSystem.Instance.DestroyItem(m_listBoothSold[i]);
                m_listBoothSold[i] = null;
            }
        }

        m_nBoothSoldNumber = 0;
    }
    public void SetItem(int nBoothIndex, CObject_Item pItem)
    {
        if(	nBoothIndex < 0 || nBoothIndex >= GAMEDEFINE.MAX_BOOTH_NUMBER ) return;

	    //销毁原有数据
	    if(m_listBooth.Count > nBoothIndex && m_listBooth[nBoothIndex] !=null)
	    {
		    ObjectSystem.Instance.DestroyItem(m_listBooth[nBoothIndex]);
		    m_listBooth[nBoothIndex] = null;
	    }

	    if(pItem != null)
	    {
		    pItem.TypeOwner = ITEM_OWNER.IO_BOOTH;
		    pItem.PosIndex  = (short)nBoothIndex;
	    }
     
	    m_listBooth[nBoothIndex] = pItem;
    }
    public void SetItemPrice(int nBoothIndex, uint nPrice)
    {
        if (nBoothIndex < 0 || nBoothIndex >= GAMEDEFINE.MAX_BOOTH_NUMBER) 
            return;

        m_nlistPrice[nBoothIndex] = nPrice;
    }
    public void SetSoldItem(int nBoothIndex, CObject_Item pItem)
    {
        if (nBoothIndex < 0 || nBoothIndex >= GAMEDEFINE.MAX_BOOTH_SOLD_NUMBER) return;
        //销毁原有数据
        if (m_listBoothSold[nBoothIndex] != null)
        {
            ObjectSystem.Instance.DestroyItem(m_listBoothSold[nBoothIndex]);
            m_listBoothSold[nBoothIndex] = null;
        }

        if (pItem != null)
        {
            //pItem->SetTypeOwner(tObject_Item::IO_BOOTH);
            //pItem->SetPosIndex(nBoothIndex+200);//用来与货架中的普通商品区分
            pItem.TypeOwner = ITEM_OWNER.IO_BOOTH_CALLBACK;
            pItem.PosIndex = (short)nBoothIndex;
        }
        m_listBoothSold[nBoothIndex] = pItem;
    }

    public void SetSoldPrice(int nSoldIndex, uint uPrice)
    {
        if (nSoldIndex < 0 || nSoldIndex >= GAMEDEFINE.MAX_BOOTH_SOLD_NUMBER) return;

        m_listSoldPrice[nSoldIndex] = uPrice;
    }
    public void SetNumber(int nBoothNumber) { m_nBoothNumber = nBoothNumber; }
    public void SetSoldNumber(int nBoothNumber) { m_nBoothSoldNumber = nBoothNumber; }
    public CObject_Item GetItem(int nBoothIndex)
    {
        if (nBoothIndex < 0 || nBoothIndex >= GAMEDEFINE.MAX_BOOTH_NUMBER) return null;

        return m_listBooth[nBoothIndex];
    }
    public uint GetItemPrice(int nBoothIndex)
    {
        if (nBoothIndex < 0 || nBoothIndex >= GAMEDEFINE.MAX_BOOTH_NUMBER) return 0;

        return m_nlistPrice[nBoothIndex];
    }
    public uint GetSoldPrice(int nSoldIndex)
    {
        if (nSoldIndex < 0 || nSoldIndex >= GAMEDEFINE.MAX_BOOTH_SOLD_NUMBER) return 0;

        return m_listSoldPrice[nSoldIndex];
    }
    public CObject_Item GetSoldItem(int nBoothIndex)
    {
        if (nBoothIndex < 0 || nBoothIndex >= GAMEDEFINE.MAX_BOOTH_SOLD_NUMBER) return null;

        return m_listBoothSold[nBoothIndex];
    }
    public CObject_Item GetItemByID(int IDtable)
    {
        for (int i = 0; i < m_nBoothNumber; i++)
        {
            if (m_listBooth[i].GetIdTable() == IDtable)
                return m_listBooth[i];
        }
        return null;
    }
    public int GetNumber() { return m_nBoothNumber; }
    public int GetSoldNumber() { return m_nBoothSoldNumber; }

    public bool IsClose() { return m_bBoothWindowIsDirty; }
    public void Open() { m_bBoothWindowIsDirty = false; }
    public void Close() { m_bBoothWindowIsDirty = true; }
    public void SetBuyType(int nBuyType) { m_nBuyType = nBuyType; }
    public int GetBuyType() { return m_nBuyType; }
    public void SetRepairType(int nRepairType) { m_nRepairType = nRepairType; }
    public int GetRepairType() { return m_nRepairType; }

    public void SetRepairLevel(int nRepairLevel) { m_nRepairLevel = nRepairLevel; }
    public int GetRepairLevel() { return m_nRepairLevel; }
    public void SetBuyLevel(int nBuyLevel) { m_nBuyLevel = nBuyLevel; }
    public int GetBuyLevel() { return m_nBuyLevel; }
    public void SetRepairSpend(float nRepairSpend) { m_nRepairSpend = nRepairSpend; }
    public float GetRepairSpend() { return m_nRepairSpend; }
    public void SetRepairOkProb(float nRepairOkProb) { m_nRepairOkProb = nRepairOkProb; }
    public float GetRepairOkProb() { return m_nRepairOkProb; }
    public void SetScale(float fScale) { m_fScale = fScale; }
    public float GetScale() { return m_fScale; }

    public void SetShopNpcId(int nShopNpcId) { m_nShopNpcId = nShopNpcId; }
    public int GetShopNpcId() { return m_nShopNpcId; }

    public void SetShopUniqueId(uint nShopUniqueId) { m_nShopUniqueId = nShopUniqueId; }
    public uint GetShopUniqueId() { return m_nShopUniqueId; }

    public bool IsCanRepair(CObject_Item pItem)
    {

        if (pItem == null)
            return false;
        return true;
    }
    public bool IsCanBuy(CObject_Item pItem)
    {
        if (pItem == null)
            return false;
        return true;
    }

    public void SetCurrencyUnit(int nCurrencyUnit) { m_nCurrencyUnit = nCurrencyUnit; }
    public int GetCurrencyUnit() { return m_nCurrencyUnit; }

    public void SetSerialNum(int nSerialNum) { m_nSerialNum = nSerialNum; }
    public int GetSerialNum() { return m_nSerialNum; }

    public void SetbCanBuyMult(int bBuyMulti) { m_nBuyMulti = bBuyMulti; }
    public int GetbCanBuyMult() { return m_nBuyMulti; }

    public void SetCallBack(bool bCallBack) { m_bCallBack = bCallBack; }
    public bool GetCallBack() { return m_bCallBack; }

    public void SetShopType(int nType) { m_nShopType = nType; }
    public int GetShopType() { return m_nShopType; }

    public int GetShopNpcCamp() { return 1; }
}