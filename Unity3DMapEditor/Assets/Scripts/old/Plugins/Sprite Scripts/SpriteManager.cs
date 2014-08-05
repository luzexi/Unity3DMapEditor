//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


#define SPRITE_WANT_NORMALS
#define USE_GENERICS


using UnityEngine;
using System.Collections;

#if USE_GENERICS
using System.Collections.Generic;
#endif


/// <summary>
/// Allows multiple sprites to be combined into a single mesh.
/// Sprites are transformed using bones which are linked to
/// GameObjects.
/// </summary>
[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class SpriteManager : MonoBehaviour 
{
	/// <summary>
	/// Whether sprite polygons should be wound 
	/// clock-wise or counter clock-wise.  
	/// Determines which side of the sprite is 
	/// considered by the renderer to be the "front".
	/// </summary>
	public SpriteRoot.WINDING_ORDER winding = SpriteRoot.WINDING_ORDER.CW;

	/// <summary>
	/// How many sprites to allocate at a time 
	/// if the sprite pool is used up.
	/// This is also the starting pool size.
	/// </summary>
	public int allocBlockSize = 10;			// How many sprites to allocate space for at a time. ex: if set to 10, 10 new sprite blocks will be allocated at a time. Once all of these are used, 10 more will be allocated, and so on...

	/// <summary>
	/// When true, will automatically recalculate
	/// the bounding box for the mesh whenever sprite
	/// vertices change.
	/// If the bounding box is never recalculated,
	/// when the camera moves, Unity may think the
	/// mesh is out of sight and cull it, even though
	/// it is not.
	/// </summary>
	public bool autoUpdateBounds = true;	// Automatically recalculate the bounds of the mesh when vertices change?
	//public bool ensureUnique = true;		// Ensures that all sprite additions are unique by checking added sprites against the current list

	public bool drawBoundingBox = false;

	/// <summary>
	/// If true, the SpriteManager object and associated mesh
	/// will survive a level load.
	/// </summary>
	public bool persistent = false;

	protected bool initialized = false;

	protected EZLinkedList<SpriteMesh_Managed> availableBlocks = new EZLinkedList<SpriteMesh_Managed>(); // Array of references to sprites which are currently not in use
	protected bool vertsChanged = false;	// Have changes been made to the vertices of the mesh since the last frame?
	protected bool uvsChanged = false;		// Have changes been made to the UVs of the mesh since the last frame?
	protected bool colorsChanged = false;	// Have the colors changed?
	protected bool vertCountChanged = false;// Has the number of vertices changed?
	protected bool updateBounds = false;	// Update the mesh bounds?
	protected SpriteMesh_Managed[] sprites;	// Array of all sprites (the offset of the vertices corresponding to each sprite should be found simply by taking the sprite's index * 4 (4 verts per sprite).
	protected EZLinkedList<SpriteMesh_Managed> activeBlocks = new EZLinkedList<SpriteMesh_Managed>();	// Array of references to all the currently active (non-empty) sprites
	//protected ArrayList activeBillboards = new ArrayList(); // Array of references to all the *active* sprites which are to be rendered as billboards
#if USE_GENERICS
	protected List<SpriteMesh_Managed> spriteDrawOrder = new List<SpriteMesh_Managed>();	// Array of indices of sprite objects stored in the order they are to be drawn (corresponding to the position of their vertex indices in the triIndices list)  Allows us to keep track of where a given Sprite is in the drawing order (triIndices)
#else
	protected ArrayList spriteDrawOrder = new ArrayList();
#endif
	protected SpriteDrawLayerComparer drawOrderComparer = new SpriteDrawLayerComparer(); // Used to sort our draw order array
	protected float boundUpdateInterval;	// Interval, in seconds, to update the mesh bounds

	protected List<SpriteRoot> spriteAddQueue; // List of sprites to be added that we have accumilated before initialization

	protected SkinnedMeshRenderer meshRenderer;
	protected Mesh mesh;					// Reference to our mesh (contained in the SkinnedMeshRenderer)
	protected Texture texture;

	protected Transform[] bones;			// The bones by which we'll transform our sprites' vertices
	protected BoneWeight[] boneWeights;		// Bone weights - Each element corresponds to a vertex in the vertices array, and indicates which bones, and at what weight, will influence the vertex.
	protected Matrix4x4[] bindPoses;		// Refers the bind pose of each bone
	protected Vector3[] vertices;			// The vertices of our mesh
	protected int[] triIndices;				// Indices into the vertex array
	protected Vector2[] UVs;				// UV coordinates
	protected Vector2[] UVs2;				// UV coordinates
	protected Color[] colors;				// Color values

	// Working vars:
	protected SpriteMesh_Managed tempSprite = null;

	//--------------------------------------------------------------
	// Utility functions:
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
		if (texture == null)
			return Vector2.zero;

		return new Vector2(xy.x / ((float)texture.width), xy.y / ((float)texture.height));
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
		if (texture == null)
			return Vector2.zero;

		return new Vector2(xy.x / ((float)texture.width - 1), 1.0f - (xy.y / ((float)texture.height - 1)));
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

	// Setups up bone weights for a sprite
	protected void SetupBoneWeights(SpriteMesh_Managed s)
	{
		// Each bone element corresponds to each sprite element:
		boneWeights[s.mv1].boneIndex0 = s.index;
		boneWeights[s.mv1].weight0 = 1f;
		boneWeights[s.mv2].boneIndex0 = s.index;
		boneWeights[s.mv2].weight0 = 1f;
		boneWeights[s.mv3].boneIndex0 = s.index;
		boneWeights[s.mv3].weight0 = 1f;
		boneWeights[s.mv4].boneIndex0 = s.index;
		boneWeights[s.mv4].weight0 = 1f;
	}

	//--------------------------------------------------------------
	// End utility functions
	//--------------------------------------------------------------

	void Awake()
	{
		if (spriteAddQueue == null)
			spriteAddQueue = new List<SpriteRoot>();
		
		// Make sure the manager is centered:
		//transform.position = Vector3.zero;

		meshRenderer = (SkinnedMeshRenderer)GetComponent(typeof(SkinnedMeshRenderer));

		if(meshRenderer != null)
			if(meshRenderer.sharedMaterial != null)
				texture = meshRenderer.sharedMaterial.GetTexture("_MainTex");

		if (meshRenderer.sharedMesh == null)
			meshRenderer.sharedMesh = new Mesh();
		mesh = meshRenderer.sharedMesh;

		if(persistent)
		{
			DontDestroyOnLoad(this);
			DontDestroyOnLoad(mesh);
		}

		// Create our first batch of sprites 'n' such:
		EnlargeArrays(allocBlockSize);

		// Move the object to the origin so the objects drawn will not
		// be offset from the objects they are intended to represent.
		//transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		initialized = true;

		// Now process any outstanding sprite additions:
		for(int i=0; i<spriteAddQueue.Count; ++i)
			AddSprite(spriteAddQueue[i]);
	}

	// Allocates initial arrays
	protected void InitArrays()
	{
		bones = new Transform[1];
		bones[0] = transform;	// Just give it something for now
		bindPoses = new Matrix4x4[1];
		sprites = new SpriteMesh_Managed[1];
		sprites[0] = new SpriteMesh_Managed();
		vertices = new Vector3[4];
		UVs = new Vector2[4];
		colors = new Color[4];
		triIndices = new int[6];
		boneWeights = new BoneWeight[4];

		sprites[0].index = 0;
		sprites[0].mv1 = 0;
		sprites[0].mv2 = 1;
		sprites[0].mv3 = 2;
		sprites[0].mv4 = 3;
		SetupBoneWeights(sprites[0]);
	}

	// Enlarges the sprite array by the specified count and also resizes
	// the UV and vertex arrays by the necessary corresponding amount.
	// Returns the index of the first newly allocated element
	// (ex: if the sprite array was already 10 elements long and is 
	// enlarged by 10 elements resulting in a total length of 20, 
	// EnlargeArrays() will return 10, indicating that element 10 is the 
	// first of the newly allocated elements.)
	protected int EnlargeArrays(int count)
	{
		int firstNewElement;

		if (sprites == null)
		{
			InitArrays();
			firstNewElement = 0;
			count = count - 1;	// Allocate one less since InitArrays already allocated one sprite for us
		}
		else
			firstNewElement = sprites.Length;

		// Resize sprite array:
		SpriteMesh_Managed[] tempSprites = sprites;
		sprites = new SpriteMesh_Managed[sprites.Length + count];
		tempSprites.CopyTo(sprites, 0);

		// Resize the bone array:
		Transform[] tempBones = bones;
		bones = new Transform[bones.Length + count];
		tempBones.CopyTo(bones, 0);

		// Resize the bind poses array:
		Matrix4x4[] tempPoses = bindPoses;
		bindPoses = new Matrix4x4[bindPoses.Length + count];
		tempPoses.CopyTo(bindPoses, 0);

		// Vertices:
		Vector3[] tempVerts = vertices;
		vertices = new Vector3[vertices.Length + count * 4];
		tempVerts.CopyTo(vertices, 0);

		// BoneWeights:
		BoneWeight[] tempWeights = boneWeights;
		boneWeights = new BoneWeight[boneWeights.Length + count * 4];
		tempWeights.CopyTo(boneWeights, 0);

		// UVs:
		Vector2[] tempUVs = UVs;
		UVs = new Vector2[UVs.Length + count * 4];
		tempUVs.CopyTo(UVs, 0);

		// Colors:
		Color[] tempColors = colors;
		colors = new Color[colors.Length + count * 4];
		tempColors.CopyTo(colors, 0);

		// Triangle indices:
		int[] tempTris = triIndices;
		triIndices = new int[triIndices.Length + count * 6];
		tempTris.CopyTo(triIndices, 0);

		// Inform existing sprites of the new vertex and UV buffers:
		for (int i = 0; i < firstNewElement; ++i)
		{
			sprites[i].SetBuffers(vertices, UVs, UVs2, colors);
		}

		// Setup the newly-added sprites and Add them to the list of available 
		// sprite blocks. Also initialize the triangle indices while we're at it:
		for (int i = firstNewElement; i < sprites.Length; ++i)
		{
			// Create and setup sprite:

			sprites[i] = new SpriteMesh_Managed();
			sprites[i].index = i;
			sprites[i].manager = this;

			sprites[i].SetBuffers(vertices, UVs, UVs2, colors);

			// Setup indices of the sprite's vertices in the vertex buffer:
			sprites[i].mv1 = i * 4 + 0;
			sprites[i].mv2 = i * 4 + 1;
			sprites[i].mv3 = i * 4 + 2;
			sprites[i].mv4 = i * 4 + 3;

			// Setup the indices of the sprite's UV entries in the UV buffer:
			sprites[i].uv1 = i * 4 + 0;
			sprites[i].uv2 = i * 4 + 1;
			sprites[i].uv3 = i * 4 + 2;
			sprites[i].uv4 = i * 4 + 3;

			// Setup the indices to the color values:
			sprites[i].cv1 = i * 4 + 0;
			sprites[i].cv2 = i * 4 + 1;
			sprites[i].cv3 = i * 4 + 2;
			sprites[i].cv4 = i * 4 + 3;

			// Add as an available sprite:
			availableBlocks.Add(sprites[i]);

			// Init triangle indices:
/*			if (winding == SpriteRoot.WINDING_ORDER.CCW)
			{	// Counter-clockwise winding
				triIndices[i * 6 + 0] = i * 4 + 0;	//	0_ 2			0 ___ 3
				triIndices[i * 6 + 1] = i * 4 + 1;	//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = i * 4 + 3;	// 1|/				1|/__|2

				triIndices[i * 6 + 3] = i * 4 + 3;	//	  3
				triIndices[i * 6 + 4] = i * 4 + 1;	//   /|
				triIndices[i * 6 + 5] = i * 4 + 2;	// 4/_|5
			}
			else
*/			{	// Clockwise winding
				triIndices[i * 6 + 0] = i * 4 + 0;	//	0_ 1			0 ___ 3
				triIndices[i * 6 + 1] = i * 4 + 3;	//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = i * 4 + 1;	// 2|/				1|/__|2

				triIndices[i * 6 + 3] = i * 4 + 3;	//	  3
				triIndices[i * 6 + 4] = i * 4 + 2;	//   /|
				triIndices[i * 6 + 5] = i * 4 + 1;	// 5/_|4
			}

			// Add the index of this sprite to the draw order list
			spriteDrawOrder.Add(sprites[i]);

			// Give the bones something to point to:
			bones[i] = transform;

			// Setup a default bindpose:
			bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;

			// Setup the weights:
			SetupBoneWeights(sprites[i]);
		}

		vertsChanged = true;
		uvsChanged = true;
		colorsChanged = true;
		vertCountChanged = true;

		return firstNewElement;
	}

	/// <summary>
	/// Returns whether the sprite is already managed by this
	/// manager.
	/// </summary>
	/// <param name="sprite">Sprite for which to look.</param>
	/// <returns>True when the sprite is already being managed by this manager.</returns>
	public bool AlreadyAdded(SpriteRoot sprite)
	{
		if(activeBlocks.Rewind())
		{
			do
			{
				if (activeBlocks.Current.sprite == sprite)
					return true;
			} while (activeBlocks.MoveNext());
		}

		return false;
	}

	/// <summary>
	/// Adds the sprite attached to the specified GameObject
	/// to the SpriteManager.
	/// </summary>
	/// <param name="go">GameObject containing a sprite.</param>
	/// <returns>A reference to the sprite's managed mesh.</returns>
	public SpriteMesh_Managed AddSprite(GameObject go)
	{
		SpriteRoot sprite = (SpriteRoot)go.GetComponent(typeof(SpriteRoot));
		if (sprite == null)
			return null;

		return AddSprite(sprite);
	}

	/// <summary>
	/// Adds the specified sprite to the SpriteManager.
	/// </summary>
	/// <param name="go">Reference to the desired sprite.</param>
	/// <returns>A reference to the sprite's managed mesh.</returns>
	public SpriteMesh_Managed AddSprite(SpriteRoot sprite)
	{
		int spriteIndex;

		// See if the sprite is already added:
		if(sprite.manager == this && sprite.AddedToManager)
			return (SpriteMesh_Managed)sprite.spriteMesh;
/*
		if (ensureUnique)
			if (AlreadyAdded(sprite))
				return (SpriteMesh_Managed) sprite.spriteMesh;
*/

		// See if we're ready to add sprites yet,
		// and if not, add the sprite to our deferred
		// add queue:
		if(!initialized)
		{
			if (spriteAddQueue == null)
				spriteAddQueue = new List<SpriteRoot>();

			spriteAddQueue.Add(sprite);
			return null;
		}

		// Get an available sprite:
		if (availableBlocks.Empty)
			EnlargeArrays(allocBlockSize);	// If we're out of available sprites, allocate some more:

		// Use a sprite from the list of available blocks:
		spriteIndex = availableBlocks.Head.index;
		availableBlocks.Remove(availableBlocks.Head);	// Now that we're using this one, remove it from the available list

		// Assign the new sprite:
		SpriteMesh_Managed newSpritesMesh = sprites[spriteIndex];
		sprite.spriteMesh = newSpritesMesh;
		sprite.manager = this;
		sprite.AddedToManager = true;
		newSpritesMesh.drawLayer = sprite.drawLayer;

		// Associate the sprite's bone:
		bones[spriteIndex] = sprite.gameObject.transform;
//		meshRenderer.bones = bones;
		bindPoses[spriteIndex] = bones[spriteIndex].worldToLocalMatrix * sprite.transform.localToWorldMatrix;

/*
		// Save this to an active list now that it is in-use:
		if (billboarded)
		{
			newSprite.billboarded = true;
			activeBillboards.Add(newSprite);
		}
		else
*/
			activeBlocks.Add(newSpritesMesh);

		newSpritesMesh.Init();

		// Sort the draw layers:
		SortDrawingOrder();

		// Set our flags:
		vertCountChanged = true;
		vertsChanged = true;
		uvsChanged = true;

		return newSpritesMesh;
	}


	/// <summary>
	/// Instantiates the specified prefab, which should contain
	/// a sprite, and immediately adds it to the manager.
	/// </summary>
	/// <param name="prefab">Prefab to be instantiated.</param>
	/// <returns>Reference to the sprite contained in the prafab.</returns>
	public SpriteRoot CreateSprite(GameObject prefab)
	{
		return CreateSprite(prefab, Vector3.zero, Quaternion.identity);
	}


	/// <summary>
	/// Instantiates the specified prefab, which should contain
	/// a sprite, and immediately adds it to the manager.
	/// </summary>
	/// <param name="prefab">Prefab to be instantiated.</param>
	/// <param name="position">Where to place the new sprite.</param>
	/// <param name="rotation">Rotation of the new sprite.</param>
	/// <returns>Reference to the sprite contained in the prafab.</returns>
	public SpriteRoot CreateSprite(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		GameObject go = (GameObject) Instantiate(prefab, position, rotation);
		SpriteRoot ret = (SpriteRoot) go.GetComponent(typeof(SpriteRoot));
		AddSprite(go);

	//	if (sm == null)
	//		return null;
		
		return ret;
	}

/*
	public void SetBillboarded(Sprite sprite)
	{
		// Make sure the sprite isn't in the active list
		// or else it'll get handled twice:
		activeBlocks.Remove(sprite);
		activeBillboards.Add(sprite);
	}
*/
	/// <summary>
	/// Removes the specified sprite from the manager.
	/// </summary>
	/// <param name="sprite">Sprite to remove from the manager.</param>
	public void RemoveSprite(SpriteRoot sprite)
	{
		if (sprite.spriteMesh is SpriteMesh_Managed && sprite.spriteMesh != null)
		{
			// Disown the sprite:
			if (sprite.manager == this)
			{
				sprite.manager = null;
				sprite.AddedToManager = false;
			}

			RemoveSprite((SpriteMesh_Managed)sprite.spriteMesh);
		}
	}


	// Removes the sprite with the specified managed mesh from the manager.
	public void RemoveSprite(SpriteMesh_Managed sprite)
	{
		vertices[sprite.mv1] = Vector3.zero;
		vertices[sprite.mv2] = Vector3.zero;
		vertices[sprite.mv3] = Vector3.zero;
		vertices[sprite.mv4] = Vector3.zero;

		// Remove the sprite from the billboarded list
		// since that list should only contain active
		// sprites:
/*
		if (sprite.billboarded)
			activeBillboards.Remove(sprite);
		else
*/
			activeBlocks.Remove(sprite);

		// Reset the bone:
		if (gameObject != null)
			bones[sprite.index] = transform;

		// Clean the sprite's settings:
		sprite.Clear();
		sprite.sprite.spriteMesh = null;
		sprite.sprite = null;

		availableBlocks.Add(sprite);

		vertsChanged = true;
	}


	/// <summary>
	/// Moves the specified sprite to the end of the drawing order.
	/// (Causes it to appear in front of other sprites.)
	/// </summary>
	/// <param name="s">Sprite to move to the end of the drawing order.</param>
	public void MoveToFront(SpriteMesh_Managed s)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(s) * 6;

		if (offset < 0)
			return;

		s.drawLayer = spriteDrawOrder[spriteDrawOrder.Count - 1].drawLayer + 1;

		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];

		// Shift all indices from here forward down 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i < triIndices.Length - 6; i += 6)
		{
			triIndices[i] = triIndices[i + 6];
			triIndices[i + 1] = triIndices[i + 7];
			triIndices[i + 2] = triIndices[i + 8];
			triIndices[i + 3] = triIndices[i + 9];
			triIndices[i + 4] = triIndices[i + 10];
			triIndices[i + 5] = triIndices[i + 11];

			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 + 1];
		}

		// Place our desired index value at the end:
		triIndices[triIndices.Length - 6] = indices[0];
		triIndices[triIndices.Length - 5] = indices[1];
		triIndices[triIndices.Length - 4] = indices[2];
		triIndices[triIndices.Length - 3] = indices[3];
		triIndices[triIndices.Length - 2] = indices[4];
		triIndices[triIndices.Length - 1] = indices[5];

		// Update the sprite's index offset:
		spriteDrawOrder[spriteDrawOrder.Count - 1] = s;

		vertCountChanged = true;
	}

	/// <summary>
	/// Moves the specified sprite to the start of the drawing order.
	/// (Causes it to appear behind other sprites.)
	/// </summary>
	/// <param name="s">Sprite to move to the start of the drawing order.</param>
	public void MoveToBack(SpriteMesh_Managed s)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(s) * 6;

		if (offset < 0)
			return;

		s.drawLayer = spriteDrawOrder[0].drawLayer - 1;

		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];

		// Shift all indices from here back up 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i > 5; i -= 6)
		{
			triIndices[i] = triIndices[i - 6];
			triIndices[i + 1] = triIndices[i - 5];
			triIndices[i + 2] = triIndices[i - 4];
			triIndices[i + 3] = triIndices[i - 3];
			triIndices[i + 4] = triIndices[i - 2];
			triIndices[i + 5] = triIndices[i - 1];

			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 - 1];
		}

		// Place our desired index value at the beginning:
		triIndices[0] = indices[0];
		triIndices[1] = indices[1];
		triIndices[2] = indices[2];
		triIndices[3] = indices[3];
		triIndices[4] = indices[4];
		triIndices[5] = indices[5];

		// Update the sprite's index offset:
		spriteDrawOrder[0] = s;

		vertCountChanged = true;
	}

	/// <summary>
	/// Moves the first sprite in front of the second sprite by
	/// placing it later in the draw order. If the sprite is already
	/// in front of the reference sprite, nothing is changed.
	/// </summary>
	/// <param name="toMove">Sprite to move in front of the reference sprite.</param>
	/// <param name="reference">Sprite in front of which "toMove" will be drawn.</param>
	public void MoveInfrontOf(SpriteMesh_Managed toMove, SpriteMesh_Managed reference)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(toMove) * 6;
		int refOffset = spriteDrawOrder.IndexOf(reference) * 6;

		if (offset < 0)
			return;

		// Check to see if the sprite is already in front:
		if (offset > refOffset)
			return;

		toMove.drawLayer = reference.drawLayer + 1;

		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];

		// Shift all indices from here to the reference sprite down 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i < refOffset; i += 6)
		{
			triIndices[i] = triIndices[i + 6];
			triIndices[i + 1] = triIndices[i + 7];
			triIndices[i + 2] = triIndices[i + 8];
			triIndices[i + 3] = triIndices[i + 9];
			triIndices[i + 4] = triIndices[i + 10];
			triIndices[i + 5] = triIndices[i + 11];

			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 + 1];
		}

		// Place our desired index value at the destination:
		triIndices[refOffset] = indices[0];
		triIndices[refOffset + 1] = indices[1];
		triIndices[refOffset + 2] = indices[2];
		triIndices[refOffset + 3] = indices[3];
		triIndices[refOffset + 4] = indices[4];
		triIndices[refOffset + 5] = indices[5];

		// Update the sprite's index offset:
		spriteDrawOrder[refOffset / 6] = toMove;

		vertCountChanged = true;
	}

	/// <summary>
	/// Moves the first sprite behind the second sprite by
	/// placing it earlier in the draw order. If the sprite
	/// is already behind, nothing is done.
	/// </summary>
	/// <param name="toMove">Sprite to move behind the reference sprite.</param>
	/// <param name="reference">Sprite behind which "toMove" will be drawn.</param>
	public void MoveBehind(SpriteMesh_Managed toMove, SpriteMesh_Managed reference)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(toMove) * 6;
		int refOffset = spriteDrawOrder.IndexOf(reference) * 6;

		if (offset < 0)
			return;

		// Check to see if the sprite is already behind:
		if (offset < refOffset)
			return;

		toMove.drawLayer = reference.drawLayer - 1;

		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];

		// Shift all indices from here to the reference sprite up 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i > refOffset; i -= 6)
		{
			triIndices[i] = triIndices[i - 6];
			triIndices[i + 1] = triIndices[i - 5];
			triIndices[i + 2] = triIndices[i - 4];
			triIndices[i + 3] = triIndices[i - 3];
			triIndices[i + 4] = triIndices[i - 2];
			triIndices[i + 5] = triIndices[i - 1];

			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 - 1];
		}

		// Place our desired index value at the destination:
		triIndices[refOffset] = indices[0];
		triIndices[refOffset + 1] = indices[1];
		triIndices[refOffset + 2] = indices[2];
		triIndices[refOffset + 3] = indices[3];
		triIndices[refOffset + 4] = indices[4];
		triIndices[refOffset + 5] = indices[5];

		// Update the sprite's index offset:
		spriteDrawOrder[refOffset / 6] = toMove;

		vertCountChanged = true;
	}


	// Rebuilds the drawing order based upon the drawing order buffer
	public void SortDrawingOrder()
	{
		SpriteMesh_Managed s;

		spriteDrawOrder.Sort(drawOrderComparer);

		// Now reconstitute the triIndices in the order we want:
		if (winding == SpriteRoot.WINDING_ORDER.CCW)
		{
			for (int i = 0; i < spriteDrawOrder.Count; ++i)
			{
				s = (SpriteMesh_Managed)spriteDrawOrder[i];

				// Counter-clockwise winding
				triIndices[i * 6 + 0] = s.mv1;		//	0_ 2			1 ___ 4
				triIndices[i * 6 + 1] = s.mv2;		//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = s.mv4;		// 1|/				2|/__|3

				triIndices[i * 6 + 3] = s.mv4;		//	  3
				triIndices[i * 6 + 4] = s.mv2;		//   /|
				triIndices[i * 6 + 5] = s.mv3;		// 4/_|5
			}
		}
		else
		{
			for (int i = 0; i < spriteDrawOrder.Count; ++i)
			{
				s = (SpriteMesh_Managed)spriteDrawOrder[i];

				// Clockwise winding
				triIndices[i * 6 + 0] = s.mv1;		//	0_ 1			1 ___ 4
				triIndices[i * 6 + 1] = s.mv4;		//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = s.mv2;		// 2|/				2|/__|3

				triIndices[i * 6 + 3] = s.mv4;		//	  3
				triIndices[i * 6 + 4] = s.mv3;		//   /|
				triIndices[i * 6 + 5] = s.mv2;		// 5/_|4
			}
		}

		vertCountChanged = true;
	}

	// Returns the mesh of the sprite at the specified index.
	public SpriteMesh_Managed GetSprite(int i)
	{
		if (i < sprites.Length)
			return sprites[i];
		else
			return null;
	}

	// Updates the vertices of a sprite such that it is oriented
	// more or less toward the camera
/*
	public void TransformBillboarded(Sprite sprite)
	{
		Vector3 pos = sprite.clientTransform.position;
		Transform t = Camera.main.transform;

		vertices[sprite.mv1] = pos + t.TransformDirection(sprite.v1);
		vertices[sprite.mv2] = pos + t.TransformDirection(sprite.v2);
		vertices[sprite.mv3] = pos + t.TransformDirection(sprite.v3);
		vertices[sprite.mv4] = pos + t.TransformDirection(sprite.v4);

		vertsChanged = true;
	}
*/

	// Informs the SpriteManager that some vertices have changed position
	// and the mesh needs to be reconstructed accordingly
	public void UpdatePositions()
	{
		vertsChanged = true;
	}

	// Informs the SpriteManager that some UVs have changed
	public void UpdateUVs()
	{
		uvsChanged = true;
	}

	// Informs the SpriteManager that some colors have changed
	public void UpdateColors()
	{
		colorsChanged = true;
	}

	/// <summary>
	/// Instructs the manager to recalculate the bounds of the mesh.
	/// </summary>
	public void UpdateBounds()
	{
		updateBounds = true;
	}

	/// <summary>
	/// Schedules a recalculation of the mesh bounds to occur at a
	/// regular interval (given in seconds).
	/// </summary>
	/// <param name="seconds">Time, in seconds, between bounds updates.</param>
	public void ScheduleBoundsUpdate(float seconds)
	{
		boundUpdateInterval = seconds;
		InvokeRepeating("UpdateBounds", seconds, seconds);
	}

	/// <summary>
	/// Cancels any previously scheduled bounds recalculations.
	/// </summary>
	public void CancelBoundsUpdate()
	{
		CancelInvoke("UpdateBounds");
	}

	// Returns whether the SpriteManager is done initializing.
	public bool IsInitialized
	{
		get { return initialized; }
	}

	// LateUpdate is called once per frame
	virtual public void LateUpdate()
	{
		// Were changes made to the mesh since last time?
		if (vertCountChanged)
		{
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;

			meshRenderer.bones = bones;
			
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.bindposes = bindPoses;
			mesh.boneWeights = boneWeights;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.triangles = triIndices;

#if SPRITE_WANT_NORMALS
			mesh.RecalculateNormals();
#endif
			if (autoUpdateBounds)
				mesh.RecalculateBounds();
		}
		else
		{
			if (vertsChanged)
			{
				vertsChanged = false;

				if (autoUpdateBounds)
					updateBounds = true;

				mesh.vertices = vertices;
			}

			if (updateBounds)
			{
				mesh.RecalculateBounds();
				updateBounds = false;
			}

			if (colorsChanged)
			{
				colorsChanged = false;

				mesh.colors = colors;
			}

			if (uvsChanged)
			{
				uvsChanged = false;
				mesh.uv = UVs;
			}
		}
	}



	// Empties the manager of all sprites.
	// This is used internally in edit mode
	// to keep the manager up-to-date despite
	// Unity's mucking with us.
/*
	protected void Clear()
	{
		SpriteRoot spr;

		if(activeBlocks.Rewind())
		{
			do
			{
				spr = activeBlocks.Current.sprite;
				RemoveSprite(activeBlocks.Current);
				spr.manager = this; // Don't forget us!
			}while(activeBlocks.MoveNext());
		}
	}

	// Scans the scene for sprites managed
	// by this manager.
	// This is used internally in edit mode
	// to keep the manager up-to-date despite
	// Unity's mucking with us.
	protected void ScanScene()
	{
		Object[] c = FindObjectsOfType(typeof(SpriteRoot));
		SpriteRoot spr;

		if(c == null)
			return;

		for(int i=0; i<c.Length; ++i)
		{
			spr = (SpriteRoot) c[i];

			if (spr.managed)
				if (spr.manager == this)
					AddSprite(spr);
		}
	}
*/

	// Ensures that the sprites are updated in the scene view
	// while editing:
	public virtual void DoMirror()
	{
		// Only run if we're not playing:
		if (Application.isPlaying)
			return;

		// Were changes made to the mesh since last time?
		if (vertCountChanged)
		{
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;

			meshRenderer.bones = bones;

			mesh.Clear();
			mesh.vertices = vertices;
			mesh.bindposes = bindPoses;
			mesh.boneWeights = boneWeights;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.triangles = triIndices;
		}
		else
		{
			if (vertsChanged)
			{
				vertsChanged = false;

				updateBounds = true;

				mesh.vertices = vertices;
			}

			if(updateBounds)
			{
				mesh.RecalculateBounds();
				updateBounds = false;
			}

			if (colorsChanged)
			{
				colorsChanged = false;

				mesh.colors = colors;
			}

			if (uvsChanged)
			{
				uvsChanged = false;
				mesh.uv = UVs;
			}
		}
	}

#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9) && UNITY_EDITOR
	void Update() 
	{
		DoMirror();
	}
#endif
	// Ensures that the text is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmos()
	{
#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9) && UNITY_EDITOR
		DoMirror();
#endif
		if (drawBoundingBox)
		{
			Gizmos.color = Color.yellow;
			DrawCenter();
			// Draw bounding box:
			Gizmos.DrawWireCube(meshRenderer.bounds.center, meshRenderer.bounds.size);
		}
	}


	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		DrawCenter();
		// Draw bounding box:
		Gizmos.DrawWireCube(meshRenderer.bounds.center, meshRenderer.bounds.size);
	}

	protected void DrawCenter()
	{
		float longestSide;
		float starSize;

		longestSide = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y);
		longestSide = Mathf.Max(longestSide, meshRenderer.bounds.size.z);

		starSize = longestSide * 0.015f;

		// Draw the center:
		Gizmos.DrawLine(meshRenderer.bounds.center - Vector3.up * starSize, meshRenderer.bounds.center + Vector3.up * starSize);
		Gizmos.DrawLine(meshRenderer.bounds.center - Vector3.right * starSize, meshRenderer.bounds.center + Vector3.right * starSize);
		Gizmos.DrawLine(meshRenderer.bounds.center - Vector3.forward * starSize, meshRenderer.bounds.center + Vector3.forward * starSize);
	}
}


#if USE_GENERICS
// Compares drawing layers of sprites
public class SpriteDrawLayerComparer : IComparer<SpriteMesh_Managed>
{
	public int Compare(SpriteMesh_Managed a, SpriteMesh_Managed b)
	{
		if (a.drawLayer > b.drawLayer)
			return 1;
		else if (a.drawLayer < b.drawLayer)
			return -1;
		else
			return 0;
	}
}

#else

// Compares drawing layers of sprites
public class SpriteDrawLayerComparer : IComparer
{
	static SpriteMesh_Managed s1;
	static SpriteMesh_Managed s2;

	int IComparer.Compare(object a, object b)
	{
		s1 = (SpriteMesh_Managed)a;
		s2 = (SpriteMesh_Managed)b;

		if (s1.drawLayer > s2.drawLayer)
			return 1;
		else if (s1.drawLayer < s2.drawLayer)
			return -1;
		else
			return 0;
	}
}

#endif