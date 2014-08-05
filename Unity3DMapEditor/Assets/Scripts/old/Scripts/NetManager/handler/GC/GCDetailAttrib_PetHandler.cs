
using System;
using System.Collections.Generic;
using Network.Packets;

namespace Network.Handlers
{
    public class GCDetailAttrib_PetHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCDetailAttrib_Pet petAttribPacket = (GCDetailAttrib_Pet)pPacket;
            byte ExtraInfoLength = petAttribPacket.ExtraInfoLength;
            byte[] ExtraInfoData = new byte[GAMEDEFINE.MAX_EXTRAINFO_LENGTH];
            Array.Copy(petAttribPacket.ExtraInfoData, ExtraInfoData, ExtraInfoLength);
            LogManager.Log("RECV GCDetailAttrib_Pet");
            if (ExtraInfoLength == 0)
            {//正常情况下的宠物信息
                //第几只宠物的消息；
                SDATA_PET My_Pet = null;
                int nPet_num = -1;
                PET_GUID_t guidPet = petAttribPacket.GUID;
                int i;
                for (i = 0; i < (int)PET_INDEX.PET_INDEX_SELF_NUMBERS; i++)
                {
                    SDATA_PET pPetData = CDataPool.Instance.Pet_GetPet(i);
                    if (pPetData != null && pPetData.GUID == guidPet)
                    {
                        nPet_num = i;
                        My_Pet = pPetData;
                        break;
                    }
                }

                if (My_Pet == null)
                {
                    for (i = 0; i < (int)PET_INDEX.PET_INDEX_SELF_NUMBERS; i++)
                    {
                        SDATA_PET pPetData = CDataPool.Instance.Pet_GetPet(i);
                        if (pPetData != null && (pPetData.GUID.IsNull()))
                        {
                            nPet_num = i;
                            My_Pet = pPetData;
                            break;
                        }
                    }
                }

                if (My_Pet == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                //			My_Pet.m_nIsPresent = 2;
                My_Pet.GUID = petAttribPacket.GUID;

                if (petAttribPacket.HighFlag == 0 && petAttribPacket.LowFlag == 0)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                //	收到过宠物详细数据，把标志位置为2;
                My_Pet.IsPresent = 2;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_OBJ_ID))			//0
				{
                    LogManager.LogWarning("PET_DETAIL_ATTR_OBJ_ID idServer " + petAttribPacket.ObjectID);
					My_Pet.idServer = petAttribPacket.ObjectID;
				}

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DATA_ID))			//0
                    My_Pet.DataID = petAttribPacket.DataID;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_NAME))			//0
                    My_Pet.Name = EncodeUtility.Instance.GetUnicodeString(petAttribPacket.Name);

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_AI_TYPE))			//2
                    My_Pet.AIType = petAttribPacket.AiType;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPOUSE_GUID))		//2
                    My_Pet.SpouseGUID = petAttribPacket.SpouseGUID;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_LEVEL))			//2
                {
                    int oldLv = My_Pet.Level;
                    My_Pet.Level = petAttribPacket.Level;

                    if (My_Pet.Level > oldLv && oldLv != -1)
                    {
                        ////显示宠物升级提示信息
                        //string strTemp = COLORMSGFUNC("GCDetailAttrib_Pet_PetLevelUp", My_Pet->m_szName.c_str(), My_Pet.m_nLevel);
                        //ADDTALKMSG(strTemp);
                    }
                }

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_LIFE))			//3
                    My_Pet.Age = petAttribPacket.Life;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_GENERATION))		//5
                    My_Pet.EraCount = petAttribPacket.Generation;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HAPPINESS))		//6
                    My_Pet.Happiness = petAttribPacket.Happiness;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HP))				//7
                    My_Pet.HP = petAttribPacket.HP;
                if (My_Pet.HP < 0) My_Pet.HP = 0;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HPMAX))
                    My_Pet.HPMax = petAttribPacket.HPMax;
                
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEF_CRITICAL))
                    My_Pet.DefCritical = petAttribPacket.DefCritical;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_ATT_PHYSICS))
                    My_Pet.AttPhysics = petAttribPacket.AttPhysics;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_ATT_MAGIC))
                    My_Pet.AttMagic = petAttribPacket.AttMagic;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEF_PHYSICS))
                    My_Pet.DefPhysics = petAttribPacket.DefPhysics;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEF_MAGIC))
                    My_Pet.DefMagic = petAttribPacket.DefMagic;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HIT))
                    My_Pet.Hit = petAttribPacket.Hit;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_MISS))
                    My_Pet.Miss = petAttribPacket.Miss;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_CRITICAL))
                    My_Pet.Critical = petAttribPacket.Critical;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_MODELID))
                    My_Pet.ModelID = petAttribPacket.ModelID;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_MOUNTID))
                    My_Pet.MountID = petAttribPacket.MountID;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_EXP))
                {
                    int oldExp = My_Pet.Exp;
                    My_Pet.Exp = petAttribPacket.Exp;

                    if (My_Pet.Exp > oldExp && oldExp != -1 && oldExp != 0)
                    {
                        //CHAR szText[1024] = {0};
                        //_snprintf( szText, 1024, "你的宠物#cCCFFCC%s#W获得了经验奖励#cC8B88E%d#W", My_Pet.m_szName.c_str(), My_Pet.m_nExp-oldExp);
                        //ADDTALKMSG(szText);
                    }
                }
                if (My_Pet.Exp < 0) My_Pet.Exp = 0;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_STRPERCEPTION))
                    My_Pet.AttrStrApt = petAttribPacket.StrPerception;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_CONPERCEPTION))
                    My_Pet.AttrConApt = petAttribPacket.ConPerception;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEXPERCEPTION))
                    My_Pet.AttrDexApt = petAttribPacket.DexPerception;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPRPERCEPTION))
                    My_Pet.AttrSprApt = petAttribPacket.SprPerception;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_INTPERCEPTION))
                    My_Pet.AttrIntApt = petAttribPacket.IntPerception;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_GENGU))
                    My_Pet.Basic = petAttribPacket.GenGu;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_STR))
                    My_Pet.AttrStr = petAttribPacket.Str;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_CON))
                    My_Pet.AttrCon = petAttribPacket.Con;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEX))
                    My_Pet.AttrDex = petAttribPacket.Dex;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPR))
                    My_Pet.AttrSpr = petAttribPacket.Spr;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_INT))
                    My_Pet.AttrInt = petAttribPacket.Int;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_STR))					//力量
                    My_Pet.AttrStrBring = petAttribPacket.StrBring;
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_SPR))
                    My_Pet.AttrSprBring = (petAttribPacket.SprBring);
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_CON))
                    My_Pet.AttrConBring = (petAttribPacket.ConBring);
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_INT))
                    My_Pet.AttrIntBring = (petAttribPacket.IntBring);
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_DEX))
                    My_Pet.AttrDexBring = (petAttribPacket.DexBring);

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_POINT_REMAIN))
                    My_Pet.Pot = petAttribPacket.RemainPoint;

                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_0))
                {
                    //My_Pet.m_aSkill[0] = petAttribPacket.GetSkill(0);
					LogManager.Log("Pet_SetSkill " + nPet_num + " " + petAttribPacket.PetSkill[0]);
                    CDataPool.Instance.Pet_SetSkill(nPet_num, 0, ref petAttribPacket.PetSkill[0]);

                }
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_1))
                {
                    //My_Pet.m_aSkill[0] = petAttribPacket.GetSkill(0);
					LogManager.Log("Pet_SetSkill " + nPet_num + " " + petAttribPacket.PetSkill[1]);
                    CDataPool.Instance.Pet_SetSkill(nPet_num, 1, ref petAttribPacket.PetSkill[1]);

                }
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_2))
                {
                    //My_Pet.m_aSkill[0] = petAttribPacket.GetSkill(0);
                    LogManager.Log("Pet_SetSkill " + nPet_num + " " + petAttribPacket.PetSkill[2]);
                    CDataPool.Instance.Pet_SetSkill(nPet_num, 2, ref petAttribPacket.PetSkill[2]);

                }
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_3))
                {
                    //My_Pet.m_aSkill[0] = petAttribPacket.GetSkill(0);
                    LogManager.Log("Pet_SetSkill " + nPet_num + " " + petAttribPacket.PetSkill[3]);
                    CDataPool.Instance.Pet_SetSkill(nPet_num, 3, ref petAttribPacket.PetSkill[3]);

                }
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_4))
                {
                    //My_Pet.m_aSkill[0] = petAttribPacket.GetSkill(0);
                    LogManager.Log("Pet_SetSkill " + nPet_num + " " + petAttribPacket.PetSkill[4]);
                    CDataPool.Instance.Pet_SetSkill(nPet_num, 4, ref petAttribPacket.PetSkill[4]);

                }
                if (petAttribPacket.IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_5))
                {
                    //My_Pet.m_aSkill[0] = petAttribPacket.GetSkill(0);
                    LogManager.Log("Pet_SetSkill " + nPet_num + " " + petAttribPacket.PetSkill[5]);
                    CDataPool.Instance.Pet_SetSkill(nPet_num, 5, ref petAttribPacket.PetSkill[5]);
                }

                 CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PET_PAGE);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAILATTRIB_PET;
        }
    }
}
