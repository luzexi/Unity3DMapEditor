//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// A type of panel that responds to user input.
/// Use this to implement draggable panels, or
/// something like your own OSX dock-like panel of
/// controls.
/// </remarks>
[System.Serializable]
[AddComponentMenu("EZ GUI/Panels/Interactive Panel")]
public class UIInteractivePanel : UIPanelBase
{
	/// <summary>
	/// Indicates the state of the button
	/// </summary>
	public enum STATE
	{
		/// <summary>
		/// The panel is "normal", awaiting input
		/// </summary>
		NORMAL,

		/// <summary>
		/// The panel has an input device hovering over it.
		/// </summary>
		OVER,

		/// <summary>
		/// The panel is being dragged.
		/// </summary>
		DRAGGING
	};


	protected STATE m_panelState;

	/// <summary>
	/// Gets the current state of the panel.
	/// </summary>
	public STATE State
	{
		get { return m_panelState; }
	}


	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList transitions = new EZTransitionList(new EZTransition[]
		{
			new EZTransition("Bring In Forward"),
			new EZTransition("Bring In Back"),
			new EZTransition("Dismiss Forward"),
			new EZTransition("Dismiss Back"),
			new EZTransition("Normal from Over"),
			new EZTransition("Normal from Dragging"),
			new EZTransition("Over from Normal"),
			new EZTransition("Over from Dragging"),
			new EZTransition("Dragging")
		});

	public override EZTransitionList Transitions
	{
		get { return transitions; }
	}


	/// <summary>
	/// When true, the panel can be dragged by clicking anywhere
	/// on its collider and dragging to reposition it.
	/// </summary>
	public bool draggable = false;

	/// <summary>
	/// If the draggable option is enabled, this setting indicates
	/// whether the position of the panel should be constrained to
	/// a certain area in world space when being dragged.  If set
	/// to true, the panel's world space position will be
	/// constrained to the boundary specified by the dragBondaryMin
	/// and dragBoundaryMax points.
	/// </summary>
	public bool constrainDragArea = false;

	/// <summary>
	/// NOTE: Only has effect if draggable and constrainDragArea
	/// are set to true.
	/// Specifies the minimum point in space where the panel can
	/// be dragged.
	/// </summary>
	public Vector3 dragBoundaryMin;

	/// <summary>
	/// NOTE: Only has effect if draggable and constrainDragArea
	/// are set to true.
	/// Specifies the maximum point in space where the panel can
	/// be dragged.
	/// </summary>
	public Vector3 dragBoundaryMax;


	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
			return;

		if (inputDelegate != null)
			inputDelegate(ref ptr);

// Only check for out-of-viewport input if such is possible
// (i.e. if we aren't on a hardware device where it is
// impossible to have input outside the viewport)
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID)
		// See if the input should be considered a move-off
		// because it is outside the viewport:
		if (ptr.devicePos.x < 0 ||
			ptr.devicePos.y < 0 ||
			ptr.devicePos.x > ptr.camera.pixelWidth ||
			ptr.devicePos.y > ptr.camera.pixelHeight)
		{
			if (m_panelState == STATE.OVER)
				SetPanelState(STATE.NORMAL);

			// Only continue if we are dragging the panel:
			if (m_panelState != STATE.DRAGGING)
				return;
		}
#endif

		// Change the state if necessary:
		switch(ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (m_panelState != STATE.OVER)
				{
					SetPanelState(STATE.OVER);
				}
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if(draggable && !ptr.callerIsControl)
				{
					// Early out:
					if (ptr.inputDelta.sqrMagnitude == 0)
						break; ;

					float dist;
					Vector3 pt1, pt2;
					Plane plane = default(Plane);

					plane.SetNormalAndPosition(transform.forward * -1f, transform.position);
					
					plane.Raycast(ptr.ray, out dist);
					pt1 = ptr.ray.origin + ptr.ray.direction * dist;

					plane.Raycast(ptr.prevRay, out dist);
					pt2 = ptr.prevRay.origin + ptr.prevRay.direction * dist;

					pt1 = transform.position + (pt1 - pt2);

					if(constrainDragArea)
					{
						pt1.x = Mathf.Clamp(pt1.x, dragBoundaryMin.x, dragBoundaryMax.x);
						pt1.y = Mathf.Clamp(pt1.y, dragBoundaryMin.y, dragBoundaryMax.y);
						pt1.z = Mathf.Clamp(pt1.z, dragBoundaryMin.z, dragBoundaryMax.z);
					}

					transform.position = pt1;

					SetPanelState(STATE.DRAGGING);
				}
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
// 				if (!ptr.callerIsControl)
// 					SetPanelState(STATE.NORMAL);
// 				else
				{	// Else do a raycast to see if we're
					// still under the pointer and one of
					// our child controls is just catching
					// the hit instead:
					RaycastHit hit;
					if (collider != null)
					{
						if (!collider.Raycast(ptr.ray, out hit, ptr.rayDepth))
						{
							SetPanelState(STATE.NORMAL);
						}
						else
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
					}
				}
				break;
		}

		base.OnInput(ptr);
	}

	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------

	// Switches the displayed sprite(s) to match the current state:
	protected void SetPanelState(STATE s)
	{
		// If this is the same as the current state, ignore:
		if (m_panelState == s)
			return;

		STATE prevState = m_panelState;
		m_panelState = s;

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		// Start a new transition:
		StartTransition(s, prevState);
	}

	// Starts the appropriate transition
	protected void StartTransition(STATE s, STATE prevState)
	{
		int transIndex;

		switch(s)
		{
			case STATE.NORMAL:
				if (prevState == STATE.OVER)
					transIndex = 4;
				else
					transIndex = 5;
				break;
			case STATE.OVER:
				if (prevState == STATE.NORMAL)
					transIndex = 6;
				else
					transIndex = 7;
				break;
			case STATE.DRAGGING:
				transIndex = 8;
				break;
			default:
				transIndex = 4;
				break;
		}

		prevTransition = transitions.list[transIndex];
		prevTransition.Start();
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
	static public UIInteractivePanel Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIInteractivePanel)go.AddComponent(typeof(UIInteractivePanel));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIInteractivePanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIInteractivePanel)go.AddComponent(typeof(UIInteractivePanel));
	}
}
