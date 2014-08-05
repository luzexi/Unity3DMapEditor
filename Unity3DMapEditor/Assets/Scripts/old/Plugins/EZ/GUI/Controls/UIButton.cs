//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Button class that allows you to invoke a specified method
/// on a specified component script.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Button")]
public class UIButton : AutoSpriteControlBase
{
	/// <summary>
	/// Indicates the state of the button
	/// </summary>
	public enum CONTROL_STATE
	{
		/// <summary>
		/// The button is "normal", awaiting input
		/// </summary>
		NORMAL,

		/// <summary>
		/// The button has an input device hovering over it.
		/// </summary>
		OVER,

		/// <summary>
		/// The button is being pressed
		/// </summary>
		ACTIVE,

		/// <summary>
		/// The button is disabled
		/// </summary>
		DISABLED
	};


	protected CONTROL_STATE m_ctrlState;

	/// <summary>
	/// Gets the current state of the button.
	/// </summary>
	public CONTROL_STATE controlState
	{
		get { return m_ctrlState; }
	}

	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			bool prevValue = m_controlIsEnabled;
			m_controlIsEnabled = value;
			if (!value)
				SetControlState(CONTROL_STATE.DISABLED);
			else if (!prevValue) // Only switch to normal if we weren't already enabled
				SetControlState(CONTROL_STATE.NORMAL);
		}
	}

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Normal"),
			new TextureAnim("Over"),
			new TextureAnim("Active"),
			new TextureAnim("Disabled")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			new EZTransitionList( new EZTransition[]	// Normal
			{
				new EZTransition("From Over"),
				new EZTransition("From Active"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// Over
			{
				new EZTransition("From Normal"),
				new EZTransition("From Active")
			}),
			new EZTransitionList( new EZTransition[]	// Active
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over"),
				new EZTransition("From Active")
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

		if (index == (int)m_ctrlState)
			UseStateLabel(index);
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
	/// you wish to invoke when the button is tapped.
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
	/// Sound that will be played when the button is is in an "over" state (mouse over)
	/// </summary>
	public AudioSource soundOnOver;

	/// <summary>
	/// Sound that will be played when the button is activated (pressed)
	/// </summary>
	public AudioSource soundOnClick;

	/// <summary>
	/// When repeat is true, the button will call the various
	/// delegates and invokes as long as the button is held
	/// down.
	/// NOTE: If repeat is true, it overrides any setting of
	/// "whenToInvoke"/"When To Invoke". One exception to this
	/// is that "soundToPlay" is still played based upon
	/// "whenToInvoke".
	/// </summary>
	public bool repeat;

	/// <summary>
	/// When set to true, the active state transition will
	/// always run to completion even if the control changes
	/// to another state while it is running.  Otherwise,
	/// the active state transition will be aborted if the
	/// control's state changes to another state while it is
	/// running.
	/// </summary>
	public bool alwaysFinishActiveTransition = false;

	// Tracks whether a follow-up transition is queued.
	// This is used to transition to the current state
	// from an active state transition that is finishing
	// late because "alwaysFinishActiveTransition" is set
	// to true.
	protected bool transitionQueued = false;

	// This holds a reference to the transition that is to 
	// be run when the current late-finishing transition 
	// completes.
	protected EZTransition nextTransition;

	// Like the above member, it holds the next
	// state to be entered when the active transition
	// completes.
	protected CONTROL_STATE nextState;

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

		// Change the state if necessary:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (m_ctrlState != CONTROL_STATE.OVER)
				{
					SetControlState(CONTROL_STATE.OVER);
					if (soundOnOver != null)
						soundOnOver.PlayOneShot(soundOnOver.clip);
				}
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.PRESS:
				SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
				// Only go to the OVER state if we have
				// have frame info for that or if we aren't
				// in touchpad mode, or if the collider hit
				// by the touch was actually us, indicating
				// that we're still under the pointer:
				if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD &&
					ptr.hitInfo.collider == collider)
					SetControlState(CONTROL_STATE.OVER);
				else
					SetControlState(CONTROL_STATE.NORMAL);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				SetControlState(CONTROL_STATE.NORMAL);
				break;
		}

		base.OnInput(ref ptr);

		if (repeat)
		{
			if (m_ctrlState == CONTROL_STATE.ACTIVE)
				goto Invoke;
		}
		else if (ptr.evt == whenToInvoke)
			goto Invoke;

		return;

	Invoke:
		if (ptr.evt == whenToInvoke)
		{
			if (soundOnClick != null)
				soundOnClick.PlayOneShot(soundOnClick.clip);
		}
		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		if (changeDelegate != null)
			changeDelegate(this);
	}


	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	public override void Start()
	{
		if (m_started)
			return;

		base.Start();

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Assign our aggregate layers:
			aggregateLayers = new SpriteRoot[1][];
			aggregateLayers[0] = layers;

			// Setup our transitions:
			for (int i = 0; i < transitions.Length; ++i)
				for (int j = 0; j < transitions[i].list.Length; ++j)
				{
					transitions[i].list[j].MainSubject = this.gameObject;

					// Add our text as a sub-subject of our transitions:
					if (spriteText != null)
						transitions[i].list[j].AddSubSubject(spriteText.gameObject);
				}

			stateIndices = new int[layers.Length, 4];

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

				stateIndices[i, (int)CONTROL_STATE.NORMAL] = layers[i].GetStateIndex("normal");
				stateIndices[i, (int)CONTROL_STATE.OVER] = layers[i].GetStateIndex("over");
				stateIndices[i, (int)CONTROL_STATE.ACTIVE] = layers[i].GetStateIndex("active");
				stateIndices[i, (int)CONTROL_STATE.DISABLED] = layers[i].GetStateIndex("disabled");

				// Add this as a subject of our transition for 
				// each state, as appropriate:
				if (stateIndices[i, 0] != -1)
				{
					transitions[0].list[0].AddSubSubject(layers[i].gameObject);
					transitions[0].list[1].AddSubSubject(layers[i].gameObject);
					transitions[0].list[2].AddSubSubject(layers[i].gameObject);
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
				if (stateIndices[i, 3] != -1)
				{
					transitions[3].list[0].AddSubSubject(layers[i].gameObject);
					transitions[3].list[1].AddSubSubject(layers[i].gameObject);
					transitions[3].list[2].AddSubSubject(layers[i].gameObject);
				}

				// Setup the layer:
				if (stateIndices[i, (int)m_ctrlState] != -1)
					layers[i].SetState(stateIndices[i, (int)m_ctrlState]);
				else
					layers[i].Hide(true);
			}

			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			SetState((int)m_ctrlState);
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (Application.isPlaying && m_started)
		{
			// Set it to some bogus value so we can force
			// it to be reset:
			m_ctrlState = (CONTROL_STATE)(-1);

			if (controlIsEnabled)
				SetControlState(CONTROL_STATE.NORMAL, true);
			else
				SetControlState(CONTROL_STATE.DISABLED, true);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		// Cancel any queued transitions:
		if (transitionQueued)
		{
			nextTransition.RemoveTransitionEndDelegate(RunFollowupTrans);
			transitionQueued = false;
		}

		// Revert to our normal state, but if there isn't
		// an EZAnimator, then we either don't need to
		// revert, or we don't want to because the scene
		// is closing:
		if (EZAnimator.Exists() && !deleted)
		{
			// Temporarily disable running follow-up transitions:
			bool prevAFAT = alwaysFinishActiveTransition;
			alwaysFinishActiveTransition = false;

			// Cancel any running transition:
			if (prevTransition != null)
			{
				if (prevTransition.IsRunning())
					prevTransition.End();
			}

			// Restore our setting:
			alwaysFinishActiveTransition = prevAFAT;
		}

		prevTransition = null;
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIButton))
			return;

		UIButton btn = (UIButton)s;

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = btn.prevTransition;

			if (Application.isPlaying)
				SetControlState(btn.controlState);
		}

		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = btn.scriptWithMethodToInvoke;
			methodToInvoke = btn.methodToInvoke;
			whenToInvoke = btn.whenToInvoke;
			delay = btn.delay;
		}

		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundOnOver = btn.soundOnOver;
			soundOnClick = btn.soundOnClick;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			repeat = btn.repeat;
		}
	}

	public virtual void SetControlState(CONTROL_STATE s)
	{
		SetControlState(s, false);
	}

	// Switches the displayed sprite(s) to match the current state:
	public virtual void SetControlState(CONTROL_STATE s, bool suppressTransitions)
	{
		// If this is the same as the current state, ignore:
		if (m_ctrlState == s)
			return;

		// Only stop our transition if it isn't an active state
		// transition and we don't want it to run to completion:
		if (!(alwaysFinishActiveTransition &&
			(prevTransition == transitions[(int)UIButton.CONTROL_STATE.ACTIVE].list[0] ||
			 prevTransition == transitions[(int)UIButton.CONTROL_STATE.ACTIVE].list[1] &&
			 prevTransition.IsRunning())
			))
		{

			int prevState = (int)m_ctrlState;

			m_ctrlState = s;

			// Validate that we can go to this appearance:
			if (animations[(int)s].GetFrameCount() > 0)
				this.SetState((int)s);

			this.UseStateLabel((int)s);

			if (s == CONTROL_STATE.DISABLED)
				m_controlIsEnabled = false;
			else
				m_controlIsEnabled = true;

			// Recalculate our collider
			UpdateCollider();

			// Loop through each layer and set its state,
			// provided we have a valid index for that state:
			for (int i = 0; i < layers.Length; ++i)
			{
				if (-1 != stateIndices[i, (int)s])
				{
					layers[i].Hide(IsHidden());
					layers[i].SetState(stateIndices[i, (int)s]);
				}
				else
					layers[i].Hide(true);
			}

			// End any current transition:
			if (prevTransition != null)
				prevTransition.StopSafe();

			// Start a new transition:
			StartTransition((int)s, prevState, suppressTransitions);
		}
		else  // Else, have our desired transition run when the active transition is complete:
		{
			// Go no further if we're suppressing transitions:
			QueueTransition((int)s, (int)UIButton.CONTROL_STATE.ACTIVE, suppressTransitions);
		}
	}

	// Returns the desired transition to run next based
	// upon the in-coming state and the previous state:
	protected int DetermineNextTransition(int newState, int prevState)
	{
		int transIndex = 0;

		// What state are we now in?
		switch (newState)
		{
			case 0:	// Normal
				// Where did we come from?
				switch (prevState)
				{
					case 1: // Over
						transIndex = 0;
						break;
					case 2:	// Active
						transIndex = 1;
						break;
					case 3:	// Disabled
						transIndex = 2;
						break;
				}
				break;
			case 1:	// Over
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 2:	// Active
						transIndex = 1;
						break;
				}
				break;
			case 2:	// Active
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 1:	// Over
						transIndex = 1;
						break;
				}
				break;
			case 3:	// Disabled
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 1:	// Over
						transIndex = 1;
						break;
					case 2:	// Active
						transIndex = 2;
						break;
				}
				break;
		}

		return transIndex;
	}

	// Starts the appropriate transition
	protected void StartTransition(int newState, int prevState, bool suppressTransition)
	{
		int transIndex = DetermineNextTransition(newState, prevState);

		prevTransition = transitions[newState].list[transIndex];

		// See if the transition has any content:
		if (prevTransition.animationTypes == null || prevTransition.animationTypes.Length < 1)
			prevTransition = null;
		else
		{
			prevTransition.Start();
			if (suppressTransition)
				prevTransition.End(); // End immediately in the "end state"
		}
	}

	// Queues a transition to play following the previous (currently-running) transition
	protected void QueueTransition(int newState, int prevState, bool suppressTransition)
	{
		if (deleted)
			return;

		nextTransition = transitions[newState].list[DetermineNextTransition(newState, prevState)];
		nextState = (CONTROL_STATE)newState;

		if (suppressTransition)
		{
			prevTransition.End();
			prevTransition = nextTransition;
			prevTransition.Start();
			prevTransition.End(); // Immediately place the transition into its "end state".
			return;
		}

		// See if we've already queued to run a follow-up transition:
		if (!transitionQueued)
			prevTransition.AddTransitionEndDelegate(RunFollowupTrans);

		transitionQueued = true;
	}

	// Runs a follow-up transition to the one which just completed
	protected void RunFollowupTrans(EZTransition trans)
	{
		if (deleted)
		{
			trans.RemoveTransitionEndDelegate(RunFollowupTrans);
			return;
		}

		//prevTransition = nextTransition;
		prevTransition = null;
		nextTransition = null;
		trans.RemoveTransitionEndDelegate(RunFollowupTrans);
		transitionQueued = false;

		//if(prevTransition != null)
		//	prevTransition.Start();
		SetControlState(nextState);
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
		if (states[0].spriteFrames.Length != 0)
			frameInfo.Copy(states[0].spriteFrames[0]);

		base.InitUVs();
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIButton Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIButton)go.AddComponent(typeof(UIButton));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIButton)go.AddComponent(typeof(UIButton));
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}
}
