using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCAbilityResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                CObjectManager pObjectManager = CObjectManager.Instance;
                GCAbilityResult packet = pPacket as GCAbilityResult;
                //		//失败！
                if (packet.Result != (int)OPERATE_RESULT.OR_OK)
                {

                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, GameDefineResult.Instance.GetOResultText((OPERATE_RESULT)packet.Result));
                    if (packet.Result == (int)OPERATE_RESULT.OR_FAILURE)
                    {
                        //switch(packet.AbilityID)
                        //{
                        //case 3:
                        //    CSoundSystemFMod::_PlayUISoundFunc(22+59);
                        //    break;
                        //default:
                        //    break;				
                        //}

                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CLOSE_SYNTHESIZE_ENCHASE);
                    }
                    // 结束采集操作 [4/18/2012 Ivan]
                    {
                        CAI_MySelf pMySelfAI = (CAI_MySelf)CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
                        pMySelfAI.FinishTripperActive();
                    }

                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ABILITY_RESULT;
        }
    }
}