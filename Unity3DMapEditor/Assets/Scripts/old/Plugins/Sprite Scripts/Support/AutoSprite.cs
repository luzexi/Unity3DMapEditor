//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


// Defines a very basic packable sprite.
public class AutoSprite : AutoSpriteBase
{
	// The animations as defined using individual textures.
	// See <see cref="TextureAnim"/>
	public TextureAnim[] textureAnimations;

	public override TextureAnim[] States
	{
		get { return textureAnimations; }
		set { textureAnimations = value; }
	}


	protected override void Awake()
	{
		if (textureAnimations == null)
			textureAnimations = new TextureAnim[0];

		base.Awake();

		Init();
	}


	protected override void Init()
	{
		base.Init();
	}
}
