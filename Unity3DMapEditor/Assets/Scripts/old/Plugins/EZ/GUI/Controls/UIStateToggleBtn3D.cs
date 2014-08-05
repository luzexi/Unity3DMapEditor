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
/// This differs from UIStateToggleBtn in that it has no sprite
/// graphics and instead is intended to be used in conjuction
/// with an existing 3D object in the scene.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/3D Toggle Button")]
public class UIStateToggleBtn3D : ControlBase
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
		get { return states[curStateIndex]; }
	}

	/// <summary>
	/// Zero-based index of the state that 
	/// should be the default, initial state.
	/// </summary>
	public int defaultState;


	[HideInInspector]
	public string[] states = { "Unnamed", "Disabled" };

	public override string[] States
	{
		get { return states; }
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


	
	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		if (!m_controlIsEnabled)
		{
			base.OnInput(ref ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Check to see if we're disabled or hidden again in case
		// we were disabled by an input delegate:
		if (!m_controlIsEnabled)
		{
			base.OnInput(ref ptr);
			return;
		}

		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ToggleState();
			if (soundToPlay != null)
				soundToPlay.PlayOneShot(soundToPlay.clip);
		}

		// Toggle if the required event occurred:
		if (ptr.evt == whenToInvoke)
		{
			if (scriptWithMethodToInvoke != null)
				scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
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
		base.Start();

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			for (int j = 0; j < states.Length; ++j)				
			{
				// Setup the transition:
				transitions[j].list[0].MainSubject = this.gameObject;

				// Add our text as a sub-subject of our transition as well:
				if (spriteText != null)
					transitions[j].list[0].AddSubSubject(spriteText.gameObject);
			}

			// Create a default collider if none exists:
			if (collider == null)
			{
				AddCollider();
			}

			//SetToggleState(curStateIndex);
		}
	}

	public override void Copy(IControl c)
	{
		Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		base.Copy(c);

		if (!(c is UIStateToggleBtn3D))
			return;

		UIStateToggleBtn3D b = (UIStateToggleBtn3D)c;

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

		// Call our changed delegate:
		if (changeDelegate != null)
			changeDelegate(this);

		return curStateIndex;
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// </summary>
	/// <param name="s">The zero-based state number/index.</param>
	public virtual void SetToggleState(int s)
	{
		curStateIndex = s % (states.Length-1);

		this.UseStateLabel(curStateIndex);

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		transitions[curStateIndex].list[0].Start();
		prevTransition = transitions[curStateIndex].list[0];
	}


	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// Does nothing if the specified state is not found.
	/// </summary>
	/// <param name="s">The name of the desired state.</param>
	public virtual void SetToggleState(string stateName)
	{
		for(int i=0; i<states.Length; ++i)
		{
			if (states[i] == stateName)
			{
				SetToggleState(i);
				return;
			}
		}
	}

	// Sets the control to its disabled appearance:
	protected void DisableMe()
	{
		this.UseStateLabel(states.Length - 1);

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		transitions[states.Length - 1].list[0].Start();
		prevTransition = transitions[states.Length - 1].list[0];
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

	// Draw our state creation/deletion controls in the GUI:
	public override int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));

		// Add a new state
		if(GUILayout.Button(inspector?"+":"Add State", inspector?"ToolbarButton":"Button"))
		{
			// Insert the new state before the "disabled" state:
			List<string> tempList = new List<string>();
			tempList.AddRange(states);
			tempList.Insert(states.Length - 1, "State " + (states.Length - 1));
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
				List<string> tempList = new List<string>();
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
			states[selState] = GUILayout.TextField(states[selState]);
		}
		else
			GUILayout.TextField(states[selState]);

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
	static public UIStateToggleBtn3D Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIStateToggleBtn3D)go.AddComponent(typeof(UIStateToggleBtn3D));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIStateToggleBtn3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIStateToggleBtn3D)go.AddComponent(typeof(UIStateToggleBtn3D));
	}
}
