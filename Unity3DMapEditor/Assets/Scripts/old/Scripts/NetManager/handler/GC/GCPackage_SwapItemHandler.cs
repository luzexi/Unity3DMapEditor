using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCPackage_SwapItemHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCPackage_SwapItem Packet = (GCPackage_SwapItem)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
	        {
		        //-----------------------------------------------------
		        //失败的操作
		        if(Packet.Result == 0)
		        {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"包裹操作失败!");
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }

		        //-----------------------------------------------------
		        //UI数据池
                short nIndex1 = (short)Packet.PIndex1;
                short nIndex2 = (short)Packet.PIndex2;
                if (nIndex1 >= GAMEDEFINE.MAX_BAG_SIZE && nIndex2 >= GAMEDEFINE.MAX_BAG_SIZE)
                {

                    CTalisman_Item item1 = CDataPool.Instance.TalismanInventory_GetItem(nIndex1 - GAMEDEFINE.MAX_BAG_SIZE);
                    CTalisman_Item item2 = CDataPool.Instance.TalismanInventory_GetItem(nIndex2 - GAMEDEFINE.MAX_BAG_SIZE);
                    if (item1 != null && item2 == null)
                    {
                        CDataPool.Instance.TalismanInventory_SetItem((short)(nIndex2 - GAMEDEFINE.MAX_BAG_SIZE), item1 as CObject_Item, false, false);
                        CDataPool.Instance.TalismanInventory_SetItem((short)(nIndex1 - GAMEDEFINE.MAX_BAG_SIZE), null, false, false);
                    }
                    else if (item1 == null && item2 != null)
                    {
                        CDataPool.Instance.TalismanInventory_SetItem((short)(nIndex1 - GAMEDEFINE.MAX_BAG_SIZE), item2 as CObject_Item, false, false);
                        CDataPool.Instance.TalismanInventory_SetItem((short)(nIndex2 - GAMEDEFINE.MAX_BAG_SIZE), null, false, false);
                    }
                    CActionSystem.Instance.UserTalismanBag_Update();
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_TALISMANITEM_CHANGED);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

		        //背包里的装备
		        CObject_Item pItem1 = CDataPool.Instance.UserBag_GetItemByIndex(nIndex1);
		        CObject_Item pItem2 = CDataPool.Instance.UserBag_GetItemByIndex(nIndex2);

		        //全空
		        if(pItem1 == null && pItem2 == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        //Item1 -> Item2
		        else if(pItem1!=null && pItem2 == null)
		        {
			        CDataPool.Instance.UserBag_SetItem(nIndex2, pItem1, false, false);
			        CDataPool.Instance.UserBag_SetItem(nIndex1, null, false, false);
		        }
		        //Item1 <- Item2
		        else if(pItem1 == null && pItem2 != null)
		        {
			        CDataPool.Instance.UserBag_SetItem(nIndex1, pItem2, false, false);
			        CDataPool.Instance.UserBag_SetItem(nIndex2, null, false, false);
		        }
		        //Item1 <-> Item2
		        else
		        {
			       CDataPool.Instance.UserBag_SetItem(nIndex2, pItem1, false, false);
			        CDataPool.Instance.UserBag_SetItem(nIndex1, pItem2, false, false);
		        }

		        // 更新主角身上的背包到ActionSystem
                CActionSystem.Instance.UserBag_Update();

		        // 通知界面事件
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);

	        }	
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_PACKAGE_SWAPITEM;
        }
    }
}
