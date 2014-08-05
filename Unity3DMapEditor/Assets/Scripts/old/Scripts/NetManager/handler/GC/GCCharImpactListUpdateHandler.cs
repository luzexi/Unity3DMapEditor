using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCharImpactListUpdateHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharImpactListUpdate Packet = (GCCharImpactListUpdate)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.OwnerID);
		        if ( pObj != null )
                {
		            ////如果需要更新资源，尝试放入缓冲队列
		            //if(pObjectManager->GetLoadQueue()->TryAddLoadTask(pObj->GetID(), pPacket))
		            //	return PACKET_EXE_NOTREMOVE;

                    CObject_Character  pChar = (CObject_Character)pObj;
                    if(pChar != null)
                    {
                        pChar.RemoveAllImpact();
                    }
                    SimpleImpactList ImpactList = new SimpleImpactList();
                    
                    for ( short i = 0; i < Packet.NumImpact; i++ )
                    {
                        SCommand_Object cmdTemp = new SCommand_Object();
                        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_UPDATE_IMPACT_EX;
                        cmdTemp.SetValue<short>(0,Packet.ImpactID[i]);
                        pObj.PushCommand(cmdTemp);
                        ImpactList.AddImpact(Packet.ImpactID[i]);
                    }
                    //待实现
                    //CTeamOrGroup* pTeam = CUIDataPool::GetMe()->GetTeamOrGroup();
                    //if ( pTeam != NULL )
                    //{
                    //    pTeam->UpdateImpactsList( pPacket->GetOwnerID(), &ImpactList );
                    //}

                    //pObj->PushDebugString("GCCharImpactListUpdate");
                    pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
                }
                LogManager.Log("RECV GCCharImpactListUpdate");
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHAR_IMPACT_LIST_UPDATE;
        }
    }
};
