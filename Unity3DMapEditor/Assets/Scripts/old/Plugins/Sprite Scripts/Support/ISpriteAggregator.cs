//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


public delegate Object LoadAssetDelegate(string path, System.Type type);
public delegate string PathFromGUIDDelegate(string path);
public delegate string GUIDFromPathDelegate(string guid);

public delegate string GetAssetPathDelegate(Object asset);


// Provides a common interface for aggregating,
// extracting, and assigning information relevant
// to texture atlas packing and subsequent UV (and
// other info) assignment.
public interface ISpriteAggregator
{
	// Collects all textures intended for packing,
	// as well as sprite frames, together into a 
	// standard form for processing.
	// Receives a delegate reference to AssetDatabase.LoadAssetAtPath
	void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid);

	// Provides access to the array of source textures.
	// NOTE: Should be ordered to parallel the
	// SpriteFrames array.
	Texture2D[] SourceTextures
	{
		get;
	}

	// Provides access to the array of Sprite Frames.
	// NOTE: Should be ordered to parallel the
	// SourceTextures array.
	CSpriteFrame[] SpriteFrames
	{
		get;
	}

	// Used for retrieving the material used by
	// the object for the purposes of building an
	// atlas texture for it.
	Material GetPackedMaterial(out string errString);

	CSpriteFrame DefaultFrame
	{
		get;
	}

	void SetUVs(Rect uvs);

	GameObject gameObject
	{
		get;
	}

	bool DoNotTrimImages
	{
		get;
	}
}
