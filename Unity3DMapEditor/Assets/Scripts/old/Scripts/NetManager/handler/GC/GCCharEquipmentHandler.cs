
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCCharEquipmentHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPack, ref Peer pPlayer) 
        {
            LogManager.Log("RECV GCCharEquipment Packet ");
	        if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
                
		        CObjectManager pObjectManager = CObjectManager.Instance;
                GCCharEquipment pPacket = (GCCharEquipment)pPack;
		        CObject pObj = (CObject)(pObjectManager.FindServerObject((int)pPacket.getObjID() ));
		        if(pObj == null || !(pObj is CObject_Character))
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        /*
		        static UINT pBuf[UPDATE_CHAR_ATT_NUMBERS];
		        pPacket.FillParamBuf( (VOID*)pBuf );

		        SCommand_Object cmdTemp;
		        cmdTemp.m_wID			= OC_UPDATE_EQUIPMENT;
		        cmdTemp.m_adwParam[0]	= pPacket.getFlags();
		        cmdTemp.m_apParam[1]	= (VOID*)pBuf;
		        pObj.PushCommand(&cmdTemp );
        */
		        //刷入数据池
		        CCharacterData  pCharacterData = ((CObject_Character)pObj).GetCharacterData();
        //		if(pObj == (CObject*)CObjectManager::GetMe().GetMySelf())
		        {
			        uint dwEquipFlag = pPacket.getFlags();

			        if( (dwEquipFlag & (1<<(int)HUMAN_EQUIP.HEQUIP_WEAPON)) != 0) 
			        {
                        pCharacterData.Set_EquipGem(HUMAN_EQUIP.HEQUIP_WEAPON, (int)pPacket.getWeaponGemID());
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_WEAPON, (int)pPacket.getWeaponID());
			        }

                    if ((dwEquipFlag & (1 <<(int) HUMAN_EQUIP.HEQUIP_CAP)) != 0)	 
			        {
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_CAP, (int)pPacket.getCapID());
			        }

                    if ((dwEquipFlag & (1 << (int)HUMAN_EQUIP.HEQUIP_ARMOR)) != 0)
			        {
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_ARMOR, (int)pPacket.getArmourID());
                        pCharacterData.Set_EquipGem(HUMAN_EQUIP.HEQUIP_ARMOR, (int)pPacket.getArmourGemID());
			        }

                    if ((dwEquipFlag & (1 << (int)HUMAN_EQUIP.HEQUIP_CUFF)) != 0) 
			        {
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_CUFF, (int)pPacket.getCuffID());
			        }

                    if ((dwEquipFlag & (1 << (int)HUMAN_EQUIP.HEQUIP_BOOT)) != 0)  
			        {
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_BOOT, (int)pPacket.getBootID());
			        }

			        //  [8/30/2010 Sun]
                    if ((dwEquipFlag & (1 << (int)HUMAN_EQUIP.HEQUIP_BACK)) != 0)
			        {
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_BACK, (int)pPacket.getBackID());
			        }

			        // 支持进入游戏时，服务器刷新肩甲 [4/27/2011 ivan edit]
                    if ((dwEquipFlag & (1 << (int)HUMAN_EQUIP.HEQUIP_SASH)) != 0)
			        {
                        pCharacterData.Set_Equip(HUMAN_EQUIP.HEQUIP_SASH, (int)pPacket.getSashID());
                        pCharacterData.Set_EquipGem(HUMAN_EQUIP.HEQUIP_SASH, (int)pPacket.getSashGemID());
			        }
		        }

		        pObj.PushDebugString("GCCharEquipment");
		        pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
	        }
	        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE ;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHAREQUIPMENT;
        }
    }

}

