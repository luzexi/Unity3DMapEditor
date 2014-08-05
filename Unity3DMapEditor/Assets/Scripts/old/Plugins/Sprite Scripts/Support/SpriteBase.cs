//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Describes a UV animation by holding all the information necessary
/// to move the UVs of a sprite across a texture atlas.
/// </remarks>
[System.Serializable]
public class UVAnimation
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
		/// Revert to the static image when the animation ends.
		/// </summary>
		Revert_To_Static,

		/// <summary>
		/// Play the default animation when the animation ends.
		/// </summary>
		Play_Default_Anim,

		/// <summary>
		/// Hide the sprite when the animation ends.
		/// </summary>
		Hide,

		/// <summary>
		/// Deactivate the sprite when the animation ends 
		/// (sets the GameObject's .active property to false).
		/// </summary>
		Deactivate,

		/// <summary>
		/// Destroys the sprite when the animation ends.
		/// </summary>
		Destroy
	};


	protected SPRITE_FRAME[] frames;					// Array of values defining the frames of an animation

	// Animation state vars:
	protected int curFrame = -1;					// The current frame
	protected int stepDir = 1;						// The direction we're currently playing the animation (1=forwards (default), -1=backwards)
	protected int numLoops = 0;						// Number of times we've looped since last animation
	protected bool playInReverse = false;			// Indicates that we've been instructed to play in reverse, as opposed to reversing merely as a result of loopReverse.

	protected float length;							// Length of the clip, in seconds (not taking into account looping, loop reversing, etc)

	/// <summary>
	/// The name of the animation.
	/// </summary>
	public string name;								// The name of the 

	/// <summary>
	/// How many times to loop the animation IN ADDITION to the initial play-through.  
	/// -1 indicates to loop infinitely.  
	/// 0 indicates to place the animation once then stop.  
	/// 1 indicates to repeat the animation once before 
	/// stopping, and so on.
	/// </summary>
	public int loopCycles = 0;						// How many times to loop the animation (-1 loop infinitely)
	
	/// <summary>
	/// Reverse the play direction when the end of the 
	/// animation is reached? (Ping-pong)
	/// If true, a loop iteration isn't counted until 
	/// the animation returns to the beginning.
	/// </summary>
	public bool loopReverse = false;				// Reverse the play direction when the end of the animation is reached? (if true, a loop iteration isn't counted until we return to the beginning)

	/// <summary>
	/// The rate in frames per second at which to play 
	/// the animation
	/// </summary>
	[HideInInspector]
	public float framerate = 15f;					// The rate in frames per second at which to play the animation

	/// <summary>
	/// The index of this animation in the sprite's 
	/// animation list.
	/// This value is only populated by and used for
	/// sprites derived from AutoSpriteBase.
	/// i.e. SM2 PackedSprites and EZ GUI controls.
	/// </summary>
	[HideInInspector]
	public int index = -1;

	/// <summary>
	/// What the sprite should do when the animation is done playing.
	/// The options are to: 1) Do nothing, 2) return to the static image,
	/// 3) play the default animation.
	/// </summary>
	[HideInInspector]
	public ANIM_END_ACTION onAnimEnd = ANIM_END_ACTION.Do_Nothing;

	/// <summary>
	/// The direction in which the animation is currently set to
	/// advance.  1 for forwards, -1 for backwards.
	/// </summary>
	public int StepDirection
	{
		get	{	return stepDir;	}
		set { SetStepDir(value); }
	}

	// Copy constructor:
	public UVAnimation(UVAnimation anim)
	{
		frames = new SPRITE_FRAME[anim.frames.Length];
		anim.frames.CopyTo(frames, 0);

		name = anim.name;

		loopCycles = anim.loopCycles;
		loopReverse = anim.loopReverse;
		framerate = anim.framerate;
		onAnimEnd = anim.onAnimEnd;

		curFrame = anim.curFrame;
		stepDir = anim.stepDir;
		numLoops = anim.numLoops;
		playInReverse = anim.playInReverse;
		length = anim.length;

		CalcLength();
	}

	public UVAnimation Clone()
	{
		return new UVAnimation(this);
	}
	
	public UVAnimation()
	{
		frames = new SPRITE_FRAME[0];
	}

	/// <summary>
	/// Resets all the animation state vars to ready the object
	/// for playing anew.
	/// </summary>
	public void Reset()
	{
		curFrame = -1;
		stepDir = 1;
		numLoops = 0;
		playInReverse = false;
	}

	// Sets the stepDir to -1 and sets the current frame to the end
	// so that the animation plays in reverse
	public void PlayInReverse()
	{
		stepDir = -1;
		curFrame = frames.Length;
		numLoops = 0;
		playInReverse = true;
	}

	public void SetStepDir(int dir)
	{
		if (dir < 0)
		{
			stepDir = -1;
			playInReverse = true;
		}
		else
			stepDir = 1;
	}

	// Stores the UV and other info of the next frame in 'nextFrame', 
	// returns false if we've reached the end of the animation (this 
	// will never happen if it is set to loop infinitely)
	public bool GetNextFrame(ref SPRITE_FRAME nextFrame)
	{
		if (frames.Length < 1)
			return false;

		// See if we can advance to the next frame:
		if ((curFrame + stepDir) >= frames.Length || (curFrame + stepDir) < 0)
		{
			// See if we need to loop (if we're reversing, we don't loop until we get back to the beginning):
			if (stepDir > 0 && loopReverse)
			{
				stepDir = -1;	// Reverse playback direction
				curFrame += stepDir;

				curFrame = Mathf.Clamp(curFrame, 0, frames.Length - 1);

				nextFrame = frames[curFrame];
			}
			else
			{
				// See if we can loop:
				if (numLoops + 1 > loopCycles && loopCycles != -1)
					return false;
				else
				{	// Loop the animation:
					++numLoops;

					if (loopReverse)
					{
						stepDir *= -1;
						curFrame += stepDir;
						curFrame = Mathf.Clamp(curFrame, 0, frames.Length - 1);
					}
					else
					{
						if (playInReverse)
							curFrame = frames.Length - 1;
						else
							curFrame = 0;
					}

					nextFrame = frames[curFrame];
				}
			}
		}
		else
		{
			curFrame += stepDir;
			nextFrame = frames[curFrame];
		}

		return true;
	}


	/// <summary>
	/// Returns the current frame info without advancing 
	/// to the next frame.
	/// </summary>
	/// <returns>A Rect containing the UV coordinates of the current frame.</returns>
	public SPRITE_FRAME GetCurrentFrame()
	{
		return frames[Mathf.Clamp(curFrame, 0, curFrame)];
	}


	/// <summary>
	/// Returns the specified frame.
	/// </summary>
	/// <param name="frame">The zero-based index of the frame.</param>
	/// <returns>The frame having the specified index.</returns>
	public SPRITE_FRAME GetFrame(int frame)
	{
		return frames[frame];
	}


	/// <summary>
	/// Constructs an array of frames based upon the info
	/// supplied.  NOTE: When the edge of the texture is reached,
	/// this algorithm will "wrap" to the next row down, starting
	/// directly below the position of the first animation cell
	/// on the row above.
	/// </summary>
	/// <param name="start">The UV of the lower-left corner of the first cell</param>
	/// <param name="cellSize">width and height, in UV space, of each cell</param>
	/// <param name="cols">Number of columns in the grid</param>
	/// <param name="rows">Number of rows in the grid</param>
	/// <param name="totalCells">Total number of cells in the grid (left-to-right, top-to-bottom ordering is assumed, just like reading English).</param>
	/// <returns>Array of SPRITE_FRAME objects that contain the UVs of each frame of animation.</returns>
	public SPRITE_FRAME[] BuildUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells)
	{
		int cellCount = 0;

		frames = new SPRITE_FRAME[totalCells];

		frames[0] = new SPRITE_FRAME(0);
		frames[0].uvs.x = start.x;
		frames[0].uvs.y = start.y;
		frames[0].uvs.xMax = start.x + cellSize.x;
		frames[0].uvs.yMax = start.y + cellSize.y;

		for (int row = 0; row < rows; ++row)
		{
			for (int col = 0; col < cols && cellCount < totalCells; ++col)
			{
				frames[cellCount] = new SPRITE_FRAME(0);
				frames[cellCount].uvs.x = start.x + cellSize.x * ((float)col);
				frames[cellCount].uvs.y = start.y - cellSize.y * ((float)row);
				frames[cellCount].uvs.xMax = frames[cellCount].uvs.x + cellSize.x;
				frames[cellCount].uvs.yMax = frames[cellCount].uvs.y + cellSize.y;

				++cellCount;
			}
		}

		CalcLength();
		
		return frames;
	}


	/// <summary>
	/// Constructs an array of frames based upon the info
	/// supplied.  NOTE: When the edge of the texture is reached,
	/// this algorithm will "wrap" to the extreme left edge of the
	/// atlas.
	/// </summary>
	/// <param name="start">The UV of the lower-left corner of the first cell</param>
	/// <param name="cellSize">width and height, in UV space, of each cell</param>
	/// <param name="cols">This argument is ignored but retained for compatibility.</param>
	/// <param name="rows">This argument is ignored but retained for compatibility.</param>
	/// <param name="totalCells">Total number of cells in the in the animation.</param>
	/// <returns>Array of SPRITE_FRAME objects that contain the UVs of each frame of animation.</returns>
	public SPRITE_FRAME[] BuildWrappedUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells)
	{
		return BuildWrappedUVAnim(start, cellSize, totalCells);
	}


	/// <summary>
	/// Constructs an array of frames based upon the info
	/// supplied.  NOTE: When the edge of the texture is reached,
	/// this algorithm will "wrap" to the extreme left edge of the
	/// atlas.
	/// </summary>
	/// <param name="start">The UV of the lower-left corner of the first cell</param>
	/// <param name="cellSize">width and height, in UV space, of each cell</param>
	/// <param name="totalCells">Total number of cells in the in the animation.</param>
	/// <returns>Array of SPRITE_FRAME objects that contain the UVs of each frame of animation.</returns>
	public SPRITE_FRAME[] BuildWrappedUVAnim(Vector2 start, Vector2 cellSize, int totalCells)
	{
		int cellCount = 0;
		Vector2 curPos;

		frames = new SPRITE_FRAME[totalCells];

		frames[0] = new SPRITE_FRAME(0);
		frames[0].uvs.x = start.x;
		frames[0].uvs.y = start.y;
		frames[0].uvs.xMax = start.x + cellSize.x;
		frames[0].uvs.yMax = start.y + cellSize.y;

		curPos = start;

		for (cellCount = 1; cellCount < totalCells; ++cellCount)
		{
			curPos.x += cellSize.x;
			if (curPos.x + cellSize.x > 1.01f)
			{
				curPos.x = 0;
				curPos.y -= cellSize.y;
			}

			frames[cellCount] = new SPRITE_FRAME(0);
			frames[cellCount].uvs.x = curPos.x;
			frames[cellCount].uvs.y = curPos.y;
			frames[cellCount].uvs.xMax = curPos.x + cellSize.x;
			frames[cellCount].uvs.yMax = curPos.y + cellSize.y;
		}

		return frames;
	}


	/// <summary>
	/// Assigns the specified array of frames to the
	/// animation, replacing its current contents.
	/// </summary>
	/// <param name="anim">Array of ANIM_FRAMEs which hold the UV
	/// coordinates defining the animation.</param>
	public void SetAnim(SPRITE_FRAME[] anim)
	{
		frames = anim;
		CalcLength();
	}

	/// <summary>
	/// Assigns all the various animation info to this animation
	/// from the specified TextureAnim, replacing any existing
	/// animation data.
	/// </summary>
	/// <param name="anim">Reference to the TextureAnim containing the desired info.</param>
	/// <param name="idx">The index of this animation.</param>
	public void SetAnim(TextureAnim anim, int idx)
	{
		if (anim == null)
			return;
		if (anim.spriteFrames == null)
			return;

		frames = new SPRITE_FRAME[anim.spriteFrames.Length];

		index = idx;
		name = anim.name;
		loopCycles = anim.loopCycles;
		loopReverse = anim.loopReverse;
		framerate = anim.framerate;
		onAnimEnd = anim.onAnimEnd;

		try
		{
			for (int i = 0; i < frames.Length; ++i)
			{
                frames[i] = anim.spriteFrames[i].ToStruct();
			}
		}
		catch (System.Exception err)
		{
			Debug.LogError("Exception caught in UVAnimation.SetAnim(). Make sure you have re-built your atlases!\nException: " + err.Message);
		}

		CalcLength();
	}

	/// <summary>
	/// Appends the specified array of frames to the
	/// existing animation.
	/// </summary>
	/// <param name="anim">Array of ANIM_FRAMEs which hold the UV
	/// coordinates defining the animation.</param>
	public void AppendAnim(SPRITE_FRAME[] anim)
	{
		SPRITE_FRAME[] tempFrames = frames;

		frames = new SPRITE_FRAME[frames.Length + anim.Length];
		tempFrames.CopyTo(frames, 0);
		anim.CopyTo(frames, tempFrames.Length);

		CalcLength();
	}

	/// <summary>
	/// Sets the current frame of animation.
	/// </summary>
	/// <param name="f">The number of the frame.</param>
	public void SetCurrentFrame(int f)
	{
		// Allow to go 1 out of bounds since this is
		// how we prepare to "advance" to the first or
		// last frame:
		f = Mathf.Clamp(f, -1, frames.Length + 1);

		curFrame = f;
	}

	/// <summary>
	/// Sets the current frame based on a 0-1 value indicating
	/// the desired position in the animation.  For example, a
	/// position of 0.5 would specify a frame half-way into the
	/// animation.  A position of 0 would specify the starting frame.
	/// A position of 1 would specify the last frame in the animation.
	/// NOTE: Loop cycles and loop reverse are taken into account.
	/// To set the frame without regard to loop cycles etc, use
	/// SetClipPosition().
	/// </summary>
	/// <param name="pos">Percentage of the way through the animation (0-1).</param>
	public void SetPosition(float pos)
	{
		pos = Mathf.Clamp01(pos);

		// If this is an infinitely looping animation,
		// or a single-play animation, just set the
		// position within the clip:
		if (loopCycles < 1)
		{
			SetClipPosition(pos);
			return;
		}

		// The percentage of total animation that is
		// accounted for by a single loop iteration:
		float iterationPct = 1f / (loopCycles + 1f);

		// The loop iteration containing the desired
		// frame:
		numLoops = Mathf.FloorToInt(pos / iterationPct);

		// Portion of our "pos" that is unaccounted for
		// merely by counting loop iterations:
		float remainder = pos - (((float)numLoops) * iterationPct);

		// Position within the "clip" (the "clip" being 
		// the series of frames between the first frame 
		// and last frame, without regard to the loop cycles)
		float clipPos = remainder / iterationPct;

		if(loopReverse)
		{
			if (clipPos < 0.5f)
			{
				curFrame = (int) ( ((float)frames.Length-1) * (clipPos/0.5f) );
				// We're stepping forward from here:
				stepDir = 1;
			}
			else
			{
				curFrame = (frames.Length-1) - (int)(((float)frames.Length-1) * ((clipPos-0.5f) / 0.5f));
				// We're stepping backwards from here:
				stepDir = -1;
			}
		}
		else
		{
			curFrame = (int) ( ((float)frames.Length-1) * clipPos );
		}
	}

	/// <summary>
	/// Sets the current frame based on a 0-1 value indicating
	/// the desired position in the animation.  For example, a
	/// position of 0.5 would specify a frame half-way into the
	/// animation.  A position of 0 would specify the starting frame.
	/// A position of 1 would specify the last frame in the animation.
	/// NOTE: Loop cycles and loop reverse are NOT taken into account.
	/// Rather, this method sets the desired frame within the clip,
	/// the clip being the series of frames 0-n without regard to
	/// loop cycles or loop reversing.
	/// To set the frame with regard to loop cycles etc, use
	/// SetPosition().
	/// </summary>
	/// <param name="pos">Percentage of the way through the animation (0-1).</param>
	public void SetClipPosition(float pos)
	{
		curFrame = (int)(((float)frames.Length - 1) * pos);
	}

	// Calculates the length of the animation clip (not accounting for looping, loop reversing, etc)
	protected void CalcLength()
	{
		length = (1f / framerate) * frames.Length;
	}

	/// <summary>
	/// Returns the length, in seconds, of the animation.
	/// NOTE: This does not take into account looping or reversing.
	/// It simply returns the length, in seconds, of the animation, when
	/// played once from beginning to end.
	/// To get the duration of the animation including looping and reversing,
	/// use GetDuration().
	/// </summary>
	/// <returns>The length of the animation in seconds.</returns>
	public float GetLength()
	{
		return length;
	}

	/// <summary>
	/// Returns the duration, in seconds, of the animation.
	/// NOTE: This takes into account loop cycles and loop reverse.
	/// Ex: If an animation has a framerate of 30fps, consists of 60
	/// frames, and is set to loop once, the duration will be 4 seconds.
	/// To retrieve the length of the animation without regard to the loop
	/// cycles and loop reverse settings, use GetLength().
	/// </summary>
	/// <returns>The duration of the animation in seconds.  -1 if the animation loops infinitely.</returns>
	public float GetDuration()
	{
		// If this loops infinitely, return -1:
		if(loopCycles < 0)
			return -1f;

		float length = GetLength();

		if (loopReverse)
			length *= 2f;

		return length + (loopCycles * length);
	}

	/// <summary>
	/// Returns the number of frames in the animation.
	/// </summary>
	/// <returns>The number of frames in the animation.</returns>
	public int GetFrameCount()
	{
		return frames.Length;
	}

	/// <summary>
	/// Returns the number of frames displayed by the
	/// animation according to its current loop cycles
	/// and loop reverse settings.
	/// </summary>
	/// <returns>The number of frames displayed, -1 if set to loop infinitely.</returns>
	public int GetFramesDisplayed()
	{
		if (loopCycles == -1)
			return -1;

		int count = frames.Length + (frames.Length * loopCycles);
		
		if (loopReverse)
			count *= 2;

		return count;
	}

	/// <summary>
	/// Returns the (zero-based) index of the current frame.
	/// </summary>
	/// <returns>Zero-based index of the current frame.</returns>
	public int GetCurPosition()
	{
		return curFrame;
	}
}


/// <remarks>
/// This derived class allows you to specify parameters in-editor
/// that will build an animation for you.
/// </remarks>
[System.Serializable]
public class UVAnimation_Auto : UVAnimation
{
	/// <summary>
	/// The pixel coordinates of the lower-left corner of the first
	/// frame in the animation sequence.
	/// </summary>
	public Vector2 start;

	/// <summary>
	/// The number of pixels from the left edge of one sprite frame
	/// to the left edge of the next one, and the number of pixels
	/// from the top of one sprite frame to the top of the one in
	/// the next row.  You may also want to think of this as the
	/// size (width and height) of each animation cell.
	/// </summary>
	public Vector2 pixelsToNextColumnAndRow;

	/// <summary>
	/// The number of columns in the animation.
	/// </summary>
	public int cols;
	
	/// <summary>
	/// The number of rows in the animation.
	/// </summary>
	public int rows;

	/// <summary>
	/// The total number of frames (cells) of animation.
	/// </summary>
	public int totalCells;

	// No-arg constructor:
	public UVAnimation_Auto() {}

	// Copy constructor:
	public UVAnimation_Auto(UVAnimation_Auto anim) : base(anim)
	{
		start = anim.start;
		pixelsToNextColumnAndRow = anim.pixelsToNextColumnAndRow;
		cols = anim.cols;
		rows = anim.rows;
		totalCells = anim.totalCells;
	}

	public new UVAnimation_Auto Clone()
	{
		return new UVAnimation_Auto(this);
	}
	
	/// <summary>
	/// Uses the information stored in this class to build
	/// a UV animation for the specified sprite.
	/// </summary>
	/// <param name="s">The sprite for which the animation will be built.</param>
	/// <returns>An array of ANIM_FRAMEs that define the animation.</returns>
	public SPRITE_FRAME[] BuildUVAnim(SpriteRoot s)
	{
		if (totalCells < 1)
			return null;

		return this.BuildUVAnim(s.PixelCoordToUVCoord(start), s.PixelSpaceToUVSpace(pixelsToNextColumnAndRow), cols, rows, totalCells);
	}
}


/// <remarks>
/// This class allows you to specify multiple 
/// animation "clips" that will play sequentially
/// to the end of the list of clips.
/// </remarks>
[System.Serializable]
public class UVAnimation_Multi
{
	/// <summary>
	/// The name of the animation sequence
	/// </summary>
	public string name;					// The name of the animation sequence

	/// <summary>
	/// How many times to loop the animation IN ADDITION TO the initial play-through. (-1 to loop infinitely, 0 not to loop at all, 1 to repeat once before stopping, etc.)
	/// </summary>
	public int loopCycles = 0;			// How many times to loop the animation (-1 loop infinitely)

	/// <summary>
	/// Reverse the play direction when the end of the animation is reached? If true, a loop iteration isn't counted until the animation returns to the beginning.
	/// </summary>
	public bool loopReverse = false;	// Reverse the play direction when the end of the animation is reached? (if true, a loop iteration isn't counted until we return to the beginning)

	/// <summary>
	/// The rate in frames per second at which to play the animation.
	/// </summary>
	public float framerate = 15f;		// The rate in frames per second at which to play the animation

	/// <summary>
	/// What the sprite should do when the animation is done playing.
	/// The options are to: 1) Do nothing, 2) return to the static image,
	/// 3) play the default animation.
	/// </summary>
	public UVAnimation.ANIM_END_ACTION onAnimEnd = UVAnimation.ANIM_END_ACTION.Do_Nothing;

	/// <summary>
	/// The actual sprite animation clips that make up the animation sequence.
	/// </summary>
	public UVAnimation_Auto[] clips;	// The actual sprite animation clips that make up this animation sequence

	[HideInInspector]
	public int index;

	protected int curClip;				// Index of the currently-playing clip
	protected int stepDir = 1;			// The direction to step through our clips (1 == forwards, -1 == backwards)
	protected int numLoops = 0;			// Number of times we've looped since last animation

	protected float duration;			// The duration of the animation, accounting for looping and loop reversing.


	// Working vars:
	protected bool ret;
	protected int i;
	protected int framePos = -1;		// Keeps track of which frame (over all) we are on in the animation multi.
										// In other words, if the multi contains a total of 100 frames across all 
										// sub-animations, then this value will count from 0-99 (and back again if
										// if we're reversing).


	public UVAnimation_Multi()
	{
		if (clips == null)
			clips = new UVAnimation_Auto[0];
	}

	// Copy constructor:
	public UVAnimation_Multi(UVAnimation_Multi anim)
	{
		name = anim.name;
		loopCycles = anim.loopCycles;
		loopReverse = anim.loopReverse;
		framerate = anim.framerate;
		onAnimEnd = anim.onAnimEnd;
		curClip = anim.curClip;
		stepDir = anim.stepDir;
		numLoops = anim.numLoops;
		duration = anim.duration;

		clips = new UVAnimation_Auto[anim.clips.Length];
		for (int i = 0; i < clips.Length; ++i)
			clips[i] = anim.clips[i].Clone();

		CalcDuration();
	}

	public UVAnimation_Multi Clone()
	{
		return new UVAnimation_Multi(this);
	}

	/// <summary>
	/// Gets a reference to the currently-playing clip.
	/// </summary>
	/// <returns>Reference to the currently-playing clip.</returns>
	public UVAnimation_Auto GetCurrentClip()
	{
		return clips[curClip];
	}

	/// <summary>
	/// The direction in which the animation is currently set to
	/// advance.  1 for forwards, -1 for backwards.
	/// </summary>
	public int StepDirection
	{
		get { return stepDir; }
		set { stepDir = value; }
	}

	/// <summary>
	/// Builds the UV animations for all animation clips
	/// that are a part of this animation sequence.
	/// </summary>
	/// <param name="s">The sprite for which to build the animation.</param>
	/// <returns>Array of animation clips that constitute the animation sequence.</returns>
	public UVAnimation_Auto[] BuildUVAnim(SpriteRoot s)
	{
		for (i = 0; i < clips.Length; ++i)
		{
			clips[i].BuildUVAnim(s);
		}

		CalcDuration();

		return clips;
	}

	public bool GetNextFrame(ref SPRITE_FRAME nextFrame)
	{
		if (clips.Length < 1)
			return false;

		ret = clips[curClip].GetNextFrame(ref nextFrame);

		if (!ret)
		{
			// See if we have another clip in the queue:
			if ((curClip + stepDir) >= clips.Length || (curClip + stepDir) < 0)
			{
				// See if we need to loop (if we're reversing, we don't loop until we get back to the beginning):
				if (stepDir > 0 && loopReverse)
				{
					stepDir = -1;	// Reverse playback direction
					curClip += stepDir;

					curClip = Mathf.Clamp(curClip, 0, clips.Length - 1);

					// Make the newly selected clip ready for playing:
					clips[curClip].Reset();
					clips[curClip].PlayInReverse();
					// Go ahead and step one since it will start
					// at the end, and that's where we are already:
					clips[curClip].GetNextFrame(ref nextFrame);
				}
				else
				{
					// See if we can loop:
					if (numLoops + 1 > loopCycles && loopCycles != -1)
						return false;	// We've reached the end of the last clip
					else
					{	// Loop the animation:
						++numLoops;

						if (loopReverse)
						{
							stepDir *= -1;
							curClip += stepDir;

							curClip = Mathf.Clamp(curClip, 0, clips.Length - 1);

							// Make the newly selected clip ready for playing:
							clips[curClip].Reset();

							if (stepDir < 0)
								clips[curClip].PlayInReverse();

							// Go ahead and step one since it will start
							// where we are already:
							clips[curClip].GetNextFrame(ref nextFrame);
						}
						else
						{
							curClip = 0;
							framePos = -1;

							// Make the newly selected clip ready for playing:
							clips[curClip].Reset();
						}
					}
				}
			}
			else
			{
				curClip += stepDir;

				// Make the newly selected clip ready for playing:
				clips[curClip].Reset();

				if (stepDir < 0)
				{
					clips[curClip].PlayInReverse();
					// Go ahead and step one since it will start
					// at the end, and that's where we are already:
					clips[curClip].GetNextFrame(ref nextFrame);
				}
			}

			framePos += stepDir;

			// Get the next frame since our previous
			// attempt failed, and all that we just
			// did was to be able to get the next one:
			clips[curClip].GetNextFrame(ref nextFrame);

			return true;	// Keep playing
		}

		framePos += stepDir;

		/*
				// Simpler, non-looping logic:
				if (curClip < clips.Length - 1)
				{
					// Go to the next clip:
					++curClip;
					return true;	// Keep playing
				}
				else
					return false;	// We've reached the end of the last clip
		*/

		return true;
	}


	/// <summary>
	/// Returns the SPRITE_FRAME of the current frame without
	/// advancing to the next frame.
	/// </summary>
	/// <returns>An SPRITE_FRAME containing the UV coordinates of the current frame.</returns>
	public SPRITE_FRAME GetCurrentFrame()
	{
		return clips[Mathf.Clamp(curClip, 0, curClip)].GetCurrentFrame();
	}


	/// <summary>
	/// Appends UV animation to the clip specified by index.
	/// </summary>
	/// <param name="index">The animation clip to append to.</param>
	/// <param name="anim">Array of ANIM_FRAMEs that define the animation to be appended.</param>
	public void AppendAnim(int index, SPRITE_FRAME[] anim)
	{
		if (index >= clips.Length)
			return;

		clips[index].AppendAnim(anim);

		CalcDuration();
	}

	/// <summary>
	/// Appends UV animation clip to the end of the animation sequence.
	/// </summary>
	/// <param name="clip">Animation clip to append.</param>
	public void AppendClip(UVAnimation clip)
	{
		UVAnimation[] temp;
		temp = clips;

		clips = new UVAnimation_Auto[clips.Length + 1];
		temp.CopyTo(clips, 0);

		clips[clips.Length - 1] = (UVAnimation_Auto)clip;

		CalcDuration();
	}

	public void PlayInReverse()
	{
		for (i = 0; i < clips.Length; ++i)
		{
			clips[i].PlayInReverse();
		}

		stepDir = -1;
		framePos = GetFrameCount()-1;
		curClip = clips.Length - 1;
	}

	/// <summary>
	/// Replaces the contents of the specified clip.
	/// </summary>
	/// <param name="index">Index of the clip the contents of which you wish to replace.</param>
	/// <param name="frames">Array of ANIM_FRAMEs that define the content of an animation clip.</param>
	public void SetAnim(int index, SPRITE_FRAME[] frames)
	{
		if (index >= clips.Length)
			return;

		clips[index].SetAnim(frames);

		CalcDuration();
	}

	/// <summary>
	/// Resets the animation sequence for playing anew.
	/// </summary>
	public void Reset()
	{
		curClip = 0;
		stepDir = 1;
		numLoops = 0;
		framePos = -1;

		for (i = 0; i < clips.Length; ++i)
		{
			clips[i].Reset();
		}
	}

	/// <summary>
	/// Sets the current playing position of the animation.
	/// NOTE: This method takes loop cycles and loop reversing
	/// into account.  To set the position without regard to
	/// loop cycles or loop reversing, use SetAnimPosition().
	/// </summary>
	/// <param name="pos">Desired position in the animation (0-1).</param>
	public void SetPosition(float pos)
	{
		pos = Mathf.Clamp01(pos);

		// If this is an infinitely looping animation,
		// or if it is a single-play animation, just
		// set the position:
		if(loopCycles < 1)
		{
			SetAnimPosition(pos);
			return;
		}

		// The percentage of total animation that is
		// accounted for by a single loop iteration:
		float iterationPct = 1f / (loopCycles + 1f);

		// Find the loop iteration of the desired position:
		numLoops = Mathf.FloorToInt(pos / iterationPct);

		// Portion of our "pos" that is unaccounted for
		// merely by counting loop iterations:
		float remainder = pos - (((float)numLoops) * iterationPct);

		SetAnimPosition(remainder / iterationPct);
	}

	/// <summary>
	/// Sets the current playing position of the animation.
	/// NOTE: This method does NOT take loop cycles and loop 
	/// reversing into account.  To set the position taking 
	/// loop cycles and loop reversing into account, use 
	/// SetPosition().
	/// </summary>
	/// <param name="pos">Desired position in the animation (0-1).</param>
	public void SetAnimPosition(float pos)
	{
		int totalFrames = 0;
		float pct;
		float remaining = pos;

		// Get the total number of frames:
		for (int n = 0; n < clips.Length; ++n)
		{
			totalFrames += clips[n].GetFramesDisplayed();
		}

		// Find which clip our desired position is in:
		if(loopReverse)
		{
			if(pos < 0.5f)
			{
				// We will step forward from here:
				stepDir = 1;

				// Adjust to account for the fact that a value
				// of .5 in this context means 100% of the way
				// from the first frame to the last frame:
				remaining *= 2f;

				for (int n = 0; n < clips.Length; ++n)
				{
					// Get the percentage of our animation
					// that is accounted for by this clip:
					pct = clips[n].GetFramesDisplayed() / totalFrames;

					// If the distance we have left to go into
					// our animation is less than the distance
					// accounted for by this clip, this is the
					// clip we're looking for!:
					if (remaining <= pct)
					{
						curClip = n;
						clips[curClip].SetPosition(remaining / pct);

						framePos = ((int)pct * (totalFrames-1));

						return;
					}
					else
						remaining -= pct;
				}
			}
			else
			{
				// We will step backward from here:
				stepDir = -1;

				// Adjust for the fact that in this context, 
				// a value of .5 means 0% of the way from the
				// last frame to the first frame:
				remaining = (remaining-0.5f) / 0.5f;

				for (int n = clips.Length-1; n >= 0; --n)
				{
					// Get the percentage of our animation
					// that is accounted for by this clip:
					pct = clips[n].GetFramesDisplayed() / totalFrames;

					// If the distance we have left to go into
					// our animation is less than the distance
					// accounted for by this clip, this is the
					// clip we're looking for!:
					if (remaining <= pct)
					{
						curClip = n;
						clips[curClip].SetPosition(1f - (remaining / pct));
						clips[curClip].SetStepDir(-1);

						framePos = ((int)pct * (totalFrames - 1));
						
						return;
					}
					else
						remaining -= pct;
				}
			}
		}
		else
		{
			for (int n = 0; n < clips.Length; ++n)
			{
				// Get the percentage of our animation
				// that is accounted for by this clip:
				pct = clips[n].GetFramesDisplayed() / totalFrames;

				// If the distance we have left to go into
				// our animation is less than the distance
				// accounted for by this clip, this is the
				// clip we're looking for!:
				if (remaining <= pct)
				{
					curClip = n;
					clips[curClip].SetPosition(remaining / pct);

					framePos = ((int)pct * (totalFrames - 1));
					
					return;
				}
				else
					remaining -= pct;
			}
		}
	}

	// Calculates the duration of the animation:
	protected void CalcDuration()
	{
		// If this loops infinitely, set duration to -1:
		if (loopCycles < 0)
		{
			duration = -1f;
			return;
		}

		duration = 0;

		for (int n = 0; n < clips.Length; ++n)
		{
			duration += clips[n].GetDuration();
		}

		if (loopReverse)
			duration *= 2f;

		duration += (loopCycles * duration);
	}

	/// <summary>
	/// Returns the duration, in seconds, of the animation.
	/// NOTE: This takes into account loop cycles and loop reverse.
	/// Ex: If an animation has a framerate of 30fps, consists of 60
	/// frames, and is set to loop once, the duration will be 4 seconds.
	/// </summary>
	/// <returns>The duration of the animation in seconds. -1 if the animation loops infinitely.</returns>
	public float GetDuration()
	{
		return duration;
	}

	/// <summary>
	/// Returns the total number of frames displayed by
	/// this animation.
	/// </summary>
	/// <returns>Number of frames to be displayed.</returns>
	public int GetFrameCount()
	{
		int totalFrames = 0;

		// Get the total number of frames:
		for (int n = 0; n < clips.Length; ++n)
		{
			totalFrames += clips[n].GetFramesDisplayed();
		}

		return totalFrames;
	}

	/// <summary>
	/// Returns the (zero-based) frame number of the current position in the over all animation.
	/// Example: If the multi contains a total of 100 frames and 25 frames have played
	/// so far, then 24 will be returned (because it is zero-based). If the multi is playing 
	/// backwards, this number will count down from 100 as well.
	/// </summary>
	/// <returns>Zero-based frame number of the current position in the over all animation.</returns>
	public int GetCurPosition()
	{
		return framePos;
	}

	/// <summary>
	/// Returns the (zero-based) index of the current clip.
	/// </summary>
	/// <returns>The (zero-based) index of the current clip.</returns>
	public int GetCurClipNum()
	{
		return curClip;
	}

	/// <summary>
	/// Sets the current clip by index.
	/// </summary>
	/// <param name="index">The zero-based index of the clip.</param>
	public void SetCurClipNum(int index)
	{
		curClip = index;
	}
}



/// <remarks>
/// Serves as the base for defining an animatable sprite.
/// This class should not actually be used despite
/// the fact that Unity will allow you to attach it
/// to a GameObject.
/// </remarks>
[ExecuteInEditMode]
public abstract class SpriteBase : SpriteRoot, ISpriteAnimatable
{
	// Animation-related vars and types:

	/// <remarks>
	/// Defines a delegate that can be called upon animation completion.
	/// Use this if you want something to happen as soon as an animation
	/// reaches the end.  Receives a reference to the sprite.
	/// </remarks>
	/// <param name="sprite">A reference to the sprite whose animation has finished.</param>
	public delegate void AnimCompleteDelegate(SpriteBase sprite);		// Definition of delegate to be called upon animation completion

	/// <remarks>
	/// Defines a delegate that can be called once for each frame of animation.
	/// Use this if you want something to happen on an animation frame.
	/// Receives a reference to the sprite and the zero-based index of the 
	/// current frame as an argument.
	/// </remarks>
	/// <param name="sprite">A reference to the sprite which is calling the delegate.</param>
	/// <param name="frame">The current animation frame number. (0-based)</param>
	public delegate void AnimFrameDelegate(SpriteBase sprite, int frame);

	/// <summary>
	/// When set to true, the sprite will play the default
	/// animation (see <see cref="defaultAnim"/>) when the sprite
	/// is instantiated.
	/// </summary>
	public bool playAnimOnStart = false;				// When set to true, will start playing the default animation on start

	// <summary>
	// When set to true, the sprite will crossfade between the current frame
	// and the next frame in the sprite animation.  NOTE: requires the use of the
	// "Crossfade Sprite" shader, which replaces sprite coloring functionality.
	// </summary>
	[HideInInspector]
	public bool crossfadeFrames = false;

	/// <summary>
	/// Index of the animation to play by default.
	/// </summary>
	public int defaultAnim = 0;							// Index of the default animation


	protected int curAnimIndex = 0;						// Index of the "current" animation, if any

	protected AnimCompleteDelegate animCompleteDelegate = null;	// Delegate to be called upon animation completion
	protected AnimFrameDelegate animFrameDelegate = null; // Delegate to be called each frame
	protected float timeSinceLastFrame = 0;				// The total time since our last animation frame change
	protected float timeBetweenAnimFrames;				// The amount of time we want to pass before moving to the next frame of animation
	protected float framesToAdvance;						// (working) The number of animation frames to advance given the time elapsed
	protected bool animating = false;					// True when an animation is playing
	protected SPRITE_FRAME nextFrameInfo = new SPRITE_FRAME(0);


	protected override void Awake()
	{
		base.Awake();

		// Start the shared animation coroutine if it is not running already:
// 		if (!SpriteAnimationPump.pumpIsRunning && Application.isPlaying)
// 			SpriteAnimationPump.Instance.StartAnimationPump();
	}

	public override void Start()
	{
		base.Start();

		if(m_spriteMesh != null)
			m_spriteMesh.UseUV2 = crossfadeFrames;
	}


	/// <summary>
	/// Resets important sprite values to defaults for reuse.
	/// </summary>
	public override void Clear()
	{
		base.Clear();

		//animations.Clear();
		animCompleteDelegate = null;
	}

	public override void Delete()
	{
		if (animating)
		{
			// Remove ourselves from the animating list.
			RemoveFromAnimatedList();

			// Leave "animating" set to true so that when
			// we re-enable, we can pick up animating again:
			animating = true;
		}

		base.Delete();
	}

	// Called when the GO is disabled or destroyed
	protected override void OnDisable()
	{
		base.OnDisable();

		if(animating)
		{
			// Remove ourselves from the animating list.
			RemoveFromAnimatedList();

			// Leave "animating" set to true so that when
			// we re-enable, we can pick up animating again:
			animating = true;
		}
	}

	// Called when the GO is enabled or created:
	protected override void OnEnable()
	{
		base.OnEnable();

		// If this is being called in edit mode,
		// disregard:
		if (!Application.isPlaying)
			return;

		// If we were previously animating,
		// resume animation:
		if (animating)
		{
			// Set to false so AddToAnimatingList()
			// won't bail out:
			animating = false;
			AddToAnimatedList();
		}
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);

		if (!(s is SpriteBase))
			return;

		SpriteBase sb = (SpriteBase)s;

		defaultAnim = sb.defaultAnim;
		playAnimOnStart = sb.playAnimOnStart;
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);

		// If we're hideAtStart, pause animation:
		if (tf)
			PauseAnim();
	}


/*
	/// <summary>
	/// Allows you to setup some of the main features of a sprite in a single method call.
	/// </summary>
	/// <param name="width">Width of the sprite in world space.</param>
	/// <param name="height">Height of the sprite in world space.</param>
	/// <param name="lowerleftPixel">Lower-left pixel of the sprite when no animation has been played.</param>
	/// <param name="pixeldimensions">Pixel dimeinsions of the sprite when no animation has been played.</param>
	public virtual void Setup(float width, float height, Vector2 lowerleftPixel, Vector2 pixeldimensions)
	{
		SetSize(width, height);
		lowerLeftPixel = lowerleftPixel;
		pixelDimensions = pixeldimensions;

		tempUV = PixelCoordToUVCoord(lowerLeftPixel);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;

		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;

		SetBleedCompensation(bleedCompensation);
	}
*/


	//-----------------------------------------------------------------
	// Animation-related routines:
	//-----------------------------------------------------------------

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
	public void SetAnimFrameDelegate(AnimFrameDelegate del)
	{
		animFrameDelegate = del;
	}

	/// <summary>
	/// Sets the delegate to be called when the sprite is resized.
	/// </summary>
	/// <param name="del">The delegate to be called when the sprite is resized.</param>
	public void SetSpriteResizedDelegate(SpriteResizedDelegate del)
	{
		resizedDelegate = del;
	}

	/// <summary>
	/// Adds the delegate to be called when the sprite is resized.
	/// </summary>
	/// <param name="del">A delegate to be called when the sprite is resized.</param>
	public void AddSpriteResizedDelegate(SpriteResizedDelegate del)
	{
		resizedDelegate += del;
	}

	/// <summary>
	/// Removes the specified delegate from the list of those to be called when the sprite is resized.
	/// </summary>
	/// <param name="del">The delegate to be removed.</param>
	public void RemoveSpriteresizedDelegate(SpriteResizedDelegate del)
	{
		resizedDelegate -= del;
	}

	public virtual bool StepAnim(float time) { return false; }

	public virtual void PlayAnim(int index) { }
	public virtual void PlayAnim(string name) { }
	public virtual void PlayAnimInReverse(int index) { }
	public virtual void PlayAnimInReverse(string name) { }
/*	public virtual void PlayAnim(int index, int frame) {}
	public virtual void PlayAnim(string name, int frame) {}
*/

	/// <summary>
	/// Changes the framerate at which the current animation plays.
	/// NOTE: This only has effect if called AFTER PlayAnim() is called.
	/// Otherwise, PlayAnim() sets the framerate to whatever is specified 
	/// in the animation itself.
	/// </summary>
	/// <param name="fps">The new framerate, in frames per second.</param>
	public void SetFramerate(float fps)
	{
		timeBetweenAnimFrames = 1f / fps;
	}


	/// <summary>
	/// Pauses the currently-playing animation.
	/// </summary>
	public void PauseAnim()
	{
		if(animating)
			RemoveFromAnimatedList();
		// Stop coroutine
		//animating = false;
		//StopCoroutine("AnimationPump");
		//StopAllCoroutines();
	}


	/// <summary>
	/// Stops the current animation from playing
	/// and resets it to the beginning for playing
	/// again.  The sprite then reverts to the static
	/// image.
	/// </summary>
	public virtual void StopAnim(){}


	/// <summary>
	/// Reverts the sprite to its static (non-animating) default appearance.
	/// </summary>
	public void RevertToStatic()
	{
		if (animating)
			StopAnim();

		InitUVs();
		SetBleedCompensation();

		if (autoResize || pixelPerfect)
			CalcSize();
	}

	// Adds the sprite to the list of currently
	// animating sprites:
	protected abstract void AddToAnimatedList();

	// Removes the sprite from the list of currently
	// animating sprites:
	protected abstract void RemoveFromAnimatedList();


	//--------------------------------------------------------------
	// Accessors:
	//--------------------------------------------------------------
	/// <summary>
	/// Returns whether the sprite is currently animating.
	/// </summary>
	/// <returns>True if the sprite is currently animating, false otherwise.</returns>
	public bool IsAnimating() { return animating; }

	/// <summary>
	/// Property useful for use with EZ Game Saver.
	/// When it gets set, the current animation is started.
	/// Include this, along with CurAnimIndex, as a saved 
	/// property so that any playing animation is resumed.
	/// </summary>
	public bool Animating
	{
		get { return animating; }

		set
		{
			if(value)
				PlayAnim(curAnimIndex);
		}
	}

	/// <summary>
	/// Property useful for use with EZ Game Saver.
	/// Include this as a saved property, along with
	/// Animating, so that any playing animation is 
	/// resumed.
	/// </summary>
	public int CurAnimIndex
	{
		get { return curAnimIndex; }		
		set	{ curAnimIndex = value; }
	}
}



