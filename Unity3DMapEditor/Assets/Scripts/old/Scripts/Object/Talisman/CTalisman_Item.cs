using System.Collections.Generic;
using DBSystem;
//法宝属性表

//法宝品质类型
public enum ENUM_TAILSMAN_QUALITY_TYPE
{
    RED,
    WHITE,
    GREEN,
    BLUE,
    PURPLE,
    ORANGE,
}
//法宝属性类型
public enum ENUM_TAILSMAN_ATTRI_TYPE
{
    PHYSICS_ATTK,//增加角色的物理攻击力
    PHYSICS_DEF,//增加角色的法术攻击力
    MAGIC_ATTK,//增加角色的物理防御力
    MAGIC_DEF,//增加角色的法术防御力
    ENLARGE_HP,//增加角色HP的上限值
    HIT,//增加角色物理和法术攻击命中目标的概率
    CRITICAL,//增加角色所有攻击造成暴击的概率
    MISS,//增加角色闪避物理和法术攻击的概率
    CRITICAL_DEF,//减少角色被攻击时被暴击的几率
    IGNORE_PHYSICS_ATTK,//攻击时忽视对方一定百分值的武力防御
    IGNORE_MAGIC_ATTK,//攻击时忽视对方一定百分值的仙术防御
    PHYSICS_DAMAGE_INC,//增加角色一定百分值的武力攻击力
    MAGIC_DAMAGE_INC,//增加角色一定百分值的仙术攻击力
    IMMUNE_PHYSICS_ATTK,//免疫一定百分值的武力攻击
    IMMUNE_MAGIC_ATTK,//免疫一定百分值的仙术攻击
    REFLECT_DAMAGE,//反弹受到的伤害的一定百分值给攻击者
    PENETRATE_PHYSCICS_DEF,//减少目标武力防御的值
    PENETRATE_MAGIC_DEF//减少目标仙术防御的值
}

public enum ENUM_TALISMAN_ITEM_OWNER
{
    INVENTORY,
    EQUIPT
}
//合成规则
public class Talisman_Compound_Rule
{
    public static bool Judge(ENUM_TAILSMAN_QUALITY_TYPE type1, int lv1, ENUM_TAILSMAN_QUALITY_TYPE type2, int lv2)
    {
        bool result = true;
        if(type1 == type2)
        {
            if(lv1 <= lv2)
            {
                result = false;
            }
        }
        else
        {
            switch (type1)
            {
                case ENUM_TAILSMAN_QUALITY_TYPE.PURPLE:
                    if (type2 == ENUM_TAILSMAN_QUALITY_TYPE.ORANGE)
                    {
                        result = false;
                    }
                    break;
                case ENUM_TAILSMAN_QUALITY_TYPE.BLUE:
                    {
                        if (type2 == ENUM_TAILSMAN_QUALITY_TYPE.ORANGE || type2 == ENUM_TAILSMAN_QUALITY_TYPE.PURPLE)
                        {
                            result = false;
                        }
                    }
                    break;
                case ENUM_TAILSMAN_QUALITY_TYPE.GREEN:
                    {
                        if (type2 == ENUM_TAILSMAN_QUALITY_TYPE.RED || type2 == ENUM_TAILSMAN_QUALITY_TYPE.WHITE)
                        {
                            result = false;
                        }
                    }
                    break;
                case ENUM_TAILSMAN_QUALITY_TYPE.WHITE:
                    {
                        if (type2 == ENUM_TAILSMAN_QUALITY_TYPE.RED)
                        {
                            result = false;
                        }
                    }
                    break;
                case ENUM_TAILSMAN_QUALITY_TYPE.RED:
                    {
                        result = false;
                    }
                    break;
            }
        }
        return result;
    }
}

public class CTalisman_Item:CObject_Item
{

    private uint  curExp_;//当前经验
   // private int  nextExp_;//升级需要经验
    private _DBC_ITEM_TALISMAN define_;

    public CTalisman_Item(int id)
        :base(id)
    {
    }

    public override int GetItemPrice()
    {
        return (int)define_.uBasePrice;
    }

    public override ITEM_CLASS GetItemClass()
    {
        return (ITEM_CLASS)define_.nClass;
    }

    public override int GetItemTableQuality()
    {
        return define_.nQuality;
    }

    public override int GetItemTableType()
    {
        return define_.nType;
    }

    public override string GetName()
    {
        return define_.szName;
    }

    public override string GetDesc()
    {
        return define_.szDesc;
    }

    public override string GetIconName()
    {
        return define_.szIcon;
    }

    public override string GetExtraDesc()
    {
        return define_.szDesc;
    }

    public override int GetMaxOverLay()
    {
        return 1;
    }

    public int GetLV()
    {
        if (define_ != null)
        {
            return define_.nType;
        }
        return 0;
    }

    public override void SetExtraInfo(ref _ITEM pItemInfo)
    {
        base.SetExtraInfo(ref pItemInfo);
		if (pItemInfo.m_Talisman == null)
        {
            return;
        }
        curExp_ = pItemInfo.m_Talisman.m_uCurExp;
    }

    public void Reset()
    {
    }

    public _DBC_ITEM_TALISMAN Define
    {
        get{return define_;}
        set{define_ = value;}
    }

    public uint NextExp
    {
        get 
        {
            if (define_ != null)
            {
                return define_.uLevelrequireExp;
            }
            return 0;
        }
    }
    public uint  Exp
    {
        get{return curExp_;}
        set{curExp_ = value;}
    }

    //public string getDesc()
    //{
    //    switch(attriType_)
    //    {
    //        case ENUM_TAILSMAN_ATTRI_TYPE.PHYSICS_ATTK:
    //            return "武力攻击：" + AttriValue; 
    //        case ENUM_TAILSMAN_ATTRI_TYPE.PHYSICS_DEF:
    //            return "武力防御：" + AttriValue; 
    //        case ENUM_TAILSMAN_ATTRI_TYPE.MAGIC_ATTK:
    //            return "仙术攻击：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.MAGIC_DEF:
    //            return "仙术防御：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.ENLARGE_HP:
    //            return "血量：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.HIT:
    //            return "命中：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.CRITICAL:
    //            return "暴击：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.MISS:
    //            return "闪避：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.CRITICAL_DEF:
    //            return "抗暴：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.IGNORE_PHYSICS_ATTK:
    //            return "忽视武力防御：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.IGNORE_MAGIC_ATTK:
    //            return "忽视仙术防御：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.PHYSICS_DAMAGE_INC:
    //            return "武力伤害增幅：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.MAGIC_DAMAGE_INC:
    //            return "仙术伤害增幅：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.IMMUNE_PHYSICS_ATTK:
    //            return "武力免伤：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.IMMUNE_MAGIC_ATTK:
    //            return "仙术免伤：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.REFLECT_DAMAGE:
    //            return "伤害反射：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.PENETRATE_PHYSCICS_DEF:
    //            return "武防穿透：" + AttriValue;
    //        case ENUM_TAILSMAN_ATTRI_TYPE.PENETRATE_MAGIC_DEF:
    //            return "仙防穿透：" + AttriValue;
    //    }
    //    return "";
    //}

    public string getFontColor()
    {
        switch ((ENUM_TAILSMAN_QUALITY_TYPE)GetItemTableQuality())
        {
            case ENUM_TAILSMAN_QUALITY_TYPE.WHITE:
                return "RGBA(1.000, 1.000, 1.000, 1.000)";
            case ENUM_TAILSMAN_QUALITY_TYPE.GREEN:
                return "RGBA(0.000, 1.000, 0.000, 1.000)";
            case ENUM_TAILSMAN_QUALITY_TYPE.BLUE:
                return "RGBA(0.000, 0.000, 1.000, 1.000)";
            case ENUM_TAILSMAN_QUALITY_TYPE.RED:
                return "RGBA(1.000, 0.000, 0.000, 1.000)";
            case ENUM_TAILSMAN_QUALITY_TYPE.PURPLE:
                return "RGBA(0.615, 0.098, 0.678, 1.000)";
            case ENUM_TAILSMAN_QUALITY_TYPE.ORANGE:
                return "RGBA(0.901, 0.698, 0.133, 1.000)";
        }
        return "";
    }
}

public class CTalisman_Inventory
{
    public const int MAX_TALISMAN_ITEM_NUMBER = 24;
    private CTalisman_Item[] mItems = new CTalisman_Item[MAX_TALISMAN_ITEM_NUMBER];
    private int mUnLockCount = 0;
    public CTalisman_Inventory()
    {
       
    }

    public int UnLockCount
    {
        get { return mUnLockCount; }
        set { mUnLockCount = value; }
    }

    public void SetItem(short nBagIndex, CObject_Item pItem, bool bClearOld, bool bNew)
    {
        if (nBagIndex < 0 || nBagIndex >= MAX_TALISMAN_ITEM_NUMBER) return;
        //销毁原有数据
        if (mItems[nBagIndex] != null && bClearOld)
        {
            ObjectSystem.Instance.DestroyItem(mItems[nBagIndex]);
            mItems[nBagIndex] = null;
        }

        if (pItem != null)
        {
            pItem.TypeOwner = ITEM_OWNER.IO_TALISMAN_PACKET;
            pItem.PosIndex = nBagIndex;

            CObject_Item pItemLog = mItems[nBagIndex];
            if (pItemLog != null)
            {
                pItemLog.ItemSaveStatus = SAVE_ITEM_STATUS.NO_MORE_INFO;
            }

        }
        mItems[nBagIndex] = pItem as CTalisman_Item;
    }

    public void SetItemInfo(int nBagIndex, bool bEmpty, ref _ITEM pItem)
    {
        if (nBagIndex < 0 || nBagIndex >= MAX_TALISMAN_ITEM_NUMBER) return;

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
                if (mItems[nBagIndex].GetIdTable() != pItem.m_ItemIndex)
                {
                    mItems[nBagIndex].Define = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_TALISMAN>((int)DataBaseStruct.DBC_ITEM_TALISMAN).Search_Index_EQU((int)pItem.m_ItemIndex);
                }
                mItems[nBagIndex].SetExtraInfo(ref pItem);
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_TALISMANITEM, nBagIndex);
            }
        }
    }

    public void setItem(int index, ref _ITEM item)
    {
        if (index < 0 || index >= MAX_TALISMAN_ITEM_NUMBER) return;
        if (mItems[index] != null)
        {
            ObjectSystem.Instance.DestroyItem(mItems[index]);
            mItems[index] = null;
        }
        CTalisman_Item newItem = ObjectSystem.Instance.NewItem(item.m_ItemIndex) as CTalisman_Item;
        mItems[index] = newItem;
        newItem.SetGUID(item.m_ItemGUID.m_World, item.m_ItemGUID.m_Server, (uint)item.m_ItemGUID.m_Serial);
		newItem.TypeOwner = ITEM_OWNER.IO_TALISMAN_PACKET;
        newItem.PosIndex = (short)index;
        newItem.SetExtraInfo(ref item);
    }

    public CTalisman_Item getItem(int index)
    {
        if(index < 0 || index >= MAX_TALISMAN_ITEM_NUMBER)
            return null;
        return mItems[index];
    }

    public int FindFirstEmptyItemPlace()
    {
        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public CTalisman_Item GetSuitableCompoundItem(int lv,int quality)
    {
        for (int i = 0; i < mUnLockCount; i++)
        {
            if (mItems[i] != null)
            {
                if (Talisman_Compound_Rule.Judge((ENUM_TAILSMAN_QUALITY_TYPE)quality, 
                                                 lv, 
                                                 (ENUM_TAILSMAN_QUALITY_TYPE)mItems[i].GetItemTableQuality(), 
                                                 mItems[i].GetLV()))
                {
                    return mItems[i];
                }
            }
        }
        return null;
    }

    //public bool addItem(CTalisman_Item newItem)
    //{
    //    int emptyPlace = -1;

    //    if(emptyPlace == -1)
    //        return false;

    //    items_[emptyPlace] = newItem;
    //    return true;
    //}

    //public CTalisman_Item  getItem(int index)
    //{
    //    if(index < 0 || index >= items_.Count)
    //    {
    //        return null;
    //    }
    //    return items_[index];
    //}

    //public void clear()
    //{
    //    for(int i = 0;i < MAX_TALISMAN_ITEM_NUMBER;i++)
    //    {
    //        items_[i] = new CTalisman_Item();
    //        items_[i] .Reset();
    //    }
    //}
}

public class CTalisman_Equipments
{
        public const int MAX_TALISMAN_EQUIPT_NUMBER = 10;
        private CTalisman_Item[] mItems = new CTalisman_Item[MAX_TALISMAN_EQUIPT_NUMBER];
        private int mUnLockCount = 0;
        public int UnLockCount
        {
            get { return mUnLockCount; }
            set { mUnLockCount = value; }
        }
    
        public CTalisman_Equipments()
        {
            //for(int i = 0;i < MAX_TALISMAN_EQUIPT_NUMBER;i++)
            //{
            //    CTalisman_Item newItem = ObjectSystem.Instance.NewTalismanItem(-1);
            //    items_.Add(newItem);
            //    newItem.Owner = ENUM_TALISMAN_ITEM_OWNER.EQUIPT;
            //    newItem.Index = i;
            //    CActionSystem.Instance.UpdateAction_FromTalismanItem(newItem);
            //}
        }

        public int FindFirstEmptyItemPlace()
        {
            for (int i = 0; i < mUnLockCount; i++)
            {
                if(mItems[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetItem(int nEquiptIndex, CObject_Item pItem, bool bClearOld, bool bNew)
        {
            if (nEquiptIndex < 0 || nEquiptIndex >= MAX_TALISMAN_EQUIPT_NUMBER) return;
            //销毁原有数据
            if (mItems[nEquiptIndex] != null && bClearOld)
            {
                ObjectSystem.Instance.DestroyItem(mItems[nEquiptIndex]);
                mItems[nEquiptIndex] = null;
            }

            if (pItem != null)
            {
                pItem.TypeOwner = ITEM_OWNER.IO_TALISMAN_EQUIPT;
                pItem.PosIndex = (short)nEquiptIndex;

                CObject_Item pItemLog = mItems[nEquiptIndex];
                if (pItemLog != null)
                {
                    pItemLog.ItemSaveStatus = SAVE_ITEM_STATUS.NO_MORE_INFO;
                }

            }
            mItems[nEquiptIndex] = pItem as CTalisman_Item;
        }

        public void setItem(int index, ref _ITEM item)
        {
            if (index < 0 || index >= MAX_TALISMAN_EQUIPT_NUMBER) return;
            if (mItems[index] != null)
            {
                ObjectSystem.Instance.DestroyItem(mItems[index]);
                mItems[index] = null;
            }
            CTalisman_Item newItem = ObjectSystem.Instance.NewItem(item.m_ItemIndex) as CTalisman_Item;
            mItems[index] = newItem;
			newItem.PosIndex = (short)index;
			newItem.TypeOwner = ITEM_OWNER.IO_TALISMAN_EQUIPT;
            newItem.SetGUID(item.m_ItemGUID.m_World, item.m_ItemGUID.m_Server, (uint)item.m_ItemGUID.m_Serial);
            newItem.SetExtraInfo(ref item);
        }

        //public bool addItem(CTalisman_Item newItem)
        //{
        //    int emptyPlace = -1;

        //    if(emptyPlace == -1)
        //        return false;

        //    items_[emptyPlace] = newItem;
        //    return true;
        //}

        public CTalisman_Item getItem(int index)
        {
            if (index < 0 || index >= MAX_TALISMAN_EQUIPT_NUMBER)
            {
                return null;
            }
            return mItems[index];
        }

        //public void clear()
        //{
        //    for(int i = 0;i < MAX_TALISMAN_EQUIPT_NUMBER;i++)
        //    {
        //        items_[i] = new CTalisman_Item();
        //        items_[i] .Reset();
        //    }
        //}
}