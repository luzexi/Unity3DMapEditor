using System;

public class CObject_Item_Symbol : CObject_Item
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

    public override int GetMaxOverLay()
    {
        return 1;
    }

    public override void Clone(CObject_Item pItemSource)
    {
        SetNumber(((CObject_Item_Symbol)pItemSource).GetNumber());
	    base.Clone(pItemSource);
    }
 
    #endregion

    // 得到符印等级
    public int GetSymbolLevel()
    {
        return m_theBaseDef.nQuality;
    }
    public CObject_Item_Symbol(int id)
        :base(id)
    {
    }
    public void AsSymbol(_DBC_ITEM_SYMBOL symbolDefine)
    {
        if (symbolDefine == null)
            throw new NullReferenceException("symbol define is null in CObject_Item_Gem.AsSymbol()");

        m_theBaseDef = symbolDefine;
        m_nParticularID = (((((m_theBaseDef.nClass * 100) + m_theBaseDef.nQuality) * 100) + m_theBaseDef.nType) * 1000) + m_theBaseDef.nSymbolIndex;
    }
    _DBC_ITEM_SYMBOL m_theBaseDef;
}
 