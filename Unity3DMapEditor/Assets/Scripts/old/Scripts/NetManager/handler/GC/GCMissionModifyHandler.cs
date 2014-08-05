using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCMissionModifyHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    CUIDataPool pUIDataPool = GameProcedure.s_pUIDataPool;
                    GCMissionModify newMissionModifyPacket = (GCMissionModify)pPacket;

                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_MODIFY;

                    cmdTemp.SetValue<int>(1, newMissionModifyPacket.Flag);

                    if (newMissionModifyPacket.Flag == (int)GCMissionModify.MISSIONMODIFY.MISSIONMODIFY_MISSION)
                    {
                        cmdTemp.SetValue<_OWN_MISSION>(0, newMissionModifyPacket.GetMission());
                    }
                    else if (newMissionModifyPacket.Flag == (int)GCMissionModify.MISSIONMODIFY.MISSIONMODIFY_MISSIONDATA)
                    {
                        cmdTemp.SetValue<int[]>(0, newMissionModifyPacket.GetMissionData());
                    }
                    pUIDataPool.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCMissionModifyHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MISSIONMODIFY;
        }
    }
}