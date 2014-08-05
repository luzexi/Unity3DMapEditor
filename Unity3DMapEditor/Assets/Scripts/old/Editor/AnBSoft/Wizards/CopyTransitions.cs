//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Threading;


public class CopyTransitions : ScriptableObject
{
	static EZTransitionList[] transList;


	[UnityEditor.MenuItem("Tools/A&B Software/Copy Transitions")]
	static void Copy()
	{
		UIPanelBase panel = null;
		IControl ctl = (IControl)Selection.activeGameObject.GetComponent("IControl");
		if (ctl == null)
			panel = (UIPanelBase)Selection.activeGameObject.GetComponent(typeof(UIPanelBase));

		if (ctl != null)
			CopyControl(ctl);
		else if (panel != null)
			CopyPanel(panel);
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Copy Transitions", true)]
	static bool ValidateCopy()
	{
		if (Selection.activeGameObject == null)
			return false;
		if (Selection.activeGameObject.GetComponent("IControl") != null)
			return true;
		if (Selection.activeGameObject.GetComponent(typeof(UIPanelBase)) != null)
			return true;

		return false;
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Paste Transitions", true)]
	static bool ValidatePaste()
	{
		if (transList == null)
			return false;
		if (transList.Length < 1)
			return false;
		if (Selection.activeGameObject == null)
			return false;
		if (Selection.activeGameObject.GetComponent("IControl") != null)
			return true;
		if (Selection.activeGameObject.GetComponent(typeof(UIPanelBase)) != null)
			return true;

		return false;
	}

	[UnityEditor.MenuItem("Tools/A&B Software/Paste Transitions")]
	static void Paste()
	{
		/*
				if (contList == null)
					return;
				if (contList.Length < 1)
					return;

				UIPanelBase panel = null;
				IControl ctl = (IControl)Selection.activeGameObject.GetComponent("IControl");
				if (ctl == null)
					panel = (UIPanelBase)Selection.activeGameObject.GetComponent(typeof(UIPanelBase));

				if (ctl != null)
					PasteControl(ctl);
				else if (panel != null)
					PastePanel(panel);
		*/

		Object[] o = Selection.GetFiltered(typeof(ControlBase), SelectionMode.Unfiltered);
		if (o != null)
			for (int i = 0; i < o.Length; ++i)
			{
				PasteControl((IControl)o[i]);
			}

		o = Selection.GetFiltered(typeof(AutoSpriteControlBase), SelectionMode.Unfiltered);
		if (o != null)
			for (int i = 0; i < o.Length; ++i)
			{
				PasteControl((IControl)o[i]);
			}

		o = Selection.GetFiltered(typeof(UIPanelBase), SelectionMode.Unfiltered);
		if (o != null)
			for (int i = 0; i < o.Length; ++i)
			{
				PastePanel((UIPanelBase)o[i]);
			}
	}


	static void CopyControl(IControl ctl)
	{
		int num = 0;

		// Count how many transition lists there are:
		while (ctl.GetTransitions(num) != null)
			++num;

		transList = new EZTransitionList[num];

		for (int i = 0; i < transList.Length; ++i)
		{
			transList[i] = new EZTransitionList();
			ctl.GetTransitions(i).CopyToNew(transList[i], true);
			transList[i].MarkAllInitialized();
		}

		Debug.Log(num + " transition lists Copied");
	}

	static void CopyPanel(UIPanelBase panel)
	{
		transList = new EZTransitionList[1];
		transList[0] = new EZTransitionList();
		panel.Transitions.CopyToNew(transList[0], true);
		transList[0].MarkAllInitialized();

		Debug.Log("Transitions Copied");
	}

	static void PasteControl(IControl ctl)
	{
		int i;
		for (i = 0; i < transList.Length; ++i)
		{
			if (ctl.GetTransitions(i) == null)
				break;

			transList[i].CopyTo(ctl.GetTransitions(i), true);
			transList[i].MarkAllInitialized();
		}

		EditorUtility.SetDirty(((MonoBehaviour)ctl).gameObject);

		Debug.Log(i + " transition lists Pasted");
	}

	static void PastePanel(UIPanelBase panel)
	{
		transList[0].CopyTo(panel.Transitions, true);
		transList[0].MarkAllInitialized();

		EditorUtility.SetDirty(panel.gameObject);

		Debug.Log("Transitions Pasted");
	}
}
