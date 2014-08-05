//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------



using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Definition of a radio button interface
public interface IRadioButton
{
	bool Value
	{
		get;
		set;
	}

	string name
	{
		get;
		set;
	}
}


/// <summary>
/// A class which encapsulates all that is required to track
/// and manage radio buttons as a group.
/// </summary>
public class RadioBtnGroup
{
	static List<RadioBtnGroup> groups = new List<RadioBtnGroup>();

	public RadioBtnGroup(int id)
	{
		groupID = id;
		groups.Add(this);
	}

	~RadioBtnGroup()
	{
		groups.Remove(this);
	}

	public int groupID;

	public ArrayList buttons = new ArrayList();

	/// <summary>
	/// Returns a reference to the selected radio button for
	/// the specified group.
	/// </summary>
	/// <param name="id">The parent GameObject of the radio buttons.</param>
	/// <returns>A reference to the currently selected (true) radio button.  Null if none is set to true.</returns>
	public static IRadioButton GetSelected(GameObject go)
	{
		return GetSelected(go.transform.GetHashCode());
	}

	/// <summary>
	/// Returns a reference to the selected radio button for
	/// the specified group.
	/// </summary>
	/// <param name="id">The ID of the group (either an arbitrary integer ID, or the hashcode of a radio button's parent transform, depending on the settings).</param>
	/// <returns>A reference to the currently selected (true) radio button.  Null if none is set to true.</returns>
	public static IRadioButton GetSelected(int id)
	{
		RadioBtnGroup group = null;

		for (int i = 0; i < groups.Count; ++i)
		{
			if(groups[i].groupID == id)
			{
				group = groups[i];
				break;
			}
		}

		if(group == null)
			return null;

		for (int i = 0; i < group.buttons.Count; ++i)
			if (((IRadioButton)group.buttons[i]).Value)
				return (IRadioButton)group.buttons[i];

		return null;
	}

	/// <summary>
	/// Returns a reference to the radio button group
	/// specified by "id".
	/// </summary>
	/// <param name="id">The ID of the group (either an arbitrary integer ID, or the hashcode of a radio button's parent transform, depending on the settings).</param>
	/// <returns>Returns a reference to the group with the specified ID.</returns>
	public static RadioBtnGroup GetGroup(int id)
	{
		RadioBtnGroup group= null;

		for (int i = 0; i < groups.Count; ++i)
		{
			if(groups[i].groupID == id)
			{
				group = groups[i];
				break;
			}
		}

		if (group == null)
			group = new RadioBtnGroup(id);

		return group;
	}
}


/// <remarks>
/// Button class that allows you to allow selection
/// among of set of mutually exclusive options.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Radio Button")]
public class UIRadioBtn : AutoSpriteControlBase, IRadioButton
{
	protected enum CONTROL_STATE
	{
		True,
		False,
		Disabled,
		/// <summary>
		/// Over is not supported directly by the control's
		/// states, but is included to allow layers to
		/// represent the control's Over state.
		/// </summary>
		Over,
		/// <summary>
		/// Active is not supported directly by the control's
		/// states, but is included to allow layers to
		/// represent the control's Active state.
		/// </summary>
		Active
	}

	// Keeps track of the control's state
	CONTROL_STATE state;
	CONTROL_STATE layerState;


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
	public virtual bool Value
	{
		get { return btnValue; }
		set
		{
			SetValue(value);
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

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("True"),
			new TextureAnim("False"),
			new TextureAnim("Disabled")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

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

	public override CSpriteFrame DefaultFrame
	{
		get
		{
			int stateNum = btnValue ? 0 : 1;

			if (States[stateNum].spriteFrames.Length != 0)
				return States[stateNum].spriteFrames[0];
			else
				return null;
		}
	}

	public override TextureAnim DefaultState
	{
		get
		{
			int stateNum = btnValue ? 0 : 1;

			return States[stateNum];
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
			Value = true;
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
					SetLayerState(CONTROL_STATE.Over);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if (!disableHoverEffect)
					SetLayerState(CONTROL_STATE.Active);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
				if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD &&
					ptr.hitInfo.collider == collider &&
					!disableHoverEffect)
					SetLayerState(CONTROL_STATE.Over);
				else
					SetLayerState(state);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				SetLayerState(state);
				break;
		}

		base.OnInput(ref ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIRadioBtn))
			return;

		UIRadioBtn b = (UIRadioBtn)s;

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			state = b.state;
			prevTransition = b.prevTransition;
			if (Application.isPlaying)
				Value = b.Value;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			group = b.group;
			defaultValue = b.defaultValue;
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


	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
/*
	protected override void OnEnable()
	{
		base.OnEnable();

#if RADIOBTN_USE_PARENT
		SetGroup(transform.parent);
#else
		SetGroup(radioGroup);
#endif
	}
 */


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
	/// <param name="parent">The parent object of the radio button.</param>
	public void SetGroup(GameObject parent)
	{
		SetGroup(parent.transform.GetHashCode());
	}

	/// <summary>
	/// Makes the radio button a part of the specified group
	/// and it will thenceforth be mutually exclusive to all
	/// other radio buttons in the same group.
	/// </summary>
	/// <param name="parent">The parent transform of the radio button.</param>
	public void SetGroup(Transform parent)
	{
		SetGroup(parent.GetHashCode());
	}

	/// <summary>
	/// Makes the radio button a part of the specified group
	/// and it will thenceforth be mutually exclusive to all
	/// other radio buttons in the same group.
	/// </summary>
	/// <param name="groupID">The ID of the group to which this radio will be assigned.  Can be an arbitrary integer, or if useParentForGrouping is true, the hashcode of the parent transform.</param>
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
		if (m_started)
			return;

		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[1][];
		aggregateLayers[0] = layers;

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

			// Add our text as a sub-subject of our transitions:

			stateIndices = new int[layers.Length, 5];

			// We'll use this to setup our state:
			int stateIdx = btnValue ? 0 : 1;
			stateIdx = m_controlIsEnabled ? stateIdx : 2;

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			for (int i = 0; i < layers.Length; ++i)
			{
				if (layers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}

				stateIndices[i, 0] = layers[i].GetStateIndex("true");
				stateIndices[i, 1] = layers[i].GetStateIndex("false");
				stateIndices[i, 2] = layers[i].GetStateIndex("disabled");
				stateIndices[i, 3] = layers[i].GetStateIndex("over");
				stateIndices[i, 4] = layers[i].GetStateIndex("active");

				// Add this as a subject of our transition for 
				// each state, as appropriate:
				if (stateIndices[i, 0] != -1)
				{
					transitions[0].list[0].AddSubSubject(layers[i].gameObject);
					transitions[0].list[1].AddSubSubject(layers[i].gameObject);
				}
				if (stateIndices[i, 1] != -1)
				{
					transitions[1].list[0].AddSubSubject(layers[i].gameObject);
					transitions[1].list[1].AddSubSubject(layers[i].gameObject);
				}
				if (stateIndices[i, 2] != -1)
				{
					transitions[2].list[0].AddSubSubject(layers[i].gameObject);
					transitions[2].list[1].AddSubSubject(layers[i].gameObject);
				}

				// Set the layer's state:
				if (stateIndices[i, stateIdx] != -1)
					layers[i].SetState(stateIndices[i, stateIdx]);
				else
					layers[i].Hide(true);
			}

			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			SetValue(btnValue, true);

			if (useParentForGrouping && transform.parent != null)
				SetGroup(transform.parent.GetHashCode());
			else
				SetGroup(radioGroup);
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	// Sets all other buttons in the group to false.
	protected void PopOtherButtonsInGroup()
	{
		if (group == null)
			return;

		for (int i = 0; i < group.buttons.Count; ++i)
		{
			if (((UIRadioBtn)group.buttons[i]) != this)
				((UIRadioBtn)group.buttons[i]).Value = false;
		}
	}

	protected virtual void SetValue(bool val)
	{
		SetValue(val, false);
	}

	protected virtual void SetValue(bool val, bool suppressTransition)
	{
		bool prevValue = btnValue;
		btnValue = val;

		// Pop out the other buttons in the group:
		if (btnValue)
			PopOtherButtonsInGroup();

		// Update the button's visual state:
		SetButtonState(suppressTransition);

		// If the value changed:
		if (prevValue != btnValue)
		{
			// Notify our change delegate:
			if (changeDelegate != null)
				changeDelegate(this);
		}
	}

	protected virtual void SetButtonState()
	{
		SetButtonState(false);
	}

	// Sets the button's visual state to match its value.
	protected virtual void SetButtonState(bool suppressTransition)
	{
		// Make sure we have a mesh:
		if (spriteMesh == null)
			return;

		// Make sure we're initialized since
		// we might have been called as a result of
		// another button in our group settings its
		// value on Start() before we've had a cance
		// to Start() ourselves, meaning we may lack
		// important info like a valid screensize,
		// etc. which is necessary for sizing or else
		// we'll get vertices of "infinite" value
		// resulting in a !local.IsValid() error:
		if (!m_started)
			return;

		int prevState = (int)state;
		state = controlIsEnabled ? (btnValue ? CONTROL_STATE.True : CONTROL_STATE.False) : CONTROL_STATE.Disabled;

		// Clamp the index just to the state indices
		// directly supported by the control
		int index = Mathf.Clamp((int)state, 0, 2);

		// First see if we need to postpone this state
		// change for when we are active:
		if (!gameObject.active)
		{
			stateChangeWhileDeactivated = true;
			return;
		}

		this.SetState(index);

		this.UseStateLabel(index);

		// Recalculate our collider
		UpdateCollider();

		SetLayerState(state);

		if (!suppressTransition)
		{
			// End any current transition:
			if (prevTransition != null && prevState != (int)state)
				prevTransition.StopSafe();

			StartTransition(index, prevState);
		}
	}

	// Sets the layers to represent the current control state
	// (includes states not directly supported by the control
	// itself, such as Over and Active)
	protected void SetLayerState(CONTROL_STATE s)
	{
		// Skip if redundant:
		if (s == layerState)
			return;

		layerState = s;

		// This index is valid for our layers as
		// they can also support "Over" and "Active":
		int layerIndex = (int)layerState;

		// Loop through each layer and set its state,
		// provided we have a valid index for that state:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (-1 != stateIndices[i, layerIndex])
			{
				layers[i].Hide(IsHidden());
				layers[i].SetState(stateIndices[i, layerIndex]);
			}
			else
				layers[i].Hide(true);
		}
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
					case 0: // True
						prevTransition = null;
						return;
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
					case 1: // False
						prevTransition = null;
						return;
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
					case 2: // Disabled
						prevTransition = null;
						return;
				}
				break;
		}

		transitions[newState].list[transIndex].Start();
		prevTransition = transitions[newState].list[transIndex];
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (stateChangeWhileDeactivated)
		{
			SetButtonState();
			stateChangeWhileDeactivated = false;
		}
	}


	// Sets the control to its disabled appearance:
	protected void DisableMe()
	{
		// The disabled state is the last in the state list:
		SetState(states.Length - 1);

		// Set the label:
		this.UseStateLabel(states.Length - 1);

		// Set the layer states:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (stateIndices[i, states.Length - 1] != -1)
				layers[i].SetState(stateIndices[i, states.Length - 1]);
		}

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

	// Sets the default UVs:
	public override void InitUVs()
	{
		int index;

		if (!m_controlIsEnabled)
			index = states.Length - 1;
		else
			index = defaultValue ? 0 : 1;

		if (states[index].spriteFrames.Length != 0)
			frameInfo.Copy(states[index].spriteFrames[0]);

		base.InitUVs();
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
	static public UIRadioBtn Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIRadioBtn)go.AddComponent(typeof(UIRadioBtn));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIRadioBtn Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIRadioBtn)go.AddComponent(typeof(UIRadioBtn));
	}
}
