//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;



// Provides a common parent stub class for 
// packable/aggregated sprite-based objects.
public abstract class PackableStub : MonoBehaviour
{
	// Collects all textures intended for packing,
	// as well as sprite frames, together into a 
	// standard form for processing.
	// Receives a delegate reference to AssetDatabase.LoadAssetAtPath
	public abstract void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid);

	// Provides access to the array of source textures.
	// NOTE: Should be ordered to parallel the
	// SpriteFrames array.
	public abstract Texture2D[] SourceTextures
	{
		get;
	}

	// Provides access to the array of Sprite Frames.
	// NOTE: Should be ordered to parallel the
	// SourceTextures array.
	public abstract CSpriteFrame[] SpriteFrames
	{
		get;
	}

	// Used for retrieving the material used by
	// the object for the purposes of building an
	// atlas texture for it.
	public abstract Material GetPackedMaterial(out string errString);

	public abstract CSpriteFrame DefaultFrame
	{
		get;
	}

	public abstract void SetUVs(Rect uvs);

	public abstract bool DoNotTrimImages
	{
		get;
		set;
	}
}
