/****************************************\
*										*
* 			   数据池数据结构接口		*
*					-角色数据			*
*										*
\****************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using DBSystem;
using UnityEngine;
using Interface;

public class CCharacterData
{

    public CCharacterData(CObject_Character pCharObj)
    {
        m_pObjRef = pCharObj;
        switch (pCharObj.GetCharacterType())
        {
            case CHARACTER_TYPE.CT_MONSTER:
                m_pData = new SDATA_NPC();
                _Init_AsCharacter();
                _Init_AsNPC();
                break;
            case CHARACTER_TYPE.CT_PLAYEROTHER:
                m_pData = new SDATA_PLAYER_OTHER();
                _Init_AsCharacter();
                _Init_AsNPC();
                _Init_AsPlayerOther();
                break;
            case CHARACTER_TYPE.CT_PLAYERMYSELF:
                m_pData = new SDATA_PLAYER_MYSELF();
                _Init_AsCharacter();
                _Init_AsNPC();
                _Init_AsPlayerOther();
                _Init_AsPlayerMySelf();

                break;
            case CHARACTER_TYPE.CT_SpecialBus:
                m_pData = new SDATA_SPECIAL_BUS();
                _Init_AsCharacter();
                _Init_AsSpecialBus();
                break;
        }
    }

    //公用接口
    public int Get_RaceID() { return m_pData.m_nRaceID; }
    public void Set_RaceID(int nRaceID)
    {
        if (nRaceID != m_pData.m_nRaceID)
        {
            m_pData.m_nRaceID = nRaceID;
            m_pObjRef.OnDataChanged_RaceID();

            if (m_pObjRef is CObject_PlayerNPC)
            {
                CObject_PlayerNPC npc = m_pObjRef as CObject_PlayerNPC;
                npc.UpdateMissionState();
            }
            
        }
    }

    public string Get_Name() { return m_pData.m_strName; }
    public void Set_Name(string szName)
    {
        if (szName != m_pData.m_strName)
        {
            m_pData.m_strName = szName;
            m_pObjRef.OnDataChanged_Name();

            //Push事件
            string strReturn = "";
            if (m_pObjRef.IsSpecialObject(ref strReturn))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_NAME, strReturn);
            }
        }
    }
    public int Get_ModelID() { return m_pData.m_nModelID; }
    public void Set_ModelID(int nModelID)
    {
        if (nModelID != m_pData.m_nModelID)
        {
            m_pData.m_nModelID = nModelID;
            m_pObjRef.OnDataChanged_ModelID();
        }
    }
    public int Get_Level() { return m_pData.m_nLevel; }
    public void Set_Level(int nLevel)
    {
        m_pData.m_nLevel = nLevel;

        m_pObjRef.OnDataChanged_Level();

        //Push事件
        string strReturn = "";
        if (m_pObjRef.IsSpecialObject(ref strReturn))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_LEVEL, strReturn);
        }

        //根据Level重新刷新名字
        Set_Name(Get_Name());
    }
    protected void _Init_AsCharacter()
    {
        m_pData.m_nRaceID = MacroDefine.INVALID_ID;
        m_pData.m_nPortraitID = -1;			// 默认头像的值
        m_pData.m_strName = "";
        m_pData.m_fHPPercent = 0.1f;
        m_pData.m_fMPPercent = 0.1f;
        m_pData.m_fMoveSpeed = -1.0f;
        m_pData.m_CampData.CleanUp();
        m_pData.m_nOwnerID = -1;
        m_pData.m_OccupantGUID = MacroDefine.INVALID_GUID;
        m_pData.m_nModelID = -1;
        m_pData.m_nMountID = -1;
        m_pData.m_nLevel = 0;
        m_pData.m_bFightState = false;
        m_pData.m_nRelative = (int)ENUM_NPC_AI_ATTR.NPC_AI_TYPE_CANNOTATTACK;
        m_pData.m_bSit = false;
        m_pData.m_nStealthLevel = 0;
        m_pData.m_nRage = 0;
        m_pData.m_nAIType = 0;			//默认为0
        m_pData.m_nAmbit = MacroDefine.INVALID_ID;
    }
    protected void _Init_AsNPC()
    {

    }
    protected void _Init_AsPlayerOther()
    {
        

        ((SDATA_PLAYER_OTHER)m_pData).m_nMenPai = 0;
        ((SDATA_PLAYER_OTHER)m_pData).m_nHairMeshID = MacroDefine.INVALID_ID;
        ((SDATA_PLAYER_OTHER)m_pData).m_uHairColor = 0xFFFFFFFF;

        ((SDATA_PLAYER_OTHER)m_pData).m_nFaceMeshID = MacroDefine.INVALID_ID;

        ((SDATA_PLAYER_OTHER)m_pData).m_nEquipVer = 0;
        for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
        {
            ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentID[i] = MacroDefine.INVALID_ID;
            ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentGemID[i] = 0;
        }

        ((SDATA_PLAYER_OTHER)m_pData).m_bTeamFlag = false;
        ((SDATA_PLAYER_OTHER)m_pData).m_bTeamLeaderFlag = false;
        ((SDATA_PLAYER_OTHER)m_pData).m_bTeamFullFlag = false;
        ((SDATA_PLAYER_OTHER)m_pData).m_bTeamFollowFlag = false;
        ((SDATA_PLAYER_OTHER)m_pData).m_bIsInStall = false;
        ((SDATA_PLAYER_OTHER)m_pData).m_strStallName = "";
        ((SDATA_PLAYER_OTHER)m_pData).m_BusObjID = MacroDefine.UINT_MAX;
    }   
    protected void _Init_AsPlayerMySelf()
    {
        ((SDATA_PLAYER_MYSELF)m_pData).m_bLimitMove = false;
        ((SDATA_PLAYER_MYSELF)m_pData).m_theSkillClass.Clear();
        ((SDATA_PLAYER_MYSELF)m_pData).m_theSkill.Clear();
        
//     ((SDATA_PLAYER_MYSELF*)m_pData).m_nHP			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nMP			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nExp			= -1;
// 	//((SDATA_PLAYER_MYSELF*)m_pData).m_nMaxExp		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAmbitExp	= -1;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nMoney		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nStrikePoint		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nRMB		= 0;
// 	//((SDATA_PLAYER_MYSELF*)m_pData).m_nMaxVigor	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDoubleExpTime_Num		= 1;
// 	//((SDATA_PLAYER_MYSELF*)m_pData).m_nMaxEnergy	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nGoodBadValue= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_guidCurrentPet.Reset();
// 
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nGuild		= INVALID_ID;
// 
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nSTR			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nSPR			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nCON			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nINT			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDEX			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nWind		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nThunder		= 0;// 新增风、雷 [11/30/2010 Sun]
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nPoint_Remain= 0;
// 
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Physics	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Magic	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Physics	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Magic	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nMaxHP		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nMaxMP		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nHP_ReSpeed	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nMP_ReSpeed	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nHit			= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nMiss		= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nCritRate	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDefCritRate	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAttackSpeed	= 0;
// 
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Cold	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Cold	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Fire	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Fire	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Light	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Light	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Posion	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Wind	= 0;// 新增风、雷 [11/30/2010 Sun]
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Wind	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nAtt_Thunder = 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nDef_Thunder	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_bLimitMove	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_bCanActionFlag1	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_bCanActionFlag2	= 0;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_nTutorialMask	=0;
// 
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_bIsMinorPwdSetup = FALSE;
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_bIsMinorPwdUnlocked = FALSE;
// 
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_theSkillClass.clear();
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_theSkill.clear();
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_theLifeAbility.clear();
// 	((SDATA_PLAYER_MYSELF*)m_pData).m_theSprescr.clear();

    }
    protected void _Init_AsSpecialBus()
    {
    }
    //	int					Get_PortraitID(void) const { return m_pData.m_nPortraitID; }			
    //	void				Set_PortraitID(int nPortraitID);

    public int Get_PortraitID()
    {
        return m_pData.m_nPortraitID;
    }

    public void Set_PortraitID(int nPortraitID)
    {
        if (m_pData.m_nPortraitID != nPortraitID)
        {
            m_pData.m_nPortraitID = nPortraitID;
        }

    }
    // 刷新文字颜色 [10/9/2011 Ivan edit]
    public void RefreshName()
    {
        m_pObjRef.OnDataChanged_Name();
    }

    // 	string				Get_CurTitle(void) const { return m_pData.m_strTitle.c_str(); }											//1
    // 	void				Set_CurTitle(string szTitle);

    public string Get_CurTitle() { return m_pData.m_strTitle; }
    public void Set_CurTitle(string szTitle)
    {
        m_pData.m_strTitle = szTitle;
    }
    // 
    // 	byte				Get_CurTitleType(void) const { return m_pData.m_TitleType; }											//1
    //	void				Set_CurTitleType(byte bTitleType);

    public byte Get_CurTitleType() { return m_pData.m_TitleType; }
    public void Get_CurTitleType(byte bTitleType)
    {
        m_pData.m_TitleType = bTitleType;
        m_pObjRef.OnDataChanged_CurTitles();
    }

    public float Get_HPPercent() { return m_pData.m_fHPPercent; }
    public void Set_HPPercent(float fPercent)
    {
        float fOldHPPercent = m_pData.m_fHPPercent;
        m_pData.m_fHPPercent = fPercent;

        ////如果是玩家自己
        //if(	m_pObjRef.GetCharacterType() == CObject_Character::CT_PLAYERMYSELF)
        //{
        //	Set_HP((INT)(fPercent*Get_MaxHP()));
        //}

        //Push事件
        string strReturn = "";
        if (m_pObjRef.IsSpecialObject(ref strReturn))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_HP, strReturn);
        }
	
		//if(m_pData.m_fHPPercent <= 0.8)
		//{
			//CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_HP_LOWER,strReturn);
		//}
        if ((fOldHPPercent > 0 && m_pData.m_fHPPercent <= 0)
         || (fOldHPPercent <= 0 && m_pData.m_fHPPercent > 0))
        {
            m_pObjRef.OnDataChanged_Die();
        }

    }
    public bool IsDie() { return (Get_HPPercent() <= 0.0f); }
    // 
    // public	float				Get_MPPercent()  { return m_pData.m_fMPPercent; }		
    // public	void				Set_MPPercent(float fPercent)
    // {
    // 
    // }

    public float Get_MPPercent() { return m_pData.m_fMPPercent; }
    public void Set_MPPercent(float fPercent)
    {
        m_pData.m_fMPPercent = fPercent;

        ////如果是玩家自己
        //if(	m_pObjRef.GetCharacterType() == CObject_Character::CT_PLAYERMYSELF)
        //{
        //	Set_MP((INT)(fPercent*Get_MaxMP()));
        //}

        // Push事件
        string strReturn = "";
        if (m_pObjRef.IsSpecialObject(ref strReturn))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_MP, strReturn);
        }
    }

    public float Get_MoveSpeed() { return m_pData.m_fMoveSpeed; }
    public void Set_MoveSpeed(float fSpeed)
    {
        const float fBaseMoveSpeed = 3.5f;
        bool bChanged = Math.Abs(fSpeed - m_pData.m_fMoveSpeed) > 0.2f;
        if (bChanged)
        {// 速度有了明显的变化，需要改变其频率
        }
        m_pData.m_fMoveSpeed = fSpeed;
        m_fSpeedRate = fSpeed / fBaseMoveSpeed;
        //m_pObjRef.nDataChanged_MoveSpeed();

        //Push事件
        // 	STRING strReturn;
        // 	if(m_pObjRef.IsSpecialObject(strReturn))
        // 	{
        // 		CEventSystem::GetMe().PushEvent(GE_UNIT_MOVE_SPEED, strReturn.c_str());
        // 	}
    }

    public _CAMP_DATA Get_CampData() { return m_pData.m_CampData; }
    public void Set_CampData(_CAMP_DATA pCampData) { m_pData.m_CampData = pCampData; }
    // 
    public int Get_OwnerID() { return m_pData.m_nOwnerID; }
    public void Set_OwnerID(int nOwnerID)
    {
        m_pData.m_nOwnerID = nOwnerID;
    }

    // 	int					Get_AIType(void) const { return m_pData.m_nAIType; }
    // 	void				Set_AIType(int nAIType);

    // 	GUID_t				Get_OccupantGUID(void) const { return m_pData.m_OccupantGUID; }
    // 	void				Set_OccupantGUID(GUID_t guid);
    // 
    // 	int					Get_Relative(void) const { return m_pData.m_nRelative; }											
    // 	void				Set_Relative(int nRelative);



    public int Get_MountID() { return m_pData.m_nMountID; }
    public void Set_MountID(int nMountID)
    {

        if (nMountID != m_pData.m_nMountID)
        {
            m_pData.m_nMountID = nMountID;
            m_pObjRef.OnDataChanged_MountID();
        }
    }



    //public	bool				Get_FightState(){ return m_pData.m_bFightState; }	
    //public	void				Set_FightState(bool bFightState);

    public int Get_StealthLevel() { return m_pData.m_nStealthLevel; }
    public void Set_StealthLevel(int nStealthLevel)
    {
        if (m_pData.m_nStealthLevel != nStealthLevel)
        {
            m_pData.m_nStealthLevel = nStealthLevel;
            m_pObjRef.OnDataChanged_StealthLevel();
        }
    }
    // 
    public bool IsSit() { return m_pData.m_bSit; }
    public void Set_Sit(bool bSit)
    {
        if (m_pData.m_bSit != bSit)
        {
            m_pData.m_bSit = bSit;

            if (m_pObjRef != null)
            {
                m_pObjRef.OnDataChanged_Sit();
            }
        }
    }

    // 	//---------------------------------
    // 	/**
    // 	 *	Character_PlayerOther 对于其他玩家
    // 	 */
    public int Get_MenPai()
    {
        return ((SDATA_PLAYER_OTHER)m_pData).m_nMenPai;
    }
    public	int					Get_MenPaiByOrder() 
    {
        int oMenPai = -1;
        switch (((SDATA_PLAYER_OTHER)m_pData).m_nMenPai)
        {
        case 0:
	        oMenPai = 0;//金
	        break;
        case 1:
	        oMenPai = 2;//水
	        break;
        case 2:
	        oMenPai = 3;//火
	        break;
        case 3:
	        oMenPai = 4;//土
	        break;
        case 4:
	        oMenPai = 1;//木
	        break;
        }
        return oMenPai;
     }
     public	void				Set_MenPai(int nMenPai)
     {
        bool bReset = (((SDATA_PLAYER_OTHER)m_pData).m_nMenPai != nMenPai);
        //如果门派相同就不设置其他的属性
         if (!bReset) return;
         //todo
// 	    if(((SDATA_PLAYER_OTHER*)m_pData).m_nMenPai != -1)
// 	    {
// 		    if((9 == ((SDATA_PLAYER_OTHER*)m_pData).m_nMenPai) && (nMenPai != ((SDATA_PLAYER_OTHER*)m_pData).m_nMenPai))
// 			    CSoundSystemFMod::_PlayUISoundFunc(62);
// 	    }

	    ((SDATA_PLAYER_OTHER)m_pData).m_nMenPai = nMenPai;

	    // 设置默认攻击为0号技能：普通攻击 [7/27/2011 ivan edit]
        Interface.GameInterface.Instance.Skill_SetActive(CActionSystem.Instance.GetAction_SkillID(0));
    	
	    // 初始化技能，策划要求取消通用普通攻击，每个门派有自己的普通攻击 [11/14/2011 Ivan edit]
         //todo
	    //InitDefaultSkill(nMenPai);
    	
	    //如果是自己的门派数据，重构心法/技能数据
	    //if(bReset && m_pObjRef.GetCharacterType() == CObject_Character::CT_PLAYERMYSELF)
	    //{
	    //	_On_MenPaiChanged();
	    //}

	    //Talk::s_Talk.MenPaiJoin((MenPaiID_t)nMenPai); //__

	    // fresh animation 因为门派会影响动作 [10/14/2010 Sun]
	    if(m_pObjRef != null)
	    {
		    m_pObjRef.FreshAnimation();
	    }

	    //Push事件
	    string strReturn="";
	    if(m_pObjRef.IsSpecialObject(ref strReturn))
	    {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_MENPAI, strReturn);
	    }
     }
    
    public 	int					Get_Guild()
    {
        return ((SDATA_PLAYER_MYSELF)m_pData).m_nGuild;
    }									
    public 	void				Set_Guild(int nGuild)
    {
        ((SDATA_PLAYER_MYSELF)m_pData).m_nGuild = nGuild;

        if (((SDATA_PLAYER_MYSELF)m_pData).m_nGuild != MacroDefine.INVALID_ID)
        {
            //Talk.s_Talk.GuildJoin(Get_Guild());
        }
        else
        {
           // Talk.s_Talk.GuildLeave();
        }
    }
    
    public int Get_HairMesh()
    {
        return ((SDATA_PLAYER_OTHER)m_pData).m_nHairMeshID;
    }
    public void Set_HairMesh(int nHairMesh)
    {
        if (((SDATA_PLAYER_OTHER)m_pData).m_nHairMeshID != nHairMesh)
        {
            ((SDATA_PLAYER_OTHER)m_pData).m_nHairMeshID = nHairMesh;
            ((CObject_PlayerOther)m_pObjRef).OnDataChanged_HairMesh();
        }
    }
    public uint Get_HairColor()
    {
        return ((SDATA_PLAYER_OTHER)m_pData).m_uHairColor;
    }
    public void Set_HairColor(uint uHairColor)
    {
        ((SDATA_PLAYER_OTHER)m_pData).m_uHairColor = uHairColor;
    }

    public int Get_FaceMesh()
    {
        return ((SDATA_PLAYER_OTHER)m_pData).m_nFaceMeshID;
    }
    public void Set_FaceMesh(int nFaceMesh)
    {
        if (((SDATA_PLAYER_OTHER)m_pData).m_nFaceMeshID != nFaceMesh)
        {
            ((SDATA_PLAYER_OTHER)m_pData).m_nFaceMeshID = nFaceMesh;
            ((CObject_PlayerOther)m_pObjRef).OnDataChanged_FaceMesh();
        }
    }

    public int Get_EquipVer()
    {
        return ((SDATA_PLAYER_OTHER)m_pData).m_nEquipVer;
    }
    public void Set_EquipVer(int nEquipVer)
    {
        bool bNeedUpdate = ((SDATA_PLAYER_OTHER)m_pData).m_nEquipVer != nEquipVer;
        if (bNeedUpdate)
        {
            ((SDATA_PLAYER_OTHER)m_pData).m_nEquipVer = nEquipVer;
            ((CObject_PlayerOther)m_pObjRef).OnDataChanged_EquipVer();
            //Push事件
            string strReturn = "";
            if (m_pObjRef.IsSpecialObject(ref strReturn))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_EQUIP_VER, strReturn);
            }
        }
    }
    // 
    public int Get_Equip(HUMAN_EQUIP point)
    {
        return ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentID[(int)point];
    }
    public void Set_Equip(HUMAN_EQUIP point, int nID)
    {
        if (nID != ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentID[(int)point])
        {
            ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentID[(int)point] = nID;
            //通知角色层，更改外表
            ((CObject_PlayerOther)m_pObjRef).OnDataChanged_Equip(point);
        }
        //Push事件
        string strReturn = "";
        if (m_pObjRef.IsSpecialObject(ref strReturn))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_EQUIP, strReturn);

            if (point == HUMAN_EQUIP.HEQUIP_WEAPON)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_EQUIP_WEAPON, strReturn);
            }
        }
    }
    // 
    public int Get_EquipGem(HUMAN_EQUIP point)
    {
        //如果是玩家自己，存储物品实例
        return ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentGemID[(int)point];
    }
    public void Set_EquipGem(HUMAN_EQUIP point, int nID)
    {
        if (((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentGemID[(int)point] == nID) return;
        ((SDATA_PLAYER_OTHER)m_pData).m_nEquipmentGemID[(int)point] = nID;

        _On_GemChanged(point);
        //通知角色层，更改外表
        ((CObject_PlayerOther)m_pObjRef).OnDataChanged_Equip(point);

        //Push事件
        string strReturn = "";
        if (m_pObjRef.IsSpecialObject(ref strReturn))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_EQUIP, strReturn);

            if (point == HUMAN_EQUIP.HEQUIP_WEAPON)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_EQUIP_WEAPON, strReturn);
            }
        }
    }
    // 	void				Set_EquipEffect(HUMAN_EQUIP point, UINT nEffect);
    // 	int					Get_EquipGemEffect(HUMAN_EQUIP point) const;// 获取宝石特效索引 [10/25/2011 Sun]
    // 	int					Get_EquipStrengthenEffect(HUMAN_EQUIP point) const;// 获取强化特效索引 [10/25/2011 Sun]
    // 
    public	bool				Get_HaveTeamFlag()
    {
        SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
        if (otherData == null)
            throw new NullReferenceException("Not Player Other in Set_HaveTeamFlag");

        return otherData.m_bTeamFlag;
    }
    public	void				Set_HaveTeamFlag(bool flag)
    {
       SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
        if(otherData == null)
            throw new NullReferenceException("Not Player Other in Set_HaveTeamFlag");
        otherData.m_bTeamFlag = false;

	    // 广播是否组队 [6/16/2011 ivan edit]
	    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TEAM_STATE, flag ? 1 : 0);
    }
     
    public	bool				Get_TeamLeaderFlag()
    {
        SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
        if (otherData == null)
            throw new NullReferenceException("Not Player Other in Get_TeamLeaderFlag");

        return otherData.m_bTeamLeaderFlag;
    }
    public	void				Set_TeamLeaderFlag(bool flag)
    {
        SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
        if (otherData == null)
            throw new NullReferenceException("Not Player Other in Set_TeamLeaderFlag");

        otherData.m_bTeamLeaderFlag = false;

        m_pObjRef.OnDataChanged_TeamLeaderFlag();
    }
     
    public	bool				Get_TeamFullFlag()
    {
        SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
        if (otherData == null)
            throw new NullReferenceException("Not Player Other in Get_TeamFullFlag");

        return otherData.m_bTeamFullFlag;
    }
     public	void				Set_TeamFullFlag(bool flag)
    {
        SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
        if (otherData == null)
            throw new NullReferenceException("Not Player Other in Set_TeamFullFlag");

        otherData.m_bTeamFullFlag = flag;

    }

     public bool Get_TeamFollowFlag()
     {
         SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
         if (otherData == null)
             throw new NullReferenceException("Not Player Other in Get_TeamFollowFlag");

         return otherData.m_bTeamFollowFlag;
     }
     public void Set_TeamFollowFlag(bool flag)
     {
         SDATA_PLAYER_OTHER otherData = m_pData as SDATA_PLAYER_OTHER;
         if (otherData == null)
             throw new NullReferenceException("Not Player Other in Set_TeamFollowFlag");

         otherData.m_bTeamFollowFlag = flag;
     }
    // 
    // 	int					Get_TeamFollowListCount(void) const;
    // 	void				Set_TeamFollowListCount(int count);
    // 
    // 	GUID_t				Get_TeamFollowedMember(int index) const;
    // 	void				Set_TeamFollowedMember(int index, GUID_t guid);
    // 
    // 	_TITLE_				Get_Title(int index) const;										
    // 	void				Set_Title(int index, _TITLE_ title);
    // 
    // 	int					Get_TitleNum() const;
    // 	void				Set_TitleNum(int num );
    // 
    // 	int					Get_CurTitleIndex() const;
    // 	void				Set_CurTitleIndex(int index );
    // 
    // 	void				Clear_AllTitles();		
    // 	// Bus相关接口 [8/15/2011 ivan edit]
    public void Set_BusObjID(uint nBusObjID)
    {
        SDATA_PLAYER_OTHER otherPlayerData = (SDATA_PLAYER_OTHER)m_pData;
        if (otherPlayerData != null)
        {
            otherPlayerData.m_BusObjID = nBusObjID;
            m_pObjRef.OnDataChanged_BusObjID();
        }
        else
        {
            LogManager.LogError("Set_BusObjID SDATA_PLAYER_OTHER Fail");
        }
    }

    public uint Get_BusObjID()
    {
        SDATA_PLAYER_OTHER otherPlayerData = (SDATA_PLAYER_OTHER)m_pData;
        if (otherPlayerData != null)
        {
            return otherPlayerData.m_BusObjID;
        }
        else
        {
            LogManager.LogError("Get_BusObjID SDATA_PLAYER_OTHER Fail");
            return 0;
        }
    }
    // 	bool				IsInBusAndLimitMove(void);
    // 
    // 	//---------------------------------
    // 	/**
    // 	 *	Character_SpecialBus --特殊bus对象属性 ivan
    // 	 */
    // 	void				Set_BusID(ObjID_t busId);// 该id关联到businfo表 [8/23/2011 ivan edit]
    // 	ObjID_t				Get_BusID(void)const;
    // 	
    // 	int					Get_BusHP(void) const;
    // 	void				Set_BusHP(int nHP);
    // 
    // 	int					Get_BusMaxHP(void) const;
    // 	void				Set_BusMaxHP(int nHP);
    // 	//---------------------------------
    // 	/**
    // 	 *	Character_PlayerMySelf 对于玩家自己
    // 	 */
    // 
    public int Get_HP()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_HP SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nHP;
    }

    public void Set_HP(int nHP)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_HP SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nHP = nHP;
    }

    public int Get_MP()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_MP SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nMP;
    }

    public void Set_MP(int nMP)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_MP SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nMP = nMP;
    }

    public int Get_Exp()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_Exp SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nExp;
    }

    public void Set_Exp(int nExp)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_Exp SDATA_PLAYER_MYSELF Fail");
            return;
        }

        ShowExpDelta(mySelfData.m_nExp, nExp);

        mySelfData.m_nExp = nExp;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_EXP, "player");
    }

    void ShowExpDelta(int oldExp,int newExp)
    {
        if (oldExp <= 0)
            return;
        int expDel = newExp - oldExp;
        if (expDel > 0)
            m_pObjRef.ShowExp(expDel);
    }

    // 
    // 	int					Get_AmbitExp(void) const;
    // 	void				Set_AmbitExp(int nExp);
    // 
    // 	int					Get_Ambit(void) const;
    // 	void				Set_Ambit(int nAmbit);
    public int Get_Ambit()
    {
        return m_pData.m_nAmbit;
    }

    public void Set_Ambit(int nAmbit)
    {
        m_pData.m_nAmbit = nAmbit;
        m_pObjRef.OnDataChanged_Ambit();

        //Push事件
        string strReturn = "";
        if (m_pObjRef.IsSpecialObject(ref strReturn))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_AMBIT, strReturn);
        }
    }
    // 
    // 	int					Get_MaxExp(void) const;											
    // 	//void				Set_MaxExp(int nMaxExp); //通过查表获得
    public int Get_MaxExp()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_MaxExp SDATA_PLAYER_MYSELF Fail");
            return 0;
        }

        _DBC_CHARACTER_EXPLEVEL pCharExpLevel = CDataBaseSystem.Instance.GetDataBase<_DBC_CHARACTER_EXPLEVEL>((int)DataBaseStruct.DBC_CHARACTER_EXPLEVEL).Search_Index_EQU(Get_Level());
        if (pCharExpLevel == null)
        {
            return -1;
        }
        else
            return pCharExpLevel.nEffectNeed;
    }
    // 
    // 	// 提升所需仙缘值 [12/1/2010 Sun]
    // 	int					Get_MaxAmbitExp(void) const;
    // 	// 获取境界名称 [1/6/2011 ivan edit]
    public string Get_AmbitName()
    {
        //TODO...
        return "";
    }
     
    public 	int					Get_Money()	
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if(selfData == null)
        {   
            throw new NullReferenceException("Not myself in CharacterData:Get_Money()");
        }
        return selfData.m_nMoney;
    }							
    public 	void				Set_Money(int nMoney)
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if(selfData == null)
        {
            throw new NullReferenceException("Not myself Data CharacterData:Set_Money()");
        }
        selfData.m_nMoney = nMoney;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_MONEY, "player" );

        //if(0 < Get_Level() && 100 >= Get_Level())
        //{
        //    DBC_DEFINEHANDLE(s_pLvMaxMoneyDBC, DBC_LV_MAXMONEY);
        //    const _DBC_LV_MAXMONEY* pMax = (const _DBC_LV_MAXMONEY*)s_pLvMaxMoneyDBC.Search_Index_EQU(Get_Level());
        //    if(!pMax) return;

        //    BOOL bLog = (CGameProcedure::s_pUISystem != NULL)?TRUE:false;
    		
        //    INT myBankMoney = CDataPool::GetMe().UserBank_GetBankMoney();
        //    if((((SDATA_PLAYER_MYSELF*)m_pData).m_nMoney + myBankMoney) > pMax.nMaxMoney && bLog)
        //    {
        //        STRING strWarn = COLORMSGFUNC("MONEY_OVERFLOW");
        //        ADDTALKMSG(strWarn);
        //    }
    			
        //}
    }

    public PET_GUID_t[] Get_CurrentPetGUID()
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if(selfData == null)
            throw new NullReferenceException("Not myself Data CharacterData:Get_CurrentPetGUID()");

        return selfData.m_guidCurrentPet;
    }

    public bool isFightPet(PET_GUID_t guid)
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("Not myself Data CharacterData:isFightPet()");
        for (int i = 0; i < GAMEDEFINE.MAX_CURRENT_PET; i++)
        {
            if (guid == selfData.m_guidCurrentPet[i])
            {
                return true;
            }
        }
        return false;
    }

    public bool IsMyPet(uint serverId)
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("Not myself Data CharacterData:isFightPet()");
        for (int i = 0; i < GAMEDEFINE.MAX_CURRENT_PET; i++)
        {
            SDATA_PET pet = CDataPool.Instance.Pet_GetPet(selfData.m_guidCurrentPet[i]);
            if (pet != null && pet.idServer == serverId)
            {
                return true;
            }
        }
        return false;
    }

    public void Set_CurrentPetGUID(PET_GUID_t[] guid)
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("Not myself Data CharacterData:Set_CurrentPetGUID()");

        for (int i = 0; i < GAMEDEFINE.MAX_CURRENT_PET; i++)
        {
            //LogManager.LogWarning(i + " CurrentPetGUI " + guid[i].m_uHighSelection + " " + guid[i].m_uLowSelection);
            if (guid[i] != selfData.m_guidCurrentPet[i])
            {
                selfData.m_guidCurrentPet[i] = guid[i];
                //CActionSystem.Instance.UpdateToolBarForPetSkill();
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PET_PAGE);
            }
        }
        //if (selfData.m_guidCurrentPet != guid)
        //{
        //    selfData.m_guidCurrentPet = guid;
        //    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PET_PAGE);
        //}

    }
    // 
    public int Get_Rage()
    {
        return ((SDATA_CHARACTER)m_pData).m_nRage;
    }

    public void Set_Rage(int nRage)
    {
        ((SDATA_CHARACTER)m_pData).m_nRage = nRage;

        //string strReturn;
        //if(m_pObjRef.IsSpecialObject(strReturn) )
        //{
        //    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_RAGE, strReturn);
        //}
    }

    public int Get_StrikePoint()
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;

        return selfData.m_nStrikePoint;
    }

    public void Set_StrikePoint(int nStrikePoint)
    {
	    SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        selfData.m_nStrikePoint = nStrikePoint;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_STRIKEPOINT, "player");
    }
     
    public	int					Get_RMB()
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("Not myself Data CharacterData:Get_RMB()");
        return selfData.m_nRMB;
    }
    public	void				Set_RMB(int nRMB)
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("Not myself Data CharacterData:Set_RMB()");

        selfData.m_nRMB = nRMB;
	    CEventSystem.Instance.PushEvent( GAME_EVENT_ID.GE_UNIT_RMB, "player");
    }
    // 
    // 	int				    Get_MaxVigor(void)const;
    // 	void				Set_MaxVigor(int nVigor);
    // 
    public int Get_DoubleExpTime_Num()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_DoubleExpTime_Num SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nDoubleExpTime_Num;
    }

    public void Set_DoubleExpTime_Num(int nDoubleExpTime_Num)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_DoubleExpTime_Num SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nDoubleExpTime_Num = nDoubleExpTime_Num;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_ENERGY, "player");
    }

    public int Get_MaxEnergy()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_MaxEnergy SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nMaxEnergy;
    }

    public void Set_MaxEnergy(int nMaxEnergy)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_MaxEnergy SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nMaxEnergy = nMaxEnergy;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_MAX_ENERGY, "player");

    }
    // 
    // 	//int					Get_MaxEnergy(void)const;
    // 	//void				Set_MaxEnergy(int nEnergy);
    // 
    // 	int					Get_GoodBadValue(void)const;
    // 	void				Set_GoodBadValue(int nValue);
    // 
    public int Get_Gender()
    {
        return m_pData.m_nRaceID % DBC_DEFINE.CHAR_RACE_NUM;
    }

    public void Set_Gender(int nGender)
    {

    }
    // 
    // 	UINT				Get_TutorialMask(void) const;
    // 	void				Set_TutorialMask(UINT nMask);
    public int Get_STR()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_STR SDATA_PLAYER_MYSELF Fail");
            return 0;
        }

        return mySelfData.m_nSTR;    
    }

    public void Set_STR(int nSTR)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_STR SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nSTR = nSTR;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_STR, "player");

    }

    public int Get_SPR()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_SPR SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nSPR;
    }

    public void Set_SPR(int nSPR)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_SPR SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nSPR = nSPR;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_SPR, "player");
    }

    public int Get_CON()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_CON SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nCON;
    }

    public void Set_CON(int nCON)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_CON SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nCON = nCON;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_CON, "player");
    }

    public int Get_INT()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_INT SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nINT;
    }

    public void Set_INT(int nINT)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_INT SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nINT = nINT;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_INT, "player");
    }
    public int Get_DEX()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_DEX SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nDEX;
    }

    public void Set_DEX(int nDEX)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_DEX SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nDEX = nDEX;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_DEX, "player");
    }
    public int Get_BringSTR()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_BringSTR SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nBringSTR;
    }
    public void Set_BringSTR(int nSTR)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_BringSTR SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nBringSTR = nSTR;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_BRING_STR, "player");
    }
    public int Get_BringSPR()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_BringSPR SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nBringSPR;
    }
    public void Set_BringSPR(int nSPR)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_BringSPR SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nBringSPR = nSPR;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_BRING_SPR, "player");
    }
    public int Get_BringCON()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_BringCON SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nBringCON;
    }
    public void Set_BringCON(int nCON)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_BringCON SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nBringCON = nCON;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_BRING_CON, "player");
    }
    public int Get_BringINT()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_BringINT SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nBringINT;
    }
    public void Set_BringINT(int nINT)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_BringINT SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nBringINT = nINT;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_BRING_INT, "player");
    }
    public int Get_BringDEX()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_BringDEX SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nBringDEX;
    }
    public void Set_BringDEX(int nDEX)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_BringDEX SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nBringDEX = nDEX;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_BRING_DEX, "player");
    }
    // public:
    // 	//金
    //public int              Get_Metal() { return this.Get_STR(); }
    //public void             Set_Metal(int nSTR) {  this.Set_STR(nSTR); }
    //// 	//木
    //public int              Get_Wood() { return this.Get_SPR(); }
    //public void             Set_Wood(int nWood) { this.Set_SPR(nWood); }
    //// 	//水
    //public int              Get_Water() { return this.Get_CON(); }
    //public void             Set_Water(int nWater) { this.Set_CON(nWater); }

    //// 	//火
    //public int              Get_Fire() { return this.Get_INT(); }
    //public void             Set_Fire(int nFire) { this.Set_INT(nFire); }
    //// 	//土
    //// 	int					Get_Earth(void) const	{   return this.Get_DEX();	}											
    //// 	void				Set_Earth(int nEarth)	{	this.Set_DEX(nEarth);  }
    //public int              Get_Earth() { return this.Get_DEX(); }
    //public void             Set_Earth(int nEarth) { this.Set_DEX(nEarth); }
    // 
    // 	//风
    // 	int					Get_Wind(void) const;
    // 	void				Set_Wind(int nWind);
    // 	//雷
    // 	int					Get_Thunder(void) const;
    // 	void				Set_Thunder(int nThunder);
    // 
    // public:
    // 
    // 	int					Get_PointRemain(void) const;									
    // 	void				Set_PointRemain(int nPoint);
    // 
    public int Get_AttPhysics()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_AttPhysics SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nAtt_Physics;
    }

    public void Set_AttPhysics(int nAttPhysics)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_AttPhysics SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nAtt_Physics = nAttPhysics;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_ATT_PHYSICS, "player");
    }

    public int Get_AttMagic()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_AttMagic SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nAtt_Magic;
    }

    public void Set_AttMagic(int nAttMagic)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_AttMagic SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nAtt_Magic = nAttMagic;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_ATT_MAGIC, "player");
    }

    public int Get_DefPhysics()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_DefPhysics SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nDef_Physics;
    }

    public void Set_DefPhysics(int nDefPhysics)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_DefPhysics SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nDef_Physics = nDefPhysics;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_DEF_PHYSICS, "player");
    }

    public int Get_DefMagic()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_DefMagic SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nDef_Magic;
    }

    public void Set_DefMagic(int nDefMagic)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_DefMagic SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nDef_Magic = nDefMagic;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_DEF_MAGIC, "player");
    }

    public int Get_Hit()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_Hit SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nHit;
    }

    public void Set_Hit(int nHit)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_Hit SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nHit = nHit;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_HIT, "player");
    }

    public int Get_Miss()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_Miss SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nMiss;
    }

    public void Set_Miss(int nMiss)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_Miss SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nMiss = nMiss;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_MISS, "player");
    }

    public int Get_CritRate()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_CritRate SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nCritRate;
    }

    public void Set_CritRate(int nCritRate)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_Miss SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nCritRate = nCritRate;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_CRIT_RATE, "player");
    }

    //增加会心防御[12/24/2010 ivan edit]

    public int Get_DefCritRate()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_DefCritRate SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nDefCritRate;
    }

    public void Set_DefCritRate(int nDefCritRate)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_DefCritRate SDATA_PLAYER_MYSELF Fail");
            return;
        }

        mySelfData.m_nDefCritRate = nDefCritRate;
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UNIT_DEF_CRIT_RATE, "player");
    }

    public int Get_MaxHP()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_MaxHP SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nMaxHP;
    }

    public void Set_MaxHP(int nMaxHP)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_MaxHP SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nMaxHP = nMaxHP;
    }
    // 
    public int Get_MaxMP()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_MaxMP SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nMaxMP;
    }

    public void Set_MaxMP(int nMaxMP)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_MaxMP SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nMaxMP = nMaxMP;
    }
    // 
    public int Get_HPRespeed()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_HPRespeed SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nHP_ReSpeed;
    }

    public void Set_HPRespeed(int nHPRespeed)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_HPRespeed SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nHP_ReSpeed = nHPRespeed;
    }
    // 
    public int Get_MPRespeed()
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Get_MPRespeed SDATA_PLAYER_MYSELF Fail");
            return 0;
        }
        return mySelfData.m_nMP_ReSpeed;
    }

    public void Set_MPRespeed(int nMPRespeed)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_MPRespeed SDATA_PLAYER_MYSELF Fail");
            return;
        }
        mySelfData.m_nMP_ReSpeed = nMPRespeed;
    }

    public  uint Get_TutorialMask() 
    {
	    SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
            if (mySelfData == null)
            {
                LogManager.LogError("Get_TutorialMask SDATA_PLAYER_MYSELF Fail");
                return 0;
            }

	    return mySelfData.m_nTutorialMask;
    }
    public void Set_TutorialMask(uint nMask)
    {
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData == null)
        {
            LogManager.LogError("Set_TutorialMask SDATA_PLAYER_MYSELF Fail");
            return;
        }


	    mySelfData.m_nTutorialMask = nMask;
    }
    // 
    // 	int					Get_Hit(void) const;											
    // 	void				Set_Hit(int nHit);
    // 
    // 	int					Get_Miss(void) const;											
    // 	void				Set_Miss(int nMiss);
    // 
    // 	int					Get_CritRate(void) const;										
    // 	void				Set_CritRate(int nCritRate);
    // 
    // 	// 新增会心防御 [12/24/2010 ivan edit]
    // 	int					Get_DefCritRate(void) const;
    // 	void				Set_DefCritRate(int nDefCritRate);
    // 
    // 	int					Get_AttackSpeed(void) const;									
    // 	void				Set_AttackSpeed(int fSpeed);
    // 
    // public:
    // 	int					Get_AttMagic(void) const;										
    // 	void				Set_AttMagic(int nAttMagic);
    // 
    // 	int					Get_DefMagic(void) const;										
    // 	void				Set_DefMagic(int nDefMagic);
    // 
    // 	int					Get_AttCold(void) const;										
    // 	void				Set_AttCold(int nAttCold);
    // 
    // 	int					Get_DefCold(void) const;										
    // 	void				Set_DefCold(int nDefCold);
    // 
    // 	int					Get_AttLight(void) const;										
    // 	void				Set_AttLight(int nAttLight);
    // 
    // 	int					Get_DefLight(void) const;										
    // 	void				Set_DefLight(int nDefLight);
    // 
    // 	int					Get_AttPosion(void) const;										
    // 	void				Set_AttPosion(int nAttPosion);
    // 
    // 	int					Get_DefPosion(void) const;										
    // 	void				Set_DefPosion(int nDefPosion);
    // 
    // 
    // 	//---五行战斗属性
    // public:
    // 
    // 	//金防、金攻 (冰)
    // 	int					Get_DefMetal(void) const { return this.Get_DefLight();}									
    // 	void				Set_DefMetal(int nDef)   { this.Set_DefLight(nDef);   }
    // 	int					Get_AttMetal(void) const { return this.Get_AttLight();}					
    // 	void				Set_AttMetal(int nAttMetal){ this.Set_AttLight(nAttMetal);}
    // 	//木防、木攻(火)
    // 	int					Get_AttWood(void) const { return this.Get_AttPosion(); }
    // 	void				Set_AttWood(int nAttWood) {this.Set_AttPosion(nAttWood); }
    // 	int					Get_DefWood(void) const { return this.Get_DefPosion();  }
    // 	void				Set_DefWood(int nDefWood) { this.Set_DefPosion(nDefWood); }
    // 	//水防、水攻(电)
    // 	int					Get_AttWater(void) const { return this.Get_AttCold(); }
    // 	void				Set_AttWater(int nAttWater) {this.Set_AttCold(nAttWater); }
    // 	int					Get_DefWater(void) const { return this.Get_DefCold();  }
    // 	void				Set_DefWater(int nDefWater) { this.Set_DefCold(nDefWater); }
    // 	//火防、火攻(毒)
    // 	int					Get_AttFire(void) const;
    // 	int					Get_DefFire(void) const;
    // 	void				Set_AttFire(int nAttFire);
    // 	void				Set_DefFire(int nDefFire);
    // 	//土防、土攻
    // 	int					Get_AttEarth(void) const;
    // 	void				Set_AttEarth(int nAttEarth);
    // 	int					Get_DefEarth(void) const;
    // 	void				Set_DefEarth(int nDefEarth);
    // 	//风防、风攻
    // 	int					Get_AttWind(void) const;
    // 	void				Set_AttWind(int nAttWind);
    // 	int					Get_DefWind(void) const;
    // 	void				Set_DefWind(int nDefWind);
    // 	//雷防，雷攻
    // 	int					Get_AttThunder(void) const;
    // 	void				Set_AttThunder(int nAttThunder);
    // 	int					Get_DefThunder(void) const;
    // 	void				Set_DefThunder(int nDefThunder);
    // 
    // 		//----
    // 
    public bool IsLimitMove()
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }

        return ((SDATA_PLAYER_MYSELF)m_pData).m_bLimitMove;
    }

    public void SetLimitMove(bool bSet)
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }
        ((SDATA_PLAYER_MYSELF)m_pData).m_bLimitMove = bSet;
    }
    // 
    // 	//bool				IsLimitUseSkill(void) const;
    // 	//void				SetLimitUseSkill(bool bSet);
    // 
    // 	//bool				IsLimitHandle(void) const;
    // 	//void				SetLimitHandle(bool bSet);
    // 
    // 	bool				IsHaveCanActionFlag1( void )const;
    // 	void				SetCanActionFlag1( bool bSet );
    // 
    // 	bool				IsHaveCanActionFlag2( void )const;
    // 	void				SetCanActionFlag2( bool bSet );
    // 
    // 
    // 	// 设置二级保护密码 2006-3-27
    // 	bool				Get_IsMinorPwdSetup(  bool bOpenDlg = TRUE ) const;
    // 	void				Set_SetMinorPwdSetup( int nSet );
    // 
    // 	// 是否验证过二级保护密码 2006-3-27
    // 	bool				Get_IsMinorPwdUnlocked( bool bOpenDlg = TRUE ) const;
    // 	void				Set_SetMinorPwdUnlocked( int nSet );
    // 
    // 	//---------------------------------
    // 	const SKILLCLASS_MAP&		Get_SkillClass(void) const;
    public SCLIENT_SKILLCLASS Get_SkillClass(int nID)
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData.m_theSkillClass.ContainsKey(nID))
        {
            return mySelfData.m_theSkillClass[nID];
        }
        return null;
    }

    public Dictionary<int, SCLIENT_SKILLCLASS> Get_SkillClass()
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }

        return ((SDATA_PLAYER_MYSELF)m_pData).m_theSkillClass;
    }

    public void Set_SkillClass(int nID, int nLevel)
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }

        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (!mySelfData.m_theSkillClass.ContainsKey(nID))
        {
            _DBC_SKILL_XINFA pDefine = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_XINFA>((int)DataBaseStruct.DBC_SKILL_XINFA).Search_Index_EQU(nID);
            if (pDefine == null) return;
            _DBC_XINFA_REQUIREMENTS pRequirement = CDataBaseSystem.Instance.GetDataBase<_DBC_XINFA_REQUIREMENTS>((int)DataBaseStruct.DBC_XINFA_REQUIREMENTS).Search_Index_EQU(nID);
            if (pRequirement == null) return;

            SCLIENT_SKILLCLASS skillClass = new SCLIENT_SKILLCLASS();
            skillClass.m_pDefine = pDefine;
            skillClass.m_pRequire = pRequirement;
            skillClass.m_nPosIndex = mySelfData.m_theSkillClass.Count;
            skillClass.m_nLevel = nLevel;
            skillClass.m_bLeaned = true;
            //skillClass.UpdateLearnState();
            skillClass.UpdateLevelUpState();
            // 因为第一次获得技能的时候不需要广播技能可用，所以刷新后再设置标志位 [11/15/2011 Ivan edit]
            skillClass.enableLearnEvent = true;
            mySelfData.m_theSkillClass.Add(nID, skillClass);
        }
        else
        {
            mySelfData.m_theSkillClass[nID].m_nLevel = nLevel;
            //mySelfData.m_theSkillClass[nID].UpdateLearnState();
            mySelfData.m_theSkillClass[nID].UpdateLevelUpState();
        }

        // 当心法改变时应该改变技能
        Update_SkillByXinFaID(nID);

        //通知ActionSystem
        CActionSystem.Instance.SkillClass_Update();
    }

    public void Set_Skill(int nID, byte nLevel, bool bLean)
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }

        _DBC_SKILL_DATA pDefine = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).Search_Index_EQU((int)nID);
        if (pDefine == null) return;

        int nSkillID = nID * 100 + nLevel;
        if (nID == 0)
        {
            nSkillID = nID;
            nLevel = 0;
        }
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (!mySelfData.m_theSkill.ContainsKey(nSkillID))
        {
            int nDepleteID = MacroDefine.INVALID_ID;
            if (pDefine.m_nSkillClass != MacroDefine.INVALID_ID)
            {
                //是否是本门派的门派技能
              //  if (pDefine.m_nMenPai != this.Get_MenPai()) return;

                SCLIENT_SKILLCLASS pSkillClass = Get_SkillClass(pDefine.m_nSkillClass);
                // 境界心法会超出12级 [4/19/2012 SUN]
                if (pSkillClass != null)
                {
                    if (nLevel < 12)
                        nDepleteID = pDefine.m_anSkillByLevel[nLevel - 1];
                    else if (pSkillClass.IsJingJie())
                        nDepleteID = pDefine.m_anSkillByLevel[11];
                }
            }
            if (nDepleteID == MacroDefine.INVALID_ID)
                nDepleteID = pDefine.m_anSkillByLevel[0];

            _DBC_SKILL_DEPLETE pDeplete = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DEPLETE>((int)DataBaseStruct.DBC_SKILL_DEPLETE).Search_Index_EQU(nDepleteID);
            SCLIENT_SKILL newSkill = new SCLIENT_SKILL();
            newSkill.m_pDefine = pDefine;
            newSkill.m_pDeplete = pDeplete;
            newSkill.m_nLevel = nLevel;
            newSkill.m_nPosIndex = mySelfData.m_theSkill.Count;
            newSkill.m_IsActive  = newSkill.IsCanUse_CheckLevel(CObjectManager.Instance.getPlayerMySelf().ID, newSkill.m_nLevel) == OPERATE_RESULT.OR_OK;
            mySelfData.m_theSkill.Add(nSkillID, newSkill);
        }
        //通知ActionSystem
        CActionSystem.Instance.UserSkill_Update();
    }
     	// 更新技能状态 [10/18/2011 Ivan edit]
    public void UpdateAllSkillClassState()
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if(selfData == null)
            throw new NullReferenceException("Not mine ! in CharacterData: UpdateAllSkillClassState()");
        foreach (SCLIENT_SKILLCLASS skillClass in selfData.m_theSkillClass.Values)
        {
            skillClass.UpdateLevelUpState();
        }

	    // 更新技能界面 [10/19/2011 Ivan edit]
	    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SKILL_UPDATE);
    }

    public void UpdateAllSkillState()
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("Not mine ! in CharacterData: UpdateAllSkillState()");
        
        foreach (SCLIENT_SKILL skill in selfData.m_theSkill.Values)
        {
            if (skill.IsCanUse_CheckLevel(CObjectManager.Instance.getPlayerMySelf().ID, skill.m_nLevel) == OPERATE_RESULT.OR_OK)
            {
                if (!skill.m_IsActive &&
                    skill.m_pDefine.m_nMenPai >= 0 && skill.m_pDefine.m_nMenPai <= 3)//职业技能被激活
                {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_SKILL,skill.m_pDefine.m_nID * 100 + skill.m_nLevel);
                }
                skill.m_IsActive = true;
            }
            else
            {
                skill.m_IsActive = false;
            }
        }
        // 更新技能界面 [10/19/2011 Ivan edit]
    }
     
     
    public Dictionary<int, SCLIENT_SKILL> Get_Skill()
    {
        return ((SDATA_PLAYER_MYSELF)m_pData).m_theSkill;
    }

    public SCLIENT_SKILL Get_Skill(int nID)
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }
        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;
        if (mySelfData.m_theSkill.ContainsKey(nID))
        {
            return mySelfData.m_theSkill[nID];
        }
        return null;
    }

    public void Skill_CleanAll()
    {
        ((SDATA_PLAYER_MYSELF)m_pData).m_theSkill.Clear();
        //通知ActionSystem
        CActionSystem.Instance.UserSkill_Update();
    }

    public void Update_SkillByXinFaID(int nXinFaID)
    {
        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        {
            throw new System.Exception("Character must not CT_MONSTER");
        }

        if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        {
            throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        }

        SDATA_PLAYER_MYSELF mySelfData = (SDATA_PLAYER_MYSELF)m_pData;

        foreach (KeyValuePair<int, SCLIENT_SKILL> skillPair in mySelfData.m_theSkill)
        {
            SCLIENT_SKILL skill = skillPair.Value;
            if (skill.m_pDefine.m_nSkillClass == nXinFaID)
            {
                int nDepleteID = MacroDefine.INVALID_ID;
                if (skill.m_pDefine.m_nSkillClass != MacroDefine.INVALID_ID)
                {
                    SCLIENT_SKILLCLASS pClass = Get_SkillClass(skill.m_pDefine.m_nSkillClass);
                    if (pClass != null)
                    {
                        int nLevel = pClass.m_nLevel/* / 10*/;
                        if (nLevel >= 0 && nLevel < 12)
                            nDepleteID = skill.m_pDefine.m_anSkillByLevel[nLevel];
                        else if (pClass.IsJingJie())// 境界心法超出12级 [4/19/2012 SUN]
                            nDepleteID = skill.m_pDefine.m_anSkillByLevel[11];
                    }
                }
                if (nDepleteID == MacroDefine.INVALID_ID)
                {
                    nDepleteID = skill.m_pDefine.m_anSkillByLevel[0];
                }

                if (nDepleteID != MacroDefine.INVALID_ID)
                {
                    _DBC_SKILL_DEPLETE pDeplete = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DEPLETE>((int)DataBaseStruct.DBC_SKILL_DEPLETE).Search_Index_EQU(nDepleteID);
                    skill.m_pDeplete = pDeplete;
                }
            }
        }
        //通知ActionSystem
        CActionSystem.Instance.UserSkill_Update();
    }
    // 
    public  Dictionary<int, SCLIENT_LIFEABILITY> Get_LifeAbility()
    {
        return (((SDATA_PLAYER_MYSELF)m_pData).m_theLifeAbility);
    }

    public SCLIENT_LIFEABILITY Get_LifeAbility(int nID)
    {
        if ((((SDATA_PLAYER_MYSELF)m_pData).m_theLifeAbility).ContainsKey(nID))
        {
            return (((SDATA_PLAYER_MYSELF)m_pData).m_theLifeAbility)[nID];
        }
        else
            return null;
    }

    public void Set_LifeAbility(short nID, int nLevel, int nExp)
    {
        Dictionary<int, SCLIENT_LIFEABILITY> mapLifeAbility = ((SDATA_PLAYER_MYSELF)m_pData).m_theLifeAbility;

	    bool find  = mapLifeAbility.ContainsKey(nID);

	    if ( nExp > 0 )
	    {
		    nExp /= 100; // 100 以下的部分只用于服务器端程序内部计算
	    }

	    //新的生活技能
	    if(find ==false)
	    {
		    //查找心法定义
            _DBC_LIFEABILITY_DEFINE pDefine= CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_DEFINE>((int)DataBaseStruct.DBC_LIFEABILITY_DEFINE).Search_Index_EQU(nID);

		    SCLIENT_LIFEABILITY newLifeAbility = new SCLIENT_LIFEABILITY();
		    newLifeAbility.m_pDefine = pDefine;
		    newLifeAbility.m_nPosIndex = mapLifeAbility.Count;
		    newLifeAbility.m_nLevel = nLevel;

		    if ( nExp < 0 )
		    {
			    nExp = 0;
		    }

		    newLifeAbility.m_nExp = nExp;

		    mapLifeAbility[nID] =  newLifeAbility;
	    }
	    //已有心法
	    else
	    {
            SCLIENT_LIFEABILITY lifeAbility = mapLifeAbility[nID];
		    if(nLevel >= 0) lifeAbility.m_nLevel = nLevel;
		    if(nExp >= 0) lifeAbility.m_nExp = nExp;
	    }

	    //通知ActionSystem
	    CActionSystem.Instance.UserLifeAbility_Update();

    //	((CObject_PlayerMySelf*)m_pObjRef).OnDataChanged_LifeAbility(nID);
}
     
     //const	SPRESCR_MAP&			Get_Prescr(void) const;
    public Dictionary<int, SCLIENT_PRESCR> Get_Prescr()
    {
        SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
        if (selfData == null)
            throw new NullReferenceException("CharacterData:Get_Prescr()");

        return selfData.m_theSprescr;
    }
     	public SCLIENT_PRESCR		Get_Prescr(int nID)
        {
            SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
            if (selfData == null)
                throw new NullReferenceException("CharacterData:Get_Prescr(): id=" + nID);

            SCLIENT_PRESCR prescr = null;
            if(selfData.m_theSprescr.TryGetValue(nID, out prescr))
            {
                return prescr;
            }
            return null;

        }
        DBC.COMMON_DBC<_DBC_LIFEABILITY_ITEMCOMPOSE> prescrDBC;
        public DBC.COMMON_DBC<_DBC_LIFEABILITY_ITEMCOMPOSE> PrescrDBC
        {
            get {
                if (prescrDBC == null)
                    prescrDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_ITEMCOMPOSE>((int)DataBaseStruct.DBC_LIFEABILITY_ITEMCOMPOSE);
                return prescrDBC;
            }
        }

     	public void	Set_Prescr(int nID, bool bLean)
        {
            SDATA_PLAYER_MYSELF selfData = m_pData as SDATA_PLAYER_MYSELF;
            if (selfData == null)
                throw new NullReferenceException("CharacterData:Set_Prescr(): id=" + nID);

            SCLIENT_PRESCR prescr = null;
            if(!bLean && selfData.m_theSprescr.TryGetValue(nID, out prescr))
            {
                selfData.m_theSprescr.Remove(nID);
            }
            else
            {
                _DBC_LIFEABILITY_ITEMCOMPOSE pDefine = PrescrDBC.Search_Index_EQU(nID);
                if(pDefine == null)
                    throw new NullReferenceException("Can not found Prescr:"+nID);

                SCLIENT_PRESCR newSprscr = new SCLIENT_PRESCR();
                newSprscr.m_pDefine = pDefine;
                selfData.m_theSprescr.Add(nID, newSprscr);
            }
   
        }
     
     	//  [10/8/2011 Sun]
     	//SPRESCR_MAP&				Get_PrescrMap(int nLifeAbility) const;
     	//const SCLIENT_PRESCR*		Get_PrescrNew(int nID) const;
    // 暂时不写逻辑
        public bool Get_IsInStall()
        {
            return false;
        }				
    // 	void						Set_IsInStall(bool bIsInStall);
    // 
    // 	string						Get_StallName(void) const;		
    // 	void						Set_StallName(string szName);
    // 
    public float Get_SpeedRate() { return m_fSpeedRate; }
    // 
    // 	// 势力相关 [9/26/2011 Ivan edit]
    // 	int							GetShiLi(void)const { return shiLiId; }
    // 	void						SetShiLi(int slId);
    // 

    // 
    // protected:

    // 	void				_Init_AsSpecialBus(void);// 新增载具 [8/22/2011 ivan edit]
    // 
    protected void _On_MenPaiChanged()
    {
        //if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
        //{
        //    throw new System.Exception("Character must not CT_MONSTER");
        //}

        //if (m_pObjRef.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER)
        //{
        //    throw new System.Exception("Character must not CT_PLAYEROTHER1`");
        //}


        ////清空旧门派技能
        //((SDATA_PLAYER_MYSELF)m_pData).m_theSkill.Clear();
        ////重新加载技能表
        //int nNum = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).m_iDataNum;
        //for(int i = 0; i < nNum; i++)
        //{
        //    _DBC_SKILL_DATA pSkillDef = CDataBaseSystem.Instance.GetDataBase<_DBC_SKILL_DATA>((int)DataBaseStruct.DBC_SKILL_DATA).Search_Index_EQU(i);

        //    if(pSkillDef.m_nClassByUser != (int)ENUM_SKILL_CLASS_BY_USER_TYPE.A_SKILL_FOR_PLAYER) continue; //不是玩家用的技能

        //    if(pSkillDef.m_nMenPai == Get_MenPai() || MacroDefine.INVALID_ID == pSkillDef.m_nMenPai)
        //    {
        //        //属于该门派的技能 或是 通用技能
        //        int nActiveTime = (pSkillDef.m_nActiveTime==-1?-1:0);
        //        Set_Skill(pSkillDef.m_nID, 1, false);
        //    }
        //}
        ////待实现
        ////设置激活技能为物理攻击
        //CObjectManager.Instance.getPlayerMySelf().SetActiveSkill(CActionSystem.Instance.GetAction_SkillID(0));
        //CGameInterface::GetMe().Skill_SetActive(CActionSystem::GetMe().GetAction_SkillID(0));

    }
    // 
    protected void _On_GemChanged(HUMAN_EQUIP point)
    {
        //     UINT uGemType = Get_EquipGem(point);
        // 
        // 	//宝石特效
        // 	INT nGemCount = 0;
        // 	BYTE bMaxInlaid = 0;
        // 	BYTE nMaxLevel = 0;
        // 	for (int i = 0; i < 3; i++)
        // 	{
        // 		BYTE binlaid = (BYTE)((uGemType>>(22-(i<<3))) &0x03);//state
        // 		if(binlaid > bMaxInlaid)
        // 		{
        // 			bMaxInlaid = binlaid;
        // 			nGemCount = 1;
        // 			nMaxLevel = (BYTE)((uGemType  >>(16-(i<<3))) &0x3F);
        // 		}
        // 		else if(binlaid == bMaxInlaid)
        // 		{
        // 			BYTE bLevel = (BYTE)((uGemType  >>(16-(i<<3))) &0x3F);
        // 			if(bLevel > nMaxLevel)
        // 			{
        // 				nMaxLevel = bLevel;
        // 				nGemCount = 1;
        // 			}
        // 			else if(bLevel == nMaxLevel && bLevel != 0)
        // 				nGemCount++;
        // 		}
        // 	}
        // 	BYTE InaidState = bMaxInlaid == 2 ? 1 : 0;
        // 	BYTE HighLevel  = nMaxLevel > 3 ? 1 : 0;
        // 
        // 	// 镶嵌，宝石等级，宝石数量
        // 	static const BYTE EffectIndex[2][2][3] = {
        // 		{//镶嵌状态 
        // 			{//宝石等级
        // 				{1}, {1}, {1} 
        // 			},
        // 			{
        // 				{1}, {1}, {1}
        // 			}
        // 		}, 
        // 		{ 
        // 			{ 
        // 				{2}, {3}, {4}
        // 			},
        // 			{ 
        // 				{5}, {6}, {7}
        // 			} 
        // 		}
    }
    // 
    // protected:
    // 	void				InitDefaultSkill(int menPai);

    //角色逻辑指针
    protected CObject_Character m_pObjRef;
    //角色数据结构
    protected SDATA_CHARACTER m_pData;
    //改变角色频率相关
    protected float m_fSpeedRate;

}
