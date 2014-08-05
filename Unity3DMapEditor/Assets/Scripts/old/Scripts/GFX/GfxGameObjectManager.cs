using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GFX
{
	public class GfxGameObjectManager //GO管理器，避免重复初始化GO[2012/2/9 ZZY]
	{
        static readonly GfxGameObjectManager sInstance = new GfxGameObjectManager();//唯一的实例

        static public GfxGameObjectManager Instance
        {
            get
            {
                return sInstance;
            }
        }

        public GameObject Instantiate(Object asset)
        {
//             if (asset == null) return null;
//             string assetName = asset.name;
//             foreach (GameObject go in mCachedGameObjects)
//             {
//                 if (go.name == assetName)
//                 {
//                     mCachedGameObjects.Remove(go);
//                     GfxUtility.setGameObjectVisible(go, true);
// 					if(asset is GameObject)
// 					{
// 						GameObject assetGo = (GameObject)asset;
// 						go.transform.localPosition = assetGo.transform.localPosition;
// 						go.transform.localScale = assetGo.transform.localScale;
// 						go.transform.localRotation = assetGo.transform.localRotation;
// 					}
//                     return go;
//                 }
//             }
//             GameObject theGo = (GameObject)Object.Instantiate(asset);//没有发现，需要创建新的GO
//             theGo.name = assetName;//新的Go名字和资产名字相同
            if (asset == null) return null;
            GameObject theGo = (GameObject)Object.Instantiate(asset);
            return theGo;
        }
        public void Destroy(GameObject go)
        {
//             GfxUtility.setGameObjectVisible(go, false);
//             go.transform.parent = null;
//             mCachedGameObjects.Add(go);
               if(Application.isEditor)
               {
                   Object.DestroyImmediate(go);
               }
               else
               {
                   Object.Destroy(go);
               }
              
        }
        public void clean()//切换场景需要清理存储的go，减少内存占用
        {
            foreach (GameObject go in mCachedGameObjects)
            {
                Object.Destroy(go);
            }
            mCachedGameObjects.Clear();
        }
        protected List<GameObject> mCachedGameObjects = new List<GameObject>();
	}
}