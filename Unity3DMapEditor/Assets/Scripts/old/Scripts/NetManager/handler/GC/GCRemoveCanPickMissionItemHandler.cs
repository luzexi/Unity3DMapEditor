using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCRemoveCanPickMissionItemHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    //加入UI数据池
                    CUIDataPool pUIDataPool = GameProcedure.s_pUIDataPool;
                    GCRemoveCanPickMissionItem newRemoveCanPickMissionItem = (GCRemoveCanPickMissionItem)pPacket;

                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_REMOVE_CAN_PICK_MISSION_ITEM;
                    cmdTemp.SetValue<uint>(0, newRemoveCanPickMissionItem.ItemDataID);

                    pUIDataPool.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCRemoveCanPickMissionItemHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_REMOVECANPICKMISSIONITEM;
        }
    }
}