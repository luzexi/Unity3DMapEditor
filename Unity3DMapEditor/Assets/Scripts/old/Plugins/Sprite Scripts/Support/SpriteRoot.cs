//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// A delegate type that matches the profile of AssetDatabase.LoadAssetAtPath
public delegate Object AssetLoaderDelegate(string path, System.Type type);


/// <remarks>
/// Describes a sprite frame.
/// </remarks>
[System.Serializable]
public struct SPRITE_FRAME
{
	// The UV coordinates of the sprite within the atlas.
	public Rect uvs;

	// A pair of values that, when multiplied by the sprite's
	// current size, yields values that are half of how large 
	// the original texture area would be were it not trimmed.
	public Vector2 scaleFactor;

	// The offset distance from the center of the GameObject
	// of the edges of the sprite - given as a percentage.
	public Vector2 topLeftOffset;
	public Vector2 bottomRightOffset;

	public SPRITE_FRAME(float dummy)
	{
		uvs = new Rect(1f, 1f, 1f, 1f);
		scaleFactor = new Vector2(0.5f, 0.5f);
		topLeftOffset = new Vector2(-1f, 1f);
		bottomRightOffset = new Vector2(1f, -1f);
	}

	public void Copy(CSpriteFrame f)
	{
		uvs = f.uvs;
		scaleFactor = f.scaleFactor;
		topLeftOffset = f.topLeftOffset;
		bottomRightOffset = f.bottomRightOffset;
	}
}


/// <remarks>
/// Describes a single-frame sprite state.
/// </remarks>
[System.Serializable]
public class SpriteState
{
	public string name;
	[HideInInspector]
	public string imgPath;
	[HideInInspector]
	public CSpriteFrame frameInfo;

	public SpriteState(string n, string p)
	{
		name = n;
		imgPath = p;
	}
}


/// <remarks>
/// A Unity-serializable form of SPRITE_FRAME, but not as efficient.
/// Not to be used for animations at run-time.
/// </remarks>
[System.Serializable]
public class CSpriteFrame
{
	// The UV coordinates of the sprite within the atlas.
	public Rect uvs = new Rect(1f, 1f, 1f, 1f);

	// A pair of values that, when multiplied by the sprite's
	// current size, yields values that are half of how large 
	// the original texture area would be were it not trimmed.
	public Vector2 scaleFactor = new Vector2(0.5f, 0.5f);

	// The offset distance from the center of the GameObject
	// of the edges of the sprite - given as a percentage.
	public Vector2 topLeftOffset = new Vector2(-1f, 1f);
	public Vector2 bottomRightOffset = new Vector2(1f, -1f);

	public void Copy(SPRITE_FRAME f)
	{
		uvs = f.uvs;
		scaleFactor = f.scaleFactor;
		topLeftOffset = f.topLeftOffset;
		bottomRightOffset = f.bottomRightOffset;
	}

	public void Copy(CSpriteFrame f)
	{
		uvs = f.uvs;
		scaleFactor = f.scaleFactor;
		topLeftOffset = f.topLeftOffset;
		bottomRightOffset = f.bottomRightOffset;
	}

	public SPRITE_FRAME ToStruct()
	{
		SPRITE_FRAME sf;

		sf.uvs = uvs;
		sf.scaleFactor = scaleFactor;
		sf.topLeftOffset = topLeftOffset;
		sf.bottomRightOffset = bottomRightOffset;

		return sf;
	}

	public CSpriteFrame(){}

	public CSpriteFrame(CSpriteFrame f)
	{
		Copy(f);
	}

	public CSpriteFrame(SPRITE_FRAME f)
	{
		Copy(f);
	}
}


/// <summary>
/// The interface of an item/element in an EZLinkedList list.
/// </summary>
/// <typeparam name="T">Put your class name here.</typeparam>
public interface IEZLinkedListItem<T>
{
	T prev { get; set; }
	T next { get; set; }
}

/// <remarks>
/// Iterator for an EZLinkedList
/// </remarks>
/// <typeparam name="T">Type that is used for the list to be iterated.</typeparam>
public class EZLinkedListIterator<T> where T : IEZLinkedListItem<T>
{
	protected T cur;
	protected EZLinkedList<T> list;

	public T Current
	{
		get { return cur; }
		set { cur = value; }
	}

	/// <summary>
	/// Begin iterating through a list.
	/// </summary>
	/// <param name="l"></param>
	/// <returns></returns>
	public bool Begin(EZLinkedList<T> l)
	{
		list = l;
		cur = l.Head;

		return cur == null;
	}

	/// <summary>
	/// Ends iteration of the list and
	/// frees the iterator.
	/// </summary>
	public void End()
	{
		list.End(this);
	}


	/// <summary>
	/// Returns whether the iterator has
	/// reached the end of the list.
	/// </summary>
	public bool Done
	{
		get { return cur == null; }
	}

	/// <summary>
	/// Moves Current to the next item in the list.
	/// </summary>
	/// <returns>True if move succeeded, false if the end of the list was reached.</returns>
	public bool Next()
	{
		if (cur != null)
			cur = cur.next;

		if (cur == null)
		{
			list.End(this);
			return false;
		}
		
		return true;
	}

	/// <summary>
	/// Moves Current to the next item in the list
	/// and does not call the associated list's
	/// End() when the end is reached.
	/// Use this when you will constantly reuse
	/// the iterator.
	/// </summary>
	/// <returns>True if move succeeded, false if the end of the list was reached.</returns>
	public bool NextNoRemove()
	{
		if (cur != null)
			cur = cur.next;

		return cur != null;
	}
}

// Wrapper for an arbitrary type so we can make any
// object into a linked list element.
public class EZLinkedListNode<T> : IEZLinkedListItem<EZLinkedListNode<T>>
{
	public T val;
	EZLinkedListNode<T> m_prev;
	EZLinkedListNode<T> m_next;

	public EZLinkedListNode(T v)
	{
		val = v;
	}

	public EZLinkedListNode<T> prev
	{
		get { return m_prev; }
		set { m_prev = value; }
	}

	public EZLinkedListNode<T> next
	{
		get { return m_next; }
		set { m_next = value; }
	}
}

/// <summary>
/// A custom doubly linked list generic class.
/// </summary>
/// <typeparam name="T">Put your class name here.</typeparam>
public class EZLinkedList<T> where T : IEZLinkedListItem<T>
{
	List<EZLinkedListIterator<T>> iters = new List<EZLinkedListIterator<T>>();
	List<EZLinkedListIterator<T>> freeIters = new List<EZLinkedListIterator<T>>();

	// The first element in our list:
	protected T head;

	// Used to iterate through the list:
	protected T cur;

	// Used for iterating with MoveNext so that
	// if we remove the current item, we still
	// can safely proceed to the next item:
	protected T nextItem;

	// Running total of items in our list:
	protected int count=0;

	public int Count
	{
		get { return count; }
	}

	public bool Empty
	{
		get { return head == null; }
	}

	public T Head
	{
		get { return head; }
	}

	public T Current
	{
		get { return cur; }
		set { cur = value; }
	}

	/// <summary>
	/// Gets an iterator with which to iterate
	/// through the list.
	/// </summary>
	/// <returns>Iterator.</returns>
	public EZLinkedListIterator<T> Begin()
	{
		EZLinkedListIterator<T> it;

		// Get a free iterator, if any:
		if (freeIters.Count > 0)
		{
			it = freeIters[freeIters.Count-1];
			freeIters.RemoveAt(freeIters.Count-1);
		}
		else
		{
			it = new EZLinkedListIterator<T>();
		}

		iters.Add(it);
		it.Begin(this);
		return it;
	}

	/// <summary>
	/// Ends iteration of the list using the specified iterator.
	/// The iterator is freed for use again later.
	/// This is normally called automatically by the iterator
	/// when it reaches the end of the list.
	/// </summary>
	/// <param name="it">The iterator no longer to be used.</param>
	public void End(EZLinkedListIterator<T> it)
	{
		if(iters.Remove(it))
			freeIters.Add(it);
	}

	public bool Rewind()
	{
		cur = head;

		if (cur != null)
		{
			nextItem = cur.next;
			return true;
		}
		else
		{
			nextItem = default(T);
			return false;
		}
	}

	/// <summary>
	/// Moves Current to the next item in the list.
	/// </summary>
	/// <returns>True if move succeeded, false if the end of the list was reached.</returns>
	public bool MoveNext()
	{
		cur = nextItem;

		if (cur != null)
			nextItem = cur.next;

		return cur != null;
	}

	/// <summary>
	/// Adds an item to our list
	/// </summary>
	/// <param name="item">Item to be added to the list</param>
	public void Add(T item)
	{
		if (head != null)
		{
			item.next = head;
			head.prev = item;
		}

		head = item;

		++count;
	}

	/// <summary>
	/// Removes the specified item from the list.
	/// </summary>
	/// <param name="item">The item to be removed.</param>
	public void Remove(T item)
	{
		if (head == null || item == null)
			return;


		if (head.Equals(item))
		{
			head = item.next;

			// See if any iterators need to be notified:
			if (iters.Count > 0)
			{
				// Set the current to the next:
				for (int i = 0; i < iters.Count; ++i)
					if (iters[i].Current != null)
						if(iters[i].Current.Equals(item))
							iters[i].Current = item.next;
			}
		}
		else
		{
			// See if any iterators need to be notified:
			if (iters.Count > 0)
			{
				// Start the current up to the previous:
				for (int i = 0; i < iters.Count; ++i)
					if (iters[i].Current != null)
						if (iters[i].Current.Equals(item))
							iters[i].Current = item.prev;
			}

			if (item.next != null)
			{	// Connect both sides:
				if (item.prev != null)
					item.prev.next = item.next;
				item.next.prev = item.prev;
			}
			else if (item.prev != null)
			{
				// Removing the tail item:
				item.prev.next = default(T);
			}
		}
		item.next = default(T);
		item.prev = default(T);

		--count;
	}


	/// <summary>
	/// Removes all items from the list and ensures
	/// their prev and next references are cleared.
	/// </summary>
	public void Clear()
	{
		count = 0;

		if (head == null)
			return;

		T next;
		cur = head;
		head = default(T);

		do
		{
			next = cur.next;
			cur.prev = default(T);
			cur.next = default(T);
			cur = next;
		} while (cur != null);
	}
}



/// <summary>
/// Represents a 2D rect that exists in 3D space and
/// is not axis-aligned.
/// </summary>
public struct Rect3D
{
	Vector3 m_tl;		// Top-left
	Vector3 m_tr;		// Top-right
	Vector3 m_bl;		// Bottom-left
	Vector3 m_br;		// Bottom-right
	float m_width;		// Width
	float m_height;		// Height

	public Vector3 topLeft		{ get { return m_tl; } }
	public Vector3 topRight		{ get { return m_tr; } }
	public Vector3 bottomLeft	{ get { return m_bl; } }
	public Vector3 bottomRight	{ get { return m_br; } }
	
	public float width			
	{ 
		get 
		{ 
			if(float.IsNaN(m_width))
				m_width = Vector3.Distance(m_tr, m_tl);
			return m_width; 
		} 
	}
	
	public float height	
	{ 
		get 
		{
			if(float.IsNaN(m_height))
				m_height = Vector3.Distance(m_tl, m_bl);
			return m_height;
		} 
	}

	/// <summary>
	/// Defines a 3D rectangle from three points
	/// which must form a right-triangle.
	/// </summary>
	/// <param name="tl">The top-left point</param>
	/// <param name="tr">The top-right point</param>
	/// <param name="bl">The bottom-left point</param>
	public void FromPoints(Vector3 tl, Vector3 tr, Vector3 bl)
	{
		m_tl = tl;
		m_tr = tr;
		m_bl = bl;
		m_br = tr + (bl - tl);

		m_width = m_height = float.NaN;
	}

	/// <summary>
	/// Constructs a new 3D rectangle from three points
	/// which must form a right-triangle.
	/// </summary>
	/// <param name="tl">The top-left point</param>
	/// <param name="tr">The top-right point</param>
	/// <param name="bl">The bottom-left point</param>
	public Rect3D(Vector3 tl, Vector3 tr, Vector3 bl)
	{
		m_tl = m_tr = m_bl = m_br = Vector3.zero;
		m_width = m_height = 0;
		FromPoints(tl, tr, bl);
	}

	/// <summary>
	/// Constructs a new 3D rectangle from a standard
	/// Rect.
	/// </summary>
	/// <param name="r">A Rect upon which the 3D rect will be based.</param>
	public Rect3D(Rect r)
	{
		m_tl = m_tr = m_bl = m_br = Vector3.zero;
		m_width = m_height = 0;
		FromRect(r);
	}

	/// <summary>
	/// Returns a Rect just using the x and y coordinates
	/// of the 3D rect.
	/// </summary>
	/// <returns>A new Rect based upon the X,Y coordinates of the 3D rect.</returns>
	public Rect GetRect()
	{
		return Rect.MinMaxRect(m_bl.x, m_bl.y, m_tr.x, m_tl.y);
	}

	/// <summary>
	/// Defines a Rect3D from a standard Rect.
	/// </summary>
	/// <param name="r">The rect from which to define the 3D rect.</param>
	public void FromRect(Rect r)
	{
		FromPoints( new Vector3(r.xMin, r.yMax, 0),
					new Vector3(r.xMax, r.yMax, 0),
					new Vector3(r.xMin, r.yMin, 0));
	}

	/// <summary>
	/// Multiplies the points in the Rect3D by the specified
	/// matrix in a non-projective way.  Alters the contents
	/// of the Rect3D.
	/// </summary>
	/// <param name="matrix">The matrix by which the points shall be transformed.</param>
	public void MultFast(Matrix4x4 matrix)
	{
		m_tl = matrix.MultiplyPoint3x4(m_tl);
		m_tr = matrix.MultiplyPoint3x4(m_tr);
		m_bl = matrix.MultiplyPoint3x4(m_bl);
		m_br = matrix.MultiplyPoint3x4(m_br);

		// Invalidate our width and height since it
		// may have changed:
		m_width = m_height = float.NaN;
	}

	/// <summary>
	/// Multiplies the points in the specified Rect3D by the 
	/// specified matrix in a non-projective way and returns 
	/// the result.
	/// </summary>
	/// <param name="rect">The Rect3D to be transformed.</param>
	/// <param name="matrix">The matrix by which the points shall be transformed.</param>
	public static Rect3D MultFast(Rect3D rect, Matrix4x4 matrix)
	{
		return new Rect3D(matrix.MultiplyPoint3x4(rect.m_tl),
						  matrix.MultiplyPoint3x4(rect.m_tr),
						  matrix.MultiplyPoint3x4(rect.m_bl));
	}
}






/// <summary>
/// The root class of all sprites.
/// Does not assume any animation capabilities
/// or atlas packing.
/// </summary>
[ExecuteInEditMode]
public abstract class SpriteRoot : MonoBehaviour, IEZLinkedListItem<ISpriteAnimatable>, IUseCamera
{
	/// <remarks>
	/// The plane in which a sprite should be drawn.
	/// </remarks>
	public enum SPRITE_PLANE
	{
		XY,
		XZ,
		YZ
	};

	/// <remarks>
	/// The anchoring scheme of a sprite.  The anchor point is the
	/// point on the sprite that will remain stationary when the
	/// sprite's size changes.
	/// <example>For a health bar that "grows" to the right while
	/// its left edge remains stationary, you would use UPPER_LEFT,
	/// MIDDLE_LEFT, or BOTTOM_LEFT.</example>
	/// <example>For a health bar that "grows" upward while the
	/// bottom edge remains stationary, you would use BOTTOM_LEFT,
	/// BOTTOM_CENTER, or BOTTOM_RIGHT.</example>
	/// </remarks>
	public enum ANCHOR_METHOD
	{
		UPPER_LEFT,
		UPPER_CENTER,
		UPPER_RIGHT,
		MIDDLE_LEFT,
		MIDDLE_CENTER,
		MIDDLE_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT,
		TEXTURE_OFFSET		// The sprite is moved relative to the 
		// GameObject's center based on its 
		// position on the original texture.
	}

	/// <remarks>
	/// Defines which way the polygons of a sprite
	/// should be wound.  The two options are
	/// clock-wise (CW) and counter clock-wise (CCW).
	/// These determine the direction the sprite will "face".
	/// </remarks>
	public enum WINDING_ORDER
	{
		CCW,		// Counter-clockwise
		CW			// Clockwise
	};

	/// <remarks>
	/// Defines a delegate that can be called upon resizing of the sprite.
	/// Use this if you want to adjust colliders, etc, when the sprites
	/// dimensions are resized.
	/// </remarks>
	public delegate void SpriteResizedDelegate(float newWidth, float newHeight, SpriteRoot sprite);


	//---------------------------------------
	// Data members:
	//---------------------------------------

	/// <summary>
	/// When true, the sprite will be managed by the selected sprite manager script.
	/// When false, the sprite has its own mesh and will be batched automatically
	/// with other sprites (when available - Unity iPhone).
	/// </summary>
	public bool managed = false;

	/// <summary>
	/// Reference to the manager which will manage this sprite,
	/// provided managed is set to true.
	/// </summary>
	public SpriteManager manager;

	// Is set to try by the managing SpriteManager when this
	// sprite has been added.
	protected bool addedToManager = false;

	/// <summary>
	/// The layer in which the sprite will be drawn if managed.
	/// </summary>
	public int drawLayer;

	/// <summary>
	/// This must be set to true at design time for the sprite to survive loading a new level.
	/// </summary>
	public bool persistent = false;

	/// <summary>
	/// The plane in which the sprite will be drawn.
	/// </summary>
	public SPRITE_PLANE plane = SPRITE_PLANE.XY;// The plane in which the sprite will be drawn

	/// <summary>
	/// The winding order of the sprite's polygons - determines
	/// the direction the sprite will "face".
	/// </summary>
	public WINDING_ORDER winding = WINDING_ORDER.CW;

	/// <summary>
	/// Width of the sprite in world space.
	/// </summary>
	public float width;							// Width and Height of the sprite in worldspace units

	/// <summary>
	/// Height of the sprite in world space.
	/// </summary>
	public float height;

	/// <summary>
	/// Will contract the UV edges of the sprite 
	/// by the specified amount to prevent "bleeding" 
	/// from neighboring pixels, especially when mipmapping.
	/// </summary>
	public Vector2 bleedCompensation;			// Will contract the UV edges of the sprite to prevent "bleeding" from neighboring pixels, especially when mipmapping

	/// <summary>
	/// Anchor method to use. <seealso cref="ANCHOR_METHOD"/>
	/// </summary>
	public ANCHOR_METHOD anchor = ANCHOR_METHOD.TEXTURE_OFFSET;

	/// <summary>
	/// Automatically sizes the sprite so that it will 
	/// display pixel-perfect on-screen.
	/// NOTE: If you change the orthographic size of 
	/// the camera or the distance between the sprite 
	/// and a perspective camera, call SetCamera()
	/// to make the text pixel-perfect again.
	/// However, if you want automatic resizing functionality
	/// without being pixel-perfect and therefore allowing
	/// zooming in and out, use <see cref="autoResize"/> instead.
	/// </summary>
	public bool pixelPerfect = false;			// Automatically sizes the sprite so that it will display pixel-perfect on-screen

	/// <summary>
	/// Automatically resizes the sprite based on its new 
	/// UV dimensions compared to its previous dimensions.
	/// Setting this to true allows you to use non-uniform
	/// sized sprites for animation without causing the
	/// sprite to appear "squashed" while animating.
	/// </summary>
	public bool autoResize = false;				// Automatically resizes the sprite based on its new UV dimensions compared to its previous dimensions

	protected Vector2 bleedCompensationUV;		// UV-space version of bleedCompensation
	protected Vector2 bleedCompensationUVMax;	// Same, but used for the "max" parts of the UV rect (is double bleedCompensationUV, and negated).
	protected SPRITE_FRAME frameInfo = new SPRITE_FRAME(0);	// All the info we need to define our current frame
	protected Rect uvRect;						// UVs of the current frame
	protected Vector2 scaleFactor = new Vector2(0.5f, 0.5f);// Scale factor of the current frame (see SPRITE_FRAME)
	protected Vector2 topLeftOffset = new Vector2(-1f, 1f);	// Top-left offset of the current frame (see SPRITE_FRAME)
	protected Vector2 bottomRightOffset = new Vector2(1f, -1f);	// Bottom-right offset of the current frame (see SPRITE_FRAME)
	/*
		protected Vector2 lowerLeftUV;				// UV coordinate for the upper-left corner of the sprite
		protected Vector2 uvDimensions;				// Distance from the upper-left UV to place the other UVs
	*/
	protected Vector3 topLeft;					// The adjustment needed for the current anchoring scheme
	protected Vector3 bottomRight;				// The adjustment needed for the current anchoring scheme

	protected Vector3 unclippedTopLeft;			// Where the corner would be without any clipping or trimming.
	protected Vector3 unclippedBottomRight;		// Where the corner would be without any clipping or trimming.

	// (top-left) Will hold values from 0-1 that indicate by how much the sprite should be "truncated" on each side.
	protected Vector2 tlTruncate = new Vector2(1f, 1f);
	// (bottom-right) "
	protected Vector2 brTruncate = new Vector2(1f, 1f);
	protected bool truncated;					// Keeps track of whether truncation is to be applied - gets set to true when any truncation value is set to anything other than 1.0.
	protected Rect3D clippingRect;				// Rect against which the sprite will be clipped.
	protected Rect localClipRect;				// Will hold clippingRect in local space.
	protected float leftClipPct = 1f;			// The percentage of the sprite that remains unclipped
	protected float rightClipPct= 1f;			// The percentage of the sprite that remains unclipped
	protected float topClipPct = 1f;			// The percentage of the sprite that remains unclipped
	protected float bottomClipPct = 1f;			// The percentage of the sprite that remains unclipped
	protected bool clipped = false;				// Keeps track of whether we are to be clipped by a bounding box

	[HideInInspector]
	public bool billboarded = false;			// Is the sprite to be billboarded? (not currently supported)
	[System.NonSerialized]
	public bool isClone = false;				// Set this to true when the sprite has been instantiated from another sprite in the scene.
	[System.NonSerialized]
	public bool uvsInitialized = false;			// This is set to true when the sprite's UVs have been initialized.
	protected bool m_started = false;			// This gets set when the sprite has entered Start()
	protected bool deleted = false;				// This is set to true when the sprite's mesh has been deleted in preparation for destruction.

	/// <summary>
	/// Offsets the sprite, in world space, from the center of its
	/// GameObject.
	/// </summary>
	public Vector3 offset = new Vector3();		// Offset of sprite from center of client GameObject

	/// <summary>
	/// The color to be used by all four of the sprite's
	/// vertices.  This can be used to color, highlight,
	/// or fade the sprite. Be sure to use a vertex-colored
	/// shader for this to have an effect.
	/// </summary>
	public Color color = Color.white;			// The color to be used by all four vertices

	// Reference to the class that will encapsulate
	// all mesh operations.  This will allow us to
	// decide later whether this will be a batched
	// or a managed sprite:
	protected ISpriteMesh m_spriteMesh;

	// References to other sprite objects used 
	// for stepping through sprite lists:
	protected ISpriteAnimatable m_prev;
	protected ISpriteAnimatable m_next;

	// Vars that make pixel-perfect sizing and
	// automatic sizing work:
	protected Vector2 screenSize;				// The size of the screen in pixels
	public Camera renderCamera;
	protected Vector2 sizeUnitsPerUV;			// The width and height of the sprite, in local units, per UV. This is used with auto-resize to determine what the width/height of the sprite should be given its current UVs.
	[HideInInspector]
	public Vector2 pixelsPerUV;				// The number of pixels in both axes per UV unit
	protected float worldUnitsPerScreenPixel;	// The number of world units in both axes per screen pixel

	protected SpriteResizedDelegate resizedDelegate = null; // Delegate to be called upon sprite resizing.


	// Reference to the attached EZScreenPlacement component, if any
	protected EZScreenPlacement screenPlacer;

	// Start-up state vars:
	/// <summary>
	/// Whether the sprite will be hideAtStart when it starts.
	/// </summary>
	public bool hideAtStart = false;

	// Will tell us if we INTEND for the sprite to be hideAtStart,
	// so that if the mesh renderer happens to be incidentally
	// disabled, such as on a prefab that is uninstantiated,
	// we don't mistake that for being hideAtStart.
	// We set this when we intentionally hide the sprite.
	protected bool m_hidden = false;

	/// <summary>
	/// When true, the sprite will not be clipped.
	/// </summary>
	public bool ignoreClipping = false;


	// Used to help us update the sprite's 
	// appearance in the editor:
	protected SpriteRootMirror mirror = null;

	// Working vars:
	protected Vector2 tempUV;
	protected Mesh oldMesh;
	protected SpriteManager savedManager; // When we're disabled, save a backup of the manager we used, so if it wasn't a deletion, we can re-add ourselves back on enable.


	protected virtual void Awake()
	{
		// This will get reset SetCamera() in Start() (if running the game),
		// or in Start() via DoMirror() if the game isn't running.
		screenSize.x = 0;
		screenSize.y = 0;

		// Determine if we are a clone:
#if UNITY_FLASH
		if (StringEndsWith(name, "(Clone)"))
#else
		if (name.EndsWith("(Clone)"))
#endif
			isClone = true;

		if (!managed)
		{
			MeshFilter mf = (MeshFilter)GetComponent(typeof(MeshFilter));
			if (mf != null)
			{
				oldMesh = mf.sharedMesh;
				mf.sharedMesh = null;
			}

			AddMesh();
		}
		else
		{
			if (manager != null)
			{
				manager.AddSprite(this);
			}
			else
				Debug.LogError("Managed sprite \"" + name + "\" has not been assigned to a SpriteManager!");
		}
	}


	public virtual void Start()
	{
		m_started = true;

		if (!managed)
		{
			if (Application.isPlaying)
			{
				// Free the default sharedMesh:
				if (!isClone)
					Destroy(oldMesh);
				oldMesh = null;
			}
		}
		else if (m_spriteMesh != null)
			Init();

		// If this sprite is to persist, prevent it from being
		// destroyed on load:
		if (persistent)
		{
			DontDestroyOnLoad(this);
			if (m_spriteMesh is SpriteMesh)
				((SpriteMesh)m_spriteMesh).SetPersistent();
		}

		if (m_spriteMesh == null && !managed)
			AddMesh();

		// Calculate our original dimensions per UV:
		CalcSizeUnitsPerUV();

		if (m_spriteMesh != null)
		{
			if (m_spriteMesh.texture != null)
			{
				SetPixelToUV(m_spriteMesh.texture);
			}
		}

		if (renderCamera == null)
			renderCamera = Camera.mainCamera;

		SetCamera(renderCamera);

		if (clipped)
			UpdateUVs();

		if(hideAtStart)
			Hide(true);
	}


	protected void CalcSizeUnitsPerUV()
	{
		Rect uvs = frameInfo.uvs;

		// Avoid a divide-by-zero and the problem that will
		// set our dimensions to 0 when the script
		// is re-imported:
		if (uvs.width == 0 || uvs.height == 0 || (uvs.xMin == 1f && uvs.yMin == 1f))
		{
			sizeUnitsPerUV = Vector2.zero;
			return;
		}

		sizeUnitsPerUV.x = width / uvs.width;
		sizeUnitsPerUV.y = height / uvs.height;
	}


	protected virtual void Init()
	{
		// Get our screen placer, if any:
		screenPlacer = (EZScreenPlacement)GetComponent(typeof(EZScreenPlacement));
		
		//if(!Application.isPlaying)
		{
			if (screenPlacer != null)
				screenPlacer.SetCamera(renderCamera);
		}

		if(m_spriteMesh != null)
		{
			// If this sprite is to persist, prevent it from being
			// destroyed on load:
			if (persistent && !managed)
			{
				DontDestroyOnLoad(((SpriteMesh)m_spriteMesh).mesh);
			}

			if (m_spriteMesh.texture != null)
			{
				SetPixelToUV(m_spriteMesh.texture);
			}

			m_spriteMesh.Init();
		}

		// Re-calculate it here since Start() won't be called
		// while in-editor:
		if (!Application.isPlaying)
			CalcSizeUnitsPerUV();
	}

	/// <summary>
	/// Resets important sprite values to defaults for reuse.
	/// </summary>
	public virtual void Clear()
	{
		billboarded = false;
		SetColor(Color.white);
		offset = Vector3.zero;
	}

	/// <summary>
	/// Copies all the vital attributes of another sprite.
	/// </summary>
	/// <param name="s">Source sprite to be copied.</param>
	public virtual void Copy(SpriteRoot s)
	{
		// Copy the material:
		if (!managed)
		{
			if (m_spriteMesh != null && s.spriteMesh != null)
				((SpriteMesh)m_spriteMesh).material = s.spriteMesh.material;
			else if (!s.managed)
			{
				renderer.sharedMaterial = s.renderer.sharedMaterial;
			}
		}

		drawLayer = s.drawLayer;

		// Copy the camera:
		if (s.renderCamera != null)
			SetCamera(s.renderCamera);
		if (renderCamera == null)
			renderCamera = Camera.main;

		if (m_spriteMesh != null)
		{
			if (m_spriteMesh.texture != null)
				SetPixelToUV(m_spriteMesh.texture);
			else if (!managed)
			{
				((SpriteMesh)m_spriteMesh).material = renderer.sharedMaterial;
				SetPixelToUV(m_spriteMesh.texture);
			}
		}

		plane = s.plane;
		winding = s.winding;
		offset = s.offset;
		anchor = s.anchor;
		bleedCompensation = s.bleedCompensation;
		autoResize = s.autoResize;
		pixelPerfect = s.pixelPerfect;
		ignoreClipping = s.ignoreClipping;

		uvRect = s.uvRect;
		scaleFactor = s.scaleFactor;
		topLeftOffset = s.topLeftOffset;
		bottomRightOffset = s.bottomRightOffset;

		width = s.width;
		height = s.height;

		SetColor(s.color);
	}

	// Derived versions should calculate UVs 
	// in a manner appropriate to the derived
	// class.  Be sure to call the base
	// implementation as well at the end.
	public virtual void InitUVs()
	{
		// Copy over any value in frameInfo:
		uvRect = frameInfo.uvs;
	}

	/// <summary>
	/// If non-managed, call Delete() before destroying 
	/// this component or the GameObject to which it is 
	/// attached. Memory leaks can ensue otherwise.
	/// </summary>
	public virtual void Delete()
	{
		deleted = true;

		// Destroy our mesh:
		if (!managed && Application.isPlaying)
		{
			Destroy(((SpriteMesh)spriteMesh).mesh);
			((SpriteMesh)spriteMesh).mesh = null;
		}
	}

	protected virtual void OnEnable()
	{
		// Only do this at runtime, or else some managed 
		// sprites will disappear inexplicably:
		//if (Application.isPlaying)
		{
			if (managed && manager != null && m_started)
			{
				SPRITE_FRAME oldFrame = frameInfo;
				manager.AddSprite(this);
				// Restore our previous frame info since
				// AddSprite() re-inits our UVs:
				frameInfo = oldFrame;
				uvRect = frameInfo.uvs;
				SetBleedCompensation();
			}
			else if (savedManager != null)
			{
				savedManager.AddSprite(this);
			}
		}
	}

	protected virtual void OnDisable()
	{
		// Only do this at runtime, or else some managed 
		// sprites will disappear inexplicably:
		//if (Application.isPlaying)
		{
			// Make sure if we are being deleted that
			// we are removed from the manager so that
			// we don't wind up with a null bone:
			if (managed && manager != null)
			{
				savedManager = manager;
				manager.RemoveSprite(this);
			}
		}
	}

	public virtual void OnDestroy()
	{
		Delete();
	}


	// Sets the edge positions needed to properly
	// orient our sprite according to our anchoring
	// method:
	public void CalcEdges()
	{
		switch (anchor)
		{
			case ANCHOR_METHOD.TEXTURE_OFFSET:
				// sizeFactor is a number that, when multiplied by the
				// width and height of the sprite, yields half the total
				// size the sprite would be if it occupied the entire
				// original texture (half so we avoid an additional
				// mult by 0.5 when calculating our edges):

				// halfSizeIfFull is half the size the sprite would be
				// if the entire area of the original texture were being
				// used:
				Vector2 halfSizeIfFull;
				halfSizeIfFull.x = width * scaleFactor.x;
				halfSizeIfFull.y = height * scaleFactor.y;

				// Adjust the offsets if the width has already
				// been adjusted via pixelPerfect or autoResize:
				/*
								if((pixelPerfect || autoResize) && truncated)
								{
									topLeftOffset.x = bottomRightOffset.x - (bottomRightOffset.x - topLeftOffset.x) * tlTruncate.x;
									topLeftOffset.y = bottomRightOffset.y - (bottomRightOffset.y - topLeftOffset.y) * tlTruncate.y;
									bottomRightOffset.x = topLeftOffset.x - (topLeftOffset.x - bottomRightOffset.x) * brTruncate.x;
									bottomRightOffset.y = topLeftOffset.y - (topLeftOffset.y - bottomRightOffset.y) * brTruncate.y;
								}
				*/

				topLeft.x = halfSizeIfFull.x * topLeftOffset.x;
				topLeft.y = halfSizeIfFull.y * topLeftOffset.y;
				bottomRight.x = halfSizeIfFull.x * bottomRightOffset.x;
				bottomRight.y = halfSizeIfFull.y * bottomRightOffset.y;
				break;
			case ANCHOR_METHOD.UPPER_LEFT:
				topLeft.x = 0;
				topLeft.y = 0;
				bottomRight.x = width;
				bottomRight.y = -height;
				break;
			case ANCHOR_METHOD.UPPER_CENTER:
				topLeft.x = width * -0.5f;
				topLeft.y = 0;
				bottomRight.x = width * 0.5f;
				bottomRight.y = -height;
				break;
			case ANCHOR_METHOD.UPPER_RIGHT:
				topLeft.x = -width;
				topLeft.y = 0;
				bottomRight.x = 0;
				bottomRight.y = -height;
				break;
			case ANCHOR_METHOD.MIDDLE_LEFT:
				topLeft.x = 0;
				topLeft.y = height * 0.5f;
				bottomRight.x = width;
				bottomRight.y = height * -0.5f;
				break;
			case ANCHOR_METHOD.MIDDLE_CENTER:
				topLeft.x = width * -0.5f;
				topLeft.y = height * 0.5f;
				bottomRight.x = width * 0.5f;
				bottomRight.y = height * -0.5f;
				break;
			case ANCHOR_METHOD.MIDDLE_RIGHT:
				topLeft.x = -width;
				topLeft.y = height * 0.5f;
				bottomRight.x = 0;
				bottomRight.y = height * -0.5f;
				break;
			case ANCHOR_METHOD.BOTTOM_LEFT:
				topLeft.x = 0;
				topLeft.y = height;
				bottomRight.x = width;
				bottomRight.y = 0;
				break;
			case ANCHOR_METHOD.BOTTOM_CENTER:
				topLeft.x = width * -0.5f;
				topLeft.y = height;
				bottomRight.x = width * 0.5f;
				bottomRight.y = 0;
				break;
			case ANCHOR_METHOD.BOTTOM_RIGHT:
				topLeft.x = -width;
				topLeft.y = height;
				bottomRight.x = 0;
				bottomRight.y = 0;
				break;
		}

		unclippedTopLeft = topLeft + offset;
		unclippedBottomRight = bottomRight + offset;

		if (truncated)
		{
			// If we haven't already adjusted the size
			// based on our UV truncation:
			topLeft.x = bottomRight.x - (bottomRight.x - topLeft.x) * tlTruncate.x;
			topLeft.y = bottomRight.y - (bottomRight.y - topLeft.y) * tlTruncate.y;
			bottomRight.x = topLeft.x - (topLeft.x - bottomRight.x) * brTruncate.x;
			bottomRight.y = topLeft.y - (topLeft.y - bottomRight.y) * brTruncate.y;
		}

		// If we're clipping and our sprite isn't of 0 size on one or more sides:
		if(clipped && bottomRight.x - topLeft.x != 0 && topLeft.y - bottomRight.y != 0)
		{
			Vector3 origTL = topLeft;
			Vector3 origBR = bottomRight;
			Rect tempClipRect = localClipRect;

			// Account for any offset:
			tempClipRect.x -= offset.x;
			tempClipRect.y -= offset.y;

			// Clip the sprite horizontally:
			if (topLeft.x < tempClipRect.x)
			{
				// Trim the sprite:
				leftClipPct = 1f - (tempClipRect.x - origTL.x) / (origBR.x - origTL.x);
				topLeft.x = Mathf.Clamp(tempClipRect.x, origTL.x, origBR.x);

				if (leftClipPct <= 0)
					topLeft.x = bottomRight.x = tempClipRect.x;
			}
			else
				leftClipPct = 1;

			if (bottomRight.x > tempClipRect.xMax)
			{
				// Trim the sprite:
				rightClipPct = (tempClipRect.xMax - origTL.x) / (origBR.x - origTL.x);
				bottomRight.x = Mathf.Clamp(tempClipRect.xMax, origTL.x, origBR.x);

				if (rightClipPct <= 0)
					bottomRight.x = topLeft.x = tempClipRect.xMax;
			}
			else
				rightClipPct = 1;

			// Clip the sprite vertically:
			if (topLeft.y > tempClipRect.yMax)
			{
				// Trim the sprite:
				topClipPct = (tempClipRect.yMax - origBR.y) / (origTL.y - origBR.y);
				topLeft.y = Mathf.Clamp(tempClipRect.yMax, origBR.y, origTL.y);

				if (topClipPct <= 0)
					topLeft.y = bottomRight.y = tempClipRect.yMax;
			}
			else
				topClipPct = 1;

			if (bottomRight.y < tempClipRect.y)
			{
				// Trim the sprite:
				bottomClipPct = 1f - (tempClipRect.y - origBR.y) / (origTL.y - origBR.y);
				bottomRight.y = Mathf.Clamp(tempClipRect.y, origBR.y, origTL.y);

				if (bottomClipPct <= 0)
					bottomRight.y = topLeft.y = tempClipRect.y;
			}
			else
				bottomClipPct = 1;
		}

		// Reverse the X positions if the winding order is CCW:
		if(winding == WINDING_ORDER.CCW)
		{
			topLeft.x *= -1f;
			bottomRight.x *= -1f;
		}
	}

	// Sets the width and height of the sprite based upon
	// the change in its UV dimensions
	/// <summary>
	/// Recalculates the width and height of the sprite
	/// based upon the change in its UV dimensions (autoResize) or
	/// on the current camera's disposition (pixelPerfect).
	/// </summary>
	public void CalcSize()
	{
		// Make sure we don't have a zero-sized UV rect:
		if (uvRect.width == 0)
			uvRect.width = 0.0000001f;
		if (uvRect.height == 0)
			uvRect.height = 0.0000001f;

		if (pixelPerfect)
		{
			// Calculate the size based on the camera's disposition:
			//worldUnitsPerScreenPixel = (renderCamera.orthographicSize * 2f) / screenSize.y;
			width = worldUnitsPerScreenPixel * frameInfo.uvs.width * pixelsPerUV.x;
			height = worldUnitsPerScreenPixel * frameInfo.uvs.height * pixelsPerUV.y;
		}
		else if (autoResize) // Else calculate the size based on the change in UV dimensions:
		{
			if (sizeUnitsPerUV.x != 0 && sizeUnitsPerUV.y != 0)
			{
				// Change the width and height according to the new UV dimensions:
				width = frameInfo.uvs.width * sizeUnitsPerUV.x;
				height = frameInfo.uvs.height * sizeUnitsPerUV.y;
			}
		}

		SetSize(width, height);
	}

	/// <summary>
	/// Sets the physical dimensions of the sprite in the 
	/// plane selected
	/// </summary>
	/// <param name="width">Width of the sprite in world space.</param>
	/// <param name="height">Height of the sprite in world space.</param>
	public virtual void SetSize(float w, float h)
	{
		if (m_spriteMesh == null)
			return;

		width = w;
		height = h;

		CalcSizeUnitsPerUV();

		switch (plane)
		{
			case SPRITE_PLANE.XY:
				SetSizeXY(width, height);
				break;
			case SPRITE_PLANE.XZ:
				SetSizeXZ(width, height);
				break;
			case SPRITE_PLANE.YZ:
				SetSizeYZ(width, height);
				break;
		}

		if (resizedDelegate != null)
			resizedDelegate(width, height, this);
	}

	// Sets the physical dimensions of the sprite in the XY plane:
	protected void SetSizeXY(float w, float h)
	{
		CalcEdges();

		Vector3[] vertices = m_spriteMesh.vertices;

		if(winding == WINDING_ORDER.CW)
		{
			// Upper-left
			vertices[0].x = offset.x + topLeft.x;
			vertices[0].y = offset.y + topLeft.y;
			vertices[0].z = offset.z;

			// Lower-left
			vertices[1].x = offset.x + topLeft.x;
			vertices[1].y = offset.y + bottomRight.y;
			vertices[1].z = offset.z;

			// Lower-right
			vertices[2].x = offset.x + bottomRight.x;
			vertices[2].y = offset.y + bottomRight.y;
			vertices[2].z = offset.z;

			// Upper-right
			vertices[3].x = offset.x + bottomRight.x;
			vertices[3].y = offset.y + topLeft.y;
			vertices[3].z = offset.z;
		}
		else
		{
			// Upper-left
			vertices[0].x = offset.x + topLeft.x;
			vertices[0].y = offset.y + topLeft.y;
			vertices[0].z = offset.z;

			// Lower-left
			vertices[1].x = offset.x + topLeft.x;
			vertices[1].y = offset.y + bottomRight.y;
			vertices[1].z = offset.z;

			// Lower-right
			vertices[2].x = offset.x + bottomRight.x;
			vertices[2].y = offset.y + bottomRight.y;
			vertices[2].z = offset.z;

			// Upper-right
			vertices[3].x = offset.x + bottomRight.x;
			vertices[3].y = offset.y + topLeft.y;
			vertices[3].z = offset.z;

		}

		m_spriteMesh.UpdateVerts();
	}

	// Sets the physical dimensions of the sprite in the XZ plane:
	protected void SetSizeXZ(float w, float h)
	{
		CalcEdges();

		Vector3[] vertices = m_spriteMesh.vertices;

		// Upper-left
		vertices[0].x = offset.x + topLeft.x;
		vertices[0].y = offset.y;
		vertices[0].z = offset.z + topLeft.y;

		// Lower-left
		vertices[1].x = offset.x + topLeft.x;
		vertices[1].y = offset.y;
		vertices[1].z = offset.z + bottomRight.y;

		// Lower-right
		vertices[2].x = offset.x + bottomRight.x;
		vertices[2].y = offset.y;
		vertices[2].z = offset.z + bottomRight.y;

		// Upper-right
		vertices[3].x = offset.x + bottomRight.x;
		vertices[3].y = offset.y;
		vertices[3].z = offset.z + topLeft.y;

		m_spriteMesh.UpdateVerts();
	}

	// Sets the physical dimensions of the sprite in the YZ plane:
	protected void SetSizeYZ(float w, float h)
	{
		CalcEdges();

		Vector3[] vertices = m_spriteMesh.vertices;

		// Upper-left
		vertices[0].x = offset.x;
		vertices[0].y = offset.y + topLeft.y;
		vertices[0].z = offset.z + topLeft.x;

		// Lower-left
		vertices[1].x = offset.x;
		vertices[1].y = offset.y + bottomRight.y;
		vertices[1].z = offset.z + topLeft.x;

		// Lower-right
		vertices[2].x = offset.x;
		vertices[2].y = offset.y + bottomRight.y;
		vertices[2].z = offset.z + bottomRight.x;

		// Upper-right
		vertices[3].x = offset.x;
		vertices[3].y = offset.y + topLeft.y;
		vertices[3].z = offset.z + bottomRight.x;

		m_spriteMesh.UpdateVerts();
	}

	/// <summary>
	/// Truncates the right edge of the sprite to the specified percentage.
	/// 1 == no truncation
	/// 0 == complete truncation
	/// </summary>
	/// <param name="pct">The percentage of the sprite to truncate (0-1)</param>
	public virtual void TruncateRight(float pct)
	{
		tlTruncate.x = 1f;
		brTruncate.x = Mathf.Clamp01(pct);
		if (brTruncate.x < 1f || tlTruncate.y < 1f || brTruncate.y < 1f)
			truncated = true;
		else
		{
			Untruncate();
			return;
		}

		UpdateUVs();
		CalcSize();
	}

	/// <summary>
	/// Truncates the left edge of the sprite to the specified percentage.
	/// 1 == no truncation
	/// 0 == complete truncation
	/// </summary>
	/// <param name="pct">The percentage of the sprite to truncate (0-1)</param>
	public virtual void TruncateLeft(float pct)
	{
		tlTruncate.x = Mathf.Clamp01(pct);
		brTruncate.x = 1f;
		if (tlTruncate.x < 1f || tlTruncate.y < 1f || brTruncate.y < 1f)
			truncated = true;
		else
		{
			Untruncate();
			return;
		}

		UpdateUVs();
		CalcSize();
	}

	/// <summary>
	/// Truncates the top edge of the sprite to the specified percentage.
	/// 1 == no truncation
	/// 0 == complete truncation
	/// </summary>
	/// <param name="pct">The percentage of the sprite to truncate (0-1)</param>
	public virtual void TruncateTop(float pct)
	{
		tlTruncate.y = Mathf.Clamp01(pct);
		brTruncate.y = 1f;
		if (tlTruncate.y < 1f || tlTruncate.x < 1f || brTruncate.x < 1f)
			truncated = true;
		else
		{
			Untruncate();
			return;
		}

		UpdateUVs();
		CalcSize();
	}

	/// <summary>
	/// Truncates the bottom edge of the sprite to the specified percentage.
	/// 1 == no truncation
	/// 0 == complete truncation
	/// </summary>
	/// <param name="pct">The percentage of the sprite to truncate (0-1)</param>
	public virtual void TruncateBottom(float pct)
	{
		tlTruncate.y = 1f;
		brTruncate.y = Mathf.Clamp01(pct);
		if (brTruncate.y < 1f || tlTruncate.x < 1f || brTruncate.x < 1f)
			truncated = true;
		else
		{
			Untruncate();
			return;
		}

		UpdateUVs();
		CalcSize();
	}

	/// <summary>
	/// Removes any truncation.
	/// </summary>
	public virtual void Untruncate()
	{
		tlTruncate.x = 1f;
		tlTruncate.y = 1f;
		brTruncate.x = 1f;
		brTruncate.y = 1f;
		truncated = false;

		uvRect = frameInfo.uvs;// Need to make sure we reset

		SetBleedCompensation();// Redo bleed compensation since we just re-copied the raw framinfo which lacks this
		CalcSize();
	}

	/// <summary>
	/// Removes any clipping that is being applied to the
	/// sprite.
	/// </summary>
	public virtual void Unclip()
	{
		if (ignoreClipping)
			return;

		leftClipPct = 1f;
		rightClipPct = 1f;
		topClipPct = 1f;
		bottomClipPct = 1f;
		clipped = false;
		uvRect = frameInfo.uvs;// Need to make sure we reset
		SetBleedCompensation();// Redo bleed compensation since we just re-copied the raw framinfo which lacks this
		CalcSize();
	}

	/// <summary>
	/// Applies any changes to the UVs to the actual sprite mesh.
	/// </summary>
	public virtual void UpdateUVs()
	{
		scaleFactor = frameInfo.scaleFactor;
		topLeftOffset = frameInfo.topLeftOffset;
		bottomRightOffset = frameInfo.bottomRightOffset;

		// Truncate our UVs if needed:
		if (truncated)
		{
			uvRect.x = frameInfo.uvs.xMax + bleedCompensationUV.x - (frameInfo.uvs.width) * tlTruncate.x * leftClipPct;
			uvRect.y = frameInfo.uvs.yMax + bleedCompensationUV.y - (frameInfo.uvs.height) * brTruncate.y * bottomClipPct;
			uvRect.xMax = frameInfo.uvs.x + bleedCompensationUVMax.x + (frameInfo.uvs.width) * brTruncate.x * rightClipPct;
			uvRect.yMax = frameInfo.uvs.y + bleedCompensationUVMax.y + (frameInfo.uvs.height) * tlTruncate.y * topClipPct;
		}
		else if(clipped)
		{
			Rect baseUV = Rect.MinMaxRect(frameInfo.uvs.x + bleedCompensationUV.x, frameInfo.uvs.y + bleedCompensationUV.y, frameInfo.uvs.xMax + bleedCompensationUVMax.x, frameInfo.uvs.yMax + bleedCompensationUVMax.y);
			uvRect.x = Mathf.Lerp(baseUV.xMax, baseUV.x, leftClipPct);
			uvRect.y = Mathf.Lerp(baseUV.yMax, baseUV.y, bottomClipPct);
			uvRect.xMax = Mathf.Lerp(baseUV.x, baseUV.xMax, rightClipPct);
			uvRect.yMax = Mathf.Lerp(baseUV.y, baseUV.yMax, topClipPct);
		}

		if (m_spriteMesh == null)
			return;

		Vector2[] uvs = m_spriteMesh.uvs;

//		if (winding == WINDING_ORDER.CW)
		{
			uvs[0].x = uvRect.x; uvs[0].y = uvRect.yMax;
			uvs[1].x = uvRect.x; uvs[1].y = uvRect.y;
			uvs[2].x = uvRect.xMax; uvs[2].y = uvRect.y;
			uvs[3].x = uvRect.xMax; uvs[3].y = uvRect.yMax;
		}
/*	Commented out because this should now be addressed by
 *  the fact that we reverse the X position of the vertices.
		else
		{
			uvs[3].x = uvRect.x; uvs[3].y = uvRect.yMax;
			uvs[2].x = uvRect.x; uvs[2].y = uvRect.y;
			uvs[1].x = uvRect.xMax; uvs[1].y = uvRect.y;
			uvs[0].x = uvRect.xMax; uvs[0].y = uvRect.yMax;
		}
*/

		m_spriteMesh.UpdateUVs();
	}

	// Applies the transform of the client GameObject and stores
	// the results in the associated vertices of the overall mesh:
	public void TransformBillboarded(Transform t)
	{	//Todo
		/*
				Vector3 pos = clientTransform.position;

				meshVerts[mv1] = pos + t.InverseTransformDirection(v1);
				meshVerts[mv2] = pos + t.InverseTransformDirection(v2);
				meshVerts[mv3] = pos + t.InverseTransformDirection(v3);
				meshVerts[mv4] = pos + t.InverseTransformDirection(v4);

				m_manager.UpdatePositions();
		 */
	}

	/// <summary>
	/// Sets the sprite's color to the specified color.
	/// </summary>
	/// <param name="c">Color to shade the sprite.</param>
	public virtual void SetColor(Color c)
	{
		color = c;

		// Update vertex colors:
		if(m_spriteMesh != null)
			m_spriteMesh.UpdateColors(color);
	}

	/// <summary>
	/// Accessor for the object's current overall color tint.
	/// </summary>
	public Color Color
	{
		get { return color; }
		set { SetColor(value); }
	}

	// Sets the number of pixels per UV unit:
	public void SetPixelToUV(int texWidth, int texHeight)
	{
		Vector2 oldPPUV = pixelsPerUV;

		pixelsPerUV.x = texWidth;
		pixelsPerUV.y = texHeight;

		// Recalculate our size-per-UV based on the
		// current size-to-pixel ratio.
		// NOTE: Assumes the sprite will be resized
		// via CalcSize() immediately after this has
		// been called.
		Rect uvs = frameInfo.uvs;
		// Avoid NaN/Infinity:
		if (uvs.width == 0 || uvs.height == 0 || oldPPUV.x == 0 || oldPPUV.y == 0)
			return;
		Vector2 sizePerTexel = new Vector2(width / (uvs.width * oldPPUV.x), height / (uvs.height * oldPPUV.y));
		sizeUnitsPerUV.x = sizePerTexel.x * pixelsPerUV.x;
		sizeUnitsPerUV.y = sizePerTexel.y * pixelsPerUV.y;
	}

	// Sets the number of pixels per UV unit:
	public void SetPixelToUV(Texture tex)
	{
		if (tex == null)
			return;
		SetPixelToUV(tex.width, tex.height);
	}

	/// <summary>
	/// Recalculates the pixel-to-UV ratio based on the
	/// current texture.
	/// </summary>
	public void CalcPixelToUV()
	{
		if(managed)
		{
			if (spriteMesh != null && spriteMesh.material != null && spriteMesh.material.mainTexture != null)
				SetPixelToUV(spriteMesh.material.mainTexture);
		}
		else
		{
			if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
				SetPixelToUV(renderer.sharedMaterial.mainTexture);
		}
	}

	/// <summary>
	/// Changes the texture to be used by the sprite's material.
	/// NOTE: This will cause the sprite not to batch with other
	/// sprites and can only be used with non-managed sprites.
	/// </summary>
	/// <param name="tex">The new texture.</param>
	public void SetTexture(Texture2D tex)
	{
		if (managed || renderer == null)
			return;

		renderer.material.mainTexture = tex;

		SetPixelToUV(tex);

		SetCamera();
	}


	/// <summary>
	/// Changes the material to be used by the sprite.
	/// NOTE: This can only be used with non-managed sprites.
	/// </summary>
	/// <param name="mat">The new material.</param>
	public void SetMaterial(Material mat)
	{
		if (managed || renderer == null)
			return;

		renderer.sharedMaterial = mat;

		SetPixelToUV(mat.mainTexture);

		SetCamera();
	}

	/// <summary>
	/// Accessor for the camera that will be used to render this object.
	/// Use this to ensure the object is properly configured for the
	/// specific camera that will render it.
	/// </summary>
	public virtual Camera RenderCamera
	{
		get { return renderCamera; }
		set 
		{
			renderCamera = value;
			SetCamera(value); 
		}
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

	// Sets the camera to use when calculating 
	// pixel-perfect sprite size:
	/// <summary>
	/// Sets the camera to use when calculating
	/// a pixel-perfect sprite size.
	/// </summary>
	/// <param name="c"></param>
	public virtual void SetCamera(Camera c)
	{
		if (c == null || !m_started)
			return;

		float dist;
		Plane nearPlane = new Plane(c.transform.forward, c.transform.position);

		if (!Application.isPlaying)
		{
			// If the screenSize has never been initialized,
			// or if this is a different camera, get what 
			// values we can get, otherwise just keep the 
			// values we got during our last run:
#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			if ((screenSize.x == 0 || c != renderCamera) && c.pixelHeight > 100)
#endif
			{
				screenSize.x = c.pixelWidth;
				screenSize.y = c.pixelHeight;
			}

			if (screenSize.x == 0)
				return;

			renderCamera = c;

			if (screenPlacer != null)
				screenPlacer.SetCamera(renderCamera);

			// Determine the world distance between two vertical
			// screen pixels for this camera:
			dist = nearPlane.GetDistanceToPoint(transform.position);
			//worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0, 1, dist)), c.ScreenToWorldPoint(new Vector3(0, 0, dist)));
			worldUnitsPerScreenPixel = 1;

			if(!hideAtStart)
				CalcSize();
			return;
		}

		renderCamera = c;
		screenSize.x = c.pixelWidth;
		screenSize.y = c.pixelHeight;

		if (screenPlacer != null)
			screenPlacer.SetCamera(renderCamera);

		// Determine the world distance between two vertical
		// screen pixels for this camera:
		dist = nearPlane.GetDistanceToPoint(transform.position);
		worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0, 1, dist)), c.ScreenToWorldPoint(new Vector3(0, 0, dist)));

		CalcSize();
	}

	/// <summary>
	/// Hides or displays the sprite by disabling/enabling
	/// the sprite's mesh renderer component, or if managed,
	/// sets the mesh size to 0.
	/// </summary>
	/// <param name="tf">When true, the sprite is hideAtStart, when false, the sprite will be displayed.</param>
	public virtual void Hide(bool tf)
	{
		if (m_spriteMesh != null)
			m_spriteMesh.Hide(tf);
		m_hidden = tf;
	}

	/// <summary>
	/// Returns whether the sprite is currently set to be hideAtStart
	/// (whether its mesh renderer component is enabled).
	/// </summary>
	/// <returns>True when hideAtStart, false when set to be displayed.</returns>
	public bool IsHidden()
	{
		return m_hidden;
// 		if (m_spriteMesh != null)
// 			return m_spriteMesh.IsHidden();
// 		else
// 			return true;
	}


	/// <summary>
	/// Gets/Sets the width and height of the sprite in pixels as it appears on-screen.
	/// </summary>
	public Vector2 PixelSize
	{
		get { return new Vector2(width * worldUnitsPerScreenPixel, height * worldUnitsPerScreenPixel); }
		set
		{
			SetSize(value.x * worldUnitsPerScreenPixel, value.y * worldUnitsPerScreenPixel);
		}
	}

	/// <summary>
	/// Gets the width and height of the image the sprite is displaying.
	/// NOTE: This is not the number of screen pixels occupied by the sprite,
	/// but rather the original number of texels which compose the sprite's image.
	/// </summary>
	public Vector2 ImageSize
	{
		get { return new Vector2(uvRect.width * pixelsPerUV.x, uvRect.height * pixelsPerUV.y);  }
	}


	/// <summary>
	/// Sets the sprite to a managed or batched state.
	/// </summary>
	public bool Managed
	{
		get { return managed; }

		set
		{
			if (value)
			{
				if (!managed)
					DestroyMesh();
				managed = value;
			}
			else
			{
				if (managed)
				{
					if (manager != null)
						manager.RemoveSprite(this);
					manager = null;
				}

				managed = value;

				if (m_spriteMesh == null)
					AddMesh();
				else if (!(m_spriteMesh is SpriteMesh))
					AddMesh();
			}
		}
	}

	// Has the sprite entered Start() yet?
	public bool Started
	{
		get { return m_started; }
	}

	// Called when the sprite is
	// changed to a managed sprite:
	protected void DestroyMesh()
	{
		// Get rid of our non-managed sprite mesh:
		if(m_spriteMesh != null)
			m_spriteMesh.sprite = null;
		m_spriteMesh = null;
		// Destroy our unneeded components:
		if(renderer != null)
			DestroyImmediate(renderer);
		Object filter = gameObject.GetComponent(typeof(MeshFilter));
		if(filter != null)
			DestroyImmediate(filter);
	}

	// Called when the sprite is
	// made a batched/independent mesh.
	protected void AddMesh()
	{
		m_spriteMesh = new SpriteMesh();
		m_spriteMesh.sprite = this;
	}


	//--------------------------------------------------------------
	// Accessors:
	//--------------------------------------------------------------

	public void SetBleedCompensation() { SetBleedCompensation(bleedCompensation); }

	/// <summary>
	/// Sets the bleed compensation to use (see <see cref="bleedCompensation"/>).
	/// </summary>
	public void SetBleedCompensation(float x, float y) { SetBleedCompensation(new Vector2(x, y)); }

	/// <summary>
	/// Sets the bleed compensation to use (see <see cref="bleedCompensation"/>).
	/// </summary>
	public void SetBleedCompensation(Vector2 xy)
	{
		bleedCompensation = xy;
		bleedCompensationUV = PixelSpaceToUVSpace(bleedCompensation);
		bleedCompensationUVMax = bleedCompensationUV * -2f;

		uvRect.x += bleedCompensationUV.x;
		uvRect.y += bleedCompensationUV.y;
		uvRect.xMax += bleedCompensationUVMax.x;
		uvRect.yMax += bleedCompensationUVMax.y;

		UpdateUVs();
	}

	/// <summary>
	/// Sets the plane in which the sprite is to be drawn. See: <see cref="SPRITE_PLANE"/>
	/// </summary>
	/// <param name="p">The plane in which the sprite should be drawn.</param>
	public void SetPlane(SPRITE_PLANE p)
	{
		plane = p;
		SetSize(width, height);
	}

	/// <summary>
	/// Sets the winding order to use. See <see cref="WINDING_ORDER"/>.
	/// </summary>
	/// <param name="order">The winding order to use.</param>
	public void SetWindingOrder(WINDING_ORDER order)
	{
		winding = order;

		if(!managed && m_spriteMesh != null)
			((SpriteMesh)m_spriteMesh).SetWindingOrder(order);
	}

	/// <summary>
	/// Sets the draw layer of the sprite (only applies to
	/// managed sprites).
	/// </summary>
	/// <param name="layer">The draw layer of the sprite. Lower values cause an earlier draw order, higher values, a later draw order.</param>
	public void SetDrawLayer(int layer)
	{
		if (!managed)
			return;

		drawLayer = layer;
		((SpriteMesh_Managed)m_spriteMesh).drawLayer = layer;
		if (manager != null)
			manager.SortDrawingOrder();
	}

	/// <summary>
	/// Sets the sprite's frame info, which includes UVs,
	/// offsets, etc.
	/// </summary>
	/// <param name="fInfo">A SPRITE_FRAME structure containing the frame info.</param>
	public void SetFrameInfo(SPRITE_FRAME fInfo)
	{
		frameInfo = fInfo;
		uvRect = fInfo.uvs;

		SetBleedCompensation();

		if (autoResize || pixelPerfect)
			CalcSize();
	}

	/// <summary>
	/// Sets the sprite's UVs to the specified values.
	/// </summary>
	/// <param name="nextFrame">A Rect containing the new UV coordinates.</param>
	public void SetUVs(Rect uv)
	{
		frameInfo.uvs = uv;
		uvRect = uv;

		SetBleedCompensation();

		// Re-calculate it here since Start() won't be called
		// while in-editor:
		if (!Application.isPlaying)
			CalcSizeUnitsPerUV();

		if (autoResize || pixelPerfect)
			CalcSize();
	}

	/// <summary>
	/// Sets the sprite's UVs from pixel coordinates.
	/// </summary>
	/// <param name="pxCoords">A rect containing the pixel coordinates.
	/// When populating the Rect, use the following syntax:
	/// Rect.MinMaxRect(leftCoord, bottomCoord, rightCoord, topCoord)
	/// Where the coordinates are in pixel space.  Like in any image editor,
	/// the coordinate space runs from 0 on the left to width-1 on the right,
	/// and 0 at the top, to height-1 at the bottom.</param>
	public void SetUVsFromPixelCoords(Rect pxCoords)
	{
		tempUV = PixelCoordToUVCoord((int)pxCoords.x, (int)pxCoords.yMax);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;

		tempUV = PixelCoordToUVCoord((int)pxCoords.xMax, (int)pxCoords.y);
		uvRect.xMax = tempUV.x;
		uvRect.yMax = tempUV.y;

		frameInfo.uvs = uvRect;

		SetBleedCompensation();

		if (autoResize || pixelPerfect)
			CalcSize();
	}

	/// <summary>
	/// Returns the current UV coordinates of the sprite (before bleed compensation).
	/// </summary>
	/// <returns>Rect containing the sprite's UV coordinates.</returns>
	public Rect GetUVs()
	{
		return uvRect;
	}

	/// <summary>
	/// Returns a reference to the sprite's vertices.
	/// NOTE: You can only directly modify the sprite's
	/// vertices if it is a non-managed sprite.
	/// </summary>
	/// <returns>A reference to the sprite's vertices.</returns>
	public Vector3[] GetVertices()
	{
		if (!managed)
			return ((SpriteMesh)m_spriteMesh).mesh.vertices;
		else
			return m_spriteMesh.vertices;
	}

	/// <summary>
	/// Gets the center point of the sprite, taking
	/// into account the actual positions of vertices.
	/// </summary>
	/// <returns>The center point of the sprite.</returns>
	public Vector3 GetCenterPoint()
	{
		if (m_spriteMesh == null)
			return Vector3.zero;

		Vector3[] verts = m_spriteMesh.vertices;

		switch(plane)
		{
			case SPRITE_PLANE.XY:
				return new Vector3(verts[0].x + 0.5f * (verts[2].x - verts[0].x), verts[0].y - 0.5f * (verts[0].y - verts[2].y), offset.z);
			case SPRITE_PLANE.XZ:
				return new Vector3(verts[0].x + 0.5f * (verts[2].x - verts[0].x), offset.y, verts[0].z - 0.5f * (verts[0].z - verts[2].z));
			case SPRITE_PLANE.YZ:
				return new Vector3(offset.x, verts[0].y - 0.5f * (verts[0].y - verts[2].y), verts[0].z - 0.5f * (verts[0].z - verts[2].z));
			default:
				return new Vector3(verts[0].x + 0.5f * (verts[2].x - verts[0].x), verts[0].y - 0.5f * (verts[0].y - verts[2].y), offset.z);
		}
	}

	/// <summary>
	/// The rect against which the sprite should be clipped.  
	/// The sprite will be immediately clipped by this rect when set.
	/// When setting, the rect should be in world space.
	/// </summary>
	public virtual Rect3D ClippingRect
	{
		get { return clippingRect; }
		set
		{
			if (ignoreClipping)
				return;

			clippingRect = value;

			localClipRect = Rect3D.MultFast(clippingRect, transform.worldToLocalMatrix).GetRect();

			clipped = true;
			CalcSize();
			UpdateUVs();
		}
	}


	/// <summary>
	/// Accessor for whether the sprite is to be clipped
	/// by any already-specified clipping rect.
	/// </summary>
	public virtual bool Clipped
	{
		get { return clipped; }
		set
		{
			if (ignoreClipping)
				return;

			if (value && !clipped)
			{
				clipped = true;
				CalcSize();
			}
			else if (clipped)
				Unclip();
		}
	}

	/// <summary>
	/// Sets the anchor method to use.
	/// See <see cref="ANCHOR_METHOD"/>
	/// </summary>
	/// <param name="a">The anchor method to use.</param>
	public void SetAnchor(ANCHOR_METHOD a)
	{
		anchor = a;

		SetSize(width, height);
	}

	/// <summary>
	/// Accessor for the object's anchor method.
	/// </summary>
	public ANCHOR_METHOD Anchor
	{
		get { return anchor; }
		set { SetAnchor(value); }
	}

	/// <summary>
	/// Sets the offset of the sprite from its
	/// GameObject.
	/// See <see cref="offset"/>
	/// </summary>
	/// <param name="o">The offset to use.</param>
	public void SetOffset(Vector3 o)
	{
		offset = o;
		SetSize(width, height);
	}

	/// <summary>
	/// The top-left corner of the sprite when
	/// no clipping or trimming is applied.
	/// </summary>
	public Vector3 UnclippedTopLeft
	{
		get 
		{
			// If we're being asked for our outline, then
			// we need to have already started:
			if (!m_started)
				Start();

			return unclippedTopLeft; 
		}
	}

	/// <summary>
	/// The bottom-right corner of the sprite when
	/// no clipping or trimming is applied.
	/// </summary>
	public Vector3 UnclippedBottomRight
	{
		get
		{		
			// If we're being asked for our outline, then
			// we need to have already started:
			if (!m_started)
				Start();
 
			return unclippedBottomRight; 
		}
	}

	/// <summary>
	/// Returns the position of the top-left vertex
	/// of the sprite after any clipping or trimming.
	/// </summary>
	public Vector3 TopLeft
	{
		get
		{
			if (m_spriteMesh != null)
				return m_spriteMesh.vertices[0];
			else
				return Vector3.zero;
		}
	}

	/// <summary>
	/// Returns the position of the bottom-right vertex
	/// of the sprite after any clipping or trimming.
	/// </summary>
	public Vector3 BottomRight
	{
		get
		{
			if (m_spriteMesh != null)
				return m_spriteMesh.vertices[2];
			else
				return Vector3.zero;
		}
	}

	// Accessor for the sprite's mesh manager
	public ISpriteMesh spriteMesh
	{
		get { return m_spriteMesh; }
		set
		{
			m_spriteMesh = value;
			if (m_spriteMesh != null)
			{
				if (m_spriteMesh.sprite != this)
					m_spriteMesh.sprite = this;
			}
			else
				return;

			if (managed)
				manager = ((SpriteMesh_Managed)m_spriteMesh).manager;
		}
	}

	// Called by the managing SpriteManager to set
	// whether the sprite has been added or not:
	public bool AddedToManager
	{
		get { return addedToManager; }
		set	{ addedToManager = value; }
	}

	// Returns pixel dimensions of the sprite when in its default appearance
	// (the appearance of the sprite in the editor):
	public abstract Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader);


	//--------------------------------------------------------------
	// Utility methods:
	//--------------------------------------------------------------

	/// <summary>
	/// Converts pixel-space values to UV-space scalar values
	/// according to the currently assigned material.
	/// NOTE: This is for converting widths and heights-not
	/// coordinates (which have reversed Y-coordinates).
	/// For coordinates, use <see cref="PixelCoordToUVCoord"/>()!
	/// </summary>
	/// <param name="xy">The values to convert.</param>
	/// <returns>The values converted to UV space.</returns>
	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		if (pixelsPerUV.x == 0 || pixelsPerUV.y == 0)
			return Vector2.zero;

		return new Vector2(xy.x / pixelsPerUV.x, xy.y / pixelsPerUV.y);
	}

	/// <summary>
	/// Converts pixel-space values to UV-space scalar values
	/// according to the currently assigned material.
	/// NOTE: This is for converting widths and heights-not
	/// coordinates (which have reversed Y-coordinates).
	/// For coordinates, use <see cref="PixelCoordToUVCoord"/>()!
	/// </summary>
	/// <param name="x">The X-value to convert.</param>
	/// <param name="y">The Y-value to convert.</param>
	/// <returns>The values converted to UV space.</returns>
	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2((float)x, (float)y));
	}

	/// <summary>
	/// Converts pixel coordinates to UV coordinates according to
	/// the currently assigned material.
	/// NOTE: This is for converting coordinates and will reverse
	/// the Y component accordingly.  For converting widths and
	/// heights, use <see cref="PixelSpaceToUVSpace"/>()!
	/// </summary>
	/// <param name="xy">The coordinates to convert.</param>
	/// <returns>The coordinates converted to UV coordinates.</returns>
	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		if (pixelsPerUV.x == 0 || pixelsPerUV.y == 0)
			return Vector2.zero;

		return new Vector2(xy.x / (pixelsPerUV.x - 1f), 1.0f - (xy.y / (pixelsPerUV.y - 1f)));
	}

	/// <summary>
	/// Converts pixel coordinates to UV coordinates according to
	/// the currently assigned material.
	/// NOTE: This is for converting coordinates and will reverse
	/// the Y component accordingly.  For converting widths and
	/// heights, use <see cref="PixelSpaceToUVSpace"/>()!
	/// </summary>
	/// <param name="x">The x-coordinate to convert.</param>
	/// <param name="y">The y-coordinate to convert.</param>
	/// <returns>The coordinates converted to UV coordinates.</returns>
	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2((float)x, (float)y));
	}

	/// <summary>
	/// Returns the index of the state with the specified name.
	/// -1 if no state matching the specified name is found.
	/// </summary>
	/// <param name="stateName">The name of the state sought.</param>
	/// <returns>The zero-based index of the state, or -1 if no state is found by that name.</returns>
	public abstract int GetStateIndex(string stateName);

	/// <summary>
	/// Sets the sprite to the specified state/animation.
	/// </summary>
	/// <param name="index">The zero-based index of the desired state/animation.</param>
	public abstract void SetState(int index);


	public ISpriteAnimatable prev
	{
		get { return m_prev; }
		set { m_prev = value; }
	}

	public ISpriteAnimatable next
	{
		get { return m_next; }
		set { m_next = value; }
	}

	// Uses the mirror object to validate and respond
	// to changes in our inspector.
	public virtual void DoMirror()
	{
		// Only run if we're not playing:
		if (Application.isPlaying)
			return;

		// This means Awake() was recently called, meaning
		// we couldn't reliably get valid camera viewport
		// sizes, so we zeroed them out so we'd know to
		// get good values later on (when OnDrawGizmos()
		// is called):
		if (screenSize.x == 0 || screenSize.y == 0)
			Start();

		if (mirror == null)
		{
			mirror = new SpriteRootMirror();
			mirror.Mirror(this);
		}

		mirror.Validate(this);

		// Compare our mirrored settings to the current settings
		// to see if something was changed:
		if (mirror.DidChange(this))
		{
			Init();
			mirror.Mirror(this);	// Update the mirror
		}
	}

	// Included to work around the Unity bug where Start() is not
	// called when reentering edit mode if play lasts for longer 
	// than 10 seconds:
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9) && UNITY_EDITOR
	void Update() 
	{
		DoMirror();
	}

	public virtual void OnDrawGizmosSelected()
	{
	}
#else
	// Ensures that the sprite is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmosSelected()
	{
		DoMirror();
	}

	// Ensures that the sprite is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmos()
	{
		DoMirror();
	}
#endif

#if UNITY_FLASH
	// A custom "EndsWith()" implementation since Unity flash lacks this:
	protected bool StringEndsWith(string toSearch, string sought)
	{
		if (toSearch.Length < sought.Length)
			return false;

		for (int i = sought.Length - 1, j = toSearch.Length - 1; i >= 0; --i, --j)
			if (toSearch[j] != sought[i])
				return false;

		return true;
	}
#endif
}


// Mirrors the editable settings of a sprite that affect
// how the sprite is drawn in the scene view
public class SpriteRootMirror
{
	public bool managed;
	public SpriteManager manager;
	public int drawLayer;
	public SpriteRoot.SPRITE_PLANE plane;
	public SpriteRoot.WINDING_ORDER winding;
	public float width, height;
	public Vector2 bleedCompensation;
	public SpriteRoot.ANCHOR_METHOD anchor;
	public Vector3 offset;
	public Color color;
	public bool pixelPerfect;
	public bool autoResize;
	public Camera renderCamera;
	public bool hideAtStart;
/*
	public Vector3 pos;
	public Vector3 scale;
	public Quaternion rot;
*/

	// Mirrors the specified sprite's settings
	public virtual void Mirror(SpriteRoot s)
	{
		managed = s.managed;
		manager = s.manager;
		drawLayer = s.drawLayer;
		plane = s.plane;
		winding = s.winding;
		width = s.width;
		height = s.height;
		bleedCompensation = s.bleedCompensation;
		anchor = s.anchor;
		offset = s.offset;
		color = s.color;
		pixelPerfect = s.pixelPerfect;
		autoResize = s.autoResize;
		renderCamera = s.renderCamera;
		hideAtStart = s.hideAtStart;
/*
		pos = s.transform.position;
		scale = s.transform.localScale;
		rot = s.transform.localRotation;
*/
	}

	// Validates certain settings:
	public virtual bool Validate(SpriteRoot s)
	{
		if (s.pixelPerfect)
		{
			s.autoResize = true;
		}

/*
		if (s.transform.position != pos ||
			s.transform.localScale != scale ||
			s.transform.localRotation != rot)
		{
			pos = s.transform.position;
			scale = s.transform.localScale;
			rot = s.transform.localRotation;
			if (s.managed)
			{
				if (s.manager != null)
					s.manager.UpdatePositions();
			}
		}
*/
		return true;
	}

	// Returns true if any of the settings do not match:
	public virtual bool DidChange(SpriteRoot s)
	{
		if (s.managed != managed)
		{
			HandleManageState(s);
			return true;
		}
		if (s.manager != manager)
		{
			UpdateManager(s);
			return true;
		}
		if (s.drawLayer != drawLayer)
		{
			HandleDrawLayerChange(s);
			return true;
		}
		if (s.plane != plane)
			return true;
		if (s.winding != winding)
			return true;
		if (s.width != width)
			return true;
		if (s.height != height)
			return true;
		if (s.bleedCompensation != bleedCompensation)
			return true;
		if (s.anchor != anchor)
			return true;
		if (s.offset != offset)
			return true;
		if (s.color.r != color.r ||
			s.color.g != color.g ||
			s.color.b != color.b ||
			s.color.a != color.a)
			return true;
		if (s.pixelPerfect != pixelPerfect)
			return true;
		if (s.autoResize != autoResize)
			return true;
		if (s.renderCamera != renderCamera)
			return true;

		if (s.hideAtStart != hideAtStart)
		{
			s.Hide(s.hideAtStart);
			return true;
		}

		return false;
	}

	// Handles things when the managed state of the
	// sprite is changed:
	protected virtual void HandleManageState(SpriteRoot s)
	{
		// Since the Managed property checks the previous
		// value, we'll need to set it back temporarily
		// and it will get updated to the current desired
		// value internally:
		s.managed = managed;
		s.Managed = !managed;
	}

	public virtual void UpdateManager(SpriteRoot s)
	{
		if (!s.managed)
			s.manager = null;
		else
		{
			if(manager != null)
				manager.RemoveSprite(s);
			if(s.manager != null)
				s.manager.AddSprite(s);
		}
	}

	protected virtual void HandleDrawLayerChange(SpriteRoot s)
	{
		if(!s.managed)
		{	// Don't let it be changed to underscore
			// the fact that this is invalid/useless
			// if not managed:
			s.drawLayer = 0;
			return;
		}

		s.SetDrawLayer(s.drawLayer);
	}
}