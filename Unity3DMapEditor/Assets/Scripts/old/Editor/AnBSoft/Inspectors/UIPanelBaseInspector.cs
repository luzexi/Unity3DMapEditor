//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;

// Only compile if not using Unity iPhone
#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)

[CustomEditor(typeof(UIPanelBase))]
public class UIPanelBaseInspector : Editor, IGUIHelper
{
	protected int curTrans;
	protected int curTransElement;

	// Working vars:
	protected UIPanelBase panel;
	protected bool isDirty = false;



	// Begins watching the GUI for changes
	protected void BeginMonitorChanges()
	{
		GUI.changed = false;
	}

	// Determines if something changed
	protected void EndMonitorChanges()
	{
		if (GUI.changed)
			isDirty = true;
	}



	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		isDirty = false;

		panel = (UIPanelBase)target;

		GUILayout.BeginVertical();




		//-----------------------------------------
		// Draw our transition stuff:
		//-----------------------------------------
		if (panel.Transitions != null)
			if (panel.Transitions.list != null)
				if (panel.Transitions.list.Length > 0)
					DoTransitionStuff();




		GUILayout.Space(10f);

		GUILayout.EndVertical();

		// Set dirty if anything changed:
		if (isDirty)
			EditorUtility.SetDirty(panel);
	}


	// Draws all our transition-related UI stuff:
	void DoTransitionStuff()
	{
		//-------------------------------		
		// Draw a nice separator:
		//-------------------------------		
		GUILayout.Space(5.0f);
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

		GUILayout.BeginVertical("Toolbar");
		GUI.backgroundColor = backgroundColor;

		GUILayout.BeginHorizontal();
		GUILayout.Space(15.0f);

		GUILayout.Label("Trans:");

		// Draw our "From" transition selection
		// as well as our clone button:
		curTrans = EditorGUILayout.Popup(curTrans, panel.GetTransitionNames(), "toolbarPopup", GUILayout.MaxWidth(90f));
		if (GUILayout.Button("Clone", "toolbarButton"))
			if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to copy this transition to all others in this set?", "Yes", "No"))
			{
				Undo.RegisterUndo(this, "Clone transition");
				panel.Transitions.CloneAll(curTrans);
				isDirty = true;
			}

		BeginMonitorChanges();
		if(curTrans < 4)
			panel.blockInput[curTrans] = GUILayout.Toggle(panel.blockInput[curTrans], "Block Input", "toolbarButton");
		EndMonitorChanges();

		GUILayout.FlexibleSpace();

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

		GUILayout.BeginVertical("Toolbar");
		GUILayout.BeginHorizontal();

		GUI.backgroundColor = backgroundColor;

		GUILayout.Space(10f);
		GUILayout.Label("Elmnts:");


		if (GUILayout.Button("+", "toolbarButton"))
		{
			Undo.RegisterUndo(this, "Add transition element");
			curTransElement = panel.Transitions.list[curTrans].Add();
			isDirty = true;
		}
		if (panel.Transitions.list[curTrans].animationTypes.Length > 0)
			if (GUILayout.Button("-", "toolbarButton"))
			{
				Undo.RegisterUndo(this, "Delete transition element");
				panel.Transitions.list[curTrans].Remove(curTransElement);
				isDirty = true;
			}

		GUILayout.Space(20f);

		//-------------------------------		
		// End separator (maybe, depending on if we have a drop-down selection)
		//-------------------------------		


		// Make sure our selection is in a valid range:
		curTransElement = Mathf.Clamp(curTransElement, 0, panel.Transitions.list[curTrans].animationTypes.Length - 1);

		// See if we have more to draw:
		if (panel.Transitions.list[curTrans].animationTypes.Length > 0)
		{
			// Let the user select the transition element:
			curTransElement = EditorGUILayout.Popup(curTransElement, panel.Transitions.list[curTrans].GetNames(), "toolbarPopup", GUILayout.MaxWidth(110f));

			GUILayout.FlexibleSpace();

			// Cap off our toolbar-ish separator thingy
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();



			BeginMonitorChanges();

			// Draw the type selection:
			GUILayout.Space(0);
			panel.Transitions.list[curTrans].SetElementType(curTransElement, (EZAnimation.ANIM_TYPE)EditorGUILayout.EnumPopup("Type:", panel.Transitions.list[curTrans].animationTypes[curTransElement]));

			// Draw the input fields for the selected
			// type's parameters:
			panel.Transitions.list[curTrans].animParams[curTransElement].DrawGUI(panel.Transitions.list[curTrans].animationTypes[curTransElement], panel.gameObject, this, true);

			EndMonitorChanges();
		}
		else
		{
			// Pad the right end of our separator bar:
			GUILayout.FlexibleSpace();

			// Cap off our toolbar-ish separator thingy
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		// Clone this transition as needed:
		panel.Transitions.CloneAsNeeded(curTrans);
	}


	//---------------------------------------
	// IGUIHelper interface stuff:
	//---------------------------------------
	public System.Enum EnumField(string label, System.Enum selected)
	{
		return EditorGUILayout.EnumPopup(label, selected);
	}

	public Color ColorField(string label, Color color)
	{
		return EditorGUILayout.ColorField(label, color);
	}

	public Vector3 Vector3Field(string label, Vector3 val)
	{
		EditorGUIUtility.LookLikeControls();
		Vector3 v = EditorGUILayout.Vector3Field(label, val);
		EditorGUIUtility.LookLikeInspector();
		return v;
	}

	public float FloatField(string label, float val)
	{
		return EditorGUILayout.FloatField(label, val);
	}

	public string TextField(string label, string val)
	{
		return EditorGUILayout.TextField(label, val);
	}

	public Object ObjectField(string label, System.Type type, Object obj)
	{
#if UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
		return EditorGUILayout.ObjectField(label, obj, type, true, GUILayout.Width(200f));
#else
		return EditorGUILayout.ObjectField(label, obj, type, GUILayout.Width(200f));
#endif
	}
}

#else

// Keep Unity iPhone happy:
public class UIPanelBaseInspector : Editor
{}

#endif	// END iPhone exclusion
