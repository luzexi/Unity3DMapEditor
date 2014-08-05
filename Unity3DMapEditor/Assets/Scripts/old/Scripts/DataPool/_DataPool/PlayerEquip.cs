
using System.Collections.Generic;

public class PlayerEquip: IPlayerEquip
{
    //////////////////////////////////////////////////////////////////////////
    //from IPlayerEquip
    //清空
    public void Clear()
    {
        	for(int i=0; i<(int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
	        {
		        if(mPlayerEquip[i] != null)
		        {
			        ObjectSystem.Instance.DestroyItem(mPlayerEquip[i]);
                    mPlayerEquip[i] = null;
		        }
	        }
    }
    //设置装备
    public void SetItem(short ptEquip, CObject_Item item, bool bClearOld)
    {
        if (ptEquip < 0 || ptEquip >= (int)HUMAN_EQUIP.HEQUIP_NUMBER)
            return;

        if (mPlayerEquip[ptEquip] != null && bClearOld)
        {
            ObjectSystem.Instance.DestroyItem(mPlayerEquip[ptEquip]);
        }
        if (item != null)
        {
            HUMAN_EQUIP pointEquip = ((CObject_Item_Equip)item).GetItemType();
            if (pointEquip != (HUMAN_EQUIP)ptEquip) return;

            // 设置所属
            item.TypeOwner = ITEM_OWNER.IO_MYSELF_EQUIP;

            // 设置装备在action item 中的位置.
            item.PosIndex = ptEquip;
            mPlayerEquip[ptEquip] = item;

        }
        else
        {
            // 在装备点清空数据
            mPlayerEquip[ptEquip] = null;
        }
    }
    //获取装备
    public CObject_Item GetItem(int ptEquip)
    {
        if (ptEquip < 0 || ptEquip > (int)HUMAN_EQUIP.HEQUIP_NUMBER)
            return null;
        return mPlayerEquip[ptEquip];
    }
    //套装是否组合成功，返回套装编号，否则返回-1
    public int IsUnion()
    {
        /*
		会出现套装的装备
	
		HEQUIP_CAP			=1,		//帽子	
		HEQUIP_ARMOR		=2,		//盔甲	
		HEQUIP_CUFF			=3,		//护腕	
		HEQUIP_BOOT			=4,		//鞋	
	    */

        int nSetID = -1;
        for (int i = (int)HUMAN_EQUIP.HEQUIP_CAP; i <= (int)HUMAN_EQUIP.HEQUIP_BOOT; i++)
        {
            //缺失
            if (mPlayerEquip[i] == null) return -1;

            //出现非套装装备
            if (((CObject_Item_Equip)(mPlayerEquip[i])).GetSetID() <= 0) return -1;

            //第一个
            if (nSetID <= 0)
            {
                nSetID = ((CObject_Item_Equip)(mPlayerEquip[i])).GetSetID();
            }
            else
            {
                //不同套装号的装备
                if (((CObject_Item_Equip)(mPlayerEquip[i])).GetSetID() != nSetID) return -1;
            }
        }

        return nSetID;
    }


    private CObject_Item[] mPlayerEquip = new CObject_Item[(int)HUMAN_EQUIP.HEQUIP_NUMBER];
}