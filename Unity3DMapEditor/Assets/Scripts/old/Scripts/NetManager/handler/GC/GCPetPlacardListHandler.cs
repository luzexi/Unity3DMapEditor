using System;
using System.Collections.Generic;
using Network.Packets;
using Interface;
namespace Network.Handlers
{
    public class GCPetPlacardListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCPetPlacardList Packet");
                GCPetPlacardList packet = (GCPetPlacardList)pPacket;
                CDataPool.Instance.PetPlacard_CleanUp();

		        int nItemCount;
                nItemCount = packet.Count;
		        for(int i = 0; i < nItemCount; i++)
		        {
			        _PET_PLACARD_ITEM pItem = packet.Item[i];
			        CDataPool.Instance.PetPlacard_AddItem(pItem);
		        }

		        if(nItemCount > 0)
		        {
                    PetInviteFriend.Instance.ConvertPlaceCard2PetFriend();
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PETINVITEFRIEND, "invite");
		        }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_PETPLACARDLIST;
        }
    }
}