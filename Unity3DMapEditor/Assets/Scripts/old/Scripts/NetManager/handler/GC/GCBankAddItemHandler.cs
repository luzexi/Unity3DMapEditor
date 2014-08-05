using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankAddItemHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankAddItem packet = pPacket as GCBankAddItem;

                if (packet.FromType == (byte)GCBankAddItem.PosType.EQUIP_POS)
                {

                }
                else if (packet.FromType == (byte)GCBankAddItem.PosType.BAG_POS)
                {
                    CObject_Item pItemFrom = CDataPool.Instance.UserBag_GetItemByIndex(packet.IndexFrom);
                    CObject_Item pItemTo = CDataPool.Instance.UserBank_GetItem(packet.IndexTo);

                    switch ((GCBankAddItem.OperateType)packet.Operatetype)
                    {
                        case GCBankAddItem.OperateType.OPERATE_MOVE:		// 移动到空格
                            {
                                CDataPool.Instance.UserBank_SetItem(packet.IndexTo, pItemFrom, true);
                                CDataPool.Instance.UserBag_SetItem(packet.IndexFrom, null, false, false);
                            }
                            break;
                        case GCBankAddItem.OperateType.OPERATE_SPLICE:		// 合并
                            {
                                //CDataPool::GetMe()->UserBank_SetItem(indexTo, pItemFrom);
                                //CDataPool::GetMe()->UserBag_SetItem(indexFrom, NULL, FALSE);
                            }
                            break;
                        case GCBankAddItem.OperateType.OPERATE_SWAP:		// 交换
                            {
                                CDataPool.Instance.UserBank_SetItem(packet.IndexTo, pItemFrom, false);
                                CDataPool.Instance.UserBag_SetItem(packet.IndexFrom, pItemTo, false, false);
                                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItemTo.GetID());
                            }
                            break;
                        default:
                            break;
                    }
                    CActionSystem.Instance.UserBank_Update();
                    CActionSystem.Instance.UserBag_Update();
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "物品存入银行成功");
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                }
                else if (packet.FromType == (byte)GCBankAddItem.PosType.ERROR_POS)
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "物品存入银行失败");
                }

            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKADDITEM;
        }
    }
}