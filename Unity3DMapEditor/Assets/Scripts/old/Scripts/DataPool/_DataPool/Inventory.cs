
using System.Collections.Generic;


public class Inventory
{
    public void Clear()
    {
        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
            {
                ObjectSystem.Instance.DestroyItem(mItems[i]);
                mItems[i] = null;
            }
        }

        mSetAskExtra.Clear();
    }
    // 增加一个是否新获得物品的标志 [8/31/2011 Sun]
    public void SetItem(short nBagIndex, CObject_Item pItem, bool bClearOld, bool bNew)
    {
        if (nBagIndex < 0 || nBagIndex >= GAMEDEFINE.MAX_BAG_SIZE) return;

        //销毁原有数据
        if (mItems[nBagIndex] != null && bClearOld)
        {
            ObjectSystem.Instance.DestroyItem(mItems[nBagIndex]);
            mItems[nBagIndex] = null;
        }

        if (pItem != null)
        {
            pItem.TypeOwner = ITEM_OWNER.IO_MYSELF_PACKET;
            pItem.PosIndex = nBagIndex;

            CObject_Item pItemLog = mItems[nBagIndex];
            if (pItemLog != null)
            {
                // 设置物品的保存状态为无详细信息。2006-3-24
                pItemLog.ItemSaveStatus = SAVE_ITEM_STATUS.NO_MORE_INFO;
            }

        }
        else
        {//原来有物品，移走

        }

        mItems[nBagIndex] = pItem;
		
        // 产生一个新道具的事件 [8/31/2011 Sun]
        if (bNew == true && pItem != null)
        {
			CObject_Item_Equip item = pItem as CObject_Item_Equip;
			if(item != null)
			{
				int point = (int)item.GetItemType();
				if(point == 8)
				{
					CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_ITEM,item.GetID());
				}
			}
        }
    }

    //得到背包里面的药品
    public CObject_Item GetMedicialItem()
    {
        int i = 0;
        for (i = 0; i < GAMEDEFINE.MAX_BAG_SIZE; i++)
        {
			if(mItems[i] != null)
			{
				uint id = (uint)mItems[i].GetIdTable();
				if(id >= 30001001 && id <= 30003005)
				{
					break;
				}
			}
			else
			{
				continue;
			}
        }

        if (i == GAMEDEFINE.MAX_BAG_SIZE)
            return null;
		
		return mItems[i];
    }
    //重新设置物品属性
    public void SetItemInfo(int nBagIndex, bool bEmpty, ref _ITEM pItem)
    {
        if (nBagIndex >= GAMEDEFINE.MAX_BAG_SIZE) return;

        //物品消失
        if (bEmpty)
        {
            if (mItems[nBagIndex] != null)
            {
                ObjectSystem.Instance.DestroyItem(mItems[nBagIndex]);
                mItems[nBagIndex] = null;
            }
        }
        else//设置物品详细信息
        {
            if (mItems[nBagIndex] != null)
            {

                if (mItems[nBagIndex].TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
                {
                    CObject_Item Item = mItems[nBagIndex];
                    Item.ItemSaveStatus = SAVE_ITEM_STATUS.GET_MORE_INFO;
                }
                // 判断是否增加item [9/20/2011 Sun]
                if (pItem.GetItemCount() > mItems[nBagIndex].GetNumber())
                {
					CObject_Item_Equip item = mItems[nBagIndex] as CObject_Item_Equip;
					if(item != null)
					{
						int point = (int)item.GetItemType();
						if(point == 8)
						{
							CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_ITEM,mItems[nBagIndex].GetID());
						}
							
					}
					if(mItems[nBagIndex].GetIdTable() == 30001001)
					{
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_ITEM,mItems[nBagIndex].GetID());
					}
                }
                mItems[nBagIndex].SetExtraInfo(ref pItem);
            }

            mSetAskExtra.Remove(nBagIndex);
        }
    }
    //取得装备
    public CObject_Item GetItem(int nBagIndex)
    {
        if (nBagIndex < 0 || nBagIndex >= GAMEDEFINE.MAX_BAG_SIZE) return null;

        return mItems[nBagIndex];
    }
    //取得道具，根据客户端ID
    public CObject_Item GetItemByID(int nID)
    {
        foreach (CObject_Item item in mItems)
        {
            if (item != null && item.GetID() == nID)
                return item;
        }
        return null;
    }
    //通过GUID取得装备位置
    public int GetItemIndexByGUID(long ItemGUID)
    {
        //Todo:
        return 0;
    }
    // 通过表ID来获得装备位置 [7/19/2011 ivan edit]
    public int GetItemIndexByIDTable(int idTable)
    {
        //Todo:
        return 0;
    }
    //通过ID取得物品位置
    public int GetItemIndexByID(int nID)
    {
        int index;
        for (index = 0; index < GAMEDEFINE.MAX_BAG_SIZE; index++)
        {
            if (mItems[index].GetID() == nID)
                break;
        }
        if (index == GAMEDEFINE.MAX_BAG_SIZE)
        {
            return -1;
        }
        return index;
    }
    //请求装备详细信息
    public void AskExtraInfo(int nBagIndex) { }
    //通过table id取得装备的数量
    public int CountItemByIDTable(int ItemGUID)
    {
        int index, count;
        count = 0;
        for (index = 0; index < GAMEDEFINE.MAX_BAG_SIZE; index++)
        {
            if (mItems[index] == null)
                continue;
            if (mItems[index].GetIdTable() == ItemGUID)
            {
                count += mItems[index].Num;
            }
        }

        return count;
    }
	
	//通过table id取得宝石的数量
	public int CountGemByIDTable(int GemGUID)
	{
		int index,count;
		count = 0;
		for(index = 0; index < GAMEDEFINE.MAX_BAG_SIZE; index++)
		{
			if(mItems[index] == null)
				continue;
			if(mItems[index].GetIdTable() == GemGUID)
			{
				count++;
			}
		}
		if(index == GAMEDEFINE.MAX_BAG_SIZE)
			return count;
		
		return count;
	}

    public int FindFirstEmptyPlace()
    {
        for (int index = 0; index < GAMEDEFINE.MAX_BAG_SIZE; index++)
        {
            if (mItems[index] == null)
                return index;
        }
        return -1;
    }

    private CObject_Item[] mItems = new CObject_Item[GAMEDEFINE.MAX_BAG_SIZE];
    private HashSet<int> mSetAskExtra = new HashSet<int>();
}