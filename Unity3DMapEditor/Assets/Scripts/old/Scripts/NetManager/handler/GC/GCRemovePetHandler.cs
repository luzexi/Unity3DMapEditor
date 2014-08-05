using System;
using System.Collections.Generic;
using Network.Packets;

namespace Network.Handlers
{
    public class GCRemovePetHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCRemovePet Packet");
                GCRemovePet packet = (GCRemovePet)pPacket;
                PET_GUID_t guidPet = packet.GUID;
		        for(int i = 0; i < GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++ )
		        {
			        SDATA_PET pPetData = CDataPool.Instance.Pet_GetPet(i);
			        if (pPetData != null && pPetData.GUID == guidPet)
			        {
				        pPetData.CleanUp();
				        CActionSystem.Instance.UpdateToolBarForPetSkill();
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PET_PAGE);
				        break;
			        }
		        }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_REMOVEPET;
        }
    }
}