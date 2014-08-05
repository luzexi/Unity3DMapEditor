using System;
using System.Collections.Generic;
using Network.Packets;
namespace Network.Handlers
{
    public class GCDetailImpactListUpdateHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCDetailImpactListUpdate Packet");
                GCDetailImpactListUpdate Packet = (GCDetailImpactListUpdate)pPacket;
                CDataPool.Instance.BuffImpact_RemoveAll();
                DetailImpactStruct_T[] pImpactList = Packet.Impacts;
                for (short i = 0; i < Packet.NumImpact; i++)
                {
                    _BUFF_IMPACT_INFO infoBuffImpact = new _BUFF_IMPACT_INFO();
                    infoBuffImpact.m_nReceiverID = (uint)Packet.OwnerID;
                    infoBuffImpact.m_nSenderID   = (uint)pImpactList[i].m_nSenderID;
                    infoBuffImpact.m_nBuffID     = pImpactList[i].m_nBuffID;
                    infoBuffImpact.m_nSkillID    = pImpactList[i].m_nSkillID;
                    infoBuffImpact.m_nSenderLogicCount = -1;
                    infoBuffImpact.m_nSN    = pImpactList[i].m_nSN;
                    infoBuffImpact.m_nTimer = pImpactList[i].m_nContinuance;

                    CDataPool.Instance.BuffImpact_Add(infoBuffImpact);
                }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAIL_IMPACT_LIST_UPDATE;
        }
    }
}