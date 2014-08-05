//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

using UnityEngine;
using System.Collections;


/// <remarks>
/// This class serves as a scene-wide store of all
/// fonts currently in use.  This is so we can
/// cache the font data once instead of having to
/// read from disk every time we create some text.
/// </remarks>
public static class FontStore
{
	// The list of fonts currently loaded.
	static SpriteFont[] fonts = new SpriteFont[0];

	/// <summary>
	/// Returns the SpriteFont object for the
	/// specified definition file.
	/// If no existing object is found, it is
	/// loaded from storage.
	/// </summary>
	/// <param name="fontDef">The TextAsset that defines the font.</param>
	/// <returns>A reference to the font definition object.</returns>
	public static SpriteFont GetFont(TextAsset fontDef)
	{
		if (fontDef == null)
			return null;

		for (int i = 0; i < fonts.Length; ++i)
		{
			if (fonts[i].fontDef == fontDef)
			{
				if (!Application.isPlaying)
					fonts[i] = new SpriteFont(fontDef); // Always force a reload

				return fonts[i];
			}
		}

		// If we're this far, no existing font was found.
		SpriteFont f = new SpriteFont(fontDef);
		AddFont(f);
		return f;
	}

	// Adds a font to our list:
	static void AddFont(SpriteFont f)
	{
		SpriteFont[] newFonts = new SpriteFont[fonts.Length + 1];
		fonts.CopyTo(newFonts, 0);
		newFonts[fonts.Length] = f;
		fonts = newFonts;
	}
}