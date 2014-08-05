//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// When clicked, causes a UIPanelManager to display
/// either a specific panel, or the next or prevoius
/// panel in its sequence, depending on the value of
/// changeType.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Change Panel Button")]
public class UIBtnChangePanel : UIButton
{
	/// <remarks>
	/// The manner in which the button causes the panel manager
	/// to change panels.
	/// </remarks>
	public enum ChangeType
	{
		/// <summary>
		/// Bring in a specific panel by name.
		/// The panel manager will determine
		/// whether movement is forward or
		/// backward based upon the index of
		/// the incoming panel versus the index
		/// of the existing panel.
		/// </summary>
		BringIn,

		/// <summary>
		/// Same as BringIn, but always uses the 
		/// forward direction.
		/// </summary>
		BringInForward,

		/// <summary>
		/// Same as BringIn, but always uses the
		/// backward direction.
		/// </summary>
		BringInBack,

		/// <summary>
		/// Dismisses the panel specified by name.
		/// </summary>
		Dismiss,

		/// <summary>
		/// Dismisses the currently showing panel (forward).
		/// </summary>
		DismissCurrent,
		
		/// <summary>
		/// Brings in the panel specified by name
		/// if it is not the current panel, or
		/// dismisses (forwards) it if it is.
		/// </summary>
		Toggle,

		/// <summary>
		/// Go to the next panel in the sequence.
		/// </summary>
		Forward,

		/// <summary>
		/// Go to the previous panel in the sequence.
		/// </summary>
		Back,

		/// <summary>
		/// Works like BringIn, but skips the panel's 
		/// transition, jumping immediately to its
		/// end state).
		/// </summary>
		BringInImmediate,

		/// <summary>
		/// Works like DismissCurrent, but skips the panel's
		/// transition, jumping immediately to its
		/// end state).
		/// </summary>
		DismissImmediate
	}

	/// <summary>
	/// The panel manager that contains the panel(s)
	/// we will be bringing up/dismissing.
	/// This can be left to None/null if there is
	/// only one UIPanelManager object in the scene.
	/// </summary>
	public UIPanelManager panelManager;

	/// <summary>
	/// The type of change to execute.
	/// </summary>
	public ChangeType changeType;

	/// <summary>
	/// When true, all other active pointers will detarget
	/// their controls, ensuring that other pointers are
	/// not interacting with controls while this transition
	/// takes place.
	/// </summary>
	public bool detargetAllOthers = true;

	/// <summary>
	/// (Optional) The name of the panel to be brought in.
	/// </summary>
	public string panel;

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if (ptr.evt == whenToInvoke)
		{
			if (panelManager == null)
			{
				if (UIPanelManager.instance == null)
				{
					base.OnInput(ref ptr);
					return;
				}
				else
					panelManager = UIPanelManager.instance;

				// If we still have nothing, return:
				if(panelManager == null)
				{
					base.OnInput(ref ptr);
					return;
				}
			}


			if (detargetAllOthers)
				UIManager.instance.DetargetAllExcept(ptr.id);


			switch(changeType)
			{
				case ChangeType.BringIn:
					panelManager.BringIn(panel);
					break;
				case ChangeType.BringInImmediate:
					panelManager.BringInImmediate(panel);
					break;
				case ChangeType.BringInForward:
					panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
					break;
				case ChangeType.BringInBack:
					panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Backwards);
					break;
				case ChangeType.Dismiss:
					if(panelManager.CurrentPanel != null)
					{
						if(string.Equals(panelManager.CurrentPanel.name, panel, System.StringComparison.CurrentCultureIgnoreCase))
						{
							panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
						}
					}
					break;
				case ChangeType.DismissCurrent:
					panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
					break;
				case ChangeType.DismissImmediate:
					panelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
					break;
				case ChangeType.Toggle:
					if (panelManager != null &&
						panelManager.CurrentPanel != null && 
						string.Equals(panelManager.CurrentPanel.name, panel, System.StringComparison.CurrentCultureIgnoreCase))
					{
						panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
					}
					else
					{
						panelManager.BringIn(panel);
					}
					break;
				case ChangeType.Forward:
					panelManager.MoveForward();
					break;
				case ChangeType.Back:
					panelManager.MoveBack();
					break;
			}
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

		if (!(s is UIBtnChangePanel))
			return;

		UIBtnChangePanel b = (UIBtnChangePanel)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			panelManager = b.panelManager;
			changeType = b.changeType;
			panel = b.panel;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIBtnChangePanel Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIBtnChangePanel)go.AddComponent(typeof(UIBtnChangePanel));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIBtnChangePanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIBtnChangePanel)go.AddComponent(typeof(UIBtnChangePanel));
	}
}
