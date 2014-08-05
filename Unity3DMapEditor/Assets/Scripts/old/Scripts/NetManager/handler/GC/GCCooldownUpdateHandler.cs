using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCooldownUpdateHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCooldownUpdate Packet = (GCCooldownUpdate)pPacket;
            //LogManager.Log("RECV GCCooldownUpdate");
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                if (Packet.GuidPet.IsNull())
		        {	//人物冷却信息
                    //LogManager.Log("RECV GCCooldownUpdate GuidPet.IsNull");
			        CDataPool.Instance.CoolDownGroup_UpdateList(Packet.CoolDowns, Packet.numCoolDown);

                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_CDTIME);
		        }
		        else
		        {
			        //宠物冷却信息
                    CDataPool.Instance.PetSkillCoolDownGroup_UpdateList(Packet.CoolDowns, Packet.numCoolDown, Packet.GuidPet);
		        }
                //LogManager.Log("RECV GCCooldownUpdate");
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_COOLDOWN_UPDATE;
        }
    }
};
