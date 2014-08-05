using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCResetTalismanBagSizeHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.Log("RECV GCResetTalismanBagSize");
                GCResetTalismanBagSize packet = (GCResetTalismanBagSize)pPacket;
                if (packet.Type == 0)
                {
                    LogManager.Log("RECV UnLockCount" + packet.BagSize);
                    CDataPool.Instance.TalismanEquipment_UnLockCount(packet.BagSize);
                }
                else
                {
                    LogManager.Log("RECV UnLockCount" + packet.BagSize);
                    CDataPool.Instance.TalismanInventory_UnLockCount(packet.BagSize);
                }
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_TALISMANITEM);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_RESETTALISMANBAGSIZE;
        }
    }
}