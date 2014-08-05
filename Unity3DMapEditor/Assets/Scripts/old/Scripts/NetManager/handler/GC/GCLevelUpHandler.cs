using System;
using System.Collections.Generic;
using Network.Packets;

namespace Network.Handlers
{
    public class GCLevelUpHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                //LogManager.Log("Receive GCLevelUpResult Packet");
                GCLevelUp levelUp = (GCLevelUp)pPacket;

   
			CObject pObj = CObjectManager.Instance.FindServerObject((int)levelUp.ObjectID);//(CObject*)(pObjectManager->FindServerObject( (INT)pPacket->getObjId() ));
			if ( pObj == null )
				return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

            //pObj->PushDebugString("LevelUp!");
            //pObj->SetMsgTime(CGameProcedure::s_pTimeSystem->GetTimeNow());

            //if(pObj ==(CObject*) CObjectManager::GetMe()->GetMySelf())
            //{
            //    CGameProcedure::s_pGfxSystem->PushDebugString("LeveUP");
            //    CSoundSystemFMod::_PlayUISoundFunc(66);
            //}

			    SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_LEVEL_UP;
			    pObj.PushCommand(cmdTemp );
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_LEVELUP;
        }
    }
}