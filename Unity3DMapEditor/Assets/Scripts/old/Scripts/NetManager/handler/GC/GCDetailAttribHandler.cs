using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCDetailAttribHandler 空壳
    /// </summary>
    public class GCDetailAttribHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCDetailAttrib detailAttribPacket = (GCDetailAttrib)pPacket;
            //LogManager.Log("RECV GCCharBaseAttrib : Flag=" + detailAttribPacket.Flags);

            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {

                CObjectManager pObjectManager = CObjectManager.Instance;

                CObject pObj = (CObject)(pObjectManager.FindServerObject((int)detailAttribPacket.ObjID));
                if (pObj == null || !(pObj is CObject_Character))
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                CCharacterData pCharacterData = ((CObject_Character)pObj).GetCharacterData();

                //        //玩家详细的属性刷新

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_LEVEL))		//0
                    pCharacterData.Set_Level((int)detailAttribPacket.LV);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HP))			//1
                    pCharacterData.Set_HP(detailAttribPacket.HP);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MP))			//2
                    pCharacterData.Set_MP(detailAttribPacket.MP);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_EXP))			//3
                    pCharacterData.Set_Exp(detailAttribPacket.Exp);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MONEY))		//4
                    pCharacterData.Set_Money(detailAttribPacket.Money);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_STR))  //5
                    pCharacterData.Set_STR(detailAttribPacket.Str);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_SPR))  //6
                    pCharacterData.Set_SPR(detailAttribPacket.SPR);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CON))  //7
                    pCharacterData.Set_CON(detailAttribPacket.CON);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_INT))   //8
                    pCharacterData.Set_INT(detailAttribPacket.INT);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEX))   //9
                    pCharacterData.Set_DEX(detailAttribPacket.DEX);
                //        if ( flagDetail.isSetBit( DETAIL_ATTRIB_POINT_REMAIN) )//10
                //            pCharacterData->Set_PointRemain(pPacket->GetPoint_Remain());
                // 培养点数
                if( detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_STR_RANDOM_POINT))					//力量 力量
                    pCharacterData.Set_BringSTR(detailAttribPacket.StrRandomPoint);
                if( detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_SPR_RANDOM_POINT))					//力量 力量
                    pCharacterData.Set_BringSPR(detailAttribPacket.SprRandomPoint);
                if( detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CON_RANDOM_POINT))					
                    pCharacterData.Set_BringCON(detailAttribPacket.ConRandomPoint);
                if( detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_INT_RANDOM_POINT))					
                    pCharacterData.Set_BringINT(detailAttribPacket.IntRandomPoint);
                if( detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEX_RANDOM_POINT))					
                    pCharacterData.Set_BringDEX(detailAttribPacket.DexRandomPoint);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATT_PHYSICS))   //11
                    pCharacterData.Set_AttPhysics(detailAttribPacket.PhysicsAttk);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEF_PHYSICS))   //12
                    pCharacterData.Set_DefPhysics(detailAttribPacket.PhysicsDef);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATT_MAGIC))   //13
                    pCharacterData.Set_AttMagic(detailAttribPacket.MagicAttk);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEF_MAGIC))   //14
                    pCharacterData.Set_DefMagic(detailAttribPacket.MagicDef);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAXHP))		//15
                    pCharacterData.Set_MaxHP(detailAttribPacket.MaxHP);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAXMP))		//16
                    pCharacterData.Set_MaxMP(detailAttribPacket.MaxMP);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HP_RESPEED))	//17
                    pCharacterData.Set_HPRespeed(detailAttribPacket.HPReSpeed);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MP_RESPEED))	//18
                    pCharacterData.Set_MPRespeed(detailAttribPacket.MPReSpeed);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HIT))	//19
                    pCharacterData.Set_Hit(detailAttribPacket.Hit);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MISS))	//20
                    pCharacterData.Set_Miss(detailAttribPacket.Miss);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CRITRATE))	//21
                    pCharacterData.Set_CritRate(detailAttribPacket.CriticalRate);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCE_C))	//24
                    pCharacterData.Set_DefCritRate(detailAttribPacket.CriticalDef);


                //if ( detailAttribPacket.IsSetBit( ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_RAGE) )	//22
                //{
                //    pCharacterData.Set_Rage(detailAttribPacket.Rage);

                //    // 怒气更新要通知界面。2006－4－26
                //    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_RAGE);
                //}

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_STRIKE_POINT))	//23
                    pCharacterData.Set_StrikePoint(detailAttribPacket.StrikePoint);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MOVESPEED))			//24
                    pCharacterData.Set_MoveSpeed(detailAttribPacket.MoveSpeed);

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKSPEED))	//25
                //            pCharacterData->Set_AttackSpeed(pPacket->GetAttSpeed());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKCOLD))	//26
                //            pCharacterData->Set_AttWater(pPacket->GetAttCold());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCECOLD))	//27
                //            pCharacterData->Set_DefWater(pPacket->GetDefCold());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKFIRE))	//28
                //            pCharacterData->Set_AttFire(pPacket->GetAttFire());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCEFIRE))	//29
                //            pCharacterData->Set_DefFire(pPacket->GetDefFire());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKLIGHT))	//30
                //            pCharacterData->Set_AttMetal(pPacket->GetAttLight());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCELIGHT))	//31
                //            pCharacterData->Set_DefMetal(pPacket->GetDefLight());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKPOISON))	//32
                //            pCharacterData->Set_AttWood(pPacket->GetAttPoison());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCEPOISON))	//33
                //            pCharacterData->Set_DefWood(pPacket->GetDefPoison());

                //        // 增加土攻土防 [11/9/2011 Ivan edit]
                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKEARTH))
                //            pCharacterData->Set_AttEarth(pPacket->GetAttEarth());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCEEARTH))
                //            pCharacterData->Set_DefEarth(pPacket->GetDefEarth());

                //// 		if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKWIND))	//风攻
                //// 			pCharacterData->Set_AttWind(pPacket->GetAttWind());
                //// 
                //// 		if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCEWIND))	//风防
                //// 			pCharacterData->Set_DefWind(pPacket->GetDefWind());
                //// 
                //// 		if(flagDetail.isSetBit(DETAIL_ATTRIB_ATTACKTHUNDER))	//雷攻
                //// 			pCharacterData->Set_AttThunder(pPacket->GetAttThunder());
                //// 
                //// 		if(flagDetail.isSetBit(DETAIL_ATTRIB_DEFENCETHUNDER))	//雷防
                //// 			pCharacterData->Set_DefThunder(pPacket->GetDefThunder());

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MENPAI))		//34
                    pCharacterData.Set_MenPai(detailAttribPacket.MenPai);

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_GUILD)	)		//34
                //            pCharacterData->Set_Guild(pPacket->GetGuild());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_CAMP)	)			//35
                //            pCharacterData->Set_CampData(pPacket->GetCampData());

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DATAID))			//36
                    //pCharacterData.Set_RaceID(detailAttribPacket.DataID); //因为数据包接收错误 暂时屏蔽接收Race ID

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MODELID))			//36
                    pCharacterData.Set_ModelID(detailAttribPacket.ModelID);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MOUNTID))			//37
                    pCharacterData.Set_MountID(detailAttribPacket.MountID);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CURRENT_PET_GUID))			//37
                {
                    pCharacterData.Set_CurrentPetGUID(detailAttribPacket.CurrentPet);
                }

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_LIMIT_MOVE))	//38
                    pCharacterData.SetLimitMove(detailAttribPacket.LimitMove);

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_CAN_ACTION1)	)	//39
                //            pCharacterData->SetCanActionFlag1(pPacket->GetCanActionFlag1());

                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_CAN_ACTION2)	)	//40
                //            pCharacterData->SetCanActionFlag2(pPacket->GetCanActionFlag2());

                //        //if(flagDetail.isSetBit(DETAIL_ATTRIB_LIMIT_USE_SKILL)	)	//39
                //        //	pCharacterData->SetLimitUseSkill(pPacket->GetLimitUseSkill());

                //        //if(flagDetail.isSetBit(DETAIL_ATTRIB_LIMIT_HANDLE)	)	//40
                //        //	pCharacterData->SetLimitHandle(pPacket->GetLimitHandle());

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_RMB))
                    pCharacterData.Set_RMB(detailAttribPacket.RMBMoney);
                //// 		if(flagDetail.isSetBit(DETAIL_ATTRIB_MAX_VIGOR) )
                //// 			pCharacterData->Set_MaxVigor(pPacket->GetMaxVigor());

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ENERGY))
                    pCharacterData.Set_DoubleExpTime_Num(detailAttribPacket.DoubleExpTimeNum);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HELPMASK))
                    pCharacterData.Set_TutorialMask(detailAttribPacket.HelpMask);

                if (detailAttribPacket.IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAX_ENERGY))
                    pCharacterData.Set_MaxEnergy(detailAttribPacket.MaxEnergy);

                //        //if(flagDetail.isSetBit(DETAIL_ATTRIB_GOODBADVALUE) )
                //        //	pCharacterData->Set_GoodBadValue(pPacket->GetGoodBadValue());

                ////		if(flagDetail.isSetBit(DETAIL_ATTRIB_MISSION_HAVEDONE_FLAGS) )
                ////			pCharacterData->Set_QuestHistory(pPacket->GetMissionHaveDoneFlags());

                //// 		if(flagDetail.isSetBit(DETAIL_ATTRIB_AMBIT_EXP))
                //// 			pCharacterData->Set_AmbitExp(pPacket->GetAmbitExp());
                //// 
                //        if(flagDetail.isSetBit(DETAIL_ATTRIB_AMBIT))
                //            pCharacterData->Set_Ambit(pPacket->GetAmbit());

                //        pObj->PushDebugString("GCDetailAttrib");
                //        pObj->SetMsgTime(CGameProcedure::s_pTimeSystem->GetTimeNow());
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAILATTRIB;
        }
    }
}
