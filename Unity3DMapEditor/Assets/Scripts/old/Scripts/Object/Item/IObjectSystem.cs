using System;
using System.Collections.Generic;

// namespace SGWEB
// {
    //-----------------------------------------------------
	//物品规则
    public enum ITEM_RULE
	{
		RULE_DROP = 0,	// 是否可丢弃
		RULE_OVERLAY,	// 是否可重叠
		RULE_QUICK,		// 是否可放入快捷栏
		RULE_SALE,		// 是否可以出售给NPC商店
		RULE_EXCHANGE,	// 是否可以交易
		RULE_USE,		// 是否可以使用
		RULE_PICKBIND,	// 是否是拾取邦定
		RULE_EQUIPBIND,	// 是否是装备邦定
		RULE_UNIQUE,	// 是否是唯一
	};

	//-----------------------------------------------------
	//物品归属
	public enum ITEM_OWNER
	{
		IO_UNKNOWN,

		IO_MYSELF_EQUIP,		//玩家自己身上的装备
		IO_MYSELF_PACKET,		//玩家自己身上的包中
		IO_MYSELF_BANK,			//玩家自己身上的银行中
		IO_PLAYEROTHER_EQUIP,	//其他玩家身上的装备
		IO_ITEMBOX,				//打开的宝箱中
		IO_BOOTH,				//商人的货架
		IO_MYEXBOX,				//自己的交易盒
		IO_OTHEREXBOX,			//对方的交易盒
		//		IO_ENCHASEPAGE,			// 宝石合成/镶嵌界面
		IO_MISSIONBOX,			//任务递交盒
		IO_MYSTALLBOX,			//自己的摊位盒
		IO_OTSTALLBOX,			//对方的摊位盒
		//		IO_APPENDITEM,			//装备上附加的宝石
		IO_QUESTVIRTUALITEM,	// 任务奖励的（虚拟）物品，只用于显示。
		IO_PS_SELFBOX,			//
		IO_PS_OTHERBOX,			//
		IO_BOOTH_CALLBACK,		//Shop界面上的回购物品
		IO_GEM_SEPARATE,		//宝石摘除UI的宝石
		IO_TRANSFER_ITEM,		//物品信息传送
		IO_CITY_RESEARCH,		//城市研究显示
		IO_CITY_SHOP,			//城市商店显示
		IO_QUESTUI_DEMAND,		//Quest UI Demand
		IO_QUESTUI_REWARD,		//Quest UI Reward
        IO_TALISMAN_PACKET,
        IO_TALISMAN_EQUIPT,
        IO_PET_EQUIPT, //宠物装备
        IO_OTHERPLAYER_PET_EQUIPT,//其他玩家宠物装备

	};
    // 白绿蓝紫黄 [9/26/2011 edit by ZL]
    public enum EQUIP_QUALITY
    {

        WHITE_EQUIP = 0,
        GREEN_EQUIP,
        BLUE_EQUIP,
        PURPLE_EQUIP,
        YELLOW_EQUIP,
        INVALID_EQUIP,
    };

    // 物品在客户端的保存状态 2006－3－24
    public enum SAVE_ITEM_STATUS
    {
        NO_MORE_INFO = 0,		// 没有详细信息。
        GET_MORE_INFO,			// 得到详细信息
    };
    public enum EQUIP_ATTRIB
    {
        EQUIP_ATTRIB_UNIDENTIFY,	// 装备未鉴定。
        EQUIP_ATTRIB_IDENTIFY,		// 装备已鉴定。
    };
    public enum EQUIP_BIND_INFO
    {
        FREE_BIND = 0,		// 无绑定限制
        BINDED,				// 已经绑定
        GETUP_BIND,			// 拾取绑定
        EQUIP_BIND,			// 装备绑定
    };

    public struct ITEM_GUID
    {
        //服务器传来的数据 ref GameStruct_Item.h
        public ushort m_idServer;			//世界号: (例)101
        public ushort m_idWorld;			//服务端程序号：(例)5
        public uint m_uSerial;			//物品序列号：(例)123429

        private UInt64 idUnion;//合并后的ID
        public System.UInt64 IdUnion
        {
            get
            {
                if (idUnion == 0)
                    idUnion = ((UInt64)m_idServer << 48) | ((UInt64)m_idWorld << 32) | ((UInt64)m_uSerial);
                return idUnion;
            }
        }
    }

    //扩展属性定义
    public class EXTRA_DEFINE
    {
        public int m_CurDurPoint;				//当前耐久值
        public int m_MaxDurPoint;				//最大耐久值
        public List<_ITEM_ATTR> m_vEquipAttributes;			//装备属性(白色和绿色共有)
        public List<_ITEM_ATTR> m_vBlueEquipAttributes;		//装备属性(蓝色装备属性)
        public List<_ITEM_GEMINFO> m_vEquipAttachGem;			//附加的宝石
        public int m_nLevelNeed;				//需求等级
        //		INT					m_nFrogLevel;				//锻造等级
        public int m_nRepairFailureTimes;		//修理失败次数
        public int m_nEquipQulity;				//装备品质
        public int m_nSellPrice;				//售出价格
        //BOOL				m_bLocked;					//是否锁定
        public EQUIP_BIND_INFO M_nEquipBindInfo;			// 装备是否绑定
        public int m_EnableGemCount;			// 可以镶嵌的宝石个数2006－4－30
        public int m_EquipEnhanceLevel;		// 强化等级 [7/19/2011 ivan edit]
        public _ITEM_ATTR m_PrintSoulAttribute;		// 装备魂印属性 [10/10/2011 edit by ZL]
        public byte m_PrintSoulType;			// 装备魂印类型  0则没有 [10/10/2011 edit by ZL]
        public EXTRA_DEFINE()
        {
            m_CurDurPoint = 0;
            m_MaxDurPoint = 0;
            m_nLevelNeed = 0;
            m_nRepairFailureTimes = 0;
            m_nEquipQulity = 0;
            m_nSellPrice = 0;
            M_nEquipBindInfo = EQUIP_BIND_INFO.FREE_BIND;
            m_EnableGemCount = 0;
            m_EquipEnhanceLevel = 0;
            m_PrintSoulType = 0;
            m_vEquipAttachGem = new List<_ITEM_GEMINFO>();
            m_vEquipAttributes = new List<_ITEM_ATTR>();
            m_vBlueEquipAttributes = new List<_ITEM_ATTR>();
        }

    };
/*}*/
