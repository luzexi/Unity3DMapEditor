//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;



//[CustomEditor(typeof(UIButton))]
public class UICtlEditor : EditorWindow, IGUIHelper, IGUIScriptSelector
{
	Rect texRect = new Rect(45, 20, 200, 200);
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	Rect clearRect = new Rect(10, 20, 30, 15);
#endif
	const float ctlLeft = 10;
	const float ctlIndent = 30f;
	const float ctlHeight = 15f;
	const float ctlVirtSpace = 2f;
	Rect wndRect;
	static UICtlEditor instance;

	// Will hold the state names for the control
	protected string[] stateNames = new string[0];
	protected int curState;
	protected int curFromTrans;	// The current "from" transition selected
	protected int curTransElement;

	// Our timeline editor (if available)
	ISTE ste;

	// Working vars:
	protected ASCSEInfo stateInfo;
	protected EZTransitionList transitions;
	protected Rect tempRect;
	protected IControl control;
	protected GameObject selGO; // The currently-selected GameObject, if any
	protected int height;
	protected bool isDirty = false;
	protected float textureAreaBottom = 0;
	protected bool restarted = true;
	protected MonoBehaviour[] scripts = new MonoBehaviour[0];
	protected string[] scriptNames = new string[0];
	bool needRepaint = false;



	[UnityEditor.MenuItem("Window/UI Control Editor")]
	public static void ShowEditor()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (instance != null)
		{
			instance.Show(true);
			return;
		}

		instance = new UICtlEditor();

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
			instance = (UICtlEditor)EditorWindow.GetWindow(typeof(UICtlEditor), false, "Control Editor");
			instance.OnSelectionChange();
			instance.ShowUtility();
		}
#endif
	}

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

	public void OnSelectionChange()
	{
		bool somethingChanged = false;

		if (null != (MonoBehaviour)control)
		{
			if (((MonoBehaviour)control).gameObject != Selection.activeGameObject)
			{
				somethingChanged = true;
			}
		}
		else
			somethingChanged = true;

		if(somethingChanged)
		{
			if (Selection.activeGameObject != null)
			{
				control = (IControl)Selection.activeGameObject.GetComponent("IControl");
				selGO = Selection.activeGameObject;
				curState = 0;	// Reset the selected state
				curFromTrans = 0;
				curTransElement = 0;
			}
			else if (Selection.activeObject == null)
			{
				// Else only unset stuff if we don't have an active object of any kind
				// so that we can still select textures, etc:
				selGO = null;
				control = null;
			}
		}

		if (ste != null)
			ste.OnSelectionChange();

		Repaint();
	}


	public void OnGUI()
	{
		needRepaint = false;
		int oldState = curState;
		textureAreaBottom = 0;
		isDirty = false;

		if(restarted)
		{
			selGO = null;
			control = null;
			OnSelectionChange();
			restarted = false;
		}

		// See if our window size has changed:
		if (wndRect != position)
			WindowResized();

		// See if we need to update our selection:
		if (Selection.activeGameObject != selGO)
			OnSelectionChange();

//#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if(Selection.activeGameObject != null)
			control = (IControl)Selection.activeGameObject.GetComponent("IControl");
//#endif

		// Bailout if we don't have valid values:
		if (null == (MonoBehaviour)control)
		{
			// See if this is a scroll list:
			UIScrollList list = null;
			if (Selection.activeGameObject != null)
				list = Selection.activeGameObject.GetComponent<UIScrollList>();

			if (list != null)
			{
				list.DrawPreTransitionUI(0, this);
				return;
			}
			else
			{
				PrintNoSelectMsg();
				return;
			}
		}



		// Start keeping track of any changed values:
		BeginMonitorChanges();

		// Do the pre-state selection GUI, if any:
		height = control.DrawPreStateSelectGUI(curState, false);

		EndMonitorChanges();



		// Get the control's state names:
		stateNames = control.EnumStateElements();
		if (stateNames == null)
			return;

		// Cap our state to the number of states available:
		if (stateNames != null)
			curState = Mathf.Min(curState, stateNames.Length - 1);
		else
			curState = 0;

		// Choose the state we want to edit:
		curState = GUILayout.Toolbar(curState, stateNames);

		// Reset our selected transition element 
		// if the state selection changed:
		if (curState != oldState)
		{
			curFromTrans = 0;
			curTransElement = 0;
		}


		
		
		// Keep track of any changed values:
		BeginMonitorChanges();

		// Do the post-state selection GUI, if any:
		height += control.DrawPostStateSelectGUI(curState);

		EndMonitorChanges();


		// Adjust our texture selection rect:
		tempRect = texRect;
		tempRect.y += height;



		if (control is IPackableControl)
			ShowSpriteSettings();
		else
			stateInfo = control.GetStateElementInfo(curState);

		transitions = stateInfo.transitions;





		if (!Application.isPlaying)
		{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			// Box off our script selection and transition fields area:
			GUILayout.BeginArea(new Rect(0, textureAreaBottom, position.width, position.height-textureAreaBottom));
			GUILayout.FlexibleSpace();
#endif
			//-----------------------------------------
			// Draw script selection:
			//-----------------------------------------
			BeginMonitorChanges();
			control.DrawPreTransitionUI(curState, this);
			EndMonitorChanges();


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



#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		// End the boxed off area for our script selection and transition fields.
		GUILayout.EndArea();
#endif
		}


		GUILayout.BeginVertical();
		GUILayout.Space(10f);
		GUILayout.EndVertical();

		// Set dirty if anything changed:
		if (isDirty)
		{
			EditorUtility.SetDirty((MonoBehaviour)control);
		}

		if (needRepaint)
			Repaint();
	}


	void ShowSpriteSettings()
	{
		IPackableControl cont = (IPackableControl)control;

		// Get the info for this state/element:
		stateInfo = cont.GetStateElementInfo(curState);


		// See if the sprite timeline is available:
		if (ste == null)
		{
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(UICtlEditor));
			System.Type steType = asm.GetType("SpriteTimeline");
			if (steType != null)
			{
				ste = (ISTE)System.Activator.CreateInstance(steType);
				ste.Setup(position, 0);
			}
		}

		// NOW see if the timeline is available:
		if (ste != null)
		{
			ste.SetCurAnim(curState);
			needRepaint = ste.STEOnGUI(-(20f + height), out textureAreaBottom);
		}
		else
		{
			// Put up a texture drop box:
			// Load the texture:
			if (stateInfo.stateObj.frameGUIDs.Length < 1)
			{
				stateInfo.stateObj.frameGUIDs = new string[1] { "" };
			}
			stateInfo.tex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(stateInfo.stateObj.frameGUIDs[0]), typeof(Texture2D));


			BeginMonitorChanges();
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			// Draw a "clear" button:
			if(GUI.Button(new Rect(clearRect.x, clearRect.y + height, clearRect.width, clearRect.height), "X"))
				stateInfo.tex = null;

			// Select the texture for the state:
			stateInfo.tex = (Texture2D)EditorGUI.ObjectField(tempRect, stateInfo.tex, typeof(Texture2D));
			textureAreaBottom = tempRect.yMax + ctlVirtSpace;
#else
			// Select the texture for the state:
	#if UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
			stateInfo.tex = (Texture2D)EditorGUILayout.ObjectField(stateNames[curState], stateInfo.tex, typeof(Texture2D), false);
	#else
			stateInfo.tex = (Texture2D)EditorGUILayout.ObjectField(stateNames[curState], stateInfo.tex, typeof(Texture2D));
	#endif
			textureAreaBottom = 0; // This tells us to use Layout.
#endif

			// Handle drag and drop from an outside source:
			EventType eventType = Event.current.type;
			if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
			{
				// Validate what is being dropped:
				if (DragAndDrop.objectReferences[0] is Texture2D)
				{
					// Show a copy icon on the drag
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (eventType == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						stateInfo.tex = (Texture2D)DragAndDrop.objectReferences[0];
						needRepaint = true;
					}

					Event.current.Use();
				}
			}

			EndMonitorChanges();

			// Re-assign the state info to the control:
			stateInfo.stateObj.frameGUIDs[0] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(stateInfo.tex));
		}
	}


	// Draws the state label selection
	void DoStateLabel()
	{
		//-------------------------------		
		// Draw a nice separator:
		//-------------------------------		
		GUILayout.BeginVertical();
		GUILayout.Space(5.0f);
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		GUILayout.BeginVertical(GUILayout.MaxWidth(200f));
		EditorGUIUtility.UseControlStyles();
#else
		GUILayout.BeginVertical("Toolbar");
#endif
		GUI.backgroundColor = backgroundColor;

		GUILayout.BeginHorizontal();


#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		Color oldColor = GUI.contentColor;
		GUI.contentColor = Color.black;
		GUILayout.Space(10f);
		GUILayout.Label("Label:");
		control.SetStateLabel(curState, EditorGUILayout.TextField(stateInfo.stateLabel, GUILayout.Width(250f)));

		GUI.contentColor = oldColor;
#else
		control.SetStateLabel(curState, EditorGUILayout.TextField("Label:", stateInfo.stateLabel, GUILayout.Width(250f)));
		GUILayout.FlexibleSpace();
#endif
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndVertical();
	}


	// Draws all transition-related UI stuff:
	void DoTransitionStuff()
	{
		//-------------------------------		
		// Draw a nice separator:
		//-------------------------------		
		GUILayout.Space(5.0f);
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		GUILayout.BeginVertical(GUILayout.MaxWidth(200f));
		EditorGUIUtility.UseControlStyles();
#else
		GUILayout.BeginVertical("Toolbar");
#endif
		GUI.backgroundColor = backgroundColor;

		GUILayout.BeginHorizontal();
		GUILayout.Space(10.0f);

		GUILayout.Label("Transitions: ");

		// Clamp our current "From" transition selection:
		curFromTrans = Mathf.Clamp(curFromTrans, 0, transitions.list.Length - 1);

		// Draw our "From" transition selection
		// as well as our clone button:
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		Color oldColor = GUI.contentColor;
		GUI.contentColor = Color.black;
		curFromTrans = EditorGUILayout.Popup(curFromTrans, transitions.GetTransitionNames(), GUILayout.MaxWidth(90f));

		if (GUILayout.Button("Clone"))
			if(EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to copy this transition to all others in this set?", "Yes", "No"))
			{
				transitions.CloneAll(curFromTrans);
				isDirty = true;
			}

		GUI.contentColor = oldColor;
#else
		curFromTrans = EditorGUILayout.Popup(curFromTrans, transitions.GetTransitionNames(), "toolbarPopup", GUILayout.MaxWidth(90f));
		if (GUILayout.Button("Clone", "toolbarButton"))
			if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to copy this transition to all others in this set?", "Yes", "No"))
			{
				Undo.RegisterUndo(this, "Clone transition");
				transitions.CloneAll(curFromTrans);
				isDirty = true;
			}
#endif


		GUILayout.Space(53f);
		GUILayout.Label("Elements:");


#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (GUILayout.Button("+"))
		{
			curTransElement = transitions.list[curFromTrans].Add();
			isDirty = true;
		}
		if(transitions.list[curFromTrans].animationTypes.Length > 0)
			if (GUILayout.Button("-"))
			{
				transitions.list[curFromTrans].Remove(curTransElement);
				isDirty = true;
			}
#else
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
#endif

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
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				oldColor = GUI.contentColor;
				GUI.contentColor = Color.black;
				curTransElement = EditorGUILayout.Popup(curTransElement, transitions.list[curFromTrans].GetNames(), GUILayout.MaxWidth(110f));
#else
			curTransElement = EditorGUILayout.Popup(curTransElement, transitions.list[curFromTrans].GetNames(), "toolbarPopup", GUILayout.MaxWidth(110f));
#endif


#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			GUILayout.FlexibleSpace();
#endif

			// Cap off our toolbar-ish separator thingy
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();


			GUILayout.BeginHorizontal();


			// Start watching for changes:
			BeginMonitorChanges();



			// Draw the type selection:
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			GUILayout.Space(10f);
			GUILayout.Label("Type:");
			transitions.list[curFromTrans].SetElementType(curTransElement, (EZAnimation.ANIM_TYPE) EditorGUILayout.Popup((int)transitions.list[curFromTrans].animationTypes[curTransElement], System.Enum.GetNames(typeof(EZAnimation.ANIM_TYPE)), GUILayout.Width(150f)));
			GUILayout.Space(20f);
#else
			GUILayout.Space(0);
			transitions.list[curFromTrans].SetElementType(curTransElement, (EZAnimation.ANIM_TYPE)EditorGUILayout.EnumPopup("Type:", transitions.list[curFromTrans].animationTypes[curTransElement], GUILayout.Width(220f)));
#endif
			// Draw the input fields for the selected
			// type's parameters:
			transitions.list[curFromTrans].animParams[curTransElement].DrawGUI(transitions.list[curFromTrans].animationTypes[curTransElement], ((MonoBehaviour)control).gameObject, this, false);
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

		// Clone this transition as needed:
		transitions.CloneAsNeeded(curFromTrans);
	}


	// Builds a list of all scripts in the scene.
	// Returns the index of the selected script.
	int BuildScriptList(MonoBehaviour selScript)
	{
		int selIndex=0;
		ArrayList s = new ArrayList();

		Object[] objs = FindObjectsOfType(typeof(GameObject));

		// Save a null one so we can select "None":
		s.Add(null);
		
		for(int i=0; i<objs.Length; ++i)
		{
			Component[] comps = ((GameObject)objs[i]).GetComponents(typeof(MonoBehaviour));
			
			if(comps == null)
				continue;

			s.AddRange(comps);
		}

		if(s.Count < 1)
		{
			scripts = new MonoBehaviour[1];
			scriptNames = new string[] {"None"};
			return 0;
		}

		scripts = (MonoBehaviour[]) s.ToArray(typeof(MonoBehaviour));
		scriptNames = new string[scripts.Length];

		scriptNames[0] = "None";

		for(int i=1; i<scripts.Length; ++i)
		{
			scriptNames[i] = scripts[i].name + " - " + scripts[i].GetType().Name;

			if (selScript == scripts[i])
				selIndex = i;
		}

		return selIndex;
	}


	void PrintNoSelectMsg()
	{
		GUI.Label(new Rect(position.width / 2 - 67, position.height / 2 - 20, 150, 20), "No EZ GUI Control selected");
	}

	void WindowResized()
	{
		if (ste != null)
			ste.position = position;

		wndRect = position;
	}

	void OnEnable()
	{
		restarted = true;
	}

	void OnDisable()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		//this.Close();
#else

#endif
	}

	void OnCloseWindow()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
/*
		instance = null;
		closing = true;
		DestroyImmediate(this);
*/
#endif
	}


	// Relay events to our timeline editor, if available:
	void Update()
	{
/*
		if(instance == null)
		{
			if (!closing)
				ShowEditor();
			else
				return;
		}
*/

		if (ste != null)
			if (ste.STEUpdate())
				Repaint();
	}

	void OnFocus()
	{
		if (ste != null)
			ste.OnFocus();
	}

	void OnHierarchyChange()
	{
		if (ste != null)
			ste.OnHierarchyChange();
	}


	//---------------------------------------
	// IGUIHelper interface stuff:
	//---------------------------------------
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



	//---------------------------------------
	// IGUIScriptSelector interface stuff:
	//---------------------------------------

	// Draws the script selection bar
	public MonoBehaviour DrawScriptSelection(MonoBehaviour script, ref string method)
	{
		//-------------------------------		
		// Draw a nice separator:
		//-------------------------------		
		GUILayout.Space(5.0f);
		Color backgroundColor = GUI.backgroundColor;
		//GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		GUILayout.BeginVertical(GUILayout.MaxWidth(400f));
#else
		GUILayout.BeginVertical("Toolbar");
#endif
		GUI.backgroundColor = backgroundColor;

		GUILayout.BeginHorizontal();
		GUILayout.Space(10.0f);

		GUILayout.Label("Script:");
		int selScript = BuildScriptList(script);

		// Draw our script popup:
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		Color oldColor = GUI.contentColor;
		GUI.contentColor = Color.black;
		script = scripts[EditorGUILayout.Popup(selScript, scriptNames, GUILayout.Width(150f))];
		GUILayout.Space(10f);
		GUILayout.Label("Method:");
		method = EditorGUILayout.TextField(method, GUILayout.Width(175f));

		GUI.contentColor = oldColor;
#else
		script = scripts[EditorGUILayout.Popup(selScript, scriptNames, "toolbarPopup", GUILayout.MaxWidth(120f))];
		method = EditorGUILayout.TextField("Method:", method, GUILayout.Width(195f));
		GUILayout.FlexibleSpace();
#endif

		// Cap off our toolbar-ish separator thingy
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		return script;
	}

}