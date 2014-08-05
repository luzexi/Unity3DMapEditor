using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCItemInfoHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                //LogManager.Log("RECV GCItemInfo");
                
                GCItemInfo packet = (GCItemInfo)pPacket;
                if (packet.BagIndex >= GAMEDEFINE.MAX_BAG_SIZE)
                {
                    CDataPool.Instance.TalismanInventory_SetItemInfo(packet.BagIndex-GAMEDEFINE.MAX_BAG_SIZE, packet.IsNull == 1, packet.Item);
                    CActionSystem.Instance.UserTalismanBag_Update();
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_TALISMANITEM_CHANGED, packet.BagIndex - GAMEDEFINE.MAX_BAG_SIZE);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
                CDataPool.Instance.UserBag_SetItemInfo(packet.BagIndex, packet.IsNull == 1, packet.Item);

		        CActionSystem.Instance.UserBag_Update();
		        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED,packet.BagIndex);

		        // 得到详细信息后， 更新surpper tooltip.
		        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_SUPERTOOLTIP);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ITEMINFO;
        }
    }
}