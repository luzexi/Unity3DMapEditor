using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
	public class GCCharSkillSendHandler : HandlerBase
    {
      	public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharSkillSend Packet = (GCCharSkillSend)pPacket;

            if(GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
			{
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.ObjectID);
				if ( pObj != null )
				{
					SCommand_Object cmdTemp = new SCommand_Object();
					cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_MAGIC_SEND;
					cmdTemp.SetValue<uint>(0,0);
					cmdTemp.SetValue<int>(1,Packet.nLogicCount);
					cmdTemp.SetValue<short>(2,Packet.SkillDataID);
					cmdTemp.SetValue<uint>(3,(uint)Packet.TargetID);
					cmdTemp.SetValue<float>(4,Packet.posTarget.m_fX);
					cmdTemp.SetValue<float>(5,Packet.posTarget.m_fZ);
					cmdTemp.SetValue<float>(6,Packet.Dir);
					pObj.PushCommand(cmdTemp );
					//pObj->PushDebugString("GCCharSkill_Send");
					pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
				}
                LogManager.LogWarning("RECV GCCharSkillSend " + Packet.SkillDataID);
			}
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARSKILL_SEND;
        }
	}
};
