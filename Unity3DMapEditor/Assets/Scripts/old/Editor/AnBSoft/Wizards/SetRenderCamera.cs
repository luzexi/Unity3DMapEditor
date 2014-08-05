
//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;



public class SetRenderCamera : ScriptableWizard
{
	public Camera renderCamera;
	public bool enablePixelPerfect;


	// Loads previous settings from PlayerPrefs.
	void LoadSettings()
	{
		enablePixelPerfect = 1 == PlayerPrefs.GetInt("SetRenderCamera.enablePixelPerfect");

		string camName = PlayerPrefs.GetString("SetRenderCamera.camName");
		if(!String.IsNullOrEmpty(camName))
		{
			GameObject go = GameObject.Find(camName);

			if(go != null)
				renderCamera = go.GetComponent(typeof(Camera)) as Camera;
		}

		OnWizardUpdate();
	}

	// Saves settings to PlayerPrefs.
	void SaveSettings()
	{
		PlayerPrefs.SetString("SetRenderCamera.camName", renderCamera.name);
		PlayerPrefs.SetInt("SetRenderCamera.enablePixelPerfect", enablePixelPerfect?1:0);
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Set Render Cameras", true)]
	static bool ValidateSetCams()
	{
		if(Selection.activeObject == null)
			return false;
		
		return true;
	}


	[UnityEditor.MenuItem("Tools/A&B Software/Set Render Cameras")]
	static void SetCams()
	{
		SetRenderCamera s = (SetRenderCamera) ScriptableWizard.DisplayWizard("Set Render Camera", typeof(SetRenderCamera), "Set");
		s.LoadSettings();
		s.helpString = "NOTE: Press Play to ensure any screen-relative sizing or placement settings are properly re-calculated.";
	}
	
	void OnWizardUpdate()
	{
		isValid = (renderCamera != null);
	}
		
	void OnWizardCreate()
	{
		List<IUseCamera> camObjs = new List<IUseCamera>();
		
		// Find only the top-level objects:
		UnityEngine.Object[] gos = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
		
		foreach (GameObject go in gos)
		{
			Component[] cs = go.GetComponentsInChildren(typeof(IUseCamera), true);
			if(cs != null && cs.Length > 0)
				foreach (IUseCamera c in cs)
					camObjs.Add((IUseCamera)c);
		}

		foreach (IUseCamera o in camObjs)
		{
			if (enablePixelPerfect)
			{
				if (o is SpriteRoot)
				{
					((SpriteRoot)o).pixelPerfect = true;
				}
				else if (o.GetType().Name.Equals("SpriteText"))
				{
					o.GetType().GetField("pixelPerfect").SetValue(o, true);
				}
			}

			o.RenderCamera = renderCamera;
		}
					
		SaveSettings();
	}
}
