using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCMissionRemoveHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    GCMissionRemove newMissionRemove = (GCMissionRemove)pPacket;
                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_REMOVE;

                    cmdTemp.SetValue<int>(0, newMissionRemove.MissionID);
                    CUIDataPool.Instance.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCMissionRemoveHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MISSIONREMOVE;
        }
    }
}