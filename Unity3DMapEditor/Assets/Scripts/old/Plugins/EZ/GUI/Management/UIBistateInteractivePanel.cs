 //-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// A type of panel that, when clicked, will toggle
/// between its showing and hideAtStart states.
/// Use this to implement things like popup-menus.
/// NOTE: The panel is shown using the BringInForward
/// transition, and dismissed using the DismissForward
/// transition.
/// </remarks>
[System.Serializable]
[AddComponentMenu("EZ GUI/Panels/Bi-State Interactive Panel")]
public class UIBistateInteractivePanel : UIPanelBase
{
	/// <summary>
	/// Indicates the state of the button
	/// </summary>
	public enum STATE
	{
		/// <summary>
		/// The panel is "normal", awaiting input
		/// </summary>
		SHOWING,

		/// <summary>
		/// The panel has an input device hovering over it.
		/// </summary>
		HIDDEN
	};


	protected STATE m_panelState;

	/// <summary>
	/// Gets the current state of the panel.
	/// </summary>
	public STATE State
	{
		get { return m_panelState; }
		set { SetPanelState(value); }
	}

	/// <summary>
	/// Returns true when the panel is showing, false
	/// when dismissed/hidden.
	/// </summary>
	public bool IsShowing
	{
		get { return m_panelState == STATE.SHOWING; }
	}


	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList transitions = new EZTransitionList(new EZTransition[]
		{
			new EZTransition("Bring In Forward"),
			new EZTransition("Bring In Back"),
			new EZTransition("Dismiss Forward"),
			new EZTransition("Dismiss Back")
		});

	public override EZTransitionList Transitions
	{
		get { return transitions; }
	}


	/// <summary>
	/// When true, a PRESS is not sufficient, but
	/// rather a TAP event is required (press and
	/// release).
	/// </summary>
	public bool requireTap = true;

	/// <summary>
	/// When true, the panel will always show itself
	/// when clicked, and some other action will be
	/// required to dismiss/hide it.
	/// </summary>
	public bool alwaysShowOnClick = true;

	/// <summary>
	/// When true, the panel will dismiss itself
	/// when a "PRESS" event occurs anywhere
	/// outside of the panel.
	/// </summary>
	public bool dismissOnOutsideClick = true;

	/// <summary>
	/// When true, the panel dismisses itself when
	/// a control or panel which is not a child is
	/// clicked.
	/// </summary>
	public bool dismissOnPeerClick = false;

	/// <summary>
	/// When true, the panel will dismiss itself
	/// when one of its child controls are clicked.
	/// </summary>
	public bool dismissOnChildClick = false;

	/// <summary>
	/// When true, the panel will be dismissed when
	/// the pointer is moved away from the panel and
	/// its child controls.
	/// </summary>
	public bool dismissOnMoveOff = false;

	/// <summary>
	/// When true, if any child is clicked/tapped
	/// while the panel is hideAtStart, the panel will
	/// be shown.
	/// </summary>
	public bool showOnChildClick = true;

	/// <summary>
	/// The starting state of the panel.
	/// </summary>
	public STATE initialState = STATE.HIDDEN;

	
	// Keeps us from responding to the same action twice:
	protected int lastActionID=-1;

	// The type of the last pointer to interact with us:
	protected POINTER_INFO.POINTER_TYPE lastPtrType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;

	// The type of the last listener we registered
	protected POINTER_INFO.POINTER_TYPE lastListenerType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;


	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
			return;

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		lastPtrType = ptr.type;

// Only check for out-of-viewport input if such is possible
// (i.e. if we aren't on a hardware device where it is
// impossible to have input outside the viewport)
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID)
		// See if the input should be considered a move-off
		// because it is outside the viewport:
		if(dismissOnMoveOff)
		{
			if (ptr.devicePos.x < 0 ||
				ptr.devicePos.y < 0 ||
				ptr.devicePos.x > ptr.camera.pixelWidth ||
				ptr.devicePos.y > ptr.camera.pixelHeight)
			{
				// Interpret this as a move-off
				if (m_panelState == STATE.SHOWING)
					Hide();
				return;
			}
		}
#endif

		// Change the state if necessary:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if (!requireTap)
					PanelClicked(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.TAP:
				PanelClicked(ptr);
				break;
			/*
						case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
							// Check to see if we are still under 
							// the pointer and one of our child 
							// controls is just catching the hit 
							// instead:
							RaycastHit hit;
							if (collider != null)
							{
								if (collider.Raycast(ptr.ray, out hit, ptr.rayDepth))
								{
									// Change the event to a MOVE so that any
									// parent panel doesn't try its own raycast
									// and fail since the pointer is over us
									// instead:
									ptr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
								}
							}
							break;
			*/
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				// Do a raycast to see if we're
				// still under the pointer and one of
				// our child controls is just catching
				// the hit instead:
				RaycastHit hit;
				if (collider != null)
				{
					if (collider.Raycast(ptr.ray, out hit, ptr.rayDepth))
					{
						if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
						{
							// Change the event to a MOVE so that any
							// parent panel doesn't try its own raycast
							// and fail since the pointer is over us
							// instead:
							ptr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
						}
						else
						{
							ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
						}
					}
					else
					{
						if(dismissOnMoveOff)
						{
							if (m_panelState == STATE.SHOWING)
								SetPanelState(STATE.HIDDEN);
						}
					}
				}
				break;
		}

		base.OnInput(ptr);
	}

	protected void PanelClicked(POINTER_INFO ptr)
	{
		// If this is the same action to which we've
		// already responded, ignore:
		if (ptr.actionID == lastActionID)
			return;
		else
			lastActionID = ptr.actionID;

		// Is this a click from a child?
		if (ptr.callerIsControl)
		{
			// If we're hideAtStart, see if we should show:
			if (m_panelState == STATE.HIDDEN && showOnChildClick)
			{
				SetPanelState(STATE.SHOWING);
			}// If we're showing, see if we should dismiss:
			else if (m_panelState == STATE.SHOWING && dismissOnChildClick)
			{
				SetPanelState(STATE.HIDDEN);
			}
			return;
		}

		if (alwaysShowOnClick)
			SetPanelState(STATE.SHOWING);
		else
			ToggleState();
	}


	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	public void Awake()
	{
		m_panelState = initialState;
	}

	public void ToggleState()
	{
		if (m_panelState == STATE.HIDDEN)
			SetPanelState(STATE.SHOWING);
		else
			SetPanelState(STATE.HIDDEN);
	}

	// Switches the displayed sprite(s) to match the current state:
	protected void SetPanelState(STATE s)
	{
		// If this is the same as the current state, ignore:
		if (m_panelState == s)
			return;

		m_panelState = s;

		// See if we need to set/unset our listeners
		// for outside clicks:
		if (dismissOnPeerClick || dismissOnOutsideClick)
		{
			if (m_panelState == STATE.SHOWING)
			{
				if ((lastPtrType & POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD) == lastPtrType)
				{
					UIManager.instance.AddMouseTouchPtrListener(ClickListener);
					lastListenerType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;
				}
				else
				{
					UIManager.instance.AddRayPtrListener(ClickListener);
					lastListenerType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;
				}
			}
			else // Unset our listener:
			{
				if ((lastListenerType & POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD) == lastListenerType)
				{
					UIManager.instance.RemoveMouseTouchPtrListener(ClickListener);
				}
				else
				{
					UIManager.instance.RemoveRayPtrListener(ClickListener);
				}
			}
		}
		
		// Start a new transition:
		if(m_panelState == STATE.SHOWING)
			base.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		else
			base.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	// A pass-through for starting our transition so
	// that we capture our state:
	public override void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		if (mode == UIPanelManager.SHOW_MODE.BringInBack ||
		   mode == UIPanelManager.SHOW_MODE.BringInForward)
		{
			SetPanelState(STATE.SHOWING);
		}
		else
			SetPanelState(STATE.HIDDEN);
	}

	// Listens to clicks from outside us:
	protected void ClickListener(POINTER_INFO ptr)
	{
		// First see if this is a press:
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.PRESS)
			return; // Fuhgettaboutit

		// See if this is a click on nothing:
		if (ptr.targetObj == null && dismissOnOutsideClick)
		{
			SetPanelState(STATE.HIDDEN);
			return;
		}
		else if (!dismissOnPeerClick)
			return; // Nothing more to do

		// See if this is for us or a child:
		if(ptr.targetObj is Component)
			if (((Component)ptr.targetObj).transform.IsChildOf(transform))
				return; // Fuhgettaboutit

		// If we've gotten this far, it's a click on something else:
		if (dismissOnPeerClick)
			SetPanelState(STATE.HIDDEN);
	}

	/// <summary>
	/// Hides the panel by invoking its
	/// Dismiss Forward transition.
	/// </summary>
	public void Hide()
	{
		StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	/// <summary>
	/// Reveals the panel by invoking its
	/// Bring In Forward transition.
	/// </summary>
	public void Reveal()
	{
		StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIBistateInteractivePanel Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIBistateInteractivePanel)go.AddComponent(typeof(UIBistateInteractivePanel));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIBistateInteractivePanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIBistateInteractivePanel)go.AddComponent(typeof(UIBistateInteractivePanel));
	}
}
