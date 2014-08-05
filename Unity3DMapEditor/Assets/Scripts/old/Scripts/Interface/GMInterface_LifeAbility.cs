
/// <summary>
/// 拆分原来的类结构，将所有的装备的升级操作，以及材料合成等移入CompoundManager
/// 抽象原来的每个功能为单个Operator,例如装备强化EquipStrendthenOp
/// </summary>
using System;
using DBSystem;
using Network;
using Network.Packets;

//合成结果[2012/4/18 ZZY]
public enum COMPOSE_ITEM_RESULT
{
    COMPOSE_SUCCESS,
    COMPOSE_NO_ABILITY,
    COMPOSE_NO_ENOUGHSTUFF,
    COMPOSE_ERROR,
}
namespace Interface
{

    public class LifeAbility
    {
        private static LifeAbility _instance;
        public static LifeAbility Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LifeAbility();
                return _instance;
            }
        }

        public CompoundManager CompoundInstance
        {
            get
            {
                if (mCompoundManager == null)
                {
                    mCompoundManager = new CompoundManager();
                    mCompoundManager.InitializeOperator();
                }
                return mCompoundManager;
            }
        }

        CompoundManager mCompoundManager;

        
        CObject_Item_Equip mPreViewEquip;



        public int GetLifeAbility_Number(int nActionID)
        {
            return 0;
        }

        //测试强化功能
        public void testStrengthen(CObject_Item item)
        {
            CompoundInstance.setOperator(EquipStrengthenOp.Name);
           // CObject_Item item = CDataPool.Instance.UserBag_GetItemByIndex(2);
            CompoundInstance.setOperatorTarget(item);

            CompoundInstance.beginOperation();
        }

        //强化，装备升档
        public void equipOperate(CObject_Item item, string operate)
        {
            CompoundInstance.setOperator(operate);
            CompoundInstance.setOperatorTarget(item);
       
            string error = CompoundInstance.beginOperation();
            if(error != null)
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MESSAGEBOX, error);

        }

        //设置操作对象为人物，还是宠物
        public void setOperaterRole(int index)
        {
            CompoundInstance.setOperatorRole(index);
        }

        //镶嵌宝石
        public void Do_Enchase(CObject_Item equip, short[] pos)
        {
            CompoundInstance.setOperator(EquipEnchaseGemOp.Name);
            //装备
            CompoundInstance.setOperatorTarget(equip);
 
            //获取镶嵌位置
            CompoundInstance.setEnchaseGemPos(pos);

            string error = CompoundInstance.beginOperation();
            if (error != null)
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MESSAGEBOX, error);
        }
        //宝石合成
        public void Do_Combine(CObject_Item[] stuffs)
        {
            CompoundInstance.setOperator(GemCompoundOp.Name);
            foreach (CObject_Item item in stuffs)
            {
                CompoundInstance.setOperatorTarget(item);
            }
            string error = CompoundInstance.beginOperation();
            if (error != null)
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MESSAGEBOX, error);
        }
        // 宝石摘除 [7/16/2011 ivan edit]
        public int Do_SeparateGem(byte nIndexEquip, byte nIndexGem, byte nIndexMat)
        {
            CGRemoveGem msg = NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_REMOVEGEM) as CGRemoveGem;
            
            msg.GemIndex = nIndexGem;
            msg.MatBagIndex = nIndexMat;
            msg.EquipBagIndex = nIndexEquip;

            NetManager.GetNetManager().SendPacket(msg);

            return 0;
        }

        //配方
        public Stuff GetPrescrResult(int nPrescrID)
        {
            SCLIENT_PRESCR myPrescr = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr(nPrescrID);
            if (myPrescr == null)
                throw new NullReferenceException("can not found prescr id=" + nPrescrID + " in GetPrescrResult()");

            Stuff stuffPair = new Stuff();
            stuffPair.nID = myPrescr.m_pDefine.nResultID;
            stuffPair.nNum = myPrescr.m_pDefine.nResultNum;

            return stuffPair;
        }
        public int GetPrescrStuffCount(int nPrescrID)
        {
            SCLIENT_PRESCR myPrescr = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr(nPrescrID);
            if (myPrescr == null)
                throw new NullReferenceException("can not found prescr id=" + nPrescrID + " in GetPrescrStuffCount()");


            return myPrescr.m_pDefine.mStuffs.Length;

        }
        public Stuff GetPrescrStuff(int nIndex, int nPrescrID)
        {
            SCLIENT_PRESCR myPrescr = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr(nPrescrID);
            if (myPrescr == null)
                throw new NullReferenceException("can not found prescr id=" + nPrescrID + " in GetPrescrStuff()");
            if (nIndex >= myPrescr.m_pDefine.mStuffs.Length)
                throw new NullReferenceException("Not validate stuff index= " + nIndex + "in Prescr=" +nPrescrID);

            return myPrescr.m_pDefine.mStuffs[nIndex];
        }
        public int GetPrescrID(int nTableID)
        {
            return EquipUpLevelOp.getPrescrID(nTableID);
        }

        public COMPOSE_ITEM_RESULT ComposeItem_Begin(int nPrescrID, int nMakeCount)
        {
            if (nMakeCount < 1)
                return COMPOSE_ITEM_RESULT.COMPOSE_ERROR;
            bool bEnoughStuff = true;
            SCLIENT_PRESCR myPrescr = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr(nPrescrID);
            if (myPrescr == null)
			{
				//throw new NullReferenceException("can not found prescr id=" + nPrescrID + " in ComposeItem_Begin()");
				//告知玩家需要使用/学习什么生活技能 [2012/4/18 ZZY]
				_DBC_LIFEABILITY_ITEMCOMPOSE pDefine =
            	CDataBaseSystem.Instance.GetDataBase<_DBC_LIFEABILITY_ITEMCOMPOSE>((int)DataBaseStruct.DBC_LIFEABILITY_ITEMCOMPOSE).Search_Index_EQU(nPrescrID);
				CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "合成该物品前需要学习/使用:"+pDefine.szName);
                return COMPOSE_ITEM_RESULT.COMPOSE_NO_ABILITY;
			}
                
            for (int i = 0; i < myPrescr.m_pDefine.mStuffs.Length; i++ )
            {
                Stuff stuffNeed = myPrescr.m_pDefine.mStuffs[i];
                if (stuffNeed.nID != MacroDefine.INVALID_ID)
                {
                    int nCount = CDataPool.Instance.UserBag_CountItemByIDTable(stuffNeed.nID);
                    if (nCount < stuffNeed.nNum * nMakeCount)
                    {
                        bEnoughStuff = false;
                        break;
                    }
                }
            }
            if (!bEnoughStuff)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "材料不足");
                return COMPOSE_ITEM_RESULT.COMPOSE_NO_ENOUGHSTUFF;
            }
            GameProcedure.s_pGameInterface.Player_UseLifeAbility(nPrescrID, nMakeCount, 0);
            return COMPOSE_ITEM_RESULT.COMPOSE_SUCCESS;
        }
        //    // 通过编号去查该配方，共需要几个材料
        //    INT GetPrescr_Material_Number(INT	nPrescrID);
        //    INT GetPrescrMaterialNumberNew(INT nPrescrID);
        //    // 通过材料的编号，查询材料的详细信息，返回图标和名称。
        //    Lua_ICON_INFO	GetPrescr_Material(INT	nItemID);
        //    // 通过序号去查第几个配方的编号
        //    INT GetPrescrList_Item_FromNum(INT nIndex);
        //    INT GetPrescrList_Item_FromNum(INT nIndex, INT nLifeAbility);
        //    // 计算身上原料总共可以合成多少个这个配方。
        //    INT GetPrescr_Item_Maximum(INT nIndex);
        //    INT GetPrescrItemMaximumNew(INT nIndex, INT nMaterial);//  [10/8/2011 Sun]
        //    string GetPrescription_Kind(INT nPrescrKind);
        //    // 计算原料数量。
        //    INT GetPrescr_Material_Hold_Count(INT nIndex,INT nMaterial);
        //    INT GetPrescrMaterialCountInPackage(INT nIndex, INT nMaterial);// 根据新需求 [10/8/2011 Sun]
        //    // 刷新合成界面
        //    INT Update_Synthesize( INT	nAbility );
        //    // 打开宝石合成/镶嵌界面
        //    INT Open_Compose_Gem_Page( INT	nPage );
        //    // 开始镶嵌
        //    INT	Do_Enchase( INT nIndexEquip, INT nIndexGem, INT nIndexMat1 = -1, INT nIndexMat2 = -1 );
        //    // 开始合成
        //    INT	Do_Combine(INT nIndexGem1, INT nIndexGem2, INT nIndexGem3, INT nIndexGem4, INT nIndexGem5, INT nIndexGem6 );
        //    // 宝石摘除 [7/16/2011 ivan edit]
        //    INT	Do_SeparateGem(INT nIndexEquip, INT nIndexGem, INT nIndexMat);
        //    // 返回装备上，第i颗宝石的图标信息
        //    string	GetEquip_Gem( INT nIndexEquip , UINT nIndexGem );
        // 获取装备上的宝石数据 [7/16/2011 ivan edit]
        public string GetEquipGemInfo(CObject_Item_Equip equip, int nIndexGem, string dataType)
        {
            if (equip == null) return "";
            int nGemType = equip.GetGemTableId(nIndexGem);

            _DBC_ITEM_GEM pGem = ObjectSystem.GemDBC.Search_Index_EQU(nGemType);
            if (pGem == null) return "";

            string gemData = "";

            if (dataType == "icon")
            {
                gemData = pGem.szIcon;
            }
            else if (dataType == "level")
            {
                gemData = pGem.nQuality.ToString();
            }
            else if (dataType == "type")
            {
                gemData = pGem.nType.ToString();
            }
            else if (dataType == "id")
            {
                gemData = pGem.nID.ToString();
            }
            else if (dataType == "name")
            {
                gemData = pGem.szName;
            }
            else if (dataType == "attribute")
            {
                equip.GetGemExtAttr(nIndexGem, ref gemData);
            }

            return gemData;
        }
        //    // 装备镶嵌消息 [9/16/2011 edit by ZL]
        //    void Send_EnchaseMsg(int type);
        //    // 返回装备上，总共有多少颗宝石。
        //    INT GetEquip_GemCount(INT nIndexEquip );
        //    // 返回装备上，总共可以镶嵌多少个宝石	
        ////	INT GetEquip_GemCount( );
        //    // 返回宝石的级别。
        ////	INT Compound_Preparation(  );
        //    // 是否可以镶嵌
        //    INT	Can_Enchase( INT nIndexEquip, INT nIndexGem );
        //    // 是否可以合成
        //    bool Can_Combine( INT nIndexGem1, INT nIndexGem2 );
        //    // 物品可以放到镶嵌界面这个位置。
        //    bool Enchase_Preparation( INT nIndexInterface, INT nIndexPacket );
        //    // 物品可以放到合成界面这个位置。
        //    bool Compound_Preparation( INT nIndexPacket );
        //    //////////////////////////////////////////////////////////////////////////
        //    INT	Stiletto_GetEquipOwner();
        //    // 装备打孔检测,必须先将装备拖到目标位置上 [7/14/2011 ivan edit]
        //    Lua_SLOT_COST Stiletto_Preparation(INT nEquipIndex, INT equipOwner);
        //    // 装备打孔 [7/15/2011 ivan edit]
        //    INT	Do_Stilietto(int equipOwner = -1, int equipPos = -1);

        //    // 装备强化预览 [9/14/2011 edit by ZL]
        //    CActionItem*	GetStrengthenPreView(INT actionId);
        //    // 装备强化预览 [9/14/2011 edit by ZL]

        // 升档预览 [4/11/2012 SUN]
        public CActionItem	GetEquipUpdatePreView(int actionId) 
        {
            CActionItem_Item action = CActionSystem.Instance.GetActionByActionId(actionId) as CActionItem_Item;
            if(action == null)
                throw new NullReferenceException("Can not find actionitem: " + actionId);
            CObject_Item_Equip equip = action.GetImpl() as CObject_Item_Equip;
            if(equip == null) return null;
	        
            CObject_Item_Equip resEquip = CDataPool.Instance.PreviewEquip;
            int nNextTableId = equip.GetEquipBaseDefine().nNextDangCiItemSN;
            if(resEquip != null && resEquip.GetIdTable() != nNextTableId)
            {
                CDataPool.Instance.PreviewEquip = null;
            }

            resEquip = ObjectSystem.Instance.NewItem((uint)nNextTableId) as CObject_Item_Equip;
            resEquip.CloneForUpLevel(equip);
  
            resEquip.TypeOwner =  ITEM_OWNER.IO_QUESTVIRTUALITEM ;
            // 必须保存回去 [10/25/2011 Ivan edit]
            CDataPool.Instance.PreviewEquip = resEquip;

            CActionItem pActionItem = CActionSystem.Instance.GetAction_ItemID(resEquip.GetID(), false);
            if (pActionItem != null)
            {
                return pActionItem;
            }
            else
            {
                return CActionItem.s_InvalidAction;
            }
        }

        //    // 获取装备点 [7/14/2011 ivan edit]
        //    INT	Get_Equip_Point(INT nIndexPacket);
        public int Get_Equip_Point(CObject_Item My_Equip)
        {
            if (My_Equip == null || My_Equip.GetItemClass() != ITEM_CLASS.ICLASS_EQUIP)
            {
                return -1;
            }

            HUMAN_EQUIP equipType = ((CObject_Item_Equip)My_Equip).GetItemType();

            return (int)equipType;
        }
        //    // 取得装备的绑定状态 [9/15/2011 edit by ZL]
        //    INT	Get_Equip_Bind(INT nIndexPacket);
        //    // 获取装备等级 [7/18/2011 ivan edit]
        //    INT	Get_Equip_Level(INT nIndexPacket);
        //    // 获取装备强化等级 [7/18/2011 ivan edit]
        //    Lua_Item_Strength Get_Equip_CurStrengthLevel(INT nIndexPacket);
        public Lua_Item_Strength Get_Equip_CurStrengthLevel(CObject_Item My_Equip)
        {
            Lua_Item_Strength tmp = new Lua_Item_Strength();
            if (My_Equip == null || My_Equip.GetItemClass() != ITEM_CLASS.ICLASS_EQUIP)
            {
                return tmp;
            }
            int enhanceLevel = ((CObject_Item_Equip)My_Equip).GetStrengthLevel();
            tmp.currentLevel = enhanceLevel & 0xf;
            tmp.highLevel = enhanceLevel >> 4;
            return tmp;
        }
        //    // 强化需求 [7/19/2011 ivan edit]
        //public Item_Enhance Get_Equip_EnhanceLevel( int nIndexPacket)
        public Item_Enhance Get_Equip_EnhanceLevel(CObject_Item My_Equip)
        {
            Item_Enhance itemEnhance = new Item_Enhance();

            if (My_Equip == null || My_Equip.GetItemClass() != ITEM_CLASS.ICLASS_EQUIP)
            {
                return itemEnhance;
            }
            CObject_Item_Equip equip = My_Equip as CObject_Item_Equip;
            if(equip == null)
                return itemEnhance;

            int enhanceID = equip.GetStrengthIndex();
            _DBC_ITEM_ENHANCE pItemEnhance = ObjectSystem.EquipEnchanceDBC.Search_Index_EQU((int)enhanceID);
            if (pItemEnhance == null)
                return itemEnhance;
            
            _DBC_ITEM_ENCHANCE_RATE pItemEnchanceRate = ObjectSystem.EquipEnchanceRateDBC.Search_Index_EQU(equip.GetCurrentDangCi());
            float fRatio = pItemEnchanceRate.nDangCiRatio /100.0f;
          
            pItemEnchanceRate = ObjectSystem.EquipEnchanceRateDBC.Search_Index_EQU((int)equip.GetItemType());
            fRatio *= pItemEnchanceRate.nEquipPointRatio / 100.0f;

            itemEnhance.needMoney = (int)(fRatio * pItemEnhance.needMoney + 0.5f);

            itemEnhance.addProperty = pItemEnhance.attribute[0];// 因为增加属性的百分比都一样，所以只获取一个就够了[7/19/2011 ivan edit]

            return itemEnhance;

        }

        //获取装备升档消耗的材料
        public Lua_Item_LevelUp Get_Equip_LevelUpMaterial(int nLevel)
        {
            Lua_Item_LevelUp item = new Lua_Item_LevelUp();
            _DBC_ITEM_UPLEVEL itemUpLevel = ObjectSystem.EquipUpLevelDBC.Search_Index_EQU(nLevel);
            if (itemUpLevel == null)
                return item;
            item.needItemID = itemUpLevel.needItem1ID;
            item.needItemID2 = itemUpLevel.needItem2ID;
            item.needItemNum = itemUpLevel.needItem1Num;
            item.needItem2Num = itemUpLevel.needItem2Num;
            item.money = itemUpLevel.nMoney;
            return item;
        }

        
        //    //////////////////////////////////////////////////////////////////////////
        //    // 装备品级 [9/19/2011 edit by ZL]
        //    INT Get_Equip_Quality(INT nIndexPacket);
        //    // 装备品级值 [9/19/2011 edit by ZL]
        //    INT Get_Equip_Quality_Value(INT nIndexPacket);
        //    // 装备升品需要的品级值 [9/21/2011 edit by ZL]
        //    INT Get_Equip_NextQuality_Value(INT nIndexPacket);
        //    // 材料提升的品级值 [9/19/2011 edit by ZL]
        //    INT Get_Equip_Upate_Quality_Value(INT nIndexPacket);
        //    // 增加的品级值提升的基础属性 [9/19/2011 edit by ZL]
        //    INT Get_Equip_Upate_Property_Value(INT nIndexPacket, INT Quality);
        //    // 装备升品需要的金钱 [9/19/2011 edit by ZL]
        //    INT Get_Equip_Upate_Money(INT Quality);
        //    // 装备升品 [9/19/2011 edit by ZL]
        //    INT Equip_Quality_Upate(INT epos, INT mpos1, INT mpos2, INT mpos3, INT mpos4, INT mpos5, INT mpos6, INT mpos7, INT mpos8, INT mpos9, INT mpos10);
        //    //////////////////////////////////////////////////////////////////////////
        //    // 装备的档次 [9/22/2011 edit by ZL]
        //    INT Get_Equip_DangCi(INT nIndexPacket);
        //    //////////////////////////////////////////////////////////////////////////
        // 把物品栏某位置的物品置灰。
        public  void Lock_Packet_Item( int nIndex, bool isLock )
        {
	        if(nIndex >= 0)
	        {
		        CObject_Item pObj = CDataPool.Instance.UserBag_GetItem(nIndex);
		        if(pObj != null)
		        {
			        pObj.SetLock(isLock);

		        }
	        }

        }
        // 把装备栏某位置的物品置灰 [7/15/2011 ivan edit]
        public void Lock_Equip_Item(int nIndex, bool isLock)
        {
            if(nIndex >= 0)
	        {
		        CObject_Item pObj = CDataPool.Instance.UserEquip_GetItem((HUMAN_EQUIP)nIndex);
		        if(pObj != null)
		        {
			        // 设置装备栏物品状态，是否锁定 [7/15/2011 ivan edit]
			        CActionItem_Item pItemAction = CActionSystem.Instance.GetAction_ItemID(pObj.GetID());

			        if(isLock)
			        {
				        pObj.SetLock(true);
				        if(pItemAction != null)
					        pItemAction.Disable();
			        }
			        else
			        {
				        pObj.SetLock(false);
				        if(pItemAction!=null)
					        pItemAction.Enable();
			        }

		        }
	        }
        }
        //    // 取得配方的详细描述
        //    string	GetPrescr_Explain(INT nIndex);
        //    // 生活技能界面关心的Npc
        //    INT GetNpcId(  );
        //    // temp [3/1/2010 Sun]
        //    INT GetLifeAbility_LimitExp(INT nLifeSkillID, INT nLevel);

        //    //// zzh+
        //    INT Get_UserEquip_VisualID(INT nIndex);

        //    //  [11/3/2010 Sun]
        //    //简单描述
        //    Lua_SimpleTooltip GetPrescr_Material_Tooltip(INT nItemID);
        //    // 消耗活力和精力
        //    Lua_COMMON_NUM GetPrescr_Consume_Vigor_Energy(INT nPrescr);
        //    // 消耗门派贡献度
        //    INT GetPrescr_Consume_ContriAttr(INT nPrescr);
        //    // 仙石需求 ，1绑定仙石，2非绑定仙石
        //    Lua_COMMON_NUM GetPrescrConsumeXianShi(INT nPrescr);
        //    // 灵石需求
        //    INT GetPrescrConsumeLingShi(INT nPrescr);

        //    //获取特殊材料
        //    INT GetPrescr_Item_IsNeedSpecial(INT nPrescr);

        //    CActionItem* GetPreViewEquip(INT idTable);
        //    //获取材料，如果背包中没有创建一个虚拟的物品用于显示
        //    CActionItem* GetPrescrMaterialItem(INT idTable);

    }

    // 强化消耗 [7/19/2011 ivan edit]
    public struct Item_Enhance
    {//tolua_export

        //tolua_begin
        public int needMoney;
        public int successRate;
        public int addProperty;
        //tolua_end
        public Item_Enhance(int nMoney,int rate,int property)
        {
            needMoney = nMoney;
            successRate = rate;
            addProperty = property;
        }
    } //tolua_export

    //强化等级，高位曾经最高强化等级，低位当前等级[9/8/2011 edit by ZL]
    public struct Lua_Item_Strength
    {//tolua_export

        //tolua_begin
        public int highLevel;
        public int currentLevel;
        //tulua_end
        public Lua_Item_Strength(int high,int current)
        {
            highLevel = high;
            currentLevel = current;
        }
    } 

    //升档消耗
    public struct Lua_Item_LevelUp
    {

        public int needItemID;
        public int needItemID2;
        public int needItemNum;
        public int needItem2Num;
        public int money;

        public Lua_Item_LevelUp(int id1,int id2,int num1,int num2,int Money)
        {
            needItemID = id1;
            needItemID2 = id2;
			needItemNum = num1;
			needItem2Num =num2;
			money = Money;			
        }
    }
}