using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCDetailXinFaClassHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCSkillClass Packet = (GCSkillClass)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                //CUIDataPool* pDataPool = (CUIDataPool*)(CGameProcedure::s_pDataPool);

		        //确认是自己的数据
                CObject_PlayerMySelf pMySelf = CObjectManager.Instance.getPlayerMySelf();
		        if(pMySelf.ServerID != Packet.ObjectID) 
		        {
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }

		        //刷新到用户数据
                _OWN_XINFA[]  xinFa = Packet.XinFa;
		        for(short i=0; i< Packet.numXinFa; i++)
		        {
			        pMySelf.GetCharacterData().Set_SkillClass(xinFa[i].m_nXinFaID,xinFa[i].m_nXinFaLevel);
		        }
        		
		        CActionSystem.Instance.SkillClass_Update();

                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SKILL_UPDATE);
                LogManager.Log("RECV GCDetailXinFaClass");
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAILXINFALIST;
        }
    }
};
