using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCUseItemResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                USEITEM_RESULT res = ((GCUseItemResult)pPacket).Result;
                switch (res)
                {
                    case USEITEM_RESULT.USEITEM_SUCCESS:
                        break;
                    case USEITEM_RESULT.USEITEM_CANNT_USE:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "此物品不可以使用");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_LEVEL_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "等级不够");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_TYPE_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "物品类型错误");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_TARGET_TYPE_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "目标类型错误");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_SKILL_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "物品附加技能错误");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_IDENT_TYPE_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "不能鉴定这种物品");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_IDENT_TARGET_TYPE_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "不能鉴定这种物品");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_IDENT_LEVEL_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "鉴定卷轴等级不够");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_CANNT_USE_INBUS:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "此时无法使用物品");
                        }
                        break;
                    case USEITEM_RESULT.USEITEM_INVALID:
                        break;
                    default:
                        break;
                }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USEITEMRESULT;
        }
    }
}