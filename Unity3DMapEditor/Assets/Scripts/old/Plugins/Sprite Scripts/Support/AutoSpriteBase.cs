//-----------------------------------------------------------------
//	AutoSpriteBase
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

// #define SPRITE_FRAME_DELEGATE		// Adds code to call a delegate for each frame of animation (comment out this line to improve performance)


using UnityEngine;
using System.Collections;




/// <remarks>
/// Defines an animation composed of individual
/// textures.  This is primarily used for atlas
/// compilation and is not used at run-time except
/// for initialization.
/// </remarks>
[System.Serializable]
public class TextureAnim
{
	/// <summary>
	/// Name of the animation.
	/// </summary>
	public string name;

	/// <summary>
	/// Loop cycles.
	/// See loopCycles member of <see cref="UVAnimation"/>.
	/// </summary>
	public int loopCycles = 0;

	/// <summary>
	/// See loopReverse member of <see cref="UVAnimation"/>.
	/// </summary>
	public bool loopReverse = false;

	/// <summary>
	/// The framerate at which the animation should play in frames per second.
	/// </summary>
	public float framerate = 15f;// The rate in frames per second at which to play the animation

	/// <summary>
	/// What the sprite should do when the animation is done playing.
	/// The options are to: 1) Do nothing, 2) return to the static image,
	/// 3) play the default animation.
	/// </summary>
	public UVAnimation.ANIM_END_ACTION onAnimEnd = UVAnimation.ANIM_END_ACTION.Do_Nothing;

	// Array of paths to textures each of which serve as a frame of animation.
	[HideInInspector]
	public string[] framePaths;
	// Array of GUIDs for textures each of which serve as a frame of animation.
	[HideInInspector]
	public string[] frameGUIDs;

	// Arrays that parallel the Texture2D frames but are populated 
	// with UV coordinates and other info upon atlas compilation which 
	// point to the UV locations of where the textures were placed on 
	// the atlas.
	[HideInInspector]
	public CSpriteFrame[] spriteFrames;		// Array that parallels the Texture2D frameGUIDs array and holds the post-packing atlas UV coords for the given frame

	// Allocates enough elements in each of the "frame info" fields
	// to hold info for each animation frame specified in "frames":
	public void Allocate()
	{
		bool allocate = false;

		if (framePaths == null)
			framePaths = new string[0];
		if (frameGUIDs == null)
			frameGUIDs = new string[0];

		if (spriteFrames == null)
			allocate = true;
		else if (spriteFrames.Length != frameGUIDs.Length)
			allocate = true;

		if(allocate)
		{
			spriteFrames = new CSpriteFrame[ Mathf.Max(frameGUIDs.Length, framePaths.Length) ];
			for (int i = 0; i < spriteFrames.Length; ++i)
				spriteFrames[i] = new CSpriteFrame();
		}
	}

	public TextureAnim()
	{
		Allocate();
	}

	public TextureAnim(string n)
	{
		name = n;
		Allocate();
	}

	// Member-wise copy:
	public void Copy(TextureAnim a)
	{
		name = a.name;
		loopCycles = a.loopCycles;
		loopReverse = a.loopReverse;
		framerate = a.framerate;
		onAnimEnd = a.onAnimEnd;

		framePaths = new string[a.framePaths.Length];
		a.framePaths.CopyTo(framePaths, 0);

		frameGUIDs = new string[a.frameGUIDs.Length];
		a.frameGUIDs.CopyTo(frameGUIDs, 0);

		spriteFrames = new CSpriteFrame[a.spriteFrames.Length];
		for(int i=0; i<spriteFrames.Length; ++i)
			spriteFrames[i] = new CSpriteFrame(a.spriteFrames[i]);
	}
}



/// <remarks>
/// Serves as a base for all "packable" sprite types.
/// That is, all sprites which are defined by one or
/// more source textures, which then get "packed" into
/// an atlas.
/// </remarks>
public abstract class AutoSpriteBase : SpriteBase, ISpriteAggregator, ISpritePackable
{
	//---------------------------------------------
	// Members related to building atlases:
	// (Used for ISpriteAggregator implementation, such
	// as Aggregate(), etc)
	//---------------------------------------------
	protected Texture2D[] sourceTextures;
	protected CSpriteFrame[] spriteFrames;



	/// <summary>
	/// When set to true, even if the "Trim Images" option is
	/// enabled in the atlas builder, the images for this object
	/// will not be trimmed.
	/// </summary>
	public bool doNotTrimImages = false;



	/// <summary>
	/// Holds the actual UV sequences that will be used at run-time.
	/// </summary>
	[HideInInspector]
	public UVAnimation[] animations;
	protected UVAnimation curAnim = null;			// The current animation

	

	// Used to allow us to write code that accesses
	// the sprite's states/animation lists while
	// allowing children to declare the actual
	// array that will hold them.
	/// <summary>
	/// Accessor for the sprite's various states as defined in the editor (not used at runtime).
	/// </summary>
	public abstract TextureAnim[] States
	{
		get;
		set;
	}


	/// <summary>
	/// Gets the default frame of the sprite object.
	/// This is the appearance the sprite is to have
	/// in the editor.
	/// </summary>
	public virtual CSpriteFrame DefaultFrame
	{
		get
		{
			if (States[0].spriteFrames.Length != 0)
				return States[0].spriteFrames[0];
			else
				return null;
		}
	}

	// Gets the default state (or animation)
	// of the sprite object.
	public virtual TextureAnim DefaultState
	{
		get
		{
			if (States != null)
				if(States.Length != 0)
					return States[0];

			return null;
		}
	}

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		TextureAnim a = DefaultState;
		CSpriteFrame f = DefaultFrame;

		if(a == null)
			return Vector2.zero;
		if(a.frameGUIDs == null)
			return Vector2.zero;
		if (a.frameGUIDs.Length == 0)
			return Vector2.zero;
		if(f == null)
		{
			Debug.LogWarning("Sprite \"" + name + "\" does not seem to have been built to an atlas yet.");
			return Vector2.zero;
		}

		Vector2 size = Vector2.zero;

		Texture2D tex = (Texture2D) loader(guid2Path(a.frameGUIDs[0]), typeof(Texture2D));

		if(tex == null)
		{
			if(spriteMesh != null)
			{
				tex = (Texture2D)spriteMesh.material.GetTexture("_MainTex");
				size = new Vector2(f.uvs.width * tex.width, f.uvs.height * tex.height);
			}
		}
		else
			size = new Vector2(tex.width * (1f / (f.scaleFactor.x * 2f)), tex.height * (1f / (f.scaleFactor.y * 2f)));

		return size;
	}
	
	protected override void Awake()
	{
		base.Awake();

		animations = new UVAnimation[States.Length];

		for (int i = 0; i < States.Length; ++i)
		{
			animations[i] = new UVAnimation();
			animations[i].SetAnim(States[i], i);
		}
	}


	// Resets all sprite values to defaults for reuse:
	public override void Clear()
	{
		base.Clear();

		if (curAnim != null)
		{
			PauseAnim();
			curAnim = null;
		}
	}


	/// <summary>
	/// Sets up the essential elements of a sprite.
	/// </summary>
	/// <param name="w">The width, in local space, of the sprite.</param>
	/// <param name="h">The height, in local space, of the sprite.</param>
	public void Setup(float w, float h)
	{
		this.Setup(w, h, m_spriteMesh.material);
	}

	/// <summary>
	/// Sets up the essential elements of a sprite.
	/// </summary>
	/// <param name="w">The width, in local space, of the sprite.</param>
	/// <param name="h">The height, in local space, of the sprite.</param>
	/// <param name="material">The material to use for the sprite.</param>
	public void Setup(float w, float h, Material material)
	{
		width = w;
		height = h;

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
		AutoSpriteBase sp;

		base.Copy(s);

		// Check the type:
		if (!(s is AutoSpriteBase))
			return;

		sp = (AutoSpriteBase)s;

		// See if it is an actual sprite instance:
		if(sp.spriteMesh != null)
		{
			if (sp.animations.Length > 0)
			{
				animations = new UVAnimation[sp.animations.Length];
				for (int i = 0; i < animations.Length; ++i)
					animations[i] = sp.animations[i].Clone();
			}
		}
		else if(States != null)
		{
			// Assume this is a prefab, so copy the UV info 
			//from its TextureAnimations instead:			
			animations = new UVAnimation[sp.States.Length];

			for (int i = 0; i < sp.States.Length; ++i)
			{
				animations[i] = new UVAnimation();
				animations[i].SetAnim(sp.States[i], i);
			}
		}
	}

	/// <summary>
	/// Copies all the attributes of another sprite,
	/// including its edit-time TextureAnimations.
	/// </summary>
	/// <param name="s">A reference to the sprite to be copied.</param>
	public virtual void CopyAll(SpriteRoot s)
	{
		AutoSpriteBase sp;

		base.Copy(s);

		// Check the type:
		if (!(s is AutoSpriteBase))
			return;

		sp = (AutoSpriteBase)s;

		States = new TextureAnim[sp.States.Length];

		for (int i = 0; i < States.Length; ++i)
		{
			States[i] = new TextureAnim();
			States[i].Copy(sp.States[i]);
		}

		animations = new UVAnimation[States.Length];

		for (int i = 0; i < States.Length; ++i)
		{
			animations[i] = new UVAnimation();
			animations[i].SetAnim(States[i], i);
		}

		doNotTrimImages = sp.doNotTrimImages;
	}


	// Steps to the next frame of sprite animation
	public override bool StepAnim(float time)
	{
		if (curAnim == null)
			return false;

		timeSinceLastFrame += Mathf.Max(0, time);
		//timeSinceLastFrame += time;

		framesToAdvance = (timeSinceLastFrame / timeBetweenAnimFrames);

		// If there's nothing to do, return:
		if (framesToAdvance < 1)
		{
			if (crossfadeFrames)
				SetColor(new Color(1f, 1f, 1f, (1f - framesToAdvance)));
			return true;
		}

		//timeSinceLastFrame -= timeBetweenAnimFrames * (float)framesToAdvance;

		while (framesToAdvance >= 1f)
		{
			if (curAnim.GetNextFrame(ref frameInfo))
			{
#if SPRITE_FRAME_DELEGATE
				// Notify the delegate:
				if(framesToAdvance >= 1f)
					if (animFrameDelegate != null)
						animFrameDelegate(this, curAnim.GetCurPosition());
#endif
				framesToAdvance -= 1f;
				timeSinceLastFrame -= timeBetweenAnimFrames;
			}
			else
			{
				// We reached the end of our animation
				if (crossfadeFrames)
					SetColor(Color.white);

				// See if we should revert to a static appearance,
				// default anim, or do nothing, etc:
				switch(curAnim.onAnimEnd)
				{
					case UVAnimation.ANIM_END_ACTION.Do_Nothing:
						PauseAnim();

						// Update mesh UVs:
						uvRect = frameInfo.uvs;
						SetBleedCompensation();

						// Resize if selected:
						if (autoResize || pixelPerfect)
							CalcSize();
						break;

					case UVAnimation.ANIM_END_ACTION.Revert_To_Static:
						RevertToStatic();
						break;

					case UVAnimation.ANIM_END_ACTION.Play_Default_Anim:
						// Notify the delegates:
/*
#if SPRITE_FRAME_DELEGATE
						if (animFrameDelegate != null)
							animFrameDelegate(this, curAnim.GetCurPosition());
#endif
*/

						if (animCompleteDelegate != null)
							animCompleteDelegate(this);

						// Play the default animation:
						PlayAnim(defaultAnim);
						return false;

					case UVAnimation.ANIM_END_ACTION.Hide:
						Hide(true);
						break;
					case UVAnimation.ANIM_END_ACTION.Deactivate:
						gameObject.active = false;
						break;
					case UVAnimation.ANIM_END_ACTION.Destroy:
						// Notify the delegates:
/*
#if SPRITE_FRAME_DELEGATE
						if (animFrameDelegate != null)
							animFrameDelegate(this, curAnim.GetCurPosition());
#endif
*/
						if (animCompleteDelegate != null)
							animCompleteDelegate(this);

						Delete();
						Destroy(gameObject);
						break;
				}

				// Notify the delegates:
/*
#if SPRITE_FRAME_DELEGATE
				if (animFrameDelegate != null)
					animFrameDelegate(this, curAnim.GetCurPosition());
#endif
*/
				if (animCompleteDelegate != null)
					animCompleteDelegate(this);

				// Check to see if we are still animating
				// before setting the curAnim to null.
				// Animating should be turned off if
				// PauseAnim() was called above, or if we
				// reverted to static.  But it could have
				// been turned on again by the 
				// animCompleteDelegate.
				if (!animating)
					curAnim = null;

				return false;
			}
		}

		// Cross-fade to the next frame:
		if (crossfadeFrames)
		{
			int curFrame = curAnim.GetCurPosition();
			int stepDir = curAnim.StepDirection;

			curAnim.GetNextFrame(ref nextFrameInfo);

			Vector2[] uvs2 = m_spriteMesh.uvs2;
			Rect nextUV = nextFrameInfo.uvs;
			uvs2[0].x = nextUV.xMin; uvs2[0].y = nextUV.yMax;
			uvs2[1].x = nextUV.xMin; uvs2[1].y = nextUV.yMin;
			uvs2[2].x = nextUV.xMax; uvs2[2].y = nextUV.yMin;
			uvs2[3].x = nextUV.xMax; uvs2[3].y = nextUV.yMax;

			// Undo our advance:
			curAnim.SetCurrentFrame(curFrame);
			curAnim.StepDirection = stepDir;

			SetColor(new Color(1f,1f,1f,(1f - framesToAdvance)));
		}
/*
#if SPRITE_FRAME_DELEGATE
		if (animFrameDelegate != null)
			animFrameDelegate(this, curAnim.GetCurPosition());
#endif
*/
		// Update mesh UVs:
		uvRect = frameInfo.uvs;
		SetBleedCompensation();

		// Resize if selected:
		if (autoResize || pixelPerfect)
			CalcSize();
		else if (anchor == SpriteRoot.ANCHOR_METHOD.TEXTURE_OFFSET)
			SetSize(width, height);

		//timeSinceLastFrame = 0;

		return true;
	}


	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="anim">A reference to the animation to play.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnim(UVAnimation anim, int frame)
	{
		if (deleted || !gameObject.active)
			return;

		if (!m_started)
			Start();

		curAnim = anim;
		curAnimIndex = curAnim.index;
		curAnim.Reset();
		curAnim.SetCurrentFrame(frame - 1);	// curFrame inside UVAnimation will be incremented before it is used, so anticipate this by decrementing by 1

		// Ensure the framerate is not 0 so we don't
		// divide by zero:
		if(anim.framerate != 0.0f)
		{
			timeBetweenAnimFrames = 1f / anim.framerate;
		}
		else
		{
			timeBetweenAnimFrames = 1f; // Just some dummy value since it won't be used
		}

		timeSinceLastFrame = timeBetweenAnimFrames;

		// Only add to the animated list if
		// the animation has more than 1 frame
		// or the framerate is non-zero:
		if ( (anim.GetFrameCount() > 1 || anim.onAnimEnd != UVAnimation.ANIM_END_ACTION.Do_Nothing) && anim.framerate != 0.0f)
		{
			StepAnim(0);
			// Start coroutine
			if (!animating)
			{
				//animating = true;
				AddToAnimatedList();
				//StartCoroutine(AnimationPump());
			}
		}
		else
		{
			// Make sure we are no longer in the animation pump:
			PauseAnim();

			// Since this is a single-frame anim,
			// call our delegate before setting
			// the frame so that our behavior is
			// consistent with multi-frame anims:
			if (animCompleteDelegate != null)
				animCompleteDelegate(this);

			StepAnim(0);
		}
	}

	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="anim">A reference to the animation to play.</param>
	public void PlayAnim(UVAnimation anim)
	{
		PlayAnim(anim, 0);
	}


	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="index">Index of the animation to play.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnim(int index, int frame)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}

		PlayAnim(animations[index], frame);
	}

	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="index">Index of the animation to play.</param>
	public override void PlayAnim(int index)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}

		PlayAnim(animations[index], 0);
	}

	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="name">The name of the animation to play.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnim(string name, int frame)
	{
		for (int i = 0; i < animations.Length; ++i)
		{
			if (animations[i].name == name)
			{
				PlayAnim(animations[i], frame);
				return;
			}
		}

		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="name">The name of the animation to play.</param>
	public override void PlayAnim(string name)
	{
		PlayAnim(name, 0);
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="anim">Reference to the animation to play in reverse.</param>
	public void PlayAnimInReverse(UVAnimation anim)
	{
		if (deleted || !gameObject.active)
			return;

		curAnim = anim;
		curAnim.Reset();
		curAnim.PlayInReverse();

		// Ensure the framerate is not 0 so we don't
		// divide by zero:
		if (anim.framerate != 0.0f)
		{
			timeBetweenAnimFrames = 1f / anim.framerate;
		}
		else
		{
			timeBetweenAnimFrames = 1f; // Just some dummy value since it won't be used
		}

		timeSinceLastFrame = timeBetweenAnimFrames;

		// Only add to the animated list if
		// the animation has more than 1 frame:
		if ((anim.GetFrameCount() > 1 || anim.onAnimEnd != UVAnimation.ANIM_END_ACTION.Do_Nothing) && anim.framerate != 0.0f)
		{
			StepAnim(0);
			// Start coroutine
			if (!animating)
			{
				//animating = true;
				AddToAnimatedList();
				//StartCoroutine(AnimationPump());
			}
		}
		else
		{
			// Make sure we are no longer in the animation pump:
			PauseAnim();

			// Since this is a single-frame anim,
			// call our delegate before setting
			// the frame so that our behavior is
			// consistent with multi-frame anims:
			if (animCompleteDelegate != null)
				animCompleteDelegate(this);

			StepAnim(0);
		}
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="anim">Reference to the animation to play in reverse.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnimInReverse(UVAnimation anim, int frame)
	{
		if (deleted || !gameObject.active)
			return;
		if (!m_started)
			Start();

		curAnim = anim;
		curAnim.Reset();
		curAnim.PlayInReverse();
		curAnim.SetCurrentFrame(frame + 1);	// curFrame inside UVAnimation will be decremented before it is used, so anticipate this by decrementing by 1

		// Ensure the framerate is not 0 so we don't
		// divide by zero:
		anim.framerate = Mathf.Max(0.0001f, anim.framerate);

		timeBetweenAnimFrames = 1f / anim.framerate;
		timeSinceLastFrame = timeBetweenAnimFrames;

		// Only add to the animated list if
		// the animation has more than 1 frame:
		if (anim.GetFrameCount() > 1)
		{
			StepAnim(0);
			// Start coroutine
			if (!animating)
			{
				//animating = true;
				AddToAnimatedList();
				//StartCoroutine(AnimationPump());
			}
		}
		else
		{
			// Since this is a single-frame anim,
			// call our delegate before setting
			// the frame so that our behavior is
			// consistent with multi-frame anims:
			if (animCompleteDelegate != null)
				animCompleteDelegate(this);

			StepAnim(0);
		}
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="index">Index of the animation to play in reverse.</param>
	public override void PlayAnimInReverse(int index)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}

		PlayAnimInReverse(animations[index]);
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="index">Index of the animation to play in reverse.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnimInReverse(int index, int frame)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}

		PlayAnimInReverse(animations[index], frame);
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="name">Name of the animation to play in reverse.</param>
	public override void PlayAnimInReverse(string name)
	{
		for (int i = 0; i < animations.Length; ++i)
		{
			if (animations[i].name == name)
			{
				animations[i].PlayInReverse();
				PlayAnimInReverse(animations[i]);
				return;
			}
		}

		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="name">Name of the animation to play in reverse.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnimInReverse(string name, int frame)
	{
		for (int i = 0; i < animations.Length; ++i)
		{
			if (animations[i].name == name)
			{
				animations[i].PlayInReverse();
				PlayAnimInReverse(animations[i], frame);
				return;
			}
		}

		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}


	/// <summary>
	/// Plays the specified animation only if it
	/// is not already playing.
	/// </summary>
	/// <param name="index">Index of the animation to play.</param>
	public void DoAnim(int index)
	{
		if (curAnim == null)
			PlayAnim(index);
		else if (curAnim.index != index || !animating)
			PlayAnim(index);
	}

	/// <summary>
 	/// Plays the specified animation only if it
	/// is not already playing.
	/// </summary>
	/// <param name="name">Name of the animation to play.</param>
	public void DoAnim(string name)
	{
		if (curAnim == null)
			PlayAnim(name);
		else if (curAnim.name != name || !animating)
			PlayAnim(name);
	}

	/// <summary>
	/// Plays the specified animation only if it
	/// is not already playing.
	/// </summary>
	/// <param name="anim">Reference to the animation to play.</param>
	public void DoAnim(UVAnimation anim)
	{
		if (curAnim != anim || !animating)
			PlayAnim(anim);
	}

	/// <summary>
	/// Sets the current frame of the current animation immediately.
	/// </summary>
	/// <param name="index">Zero-based index of the desired frame.</param>
	public void SetCurFrame(int index)
	{
		if (curAnim == null)
			return;
		if (!m_started)
			Start();

		curAnim.SetCurrentFrame(index - curAnim.StepDirection);
		timeSinceLastFrame = timeBetweenAnimFrames;
		StepAnim(0);
	}

	/// <summary>
	/// Sets the sprite to display the given frame of the
	/// specified animation.
	/// </summary>
	/// <param name="anim">The animation containing the desired frame.</param>
	/// <param name="frameNum">The 0-based index of the frame to be displayed.</param>
	public void SetFrame(UVAnimation anim, int frameNum)
	{
		PlayAnim(anim);
		if (IsAnimating())
			PauseAnim();
		SetCurFrame(frameNum);
	}

	/// <summary>
	/// Sets the sprite to display the given frame of the
	/// specified animation.
	/// </summary>
	/// <param name="anim">The animation containing the desired frame.</param>
	/// <param name="frameNum">The 0-based index of the frame to be displayed.</param>
	public void SetFrame(string anim, int frameNum)
	{
		PlayAnim(anim);
		if (IsAnimating())
			PauseAnim();
		SetCurFrame(frameNum);
	}

	/// <summary>
	/// Sets the sprite to display the given frame of the
	/// specified animation.
	/// </summary>
	/// <param name="anim">The 0-based index of the animation containing the desired frame.</param>
	/// <param name="frameNum">The 0-based index of the frame to be displayed.</param>
	public void SetFrame(int anim, int frameNum)
	{
		PlayAnim(anim);
		if (IsAnimating())
			PauseAnim();
		SetCurFrame(frameNum);
	}

	/// <summary>
	/// Stops the current animation from playing
	/// and resets it to the beginning for playing
	/// again.  The sprite then reverts to the static
	/// image.
	/// </summary>
	public override void StopAnim()
	{
		// Stop coroutine
		//animating = false;
		RemoveFromAnimatedList();

		//StopAllCoroutines();

		// Reset the animation:
		if (curAnim != null)
			curAnim.Reset();

		// Revert to our static appearance:
		RevertToStatic();
	}

	/// <summary>
	/// Resumes an animation from where it left off previously.
	/// </summary>
	public void UnpauseAnim()
	{
		if (curAnim == null) return;

		// Resume coroutine
		//animating = true;
		AddToAnimatedList();
		//StartCoroutine(AnimationPump());
	}

	// Adds the sprite to the list of currently
	// animating sprites:
	protected override void AddToAnimatedList()
	{
		// Check to see if the coroutine is running,
		// and if not, start it:
		// 		if (!SpriteAnimationPump.pumpIsRunning)
		// 			SpriteAnimationPump.Instance.StartAnimationPump();

		// If we're already animating, then we're
		// already in the list, no need to add again.
		// Also, if we're inactive, also don't add:
		if (animating || !Application.isPlaying || !gameObject.active)
			return;

		animating = true;
		SpriteAnimationPump.Add(this);
	}

	// Removes the sprite from the list of currently
	// animating sprites:
	protected override void RemoveFromAnimatedList()
	{
		SpriteAnimationPump.Remove(this);
		animating = false;
	}



	//--------------------------------------------------------------
	// Accessors:
	//--------------------------------------------------------------
	/// <summary>
	/// Returns a reference to the currently selected animation.
	/// NOTE: This does not mean the animation is currently playing.
	/// To determine whether the animation is playing, use <see cref="IsAnimating"/>
	/// </summary>
	/// <returns>Reference to the currently selected animation.</returns>
	public UVAnimation GetCurAnim() { return curAnim; }

	/// <summary>
	/// Returns a reference to the animation that matches the
	/// name specified.
	/// </summary>
	/// <param name="name">Name of the animation sought.</param>
	/// <returns>Reference to the animation, if found, null otherwise.</returns>
	public UVAnimation GetAnim(string name)
	{
		for (int i = 0; i < animations.Length; ++i)
		{
			if (animations[i].name == name)
			{
				return animations[i];
			}
		}

		return null;
	}

	public override int GetStateIndex(string stateName)
	{
		for (int i = 0; i < animations.Length; ++i)
		{
			if (string.Equals(animations[i].name, stateName, System.StringComparison.CurrentCultureIgnoreCase))
			{
				return i;
			}
		}

		return -1;
	}

	public override void SetState(int index)
	{
		PlayAnim(index);
	}

	public virtual bool SupportsArbitraryAnimations
	{
		get { return false; }
	}

	public virtual Material GetPackedMaterial(out string errString)
	{
		errString = "Sprite \"" + name + "\" has not been assigned a material, and can therefore not be included in the atlas build.";

		if (spriteMesh == null)
		{
			// See if it is because the sprite isn't associated
			// with a manager:
			if (managed)
			{
				// See if we can get the material
				// from an associated manager:
				if (manager != null)
				{
					return manager.renderer.sharedMaterial;
				}
				else // Else, no manager associated!:
				{
					errString = "Sprite \"" + name + "\" is not associated with a SpriteManager, and can therefore not be included in the atlas build.";
					return null;
				}
			}
			else // Else get the material from the sprite's renderer
			{	 // as this is probably a prefab and that's why it
				// doesn't have a mesh:
				return renderer.sharedMaterial;
			}
		}
		else if (managed)
		{
			if (manager != null)
			{
				return manager.renderer.sharedMaterial;
			}
			else // Else, no manager associated!:
			{
				errString = "Sprite \"" + name + "\" is not associated with a SpriteManager, and can therefore not be included in the atlas build.";
				return null;
			}
		}
		else
			return spriteMesh.material;
	}

	/// <summary>
	/// When set to true, even if the "Trim Images" option is
	/// enabled in the atlas builder, the images for this object
	/// will not be trimmed.
	/// </summary>
	public virtual bool DoNotTrimImages
	{
		get { return doNotTrimImages; }
		set { doNotTrimImages = value; }
	}


	// Collects all textures intended for packing,
	// as well as sprite frames, together into a 
	// standard form for processing.
	public virtual void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid)
	{
		ArrayList texList = new ArrayList();
		ArrayList frameList = new ArrayList();

		for (int i = 0; i < States.Length; ++i)
		{
			States[i].Allocate();

			// First try GUIDs:
			if(States[i].frameGUIDs.Length >= States[i].framePaths.Length)
			{
				for (int j = 0; j < States[i].frameGUIDs.Length; ++j)
				{
					string path = guid2Path(States[i].frameGUIDs[j]);
					texList.Add(load(path, typeof(Texture2D)));
					frameList.Add(States[i].spriteFrames[j]);
				}

				// Make sure we always use GUIDs in the future:
				States[i].framePaths = new string[0];
			}
			else
			{
				States[i].frameGUIDs = new string[States[i].framePaths.Length];

				States[i].spriteFrames = new CSpriteFrame[States[i].framePaths.Length];
				for (int j = 0; j < States[i].spriteFrames.Length; ++j)
					States[i].spriteFrames[j] = new CSpriteFrame();

				for (int j = 0; j < States[i].framePaths.Length; ++j)
				{
					// First get a GUID and save it:
					States[i].frameGUIDs[j] = path2Guid(States[i].framePaths[j]);

					texList.Add(load(States[i].framePaths[j], typeof(Texture2D)));
					frameList.Add(States[i].spriteFrames[j]);
				}
			}
		}

		sourceTextures = (Texture2D[])texList.ToArray(typeof(Texture2D));
		spriteFrames = (CSpriteFrame[])frameList.ToArray(typeof(CSpriteFrame));
	}

	// Provides access to the array of source textures.
	// NOTE: Should be ordered to parallel the
	// SpriteFrames array.
	public Texture2D[] SourceTextures
	{
		get { return sourceTextures; }
	}

	// Provides access to the array of Sprite Frames.
	// NOTE: Should be ordered to parallel the
	// SourceTextures array.
	public CSpriteFrame[] SpriteFrames
	{
		get { return spriteFrames; }
	}
}