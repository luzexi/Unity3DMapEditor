//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <remarks>
/// A class that eases the process of placing objects
/// on-screen in a screen-relative, or object-relative
/// way, using pixels as units of distance.
/// </remarks>
[ExecuteInEditMode]
[System.Serializable]
[AddComponentMenu("EZ GUI/Utility/EZ Screen Placement")]
public class EZScreenPlacement : MonoBehaviour, IUseCamera
{
	/// <summary>
	/// Specifies what the object will be aligned relative to on the horizontal axis.
	/// </summary>
	public enum HORIZONTAL_ALIGN
	{
		/// <summary>
		/// The object will not be repositioned along the X axis.
		/// </summary>
		NONE,

		/// <summary>
		/// The X coordinate of screenPos will be interpreted as the number of pixels from the left edge of the screen.
		/// </summary>
		SCREEN_LEFT,

		/// <summary>
		/// The X coordinate of screenPos will be interpreted as the number of pixels from the right edge of the screen.
		/// </summary>
		SCREEN_RIGHT,

		/// <summary>
		/// The X coordinate of screenPos will be interpreted as the number of pixels from the center of the screen.
		/// </summary>
		SCREEN_CENTER,

		/// <summary>
		/// The X coordinate of screenPos will be interpreted as the number of pixels from the object assigned to horizontalObj. i.e. negative values will place this object to the left of horizontalObj, and positive values to the right.
		/// </summary>
		OBJECT
	}

	/// <summary>
	/// Specifies what the object will be aligned relative to on the vertical axis.
	/// </summary>
	public enum VERTICAL_ALIGN
	{
		/// <summary>
		/// The object will not be repositioned along the Y axis.
		/// </summary>
		NONE,

		/// <summary>
		/// The Y coordinate of screenPos will be interpreted as the number of pixels from the top edge of the screen.
		/// </summary>
		SCREEN_TOP,

		/// <summary>
		/// The Y coordinate of screenPos will be interpreted as the number of pixels from the bottom edge of the screen.
		/// </summary>
		SCREEN_BOTTOM,

		/// <summary>
		/// The Y coordinate of screenPos will be interpreted as the number of pixels from the center of the screen.
		/// </summary>
		SCREEN_CENTER,

		/// <summary>
		/// The Y coordinate of screenPos will be interpreted as the number of pixels from the object assigned to verticalObj. i.e. negative values will place this object above verticalObj, and positive values below.
		/// </summary>
		OBJECT
	}

	[System.Serializable]
	public class RelativeTo
	{
		public HORIZONTAL_ALIGN horizontal = HORIZONTAL_ALIGN.SCREEN_LEFT;
		public VERTICAL_ALIGN vertical = VERTICAL_ALIGN.SCREEN_TOP;

		// The script that contains this object
		protected EZScreenPlacement script;

		public EZScreenPlacement Script
		{
			get { return script; }
			set { Script = value; }
		}

		public bool Equals(RelativeTo rt)
		{
			if (rt == null)
				return false;
			return (horizontal == rt.horizontal && vertical == rt.vertical);
		}

		public void Copy(RelativeTo rt)
		{
			if (rt == null)
				return;
			horizontal = rt.horizontal;
			vertical = rt.vertical;
		}

		// Copy constructor
		public RelativeTo(EZScreenPlacement sp, RelativeTo rt)
		{
			script = sp;
			Copy(rt);
		}

		public RelativeTo(EZScreenPlacement sp)
		{
			script = sp;
		}
	}

	/// <summary>
	/// The camera with which this object should be positioned.
	/// </summary>
	public Camera renderCamera;

	/// <summary>
	/// The position of the object, relative to the screen or other object.
	/// </summary>
	public Vector3 screenPos = Vector3.forward;

	/// <summary>
	/// When set to true, the Z component of Screen Pos will be calculated
	/// based upon the distance of the object to the render camera, rather
	/// than controlling the distance to the render camera.  Enable this
	/// option, for example, when you want to preserve parent-relative
	/// positioning on the Z-axis.
	/// </summary>
	public bool ignoreZ = false;

	/// <summary>
	/// Settings object that describes what this object is positioned
	/// relative to.
	/// </summary>
	public RelativeTo relativeTo;

	/// <summary>
	/// The object to which this object is relative.
	/// NOTE: Only used if either the vertical or horizontal elements 
	/// of relativeTo are set to OBJECT (or both).
	/// </summary>
	public Transform relativeObject;

	/// <summary>
	/// When true, positioning of the object is always done in a recursive
	/// fashion.  That is, if this object is relative to any other objects,
	/// those objects, should they also hold an EZScreenPlacement component,
	/// will be positioned before this one.
	/// </summary>
	public bool alwaysRecursive = true;

	/// <summary>
	/// When checked, you can simply use the transform handles in the scene view
	/// to roughly position your object in the scene, and the appropriate
	/// values will be automatically calculated for Screen Pos based on your
	/// other settings.
	/// If you're having problems with slight rounding errors making small
	/// changes to your Screen Pos coordinate, you should probably uncheck
	/// this option.  This is particularly likely to happen if your camera
	/// is rotated at all.
	/// </summary>
	public bool allowTransformDrag = false;

	protected Vector2 screenSize;

#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9) && UNITY_EDITOR
	[System.NonSerialized]
	protected bool justEnabled = true;	// Helps us get around a silly issue that, sometimes when an object has OnEnabled() called, it may call SetCamera() on us in edit mode, and this happens in response to an OnGUI() call in-editor, meaning we'll get invalid camera information as a result, thereby positioning the object in the wrong place, so we need to detect this and just ignore any such SetCamera() call.
	[System.NonSerialized]
	protected EZScreenPlacementMirror mirror = new EZScreenPlacementMirror();
#else
	[System.NonSerialized]
	protected bool justEnabled = true;	// Helps us get around a silly issue that, sometimes when an object has OnEnabled() called, it may call SetCamera() on us in edit mode, and this happens in response to an OnGUI() call in-editor, meaning we'll get invalid camera information as a result, thereby positioning the object in the wrong place, so we need to detect this and just ignore any such SetCamera() call.
	[System.NonSerialized]
	protected EZScreenPlacementMirror mirror = new EZScreenPlacementMirror();
#endif

	protected bool m_awake = false;
	protected bool m_started = false;


	void Awake()
	{
		if (m_awake)
			return;
		m_awake = true;

		IUseCamera uc = (IUseCamera)GetComponent("IUseCamera");
		if (uc != null)
			renderCamera = uc.RenderCamera;

		if (renderCamera == null)
			renderCamera = Camera.main;

		if (relativeTo == null)
			relativeTo = new RelativeTo(this);
		else if (relativeTo.Script != this)
		{
			// This appears to be a duplicate object,
			// so create our own copy of the relativeTo:
			RelativeTo newRT = new RelativeTo(this, relativeTo);
			relativeTo = newRT;
		}
	}

	public void Start()
	{
		if (m_started)
			return;
		m_started = true;

		if (renderCamera != null)
		{
			screenSize.x = renderCamera.pixelWidth;
			screenSize.y = renderCamera.pixelHeight;
		}

#if UNITY_EDITOR
		if (relativeObject != null)
		{
			EZScreenPlacement c = relativeObject.GetComponent<EZScreenPlacement>();
			if (c != null)
				c.AddDependent(this);
		}

		justEnabled = false;
#endif

		PositionOnScreenRecursively();
	}

#if UNITY_EDITOR
	public void OnEnable()
	{
		justEnabled = true;
	}

	public void OnDestroy()
	{
		if(relativeObject != null)
		{
			EZScreenPlacement c = relativeObject.GetComponent<EZScreenPlacement>();
			if (c != null)
				c.RemoveDependent(this);
		}
	}
#endif

	/// <summary>
	/// Positions the object, taking into account any object-relative
	/// dependencies, making sure the objects to which this object is
	/// relative are correctly positioned before positioning this one.
	/// </summary>
	public void PositionOnScreenRecursively()
	{
		if (!m_started)
			Start();

		if (relativeObject != null)
		{
			EZScreenPlacement sp = relativeObject.GetComponent(typeof(EZScreenPlacement)) as EZScreenPlacement;
			if (sp != null)
				sp.PositionOnScreenRecursively();
		}
		
		
		PositionOnScreen();
	}

	/// <summary>
	/// Calculates a local position from a given screen or object-relative position,
	/// according to the current screen-space settings.
	/// </summary>
	/// <param name="screenPos">The screen/object-relative position</param>
	/// <returns>The corresponding position in local space.</returns>
	public Vector3 ScreenPosToLocalPos(Vector3 screenPos) 
	{
		return transform.InverseTransformPoint(ScreenPosToWorldPos(screenPos));
	}

	/// <summary>
	/// Calculates a in the parent's local space from a given screen or 
	/// object-relative position, according to the current screen-space settings.
	/// </summary>
	/// <param name="screenPos">The screen/object-relative position</param>
	/// <returns>The corresponding position in the parent object's local space.</returns>
	public Vector3 ScreenPosToParentPos(Vector3 screenPos)
	{
		return ScreenPosToLocalPos(screenPos) + transform.localPosition;
	}

    /// <summary>
	/// Calculates the world position from a given screen or object-relative position,
	/// according to the current screen-space settings.
    /// </summary>
    /// <param name="screenPos">The screen/object-relative position</param>
    /// <returns>The corresponding position in world space.</returns>
    public Vector3 ScreenPosToWorldPos(Vector3 screenPos) 
	{
		if (!m_started)
			Start();

		if (renderCamera == null)
		{
			Debug.LogError("Render camera not yet assigned to EZScreenPlacement component of \"" + name + "\" when attempting to call PositionOnScreen()");
			return transform.position;
		}

		Vector3 curPos = renderCamera.WorldToScreenPoint(transform.position);
		Vector3 pos = screenPos;

		switch (relativeTo.horizontal)
		{
			case HORIZONTAL_ALIGN.SCREEN_RIGHT:
				pos.x = screenSize.x + pos.x;
				break;
			case HORIZONTAL_ALIGN.SCREEN_CENTER:
				pos.x = screenSize.x * 0.5f + pos.x;
				break;
			case HORIZONTAL_ALIGN.OBJECT:
				if (relativeObject != null)
				{
					pos.x = renderCamera.WorldToScreenPoint(relativeObject.position).x + pos.x;
				}
				else
					pos.x = curPos.x;
				break;
			case HORIZONTAL_ALIGN.NONE:
				pos.x = curPos.x;
				break;
		}

		switch (relativeTo.vertical)
		{
			case VERTICAL_ALIGN.SCREEN_TOP:
				pos.y = screenSize.y + pos.y;
				break;
			case VERTICAL_ALIGN.SCREEN_CENTER:
				pos.y = screenSize.y * 0.5f + pos.y;
				break;
			case VERTICAL_ALIGN.OBJECT:
				if (relativeObject != null)
				{
					pos.y = renderCamera.WorldToScreenPoint(relativeObject.position).y + pos.y;
				}
				else
					pos.y = curPos.y;
				break;
			case VERTICAL_ALIGN.NONE:
				pos.y = curPos.y;
				break;
		}

		return renderCamera.ScreenToWorldPoint(pos);
	}


	/// <summary>
	/// Repositions the object using the existing screen-space settings.
	/// </summary>
	public void PositionOnScreen()
	{
		if (!m_awake)
			return;

		// Keep the 'z' updated in the inspector
		if (ignoreZ)
		{
			Plane plane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);
			screenPos.z = plane.GetDistanceToPoint(transform.position);
		}

		if (ignoreZ)
		{
			Vector3 pos = ScreenPosToWorldPos(screenPos);
			pos.z = transform.position.z;
			transform.position = pos;
		}
		else
			transform.position = ScreenPosToWorldPos(screenPos);

#if UNITY_EDITOR
		if(!Application.isPlaying)
			UpdateDependents();
#endif

		// Notify the object that it was repositioned:
		SendMessage("OnReposition", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Positions the object using screen coordinates, according to
	/// the relativity settings stored in relativeToScreen.
	/// </summary>
	/// <param name="x">The number of pixels in the X axis relative to the position specified in relativeToScreen.</param>
	/// <param name="y">The number of pixels in the Y axis relative to the position specified in relativeToScreen.</param>
	/// <param name="depth">The distance the object should be in front of the camera.</param>
	public void PositionOnScreen(int x, int y, float depth)
	{
		PositionOnScreen(new Vector3((float)x, (float)y, depth));
	}

	/// <summary>
	/// Positions the object using screen coordinates, according to
	/// the relativity settings stored in relativeToScreen.
	/// </summary>
	/// <param name="pos">The X and Y screen coordinates where the object should be positioned, as well as the Z coordinate which represents the distance in front of the camera.</param>
	public void PositionOnScreen(Vector3 pos)
	{
		screenPos = pos;
		PositionOnScreen();
	}


	/// <summary>
	/// Accessor for the camera that will be used to render this object.
	/// Use this to ensure the object is properly configured for the
	/// specific camera that will render it.
	/// </summary>
	public Camera RenderCamera
	{
		get { return renderCamera; }
		set { SetCamera(value); }
	}

	/// <summary>
	/// Updates the object's position based on the currently
	/// selected renderCamera.
	/// </summary>
	public void UpdateCamera()
	{
		SetCamera(renderCamera);
	}
	
	/// <summary>
	/// A no-argument version of SetCamera() that simply
	/// re-assigns the same camera to the object, forcing
	/// it to recalculate all camera-dependent calculations.
	/// </summary>
	public void SetCamera()
	{
		SetCamera(renderCamera);
	}


	/// <summary>
	/// Sets the camera to be used for calculating positions.
	/// </summary>
	/// <param name="c"></param>
	public void SetCamera(Camera c)
	{
		if (c == null || !enabled)
			return;
#if UNITY_EDITOR
		if (!Application.isPlaying && justEnabled)
			return;
#endif

		renderCamera = c;
		screenSize.x = renderCamera.pixelWidth;
		screenSize.y = renderCamera.pixelHeight;

		if (alwaysRecursive || (Application.isEditor && !Application.isPlaying))
			PositionOnScreenRecursively();
		else
			PositionOnScreen();
	}


	public void WorldToScreenPos(Vector3 worldPos)
	{
		if (renderCamera == null)
			return;

		Vector3 newPos = renderCamera.WorldToScreenPoint(worldPos);

		switch (relativeTo.horizontal)
		{
			case EZScreenPlacement.HORIZONTAL_ALIGN.SCREEN_CENTER:
				screenPos.x = newPos.x - (renderCamera.pixelWidth / 2f);
				break;
			case EZScreenPlacement.HORIZONTAL_ALIGN.SCREEN_LEFT:
				screenPos.x = newPos.x;
				break;
			case EZScreenPlacement.HORIZONTAL_ALIGN.SCREEN_RIGHT:
				screenPos.x = newPos.x - renderCamera.pixelWidth;
				break;
			case EZScreenPlacement.HORIZONTAL_ALIGN.OBJECT:
				if (relativeObject != null)
				{
					Vector3 objPos = renderCamera.WorldToScreenPoint(relativeObject.transform.position);
					screenPos.x = newPos.x - objPos.x;
				}
				break;
		}

		switch (relativeTo.vertical)
		{
			case EZScreenPlacement.VERTICAL_ALIGN.SCREEN_CENTER:
				screenPos.y = newPos.y - (renderCamera.pixelHeight / 2f);
				break;
			case EZScreenPlacement.VERTICAL_ALIGN.SCREEN_TOP:
				screenPos.y = newPos.y - renderCamera.pixelHeight;
				break;
			case EZScreenPlacement.VERTICAL_ALIGN.SCREEN_BOTTOM:
				screenPos.y = newPos.y;
				break;
			case EZScreenPlacement.VERTICAL_ALIGN.OBJECT:
				if (relativeObject != null)
				{
					Vector3 objPos = renderCamera.WorldToScreenPoint(relativeObject.transform.position);
					screenPos.y = newPos.y - objPos.y;
				}
				break;
		}

		screenPos.z = newPos.z;

		if (alwaysRecursive)
			PositionOnScreenRecursively();
		else
			PositionOnScreen();
	}


	/// <summary>
	/// Retrieves the screen coordinate of the object's current position.
	/// </summary>
	public Vector3 ScreenCoord
	{
		get { return renderCamera.WorldToScreenPoint(transform.position); }
	}


	// Tests dependencies for circular dependency.
	// Returns true if safe, false if circular.
	static public bool TestDepenency(EZScreenPlacement sp)
	{
		if (sp.relativeObject == null)
			return true;

		// Table of all objects in the chain of dependency:
		List<EZScreenPlacement> objs = new List<EZScreenPlacement>();

		objs.Add(sp);

		EZScreenPlacement curObj = sp.relativeObject.GetComponent(typeof(EZScreenPlacement)) as EZScreenPlacement;

		// Walk the chain:
		while (curObj != null)
		{
			if (objs.Contains(curObj))
				return false; // Circular!

			// Add this one to the list and keep walkin'
			objs.Add(curObj);

			// See if we're at the end of the chain:
			if (curObj.relativeObject == null)
				return true;

			// Get the next one:
			curObj = curObj.relativeObject.GetComponent(typeof(EZScreenPlacement)) as EZScreenPlacement;
		}

		return true;
	}


	// List of dependent objects.
	[HideInInspector]
	public EZScreenPlacement[] dependents = new EZScreenPlacement[0];

#if UNITY_EDITOR
	// Notify this object that it has a dependent object.
	public void AddDependent(EZScreenPlacement sp)
	{
		// Ensure the object isn't already on our list:
		foreach (EZScreenPlacement d in dependents)
			if(d == sp)
				return;
		
		List<EZScreenPlacement> temp = new List<EZScreenPlacement>();
		temp.AddRange(dependents);

		if(!temp.Contains(sp))
		{
			temp.Add(sp);
			dependents = temp.ToArray();
		}
	}

	// Notify the object that it has one fewer dependent.
	public void RemoveDependent(EZScreenPlacement sp)
	{
		List<EZScreenPlacement> temp = new List<EZScreenPlacement>();
		temp.AddRange(dependents);
		temp.Remove(sp);
		dependents = temp.ToArray();
	}
	
	// Cleans dependents from the list which are no longer valid:
	public void CleanDependents()
    {
		if(dependents == null)
			dependents = new EZScreenPlacement[0];

		List<EZScreenPlacement> temp = new List<EZScreenPlacement>();

		foreach (EZScreenPlacement sp in dependents)
		{
			if (sp != null)
			{
				temp.Add(sp);
			}
		}

		dependents = temp.ToArray();

		if(dependents == null)
			dependents = new EZScreenPlacement[0];
	}

	// Updates the positions of all dependent objects
	public void UpdateDependents()
	{
		CleanDependents();
		
		foreach (EZScreenPlacement sp in dependents)
			if(sp != null)
				sp.PositionOnScreen();
	}
#endif


	public virtual void DoMirror()
	{
		// Only run if we're not playing:
		if (Application.isPlaying)
			return;

		if (mirror == null)
		{
			mirror = new EZScreenPlacementMirror();
			mirror.Mirror(this);
		}

		mirror.Validate(this);

		// Compare our mirrored settings to the current settings
		// to see if something was changed:
		if (mirror.DidChange(this))
		{
			SetCamera(renderCamera);
			mirror.Mirror(this);	// Update the mirror
		}
	}

#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9) && UNITY_EDITOR
	void Update() 
	{
		DoMirror();
	}
#else
	// Ensures that the text is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmosSelected()
	{
		DoMirror();
	}

	// Ensures that the text is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmos()
	{
		DoMirror();
	}
#endif

}



// Used to automatically update an EZScreenPlacement object
// when its settings are modified in-editor.
public class EZScreenPlacementMirror
{
	public Vector3 worldPos;
	public Vector3 screenPos;
	public EZScreenPlacement.RelativeTo relativeTo;
	public Transform relativeObject;
	public Camera renderCamera;
	public Vector2 screenSize;

	public EZScreenPlacementMirror()
	{
		relativeTo = new EZScreenPlacement.RelativeTo(null);
	}

	public virtual void Mirror(EZScreenPlacement sp)
	{
		worldPos = sp.transform.position;
		screenPos = sp.screenPos;
		relativeTo.Copy(sp.relativeTo);
		relativeObject = sp.relativeObject;
		renderCamera = sp.renderCamera;
		screenSize = new Vector2(sp.renderCamera.pixelWidth, sp.renderCamera.pixelHeight);
	}

	public virtual bool Validate(EZScreenPlacement sp)
	{
		// Only allow assignment of a relative object IF
		// we intend to use it:
		if (sp.relativeTo.horizontal != EZScreenPlacement.HORIZONTAL_ALIGN.OBJECT &&
			sp.relativeTo.vertical != EZScreenPlacement.VERTICAL_ALIGN.OBJECT)
			sp.relativeObject = null;

		// See if our dependency is circular:
		if (sp.relativeObject != null)
		{
			if (!EZScreenPlacement.TestDepenency(sp))
			{
				Debug.LogError("ERROR: The Relative Object you recently assigned on \"" + sp.name + "\" which points to \"" + sp.relativeObject.name + "\" would create a circular dependency.  Please check your placement dependencies to resolve this.");
				sp.relativeObject = null;
			}
		}

		return true;
	}

	public virtual bool DidChange(EZScreenPlacement sp)
	{
		if (worldPos != sp.transform.position)
		{
			if (sp.allowTransformDrag)
			{
				// Calculate new screen position:
				sp.WorldToScreenPos(sp.transform.position);
			}
			else
				sp.PositionOnScreen();
			return true;
		}
		if (screenPos != sp.screenPos)
			return true;
		if (renderCamera != null && (screenSize.x != sp.renderCamera.pixelWidth || screenSize.y != sp.renderCamera.pixelHeight))
			return true;
		if (!relativeTo.Equals(sp.relativeTo))
			return true;
		if (renderCamera != sp.renderCamera)
			return true;
		if (relativeObject != sp.relativeObject)
		{
#if UNITY_EDITOR
			// Remove ourselves as a dependent on the previous object:
			if(relativeObject != null)
			{
				EZScreenPlacement c = relativeObject.GetComponent<EZScreenPlacement>();
				if (c != null)
					c.RemoveDependent(sp);
			}

			// Add ourselves as a dependent to the new object:
			if(sp.relativeObject != null)
			{
				EZScreenPlacement c = sp.relativeObject.GetComponent<EZScreenPlacement>();
				if (c != null)
					c.AddDependent(sp);
			}
#endif
			return true;
		}

		return false;
	}
}