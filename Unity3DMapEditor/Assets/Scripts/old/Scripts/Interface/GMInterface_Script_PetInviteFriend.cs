using System;
using System.Collections.Generic;
using DBSystem;
using Network;
using Network.Packets;

namespace Interface
{
    public class PetInviteFriend
    {
        public class Lua_PetHumanInfo
        {//tolua_export
	
		    //tolua_begin
            public uint m_HumanGUID = MacroDefine.UINT_MAX;
            public string m_strHumanGUID = "";
            public int m_nHumanLevel = 0;
            public string m_szHumanName = "";
            public string m_szHumanGuildName = "";
            public int m_nHumanMenPai = MacroDefine.INVALID_ID;
		    //tolua_end
		    
		    public void CleanUp()
		    {
                m_HumanGUID = MacroDefine.UINT_MAX;
			    m_strHumanGUID		=   "";
			    m_nHumanLevel		=	0;
			    m_szHumanName		=	"";
			    m_szHumanGuildName	=	"";
                m_nHumanMenPai = MacroDefine.INVALID_ID;
		    }

	    };//tolua_export

        public class PET_FRIEND
		{
            public SDATA_PET m_Pet               = new SDATA_PET();
			public CObject_PlayerNPC	m_Avatar = null;

			public PET_FRIEND()
			{
				m_Avatar = null;
			}
		};

        public int GetInviteNum()
        {
            for (int i = 0; i < m_PetList.Count; ++i)
            {
                if (MacroDefine.INVALID_ID == GetPetListData(i).IsPresent)
                {
                    return i;
                }
            }
            return m_PetList.Count;
        }

		//获得征友宠主的详细信息（名称、等级之类）
		// 修改了返回值和参数，需要修改lua脚本 [6/8/2010 Sun]
        public Lua_PetHumanInfo GetHumanINFO(int nIndex)
        {
            int plidx = nIndex - 1;	//Lua is begin from 1, but c begin from 0
		    Lua_PetHumanInfo petInfo = new Lua_PetHumanInfo();
            _PET_PLACARD_ITEM curItem = GetPlaceCardItem(plidx);
		    if( (object)curItem  != null)
		    {
                string str = string.Format("{0:X}", curItem.m_HumanGUID);
			    petInfo.m_strHumanGUID = str;
			    petInfo.m_HumanGUID = curItem.m_HumanGUID;
			    petInfo.m_szHumanName = UIString.Instance.GetUnicodeString(curItem.m_szHumanName);
			    petInfo.m_nHumanLevel = curItem.m_nHumanLevel;
			    petInfo.m_nHumanMenPai = curItem.m_nHumanMenPai;
			    petInfo.m_szHumanGuildName = UIString.Instance.GetUnicodeString(curItem.m_szHumanGuildName);
		    }
		    return petInfo;
        }

		//获得宠主对宠物的介绍信息
        public string GetInviteMsg(int nIndex)
        {
            int plidx = nIndex - 1;	//Lua is begin from 1, but c begin from 0
            _PET_PLACARD_ITEM curItem = GetPlaceCardItem(plidx);
            if ((object)curItem != null)
            {
                return UIString.Instance.GetUnicodeString(curItem.m_szMessage);
            }
            return "";
        }

		//设置界面上显示的模型
        public void SetPetModel(int nIndex)
        {
            int lidx = nIndex - 1;	//Lua is begin from 1, but c begin from 0
            SDATA_PET petData = GetPetListData(lidx);
            if (petData != null)
            {
                if (MacroDefine.INVALID_ID != petData.IsPresent)
                {
                    SetPetListFakeObj(lidx, petData.DataID);
                }
            }
        }

		//显示相应宠物的详细信息，需要调用TargetPet界面来显示
        public void ShowTargetPet(int nIndex)
        {
            int lidx = nIndex - 1;	//Lua is begin from 1, but c begin from 0
            SDATA_PET petData = GetPetListData(lidx);
		    if(petData != null)
		    {
			    if(MacroDefine.INVALID_ID != petData.IsPresent)
			    {
				    CDataPool.Instance.Pet_CopyToTarget(petData);
				    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PETINVITEFRIEND, "target");	//通知TargetPet显示宠物
			    }
		    }
        }

		//获得宠物板宠物的信息
        public string GetPetINFO(int nIndex, string lpOp)
        {
            int lidx = nIndex - 1;	//Lua is begin from 1, but c begin from 0
            SDATA_PET petData = GetPetListData(lidx);
            if (petData != null)
		    {
                if (lpOp == "NAME")
			    {
                    return petData.Name;
			    }
		    }
		    return null;
        }
		////tolua_end

	    //网络处理相关接口
        public void ConvertPlaceCard2PetFriend()
        {
            CleanUpPetList();
            for (int i = 0; i < m_PetList.Count; ++i)
            {
                SDATA_PET pOut = GetPetListData(i);
                _PET_PLACARD_ITEM curItem = GetPlaceCardItem(i);
                if (pOut != null && (object)curItem != null)
                {
                    _PET_DETAIL_ATTRIB pIn = curItem.m_PetAttr;
                    PET_DETAIL_ATTRIB2SDATA_PAT(pIn, pOut);
                }
            }
        }

	    //公用函数接口
        public void PET_DETAIL_ATTRIB2SDATA_PAT(_PET_DETAIL_ATTRIB pIn, SDATA_PET pOut)
        {
            if ((object)pIn == null || pOut == null)
			    return;

		    //清除旧数据
		    pOut.CleanUp();
            pOut.IsPresent = 2; 
		    //基本数据转换
		    pOut.GUID			= pIn.m_GUID;				// ID
		    pOut.idServer		= (uint)pIn.m_ObjID;				// 所有Obj类型的ObjID
            pOut.DataID = pIn.m_nDataID;
            // Data ID,宠物类型
            pOut.Name = UIString.Instance.GetUnicodeString(pIn.m_szName);
            pOut.AIType = pIn.m_nAIType;			// 性格
            pOut.SpouseGUID = pIn.m_SpouseGUID;		// 配偶的GUID
            pOut.Level = pIn.m_nLevel;			// 等级
            pOut.Exp = pIn.m_nExp;				// 经验
            pOut.HP = pIn.m_nHP;				// 血当前值
            pOut.HPMax = pIn.m_nHPMax;			// 血最大值

            pOut.Age = pIn.m_nLife;				// 当前寿命
            pOut.EraCount = pIn.m_byGeneration;		// 几代宠
            pOut.Happiness = pIn.m_byHappiness;		// 快乐度

            pOut.AttPhysics = pIn.m_nAtt_Physics;		// 物理攻击力
            pOut.AttMagic = pIn.m_nAtt_Magic;		// 魔法攻击力
            pOut.DefPhysics = pIn.m_nDef_Physics;		// 物理防御力
            pOut.DefMagic = pIn.m_nDef_Magic;		// 魔法防御力

            pOut.Hit = pIn.m_nHit;				// 命中率
            pOut.Miss = pIn.m_nMiss;				// 闪避率
            pOut.Critical = pIn.m_nCritical;			// 会心率

            pOut.ModelID = pIn.m_nModelID;			// 外形
            pOut.MountID = pIn.m_nMountID;			// 座骑ID

            pOut.AttrStrApt = pIn.m_StrPerception;		// 力量资质
            pOut.AttrConApt = pIn.m_ConPerception;		// 体力资质
            pOut.AttrDexApt = pIn.m_DexPerception;		// 身法资质
            pOut.AttrSprApt = pIn.m_SprPerception;		// 灵气资质
            pOut.AttrIntApt = pIn.m_IntPerception;		// 定力资质

            pOut.AttrStr = pIn.m_Str;				// 力量
            pOut.AttrCon = pIn.m_Con;				// 体力
            pOut.AttrDex = pIn.m_Dex;				// 身法
            pOut.AttrSpr = pIn.m_Spr;				// 灵气
            pOut.AttrInt = pIn.m_Int;				// 定力
            pOut.Basic = pIn.m_GenGu;				// 根骨

            pOut.Pot = pIn.m_nRemainPoint;		// 潜能点

		    //技能转换
            int minSkill = ((int)ENUM_PET_SKILL_INDEX.PET_SKILL_INDEX_NUMBERS > GAMEDEFINE.MAX_PET_SKILL_COUNT) ? GAMEDEFINE.MAX_PET_SKILL_COUNT : (int)ENUM_PET_SKILL_INDEX.PET_SKILL_INDEX_NUMBERS;
		    for(int i=0; i< minSkill; ++i)
		    {
			    PET_SKILL theSkill = pOut[i];
                _DBC_SKILL_DATA pDefine = CSkillDataMgr.Instance.GetSkillData((uint)pIn.m_aSkill[i].m_nSkillID);

			    if(theSkill != null)
			    {
				    PET_SKILL newSkill = new PET_SKILL();

				    newSkill.m_pDefine = pDefine;
                    newSkill.m_nPetNum = GAMEDEFINE.MENPAI_PETSKILLSTUDY_PETNUM - (i + 1);	//no need to set if only shown.
				    newSkill.m_nPosIndex = i;
				    newSkill.m_bCanUse = true;
                    pOut[i] = newSkill;
			    }
			    else
			    {
				    theSkill.m_pDefine = pDefine;
                    theSkill.m_nPetNum = GAMEDEFINE.MENPAI_PETSKILLSTUDY_PETNUM - (i + 1);	//no need to set if only shown.
				    theSkill.m_bCanUse = true;
				    theSkill.m_nPosIndex = i;
			    }
		    }
        }
        static bool bFirst = false;
        public void InitPetList()
        {
		    if(bFirst) return;

		    m_PetList.Clear();

		    for(int i=0; i < GAMEDEFINE.MAX_PETPLACARD_LIST_ITEM_NUM; ++i)
		    {
			    PET_FRIEND pf = new PET_FRIEND();
                pf.m_Pet.CleanUp();
                pf.m_Avatar = CObjectManager.Instance.NewFakePlayerNPC();
			    m_PetList.Add(pf);
		    }
		    bFirst = true;
        }

        public void CleanUpPetList()
        {
            for (int i = 0; i < m_PetList.Count; ++i)
            {
                m_PetList[i].m_Pet.CleanUp();
            }
        }

        public void DestoryPetList()
        {
            m_PetList.Clear();
        }

        public void SetPetListFakeObj(int idx, int rdx)
        {
            if (idx >= m_PetList.Count || idx < 0)
                return;

            m_PetList[idx].m_Avatar.GetCharacterData().Set_RaceID(rdx);
            m_PetList[idx].m_Avatar.SetFaceDir(0.0f);
        }

        public SDATA_PET GetPetListData(int idx)
        {
            if (idx >= m_PetList.Count || idx < 0)
                return null;

            return m_PetList[idx].m_Pet;
        }

        public _PET_PLACARD_ITEM GetPlaceCardItem(int idx)
        {
            return CDataPool.Instance.PetPlacard_GetItem(idx);
        }

        static readonly PetInviteFriend instance = new PetInviteFriend();
        public static PetInviteFriend Instance
        {
            get
            {
                return instance;
            }
        }
        List<PET_FRIEND> m_PetList = new List<PET_FRIEND>();

    }
}