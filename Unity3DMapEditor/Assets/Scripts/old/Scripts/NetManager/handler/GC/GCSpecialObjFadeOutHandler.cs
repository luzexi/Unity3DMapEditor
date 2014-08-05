using System;
using System.Collections.Generic;
using Network.Packets;
namespace Network.Handlers
{
    public class GCSpecialObjFadeOutHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCSpecialObjFadeOut Packet");
                GCSpecialObjFadeOut Packet = (GCSpecialObjFadeOut)pPacket;    
		        CObject pObj = CObjectManager.Instance.FindServerObject( Packet.ObjID);
		        if ( pObj == null )
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        SCommand_Object cmdTemp = new SCommand_Object();
		        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_SPECIAL_OBJ_DIE;
		        pObj.PushCommand(cmdTemp);
		        pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SPECIAL_OBJ_FADE_OUT;
        }
    }
}