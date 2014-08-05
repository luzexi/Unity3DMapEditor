using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundleManager;
using System;

//设置鼠标选中优先级，数字越大优先级越高
public enum RAYQUERY_LEVEL
{
    RL_UNKNOWN = 0,

    RL_ITEMBOX = 1,		//掉落包		1
    RL_PLAYER_DEADBODY = 2,		//玩家尸体		2
    RL_CREATURE = 3,		//NPC&Monster	3
    RL_TRIPPEROBJ = 4,		//资源			4
    RL_PLAYEROTHER = 5,		//玩家			5
    RL_PLAYERMYSLEF = 6,		//自己			6
    RL_PET = 7,		//宠物			7
    RL_CREATURE_DEADBODY = 8,		//怪物尸体		8
    RL_PLATFORM = 9,		//操作平台		9
};
namespace GFX
{
    public delegate bool OnAnimationEvent(string animName);
    public delegate void OnHitGroundedEvent();
	public class GfxObject
	{
        public virtual GFXObjectType getObjectType() { return GFXObjectType.NONE; }
		//更换装备接口
		public virtual void changePart(int part, string equipName){}
		//挂接物件
		public virtual void attachActor(string locatorName, string actorName){}
		//挂接特效,会替换该挂接点下已经存在的特效
		public virtual void attachEffect(string locatorName, string effectName){}
        //在某个挂接点位置播放特效
        public virtual void PlayEffect(string locatorName, string effectName){}

		public virtual void changeRightWeapon(string weaponName){}//更换右手武器,名字为空时去掉该武器
		public virtual void changeLeftWeapon(string weaponName){}//更换左手武器

		public virtual void setBodyEffect(string effectName){} //身体装备特效
		public virtual void setShoulderEffect(string effectName){} //肩胛装备特效
       

        public virtual void SetRightWeaponEffect(string effectName){}//设定右手武器特效	
		public virtual void SetLeftWeaponEffect(string effectName){}//设定左手武器特效

        public virtual void SetVisible(bool bVisible) 
        {
            mVisible = bVisible;
            GfxUtility.setGameObjectVisible(mGameObject, bVisible);
            foreach (KeyValuePair<PROJTEX_TYPE, GfxEffect> projValue in mProjectors)
            {
                projValue.Value.SetVisible(bVisible);
            }
        }

        //设置是否能被鼠标选中
        bool m_bRayQuery = true;
        public virtual void SetRayQuery(bool bQuery) { m_bRayQuery = bQuery; }
        public virtual bool GetRayQuery() { return m_bRayQuery;  }

	    public virtual void			RayQuery_SetLevel(RAYQUERY_LEVEL nLevel)	{}
        public virtual RAYQUERY_LEVEL RayQuery_GetLevel() { return RAYQUERY_LEVEL.RL_UNKNOWN; }

        protected int objID = -1;
        public void SetData(int id)
        {
            objID = id;
        }
        protected GfxObject mParentObject;
        public GfxObject Parent
        {
            set
            {
                mParentObject = value;
            }
            get
            {
                return mParentObject;
            }
        }

	    // 新版附加一个特效，一个挂点下可挂接多个特效
        public virtual uint AddEffect(string effectName, string locatorName) { return 0; }
        public virtual void DelEffect(uint effect) { }
	 //   public virtual void			DelEffect( string szLocatorName)					{}
    //    public virtual void DelAllEffect() { }
	    //附加一个投射纹理
	    public enum PROJTEX_TYPE
	    {
		    PROJTEX_NONE = -1,  //根据名字创建
		    SELECT_RING,		//选择环
		    SHADOW_BLOB,		//阴影点
		    REACHABLE,			//可以到达的目的点
		    UNREACHABLE,		//无法到达的目的点
		    AURA_RUNE,			//技能作用范围环
		    MOVE_TRACK,			// 移动轨迹 [9/5/2011 Sun]
	    };
        protected Dictionary<PROJTEX_TYPE, GfxEffect> mProjectors = new Dictionary<PROJTEX_TYPE, GfxEffect>();
        //给该对象增加一个贴花
	    public virtual void	Attach_ProjTexture(PROJTEX_TYPE type, bool bShow, float Ring_Range, float fHeight, string szTextureName )
        {
            if (mGameObject == null) return;
            GFX.GfxEffect effect = null;
            if( !mProjectors.ContainsKey(type))
            {
                if (bShow == false) return;
                _DBC_EFFECT pProjector = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_EFFECT>((int)DataBaseStruct.DBC_EFFECT).Search_Index_EQU((int)type);
                if (pProjector.effectName.Length > 0 )
                {
                    effect = (GFX.GfxEffect)GFX.GFXObjectManager.Instance.createObject(pProjector.effectName, GFX.GFXObjectType.EFFECT);
                    mProjectors.Add(type, effect);
                    mObjectOutOfDate = true;
                    mAttachProjectorOutOfDate = true;
                }
            }
             effect = mProjectors[type];
//             effect.SetVisible(bShow);
            if (bShow)
            {                
				effect.setProjectorEffectSize(Ring_Range);//设置贴花大小
                Vector3 pos = effect.position;
                pos.y = fHeight;
                effect.position = pos; 
            }
            else
            {
                mProjectors.Remove(type);
                GFXObjectManager.Instance.DestroyObject(effect);//删除贴花
            }
        }
	    public virtual void	Attach_Object(GfxObject pObject, string szAttachLocator)	{	}
	    public virtual void	Detach_Object(GfxObject pObject)	{	}

        //取得角色模型上的某关键点位置
        public virtual void  GetLocator(string locatorName, ref Vector3 locPos){}

        //取得"头顶状态点"在屏幕上的位置,如果返回FALSE，表示在屏幕之外,或者没有该点
        public virtual bool GetInfoBoardPos(ref Vector3 ivPos, ref Vector3 pvObjPos, float fObligeHeight) { return false; }
        public virtual bool GetInfoBoardPos(ref Vector3 ivPos, ref Vector3 pvObjPos) { return false; }
	    //进入招式
	    // bAnim		- TRUE(调用动作)/FALSE(调用招式)
	    // szSkillName	- 动作名或者招式名
	    // bLoop		- 是否循环
	    // fFuseParam	- 骨骼动画融合参数 
	    public virtual void	 EnterSkill(bool bAnim, string szSkillName, bool bLoop, float fFuseParam) {}
	    public virtual void	 SetHairColor(uint HairColor){}
	    // 改变是否循环 [8/15/2011 ivan edit]
	    public virtual void	 ChangeActionLoop(bool bLoop){}
	    // 切换动画的播放速度
	    // fRate		- 缩放比率
	    public virtual void	 ChangeActionRate(float fRate) {}
	    //设置缺省动作
	    public virtual void	 SetDefaultAnim(string szAnimName) {}
	    //设置UI VisibleFlag
	    public virtual void	 SetUIVisibleFlag() {}
	    //设置透明度
        float mFTransparency = 0;
        Dictionary<Renderer, List<Shader>> mOrigShader = new Dictionary<Renderer,List<Shader>>();
	    public virtual void	 SetTransparent(float fTransparency, float fTime)
        {
            if (mGameObject == null)
            {
                return;
            }
            mFTransparency = fTransparency;

            //暂时忽略fTime，直接半透明
            Renderer[] allRenderer = mGameObject.GetComponentsInChildren<Renderer>();
            
            foreach (Renderer r in allRenderer)
            {
                if (fTransparency > 0.01f)//大于0.01设置透明
                {
                    List<Shader> shaders = new List<Shader>();
                    if (r.materials != null)
                    {
                        foreach (Material m in r.materials)
                        {
                            if (m != null)
                            {
                                shaders.Add(m.shader);//保存初始的shader
                                m.shader = Shader.Find("Transparent/Diffuse");
                                m.color = new Color(m.color.r, m.color.g, m.color.b, 1-fTransparency);
                            }
                        }
                    }
                    if (!mOrigShader.ContainsKey(r))
                    {
                        mOrigShader.Add(r, shaders);
                    } 
                }
                else//设为不透明
                {
                    if (r.materials != null)
                    {
                        for(int i = 0; i < r.materials.Length; ++i)
                        {
                            Material m = r.materials[i];
                            if (m != null && mOrigShader.ContainsKey(r))
                            {
                                m.shader = (mOrigShader[r])[i];
                                m.color = new Color(m.color.r, m.color.g, m.color.b, 1);
                            }
                        }
                    }
                }
            }
        }
	    // 设置连线特效的目标点
	    public virtual void	 SetEffectExtraTransformInfos(uint Effect, ref Vector3 fvPosition) {}
	    //设置是否出于鼠标Hover状态
        public virtual void  SetMouseHover(bool bHover){}
// 	    //设置角色动画结束通知
        public virtual void SetAnimationEndEvent(OnAnimationEvent pFunc) { }
	    //设置角色动画可以结束通知
        public virtual void SetAnimationCanBreakEvent(OnAnimationEvent pFunc) { }
	    //设置角色动画打击点通知
        public virtual void SetAnimationHitEvent(OnAnimationEvent pFunc) { }

        public virtual void SetAnimationShakeEvent(OnAnimationEvent pFunc) { }

        public virtual void SetHitGroundedEvent(OnHitGroundedEvent pFunc) { }

        //通知角色碰撞到地面
        public virtual void OnHitGroundEvent() { }

        public GameObject getGameObject()
        {
            return mGameObject;
        }
        protected bool mVisible = true;
		//位置朝向和旋转渲染属性
        protected Vector3 mPosition = Vector3.zero;
        protected Vector3 mLocalPosition = Vector3.zero;
		public Vector3 position
		{
			set
			{
                if( mParentObject == null )//被挂接的时候位置由父对象管理
                {
                    mPosition = value;
                    updateTranform();
                }
                if ( mTempObject != null )//临时对象的位置
                {
                    mTempObject.transform.position = value;
                }
			}
			get
			{
                return mPosition;
			}
		}
        protected Vector3 mScale = Vector3.one;
        protected Vector3 mLocScale = Vector3.one;
		public Vector3 scale
		{
			set
			{
                if (mParentObject == null)//被挂接的时候缩放由父对象管理
                {
                    mScale = value;
                    updateTranform();
                }

			}
			get
			{
                return mScale;
			}
		}
        protected Quaternion mRotation = Quaternion.identity;
        protected Quaternion mLocRotation = Quaternion.identity;
		public Quaternion rotation
		{
			set
			{
                if (mParentObject == null)//被挂接的时候旋转由父对象管理
                {
                    mRotation = value;
                    updateTranform();
                }
			}
			get
			{
                return mRotation;
			}
		}
        void updateTranform()
        {
            if(mGameObject == null) return;
           
            Vector3 worldPos = mLocalPosition;
            worldPos.Scale(mScale);
            worldPos = mRotation * worldPos + mPosition;
            mGameObject.transform.localPosition = worldPos;
            Vector3 scale = mScale;
            scale.Scale(mLocScale);
            mGameObject.transform.localScale = scale;
            mGameObject.transform.localRotation = mLocRotation * mRotation;
        }
		protected GameObject mGameObject;
        protected GameObject mTempObject;
        protected string  mTempAssetName = null;
		protected bool		 mObjectOutOfDate = true;
        protected bool mGameObjectOutOfDate = true;
        protected bool mAttachProjectorOutOfDate = false;
        protected AssetBundleManager.AssetRequest mObjectRequest; //gfxObject资源请求
        protected AssetBundleRequest mAssetLoadRequest; //资源加载请求
		protected bool isObjectAssetBundleReady()//检查对象资源下载请求是否完成
		{
            if (mObjectRequest != null && !mObjectRequest.isDone) 
                return false;
       
            return true;
		}
		
		public  virtual void update()//每帧检测是否需要更新
		{
			//updateObject();
            updateObjectAsset();
            updateProjectors();
            if (!mGameObjectOutOfDate && !mAttachProjectorOutOfDate)
                mObjectOutOfDate = false;
		}
        protected void downloadDelegate(System.Object obj, AssetBundle asset)
        {
            _resetObject();
            if (asset == null) return;
            mAssetLoadRequest = asset.LoadAsync((string)obj, typeof(GameObject));

        }
        void updateObjectAsset()
        {
            if (mAssetLoadRequest != null && mAssetLoadRequest.isDone)
            {
                if (mAssetLoadRequest.asset == null)
                    throw new Exception("asset load failed");
                mGameObject = GfxGameObjectManager.Instance.Instantiate(mAssetLoadRequest.asset);
                mLocalPosition = mGameObject.transform.localPosition;//
                mLocScale = mGameObject.transform.localScale;
                mLocRotation = mGameObject.transform.localRotation;

                mAssetLoadRequest = null;

                afterUpdateObject();
                mGameObjectOutOfDate = false;
            }
        }
        void updateProjectors()
        {
            if (mAttachProjectorOutOfDate && mGameObject != null)
            {
                bool allAttached = true;
                foreach (KeyValuePair<PROJTEX_TYPE, GfxEffect> effect in mProjectors)
                {
                    if (effect.Value.getGameObject())
                    {
                        if (effect.Value.getGameObject().transform.parent == null)
                            GFX.GfxUtility.attachGameObject(effect.Value.getGameObject(), mGameObject);
                    }
                    else
                    {
                        allAttached = false;
                    }
                }

                mAttachProjectorOutOfDate = !allAttached;
            }
        }
		public  void updateObject()
		{
			if( mObjectOutOfDate)
			{
                if (mGameObjectOutOfDate && isObjectAssetBundleReady())
                {
                    _resetObject();//先删除对象
                    if (mObjectRequest != null && mObjectRequest.mSuccess)
                    {
						//Object[] allAsset = mObjectRequest.mWWW.assetBundle.LoadAll();
                        if (mObjectRequest.mWWW == null || mObjectRequest.mWWW.assetBundle == null)
                        {
                            LogManager.LogError("WWW is null in " + mObjectRequest.mOjbectName);
                            return;
                        }
                        UnityEngine.Object asset = mObjectRequest.mWWW.assetBundle.Load(mObjectRequest.mOjbectName, typeof(GameObject));
                        if (!asset)
                        {
                            LogManager.LogError("asset is null in " + mObjectRequest.mOjbectName);
                            return;
                        }
                        //mGameObject = (GameObject)Object.Instantiate(asset);//创建新的gfxObject
                        mGameObject = GfxGameObjectManager.Instance.Instantiate(asset);
                        mLocalPosition = mGameObject.transform.localPosition;//
                        mLocScale = mGameObject.transform.localScale;
                        mLocRotation = mGameObject.transform.localRotation;
                    }
                    mGameObjectOutOfDate = false;
                    afterUpdateObject();
                }
                if(mAttachProjectorOutOfDate && mGameObject != null)
                {
                    bool allAttached = true;
                    foreach (KeyValuePair<PROJTEX_TYPE, GfxEffect> effect in mProjectors)
                    {
                        if (effect.Value.getGameObject())
                        {
                            if (effect.Value.getGameObject().transform.parent == null)
                                GFX.GfxUtility.attachGameObject(effect.Value.getGameObject(), mGameObject);
                        }
                        else
                        {
                            allAttached = false;
                        }
                    }

                    mAttachProjectorOutOfDate = !allAttached;
                }
                if (mAttachProjectorOutOfDate==false && mGameObjectOutOfDate == false)
                {
                    mObjectOutOfDate = false;
                }
			}
		}
		protected virtual void afterUpdateObject()
        {
            if(mGameObject != null)
            {
                updateTranform();
                SetTransparent(mFTransparency, 0);
                SetVisible(mVisible);
            }
        }
		protected void _resetObject()
		{
			if(mGameObject)
			{
				mGameObject.transform.parent = null;
                GfxGameObjectManager.Instance.Destroy(mGameObject);
				mGameObject = null;
			}
			mGameObjectOutOfDate = true;
		}
		public virtual void destroy()// destroy gfxObject
		{
            //删除贴花
            foreach (KeyValuePair<PROJTEX_TYPE, GfxEffect> projValue in mProjectors)
            {
                GFXObjectManager.Instance.DestroyObject(projValue.Value);
            }
            mProjectors.Clear();
			if(mGameObject != null)
			{
                GfxGameObjectManager.Instance.Destroy(mGameObject);
				mGameObject = null;
			}
            if(mTempObject != null)
            {
                GfxGameObjectManager.Instance.Destroy(mTempObject);
                mTempObject = null;
            }
		}
        public virtual void useTempAsset()//当资源没有下载完成时，临时代替的资源
        {
            mTempAssetName = "Asset/TempBodyAsset"; 
        }

        public static string ActorRoot
        {
            get
            {
                return "Build/Actors/";
            }
        }
        public static string EquipRoot
        {
            get { return "Build/Equips/"; }
        }
        protected static string assetbundleExtName = ".assetbundle";
	}
}

