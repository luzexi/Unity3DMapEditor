using System;
using System.Collections.Generic;
using UnityEngine;

// namespace SGWEB
// {
public class CObject_Item_Medicine : CObject_Item
{
    //--------------------------------------------------------------
    //基本属性
    _DBC_ITEM_MEDIC m_theBaseDef;			//基本属性定义(数据表)

    public CObject_Item_Medicine(int id)
        : base(id)
    {

    }
    public void AsMedicine(_DBC_ITEM_MEDIC medicDefine)
    {
        m_theBaseDef = medicDefine;
        m_nParticularID = (((((m_theBaseDef.nClass * 100) + m_theBaseDef.nQuality) * 100) + m_theBaseDef.nType) * 1000) + m_theBaseDef.nIndex;
    }

    public override ITEM_CLASS GetItemClass()
    {
        return (ITEM_CLASS)m_theBaseDef.nClass;
    }
    // 得到quality信息
	public override int	GetItemTableQuality()
    {
        return m_theBaseDef.nQuality;
    }
    // 得到type信息
	public override int	GetItemTableType()
    {
        return m_theBaseDef.nType;
    }
    //  [11/17/2010 Sun]
	public override int	GetItemTableIndex()
    {
        return m_theBaseDef.nIndex;
    }
    //得到表中的定义
	public _DBC_ITEM_MEDIC	GetBaseDefine()  { return m_theBaseDef; }
    public override string GetName()
    {
        return m_theBaseDef.szName;
    }
    //物品解释
	public override string GetDesc()	{ return m_theBaseDef.szDesc; }
    public override string GetIconName()
    {
        return m_theBaseDef.szIcon;
    }
    //详细解释(可能需要服务器)
	public override string	GetExtraDesc()
    {
	    return m_theBaseDef.szDesc;
    }
    public override void SetExtraInfo(ref _ITEM pItemInfo)
    {
        if(pItemInfo == null) return;

        base.SetExtraInfo(ref pItemInfo);
	    MEDIC_INFO infoMedic = pItemInfo.m_Medic;

	    SetNumber(pItemInfo.GetItemCount());
	    SetManufacturer(ref pItemInfo);
    }
    //克隆详细信息
	public override void Clone(CObject_Item pItemSource)
    {
        SetNumber(((CObject_Item_Medicine)pItemSource).GetNumber());
	    base.Clone(pItemSource);
    }

    // 得到物品的品质
	public int GetQuantity()
    {
        if(m_theBaseDef!=null)
	    {
		    return m_theBaseDef.nQuality;
	    }

	    return -1;
    }
    // 得到物品卖给npc的价格
	public override int	GetItemPrice()
    {
        if(m_theBaseDef!=null)
	    {
		    return m_theBaseDef.nSalePrice;
	    }
	    return 0;
    }
    // 得到物品会引起的技能ID
	public int GetItemSkillID()
    {
        if(m_theBaseDef!=null)
	    {
		    return m_theBaseDef.nSkillID;
	    }
	    return 0;
    }
    //得到物品的选择对象类型
	public override int	GetItemTargetType()
    {
        if(m_theBaseDef!=null)
	    {
		    return m_theBaseDef.nTargetType;
	    }
	    return (int)ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_INVALID;
    }
    // 得到玩家使用这个物品需要的等级
	public override int	GetNeedLevel()
    {
        if(m_theBaseDef!=null)
	    {
		    return m_theBaseDef.nLevelRequire;
	    }
	    return 0;
    }
    //该物品是否能够在某对象上使用
	public bool	IsValidTarget(CObject pSelectObj, ref fVector2 fvPos, ref int objID, ref PET_GUID_t petID)
    {
        if(m_theBaseDef==null)
            return false;
	    //玩家自己
	    CObject_PlayerMySelf pMySlef = CObjectManager.Instance.getPlayerMySelf();

        ENUM_ITEM_TARGET_TYPE targetType = (ENUM_ITEM_TARGET_TYPE)m_theBaseDef.nTargetType;
	    //无需目标
	    if(ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_NONE==targetType) 
		    return true;

	    //需要选择一个场景位置
	    if(ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_POS==targetType) 
	    {
            return WorldManager.Instance.ActiveScene.IsValidPosition(ref fvPos);
	    }

	    //目前不支持的方式
	    if(	ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_DIR==targetType ||	//方向
		    ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ITEM==targetType)	//物品->物品
		    return false;

	    switch(targetType)
	    {
	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_SELF:	//自已
		    {
			    objID = pMySlef.ServerID;
		    }
		    return true;

	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_SELF_PET:	//自己的宠物
		    {
                //petID = pMySlef.GetCharacterData().Get_CurrentPetGUID();
                ////宠物尚未释放
                //if(petID.IsNull())
                //    return false;

			    objID = pMySlef.ServerID;
			    return true;
		    }
		    //break;

	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND:	//友好的目标
	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY:	//敌对目标
		    {
			    //必须需要一个目标
			    if(pSelectObj==null) return false;
			    //必须是角色
			    if(!(pSelectObj is CObject_Character))
				    return false;

			    //检查阵营
			    ENUM_RELATION eCampType = 
				    GameProcedure.s_pGameInterface.GetCampType(pMySlef, pSelectObj);

			    //必须是友好目标/敌对目标
			    if( (targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND && 
					    (pSelectObj==pMySlef || eCampType==ENUM_RELATION.RELATION_FRIEND)) ||
				    (targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY && 
					    (eCampType==ENUM_RELATION.RELATION_ENEMY)))
			    {
				    objID = pSelectObj.ServerID;
				    return true;
			    }
		    }
		    return false;

	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND_PLAYER: // 友好玩家	
	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY_PLAYER:	//	敌对玩家
		    {
			    //必须需要一个目标
			    if(pSelectObj == null) return false;
			    //必须是玩家
			    if(!(pSelectObj is CObject_PlayerOther))
				    return false;

			    //检查阵营
			    ENUM_RELATION eCampType = 
				    GameProcedure.s_pGameInterface.GetCampType(pMySlef, pSelectObj);

			    //必须是友好玩家/敌对玩家
			    if(	(targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND_PLAYER &&
					    (pSelectObj==pMySlef || eCampType==ENUM_RELATION.RELATION_FRIEND)) ||
				    (targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY_PLAYER &&
					    (eCampType==ENUM_RELATION.RELATION_ENEMY)))
			    {
				    objID = pSelectObj.ServerID;
				    return true;
			    }
		    }
		    return false;

	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND_MONSTER:	//友好怪物
	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY_MONSTER:	//敌对怪物
		    {
			    //必须需要一个目标
			    if(pSelectObj==null) return false;
			    //必须是角色
			    if(!(pSelectObj is CObject_Character))
				    return false;
			    //必须是NPC
			    if(((CObject_Character)pSelectObj).GetCharacterType() != 
					    CHARACTER_TYPE.CT_MONSTER) return false;

			    //检查阵营
			    ENUM_RELATION eCampType = 
				    GameProcedure.s_pGameInterface.GetCampType(pMySlef, pSelectObj);

			    //必须是友好NPC
			    if(	(targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND_MONSTER && 
					    eCampType==ENUM_RELATION.RELATION_FRIEND) ||
				    (targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY_MONSTER && 
					    eCampType==ENUM_RELATION.RELATION_ENEMY))
			    {
				    objID = pSelectObj.ServerID;
				    return true;
			    }
		    }
		    return false;

	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND_PET:	//友好宠物
	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY_PET:	//敌对宠物	
		    {
			    //必须需要一个目标
			    if(pSelectObj==null) return false;
			    //必须是角色
			    if(!(pSelectObj is CObject_Character))
				    return false;
			    //必须是NPC
			    if(((CObject_Character)pSelectObj).GetCharacterType() != 
					    CHARACTER_TYPE.CT_MONSTER) return false;
			    //必须是宠物
			    if(((CObject_PlayerNPC)pSelectObj).GetNpcType() !=
					    ENUM_NPC_TYPE.NPC_TYPE_PET) return false;

			    //检查阵营
			    ENUM_RELATION eCampType = 
				    GameProcedure.s_pGameInterface.GetCampType(pMySlef, pSelectObj);

			    //必须是友好宠物
			    if(	(targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_FRIEND_MONSTER && 
					    eCampType==ENUM_RELATION.RELATION_FRIEND) ||
				    (targetType==ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ENEMY_PET && 
					    eCampType==ENUM_RELATION.RELATION_ENEMY))
			    {
				    objID = pSelectObj.ServerID;
				    return true;
			    }
		    }
		    return false;

	    case ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_ALL_CHARACTER:	//所有角色
		    {
			    //必须需要一个目标
			    if(pSelectObj==null) return false;
			    //必须是角色
			    if(!(pSelectObj is CObject_Character))
				    return false;

			    return true;
		    }
		    //return false;

	    default:
		    break;
	    }

	    return false;
    }
    public bool IsAreaTargetType()
    {
       if(m_theBaseDef==null) return false;
	    //玩家自己
	    CObject_PlayerMySelf pMySlef = CObjectManager.Instance.getPlayerMySelf();

	    //无需目标
	    return ((int)ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_POS==m_theBaseDef.nTargetType);
    }
   //是否目标对象是绝对唯一的(自己，自己的宠物)
	public bool	IsTargetOne()
    {
        if(m_theBaseDef==null) return false;

	    return ((int)ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_SELF==m_theBaseDef.nTargetType || 
			(int)ENUM_ITEM_TARGET_TYPE.ITEM_TARGET_TYPE_SELF_PET==m_theBaseDef.nTargetType);
    }


	//获得物品的最大叠加数量
	public override int				GetMaxOverLay() { return m_theBaseDef.nPileCount; }
	// 得到物品的材料品级值 [9/19/2011 edit by ZL]
	public override int				GetPinJiValue() { return m_theBaseDef.nCurPinJiValue; }

	// 得到物品的类型描述2006-4-28
	public override string	GetItemTableTypeDesc()
    {
        return m_theBaseDef.szTypeDesc;
    }

	// 得到物品档次描述信息
	public override int	GetItemLevelDesc()
    {
        return 1;
    }
// 得到消耗品在表中的类型
	public int GetMedicineItemTableType()
    {
        if(m_theBaseDef!=null)
	    {
		    return m_theBaseDef.nType;
	    }

	    return -1;
    }
}
/*}*/
