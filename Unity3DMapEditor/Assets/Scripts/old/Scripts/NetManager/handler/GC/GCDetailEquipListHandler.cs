using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCDetailEquipListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase packet, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCDetailEquipList pPacket = packet as GCDetailEquipList;
                CObject pObj = CObjectManager.Instance.FindServerObject((int)pPacket.ObjId);
		        if(pObj == null || ! (pObj is CObject_Character))
		        {
                    LogManager.LogError("找不到对应的NPC, ID:" + pPacket.ObjId);
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }

		        CCharacterData pCharacterData = ((CObject_Character)pObj).GetCharacterData();

		        //刷入数据池
		        uint dwEquipMask = pPacket.PartFlags;
		        if(pObj != (CObject)CObjectManager.Instance.getPlayerMySelf())
		        {
			        for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
			        {
				        if ((dwEquipMask & (1 << i) ) != 0) 
				        {
                            _ITEM item = pPacket.GetEquipData((HUMAN_EQUIP)i);
					        CObject_Item pItemObj = ObjectSystem.Instance.NewItem(item.m_ItemIndex);
					        if(pItemObj == null)
					        {
                                CDataPool.Instance.OtherPlayerEquip_SetItem((HUMAN_EQUIP)i, null, true);
					        }
					        else
					        {
						        pItemObj.SetGUID((ushort)item.m_ItemGUID.m_World,
							        (ushort)item.m_ItemGUID.m_Server,
							        (uint)item.m_ItemGUID.m_Serial);
                                pItemObj.SetExtraInfo(ref item);
                                pItemObj.SetTypeOwner(ITEM_OWNER.IO_PLAYEROTHER_EQUIP);
						        pItemObj.PosIndex = (short)i;
        						
						        CDataPool.Instance.OtherPlayerEquip_SetItem((HUMAN_EQUIP)i, pItemObj,true);

					        }
				        }
				        else
				        {
                            CDataPool.Instance.OtherPlayerEquip_SetItem((HUMAN_EQUIP)i, null, true);
				        }

			        }
        			
// 			        CObject_PlayerOther* TargetAvatar = CObjectManager::GetMe()->GetTarget_Avatar();
// 
// 			        TargetAvatar->GetCharacterData()->Set_RaceID( ((CObject_Character*)pObj)->GetCharacterData()->Get_RaceID() );
// 			        for(INT i=0; i<HEQUIP_NUMBER; i++)
// 			        {
// 				        TargetAvatar->GetCharacterData()->Set_Equip((HUMAN_EQUIP)i,((CObject_Character*)pObj)->GetCharacterData()->Get_Equip((HUMAN_EQUIP)i));
// 			        }
// 			        TargetAvatar->SetFaceDir(0.0f);
//         			
// 			        CDataPool::GetMe()->Copy_To_TargetEquip(pObj);

			        // 更新其他玩家身上的装备到ActionSystem
                    CActionSystem.Instance.OtherPlayerEquip_Update();

			        // 通知界面事件
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OTHERPLAYER_UPDATE_EQUIP);
		        }
		        else
		        {
			        for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
			        {
				        if ((dwEquipMask & (1 << i) ) != 0) 
				        {
                            _ITEM item = pPacket.GetEquipData((HUMAN_EQUIP)i);
					        CObject_Item pItemObj = ObjectSystem.Instance.NewItem(item.m_ItemIndex);
					        if(pItemObj == null)
                                continue;

					        pItemObj.SetGUID((ushort)item.m_ItemGUID.m_World,
						        (ushort)item.m_ItemGUID.m_Server,
						        (uint)item.m_ItemGUID.m_Serial);
                            pItemObj.SetExtraInfo(ref item);
                            pItemObj.SetTypeOwner(ITEM_OWNER.IO_MYSELF_EQUIP);
					        pItemObj.PosIndex = (short)i;
    						
					        CDataPool.Instance.UserEquip_SetItem((HUMAN_EQUIP)i, pItemObj,true);
				        }
			        }

			        // 更新主角身上的装备到ActionSystem
                    CActionSystem.Instance.UserEquip_Update();

			        // 通知界面事件
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_EQUIP);
        		
		        }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAILEQUIPLIST;
        }
    }
}