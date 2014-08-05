using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCDetailPetEquipListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.Log("RECV GCDetailPetEquipList");
                GCDetailPetEquipList packet = pPacket as GCDetailPetEquipList;
                CObject pObj = CObjectManager.Instance.FindServerObject((int)packet.ObjID);
                //判定这个宠物是否是其他玩家，如果是，那么objid有效
                uint flags = packet.PartFlags;
                if (pObj != null)
                {
                    for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
                    {
                        if ((flags & (1 << i)) != 0)
                        {
                            _ITEM item = packet.Items[i];
                            CObject_Item pItemObj = ObjectSystem.Instance.NewItem(item.m_ItemIndex);
                            if (pItemObj == null)
                            {
                                CDataPool.Instance.OtherPlayerPetEquip_SetItem(packet.GUID, (PET_EQUIP)i, null, true);
                            }
                            else
                            {
                                pItemObj.SetGUID((ushort)item.m_ItemGUID.m_World,
                                    (ushort)item.m_ItemGUID.m_Server,
                                    (uint)item.m_ItemGUID.m_Serial);
                                pItemObj.SetExtraInfo(ref item);
                                pItemObj.SetTypeOwner(ITEM_OWNER.IO_PLAYEROTHER_EQUIP);
                                pItemObj.PosIndex = (short)i;
                                CDataPool.Instance.OtherPlayerPetEquip_SetItem(packet.GUID, (PET_EQUIP)i, pItemObj, true);
                            }
                        }
                        else
                        {
                            CDataPool.Instance.OtherPlayerPetEquip_SetItem(packet.GUID, (PET_EQUIP)i, null, true);
                        }
                    }
                    CActionSystem.Instance.OtherPlayerPetEquip_Update(packet.GUID);
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OTHERPLAYER_UPDATE_PETEQUIP);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
                
                SDATA_PET curPet = CDataPool.Instance.Pet_GetPet(packet.GUID);
                if (curPet == null)
                {
                    LogManager.LogError("找不到对应的Pet, GUID:" + packet.GUID.m_uHighSelection + " " + packet.GUID.m_uLowSelection);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
                
                for (int i = 0; i < (int)(int)PET_EQUIP.PEQUIP_NUMBER; i++)
                {
                    if ((flags & (1 << i)) != 0)
                    {
                        _ITEM item = packet.Items[i];
                        LogManager.Log("GCDetailPetEquipList flags " + flags + " " + item.m_ItemIndex);
                        CObject_Item pItemObj = ObjectSystem.Instance.NewItem(item.m_ItemIndex);
                        pItemObj.SetGUID((ushort)item.m_ItemGUID.m_World,
                            (ushort)item.m_ItemGUID.m_Server,
                            (uint)item.m_ItemGUID.m_Serial);
                        pItemObj.SetExtraInfo(ref item);
                        pItemObj.SetTypeOwner(ITEM_OWNER.IO_PET_EQUIPT);
                        pItemObj.PosIndex = (short)i;
                        CDataPool.Instance.UserPetEquipt_SetItem(packet.GUID, (PET_EQUIP)i, pItemObj, true);
                    }
                }
                CActionSystem.Instance.UserPetEquiptItem_Update(packet.GUID);
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PETEQUIP);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAIL_PET_EQUIPLIST;
        }
    }
}