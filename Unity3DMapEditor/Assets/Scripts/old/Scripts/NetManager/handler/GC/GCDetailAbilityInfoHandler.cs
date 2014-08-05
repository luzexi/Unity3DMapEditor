using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCDetailAbilityInfoHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            //LogManager.LogWarning("Receive GCDetailAbilityInfo");
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCDetailAbilityInfo Packet = (GCDetailAbilityInfo)pPacket;

                for (int i = 0; i < Packet.NumAbility; i++)
                {
                    CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_LifeAbility(
                        Packet.AbilityIDList[i],
                        Packet.Ability[i].m_Level,
                        Packet.Ability[i].m_Exp);
                }

                for (int i = 0; i < GAMEDEFINE.MAX_ABILITY_PRESCRIPTION_NUM; i++)
                {
                    bool bCando = (Packet.PrescrList[i >> 3] & (1 << (i % 8))) != 0;
                    if (!bCando) continue;

                    //配方id从“1”开始
                    CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_Prescr(i, true);
                }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAILABILITYINFO;
        }
    }
};
