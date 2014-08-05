//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Slider control that allows the setting of a value
/// between 0-1 by sliding the knob of the bar from
/// left to right, respectively. Can also be a vertical
/// slider by rotating the control.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Slider")]
public class UISlider : AutoSpriteControlBase
{
	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			// Forward to our knob:
			if(knob != null)
				knob.controlIsEnabled = value;
		}
	}


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
			float prevVal = m_value;
			m_value = Mathf.Clamp01(value);
			
			if(m_value != prevVal)
				UpdateValue();
		}
	}

	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when the slider is moved.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;

	/// <summary>
	/// A string containing the name of the method to be invoked.
	/// </summary>
	public string methodToInvoke = "";

	/// <summary>
	/// The default value of the slider:
	/// </summary>
	public float defaultValue;

	/// <summary>
	/// The distance, in local space, from
	/// the edge of the slider bar to stop
	/// the knob from moving.
	/// </summary>
	public float stopKnobFromEdge;

	/// <summary>
	/// The distance the knob should be offset from
	/// the bar.  Defaults to 0.1 units in front of
	/// the bar.  Use this to avoid the bar from
	/// being drawn in front of the knob.
	/// </summary>
	public Vector3 knobOffset = new Vector3(0,0,-0.1f);

	/// <summary>
	/// Dimensions of the knob in local space.
	/// </summary>
	public Vector2 knobSize;

	/// <summary>
	/// Factor by which the knob's size will be multiplied to
	/// determine the size of the knob collider.
	/// Use this to make the knob collider larger or smaller
	/// than the knob itself.  Ex: If you want a collider that
	/// is 2x the size of the knob itself, set this to 2,2.
	/// If you want a collider that is twice as tall as the knob
	/// itself, but the same width as the knob, set this to 1,2.
	/// </summary>
	public Vector2 knobColliderSizeFactor = new Vector2(1f, 1f);


	// References to the sprite that will be used
	// to draw the knob and empty portion of the 
	// bar:
	protected AutoSprite emptySprite;

	// Reference to slider knob:
	protected UIScrollKnob knob;

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Filled bar"),
			new TextureAnim("Empty bar"),
			new TextureAnim("Knob, Normal"),
			new TextureAnim("Knob, Over"),
			new TextureAnim("Knob, Active")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			null,
			null,
			new EZTransitionList( new EZTransition[]	// Knob Normal
			{
				new EZTransition("From Over"),
				new EZTransition("From Active"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// Knob Over
			{
				new EZTransition("From Normal"),
				new EZTransition("From Active")
			}),
			new EZTransitionList( new EZTransition[]	// Knob Active
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over")
			}),
		};

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= transitions.Length)
			return null;
		return transitions[index];
	}

	public override EZTransitionList[] Transitions
	{
		get { return transitions; }
		set { transitions = value; }
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
	public SpriteRoot[] knobLayers = new SpriteRoot[0];

	// Values which keep our truncation within a range
	// that takes into account where the knob will stop
	// at either end of the bar:
	protected float truncFloor;
	protected float truncRange;


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[] filledIndices;
	protected int[] emptyIndices;
	
	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected override void Awake()
	{
		base.Awake();

		m_value = defaultValue;
	}

	public override void Start()
	{
		if (m_started)
			return;

		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[2][];
		aggregateLayers[0] = filledLayers;
		aggregateLayers[1] = emptyLayers;
		// The knob will handle its own layers

		// Runtime init stuff:
		if(Application.isPlaying)
		{
			// Calculate our truncation floor and range:
			truncFloor = stopKnobFromEdge / width;
			truncRange = 1f - truncFloor * 2f;

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

			// Create our knob:
			GameObject go = new GameObject();
			go.name = name + " - Knob";
			go.transform.parent = transform;
			go.transform.localPosition = CalcKnobStartPos();
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			go.layer = gameObject.layer;
			knob = (UIScrollKnob) go.AddComponent(typeof(UIScrollKnob));
			knob.plane = plane;
			knob.SetOffset(knobOffset);
			knob.persistent = persistent;
			knob.bleedCompensation = bleedCompensation;
			if (!managed)
			{
				if (knob.spriteMesh != null)
					((SpriteMesh)knob.spriteMesh).material = renderer.sharedMaterial;
			}
			else
			{
				if (manager != null)
				{
					knob.Managed = managed;
					manager.AddSprite(knob);
					knob.SetDrawLayer(drawLayer + 1);	// Knob should be drawn in front of the bar

					// Force it to update its UVs:
					knob.SetControlState(UIButton.CONTROL_STATE.ACTIVE);	// We have to change to active so that when we set to normal, it isn't considered a redundant change and ignored.
					knob.SetControlState(UIButton.CONTROL_STATE.NORMAL);
				}
				else
					Debug.LogError("Sprite on object \"" + name + "\" not assigned to a SpriteManager!");
			}

			if (pixelPerfect)
			{
				knob.pixelPerfect = true;

			}
			else
				knob.SetSize(knobSize.x, knobSize.y);

			knob.ignoreClipping = ignoreClipping;

			knob.color = color;
			knob.SetColliderSizeFactor(knobColliderSizeFactor);
			knob.SetSlider(this);
			knob.SetMaxScroll(width - (stopKnobFromEdge * 2f));
			knob.SetInputDelegate(inputDelegate);
			// Setup knob's transitions:
			knob.transitions[0] = transitions[2];
			knob.transitions[1] = transitions[3];
			knob.transitions[2] = transitions[4];

			// Tell the knob what it will look like:
			knob.layers = knobLayers;
			// Child the knob's layers to the knob:
			for (int i = 0; i < knobLayers.Length; ++i)
				knobLayers[i].transform.parent = knob.transform;
			knob.animations[0].SetAnim(states[2], 0);
			knob.animations[1].SetAnim(states[3], 1);
			knob.animations[2].SetAnim(states[4], 2);

			knob.SetupAppearance();
			knob.SetCamera(renderCamera);
			knob.Hide(IsHidden());

			knob.autoResize = autoResize;

			// Create our other sprite for the 
			// empty/background portion:
			go = new GameObject();
			go.name = name + " - Empty Bar";
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			go.layer = gameObject.layer;
			emptySprite = (AutoSprite) go.AddComponent(typeof(AutoSprite));
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
			
			// Add our child objects as children
			// of our container:
			if(container != null)
			{
				container.AddChild(knob.gameObject);
				container.AddChild(emptySprite.gameObject);
			}

			SetState(0);

			// Force the value to update:
			m_value = -1f;
			Value = defaultValue;
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

		if(knob == null)
			return;

		knob.SetStartPos(CalcKnobStartPos());
		knob.SetMaxScroll(width - (stopKnobFromEdge * 2f));
		knob.SetPosition(m_value);
		emptySprite.SetSize(width, height);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UISlider))
			return;

		UISlider b = (UISlider)s;


		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = b.scriptWithMethodToInvoke;
			methodToInvoke = b.methodToInvoke;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			defaultValue = b.defaultValue;
			stopKnobFromEdge = b.stopKnobFromEdge;
			knobOffset = b.knobOffset;
			knobSize = b.knobSize;
			knobColliderSizeFactor = b.knobColliderSizeFactor;
		}

		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			if (Application.isPlaying)
			{
				if (emptySprite != null)
					emptySprite.Copy(b.emptySprite);

				if (knob != null)
					knob.Copy(b.knob);

				truncFloor = b.truncFloor;
				truncRange = b.truncRange;
			}
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			CalcKnobStartPos();
			Value = b.Value;
		}
	}


	// Calculates where to place the knob's start position
	// relative to the parent control's transform:
	protected Vector3 CalcKnobStartPos()
	{
		Vector3 pos = Vector3.zero;
		
		switch(anchor)
		{
			case ANCHOR_METHOD.TEXTURE_OFFSET:
				pos.x = (width * -0.5f) + stopKnobFromEdge;
				break;
			case ANCHOR_METHOD.UPPER_LEFT:
				pos.x = stopKnobFromEdge;
				pos.y = height * -0.5f;
				break;
			case ANCHOR_METHOD.UPPER_CENTER:
				pos.x = (width * -0.5f) + stopKnobFromEdge;
				pos.y = height * -0.5f;
				break;
			case ANCHOR_METHOD.UPPER_RIGHT:
				pos.x = (width * -1f) + stopKnobFromEdge;
				pos.y = height * -0.5f;
				break;
			case ANCHOR_METHOD.MIDDLE_LEFT:
				pos.x = stopKnobFromEdge;
				break;
			case ANCHOR_METHOD.MIDDLE_CENTER:
				pos.x = (width * -0.5f) + stopKnobFromEdge;
				break;
			case ANCHOR_METHOD.MIDDLE_RIGHT:
				pos.x = (width * -1f) + stopKnobFromEdge;
				break;
			case ANCHOR_METHOD.BOTTOM_LEFT:
				pos.x = stopKnobFromEdge;
				pos.y = height * 0.5f;
				break;
			case ANCHOR_METHOD.BOTTOM_CENTER:
				pos.x = (width * -0.5f) + stopKnobFromEdge;
				pos.y = height * 0.5f;
				break;
			case ANCHOR_METHOD.BOTTOM_RIGHT:
				pos.x = (width * -1f) + stopKnobFromEdge;
				pos.y = height * 0.5f;
				break;
		}

		return pos;
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
	protected void UpdateValue()
	{
		if (knob == null)
			return;

		float truncVal = truncFloor + m_value * truncRange;

		UpdateAppearance(truncVal);
		
		knob.SetPosition(m_value);

		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvoke, 0);
		if (changeDelegate != null)
			changeDelegate(this);
	}

	// Called by the knob when it moves.
	public void ScrollKnobMoved(UIScrollKnob knob, float val)
	{
		m_value = val;
		float truncVal = truncFloor + m_value * truncRange;

		UpdateAppearance(truncVal);

		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvoke, 0);
		if (changeDelegate != null)
			changeDelegate(this);
	}

	public override void SetInputDelegate(EZInputDelegate del)
	{
		if(knob != null)
			knob.SetInputDelegate(del);

		base.SetInputDelegate(del);
	}

	public override void AddInputDelegate(EZInputDelegate del)
	{
		if (knob != null)
			knob.AddInputDelegate(del);

		base.AddInputDelegate(del);
	}

	public override void RemoveInputDelegate(EZInputDelegate del)
	{
		if (knob != null)
			knob.RemoveInputDelegate(del);

		base.RemoveInputDelegate(del);
	}


	// Called to update the slider bar's appearance
	protected void UpdateAppearance(float truncVal)
	{
		this.TruncateRight(truncVal);

		if(emptySprite != null)
			emptySprite.TruncateLeft(1f - truncVal);

		// Truncate layers:
		for (int i = 0; i < filledLayers.Length; ++i)
		{
			//if (filledIndices[i] != -1)
				filledLayers[i].TruncateRight(truncVal);
		}
		for (int i = 0; i < emptyLayers.Length; ++i)
		{
			//if (emptyIndices[i] != -1)
				emptyLayers[i].TruncateLeft(1f - truncVal);
		}
	}

	/// <summary>
	/// Returns a reference to the slider's knob.
	/// </summary>
	/// <returns>A reference to the UIScrollKnob that serves as the slider's knob. Null if this object has not yet been created (means that Start() has not yet run).</returns>
	public UIScrollKnob GetKnob()
	{
		return knob;
	}

	public override IUIContainer Container
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
				{
					container.RemoveChild(emptySprite.gameObject);
					container.RemoveChild(knob.gameObject);
				}

				if(value != null)
				{
					if(emptySprite != null)
						value.AddChild(emptySprite.gameObject);
					if(knob != null)
						value.AddChild(knob.gameObject);
				}
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
		knob.Unclip();
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
			knob.Clipped = value;
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
			knob.ClippingRect = value;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UISlider Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UISlider)go.AddComponent(typeof(UISlider));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UISlider Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UISlider)go.AddComponent(typeof(UISlider));
	}


	public override void Hide(bool tf)
	{
		base.Hide(tf);

		if (emptySprite != null)
			emptySprite.Hide(tf);
		if (knob != null)
			knob.Hide(tf);
	}

	public override void SetColor(Color c)
	{
		base.SetColor(c);

		if (emptySprite != null)
			emptySprite.SetColor(c);
		if (knob != null)
			knob.SetColor(c);
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}
}
