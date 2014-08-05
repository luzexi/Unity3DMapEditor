//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define DONT_CENTER_OBJECT

using UnityEngine;
using System.Collections;



#region Drag&Drop_Types

/// <remarks>
/// Identifies the type of drag & drop event that is taking place.
/// </remarks>
public enum EZDragDropEvent
{
	/// <summary>
	/// Sent to an object when its drag operation has just begun.
	/// </summary>
	Begin,

	/// <summary>
	/// This event occurs every frame that a
	/// drag operation continues in which
	/// one of the other event types does not
	/// occur.
	/// </summary>
	Update,

	/// <summary>
	/// Indicates the object being dragged
	/// has entered the area of a potential
	/// drop target.
	/// </summary>
	DragEnter,

	/// <summary>
	/// Indicates the object being dragged
	/// has exited the area of a potential
	/// drop target.
	/// </summary>
	DragExit,

	/// <summary>
	/// Indicates the drag operation was
	/// cancelled (usually the object was
	/// dropped outside a valid drop target).
	/// NOTE: The information contained in the
	/// ptr member of the EZDragDropParams
	/// parameter will not contain valid pointer
	/// information since this event occurs
	/// when the pointer is no longer interacting
	/// with the object.
	/// </summary>
	Cancelled,

	/// <summary>
	/// Indicates that the drag cancel is done.
	/// This is sent when the default cancel
	/// animation which returns the object back 
	/// to its starting position is finished.
	/// NOTE: The information contained in the
	/// ptr member of the EZDragDropParams
	/// parameter will not contain valid pointer
	/// information since this event occurs
	/// when the pointer is no longer interacting
	/// with the object.
	/// </summary>
	CancelDone,

	/// <summary>
	/// The object being dragged was dropped.
	/// It may or may not have had a drop target.
	/// </summary>
	Dropped
}

/// <remarks>
/// A structure which holds information about a
/// drag and drop operation's status.
/// </remarks>
public struct EZDragDropParams
{
	/// <summary>
	/// The drag/drop event that has occurred.
	/// </summary>
	public EZDragDropEvent evt;

	/// <summary>
	/// The object being dragged.
	/// </summary>
	public IUIObject dragObj;

	/// <summary>
	/// The pointer involved in the drag/drop operation.
	/// </summary>
	public POINTER_INFO ptr;

	/// <summary>
	/// Constructs an EZDragDropParams object.
	/// </summary>
	/// <param name="e">The drag/drop event that occurred.</param>
	/// <param name="obj">The object being dragged.</param>
	/// <param name="p">The pointer involved in the drag/drop operation.</param>
	public EZDragDropParams(EZDragDropEvent e, IUIObject obj, POINTER_INFO p)
	{
		evt = e;
		dragObj = obj;
		ptr = p;
	}
}

/// <remarks>
/// Definition of a delegate that receives regular 
/// notification of drag & drop events pertaining 
/// to this object when an object is being dragged.  
/// This is called on potential drop targets when 
/// an object is dragged over them.  It is also
/// called on the object(s) being dragged/dropped.
/// </remarks>
/// <param name="parms">The EZDragDropParams structure which holds information about the event.</param>
public delegate void EZDragDropDelegate(EZDragDropParams parms);

#endregion



/// <remarks>
/// Interface for any object that implements EZ GUI drag-and-drop functionality
/// </remarks>
public interface IEZDragDrop
{
	/// <summary>
	/// Holds "boxed" data for the control.
	/// This can be used to associate any
	/// object or value with the control
	/// for later reference and use.
	/// </summary>
	object Data
	{
		get;
		set;
	}

	/// <summary>
	/// Indicates whether the object can be dragged as part of a drag & drop operation.
	/// </summary>
	bool IsDraggable
	{
		get;
		set;
	}

	/// <summary>
	/// A mask which can be used to make the object only be "droppable" on
	/// objects in a certain layer.
	/// NOTE: This mask is combined with the camera's mask.
	/// </summary>
	LayerMask DropMask
	{
		get;
		set;
	}

	/// <summary>
	/// Indicates whether the object is being dragged as part of a drag & drop operation.
	/// Setting this value to false while the object is being dragged will cause the drag
	/// and drop operation to be cancelled.
	/// </summary>
	bool IsDragging
	{
		get;
		set;
	}

	/// <summary>
	/// The GameObject over which the object being dragged
	/// is hovering and will attempt to be dropped if it 
	/// let go.
	/// </summary>
	GameObject DropTarget
	{
		get;
		set;
	}

	/// <summary>
	/// In the context of a drag & drop operation, this
	/// indicates whether the drop action was handled.
	/// If this is not set to true in response to a
	/// Dropped message sent to OnEZDragDrop(), the
	/// drop will be considered to be unhandled and will
	/// result in a cancelled drop, causing the dragged
	/// object to return to its original position.
	/// </summary>
	bool DropHandled
	{
		get;
		set;
	}

	/*	// Gets the plane in which the control was when it
		// was first dragged (before being offset)
		Plane DragPlane
		{
			get;
		}
	*/
	/// <summary>
	/// The distance an object being dragged and dropped
	/// should be offset toward the camera to ensure it
	/// hovers above other objects and controls in the
	/// scene.
	/// </summary>
	float DragOffset
	{
		get;
		set;
	}

	/// <summary>
	/// The type of easing to use to animate the object back to its starting
	/// position when a drag operation is cancelled.
	/// </summary>
	EZAnimation.EASING_TYPE CancelDragEasing
	{
		get;
		set;
	}

	/// <summary>
	/// The duration of the easing animation when a drag and drop operation
	/// is cancelled.
	/// </summary>
	float CancelDragDuration
	{
		get;
		set;
	}

	// Updates the position of an object being dragged and dropped
	// according to the current pointer position.
	void DragUpdatePosition(POINTER_INFO ptr);

	/// <summary>
	/// Cancels any pending drag and drop operation.
	/// </summary>
	void CancelDrag();

	// <summary>
	// Receives regular notification of drag & drop events
	// pertaining to this object when an object is being
	// dragged.  This is called on potential drop targets
	// when an object is dragged over them.  It is also
	// called on the object(s) being dragged/dropped.
	// </summary>
	// <param name="parms">The EZDragDropParams structure which holds information about the event.</param>
	void OnEZDragDrop_Internal(EZDragDropParams parms);

	/// <summary>
	/// Adds a delegate to be called with drag and drop notifications.
	/// </summary>
	/// <param name="del">The delegate to add.</param>
	void AddDragDropDelegate(EZDragDropDelegate del);

	/// <summary>
	/// Removes a delegate from the list of those to be called 
	/// with drag and drop notifications.
	/// </summary>
	/// <param name="del">The delegate to add.</param>
	void RemoveDragDropDelegate(EZDragDropDelegate del);

	/// <summary>
	/// Sets the delegate to be called with drag and drop notifications.
	/// NOTE: This will replace any previously registered delegates.
	/// </summary>
	/// <param name="del">The delegate to add.</param>
	void SetDragDropDelegate(EZDragDropDelegate del);

	// Setters for the internal drag drop handler delegate:
	void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del);
	void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del);
	void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del);
	EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate();
}


/// <remarks>
/// Helper class that encapsulates all the major drag-and-drop
/// handling logic.
/// </remarks>
public class EZDragDropHelper
{
	/// <summary>
	/// The delegate pattern for a delegate that updates the dragged object's
	/// position.
	/// </summary>
	/// <param name="ptr">The pointer info struct for the pointer dragging the object.</param>
	public delegate void UpdateDragPositionDelegate(POINTER_INFO ptr);

	// Used internally to allow processing of input before the internal
	// drag-drop handler runs.  This is useful for virtual screens.
	public delegate void DragDrop_InternalDelegate(ref POINTER_INFO ptr);



	// The host object
	public IUIObject host;


	public EZDragDropHelper(IUIObject h)
	{
		host = h;
		dragPosUpdateDel = DefaultDragUpdatePosition;
	}

	public EZDragDropHelper() 
	{
		dragPosUpdateDel = DefaultDragUpdatePosition;
	}

	// Delegate to call to update the object while dragging
	protected UpdateDragPositionDelegate dragPosUpdateDel;

	protected DragDrop_InternalDelegate dragDrop_InternalDel;

	// Holds an offset value that lets our dragged
	// object not "jump" suddenly to be centered
	// at the point being touched.
	Vector3 touchCompensationOffset = Vector3.zero;

	// Will hold the starting position of our control when
	// it began to be dragged.
	protected Vector3 dragOrigin;

	// Version of dragOrigin that is offset by dragOffset.
	protected Vector3 dragOriginOffset;

	// Defines the plane in which the control sat when it
	// was first dragged (prior to being offset).
	protected Plane dragPlane;

	Plane DragPlane
	{
		get { return dragPlane; }
	}

	protected bool isDragging = false;

	// Indicates whether the object is in the midst of canceling its drag
	protected bool isCanceling = false;

	// Indicates whether to use the default drag canceling animation
	protected bool useDefaultCancelDragAnim = true;
	
	/// <summary>
	/// Indicates whether to use the default drag canceling animation.
	/// </summary>
	public bool UseDefaultCancelDragAnim
	{
		get { return useDefaultCancelDragAnim; }
		set
		{
			useDefaultCancelDragAnim = value;
		}
	}

	// Setters for the internal drag drop handler delegate:
	public void SetDragDropInternalDelegate(DragDrop_InternalDelegate del)
	{
		dragDrop_InternalDel = del;
	}
	public void AddDragDropInternalDelegate(DragDrop_InternalDelegate del)
	{
		dragDrop_InternalDel += del;
	}
	public void RemoveDragDropInternalDelegate(DragDrop_InternalDelegate del)
	{
		dragDrop_InternalDel -= del;
	}
	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate()
	{
		return dragDrop_InternalDel;
	}


	/// <summary>
	/// Indicates whether the object is being dragged as part of a drag & drop operation.
	/// Setting this value to false while the object is being dragged will cause the drag
	/// and drop operation to be cancelled.
	/// </summary>
	public bool IsDragging
	{
		get { return isDragging; }
		set
		{
			bool wasDragging = isDragging;

			if (wasDragging && !value)
				CancelDrag();

			isDragging = value;
		}
	}

	/// <summary>
	/// Indicates whether the object is in the midst of canceling
	/// a drag operation and transitioning back to its point of
	/// origin.
	/// </summary>
	public bool IsCanceling
	{
		get { return isCanceling; }
	}

	/// <summary>
	/// Signals to the object that its cancel drag transition
	/// has completed.  Only call this yourself if you have
	/// overridden the default drag canceling and you are
	/// finished animating/whatever the object as a result of
	/// having its drag cancelled.
	/// </summary>
	public void CancelFinished()
	{
		isCanceling = false;
	}

	// The potential object onto which this object may be dropped.
	protected GameObject dropTarget;

	/// <summary>
	/// The GameObject over which the object being dragged
	/// is hovering and will attempt to be dropped if it 
	/// let go.
	/// </summary>
	public GameObject DropTarget
	{
		get { return dropTarget; }
		set
		{
			// Just to be safe:
			if (value == host.gameObject)
				return; // Don't set ourselves as our own drop target!

			if (dropTarget != value)
			{
				// Notify the target objects of the change:
				if (dropTarget != null)
				{
					// Send the message to everyone:
					OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.DragExit, host, default(POINTER_INFO)));
				}

				dropTarget = value;

				if (value != null)
				{
					// Send the message to everyone:
					OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.DragEnter, host, default(POINTER_INFO)));
				}
			}
		}
	}

	// Indicates whether the drop operation was handled.
	protected bool dropHandled = false;

	/// <summary>
	/// In the context of a drag & drop operation, this
	/// indicates whether the drop action was handled.
	/// If this is not set to true in response to a
	/// Dropped message sent to OnEZDragDrop(), the
	/// drop will be considered to be unhandled and will
	/// result in a cancelled drop, causing the dragged
	/// object to return to its original position.
	/// </summary>
	public bool DropHandled
	{
		get { return dropHandled; }
		set { dropHandled = value; }
	}

	/// <summary>
	/// Sets the delegate to be called in order to update the drag
	/// position of the object being dragged.
	/// </summary>
	/// <param name="del">The delegate that will update the object's position.</param>
	public void SetDragPosUpdater(UpdateDragPositionDelegate del)
	{
		dragPosUpdateDel = del;
	}

	// Updates the position of an object being dragged and dropped
	// according to the current pointer position.
	public void DragUpdatePosition(POINTER_INFO ptr)
	{
		if (dragPosUpdateDel != null)
			dragPosUpdateDel(ptr);
	}

	/// <summary>
	/// The default method of updating the drag position
	/// </summary>
	/// <param name="ptr">The pointer info struct for the pointer dragging the object.</param>
	public void DefaultDragUpdatePosition(POINTER_INFO ptr)
	{
		float dist;

		dragPlane.Raycast(ptr.ray, out dist);
		host.transform.position = touchCompensationOffset + ptr.ray.origin + ptr.ray.direction * (dist - host.DragOffset);
	}

	/// <summary>
	/// Cancels any pending drag and drop operation.
	/// </summary>
	public void CancelDrag()
	{
		if (!isDragging)
			return;

		// Notify everyone involved:
		EZDragDropParams parms = new EZDragDropParams(EZDragDropEvent.Cancelled, host, default(POINTER_INFO));
		OnEZDragDrop_Internal(parms);

		dropTarget = null;
		dropHandled = false;
		isDragging = false;
		isCanceling = true;

		if(useDefaultCancelDragAnim)
			DoDefaultCancelDrag();

		// Since we are detargeting, send a release off event:
		POINTER_INFO ptr = default(POINTER_INFO);
		ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
		host.OnInput(ptr);

		if (UIManager.Exists())
 			UIManager.instance.Detarget(host);
	}

	// Performs the default cancel drag animation.
	public void DoDefaultCancelDrag()
	{
		AnimatePosition.Do(host.gameObject, EZAnimation.ANIM_MODE.To, dragOriginOffset, EZAnimation.GetInterpolator(host.CancelDragEasing), host.CancelDragDuration, 0, null, FinishCancelDrag);
	}

	// Finishes off the CancelDrag animation.
	protected void FinishCancelDrag(EZAnimation anim)
	{
		// See if the object has been destroyed:
		if (host == null)
			return;

		host.transform.localPosition = dragOrigin;
		isCanceling = false;
		OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.CancelDone, host, default(POINTER_INFO)));
	}

	/// <summary>
	/// Receives regular notification of drag & drop events
	/// pertaining to this object when an object is being
	/// dragged.  This is called on potential drop targets
	/// when an object is dragged over them.  It is also
	/// called on the object(s) being dragged/dropped.
	/// </summary>
	/// <param name="parms">The EZDragDropParams structure which holds information about the event.</param>
	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		//if (dragDrop_InternalDel != null)
		//	dragDrop_InternalDel(ref parms.ptr);

		switch (parms.evt)
		{
			case EZDragDropEvent.Begin:
				// First check to see if we are in mid-cancel:
				if (isCanceling)
					return;

				// Set initial flags:
				isDragging = true;
				dropHandled = false;

				// Save our initial local position:
				Transform transform = host.transform;
				dragOrigin = transform.localPosition;

				// Setup our drag plane:
				Transform camTrans = parms.ptr.camera.transform;
				dragPlane.SetNormalAndPosition(camTrans.forward * -1f, transform.position);

				// Find our offset drag origin:
				float dist;
				Ray ray = parms.ptr.camera.ScreenPointToRay(parms.ptr.camera.WorldToScreenPoint(transform.position));
				dragPlane.Raycast(ray, out dist);
				dragOriginOffset = ray.origin + ray.direction * (dist - host.DragOffset);
				if (transform.parent != null)
					dragOriginOffset = transform.parent.InverseTransformPoint(dragOriginOffset);

#if DONT_CENTER_OBJECT
				// Find the offset of the actual touch position
				// from our object center so that our object
				// doesn't just "jump" to be centered at the
				// touch position:
				dragPlane.Raycast(parms.ptr.ray, out dist);
				touchCompensationOffset = transform.position - (parms.ptr.ray.origin + parms.ptr.ray.direction * dist);
#endif
				break;
		}

		if (dragDropDelegate != null)
			dragDropDelegate(parms);

		if (dropTarget != null)
			dropTarget.SendMessage("OnEZDragDrop", parms, SendMessageOptions.DontRequireReceiver);

		// Notify our own GameObject:
		host.gameObject.SendMessage("OnEZDragDrop", parms, SendMessageOptions.DontRequireReceiver);

		// See if this was a good or bad drop of this object:
		if (parms.evt == EZDragDropEvent.Dropped && parms.dragObj.Equals(host))
		{
			if (dropHandled)
			{
				// A successful drop!
				isDragging = false;
				dropTarget = null;
			}
			else
			{
				// A failed drop:
				CancelDrag();
			}
		}
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
}