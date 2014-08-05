using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCharJumpHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.Log("RECV GCCharJump");
                GCCharJump Packet = (GCCharJump)pPacket;
                CObject pObj = (CObject)(CObjectManager.Instance.FindServerObject(Packet.ObjectID));
                if (pObj == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_JUMP;
                pObj.PushCommand(cmdTemp);
                pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARJUMP;
        }
    }
}