//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define AUTO_ORIENT_ITEMS
#define AUTO_SCALE_ITEMS



using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Interface for any list object that is to be
// part of a UIScrollList list.
public interface IUIListObject : IUIObject
{
	/// <summary>
	/// Returns whether this object serves as a
	/// container for other controls.
	/// </summary>
	/// <returns>True if a container, false otherwise.</returns>
	bool IsContainer();

/*	/// <summary>
	/// Gets/sets whether this object is a clone
	/// (has been cloned from a prefab or other
	/// scene object).
	/// </summary>
	bool IsClone
	{
		get;
		set;
	}
*/
	//  Finds the outer edges of the object.
	void FindOuterEdges();

	/// <summary>
	/// The position of the top-left edge of the object.
	/// </summary>
	Vector2 TopLeftEdge { get; }

	/// <summary>
	/// The position of the bottom-right edge of the object.
	/// </summary>
	Vector2 BottomRightEdge { get; }

	/// <summary>
	/// Hides the object.
	/// </summary>
	/// <param name="tf">Whether to hide the object or not.</param>
	void Hide(bool tf);

	bool Managed
	{
		get;
	}

	/// <summary>
	/// Sets the Rect (specified in world space)
	/// against which this object will be clipped.
	/// </summary>
	Rect3D ClippingRect
	{
		get;
		set;
	}

	/// <summary>
	/// Accessor for whether or not the object is
	/// to be clipped against a clipping rect.
	/// </summary>
	bool Clipped
	{
		get;
		set;
	}

	/// <summary>
	/// Un-clips the object (undoes any clipping
	/// specified by ClippingRect).
	/// </summary>
	void Unclip();

	/// <summary>
	/// Updates the object's collider based upon the
	/// extents of its content.
	/// </summary>
	void UpdateCollider();

	// Sets the scroll list that contains this object.
	void SetList(UIScrollList c);

	/// <summary>
	/// Returns the scroll list with which this item
	/// is associated.
	/// </summary>
	/// <returns>The scroll list.</returns>
	UIScrollList GetScrollList();

	/// <summary>
	/// The index of the object in the list.
	/// </summary>
	int Index
	{
		get;
		set;
	}

	/// <summary>
	/// The text to be displayed by the object
	/// </summary>
	string Text
	{
		get;
		set;
	}

	/// <summary>
	/// Gets the reference to the SpriteText object
	/// attached to this object, if any. (Null if
	/// there is none.)
	/// </summary>
	SpriteText TextObj
	{
		get;
	}

	/// <summary>
	/// Sets/Gets whether this object is the currently
	/// selected item in the list.
	/// </summary>
	bool selected
	{
		get;
		set;
	}

	/// <summary>
	/// Provided for compatibility with destroying
	/// sprite-based controls.
	/// </summary>
	void Delete();

	/// <summary>
	/// The camera that is to render this object.
	/// </summary>
	Camera RenderCamera
	{
		get;
		set;
	}

	/// <summary>
	/// Updates any camera-dependent settings.
	/// </summary>
	void UpdateCamera();
}


[System.Serializable]
public class PrefabListItem
{
	public GameObject item;
	public string itemText;
}



/// <remarks>
/// Acts as a container of list items (basically buttons)
/// and manages them in such a manner as to allow the
/// user to scroll through the list.
/// NOTE: For proper appearance, list items should each 
/// be of uniform width when the list is vertical, and of 
/// uniform height when the list is horizontal. Besides
/// that, list items can vary in size.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Scroll List")]
public class UIScrollList : MonoBehaviour, IUIObject
{
	/// <remarks>
	/// The possible orientations of a scroll list.
	/// * HORIZONTAL will cause the list items to be
	/// side-by-side, and the list will scroll right 
	/// and left.
	/// * VERTICAL will cause the list items to be
	/// displayed vertically and the list will scroll
	/// up and down.
	/// </remarks>
	public enum ORIENTATION
	{
		HORIZONTAL,
		VERTICAL
	};

	/// <remarks>
	/// Determines whether items will be added to the
	/// list from top-to-bottom and left-to-right, or
	/// from bottom-to-top and right-to-left.
	/// </remarks>
	public enum DIRECTION
	{
		/// <summary>
		/// Items will be added top-to-bottom, or
		/// left-to-right, depending on orientation.
		/// </summary>
		TtoB_LtoR,

		/// <summary>
		/// Items will be added bottom-to-top, or
		/// left-to-right, depending on orientation.
		/// </summary>
		BtoT_RtoL
	}

	/// <summary>
	/// Determines how items will be aligned within
	/// the list when added.
	/// </summary>
	public enum ALIGNMENT
	{
		/// <summary>
		/// Items will be aligned to the left edge
		/// of the viewable area (if vertical), or
		/// the top edge (if horizontal).
		/// </summary>
		LEFT_TOP,

		/// <summary>
		/// Items will be centered within the
		/// viewable area.
		/// </summary>
		CENTER,

		/// <summary>
		/// Items will be aligned to the right edge
		/// of the viewable area (if vertical), or
		/// the bottom edge (if horizontal).
		/// </summary>
		RIGHT_BOTTOM
	}

	// Used to allow cleaner code separation for the code that
	// aligns list items this or that way.
	protected delegate float ItemAlignmentDel(IUIListObject item);

	// Used to allow cleaner code for processing coordinate
	// values in GetSnapPos().
	protected delegate bool SnapCoordProc(float val);

	/// <summary>
	/// Delegate definition for a delegate that receives a notification
	/// when the list begins to snap to an item.
	/// </summary>
	/// <param name="item">The item to which the list is snapping</param>
	public delegate void ItemSnappedDelegate(IUIListObject item);

	/// <summary>
	/// When true, scrolling will operate like a
	/// scrollable list on an iPhone - when a
	/// pointing device (normally a finger) is
	/// dragged across it, it scrolls and coasts
	/// to an eventual stop if not stopped 
	/// manually.
	/// </summary>
	public bool touchScroll = true;

	/// <summary>
	/// The factor by which any mouse scrollwheel delta is
	/// multiplied to arrive at how far the list should be
	/// scrolled in response.  To make your list scroll
	/// faster or slower in response to mousewheel input,
	/// increase or decrease this value, respectively.
	/// </summary>
	public float scrollWheelFactor = 100f;

	/// <summary>
	/// The scroll deceleration coefficient. This value
	/// determines how quickly a touch scroll will coast
	/// to a stop.  When using snapping, this also essentially
	/// determines which item will be snapped to when the list
	/// is let go.  If you want to always scroll one item at a
	/// time, for example, set this value high.
	/// </summary>
	public float scrollDecelCoef = 0.04f;

	/// <summary>
	/// When true, the scroll position will "snap" so that
	/// the item nearest the center of the list will snap
	/// to the center, provided it wouldn't take us past
	/// the edge of the list.
	/// </summary>
	public bool snap = false;

	/// <summary>
	/// The minimum number of seconds a snap should last.
	/// </summary>
	public float minSnapDuration = 1f;

	/// <summary>
	/// The type of easing to use when snapping to an item.
	/// </summary>
	public EZAnimation.EASING_TYPE snapEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	/// <summary>
	/// An optional slider control that will control
	/// the scroll position of the list.
	/// </summary>
	public UISlider slider;

	/// <summary>
	/// The orientation of the list - horizontal or
	/// vertical.
	/// </summary>
	public ORIENTATION orientation;

	/// <summary>
	/// Determines whether items will be added to the
	/// list from top-to-bottom and left-to-right, or
	/// from bottom-to-top and right-to-left.
	/// See UIScrollList.DIRECTION [<see cref="DIRECTION"/>]
	/// </summary>
	public DIRECTION direction = DIRECTION.TtoB_LtoR;

	/// <summary>
	/// Determines how the items will be aligned in the
	/// list when added.  See UIScrollList.ALIGNMENT [<see cref="ALIGNMENT"/>]
	/// </summary>
	public ALIGNMENT alignment = ALIGNMENT.CENTER;

	/// <summary>
	/// The extents of the viewable area, in local
	/// space units.  The contents of the list will
	/// be clipped to this area. Ex: If an area of
	/// 10x10 is specified, the list contents will
	/// be clipped 5 units above the center of the
	/// scroll list's GameObject, 5 units below,
	/// and 5 units on either side.
	/// </summary>
	public Vector2 viewableArea;

	// The one that is actually used for calculations
	// (always in world/local units)
	protected Vector2 viewableAreaActual;

	/// <summary>
	/// When true, the viewableArea itemSpacing, etc, are interpreted as
	/// being specified in screen pixels.
	/// </summary>
	public bool unitsInPixels = false;

	/// <summary>
	/// The camera that will render this list.
	/// This needs to be set when unitsInPixels
	/// is set to true or when you have prefab
	/// items that need to know which camera to
	/// use to size themselves.
	/// </summary>
	public Camera renderCamera;
	
	// Used to clip items in the list:
	protected Rect3D clientClippingRect;

	/// <summary>
	/// Empty space to put between each list item.
	/// </summary>
	public float itemSpacing;
	
	// The one that is actually used for calculations
	// (always in world/local units)
	protected float itemSpacingActual;

	/// <summary>
	/// When set to true, the itemSpacing value will be
	/// applied before the first item, and after the last
	/// so as to "pad" the first and last items a bit from
	/// the edge.  If false, the first and last items will
	/// be flush with the edges of the viewable area.
	/// </summary>
	public bool spacingAtEnds = true;

	/// <summary>
	/// Adds the specified amount of extra space to the ends of the list
	/// (before the first item, and after the last).  This value is in
	/// local units if "unitsInPixels" is false, or in pixels if it is
	/// true.
	/// </summary>
	public float extraEndSpacing = 0;

	// The one that is actually used for calculations
	// (always in world/local units)
	protected float extraEndSpacingActual;

	/// <summary>
	/// When true, items added or inserted to this list will
	/// be activated recursively.  This is STRONGLY recommended
	/// when using managed sprites/controls.
	/// </summary>
	public bool activateWhenAdding = true;

	/// <summary>
	/// When true, the contents of the list will be clipped
	/// to the viewable area.  Otherwise, the content will
	/// extend beyond the specified viewable area.
	/// </summary>
	public bool clipContents = true;

	/// <summary>
	/// When true, items will be clipped each frame in which
	/// it is detected that the list has moved, rotated, or
	/// scaled.  Use this if you are having clipping errors
	/// after scaling, rotating, etc, a list as part of a
	/// panel transition, etc.  Leave set to false otherwise
	/// to improve performance.
	/// </summary>
	public bool clipWhenMoving = false;

	/// <summary>
	/// When set to true, items added to the list will be
	/// positioned in the list immediately so that any
	/// immediately following code that does something to
	/// the list that depends on the new item positioning
	/// such as ScrollToItem(), etc, will take the new
	/// list contents into account.  Otherwise, if left
	/// set to false, items are positioned in the list
	/// at the end of the frame in which they were added
	/// (inside LateUpdate()).  If you are adding lots
	/// of items to a list in batches, you can save a lot
	/// of performance by setting this setting to false so
	/// that the items all get positioned in one batch.
	/// But if you are having problems with ScrollToItem()
	/// or other similar features not working when called
	/// immediately after adding items to the list, set
	/// this setting to true.
	/// </summary>
	public bool positionItemsImmediately = true;

	/// <summary>
	/// Distance the pointer must travel beyond
	/// which when it is released, the item
	/// under the pointer will not be considered
	/// to have been selected.  This allows
	/// touch scrolling without inadvertently
	/// selecting the item under the pointer.
	/// </summary>
	public float dragThreshold = float.NaN;

	/// <summary>
	/// Optional array of items in the scene that 
	/// are to be added to the list on startup.
	/// These will be added before the items in
	/// the prefabItems array.
	/// </summary>
	public GameObject[] sceneItems = new GameObject[0];

	/// <summary>
	/// Optional array of item prefabs that are
	/// to be added to the list on startup.
	/// These will be added after the items in
	/// the sceneItems array.
	/// </summary>
	public PrefabListItem[] prefabItems = new PrefabListItem[0];

	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when an item is selected.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;
	
	/// <summary>
	/// A string containing the name of the method to be invoked
	/// when an item is selected.
	/// </summary>
	public string methodToInvokeOnSelect;

	/// <summary>
	/// (Optional) Manager to which instantiated list items should be added
	/// once created. If none is set, sprites must either be unmanaged,
	/// or must already have been added to a manager.
	/// </summary>
	public SpriteManager manager;

	/// <summary>
	/// When set to true, the control will instruct any
	/// pointers which have it as their target to
	/// de-target them.  Use this if you are deactivating
	/// a control and want no input to go to it.
	/// It is strongly recommended NOT to use this feature
	/// on any control that appears in a scroll list, or
	/// else you may be unable to scroll past the edge of
	/// the list's viewable area.
	/// </summary>
	public bool detargetOnDisable = false;

	/// <summary>
	/// The easing to use to reposition items in the list 
	/// when items are inserted to, or removed from, the list.
	/// </summary>
	public EZAnimation.EASING_TYPE positionEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	/// <summary>
	/// The duration of the position easing, in seconds.
	/// </summary>
	public float positionEaseDuration = 0.5f;

	/// <summary>
	/// The delay of the position easing, in seconds.
	/// </summary>
	public float positionEaseDelay = 0;

	/// <summary>
	/// Indicates whether or not input should be blocked while items
	/// are easing into place. It is recommended to leave this at the
	/// default value of true.
	/// </summary>
	public bool blockInputWhileEasing = true;

	// Flag that indicates that position easing is to be done
	// when we reposition our items.
	protected bool doItemEasing = false;

	// Same as doItemEasing, except used to track whether the
	// scroll position itself can be eased.
	protected bool doPosEasing = false;

	// List of all active animators/easers for our items:
	protected List<EZAnimation> itemEasers = new List<EZAnimation>();

	// Gets assigned an EZAnimation if the scroll position has to be eased
	// back into a valid range because we are using positionEasing
	// and an item has been removed that places the visible area
	// outside the edge of the list.
	protected EZAnimation scrollPosAnim;

	/// <summary>
	/// When true, the content of the list will be re-laid out
	/// whenever the list object becomes active (enabled).
	/// </summary>
	[HideInInspector]
	[System.NonSerialized]
	public bool repositionOnEnable = true;

	// The extents of the content held in the list.
	// Similar to viewableArea, except this is the
	// partially-hidden area that is taken up by all
	// the items in the list.
	// The value indicates the extent in the direction
	// of the list's orientation.
	protected float contentExtents;

	// Reference to the currently selected item.
	protected IUIListObject selectedItem;

	// The last control to have been clicked
	// This is used for accessing nested children
	// inside a UIListItemContainer, but can be
	// used for other things as well.
	protected IUIObject lastClickedControl;

	// The scroll position of the list.
	// (0 == beginning, 1 == end)
	protected float scrollPos;

	// The GameObject that will ease the process
	// of moving our list items about.
	protected GameObject mover;

	// Items in our list.
	protected List<IUIListObject> items = new List<IUIListObject>();

	// Items which are presently visible.
	protected List<IUIListObject> visibleItems = new List<IUIListObject>();
	// Temporary version of the above:
	protected List<IUIListObject> tempVisItems = new List<IUIListObject>();

	protected bool m_controlIsEnabled = true;
	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;
	protected EZValueChangedDelegate changeDelegate;
	protected ItemSnappedDelegate itemSnappedDel;

	// Cached transform info of the list, so if it changes,
	// we can update our client clipping rect.
	protected Vector3 cachedPos;
	protected Quaternion cachedRot;
	protected Vector3 cachedScale;

	// Tells us if Start() has already run
	protected bool m_started = false;
	// Tells us if Awake() has already run
	protected bool m_awake = false;


	// Scrolling vars:
	protected List<IUIListObject> newItems = new List<IUIListObject>(); // The list of newly added items
	protected bool itemsInserted;				// Indicates if any of the items added during the current frame were inserted into the middle of the list.
	protected bool isScrolling;					// Is true when the list is scrolling.
	protected bool noTouch = true;				// Indicates whether there's currently an active touch/click/drag
	protected const float reboundSpeed = 1f;	// Rate at which the list will rebound from the edges.
	protected const float overscrollAllowance = 0.5f; // The percentage of the viewable area that can be "over-scrolled" (scrolled beyond the end/beginning of the list)
	protected const float lowPassKernelWidthInSeconds = 0.045f; // Inertial sampling width, in seconds
	//protected const float scrollDeltaUpdateInterval = 0.0166f;// The standard framerate update interval, in seconds
	protected float lowPassFilterFactor;// = scrollDeltaUpdateInterval / lowPassKernelWidthInSeconds;
	float scrollInertia;						// Holds onto some of the energy from previous scrolling action for a while
	protected const float backgroundColliderOffset = 0.01f; // How far behind the list items the background collider should be placed.
	protected float scrollMax;					// Absolute max we can scroll beyond the edge of the list.
	float scrollDelta;
	const float scrollStopThreshold = 0.0001f;
	float scrollStopThresholdLog = Mathf.Log10(scrollStopThreshold);
	float lastTime = 0;
	float timeDelta = 0;
	float inertiaLerpInterval = 0.06f;			// Lerp the scroll inertia every 60ms
	float inertiaLerpTime;						// Accumulates the amount of time passed since the last inertia lerp
	float amtOfPlay;							// The sum total of the extents of our list that are not covered by the viewable area.
	float autoScrollDuration;					// Holds the total number of seconds the current auto scroll operation should take
	float autoScrollStart;						// The starting point of the auto-scroll.
	float autoScrollPos;						// Where the auto-scroll operation is taking us.
	float autoScrollDelta;						// The change in scroll position from where we started when we began auto-scrolling.
	float autoScrollTime;						// The total number of seconds passed since we began auto-scrolling.
	bool autoScrolling = false;					// Flag to tell us if we're presently auto-scrolling.
	bool listMoved = false;						// Gets set to true whenever the list is scrolled by hand, so that we can track whether we need to snap or not.
	EZAnimation.Interpolator autoScrollInterpolator; // Easing delegate for snapping/auto-scrolling.
	IUIListObject snappedItem;					// The item to which the list has snapped.
	float localUnitsPerPixel;					// The number of local space units per pixel.



	protected void Awake()
	{
		if (m_awake)
			return;
		m_awake = true;

		// Create our mover object:
		mover = new GameObject();
		mover.name = "Mover";
		mover.transform.parent = transform;
		mover.transform.localPosition = Vector3.zero;
		mover.transform.localRotation = Quaternion.identity;
		mover.transform.localScale = Vector3.one;

		if (direction == DIRECTION.BtoT_RtoL)
			scrollPos = 1f; // We start at the bottom in this case

		autoScrollInterpolator = EZAnimation.GetInterpolator(snapEasing);

		lowPassFilterFactor = inertiaLerpInterval / lowPassKernelWidthInSeconds;
	}

	protected void Start()
	{
		if (m_started)
			return;
		m_started = true;

		// Convert our values to the proper units:
		SetupCameraAndSizes();

		lastTime = Time.realtimeSinceStartup;

		cachedPos = transform.position;
		cachedRot = transform.rotation;
		cachedScale = transform.lossyScale;
		CalcClippingRect();

		if (slider != null)
		{
			slider.AddValueChangedDelegate(SliderMoved);
			slider.AddInputDelegate(SliderInputDel);
		}

		// Create a background box collider to catch
		// input events between list items:
		if(collider == null && touchScroll)
		{
			BoxCollider bc = (BoxCollider) gameObject.AddComponent(typeof(BoxCollider));
			bc.size = new Vector3(viewableAreaActual.x, viewableAreaActual.y, 0.001f);
			bc.center = Vector3.forward * backgroundColliderOffset; // Set the collider behind where the list items will be.
			bc.isTrigger = true;
		}

		for(int i=0; i<sceneItems.Length; ++i)
			if(sceneItems[i] != null)
				AddItem(sceneItems[i]);

		for (int i = 0; i < prefabItems.Length; ++i)
			if(prefabItems[i] != null)
			{
				// If this one is null, use the first prefab:
				if (prefabItems[i].item == null)
				{
					if(prefabItems[0].item != null)
						CreateItem(prefabItems[0].item, (prefabItems[i].itemText == "") ? (null) : (prefabItems[i].itemText));
				}
				else
				{
					CreateItem(prefabItems[i].item, (prefabItems[i].itemText == "") ? (null) : (prefabItems[i].itemText));
				}
			}

		// Use the default threshold if ours
		// has not been set to anything:
		if (float.IsNaN(dragThreshold))
			dragThreshold = UIManager.instance.dragThreshold;
	}

	/// <summary>
	/// Updates any camera-dependent settings, such as
	/// the calculated pixel-perfect size.
	/// Use this with BroadcastMessage() to do bulk
	/// re-calculation of object sizes whenever your
	/// screensize/resolution changes at runtime.
	/// </summary>
	public void UpdateCamera()
	{
		SetupCameraAndSizes();
		CalcClippingRect();
		RepositionItems();
	}


	public void SetupCameraAndSizes()
	{
		// Convert our values to the proper units:
		if (renderCamera == null)
		{
			if (UIManager.Exists() && UIManager.instance.uiCameras[0].camera != null)
				renderCamera = UIManager.instance.uiCameras[0].camera;
			else
				renderCamera = Camera.main;
		}

		if (unitsInPixels)
		{
			CalcScreenToWorldUnits();
			viewableAreaActual = new Vector2(viewableArea.x * localUnitsPerPixel, viewableArea.y * localUnitsPerPixel);
			itemSpacingActual = itemSpacing * localUnitsPerPixel;
			extraEndSpacingActual = extraEndSpacing * localUnitsPerPixel;
		}
		else
		{
			viewableAreaActual = viewableArea;
			itemSpacingActual = itemSpacing;
			extraEndSpacingActual = extraEndSpacing;
		}

		for (int i = 0; i < items.Count; ++i)
			items[i].UpdateCamera();
	}

	protected void CalcScreenToWorldUnits()
	{
		Plane nearPlane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);

		// Determine the world distance between two vertical
		// screen pixels for this camera:
		float dist = nearPlane.GetDistanceToPoint(transform.position);
		localUnitsPerPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0, 1, dist)), renderCamera.ScreenToWorldPoint(new Vector3(0, 0, dist)));
	}

	// Updates the clipping rect if, for example, the viewable area changes.
	protected void CalcClippingRect()
	{
		clientClippingRect.FromPoints(new Vector3(-viewableAreaActual.x * 0.5f, viewableAreaActual.y * 0.5f, 0),
									  new Vector3(viewableAreaActual.x * 0.5f, viewableAreaActual.y * 0.5f, 0),
									  new Vector3(-viewableAreaActual.x * 0.5f, -viewableAreaActual.y * 0.5f, 0));
		clientClippingRect.MultFast(transform.localToWorldMatrix);

		for(int i=0; i<items.Count; ++i)
		{
			if(items[i].TextObj != null)
				items[i].TextObj.ClippingRect = clientClippingRect;
		}
	}

	// Is called when the optional slider control
	// is moved.
	public void SliderMoved(IUIObject slider)
	{
		ScrollListTo_Internal(((UISlider)slider).Value);
	}

	// Listens for when the slider knob is released.
	public void SliderInputDel(ref POINTER_INFO ptr)
	{
		if (!snap)
			return; // We're only interested for the purposes of snapping.

		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP ||
			ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE ||
			ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF)
			CalcSnapItem();
	}

	// Internal version of ScrollListTo that doesn't eliminate
	// scroll coasting inertia, etc.
	protected void ScrollListTo_Internal(float pos)
	{
		if (float.IsNaN(pos) || mover == null)
			return; // Ignore since the viewing area exactly matches our content, no need to scroll anyway

		if (orientation == ORIENTATION.VERTICAL)
		{
			float directionFactor = (direction == DIRECTION.TtoB_LtoR) ? (1f) : (-1f);

			mover.transform.localPosition = (Vector3.up * directionFactor * Mathf.Clamp(amtOfPlay, 0, amtOfPlay) * pos);
		}
		else
		{
			float directionFactor = (direction == DIRECTION.TtoB_LtoR) ? (-1f) : (1f);

			mover.transform.localPosition = (Vector3.right * directionFactor * Mathf.Clamp(amtOfPlay, 0, amtOfPlay) * pos);
		}
		scrollPos = pos;
		ClipItems();

		if (slider != null)
			slider.Value = scrollPos;
	}

	/// <summary>
	/// Scrolls the list directly to the position
	/// indicated (0-1).
	/// </summary>
	/// <param name="pos">Position of the list - 0-1 (0 == beginning, 1 == end)</param>
	public void ScrollListTo(float pos)
	{
		scrollInertia = 0;
		scrollDelta = 0;
		isScrolling = false;
		autoScrolling = false;

		ScrollListTo_Internal(pos);
	}

	/// <summary>
	/// Sets or retrieves the scroll position of the list.
	/// The position is given as a value between 0 and 1.
	/// 0 indicates the beginning of the list, 1 the end.
	/// </summary>
	public float ScrollPosition
	{
		get { return scrollPos; }
		set { ScrollListTo(value); }
	}

	/// <summary>
	/// The item to which the list is snapped, or is in the process
	/// of snapping.  Only gets set when "Snap" is enabled, or when
	/// ScrollToItem() is used to scroll directly to a particular
	/// item.
	/// </summary>
	public IUIListObject SnappedItem
	{
		get { return snappedItem; }
	}

	/// <summary>
	/// Scrolls the list to the specified item in the specified
	/// number of seconds.
	/// </summary>
	/// <param name="item">The item to scroll to.</param>
	/// <param name="scrollTime">The number of seconds the scroll should last.</param>
	/// <param name="easing">The type of easing to be used for the scroll.</param>
	public void ScrollToItem(IUIListObject item, float scrollTime, EZAnimation.EASING_TYPE easing)
	{
		snappedItem = item;

		if (newItems.Count != 0)
		{
			if (itemsInserted || doItemEasing)
				RepositionItems();
			else
				PositionNewItems();

			itemsInserted = false;
			newItems.Clear();
		}

		if (orientation == ORIENTATION.HORIZONTAL)
		{
			if(direction == DIRECTION.TtoB_LtoR)
				autoScrollPos = Mathf.Clamp01(item.transform.localPosition.x / amtOfPlay);
			else
				autoScrollPos = Mathf.Clamp01(-item.transform.localPosition.x / amtOfPlay);
		}
		else
		{
			if(direction == DIRECTION.TtoB_LtoR)
				autoScrollPos = Mathf.Clamp01(-item.transform.localPosition.y / amtOfPlay);
			else
				autoScrollPos = Mathf.Clamp01(item.transform.localPosition.y / amtOfPlay);
		}

		autoScrollInterpolator = EZAnimation.GetInterpolator(easing);
		autoScrollStart = scrollPos;
		autoScrollDelta = autoScrollPos - scrollPos;
		autoScrollDuration = scrollTime;
		autoScrollTime = 0;
		autoScrolling = true;

		// Do some state cleanup:
		scrollDelta = 0;
		isScrolling = false;

		if (itemSnappedDel != null)
			itemSnappedDel(snappedItem);
	}

	/// <summary>
	/// Scrolls the list to the specified item in the specified
	/// number of seconds.
	/// </summary>
	/// <param name="index">The index of the item to scroll to.</param>
	/// <param name="scrollTime">The number of seconds the scroll should last.</param>
	/// <param name="easing">The type of easing to be used for the scroll.</param>
	public void ScrollToItem(int index, float scrollTime, EZAnimation.EASING_TYPE easing)
	{
		if(index < 0 || index >= items.Count)
			return;
		ScrollToItem(items[index], scrollTime, easing);
	}

	/// <summary>
	/// Scrolls the list to the specified item in the specified
	/// number of seconds.
	/// </summary>
	/// <param name="item">The item to scroll to.</param>
	/// <param name="scrollTime">The number of seconds the scroll should last.</param>
	public void ScrollToItem(IUIListObject item, float scrollTime)
	{
		ScrollToItem(item, scrollTime, snapEasing);
	}

	/// <summary>
	/// Scrolls the list to the specified item in the specified
	/// number of seconds.
	/// </summary>
	/// <param name="index">The index of the item to scroll to.</param>
	/// <param name="scrollTime">The number of seconds the scroll should last.</param>
	public void ScrollToItem(int index, float scrollTime)
	{
		ScrollToItem(index, scrollTime, snapEasing);
	}

	/// <summary>
	/// Sizes the viewable area of the scroll list using
	/// pixel dimensions as the unit of measurement.
	/// </summary>
	/// <param name="cam">The camera through which the list will be viewed.</param>
	/// <param name="width">The width, in pixels, of the list's viewable area.</param>
	/// <param name="height">The height, in pixels, of the list's viewable area.</param>
	public void SetViewableAreaPixelDimensions(Camera cam, int width, int height)
	{
		Plane nearPlane = new Plane(cam.transform.forward, cam.transform.position);

		// Determine the world distance between two vertical
		// screen pixels for this camera:
		float dist = nearPlane.GetDistanceToPoint(transform.position);
		float worldUnitsPerScreenPixel = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(0, 1, dist)), cam.ScreenToWorldPoint(new Vector3(0, 0, dist)));
		viewableAreaActual = new Vector2(width * worldUnitsPerScreenPixel, height * worldUnitsPerScreenPixel);

		CalcClippingRect();

		RepositionItems();
	}

	/// <summary>
	/// Inserts a list item at the specified position in the list.
	/// </summary>
	/// <param name="item">Reference to the item to be inserted into the list.</param>
	/// <param name="position">0-based index of the position in the list where the item will be placed.</param>
	public void InsertItem(IUIListObject item, int position)
	{
		InsertItem(item, position, null, false);
	}

	/// <summary>
	/// Inserts a list item at the specified position in the list.
	/// </summary>
	/// <param name="item">Reference to the item to be inserted into the list.</param>
	/// <param name="position">0-based index of the position in the list where the item will be placed.</param>
	/// <param name="doEasing">Indicates whether easing should be performed to reposition other items in the list when making room for the inserted item.  Only applies if the item is being inserted before another item.</param>
	public void InsertItem(IUIListObject item, int position, bool doEasing)
	{
		InsertItem(item, position, null, doEasing);
	}


	/// <summary>
	/// Inserts a list item at the specified position in the list.
	/// </summary>
	/// <param name="item">Reference to the item to be inserted into the list.</param>
	/// <param name="position">0-based index of the position in the list where the item will be placed.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	public void InsertItem(IUIListObject item, int position, string text)
	{
		InsertItem(item, position, text, false);
	}

	/// <summary>
	/// Inserts a list item at the specified position in the list.
	/// </summary>
	/// <param name="item">Reference to the item to be inserted into the list.</param>
	/// <param name="position">0-based index of the position in the list where the item will be placed.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	/// <param name="doEasing">Indicates whether easing should be performed to reposition other items in the list when making room for the inserted item.  Only applies if the item is being inserted before another item.</param>
	public void InsertItem(IUIListObject item, int position, string text, bool doEasing)
	{
		// Only ease if the item is being inserted before another item:
		if (position >= items.Count)
			doItemEasing = false;
		else
			doItemEasing = doEasing;

		doPosEasing = doEasing;

		// Make sure Awake() has already run:
		if (!m_awake)
			Awake();

		// Make sure Start() has already run:
		if (!m_started)
			Start();

		// See if the item needs to be enabled:
		if (activateWhenAdding)
		{
			if (!((Component)item).gameObject.active)
				((Component)item).gameObject.SetActiveRecursively(true);
		}

		// Now deactivate again if the list itself is deactivated:
		if(!gameObject.active)
			((Component)item).gameObject.SetActiveRecursively(false);

		// Put the item in the correct layer:
		item.gameObject.layer = gameObject.layer;

		// Add the item to our container:
		if (container != null)
			container.AddChild(item.gameObject);


		//-------------------------------------
		// Position our item:
		//-------------------------------------
		item.transform.parent = mover.transform;
#if AUTO_ORIENT_ITEMS
		item.transform.localRotation = Quaternion.identity;
#endif
#if AUTO_SCALE_ITEMS
		item.transform.localScale = Vector3.one;
#endif
		// Go ahead and get the item in the mover's plane
		// on the local Z-axis.  This must be done here
		// before anything that follows because if we are
		// using a perspective camera and these are newly
		// created items, their Start() will be called for
		// the first time when FindOuterEdges() is called
		// either by us, or by the Text property, and if
		// the item isn't already positioned relative to
		// the camera, then its size will be calculated
		// wrong.
		item.transform.localPosition = Vector3.zero;

		item.SetList(this);

		if (text != null)
			item.Text = text;

		// Clamp our position:
		position = Mathf.Clamp(position, 0, items.Count);

		// Hide the item by default:
		if (clipContents)
		{
			item.Hide(true);
			if (!item.Managed)
				item.gameObject.SetActiveRecursively(false);
		}

		item.Index = position;

		// Add the item:
		newItems.Add(item);
		if (position != items.Count)
		{
			itemsInserted = true;
			items.Insert(position, item);

			// Add to our visible items so that
			// it will be clipped in the next go:
			if (visibleItems.Count == 0)
			{
				visibleItems.Add(item);
			}
			else if (item.Index > 0)
			{
				int prevIdx = visibleItems.IndexOf(items[item.Index - 1]);
				if (prevIdx == -1)
				{
					// See if it should be added to the beginning or the end:
					if (visibleItems[0].Index >= item.Index)
						visibleItems.Insert(0, item);
					else
						visibleItems.Add(item);
				}
				else
					visibleItems.Insert(prevIdx + 1, item);
			}
		}
		else
		{
			items.Add(item);

			// Add to our visible items so that
			// it will be clipped in the next go:
			visibleItems.Add(item);
		}

		// See if we need to go ahead and position the item:
		if(positionItemsImmediately)
		{
			if (itemsInserted || doItemEasing)
			{
				RepositionItems();
				itemsInserted = false;
				newItems.Clear();
			}
			else
				PositionNewItems();
		}

		// Compute the edges of the item:
		/*		item.FindOuterEdges();
				item.UpdateCollider();
		
				// See if we can just add it to the end:
				if(position == items.Count)
				{
					float x=0, y=0;
					bool addItemSpacing = false;

					if (orientation == ORIENTATION.HORIZONTAL)
					{
						// Find the X-coordinate:
						if (items.Count > 0)
						{
							addItemSpacing = true; // We will be adding itemSpacing

							lastItem = items[items.Count - 1];

							if (direction == DIRECTION.TtoB_LtoR)
								x = lastItem.transform.localPosition.x + lastItem.BottomRightEdge.x + itemSpacing - item.TopLeftEdge.x;
							else
								x = lastItem.transform.localPosition.x - lastItem.BottomRightEdge.x - itemSpacing + item.TopLeftEdge.x;
						}
						else
						{
							if (spacingAtEnds)
								addItemSpacing = true; // We will be adding itemSpacing

							if (direction == DIRECTION.TtoB_LtoR)
								x = (viewableAreaActual.x * -0.5f) - item.TopLeftEdge.x + ((spacingAtEnds) ? (itemSpacing) : (0)) + extraEndSpacing;
							else
								x = (viewableAreaActual.x * 0.5f) - item.BottomRightEdge.x - ((spacingAtEnds) ? (itemSpacing) : (0)) - extraEndSpacing;
						}

						// Find the Y-coordinate:
						switch(alignment)
						{
							case ALIGNMENT.CENTER:
								y = 0;
								break;
							case ALIGNMENT.LEFT_TOP:
								y = (viewableAreaActual.y * 0.5f) - item.TopLeftEdge.y;
								break;
							case ALIGNMENT.RIGHT_BOTTOM:
								y = (viewableAreaActual.y * -0.5f) - item.BottomRightEdge.y;
								break;
						}

						contentDelta = item.BottomRightEdge.x - item.TopLeftEdge.x + ((addItemSpacing && lastItem != null) ? (itemSpacing) : (0));
					}
					else
					{
						// Determine the Y-coordinate:
						if (items.Count > 0)
						{
							addItemSpacing = true; // We will be adding itemSpacing

							lastItem = items[items.Count - 1];

							if(direction == DIRECTION.TtoB_LtoR)
								y = lastItem.transform.localPosition.y + lastItem.BottomRightEdge.y - itemSpacing - item.TopLeftEdge.y;
							else
								y = lastItem.transform.localPosition.y - lastItem.BottomRightEdge.y + itemSpacing + item.TopLeftEdge.y;
						}
						else
						{
							if (spacingAtEnds)
								addItemSpacing = true; // We will be adding itemSpacing

							if(direction == DIRECTION.TtoB_LtoR)
								y = (viewableAreaActual.y * 0.5f) - item.TopLeftEdge.y - ((spacingAtEnds) ? (itemSpacing) : (0)) - extraEndSpacing;
							else
								y = (viewableAreaActual.y * -0.5f) - item.BottomRightEdge.y + ((spacingAtEnds) ? (itemSpacing) : (0)) + extraEndSpacing;
						}

						// Determine the X-coordinate:
						switch (alignment)
						{
							case ALIGNMENT.CENTER:
								x = 0;
								break;
							case ALIGNMENT.LEFT_TOP:
								x = (viewableAreaActual.x * -0.5f) - item.TopLeftEdge.x;
								break;
							case ALIGNMENT.RIGHT_BOTTOM:
								x = (viewableAreaActual.x * 0.5f) - item.BottomRightEdge.x;
								break;
						}

						contentDelta = item.TopLeftEdge.y - item.BottomRightEdge.y + ((addItemSpacing && lastItem != null) ? (itemSpacing) : (0));
					}

					// Position the new item:
					item.transform.localPosition = new Vector3(x, y, 0);

					item.Index = items.Count;
					items.Add(item);

					UpdateContentExtents(contentDelta);
					ClipItems();
				}
				else
				{
					// Else, insert the item in the midst of our list:
					items.Insert(position, item);

					PositionItems();
				}
		*/
	}

	protected void PositionNewItems()
	{
		IUIListObject item, lastItem = null;
		float contentDelta = 0;

		for (int i = 0; i < newItems.Count; ++i )
		{
			if (null == newItems[i])
				continue;	// Must have been destroyed in the same frame as it was added

			int index = newItems[i].Index;
			item = items[index];

			// Compute the edges of the item:
			item.FindOuterEdges();
			item.UpdateCollider();

			float x = 0, y = 0;
			bool addItemSpacing = false;

			if (orientation == ORIENTATION.HORIZONTAL)
			{
				// Find the X-coordinate:
				if (index > 0)
				{
					addItemSpacing = true; // We will be adding itemSpacing

					lastItem = items[index - 1];

					if (direction == DIRECTION.TtoB_LtoR)
						x = lastItem.transform.localPosition.x + lastItem.BottomRightEdge.x + itemSpacingActual - item.TopLeftEdge.x;
					else
						x = lastItem.transform.localPosition.x - lastItem.BottomRightEdge.x - itemSpacingActual + item.TopLeftEdge.x;
				}
				else
				{
					if (spacingAtEnds)
						addItemSpacing = true; // We will be adding itemSpacing

					if (direction == DIRECTION.TtoB_LtoR)
						x = (viewableAreaActual.x * -0.5f) - item.TopLeftEdge.x + ((spacingAtEnds) ? (itemSpacingActual) : (0)) + extraEndSpacingActual;
					else
						x = (viewableAreaActual.x * 0.5f) - item.BottomRightEdge.x - ((spacingAtEnds) ? (itemSpacingActual) : (0)) - extraEndSpacingActual;
				}

				// Find the Y-coordinate:
				switch (alignment)
				{
					case ALIGNMENT.CENTER:
						y = 0;
						break;
					case ALIGNMENT.LEFT_TOP:
						y = (viewableAreaActual.y * 0.5f) - item.TopLeftEdge.y;
						break;
					case ALIGNMENT.RIGHT_BOTTOM:
						y = (viewableAreaActual.y * -0.5f) - item.BottomRightEdge.y;
						break;
				}

				contentDelta += item.BottomRightEdge.x - item.TopLeftEdge.x + ((addItemSpacing && lastItem != null) ? (itemSpacingActual) : (0));
			}
			else
			{
				// Determine the Y-coordinate:
				if (index > 0)
				{
					addItemSpacing = true; // We will be adding itemSpacing

					lastItem = items[index - 1];

					if (direction == DIRECTION.TtoB_LtoR)
						y = lastItem.transform.localPosition.y + lastItem.BottomRightEdge.y - itemSpacingActual - item.TopLeftEdge.y;
					else
						y = lastItem.transform.localPosition.y - lastItem.BottomRightEdge.y + itemSpacingActual + item.TopLeftEdge.y;
				}
				else
				{
					if (spacingAtEnds)
						addItemSpacing = true; // We will be adding itemSpacing

					if (direction == DIRECTION.TtoB_LtoR)
						y = (viewableAreaActual.y * 0.5f) - item.TopLeftEdge.y - ((spacingAtEnds) ? (itemSpacingActual) : (0)) - extraEndSpacingActual;
					else
						y = (viewableAreaActual.y * -0.5f) - item.BottomRightEdge.y + ((spacingAtEnds) ? (itemSpacingActual) : (0)) + extraEndSpacingActual;
				}

				// Determine the X-coordinate:
				switch (alignment)
				{
					case ALIGNMENT.CENTER:
						x = 0;
						break;
					case ALIGNMENT.LEFT_TOP:
						x = (viewableAreaActual.x * -0.5f) - item.TopLeftEdge.x;
						break;
					case ALIGNMENT.RIGHT_BOTTOM:
						x = (viewableAreaActual.x * 0.5f) - item.BottomRightEdge.x;
						break;
				}

				contentDelta += item.TopLeftEdge.y - item.BottomRightEdge.y + ((addItemSpacing && lastItem != null) ? (itemSpacingActual) : (0));
			}

			// Position the new item:
			item.transform.localPosition = new Vector3(x, y, 0);
		}

		UpdateContentExtents(contentDelta);
		ClipItems();

		newItems.Clear();
	}

	/// <summary>
	/// Adds an item to the end of the list.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="item">Reference to a GameObject containing a list item to be added.</param>
	public void AddItem(GameObject itemGO)
	{
		IUIListObject item = (IUIListObject)itemGO.GetComponent(typeof(IUIListObject));
		if (item == null)
		{
			Debug.LogWarning("GameObject \"" + itemGO.name + "\" does not contain any list item component suitable to be added to scroll list \"" + name + "\".");
			return;
		}

		AddItem(item, null);
	}

	/// <summary>
	/// Adds an item to the end of the list.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="item">Reference to the list item to be added.</param>
	public void AddItem(IUIListObject item)
	{
		AddItem(item, null);
	}

	/// <summary>
	/// Adds an item to the end of the list.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="item">Reference to the list item to be added.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	public void AddItem(IUIListObject item, string text)
	{
		if (!m_awake)
			Awake();
		if (!m_started)
			Start();

		InsertItem(item, items.Count, text, false);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the end of the 
	/// list and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab)
	{
		if (!m_awake)
			Awake();
		if (!m_started)
			Start();

		return CreateItem(prefab, items.Count, null);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the end of the 
	/// list and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, string text)
	{
		if (!m_awake)
			Awake();
		if (!m_started)
			Start();

		return CreateItem(prefab, items.Count, text);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the list at the position 
	/// specified by "position" and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="position">0-based index where the new item will be placed in the list.</param>
	/// <param name="doEasing">Indicates whether easing should be performed to reposition other items in the list when making room for the inserted item.  Only applies if the item is being inserted before another item.</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, int position, bool doEasing)
	{
		return CreateItem(prefab, position, null, doEasing);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the list at the position 
	/// specified by "position" and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="position">0-based index where the new item will be placed in the list.</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, int position)
	{
		return CreateItem(prefab, position, null, false);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the list at the position 
	/// specified by "position" and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="position">0-based index where the new item will be placed in the list.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, int position, string text)
	{
		return CreateItem(prefab, position, text, false);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the list at the position 
	/// specified by "position" and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="position">0-based index where the new item will be placed in the list.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	/// <param name="doEasing">Indicates whether easing should be performed to reposition other items in the list when making room for the inserted item.  Only applies if the item is being inserted before another item.</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, int position, string text, bool doEasing)
	{
		IUIListObject newItem;
		GameObject go;

		newItem = (IUIListObject)prefab.GetComponent(typeof(IUIListObject));

		if (null == newItem)
			return null;

		newItem.RenderCamera = renderCamera;

		if (manager != null)
		{	// Managed:
			if(newItem.IsContainer())
			{
				// This object contains other sprite-based objects:
				go = (GameObject)Instantiate(prefab);

				Component[] sprites = go.GetComponentsInChildren(typeof(SpriteRoot));
				for (int i = 0; i < sprites.Length; ++i)
					manager.AddSprite((SpriteRoot)sprites[i]);
			}
			else
			{
				SpriteRoot s = manager.CreateSprite(prefab);

				if (s == null)
					return null;

				go = s.gameObject;
			}
		}
		else // Unmanaged:
		{
			go = (GameObject)Instantiate(prefab);
		}

		newItem = (IUIListObject)go.GetComponent(typeof(IUIListObject));

		if (newItem == null)
			return null;

		//newItem.IsClone = true;

		// Make sure it has a collider:
/*
		if (go.collider == null)
		{
			go.AddComponent(typeof(BoxCollider));
		}
*/

		InsertItem(newItem, position, text, doEasing);
		return newItem;
	}

	// Updates the extents of the content area by
	// the specified amount.
	// change: The amount the content extents have changed (+/-)
	protected void UpdateContentExtents(float change)
	{
		float oldAmtOfPlay = amtOfPlay;

		float addlSpacing = (spacingAtEnds ? (itemSpacingActual * 2f) : 0) + extraEndSpacingActual * 2f;

		contentExtents += change;

		if (orientation == ORIENTATION.HORIZONTAL)
		{
			// Adjust the scroll position:
			amtOfPlay = ((contentExtents + addlSpacing) - viewableAreaActual.x);
			scrollMax = (viewableAreaActual.x / ((contentExtents + addlSpacing) - viewableAreaActual.x)) * overscrollAllowance;
		}
		else
		{
			// Adjust the scroll position:
			amtOfPlay = ((contentExtents + addlSpacing) - viewableAreaActual.y);
			scrollMax = (viewableAreaActual.y / ((contentExtents + addlSpacing) - viewableAreaActual.y)) * overscrollAllowance;
		}

		float newPos = ((oldAmtOfPlay * scrollPos) / amtOfPlay);

		// See if we need to ease into position:
		if (doPosEasing && newPos > 1f)
		{
			scrollPosAnim = AnimatePosition.Do(gameObject, EZAnimation.ANIM_MODE.By, Vector3.zero, ScrollPosInterpolator, positionEaseDuration, positionEaseDelay, null, OnPosEasingDone);
			scrollPosAnim.Data = new Vector2(newPos, 1f - newPos); // (Start, Delta)
			itemEasers.Add(scrollPosAnim);
		}
		else
			ScrollListTo_Internal(Mathf.Clamp01(newPos));

		doPosEasing = false;
	}

	// Allows us to use an EZAnimation for our scroll position easing
	protected float ScrollPosInterpolator(float time, float start, float delta, float duration)
	{
		Vector2 startDelta = (Vector2)scrollPosAnim.Data;

		ScrollListTo_Internal(EZAnimation.GetInterpolator(positionEasing)(time, startDelta.x, startDelta.y, duration));

		// We're done:
		if (time >= duration)
			scrollPosAnim = null;

		return start;
	}

	//=====================================================
	// Alignment delegates
	//=====================================================
	protected float GetYCentered(IUIListObject item)
	{	return 0;	}

	protected float GetYAlignTop(IUIListObject item)
	{ return (viewableAreaActual.y * 0.5f) - item.TopLeftEdge.y; }

	protected float GetYAlignBottom(IUIListObject item)
	{ return (viewableAreaActual.y * -0.5f) - item.BottomRightEdge.y; }

	protected float GetXCentered(IUIListObject item)
	{	return 0;	}

	protected float GetXAlignLeft(IUIListObject item)
	{ return (viewableAreaActual.x * -0.5f) - item.TopLeftEdge.x; }

	protected float GetXAlignRight(IUIListObject item)
	{ return (viewableAreaActual.x * 0.5f) - item.BottomRightEdge.x; }
	//=====================================================
	// End Alignment delegates
	//=====================================================

	/// <summary>
	/// Positions list items according to the
	/// current scroll position.
	/// </summary>
	public void PositionItems()
	{
		if (itemEasers.Count > 0)
		{
			// Cancel any pending easings:
			for (int i = 0; i < itemEasers.Count; ++i)
			{
				itemEasers[i].CompletedDelegate = null; // Prevent end delegate call
				itemEasers[i].End();
			}

			itemEasers.Clear();

			// Unlock input:
			if (blockInputWhileEasing)
				UIManager.instance.UnlockInput();
		}

		if (orientation == ORIENTATION.HORIZONTAL)
			PositionHorizontally(false);
		else
			PositionVertically(false);

		UpdateContentExtents(0);	// Just so other stuff gets updated
		ClipItems();

		// See if we need to lock input for easing:
		if (itemEasers.Count > 0 && blockInputWhileEasing)
			UIManager.instance.LockInput();

		doItemEasing = false;
	}

	/// <summary>
	/// Repositions list items according to the
	/// current scroll position, and adjusts for
	/// any change in the items' extents.
	/// </summary>
	public void RepositionItems()
	{
		if (itemEasers.Count > 0)
		{
			// Cancel any pending easings:
			for (int i = 0; i < itemEasers.Count; ++i)
			{
				itemEasers[i].CompletedDelegate = null; // Prevent end delegate call
				itemEasers[i].End();
			}

			itemEasers.Clear();

			// Unlock input:
			if (blockInputWhileEasing)
				UIManager.instance.UnlockInput();
		}

		if (orientation == ORIENTATION.HORIZONTAL)
			PositionHorizontally(true);
		else
			PositionVertically(true);

		UpdateContentExtents(0);	// Just so other stuff gets updated
		ClipItems();

		// See if we need to lock input for easing:
		if (itemEasers.Count > 0 && blockInputWhileEasing)
			UIManager.instance.LockInput();

		doItemEasing = false;
	}

	// Positions list items horizontally.
	protected void PositionHorizontally(bool updateExtents)
	{
		// Will hold the leading edge of the list throughout
		float edge;

		float step;

		Vector3 newPos;

		ItemAlignmentDel GetYAlignment;

		// Don't count the end-spacing:
		contentExtents = 0;

		// Pick a suitable delegate for alignment:
		switch (alignment)
		{
			case ALIGNMENT.CENTER:
				GetYAlignment = GetYCentered;
				break;
			case ALIGNMENT.LEFT_TOP:
				GetYAlignment = GetYAlignTop;
				break;
			case ALIGNMENT.RIGHT_BOTTOM:
				GetYAlignment = GetYAlignBottom;
				break;
			default:
				GetYAlignment = GetYCentered;
				break;
		}


		if(direction == DIRECTION.TtoB_LtoR)
		{
			edge = (viewableAreaActual.x * -0.5f) + ((spacingAtEnds) ? (itemSpacingActual) : (0)) + extraEndSpacingActual;

			for (int i = 0; i < items.Count; ++i)
			{
				if (updateExtents)
				{
					items[i].FindOuterEdges();
					items[i].UpdateCollider();
				}

				newPos = new Vector3(edge - items[i].TopLeftEdge.x, GetYAlignment(items[i]), 0);

				if (doItemEasing)
				{
					if (newItems.Contains(items[i]))
						items[i].transform.localPosition = newPos;
					else
					{
						itemEasers.Add(AnimatePosition.Do(items[i].gameObject, EZAnimation.ANIM_MODE.To, newPos, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
					}
				}
				else
					items[i].transform.localPosition = newPos;

				step = items[i].BottomRightEdge.x - items[i].TopLeftEdge.x + itemSpacingActual;
				contentExtents += step;
				edge += step;

				// Assign indices:
				items[i].Index = i;
			}

			if (!spacingAtEnds)
				contentExtents -= itemSpacingActual;
		}
		else
		{
			edge = (viewableAreaActual.x * 0.5f) - ((spacingAtEnds) ? (itemSpacingActual) : (0)) - extraEndSpacingActual;

			for (int i = 0; i < items.Count; ++i)
			{
				if (updateExtents)
				{
					items[i].FindOuterEdges();
					items[i].UpdateCollider();
				}

				newPos = new Vector3(edge - items[i].BottomRightEdge.x, GetYAlignment(items[i]), 0);

				if (doItemEasing)
				{
					if (newItems.Contains(items[i]))
						items[i].transform.localPosition = newPos;
					else
					{
						itemEasers.Add(AnimatePosition.Do(items[i].gameObject, EZAnimation.ANIM_MODE.To, newPos, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
					}
				}
				else
					items[i].transform.localPosition = newPos;
				
				step = items[i].BottomRightEdge.x - items[i].TopLeftEdge.x + itemSpacingActual;
				contentExtents += step;
				edge -= step;

				// Assign indices:
				items[i].Index = i;
			}

			if (!spacingAtEnds)
				contentExtents -= itemSpacingActual;
		}
	}

	// Positions list items vertically.
	protected void PositionVertically(bool updateExtents)
	{
		// Will hold the leading edge of the list throughout
		float edge;

		float step;

		Vector3 newPos;

		ItemAlignmentDel GetXAlignment;

		// Don't count the end-spacing:
		contentExtents = 0;

		// Pick a suitable delegate for alignment:
		switch (alignment)
		{
			case ALIGNMENT.CENTER:
				GetXAlignment = GetXCentered;
				break;
			case ALIGNMENT.LEFT_TOP:
				GetXAlignment = GetXAlignLeft;
				break;
			case ALIGNMENT.RIGHT_BOTTOM:
				GetXAlignment = GetXAlignRight;
				break;
			default:
				GetXAlignment = GetXCentered;
				break;
		}

		if(direction == DIRECTION.TtoB_LtoR)
		{
			edge = (viewableAreaActual.y * 0.5f) - ((spacingAtEnds) ? (itemSpacingActual) : (0)) - extraEndSpacingActual;

			for (int i = 0; i < items.Count; ++i)
			{
				if (updateExtents)
				{
					items[i].FindOuterEdges();
					items[i].UpdateCollider();
				}

				newPos = new Vector3(GetXAlignment(items[i]), edge - items[i].TopLeftEdge.y, 0);

				if (doItemEasing)
				{
					if (newItems.Contains(items[i]))
						items[i].transform.localPosition = newPos;
					else
					{
						itemEasers.Add(AnimatePosition.Do(items[i].gameObject, EZAnimation.ANIM_MODE.To, newPos, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
					}
				}
				else
					items[i].transform.localPosition = newPos;
				
				step = items[i].TopLeftEdge.y - items[i].BottomRightEdge.y + itemSpacingActual;
				contentExtents += step;
				edge -= step;

				// Assign indices:
				items[i].Index = i;
			}

			if (!spacingAtEnds)
				contentExtents -= itemSpacingActual;
		}
		else
		{
			edge = (viewableAreaActual.y * -0.5f) + ((spacingAtEnds) ? (itemSpacingActual) : (0)) + extraEndSpacingActual;

			for (int i = 0; i < items.Count; ++i)
			{
				if (updateExtents)
				{
					items[i].FindOuterEdges();
					items[i].UpdateCollider();
				}

				newPos = new Vector3(GetXAlignment(items[i]), edge - items[i].BottomRightEdge.y, 0);

				if (doItemEasing)
				{
					if (newItems.Contains(items[i]))
						items[i].transform.localPosition = newPos;
					else
					{
						itemEasers.Add(AnimatePosition.Do(items[i].gameObject, EZAnimation.ANIM_MODE.To, newPos, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
					}
				}
				else
					items[i].transform.localPosition = newPos;
				
				step = items[i].TopLeftEdge.y - items[i].BottomRightEdge.y + itemSpacingActual;
				contentExtents += step;
				edge += step;

				// Assign indices:
				items[i].Index = i;
			}

			if (!spacingAtEnds)
				contentExtents -= itemSpacingActual;
		}
	}


	// Called when a position easing animation ends.
	protected void OnPosEasingDone(EZAnimation anim)
	{
		itemEasers.Remove(anim);

		// If that was the last one, see if we need to
		// unlock input:
		if (itemEasers.Count == 0 && blockInputWhileEasing)
			UIManager.instance.UnlockInput();
	}


	// Clips list items to the viewable area.
	public void ClipItems()
	{
		if (mover == null || items.Count < 1 || !clipContents || !gameObject.active)
			return;

		IUIListObject firstItem = null;
		IUIListObject lastItem = null;


		if (orientation == ORIENTATION.HORIZONTAL)
		{
			float moverOffset = mover.transform.localPosition.x;
			float itemOffset;
			// Calculate the visible edges inside the mover's local space:
			float leftVisibleEdge = (viewableAreaActual.x * -0.5f) - moverOffset;
			float rightVisibleEdge = (viewableAreaActual.x * 0.5f) - moverOffset;

			// Find the first visible item:
			// Start looking at our approximate scroll position:
			int index = (int)(((float)(items.Count - 1)) * Mathf.Clamp01(scrollPos));

			if (direction == DIRECTION.TtoB_LtoR)
			{
				// See if the first item we checked is to the right
				// of our left-most viewable edge:
				itemOffset = items[index].transform.localPosition.x;
				if (items[index].BottomRightEdge.x + itemOffset >= leftVisibleEdge)
				{
					// Search backward until we find one that is not:
					for (index -= 1; index > -1; --index)
					{
						itemOffset = items[index].transform.localPosition.x;
						if (items[index].BottomRightEdge.x + itemOffset < leftVisibleEdge)
							break;
					}

					// The previous item is the one we're looking for:
					firstItem = items[index + 1];
				}
				else
				{
					// Search forward until we find the first visible item:
					for (; index < items.Count; ++index)
					{
						itemOffset = items[index].transform.localPosition.x;
						if (items[index].BottomRightEdge.x + itemOffset >= leftVisibleEdge)
						{
							// We've found our first visible item:
							firstItem = items[index];
							break;
						}
// 						else
// 						{
// 							if (items[index].gameObject.active)
// 							{
// 								items[index].gameObject.SetActiveRecursively(false);
// 								items[index].Hide(true);
// 							}
// 						}
					}
				}


				if (firstItem != null)
				{
					// Add the first visible item to our list and clip it:
					tempVisItems.Add(firstItem);
					if (!firstItem.gameObject.active)
						firstItem.gameObject.SetActiveRecursively(true);
					firstItem.Hide(false);
					firstItem.ClippingRect = clientClippingRect;

					// See if this is the only visible item:
					itemOffset = firstItem.transform.localPosition.x;
					if (firstItem.BottomRightEdge.x + itemOffset < rightVisibleEdge)
					{
						// Now search forward until we find an item that is outside
						// the viewable area:
						for (index = firstItem.Index + 1; index < items.Count; ++index)
						{
							itemOffset = items[index].transform.localPosition.x;
							if (items[index].BottomRightEdge.x + itemOffset >= rightVisibleEdge)
							{
								// We've found the last visible item
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[index]);
								break;
							}
							else
							{
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].Clipped = false;
								tempVisItems.Add(items[index]);
							}
						}
					}
				}
			}
			else
			{
				// See if the first item we checked is to the left
				// of our right-most viewable edge:
				itemOffset = items[index].transform.localPosition.x;
				if (items[index].TopLeftEdge.x + itemOffset <= rightVisibleEdge)
				{
					// Search backward until we find one that is not:
					for (index -= 1; index > -1; --index)
					{
						itemOffset = items[index].transform.localPosition.x;
						if (items[index].TopLeftEdge.x + itemOffset > rightVisibleEdge)
							break;
					}

					// The previous item is the one we're looking for:
					firstItem = items[index + 1];
				}
				else
				{
					// Search forward until we find the first visible item:
					for (; index < items.Count; ++index)
					{
						itemOffset = items[index].transform.localPosition.x;
						if (items[index].TopLeftEdge.x + itemOffset <= rightVisibleEdge)
						{
							// We've found our first visible item:
							firstItem = items[index];
							break;
						}
// 						else
// 						{
// 							if (items[index].gameObject.active)
// 							{
// 								items[index].gameObject.SetActiveRecursively(false);
// 								items[index].Hide(true);
// 							}
// 						}
					}
				}


				if (firstItem != null)
				{
					// Add the first visible item to our list and clip it:
					tempVisItems.Add(firstItem);
					if (!firstItem.gameObject.active)
						firstItem.gameObject.SetActiveRecursively(true);
					firstItem.Hide(false);
					firstItem.ClippingRect = clientClippingRect;

					// See if this is the only visible item:
					itemOffset = firstItem.transform.localPosition.x;
					if (firstItem.TopLeftEdge.x + itemOffset > leftVisibleEdge)
					{
						// Now search forward until we find an item that is outside
						// the viewable area:
						for (index = firstItem.Index + 1; index < items.Count; ++index)
						{
							itemOffset = items[index].transform.localPosition.x;
							if (items[index].TopLeftEdge.x + itemOffset <= leftVisibleEdge)
							{
								// We've found the last visible item
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[index]);
								break;
							}
							else
							{
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].Clipped = false;
								tempVisItems.Add(items[index]);
							}
						}
					}
				}
			}
		}
		else
		{
			float moverOffset = mover.transform.localPosition.y;
			float itemOffset;
			// Calculate the visible edges inside the mover's local space:
			float topVisibleEdge = (viewableAreaActual.y * 0.5f) - moverOffset;
			float bottomVisibleEdge = (viewableAreaActual.y * -0.5f) - moverOffset;

			// Find the first visible item:
			// Start looking at our approximate scroll position:
			int index = (int)(((float)(items.Count - 1)) * Mathf.Clamp01(scrollPos));

			if (direction == DIRECTION.TtoB_LtoR)
			{
				// See if the first item we checked is below
				// our top-most viewable edge:
				itemOffset = items[index].transform.localPosition.y;
				if (items[index].BottomRightEdge.y + itemOffset <= topVisibleEdge)
				{
					// Search backward until we find one that is not:
					for (index -= 1; index > -1; --index)
					{
						itemOffset = items[index].transform.localPosition.y;
						if (items[index].BottomRightEdge.y + itemOffset > topVisibleEdge)
							break;
					}

					// The previous item is the one we're looking for:
					firstItem = items[index + 1];
				}
				else
				{
					// Search forward until we find the first visible item:
					for (; index < items.Count; ++index)
					{
						itemOffset = items[index].transform.localPosition.y;
						if (items[index].BottomRightEdge.y + itemOffset <= topVisibleEdge)
						{
							// We've found our first visible item:
							firstItem = items[index];
							break;
						}
// 						else
// 						{
// 							if (items[index].gameObject.active)
// 							{
// 								items[index].gameObject.SetActiveRecursively(false);
// 								items[index].Hide(true);
// 							}
// 						}
					}
				}


				if (firstItem != null)
				{
					// Add the first visible item to our list and clip it:
					tempVisItems.Add(firstItem);
					if (!firstItem.gameObject.active)
						firstItem.gameObject.SetActiveRecursively(true);
					firstItem.Hide(false);
					firstItem.ClippingRect = clientClippingRect;

					// See if this is the only visible item:
					itemOffset = firstItem.transform.localPosition.y;
					if (firstItem.BottomRightEdge.y + itemOffset > bottomVisibleEdge)
					{
						// Now search forward until we find an item that is outside
						// the viewable area:
						for (index = firstItem.Index + 1; index < items.Count; ++index)
						{
							itemOffset = items[index].transform.localPosition.y;
							if (items[index].BottomRightEdge.y + itemOffset <= bottomVisibleEdge)
							{
								// We've found the last visible item
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[index]);
								break;
							}
							else
							{
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].Clipped = false;
								tempVisItems.Add(items[index]);
							}
						}
					}
				}
			}
			else
			{
				// See if the first item we checked is above
				// our bottom-most viewable edge:
				itemOffset = items[index].transform.localPosition.y;
				if (items[index].TopLeftEdge.y + itemOffset >= bottomVisibleEdge)
				{
					// Search backward until we find one that is not:
					for (index -= 1; index > -1; --index)
					{
						itemOffset = items[index].transform.localPosition.y;
						if (items[index].TopLeftEdge.y + itemOffset < bottomVisibleEdge)
							break;
					}

					// The previous item is the one we're looking for:
					firstItem = items[index + 1];
				}
				else
				{
					// Search forward until we find the first visible item:
					for (; index < items.Count; ++index)
					{
						itemOffset = items[index].transform.localPosition.y;
						if (items[index].TopLeftEdge.y + itemOffset >= bottomVisibleEdge)
						{
							// We've found our first visible item:
							firstItem = items[index];
							break;
						}
// 						else
// 						{
// 							if (items[index].gameObject.active)
// 							{
// 								items[index].gameObject.SetActiveRecursively(false);
// 								items[index].Hide(true);
// 							}
// 						}
					}
				}


				if (firstItem != null)
				{
					// Add the first visible item to our list and clip it:
					tempVisItems.Add(firstItem);
					if (!firstItem.gameObject.active)
						firstItem.gameObject.SetActiveRecursively(true);
					firstItem.Hide(false);
					firstItem.ClippingRect = clientClippingRect;

					// See if this is the only visible item:
					itemOffset = firstItem.transform.localPosition.y;
					if (firstItem.TopLeftEdge.y + itemOffset < topVisibleEdge)
					{
						// Now search forward until we find an item that is outside
						// the viewable area:
						for (index = firstItem.Index + 1; index < items.Count; ++index)
						{
							itemOffset = items[index].transform.localPosition.y;
							if (items[index].TopLeftEdge.y + itemOffset >= topVisibleEdge)
							{
								// We've found the last visible item
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[index]);
								break;
							}
							else
							{
								if (!items[index].gameObject.active)
									items[index].gameObject.SetActiveRecursively(true);
								items[index].Hide(false);
								items[index].Clipped = false;
								tempVisItems.Add(items[index]);
							}
						}
					}
				}
			}
		}

		if (firstItem == null)
			return;

		lastItem = tempVisItems[tempVisItems.Count - 1];

		if (visibleItems.Count > 0)
		{
			// Hide any items that are no longer visible:

			// First see if our previous visible list lies entirely outside
			// our new list:
			if (visibleItems[0].Index > lastItem.Index ||
			   visibleItems[visibleItems.Count - 1].Index < firstItem.Index)
			{
				for (int i = 0; i < visibleItems.Count; ++i)
				{
					visibleItems[i].Hide(true);
					if (!visibleItems[i].Managed)
						visibleItems[i].gameObject.SetActiveRecursively(false);
				}
			}
			else
			{
				// Process items until we reach our first currently visible item:
				for (int i = 0; i < visibleItems.Count; ++i)
				{
					if (visibleItems[i].Index < firstItem.Index)
					{
						visibleItems[i].Hide(true);
						if (!visibleItems[i].Managed)
							visibleItems[i].gameObject.SetActiveRecursively(false);
					}
					else
						break;
				}

				// Process items from the end backward until we reach our
				// last currently visible item:
				for (int i = visibleItems.Count - 1; i > -1; --i)
				{
					if (visibleItems[i].Index > lastItem.Index)
					{
						visibleItems[i].Hide(true);
						if (!visibleItems[i].Managed)
							visibleItems[i].gameObject.SetActiveRecursively(false);
					}
					else
						break;
				}
			}
		}

		// Swap our lists:
		List<IUIListObject> swapList = visibleItems;
		visibleItems = tempVisItems;
		tempVisItems = swapList;
		tempVisItems.Clear();
	}

	// Called by a list item when it is selected:
	public void DidSelect(IUIListObject item)
	{
		if (selectedItem != null)
			selectedItem.selected = false;

		selectedItem = item;
		item.selected = true;

/*
		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvokeOnSelect, 0);
		if (changeDelegate != null)
			changeDelegate(this);
*/

		DidClick((IUIObject)item);
	}

	// Called by a list button when it is clicked
	public void DidClick(IUIObject item)
	{
		lastClickedControl = item;

		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvokeOnSelect, 0);
		if (changeDelegate != null)
			changeDelegate(this);
	}

	// Is called by a list item when a drag is detected.
	// For our purposes, it is only called when the drag
	// extends beyond the drag threshold.
	public void ListDragged(POINTER_INFO ptr)
	{
		if (!touchScroll || !controlIsEnabled)
			return;	// Ignore

		autoScrolling = false;

		// Calculate the pointer's motion relative to our control:
		Vector3 inputPoint1;
		Vector3 inputPoint2;
		Vector3 ptrVector;
		float dist;
		Plane ctrlPlane = default(Plane);

		// Early out:
		if(Mathf.Approximately(ptr.inputDelta.sqrMagnitude, 0))
		{
			scrollDelta = 0;
			return;
		}

		listMoved = true;

		ctrlPlane.SetNormalAndPosition(mover.transform.forward * -1f, mover.transform.position);

		ctrlPlane.Raycast(ptr.ray, out dist);
		inputPoint1 = ptr.ray.origin + ptr.ray.direction * dist;

		ctrlPlane.Raycast(ptr.prevRay, out dist);
		inputPoint2 = ptr.prevRay.origin + ptr.prevRay.direction * dist;

		// Get the input points into the local space of our list:
		inputPoint1 = transform.InverseTransformPoint(inputPoint1);
		inputPoint2 = transform.InverseTransformPoint(inputPoint2);

		ptrVector = inputPoint1 - inputPoint2;

		// Find what percentage of our content 
		// extent this value represents:
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			scrollDelta = (-ptrVector.x) / amtOfPlay;
			//scrollDelta *= transform.localScale.x;
		}
		else
		{
			scrollDelta = ptrVector.y / amtOfPlay;
			//scrollDelta *= transform.localScale.y;
		}


/*
		ptr.devicePos.z = mover.transform.position.z;
		ptrVector = cam.ScreenToWorldPoint(ptr.devicePos) - cam.ScreenToWorldPoint(ptr.devicePos - ptr.inputDelta);

		if(orientation == ORIENTATION.HORIZONTAL)
		{
			localVector = transform.TransformDirection(Vector3.right);
			scrollDelta = -Vector3.Project(ptrVector, localVector).x;
			scrollDelta *= transform.localScale.x;
			// Find what percentage of our content 
			// extent this value represents:
			scrollDelta /= ( (contentExtents+itemSpacing) - viewableAreaActual.x);
		}
		else
		{
			localVector = transform.TransformDirection(Vector3.up);
			scrollDelta = Vector3.Project(ptrVector, localVector).y;
			scrollDelta *= transform.localScale.y;
			// Find what percentage of our content 
			// extent this value represents:
			scrollDelta /= ((contentExtents + itemSpacing) - viewableAreaActual.y);
		}
*/

		float target = scrollPos + scrollDelta;

		if (target > 1f)
		{
			// Scale our delta according to how close we
			// are to reaching our max scroll:
			scrollDelta *= Mathf.Clamp01(1f - (target - 1f) / scrollMax);
		}
		else if (target < 0)
		{
			scrollDelta *= Mathf.Clamp01(1f + (target / scrollMax));
		}

		// See if the scroll delta needs to be inverted due to our
		// direction:
		if (direction == DIRECTION.BtoT_RtoL)
			scrollDelta *= -1f;

		ScrollListTo_Internal(scrollPos + scrollDelta);

		noTouch = false;
		isScrolling = true;
	}

	// Responds to a movement delta of the mouse scroll wheel
	public void ScrollWheel(float amt)
	{
		if (direction == DIRECTION.BtoT_RtoL)
			amt *= -1f;
		ScrollListTo(Mathf.Clamp01(scrollPos - ((amt * scrollWheelFactor) / amtOfPlay)));
	}

	// Is called by a list item or internally 
	// when the pointing device is released.
	public void PointerReleased()
	{
		noTouch = true;

		if (scrollInertia != 0)
			scrollDelta = scrollInertia;

		scrollInertia = 0;

		if(snap && listMoved)
			CalcSnapItem();

		listMoved = false;
	}

	public void OnEnable()
	{
		gameObject.SetActiveRecursively(true);

		if (repositionOnEnable)
		{
			// In case since our sub-objects were likely
			// disabled earlier, make sure their extents
			// are re-computed to be safe:
			RepositionItems();
		}
		
		ClipItems();
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
			{
				EZAnimator.instance.Stop(gameObject);
				EZAnimator.instance.Stop(this);
			}

			if (detargetOnDisable && UIManager.Exists())
			{
				UIManager.instance.Detarget(this);
			}
		}
	}

	//---------------------------------------------
	// Accessors:
	//---------------------------------------------

	/// <summary>
	/// The length of the content, from start to end.
	/// </summary>
	public float ContentExtents
	{
		get { return contentExtents; }
	}

	/// <summary>
	/// The total size of the portions of the list
	/// not able to fit into the viewable area at
	/// any given time.
	/// </summary>
	public float UnviewableArea
	{
		get { return amtOfPlay; }
	}

	/// <summary>
	/// Accessor that returns a reference to the
	/// currently selected item.  Null is returned
	/// if no item is currently selected.
	/// Can also be used to set the currently
	/// selected item.
	/// </summary>
	public IUIListObject SelectedItem
	{
		get { return selectedItem; }
		set
		{
			IUIListObject oldSel = selectedItem;

			// Unset the previous selection:
			if (selectedItem != null)
				selectedItem.selected = false;

			if(value == null)
			{
				selectedItem = null;
				return;
			}

			selectedItem = value;
			selectedItem.selected = true;

			// If the selected item changed,
			// notify our delegate:
			if (oldSel != selectedItem)
			{
				if (changeDelegate != null)
					changeDelegate(this);
			}
		}
	}

	/// <summary>
	/// The last control to have been clicked.
	/// This can be used for easily accessing nested children
	/// inside a UIListItemContainer, but can be used for any
	/// control that implements IUIObject.
	/// </summary>
	public IUIObject LastClickedControl
	{
		get { return lastClickedControl; }
	}

	/// <summary>
	/// Sets the item at the specified index as the
	/// currently selected item.
	/// </summary>
	/// <param name="index">The zero-based index of the item.</param>
	public void SetSelectedItem(int index)
	{
		IUIListObject oldSel = selectedItem;

		if (index < 0 || index >= items.Count)
		{
			// Unset the previous selection:
			if (selectedItem != null)
				selectedItem.selected = false;

			selectedItem = null;

			// If the selected item changed,
			// notify our delegate:
			if (oldSel != selectedItem)
			{
				if (changeDelegate != null)
					changeDelegate(this);
			}

			return;
		}

		IUIListObject item = items[index];

		// Unset the previous selection:
		if (selectedItem != null)
			selectedItem.selected = false;

		selectedItem = item;
		item.selected = true;

		// If the selected item changed,
		// notify our delegate:
		if (oldSel != selectedItem)
		{
			if (changeDelegate != null)
				changeDelegate(this);
		}
	}

	/// <summary>
	/// Returns the number of items currently
	/// in the list.
	/// </summary>
	public int Count
	{
		get { return items.Count; }
	}

	/// <summary>
	/// Returns a reference to the specified list item.
	/// </summary>
	/// <param name="index">Index of the item to retrieve.</param>
	/// <returns>Reference to the desired list item, null if index is out of range.</returns>
	public IUIListObject GetItem(int index)
	{
		if (index < 0 || index >= items.Count)
			return null;
		return items[index];
	}

	/// <summary>
	/// Removes the item at the specified index.
	/// Remaining items are repositioned to fill
	/// the gap.
	/// The removed item is destroyed if 'destroy'
	/// is true. Otherwise, it is deactivated.
	/// </summary>
	/// <param name="index">Index of the item to remove.</param>
	/// <param name="destroy">Whether or not to destroy the item being removed.</param>
	public void RemoveItem(int index, bool destroy)
	{
		RemoveItem(index, destroy, false);
	}

	/// <summary>
	/// Removes the item at the specified index.
	/// Remaining items are repositioned to fill
	/// the gap.
	/// The removed item is destroyed if 'destroy'
	/// is true. Otherwise, it is deactivated.
	/// </summary>
	/// <param name="index">Index of the item to remove.</param>
	/// <param name="destroy">Whether or not to destroy the item being removed.</param>
	/// <param name="doEasing">Indicates whether easing should be performed on the items which will move to fill the void created by removal of this item. Only applies if the item being removed is not at the end of the list.</param>
	public void RemoveItem(int index, bool destroy, bool doEasing)
	{
		if (index < 0 || index >= items.Count)
			return;

		if (index == items.Count - 1)
			doItemEasing = false;
		else
			doItemEasing = doEasing;

		doPosEasing = doEasing;

		// Remove the item from our container:
		if (container != null)
			container.RemoveChild(items[index].gameObject);

		// Unselect it, if necessary:
		if(selectedItem == items[index])
		{
			selectedItem = null;
			items[index].selected = false;
		}
		
		// Null out our last clicked control if this is it:
		if (lastClickedControl != null &&
			(lastClickedControl == items[index] || (lastClickedControl.Container != null && lastClickedControl.Container.Equals(items[index]))) )
			lastClickedControl = null;

		// Remove the item from our visible list, if it's there:
		visibleItems.Remove(items[index]);

		if (destroy)
		{
			items[index].Delete();
			Destroy(items[index].gameObject);
		}
		else
		{
			// Move to the root of the hierarchy:
			items[index].transform.parent = null;
			// Deactivate:
			items[index].gameObject.SetActiveRecursively(false);
		}

		items.RemoveAt(index);

		// Reposition our items:
		PositionItems();
	}

	/// <summary>
	/// Removes the specified item.
	/// Remaining items are repositioned to fill
	/// the gap.
	/// The removed item is destroyed if 'destroy'
	/// is true. Otherwise, it is deactivated.
	/// </summary>
	/// <param name="item">Reference to the item to be removed.</param>
	/// <param name="destroy">Whether or not to destroy the item being removed.</param>
	public void RemoveItem(IUIListObject item, bool destroy)
	{
		RemoveItem(item, destroy, false);
	}

	/// <summary>
	/// Removes the specified item.
	/// Remaining items are repositioned to fill
	/// the gap.
	/// The removed item is destroyed if 'destroy'
	/// is true. Otherwise, it is deactivated.
	/// </summary>
	/// <param name="item">Reference to the item to be removed.</param>
	/// <param name="destroy">Whether or not to destroy the item being removed.</param>
	/// <param name="doEasing">Indicates whether easing should be performed on the items which will move to fill the void created by removal of this item. Only applies if the item being removed is not at the end of the list.</param>
	public void RemoveItem(IUIListObject item, bool destroy, bool doEasing)
	{
		for(int i=0; i<items.Count; ++i)
		{
			if(items[i] == item)
			{
				RemoveItem(i, destroy, doEasing);
				return;
			}
		}
	}

	/// <summary>
	/// Empties the contents of the list entirely.
	/// Destroys the items if instructed, otherwise
	/// it just deactivates them.
	/// </summary>
	/// <param name="destroy">When true, the list items are actually destroyed. Otherwise, they are deactivated.</param>
	public void ClearList(bool destroy)
	{
		RemoveItemsFromContainer();

		selectedItem = null;
		lastClickedControl = null;

		for(int i=0; i<items.Count; ++i)
		{
			// Move them out of the mover object
			// and into the root of the scene
			// hierarchy:
			items[i].transform.parent = null;

			if (destroy)
				Destroy(items[i].gameObject);
			else
				items[i].gameObject.SetActiveRecursively(false);
		}

		visibleItems.Clear();
		items.Clear();
		PositionItems();
	}


	//---------------------------------------------
	// Misc:
	//---------------------------------------------

	public void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
		{
			if(Container != null)
			{
				ptr.callerIsControl = true;
				Container.OnInput(ptr);
			}

			return;
		}


		// Do our own tap checking with the list's
		// own threshold:
		if (Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > (dragThreshold * dragThreshold))
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
		}
		else
			ptr.isTap = true;

		if (inputDelegate != null)
			inputDelegate(ref ptr);


		// Change the state if necessary:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				if (ptr.active)	// If this is a hold
					ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (!ptr.isTap)
					ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			case POINTER_INFO.INPUT_EVENT.TAP:
				PointerReleased();
				break;
		}

		// Apply any mousewheel scrolling:
		if(scrollWheelFactor != 0 && ptr.inputDelta.z != 0 && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			ScrollWheel(ptr.inputDelta.z);
		}

		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
	}


	public void LateUpdate()
	{
		// See if we have any items which were added
		// during this frame:
		if (newItems.Count != 0)
		{
			if (itemsInserted || doItemEasing)
				RepositionItems();
			else
				PositionNewItems();

			itemsInserted = false;
			newItems.Clear();
		}

		timeDelta = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
		inertiaLerpTime += timeDelta;

		// Update our clipping rect if we've moved:
		if (cachedPos != transform.position ||
			cachedRot != transform.rotation ||
			cachedScale != transform.lossyScale)
		{
			cachedPos = transform.position;
			cachedRot = transform.rotation;
			cachedScale = transform.lossyScale;
			CalcClippingRect();

			if (clipWhenMoving)
			{
				ClipItems();
			}
		}

		// Clip if we are easing our items:
		if (itemEasers.Count > 0)
			ClipItems();

		// Smooth our scroll inertia:
		if (!noTouch)
		{
			if (inertiaLerpTime >= inertiaLerpInterval)
			{
				// Accumulate inertia if we aren't coasting or auto-scrolling:
				scrollInertia = Mathf.Lerp(scrollInertia, scrollDelta, lowPassFilterFactor);
				scrollDelta = 0;
				inertiaLerpTime = inertiaLerpTime % inertiaLerpInterval;
			}
		}


		if (isScrolling && noTouch && !autoScrolling)
		{
			scrollDelta -= (scrollDelta * scrollDecelCoef);

			// See if we need to rebound from the edge:
			if (scrollPos < 0)
			{
				scrollPos -= scrollPos * reboundSpeed * (timeDelta / 0.166f);
				// Compute resistance:
				scrollDelta *= Mathf.Clamp01(1f + (scrollPos / scrollMax));
			}
			else if (scrollPos > 1f)
			{
				scrollPos -= (scrollPos - 1f) * reboundSpeed * (timeDelta / 0.166f);
				// Compute resistance:
				scrollDelta *= Mathf.Clamp01(1f - (scrollPos - 1f) / scrollMax);
			}

			if (Mathf.Abs(scrollDelta) < 0.0001f)
			{
				scrollDelta = 0;
				if (scrollPos > -0.0001f && scrollPos < 0.0001f)
					scrollPos = Mathf.Clamp01(scrollPos);
			}

			ScrollListTo_Internal(scrollPos + scrollDelta);

			if ((scrollPos >= 0 && scrollPos <= 1.001f && scrollDelta == 0))
				isScrolling = false;
		}
		else if (autoScrolling)
		{
			// Auto-scroll:
			autoScrollTime += timeDelta;

			if (autoScrollTime >= autoScrollDuration)
			{
				autoScrolling = false;
				scrollPos = autoScrollPos;
			}
			else
				scrollPos = autoScrollInterpolator(autoScrollTime, autoScrollStart, autoScrollDelta, autoScrollDuration);

			ScrollListTo_Internal(scrollPos);
		}
	}

	// Calculates the item to which we should snap.
	// If we have an item to which we should snap,
	// true is returned, indicating that we are snapping.
	// Otherwise if our scroll inertia will carry us
	// to the edge or beyond, false is returned
	// indicating that we are not snapping.
	protected void CalcSnapItem()
	{
		float nearestDist = 99999999f;
		float tempDist;
		IUIListObject nearestItem = null;
		IUIListObject tempItem = null;
		int searchDirection = 1;
		float endPos;
		float scrollTime;
		
		if (items.Count < 1)
			return;

		// Since we're going to land somewhere in the midst
		// of the list, find out when, where, and find the nearest
		// item to that position

		if (Mathf.Approximately(scrollDelta, 0))
		{
			scrollTime = minSnapDuration;
			endPos = scrollPos;
		}
		else
		{
			endPos = scrollPos + scrollDelta / scrollDecelCoef;

			// Calculate how long the scroll would take:
			float d = Mathf.Abs(scrollDelta);
			scrollTime = Time.fixedDeltaTime * (scrollStopThresholdLog - Mathf.Log10(d)) / Mathf.Log10((d - d * scrollDecelCoef) / d);
			scrollTime = Mathf.Max(scrollTime, minSnapDuration);
		}

		if (endPos >= 1f || endPos <= 0)
		{
			if (endPos <= 0)
				ScrollToItem(0, scrollTime);
			else
				ScrollToItem(items.Count - 1, scrollTime);
			return;
		}

		// Start looking at our approximate scroll position:
		int index = (int)Mathf.Clamp((((float)(items.Count - 1)) * endPos), 0, items.Count-1);

		if(orientation == ORIENTATION.HORIZONTAL)
		{
			float sign = (direction == DIRECTION.TtoB_LtoR) ? -1f : 1f;

			// Get the distance from our endPos to this item:
			nearestItem = items[index];
			nearestDist = Mathf.Abs(endPos + (sign * nearestItem.transform.localPosition.x) / amtOfPlay);

			// Now see which direction we should search for
			// an even closer item:
			if (index + searchDirection < items.Count)
			{
				tempItem = items[index + searchDirection];
				tempDist = Mathf.Abs(endPos + (sign * tempItem.transform.localPosition.x) / amtOfPlay);

				if (tempDist < nearestDist)
				{
					nearestDist = tempDist;
					nearestItem = tempItem;
					index += searchDirection;
				}
				else
					searchDirection = -1;
			}
			else
				searchDirection = -1;

			// Now search for a nearer item:
			for (int i = index + searchDirection; i > -1 && i < items.Count; i += searchDirection)
			{
				tempDist = Mathf.Abs(endPos + (sign * items[i].transform.localPosition.x) / amtOfPlay);

				if (tempDist < nearestDist)
				{
					nearestDist = tempDist;
					nearestItem = items[i];
				}
				else
					break;	// We've passed our nearest item
			}

			ScrollToItem(nearestItem, scrollTime);
		}
		else
		{
			float sign = (direction == DIRECTION.TtoB_LtoR) ? 1f : -1f;

			// Get the distance from our endPos to this item:
			nearestItem = items[index];
			nearestDist = Mathf.Abs(endPos + (sign * nearestItem.transform.localPosition.y) / amtOfPlay);

			// Now see which direction we should search for
			// an even closer item:
			if (index + searchDirection < items.Count)
			{
				tempItem = items[index + searchDirection];
				tempDist = Mathf.Abs(endPos + (sign * tempItem.transform.localPosition.y) / amtOfPlay);

				if (tempDist < nearestDist)
				{
					nearestDist = tempDist;
					nearestItem = tempItem;
					index += searchDirection;
				}
				else
					searchDirection = -1;
			}
			else
				searchDirection = -1;

			// Now search for a nearer item:
			for (int i = index + searchDirection; i > -1 && i < items.Count; i += searchDirection)
			{
				tempDist = Mathf.Abs(endPos + (sign * items[i].transform.localPosition.y) / amtOfPlay);

				if (tempDist < nearestDist)
				{
					nearestDist = tempDist;
					nearestItem = items[i];
				}
				else
					break;	// We've passed our nearest item
			}

			ScrollToItem(nearestItem, scrollTime);
		}
	}


	// Adds all of the control's items to its container.
	protected void AddItemsToContainer()
	{
		if (container == null)
			return;

		for(int i=0; i<items.Count; ++i)
		{
			container.AddChild(items[i].gameObject);
		}
	}

	// Removes all of the control's items from its container/
	protected void RemoveItemsFromContainer()
	{
		if (container == null)
			return;

		for (int i = 0; i<items.Count; ++i)
		{
			container.RemoveChild(items[i].gameObject);
		}
	}


	public bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;

			for(int i=0; i<items.Count; ++i)
			{
				items[i].controlIsEnabled = value;
			}
		}
	}

	/// <summary>
	/// When set to true, the control will instruct any
	/// pointers which have it as their target to
	/// de-target them.  Use this if you are deactivating
	/// a control and want no input to go to it.
	/// It is strongly recommended NOT to use this feature
	/// on any control that appears in a scroll list, or
	/// else you may be unable to scroll past the edge of
	/// the list's viewable area.
	/// </summary>
	public virtual bool DetargetOnDisable
	{
		get { return DetargetOnDisable; }
		set { DetargetOnDisable = value; }
	}

	// Allows an object to act as a proxy for other
	// controls - i.e. a UIVirtualScreen
	// But in our case, just return ourselves since
	// we're not acting as a proxy
	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		return this;
	}

	public virtual IUIContainer Container
	{
		get { return container; }
		set
		{
			if (value != container)
			{
				if (container != null)
				{
					RemoveItemsFromContainer();
				}

				container = value;
				AddItemsToContainer();
			}
			else
				container = value;
		}
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

	public void SetInputDelegate(EZInputDelegate del)
	{
		inputDelegate = del;
	}

	public void AddInputDelegate(EZInputDelegate del)
	{
		inputDelegate += del;
	}

	public void RemoveInputDelegate(EZInputDelegate del)
	{
		inputDelegate -= del;
	}


	public void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = del;
	}

	public void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate += del;
	}

	public void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate -= del;
	}

	/// <summary>
	/// Adds a delegate to be called when the list snaps to an item.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddItemSnappedDelegate(ItemSnappedDelegate del)
	{
		itemSnappedDel += del;
	}

	/// <summary>
	/// Removes a delegate from the list of those to be called when
	/// the list snaps to an item.
	/// </summary>
	/// <param name="del">The delegate to remove.</param>
	public void RemoveItemSnappedDelegate(ItemSnappedDelegate del)
	{
		itemSnappedDel -= del;
	}


	#region Drag&Drop

	//---------------------------------------------------
	// Drag & Drop stuff
	//---------------------------------------------------

	public object Data
	{
		get { return null; }
		set {}
	}

	public bool IsDraggable
	{
		get { return false; }
		set {}
	}

	public LayerMask DropMask
	{
		get { return -1; }
		set {}
	}

	public float DragOffset
	{
		get { return 0; }
		set {}
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get { return EZAnimation.EASING_TYPE.Default; }
		set {}
	}

	public float CancelDragDuration
	{
		get { return 0; }
		set {}
	}

	public bool IsDragging
	{
		get { return false; }
		set {}
	}

	public GameObject DropTarget
	{
		get { return null; }
		set {}
	}

	public bool DropHandled
	{
		get { return false; }
		set {}
	}

	public void DragUpdatePosition(POINTER_INFO ptr) {}

	public void CancelDrag() {}

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
	public void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) {	}
	public void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)	{	}
	public void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)	{	}
	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate() { return null; }


	#endregion

	void OnDrawGizmosSelected()
	{
		Vector3 ul, ll, lr, ur;

		SetupCameraAndSizes();

		ul = (transform.position - transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * transform.lossyScale.y));
		ll = (transform.position - transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * transform.lossyScale.y));
		lr = (transform.position + transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * transform.lossyScale.y));
		ur = (transform.position + transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * transform.lossyScale.y));

		Gizmos.color = new Color(1f, 0, 0.5f, 1f);
		Gizmos.DrawLine(ul, ll);	// Left
		Gizmos.DrawLine(ll, lr);	// Bottom
		Gizmos.DrawLine(lr, ur);	// Right
		Gizmos.DrawLine(ur, ul);	// Top
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIScrollList Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIScrollList)go.AddComponent(typeof(UIScrollList));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIScrollList Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIScrollList)go.AddComponent(typeof(UIScrollList));
	}


	public void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvokeOnSelect);
	}
}
