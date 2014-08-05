using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;
namespace Network.Packets
{
    public class GCBusRemovePassengerHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCBusRemovePassenger busPacket = pPacket as GCBusRemovePassenger;
                if (busPacket == null)
                {
                    LogManager.LogError("GCBusRemovePassenger 收包错误。");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
                }
                CObject pBus = CObjectManager.Instance.FindServerObject((int)busPacket.ObjID);
                if (pBus == null)
                {
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_BUS_REMOVE_PASSENGER;
                cmdTemp.SetValue(0, busPacket.PassengerID);
                pBus.PushCommand(cmdTemp);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BUSREMOVEPASSENGER;
        }
    }
}
