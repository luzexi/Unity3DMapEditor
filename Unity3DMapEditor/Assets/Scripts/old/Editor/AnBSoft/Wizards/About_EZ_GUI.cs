//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;


public class About_EZ_GUI : ScriptableObject
{
	public static string version = "1.078";


	[UnityEditor.MenuItem("Tools/A&B Software/About EZ GUI")]
	static void ShowVersion()
	{
		EditorUtility.DisplayDialog("EZ GUI v" + version, "EZ GUI Version " + version + "\n\nCopyright 2010 Above and Beyond Software\nAll rights reserved.", "OK");
	}
}
