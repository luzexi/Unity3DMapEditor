using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCBusMoveHandler : HandlerBase
    {

        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
		        //检查位置是否合法
                //const WORLD_POS* pTargetPos = pPacket->GetTargetPos();
                //if(!CWorldManager::GetMe()->GetActiveScene()->IsValidPosition(fVector2(pTargetPos->m_fX, pTargetPos->m_fZ)))
                //{
                //    TDThrow("ERROR POSITION @ GCBusMoveHandler target");
                //}
                
                GCBusMove busPacket = pPacket as GCBusMove;
                if (busPacket == null)
                {
                    LogManager.LogError("GCBusMove 收包错误。");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
                }
		        CObject pBus = CObjectManager.Instance.FindServerObject((int)busPacket.ObjID);
                if (pBus == null)
		        {
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }

		        SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_BUS_MOVE;
                cmdTemp.SetValue(0, busPacket.PosTarget.m_fX);
                cmdTemp.SetValue(1, busPacket.TargetHeight);
                cmdTemp.SetValue(2, busPacket.PosTarget.m_fZ);
                pBus.PushCommand(cmdTemp);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BUSMOVE;
        }
    }
}
