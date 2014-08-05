//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define AUTO_SET_LAYER
#define SET_LAYERS_RECURSIVELY


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// UIPanelBase provides functionality as a container for controls.
/// Controls can be organized into panels and manipulated as a
/// group.
/// 
/// To organize controls into a panel, simply make them children
/// of a GameObject containing a UIPanel component.
/// </remarks>
[System.Serializable]
public abstract class UIPanelBase : MonoBehaviour, IUIContainer, IUIObject
{
	/// <remarks>
	/// Delegate definition for receiving notification of
	/// when a UIPanel has finished transitioning.
	/// </remarks>
	/// <param name="panel">Reference to the panel itself.</param>
	/// <param name="transition">Reference to the transition which has just completed.</param>
	public delegate void TransitionCompleteDelegate(UIPanelBase panel, EZTransition transition);

	// Array of UI Objects (buttons, etc)
	protected EZLinkedList<EZLinkedListNode<IUIObject>> uiObjs = new EZLinkedList<EZLinkedListNode<IUIObject>>();

	// Array of child panels
	protected EZLinkedList<EZLinkedListNode<UIPanelBase>> childPanels = new EZLinkedList<EZLinkedListNode<UIPanelBase>>();

	/// <remarks>
	/// UIPanel provides functionality as a container for controls.
	/// Controls can be organized into panels and manipulated as a
	/// group.
	/// 
	/// To organize controls into a panel, simply make them children
	/// of a GameObject containing a UIPanel component.
	/// </remarks>


	//------------------------------------------
	// Transition stuff:
	//------------------------------------------
	public abstract EZTransitionList Transitions
	{
		get;
	}

	// Flags indicating whether each corresponding
	// transition should block input until it
	// completes:
	[HideInInspector]
	public bool[] blockInput = new bool[] { true, true, true, true };


	protected EZTransition prevTransition;
	protected int prevTransIndex;
	protected bool m_started = false;

	/// <summary>
	/// The index of the panel in the optional UIPanelManager which
	/// manages it.  This is an optional value that allows you to
	/// determine the order in which this panel will appear in a
	/// menu that is navigated without specifying the specific 
	/// panels to go to, but rather moves to each panel based upon
	/// its index.  If no default panel is specified in the 
	/// UIPanelManager, the panel with the lowest index value will 
	/// be the first panel shown.
	/// </summary>
	public int index;

	/// <summary>
	/// When true, will recursively set all child objects to inactive.
	/// Conversely, it will recursively set all child objects to active
	/// when brought in.
	/// </summary>
	public bool deactivateAllOnDismiss;

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

	// List of child objects which are transition subjects:
	[System.NonSerialized]
	protected Dictionary<int, GameObject> subjects = new Dictionary<int, GameObject>();

	// Delegate to be called when a transition completes
	protected TransitionCompleteDelegate tempTransCompleteDel;

	// When true, indicates that the corresponding
	// transition should apply to all children of
	// this object.
/*
	[HideInInspector]
	public bool[] applyToChildren = new bool[4];
*/
	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
				EZAnimator.instance.Stop(gameObject);

			if (detargetOnDisable && UIManager.Exists())
				UIManager.instance.Detarget(this);
		}
	}


	// Use this for initialization
	public virtual void Start () 
	{
		// Only run start once!
		if (m_started)
			return;

		m_started = true;

		ScanChildren();

		for (int i = 0; i < Transitions.list.Length; ++i )
			Transitions.list[i].MainSubject = this.gameObject;

		// See if we'll need to add children to our transitions:
		//if(applyToChildren[0] || applyToChildren[1] || applyToChildren[2] || applyToChildren[3])
			SetupTransitionSubjects();
	}

	// Scans all child objects looking for IUIObjects and other panels
	public void ScanChildren()
	{
		uiObjs.Clear();

		// Reuse for IUIObjects:
		IUIObject obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(IUIObject), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			// Don't add ourselves as children:
			if (objs[i] == this || objs[i].gameObject == gameObject)
				continue;

#if AUTO_SET_LAYER
			// Only reset the child object layers if we're in-line
			// with the UIManager:
			if (gameObject.layer == UIManager.instance.gameObject.layer)
	#if SET_LAYERS_RECURSIVELY
				UIPanelManager.SetLayerRecursively(objs[i].gameObject, gameObject.layer);
	#else
				objs[i].gameObject.layer = gameObject.layer;
	#endif
#endif
			obj = (IUIObject)objs[i];
			uiObjs.Add(new EZLinkedListNode<IUIObject>(obj));
			obj.RequestContainership(this);
		}


		// Reuse for UIPanelBase objects:
		UIPanelBase panel;
		objs = transform.GetComponentsInChildren(typeof(UIPanelBase), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			// Don't add ourselves as children:
			if (objs[i] == this || objs[i].gameObject == gameObject)
				continue;

#if AUTO_SET_LAYER
			// Only reset the child object layers if we're in-line
			// with the UIManager:
			if(gameObject.layer == UIManager.instance.gameObject.layer)
	#if SET_LAYERS_RECURSIVELY
				UIPanelManager.SetLayerRecursively(objs[i].gameObject, gameObject.layer);
	#else
				objs[i].gameObject.layer = gameObject.layer;
	#endif
#endif
			panel = (UIPanelBase)objs[i];
			childPanels.Add(new EZLinkedListNode<UIPanelBase>(panel));
			panel.RequestContainership(this);
		}
	}

	// Sets up the subjects of our transitions:
	protected virtual void SetupTransitionSubjects()
	{
		GameObject go;
		int hash;

		// Only register the callback for the BringIn/Dismiss transitions:
		for (int i = 0; i < 4; ++i)
			Transitions.list[i].AddTransitionEndDelegate(TransitionCompleted);

		if(uiObjs.Rewind())
		{
			do
			{
				go = ((Component)uiObjs.Current.val).gameObject;
				hash = go.GetHashCode();

				for (int i = 0; i < Transitions.list.Length; ++i)
					Transitions.list[i].AddSubSubject(go);

				if(!subjects.ContainsKey(hash))
					subjects.Add(hash, go);
			} while (uiObjs.MoveNext());
		}

		// Now find any sprites, since that's a common object type
		// we'll want to transition:
		Component[] sprites = transform.GetComponentsInChildren(typeof(SpriteRoot), true);

		for(int i=0; i<sprites.Length; ++i)
		{
			// Don't add ourselves as we are the main subject:
			if (sprites[i].gameObject == gameObject)
				continue;

			go = sprites[i].gameObject;
			hash = go.GetHashCode();
			
			// Only add new objects:
			if (subjects.ContainsKey(hash))
				continue;

			for (int j = 0; j < Transitions.list.Length; ++j)
				Transitions.list[j].AddSubSubject(go);

			subjects.Add(hash, go);
		}

		// Now find any SpriteText, since that's another common object type
		// we'll want to transition:
		Component[] texts = transform.GetComponentsInChildren(typeof(SpriteText), true);

		for (int i = 0; i < texts.Length; ++i)
		{
			// Don't add ourselves as we are the main subject:
			if (texts[i].gameObject == gameObject)
				continue;

			go = texts[i].gameObject;
			hash = go.GetHashCode();

			// Only add new objects:
			if (subjects.ContainsKey(hash))
				continue;

			for (int j = 0; j < Transitions.list.Length; ++j)
				Transitions.list[j].AddSubSubject(go);

			subjects.Add(hash, go);
		}

		// Now find any other renderables:
		Component[] renderers = transform.GetComponentsInChildren(typeof(Renderer), true);

		for (int i = 0; i < renderers.Length; ++i)
		{
			// Don't add ourselves as we are the main subject:
			if (renderers[i].gameObject == gameObject)
				continue;

			go = renderers[i].gameObject;
			hash = go.GetHashCode();

			// Only add new objects:
			if (subjects.ContainsKey(hash))
				continue;

			for (int j = 0; j < Transitions.list.Length; ++j)
				Transitions.list[j].AddSubSubject(go);

			subjects.Add(hash, go);
		}


		// Add our children IUIObjects to the list, and then
		// see if we have any additional GameObjects to add:
/*
		Dictionary<int, GameObject> objs = new Dictionary<int, GameObject>();
		if (uiObjs.Rewind())
		{
			do
			{
				go = ((Component)uiObjs.Current.val).gameObject;
				objs.Add(go.GetHashCode(), go);
			} while (uiObjs.MoveNext());
		}

		// Add first-tier children if they aren't the same
		// as our IUIObjects:
		foreach (Transform child in transform)
		{
			go = child.gameObject;
			int hash = go.GetHashCode();
			if (objs.ContainsKey(hash))
				continue;
			objs.Add(hash, go);
		}

		// Now add all of these as subjects:
		for(int i=0; i<applyToChildren.Length; ++i)
		{
			if (!applyToChildren[i])
				continue;

			Dictionary<int, GameObject>.ValueCollection coll = objs.Values;
			
			foreach(GameObject g in coll)
			{
				Transitions.list[i].AddSubSubject(g);
			}
		}
*/
	}

	public void AddChild(GameObject go)
	{
		IUIObject obj = (IUIObject) go.GetComponent("IUIObject");

		if(obj != null)
		{
			if(obj.Container != (IUIContainer)this)
				obj.Container = this;
			uiObjs.Add(new EZLinkedListNode<IUIObject>(obj));
		}
		else
		{
			UIPanelBase panel = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
			if (panel != null)
			{
				if (panel.Container != (IUIContainer)this)
					panel.Container = this;
				childPanels.Add(new EZLinkedListNode<UIPanelBase>(panel));
			}
		}

		// Disable the new child if we are also disabled:
		if (!gameObject.active)
			go.SetActiveRecursively(false);

		AddSubject(go);
	}

	public void RemoveChild(GameObject go)
	{
		IUIObject obj = (IUIObject)go.GetComponent("IUIObject");

		if (obj != null)
		{
			if(uiObjs.Rewind())
			{
				do
				{
					if(uiObjs.Current.val == obj)
					{
						uiObjs.Remove(uiObjs.Current);
						break;
					}
				} while (uiObjs.MoveNext());
			}

			if (obj.Container == (IUIContainer)this)
				obj.Container = null;
		}
		else
		{
			UIPanelBase panel = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
			if (panel != null)
			{
				if (childPanels.Rewind())
				{
					do
					{
						if (childPanels.Current.val == panel)
						{
							childPanels.Remove(childPanels.Current);
							break;
						}
					} while (childPanels.MoveNext());
				}

				if (panel.Container == (IUIContainer)this)
					panel.Container = null;
			}
		}

		RemoveSubject(go);
	}

	/// <summary>
	/// Makes the specified GameObject a child of
	/// this panel, including making it a child of
	/// the panel's transform.
	/// </summary>
	/// <param name="go">GameObject to make a child of the panel.</param>
	public void MakeChild(GameObject go)
	{
		AddChild(go);

		go.transform.parent = transform;
	}

	/// <summary>
	/// Adds a GameObject as a subject of this
	/// panel's transitions.
	/// </summary>
	/// <param name="go">GameObject to be added as a subject.</param>
	public void AddSubject(GameObject go)
	{
		int hash = go.GetHashCode();

		if (subjects.ContainsKey(hash))
			return;

		subjects.Add(hash, go);

		for (int i = 0; i < Transitions.list.Length; ++i)
			Transitions.list[i].AddSubSubject(go);

		// Add the subject to any parent container as well:
		if (container != null)
			container.AddSubject(go);
	}

	/// <summary>
	/// Removes a GameObject as a subjects of
	/// this panel's transitions.
	/// </summary>
	/// <param name="go">GameObject to be removed as a subject.</param>
	public void RemoveSubject(GameObject go)
	{
		int hash = go.GetHashCode();

		if (!subjects.ContainsKey(hash))
			return;

		subjects.Remove(hash);

		for (int i = 0; i < Transitions.list.Length; ++i)
			Transitions.list[i].RemoveSubSubject(go);

		// Remove the subject from any parent container as well:
		if (container != null)
			container.RemoveSubject(go);
	}

	// Returns array of the names of the transitions
	public string[] GetTransitionNames()
	{
		if (Transitions == null)
			return null;

		string[] names = new string[Transitions.list.Length];

		for (int i = 0; i < Transitions.list.Length; ++i)
			names[i] = Transitions.list[i].name;

		return names;
	}

	/// <summary>
	/// Returns a reference to the transition at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the transition to retrieve.</param>
	/// <returns>Returns a reference to the transition at the specified index.  Null if none is found at the specified index.</returns>
	public EZTransition GetTransition(int index)
	{
		if (Transitions == null)
			return null;
		if (Transitions.list == null)
			return null;
		if (Transitions.list.Length <= index || index < 0)
			return null;

		return Transitions.list[index];
	}

	/// <summary>
	/// Returns a reference to the specified transition.
	/// </summary>
	/// <param name="transition">The enum identifying the transition to retrieve.</param>
	/// <returns>Returns a reference to the specified transition.  Null if none is found.</returns>
	public EZTransition GetTransition(UIPanelManager.SHOW_MODE transition)
	{
		return GetTransition((int)transition);
	}

	/// <summary>
	/// Returns a reference to the specified transition.
	/// </summary>
	/// <param name="transName">The name of the transition to retrieve.</param>
	/// <returns>Returns a reference to the specified transition.  Null if none is found.</returns>
	public EZTransition GetTransition(string transName)
	{
		if (Transitions == null)
			return null;
		if (Transitions.list == null)
			return null;

		EZTransition[] list = Transitions.list;

		for (int i = 0; i < list.Length; ++i)
			if (string.Equals(list[i].name, transName, System.StringComparison.CurrentCultureIgnoreCase))
				return list[i];

		return null;
	}

	/// <summary>
	/// Starts one of the panel's "bring in" or "dismiss" transitions.
	/// </summary>
	/// <param name="mode">The mode corresponding to the transition that should be played.</param>
	public virtual void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		if (!m_started)
			Start();

		// Finish any pending transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		prevTransIndex = (int)mode;
		
		if(blockInput[prevTransIndex])
			UIManager.instance.LockInput();

		prevTransition = Transitions.list[prevTransIndex];

		// Activate all children, if we were set to deactivate
		// them on dismissal:
		if(deactivateAllOnDismiss)
		{
			if(mode == UIPanelManager.SHOW_MODE.BringInBack ||
			    mode == UIPanelManager.SHOW_MODE.BringInForward)
			{
				gameObject.SetActiveRecursively(true);
				Start();
			}
		}

		prevTransition.Start();
	}

	/// <summary>
	/// Starts the transition matching the specified name (case-insensitive).
	/// </summary>
	/// <param name="transName">The name of the transition to start.  Ex: "Bring In Forward"</param>
	public virtual void StartTransition(string transName)
	{
		if (!m_started)
			Start();

		EZTransition[] list = Transitions.list;

		for(int i=0; i < list.Length; ++i)
			if (string.Equals(list[i].name, transName, System.StringComparison.CurrentCultureIgnoreCase))
			{
				// Finish any pending transition:
				if (prevTransition != null)
					prevTransition.StopSafe();

				prevTransIndex = i;

				if (blockInput[prevTransIndex])
					UIManager.instance.LockInput();

				prevTransition = list[prevTransIndex];

				// Activate all children, if we were set to deactivate
				// them on dismissal:
				if (deactivateAllOnDismiss)
				{
					if (prevTransition == list[(int)UIPanelManager.SHOW_MODE.BringInBack] ||
						prevTransition == list[(int)UIPanelManager.SHOW_MODE.BringInForward])
					{
						gameObject.SetActiveRecursively(true);
						Start();
					}
				}

				prevTransition.Start();
			}
	}

	// Gets called when our transition is complete
	public void TransitionCompleted(EZTransition transition)
	{
		prevTransition = null;

		// Deactivate all child objects?
		if(deactivateAllOnDismiss)
		{
			// If this was a dismissal:
			if (transition == Transitions.list[2] ||
			   transition == Transitions.list[3])
				gameObject.SetActiveRecursively(false);
		}

		if (tempTransCompleteDel != null)
			tempTransCompleteDel(this, transition);
		tempTransCompleteDel = null;

		if (blockInput[prevTransIndex] && UIManager.Exists())
			UIManager.instance.UnlockInput();
	}


	/// <summary>
	/// Brings in the panel by playing its "Bring In Forward" transition.
	/// </summary>
	public virtual void BringIn()
	{
		StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}


	/// <summary>
	/// Dismisses the panel by playing its "Dismiss Forward" transition.
	/// </summary>
	public virtual void Dismiss()
	{
		StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}


	// Compares panel indices so they can be sorted this way.
	public static int CompareIndices(UIPanelBase a, UIPanelBase b)
	{
		return a.index - b.index;
	}


	/// <summary>
	/// Temporarily adds a delegate to be called when the next
	/// transition completes.  This will be unset after it is called.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddTempTransitionDelegate(TransitionCompleteDelegate del)
	{
		tempTransCompleteDel += del;
	}

	/// <summary>
	/// Indicates whether the panel is currently transitioning.
	/// </summary>
	public bool IsTransitioning
	{
		get { return prevTransition != null; }
	}


	//---------------------------------------------------
	// IUIObject interface stuff
	//---------------------------------------------------
	protected bool m_controlIsEnabled = true;

	public virtual bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set { m_controlIsEnabled = value; }
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
		get { return detargetOnDisable; }
		set { detargetOnDisable = value; }
	}

	// Allows an object to act as a proxy for other
	// controls - i.e. a UIVirtualScreen
	// But in our case, just return ourselves since
	// we're not acting as a proxy
	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		return this;
	}

	protected IUIContainer container;

	public virtual IUIContainer Container
	{
		get { return container; }
		set 
		{
			IUIContainer oldCont = container;
			container = value;

			// Add any subjects:
			if (container != null)
			{
				foreach (KeyValuePair<int, GameObject> sub in subjects)
					container.AddSubject(sub.Value);
			}

			// Remove subjects from previous container:
			if(oldCont != null && oldCont != container)
			{
				foreach (KeyValuePair<int, GameObject> sub in subjects)
					oldCont.RemoveSubject(sub.Value);
			}
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

	protected EZInputDelegate inputDelegate;
	protected EZValueChangedDelegate changeDelegate;
	public virtual void SetInputDelegate(EZInputDelegate del)
	{
		inputDelegate = del;
	}
	public virtual void AddInputDelegate(EZInputDelegate del)
	{
		inputDelegate += del;
	}
	public virtual void RemoveInputDelegate(EZInputDelegate del)
	{
		inputDelegate -= del;
	}
	public virtual void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = del;
	}
	public virtual void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate += del;
	}
	public virtual void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate -= del;
	}

	public virtual void OnInput(POINTER_INFO ptr)
	{
		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
	}

	#region Drag&Drop

	//---------------------------------------------------
	// Drag & Drop stuff
	//---------------------------------------------------

	public object Data
	{
		get { return null; }
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

	public float DragOffset
	{
		get { return 0; }
		set { }
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get { return EZAnimation.EASING_TYPE.Default; }
		set { }
	}

	public float CancelDragDuration
	{
		get { return 0; }
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

	public void DragUpdatePosition(POINTER_INFO ptr) { }

	public void CancelDrag() { }

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
	public void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del) { }
	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate() { return null; }


	#endregion
}