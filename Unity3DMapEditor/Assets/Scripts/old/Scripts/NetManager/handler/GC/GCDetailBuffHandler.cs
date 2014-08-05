using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCDetailBuffHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCDetailBuff Packet");
                GCDetailBuff detailBuffPacket = (GCDetailBuff)pPacket;
                if(detailBuffPacket.Enable != 0)
                {
                    _BUFF_IMPACT_INFO infoBuffImpact    = new _BUFF_IMPACT_INFO();
                    infoBuffImpact.m_nReceiverID        = (uint)detailBuffPacket.RecieverID;
                    infoBuffImpact.m_nSenderID          = (uint)detailBuffPacket.SenderID;
                    infoBuffImpact.m_nBuffID            = detailBuffPacket.BuffID;
                    infoBuffImpact.m_nSkillID           = detailBuffPacket.SkillID;
                    infoBuffImpact.m_nSenderLogicCount  = detailBuffPacket.SenderLogicCount;
                    infoBuffImpact.m_nSN                = detailBuffPacket.SN;
                    infoBuffImpact.m_nTimer             = detailBuffPacket.Continuance;
                    // 接口未实现
                    CDataPool.Instance.BuffImpact_Add(infoBuffImpact);
                }
                else
                {
                    // 接口未实现
                    CDataPool.Instance.BuffImpact_Remove((int)detailBuffPacket.SN);
                }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAIL_BUFF;
        }
    }
}