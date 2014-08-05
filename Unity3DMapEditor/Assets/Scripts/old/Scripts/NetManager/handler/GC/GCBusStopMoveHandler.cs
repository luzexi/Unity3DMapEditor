using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCBusStopMoveHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
		        //检查位置是否合法
                //const WORLD_POS* pTargetPos = pPacket->GetCurPos();
                //if(!CWorldManager::GetMe()->GetActiveScene()->IsValidPosition(fVector2(pTargetPos->m_fX, pTargetPos->m_fZ)))
                //{
                //    TDThrow("ERROR POSITION @ GCBusStopMoveHandler target");
                //}
                
                GCBusStopMove busPacket = pPacket as GCBusStopMove;
                if (busPacket == null)
                {
                    LogManager.LogError("GCBusStopMove 收包错误。");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
                }
		        CObject pBus = CObjectManager.Instance.FindServerObject((int)busPacket.ObjID);
                if (pBus == null)
		        {
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }
		        SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_BUS_STOP_MOVE;
                cmdTemp.SetValue(0, busPacket.PosWorld.m_fX);
                cmdTemp.SetValue(1, -1.0f);
                cmdTemp.SetValue(2, busPacket.PosWorld.m_fZ);
                pBus.PushCommand(cmdTemp);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_BUSSTOPMOVE;
        }
    }
}
