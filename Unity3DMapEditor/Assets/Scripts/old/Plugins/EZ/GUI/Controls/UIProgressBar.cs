//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// A progress bar class. Progress is altered by changing
/// the "Value" property (valid values are from 0-1).
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Progress Bar")]
public class UIProgressBar : AutoSpriteControlBase
{
	// Stores the value of the progress bar
	protected float m_value;

	/// <summary>
	/// Represents the percent of progress. Valid range is from 0-1.
	/// </summary>
	public float Value
	{
		get { return m_value; }
		set 
		{
			m_value = Mathf.Clamp01(value);
			UpdateProgress();
		}
	}


	// Reference to the sprite that will be used
	// to draw the empty portion of the progress bar:
	protected AutoSprite emptySprite;

	// State info to use to draw the appearance
	// of the bar.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Filled"),
			new TextureAnim("Empty")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	public override EZTransitionList GetTransitions(int index)
	{
		return null;
	}

	public override EZTransitionList[] Transitions
	{
		get { return null; }
		set {  }
	}



	/// <summary>
	/// An array of references to sprites which will
	/// visually represent this control.  Each element
	/// (layer) represents another layer to be drawn.
	/// This allows you to use multiple sprites to draw
	/// a single control, achieving a sort of layered
	/// effect. Ex: You can use a second layer to overlay 
	/// a button with a highlight effect.
	/// </summary>
	public SpriteRoot[] filledLayers = new SpriteRoot[0];
	public SpriteRoot[] emptyLayers = new SpriteRoot[0];


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[] filledIndices;
	protected int[] emptyIndices;

	
	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	public override void OnInput(ref POINTER_INFO ptr) {}

	public override void Start()
	{
		if (m_started)
			return;

		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[2][];
		aggregateLayers[0] = filledLayers;
		aggregateLayers[1] = emptyLayers;

		// Runtime init stuff:
		if(Application.isPlaying)
		{
			filledIndices = new int[filledLayers.Length];
			emptyIndices = new int[emptyLayers.Length];

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			for (int i = 0; i < filledLayers.Length; ++i)
			{
				if (filledLayers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}

				filledIndices[i] = filledLayers[i].GetStateIndex("filled");
				if (filledIndices[i] != -1)
					filledLayers[i].SetState(filledIndices[i]);
			}
			for (int i = 0; i < emptyLayers.Length; ++i)
			{
				if (emptyLayers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}

				emptyIndices[i] = emptyLayers[i].GetStateIndex("empty");
				if (emptyIndices[i] != -1)
					emptyLayers[i].SetState(emptyIndices[i]);
			}

			// Create our other sprite for the 
			// empty/background portion:
			GameObject emptyObj = new GameObject();
			emptyObj.name = name + " - Empty Bar";
			emptyObj.transform.parent = transform;
			emptyObj.transform.localPosition = Vector3.zero;
			emptyObj.transform.localScale = Vector3.one;
			emptyObj.transform.localRotation = Quaternion.identity;
			emptyObj.layer = gameObject.layer;
			emptySprite = (AutoSprite)emptyObj.AddComponent(typeof(AutoSprite));
			emptySprite.plane = plane;
			emptySprite.autoResize = autoResize;
			emptySprite.pixelPerfect = pixelPerfect;
			emptySprite.persistent = persistent;
			emptySprite.ignoreClipping = ignoreClipping;
			emptySprite.bleedCompensation = bleedCompensation;
			if (!managed)
				emptySprite.renderer.sharedMaterial = renderer.sharedMaterial;
			else
			{
				if (manager != null)
				{
					emptySprite.Managed = managed;
					manager.AddSprite(emptySprite);
					emptySprite.SetDrawLayer(drawLayer);	// Knob should be drawn in front of the bar
				}
				else
					Debug.LogError("Sprite on object \"" + name + "\" not assigned to a SpriteManager!");
			}
			emptySprite.color = color;
			emptySprite.SetAnchor(anchor);
			emptySprite.Setup(width, height, m_spriteMesh.material);
			if (states[1].spriteFrames.Length != 0)
			{
				emptySprite.animations = new UVAnimation[1];
				emptySprite.animations[0] = new UVAnimation();
				emptySprite.animations[0].SetAnim(states[1], 0);
				emptySprite.PlayAnim(0, 0);
			}
			emptySprite.renderCamera = renderCamera;
			emptySprite.Hide(IsHidden());

			Value = m_value;

			if (container != null)
				container.AddChild(emptyObj);

			SetState(0);
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);

		if (emptySprite == null)
			return;

		emptySprite.SetSize(width, height);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIProgressBar))
			return;

		if (Application.isPlaying)
		{
			UIProgressBar b = (UIProgressBar)s;


			if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
			{
				if (emptySprite != null)
					emptySprite.Copy(b.emptySprite);
			}
		}
	}


	// Sets the default UVs:
	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
			frameInfo.Copy(states[0].spriteFrames[0]);

		base.InitUVs();
	}

	// Updates the appearance of the bar
	// according to our progress value:
	protected void UpdateProgress()
	{
		this.TruncateRight(m_value);

		if(emptySprite != null)
			emptySprite.TruncateLeft(1f-m_value);

		// Truncate layers:
		for(int i=0; i<filledLayers.Length; ++i)
		{
			//if (filledIndices[i] != -1)
				filledLayers[i].TruncateRight(m_value);
		}
		for (int i = 0; i < emptyLayers.Length; ++i)
		{
			//if (emptyIndices[i] != -1)
				emptyLayers[i].TruncateLeft(1f-m_value);
		}
	}

	public override IUIContainer  Container
	{
		get 
		{ 
			 return base.Container;
		}
		
		set 
		{
			if (value != container)
			{
				if (container != null)
					container.RemoveChild(emptySprite.gameObject);

				if(value != null)
					if(emptySprite != null)
						value.AddChild(emptySprite.gameObject);
			}

			base.Container = value;
		}
	}


	public override void Unclip()
	{
		if (ignoreClipping)
			return;

		base.Unclip();
		emptySprite.Unclip();
	}

	public override bool Clipped
	{
		get
		{
			return base.Clipped;
		}
		set
		{
			if (ignoreClipping)
				return;

			base.Clipped = value;
			emptySprite.Clipped = value;
		}
	}

	public override Rect3D ClippingRect
	{
		get
		{
			return base.ClippingRect;
		}
		set
		{
			if (ignoreClipping)
				return;

			base.ClippingRect = value;
			emptySprite.ClippingRect = value;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIProgressBar Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIProgressBar)go.AddComponent(typeof(UIProgressBar));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIProgressBar Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIProgressBar)go.AddComponent(typeof(UIProgressBar));
	}


	public override void Hide(bool tf)
	{
		base.Hide(tf);

		if (emptySprite != null)
			emptySprite.Hide(tf);
	}

	public override void SetColor(Color c)
	{
		base.SetColor(c);

		if (emptySprite != null)
			emptySprite.SetColor(c);
	}
}
