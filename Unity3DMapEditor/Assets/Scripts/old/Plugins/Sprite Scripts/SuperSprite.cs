//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------



using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// An element in a SuperSpriteAnim's
/// array of sprite animatinos.
/// </remarks>
[System.Serializable]
public class SuperSpriteAnimElement
{
	/// <summary>
	/// The reference to the sprite containing
	/// the animation to be played.
	/// </summary>
	public AutoSpriteBase sprite;

	/// <summary>
	/// The name of the animation to play.
	/// </summary>
	public string animName;

	// The actual reference to the desired
	// animation.
	[HideInInspector]
	public UVAnimation anim;

	public void Init()
	{
		bool wasDeactivated = false;

		if (sprite != null)
		{
			// See if we need to activate our sprite
			// to work with it:
			if (!sprite.gameObject.active)
			{
				wasDeactivated = true;
				sprite.gameObject.active = true;
			}

			anim = sprite.GetAnim(animName);

			if (anim == null)
				Debug.LogError("SuperSprite error: No animation by the name of \"" + animName + "\" was found on sprite \"" + sprite.name + "\". Please verify the spelling and capitalization of the name, including any extra spaces, etc.");

			if (wasDeactivated)
				sprite.gameObject.active = false;
		}
	}

	/// <summary>
	/// Convenience method that plays the
	/// associated sprite's animation.
	/// </summary>
	public void Play()
	{
		sprite.PlayAnim(anim);
	}

	/// <summary>
	/// Convenience method that plays the
	/// associated sprite's animation in
	/// reverse:
	/// </summary>
	public void PlayInReverse()
	{
		sprite.PlayAnimInReverse(anim);
	}
}


/// <remarks>
/// Defines animations to be used with the
/// SuperSprite class.  Each animation
/// consists of references to one or more
/// AutoSpriteBase objects and the name of
/// one of its animations, which will be played
/// sequentially.
/// </remarks>
[System.Serializable]
public class SuperSpriteAnim
{
	/// <remarks>
	/// The action to take when an animation ends.
	/// </remarks>
	public enum ANIM_END_ACTION
	{
		/// <summary>
		/// Do nothing when the animation ends.
		/// </summary>
		Do_Nothing,

		/// <summary>
		/// Play the default animation when the animation ends.
		/// </summary>
		Play_Default_Anim,

		/// <summary>
		/// Deactivate the SuperSprite when the animation ends 
		/// (sets the GameObject's .active property to false).
		/// </summary>
		Deactivate,

		/// <summary>
		/// Destroys the SuperSprite when the animation ends.
		/// </summary>
		Destroy
	};

	public delegate void AnimCompletedDelegate(SuperSpriteAnim anim);

	/// <summary>
	/// Index of this animation 
	/// in the SuperSprite's list
	/// </summary>
	[HideInInspector]
	public int index;

	/// <summary>
	/// Name of the animation.
	/// </summary>
	public string name;

	/// <summary>
	/// The actual sprite animations to use.
	/// </summary>
	public SuperSpriteAnimElement[] spriteAnims = new SuperSpriteAnimElement[0];

	/// <summary>
	/// How many times to loop the animation, IN ADDITION
	/// to the initial play-through.
	/// 0 indicates to place the animation once then stop.  
	/// 1 indicates to repeat the animation once before 
	/// stopping, and so on.
	/// </summary>
	public int loopCycles = 0;

	/// <summary>
	/// Reverse the play direction when the end of the 
	/// animation is reached? (Ping-pong)
	/// If true, a loop iteration isn't counted until 
	/// the animation returns to the beginning.
	/// NOTE: This automatically plays the constituent
	/// animations in reverse when going back through
	/// the animation list, so keep that in mind when
	/// creating your individual sprite animations.
	/// </summary>
	public bool pingPong = false;

	/// <summary>
	/// What the SuperSprite should do when the animation is done playing.
	/// </summary>
	public ANIM_END_ACTION onAnimEnd = ANIM_END_ACTION.Do_Nothing;

	/// <summary>
	/// When set to true, non-playing sprites' GameObjects will be 
	/// deactivated and not merely hidden.
	/// </summary>
	public bool deactivateNonPlaying = false;

	/// <summary>
	/// When set to true, non-playing sprites' GameObjecs will be
	/// deactivated recursively.
	/// NOTE: Only has effect if deactivateNonPlaying is set to true.
	/// </summary>
	public bool deactivateRecursively = false;

	protected int curAnim = 0;		// The index of the current anim being played.
	protected int stepDir = 1;		// The direction we're moving through our list.
	protected int numLoops = 0;		// The number of loop iterations that have passed
	protected bool isRunning;		// Tells whether we're running or not.
	AnimCompletedDelegate endDelegate;


	public void Init(int idx, AnimCompletedDelegate del, SpriteBase.AnimFrameDelegate frameDel)
	{
		endDelegate = del;
		index = idx;

		List<SuperSpriteAnimElement> anims = new List<SuperSpriteAnimElement>();

		for (int i = 0; i < spriteAnims.Length; ++i)
			if (spriteAnims[i] != null)
				if (spriteAnims[i].sprite != null)
				{
					spriteAnims[i].Init();

					// Only save the ones that are valid
					// so we don't have to check them
					// constantly later on:
					anims.Add(spriteAnims[i]);

					if(frameDel != null)
						spriteAnims[i].sprite.SetAnimFrameDelegate(frameDel);

					// Hide each sprite by default:
					HideSprite(spriteAnims[i].sprite, true);
				}

		spriteAnims = anims.ToArray();
	}

	public void SetAnimFrameDelegate(SpriteBase.AnimFrameDelegate frameDel)
	{
		for (int i = 0; i < spriteAnims.Length; ++i)
			if (spriteAnims[i] != null)
				if (spriteAnims[i].sprite != null)
					spriteAnims[i].sprite.SetAnimFrameDelegate(frameDel);
	}

	// Delegate that is called when an animation finishes:
	void AnimFinished(SpriteBase sp)
	{
		// See if we can't advance to the next animation:
		if ((curAnim + stepDir) >= spriteAnims.Length || (curAnim + stepDir) < 0)
		{
			// See if we need to loop (if we're reversing, we don't loop until we get back to the beginning):
			if (stepDir > 0 && pingPong)
			{
				stepDir = -1;	// Reverse direction of playback

				// Bounce back from the end:
				((AutoSpriteBase)sp).PlayAnimInReverse(spriteAnims[curAnim].anim, spriteAnims[curAnim].anim.GetFrameCount() - 2);
				return;

				// See if we need to tell our first animation
				// to loop us back again in anticipation of
				// another loop iteration:
				// 				if(loopCycles == -1 || numLoops < loopCycles)
				// 				{
				// 					spriteAnims[0].anim.loopReverse = true;
				// 				}

				// Proceed
			}
			else
			{
				// See if we can't loop:
				if (numLoops + 1 > loopCycles && loopCycles != -1)
				{
					isRunning = false;

					// Unset our delegate:
					sp.SetAnimCompleteDelegate(null);

					// Notify that we're ending:
					if (endDelegate != null)
						endDelegate(this);
					return;
				}
				else
				{
					// Loop the animation:
					++numLoops;

					if (pingPong)
					{
						// Bounce back from the first frame:
						spriteAnims[curAnim].sprite.PlayAnim(spriteAnims[curAnim].anim, 1);
						stepDir *= -1;
						return;
					}
					else
					{
						// Hide the current sprite
						HideSprite(sp, true);

						// Unset our delegate:
						sp.SetAnimCompleteDelegate(null);

						// Start back at the first animation:
						curAnim = 0;
					}
				}
			}
		}
		else
		{
			// Unset our delegate:
			sp.SetAnimCompleteDelegate(null);
			HideSprite(sp, true);
			curAnim += stepDir;
		}

		// Proceed to play the next animation:
		HideSprite(spriteAnims[curAnim].sprite, false);
		spriteAnims[curAnim].sprite.SetAnimCompleteDelegate(AnimFinished);
		if (stepDir > 0)
			spriteAnims[curAnim].Play();
		else
			spriteAnims[curAnim].PlayInReverse();
	}


	/// <summary>
	/// Resets the animation to the beginning.
	/// </summary>
	public void Reset()
	{
		Stop();

		curAnim = 0;
		stepDir = 1;
		numLoops = 0;

		// Hide all but the first:
		for (int i = 1; i < spriteAnims.Length; ++i)
			HideSprite(spriteAnims[i].sprite, true);
	}

	/// <summary>
	/// Starts the animation to playing.
	/// </summary>
	public void Play()
	{
		// 		if(pingPong)
		// 		{	// Setup our last anim to loop back properly:
		// 			spriteAnims[spriteAnims.Length - 1].anim.loopReverse = true;
		// 		}

		isRunning = true;

		// Register our ending delegate:
		spriteAnims[curAnim].sprite.SetAnimCompleteDelegate(AnimFinished);

		HideSprite(spriteAnims[curAnim].sprite, false);
		spriteAnims[curAnim].Play();
	}

	/// <summary>
	/// Stops the animation playing.
	/// </summary>
	public void Stop()
	{
		isRunning = false;
		spriteAnims[curAnim].sprite.StopAnim();

		// Unset our delegate:
		spriteAnims[curAnim].sprite.SetAnimCompleteDelegate(null);
	}

	/// <summary>
	/// Pauses the animation.
	/// </summary>
	public void Pause()
	{
		isRunning = false;
		spriteAnims[curAnim].sprite.PauseAnim();
	}

	/// <summary>
	/// Unpauses the animation.
	/// </summary>
	public void Unpause()
	{
		isRunning = true;
		spriteAnims[curAnim].sprite.UnpauseAnim();
	}

	/// <summary>
	/// Tells whether the animation is running.
	/// </summary>
	public bool IsRunning
	{
		get { return isRunning; }
	}

	/// <summary>
	/// Returns a reference to the sprite currently showing
	/// for this animation.  Returns null if no sprite is
	/// the current one.
	/// </summary>
	public SpriteBase CurrentSprite
	{
		get
		{
			if (curAnim < 0 || curAnim >= spriteAnims.Length)
				return null;

			return spriteAnims[curAnim].sprite;
		}
	}

	/// <summary>
	/// Hides the sprite of the current animation.
	/// </summary>
	/// <param name="tf">Whether to hide the animation.</param>
	public void Hide(bool tf)
	{
		if (curAnim < 0 || curAnim >= spriteAnims.Length)
			return;

		HideSprite(spriteAnims[curAnim].sprite, tf);
	}

	/// <summary>
	/// Returns whether the current sprite
	/// in this animation is hidden:
	/// </summary>
	/// <returns></returns>
	public bool IsHidden()
	{
		if (curAnim < 0 || curAnim >= spriteAnims.Length)
			return false; // Assume no

		return spriteAnims[curAnim].sprite.IsHidden();
	}

	// Helper method that hides a sprite using the method
	// set for this animation (hiding vs deactivation).
	protected void HideSprite(SpriteBase sp, bool tf)
	{
		if (deactivateNonPlaying)
			if (deactivateRecursively)
				sp.gameObject.SetActiveRecursively(!tf);
			else
				sp.gameObject.active = !tf;
		else
			sp.Hide(tf);
	}

	/// <summary>
	/// Calls Delete() on each constituent sprite.
	/// </summary>
	public void Delete()
	{
		for (int i = 0; i < spriteAnims.Length; ++i)
			if (spriteAnims[i].sprite != null)
			{
				spriteAnims[i].sprite.Delete();
				UnityEngine.Object.Destroy(spriteAnims[i].sprite);
			}
	}
}


/// <remarks>
/// Wraps functionality of multiple sprites,
/// allowing you to define animations on
/// multiple sprites, which may use multiple
/// materials/atlases, and then link them
/// together and use them at runtime as 
/// though they were a single sprite.
/// </remarks>
[System.Serializable]
public class SuperSprite : MonoBehaviour
{
	/// <remarks>
	/// Defines a delegate that can be called upon animation completion.
	/// Use this if you want something to happen as soon as an animation
	/// reaches the end.  Receives a reference to the SuperSprite.
	/// </remarks>
	/// <param name="sprite">A reference to the SuperSprite whose animation has finished.</param>
	public delegate void AnimCompleteDelegate(SuperSprite sprite);


	/// <summary>
	/// Whether or not to play the default animation
	/// when the object is started.
	/// </summary>
	public bool playDefaultAnimOnStart = false;

	/// <summary>
	/// The default animation.
	/// </summary>
	public int defaultAnim = 0;

	/// <summary>
	/// The animations that comprise this SuperSprite.
	/// </summary>
	public SuperSpriteAnim[] animations = new SuperSpriteAnim[0];

	// The current animation.
	protected SuperSpriteAnim curAnim;

	protected bool animating;

	protected AnimCompleteDelegate animCompleteDelegate;

	protected SpriteBase.AnimFrameDelegate animFrameDelegate;

	protected bool m_started = false;


	// Use this for initialization
	public void Start()
	{
		if (m_started)
			return;
		m_started = true;

		// Initialize animations:
		for (int i = 0; i < animations.Length; ++i)
			if (animations[i] != null)
			{
				animations[i].Init(i, AnimFinished, animFrameDelegate);
			}

		if (playDefaultAnimOnStart)
			PlayAnim(animations[defaultAnim]);
	}

	/// <summary>
	/// Plays the specified SuperSprite animation.
	/// </summary>
	/// <param name="anim">The SuperSprite animation to be played.</param>
	public void PlayAnim(SuperSpriteAnim anim)
	{
		if (!m_started)
			Start();

		if (curAnim != null)
			curAnim.Hide(true);

		curAnim = anim;
		curAnim.Reset();

		animating = true;
		anim.Play();
	}

	/// <summary>
	/// Plays the specified SuperSprite animation.
	/// </summary>
	/// <param name="index">The index of the SuperSprite animation to be played.</param>
	public void PlayAnim(int index)
	{
		if (index < 0 || index >= animations.Length)
			return;

		PlayAnim(animations[index]);
	}

	/// <summary>
	/// Plays the specified SuperSprite animation.
	/// </summary>
	/// <param name="anim">The name of the SuperSprite animation to be played.</param>
	public void PlayAnim(string anim)
	{
		for (int i = 0; i < animations.Length; ++i)
			if (animations[i].name == anim)
			{
				PlayAnim(animations[i]);
				return;
			}
	}

	/// <summary>
	/// Plays the specified SuperSprite animation,
	/// but only if it isn't playing already.
	/// </summary>
	/// <param name="anim">The SuperSprite animation to be played.</param>
	public void DoAnim(SuperSpriteAnim anim)
	{
		if (curAnim != anim | !animating)
			PlayAnim(anim);
	}

	/// <summary>
	/// Plays the specified SuperSprite animation,
	/// but only if it isn't playing already.
	/// </summary>
	/// <param name="index">The index of the SuperSprite animation to be played.</param>
	public void DoAnim(int index)
	{
		if (curAnim == null)
			PlayAnim(index);
		else if (curAnim.index != index || !animating)
			PlayAnim(index);
	}

	/// <summary>
	/// Plays the specified SuperSprite animation,
	/// but only if it isn't playing already.
	/// </summary>
	/// <param name="anim">The name of the SuperSprite animation to be played.</param>
	public void DoAnim(string name)
	{
		if (curAnim == null)
			PlayAnim(name);
		else if (curAnim.name != name || !animating)
			PlayAnim(name);
	}

	/// <summary>
	/// Stops the current animation from playing
	/// and resets it to the beginning for playing
	/// again.
	/// </summary>
	public void StopAnim()
	{
		if (curAnim != null)
			curAnim.Stop();

		animating = false;
	}

	/// <summary>
	/// Pauses the currently playing animation.
	/// </summary>
	public void PauseAnim()
	{
		if (curAnim != null)
			curAnim.Pause();

		animating = false;
	}

	/// <summary>
	/// Resumes an animation from where it left off previously.
	/// </summary>
	public void UnpauseAnim()
	{
		if (curAnim != null)
		{
			curAnim.Unpause();
			animating = true;
		}
	}

	/// <summary>
	/// Hides the SuperSprite.
	/// </summary>
	/// <param name="tf"></param>
	public void Hide(bool tf)
	{
		if (curAnim != null)
		{
			curAnim.Hide(tf);

			if (!tf)
			{
				if (animating)
					curAnim.Pause();
			}
			else
			{
				if (animating)
					curAnim.Unpause();
			}
		}
	}

	/// <summary>
	/// Returns whether the SuperSprite is hidden.
	/// </summary>
	/// <returns>True if hidden, false otherwise.</returns>
	public bool IsHidden()
	{
		if (curAnim == null)
		{
			// Assume unhidden:
			return false;
		}

		return curAnim.IsHidden();
	}

	/// Returns a reference to the currently selected animation.
	/// NOTE: This does not mean the animation is currently playing.
	/// To determine whether the animation is playing, use <see cref="IsAnimating"/>
	/// </summary>
	/// <returns>Reference to the currently selected animation.</returns>
	public SuperSpriteAnim GetCurAnim()
	{
		return curAnim;
	}


	/// <summary>
	/// Returns a reference to the sprite that is currently showing
	/// for the current animation, if any.
	/// </summary>
	public SpriteRoot CurrentSprite
	{
		get
		{
			if (curAnim == null)
				return null;

			return curAnim.CurrentSprite;
		}
	}

	/// <summary>
	/// Returns the SuperSpriteAnim at the specified index.
	/// </summary>
	/// <param name="index">The index of the desired animation.</param>
	/// <returns>The desired animation, or null if the index is out of range.</returns>
	public SuperSpriteAnim GetAnim(int index)
	{
		if (index < 0 || index >= animations.Length)
			return null;

		return animations[index];
	}

	/// <summary>
	/// Returns the SuperSpriteAnim with the specified name.
	/// </summary>
	/// <param name="name">The name of the desired animation.</param>
	/// <returns>The desired animation, or null if no matching animation is found.</returns>
	public SuperSpriteAnim GetAnim(string name)
	{
		for (int i = 0; i < animations.Length; ++i)
			if (animations[i].name == name)
				return animations[i];

		return null;
	}

	/// <summary>
	/// Returns whether an animation is playing.
	/// </summary>
	/// <returns>Whether an animation is currently playing.</returns>
	public bool IsAnimating()
	{
		return animating;
	}

	// Is called when the animation finishes:
	protected void AnimFinished(SuperSpriteAnim anim)
	{
		animating = false;

		if (animCompleteDelegate != null)
			animCompleteDelegate(this);

		// Handle the OnAnimEnd action:
		if (curAnim != null)
		{
			switch (curAnim.onAnimEnd)
			{
				case SuperSpriteAnim.ANIM_END_ACTION.Play_Default_Anim:
					PlayAnim(defaultAnim);
					break;
				case SuperSpriteAnim.ANIM_END_ACTION.Deactivate:
					gameObject.SetActiveRecursively(false);
					break;
				case SuperSpriteAnim.ANIM_END_ACTION.Destroy:
					for (int i = 0; i < animations.Length; ++i)
						animations[i].Delete();

					Destroy(gameObject);
					break;
				default:
					// Do nothing
					break;
			}
		}
	}


	/// <summary>
	/// Sets the delegate to be called upon animation completion.
	/// </summary>
	/// <param name="del">The delegate to be called when an animation finishes playing.</param>
	public void SetAnimCompleteDelegate(AnimCompleteDelegate del)
	{
		animCompleteDelegate = del;
	}

	/// <summary>
	/// Sets the delegate to be called each frame of animation.
	/// </summary>
	/// <param name="del">The delegate to be called each frame of animation.</param>
	public void SetAnimFrameDelegate(SpriteBase.AnimFrameDelegate del)
	{
		animFrameDelegate = del;

		for(int i=0; i<animations.Length; ++i)
		{
			animations[i].SetAnimFrameDelegate(animFrameDelegate);
		}
	}
}
