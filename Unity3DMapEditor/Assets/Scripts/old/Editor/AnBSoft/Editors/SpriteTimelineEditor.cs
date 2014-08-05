//-----------------------------------------------------------------
//	SpriteTimelineEditor v1.0 (1-25-2010)
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


// Uncomment to display texture names in the timeline:
// #define SHOW_TEX_NAMES

using UnityEditor;
using UnityEngine;
using System.Collections;


public class TextureAnimSettingsTracker
{
	string name;
	int loopCycles;
	bool loopReverse;
	float framerate;

	public bool Synch(TextureAnim t)
	{
		bool changed = false;

		if (name != t.name)
			changed = true;
		if (loopCycles != t.loopCycles)
			changed = true;
		if (loopReverse != t.loopReverse)
			changed = true;
		if (framerate != t.framerate)
			changed = true;

		name = t.name;
		loopCycles = t.loopCycles;
		loopReverse = t.loopReverse;
		framerate = t.framerate;

		return changed;
	}
}


public class SpriteTimelineEditor : EditorWindow
{
	static SpriteTimelineEditor instance;
	static SpriteTimeline timelineInstance;
	bool closing = false;
	Rect wndRect;


	[UnityEditor.MenuItem("Window/Sprite Timeline &t")]
	static public void ShowEditor()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (instance != null)
		{
			instance.Show(true);
			return;
		}

		instance = new SpriteTimelineEditor();

		if(timelineInstance == null)
			timelineInstance = new SpriteTimeline();

		// Get the selected object and see if it has a sprite:
		timelineInstance.Editor = instance;
		timelineInstance.SetupSelection();
		instance.position = new Rect(200, 200, 700, 400);
		timelineInstance.position = instance.position;
		timelineInstance.SetupRects();

		instance.autoRepaintOnSceneChange = true;
		instance.Show(true);
#else
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}

		instance = (SpriteTimelineEditor)EditorWindow.GetWindow(typeof(SpriteTimelineEditor), false, "Sprite Timeline");

		if (timelineInstance == null)
			timelineInstance = new SpriteTimeline();

		timelineInstance.Editor = instance;
		/*
				timelineInstance.SetupSelection();
				timelineInstance.SetupRects();
		*/
		timelineInstance.position = instance.position;

		instance.ShowUtility();
#endif
		instance.wndRect = instance.position;
	}



	public void OnDisable()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		this.Close();
#endif
	}

	public void OnCloseWindow()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		instance = null;
		closing = true;
		DestroyImmediate(this);
#endif
	}


	// Relay events to our timeline editor, if available:
	void OnSelectionChange()
	{
		if (timelineInstance != null)
			timelineInstance.OnSelectionChange();
	}

	void Update()
	{
		// If we're receiving an update but our instance is null,
		// we need to show our window again:
		if (instance == null)
		{
			if (!closing)
				ShowEditor();
			else
				return;
		}

		if (timelineInstance != null)
			if (timelineInstance.STEUpdate())
				Repaint();
	}

	void OnFocus()
	{
		if (timelineInstance != null)
			timelineInstance.OnFocus();
	}

	void OnHierarchyChange()
	{
		if (timelineInstance != null)
			timelineInstance.OnHierarchyChange();
	}

	void OnGUI()
	{
		float dummy;

		// Bailout if we don't have valid values:
		if (instance == null)
			return;

		if (timelineInstance != null)
		{
			if (wndRect != position)
			{
				wndRect = position;
				timelineInstance.position = wndRect;
			}

			if (timelineInstance.STEOnGUI(0, out dummy))
				Repaint();
		}
	}
}


// Contains the actual implementation of drawing the timeline and
// managing its contents/settings:
public class SpriteTimeline : ISTE
{
	static DraggableTextureComparer textureComparer = new DraggableTextureComparer(); // Used to sort our textures when dropped
	ISpritePackable sprite;
	GameObject selGO; // The currently-selected GameObject, if any
	EditorWindow m_editor;
	bool nonWindow;	// Is this a non-window associated instance?
	GUIStyle centeredLabel = new GUIStyle();

	string[] animList = new string[0];

	// Array of texture frames (DraggableTextures)
	ArrayList frames = new ArrayList();
	Texture2D staticTexture;	// Refers to the sprite's static texture

	int selectedAnim;
	int newSelectedAnim;


	// Rect positions:
	Rect addBtnRect;
	Rect delBtnRect;
	Rect popupRect;
	Rect nameLabel;
	Rect nameRect;
	Rect loopCyclesRect;
	Rect loopReverseRect;
	Rect fpsLabelRect;
	Rect fpsRect;
	Rect durationLabel;
	Rect durationRect;
	Rect previewButtonRect;
	Rect clearRect;
	Rect staticTexRect;
	Rect timelineRect;
	Rect timelineViewportRect;
	Vector2 scrollPos = Vector2.zero;
	Vector2 outerScrollPos = Vector2.zero;
	Rect m_position;	// Rect for the entire window
	float nonWndAdjust; // Adjustment for drawing frames in the timeline for a non-window timeline


	float timelineTop = 80f;
	static float spaceBetweenFrames = 5f;
	static int spaceAbovePreview = 10;
	float zoomCoef = 1f;

	int animWidth, animHeight;
	DraggableTexture selectedFrame = null;

	Vector2 mousePos;
	TextureAnimSettingsTracker animSettingsTracker = new TextureAnimSettingsTracker();
	bool doRepaint = false;
	bool previewing = false;
	float timeSinceLastFrameUpdate = 0;
	float lastTimeCheck = 0;
	int curPreviewTexture;
	int stepDir = 1;


	public Rect position
	{
		get { return m_position; }
		set { m_position = value; }
	}

	public EditorWindow Editor
	{
		get { return m_editor; }
		set { m_editor = value; }
	}



	public void Setup(Rect wnd, float top)
	{
		nonWindow = true;

		position = wnd;

		SetupSelection();

		// Adjust our rects:
		// 		addBtnRect = new Rect(5, 5, 30, 20);
		// 		delBtnRect = new Rect(5, 30, 30, 20);
		// 		popupRect = new Rect(40, 5, 140, 20);
		// 		nameRect = new Rect(220, 5, 140, 20);

		// Move the buttons left to align with the
		// possible state name box:
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		float diff = Mathf.Abs(loopCyclesRect.x - 20f);
#else
		float diff = Mathf.Abs(loopCyclesRect.x - 37f);
#endif
		loopCyclesRect.x -= diff;
		loopReverseRect.x -= diff;
		fpsLabelRect.x -= diff;
		fpsRect.x -= diff;
		durationRect.x -= diff;

		// Move the buttons up to replace the top row:
		diff = previewButtonRect.height + 5f;
		loopCyclesRect.y -= diff;
		loopReverseRect.y -= diff;
		fpsLabelRect.y -= diff;
		fpsRect.y -= diff;
		durationLabel.y -= diff;
		durationLabel.x -= 20;
		durationRect.y -= diff;

		previewButtonRect.y = durationRect.y;
		previewButtonRect.x = durationRect.xMax + 5f;
		//		staticTexRect = new Rect(40, 50, 200, 20);

		// Position the timeline's top edge:
		timelineTop = previewButtonRect.yMax + 5f;
	}


	public void SetupRects()
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		addBtnRect = new Rect(5, 5, 30, 20);
		delBtnRect = new Rect(5, 30, 30, 20);
		popupRect = new Rect(40, 5, 140, 20);
		nameLabel = new Rect(220, 5, 33, 20);
		nameRect = new Rect(255, 5, 74, 20);
		loopCyclesRect = new Rect(40, 30, 120, 20);
		loopReverseRect = new Rect(170, 30, 90, 20);
		fpsLabelRect = new Rect(275, 30, 30, 20);
		fpsRect = new Rect(299, 30, 30, 20);
		durationLabel = new Rect(350, 30, 60, 20);
		durationRect = new Rect(400, 30, 40, 20);
		previewButtonRect = new Rect(400, 5, 60, 20);
		clearRect = new Rect(5, 50, 30, 20);
		staticTexRect = new Rect(40, 50, 200, 20);
#else
		addBtnRect = new Rect(5, 5, 20, 20);
		delBtnRect = new Rect(24, 5, 20, 20);
#if	(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		popupRect = new Rect(50, 5, 240, 20);
		nameLabel = new Rect(293, 5, 40, 20);
		nameRect = new Rect(330, 5, 134, 20);
#else
		popupRect = new Rect(15, 5, 220, 20);
		nameLabel = new Rect(253, 5, 40, 20);
		nameRect = new Rect(330, 5, 134, 20);
#endif
		loopCyclesRect = new Rect(50, 30, 200, 20);
		loopReverseRect = new Rect(260, 30, 90, 20);
		fpsLabelRect = new Rect(365, 30, 30, 20);
		fpsRect = new Rect(390, 30, 30, 20);
		durationLabel = new Rect(430, 30, 60, 20);
		durationRect = new Rect(484, 30, 40, 20);
		previewButtonRect = new Rect(300, 55, 60, 20);
		staticTexRect = new Rect(50, 55, 200, 20);
#endif
	}

	public void SetCurAnim(int index)
	{
		// If this is different,
		if (index != selectedAnim)
		{
			// Make sure the anim timeline is updated:
			selectedAnim = -1;
			newSelectedAnim = index;
		}
	}

	public void OnSelectionChange()
	{
		SetupSelection();
	}

	public void OnFocus()
	{
		SetupSelection();
	}

	public void OnHierarchyChange()
	{
		SetupSelection();
	}

	public SpriteTimeline()
	{
		SetupRects();
		centeredLabel.alignment = TextAnchor.MiddleCenter;
	}

	/*
		void OnEnable()
		{
	#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			addBtnRect = new Rect(5, 5, 30, 30);
			delBtnRect = new Rect(24, 5, 20, 20);
			popupRect = new Rect(15, 5, 220, 20);
			nameRect = new Rect(190, 5, 225, 20);
			loopCyclesRect = new Rect(15, 30, 120, 20);
			loopReverseRect = new Rect(150, 30, 90, 20);
			fpsLabelRect = new Rect(265, 30, 30, 20);
			fpsRect = new Rect(290, 30, 30, 20);
			durationRect = new Rect(280, 30, 135, 20);
			previewButtonRect = new Rect(425, 30, 60, 20);
	#else
			addBtnRect = new Rect(5, 5, 20, 20);
			delBtnRect = new Rect(24, 5, 20, 20);
			popupRect = new Rect(15, 5, 220, 20);
			nameRect = new Rect(190, 5, 225, 20);
			loopCyclesRect = new Rect(15, 30, 120, 20);
			loopReverseRect = new Rect(150, 30, 90, 20);
			fpsLabelRect = new Rect(265, 30, 30, 20);
			fpsRect = new Rect(290, 30, 30, 20);
			durationRect = new Rect(280, 30, 135, 20);
			previewButtonRect = new Rect(425, 30, 60, 20);
	#endif
		}
	*/


	public bool STEUpdate()
	{
		if (previewing && sprite != null)
		{
			if (selectedAnim < sprite.States.Length)
			{
				if (sprite.States[selectedAnim].frameGUIDs.Length > 0)
				{
					// Clamp our time value so that it is never negative:
					timeSinceLastFrameUpdate = Mathf.Max(0, timeSinceLastFrameUpdate);

					// See if it is time to show a new frame:
					if (timeSinceLastFrameUpdate >= (1f / sprite.States[selectedAnim].framerate))
					{
						timeSinceLastFrameUpdate = 0;
						doRepaint = true;

						// Move to the next texture,
						curPreviewTexture += stepDir;

						// Loop reverse, if necessary:
						if (curPreviewTexture >= sprite.States[selectedAnim].frameGUIDs.Length ||
							curPreviewTexture < 0)
						{
							// Figure out which one it is:
							if (curPreviewTexture > 0) // we reached the end
							{
								if (sprite.States[selectedAnim].loopReverse)
								{
									curPreviewTexture = Mathf.Max(sprite.States[selectedAnim].frameGUIDs.Length - 2, 0);
									stepDir = -1;
								}
								else
								{
									// Just wrap to the beginning:
									curPreviewTexture = 0;
								}
							}
							else
							{
								curPreviewTexture = 1;
								stepDir = 1;
							}
						}
					}
					else
					{
						timeSinceLastFrameUpdate += Mathf.Max(0, Time.realtimeSinceStartup - lastTimeCheck);
					}

					lastTimeCheck = Time.realtimeSinceStartup;

					curPreviewTexture %= sprite.States[selectedAnim].frameGUIDs.Length;
				}
				else
					previewing = false;
			}
			else
				previewing = false;
		}
		else
			previewing = false;

		return doRepaint;
	}


	// Returns whether a repaint is needed
	public bool STEOnGUI(float adjust, out float bottom)
	{
		bool prevDrag, flipRepaint;
		DraggableTexture toFront = null;
		doRepaint = false;
		flipRepaint = false;
		Color oldColor;

		bottom = timelineTop;


#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		EditorGUIUtility.UseControlStyles();
#endif


		// See if we need to update our selection:
		if (Selection.activeGameObject != selGO)
			SetupSelection();


		if (!nonWindow)
		{
			// Bailout if we don't have valid values:
			if (sprite == null)
			{
				GUI.Label(new Rect(0, 0, position.width, position.height), "No packable sprite selected", centeredLabel);
				return false;
			}

			// Setup our window rect:
			m_position = position;
		}
		else
		{
			if (sprite == null)
				return false;
		}

		// Handle certain input events:
		switch (Event.current.type)
		{
			case EventType.KeyDown:
				// Check to make sure we aren't intercepting
				// text input:
				if (GUIUtility.keyboardControl == 0)
				{
					// Handle our supported keys:
					switch (Event.current.keyCode)
					{
						case KeyCode.Backspace:
						case KeyCode.Delete:
							// Delete the currently selected frame:
							if (selectedFrame != null)
							{
								DeleteSelectedFrame();
								Event.current.Use();
							}
							break;
						case KeyCode.D:
							// Check for a "duplicate" command:
							//if(Event.current.control || Event.current.command)
							{
								// Duplicate the selected frame:
								if (selectedFrame != null)
								{
									DuplicateSelectedFrame();
									Event.current.Use();
								}
							}
							break;
						case KeyCode.RightArrow:
							// Step the selection right:
							if (selectedFrame != null)
								if (!selectedFrame.Dragging)
								{
									ChangeSelectedFrame((DraggableTexture)frames[Mathf.Clamp(selectedFrame.index + 1, 0, frames.Count - 1)]);
									Event.current.Use();
								}
							break;
						case KeyCode.LeftArrow:
							// Step the selection left:
							if (selectedFrame != null)
								if (!selectedFrame.Dragging)
								{
									ChangeSelectedFrame((DraggableTexture)frames[Mathf.Clamp(selectedFrame.index - 1, 0, frames.Count - 1)]);
									Event.current.Use();
								}
							break;
					}
				}
				else if (Event.current.keyCode == KeyCode.Return)
				{
					// Prevent multi-line input in the text field:
					Event.current.Use();
					// Unset the keyboard focus:
					GUIUtility.keyboardControl = 0;
				}
				break;


			case EventType.ScrollWheel:
				zoomCoef -= Event.current.delta.y * 0.1f;
				zoomCoef = Mathf.Clamp(zoomCoef, 0.2f, 4f);
				SetFramePositions();
				break;
		}



		if (sprite.SupportsArbitraryAnimations)
		{
			// Button to add a new animation:
			if (GUI.Button(addBtnRect, new GUIContent("+", null, "Add a new animation")))
			{
				AddAnimation();
			}


			// Button to delete the current animation:
			if (GUI.Button(delBtnRect, new GUIContent("-", null, "Delete this animation")))
			{
				if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete the animation \"" + sprite.States[selectedAnim].name + "\"?", "Yes", "No"))
				{
					DeleteCurrentAnimation();
					if (animList.Length < 1)
						return false;
				}
			}
		}



		// Draw the static texture selection box:
		if (sprite is PackedSprite)
		{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			// Draw a "clear" button:
			if(GUI.Button(clearRect, "X"))
				((PackedSprite)sprite).staticTexGUID = System.Guid.Empty.ToString();
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
			staticTexture = (Texture2D)EditorGUI.ObjectField(staticTexRect, "Static Texture:", AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(((PackedSprite)sprite).staticTexGUID), typeof(Texture2D)), typeof(Texture2D), false);
#else
			staticTexture = (Texture2D)EditorGUI.ObjectField(staticTexRect, "Static Texture:", AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(((PackedSprite)sprite).staticTexGUID), typeof(Texture2D)), typeof(Texture2D));
#endif
			((PackedSprite)sprite).staticTexPath = AssetDatabase.GetAssetPath(staticTexture);
			((PackedSprite)sprite).staticTexGUID = AssetDatabase.AssetPathToGUID(((PackedSprite)sprite).staticTexPath);
		}



		// Stop here if there are no animations:
		if (animList == null || sprite.States.Length < 1)
			return false;
		if (animList.Length < 1)
			return false;


		////////////////////////////////////////////
		// Draw the interface:
		////////////////////////////////////////////
		outerScrollPos = GUILayout.BeginScrollView(outerScrollPos);
		// Tell our scroll view we want this to be the minimum
		// view area:
		GUILayout.Box(GUIContent.none, GUI.skin.label, GUILayout.Width(previewButtonRect.xMax), GUILayout.Height(animHeight + timelineTop + 10));

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		oldColor = GUI.contentColor;
		GUI.contentColor = Color.black;
#endif



		if (sprite.SupportsArbitraryAnimations)
		{
			// Clamp the selected animation:
			selectedAnim = Mathf.Clamp(selectedAnim, 0, sprite.States.Length - 1);
			// Draw the controls across the top of the window:
			newSelectedAnim = EditorGUI.Popup(popupRect, "Animation:", selectedAnim, animList);
		}


		// If the selected animation changed, or
		if (newSelectedAnim != selectedAnim)
		{
			// Rebuild our frame list:
			selectedAnim = newSelectedAnim;
			zoomCoef = 1f;	// Reset the zoom
			RebuildFrameList();
			animSettingsTracker.Synch(sprite.States[selectedAnim]);

			// Remove any keyboard focus:
			GUIUtility.keyboardControl = 0;
		}	// if our frame list appears out of synch:
		else if (sprite.States[selectedAnim].frameGUIDs.Length != frames.Count)
		{
			RebuildFrameList();
		}


		if (sprite.SupportsArbitraryAnimations)
		{
			GUI.Label(nameLabel, "Name:");
			sprite.States[selectedAnim].name = EditorGUI.TextField(nameRect, sprite.States[selectedAnim].name);
			animList[selectedAnim] = selectedAnim.ToString() + " - " + sprite.States[selectedAnim].name;
		}



		// Only draw this if there are frames:
		if (sprite.States[selectedAnim].frameGUIDs.Length > 0)
		{
			// Don't allow NaN or Infinity going into the UI:
			if (float.IsNaN(sprite.States[selectedAnim].framerate) ||
				float.IsInfinity(sprite.States[selectedAnim].framerate))
				sprite.States[selectedAnim].framerate = 0;

			sprite.States[selectedAnim].loopCycles = Mathf.Max(-1, EditorGUI.IntField(loopCyclesRect, "Loop Cycles:", sprite.States[selectedAnim].loopCycles));
			sprite.States[selectedAnim].loopReverse = GUI.Toggle(loopReverseRect, sprite.States[selectedAnim].loopReverse, "Loop Reverse");
			GUI.Label(fpsLabelRect, "FPS:");
			sprite.States[selectedAnim].framerate = Mathf.Max(0, EditorGUI.FloatField(fpsRect, sprite.States[selectedAnim].framerate));
			float duration = ((float)sprite.States[selectedAnim].frameGUIDs.Length) / sprite.States[selectedAnim].framerate;
			GUI.Label(durationLabel, "Duration:");
			sprite.States[selectedAnim].framerate = Mathf.Max(0, sprite.States[selectedAnim].frameGUIDs.Length / EditorGUI.FloatField(durationRect, float.IsNaN(duration) ? Mathf.Infinity : duration));
			previewing = GUI.Toggle(previewButtonRect, previewing, "Preview", GUI.skin.button);

			// Don't allow NaN or Infinity:
			if (float.IsNaN(sprite.States[selectedAnim].framerate) ||
				float.IsInfinity(sprite.States[selectedAnim].framerate))
				sprite.States[selectedAnim].framerate = 0;
		}

		// See if any settings were changed:
		if (animSettingsTracker.Synch(sprite.States[selectedAnim]))
			EditorUtility.SetDirty(sprite.gameObject);


#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		GUI.contentColor = oldColor;
#endif

		// Draw the timeline:
		animWidth = (int)(((float)GetSelectedAnimWidth((int)spaceBetweenFrames)) * zoomCoef);
		animHeight = 20 + (int)(((float)(GetSelectedAnimMaxHeight())) * zoomCoef);
		timelineRect = new Rect(0, timelineTop, animWidth, animHeight);
		timelineViewportRect = new Rect(0, timelineTop, Mathf.Max(m_position.width, previewButtonRect.xMax), animHeight + 10);

		GUILayout.BeginArea(timelineViewportRect, GUI.skin.box);

		scrollPos = GUILayout.BeginScrollView(scrollPos);

		// Makes our scroll bars work:
		GUILayout.BeginHorizontal();
		GUILayout.Box(GUIContent.none, GUI.skin.label, GUILayout.Width(timelineRect.width));
		GUILayout.EndHorizontal();

		// Draw the animation frames:
		DraggableTexture texture;

		// Set the gui matrix for zooming:
		//Matrix4x4 oldGUIMatrix = GUI.matrix;
		//GUI.matrix = Matrix4x4.TRS(new Vector3(0, (timelineTop+24f) * (1f-zoomCoef), 0), Quaternion.identity, new Vector3(zoomCoef, zoomCoef, zoomCoef));

		for (int i = 0; i < frames.Count; ++i)
		{
			texture = (DraggableTexture)frames[i];

			prevDrag = texture.Dragging;

			// If we are previewing, draw a box behind this frame
			// if it is the currently-displayed frame:
			if (previewing)
			{
				if (curPreviewTexture == i)
				{
					oldColor = GUI.contentColor;
					GUI.color = new Color(0, 1, 0, 0.5f);
					GUI.Box(new Rect(texture.Position.x, texture.Position.y, texture.texture.width * zoomCoef, texture.texture.height * zoomCoef), GUIContent.none);
					GUI.color = oldColor;
				}
			}

			// Highlight this frame if it is the selected frame:
			if (selectedFrame == texture)
			{
				texture.selected = true;
			}

			texture.Scale = zoomCoef;

			texture.OnGUI((nonWindow ? adjust : 0));

			if (texture.Dragging)
			{
				ChangeSelectedFrame(texture);

				doRepaint = true;

				// Move this texture to the front:
				if (frames.IndexOf(texture) != frames.Count - 1)
				{
					toFront = texture;
				}
			}// Else, the frame was dropped, so reorder and refresh:
			else if (prevDrag)
			{
				// Reorder the textures according to their x-position:
				ReorderTextures();

				// Update the selected texture index now that it has been reordered:
				curPreviewTexture = texture.index;

				flipRepaint = true;
			}
		}
		//GUI.matrix = oldGUIMatrix;
		GUILayout.EndScrollView();

		GUILayout.EndArea();

		// Move the dragging object to the front:
		if (toFront != null)
		{
			frames.Remove(toFront);
			frames.Add(toFront);
			curPreviewTexture = frames.Count - 1;
			toFront.index = curPreviewTexture;
		}

		// Update our bottom extent:
		bottom = timelineViewportRect.yMax + spaceAbovePreview;


		// Draw our preview:
		if (sprite.States[selectedAnim].frameGUIDs.Length > 0)
		{
			// Make sure we're in a safe range incase we just deleted a frame:
			selectedAnim = Mathf.Clamp(selectedAnim, 0, sprite.States.Length - 1);
			curPreviewTexture = Mathf.Clamp(curPreviewTexture, 0, sprite.States[selectedAnim].frameGUIDs.Length - 1);

			// Set the sprite color:
			oldColor = GUI.contentColor;
			GUI.contentColor = sprite.Color;

			// Paint the texture:
			Texture2D tex = ((DraggableTexture)frames[curPreviewTexture]).texture;

			if (tex != null)
			{
				Rect previewRect = CalcPreviewRect(tex);
				GUI.Label(previewRect, tex);

				// Print the frame number:
				previewRect.y = previewRect.yMax + 5f;
				previewRect.height = 20f;
				// Ensure it is big enough to hold our text:
				if (previewRect.width < 70f)
				{
					previewRect.width = 70f;
					previewRect.x = (m_position.width / 2f) - 35f;
				}

				// Only draw the frame number if we aren't dragging:
				if(!((DraggableTexture)frames[curPreviewTexture]).Dragging)
					GUI.Label(previewRect, "Frame: " + curPreviewTexture, centeredLabel);

				bottom = previewRect.yMax + spaceAbovePreview;
			}

			GUI.contentColor = oldColor;
		}

		// End our outer scroll view:
		GUILayout.EndScrollView();


		// Handle drag and drop from an outside source:
		EventType eventType = Event.current.type;
		if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
		{
			// Ensure the drag/drop is within our timeline area:
			if (Event.current.mousePosition.y >= timelineTop)
			{
				// Show a copy icon on the drag
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (eventType == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					AppendTextures(DragAndDrop.objectReferences);
					flipRepaint = true;
				}

				Event.current.Use();
			}
		}



		return flipRepaint;
	}


	public void SetupSelection()
	{
		bool somethingChanged = false;

		// See if something changed:
		if (sprite != null)
		{
			try
			{
				if (sprite.gameObject != Selection.activeGameObject)
					somethingChanged = true;
				else if (sprite.States.Length != animList.Length)
					somethingChanged = true;
				else if (sprite.States.Length < 1)
					somethingChanged = true;
				else if (selectedAnim >= sprite.States.Length || selectedAnim < 0)
					somethingChanged = true;
				else if (sprite.States[selectedAnim].frameGUIDs.Length != frames.Count)
					somethingChanged = true;
			}
			catch
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
				sprite = (ISpritePackable)Selection.activeGameObject.GetComponent("ISpritePackable");
				selGO = Selection.activeGameObject;
				zoomCoef = 1f;
			}
			else
			{
				// If another GameObject wasn't selected,
				// don't change anything so we can drag
				// textures in, etc.
				// So only change if there is no activeObject
				// (as opposed to activeGameObject):
				if(Selection.activeObject == null)
				{
					selGO = null;
					sprite = null;
				}
			}

			// Select a safe value for our selected frame:
			if (sprite != null)
			{
				if (sprite.States.Length > 0)
				{
					selectedAnim = Mathf.Clamp(selectedAnim, 0, sprite.States.Length - 1);
					animSettingsTracker.Synch(sprite.States[selectedAnim]);
				}

				// Commented out because two different sprites with
				// the same number of frames in their selected anim
				// would cause the list not to be updated:
				//if (sprite.States.Length != animList.Length)
				BuildAnimList();

				if (sprite.States.Length > 0)
				{
					selectedAnim = Mathf.Clamp(selectedAnim, 0, sprite.States.Length - 1);
					if (sprite.States[selectedAnim].frameGUIDs.Length != frames.Count)
						RebuildFrameList();

					// Only change the selected frame if it is not valid:
					if (selectedFrame != null)
						if (selectedFrame.index >= sprite.States[selectedAnim].frameGUIDs.Length)
						{
							selectedFrame = null;
						}
				}
				else
					selectedFrame = null;
			}
			else
			{
				selectedFrame = null;
			}

			if (m_editor != null)
				m_editor.Repaint();
		}
	}

	int GetSelectedAnimWidth(int padding)
	{
		int total = 0;
		for (int i = 0; i < frames.Count; ++i)
		{
			if (((DraggableTexture)frames[i]).texture != null)
				total += ((DraggableTexture)frames[i]).texture.width + padding;
			else
				total += DraggableTexture.NULL_SIZE + padding;
		}
		return total;
	}

	int GetSelectedAnimMaxHeight()
	{
		int max = 0;
		for (int i = 0; i < frames.Count; ++i)
		{
			if (((DraggableTexture)frames[i]).texture != null)
			{
				if (max < ((DraggableTexture)frames[i]).texture.height)
					max = ((DraggableTexture)frames[i]).texture.height;
			}
			else
			{
				if (max < DraggableTexture.NULL_SIZE)
					max = DraggableTexture.NULL_SIZE;
			}
		}
		return max;
	}

	// Returns the width of the widest frame in the animation.
	int GetSelectedAnimMaxWidth()
	{
		int max = 0;
		for (int i = 0; i < frames.Count; ++i)
		{
			if (((DraggableTexture)frames[i]).texture != null)
			{
				if (max < ((DraggableTexture)frames[i]).texture.width)
					max = ((DraggableTexture)frames[i]).texture.width;
			}
			else
			{
				if (max < DraggableTexture.NULL_SIZE)
					max = DraggableTexture.NULL_SIZE;
			}
		}
		return max;
	}

	Rect CalcPreviewRect(Texture2D tex)
	{
		if (sprite == null)
			return new Rect(0, 0, 0, 0);

		int maxWidth = GetSelectedAnimMaxWidth();
		int maxHeight = GetSelectedAnimMaxHeight();
		int clampedMaxWidth = Mathf.Min(maxWidth, (int)m_position.width);
		int clampedMaxHeight = Mathf.Min(maxHeight, ((int)m_position.height) - (((int)timelineViewportRect.yMax) + spaceAbovePreview));
		int left = ((int)m_position.width) / 2 - clampedMaxWidth / 2;
		int top = (int)timelineViewportRect.yMax + spaceAbovePreview;

		// Adjust the clamped max width and height 
		// according to which has been reduced the most:
		float widthReduction = ((float)clampedMaxWidth) / ((float)maxWidth);
		float heightReduction = ((float)clampedMaxHeight) / ((float)maxHeight);

		if (widthReduction < heightReduction)
			clampedMaxHeight = (int)(((float)maxHeight) * widthReduction);
		else
			clampedMaxWidth = (int)(((float)maxWidth) * heightReduction);

		int clampedWidth = (int)(((float)tex.width) * (((float)clampedMaxWidth) / ((float)maxWidth)));
		int clampedHeight = (int)(((float)tex.height) * (((float)clampedMaxHeight) / ((float)maxHeight)));

		// X-position, when we want it centered:
		int centeredX = left + (clampedMaxWidth - clampedWidth) / 2;

		// Y-position, when we want it centered vertically:
		int centeredY = top + (clampedMaxHeight - clampedHeight) / 2;

		switch (sprite.Anchor)
		{
			case SpriteBase.ANCHOR_METHOD.UPPER_LEFT:
				return new Rect(left, top, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.UPPER_CENTER:
				return new Rect(centeredX, top, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.UPPER_RIGHT:
				return new Rect(left + clampedMaxWidth - clampedWidth, top, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.MIDDLE_LEFT:
				return new Rect(left, centeredY, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.MIDDLE_CENTER:
				return new Rect(centeredX, centeredY, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.MIDDLE_RIGHT:
				return new Rect(left + clampedMaxWidth - clampedWidth, centeredY, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.BOTTOM_LEFT:
				return new Rect(left, top + clampedMaxHeight - clampedHeight, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.BOTTOM_CENTER:
				return new Rect(centeredX, top + clampedMaxHeight - clampedHeight, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.BOTTOM_RIGHT:
				return new Rect(left + clampedMaxWidth - clampedWidth, top + clampedMaxHeight - clampedHeight, clampedWidth, clampedHeight);
			case SpriteBase.ANCHOR_METHOD.TEXTURE_OFFSET:
				return new Rect(centeredX, centeredY, clampedWidth, clampedHeight);
			default:
				return new Rect(0, 0, 0, 0);
		}
		/*
				new Rect(wndRect.width / 2f - tex.width / 2f,
													timelineViewportRect.yMax + spaceAbovePreview,
													tex.width, Mathf.Min(tex.height, wndRect.height - (timelineViewportRect.yMax + spaceAbovePreview)));
		*/
	}

	void ChangeSelectedFrame(DraggableTexture tex)
	{
		// Deselect the previous frame:
		if (selectedFrame != null)
		{
			selectedFrame.selected = false;
		}

		selectedFrame = tex;
		tex.selected = true;

		// If we aren't previewing, use this
		// frame as our preview frame:
		if (!previewing)
			curPreviewTexture = selectedFrame.index;
	}

	void BuildAnimList()
	{
		if (sprite == null)
			return;

		animList = new string[sprite.States.Length];

		for (int i = 0; i < animList.Length; ++i)
		{
			animList[i] = i.ToString() + " - " + sprite.States[i].name;
		}

		selectedAnim = Mathf.Min(selectedAnim, animList.Length - 1);

		RebuildFrameList();
	}

	void SetFramePositions()
	{
		float leftEdge = 0;

		for (int i = 0; i < frames.Count; ++i)
		{
			DraggableTexture tex = (DraggableTexture)frames[i];
			tex.Position = new Vector2(leftEdge, 0);

			if (tex.texture != null)
				leftEdge += (tex.texture.width * zoomCoef) + spaceBetweenFrames;
			else
				leftEdge += DraggableTexture.NULL_SIZE * zoomCoef + spaceBetweenFrames;
		}
	}

	void RebuildFrameList()
	{
		frames.Clear();

		float leftEdge = 0;
		bool matchedSelection = false;
		Texture2D frameTex;

		if (sprite.States.Length < 1)
			return;

		selectedAnim = Mathf.Clamp(selectedAnim, 0, sprite.States.Length - 1);

		for (int i = 0; i < sprite.States[selectedAnim].frameGUIDs.Length; ++i)
		{
			frameTex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(sprite.States[selectedAnim].frameGUIDs[i]), typeof(Texture2D));
			DraggableTexture tex = new DraggableTexture(frameTex, i, new Vector2(leftEdge, 0));

			if (tex.texture != null)
				leftEdge += (tex.texture.width * zoomCoef) + spaceBetweenFrames;
			else
				leftEdge += DraggableTexture.NULL_SIZE + spaceBetweenFrames;

			frames.Add(tex);

			// Attempt to preserve our selection:
			if (selectedFrame != null)
				if (tex.index == selectedFrame.index && tex.texture == selectedFrame.texture)
				{
					selectedFrame = tex;
					matchedSelection = true;
				}
		}

		if (!matchedSelection)
			selectedFrame = null;
	}

	void ReorderTextures()
	{
		frames.Sort(textureComparer);

		float leftEdge = 0;
		bool changed = false;

		// Now assign them back to the sprite in this order:
		for (int i = 0; i < frames.Count; ++i)
		{
			sprite.States[selectedAnim].frameGUIDs[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(((DraggableTexture)frames[i]).texture));
			if (((DraggableTexture)frames[i]).index != i)
				changed = true;

			// Update the index of the selected texture:
			if (curPreviewTexture == ((DraggableTexture)frames[i]).index)
				curPreviewTexture = i;

			((DraggableTexture)frames[i]).index = i;
			((DraggableTexture)frames[i]).Position = new Vector2(leftEdge, 0);

			if (((DraggableTexture)frames[i]).texture != null)
				leftEdge += (((DraggableTexture)frames[i]).texture.width * zoomCoef) + spaceBetweenFrames;
			else
				leftEdge += DraggableTexture.NULL_SIZE * zoomCoef + spaceBetweenFrames;
		}

		if (changed)
			EditorUtility.SetDirty(sprite.gameObject);
	}

	void AppendTextures(Object[] objs)
	{
		if (sprite.States.Length < 1)
			return;

		ArrayList newFrameList = new ArrayList();	// Array of paths
		ArrayList sortedNewList = new ArrayList();	// Array of new Texture2Ds

		// Get the existing array:
		newFrameList.AddRange(sprite.States[selectedAnim].frameGUIDs);

		// Now iterate through the newly dropped objects and verify they are
		// textures:
		for (int i = 0; i < objs.Length; ++i)
		{
			if (objs[i] is Texture2D)
			{
				sortedNewList.Add(objs[i]);
			}
		}

		// Sort the new textures alphabetically:
		sortedNewList.Sort(new TextureNameComparer());

		// Now append these onto the list:
		for (int i = 0; i < sortedNewList.Count; ++i)
			newFrameList.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath((Texture2D)sortedNewList[i])));

		// Now dump it back to the original array:
		sprite.States[selectedAnim].frameGUIDs = (string[])newFrameList.ToArray(typeof(string));

		RebuildFrameList();
		EditorUtility.SetDirty(sprite.gameObject);
	}

	void AddAnimation()
	{
		TextureAnim[] temp = sprite.States;

		sprite.States = new TextureAnim[sprite.States.Length + 1];
		temp.CopyTo(sprite.States, 0);
		sprite.States[sprite.States.Length - 1] = new TextureAnim();
		sprite.States[sprite.States.Length - 1].frameGUIDs = new string[0];
		sprite.States[sprite.States.Length - 1].name = "New Animation";
		sprite.States[sprite.States.Length - 1].framerate = 30f;
		sprite.States[sprite.States.Length - 1].loopCycles = 0;
		sprite.States[sprite.States.Length - 1].loopReverse = false;

		animList = new string[0];
		SetupSelection();
		frames.Clear();

		selectedAnim = sprite.States.Length - 1;

		EditorUtility.SetDirty(sprite.gameObject);
	}

	void DeleteCurrentAnimation()
	{
		TextureAnim[] newList = new TextureAnim[sprite.States.Length - 1];

		for (int i = 0, j = 0; i < sprite.States.Length; ++i)
		{
			if (i != selectedAnim)
			{
				newList[j] = sprite.States[i];
				++j;
			}
			else
				continue;
		}

		sprite.States = newList;

		SetupSelection();

		EditorUtility.SetDirty(sprite.gameObject);
	}

	void DeleteSelectedFrame()
	{
		// Now move each frame after the one we're removing down one slot:
		for (int i = selectedFrame.index; i < sprite.States[selectedAnim].frameGUIDs.Length - 1; ++i)
		{
			sprite.States[selectedAnim].frameGUIDs[i] = sprite.States[selectedAnim].frameGUIDs[i + 1];
		}

		selectedFrame = null;

		// Save our old list:
		string[] temp = sprite.States[selectedAnim].frameGUIDs;
		// Create a new one that is one shorter:
		sprite.States[selectedAnim].frameGUIDs = new string[temp.Length - 1];

		if (sprite.States[selectedAnim].frameGUIDs.Length > 0)
		{
			// Copy the old contents back, sans the last element:
			System.Array.Copy(temp, sprite.States[selectedAnim].frameGUIDs, temp.Length - 1);
		}

		RebuildFrameList();
		EditorUtility.SetDirty(sprite.gameObject);
	}

	void DuplicateSelectedFrame()
	{
		// Copy the array (reference):
		string[] temp = sprite.States[selectedAnim].frameGUIDs;
		// Make a new, empty array:
		sprite.States[selectedAnim].frameGUIDs = new string[temp.Length + 1];

		for (int i = 0, offset = 0; (i - offset) < temp.Length; ++i)
		{
			sprite.States[selectedAnim].frameGUIDs[i] = temp[i - offset];
			if (temp[i - offset] == AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selectedFrame.texture)))
				offset = 1;
		}

		RebuildFrameList();
		EditorUtility.SetDirty(sprite.gameObject);
	}
}


public class DraggableTexture : GUIDraggableObject
{
	public const int NULL_SIZE = 40;	// Size of the box we'll display if our texture is null

	public Texture2D texture;
	public int index;
	public bool selected;
	protected float scale = 1f;

	public float Scale
	{
		get { return scale; }
		set
		{
			scale = value;
		}
	}

	public DraggableTexture(Texture2D tex, int idx, Vector2 position)
		: base(position)
	{
		texture = tex;
		index = idx;
	}

	public DraggableTexture(Texture2D tex, int idx)
		: base(Vector2.zero)
	{
		texture = tex;
		index = idx;
	}

	public void OnGUI(float adjust)
	{
		Rect drawRect, dragRect;

		if (texture == null)
			drawRect = new Rect(m_Position.x, m_Position.y, NULL_SIZE * scale, NULL_SIZE * scale);
		else
			drawRect = new Rect(m_Position.x, m_Position.y, texture.width * scale, texture.height * scale);

		// Apply adjustment:
		if (Dragging)
		{
			drawRect.y += adjust;
		}

		// If the object is selected, draw a box around it:
		if (selected)
		{
			Color oldBGColor = GUI.backgroundColor;
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			GUI.backgroundColor = new Color(1f, 0, 1f, 1f);
#else
			GUI.backgroundColor = new Color(1f, 0, 1f, 0.5f);
#endif
			GUI.Box(drawRect, GUIContent.none);

			GUI.backgroundColor = oldBGColor;
		}

		if (texture != null)
		{
			// Draw the texture:
			GUI.DrawTexture(drawRect, texture);
		}
		else
		{
			Color oldBGColor = GUI.backgroundColor;
			Color oldContentColor = GUI.contentColor;

			if (selected)
				GUI.backgroundColor = new Color(1f, 0, 1f, 0.5f);
			else
				GUI.backgroundColor = Color.gray;
			GUI.contentColor = Color.black;
			GUI.Box(drawRect, "NONE");
			GUI.backgroundColor = oldBGColor;
			GUI.contentColor = oldContentColor;
		}

		dragRect = drawRect;
		//dragRect = new Rect(dragRect.x /*+ m_Position.x*/, dragRect.y /*+ m_Position.y*/, dragRect.width, dragRect.height);


#if SHOW_TEX_NAMES
		// Draw the texture name:
		drawRect = new Rect(drawRect.left, drawRect.yMax, drawRect.width, 20);
		GUILayout.BeginArea(drawRect);
			GUILayout.Label(texture.name, GUILayout.MaxWidth(drawRect.width));
		GUILayout.EndArea();
#endif

		Drag(dragRect);
	}
}

// Tip o' the hat to AngryAnt:
public class GUIDraggableObject
{
	protected Vector2 m_Position;
	private Vector2 m_DragStart;
	private bool m_Dragging;

	public GUIDraggableObject(Vector2 position)
	{
		m_Position = position;
	}

	public bool Dragging
	{
		get
		{
			return m_Dragging;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_Position;
		}

		set
		{
			m_Position = value;
		}
	}

	public void Drag(Rect draggingRect)
	{
		if (Event.current.type == EventType.MouseUp)
		{
			m_Dragging = false;
		}
		else if (Event.current.type == EventType.MouseDown && draggingRect.Contains(Event.current.mousePosition))
		{
			m_Dragging = true;
			m_DragStart = Event.current.mousePosition - m_Position;
			Event.current.Use();

			// Remove keyboard focus from any existing control:
			GUIUtility.keyboardControl = 0;
		}

		if (m_Dragging)
		{
			m_Position = Event.current.mousePosition - m_DragStart;
		}
	}
}


// Compares texture X-coordinates
public class DraggableTextureComparer : IComparer
{
	static DraggableTexture t1;
	static DraggableTexture t2;

	int IComparer.Compare(object a, object b)
	{
		t1 = (DraggableTexture)a;
		t2 = (DraggableTexture)b;

		if (t1.Position.x > t2.Position.x)
			return 1;
		else if (t1.Position.x < t2.Position.x)
			return -1;
		else
			return 0;
	}
}

// Compares texture names alphabetically:
public class TextureNameComparer : IComparer
{
	static Texture2D t1;
	static Texture2D t2;

	int IComparer.Compare(object a, object b)
	{
		t1 = (Texture2D)a;
		t2 = (Texture2D)b;

		AlphanumComparatorFast comparer = new AlphanumComparatorFast();

		return comparer.Compare(t1.name, t2.name);
	}
}

// Does an alphanumeric comparison of two strings:
public class AlphanumComparatorFast : IComparer
{
	public int Compare(object x, object y)
	{
		string s1 = x as string;
		if (s1 == null)
		{
			return 0;
		}
		string s2 = y as string;
		if (s2 == null)
		{
			return 0;
		}

		int len1 = s1.Length;
		int len2 = s2.Length;
		int marker1 = 0;
		int marker2 = 0;

		// Walk through two the strings with two markers.
		while (marker1 < len1 && marker2 < len2)
		{
			char ch1 = s1[marker1];
			char ch2 = s2[marker2];

			// Some buffers we can build up characters in for each chunk.
			char[] space1 = new char[len1];
			int loc1 = 0;
			char[] space2 = new char[len2];
			int loc2 = 0;

			// Walk through all following characters that are digits or
			// characters in BOTH strings starting at the appropriate marker.
			// Collect char arrays.
			do
			{
				space1[loc1++] = ch1;
				marker1++;

				if (marker1 < len1)
				{
					ch1 = s1[marker1];
				}
				else
				{
					break;
				}
			} while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

			do
			{
				space2[loc2++] = ch2;
				marker2++;

				if (marker2 < len2)
				{
					ch2 = s2[marker2];
				}
				else
				{
					break;
				}
			} while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

			// If we have collected numbers, compare them numerically.
			// Otherwise, if we have strings, compare them alphabetically.
			string str1 = new string(space1);
			string str2 = new string(space2);

			int result;

			if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
			{
				int thisNumericChunk = int.Parse(str1);
				int thatNumericChunk = int.Parse(str2);
				result = thisNumericChunk.CompareTo(thatNumericChunk);
			}
			else
			{
				result = str1.CompareTo(str2);
			}

			if (result != 0)
			{
				return result;
			}
		}
		return len1 - len2;
	}
}
