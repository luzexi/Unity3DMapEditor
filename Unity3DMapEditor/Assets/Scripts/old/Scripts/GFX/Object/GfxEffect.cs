using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundleManager;
namespace GFX
{
	public class GfxEffect:GfxObject //特效
	{
		public GfxEffect(string effectName)
		{
            //mObjectRequest = AssetBundleManager.AssetBundleRequestManager.getActorAsset(effectName);
            string filePath = GfxObject.ActorRoot + effectName + GfxObject.assetbundleExtName;
            ResourceManager.Me.DownloadAsync(filePath, effectName, base.downloadDelegate, AskType.Normal, ResourceType.RT_Actor);
		}

        public override GFXObjectType getObjectType() { return GFXObjectType.EFFECT; }
        protected float mProjSize = 1.0f;//正交投影特效的大小
        public void setProjectorEffectSize(float size)
        {
            mProjSize = size;
            if(mGameObject != null)
            {
                Projector theProj = (Projector)mGameObject.GetComponent(typeof(Projector));
                if(theProj &&  theProj.isOrthoGraphic)
                {
                    theProj.orthoGraphicSize = mProjSize;//投影的大小不能通过transform改变，只能设置投影窗口的大小
                }
            }
        }
        protected override void afterUpdateObject() 
        {
            base.afterUpdateObject();
            setProjectorEffectSize(mProjSize);
            if (mGameObject != null)
            {
                Projector theProj = (Projector)mGameObject.GetComponent(typeof(Projector));
                if (theProj)
                {
                    theProj.ignoreLayers = ~LayerManager.GroundMask;
                }

                //查看是否有EffectScript脚本
                EffectScript lifeScript = (EffectScript)mGameObject.GetComponent("EffectScript");
                if (lifeScript)
                {
                    mHasLife = lifeScript.UseLifeTime;
                    mLifeTime = lifeScript.LifeTime;
                }
            }
        }
        public override void update()//每帧检测是否需要更新
        {
            base.update();
            mElapsedTime += Time.deltaTime;
            if(isDead())
            {
                GFXObjectManager.Instance.DestroyObject(this);
            }
        }

        float mElapsedTime = 0.0f;

		protected  bool isDead()//特效是否死亡
		{
            if (mHasLife&& mElapsedTime > mLifeTime)
            {
                mElapsedTime = 0.0f;
                return true;
            }
            else
                return false;
            
		}
        bool mHasLife = false;//是否有生命期
        float mLifeTime = 1.0f;
        public bool  hasLifeTime()
        {
            return mHasLife;
        }
        public float getLifeTime()//返回生命期
        {
            return mLifeTime;
        }
	}
}

