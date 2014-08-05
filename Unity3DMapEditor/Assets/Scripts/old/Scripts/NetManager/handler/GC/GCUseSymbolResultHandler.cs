using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCUseSymbolResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                int res = ((GCUseSymbolResult)pPacket).Result;
                if (res == 0)
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该类型符印使用数量已超过上限");
                }
               /* else//使用成功
                {
                    //向服务器请求符印信息
                    CGAskFlushCharmInfo msgFlushCharmInfo = new CGAskFlushCharmInfo();
                    NetManager.GetNetManager().SendPacket(msgFlushCharmInfo);
                }*/
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USECHARMRESULT;
        }
    }
}