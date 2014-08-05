using System;
using System.Text;

using Network;
using Network.Packets;

namespace Interface
{
    public class UIInfterface
    {
        private static UIInfterface _instance;
        public static UIInfterface Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UIInfterface();
                return _instance;
            }
        }

        public void Set_XSCRIPT_Function_Name(string funcName)
        {
            CDataPool.Instance._X_SCRIPT.FunName = Encoding.ASCII.GetBytes(funcName);
        }

        public void Set_XSCRIPT_Parameter(int index, int param)
        {
            CDataPool.Instance._X_SCRIPT[index] = param;
        }

        public void Set_XSCRIPT_ScriptID(int scriptId)
        {
            CDataPool.Instance._X_SCRIPT.ScriptID = scriptId;
        }

        public void Set_XSCRIPT_ParamCount(byte paramCount)
        {
            CDataPool.Instance._X_SCRIPT.ParamCount = paramCount;
        }

        public void Clear_XSCRIPT()
        {
            CDataPool.Instance._X_SCRIPT.CleanUp();
        }

        public void Send_XSCRIPT()
        {
            CGExecuteScript msg = (CGExecuteScript)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_EXECUTESCRIPT);

            msg.x_Script = CDataPool.Instance._X_SCRIPT;

            NetManager.GetNetManager().SendPacket(msg);
        }
        public int GetActionNum(ActionNameType nameType)
        {
            return CActionSystem.Instance.GetActionNum(nameType);
        }

        // 请求队伍列表 [3/23/2012 SUN]
        public void RequestActivityTeamInfo(short nID)
        {
            Clear_XSCRIPT();
                Set_XSCRIPT_Function_Name("OnDefaultEvent");
                Set_XSCRIPT_ScriptID(808000);
                Set_XSCRIPT_Parameter(0, nID);
                Set_XSCRIPT_ParamCount(1);
            Send_XSCRIPT();
        }
        // 关闭队伍列表，需要通知服务器 [3/23/2012 SUN]
        public void CloseActivityTeamInfo(short nID)
        {
            Clear_XSCRIPT();
                Set_XSCRIPT_Function_Name("OnActivityAbandon");
                Set_XSCRIPT_ScriptID(808000);
                Set_XSCRIPT_Parameter(0, nID);
                Set_XSCRIPT_ParamCount(1);
            Send_XSCRIPT();
        }

        // 自动加入队伍 [3/23/2012 SUN]
        public void AutoJoinActivityTeam( int nId)
        {
            if (!CDataPool.Instance.Campaign_CheckMySelf(nId))
                return;
            Clear_XSCRIPT();
            Set_XSCRIPT_Function_Name("NotifyActivity");
            Set_XSCRIPT_ScriptID(808000);
            Set_XSCRIPT_Parameter(0, nId);
            Set_XSCRIPT_Parameter(1, 1);
            Set_XSCRIPT_ParamCount(2);
            Send_XSCRIPT();
        }
        // 自己创建队伍 [3/23/2012 SUN]
        public void CreateActivityTeam(int nId)
        {
            if (!CDataPool.Instance.Campaign_CheckMySelf(nId))
                return;
            Clear_XSCRIPT();
            Set_XSCRIPT_Function_Name("NotifyActivity");
            Set_XSCRIPT_ScriptID(808000);
            Set_XSCRIPT_Parameter(0, nId);
            Set_XSCRIPT_Parameter(1, 0);
            Set_XSCRIPT_ParamCount(2);
            Send_XSCRIPT();
        }
        //开始活动（传送到活动入口）
        public void BeginJoinActivity(int nId)
        {
            Clear_XSCRIPT();
            Set_XSCRIPT_Function_Name("NotifyActivity");
            Set_XSCRIPT_ScriptID(808000);
            Set_XSCRIPT_Parameter(0, nId);
            Set_XSCRIPT_Parameter(1, 2);
            Set_XSCRIPT_ParamCount(2);
            Send_XSCRIPT();
        }
        /// <summary>
        /// 加入一个活动队伍
        /// </summary>
        /// <param name="nIndex">队伍索引，对应CDataPool中的编号</param>
        /// <param name="nId">活动ID</param>
        public void JoinActivityTeam(int nIndex, int nId)
        {

        }

        // 销毁物品
        public void DiscardItem()
        {
	        //销毁物品,根据不同的物品来源，需要发送不同的销毁消息
	        int nTypeOwner	=	CDataPool.Instance.DisCard_GetItemConta();
	        int	nItemPos	=	CDataPool.Instance.DisCard_GetItemPos();
	        switch( (ITEM_OWNER)nTypeOwner ) 
	        {
	        case ITEM_OWNER.IO_MYSELF_PACKET:				// 来自玩家的背包
		        {
			        CObject_Item pItem = CDataPool.Instance.UserBag_GetItem(nItemPos);

			        if(pItem==null) break;
			        //验证是否可以丢弃
			        if(!pItem.Rule(ITEM_RULE.RULE_DROP))
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, pItem.RuleFailMessage(ITEM_RULE.RULE_DROP));
				        break;
			        }

                    CGDiscardItem msg = new CGDiscardItem();
                    msg.Operate = (byte)(CGDiscardItem.Operator.FromBag);
                    msg.BagIndex = (byte)(nItemPos);

                    NetManager.GetNetManager().SendPacket(msg);
		        }
		        break;
	        case ITEM_OWNER.IO_MYSELF_EQUIP:					// 来自玩家身上的装备
		        {
                    //break;
                    //tObject_Item* pItem = CDataPool::GetMe()->UserEquip_GetItem((HUMAN_EQUIP)nItemPos);

                    //if(!pItem) break;
                    ////验证是否可以丢弃
                    //if(!pItem->Rule(tObject_Item::RULE_DROP))
                    //{
                    //    CEventSystem::GetMe()->PushEvent(GE_INFO_SELF, pItem->RuleFailMessage(tObject_Item::RULE_DROP).c_str());
                    //    break;
                    //}

                    //CGDiscardEquip msg;
                    //msg.SetEquipPoint(nItemPos);

                    //CNetManager::GetMe()->SendPacket(&msg);
		        }
		        break;
	        case ITEM_OWNER.IO_MYSELF_BANK:					// 玩家的银行
		        {
			        CObject_Item pItem = CDataPool.Instance.UserBank_GetItem(nItemPos);

			        if(pItem==null) break;
			        //验证是否可以丢弃
			        if(!pItem.Rule(ITEM_RULE.RULE_DROP))
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, pItem.RuleFailMessage(ITEM_RULE.RULE_DROP));
				        break;
			        }

                    CGDiscardItem msg = new CGDiscardItem();
                    msg.Operate = (byte)(CGDiscardItem.Operator.FromBank);
                    msg.BagIndex = (byte)(nItemPos);

                    NetManager.GetNetManager().SendPacket(msg);
		        }
		        break;
	        default:
		        break;
	        }
        }
        // 取消物品的锁定状态
        public void  DiscardItemCancelLocked()
        {
	        int nTypeOwner	=	CDataPool.Instance.DisCard_GetItemConta();
	        int	nItemPos	=	CDataPool.Instance.DisCard_GetItemPos();

	        switch( (ITEM_OWNER)nTypeOwner ) 
	        {
	        case ITEM_OWNER.IO_MYSELF_PACKET:				// 来自玩家的背包
		        {
			        CObject_Item pItem = CDataPool.Instance.UserBag_GetItem(nItemPos);

			        if(pItem != null)
			        {
				        pItem.SetLock(false);
			        }
		        }
		        break;
	        case ITEM_OWNER.IO_MYSELF_EQUIP:					// 来自玩家身上的装备
		        {
			        CObject_Item pItem = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)nItemPos);

			        if(pItem != null)
			        {
				        pItem.SetLock(false);
			        }
		        }
		        break;
	        case ITEM_OWNER.IO_MYSELF_BANK:					// 玩家的银行
		        {
			        CObject_Item pItem = CDataPool.Instance.UserBank_GetItem(nItemPos);

			        if(pItem!=null)
			        {
				        pItem.SetLock(false);
			        }
		        }
		        break;
	        default:
		        break;
	        }

	        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
        }
    }

    /// <summary>
    /// npc商店
    /// </summary>
    public class NPCShop
    {
        private static NPCShop _instance;
        public static NPCShop Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NPCShop();
                return _instance;
            }
        }

    //关闭NpcShop
        public int Close()
        {
            //在点击Quest的"再见"的时候，关闭NpcShop
	        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CLOSE_BOOTH);

	        return 0;
        }
	//获得商店NpcID
        public int GetNpcId()
        {
            return CDataPool.Instance.Booth_GetShopNpcId();
        }
	//获得商店的物品
	//INT EnumCallBackItem();
	//获得回购物品的数量
        public int GetCallBackNum()
        {
            int nItemNum = CDataPool.Instance.Booth_GetSoldNumber();
	        if(nItemNum >= 0)
		        return nItemNum;
	        else
		        return 0;
        }
        public CActionItem EnumCallBackItem(int nIndex)
        {
            int nNum = CDataPool.Instance.Booth_GetSoldNumber();
            for (int i = 0; i < nNum; i++ )
            {
                CObject_Item item = CDataPool.Instance.Booth_GetSoldItem(i);
                if(item != null)
                    return CActionSystem.Instance.GetAction_ItemID(item.GetID(), false);
            }
            return null;
        }
        public uint EnumItemSoldPrice(int nIndex)
        {
            return CDataPool.Instance.Booth_GetSoldPrice(nIndex);
        }
	//获得商店的物品价格
        public uint EnumItemPrice(int nIndex)
        {
            return CDataPool.Instance.Booth_GetItemPrice(nIndex);
        }
	//获得商店物品叠加数量
        public int EnumItemMaxOverlay(int nIndex)
        {
            CObject_Item pBoothItem = CDataPool.Instance.Booth_GetItem(nIndex);
	        if(pBoothItem != null)
	        {
		        return pBoothItem.GetMaxOverLay();
	        }
	        else
	        {
		        return -1;
	        }
        }
	//获得商店物品名称
        public string EnumItemName(int nIndex)
        {
            CObject_Item pBoothItem = CDataPool.Instance.Booth_GetItem(nIndex);
	        if(pBoothItem != null)
	        {
		        return pBoothItem.GetName();
	        }
	        else
	        {
		        return "";
	        }
        }
	//一次购买多个物品
        public int BulkBuyItem(int nIndex, int nNumber)
        {
            CObject_Item pBoothItem = CDataPool.Instance.Booth_GetItem(nIndex);
	        if(pBoothItem != null)
	        {
		        nNumber = (nNumber < 1 || nNumber > pBoothItem.GetMaxOverLay())?1:nNumber;

		        if(CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money() >= 
			        (int)(CDataPool.Instance.Booth_GetItemPrice(nIndex)*nNumber))
		        {
                    CGShopBuy msg = new CGShopBuy();

                    msg.UniqueID = CDataPool.Instance.Booth_GetShopUniqueId();
                    msg.Index = (byte)pBoothItem.GetPosIndex();
                    msg.SerialNum = (byte)CDataPool.Instance.Booth_GetSerialNum();
                    msg.BuyNum = (uint)nNumber;

                    NetManager.GetNetManager().SendPacket( msg );
			        return 0;
		        }
	        }

	        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE, "#{LowMoney}");

	        return 0;
        }
	//得到ActionItem通过item id
        public CActionItem getItemById(int itemId)
        {
           return CActionSystem.Instance.GetActionByActionId(itemId);
        }
	//获得商店的详细类型
        public int GetShopType(string str)
        {
            if(str == "repair")
	        {
		        return CDataPool.Instance.Booth_GetRepairType();
	        }
	        else if(str == "callback")
	        {
		        return CDataPool.Instance.Booth_GetCallBack() ? 1 : 0;
	        }
	        else if(str == "unit")
	        {
		        return CDataPool.Instance.Booth_GetCurrencyUnit();
	        }
	        else if(str == "buy")//收购
	        {
		        return CDataPool.Instance.Booth_GetBuyType();
	        }

	        return 1;
        }
	// 获得当前玩家身上装备的所有修理价格，（需要考虑这个Npc的修理价格系数，能不能修理等条件）
	    public int GetRepairAllPrice()
        {
            int nRepairPrice = 0;
	        float fRepairSpend = CDataPool.Instance.Booth_GetRepairSpend();

	        for(HUMAN_EQUIP i=HUMAN_EQUIP.HEQUIP_WEAPON; i<HUMAN_EQUIP.HEQUIP_NUMBER; i++)
	        {
		        CObject_Item pItem = CDataPool.Instance.UserEquip_GetItem(i);
		        if(pItem != null)
		        {
			        if( pItem.GetItemClass() == ITEM_CLASS.ICLASS_EQUIP )
			        {
				        if(pItem.GetItemDur() < pItem.GetItemMaxDur())
				        {
					        //计算修理价格
					        nRepairPrice += (int)(((CObject_Item_Equip)pItem).GetRepairPrice() * fRepairSpend);
				        }
			        }
		        }
	        }

	        return nRepairPrice;
        }

	    //得到NPC的阵营
	    public int GetNpcCamp()
        {
            return 0;
        }

	// temp fix [10/21/2010 Sun]
	    public void CloseConfirm()
        {

        }


	    public bool GetBuyDirectly()
        {
            return true;
        }


    }
}