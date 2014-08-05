using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCharDirectImpactHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharDirectImpact Packet = (GCCharDirectImpact)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {               
			    CObject pObj = CObjectManager.Instance.FindServerObject(Packet.RecieverID);
			    if ( pObj != null )
			    {
				    _DAMAGE_INFO infoDamage = new _DAMAGE_INFO();
                    infoDamage.m_nSkillID				= Packet.SkillID;
				    infoDamage.m_nTargetID				= (uint)Packet.RecieverID;
				    infoDamage.m_nSenderID				= (uint)Packet.SenderID;
				    infoDamage.m_nSenderLogicCount		= Packet.SenderLogicCount;
				    infoDamage.m_nImpactID				= Packet.ImpactID;
				    infoDamage.m_nType					= _DAMAGE_INFO.DAMAGETYPE.TYPE_EFFECT;

				    _LOGIC_EVENT logicEvent = new _LOGIC_EVENT();
				    logicEvent.Init(infoDamage.m_nSenderID, infoDamage.m_nSenderLogicCount,infoDamage);

				    SCommand_Object cmdTemp = new SCommand_Object();
				    cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_LOGIC_EVENT;
                    cmdTemp.SetValue<object>(0,logicEvent);
				    pObj.PushCommand(cmdTemp );
				    pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
			    }
                LogManager.Log("RECV GCCharDirectImpact");
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHAR_DIRECT_IMPACT;
        }
    }
};
