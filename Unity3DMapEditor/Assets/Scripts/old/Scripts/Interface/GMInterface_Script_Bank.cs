using Network.Packets;
namespace Interface
{
    public struct BankMoney
    {
        public bool available;
        public int nMoney;
        public int goldCoin;
        public int silverCoin;
        public int copperCoin;
    }
    public struct BoxInfo
    {
        public int beginIndex;
        public int gridNum;
    }
    public struct ActionItem
    {
        public CActionItem actionItem;
        public bool isLock;
    }
    public class Bank
    {
        static Bank _instance;
        public static Bank Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Bank();
                return _instance;
            }
        }
        public bool IsPresent()
        {
            if(CObjectManager.Instance.GetMainTarget() != null)
		    {
			    return true;
		    }
		    else
		    {
			    return false;
		    }
        }
        //打开存钱界面（输入存钱数量的界面）
        public void OpenSaveFrame()
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_INPUT_MONEY,"save");
        }
        //打开取钱界面（输入取钱数量的界面）
        public void OpenGetFrame()
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_INPUT_MONEY,"get");
        }
        //金钱的转换
        BankMoney GetInputMoney(string szGold, string szSilver, string szCopperCoin)
        {
            /*
		检查数值的大小不能大于1亿，如果已经输入了
		*/

            if (szGold.Length > 6)
                szGold = szGold.Remove(0, szGold.Length - 6);

            if (szSilver.Length > 2)
                szSilver = szSilver.Remove(0, szSilver.Length - 2);

            if (szCopperCoin.Length > 2)
                szCopperCoin = szCopperCoin.Remove(0, szCopperCoin.Length - 2);

            int nGold = int.Parse(szGold);
            int nSilver = int.Parse(szSilver);
            int nCopperCoin = int.Parse(szCopperCoin);
            bool bAvailability = true;
            int nMoney = 0;

            nMoney = nCopperCoin + nSilver * 100 + nGold * 10000;

            BankMoney luaMoney = new BankMoney();
            luaMoney.available = bAvailability;
            luaMoney.nMoney = nMoney;
            return luaMoney;
        }
        //发送存钱消息
        public int SaveMoneyToBank(int amount)
        {
            if (amount == 0)
			    return 0;

		    CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();

		    if(pCharData != null)
		    {
                if ((pCharData.Get_Money()) >= amount)
			    {
				    byte SaveType = (byte)CGBankMoney.OPtype.SAVE_MONEY;
				    CGBankMoney msg = new CGBankMoney();
				    msg.SaveType = (SaveType);
                    msg.AmountMoney = (amount);
				    NetManager.GetNetManager().SendPacket(msg);
			    }
			    else
			    {
				    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE,"存钱数目超过身上金钱数目");
			    }
		    }
		    return 0;
        }
        //发送取钱消息
        public int GetMoneyFromBank(int amount)
        {
            if (amount == 0)
			    return 0;

            if ((CDataPool.Instance.UserBank_GetBankMoney()) >= amount)
		    {
			    byte SaveType = (byte)CGBankMoney.OPtype.PUTOUT_MONEY;
			    CGBankMoney msg = new CGBankMoney();
			    msg.SaveType = (SaveType);
                msg.AmountMoney = (amount);
			    NetManager.GetNetManager().SendPacket(msg);
		    }
		    else
		    {
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE,"取钱数目超过银行中现有金钱数目");
		    }
		    return 0;
        }
        //导出银行的金钱数
        public BankMoney GetBankMoney()
        {
            int nMoney = CDataPool.Instance.UserBank_GetBankMoney();
            int nGoldCoin;
            int nSilverCoin;
            int nCopperCoin;

		    nCopperCoin = nMoney % 100;

		    if( nMoney >= 100 )
		    {
			    nSilverCoin = ( (nMoney-nCopperCoin)/100 ) % 100;
		    }
		    else
		    {
			    nSilverCoin = 0;
		    }

		    if ( nMoney >= 10000 )
		    {
			    nGoldCoin = (  ( (nMoney-nCopperCoin)/100 )-nSilverCoin  )/100;
		    }
		    else
			    nGoldCoin = 0;

		    BankMoney luaMoney = new BankMoney();
		    luaMoney.nMoney = nMoney;
		    luaMoney.goldCoin = nGoldCoin;
		    luaMoney.silverCoin = nSilverCoin;
		    luaMoney.copperCoin = nCopperCoin;

		    return luaMoney;
        }
        //导出银行的元宝数
        public int GetBankRMB()
        {
            int nMoney = CDataPool.Instance.UserBank_GetBankRMB();
		    return nMoney;
        }
        //发送存元宝消息
        public int SaveRMBToBank(string szRMB)
        {
            int	Amount = int.Parse(szRMB);

		    if(Amount == 0)
			    return 0;

		    CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();

		    if(pCharData != null)
		    {
			    if((pCharData.Get_RMB()) >= Amount)
			    {
				    byte SaveType = (byte)CGBankMoney.OPtype.SAVE_RMB;
				    CGBankMoney msg = new CGBankMoney();
				    msg.SaveType = SaveType;
				    msg.AmountRMB = Amount;
				    NetManager.GetNetManager().SendPacket(msg);
			    }
			    else
			    {
				    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE,"存元宝的数目超过身上元宝的数目");
			    }
		    }
		    return 0;
        }
        //发送取元宝消息
        public int GetRMBFromBank(string szRMB)
        {
            int	Amount = int.Parse(szRMB);
	
		    if(Amount == 0)
			    return 0;

		    if((uint)(CDataPool.Instance.UserBank_GetBankRMB()) >= Amount)
		    {
			    byte SaveType = (byte)CGBankMoney.OPtype.PUTOUT_RMB;
			    CGBankMoney msg = new CGBankMoney();
			    msg.SaveType = (SaveType);
			    msg.AmountRMB = (Amount);
			    NetManager.GetNetManager().SendPacket(msg);
		    }
		    else
		    {
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE,"取元宝的数目超过银行中现有元宝的数目");
		    }
		    return 0;
        }
        //发送一个移动银行物品到背包的消息
        public int MoveItemToPacket(int nIndex)
        {
            CGBankRemoveItem msg = new CGBankRemoveItem();
		    msg.ToType = (byte)(CGBankRemoveItem.RemoveType.BAG_POS);
		    msg.IndexFrom = (byte)(nIndex);
		    msg.IndexTo = (255);

		    NetManager.GetNetManager().SendPacket(msg);
		    return 0;
        }
        //导出现在拥有的租赁箱的个数
        public int GetRentBoxNum()
        {
            int nRentBosNum = -1;
		    //格子的最大只
		    int nEndIndex = CDataPool.Instance.UserBank_GetBankEndIndex();
		    //查询现在拥有的租赁箱个数
		    if( GAMEDEFINE.RENTBOX2_START_INDEX == nEndIndex )
		    {
			    nRentBosNum = 1;
		    }
            else if (GAMEDEFINE.RENTBOX3_START_INDEX == nEndIndex)
		    {
			    nRentBosNum = 2;
		    }
            else if (GAMEDEFINE.RENTBOX4_START_INDEX == nEndIndex)
		    {
			    nRentBosNum = 3;
		    }
            else if (GAMEDEFINE.RENTBOX5_START_INDEX == nEndIndex)
		    {
			    nRentBosNum = 4;
		    }
            else if (GAMEDEFINE.MAX_BANK_SIZE == nEndIndex)
		    {
			    nRentBosNum = 5;
		    }
		    return nRentBosNum;
        }
        //导出指定租赁箱的开始数和格子数
        public BoxInfo GetRentBoxInfo(int nIndex)
        {
            int nGridNum = 0;
            int nBeginIndex = 0;

            switch (nIndex)
            {
                case 1:
                    {
                        nBeginIndex = 0;
                        nGridNum = GAMEDEFINE.RENTBOX2_START_INDEX;
                    }
                    break;
                case 2:
                    {
                        nBeginIndex = GAMEDEFINE.RENTBOX2_START_INDEX;
                        nGridNum = GAMEDEFINE.RENTBOX3_START_INDEX - GAMEDEFINE.RENTBOX2_START_INDEX;
                    }
                    break;
                case 3:
                    {
                        nBeginIndex = GAMEDEFINE.RENTBOX3_START_INDEX;
                        nGridNum = GAMEDEFINE.RENTBOX4_START_INDEX - GAMEDEFINE.RENTBOX3_START_INDEX;
                    }
                    break;
                case 4:
                    {
                        nBeginIndex = GAMEDEFINE.RENTBOX4_START_INDEX;
                        nGridNum = GAMEDEFINE.RENTBOX5_START_INDEX - GAMEDEFINE.RENTBOX4_START_INDEX;
                    }
                    break;
                case 5:
                    {
                        nBeginIndex = GAMEDEFINE.RENTBOX5_START_INDEX;
                        nGridNum = GAMEDEFINE.MAX_BANK_SIZE - GAMEDEFINE.RENTBOX5_START_INDEX;
                    }
                    break;
                default:
                    break;
            }
            BoxInfo boxInfo;
            boxInfo.beginIndex = nBeginIndex;
            boxInfo.gridNum = nGridNum;
            return boxInfo;
        }
        //设置当前的租赁箱
        public int SetCurRentIndex(int nIndex)
        {
            CActionSystem.Instance.SetCurBankRentBoxIndex(nIndex);
		    return 0;
        }
        //转换货币
        public BankMoney TransformCoin(int nMoney)
        {
            int nGoldCoin;
            int nSilverCoin;
            int nCopperCoin;

            nCopperCoin = nMoney % 100;

            if (nMoney >= 100)
            {
                nSilverCoin = ((nMoney - nCopperCoin) / 100) % 100;
            }
            else
            {
                nSilverCoin = 0;
            }

            if (nMoney >= 10000)
            {
                nGoldCoin = (((nMoney - nCopperCoin) / 100) - nSilverCoin) / 100;
            }
            else
                nGoldCoin = 0;

            BankMoney luaMoney = new BankMoney();
            luaMoney.goldCoin = nGoldCoin;
            luaMoney.silverCoin = nSilverCoin;
            luaMoney.copperCoin = nCopperCoin;
            return luaMoney;
        }
        //关闭银行
        public void Close()
        {
            //直接发送关闭InputMoney的事件
		    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CLOSE_INPUT_MONEY);

        }
        //获得操作物品
        public ActionItem EnumItem(int nIndex)
        {
            //查询物品
		    ActionItem luaItem;

		    CActionItem pActionItem = CActionSystem.Instance.EnumAction(nIndex, ActionNameType.bankItem);
		    if(pActionItem != null)
		    {
			    luaItem.actionItem = pActionItem;

			    if(((CObject_Item)(pActionItem.GetImpl())).IsLocked())
				    luaItem.isLock = true;
			    else
				    luaItem.isLock = false;
			    return luaItem;
		    }

		    //非法ActionItem
		    luaItem.actionItem = null;
		    luaItem.isLock = false;

		    return luaItem;
        }
        public int GetNpcId()
        {
            return CDataPool.Instance.UserBank_GetNpcId();
        }
        public BankMoney GetBagMoney()
        {
            CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();

		    int nMoney = 0;
		    if(pCharData != null)
		    {
			    nMoney = pCharData.Get_Money();
		    }
		    return TransformCoin(nMoney);
        }
    }
}