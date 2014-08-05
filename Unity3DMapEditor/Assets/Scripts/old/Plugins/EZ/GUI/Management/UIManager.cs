//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------




// Tells when set, UIManager will use a faster, but less tolerant
// method of attempting to get the IUIObject component attached 
// to the object that was clicked/touched:
// #define INTOLERANT_GET_COMPONENT

// When defined, mouse wheel scrolling is time-coherent
// (the value polled is scaled by realtimeDelta (equivalent to 
// Time.deltaTime, but not susceptible to Time.timeScale):
#define SCROLL_IS_TIME_COHERENT




using UnityEngine;
using System.Collections;


/// <remarks>
/// Tracks important information about the status of
/// a pointing device (mouse, finger, arbitrary ray)
/// </remarks>
public struct POINTER_INFO
{
	/// <remarks>
	/// Enum IDs of various input events
	/// </remarks>
	public enum INPUT_EVENT
	{
		/// <remarks>
		/// Nothing happened from the last time 
		/// we polled the device
		/// </remarks>
		NO_CHANGE,								// Nothing happened from the last time we polled the device

		/// <remarks>
		/// The device has been pressed (mouse 
		/// button down, screen touched, etc)
		/// </remarks>
		PRESS,									// The device has been pressed (mouse button down, screen touched, etc)

		/// <remarks>
		/// The device has been released within the
		/// control's area but was not a TAP
		/// because the touch traveled beyond the
		/// drag threshold.
		/// (mouse button up, touch ended, etc)
		/// </remarks>
		RELEASE,								// The device has been released but was not a tap (mouse button up, touch ended, etc)

		/// <remarks>
		/// The device has been released as a tap 
		/// (mouse button up, touch ended, etc)
		/// </remarks>
		TAP,									// The device has been released as a tap (mouse button up, touch ended, etc)

		/// <remarks>
		/// The device was moved (mouse moved 
		/// with no buttons down)
		/// </remarks>
		MOVE,									// The device was moved (mouse moved with no buttons down)

		/// <summary>
		/// The device has left the area of
		/// the control.
		/// </summary>
		MOVE_OFF,

		/// <summary>
		/// The device was released (mouse button
		/// was released/finger was let go, etc)
		/// outside of the control's area.
		/// </summary>
		RELEASE_OFF,

		/// <remarks>
		/// The device was moved while being pressed (mouse 
		/// moved with button down, touch was dragged, etc)
		/// </remarks>
		DRAG									// The device was moved while being pressed (mouse moved with button down, touch was dragged, etc)
	}

	public enum POINTER_TYPE
	{
		/// <summary>
		/// The pointer type is a mouse.
		/// </summary>
		MOUSE = 0x01,

		/// <summary>
		/// The pointer type is a touchpad touch.
		/// </summary>
		TOUCHPAD = 0x02,

		/// <summary>
		/// The pointer type is either mouse or
		/// touchpad.
		/// </summary>
		MOUSE_TOUCHPAD = 0x03,

		/// <summary>
		/// The pointer is a ray pointer.
		/// </summary>
		RAY = 0x04
	}

	/// <summary>
	/// The type of pointer this is.
	/// </summary>
	public POINTER_TYPE type;

	/// <summary>
	/// The camera with which this pointer was used.
	/// This indicates which camera generated the
	/// ray information.
	/// </summary>
	public Camera camera;

	/// <summary>
	/// ID of the pointer.
	/// </summary>
	public int id;

	/// <summary>
	/// ID of the current action.
	/// A new action ID is assigned
	/// each time the pointer goes
	/// "active" anew.
	/// </summary>
	public int actionID;

	/// <summary>
	/// The type of event the state of this pointer has generated
	/// </summary>
	public INPUT_EVENT evt;

	/// <summary>
	/// Struct that holds info about the raycast hit (if any)
	/// against a UI element.
	/// </summary>
	public RaycastHit hitInfo;

	/// <summary>
	/// A touch is currently active (finger is still on the pad, mouse button is held, etc).
	/// </summary>
	public bool active;

	/// <summary>
	/// Current position of the input device (mouse, finger, whatever).
	/// When using a mouse or touchpad, this is the screen position.
	/// When using a ray, this is the position of the pointer in world
	/// space as projected from the camera a distance specified by the
	/// UIManager's "Ray Depth" value.
	/// </summary>
	public Vector3 devicePos;

	/// <summary>
	/// Original position where the touch/click began.
	/// When using a mouse or touchpad, this is the screen position.
	/// When using a ray, this is the position of the pointer in world
	/// space as projected from the camera a distance specified by the
	/// UIManager's "Ray Depth" value.
	/// </summary>
	public Vector3 origPos;

	/// <summary>
	/// Change in the devicePos since the last polling.
	/// </summary>
	public Vector3 inputDelta;

	/// <summary>
	/// Gets set to false after a touch/click moves beyond the drag threshold.
	/// </summary>
	public bool isTap;

	/// <summary>
	/// The ray projecting into the world for this pointing device.
	/// </summary>
	public Ray ray;

	/// <summary>
	/// The ray from the previous polling.
	/// </summary>
	public Ray prevRay;

	/// <summary>
	/// Depth into the scene the ray is to be/was cast.
	/// </summary>
	public float rayDepth;

	/// <summary>
	/// The IUIObject that this pointer is affecting, if any.
	/// </summary>
	public IUIObject targetObj;

	/// <summary>
	/// The layer mask for this pointer.
	/// </summary>
	public int layerMask;

	/// <summary>
	/// Signals whether the caller that is sending this
	/// pointer info is a control or not.
	/// </summary>
	public bool callerIsControl;

	/// <summary>
	/// The time (based on Time.time) that the pointer
	/// went active.  This value is 0 when the pointer
	/// is not active.
	/// </summary>
	public float activeTime;


	public void Copy(POINTER_INFO ptr)
	{
		type = ptr.type;
		camera = ptr.camera;
		id = ptr.id;
		actionID = ptr.actionID;
		evt = ptr.evt;
		active = ptr.active;
		devicePos = ptr.devicePos;
		origPos = ptr.origPos;
		inputDelta = ptr.inputDelta;
		ray = ptr.ray;
		prevRay = ptr.prevRay;
		rayDepth = ptr.rayDepth;
		isTap = ptr.isTap;
		targetObj = ptr.targetObj;
		layerMask = ptr.layerMask;
		hitInfo = ptr.hitInfo;
		activeTime = ptr.activeTime;
	}

	// Copies just those members needed to 
	// be able to re-use the same polled 
	// input for different cameras.
	public void Reuse(POINTER_INFO ptr)
	{
		evt = ptr.evt;
		actionID = ptr.actionID;
		active = ptr.active;
		devicePos = ptr.devicePos;
		origPos = ptr.origPos;
		inputDelta = ptr.inputDelta;
		isTap = ptr.isTap;
		hitInfo = default(RaycastHit);
		activeTime = ptr.activeTime;
	}

	// Intended only to reset values to
	// allow the pointer to be reused for
	// a new event, so do not set things
	// like layerMask or rayDepth.
	public void Reset(int actID)
	{
		actionID = actID;
		evt = INPUT_EVENT.NO_CHANGE;
		active = false;
		devicePos = Vector3.zero;
		origPos = Vector3.zero;
		inputDelta = Vector3.zero;
		ray = default(Ray);
		prevRay = default(Ray);
		isTap = true;
		hitInfo = default(RaycastHit);
		activeTime = 0;
	}
}


public struct KEYBOARD_INFO
{
#if UNITY_IPHONE || UNITY_ANDROID
	#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
	public TouchScreenKeyboardType type;
	#else
	public iPhoneKeyboardType type;
	#endif
	public bool autoCorrect;
	public bool multiline;
	public bool secure;
	public bool alert;
	public bool hideInput;
#endif
	// The insertion point for the text
	public int insert;
}


/// <summary>
/// Contains settings for a camera in a
/// UIManager's list of cameras to use
/// when casting rays for the GUI.
/// </summary>
[System.Serializable]
public class EZCameraSettings
{
	/// <summary>
	/// A camera, through which input events will be
	/// cast into the scene.
	/// </summary>
	public Camera camera;

	/// <summary>
	/// Layer mask to use for input through this camera.
	/// </summary>
	public LayerMask mask = -1;

	/// <summary>
	/// The depth into the scene to cast input events
	/// into the scene through this camera.  Only modify 
	/// this value if you wish to limit the player's 
	/// "reach" into a 3D scene when using the mouse or 
	/// touchpad.
	/// </summary>
	public float rayDepth = Mathf.Infinity;
}


/// <remarks>
/// UIManager polls for input, and dispatches
/// input events to controls in the scene.
/// Exactly one UIManager object should be in
/// each scene that contains EZ GUI controls.
/// </remarks>
[AddComponentMenu("EZ GUI/Management/UI Manager")]
public class UIManager : MonoBehaviour
{
	//----------------------------------------------------------------
	// Singleton code
	//----------------------------------------------------------------
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static UIManager s_Instance = null;

	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static UIManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				UIManager tmpInst = FindObjectOfType(typeof(UIManager)) as UIManager;
				if (tmpInst != null)
					tmpInst.Awake();
				s_Instance = tmpInst;

				if (s_Instance == null && Application.isEditor)
					Debug.LogError("Could not locate a UIManager object. You have to have exactly one UIManager in the scene.");
			}

			return s_Instance;
		}
	}

	public static bool Exists()
	{
		if(s_Instance == null)
		{
			UIManager tmpInst = FindObjectOfType(typeof(UIManager)) as UIManager;
			if (tmpInst != null)
				tmpInst.Awake();
			s_Instance = tmpInst;
		}

		return s_Instance != null;
	}
	
	public void OnDestroy()
	{
		s_Instance = null;
	}
	//----------------------------------------------------------------
	// End Singleton code
	//----------------------------------------------------------------

	//----------------------------------------------------------------
	// Local enums
	//----------------------------------------------------------------
	/// <remarks>
	/// Identifiers for different types of pointing devices.
	/// </remarks>
	public enum POINTER_TYPE
	{
		/// <remarks>
		/// A mouse pointer
		/// </remarks>
		MOUSE,

		/// <remarks>
		/// Multi-touch touchpad (iPhone/iPod/iPad finger touches)
		/// </remarks>
		TOUCHPAD,

		/// <summary>
		/// Same as using TOUCHPAD when on-device, but supports
		/// both mouse and touchpad input in-editor.
		/// </summary>
		AUTO_TOUCHPAD,

		/// <remarks>
		/// A ray cast in world space
		/// </remarks>
		RAY,

		/// <remarks>
		/// Mouse pointer and ray supported simultaneously.
		/// </remarks>
		MOUSE_AND_RAY,

		/// <remarks>
		/// Touchpad and ray supported simultaneously.
		/// </remarks>
		TOUCHPAD_AND_RAY
	}

	/// <remarks>
	/// Type of ray active state
	/// </remarks>
	public enum RAY_ACTIVE_STATE
	{
		/// <summary>
		/// The ray is not active
		/// </summary>
		Inactive,

		/// <summary>
		/// The ray is active for a single frame only.
		/// </summary>
		Momentary,

		/// <summary>
		/// The ray is active until released.
		/// </summary>
		Constant
	}

	/// <remarks>
	/// How input that occurs outside the viewport
	/// should be treated.
	/// </remarks>
	public enum OUTSIDE_VIEWPORT
	{
		/// <summary>
		/// Process all input that occurs outside
		/// the game's viewport.
		/// </summary>
		Process_All,

		/// <summary>
		/// Ignore all input that occurs outside
		/// the game's viewport.  NOTE: This may
		/// lead to missing events, such as a
		/// pointer release that occurs outside
		/// the viewport.
		/// </summary>
		Ignore,

		/// <summary>
		/// If any pointer that moves outside the
		/// game's viewport has a current target
		/// object, that object is sent a MOVE_OFF
		/// or RELEASE_OFF (as appropriate) event.
		/// Otherwise, it is ignored.
		/// </summary>
		Move_Off
	}

	public struct NonUIHitInfo
	{
		public int ptrIndex;
		public int camIndex;

		public NonUIHitInfo(int pIndex, int cIndex)
		{
			ptrIndex = pIndex;
			camIndex = cIndex;
		}
	}

	// Delegate definition for pointer poller methods.
	public delegate void PointerPollerDelegate();

	// Delegate definition for receiving pointer info
	public delegate void PointerInfoDelegate(POINTER_INFO ptr);


	/// <summary>
	/// The type of pointing device to be used
	/// </summary>
	public POINTER_TYPE pointerType = POINTER_TYPE.AUTO_TOUCHPAD;

	/// <summary>
	/// The number of pixels a device must be dragged from its
	/// original click/touch location to cause the click/touch
	/// event not to be considered to be a "tap" (click) but
	/// rather just a drag action.
	/// </summary>
	public float dragThreshold = 8f;

	/// <summary>
	/// This is similar to dragThreshold except instead of a
	/// distance in pixels, it is the world-distance 
	/// between the endpoint of the ray (based on rayDepth)
	/// when the ray went active to the endpoint of the ray
	/// in successive frames.
	/// </summary>
	public float rayDragThreshold = 2f;

	/// <summary>
	/// Depth into the scene from the camera that a ray
	/// pointer should extend.
	/// </summary>
	public float rayDepth = Mathf.Infinity;

	/// <summary>
	/// Layer mask to use for casting the Ray pointer
	/// type into the scene.
	/// </summary>
	public LayerMask rayMask = -1;

	/// <summary>
	/// Sets whether the ray pointer (if any) should be
	/// used to set the keyboard focus, at the exclusion
	/// of the mouse/touchpad pointer(s).
	/// Set this to false if you want to use the touchpad
	/// or mouse to set the keyboard focus, and set it to
	/// true if you instead want to set the focus using
	/// the ray pointer.
	/// </summary>
	public bool focusWithRay = false;

	/// <summary>
	/// Name of the virtual axis that is used when the
	/// input ray is to be active.
	/// NOTE: See the "Input" section of the Unity
	/// User Guide. Set this value to an empty string
	/// ("") if you will not be using an axis setup in
	/// Player Settings->Input and instead will use
	/// a UIActionBtn or call RayActive(). Failure to
	/// do so will result in an exception being raised.
	/// </summary>
	public string actionAxis = "Fire1";

	/// <summary>
	/// Determines how input events that occur outside
	/// the game's viewport will be handled.  Defaults
	/// to Move_Off. <see cref="Move_Off"/>
	/// </summary>
	public OUTSIDE_VIEWPORT inputOutsideViewport = OUTSIDE_VIEWPORT.Move_Off;

	/// <summary>
	/// When set to true, a warning will be logged to
	/// the console whenever a non-UI object is hit by
	/// an EZ GUI input raycast (either from mouse,
	/// touchpad, or ray pointer types).
	/// </summary>
	public bool warnOnNonUiHits = true;

	/*
		/// <summary>
		/// Object whose transform's "forward" vector will
		/// be used as the source of the raycast.
		/// If this is unassigned, it will default to the
		/// UI camera.
		/// </summary>
		public GameObject raycastingObject;
	*/
	protected Transform raycastingTransform; // What we actually use at runtime

	/// <summary>
	/// The cameras to use for raycasts and the like
	/// for mouse and touchpad input.
	/// </summary>
	public EZCameraSettings[] uiCameras = new EZCameraSettings[1];

	/// <summary>
	/// The camera to use for casting the Ray
	/// pointer type.
	/// </summary>
	public Camera rayCamera;

	/// <summary>
	/// Input will not be processed while true.
	/// </summary>
	public bool blockInput = false;

	/// <summary>
	/// The default distance, in world units, a dragged control
	/// should be offset toward the camera so as to ensure it
	/// hovers above other objects and controls in the scene.
	/// </summary>
	public float defaultDragOffset = 1f;

	/// <summary>
	/// The default easing for when a drag and drop is canceled.
	/// </summary>
	public EZAnimation.EASING_TYPE cancelDragEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	/// <summary>
	/// The duration of a canceled drag's easing animation.
	/// </summary>
	public float cancelDragDuration = 1f;

	/// <summary>
	/// The default font definition to use for control text.
	/// </summary>
	public TextAsset defaultFont;

	/// <summary>
	/// The material to use for the default font.
	/// </summary>
	public Material defaultFontMaterial;

#if UNITY_IPHONE || UNITY_ANDROID
	/// <summary>
	/// Auto rotate the keyboard to portrait orientation?
	/// </summary>
	public bool autoRotateKeyboardPortrait = true;

	/// <summary>
	/// Auto rotate the keyboard to portrait upside-down orientation?
	/// </summary>
	public bool autoRotateKeyboardPortraitUpsideDown = true;

	/// <summary>
	/// Auto rotate the keyboard to landscape left orientation?
	/// </summary>
	public bool autoRotateKeyboardLandscapeLeft = true;

	/// <summary>
	/// Auto rotate the keyboard to landscape right orientation?
	/// </summary>
	public bool autoRotateKeyboardLandscapeRight = true;
#endif

	// Holds whether our ray is active or not.
	protected bool rayActive = false;
	protected RAY_ACTIVE_STATE rayState;

	// Pointer-related stuff:
	protected POINTER_INFO[,] pointers;			// Used to track the status of pointer devices (mouse pointer, finger touches, etc) (one array for each mouse/touchpad camera in use)
	protected NonUIHitInfo[] nonUIHits;					// Holds a list of indices of all pointers hitting something other than a UI element for the current frame.
	protected bool[] usedPointers;				// Used to track which pointers have been used by a camera already (have already hit a control)
	protected bool[] usedNonUIHits;				// Same as usedPointers, but tracks which pointers are being "used" for non-UI hits, so we don't register multiple non-UI hits with the same pointer through different cameras.
	protected bool rayIsNonUIHit;				// Tracks whether the RAY pointer is hitting a non-UI object
	protected int numPointers;					// The number of pointers we have elements for in the "pointers" arrays (one for each camera).
	protected int numTouchPointers;				// The number of pointers which are part of our touchpad input detection (excludes any mouse or ray pointer)
	protected int[] activePointers;				// Indices of active pointers (same for all cameras)
	protected int numActivePointers;			// Holds the number of active pointers (this minus 1 indicates the index of the last valid index in activePointers)
	protected int numNonUIHits;					// Holds the number of pointers hitting something other than the UI for the current frame.
	protected POINTER_INFO rayPtr;				// The ray pointer
	protected PointerPollerDelegate pointerPoller; // Delegate we'll call to poll the pointing device(s).
	protected PointerInfoDelegate informNonUIHit; // Delegate to call when we have a non-UI raycast hit
	protected PointerInfoDelegate mouseTouchListeners; // Delegate to call with all mouse/touchpad input
	protected PointerInfoDelegate rayListeners;	// Delegate to call with all ray input

	// Keyboard stuff:
	protected IUIObject focusObj;				// The object that has the keyboard focus
	protected string controlText;				// Text of the focusObj.
	protected int insert;						// The index of the insertion point for the focused control.
	KEYBOARD_INFO kbInfo = default(KEYBOARD_INFO);// Used to retrieve information on the keyboard.

	// Misc.:
	protected int inputLockCount;				// Keeps track of how many objects have locked input.  This is used to prevent input during panel transitions.
	protected bool m_started = false;			// Lets us detect whether Start() has yet been called.
	protected bool m_awake = false;				// Lets us detect whether Awake() has yet been called.

	// Working vars:
#if SCROLL_IS_TIME_COHERENT
	float time, startTime, realtimeDelta;
#endif
	int lastUpdateFrame = 0;
	int curActionID = 0;
	int numTouches;
	protected RaycastHit hit;					// Used to perform raycasts
	protected Vector3 tempVec;
	bool down;
	IUIObject tempObj;
	POINTER_INFO tempPtr;
	System.Text.StringBuilder sb = new System.Text.StringBuilder();
#if UNITY_IPHONE || UNITY_ANDROID
	#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
	TouchScreenKeyboard iKeyboard;
	#else
	iPhoneKeyboard iKeyboard;
	#endif
#endif


	void Awake()
	{
		if (m_awake)
			return;
		m_awake = true;

		// See if we are a superfluous instance:
		if (s_Instance != null)
		{
			Debug.LogError("You can only have one instance of this singleton object in existence.");
		}
		else
			s_Instance = this;

#if (UNITY_IPHONE || UNITY_ANDROID)
		// See if we're supposed to auto-switch to TOUCHPAD:
		if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD)
		{
			if (!Application.isEditor)
				pointerType = POINTER_TYPE.TOUCHPAD;
		}
#endif

		// Determine how many touches should be supported:
		if (pointerType == POINTER_TYPE.TOUCHPAD || pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
#if UNITY_IPHONE || UNITY_ANDROID
	#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
			TouchScreenKeyboard.autorotateToPortrait = autoRotateKeyboardPortrait;
			TouchScreenKeyboard.autorotateToPortraitUpsideDown = autoRotateKeyboardPortraitUpsideDown;
			TouchScreenKeyboard.autorotateToLandscapeLeft = autoRotateKeyboardLandscapeLeft;
			TouchScreenKeyboard.autorotateToLandscapeRight = autoRotateKeyboardLandscapeRight;
	#else
			iPhoneKeyboard.autorotateToPortrait = autoRotateKeyboardPortrait;
			iPhoneKeyboard.autorotateToPortraitUpsideDown = autoRotateKeyboardPortraitUpsideDown;
			iPhoneKeyboard.autorotateToLandscapeLeft = autoRotateKeyboardLandscapeLeft;
			iPhoneKeyboard.autorotateToLandscapeRight = autoRotateKeyboardLandscapeRight;
	#endif

#if UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
			if (SystemInfo.deviceModel == "iPad")
#else
			if (iPhoneSettings.model == "iPad")
#endif
			{
#endif
			//				if (pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
			//					numTouches = 12;
			//				else
			numTouches = 11;
#if UNITY_IPHONE || UNITY_ANDROID
			}
			else
			{
// 				if (pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
// 					numTouches = 6;
// 				else
					numTouches = 5;
			}
#endif
		}
		else if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD)
			numTouches = 12;
		else if (pointerType == POINTER_TYPE.MOUSE_AND_RAY)
			numTouches = 1;
		else
			numTouches = 1;

		// Now figure the number of touchpad-based pointers:
		if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD ||
			pointerType == POINTER_TYPE.MOUSE ||
			pointerType == POINTER_TYPE.MOUSE_AND_RAY)
		{
			numTouchPointers = numTouches - 1;
		}
		else
			numTouchPointers = numTouches;

		// Get a reference to the camera:
		if (uiCameras.Length < 1)
		{
			uiCameras = new EZCameraSettings[1];
			uiCameras[0].camera = Camera.main;
		}
		else
		{
			for (int i = 0; i < uiCameras.Length; ++i)
				if (uiCameras[i].camera == null)
					uiCameras[i].camera = Camera.main;
		}

		if (rayCamera == null)
			rayCamera = uiCameras[0].camera;
	}

	void Start()
	{
		if (m_started)
			return;
		m_started = true;

		// Allocate our pointers, etc:
		numPointers = numTouches;
		activePointers = new int[numTouches];
		usedPointers = new bool[numPointers];

		nonUIHits = new NonUIHitInfo[numTouches];
		usedNonUIHits = new bool[numPointers];
		numNonUIHits = 0;

		SetupPointers();
	}

	protected void SetupPointers()
	{
		Start(); // Incase we've just been instantiated

		pointers = new POINTER_INFO[uiCameras.Length, numTouches];

		try
		{
			// Get our raycasting object:
			if (rayCamera == null)
				raycastingTransform = Camera.main.transform;
			else
				raycastingTransform = rayCamera.gameObject.transform;
		}
		catch
		{
			Debug.LogError("There appears to be no \"Main\" camera. Please tag one of your cameras with the \"MainCamera\" tag.");
		}

		switch (pointerType)
		{
			case POINTER_TYPE.MOUSE:
				pointerPoller = PollMouse;
				activePointers[0] = 0;
				numActivePointers = 1;
				for (int i = 0; i < uiCameras.Length; ++i)
				{
					pointers[i, 0].id = 0;
					pointers[i, 0].rayDepth = uiCameras[i].rayDepth;
					pointers[i, 0].layerMask = uiCameras[i].mask;
					pointers[i, 0].camera = uiCameras[i].camera;
					pointers[i, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
				}
				break;
			case POINTER_TYPE.TOUCHPAD:
				pointerPoller = PollTouchpad;
				for (int i = 0; i < uiCameras.Length; ++i)
					for (int j = 0; j < numPointers; ++j)
					{
						pointers[i, j].id = j;
						pointers[i, j].rayDepth = uiCameras[i].rayDepth;
						pointers[i, j].layerMask = uiCameras[i].mask;
						pointers[i, j].camera = uiCameras[i].camera;
						pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
					}
				break;
			case POINTER_TYPE.AUTO_TOUCHPAD:
				pointerPoller = PollMouseAndTouchpad;
				for (int i = 0; i < uiCameras.Length; ++i)
				{
					for (int j = 0; j < numPointers; ++j)
					{
						pointers[i, j].id = j;
						pointers[i, j].rayDepth = uiCameras[i].rayDepth;
						pointers[i, j].layerMask = uiCameras[i].mask;
						pointers[i, j].camera = uiCameras[i].camera;
						pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
					}
					pointers[i, numPointers - 1].type = POINTER_INFO.POINTER_TYPE.MOUSE;
				}
				break;
			case POINTER_TYPE.RAY:
				pointerPoller = PollRay;
				numActivePointers = 0;
				rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
				rayPtr.id = -1;
				rayPtr.rayDepth = rayDepth;
				rayPtr.layerMask = rayMask;
				rayPtr.camera = rayCamera;
				break;
			case POINTER_TYPE.MOUSE_AND_RAY:
				pointerPoller = PollMouseRay;
				activePointers[0] = 0;
				numActivePointers = 1;
				// 0 is the mouse:
				for (int i = 0; i < uiCameras.Length; ++i)
				{
					pointers[i, 0].id = 0;
					pointers[i, 0].rayDepth = uiCameras[i].rayDepth;
					pointers[i, 0].layerMask = uiCameras[i].mask;
					pointers[i, 0].camera = uiCameras[i].camera;
					pointers[i, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
				}

				rayPtr.id = -1;
				rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
				rayPtr.rayDepth = rayDepth;
				rayPtr.layerMask = rayMask;
				rayPtr.camera = rayCamera;
				break;
			case POINTER_TYPE.TOUCHPAD_AND_RAY:
				pointerPoller = PollTouchpadRay;
				for (int i = 0; i < uiCameras.Length; ++i)
					for (int j = 0; j < numPointers; ++j)
					{
						pointers[i, j].id = j;
						pointers[i, j].rayDepth = uiCameras[i].rayDepth;
						pointers[i, j].layerMask = uiCameras[i].mask;
						pointers[i, j].camera = uiCameras[i].camera;
						pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
					}

				rayPtr.id = -1;
				rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
				rayPtr.rayDepth = rayDepth;
				rayPtr.layerMask = rayMask;
				rayPtr.camera = rayCamera;
				break;
			default:
				Debug.LogError("ERROR: Invalid pointer type selected!");
				break;
		}
	}


	/// <summary>
	/// NOTE: Not recommended unless you intend to replace
	/// any and all other non-UI hit delegates.  Otherwise,
	/// use AddNonUIHitDelegate()<see cref="AddNonUIHitDelegate"/>
	/// This method sets the delegate to be called when a raycast is
	/// performed on an input event which hits a non-UI
	/// object.  Use this when your game uses raycasts for
	/// non-UI game purposes but you don't want to waste 
	/// performance with a redundant set of raycasts.
	/// IMPORTANT NOTE: This will replace any previously registered
	/// delegates.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void SetNonUIHitDelegate(PointerInfoDelegate del)
	{
		informNonUIHit = del;
	}

	/// <summary>
	/// Adds a delegate to be called when a raycast is
	/// performed on an input event which hits a non-UI
	/// object.  Use this when your game uses raycasts for
	/// non-UI game purposes but you don't want to waste 
	/// performance with a redundant set of raycasts.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddNonUIHitDelegate(PointerInfoDelegate del)
	{
		informNonUIHit += del;
	}

	/// <summary>
	/// Removes a delegate previously added with SetNonUIHitDelegate()
	/// or AddNonUIHitDelegate().
	/// </summary>
	/// <param name="del"></param>
	public void RemoveNonUIHitDelegate(PointerInfoDelegate del)
	{
		informNonUIHit -= del;
	}


	/// <summary>
	/// Registers a delegate to be called with all mouse 
	/// and touchpad pointer input.  Use this when you 
	/// want to "listen" to all mouse or touchpad input.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddMouseTouchPtrListener(PointerInfoDelegate del)
	{
		mouseTouchListeners += del;
	}


	/// <summary>
	/// Registers a delegate to be called with all ray 
	/// pointer input.  Use this when you want to "listen" 
	/// to all ray input.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddRayPtrListener(PointerInfoDelegate del)
	{
		rayListeners += del;
	}


	/// <summary>
	/// Removes a mouse/touchpad pointer listener.
	/// </summary>
	/// <param name="del">Delegate to be removed.</param>
	public void RemoveMouseTouchPtrListener(PointerInfoDelegate del)
	{
		mouseTouchListeners -= del;
	}


	/// <summary>
	/// Removes a mouse/touchpad pointer listener.
	/// </summary>
	/// <param name="del">Delegate to be removed.</param>
	public void RemoveRayPtrListener(PointerInfoDelegate del)
	{
		rayListeners -= del;
	}


	protected void AddNonUIHit(int ptrIndex, int camIndex)
	{
		if (informNonUIHit == null)
			return;

		// See if this is a ray pointer:
		if (camIndex == -1)
		{
			rayIsNonUIHit = true;
			return;
		}

		// See if this pointer was used by another camera:
		if (usedPointers[ptrIndex])
			return;

		// See if this pointer already hit a non-UI object:
		if (usedNonUIHits[ptrIndex])
			return;

		// Mark that we've now used this pointer for a non-UI hit,
		// so don't use it again later for another camera:
		usedNonUIHits[ptrIndex] = true;

		nonUIHits[numNonUIHits] = new NonUIHitInfo(ptrIndex, camIndex);
		++numNonUIHits;
	}

	protected void CallNonUIHitDelegate()
	{
		if(informNonUIHit == null)
			return;

		NonUIHitInfo info;

		for (int i = 0; i < numNonUIHits; ++i)
		{
			info = nonUIHits[i];

			// Unset our flag:
			usedNonUIHits[info.ptrIndex] = false;

			if (usedPointers[info.ptrIndex])
				continue;

			informNonUIHit(pointers[info.camIndex, info.ptrIndex]);
		}

		// See if we also have a RAY pointer non-UI hit:
		if (rayIsNonUIHit)
			informNonUIHit(rayPtr);
	}

	/// <summary>
	/// Returns whether the specified pointer (designated by ID)
	/// hit a UI element during the current frame.
	/// </summary>
	/// <param name="id">ID of the pointer. NOTE: On touchpad devices, this value should correspond to the fingerID of the touch.
	/// When using a MOUSE pointer type, the mouse's ID is 0.
	/// When using any of the pointer types that include the RAY type, the ray ID is -1.
	/// When using MOUSE with touchpad in-editor, the mouse ID is equal to the max supported touchpad touches. This is usually 11 touches, so the mouse ID in such a situation is normally 11.</param>
	/// <returns>True if the pointer hit a UI element, false otherwise.</returns>
	public bool DidPointerHitUI(int id)
	{
		// Make sure our information is up-to-date for this frame:
		if (lastUpdateFrame != Time.frameCount)
			Update();

		if(id == -1)
			return (rayPtr.targetObj != null);

		Mathf.Clamp(id, 0, usedPointers.Length - 1);
		return usedPointers[id];
	}

	/// <summary>
	/// Returns whether any pointer hit a UI element during the current frame.
	/// </summary>
	/// <returns>True if a pointer hit a UI element, false otherwise.</returns>
	public bool DidAnyPointerHitUI()
	{
		// Make sure our information is up-to-date for this frame:
		if (lastUpdateFrame != Time.frameCount)
			Update();

		if (rayPtr.targetObj != null)
			return true;

		for (int i = 0; i < usedPointers.Length; ++i)
			if (usedPointers[i])
				return true;

		return false;
	}


	/// <summary>
	/// Adds the specified camera to the UI Cameras list at the specified index.
	/// </summary>
	/// <param name="cam">The camera to be added.</param>
	/// <param name="mask">The layer mask for the camera.</param>
	/// <param name="depth">The depth into the scene the pointer should reach.</param>
	/// <param name="index">The index in the list where the camera should be added.
	/// Note that cameras higher on the list (at a lower index value) process input before
	/// cameras later in the list.</param>
	public void AddCamera(Camera cam, LayerMask mask, float depth, int index)
	{
		EZCameraSettings[] cams = new EZCameraSettings[uiCameras.Length + 1];

		// Keep the index in a valid range:
		index = Mathf.Clamp(index, 0, uiCameras.Length + 1);

		for (int i = 0, src = 0; i < cams.Length; ++i)
        {
            if (i == index)
            {
                cams[i] = new EZCameraSettings();
                cams[i].camera = cam;
                cams[i].mask = mask;
                cams[i].rayDepth = depth;
            }
            else
            {
                cams[i] = uiCameras[src++];
            }
        }

		uiCameras = cams;

		SetupPointers();
	}


	/// <summary>
	/// Removes the camera at the specified index from the UI Cameras
	/// array.
	/// </summary>
	/// <param name="index">The index of the camera that should be removed.</param>
	public void RemoveCamera(int index)
	{
		EZCameraSettings[] cams = new EZCameraSettings[uiCameras.Length - 1];

		// Keep the index in a valid range:
		index = Mathf.Clamp(index, 0, uiCameras.Length);

		for (int dst = 0, src = 0; src < uiCameras.Length; src++)
		{
			if (src != index)
			{
				cams[dst] = uiCameras[src];
				dst++;
			}
		}

		uiCameras = cams;

		SetupPointers();
	}


	/// <summary>
	/// Replaces the camera at the specified index with the specified camera.
	/// </summary>
	/// <param name="index">The index of the camera to be replaced.</param>
	/// <param name="cam">The new camera that will replace the existing camera.</param>
	public void ReplaceCamera(int index, Camera cam)
	{
		// Keep the index in a valid range:
		index = Mathf.Clamp(index, 0, uiCameras.Length);

		uiCameras[index].camera = cam;

		SetupPointers();
	}


	public void OnLevelWasLoaded(int level)
	{
		// Check cameras:
		for (int i = 0; i < uiCameras.Length; ++i)
			if (uiCameras[i].camera == null)
				uiCameras[i].camera = Camera.main;

		if (rayCamera == null)
			rayCamera = Camera.main;

		// Check the focus object:
		if (focusObj == null)
			FocusObject = null;

		blockInput = false;
		inputLockCount = 0;
	}

	// Does all the stuff necessary to begin a new drag-and-drop operation:
	protected void BeginDrag(ref POINTER_INFO curPtr)
	{
		curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Begin, curPtr.targetObj, curPtr));
		curPtr.targetObj.DragUpdatePosition(curPtr);
	}

	// Does all the stuff necessary to update the status of a drag-and-drop operation:
	protected void DoDragUpdate(POINTER_INFO curPtr)
	{
		IUIObject dragObj = curPtr.targetObj;

// 		EZDragDropHelper.DragDrop_InternalDelegate del = dragObj.GetDragDropInternalDelegate();
// 		if (del != null)
// 			del(ref curPtr);
		dragObj.DragUpdatePosition(curPtr);

		// See what the object is over:
		RaycastHit[] hits = Physics.RaycastAll(curPtr.ray, curPtr.rayDepth, curPtr.layerMask & dragObj.DropMask);

		// See if we should check other cameras:
		if (hits.Length == 0 || (hits.Length == 1 && hits[0].transform == dragObj.transform))
		{
			POINTER_INFO ptr;
			for (int i=0; i < uiCameras.Length; ++i)
			{
				if (uiCameras[i].camera == curPtr.camera)
					continue;
				
				ptr = pointers[i, curPtr.id];
				hits = Physics.RaycastAll(ptr.ray, ptr.rayDepth, ptr.layerMask & dragObj.DropMask);

				if (hits.Length != 0 && !(hits.Length == 1 && hits[0].transform == dragObj.transform))
					break;
			}
		}

		RaycastHit nearest = default(RaycastHit);
		nearest.distance = float.PositiveInfinity;
		for (int i = 0; i < hits.Length; ++i)
		{
			if (hits[i].transform != dragObj.transform && hits[i].distance < nearest.distance)
			{
				nearest = hits[i];
			}
		}

		dragObj.DropTarget = nearest.transform ? nearest.transform.gameObject : null;

		// Respond to events:
		switch(curPtr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Update, dragObj, curPtr));
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
				curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Dropped, dragObj, curPtr));
				break;
		}
	}


	// Update is called once per frame
	public virtual void Update()
	{
#if SCROLL_IS_TIME_COHERENT
		// Realtime time tracking
		time = Time.realtimeSinceStartup;
		realtimeDelta = time - startTime;
		startTime = time;
		// END Realtime time tracking
#endif

		if (lastUpdateFrame != Time.frameCount)
			lastUpdateFrame = Time.frameCount;
		else
			return; // We've already Updated for this frame

		// Poll for input:
		pointerPoller();

		// Poll the keyboard:
		if (focusObj != null)
			PollKeyboard();

		DispatchInput();
		/*
				if (Input.GetMouseButton(0))
				{
					if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask))
					{
						((UIView)views[curView]).OnTouch(hit.collider.gameObject, touchActive);
						touchActive = true;
					}
					else
					{
						// See if we have a focus object to send the event:
						if(focusObj != null)
						{

						}
						else
							touchActive = false;
					}
				}
				else
				{
					// Obviously no touch is active:
					touchActive = false;
				}
		*/
	}

	// Dispatches detected input to any hit UIObject
	protected void DispatchInput()
	{
		// Reset our non-UI hit tracking:
		numNonUIHits = 0;
		rayIsNonUIHit = false;

		// Unset our used flags:
		for (int i = 0; i < usedPointers.Length; ++i)
			usedPointers[i] = false;

		// Loop through each active pointer once for each camera
		// and dispatch input events:
		if (mouseTouchListeners != null)
		{
			for (int j = 0; j < numActivePointers; ++j)
				for (int i = 0; i < uiCameras.Length; ++i)
					if (uiCameras[i].camera.gameObject.active)
					{
						DispatchHelper(ref pointers[i, activePointers[j]], i);
						if (mouseTouchListeners != null)
							mouseTouchListeners(pointers[i, activePointers[j]]);

						// Only handle a pointer in a successive camera if
						// it didn't already get used by a previous camera:
						if (usedPointers[activePointers[j]])
							break; // Move to the next pointer
					}
		}
		else
		{
			for (int j = 0; j < numActivePointers; ++j)
				for (int i = 0; i < uiCameras.Length; ++i)
					if (uiCameras[i].camera.gameObject.active)
					{
						DispatchHelper(ref pointers[i, activePointers[j]], i);

						// Only handle a pointer in a successive camera if
						// it didn't already get used by a previous camera:
						if (usedPointers[activePointers[j]])
							break;
					}
		}

		// Dispatch for the ray:
		if (pointerType == POINTER_TYPE.RAY ||
			pointerType == POINTER_TYPE.MOUSE_AND_RAY ||
			pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
			DispatchHelper(ref rayPtr, -1);
			if (rayListeners != null)
				rayListeners(rayPtr);
		}
		
		// Call any non-UI hit delegate:
		CallNonUIHitDelegate();
	}

	protected void DispatchHelper(ref POINTER_INFO curPtr, int camIndex)
	{
// Only check for out-of-viewport input if such is possible
// (i.e. if we aren't on a hardware device where it is
// impossible to have input outside the viewport)
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID)
		// See if the input should be ignored:
		if(inputOutsideViewport != OUTSIDE_VIEWPORT.Process_All && curPtr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			// If the input is outside our viewport:
			if (!curPtr.camera.pixelRect.Contains(curPtr.devicePos))
			{
				// See how we should handle it:
				if (inputOutsideViewport == OUTSIDE_VIEWPORT.Ignore)
					return;

				// Otherwise, see if the pointer has a target:
				if (curPtr.targetObj == null)
					return; // Ignore new input

				tempPtr.Copy(curPtr);

				if (curPtr.active)
				{
					tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
					curPtr.targetObj.OnInput(tempPtr);
				}
				else
				{
					tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
					tempPtr.targetObj.OnInput(tempPtr);
				}

				// See if we need to cancel a drag-and-drop:
				if (curPtr.targetObj.IsDragging)
				{
					// Act like the object was dropped:
					curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Dropped, curPtr.targetObj, curPtr));
				}

				// Lose the target:
				curPtr.targetObj = null;
				return;
			}
		}
#endif
		
		// First see if this is an object being dragged:
		// See if we need to cancel a drag-and-drop:
		if (curPtr.targetObj != null && curPtr.targetObj.IsDragging)
		{
			DoDragUpdate(curPtr);
		}
		else
		{
			switch (curPtr.evt)
			{
				case POINTER_INFO.INPUT_EVENT.DRAG:
				case POINTER_INFO.INPUT_EVENT.RELEASE:
				case POINTER_INFO.INPUT_EVENT.TAP:
					// See if a tap or release took place outside of
					// the targetObj:
					if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE ||
						curPtr.evt == POINTER_INFO.INPUT_EVENT.TAP)
					{
						tempObj = null;

						if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
						{
#if INTOLERANT_GET_COMPONENT
							tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
							// Else, get the component in a tolerant way:
							tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif
							curPtr.hitInfo = hit;

							if(tempObj != null)
								tempObj = tempObj.GetControl(ref curPtr);

							// See if we hit a non-UI object:
							if (tempObj == null)
							{
								AddNonUIHit(curPtr.id, camIndex);
							}
						}
						else
							curPtr.hitInfo = default(RaycastHit);

						// If the hit object (or lack thereof) is not
						// the same as the current target:
						if (tempObj != curPtr.targetObj)
						{
							// If we have someone to notify of a release-off:
							if (curPtr.targetObj != null)
							{
								// Notify the targetObj of the event:
								tempPtr.Copy(curPtr);
								// Pass on a RELEASE_OFF if it was a RELEASE event,
								// else, pass on the TAP:
								if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
									tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
								else
									tempPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;

								curPtr.targetObj.OnInput(tempPtr);
							}

							// This pointer has now been used:
							if (curPtr.id >= 0)
								usedPointers[curPtr.id] = true;

							// Don't change the targetObj if input is blocked
							if (!blockInput)
								curPtr.targetObj = tempObj;

							// See if we need to notify our new target:
							if (tempObj != null)
							{
								// Only pass on a non-tap event
								// to a different target from the
								// one the pointer already had:
								if (curPtr.evt != POINTER_INFO.INPUT_EVENT.TAP && !blockInput)
									tempObj.OnInput(curPtr);
							}
						}
						else if (curPtr.targetObj != null)
						{
							// Tell our target about the event:
							curPtr.targetObj.OnInput(curPtr);

							// This pointer has now been used:
							if (curPtr.id >= 0)
								usedPointers[curPtr.id] = true;

							// In this case we don't care about whether input
							// is blocked because we want objects to still
							// get notified of a release since it is likely
							// that the control that is now being released
							// is being released from a press that initiated
							// whatever is now blocking input.
							// Plus, further input will not be processed from
							// this point until input is unblocked because
							// neither a move nor another press will be
							// processed.
						}

						// If this is a touchpad pointer, set the target to null since
						// it essentially no longer exists:
						if (curPtr.type == POINTER_INFO.POINTER_TYPE.TOUCHPAD)
							curPtr.targetObj = null;
					}
					else // Notify focus object:
					{
						if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
						{
							curPtr.hitInfo = hit;

							if (curPtr.targetObj == null)
								AddNonUIHit(curPtr.id, camIndex);
						}
						else
							curPtr.hitInfo = default(RaycastHit);

						if (curPtr.targetObj != null && !blockInput)
						{
							curPtr.targetObj.OnInput(curPtr);

							// Check to see if we should start a drag-and-drop:
							if (curPtr.targetObj.IsDraggable && !curPtr.isTap && curPtr.targetObj.controlIsEnabled)
								BeginDrag(ref curPtr);
						}
					}
					break;
				case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				case POINTER_INFO.INPUT_EVENT.MOVE:
					tempObj = null;
					// See if we need to notify anyone:
					if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
					{
#if INTOLERANT_GET_COMPONENT
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
						// Else, get the component in a tolerant way:
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif

						curPtr.hitInfo = hit;

						if (tempObj != null)
							tempObj = tempObj.GetControl(ref curPtr);

						// Check for a non-UI hit:
						if (tempObj == null)
						{
							// Let any listener know the UI was not hit
							AddNonUIHit(curPtr.id, camIndex);
							if (warnOnNonUiHits)
								LogNonUIObjErr(hit.collider.gameObject);
						}

						// If the mouse/touch isn't being held, then
						// see if we're over a new object since even
						// if this was a NO_CHANGE, the control could
						// have moved out from under a stationary
						// pointer.
						if (!curPtr.active)
						{
							if (curPtr.targetObj != tempObj)
								if (curPtr.targetObj != null)
								{
									tempPtr.Copy(curPtr);
									tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
									if (!blockInput)
										curPtr.targetObj.OnInput(tempPtr);
								}

							// Only update the pointer's targetObj
							// if the pointer wasn't held down and
							// if input isn't blocked:
							if (!blockInput)
							{
								curPtr.targetObj = tempObj;

								// Now dispatch this input to the new target,
								// if possible:
								if (tempObj != null)
									curPtr.targetObj.OnInput(curPtr);
							}
						}
						else // Else dispatch input to the target object:
						{
							if (curPtr.targetObj != null && !blockInput)
								curPtr.targetObj.OnInput(curPtr);
						}
					}
					else
					{
						curPtr.hitInfo = default(RaycastHit);

						// Notify the target object of the move-off:
						if (curPtr.targetObj != null && !curPtr.active)
						{
							curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
							curPtr.targetObj.OnInput(curPtr);
						}

						// If the pointer isn't active the target is now null:
						if (!curPtr.active)
						{
							curPtr.targetObj = null;
						}
					}
					break;
				case POINTER_INFO.INPUT_EVENT.PRESS:
					if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
					{
#if INTOLERANT_GET_COMPONENT
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
						// Else, get the component in a tolerant way:
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif

						curPtr.hitInfo = hit;

						if (tempObj != null)
							tempObj = tempObj.GetControl(ref curPtr);

						if (tempObj == null)
						{
							// Let any listener know the UI was not hit
							AddNonUIHit(curPtr.id, camIndex);
							if (warnOnNonUiHits)
								LogNonUIObjErr(hit.collider.gameObject);
						}

						// If this is different than the target obj:
						if (tempObj != curPtr.targetObj && curPtr.targetObj != null)
						{
							// Tell the target obj it's a move-off:
							tempPtr.Copy(curPtr);
							tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
							if (!blockInput)
								curPtr.targetObj.OnInput(tempPtr);
						}

						// Don't acquire a new targetObj if input is blocked
						if (!blockInput)
							curPtr.targetObj = tempObj;
						else
						{
							// If we clicked while input was blocked, lose the target
							if (curPtr.targetObj != null)
							{
								// Send an innocuous message that will reset any
								// control's state since we're blocking input but
								// need to make sure the control's state is reset:
								tempPtr.Copy(curPtr);
								tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
								curPtr.targetObj.OnInput(tempPtr);
							}
							curPtr.targetObj = null;
						}

						if (curPtr.targetObj != null)
						{
							if (!blockInput)
								curPtr.targetObj.OnInput(curPtr);

							// If this object doesn't already have focus
							// and this is a pointer capable of setting
							// the keyboard focus:
							if (curPtr.targetObj != focusObj &&
								((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY && focusWithRay) ||
								 (curPtr.type != POINTER_INFO.POINTER_TYPE.RAY)))
							{
								FocusObject = curPtr.targetObj;
							}
							break;
						}

						// Make sure this pointer is allowed to unset the focus:
						if ((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY) == focusWithRay)
						{
							// If we got here, the user clicked/tapped
							// somewhere other than on the focus obj:
							FocusObject = null;
						}
					}
					else
					{
						curPtr.hitInfo = default(RaycastHit);

						if (blockInput)
						{
							// If we clicked while input was blocked, lose the target
							if (curPtr.targetObj != null)
							{
								// Send an innocuous message that will reset any
								// control's state since we're blocking input but
								// need to make sure the control's state is reset:
								tempPtr.Copy(curPtr);
								tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
								curPtr.targetObj.OnInput(tempPtr);
							}
						}

						// Nothing was hit, so unset the focus object, if allowed:
						// Change the target even if input is blocked since we
						// don't want new PRESSes passing through to our existing
						// target object:
						//if (!blockInput)
						curPtr.targetObj = null;

						// Make sure this pointer is allowed to unset the focus:
						if ((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY) == focusWithRay)
						{
							FocusObject = null;
						}
					}
					break;
			}
		}

		// See if this pointer has been used:
		if (curPtr.targetObj != null)
		{
			if (curPtr.id >= 0)
				usedPointers[curPtr.id] = true;
		}	
	}

	// Polls the mouse pointing device
	protected void PollMouse()
	{
		PollMouse(ref pointers[0, 0]);
		// Make a copy of the pointer for
		// each camera:
		for (int i = 1; i < uiCameras.Length; ++i)
		{
			// See if the camera has been destroyed:
			if (uiCameras[i].camera == null)
			{
				RemoveCamera(i);
				continue;
			}

			if (!uiCameras[i].camera.gameObject.active)
				continue;
			pointers[i, 0].Reuse(pointers[0, 0]);
			pointers[i, 0].prevRay = pointers[i, 0].ray;
			pointers[i, 0].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, 0].devicePos);
		}
	}

	// Polls the mouse AND touchpad (Unity remote):
	protected void PollMouseAndTouchpad()
	{
// If we have a touchpad input device:
#if (UNITY_IPHONE || UNITY_ANDROID)
		
		PollTouchpad();

	// See if we might also have a mouse:
	#if UNITY_EDITOR
		numActivePointers += 1; // Add one for the mouse that is always active
	#endif

#else // Else, we only have a mouse:
		numActivePointers = 1; // Just the mouse
#endif

// If we have a mouse under any possible circumstances, poll it:
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID)
		int mouseIndex = numTouches - 1;

		activePointers[numActivePointers - 1] = mouseIndex;

		// Our mouse is the last pointer in the list:
		PollMouse(ref pointers[0, mouseIndex]);
		// Make a copy of the pointer for
		// each camera:
		for (int i = 1; i < uiCameras.Length; ++i)
		{
			// See if the camera has been destroyed:
			if (uiCameras[i].camera == null)
			{
				RemoveCamera(i);
				continue;
			}

			if (!uiCameras[i].camera.gameObject.active)
				continue;
			pointers[i, mouseIndex].Reuse(pointers[0, mouseIndex]);
			pointers[i, mouseIndex].prevRay = pointers[i, mouseIndex].ray;
			pointers[i, mouseIndex].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, mouseIndex].devicePos);
		}
#endif
	}

	// Polls the mouse pointing device
	protected void PollMouse(ref POINTER_INFO curPtr)
	{
		down = Input.GetMouseButton(0);

		// Check for scroll wheel:
		float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
#if SCROLL_IS_TIME_COHERENT
		scrollDelta *= realtimeDelta;
#endif
		bool scrolled = (scrollDelta != 0);

		// If the device is pressed and it 
		// was already pressed, it's a drag
		// or a repeat, depending on if there
		// was movement:
		if (down && curPtr.active)
		{
			// Drag:
			if (Input.mousePosition != curPtr.devicePos)
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
				curPtr.devicePos = Input.mousePosition;

				// See if we have exceeded the drag threshold:
				if (curPtr.isTap)
				{
					tempVec = curPtr.origPos - curPtr.devicePos;
					if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
						curPtr.isTap = false;
				}
			}
			else
			{
				// Nothing to see here:
				curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				curPtr.inputDelta = Vector3.zero;
			}
		}
		else if (down && !curPtr.active) // Else if it's a new press, it's a press
		{
			curPtr.Reset(curActionID++);
			curPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			curPtr.active = true;
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.origPos = Input.mousePosition;
			curPtr.isTap = true; // True for now, until it moves too much
			curPtr.activeTime = Time.time;
			curPtr.targetObj = null; // Make sure we start with null and let it be assigned later when we raycast
		}
		else if (!down && curPtr.active) // A release
		{
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.devicePos = Input.mousePosition;

			// See if we have exceeded the drag threshold:
			if (curPtr.isTap)
			{
				tempVec = curPtr.origPos - curPtr.devicePos;
				if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
					curPtr.isTap = false;
			}

			if (curPtr.isTap)
				curPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			else
				curPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;

			curPtr.active = false;
			curPtr.activeTime = 0;
		}
		else if (!down && Input.mousePosition != curPtr.devicePos) // Mouse was moved
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.devicePos = Input.mousePosition;
		}
		else
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			curPtr.inputDelta = Vector3.zero;
		}

		// Add any scroll delta in to the Z coordinate:
		if (scrolled)
		{
			curPtr.inputDelta.z = scrollDelta;
		}

		curPtr.devicePos = Input.mousePosition;
		curPtr.prevRay = curPtr.ray;
		curPtr.ray = uiCameras[0].camera.ScreenPointToRay(curPtr.devicePos);
	}


	// Polls the touchpad
	protected void PollTouchpad()
	{
#if UNITY_IPHONE || UNITY_ANDROID

#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		Touch touch;
#else
		iPhoneTouch touch;
#endif
		int id;

#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		numActivePointers = Mathf.Min(numTouches, Input.touchCount);
#else
		numActivePointers = Mathf.Min(numTouches, iPhoneInput.touchCount);
#endif

		// Process our touches:
		for(int i=0; i<numActivePointers; ++i)
		{
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			touch = Input.GetTouch(i);
#else
			touch = iPhoneInput.GetTouch(i);
#endif
			id = touch.fingerId;

			if(id >= numTouchPointers)
				id = numTouchPointers - 1;

			// Assign which pointer in our pointer 
			// array is active here:
			activePointers[i] = id;
			
			switch(touch.phase)
			{
				// Drag:
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				case TouchPhase.Moved:
#else
				case iPhoneTouchPhase.Moved:
#endif
					pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.DRAG;
					pointers[0, id].inputDelta = touch.deltaPosition;
					pointers[0, id].devicePos = touch.position;

					// See if we have exceeded the drag threshold:
					if (pointers[0, id].isTap)
					{
						tempVec = pointers[0, id].origPos - pointers[0, id].devicePos;
						if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
							pointers[0, id].isTap = false;
					}
					break;

				// Press:
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				case TouchPhase.Began:
#else
				case iPhoneTouchPhase.Began:
#endif
					pointers[0, id].Reset(curActionID++);
					pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.PRESS;
					pointers[0, id].active = true;
					pointers[0, id].inputDelta = Vector3.zero;
					pointers[0, id].origPos = touch.position;
					pointers[0, id].isTap = true; // True for now, until it moves too much
					pointers[0, id].activeTime = Time.time;
					pointers[0, id].targetObj = null; // Make sure we start with null and let it be assigned later when we raycast
					break;

				// Release
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
#else
				case iPhoneTouchPhase.Ended:
				case iPhoneTouchPhase.Canceled:
#endif
					if (pointers[0, id].isTap)
						pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.TAP;
					else
						pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.RELEASE;

					pointers[0, id].inputDelta = touch.deltaPosition;
					pointers[0, id].active = false;
					pointers[0, id].activeTime = 0;
					break;

				// No change:
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				case TouchPhase.Stationary:
#else
				case iPhoneTouchPhase.Stationary:
#endif
					pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
					pointers[0, id].inputDelta = Vector3.zero;
					break;
			}

			pointers[0, id].devicePos = touch.position;
			pointers[0, id].prevRay = pointers[0, id].ray;
			pointers[0, id].ray = uiCameras[0].camera.ScreenPointToRay(pointers[0, id].devicePos);
		}

		// Make a copy of the pointers for
		// each camera:
		for (int i = 1; i < uiCameras.Length; ++i)
			for (int j = 0; j < numActivePointers; ++j)
			{
				int ptrIdx = activePointers[j];
				pointers[i, ptrIdx].Reuse(pointers[0, ptrIdx]);
				pointers[i, ptrIdx].prevRay = pointers[i, ptrIdx].ray;
				pointers[i, ptrIdx].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, ptrIdx].devicePos);
			}
#endif
	}


	protected void PollRay()
	{
		if (actionAxis.Length != 0)
			rayActive = Input.GetButton(actionAxis);
		else
		{
			rayActive = rayState != RAY_ACTIVE_STATE.Inactive;

			// Deactivate a momentary action:
			if (rayState == RAY_ACTIVE_STATE.Momentary)
				rayState = RAY_ACTIVE_STATE.Inactive;
		}

		// If the device is pressed and it 
		// was already pressed, it's a drag
		// or a repeat, depending on if there
		// was movement:
		if (rayActive && rayPtr.active)
		{
			// Drag:
			if (raycastingTransform.forward != rayPtr.ray.direction ||
				raycastingTransform.position != rayPtr.ray.origin)
			{
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
				rayPtr.inputDelta = tempVec - rayPtr.devicePos;
				rayPtr.devicePos = tempVec;

				// See if we have exceeded the drag threshold:
				if (rayPtr.isTap)
				{
					tempVec = rayPtr.origPos - rayPtr.devicePos;
					if (tempVec.sqrMagnitude > (rayDragThreshold * rayDragThreshold))
						rayPtr.isTap = false;
				}
			}
			else
			{
				// Nothing to see here:
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				rayPtr.inputDelta = Vector3.zero;
			}
		}
		else if (rayActive && !rayPtr.active) // Else if it's a new press, it's a press
		{
			rayPtr.Reset(curActionID++);
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			rayPtr.active = true;
			rayPtr.origPos = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = rayPtr.origPos - rayPtr.devicePos;
			rayPtr.devicePos = rayPtr.origPos;
			rayPtr.isTap = true; // True for now, until it moves too much
			rayPtr.activeTime = Time.time;
			rayPtr.targetObj = null; // Make sure we start with null and let it be assigned later when we raycast
		}
		else if (!rayActive && rayPtr.active) // A release
		{
			if (rayPtr.isTap)
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			else
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;

			tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = tempVec - rayPtr.devicePos;
			rayPtr.devicePos = tempVec;
			rayPtr.active = false;
			rayPtr.activeTime = 0;
		}
		else if (!rayActive && Input.mousePosition != rayPtr.devicePos) // Mouse was moved
		{
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = tempVec - rayPtr.devicePos;
			rayPtr.devicePos = tempVec;
		}
		else
		{
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			rayPtr.inputDelta = Vector3.zero;
		}

		rayPtr.prevRay = rayPtr.ray;
		rayPtr.ray = new Ray(raycastingTransform.position, raycastingTransform.forward);
	}

	protected void PollMouseRay()
	{
		PollMouse();
		PollRay();
	}

	protected void PollTouchpadRay()
	{
		PollTouchpad();
		PollRay();
	}

	protected void PollKeyboard()
	{
#if UNITY_IPHONE || UNITY_ANDROID
		if(!Application.isEditor)
		{
			if(iKeyboard == null)
				return;
			
			if(iKeyboard.done || !iKeyboard.active)
			{
				controlText = iKeyboard.text;
				controlText = ((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
				((IKeyFocusable)focusObj).Commit();
				FocusObject = null;
				return;
			}
			else if(controlText == iKeyboard.text)
				return; // Nothing to do

			string oldText = controlText;

			controlText = iKeyboard.text;

			// Find the insertion point:
			insert = FindInsertionPoint(oldText, controlText);
			
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else
			ProcessKeyboard();
#else
		ProcessKeyboard();

		// Look for arrow keys:
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = Mathf.Min(controlText.Length, insert + 1);
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = Mathf.Max(0, insert - 1);
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else if (Input.GetKeyDown(KeyCode.Home))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = 0;
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else if (Input.GetKeyDown(KeyCode.End))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = controlText.Length;
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			((IKeyFocusable)focusObj).GoUp();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			((IKeyFocusable)focusObj).GoDown();
		}
#endif
	}

	protected void ProcessKeyboard()
	{
		if (Input.inputString.Length == 0 && !Input.GetKeyDown(KeyCode.Delete))
			return;

		// Start with the actual content of the focus object:
		controlText = ((IKeyFocusable)focusObj).Content;
		insert = Mathf.Clamp(insert, 0, controlText.Length);

		sb.Length = 0;
		sb.Append(controlText);

		foreach (char c in Input.inputString)
		{
			// Backspace
			if (c == '\b')
			{
				insert = Mathf.Max(0, insert - 1);
				if (insert < sb.Length)
					sb.Remove(insert, 1);
				continue;
			} // Delete

			sb.Insert(insert, c);
			++insert;
		}

		if (Input.GetKeyDown(KeyCode.Delete))
		{
			if (insert < sb.Length)
				sb.Remove(insert, 1);
		}

		controlText = sb.ToString();
		controlText = ((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
	}

	/// <summary>
	/// Used to set the active status of the ray.
	/// When set to Momentary or Constant, this is 
	/// analogous to having the mouse button down.
	/// </summary>
	public RAY_ACTIVE_STATE RayActive
	{
		get { return rayState; }
		set { rayState = value; }
	}

	/// <summary>
	/// Scans all active pointers and if any are
	/// found to be targeting the specified object,
	/// their targetObj field is set to null.
	/// </summary>
	/// <param name="obj">A reference to the object to be de-targeted.</param>
	public void Detarget(IUIObject obj)
	{
		Retarget(obj, null);
	}

	/// <summary>
	/// Detargets the specified pointer from any object it may
	/// be presently targetting.
	/// </summary>
	/// <param name="pointerID">The ID of the pointer to be detargetted.</param>
	public void Detarget(int pointerID)
	{
		if (uiCameras == null)
			return;

		for (int i = 0; i < uiCameras.Length; ++i)
			if (uiCameras[i].camera != null && uiCameras[i].camera.gameObject.active)
			{
				if (pointers[i, pointerID].targetObj != null)
				{
					// Gracefully release the current control:
					POINTER_INFO ptr = new POINTER_INFO();
					ptr.Copy(pointers[i, pointerID]);
					ptr.isTap = false;
					ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
					pointers[i, pointerID].targetObj.OnInput(ptr);

					pointers[i, pointerID].targetObj = null;
				}
			}
	}

	/// <summary>
	/// Detargets all pointers except the one with the specified ID.
	/// </summary>
	/// <param name="pointerID">The pointer to leave targetted.</param>
	public void DetargetAllExcept(int pointerID)
	{
		for (int j = 0; j < numActivePointers; ++j)
		{
			if (activePointers[j] != pointerID)
			{
				for (int i = 0; i < uiCameras.Length; ++i)
					if (uiCameras[i].camera != null && uiCameras[i].camera.gameObject.active)
					{
						if (pointers[i, activePointers[j]].targetObj != null)
						{
							// Gracefully release the current control:
							POINTER_INFO ptr = new POINTER_INFO();
							ptr.Copy(pointers[i, pointerID]);
							ptr.isTap = false;
							ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
							pointers[i, pointerID].targetObj.OnInput(ptr);

							pointers[i, activePointers[j]].targetObj = null;
						}
					}
			}
		}
	}

	/// <summary>
	/// Scans all active pointers and if any are
	/// found to be targeting the specified object,
	/// their targetObj field will instead be set
	/// to reference the object specified by newObj.
	/// </summary>
	/// <param name="obj">A reference to the object to be re-targeted.</param>
	/// <param name="newObj">The new object that should replace the old.</param>
	public void Retarget(IUIObject oldObj, IUIObject newObj)
	{
		if (uiCameras == null)
			return;

		for (int j = 0; j < numActivePointers; ++j)
		{
			for (int i = 0; i < uiCameras.Length; ++i)
				if (uiCameras[i].camera != null && uiCameras[i].camera.gameObject.active)
				{
					if (pointers[i, activePointers[j]].targetObj != null)
					{
						if (pointers[i, activePointers[j]].targetObj == oldObj)
						{
							pointers[i, activePointers[j]].targetObj = newObj;
						}
					}
				}
		}

		if (rayPtr.targetObj == oldObj)
			rayPtr.targetObj = newObj;
	}

	/// <summary>
	/// Attempts to retrieve the pointer that is currently targeting
	/// the specified object.
	/// </summary>
	/// <param name="obj">The object for which a targeting pointer is being sought.</param>
	/// <param name="ptr">A variable that will hold the POINTER_INFO of the pointer that is targeting the object, should one be found.</param>
	/// <returns>True if the ptr argument has been populated with the information of the targeting pointer, false if no targeting pointer was found.</returns>
	public bool GetPointer(IUIObject obj, out POINTER_INFO ptr)
	{
		if (uiCameras == null)
		{
			ptr = default(POINTER_INFO);
			return false;
		}

		for (int j = 0; j < numActivePointers; ++j)
		{
			for (int i = 0; i < uiCameras.Length; ++i)
				if (uiCameras[i].camera != null && uiCameras[i].camera.gameObject.active)
				{
					if (pointers[i, activePointers[j]].targetObj != null)
					{
						if (pointers[i, activePointers[j]].targetObj == obj)
						{
							ptr = pointers[i, activePointers[j]];
							return true; // Only one pointer can target an object at once, so we're done
						}
						else
							break; // Move to the next pointer since this one is used on this camera
					}
				}
		}

		if (rayPtr.targetObj == obj)
		{
			ptr = rayPtr;
			return true;
		}

		ptr = default(POINTER_INFO);
		return false;
	}

	/// <summary>
	/// Attempts to retrieve information on the pointer with the specified index.
	/// Valid information can only be returned for pointers which are currently
	/// active (i.e. a mouse pointer is always active, a touch pointer is only
	/// active when a touch exists).
	/// </summary>
	/// <param name="pointerID">The index of the pointer.</param>
	/// <param name="camera">The index of the camera for which you want the pointer information (the index is 0-based, as it appears in the UIManager's UI Cameras array).</param>
	/// <param name="ptr">A variable that will hold the POINTER_INFO of the pointer at the specified index.</param>
	/// <returns>True if the ptr argument has been populated with the information of the desired pointer, false if the specified pointer is inactive or does not exist.</returns>
	public bool GetPointer(int pointerID, int camera, out POINTER_INFO ptr)
	{
		if (uiCameras == null ||
			(camera < 0 || camera >= uiCameras.Length) || 
			(pointerID < 0 || pointerID >= numPointers) )
		{
			ptr = default(POINTER_INFO);
			return false;
		}

		for (int j = 0; j < numActivePointers; ++j)
		{
			if (activePointers[j] == pointerID)
			{
				ptr = pointers[camera, activePointers[j]];
				return true;
			}
		}

		ptr = default(POINTER_INFO);
		return false;
	}


	/// <summary>
	/// Adds a lock to the input.
	/// As long as there is at least
	/// one lock on input, input will
	/// not be processed.
	/// Use UnlockInput() to release
	/// a lock.  It is important to
	/// call UnlockInput() as many
	/// times as LockInput() for
	/// input processing to resume.
	/// </summary>
	public void LockInput()
	{
		blockInput = true;
		++inputLockCount;
	}

	/// <summary>
	/// Removes a lock on input.
	/// Input will not be processed
	/// until all locks placed by
	/// LockInput() have been removed.
	/// There must be as many calls to
	/// UnlockInput() as there were
	/// LockInput() for input processing
	/// to resume.
	/// </summary>
	public void UnlockInput()
	{
		--inputLockCount;
		if (inputLockCount < 1)
		{
			inputLockCount = 0; // Clamp to 0
			blockInput = false;
		}
	}

	/// <summary>
	/// Accessor for the object which has the
	/// keyboard focus.
	/// </summary>
	public IUIObject FocusObject
	{
		get { return focusObj; }
		set
		{
			// If the control accepts focus, use it,
			// otherwise, set to null:
			IUIObject val;
			if (null != value && value.GotFocus())
				val = value;
			else
				val = null;

			// See if another object is losing focus:
			if (focusObj != null && focusObj is IKeyFocusable)
				((IKeyFocusable)focusObj).LostFocus();

			focusObj = val;

            // 焦点丢失后重置输入法模式 [4/16/2012 SUN]
            if (val == null)
                Input.imeCompositionMode = IMECompositionMode.Auto;
			if (focusObj != null)
			{
                // unity3.5更新后无法输入中文，在这里强制打开输入法 [4/16/2012 SUN]
                Input.imeCompositionMode = IMECompositionMode.On;

				controlText = ((IKeyFocusable)focusObj).GetInputText(ref kbInfo);
				if (controlText == null)
					controlText = "";	// To be safe

#if UNITY_IPHONE || UNITY_ANDROID
				if(!Application.isEditor)
				{
	#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
					TouchScreenKeyboard.hideInput = kbInfo.hideInput;
					iKeyboard = TouchScreenKeyboard.Open(controlText, kbInfo.type, kbInfo.autoCorrect, kbInfo.multiline, kbInfo.secure, kbInfo.alert, controlText);
	#else
					iPhoneKeyboard.hideInput = kbInfo.hideInput;
					iKeyboard = iPhoneKeyboard.Open(controlText, kbInfo.type, kbInfo.autoCorrect, kbInfo.multiline, kbInfo.secure, kbInfo.alert, controlText);
	#endif
					iKeyboard.text = controlText;
				}
#endif
				insert = kbInfo.insert;

				if (sb.Length > 0)
					sb.Replace(sb.ToString(), controlText);
				else
					sb.Append(controlText);
			}
#if UNITY_IPHONE || UNITY_ANDROID
			else
			{
				if(iKeyboard != null)
				{
					iKeyboard.active = false;
					iKeyboard = null;
				}
			}
#endif
		}
	}

	/// <summary>
	/// Accessor for the text input insertion point.
	/// </summary>
	public int InsertionPoint
	{
		get { return insert; }
		set { insert = value; }
	}

	/// <summary>
	/// Returns the number of pointers currently supported.
	/// </summary>
	public int PointerCount
	{
		get { return numPointers; }
	}


	// Finds where the insertion point should be by comparing
	// changes from one string to another. The index after the
	// first change encountered is the insertion point.
	// This must be used when getting input from, for example,
	// the iOS keyboard, which doesn't tell us where the
	// insertion point currently is.
	protected static int FindInsertionPoint(string before, string after)
	{
		if (before == null || after == null)
			return 0;

		for (int i = 0; i < before.Length && i < after.Length; ++i)
			if (before[i] != after[i])
				return i + 1;

		// If we got this far, either the two are identical,
		// or one is longer/shorter than the other.
		// In which case, the insertion point should be
		// at the end of "after":
		return after.Length;
	}

	/////////////////////////////////////////////////////
	//	Error logging routines
	/////////////////////////////////////////////////////

	protected void LogNonUIObjErr(GameObject obj)
	{
		Debug.LogWarning("The UIManager encountered a collider on object \"" + obj.name + "\" that does not not contain an IUIObject or derivative component.  Please double-check that this object has the correct layer and components assigned.");
	}
}
