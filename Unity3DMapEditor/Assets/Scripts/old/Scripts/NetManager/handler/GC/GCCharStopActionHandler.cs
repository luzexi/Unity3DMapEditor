using System;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCharStopActionHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCCharStopAction Packet = (GCCharStopAction)pPacket;
                CObjectManager pObjectManager = CObjectManager.Instance;

                CObject pObj = (CObject)(pObjectManager.FindServerObject((int)Packet.ObjectID));
                if (pObj == null || !(pObj is CObject_Character))
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                SCommand_Object cmdTemp = new SCommand_Object();

                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_STOP_ACTION;
                cmdTemp.SetValue(0, Packet.LogicCount);
                cmdTemp.SetValue(1, Packet.StopTime);

                pObj.PushCommand(cmdTemp);
                pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARSTOPACTION;
        }
    }
}