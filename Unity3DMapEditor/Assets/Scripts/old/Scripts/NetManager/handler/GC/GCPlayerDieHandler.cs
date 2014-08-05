using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCPlayerDieHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    GCPlayerDie newPlayerDie = (GCPlayerDie)pPacket;
                    int nTime = newPlayerDie.ReliveTime/1000;

                    //CEventSystem.Instance.PushEvent( GE_RELIVE_SHOW, ((pPacket->IsCanRelive())?("1"):("0")), szText, -1 );
                    List<string> LString = new List<string>();
                    LString.Add(newPlayerDie.IfCanRelive > 0 ? "1" : "0");
                    LString.Add(nTime.ToString());

                    UIWindowMng.Instance.ShowWindow("ReliveWindow");
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_RELIVE_SHOW, LString);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCPlayerDieHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_PLAYER_DIE;
        }
    }
}