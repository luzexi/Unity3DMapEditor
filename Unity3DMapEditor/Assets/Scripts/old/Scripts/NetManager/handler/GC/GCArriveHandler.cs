using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCArriveHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.Log("RECV GCArrive");
                GCArrive Packet = (GCArrive)pPacket;
                CObject pObj = (CObject)(CObjectManager.Instance.FindServerObject(Packet.ObjectID));
		        if ( pObj == null )
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

		        SCommand_Object cmdTemp = new SCommand_Object();
		        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_STOP_MOVE;
                cmdTemp.SetValue<int>(0,Packet.HandleID);
		        cmdTemp.SetValue<int>(1,0);
		        cmdTemp.SetValue<float>(2,Packet.PosWorld.m_fX);
		        cmdTemp.SetValue<float>(3,Packet.PosWorld.m_fZ);
		        pObj.PushCommand(cmdTemp);
		        pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
           
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ARRIVE;
        }
    }
}