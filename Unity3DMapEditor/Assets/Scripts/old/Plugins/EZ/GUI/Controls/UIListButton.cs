//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;

/// <remarks>
/// Serves as an item in a list, such as a
/// scroll list.  This differs from UIListItem
/// in that it is not "selectable" and behaves
/// just like a button.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/List Button")]
public class UIListButton : UIListItem
{
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		if (!m_controlIsEnabled /*|| IsHidden()*/)
		{
			DoNeccessaryInput(ref ptr);			
			return;
		}

		// Do our own tap checking with the list's
		// own threshold:
		if (list != null && Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > (list.dragThreshold * list.dragThreshold))
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
		}
		else
			ptr.isTap = true;

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Check to see if we're disabled or hidden again in case
		// we were disabled by an input delegate:
		if (!m_controlIsEnabled)
		{
			DoNeccessaryInput(ref ptr);
			return;
		}


		// Change the state if necessary:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				if (ptr.active && list != null)	// If this is a hold
					list.ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (soundOnOver != null && m_ctrlState != CONTROL_STATE.OVER)
					soundOnOver.PlayOneShot(soundOnOver.clip);

				SetControlState(CONTROL_STATE.OVER);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (!ptr.isTap)
				{
					SetControlState(CONTROL_STATE.NORMAL);
					if (list != null)
						list.ListDragged(ptr);
				}
				else
					SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.PRESS:
				SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.TAP:
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				if (list != null && ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
					list.DidClick(this);

				if (list != null)
					list.PointerReleased();

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
				SetControlState(CONTROL_STATE.NORMAL);
				break;
		}

		// Apply any mousewheel scrolling to our list:
		if (list != null && ptr.inputDelta.z != 0 && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			list.ScrollWheel(ptr.inputDelta.z);
		}

		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}

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


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIListButton Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIListButton)go.AddComponent(typeof(UIListButton));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIListButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIListButton)go.AddComponent(typeof(UIListButton));
	}
}
