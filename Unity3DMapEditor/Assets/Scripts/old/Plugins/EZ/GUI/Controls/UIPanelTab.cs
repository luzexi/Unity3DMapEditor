//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Allows functionality similar to a tab when paired
/// with a UIPanel such that when the button is clicked,
/// the associated panel will be shown.
/// By setting up a UIPanelManager with multiple panels
/// and pairing each panel with a UIPanelTab control,
/// you can create the effect of a tabbed interface.
/// When set to toggle, dismissals use the forward
/// transition type.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Panel Tab")]
public class UIPanelTab : UIRadioBtn
{
	/// <summary>
	/// When true, the button will toggle the
	/// associated panel on and off with each
	/// press of the tab.
	/// </summary>
	public bool toggle;

	/// <summary>
	/// The optional panel manager that contains the panel(s)
	/// we will be bringing up/dismissing.
	/// This can be left to None/null if there is
	/// only one UIPanelManager object in the scene.
	/// NOTE: For other panels to be hideAtStart when this
	/// one is shown requires the use of a UIPanelManager.
	/// </summary>
	public UIPanelManager panelManager;

	/// <summary>
	/// Reference to the panel to show/hide.
	/// </summary>
	public UIPanelBase panel;

	/// <summary>
	/// Indicates whether the associated panel
	/// is to be considered to be showing at
	/// the start.  This value is only used if
	/// the panel is not associated with a
	/// UIPanelManager.  This allows toggling
	/// to keep track of when the panel needs
	/// to be shown or dismissed.
	/// </summary>
	public bool panelShowingAtStart = true;

	protected bool panelIsShowing = true;

	// Flag to let us know when the .Value property
	// is being accessed internally
	protected bool internalCall = false;


	public override bool Value
	{
		get { return base.Value; }
		set
		{
			base.Value = value;

			// Only need to process if the new value
			// is out of sync with the panel state
			// if we're not in toggle mode:
			if (!toggle)
			{
				if (panelIsShowing == value)
					return;
			}
			else
			{
				if (internalCall)
					return;
			}

			if (panelManager != null)
			{
				if (value)
					panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
				else if (panelManager.CurrentPanel == panel)
					panelManager.Dismiss();
			}
			else
			{
				if (value)
					panel.BringIn();
				else
					panel.Dismiss();
			}

			panelIsShowing = value;
		}
	}

	public override void Start()
	{
		if (m_started)
			return;

		base.Start();

		if(Application.isPlaying)
		{
			// Try to get a manager:
			if (panelManager == null)
			{
				if (panel != null)
					if (panel.Container != null)
						panelManager = (UIPanelManager)panel.Container;

				// If we still don't have anything:
				/*
							if(panelManager == null)
								if (UIPanelManager.instance != null)
									panelManager = UIPanelManager.instance;
				*/
			}

			panelIsShowing = panelShowingAtStart;
			Value = panelShowingAtStart;
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		// We will now be calling .Value internally:
		internalCall = true;

		base.OnInput(ref ptr);

		if (!m_controlIsEnabled || IsHidden())
		{
			return;
		}

		if(panel == null)
			return;

		if (ptr.evt == whenToInvoke)
		{
			DoPanelStuff();
		}

		// We're done with our internal calls to .Value:
		internalCall = false;
	}

	protected void DoPanelStuff()
	{
		if (toggle)
		{
			if (panelManager != null)
			{
				if (panelManager.CurrentPanel == panel)
				{
					panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
					panelIsShowing = false;
				}
				else
				{
					panelManager.BringIn(panel);
					panelIsShowing = true;
				}
			}
			else
			{
				if (panelIsShowing)
					panel.Dismiss();
				else
					panel.BringIn();

				panelIsShowing = !panelIsShowing;
			}

			base.Value = panelIsShowing;
		}
		else
		{
			if (panelManager != null)
				panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
			else
				panel.BringIn();
		}
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIPanelTab))
			return;

		UIPanelTab b = (UIPanelTab)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			toggle = b.toggle;
			panelManager = b.panelManager;
			panel = b.panel;
			panelShowingAtStart = b.panelShowingAtStart;
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			Value = b.Value;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIPanelTab Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIPanelTab)go.AddComponent(typeof(UIPanelTab));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIPanelTab Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIPanelTab)go.AddComponent(typeof(UIPanelTab));
	}
}
