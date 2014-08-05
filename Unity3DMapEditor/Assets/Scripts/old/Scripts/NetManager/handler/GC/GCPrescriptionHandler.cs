using System;
using System.Collections.Generic;
using Network.Packets;


namespace Network.Handlers
{
    public class GCPrescriptionHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCPrescription Packet = pPacket as GCPrescription;
                //加入UI数据池
                //CUIDataPool* pDataPool = (CUIDataPool*)(CGameProcedure::s_pDataPool);

                //SCommand_DPC cmdTemp;

                //--------------------------------------------------------------
                //生活技能配方表刷新
                /*
                        cmdTemp.m_wID			= DPC_UPDATE_LIFE_PRESCR;
                        cmdTemp.m_adwParam[0]	= 2; // update a specified recipe
                        cmdTemp.m_adwParam[1]	= pPacket->getPrescription();
                        cmdTemp.m_abParam[2]	= pPacket->getLearnOrAbandon();
                        pDataPool->OnCommand_( &cmdTemp );
                */
                CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_Prescr(
                    (int)Packet.Prescription, Packet.LearnOrAbandom > 0);

                GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_UPDATE_PRESCR);

            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_PRESCRIPTION;
        }
    }
}
