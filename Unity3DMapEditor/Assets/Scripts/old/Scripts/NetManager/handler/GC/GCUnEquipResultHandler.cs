using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCUnEquipResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCUnEquipResult packet = (GCUnEquipResult)pPacket;
                UnEquipResultCode ret = (UnEquipResultCode)packet.Result;
                switch (ret)
                {
                    case UnEquipResultCode.UNEQUIP_SUCCESS:
                        {
				            //---------------------------------------------------
				            //数据池
                            CObject_Item pItem = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)packet.EquipPoint);
				            if(pItem == null) 
                                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

				            // 改变物品的id, 以便更新新的action 按钮
                            ObjectSystem.Instance.ChangeItemClientID(pItem);
                            CDataPool.Instance.UserBag_SetItem((short)packet.BagIndex,
                                pItem, true,false);
                            CDataPool.Instance.UserEquip_SetItem((HUMAN_EQUIP)packet.EquipPoint,
                                null, false);

				            //---------------------------------------------------
				            //逻辑层
                            CObjectManager.Instance.getPlayerMySelf().UnEquipItem((HUMAN_EQUIP)packet.EquipPoint);

				            //---------------------------------------
				            //刷新角色属性
                            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_Equip(
                                (HUMAN_EQUIP)packet.EquipPoint, -1);

				            // 更新主角身上的装备到ActionSystem
                            CActionSystem.Instance.UserEquip_Update();
                            CActionSystem.Instance.UserBag_Update();

				            // 通知界面事件
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_EQUIP);
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
			            }
                        break;
                    case UnEquipResultCode.UNEQUIP_BAG_FULL:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "背包已满");
                        break;
                    case UnEquipResultCode.UNEQUIP_HAS_ITEM:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "该位置已经有物品");
                        break;
                    default:
                        break;
                }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_UNEQUIPRESULT;
        }
    }
}