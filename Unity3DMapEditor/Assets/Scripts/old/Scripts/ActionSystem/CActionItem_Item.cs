/****************************************\
*										*
* 			  操作管理器-物品			*
*										*
\****************************************/

using System;
using System.Collections.Generic;
using Network;
using Network.Packets;
using Interface;
using UnityEngine;

public class CActionItem_Item : CActionItem
{
    public CActionItem_Item(int id)
        : base(id)
    {

    }
    //用于物品操作时，物品ID(指向CObject_Item)
    int m_idItemImpl;
    public int IDItemImpl
    {
        get { return m_idItemImpl; }
        set { m_idItemImpl = value; }
    }
    public CObject_Item ItemImpl { get; set; }
    //得到物品
    public override object GetImpl() { return ItemImpl; }

    public override ACTION_OPTYPE GetType() { return ACTION_OPTYPE.AOT_ITEM; }

    public void Update_Item(CObject_Item item)
    {
        ItemImpl = item;
        //引用
        IDItemImpl = item.GetID();
        //名称
        m_strName = item.GetName();
        //图标
        if (item.GetIconName() != "")
            m_strIconName = item.GetIconName();
        //通知UI
        UpdateToRefrence();
    }

    //得到数量
    public override int GetNum() { return ItemImpl.GetNumber(); }

    //类型字符串
    public override ActionNameType GetType_String() 
    {
        ActionNameType typeName = ActionNameType.UnName;
        if (ItemImpl == null)
            return typeName;
        switch (ItemImpl.GetTypeOwner())
        {
            case ITEM_OWNER.IO_UNKNOWN:
                break;
            case ITEM_OWNER.IO_MYSELF_EQUIP:
                typeName = ActionNameType.Equip;
                break;
            case ITEM_OWNER.IO_MYSELF_PACKET:
                typeName = ActionNameType.PackageItem;
                break;
            case ITEM_OWNER.IO_MYSELF_BANK:
                typeName = ActionNameType.bankItem;
                break;
            case ITEM_OWNER.IO_PLAYEROTHER_EQUIP:
                break;
            case ITEM_OWNER.IO_ITEMBOX:
                break;
            case ITEM_OWNER.IO_BOOTH:
                typeName = ActionNameType.boothItem;
                break;
            case ITEM_OWNER.IO_MYEXBOX:
                break;
            case ITEM_OWNER.IO_OTHEREXBOX:
                break;
            case ITEM_OWNER.IO_MISSIONBOX:
                break;
            case ITEM_OWNER.IO_MYSTALLBOX:
                break;
            case ITEM_OWNER.IO_OTSTALLBOX:
                break;
            case ITEM_OWNER.IO_QUESTVIRTUALITEM:
                break;
            case ITEM_OWNER.IO_PS_SELFBOX:
                break;
            case ITEM_OWNER.IO_PS_OTHERBOX:
                break;
            case ITEM_OWNER.IO_BOOTH_CALLBACK:
                typeName = ActionNameType.boothItemCallBack;
                break;
            case ITEM_OWNER.IO_GEM_SEPARATE:
                break;
            case ITEM_OWNER.IO_TRANSFER_ITEM:
                break;
            case ITEM_OWNER.IO_CITY_RESEARCH:
                break;
            case ITEM_OWNER.IO_CITY_SHOP:
                break;
            case ITEM_OWNER.IO_QUESTUI_DEMAND:
                break;
            case ITEM_OWNER.IO_QUESTUI_REWARD:
                break;
            default:
                break;
        }
        return typeName;
    }

    //////////////////////////////////////////////////////////////////////////
    // 所有标记 [7/14/2011 ivan edit]
    // case 'S':		//摆摊界面
    // case 'Q':		//任务提交界面(MissionReply)
    // case 'B':		//银行
    // case 'P':		//背包
    // case 'E':		//交易界面,只能拖动到自己的（右边的）格子里头（1~6）
    // case 'R':		//银行的界面上的租赁箱格子
    // case 'G':		//宝石合成/镶嵌界面
    // case 'C':		//装备打孔 [7/14/2011 ivan edit]
    // case 'T':		//宠物技能学习/还童/延长寿命/宠物驯养/宠物征友
    // case 'Z':		//人物装备界面
    // case 'A':		//玩家商店
    // case 'M':		//主菜单
    // case 'Y':		//对象头像
    // case 'L':		//自己头像
    // case 'I':		// 装备强化 [7/18/2011 ivan edit]
    // case 'E':        // 宠物装备
    //////////////////////////////////////////////////////////////////////////
    public override void NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName) 
    {
        //LogManager.LogWarning("Drag From:" + szSourceName + " to:" + szTargetName);
        //拖动到表示销毁的地方
        if (bDestory)
        {
            DestoryItem(szSourceName);
            return;
        }
        //拖动到空白地方
        if (szTargetName == "")
        {
            return;
        }

        char cSourceName = szSourceName[0];
        char cTargetType = szTargetName[0];
        //如果是快捷栏，不能往其他地方拖，除了自己本身。
        if (cSourceName == 'M' &&
             cTargetType != 'M')
            return;

        switch (cTargetType)
        {
            case 'P':
                NotifyPacketDrag(szSourceName, szTargetName);
                break;
            case 'E':
                {
                    ITEM_OWNER typeOwner = ItemImpl.TypeOwner;
                    switch (typeOwner)
                    {
                        case ITEM_OWNER.IO_MYSELF_PACKET:			
                            {
                                // 发送装备宠物装备消息.
                                CObject_Item_Equip curEquipt = ItemImpl as CObject_Item_Equip;
                                if (GameProcedure.s_pUISystem != null &&
                                    UISystem.Instance.IsWindowShow("RoleTipWindow") &&
                                    curEquipt != null)
                                {
                                    GameObject roleTip = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
                                    UISelfEquip selfEquip = roleTip.GetComponent<UISelfEquip>();

                                    SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(selfEquip.ActivePet); //CDataPool.Instance.Pet_GetPet(selfEquip.ActivePet);
                                    int tarIndex = Convert.ToInt32(szTargetName.Substring(1, szTargetName.Length - 1)) - 1;
                                    if((int)curEquipt.GetItemType() != tarIndex)
                                    {
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该装备不能装备到目标位置");
                                    }
                                    else if(!curEquipt.IsPetEquipt())
                                    {
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该装备不能装备到宠物上");
                                    }
                                    else if (curPet != null &&
                                        curEquipt != null)
                                    {
                                        CGOperatePetEquip msg = new CGOperatePetEquip();
                                        msg.OperatorType = 0;
                                        msg.GUID = curPet.GUID;
                                        msg.DestBagIndex = (byte)curEquipt.GetItemType();
                                        msg.SourecBagIndex = (byte)GetPosIndex();
                                        NetManager.GetNetManager().SendPacket(msg);
                                    }
                                }
                            }
                            break;
                    }
                }
                break;
            case 'B'://bank
                {
                    int tarIndex = Convert.ToInt32(szTargetName.Substring(1, szTargetName.Length - 1)) - 1;

                    if (!CDataPool.Instance.UserBank_IsValid(tarIndex))
                    {
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "格子未解封");
                        return;
                    }
                    //根据物品的来源决定发送什么消息
			        ITEM_OWNER typeOwner = ItemImpl.TypeOwner;
			        //任务物品不能放入银行
			        if(ItemImpl.GetItemClass() == ITEM_CLASS.ICLASS_TASKITEM)
			        {
				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "任务物品不能放入");
				        return;
			        }
                    switch (typeOwner) 
			        {
			        case ITEM_OWNER.IO_MYSELF_PACKET:			//来自玩家的背包
				        {
					        CGBankAddItem msg = new CGBankAddItem();
					        msg.FromType = (byte)(CGBankAddItem.AddType.BAG_POS);
					        msg.IndexFrom = (byte)GetPosIndex();
					        msg.IndexTo = (byte)tarIndex ;
					        NetManager.GetNetManager().SendPacket(msg);
				        }
				        break;
			        case ITEM_OWNER.IO_MYSELF_BANK:				//来自银行内部
				        {
					        CGBankSwapItem msg = new CGBankSwapItem();
					        msg.FromType = (byte)CGBankSwapItem.AddType.BANK_POS;
					        msg.IndexFrom = (byte)GetPosIndex();
					        msg.IndexTo = (byte)tarIndex;
					        msg.ToType = (byte)(CGBankSwapItem.AddType.BANK_POS);
					        if( msg.IndexFrom == msg.IndexTo )
						        break;
					        NetManager.GetNetManager().SendPacket(msg);
				        }
				        break;
			        default:
				        break;
			        }
                }
                break;
        }
        //switch(cSourceName)
        //{
        //case 'M':
        //    {
        //        INT nIndex = szSourceName[1]-'0';
        //        nIndex = szSourceName[2]-'0' + nIndex*10 ;
        //        CActionSystem::GetMe()->MainMenuBar_Set(nIndex, nOldTargetId);
        //    }
        //    break;
        //default:
        //    break;
        //}

        //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP);
    }

    public override int GetPosIndex() 
    { 
        return ItemImpl.PosIndex; 
    }

    void NotifyPacketDrag(string sName,string tName)
    {
        int srcIndex = Convert.ToInt32(sName.Substring(1, sName.Length-1)) -1;
        int tarIndex = Convert.ToInt32(tName.Substring(1, tName.Length-1)) -1;
		ITEM_OWNER nTypeOwner = ItemImpl.TypeOwner;
        
        switch (nTypeOwner)
	    {
			//Package -> Package
            case ITEM_OWNER.IO_MYSELF_PACKET:
                {
					//同一格
                    
                    if (srcIndex == tarIndex) 
                        break;

					//不同格
                    CGPackage_SwapItem msg = new CGPackage_SwapItem();
                    msg.PIndex1 = (byte)srcIndex;
                    msg.PIndex2 = (byte)tarIndex;
                    NetManager.GetNetManager().SendPacket(msg);
                }
                break;
            case ITEM_OWNER.IO_PET_EQUIPT:
                {
                    // 发送卸下宠物装备消息.
                    CObject_Item_Equip curEquipt = ItemImpl as CObject_Item_Equip;
                    if (GameProcedure.s_pUISystem != null &&
                       UISystem.Instance.IsWindowShow("RoleTipWindow") &&
                       curEquipt != null)
                    {
                        GameObject roleTip = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
                        UISelfEquip selfEquip = roleTip.GetComponent<UISelfEquip>();
                        SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(selfEquip.ActivePet);//CDataPool.Instance.Pet_GetPet(selfEquip.ActivePet);
                        if (curPet != null)
                        {
                            CGOperatePetEquip msg = new CGOperatePetEquip();
                            msg.OperatorType = 1;
                            msg.GUID = curPet.GUID;
                            msg.DestBagIndex = (byte)curEquipt.GetItemType();
                            msg.SourecBagIndex = (byte)tarIndex;
                            NetManager.GetNetManager().SendPacket(msg);
                        }
                    }
                }
                break;
            default:
             break;
	    }
			
// 			switch(nTypeOwner)
// 			{
// 				//jiaoyikuang->Package
// 			case tObject_Item::IO_MYEXBOX:
// 				{
// 					//发送拖动物品的消息
// 					CGExchangeSynchItemII msg;
// 					msg.SetOpt(EXCHANGE_MSG::OPT_REMOVEITEM);
// 					msg.SetFromIndex(this->GetPosIndex());
// 					msg.SetToType(EXCHANGE_MSG::POS_BAG);
// 					msg.SetToIndex(nTargetIndex);
// 					CNetManager::GetMe()->SendPacket(&msg);
// 				}
// 				break;
// 			//Bank -> Package
// 			case tObject_Item::IO_MYSELF_BANK:
// 				{
// 					CGBankRemoveItem msg;
// 					msg.SetToType(CGBankRemoveItem::BAG_POS);
// 					msg.SetIndexFrom(this->GetPosIndex());
// 					msg.SetIndexTo(nTargetIndex);
// 
// 					CNetManager::GetMe()->SendPacket(&msg);
// 				}
// 				break;
// 			//Package -> Package
// 			case tObject_Item::IO_MYSELF_PACKET:
// 				{
// 					//同一格
// 					if(this->GetPosIndex() == nTargetIndex) break;
// 
// 					//不同格
// 					CGPackage_SwapItem msg;
// 					msg.SetPackageIndex1(GetPosIndex());
// 					msg.SetPackageIndex2(nTargetIndex);
// 
// 					CNetManager::GetMe()->SendPacket(&msg);
// 				}
// 				break;
// 			//SelfEquip -> package
// 			case tObject_Item::IO_MYSELF_EQUIP:
// 				{ 
// 					//发送卸下装备的消息
// 					CGUnEquip msg;
// 					msg.setEquipPoint(this->GetPosIndex());
// 					msg.setBagIndex((BYTE)nTargetIndex);
// 
// 					CNetManager::GetMe()->SendPacket(&msg);
// 					break;
// 				}
// 			//playerShop -> package
// 			case tObject_Item::IO_PS_SELFBOX:
// 				{
// 					_ITEM_GUID Guid;
// 					tObject_Item::ITEM_GUID temp;
// 					temp.m_idUnion = 0;
// 
// 					tObject_Item* pSelectItem = this->GetItemImpl();
// 					if(NULL == pSelectItem)
// 					{
// 						return;
// 					}
// 					
// 					//需要先判定这个物品是否满足移动的条件
// 					//查询这个物品是不是处于上架的状态
// 					INT nIndex = pSelectItem->GetPosIndex();
// 					INT nConIndex = nIndex/20;
// 					INT nPosition = nIndex%20;
// 					if(CDataPool::GetMe()->PlayerShop_GetItemOnSale(TRUE, nConIndex, nPosition))
// 					{
// 						//已经上架，不能取回
// 						return;
// 					}
// 
// 					pSelectItem->GetGUID(temp.m_idOrg.m_idWorld, temp.m_idOrg.m_idServer, temp.m_idOrg.m_uSerial);
// 					Guid.m_World	= (BYTE)temp.m_idOrg.m_idWorld;
// 					Guid.m_Server	= (BYTE)temp.m_idOrg.m_idServer;	
// 					Guid.m_Serial	= (INT)temp.m_idOrg.m_uSerial;
// 
// 					CGItemSynch msg;
// 					msg.SetOpt(CGItemSynch::OPT_MOVE_ITEM_MANU);
// 					msg.SetFromType(CGItemSynch::POS_PLAYERSHOP);
// 					msg.SetToIndex(nTargetIndex);
// 					msg.SetToType(CGItemSynch::POS_BAG);
// 					msg.SetItemGUID(Guid);
// 
// 					INT nPage = CDataPool::GetMe()->PlayerShop_GetMySelectConTa();
// 					CGManuMoveItemFromPlayerShopToBag_t ExtraDataInfo;
// 					ExtraDataInfo.m_ShopGuid	= CDataPool::GetMe()->PlayerShop_GetShopID(TRUE);
// 					ExtraDataInfo.m_nStallIndex = (BYTE)nPage;
// 					ExtraDataInfo.m_uSerial		= CDataPool::GetMe()->PlayerShop_GetItemSerial(TRUE,nPage,this->GetPosIndex()%20);
// 					msg.SetExtraInfoLength(ExtraDataInfo.GetSize());
// 					msg.SetExtraInfoData((BYTE*)&ExtraDataInfo);
// 				
// 					CNetManager::GetMe()->SendPacket(&msg);
// 				}
// 				break;
// 			default:
// 				break;
// 			}
    }

    private void DestoryItem(string szSourceName)
    {
        char cSourceType = szSourceName[0];
	    switch(cSourceType)
	    {
	    case 'M':		//主菜单
		    {
			    int nIndex = int.Parse(szSourceName.Substring(1));
			    CActionSystem.Instance.MainMenuBar_Set(nIndex, -1 );
			    return;
		    }
	    default:
		    break;
	    }

        ITEM_OWNER itemOwner = ItemImpl.TypeOwner;
        if (itemOwner == ITEM_OWNER.IO_MYSELF_PACKET || itemOwner == ITEM_OWNER.IO_MYSELF_BANK)
        {
            if (itemOwner == ITEM_OWNER.IO_MYSELF_PACKET && ItemImpl.GetItemClass() == ITEM_CLASS.ICLASS_TASKITEM)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "任务道具不能丢弃");
            }
            else
            {
                // 先取消可能存在的锁定
			    int nTypeOwner	=	CDataPool.Instance.DisCard_GetItemConta();
			    int	nItemPos	=	CDataPool.Instance.DisCard_GetItemPos();
                switch( (ITEM_OWNER)nTypeOwner ) 
			    {
			    case ITEM_OWNER.IO_MYSELF_PACKET:				// 来自玩家的背包
				    {
					    CObject_Item pItem = CDataPool.Instance.UserBag_GetItem(nItemPos);

					    if(pItem!=null)
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

					    if(pItem != null)
					    {
                            pItem.SetLock(false);
					    }
				    }
				    break;
			    default:
				    break;
			    }
                // 需要先询问是否销毁这个装备
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OPEN_DISCARD_ITEM_FRAME,ItemImpl.GetName());

			    // 锁定这个物品
			    ItemImpl.SetLock(true);

			    //通知背包锁定状态
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);

			    CDataPool.Instance.DisCard_SetItemConta((int)ItemImpl.TypeOwner);
			    CDataPool.Instance.DisCard_SetItemPos(GetPosIndex());
            }
        }

    }
    public override void DestroyImpl()
    {
        if (ItemImpl != null)
            ObjectSystem.Instance.DestroyItem(ItemImpl);
        ItemImpl = null;

    }
    public override void DoAction()
    {
        base.DoAction();

        if (ItemImpl == null)
            return;
        
        //处理拆分
        //...TODO

        // 发送物品链接
        //...TODO

        switch (ItemImpl.GetTypeOwner())
        {
            case ITEM_OWNER.IO_UNKNOWN:
                break;
            case ITEM_OWNER.IO_MYSELF_EQUIP:
                DoAction_MyEquip();
                break;
            case ITEM_OWNER.IO_PET_EQUIPT:
                DoAction_PetEquip();
                break;
            case ITEM_OWNER.IO_MYSELF_PACKET://玩家自己身上的包中
                DoAction_Packet();
                break;
            case ITEM_OWNER.IO_MYSELF_BANK:
                {
                    CGBankRemoveItem msg = new CGBankRemoveItem();
			        msg.ToType = (byte)(CGBankRemoveItem.RemoveType.BAG_POS);
			        msg.IndexFrom = (byte)GetPosIndex();
			        msg.IndexTo = 255;

			        NetManager.GetNetManager().SendPacket(msg);
                }
                break;
            case ITEM_OWNER.IO_PLAYEROTHER_EQUIP:
                break;
            case ITEM_OWNER.IO_ITEMBOX:
                break;
            case ITEM_OWNER.IO_BOOTH:
                {
                    DoSubAction();
                }
                break;
            case ITEM_OWNER.IO_MYEXBOX:
                break;
            case ITEM_OWNER.IO_OTHEREXBOX:
                break;
            case ITEM_OWNER.IO_MISSIONBOX:
                break;
            case ITEM_OWNER.IO_MYSTALLBOX:
                break;
            case ITEM_OWNER.IO_OTSTALLBOX:
                break;
            case ITEM_OWNER.IO_QUESTVIRTUALITEM:
                break;
            case ITEM_OWNER.IO_PS_SELFBOX:
                break;
            case ITEM_OWNER.IO_PS_OTHERBOX:
                break;
            case ITEM_OWNER.IO_BOOTH_CALLBACK:
                {
            //        if(CActionSystem::GetMe()->GetDefaultAction() == CActionItem_MouseCmd_Repair::GetMe())
            //{
            //    CEventSystem::GetMe()->PushEvent(GE_INFO_SELF,"不能修理此物品");
            //    break;
            //}

                    if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money() >=
                        (int)(CDataPool.Instance.Booth_GetSoldPrice(this.GetPosIndex())))
                    {
                        _ITEM_GUID Guid;
                        ITEM_GUID temp = new ITEM_GUID();

                        ItemImpl.GetGUID(ref temp.m_idWorld, ref temp.m_idServer, ref temp.m_uSerial);
                        Guid.m_World = (byte)temp.m_idWorld;
                        Guid.m_Server = (byte)temp.m_idServer;
                        Guid.m_Serial = (int)temp.m_uSerial;

                        CGShopBuy msg = new CGShopBuy();
                        msg.UniqueID = CDataPool.Instance.Booth_GetShopUniqueId();
                        msg.Index = (byte)(ItemImpl.PosIndex + 200);
                        msg.ItemGuid = Guid;
                        msg.SerialNum = (byte)CDataPool.Instance.Booth_GetSerialNum();

                        NetManager.GetNetManager().SendPacket(msg);
                    }
                    else
                    {
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "#{LowMoney}");
                    }
                }
                break;
            case ITEM_OWNER.IO_GEM_SEPARATE:
                break;
            case ITEM_OWNER.IO_TRANSFER_ITEM:
                break;
            case ITEM_OWNER.IO_CITY_RESEARCH:
                break;
            case ITEM_OWNER.IO_CITY_SHOP:
                break;
            case ITEM_OWNER.IO_QUESTUI_DEMAND:
                break;
            case ITEM_OWNER.IO_QUESTUI_REWARD:
                break;
            default:
                break;
        }
    }
    public override void DoSubAction()
    {
        if (ItemImpl == null)
            return;

        if (ItemImpl.GetTypeOwner() == ITEM_OWNER.IO_BOOTH)
        {
            if (CDataPool.Instance.Booth_GetShopType() == 0)//虚拟货币
            {
                if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money() >= ItemImpl.GetItemPrice())
                {
                    GameProcedure.s_pGameInterface.Booth_BuyItem( ItemImpl );
                }
                else
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "#{LowMoney}");
                }
            }//else if
        }
        base.DoSubAction();
    }
    private void DoAction_MyEquip()
    {
        // 如果action item在装备栏上.
// 		if(!CDataPool.Instance.Booth_IsClose())
// 		{
// 			//在修理状态
// 			if(CActionSystem::GetMe()->GetDefaultAction() == CActionItem_MouseCmd_Repair::GetMe())
// 			{
// 				//判以下是不是装备,只有装备可以修理
// 				if(pItem->GetItemClass() == ICLASS_EQUIP)
// 				{
// 					if( pItem->GetItemDur() < pItem->GetItemMaxDur() )
// 					{
// 						CGameProcedure::s_pGameInterface->Booth_Repair(0, pItem->GetPosIndex(), FALSE);
// 					}
// 					else
// 					{
// 						CEventSystem::GetMe()->PushEvent(GE_INFO_SELF,"此物品不需要修理");
// 					}
// 				}
// 			}
// 		}
// 		else
		{
			CObject_Item_Equip pItemEquip = (CObject_Item_Equip)ItemImpl;
			if(pItemEquip.GetItemClass() != ITEM_CLASS.ICLASS_EQUIP)
			{
				return;
			}

            // 发送卸下装备消息.
            CGUnEquip msg = new CGUnEquip();
            msg.EquipPoint = (byte)pItemEquip.GetItemType();
            NetManager.GetNetManager().SendPacket(msg);
		}
    }

    private void DoAction_PetEquip()
    {
        CObject_Item_Equip pItemEquip = (CObject_Item_Equip)ItemImpl;
        if (pItemEquip.GetItemClass() != ITEM_CLASS.ICLASS_EQUIP)
        {
            return;
        }
        int emptyPlace = CDataPool.Instance.UserBag_FindFirstEmptyPlace();
        if (emptyPlace == -1)
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "背包已满，清整理背包后再尝试 ^0^!");
        }
        else
        {
            // 发送卸下宠物装备消息.
            GameObject roleTip = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
            UISelfEquip selfEquip = roleTip.GetComponent<UISelfEquip>();
            SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(selfEquip.ActivePet);//CDataPool.Instance.Pet_GetPet(selfEquip.ActivePet);
            if (curPet != null)
            {
                CGOperatePetEquip msg = new CGOperatePetEquip();
                msg.OperatorType = 1;
                msg.GUID         = curPet.GUID;
                msg.DestBagIndex = (byte)pItemEquip.GetItemType();
                msg.SourecBagIndex = (byte)emptyPlace;
                NetManager.GetNetManager().SendPacket(msg);
            }
        }
    }

    private void DoAction_Packet()
    {
        // 判断商店是否开启 [2/20/2012 Ivan]
        if (UISystem.Instance.IsWindowShow("ShopWindow"))
        {
        //    //是否在修理状态
        //if(CActionSystem::GetMe()->GetDefaultAction() == CActionItem_MouseCmd_Repair::GetMe())
        //{
        //    //取消修理
        //    CGameProcedure::s_pGameInterface->Skill_CancelAction();
        //}
        ////执行销售
        //else
            {
                //任务物品不可以卖
                if (ItemImpl.GetItemClass() == ITEM_CLASS.ICLASS_TASKITEM)
                {
                    //任务物品不能卖的信息提示
                    //if(bLog)
                    //{
                    //    STRING strTemp = "这件物品不能出售";//NOCOLORMSGFUNC("stall_sold_failure");
                    //    ADDNEWDEBUGMSG(strTemp);
                    //}
                }
                else
                {
                    GameProcedure.s_pGameInterface.Booth_SellItem( ItemImpl );
                }
            }

            return;
        }
        //银行开启
        else if (GameProcedure.s_pUISystem != null && UISystem.Instance.IsWindowShow("StoreWindow"))
        {
             //需要先判定是不是能够存入银行的物品
		    if( ItemImpl != null && ItemImpl.GetItemClass() == ITEM_CLASS.ICLASS_TASKITEM )
		    //if(pItem->GetPosIndex() >= TASK_CONTAINER_OFFSET )
		    {
			    //任务物品不能放入银行
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"任务物品不能存入银行");
			    return;
		    }	
    		

		    CGBankAddItem msg = new CGBankAddItem();
		    msg.FromType = (byte)(CGBankAddItem.AddType.BAG_POS);
		    msg.IndexFrom = (byte)(GetPosIndex());
            msg.IndexTo = (byte)CGBankAddItem.AutoPosBox.AUTO_POS_BOX1;

		    //检测这个租赁箱是不是有空位
		    if( !CDataPool.Instance.UserBank_IsEmpty( 1 ) )
		    {
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"仓库已经满了");
			    return;
		    }

		    NetManager.GetNetManager().SendPacket(msg);

		    return;
        }
        else// 没有打开任何其他窗口的时候 [2/20/2012 Ivan]
        {
            switch (ItemImpl.GetItemClass())
            {
                case ITEM_CLASS.ICLASS_EQUIP:
			        //装备
			        {
				        //使用
                        CObject_Item_Equip curEquipt = ItemImpl as CObject_Item_Equip;
                        //装备宠物
                        if (GameProcedure.s_pUISystem != null && 
                            UISystem.Instance.IsWindowShow("RoleTipWindow") &&
                            curEquipt != null)
                        {
                            if(!curEquipt.IsPetEquipt())
                            {
                                GameInterface.Instance.PacketItem_UserEquip(ItemImpl);// CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该装备不是宠物装备");
                            }
                            else
                            {
                                GameObject roleTip = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
                                UISelfEquip selfEquip = roleTip.GetComponent<UISelfEquip>();
                                SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(selfEquip.ActivePet);//CDataPool.Instance.Pet_GetPet(selfEquip.ActivePet);
                                if(curPet != null)
                                {
                                    CGOperatePetEquip msg = new CGOperatePetEquip();
                                    msg.OperatorType = 0;
                                    msg.GUID = curPet.GUID;
                                    msg.SourecBagIndex = (byte)GetPosIndex();
                                    msg.DestBagIndex   = (byte)curEquipt.GetItemType();
                                    NetManager.GetNetManager().SendPacket(msg);
                                }
                            }
                        }
                        else
                        {
                            if (curEquipt != null)
                            {
                                if (!curEquipt.IsPetEquipt())
                                {
                                    GameInterface.Instance.PacketItem_UserEquip(ItemImpl);
                                }
                                else
                                {
                                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该装备只能给宠物使用");
                                }
                            }
                            else
                            {
                                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该物品不是装备");
                            }
                        }
			        }
                    break;
                case ITEM_CLASS.ICLASS_MATERIAL:
                    break;
                case ITEM_CLASS.ICLASS_COMITEM:
                case ITEM_CLASS.ICLASS_TASKITEM:
                case ITEM_CLASS.ICLASS_STOREMAP:
                    {
				        int iType = ItemImpl.GetItemTableType();

				        //摆摊时不可以随便使用物品
// 				        if(TRUE == CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_IsInStall())
// 				        {
// 					        CEventSystem::GetMe()->PushEvent(GE_INFO_SELF,"你正在摆摊……");
// 					        break;
// 				        }

				        //验证是否可以使用
                        if(!ItemImpl.Rule(ITEM_RULE.RULE_USE))
	                    {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, ItemImpl.RuleFailMessage(ITEM_RULE.RULE_USE));
		                    return;
	                    }

				        //检查冷却是否结束
				        if(!CoolDownIsOver()) 
				        {
                            CActionSystem.Instance.SetoutAction = this;
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,
                                GameDefineResult.Instance.GetOResultText( OPERATE_RESULT.OR_COOL_DOWNING ));
					        break;
				        }
				        CObject pMainTarget = CObjectManager.Instance.GetMainTarget();
                        //使用物品时自动下坐骑  [3/14/2012 ZZY]
                        bool result = CObjectManager.Instance.getPlayerMySelf().UnRideUseItem(this,
                            (pMainTarget != null ? pMainTarget.ServerID : -1),
                            new Vector2(-1, -1));

                        if (!result)
                        {
                            GameInterface.Instance.PacketItem_UserItem(this, (pMainTarget != null? pMainTarget.ServerID:-1),
                            new Vector2(-1,-1));
                        }

                    }
                    break;
                case ITEM_CLASS.ICLASS_GEM:
                    break;
                case ITEM_CLASS.ICLASS_SYMBOLITEM://  [3/22/2012 ZZY]
                    //请求使用符印
                    GameInterface.Instance.PacketItem_UserSymbol(this);
                    break;
                case ITEM_CLASS.ICLASS_TALISMAN:
                    break;
                case ITEM_CLASS.ICLASS_GUILDITEM:
                    break;
                case ITEM_CLASS.ICLASS_NUMBER:
                    break;
                default:
                    break;
            }
        }

    }

    public void SendAskItemInfoMsg()
    {
        //  [10/21/2011 Sun]
        CObject_Item pItem = (CObject_Item)GetImpl();
        if (pItem == null)
            return;
        if (pItem.TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
        {
            if (pItem.IsItemHasDetailInfo())
            {
                CGAskItemInfo msg = new CGAskItemInfo();
                msg.BagIndex = pItem.GetPosIndex();
                NetManager.GetNetManager().SendPacket(msg);
            }
        }
    }

    public int GetStrengthLevel()
    {
        CObject_Item pItem = GetImpl() as CObject_Item;
        if (pItem != null)
        {
            return pItem.GetStrengthLevel();
        }
        return -1;
    }

    public override string GetDesc()
    {
        CObject_Item pItem = (CObject_Item)GetImpl();
        if (pItem == null)
            return "";
        return pItem.GetDesc();
    }
    public override int GetIDTable()
    {
        CObject_Item pItem = GetImpl() as CObject_Item;
        if (pItem != null)
            return pItem.GetIdTable();
        return -1;
    }
    public int GetItemLevel()
    {
        CObject_Item pItem = GetImpl() as CObject_Item;
        if (pItem != null)
        {
            return pItem.GetNeedLevel();
        }
        return -1;
    }

    public int GetNeedJob()
    {
        CObject_Item_Equip pItem = GetImpl() as CObject_Item_Equip;
        if (pItem != null)
        {
            return pItem.GetNeedJob();
        }
        return -1;
    }

    public string GetEquipType()
    {
        CObject_Item_Equip pItem = GetImpl() as CObject_Item_Equip;
        if (pItem != null)
        {
            switch (pItem.GetItemType())
            {
                case HUMAN_EQUIP.HEQUIP_WEAPON:
                    return "武器";
                    //break;
                case HUMAN_EQUIP.HEQUIP_CAP:
                    return "头盔";
                    //break;
                case HUMAN_EQUIP.HEQUIP_ARMOR:
                    return "盔甲";
                   // break;
                case HUMAN_EQUIP.HEQUIP_CUFF:
                    return "手套";
                    //break;
                case HUMAN_EQUIP.HEQUIP_BOOT:
                    return "鞋子";
                    //break;
                case HUMAN_EQUIP.HEQUIP_SASH:
                    return "腰带";
                    //break;
                case HUMAN_EQUIP.HEQUIP_RING:
                    return "戒指";
                    //break;
                case HUMAN_EQUIP.HEQUIP_NECKLACE:
                    return "项链";
                    //break;
                case HUMAN_EQUIP.HEQUIP_BACK:
                    return "背饰";
                    //break;
                case HUMAN_EQUIP.HEQUIP_FABAO:
                    return "法宝";
                    //break;
                case HUMAN_EQUIP.HEQUIP_RING2:
                    return "戒指";
                    //break;
                case HUMAN_EQUIP.HEQUIP_CHARM:
                    return "护符";
                    //break;
                case HUMAN_EQUIP.HEQUIP_JADE:
                    return "玉佩";
                    //break;
                case HUMAN_EQUIP.HEQUIP_WRIST:
                    return "护腕";
                    //break;
                case HUMAN_EQUIP.HEQUIP_SHOULDER:
                    return "护肩";
                    //break;
                default:
                    break;
            }
        }
        return "";
    }
}
