using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCPlayerReliveHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_RELIVE_HIDE);
                    CUIDataPool.Instance.EndOutGhostTimer();
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCPlayerReliveHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_PLAYER_RELIVE;
        }
    }
}