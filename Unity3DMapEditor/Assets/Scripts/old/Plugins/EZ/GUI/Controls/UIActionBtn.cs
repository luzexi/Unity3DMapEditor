//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// A button that sets the UIManager's ray to an
/// active state when pressed.
/// This can be used for things such as first-person 
/// shooters on devices that do not have hardware buttons.
/// NOTE: to allow more than simple "clicking" of objects
/// in the scene (i.e. allow dragging, etc.) the button
/// must be set to repeat.
/// </remakrs>
[AddComponentMenu("EZ GUI/Controls/Action Button")] 
public class UIActionBtn : UIButton
{
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if(repeat)
		{
			UIManager.instance.RayActive = (controlState == CONTROL_STATE.ACTIVE)?UIManager.RAY_ACTIVE_STATE.Constant : UIManager.RAY_ACTIVE_STATE.Inactive;
		}
		else
		{
			if (ptr.evt == whenToInvoke)
				UIManager.instance.RayActive = UIManager.RAY_ACTIVE_STATE.Momentary;
		}

		base.OnInput(ref ptr);
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIActionBtn Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIActionBtn)go.AddComponent(typeof(UIActionBtn));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIActionBtn Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIActionBtn)go.AddComponent(typeof(UIActionBtn));
	}
}
