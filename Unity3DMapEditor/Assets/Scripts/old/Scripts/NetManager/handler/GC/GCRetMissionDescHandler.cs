using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCRetMissionDescHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    CUIDataPool pUIDataPool = GameProcedure.s_pUIDataPool;
                    GCRetMissionDesc newMissionDesc = (GCRetMissionDesc)pPacket;


                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_DESC;

                    cmdTemp.SetValue<GCRetMissionDesc>(0, newMissionDesc);
                    pUIDataPool.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCRetMissionDescHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MISSIONDESC;
        }
    }
}