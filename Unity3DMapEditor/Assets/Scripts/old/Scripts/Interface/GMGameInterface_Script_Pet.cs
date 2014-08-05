using System;

using Network;
using Network.Packets;
//UI使用的宠物数据接口
namespace Interface
{
    public class PET_INFO
    {
        public string petName;
        public string petName2;
        public int sex;
        public int petOn;
        public string szName;
    };
    public struct PET_TYPE_INFO
    {
        public int nGen;
        public string strTypeName;
    };
    public struct PET_EXP
    {
        public int nExp;
        public int nMaxExp;
    }
    public class Pet
    {
        static Pet _instance;
        public static Pet Instance
        {
            get {
                if (_instance == null)
                    _instance = new Pet();
                return _instance;
            }
        }
        //取得当前宠物是否存在

        public int GetPet_Count()
        {
            return CDataPool.Instance.Pet_GetCount();
        }
        public PET_INFO GetPetList_Appoint(int nPetNum)
        {
            SDATA_PET My_Pet;
            PET_INFO petInfo = new PET_INFO();
            int count = 0;
            for (int i = 0; i < (int)ENUM_PET_SKILL_INDEX.PET_SKILL_INDEX_NUMBERS; i++)
            {

                My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
                if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
                    continue;

                if (count == nPetNum)
                {
                    petInfo.petName = My_Pet.Name;

                    //if(CDataPool.Instance.MyStallBox_IsPetOnStall(My_Pet->m_GUID))
                    //{
                    //    petInfo.petOn = 1;
                    //}
                    //else
                    //{
                    //    petInfo.petOn = 0;
                    //}
                    return petInfo;
                }
                count++;
            }
            return petInfo;
        }
        public void Go_Fight(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetValidPet(nPetNum);//CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet Go_Fight parameter error");

            }
            // 如果自己正在摆摊，不能出战
            if(true == CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_IsInStall())
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"你正在摆摊……");
                return;
            }
            //向服务器发送
            CGManipulatePet Msg = NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_MANIPULATEPET) as CGManipulatePet;
            Msg.PetGUID = My_Pet.GUID;
            Msg.Type = (int) ENUM_MANIPULATE_TYPE.MANIPULATE_CREATEPET;
            NetManager.GetNetManager().SendPacket(Msg);
        }
        public void Go_Relax(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetValidPet(nPetNum);//CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet Go_Relax parameter error");
            }
            //向服务器发送
            CGManipulatePet Msg = NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_MANIPULATEPET) as CGManipulatePet;
            Msg.PetGUID = My_Pet.GUID;
            Msg.Type = (int)ENUM_MANIPULATE_TYPE.MANIPULATE_DELETEPET;
            NetManager.GetNetManager().SendPacket(Msg);
        }
        public void Go_Free(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetValidPet(nPetNum);//CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet Go_Free parameter error");
            }
            //if( CGameProcedure::s_pUISystem->IsWindowShow("ProgressBar") )
            //{
            //    STRING strMsg = "你正在进行其他操作，目前不能放生宠物";
            //    ADDNEWDEBUGMSG(strMsg);
            //    return;

            //}
            // 如果自己正在摆摊，不能放生
            //if(TRUE == CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_IsInStall())
            //{
            //    CEventSystem::GetMe()->PushEvent(GE_INFO_SELF,"你正在摆摊……");
            //    return;
            //}

            for(int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER;i++)
            {
                if (My_Pet.Equipts[i] != null)
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "宠物身上有装备不能放生");
                    return;
                }
            }

            //向服务器发送
            CGManipulatePet Msg = NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_MANIPULATEPET) as CGManipulatePet;
            Msg.PetGUID = My_Pet.GUID;
            Msg.Type = (int) ENUM_MANIPULATE_TYPE.MANIPULATE_FREEPET;
            NetManager.GetNetManager().SendPacket(Msg);

        }
        //重置宠物技能
        public void ResetPetSkill(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetValidPet(nPetNum);//CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet Reset Skill parameter error");
            }
           
            //向服务器发送
            CGManipulatePet Msg = NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_MANIPULATEPET) as CGManipulatePet;
            Msg.PetGUID = My_Pet.GUID;
            Msg.Type = (int)ENUM_MANIPULATE_TYPE.MANIPULATE_RESETSKILL;
            NetManager.GetNetManager().SendPacket(Msg);

        }
        public bool IsFighting(int nPetIndex)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetValidPet(nPetIndex);//CDataPool.Instance.Pet_GetPet(nPetIndex);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet GetIsFighting parameter error");
            }
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().isFightPet(My_Pet.GUID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Feed(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet Feed parameter error");
            }

            int idxPackage = -1;
            CObject_Item  pFind = (CObject_Item)CDataPool.Instance.Pet_GetLowestLevel_Food_From_Package(My_Pet.GUID,ref idxPackage);
            if(pFind != null || idxPackage < 0) 
            {
               // STRING strMsg = "你没有适合喂养的宠粮";//COLORMSGFUNC("GMGameInterface_Script_Pet_NoFeedFood");
               // ADDNEWDEBUGMSG(strMsg);
                return;
            }

            CGUseItem pi = new CGUseItem();
            pi.BagIndex = (byte)idxPackage;
            pi.TargetPetGUID = My_Pet.GUID;
            NetManager.GetNetManager().SendPacket(pi);
        }
        public void Dome(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet Dome parameter error");
            }

            int idxPackage = -1;
            CObject_Item pFind = (CObject_Item)CDataPool.Instance.Pet_GetLowestLevel_Dome_From_Package(My_Pet.GUID,ref idxPackage);

            if(My_Pet.Happiness == 100)
            {
            //    STRING strMsg = "你的宠物无需驯养";//COLORMSGFUNC("GMGameInterface_Script_Pet_NoNeedDemo");
            //    ADDNEWDEBUGMSG(strMsg);
            //    return;
            }

            if(pFind != null || idxPackage < 0)
            {
            //    STRING strMsg = "你没有驯养道具";//COLORMSGFUNC("GMGameInterface_Script_Pet_NoDemoFood");
            //    ADDNEWDEBUGMSG(strMsg);
            //    return;
            }

            CGUseItem pi = new CGUseItem();
            pi.BagIndex = (byte)idxPackage;
            pi.TargetPetGUID = My_Pet.GUID;
            NetManager.GetNetManager().SendPacket(pi);
        }

        public PET_INFO GetName(int nPetNum)
        {
            PET_INFO petInfo = new PET_INFO();
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet GetName parameter error");

            }
            petInfo.szName = My_Pet.Name;

            //DBC_DEFINEHANDLE(s_pMonsterAttr, DBC_CREATURE_ATT);
            //const _DBC_CREATURE_ATT* pPet = (const _DBC_CREATURE_ATT*)s_pMonsterAttr->Search_Index_EQU(My_Pet->m_nDataID);
            //if(pPet)
            //{
            //    petInfo.petName = pPet->pName;
            //}
            //else
            //{
            //    petInfo.petName = "未知宠";
            //}
            return petInfo;
        }
        public PET_TYPE_INFO GetPetTypeName(int nPetNum)
        {
            PET_TYPE_INFO petTypeInfo;
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new Exception("LUA:Pet GetPetTypeName parameter error");
            }
            petTypeInfo.nGen = this[nPetNum].EraCount;
            petTypeInfo.strTypeName = GetName(nPetNum).petName;

            return petTypeInfo;
        }
        public PET_INFO Other_GetName(int nPetNum)
        {
            return GetName(nPetNum);
        }
        public PET_TYPE_INFO Other_GetPetTypeName(int nPetNum)
        {
            return GetPetTypeName(nPetNum);
        }

        public SDATA_PET this[int nIndex]
        {
            get
            {
                SDATA_PET pet = CDataPool.Instance.Pet_GetPet(nIndex);
                if (pet == null || pet.IsPresent == MacroDefine.INVALID_ID)
                {
                    throw new NullReferenceException("Pet is not present :" + nIndex);
                }
                return pet;
            }
        }
        public PET_INFO GetID(int nPetNum)
        {
            PET_INFO petInfo = new PET_INFO();
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new NullReferenceException("LUA:Pet GetID parameter error");
            }

            petInfo.petName = string.Format("%.8X", My_Pet.GUID.m_uHighSelection);

            petInfo.petName2 = string.Format("%.8X", My_Pet.GUID.m_uLowSelection);

            int sex = (int)My_Pet.GUID.m_uLowSelection % 2;
            petInfo.sex = sex;

            return petInfo;
        }
        public string GetConsort(int nPetNum)
        {
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new NullReferenceException("LUA:Pet GetConsort parameter error");
            }

            return string.Format("%.8X", My_Pet.SpouseGUID.m_uLowSelection);
        }

        public PET_EXP GetExp(int nPetNum)
        {
            PET_EXP petExp;
            SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPetNum);
            if ((My_Pet == null) || (MacroDefine.INVALID_ID == My_Pet.IsPresent))
            {
                throw new NullReferenceException("LUA:Pet GetExp parameter error");
            }

            //DBC_DEFINEHANDLE(s_pPetLevelup, DBC_PET_LEVELUP);
            //const _DBC_PET_LEVELUP* pPetExp = (const _DBC_PET_LEVELUP*)s_pPetLevelup->Search_Index_EQU(My_Pet->m_nLevel);

            petExp.nExp = My_Pet.Exp;
            petExp.nMaxExp = -1;

            //if(pPetExp)
            //{
            //    petExp.nMaxExp = pPetExp->nExp;
            //}
            //else
            //{
            //    petExp.nMaxExp = INVALID_ID;
            //}
            return petExp;
        }


        public void Add_Attribute(int nPetNum, int nStr, int nInt, int nDex, int nPF, int nSta)
        {

        }
        //public void	Change_Name(INT nPetNum,const char* szName);

        //public bool	GetIsFighting(INT nPetNum);
        //public void	SetModel(INT nPetNum);
        //public int	GetDeathWarrant(INT nPetNum);

        ////yangjun add for petskillstudy
        //VOID	SetSkillStudyModel(INT nPetNum);
        //VOID	ShowTargetPet(INT nPetNum);
        //INT		SkillStudy_Do(INT uitype,INT lid,string value3 = "",string value4 = "");
        //VOID	SkillStudyUnlock(INT pid);
        //VOID	SkillStudy_MenPaiSkill_Created(INT sk1,INT sk2,INT sk3);
        //VOID	SkillStudy_MenPaiSkill_Clear();
        //Lua_PET_GUID	GetGUID(INT nPetNum);
        //VOID	ShowPetList(INT nShow);

        ////yangjun add for petcontexmenu
        //VOID	ShowMyPetContexMenu();
        //VOID	HandlePetMenuItem(string strOrder,INT petIdx = -1);

        ////yangjun add for petImpact
        //string	GetPetPortraitByIndex(INT petNum);
        //INT		GetPetImpactNum(INT petNum);
        //Lua_ICON_INFO GetPetImpactIconNameByIndex(INT petNum,INT impactIdx);

        //////ZZH- 改成CActionItem for lua binding: tActionItem*	EnumPetSkill(INT nPetIndex,INT nPetSkill,const char* szFilter);
        //CActionItem*	EnumPetSkill(INT nPetIndex,INT nPetSkill,const char* szFilter);
        //VOID	Select_Pet(INT nPetNum);
        //INT		GetAIType(INT nPetNum);
        //INT		GetSkillPassive(INT nPetNum,INT nPetSkill);
        //VOID	Free_Confirm(INT nPetNum);

        //VOID NotifyPetDlgClosed();////ZZH+
        //VOID SetSelectPetIdx(INT nIndex); ////ZZH+: Pet:SetSelectPetIdx(nIndex);
        //tActionItem*	GetPetFightAction();////ZZH+
        //tActionItem*    GetPetRelaxAction();////ZZH+
        //INT GetPetLocation(INT nPetNum);////ZZH+
        //INT GetTakeLevel(INT nPetNum);////ZZH+

        ////// 宠物的食物类型
        //INT GetFoodType(INT nPetNum);////ZZH+
        //VOID NotifySelChange(INT nPetNum);////ZZH+
        ////// 宠物称号
        //INT GetTitleNum(INT nPetNum);////ZZH+

        //VOID PetOpenPetJian(INT nPetNum, const char* szName); ////zzh+
    };
}