
using System.Collections.Generic;
/// <summary>
/// 银行，原CDataPool数据接口
/// </summary>
public class Bank
{
    //主角银行物品列表
    private List<CObject_Item> m_listUserBank;
    //当前银行中已经打开的位置
    private int m_CurBankEndIndex;
    //当前银行中钱数
    private int m_CurBankMoney;
    //当前银行元宝数
    private int m_CurBankRMB;
    //当前银行NPC的ID
    private int m_nBankNpcID;

    public Bank()
    {
        m_listUserBank = new List<CObject_Item>();
        for (int i = 0; i < GAMEDEFINE.MAX_BANK_SIZE; i++ )
        {
            m_listUserBank.Add(null);
        }
    }

    public void Clear()
    {
        for(int i=0; i<GAMEDEFINE.MAX_BANK_SIZE; i++)
	    {
		    if(m_listUserBank[i] != null)
		    {
			    ObjectSystem.Instance.DestroyItem(m_listUserBank[i]);
			    m_listUserBank[i] = null;
		    }
	    }
    }
    public void SetItem(int nBankIndex, CObject_Item pItem, bool bClearOld)
    {
        if(	nBankIndex < 0 || nBankIndex>=GAMEDEFINE.MAX_BANK_SIZE) return;
	    //销毁原有数据
	    if(m_listUserBank[nBankIndex] != null && bClearOld)
	    {
		    ObjectSystem.Instance.DestroyItem(m_listUserBank[nBankIndex]);
		    m_listUserBank[nBankIndex] = null;
	    }

	    if(pItem != null)
	    {
		    pItem.TypeOwner = ITEM_OWNER.IO_MYSELF_BANK;
		    pItem.PosIndex  = (short)nBankIndex;
	    }

	    m_listUserBank[nBankIndex] = pItem;
    }
    public CObject_Item GetItem(int nBankIndex)
    {
        if (nBankIndex < 0 || nBankIndex >= GAMEDEFINE.MAX_BANK_SIZE) 
            return null;
        return m_listUserBank[nBankIndex];
    }
	public 	void				SetBankEndIndex(int endindex)	{m_CurBankEndIndex = endindex;}
	public 	int					GetBankEndIndex(){	return m_CurBankEndIndex;}
	public 	void				SetBankMoney(int Money){m_CurBankMoney = Money; }
	public 	int					GetBankMoney(){return m_CurBankMoney;}
	public 	void				SetBankRMB(int Money){m_CurBankRMB = Money; }
	public 	int					GetBankRMB(){return m_CurBankRMB;}
	public 	void				SetItemExtraInfo(int nBankIndex, bool bEmpty, ref _ITEM pItem)
    {
        if(nBankIndex >= GAMEDEFINE.MAX_BANK_SIZE) return;

	    if(bEmpty)
	    {
		    if(m_listUserBank[nBankIndex] != null)
		    {
			    ObjectSystem.Instance.DestroyItem(m_listUserBank[nBankIndex]);
			    m_listUserBank[nBankIndex] = null;
		    }
	    }
	    else
	    {
		    if(m_listUserBank[nBankIndex] != null)
			    m_listUserBank[nBankIndex].SetExtraInfo( ref pItem);
	    }
    }
    //是否解封格子
    public bool IsValid(int nIndex)
    {
        return nIndex <= m_CurBankEndIndex;
    }
    // 查询银行nIndex编号的租赁箱子是不是有空格
    public bool IsEmpty(int nIndex)
    {
        int nBeginIndex = 0;
        int nEndIndex = GetBankEndIndex();
        //if (nIndex == 1)
        //{
        //    nBeginIndex = GAMEDEFINE.RENTBOX1_START_INDEX;
        //    nEndIndex = GAMEDEFINE.RENTBOX2_START_INDEX;
        //}
        //else if (nIndex == 2)
        //{
        //    nBeginIndex = GAMEDEFINE.RENTBOX2_START_INDEX;
        //    nEndIndex = GAMEDEFINE.RENTBOX3_START_INDEX;
        //}
        //else if (nIndex == 3)
        //{
        //    nBeginIndex = GAMEDEFINE.RENTBOX3_START_INDEX;
        //    nEndIndex = GAMEDEFINE.RENTBOX4_START_INDEX;
        //}
        //else if (nIndex == 4)
        //{
        //    nBeginIndex = GAMEDEFINE.RENTBOX4_START_INDEX;
        //    nEndIndex = GAMEDEFINE.RENTBOX5_START_INDEX;
        //}
        //else if (nIndex == 5)
        //{
        //    nBeginIndex = GAMEDEFINE.RENTBOX5_START_INDEX;
        //    nEndIndex = GAMEDEFINE.MAX_BANK_SIZE;
        //}

        for (int i = nBeginIndex; i < nEndIndex; i++)
        {
            if (m_listUserBank[i] == null)
            {
                return true;
            }
        }
        return false;
    }
	public  void				SetNpcId(int nNpcId){m_nBankNpcID = nNpcId;}
	public  int					GetNpcId(){return m_nBankNpcID;}
}