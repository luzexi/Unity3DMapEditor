using System;

public class CObject_Item_Gem : CObject_Item
{

    #region CObject_Item

    public override int GetItemPrice()
    {
        return m_theBaseDef.nPrice;
    }
    
    public override ITEM_CLASS GetItemClass()
    {
        return (ITEM_CLASS)m_theBaseDef.nClass;
    }

    public override int GetItemTableQuality()
    {
        return m_theBaseDef.nQuality;
    }
    //宝石类型
    public override int GetItemTableType()
    {
        return m_theBaseDef.nType;
    }

    public override string GetName()
    {
        return m_theBaseDef.szName;
    }

    public override string GetDesc()
    {
        return m_theBaseDef.szDesc;
    }

    public override string GetIconName()
    {
        return m_theBaseDef.szIcon;
    }

    public override string GetExtraDesc()
    {
        return m_theBaseDef.szDesc;
    }

    public override void SetExtraInfo(ref _ITEM pItemInfo)
    {
        if(pItemInfo == null)
            throw new NullReferenceException("iteminfo is null in CObject_Item_Gem.SetExtraInfo()");

	    //调用基类函数，保存_ITEM结构
	    base.SetExtraInfo(ref pItemInfo);

	    SetNumber(pItemInfo.GetItemCount());
	    SetManufacturer(ref pItemInfo);
    }

    public override int GetMaxOverLay()
    {
        return 1;
    }

    public override void Clone(CObject_Item pItemSource)
    {
        SetNumber(((CObject_Item_Gem)pItemSource).GetNumber());
	    base.Clone(pItemSource);
    }
   
    public override string GetItemTableTypeDesc()
    {
        return m_theBaseDef.szTypeDesc;
    }
 
    #endregion

    // 得到宝石的等级
    public int GetGemLevel()
    {
        return m_theBaseDef.nQuality;
    }
    // 得到宝石属性信息
    public string GetGemArribInfo()
    {
        if (m_theBaseDef != null)
        {
            // 修改宝石属性显示 [9/28/2011 edit by ZL]
            return string.Format("%{0}+%{1}\n", g_szGemAttName[m_theBaseDef.m_GemAttrType], m_theBaseDef.m_GemAttrValue);

        }
        return "";
    }


    public CObject_Item_Gem(int id)
        :base(id)
    {
        m_theBaseDef = null;
    }

    public void AsGem(_DBC_ITEM_GEM gemDefine)
    {
        if (gemDefine == null)
            throw new NullReferenceException("Gem define is null in CObject_Item_Gem.AsGem()");

        m_theBaseDef = gemDefine;
        m_nParticularID = (((((m_theBaseDef.nClass * 100) + m_theBaseDef.nQuality) * 100) + m_theBaseDef.nType) * 1000) + m_theBaseDef.nGemIndex;

    }

    _DBC_ITEM_GEM m_theBaseDef;
    string m_strExtraDesc;

    
    public static readonly string[] g_szGemAttName = new string[] 
    {
	    "equip_base_attack_p",
	    "equip_base_attack_m",
	    "equip_base_defence_p",
	    "equip_base_defence_m",
	    "equip_base_hit",
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