using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCMissionListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                GCMissionList newGCMissionList = (GCMissionList)pPacket;
                CUIDataPool pUIDataPool = GameProcedure.s_pUIDataPool;

                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    // 保存任务是否完成标志
                    if (CDetailAttrib_Player.Instance != null)
                    {
                        CDetailAttrib_Player.Instance.SetMissionHaveDone(newGCMissionList.GetMissionHaveDone());
                    }

                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_UPDATE_MISSION_LIST;

                    cmdTemp.SetValue<uint>(0, newGCMissionList.ObjID);
                    cmdTemp.SetValue<uint>(1, newGCMissionList.MissionListFlags);
                    cmdTemp.SetValue<_OWN_MISSION[]>(2, newGCMissionList.GetMissionBuf());
                    pUIDataPool.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCMissionListHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MISSIONLIST;
        }
    }
}