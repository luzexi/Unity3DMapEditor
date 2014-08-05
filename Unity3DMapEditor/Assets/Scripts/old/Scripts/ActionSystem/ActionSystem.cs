using System;
using System.Collections.Generic;
using UnityEngine;

using Interface;
using Network;
using Network.Packets;
// namespace SGWEB
// {
public class CActionSystem /*: tActionSystem*/
{
    static readonly CActionSystem instance = new CActionSystem();
    public static CActionSystem Instance
    {
        get { return instance; }
    }

    const int s_MainMenuBarNum = 30;
    //-----------------------------------------------------
    //所有操作
    // 改用嵌套结构，第一层索引使用物体类型，如AOT_ITEM，降低遍历次数 [1/6/2012 Ivan]
    Dictionary<int, List<CActionItem>> m_mapAllAction = new Dictionary<int, List<CActionItem>>();

    // 注意！由于dictionary直接取不到数据的时候会抛异常，所以封装了一个接口 [1/12/2012 Ivan]
    List<CActionItem> GetSubMapByClass(ACTION_OPTYPE type)
    {
        List<CActionItem> subMap;
        if (m_mapAllAction.TryGetValue((int)type, out subMap))
        {
            return subMap;
        }
        else
        {
            subMap = new List<CActionItem>();
            m_mapAllAction[(int)type] = subMap;
            return subMap;
        }
    }
    int m_nCurBankRentBoxIndex;

    //缺省操作(物理攻击)
    tActionItem m_pDefaultAction;
    //准备发出的action
    CActionItem setoutAction;
    public CActionItem SetoutAction
    {
        get { return setoutAction; }
        set { setoutAction = value; }
    }
    // 用于超链接的action [4/12/2011 Sun]
    //tActionItem m_pUILinkAction;

    static int ActionId = 0;

    private int CreateID()
    {
        return ActionId++;
    }

    //         public void EraseActionItem(int id)
    //         {
    //             m_mapAllAction.Remove(id);
    //         }

    //-----------------------------------------------------
    //工具条设置
    struct ACTIONBAR_ITEM
    {

        public ACTION_OPTYPE typeItem;	// 所引用的ActionItem的类型

        public int idImpl;			// 引用项目的定义ID
        /*
        |
        |	技能		- 技能ID
        |	生活技能	- 生活技能ID
        |	物品		- client的idtable
        |	宠物技能	- 技能ID
        */
        //---运行时
        public int idActionItem;		// 引用的ActionItem

        public int idItemCount;		// 物品数量

        public int idPetIndex;			// 宠物索引

        /**************************************************************
        * 用户定义快捷键 [8/26/2011 edit by ZL]
        * 第 1到16位预留置0
        * 第16到24位保存用户键1
        * 第24到32位保存用户键2
        ***************************************************************/

        public int idHotKey;			// 快捷键 [8/26/2011 edit by ZL]

    };

    ACTIONBAR_ITEM[] m_barMain = new ACTIONBAR_ITEM[s_MainMenuBarNum];

    public void CleanInvalidAction()
    {
        foreach (int id in m_mapAllAction.Keys)
        {
            List<CActionItem> actions = m_mapAllAction[id];
            List<CActionItem>.Enumerator enumerator = actions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CActionItem Current = enumerator.Current;
                if (Current.GetImpl() == null)
                {
                    actions.Remove(Current);
                    enumerator = actions.GetEnumerator();
                }
                else
                {
                    CObject_Item item = Current.GetImpl() as CObject_Item;
                    if (item != null && !ObjectSystem.Instance.ItemExist(item.GetID()))
                    {
                        actions.Remove(Current);
                        enumerator = actions.GetEnumerator();
                    }
                }
            }
        }
    }

    public CActionItem_Item GetAction_ItemID(int itemId)
    {
        return GetAction_ItemID(itemId, true);
    }
    public CActionItem_Item GetAction_ItemID(int itemId, bool bUpdateToolBar)
    {
        if (m_mapAllAction.ContainsKey((int)ACTION_OPTYPE.AOT_ITEM))
        {
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
            foreach (CActionItem kvp in actionItems)
            {
                if (kvp != null)
                {
                    CObject_Item tItem = (CObject_Item)kvp.GetImpl();
                    if (tItem != null && tItem.GetID() == itemId)
                    {
                        return (CActionItem_Item)kvp;
                    }
                }
            }
        }
        CObject_Item pItem = ObjectSystem.Instance.FindItem(itemId);
        if (pItem != null)
        {
            CActionItem_Item pItemAction = new CActionItem_Item(CreateID());
            pItemAction.Update_Item(pItem);

            //加入链表
            List<CActionItem> actionList = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);

            actionList.Add(pItemAction);
            if (bUpdateToolBar)// 超链接时提高效率 [5/12/2011 Sun]
                UpdateToolBar();

            return pItemAction;
        }

        return null;
    }

    public CActionItem_Skill GetAction_SkillID(int defineId)
    {
        if (m_mapAllAction.ContainsKey((int)ACTION_OPTYPE.AOT_SKILL))
        {
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_SKILL);
            foreach (CActionItem kvp in actionItems)
            {
                if (kvp != null)
                {
                    SCLIENT_SKILL skill = (SCLIENT_SKILL)kvp.GetImpl();
                    if (skill == null) continue;
                    if (skill.m_pDefine.m_nID * 100 + skill.m_nLevel == defineId)
                    {
                        return (CActionItem_Skill)kvp;
                    }
                }
            }
        }
        return null;
    }
    public CActionItem GetAction_PetSkillID(int nPetNum, int id)
    {
        if (m_mapAllAction.ContainsKey((int)ACTION_OPTYPE.AOT_PET_SKILL))
        {
            List<CActionItem> items = GetSubMapByClass(ACTION_OPTYPE.AOT_PET_SKILL);
            foreach (CActionItem kvp in items)
            {
                if (kvp != null)
                {
                    PET_SKILL skill = kvp.GetImpl() as PET_SKILL;
                    if (skill == null || skill.m_pDefine == null) continue;
                    if (skill.m_pDefine.m_nID == id && skill.m_nPetNum == nPetNum)
                    {
                        return kvp;
                    }
                }
            }
        }
        return null;
    }
    public CActionItem GetAction_ItemIDTable_FromMyPacket(int id)
    {
        if (m_mapAllAction.ContainsKey((int)ACTION_OPTYPE.AOT_ITEM))
        {
            List<CActionItem> items = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
            foreach (CActionItem kvp in items)
            {
                if (kvp != null)
                {
                    CObject_Item item = kvp.GetImpl() as CObject_Item;
                    if (item != null && item.GetIdTable() == id && item.TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
                        return kvp;
                }
            }
        }
        return null;


    }
    public void UpdateCommonCoolDown(int nSkillID)
    {
        int nCommonCoolDown = CDataPool.Instance.CommonCoolDown_Get();
        if (nCommonCoolDown > 0) return;
        CDataPool.Instance.CommonCoolDown_Update();
        foreach (List<CActionItem> subItems in m_mapAllAction.Values)
        {
            foreach (CActionItem pActionItem in subItems)
            {
                if (pActionItem.GetCoolDownID() == MacroDefine.INVALID_ID)
                {
                    if (MacroDefine.INVALID_ID != nSkillID &&
                        pActionItem.GetDefineID() == nSkillID && pActionItem.GetType() == ACTION_OPTYPE.AOT_SKILL) continue;
                    pActionItem.UpdateCommonCoolDown();
                }
            }
        }
    }

    void UpdateAction_FromLifeAbility(SCLIENT_LIFEABILITY pLifeAbility)
    {
        //如果没有学会的技能，退出
        if (pLifeAbility.m_nLevel <= 0) return;

        if (pLifeAbility.m_pDefine == null)
            return;

        //检查时候已经有该操作
        CActionItem_LifeAbility pLifeAbilityAction = (CActionItem_LifeAbility)GetAction_LifeAbilityID(pLifeAbility.m_pDefine.nID);

        //如果没有,创建
        if (pLifeAbilityAction == null)
        {
            pLifeAbilityAction = new CActionItem_LifeAbility(CreateID());
            pLifeAbilityAction.Update_LifeAbility(pLifeAbility);
            //加入
            List<CActionItem> actionMap = null;
            if (m_mapAllAction.ContainsKey((int)pLifeAbilityAction.GetType()))
            {
                actionMap = m_mapAllAction[(int)pLifeAbilityAction.GetType()];
            }
            else
            {
                actionMap = new List<CActionItem>();
                m_mapAllAction.Add((int)pLifeAbilityAction.GetType(), actionMap);
            }
            actionMap.Add(pLifeAbilityAction);
            //		UpdateToolBar();
        }
    }
    CActionItem GetAction_LifeAbilityID(int idAbility)
    {
        return null;
        //todo
        // 	        register std::map< INT, CActionItem* >::iterator it;
        //         	
        // 	        for(it=m_mapAllAction.begin(); it!=m_mapAllAction.end(); it++)
        // 	        {
        // 		        CActionItem* pActionItem = (CActionItem*) it->second;
        // 
        // 		        //如果是生活技能
        // 		        if(	pActionItem && 
        // 			        pActionItem->GetType() == AOT_SKILL_LIFEABILITY )
        // 		        {
        // 			        const SCLIENT_LIFEABILITY* pAbility = (const SCLIENT_LIFEABILITY*)pActionItem->GetImpl();
        // 			        if(pAbility && pAbility->m_pDefine->nID == idAbility) return pActionItem;
        // 		        }
        // 	        }
        // 
        // 	        return NULL;
    }
    public void UserTargetPetSkill_Update()
    {
        CleanInvalidAction();

	    //技能列表
	    for(int i=0; i < GAMEDEFINE.MAX_PET_SKILL_COUNT; i++)
	    {
		    PET_SKILL arraySkill = CDataPool.Instance.TargetPet_GetSkill(i);
		    if(arraySkill==null || (arraySkill.m_pDefine== null)) continue;
		    UpdateAction_FromPetSkill((int)PET_INDEX.TARGETPET_INDEX,arraySkill);
	    }
    }
    public void UserBank_Update()
    {
	    //清空已经不存在的Action
	    CleanInvalidAction();

	    for(int i=0; i<GAMEDEFINE.MAX_BANK_SIZE; i++)
	    {
		    //取得物品
		    CObject_Item	pItem = CDataPool.Instance.UserBank_GetItem(i);
		    if(pItem != null)
		    {
			    //设置新的物品数据
			    UpdateAction_FromItem(pItem);
		    }
	    }
    }
    public void UpdateAction_FromItem(CObject_Item item)
    {
        if (item == null)
            return;
        //检查时候已经有该操作
        CActionItem_Item actionItem = (CActionItem_Item)GetAction_ItemID(item.GetID());
        if (actionItem == null)
        {
            if (item != null)
            {
                CActionItem_Item itemAction = new CActionItem_Item(CreateID());
                itemAction.Update_Item(item);

                List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
                actionItems.Add(itemAction);
            }
        }
        else//重新刷新数据
        {
            actionItem.Update_Item(item);
            //该物品如果关联到主菜单，必须刷新主菜单
            //UpdateToolBar(item.GetID());
        }
    }

    //主工具条快捷操作 
    // nIndex	(0,1,2,3,4,5,6,7,8,9)
    // bPrepare 是否是预备(仅显示Check状态)
    public void MainMenuBar_DoAction(int nIndex, bool bPrepare)
    {
        if (nIndex < 0 || nIndex >= s_MainMenuBarNum) return;

        ACTIONBAR_ITEM item = m_barMain[nIndex];

        //尚未设定
        CActionItem pItem = GetActionByItemId(item.idActionItem);
        if (pItem == null) return;

        if (bPrepare)
        {
            //显示Check
            pItem.DoActionPrepare(true);
        }
        else
        {
            //如果有交易，摆摊，玩家商店界面开着，不能执行
            // 如果交易界面开着
            if (GameProcedure.s_pUISystem != null && GameProcedure.s_pUISystem.IsWindowShow("Exchange"))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "你现在无法这样做");
                return;
            }
            //如果摆摊界面开着
            if (GameProcedure.s_pUISystem != null && GameProcedure.s_pUISystem.IsWindowShow("StallSale"))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "你现在无法这样做");
                return;
            }
            //如果玩家商店界面开着
            if (GameProcedure.s_pUISystem != null && GameProcedure.s_pUISystem.IsWindowShow("PS_ShopMag"))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "你现在无法这样做");
                return;
            }

            //隐藏Check
            pItem.DoActionPrepare(false);
            //执行动作
            pItem.DoAction();
        }
    }
    //设置主菜单上项目
    public void MainMenuBar_Set(int nIndex, int idAction)
    {
        if (nIndex < 0 || nIndex > s_MainMenuBarNum) return;

        if (idAction == MacroDefine.INVALID_ID)
        {
            m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_EMPTY;
            m_barMain[nIndex].idImpl = MacroDefine.INVALID_ID;
            m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;

            UpdateToolBar();
        }

        CActionItem pItem = GetActionByActionId(idAction);
        if (pItem != null)
        {

            switch (pItem.GetType())
            {
                case ACTION_OPTYPE.AOT_SKILL:
                    {
                        SCLIENT_SKILL pSkill = pItem.GetImpl() as SCLIENT_SKILL;
                        if (pSkill == null) break;

                        m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_SKILL;
                        m_barMain[nIndex].idImpl = pSkill.m_pDefine.m_nID * 100 + pSkill.m_nLevel;
                        m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;
                    }
                    break;
                case ACTION_OPTYPE.AOT_ITEM:
                    {
                        CObject_Item pItemInfo = pItem.GetImpl() as CObject_Item;
                        if (pItemInfo == null) break;

                        m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_ITEM;
                        m_barMain[nIndex].idImpl = pItemInfo.GetIdTable();
                        m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;

                    }
                    break;
                case ACTION_OPTYPE.AOT_PET_SKILL:
                    {
                        PET_SKILL pPetSkill = pItem.GetImpl() as PET_SKILL;
                        if (pPetSkill == null) break;

                        m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_PET_SKILL;
                        m_barMain[nIndex].idImpl = pPetSkill.m_pDefine.m_nID;
                        m_barMain[nIndex].idPetIndex = pPetSkill.m_nPetNum;
                        m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;
                    }
                    break;
                case ACTION_OPTYPE.AOT_CHATMOOD:
                    {
                        TALK_ACT_STRUCT pActStruct = pItem.GetImpl() as TALK_ACT_STRUCT;
                        if (pActStruct == null) break;

                        m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_CHATMOOD;
                        m_barMain[nIndex].idImpl = pActStruct.m_actIdx;
                        m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;
                    }
                    break;
                default:
                    break;
            }

            UpdateToolBar();
        }
        ACTIONBAR_ITEM barItem = m_barMain[nIndex];

        CActionItem pActionItem = null;
        CGModifySetting msg = NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_MODIFYSETTING) as CGModifySetting;
        _OWN_SETTING setting;
        switch (barItem.typeItem)
        {
            case ACTION_OPTYPE.AOT_PET_SKILL:
                //pActionItem = GetAction_PetSkillID(barItem.idPetIndex,barItem.idImpl);
                msg.Type = (byte)(nIndex + SETTING_TYPE.SETTING_TYPE_K0);

                setting.m_SettingType = (byte)barItem.typeItem;
                setting.m_SettingData = (barItem.idPetIndex << 16) + barItem.idImpl;
                msg.Value = setting;

                NetManager.GetNetManager().SendPacket(msg);
                break;

            case ACTION_OPTYPE.AOT_SKILL:
                //pActionItem = GetAction_SkillID(barItem.idImpl);
                msg.Type = (byte)(nIndex + SETTING_TYPE.SETTING_TYPE_K0);

                setting.m_SettingType = (byte)barItem.typeItem;
                setting.m_SettingData = barItem.idImpl;
                msg.Value = setting;

                NetManager.GetNetManager().SendPacket(msg);
                break;

            case ACTION_OPTYPE.AOT_SKILL_LIFEABILITY:
                break;

            case ACTION_OPTYPE.AOT_ITEM:
                pActionItem = GetAction_ItemIDTable_FromMyPacket(barItem.idImpl);
                if (pActionItem != null)
                {
                    barItem.idItemCount = CDataPool.Instance.UserBag_CountItemByIDTable(barItem.idImpl);
                }
                else
                {
                    barItem.idImpl = MacroDefine.INVALID_ID;
                }

                msg.Type = (byte)(nIndex + SETTING_TYPE.SETTING_TYPE_K0);

                setting.m_SettingType = (byte)barItem.typeItem;
                setting.m_SettingData = barItem.idImpl;
                msg.Value = setting;

                NetManager.GetNetManager().SendPacket(msg);
                break;

            case ACTION_OPTYPE.AOT_XINFA:
                break;

            case ACTION_OPTYPE.AOT_EMPTY:
                msg.Type = (byte)(nIndex + SETTING_TYPE.SETTING_TYPE_K0);

                setting.m_SettingType = (byte)barItem.typeItem;
                setting.m_SettingData = barItem.idImpl;
                msg.Value = setting;

                NetManager.GetNetManager().SendPacket(msg);
                break;

            default:
                break;
        }
    }
    //设置主菜单上项目，从服务器传过来
    public void MainMenuBar_SetID(int nIndex, int idType, int idData)
    {
        if (nIndex < 0 || nIndex > s_MainMenuBarNum) return;

        //LogManager.LogWarning("Update MenuBar : " + nIndex);
        // 服务器端非法值是0 [1/13/2011 ivan edit]
        if (idType == (int)ACTION_OPTYPE.AOT_EMPTY)
        {
            m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_EMPTY;
            m_barMain[nIndex].idImpl = MacroDefine.INVALID_ID;
            m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;

            UpdateToolBar();
            return;
        }

        switch ((ACTION_OPTYPE)idType)
        {
            case ACTION_OPTYPE.AOT_SKILL:
                {
                    m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_SKILL;
                    m_barMain[nIndex].idImpl = idData;
                    m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;
                }
                break;
            case ACTION_OPTYPE.AOT_ITEM:
                {
                    m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_ITEM;
                    m_barMain[nIndex].idImpl = idData;
                    m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;

                }
                break;
            case ACTION_OPTYPE.AOT_PET_SKILL:
                {
                    m_barMain[nIndex].typeItem = ACTION_OPTYPE.AOT_PET_SKILL;
                    m_barMain[nIndex].idImpl = idData & 0xFFFF;
                    m_barMain[nIndex].idActionItem = MacroDefine.INVALID_ID;
                    m_barMain[nIndex].idPetIndex = (idData >> 16) & 0xFFFF;
                }
                break;

            default:
                break;
        }
    }

    //取得主菜单上项目
    public int MainMenuBar_Get(int nIndex)
    {
        if (nIndex < 0 || nIndex > s_MainMenuBarNum) return -1;

        return m_barMain[nIndex].idActionItem;
    }

    public void UpdateToolBar()
    {
        // 保存MainMenuBar上修改过的快捷键

        ///////////////////////////////////////////////

        string szTemp = "";

        string szRightTemp = "";

        int nPet_Num = -1;
        ///////////////////////////////////////////////
        //MainMenu Bar

        for (int i = 0; i < (int)m_barMain.Length; i++)
        {
            //如果尚未引用
            if (m_barMain[i].idActionItem == MacroDefine.INVALID_ID ||

                null == GetActionByActionId(m_barMain[i].idActionItem) ||

                m_barMain[i].typeItem == ACTION_OPTYPE.AOT_PET_SKILL)
            {
                //查询是否可以引用
                tActionItem pActionItem = null;
                switch (m_barMain[i].typeItem)
                {
                    case ACTION_OPTYPE.AOT_PET_SKILL:
                        pActionItem = GetAction_PetSkillID(m_barMain[i].idPetIndex, m_barMain[i].idImpl);	
                        break;
                    case ACTION_OPTYPE.AOT_SKILL:
                        pActionItem = GetAction_SkillID(m_barMain[i].idImpl);
                        break;
                    case ACTION_OPTYPE.AOT_SKILL_LIFEABILITY:
                        pActionItem = GetAction_ItemIDTable_FromMyPacket(m_barMain[i].idImpl);
                        if (pActionItem != null)
                        {
                            m_barMain[i].idItemCount = CDataPool.Instance.UserBag_CountItemByIDTable(m_barMain[i].idImpl);
                        }
                        else
                        {
                            m_barMain[i].idImpl = MacroDefine.INVALID_ID;
                        }
                        break;
                    case ACTION_OPTYPE.AOT_XINFA:
                        break;
                    case ACTION_OPTYPE.AOT_EMPTY:
                        break;
                    case ACTION_OPTYPE.AOT_CHATMOOD:
                        //TODO:
                        break;
                    default:
                        break;
                }

                int idAction;

                if (pActionItem != null)
                {

                    m_barMain[i].idActionItem = pActionItem.GetID();

                    idAction = pActionItem.GetID();
                }
                else
                {
                    idAction = MacroDefine.INVALID_ID;
                }

                //可以引用了，给UI发送事件

                // int [] szP = new int[32];

                List<string> vParam = new List<string>();

                vParam.Add("main");

                vParam.Add(Convert.ToString(i + 1));

                vParam.Add(Convert.ToString(idAction));

                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CHANGE_BAR, vParam);

            }
        }

    }

    public void ItemBox_Update()
    {
	    CleanInvalidAction();
	    //创建新的物品
	    for(int i=0; i<GAMEDEFINE.MAX_BOXITEM_NUMBER; i++)
	    {
		    //取得物品
		    CObject_Item pItem = CDataPool.Instance.ItemBox_GetItem(i);
		    if( pItem == null) continue;

		    UpdateAction_FromItem(pItem);
	    }
    }

    // 刷新所有的背包物品 [1/6/2012 Ivan]
    public void UserBag_Update()
    {
        CleanInvalidAction();

        for (int i = 0; i < GAMEDEFINE.MAX_BAG_SIZE; i++)
        {
            //取得物品
            CObject_Item item = (CObject_Item)CDataPool.Instance.UserBag_GetItemByIndex(i);
            if (item != null)
            {
                //设置新的物品数据
                UpdateAction_FromItem(item);
            }
        }
    }
	
	public void UserTalismanBag_Update()
	{
		CleanInvalidAction();

        for (int i = 0; i < CTalisman_Inventory.MAX_TALISMAN_ITEM_NUMBER; i++)
        {
            //取得物品
            CObject_Item item = (CObject_Item)CDataPool.Instance.TalismanInventory_GetItem(i);
            if (item != null)
            {
                //设置新的物品数据
                UpdateAction_FromTalismanItem(item);
            }
        }
	}
	
	public void UserTalismanEquip_Update()
	{
		CleanInvalidAction();
        for (int i = 0; i < CTalisman_Equipments.MAX_TALISMAN_EQUIPT_NUMBER; i++)
        {
            //取得物品
            CObject_Item item = (CObject_Item)CDataPool.Instance.TalismanEquipment_GetItem(i);
            if (item != null)
            {
                //设置新的物品数据
                UpdateAction_FromTalismanItem(item);
            }
        }
	}
	
	public void UpdateAction_FromTalismanItem(CObject_Item item)
	{
		if (item == null)
            return;
        //检查时候已经有该操作
        CActionItem_Talisman actionItem = (CActionItem_Talisman)GetAction_TalismanID(item.GetID());
        if (actionItem == null)
        {
            if (item != null)
            {
                CActionItem_Talisman itemAction = new CActionItem_Talisman(CreateID());
                itemAction.Update_Item(item as CTalisman_Item);

                List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_TALISMAN);
                actionItems.Add(itemAction);
            }
        }
        else//重新刷新数据
        {
            actionItem.Update_Item(item as CTalisman_Item);
        }
	}
	
	public CActionItem_Talisman GetAction_TalismanID(int id)
	{
		if (m_mapAllAction.ContainsKey((int)ACTION_OPTYPE.AOT_TALISMAN))
        {
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_TALISMAN);
            foreach (CActionItem kvp in actionItems)
            {
                if (kvp != null)
                {
                    CTalisman_Item item = (CTalisman_Item)kvp.GetImpl();
                    if (item.GetID() == id)
                    {
                        return (CActionItem_Talisman)kvp;
                    }
                }
            }
        }
        return null;
	}
	
	
    public void UserLifeAbility_Update()
    {
        CleanInvalidAction();
        //技能
        Dictionary<int, SCLIENT_LIFEABILITY> mapLifeAbility =
            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_LifeAbility();

        foreach (KeyValuePair<int, SCLIENT_LIFEABILITY> lifeAb in mapLifeAbility)
        {
            UpdateAction_FromLifeAbility(lifeAb.Value);
        }
        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_UPDATE_LIFETALISMAN_PAGE);
    }
    public void SkillClass_Update()
    {
        CleanInvalidAction();
        //心法
        Dictionary<int, SCLIENT_SKILLCLASS> skillClasses = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_SkillClass();
        foreach (KeyValuePair<int, SCLIENT_SKILLCLASS> skillClassPair in skillClasses)
        {
            SCLIENT_SKILLCLASS skillClass = skillClassPair.Value;
            UpdateAction_FromSkillClass(skillClass);
        }
    }


    public void UserSkill_Update()
    {
        CleanInvalidAction();
        //技能列表
        if (CObjectManager.Instance.getPlayerMySelf() != null)
        {
            Dictionary<int, SCLIENT_SKILL> mapSkill = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Skill();

            foreach (KeyValuePair<int, SCLIENT_SKILL> skill in mapSkill)
            {
                UpdateAction_FromSkill(skill.Value);
            }
        }
    }

    public void UpdateAction_FromSkill(SCLIENT_SKILL pSkill)
    {
        CActionItem_Skill pSkillAction = (CActionItem_Skill)GetAction_SkillID(pSkill.m_pDefine.m_nID * 100 + pSkill.m_nLevel);

        //AxTrace(0, 0, "Skill:%d", pSkill->m_pDefine->m_dwID);
        //如果没有,创建
        if (pSkillAction == null)
        {
            pSkillAction = new CActionItem_Skill(CreateID());
            pSkillAction.Update_Skill(pSkill);
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_SKILL);
            actionItems.Add(pSkillAction);
            UpdateToolBar();

            // 如果没有设置过默认技能 [8/25/2011 ivan edit]
            if (GetDefaultAction() == null)
            {
                //如果有上次存的默认技能，则设置为它。
                int defaultSkill = 351;//GameProcedure.s_pVariableSystem.GetAs_Int("DefaultSkill");
                // 			if (defaultSkill == 0)
                // 			{
                // 				// 如果没有设置过默认技能，则设置为对应的五行第一个技能 [11/14/2011 Ivan edit]
                // 				// 按顺序排序 金:301 水:401 火:451 土:501 木351 [8/26/2011 ivan edit]
                switch (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_MenPai())
                {
                    case 0:
                        defaultSkill = 351;
                        break;
                    case 1:
                        defaultSkill = 401;
                        break;
                    case 2:
                        defaultSkill = 451;
                        break;
                    case 3:
                        defaultSkill = 501;
                        break;
                }
                // 				if( pSkill->m_pDefine->m_nID == defaultSkill )
                // 				{
                // 					CGameInterface::GetMe()->Skill_SetActive(pSkillAction);
                // 				}
                // 			}
                // 			else 

                //if( pSkill->m_pDefine->m_nID*100 + pSkill->m_nLevel == defaultSkill )
                if (pSkill.m_pDefine.m_nID == defaultSkill)
                {
                    //LogManager.Log("SetActiveSkill" + defaultSkill);
                    CObjectManager.Instance.getPlayerMySelf().SetActiveSkill(pSkillAction);
                }
            }
        }
        else
        {
            pSkillAction.Update_Skill(pSkill);
        }
    }

    public void UpdateCoolDown(int nCoolDownID)
    {
        foreach (List<CActionItem> subItems in m_mapAllAction.Values)
        {
            foreach (CActionItem pActionItem in subItems)
            {
                if (pActionItem.GetCoolDownID() == nCoolDownID)
                {
                    pActionItem.UpdateCoolDown();
                }
            }
        }
    }

    public void UpdateAction_FromSkillClass(SCLIENT_SKILLCLASS pSkillClass)
    {
        CActionItem_XinFa pSkillClassAction = (CActionItem_XinFa)GetAction_SkillClass(pSkillClass.m_pDefine.nID);

        //如果没有,创建
        if (pSkillClassAction == null)
        {
            pSkillClassAction = new CActionItem_XinFa(CreateID());
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_XINFA);
            actionItems.Add(pSkillClassAction);
        }

        pSkillClassAction.Update_SkillClass(pSkillClass);
    }

    public CActionItem_XinFa GetAction_SkillClass(int idXinFa)
    {
        if (m_mapAllAction.ContainsKey((int)ACTION_OPTYPE.AOT_XINFA))
        {
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_XINFA);
            foreach (CActionItem kvp in actionItems)
            {
                if (kvp != null)
                {
                    SCLIENT_SKILLCLASS pSkillCLass = (SCLIENT_SKILLCLASS)kvp.GetImpl();
                    if (pSkillCLass != null && pSkillCLass.m_pDefine.nID == idXinFa) return (CActionItem_XinFa)kvp;
                }
            }
        }
        return null;
    }

    public int GetSkillActionNum()
    {
        List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_SKILL);
        return actionItems.Count;
    }
    public int GetActionNum(ActionNameType nameType)
    {
        int nNum = 0;
        if (nameType == ActionNameType.boothItem)
        {
            List<CActionItem> actions = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
            foreach (CActionItem action in actions)
            {
                if (action.GetType_String() == nameType)
                    nNum++;
            }
        }
        else if (nameType == ActionNameType.Skill)
        {
            List<CActionItem> actions = GetSubMapByClass(ACTION_OPTYPE.AOT_SKILL);
            return actions.Count;
        }

        return nNum;
    }

    public CActionItem EnumAction(int index, ActionNameType nameType)
    {
        List<CActionItem> actionItems = null;
        switch (nameType)
        {
            case ActionNameType.UnName:
                break;
            case ActionNameType.Skill:
                {
                    actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_SKILL);
                }
                break;
            case ActionNameType.LifeSkill:
                break;
            case ActionNameType.Xinfa:
                break;
            case ActionNameType.PackageItem:
            case ActionNameType.Equip:
                {
                    actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
                }
                break;
            case ActionNameType.boothItem:
                actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
                break;
            case ActionNameType.bankItem:
                actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_ITEM);
                break;
            default:
                break;
        }
        if (actionItems == null)
            return null;

        int tempCount = 0;
        foreach (CActionItem item in actionItems)
        {
            if (item.GetType_String() == nameType)
            {
                if (nameType == ActionNameType.Skill)
                {
                    if (tempCount == index)
                    {
                        return item;
                    }
                    tempCount++;
                }
                else if (item.GetPosIndex() == index)
                {
                    return item;
                }
            }
        }
        return null;
    }
    //该接口主要是给界面显示来调用，其原理为只要有技能的才能返回值
    public CActionItem EnumPetAction(int petIndex,int SkillIndex)
    {
        List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_PET_SKILL);
        if (actionItems != null)
        {
            int skillCount = 0;
            foreach (CActionItem item in actionItems)
            {
                CActionItem_PetSkill petSkill = item as CActionItem_PetSkill;
                //modified by ss 将getnum 接口换成 getpetindex
                if (item != null && petSkill.GetPetIndex() == petIndex)
                {
                    if(skillCount == SkillIndex)
                    {
                        return item;
                    }
                    else
                    {
                        skillCount++;
                    }
                }
                
            }
        }
        return null;
    }


    #region tActionSystem Members
    // 如果知道id是神马类型的对象，请不要使用这个函数来查找action，很耗资源 [2/11/2012 Ivan]
    public CActionItem GetActionByItemId(int itemId)
    {
        foreach (int id in m_mapAllAction.Keys)
        {
            List<CActionItem> actions = m_mapAllAction[id];
            List<CActionItem>.Enumerator enumerator = actions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CActionItem Current = enumerator.Current;
                CObject_Item item = Current.GetImpl() as CObject_Item;
                if (item != null && item.GetID() == itemId)
                {
                    return Current;
                }
            }
        }
        return null;
    }

    public CActionItem GetActionByActionId(int actionId)
    {
        foreach (int id in m_mapAllAction.Keys)
        {
            List<CActionItem> actions = m_mapAllAction[id];
            List<CActionItem>.Enumerator enumerator = actions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CActionItem Current = enumerator.Current;
                if (Current != null && Current.GetID() == actionId)
                {
                    return Current;
                }
            }
        }
        return null;
    }

    public tActionItem GetTooltipsFocus()
    {
        return SuperTooltips.Instance.GetActionItem();
    }

    public void SaveAction()
    {

    }

    public tActionItem GetDefaultAction()
    {
        return m_pDefaultAction;
    }

    public void SetDefaultAction(tActionItem pDefAction)
    {
        if (m_pDefaultAction != pDefAction && m_pDefaultAction != null)
        {
            //((CActionItem)m_pDefaultAction).SetDefaultState(false);
        }

        m_pDefaultAction = pDefAction;

        if (m_pDefaultAction != null)
        {
            // ((CActionItem)m_pDefaultAction).SetDefaultState(true);
        }
    }

    public void UpdateLinkByItemTable(int nIdTable)
    {
    }
    #endregion

    internal void OtherPlayerEquip_Update()
    {
        //清空已经不存在的Action
        CleanInvalidAction();

        // 装备点的最大个数.
        for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
        {
            //取得玩家身上的装备
            CObject_Item pItem = CDataPool.Instance.OtherPlayerEquip_GetItem((HUMAN_EQUIP)i);
            if (pItem != null)
            {
                //创建新的装备的action item
                UpdateAction_FromItem(pItem);
            }
        }
    }

    // 通过数据池中的数据, 创建出来装备action item
    internal void UserEquip_Update()
    {
        //清空已经不存在的Action
        CleanInvalidAction();

        // 装备点的最大个数.
        for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
        {
            //取得玩家身上的装备
            CObject_Item pItem = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)i);
            if (pItem != null)
            {
                //创建新的装备的action item
                UpdateAction_FromItem(pItem);
            }
        }
    }

    public void UserPetEquiptItem_Update(PET_GUID_t guid)
    {
        //清空已经不存在的Action
        CleanInvalidAction();

        // 装备点的最大个数.
        for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
        {
            //取得宠物身上的装备
            CObject_Item pItem = CDataPool.Instance.UserPetEquipt_GetItem(guid, (PET_EQUIP)i);
            if (pItem != null)
            {
                //创建新的装备的action item
                UpdateAction_FromItem(pItem);
            }
        }
    }

    public void OtherPlayerPetEquip_Update(PET_GUID_t guid)
    {
        //清空已经不存在的Action
        CleanInvalidAction();

        // 装备点的最大个数.
        for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
        {
            //取得宠物身上的装备
            CObject_Item pItem = CDataPool.Instance.OtherPlayerPetEquip_GetItem(guid, (PET_EQUIP)i);
            if (pItem != null)
            {
                //创建新的装备的action item
                UpdateAction_FromItem(pItem);
            }
        }
    }

    public void UserPetSkill_Update(int nPetNum)
    {
	    CleanInvalidAction();
	    if( nPetNum < 0 || nPetNum >= (int)PET_INDEX.PET_INDEX_SELF_NUMBERS )
		    return;
	    //技能列表
	    for(int i=0; i < GAMEDEFINE.MAX_PET_SKILL_COUNT; i++)
	    {
		    PET_SKILL arraySkill = CDataPool.Instance.Pet_GetSkill(nPetNum,i);
		    if(arraySkill == null || (arraySkill.m_pDefine== null)) continue;
		    UpdateAction_FromPetSkill(nPetNum,arraySkill);
	    }
	    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_PET_PAGE);
    }
    
    public void UpdateAction_FromPetSkill(int nPetNum, PET_SKILL pSkill)
    {
        if (pSkill == null)
            throw new NullReferenceException("pet skill is null in UpdateAction_FromPetSkill()");

	    //检查时候已经有该操作

	    CActionItem_PetSkill pSkillAction = GetAction_PetSkillID(nPetNum,pSkill.m_pDefine.m_nID) as CActionItem_PetSkill;
    	
	    //AxTrace(0, 0, "Skill:%d", pSkill->m_pDefine->m_dwID);
	    //如果没有,创建
	    if(pSkillAction == null)
	    {
		    pSkillAction = new CActionItem_PetSkill(CreateID());
		    pSkillAction.Update_PetSkill(pSkill);
            List<CActionItem> actionItems = GetSubMapByClass(ACTION_OPTYPE.AOT_PET_SKILL);
            actionItems.Add(pSkillAction);
		    UpdateToolBar();
	    }
	    else
	    {
		    pSkillAction.Update_PetSkill(pSkill);
		    UpdateToolBar();
	    }
    }

    public void Booth_Update()
    {
	    CleanInvalidAction();
	    //创建新的物品
	    for(int i=0; i<GAMEDEFINE.MAX_BOOTH_NUMBER; i++)
	    {
		    //取得物品
		    CObject_Item pItem = CDataPool.Instance.Booth_GetItem(i);
		    if(pItem == null) continue;

		    UpdateAction_FromItem(pItem);
	    }

	    //回购商品刷新
	    for(int i=0; i<GAMEDEFINE.MAX_BOOTH_SOLD_NUMBER ; i++)
	    {
		    //取得物品
		    CObject_Item pItem = CDataPool.Instance.Booth_GetSoldItem(i);
		    if(null==pItem) continue;

		    UpdateAction_FromItem(pItem);
	    }
    }
    //当前银行租赁箱的编号
	public void		SetCurBankRentBoxIndex(int nCurBankRentBoxIndex){m_nCurBankRentBoxIndex=nCurBankRentBoxIndex;}
	public int		GetCurBankRentBoxIndex(){return m_nCurBankRentBoxIndex;}
    public void ItemBox_Created(int nObjId)
    {
	    //清空原有ItemBox中的所有物品Action
	    CleanInvalidAction();
	    //创建新的物品
	    for(int i=0; i<GAMEDEFINE.MAX_BOXITEM_NUMBER; i++)
	    {
		    //取得物品
		    CObject_Item pItem = CDataPool.Instance.ItemBox_GetItem(i);
		    if( pItem != null ) continue;

		    UpdateAction_FromItem(pItem);
	    }

	    //产生事件打开UI箱子
	    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_LOOT_OPENED,nObjId);
    }

    public void UpdateToolBarForPetSkill()
    {
	    CleanInvalidAction();
	    for(int i=0; i<m_barMain.Length; i++)
	    {
		    ACTIONBAR_ITEM barItem = m_barMain[i];

		    if(	barItem.typeItem == ACTION_OPTYPE.AOT_PET_SKILL)
		    {
			    //查询是否可以引用
			    tActionItem pActionItem = null;

			    pActionItem = GetAction_PetSkillID(barItem.idPetIndex,barItem.idImpl);
			    if(pActionItem != null)
			    {
				    MainMenuBar_Set(i,MacroDefine.INVALID_ID);
				    continue;
			    }
    ///////////////////////////////////////////////////
			    //要是以后策划改主意了，就把这段注释掉。
                CActionItem_PetSkill petSkill = pActionItem as CActionItem_PetSkill; //modified by ss 将getnum 接口换成 getpetindex
                int nPet_Num = petSkill.GetPetIndex();
			    if(nPet_Num >= 0 && nPet_Num < GAMEDEFINE.HUMAN_PET_MAX_COUNT )
			    {
				    SDATA_PET My_Pet = CDataPool.Instance.Pet_GetPet(nPet_Num);
                    if (!CObjectManager.Instance.getPlayerMySelf().GetCharacterData().isFightPet(My_Pet.GUID))
				    {
					    MainMenuBar_Set(i,MacroDefine.INVALID_ID);
					    continue;
				    }
			    }
    		
    ////////////////////////////////////////////////////			
			    int idAction;
    			
			    if(pActionItem != null)
			    {
				    barItem.idActionItem = pActionItem.GetID();
				    idAction = pActionItem.GetID();
			    }
			    else
			    {
				    idAction = MacroDefine.INVALID_ID;
				    MainMenuBar_Set(i,MacroDefine.INVALID_ID);
			    }

			    //可以引用了，给UI发送事件
			    string temp="";
                List<string> vParam = new List<string>();
			    vParam.Add("main");

			    temp = ""+ (i+1);
			    vParam.Add(temp);
                temp = ""+ idAction;
			    vParam.Add(temp);
    			
			    if(idAction != MacroDefine.INVALID_ID)
			    {
    //				_snprintf(szTemp, 32, "%d", pActionItem->GetNum());
    // 以后策划要是变想法了，就把上面这句恢复回来。把下面那句注释起来。
				    temp = "" +  MacroDefine.INVALID_ID;
			    }
			    else
			    {
				    temp = "" +  MacroDefine.INVALID_ID;
				    vParam.Add(temp);
			    }
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_CHANGE_BAR, vParam);
		    }

	    }
    }
}

public enum ActionNameType
{
    UnName = -1,
    Skill = 0,
    LifeSkill,
    PackageItem,
    Xinfa,
    Equip,
    PetSkill,
    boothItem, //npc商店
    boothItemCallBack, //回购
    bankItem,
	talismanItem,//法宝栏
	talismanEquip//法宝装备栏
}

public class CActionItem : tActionItem
{
    #region CActionItem Members
    //ID(客户端内部标示)
    protected int m_ID;
    //名称
    protected string m_strName;
    //图标
    protected string m_strIconName;
    //UI引用
    protected List<tActionReference> m_setReference = new List<tActionReference>();
    //Checked
    protected bool m_bChecked;
    // 有效无效
    protected bool m_bEnabled;
    protected bool m_bLocked;

    //非法ActionItem
    public static CActionItem s_InvalidAction = new CActionItem(0);

    public CActionItem(int id)
    {
        m_ID = id;
        m_bChecked = false;
        m_bEnabled = true;
    }

    ~CActionItem()
    {
        //             if(GameProcedure.s_pGameInterface && this==CGameProcedure.s_pGameInterface.Skill_GetActive())
        // 	        {
        // 		        CGameInterface.GetMe().Skill_SetActive(0);
        // 	        }
        // 
        // 	        if(CGameProcedure.s_pActionSystem && this==CActionSystem.GetMe().GetDefaultAction())
        // 	        {
        // 		        CActionSystem.GetMe().SetDefaultAction(0);
        // 	        }
    }

    ////zzh+ for shop
    public int GetEquipPoint() { return -1; }
    ////zzh+ for shop
    public string GetItemColorInShop() { return ""; }

    // 得到物品的耐久
    //         public virtual int GetItemDur()
    //         {
    //             CObject_Item pItem = (CObject_Item)GetImpl();
    // 
    //             if (pItem != null)
    //             {
    //                 return pItem.GetItemDur();
    //             }
    //             else
    //             {
    //                 return -1;
    //             }
    //         }

    // 得到物品的最大耐久
    //         public virtual int GetItemMaxDur()
    //         {
    //             CObject_Item pItem = (CObject_Item)GetImpl();
    // 
    //             if (pItem != null)
    //             {
    //                 return pItem.GetItemMaxDur();
    //             }
    //             else
    //             {
    //                 return -1;
    //             }
    //         }

    //清空链接
    public void ClearRefrence()
    {
        foreach (tActionReference item in m_setReference)
        {
            item.BeDestroyed();
        }
    }

    //进入冷却
    public virtual void UpdateCoolDown()
    {
        int nCoolDownID = GetCoolDownID();

        //对于没有冷却的Action
        if (-1 == nCoolDownID)
        {
            foreach (tActionReference item in m_setReference)
            {
                item.EnterCoolDown(-1, 0.0f);
            }
            return;
        }

        //取得冷却组数据
        COOLDOWN_GROUP pCoolDownGroup =
                CDataPool.Instance.CoolDownGroup_Get(nCoolDownID);

        int nTimeNow = pCoolDownGroup.nTime;
        int nTimeTotal = pCoolDownGroup.nTotalTime;

        foreach (tActionReference it in m_setReference)
        {
            if (nTimeNow <= 0 || nTimeTotal <= 0)
            {
                it.EnterCoolDown(-1, 0.0f);
            }
            else
            {
                it.EnterCoolDown(nTimeTotal, 1.0f - (float)nTimeNow / (float)nTimeTotal);
            }
        }
    }

    //通知UI
    public void UpdateToRefrence()
    {
        foreach (tActionReference item in m_setReference)
        {
            item.UpdateRef(GetID());
        }
    }

    // 得到物品卖给npc的价格
    public int GetItemPrice()
    {
        return -1;
    }

    public int GetPrice()
    {
        return GetItemPrice();
    }

    //激活动作仅显示按钮动画
    public void DoActionPrepare(bool bCheck)
    {
        foreach (tActionReference item in m_setReference)
        {
            item.SetCheck(bCheck);
        }
    }
    //进入公共冷却
    public void UpdateCommonCoolDown()
    {
        //药品也要走cooldown
        object impleData = (object)GetImpl();
        if (impleData == null)
            return;

        CObject_Item_Medicine pItem = impleData as CObject_Item_Medicine;
        if (pItem == null)
            return;

        //针对技能Action
        if (GetType() != ACTION_OPTYPE.AOT_SKILL)
            return;

        //取得冷却组数据
        COOLDOWN_GROUP pCoolDownGroup =
                CDataPool.Instance.CoolDownGroup_Get(GetCoolDownID());
        if (pCoolDownGroup == null)
            return;

        //如果处于冷却中
        if (pCoolDownGroup.nTime > 0)
            return;

        int nCommonCoolDown = CDataPool.Instance.CommonCoolDown_Get();

        //通知UI，进入公共冷却
        foreach (tActionReference it in m_setReference)
        {
            it.EnterCoolDown(nCommonCoolDown, 1.0f - (float)nCommonCoolDown / (float)COMMONCOOLDOWN.COMMONCOOLDOWN_TIME);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------
    //-- super tool tip 使用
    //--
    // 得到物品的等级
    // 	    public virtual int				GetItemLevel(){return -1;}
    // 
    // 	    // 得到物品的修理次数
    // 	    public virtual int				GetItemRepairCount(){return -1;}
    // 
    // 	    // 得到物品的强化等级 [7/28/2011 edit by ZL] 
    // 	    public virtual int				GetStrengthLevel(){return -1;}
    // 
    // 	    // 得到物品的品级 [9/19/2011 edit by ZL]
    // 	    public virtual int				GetPinJi(){return -1;}
    // 
    // 	    // 得到物品的品级值 [9/19/2011 edit by ZL]
    // 	   public  virtual int				GetPinJiValue(){return -1;}
    // 
    // 	    // 得到物品的绑定信息 
    // 	    public virtual int				GetItemBindInfo();
    // 
    // 	    // 得到物品的制作人
    // 	   public  virtual string			GetManufacturer();
    // 
    // 	    // 得到白色属性
    // 	    public virtual string			GetBaseWhiteAttrInfo();
    // 
    // 	    // 得到扩展蓝色属性
    // 	    public virtual string			GetExtBlueAttrInfo();
    // 
    // 	    // 得到action所引用的item 的类型
    // 	   public  virtual int				GetItemType();
    // 
    // 	    // 得到类型描述
    // 	   public  virtual string			GetTypeDesc();

    //得到内部数据
    //得到冷却状组ID
    public override int GetCoolDownID() { return -1; }
    //得到所在容器的索引
    public override int GetPosIndex() { return -1; }
    //激活动作
    public override void DoAction()
    {
        //Todo:

    }
    //激活动作(副操作)
    public override void DoSubAction() { }

    //查询逻辑属性
    // 	  public   virtual string			GetAttributeValue(string szAttributeName){ return ""; }
    // 	    // 得到心法等级
    // 	    //virtual int				GetXinfaLevel(){return -1;}
    // 	    //聊天界面超链接的字符串
    // 	   public  virtual string			GenHyperLinkString() {	return ""; }
    // 
    // 	    // 是否需要在ToolTip上显示价格
    // 	    public virtual int				IsViewToolTopsPrice();
    // 	    // 获得在ToolTip上显示的价格
    // 	    public virtual int		 		GetViewToolTopsPrice();
    // 	    // 获取修理价格 [9/15/2010 Sun]
    // 	   public  virtual int				GetViewRepairPrice();
    // 	    // 获取出售价格
    // 	   public  virtual int				GetViewSoldPrice();

    // 得到物品档次等级描述 2006-5-18
    //public virtual int GetItemLevelDesc(){	return 1;}

    //--------------------------------------------
    //导出到脚本中的函数
    //friend class CScriptSvm;

    /*
    |	对于战斗技能, 是技能表中的ID (DBC_SKILL_DATA)
    |	对于宠物技能, 是技能表中的ID (DBC_SKILL_DATA)
    |	对于生活技能，是生活技能表中的ID(DBC_LIFEABILITY_DEFINE)
    |	对于物品，是物品表中的ID(DBC_ITEM_*)
    |	对于心法，是心法表中的ID(DBC_SKILL_XINFA)
    |
    */
    public virtual int GetIDTable() { return -1; }
    // 	    int		GetOwnerXinfa();	//!!只针对战斗技能，得到所属的心法
    // 	    //  [10/13/2011 Ivan edit]
    // 	    int		GetOwnerXinfaTableID();
    //  	    int		GetPetSkillOwner();//!!只针对宠物技能，得到技能是属于第几只宠物的。
    //  	    int		GetOwnerQuest();	//!!只针对任务奖励虚拟物品，得到物品是属于第几个任务的。
    //  	    int		GetPrice();	//得到价格
    //int		GetPosition() {return GetPosIndex();}
    // 获取物品五行 [12/29/2010 ivan edit]
    //int		GetWuXing();
    //const string GetShortIconName();


    #endregion

    #region tActionItem Members

    //查询逻辑属性
    public override string GetAttributeValue(string szAttributeName)
    {
        CObject_Item item = GetImpl() as CObject_Item;
        if (item != null)
            return item.GetAttributeValue(szAttributeName);
        return "";
    }

    public override int GetID()
    {
        return m_ID;
    }

    public override string GetName()
    {
        return m_strName;
    }

    public override string GetIconName()
    {
        return m_strIconName;
    }

    public override void SetCheckState(bool bCheck)
    {
        m_bChecked = bCheck;
        foreach (tActionReference item in m_setReference)
        {
            item.SetCheck(bCheck);
        }
    }

    public override void AddReference(tActionReference pRef, bool bIsInMenuToolbar)
    {
        //如果没有,加入
        if (!m_setReference.Contains(pRef))
        {
            m_setReference.Add(pRef);
        }
        //刷新Check信息
        pRef.SetCheck(m_bChecked);
        pRef.SetDefault(CActionSystem.Instance.GetDefaultAction() == this);

        switch (GetType())
        {
            case ACTION_OPTYPE.AOT_ITEM:
                {
                    //物品的每组的个数显示在右下角
                    CObject_Item pItem = (CObject_Item)GetImpl();

                    if (pItem.TypeOwner == ITEM_OWNER.IO_BOOTH
                        || pItem.TypeOwner == ITEM_OWNER.IO_BOOTH_CALLBACK
                        || pItem.TypeOwner == ITEM_OWNER.IO_CITY_SHOP)
                    {
                        int nGroupNumber = GetNum();
                        if (nGroupNumber > 1)
                        {
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, Convert.ToString(nGroupNumber));
                        }
                        else
                        {
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, "");
                        }

                        //有限物品的最大数量显示在左上角
                        int nMaxNumber = pItem.GetMax();
                        if (nMaxNumber > 1)
                        {
                            pRef.Enable();
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_TOPLEFT, Convert.ToString(nMaxNumber));
                        }
                        else if (nMaxNumber == 0)
                        {
                            pRef.Disable();
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_TOPLEFT, "");
                        }
                        else
                        {
                            pRef.Enable();
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_TOPLEFT, "");
                        }
                    }
                    else if (pItem.TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET
                        || pItem.TypeOwner == ITEM_OWNER.IO_MYSELF_BANK
                        || pItem.TypeOwner == ITEM_OWNER.IO_MYEXBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_OTHEREXBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_MISSIONBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_MYSTALLBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_OTSTALLBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_QUESTVIRTUALITEM
                        || pItem.TypeOwner == ITEM_OWNER.IO_ITEMBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_PS_SELFBOX
                        || pItem.TypeOwner == ITEM_OWNER.IO_PS_OTHERBOX
                        )
                    {
                        int nGroupNumber = GetNum();
                        if (bIsInMenuToolbar == true)
                        {
                            int num = CDataPool.Instance.UserBag_CountItemByIDTable(pItem.GetIdTable());
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, Convert.ToString(num));
                        }
                        else if (nGroupNumber > 1)
                        {
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, Convert.ToString(nGroupNumber));
                        }
                        else
                        {
                            pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, "");
                        }
                    }
                }
                break;

            default:
                pRef.SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, "");
                break;
        }

        //UpdateCoolDown();
    }

    public override void RemoveRefrence(tActionReference pRef)
    {
        if (!m_setReference.Contains(pRef))
            return;

        m_setReference.Remove(pRef);

        pRef.SetCheck(false);
        pRef.SetDefault(false);
    }

    public override bool IsEnabled()
    {
        return m_bEnabled;
    }

    public override void Enable()
    {
        m_bEnabled = true;
    }

    public override void Disable()
    {
        m_bEnabled = false;
    }

    public override bool CoolDownIsOver()
    {
        return true;
    }

    public override void NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName) { }

    public override void NotifyTooltipsShow()
    {
        //生成数据
        SuperTooltips.Instance.SetActionItem(this);

        //产生UI事件
        List<string> vParam = new List<string>();
        vParam.Add("1");
        vParam.Add(GetType_String().ToString());

        Vector2 ptMouse = GameProcedure.s_pInputSystem.GetMouseUIPos();

        vParam.Add(Convert.ToString(ptMouse.x));
        vParam.Add(Convert.ToString(ptMouse.y));

        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, vParam);
    }

    public override void NotifyTooltipsHide()
    {
        //生成数据
        SuperTooltips.Instance.SetActionItem(null);

        //隐藏Tooltip
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, "0");
    }

    public override void NotifyTooltips2Show()
    {
        //生成数据
        SuperTooltips.Instance.SetActionItem(this);

        //产生UI事件
        List<string> vParam = new List<string>();
        vParam.Add("1");
        vParam.Add(GetType_String().ToString());

        Vector2 ptMouse = GameProcedure.s_pInputSystem.GetMousePos();

        vParam.Add(Convert.ToString(ptMouse.x));
        vParam.Add(Convert.ToString(ptMouse.y));

        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP2, vParam);
    }

    public override void NotifyTooltips2Hide()
    {
        //生成数据
        SuperTooltips.Instance.SetActionItem(null);

        //隐藏Tooltip
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP2, "0");
    }

    public override void DestroyImpl()
    {

    }

    public override ACTION_OPTYPE GetType()
    {
        return ACTION_OPTYPE.AOT_EMPTY;
    }

    public override ActionNameType GetType_String()
    {
        return ActionNameType.UnName;
    }

    public override int GetDefineID()
    {
        return -1;
    }

    public override int GetNum()
    {
        return -1;
    }

    public override object GetImpl()
    {
        return null;
    }

    public override string GetDesc()
    {
        return "";
    }

    public override bool IsValidate() { return false; }
    #endregion
}
/*}*/
