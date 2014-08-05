//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <remarks>
/// This implements SpriteBase and adds animation functionality
/// allowing the user to define their sprite animations using
/// individual, non-uniform sprite textures which will then be
/// compiled into a sprite atlas automatically.
/// </remarks>
public class PackedSprite : AutoSpriteBase
{
    /// <summary>
	/// Texture to use for the sprite before animating.
	/// </summary>
	[HideInInspector]
	public string staticTexPath="";
	[HideInInspector]
	public string staticTexGUID="";

	// This will hold the UV coordinates calculated
	// by the atlas builder for our static texture:
	[HideInInspector]
	public CSpriteFrame _ser_stat_frame_info = new CSpriteFrame();
	[HideInInspector]
	public SPRITE_FRAME staticFrameInfo;


	/// <summary>
	/// The animations as defined using individual textures.
	/// See <see cref="TextureAnim"/>
	/// </summary>
	public TextureAnim[] textureAnimations;

	public override TextureAnim[] States
	{
		get 
		{
			if (textureAnimations == null)
				textureAnimations = new TextureAnim[0];
			return textureAnimations; 
		}
		set { textureAnimations = value; }
	}

	public override CSpriteFrame DefaultFrame
	{
		get { return _ser_stat_frame_info; }
	}

	public override TextureAnim DefaultState
	{
		get
		{
			if (textureAnimations != null)
				if (textureAnimations.Length != 0)
					if(defaultAnim < textureAnimations.Length)
						return textureAnimations[defaultAnim];

			return null;
		}
	}

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		if (staticTexGUID == "")
			return Vector2.zero;

		Texture2D tex = (Texture2D)loader(guid2Path(staticTexGUID), typeof(Texture2D));
		Vector2 size = new Vector2(tex.width * (1f / (_ser_stat_frame_info.scaleFactor.x * 2f)), tex.height * (1f / (_ser_stat_frame_info.scaleFactor.y * 2f)));

		return size;
	}

	public override bool SupportsArbitraryAnimations
	{
		get { return true; }
	}


	protected override void Awake()
	{
		if (textureAnimations == null)
			textureAnimations = new TextureAnim[0];

		staticFrameInfo = _ser_stat_frame_info.ToStruct();

		base.Awake();

		Init();
	}


	public override void Start()
	{
		if (m_started)
			return;
		base.Start();

		// See if we should play a default animation:
		if (playAnimOnStart && defaultAnim < animations.Length)
			if (Application.isPlaying)
				PlayAnim(defaultAnim);
	}


	protected override void Init()
	{
		base.Init();
	}



	/// <summary>
	/// Copies all the attributes of another sprite.
	/// </summary>
	/// <param name="s">A reference to the sprite to be copied.</param>
	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);

		PackedSprite sp;

		// Check the type:
		if (!(s is PackedSprite))
			return;

		sp = (PackedSprite)s;

		if (!sp.m_started)
			staticFrameInfo = sp._ser_stat_frame_info.ToStruct();
		else
			staticFrameInfo = sp.staticFrameInfo;

		if (curAnim != null)
		{
			if (curAnim.index == -1)
				PlayAnim(curAnim);
			else
				SetState(curAnim.index);
		}
		else
		{
			frameInfo = staticFrameInfo;
			uvRect = frameInfo.uvs;

			if (autoResize || pixelPerfect)
				CalcSize();
			else
				SetSize(s.width, s.height);
		}

		SetBleedCompensation();
	}


	// Implements the functionality of acquiring our "static" UV coordinates:
	public override void InitUVs()
	{
		frameInfo = staticFrameInfo;
		uvRect = staticFrameInfo.uvs;
	}

	//-----------------------------------------------------------------
	// Animation-related routines:
	//-----------------------------------------------------------------


	/// <summary>
	/// Adds an animation to the sprite for its use.
	/// </summary>
	/// <param name="anim">The animation to add</param>
	public void AddAnimation(UVAnimation anim)
	{
		UVAnimation[] temp;
		temp = animations;

		animations = new UVAnimation[temp.Length + 1];
		temp.CopyTo(animations, 0);

		animations[animations.Length - 1] = anim;
	}


	/// <summary>
	/// Collects all textures intended for packing,
	/// as well as sprite frames, together into a 
	/// standard form for processing.
	/// </summary>
	public override void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid)
	{
		List<Texture2D> texList = new List<Texture2D>();
		List<CSpriteFrame> frameList = new List<CSpriteFrame>();

		for (int i = 0; i < textureAnimations.Length; ++i)
		{
			textureAnimations[i].Allocate();

			// First try GUIDs:
			if (textureAnimations[i].frameGUIDs.Length >= textureAnimations[i].framePaths.Length)
			{
				for (int j = 0; j < textureAnimations[i].frameGUIDs.Length; ++j)
				{
					string path = guid2Path(textureAnimations[i].frameGUIDs[j]);
					texList.Add((Texture2D)load(path, typeof(Texture2D)));
					frameList.Add(textureAnimations[i].spriteFrames[j]);
				}

				// Make sure we always use GUIDs in the future:
				textureAnimations[i].framePaths = new string[0];
			}
			else
			{
				textureAnimations[i].frameGUIDs = new string[textureAnimations[i].framePaths.Length];

				textureAnimations[i].spriteFrames = new CSpriteFrame[textureAnimations[i].framePaths.Length];
				for (int j = 0; j < textureAnimations[i].spriteFrames.Length; ++j)
					textureAnimations[i].spriteFrames[j] = new CSpriteFrame();

				for (int j = 0; j < textureAnimations[i].framePaths.Length; ++j)
				{
					if (textureAnimations[i].framePaths[j].Length < 1)
						continue;

					// First get a GUID and save it:
					textureAnimations[i].frameGUIDs[j] = path2Guid(textureAnimations[i].framePaths[j]);

					texList.Add((Texture2D)load(textureAnimations[i].framePaths[j], typeof(Texture2D)));
					frameList.Add(textureAnimations[i].spriteFrames[j]);
				}
			}
		}
		
		// Get the static frame info:
		
		// First try GUID:
		if(staticTexGUID.Length > 1)
		{
			staticTexPath = guid2Path(staticTexGUID);
		}
		else // Else, populate the GUID:
		{
			staticTexGUID = path2Guid(staticTexPath);
		}

		texList.Add((Texture2D)load(staticTexPath, typeof(Texture2D)));
		frameList.Add(_ser_stat_frame_info);

		sourceTextures = texList.ToArray();
		spriteFrames = frameList.ToArray();
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public PackedSprite Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (PackedSprite)go.AddComponent(typeof(PackedSprite));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public PackedSprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (PackedSprite)go.AddComponent(typeof(PackedSprite));
	}
}
