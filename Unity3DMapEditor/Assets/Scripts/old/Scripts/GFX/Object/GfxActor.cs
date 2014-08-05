using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundleManager;
namespace GFX
{ 
    public delegate void OnRenderInterfaceEvent(GfxActor actor);
	public class GfxActor:GfxObject //动态创建的物体 by ZZY
	{
		public const  string RIGHT_WEPON_LOACTOR = "RightWeaponLocator";
		public const  string LEFT_WEPON_LOACTOR = "LeftWeaponLocator";
		public const  string WEAPON_EFFECT_LOCATOR = "WeaponEffectLocator";
		public const  string BODY_EFFECT_LOCATOR = "BodyEffectLocator";
		public const  string SHOULDER_EFFECT_LOCATOR = "ShoulderEffectLocator";
        protected Dictionary<string, GameObject> mLocatorMaps = new Dictionary<string, GameObject>();//挂接点名字和go的映射
        
        public const int ActorLayerMask = LayerManager.ActorMask; 
		public const int ActorLayerUnMask = ~ActorLayerMask; 
		
		protected bool		 mActorOutOfDate = true;
		protected bool       mObjectAttachOutOfDate = false;
        protected bool       mAddedEffectOutOfDate = false;
		protected bool		 mEquipOutOfDate = false;
        protected bool       mHitGround = false;  //add by ss 角色碰撞到地面通知
        protected uint       mHitGroundStamp = 0;
        public bool HitGround
        {
            get { return mHitGround; }
            set 
            {
                mHitGround = value;
                if (!value && mHitGroundEvent != null)
                {
                    mHitGroundEvent();
                }
            }
        }

        class EquipAsset
        {
            public string name;
            public AssetBundle asset = null;
        }
        Dictionary<int, EquipAsset> mActorEquipAssets = new Dictionary<int, EquipAsset>();

        protected Dictionary<int, AssetBundleManager.AssetRequest> mActorEquipRequest = new Dictionary<int, AssetBundleManager.AssetRequest>();//gfxObject装备资源请求

		protected class attachedObject
		{
			public string locatorName;
            public GfxObject gfxObject;
			public bool   isAttached;
			public bool   isExternal = false;//是否外部的对象，外部的对象不能删除
		}
        uint mEffectHandleCount = 0;
		protected Dictionary<string, attachedObject> mAttachedObjects = new Dictionary<string, attachedObject>();

        protected class addedEffect
        {
            public string locatorName;
            public GfxObject gfxObject;
            public bool isAttached;
            public uint handle;
        }
        protected List<addedEffect> mAddedEffects = new List<addedEffect>();
		protected string 	mCurrentAnimName = "";
		protected float	mCurrentAnimFadeTime = 0.3f;
		protected bool 	mCurrentAnimLoop = true;
		protected float mOldAnimTime = 0.0f;
        protected GfxSkill mCurrentskill = null;

        WeaponRibbonScript mRightWeaponScript = null;
        WeaponRibbonScript mLeftWeaponScript = null;
        bool mWeaponScriptDirty = false;
        public GFX.GfxSkill Currentskill
        {
            get { return mCurrentskill; }
            set { mCurrentskill = value; }
        }

		public GfxActor(string objectName)
		{
            //mObjectRequest = AssetBundleManager.AssetBundleRequestManager.getActorAsset(objectName);
            string filePath = GfxObject.ActorRoot + objectName + GfxObject.assetbundleExtName;
            ResourceManager.Me.DownloadAsync(filePath, objectName, base.downloadDelegate, AskType.Normal, ResourceType.RT_Actor);
		}

        public override GFXObjectType getObjectType() { return GFXObjectType.ACTOR; }
        protected GameObject getLoactor(string locatorName)
        {
            if (mGameObject == null) return null;
            if(!mLocatorMaps.ContainsKey(locatorName))
            {
                Transform[] actorBones = mGameObject.GetComponentsInChildren<Transform>();
				Transform Locator=null;
				foreach(Transform t in actorBones)
				{
					if(t.name == locatorName)
					{
						Locator = t;
						break;
					}
				}
                if (Locator != null)
                {
                    mLocatorMaps[locatorName] = Locator.gameObject;
                }
                else
                {
                    //LogManager.LogWarning("Can not findLocator:" + locatorName + " so get GameObject");
                    return this.getGameObject();
                }
            }
            return mLocatorMaps[locatorName];
        }
        public override void GetLocator(string locatorName, ref Vector3 Pos)
        {
            GameObject go = getLoactor(locatorName);
            if (go)
                Pos = go.transform.position;
            else
                LogManager.LogWarning("Can not find Loactor:" + locatorName);
        }
		public override void destroy()// destroy gfxObject
		{
			base.destroy();
			//销毁挂接对象
			foreach(KeyValuePair<string, attachedObject> attObject in mAttachedObjects)
			{
				if(mAttachedObjects[attObject.Key].gfxObject !=null && mAttachedObjects[attObject.Key].isExternal == false)
				{
					GFXObjectManager.Instance.DestroyObject(mAttachedObjects[attObject.Key].gfxObject);
				}
			}
            foreach(addedEffect effect in mAddedEffects)
            {
                if(effect.gfxObject != null)
                {
                    GFXObjectManager.Instance.DestroyObject(effect.gfxObject);
                }
            }
		}
		protected void _resetEquip()
		{
			//标记挂接对象
			foreach(KeyValuePair<string, attachedObject> attObject in mAttachedObjects)
			{
				mAttachedObjects[attObject.Key].isAttached = false;//需要重新挂接
			}
			mObjectAttachOutOfDate = true;
		}

        public override void update()//每帧检测是否需要更新装备和挂件
		{
            if (mActorOutOfDate)
            {
                //updateObject();
				base.update();
                //updateEquip();
                updateEquipAsset();
                updateAttacheObject();
                updateAddedEffects();
                if (mObjectOutOfDate == false
                   && mEquipOutOfDate == false
                   && mObjectAttachOutOfDate == false
                   && mAddedEffectOutOfDate == false)
                {
                    mActorOutOfDate = false;
                }
                if (mObjectOutOfDate == true || mEquipOutOfDate == true)//资源没有准备好时显示临时的模型
                {
                    if (mTempObject == null && mTempAssetName != null && mTempAssetName.Length > 0)
                    {
                        mTempObject = GfxGameObjectManager.Instance.Instantiate(Resources.Load(mTempAssetName));
                    }
                }
                else
                {
                    if(mTempObject != null)
                    {
                        GfxGameObjectManager.Instance.Destroy(mTempObject);
                        mTempObject = null;
                    }
                }
            }
            execute(UnityEngine.Time.deltaTime);	
		}

        public override void Attach_ProjTexture(PROJTEX_TYPE type, bool bShow, float Ring_Range, float fHeight, string szTextureName)
        {
            base.Attach_ProjTexture(type, bShow, Ring_Range, fHeight, szTextureName);
            mActorOutOfDate = true;
        }
		protected override void afterUpdateObject()//GO创建之后需要设置物理属性
		{
			if(mGameObject == null) return;
            base.afterUpdateObject();
            mGameObject.layer = LayerManager.ActorLayer;
            if (mGameObject.collider)//如果actor绑定了碰撞体，检查是否有相应的rigidbody
            {
                if (mGameObject.rigidbody==null)
                {
                    mGameObject.AddComponent<Rigidbody>();
                }
                mGameObject.rigidbody.isKinematic = true;//设置为运动学对象
            }
            if ( mGameObject.animation != null )
            {
                mGameObject.animation.playAutomatically = false;
            }
            
            if (mCurrentAnimName != "")
            {
                changeAnimation(mCurrentAnimName, mCurrentAnimLoop, mCurrentAnimFadeTime);
            }
            if (mRenderInterfaceCreateEvt != null)//通知渲染对象已经创建
                mRenderInterfaceCreateEvt(this);

		}
		protected bool isEquipAssetBundleReady()//检查角色装备资源下载请求是否完成
		{
            foreach (KeyValuePair<int, AssetBundleManager.AssetRequest> r in mActorEquipRequest)
			{
                if ( (r.Value != null && !r.Value.isDone) )
				{
					return false;
				}
			}
			return true;
		}
        protected bool isEquipAsssetReady()
        {
            foreach ( EquipAsset asset in mActorEquipAssets.Values)
            {
                if (asset != null && asset.asset == null)
                    return false;
            }
            return true;

        }
        protected void updateEquipAsset()
        {
            if (mEquipOutOfDate == false
               || isEquipAsssetReady() == false
               || mObjectOutOfDate
               || mGameObject == null)
            {
                return;
            }
            _resetEquip();//重置设备
            SkinnedMeshRenderer actorRenderer = mGameObject.GetComponent<SkinnedMeshRenderer>();
            if (actorRenderer != null)
            {
                Transform[] actorBones = mGameObject.GetComponentsInChildren<Transform>();
                List<Transform> bones = new List<Transform>();
                List<CombineInstance> combineInstances = new List<CombineInstance>();
                List<Material> materials = new List<Material>();
                //创建各个部分
                foreach (EquipAsset ea in mActorEquipAssets.Values)
                {
                    if (ea == null || ea.asset == null) continue;
                    //Object[] allAsset = equipReq.mWWW.assetBundle.LoadAll();
                    GameObject equipGO = GfxGameObjectManager.Instance.Instantiate(ea.asset.Load(ea.name, typeof(GameObject)));
                    string[] equipBoneNames = ((StringHolder)ea.asset.Load("bonenames", typeof(StringHolder))).content;

                    foreach (string bone in equipBoneNames)//查找这个装备部分用到的骨骼
                    {
                        foreach (Transform transform in actorBones)
                        {
                            if (transform.name != bone) continue;
                            bones.Add(transform);
                            break;
                        }
                    }
                    SkinnedMeshRenderer equipRenderer = equipGO.GetComponent<SkinnedMeshRenderer>();
                    materials.AddRange(equipRenderer.materials);
                    for (int sub = 0; sub < equipRenderer.sharedMesh.subMeshCount; sub++)
                    {
                        CombineInstance ci = new CombineInstance();
                        ci.mesh = equipRenderer.sharedMesh;
                        ci.subMeshIndex = sub;
                        combineInstances.Add(ci);
                    }
                    //Object.Destroy(equipRenderer.gameObject);
                    GfxGameObjectManager.Instance.Destroy(equipRenderer.gameObject);
                }
                if (combineInstances.Count != 0)
                {
                    actorRenderer.sharedMesh = new Mesh();
                    actorRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);//组合网格
                    actorRenderer.bones = bones.ToArray();
                    actorRenderer.materials = materials.ToArray();
                }
            }

            mEquipOutOfDate = false;
        }
		protected void updateEquip()  
		{
			if(mEquipOutOfDate == false 
			   || isEquipAssetBundleReady()==false  
			   || mObjectOutOfDate 
			   || mGameObject ==null)
			{
				return;
			}
			_resetEquip();//重置设备
			SkinnedMeshRenderer actorRenderer = mGameObject.GetComponent<SkinnedMeshRenderer>();
			if(actorRenderer != null)
			{
				Transform[] actorBones = mGameObject.GetComponentsInChildren<Transform>();
				List<Transform> bones = new List<Transform>();
				List<CombineInstance> combineInstances = new List<CombineInstance>();
	        	List<Material> materials = new List<Material>();
				//创建各个部分
				foreach (KeyValuePair<int, AssetBundleManager.AssetRequest> r in mActorEquipRequest)
				{
					AssetBundleManager.AssetRequest equipReq = r.Value;
                    if (equipReq == null || !equipReq.mSuccess) continue;
					//Object[] allAsset = equipReq.mWWW.assetBundle.LoadAll();
                    GameObject equipGO = GfxGameObjectManager.Instance.Instantiate(equipReq.mWWW.assetBundle.Load(equipReq.mOjbectName, typeof(GameObject)));
					string[] equipBoneNames = ((StringHolder)equipReq.mWWW.assetBundle.Load("bonenames", typeof(StringHolder))).content;
					
					foreach (string bone in equipBoneNames)//查找这个装备部分用到的骨骼
	                {
	                    foreach (Transform transform in actorBones)
	                    {
	                        if (transform.name != bone) continue;
	                        bones.Add(transform);
	                        break;
	                    }
	                }
	               SkinnedMeshRenderer equipRenderer = equipGO.GetComponent<SkinnedMeshRenderer>();
				   materials.AddRange(equipRenderer.materials);
				   for (int sub = 0; sub < equipRenderer.sharedMesh.subMeshCount; sub++)
	               {
	                	CombineInstance ci = new CombineInstance();
	                	ci.mesh = equipRenderer.sharedMesh;
	                	ci.subMeshIndex = sub;
	                	combineInstances.Add(ci);
	               }
	               //Object.Destroy(equipRenderer.gameObject);
                   GfxGameObjectManager.Instance.Destroy(equipRenderer.gameObject);
				}
				if(combineInstances.Count != 0)
				{
					actorRenderer.sharedMesh = new Mesh();
		        	actorRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);//组合网格
		        	actorRenderer.bones = bones.ToArray();
		       		actorRenderer.materials = materials.ToArray();
				}
			}

			mEquipOutOfDate = false;

		}
		protected void updateAttacheObject()
		{
			if(mObjectAttachOutOfDate && !mObjectOutOfDate && mGameObject !=null)
			{
				foreach(KeyValuePair<string, attachedObject> attObject in mAttachedObjects)
				{
					bool attached = attObject.Value.isAttached;
                    if (!attached && attObject.Value.gfxObject.getGameObject() != null)//开始挂接gfxObject
					{
                        GameObject Locator = getLoactor(attObject.Value.locatorName);
						attObject.Value.isAttached = true;
						if(Locator == null) continue;	
						GameObject attGo = attObject.Value.gfxObject.getGameObject();
						GfxUtility.attachGameObject(attGo, Locator);
					}
				}
				//查看是否完成所有的挂接
				bool allAttached = true;
				foreach(KeyValuePair<string, attachedObject> attObject in mAttachedObjects)
				{
					if(!attObject.Value.isAttached)
					{
						allAttached = false;
						break;
					}
				}
				mObjectAttachOutOfDate = (allAttached==false);
			}

		}
        protected void updateAddedEffects()//更新所有挂接的特效
        {
            if (mAddedEffectOutOfDate && !mObjectOutOfDate && mGameObject != null)
            {
                bool allAttached = true;
                foreach (addedEffect effect in mAddedEffects)
                {
                    if (!effect.isAttached)
                    {
                        if (effect.gfxObject.getGameObject() != null)
                        {
                            GameObject Locator = getLoactor(effect.locatorName);
                            effect.isAttached = true;
                            if (Locator == null) continue;
                            GameObject attGo = effect.gfxObject.getGameObject();
                            GfxUtility.attachGameObject(attGo, Locator);
                        }
                        else
                        {
                            allAttached = false;
                        }
                    }
                }

                mAddedEffectOutOfDate = (allAttached == false);
            }
        }
		//更换装备接口
		public override void changePart(int part, string equipName)
		{
            mActorOutOfDate = true;
			mEquipOutOfDate = true;
            if (equipName == "")//名字为空表示去掉这个装备
            {
                //mActorEquipRequest[part] = null;
                mActorEquipAssets[part] = null;
                return;
            }
           // mActorEquipRequest[part] = AssetBundleManager.AssetBundleRequestManager.getEquipAsset(equipName);

            EquipAsset equipAsset = new EquipAsset();
            equipAsset.name = equipName;
            mActorEquipAssets[part] = equipAsset;

            string filePath = GfxObject.EquipRoot + equipName + GfxObject.assetbundleExtName;
            ResourceManager.Me.DownloadAsync(filePath, part, downloadEquipAssetDelegate);

		}
        void downloadEquipAssetDelegate(System.Object obj, AssetBundle asset)
        {
            if (asset == null) return;
            int part = (int)obj;

            if (mActorEquipAssets[part] != null)
            {
                mActorEquipAssets[part].asset = asset;
            }
        }
		void delCurrentSkill()
		{
            EnableWeaponRibbon(false);   
			GfxSkillManager.Instance.removeSkill(mCurrentskill);
			mCurrentskill = null;
		}
		
        public override void EnterSkill(bool bAnim, string szSkillName, bool bLoop, float fFuseParam) 
        {
			delCurrentSkill();
			mCurrentskill = GfxSkillManager.Instance.createSkill(szSkillName);
            if(!bAnim )
            {
                if (mCurrentskill == null)
                {
                    changeAnimation(szSkillName, bLoop, fFuseParam);
                }
                else
                {
                    //LogManager.Log("EnterSkill playskillname" + mCurrentskill.getAnimateName());
                    if (mCurrentskill.EnableRibbon)
                    {
                        EnableWeaponRibbon(true);
                    }
                    if (mCurrentskill.getAnimateName().Length > 0)
                    {
                        changeAnimation(mCurrentskill.getAnimateName(), bLoop, fFuseParam);
                    }
                    else
                    {
                        changeAnimation(mCurrentAnimName, mCurrentAnimLoop, mCurrentAnimFadeTime);
                    }
                
                }
            }
            else
            {
                changeAnimation(szSkillName, bLoop, fFuseParam);
            }
        }
        void EnableWeaponRibbon(bool enable)//使用技能时允许武器刀光
        {
            if (mWeaponScriptDirty)
            {
                if (mAttachedObjects.ContainsKey(RIGHT_WEPON_LOACTOR) &&//右手武器
                mAttachedObjects[RIGHT_WEPON_LOACTOR].gfxObject != null)
                {
                    GfxObject obj = mAttachedObjects[RIGHT_WEPON_LOACTOR].gfxObject;
                    if (obj.getGameObject() == null)
                    {
                        return;
                    }
                    mRightWeaponScript = obj.getGameObject().GetComponentInChildren<WeaponRibbonScript>();

                }
                if (mAttachedObjects.ContainsKey(LEFT_WEPON_LOACTOR) &&//左手武器
                mAttachedObjects[LEFT_WEPON_LOACTOR].gfxObject != null)
                {
                    GfxObject obj = mAttachedObjects[LEFT_WEPON_LOACTOR].gfxObject;
                    if (obj.getGameObject() == null)
                    {
                        return;
                    }
                    mLeftWeaponScript = obj.getGameObject().GetComponentInChildren<WeaponRibbonScript>();
                }
                mWeaponScriptDirty = false;
            }

            if (mRightWeaponScript != null)
            {
                mRightWeaponScript.enable = enable;
            }
            if (mLeftWeaponScript != null)
            {
                mLeftWeaponScript.enable = enable;
            }

        }
        protected  void _createAnimationEffect(float oldTime, float currTime)
        {
            for(int i = 0; i < mCurrentskill.getSkillEffectCount();i++)
            {
               GfxSkillEffect skillEffect = mCurrentskill.getSkillEffect(i);
               float AttachTime = skillEffect.AttachTime;
               if((currTime >= oldTime && AttachTime >= oldTime && AttachTime < currTime) || 
				    (currTime < oldTime && (AttachTime >= oldTime || AttachTime < currTime)))
               {
                    GfxEffect effect = skillEffect.Effect;
                    // 获取脚本中的特效名称
                    string effectTemplateName = skillEffect.EffectName;
                    // 如果是模板特效名称
                    float terrainHeight = Mathf.Infinity;
                    ////////////////////////////////////////////////////////////////////////////
                    string attachPoint = skillEffect.AttachPoint;

                    //// 先从绑定点找，如果没有再从骨头名称找
                    GameObject  go = getLoactor(attachPoint);
					if(go == null)
					{
                        continue;
					}
					
                    Vector3     newPosition  = go.transform.position + go.transform.rotation * skillEffect.OffsetPos;
                    Quaternion  newRotation  = go.transform.rotation * skillEffect.OffsetRotation;
                    Vector3     newScale     = new Vector3(go.transform.localScale.x * skillEffect.OffsetScale.x,
                                                           go.transform.localScale.y * skillEffect.OffsetScale.y,
                                                           go.transform.localScale.z * skillEffect.OffsetScale.z);
                    
                   
                   
                    if (newPosition.y >= terrainHeight)
                        return;
                    ////////////////////////////////////////////////////////////////////////////
                    if (effect != null)
                    {
                        // 如果动作是循环的(可以走到这里，这个动作肯定是loop的)，而且当前skill需要在每次进行动作时都创建一个新特效
                        if (mCurrentskill.getRepeatEffect())
                        {
                            // 先把旧的删除，在创建新的
							GFXObjectManager.Instance.DestroyObject(effect);
                            effect = GFXObjectManager.Instance.createObject(effectTemplateName, GFXObjectType.EFFECT) as GfxEffect;
                            skillEffect.Effect = effect;
                        }
                    }
                    else
                    {
                        effect = GFXObjectManager.Instance.createObject(effectTemplateName, GFXObjectType.EFFECT) as GfxEffect;
                        skillEffect.Effect = effect;
                    }

                    if ( false == skillEffect.Attach)
                    {
                        // 给effect传入点 
                        effect.position = newPosition;
                        effect.rotation = newRotation;
                        effect.scale = newScale;
                    }
               }
            }
        }

        protected void _updateAttachedAnimationEffect(float oldTime, float currTime)
        {
            for ( int i = 0; i < mCurrentskill.getSkillEffectCount(); ++i )
		    {
			    GfxSkillEffect skillEffect = mCurrentskill.getSkillEffect(i);
                GfxEffect effect = skillEffect.Effect;
                if (effect != null && skillEffect.Attach )
                {
                    // 获取脚本中的特效名称
                    string effectTemplateName = skillEffect.EffectName;
                    string attachPoint = skillEffect.AttachPoint;
                    GameObject  go = getLoactor(attachPoint);
                    if (go == null)
                    {
                        continue;
                    }
                    Vector3     newPosition  = go.transform.position + go.transform.rotation * skillEffect.OffsetPos;
                    Quaternion  newRotation  = go.transform.rotation * skillEffect.OffsetRotation;
                    Vector3     newScale     = new Vector3(go.transform.localScale.x * skillEffect.OffsetScale.x,
                                                           go.transform.localScale.y * skillEffect.OffsetScale.y,
                                                           go.transform.localScale.z * skillEffect.OffsetScale.z);
        
                    // 给effect传入点 
                    effect.position = newPosition;
                    effect.rotation = newRotation;
                    effect.scale = newScale;
                }
		    }
        }

		protected  void changeAnimation(string animName, bool loop, float fadeTime)
		{
			if(mGameObject!=null && mGameObject.animation)
			{
                if (mGameObject.animation[animName] != null)
                {
                    mGameObject.animation.wrapMode = loop ? WrapMode.Loop:WrapMode.ClampForever;
                    mGameObject.animation[animName].speed = mActionRate;
					mGameObject.animation[animName].time = 0.0f;
                    mGameObject.animation.CrossFade(animName, fadeTime);
                }
                else
                {
                    //该动画不存在
                    LogManager.LogWarning("can not find animation:" + animName);
                }
				mOldAnimTime = 0.0f;
			}
			mCurrentAnimName = animName;
			mCurrentAnimFadeTime = fadeTime;
			mCurrentAnimLoop = loop;//游戏逻辑上动画是否循环 zzy
		}

        protected float mActionRate = 1.0f;
        public override void ChangeActionRate(float fRate)
        {
            mActionRate = fRate;
            if (mGameObject != null && 
                mGameObject.animation&&
                mGameObject.animation[mCurrentAnimName] != null)
            {
                mGameObject.animation[mCurrentAnimName].speed = mActionRate;
            }
        }

        public override void Attach_Object(GfxObject pObject, string szAttachLocator)
        {
            attachObject(szAttachLocator, pObject, false);
        }
        public override void Detach_Object(GfxObject pObject)
        {
            detachObject(pObject);
        }

        public override void attachActor(string locatorName, string actorName)
        {
            attachObject(locatorName, actorName, GFXObjectType.ACTOR);
        }
        public override void attachEffect(string locatorName, string effectName)
        {
            attachObject(locatorName, effectName, GFXObjectType.EFFECT);
        }
        public override void PlayEffect(string locatorName, string effectName) 
        {
            GameObject Locator = getLoactor(locatorName);
            if(Locator != null)
            {
                GfxObject newEffect = GFXObjectManager.Instance.createObject(effectName, GFXObjectType.EFFECT);
                newEffect.position = Locator.transform.position;
            }
        }
        protected  void attachObject(string locatorName, GfxObject obj, bool destroyOldObj)//挂接一个已经存在的对象, destroyOldObj是否删除上次挂接的对象
        {
            if (destroyOldObj && mAttachedObjects.ContainsKey(locatorName))
            {
                if (mAttachedObjects[locatorName].gfxObject != null) GFXObjectManager.Instance.DestroyObject(mAttachedObjects[locatorName].gfxObject);
                mAttachedObjects.Remove(locatorName);
            }
            if (obj == null) return;
            attachedObject newAttachedObject = new attachedObject();
            newAttachedObject.locatorName = locatorName;
            newAttachedObject.isAttached = false;
			newAttachedObject.isExternal = true;
            newAttachedObject.gfxObject = obj;
            newAttachedObject.gfxObject.Parent = this;
            mAttachedObjects[locatorName] = newAttachedObject;
            mObjectAttachOutOfDate = true;
            mActorOutOfDate = true;
        }

        protected  void detachObject( GfxObject obj)//分离一个对象
        {
            foreach (string k in mAttachedObjects.Keys)
            {
                if(mAttachedObjects[k].gfxObject == obj)
                {
                    mAttachedObjects.Remove(k);
                    obj.Parent = null;
					if(obj.getGameObject() != null)
                    	obj.getGameObject().transform.parent = null;
                    break;
                }
            }
        }

		protected void attachObject(string locatorName, string objectName, GFX.GFXObjectType objType)//创建一个新的gfxObject并挂接
		{
			if(mAttachedObjects.ContainsKey(locatorName))
			{
				if(mAttachedObjects[locatorName].gfxObject!=null) GFXObjectManager.Instance.DestroyObject(mAttachedObjects[locatorName].gfxObject);
				mAttachedObjects.Remove(locatorName);
			}
			if(objectName=="") return ;
			attachedObject newAttachedObject = new attachedObject();
            newAttachedObject.locatorName = locatorName;
            //newAttachedObject.objectName = objectName;
            newAttachedObject.isAttached = false;
            newAttachedObject.gfxObject = GFXObjectManager.Instance.createObject(objectName, objType);
            newAttachedObject.gfxObject.Parent = this;
            mAttachedObjects[locatorName] = newAttachedObject;
			mObjectAttachOutOfDate = true;
			mActorOutOfDate = true;
		}
		
        // 新版附加一个特效, 一个挂点可挂接多个特效
        public override uint AddEffect(string effectName, string locatorName)
        {
            addedEffect newEffect = new addedEffect();
            newEffect.locatorName = locatorName;
            newEffect.isAttached = false;
            newEffect.gfxObject = GFXObjectManager.Instance.createObject(effectName, GFXObjectType.EFFECT);
            mEffectHandleCount++;
            newEffect.handle = mEffectHandleCount;
            mAddedEffects.Add(newEffect);
            mAddedEffectOutOfDate = true;
            mActorOutOfDate = true;
            return mEffectHandleCount; 
        }
        public override void DelEffect(uint effect)
        {
            if (effect > 0)
            {
                foreach( addedEffect e in mAddedEffects)
                {
                    if(e.handle == effect)
                    {
                        mAddedEffects.Remove(e);
                        GFXObjectManager.Instance.DestroyObject(e.gfxObject);
						break;
                    }
                }
            }
        }
  //      public override void DelEffect(string szLocatorName)
  //    {
  //      }
  //      public override void DelAllEffect()
   //     {
   //     }

		public override void changeRightWeapon(string weaponName)//更换右手武器,名字为空时去掉该武器
		{
			attachActor(RIGHT_WEPON_LOACTOR, weaponName);
            mWeaponScriptDirty = true;
		}
		public override void changeLeftWeapon(string weaponName)//更换左手武器
		{
            attachActor(LEFT_WEPON_LOACTOR, weaponName);
            mWeaponScriptDirty = true;
		}
		public override void SetRightWeaponEffect(string effectName)//设定右手武器特效
		{
			if( mAttachedObjects.ContainsKey(RIGHT_WEPON_LOACTOR) )
			{
				if(mAttachedObjects[RIGHT_WEPON_LOACTOR].gfxObject!=null)
				{
					((GfxActor)mAttachedObjects[RIGHT_WEPON_LOACTOR].gfxObject).attachEffect(WEAPON_EFFECT_LOCATOR, effectName);
				}
			}
		}
		
		public override void SetLeftWeaponEffect(string effectName)//设定左手武器特效
		{
			if(mAttachedObjects.ContainsKey(	LEFT_WEPON_LOACTOR))
			{
				if(mAttachedObjects[LEFT_WEPON_LOACTOR].gfxObject!=null)
				{
                    ((GfxActor)mAttachedObjects[LEFT_WEPON_LOACTOR].gfxObject).attachEffect(WEAPON_EFFECT_LOCATOR, effectName);
				}
			}
		}
		public  override void setBodyEffect(string effectName) //身体装备特效
		{
            attachEffect(BODY_EFFECT_LOCATOR, effectName);
		}
		public override void setShoulderEffect(string effectName) //肩胛装备特效
		{
            attachEffect(SHOULDER_EFFECT_LOCATOR, effectName);
		}

		public List<string> getAnimationName()//获得所有可用的动画名称
		{
			List<string> animNames = new List<string>();
			if( mGameObject!=null && mGameObject.animation != null )
			{
				foreach(AnimationState state in mGameObject.animation)
				{
					animNames.Add(state.name);
				}
			}
			return animNames;
		}
		public override void SetMouseHover(bool bHover)
        {

            if (mGameObject != null)
            {
                Renderer[] allRenderer = mGameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in allRenderer)
                {
                    if (r.materials != null)
                    {
                        foreach (Material mat in r.materials)
                        {
                            if (mat.HasProperty("_Selected"))
                            {
                                mat.SetFloat("_Selected", bHover ? 1.0f : 0.0f);
                            }
                        } 
                    }
                }

            }
        }
        protected OnAnimationEvent mAnimationEndEvent;
        protected OnAnimationEvent mAnimationCanBreakEvent;
        protected OnAnimationEvent mAnimationHitEvent;
        protected OnAnimationEvent mAnimationShakeEvent;
        protected OnHitGroundedEvent mHitGroundEvent;
        protected OnRenderInterfaceEvent mRenderInterfaceCreateEvt;

       //设置角色动画结束通知
        public override void SetAnimationEndEvent(OnAnimationEvent pFunc) 
        {
            mAnimationEndEvent = pFunc;
        }
        //设置角色动画可以结束通知
        public override void SetAnimationCanBreakEvent(OnAnimationEvent pFunc)
        {
            mAnimationCanBreakEvent = pFunc; 
        }
        //设置角色动画打击点通知
        public override void SetAnimationHitEvent(OnAnimationEvent pFuncm)
        {
            mAnimationHitEvent = pFuncm;
        }

        public override void SetAnimationShakeEvent(OnAnimationEvent pFunc)
        {
            mAnimationShakeEvent = pFunc;
        }

        public override void SetHitGroundedEvent(OnHitGroundedEvent pFunc)
        {
            mHitGroundEvent = pFunc;
        }

        public void setRenderInterfaceCreateEvt(OnRenderInterfaceEvent evt)
        {
            mRenderInterfaceCreateEvt = evt;
        }
        protected bool _handleAnimationFinish(float oldTime, float currTime)
        {
            if (  (currTime<  oldTime)
                && (false == mCurrentAnimLoop) )
            {
                mOldAnimTime = 0;
                if ( mAnimationEndEvent != null )
                {
                    mAnimationEndEvent(mCurrentAnimName);
                    return false;
                }
            }
            return true;
        }
        protected void _handleSkillHitTime(float oldTime, float currTime)
        {
            if (mCurrentskill != null)
		    {
                int hitTimeCount = mCurrentskill.getHitTimeCount();
			    for (int i=0; i< hitTimeCount; ++i)
			    {
                    float hitTime = mCurrentskill.getHitTime(i);
				    if (hitTime >= 0.0f && hitTime <= 1.0f)
				    {
					    if((currTime >= oldTime && hitTime >= oldTime && hitTime < currTime) || 
						    (currTime < oldTime && (hitTime >= oldTime || hitTime < currTime)))
					    {
                            if (mAnimationHitEvent != null)
                            {
                                //  [2/13/2011 ivan edit]
                                //mOnSkillHitTime(mCurrentAnimationName.c_str(), mCallbackFuncInfo, i, hitTimeCount);
                                mAnimationHitEvent(mCurrentAnimName);
                            }
							
                            if (mCurrentskill == null)
							    break;
					    }
				    }
			    }
		    }		
        }
        protected void _handleSkillBreakTime(float oldTime, float currTime)
        {
            if (mCurrentskill != null)
		    {
                int breakTimeCount = mCurrentskill.getBreakTimeCount();
			    for (int i= 0; i<breakTimeCount; ++i)
			    {
                    float breakTime = mCurrentskill.getBreakTime(i);

				    if (breakTime >= 0.0f && breakTime <= 1.0f)
				    {
					    if((currTime >= oldTime && breakTime >= oldTime && breakTime < currTime) || 
						    (currTime < oldTime && (breakTime >= oldTime || breakTime < currTime)))
					    {
                            if (mAnimationCanBreakEvent != null)
                            {
                                //mOnSkillBreakTime(mCurrentAnimationName.c_str(), mCallbackFuncInfo, i, breakTimeCount);
                                mAnimationCanBreakEvent(mCurrentAnimName);
                            }

                            if (mCurrentskill == null)
							    break;
					    }
				    }
			    }
		    }
        }
        protected void _handleSkillShakeTime(float oldTime, float currTime)
        {
            if (mCurrentskill != null)
            {
                for (int i = 0; i < mCurrentskill.getShakeTimeCount(); ++i)
                {
                    float shakeTime = mCurrentskill.getShakeTime(i);

                    if (shakeTime >= 0.0f && shakeTime <= 1.0f)
                    {
                        if ((currTime >= oldTime && shakeTime >= oldTime && shakeTime < currTime) ||
                            (currTime < oldTime && (shakeTime >= oldTime || shakeTime < currTime)))
                        {
                            if (mAnimationShakeEvent != null)
                                mAnimationShakeEvent(mCurrentAnimName);
                        }
                    }
                }
            }
        }
        //0表明没有撞击地面特效 1表明没有到达时间点 2已经到达时间点
        protected int _handleHitGroundShakeTime(float oldTime,float curTime)
        {
            if (mCurrentskill != null && mCurrentskill.HitGroundEffect != null)
            {
                float shakeTime = mCurrentskill.HitGroundEffect.ShakeTime;

                if (shakeTime >= 0.0f && shakeTime <= 1.0f)
                {
                    if ((curTime >= oldTime && shakeTime >= oldTime && shakeTime < curTime) ||
                        (curTime < oldTime && (shakeTime >= oldTime || shakeTime < curTime)))
                    {
                        if (mAnimationShakeEvent != null)
                            mAnimationShakeEvent(mCurrentAnimName);
                        return 2;
                    }
                }
                return 1;
            }
            return 0;
        }
        //0表明没有撞击地面特效 1表明没有到达时间点 2已经到达时间点
        protected int _createHitGroundEffect(float oldTime,float curTime)
        {
            if (mCurrentskill != null && mCurrentskill.HitGroundEffect != null)
            {
               float AttachTime = Currentskill.HitGroundEffect.AttachTime;
               if ((curTime >= oldTime && AttachTime >= oldTime && AttachTime < curTime) ||
                    (curTime < oldTime && (AttachTime >= oldTime || AttachTime < curTime)))
               {
                    GfxEffect effect = mCurrentskill.HitGroundEffect.Effect;
                    // 获取脚本中的特效名称
                    string effectTemplateName = mCurrentskill.HitGroundEffect.EffectName;
                    // 如果是模板特效名称
                    float terrainHeight = Mathf.Infinity;
                    ////////////////////////////////////////////////////////////////////////////
                    string attachPoint = mCurrentskill.HitGroundEffect.AttachPoint;

                    //// 先从绑定点找，如果没有再从骨头名称找
                    GameObject  go = getLoactor(attachPoint);
					if(go == null)
					{
                        return 0;
					}

                    Vector3 newPosition = go.transform.position + go.transform.rotation * mCurrentskill.HitGroundEffect.OffsetPos;
                    Quaternion newRotation = go.transform.rotation * mCurrentskill.HitGroundEffect.OffsetRotation;
                    Vector3 newScale = new Vector3(go.transform.localScale.x * mCurrentskill.HitGroundEffect.OffsetScale.x,
                                                           go.transform.localScale.y * mCurrentskill.HitGroundEffect.OffsetScale.y,
                                                           go.transform.localScale.z * mCurrentskill.HitGroundEffect.OffsetScale.z);
                    
                   
                   
                    if (newPosition.y >= terrainHeight)
                        return 0;
                    ////////////////////////////////////////////////////////////////////////////
                    if (effect != null)
                    {
                        // 如果动作是循环的(可以走到这里，这个动作肯定是loop的)，而且当前skill需要在每次进行动作时都创建一个新特效
                        if (mCurrentskill.getRepeatEffect())
                        {
                            // 先把旧的删除，在创建新的
							GFXObjectManager.Instance.DestroyObject(effect);
                            effect = GFXObjectManager.Instance.createObject(effectTemplateName, GFXObjectType.EFFECT) as GfxEffect;
                            mCurrentskill.HitGroundEffect.Effect = effect;
                        }
                    }
                    else
                    {
                        effect = GFXObjectManager.Instance.createObject(effectTemplateName, GFXObjectType.EFFECT) as GfxEffect;
                        mCurrentskill.HitGroundEffect.Effect = effect;
                    }

                    if (false == mCurrentskill.HitGroundEffect.Attach)
                    {
                        // 给effect传入点 
                        effect.position = newPosition;
                        effect.rotation = newRotation;
                        effect.scale = newScale;
                    }
                    return 2;
               }
               return 1;
            }
            return 0;
        }

        protected void _updateHitGroundEffect(float oldTime, float curTime)
        {
            if (mCurrentskill != null && mCurrentskill.HitGroundEffect != null)
            {
                GfxEffect effect = mCurrentskill.HitGroundEffect.Effect;
                if (effect != null && mCurrentskill.HitGroundEffect.Attach)
                {
                    // 获取脚本中的特效名称
                    string effectTemplateName = mCurrentskill.HitGroundEffect.EffectName;
                    string attachPoint = mCurrentskill.HitGroundEffect.AttachPoint;
                    GameObject go = getLoactor(attachPoint);
                    if (go == null)
                    {
                        return;
                    }
                    Vector3 newPosition = go.transform.position + go.transform.rotation * mCurrentskill.HitGroundEffect.OffsetPos;
                    Quaternion newRotation = go.transform.rotation * mCurrentskill.HitGroundEffect.OffsetRotation;
                    Vector3 newScale = new Vector3(go.transform.localScale.x * mCurrentskill.HitGroundEffect.OffsetScale.x,
                                                           go.transform.localScale.y * mCurrentskill.HitGroundEffect.OffsetScale.y,
                                                           go.transform.localScale.z * mCurrentskill.HitGroundEffect.OffsetScale.z);

                    // 给effect传入点 
                    effect.position = newPosition;
                    effect.rotation = newRotation;
                    effect.scale = newScale;
                }
            }
        }

        public override void OnHitGroundEvent()
        {
            if (mCurrentskill != null && mCurrentskill.HitGroundEffect != null)
            {
                changeAnimation(mCurrentskill.HitGroundEffect.AnimateName, mCurrentAnimLoop, mCurrentAnimFadeTime);
                mHitGround = true;
                mHitGroundStamp = GameProcedure.s_pTimeSystem.GetTimeNow();
            }
        }

        /// 每帧中进行必要的更新，如动作，特效，skill的更新
        protected void execute(float deltaTime)
        {
            if (mGameObject == null || mGameObject.animation == null) return;
            
            AnimationState animState = mGameObject.animation[mCurrentAnimName];
			if(animState == null) return ;
            float curTime = animState.time;//当前帧的动画时间

            int a = (int)(curTime / animState.length);
            curTime = curTime - a * animState.length;
            //处理动画结束事件
            bool animationConitnue = _handleAnimationFinish(mOldAnimTime, curTime);
            if(animationConitnue)
            {
                //todo
                 if (mCurrentskill != null)
                 {
                     //如果现在是碰撞地面状态，创建击中地面特效和震动摄像机
                     if (mCurrentskill.isHitGroundEffectExist() && HitGround && mVisible)
                     {
                         int resultType1 = _handleHitGroundShakeTime(mOldAnimTime, curTime);
                         int resultType2 = _createHitGroundEffect(mOldAnimTime, curTime);
                         _updateHitGroundEffect(mOldAnimTime, curTime);
                         if ((resultType1 == 0 && resultType2 == 0) ||
                             (resultType1 == 2 && resultType2 == 2))
                         {
                             HitGround = false;
                         }
                         if (HitGround)
                         {
                             uint curHitTime = GameProcedure.s_pTimeSystem.GetTimeNow();
                             if (curHitTime - mHitGroundStamp >= 10000)
                             {
                                 LogManager.LogError("HitGround time out of max time 10s");
                                 HitGround = false;
                             }
                         }
                     }
                     else
                     {
                         _handleSkillHitTime(mOldAnimTime, curTime);
                         _handleSkillBreakTime(mOldAnimTime, curTime);
                         _handleSkillShakeTime(mOldAnimTime, curTime);

                         if (mCurrentskill != null && mVisible)
                         {
                             _createAnimationEffect(mOldAnimTime, curTime);
                             _updateAttachedAnimationEffect(mOldAnimTime, curTime);

                             //_createAnimationRibbon(mOldAnimTime, curTime);
                             //_updateAnimationRibbon(deltaTime);

                             //_updateAnimationSound(mOldAnimTime, curTime);

                             //_updateAnimationLight(mOldAnimTime, curTime);
                         }
                     }
                 }
                 mOldAnimTime = curTime;//保存当上一帧的动画时间
            }
           
        }

        Collider boxCollider = null;
        //取得"头顶状态点"在UI摄像机内的世界坐标,如果返回FALSE，表示在屏幕之外,或者没有该点
        public override bool GetInfoBoardPos(ref Vector3 newInfoPos, ref Vector3 objWorldPos, float fObligeHeight) 
        {
            if (mGameObject == null)
                return false;

            Vector3 vPos = objWorldPos;
            //非主角位置
            if (boxCollider == null)
                boxCollider = mGameObject.GetComponent<Collider>();
            float s_fAddHeight = boxCollider.bounds.max.y - boxCollider.bounds.min.y;

            //使用数据表中指定高度
            if (fObligeHeight > 0)
                vPos.y += fObligeHeight;
            else
                //使用Boundbox高度
                vPos.y += s_fAddHeight;

            //为了不和人物重叠，额外拉高
            vPos.y += 0.3f + 0.5f * SceneCamera.Instance.getZoom();

            //主摄像机的人物坐标转换到屏幕坐标
            Vector3 viewPos = SceneCamera.Instance.UnityMainCamera.WorldToViewportPoint(vPos);
            if (viewPos.x < 0.01f || viewPos.y < 0.01f || viewPos.x > 0.99f
                || viewPos.y > 0.99f)
            {
                return false;
            }

            //屏幕坐标转换到UI摄像机内的世界坐标
            newInfoPos = UISystem.Instance.UiCamrea.ViewportToWorldPoint(viewPos);

            //平滑处理
            //                 fvPos.x = SMOOTH(fvPos.x, fvScreen.x, 0.8f, 3.f);
            //                 fvPos.y = SMOOTH(fvPos.y, fvScreen.y, 0.8f, 3.f);

            return true;
        }
        public override bool GetInfoBoardPos(ref Vector3 ivPos, ref Vector3 pvObjPos)
        {
            return GetInfoBoardPos(ref ivPos, ref pvObjPos, -1);
        }

        public override void SetVisible(bool bVisible)
        {
            base.SetVisible(bVisible);
            foreach (KeyValuePair<string, attachedObject> attObject in mAttachedObjects)
            {
                if (attObject.Value.gfxObject != null)//开始挂接gfxObject
                {
                    attObject.Value.gfxObject.SetVisible(bVisible);
                }
            }
        }

        public override void SetTransparent(float fTransparency, float fTime)
        {
            base.SetTransparent(fTransparency, fTime);
            foreach (KeyValuePair<string, attachedObject> attObject in mAttachedObjects)
            {
                if (attObject.Value.gfxObject != null)//开始挂接gfxObject
                {
                    attObject.Value.gfxObject.SetTransparent(fTransparency, fTime);
                }
            }
        }   
	}
}

