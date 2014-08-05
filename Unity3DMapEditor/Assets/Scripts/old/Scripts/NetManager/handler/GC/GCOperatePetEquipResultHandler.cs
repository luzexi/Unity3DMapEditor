using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCOperatePetEquipResultHandler : HandlerBase
    {
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_OPERATE_PET_EQUIP;
        }

        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.Log("RECV GCOperatePetEquipResult");
                GCOperatePetEquipResult packet = pPacket as GCOperatePetEquipResult;
                GCOperatePetEquipResult.UseOperatePetEquipResultCode result = (GCOperatePetEquipResult.UseOperatePetEquipResultCode)packet.Result;
                switch(result)
                {
                    case GCOperatePetEquipResult.UseOperatePetEquipResultCode.USE_PETEQUIP_FAIL:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "装备宠物失败 !o!");
                        }
                        break;
                    case GCOperatePetEquipResult.UseOperatePetEquipResultCode.USE_PETEQUIP_SUCCESS:
                        {
                            LogManager.Log("GCOperatePetEquipResult Type " + packet.OperatorType + " BagIndex " + packet.BagIndex + " EquiptIndex " + packet.EquiptIndex);

                            //背包里的装备
                            CObject_Item pItemAtBag = CDataPool.Instance.UserBag_GetItemByIndex(packet.BagIndex);

                            //身上的装备
                            CObject_Item pItemAtPet = CDataPool.Instance.UserPetEquipt_GetItem(packet.GUID,(PET_EQUIP)packet.EquiptIndex);

                            //如果发生物品转移，则改变客户端id，以表示在客户端是不同物体
                            if (pItemAtBag != null)
                                ObjectSystem.Instance.ChangeItemClientID(pItemAtBag);
                            if (pItemAtPet != null)
                                ObjectSystem.Instance.ChangeItemClientID(pItemAtPet);

                            //---------------------------------------
                     
                            //刷新数据池
                            CDataPool.Instance.UserPetEquipt_SetItem(packet.GUID, (PET_EQUIP)packet.EquiptIndex,pItemAtBag, false);

                            CDataPool.Instance.UserBag_SetItem((short)packet.BagIndex, pItemAtPet, false, false);
                            CActionSystem.Instance.UserBag_Update();
                            CActionSystem.Instance.UserPetEquiptItem_Update(packet.GUID);
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PETEQUIP);
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
                  
                        }
                        break;
                    case GCOperatePetEquipResult.UseOperatePetEquipResultCode.USE_PETEQUIP_BAG_FULL:
                        {
                             CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"背包已满 !o!");
                        }
                        break;
                    case GCOperatePetEquipResult.UseOperatePetEquipResultCode.USE_PETEQUIP_HAS_ITEM:
                        {
                            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "宠物已装备该物品 !o!");
                        }
                        break;
                }
               
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }


    }
}