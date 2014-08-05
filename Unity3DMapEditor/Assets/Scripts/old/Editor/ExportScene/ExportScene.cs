using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class SceneNodeE {
	
	public string name;
	public string assetPath;
	public string assetBundle;
	public string textureBundle;
	public string textures;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
    public bool hasTexture = false;
}

public class Assets
{
    public string Name;
    public List<string> Objects;
}

public class ExportScene : ScriptableWizard {

    static string TagNameExport = "Export";
    string PrefabsPath = "Assets/Temps/";
    string exportPath = "Build/Maps/";
    string NeedExportTag = "Export";
    public int NumOfObjects = 5;
    public bool TextureAlone = false;
    public bool packUiAlone = true;
    public string onlyPackWinName = "";
    string SceneName;
    string uiCommonResPath = "Assets/Res/UI/UICommon/";
    string uiFontsResPath = "Assets/Res/UI/Fonts/";
    string uiIconResPath = "Assets/Res/UI/Icons/";
    string uiPrefabPath = "Assets/Res/UI/UIPrefab/";

//     string[] ExpOptions = { "Scene","UI"};
//     int opIndex = 0;
//     int lastIndex = 0;

//     void OnGUI()
//     {
//         opIndex = EditorGUI.Popup(
//                     new Rect(0, 0, position.width -20, 20),
//                     "ExportOption:",
//                     opIndex,
//                     ExpOptions);
//         if (opIndex != lastIndex)
//         {
//             if (opIndex == 0)
//             {
//                 exportPath = "Build/Maps/";
//                 NeedExportTag = "Export";
//                 NumOfObjects = 5;
//                 TextureAlone = false;
//             }
//             else
//             {
//                 exportPath = "Build/UI/";
//                 NeedExportTag = "UIExport";
//                 NumOfObjects = 1;
//                 TextureAlone = false;
//             }
//             lastIndex = opIndex;
//         }
// 
//         PrefabsPath = EditorGUI.TextField(new Rect(0, 30, position.width - 20, 20), "PrefabsPath:", PrefabsPath);
//         exportPath = EditorGUI.TextField(new Rect(0, 55, position.width - 20, 20), "exportPath:", exportPath);
//         NeedExportTag = EditorGUI.TextField(new Rect(0, 80, position.width - 20, 20), "NeedExportTag:", NeedExportTag);
//         if (opIndex == 0)
//         {
//             NumOfObjects = EditorGUI.IntField(new Rect(0, 105, position.width - 20, 20), "NumOfObjects:", NumOfObjects);
//             TextureAlone = EditorGUI.Toggle(new Rect(0, 130, position.width - 20, 20), "TextureAlone:", TextureAlone);
//             SceneName = EditorGUI.TextField(new Rect(0, 155, position.width - 20, 20), "SceneName:", SceneName);
// 
//             if (GUI.Button(new Rect(0, 180, position.width - 20, 20), "Export Scene"))
//                 OnWizardCreate();
//         }
//         else
//         {
//             uiCommonResPath = EditorGUI.TextField(new Rect(0, 105, position.width - 20, 20), "Common Res Path:", uiCommonResPath);
//             if (GUI.Button(new Rect(0, 130, position.width - 20, 20), "Export UI"))
//                 OnUIWizardCreate();
//         }
//     }

	List<SceneNodeE> PrefabsList;

    Scene mSceneClient;
	
	ImportAssetOptions IA_Option = ImportAssetOptions.Default;

    List<Assets> mAssets;
    List<Assets> mAssetsTexture;

    delegate void PrepareAsset(SceneNodeE node, List<Object> textures);
    delegate SceneNodeE CreateNewNode(GameObject go, string path);
    delegate void WriteChildsToSceneNode(XmlDocument doc, XmlElement root, SceneNodeE node, bool isChild);

    WriteChildsToSceneNode writeChildsToSceneNode;

    CreateNewNode createNewNode;

    PrepareAsset PrepareTextures;
    PrepareAsset PrepareObjects;

    int mAssetPackIndex = 0;
    int mTexturePackIndex = 0;

    //auto set tags
    [MenuItem("Tools/Set GameObject Tag: Export")]
    static void OnSetSelectTag()
    {
        Object[] objects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (GameObject go in objects)
        {
            go.tag = TagNameExport;
        }

        //save modify
        EditorApplication.SaveScene(EditorApplication.currentScene);
    }
    [MenuItem("Tools/Set GameObject Tag: Export", true)]
    static bool ValidateSelection()
    {
        return Selection.transforms.Length != 0;
    }
	
	[MenuItem("Assets/Export Assets from Tag")]
	static void ExportGameObject()
    {
		ScriptableWizard.DisplayWizard( "Select of All Tag", typeof(ExportScene),"打包场景","打包UI");
	}

    [MenuItem("Assets/Remove Shadow from UI Object")]
    static void RemoveShadowFromUI()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("UIExport");
        foreach (GameObject go in gos)
        {
            Renderer[] rs = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in rs)
            {
                render.castShadows = false;
                render.receiveShadows = false;
            }
        }
        //save modify
        EditorApplication.SaveScene(EditorApplication.currentScene);

    }
    [MenuItem("Assets/Remove Widgets Boxcollider")]
    static void RemoveBoxCollider()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("UIExport");
        foreach (GameObject go in gos)
        {
            BoxCollider[] bcs = go.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider bc in bcs)
            {
                GameObject.DestroyImmediate(bc);
            }
        }
        //save modify
        EditorApplication.SaveScene(EditorApplication.currentScene);
    }

    void InitUiVariables()
    {
        exportPath = "Build/UI/";
        NeedExportTag = "UIExport";
        NumOfObjects = 1;
        TextureAlone = false;
    }

    void OnWizardOtherButton()
    {
        InitUiVariables();

        OnUIWizardCreate();
    }

    void InitSceneVariables()
    {
        exportPath = "Build/Maps/";
        NeedExportTag = "Export";
        NumOfObjects = 5;
        TextureAlone = false;
    }

	void OnWizardCreate()
    {
        InitSceneVariables();

        //初始化容器
        InitializeExport();

        //读入已经存在的资源列表
        LoadAssetsInfo(exportPath + "Assets.json", exportPath + "Textures.json");
        
        //指定功能函数
        ExportResource();

        //开始导出
        OnExportAssets();

        //释放资源
        RealeaseExport();
	}
    void InitializeExport()
    {
        PrefabsList = new List<SceneNodeE>();
        mAssets     = new List<Assets>();
        mAssetsTexture = new List<Assets>();
    }
    void RealeaseExport()
    {
        PrefabsList = null;
        mAssets     = null;
        mAssetsTexture = null;
    }

    void ExportResource()
    {
        createNewNode = new CreateNewNode(CreateNew_v3);
        PrepareTextures = new PrepareAsset(PrepareTextureAsset_v3);
        PrepareObjects = new PrepareAsset(PrepareObjectAsset_v3);
    }
	
    void OnExportAssets()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(NeedExportTag);

        string[] scenePath = EditorApplication.currentScene.Split('/');
        string[] sceneName = scenePath[scenePath.Length - 1].Split('.');
        SceneName = sceneName[0];
        string localdir = PrefabsPath + SceneName + "/Prefabs/";
        Directory.CreateDirectory(localdir);
        //必须刷新
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        Debug.Log("**************Build Prefab begin*****************");
        foreach (GameObject go in gos)
        {
            SceneNodeE root = createNewNode(go, localdir);
            AddNodeToList(root);
        }
        Debug.Log("**************Build Prefab End*****************");
        SaveSceneNew_v3();
    }
    string GetAssetBundle(string file)
    {
        foreach (Assets asset in mAssets)
        {
            if (asset.Objects.Contains(file))
            {
                //Debug.Log("Be Found in old assetbundle:" + file);
                return asset.Name;
            }
        }
        //Debug.Log("not Found in old assetbundle:" + file);
        return "none";
    }
    string GetAssetBundleTexture(string file)
    {
        foreach (Assets asset in mAssetsTexture)
        {
            if (asset.Objects.Contains(file))
                return asset.Name;
        }
        return "none";
    }
    SceneNodeE CreateNew_v3(GameObject go, string path)
    {
        return CreateNew_v2(go, path);
    }
    SceneNodeE CreateNew_V4(GameObject go, string path)
    {
        SceneNodeE node = null;

        Renderer r = go.GetComponent<Renderer>();

        Renderer[] rs = go.GetComponentsInChildren<Renderer>();

        
        return node;
    }
   
    void SaveSceneNew_v3()
    {
        Directory.CreateDirectory(exportPath);

        Debug.Log("**************Build AssetBundle begin*****************");
        //打包游戏对象
        SaveSceneObject_v3();
        Debug.Log("**************Build AssetBundle End*****************");

        Debug.Log("**************Build Terrain Begin*****************");
        //打包地形
        SaveTerrain();
        Debug.Log("**************Build Terrain End*****************");

        Debug.Log("**************Save json file Begin*****************");
        //保存场景配置
        SaveToJson();
        Debug.Log("**************Save json file End*****************");

        //保存历史资源信息
        SaveAssetXML();
    }
    void SaveSceneObject_v3()
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;

        List<Object> objects = new List<Object>();
        List<Object> objectsTexture = new List<Object>();

        //game objects
        string localDir = exportPath + "AssetBundles/";

        //must create directory before buildAssetBundle
        Directory.CreateDirectory(localDir);

        BuildPipeline.PushAssetDependencies();


        mTexturePackIndex = 0;
        if (TextureAlone)
        {
            //先打包贴图
            for (int i = 0; i < PrefabsList.Count; i++)
            {
                SceneNodeE nodeE = PrefabsList[i];
                if (PrepareTextures != null)
                    PrepareTextures(nodeE, objectsTexture);
                if (!nodeE.hasTexture) continue;
                int texCount = objectsTexture.Count;
                string texBundle = exportPath + GetTexturePackName(SceneName);//localDir + SceneName + "_" + pack + ".t";

                if (nodeE.textureBundle == "none")
                {
                    nodeE.textureBundle = GetTexturePackName(SceneName);
                    //SetAssetBundle(nodeE.textures, nodeE.textureBundle, mAssetsTexture);
                }
				
                if (texCount >= NumOfObjects ||
                    (i == PrefabsList.Count - 1) && texCount > 0)
                {
                    BuildPipeline.BuildAssetBundle(null, objectsTexture.ToArray(), texBundle, options);
                    mTexturePackIndex++;

                    objectsTexture.Clear();
                }

            }
        }


        mAssetPackIndex = 0;
        //打包游戏对象
        for (int i = 0; i < PrefabsList.Count; i++)
        {
            SceneNodeE node = PrefabsList[i];

            if (PrepareObjects != null)
                PrepareObjects(node, objects);
            int objectCount = objects.Count;

            string Bundle = exportPath + GetAssetPackName(SceneName);//localDir + SceneName + "_" + pack + ".o";

            if (node.assetBundle == "none")
            {
                node.assetBundle = GetAssetPackName(SceneName);//"AssetBundles/" + SceneName + "_" + pack + ".o";
                //SetAssetBundle(node.name, node.assetBundle, mAssets);
            }

            if (objectCount >= NumOfObjects|| 
                (i == PrefabsList.Count - 1) && objectCount > 0)
            {
                BuildPipeline.PushAssetDependencies();
                BuildPipeline.BuildAssetBundle(null, objects.ToArray(), Bundle, options);
                BuildPipeline.PopAssetDependencies();

                mAssetPackIndex++;

                objects.Clear();

            }
        }
        BuildPipeline.PopAssetDependencies();

        string srcNavMeshPath = GetNavMeshPath();
        Object navObj = AssetDatabase.LoadMainAssetAtPath(srcNavMeshPath);
        if (navObj)
        {
            // 打包导航网格部分 [2/17/2012 Ivan]
            BuildPipeline.PushAssetDependencies();
            string tarPath = exportPath + SceneName + "/" + SceneName + ".NavMesh";
            Directory.CreateDirectory(exportPath + SceneName);
            BuildPipeline.BuildAssetBundle(navObj, null, tarPath, options);
            BuildPipeline.PopAssetDependencies();
        }

        DestroyAllPrefab();
    }

    private string GetNavMeshPath()
    {
        string allPath = EditorApplication.currentScene;
        string[] path = allPath.Split('/');
        string[] fileName = path[path.Length - 1].Split('.');
        allPath = allPath.Replace(path[path.Length - 1], fileName[0] + ".NavMesh.bytes");
        return allPath;
    }
    string GetAssetPackName(string sceneName)
    {
        return "AssetBundles/" + sceneName + "_" + mAssetPackIndex + ".o";
    }
    string GetTexturePackName(string sceneName)
    {
        return "AssetBundles/" + sceneName + "_" + mTexturePackIndex + ".t";
    }
    string GetUIPackName(string uiName)
    {
        return uiName + ".ui";
    }
   
    void DestroyAllPrefab()
    {
        foreach (SceneNodeE node in PrefabsList)
        {
            AssetDatabase.DeleteAsset(node.assetPath);
        }
    }
    void PrepareTextureAsset_v3(SceneNodeE node, List<Object> textures)
    {
        GameObject go = GameObject.Find(node.name);
        if (!go ) return;
	
        //material=texture,material0=texture0;render1()
        bool first = true;
        foreach (Renderer render in go.GetComponentsInChildren<Renderer>())
        {
            if(!first)
            {
                node.textures +=";";
            }
            for(int i = 0; i < render.sharedMaterials.Length; i++)
            {
                Texture tex = render.sharedMaterials[i].mainTexture;
                if (!tex) continue;
                string asset = GetAssetBundleTexture(tex.name);
                //if (first)
                //{
                //    node.textures = tex.name;
                //    first = false;
                //}
                //else
                //{
                //    node.textures += ";" + tex.name;
                //}
                if(first)
                    node.textures = render.sharedMaterials[i].name + "=" + tex.name;
                else
                    node.textures += "," + render.sharedMaterials[i].name + "=" + tex.name;

                node.hasTexture = true;
                if (asset != "none")
                {
                    node.textureBundle = asset;
                    continue;
                }
                SetAssetBundle(tex.name, GetTexturePackName(SceneName), mAssetsTexture);
                bool bExist = false;
                foreach(Texture t in textures)
                {
                    if(t.name == tex.name)
                        bExist = true;
                }      

                if (!bExist)
                {
                    textures.Add(tex);
                }
            }
        }
    }
    void PrepareObjectAsset_v3(SceneNodeE node, List<Object> objects)
    {
 
        Object asset = AssetDatabase.LoadMainAssetAtPath(node.assetPath);
        if (asset)
        {
            string file = GetAssetBundle(asset.name);
            if (file != "none")
            {
                node.assetBundle = file;
                return;
            }
            SetAssetBundle(asset.name, GetAssetPackName(SceneName), mAssets);
            if (!objects.Contains(asset))
                objects.Add(asset);
           
        }

    }
 
    void SetAssetBundle(string assetName, string assetBundle, List<Assets> assets)
    {

        string[] names = assetName.Split(';');


        bool found = false;
        foreach (Assets asset in  assets)
        {
            if(asset.Name == assetBundle)
            {
                if (asset.Objects == null)
                    asset.Objects = new List<string>();
                for (int i = 0; i < names.Length; i++ )
                {
                    if(!asset.Objects.Contains(names[i]))
                        asset.Objects.Add(names[i]);
                }

                found = true;
            }
  
        }
        if(!found)
        {
            Assets asset = new Assets();
            asset.Name = assetBundle;
            asset.Objects = new List<string>();
            //asset.Objects.Add(assetName);
            for (int i = 0; i < names.Length; i++)
            {   
                if(!asset.Objects.Contains(names[i]))
                    asset.Objects.Add(names[i]);
            }

            assets.Add(asset);
        }
    }

    #region 资源导出公用
   
    Object CreatePrefab(GameObject go, string file)
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
        if (!prefab)
        {
            GameObject clone = Object.Instantiate(go) as GameObject;

            prefab = EditorUtility.CreateEmptyPrefab(file);

            EditorUtility.ReplacePrefab(clone, prefab);

            Object.DestroyImmediate(clone);

            AssetDatabase.Refresh(IA_Option);
        }

        return prefab;
		
	}
	void AddNodeToList(SceneNodeE node){
		
		PrefabsList.Add(node);
	}

    void SaveTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
		if(terrain == null) return;
        int terrainLayer = terrain.gameObject.layer;
        //string terrainName = terrain.name;
        TerrainData terrainData = terrain.terrainData;
        if (terrainData == null)
            return;
        string terrainPath = AssetDatabase.GetAssetPath(terrainData);

        EditorApplication.NewScene();
        Object.DestroyImmediate(GameObject.Find("Main Camera"));

        terrainData = AssetDatabase.LoadMainAssetAtPath(terrainPath) as TerrainData;

        // Create root
        GameObject root = new GameObject(SceneName);

        GameObject terrainGO = Terrain.CreateTerrainGameObject(terrainData);
        terrainGO.transform.parent = root.transform;
        terrainGO.name = "Terrain";
        terrainGO.layer = terrainLayer;//给地形设置层  [3/21/2012 ZZY]

        Directory.CreateDirectory(exportPath + SceneName + "/");
        string savePath = exportPath + SceneName + "/" + SceneName;
        EditorApplication.SaveScene(savePath + ".unity");

        string[] loaderLevels = { savePath + ".unity" };
        BuildPipeline.BuildPlayer(loaderLevels, savePath + ".unity3d", BuildTarget.WebPlayer, BuildOptions.BuildAdditionalStreamedScenes);

        
    }

    void SaveToJson()
    {
        string savePath = PrefabsPath + SceneName + "/" + SceneName + ".asset";
        string bundlePath = exportPath + SceneName + "/" + SceneName + ".ab";
       
        mSceneClient = new Scene();
        mSceneClient.name = SceneName;
        mSceneClient.terrainFile = SceneName + "/" + SceneName + ".unity3d";
        mSceneClient.objects = new List<SceneNode>();


        foreach (SceneNodeE node in PrefabsList)
        {
            writeObjectToScene(mSceneClient, node);
        }
        EditorSerializer.SerializeObjectAndSave(mSceneClient, exportPath + SceneName + "/" + SceneName + ".json");

        //save to asset
        StringHolder jsonAsset = ScriptableObject.CreateInstance<StringHolder>();
        string[] stringholder = {EditorSerializer.ObjectToJson(mSceneClient)};
        jsonAsset.content = stringholder;
        AssetDatabase.CreateAsset(jsonAsset, savePath);
        Object o = AssetDatabase.LoadAssetAtPath(savePath, typeof(StringHolder));
        BuildPipeline.BuildAssetBundle(o, null, bundlePath);
        AssetDatabase.DeleteAsset(savePath);
 
       
    }
    void writeObjectToScene(Scene scene, SceneNodeE node)
    {

        SceneNode obj = new SceneNode();
        obj.name = node.name;
        obj.assetPath = node.assetBundle;
        obj.texture = node.textureBundle;
        if (node.textures != null)
            obj.texture += "@" + node.textures;

        string Value = node.position.ToString();
        string pos = Value.Substring(1, Value.Length - 2);
        string[] axis = pos.Split(',');
        obj.position[0] = System.Double.Parse(axis[0]);
        obj.position[1] = System.Double.Parse(axis[1]);
        obj.position[2] = System.Double.Parse(axis[2]);

        Value = node.rotation.ToString();
        pos = Value.Substring(1, Value.Length - 2);
        axis = pos.Split(',');
        obj.rotation[0] = System.Double.Parse(axis[0]);
        obj.rotation[1] = System.Double.Parse(axis[1]);
        obj.rotation[2] = System.Double.Parse(axis[2]);
        obj.rotation[3] = System.Double.Parse(axis[3]);

        Value = node.scale.ToString();
        pos = Value.Substring(1, Value.Length - 2);
        axis = pos.Split(',');
        obj.scale[0] = System.Double.Parse(axis[0]);
        obj.scale[1] = System.Double.Parse(axis[1]);
        obj.scale[2] = System.Double.Parse(axis[2]);

        scene.objects.Add(obj);

    }
    //导出特定文件资源
    void ExportResource(string file, string ex){}

    #endregion

  
    #region 资源导出 v2

    /// <summary>
    /// 只在根节点创建prefab
    /// </summary>
    /// <param name="go"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    SceneNodeE CreateNew_v2(GameObject go, string path)
    {

        SceneNodeE node = null;
        string file = path + go.name.Replace("(Clone)", "") + ".prefab";
    
        node = CreateNewSceneNodeE_v2(go, file);
        CreatePrefab(go, file);
        
        return node;

    }

    SceneNodeE CreateNewSceneNodeE_v2(GameObject go, string path)
    {
        SceneNodeE node = new SceneNodeE();
        node.name = go.name.Replace("(Clone)", "");
        node.assetPath = path;
        node.textureBundle = "none";
        node.assetBundle = "none";
        node.position = go.transform.position;
        node.rotation = go.transform.rotation;
        node.scale = go.transform.localScale;

        return node;
    }
   #endregion


	void LoadAssetsInfo(string file, string texFile){

        if (!File.Exists(file))
            return;
 
        mAssets = EditorSerializer.JsonToAssetList(file);
        mAssetsTexture = EditorSerializer.JsonToAssetList(texFile);
	}
	
    void SaveAssetXML()
    {
        //XmlDocument doc = new XmlDocument();

        //XmlElement root = doc.CreateElement("AssetBundle");
        //doc.AppendChild(root);

        //foreach (Assets asset in mAssets)
        //{

        //    XmlElement assets = doc.CreateElement("Assets");
        //    XmlAttribute name = doc.CreateAttribute("name");
        //    assets.SetAttributeNode(name);
        //    assets.SetAttribute("name", asset.Name);
        //    root.AppendChild(assets);

        //    foreach (string file in asset.Objects)
        //    {
        //        XmlElement child = doc.CreateElement("Object");
        //        XmlAttribute fileName = doc.CreateAttribute("File");
        //        child.SetAttributeNode(fileName);
        //        child.SetAttribute("File", file);
        //        assets.AppendChild(child);
        //    }
        //}
        EditorSerializer.SerializeObjectAndSave(mAssets, exportPath + "Assets.json");
        
        //foreach (Assets asset in mAssetsTexture)
        //{

        //    XmlElement assets = doc.CreateElement("Textures");
        //    XmlAttribute name = doc.CreateAttribute("name");
        //    assets.SetAttributeNode(name);
        //    assets.SetAttribute("name", asset.Name);
        //    root.AppendChild(assets);

        //    foreach (string file in asset.Objects)
        //    {
        //        XmlElement child = doc.CreateElement("Texture");
        //        XmlAttribute fileName = doc.CreateAttribute("File");
        //        child.SetAttributeNode(fileName);
        //        child.SetAttribute("File", file);
        //        assets.AppendChild(child);
        //    }
        //}

        EditorSerializer.SerializeObjectAndSave(mAssetsTexture, exportPath + "Textures.json");

        //doc.Save(exportPath + "Assets.xml");
    }

    private void OnUIWizardCreate()
    {
        if (onlyPackWinName != "")
            packUiAlone = true;

        //初始化容器
        //InitializeExport();

        //指定功能函数
        //ExportResource();

        //开始导出
        OnUIExportAssets();

        //释放资源
        //RealeaseExport();
    }

    private void OnUIExportAssets()
    {
        //GameObject[] gos;
        //if (onlyPackWinName == "")
        //    gos = GameObject.FindGameObjectsWithTag(NeedExportTag);
        //else
        //{
        //    gos = new GameObject[1];
        //    gos[0] = GameObject.Find(onlyPackWinName);
        //}

        string[] scenePath = EditorApplication.currentScene.Split('/');
        string[] sceneName = scenePath[scenePath.Length - 1].Split('.');
        SceneName = sceneName[0];
        string localdir = PrefabsPath + SceneName + "/Prefabs/";
        Directory.CreateDirectory(localdir);
        //必须刷新
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        //foreach (GameObject go in gos)
        //{
        //    SceneNodeE root = createNewNode(go, localdir);
        //    AddNodeToList(root);
        //}

        SaveUIObj();
    }

    private void SaveUIObj()
    {
        //打包UI对象
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;

        List<Object> objects = new List<Object>();
        List<Object> objectsTexture = new List<Object>();

        //game objects
        string localDir = exportPath + "AssetBundles/";

        //must create directory before buildAssetBundle
        if (Directory.Exists(localDir))
            Directory.Delete(localDir, true);
        Directory.CreateDirectory(localDir);

        BuildPipeline.PushAssetDependencies();

        // 打包UI的通用部分 [2/8/2012 Ivan]
        List<UnityEngine.Object> commonObjs = EditorHelpers.CollectAll<UnityEngine.Object>(uiCommonResPath);
        string comName = localDir + GetUIPackName("UICommon");
        BuildPipeline.BuildAssetBundle(
                null, commonObjs.ToArray(), comName, options);
        // 如果是单独打包的话，这里就要弹出依赖项，否则就公用common部分 [3/22/2012 Ivan]
        if (packUiAlone)
            BuildPipeline.PopAssetDependencies();


        BuildPipeline.PushAssetDependencies();
        List<UnityEngine.Object> fonts = EditorHelpers.CollectAll<UnityEngine.Object>(uiFontsResPath);
        string fontName = localDir + GetUIPackName("UIFonts");
        BuildPipeline.BuildAssetBundle(
                null, fonts.ToArray(), fontName, options);

        List<UnityEngine.Object> uiPrefabs = EditorHelpers.CollectAll<UnityEngine.Object>(uiPrefabPath);
        UnityEngine.Object[] packPrefab = new UnityEngine.Object[1];
        //打包UI对象
        for (int i = 0; i < uiPrefabs.Count; i++)
        {
            packPrefab[0] = uiPrefabs[i];

            if (packPrefab != null && packPrefab[0] != null &&
                (onlyPackWinName == "" || onlyPackWinName == packPrefab[0].name))
            {
                BuildPipeline.PushAssetDependencies();
                BuildPipeline.BuildAssetBundle(null, packPrefab, localDir + packPrefab[0].name + ".ui", options);
                BuildPipeline.PopAssetDependencies();
            }
        }

        // 弹出Fonts [4/20/2012 Ivan]
        BuildPipeline.PopAssetDependencies();

        // 如果不是单独打包的话，这里才弹出依赖项 [3/22/2012 Ivan]
        if (!packUiAlone)
            BuildPipeline.PopAssetDependencies();

        // 打包Icon部分 [2/17/2012 Ivan]
        BuildPipeline.PushAssetDependencies();
        List<UnityEngine.Object> uiObjs = EditorHelpers.CollectAll<UnityEngine.Object>(uiIconResPath);
        string uiPackName = localDir + GetUIPackName("Icons");
        BuildPipeline.BuildAssetBundle(
                null, uiObjs.ToArray(), uiPackName, options);
        BuildPipeline.PopAssetDependencies();

        //DestroyAllPrefab();
    }
}
