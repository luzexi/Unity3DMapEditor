using System;
using System.Collections.Generic;
using DBSystem;
using Network;
using Network.Packets;
/// <summary>
/// 玩家宠物数据，包括自身携带宠物，查看目标宠物，宠物学习，源自:CDataPool
/// </summary>
public class PetDataPool
{
   List<SDATA_PET> m_listPet = new List<SDATA_PET>();
   List<PET_SKILL> m_petSkillStudyList = new List<PET_SKILL>();
   SDATA_PET m_TargetPet;
   _PET_PLACARD_ITEM[] m_aPetPlacardItems = new _PET_PLACARD_ITEM[GAMEDEFINE.MAX_PETPLACARD_LIST_ITEM_NUM];
   int m_nPetPlacardItemCount = 0;

    ////用于Pet UI显示的宠物对象
  //  CObject_PlayerNPC m_pPetAvatar;

    ////用于TargetPet UI显示的宠物对象
//    CObject_PlayerNPC m_pTargetPetAvatar;

    ////用于PetStudySkill UI显示的宠物对象
  //  CObject_PlayerNPC m_pPetStudySkillAvatar;

    public PetDataPool()
    {
        for(int i = 0; i < (int)PET_INDEX.PET_INDEX_SELF_NUMBERS; i++)
        {
            m_listPet.Add( new SDATA_PET() ); 
        }
    }

    public int Pet_GetPetByGuid(PET_GUID_t PetGuid)
    {
        for (int i = 0; i < GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++)
        {
            if (m_listPet[i].GUID == PetGuid)
            {
                return i;
            }
        }
        return -1;
    }

    public SDATA_PET				Pet_GetPet(int nIndex)
    {
        if (nIndex >= 0 && nIndex < (int)PET_INDEX.PET_INDEX_SELF_NUMBERS)
            return m_listPet[nIndex];
        else
            return null;
    }
    public SDATA_PET Pet_GetPet(PET_GUID_t guidPet)
    {
   
        foreach (SDATA_PET pet in m_listPet)
        {
            if (pet.GUID == guidPet)
                return pet;
        }
        return null;
    }
    public void Pet_ClearPet(int nIndex)
    {
        if (nIndex >= 0 && nIndex < GAMEDEFINE.HUMAN_PET_MAX_COUNT)
        {
            m_listPet[nIndex].IsPresent = -1;
            m_listPet[nIndex].CleanUp();
        }
    }
    public int							Pet_GetCount()
    {
        int count = 0;
        for (int i = 0; i < (int)PET_INDEX.PET_INDEX_SELF_NUMBERS; i++)
        {
            if ((MacroDefine.INVALID_ID == m_listPet[i].IsPresent) || (m_listPet[i].DataID < 0)) continue;
            count++;
        }
        return count;
    }

    public SDATA_PET Pet_GetValidPet(int index)
    {
        int petCount = 0;
        for (int i = 0; i < GAMEDEFINE.HUMAN_PET_MAX_COUNT; i++)
        {
            SDATA_PET curPet = Pet_GetPet(i);
            if (curPet != null && MacroDefine.INVALID_ID != curPet.IsPresent)
            {
                if (petCount == index)
                {
                    return curPet;
                }
				petCount++;
            }
        }
        return null;
    }

    public CObject_Item GetEquiptItem(PET_GUID_t guid,PET_EQUIP ptEquipt)
    {
        SDATA_PET curPet = Pet_GetPet(guid);
        if (curPet == null) return null;
        if((int)ptEquipt < 0||(int)ptEquipt >= (int) PET_EQUIP.PEQUIP_NUMBER)return null;
        return curPet.Equipts[(int)ptEquipt];
    }

    public void SetEquiptItem(PET_GUID_t guid, PET_EQUIP ptEquipt, CObject_Item pEquiptItem, bool bClearOld)
    {
        SDATA_PET curPet = Pet_GetPet(guid);
        if (curPet == null) return;
        if ((int)ptEquipt < 0 || (int)ptEquipt >= (int)PET_EQUIP.PEQUIP_NUMBER) return;
        if (curPet.Equipts[(int)ptEquipt] != null && bClearOld)
        {
            ObjectSystem.Instance.DestroyItem(curPet.Equipts[(int)ptEquipt]);
        }

        if (pEquiptItem != null)
        {
            PET_EQUIP pointEquip = (PET_EQUIP)((CObject_Item_Equip)pEquiptItem).GetItemType();
            if (pointEquip != ptEquipt) return;
            // 设置所属
            pEquiptItem.TypeOwner = ITEM_OWNER.IO_PET_EQUIPT;
            // 设置装备在action item 中的位置.
            pEquiptItem.PosIndex = (short)ptEquipt;
            curPet.Equipts[(int)ptEquipt] = pEquiptItem;
        }
        else
        {
            // 在装备点清空数据
            curPet.Equipts[(int)ptEquipt] = null;
        }
    }

    public int Pet_GetPetFoodType(PET_GUID_t guidPet)	//获得宠物的食物类型
    {
        int nFoodType = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_INVALID;
	    //获取食物类型
	    if(Pet_GetPet(guidPet)!= null)
	    {
		    _DBC_PET_EX_ATT pLine = CDataBaseSystem.Instance.GetDataBase<_DBC_PET_EX_ATT>((int)DataBaseStruct.DBC_PET_EX_ATT).Search_Index_EQU(Pet_GetPet(guidPet).DataID);
		    if(pLine != null)
		    {
			    nFoodType = pLine.m_FoodType;
		    }
	    }

	    return nFoodType;
    }

    public void	Pet_GetPetFoodTypeMinMax(int nFoodType, ref int idMin, ref int idMax)	//获得宠物相应食物类型得idTable限制
    {
        
        switch ((ENUM_PET_FOOD_TYPE)nFoodType)
	    {
            case ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_MEAT:
                {
                    idMin = GAMEITEMDEFINE.ITEM_MEAT_MEDICINE;
                    idMax = GAMEITEMDEFINE.ITEM_GRASS_MEDICINE - 1;
                }
                break;
            case ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_GRASS:
                {
                    idMin = GAMEITEMDEFINE.ITEM_GRASS_MEDICINE;
                    idMax = GAMEITEMDEFINE.ITEM_WORM_MEDICINE - 1;
                }
                break;
            case ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_WORM:
                {
                    idMin = GAMEITEMDEFINE.ITEM_WORM_MEDICINE;
                    idMax = GAMEITEMDEFINE.ITEM_PADDY_MEDICINE - 1;
                }
                break;
            case ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_PADDY:
                {
                    idMin = GAMEITEMDEFINE.ITEM_PADDY_MEDICINE;
                    idMax = GAMEITEMDEFINE.ITEM_PET_FEED_MEDICINE_MAX - 1;
                }
                break;
            default:
                {
                    idMin = -1;
                    idMax = -1;
                }
                break;
        }
    }

    public CObject_Item				Pet_GetLowestLevel_Food_From_Package(PET_GUID_t pg, ref int idxPackage)	//从背包获得喂养所需最低级的食物
    {
        CObject_Item pFind = null;
	    idxPackage = -1;

	    //获取食物类型
	    int idTableMin = -1, idTableMax = -1, mod = 1000, modold = 0;
	    int nFoodType = CDataPool.Instance.Pet_GetPetFoodType(pg);

	    int k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_MEAT;
	    //根据食物类型来判断循环范围
	    if(nFoodType >=1000)
	    {
		    k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_MEAT;
		    mod = 1000;
	    }
	    else if(nFoodType >=100)
	    {
		    k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_GRASS;
		    mod = 100;
	    }
	    else if(nFoodType >=10)
	    {
		    k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_WORM;
		    mod = 10;
	    }
	    else
	    {
            k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_PADDY;
		    mod = 1;
	    }

	    //开始查找物品
	    for(; k < (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_NUMBER; ++k)
	    {
		    if(0 == mod) break; //keep loop safe.
		    //calculate food type enum
		    int q = (nFoodType-modold)/mod;
		    idTableMin = -1;
            idTableMax = -1;
		    if(1 == q) Pet_GetPetFoodTypeMinMax(k, ref idTableMin,ref idTableMax);
		    if(idTableMin < 0 || idTableMax < 0) continue;

		    //检查背包里等级最低的食物
		    for(int i = GAMEDEFINE.BASE_CONTAINER_OFFSET; i < GAMEDEFINE.MAX_BAG_SIZE; ++i)
		    {
                CObject_Item pItem = (CObject_Item)CDataPool.Instance.UserBag_GetItemByIndex(i);
			    if(pItem != null)
			    {
				    int id = pItem.GetIdTable();
				    //id是否是食物
				    if(id > idTableMax || id < idTableMin) continue;
				    //道具的使用等级是否合法
				    if(pItem.GetNeedLevel() > Pet_GetPet(pg).Level) continue;

				    if(pFind != null)
				    {
					    CObject_Item pOldFind = pFind;
					    pFind = (pItem.GetNeedLevel() > pFind.GetNeedLevel())?pFind:pItem;
					    if(pFind != pOldFind ) idxPackage = i;
				    }
				    else
				    {
					    pFind = pItem;
					    idxPackage = i;
				    }
			    }// end of if(pItem)
		    }
		    //change mod
		    modold += mod;
		    mod /= 10;
	    }
	    return pFind;
    }

    public CObject_Item Pet_GetLowestLevel_Dome_From_Package(PET_GUID_t pg, ref int idxPackage)	//从背包获得驯养所需最低级的食物
    {
        CObject_Item pFind = null;
	    idxPackage= -1;

	    //获取食物类型
	    int idTableMin = -1, idTableMax = -1, mod = 1000, modold = 0;
	    int nFoodType = CDataPool.Instance.Pet_GetPetFoodType(pg);

	    int k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_MEAT;
	    //根据食物类型来判断循环范围
	    if(nFoodType >=1000)
	    {
		    k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_MEAT;
		    mod = 1000;
	    }
	    else if(nFoodType >=100)
	    {
		    k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_GRASS;
		    mod = 100;
	    }
	    else if(nFoodType >=10)
	    {
		    k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_WORM;
		    mod = 10;
	    }
	    else
	    {
            k = (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_PADDY;
		    mod = 1;
	    }

	    //开始查找物品
	    for(; k < (int)ENUM_PET_FOOD_TYPE.PET_FOOD_TYPE_NUMBER; ++k)
	    {
		    if(0 == mod) break; //keep loop safe.
		    //calculate food type enum
		    int q = (nFoodType-modold)/mod;
		    idTableMin = -1; idTableMax = -1;
		    if(1 == q) Pet_GetPetFoodTypeMinMax(k, ref idTableMin, ref idTableMax);
		    if(idTableMin < 0 || idTableMax < 0) continue;

		    //检查背包里等级最低的食物
		    for(int i = GAMEDEFINE.BASE_CONTAINER_OFFSET; i < GAMEDEFINE.MAX_BAG_SIZE; ++i)
		    {
                CObject_Item pItem = (CObject_Item)CDataPool.Instance.UserBag_GetItemByIndex(i);
			    if(pItem != null)
			    {
				    int id = pItem.GetIdTable();
				    //id是否是食物
				    if(id > idTableMax || id < idTableMin) continue;
				    //道具的使用等级是否合法
				    if(pItem.GetNeedLevel() > Pet_GetPet(pg).Level) continue;

				    if(pFind != null)
				    {
					    CObject_Item pOldFind = pFind;
					    pFind = (pItem.GetNeedLevel() > pFind.GetNeedLevel())?pFind:pItem;
					    if(pFind != pOldFind) idxPackage = i;
				    }
				    else
				    {
					    pFind = pItem;
					    idxPackage = i;
				    }
			    }// end of if(pItem)
		    }

		    //change mod
		    modold += mod;
		    mod /= 10;
	    }

	    return pFind;
    }

    public SDATA_PET			TargetPet_GetPet() { return m_TargetPet; }
    public void 				TargetPet_Clear() { m_TargetPet.IsPresent = -1; m_TargetPet.CleanUp();}

    public void TargetPet_SetPetModel(int nRaceID)
    {
       // m_pTargetPetAvatar.GetCharacterData().Set_RaceID(nRaceID);
       // m_pTargetPetAvatar.SetFaceDir(0.0f);
    }

    public PET_SKILL TargetPet_GetSkill(int nSkillIndex)
    {
        PET_SKILL pPetSkill = m_TargetPet[nSkillIndex];

        if (pPetSkill == null || (pPetSkill.m_pDefine==null))
            return null;
        return pPetSkill;
    }
    public void TargetPet_SetSkill(int nSkillIndex, ref _OWN_SKILL Skill)
    {
        PET_SKILL theSkill = m_TargetPet[nSkillIndex];
        int nIndex = (int)PET_INDEX.TARGETPET_INDEX;

        _DBC_SKILL_DATA pDefine = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).Search_Index_EQU(Skill.m_nSkillID);

        if (theSkill == null)
        {
            if (pDefine != null)
            {
                PET_SKILL newSkill = new PET_SKILL();

                newSkill.m_pDefine = pDefine;
                newSkill.m_nPetNum = nIndex;
                newSkill.m_nPosIndex = nSkillIndex;
                newSkill.m_bCanUse = true;

                m_TargetPet[nSkillIndex] = newSkill;
            }
        }
        else
        {
            if (pDefine != null)
            {
                theSkill.m_pDefine = pDefine;
                theSkill.m_nPetNum = nIndex;
                theSkill.m_bCanUse = true;
            }
            else
            {
                m_TargetPet[nSkillIndex] = null;
            }
        }
        //通知ActionSystem。
        CActionSystem.Instance.UserTargetPetSkill_Update();
    }
    public void TargetPet_SetSkill(int nSkillIndex, PET_SKILL pPetSkillData, bool bClearOld)
    {
        PET_SKILL theSkill = m_TargetPet[nSkillIndex];

        if (pPetSkillData == null && theSkill != null && bClearOld)
        {
            m_TargetPet[nSkillIndex] = null;
            m_TargetPet[nSkillIndex] = pPetSkillData;
            //通知ActionSystem。
            CActionSystem.Instance.CleanInvalidAction();
        }
    }

    public void PetStudySkill_SetPetModel(int nRaceID)
    {
        //m_pPetStudySkillAvatar.GetCharacterData().Set_RaceID(nRaceID);
      //  m_pPetStudySkillAvatar.SetFaceDir(0.0f);
    }

    public void Pet_SetSkill(int nIndex, int nSkillIndex, ref _OWN_SKILL Skill)
    {
        if (nIndex < 0 || nIndex >= (int)PET_INDEX.PET_INDEX_SELF_NUMBERS)
            return;
        PET_SKILL theSkill = (m_listPet[nIndex])[nSkillIndex];
        _DBC_SKILL_DATA pDefine = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).Search_Index_EQU(Skill.m_nSkillID);

        LogManager.LogWarning("Pet_SetSkill index " + nIndex + " SkillIndex " + nSkillIndex + " SkillID " + Skill.m_nSkillID);
        if (theSkill == null)
        {
            if (pDefine != null)
            {
                PET_SKILL newSkill = new PET_SKILL();

                newSkill.m_pDefine = pDefine;
                newSkill.m_nPetNum = nIndex;
                newSkill.m_nPosIndex = nSkillIndex;
                newSkill.m_bCanUse = true;

                (m_listPet[nIndex])[nSkillIndex] = newSkill;
            }
        }
        else
        {
            if (pDefine != null)
            {
                theSkill.m_pDefine = pDefine;
                theSkill.m_nPetNum = nIndex;
                theSkill.m_nPosIndex = nSkillIndex;
                theSkill.m_bCanUse = true;
            }
            else
            {
                (m_listPet[nIndex])[nSkillIndex] = null;

            }
        }
        //通知ActionSystem。
        CActionSystem.Instance.UserPetSkill_Update(nIndex);
    }
    public void						Pet_SetSkill(int nIndex, int nSkillIndex, PET_SKILL pPetSkillData, bool bClearOld )
    {
        if ((m_listPet[nIndex])[nSkillIndex] != null && bClearOld)
        {
            (m_listPet[nIndex])[nSkillIndex] = null;
        }
        (m_listPet[nIndex])[nSkillIndex] = pPetSkillData;

        ////通知ActionSystem。
        CActionSystem.Instance.UserPetSkill_Update(nIndex);
    }
    public PET_SKILL Pet_GetSkill(int nIndex, int nSkillIndex)
    {
        if (nIndex < 0 || nIndex >= GAMEDEFINE.HUMAN_PET_MAX_COUNT)
            return null;

        if (nSkillIndex < 0 || nSkillIndex >= GAMEDEFINE.MAX_PET_SKILL_COUNT)
            return null;

        PET_SKILL pPetSkill = null;
		for(int i = 0; i < GAMEDEFINE.MAX_PET_SKILL_COUNT;i++)
		{
			pPetSkill = (m_listPet[nIndex])[i];
			if(pPetSkill != null && pPetSkill.m_pDefine != null && pPetSkill.m_nPosIndex == nSkillIndex)
				return pPetSkill;
		}

        if (pPetSkill == null || pPetSkill.m_pDefine == null)
            return null;
        return pPetSkill;
    }

    public void Pet_SetPetModel(int nIndex)
    {
        //m_pPetAvatar.GetCharacterData().Set_RaceID(m_listPet[nIndex].DataID);
       // m_pPetAvatar.SetFaceDir(0.0f);
    }

    public bool Pet_SendUseSkillMessage(int petIndex,short idSkill, int idTargetObj, float fTargetX, float fTargetZ)
    {

        SDATA_PET pPetData = Pet_GetPet(petIndex);

	    int nPetCount = CDataPool.Instance.Pet_GetCount();
	     if ( pPetData != null )
	    {
		    CObject_PlayerNPC pPet = null;
		    if(pPetData.idServer != uint.MaxValue)
		    {
			    pPet = CObjectManager.Instance.FindServerObject((int)pPetData.idServer) as CObject_PlayerNPC;
		    }
		    if(pPet != null)
		    {
			    CGCharUseSkill msg = NetManager.GetNetManager().CreatePacket((int) PACKET_DEFINE.PACKET_CG_CHARUSESKILL) as CGCharUseSkill;
			    msg.ObjectID = (int)pPetData.idServer ;
			    msg.SkillDataID =  idSkill * 100;
			    msg.TargetID =  idTargetObj ;
                WORLD_POS pos = new WORLD_POS();
                pos.m_fX = fTargetX;
                pos.m_fZ = fTargetZ;
                msg.PosTarget = pos;
               // msg.PosTarget.m_fZ = fTargetZ;
			    msg.Dir =  -1 ;
			    msg.GuidTarget =  -1;
                LogManager.LogWarning("Pet_SendUseSkillMessage");
			    NetManager.GetNetManager().SendPacket( msg );
		    }
	    }
	    return true;    
    }

    //宠物五行属性技能学习数据访问
    public void PetSkillStudy_MenPaiList_AddSkill(int skillId)
    {
        PET_SKILL sk = new PET_SKILL();

	    sk.m_bCanUse = false;
	    sk.m_nPetNum = GAMEDEFINE.MENPAI_PETSKILLSTUDY_PETNUM;
	    sk.m_nPosIndex = m_petSkillStudyList.Count;
        _DBC_SKILL_DATA pDefine = CSkillDataMgr.Instance.GetSkillData((uint)skillId);
	    if(pDefine != null)
	    {
		    sk.m_pDefine = pDefine;
		    m_petSkillStudyList.Add(sk);
	    }
    }

    public void PetSkillStudy_MenPaiList_Clear()
    {
        m_petSkillStudyList.Clear();
    }

    public PET_SKILL PetSkillStudy_MenPaiList_Get(int idx)	//idx 是 m_petSkillStudyList 的索引
    {
        if (m_petSkillStudyList.Count == 0)
            return null;

        if (idx < 0 || idx >= m_petSkillStudyList.Count)
            return null;
        else
            return m_petSkillStudyList[idx];
    }

    public PET_SKILL PetSkillStudy_MenPaiList_Get_BySkillId(int sdx) //sdx 是 PET_SKILL中的 m_pDefine->m_nID
    {
        if (sdx < 0)
        {
            return null;
        }
        for (int i = 0; i < m_petSkillStudyList.Count; ++i)
        {
            if (m_petSkillStudyList[i].m_pDefine.m_nID == sdx)
                return m_petSkillStudyList[i];
        }
        return null;
    }


    ////---------------------------------------------
    ////宠物公告板
    ////---------------------------------------------
    public int  PetPlacard_GetItemCount()
    {
        return m_nPetPlacardItemCount;
    }

    public _PET_PLACARD_ITEM	PetPlacard_GetItem(int nIndex)
    {
        if (nIndex < 0 || nIndex >= m_nPetPlacardItemCount)
        {
            throw new ArgumentOutOfRangeException("PetPlacard_GetItem Index is not correct."+ nIndex);
        }
        return m_aPetPlacardItems[nIndex];
    }

    public void PetPlacard_AddItem(_PET_PLACARD_ITEM pItem)
    {
        if (m_nPetPlacardItemCount < GAMEDEFINE.MAX_PETPLACARD_LIST_ITEM_NUM)
        {
            m_aPetPlacardItems[m_nPetPlacardItemCount++] = pItem;
        }
    }

    public void PetPlacard_CleanUp()
    {
        for (int i = 0; i < m_nPetPlacardItemCount; ++i)
        {
            m_aPetPlacardItems[i].CleanUp();
        }
        m_nPetPlacardItemCount = 0;
    }

    public void Pet_CopyToTarget(SDATA_PET pSourcePet) 
    {
	    SDATA_PET pTargetPet = CDataPool.Instance.TargetPet_GetPet();
	    pTargetPet.CleanUp();
	    CActionSystem.Instance.CleanInvalidAction();
	    pTargetPet.IsPresent		= pSourcePet.IsPresent;
	    pTargetPet.GUID				= pSourcePet.GUID;
	    pTargetPet.idServer			= pSourcePet.idServer;
	    pTargetPet.DataID			= pSourcePet.DataID;
	    pTargetPet.AIType			= pSourcePet.AIType;
	    pTargetPet.Name			    = pSourcePet.Name;
	    pTargetPet.Level			= pSourcePet.Level;
	    pTargetPet.Exp				= pSourcePet.Exp;
	    pTargetPet.HP				= pSourcePet.HP;
	    pTargetPet.HPMax			= pSourcePet.HPMax;
	    pTargetPet.Age				= pSourcePet.Age;
	    pTargetPet.EraCount			= pSourcePet.EraCount;
	    pTargetPet.Happiness		= pSourcePet.Happiness;
	    pTargetPet.SpouseGUID		= pSourcePet.SpouseGUID;
	    pTargetPet.ModelID			= pSourcePet.ModelID;
	    pTargetPet.MountID			= pSourcePet.MountID;
	    pTargetPet.AttPhysics		= pSourcePet.AttPhysics;
	    pTargetPet.AttMagic			= pSourcePet.AttMagic;
	    pTargetPet.DefPhysics		= pSourcePet.DefPhysics;
	    pTargetPet.DefMagic			= pSourcePet.DefMagic;
	    pTargetPet.Hit				= pSourcePet.Hit;
	    pTargetPet.Miss				= pSourcePet.Miss;
	    pTargetPet.Critical			= pSourcePet.Critical;
	    pTargetPet.AttrStrApt		= pSourcePet.AttrStrApt;
	    pTargetPet.AttrConApt		= pSourcePet.AttrConApt;
	    pTargetPet.AttrDexApt		= pSourcePet.AttrDexApt;
	    pTargetPet.AttrSprApt		= pSourcePet.AttrSprApt;
	    pTargetPet.AttrIntApt		= pSourcePet.AttrIntApt;
	    pTargetPet.AttrStr			= pSourcePet.AttrStr;
	    pTargetPet.AttrCon			= pSourcePet.AttrCon;
	    pTargetPet.AttrDex			= pSourcePet.AttrDex;
	    pTargetPet.AttrSpr			= pSourcePet.AttrSpr;
	    pTargetPet.AttrInt			= pSourcePet.AttrInt;
	    pTargetPet.Basic			= pSourcePet.Basic;
	    pTargetPet.Pot				= pSourcePet.Pot;
        pTargetPet.AttrSprBring     = pSourcePet.AttrSprBring;
        pTargetPet.AttrIntBring     = pSourcePet.AttrIntBring;
        pTargetPet.AttrStrBring     = pSourcePet.AttrStrBring;
        pTargetPet.AttrDexBring     = pSourcePet.AttrDexBring;
        pTargetPet.AttrConBring     = pSourcePet.AttrConBring;

	    PET_SKILL pDestPetSkill = null;
	    PET_SKILL pSourcePetSkill = null;

	    for(int i = 0; i<(int)ENUM_PET_SKILL_INDEX.PET_SKILL_INDEX_NUMBERS; i++ )
	    {
		    pSourcePetSkill = pSourcePet[i];
		    if( pSourcePetSkill != null)
		    {
			    pDestPetSkill = pTargetPet[i];
			    if(pDestPetSkill == null)
			    {
				    pTargetPet[i] = new PET_SKILL();
				    pDestPetSkill = pTargetPet[i];
			    }

			    pDestPetSkill.m_bCanUse	= pSourcePetSkill.m_bCanUse;
			    pDestPetSkill.m_nPetNum	= (int)PET_INDEX.TARGETPET_INDEX;//pPetSourceData->m_aSkill[i]->m_nPetNum;
			    pDestPetSkill.m_nPosIndex	= i;//pPetSourceData->m_aSkill[i]->m_nPosIndex;
			    pDestPetSkill.m_pDefine	= pSourcePetSkill.m_pDefine;
		    }
	    }

	    CActionSystem.Instance.UserTargetPetSkill_Update();
    }
}