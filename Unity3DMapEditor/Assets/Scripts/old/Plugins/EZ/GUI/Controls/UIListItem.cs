//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;

/// <remarks>
/// Serves as an item in a list, such as a
/// scroll list.  Holds basic information
/// to implement such functionality and
/// can be extended by derivation.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/List Item")]
public class UIListItem : UIButton, IUIListObject
{
	[HideInInspector]
	public bool activeOnlyWhenSelected = true;

	// Index of the item in the list
	protected int m_index;

	// Whether this item is the selected one
	protected bool m_selected;

	/// <summary>
	/// Indicates whether this item is currently
	/// selected.
	/// </summary>
	public bool selected
	{
		get { return m_selected; }
		set 
		{
			m_selected = value;
			if (m_selected)
				SetControlState(CONTROL_STATE.ACTIVE);
			else
				SetControlState(CONTROL_STATE.NORMAL);
		}
	}

	// The list containing this list item.
	protected UIScrollList list;

	// Top-left and bottom-right 
	// edges of the collider:
	protected Vector2 colliderTL;
	protected Vector2 colliderBR;
	// The center of the custom collider
	protected Vector3 colliderCenter;



	protected override void Awake()
	{
		base.Awake();

		if (customCollider)
		{
			if (collider is BoxCollider)
			{
				BoxCollider c = (BoxCollider)collider;
				colliderTL.x = c.center.x - c.size.x * 0.5f;
				colliderTL.y = c.center.y + c.size.y * 0.5f;
				colliderBR.x = c.center.x + c.size.x * 0.5f;
				colliderBR.y = c.center.y - c.size.y * 0.5f;
				colliderCenter = c.center;
			}
		}

		//includeTextInAutoCollider = true;
	}


	// Necessary input processing that is required
	// if the control is disabled:
	protected void DoNeccessaryInput(ref POINTER_INFO ptr)
	{
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				if (list != null && ptr.active)
					list.ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (list != null && !ptr.isTap)
					list.ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.TAP:
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				if (list != null)
					list.PointerReleased();
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
	}


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
				if (list != null && ptr.active)	// If this is a hold
					list.ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (!selected)
				{
					if (soundOnOver != null && m_ctrlState != CONTROL_STATE.OVER)
						soundOnOver.PlayOneShot(soundOnOver.clip);

					SetControlState(CONTROL_STATE.OVER);
				}
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (!ptr.isTap)
				{
					if(!selected)
						SetControlState(CONTROL_STATE.NORMAL);
					if (list != null)
						list.ListDragged(ptr);
				}
				else if(!activeOnlyWhenSelected)
					SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if(!activeOnlyWhenSelected)
					SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.TAP:
				if (list != null)
				{
					// Tell our list we were selected:
					list.DidSelect(this);
					list.PointerReleased();
				}
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				if (!selected)
					SetControlState(CONTROL_STATE.NORMAL);
				if (list != null)
					list.PointerReleased();
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
				if(!selected)
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

	protected override void OnEnable()
	{
		base.OnEnable();

		if (Application.isPlaying && m_started)
		{
			// Set it to some bogus value so we can force
			// it to be reset:
			m_ctrlState = (CONTROL_STATE)(-1);

			if (controlIsEnabled)
			{
				if (selected)
					SetControlState(CONTROL_STATE.ACTIVE);
				else
					SetControlState(CONTROL_STATE.NORMAL, true);
			}
			else
				SetControlState(CONTROL_STATE.DISABLED, true);
		}
	}

	protected override void OnDisable()
	{
		// First, save our state as it will get changed
		// in UIButton's implementation:
		CONTROL_STATE oldState = controlState;

		base.OnDisable();

		// Now restore our previous state:
		SetControlState(oldState);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIListItem))
			return;

		UIListItem b = (UIListItem)s;


		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			list = b.list;
		}

		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			topLeftEdge = b.topLeftEdge;
			bottomRightEdge = b.bottomRightEdge;
			colliderTL = b.colliderTL;
			colliderBR = b.colliderBR;
			colliderCenter = b.colliderCenter;
			customCollider = b.customCollider;
		}
	}


	public override string Text
	{
		set
		{
			base.Text = value;
			FindOuterEdges();
			
			// Inform the list we may have been resized,
			// so it needs to reposition items:
			if (spriteText != null)
				if (spriteText.maxWidth > 0 && list != null)
					list.PositionItems();
		}
	}


	// Finds the outermost edges of the control,
	// taking all layers into account.
	public override void FindOuterEdges()
	{
		base.FindOuterEdges();

		if(!customCollider)
		{
			colliderTL = topLeftEdge;
			colliderBR = bottomRightEdge;
		}
	}

	public override void TruncateRight(float pct)
	{
		base.TruncateRight(pct);

		// Resize our collider:
		if(collider != null)
		{
			if(collider is BoxCollider)
			{
				if (customCollider)
				{
					BoxCollider c = (BoxCollider)collider;
					Vector3 tmp;

					tmp = c.center;
					tmp.x = (1f - pct) * (colliderBR.x - colliderTL.x) * -0.5f;
					c.center = tmp;

					tmp = c.size;
					tmp.x = pct * (colliderBR.x - colliderTL.x);
					c.size = tmp;
				}
				else
					UpdateCollider();
			}
		}
	}

	public override void TruncateLeft(float pct)
	{
		base.TruncateLeft(pct);

		// Resize our collider:
		if (collider != null)
		{
			if (collider is BoxCollider)
			{
				if (customCollider)
				{
					BoxCollider c = (BoxCollider)collider;
					Vector3 tmp;

					tmp = c.center;
					tmp.x = (1f - pct) * (colliderBR.x - colliderTL.x) * 0.5f;
					c.center = tmp;

					tmp = c.size;
					tmp.x = pct * (colliderBR.x - colliderTL.x);
					c.size = tmp;
				}
				else
					UpdateCollider();
			}
		}
	}

	public override void TruncateTop(float pct)
	{
		base.TruncateTop(pct);

		// Resize our collider:
		if (collider != null)
		{
			if (collider is BoxCollider)
			{
				if (customCollider)
				{
					BoxCollider c = (BoxCollider)collider;
					Vector3 tmp;

					tmp = c.center;
					tmp.y = (1f - pct) * (colliderBR.y - colliderTL.y) * 0.5f;
					c.center = tmp;

					tmp = c.size;
					tmp.y = pct * (colliderTL.y - colliderBR.y);
					c.size = tmp;
				}
				else
					UpdateCollider();
			}
		}
	}

	public override void TruncateBottom(float pct)
	{
		base.TruncateBottom(pct);

		// Resize our collider:
		if (collider != null)
		{
			if (collider is BoxCollider)
			{
				if (customCollider)
				{
					BoxCollider c = (BoxCollider)collider;
					Vector3 tmp;

					tmp = c.center;
					tmp.y = (1f - pct) * (colliderBR.y - colliderTL.y) * -0.5f;
					c.center = tmp;

					tmp = c.size;
					tmp.y = pct * (colliderTL.y - colliderBR.y);
					c.size = tmp;
				}
				else
					UpdateCollider();
			}
		}
	}

	public override void Untruncate()
	{
		base.Untruncate();

		// Resize our collider:
		if (collider != null)
		{
			if (collider is BoxCollider)
			{
				if (customCollider)
				{
					BoxCollider c = (BoxCollider)collider;

					if (!customCollider)
						c.center = Vector3.zero;
					else
						c.center = colliderCenter;

					c.size = new Vector3(colliderBR.x - colliderTL.x, colliderTL.y - colliderBR.y, 0.001f);
				}
				else
					UpdateCollider();
			}
		}
	}


	public override void Hide(bool tf)
	{
		base.Hide(tf);

		// Hide layers:
		for (int i = 0; i < layers.Length; ++i)
			layers[i].Hide(tf);

		// Hide any text:
		if (spriteText != null)
		{
			if (tf)
				spriteText.gameObject.active = false;
			else
				spriteText.gameObject.active = true;
		}
	}


	// Sets the scroll list that contains this list item.
	public void SetList(UIScrollList c)
	{
		list = c;
	}

	/// <summary>
	/// Returns the scroll list with which this item
	/// is associated.
	/// </summary>
	/// <returns>The scroll list.</returns>
	public virtual UIScrollList GetScrollList()
	{
		return list;
	}

	/// <summary>
	/// The index of the item in the list.
	/// </summary>
	public int Index
	{
		get { return m_index; }
		set { m_index = value; }
	}


	// Called automatically by the list object to ensure
	// a new UIListItem instance establishes a reference
	// to its text object before it tries to set the
	// text.
	public void FindText()
	{
		// If we didn't have a text mesh assigned,
		// look for one in a child:
		if (spriteText == null)
			spriteText = (SpriteText)GetComponentInChildren(typeof(SpriteText));

		if (spriteText != null)
		{
			spriteText.gameObject.layer = gameObject.layer;
			spriteText.Parent = this;
		}
	}

	public SpriteText TextObj
	{
		get { return spriteText; }
	}

	public bool IsContainer()
	{
		return false;
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIListItem Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIListItem)go.AddComponent(typeof(UIListItem));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIListItem Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIListItem)go.AddComponent(typeof(UIListItem));
	}
}
