using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


//	MapEditor.cs
//	Author: Lu Zexi
//	2014-07-24


/// <summary>
/// Map editor.
/// </summary>
public class MapEditor : Editor
{
	private const string EXPORT_TAG = "Map";
	private const string PrefabsPath = "Assets/Temps/";
	private const int EXPORT_OBJECT_NUM = 1;
	private const string EXPORT_PATH = "Assets/Build/Maps/";

	[MenuItem("Map/Export")]
	static void ExportAssets()
	{
		Debug.Log("**************init begin*****************");
		List<MapNodeEx> lstNode = new List<MapNodeEx>();
		GameObject[] gos = GameObject.FindGameObjectsWithTag(EXPORT_TAG);
		string[] scenePath = EditorApplication.currentScene.Split('/');
		string[] sceneNames = scenePath[scenePath.Length - 1].Split('.');
		string SceneName = sceneNames[0];
		string localdir = PrefabsPath + SceneName + "/Prefabs/";
		string buildDIR = EXPORT_PATH + SceneName + "/";

		if(!Directory.Exists(localdir))
			Directory.CreateDirectory(localdir);
		if(!Directory.Exists(buildDIR))
			Directory.CreateDirectory(buildDIR);

		List<MapAssets> lstAsset = MapSerializerTool.JsonToAssetList(EXPORT_PATH + "asset.json");
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
		Debug.Log("**************init end*****************");
		
		Debug.Log("**************Build Prefab begin*****************");
		foreach (GameObject go in gos)
		{
			string file = localdir + go.name.Replace("(Clone)", "") + ".prefab";
			CreatePrefab(go, file);

			MapNodeEx node = new MapNodeEx();
			node.name = go.name.Replace("(Clone)", "");
			node.assetPath = file;
			node.textureBundle = "none";
			node.assetBundle = "none";
			node.position = go.transform.position;
			node.rotation = go.transform.rotation;
			node.scale = go.transform.localScale;

			lstNode.Add(node);
		}
		Debug.Log("**************Build Prefab End*****************");

		Debug.Log("**************AssetBundle Build begin*****************");
		int asset_index = 0;
		List<UnityEngine.Object> lstObj = new List<UnityEngine.Object>();
		for( int i = 0 ; i<lstNode.Count ; i++ )
		{
			MapNodeEx node = lstNode[i];
			string assetBundlePath = SceneName + asset_index + ".o";
			node.assetBundle = assetBundlePath;
			UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(node.assetPath);
			///// check the resource is not already export ////
			bool existNode = false;
			for( int k = 0 ; k<lstAsset.Count && !existNode ; k++ )
			{
				MapAssets mapAsset = lstAsset[k];
				foreach(string objName in mapAsset.Objects )
				{
					if(objName == asset.name)
					{
						existNode = true;
						node.assetBundle = mapAsset.Name;
						break;
					}
				}
			}

			// if the asset if not already export , then it can export.
			if( !existNode && !lstObj.Contains(asset) )
				lstObj.Add(asset);

			if(lstObj.Count >= EXPORT_OBJECT_NUM || i >= (lstNode.Count-1) )
			{
				BuildPipeline.PushAssetDependencies();
				BuildPipeline.BuildAssetBundle(null , lstObj.ToArray() , Application.dataPath + "/../" + buildDIR + assetBundlePath , BuildAssetBundleOptions.CompleteAssets|BuildAssetBundleOptions.CollectDependencies);
				BuildPipeline.PopAssetDependencies();
				MapAssets mapAsset = new MapAssets();
				mapAsset.Name = assetBundlePath;
				foreach(UnityEngine.Object item in lstObj)
				{
					mapAsset.Objects.Add(item.name);
				}
				lstAsset.Add(mapAsset);
				lstObj.Clear();
				asset_index++;
			}
		}
		Debug.Log("**************AssetBundle Build end*****************");

		Debug.Log("**************Map infomation Build begin*****************");
		Map map = new Map();
		map.name = SceneName;
		map.objects = new List<MapNode>();
		foreach( MapNodeEx item in lstNode )
		{
			map.objects.Add(item.ToMapNode());
		}
		string mapInfoPath = buildDIR + "map.p";
		string mapInfoTxtPath = PrefabsPath + "map.txt";
		MapSerializerTool.SerializeObjectAndSave(map , mapInfoTxtPath);
		AssetDatabase.Refresh();
		UnityEngine.Object mapInfoObj = AssetDatabase.LoadAssetAtPath(mapInfoTxtPath , typeof(TextAsset));
		BuildPipeline.BuildAssetBundle(mapInfoObj , null , Application.dataPath + "/../" + mapInfoPath);
		Debug.Log("**************Map infomation Build end*****************");

		Debug.Log("**************Ending begin*****************");
		Directory.Delete(PrefabsPath,true);
		MapSerializerTool.SerializeObjectAndSave(lstAsset , EXPORT_PATH + "asset.json");
		AssetDatabase.Refresh();
		Debug.Log("**************Ending end*****************");

	}

	/// <summary>
	/// Creates the prefab.
	/// </summary>
	/// <returns>The prefab.</returns>
	/// <param name="go">Go.</param>
	/// <param name="file">File.</param>
	private static UnityEngine.Object CreatePrefab(GameObject go, string file)
	{
		UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
		if (!prefab)
		{
			GameObject clone = UnityEngine.Object.Instantiate(go) as GameObject;
			
			prefab = EditorUtility.CreateEmptyPrefab(file);
			
			EditorUtility.ReplacePrefab(clone, prefab);
			
			UnityEngine.Object.DestroyImmediate(clone);
			
			AssetDatabase.Refresh();
		}
		
		return prefab;
		
	}

}
