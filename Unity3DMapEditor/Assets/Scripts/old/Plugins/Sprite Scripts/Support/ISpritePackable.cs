//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


// Interface for packable sprite-based objects.
public interface ISpritePackable 
{
	GameObject gameObject
	{
		get;
	}

	TextureAnim[] States
	{
		get;
		set;
	}

	SpriteBase.ANCHOR_METHOD Anchor
	{
		get;
	}

	Color Color
	{
		get;
		set;
	}

	bool SupportsArbitraryAnimations
	{
		get;
	}
}
