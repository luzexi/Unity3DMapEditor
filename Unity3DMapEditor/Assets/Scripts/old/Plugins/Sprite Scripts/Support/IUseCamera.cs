//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


// Generic interface for any object that has a "render camera"
public interface IUseCamera
{
	void SetCamera();

	void SetCamera(Camera c);

	Camera RenderCamera
	{
		get;
		set;
	}
}
