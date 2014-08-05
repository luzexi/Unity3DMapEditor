//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;


//[CustomEditor(typeof(UIButton))]
public class UIPanelEditor : EditorWindow, IGUIHelper
{
	static UIPanelEditor instance;

	protected int curTrans;	// The current transition selected
	protected int curTransElement;

	// Working vars:
	protected UIPanelBase panel;
	protected GameObject selGO; // The currently-selected GameObject, if any
	protected bool isDirty = false;
	protected bool restarted = true;


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



	[UnityEditor.MenuItem("Window/UI Panel Editor")]
	public static void ShowEditor()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (instance != null)
		{
			instance.Show(true);
			return;
		}

		instance = new UIPanelEditor();

		// Get the selected object and see if it has a sprite:
		instance.OnSelectionChange();

		instance.position = new Rect(200, 200, 700, 400);
		instance.autoRepaintOnSceneChange = true;
		instance.Show(true);
#else
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		else
		{
			instance = (UIPanelEditor)EditorWindow.GetWindow(typeof(UIPanelEditor), false, "Panel Editor");
			instance.OnSelectionChange();
			instance.ShowUtility();
		}
#endif
	}


	public void OnSelectionChange()
	{
		bool somethingChanged = false;

		if (panel != null)
		{
			if (panel.gameObject != Selection.activeGameObject)
			{
				somethingChanged = true;
			}
		}
		else
			somethingChanged = true;

		if (somethingChanged)
		{
			if (Selection.activeGameObject != null)
			{
				panel = (UIPanelBase)Selection.activeGameObject.GetComponent(typeof(UIPanelBase));
				selGO = Selection.activeGameObject;
				curTrans = 0;
				curTransElement = 0;
			}
			else
			{
				selGO = null;
				panel = null;
			}
		}

		Repaint();
	}


	public void OnGUI()
	{
		bool needRepaint = false;
		isDirty = false;

		if (restarted)
		{
			selGO = null;
			panel = null;
			OnSelectionChange();
			restarted = false;
		}

		// See if we need to update our selection:
		if (Selection.activeGameObject != selGO)
			OnSelectionChange();

//#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if(Selection.activeGameObject != null)
			panel = (UIPanelBase)Selection.activeGameObject.GetComponent(typeof(UIPanelBase));
//#endif

		// Bailout if we don't have valid values:
		if (panel == null)
		{
			PrintNoSelectMsg();
			return;
		}


		//-----------------------------------------
		// Draw our transition stuff:
		//-----------------------------------------
		if (panel.Transitions != null)
			if (panel.Transitions.list != null)
				if (panel.Transitions.list.Length > 0)
					DoTransitionStuff();



		GUILayout.Space(10f);

		// Set dirty if anything changed:
		if (isDirty)
		{
			EditorUtility.SetDirty(panel);
		}

		if (needRepaint)
			Repaint();
	}


	// Draws all transition-related UI stuff:
	void DoTransitionStuff()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			// Box off our transition fields area:
			GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));
			GUILayout.FlexibleSpace();
#endif

		//-------------------------------		
		// Draw a nice separator:
		//-------------------------------		
		GUILayout.Space(5.0f);
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			GUILayout.BeginVertical(GUILayout.MaxWidth(200f));
#else
		GUILayout.BeginVertical("Toolbar");
#endif
		GUI.backgroundColor = backgroundColor;

		GUILayout.BeginHorizontal();
		GUILayout.Space(10.0f);

		GUILayout.Label("Transitions: ");

		// Draw our "From" transition selection
		// as well as our clone button:
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		Color oldColor = GUI.contentColor;
		GUI.contentColor = Color.black;
		curTrans = EditorGUILayout.Popup(curTrans, panel.GetTransitionNames(), GUILayout.MaxWidth(100f));

		if (GUILayout.Button("Clone"))
			if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to copy this transition to all others in this set?", "Yes", "No"))
			{
				panel.Transitions.CloneAll(curTrans);
				isDirty = true;
			}
		
		BeginMonitorChanges();
		
		if(curTrans < 4)
			panel.blockInput[curTrans] = GUILayout.Toggle(panel.blockInput[curTrans], "Block Input");

		EndMonitorChanges();

		GUI.contentColor = oldColor;
#else
		curTrans = EditorGUILayout.Popup(curTrans, panel.GetTransitionNames(), "toolbarPopup", GUILayout.MaxWidth(100f));
		if (GUILayout.Button("Clone", "toolbarButton"))
			if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to copy this transition to all others in this set?", "Yes", "No"))
			{
				Undo.RegisterUndo(this, "Clone transition");
				panel.Transitions.CloneAll(curTrans);
				isDirty = true;
			}

		BeginMonitorChanges();

		if (curTrans < 4)
			panel.blockInput[curTrans] = GUILayout.Toggle(panel.blockInput[curTrans], "Block Input", "toolbarButton");

		EndMonitorChanges();
#endif


		GUILayout.Space(53f);
		GUILayout.Label("Elements:");


#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (GUILayout.Button("+"))
		{
			curTransElement = panel.Transitions.list[curTrans].Add();
			isDirty = true;
		}
		if(panel.Transitions.list[curTrans].animationTypes.Length > 0)
			if (GUILayout.Button("-"))
			{
				panel.Transitions.list[curTrans].Remove(curTransElement);
				isDirty = true;
			}
#else
		if (GUILayout.Button("+", "toolbarButton"))
		{
			Undo.RegisterUndo(this, "Add transition element");
			curTransElement = panel.Transitions.list[curTrans].Add();
			isDirty = true;
		}
		if(panel.Transitions.list[curTrans].animationTypes.Length > 0)
			if (GUILayout.Button("-", "toolbarButton"))
			{
				Undo.RegisterUndo(this, "Delete transition element");
				panel.Transitions.list[curTrans].Remove(curTransElement);
				isDirty = true;
			}
#endif

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
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				oldColor = GUI.contentColor;
				GUI.contentColor = Color.black;
				curTransElement = EditorGUILayout.Popup(curTransElement, panel.Transitions.list[curTrans].GetNames(), GUILayout.MaxWidth(110f));
#else
			curTransElement = EditorGUILayout.Popup(curTransElement, panel.Transitions.list[curTrans].GetNames(), "toolbarPopup", GUILayout.MaxWidth(110f));
#endif


#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			GUILayout.FlexibleSpace();
#endif

			// Cap off our toolbar-ish separator thingy
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();


			GUILayout.BeginHorizontal();



			BeginMonitorChanges();

			// Draw the type selection:
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				GUILayout.Space(10f);
				panel.Transitions.list[curTrans].SetElementType(curTransElement, (EZAnimation.ANIM_TYPE) EditorGUILayout.Popup("Type:", (int)panel.Transitions.list[curTrans].animationTypes[curTransElement], System.Enum.GetNames(typeof(EZAnimation.ANIM_TYPE)), GUILayout.Width(150f)));
				GUILayout.Space(20f);
#else
			GUILayout.Space(0);
			panel.Transitions.list[curTrans].SetElementType(curTransElement, (EZAnimation.ANIM_TYPE)EditorGUILayout.EnumPopup("Type:", panel.Transitions.list[curTrans].animationTypes[curTransElement], GUILayout.Width(220f)));
#endif
			// Draw the input fields for the selected
			// type's parameters:
			panel.Transitions.list[curTrans].animParams[curTransElement].DrawGUI(panel.Transitions.list[curTrans].animationTypes[curTransElement], panel.gameObject, this, false);
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				GUI.contentColor = oldColor;
#endif
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
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			GUILayout.EndArea();
#endif
		// Clone this transition as needed:
		panel.Transitions.CloneAsNeeded(curTrans);
	}


	void PrintNoSelectMsg()
	{
		GUIStyle centeredLabel = new GUIStyle();
		centeredLabel.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, position.height / 2 - 20, position.width, 20), "No UIPanelBase selected", centeredLabel);
	}


	void OnEnable()
	{
		restarted = true;
	}





	//---------------------------------------
	// IGUIHelper interface stuff:
	public System.Enum EnumField(string label, System.Enum selected)
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		GUILayout.Label(label);
		return (System.Enum) System.Enum.ToObject(selected.GetType(), EditorGUILayout.Popup((int)selected, System.Enum.GetNames(selected.GetType()), GUILayout.Width(150f)));
#else
		return EditorGUILayout.EnumPopup(label, selected, GUILayout.Width(220f));
#endif
	}

	public Color ColorField(string label, Color color)
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		float r, g, b, a;
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10f);
		r = Mathf.Clamp01(EditorGUILayout.FloatField("R:", color.r, GUILayout.Width(50f)));
		g = Mathf.Clamp01(EditorGUILayout.FloatField("G:", color.g, GUILayout.Width(50f)));
		b = Mathf.Clamp01(EditorGUILayout.FloatField("B:", color.b, GUILayout.Width(50f)));
		a = Mathf.Clamp01(EditorGUILayout.FloatField("A:", color.a, GUILayout.Width(50f)));
		EditorGUILayout.EndHorizontal();

		return new Color(r,g,b,a);
#else
		return EditorGUILayout.ColorField(label, color);
#endif
	}

	public Vector3 Vector3Field(string label, Vector3 val)
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		return EditorGUILayout.Vector3Field(label, val, GUILayout.Width(200f));
#else
		return EditorGUILayout.Vector3Field(label, val);
#endif
	}

	public float FloatField(string label, float val)
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		float width = 100f;
		GUILayout.Label(label);
		return EditorGUILayout.FloatField(val, GUILayout.Width(width));
#else
		float width = 130f;
		return EditorGUILayout.FloatField(label, val, GUILayout.Width(width));
#endif
	}

	public string TextField(string label, string val)
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		float width = 170f;
		GUILayout.Label(label);
		return EditorGUILayout.TextField(val, GUILayout.Width(width));
#else
		float width = 200f;
		return EditorGUILayout.TextField(label, val, GUILayout.Width(width));
#endif
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