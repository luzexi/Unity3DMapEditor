//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;

/// <summary>
/// Acts as a slider knob for a slider bar.
/// </summary>
public class UIScrollKnob : UIButton
{
	protected Vector3 origPos;		// The starting position of the knob
	protected UISlider slider;		// The object the knob will be scrolling
	protected float maxScrollPos;	// The farthest, in world units, the scroll knob is allowed to go from its starting position
	protected Plane ctrlPlane;		// The plane in which the knob sits opposite the camera
	protected Vector2 colliderSizeFactor; // Factor by which our size will be multiplied to set the size of our collider.
	protected float colliderExtent;	// The extent of our collider on the X-axis (width/2)


	// Working vars:
	float dist;
	Vector3 inputPoint;
	Vector3 newPos;
	Vector3 prevPoint;


	protected override void Awake()
	{
		base.Awake();
		origPos = transform.localPosition;
	}


	public override void OnInput(ref POINTER_INFO ptr)
	{
		base.OnInput(ref ptr);

		if (!m_controlIsEnabled)
		{
			return;
		}

		switch(ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.PRESS:
				// Save this input point:
				prevPoint = GetLocalInputPoint(ptr.ray);
				break;

			case POINTER_INFO.INPUT_EVENT.DRAG:
				inputPoint = GetLocalInputPoint(ptr.ray);

				dist = inputPoint.x - prevPoint.x;

				// Save this as our previous point:
				prevPoint = inputPoint;

				newPos = transform.localPosition;
				newPos.x = Mathf.Clamp(newPos.x + dist, origPos.x, origPos.x + maxScrollPos);

				transform.localPosition = newPos;
				prevPoint.x = Mathf.Clamp(prevPoint.x, origPos.x - colliderExtent, origPos.x + colliderExtent + maxScrollPos);

				// Inform the slider that we've moved:
				slider.ScrollKnobMoved(this, GetScrollPos());
				break;
		}
	}

	public void SetStartPos(Vector3 startPos)
	{
		origPos = startPos;
	}

	// Returns the location of the input point in local space.
	protected Vector3 GetLocalInputPoint(Ray ray)
	{
		// Get the location of the input device in world space:

		// Calculate the plane in which the knob sits:
		ctrlPlane.SetNormalAndPosition(transform.forward * -1f, transform.position);

		ctrlPlane.Raycast(ray, out dist);

		// Get the input point into the local space of our parent:
		return transform.parent.InverseTransformPoint(ray.origin + ray.direction * dist);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIScrollKnob))
			return;

		UIScrollKnob b = (UIScrollKnob)s;


		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			origPos = b.origPos;
			ctrlPlane = b.ctrlPlane;
			slider = b.slider;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			maxScrollPos = b.maxScrollPos;
			colliderSizeFactor = b.colliderSizeFactor;
		}
	}


	public void SetColliderSizeFactor(Vector2 csf)
	{
		colliderSizeFactor = csf;
	}

	public override void UpdateCollider()
	{
		base.UpdateCollider();

		if (!(collider is BoxCollider) || IsHidden())
			return;

		BoxCollider bc = (BoxCollider)collider;
		bc.size = new Vector3(bc.size.x * colliderSizeFactor.x, bc.size.y * colliderSizeFactor.y, 0.001f);
		colliderExtent = bc.size.x * 0.5f;
	}


	/// <summary>
	/// Returns a value in the range 0-1 indicating the scroll position.
	/// 0 indicates the starting position of the knob, 1
	/// indicates the end position.
	/// </summary>
	/// <returns>A value in the range 0-1 indicating the scroll position.</returns>
	public float GetScrollPos()
	{
		return (transform.localPosition.x - origPos.x) / maxScrollPos;
	}


	/// <summary>
	/// Sets the position of the scroll knob.
	/// Values should be from 0-1.
	/// </summary>
	/// <param name="pos">The new position of the scroll knob.</param>
	public void SetPosition(float pos)
	{
		transform.localPosition = origPos + (Vector3.right * maxScrollPos * pos);
	}


	// Sets the slider this knob will control.
	public void SetSlider(UISlider s)
	{
		slider = s;
	}

	/// <summary>
	/// Returns a reference to the associated UISlider control.
	/// </summary>
	/// <param name="s">The UISlider control of which this knob is part.</param>
	public UISlider GetSlider()
	{
		return slider;
	}

	// Sets the maximum distance the knob can move
	// from its starting position, given its
	// orientation. Ex: The starting position for
	// a knob on a slider is all the way to the left.
	public void SetMaxScroll(float max)
	{
		maxScrollPos = max;
	}

	// Called once all the relevant appearance elements
	// (such as the CSpriteFrame members) are populated 
	// with valid values so they can be processed.
	public void SetupAppearance()
	{
		Start();
		InitUVs();
		UpdateUVs();
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIScrollKnob Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIScrollKnob)go.AddComponent(typeof(UIScrollKnob));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIScrollKnob Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIScrollKnob)go.AddComponent(typeof(UIScrollKnob));
	}
}
