
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCNewMonsterHandler 空壳
    /// </summary>
    public class GCCharBaseAttribHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharBaseAttrib charBaseAttribPacket = (GCCharBaseAttrib) pPacket;
            //LogManager.Log("RECV GCCharBaseAttrib : Flag=" + charBaseAttribPacket.Flag);

            if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
                //LogManager.LogWarning("RECV GCCharBaseAttrib : id=" + charBaseAttribPacket.ObjectID);
		        CObjectManager pObjectManager = CObjectManager.Instance;

		        CObject pObj = (CObject)(pObjectManager.FindServerObject(charBaseAttribPacket.ObjectID ));
		        if(pObj == null || !(pObj is CObject_Character))
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

		        bool bAddLoadTask = false;

		        CCharacterData pCharacterData = ((CObject_Character)pObj).GetCharacterData();

		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_DATA_ID) && !bAddLoadTask)
		        {
                    try
                    {
                        //if (pObj is CObject_PlayerMySelf)
                        //{
                        //    LogManager.Log("主角收到RaceID:" + charBaseAttribPacket.DataID);
                        //}
                        //else if (pObj is CObject_PlayerOther)
                        //{
                        //    LogManager.Log("其他玩家收到RaceID:" + charBaseAttribPacket.DataID);
                        //}
                        pCharacterData.Set_RaceID(charBaseAttribPacket.DataID);
                        //LogManager.LogWarning("SetRace in GCCharBaseAttribHandler = " + charBaseAttribPacket.DataID + " ObjectID= " + pObj.ServerID);
                    }
                    catch (Exception e)
                    {
                        LogManager.LogError(e.ToString());
                    }
		        }
                if (charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_CAMP_ID))
		        {
			        pCharacterData.Set_CampData(charBaseAttribPacket.CampData);
		        }
		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_NAME))
		        {
                   string name = UIString.Instance.GetUnicodeString(charBaseAttribPacket.Name);
			        pCharacterData.Set_Name(name);
		        }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_TITLE))
// 		        {
// 			        pCharacterData.Set_CurTitle(charBaseAttribPacket.getTitle());
// 			        pCharacterData.Set_CurTitleType(charBaseAttribPacket.getTitleType());
// 			        CEventSystem::GetMe().PushEvent(GE_CUR_TITLE_CHANGEED);
// 		        }
 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_LEVEL))
 		        {
 			        pCharacterData.Set_Level(charBaseAttribPacket.Level);
 		        }
		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_HP_PERCENT))
		        {
 			        pCharacterData.Set_HPPercent(charBaseAttribPacket.HPPercent/100.0f);
 		        }
 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MP_PERCENT))
 		        {
                    pCharacterData.Set_MPPercent(charBaseAttribPacket.MPPercent/ 100.0f);
 		        }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_RAGE))
// 		        {
// 			        pCharacterData.Set_Rage(charBaseAttribPacket.getRage() );
// 		        }
                if (charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_STEALTH_LEVEL))
                {
                    pCharacterData.Set_StealthLevel(charBaseAttribPacket.StealthLevel);
                }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_SIT))
// 		        {
// 			        //pCharacterData.Set_Sit(charBaseAttribPacket.IsSit() );
// 		        }
		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOVE_SPEED))
                {
                    if (pObj is CObject_PlayerMySelf)
                    {
                        LogManager.Log("主角收到MOVE_SPEED:" + charBaseAttribPacket.MoveSpeed);
                    }
			        pCharacterData.Set_MoveSpeed(charBaseAttribPacket.MoveSpeed);
		        }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_ATTACK_SPEED))
// 		        {
// 			        //必须是主角
// 			        if(pObj == CObjectManager::GetMe().GetMySelf())
// 				        pCharacterData.Set_AttackSpeed((int)charBaseAttribPacket.AttackSpeed);
// 		        }
        		
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OWNER))
// 		        {
// 			        pCharacterData.Set_OwnerID(charBaseAttribPacket.OwnerID);
// 		        }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OCCUPANT))
// 		        {
// 			        pCharacterData.Set_OccupantGUID(charBaseAttribPacket.OccupantGUID);
// 		        }
                if (charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PORTRAIT_ID))
                {
                    pCharacterData.Set_PortraitID(charBaseAttribPacket.PortraitID);
                }
		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MODEL_ID) && !bAddLoadTask)
		        {
			        pCharacterData.Set_ModelID(charBaseAttribPacket.ModelID);
		        }
                if (charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOUNT_ID) && !bAddLoadTask)
                {
                   pCharacterData.Set_MountID(charBaseAttribPacket.MountID);//坐骑ID
                }
		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA))
		        {
			        pCharacterData.Set_FaceMesh(charBaseAttribPacket.FaceMesh);
			        pCharacterData.Set_HairMesh(charBaseAttribPacket.HairMesh);
			        pCharacterData.Set_HairColor(charBaseAttribPacket.HairColor);
		        }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_IS_IN_STALL))
// 		        {
// 			        pCharacterData.Set_IsInStall(charBaseAttribPacket.getIsOpen());
// 		        }
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_STALL_NAME))
// 		        {
// 			        pCharacterData.Set_StallName(charBaseAttribPacket.getStallName());
// 		        }
// 
// 		        if(charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_STALL_NAME))
// 		        {
// 			        pCharacterData.Set_StallName(charBaseAttribPacket.getStallName());
// 		        }
//		        pObj.SetMsgTime(CGameProcedure::s_pTimeSystem.GetTimeNow());

		        //如果不是玩家自己
//		        if(pObj != (CObject*)CObjectManager::GetMe().GetMySelf() && charBaseAttribPacket.IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_AITYPE))
	///	        {
	//		        pCharacterData.Set_AIType(charBaseAttribPacket.getAIType());

	//	        }
	        }
	        else
	        {
	        }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARBASEATTRIB;
        }
    }
}
