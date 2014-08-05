//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// Button class that allows you to toggle sequentially
/// through an arbitrary number of states.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Toggle Button")]
public class UIStateToggleBtn : AutoSpriteControlBase
{
	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			if (!value)
				DisableMe();
			else
				SetToggleState(curStateIndex);
		}
	}

	// The zero-based index of the current state
	protected int curStateIndex;

	// Tracks whether the state was changed while
	// the control was deactivated
	protected bool stateChangeWhileDeactivated = false;

	/// <summary>
	/// Returns the zero-based number/index
	/// of the current state.
	/// </summary>
	public int StateNum
	{
		get { return curStateIndex; }
	}

	/// <summary>
	/// Returns the name of the current state.
	/// </summary>
	public string StateName
	{
		get { return states[curStateIndex].name; }
	}

	/// <summary>
	/// Zero-based index of the state that 
	/// should be the default, initial state.
	/// </summary>
	public int defaultState;

	/// Array of states that this button can have.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Unnamed"),
			new TextureAnim("Disabled")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Strings to display for each state.
	[HideInInspector]
	public string[] stateLabels = new string[] { DittoString, DittoString, DittoString, DittoString };

	public override string GetStateLabel(int index)
	{
		return stateLabels[index];
	}

	public override void SetStateLabel(int index, string label)
	{
		stateLabels[index] = label;

		if (index == curStateIndex)
			UseStateLabel(index);
	}

	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			new EZTransitionList( new EZTransition[]	// First State
			{
				new EZTransition("From Prev")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From State")
			})
		};

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= transitions.Length)
			return null;
		return transitions[index];
	}

	public override EZTransitionList[] Transitions
	{
		get { return transitions; }
		set { transitions = value; }
	}

	// Helps us keep track of the previous transition:
	EZTransition prevTransition;



	public override CSpriteFrame DefaultFrame
	{
		get
		{
			if (States[defaultState].spriteFrames.Length != 0)
				return States[defaultState].spriteFrames[0];
			else
				return null;
		}
	}

	public override TextureAnim DefaultState
	{
		get
		{
			return States[defaultState];
		}
	}


	/// <summary>
	/// An array of references to sprites which will
	/// visually represent this control.  Each element
	/// (layer) represents another layer to be drawn.
	/// This allows you to use multiple sprites to draw
	/// a single control, achieving a sort of layered
	/// effect. Ex: You can use a second layer to overlay 
	/// a button with a highlight effect.
	/// </summary>
	public SpriteRoot[] layers = new SpriteRoot[0];


	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when the button changes states.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;

	/// <summary>
	/// A string containing the name of the method to be invoked.
	/// </summary>
	public string methodToInvoke = "";

	/// <summary>
	/// Sets what event should have occurred to 
	/// invoke the associated MonoBehaviour method.
	/// Defaults to TAP.
	/// </summary>
	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	/// <summary>
	/// Delay, in seconds, between the time the control is tapped
	/// and the time the method is executed.
	/// </summary>
	public float delay;

	/// <summary>
	/// Sound that will be played when the button is tapped.
	/// </summary>
	public AudioSource soundToPlay;

	/// <summary>
	/// When false, any "layer" that is set will be assumed to
	/// contain states for "Over" and "Active" so as to provide
	/// visuals for the "Hover" effect, as well as the "Active"
	/// (pressed) effect.  When true, however, the layers are
	/// assumed to only be used to mirror the control's current
	/// state/value, and will not provide "Over" and "Active"
	/// state appearances.
	/// </summary>
	public bool disableHoverEffect = false;


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[,] stateIndices;
	// Will hold the indices of the "Over" and "Active"
	// states supported by our layers:
	protected int overLayerState, activeLayerState;
	// Holds the current state of our layers:
	protected int layerState;

	
	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Check to see if we're disabled or hidden again in case
		// we were disabled by an input delegate:
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		// Toggle if the required event occurred:
		if (ptr.evt == whenToInvoke)
		{
			ToggleState();
			if (soundToPlay != null)
				soundToPlay.PlayOneShot(soundToPlay.clip);

			if (scriptWithMethodToInvoke != null)
				scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}

		// Check for over and active-related states:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (!disableHoverEffect)
					SetLayerState(overLayerState);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if (!disableHoverEffect)
					SetLayerState(activeLayerState);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
				if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD &&
					ptr.hitInfo.collider == collider &&
					!disableHoverEffect)
					SetLayerState(overLayerState);
				else
					SetLayerState(curStateIndex);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				SetLayerState(curStateIndex);
				break;
		}

		base.OnInput(ref ptr);
	}

	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected override void Awake()
	{
		base.Awake();

		curStateIndex = defaultState;
	}

	public override void Start()
	{
		if (m_started)
			return;

		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[1][];
		aggregateLayers[0] = layers;

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Add 3 additional states:
			// +1 - Disabled state
			// +1 - Over state (only supported by layers)
			// +1 - Active state (only supported by layers)
			stateIndices = new int[layers.Length, states.Length+3];

			// Assign our "Over" and "Active" layer state indices:
			overLayerState = states.Length + 1;
			activeLayerState = states.Length + 2;

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			int j, i;
			for (j = 0; j < states.Length; ++j)				
			{
				// Setup the transition:
				transitions[j].list[0].MainSubject = this.gameObject;

				for (i = 0; i < layers.Length; ++i)
				{
					if (layers[i] == null)
					{
						Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
						continue;
					}

					stateIndices[i, j] = layers[i].GetStateIndex(states[j].name);

					// Set the layer's state:
					if (stateIndices[i, curStateIndex] != -1)
						layers[i].SetState(stateIndices[i, curStateIndex]);
					else
						layers[i].Hide(true);

					// Add this as a subject of our transition for 
					// each state, as appropriate:
					if (stateIndices[i, j] != -1)
						transitions[j].list[0].AddSubSubject(layers[i].gameObject);
				}

				// Add our text as a sub-subject of our transition as well:
				if (spriteText != null)
					transitions[j].list[0].AddSubSubject(spriteText.gameObject);
			}

			// Loop once more to add our "Over" and "Active"
			// state support for our layers:
			for (i = 0; i < layers.Length; ++i)
			{
				stateIndices[i, overLayerState] = layers[i].GetStateIndex("Over");
				stateIndices[i, activeLayerState] = layers[i].GetStateIndex("Active");
			}

			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			SetToggleState(curStateIndex, true);
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIStateToggleBtn))
			return;

		UIStateToggleBtn b = (UIStateToggleBtn)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			defaultState = b.defaultState;
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = b.prevTransition;
	
			if (Application.isPlaying)
				SetToggleState(b.StateNum);
		}

		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = b.scriptWithMethodToInvoke;
			methodToInvoke = b.methodToInvoke;
			whenToInvoke = b.whenToInvoke;
			delay = b.delay;
		}

		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundToPlay = b.soundToPlay;
		}
	}


	/// <summary>
	/// Toggles the button's state to the next in the
	/// sequence and returns the resulting state number.
	/// </summary>
	public int ToggleState()
	{
		SetToggleState(curStateIndex + 1);

		return curStateIndex;
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// </summary>
	/// <param name="s">The zero-based state number/index.</param>
	/// <param name="suppressTransition">Whether or not to suppress transitions when changing states.</param>
	public virtual void SetToggleState(int s, bool suppressTransition)
	{
		curStateIndex = s % (states.Length - 1);

		// First see if we need to postpone this state
		// change for when we are active:
		if (!gameObject.active)
		{
			stateChangeWhileDeactivated = true;

			// Call our changed delegate:
			if (changeDelegate != null)
				changeDelegate(this);

			return;
		}

		this.SetState(curStateIndex);

		this.UseStateLabel(curStateIndex);

		// Recalculate our collider
		UpdateCollider();

		SetLayerState(curStateIndex);

		// Loop through each layer and set its state,
		// provided we have a valid index for that state:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (-1 != stateIndices[i, curStateIndex])
			{
				layers[i].Hide(IsHidden());
				layers[i].SetState(stateIndices[i, curStateIndex]);
			}
			else
				layers[i].Hide(true);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		if(!suppressTransition)
		{
			transitions[curStateIndex].list[0].Start();
			prevTransition = transitions[curStateIndex].list[0];
		}

		// Call our changed delegate:
		if (changeDelegate != null && !stateChangeWhileDeactivated)
			changeDelegate(this);
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// </summary>
	/// <param name="s">The zero-based state number/index.</param>
	public virtual void SetToggleState(int s)
	{
		SetToggleState(s, false);
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// Does nothing if the specified state is not found.
	/// </summary>
	/// <param name="s">The name of the desired state.</param>
	/// <param name="suppressTransition">Whether or not to suppress transitions when changing states.</param>
	public virtual void SetToggleState(string stateName, bool suppressTransition)
	{
		for(int i=0; i<states.Length; ++i)
		{
			if (states[i].name == stateName)
			{
				SetToggleState(i, suppressTransition);
				return;
			}
		}
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// Does nothing if the specified state is not found.
	/// </summary>
	/// <param name="s">The name of the desired state.</param>
	public virtual void SetToggleState(string stateName)
	{
		SetToggleState(stateName, false);
	}


	// Sets the layers to represent the current control state
	// (includes states not directly supported by the control
	// itself, such as Over and Active)
	protected void SetLayerState(int s)
	{
		// Skip if redundant:
		if (s == layerState)
			return;

		layerState = s;

		// Loop through each layer and set its state,
		// provided we have a valid index for that state:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (-1 != stateIndices[i, layerState])
			{
				layers[i].Hide(false);
				layers[i].SetState(stateIndices[i, layerState]);
			}
			else
				layers[i].Hide(true);
		}
	}

	// Sets the control to its disabled appearance:
	protected void DisableMe()
	{
		// The disabled state is the last in the states list:
		SetState(states.Length-1);

		this.UseStateLabel(states.Length - 1);

		// Set the layer states:
		for(int i=0; i<layers.Length; ++i)
		{
			if (stateIndices[i, states.Length - 1] != -1)
				layers[i].SetState(stateIndices[i, states.Length - 1]);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		transitions[states.Length - 1].list[0].Start();
		prevTransition = transitions[states.Length - 1].list[0];
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if(stateChangeWhileDeactivated)
		{
			SetToggleState(curStateIndex);
			stateChangeWhileDeactivated = false;
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool nullText = (spriteText == null);

			base.Text = value;

			// If this is a new SpriteText object, add it as a sub-subject of our transitions:
			if (nullText && spriteText != null && Application.isPlaying)
			{
				for (int i = 0; i < transitions.Length; ++i)
					for (int j = 0; j < transitions[i].list.Length; ++j)
						transitions[i].list[j].AddSubSubject(spriteText.gameObject);
			}
		}
	}

	// Sets the default UVs:
	public override void InitUVs()
	{
		if(states != null)
			if(defaultState <= states.Length-1)
				if (states[defaultState].spriteFrames.Length != 0)
					frameInfo.Copy(states[defaultState].spriteFrames[0]);

		base.InitUVs();
	}

	// Draw our state creation/deletion controls in the GUI:
	public override int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));

		// Add a new state
		if(GUILayout.Button(inspector?"+":"Add State", inspector?"ToolbarButton":"Button"))
		{
			// Insert the new state before the "disabled" state:
			List<TextureAnim> tempList = new List<TextureAnim>();
			tempList.AddRange(states);
			tempList.Insert(states.Length - 1, new TextureAnim("State " + (states.Length-1)));
			states = tempList.ToArray();

			// Add a transition to match:
			List<EZTransitionList> tempTrans = new List<EZTransitionList>();
			tempTrans.AddRange(transitions);
			tempTrans.Insert(transitions.Length - 1, new EZTransitionList( new EZTransition[] {new EZTransition("From Prev")} ) );
			transitions = tempTrans.ToArray();

			// Add a state label to match:
			List<string> tempLabels = new List<string>();
			tempLabels.AddRange(stateLabels);
			tempLabels.Insert(stateLabels.Length - 1, DittoString);
			stateLabels = tempLabels.ToArray();
		}

		// Only allow removing a state if it isn't
		// our last one or our "disabled" state
		// which is always our last state:
		if(states.Length > 2 && selState != states.Length-1)
		{
			// Delete a state
			if (GUILayout.Button(inspector ? "-" : "Delete State", inspector ? "ToolbarButton" : "Button"))
			{
				// Remove the selected state:
				List<TextureAnim> tempList = new List<TextureAnim>();
				tempList.AddRange(states);
				tempList.RemoveAt(selState);
				states = tempList.ToArray();

				// Remove the associated transition:
				List<EZTransitionList> tempTrans = new List<EZTransitionList>();
				tempTrans.AddRange(transitions);
				tempTrans.RemoveAt(selState);
				transitions = tempTrans.ToArray();

				// Remove the associated label:
				List<string> tempLabels = new List<string>();
				tempLabels.AddRange(stateLabels);
				tempLabels.RemoveAt(selState);
				stateLabels = tempLabels.ToArray();
			}

			// Make sure the default state is
			// within a valid range:
			defaultState = defaultState % states.Length;
		}
		
		if (inspector)
		{
			GUILayout.FlexibleSpace();
		}


		GUILayout.EndHorizontal();

		return 14;
	}

	// Draw our state naming controls in the GUI:
	public override int DrawPostStateSelectGUI(int selState)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));

		GUILayout.Space(20f);
		GUILayout.Label("State Name:");

		// Only allow editing if this is not the disabled state:
		if (selState < states.Length - 1)
		{
			states[selState].name = GUILayout.TextField(states[selState].name);
		}
		else
			GUILayout.TextField(states[selState].name);

		GUILayout.EndHorizontal();

		return 28;
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIStateToggleBtn Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIStateToggleBtn)go.AddComponent(typeof(UIStateToggleBtn));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIStateToggleBtn Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIStateToggleBtn)go.AddComponent(typeof(UIStateToggleBtn));
	}
}
