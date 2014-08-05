using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBoxItemListHandler: HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase packet, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
                LogManager.Log("Receive GCBoxItemList Packet");
		        CObjectManager pObjectManager = CObjectManager.Instance;
                GCBoxItemList pPacket = (GCBoxItemList)packet;
		        // 清空
		        CDataPool.Instance.ItemBox_Clear();		
		        // 记录箱子id
		        CDataPool.Instance.ItemBox_SetID( (int)pPacket.getItemBoxId() );

		        // 放置每一个物品
		        for( int i = 0; i < pPacket.getItemNum(); i ++ )
		        {
                    CObject_Item pItemObj = ObjectSystem.Instance.NewItem(pPacket.getItemList()[i].m_ItemIndex);
                    if (pItemObj == null) continue;

                    pItemObj.SetGUID(
                        (ushort)((pPacket.getItemList())[i].m_ItemGUID.m_World),
                        (ushort)((pPacket.getItemList())[i].m_ItemGUID.m_Server),
                        (uint)((pPacket.getItemList())[i].m_ItemGUID.m_Serial));

                    pItemObj.SetExtraInfo(ref (pPacket.getItemList()[i]));
                    pItemObj.SetTypeOwner(ITEM_OWNER.IO_ITEMBOX);
                    pItemObj.SetPosIndex((short)i);

                    CDataPool.Instance.ItemBox_SetItem(i, pItemObj, true);

                    _ITEM pItem = (pPacket.getItemList())[i];
                    GameProcedure.s_pGameInterface.ItemBox_PickItem(pItem.m_ItemGUID, pPacket.getItemBoxId());
		        }
        	
                // 自动拾取，不再产生actionItem [3/29/2012 SUN]
                //int nObjId = CObjectManager.Instance.FindServerObject((int)pPacket.getItemBoxId()).ID;

                ////产生事件
                //CActionSystem.Instance.ItemBox_Created(nObjId);
	        }
	        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE ;

        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BOXITEMLIST;
        }
    }
}
