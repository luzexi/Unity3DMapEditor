using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCObjTeleportHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCObjTeleport Packet");
                GCObjTeleport Packet = (GCObjTeleport)pPacket;
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.ObjID);
                if (pObj == null || !(pObj is CObject_Character))
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
               
                //if(!GFX.GfxSystem.Instance.IsValidPosition(Packet.PosWorld.m_fX, Packet.PosWorld.m_fZ))
		       // {
			   //     LogManager.LogError("ERROR POSITION @ GCCharIdleHandler");
		       // }

		        SCommand_Object cmdTemp = new SCommand_Object();
		        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_TELEPORT;
		        cmdTemp.SetValue<float>(0,Packet.PosWorld.m_fX);
		        cmdTemp.SetValue<float>(1,Packet.PosWorld.m_fZ);
		        pObj.PushCommand(cmdTemp );

		        // 瞬移时，清空寻路节点 [9/2/2011 Sun]
		        //CWorldManager::GetMe()->SetPathNodesDirty();

                pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_OBJ_TELEPORT;
        }
    }
}