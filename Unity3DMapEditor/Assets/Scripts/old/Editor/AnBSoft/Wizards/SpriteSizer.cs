//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Threading;


public class SpriteSizer : ScriptableWizard 
{
	public bool onlyApplyToSelected = false;	// Only apply settings to the selected sprites
	public bool applyToAllInScene = false;		// Applies the sizing to all sprites in the scene
	public bool applyToAllPrefabs = true;		// Applies sizing to all prefabs in the project folder
	public bool disablePixelPerfect = false;	// When set, disables pixel perfect on all sprites processed. If this is not set, any sprites with pixel perfect enabled will not be changed.

	//public int targetScreenWidth = 480;
	public int targetScreenHeight = 320;

	public Camera renderCamera;

	string onlySelectedHelp = "Only apply sizing to currently selected sprites.";
	string allInSceneHelp = "Will apply sizing to all sprites in this scene. WARNING: This can override the height/width settings in any prefab instances! Try checking \"Apply To All Prefabs\" instead.";
	string allPrefabsHelp = "scan the entire project folder for prefabs and apply sizing to them.";

	ArrayList sprites = new ArrayList();


	// Loads previous settings from PlayerPrefs
	void LoadSettings()
	{
		onlyApplyToSelected = 1 == PlayerPrefs.GetInt("SpriteSizer.onlyApplyToSelected", onlyApplyToSelected ? 1 : 0);
		applyToAllInScene = 1 == PlayerPrefs.GetInt("SpriteSizer.applyToAllInScene", applyToAllInScene ? 1 : 0);
		applyToAllPrefabs = 1 == PlayerPrefs.GetInt("SpriteSizer.applyToAllPrefabs", applyToAllPrefabs ? 1 : 0);
		disablePixelPerfect = 1 == PlayerPrefs.GetInt("SpriteSizer.disablePixelPerfect", disablePixelPerfect ? 1 : 0);
		targetScreenHeight = PlayerPrefs.GetInt("SpriteSizer.targetScreenHeight", targetScreenHeight);

		string camName = PlayerPrefs.GetString("SpriteSizer.camName");
		if(!System.String.IsNullOrEmpty(camName))
		{
			GameObject go = GameObject.Find(camName);

			if (go != null)
				renderCamera = go.GetComponent(typeof(Camera)) as Camera;
		}
	}

	// Saves settings to PlayerPrefs
	void SaveSettings()
	{
		PlayerPrefs.SetInt("SpriteSizer.onlyApplyToSelected", onlyApplyToSelected ? 1 : 0);
		PlayerPrefs.SetInt("SpriteSizer.applyToAllInScene", applyToAllInScene ? 1 : 0);
		PlayerPrefs.SetInt("SpriteSizer.applyToAllPrefabs", applyToAllPrefabs ? 1 : 0);
		PlayerPrefs.SetInt("SpriteSizer.disablePixelPerfect", disablePixelPerfect ? 1 : 0);
		PlayerPrefs.SetInt("SpriteSizer.targetScreenHeight", targetScreenHeight);
		PlayerPrefs.SetString("SpriteSizer.camName", renderCamera.name);
	}


	[UnityEditor.MenuItem("Tools/A&B Software/Size Sprites")]
	static void StartSizingSprites()
	{
		SpriteSizer ss = (SpriteSizer) ScriptableWizard.DisplayWizard("Size Sprites", typeof(SpriteSizer), "Ok");
		ss.LoadSettings();
	}


	// Updates the wizard:
	void OnWizardUpdate()
	{
		if (renderCamera == null)
			renderCamera = Camera.main;

		if(onlyApplyToSelected)
		{
			// See if we have a valid selection:
			Object[] o = Selection.GetFiltered(typeof(SpriteRoot), SelectionMode.Unfiltered);
			if(o != null)
				if(o.Length != 0)
				{
					// Uncheck other options:
					applyToAllInScene = false;
					applyToAllPrefabs = false;
					helpString = onlySelectedHelp;
					errorString = "";
					isValid = true;
					return;
				}

			// Else we don't have a valid selection, so uncheck:
			onlyApplyToSelected = false;
		}

		helpString = "";

		if (applyToAllInScene)
			helpString = allInSceneHelp;

		if(applyToAllPrefabs)
		{
			if (helpString.Length != 0)
				helpString += "  Will also ";
			else
				helpString += "Will ";

			helpString += allPrefabsHelp;
		}

		if (helpString.Length == 0)
		{
			isValid = false;
			errorString = "Nothing to do!";
		}
		else
		{
			isValid = true;
			errorString = "";
		}
	}


	// Let's do this thing!:
	void OnWizardCreate()
	{
		float worldUnitsPerScreenPixel;
		int skipped = 0; // How many sprites had to be skipped because they were prefabs?

		if (disablePixelPerfect)
			disablePixelPerfect = EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to disable pixel-perfect on all selected sprites?", "Yes", "No");

		// Get our desired sprites:
		FindSprites();


		if (renderCamera.isOrthoGraphic)
		{
			// Use orthographic logic:
			worldUnitsPerScreenPixel = (renderCamera.orthographicSize * 2f) / targetScreenHeight;

			// Now set their sizes:
			for (int i = 0; i < sprites.Count; ++i)
			{
				SpriteRoot sprite = (SpriteRoot)sprites[i];
				Vector2 pxSize = sprite.GetDefaultPixelSize(AssetDatabase.GUIDToAssetPath, AssetDatabase.LoadAssetAtPath);

				if (disablePixelPerfect)
					sprite.pixelPerfect = false;

				if (sprite.spriteMesh == null)
				{
					sprite.width = pxSize.x * worldUnitsPerScreenPixel;
					sprite.height = pxSize.y * worldUnitsPerScreenPixel;
				}
				else
					sprite.SetSize(pxSize.x * worldUnitsPerScreenPixel, pxSize.y * worldUnitsPerScreenPixel);

				EditorUtility.SetDirty(((SpriteRoot)sprites[i]).gameObject);
			}
		}
		else
		{
			// Use perspective logic:
			float dist;
			Plane nearPlane;

			// Now set their sizes:
			for (int i = 0; i < sprites.Count; ++i)
			{
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	#if (UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				PrefabType ptype = PrefabUtility.GetPrefabType(((SpriteRoot)sprites[i]).gameObject);
	#else
				PrefabType ptype = EditorUtility.GetPrefabType(((SpriteRoot)sprites[i]).gameObject);
	#endif
				// We can't do prefabs with perspective cameras:
				if (ptype == PrefabType.Prefab || ptype == PrefabType.ModelPrefab)
				{
					++skipped;
					Debug.LogWarning("Sprite \"" + ((SpriteRoot)sprites[i]).name + "\" skipped because it is a prefab and the selected camera is perspective. Size cannot be calculated for perspective cameras without the object actually being positioned in front of the camera.\n Either use an orthographic camera, or use an instance of the prefab in the scene instead.");
					continue;
				}
#endif
				SpriteRoot sprite = (SpriteRoot)sprites[i];
				nearPlane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);
				Vector2 pxSize = sprite.GetDefaultPixelSize(AssetDatabase.GUIDToAssetPath, AssetDatabase.LoadAssetAtPath);

				// Determine the world distance between two vertical
				// screen pixels for this camera:
				dist = nearPlane.GetDistanceToPoint(sprite.transform.position);
				worldUnitsPerScreenPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0, 1, dist)), renderCamera.ScreenToWorldPoint(new Vector3(0, 0, dist)));

				if (disablePixelPerfect)
					sprite.pixelPerfect = false;

				if (sprite.spriteMesh == null)
				{
					sprite.width = pxSize.x * worldUnitsPerScreenPixel;
					sprite.height = pxSize.y * worldUnitsPerScreenPixel;
				}
				else
					sprite.SetSize(pxSize.x * worldUnitsPerScreenPixel, pxSize.y * worldUnitsPerScreenPixel);

				EditorUtility.SetDirty(((SpriteRoot)sprites[i]).gameObject);
			}
		}



		// See if we need to advise the user to reload the scene:
		if (applyToAllPrefabs)
			EditorUtility.DisplayDialog("NOTE", "You may need to reload the current scene for prefab instances to reflect your changes.", "OK");

		Debug.Log( (sprites.Count - skipped) + " sprites sized.");

		// Save our settings for next time:
		SaveSettings();
	}


	// Finds all desired sprites
	void FindSprites()
	{
		sprites.Clear();

		if(onlyApplyToSelected)
		{
			Object[] o = Selection.GetFiltered(typeof(SpriteRoot), SelectionMode.Unfiltered);
			sprites.AddRange(o);
			return;
		}

		if(applyToAllInScene)
		{
			// Get all packed sprites in the scene:
			Object[] o = FindObjectsOfType(typeof(SpriteRoot));

			for (int i = 0; i < o.Length; ++i)
			{
				if (applyToAllPrefabs)
				{
					// Check to see if this is a prefab instance,
					// and if so, don't use it since we'll be updating
					// the prefab itself anyway.
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	#if (UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
					if (PrefabType.PrefabInstance != PrefabUtility.GetPrefabType(o[i]))
	#else
					if (PrefabType.PrefabInstance != EditorUtility.GetPrefabType(o[i]))
	#endif
#endif
						sprites.Add(o[i]);
				}
				else
					sprites.Add(o[i]);
			}
		}

		// See if we need to scan the Assets folder for sprite objects
		if (applyToAllPrefabs)
			ScanProjectFolder(sprites);
	}


	// Scans the project folder, looking for sprite prefabs
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
						c = obj.GetComponentsInChildren(typeof(SpriteRoot), true);

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
}
