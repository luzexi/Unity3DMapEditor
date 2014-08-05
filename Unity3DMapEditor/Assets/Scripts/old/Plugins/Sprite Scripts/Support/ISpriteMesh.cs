//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


// Provides a generic interface to an object
// that serves to encapsulate a sprite's mesh
// functionality.  This allows a sprite to be
// either managed (use a shared mesh), or 
// unmanaged (have its own mesh), without having
// to worry much about the difference.
public interface ISpriteMesh
{
	SpriteRoot sprite
	{
		get;
		set;
	}

	Texture texture
	{
		get;
	}

	Material material
	{
		get;
	}

	Vector3[] vertices
	{
		get;
	}

	Vector2[] uvs
	{
		get;
	}

	Vector2[] uvs2
	{
		get;
	}

	bool UseUV2
	{
		get;
		set;
	}

	void Init();

	void UpdateVerts();

	void UpdateUVs();

	void UpdateColors(Color color);

	void Hide(bool tf);

	bool IsHidden();
}
