//////////////////////////////////////////////////////////////////////////
//Tooltips
//UI Interface
//////////////////////////////////////////////////////////////////////////

using System;

namespace Interface
{
    // 从tempclass 移到此处 [2/13/2012 Administrator]
    public class SuperTooltips
    {
        static readonly SuperTooltips instance = new SuperTooltips();
        public static SuperTooltips Instance
        {
            get
            {
                return instance;
            }
        }

        CActionItem currAction;

        internal void SetActionItem(CActionItem action)
        {
            currAction = action;
        }

        public CActionItem GetActionItem() { return currAction; }


        //////////////////////////////////////////////////////////////////////////
        // Interface for UI
        // 0[int] - 心法等级
        public int GetSkillXinfaLevel()
        {
            return -1;
        }




        //----------------------------------------------------------------------------------------------------------------------
        //
        // 宝石信息
        //

        // 得到宝石等级
        // 传出参数1个
        // 0[int] - 宝石等级
        public int GetGemLevel()
        {
            return -1;
        }

        // 得到宝石属性
        // 传出参数1个
        // 0[str] - 宝石属性
        public string GetGemArribInfo()
        {
            return "";
        }


        //----------------------------------------------------------------------------------------------------------------------
        //
        // 得到物品的quality
        //

        // 传出参数1个
        // 0[int] - 药品或宝石品质
        public int GetItemQuality()
        {
            return -1;
        }


        // 得到物品在表中的类型
        // 传出参数1个
        // 0[int] - 表中的类型。
        public int GetItemTableType()
        {
            return -1;
        }



        // 得到配方需要的技能名字。
        // 传出参数1个
        // 0[int] - 表中的类型。
        public string GetPrescrSkillName()
        {
            return "";
        }

        // 得到配方需要的技能等级。
        // 传出参数1个
        // 0[int] - 表中的等级。
        public int GetPrescrNeedSkillLevel()
        {
            return -1;
        }


        // 得到配方需要技能的当前等级。
        // 传出参数1个
        // 0[int] - 配方需要的当前等级。
        public int GetPrescrNeedSkillCurLevel()
        {
            return -1;
        }


        // 向服务器请求item的详细信息
        // 
        // 
        public void SendAskItemInfoMsg()
        {
            CActionItem_Item actionItem = currAction as CActionItem_Item;
            if (actionItem != null)
            {
                actionItem.SendAskItemInfoMsg();
            }
        }


        // 得到蓝色装备是否鉴定
        // 
        // 
        public int IsBlueEquipIdentify()
        {
            return 0;
        }

        //
        // npc商店是否打开。
        // 
        public bool IsNpcShopOpen()
        {
            return false;
        }

        //是否存在
        public bool IsPresent()
        {
            return false;
        }

        //得到描述信息2006－4－48
        public string GetTypeDesc()
        {
            return "";
        }

        //得到物品是否在背包中2006-5-10
        public bool IsItemInPacket()
        {
            return false;
        }

        //
        // 得到是否显示价格 2006-5-18
        //
        public bool IsShowPrice()
        {
            return false;
        }

        //
        // 得到显示价格 2006-5-18
        //
        public int GetShowPrice()
        {
            return 0;
        }

        //
        // 得到物品档次等级描述 2006-5-18
        //
        public int GetItemLevelDesc()
        {
            return 0;
        }

        // 
        // 得到定位点类型物品的参数
        //Lua_ItemAnchorInfo GetAnchorValue( void );

        // 得到套装属性函数
        // 是否是套装
        public bool IsItemSuit()
        {
            return false;
        }
        // 得到套装的数量
        public int GetItemSuitNumber()
        {
            return 0;
        }
        //// 判断自己是不是有第几件套装如果有就返回名字，没有就返回空字符串
        //Lua_ItemSuitInfo IsItemExist( INT nIdx );
        // 得到第几件套装的名字,
        //INT GetSuitName( LuaPlus::LuaState* state );
        // 得到套装的属性
        public string GetSuitAtt()
        {
            return "";
        }
        //
        // 得到银票的价值参数
        //void GetTwoMoneyValue(INT moneyValue[2]);

        // 得到物品的显示颜色
        public string GetSysColour()
        {
            return "";
        }

        // temp [3/1/2010 Sun]
        public int GetGemHoleCounts()
        {
            return 0;
        }

        // temp [3/1/2010 Sun]
        public int GetMoney1()
        {
            return 0;
        }
        //INT GetMoney2(void);
        //INT GetPropertys(void);
        //INT GetAuthorInfo(void);
        //INT GetExplain(void);
        //const char* GetDesc1(void);
        //string GetDesc2(void);
        //INT GetDesc3(void);

        //// 取得强化状态 [9/9/2011 edit by ZL]
        //Item_Enhance GetStrenthState(void);

        //// 取得强化等级 [9/9/2011 edit by ZL]
        //Lua_Item_Strength GetStrenth(void);

        //// 取得装备评分 [9/26/2011 edit by ZL]
        //INT GetItemScoreInfo(void);

        //// 取得装备档次 [9/26/2011 edit by ZL]
        //INT GetItemDangci(void);

        //// 得到装备的品级 [9/26/2011 edit by ZL]
        //INT GetItemPinJi(void);

        //// 得到装备的颜色 [9/26/2011 edit by ZL]
        //INT GetItemColor(void);

        //// 得到装备的五行 [9/26/2011 edit by ZL]
        //INT GetItemWuXing(void);

        //// 得到装备的品级值 [9/26/2011 edit by ZL]
        //Lua_ItemPinJiValue GetItemPinJiValue(void);

        //// 得到镶嵌宝石信息 [9/26/2011 edit by ZL]
        //Lua_ItemGemDescribe GetGemDescribe(int nIndex);

        //// 得到装备的魂印属性 [10/10/2011 edit by ZL]
        //const char*	GetPrintSoulAttribute(void);

        //// 得到当前装备的魂印属性是否激活 [10/10/2011 edit by ZL]
        //INT GetItemSoulActive(void);

        //// 得到装备的魂印类型 [10/10/2011 edit by ZL]
        //INT	GetItemPrintSoulType(void);

        //INT GetDesc5(void);
        //INT GetIconName(void);
        //string GetGemIcon1(void);
        //string GetGemIcon2(void);
        //string GetGemIcon3(void);

        internal string GetTitle()
        {
            if (currAction != null)
                return currAction.GetName();
            else
                return "";
        }

        internal string GetDesc()
        {
            if (currAction != null)
                return currAction.GetDesc();
            else
                return "";
        }
        internal string GetIcon()
        {
            if (currAction != null)
                return currAction.GetIconName();
            return "";
        }
    }
}