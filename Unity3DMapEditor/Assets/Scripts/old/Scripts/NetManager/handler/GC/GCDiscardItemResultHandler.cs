
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCEnterSceneHandler 空壳
    /// </summary>
    public class GCDiscardItemResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if(GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
	        {
                GCDiscardItemResult packet = pPacket as GCDiscardItemResult;
		        DISCARDITEM_RESULT ret = (DISCARDITEM_RESULT)packet.Result;
		        switch(ret)
		        {
		        case DISCARDITEM_RESULT.DISCARDITEM_SUCCESS:
			        {
				        if(packet.Operate == (byte)GCDiscardItemResult.Operator.FromBag)
				        {
					        CDataPool.Instance.UserBag_SetItemInfo(packet.BagIndex, true, null);
				        }
				        else if(packet.Operate == (byte)GCDiscardItemResult.Operator.FromBank)
				        {
					        CDataPool.Instance.UserBank_SetItem(packet.BagIndex,  null, true);
					        //刷新ActionSystem
					        //...
					        CActionSystem.Instance.UserBank_Update();
					        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
				        }
			        }
			        break;

		        case DISCARDITEM_RESULT.DISCARDITEM_FAIL:
		        default:
			        {
				        LogManager.LogWarning("Discard item failed!");
			        }
			        break;
		        }

	        }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DISCARDITEMRESULT;
        }
    }
}
