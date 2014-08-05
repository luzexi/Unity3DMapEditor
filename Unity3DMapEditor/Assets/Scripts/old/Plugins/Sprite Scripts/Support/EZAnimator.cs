//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define COROUTINE_PUMP		// Tells EZAnimator to use a coroutine for the animation pump.  Otherwise, Update() is used.
#define PUMP_ALWAYS_RUNNING	// Sets the pump to run once per frame and does not suspend.  This is the recommended method.  Commenting this line out will have no effect if COROUTINE_PUMP is also commented out.
#define STOP_ON_LEVEL_LOAD	// Stop animation when a new level is loaded (this is to be safe so that objects that are destroyed on load are not attempted to be animated)
// #define USE_DELTA_TIME	// Base animation on Time.deltaTime instead of Time.realtimeSinceStartup


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Holds a list of EZTransitions
/// </summary>
[System.Serializable]
public class EZTransitionList
{
	public EZTransition[] list = new EZTransition[0];

	public EZTransitionList(EZTransition[] l)
	{
		list = l;
	}

	public EZTransitionList()
	{
		list = new EZTransition[0];
	}

	// Makes other transitions in the list clones
	// of the specified transition.
	// "force" indicates whether or not to force
	// already-initialized transitions to be clones.
	public void Clone(int source, bool force)
	{
		if (source >= list.Length)
			return;

		EZTransition src = list[source];

		// Only truncate the other transitions to 0
		// if this is a forced clone:
		if (!force && src.animationTypes.Length < 1)
			return;

		for (int i = 0; i < list.Length; ++i)
			if (i != source && (force || !list[i].initialized))
				list[i].Copy(src);
	}

	// Clones the specified source transition
	// to any uninitialized transitions in this
	// list.
	public void CloneAsNeeded(int source)
	{
		Clone(source, false);
	}

	// Forces all other transitions to be clonse
	// of the specified transition.
	public void CloneAll(int source)
	{
		Clone(source, true);
	}

	// Marks all transitions as initialized.
	public void MarkAllInitialized()
	{
		for (int i = 0; i < list.Length; ++i)
			list[i].initialized = true;
	}

	// Returns array of the names of the transitions
	public string[] GetTransitionNames()
	{
		if (list == null)
			return null;

		string[] names = new string[list.Length];

		for (int i = 0; i < list.Length; ++i)
			names[i] = list[i].name;

		return names;
	}

	// Copies all of the transitions in this list
	// to the target list
	public void CopyTo(EZTransitionList target)
	{
		CopyTo(target, false);
	}

	// Copies all of the transitions in this list
	// to the target list
	// copyInit indicates whether the initialization
	// member of each transition should also be copied.
	public void CopyTo(EZTransitionList target, bool copyInit)
	{
		if(target == null)
			return;
		if (target.list == null)
			return;

		for(int i=0; i<list.Length && i<target.list.Length; ++i)
		{
			if(target.list[i] == null)
				continue;

			target.list[i].Copy(list[i]);
			if (copyInit)
				target.list[i].initialized = list[i].initialized;
		}
	}

	// Same as CopyTo, but creates new lists for
	// the target
	public void CopyToNew(EZTransitionList target)
	{
		CopyToNew(target, false);
	}

	// Same as CopyTo, but creates new lists for
	// the target
	// copyInit indicates whether the initialization
	// member of each transition should also be copied.
	public void CopyToNew(EZTransitionList target, bool copyInit)
	{
		if (target == null)
			return;
		if (target.list == null)
			return;

		target.list = new EZTransition[list.Length];
		for (int i = 0; i < target.list.Length; i++)
			target.list[i] = new EZTransition(list[i].name);

		CopyTo(target, copyInit);
	}
}


/// <remarks>
/// A sort of macro class which contains a list of
/// animations to carry out at once.
/// </remarks>
[System.Serializable]
public class EZTransition
{
	/// <remarks>
	/// Definition for a delegate that is called when a transition completes.
	/// </remarks>
	/// <param name="transition">Receives a reference to the transition that has ended.</param>
	public delegate void OnTransitionEndDelegate(EZTransition transition);

	/// <remarks>
	/// Definition for a delegate that is called when a transition starts.
	/// </remarks>
	/// <param name="transition">Receives a reference to the transition that has started.</param>
	public delegate void OnTransitionStartDelegate(EZTransition transition);

	public string name;
	public EZAnimation.ANIM_TYPE[] animationTypes = new EZAnimation.ANIM_TYPE[0];
	public AnimParams[] animParams = new AnimParams[0];

	[System.NonSerialized]
	protected EZLinkedList<EZLinkedListNode<EZAnimation>> runningAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();
	protected EZLinkedList<EZLinkedListNode<EZAnimation>> idleAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();

	// The main object which is the subject of this transition
	[System.NonSerialized]
	protected GameObject mainSubject;

	// Sub-objects which are also subjects (for applicable elements, such as FadeSprite)
	[System.NonSerialized]
	protected EZLinkedList<EZLinkedListNode<GameObject>> subSubjects = new EZLinkedList<EZLinkedListNode<GameObject>>();

	[System.NonSerialized]
	protected OnTransitionEndDelegate onEndDelegates;
	[System.NonSerialized]
	protected OnTransitionStartDelegate onStartDelegates;

	// Used to tell if this transition has its own 
	// values, or if those that exist are cloned 
	// from another transition.
	public bool initialized;

	// Lets us tell whether we have forced a stop
	// of the transition (StopSafe() or End()) and
	// therefore whether we should run any OnEnd
	// delegates mid-stream.
	protected bool forcedStop = false;


	public EZTransition(string n)
	{
		name = n;
		runningAnims = null;
	}

	/// <summary>
	/// Adds a delegate to be called when the transition is started.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddTransitionStartDelegate(OnTransitionStartDelegate del)
	{
		onStartDelegates += del;
	}

	/// <summary>
	/// Removes the specified delegate from the list of those to be
	/// called when the transition begins.
	/// </summary>
	/// <param name="del">The delegate to be removed.</param>
	public void RemoveTransitionStartDelegate(OnTransitionStartDelegate del)
	{
		onStartDelegates -= del;
	}

	/// <summary>
	/// Adds a delegate to be called when the transition is completed.
	/// NOTE: This will be called when all non-looping elements have
	/// finished.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddTransitionEndDelegate(OnTransitionEndDelegate del)
	{
		onEndDelegates += del;
	}

	/// <summary>
	/// Removes the specified delegate from the list of those to be
	/// called upon completion of a transition.
	/// </summary>
	/// <param name="del">The delegate to be removed.</param>
	public void RemoveTransitionEndDelegate(OnTransitionEndDelegate del)
	{
		onEndDelegates -= del;
	}

	/// <summary>
	/// Causes this transition to become a copy of the specified transition.
	/// The name is not copied - just the design-time parameters, save for
	/// the initialization flag for obvious reasons.
	/// i.e. the animationTypes list and animParams.
	/// This also sets the transition's "initialized" setting to false which
	/// tells us it no longer holds its "own" unique values, but rather is a
	/// clone of another transition.
	/// </summary>
	/// <param name="src">Reference to the transition to be copied.</param>
	public void Copy(EZTransition src)
	{
		initialized = false;	// We're a clone now.

		if (src.animationTypes == null)
			return;
// 		if (src.animationTypes.Length == 0)
// 			return;

		animationTypes = new EZAnimation.ANIM_TYPE[src.animationTypes.Length];
		src.animationTypes.CopyTo(animationTypes, 0);

		animParams = new AnimParams[src.animParams.Length];

		for (int i = 0; i < animParams.Length; ++i)
		{
			animParams[i] = new AnimParams(this);
			animParams[i].Copy(src.animParams[i]);
		}
	}

	/// <summary>
	/// Accessor of the list of subjects subordinate
	/// to the main subject of the transition.
	/// </summary>
	public EZLinkedList<EZLinkedListNode<GameObject>> SubSubjects
	{
		get { return subSubjects; }
	}

	/// <summary>
	/// Sets the main, primary subject of this transition.
	/// For most transition elements, this is the only
	/// object that will be concerned.  For others, such 
	/// as FadeSprite, sub-objects will also be subjects.
	/// </summary>
	public GameObject MainSubject
	{
		get { return mainSubject; }
		set { mainSubject = value; }
	}


	/// <summary>
	/// Adds a subject to the subjects list
	/// </summary>
	/// <param name="go">GameObject that shall be a subject of the transition.</param>
	public void AddSubSubject(GameObject go)
	{
		if (subSubjects == null)
			subSubjects = new EZLinkedList<EZLinkedListNode<GameObject>>();
		subSubjects.Add(new EZLinkedListNode<GameObject>(go));
	}

	/// <summary>
	/// Removes a subject from the transition.
	/// </summary>
	/// <param name="go">GameObject to remove.</param>
	public void RemoveSubSubject(GameObject go)
	{
		if (subSubjects == null)
			return;

		// Save our current:
		EZLinkedListNode<GameObject> prevCur = subSubjects.Current;

		if(subSubjects.Rewind())
		{
			do
			{
				// Assumes only one entry:
				if (subSubjects.Current.val == go)
				{
					subSubjects.Remove(subSubjects.Current);
					break;
				}

			} while (subSubjects.MoveNext());
		}

		subSubjects.Current = prevCur;
	}

	// Called when a transition element ends its animation.
	public void OnAnimEnd(EZAnimation anim)
	{
		EZLinkedListNode<EZAnimation> node = (EZLinkedListNode<EZAnimation>)anim.Data;

		// This may be a zero-duration animation
		// and we want to skip calling our delegate
		// since we could get a bunch of these,
		// resulting in multiple calls in one frame:
		if (node == null)
			return;
		if (runningAnims == null)
			return;
		
		node.val = null;
		runningAnims.Remove(node);
		idleAnims.Add(node);

		// If we don't have any delegates, or this was a
		// forced stop, don't bother continuing:
		if (onEndDelegates == null || forcedStop)
			return;

		// Look to see if we have any animations still running
		// which are non-looping:
		EZLinkedListNode<EZAnimation> cur = runningAnims.Current;
		if(runningAnims.Rewind())
		{
			do
			{
				if (runningAnims.Current.val.Duration > 0)
					return;
			} while (runningAnims.MoveNext());
		}
		runningAnims.Current = cur;

		// If we got here, we don't have any non-looping 
		// animations left:
		CallEndDelegates();
	}

	// Adds an animation to our running animation list:
	public EZLinkedListNode<EZAnimation> AddRunningAnim()
	{
		EZLinkedListNode<EZAnimation> animNode;

		if (runningAnims == null)
		{
			runningAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();
			if(idleAnims == null)
				idleAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();
		}

		if(idleAnims.Count > 0)
		{
			animNode = idleAnims.Head;
			idleAnims.Remove(animNode);
		}
		else
		{
			animNode = new EZLinkedListNode<EZAnimation>(null);
		}

		runningAnims.Add(animNode);
		return animNode;
	}

	/// <summary>
	/// Starts the transition.
	/// </summary>
	public void Start()
	{
		if (mainSubject == null)
		{
			if (subSubjects == null)
				return;
			else if (subSubjects.Count < 1)
				return;
		}

		// Ensure the transition isn't already running:
		StopSafe();

		// Call any begin delegate(s):
		if (onStartDelegates != null)
			onStartDelegates(this);

		EZAnimator.instance.AddTransition(this);

		// If we don't have any running animations, then
		// our transition was probably all 0-duration,
		// meaning our on end delegate wasn't called,
		// so call it here:
		if (runningAnims == null)
			goto OnEnd;
		if (runningAnims.Count < 1)
			goto OnEnd;

		return;

		OnEnd:
		CallEndDelegates();
	}

	/// <summary>
	/// Ends the transition's elements prematurely.
	/// </summary>
	public void End()
	{
		if (runningAnims == null)
			return;

		EZLinkedListNode<EZAnimation> animNode;
		EZAnimation anim;

		if(runningAnims.Rewind())
		{
			// Keep our delegate from being run by OnAnimEnd:
			forcedStop = true;

			do
			{
				animNode = runningAnims.Current;
				anim = animNode.val;

				if (null != anim)
				{
					// First, disable its completed delegate
					// so we don't screw up our place in our
					// loop by having OnAnimEnd() called which
					// also uses runningAnims:
					anim.CompletedDelegate = null;

					EZAnimator.instance.StopAnimation(anim, true);
				}

				runningAnims.Remove(animNode);
				idleAnims.Add(animNode);
				animNode.val = null;
			} while (runningAnims.MoveNext());

			// Call any completion delegate:
			forcedStop = false;
			CallEndDelegates();
		}
	}

	/// <summary>
	/// Stops this transition's elements safely.
	/// That is, if any elements are of the "By"
	/// mode, then are ended (End()), or else they
	/// are simply stopped (Stop()).
	/// </summary>
	public void StopSafe()
	{
		if (runningAnims == null)
			return;

		EZLinkedListNode<EZAnimation> cur = runningAnims.Current;
		if(runningAnims.Rewind())
		{
			EZLinkedListNode<EZAnimation> animNode;
			EZAnimation anim;

			// Keep our delegate from being run by OnAnimEnd:
			forcedStop = true;

			do
			{
				animNode = runningAnims.Current;
				anim = animNode.val;

				if (null != anim)
				{
					// First, disable its completed delegate
					// so we don't screw up our place in our
					// loop by having OnAnimEnd() called which
					// also uses runningAnims:
					anim.CompletedDelegate = null;

					if (anim.Mode == EZAnimation.ANIM_MODE.By)
						EZAnimator.instance.StopAnimation(anim, true);
					else
						EZAnimator.instance.StopAnimation(anim, false);
				}

				runningAnims.Remove(animNode);
				idleAnims.Add(animNode);
				animNode.val = null;
			} while (runningAnims.MoveNext());

			// Call any completion delegate:
			forcedStop = false;
			CallEndDelegates();
		}
		runningAnims.Current = cur;
	}

	/// <summary>
	/// Pauses all running transition elements.
	/// </summary>
	public void Pause()
	{
		EZLinkedListIterator<EZLinkedListNode<EZAnimation>> i;
		for (i = runningAnims.Begin(); !i.Done; i.Next())
			i.Current.val.Paused = true;
		i.End();
	}

	/// <summary>
	/// Unpauses all running transition elements.
	/// </summary>
	public void Unpause()
	{
		EZLinkedListIterator<EZLinkedListNode<EZAnimation>> i;
		for (i = runningAnims.Begin(); !i.Done; i.Next())
			i.Current.val.Paused = false;
		i.End();
	}

	/// <summary>
	/// Returns whether the transition is currently running
	/// (has any active elements remaining).
	/// Infinitely looping elements are ignored here.
	/// </summary>
	/// <returns>True if any non-looping elements are still running. False otherwise.</returns>
	public bool IsRunning()
	{
		if (runningAnims == null)
			return false;

		EZLinkedListIterator<EZLinkedListNode<EZAnimation>> i;
		for (i = runningAnims.Begin(); !i.Done; i.Next())
			if (i.Current.val.Duration > 0)
			{
				i.End();
				return true;
			}
		i.End();

		return false;
	}

	/// <summary>
	/// Returns whether the transition is currently running
	/// (has any active elements remaining).
	/// Infinitely looping elements are included in the check.
	/// </summary>
	/// <returns>True if any elements at all are still running. False otherwise.</returns>
	public bool IsRunningAtAll()
	{
		if (runningAnims == null)
			return false;

		if (runningAnims.Count > 0)
			return true;
		else
			return false;
	}

	// Calls all our ending delegates
	protected void CallEndDelegates()
	{
		// If this is a forced stop (StopSafe() or End()),
		// then wait 'til its over first:
		if (forcedStop)
			return;

		if(onEndDelegates != null)
			onEndDelegates(this);
	}

	/// <summary>
	/// Adds a new transition element.
	/// </summary>
	/// <returns>Returns the index of the newly created element.</returns>
	public int Add()
	{
		initialized = true; // We have a value of our own! Yay!

		List<EZAnimation.ANIM_TYPE> tempTypes = new List<EZAnimation.ANIM_TYPE>();

		if (animationTypes.Length > 0)
			tempTypes.AddRange(animationTypes);

		tempTypes.Add(EZAnimation.ANIM_TYPE.Translate);
		animationTypes = tempTypes.ToArray();

		List<AnimParams> tempParams = new List<AnimParams>();

		if (animParams.Length > 0)
			tempParams.AddRange(animParams);

		tempParams.Add(new AnimParams(this));
		animParams = tempParams.ToArray();

		return animationTypes.Length-1;
	}

	/// <summary>
	/// Adds a new transition element of the specified type.
	/// </summary>
	/// <param name="type">An enum value indicating the element type.</param>
	/// <returns>A reference to the AnimParams object that will contain information
	/// about the transition element.</returns>
	public AnimParams AddElement(EZAnimation.ANIM_TYPE type)
	{
		int idx = Add();
		animationTypes[idx] = type;
		return animParams[idx];
	}

	/// <summary>
	/// Removes the specified transition element.
	/// </summary>
	/// <param name="index">Index of the transition element to be removed.</param>
	public void Remove(int index)
	{
		initialized = true; // We have a value of our own! Yay!

		List<EZAnimation.ANIM_TYPE> tempTypes = new List<EZAnimation.ANIM_TYPE>();

		if(animationTypes.Length > 0)
			tempTypes.AddRange(animationTypes);

		tempTypes.RemoveAt(index);
		animationTypes = tempTypes.ToArray();

		List<AnimParams> tempParams = new List<AnimParams>();

		if(animParams.Length > 0)
			tempParams.AddRange(animParams);

		tempParams.RemoveAt(index);
		animParams = tempParams.ToArray();
	}

	/// <summary>
	/// Sets the animation type of the element specified by index.
	/// </summary>
	/// <param name="index">Index of the element.</param>
	/// <param name="type">Type to set the element to.</param>
	public void SetElementType(int index, EZAnimation.ANIM_TYPE type)
	{
		if(index >= animationTypes.Length)
			return;

		// If this is changing, then we have our own unique values!:
		if (animationTypes[index] != type)
			initialized = true;

		animationTypes[index] = type;
	}

	// Returns an array of strings that serves as
	// the "names" of the transition elements:
	public string[] GetNames()
	{
		string[] names = new string[animationTypes.Length];

		for(int i=0; i<animationTypes.Length; ++i)
		{
			names[i] = i.ToString() + " - " + System.Enum.GetName(typeof(EZAnimation.ANIM_TYPE), animationTypes[i]);

			// Check our params' transition references while we're at it:
			if(animParams[i].transition != this)
				animParams[i].transition = this;
		}

		return names;
	}

	// Called by a control on startup
/*
	public void Init()
	{
		for (int i = 0; i < animParams.Length; ++i)
			animParams[i].Init();
	}
*/
}


// Defines an interface by which we'll call
// EditorGUI methods
public interface IGUIHelper
{
	System.Enum EnumField(string label, System.Enum selected);
	Color ColorField(string label, Color color);
	Vector3 Vector3Field(string label, Vector3 val);
	float FloatField(string label, float val);
	string TextField(string label, string val);
	Object ObjectField(string label, System.Type type, Object obj); 
}


/// <remarks>
/// Singleton class that manages all running animations as well as
/// provides an interface for creating/starting new animations.
/// </remarks>
public class EZAnimator : MonoBehaviour
{
	//----------------------------------------------------------------
	// Singleton code
	//----------------------------------------------------------------
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static EZAnimator s_Instance = null;

	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static EZAnimator instance
	{
		get
		{
			if (s_Instance == null)
			{
				GameObject go = new GameObject("EZAnimator");
				s_Instance = (EZAnimator)go.AddComponent(typeof(EZAnimator));
			}

			return s_Instance;
		}
	}

	public static bool Exists()
	{
		return s_Instance != null;
	}
	
	public void OnDestroy()
	{
		s_Instance = null;
	}
	//----------------------------------------------------------------
	// End Singleton code
	//----------------------------------------------------------------



	protected static Dictionary<EZAnimation.ANIM_TYPE, EZLinkedList<EZAnimation>> freeAnimPool = new Dictionary<EZAnimation.ANIM_TYPE, EZLinkedList<EZAnimation>>(); 
	protected static EZLinkedList<EZAnimation> animations = new EZLinkedList<EZAnimation>();// List of animation objects to animate
	protected static bool pumpIsRunning = false;

	// Tells us when the coroutine has run its course.
	protected static bool pumpIsDone = true;

	// Stuff used in our animation pump:
	protected static float _timeScale = 1f;
	protected static float startTime;
	protected static float time;
	protected static float elapsed;
	protected static EZAnimation anim;
	protected static float timePaused;


	// Working vars:
	int i;


	/// <summary>
	/// Works like Time.timeScale, only it still works when
	/// using realtime tracking.  When USE_DELTA_TIME is
	/// defined, this just acts as an alias for Time.timeScale.
	/// Otherwise, it is an independent value that only affects
	/// the rate of EZAnimations.
	/// </summary>
	public static float timeScale
	{
		get
		{
#if USE_DELTA_TIME
			return Time.timeScale;
#else
			return _timeScale;
#endif
		}

		set
		{
#if USE_DELTA_TIME
			Time.timeScale = value;
			_timeScale = Time.timeScale;
#else
			_timeScale = Mathf.Max(0, value);
#endif
		}
	}


	void Awake()
	{
		DontDestroyOnLoad(this);
	}

#if !COROUTINE_PUMP
	void Start()
	{
		startTime = Time.realtimeSinceStartup;
	}
#endif

	void OnLevelWasLoaded(int level)
	{
		if(animations == null)
			return;

		EZAnimation anim;
#if STOP_ON_LEVEL_LOAD
		// Check to see which animations are still valid:
		EZLinkedListIterator<EZAnimation> i;
		for(i = animations.Begin(); !i.Done; /*i.Next()*/)
		{
			//if(i.Current.GetSubject() == null)
			{
				anim = i.Current;
				anim._cancel();
				animations.Remove(anim);
				ReturnAnimToPool(anim);
			}
		}
		i.End();
#endif
	}

	// Makes sure we account for realtime passage while paused
	// due to loss of focus, etc:
	void OnApplicationPause(bool paused)
	{
#if !USE_DELTA_TIME
		if (paused)
			timePaused = Time.realtimeSinceStartup;
		else
		{
			// See how long we were paused:
			float pauseDuration = Time.realtimeSinceStartup - timePaused;
			startTime += pauseDuration;
		}
#endif
	}

// Decide whether to use the coroutine pump, or the Update() pump:
#if COROUTINE_PUMP

	// Actually runs the animations:
	protected static IEnumerator AnimPump()
	{
		EZLinkedListIterator<EZAnimation> i = animations.Begin();
		startTime = Time.realtimeSinceStartup;

		pumpIsDone = false;

		while (pumpIsRunning)
		{
#if !USE_DELTA_TIME
			// Realtime time tracking
			time = Time.realtimeSinceStartup;
			elapsed = (time - startTime) * _timeScale;
			startTime = time;
			// END Realtime time tracking
#else
			// deltaTime time tracking
			elapsed = Time.deltaTime;
			// END deltaTime time tracking
#endif
			for (i.Begin(animations); !i.Done; )
			{
				// If the animation is done, remove it:
				if (!i.Current.Step(elapsed))
				{
					anim = i.Current;
					animations.Remove(anim);

					// Add the animation object to our free pool:
					ReturnAnimToPool(anim);

					// Don't advance to the next since
					// this already happened when we
					// removed this one:
					continue;
				}

				i.NextNoRemove();
			}

			yield return null;	// Wait for the next frame 

#if !PUMP_ALWAYS_RUNNING
			// See if we're out of animations:
			if (animations.Empty)
            {
                pumpIsRunning = false;
				i.End();
                break;
            }
#endif
		}

		pumpIsDone = true;
	}

#else

	// Update() version of our animation pump:
	void Update()
	{
		EZLinkedListIterator<EZAnimation> i = animations.Begin();

#if !USE_DELTA_TIME
		// Realtime time tracking
		time = Time.realtimeSinceStartup;
		elapsed = (time - startTime) * _timeScale;
		startTime = time;
		// END Realtime time tracking
#else
			// deltaTime time tracking
			elapsed = Time.deltaTime;
			// END deltaTime time tracking
#endif
		for (i.Begin(animations); !i.Done; )
		{
			// If the animation is done, remove it:
			if (!i.Current.Step(elapsed))
			{
				anim = i.Current;
				animations.Remove(anim);

				// Add the animation object to our free pool:
				ReturnAnimToPool(anim);
			
				// Don't advance to the next since
				// this already happened when we
				// removed this one:
				continue;
			}

			i.NextNoRemove();
		}
	}

#endif


	/// <summary>
	/// Starts the animation pump coroutine.
	/// Normally, there is no need to call 
	/// this directly.  Only use this if you
	/// have manually stopped the pump to pause
	/// all animations or something.
	/// </summary>
	public void StartAnimationPump()
	{
#if COROUTINE_PUMP
		if (!pumpIsRunning && gameObject.active)
		{
			pumpIsRunning = true;
			StartCoroutine(PumpStarter());
		}
#endif
	}

#if COROUTINE_PUMP
	// Coroutine that gets the pump started:
	protected IEnumerator PumpStarter()
	{
		while (!pumpIsDone)
			yield return null;

		StartCoroutine(AnimPump());
	}
#endif

	/// <summary>
	/// Stops the animation pump from running.
	/// Normally, there is no need to call 
	/// this directly.  Only use this if you
	/// want to pause all animations or something.
	/// </summary>
	public static void StopAnimationPump()
	{
#if !PUMP_ALWAYS_RUNNING
		pumpIsRunning = false;
#endif
	}


	// Creates a new animation of the specified type
	// and returns a reference to it.
	protected EZAnimation CreateNewAnimation(EZAnimation.ANIM_TYPE type)
	{
		switch (type)
		{
			case EZAnimation.ANIM_TYPE.AnimClip:
				return new RunAnimClip();
			case EZAnimation.ANIM_TYPE.Crash:
				return new Crash();
			case EZAnimation.ANIM_TYPE.CrashRotation:
				return new CrashRotation();
			case EZAnimation.ANIM_TYPE.FadeAudio:
				return new FadeAudio();
			case EZAnimation.ANIM_TYPE.FadeSprite:
				return new FadeSprite();
			case EZAnimation.ANIM_TYPE.FadeMaterial:
				return new FadeMaterial();
			case EZAnimation.ANIM_TYPE.FadeText:
				return new FadeText();
			case EZAnimation.ANIM_TYPE.PunchPosition:
				return new PunchPosition();
			case EZAnimation.ANIM_TYPE.PunchRotation:
				return new PunchRotation();
			case EZAnimation.ANIM_TYPE.PunchScale:
				return new PunchScale();
			case EZAnimation.ANIM_TYPE.Rotate:
				return new AnimateRotation();
//			case EZAnimation.ANIM_TYPE.RotateAround:
//				return new AnimateRotation();
			case EZAnimation.ANIM_TYPE.Scale:
				return new AnimateScale();
			case EZAnimation.ANIM_TYPE.Shake:
				return new Shake();
			case EZAnimation.ANIM_TYPE.ShakeRotation:
				return new ShakeRotation();
			case EZAnimation.ANIM_TYPE.SmoothCrash:
				return new SmoothCrash();
			case EZAnimation.ANIM_TYPE.Translate:
				return new AnimatePosition();
			case EZAnimation.ANIM_TYPE.TranslateScreen:
				return new AnimateScreenPosition();
			case EZAnimation.ANIM_TYPE.TuneAudio:
				return new TuneAudio();
			case EZAnimation.ANIM_TYPE.FadeSpriteAlpha:
				return new FadeSpriteAlpha();
			case EZAnimation.ANIM_TYPE.FadeTextAlpha:
				return new FadeTextAlpha();
			case EZAnimation.ANIM_TYPE.RotateEuler:
				return new AnimateRotationEuler();
		}

		return null;
	}

	/// <summary>
	/// Fetches a free EZAnimation object of the specified type
	/// and creates a new one if no free one exists.
	/// If the object is found in a free list, it is removed
	/// from that list.
	/// </summary>
	/// <param name="type">The type of animation to get.</param>
	/// <returns>Reference to the animaiton object.</returns>
	public EZAnimation GetAnimation(EZAnimation.ANIM_TYPE type)
	{
		EZLinkedList<EZAnimation> freeList;
		EZAnimation anim;

		// See if we have a list of this type:
		if (freeAnimPool.TryGetValue(type, out freeList))
		{
			// See if there's a free object:
			if(!freeList.Empty)
			{
				anim = freeList.Head;
				freeList.Remove(anim);
				return anim;
			}
		}

		// Else, we need to create a new one:
		return CreateNewAnimation(type);
	}

	protected static void ReturnAnimToPool(EZAnimation anim)
	{
		EZLinkedList<EZAnimation> freeList;

		// Clear out some vital parts of the anim:
		anim.Clear();

		// First, see if a free list exists for this type:
		if (!freeAnimPool.TryGetValue(anim.type, out freeList))
		{
			freeList = new EZLinkedList<EZAnimation>();
			freeAnimPool.Add(anim.type, freeList);
		}

		freeList.Add(anim);

		// Return settings to defaults:
		anim.ResetDefaults();
	}

	/// <summary>
	/// Adds an animation object to the animator
	/// </summary>
	/// <param name="a">Animation object to be added.</param>
	public void AddAnimation(EZAnimation a)
	{
        // Only add if not already running:
        if (!a.running)
        {
            animations.Add(a);
            a.running = true;
        }

		StartAnimationPump();
	}

	/// <summary>
	/// Adds the specified transition's elements.
	/// </summary>
	/// <param name="t">Transition to be added.</param>
	public void AddTransition(EZTransition t)
	{
		EZAnimation anim;
		EZLinkedListNode<EZAnimation> animNode;
		EZLinkedList<EZLinkedListNode<GameObject>> subjects;
		EZAnimation.ANIM_TYPE type;

		if (t.animationTypes == null)
			return;

		for (int i = 0; i < t.animationTypes.Length; ++i)
		{
			type = t.animationTypes[i];

			// See if this type is one that applies to subSubjects:
			if(type == EZAnimation.ANIM_TYPE.FadeSprite ||
				type == EZAnimation.ANIM_TYPE.FadeText ||
				type == EZAnimation.ANIM_TYPE.FadeMaterial ||
				type == EZAnimation.ANIM_TYPE.FadeTextAlpha ||
				type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha)
			{
				subjects = t.SubSubjects;

				if (subjects != null && subjects.Rewind())
				{
					do
					{
						anim = GetAnimation(type);

						t.animParams[i].transition = t;

						if (!anim.Start(subjects.Current.val, t.animParams[i]))
						{
							ReturnAnimToPool(anim);
						}
						else if(anim.running)
						{
							// Save a reference to this running animation:
							animNode = t.AddRunningAnim();
							animNode.val = anim;

							// Save a reference to this node for later:
							anim.Data = animNode;
						}
					} while (subjects.MoveNext());
				}
			}


			// Add for the main subject:
			if (t.MainSubject == null)
				continue;

			anim = GetAnimation(type);

			t.animParams[i].transition = t;

			// If this animation couldn't start, return it to the pool:
			if (!anim.Start(t.MainSubject, t.animParams[i]))
			{
				ReturnAnimToPool(anim);
			}
			else if(anim.running) // Only add if still running
			{
				// Save a reference to this running animation:
				animNode = t.AddRunningAnim();
				animNode.val = anim;

				// Save a reference to this node for later:
				anim.Data = animNode;
			}
		}
	}

	/// <summary>
	/// Cancels all animations on the specified object.
	/// </summary>
	/// <param name="obj">The object for which all animations should be canceled.</param>
	public void Cancel(System.Object obj)
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; )
		{
			if (i.Current.GetSubject() == obj)
			{
				EZAnimation anim = i.Current;

				if (anim.running)
				{
					anim._cancel();

					animations.Remove(anim);
					ReturnAnimToPool(anim);

					// Don't advance to the next since
					// this already happened when we
					// removed this one:
					continue;
				}
				// Else let the pump dispense with it
				// since it isn't running anymore.
			}

			i.Next();
		}
		i.End();

#if !PUMP_ALWAYS_RUNNING
		if (animations.Empty)
			StopAnimationPump();
#endif
	}

	/// <summary>
	/// Removes the specified animation from the
	/// list of currently running animations.
	/// </summary>
	/// <param name="a">Animation to stop.</param>
	public void StopAnimation(EZAnimation a)
	{
		StopAnimation(a, false);
	}

	/// <summary>
	/// Removes the specified animation from the
	/// list of currently running animations.
	/// </summary>
	/// <param name="a">Animation to stop.</param>
	/// <param name="end">Whether the animation should be placed into
	/// a completed state upon removal.</param>
	public void StopAnimation(EZAnimation a, bool end)
	{
		// See if it's stopped already:
		if (!a.running)
			return;

		if (end)
			a._end();
		else // Else, still call the completion delegate:
			a._stop();

		animations.Remove(a);
		ReturnAnimToPool(a);

		if (animations.Empty)
			StopAnimationPump();
	}

	/// <summary>
	/// Stops all animations on the specified object:
	/// </summary>
	/// <param name="obj">Object for which animation should be stopped.</param>
	public void Stop(System.Object obj)
	{
		Stop(obj, false);
	}

	/// <summary>
	/// Stops all animations on the specified object.
	/// </summary>
	/// <param name="obj">The object for which all animations should cease.</param>
	/// <param name="end">When true, the animation will be set to its end point before stopping.</param>
	public void Stop(System.Object obj, bool end)
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; )
		{
			if (i.Current.GetSubject() == obj)
			{
				EZAnimation anim = i.Current;

				if (anim.running)
				{
					// Make sure the animation completes if 'end' is specified:
					if (end)
						anim._end();
					else
						anim._stop();

					animations.Remove(anim);
					ReturnAnimToPool(anim);

					// Don't advance to the next since
					// this already happened when we
					// removed this one:
					continue;
				}
				// Else let the pump dispense with it
				// since it isn't running anymore.
			}

			i.Next();
		}
		i.End();

#if !PUMP_ALWAYS_RUNNING
		if (animations.Empty)
			StopAnimationPump();
#endif
	}

	/// <summary>
	/// Stops any animations of the specified type on the
	/// specified object.
	/// </summary>
	/// <param name="obj">Object for which animations should be stopped.</param>
	/// <param name="type">Type of animations to stop for the specified object.</param>
	/// <param name="end">Whether the animation should be placed in its end state
	/// before being stopped.</param>
	public void Stop(System.Object obj, EZAnimation.ANIM_TYPE type, bool end)
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; )
		{
			if (i.Current.GetSubject() == obj && i.Current.type == type)
			{
				EZAnimation anim = i.Current;

				if (anim.running)
				{
					// Make sure the animation completes if 'end' is specified:
					if (end)
						anim._end();
					else
						anim._stop();

					animations.Remove(anim);
					ReturnAnimToPool(anim);

					// Don't advance to the next since
					// this already happened when we
					// removed this one:
					continue;
				}
				// Else let the pump dispense with it
				// since it isn't running anymore.
			}

			i.Next();
		}
		i.End();

#if !PUMP_ALWAYS_RUNNING
		if (animations.Empty)
			StopAnimationPump();
#endif
	}

	/// <summary>
	/// Ends animation on an object prematurely:
	/// </summary>
	/// <param name="obj">Object for which any running 
	/// animations will be ended prematurely.</param>
	public void End(System.Object obj)
	{
		Stop(obj, true);
	}

	/// <summary>
	/// Ends all running animation elements, leaving them at
	/// their destination/end states.
	/// </summary>
	public void EndAll()
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; /*i.Next()*/)
		{
			i.Current.End();
		}
		i.End();
	}

	/// <summary>
	/// Stops all running animation elements in their
	/// current state.
	/// </summary>
	public void StopAll()
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; /*i.Next()*/)
		{
			i.Current.Stop();
		}
		i.End();
	}

	/// <summary>
	/// Pauses all running animation elements.
	/// </summary>
	public void PauseAll()
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; i.Next())
		{
			i.Current.Paused = true;
		}
		i.End();
	}

	/// <summary>
	/// Unpauses all animation elements.
	/// </summary>
	public void UnpauseAll()
	{
		EZLinkedListIterator<EZAnimation> i;
		for (i = animations.Begin(); !i.Done; i.Next())
		{
			i.Current.Paused = false;
		}
		i.End();
	}

	public static int GetNumAnimations()
	{
		return animations.Count;
	}
}



/// <remarks>
/// The base class of all EZ animations (also referred to
/// as transition elements when using transitions).
/// The object to be animted by the animation is called the
/// "subject" of the animation.
/// </remarks>
public abstract class EZAnimation : IEZLinkedListItem<EZAnimation>
{
	/// <remarks>
	/// Provides an integral identifier for
	/// each type of animation.
	/// </remarks>
	public enum ANIM_TYPE
	{
		/// <summary>
		/// Plays an AnimationClip.
		/// </summary>
		AnimClip,

		/// <remarks>
		/// Fades a sprite from one color/alpha to another.
		/// </remarks>
		FadeSprite,

		/// <summary>
		/// Fades an object's material from one color/alpha to another.
		/// </summary>
		FadeMaterial,

		/// <summary>
		/// Same as FadeMaterial, but only works on SpriteText objects.
		/// </summary>
		FadeText,

		/// <remarks>
		/// Translates an object from one position to another
		/// </remarks>
		Translate,

		/// <summary>
		/// Move the position by a certain amount, and then back again.
		/// </summary>
		PunchPosition,

		/// <summary>
		/// Shakes an object randomly, as if it had crashed or experienced an explosion.
		/// </summary>
		Crash,

		/// <summary>
		/// Like crash, but moves in a smoother, more connected manner.
		/// </summary>
		SmoothCrash,

		/// <summary>
		/// Shakes an object back and forth at a regular rate and amount.
		/// </summary>
		Shake,
				
		/// <remarks>
		/// Scales an object
		/// </remarks>
		Scale,

		/// <summary>
		/// Move the scale by a certain amount, and then back again.
		/// </summary>
		PunchScale,

		/// <remarks>
		/// Rotates an object about its axis.
		/// </remarks>
		Rotate,

		/// <summary>
		/// Rotate by a certain amount, and then back again.
		/// </summary>
		PunchRotation,

		/// <summary>
		/// Shakes an object by rotating it back and forth by a certain amount on each axis.
		/// </summary>
		ShakeRotation,

		/// <summary>
		/// Shakes an object by rotating it back and forth with diminishing effect over time.
		/// </summary>
		CrashRotation,

		/// <summary>
		/// Fades audio from one volume level to another.
		/// </summary>
		FadeAudio,

		/// <summary>
		/// Transitions audio pitch from one pitch to another.
		/// </summary>
		TuneAudio,

		/// <summary>
		/// Translates an object relative to a part of the screen or another object.
		/// NOTE: Requires that the object in question have an EZScreenPlacement
		/// component attached.  The values of the animation/transition element are
		/// fed into this component's screenPos member.
		/// </summary>
		TranslateScreen,

		/// <remarks>
		/// Fades a sprite from one alpha to another.
		/// </remarks>
		FadeSpriteAlpha,

		/// <remarks>
		/// Fades a SpriteText from one alpha to another.
		/// </remarks>
		FadeTextAlpha,

		/// <summary>
		/// Rotates using euler angles internally instead of a 
		/// quaternion.  This allows you to specify multiple 
		/// revolutions by specifying angles greater than 360, 
		/// but is subject to the limitations of euler angles.
		/// </summary>
		RotateEuler

		/*
		/// <remarks>
		/// Rotates an object about a point
		/// </remarks>
		RotateAround
		*/
	}


	/// <remarks>
	/// The mode of the animation.
	/// i.e. Whether the object being animated will
	/// be moving toward an absolute target, or will
	/// move an amount relative to its current value,
	/// etc.
	/// </remarks>
	public enum ANIM_MODE
	{
		By,			// Move relative to the current value by a given amount
		To,			// Move to an absolute value from the current value
		FromTo		// Move from a specified start value, toward a specified destination value
	}

	/// <summary>
	/// The type of animation this is.
	/// </summary>
	public ANIM_TYPE type;

	/// <remarks>
	/// Definition of a delegate to be called upon completion
	/// </remarks>
	/// <param name="anim">Reference to the EZAnimation-derived class which has completed.</param>
	public delegate void CompletionDelegate(EZAnimation anim);

	/// <summary>
	/// Definition of an interpolator delegate.
	/// Interpolator delegates are used to interpolate
	/// values during an animation.
	/// </summary>
	/// <param name="time">The time elapsed.</param>
	/// <param name="start">The starting value.</param>
	/// <param name="delta">The total amount by which start is to be changed.</param>
	/// <param name="duration">The total time of the animation.</param>
	/// <returns>Should return a value between start (inclusive) and start+delta (inclusive).</returns>
	public delegate float Interpolator(float time, float start, float delta, float duration);

	/// <summary>
	/// Should the transition bounce back and forth when looping?
	/// </summary>
	public bool pingPong = true;

	/// <summary>
	/// Should the delay be repeated when the animation loops?
	/// </summary>
	public bool repeatDelay = false;

	/// <summary>
	/// Should the starting value be reset when a loop is
	/// iterated? (Only has meaning for certain animations,
	/// (such as translation, rotation, fading, etc) and 
	/// among those, only ones which use the "By" mode and 
	/// for which pingPong is false.)
	/// </summary>
	public bool restartOnRepeat = false;

    public bool running = false;                    // Indicates whether this animation is currently running

	protected bool m_paused = false;				// When set to true, the animation will not update

	protected System.Object data;					// Data associated with this animation
	protected ANIM_MODE m_mode;						// The mode of the animation (By, To, etc)
	protected float direction = 1f;					// Used to control the direction of animation
	protected float timeElapsed;					// How long the transition has been running
	protected float wait;							// How long to wait before beginning the animation
	protected float m_wait;							// Where we preserve the 'wait' value for future runs, if necessary.
	protected float duration;						// How long the transition should take (can be negative, indicating infinite looping)
	protected float interval;						// Interval between transition start and end
	protected CompletionDelegate completedDelegate = null; // The delegate to call upon completion of the transition
	protected CompletionDelegate startDelegate = null;
	protected EZAnimation.Interpolator interpolator = null;	// Interpolator to use


	/// <summary>
	/// Accessor for the data associated with this animation.
	/// </summary>
	public System.Object Data
	{
		get { return data; }
		set { data = value; }
	}


	/// <summary>
	/// Returns the desired duration of the animation.
	/// </summary>
	public float Duration
	{
		get { return duration; }
	}

	/// <summary>
	/// Returns the desired delay of the animation.
	/// </summary>
	public float Wait
	{
		get { return wait; }
	}

	/// <summary>
	/// Determines whether the animation is currently paused.
	/// </summary>
	public bool Paused
	{
		get { return m_paused; }
		set { m_paused = (running && value); }
	}

	/// <summary>
	/// Returns the mode of this animation (By, To, etc)
	/// </summary>
	public ANIM_MODE Mode
	{
		get { return m_mode; }
	}

	/// <summary>
	/// Accessor for the delegate to be called upon
	/// completion of the animation.
	/// </summary>
	public CompletionDelegate CompletedDelegate
	{
		get { return completedDelegate; }
		set { completedDelegate = value; }
	}

	/// <summary>
	/// Accessor for the delegate to be called as
	/// soon as the animation begins (the delay 
	/// expires).
	/// NOTE: For looping animations, this will be
	/// called upon each iteration.
	/// </summary>
	public CompletionDelegate StartDelegate
	{
		get { return startDelegate; }
		set { startDelegate = value; }
	}

	// Clears out important values that should always
	// be cleared between uses.  This is called when
	// an animation is returned to the free pool.
	public void Clear()
	{
		completedDelegate = null;
		startDelegate = null;
		data = null;
	}

	/// <summary>
	/// Starts an animation.
	/// </summary>
	/// <param name="sub">The subject of the animation.</param>
	/// <param name="parms">Parameters for the animation.</param>
	/// <returns>True if start succeeded. False if not.</returns>
	public abstract bool Start(GameObject sub, AnimParams parms);

	// Does the work of the transition, frame-to-frame:
	protected abstract void DoAnim();

	// Steps the animation:
	public virtual bool Step(float timeDelta)
	{
		if (m_paused)
			return true;

		if (wait > 0)
		{
			wait -= timeDelta;

			// If we're done waiting:
			if (wait < 0)
			{
				if (startDelegate != null)
					startDelegate(this);

				// Take the overage off our timeDelta:
				timeDelta -= (timeDelta + wait);

				// Call our method that handles things when the wait is done:
				WaitDone();
			}
			else
				return true;
		}

		timeElapsed += timeDelta * direction;

		if (timeElapsed >= interval || timeElapsed < 0)
		{
			// If this is supposed to loop forever,
			if (duration < 0)
			{
				if (pingPong)
				{
					// Reverse direction and keep going:
					if (timeElapsed >= interval)
					{
						direction = -1f;
						timeElapsed = interval - (timeElapsed - interval); // Subtract the overage from the timeElapsed
					}
					else
					{
						if (repeatDelay)
						{
							wait = m_wait - (timeElapsed - interval); // Subtract the overage from the wait time
						}
						else
						{
							if (startDelegate != null)
								startDelegate(this);

							timeElapsed *= -1f; // Convert our negative overage to positive time elapsed
						}

						direction = 1f;
					}
				}
				else
				{
					// Reset timeElapsed and keep going:
					if (repeatDelay)
						wait = m_wait;
					else if (startDelegate != null)
						startDelegate(this);

					LoopReset();

					timeElapsed -= interval;
				}
			}
			else
			{
				_end();

				return false;
			}
		}

		DoAnim();

		return true;
	}

	/// <summary>
	/// Stops the animation.
	/// </summary>
	public virtual void Stop()
	{
		EZAnimator.instance.StopAnimation(this);
	}

	// Calls the completedDelegate, if any.
	public void _stop()
	{
		running = false;
		Paused = false;
		if (completedDelegate != null)
			completedDelegate(this);
	}

	/// <summary>
	/// Ends the animation prematurely, setting
	/// the subject to its end state.
	/// </summary>
	public void End()
	{
		// _end() will be called by StopAnimation():
		EZAnimator.instance.StopAnimation(this, true);
	}

	// Cancels the animation from running without
	// calling the completedDelegate:
	public void _cancel()
	{
		running = false;
		Clear();
	}

	// Sets the subject to its end state
	// and calls the completedDelegate,
	// if any.
	public virtual void _end()
	{
		_stop();
	}

	// Resets the animation for another
	// loop iteration.
	protected virtual void LoopReset()	{}

	/// <summary>
	/// Returns the subject of the animation.
	/// </summary>
	/// <returns>Returns a reference to the subject of the animation.</returns>
	public abstract System.Object GetSubject();

	// Can be overridden by subclasses to do stuff
	// that needs to be done as soon as the delay/wait
	// is finished elapsing:
	protected virtual void WaitDone()
	{
	}


	// Performs common starting operations
	protected void StartCommon()
	{
		wait = m_wait;
		
		if (wait == 0)
			if (startDelegate != null)
				startDelegate(this);

		interval = Mathf.Abs(duration);

		direction = 1f;
		timeElapsed = 0;
		Paused = false;
	}

	
	// Called when an animation is returned to the pool:
	public void ResetDefaults()
	{
		pingPong = true;
		restartOnRepeat = false;
		data = null;
		completedDelegate = null;
		startDelegate = null;
	}


	/// <remarks>
	/// Integral identifier for each easing type.
	/// </remarks>
	public enum EASING_TYPE
	{
		Default = -1,
		Linear,
		BackIn,
		BackOut,
		BackInOut,
		BackOutIn,
		BounceIn,
		BounceOut,
		BounceInOut,
		BounceOutIn,
		CircularIn,
		CircularOut,
		CircularInOut,
		CircularOutIn,
		CubicIn,
		CubicOut,
		CubicInOut,
		CubicOutIn,
		ElasticIn,
		ElasticOut,
		ElasticInOut,
		ElasticOutIn,
		ExponentialIn,
		ExponentialOut,
		ExponentialInOut,
		ExponentialOutIn,
		QuadraticIn,
		QuadraticOut,
		QuadraticInOut,
		QuadraticOutIn,
		QuarticIn,
		QuarticOut,
		QuarticInOut,
		QuarticOutIn,
		QuinticIn,
		QuinticOut,
		QuinticInOut,
		QuinticOutIn,
		SinusoidalIn,
		SinusoidalOut,
		SinusoidalInOut,
		SinusoidalOutIn,
		Spring
	}

	// Interpolation routines:

	// No easing
	public static float linear(float time, float start, float delta, float duration)
	{ return delta * time / duration + start; }
	
	// Easing equation function for a quadratic (t^2) easing in: accelerating from zero velocity.
	public static float quadraticIn(float time, float start, float delta, float duration)
	{ time /= duration; return delta * time * time + start; }
	// Easing equation function for a quadratic (t^2) easing out: decelerating to zero velocity.
	public static float quadraticOut(float time, float start, float delta, float duration)
	{ time /= duration; return -delta * time * (time - 2f) + start; }
	// In/Out
	public static float quadraticInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < duration / 2f)
			return delta / 2f * time * time + start;
		--time;
		return -delta / 2f * (time * (time - 2f) - 1f) + start;
	}
	// Out-In:
	public static float quadraticOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return quadraticOut(time * 2f, start, delta / 2f, duration);
		return quadraticIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Easing equation function for a cubic (t^3) easing in: accelerating from zero velocity.
	public static float cubicIn(float time, float start, float delta, float duration)
	{ time /= duration; return delta * time * time * time + start; }
	// Easing equation function for a cubic (t^3) easing out: decelerating from zero velocity.
	public static float cubicOut(float time, float start, float delta, float duration)
	{ time /= duration; --time; return delta * (time * time * time + 1f) + start; }
	// In/Out
	public static float cubicInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
			return delta / 2f * time * time * time + start;
		time -= 2f;
		return delta / 2f * (time * time * time + 2f) + start;
	}
	// Out-In:
	public static float cubicOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return cubicOut(time * 2f, start, delta / 2f, duration);
		return cubicIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Easing equation function for a quartic (t^4) easing in: accelerating from zero velocity.
	public static float quarticIn(float time, float start, float delta, float duration)
	{ time /= duration; return delta * time * time * time * time + start; }
	// Easing equation function for a quartic (t^4) easing in: decelerating from zero velocity.
	public static float quarticOut(float time, float start, float delta, float duration)
	{ time /= duration; time--; return -delta * (time * time * time * time - 1) + start; }
	// Accelerate, then decelerate
	public static float quarticInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
			return delta / 2f * time * time * time * time + start;
		time -= 2f;
		return -delta / 2f * (time * time * time * time - 2f) + start;
	}
	// Out-In:
	public static float quarticOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return quarticOut(time * 2f, start, delta / 2f, duration);
		return quarticIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Easing equation function for a quintic (t^5) easing in: accelerating from zero velocity.
	public static float quinticIn(float time, float start, float delta, float duration)
	{ time /= duration; return delta * time * time * time * time * time + start; }
	// Easing equation function for a quintic (t^5) easing in: decelerating from zero velocity.
	public static float quinticOut(float time, float start, float delta, float duration)
	{ time /= duration; time--; return delta * (time * time * time * time * time + 1f) + start; }
	// Easing equation function for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
	public static float quinticInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
			return delta / 2f * time * time * time * time * time + start;
		time -= 2f;
		return delta / 2f * (time * time * time * time * time + 2f) + start;
	}
	// Easing equation function for a quintic (t^5) easing out/in: deceleration until halfway, then acceleration.
	public static float quinticOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return quinticOut(time * 2f, start, delta / 2f, duration);
		return quinticIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
	public static float sinusIn(float time, float start, float delta, float duration)
	{ return -delta * Mathf.Cos(time / duration * (Mathf.PI / 2f)) + delta + start; }
	// Easing equation function for a sinusoidal (sin(t)) easing in: decelerating from zero velocity.
	public static float sinusOut(float time, float start, float delta, float duration)
	{ return delta * Mathf.Sin(time / duration * (Mathf.PI / 2f)) + start; }
	// Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
	public static float sinusInOut(float time, float start, float delta, float duration)
	{ return -delta / 2f * (Mathf.Cos(Mathf.PI * time / duration) - 1f) + start; }
	// Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
	public static float sinusOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return sinusOut(time * 2f, start, delta / 2f, duration);
		return sinusIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.
	public static float expIn(float time, float start, float delta, float duration)
	{ return delta * Mathf.Pow(2f, 10f * (time / duration - 1f)) + start; }
	// Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.
	public static float expOut(float time, float start, float delta, float duration)
	{ return delta * (-Mathf.Pow(2f, -10f * time / duration) + 1f) + start; }
	// Accelerate, then decelerate
	public static float expInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
			return delta / 2f * Mathf.Pow(2f, 10f * (time - 1f)) + start;
		time--;
		return delta / 2f * (-Mathf.Pow(2f, -10f * time) + 2f) + start;
	}
	// Decelerate, then accelerate
	public static float expOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return expOut(time * 2f, start, delta / 2f, duration);
		return expIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
	public static float circIn(float time, float start, float delta, float duration)
	{ time /= duration; return -delta * (Mathf.Sqrt(1f - time * time) - 1f) + start; }
	// Easing equation function for a circular (sqrt(1-t^2)) easing in: decelerating from zero velocity.
	public static float circOut(float time, float start, float delta, float duration)
	{ time /= duration; time--; return delta * Mathf.Sqrt(1f - time * time) + start; }
	// Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
	public static float circInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
			return -delta / 2f * (Mathf.Sqrt(1f - time * time) - 1f) + start;
		time -= 2f;
		return delta / 2f * (Mathf.Sqrt(1f - time * time) + 1f) + start;
	}
	// Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
	public static float circOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return circOut(time * 2f, start, delta / 2f, duration);
		return circIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}

	// Moves a value a certain distance away from its origin, then returns to it.
	public static float punch(float amplitude, float value)
    {
        float s = 9f;

		if (value == 0)
            return 0;
        if (value == 1f)
            return 0;

        float period = 1f * 0.3f;
        s = period / (2f * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - s) * (2f * Mathf.PI) / period));
    }

	// Springy easing
	public static float spring(float time, float start, float delta, float duration)
	{
		float value = time / duration;
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + delta * value;
	}

	/*
	Disclaimer for Robert Penner's Easing Equations license:

	TERMS OF USE - EASING EQUATIONS

	Open source under the BSD License.

	Copyright 2001 Robert Penner
	All rights reserved.

	Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

		* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		* Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	*/

	// Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
	// Default args:
	public static float elasticIn(float time, float start, float delta, float duration)
	{ return elasticIn(time, start, delta, duration, 0, duration*0.3f); }
	// Full args:
	public static float elasticIn(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time == 0)	return start;
        if (delta == 0) return start;
        if ((time /= duration) == 1f) return start + delta;
		float s;
		if(amplitude < Mathf.Abs(delta))
		{
			amplitude = delta;
			s = period / 4f;
		}
		else
		{
			s = period / (2f * Mathf.PI) * Mathf.Asin(delta / amplitude);
		}
		return -(amplitude * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / period)) + start;
	}
	// Easing equation function for a circular (sqrt(1-t^2)) easing in: decelerating from zero velocity.
	// Default args:
	public static float elasticOut(float time, float start, float delta, float duration)
	{ return elasticOut(time, start, delta, duration, 0, duration * 0.3f); }
	// Full args:
	public static float elasticOut(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time == 0)	return start;
        if (delta == 0) return start;
		if ((time /= duration) == 1f) return start + delta;
		float s;
		if(amplitude < Mathf.Abs(delta))
		{
			amplitude = delta;
			s = period / 4f;
		}
		else
		{
			s = period / (2f * Mathf.PI) * Mathf.Asin(delta / amplitude);
		}
		return (amplitude * Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / period) + delta + start);
	}
	// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
	// Default args:
	public static float elasticInOut(float time, float start, float delta, float duration)
	{ return elasticInOut(time, start, delta, duration, 0, duration * 0.3f * 1.5f); }
	// Full args:
	public static float elasticInOut(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time==0) return start;
        if (delta == 0) return start;
		if ((time/=duration/2f)==2f) return start+delta;
		float s;
		if (amplitude < Mathf.Abs(delta)) {
			amplitude = delta;
			s = period/4f;
		} else {
			s = period/(2f*Mathf.PI) * Mathf.Asin (delta/amplitude);
		}
		if (time < 1f) return -.5f*(amplitude*Mathf.Pow(2f,10f*(time-=1f)) * Mathf.Sin( (time*duration-s)*(2f*Mathf.PI)/period )) + start;
		return amplitude*Mathf.Pow(2f,-10f*(time-=1f)) * Mathf.Sin( (time*duration-s)*(2f*Mathf.PI)/period )*.5f + delta + start;
	}
	// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway, then acceleration.
	// Default args:
	public static float elasticOutIn(float time, float start, float delta, float duration)
	{ return elasticOutIn(time, start, delta, duration, 0, duration * 0.3f); }
	// Full args:
	public static float elasticOutIn(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time < duration / 2f) return elasticOut(time * 2f, start, delta / 2f, duration, amplitude, period);
		return elasticIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration, amplitude, period);
	}

	// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
	// Overshoot amount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	// Default args:
	public static float backIn(float time, float start, float delta, float duration)
	{ return backIn(time, start, delta, duration, 1.70158f); }
	// Full args:
	public static float backIn(float time, float start, float delta, float duration, float overshootAmt)
	{	return delta*(time/=duration)*time*((overshootAmt+1f)*time - overshootAmt) + start;	}
	// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: decelerating from zero velocity.
	// Overshoot amount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	// Default args:
	public static float backOut(float time, float start, float delta, float duration)
	{ return backOut(time, start, delta, duration, 1.70158f); }
	// Full args:
	public static float backOut(float time, float start, float delta, float duration, float overshootAmt)
	{	return delta * ((time = time / duration - 1f) * time * ((overshootAmt + 1f) * time + overshootAmt) + 1f) + start;	}
	// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.
	// Overshoot amount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	// Default args:
	public static float backInOut(float time, float start, float delta, float duration)
	{ return backInOut(time, start, delta, duration, 1.70158f); }
	// Full args:
	public static float backInOut(float time, float start, float delta, float duration, float overshootAmt)
	{
		if ((time /= duration / 2f) < 1f) return delta / 2f * (time * time * (((overshootAmt *= (1.525f)) + 1f) * time - overshootAmt)) + start;
		return delta / 2f * ((time -= 2f) * time * (((overshootAmt *= (1.525f)) + 1f) * time + overshootAmt) + 2f) + start;
	}
	// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway, then acceleration.
	// Overshoot amount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	// Default args:
	public static float backOutIn(float time, float start, float delta, float duration)
	{ return backOutIn(time, start, delta, duration, 1.70158f); }
	// Full args:
	public static float backOutIn(float time, float start, float delta, float duration, float overshootAmt)
	{
		if (time < duration / 2f) return backOut(time * 2f, start, delta / 2f, duration, overshootAmt);
		return backIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration, overshootAmt);
	}

	// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
	public static float bounceIn(float time, float start, float delta, float duration)
	{	return delta - bounceOut(duration - time, 0, delta, duration) + start;	}
	// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
	public static float bounceOut(float time, float start, float delta, float duration)
	{
		if ((time /= duration) < (1f / 2.75f))
		{
			return delta * (7.5625f * time * time) + start;
		}
		else if (time < (2f / 2.75f))
		{
			return delta * (7.5625f * (time -= (1.5f / 2.75f)) * time + .75f) + start;
		}
		else if (time < (2.5f / 2.75f))
		{
			return delta * (7.5625f * (time -= (2.25f / 2.75f)) * time + .9375f) + start;
		}
		else
		{
			return delta * (7.5625f * (time -= (2.625f / 2.75f)) * time + .984375f) + start;
		}
	}
	// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
	public static float bounceInOut(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return bounceIn(time * 2f, 0, delta, duration) * .5f + start;
		else return bounceOut(time * 2f - duration, 0, delta, duration) * .5f + delta * .5f + start;
	}
	// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway, then acceleration.
	public static float bounceOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f) return bounceOut(time * 2f, start, delta / 2f, duration);
		return bounceIn((time * 2f) - duration, start + delta / 2f, delta / 2f, duration);
	}


	/// <summary>
	/// Returns the interpolation/easing function 
	/// to which the specified enum refers:
	/// </summary>
	/// <param name="type">Enum of the desired interpolator.</param>
	/// <returns>Retruns a reference to the desired interpolator method/delegate.</returns>
	public static EZAnimation.Interpolator GetInterpolator(EZAnimation.EASING_TYPE type)
	{
		switch(type)
		{
			case EASING_TYPE.BackIn:
				return backIn;
			case EASING_TYPE.BackInOut:
				return backInOut;
			case EASING_TYPE.BackOut:
				return backOut;
			case EASING_TYPE.BackOutIn:
				return backOutIn;
			case EASING_TYPE.BounceIn:
				return bounceIn;
			case EASING_TYPE.BounceInOut:
				return bounceInOut;
			case EASING_TYPE.BounceOut:
				return bounceOut;
			case EASING_TYPE.BounceOutIn:
				return bounceOutIn;
			case EASING_TYPE.CircularIn:
				return circIn;
			case EASING_TYPE.CircularInOut:
				return circInOut;
			case EASING_TYPE.CircularOut:
				return circOut;
			case EASING_TYPE.CircularOutIn:
				return circOutIn;
			case EASING_TYPE.CubicIn:
				return cubicIn;
			case EASING_TYPE.CubicInOut:
				return cubicInOut;
			case EASING_TYPE.CubicOut:
				return cubicOut;
			case EASING_TYPE.CubicOutIn:
				return cubicOutIn;
			case EASING_TYPE.ElasticIn:
				return elasticIn;
			case EASING_TYPE.ElasticInOut:
				return elasticInOut;
			case EASING_TYPE.ElasticOut:
				return elasticOut;
			case EASING_TYPE.ElasticOutIn:
				return elasticOutIn;
			case EASING_TYPE.ExponentialIn:
				return expIn;
			case EASING_TYPE.ExponentialInOut:
				return expInOut;
			case EASING_TYPE.ExponentialOut:
				return expOut;
			case EASING_TYPE.ExponentialOutIn:
				return expOutIn;
			case EASING_TYPE.Linear:
				return linear;
			case EASING_TYPE.QuadraticIn:
				return quadraticIn;
			case EASING_TYPE.QuadraticInOut:
				return quadraticInOut;
			case EASING_TYPE.QuadraticOut:
				return quadraticOut;
			case EASING_TYPE.QuadraticOutIn:
				return quadraticOutIn;
			case EASING_TYPE.QuarticIn:
				return quarticIn;
			case EASING_TYPE.QuarticInOut:
				return quarticInOut;
			case EASING_TYPE.QuarticOut:
				return quarticOut;
			case EASING_TYPE.QuarticOutIn:
				return quarticOutIn;
			case EASING_TYPE.QuinticIn:
				return quinticIn;
			case EASING_TYPE.QuinticInOut:
				return quinticInOut;
			case EASING_TYPE.QuinticOut:
				return quinticOut;
			case EASING_TYPE.QuinticOutIn:
				return quinticOutIn;
			case EASING_TYPE.SinusoidalIn:
				return sinusIn;
			case EASING_TYPE.SinusoidalInOut:
				return sinusInOut;
			case EASING_TYPE.SinusoidalOut:
				return sinusOut;
			case EASING_TYPE.SinusoidalOutIn:
				return sinusOutIn;
			case EASING_TYPE.Spring:
				return spring;
			default:
				return linear;
		}
	}


	// Implementation of our linked list interface:
	protected EZAnimation m_prev;
	public EZAnimation prev
	{
		get { return m_prev; }
		set { m_prev = value; }
	}

	protected EZAnimation m_next;
	public EZAnimation next
	{
		get { return m_next; }
		set { m_next = value; }
	}
}


/// <remarks>
/// Fades a sprite or EZ GUI control from one color to another.
/// NOTE: Only one FadeSprite can be active on a sprite at a time.
/// If a FadeSprite animation is started on an object that already
/// has one active, the previous one will be stopped.
/// </remarks>
public class FadeSprite : EZAnimation
{
	protected Color start;			// Our starting color
	protected Color delta;			// Color change
	protected Color end;			// Destination color
	protected SpriteRoot sprite;		// Reference to the subject
	protected Color temp;


	public override object GetSubject()
	{
		return sprite;
	}

	public override void _end()
	{
		if(sprite != null)
			sprite.SetColor(end);

		base._end();
	}

	protected override void LoopReset()
	{
		if(Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && sprite != null)
		{
			// Get the most up-to-date state starting:
			start = sprite.color;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (sprite == null)
		{
			_stop();
			return;
		}

		temp.r = interpolator(timeElapsed, start.r, delta.r, interval);
		temp.g = interpolator(timeElapsed, start.g, delta.g, interval);
		temp.b = interpolator(timeElapsed, start.b, delta.b, interval);
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);

		sprite.SetColor(temp);
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeSprite Do(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSprite anim = (FadeSprite)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSprite);
		anim.Start(sprt, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeSprite Do(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSprite anim = (FadeSprite)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSprite);
		anim.Start(sprt, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		sprite = (SpriteRoot) sub.GetComponent(typeof(SpriteRoot));
		if (sprite == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sprite, parms.mode, parms.color, parms.color2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sprite, parms.mode, sprite.color, parms.color, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(sprt, mode, sprt.color, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		sprite = sprt;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = new Color(dest.r - start.r, dest.g - start.g, dest.b - start.b, dest.a - start.a);

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		//EZAnimator.instance.Stop(sprite, type, (mode == ANIM_MODE.By)?true:false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			sprite.SetColor(start);
		
		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (sprite == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;
		
		if(m_mode == ANIM_MODE.By)
		{
			start = sprite.color;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

    public FadeSprite() 
	{
		type = ANIM_TYPE.FadeSprite;
	}
}

/// <remarks>
/// Fades a sprite or EZ GUI control from one alpha value to another.
/// NOTE: Only one FadeSpriteAlpha can be active on a sprite at a time.
/// If a FadeSpriteAlpha animation is started on an object that already
/// has one active, the previous one will be stopped.
/// </remarks>
public class FadeSpriteAlpha : EZAnimation
{
	protected Color start;			// Our starting color
	protected Color delta;			// Color change
	protected Color end;			// Destination color
	protected SpriteRoot sprite;		// Reference to the subject
	protected Color temp;


	public override object GetSubject()
	{
		return sprite;
	}

	public override void _end()
	{
		if (sprite != null)
			sprite.SetColor(end);

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && sprite != null)
		{
			// Get the most up-to-date state starting:
			start = sprite.color;
			end = start + delta;
		}

		temp = start;
	}

	protected override void DoAnim()
	{
		if (sprite == null)
		{
			_stop();
			return;
		}

		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);

		sprite.SetColor(temp);
	}

	/// <summary>
	/// Starts a FadeSpriteAlpha animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeSpriteAlpha Do(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSpriteAlpha anim = (FadeSpriteAlpha)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSpriteAlpha);
		anim.Start(sprt, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts a FadeSpriteAlpha animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeSpriteAlpha Do(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSpriteAlpha anim = (FadeSpriteAlpha)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSpriteAlpha);
		anim.Start(sprt, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		sprite = (SpriteRoot)sub.GetComponent(typeof(SpriteRoot));
		if (sprite == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sprite, parms.mode, parms.color, parms.color2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sprite, parms.mode, sprite.color, parms.color, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts a FadeSpriteAlpha animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(sprt, mode, sprt.color, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a FadeSpriteAlpha animation on the specified subject.
	/// </summary>
	/// <param name="sprt">The sprite to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		sprite = sprt;
		start = sprite.Color;
		start.a = begin.a;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = new Color(0, 0, 0, dest.a);
		else
			delta = new Color(0, 0, 0, dest.a - start.a);

		end = start + delta;
		temp = start;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		//EZAnimator.instance.Stop(sprite, type, (mode == ANIM_MODE.By)?true:false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			sprite.SetColor(start);

		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (sprite == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = sprite.color;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public FadeSpriteAlpha()
	{
		type = ANIM_TYPE.FadeSpriteAlpha;
	}
}

/// <remarks>
/// Fades a material from one color to another.
/// NOTE: Only one FadeMaterial can be active on a material at a time.
/// If a FadeMaterial animation is started on an object that already
/// has one active, the previous one will be stopped.
/// NOTE: Modifying an object's material at runtime will
/// cause it not to be batched with other objects in Unity iPhone.
/// </remarks>
public class FadeMaterial : EZAnimation
{
	protected Color start;			// Our starting color
	protected Color delta;			// Color change
	protected Color end;			// Destination color
	protected Material mat;			// Reference to the subject
	protected Color temp;


	public override object GetSubject()
	{
		return mat;
	}

	public override void _end()
	{
		if(mat != null)
			mat.color = end;

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && mat != null)
		{
			// Get the most up-to-date state starting:
			start = mat.color;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (mat == null)
		{
			_stop();
			return;
		}

		temp.r = interpolator(timeElapsed, start.r, delta.r, interval);
		temp.g = interpolator(timeElapsed, start.g, delta.g, interval);
		temp.b = interpolator(timeElapsed, start.b, delta.b, interval);
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);

		mat.color = temp;
	}

	/// <summary>
	/// Starts a FadeMaterial animation on the specified subject.
	/// </summary>
	/// <param name="material">The material to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeMaterial Do(Material material, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeMaterial anim = (FadeMaterial)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeMaterial);
		anim.Start(material, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts a FadeMaterial animation on the specified subject.
	/// </summary>
	/// <param name="material">The material to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeMaterial Do(Material material, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeMaterial anim = (FadeMaterial)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeMaterial);
		anim.Start(material, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;
		if(sub.renderer == null)
			return false;
		if(sub.renderer.material == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sub.renderer.material, parms.mode, parms.color, parms.color2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sub.renderer.material, parms.mode, sub.renderer.material.color, parms.color, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts a FadeMaterial animation on the specified subject.
	/// </summary>
	/// <param name="material">The material to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(Material material, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(material, mode, material.color, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a FadeMaterial animation on the specified subject.
	/// </summary>
	/// <param name="material">The material to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(Material material, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		mat = material;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = new Color(dest.r - start.r, dest.g - start.g, dest.b - start.b, dest.a - start.a);

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		//EZAnimator.instance.Stop(mat, type, (mode == ANIM_MODE.By) ? true : false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			mat.color = start;
		
		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (mat == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = mat.color;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public FadeMaterial()
	{
		type = ANIM_TYPE.FadeMaterial;
	}
}

/// <remarks>
/// Fades a SpriteText object from one color to another.
/// NOTE: Only one FadeText can be active on a SpriteText at a time.
/// If a FadeText animation is started on an object that already
/// has one active, the previous one will be stopped.
/// </remarks>
public class FadeText : EZAnimation
{
	protected Color start;			// Our starting color
	protected Color delta;			// Color change
	protected Color end;			// Destination color
	protected SpriteText text;	// Reference to the subject
	protected Color temp;


	public override object GetSubject()
	{
		return text;
	}

	public override void _end()
	{
		if(text != null)
			text.SetColor(end);

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && text != null)
		{
			// Get the most up-to-date state starting:
			start = text.color;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (text == null)
		{
			_stop();
			return;
		}

		temp.r = interpolator(timeElapsed, start.r, delta.r, interval);
		temp.g = interpolator(timeElapsed, start.g, delta.g, interval);
		temp.b = interpolator(timeElapsed, start.b, delta.b, interval);
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);

		text.SetColor(temp);
	}


	/// <summary>
	/// Starts a FadeText animation on the specified subject.
	/// </summary>
	/// <param name="txt">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeText Do(SpriteText txt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeText anim = (FadeText)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeText);
		anim.Start(txt, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts a FadeText animation on the specified subject.
	/// </summary>
	/// <param name="txt">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeText Do(SpriteText txt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeText anim = (FadeText)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeText);
		anim.Start(txt, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;
		text = (SpriteText) sub.GetComponent(typeof(SpriteText));
		if (text == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(text, parms.mode, parms.color, parms.color2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(text, parms.mode, text.color, parms.color, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="text">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteText txt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(txt, mode, txt.color, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="text">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteText txt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		text = txt;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = new Color(dest.r - start.r, dest.g - start.g, dest.b - start.b, dest.a - start.a);

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		//EZAnimator.instance.Stop(text, type, (mode == ANIM_MODE.By)?true:false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			text.SetColor(start);

		EZAnimator.instance.AddAnimation(this);
	}

    public FadeText() 
	{
		type = ANIM_TYPE.FadeText;
	}
}


/// <remarks>
/// Fades a SpriteText object from one alpha value to another.
/// NOTE: Only one FadeTextAlpha can be active on a SpriteText at a time.
/// If a FadeTextAlpha animation is started on an object that already
/// has one active, the previous one will be stopped.
/// </remarks>
public class FadeTextAlpha : EZAnimation
{
	protected Color start;			// Our starting color
	protected Color delta;			// Color change
	protected Color end;			// Destination color
	protected SpriteText text;	// Reference to the subject
	protected Color temp;


	public override object GetSubject()
	{
		return text;
	}

	public override void _end()
	{
		if (text != null)
			text.SetColor(end);

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && text != null)
		{
			// Get the most up-to-date state starting:
			start = text.color;
			end = start + delta;
		}

		temp = start;
	}

	protected override void DoAnim()
	{
		if (text == null)
		{
			_stop();
			return;
		}

		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);

		text.SetColor(temp);
	}


	/// <summary>
	/// Starts a FadeTextAlpha animation on the specified subject.
	/// </summary>
	/// <param name="txt">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeTextAlpha Do(SpriteText txt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeTextAlpha anim = (FadeTextAlpha)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeTextAlpha);
		anim.Start(txt, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts a FadeTextAlpha animation on the specified subject.
	/// </summary>
	/// <param name="txt">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeTextAlpha Do(SpriteText txt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeTextAlpha anim = (FadeTextAlpha)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeTextAlpha);
		anim.Start(txt, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;
		text = (SpriteText)sub.GetComponent(typeof(SpriteText));
		if (text == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(text, parms.mode, parms.color, parms.color2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(text, parms.mode, text.color, parms.color, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="text">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteText txt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(txt, mode, txt.color, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a FadeSprite animation on the specified subject.
	/// </summary>
	/// <param name="text">The SpriteText object to fade.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting color.</param>
	/// <param name="dest">The destination color.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(SpriteText txt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		text = txt;
		start = txt.Color;
		start.a = begin.a;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = new Color(0, 0, 0, dest.a);
		else
			delta = new Color(0, 0, 0, dest.a - start.a);

		end = start + delta;
		temp = start;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		//EZAnimator.instance.Stop(text, type, (mode == ANIM_MODE.By)?true:false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			text.SetColor(start);

		EZAnimator.instance.AddAnimation(this);
	}

	public FadeTextAlpha()
	{
		type = ANIM_TYPE.FadeTextAlpha;
	}
}


/// <remarks>
/// Rotates an object's transform.
/// </remarks>
public class AnimateRotation : EZAnimation
{
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;

	protected Quaternion temp;
	protected Quaternion delta;
	protected Quaternion start;
	protected Quaternion end;


	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localRotation = end;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end.x = start.x + delta.x;
			end.y = start.y + delta.y;
			end.z = start.z + delta.z;
			end.w = start.w + delta.w;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subTrans != null)
		{
			// Get the most up-to-date state starting:
			start = subTrans.localRotation;
			end.x = start.x + delta.x;
			end.y = start.y + delta.y;
			end.z = start.z + delta.z;
			end.w = start.w + delta.w;
		}
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		temp = Quaternion.Slerp(start, end, interpolator(timeElapsed, 0, 1f, interval));

		subTrans.localRotation = temp;
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts an AnimateRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting rotation.</param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateRotation Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateRotation anim = (AnimateRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.Rotate);
		anim.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts an AnimateRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateRotation Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateRotation anim = (AnimateRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.Rotate);
		anim.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sub, parms.mode, parms.vec, parms.vec2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sub, parms.mode, sub.transform.localEulerAngles, parms.vec, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts an AnimateRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, mode, sub.transform.localEulerAngles, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts an AnimateRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting rotation.</param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = Quaternion.Euler(begin);

		m_mode = mode;

		if (mode == ANIM_MODE.By)
		{
			Quaternion destQ = Quaternion.Euler(begin + dest);
			delta = new Quaternion(destQ.x - start.x, destQ.y - start.y, destQ.z - start.z, destQ.w - start.w);
		}
		else
		{
			Quaternion destQ = Quaternion.Euler(dest);
			delta = new Quaternion(destQ.x - start.x, destQ.y - start.y, destQ.z - start.z, destQ.w - start.w);
		}

		end.x = start.x + delta.x;
		end.y = start.y + delta.y;
		end.z = start.z + delta.z;
		end.w = start.w + delta.w;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subTrans.localRotation = start;

		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (subject == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = subject.transform.localRotation;
			end.x = start.x + delta.x;
			end.y = start.y + delta.y;
			end.z = start.z + delta.z;
			end.w = start.w + delta.w;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public AnimateRotation()
	{
		type = ANIM_TYPE.Rotate;
	}
}


/// <remarks>
/// Rotates using euler angles internally instead of a 
/// quaternion.  This allows you to specify multiple 
/// revolutions by specifying angles greater than 360, 
/// but is subject to the limitations of euler angles.
/// </remarks>
public class AnimateRotationEuler : EZAnimation
{
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;
	protected Vector3 start;		// Our starting value
	protected Vector3 delta;		// Amount to change
	protected Vector3 end;			// Destination value
	protected Vector3 temp;


	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localEulerAngles = end;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subTrans != null)
		{
			// Get the most up-to-date state starting:
			start = subTrans.localEulerAngles;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		temp.x = interpolator(timeElapsed, start.x, delta.x, interval);
		temp.y = interpolator(timeElapsed, start.y, delta.y, interval);
		temp.z = interpolator(timeElapsed, start.z, delta.z, interval);

		subTrans.localEulerAngles = temp;
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts an AnimateRotationEuler animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting rotation.</param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateRotationEuler Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateRotationEuler anim = (AnimateRotationEuler)EZAnimator.instance.GetAnimation(ANIM_TYPE.RotateEuler);
		anim.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts an AnimateRotationEuler animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateRotationEuler Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateRotationEuler anim = (AnimateRotationEuler)EZAnimator.instance.GetAnimation(ANIM_TYPE.RotateEuler);
		anim.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sub, parms.mode, parms.vec, parms.vec2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sub, parms.mode, sub.transform.localEulerAngles, parms.vec, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts an AnimateRotationEuler animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, mode, sub.transform.localEulerAngles, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts an AnimateRotationEuler animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to rotate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting rotation.</param>
	/// <param name="dest">The destination rotation.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = new Vector3(dest.x - start.x, dest.y - start.y, dest.z - start.z);

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subTrans.localEulerAngles = start;

		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (subject == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = subject.transform.localEulerAngles;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public AnimateRotationEuler()
	{
		type = ANIM_TYPE.RotateEuler;
	}
}



/// <remarks>
/// Animates an object's position in local space.
/// (Modifies transform.localPosition)
/// </remarks>
public class AnimatePosition : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 delta;		// Amount to change
	protected Vector3 end;			// Destination value
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;
	protected Vector3 temp;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localPosition = end;
			subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subTrans != null)
		{
			// Get the most up-to-date state starting:
			start = subTrans.localPosition;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		temp.x = interpolator(timeElapsed, start.x, delta.x, interval);
		temp.y = interpolator(timeElapsed, start.y, delta.y, interval);
		temp.z = interpolator(timeElapsed, start.z, delta.z, interval);

		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting position.</param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimatePosition Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimatePosition anim = (AnimatePosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.Translate);
		anim.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimatePosition Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimatePosition anim = (AnimatePosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.Translate);
		anim.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sub, parms.mode, parms.vec, parms.vec2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sub, parms.mode, sub.transform.localPosition, parms.vec, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, mode, sub.transform.localPosition, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting position.</param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = dest - start;

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subTrans.localPosition = start;

		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (subject == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = subject.transform.localPosition;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public AnimatePosition()
	{
		type = ANIM_TYPE.Translate;
	}
}


/// <remarks>
/// Animates an object's position in screen space.
/// (Modifies EZScreenPlacement.screenPos).
/// NOTE: This currently calls PositionOnScreen() every frame so it might hurt performance.
/// </remarks>
public class AnimateScreenPosition : EZAnimation
{
	protected Vector3 start;      // Our starting value
	protected Vector3 delta;      // Amount to change
	protected Vector3 end;        // Destination value
	protected GameObject subject; // Reference to our subject
	protected EZScreenPlacement subSP;
	protected Vector3 temp;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subSP != null)
		{
			subSP.screenPos = end;
			subSP.SetCamera();
			subSP.PositionOnScreen();
			subSP.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subSP != null)
		{
			// Get the most up-to-date state starting:
			start = subSP.screenPos;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subSP == null)
		{
			_stop();
			return;
		}

		temp.x = interpolator(timeElapsed, start.x, delta.x, interval);
		temp.y = interpolator(timeElapsed, start.y, delta.y, interval);
		temp.z = interpolator(timeElapsed, start.z, delta.z, interval);

		subSP.screenPos = temp;
		subSP.PositionOnScreen();
		subSP.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting position.</param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateScreenPosition Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateScreenPosition anim = (AnimateScreenPosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.TranslateScreen);
		anim.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateScreenPosition Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateScreenPosition anim = (AnimateScreenPosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.TranslateScreen);
		anim.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;
		
		EZScreenPlacement ezsp = (EZScreenPlacement) sub.GetComponent(typeof(EZScreenPlacement));

		if (ezsp == null)
		{
			Debug.LogError(string.Format("{0} has no EZScreenPlacement attached - but it's required for using AnimateScreenPosition/TranslateScreen!", sub.name));
			return false;
		}

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
		{
			Start(sub, parms.mode, parms.vec, parms.vec2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		else
		{
			Start(sub, parms.mode, ezsp.screenPos, parms.vec, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}

		return true;
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		EZScreenPlacement ezsp = (EZScreenPlacement)sub.GetComponent(typeof(EZScreenPlacement));

		if (ezsp == null)
		{
			Debug.LogError(string.Format("{0} has no EZScreenPlacement attached - but it's required for using AnimateScreenPosition/TranslateScreen!", sub.name));
			return;
		}

		Start(sub, mode, ezsp.screenPos, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts an AnimatePosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to translate.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting position.</param>
	/// <param name="dest">The destination position.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		EZScreenPlacement ezsp = (EZScreenPlacement)sub.GetComponent(typeof(EZScreenPlacement));

		if (ezsp == null)
		{
			Debug.LogError(string.Format("{0} has no EZScreenPlacement attached - but it's required for using AnimateScreenPosition/TranslateScreen!", sub.name));
			return;
		}

		subject = sub;
		subSP = ezsp;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = dest - start;

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subSP.screenPos = start;

		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (subject == null)
			return;   // Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = subject.transform.localEulerAngles;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public AnimateScreenPosition()
	{
		type = ANIM_TYPE.TranslateScreen;
	}
}



/// <remarks>
/// Animates an object's transform's scale.
/// </remarks>
public class AnimateScale : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 delta;		// Amount to change
	protected Vector3 end;			// Destination value
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;
	protected Vector3 temp;


	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localScale = end;
			subTrans.BroadcastMessage("OnEZScaled", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subTrans != null)
		{
			// Get the most up-to-date state starting:
			start = subTrans.localScale;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		temp.x = interpolator(timeElapsed, start.x, delta.x, interval);
		temp.y = interpolator(timeElapsed, start.y, delta.y, interval);
		temp.z = interpolator(timeElapsed, start.z, delta.z, interval);

		subTrans.localScale = temp;
		subTrans.BroadcastMessage("OnEZScaled", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts an AnimateScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to scale.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting scale.</param>
	/// <param name="dest">The destination scale.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateScale Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateScale anim = (AnimateScale)EZAnimator.instance.GetAnimation(ANIM_TYPE.Scale);
		anim.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts an AnimateScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to scale.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination scale.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static AnimateScale Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateScale anim = (AnimateScale)EZAnimator.instance.GetAnimation(ANIM_TYPE.Scale);
		anim.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(sub, parms.mode, parms.vec, parms.vec2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(sub, parms.mode, sub.transform.localScale, parms.vec, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts an AnimateScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to scale.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination scale.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, mode, sub.transform.localScale, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts an AnimateScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to scale.</param>
	/// <param name="mode">The mode of the animation. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting scale.</param>
	/// <param name="dest">The destination scale.</param>
	/// <param name="interp">The easing/interpolator to use.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = Vector3.Scale(start, dest) - start;
		else
			delta = dest - start;

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subTrans.localScale = start;

		EZAnimator.instance.AddAnimation(this);
	}

	// Run the previous animation with the same parameters:
	public void Start()
	{
		if (subject == null)
			return;	// Fughetaboutit

		direction = 1f;
		timeElapsed = 0;
		wait = m_wait;

		if (m_mode == ANIM_MODE.By)
		{
			start = subject.transform.localScale;
			end = start + delta;
		}

		EZAnimator.instance.AddAnimation(this);
	}

	public AnimateScale()
	{
		type = ANIM_TYPE.Scale;
	}
}


/// <remarks>
/// Animates an object's position by a certain
/// amount in a bouncy way, and then settles it 
/// back it to its original position.
/// </remarks>
public class PunchPosition : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localPosition = start;
			subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localPosition;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		factor = timeElapsed / interval;
		temp.x = start.x + punch(magnitude.x, factor);
		temp.y = start.y + punch(magnitude.y, factor);
		temp.z = start.z + punch(magnitude.z, factor);

		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a PunchPosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static PunchPosition Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		PunchPosition anim = (PunchPosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.PunchPosition);
		anim.Start(sub, mag, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localPosition, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a PunchPosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localPosition, mag, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a PunchPosition animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting position of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		magnitude = mag;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public PunchPosition()
	{
		type = ANIM_TYPE.PunchPosition;
	}
}


/// <remarks>
/// Animates an object's scale by a certain amount
/// in a bouncy way and then settles it back to its
/// original scale.
/// </remarks>
public class PunchScale : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localScale = start;
			subTrans.BroadcastMessage("OnEZScaled", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localScale;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		factor = timeElapsed / interval;
		temp.x = start.x + (punch(magnitude.x, factor));
		temp.y = start.y + (punch(magnitude.y, factor));
		temp.z = start.z + (punch(magnitude.z, factor));

		subTrans.localScale = temp;
		subTrans.BroadcastMessage("OnEZScaled", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a PunchScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static PunchScale Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		PunchScale anim = (PunchScale)EZAnimator.instance.GetAnimation(ANIM_TYPE.PunchScale);
		anim.Start(sub, mag, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localScale, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a PunchScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localScale, mag, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a PunchScale animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting scale of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localScale = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		magnitude = mag;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public PunchScale()
	{
		type = ANIM_TYPE.PunchScale;
	}
}


/// <remarks>
/// Rotates an object by a certain amount
/// in a bouncy way, and settles it back to
/// its original rotation.
/// </remarks>
public class PunchRotation : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localEulerAngles = start;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localEulerAngles;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		factor = timeElapsed / interval;
		temp.x = start.x + punch(magnitude.x, factor);
		temp.y = start.y + punch(magnitude.y, factor);
		temp.z = start.z + punch(magnitude.z, factor);

		subTrans.localRotation = Quaternion.Euler(temp);
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a PunchRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static PunchRotation Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		PunchRotation anim = (PunchRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.PunchRotation);
		anim.Start(sub, mag, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localEulerAngles, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a PunchRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localEulerAngles, mag, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a PunchRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">Starting rotation of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localEulerAngles = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		magnitude = mag;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public PunchRotation()
	{
		type = ANIM_TYPE.PunchRotation;
	}
}


/// <remarks>
/// Shakes an object randomly as if it had
/// crashed or experienced an explosion.
/// The effect diminishes linearly over time.
/// </remarks>
public class Crash : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 tempMag;
	protected Vector3 temp;
	protected float factor;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localPosition = start;
			subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date position before starting:
		if (subTrans != null)
			start = subTrans.localPosition;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		factor = timeElapsed / interval;

		tempMag.x = magnitude.x - (factor * magnitude.x);
		tempMag.y = magnitude.y - (factor * magnitude.y);
		tempMag.z = magnitude.z - (factor * magnitude.z);

		temp.x = start.x + Random.Range(-tempMag.x, tempMag.x);
		temp.y = start.y + Random.Range(-tempMag.y, tempMag.y);
		temp.z = start.z + Random.Range(-tempMag.z, tempMag.z);

		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a Crash animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static Crash Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Crash anim = (Crash)EZAnimator.instance.GetAnimation(ANIM_TYPE.Crash);
		anim.Start(sub, mag, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localPosition, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a Crash animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localPosition, mag, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a Crash animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting position of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		magnitude = mag;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public Crash()
	{
		type = ANIM_TYPE.Crash;
		pingPong = false;
	}
}


/// <remarks>
/// Like Crash, but moves in a smoother, more
/// connected manner.  The effect diminishes
/// linearly over time.
/// </remarks>
public class SmoothCrash : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected Vector3 oscillations;	// Number of desired oscillations per axis
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;
	protected float invFactor;
	protected const float PIx2 = Mathf.PI * 2f;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localPosition = start;
			subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localPosition;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		// Take advantage of the commutative property by
		// rolling 2*PI and the inverse(sorta) of our factor
		// all into a single value:
		factor = (timeElapsed / interval);
		invFactor = (1f - factor);
		factor *= PIx2;

		temp.x = start.x + Mathf.Sin(factor * oscillations.x) * magnitude.x * invFactor;
		temp.y = start.y + Mathf.Sin(factor * oscillations.y) * magnitude.y * invFactor;
		temp.z = start.z + Mathf.Sin(factor * oscillations.z) * magnitude.z * invFactor;

		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a SmoothCrash animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur on each axis.  If any of these values is negative, that axis will be assigned a random value in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static SmoothCrash Do(GameObject sub, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		SmoothCrash anim = (SmoothCrash)EZAnimator.instance.GetAnimation(ANIM_TYPE.SmoothCrash);
		anim.Start(sub, mag, oscill, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localPosition, parms.vec, parms.vec2, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a SmoothCrash animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur on each axis.  If any of these values is negative, that axis will be assigned a random value in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localPosition, mag, oscill, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a SmoothCrash animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting position of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur on each axis.  If any of these values is negative, that axis will be assigned a random value in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		magnitude = mag;
		
		// Assign random values if negative:
		if (oscill.x < 0)
			oscill.x = Random.Range(1f, -oscill.x);
		if (oscill.y < 0)
			oscill.y = Random.Range(1f, -oscill.y);
		if (oscill.z < 0)
			oscill.z = Random.Range(1f, -oscill.z);
		
		oscillations = oscill;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public SmoothCrash()
	{
		type = ANIM_TYPE.SmoothCrash;
		pingPong = false;
	}
}


/// <remarks>
/// Shakes an object back and forth
/// at a regular rate and amount.
/// </remarks>
public class Shake : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected float oscillations;	// Number of desired oscillations per axis
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;
	protected const float PIx2 = Mathf.PI * 2f;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localPosition = start;
			subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localPosition;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		// Take advantage of the commutative property by
		// rolling 2*PI and oscillations all into a single 
		// value:
		factor = Mathf.Sin((timeElapsed / interval) * PIx2 * oscillations);

		temp.x = start.x + (factor * magnitude.x);
		temp.y = start.y + (factor * magnitude.y);
		temp.z = start.z + (factor * magnitude.z);

		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a Shake animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur.  If this value is negative, a random value in the range 0-|n| will be assigned.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static Shake Do(GameObject sub, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Shake anim = (Shake)EZAnimator.instance.GetAnimation(ANIM_TYPE.Shake);
		anim.Start(sub, mag, oscill, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localPosition, parms.vec, parms.floatVal, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a Shake animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur.  If this value is negative, a random value in the range 0-|n| will be assigned.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localPosition, mag, oscill, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a Shake animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting position of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur.  If this value is negative, a random value in the range 0-|n| will be assigned.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		if (oscill < 0)
			oscill = Random.Range(1f, -oscill);

		magnitude = mag;
		oscillations = oscill;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public Shake()
	{
		type = ANIM_TYPE.Shake;
		pingPong = false;
	}
}


/// <remarks>
/// Shakes an object by rotating it back and 
/// forth a certain number of times on each
/// axis by a certain amount on each axis.
/// The effect diminishes linearly over time.
/// </remarks>
public class CrashRotation : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected Vector3 oscillations;	// Number of desired oscillations per axis
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;
	protected float invFactor;
	protected const float PIx2 = Mathf.PI * 2f;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localEulerAngles = start;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localEulerAngles;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		// Take advantage of the commutative property by
		// rolling 2*PI and oscillations all into a single 
		// value:
		factor = (timeElapsed / interval);
		invFactor = (1f - factor);
		factor *= PIx2;

		temp.x = start.x + Mathf.Sin(factor * oscillations.x) * magnitude.x * invFactor;
		temp.y = start.y + Mathf.Sin(factor * oscillations.y) * magnitude.y * invFactor;
		temp.z = start.z + Mathf.Sin(factor * oscillations.z) * magnitude.z * invFactor;

		subTrans.localRotation = Quaternion.Euler(temp);
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a CrashRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur on each axis.  If any of these values is negative, that axis will be assigned a random value in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static CrashRotation Do(GameObject sub, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		CrashRotation anim = (CrashRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.CrashRotation);
		anim.Start(sub, mag, oscill, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localEulerAngles, parms.vec, parms.vec2, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a CrashRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur on each axis.  If any of these values is negative, that axis will be assigned a random value in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localEulerAngles, mag, oscill, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a CrashRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting rotation of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur on each axis.  If any of these values is negative, that axis will be assigned a random value in the range 0-|n|.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localEulerAngles = start;

		// Assign random values if negative:
		if (mag.x < 0)
			mag.x = Random.Range(1f, -mag.x);
		if (mag.y < 0)
			mag.y = Random.Range(1f, -mag.y);
		if (mag.z < 0)
			mag.z = Random.Range(1f, -mag.z);

		// Assign random values if negative:
		if (oscill.x < 0)
			oscill.x = Random.Range(1f, -oscill.x);
		if (oscill.y < 0)
			oscill.y = Random.Range(1f, -oscill.y);
		if (oscill.z < 0)
			oscill.z = Random.Range(1f, -oscill.z);

		magnitude = mag;
		oscillations = oscill;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public CrashRotation()
	{
		type = ANIM_TYPE.CrashRotation;
		pingPong = false;
	}
}


/// <remarks>
/// Shakes an object by rotating it back and forth
/// a certain number of times by a certain amount
/// on each axis.  Unlike CrashRotation, this effect
/// ends abruptly rather than diminishing over time.
/// </remarks>
public class ShakeRotation : EZAnimation
{
	protected Vector3 start;		// Our starting value
	protected Vector3 magnitude;	// Amount to change
	protected float oscillations;	// Number of desired oscillations per axis
	protected GameObject subject;	// Reference to our subject
	protected Transform subTrans;	// Transform of our subject
	protected Vector3 temp;
	protected float factor;
	protected const float PIx2 = Mathf.PI * 2f;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localEulerAngles = start;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}

		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		// Get the most up-to-date state starting:
		if (subTrans != null)
			start = subTrans.localEulerAngles;
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}

		// Take advantage of the commutative property by
		// rolling 2*PI and oscillations all into a single 
		// value:
		factor = Mathf.Sin((timeElapsed / interval) * PIx2 * oscillations);

		temp.x = start.x + (factor * magnitude.x);
		temp.y = start.y + (factor * magnitude.y);
		temp.z = start.z + (factor * magnitude.z);

		subTrans.localRotation = Quaternion.Euler(temp);
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Starts a ShakeRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur.  If this value is negative, a random value in the range 0-|n| will be assigned.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static ShakeRotation Do(GameObject sub, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		ShakeRotation anim = (ShakeRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.ShakeRotation);
		anim.Start(sub, mag, oscill, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, sub.transform.localEulerAngles, parms.vec, parms.floatVal, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts a ShakeRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur.  If this value is negative, a random value in the range 0-|n| will be assigned.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		Start(sub, sub.transform.localEulerAngles, mag, oscill, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts a ShakeRotation animation on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject to animate.</param>
	/// <param name="begin">The starting rotation of the object.</param>
	/// <param name="mag">The magnitude of the effect on each axis.  If any of these values is negative, that axis will be assigned a random magnitude in the range 0-|n|.</param>
	/// <param name="oscill">The number of oscillations that should occur.  If this value is negative, a random value in the range 0-|n| will be assigned.</param>
	/// <param name="dur">The duration of the animation, in seconds. If negative, the animation will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localEulerAngles = start;

		magnitude = mag;
		oscillations = oscill;
		m_mode = ANIM_MODE.By; // Always consider this a "by" animation.

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		// Stop any existing animations of this type on the subject:
		EZAnimator.instance.Stop(subject, type, true);

		EZAnimator.instance.AddAnimation(this);
	}

	public ShakeRotation()
	{
		type = ANIM_TYPE.ShakeRotation;
		pingPong = false;
	}
}


/// <remarks>
/// Plays an AnimationClip on the subject.
/// Useful for working with transitions.
/// </remarks>
public class RunAnimClip : EZAnimation
{
	protected GameObject subject;	// Reference to our subject
	protected string m_clip;
	protected bool waitForClip = true; // Does not call the completedDelegate until the clip is no longer playing
	protected bool playedYet = false;
	protected float blending;

	public override object GetSubject()
	{
		return subject;
	}

	public override bool Step(float timeDelta)
	{
		if (wait > 0)
		{
			wait -= timeDelta;

			if (wait < 0)
			{
				// Take the overage off our timeDelta:
				timeDelta -= (timeDelta + wait);
			}
			else
				return true;
		}

		if (!playedYet)
		{
			if (duration == 0 && blending == 0)
				subject.animation.Play(m_clip);
			else
				subject.animation.Blend(m_clip, blending, duration);
			playedYet = true;
			return true;
		}
		else if (subject.animation.IsPlaying(m_clip))
			return true;
		else
		{
			_end();

			return false;
		}
	}

	protected override void DoAnim()
	{
	}

	/// <summary>
	/// Starts an AnimationClip on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject which contains the animation clip by the specified name.</param>
	/// <param name="clip">The name of the animation clip to play.</param>
	/// <param name="blend">The target blend weight of the clip. If set to 1, the clip will complete override any existing animation by the time duration has elapsed.</param>
	/// <param name="dur">The number of seconds it should take for the new clip to blend in to the full target blend weight.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	/// <returns>A reference to the RunAnimClip object.</returns>
	public static RunAnimClip Do(GameObject sub, string clip, float blend, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		RunAnimClip anim = (RunAnimClip)EZAnimator.instance.GetAnimation(ANIM_TYPE.AnimClip);
		anim.Start(sub, clip, blend, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts an AnimationClip on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject which contains the animation clip by the specified name.</param>
	/// <param name="clip">The name of the animation clip to play.</param>
	/// <param name="blend">The target blend weight of the clip. If set to 1, the clip will complete override any existing animation by the time duration has elapsed.</param>
	/// <param name="dur">The number of seconds it should take for the new clip to blend in to the full target blend weight.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		if (sub.animation == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		Start(sub, parms.strVal, parms.floatVal, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	/// <summary>
	/// Starts an AnimationClip on the specified subject.
	/// </summary>
	/// <param name="sub">GameObject which contains the animation clip by the specified name.</param>
	/// <param name="clip">The name of the animation clip to play.</param>
	/// <param name="blend">The target blend weight of the clip. If set to 1, the clip will complete override any existing animation by the time duration has elapsed.</param>
	/// <param name="dur">The number of seconds it should take for the new clip to blend in to the full target blend weight.</param>
	/// <param name="delay">The number of seconds to wait before the animation begins.</param>
	/// <param name="startDel">Delegate to call when the animation begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the animation ends.</param>
	public void Start(GameObject sub, string clip, float blend, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
			return;

		if (sub.animation == null)
			return;

		playedYet = false;

		subject = sub;

		m_clip = clip;
		blending = blend;

		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		EZAnimator.instance.AddAnimation(this);
	}

	public RunAnimClip()
	{
		type = ANIM_TYPE.AnimClip;
		pingPong = false;
	}
}


/// <remarks>
/// Fades the volume of an audio clip from
/// one volume to another.
/// If any existing FadeAudio is running on
/// the subject, it is stopped.
/// </remarks>
public class FadeAudio : EZAnimation
{
	protected float start;		// Our starting value
	protected AudioSource subject;	// Reference to our subject
	protected float delta;
	protected float end;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if(subject != null)
			subject.volume = end;

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subject != null)
		{
			// Get the most up-to-date state starting:
			start = subject.volume;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subject == null)
		{
			_stop();
			return;
		}

		subject.volume = interpolator(timeElapsed, start, delta, interval);
	}

	/// <summary>
	/// Starts fading the volume on the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to fade.</param>
	/// <param name="mode">The mode of the fade. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting volume of the subject.</param>
	/// <param name="dest">The destination volume.</param>
	/// <param name="dur">The duration of the fade, in seconds. If negative, the fade will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the fade begins.</param>
	/// <param name="startDel">Delegate to call when the fade begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the fade ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeAudio Do(AudioSource audio, ANIM_MODE mode, float begin, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeAudio anim = (FadeAudio)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeAudio);
		anim.Start(audio, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts fading the volume on the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to fade.</param>
	/// <param name="mode">The mode of the fade. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination volume.</param>
	/// <param name="dur">The duration of the fade, in seconds. If negative, the fade will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the fade begins.</param>
	/// <param name="startDel">Delegate to call when the fade begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the fade ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static FadeAudio Do(AudioSource audio, ANIM_MODE mode, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeAudio anim = (FadeAudio)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeAudio);
		anim.Start(audio, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		subject = (AudioSource)sub.GetComponent(typeof(AudioSource));
		if (subject == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(subject, parms.mode, parms.floatVal, parms.floatVal2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(subject, parms.mode, subject.volume, parms.floatVal, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts fading the volume on the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to fade.</param>
	/// <param name="mode">The mode of the fade. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination volume.</param>
	/// <param name="dur">The duration of the fade, in seconds. If negative, the fade will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the fade begins.</param>
	/// <param name="startDel">Delegate to call when the fade begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the fade ends.</param>
	public void Start(AudioSource audio, ANIM_MODE mode, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(audio, mode, audio.volume, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts fading the volume on the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to fade.</param>
	/// <param name="mode">The mode of the fade. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting volume of the subject.</param>
	/// <param name="dest">The destination volume.</param>
	/// <param name="dur">The duration of the fade, in seconds. If negative, the fade will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the fade begins.</param>
	/// <param name="startDel">Delegate to call when the fade begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the fade ends.</param>
	public void Start(AudioSource sub, ANIM_MODE mode, float begin, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = dest - start;

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		//EZAnimator.instance.Stop(subject, type, (mode == ANIM_MODE.By)?true:false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subject.volume = start;

		EZAnimator.instance.AddAnimation(this);
	}

	public FadeAudio()
	{
		type = ANIM_TYPE.FadeAudio;
		pingPong = false;
	}
}


/// <remarks>
/// Changes the pitch of an AudioSource
/// from one pitch to another.
/// If any existing TuneAudio is running on
/// the subject, it is stopped.
/// </remarks>
public class TuneAudio : EZAnimation
{
	protected float start;		// Our starting value
	protected AudioSource subject;	// Reference to our subject
	protected float delta;
	protected float end;

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if(subject != null)
			subject.volume = end;

		base._end();
	}

	protected override void LoopReset()
	{
		if (Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();

		if (Mode == ANIM_MODE.By && subject != null)
		{
			// Get the most up-to-date state starting:
			start = subject.pitch;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subject == null)
		{
			_stop();
			return;
		}

		subject.pitch = interpolator(timeElapsed, start, delta, interval);
	}

	/// <summary>
	/// Starts changing the pitch of the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to tune.</param>
	/// <param name="mode">The mode of the effect. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting pitch of the subject.</param>
	/// <param name="dest">The destination pitch.</param>
	/// <param name="dur">The duration of the effect, in seconds. If negative, the effect will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the effect begins.</param>
	/// <param name="startDel">Delegate to call when the effect begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the effect ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static TuneAudio Do(AudioSource audio, ANIM_MODE mode, float begin, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		TuneAudio anim = (TuneAudio)EZAnimator.instance.GetAnimation(ANIM_TYPE.TuneAudio);
		anim.Start(audio, mode, begin, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	/// <summary>
	/// Starts changing the pitch of the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to tune.</param>
	/// <param name="mode">The mode of the effect. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination pitch.</param>
	/// <param name="dur">The duration of the effect, in seconds. If negative, the effect will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the effect begins.</param>
	/// <param name="startDel">Delegate to call when the effect begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the effect ends.</param>
	/// <returns>A reference to the animation object.</returns>
	public static TuneAudio Do(AudioSource audio, ANIM_MODE mode, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		TuneAudio anim = (TuneAudio)EZAnimator.instance.GetAnimation(ANIM_TYPE.TuneAudio);
		anim.Start(audio, mode, dest, interp, dur, delay, startDel, del);
		return anim;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
			return false;

		subject = (AudioSource)sub.GetComponent(typeof(AudioSource));
		if (subject == null)
			return false;

		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;

		if (parms.mode == ANIM_MODE.FromTo)
			Start(subject, parms.mode, parms.floatVal, parms.floatVal2, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		else
			Start(subject, parms.mode, subject.pitch, parms.floatVal, GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);

		return true;
	}

	/// <summary>
	/// Starts changing the pitch of the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to tune.</param>
	/// <param name="mode">The mode of the effect. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="dest">The destination pitch.</param>
	/// <param name="dur">The duration of the effect, in seconds. If negative, the effect will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the effect begins.</param>
	/// <param name="startDel">Delegate to call when the effect begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the effect ends.</param>
	public void Start(AudioSource audio, ANIM_MODE mode, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(audio, mode, audio.pitch, dest, interp, dur, delay, startDel, del);
	}

	/// <summary>
	/// Starts changing the pitch of the specified subject.
	/// </summary>
	/// <param name="sub">AudioSource to tune.</param>
	/// <param name="mode">The mode of the effect. See <see cref="EZAnimation.ANIM_MODE"/></param>
	/// <param name="begin">The starting pitch of the subject.</param>
	/// <param name="dest">The destination pitch.</param>
	/// <param name="dur">The duration of the effect, in seconds. If negative, the effect will loop infinitely at an interval of |dur|.</param>
	/// <param name="delay">The number of seconds to wait before the effect begins.</param>
	/// <param name="startDel">Delegate to call when the effect begins (after the delay has elapsed). NOTE: For looping animations, this will be called on each iteration.</param>
	/// <param name="del">Delegate to call when the effect ends.</param>
	public void Start(AudioSource sub, ANIM_MODE mode, float begin, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		start = begin;

		m_mode = mode;

		if (mode == ANIM_MODE.By)
			delta = dest;
		else
			delta = dest - start;

		end = start + delta;

		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;

		StartCommon();

		//EZAnimator.instance.Stop(subject, type, (mode == ANIM_MODE.By) ? true : false);

		// See if we need to set to an initial state:
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0)
			subject.pitch = start;

		EZAnimator.instance.AddAnimation(this);
	}

	public TuneAudio()
	{
		type = ANIM_TYPE.TuneAudio;
		pingPong = false;
	}
}



//-----------------------------------------
// Class which serves as a parameter bundle
// for each animation type:
//-----------------------------------------

/// <remarks>
/// Holds parameters needed to setup
/// an animation.
/// </remarks>
[System.Serializable]
public class AnimParams
{
	public EZAnimation.ANIM_MODE mode = EZAnimation.ANIM_MODE.To;
	public float delay;
	public float duration;
	public bool pingPong;
	public bool restartOnRepeat;
	public bool repeatDelay;
	public EZAnimation.EASING_TYPE easing;
	[System.NonSerialized]
	protected EZTransition m_transition; // Transition that uses these params


	// Data members for each type of animation:
	public Color color = Color.white;
	public Color color2 = Color.white;
	public Vector3 vec;
	public Vector3 vec2;
	public Vector3 axis;
	public float floatVal;
	public float floatVal2;
	public string strVal = "";


	public AnimParams(EZTransition trans)
	{
		m_transition = trans;
	}


	/// <summary>
	/// Copies the specified AnimParams object.
	/// </summary>
	/// <param name="src">AnimParams to be copied.</param>
	public void Copy(AnimParams src)
	{
		mode = src.mode;
		delay = src.delay;
		duration = src.duration;
		easing = src.easing;
		color = src.color;
		vec = src.vec;
		axis = src.axis;
		floatVal = src.floatVal;
		color2 = src.color2;
		vec2 = src.vec2;
		floatVal2 = src.floatVal2;
		strVal = src.strVal;
		pingPong = src.pingPong;
		repeatDelay = src.repeatDelay;
		restartOnRepeat = src.restartOnRepeat;
	}


	public EZTransition transition
	{
		get { return m_transition; }
		set { m_transition = value; }
	}

	public virtual void DrawGUI(EZAnimation.ANIM_TYPE type, GameObject go, IGUIHelper gui, bool inspector)
	{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		float spacing = 20f;
		float indent = 10f;
#else
		float spacing = 0f;
		float indent = 0f;
#endif
		// Track any changes in values:
		bool valueChanged = GUI.changed;
		GUI.changed = false;

		delay = gui.FloatField("Delay:", delay);
		if(!inspector)
			GUILayout.Space(spacing);
		duration = gui.FloatField("Duration:", duration);

		// See if we need to present looping options:
		if (duration < 0)
		{
			repeatDelay = GUILayout.Toggle(repeatDelay, new GUIContent("Rep. Delay", "Repeats the delay on each loop iteration"));

			if (type != EZAnimation.ANIM_TYPE.AnimClip ||
				type != EZAnimation.ANIM_TYPE.Crash ||
				type != EZAnimation.ANIM_TYPE.CrashRotation ||
				type != EZAnimation.ANIM_TYPE.PunchPosition ||
				type != EZAnimation.ANIM_TYPE.PunchRotation ||
				type != EZAnimation.ANIM_TYPE.PunchScale ||
				type != EZAnimation.ANIM_TYPE.Shake ||
				type != EZAnimation.ANIM_TYPE.ShakeRotation ||
				type != EZAnimation.ANIM_TYPE.SmoothCrash)
			{
				pingPong = GUILayout.Toggle(pingPong, new GUIContent("PingPong","Ping-Pong: Causes the animated value to go back and forth as it loops."));
			}
		}
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		else
			GUILayout.FlexibleSpace();
#endif
		if (!inspector)
		{
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space(indent);
		}

		// Only show easing selection for certain types:
		if(type == EZAnimation.ANIM_TYPE.FadeMaterial ||
		   type == EZAnimation.ANIM_TYPE.FadeSprite ||
		   type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha ||
		   type == EZAnimation.ANIM_TYPE.FadeAudio ||
		   type == EZAnimation.ANIM_TYPE.TuneAudio ||
		   type == EZAnimation.ANIM_TYPE.FadeText ||
		   type == EZAnimation.ANIM_TYPE.FadeTextAlpha ||
		   type == EZAnimation.ANIM_TYPE.Rotate ||
		   type == EZAnimation.ANIM_TYPE.Scale ||
		   type == EZAnimation.ANIM_TYPE.Translate ||
		   type == EZAnimation.ANIM_TYPE.TranslateScreen ||
		   type == EZAnimation.ANIM_TYPE.RotateEuler)
		{
			easing = (EZAnimation.EASING_TYPE)gui.EnumField("Easing:", easing);
		}

		if (!inspector)
			GUILayout.Space(spacing);

		// Only show mode selection for certain types:
		if(type == EZAnimation.ANIM_TYPE.FadeMaterial ||
		   type == EZAnimation.ANIM_TYPE.FadeSprite ||
		   type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha ||
		   type == EZAnimation.ANIM_TYPE.FadeAudio ||
		   type == EZAnimation.ANIM_TYPE.TuneAudio ||
		   type == EZAnimation.ANIM_TYPE.FadeText ||
		   type == EZAnimation.ANIM_TYPE.FadeTextAlpha ||
		   type == EZAnimation.ANIM_TYPE.Rotate ||
		   type == EZAnimation.ANIM_TYPE.Scale ||
		   type == EZAnimation.ANIM_TYPE.Translate ||
		   type == EZAnimation.ANIM_TYPE.TranslateScreen || 
		   type == EZAnimation.ANIM_TYPE.RotateEuler)
		{
			mode = (EZAnimation.ANIM_MODE)gui.EnumField("Mode:", mode);
		}

		// If it is the right mode, it loops, and is the right
		// type of animation, show the "resetOnRepeat" option:
		if (/*mode == EZAnimation.ANIM_MODE.By &&*/ duration < 0 && (
			type == EZAnimation.ANIM_TYPE.FadeMaterial ||
			type == EZAnimation.ANIM_TYPE.FadeSprite ||
		    type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha ||
			type == EZAnimation.ANIM_TYPE.FadeAudio ||
			type == EZAnimation.ANIM_TYPE.TuneAudio ||
			type == EZAnimation.ANIM_TYPE.FadeText ||
		    type == EZAnimation.ANIM_TYPE.FadeTextAlpha ||
			type == EZAnimation.ANIM_TYPE.Rotate ||
			type == EZAnimation.ANIM_TYPE.Scale ||
			type == EZAnimation.ANIM_TYPE.Translate ||
			type == EZAnimation.ANIM_TYPE.TranslateScreen ||
			type == EZAnimation.ANIM_TYPE.RotateEuler))
		{
			restartOnRepeat = GUILayout.Toggle(restartOnRepeat, new GUIContent("Restart on Loop","Resets the starting value on each loop iteration. Set this to false if you want something like continuous movement in the same direction without going back to the starting point."));
		}
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		else
			GUILayout.FlexibleSpace();
#endif
		if (!inspector)
			GUILayout.EndHorizontal();

		switch(type)
		{
			case EZAnimation.ANIM_TYPE.FadeSprite:
			case EZAnimation.ANIM_TYPE.FadeSpriteAlpha:
			case EZAnimation.ANIM_TYPE.FadeMaterial:
			case EZAnimation.ANIM_TYPE.FadeText:
			case EZAnimation.ANIM_TYPE.FadeTextAlpha:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					color = gui.ColorField("Start Color:", color);
					color2 = gui.ColorField("End Color:", color2);
				}
				else
					color = gui.ColorField("Color:", color);
				break;
			case EZAnimation.ANIM_TYPE.Rotate:
			case EZAnimation.ANIM_TYPE.RotateEuler:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					vec = gui.Vector3Field("Start Angles:", vec);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec = go.transform.localEulerAngles;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localEulerAngles = vec;
					GUILayout.EndHorizontal();

					vec2 = gui.Vector3Field("End Angles:", vec2);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec2 = go.transform.localEulerAngles;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localEulerAngles = vec2;
					GUILayout.EndHorizontal();
				}
				else
				{
					vec = gui.Vector3Field("Angles:", vec);
					if (mode == EZAnimation.ANIM_MODE.To)
					{
						GUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
						{
							vec = go.transform.localEulerAngles;
							GUI.changed = true;
						}
						if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
							go.transform.localEulerAngles = vec;
						GUILayout.EndHorizontal();
					}
				}
				break;
/*
			case EZAnimation.ANIM_TYPE.RotateAround:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					vec = gui.Vector3Field("Point:", vec);
					axis = gui.Vector3Field("Axis:", axis);
					floatVal = gui.FloatField("Start Angle:", floatVal);
					floatVal2 = gui.FloatField("End Angle:", floatVal2);
				}
				else
				{
					vec = gui.Vector3Field("Point:", vec);
					axis = gui.Vector3Field("Axis:", axis);
					floatVal = gui.FloatField("Angle:", floatVal);
				}
				break;
*/
			case EZAnimation.ANIM_TYPE.Scale:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					vec = gui.Vector3Field("Start Scale:", vec);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec = go.transform.localScale;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localScale = vec;
					GUILayout.EndHorizontal();

					vec2 = gui.Vector3Field("End Scale:", vec2);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec2 = go.transform.localScale;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localScale = vec2;
					GUILayout.EndHorizontal();
				}
				else
				{
					vec = gui.Vector3Field("Scale:", vec);
					if (mode == EZAnimation.ANIM_MODE.To)
					{
						GUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
						{
							vec = go.transform.localScale;
							GUI.changed = true;
						}
						if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
							go.transform.localScale = vec;
						GUILayout.EndHorizontal();
					}
				}
				break;
			case EZAnimation.ANIM_TYPE.Translate:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					vec = gui.Vector3Field("Start Pos:", vec);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec = go.transform.localPosition;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localPosition = vec;
					GUILayout.EndHorizontal();

					vec2 = gui.Vector3Field("End Pos:", vec2);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec2 = go.transform.localPosition;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localPosition = vec2;
					GUILayout.EndHorizontal();
				}
				else if (mode == EZAnimation.ANIM_MODE.By)
					vec = gui.Vector3Field("Vector:", vec);
				else
				{
					vec = gui.Vector3Field("Pos:", vec);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec = go.transform.localPosition;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
						go.transform.localPosition = vec;
					GUILayout.EndHorizontal();
				}
				break;
			case EZAnimation.ANIM_TYPE.TranslateScreen:
				EZScreenPlacement ezsp = (EZScreenPlacement) go.GetComponent(typeof(EZScreenPlacement));

				if(ezsp == null)
				{
					Debug.LogError("ERROR: A transition element of type TranslateScreen has been selected, but the object \"" + go.name + "\" does not have an EZScreenPlacement component attached.");
					break;
				}
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					vec = gui.Vector3Field("Start Pos:", vec);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec = ezsp.screenPos;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
					{
						ezsp.screenPos = vec;
						ezsp.PositionOnScreen();
					}
					GUILayout.EndHorizontal();

					vec2 = gui.Vector3Field("End Pos:", vec2);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec2 = ezsp.screenPos;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
					{
						ezsp.screenPos = vec2;
						ezsp.PositionOnScreen();
					}
					GUILayout.EndHorizontal();
				}
				else if (mode == EZAnimation.ANIM_MODE.By)
					vec = gui.Vector3Field("Vector:", vec);
				else
				{
					vec = gui.Vector3Field("Pos:", vec);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
					{
						vec = ezsp.screenPos;
						GUI.changed = true;
					}
					if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
					{
						ezsp.screenPos = vec;
						ezsp.PositionOnScreen();
					}
					GUILayout.EndHorizontal();
				}
				break;
			case EZAnimation.ANIM_TYPE.PunchPosition:
			case EZAnimation.ANIM_TYPE.PunchScale:
			case EZAnimation.ANIM_TYPE.PunchRotation:
			case EZAnimation.ANIM_TYPE.Crash:
				vec = gui.Vector3Field("Magnitude:", vec);
				break;
			case EZAnimation.ANIM_TYPE.Shake:
			case EZAnimation.ANIM_TYPE.ShakeRotation:
				vec = gui.Vector3Field("Magnitude:", vec);
				floatVal = gui.FloatField("Oscillations:", floatVal);
				break;
			case EZAnimation.ANIM_TYPE.SmoothCrash:
			case EZAnimation.ANIM_TYPE.CrashRotation:
				vec = gui.Vector3Field("Magnitude:", vec);
				vec2 = gui.Vector3Field("Oscillations:", vec2);
				break;
			case EZAnimation.ANIM_TYPE.AnimClip:
				strVal = gui.TextField("Anim Clip:", strVal);
				if(!inspector)
				{
					GUILayout.BeginHorizontal();
					floatVal = Mathf.Clamp01(gui.FloatField("Blend Weight:", floatVal));
					GUILayout.Space(15f);
					floatVal = GUILayout.HorizontalSlider(floatVal, 0, 1f, GUILayout.Width(200f));
					GUILayout.EndHorizontal();
				}
				else
					floatVal = Mathf.Clamp01(gui.FloatField("Blend Weight:", floatVal));
				break;
			case EZAnimation.ANIM_TYPE.FadeAudio:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					floatVal = gui.FloatField("Start Volume:", floatVal);
					floatVal2 = gui.FloatField("End Volume:", floatVal2);
				}
				else
				{
					floatVal = gui.FloatField("Volume:", floatVal);
				}
				break;
			case EZAnimation.ANIM_TYPE.TuneAudio:
				if (mode == EZAnimation.ANIM_MODE.FromTo)
				{
					floatVal = gui.FloatField("Start pitch:", floatVal);
					floatVal2 = gui.FloatField("End pitch:", floatVal2);
				}
				else
				{
					floatVal = gui.FloatField("Pitch:", floatVal);
				}
				break;
		}

		// If something changed, tell the transition
		// that it has unique values:
		if(GUI.changed)
			m_transition.initialized = true;
		
		GUI.changed = valueChanged || GUI.changed;
	}
}
