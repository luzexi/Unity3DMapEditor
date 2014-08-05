using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankRemoveItemHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankRemoveItem packet = pPacket as GCBankRemoveItem;
                byte	indexTo = packet.IndexTo;
			    byte	indexFrom = packet.IndexFrom;

			if(packet.ToType == (byte)GCBankRemoveItem.PosType.EQUIP_POS)
			{

			}
			else if(packet.ToType == (byte)GCBankRemoveItem.PosType.BAG_POS)
			{
				switch( packet.operateType )
				{
				case (byte)GCBankRemoveItem.OPtype.OPERATE_MOVE:		// 移动物品到空格
					{
						CObject_Item	pItem = CDataPool.Instance.UserBank_GetItem(indexFrom);
						CDataPool.Instance.UserBag_SetItem(indexTo, pItem, true, false);
						CDataPool.Instance.UserBank_SetItem(indexFrom, null, false);
						CActionSystem.Instance.UserBank_Update();
						CActionSystem.Instance.UserBag_Update();

						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"物品取出银行成功");
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItem.GetID());
					}
					break;
				case (byte)GCBankRemoveItem.OPtype.OPERATE_SPLICE:		// 叠加物品
					{
						
					}
					break;
				case (byte)GCBankRemoveItem.OPtype.OPERATE_SPLIT:		// 拆分物品
					{
						
					}
					break;
				case (byte)GCBankRemoveItem.OPtype.OPERATE_SWAP:		// 交换物品
					{
						CObject_Item	pItem  = CDataPool.Instance.UserBank_GetItem(indexFrom);
						CObject_Item	pItem1 = CDataPool.Instance.UserBag_GetItemByIndex(indexTo);
						CDataPool.Instance.UserBag_SetItem(indexTo, pItem, false, false);
						CDataPool.Instance.UserBank_SetItem(indexFrom, pItem1, false);
						CActionSystem.Instance.UserBank_Update();
						CActionSystem.Instance.UserBag_Update();

						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"银行和背包交换物品成功");
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
						CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItem.GetID());
					}
					break;
				default:
					break;
				}

			}
			else if(packet.ToType== (byte)GCBankRemoveItem.PosType.ERROR_POS)
			{
				CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"移动物品失败");
			}
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKREMOVEITEM;
        }
    }
}