using System;
using System.Collections.Generic;

using UnityEngine;
using Network.Packets;

namespace Interface
{

    public class GameInterface
    {
        private static readonly GameInterface instance = new GameInterface();
        
        private uint mAutoHitState = 0;
        public uint AutoHitState
        {
            get { return mAutoHitState; }
            set { mAutoHitState = value; }
        }
        // 禁止在外面创建实例 [1/30/2012 Ivan]
        private GameInterface()
        {
        }



        public static GameInterface Instance
        {
            get { return instance; }
        }

        public void Initial()
        {
            CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_EQUIP_MOUNT, handlerUseMountEvent);
        }

        public void handlerUseMountEvent(GAME_EVENT_ID eventId, List<string> vParam)
        {
            if(eventId == GAME_EVENT_ID.GE_EQUIP_MOUNT)
            {
                //LogManager.LogWarning("Handle Use Mount Event");
                //使用召唤坐骑技能
                CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(2101);
            }
        }

        //------------------------------------------------
        // 玩家
        // 为移动函数添加一个标志，用于表示自己点击移动还是系统寻路 [9/6/2011 Sun]
        //移动到场景中某个位置
        public void Player_MoveTo(Vector3 targeMove)
        {
             Player_MoveTo(targeMove, false);
        }

        public void Player_MoveTo_Auto(Vector3 targeMove)
        {
            Player_MoveTo(targeMove, true);
        }

        void Player_MoveTo(Vector3 fvAimPos, bool bAutoMove)
        {
            // 点击贴花
            GameProcedure.s_ProcMain.SetMouseTargetProjTex(fvAimPos.x, fvAimPos.z);
            //移动指令
            CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
            if (pMySelfAI != null)
            {
                pMySelfAI.PushCommand_MoveTo(fvAimPos.x, fvAimPos.z, bAutoMove);
            }

            // 当玩家主动点击的时候取消自动寻路 [8/3/2011 ivan edit]
            StopAutoMove();
        }

        public void StartAutoHit()
        {
            CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
            pMySelfAI.PushCommand_AutoHit(1);
        }

        public void StopAutoHit()
        {
            CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
            pMySelfAI.PushCommand_AutoHit(0);
        }

        public ENUM_RELATION GetCampType(CObject pObj_A, CObject pObj_B)
        {
            if (pObj_A == null || pObj_B == null)
            {
                return ENUM_RELATION.RELATION_ENEMY;
            }

            if (pObj_A == pObj_B)
            {
                return ENUM_RELATION.RELATION_FRIEND;
            }

            // 	    int idOwner_A, idOwner_B, idA, idB;
            // 	    idA			= pObj_A.ServerID;
            // 	    idB			= pObj_B.ServerID;
            // 	    idOwner_A	= pObj_A.ownerid;
            // 	    idOwner_B	= pObj_B->GetOwnerID();
            // 	    if ( idOwner_A != INVALID_ID || idOwner_B != INVALID_ID )
            // 	    {
            // 		    if ( idOwner_A == idOwner_B
            // 			    || idOwner_A == idB
            // 			    || idOwner_B == idA )
            // 		    {
            // 			    return RELATION_FRIEND;
            // 		    }
            // 	    }

            //const _CAMP_DATA *pCamp_A, *pCamp_B;
            //BOOL bHuman_A, bHuman_B;
            //const CampAndStandDataMgr_T *pCampMgr;

            //pCamp_A		= pObj_A->GetCampData();
            //pCamp_B		= pObj_B->GetCampData();
            //bHuman_A	= g_theKernel.IsKindOf( pObj_A->GetClass(), GETCLASS(CObject_PlayerMySelf));
            //bHuman_B	= g_theKernel.IsKindOf( pObj_B->GetClass(), GETCLASS(CObject_PlayerMySelf));
            //pCampMgr	= CGameProcedure::s_pCampDataMgr;

            _CAMP_DATA cap_a = pObj_A.GetCampData();
            _CAMP_DATA cap_b = pObj_B.GetCampData();

            return CampStand.Instance.CalcRelationType(cap_a.m_nCampID, cap_b.m_nCampID);
        }

        internal void Object_SelectAsMainTarget(int tarId, CObjectManager.DESTROY_MAIN_TARGET_TYPE tarType)
        {
            CObjectManager.Instance.SetMainTarget(tarId, tarType);
        }

        internal void Object_SelectAsMainTarget(int tarId)
        {
            CObjectManager.Instance.SetMainTarget(tarId, CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT);
        }

        internal void Player_UseSkill(CActionItem_Skill skillAction, int idObject)
        {
            //判断是否为合法的目标技能
            //CActionItem_Skill pAction = CActionSystem.Instance.GetAction_SkillID(idSkill);
            if (skillAction == null)
                return;

            //取得技能数据
            SCLIENT_SKILL pSkill = (SCLIENT_SKILL)skillAction.GetImpl();
            if (pSkill == null)
                return;

            //检测目标是否合法
            //	if(!(pSkill->IsValidTarget(idObj))) return;

            CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
            if (pMySelfAI != null)
            {
				
                //发送消息
                SCommand_AI cmdTemp = new SCommand_AI();
                cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
                cmdTemp.SetValue(0, pSkill.m_pDefine.m_nID);
                cmdTemp.SetValue(1, pSkill.m_nLevel);
                cmdTemp.SetValue(2, idObject);
                cmdTemp.SetValue<float>(3, -1);
                cmdTemp.SetValue<float>(4, -1);
                cmdTemp.SetValue<float>(5, -1);
                cmdTemp.SetValue<uint>(6, 0);
                pMySelfAI.PushCommand(cmdTemp);

                //发送事件
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, skillAction.GetDefineID());
            }
        }

        internal void Player_UseSkill(CActionItem_Skill skillAction, Vector3 pos)
        {
            //CActionItem pAction = (CActionItem)CActionSystem.Instance.GetAction_SkillID(skillDefineId);
            if (skillAction == null) return;

            CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();

            SCLIENT_SKILL pSkill = (SCLIENT_SKILL)skillAction.GetImpl();
            if (pSkill == null) return;

            //发送消息
            SCommand_AI cmdTemp = new SCommand_AI();
            cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
            cmdTemp.SetValue<int>(0, pSkill.m_pDefine.m_nID);
            cmdTemp.SetValue<byte>(1, pSkill.m_nLevel);
            cmdTemp.SetValue<int>(2, MacroDefine.INVALID_ID);
            cmdTemp.SetValue<float>(3, pos.x);
            cmdTemp.SetValue<float>(4, pos.z);
            cmdTemp.SetValue<float>(5, -1);
            cmdTemp.SetValue<uint>(6, MacroDefine.INVALID_GUID);
            pMySelfAI.PushCommand(cmdTemp);

            //发送事件
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, skillAction.GetDefineID());
        }

        internal void Player_UseSkill(CActionItem_Skill skillAction, float dir)
        {
            //判断是否为合法的范围技能
            //CActionItem pAction = (CActionItem)CActionSystem.Instance.GetAction_SkillID(idSkill);

            if (skillAction == null)
                return;

            //取得技能数据
            SCLIENT_SKILL pSkill = (SCLIENT_SKILL)skillAction.GetImpl();
            if (pSkill == null) return;


            //检测目标是否合法

            //	if(!(pSkill->IsValidTarget(idObj))) return;


            CAI_Base pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
            //发送消息
            SCommand_AI cmdTemp = new SCommand_AI();
            cmdTemp.m_wID = (int)AICommandDef.AIC_USE_SKILL;
            cmdTemp.SetValue<int>(0, pSkill.m_pDefine.m_nID);
            cmdTemp.SetValue<byte>(1, pSkill.m_nLevel);
            cmdTemp.SetValue<int>(2, MacroDefine.INVALID_ID);
            cmdTemp.SetValue<float>(3, -1.0f);
            cmdTemp.SetValue<float>(4, -1.0f);
            cmdTemp.SetValue<float>(5, dir);
            cmdTemp.SetValue<uint>(6, MacroDefine.INVALID_GUID);
            pMySelfAI.PushCommand(cmdTemp);

            //发送事件
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ON_SKILL_ACTIVE, skillAction.GetDefineID());
        }

        internal void Player_Speak(int npcId)
        {
            StopAutoHit();
            CAI_MySelf pMySelfAI = (CAI_MySelf)CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();

            SCommand_AI cmdSpeak = new SCommand_AI();
            cmdSpeak.m_wID = (int)AICommandDef.AIC_DEFAULT_EVENT;
            cmdSpeak.SetValue(0, npcId);
            pMySelfAI.PushCommand(cmdSpeak);
        }
        public void TripperObj_Active( int dwId )
        {
            StopAutoHit();
            CObjectManager.Instance.getPlayerMySelf().Ride = false;
	        CAI_Base pMySelfAI;
	        pMySelfAI		= CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
        	
	        SCommand_AI cmdTemp = new SCommand_AI();
            cmdTemp.m_wID = (int)AICommandDef.AIC_TRIPPER_ACTIVE;
	        cmdTemp.SetValue(0, dwId);
	        pMySelfAI.PushCommand( cmdTemp );
        }
        internal void PacketItem_UserEquip(CObject_Item item)
        {
            if (item == null)
                return;
            //摆摊时不可以随便换装
            //...TODO

            //验证是否可以使用
            if (!item.Rule(ITEM_RULE.RULE_USE))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, item.RuleFailMessage(ITEM_RULE.RULE_USE));
                return;
            }

            CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();
            if (pCharData == null)
            {
                LogManager.LogError("playerMyself's charData is null.");
                return;
            }
            int needLevel = item.GetNeedLevel();
            int playerLevel = pCharData.Get_Level();

            if (playerLevel < needLevel)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "等级不够");

                return;
            }
            //是否鉴定
            if (((CObject_Item_Equip)item).GetEquipQuantity() == EQUIP_QUALITY.BLUE_EQUIP &&
                ((CObject_Item_Equip)item).GetEquipAttrib() == EQUIP_ATTRIB.EQUIP_ATTRIB_UNIDENTIFY)
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "此物品需要鉴定");
                return;
            }

            //取得详细信息
            string szExtraInfo = item.GetExtraDesc();

            //如果没有详细信息，从服务器上请求详细信息
            if (szExtraInfo == "")
            {
                CDataPool.Instance.UserBag_AskExtraInfo(item.PosIndex);
            }

            CGUseEquip msg = new CGUseEquip();
            msg.BagIndex = (byte)item.PosIndex;
            NetManager.GetNetManager().SendPacket(msg);
        }

        internal void PacketItem_UserItem(CActionItem_Item pActionItem, int targetServerID, Vector2 fvPos)
        {
	        //空物品
	        if(pActionItem == null || pActionItem.GetType() != ACTION_OPTYPE.AOT_ITEM)
                return;
	        CObject_Item pItem = pActionItem.ItemImpl;
	        if(pItem == null) 
                return;
	        //必须是能够使用的物品
	        if(pItem.GetItemClass()!= ITEM_CLASS.ICLASS_COMITEM && pItem.GetItemClass()!= ITEM_CLASS.ICLASS_TASKITEM)
                return;

	        //特殊物品不能在背包中直接使用，例如，宠物技能书
// 	        STRING strTemp;
// 	        if(!CObject_Item::CheckUseInPackage(pItem, strTemp))
// 	        {
// 		        if(!strTemp.empty()) CGameProcedure::s_pEventSystem->PushEvent(GE_INFO_SELF, strTemp.c_str());
// 		        return;
// 	        }
	        //组队跟随中...
	        //if(CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_TeamFollowFlag()) return;

	        //检查目前选中的目标
	        CObject pObj = CObjectManager.Instance.FindServerObject(targetServerID);
        	
	        //检查物品是否能够直接使用
	        int objID = 0;
            PET_GUID_t petID = new PET_GUID_t();
            fVector2 pos;
            pos.x = fvPos.x;
            pos.y = fvPos.y;
	        bool bCanuseDir = ((CObject_Item_Medicine)pItem).IsValidTarget(pObj,ref pos,ref objID,ref petID);

	        if(bCanuseDir)
	        {
		        WORLD_POS posTarget;
                posTarget.m_fX = pos.x;
                posTarget.m_fZ = pos.y;

		        //能够直接使用
		        CGUseItem msg = new CGUseItem();
                msg.BagIndex = (byte)pItem.GetPosIndex();
                msg.Target = (uint)objID;
                msg.TargetPetGUID = petID;
                msg.PosTarget = posTarget;
                NetManager.GetNetManager().SendPacket(msg);
		        return;
	        }

	        //如果已经选中目标，说明目标不合适,如果是用在自己宠物上的物品，说明宠物没有释放
	        if(pObj!=null || ((CObject_Item_Medicine)pItem).IsTargetOne())
	        {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"无效目标");
		        return;
	        }

	        //需要选中目标,在鼠标上挂上物品
            CActionSystem.Instance.SetDefaultAction(pActionItem);
        }
        internal void PacketItem_UserSymbol(CActionItem_Item pActionItem)
        {
            if (pActionItem.ItemImpl != null)
            {
                CGUseSymbol useSymbolMsg = new CGUseSymbol();
                useSymbolMsg.BagIndex = (byte)pActionItem.ItemImpl.PosIndex;
                NetManager.GetNetManager().SendPacket(useSymbolMsg);
            }
        }
        //设置当前激活技能
        public virtual void Skill_SetActive(tActionItem pActiveSkill)
        { 
            //设置激活技能
	        GameProcedure.s_ProcMain.ActiveSkill = pActiveSkill;

	        //通知UI设置按钮的Check状态
	        CActionSystem.Instance.SetDefaultAction(pActiveSkill);
        }
		public tActionItem Skill_GetActive()
		{
			return GameProcedure.s_ProcMain != null ? GameProcedure.s_ProcMain.ActiveSkill : null;
		}

        internal void Player_MoveTo(int SceneId, Vector3 PosTarget)
        {
            int currentScene = GameProcedure.s_pWorldManager.GetActiveSceneID();
	        //同场景寻路
	        if (currentScene == SceneId)
	        {
		        //  自动寻路开始（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE,1);
		        Player_MoveTo(PosTarget);
	        }
	        //跨场景寻路
	        else
	        {
 		        List<SceneTransferData> oResult;
 		        if( SceneTransportPath.Instance.FindPath(SceneId,currentScene,out oResult) )
 		        {
                    m_SceneFindPathStatus.SetValue(SceneId, new Vector2(PosTarget.x, PosTarget.z), oResult);
 			        //移动到第一个传送点
 			        Vector2 vPos = m_SceneFindPathStatus.GetNextPosition();
 			        if (vPos.x > -1 && vPos.y > -1)
 			        {
 				        //  自动寻路开始（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE,1);
 				        Player_MoveToAndKeepAutoMove(vPos);
 			        }
 		        }
 		        else
 		        {
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_NEW_DEBUGMESSAGE,"当前场景无法达到目的场景！");
 		        }
	        }
        }

        //使用生活技能，配方合成
        internal void Player_UseLifeAbility(int idPrescription , int nMakeCount, uint uFlag)
        {
            // 暂不实现自动重复制作的功能
	        if(nMakeCount < 1) return;

	        //判断是否满足使用技能之条件
	        CAI_Base  pMySelfAI = CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();

	        //发送消息
	        SCommand_AI cmdTemp = new SCommand_AI();
	        cmdTemp.m_wID			= (int)AICommandDef.AIC_COMPOSE_ITEM;
	        cmdTemp.SetValue<int>(0, idPrescription);
	        cmdTemp.SetValue<uint>(1, uFlag);
	        pMySelfAI.PushCommand( cmdTemp );
        }

        //-----------------------------------------------------------
// 从货架上头购买一个物品
        internal void Booth_BuyItem(CObject_Item pItem)
        {
            CGShopBuy msg = new CGShopBuy();
            // 先判断是否够钱买

            msg.UniqueID = CDataPool.Instance.Booth_GetShopUniqueId();
            msg.Index = (byte)pItem.PosIndex;

            NetManager.GetNetManager().SendPacket(msg);
        }
        // 卖出一个物品
        internal void Booth_SellItem(CObject_Item pItem)
        {
            //验证是否可以卖出
            if (!pItem.Rule(ITEM_RULE.RULE_SALE))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE, pItem.RuleFailMessage(ITEM_RULE.RULE_SALE));
                return;
            }

            //判断当前的NPC是不是收购这类物品
            if (!CDataPool.Instance.Booth_IsCanBuy(pItem))
            {
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_INTERCOURSE, "#{cannot sell}");
                return;
            }

            CGShopSell msg = new CGShopSell();
            // 先判断是否够钱买，
            msg.UniqueID = CDataPool.Instance.Booth_GetShopUniqueId();
            msg.Index = (byte)pItem.PosIndex;
            NetManager.GetNetManager().SendPacket( msg );
        }


        private void Player_MoveToAndKeepAutoMove(Vector2 vPos)
        {
	        // 点击特效 [10/11/2010 Sun]
            //GameProcedure.s_ProcMain.SetMouseClickedEffect(vPos);	

	        //移动指令
	        CAI_Base pMySelfAI	=CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();
            pMySelfAI.PushCommand_MoveTo(vPos.x, vPos.y, true);
        }

        SceneFindPathStatus m_SceneFindPathStatus = new SceneFindPathStatus();
        public void StopAutoMove()
        {
	        // 玩家主动点击的时候停止寻路 [8/3/2011 ivan edit]
	        if (m_SceneFindPathStatus.IsUse())
	        {
		        m_SceneFindPathStatus.Clear();
	        }
        }

        public void CheckAutoMove( )
        {
	        if( m_SceneFindPathStatus.IsUse() )
	        {
		        //移动到下一个传送点
                Vector2 vPos = m_SceneFindPathStatus.GetNextPosition();
                // 如果是最后一个节点，且有需要进行的操作，则直接执行操作，否则执行move [3/14/2012 Ivan]
                if (!m_SceneFindPathStatus.IsUse() && m_SceneFindPathStatus.Hyper != null)
                    m_SceneFindPathStatus.Hyper.Click();
                else if (vPos.x != -1 && vPos.y != -1)
                    Player_MoveToAndKeepAutoMove(vPos);
	        }
            autoMoveNavReady = autoMoveplayerReady = false;
        }

        // 由于跨场景寻路的时候需要使用玩家的数据和导航数据，而导航数据是异步加载的，需要做一下同步 [3/14/2012 Ivan]
        bool autoMoveNavReady = false;
        public void AutoMoveNavReady()
        {
            autoMoveNavReady = true;
            if (autoMoveplayerReady)
            {
                CheckAutoMove();
            }
        }

        bool autoMoveplayerReady = false;
        public void AutoMovePlayerReady()
        {
            autoMoveplayerReady = true;
            if (autoMoveNavReady)
            {
                CheckAutoMove();
            }
        }

        internal void Player_AddCMDAfterMove(CMD_AFTERMOVE_TYPE type, int id, Vector2 PosTarget)
        {
	        CAI_Base pMySelfAI	=CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI();

	        SCommand_AI cmdAfterMove = new SCommand_AI();
            cmdAfterMove.m_wID = (int)AICommandDef.AIC_CMD_AFTERMOVE;
            cmdAfterMove.SetValue(0, type);
            cmdAfterMove.SetValue(1, id);
            // 保存目的地信息 [6/1/2011 ivan edit]
            cmdAfterMove.SetValue(2, PosTarget);
	        pMySelfAI.PushCommand(cmdAfterMove);
        }

        /// <summary>
        /// 跨场景移动后执行操作
        /// </summary>
        /// <param name="SceneId"></param>
        /// <param name="PosTarget"></param>
        /// <param name="hyperNpc"></param>
        internal void Player_DoingAfterMoveScene(int SceneId, Vector3 PosTarget, HyperItemBase hyper)
        {
            Player_MoveTo(SceneId, PosTarget);
            // 如果跨场景，保存移动后的操作 [3/14/2012 Ivan]
            if (m_SceneFindPathStatus.IsUse() && hyper != null)
                m_SceneFindPathStatus.Hyper = hyper;
        }

        public void ItemBox_PickItem( _ITEM_GUID guid, uint uBoxId )
        {
            CGPickBoxItem msg = new CGPickBoxItem();
            msg.ObjectId = uBoxId;
            msg.ItemGUID = guid;

            NetManager.GetNetManager().SendPacket(msg);
        }
        public void AddFriendByName(string name, RELATION_GROUP nGroup)
        {
            if (string.IsNullOrEmpty(name))
                return;
            int group =-1;
            int nIndex = -1;
            if(CDataPool.Instance.GetRelation().GetRelationByName(name, ref group, ref nIndex)
                != RELATION_TYPE.RELATION_TYPE_STRANGER)
                return; 

            CObject_Character pTargetObj = CObjectManager.Instance.FindCharacterByName(name);
            if (pTargetObj == CObjectManager.Instance.getPlayerMySelf())
                return;
            if (pTargetObj is CObject_PlayerOther)
            {
                ENUM_RELATION sCamp = Interface.GameInterface.Instance.GetCampType(pTargetObj, CObjectManager.Instance.getPlayerMySelf());
                if (sCamp != ENUM_RELATION.RELATION_FRIEND)
                {// 如果不是同一阵营的
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "不能添加不同阵营的人到好友");
                    return;
                }
            }
            CGRelation Msg = new CGRelation();	
	        Msg.GetRelation().CleanUp();
	        Msg.GetRelation().m_Type = (byte)RELATION_REQUEST_TYPE.REQ_ADDFRIEND;
	        REQUEST_ADD_RELATION_WITH_GROUP pFriend = new REQUEST_ADD_RELATION_WITH_GROUP();
            Msg.GetRelation().mRelation = pFriend;
	        pFriend.CleanUp();
	        pFriend.SetTargetName( EncodeUtility.Instance.GetGbBytes(name) );
	        pFriend.SetGroup( (byte)nGroup );
	        pFriend.SetRelationType( (byte)RELATION_TYPE.RELATION_TYPE_FRIEND );

	        NetManager.GetNetManager().SendPacket( Msg );
        }

        //加入好友功能（包括好友、临时好友、黑名单）
        public void addFriend(RELATION_GROUP nGroup, string name)
        {
        	string strva = name;
            CGRelation Msg = new CGRelation() ;
            CObject_Character pCharObj = null;
            Msg.GetRelation().m_Type =(byte) RELATION_REQUEST_TYPE.REQ_ADDFRIEND;
            REQUEST_ADD_RELATION_WITH_GROUP pFriend = new REQUEST_ADD_RELATION_WITH_GROUP();
            Msg.GetRelation().mRelation = pFriend;
            pFriend.CleanUp();

            bool valueIsNum =false; /*Ogre::StringConverter::isNumber(strva.c_str());*/
            if( nGroup == RELATION_GROUP.RELATION_GROUP_FRIEND_ALL ) // 如果直接一个名字，就自动往所有的列表里加，
            {
	            nGroup = RELATION_GROUP.RELATION_GROUP_F1;
            }
            else if( nGroup == RELATION_GROUP.RELATION_GROUP_TEMPFRIEND )//临时好友
            {
	            SDATA_RELATION_MEMBER pMember = new SDATA_RELATION_MEMBER();
        		
	            if( !valueIsNum )
	            {
		            pCharObj = (CObject_Character)CObjectManager.Instance.FindCharacterByName(strva);
		            if( pCharObj == null ) return ;
                    pMember.m_szName = strva;
	            }
	            else
	            {
		            pCharObj = (CObject_Character)CObjectManager.Instance.GetMainTarget();
		            if( pCharObj == null ) return ;
                    pMember.m_szName = pCharObj.GetCharacterData().Get_Name();
	            }
	            // 如果是玩家并且是统一阵营的才会添加
	            ENUM_RELATION sCamp = Interface.GameInterface.Instance.GetCampType( pCharObj, CObjectManager.Instance.getPlayerMySelf());
	            if( (pCharObj is CObject_PlayerOther ) == false ) 
		            return ;
	            if( sCamp != ENUM_RELATION.RELATION_FRIEND ) return;

	            int nTmpGroup = -1, nIndex = -1;
	            CDataPool.Instance.GetRelation().GetRelationByName( pMember.m_szName, ref nTmpGroup , ref nIndex );
	            if( nTmpGroup >= 0 ) return ;

	            pMember.m_RelationType = RELATION_TYPE.RELATION_TYPE_TEMPFRIEND;
	            if( CDataPool.Instance.GetRelation().AddRelation( RELATION_GROUP.RELATION_GROUP_TEMPFRIEND, pMember ) )
	            {
		            string szText = "你将"+pMember.m_szName+  " 添加为临时好友";
		            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, szText );
	            }
	            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_FRIEND);
	            return ;
            }
            else if( nGroup== RELATION_GROUP.RELATION_GROUP_BLACK )
            { // 增加黑名单的添加
	            CGRelation Msg1 = new CGRelation();
	            Msg1.GetRelation().m_Type = (byte)RELATION_REQUEST_TYPE.REQ_ADDTOBLACKLIST;
	            REQUEST_ADD_RELATION pBlackName =  new REQUEST_ADD_RELATION();
                Msg1.GetRelation().mRelation = pBlackName;
	            pBlackName.CleanUp();

	            if(strva == "")
	            {
		            pCharObj = (CObject_Character)CObjectManager.Instance.GetMainTarget();
		            if( pCharObj == null ) return ;
                    pBlackName.SetTargetName(EncodeUtility.Instance.GetGbBytes(pCharObj.GetCharacterData().Get_Name()));
	            }
	            else if( valueIsNum ) 
	            {
// 			            pCharObj = (CObject_Character)CObjectManager.Instance.FindCharacterByRaceID(int.Parse(strva));
// 			            if( pCharObj == null ) pCharObj = (CObject_Character*)CObjectManager::GetMe()->FindCharacterByName(strva.c_str());
// 			            if( pCharObj == null ) return ;
// 			            pBlackName->SetTargetGUID( Ogre::StringConverter::parseInt(strva.c_str()) );
	            }
	            else
	            {
		            pCharObj = (CObject_Character)CObjectManager.Instance.FindCharacterByName(strva);
		            if( pCharObj == null ) return ;
		            pBlackName.SetTargetName( EncodeUtility.Instance.GetGbBytes(strva) );
	            }

	            if( pCharObj == CObjectManager.Instance.getPlayerMySelf() ) return ;

	            if(  pCharObj is CObject_PlayerOther  ) // 如果是玩家
	            {
		            ENUM_RELATION sCamp = Interface.GameInterface.Instance.GetCampType( pCharObj, CObjectManager.Instance.getPlayerMySelf() );
		            if( sCamp != ENUM_RELATION.RELATION_FRIEND ) {
			            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, "不能添加不同阵营的人到黑名单" );
			            return ;
		            }
	            }
	            else
	            {
		            GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, "不能添加非玩家到黑名单" );
		            return ;
	            }

	            pBlackName.SetRelationType((byte)RELATION_TYPE.RELATION_TYPE_BLACKNAME);
	            NetManager.GetNetManager().SendPacket( Msg1 );
	            return ;
            }

	        if( strva == "")
	        {
		        pCharObj = (CObject_Character)CObjectManager.Instance.GetMainTarget();
		        if( pCharObj == null ) return ;
		        pFriend.SetTargetName( EncodeUtility.Instance.GetGbBytes(pCharObj.GetCharacterData().Get_Name()) );
	        }
	        else if( valueIsNum ) 
	        {
// 		        pCharObj = (CObject_Character)CObjectManager::GetMe()->FindCharacterByRaceID(Ogre::StringConverter::parseInt(strva.c_str()));
// 		        if( pCharObj == NULL ) pCharObj = (CObject_Character*)CObjectManager::GetMe()->FindCharacterByName(strva.c_str());
// 		        if( pCharObj == NULL ) return 0;
// 		        pFriend->SetTargetGUID( Ogre::StringConverter::parseInt(strva.c_str()) );
	        }
	        else
	        {
		        pFriend.SetTargetName( EncodeUtility.Instance.GetGbBytes(strva) );
		        pCharObj = (CObject_Character)CObjectManager.Instance.FindCharacterByName(strva);
		        if( pCharObj == null ) return ;
	        }
	        if( pCharObj == CObjectManager.Instance.getPlayerMySelf() ) // 如果是自己，就不加
		        return ;

	        if( pCharObj is CObject_PlayerOther  ) // 如果是玩家
	        {

                ENUM_RELATION sCamp = Interface.GameInterface.Instance.GetCampType( pCharObj, CObjectManager.Instance.getPlayerMySelf() );
 		         if( sCamp != ENUM_RELATION.RELATION_FRIEND )  {// 如果不是同一阵营的
 			        GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, "不能添加不同阵营的人到好友" );
 			        return ;
 		        }
// 		        // 改为势力判断 [9/26/2011 Ivan edit]
// 		        if (pCharObj.GetCharacterData().GetShiLi() !=
// 			        CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->GetShiLi())
// 		        {
// 			        CGameProcedure::s_pEventSystem->PushEvent( GE_INFO_SELF, "不能添加不同势力的人到好友" );
// 			        return 0;
// 		        }
	        }
	        else
	        {
		        GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, "不能添加非玩家到好友" );
		        return ;
	        }

	        pFriend.SetGroup( (byte)nGroup );
	        pFriend.SetRelationType( (byte) RELATION_TYPE.RELATION_TYPE_FRIEND );

	        NetManager.GetNetManager().SendPacket( Msg );

	        return ;
        }
        public void delFriend(RELATION_GROUP group, int index) //删除一个好友
        {
            SDATA_RELATION_MEMBER pInfo = CDataPool.Instance.GetRelation().GetRelationInfo(  group , index );

	        if( group == RELATION_GROUP.RELATION_GROUP_TEMPFRIEND ) // 如果是临时好友
	        {
		        CDataPool.Instance.GetRelation().RemoveRelation(  group , index );
		        GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_UPDATE_FRIEND );
		        return ;
	        }
	        else if( group == RELATION_GROUP.RELATION_GROUP_BLACK )
	        {
		        CGRelation Msg = new CGRelation();

		        Msg.GetRelation().CleanUp();
		        Msg.GetRelation().m_Type = (byte)RELATION_REQUEST_TYPE.REQ_DELFROMBLACKLIST;

		        RELATION_GUID pBlackName = new RELATION_GUID();
                Msg.GetRelation().mRelation = pBlackName;
		        pBlackName.CleanUp();
		        pBlackName.SetTargetGUID( pInfo.m_GUID );
		        NetManager.GetNetManager().SendPacket( Msg );
	        }
	        else
	        {
		        CGRelation Msg = new CGRelation();
		        Msg.GetRelation().CleanUp();
		        Msg.GetRelation().m_Type = (byte)RELATION_REQUEST_TYPE.REQ_DELFRIEND;
		        REQUEST_DEL_FRIEND pFriend = new REQUEST_DEL_FRIEND();
                Msg.GetRelation().mRelation = pFriend;
		        pFriend.CleanUp();
		        pFriend.SetTargetGUID( pInfo.m_GUID );
                NetManager.GetNetManager().SendPacket(Msg);
	        }

	        return ;
        }
    }

    public class Lua_EVENT_LIST
    {//tolua_export

        //tolua_begin
        public string name;
        public int state;
        public int scriptId;
        public int index;
        public string itemStr;
        //tolua_end

    };//tolua_export


}
