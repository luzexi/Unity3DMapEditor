using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCRemoveGemResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCRemoveGemResult packet = pPacket as GCRemoveGemResult;

                switch (packet.Result)
                {
                    case REMOVEGEM_RESULT.REMOVEGEM_SUCCESS:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_REMOVE_GEM_SUCCESS);

                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "GemRemoveSuccess");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_SUCCESS1:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_REMOVE_GEM_SUCCESS);

                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "GemRemoveSuccess");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_NO_MAT:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "MatNotExist");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_ERROR_GEMINDEX:
                    case REMOVEGEM_RESULT.REMOVEGEM_TOO_LARGE_GEMINDEX:
                        {
                            //请选择要摘除的宝石
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "SelectGem");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_NO_GEM:
                        {

                            //STRING strTemp = "不存在要摘除的宝石" ;
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "GemNotExist");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_INVALID_EQUIP:
                    case REMOVEGEM_RESULT.REMOVEGEM_NO_ITEM:
                        {

                            //STRING strTemp = "请放入要摘除的装备" ;
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "NoEquip");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_BAG_FULL:
                        {

                            //STRING strTemp = "背包已满，请先整理出空位放置宝石" ;
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "inventory full");
                        }
                        break;
                    case REMOVEGEM_RESULT.REMOVEGEM_UNKNOW_ERROR:
                    default:
                        {
                            //CGameProcedure::s_pGfxSystem->PushDebugString("Equip UNKNOWN ERROR");
                        }
                        break;
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_REMOVEGEMRESULT;
        }
    }
}