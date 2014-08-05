
using System;


using Network.Packets;

namespace Network.Handlers
{

    public class GCNotifyEquipHanlder : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                //LogManager.Log("RECV GCNotifyEquip");
                GCNotifyEquip packet = (GCNotifyEquip) pPacket;

		        short wBagIndex = packet.BagIndex;
		        _ITEM pItem = packet.Item;

		        CObject_Item pItemObj = ObjectSystem.Instance.NewItem(pItem.m_ItemIndex);
		        if(pItemObj==null) return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

		        pItemObj.SetGUID(pItem.m_ItemGUID.m_World, pItem.m_ItemGUID.m_Server,(uint)pItem.m_ItemGUID.m_Serial);

		        pItemObj.SetExtraInfo(ref pItem);
				if(pItem.m_Talisman == null)
				{
			        CDataPool.Instance.UserBag_SetItem(wBagIndex, pItemObj, true, true );
			        CActionSystem.Instance.UserBag_Update();
			        //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItemObj.GetID());
			        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, wBagIndex);
				}
				else
				{
			        short talismanIndex = (short)(wBagIndex- GAMEDEFINE.MAX_BAG_SIZE);
                    CDataPool.Instance.TalismanInventory_SetItem(talismanIndex, pItemObj, true, true);
                    CActionSystem.Instance.UserTalismanBag_Update();
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_TALISMANITEM, talismanIndex);
					
				}
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NOTIFYEQUIP;
        }
    }
}
