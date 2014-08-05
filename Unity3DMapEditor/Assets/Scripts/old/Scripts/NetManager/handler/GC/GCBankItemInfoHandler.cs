using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankItemInfoHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankItemInfo packet = pPacket as GCBankItemInfo;
                CDataPool.Instance.UserBank_SetItemExtraInfo(packet.BankIndex, packet.IsNull != 0, packet.Item);

		        CActionSystem.Instance.UserBank_Update();
		        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKITEMINFO;
        }
    }
}