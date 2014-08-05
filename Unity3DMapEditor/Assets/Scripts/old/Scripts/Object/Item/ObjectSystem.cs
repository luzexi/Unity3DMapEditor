using System;
using System.Collections.Generic;
using DBSystem;
using DBC;

public class Rule_t
{
   public bool bCanDrop       = true;		// 是否可丢弃
   public bool bCanOverLay    = true;	// 是否可重叠
   public bool bCanQuick = true;		// 是否可放入快捷栏
   public bool bCanSale = true;		// 是否可以出售给NPC商店
   public bool bCanExchange = true;	// 是否可以交易
   public bool bCanUse = true;		// 是否可以使用
   public bool bPickBind = true;		// 是否是拾取邦定
   public bool bEquipBind = true;		// 是否是装备邦定
   public bool bUnique = true;		// 是否是唯一

};
//需要在ui上显示的属性
public struct ITEMATTRIBUTE
{
   public const string  ITEMATTRIBUTE_NAME = "name";			//物品名
   public const string  ITEMATTRIBUTE_LEVEL = "level";			//物品等级
   public const string  ITEMATTRIBUTE_DAM = "dam";			//损坏度
   public const string  ITEMATTRIBUTE_DAMMAX = "dam_max";		//损坏度最大值
   public const string  ITEMATTRIBUTE_PRICE = "price";			//价格
   public const string  ITEMATTRIBUTE_REPAIRNUM = "repair_num";		//修理次数
   public const string  ITEMATTRIBUTE_WHITE_ATT = "white_all";		//所有白色属性
   public const string  ITEMATTRIBUTE_BLUE_ATT = "blue_all";		//所有蓝色属性
   public const string  ITEMATTRIBUTE_GREEN_ATT = "green_all";		//所有绿色属性
   public const string  ITEMATTRIBUTE_PRODUCER = "producer";		//作者
   public const string  ITEMATTRIBUTE_DESC = "desc";			//详细解释
}

// namespace SGWEB
// {
	public abstract class CObject_Item
    {
      
        public int Num
        {
            get { return m_nNum; }
            set { m_nNum = value; }
        }
        public ITEM_OWNER TypeOwner
        {
            get { return m_typeOwner; }
            set { m_typeOwner = value; }
        }
        public short PosIndex
        {
            get { return m_nPosIndex; }
            set { m_nPosIndex = value; }
        }
        public SAVE_ITEM_STATUS ItemSaveStatus
        {
            get { return m_nItemSaveStatus; }
            set { m_nItemSaveStatus = value; }
        }

        // 基础数据
        //从服务器传来的数据标示(用于和服务器通讯)
        ITEM_GUID m_ID;
        ITEM_OWNER m_typeOwner;	//归属的类型
        int m_nNum;			//数量
        int m_idClient;		//客户端ID;
        short m_nPosIndex;	//所在的位置编号，针对处于包裹，箱子，窗口等需要用索引定位的情况
        int m_nMax;
        bool m_bLocked;
        SAVE_ITEM_STATUS m_nItemSaveStatus;	//客户端物品的保存状态。 
        //
        // 0 ： 无详细信息
        // 1 ： 得到详细信息。 

        //-----------------------------------------------------
        //在资源表中的定义ID
        // 20000000 - 49999999		普通道具：包括材料、药品、卷轴、任务物品等
        // 50000000 - 59999999		宝石
        // 60000000 - 69999999		藏宝图
        // 10200000 - 10299999		蓝色装备
        // 10400000 - 10499999		绿色装备
        // 10100000 - 10199999		白色装备
        public int m_idTable;
        //物品的详细编号 = ((nClass*100+nQuality)*100+nType)*1000+nIndex
        public int m_nParticularID;
        _ITEM m_ExtraInfoItem;
        bool m_bByProtect;
        //制作者名称
	    byte[]		m_Creator= new byte[ItemDefine.MAX_ITEM_CREATOR_NAME];

        Rule_t m_Rule = new Rule_t();

        public CObject_Item(int id)
        {
            m_idClient = id;
        }

        public int GetID() { return m_idClient; }
        public void  ChangeClientID( int id)
        { 
	        m_idClient = id; 
        }
        
        public void SetGUID(ushort idWorld, ushort idServer, uint idSerial)
        {
	        m_ID.m_idWorld	= idWorld;
	        m_ID.m_idServer	= idServer;
	        m_ID.m_uSerial	= idSerial;
        }
        public void GetGUID(ref ushort idWorld, ref ushort idServer, ref uint idSerial)
        {
            idWorld = m_ID.m_idWorld;
            idServer = m_ID.m_idServer;
            idSerial = m_ID.m_uSerial;
        }
        public System.UInt64 GetGUID()
        {
            return m_ID.IdUnion;
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        //-- super tool tip 使用
        //-- 
        // 得到玩家使用这个物品需要的等级
       public virtual int GetNeedLevel()
       {
           return -1;
       }

        // 得到物品耐久度
       public virtual int GetItemDur()
       {
          return -1;
       }

        // 得到物品最大耐久度
       public virtual int GetItemMaxDur()
       {
           return -1;
       }

        // 得到物品的修理次数 
       public virtual int GetItemRepairCount()
       {
           return -1;
       }

        //  得到物品的强化等级  [7/28/2011 edit by ZL]
       public virtual int GetStrengthLevel()
       {
		   //TODO:
           return -1;
       }

        // 得到物品的品级 [9/19/2011 edit by ZL]
       public virtual int GetPinJi()
       {
           //TODO:
           return -1;
       }

        // 得到物品的品级值 [9/19/2011 edit by ZL]
       public virtual int GetPinJiValue()
       {
           //TODO:
           return -1;
       }

        // 得到物品的绑定信息 
       public virtual int GetItemBindInfo()
       {
           //TODO:
           return -1;
       }

        // 得到物品卖给npc的价格
       public virtual int GetItemPrice()
       {
           //TODO:
           return -1;
       }

        // 得到物品的制作人
       public virtual string GetManufacturer()
       {
           return "";
       }

        // 得到白色属性
       public virtual string GetBaseWhiteAttrInfo()
       {
           return "";
       }

       //道具的类别
       public abstract ITEM_CLASS GetItemClass();
        // 物品重新分类以后， 需要 getitemClass ItemQuality 和GetItemType（）；才能确定。
	    // 2006-3-30;
       public virtual int GetItemTableQuality()
       {
           return 0;
       }
       public virtual int GetItemTableType()
       {
           //TODO:
           return 0;
       }
	   public virtual int  GetItemTableIndex()
       {
           //TODO:
           return 0;
       }

       public virtual string            GetName() { return ""; }
       public virtual string            GetDesc() { return ""; }
       public virtual string            GetIconName() { return ""; }

       public virtual int			GetParticularID() { return m_nParticularID; }
	    //详细解释(可能需要服务器)
       public virtual string GetExtraDesc()
       {
           return "";
       }
	    //设置详细解释
       public virtual void SetExtraInfo(ref _ITEM pItemInfo)
       {
	        if(pItemInfo != null)
	        {
                m_ExtraInfoItem = pItemInfo;
	        }
       }
	    //设置物品的制作者
       public virtual void SetManufacturer(ref _ITEM pItemInfo)
       {
           if (pItemInfo != null && pItemInfo.GetCreatorVar())
           {
               Array.Copy(pItemInfo.m_Creator, m_Creator, ItemDefine.MAX_ITEM_CREATOR_NAME);
           }
       }
	    //数量
	   public virtual void			SetNumber(int nNum) { m_nNum = nNum; }
	   public virtual int			GetNumber()  { return m_nNum; }
	    //获得物品的最大叠加数量
       public abstract int GetMaxOverLay();

       //最大数量,暂时用于有限商品得显示
	   public virtual void			SetMax(int nNum){m_nMax = nNum;}
	   public virtual int			GetMax() {return m_nMax;}

        //归属
	   public virtual void 		    SetTypeOwner(ITEM_OWNER owner) { m_typeOwner = owner; }
	   public virtual ITEM_OWNER	GetTypeOwner()  { return m_typeOwner; }

	    //所在容器的索引
       public virtual short GetPosIndex() { return m_nPosIndex; }
	    public virtual void			SetPosIndex(short nPosIndex) { m_nPosIndex = nPosIndex; }

        public virtual int          GetIdTable(){ return m_idTable; }

        	//查询逻辑属性,一般用于脚本调用
	    public virtual string		GetAttributeValue(string szValueName)
        {
            if (szValueName==null) return "";

            if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_NAME) == 0)
            {
                return GetName();
            }
            else
            {
                return "";
            }
        }
        public virtual string GetWhiteAttribute(int nIndex)
        {
            return "";
        }
        public virtual int GetWhiteAttributeCount()
        {
            return 0;
        }

	    //克隆详细信息
	    public virtual void Clone(CObject_Item  pItemSource)
        {
            //克隆GUID
	        ushort idWorld		= 0;
	        ushort idServer		= 0;
	        uint idSerail		= 0;
	        pItemSource.GetGUID(ref idWorld, ref idServer, ref idSerail);
	        SetGUID(idWorld, idServer, idSerail);
	        //克隆制造者
	        string pManufacturer = pItemSource.GetManufacturer();
	        //  [11/8/2010 Sun]
	        if(pManufacturer != null)
            {
                m_Creator = System.Text.Encoding.ASCII.GetBytes(pManufacturer);
            }

	        //克隆物品的额外信息
	        if(pItemSource.GetItemExtraInfoItem() != null)
	        {
		        if(m_ExtraInfoItem != null) 
		        {
			       m_ExtraInfoItem = null;
		        }
		        m_ExtraInfoItem = pItemSource.m_ExtraInfoItem;
	        }
        }

	    //物品是否被锁定 =TURE锁定
	    public virtual bool			IsLocked(){return m_bLocked;}
	    public virtual void			SetLock(bool bLock){m_bLocked = bLock;}
        public virtual bool isLocked
        {
            get { return m_bLocked; }
            set {
                if (TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, -1);
                else if (TypeOwner == ITEM_OWNER.IO_MYSELF_EQUIP)
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_EQUIP, -1);
                m_bLocked = value; 
            }
        }

        public Rule_t RULE
        {
            set { m_Rule = value; }
            get { return m_Rule; }
        }
	    //物品规则验证
        public virtual bool Rule(ITEM_RULE ruletype)
        {
            switch (ruletype)
            {
             case ITEM_RULE.RULE_DROP:
             {
                 return m_Rule.bCanDrop;
             }
             case ITEM_RULE.RULE_OVERLAY:
                    {
                        return m_Rule.bCanOverLay;
                    }

             case ITEM_RULE.RULE_QUICK:
                    {
                        return m_Rule.bCanQuick;
                    }

             case ITEM_RULE.RULE_SALE:
                    {
                        return m_Rule.bCanSale;
                    }

             case ITEM_RULE.RULE_EXCHANGE:
                    {
                        return m_Rule.bCanExchange;
                    }

             case ITEM_RULE.RULE_USE:
                    {
                        return m_Rule.bCanUse;
                    }

             case ITEM_RULE.RULE_PICKBIND:
                    {
                        return m_Rule.bPickBind;
                    }

             case ITEM_RULE.RULE_EQUIPBIND:
                    {
                        return m_Rule.bEquipBind;
                    }

             case ITEM_RULE.RULE_UNIQUE:
                    {
                        return m_Rule.bUnique;
                    }
                default:
                    break;
            }
            return true;
        }
        public virtual string RuleFailMessage(ITEM_RULE ruletype)
        {

            switch (ruletype)
            {
                case ITEM_RULE.RULE_DROP:
                    {
                        return "此物品不可以丢弃";
                    }
                case ITEM_RULE.RULE_OVERLAY:
                    {
                        return "此物品不可以叠加";
                    }

                case ITEM_RULE.RULE_QUICK:
                    {
                        return "此物品不可以放入快捷栏";
                    }

                case ITEM_RULE.RULE_SALE:
                    {
                        return "此物品不可以卖出";
                    }

                case ITEM_RULE.RULE_EXCHANGE:
                    {
                        return "此物品不可以交易";
                    }

                case ITEM_RULE.RULE_USE:
                    {
                        return "此物品不可以使用";
                    }

                case ITEM_RULE.RULE_PICKBIND:
                    {
                        return "此物品已绑定";
                    }

                case ITEM_RULE.RULE_EQUIPBIND:
                    {
                        return "此物品已绑定";
                    }

                case ITEM_RULE.RULE_UNIQUE:
                    {
                        return "";
                    }
                default:
                    break;
            }

            return "";
        }

	    //得到物品使用的目标类型
	    public virtual int			GetItemTargetType()
        {
            //TODO:
            return (int)ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_INVALID;
        }

	    // 得到物品的类型描述2006-4-28
	    public virtual string		GetItemTableTypeDesc()
        {
            //Todo:
            return "";
        }



	    //物品是否有详细信息2006－3－30
	    public bool IsItemHasDetailInfo()
        {
            if (SAVE_ITEM_STATUS.GET_MORE_INFO == m_nItemSaveStatus)
            {
                return false;
            }

            return true;
        }

	    public virtual bool			GetByProtect(){return m_bByProtect;}
	    public virtual void			SetByProtect(bool bByProtect){m_bByProtect = bByProtect;}
	
	    // 得到物品档次描述信息
	    public virtual int	GetItemLevelDesc(){	return 1;}

	    public virtual _ITEM GetItemExtraInfoItem() {return m_ExtraInfoItem;}

        public virtual int GetItemTransferString(string strResult)
        {
            //TODO:
            return 0;
        }
        public virtual void SetExtraInfoFromString(string strIn, int nLen)
        { 
        }
	    public virtual string	GetSysColour(){return ConvertSysColour(-1);}		//默认返回白色
	    //virtual INT				GetNeedJob(VOID) const	{ return -1; }

	    public virtual string GenHypeLinkString()
        {
             /*
		        format: #aB{playID,itemID,##z}name#aE
		        增加一个静态Id，用于区别hypelink，否则{}里内容相同时会出现两个link连体现象
	        */
	        return "#aB{" + GetHypeLinkID() + "," + GetIdTable() + ",##z}[" + GetName() + "]#aE";
        }
        static int linkID = 0;
	    public static int GetHypeLinkID()
        {
            return linkID++;
        }

        //public virtual void SetExtraInfo(ref _ITEM _item)
        //{
        //    m_ExtraInfoItem = _item;
        //}

        string ConvertSysColour(int nColor)
        {
            //DBC_DEFINEHANDLE(s_pSystemColorDBC, DBC_SYSTEM_COLOR);
            //const _DBC_SYSTEM_COLOR* pSysColor = (const _DBC_SYSTEM_COLOR*)s_pSystemColorDBC->Search_Index_EQU(nColor);

            //static STRING strColor;
            //if(pSysColor)
            //{
            //    //字体颜色
            //    if(strlen(pSysColor->strFontColor) == 6)
            //    {
            //        strColor += "#c";
            //        strColor += pSysColor->strFontColor;
            //    }

            //    //扩边颜色
            //    if(strlen(pSysColor->strExtenColor) == 6)
            //    {
            //        strColor += "#e";
            //        strColor += pSysColor->strExtenColor;
            //    }
            //}
            //else
            //{
            //    strColor = "#cFFFFFF";	//缺省白色
            //}

            return "#cFFFFFF";
        }
    }

    public class ObjectSystem
    {
        private static readonly ObjectSystem instance = new ObjectSystem();
        public static ObjectSystem Instance
        {
            get { return instance; }
        }

        //DBC
        public static COMMON_DBC<_DBC_ITEM_EQUIP_BASE> EquipDBC
        {
            get
            {
                if (s_allEquip == null)
                    s_allEquip = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_EQUIP_BASE>((int)DataBaseStruct.DBC_ITEM_EQUIP_BASE);
                return s_allEquip;
            }
                
        }
        private static COMMON_DBC<_DBC_ITEM_EQUIP_BASE> s_allEquip;

        //宝石
        public static COMMON_DBC<_DBC_ITEM_GEM> GemDBC
        {
            get
            {
                if (s_Gems == null)
                    s_Gems = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_GEM>((int)DataBaseStruct.DBC_ITEM_GEM);
                return s_Gems;
            }
        }
        private static COMMON_DBC<_DBC_ITEM_GEM> s_Gems;

        private static COMMON_DBC<_DBC_ITEM_SYMBOL> s_Symbols;
        public static COMMON_DBC<_DBC_ITEM_SYMBOL> SymbolDBC
        {
            get
            {
                if (s_Symbols == null)
                {
                    s_Symbols = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_SYMBOL>((int)DataBaseStruct.DBC_ITEM_SYMBOL);
                }
                return s_Symbols;
            }
        }
        //装备强化
        public static COMMON_DBC<_DBC_ITEM_ENHANCE> EquipEnchanceDBC
        {
            get {
                if (s_EquipEnhance == null)
                    s_EquipEnhance = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_ENHANCE>((int)DataBaseStruct.DBC_ITEM_ENHANCE);
                return s_EquipEnhance;
            }
        }
        private static COMMON_DBC<_DBC_ITEM_ENHANCE> s_EquipEnhance;
        //强化消耗系数
        public static COMMON_DBC<_DBC_ITEM_ENCHANCE_RATE> EquipEnchanceRateDBC
        {
            get
            {
                if (s_EquipEnchanceRate == null)
                    s_EquipEnchanceRate = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_ENCHANCE_RATE>((int)DataBaseStruct.DBC_ITEM_ENCHANCE_RATE);
                return s_EquipEnchanceRate;
            }
        }
        private static COMMON_DBC<_DBC_ITEM_ENCHANCE_RATE> s_EquipEnchanceRate;
  
        //装备升档消耗
        public static COMMON_DBC<_DBC_ITEM_UPLEVEL> EquipUpLevelDBC
        {
            get
            {
                if (s_EquipUpLevel == null)
                    s_EquipUpLevel = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_UPLEVEL>((int)DataBaseStruct.DBC_ITEM_UPLEVEL);
                return s_EquipUpLevel;
            }
        }
        private static COMMON_DBC<_DBC_ITEM_UPLEVEL> s_EquipUpLevel;

        public static COMMON_DBC<_DBC_ITEM_RULE> ItemRuleDBC
        {
           get
           {
               if (s_ItemRule == null)
                   s_ItemRule = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_RULE>((int)DataBaseStruct.DBC_ITEM_RULE);
               return s_ItemRule;
           }
           
        }
        private static COMMON_DBC<_DBC_ITEM_RULE> s_ItemRule;


        public ObjectSystem()
        {
            allItems = new Dictionary<int, CObject_Item>();
        }

        public bool IsWhiteEquip(uint idTable)
        {
            if (idTable >= 10000000 && idTable <= 10099999)			// 白色装备
                return true;

            return false;
        }
        public bool IsGreenEquip(uint idTable)
        {
            if (idTable >= 10100000 && idTable <= 10199999)			// 绿色装备
                return true;

            return false;
        }
        public bool IsBlueEquip(uint idTable)
        {
            if (idTable >= 10200000 && idTable <= 10299999)			//蓝色装备
                return true;

            return false;
        }
        public bool IsPurpleEquip(uint idTable)
        {
            if (idTable >= 10300000 && idTable <= 10399999)			// 紫色装备
                return true;

            return false;
        }

        // 现在所有装备共用一张装备表 [1/10/2011 ivan edit]
        public bool IsEquip(int idTable)
        {
	        if(idTable >= 10000000 && idTable <= 10499999)	// 装备
                return true;

            return false;
        }

        // 原材料 + 食品 + 药瓶
        public bool IsMedicItem(uint idTable)
        {
	        if(idTable >= 20000000 && idTable <= 49999999)	// 原材料 + 食品 + 药瓶
                return true;

            return false;
        }
        public bool IsGemItem(int idTable)
        {
            if (idTable >= 50000000 && idTable <= 59999999)	// 宝石
                return true;

            return false;
        }
        public bool isSymbolItem(uint idTable)
        {
            if (idTable >= 90000000 && idTable <= 99999999)	// 符印
                return true;
            return false;
        }

        public bool isTalismanItem(uint idTable)
        {
            if (idTable >= 70000000 && idTable <= 79999999)	// 法宝
                return true;
            return false;
        }

        public CObject_Item NewItem(uint idTable)
        {
            CObject_Item newItem = null;
            if (IsWhiteEquip(idTable))
            {
		        //搜索纪录
		        _DBC_ITEM_EQUIP_BASE pEquip = EquipDBC.Search_Index_EQU((int)idTable);
		        if(pEquip==null) return null;

                newItem = new CObject_Item_Equip(CreateID());
                ((CObject_Item_Equip)newItem).AsWhiteEquip(ref pEquip);

		        //物品规则
		        itemRule(pEquip, newItem);
            }
            else if (IsGreenEquip(idTable))
            {
                //搜索纪录
                _DBC_ITEM_EQUIP_BASE pEquip = EquipDBC.Search_Index_EQU((int)idTable);
                if (pEquip == null) return null;

                newItem = new CObject_Item_Equip(CreateID());
                ((CObject_Item_Equip)newItem).AsGreenEquip(ref pEquip);

                //物品规则
                itemRule(pEquip, newItem);
            }
            else if(IsBlueEquip(idTable))
            {
                //搜索纪录
                _DBC_ITEM_EQUIP_BASE pEquip = EquipDBC.Search_Index_EQU((int)idTable);
                if (pEquip == null) return null;

                newItem = new CObject_Item_Equip(CreateID());
                ((CObject_Item_Equip)newItem).AsBlueEquip(ref pEquip);

                //物品规则
                itemRule(pEquip, newItem);
            }
            else if(IsPurpleEquip(idTable))
            {
                //搜索纪录
                _DBC_ITEM_EQUIP_BASE pEquip = EquipDBC.Search_Index_EQU((int)idTable);
                if (pEquip == null) return null;

                newItem = new CObject_Item_Equip(CreateID());
                ((CObject_Item_Equip)newItem).AsPurpleEquip(ref pEquip);

                //物品规则
                itemRule(pEquip, newItem);
            }
            else if (IsMedicItem(idTable))
            {
                COMMON_DBC<_DBC_ITEM_MEDIC> allMedic = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_MEDIC>((int)DataBaseStruct.DBC_ITEM_MEDIC);
                if(allMedic != null)
                {
                    _DBC_ITEM_MEDIC pMedic = allMedic.Search_Index_EQU((int)idTable);

                    newItem = new CObject_Item_Medicine(CreateID());
                    ((CObject_Item_Medicine)newItem).AsMedicine(pMedic);

                    itemRule(pMedic, newItem);
                }
                else
                {
                    throw new Exception("_DBC_ITEM_MEDIC database not found");
                }
                
            }
            else if (IsGemItem((int)idTable))
            {
		        //搜索纪录
		        _DBC_ITEM_GEM pGem = GemDBC.Search_Index_EQU((int)idTable);
		        if(pGem == null) return null;

                newItem = new CObject_Item_Gem(CreateID());
                ((CObject_Item_Gem)newItem).AsGem(pGem);

		        //物品规则
                itemRule(pGem, newItem);
            }
            else if (isSymbolItem(idTable))//是符印
            {
                _DBC_ITEM_SYMBOL pSymbol = SymbolDBC.Search_Index_EQU((int)idTable);
                if (pSymbol == null) return null;

                newItem = new CObject_Item_Symbol(CreateID());
                ((CObject_Item_Symbol)newItem).AsSymbol(pSymbol);
                //物品规则
                itemRule(pSymbol, newItem);
            }
            else if (isTalismanItem(idTable))//是法宝
            {
                _DBC_ITEM_TALISMAN dbItem = CDataBaseSystem.Instance.GetDataBase<_DBC_ITEM_TALISMAN>((int)DataBaseStruct.DBC_ITEM_TALISMAN).Search_Index_EQU((int)idTable);
                if (dbItem == null) return null;

                newItem = new CTalisman_Item(CreateID());
                CTalisman_Item curItem = newItem as CTalisman_Item;
                curItem.Define = dbItem;
                //物品规则
                itemRule(dbItem, newItem);
            }
            if (newItem == null)
            {
                LogManager.LogError("物品编号填写错误!,ID：" + idTable);
            }
            else
            {
                newItem.m_idTable = (int)idTable;
                allItems[newItem.GetID()] = newItem;
            }

            return newItem;
        }

        public static int CreateID()
        {
            return ID++;
        }
        // 全局唯一物品id [1/6/2012 Ivan]
        static int ID = 1;
        Dictionary<int, CObject_Item>   allItems;
        public bool ItemExist(int itemId)
        {
            return allItems.ContainsKey(itemId);
        }
        public CObject_Item FindItem(int id)
        {
            CObject_Item item = null;
            if (allItems.TryGetValue(id, out item))
                return item;
            return null;
        }
        public void DestroyItem(CObject_Item item)
        {
			if(item != null)
            	allItems.Remove(item.GetID());
// 	        if(pItem)
// 	        {
// 		        std::map< UINT, CObject_Item* >::iterator it = s_mapAllItem.find(pItem->GetID());
// 		        if(it != s_mapAllItem.end()) s_mapAllItem.erase(it);
// 		        if(pItem->m_pExtraInfoItem) 
// 		        {
// 			        delete pItem->m_pExtraInfoItem;
// 			        pItem->m_pExtraInfoItem = 0;
// 		        }
// 		        delete pItem;
// 	        }
// 	        return;
        }

        public void ChangeItemClientID(CObject_Item item)
        {
            allItems.Remove(item.GetID());
            item.ChangeClientID(CreateID());
            allItems.Add(item.GetID(), item);
        }

        void itemRule(_DBC_ITEM_EQUIP_BASE pEquipDBC, CObject_Item pItem)
        {
            _DBC_ITEM_RULE pItemRule = ItemRuleDBC.Search_Index_EQU(pEquipDBC.nRule);
            if(pItemRule == null) return ;
            pItem.RULE.bCanDrop		    = pItemRule.bCanDrop == 1;
            pItem.RULE.bCanOverLay = pItemRule.bCanOverLay == 1;
            pItem.RULE.bCanQuick = pItemRule.bCanQuick == 1;
            pItem.RULE.bCanSale = pItemRule.bCanSale == 1;
            pItem.RULE.bCanExchange = pItemRule.bCanExchange == 1;
            pItem.RULE.bCanUse = pItemRule.bCanUse == 1;
            pItem.RULE.bPickBind = pItemRule.bPickBind == 1;
            pItem.RULE.bEquipBind = pItemRule.bEquipBind == 1;
            pItem.RULE.bUnique = pItemRule.bUnique == 1;
        }
        void itemRule(_DBC_ITEM_MEDIC pMedic, CObject_Item pItem)
        {
            _DBC_ITEM_RULE pItemRule = ItemRuleDBC.Search_Index_EQU(pMedic.nRule);
            if (pItemRule == null) return;
            pItem.RULE.bCanDrop = pItemRule.bCanDrop == 1;
            pItem.RULE.bCanOverLay = pItemRule.bCanOverLay == 1;
            pItem.RULE.bCanQuick = pItemRule.bCanQuick == 1;
            pItem.RULE.bCanSale = pItemRule.bCanSale == 1;
            pItem.RULE.bCanExchange = pItemRule.bCanExchange == 1;
            pItem.RULE.bCanUse = pItemRule.bCanUse == 1;
            pItem.RULE.bPickBind = pItemRule.bPickBind == 1;
            pItem.RULE.bEquipBind = pItemRule.bEquipBind == 1;
            pItem.RULE.bUnique = pItemRule.bUnique == 1;
        }
        void itemRule(_DBC_ITEM_GEM pDefine, CObject_Item pItem)
        {
            _DBC_ITEM_RULE pItemRule = ItemRuleDBC.Search_Index_EQU(pDefine.nRule);
            if (pItemRule == null) return;
            pItem.RULE.bCanDrop = pItemRule.bCanDrop == 1;
            pItem.RULE.bCanOverLay = pItemRule.bCanOverLay == 1;
            pItem.RULE.bCanQuick = pItemRule.bCanQuick == 1;
            pItem.RULE.bCanSale = pItemRule.bCanSale == 1;
            pItem.RULE.bCanExchange = pItemRule.bCanExchange == 1;
            pItem.RULE.bCanUse = pItemRule.bCanUse == 1;
            pItem.RULE.bPickBind = pItemRule.bPickBind == 1;
            pItem.RULE.bEquipBind = pItemRule.bEquipBind == 1;
            pItem.RULE.bUnique = pItemRule.bUnique == 1;
        }
        void itemRule(_DBC_ITEM_SYMBOL pDefine, CObject_Item pItem)
        {
            _DBC_ITEM_RULE pItemRule = ItemRuleDBC.Search_Index_EQU(pDefine.nRule);
            if (pItemRule == null) return;
            pItem.RULE.bCanDrop = pItemRule.bCanDrop == 1;
            pItem.RULE.bCanOverLay = pItemRule.bCanOverLay == 1;
            pItem.RULE.bCanQuick = pItemRule.bCanQuick == 1;
            pItem.RULE.bCanSale = pItemRule.bCanSale == 1;
            pItem.RULE.bCanExchange = pItemRule.bCanExchange == 1;
            pItem.RULE.bCanUse = pItemRule.bCanUse == 1;
            pItem.RULE.bPickBind = pItemRule.bPickBind == 1;
            pItem.RULE.bEquipBind = pItemRule.bEquipBind == 1;
            pItem.RULE.bUnique = pItemRule.bUnique == 1;
        }
        void itemRule(_DBC_ITEM_TALISMAN pDefine, CObject_Item pItem)
        {
            _DBC_ITEM_RULE pItemRule = ItemRuleDBC.Search_Index_EQU(pDefine.nRule);
            if (pItemRule == null) return;
            pItem.RULE.bCanDrop = pItemRule.bCanDrop == 1;
            pItem.RULE.bCanOverLay = pItemRule.bCanOverLay == 1;
            pItem.RULE.bCanQuick = pItemRule.bCanQuick == 1;
            pItem.RULE.bCanSale = pItemRule.bCanSale == 1;
            pItem.RULE.bCanExchange = pItemRule.bCanExchange == 1;
            pItem.RULE.bCanUse = pItemRule.bCanUse == 1;
            pItem.RULE.bPickBind = pItemRule.bPickBind == 1;
            pItem.RULE.bEquipBind = pItemRule.bEquipBind == 1;
            pItem.RULE.bUnique = pItemRule.bUnique == 1;
        }
    }
/*}*/
