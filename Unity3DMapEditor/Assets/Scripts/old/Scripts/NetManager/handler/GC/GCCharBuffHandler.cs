using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCharBuffHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCCharBuff Packet");
                GCCharBuff charBuffPacket = (GCCharBuff)pPacket;
                CObject pObj = CObjectManager.Instance.FindServerObject((int)charBuffPacket.RecieverID);
                if (pObj == null || !(pObj is CObject_Character))
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_UPDATE_IMPACT;
                cmdTemp.SetValue<short>(0, charBuffPacket.BuffID);
                cmdTemp.SetValue<int>(1, charBuffPacket.Enable);
                cmdTemp.SetValue<int>(2, charBuffPacket.SenderID);
                pObj.PushCommand(cmdTemp);
                //待实现
                //CTeamOrGroup* pTeam = CUIDataPool::GetMe()->GetTeamOrGroup();
                //if ( pTeam != NULL )
                //{
                //    if ( pPacket->GetEnable() )
                //    {
                //        pTeam->AddImpact( pPacket->GetReceiverID(), pPacket->GetBuffID() );
                //    }
                //    else
                //    {
                //        pTeam->RemoveImpact( pPacket->GetReceiverID(), pPacket->GetBuffID() );
                //    }
                //}

                //pObj.PushDebugString("GCCharBuff");
                pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());

			    // 广播更新buff [11/10/2011 Ivan edit]
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_BUFF_UPDATE);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHAR_BUFF;
        }
    }
}