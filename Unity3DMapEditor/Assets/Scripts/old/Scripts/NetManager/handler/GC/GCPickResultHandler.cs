

using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCPickResultHandler: HandlerBase
	{
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase Packet, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
                LogManager.Log("Receive GCPickResult Packet");
                GCPickResult pPacket = (GCPickResult)Packet;
		        CObjectManager pObjectManager = CObjectManager.Instance;
		        CDataPool pDataPool = GameProcedure.s_pDataPool;
		        //if( pPacket.getItemBoxId() != pDataPool.GetItemBoxID() )
        		
		        bool bLog = (GameProcedure.s_pUISystem != null)?true:false;
		        // 放置每一个物品
		        if( pPacket.getResult() == (byte)PickResultCode.PICK_SUCCESS )
		        {
			        int nIndex = 0;
			        CObject_Item pItem = CDataPool.Instance.ItemBox_GetItem(
											        pPacket.getItemID().m_World,
											        pPacket.getItemID().m_Server,
											        pPacket.getItemID().m_Serial,
											        ref nIndex);

			        if(pItem == null) 
			        {
				        if(bLog) GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, "内部错误，非法的物品GUID!");
				        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
			        }

			        // 先保存物品名称，后面可能会删除掉落包里的物品 [6/13/2011 ivan edit]
			        string strItemName = pItem.GetName();

			        //如果发生物品转移，(从箱子里面移到包裹里),则改变客户端id，以表示在客户端是不同物体
                    ObjectSystem.Instance.ChangeItemClientID(pItem);
			        ((CObject_Item)pItem).SetTypeOwner(ITEM_OWNER.IO_MYSELF_PACKET);

			        CObject_Item pItemBag = CDataPool.Instance.UserBag_GetItem(pPacket.getBagIndex());
			        if(pItemBag != null)
			        {//有东西,应该一定是一类物品
				        if(pItem.GetIdTable() == pItemBag.GetIdTable())
				        {//同一类物品
					        //CDataPool.GetMe().ItemBox_SetItem(nIndex, NULL, FALSE);
					        // 不删除会泄露内存 [6/10/2011 ivan edit]
					        CDataPool.Instance.ItemBox_SetItem(nIndex, null, true);
				        }
				        else
				        {
                            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
				        }
			        }
			        else
			        {//没东西
				        //存入数据池
				        CDataPool.Instance.UserBag_SetItem(pPacket.getBagIndex(), pItem, true, true);
                        CDataPool.Instance.ItemBox_SetItem(nIndex, null, false);

                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItem.GetID());
			        }
        			
			        //通知ActionButton
			        CActionSystem.Instance.UserBag_Update();
			        CActionSystem.Instance.ItemBox_Update();

			        //产生ui事件
			        if(bLog)
			        {
                        //ADDTALKMSG(strItemName); //todo
			        }

                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_LOOT_SLOT_CLEARED);
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, pPacket.getBagIndex());

			        //如果掉落箱空，关闭
                    if (CDataPool.Instance.ItemBox_GetNumber() == 0)
			        {
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_LOOT_CLOSED);
			        }
		        }
		        else
		        {
			        switch( pPacket.getResult() )
			        {
                        case (byte)PickResultCode.PICK_BAG_FULL:
				        {
					        if(bLog) 
					        {
						        GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, ("背包已满!"));
					        }
					        //CSoundSystemFMod._PlayUISoundFunc(96); todo
				        }
				        break;
                        case (byte)PickResultCode.PICK_INVALID_OWNER:
				        {
					        if(bLog) GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, ("不属于自己的背包!"));
				        }
				        break;
                        case (byte)PickResultCode.PICK_INVALID_ITEM:
				        {
					        if(bLog) GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, ("非法物品!"));
				        }
				        break;
                        case (byte)PickResultCode.PICK_TOO_FAR:
				        {
					        if(bLog) GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, ("距离太远!"));
				        }
				        break;
			        default:
				        break;
			        }
		        }
	        }
	        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE ;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_PICKRESULT;
        }
	};

}

