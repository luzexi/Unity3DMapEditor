using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCMissionResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                CUIDataPool pUIDataPool = GameProcedure.s_pUIDataPool;
                GCMissionResult newMissionResult = (GCMissionResult)pPacket;
                if (newMissionResult.IsFinished > 0) //任务完成
                {
                    // 关闭提交任务物品的界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE);

                    // 打开接收奖励的界面

                }
                else //没有完成任务
                {
                    // 答应一个提示信息，不关闭提交任务物品的界面，这样可以让玩家检查是不是自己放错了物品
                    string strTemp = "你给我的物品不是我需要的!";
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, strTemp);
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MISSIONRESULT;
        }
    }
}