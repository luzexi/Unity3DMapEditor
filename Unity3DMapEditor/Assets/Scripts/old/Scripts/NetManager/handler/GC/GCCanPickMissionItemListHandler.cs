using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCanPickMissionItemListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    //加入UI数据池
                    CUIDataPool pUIDataPool = GameProcedure.s_pUIDataPool;
                    GCCanPickMissionItemList newCanPickMissionItemList = (GCCanPickMissionItemList)pPacket;

                    SCommand_DPC cmdTemp = new SCommand_DPC();
                    cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_UPDATE_CAN_PICK_MISSION_ITEM_LIST;
                    cmdTemp.SetValue<byte>(0, newCanPickMissionItemList.ItemCount);
                    cmdTemp.SetValue<uint[]>(1, newCanPickMissionItemList.CanPickMissionItemList);

                    pUIDataPool.OnCommand_(cmdTemp);
                }

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCCanPickMissionItemListHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CANPICKMISSIONITEMLIST;
        }
    }
}