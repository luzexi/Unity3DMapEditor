using System;
using System.Collections.Generic;

using Network.Packets;
namespace Network.Handlers
{
    public class GCStudyXinfaHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                //刷新心法升级后的客户端数据（心法等级、当前金钱、当前经验值）
                GCStudyXinfa packet = pPacket as GCStudyXinfa;
		        CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();
                GCStudyXinfa._STUDERESULT_INFO studyResult = packet.StudyResult;
		        pCharData.Set_SkillClass( studyResult.m_idXinfa, studyResult.m_StudedLevel );

		        // 刷新所有技能 [10/18/2011 Ivan edit]
		        pCharData.UpdateAllSkillClassState();
		       
		        string Action = string.Format("{0};0", (int)(ENUM_BASE_ACTION.BASE_ACTION_XINFA_LEVEL_UP));
		        CObjectManager.Instance.getPlayerMySelf().SetChatMoodAction(Action);

		        //发送更新界面的事件
		        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SKILLSTUDY_SUCCEED);
		        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SKILL_UPDATE);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_STUDYXINFA_H;
        }
    }
};
