//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

//#define BINARY_FONT


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if BINARY_FONT
using System.IO;
#endif


/// <remarks>
/// A class that holds important information
/// about a sprite-based character.
/// </remarks>
public class SpriteChar
{
	/// <summary>
	/// The "id" of the character (usually the ASCII value).
	/// </summary>
	public int id;

	/// <summary>
	/// The UV coords of the character.
	/// </summary>
	public Rect UVs;

	/// <summary>
	/// The offset, in pixels, of the char's mesh from its "zero-point".
	/// </summary>
	public float xOffset, yOffset;

	/// <summary>
	/// How far to move, in pixels, from this char to position the next one.
	/// </summary>
	public float xAdvance;

	/// <summary>
	/// The map of kernings to use for preceding characters.
	/// The key is the previous character, and the value is 
	/// the kerning amount, in pixels.
	/// </summary>
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	public Hashtable kernings;
#else
	public Dictionary<int, float> kernings;
#endif

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	public Hashtable origKernings;
#else
	public Dictionary<int, float> origKernings;
#endif

	/// <summary>
	/// Gets the kerning amount given the previous character.
	/// </summary>
	/// <param name="prevChar">The character that precedes this one.</param>
	/// <returns>The kerning amount, in pixels.</returns>
	public float GetKerning(int prevChar)
	{
		if (kernings == null)
			return 0;

		float amount = 0;

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (kernings.ContainsKey(prevChar))
			amount = (float) kernings[prevChar];
#else
		kernings.TryGetValue(prevChar, out amount);
#endif
		return amount;
	}
}


/// <remarks>
/// A class that holds information about a font
/// intended for use with SpriteText.
/// </remarks>
public class SpriteFont
{
	// Parsing delegate type:
	protected delegate void ParserDel(string line);

	/// <summary>
	/// The TextAsset that defines the font.
	/// </summary>
	public TextAsset fontDef;

	// Maps character IDs to that character's
	// index in the array.
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
	protected Hashtable charMap = new Hashtable();
#else
	protected Dictionary<int, int> charMap = new Dictionary<int, int>();
#endif

	// Our characters:
	protected SpriteChar[] chars;

	// Bleed compensate by 1/2 pixel.
	protected const float bleedCompensation = 0; //0.5f;
	protected Vector2 bleedCompUV, bleedCompUVMax;

	protected int lineHeight;
	/// <summary>
	/// The default height, in pixels, between lines.
	/// </summary>
	public int LineHeight
	{
		get { return lineHeight; }
		set { lineHeight = value; }
	}

	protected int baseHeight;
	/// <summary>
	///	The distance, in pixels, from the absolute top
	/// of a line to the baseline:
	/// </summary>
	public int BaseHeight
	{
		get { return baseHeight; }
	}
	// The width and height of the font atlas.
	protected int texWidth, texHeight;

	/// <summary>
	/// The name of the font face.
	/// </summary>
	protected string face;

	protected int pxSize;
	/// <summary>
	///	The size (height) of the font, in pixels.
	/// This is the height, in pixels of a full-height
	/// character.
	/// </summary>
	public int PixelSize
	{
		get { return pxSize; }
	}

	protected float charSpacing = 1f;
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
		get { return charSpacing; }
		set
		{
			float oldVal = charSpacing;
			charSpacing = value;

			// Only update things if the value has changed:
			if (oldVal != charSpacing)
			{
				if (chars != null)
				{
					for (int i = 0; i < chars.Length; ++i)
					{
						if (chars[i] == null)
							continue;

						chars[i].xAdvance *= charSpacing;

						if (chars[i].kernings != null)
						{
							int[] keys = new int[chars[i].kernings.Keys.Count];
							chars[i].kernings.Keys.CopyTo(keys, 0);

							for (int j = 0; j < keys.Length; ++j)
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
								chars[i].kernings[keys[j]] = (object) (((float)chars[i].origKernings[keys[j]]) * charSpacing);
#else
								chars[i].kernings[keys[j]] = charSpacing * chars[i].origKernings[keys[j]];
#endif
						}
					}
				}
			}
		}
	}

	// Working vars:
	int kerningsCount;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="def">TextAsset that defines the font.</param>
	public SpriteFont(TextAsset def)
	{
		Load(def);
	}

#if !BINARY_FONT
	/// <summary>
	/// Loads a font from the specified font definition TextAsset
	/// and adds it to the font store.
	/// </summary>
	/// <param name="fontDef">The TextAsset that defines the font.</param>
	public void Load(TextAsset def)
	{
		if (def == null)
			return;

		int pos, c = 0;

		fontDef = def;

		string[] lines = fontDef.text.Split(new char[] { '\n' });

		pos = ParseSection("info", lines, HeaderParser, 0);
		pos = ParseSection("common", lines, CommonParser, pos);
		pos = ParseSection("chars count", lines, CharCountParser, pos);

		// Do some quick computations
		bleedCompUV = new Vector2(bleedCompensation / texWidth, bleedCompensation / texHeight);
		bleedCompUVMax = bleedCompUV * -2f;


		while (pos < lines.Length && c < chars.Length)
		{
			if (CharParser(lines[pos++], c))
				++c; // Tally another character
			else
				break;
		}

		// Back up one line to be safe:
		--pos;

		pos = ParseSection("kernings count", lines, KerningCountParser, pos);

		c = 0;
		while (pos < lines.Length && c < kerningsCount)
			if (KerningParser(lines[pos++]))
				++c; // Tally another kerning

		// Now apply any character spacing
		// (setting it temporarily to another
		// value incase this was set in-script
		// before the file was parsed):
		float tempSpacing = charSpacing;
		charSpacing = 0;
		CharacterSpacing = tempSpacing;
	}

#else

	/// <summary>
	/// Loads a font from the specified font definition TextAsset
	/// and adds it to the font store.
	/// </summary>
	/// <param name="fontDef">The TextAsset that defines the font.</param>
	public void Load(TextAsset def)
	{
		if (def == null)
			return;

		//Binary BM font support
		Stream stream = new MemoryStream(def.bytes);
		BinaryReader br = new BinaryReader(stream);
		byte[] magicString = br.ReadBytes(4);

		if (Encoding.UTF8.GetString(magicString).Equals("BMF" + (char)3))
		{
			char blockType;
			int blockSize;
			fontDef = def;

			while (true)
			{
				try
				{
					blockType = br.ReadChar();
					blockSize = br.ReadInt32();

					switch (blockType)
					{
						case (char)1:
							ReadInfoBlock(br, blockSize);
							break;
						case (char)2:
							ReadCommonBlock(br, blockSize);
							break;
						case (char)3:
							ReadPagesBlock(br, blockSize);
							break;
						case (char)4:
							ReadCharsBlock(br, blockSize);
							break;
						case (char)5:
							ReadKerningPairsBlock(br, blockSize);
							break;
						default:
							Debug.Log("Unexpected block type " + (int)blockType);
							break;
					}
				}
				catch (EndOfStreamException eose)
				{
					EndOfStreamException dummy = eose;
					eose = dummy;
					break;
				}
			}
		}
		else
		{
			int pos, c = 0;
			fontDef = def;

			string[] lines = fontDef.text.Split(new char[] { '\n' });

			pos = ParseSection("info", lines, HeaderParser, 0);
			pos = ParseSection("common", lines, CommonParser, pos);
			pos = ParseSection("chars count", lines, CharCountParser, pos);

			while (pos < lines.Length && c < chars.Length)
				if (CharParser(lines[pos++], c))
					++c;

			// Tally another character
			pos = ParseSection("kernings count", lines, KerningCountParser, pos);

			c = 0;

			while (pos < lines.Length && c < kerningsCount)
				if (KerningParser(lines[pos++]))
					++c;
		}

		// Tally another kerning
		// Now apply any character spacing
		// (setting it temporarily to another
		// value incase this was set in-script
		// before the file was parsed):
		float tempSpacing = charSpacing;
		charSpacing = 0;
		CharacterSpacing = tempSpacing;

		br.Close();
		stream.Close();
		stream.Dispose();
	}

	void ReadInfoBlock(BinaryReader br, int blockSize)
	{
		int remaining;

		remaining = blockSize;
		pxSize = br.ReadUInt16();
		br.ReadBytes(12);
		remaining -= 14;
		face = Encoding.Default.GetString(br.ReadBytes(remaining));
	}

	void ReadCommonBlock(BinaryReader br, int blockSize)
	{
		int remaining;

		remaining = blockSize;
		lineHeight = br.ReadUInt16();
		baseHeight = br.ReadUInt16();
		texWidth = br.ReadUInt16();
		texHeight = br.ReadUInt16();

		int pages = br.ReadUInt16();

		if (pages > 1)
			Debug.LogError("Multiple pages/textures detected for font \"" + face + "\". only one font atlas is supported.");

		remaining -= 10;
		br.ReadBytes(remaining);
	}

	void ReadPagesBlock(BinaryReader br, int blockSize)
	{
		int remaining;
		remaining = blockSize;
		br.ReadBytes(remaining);
	}


	void ReadCharsBlock(BinaryReader br, int blockSize)
	{
		int count = blockSize / 20;
	
		chars = new SpriteChar[count + 1];
	
		for (int i = 0; i < count; i++)
		{
			chars[i] = new SpriteChar();
			chars[i].id = (int)br.ReadUInt32();

			float x = br.ReadUInt16() / (float)texWidth;
			float y = 1f - br.ReadUInt16() / (float)texHeight;
			float width = br.ReadUInt16() / (float)texWidth;
			float height = br.ReadUInt16() / (float)texHeight;

			chars[i].xOffset = br.ReadInt16();
			chars[i].yOffset = -br.ReadInt16();
			chars[i].xAdvance = br.ReadInt16();

			// Build our character's UVs:
			chars[i].UVs.x = x;
			chars[i].UVs.y = y - height;
			chars[i].UVs.xMax = x + width;
			chars[i].UVs.yMax = y;
			br.ReadBytes(2);

			charMap.Add(Convert.ToInt32(chars[i].id), i);
		}
	}

	void ReadKerningPairsBlock(BinaryReader br, int blockSize)
	{
		int count = blockSize / 10;
	
		kerningsCount = count;

		for (int i = 0; i < count; i++)
		{
			int first = (int)br.ReadUInt32();
			int second = (int)br.ReadUInt32();
			int amount = br.ReadInt16();

			// Now add the kerning info to the appropriate character:
			SpriteChar ch = GetSpriteChar(Convert.ToChar(second));

			if (ch.kernings == null)
			{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
				ch.kernings = new Hashtable ();
				ch.origKernings = new Hashtable ();
#else
				ch.kernings = new Dictionary<int, float>();
				ch.origKernings = new Dictionary<int, float>();
#endif
			}

			ch.kernings.Add(first, (float)amount);
			ch.origKernings.Add(first, (float)amount);
		}
	}
#endif

	// Finds a line that starts with "tag" and passes that line
	// to the specified parsing delegate.
	int ParseSection(string tag, string[] lines, ParserDel parser, int pos)
	{
		for (; pos < lines.Length; ++pos)
		{
			string line = lines[pos].Trim();

			if (line.Length < 1)
				continue;

			if (line.StartsWith(tag))
			{
				parser(line);
				return ++pos;
			}
		}

		return pos;
	}

	// Returns the index of the field matching
	// the specified label
	int FindField(string label, string[] fields, int pos, bool logError)
	{
		for (; pos < fields.Length; ++pos)
		{
			if (label == fields[pos].Trim())
				return pos;
		}

		if (logError)
		{
			Debug.LogError("Missing \"" + label + "\" field in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
			return pos;
		}
		else
			return -1;
	}

	int FindField(string label, string[] fields, int pos)
	{
		return FindField(label, fields, pos, true);
	}

	int FindFieldOptional(string label, string[] fields, int pos)
	{
		return FindField(label, fields, pos, false);
	}

	// Parses the font definition header.
	void HeaderParser(string line)
	{
		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("face", vals, 1);
		face = vals[index + 1].Trim(new char[] { '\"' });

		index = FindField("size", vals, index);
		pxSize = Mathf.Abs(int.Parse(vals[index + 1]));

		index = FindFieldOptional("charSpacing", vals, index);
		if (index != -1)
			charSpacing = Mathf.Abs(float.Parse(vals[index + 1]));
	}

	// Parses the "common" line
	void CommonParser(string line)
	{
		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("lineHeight", vals, 1);
		lineHeight = int.Parse(vals[index + 1]);

		index = FindField("base", vals, index);
		baseHeight = int.Parse(vals[index + 1]);

		index = FindField("scaleW", vals, index);
		texWidth = int.Parse(vals[index + 1]);

		index = FindField("scaleH", vals, index);
		texHeight = int.Parse(vals[index + 1]);

		index = FindField("pages", vals, index);

		if (int.Parse(vals[index + 1]) > 1)
			Debug.LogError("Multiple pages/textures detected for font \"" + face + "\". only one font atlas is supported.");
	}

	// Parses the "chars count" line
	void CharCountParser(string line)
	{
		string[] vals = line.Split(new char[] { '=' });

		if (vals.Length < 2)
		{
			Debug.LogError("Malformed \"chars count\" line in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
			return;
		}

		// Add one for the space character that is
		// always included but not counted:
		chars = new SpriteChar[int.Parse(vals[1]) + 1];
	}

	// Parses a character definition line
	bool CharParser(string line, int charNum)
	{
		if (!line.StartsWith("char"))
			return false;

		float x, y, width, height;

		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("id", vals, 1);
		chars[charNum] = new SpriteChar();
		chars[charNum].id = int.Parse(vals[index + 1]);

		index = FindField("x", vals, index);
		x = float.Parse(vals[index + 1]) / (float)texWidth;

		index = FindField("y", vals, index);
		y = 1f - float.Parse(vals[index + 1]) / (float)texHeight;

		index = FindField("width", vals, index);
		width = float.Parse(vals[index + 1]) / (float)texWidth;

		index = FindField("height", vals, index);
		height = float.Parse(vals[index + 1]) / (float)texHeight;

		index = FindField("xoffset", vals, index);
		chars[charNum].xOffset = float.Parse(vals[index + 1]);

		index = FindField("yoffset", vals, index);
		chars[charNum].yOffset = -float.Parse(vals[index + 1]);

		index = FindField("xadvance", vals, index);
		chars[charNum].xAdvance = int.Parse(vals[index + 1]);

		// Build our character's UVs:
		chars[charNum].UVs.x = x + bleedCompUV.x;
		chars[charNum].UVs.y = y - height + bleedCompUV.y;
		chars[charNum].UVs.xMax = x + width + bleedCompUVMax.x;
		chars[charNum].UVs.yMax = y + bleedCompUVMax.y;

		charMap.Add(Convert.ToChar(chars[charNum].id), charNum);

		return true;
	}

	// Parses the kernings count
	void KerningCountParser(string line)
	{
		string[] vals = line.Split(new char[] { '=' });
		kerningsCount = int.Parse(vals[1]);
	}

	// Parses the kernings
	bool KerningParser(string line)
	{
		if (!line.StartsWith("kerning"))
			return false;

		int first, second, amount;

		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("first", vals, 1);
		first = int.Parse(vals[index + 1]);

		index = FindField("second", vals, index);
		second = int.Parse(vals[index + 1]);

		index = FindField("amount", vals, index);
		amount = int.Parse(vals[index + 1]);

		// Now add the kerning info to the appropriate character:
		SpriteChar ch = GetSpriteChar(Convert.ToChar(second));

		if (ch == null)
			return true;

		if (ch.kernings == null)
		{
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			ch.kernings = new Hashtable();
			ch.origKernings = new Hashtable();
#else
			ch.kernings = new Dictionary<int, float>();
			ch.origKernings = new Dictionary<int, float>();
#endif
		}

		ch.kernings.Add(Convert.ToChar(first), (float)amount);
		ch.origKernings.Add(Convert.ToChar(first), (float)amount);
		return true;
	}

	/// <summary>
	/// Returns a reference to the SpriteChar that
	/// corresponds to the specified character ID
	/// (usually the numeric Unicode value).
	/// </summary>
	/// <param name="ch">The numeric value/code of the desired character.
	/// This value can be obtained from a char with Convert.ToInt32().</param>
	/// <returns>Reference to the corresponding SpriteChar that contains information about the character.</returns>
	public SpriteChar GetSpriteChar(char ch)
	{
		int index;
#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		if (!charMap.ContainsKey(ch))
#else
		if (!charMap.TryGetValue(ch, out index))
#endif
			return default(SpriteChar); // Character not found

#if UNITY_IPHONE && !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
		index = (int) charMap[ch];
		return chars[index];
#else
		return chars[index];
#endif
	}

	/// <summary>
	/// Returns whether the specified character is part
	/// of this font definition.
	/// </summary>
	/// <param name="ch">Character to check.</param>
	/// <returns>True if the character exists in the font definition.  False otherwise.</returns>
	public bool ContainsCharacter(char ch)
	{
		return charMap.ContainsKey(ch);
	}

	/// <summary>
	/// Gets how wide the specified string
	/// would be, in pixels.
	/// </summary>
	/// <param name="str">The string to measure.</param>
	/// <returns>The width, in pixels, of the string.</returns>
	public float GetWidth(string str)
	{
		SpriteChar chr;
		float width = 0;

		if (str.Length < 1)
			return 0;

		// Get the first character:
		chr = GetSpriteChar(str[0]);

		if(chr != null)
			width = chr.xAdvance;

		for (int i = 1; i < str.Length; ++i)
		{
			chr = GetSpriteChar(str[i]);
			
			if (chr != null)
				width += chr.xAdvance + chr.GetKerning(str[i - 1]);
		}

		return width;
	}

	/// <summary>
	/// Gets how wide the specified string
	/// would be, in pixels.
	/// </summary>
	/// <param name="str">The string to measure.</param>
	/// <param name="start">The index of the first character of the substring to be measured.</param>
	/// <param name="end">The index of the last character of the substring.</param>
	/// <returns>The width, in pixels, of the string.</returns>
	public float GetWidth(string str, int start, int end)
	{
		SpriteChar chr;
		float width = 0;

		if (start >= str.Length || end < start)
			return 0;

		end = Mathf.Clamp(end, 0, str.Length - 1);

		// Get the first character:
		chr = GetSpriteChar(str[start]);
		
		if (chr != null)
			width = chr.xAdvance;

		for (int i = start + 1; i <= end; ++i)
		{
			chr = GetSpriteChar(str[i]);

			if (chr != null)
				width += chr.xAdvance + chr.GetKerning(str[i - 1]);
		}

		return width;
	}

	/// <summary>
	/// Gets how wide the specified string
	/// would be, in pixels.
	/// </summary>
	/// <param name="str">The string to measure.</param>
	/// <param name="start">The index of the first character of the substring to be measured.</param>
	/// <param name="end">The index of the last character of the substring.</param>
	/// <returns>The width, in pixels, of the string.</returns>
	public float GetWidth(StringBuilder sb, int start, int end)
	{
		SpriteChar chr;
		float width = 0;

		if (start >= sb.Length || end < start)
			return 0;

		end = Mathf.Clamp(end, 0, sb.Length - 1);

		// Get the first character:
		chr = GetSpriteChar(sb[start]);

		if (chr != null)
			width = chr.xAdvance;

		for (int i = start + 1; i <= end; ++i)
		{
			chr = GetSpriteChar(sb[i]);

			if (chr != null)
				width += chr.xAdvance + chr.GetKerning(sb[i - 1]);
		}

		return width;
	}

	/// <summary>
	/// Gets how wide the specified character
	/// would be, in pixels, when displayed.
	/// </summary>
	/// <param name="prevChar">The character previous to that being measured.</param>
	/// <param name="str">The character to measure.</param>
	/// <returns>The width, in pixels, of the character, as displayed (includes the xAdvance).</returns>
	public float GetWidth(char prevChar, char c)
	{
		SpriteChar chr = GetSpriteChar(c);

		if (chr == null)
			return 0;

		return chr.xAdvance + chr.GetKerning(prevChar);
	}

	/// <summary>
	/// Returns the xAdvance of the specified character.
	/// 0 is returned if the character isn't supported.
	/// </summary>
	/// <param name="c">The character to look up.</param>
	/// <returns>The xAdvance of the character.</returns>
	public float GetAdvance(char c)
	{
		SpriteChar ch = GetSpriteChar(c);

		if (ch == null)
			return 0;
		else
			return ch.xAdvance;
	}

	/// <summary>
	/// Returns a version of the specified string with all
	/// characters removed which are not defined for this font.
	/// </summary>
	/// <param name="str">The string to be stripped of unsupported characters.</param>
	/// <returns>A new string containing only those characters supported by this font.</returns>
	public string RemoveUnsupportedCharacters(string str)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < str.Length; ++i)
			if (charMap.ContainsKey(str[i]) || str[i] == '\n' || str[i] == '\t' ||
				str[i] == '#' || str[i] == '[' || str[i] == ']' || str[i] == '(' || str[i] == ')' || str[i] == ',') // Color tag chars
				sb.Append(str[i]);

		return sb.ToString();
	}
}
