//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


//#define INTOLERANT_GET_COMPONENT


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// Class that lets a surface act as a proxy for
/// controls elsewhere in the scene being rendered
/// to texture and mapped onto this surface.
/// It's like putting a virtual screen onto an object in
/// the scene, hence the name.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Virtual Screen")]
[System.Serializable]
[RequireComponent(typeof(MeshCollider))]
public class UIVirtualScreen : MonoBehaviour, IUIObject
{
	/// <summary>
	/// The camera that is rendering the controls for
	/// our "screen".
	/// </summary>
	public Camera screenCamera;

	/// <summary>
	/// Mask used to filter out what controls will are
	/// checked for input.
	/// </summary>
	public LayerMask layerMask = -1;

	/// <summary>
	/// The distance from the screen camera that
	/// input should "reach".
	/// </summary>
	public float rayDepth = float.PositiveInfinity;

	/// <summary>
	/// When set to true, the pointer information
	/// sent to the controls in the "screen" will
	/// be intercepted first and processed so that
	/// coordinates, etc, are correct in the context
	/// of the "screen".
	/// This is recommended if you are using
	/// scroll lists or any custom input delegates
	/// that need to have accurate input delta or
	/// coordinate information that is accurate
	/// relative to the screen camera.
	/// </summary>
	public bool processPointerInfo = true;

	/// <summary>
	/// This only needs to be assigned if the
	/// processPointInfo option is set to true.
	/// This points to the common parent object
	/// of all the controls being rendered to
	/// this "screen".
	/// </summary>
	public GameObject controlParent;

	/// <summary>
	/// This setting only applies if processPointerInfo is
	/// set to true.
	/// When enabled, the render texture will only be updated
	/// when the screen detects input to one of the controls
	/// on the screen.  Otherwise, the "screen camera" is
	/// deactivated until new input is detected.
	/// </summary>
	public bool onlyRenderWhenNeeded = false;

	/// <summary>
	/// This setting only applies if onlyRenderWhenNeeded
	/// is enabled.
	/// This setting determines how many seconds to wait after 
	/// the pointer has stopped interacting with a control
	/// before the screen camera is deactivated.
	/// A value of 0 waits until the next frame.
	/// </summary>
	public float renderTimeout = 0;


	protected List<IUIObject> controls = new List<IUIObject>();

	// Tracks whether we are planning on deactivating
	// the screen camera.
	protected bool shuttingDown = false;


	public virtual void Awake()
	{
		MeshCollider mc = (MeshCollider) GetComponent(typeof(MeshCollider));
		if (mc != null)
			mc.isTrigger = true;
		else
			Debug.LogError("The object \"" + name + "\" does not have the required MeshCollider attached.  Please add one, or else the screen functionality will not work.");

		if (screenCamera == null)
			screenCamera = Camera.main;

		// We do this in Awake() because we need our
		// input delegate to be the FIRST one registered
		// with each control (scroll bars, for example,
		// have theirs set to their knob's delegate in
		// their Start(), so we've got to beat the knobs
		// to it!
		if (processPointerInfo)
			SetupControls();
	}

	public virtual IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		if (onlyRenderWhenNeeded && screenCamera != null)
			screenCamera.gameObject.active = false;
	}


	// Find all the controls for which we're acting
	// as a proxy, and set input delegates
	protected void SetupControls()
	{
		// Unset any input delegates:
		for (int i = 0; i < controls.Count; ++i)
		{
			controls[i].RemoveInputDelegate(InputProcessor);
			controls[i].AddDragDropInternalDelegate(InputProcessor);
		}

		controls.Clear();

		if (controlParent == null)
			return;

		Component[] cs = controlParent.GetComponentsInChildren(typeof(IUIObject), true);

		// Add input delegates:
		IUIObject obj;
		for (int i = 0; i < cs.Length; ++i)
		{
			obj = (IUIObject)cs[i];
			controls.Add(obj);
			obj.AddInputDelegate(InputProcessor);
			obj.AddDragDropInternalDelegate(InputProcessor);
		}
	}

	/// <summary>
	/// Adds a control to be processed by the screen.
	/// </summary>
	/// <param name="obj">The control to be processed.</param>
	public void AddControl(IUIObject obj)
	{
		if (obj == null)
			return;

		controls.Add(obj);
		obj.AddInputDelegate(InputProcessor);
	}

	/// <summary>
	/// Removes a control from being processed by the screen.
	/// </summary>
	/// <param name="obj">The object to be removed.</param>
	public void RemoveControl(IUIObject obj)
	{
		controls.Remove(obj);
		obj.RemoveInputDelegate(InputProcessor);
	}

	/// <summary>
	/// Sets the camera used for this screen.
	/// </summary>
	/// <param name="cam">The camera used for the screen.</param>
	public void SetScreenCamera(Camera cam)
	{
		screenCamera = cam;
	}

	/// <summary>
	/// Sets the common parent object of the controls
	/// being rendered to the screen.
	/// Setting this will cause any newly created
	/// controls in the screen to get properly setup.
	/// </summary>
	/// <param name="go"></param>
	public void SetControlParent(GameObject go)
	{
		controlParent = go;
		if(processPointerInfo)
			SetupControls();
	}


	// Performs helpful processing of input for all controls
	// shown on the "screen" so that coordinates, etc, are
	// correct within the context of this artificial screen.
	protected void InputProcessor(ref POINTER_INFO ptr)
	{
		// Since input was received, cancel any shut-down.
		shuttingDown = false;
		StopAllCoroutines();

		// Determine the "pixel" coordinate on our "screen":
		ptr.devicePos = new Vector3(ptr.hitInfo.textureCoord.x * screenCamera.pixelWidth, ptr.hitInfo.textureCoord.y * screenCamera.pixelHeight, 0);

		// Find the input delta and previous ray:
		Vector3 prevPos = ptr.devicePos;
		RaycastHit hit;
		float deltaZ = ptr.inputDelta.z;
		if (ptr.prevRay.direction.sqrMagnitude > 0 && collider.Raycast(ptr.prevRay, out hit, ptr.rayDepth))
		{
			prevPos = new Vector3(hit.textureCoord.x * screenCamera.pixelWidth, hit.textureCoord.y * screenCamera.pixelHeight, 0);
			ptr.inputDelta = ptr.devicePos - prevPos;
		}
		else
			ptr.inputDelta = Vector3.zero;

		// Restore the Z component in case there was scrolling:
		ptr.inputDelta.z = deltaZ;

		ptr.ray = screenCamera.ScreenPointToRay(ptr.devicePos);
		ptr.prevRay = screenCamera.ScreenPointToRay(prevPos);

		ptr.camera = screenCamera;

		ptr.rayDepth = rayDepth;
		ptr.layerMask = layerMask;

		// Get updated hit info:
		Physics.Raycast(ptr.ray, out ptr.hitInfo, rayDepth, layerMask);

		// See if we need to snoop for a loss of input:
		if(onlyRenderWhenNeeded)
		{
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF ||
				ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
			{
				StartCoroutine(DeactivateScreenCam(renderTimeout));
			} // Else, check to see if a TAP occurred outside the control's area:
			else if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				Component c = (Component)ptr.targetObj;
				if (!c.collider.Raycast(ptr.ray, out hit, rayDepth))
					StartCoroutine(DeactivateScreenCam(renderTimeout));
			}
		}
	}


	protected IEnumerator DeactivateScreenCam(float timeout)
	{
		shuttingDown = true;

		if (renderTimeout == 0)
			yield return null;
		else
			yield return new WaitForSeconds(renderTimeout);

		// Now wait until the end of this frame to make sure we
		// gave fair chance to receive new input:
		yield return new WaitForEndOfFrame();

		// See if new input was received during the last frame
		// if we're still decided on shutting down:
		if (shuttingDown && screenCamera != null)
			screenCamera.gameObject.active = false;
	}

	/// <summary>
	/// Renders a single frame of the screen.
	/// </summary>
	public void RenderFrame()
	{
		if (screenCamera == null)
			return;
		
		// If the camera is already active, ignore:
		if (screenCamera.gameObject.active)
			return;

		screenCamera.gameObject.active = true;
		DeactivateScreenCam(0);
	}

	/// <summary>
	/// Forces the screen to continuously update
	/// </summary>
	public void ForceOn()
	{
		if (screenCamera == null)
			return;

		gameObject.active = true;
		screenCamera.gameObject.active = true;
		shuttingDown = false;
		StopAllCoroutines();
		onlyRenderWhenNeeded = false;
	}

	public bool controlIsEnabled
	{
	    get { return false; }
	    set { }
	}
	
	public bool DetargetOnDisable
	{
	    get { return false; }
	    set { }
	}
	
	// Allows an object to act as a proxy for other
	// controls - i.e. a UIVirtualScreen
	// But in our case, just return ourselves since
	// we're not acting as a proxy
	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		// Determine the "pixel" coordinate on our "screen":
		Vector2 inputPt = new Vector2(ptr.hitInfo.textureCoord.x * screenCamera.pixelWidth, ptr.hitInfo.textureCoord.y * screenCamera.pixelHeight);

		IUIObject tempObj = null;

		// See if the camera is inactive:
		bool wasInactive = !screenCamera.gameObject.active;

		if (wasInactive)
			screenCamera.gameObject.active = true;

		Ray inputRay = screenCamera.ScreenPointToRay(inputPt);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit, rayDepth, layerMask))
		{
#if INTOLERANT_GET_COMPONENT
			tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
			// Else, get the component in a tolerant way:
			tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif
		}

		if (onlyRenderWhenNeeded)
		{
			// If new input was received over an active control,
			// cancel any shut-down or re-activate our camera:
			if (tempObj != null && tempObj.controlIsEnabled)
			{
				shuttingDown = false;
				StopAllCoroutines();

				if (wasInactive)
					wasInactive = false; // Cancel re-setting to inactive
			}
		}

		if (wasInactive)
			screenCamera.gameObject.active = false;

		return tempObj;
	}

	protected IUIContainer container;

	public virtual IUIContainer Container
	{
		get { return container; }
		set	{ container = value;}
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform t = transform.parent;
		Transform c = ((Component)cont).transform;

		while (t != null)
		{
			if (t == c)
			{
				Container = cont;
				return true;
			}
			else if (t.gameObject.GetComponent("IUIContainer") != null)
				return false;

			t = t.parent;
		}

		// Never found *any* containers:
		return false;
	}
	
	public bool GotFocus()	{ return false; }
	
	public void OnInput(POINTER_INFO ptr) { }
	
	public void SetInputDelegate(EZInputDelegate del) { }
	
	public void AddInputDelegate(EZInputDelegate del) { }
	
	public void RemoveInputDelegate(EZInputDelegate del) { }
	
	public void SetValueChangedDelegate(EZValueChangedDelegate del) { }
	
	public void AddValueChangedDelegate(EZValueChangedDelegate del) { }
	
	public void RemoveValueChangedDelegate(EZValueChangedDelegate del) { }
	
	public object Data
	{
	    get { return default(object); }
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
	
	public float DragOffset
	{
	    get { return 0; }
	    set { }
	}
	
	public EZAnimation.EASING_TYPE CancelDragEasing
	{
	    get { return default(EZAnimation.EASING_TYPE); }
	    set { }
	}
	
	public float CancelDragDuration
	{
	    get { return 0; }
	    set { }
	}
	
	public void DragUpdatePosition(POINTER_INFO ptr) { }
	
	public void CancelDrag() { }
	
	public void OnEZDragDrop_Internal(EZDragDropParams parms) { }
	
	public void AddDragDropDelegate(EZDragDropDelegate del) { }
	
	public void RemoveDragDropDelegate(EZDragDropDelegate del) { }
	
	public void SetDragDropDelegate(EZDragDropDelegate del) { }

	// Setters for the internal drag drop handler delegate:
	public void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate() { return null; }
}