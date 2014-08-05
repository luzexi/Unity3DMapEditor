
//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


// Warns when a null texture is found:
#define WARN_ON_NULL_TEXTURE


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;


// Interface for a SpriteTimelineEditor.
// This is put here since this script is common to all
// packages.
public interface ISTE
{
	void OnSelectionChange();
	void Setup(Rect wnd, float top);
	void SetCurAnim(int index);
	bool STEOnGUI(float adj, out float bottom);
	bool STEUpdate();
	void OnFocus();
	void OnHierarchyChange();
	Rect position { get; set; }
	EditorWindow Editor { get; set; }
}


public class MaterialSpriteList
{
	// The material associated with this 
	// list of sprites which use it
	public Material material;

	// The path to the texture file of this atlas
	public string texPath;

	// The sprites which use this material
	public ArrayList sprites = new ArrayList();
}


public class AtlasTextureList
{
	public Material material;	// The material associated with the atlas we're going to build
	public ArrayList textures = new ArrayList();	// The source textures for the atlas
	public ArrayList trimmedTextures = new ArrayList();	// Trimmed versions of the source textures, so our atlas will be optimized
	public Rect[] uvs;			// UVs of the trimmed textures, once placed on the atlas
	public ArrayList frames = new ArrayList();	// Frame info for each trimmed textures (offset, etc)
	public string texPath;		// The path to the texture file of this atlas
}


public class BuildAtlases : ScriptableWizard
{
	public string atlasFolder = "Sprite Atlases";	// Folder in which to place generated atlases
	public int maxAtlasSize = 1024;		// Maximum pixel dimensions of a generated atlas
	public int padding = 1;				// Pixels of padding to place around each packed texture
	public bool trimImages = true;		// Whether or not to trim blank pixels from sprite frames automatically.
	public bool forceSquare = false;	// Force the output atlases to be square?
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	public bool onlyScanProjectFolder = true;	// Scan the entire "Assets" folder for prefabs containing packable sprites.
#else
	public bool scanProjectFolder = true;	// Reuse the entire "Assets" folder for prefabs containing packable sprites.
#endif

	public static bool doSpecificMaterials = false; // Will only build the atlas for the selected materials - the ones currently selected
	public static List<Material> targetMaterials = new List<Material>();	// Reference to the selected materials for which a single-material build will occur.

	string assetPath;
	System.Type packableType = typeof(ISpriteAggregator);


	// Loads previous settings from PlayerPrefs.
	void LoadSettings()
	{
		atlasFolder = PlayerPrefs.GetString("BuildAtlases.atlasFolder", atlasFolder);
		maxAtlasSize = PlayerPrefs.GetInt("BuildAtlases.maxAtlasSize", maxAtlasSize);
		padding = PlayerPrefs.GetInt("BuildAtlases.padding", padding);
		trimImages = 1 == PlayerPrefs.GetInt("BuildAtlases.trimImages", trimImages ? 1 : 0);
		forceSquare = 1 == PlayerPrefs.GetInt("BuildAtlases.forceSquare", forceSquare ? 1 : 0);
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		onlyScanProjectFolder = 1 == PlayerPrefs.GetInt("BuildAtlases.onlyScanProjectFolder", onlyScanProjectFolder ? 1 : 0);
#else
		scanProjectFolder = 1 == PlayerPrefs.GetInt("BuildAtlases.scanProjectFolder", scanProjectFolder ? 1 : 0);
#endif
	}

	// Saves settings to PlayerPrefs.
	void SaveSettings()
	{
		PlayerPrefs.SetString("BuildAtlases.atlasFolder", atlasFolder);
		PlayerPrefs.SetInt("BuildAtlases.maxAtlasSize", maxAtlasSize);
		PlayerPrefs.SetInt("BuildAtlases.padding", padding);
		PlayerPrefs.SetInt("BuildAtlases.trimImages", trimImages ? 1 : 0);
		PlayerPrefs.SetInt("BuildAtlases.forceSquare", forceSquare ? 1 : 0);
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		PlayerPrefs.SetInt("BuildAtlases.onlyScanProjectFolder", onlyScanProjectFolder ? 1 : 0);
#else
		PlayerPrefs.SetInt("BuildAtlases.scanProjectFolder", scanProjectFolder ? 1 : 0);
#endif
	}


	[UnityEditor.MenuItem("Tools/A&B Software/Build Atlases &a")]
	static void BuildSpriteAtlases()
	{
		BuildAtlases ba = (BuildAtlases) ScriptableWizard.DisplayWizard("Build Atlases", typeof(BuildAtlases));
		ba.LoadSettings();
	}

	// Single-material build:
	[UnityEditor.MenuItem("Tools/A&B Software/Build Atlas for Material &m")]
	static void BuildMaterialAtlas()
	{
		BuildAtlases.doSpecificMaterials = true;

		Object[] sel = Selection.GetFiltered(typeof(Material), SelectionMode.ExcludePrefab);

		for (int i = 0; i < sel.Length; ++i )
			BuildAtlases.targetMaterials.Add((Material)sel[i]);

		BuildAtlases ba = (BuildAtlases) ScriptableWizard.DisplayWizard("Build Atlas", typeof(BuildAtlases));
		ba.LoadSettings();
	}

	// Validator:
	[UnityEditor.MenuItem("Tools/A&B Software/Build Atlas for Material &m", true)]
	static bool ValidateBuildMaterialAtlas()
	{
		return Selection.activeObject is Material;
	}

	// Context menu:
	[UnityEditor.MenuItem("CONTEXT/Material/Build Atlas")]
	static void BuildMaterialAtlasContext(MenuCommand cmd)
	{
		BuildAtlases.doSpecificMaterials = true;
		BuildAtlases.targetMaterials.Add((Material)cmd.context);
		
		BuildAtlases ba = (BuildAtlases) ScriptableWizard.DisplayWizard("Build Atlas", typeof(BuildAtlases));
		ba.LoadSettings();
	}

	// Validator:
	[UnityEditor.MenuItem("CONTEXT/Material/Build Atlas", true)]
	static bool ValidateBuildMaterialAtlasContext(MenuCommand cmd)
	{
		return cmd.context is Material;
	}


	void OnWizardCreate()
	{
		ArrayList sprites = new ArrayList();
		List<MaterialSpriteList> matSpriteMap;		// List of all materials used, with their associated sprites
		AtlasTextureList texList;
		int numAtlasesBuilt = 0;

		// Get the path we'll use to write our atlases:
		assetPath = Application.dataPath + "/" + atlasFolder;


		FindPackableSprites(sprites);

		matSpriteMap = OrganizeByMaterial(sprites);

		// Make sure everything went okay.
		// If not the previous method will
		// already have printed an error,
		// so just return:
		if (matSpriteMap == null)
			return;

		// If we're only building for the selected materials,
		// remove all but the desired materials:
		if (doSpecificMaterials)
		{
			// Unset our flag:
			doSpecificMaterials = false;

			List<MaterialSpriteList> matList = new List<MaterialSpriteList>();
			for (int i = 0; i < matSpriteMap.Count; ++i)
				if (targetMaterials.Contains(matSpriteMap[i].material))
				{
					matList.Add(matSpriteMap[i]);
				}

			// Remove all, then re-add just the ones we want:
			matSpriteMap.Clear();

			if (matList.Count > 0)
				matSpriteMap.AddRange(matList);
			else
			{
				EditorUtility.DisplayDialog("No packable sprites found.", "No packable sprites found for the selected materials.\n\nTry changing the \"Scan Project Folder\" setting.", "Ok");
				return;
			}

			// We don't need this list anymore:
			targetMaterials.Clear();
		}

		// Create the folder that will hold our atlases:
		Directory.CreateDirectory(assetPath);

		// Loop through each material and process its
		// sprites, then build its atlas before moving
		// to the next material.
		for (int i = 0; i < matSpriteMap.Count; ++i)
		{
			texList = BuildTextureList(matSpriteMap[i]);

			// Make sure there are textures in the list:
			if (texList.textures.Count < 1)
			{
				Debug.LogWarning("Warning: No textures found for material \"" + texList.material.name + "\".");
				--numAtlasesBuilt;
			}
			else
				BuildAtlas(texList);

			// Now assign the frame info:
			SetFrameInfo((MaterialSpriteList)matSpriteMap[i], texList);

			// Save the atlas path:
			matSpriteMap[i].texPath = texList.texPath;

			texList = null;

			// Try to free some memory:
			System.GC.Collect();
#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			Resources.UnloadUnusedAssets();
#endif

			++numAtlasesBuilt;
		}

#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		// Make sure all changes to objects are committed to disk:
		//AssetDatabase.SaveAssets();
#endif
		// Make sure the new atlases are (re-)imported:
		AssetDatabase.Refresh();

		// Now assign each material its atlas now that we've done a asset refresh:
		for (int i = 0; i < matSpriteMap.Count; ++i)
			matSpriteMap[i].material.mainTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(matSpriteMap[i].texPath, typeof(Texture2D));

		Debug.Log(numAtlasesBuilt + " atlases generated.");

		// Save our settings for next time:
		SaveSettings();
	}


	// Finds all packable sprite objects:
	void FindPackableSprites(ArrayList sprites)
	{
		List<Object> objList = new List<Object>();

		// Get all packed sprites in the scene:
		Object[] o = FindObjectsOfType(typeof(AutoSpriteBase));

		objList.AddRange(o);

		o = FindObjectsOfType(typeof(PackableStub));

		objList.AddRange(o);

		for (int i = 0; i < objList.Count; ++i)
		{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			if (onlyScanProjectFolder)
#else
			if (scanProjectFolder)
#endif
			{
				// Check to see if this is a prefab instance,
				// and if so, don't use it since we'll be updating
				// the prefab itself anyway.
				// Don't add it at all if we're in Unity iPhone and
				// we'll be scanning the project folder since in
				// iPhone, we can't tell if a scene instance is a
				// prefab instance and we'll mess up prefab relationships
				// otherwise:
#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
				if (PrefabType.PrefabInstance != PrefabUtility.GetPrefabType(objList[i]))
					sprites.Add(objList[i]);
	#else
				if (PrefabType.PrefabInstance != EditorUtility.GetPrefabType(objList[i]))
					sprites.Add(objList[i]);
	#endif
#endif
			}
			else
				sprites.Add(objList[i]);
		}

		// See if we need to scan the Assets folder for sprite objects
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (onlyScanProjectFolder)
#else
		if (scanProjectFolder)
#endif
			ScanProjectFolder(sprites);

		// Now filter for the types of sprites we want:
		for (int i = 0; i < sprites.Count; ++i)
		{
			if (!packableType.IsInstanceOfType(sprites[i]))
			{
				sprites.RemoveAt(i);
				--i;
			}
		}
	}


	// Scans the entire Assets folder recursively looking for
	// prefabs that contain packable sprites:
	void ScanProjectFolder(ArrayList sprites)
	{
		string[] files;
		GameObject obj;
		Component[] c;

		// Stack of folders:
		Stack stack = new Stack();

		// Add root directory:
		stack.Push(Application.dataPath);

		// Continue while there are folders to process
		while (stack.Count > 0)
		{
			// Get top folder:
			string dir = (string)stack.Pop();

			try
			{
				// Get a list of all prefabs in this folder:
				files = Directory.GetFiles(dir, "*.prefab");

				// Process all prefabs:
				for (int i = 0; i < files.Length; ++i)
				{
					// Make the file path relative to the assets folder:
					files[i] = files[i].Substring(Application.dataPath.Length - 6);

					obj = (GameObject)AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject));

					if (obj != null)
					{
						c = obj.GetComponentsInChildren(typeof(ISpriteAggregator), true);

						for (int j = 0; j < c.Length; ++j)
							sprites.Add(c[j]);
					}
				}

				// Add all subfolders in this folder:
				foreach (string dn in Directory.GetDirectories(dir))
				{
					stack.Push(dn);
				}
			}
			catch
			{
				// Error
				Debug.LogError("Could not access folder: \"" + dir + "\"");
			}
		}
	}


	// Organizes all sprites by associating them with the
	// material they use. Returns an array list of
	// MaterialSpriteLists:
	List<MaterialSpriteList> OrganizeByMaterial(ArrayList sprites)
	{
		string errString = "";

		// A list of all materials
		ArrayList materials = new ArrayList();
		// The map of materials to sprites
		List<MaterialSpriteList> map = new List<MaterialSpriteList>();
		// The material of the sprite:
		Material mat;

		ISpriteAggregator sprite;
		MaterialSpriteList matSpriteList;
		int index;

		for (int i = 0; i < sprites.Count; ++i)
		{
			sprite = (ISpriteAggregator)sprites[i];

			mat = sprite.GetPackedMaterial(out errString);

			if(mat == null)
			{
				EditorUtility.DisplayDialog("ERROR", errString, "Ok");
				Selection.activeGameObject = sprite.gameObject;
				return null;
			}

			index = materials.IndexOf(mat);

			// See if the material is already in our list:
			if (index >= 0)
			{
				matSpriteList = (MaterialSpriteList)map[index];
				matSpriteList.sprites.Add(sprite);
			}
			else
			{
				materials.Add(mat);
				matSpriteList = new MaterialSpriteList();
				matSpriteList.material = mat;
				matSpriteList.sprites.Add(sprite);

				map.Add(matSpriteList);
			}
		}

		return map;
	}


	// Finds all textures which are to be part
	// of a given atlas:
	AtlasTextureList BuildTextureList(MaterialSpriteList msMap)
	{
		ISpriteAggregator spr;
		Texture2D newTex = null;
		SPRITE_FRAME frameInfo = new SPRITE_FRAME(0);
		AtlasTextureList texList = new AtlasTextureList();
		texList.material = msMap.material;

		// Get all the textures pointed to by the sprites:
		for (int i = 0; i < msMap.sprites.Count; ++i)
		{
			spr = (ISpriteAggregator)msMap.sprites[i];
			spr.Aggregate(AssetDatabase.GUIDToAssetPath, AssetDatabase.LoadAssetAtPath, AssetDatabase.AssetPathToGUID);	// Get the sprite ready to give us its textures

			for (int j = 0; j < spr.SourceTextures.Length; ++j)
			{
				if (spr.SourceTextures[j] != null)
				{
					// See if the texture is already in our list:
					if (-1 >= texList.textures.IndexOf(spr.SourceTextures[j]))
					{
						VerifyImportSettings(spr.SourceTextures[j]);
						texList.textures.Add(spr.SourceTextures[j]);
						ProcessFrame(spr.SourceTextures[j], spr.DoNotTrimImages, ref newTex, ref frameInfo);
						texList.trimmedTextures.Add(newTex);
						texList.frames.Add(frameInfo);
					}
				}
				else
				{
#if WARN_ON_NULL_TEXTURE
					Debug.LogWarning("Null texture reference detected on sprite \"" + ((Component)spr).name + "\"! Please verify that this was intentional.");
#endif
				}
			}
		}

		return texList;
	}


	// Builds an atlas from the specified texture list
	void BuildAtlas(AtlasTextureList texList)
	{
		Texture2D atlas;

		atlas = new Texture2D(4, 4);
		atlas.name = texList.material.name;

		//=============================================
		// The following is currently disabled as it 
		// does not seem to work around the Unity bug:
		//=============================================
		// To get around the Unity bug where the max texture size is
		// limited to 1024x1024 when in iPhone mode for newly-created
		// textures, save this texture to disk as an asset and then
		// re-import it with modified import settings to set the max
		// texture size to what we want:
		byte[] bytes;// = atlas.EncodeToPNG();
		string atlasPath = assetPath + "/" + atlas.name + ".png";
//		string projectRelativePath = "Assets/" + atlasFolder + "/" + atlas.name + ".png";
// 
// 		// Write out the atlas file:
// 		using (FileStream fs = File.Create(atlasPath))
// 		{
// 			fs.Write(bytes, 0, bytes.Length);
// 		}
// 
// 		// Make sure the max texture size is set to what we want:
// 		// Get the texture's importer:
// 		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(projectRelativePath);
// 		if (importer.maxTextureSize < maxAtlasSize || importer.isReadable)
// 		{
// 			// Re-import it with the desired max texture size:
// 			importer.maxTextureSize = maxAtlasSize;
// 			importer.isReadable = true;
// 			AssetDatabase.ImportAsset(projectRelativePath, ImportAssetOptions.ForceSynchronousImport);
// 		}
// 
// 		// Now re-open the texture asset:
// 		atlas = (Texture2D) AssetDatabase.LoadAssetAtPath(projectRelativePath, typeof(Texture2D));

		// Pack the textures to the atlas:
		texList.uvs = atlas.PackTextures((Texture2D[])texList.trimmedTextures.ToArray(typeof(Texture2D)), padding, maxAtlasSize);

		// Check to see if the texture had to be resized to fit:
		if (texList.uvs[0].width * atlas.width != ((Texture2D)texList.trimmedTextures[0]).width ||
		   texList.uvs[0].height * atlas.height != ((Texture2D)texList.trimmedTextures[0]).height)
		{
			EditorUtility.DisplayDialog("WARNING: Textures resized!", "WARNING: Not all textures were able to fit on atlas \"" + atlas.name + "\" at their original sizes. These textures were scaled down to fit.  To resolve this, assign some of your sprites to use a different material, or if possible, use a larger maximum texture size.", "Ok");
			Debug.LogWarning("WARNING: Textures were resized to fit onto atlas \"" + atlas.name + "\"!  To resolve this, assign some of your sprites a different material, or if possible, use a larger maximum texture size.");
		}

		// Free our trimmed texture memory:
		for (int i = 0; i < texList.trimmedTextures.Count; ++i)
		{
			// If this isn't stored as an asset, destroy it:
			if (AssetDatabase.GetAssetPath((Texture2D)texList.trimmedTextures[i]).Length < 1)
				DestroyImmediate((Texture2D)texList.trimmedTextures[i]);
			texList.trimmedTextures[i] = null;
		}
		texList.trimmedTextures.Clear();

#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		Resources.UnloadUnusedAssets();
#endif

		// See if the texture needs to be made square:
		if (forceSquare && atlas.width != atlas.height)
		{
			int size = Mathf.Max(atlas.width, atlas.height);
			
			// Create a square texture:
			Texture2D tempTex = (Texture2D)Instantiate(atlas);
			tempTex.name = atlas.name;
			tempTex.Resize(size, size, TextureFormat.ARGB32, false);
			
			// Copy the contents:
			tempTex.SetPixels(0, 0, atlas.width, atlas.height, atlas.GetPixels(0), 0);
			tempTex.Apply(false);
			
			// Scale the UVs to account for this:
			for (int j = 0; j < texList.uvs.Length; ++j)
			{
				// See which side we expanded:
				if (atlas.width > atlas.height)
				{
					texList.uvs[j].y = texList.uvs[j].y * 0.5f;
					texList.uvs[j].yMax = texList.uvs[j].y + (texList.uvs[j].height * 0.5f);
				}
				else
				{
					texList.uvs[j].x = texList.uvs[j].x * 0.5f;
					texList.uvs[j].xMax = texList.uvs[j].x + (texList.uvs[j].width * 0.5f);
				}
			}

			if (atlas != tempTex)
				DestroyImmediate(atlas);
			atlas = tempTex;
		}

		// See if we need to trim the atlas to fit its content:
		if (!forceSquare)
		{
			Texture2D tempTex = TrimAtlas(atlas);

			// Scale the UVs to account for this:
			float widthFactor = ((float)atlas.width) / ((float)tempTex.width);
			float heightFactor = ((float)atlas.height) / ((float)tempTex.height);

			for (int j = 0; j < texList.uvs.Length; ++j)
			{
				texList.uvs[j].x *= widthFactor;
				texList.uvs[j].y *= heightFactor;
				texList.uvs[j].width *= widthFactor;
				texList.uvs[j].height *= heightFactor;
			}

			if (atlas != tempTex)
				DestroyImmediate(atlas);
			atlas = tempTex;
		}

		// Save the atlas as an asset:
		bytes = atlas.EncodeToPNG();

		// Write out the atlas file:
		using (FileStream fs = File.Create(atlasPath))
		{
			fs.Write(bytes, 0, bytes.Length);
		}

		// Flag this memory to be freed:
		bytes = null;

		// Save the atlas path:
		texList.texPath = "Assets/" + atlasFolder + "/" + atlas.name + ".png";

		Debug.Log("Finished building atlas \"" + texList.material.name + "\"...");
	}


	// Assigns all frame-related information to
	// the sprites of an atlas:
	void SetFrameInfo(MaterialSpriteList msMap, AtlasTextureList texList)
	{
		SPRITE_FRAME frameInfo;
		ISpriteAggregator spAg;

		int index;

		for (int i = 0; i < msMap.sprites.Count; ++i)
		{
			spAg = (ISpriteAggregator)msMap.sprites[i];

			// Assign each frame its UVs:
			for (int j = 0; j < spAg.SpriteFrames.Length; ++j)
			{
				// Validate that a texture has been selected:
				if (spAg.SourceTextures[j] == null)
				{
					// Set the frame size to zero to flag it
					// as a "null" frame:
					spAg.SpriteFrames[j].uvs = new Rect(0, 0, 0, 0);
					continue;
				}

				index = texList.textures.IndexOf(spAg.SourceTextures[j]);
				if (index < 0)
				{
					Debug.LogError("Error assigning frame info: frame " + j + " on object \"" + ((Component)spAg).name + "\" not found in the texture list!");
					continue;
				}

				frameInfo = (SPRITE_FRAME)texList.frames[index];
				frameInfo.uvs = texList.uvs[index];
				spAg.SpriteFrames[j].Copy(frameInfo);
			}

			// Update our sprite's appearance:
			if ( !(spAg is AutoSpriteBase && ((AutoSpriteBase)spAg).spriteMesh == null) )
			{
				if (spAg.DefaultFrame != null)
				{
					spAg.SetUVs(spAg.DefaultFrame.uvs);
				}
			}

			EditorUtility.SetDirty(spAg.gameObject);
		}
	}


	// Checks to see if the texture has the correct
	// import settings, and if not, re-imports it
	// with the correct settings:
	void VerifyImportSettings(Texture2D tex)
	{
		string texturePath;

		// Re-import so we can make sure it will be ready for atlas
		// generation:
		texturePath = AssetDatabase.GetAssetPath(tex);
		// Get the texture's importer:
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
		if (!importer.isReadable || importer.textureFormat != TextureImporterFormat.ARGB32 || importer.npotScale != TextureImporterNPOTScale.None)
		{
			// Reimport it with isReadable set to true and ARGB32:
			importer.isReadable = true;
			importer.textureFormat = TextureImporterFormat.ARGB32;
			importer.npotScale = TextureImporterNPOTScale.None;
			AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
		}
	}


	// Returns a Rect indicating the area of the texture that
	// contains non-transparent pixels:
	Rect GetOccupiedRect(Texture2D tex)
	{
		Rect area = new Rect(0, 0, 0, 0);
		int x, y;
		Color[] pixels;

		// NOTE: GetPixel() assumes the Y-axis runs from
		// 0 at the bottom to N at the top, just like UV
		// coordinates.

		pixels = tex.GetPixels(0);

		// Find the first column containing non-zero alpha:
		for (x = 0; x < tex.width; ++x)
		{
			for (y = 0; y < tex.height; ++y)
			{
				if (pixels[y * tex.width + x].a != 0f)
				{
					area.x = x;
					x = tex.width;
					break;
				}
			}
		}

		// Find the bottom-most row containing non-zero alpha:
		for (y = 0; y < tex.height; ++y)
		{
			for (x = 0; x < tex.width; ++x)
			{
				if (pixels[y * tex.width + x].a != 0f)
				{
					area.y = y;
					y = tex.height;
					break;
				}
			}
		}

		// Find the last column containing non-zero alpha:
		for (x = tex.width - 1; x >= 0; --x)
		{
			for (y = 0; y < tex.height; ++y)
			{
				if (pixels[y * tex.width + x].a != 0f)
				{
					area.xMax = x + 1;
					x = 0;
					break;
				}
			}
		}

		// Find the top-most row containing non-zero alpha:
		for (y = tex.height - 1; y >= 0; --y)
		{
			for (x = 0; x < tex.width; ++x)
			{
				if (pixels[y * tex.width + x].a != 0f)
				{
					area.yMax = y + 1;
					y = 0;
					break;
				}
			}
		}

		// Check for an empty frame, and in which case, 
		// leave a 2x2 pixel area:
		if (area.width == 0 || area.height == 0)
			area = new Rect(tex.width/2f-1f, tex.height/2f-1f, 2f, 2f);

		return area;
	}


	// Returns a texture that consists entirely of only
	// the pixels in the area specified from the source
	// texture:
	Texture2D SnipArea(Texture2D tex, Rect area)
	{
		if (tex.width == area.width && tex.height == area.height)
			return tex;

		Texture2D newTex = new Texture2D((int)area.width, (int)area.height, TextureFormat.ARGB32, false);

		newTex.SetPixels(tex.GetPixels((int)area.xMin, (int)area.yMin, (int)area.width, (int)area.height, 0));
		//newTex.Apply(false);

		return newTex;
	}


	// Returns a Rect containing values from -1 to 1 which
	// indicate the offset of the edges of the actual
	// used area from the center of the texture:
	void GetOffsets(Texture2D tex, Rect occupied, ref Vector2 topLeft, ref Vector2 bottomRight)
	{
		// The offset values generated look like this, where the 
		// graph represents the entire area of the original 
		// texture:
		//
		//					 1|
		//					  |
		//					  |
		//					  |
		//					  |
		//					  |
		//					  |
		// -1				 0|					1
		// --------------------------------------
		//					  |
		//					  |
		//					  |
		//					  |
		//					  |
		//					  |
		//					-1|
		//
		// So if the contents of a sprite occupy exactly
		// the upper-left quadrant of the original texture,
		// its left and top edges would be at -1, 1, respectively.
		// And its right and bottom edges at 0, 0, respectively.
		// This way, when calculating the positions of the sprite's
		// edges in local space, we need only multiply half the
		// width of what the original sprite would have been (had
		// all the blank space been included) by these offset
		// values to arrive at where the edges should be relative
		// to the origin.

		// NOTE: The pixel coordinate Y-axis runs from
		// 0 at the bottom to N at the top, just like UV
		// coordinates.


		Vector2 textureCenter = new Vector2(tex.width / 2f, tex.height / 2f);

		topLeft = new Vector2((occupied.xMin - textureCenter.x) / textureCenter.x,
							   ((occupied.yMax - textureCenter.y) / textureCenter.y));

		bottomRight = new Vector2((occupied.xMax - textureCenter.x) / textureCenter.x,
								   ((occupied.yMin - textureCenter.y) / textureCenter.y));
	}


	// Wraps all processing of a frame in one method
	// (Trimming, offset calculation, etc)
	void ProcessFrame(Texture2D tex, bool dontTrim, ref Texture2D newTex, ref SPRITE_FRAME frameInfo)
	{
		frameInfo = new SPRITE_FRAME(0);
		Rect occupiedArea;

		if (trimImages && !dontTrim)
			occupiedArea = GetOccupiedRect(tex);
		else
			occupiedArea = new Rect(0, 0, tex.width, tex.height);

		newTex = SnipArea(tex, occupiedArea);
		GetOffsets(tex, occupiedArea, ref frameInfo.topLeftOffset, ref frameInfo.bottomRightOffset);

		// Calculate the scale factor
		// (a value that when multiplied by the actual dimensions
		// of the sprite will yield a value that is half the size
		// that the sprite would be were the empty area not trimmed
		// out):
		frameInfo.scaleFactor.x = (tex.width / occupiedArea.width) * 0.5f;
		frameInfo.scaleFactor.y = (tex.height / occupiedArea.height) * 0.5f;
	}


	// Trims the specified atlas down to the next smaller
	// power of 2 if no opaque/translucent pixels are found
	// at the top or right edge:
	Texture2D TrimAtlas(Texture2D tex)
	{
		int x, y;
		int firstX, firstY;
		int newWidth, newHeight;
		Texture2D newTex;
		Color[] pixels;

		// NOTE: GetPixel() assumes the Y-axis runs from
		// 0 at the bottom to N at the top, just like UV
		// coordinates.

		pixels = tex.GetPixels(0);

		firstX = tex.width;
		firstY = tex.height;

		// Find the last column containing non-zero alpha:
		for (x = tex.width - 1; x >= 0; --x)
		{
			for (y = 0; y < tex.height; ++y)
			{
				if (pixels[y * tex.width + x].a != 0f)
				{
					firstX = x;
					x = 0;
					break;
				}
			}
		}

		// Find the top-most row containing non-zero alpha:
		for (y = tex.height - 1; y >= 0; --y)
		{
			for (x = 0; x < tex.width; ++x)
			{
				if (pixels[y * tex.width + x].a != 0f)
				{
					firstY = y;
					y = 0;
					break;
				}
			}
		}

		// Find the next power of 2:
		newWidth = NextPowerOf2(firstX + 1);
		newHeight = NextPowerOf2(firstY + 1);

		if (newWidth == tex.width && newHeight == tex.height)
			return tex;

		newTex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);

		newTex.SetPixels(tex.GetPixels(0, 0, newWidth, newHeight, 0));
		newTex.Apply(false);
		newTex.name = tex.name;

		return newTex;
	}

	// Returns the next highest power of 2:
	int NextPowerOf2(int val)
	{
		int newVal = Mathf.ClosestPowerOfTwo(val);

		while (newVal < val)
		{
			newVal <<= 1;
		}

		return newVal;
	}
}
