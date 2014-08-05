using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCMissionHaveDoneFlagHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    GCMissionHaveDoneFlag newMissionHaveDoneFlag = (GCMissionHaveDoneFlag)pPacket;

                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_HAVEDOWN_FALG;

                    cmdTemp.SetValue<int>(0, newMissionHaveDoneFlag.MissionID);
                    cmdTemp.SetValue<int>(1, newMissionHaveDoneFlag.IsHaveDone);
                    CUIDataPool.Instance.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCMissionHaveDoneFlagHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MISSIONHAVEDONEFLAG;
        }
    }
}