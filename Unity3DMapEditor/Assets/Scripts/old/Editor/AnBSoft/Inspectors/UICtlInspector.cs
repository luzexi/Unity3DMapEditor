//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;


// Only compile if not using Unity iPhone
#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)

public class UICtlInspector : Editor, IGUIHelper
{

	// Will hold the state names for the control
	protected string[] stateNames = new string[0];
	protected int curState;
	protected int curFromTrans;
	protected int curTransElement;

	// Working vars:
	protected ASCSEInfo stateInfo;
	protected IControl control;
	protected bool isDirty = false;
	protected EZTransitionList transitions;



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


	// Override this in subclasses to add settings that
	// will get drawn before the state selection block:
	public virtual void DrawPrestateSettings()
	{

	}


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		isDirty = false;

		control = (IControl)target;

		// Have the specific control class's implementation
		// draw any control-specific settings:
		DrawPrestateSettings();

		GUILayout.BeginVertical();

		//-------------------------------		
		// Draw a nice separator:
		//-------------------------------		
		GUILayout.Space(5.0f);

		GUILayout.BeginVertical("Toolbar");

		GUILayout.BeginHorizontal();
		GUILayout.Space(10.0f);
		GUILayout.Label("State/Element: ");
		GUILayout.FlexibleSpace();

		
		
		// Start keeping track of any changed values:
		BeginMonitorChanges();
		
		// Do the pre-state selection GUI, if any:
		control.DrawPreStateSelectGUI(curState, true);

		EndMonitorChanges();



		// Get the control's state names:
		stateNames = control.EnumStateElements();

		if (stateNames == null)
			return;

		// Cap our state to the number of states available:
		curState = Mathf.Min(curState, stateNames.Length - 1);

		// Choose the state we want to edit:
		curState = EditorGUILayout.Popup(curState, stateNames);

		GUILayout.FlexibleSpace();

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		//-------------------------------		
		// End separator
		//-------------------------------		


		// Keep track of any changed values:
		BeginMonitorChanges();

		// Do the post-state selection GUI, if any:
		control.DrawPostStateSelectGUI(curState);

		EndMonitorChanges();


		if (control is IPackableControl)
			ShowSpriteSettings();
		else
			stateInfo = control.GetStateElementInfo(curState);

		transitions = stateInfo.transitions;


		//-----------------------------------------
		// Draw our state label stuff:
		//-----------------------------------------
		if (stateInfo.stateLabel != null)
		{
			BeginMonitorChanges();
			DoStateLabel();
			EndMonitorChanges();
		}


		//-----------------------------------------
		// Draw our transition stuff:
		//-----------------------------------------
		if (transitions != null)
			if (transitions.list != null)
				if (transitions.list.Length > 0)
					DoTransitionStuff();



		GUILayout.Space(10f);

		GUILayout.EndVertical();

		// Set dirty if anything changed:
		if (isDirty)
			EditorUtility.SetDirty((MonoBehaviour)control);
	}


	void ShowSpriteSettings()
	{
		IPackableControl cont = (IPackableControl)control;

		// Get the info for this state/element:
		stateInfo = cont.GetStateElementInfo(curState);

		// Put up a texture drop box:
		// Load the texture:
		if (stateInfo.stateObj.frameGUIDs.Length < 1)
		{
			stateInfo.tex = null;
		}
		else
			stateInfo.tex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(stateInfo.stateObj.frameGUIDs[0]), typeof(Texture2D));


		BeginMonitorChanges();
		// Select the texture for the state:
#if UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
		stateInfo.tex = (Texture2D)EditorGUILayout.ObjectField(stateNames[curState], stateInfo.tex, typeof(Texture2D), false);
#else
		stateInfo.tex = (Texture2D)EditorGUILayout.ObjectField(stateNames[curState], stateInfo.tex, typeof(Texture2D));
#endif
		EndMonitorChanges();


		// Re-assign the state info to the control:
		// If we have an available frame, assign the new GUID
		if (stateInfo.stateObj.frameGUIDs.Length > 0)
		{
			stateInfo.stateObj.frameGUIDs[0] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(stateInfo.tex));
		} // Else only create a new GUID element if the selection is non-null:
		else if (stateInfo.tex != null)
		{
			stateInfo.stateObj.frameGUIDs = new string[1];
			stateInfo.stateObj.frameGUIDs[0] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(stateInfo.tex));
		} // Else, don't do anything. Period.

		transitions = stateInfo.transitions;
	}


	// Draws the state label selection
	void DoStateLabel()
	{
		control.SetStateLabel(curState, EditorGUILayout.TextField("Label:", stateInfo.stateLabel));
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

		// Clamp our current "From" transition selection:
		curFromTrans = Mathf.Clamp(curFromTrans, 0, transitions.list.Length - 1);

		// Draw our "From" transition selection
		// as well as our clone button:
		curFromTrans = EditorGUILayout.Popup(curFromTrans, transitions.GetTransitionNames(), "toolbarPopup", GUILayout.MaxWidth(90f));
		if (GUILayout.Button("Clone", "toolbarButton"))
			if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to copy this transition to all others in this set?", "Yes", "No"))
			{
				Undo.RegisterUndo(this, "Clone transition");
				transitions.CloneAll(curFromTrans);
				isDirty = true;
			}

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
			curTransElement = transitions.list[curFromTrans].Add();
			isDirty = true;
		}
		if (transitions.list[curFromTrans].animationTypes.Length > 0)
			if (GUILayout.Button("-", "toolbarButton"))
			{
				Undo.RegisterUndo(this, "Delete transition element");
				transitions.list[curFromTrans].Remove(curTransElement);
				isDirty = true;
			}

		GUILayout.Space(20f);

		//-------------------------------		
		// End separator (maybe, depending on if we have a drop-down selection)
		//-------------------------------		


		// Make sure our selection is in a valid range:
		curTransElement = Mathf.Clamp(curTransElement, 0, transitions.list[curFromTrans].animationTypes.Length - 1);

		// See if we have more to draw:
		if (transitions.list[curFromTrans].animationTypes.Length > 0)
		{
			// Let the user select the transition element:
			curTransElement = EditorGUILayout.Popup(curTransElement, transitions.list[curFromTrans].GetNames(), "toolbarPopup", GUILayout.MaxWidth(110f));

			GUILayout.FlexibleSpace();

			// Cap off our toolbar-ish separator thingy
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();



			BeginMonitorChanges();

			// Draw the type selection:
			GUILayout.Space(0);
			transitions.list[curFromTrans].SetElementType(curTransElement, (EZAnimation.ANIM_TYPE)EditorGUILayout.EnumPopup("Type:", transitions.list[curFromTrans].animationTypes[curTransElement]));

			// Draw the input fields for the selected
			// type's parameters:
			transitions.list[curFromTrans].animParams[curTransElement].DrawGUI(transitions.list[curFromTrans].animationTypes[curTransElement], ((MonoBehaviour)control).gameObject, this, true);

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
		transitions.CloneAsNeeded(curFromTrans);
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
public class UICtlInspector : Editor
{
}


#endif	// END iPhone exclusion
