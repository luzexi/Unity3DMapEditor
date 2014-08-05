using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCharSkillLeadHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharSkillLead Packet = (GCCharSkillLead)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
			{
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.ObjectID);
				if ( pObj == null )
					return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		
				SCommand_Object cmdTemp = new SCommand_Object();
				cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_MAGIC_CHANNEL;
				cmdTemp.SetValue<int>(0,0);
				cmdTemp.SetValue<int>(1,Packet.LogicCount);
				cmdTemp.SetValue<short>(2,Packet.SkillDataID);
				cmdTemp.SetValue<int>(3,Packet.TargetID);
				cmdTemp.SetValue<float>(4,Packet.PosTarget.m_fX);
				cmdTemp.SetValue<float>(5,Packet.PosTarget.m_fZ);
				cmdTemp.SetValue<float>(6,Packet.Dir);
				
				cmdTemp.SetValue<int>(7,Packet.TotalTime);
				pObj.PushCommand(cmdTemp );
		
				//pObj->PushDebugString("GCCharSkill_Lead");
				pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
				LogManager.Log("RECV GCCharSkillLead");
			}
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARSKILL_LEAD;
        }
    }
};
