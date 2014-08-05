using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCUseEquipResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase packet, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCUseEquipResult pPacket = packet as GCUseEquipResult;

                UseEquipResultCode ret = (UseEquipResultCode)pPacket.Result;
                switch (ret)
                {
                    case UseEquipResultCode.USEEQUIP_SUCCESS:
                        {
                            //---------------------------------------------------------
                            //背包里的装备
                            CObject_Item pItemAtBag = CDataPool.Instance.UserBag_GetItemByIndex(pPacket.BagIndex);
                            if (pItemAtBag == null)
                                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                            //身上的装备
                            CObject_Item pItemAtUser = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)pPacket.EquipPoint);

                            HUMAN_EQUIP equipPoint = ((CObject_Item_Equip)pItemAtBag).GetItemType();
                            //装配信息不对
                            if (pItemAtBag.GetItemClass() != ITEM_CLASS.ICLASS_EQUIP ||
                                equipPoint != (HUMAN_EQUIP)pPacket.EquipPoint)
                                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                            //如果发生物品转移，则改变客户端id，以表示在客户端是不同物体
                            ObjectSystem.Instance.ChangeItemClientID(pItemAtBag);
                            if (pItemAtUser != null)
                                ObjectSystem.Instance.ChangeItemClientID(pItemAtUser);

                            //---------------------------------------
                            //刷新角色属性
                            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_Equip(
                                ((CObject_Item_Equip)pItemAtBag).GetItemType(), pItemAtBag.GetIdTable());

                            //刷新数据池
                            CDataPool.Instance.UserEquip_SetItem(((CObject_Item_Equip)pItemAtBag).GetItemType(),
                                pItemAtBag, false);

                            //---------------------------------------
                            //刷新包裹数据
                            if (pItemAtBag != null)
                            {
                                pItemAtBag.SetGUID((ushort)pPacket.ItemID.m_World,
                                                    (ushort)pPacket.ItemID.m_Server,
                                                    (uint)pPacket.ItemID.m_Serial);
                            }
                            CDataPool.Instance.UserBag_SetItem((short)pPacket.BagIndex, pItemAtUser, false, false);
                            CActionSystem.Instance.UserBag_Update();
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, pPacket.BagIndex);

                            //自动使用骑术
                            if (equipPoint == HUMAN_EQUIP.HEQUIP_RIDER)
                                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_EQUIP_MOUNT);
                        }
                        break;
                    case UseEquipResultCode.USEEQUIP_LEVEL_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "物品的装配等级高于您目前的等级");
                        }
                        break;
                    case UseEquipResultCode.USEEQUIP_TYPE_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "装配类型错误");
                        }
                        break;
                    case UseEquipResultCode.USEEQUIP_JOB_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "职业不匹配");
                        }
                        break;
                    case UseEquipResultCode.USEEQUIP_IDENT_FAIL:
                    default:
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "未知错误");
                        break;
                }

                // 更新主角身上的装备到ActionSystem
                CActionSystem.Instance.UserEquip_Update();

                // 通知界面事件
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_EQUIP);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USEEQUIPRESULT;
        }
    }
}