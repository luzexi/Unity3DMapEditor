using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
//创建玩家角色资源包 [2011/12/14 ZZY]

class AssetbundlesCreatePlayerActor
{
    [MenuItem("SG Tools/Create Asset bundles/Player Actor")] 
    static void Execute()
    {
		AssertBundleCreator.createObject(true);
    }
}
//创建怪物NPC角色资源包 [2011/12/14 ZZY]
class AssetbundlesCreateNPCActor
{
    [MenuItem("SG Tools/Create Asset bundles/Actor")]
    static void Execute()
    {
  		AssertBundleCreator.createObject(false);
    }
}
//创建玩家装备资源包 [2011/12/14 ZZY]
class AssetbundlesCreatePlayerEquipFromAsset
{
    [MenuItem("SG Tools/Create Asset bundles/Equip")]
    static void Execute()
    {
 		AssertBundleCreator.createEquip();
    }
}

class AssetbundlesBuildAll
{
    [MenuItem("SG Tools/Create Asset bundles/Build All")]
    static void Execute()
    {
        buildAllPrefab("Assets/Res/Obj/Char/Prefab/NPC/", false);
        buildAllPrefab("Assets/Res/Obj/Char/Prefab/Player/", true);
        buildAllPrefab("Assets/Res/Obj/Char/Prefab/Weapon/", false);

        buildAllPrefab("Assets/Res/Obj/Effect/Prefab/", false);
        buildAllPrefab("Assets/Res/Obj/Item/Prefab/", false);
        //buildAllPrefab("Assets/Res/Obj/Effect/Prefab/Projector Prefab/", false);

        buildAllPlayerEquip("Assets/Res/Obj/Char/MPC/");
    }
    protected static  void buildAllPrefab(string path, bool isPlayer)
    {
        string npcPath =path;
        DirectoryInfo NpcInfo = new DirectoryInfo(npcPath);
        if (NpcInfo == null) return;
        FileInfo[] npcFiles = NpcInfo.GetFiles();
        if (npcFiles == null) return;
        foreach (FileInfo f in npcFiles)
        {
            string lowercaseName = f.Name.ToLower();
            if (lowercaseName.Contains(".prefab") && (!lowercaseName.Contains(".meta")))
            {
                string assetPath = npcPath + f.Name;
                Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                AssertBundleCreator._createObject(asset, isPlayer);
            }
        }
    } 
    protected static void buildAllPlayerEquip(string path)
    {
        string equipPath = path;
        DirectoryInfo equipInfo = new DirectoryInfo(equipPath);
        if (equipInfo == null) return;
        FileInfo[] equipFiles = equipInfo.GetFiles();
        if (equipFiles == null) return;
        foreach (FileInfo f in equipFiles)
        {
            string lowercaseName = f.Name.ToLower();
            if (lowercaseName.Contains(".fbx") && (!lowercaseName.Contains(".meta")))
            {
                string assetPath = equipPath + f.Name;
                Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                AssertBundleCreator._createEquip(asset, true);
            }
        }
    }
}
public class AssertBundleCreator
{
    public static string ActorAssetbundlePath
    {
        get { return "Build/Actors/"; }
    }
	public static string EquipAssetbundlePath
    {
        get { return "Build/Equips/"; }
    }

    public static string SkillAssetbundlePath
    {
        get { return "Assets/Resources/Skill/"; }
    }
	public static void createObject(bool isPlayer)//使用当前选择目录下的game object创建Object资源包 [2011/12/14 ZZY]
	{
		bool createdBundle = false;
        foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets))
        {
            if (!(o is GameObject)) continue;
            if (o.name.Contains("@")) continue;
			_createObject(o, isPlayer);
			createdBundle = true;
   		 }
		foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.ExcludePrefab))
        {
            if (!(o is GameObject)) continue;
            if (o.name.Contains("@")) continue;
			_createObject(o, isPlayer);
			createdBundle = true;
		}
		if (!createdBundle)
            EditorUtility.DisplayDialog("Object", "none assetbundle create", "Ok");
	}
	public static void _createObject(Object o, bool isPlayer)
	{
		    GameObject characterFBX = (GameObject)o;

            string name = characterFBX.name.ToLower();
           
            Debug.Log("******* Creating assetbundles for: " + name + " *******");

            // Create a directory to store the generated assetbundles.
            if (!Directory.Exists(ActorAssetbundlePath))
                Directory.CreateDirectory(ActorAssetbundlePath);

            GameObject characterClone = (GameObject)Object.Instantiate(characterFBX);
			if(isPlayer)//如果要创建玩家object,需要删除所有mesh
			{
            	foreach (SkinnedMeshRenderer smr in characterClone.GetComponentsInChildren<SkinnedMeshRenderer>())
                	Object.DestroyImmediate(smr.gameObject);
				characterClone.AddComponent<SkinnedMeshRenderer>();//加入一个空的SkinnedMeshRenderer用于换装
			}
            string path = ActorAssetbundlePath + name + ".assetbundle";
			Object characterBasePrefab = GetPrefab(characterClone, name);
			BuildPipeline.BuildAssetBundle(characterBasePrefab, null, path, BuildAssetBundleOptions.CollectDependencies);
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(characterBasePrefab));
	}
	public static void createEquip()//使用当前选择目录下的game object的对象创建装备资源包 [2011/12/14 ZZY]
	{
		bool createdBundle = false;
        foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets))
        {
            if (!(o is GameObject)) continue;
            if (o.name.Contains("@")) continue;
			_createEquip(o, true);
			createdBundle = true;
		}
		foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.ExcludePrefab))
        {
            if (!(o is GameObject)) continue;
            if (o.name.Contains("@")) continue;
			_createEquip(o, false);
			createdBundle = true;
		}
		if (!createdBundle)
            EditorUtility.DisplayDialog("equip", "none assetbundle create", "Ok");
	}
	public static void _createEquip(Object o, bool fromAsset)
	{
		GameObject characterFBX = (GameObject)o;
			// Create assetbundles for each SkinnedMeshRenderer.
	        foreach (SkinnedMeshRenderer smr in characterFBX.GetComponentsInChildren<SkinnedMeshRenderer>(true))
	        {
                List<Object> toinclude = new List<Object>();
				string bundleName = smr.gameObject.name.ToLower();/*smr.name.ToLower()*/;
                // Save the current SkinnedMeshRenderer as a prefab so it can be included
                // in the assetbundle. As instantiating part of an fbx results in the
                // entire fbx being instantiated, we have to dispose of the entire instance
                // after we detach the SkinnedMeshRenderer in question.
				GameObject rendererClone ;
				if(fromAsset)//从资源转换
				{
					rendererClone= (GameObject)EditorUtility.InstantiatePrefab(smr.gameObject);
	                GameObject rendererParent = rendererClone.transform.parent.gameObject;
	                rendererClone.transform.parent = null;
	                Object.DestroyImmediate(rendererParent);
				}
				else
				{
					rendererClone = (GameObject)Object.Instantiate(smr.gameObject);
					if(rendererClone.transform.parent)
					{
						GameObject rendererParent = rendererClone.transform.parent.gameObject;
	                	rendererClone.transform.parent = null;
	                	Object.DestroyImmediate(rendererParent);
					}
				}

                Object rendererPrefab = GetPrefab(rendererClone, bundleName);
                toinclude.Add(rendererPrefab);

                // Save the assetbundle.
                List<string> boneNames = new List<string>();
                foreach (Transform t in smr.bones)
                    boneNames.Add(t.name);
                string stringholderpath = "Assets/bonenames.asset";
				
				StringHolder holder = ScriptableObject.CreateInstance<StringHolder> ();
				holder.content = boneNames.ToArray();
                AssetDatabase.CreateAsset(holder, stringholderpath);
                toinclude.Add(AssetDatabase.LoadAssetAtPath(stringholderpath, typeof (StringHolder)));
				
				// Create a directory to store the generated assetbundles.
        		if (!Directory.Exists(EquipAssetbundlePath))
           			 Directory.CreateDirectory(EquipAssetbundlePath);
			
                string path = EquipAssetbundlePath + bundleName + ".assetbundle";
                BuildPipeline.BuildAssetBundle(null, toinclude.ToArray(), path, BuildAssetBundleOptions.CollectDependencies);
                Debug.Log("Saved " + bundleName);
                // Delete temp assets.
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(rendererPrefab));
				AssetDatabase.DeleteAsset(stringholderpath);
		}
	}
	static Object GetPrefab(GameObject go, string name)
    {
        Object tempPrefab = EditorUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
        tempPrefab = EditorUtility.ReplacePrefab(go, tempPrefab);
        Object.DestroyImmediate(go);
        return tempPrefab;
    }
}
