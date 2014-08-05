using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCharSkillMissedHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharSkillMissed Packet = (GCCharSkillMissed)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
			{
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.ReceiverID);
				if ( pObj != null )
				{
					_DAMAGE_INFO infoDamage = new _DAMAGE_INFO();
	                infoDamage.m_nSkillID				= (short)Packet.SkillID;
					infoDamage.m_nTargetID				= (uint)Packet.ReceiverID;
					infoDamage.m_nSenderID				= (uint)Packet.SenderID;
					infoDamage.m_nSenderLogicCount		= Packet.SenderLogicCount;
					infoDamage.m_nType					= _DAMAGE_INFO.DAMAGETYPE.TYPE_SKILL_TEXT;
					infoDamage.m_nImpactID				= (short)Packet.Flag; 				
	
					_LOGIC_EVENT logicEvent = new _LOGIC_EVENT();
					logicEvent.Init((uint)Packet.SenderID, Packet.SenderLogicCount,infoDamage);
	
					SCommand_Object cmdTemp = new SCommand_Object();
					cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_LOGIC_EVENT;
					cmdTemp.SetValue<object>(0,(object)logicEvent);
					pObj.PushCommand(cmdTemp );
	
					//pObj->PushDebugString("GCCharDirectImpact");
					pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
				}
				LogManager.Log("RECV GCCharSkillMissed");
			}
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARSKILL_MISSED;
        }
    }
};
