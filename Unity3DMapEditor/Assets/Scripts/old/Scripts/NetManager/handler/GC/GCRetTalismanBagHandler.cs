using System;
using System.Collections.Generic;
using Network.Packets;
namespace Network.Handlers
{
    public class GCRetTalismanBagHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCRetTalismanBag packet = pPacket as GCRetTalismanBag;
                CDataPool.Instance.TalismanEquipment_UnLockCount(packet.EquipCount);
                CDataPool.Instance.TalismanInventory_UnLockCount(packet.BagCount);
                for (byte i = 0; i < packet.BagItemCount; i++)
                {
                    CDataPool.Instance.TalismanInventory_SetItem(packet.BagItems[i].m_nIndex- GAMEDEFINE.MAX_BAG_SIZE,ref packet.BagItems[i].m_nItemData);
                }
                for (byte i = 0; i < packet.EquipItemCount; i++)
                {
                    CDataPool.Instance.TalismanEquip_SetItem(packet.EquipItems[i].m_nIndex,ref packet.EquipItems[i].m_nItemData);
                }
				CActionSystem.Instance.UserTalismanBag_Update();
				CActionSystem.Instance.UserTalismanEquip_Update();
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_TALISMANITEM);			  
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_RET_TALISMANBAG;
        }
    }
}