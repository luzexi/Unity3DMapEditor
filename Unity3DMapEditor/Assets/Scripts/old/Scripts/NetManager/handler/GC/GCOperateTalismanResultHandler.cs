using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCOperateTalismanResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCOperateTalismanResult packet = pPacket as GCOperateTalismanResult;
                string resultInfo = "";
                switch ((GCOperateTalismanResult.UseOperateTalismanResultCode)packet.Result)
                {
                    case GCOperateTalismanResult.UseOperateTalismanResultCode.USEOPERATETALISMAN_SUCCESS:
                        resultInfo = "成功";
                        break;
                    case GCOperateTalismanResult.UseOperateTalismanResultCode.USEOPERATETALISMAN__FAIL:
                        resultInfo = "失败";
                        break;
                    case GCOperateTalismanResult.UseOperateTalismanResultCode.USEOPERATETALISMAN_BAG_FULL:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "法宝栏已满");
                        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                    case GCOperateTalismanResult.UseOperateTalismanResultCode.USEOPERATETALISMAN_HAS_ITEM:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "法宝装备栏已满");
                        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                    case GCOperateTalismanResult.UseOperateTalismanResultCode.USEOPERATETALISMAN_CANNOT_EAT:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "法宝不能吃");
                        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                    case GCOperateTalismanResult.UseOperateTalismanResultCode.USEOPERATETALISMAN_MAX_LEVE:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "法宝已经达到最大等级");
                        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
               

                string operatorResult = "";
                switch ((CGOperateTalisman.EOPTMType)packet.Type)
                {
                    case CGOperateTalisman.EOPTMType.EOP_TM_EAT:
                        {
                            operatorResult = "吞噬";
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_COMPOUND_TALISMANITEM_RESULT,packet.Result);
                        }
                        break;
                    case CGOperateTalisman.EOPTMType.EOP_TM_EQUIP:
                        {
                            CTalisman_Item bagItem = CDataPool.Instance.TalismanInventory_GetItem(packet.BagIndex - GAMEDEFINE.MAX_BAG_SIZE);
                            CDataPool.Instance.TalismanEquip_SetItem(packet.EquiptIndex, bagItem as CObject_Item, true, true);
                            CDataPool.Instance.TalismanInventory_SetItem((short)(packet.BagIndex - GAMEDEFINE.MAX_BAG_SIZE), null, true, true);
                            CActionSystem.Instance.UserTalismanBag_Update();
                            CActionSystem.Instance.UserTalismanEquip_Update();
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_TALISMANITEM_CHANGED, packet.BagIndex - GAMEDEFINE.MAX_BAG_SIZE);
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_TALISMANEQUIPT_CHANGED, packet.EquiptIndex);
                            operatorResult = "法宝装备";
                        }
                        break;
                    case CGOperateTalisman.EOPTMType.EOP_TM_UNEQUIP:
                        {
                            operatorResult = "法宝卸下装备";
                            CTalisman_Item equiptItem = CDataPool.Instance.TalismanEquipment_GetItem(packet.EquiptIndex);
                            CDataPool.Instance.TalismanInventory_SetItem((short)(packet.BagIndex - GAMEDEFINE.MAX_BAG_SIZE), equiptItem as CObject_Item, true, true);
                            CDataPool.Instance.TalismanEquip_SetItem(packet.EquiptIndex, null, true, true);
                            CActionSystem.Instance.UserTalismanEquip_Update();
                            CActionSystem.Instance.UserTalismanBag_Update();
                            
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_TALISMANITEM_CHANGED, packet.BagIndex - GAMEDEFINE.MAX_BAG_SIZE);
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_TALISMANEQUIPT_CHANGED, packet.EquiptIndex);
                        }
                        break;
                }
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, operatorResult + resultInfo);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_OPERATETALISMANRESULT;
        }
    }
}