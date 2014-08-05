//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------



using UnityEngine;
using System.Collections;



/// <remarks>
/// Button class that allows you to allow selection
/// among of set of mutually exclusive options.
/// This differs from UIRadioBtn in that it has no sprite
/// graphics and instead is intended to be used in conjuction
/// with an existing 3D object in the scene.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/3D Radio Button")]
public class UIRadioBtn3D : ControlBase, IRadioButton
{
	protected enum CONTROL_STATE
	{
		True,
		False,
		Disabled
	}

	// Keeps track of the control's state
	CONTROL_STATE state;


	//---------------------------------------------
	// End Static members
	//---------------------------------------------

	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			if (!value)
				DisableMe();
			else
				SetButtonState();
		}
	}


	// The current value of the button
	protected bool btnValue;

	/// <summary>
	/// Provides access to the boolean value of the button.
	/// </summary>
	public bool Value
	{
		get { return btnValue; }
		set
		{
			bool prevValue = btnValue;
			btnValue = value;
			
			// Pop out the other buttons in the group:
			if(btnValue)
				PopOtherButtonsInGroup();

			// Update the button's visual state:
			SetButtonState();

			// If our value changed:
			if(prevValue != btnValue)
			{
				// Notify our change delegate:
				if (changeDelegate != null)
					changeDelegate(this);
			}
		}
	}


	/// <summary>
	/// When true, the radio button will group itself with other
	/// radio buttons based on whether they share the same
	/// parent GameObject.
	/// </summary>
	public bool useParentForGrouping = true;


	/// <summary>
	/// The numbered group to which this radio button 
	/// belongs.  Buttons that share a group will be 
	/// mutually exclusive to one another.
	/// This value is only available if RADIOBTN_USE_PARENT
	/// is not defined.  Otherwise, by default, radio buttons
	/// group themselves according to a common parent GameObject.
	/// </summary>
	public int radioGroup;

	// Reference to the group that contains this
	// radio button.
	protected RadioBtnGroup group;

	/// <summary>
	/// The default value of the button
	/// </summary>
	public bool defaultValue;

	// Tracks whether the state was changed while
	// the control was deactivated
	protected bool stateChangeWhileDeactivated = false;


	protected string[] states = { "True", "False", "Disabled" };

	public override string[] States
	{
		get { return states; }
	}


	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			new EZTransitionList( new EZTransition[]	// True
			{
				new EZTransition("From False"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// False
			{
				new EZTransition("From True"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From True"),
				new EZTransition("From False")
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

	// Helps us keep track of the previous transition
	EZTransition prevTransition;

	// Strings to display for each state.
	[HideInInspector]
	public string[] stateLabels = new string[] { DittoString, DittoString, DittoString };

	public override string GetStateLabel(int index)
	{
		return stateLabels[index];
	}

	public override void SetStateLabel(int index, string label)
	{
		stateLabels[index] = label;

		if (index == (int)state)
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

		// Toggle if the required event occurred:
		if (ptr.evt == whenToInvoke)
		{
			Value = true;
			if (soundToPlay != null)
				soundToPlay.PlayOneShot(soundToPlay.clip);

			if (scriptWithMethodToInvoke != null)
				scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}

		base.OnInput(ref ptr);
	}

	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
/*
	protected void OnEnable()
	{
#if RADIOBTN_USE_PARENT
		SetGroup(transform.parent);
#else
		SetGroup(radioGroup);
#endif
	}
*/

	public override void OnEnable()
	{
		base.OnEnable();

		if (stateChangeWhileDeactivated)
		{
			SetButtonState();
			stateChangeWhileDeactivated = false;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();

		if (group == null)
			return;

		// Remove self from the group
		group.buttons.Remove(this);
		group = null;
	}


	/// <summary>
	/// Makes the radio button a part of the specified group
	/// and it will thenceforth be mutually exclusive to all
	/// other radio buttons in the same group.
	/// </summary>
	/// <param name="ID">The parent object of the radio button.</param>
	public void SetGroup(GameObject parent)
	{
		SetGroup(parent.transform.GetHashCode());
	}


	/// <summary>
	/// Makes the radio button a part of the specified group
	/// and it will thenceforth be mutually exclusive to all
	/// other radio buttons in the same group.
	/// </summary>
	/// <param name="ID">The ID of the group to which this radio will be assigned.  Can be an arbitrary integer, or if useParentForGrouping is true, the hashcode of the parent transform.</param>
	public void SetGroup(int groupID)
	{
		// Remove from any existing group first:
		if (group != null)
		{
			group.buttons.Remove(this);
			group = null;
		}

		radioGroup = groupID;

		group = RadioBtnGroup.GetGroup(groupID);

		// Add self to the button group:
		group.buttons.Add(this);

		if (btnValue)
			PopOtherButtonsInGroup();
	}


	protected override void Awake()
	{
		base.Awake();

		btnValue = defaultValue;
	}

	public override void Start()
	{
		base.Start();

		state = controlIsEnabled ? (btnValue ? CONTROL_STATE.True : CONTROL_STATE.False) : CONTROL_STATE.Disabled;
/*
		if (btnValue)
			PopOtherButtonsInGroup();
*/

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Setup our transitions:
			for (int i = 0; i < transitions.Length; ++i)
				for (int j = 0; j < transitions[i].list.Length; ++j)
				{
					transitions[i].list[j].MainSubject = this.gameObject;

					// Add our text as a sub-subject of our transitions:
					if (spriteText != null)
						transitions[i].list[j].AddSubSubject(spriteText.gameObject);
				}

			// We'll use this to setup our state:
			int stateIdx = btnValue ? 0 : 1;
			stateIdx = m_controlIsEnabled ? stateIdx : 2;

			// Create a default collider if none exists:
			if (collider == null)
			{
				AddCollider();
			}

			//SetState(stateIdx);
		}

		Value = btnValue;

		if (useParentForGrouping && transform.parent != null)
			SetGroup(transform.parent.GetHashCode());
		else
			SetGroup(radioGroup);
	}

	public override void Copy(IControl c)
	{
		Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		if (!(c is UIRadioBtn3D))
			return;

		base.Copy(c);

		UIRadioBtn3D b = (UIRadioBtn3D)c;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			group = b.group;
			defaultValue = b.defaultValue;
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = b.prevTransition;

			if (Application.isPlaying)
				Value = b.Value;
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


	// Sets all other buttons in the group to false.
	protected void PopOtherButtonsInGroup()
	{
		if (group == null)
			return;

		for (int i = 0; i < group.buttons.Count; ++i)
		{
			if(((UIRadioBtn3D)group.buttons[i]) != this)
				((UIRadioBtn3D)group.buttons[i]).Value = false;
		}
	}

	// Sets the button's visual state to match its value.
	protected virtual void SetButtonState()
	{
		int prevState = (int)state;
		state = controlIsEnabled ? (btnValue ? CONTROL_STATE.True : CONTROL_STATE.False) : CONTROL_STATE.Disabled;

		int index = (int)state;

		// First see if we need to postpone this state
		// change for when we are active:
		if (!gameObject.active)
		{
			stateChangeWhileDeactivated = true;
			return;
		}

		this.UseStateLabel(index);

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		StartTransition(index, prevState);
	}

	// Starts the appropriate transition
	protected void StartTransition(int newState, int prevState)
	{
		int transIndex = 0;

		// What state are we now in?
		switch (newState)
		{
			case 0:	// True
				// Where did we come from?
				switch (prevState)
				{
					case 1: // False
						transIndex = 0;
						break;
					case 2:	// Disabled
						transIndex = 1;
						break;
				}
				break;
			case 1:	// False
				// Where did we come from?
				switch (prevState)
				{
					case 0: // True
						transIndex = 0;
						break;
					case 2:	// Disabled
						transIndex = 1;
						break;
				}
				break;
			case 2:	// Disabled
				// Where did we come from?
				switch (prevState)
				{
					case 0: // True
						transIndex = 0;
						break;
					case 1:	// False
						transIndex = 1;
						break;
				}
				break;
		}

		transitions[newState].list[transIndex].Start();
		prevTransition = transitions[newState].list[transIndex];
	}



	// Sets the control to its disabled appearance:
	protected void DisableMe()
	{
		this.UseStateLabel(states.Length - 1);

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		StartTransition((int)CONTROL_STATE.Disabled, (int)state);

		state = CONTROL_STATE.Disabled;
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
	static public UIRadioBtn3D Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIRadioBtn3D)go.AddComponent(typeof(UIRadioBtn3D));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIRadioBtn3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIRadioBtn3D)go.AddComponent(typeof(UIRadioBtn3D));
	}
}
