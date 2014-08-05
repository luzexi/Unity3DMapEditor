//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


#define AUTO_SET_LAYER
#define SET_LAYERS_RECURSIVELY


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// UIPanelManager serves as a means to manage multiple
/// UIPanels.  UIPanels which wish to be managed by a
/// UIPanelManager should be placed as children to the
/// GameObject that contains the UIPanelManager.
/// The main function of a UIPanelManager is to keep
/// track of the "current" panel that is being displayed
/// and to automatically bring in/dismiss panels as you
/// choose to display another panel instead.
/// In other words, a group of panels managed by a
/// UIPanelManager are mutually exclusive to one another.
/// If one panel is being shown, the others are considered
/// to be "dismissed" and out of view.  They may be out of
/// view, but needn't necessarily be so.  The central 
/// point is that only one panel per-manager is considered 
/// to be the "current" panel at any one time.
/// </remarks>
[System.Serializable]
[AddComponentMenu("EZ GUI/Management/Panel Manager")]
public class UIPanelManager : MonoBehaviour, IUIContainer
{
	/// <remarks>
	/// The mode in which this panel should be shown.
	/// Use these values to tell the panel to present
	/// or dismiss itself, either "forwards" or
	/// "backwards". The meaning of this is defined by
	/// either by the type of UIPanelManager you are
	/// using, or by how you have defined the 
	/// corresponding transitions.
	/// </remarks>
	public enum SHOW_MODE
	{
		/// <summary>
		/// Use this to show a panel in a manner that
		/// assumes the user is moving forwards in
		/// the menu.
		/// </summary>
		BringInForward,

		/// <summary>
		/// Use this to show a panel in a manner that
		/// assumes the user is moving backwards in
		/// the menu.
		/// </summary>
		BringInBack,

		/// <summary>
		/// Use this to dismiss a panel in a manner that
		/// assumes the user is moving forwards in
		/// the menu.
		/// </summary>
		DismissForward,

		/// <summary>
		/// Use this to dismiss a panel in a manner that
		/// assumes the user is forwards backwards in
		/// the menu.
		/// </summary>
		DismissBack
	}

	/// <remarks>
	/// Identifies the direction in which the menu
	/// is supposed to move.  Forwards or backwards.
	/// </remarks>
	public enum MENU_DIRECTION
	{
		/// <summary>
		/// Move forward in the menu.
		/// </summary>
		Forwards,

		/// <summary>
		/// Move backward in the menu.
		/// </summary>
		Backwards,

		/// <summary>
		/// Determine the direction of movement by 
		/// comparing indices of panels.
		/// Only valid when specifying which panel
		/// to bring up.
		/// </summary>
		Auto
	}
	
	protected static UIPanelManager m_instance;

	/// <summary>
	/// Accessor for the reference to the first instance of 
	/// UIPanelManager. Use this if you only have one in your 
	/// scene. Otherwise, you will need to refer directly to 
	/// the instance you need.
	/// </summary>
	public static UIPanelManager instance
	{
		get { return m_instance; }
	}

	public void OnDestroy()
	{
		m_instance = null;
	}

	// Our list of child panels
	protected List<UIPanelBase> panels = new List<UIPanelBase>();


	/// <summary>
	/// The panel to show at the start of a menu
	/// sequence.  This value overrides the indexing order
	/// in that the menu will begin at the initialPanel
	/// and then move from there based on index order
	/// (if using anonymous menu navigation).
	/// </summary>
	public UIPanelBase initialPanel;

	/// <summary>
	/// When true, all but the initial panel will be
	/// disabled at start.
	/// </summary>
	public bool deactivateAllButInitialAtStart = false;

	/// <summary>
	/// When true, the internal navigation logic of the
	/// manager assumes that all navigation will proceed
	/// one panel to the next in a predefined order 
	/// without "skipping around".
	/// Set this to true for wizard-like menus.
	/// </summary>
	public bool linearNavigation = false;

	/// <summary>
	/// When true, the panels are intended to be navigated
	/// in a circular fashion, meaning that when you
	/// advance to the next or previous panels, if the end
	/// has been reached, it wraps back to the beginning,
	/// or vice versa.
	/// This option also forces linear navigation (same as
	/// setting the lienarNavigation setting to true).
	/// </summary>
	public bool circular = false;

	/// <summary>
	/// When true, calling MoveBack() or MoveForward(),
	/// or using a control to do the same at the first
	/// panel or last panel in a series will cause the
	/// current panel to be dismissed and is replaced
	/// with nothing.  This is useful for when you want,
	/// for example, to be able to press a "Next" button
	/// at the last panel of a wizard and have it dismiss
	/// the last panel to make way for something else on
	/// the screen.
	/// </summary>
	public bool advancePastEnd = false;

	
	// Reference to the current panel
	protected UIPanelBase curPanel;

	// The total number of panels presently transitioning
	protected int transitioningPanelCount = 0;

	// Has Start() run yet?
	protected bool m_started = false;

	/// <summary>
	/// Returns a reference to the currently
	/// displaying panel, if any.
	/// </summary>
	public UIPanelBase CurrentPanel
	{
		get { return curPanel; }
	}


	// Holds a "history" of sorts of what panels we've viewed
	// so that we can use MoveBack() to return to a previous 
	// panel even in a non-linear panel series.
	protected List<UIPanelBase> breadcrumbs = new List<UIPanelBase>();


	public void AddChild(GameObject go)
	{
		UIPanelBase p = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));

		if (p == null)
			return;

		if (panels.IndexOf(p) >= 0)
			return; // Duplicate

		panels.Add(p);
		panels.Sort(UIPanelBase.CompareIndices);
		p.Container = this;
	}

	public void RemoveChild(GameObject go)
	{
		UIPanelBase p = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));

		if (p == null)
			return;

		panels.Remove(p);
		p.Container = null;
	}

	public void AddSubject(GameObject go)
	{
		// Do nothing
	}

	public void RemoveSubject(GameObject go)
	{
		// Do nothing
	}


	/// <summary>
	/// Makes the specified GameObject a child of
	/// the panel manager, as well as makes it a
	/// child of the panel manager's transform.
	/// </summary>
	/// <param name="go">The GameObject to be made a child of the panel manager.</param>
	public void MakeChild(GameObject go)
	{
		AddChild(go);

		go.transform.parent = transform;
	}


	void Awake()
	{
		if (m_instance == null)
			m_instance = this;
	}

	IEnumerator Start()
	{
		if (m_started)
			yield break;
		m_started = true;

		ScanChildren();

		if (initialPanel != null)
		{
			curPanel = initialPanel;
			breadcrumbs.Add(curPanel);
		}

		if (circular)
			linearNavigation = true;

		if(deactivateAllButInitialAtStart)
		{
			// Wait a frame so the contents of the panels
			// are done Start()'ing, or else we'll get
			// unhidden stuff:
			yield return null;

			for (int i = 0; i<panels.Count; ++i)
				if (panels[i] != initialPanel && panels[i] != curPanel)
					panels[i].gameObject.SetActiveRecursively(false);
		}
	}

	protected virtual void OnEnable()
	{
		// Only do this if we've already started:
		if(m_started)
		{
			if (deactivateAllButInitialAtStart)
			{
				for (int i = 0; i < panels.Count; ++i)
					if (panels[i] != curPanel)
						panels[i].gameObject.SetActiveRecursively(false);
			}
		}
	}


	// Scans all child objects looking for panels
	public void ScanChildren()
	{
		panels.Clear();

		UIPanelBase obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(UIPanelBase), true);

		for (int i = 0; i < objs.Length; ++i)
		{
#if AUTO_SET_LAYER
#if SET_LAYERS_RECURSIVELY
			SetLayerRecursively(objs[i].gameObject, gameObject.layer);
#else
			objs[i].gameObject.layer = gameObject.layer;
#endif
#endif
			obj = (UIPanelBase)objs[i];

			// Only add if we can be the immediate container:
			if (obj.RequestContainership(this))
				panels.Add(obj);
		}

		// Sort the list by index:
		panels.Sort(UIPanelBase.CompareIndices);
	}



	/// <summary>
	/// The number of panels presently in the process
	/// of transitioning.
	/// </summary>
	public int TransitioningPanelCount
	{
		get { return transitioningPanelCount; }
	}

	protected void DecrementTransitioningPanels(UIPanelBase p, EZTransition t)
	{
		--transitioningPanelCount;
	}

	// Starts a panel transitioning and tracks it
	protected void StartAndTrack(UIPanelBase p, SHOW_MODE mode)
	{
		p.StartTransition(mode);

		// See if it didn't quit immediately:
		if(p.IsTransitioning)
		{
			p.AddTempTransitionDelegate(DecrementTransitioningPanels);
			++transitioningPanelCount;
		}
	}


	/// <summary>
	/// Moves the menu forward to the next panel
	/// in the sequence (determined by panel 
	/// index).  Automatically dismisses the
	/// currently displaying panel using the 
	/// forward mode.
	/// </summary>
	/// <returns>True if the there are more panels 
	/// after this one. False if this is the last 
	/// panel, or if the menu was already at the end.</returns>
	public bool MoveForward()
	{
		StartCoroutine("Start");

		int index = panels.IndexOf(curPanel);

		if (index >= panels.Count - 1)
		{
			// See if we should wrap to the beginning:
			if (!circular)
			{
				// See if we should advance past the end:
				if (advancePastEnd)
				{
					// Send away the current panel:
					if (curPanel != null)
						StartAndTrack(curPanel, SHOW_MODE.DismissForward);

					curPanel = null;

					// Don't add more than one null on the stack:
					if(breadcrumbs.Count > 0)
					{
						if (breadcrumbs[breadcrumbs.Count - 1] != null)
							breadcrumbs.Add(null);
					}
					else
						breadcrumbs.Add(null);
				}
				return false; // We're already at the end
			}
			else
				index = -1; // Wrap back to the beginning
		}

		// Send away the current panel:
		if (curPanel != null)
			StartAndTrack(curPanel, SHOW_MODE.DismissForward);

		// Bring in the next panel:
		++index;
		curPanel = panels[index];
		breadcrumbs.Add(curPanel);

		if (deactivateAllButInitialAtStart && !curPanel.gameObject.active)
		{
			curPanel.Start();
			curPanel.gameObject.SetActiveRecursively(true);
		}

		StartAndTrack(curPanel, SHOW_MODE.BringInForward);

		if (index >= panels.Count - 1 && !circular)
			return false;
		else
			return true;
	}

	/// <summary>
	/// Moves the menu back to the previous panel
	/// in the sequence (determined by panel 
	/// index).  Automatically dismisses the
	/// currently displaying panel using the 
	/// "back" mode.
	/// </summary>
	/// <returns>True if the there are more panels 
	/// before this one. False if this is the first 
	/// panel, or if the menu was already at the 
	/// beginning.</returns>
	public bool MoveBack()
	{
		// If this isn't a linear menu, use our history
		// to go backward:
		if (!linearNavigation)
		{
			// See if we're at the beginning:
			if (breadcrumbs.Count <= 1)
			{
				if (advancePastEnd)
				{
					// Send away the current panel:
					if (curPanel != null)
						StartAndTrack(curPanel, SHOW_MODE.DismissBack);

					curPanel = null;

					// Don't add more than one null on the stack:
					if (breadcrumbs.Count > 0)
					{
						if (breadcrumbs[breadcrumbs.Count - 1] != null)
							breadcrumbs.Add(null);
					}
					else
						breadcrumbs.Add(null);
				}
				return false; // We're at the beginning
			}

			// Go back in our history:
			if (breadcrumbs.Count != 0)
				breadcrumbs.RemoveAt(breadcrumbs.Count-1);


			// Send away the current panel:
			if (curPanel != null)
				StartAndTrack(curPanel, SHOW_MODE.DismissBack);

			// Bring in the previous panel:
			if(breadcrumbs.Count > 0)
				curPanel = breadcrumbs[breadcrumbs.Count-1];
			if (curPanel != null)
			{
				if (deactivateAllButInitialAtStart && !curPanel.gameObject.active)
				{
					curPanel.Start();
					curPanel.gameObject.SetActiveRecursively(true);
				}

				StartAndTrack(curPanel, SHOW_MODE.BringInBack);
			}

			// Return false if we've reached the beginning:
			if (breadcrumbs.Count <= 1)
				return false;
			else
				return true;
		}
		else // Else this is a linear menu, so just go back to the previous panel in our array
		{
			int index = panels.IndexOf(curPanel);

			// If we're at the first index:
			if (index <= 0)
			{
				if(!circular)
				{
					if(advancePastEnd)
					{
						// Send away the current panel:
						if (curPanel != null)
							StartAndTrack(curPanel, SHOW_MODE.DismissBack);

						curPanel = null;
					}
					return false; // We're already at the beginning
				}
				else
				{
					// Wrap back to the end:
					index = panels.Count;
				}
			}

			// Send away the current panel:
			if (curPanel != null)
				StartAndTrack(curPanel, SHOW_MODE.DismissBack);

			// Bring in the previous panel:
			--index;
			curPanel = panels[index];

			if (deactivateAllButInitialAtStart && !curPanel.gameObject.active)
			{
				curPanel.Start();
				curPanel.gameObject.SetActiveRecursively(true);
			}

			StartAndTrack(curPanel, SHOW_MODE.BringInBack);

			if (index <= 0 && !circular)
				return false;
			else
				return true;
		}
	}

	/// <summary>
	/// Brings in the specified panel in a manner
	/// that portrays the menu moving in the
	/// specified direction.  If "Auto" is specified
	/// for the direction, forward/backward is
	/// determined by comparing the index of the
	/// current panel to the one being brought up.
	/// </summary>
	/// <param name="panel">Reference to the panel to bring up.</param>
	/// <param name="dir">Direction the menu should appear to be moving.
	/// If "Auto" is specified, the direction is determined by comparing
	/// the index of the current panel to the one being brought up.</param>
	public void BringIn(UIPanelBase panel, MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		SHOW_MODE dismissMode;
		SHOW_MODE bringInMode;

		if (curPanel == panel)
			return; // Nothing to do!

		if(dir == MENU_DIRECTION.Auto)
		{
			// See if we can determine the direction:
			if (curPanel != null)
			{
				// Forward
				if (curPanel.index <= panel.index)
					dir = MENU_DIRECTION.Forwards;
				else // Backward
					dir = MENU_DIRECTION.Backwards;
			}
			else // Assume forward:
				dir = MENU_DIRECTION.Forwards;
		}

		dismissMode = ( (dir == MENU_DIRECTION.Forwards) ? (SHOW_MODE.DismissForward) : (SHOW_MODE.DismissBack) );
		bringInMode = ( (dir == MENU_DIRECTION.Forwards) ? (SHOW_MODE.BringInForward) : (SHOW_MODE.BringInBack) );

		// Dismiss the current panel:
		if (curPanel != null)
			StartAndTrack(curPanel, dismissMode);

		// Bring in the next panel:
		curPanel = panel;
		breadcrumbs.Add(curPanel);
		if (deactivateAllButInitialAtStart && !curPanel.gameObject.active)
		{
			curPanel.Start();
			curPanel.gameObject.SetActiveRecursively(true);
		}
		StartAndTrack(curPanel, bringInMode);
	}


	/// <summary>
	/// Same as BringIn(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding BringIn() entry for more details.
	/// </summary>
	/// <param name="panel">Reference to the panel to bring up.</param>
	/// <param name="dir">Direction the menu should appear to be moving.
	/// If "Auto" is specified, the direction is determined by comparing
	/// the index of the current panel to the one being brought up.</param>
	public void BringInImmediate(UIPanelBase panel, MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		UIPanelBase prevPanel = curPanel;
		EZTransition trans;

		// Get the transition directions:
		if(dir == MENU_DIRECTION.Auto)
		{
			// See if we can determine the direction:
			if (curPanel != null)
			{
				// Forward
				if (curPanel.index <= panel.index)
					dir = MENU_DIRECTION.Forwards;
				else // Backward
					dir = MENU_DIRECTION.Backwards;
			}
			else // Assume forward:
				dir = MENU_DIRECTION.Forwards;
		}

		SHOW_MODE dismissMode = ( (dir == MENU_DIRECTION.Forwards) ? (SHOW_MODE.DismissForward) : (SHOW_MODE.DismissBack) );
		SHOW_MODE bringInMode = ((dir == MENU_DIRECTION.Forwards) ? (SHOW_MODE.BringInForward) : (SHOW_MODE.BringInBack));

		// Do the bring-in:
		BringIn(panel, dir);

		// End the transitions early:
		if(prevPanel != null)
		{
			trans = prevPanel.GetTransition(dismissMode);
			trans.End();
		}
		if(curPanel != null)
		{
			trans = curPanel.GetTransition(bringInMode);
			trans.End();
		}
	}

	/// <summary>
	/// Brings in the specified panel in a manner
	/// that portrays the menu moving in the
	/// specified direction.  If "Auto" is specified
	/// for the direction, forward/backward is
	/// determined by comparing the index of the
	/// current panel to the one being brought up.
	/// </summary>
	/// <param name="panelName">Name of the panel to bring up.</param>
	/// <param name="dir">Direction the menu should appear to be moving.
	/// If "Auto" is specified, the direction is determined by comparing
	/// the index of the current panel to the one being brought up.</param>
	public void BringIn(string panelName, MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		UIPanelBase panel = null;

		for (int i = 0; i < panels.Count; ++i)
		{
			if (string.Equals(panels[i].name, panelName, System.StringComparison.CurrentCultureIgnoreCase))
			{
				panel = panels[i];
				break;
			}
		}

		if (panel != null)
			BringIn(panel, dir);
	}

	/// <summary>
	/// Brings in the specified panel in a manner
	/// that portrays the menu moving in the direction 
	/// determined by comparing the index of the
	/// current panel to the one being brought up.
	/// </summary>
	/// <param name="panel">Reference to the panel to bring up.</param>
	public void BringIn(UIPanelBase panel)
	{
		BringIn(panel, MENU_DIRECTION.Auto);
	}

	/// <summary>
	/// Brings in the specified panel in a manner
	/// that portrays the menu moving in the direction 
	/// determined by comparing the index of the
	/// current panel to the one being brought up.
	/// </summary>
	/// <param name="panelName">Name of the panel to bring up.</param>
	public void BringIn(string panelName)
	{
		BringIn(panelName, MENU_DIRECTION.Auto);
	}

	/// <summary>
	/// Brings in the specified panel in a manner
	/// that portrays the menu moving in the direction 
	/// determined by comparing the index of the
	/// current panel to the one being brought up.
	/// </summary>
	/// <param name="panelIndex">Index of the panel.</param>
	public void BringIn(int panelIndex)
	{
		StartCoroutine("Start");

		for (int i = 0; i < panels.Count; ++i)
		{
			if (panels[i].index == panelIndex)
			{
				BringIn(panels[i]);
				return;
			}
		}

		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	/// <summary>
	/// Brings in the specified panel in a manner
	/// that portrays the menu moving in the
	/// specified direction.  If "Auto" is specified
	/// for the direction, forward/backward is
	/// determined by comparing the index of the
	/// current panel to the one being brought up.
	/// </summary>
	/// <param name="panelIndex">Index of the panel to bring in.</param>
	/// <param name="dir">Direction the menu should appear to be moving.
	/// If "Auto" is specified, the direction is determined by comparing
	/// the index of the current panel to the one being brought up.</param>
	public void BringIn(int panelIndex, MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		for (int i = 0; i < panels.Count; ++i)
		{
			if (panels[i].index == panelIndex)
			{
				BringIn(panels[i], dir);
				return;
			}
		}

		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	/// <summary>
	/// Same as BringIn(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding BringIn() entry for more details.
	/// </summary>
	/// <param name="panelName">Name of the panel to bring up.</param>
	/// <param name="dir">Direction the menu should appear to be moving.
	/// If "Auto" is specified, the direction is determined by comparing
	/// the index of the current panel to the one being brought up.</param>
	public void BringInImmediate(string panelName, MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		UIPanelBase panel = null;

		for (int i = 0; i < panels.Count; ++i)
		{
			if (string.Equals(panels[i].name, panelName, System.StringComparison.CurrentCultureIgnoreCase))
			{
				panel = panels[i];
				break;
			}
		}

		if (panel != null)
			BringInImmediate(panel, dir);
	}

	/// <summary>
	/// Same as BringIn(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding BringIn() entry for more details.
	/// </summary>
	/// <param name="panel">Reference to the panel to bring up.</param>
	public void BringInImmediate(UIPanelBase panel)
	{
		BringInImmediate(panel, MENU_DIRECTION.Auto);
	}

	/// <summary>
	/// Same as BringIn(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding BringIn() entry for more details.
	/// </summary>
	/// <param name="panelName">Name of the panel to bring up.</param>
	public void BringInImmediate(string panelName)
	{
		BringInImmediate(panelName, MENU_DIRECTION.Auto);
	}

	/// <summary>
	/// Same as BringIn(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding BringIn() entry for more details.
	/// </summary>
	/// <param name="panelIndex">Index of the panel.</param>
	public void BringInImmediate(int panelIndex)
	{
		StartCoroutine("Start");

		for (int i = 0; i < panels.Count; ++i)
		{
			if (panels[i].index == panelIndex)
			{
				BringInImmediate(panels[i]);
				return;
			}
		}

		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	/// <summary>
	/// Same as BringIn(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding BringIn() entry for more details.
	/// </summary>
	/// <param name="panelIndex">Index of the panel to bring in.</param>
	/// <param name="dir">Direction the menu should appear to be moving.
	/// If "Auto" is specified, the direction is determined by comparing
	/// the index of the current panel to the one being brought up.</param>
	public void BringInImmediate(int panelIndex, MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		for (int i = 0; i < panels.Count; ++i)
		{
			if (panels[i].index == panelIndex)
			{
				BringInImmediate(panels[i], dir);
				return;
			}
		}

		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}


	/// <summary>
	/// Dismisses the currently showing panel, if any,
	/// in the direction specified.
	/// </summary>
	/// <param name="dir">The direction in which the panel is to be dismissed.</param>
	public void Dismiss(MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		if (dir == MENU_DIRECTION.Auto)
			dir = MENU_DIRECTION.Backwards;

		SHOW_MODE mode = ( (dir == MENU_DIRECTION.Forwards) ? (SHOW_MODE.DismissForward) : (SHOW_MODE.DismissBack) );

		if (curPanel != null)
			StartAndTrack(curPanel, mode);

		curPanel = null;

		// Only push on a null ref if there
		// isn't already one on the stack:
		if(breadcrumbs.Count > 0)
			if (breadcrumbs[breadcrumbs.Count-1] != null)
				breadcrumbs.Add(null);
	}

	/// <summary>
	/// Dismisses the currently showing panel, if any,
	/// in the direction specified.
	/// </summary>
	public void Dismiss()
	{
		Dismiss(MENU_DIRECTION.Auto);
	}

	/// <summary>
	/// Same as Dismiss(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding Dismiss() entry for more details.
	/// </summary>
	/// <param name="dir">The direction in which the panel is to be dismissed.</param>
	public void DismissImmediate(MENU_DIRECTION dir)
	{
		StartCoroutine("Start");

		if (dir == MENU_DIRECTION.Auto)
			dir = MENU_DIRECTION.Backwards;

		SHOW_MODE mode = ((dir == MENU_DIRECTION.Forwards) ? (SHOW_MODE.DismissForward) : (SHOW_MODE.DismissBack));

		UIPanelBase prevPanel = curPanel;

		Dismiss(dir);

		if(prevPanel != null)
		{
			prevPanel.GetTransition(mode).End();
		}
	}

	/// <summary>
	/// Same as Dismiss(...), but skips the panel's transition, fast-forwarding
	/// it instantly to its end state.  See the corresponding Dismiss() entry for more details.
	/// </summary>
	public void DismissImmediate()
	{
		DismissImmediate(MENU_DIRECTION.Auto);
	}


	// Sets the layer of a GameObject and all its children
	public static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;

		foreach (Transform child in go.transform)
		{
			SetLayerRecursively(child.gameObject, layer);
		}
	}



	//---------------------------------------------------
	// IUIObject interface stuff
	//---------------------------------------------------
	protected bool m_controlIsEnabled = true;

	public virtual bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set { m_controlIsEnabled = value; }
	}

	public virtual bool DetargetOnDisable
	{
		get { return false; }
		set { ; }
	}

	// Allows an object to act as a proxy for other
	// controls - i.e. a UIVirtualScreen
	// But in our case, just return ourselves since
	// we're not acting as a proxy
	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		return this;
	}

	protected IUIContainer container;

	public virtual IUIContainer Container
	{
		get { return container; }
		set { container = value; }
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform t = transform.parent;
		Transform c = ((Component)cont).transform;

		while (t != null)
		{
			if (t == c)
			{
				container = cont;
				return true;
			}
			else if (t.gameObject.GetComponent("IUIContainer") != null)
				return false;

			t = t.parent;
		}

		// Never found *any* containers:
		return false;
	}

	public bool GotFocus() { return false; }

	protected EZInputDelegate inputDelegate;
	protected EZValueChangedDelegate changeDelegate;
	public virtual void SetInputDelegate(EZInputDelegate del)
	{
		inputDelegate = del;
	}
	public virtual void AddInputDelegate(EZInputDelegate del)
	{
		inputDelegate += del;
	}
	public virtual void RemoveInputDelegate(EZInputDelegate del)
	{
		inputDelegate -= del;
	}
	public virtual void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = del;
	}
	public virtual void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate += del;
	}
	public virtual void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate -= del;
	}

	public virtual void OnInput(POINTER_INFO ptr) { }


	#region Drag&Drop

	//---------------------------------------------------
	// Drag & Drop stuff
	//---------------------------------------------------

	public object Data
	{
		get { return null; }
		set { }
	}

	public bool IsDraggable
	{
		get { return false; }
		set { }
	}

	public LayerMask DropMask
	{
		get { return -1; }
		set { }
	}

	public float DragOffset
	{
		get { return 0; }
		set { }
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get { return EZAnimation.EASING_TYPE.Default; }
		set { }
	}

	public float CancelDragDuration
	{
		get { return 0; }
		set { }
	}

	public bool IsDragging
	{
		get { return false; }
		set { }
	}

	public GameObject DropTarget
	{
		get { return null; }
		set { }
	}

	public bool DropHandled
	{
		get { return false; }
		set { }
	}

	public void DragUpdatePosition(POINTER_INFO ptr) { }

	public void CancelDrag() { }

	// <summary>
	// Receives regular notification of drag & drop events
	// pertaining to this object when an object is being
	// dragged.  This is called on potential drop targets
	// when an object is dragged over them.  It is also
	// called on the object(s) being dragged/dropped.
	// </summary>
	// <param name="parms">The EZDragDropParams structure which holds information about the event.</param>
	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		if (dragDropDelegate != null)
			dragDropDelegate(parms);
	}

	// Delegate to be called on drag and drop notifications.
	protected EZDragDropDelegate dragDropDelegate = null;

	/// <summary>
	/// Adds a delegate to be called with drag and drop notifications.
	/// </summary>
	/// <param name="del">The delegate to add.</param>
	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropDelegate += del;
	}

	/// <summary>
	/// Removes a delegate from the list of those to be called 
	/// with drag and drop notifications.
	/// </summary>
	/// <param name="del">The delegate to add.</param>
	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropDelegate -= del;
	}

	/// <summary>
	/// Sets the delegate to be called with drag and drop notifications.
	/// NOTE: This will replace any previously registered delegates.
	/// </summary>
	/// <param name="del">The delegate to add.</param>
	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropDelegate = del;
	}

	// Setters for the internal drag drop handler delegate:
	public void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate() { return null; }


	#endregion

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIPanelManager Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIPanelManager)go.AddComponent(typeof(UIPanelManager));
	}
}