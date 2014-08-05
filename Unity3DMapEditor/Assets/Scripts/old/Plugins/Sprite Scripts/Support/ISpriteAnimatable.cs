//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


// Interface for animatable sprite-based objects.
public interface ISpriteAnimatable 
{
	bool StepAnim(float time);

	ISpriteAnimatable prev
	{
		get;
		set;
	}

	ISpriteAnimatable next
	{
		get;
		set;
	}
}
