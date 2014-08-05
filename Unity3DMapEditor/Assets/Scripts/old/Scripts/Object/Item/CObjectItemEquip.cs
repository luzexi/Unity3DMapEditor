using System;
using System.Collections.Generic;

// namespace SGWEB
// {
public class CObject_Item_Equip : CObject_Item

{
    public CObject_Item_Equip(int id)
        : base(id)
    {

    }

    public override ITEM_CLASS GetItemClass()
    {
        return ITEM_CLASS.ICLASS_EQUIP;
    }
    //物品的名称
	public override string GetName()
    {
		if(m_theBaseDef != null)
        	return m_theBaseDef.szName;
		else
			return "";
    }
    //物品解释
    public override string GetDesc()
    {
		if(m_theBaseDef != null)
        	return m_theBaseDef.szDesc;
		else
			return "";
    }
    //图标名
    public override string GetIconName()
    {
		if(m_theBaseDef != null)
        	return m_theBaseDef.szIcon;
		else
			return "";
    }
    //详细解释(可能需要服务器)
    public override string GetExtraDesc()
    {
        if (m_pExtraDefine == null)
        {
            return null;
        }
        // 得到兰装的描述信息.
        if (m_theBaseDef != null)
        {
            m_strExtraDesc = m_theBaseDef.szDesc;
        }
        switch ((EQUIP_QUALITY)m_pExtraDefine.m_nEquipQulity)
        {
            case EQUIP_QUALITY.WHITE_EQUIP:
                break;
            case EQUIP_QUALITY.GREEN_EQUIP:
                break;
            case EQUIP_QUALITY.BLUE_EQUIP:
                break;
            case EQUIP_QUALITY.YELLOW_EQUIP:
                break;
            case EQUIP_QUALITY.PURPLE_EQUIP:
                break;
            default:
                m_strExtraDesc = "";
                break;
        }

        return m_strExtraDesc;
    }
    //设置详细解释
    public override void SetExtraInfo(ref _ITEM pItemInfo)
    {
        if(m_pExtraDefine == null)
		    m_pExtraDefine = new EXTRA_DEFINE();

	    m_pExtraDefine.m_CurDurPoint = pItemInfo.GetEquipData().m_CurDurPoint;
	    m_pExtraDefine.m_MaxDurPoint = pItemInfo.GetEquipData().m_MaxDurPoint;
	    m_pExtraDefine.m_nLevelNeed  = pItemInfo.GetEquipData().m_NeedLevel;
    //	m_pExtraDefine.m_nFrogLevel  = pItemInfo.GetEquipData().m_Level;
	    m_pExtraDefine.m_nRepairFailureTimes  = pItemInfo.GetEquipData().m_FaileTimes;
	    m_pExtraDefine.m_nSellPrice  = (int)pItemInfo.GetEquipData().m_BasePrice;
	    // 可以镶嵌的宝石个数
	    m_pExtraDefine.m_EnableGemCount = pItemInfo.GetEquipData().m_GemMax;
	    // 物品强化等级/品级属性/魂印属性[10/10/2011 edit by ZL]
	    m_pExtraDefine.m_EquipEnhanceLevel = pItemInfo.GetEquipData().m_EquipEnhanceLevel;
	    m_pExtraDefine.m_PrintSoulType = pItemInfo.GetEquipData().m_CurSoulType;

	    // 得到鉴定信息。
	    if(pItemInfo.GetItemIdent())
	    {
		    // 已经鉴定
		    m_EquipAttrib = EQUIP_ATTRIB.EQUIP_ATTRIB_IDENTIFY;
	    }
	    else
	    {
		    // 未鉴定
		    m_EquipAttrib = EQUIP_ATTRIB.EQUIP_ATTRIB_UNIDENTIFY;
	    }

	    // 得到绑定信息。
	    if(pItemInfo.GetItemBind())
	    {
		    m_pExtraDefine.M_nEquipBindInfo = EQUIP_BIND_INFO.BINDED;
	    }
	    else
	    {
		    if(Rule(ITEM_RULE.RULE_PICKBIND))
			    m_pExtraDefine.M_nEquipBindInfo = EQUIP_BIND_INFO.GETUP_BIND;
		    else if(Rule(ITEM_RULE.RULE_EQUIPBIND))
			    m_pExtraDefine.M_nEquipBindInfo = EQUIP_BIND_INFO.EQUIP_BIND;

	    }

	    // m_vEquipAttributes存放6个的基础属性， m_vBlueEquipAttributes存放所有的扩展属性 [9/22/2011 edit by ZL]
	    m_pExtraDefine.m_vBlueEquipAttributes.Clear();
	    m_pExtraDefine.m_vEquipAttributes.Clear();

	    // 临时将属性划分  0-5基础属性  6-20扩展属性  21-27魂印属性 [10/10/2011 edit by ZL]
	    int BASE_ATTR_START = 0;
	    int EXTER_ATTR_START = DBC_DEFINE.MAX_BASE_ATTR;
	    int PRINT_SOUL_ATTR_START = DBC_DEFINE.MAX_BASE_ATTR +DBC_DEFINE. MAX_ADD_ATTR - DBC_DEFINE.MAX_SOUL_ATTR;
	    int MAX_ATTR_NUM = DBC_DEFINE.MAX_BASE_ATTR + DBC_DEFINE.MAX_ADD_ATTR;

	    for(int i=0; i<pItemInfo.GetEquipData().m_AttrCount; i++)
	    {
		    _ITEM_ATTR att = pItemInfo.GetEquipData().m_pAttr[i];
            //if(att.m_AttrType >= BASE_ATTR_START && att.m_AttrType < EXTER_ATTR_START) {
            //    m_pExtraDefine.m_vEquipAttributes.Add(att);
            //}
            //else if(att.m_AttrType >= EXTER_ATTR_START && att.m_AttrType < PRINT_SOUL_ATTR_START){
            //    m_pExtraDefine.m_vBlueEquipAttributes.Add(att);
            //} 
            //else if(att.m_AttrType >= PRINT_SOUL_ATTR_START && att.m_AttrType < MAX_ATTR_NUM){
            //    m_pExtraDefine.m_PrintSoulAttribute = att;
            //}

            // 装备属性不再区分对待，统一放在一个地方 [3/29/2012 Ivan]
            if (att.m_AttrType >= BASE_ATTR_START && att.m_AttrType < MAX_ATTR_NUM)
            {
                m_pExtraDefine.m_vEquipAttributes.Add(att);
            }
	    }

	    // 清空宝石信息
	    m_pExtraDefine.m_vEquipAttachGem.Clear();
	    // 宝石信息设置 [9/28/2011 edit by ZL]
	    for(int i=0; i < GAMEDEFINE.MAX_ITEM_GEM; i++)
	    {
		     m_pExtraDefine.m_vEquipAttachGem.Add(pItemInfo.GetEquipData().m_pGemInfo[i]);
	    }

	    SetNumber(pItemInfo.GetItemCount());
	    SetManufacturer(ref pItemInfo);

	    // 因为服务器在发送装备属性的时候，把宝石的附加属性也加在了里面，所以需要删除掉 [10/25/2011 Ivan edit]
	    ClearGemAttrInBlueAttrs();
    }

    //得到显示数据ID
    public int GetVisualID()
    {
        return m_theBaseDef.nVisualID;
    }
    //查询逻辑属性,一般用于脚本调用
    public override string GetAttributeValue(string szValueName)
    {
        if(szValueName==null || m_pExtraDefine == null) 
            return "";

	    string szTemp;

	    if(string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_LEVEL) == 0)
	    {
		    if(m_pExtraDefine != null)
		    {
                szTemp =  System.String.Format("{0}", m_pExtraDefine.m_nLevelNeed);
			    return szTemp;
		    }
	    }
        else if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_DAM) == 0)
	    {
		    if(m_pExtraDefine != null)
		    {
                szTemp = System.String.Format("{0}", m_pExtraDefine.m_CurDurPoint);
			    return szTemp;
		    }
	    }
        else if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_DAMMAX) == 0)
	    {
		    if(m_pExtraDefine != null)
		    {
                szTemp = System.String.Format("{0}", m_pExtraDefine.m_MaxDurPoint);
			    return szTemp;
		    }
	    }
        else if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_PRICE) == 0)
	    {
		    if(m_pExtraDefine != null)
		    {
                szTemp = System.String.Format("{0}", m_pExtraDefine.m_nSellPrice);
			    return szTemp;
		    }
	    }
        else if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_REPAIRNUM) == 0)
	    {
		    if(m_pExtraDefine!=null)
		    {
                szTemp = System.String.Format("{0}", m_pExtraDefine.m_nRepairFailureTimes);
			    return szTemp;
		    }
	    }
        else if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_WHITE_ATT) == 0)
	    {
		    if(m_pExtraDefine!=null)
		    {
			    string strValue="";
			    GetAllWhiteAttribute(ref strValue);
			    return strValue;
		    }
	    }
        else if (string.Compare(szValueName, ITEMATTRIBUTE.ITEMATTRIBUTE_BLUE_ATT) == 0)
	    {
		    if(m_pExtraDefine!=null)
		    {
			    string strValue="";
			    GetAllBlueAttribute(ref strValue);
			    return strValue;
		    }
        }

	    return base.GetAttributeValue(szValueName);
    }
    //克隆详细信息
    public override void Clone(CObject_Item pItemSource)
    {
        ClonetExtraInfo((CObject_Item_Equip)pItemSource);
	    base.Clone(pItemSource);
    }
    public void CloneForUpLevel(CObject_Item_Equip pItemSource)
    {
        //克隆额外属性
        if (m_pExtraDefine != null)
            m_pExtraDefine = new EXTRA_DEFINE();

        EXTRA_DEFINE pNewExtraDefine = pItemSource.GetExtraInfo();

        m_pExtraDefine.m_CurDurPoint = pNewExtraDefine.m_CurDurPoint;
        m_pExtraDefine.m_MaxDurPoint = pNewExtraDefine.m_MaxDurPoint;
        m_pExtraDefine.m_nLevelNeed = pNewExtraDefine.m_nLevelNeed;
        //	m_pExtraDefine.m_nFrogLevel  = pItemInfo.GetEquipData().m_Level;
        m_pExtraDefine.m_nRepairFailureTimes = pNewExtraDefine.m_nRepairFailureTimes;
        m_pExtraDefine.m_nSellPrice = pNewExtraDefine.m_nSellPrice;
        m_pExtraDefine.M_nEquipBindInfo = pNewExtraDefine.M_nEquipBindInfo;
        // 增加克隆时候的可以镶嵌的宝石个数、强化等级 [9/14/2011 edit by ZL]
        m_pExtraDefine.m_EnableGemCount = pNewExtraDefine.m_EnableGemCount;
        m_pExtraDefine.m_EquipEnhanceLevel = pNewExtraDefine.m_EquipEnhanceLevel;

        m_pExtraDefine.m_vEquipAttachGem.Clear();
        m_pExtraDefine.m_vBlueEquipAttributes.Clear();
      

        for (int i = 0; i < pNewExtraDefine.m_vEquipAttachGem.Count; i++)
        {
            m_pExtraDefine.m_vEquipAttachGem.Add(pNewExtraDefine.m_vEquipAttachGem[i]);
        }

        for (int i = 0; i < pNewExtraDefine.m_vBlueEquipAttributes.Count; i++)
        {
            m_pExtraDefine.m_vBlueEquipAttributes.Add(pNewExtraDefine.m_vBlueEquipAttributes[i]);
        }

        //升档回退强化
        if (m_pExtraDefine.m_EquipEnhanceLevel >= 5)
            m_pExtraDefine.m_EquipEnhanceLevel -= 5;

        //基础属性根据新的强化等级计算
        int nEnchanceIndex = m_theBaseDef.nEquipPoint * 110 + m_pExtraDefine.m_EquipEnhanceLevel;

        for (int i = 0; i < pNewExtraDefine.m_vEquipAttributes.Count; i++)
        {
            _ITEM_ATTR value = pNewExtraDefine.m_vEquipAttributes[i];
            if (value.m_Value.m_Value > 0)
            {
                value.m_Value.m_Value = (short)(m_theBaseDef.nBaseAttr[value.m_AttrType] * CaclStrendthValue(value.m_AttrType, m_theBaseDef.nEquipPoint, m_pExtraDefine.m_EquipEnhanceLevel) + 0.5f);
            }
            m_pExtraDefine.m_vEquipAttributes.Add(value);
        }
        SetNumber(pItemSource.GetNumber());

        m_Quality = pItemSource.GetEquipQuantity();

        m_EquipAttrib = pItemSource.m_EquipAttrib;

        base.Clone(pItemSource);
        
    }
    //获得物品的最大叠加数量
    public override int GetMaxOverLay() { return 1; }

    // 获得装备的修理价格
    public int GetRepairPrice()
    {
        return 0;
    }

    //--------------------------------------------------------------------------------------------------------------
    //-- supertooltip 使用
    //

    // 得到玩家使用这个物品需要的等级
    public override int GetNeedLevel()
    {
        if (m_Quality != EQUIP_QUALITY.INVALID_EQUIP)
            return m_theBaseDef.nLevelRequire;
        return 0;
    }

    // 得到物品耐久度
    public override int GetItemDur()
    {
        return GetDur();
    }

    // 得到物品最大耐久度
    public override int GetItemMaxDur()
    {
        return GetMaxDur();
    }

    // 得到物品的修理次数 
    public override int GetItemRepairCount()
    {
        return m_pExtraDefine.m_nRepairFailureTimes;
    }

    // 得到物品的强化等级  [7/28/2011 edit by ZL] 
    public override int GetStrengthLevel()
    {
        return m_pExtraDefine.m_EquipEnhanceLevel;
    }
    //升级到下一等级的消耗索引
    public int GetStrengthIndex()
    {
        if (GetStrengthLevel() == GetMaxStrengthLevel())
            return -1;
        return (int)GetItemType() * 110 + GetStrengthLevel() + 1;
    }
    public int GetMaxStrengthLevel()
    {
        return 110;
    }


    // 得到物品的绑定信息 
    public override int GetItemBindInfo()
    {
        // -1 无绑定信息
        if (m_pExtraDefine!=null)
        {
            return (int)m_pExtraDefine.M_nEquipBindInfo;
        }
        else
        {
            return -1;
        }
    }

    // 得到物品卖给npc的价格
    public override int GetItemPrice()
    {
        EXTRA_DEFINE pInfo = GetExtraInfo();
        if (pInfo != null)
        {
            return pInfo.m_nSellPrice;
        }
        return -1;
    }

    // 得到物品的制作人
    public override string GetManufacturer()
    {
        return base.GetManufacturer();
    }

    // 得到白色属性
    public override string GetBaseWhiteAttrInfo()
    {
        string strWhiteValue="";
	    GetAllWhiteAttribute(ref strWhiteValue);

	    return strWhiteValue;
    }
    // 得到物品的类型描述2006-4-28
    public override string GetItemTableTypeDesc()
    {
        if (m_Quality != EQUIP_QUALITY.INVALID_EQUIP)
            return m_theBaseDef.szTypeDesc;
        return "";
    }


    //得到套装编号
    public int GetSetID()
    {
        //  [11/4/2010 Sun]
        if (m_Quality == EQUIP_QUALITY.INVALID_EQUIP)
            return -1;

        return m_theBaseDef.nSetID;
    }
    //得到武器类型(如果是非武器，返回WEAPON_TYPE_NONE)
    public ENUM_WEAPON_TYPE GetWeaponType()
    {
	    WEAPON_TYPE nDefine = (WEAPON_TYPE)m_theBaseDef.nType;

	    //转化为ENUM_WEAPON_TYPE
        switch (nDefine)
        {
            //case WTYPE_DAO:		return WEAPON_TYPE_LONG;
            case WEAPON_TYPE.WTYPE_QIANG:
                return ENUM_WEAPON_TYPE.WEAPON_TYPE_SPEAR;
            case WEAPON_TYPE.WTYPE_1DUAN:
                return ENUM_WEAPON_TYPE.WEAPON_TYPE_SHORT;
            case WEAPON_TYPE.WTYPE_2DUAN:
                return ENUM_WEAPON_TYPE.WEAPON_TYPE_DSHORT;
            case WEAPON_TYPE.WTYPE_BOW:
                return ENUM_WEAPON_TYPE.WEAPON_TYPE_BOW;
            case WEAPON_TYPE.WTYPE_STAFF:
                return ENUM_WEAPON_TYPE.WEAPON_TYPE_STAFF;
            default:
                return ENUM_WEAPON_TYPE.WEAPON_TYPE_NONE;
        }
    }
    //设置装备当前耐久度对于一个已经生成的装备，只有这个值会被改变
    public void SetDur(int Dur)
    {
        m_pExtraDefine.m_CurDurPoint = Dur;
    }
    //获得当前耐久度
    public int GetDur()
    {
        return m_pExtraDefine.m_CurDurPoint;
    }
    //获得最大耐久度
    public int GetMaxDur()
    {
        return m_pExtraDefine.m_MaxDurPoint;
    }
    //克隆详细信息
    public void ClonetExtraInfo(CObject_Item_Equip pItemSource)
    {
        if(m_pExtraDefine!=null)
		    m_pExtraDefine = new EXTRA_DEFINE();

	    EXTRA_DEFINE pNewExtraDefine = pItemSource.GetExtraInfo();

	    m_pExtraDefine.m_CurDurPoint = pNewExtraDefine.m_CurDurPoint;
	    m_pExtraDefine.m_MaxDurPoint = pNewExtraDefine.m_MaxDurPoint;
	    m_pExtraDefine.m_nLevelNeed  = pNewExtraDefine.m_nLevelNeed;
	    //	m_pExtraDefine.m_nFrogLevel  = pItemInfo.GetEquipData().m_Level;
	    m_pExtraDefine.m_nRepairFailureTimes  = pNewExtraDefine.m_nRepairFailureTimes;
	    m_pExtraDefine.m_nSellPrice  = pNewExtraDefine.m_nSellPrice;
	    m_pExtraDefine.M_nEquipBindInfo = pNewExtraDefine.M_nEquipBindInfo;
	    // 增加克隆时候的可以镶嵌的宝石个数、强化等级 [9/14/2011 edit by ZL]
	    m_pExtraDefine.m_EnableGemCount = pNewExtraDefine.m_EnableGemCount;
	    m_pExtraDefine.m_EquipEnhanceLevel = pNewExtraDefine.m_EquipEnhanceLevel;		

	    m_pExtraDefine.m_vEquipAttachGem.Clear();
	    m_pExtraDefine.m_vBlueEquipAttributes.Clear();
	    m_pExtraDefine.m_vEquipAttributes.Clear();

	    for(int i=0; i<pNewExtraDefine.m_vEquipAttachGem.Count; i++)
	    {
		    m_pExtraDefine.m_vEquipAttachGem.Add(pNewExtraDefine.m_vEquipAttachGem[i]);
	    }

	    for(int i=0; i<pNewExtraDefine.m_vBlueEquipAttributes.Count; i++)
	    {
		    m_pExtraDefine.m_vBlueEquipAttributes.Add(pNewExtraDefine.m_vBlueEquipAttributes[i]);
	    }
	    // 增加基础属性克隆 [9/14/2011 edit by ZL]
	    for(int i=0; i<pNewExtraDefine.m_vEquipAttributes.Count; i++)
	    {
		    m_pExtraDefine.m_vEquipAttributes.Add(pNewExtraDefine.m_vEquipAttributes[i]);
	    }
	    SetNumber(pItemSource.GetNumber());

	    m_Quality = pItemSource.GetEquipQuantity();
	    m_theBaseDef = pItemSource.GetEquipBaseDefine();

	    // 装备是否鉴定过属性。2006－4－10。
	    m_EquipAttrib = pItemSource.m_EquipAttrib;
    }
    //获得详细信息
    public EXTRA_DEFINE GetExtraInfo() { return m_pExtraDefine; }

    //得到装备点
    public HUMAN_EQUIP GetItemType()
    {
        return (HUMAN_EQUIP)(m_theBaseDef.nEquipPoint);
    }
    //装备是否宠物专用
    public bool IsPetEquipt()
    {
        return (m_theBaseDef.nMenPai == -2);
    }
    // 格式化属性字符串
    public void SetAttri(ref _ITEM_ATTR att, ref string pszBuf)
    {
        switch (att.m_AttrType)
        {
            case 1: goto case 39;
            //case 2:goto case 39;
            case 4: goto case 39;
            case 20: goto case 39;
            case 21: goto case 39;
            case 23: goto case 39;
            case 24: goto case 39;
            case 27: goto case 39;
            case 28: goto case 39;
            case 30: goto case 39;
            case 31: goto case 39;
            case 33: goto case 39;
            case 34: goto case 39;
            case 39:
                {
                    if (att.m_Value.m_Value > 0)
                    {
                        pszBuf = String.Format("#{0}+{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
                    }
                    else
                    {
                        pszBuf = String.Format("#{0}-{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
                    }
                    break;
                }
            case 38:
                {
                    pszBuf = String.Format("{0}#{1}\n", att.m_Value.m_Value, g_szAttachAttName[att.m_AttrType]);
                    break;
                }
            case 40: goto case 50;
            case 41: goto case 50;
            case 49: goto case 50;
            case 50:
                {
                    pszBuf = String.Format("#{0}{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
                    break;
                }
            case 8: goto case 32;
            case 11: goto case 32;
            case 14: goto case 32;
            case 17: goto case 32;
            case 25: goto case 32;
            case 32:
                {
                    if (att.m_Value.m_Value > 0)
                    {
                        pszBuf = String.Format("#{0}-{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
                    }
                    else
                    {
                        pszBuf = String.Format("#{0}+{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
                    }
                    break;

                }

            default:
                {
                    if (att.m_Value.m_Value > 0)
                    {
                        pszBuf = String.Format("#{%s}+%d\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
                    }
                    break;
                }
        }
    }

    // 得到扩展蓝色属性
    public string GetExtBlueAttrInfo()
    {
        string strValue="";
	    GetAllBlueAttribute(ref strValue);

        return strValue;
    }

    // 得到扩展绿色属性
    public string GetGreenAttrInfo()
    {
        string strValue = "";
	    GetAllGreenAttribute(ref strValue);

        return strValue;
    }

    // 得到套装属性
    public string GetSuitAttrInfo()
    {
        string strValue = "";
	    GetAllSuitAttribute(ref strValue);

        return strValue;
    }

    // 得到宝石的个数.
    public int GetGemMaxCount()
    {
        return m_pExtraDefine.m_EnableGemCount;
    }
    ////得到当前镶嵌数量
    //public int GetCurrentGemCount()
    //{
    //    return m_pExtraDefine.m_vEquipAttachGem.Count;
    //}

    // 得到宝石的图标
    public void GetGemIcon(int iGemIndex, ref string strIconName)
    {
        if(null == m_pExtraDefine)
	    {
		    strIconName = "";
		    return;
	    }
    	
	    int iGemCount = m_pExtraDefine.m_vEquipAttachGem.Count;
	    if(iGemIndex >= iGemCount)
	    {
		    strIconName = "";
		    return;
	    }

        ////打开宝石数据表
        //DBC_DEFINEHANDLE(s_pItem_Gem, DBC_ITEM_GEM);
        ////搜索纪录
        //UINT iGemTableIndex = GetGemTableId(iGemIndex);
        //const _DBC_ITEM_GEM* pGem = (const _DBC_ITEM_GEM*)s_pItem_Gem->Search_Index_EQU(iGemTableIndex);
    	
        //if(!pGem)
        //{
        //    strIconName = _T("");
        //    return;
        //}

        //strIconName = pGem->szIcon;
    }

    // 得到宝石的名字以及镶嵌状态 [9/26/2011 edit by ZL] s
    public void GetGemNameAndState(int iGemIndex, ref string strNameState)
    {
        //if(NULL == m_pExtraDefine || iGemIndex >= (INT)m_pExtraDefine->m_vEquipAttachGem.size())
        //{
        //    strNameState = _T("未镶嵌");
        //    return;
        //}

        ////打开宝石数据表
        //DBC_DEFINEHANDLE(s_pItem_Gem, DBC_ITEM_GEM);
        ////搜索纪录
        //UINT iGemTableIndex = GetGemTableId(iGemIndex);
        //UINT iGemEnchaseState = GetGemEnchaseState(iGemIndex);
        //const _DBC_ITEM_GEM* pGem = (const _DBC_ITEM_GEM*)s_pItem_Gem->Search_Index_EQU(iGemTableIndex);

        //if(!pGem || iGemEnchaseState > 2)
        //{
        //    strNameState = _T("未镶嵌");
        //    return;
        //}

        //strNameState = STRING(pGem->szName) + STRING(g_szGemEnchaseName[iGemEnchaseState]);
    }

    // 得到宝石镶嵌状态 [9/26/2011 edit by ZL]
    //public int GetGemEnchaseState(int iGemIndex)
    //{
    //    if (null == m_pExtraDefine)
    //    {
    //        return 0;
    //    }

    //    int iGemCount = m_pExtraDefine.m_vEquipAttachGem.Count;
    //    if (iGemIndex >= iGemCount)
    //    {
    //        return 0;
    //    }

    //    return (int)(m_pExtraDefine.m_vEquipAttachGem[iGemIndex].m_GemType >> 30);
    //}

    // 得到宝石表ID [9/26/2011 edit by ZL]
    public int GetGemTableId(int iGemIndex)
    {
        if (null == m_pExtraDefine)
        {
            return 0;
        }

        int iGemCount = m_pExtraDefine.m_vEquipAttachGem.Count;
        if (iGemIndex >= iGemCount)
        {
            return 0;
        }

        return (int)(m_pExtraDefine.m_vEquipAttachGem[iGemIndex].m_GemType);
    }
    public int IsGemmy(int nType)
    {
        if (m_pExtraDefine == null)
            return -1;
        for (int i = 0; i < m_pExtraDefine.m_vEquipAttachGem.Count; i++ )
        {
            uint nGemType = m_pExtraDefine.m_vEquipAttachGem[i].m_GemType/1000;
            nGemType %= 100;
            if ( nGemType == nType)
                return i;
        }
        return -1;
    }

    // 得到宝石的附加功能
    public void GetGemExtAttr(int iGemIndex, ref string strGemAttr)
    {

        //搜索纪录
        int iGemTableIndex = GetGemTableId(iGemIndex);
        _DBC_ITEM_GEM pGem = ObjectSystem.GemDBC.Search_Index_EQU(iGemTableIndex);

        if (pGem == null) return;

        strGemAttr = string.Format("#{{{0}}} +{1}\n",g_szAttachAttName[pGem.m_GemAttrType], pGem.m_GemAttrValue);     
    }

    //获得最大修理次数
    public int GetMaxRepair()
    {
        return m_theBaseDef.nBMaxRepair;
    }

    //获得使用职业
    public int GetNeedJob()
    {
        return m_theBaseDef.nMenPai;
    }

    void ClearGemAttrInBlueAttrs()
    {
        //打开宝石数据表
        //DBC_DEFINEHANDLE(s_pItem_Gem, DBC_ITEM_GEM);

        //for(INT i=0; i < MAX_ITEM_GEM; i++)
        //{
        //    UINT iGemTableIndex = GetGemTableId(i);
        //    // 宝石编号不可能会小于等于0 [10/25/2011 Ivan edit]
        //    if (iGemTableIndex <= 0)
        //        continue;

        //    UINT iGemEnchaseState = GetGemEnchaseState(i);

        //    const _DBC_ITEM_GEM* pGem = (const _DBC_ITEM_GEM*)s_pItem_Gem->Search_Index_EQU(iGemTableIndex);
        //    if (!pGem)
        //        continue;

        //    UINT gemType = pGem->nAttr[iGemEnchaseState].m_GemAttrType;
        //    INT  gemValue = pGem->nAttr[iGemEnchaseState].m_GemAttrValue;

        //    for(INT i=0; i<(INT)m_pExtraDefine->m_vBlueEquipAttributes.size(); i++)
        //    {
        //        _ITEM_ATTR& att = m_pExtraDefine->m_vBlueEquipAttributes[i];

        //        if(att.m_AttrType == gemType && att.m_Value.m_Value == gemValue)
        //        {
        //            att.CleanUp();// 删除宝石的附加属性 [10/25/2011 Ivan edit]
        //        }
        //    }
        //}
    }

    //生成一件白色装备
    public void AsWhiteEquip(ref _DBC_ITEM_EQUIP_BASE pDefine)
    {
        this.AsEquipBase(pDefine, EQUIP_QUALITY.WHITE_EQUIP);
    }
    //生成一件绿色装备
    public void AsGreenEquip(ref _DBC_ITEM_EQUIP_BASE pDefine)
    {
        this.AsEquipBase(pDefine, EQUIP_QUALITY.GREEN_EQUIP);
    }
    //生成一件蓝色装备
    public void AsBlueEquip(ref _DBC_ITEM_EQUIP_BASE pDefine)
    {
        this.AsEquipBase(pDefine, EQUIP_QUALITY.BLUE_EQUIP);
    }

    //生成一件黄色装备
    public void AsYellowEquip(ref _DBC_ITEM_EQUIP_BASE pDefine)
    {
        this.AsEquipBase(pDefine, EQUIP_QUALITY.YELLOW_EQUIP);
    }
    //生成一件紫色装备
    public void AsPurpleEquip(ref _DBC_ITEM_EQUIP_BASE pDefine)
    {
        this.AsEquipBase(pDefine, EQUIP_QUALITY.PURPLE_EQUIP);
    }

    //得到所有白色属性
    // 现在所有属性都放在白装属性里面 [3/29/2012 Ivan]
    public void GetAllWhiteAttribute(ref string strValue)
    { 
        if(m_pExtraDefine==null)
	    {
		    strValue = "";
            return;
	    }

        string szAttachAtt = "";
        int MAX_ATTR_NUM = DBC_DEFINE.MAX_BASE_ATTR + DBC_DEFINE.MAX_ADD_ATTR;
	    // 用于UI显示的基础属性 [9/29/2011 edit by ZL]
	    for(int i=0; i<m_pExtraDefine.m_vEquipAttributes.Count; i++)
	    {
		     _ITEM_ATTR att = m_pExtraDefine.m_vEquipAttributes[i];
             if ((att.m_AttrType >= 0) && (att.m_AttrType <= MAX_ATTR_NUM))
		    {
			    string szAtt="";
			    if (att.m_Value.m_Value > 0)
                {
                     szAtt = String.Format("#{{{0}}}+{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);
			    }
                szAttachAtt += szAtt;
		    }
	    }

	    strValue = szAttachAtt;
    }
    //通过索引获得基础属性
    public override string GetWhiteAttribute(int nIndex)
    {
        if(m_pExtraDefine==null)
	    {
            return "";
	    }

        string szAttachAtt = "";
        int MAX_ATTR_NUM = DBC_DEFINE.MAX_BASE_ATTR + DBC_DEFINE.MAX_ADD_ATTR;
        int index = 0;
        for (int i = 0; i < m_pExtraDefine.m_vEquipAttributes.Count; i++)
        {
            _ITEM_ATTR att = m_pExtraDefine.m_vEquipAttributes[i];
            if ((att.m_AttrType >= 0) && (att.m_AttrType <= MAX_ATTR_NUM))
            {
                string szAtt = "";
                if (att.m_Value.m_Value > 0)
                {
                    
                    if (index == nIndex)
                    {
                        szAtt = String.Format("#{{{0}}}+{1}\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);

                        return szAtt;
                    }

                    index++;
                }
               
            }
        }
        return szAttachAtt;
    }
    public override int GetWhiteAttributeCount()
    {
        if (m_pExtraDefine == null)
        {
            return 0;
        }

        int MAX_ATTR_NUM = DBC_DEFINE.MAX_BASE_ATTR + DBC_DEFINE.MAX_ADD_ATTR;
        int index = 0;
        for (int i = 0; i < m_pExtraDefine.m_vEquipAttributes.Count; i++)
        {
            _ITEM_ATTR att = m_pExtraDefine.m_vEquipAttributes[i];
            if ((att.m_AttrType >= 0) && (att.m_AttrType <= MAX_ATTR_NUM))
            {
                if(att.m_Value.m_Value > 0)
                    index++;
            }
        }
        return index;
    }
    public string GetStrengthAttributeValue(int nStrengthLevel, int nIndex)
    {
        if (m_pExtraDefine == null)
        {
            return "";
        }

        int MAX_ATTR_NUM = DBC_DEFINE.MAX_BASE_ATTR ;
        int index = 0;
        for (int i = 0; i < m_pExtraDefine.m_vEquipAttributes.Count; i++)
        {
            _ITEM_ATTR att = m_pExtraDefine.m_vEquipAttributes[i];
            if ((att.m_AttrType >= 0) && (att.m_AttrType <= MAX_ATTR_NUM))
            {
                string szAtt = "";
                if (att.m_Value.m_Value > 0)
                {
                    if (index == nIndex)
                    {
                        //强化公式
                        int nValue =(int) (m_theBaseDef.nBaseAttr[att.m_AttrType] * CaclStrendthValue(att.m_AttrType, m_theBaseDef.nEquipPoint, nStrengthLevel) + 0.5f);

                        szAtt = String.Format("#{{{0}}}+{1}", g_szAttachAttName[att.m_AttrType], nValue);

                        return szAtt;
                    }

                    index++;
                }
            }
        }

        return "";
    }
    float CaclStrendthValue(byte attrType, int nEquipPoint, int nLevel)
    {
        if (nLevel == 0)
            return 1.0f;
        int nId = nEquipPoint * 110 + nLevel;
        _DBC_ITEM_ENHANCE enchanceDBC = ObjectSystem.EquipEnchanceDBC.Search_Index_EQU(nId);
        if (enchanceDBC == null)
            throw new NullReferenceException("EquipEnchance id not found: " + nId);
        int nRate = enchanceDBC.attribute[attrType];

        //强化公式
        return (1.0f + nLevel * nRate * 0.01f);
    }
    // 允许读取单个属性的值 [3/29/2012 Ivan]
    public short GetSingleAttribValue(byte attType)
    {
        if (m_pExtraDefine == null)
        {
            return -1;
        }

        // 用于UI显示的基础属性 [9/29/2011 edit by ZL]
        for (int i = 0; i < m_pExtraDefine.m_vEquipAttributes.Count; i++)
        {
            _ITEM_ATTR att = m_pExtraDefine.m_vEquipAttributes[i];
            if (att.m_AttrType == attType)
            {
                return att.m_Value.m_Value;
            }
        }
        return -1;
    }

    //得到所有蓝色属性
    public void GetAllBlueAttribute(ref string strValue)
    {
        if (m_pExtraDefine == null)
        {
            strValue = "";
            return;
        }
	
        // 此处修改为所有扩展属性 [9/22/2011 edit by ZL]
        const int BLUE_ATT_START	= DBC_DEFINE.MAX_BASE_ATTR;
        const int BLUE_ATT_END		= DBC_DEFINE.MAX_BASE_ATTR+DBC_DEFINE.MAX_ADD_ATTR-1;

        string szAttachAtt = "";

        for(int i=0; i<m_pExtraDefine.m_vBlueEquipAttributes.Count; i++)
        {
	        _ITEM_ATTR att = m_pExtraDefine.m_vBlueEquipAttributes[i];

	        if(att.m_AttrType >= BLUE_ATT_START && (int)att.m_AttrType <= BLUE_ATT_END) 
	        {
		        string szAtt= "";
		
                szAtt = String.Format("#{%s}+%d\n", g_szAttachAttName[att.m_AttrType], att.m_Value.m_Value);

                szAttachAtt += szAtt;
	        }

    		
        }//

        strValue = szAttachAtt;
    }
    //得到所有绿色属性. 
    public void GetAllGreenAttribute(ref string strValue)
    {
        const int GREEN_ATT_START	= 0;
	    const int GREEN_ATT_END		= DBC_DEFINE.MAX_ADD_ATTR + DBC_DEFINE.MAX_BASE_ATTR -1;
        if(m_pExtraDefine==null)
	    {
		    strValue = "";
            return;
	    }
    	
	    string szAttachAtt = "";
	    for(int i=0; i<m_pExtraDefine.m_vEquipAttributes.Count; i++)
	    {
		    _ITEM_ATTR att = m_pExtraDefine.m_vEquipAttributes[i];
    	
		    if((att.m_AttrType >= GREEN_ATT_START)&&((int)att.m_AttrType <= GREEN_ATT_END))
		    {
			    string szAtt = "";
			    SetAttri(ref att, ref szAtt);
			    szAttachAtt += szAtt;
		    }
	    }

	    strValue = szAttachAtt;
    }
    //得到所有套装属性. 
    public void GetAllSuitAttribute(ref string strValue)
    {
    //    const INT SUIT_ATT_START	= 0;
    //    const INT SUIT_ATT_END		= 53;
    //    if(!m_pExtraDefine)
    //    {
    //        strValue = "";
    //        return;
    //    }
    //    int nID = -1;
    //    int nType = GetEquipQuantity();
    //    switch( nType )
    //    {
    //    case PURPLE_EQUIP:
    //    case GREEN_EQUIP:
    //    case WHITE_EQUIP:
    //    case YELLOW_EQUIP:
    //        nID = GetEquipBaseDefine().pDefineAsBase->nSetID;
    //        break;
    //    default:
    //        strValue = "";
    //        return ;
    //    }

    //    CHAR szAttachAtt[1024] = {0};
    //    DBC_DEFINEHANDLE( s_pItem_SuitEquip, DBC_ITEM_SETATT );
    //    for(INT i=0; i<(INT)s_pItem_SuitEquip->GetRecordsNum(); i++)
    //    {
    //        const _DBC_ITEM_SETATT* pSuitEquip = (const _DBC_ITEM_SETATT*)s_pItem_SuitEquip->Search_LineNum_EQU((UINT)i);

    //        if( pSuitEquip )
    //        {
    //            if( pSuitEquip->nID == nID )
    //            {
    //                // 修改套装表结构 [1/5/2011 ivan edit]
    //// 				for( int j = 0; j < 61; j ++ )
    //// 				{
    //// 					if( pSuitEquip->nAtt[ j ] == -1 )
    //// 						continue;
    //// 					_ITEM_ATTR att;
    //// 					att.m_AttrType = j;
    //// 					att.m_Value.m_Value = pSuitEquip->nAtt[ j ];
    //// 					CHAR szAtt[MAX_PATH] = {0};
    //// 					SetAttri(att, szAtt);
    //// 					strncat(szAttachAtt, szAtt, 1024);
    //// 				}

    //                // 套装属性，目前有六个 [1/5/2011 ivan edit]
    //                _ITEM_ATTR att;
    //                if (pSuitEquip->nActivityNum1 > 0)
    //                {
    //                    att.m_AttrType = pSuitEquip->nAttrType1;
    //                    att.m_Value.m_Value = pSuitEquip->nAttrValue1;
    //                    CHAR szAtt[MAX_PATH] = {0};
    //                    SetAttri(att, szAtt);
    //                    strncat(szAttachAtt, szAtt, 1024);
    //                    att.CleanUp();
    //                }
    //                if (pSuitEquip->nActivityNum2 > 0)
    //                {
    //                    att.m_AttrType = pSuitEquip->nAttrType2;
    //                    att.m_Value.m_Value = pSuitEquip->nAttrValue2;
    //                    CHAR szAtt[MAX_PATH] = {0};
    //                    SetAttri(att, szAtt);
    //                    strncat(szAttachAtt, szAtt, 1024);
    //                    att.CleanUp();
    //                }
    //                if (pSuitEquip->nActivityNum3 > 0)
    //                {
    //                    att.m_AttrType = pSuitEquip->nAttrType3;
    //                    att.m_Value.m_Value = pSuitEquip->nAttrValue3;
    //                    CHAR szAtt[MAX_PATH] = {0};
    //                    SetAttri(att, szAtt);
    //                    strncat(szAttachAtt, szAtt, 1024);
    //                    att.CleanUp();
    //                }
    //                if (pSuitEquip->nActivityNum4 > 0)
    //                {
    //                    att.m_AttrType = pSuitEquip->nAttrType4;
    //                    att.m_Value.m_Value = pSuitEquip->nAttrValue4;
    //                    CHAR szAtt[MAX_PATH] = {0};
    //                    SetAttri(att, szAtt);
    //                    strncat(szAttachAtt, szAtt, 1024);
    //                    att.CleanUp();
    //                }
    //                if (pSuitEquip->nActivityNum5 > 0)
    //                {
    //                    att.m_AttrType = pSuitEquip->nAttrType5;
    //                    att.m_Value.m_Value = pSuitEquip->nAttrValue5;
    //                    CHAR szAtt[MAX_PATH] = {0};
    //                    SetAttri(att, szAtt);
    //                    strncat(szAttachAtt, szAtt, 1024);
    //                    att.CleanUp();
    //                }
    //                if (pSuitEquip->nActivityNum6 > 0)
    //                {
    //                    att.m_AttrType = pSuitEquip->nAttrType6;
    //                    att.m_Value.m_Value = pSuitEquip->nAttrValue6;
    //                    CHAR szAtt[MAX_PATH] = {0};
    //                    SetAttri(att, szAtt);
    //                    strncat(szAttachAtt, szAtt, 1024);
    //                    att.CleanUp();
    //                }
    //                break;
    //            }
    //        }
    //    }

    //    strValue = szAttachAtt;
    }

    //得到镶嵌的宝石个数.
    public int GetGemCount()
    {
        if (null == m_pExtraDefine)
        {
            return 0;
        }

        return m_pExtraDefine.m_vEquipAttachGem.Count;

    }
    // 得到物品表中quality一列2006－3－30
    public override int GetItemTableQuality()
    {
        //  [11/4/2010 Sun]

        if (m_Quality != EQUIP_QUALITY.INVALID_EQUIP)
            return m_theBaseDef.nQuality;
        return 0; 
    }

    // 得到物品表中type一列2006－3－30
    public override int GetItemTableType()
    {
        //  [11/4/2010 Sun]

        if (m_Quality != EQUIP_QUALITY.INVALID_EQUIP)
            return m_theBaseDef.nType;
        return 0; 
    }

    // ID最后3位索引 [11/17/2010 Sun]
    public override int GetItemTableIndex()
    {
        return m_theBaseDef.nIndex;
    }


    // 得到装备基本定义
    public _DBC_ITEM_EQUIP_BASE GetEquipBaseDefine() { return m_theBaseDef; }
    // 得到装备的属性(白, 绿, 蓝)
    public EQUIP_QUALITY GetEquipQuantity()
    {
        return m_Quality;
    }

    public EQUIP_ATTRIB GetEquipAttrib()
    {
        if (GetEquipQuantity() != EQUIP_QUALITY.BLUE_EQUIP) return EQUIP_ATTRIB.EQUIP_ATTRIB_IDENTIFY;

        return m_EquipAttrib;
    }

    // 是否是武器
    bool IsWeapon()
    {
        return false;
    }

    // 是否是防俱
    bool IsDefence()
    {
        return false;
    }

    // 是否是装饰物
    bool IsAppend()
    {
        return false;
    }

    // 设置升品预览属性 [9/19/2011 edit by ZL]
    //public void			SetPreViewAttr(CObject_Item_Equip ** resEquip, INT pPinJiValue, CObject_Item_Equip::EQUIP_ATTRIBUTES &resAttr);

    //// 取得升品（升质）后的最终Equip与品级值 [9/19/2011 edit by ZL]
    //INT				GetEquipByPinJiValue(CObject_Item_Equip ** resEquip, INT &pPinJiValue);

    //// 设置升级预览属性 [9/20/2011 edit by ZL]
    //INT				GetPreViewLevelupEquip(CObject_Item_Equip ** resEquip);

    //// 取得下一个品级的某项属性值 [9/21/2011 edit by ZL]
    //INT				GetNextPinJiAttriValue(INT curPinji, INT attriType);

    //// 取得当前品级升级需要的品级值 [9/21/2011 edit by ZL]
    //INT				GetNextPinJiValue();

    //// 根据品级值、强化属性重新计算基础属性值 [9/21/2011 edit by ZL]
    //VOID			ReJudgeBaseAttri();

    //// 取得当前装备的档次 [9/22/2011 edit by ZL]
	public int GetCurrentDangCi()
	{ 
		if(m_theBaseDef != null)
		  return m_theBaseDef.nDangCi;
		else
			return -1;
	}

    //// 取得当前装备的五行 [9/22/2011 edit by ZL]
    //INT				GetCurrentWuXing() { return m_theBaseDef.pDefineAsBase->nMenPai; }

    //// 取得当前装备的装备点 [9/22/2011 edit by ZL]
    //INT				GetEquipPoint() { return m_theBaseDef.pDefineAsBase->nEquipPoint; }

    //// 取得当前装备的魂印类型 [10/10/2011 edit by ZL]
    //INT				GetCurrentSoulType() { return m_pExtraDefine ? m_pExtraDefine->m_PrintSoulType : 0; }

    //// 取得当前装备的魂印类型 [10/10/2011 edit by ZL]
    //VOID			GetCurrentSoulAttr(STRING& strValue);

    // 按照参数生成不同品质装备 [11/4/2010 Sun]
    void AsEquipBase(_DBC_ITEM_EQUIP_BASE pDefine, EQUIP_QUALITY quality)
    {
        m_Quality = quality;
	    m_theBaseDef = pDefine;
	    m_nParticularID = ( ( ( ( ( m_theBaseDef.nClass * 100 ) + m_theBaseDef.nQuality ) * 100 ) + m_theBaseDef.nType ) * 1000 ) + m_theBaseDef.nIndex;

	    if(m_pExtraDefine == null)
		    m_pExtraDefine = new EXTRA_DEFINE();

	    m_pExtraDefine.m_CurDurPoint = pDefine.nBMaxDur;
	    m_pExtraDefine.m_MaxDurPoint = pDefine.nBMaxDur;
	    m_pExtraDefine.m_nLevelNeed  = pDefine.nLevelRequire;
	    m_pExtraDefine.m_nRepairFailureTimes  = pDefine.nBMaxRepair;
	    m_pExtraDefine.m_nEquipQulity= (int)m_Quality;
	    m_pExtraDefine.m_nSellPrice  = pDefine.nBasePrice;

	    // 暂时无绑定信息以后需要添加
	    //m_pExtraDefine.m_bLocked  = FALSE;


	    //for(INT i=0; i<pItemInfo.GetEquipData().m_AttrCount; i++)
	    //{
	    //	m_pExtraDefine.m_vEquipAttributes.push_back(pItemInfo.GetEquipData().m_pAttr[i]);
	    //}

	    //for(INT i=0; i<pItemInfo.GetEquipData().m_StoneCount; i++)
	    //{
	    //	m_pExtraDefine.m_vEquipAttachGem.push_back(pItemInfo.GetEquipData().m_pGemInfo[i]);
	    //}

	    _ITEM_ATTR	attr;
	    _ITEM_VALUE	attrValue;
	    m_pExtraDefine.m_vBlueEquipAttributes.Clear();
	    m_pExtraDefine.m_vEquipAttributes.Clear();
	    // 客户端生成装备只生成基础属性且基础属性为一品 [10/11/2011 edit by ZL]
        for (int i = 0; i < (int)ITEM_ATTRIBUTE.IATTRIBUTE_NUMBER; i++)
	    {
		    if(i < DBC_DEFINE.MAX_BASE_ATTR && (pDefine.nBaseAttr[i] != -1) )
		    {
			    attrValue.m_Value = (short)pDefine.nBaseAttr[i];
			    attr.m_AttrType = (byte)i;
			    attr.m_Value    = attrValue;
			    m_pExtraDefine.m_vEquipAttributes.Add(attr);
		    }

	    }
    }

    //品质
    EQUIP_QUALITY m_Quality;		
    //扩展属性(需要服务器传输)
    EXTRA_DEFINE m_pExtraDefine;
    string m_strExtraDesc;

    _DBC_ITEM_EQUIP_BASE m_theBaseDef;
    EQUIP_ATTRIB m_EquipAttrib;


    public static readonly string[] g_szAttachAttName = new string[] 
    {
	    "equip_base_attack_p",
	    "equip_base_attack_m",
	    "equip_base_defence_p",
	    "equip_base_defence_m",
	    //"equip_base_hit",
        "equip_base_maxhp",
	    "equip_base_miss",
	    "equip_attr_maxhp",							
	    "equip_attr_maxmp",							
	    "equip_attr_attack_metal",
	    "equip_attr_attack_wood",
	    "equip_attr_attack_water", 
	    "equip_attr_attack_fire", 
	    "equip_attr_attack_earth",
	    "equip_attr_attack_p",	
	    "equip_attr_defence_p",
	    "equip_attr_attack_m",	
	    "equip_attr_defence_m",
	    "equip_attr_hit",                 				
	    "equip_attr_miss", 
	    "equip_attr_attack_critical",					//20  会心一击（暴击）
	    "equip_attr_defence_critical",					//21  会心防御（抗暴击）
	    "equip_attr_no_p_defence_rate",
	    "equip_attr_no_m_defence_rate",
	    "equip_attr_damage_ret",						//伤害反射
	    "equip_attr_damage_scale",						//伤害增幅
	    "equip_attr_immunity_p",						//武力免伤
	    "equip_attr_immunity_m",						//仙术免伤
	    "equip_attr_resist_all",
    };
}
/*}*/
