using System;
using System.Collections.Generic;
using Network.Packets;

namespace Network.Handlers
{
    public class GCLeaveSceneHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCLeaveScene leaveScene = (GCLeaveScene)pPacket;

                CObject pObj = CObjectManager.Instance.FindServerObject((int)leaveScene.ObjectID);//(CObject*)(pObjectManager->FindServerObject( (INT)pPacket->getObjId() ));
                if (pObj == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

		        GameProcedure.s_pObjectManager.DestroyObject(pObj);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_LEAVESCENE;
        }
    }
}