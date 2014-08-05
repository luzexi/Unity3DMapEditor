//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Threading;


public class CopyControl : ScriptableObject
{
	static IControl srcControl;


	[UnityEditor.MenuItem("Tools/A&B Software/Copy Control")]
	static void Copy()
	{
		srcControl = (IControl) Selection.activeGameObject.GetComponent("IControl");
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Copy Control", true)]
	static bool ValidateCopy()
	{
		if (Selection.activeGameObject == null)
			return false;
		if (Selection.activeGameObject.GetComponent("IControl") != null)
			return true;

		return false;
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Paste Control", true)]
	static bool ValidatePaste()
	{
		IControl ctl;

		if (srcControl == null)
			return false;
		if (Selection.activeGameObject == null)
			return false;

		ctl = (IControl) Selection.activeGameObject.GetComponent("IControl");

		if (ctl != null)
		{
			// They must be of the same type:
			if (ctl.GetType() == srcControl.GetType())
				return true;
			else
				return false;
		}

		return false;
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Paste Control")]
	static void Paste()
	{
		int count=0;

		if (srcControl == null)
			return;

		Object[] o = Selection.GetFiltered(srcControl.GetType(), SelectionMode.Unfiltered);
		if(o != null)
			for (int i = 0; i < o.Length; ++i)
			{
				if (o[i].GetType() == srcControl.GetType())
				{
 					((IControl)o[i]).Copy(srcControl);
 					++count;
				}
			}

		Debug.Log(((MonoBehaviour)srcControl).gameObject.name + " pasted " + count + " times.");
	}
}
