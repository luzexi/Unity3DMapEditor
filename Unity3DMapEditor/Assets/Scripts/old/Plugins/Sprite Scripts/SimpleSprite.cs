//-----------------------------------------------------------------
//	SimpleSprite v1.0 (3-3-2010)
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Defines a simple, non-packable, non-animating sprite.
/// </remarks>
[ExecuteInEditMode]
public class SimpleSprite : SpriteRoot
{
	/// <summary>
	/// Position of the lower-left pixel of the sprite when
	/// no animation has been played.
	/// </summary>
	public Vector2 lowerLeftPixel;				// Position of the lower-left pixel of the sprite

	/// <summary>
	/// Dimensions, in pixels, of the sprite when no animation
	/// has been played.
	/// </summary>
	public Vector2 pixelDimensions;				// Dimensions, in pixels, of the sprite

	// Will help us pick a useful camera at startup
	// if none is already set.
	protected bool nullCamera = false;


	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		return pixelDimensions;
	}


	protected override void Awake()
	{
		base.Awake();

		Init();
	}


	protected override void Init()
	{
		nullCamera = (renderCamera == null);
		base.Init();
	}


	public override void Start()
	{
		base.Start();

		if (UIManager.Exists())
		{
			// Choose the first UI Camera by default:
			if (nullCamera && UIManager.instance.uiCameras.Length > 0)
				SetCamera(UIManager.instance.uiCameras[0].camera);
		}
	}

	// Resets all sprite values to defaults for reuse:
	public override void Clear()
	{
		base.Clear();
	}

	/// <summary>
	/// Sets up the essential elements of a sprite.
	/// </summary>
	/// <param name="w">The width, in local space, of the sprite.</param>
	/// <param name="h">The height, in local space, of the sprite.</param>
	/// <param name="lowerleftPixel">The coordinate of the lower-left pixel of the desired sprite on the sprite atlas.</param>
	/// <param name="pixeldimensions">The X and Y dimensions, in pixels, of the sprite.</param>
	public void Setup(float w, float h, Vector2 lowerleftPixel, Vector2 pixeldimensions)
	{
		this.Setup(w, h, lowerleftPixel, pixeldimensions, m_spriteMesh.material);
	}

	/// <summary>
	/// Sets up the essential elements of a sprite.
	/// </summary>
	/// <param name="w">The width, in local space, of the sprite.</param>
	/// <param name="h">The height, in local space, of the sprite.</param>
	/// <param name="lowerleftPixel">The coordinate of the lower-left pixel of the desired sprite on the sprite atlas.</param>
	/// <param name="pixeldimensions">The X and Y dimensions, in pixels, of the sprite.</param>
	/// <param name="material">The material to use for the sprite.</param>
	public void Setup(float w, float h, Vector2 lowerleftPixel, Vector2 pixeldimensions, Material material)
	{
		width = w;
		height = h;
		lowerLeftPixel = lowerleftPixel;
		pixelDimensions = pixeldimensions;
		uvsInitialized = false;

		if (!managed)
			((SpriteMesh)m_spriteMesh).material = material;

		Init();
	}

	/// <summary>
	/// Copies all the attributes of another sprite.
	/// </summary>
	/// <param name="s">A reference to the sprite to be copied.</param>
	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);

		// Check the type:
		if (!(s is SimpleSprite))
			return;

		lowerLeftPixel = ((SimpleSprite)s).lowerLeftPixel;
		pixelDimensions = ((SimpleSprite)s).pixelDimensions;

		InitUVs();

		SetBleedCompensation(s.bleedCompensation);

		if (autoResize || pixelPerfect)
			CalcSize();
		else
			SetSize(s.width, s.height);
	}


	// Implements UV calculation
	public override void InitUVs()
	{
		tempUV = PixelCoordToUVCoord(lowerLeftPixel);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;

		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;

		frameInfo.uvs = uvRect;

		base.InitUVs();
	}


	//--------------------------------------------------------------
	// Misc:
	//--------------------------------------------------------------
	/// <summary>
	/// Sets the lower-left pixel of the sprite.
	/// See <see cref="lowerLeftPixel"/>
	/// </summary>
	/// <param name="lowerLeft">Pixel coordinate of the lower-left corner of the sprite.</param>
	public void SetLowerLeftPixel(Vector2 lowerLeft)
	{
		lowerLeftPixel = lowerLeft;

		// Calculate UV dimensions:
		tempUV = PixelCoordToUVCoord(lowerLeftPixel);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;

		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;

		frameInfo.uvs = uvRect;

		// Adjust for bleed compensation:
		SetBleedCompensation(bleedCompensation);

		// Now see if we need to resize:
		if (autoResize || pixelPerfect)
			CalcSize();
	}

	/// <summary>
	/// Sets the lower-left pixel of the sprite.
	/// See <see cref="lowerLeftPixel"/>
	/// </summary>
	/// <param name="x">X pixel coordinate.</param>
	/// <param name="y">Y pixel coordinate.</param>
	public void SetLowerLeftPixel(int x, int y)
	{
		SetLowerLeftPixel(new Vector2((float)x, (float)y));
	}

	/// <summary>
	/// Sets the pixel dimensions of the sprite.
	/// See <see cref="pixelDimensions"/>
	/// </summary>
	/// <param name="size">Dimensions of the sprite in pixels.</param>
	public void SetPixelDimensions(Vector2 size)
	{
		pixelDimensions = size;

		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;

		frameInfo.uvs = uvRect;

		// Adjust for bleed compensation
		// NOTE: We can't call SetBleedCompensation()
		// here because we've only changed the right-hand
		// side, so we have to calculate it ourselves:
/*
		uvRect.xMax -= bleedCompensationUV.x * 2f;
		uvRect.yMax -= bleedCompensationUV.y * 2f;
*/

		// Now see if we need to resize:
		if (autoResize || pixelPerfect)
			CalcSize();
	}

	/// <summary>
	/// Sets the pixel dimensions of the sprite.
	/// See <see cref="pixelDimensions"/>
	/// </summary>
	/// <param name="x">X size in pixels.</param>
	/// <param name="y">Y size in pixels.</param>
	public void SetPixelDimensions(int x, int y)
	{
		SetPixelDimensions(new Vector2((float)x, (float)y));
	}

	public override int GetStateIndex(string stateName)
	{
		return -1;
	}

	public override void SetState(int index) {}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public SimpleSprite Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (SimpleSprite)go.AddComponent(typeof(SimpleSprite));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public SimpleSprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (SimpleSprite)go.AddComponent(typeof(SimpleSprite));
	}


	// Ensures that the sprite is updated in the scene view
	// while editing:
	public override void DoMirror()
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
 			base.Start();

		if (mirror == null)
		{
			mirror = new SimpleSpriteMirror();
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
}


// Mirrors the editable settings of a sprite that affect
// how the sprite is drawn in the scene view
public class SimpleSpriteMirror : SpriteRootMirror
{
	public Vector2 lowerLeftPixel, pixelDimensions;

	// Mirrors the specified sprite's settings
	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);

		lowerLeftPixel = ((SimpleSprite)s).lowerLeftPixel;
		pixelDimensions = ((SimpleSprite)s).pixelDimensions;
	}

	// Returns true if any of the settings do not match:
	public override bool DidChange(SpriteRoot s)
	{
		if (base.DidChange(s))
			return true;
		if (((SimpleSprite)s).lowerLeftPixel != lowerLeftPixel)
		{
			s.uvsInitialized = false;
			return true;
		}
		if (((SimpleSprite)s).pixelDimensions != pixelDimensions)
		{
			s.uvsInitialized = false;
			return true;
		}

		return false;
	}
}