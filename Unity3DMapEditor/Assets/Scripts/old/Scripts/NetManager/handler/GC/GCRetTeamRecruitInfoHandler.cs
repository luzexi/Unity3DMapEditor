using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCRetTeamRecruitInfoHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.LogWarning("RECV GCRetTeamRecruitInfo");
                GCRetTeamRecruitInfo packet = pPacket as GCRetTeamRecruitInfo;

                // 每次发来的都是当前最新的列表，因此清空原有的 [3/23/2012 SUN]
                CDataPool.Instance.Campaign_Clear();
                byte teamCount = packet.TeamCount;
                for (byte i = 0; i < teamCount; i++ )
                {

                    CDataPool.Instance.Campaign_AddTeamInfo(packet.RecruitInfo[i]);
                }
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_CAMPAIGN_TEAMINFO);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_RET_RECRUITINFO;
        }
    }
}