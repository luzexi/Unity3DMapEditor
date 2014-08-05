using System;
using System.Collections.Generic;
using Network.Packets;

namespace Network.Handlers
{
    public class GCManipulatePetRetHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                //LogManager.Log("Receive GCManipulatePetRet Packet");
                GCManipulatePetRet packet = pPacket as GCManipulatePetRet;


                switch ((GCManipulatePetRet.ENUM_MANIPULATEPET_RET)packet.Result)
                {
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_CAPTUREFALID:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_ERROR, "捕捉失败");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_CAPTURESUCC:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "捕捉成功");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_CALLUPFALID:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_ERROR, "召唤失败");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_CALLUPSUCC:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "召唤成功");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_RECALLFALID:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_ERROR, "收回宠物失败");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_RECALLSUCC:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "收回宠物成功");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_FREEFALID:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_ERROR, "放生宠物失败");
                        }
                        break;
                    case GCManipulatePetRet.ENUM_MANIPULATEPET_RET.MANIPULATEPET_RET_FREESUCC:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "放生宠物成功");
                        }
                        break;
                    default:
                        break;
                }

                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PET_PAGE);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MANIPULATEPETRET;
        }
    }
}