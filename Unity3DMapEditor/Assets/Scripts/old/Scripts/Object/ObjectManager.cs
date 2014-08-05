using System.Collections;
using System.Collections.Generic;
using Network;
using Network.Packets;
using UnityEngine;
public class CObjectManager
{
     public enum DESTROY_MAIN_TARGET_TYPE
	 {
		DEL_OBJECT = 0,		// 销毁对象
		DEL_RIGHT_CLICK,	// 鼠标右键销毁
	 }; 

     Dictionary<int, CObject> mObjects = new Dictionary<int, CObject>(); // 所有Object
     Dictionary<int, CObject> mServerObjects = new Dictionary<int, CObject>(); //所有服务器object
     public Dictionary<int, CObject> ServerObjects
     {
         get { return mServerObjects; }
     }
     Dictionary<GameObject, CObject> mPhyObjectsMap = new Dictionary<GameObject, CObject>();//GO和CObject之间的映射，用于查找和射线相交的对象  [1/4/2012 ZZY]
     CObject mRootObject;
     CObject mLogicalObject ;
     CObject_PlayerMySelf mPlayerMySelf;
     CObject mPlayerOther;
     CObject mPlayerNPC;
     CObject mMainTarget;
     CObject_Character mNearestTargetOLD;
     bool mNeedWaitSkillSend = false;
     uint mWaitSkillSend     = 0;
    // 更改为单例模式
    static readonly CObjectManager sInstance = new CObjectManager();

    static public   CObjectManager Instance
    {
        get
        {
            return sInstance;
        }
    }

    public CObjectManager()
    {
        mRootObject = new CObject();
        mLogicalObject =new  CObject();//逻辑根对象
        mRootObject.AddChild(mLogicalObject);
        mPlayerOther = new CObject();//玩家根对象
        mLogicalObject.AddChild(mPlayerOther);
        mPlayerNPC = new CObject();//npc根对象
        mLogicalObject.AddChild(mPlayerNPC);

        m_pLoadQueue = new CObjectLoadQueue();
    }

    public   CObject_PlayerMySelf getPlayerMySelf()
    {
        return mPlayerMySelf;
    }
    //创建玩家自己
    public   CObject_PlayerMySelf NewPlayerMySelf(int idServer)
    {
        if (mServerObjects.ContainsKey(idServer))
        {
            return (CObject_PlayerMySelf)mServerObjects[idServer];
        }
        CObject_PlayerMySelf playerMySelf = new CObject_PlayerMySelf();
        add(playerMySelf, idServer);
        //mLogicalObject.AddChild(playerMySelf);
        mPlayerMySelf = playerMySelf;

        return playerMySelf;
    }
    //创建其他玩家
    public   CObject_PlayerOther NewPlayerOther(int idServer)
    {
        if (mServerObjects.ContainsKey(idServer))
        {
            return (CObject_PlayerOther)mServerObjects[idServer];
        }
        CObject_PlayerOther playerOther = new CObject_PlayerOther();
        add(playerOther, idServer);
        mPlayerOther.AddChild(playerOther);
        return playerOther;
    }
    //创建怪物NPC
    public   CObject_PlayerNPC NewPlayerNPC(int idServer)
    {
        if (mServerObjects.ContainsKey(idServer))
        {
            return (CObject_PlayerNPC)mServerObjects[idServer];
        }
        CObject_PlayerNPC npc = new CObject_PlayerNPC();
        add(npc, idServer);
        mPlayerNPC.AddChild(npc);
        return npc;
    }
    //创建鼠标目标贴花对象
    public CObject_ProjTex_MouseTarget NewProjTexMouseTarget(int idServer)
    {
        CObject_ProjTex_MouseTarget mouseTarget = new CObject_ProjTex_MouseTarget();
        add(mouseTarget, idServer);
        mLogicalObject.AddChild(mouseTarget);
        return mouseTarget;
    }
    //创建鼠标特效对象
    public CObject_Effect_MouseTarget NewEffectMouseTarget(int idServer)
    {
        CObject_Effect_MouseTarget mouseEffect = new CObject_Effect_MouseTarget();
        add(mouseEffect, idServer);
        mLogicalObject.AddChild(mouseEffect);
        return mouseEffect;
    }

    public CObject_ProjTex_AuraDure NewProjTexAuraDure(int idServer)
    {
        CObject_ProjTex_AuraDure skillAura = new CObject_ProjTex_AuraDure();
        add(skillAura, idServer);
        mLogicalObject.AddChild(skillAura);
        return skillAura;
    }

    public CObject_Effect  NewEffect(int idServer)
    {
        CObject_Effect newEffect = new CObject_Effect();
        add(newEffect, idServer);
        mLogicalObject.AddChild(newEffect);
        return newEffect;
    }

    public CObject_Bullet NewBullet(int idServer)
    {
        CObject_Bullet newBullet = new CObject_Bullet();
        add(newBullet, idServer);
        mLogicalObject.AddChild(newBullet);
        return newBullet;
    }

    public CObject_Special NewSpecialObject(int idServer)
    {
        if (idServer != -1)
        {
            CObject_Special pFind = (CObject_Special)FindServerObject(idServer);
            //如果已经有
            if (pFind != null)
            {
                return pFind;
            }
        }
        CObject_Special newSpecial = new CObject_Special();
        add(newSpecial, idServer);
        mLogicalObject.AddChild(newSpecial);
        return newSpecial;
    }

    public CTripperObject_ItemBox NewTripperItemBox(int idServer)
    {
        CTripperObject_ItemBox newItemBox = new CTripperObject_ItemBox();
        add(newItemBox, idServer);
        mLogicalObject.AddChild(newItemBox);
        return newItemBox;
    }
    public CTripperObject_Resource NewTripperResource(int idServer)
    {
        CTripperObject_Resource newTripperResource = new CTripperObject_Resource();
        add(newTripperResource, idServer);
        mPlayerNPC.AddChild(newTripperResource);
        return newTripperResource;
    }

    public CObject_Bus NewBus(int idServer)
    {
        CObject_Bus bus = new CObject_Bus();
        add(bus, idServer);
        mPlayerNPC.AddChild(bus);
        return bus;
    }

    public CObject_PlayerOther NewFakePlayerOther()
    {
        CObject_PlayerOther newFakePlayerOther = new CObject_PlayerOther();
        newFakePlayerOther.ServerID = -1;
        newFakePlayerOther.SetFakeObjectFlag(true);
        newFakePlayerOther.Initial(null);
        newFakePlayerOther.SetPosition(new Vector3(0, 0, 0));
        return newFakePlayerOther;
    }
    public CObject_PlayerNPC NewFakePlayerNPC()
    {
        CObject_PlayerNPC newFakePlayerNPC = new CObject_PlayerNPC();
        newFakePlayerNPC.ServerID = -1;
        newFakePlayerNPC.SetFakeObjectFlag(true);
        newFakePlayerNPC.Initial(null);
        newFakePlayerNPC.SetPosition(new Vector3(0, 0, 0));
        return newFakePlayerNPC;
    }
    protected   void add(CObject obj,int idServer)
    {
        obj.ID = IDFactory();
        obj.ServerID = idServer;
        mObjects[obj.ID] = obj;
        if (obj.ServerID != -1)
        {
            mServerObjects[obj.ServerID] = obj;
        }

    }
    public   void DestroyObject(CObject obj)
    {
        if (obj == null)
        {
            return;
        }
			//如果是主目标,取消选择
		if(obj == mMainTarget)
		{
			mMainTarget = null;
		}
		if(obj == mNearestTargetOLD)
		{
			mNearestTargetOLD = null;
		}
        mObjects.Remove(obj.ID);
        mServerObjects.Remove(obj.ServerID);

	 //如果被ui关心，发出事件并清除
		int id = obj.ID;
        Dictionary<string, OBJECT_BECARED>.Enumerator enumerator = mapCaredObjects.GetEnumerator();
        while (enumerator.MoveNext())
        {
            OBJECT_BECARED careItem = enumerator.Current.Value;
			if(careItem.id == id)
			{
				//产生事件
				List< string > vParam = new List<string>();
	            vParam.Add(id.ToString());
				vParam.Add("destroy");

				CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OBJECT_CARED_EVENT, vParam);

                mapCaredObjects.Remove(enumerator.Current.Key);
				break;
			}
		}

	    //删除队列中其他同ID的物体
	    enumerator = mapCaredObjects.GetEnumerator();
	    while(enumerator.MoveNext())
	    {
            OBJECT_BECARED careItem = enumerator.Current.Value;
            if (careItem.id == id)
		    {
                mapCaredObjects.Remove(enumerator.Current.Key);
                enumerator = mapCaredObjects.GetEnumerator();
		    }
	    }

	

        if (obj.GetParent() != null)
        {
            obj.GetParent().EraseChild(obj);
        }
        obj.Release();
       
    }
    
    public   CObject FindObject(int id)
    {
        if (mObjects.ContainsKey(id))
        {
            return mObjects[id];
        }
        else return null;
    }
    public   CObject FindServerObject(int idServer)
    {
        if (mServerObjects.ContainsKey(idServer))
        {
            return mServerObjects[idServer];
        }
        else return null;
    }
    static int nLastUID = 100;
    int IDFactory()
    {
	    return ++nLastUID; 
    }
      
    public void insertPhyObject(GameObject go, CObject obj)
    {
        mPhyObjectsMap[go] = obj;
    }
    public void removePhyObject(GameObject go)
    {
        mPhyObjectsMap.Remove(go);
    }

    int s_nLastHitObj = -1;//上次选中的对象ID
    public CObject GetMouseOverObject(Vector3 pt, out Vector3 fvMouseHitPlan)//pt： screen Position in pixel
    {
        fvMouseHitPlan = Vector3.zero;
		CObject hitObj = null;
        Ray ray = Camera.main.ScreenPointToRay (pt);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo);
        if (hit)
        {
            GameObject hitGo = hitInfo.collider.gameObject;
            //根据layer判断是否相交于动态对象还是静态的物体
            if (hitGo.layer == LayerManager.ActorLayer && hitGo != null && mPhyObjectsMap.ContainsKey(hitGo))
            {
                 hitObj = mPhyObjectsMap[hitGo];
				 fvMouseHitPlan = hitObj.GetPosition();
            }
            else
            {
                fvMouseHitPlan = hitInfo.point;
            }
        }
        int nHitObjID = hitObj != null ? hitObj.ID : -1;
        if (s_nLastHitObj != nHitObjID)
        {
            if (hitObj != null && hitObj.GetRenderInterface() != null) hitObj.GetRenderInterface().SetMouseHover(true);
            CObject pLastHitObj = FindObject(s_nLastHitObj);

            if (pLastHitObj != null && pLastHitObj.GetRenderInterface() != null) pLastHitObj.GetRenderInterface().SetMouseHover(false);
            s_nLastHitObj = nHitObjID;
        }
        return hitObj;
    }

    public void Tick()
    {
        //执行逻辑函数
        if (mPlayerMySelf != null)
            mPlayerMySelf.Tick();
	    mLogicalObject.Tick();
        
        // 刷新关心NPC的距离
        TickCareLogic();

        //加载队列工作
	    if(m_pLoadQueue != null)
	    {
		    m_pLoadQueue.Tick();
	    }
    }

    //检查被UI关心的逻辑对象
    private void TickCareLogic()
    {
        Dictionary<string, OBJECT_BECARED>.Enumerator enumerator = mapCaredObjects.GetEnumerator();
        while (enumerator.MoveNext())
        {
            OBJECT_BECARED careItem = enumerator.Current.Value;

            CObject npc = FindObject(careItem.id);
            if (npc == null)
            {
                LogManager.LogError("Cannot found CareObject");
                continue;
            }

            float newDistance = Utility.TDU_GetDist(getPlayerMySelf().GetPosition(), npc.GetPosition());
            float fStep = Mathf.Abs(newDistance - careItem.fLastDistance);

            if (fStep > 0.01)
            {
                List<string> sParams = new List<string>();
                sParams.Add(npc.ID.ToString());
                sParams.Add(newDistance.ToString());
                
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OBJECT_CARED_EVENT, sParams);
            }
            careItem.fLastDistance = newDistance;
        }
    }

    public void SetObjectServerID(int objID, int serverID)
    {
        if (objID != MacroDefine.INVALID_ID)
        {
            CObject pObject = (CObject)(FindObject(objID));
            if (pObject != null)
            {
                int idOldServer = pObject.ServerID;
                if (idOldServer != MacroDefine.INVALID_ID)
                    mServerObjects.Remove(idOldServer);

                if (serverID != MacroDefine.INVALID_ID)
                    mServerObjects[serverID] = pObject;
                pObject.ServerID = serverID;
            }
        }
    }

    public void DisableMainTarget()
    {
        SetMainTarget(MacroDefine.INVALID_ID, CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT);
    }

    public virtual void SetMainTarget(int id, DESTROY_MAIN_TARGET_TYPE delType)
    {
        CObject pFindObj = null;
	
	    if (id != MacroDefine.INVALID_ID)
	    {
		    pFindObj = (CObject)FindServerObject(id);
	    }

    	
	    //不考虑无法选择的物体
	    //如果没选中物体或者选种的物体是不CanbeSelect的，告诉服务器取消当前选种对象。
	    if(pFindObj == null || 
           ( pFindObj != null&& !(pFindObj.CanbeSelect()) ) ) 
	    {
    	
		    if(mMainTarget != null&&mMainTarget.GetRenderInterface() != null)
		    {
			    //关闭选择环
                mMainTarget.GetRenderInterface().Attach_ProjTexture(GFX.GfxObject.PROJTEX_TYPE.SELECT_RING, false, 1.0f, 1.7f, null);
			    mMainTarget	= null;
    			
			    if((delType != DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT)||
                    (pFindObj != null && !(pFindObj.CanbeSelect())))
			    {
				    // 如果是鼠标右键销毁， target 窗口.
				    // 2006-4-12 如果当前选择的不是队友返回。
    				
				    // 设置当前没有选中队友
				   // SetCurSelTeam(-1);
					AutoReleaseSkill.Instance.SetTargetObject(-1);
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MAINTARGET_CHANGED,-1);   				
			    }
			    else
			    {
				    //if(!IsSelTeam())
				   // {
					    // 2006-4-12 如果当前选择的不是队友返回。
                     //   CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MAINTARGET_CHANGED,-1);   	
				  //  }
			    }

		    }
		    else
		    {
			    // 设置当前没有选中队友
			   // SetCurSelTeam(-1);
				AutoReleaseSkill.Instance.SetTargetObject(-1);
			    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MAINTARGET_CHANGED,-1);   	
		    }
		    return;
	    }

	    if( pFindObj != null && (pFindObj != mMainTarget) )
	    {
		    //关闭选择环
		    if(mMainTarget != null && mMainTarget.GetRenderInterface() != null)
		    {
                mMainTarget.GetRenderInterface().Attach_ProjTexture(GFX.GfxObject.PROJTEX_TYPE.SELECT_RING, false, 1.0f, 1.7f, null);
		    }
		    //设置选择对象
		    mMainTarget = pFindObj;
		    //告诉server选中的对象

		    //发往服务器
		    CGLockTarget msg = new CGLockTarget();
            msg.TargetID = id;
		    NetManager.GetNetManager().SendPacket(msg);

		    //打开新的选择环
		    if(mMainTarget != null && 
                mMainTarget.GetRenderInterface() != null)
		    {
                CObject_PlayerNPC  pFindNPC = (CObject_PlayerNPC)pFindObj;
                float Select_Ring_Range = pFindNPC.GetProjtexRange();

                //ENUM_RELATION eCampTypeMetoIt = ENUM_RELATION.RELATION_INVALID;
                //if(g_theKernel.IsKindOf(m_pMainTarget->GetClass(), GETCLASS(CObject_Character)))
                //{
                //    eCampTypeMetoIt = CalcRelationType( 
                //        CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_CampData()->m_nCampID, 
                //        pFindNPC->GetCampData()->m_nCampID, 
                //        CGameProcedure::s_pCampDataMgr );

                //    // 如果选中的是玩家则通过PK模式判断关系 [8/19/2011 edit by ZL]
                //    if (eCampTypeMetoIt != RELATION_ENEMY)
                //    {
                //        INT tempRelation = CObjectManager::GetMe()->GetMySelf()->GetRelationOther((CObject_Character*)pFindObj);
                //        if ( tempRelation != -1 ) 
                //            eCampTypeMetoIt = (ENUM_RELATION)tempRelation;
                //    }
                //}
    			
                mMainTarget.GetRenderInterface().Attach_ProjTexture(GFX.GfxObject.PROJTEX_TYPE.SELECT_RING ,true, Select_Ring_Range, 1.7f, null );
		    }

		    //产生一个选择对象更改的事件
            UIWindowMng.Instance.ShowWindow("TargetWindow");
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MAINTARGET_CHANGED,id);
			AutoReleaseSkill.Instance.SetTargetObject(id);


		    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		    //
		    // 设置选中队友2006－4－1
		    //

            //TeamMemberInfo* pInfo = CUIDataPool::GetMe()->GetTeamOrGroup()->GetMemberByServerId(m_pMainTarget->GetServerID());
            //if(pInfo)
            //{
            //    GUID_t GUID = pInfo->m_GUID;

            //    // 设置当前选中队友
            //    SetCurSelTeam(TRUE);
            //    SetCurSelTeam(GUID);
            //}


	    }
    }
	//取得当前选中物体
	public virtual CObject GetMainTarget()
    {
        return mMainTarget;
    }

    bool allObjectInfoBoardShow = true;
    public bool AllObjectInfoBoardShow
    {
        get { return allObjectInfoBoardShow; }
        set { allObjectInfoBoardShow = value; }
    }
    public bool IsAllObjectInfoBoardShow()
    {
        return AllObjectInfoBoardShow;
    }

    //资源加载队列
    CObjectLoadQueue m_pLoadQueue;
    public CObjectLoadQueue LoadQueue
    {
        get { return m_pLoadQueue; }
    }
    public void SetLoadNPCDirect(bool isDirect)
    {
        if (m_pLoadQueue != null)
        {
            m_pLoadQueue.SetLoadNPCDirect(isDirect);
        }
    }

    //被UI关心的逻辑对象
    struct OBJECT_BECARED
    {
        public int id;					    //物体ID
        public float fLastDistance;		//和主角之间的距离
    };
    //目前被关心的逻辑对象
    Dictionary<string, OBJECT_BECARED> mapCaredObjects = new Dictionary<string, OBJECT_BECARED>(); 

    public void CareObject(int objId,bool isCare,string sign)
    {
        CObject obj = FindObject(objId);
        if (obj == null)
            return;

        mapCaredObjects.Remove(sign);
        if (isCare)
        {
            OBJECT_BECARED careObj = new OBJECT_BECARED();
            careObj.id = objId;
            careObj.fLastDistance = Utility.TDU_GetDist(getPlayerMySelf().GetPosition(), obj.GetPosition());

            mapCaredObjects.Add(sign, careObj);
        }
    }

    public string ItemNameByTBIndex(int nItemIndex)
    {
        //等待实现
        return nItemIndex.ToString();
    }
    //清除所有CObject派生对象  [3/7/2012 ZZY]
    public void Clear()
    {
        List<CObject> delObject = new List<CObject>();
        foreach (int k in mServerObjects.Keys)
        {
            CObject obj = mServerObjects[k];
            if (obj.GetFakeObjectFlag() == false)
            {
                delObject.Add(mServerObjects[k]);
            }
        }
        foreach (CObject obj in delObject)
        {
            DestroyObject(obj);
        }
        DestroyObject(mPlayerMySelf);
        mPlayerMySelf = null;
    }

    public CObject_Character FindCharacterByName(string szName)
    {
	    CObject_Character pObject = null;

	    if( mServerObjects.Count > 0 )
	    {
		   foreach (int k in mServerObjects.Keys)
		   {
               if (mServerObjects[k] is CObject_Character)
               {
                   CObject_Character character  = (CObject_Character)mServerObjects[k];
                   if (character.GetCharacterData().Get_Name() == szName)
                   {
                       pObject = character;
                       break;
                   }
               }
		   }
        }
	    return pObject;
    }
    //pObject中心对象， excludePlayerOther是否排除玩家
    public   CObject GetNearestTargetByOrigin(CObject pObject, float fMaxDistance, bool excludePlayerOther )
    {
	    float fNearestDist = fMaxDistance;
	    CObject  pNearestTarget = null;
	    CObject  pOriginObject = (CObject) pObject;
	    Vector2 fvOrigin = new Vector2 (pOriginObject.GetPosition().x, pOriginObject.GetPosition().z);
	    foreach (int k in mServerObjects.Keys)
	    {
		    CObject pObj = mServerObjects[k];

		    //不再查找自身
		    if(pObj == pOriginObject) continue;

            if (CObjectManager.Instance.getPlayerMySelf().IsMyPet(pObj.ServerID))continue;
		    
            //非角色
		    if(! (pObj is CObject_Character) ) continue;
            if (excludePlayerOther)
            {
                if (pObj is CObject_PlayerOther) continue;
            }
		    //友好阵营
		    CObject_Character pChar = (CObject_Character)pObj;
		    if(ENUM_RELATION.RELATION_ENEMY != GameProcedure.s_pGameInterface.GetCampType(
			    getPlayerMySelf(), pChar))
		    {
			    continue;
		    }
            
		    //死亡
		    if(ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD == pChar.CharacterLogic_Get()) continue;


		    //距离太远
            Vector2 vec = fvOrigin- new  Vector2(pObj.GetPosition().x, pObj.GetPosition().z);
            float fDistance = vec.magnitude;

		    if(fDistance >= fNearestDist) continue;

		    pNearestTarget = pObj;
		    fNearestDist = fDistance;
	    }
	    return pNearestTarget;
    }

    internal CObject FindCharacterByTableID(int idTable)
    {
	    CObject_Character pObject = null;

	    if(mServerObjects.Count != 0)
	    {
            foreach (CObject obj in mServerObjects.Values)
	        {
                CObject_Character npc = obj as CObject_Character;
                if (npc != null)
                {
                    if (idTable == npc.GetCharacterData().Get_RaceID())
                    {
                        pObject = npc;
                        break;
                    }
                }
	        }
	    }

	    return pObject;
    }

    public void LockNearestTargetToHit()
    {
        CObject_Character pNearestTarget = LockNearestEnemy();
        //下边为挂机增加自动攻击

        if (AutoReleaseSkill.Instance.isAutoSkill())
        {
            if (AutoReleaseSkill.Instance.CanUseNextSkill())
            {
                CObject_Character target = mMainTarget as CObject_Character;
                if (target != null && !target.CannotBeAttack()) mNearestTargetOLD = target;
                if (mNearestTargetOLD == null || mNearestTargetOLD.IsDie())
                    mNearestTargetOLD = pNearestTarget;
                bool needSend = false;
                if (mNeedWaitSkillSend)
                {
                    uint curTime = GameProcedure.s_pTimeSystem.GetTimeNow();
                    if (curTime - mWaitSkillSend >= 1000)
                    {
                        needSend = true;
                        mWaitSkillSend = curTime;
                    }
                }
                else
                {
                    needSend = true;
                }

                if (needSend)
                {
                    tActionItem suitAbleItem = AutoReleaseSkill.Instance.getSuitAbleSkill(mNearestTargetOLD);
                    if (suitAbleItem == null)
                    {
                        suitAbleItem = CActionSystem.Instance.GetDefaultAction();
                    }
                    CActionItem_Skill curSkill = suitAbleItem as CActionItem_Skill;
                    mNeedWaitSkillSend = !curSkill.AutoKeepOn();
                    Vector3 fvMouseHitPlan = Vector3.zero;
                    if (mNearestTargetOLD != null)
                    {
                        fvMouseHitPlan = mNearestTargetOLD.GetPosition();
                    }
                    CursorMng.Instance.MouseCommand_Set(false, mNearestTargetOLD, fvMouseHitPlan, suitAbleItem);
                    GameProcedure.s_pGameInterface.AutoHitState = 1;
                    CursorMng.Instance.MouseCommand_Active(CursorMng.Instance.MouseCommand_GetLeft());
                    GameProcedure.s_pGameInterface.AutoHitState = 0;
                }
            }
            else
            {
                AutoReleaseSkill.Instance.waitNextSkill();
            }
        }
        else
        {
            CObject_Character target = mMainTarget as CObject_Character;
            if (target != pNearestTarget)
			{
				if(target != null && !target.CannotBeAttack()) 
					mNearestTargetOLD = target;
			}
            
            if (mNearestTargetOLD != pNearestTarget && 
                (mNearestTargetOLD == null || mNearestTargetOLD.IsDie()))
            {
                mNearestTargetOLD = pNearestTarget;
				Vector3 fvMouseHitPlan = Vector3.zero;
                bool needSend = false;
                if (mNeedWaitSkillSend)
                {
                    uint curTime = GameProcedure.s_pTimeSystem.GetTimeNow();
                    if (curTime - mWaitSkillSend >= 1000)
                    {
                        needSend = true;
                        mWaitSkillSend = curTime;
                    }
                }
                else
                {
                    needSend = true;
                }

                if (needSend)
                {
                    tActionItem suitAbleItem = null;
                    if (AutoReleaseSkill.Instance.isAutoSkill())
                    {
                        if (AutoReleaseSkill.Instance.CanUseNextSkill())
                        {
                            suitAbleItem = AutoReleaseSkill.Instance.getSuitAbleSkill(pNearestTarget);
                        }
                        else
                        {
                            AutoReleaseSkill.Instance.waitNextSkill();
                        }
                    }
          
                    if (suitAbleItem == null)
                    {
                        suitAbleItem = CActionSystem.Instance.GetDefaultAction();
                    }
                    CActionItem_Skill curSkill = suitAbleItem as CActionItem_Skill;
                    mNeedWaitSkillSend = !curSkill.AutoKeepOn();
                    if (mNearestTargetOLD != null)
                    {
                        fvMouseHitPlan = mNearestTargetOLD.GetPosition();
                    }
                    CursorMng.Instance.MouseCommand_Set(false, mNearestTargetOLD, fvMouseHitPlan, suitAbleItem);
                    GameProcedure.s_pGameInterface.AutoHitState = 1;
                    CursorMng.Instance.MouseCommand_Active(CursorMng.Instance.MouseCommand_GetLeft());
                    GameProcedure.s_pGameInterface.AutoHitState = 0;
                }

                //if (suitAbleItem == null)
                //{
                //    suitAbleItem = CActionSystem.Instance.GetDefaultAction();
                //}
                //CursorMng.Instance.MouseCommand_Set(false, pNearestTarget, fvMouseHitPlan, suitAbleItem);
                ////  CAI_MySelf aiSelf = (CAI_MySelf)(CObjectManager.Instance.getPlayerMySelf().CharacterLogic_GetAI());
                //// if (aiSelf.GetMySelfAI() == ENUM_MYSELF_AI.MYSELF_AI_GUAJI)
                ////无需判定，一定是挂机状态
                //{
                //    GameProcedure.s_pGameInterface.AutoHitState = 1;
                //}
                //CursorMng.Instance.MouseCommand_Active(CursorMng.Instance.MouseCommand_GetLeft());
                //GameProcedure.s_pGameInterface.AutoHitState = 0;
                //mNearestTargetOLD = pNearestTarget;
            }
        }
    }

    /// <summary>
    /// 锁定最近的目标
    /// </summary>
    public CObject_Character LockNearestEnemy()
    {
        if (GetMainTarget() != null && IsEnemy(GetMainTarget()))
            return (CObject_Character)GetMainTarget();

        float fNearestDist = 20.0f; //最大允许距离(m)
        CObject_PlayerMySelf myself = CObjectManager.Instance.getPlayerMySelf();
        CObject_Character pNearestTarget = (CObject_Character)CObjectManager.Instance.GetNearestTargetByOrigin(myself, fNearestDist, true);

        //锁定该目标
        if (pNearestTarget != null && pNearestTarget != GetMainTarget())
        {
            SetMainTarget(pNearestTarget.ServerID, DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT);
        }
        return pNearestTarget;
    }

    private bool IsEnemy(CObject cObject)
    {
        CObject_Character pChar = (CObject_Character)cObject;
        return ENUM_RELATION.RELATION_ENEMY == GameProcedure.s_pGameInterface.GetCampType(
            getPlayerMySelf(), pChar);
    }

	public void ResetNearestTarget()
	{
		mNearestTargetOLD = null;
	}

    public CObject_Character NearestTarget
    {
        get { return mNearestTargetOLD; }
    }

    public void FlashAllTripperObjs()
    {
	    foreach(int k in mServerObjects.Keys)
        {
            if(mServerObjects[k] is CTripperObject_Resource)
            {
                CTripperObject_Resource tripper = (CTripperObject_Resource)mServerObjects[k];
                tripper.Flash(tripper.Tripper_CanOperation());
            }
        }
    }

    public CTripperObject_Resource FindTripperResource(int resourceId)
    {
        foreach (int k in mServerObjects.Keys)
        {
            if (mServerObjects[k] is CTripperObject_Resource)
            {
                CTripperObject_Resource tripper = (CTripperObject_Resource)mServerObjects[k];
                if (tripper.GetTableId() == resourceId)
                    return tripper;
            }
        }
        return null;
    }
}



