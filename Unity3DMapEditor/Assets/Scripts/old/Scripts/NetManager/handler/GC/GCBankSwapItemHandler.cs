using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankSwapItemHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankSwapItem packet = pPacket as GCBankSwapItem;
                byte indexTo = packet.IndexTo;
                byte indexFrom = packet.IndexFrom;

                if (packet.FromType == (byte)GCBankSwapItem.PosType.ERROR_POS
                    || packet.ToType == (byte)GCBankSwapItem.PosType.ERROR_POS
                    )
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "非法命令");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }


                switch ((GCBankSwapItem.PosType)packet.FromType)
                {
                    case GCBankSwapItem.PosType.EQUIP_POS:
                        {
                            switch ((GCBankSwapItem.PosType)packet.ToType)
                            {
                                case GCBankSwapItem.PosType.BAG_POS:
                                    {
                                    }
                                    break;
                                case GCBankSwapItem.PosType.EQUIP_POS:
                                    {
                                    }
                                    break;
                                case GCBankSwapItem.PosType.BANK_POS:
                                    {
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case GCBankSwapItem.PosType.BAG_POS:
                        {
                            switch ((GCBankSwapItem.PosType)packet.ToType)
                            {
                                case GCBankSwapItem.PosType.BAG_POS:
                                    {
                                    }
                                    break;
                                case GCBankSwapItem.PosType.EQUIP_POS:
                                    {
                                    }
                                    break;
                                case GCBankSwapItem.PosType.BANK_POS:
                                    {
                                        CObject_Item pItemFrom = CDataPool.Instance.UserBag_GetItemByIndex(indexFrom);
                                        CObject_Item pItemTo = CDataPool.Instance.UserBank_GetItem(indexTo);

                                        CDataPool.Instance.UserBank_SetItem(indexTo, pItemFrom, false);
                                        CDataPool.Instance.UserBag_SetItem(indexFrom, pItemTo, false, false);
                                        CActionSystem.Instance.UserBank_Update();
                                        CActionSystem.Instance.UserBag_Update();

                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "背包到银行物品交换成功");
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItemTo.GetID());
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case GCBankSwapItem.PosType.BANK_POS:
                        {
                            switch ((GCBankSwapItem.PosType)packet.ToType)
                            {
                                case GCBankSwapItem.PosType.BAG_POS:
                                    {

                                        CObject_Item pItemFrom = CDataPool.Instance.UserBank_GetItem(indexFrom);
                                        CObject_Item pItemTo = CDataPool.Instance.UserBag_GetItemByIndex(indexTo);

                                        CDataPool.Instance.UserBag_SetItem(indexTo, pItemFrom, false, false);
                                        CDataPool.Instance.UserBank_SetItem(indexFrom, pItemTo, false);
                                        CActionSystem.Instance.UserBank_Update();
                                        CActionSystem.Instance.UserBag_Update();

                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "银行到背包物品交换成功");
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_GET_NEWEQUIP, pItemFrom.GetID());
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                                    }
                                    break;
                                case GCBankSwapItem.PosType.EQUIP_POS:
                                    {
                                    }
                                    break;
                                case GCBankSwapItem.PosType.BANK_POS:
                                    {
                                        CObject_Item pItemFrom = CDataPool.Instance.UserBank_GetItem(indexFrom);
                                        CObject_Item pItemTo = CDataPool.Instance.UserBank_GetItem(indexTo);

                                        CDataPool.Instance.UserBank_SetItem(indexTo, pItemFrom, false);
                                        CDataPool.Instance.UserBank_SetItem(indexFrom, pItemTo, false);
                                        CActionSystem.Instance.UserBank_Update();

                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "银行到银行物品移位成功");
                                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKSWAPITEM;
        }
    }
}