using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCDetailSkillListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCDetailSkillList Packet = (GCDetailSkillList)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
              //  CUIDataPool* pDataPool = (CUIDataPool*)(CGameProcedure::s_pDataPool);
		        //确认是自己的数据
                
		        CObject_PlayerMySelf pMySelf = CObjectManager.Instance.getPlayerMySelf();
		        if(pMySelf.ServerID != Packet.ObjectID) 
		        {
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }

                //清空所有技能
		        pMySelf.GetCharacterData().Skill_CleanAll();
                
		        //刷新到用户数据
		        _OWN_SKILL[] pOwnerSkill = Packet.Skill;
                byte[]   nLevel          = Packet.SkillLevel;
                for(short i=0; i< Packet.numSkill; i++)
		        {
                    //LogManager.LogWarning("GCDetailSkillList " + i + " skillID " + pOwnerSkill[i].m_nSkillID + " level " + nLevel[i]);
                    pMySelf.GetCharacterData().Set_Skill(pOwnerSkill[i].m_nSkillID, nLevel[i], true);
		        }

		        //刷新到UI
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SKILL_UPDATE);
                //LogManager.Log("RECV GCDetailSkillList");
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAILSKILLLIST;
        }
    }
};
