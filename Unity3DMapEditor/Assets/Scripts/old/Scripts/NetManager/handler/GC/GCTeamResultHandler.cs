using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCTeamResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCTeamResult packet = pPacket as GCTeamResult;
                CUIDataPool pDataPool = CUIDataPool.Instance;

		        SCommand_DPC cmdTemp = new SCommand_DPC();
		        cmdTemp.m_wID			= DPC_SCRIPT_DEFINE.DPC_UPDATE_TEAM_OR_GROUP;

		        cmdTemp.SetValue<byte> (0, packet.Return);
		        cmdTemp.SetValue<uint> (1, packet.GUID);
		        cmdTemp.SetValue<short>(2, packet.TeamId);
		        cmdTemp.SetValue<uint> (3, packet.GUIDEx);
		        cmdTemp.SetValue<short>(4, packet.SceneId);
		        cmdTemp.SetValue<int>  (5, packet.Portrait);
		        cmdTemp.SetValue<short>(6, packet.DataID);
                cmdTemp.SetValue<byte> (7, packet.AllocRuler);
                cmdTemp.SetValue<byte[]>(8, packet.Name);
		        pDataPool.OnCommand_( cmdTemp );
		        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TEAM_CHANG_WORLD);

            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_TEAMRESULT;
        }
    }
}