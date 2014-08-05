//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define WARN_ON_NO_MATERIAL
#define WANT_NORMALS
#define AVOID_SPLIT


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <remarks>
/// A class that impelements sprite-based text.
/// </remarks>
[ExecuteInEditMode]
[System.Serializable]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("EZ GUI/Controls/Label")]
public class SpriteText : MonoBehaviour, IUseCamera 
{
	/// <remarks>
	/// Which point of the text shares the position of the Transform.
	/// </remarks>
	public enum Anchor_Pos
	{
		Upper_Left,
		Upper_Center,
		Upper_Right,
		Middle_Left,
		Middle_Center,
		Middle_Right,
		Lower_Left,
		Lower_Center,
		Lower_Right
	}

	/// <summary>
	/// How the text will be aligned.
	/// </summary>
	public enum Alignment_Type
	{
		Left,
		Center,
		Right
	}

	protected struct NewlineInsertInfo
	{
		// Index in the plain text where this was inserted/replaced.
		public int index;

		// The change in characters when this newline was inserted.
		public int charDelta;

		public NewlineInsertInfo(int idx, int delta)
		{
			index = idx;
			charDelta = delta;
		}
	}

	//---------------------------------------
	// Data members:
	//---------------------------------------

	/// <summary>
	/// The (decorated) text the object is to contain.
	/// NOTE: This is only for use in the inspector at edit time.
	/// The ".Text" or ".PlainText" properties should be used in
	/// script at runtime.
	/// </summary>
	public string text = "Hello World";

	/// <summary>
	/// The distance the text is to be offset along the local Z-axis
	/// from the GameObject's center.
	/// </summary>
	public float offsetZ;

	/// <summary>
	/// The size, in world units, of a full-height character.
	/// All other characters will be sized proportionately.
	/// </summary>
	public float characterSize = 1f;

	/// <summary>
	/// An adjustable factor by which you can increase/decrease
	/// the spacing between characters.  A value of 1.0 will
	/// space characters exactly as described by the font.
	/// Decreasing this value will place the characters closer
	/// together, while increasing it will place them farther
	/// apart.
	/// </summary>
	public float characterSpacing = 1f;

	/// <summary>
	/// The distance from one line to the next as a percentage
	/// of characterSize.  Defaults to a distance that is
	/// 1.1 times the height of a character. This has the effect
	/// of creating a gap between lines that is 10% the height
	/// of a line.
	/// </summary>
	public float lineSpacing = 1.1f;

	// Actual world size of a line, calculated from
	// lineSpaceing and the world-size of a
	// full-height character.
	protected float lineSpaceSize;

	/// <summary>
	/// The position of the center of the GameObject relative
	/// to the text.
	/// </summary>
	public Anchor_Pos anchor = Anchor_Pos.Upper_Left;

	/// <summary>
	/// How the text is to be aligned.
	/// </summary>
	public Alignment_Type alignment;

	/// <summary>
	/// The number of space characters which are produced by
	/// a tab character.
	/// </summary>
	public int tabSize = 4;
	protected string tabSpaces = "    ";

	/// <summary>
	/// Text asset that defines the font to be used.
	/// </summary>
	public TextAsset font;

	/// <summary>
	/// The color to be used by all of the text's
	/// vertices.  This can be used to color, highlight,
	/// or fade the text. Be sure to use a vertex-colored
	/// shader for this to have an effect.
	/// </summary>
	public Color color = Color.white;			// The color to be used by all four vertices

	/// <summary>
	/// Automatically sizes the text so that it will 
	/// display pixel-perfect on-screen.
	/// NOTE: If you change the orthographic size of 
	/// the camera or the distance between the text 
	/// and a perspective camera, call SetCamera()
	/// to make the text pixel-perfect again.
	/// </summary>
	public bool pixelPerfect = false;			// Automatically sizes the text

	/// <summary>
	/// When set to a value over 0, if the multiline
	/// setting is true, then the content of the
	/// SpriteText object will be word-wrapped when
	/// a line reaches the specified maximum width
	/// otherwise the text displayed will be truncated 
	/// and "..." will be appended.
	/// NOTE: The actual text contents are preserved
	/// and only the display string is truncated.
	/// </summary>
	public float maxWidth = 0;

	/// <summary>
	/// When set to true, maxWidth is interpreted as the
	/// maximum number of screen pixels. When false, 
	/// maxWidth is interpreted as the max width in
	/// local space units.
	/// </summary>
	public bool maxWidthInPixels = false;

	/// <summary>
	/// When set to true, the text object will allow
	/// multi-line content.  This also enables
	/// word-wrapping when the maxWidth setting is
	/// set to a non-zero value.  If multiline is
	/// false, then a non-zero maxWidth setting will
	/// cause text that exceeds the maxWidth to be
	/// truncated and have "..." appended.
	/// NOTE: The actual text contents are preserved
	/// and only the display string is truncated.
	/// </summary>
	public bool multiline = true;

	/// <summary>
	/// When true, this setting tells the script that it 
	/// should optimize its behavior on the assumption that 
	/// its contents will change size frequently.
	/// When false, it will be optimized on the assumption
	/// that its length will remain the same for most, if
	/// not all, of the time.
	/// </summary>
	public bool dynamicLength = false;

	/// <summary>
	/// When true, any unsupported characters in the string
	/// assigned to this object at runtime will be removed 
	/// from the string.  NOTE: Using this on large amounts 
	/// of text may have an adverse impact on performance.
	/// </summary>
	public bool removeUnsupportedCharacters = true;

	/// <summary>
	/// If set to false, color tags will not be parsed out
	/// of the text.
	/// </summary>
	public bool parseColorTags = true;

	/// <summary>
	/// When set to true, all text in this control will
	/// be masked using the specified maskingCharacter.
	/// </summary>
	public bool password = false;

	/// <summary>
	/// Holds the character to be used to mask password
	/// text.  Defaults to asterisk (*).
	/// </summary>
	public string maskingCharacter = "*";

	// Reference to the attached EZScreenPlacement component, if any
	protected EZScreenPlacement screenPlacer;

	// Reference to a possible parent control
	IControl parentControl;

	protected bool clipped;						// Whether the text is being clipped by the clippingRect.
	protected bool updateClipping = false;		// Tells us whether the mesh's clipping needs to be updated.
	protected Rect3D clippingRect;				// Rect against which the text will be clipped.
	protected Rect localClipRect;				// Will hold clippingRect in local space.
	protected Vector3 topLeft, bottomRight;		// Hold the positions of the outer edges of the text
	protected Vector3 unclippedTL, unclippedBR;	// Hold the outer edges assuming no clipping.

	// Color-related vars:
	protected Color[] colors = new Color[0];	// An array of colors, each element of which, corresponds to a character in our mesh.
	protected bool updateColors = false;		// Indicates when the colors of the mesh need to be updated
	
	/// <summary>
	/// Character that must be placed on either side of a color value
	/// in a string to indicate a change of text color.
	/// </summary>
	[HideInInspector]
	public const string colorTag = "[#";
	protected static string[] colDel = { "RGBA(", colorTag, ")", "]" };// For convenience
	protected static char[] newLineDelimiter = { '\n' };
	protected static char[] commaDelimiter = { ',' };

	[HideInInspector]
	public bool isClone = false;				// Set this to true when the SpriteText object has been instantiated from another SpriteText in the scene.
	protected bool m_awake = false;
	protected bool m_started = false;			// Lets us detect whether Start() has yet been called.
	protected bool stringContentChanged = true;	// Gets set to true when a string value is assigned.


	// Vars that make pixel-perfect sizing and
	// automatic sizing work:
	protected Vector2 screenSize;				// The size of the screen in pixels
	public Camera renderCamera;
	[HideInInspector]
	public Vector2 pixelsPerUV;				// The number of pixels in both axes per UV unit
	protected float worldUnitsPerScreenPixel;	// The number of world units in both axes per screen pixel
	protected float worldUnitsPerTexel;		// The number of world units per font atlas texel.
	protected Vector2 worldUnitsPerUV;		// The number of world units per font atlas UV.


	// Start-up state vars:
	/// <summary>
	/// Whether the text will be hidden when it starts.
	/// </summary>
	public bool hideAtStart = false;

	// Will tell us if we INTEND for the sprite to be hidden,
	// so that if the mesh renderer happens to be incidentally
	// disabled, such as on a prefab that is uninstantiated,
	// we don't mistake that for being hidden.
	// We set this when we intentionally hide the sprite.
	protected bool m_hidden = false;

	/// <summary>
	/// This must be set to true at design time for the object to survive loading a new level.
	/// </summary>
	public bool persistent = false;

	/// <summary>
	/// When true, the text will not be clipped.
	/// </summary>
	public bool ignoreClipping = false;

	// How many characters our mesh is currently
	// prepared to hold.
	protected int capacity;

	// The string the mesh already contains.
	protected string meshString = "";
	// The string containing the undecorated text (text sans any color tags)
	protected string plainText = "";
	// The string actually being displayed (contains extra newlines, as needed, for word-wrapping)
	protected string displayString = "";
	// Places **in the plain text** where newlines have been inserted for word-wrapping purposes.
	protected List<NewlineInsertInfo> newLineInserts = new List<NewlineInsertInfo>();
	
	// Holds the current total width of the text (widest line, in local units).
	protected float totalWidth;

	// The SpriteFont itself
	protected SpriteFont spriteFont;

	// Used to help us update the object's 
	// appearance in the editor:
	protected SpriteTextMirror mirror = null;

	// Mesh stuff:
	protected Mesh oldMesh;
	protected Mesh mesh;
	protected MeshRenderer meshRenderer;
	protected MeshFilter meshFilter;
	protected Texture texture;
	protected Vector3[] vertices;
	protected int[] faces;
	protected Vector2[] UVs;
	protected Color[] meshColors;

	// Misc working vars:
	StringBuilder displaySB = new StringBuilder();
	StringBuilder plainSB = new StringBuilder();
	List<int> colorInserts = new List<int>();
	List<int> colorTags = new List<int>();
	List<Color> cols = new List<Color>();
	string[] lines;



	protected virtual void Awake()
	{
		if (m_awake)
			return;
		m_awake = true;

		// Determine if we are a clone:
		if (name.EndsWith("(Clone)"))
			isClone = true;

		meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
		meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));

		oldMesh = meshFilter.sharedMesh;
		meshFilter.sharedMesh = null;

		Init();
	}


	public virtual void Start()
	{
		if (m_started)
			return;
		m_started = true;

		// Free the default sharedMesh:
		if (!isClone && Application.isPlaying)
		{
			Destroy(oldMesh);
			oldMesh = null;
		}

		if (renderCamera == null)
		{
			if (UIManager.Exists() && UIManager.instance.uiCameras.Length > 0)
			{
				renderCamera = UIManager.instance.uiCameras[0].camera;
			}
			else
				renderCamera = Camera.mainCamera;
		}

		SetCamera(renderCamera);

		ProcessString(text);

		// Force a mesh update:
		updateColors = true;
		UpdateMesh();
	}


	protected virtual void Init()
	{
		// Get our screen placer, if any:
		screenPlacer = (EZScreenPlacement)GetComponent(typeof(EZScreenPlacement));

		//if (!Application.isPlaying)
		{
			if (screenPlacer != null)
				screenPlacer.SetCamera(renderCamera);
		}

		// Get a default font:
		if(font == null && UIManager.Exists())
			font = UIManager.instance.defaultFont;

		if (meshRenderer.sharedMaterial == null && UIManager.Exists())
		{
			meshRenderer.sharedMaterial = UIManager.instance.defaultFontMaterial;
			if (meshRenderer.sharedMaterial != null)
				texture = meshRenderer.sharedMaterial.mainTexture;
		}
		else
		{
			if (meshRenderer.sharedMaterial != null)
				texture = meshRenderer.sharedMaterial.mainTexture;
		}

#if WARN_ON_NO_MATERIAL
		if (texture == null && Application.isPlaying)
			Debug.LogWarning("Text on GameObject \"" + name + "\" has not been assigned either a texture or a material.");
#endif


		// Get the font:
		if (font != null)
		{
			spriteFont = FontStore.GetFont(font);

			if (spriteFont == null)
				Debug.LogWarning("Warning: " + name + " was unable to load font \"" + font.name + "\"!");
		}
		else if(Application.isPlaying)
			Debug.LogWarning("Warning: " + name + " currently has no font assigned.");

		if (mesh == null)
		{
			CreateMesh();
		}

		// If this mesh is to persist, prevent it from being
		// destroyed on load:
		if (persistent)
		{
			Persistent = true;
		}

		if (texture != null)
			SetPixelToUV(texture);

		// Build our tab expansion string:
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < tabSize; ++i)
		{
			sb.Append(' ');
		}
		tabSpaces = sb.ToString();
	}


	// Creates a new mesh object, usually because
	// our previous one was destroyed.
	protected void CreateMesh()
	{
		if (meshFilter == null)
			return;
		meshFilter.sharedMesh = new Mesh();
		mesh = meshFilter.sharedMesh;
		if (persistent)
			GameObject.DontDestroyOnLoad(mesh);
	}


	// Processes a string, breaking out the color tags
	// from the text, and assembling the text into a
	// the "text" member, and writing color values to
	// the colors array.
	protected void ProcessString(string str)
	{
		colorInserts.Clear(); // Where the color tags should be inserted in the plain text
		colorTags.Clear(); // Where the color tags are in the original input string
		cols.Clear(); // Holds the value indicated by the color tags
		newLineInserts.Clear();
		//string[] parts = str.Split(colDel, System.StringSplitOptions.RemoveEmptyEntries);
		int strOffset = 0;
		int numChars;
		int tagIdx;
		int lastWhiteSpace = -1;
		int lastPlainWhiteSpace = -1; // Last whitespace in the plain text being input
		float spaceLeft = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;

/*
		int textPartIndex;
		int colorPartIndex;
		int insertIndex;
*/
		Color tempColor;

		text = str;

		if(System.String.IsNullOrEmpty(str) || spriteFont == null)
		{
			plainText = "";
			displayString = "";
			return;
		}

/*
		// Figure out if the first part is a color tag or not:
		if (parts[0].StartsWith(colDel[0]))
		{
			textPartIndex = 1;
			colorPartIndex = 0;
			insertIndex = 1;
			colorInserts = new int[parts.Length / 2];
			colorInserts[0] = 0;
		}
		else if(parts[0].StartsWith(colDel[1]))
		{
			textPartIndex = 1;
			colorPartIndex = 0;
			insertIndex = 1;
			colorInserts = new int[parts.Length / 2];
			colorInserts[0] = 0;
		}
		else
		{
			textPartIndex = 0;
			colorPartIndex = 1;
			insertIndex = 0;
			colorInserts = new int[(parts.Length / 2) + 1];
		}
*/
		// Expand any tab characters:
		if (str.IndexOf('\t') != -1)
		{
			str = str.Replace("\t", tabSpaces);
		}

		if(parseColorTags)
		{
			// Locate the color tags:
			tagIdx = 0;
			do
			{
				tagIdx = str.IndexOf(colDel[0], tagIdx);
				if (tagIdx != -1)
				{
					colorTags.Add(tagIdx);
					++tagIdx;
				}
			} while (tagIdx != -1);

			tagIdx = 0;
			do
			{
				tagIdx = str.IndexOf(colDel[1], tagIdx);
				if (tagIdx != -1)
				{
					colorTags.Add(tagIdx);
					++tagIdx;
				}
			} while (tagIdx != -1);
		}

		
		// Make sure we have a masking character:
		if (maskingCharacter.Length < 1)
			maskingCharacter = "*";


		// If there were no tags, just set the text:
		if(colorTags.Count < 1)
		{
			plainText = str;
			displaySB.Remove(0, displaySB.Length);

			// See if we're word-wrapping:
			if(maxWidth > 0 && multiline)
			{
				spaceLeft = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;

				for(int i=0; i<str.Length; ++i)
				{
					if (char.IsWhiteSpace(str[i]))
					{
						// If it's a newline, reset our space left:
						if (str[i] == '\n')
							spaceLeft = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;

						lastWhiteSpace = displaySB.Length;
						lastPlainWhiteSpace = i;
					}

					if (password && str[i] != '\n')
						displaySB.Append(maskingCharacter[0]);
					else
						displaySB.Append(str[i]);

					if (i != lastPlainWhiteSpace+1 && i > 0)
						spaceLeft -= spriteFont.GetWidth(str[i - 1], str[i]) * worldUnitsPerTexel * characterSpacing;
					else
						spaceLeft -= spriteFont.GetAdvance(str[i]) * worldUnitsPerTexel * characterSpacing;

					if (spaceLeft < 0)
					{
						if (i == lastWhiteSpace)
							spaceLeft = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;
						else // The space left is what remains after our new linebreak:
							spaceLeft = ((maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth) -spriteFont.GetWidth(displaySB, lastWhiteSpace + 1, i) * worldUnitsPerTexel * characterSpacing;

						// If there still isn't any space left, the word itself is
						// too long, so clip it at our width:
						if(spaceLeft < 0)
						{
							// Insert a line break before the most recent character
							// which put us over the top:
							if (displaySB.Length > 0)
							{
								displaySB.Insert(displaySB.Length - 1, '\n');
								spaceLeft = ((maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth) - spriteFont.GetAdvance(str[i]) * worldUnitsPerTexel * characterSpacing;
								newLineInserts.Add(new NewlineInsertInfo(i, 1));
							}
						}
						else
						{
							// Add our carriage return at the last whitespace:
							if (lastWhiteSpace >= 0)
							{
								displaySB[lastWhiteSpace] = '\n';
								newLineInserts.Add(new NewlineInsertInfo(lastPlainWhiteSpace, 0));
							}
						}
					}
				}

				displayString = displaySB.ToString();
			}
			else
			{
				if (password)
				{
					displaySB.Remove(0, displaySB.Length);
					for (int i = 0; i < str.Length; ++i)
						displaySB.Append(maskingCharacter[0]);

					displayString = displaySB.ToString();
				}
				else
				{
					displaySB.Append(str);
					displayString = str;
				}
			}

			// See if we should truncate the string:
			if (!multiline)
			{
				DoSingleLineTruncation();
				displayString = displaySB.ToString();
			}


			if (colors.Length < displayString.Length)
			{
				colors = new Color[displayString.Length];
			}
			for (int i = 0; i < colors.Length; ++i)
				colors[i] = color;

			updateColors = true;

			return;
		}


		// Sort the order of the tags:
		colorTags.Sort();

		// Append a terminating value:
		colorTags.Add(-1);


		plainSB.Remove(0, plainSB.Length);
		displaySB.Remove(0, displaySB.Length);
		tagIdx = 0;
		while (strOffset < str.Length)
		{
			// If this is a color tag:
			if(strOffset == colorTags[tagIdx])
			{
				if (0 == System.String.Compare(str, strOffset, colDel[0], 0, colDel[0].Length))
				{
					strOffset += colDel[0].Length;

					// Find the end of the tag:
					numChars = str.IndexOf(')', colorTags[tagIdx]) - strOffset;
					if(numChars < 0)
						numChars = str.Length - strOffset;

					// Add a color insertion point:
					colorInserts.Add(displaySB.Length);
					cols.Add(ParseColor(str.Substring(strOffset, numChars)));
					strOffset += numChars + 1;
				}
				else if (0 == System.String.Compare(str, strOffset, colDel[1], 0, colDel[1].Length))
				{
					strOffset += colDel[1].Length;

					// Find the end of the tag:
					numChars = str.IndexOf(']', colorTags[tagIdx]) - strOffset;
					if (numChars < 0)
						numChars = str.Length - strOffset;

					// Add a color insertion point:
					colorInserts.Add(displaySB.Length);
					cols.Add(ParseHexColor(str.Substring(strOffset, numChars)));
					strOffset += numChars + 1;
				}

				// Move to the next tag
				++tagIdx;
			}
			else // Else it's just another character:
			{
				// See if we're word-wrapping:
				if(maxWidth > 0 && multiline)
				{
					if (char.IsWhiteSpace(str[strOffset]))
					{
						// If it's a newline, reset our space left:
						if (str[strOffset] == '\n')
							spaceLeft = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;

						lastWhiteSpace = displaySB.Length;
						lastPlainWhiteSpace = strOffset;
					}

					if (password && str[strOffset] != '\n')
						displaySB.Append(maskingCharacter[0]);
					else
						displaySB.Append(str[strOffset]);

					plainSB.Append(str[strOffset]);

					if (strOffset != lastPlainWhiteSpace + 1 && strOffset > 0)
						spaceLeft -= spriteFont.GetWidth(str[strOffset - 1], str[strOffset]) * worldUnitsPerTexel * characterSpacing;
					else
						spaceLeft -= spriteFont.GetAdvance(str[strOffset]) * worldUnitsPerTexel * characterSpacing;

					if (spaceLeft < 0)
					{
						// Go ahead and reset the space left as we'll be inserting
						// a newline somewhere one way or another:
						if (strOffset == lastPlainWhiteSpace)
							spaceLeft = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;
						else // The space left is what remains after our new linebreak:
							spaceLeft = ((maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth) - spriteFont.GetWidth(displaySB, lastWhiteSpace + 1, strOffset) * worldUnitsPerTexel * characterSpacing;

						// If there still isn't any space left, the word itself is
						// too long, so clip it at our width:
						if (spaceLeft < 0)
						{
							// Insert a line break before the most recent character
							// which put us over the top:
							if (displaySB.Length > 0)
							{
								displaySB.Insert(displaySB.Length - 1, '\n');
								spaceLeft = ((maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth) - spriteFont.GetAdvance(str[strOffset]) * worldUnitsPerTexel * characterSpacing;

								newLineInserts.Add(new NewlineInsertInfo(plainSB.Length - 1, 1));

								// See if we need to update the insertion point of
								// our previous colors since we added a character:
								int insIdx = colorInserts.Count - 1;

								// Scan backward and increment each entry that follows our newline insertion
								while (insIdx >= 0 && colorInserts[insIdx] > newLineInserts[newLineInserts.Count-1].index)
								{
									++colorInserts[insIdx];
									--insIdx;
								}
							}
						}
						else
						{
							// Add our carriage return at the last whitespace:
							if (lastWhiteSpace >= 0)
							{
								displaySB[lastWhiteSpace] = '\n';
								newLineInserts.Add(new NewlineInsertInfo(lastPlainWhiteSpace, 0));
							}
						}
					}
					
					++strOffset;
				}
				else // Else add the characters between here and the next color tag:
				{
					if (colorTags[tagIdx] == -1)
						numChars = str.Length - strOffset;
					else
						numChars = colorTags[tagIdx] - strOffset;

					plainSB.Append(str, strOffset, numChars);

					if (password)
					{
						for (int i = strOffset; i < strOffset+numChars; ++i)
							if (spriteFont.ContainsCharacter(str[i]))
								displaySB.Append(maskingCharacter[0]);
					}
					else
						displaySB.Append(str, strOffset, numChars);

					strOffset += numChars;
				}
			}
		}

		if (colorInserts.Count == 0)
		{
			colorInserts.Add(0);
			cols.Add(color);
		}

		// See if we need to truncate in single-line mode:
		if (!multiline)
			DoSingleLineTruncation();

		// Append every other part, as the text and color parts
		// are interleaved (or at least should be)
/*
		for (; textPartIndex < parts.Length; textPartIndex += 2, ++insertIndex)
		{
			plainSB.Append(parts[textPartIndex]);
			charCount += parts[textPartIndex].Length;

			if (insertIndex < colorInserts.Length)
				colorInserts[insertIndex] = charCount;
		}
*/

		plainText = plainSB.ToString();
		displayString = displaySB.ToString();

		// Now re-allocate our colors buffer:
		if (colors.Length < displayString.Length)
			colors = new Color[displayString.Length];

/*
		// Get our colors:
		cols = new Color[colorInserts.Length];
		for (int i = 0; colorPartIndex < parts.Length; colorPartIndex += 2, ++i)
		{
			cols[i] = ParseColor(parts[colorPartIndex]);
		}
*/

		// Now fill in our colors:
		tempColor = color; // Use our global color by default
		for (int i = 0, j = 0; i < displayString.Length; ++i)
		{
			// If this is the place in the string where we
			// insert a new  color:
			if(i == colorInserts[j])
			{
				tempColor = cols[j];
				j = (j + 1) % colorInserts.Count;
			}

			colors[i] = tempColor;
		}

		updateColors = true;
	}

	protected void DoSingleLineTruncation()
	{
		int linebreakIdx = displayString.IndexOf('\n');

		// Cut it off at any newline:
		if (linebreakIdx >= 0)
		{
			displaySB.Remove(linebreakIdx, displaySB.Length - linebreakIdx);
			displaySB.Append("...");
		}

		// See if we also need to check the width:
		if (maxWidth > 0)
		{
			float width = spriteFont.GetWidth(displaySB, 0, displaySB.Length - 1) * worldUnitsPerTexel * characterSpacing;
			float maximumWidth = (maxWidthInPixels) ? (maxWidth * worldUnitsPerScreenPixel) : maxWidth;

			if (width > maximumWidth)
			{
				int clipCount = 0;
				float ellipsisWidth = spriteFont.GetWidth("...") * worldUnitsPerTexel * characterSpacing;

				// Find out how many characters we need to clip:
				do
				{
					++clipCount;
					width = spriteFont.GetWidth(displaySB, 0, displaySB.Length - 1 - clipCount) * worldUnitsPerTexel * characterSpacing;
				}
				while (width + ellipsisWidth > maximumWidth && width != 0);

				// Remove the requisite characters to make room for our ellipsis:
				clipCount = Mathf.Clamp(clipCount, 0, displaySB.Length);
				displaySB.Remove(displaySB.Length - clipCount, clipCount);
				displaySB.Append("...");
			}
		}

		if (password)
		{
			for (int i = 0; i < displaySB.Length; ++i)
				displaySB[i] = maskingCharacter[0];
		}
	}


	// Parses the contents of a color tag to generate
	// a Color value
	protected Color ParseColor(string str)
	{
		string[] parts = str.Split(commaDelimiter);

		if (parts.Length != 4)
			return color;

		return color * new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
	}

	// Parses the contents of a hex color tag to
	// generate a Color value.
	protected Color ParseHexColor(string str)
	{
		if(str.Length < 6)
			return color;

		try
		{
			int red = (System.Int32.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			int green = (System.Int32.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			int blue = (System.Int32.Parse(str.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier));

			int alpha = 255;

			if (str.Length == 8)
				alpha = (System.Int32.Parse(str.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier));

			return color * new Color(((float)red) / 255f, ((float)green) / 255f, ((float)blue) / 255f, ((float)alpha) / 255f);
		}
		catch
		{
			return color;
		}
	}

	// Enlarges the number of characters
	// the mesh can hold.
	protected void EnlargeMesh()
	{
		vertices = new Vector3[displayString.Length * 4];
		UVs = new Vector2[displayString.Length * 4];
		meshColors = new Color[displayString.Length * 4];
		faces = new int[displayString.Length * 6];

		// Setup our faces:
		for (int i = 0; i < displayString.Length; ++i)
		{
			// Clock-wise winding:
			faces[i * 6 + 0] = i * 4 + 0;	//	0_ 1			0 ___ 3
			faces[i * 6 + 1] = i * 4 + 3;	//  | /		Verts:	 |	/|
			faces[i * 6 + 2] = i * 4 + 1;	// 2|/				1|/__|2

			faces[i * 6 + 3] = i * 4 + 3;	//	  3
			faces[i * 6 + 4] = i * 4 + 2;	//   /|
			faces[i * 6 + 5] = i * 4 + 1;	// 5/_|4
		}

		capacity = displayString.Length;
	}


	// Updates the mesh to reflect the new
	// string contents.
	public void UpdateMesh()
	{
		// In case our mesh was somehow destroyed:
		if (mesh == null)
		{
			CreateMesh();
		}

		if (spriteFont == null)
			return;

#if WANT_NORMALS
		bool vertCountChanged = false;
#endif

#if AVOID_SPLIT
		bool singleline = false;
#endif

		// Only do a quick equality check if
		// the strings involved are relatively
		// short:
		if(meshString.Length < 15 && !updateClipping && !updateColors)
			if (stringContentChanged)
				if (meshString == displayString)
					return;

		// See if the string is being cleared:
		if (displayString.Length < 1)
		{
			ClearMesh();
		}
		else
		{
			// See if we need to enlarge the mesh:
			if (displayString.Length > capacity)
			{
				EnlargeMesh();
#if WANT_NORMALS
				vertCountChanged = true;
#endif
			}

			// Get the number of lines:
//			int newLineIndex = 0;

// 			while (newLineIndex != -1)
// 			{
// 				newLineIndex = text.IndexOf('\n', newLineIndex);
// 				++lines;
// 			}

			// Setup our temporary local-space clipping rect:
			if(clipped)
			{
				updateClipping = false;
				localClipRect = Rect3D.MultFast(clippingRect, transform.worldToLocalMatrix).GetRect();
			}

#if AVOID_SPLIT
            // See if our string content has changed:
            if ( stringContentChanged )
            {

				lines = null;
                int strindex = displayString.IndexOf('\n');     // will return -1 if not found, or 0 if string is empty, so check for that
                if ( strindex == -1)
                {
                    singleline = true;
                }
                else
                {
                    lines = displayString.Split(newLineDelimiter);
                    singleline = false;
                }
            }

	        // See if we have only a single line:
			if ( singleline == true || lines == null || lines.Length == 1 )
#else
			// See if our string content has changed:
			if(stringContentChanged)
				lines = displayString.Split(newLineDelimiter);

             // See if we have only a single line:
             if (lines.Length == 1)
#endif
			{
				Layout_Single_Line();
			}
			else
			{
				Layout_Multiline(lines);
			}

			unclippedTL = topLeft;
			unclippedBR = bottomRight;

			// Clip our edges if we're clipped:
			if (clipped)
			{
				topLeft.x = Mathf.Max(localClipRect.x, topLeft.x);
				topLeft.y = Mathf.Min(localClipRect.yMax, topLeft.y);
				bottomRight.x = Mathf.Min(localClipRect.xMax, bottomRight.x);
				bottomRight.y = Mathf.Max(localClipRect.y, bottomRight.y);
			}
		}

		stringContentChanged = false;




		meshString = displayString;

		if(vertCountChanged)
			mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = UVs;
		mesh.colors = meshColors;
		mesh.triangles = faces;
#if WANT_NORMALS
		if(vertCountChanged)
			mesh.RecalculateNormals();
#endif
		mesh.RecalculateBounds();

		// Inform our control it needs to update itself:
		if(parentControl != null)
		{
			if(parentControl is AutoSpriteControlBase)
			{
				if(((AutoSpriteControlBase)parentControl).includeTextInAutoCollider)
					((AutoSpriteControlBase)parentControl).UpdateCollider();
				//if (((AutoSpriteControlBase)parentControl).Clipped)
					((AutoSpriteControlBase)parentControl).FindOuterEdges();
			}
			else if (parentControl is ControlBase)
			{
				if (((ControlBase)parentControl).includeTextInAutoCollider)
					((ControlBase)parentControl).UpdateCollider();
			}
		}
	}

	// Returns the start point of a line, in local space, given the current
	// anchor, as well as the base height and total width of the string.
	protected Vector3 GetStartPos_SingleLine(float baseHeight, float width)
	{
		switch (anchor)
		{
			case Anchor_Pos.Upper_Left:
				return new Vector3(0, 0, offsetZ);
			case Anchor_Pos.Upper_Center:
				return new Vector3(width * -0.5f, 0, offsetZ);
			case Anchor_Pos.Upper_Right:
				return new Vector3(-width, 0, offsetZ);
			case Anchor_Pos.Middle_Left:
				return new Vector3(0, baseHeight * 0.5f, offsetZ);
			case Anchor_Pos.Middle_Center:
				return new Vector3(width * -0.5f, baseHeight * 0.5f, offsetZ);
			case Anchor_Pos.Middle_Right:
				return new Vector3(-width, baseHeight * 0.5f, offsetZ);
			case Anchor_Pos.Lower_Left:
				return new Vector3(0, baseHeight, offsetZ);
			case Anchor_Pos.Lower_Center:
				return new Vector3(width * -0.5f, baseHeight, offsetZ);
			case Anchor_Pos.Lower_Right:
				return new Vector3(-width, baseHeight, offsetZ);
			default:
				return new Vector3(0, 0, offsetZ);
		}
	}

	/// <summary>
	/// Returns the number of lines actually being displayed,
	/// as opposed to the number of lines in the string assigned
	/// to the object.  In other words, if word-wrapping is being
	/// used, the number of actual lines displayed may be more
	/// than the number of lines in the actual string.
	/// It also stores the line number of the character at the
	/// specified index in the string.
	/// </summary>
	/// <param name="charIndex">The index of the character sought.</param>
	/// <param name="charLine">OUT: The 1-based line number on which the character 
	/// at charIndex sits.</param>
	/// <param name="lineStart">OUT: The index of the first character on the same line as the character at the specified index.</param>
	/// <param name="lineEnd">OUT: The index of the last character on the same line as the character at the specified index (excludes ending newline character).</param>
	/// <returns>The number of lines actually being displayed.</returns>
	public int GetDisplayLineCount(int charIndex, out int charLine, out int lineStart, out int lineEnd)
	{
		int count = 1;
		int ci = 0;		// Our character index counter
		charLine = -1;
// 		int lastNewline = -1;
 		int curNewLine = -1;
		lineStart = 0;
		lineEnd = -1;

		for (int i = 0; i < displayString.Length; ++i)
		{
			if (displayString[i] == '\n')
			{
				// See if this is the end of
				// the character's line:
				if (count == charLine)
					lineEnd = Mathf.Max(0, i - 1);

// 				lastNewline = curNewLine;
 				curNewLine = i;
//				lineStart = i + 1;
				++count;
			}

			if (ci == charIndex)
			{
				charLine = count;
				lineStart = curNewLine + 1;

				// If our index is on a newline, use the
				// start position of the previous line:
// 				if (ci == curNewLine)
// 				{
// 					lineStart = lastNewline + 1;
// 					charLine -= 1;
// 					charLine = Mathf.Max(0, charLine);
// 				}
// 				else
// 					lineStart = curNewLine + 1;
			}

			++ci;
		}

		// See if we ever set lineEnd:
		if (lineEnd < 0)
			lineEnd = displayString.Length - 1;
		if (charLine < 0)
		{
			charLine = count;
			lineStart = Mathf.Min(displayString.Length-1, curNewLine + 1);
		}

		return count;
	}

	/// <summary>
	/// Returns the number of lines actually being displayed,
	/// as opposed to the number of lines in the string assigned
	/// to the object.  In other words, if word-wrapping is being
	/// used, the number of actual lines displayed may be more
	/// than the number of lines in the actual string.
	/// </summary>
	/// <returns>The number of lines actually being displayed.
	/// NOTE: This value will never be less than 1.</returns>
	public int GetDisplayLineCount()
	{
		int count = 1;
		for (int i = 0; i < displayString.Length; ++i)
			if (displayString[i] == '\n')
				++count;

		return count;
	}

	/// <summary>
	/// Converts a character index in the plain text string
	/// to a corresponding index in the display string.
	/// </summary>
	/// <param name="plainCharIndex">The index in the plain string to be converted.</param>
	/// <returns>The index in the display string that corresponds to the provided index in the plain text string.</returns>
	public int PlainIndexToDisplayIndex(int plainCharIndex)
	{
		int dispIdx = plainCharIndex;

		for (int i = 0; i < newLineInserts.Count; ++i)
		{
			if (newLineInserts[i].index <= plainCharIndex)
				dispIdx += newLineInserts[i].charDelta;
			else
				break;
		}

		return dispIdx;
	}

	/// <summary>
	/// Converts a character index in the display text string
	/// to a corresponding index in the plain text string.
	/// </summary>
	/// <param name="plainCharIndex">The index in the display string to be converted.</param>
	/// <returns>The index in the plain text string that corresponds to the provided index in the display text string.</returns>
	public int DisplayIndexToPlainIndex(int dispCharIndex)
	{
		int plainIdx = dispCharIndex;

		for (int i = 0; i < newLineInserts.Count; ++i)
		{
			if (newLineInserts[i].index <= dispCharIndex)
				plainIdx -= newLineInserts[i].charDelta;
			else
				break;
		}

		return plainIdx;
	}


	// Returns the Y coordinate of the baseline of the
	// specified line, taking the anchor into account.
	// lineNum is the 1-based number of the desired line.
	// So the first (top-most) line of a string is 1.
	protected float GetLineBaseline(int numLines, int lineNum)
	{
		// Distance from the absolute top of a line to the baseline, in world units:
		float lineHeight = spriteFont.BaseHeight * worldUnitsPerTexel;
		float belowToBase = lineSpaceSize-lineHeight;
		float distBetweenLines = lineSpaceSize - characterSize;
		float totalHeight = characterSize * ((float)numLines) + distBetweenLines * ((float)numLines-1);

		switch (anchor)
		{
			case Anchor_Pos.Upper_Left:
			case Anchor_Pos.Upper_Center:
			case Anchor_Pos.Upper_Right:
				return ((float)lineNum) * (-lineSpaceSize) + belowToBase;
			case Anchor_Pos.Middle_Left:
			case Anchor_Pos.Middle_Center:
			case Anchor_Pos.Middle_Right:
				return totalHeight * 0.5f + ((float)lineNum) * (-lineSpaceSize) + belowToBase;
			case Anchor_Pos.Lower_Left:
			case Anchor_Pos.Lower_Center:
			case Anchor_Pos.Lower_Right:
				return totalHeight + ((float)lineNum) * (-lineSpaceSize) + belowToBase;
		}

		return 0;
	}

	// Lays out a single-line string.
	protected void Layout_Single_Line()
	{
		if (spriteFont == null)
			return;

		// startPos is the position of the
		// top and left of the absolute top
		// of the line.
		Vector3 startPos = Vector3.zero;
		float baseHeight = ((float)spriteFont.PixelSize) * worldUnitsPerTexel;
		float width = spriteFont.GetWidth(displayString) * worldUnitsPerTexel * characterSpacing;

		totalWidth = width;

		startPos = GetStartPos_SingleLine(baseHeight, width);

		topLeft = startPos;
		bottomRight = new Vector3(topLeft.x + width, topLeft.y - baseHeight, 0);

		Layout_Line(startPos, displayString, 0);

		// See if we have any remaining capacity we need to
		// make disappear:
		if (displayString.Length < capacity)
		{
			for (int i = displayString.Length; i < capacity; ++i)
			{
				vertices[i * 4] = Vector3.zero;
				vertices[i * 4 + 1] = Vector3.zero;
				vertices[i * 4 + 2] = Vector3.zero;
				vertices[i * 4 + 3] = Vector3.zero;
			}
		}
	}

	// Lays out each line of a multiple line string.
	protected void Layout_Multiline(string[] lines)
	{
		float[] widths = new float[lines.Length];
		float totalHeight;
		float largestWidth = 0;
		Vector3 startPos = Vector3.zero;
		int charIndex = 0;

		// Find the total height of the text:
		float distBetweenLines = lineSpaceSize - characterSize;
		totalHeight = characterSize * ((float)lines.Length) + distBetweenLines * ((float)lines.Length - 1);

		// Find the width of each line:
		for (int i = 0; i < lines.Length; ++i)
		{
			widths[i] = spriteFont.GetWidth(lines[i]) * worldUnitsPerTexel * characterSpacing;

			// Find the longest line:
			if (largestWidth < widths[i])
				largestWidth = widths[i];
		}

		totalWidth = largestWidth;

		// Find the starting position
		switch (anchor)
		{
			case Anchor_Pos.Upper_Left:
				startPos = new Vector3(0, 0, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(largestWidth, -totalHeight, offsetZ);
				break;
			case Anchor_Pos.Upper_Center:
				startPos = new Vector3(largestWidth * -0.5f, 0, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(largestWidth * 0.5f, -totalHeight, offsetZ);
				break;
			case Anchor_Pos.Upper_Right:
				startPos = new Vector3(-largestWidth, 0, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(0, -totalHeight, offsetZ);
				break;
			case Anchor_Pos.Middle_Left:
				startPos = new Vector3(0, totalHeight * 0.5f, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(largestWidth, totalHeight * -0.5f, offsetZ);
				break;
			case Anchor_Pos.Middle_Center:
				startPos = new Vector3(largestWidth * -0.5f, totalHeight * 0.5f, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(largestWidth * 0.5f, totalHeight * -0.5f, offsetZ);
				break;
			case Anchor_Pos.Middle_Right:
				startPos = new Vector3(-largestWidth, totalHeight * 0.5f, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(0, totalHeight * -0.5f, offsetZ);
				break;
			case Anchor_Pos.Lower_Left:
				startPos = new Vector3(0, totalHeight, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(largestWidth, 0, offsetZ);
				break;
			case Anchor_Pos.Lower_Center:
				startPos = new Vector3(largestWidth * -0.5f, totalHeight, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(largestWidth * 0.5f, 0, offsetZ);
				break;
			case Anchor_Pos.Lower_Right:
				startPos = new Vector3(-largestWidth, totalHeight, offsetZ);
				topLeft = startPos;
				bottomRight = new Vector3(0, 0, offsetZ);
				break;
		}

		switch (alignment)
		{
			case Alignment_Type.Left:
				for (int i = 0; i < lines.Length; ++i)
				{
					Layout_Line(startPos, lines[i], charIndex);
					charIndex += lines[i].Length + 1; // Add one for the newline character at the end
					// Zero out the vertices corresponding to the newline:
					ZeroQuad(charIndex - 1);
					startPos.y -= lineSpaceSize;
				}
				break;
			case Alignment_Type.Center:
				for (int i = 0; i < lines.Length; ++i)
				{
					Layout_Line(startPos + (Vector3.right * 0.5f * (largestWidth - widths[i])), lines[i], charIndex);
					charIndex += lines[i].Length + 1; // Add one for the newline character at the end
					// Zero out the vertices corresponding to the newline:
					ZeroQuad(charIndex - 1);
					startPos.y -= lineSpaceSize;
				}
				break;
			case Alignment_Type.Right:
				for (int i = 0; i < lines.Length; ++i)
				{
					Layout_Line(startPos + (Vector3.right * (largestWidth - widths[i])), lines[i], charIndex);
					charIndex += lines[i].Length + 1; // Add one for the newline character at the end
					// Zero out the vertices corresponding to the newline:
					ZeroQuad(charIndex - 1);
					startPos.y -= lineSpaceSize;
				}
				break;
		}

		// See if we have any remaining capacity we need to
		// make disappear:
		if (charIndex < capacity)
		{
			for (int i = charIndex; i < capacity; ++i)
			{
				vertices[i * 4] = Vector3.zero;
				vertices[i * 4 + 1] = Vector3.zero;
				vertices[i * 4 + 2] = Vector3.zero;
				vertices[i * 4 + 3] = Vector3.zero;
			}
		}
	}

	protected void ZeroQuad(int i)
	{
		i = i*4;
		if (i >= vertices.Length)
			return;
		vertices[i + 0] = vertices[i + 1] = vertices[i + 2] = vertices[i + 3] = Vector3.zero;
	}

	// Sets up the vertices, etc,
	// of a character.
	protected void BuildCharacter(int vertNum, int charNum, Vector3 upperLeft, ref SpriteChar ch)
	{
		// 			0 ___ 3
		// Verts:	 |	/|
		// 			1|/__|2
		vertices[vertNum] = upperLeft;

		vertices[vertNum + 1].x = upperLeft.x;
		vertices[vertNum + 1].y = upperLeft.y - ch.UVs.height * worldUnitsPerUV.y;
		vertices[vertNum + 1].z = upperLeft.z;

		vertices[vertNum + 2] = vertices[vertNum + 1];
		vertices[vertNum + 2].x += ch.UVs.width * worldUnitsPerUV.x;

		vertices[vertNum + 3] = vertices[vertNum + 2];
		vertices[vertNum + 3].y = vertices[vertNum].y;
		//------- UVs --------
		UVs[vertNum].x = ch.UVs.x;
		UVs[vertNum].y = ch.UVs.yMax;

		UVs[vertNum + 1].x = ch.UVs.x;
		UVs[vertNum + 1].y = ch.UVs.y;
		
		UVs[vertNum + 2].x = ch.UVs.xMax;
		UVs[vertNum + 2].y = ch.UVs.y;
		
		UVs[vertNum + 3].x = ch.UVs.xMax;
		UVs[vertNum + 3].y = ch.UVs.yMax;
		//------- Colors --------
		meshColors[vertNum] = colors[charNum];
		meshColors[vertNum + 1] = colors[charNum];
		meshColors[vertNum + 2] = colors[charNum];
		meshColors[vertNum + 3] = colors[charNum];


		// Clip the character:
		if (clipped)
		{
			// Clip the character horizontally:
			if (vertices[vertNum].x < localClipRect.x)
			{
				if (vertices[vertNum + 2].x < localClipRect.x)
				{
					// Clip the whole character:
					vertices[vertNum].x = vertices[vertNum + 1].x = vertices[vertNum + 2].x;
					return;
				}
				else
				{
					// Trim the character:
					float clipPct = (localClipRect.x - vertices[vertNum].x) / (vertices[vertNum + 2].x - vertices[vertNum].x);
					vertices[vertNum].x = vertices[vertNum + 1].x = localClipRect.x;
					UVs[vertNum].x = UVs[vertNum + 1].x = Mathf.Lerp(UVs[vertNum].x, UVs[vertNum + 2].x, clipPct);
				}
			}
			else if (vertices[vertNum + 2].x > localClipRect.xMax)
			{
				if (vertices[vertNum].x > localClipRect.xMax)
				{
					// Clip the whole character:
					vertices[vertNum + 2].x = vertices[vertNum + 3].x = vertices[vertNum].x;
					return;
				}
				else
				{
					// Trim the character:
					float clipPct = (localClipRect.xMax - vertices[vertNum].x) / (vertices[vertNum + 2].x - vertices[vertNum].x);
					vertices[vertNum + 2].x = vertices[vertNum + 3].x = localClipRect.xMax;
					UVs[vertNum + 2].x = UVs[vertNum + 3].x = Mathf.Lerp(UVs[vertNum].x, UVs[vertNum + 2].x, clipPct);
				}
			}

			// Clip the character vertically:
			if (vertices[vertNum].y > localClipRect.yMax)
			{
				if (vertices[vertNum + 2].y > localClipRect.yMax)
				{
					// Clip the whole character:
					vertices[vertNum].y = vertices[vertNum + 3].y = vertices[vertNum + 2].y;
					return;
				}
				else
				{
					// Trim the character:
					float clipPct = (vertices[vertNum].y - localClipRect.yMax) / (vertices[vertNum].y - vertices[vertNum + 1].y);
					vertices[vertNum].y = vertices[vertNum + 3].y = localClipRect.yMax;
					UVs[vertNum].y = UVs[vertNum + 3].y = Mathf.Lerp(UVs[vertNum].y, UVs[vertNum + 1].y, clipPct);
				}
			}
			else if (vertices[vertNum + 2].y < localClipRect.y)
			{
				if (vertices[vertNum].y < localClipRect.y)
				{
					// Clip the whole character:
					vertices[vertNum + 1].y = vertices[vertNum + 2].y = vertices[vertNum].y;
					return;
				}
				else
				{
					// Trim the character:
					float clipPct = (vertices[vertNum].y - localClipRect.y) / (vertices[vertNum].y - vertices[vertNum + 1].y);
					vertices[vertNum + 1].y = vertices[vertNum + 2].y = localClipRect.y;
					UVs[vertNum + 1].y = UVs[vertNum + 2].y = Mathf.Lerp(UVs[vertNum].y, UVs[vertNum + 1].y, clipPct);
				}
			}
		}
	}

	// Lays out a single line of text
	// from the specified starting point.
	// startPos - The starting position of the line in world space.
	// txt - The string of the line to be laid out.
	// charIdx - the index of the character in the overall mesh.
	protected void Layout_Line(Vector3 startPos, string txt, int charIdx)
	{
		SpriteChar ch;
		Vector3 pos;

		if (txt.Length == 0)
			return;

		// Do the first character:
		ch = spriteFont.GetSpriteChar(txt[0]);

		if (ch != null)
		{
			pos = startPos + new Vector3(ch.xOffset * worldUnitsPerTexel, ch.yOffset * worldUnitsPerTexel, 0);

			BuildCharacter(charIdx * 4, charIdx, pos, ref ch);
		}

		for (int i = 1; i < txt.Length; ++i)
		{
			if (ch != null)
				startPos.x += ch.xAdvance * worldUnitsPerTexel * characterSpacing;
			
			ch = spriteFont.GetSpriteChar(txt[i]);

			if (ch == null)
				continue;

			startPos.x += ch.GetKerning(txt[i - 1]) * worldUnitsPerTexel * characterSpacing;
			pos = startPos + new Vector3(ch.xOffset * worldUnitsPerTexel, ch.yOffset * worldUnitsPerTexel, 0);
			BuildCharacter((charIdx + i) * 4, charIdx + i, pos, ref ch);
		}
	}


	// Clears the textual contents of
	// the mesh.
	protected void ClearMesh()
	{
		if (vertices == null)
			EnlargeMesh();

		for(int i=0; i<vertices.Length; ++i)
		{
			vertices[i] = Vector3.zero;
			meshColors[i] = color;
		}

		topLeft = Vector3.zero;
		bottomRight = Vector3.zero;
		unclippedTL = Vector3.zero;
		unclippedBR = Vector3.zero;
	}

// 	/// <summary>
// 	/// Resets important values to defaults for reuse.
// 	/// </summary>
// 	public virtual void Clear()
// 	{
// 		SetColor(Color.white);
	// 	}


	/// <summary>
	/// Removes any clipping that is being applied to the
	/// text object.
	/// </summary>
	public void Unclip()
	{
		if (ignoreClipping)
			return;

		clipped = false;
		updateClipping = true;
		UpdateMesh();
	}


	/// <summary>
	/// Call Delete() before destroying 
	/// this component or the GameObject to which it is 
	/// attached. Memory leaks can ensue otherwise.
	/// </summary>
	public void Delete()
	{
		// Destroy our mesh:
		if (Application.isPlaying)
		{
			Destroy(mesh);
			mesh = null;
		}
	}

	void OnEnable()
	{
		if (parentControl == null)
			return;

		if (parentControl is AutoSpriteControlBase)
		{
			AutoSpriteControlBase c = (AutoSpriteControlBase)parentControl;
			Hide(c.IsHidden());
		}
// Below is commented out because apparently Unity, when doing a recursive
// activation, activates leaf nodes first, then walks up the
// hierarchy, meaning that the SpriteText would be enabled before
// its parent object, meaning the below test would always result
// in the SpriteText being deactivated, even though its parent
// is in the process of being activated.
//		else
//			gameObject.active = ((Component)parentControl).gameObject.active;
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying && EZAnimator.Exists())
		{
			EZAnimator.instance.Stop(gameObject);
			EZAnimator.instance.Stop(this);
		}
	}

	public virtual void OnDestroy()
	{
		Delete();
	}

	/// <summary>
	/// Duplicates the settings of another SpriteText object.
	/// </summary>
	/// <param name="s">The SpriteText object to be copied.</param>
	public virtual void Copy(SpriteText s)
	{
		offsetZ = s.offsetZ;
		characterSize = s.characterSize;
		lineSpacing = s.lineSpacing;
		lineSpaceSize = s.lineSpaceSize;
		anchor = s.anchor;
		alignment = s.alignment;
		tabSize = s.tabSize;
		multiline = s.multiline;
		maxWidth = s.maxWidth;
		removeUnsupportedCharacters = s.removeUnsupportedCharacters;
		parseColorTags = s.parseColorTags;
		password = s.password;
		maskingCharacter = s.maskingCharacter;
		ignoreClipping = s.ignoreClipping;

		texture = s.texture;
		SetPixelToUV(texture);
		
		font = s.font;
		spriteFont = FontStore.GetFont(font);

		lineSpaceSize = lineSpacing * spriteFont.LineHeight * worldUnitsPerTexel;
		
		color = s.color;
		text = s.text;
		pixelPerfect = s.pixelPerfect;
		dynamicLength = s.dynamicLength;
		SetCamera(s.renderCamera);

		Text = text;

		hideAtStart = s.hideAtStart;
		m_hidden = s.m_hidden;
		Hide(m_hidden);
	}


	public void CalcSize()
	{
		if (spriteFont == null)
			return;

		if(pixelPerfect)
		{
			characterSize = ((float)spriteFont.PixelSize) * worldUnitsPerScreenPixel;

			worldUnitsPerTexel = worldUnitsPerScreenPixel;

			worldUnitsPerUV.x = worldUnitsPerTexel * pixelsPerUV.x;
			worldUnitsPerUV.y = worldUnitsPerTexel * pixelsPerUV.y;
		}

		lineSpaceSize = lineSpacing * spriteFont.LineHeight * worldUnitsPerTexel;

		UpdateMesh();
	}

	// Re-lays out text when something has changed that may
	// affect the text's layout.
	protected void LayoutText()
	{
		stringContentChanged = true;

		ProcessString(text);
		UpdateMesh();
	}


	/// <summary>
	/// Sets the text's color to the specified color.
	/// NOTE: Will not override color tags in the text
	/// itself.
	/// </summary>
	/// <param name="c">Color to shade the text.</param>
	public void SetColor(Color c)
	{
		color = c;

		updateColors = true;
		Text = text;
	}

	/// <summary>
	/// Accessor for the object's current overall color tint.
	/// </summary>
	public Color Color
	{
		get { return color; }
		set { SetColor(value); }
	}


	/// <summary>
	/// Sets the size (height) of the text such that a
	/// capital character that extends from the baseline
	/// to the absolute top of the line will be the
	/// specified height in world units.  All other
	/// characters will be sized proportionately.
	/// If called while set to pixel perfect, pixel-perfect
	/// will be automatically disabled.
	/// </summary>
	/// <param name="size">The size of a full-height character, in world units.</param>
	public void SetCharacterSize(float size)
	{
		if (spriteFont == null)
			return;

		pixelPerfect = false;
		characterSize = size;
		SetPixelToUV(texture);
		lineSpaceSize = lineSpacing * spriteFont.LineHeight * worldUnitsPerTexel;

		LayoutText();
	}

	/// <summary>
	/// Sets the spacing between lines as a percentage
	/// of the character size.  i.e. a value of 1.1
	/// means 10% of the height of a full-height character
	/// will be placed between lines.
	/// </summary>
	/// <param name="spacing">Percentage of the line height to place between lines.</param>
	public void SetLineSpacing(float spacing)
	{
		lineSpacing = spacing;
		lineSpaceSize = lineSpacing * spriteFont.LineHeight * worldUnitsPerTexel;

		LayoutText();
	}


	/// <summary>
	/// Changes the font to be used for this text object.
	/// </summary>
	/// <param name="newFont">The TextAsset that defines the font to be used.</param>
	/// <param name="fontMaterial">The material to use for the new font.</param>
	public void SetFont(TextAsset newFont, Material fontMaterial)
	{
		font = newFont;
		SetFont(FontStore.GetFont(newFont), fontMaterial);
	}

	/// <summary>
	/// Changes the font to be used for this text object.
	/// </summary>
	/// <param name="newFont">Reference to the SpriteFont to be used for this text object.</param>
	/// <param name="fontMaterial">The material to use for the new font.</param>
	public void SetFont(SpriteFont newFont, Material fontMaterial)
	{
		font = newFont.fontDef;
		spriteFont = newFont;
		renderer.sharedMaterial = fontMaterial;
		texture = fontMaterial.GetTexture("_MainTex");
		SetPixelToUV(texture);
		lineSpaceSize = lineSpacing * spriteFont.LineHeight * worldUnitsPerTexel;
		CalcSize();
		LayoutText();
	}



	// Sets the number of pixels per UV unit:
	public void SetPixelToUV(int texWidth, int texHeight)
	{
		if (spriteFont == null)
			return;

		pixelsPerUV.x = texWidth;
		pixelsPerUV.y = texHeight;

		worldUnitsPerTexel = characterSize / (float)spriteFont.PixelSize;

		worldUnitsPerUV.x = worldUnitsPerTexel * texWidth;
		worldUnitsPerUV.y = worldUnitsPerTexel * texHeight;
	}

	// Sets the number of pixels per UV unit:
	public void SetPixelToUV(Texture tex)
	{
		if (tex == null)
			return;
		SetPixelToUV(tex.width, tex.height);
	}

	/// <summary>
	/// Accessor for the camera that will be used to render this object.
	/// Use this to ensure the object is properly configured for the
	/// specific camera that will render it.
	/// </summary>
	public Camera RenderCamera
	{
		get { return renderCamera; }
		set 
		{
			renderCamera = value;
			SetCamera(value); 
		}
	}

	/// <summary>
	/// Updates any camera-dependent settings, such as
	/// the calculated pixel-perfect size.
	/// Use this with BroadcastMessage() to do bulk
	/// re-calculation of object sizes whenever your
	/// screensize/resolution changes at runtime.
	/// </summary>
	public void UpdateCamera()
	{
		SetCamera(renderCamera);
	}

	/// <summary>
	/// A no-argument version of SetCamera() that simply
	/// re-assigns the same camera to the object, forcing
	/// it to recalculate all camera-dependent calculations.
	/// </summary>
	public void SetCamera()
	{
		SetCamera(renderCamera);
	}

	/// <summary>
	/// Sets the camera to use when calculating
	/// a pixel-perfect character size.
	/// </summary>
	/// <param name="c"></param>
	public void SetCamera(Camera c)
	{
		if (c == null || !m_started)
			return;

		float dist;
		Plane nearPlane = new Plane(c.transform.forward, c.transform.position);

		if (!Application.isPlaying)
		{
			// If the screenSize has never been initialized,
			// or if this is a different camera, get what 
			// values we can get, otherwise just keep the 
			// values we got during our last run:
#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			if ((screenSize.x == 0 || c != renderCamera) && c.pixelHeight > 100)
#endif
			{
				screenSize.x = c.pixelWidth;
				screenSize.y = c.pixelHeight;
			}

			if (screenSize.x == 0)
				return;

			renderCamera = c;

			if (screenPlacer != null)
				screenPlacer.SetCamera(renderCamera);

			// Determine the world distance between two vertical
			// screen pixels for this camera:
			dist = nearPlane.GetDistanceToPoint(transform.position);
			//worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0, 1, dist)), c.ScreenToWorldPoint(new Vector3(0, 0, dist)));
			worldUnitsPerScreenPixel = 1;

			if(!hideAtStart)
				CalcSize();
			return;
		}

		screenSize.x = c.pixelWidth;
		screenSize.y = c.pixelHeight;
		renderCamera = c;

		if (screenPlacer != null)
			screenPlacer.SetCamera(renderCamera);

		// Determine the world distance between two vertical
		// screen pixels for this camera:
		dist = nearPlane.GetDistanceToPoint(transform.position);
		worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0, 1, dist)), c.ScreenToWorldPoint(new Vector3(0, 0, dist)));

		CalcSize();
	}

	/// <summary>
	/// Hides or displays the text by disabling/enabling
	/// the mesh renderer component.
	/// </summary>
	/// <param name="tf">When true, the text is hidden, when false, the text will be displayed.</param>
	public virtual void Hide(bool tf)
	{
		m_hidden = tf;

		if (meshRenderer == null)
			meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));

		meshRenderer.enabled = !tf;
	}

	/// <summary>
	/// Returns whether the text is currently set to be hidden
	/// (whether its mesh renderer component is enabled).
	/// </summary>
	/// <returns>True when hidden, false when set to be displayed.</returns>
	public bool IsHidden()
	{
		return m_hidden;
	}




	//--------------------------------------------------------------
	// Accessors:
	//--------------------------------------------------------------

	/// <summary>
	/// Marks the object to be persistent between scenes (it will not
	/// be destroyed when a new scene is loaded).  Once this is set
	/// to true, it cannot be undone.
	/// </summary>
	public bool Persistent
	{
		get { return persistent; }
		set
		{
			if(value == true)
			{
				DontDestroyOnLoad(this);
				DontDestroyOnLoad(mesh);
				persistent = value;
			}
		}
	}

	/// <summary>
	/// Sets and retrieves the full, decorated text string.
	/// </summary>
	public string Text
	{
		get { return text; }
		set
		{
			if (!m_awake)
				Awake(); // Make sure we have tried to get a spriteFont

			if (spriteFont == null)
				return;

			// Make sure we're ready:
			if (!m_started)
				Start();

			stringContentChanged = true;

			if (removeUnsupportedCharacters)
				ProcessString(spriteFont.RemoveUnsupportedCharacters(value));
			else
				ProcessString(value);

			UpdateMesh();
		}
	}

	/// <summary>
	/// Retrieves the plain (undecorated) text string.
	/// </summary>
	public string PlainText
	{
		get { return plainText; }
	}

	/// <summary>
	/// The actual string being displayed (includes
	/// line feed characters, etc, generated from
	/// any word-wrapping).
	/// </summary>
	public string DisplayString
	{
		get { return displayString; }
	}

	/// <summary>
	/// The rect against which the text should be clipped.  
	/// The text will be immediately clipped by this rect when set.
	/// When setting, the rect should be in world space.
	/// </summary>
	public Rect3D ClippingRect
	{
		get { return clippingRect; }
		set
		{
			if (ignoreClipping)
				return;

			clippingRect = value;
			clipped = true;
			updateClipping = true;
			UpdateMesh();
		}
	}


	/// <summary>
	/// Accessor for whether the text is to be clipped
	/// by any already-specified clipping rect.
	/// </summary>
	public bool Clipped
	{
		get { return clipped; }
		set
		{
			if (ignoreClipping)
				return;

			if (value && !clipped)
			{
				clipped = true;
				updateClipping = true;
				UpdateMesh();
			}
			else if (clipped)
				Unclip();
		}
	}


	/// <summary>
	/// Returns the position, in world space, of the insertion
	/// point relative to a specified character (specified by index).
	/// The position returned is at the left edge of, and the baseline
	/// of, the specified character.
	/// </summary>
	/// <param name="charIndex">The 0-based index of the character to the right of 
	/// where you want an insertion point. 0 indicates the very beginning, 
	/// and an index equal to the string's length indicates an insertion point 
	/// at the very end of the string.</param>
	/// <returns>Returns a point at the left edge of, and the baseline of, the 
	/// specified character.</returns>
	public Vector3 GetInsertionPointPos(int charIndex)
	{
		if (spriteFont == null)
			return Vector3.zero;

		// If the string is empty, return the start:
		if(meshString.Length < 1)
		{
			float baseHeight = ((float)spriteFont.BaseHeight) * worldUnitsPerTexel;
			return transform.TransformPoint(GetStartPos_SingleLine(baseHeight, 0).x, GetLineBaseline(1,1), offsetZ);
		}

		int numLines, lineNum, lineStart, lineEnd;
		int leftSideOffset = 1;
		float charX;

		// If we want the end of the string, include the charIndex's width:
		if (charIndex >= displayString.Length)
			leftSideOffset = 0;
		
		numLines = GetDisplayLineCount(charIndex, out lineNum, out lineStart, out lineEnd);

		// Clamp the character index:
		charIndex = Mathf.Min(charIndex, displayString.Length - 1);
	
		// See if we should move to the end of the previous line:
		if (charIndex < lineStart)
			GetDisplayLineCount(charIndex-1, out lineNum, out lineStart, out lineEnd);

		charX = spriteFont.GetWidth(displayString, lineStart, charIndex - leftSideOffset) * worldUnitsPerTexel * characterSpacing;

		// Adjust for the anchor if necessary:
		switch(anchor)
		{
			case Anchor_Pos.Upper_Center:
			case Anchor_Pos.Middle_Center:
			case Anchor_Pos.Lower_Center:
				if(alignment == Alignment_Type.Left)
				{
					// Subtract half the total width:
					charX -= totalWidth * 0.5f;
				}
				else if(alignment == Alignment_Type.Right)
				{
					// Not implemented...too weird to be a likely use case
				}
				else  // Subtract half the line width:
					charX -= spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing * 0.5f;
				break;

			case Anchor_Pos.Upper_Left:
			case Anchor_Pos.Middle_Left:
			case Anchor_Pos.Lower_Left:
				if(alignment == Alignment_Type.Center)
				{
					charX += (totalWidth * 0.5f) - (spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing * 0.5f);
				}
				else if(alignment == Alignment_Type.Right)
				{
					charX += totalWidth - (spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing);
				}
				else
				{
					// Do nothing...
				}
				break;

			case Anchor_Pos.Upper_Right:
			case Anchor_Pos.Middle_Right:
			case Anchor_Pos.Lower_Right:
				if(alignment == Alignment_Type.Center)
				{
					charX += (totalWidth * -0.5f) - (spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing * 0.5f);
				}
				else if(alignment == Alignment_Type.Left)
				{
					charX -= totalWidth;
				}
				else
				{
					charX += -1f * spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing;
				}
				break;
		}

		return transform.TransformPoint(charX, GetLineBaseline(numLines, lineNum), offsetZ);
	}

	/// <summary>
	/// Returns both the world-space position of the insertion point
	/// most nearly matching the specified point, as well as the
	/// index of the character to the left of which the insertion
	/// point corresponds.
	/// </summary>
	/// <param name="point">A point, in world space, from which you want to find
	/// the nearest insertion point.</param>
	/// <param name="insertionPt">OUT: Will hold the index of the character to
	/// the left of which the insertion point will be. If the insertion point is
	/// at the end of the string, this value will be one greater than the index
	/// of the last character.</param>
	/// <returns>The position, in world space, of the desired insertion point.</returns>
	public Vector3 GetNearestInsertionPointPos(Vector3 point, ref int insertionPt)
	{
		insertionPt = GetNearestInsertionPoint(point);
		return GetInsertionPointPos(insertionPt);
	}

	/// <summary>
	/// Returns both the world-space position of the insertion point
	/// most nearly matching the specified point, as well as the
	/// index of the character to the left of which the insertion
	/// point corresponds.
	/// </summary>
	/// <param name="point">A point, in world space, from which you want to find
	/// the nearest insertion point.</param>
	/// <returns>The index of the character to
	/// the right of where the insertion point will be. If the insertion point is
	/// at the end of the string, this value will be one greater than the index
	/// of the last character.</returns>
	public int GetNearestInsertionPoint(Vector3 point)
	{
		point = transform.InverseTransformPoint(point);

		// First find which row the click is nearest to:
		int rows = GetDisplayLineCount();

		// Will tell us where to start searching in our string
		int charOffset = 0;

		if(rows > 1)
		{
			float yDiff;
			float nearestY = float.PositiveInfinity;
			int nearestLine = 1;

			for(int i=1; i <= rows; ++i)
			{
				yDiff = point.y - (GetLineBaseline(rows, i) + BaseHeight * 0.5f);

				if(Mathf.Abs(yDiff) < nearestY)
				{
					nearestY = Mathf.Abs(yDiff);
					nearestLine = i;
				}
			}

			// Now find the offset into the string of the
			// start of the nearest line:
			for (int i = 0, lineCount = 1; i < displayString.Length && lineCount < nearestLine; ++i)
			{
				if (displayString[i] == '\n')
				{
					++lineCount;
					charOffset = i+1;
				}
			}
		}

		int insertionPt = charOffset;

		// Now start searching our desired range for the nearest
		// character:
		for (int i = charOffset; i < displayString.Length && displayString[i] != '\n'; ++i)
		{
			// Skip whitespace since we can't use its geometry:
			if (char.IsWhiteSpace(displayString[i]))
				continue;

			// Default to after this character:
			insertionPt = i+1;

			// Keep going until the center of the character is 
			// to the right of our point:
			int idx = i * 4;
			float centerX = vertices[idx].x + (0.5f * (vertices[idx + 2].x - vertices[idx].x));

			if (centerX >= point.x)
			{
				insertionPt = i;
				break;
			}
		}

		return insertionPt;
	}

	/// <summary>
	/// Returns the distance from the baseline to the
	/// top of a line, in local space units.
	/// </summary>
	public float BaseHeight
	{
		get 
		{
			if (spriteFont != null)
				return spriteFont.BaseHeight * worldUnitsPerTexel;
			else
				return 0;
		}
	}

	/// <summary>
	/// The distance, in local space units, from the top of
	/// one line to the top of the next.
	/// </summary>
	public float LineSpan
	{
		get { return lineSpaceSize; }
	}

	/// <summary>
	/// Returns a reference to the text's vertices.
	/// </summary>
	/// <returns>A reference to the sprite's vertices.</returns>
	public Vector3[] GetVertices()
	{
		return mesh.vertices;
	}

	/// <summary>
	/// Gets the center point of the text's extents in local space.
	/// </summary>
	/// <returns>The center point of the text extents.</returns>
	public Vector3 GetCenterPoint()
	{
		return new Vector3(topLeft.x + 0.5f * (bottomRight.x - topLeft.x), topLeft.y - 0.5f * (topLeft.y - bottomRight.y), offsetZ);
	}

	/// <summary>
	/// Returns the position of the top-left corner
	/// of the text's extremities.
	/// </summary>
	public Vector3 TopLeft
	{
		get { return topLeft; }
	}

	/// <summary>
	/// Returns the position of the bottom-right corner
	/// of the text's extremities.
	/// </summary>
	public Vector3 BottomRight
	{
		get { return bottomRight; }
	}

	public Vector3 UnclippedTopLeft
	{
		get 
		{
			// Make sure we have a meaningful value:
			if (!m_started)
				Start();
			return unclippedTL;
		}
	}

	public Vector3 UnclippedBottomRight
	{
		get 
		{ 
			// Make sure we have a meaningful value:
			if (!m_started)
				Start();
			return unclippedBR; 
		}
	}

	/// <summary>
	/// The current total width of the text (widest line) in local units.
	/// </summary>
 	public float TotalWidth
	{
		get { return totalWidth; }
	}

	/// <summary>
	/// Returns the current total width of the text (widest line) in screen pixels.
	/// </summary>
	public float TotalScreenWidth
	{
		get 
		{
			if (renderCamera == null)
				return 0;

			float dist;
			Plane nearPlane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);

			screenSize.x = renderCamera.pixelWidth;
			screenSize.y = renderCamera.pixelHeight;

			// Determine the world distance between two vertical
			// screen pixels for this camera:
			dist = nearPlane.GetDistanceToPoint(transform.position);
			worldUnitsPerScreenPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0, 1, dist)), renderCamera.ScreenToWorldPoint(new Vector3(0, 0, dist)));

			return totalWidth / worldUnitsPerScreenPixel; 
		}
	}

	/// <summary>
	/// Returns the width, in world units, of the specified string
	/// were it to be rendered using the current font and settings.
	/// NOTE: Assumes the string is a single line.
	/// </summary>
	/// <param name="s">The string to measure.</param>
	/// <returns>The width, in world units.</returns>
	public float GetWidth(string s)
	{
		if (spriteFont == null)
			return 0;

		return spriteFont.GetWidth(s) * worldUnitsPerTexel * characterSpacing;
	}


	/// <summary>
	/// Returns the width, in screen pixels, of the specified string
	/// were it to be rendered using the current font and settings.
	/// NOTE: Assumes the string is a single line.
	/// </summary>
	/// <param name="s">The string to measure.</param>
	/// <returns>The width, in world units.</returns>
	public float GetScreenWidth(string s)
	{
		if (spriteFont == null)
			return 0;

		if (renderCamera == null)
			return 0;

		float dist;
		Plane nearPlane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);

		screenSize.x = renderCamera.pixelWidth;
		screenSize.y = renderCamera.pixelHeight;

		// Determine the world distance between two vertical
		// screen pixels for this camera:
		dist = nearPlane.GetDistanceToPoint(transform.position);
		worldUnitsPerScreenPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0, 1, dist)), renderCamera.ScreenToWorldPoint(new Vector3(0, 0, dist)));

		return GetWidth(s) / worldUnitsPerScreenPixel;
	}


	/// <summary>
	/// Gets the width and height of the text area in pixels as it appears on-screen.
	/// </summary>
	public Vector2 PixelSize
	{
		get 
		{
			Vector2 size = new Vector2(bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
			return new Vector2(size.x * worldUnitsPerScreenPixel, size.y * worldUnitsPerScreenPixel); 
		}
	}


	/// <summary>
	/// Sets the anchor type to use.
	/// See <see cref="Anchor_Pos"/>
	/// </summary>
	/// <param name="a">The anchor method to use.</param>
	public void SetAnchor(Anchor_Pos a)
	{
		anchor = a;
		LayoutText();
	}

	/// <summary>
	/// Accessor for the object's anchor method.
	/// </summary>
	public Anchor_Pos Anchor
	{
		get { return anchor; }
		set { SetAnchor(value); }
	}

	/// <summary>
	/// Sets the alignment of the text (left, right, center).
	/// </summary>
	/// <param name="a">The alignment to use.</param>
	public void SetAlignment(Alignment_Type a)
	{
		alignment = a;
		LayoutText();
	}

	/// <summary>
	/// An adjustable factor by which you can increase/decrease
	/// the spacing between characters.  A value of 1.0 will
	/// space characters exactly as described by the font.
	/// Decreasing this value will place the characters closer
	/// together, while increasing it will place them farther
	/// apart.
	/// </summary>
	public float CharacterSpacing
	{
		get { return characterSpacing; }
		set
		{
			characterSpacing = value;
			LayoutText();
		}
	}

	// Used internally by any parent control
	public IControl Parent
	{
		get { return parentControl; }
		set { parentControl = value; }
	}


	//--------------------------------------------------------------
	// Utility methods:
	//--------------------------------------------------------------

	/// <summary>
	/// Converts pixel-space values to UV-space scalar values
	/// according to the currently assigned material.
	/// NOTE: This is for converting widths and heights-not
	/// coordinates (which have reversed Y-coordinates).
	/// For coordinates, use <see cref="PixelCoordToUVCoord"/>()!
	/// </summary>
	/// <param name="xy">The values to convert.</param>
	/// <returns>The values converted to UV space.</returns>
	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		if (pixelsPerUV.x == 0 || pixelsPerUV.y == 0)
			return Vector2.zero;

		return new Vector2(xy.x / pixelsPerUV.x, xy.y / pixelsPerUV.y);
	}

	/// <summary>
	/// Converts pixel-space values to UV-space scalar values
	/// according to the currently assigned material.
	/// NOTE: This is for converting widths and heights-not
	/// coordinates (which have reversed Y-coordinates).
	/// For coordinates, use <see cref="PixelCoordToUVCoord"/>()!
	/// </summary>
	/// <param name="x">The X-value to convert.</param>
	/// <param name="y">The Y-value to convert.</param>
	/// <returns>The values converted to UV space.</returns>
	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2((float)x, (float)y));
	}

	/// <summary>
	/// Converts pixel coordinates to UV coordinates according to
	/// the currently assigned material.
	/// NOTE: This is for converting coordinates and will reverse
	/// the Y component accordingly.  For converting widths and
	/// heights, use <see cref="PixelSpaceToUVSpace"/>()!
	/// </summary>
	/// <param name="xy">The coordinates to convert.</param>
	/// <returns>The coordinates converted to UV coordinates.</returns>
	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		if (pixelsPerUV.x == 0 || pixelsPerUV.y == 0)
			return Vector2.zero;

		return new Vector2(xy.x / (pixelsPerUV.x - 1f), 1.0f - (xy.y / (pixelsPerUV.y - 1f)));
	}

	/// <summary>
	/// Converts pixel coordinates to UV coordinates according to
	/// the currently assigned material.
	/// NOTE: This is for converting coordinates and will reverse
	/// the Y component accordingly.  For converting widths and
	/// heights, use <see cref="PixelSpaceToUVSpace"/>()!
	/// </summary>
	/// <param name="x">The x-coordinate to convert.</param>
	/// <param name="y">The y-coordinate to convert.</param>
	/// <returns>The coordinates converted to UV coordinates.</returns>
	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2((float)x, (float)y));
	}


	// Uses the mirror object to validate and respond
	// to changes in our inspector.
	public virtual void DoMirror()
	{
		// Only run if we're not playing:
		if (Application.isPlaying)
			return;

		// This means Awake() was recently called, meaning
		// we couldn't reliably get valid camera viewport
		// sizes, so we zeroed them out so we'd know to
		// get good values later on (when OnDrawGizmos()
		// is called):
		if (screenSize.x == 0 || screenSize.y == 0)
			Start();

		if (mirror == null)
		{
			mirror = new SpriteTextMirror();
			mirror.Mirror(this);
		}

		mirror.Validate(this);

		// Compare our mirrored settings to the current settings
		// to see if something was changed:
		if (mirror.DidChange(this))
		{
			stringContentChanged = true;
			Init();
			meshString = "";
			ProcessString(text);
			CalcSize();
			mirror.Mirror(this);	// Update the mirror
		}
	}

	// Included to work around the Unity bug where Start() is not
	// called when reentering edit mode if play lasts for longer 
	// than 10 seconds:
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9) && UNITY_EDITOR
	void Update() 
	{
		DoMirror();
	}
#else
	// Ensures that the text is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmosSelected()
	{
		DoMirror();
	}

	// Ensures that the text is updated in the scene view
	// while editing:
	public virtual void OnDrawGizmos()
	{
		DoMirror();
	}
#endif
}


// Mirrors the editable settings of a SpriteText that affect
// how it is drawn in the scene view
public class SpriteTextMirror
{
	public string text;
	public TextAsset font;
	public float offsetZ;
	public float characterSize;
	public float characterSpacing;
	public float lineSpacing;
	public SpriteText.Anchor_Pos anchor;
	public SpriteText.Alignment_Type alignment;
	public int tabSize;
	public Color color;
	public float maxWidth;
	public bool maxWidthInPixels;
	public bool pixelPerfect;
	public Camera renderCamera;
	public bool hideAtStart;
	/*
		public Vector3 pos;
		public Vector3 scale;
		public Quaternion rot;
	*/

	// Mirrors the specified SpriteText's settings
	public virtual void Mirror(SpriteText s)
	{
		text = s.text;
		font = s.font;
		offsetZ = s.offsetZ;
		characterSize = s.characterSize;
		characterSpacing = s.characterSpacing;
		lineSpacing = s.lineSpacing;
		anchor = s.anchor;
		alignment = s.alignment;
		tabSize = s.tabSize;
		color = s.color;
		maxWidth = s.maxWidth;
		maxWidthInPixels = s.maxWidthInPixels;
		pixelPerfect = s.pixelPerfect;
		renderCamera = s.renderCamera;
		hideAtStart = s.hideAtStart;
	}

	// Validates certain settings:
	public virtual bool Validate(SpriteText s)
	{
		return true;
	}

	// Returns true if any of the settings do not match:
	public virtual bool DidChange(SpriteText s)
	{
		if (s.text != text)
			return true;
		if (s.font != font)
			return true;
		if (s.offsetZ != offsetZ)
			return true;
		if (s.characterSize != characterSize)
			return true;
		if (s.characterSpacing != characterSpacing)
			return true;
		if (s.lineSpacing != lineSpacing)
			return true;
		if (s.anchor != anchor)
			return true;
		if (s.alignment != alignment)
			return true;
		if (s.tabSize != tabSize)
			return true;
		if (s.color.r != color.r ||
			s.color.g != color.g ||
			s.color.b != color.b ||
			s.color.a != color.a)
			return true;
		if (maxWidth != s.maxWidth)
			return true;
		if (maxWidthInPixels != s.maxWidthInPixels)
			return true;
		if (s.pixelPerfect != pixelPerfect)
		{
			s.SetCamera(s.renderCamera);
			return true;
		}
		if (s.renderCamera != renderCamera)
		{
			s.SetCamera(s.renderCamera);
			return true;
		}
		if (s.hideAtStart != hideAtStart)
		{
			s.Hide(s.hideAtStart);
			return true;
		}

		return false;
	}
}
