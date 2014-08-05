//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

// #define SPRITE_FRAME_DELEGATE		// Adds code to call a delegate for each frame of animation (comment out this line to improve performance)


using UnityEngine;
using System.Collections;


/// <remarks>
/// Implements SpriteBase and adds certain animation functionality
/// specific to this type of sprite.
/// </remarks>
[ExecuteInEditMode]
public class Sprite : SpriteBase
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


	// Animation-related vars and types:

	/// <summary>
	/// Array of available animation sequences.
	/// This is typically built in-editor.
	/// </summary>
	public UVAnimation_Multi[] animations;				// Array of available animations
	protected UVAnimation_Multi curAnim = null;			// The current animation


	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		return pixelDimensions;
	}


	protected override void Awake()
	{
		base.Awake();

		Init();

		if (animations == null)
			animations = new UVAnimation_Multi[0];

		for (int i = 0; i < animations.Length; ++i)
		{
			animations[i].index = i;
			animations[i].BuildUVAnim(this);
		}
	}


	protected override void Init()
	{
		base.Init();
	}


	public override void Start()
	{
		if (m_started)
			return;
		base.Start();

		// See if we should play a default animation:
		if (playAnimOnStart && defaultAnim < animations.Length)
			if(Application.isPlaying)
				PlayAnim(defaultAnim);
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

		if(!managed)
			((SpriteMesh)m_spriteMesh).material = material;

		if (uvsInitialized)
		{
			Init();
			InitUVs();
			SetBleedCompensation();
		}
		else
			Init();
	}

	/// <summary>
	/// Copies all the attributes of another sprite.
	/// </summary>
	/// <param name="s">A reference to the sprite to be copied.</param>
	public override void Copy(SpriteRoot s)
	{
		Sprite sp;

		base.Copy(s);

		// Check the type:
		if (!(s is Sprite))
			return;

		sp = (Sprite)s;

		lowerLeftPixel = sp.lowerLeftPixel;
		pixelDimensions = sp.pixelDimensions;

		InitUVs();

		SetBleedCompensation(s.bleedCompensation);

		if (autoResize || pixelPerfect)
			CalcSize();
		else
			SetSize(s.width, s.height);

		if (sp.animations.Length > 0)
		{
			animations = new UVAnimation_Multi[sp.animations.Length];
			for (int i = 0; i < animations.Length; ++i)
				animations[i] = sp.animations[i].Clone();
		}

		for (int i = 0; i < animations.Length; ++i)
			animations[i].BuildUVAnim(this);
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
	}


	//-----------------------------------------------------------------
	// Animation-related routines:
	//-----------------------------------------------------------------


	/// <summary>
	/// Adds an animation to the sprite for its use.
	/// </summary>
	/// <param name="anim">The animation to add</param>
	public void AddAnimation(UVAnimation_Multi anim)
	{
		UVAnimation_Multi[] temp;
		temp = animations;

		animations = new UVAnimation_Multi[temp.Length + 1];
		temp.CopyTo(animations, 0);

		anim.index = animations.Length - 1;
		animations[anim.index] = anim;
	}

	/*
		// Steps to the next frame of sprite animation
		public bool StepAnim(float time)
		{
			if (curAnim == null)
				return false;

			timeSinceLastFrame += time;

			framesToAdvance = (int)(timeSinceLastFrame / timeBetweenAnimFrames);

			// If there's nothing to do, return:
			if (framesToAdvance < 1)
				return true;

			while (framesToAdvance > 0)
			{
				if (curAnim.GetNextFrame(ref lowerLeftUV))
					--framesToAdvance;
				else
				{
					// We reached the end of our animation
					if (animCompleteDelegate != null)
						animCompleteDelegate();

					// Update mesh UVs:
					UpdateUVs();
					PauseAnim();
					animating = false;
	 
					return false;
				}
			}

			// Update mesh UVs:
			UpdateUVs();

			timeSinceLastFrame = 0;

			return true;
		}
	*/

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
				switch (curAnim.onAnimEnd)
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
						curAnim = null;
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
#if SM2_FRAME_DELEGATE
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
		if(crossfadeFrames)
		{
			UVAnimation curClip = curAnim.GetCurrentClip();
			int curClipNum = curAnim.GetCurClipNum();
			int curFrame = curClip.GetCurPosition();
			int multiStepDir = curAnim.StepDirection;
			int clipStepDir = curClip.StepDirection;

			curAnim.GetNextFrame(ref nextFrameInfo);

			Vector2[] uvs2 = m_spriteMesh.uvs2;
			Rect nextUV = nextFrameInfo.uvs;
			uvs2[0].x = nextUV.xMin; uvs2[0].y = nextUV.yMax;
			uvs2[1].x = nextUV.xMin; uvs2[1].y = nextUV.yMin;
			uvs2[2].x = nextUV.xMax; uvs2[2].y = nextUV.yMin;
			uvs2[3].x = nextUV.xMax; uvs2[3].y = nextUV.yMax;

			// Undo our advance:
			curAnim.SetCurClipNum(curClipNum);
			curClip.SetCurrentFrame(curFrame);
			curAnim.StepDirection = multiStepDir;
			curClip.StepDirection = clipStepDir;

			SetColor(new Color(1f, 1f, 1f, (1f - framesToAdvance)));
		}
/*
#if SM2_FRAME_DELEGATE
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

		//timeSinceLastFrame = 0;

		return true;
	}

/*
	// Steps to the next frame of sprite animation
	public override bool StepAnim()
	{
		if (curAnim == null)
			return false;

		if (!curAnim.GetNextFrame(ref uvRect))
		{
			// We reached the end of our animation

			// See if we should revert to a static appearance,
			// default anim, or do nothing:
			if (curAnim.onAnimEnd == UVAnimation.ANIM_END_ACTION.Do_Nothing)
			{
				PauseAnim();

				// Update mesh UVs:
				SetBleedCompensation();

				// Resize if selected:
				if (autoResize || pixelPerfect)
					CalcSize();
			}
			else if (curAnim.onAnimEnd == UVAnimation.ANIM_END_ACTION.Revert_To_Static)
				RevertToStatic();
			else if (curAnim.onAnimEnd == UVAnimation.ANIM_END_ACTION.Play_Default_Anim)
			{
				// Notify the delegate:
				if (animCompleteDelegate != null)
					animCompleteDelegate();

				// Play the default animation:
				PlayAnim(defaultAnim);

				return false;
			}

			// Notify the delegate:
			if (animCompleteDelegate != null)
				animCompleteDelegate();

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

		// Update mesh UVs:
		SetBleedCompensation();

		UpdateUVs();

		// Resize if selected:
		if (autoResize || pixelPerfect)
			CalcSize();

		return true;
	}*/


	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="anim">A reference to the animation to play.</param>
	public void PlayAnim(UVAnimation_Multi anim)
	{
		if (deleted || !gameObject.active)
			return;
		if (!m_started)
			Start();

		curAnim = anim;
		curAnimIndex = curAnim.index;
		curAnim.Reset();

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
		// the animation has more than 1 frame
		// or the framerate is non-zero:
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

		PlayAnim(animations[index]);
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
		for (int i = 0; i < animations.Length; ++i)
		{
			if (animations[i].name == name)
			{
				PlayAnim(animations[i]);
				return;
			}
		}

		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="anim">Reference to the animation to play in reverse.</param>
	public void PlayAnimInReverse(UVAnimation_Multi anim)
	{
		if (deleted || !gameObject.active)
			return;
		if (!m_started)
			Start();

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
	/// Plays the specified animation only if it
	/// is not already playing.
	/// </summary>
	/// <param name="index">Index of the animation to play.</param>
	public void DoAnim(int index)
	{
		if (curAnim == null)
			PlayAnim(index);
		else if (curAnim != animations[index] || !animating)
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
	public void DoAnim(UVAnimation_Multi anim)
	{
		if (curAnim != anim || !animating)
			PlayAnim(anim);
	}

	/// <summary>
	/// Stops the current animation from playing
	/// and resets it to the beginning for playing
	/// again.  The sprite then reverts to the static
	/// (non-animating default) image.
	/// </summary>
	public override void StopAnim()
	{
		// Stop coroutine
		//animating = false;
		RemoveFromAnimatedList();

		//StopAllCoroutines();

		// Reset the animation:
		if(curAnim != null)
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
	public UVAnimation_Multi GetCurAnim() { return curAnim; }

	/// <summary>
	/// Returns a reference to the animation that matches the
	/// name specified.
	/// </summary>
	/// <param name="name">Name of the animation sought.</param>
	/// <returns>Reference to the animation, if found, null otherwise.</returns>
	public UVAnimation_Multi GetAnim(string name)
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

		// Adjust for bleed compensation
		// NOTE: We can't call SetBleedCompensation()
		// here because we've only changed the right-hand
		// side, so we have to calculate it ourselves:
		uvRect.xMax -= bleedCompensationUV.x * 2f;
		uvRect.yMax -= bleedCompensationUV.y * 2f;

		frameInfo.uvs = uvRect;

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
			mirror = new SpriteMirror();
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


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public Sprite Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (Sprite)go.AddComponent(typeof(Sprite));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public Sprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (Sprite)go.AddComponent(typeof(Sprite));
	}
}


// Mirrors the editable settings of a sprite that affect
// how the sprite is drawn in the scene view
public class SpriteMirror : SpriteRootMirror
{
	public Vector2 lowerLeftPixel, pixelDimensions;

	// Mirrors the specified sprite's settings
	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);

		lowerLeftPixel = ((Sprite)s).lowerLeftPixel;
		pixelDimensions = ((Sprite)s).pixelDimensions;
	}

	// Returns true if any of the settings do not match:
	public override bool DidChange(SpriteRoot s)
	{
		if (base.DidChange(s))
			return true;
		if (((Sprite)s).lowerLeftPixel != lowerLeftPixel)
		{
			s.uvsInitialized = false;
			return true;
		}
		if (((Sprite)s).pixelDimensions != pixelDimensions)
		{
			s.uvsInitialized = false;
			return true;
		}

		return false;
	}
}