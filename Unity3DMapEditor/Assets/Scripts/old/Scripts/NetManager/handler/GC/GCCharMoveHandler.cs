using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCharMoveHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                //LogManager.Log("Receive GCCharMove Packet");
                GCCharMove charMovePacket = (GCCharMove)pPacket;
                CObjectManager pObjectManager = CObjectManager.Instance;

                CObject pObj = (CObject)(pObjectManager.FindServerObject((int)charMovePacket.ObjID));
                if (pObj == null || !(pObj is CObject_Character))
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                SCommand_Object cmdTemp = new SCommand_Object();
                if (charMovePacket.IsHaveStopInfo())
                {
                   
                    cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_STOP_MOVE;
                    cmdTemp.SetValue(0, charMovePacket.StopLogicCount);
                    cmdTemp.SetValue(1, 0);
                    cmdTemp.SetValue(2, pObj.GetPosition().x);
                    cmdTemp.SetValue(3, pObj.GetPosition().z);
                    pObj.PushCommand(cmdTemp);
                }
                WORLD_POS[] posTarget = new WORLD_POS[1];
                posTarget[0].m_fX = charMovePacket.PosTarget.m_fX;
                posTarget[0].m_fZ = charMovePacket.PosTarget.m_fZ;

                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_MOVE;
                cmdTemp.SetValue(0, charMovePacket.StartTime);
                cmdTemp.SetValue(1, charMovePacket.HandleID);
                cmdTemp.SetValue(2, 1);
                cmdTemp.SetValue(3, posTarget);
                pObj.PushCommand(cmdTemp);
                pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARMOVE;
        }
    }
}