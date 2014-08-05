using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace AssetBundleManager
{
    public class  AssetRequest
    {
        public string mOjbectName;//www里边的对象名字
        public WWW mWWW;
        public bool mSuccess;//是否成功下载
        public bool isDone//是否下载完成
        {
            get{
				if(mWWW.isDone==false)
                {
                    return  false;
				}
				else
				{
					if(mWWW.error != null)
					{
                        LogManager.LogError(mWWW.error);
                        mSuccess = false;//缺少资源，下载失败
                        return true;
					}
					else
                    {
                         mSuccess = true;
                         return true;//完成下载
                    }
                       
				}
            }
        }
    }
    public enum AssetType
    {
        ACTOR,
        EFFECT,
        EQUIP
    }
    public class AssetBundleRequestManager
    {
        protected static string AssetbundleBaseURL//资源包所在URL/文件路径
        {
            get
            {
                if (Application.isWebPlayer)
                    return Application.dataPath + "/Build/";
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                    return "http://192.168.1.173:8080/Build/";
                else
                    return "file://" + Application.dataPath + "/../Build/";
            }
        }
        protected static string actorPath = "Actors/"; //Actors目录
		protected static string effectPath = "Effects/"; //effects目录
        protected static string equipPath = "Equips/";//equip 目录
        protected static string assetbundleExtName = ".assetbundle";
		protected static int     AssetBundleVersion = 1;
        public static string ActorAssetbundleBasePath
        {
            get
            {
                string path = AssetbundleBaseURL;
                return path + actorPath;
            }
        }
        public static string EquipAssetbundleBasePath
        {
            get
            {
                string path = AssetbundleBaseURL;
                return path + equipPath;
            }
        }
        public static string EffectAssetbundleBasePath
        {
            get
            {
                string path = AssetbundleBaseURL;
                return path + effectPath;
            }
        }
        static Dictionary<string, WWW> WWWMap = new Dictionary<string, WWW>();
        protected static AssetRequest getObjectAssetBundleRequest(string objectName, string objPath)//返回角色资源包请求
        {
            if (!WWWMap.ContainsKey(objectName))
            {
                string url = objPath;
                url = url + objectName + assetbundleExtName;
                WWW www;
                if (/*Application.isWebPlayer*/false)
				{
					www = WWW.LoadFromCacheOrDownload(url, AssetBundleVersion);
				}
				else
				{
					www = new WWW(url);
				}
				
                WWWMap.Add(objectName, www);
            }
           
            AssetRequest assetRequest = new AssetRequest();
            assetRequest.mOjbectName = objectName;
            assetRequest.mWWW =  WWWMap[objectName];
            return assetRequest;
        }

        public static AssetRequest getActorAsset(string actorName)//返回角色资源包请求
        {
            return getObjectAssetBundleRequest(actorName, ActorAssetbundleBasePath);
        }

        public static AssetRequest getEffectAsset(string effectName)//返回特效资源包请求
        {
            return getObjectAssetBundleRequest(effectName, EffectAssetbundleBasePath);
        }

        public static AssetRequest getEquipAsset(string equipName)//返回装备和装备骨骼资源包请求
        {
            return getObjectAssetBundleRequest(equipName, EquipAssetbundleBasePath);
        }

        public static void releaseAssetBundles()//释放一些www对象
        {
            const int MaxNumAsset = 200;//最多保留200个www对象
            System.Random r = new System.Random((int)Time.realtimeSinceStartup);
            while (WWWMap.Keys.Count > MaxNumAsset)
            {
                int rNum = r.Next();//随机释放www
                int a = rNum / WWWMap.Keys.Count;
                int n = rNum - a * WWWMap.Keys.Count;
                int i = 0;
                foreach (string k in WWWMap.Keys)
                {
                    if(i == n)
                    {
                        WWW www = WWWMap[k];
                        www.assetBundle.Unload(true);
                        WWWMap.Remove(k);
                        break;
                    }
                    i++; 
                }
            }

        }
    }


}

