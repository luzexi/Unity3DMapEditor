//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Class which allows single-line text input.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Text Field")]
public class UITextField : AutoSpriteControlBase, IKeyFocusable
{
	/// <summary>
	/// Delegate definition for method to be called when the
	/// field receives focus.
	/// </summary>
	/// <param name="field">A reference to the field receiving focus.</param>
	public delegate void FocusDelegate(UITextField field);

	/// <summary>
	/// Delegate definition for method to be called to validate
	/// the content of the text field.
	/// </summary>
	/// <param name="field">A reference to the text field for which text is to be validated.</param>
	/// <param name="text">The text to be validated.</param>
	/// <param name="insertionPoint">The index of where the insertion point/caret should be positioned in the resulting string (note that the end of the string would be equal to the string length).</param>
	/// <returns>The delegate should return the validated text, with any necessary changes.</returns>
	public delegate string ValidationDelegate(UITextField field, string text, ref int insertionPoint);

	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
		}
	}

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Field graphic"),
			new TextureAnim("Caret")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Transitions - Controls caret flash
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			null,
			new EZTransitionList( new EZTransition[]	// Caret flash
			{
				new EZTransition("Caret Flash")
			})
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
	/// The distance, in local units, inward from the edges of the
	/// control that the text should be clipped.
	/// </summary>
	public Vector2 margins;
	protected Rect3D clientClippingRect; // The clipping rect against which the text will be clipped
	// Corners of the margins
	protected Vector2 marginTopLeft;
	protected Vector2 marginBottomRight;

	/// <summary>
	/// The maximum number of characters the field can hold.
	/// A value of 0 is unlimited.
	/// </summary>
	public int maxLength;

	/// <summary>
	/// Whether or not this field will accept multi-line input.
	/// </summary>
	public bool multiline = false;

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

	/// <summary>
	/// The size, in local units, of the caret sprite.
	/// This can be left at default if using pixel-perfect.
	/// </summary>
	public Vector2 caretSize;

	/// <summary>
	/// The anchor method to be used by the caret.
	/// </summary>
	public SpriteRoot.ANCHOR_METHOD caretAnchor = ANCHOR_METHOD.BOTTOM_LEFT;

	/// <summary>
	/// The distance, in local units, that the caret will be
	/// offset from the insertion point.  Defaults to 0,0,-0.1
	/// to keep it from being hidden "behind" the text.
	/// </summary>
	public Vector3 caretOffset = new Vector3(0,0,-0.1f);

	/// <summary>
	/// Sets whether a caret should be shown when running on
	/// a mobile device (such as iOS).  It is recommended to
	/// keep this set to false since, for example, the keyboard
	/// on iOS has its own method of setting the insertion point,
	/// and Unity does not expose information about this insertion
	/// point, so if the EZ GUI caret is shown, there is no way for
	/// it to stay in sync with where the insertion point actually 
	/// is because Unity does not pass that information through.  
	/// So it is better to just leave the caret disabled for mobile
	/// devices and let the user use the built-in OS-specific text 
	/// entry interface.
	/// </summary>
	public bool showCaretOnMobile = false;

	/// <summary>
	/// When true, the caret/insertion point is moved to the
	/// position clicked by the user.
	/// This is ignored if there is no caret, or if the platform
	/// is a mobile device which does not support programmatic
	/// placement of the insertion point.
	/// </summary>
	public bool allowClickCaretPlacement = true;

	// Indicates whether the max length has been exceeded (see MaxLengthExceeded property)
	protected bool maxLengthExceeded = false;

#if UNITY_IPHONE || UNITY_ANDROID
	/// <summary>
	/// The type of keyboard to display. (iPhone OS only)
	/// </summary>
	#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
	public TouchScreenKeyboardType type;
	#else
	public iPhoneKeyboardType type;
	#endif

	/// <summary>
	/// Whether to use auto correction. (iPhone OS only)
	/// </summary>
	public bool autoCorrect;

/*
	Deprecated, now uses the password option:

	/// <summary>
	/// Whether the keyboard should be shown in secure mode. (iPhone OS only)
	/// </summary>
	public bool secure;
*/
	/// <summary>
	/// Whether the keyboard should be shown in alert mode. (iPhone OS only)
	/// </summary>
	public bool alert;

	/// <summary>
	/// Specifies if text input field above the keyboard will be hidden when the keyboard is on screen.
	/// </summary>
	public bool hideInput;
#endif

	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when enter is pressed (if single-line),
	/// or when "Done" is pressed on the iOS keyboard.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;

	/// <summary>
	/// A string containing the name of the method to be invoked
	/// when enter is pressed (if single-line), or when "Done" 
	/// is pressed on the iOS keyboard.
	/// </summary>
	public string methodToInvoke = "";

	// The delegate to be called when the user "commits"
	// the content of the text field.
	protected EZKeyboardCommitDelegate commitDelegate;

	// The delegate to be called to validate input.
	protected ValidationDelegate validationDelegate;

	/// <summary>
	/// Sound that will be played when the field receives keyboard input
	/// </summary>
	public AudioSource typingSoundEffect;

	/// <summary>
	/// Sound to play if typing has exceeded the
	/// field's length.
	/// </summary>
	public AudioSource fieldFullSound;

	/// <summary>
	/// When true, the field will not accept input from the hardware
	/// or OS-provided keyboard, and instead must be operated using
	/// a custom keyboard provided by you.
	/// </summary>
	public bool customKeyboard = false;

	/// <summary>
	/// When set to true, the commit delegate/method to invoke will
	/// be called when the field loses focus.
	/// </summary>
	public bool commitOnLostFocus = false;

	/// <summary>
	/// Event that should trigger a custom focus event,
	/// if using your own custom input method (not using
	/// a hardware or OS-provided keyboard).
	/// Otherwise, this setting has no effect.
	/// </summary>
	public POINTER_INFO.INPUT_EVENT customFocusEvent = POINTER_INFO.INPUT_EVENT.PRESS;

	// Sprite that will represent the caret.
	protected AutoSprite caret;

	// The delegate to be called when the field receives focus.
	protected FocusDelegate focusDelegate;

	// The text insertion point
	protected int insert;

	// Lets us keep track of whether we've moved
	protected Vector3 cachedPos;
	protected Quaternion cachedRot;
	protected Vector3 cachedScale;

	// state tracking:
	protected bool hasFocus = false;

	// Misc.
	protected Vector3 origTextPos;


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[,] stateIndices;


	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Check to see if we're disabled or hidden again in case
		// we were disabled by an input delegate:
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

#if !UNITY_IPHONE && !UNITY_ANDROID
		// Now check to see if the caret is being positioned:
		if (hasFocus)
		{
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS && caret != null && allowClickCaretPlacement)
			{
				// Cast the pointer's ray against our collider to
				// find the point clicked:
				RaycastHit hit;
				if (collider.Raycast(ptr.ray, out hit, ptr.rayDepth))
				{
					PositionInsertionPoint(hit.point);
				}
			}
		}
#endif

		// See if we got the focus
		if(ptr.evt == customFocusEvent)
		{
			// Call our focus delegate
			if (focusDelegate != null)
				focusDelegate(this);
		}

		base.OnInput(ref ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UITextField))
			return;

		UITextField b = (UITextField)s;


		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			maxLength = b.maxLength;
			multiline = b.multiline;
			password = b.password;
			maskingCharacter = b.maskingCharacter;
			customKeyboard = b.customKeyboard;
			customFocusEvent = b.customFocusEvent;
			margins = b.margins;

#if UNITY_IPHONE || UNITY_ANDROID
			type = b.type;
			autoCorrect = b.autoCorrect;
			alert = b.alert;
			hideInput = b.hideInput;
#endif
			typingSoundEffect = b.typingSoundEffect;
			fieldFullSound = b.fieldFullSound;
		}

		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = b.scriptWithMethodToInvoke;
			methodToInvoke = b.methodToInvoke;
		}

		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			caret.Copy(b.caret);

			caretSize = b.caretSize;
			caretOffset = b.caretOffset;
			caretAnchor = b.caretAnchor;
			showCaretOnMobile = b.showCaretOnMobile;
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			insert = b.insert;
			Text = b.Text;
		}

		SetMargins(margins);
	}


	public override bool GotFocus()
	{
		// Don't let the UIManager use the hardware or OS-provided
		// input devices if we're using a custom keyboard:
		if (customKeyboard)
			return false;

		hasFocus = m_controlIsEnabled;

		return m_controlIsEnabled;
	}

	public string GetInputText(ref KEYBOARD_INFO info)
	{
		info.insert = insert;
#if UNITY_IPHONE || UNITY_ANDROID
		info.type = type;
		info.autoCorrect = autoCorrect;
		info.multiline = multiline;
		info.secure = password;
		info.alert = alert;
		info.hideInput = hideInput;
#endif
		// Show our caret
		ShowCaret();

		return text;
	}

	public string SetInputText(string inputText, ref int insertPt)
	{
		// Validate our input:
		if(!multiline)
		{
			int idx;
			// Check for Enter:
			if ((idx = inputText.IndexOf('\n')) != -1)
			{
				inputText = inputText.Remove(idx, 1);
				UIManager.instance.FocusObject = null;
			}
			if ((idx = inputText.IndexOf('\r')) != -1)
			{
				inputText = inputText.Remove(idx, 1);
				UIManager.instance.FocusObject = null;
			}
		}

		// Apply custom validation
		if (validationDelegate != null)
			inputText = validationDelegate(this, inputText, ref insertPt);

		if (inputText.Length > maxLength && maxLength > 0)
		{
			// Set our changed delegate to null so we can
			// assign to our Text property without firing
			// it in order that the delegate gets the
			// correct value for maxLengthExceeded:
			EZValueChangedDelegate tempDel = changeDelegate;
			changeDelegate = null;

			Text = inputText.Substring(0, maxLength);
			insert = Mathf.Clamp(insertPt, 0, maxLength);

			maxLengthExceeded = true;
			changeDelegate = tempDel;

			if (changeDelegate != null)
				changeDelegate(this);

			if (fieldFullSound != null)
				fieldFullSound.PlayOneShot(fieldFullSound.clip);
		}
		else
		{
			Text = inputText;
			insert = insertPt;

			if (typingSoundEffect != null)
				typingSoundEffect.PlayOneShot(typingSoundEffect.clip);

			if (changeDelegate != null)
				changeDelegate(this);
		}

/*
		if (text.Length > 0)
		{
			if (caret != null)
				if (caret.IsHidden())
					ShowCaret();
		}
		else
			HideCaret();
*/

		if(caret != null)
		{
			if (caret.IsHidden() && hasFocus)
				caret.Hide(false);
		}
		// It's okay, PositionCaret() checks to see
		// if caret is null, and it performs some
		// non-caret functions too:
		PositionCaret();

		// See if enter was pressed and we didn't already handle it:
		if (UIManager.instance.FocusObject == null && !commitOnLostFocus)
			Commit();

		return text;
	}

	public void LostFocus()
	{
		if (commitOnLostFocus)
			Commit();

		hasFocus = false;

		// Hide our caret
		HideCaret();
	}

	public void Commit()
	{
		if (scriptWithMethodToInvoke != null && !string.IsNullOrEmpty(methodToInvoke))
			scriptWithMethodToInvoke.Invoke(methodToInvoke, 0);
		if (commitDelegate != null)
			commitDelegate(this);
	}

	/// <summary>
	/// Accessor that returns the textual content of
	/// the control.
	/// </summary>
	public string Content
	{
		get { return Text; }
	}

	protected void ShowCaret()
	{
		if (caret == null)
			return;

		// Recalculate our clipping rect:
		CalcClippingRect();

		caret.Hide(false);
		PositionCaret();

		// Make sure the caret is still showing:
		if (!caret.IsHidden())
		{
			transitions[1].list[0].Start();
			if (caret.animations.Length > 0)
				caret.DoAnim(0);
		}
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);

		if (caret != null)
		{
			if (!tf && hasFocus)
				caret.Hide(tf);
			else
				caret.Hide(true);
		}

		if (!tf)
			CalcClippingRect();
	}

	protected void HideCaret()
	{
		if (caret == null)
			return;

		transitions[1].list[0].StopSafe();
		caret.Hide(true);
	}

	// Used to ensure the text is always positioned within
	// the viewable area such that the insertion point is
	// always visible.  This is normally done in PositionCaret(),
	// but when a caret is not in use, we do it here.
	protected void PositionText(bool recur)
	{
		Vector3 pos = transform.InverseTransformPoint(spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert)));

		// See if the current character is in our viewable area:
		Vector3 top = pos + Vector3.up * spriteText.BaseHeight * spriteText.transform.localScale.y;

		if (recur)
		{
			if (multiline) // Only check top/bottom if we're multiline
			{
				if (top.y > marginTopLeft.y)
				{
					spriteText.transform.localPosition -= Vector3.up * spriteText.LineSpan;
					PositionText(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
				else if (pos.y < marginBottomRight.y)
				{
					spriteText.transform.localPosition += Vector3.up * spriteText.LineSpan;
					PositionText(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
			}
			else // Only check left/right if we're not multiline
			{
				if (pos.x < marginTopLeft.x)
				{
					Vector3 center = GetCenterPoint();
					// Move it so that the current character is in the middle:
					Vector3 newTxtPos = spriteText.transform.localPosition + Vector3.right * Mathf.Abs(center.x - pos.x);
					// Don't move right of its starting position:
					newTxtPos.x = Mathf.Min(newTxtPos.x, origTextPos.x);
					spriteText.transform.localPosition = newTxtPos;
					PositionText(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
				else if (pos.x > marginBottomRight.x)
				{
					Vector3 center = GetCenterPoint();
					// Move it so that the current character is in the middle:
					Vector3 newTxtPos = spriteText.transform.localPosition - Vector3.right * Mathf.Abs(center.x - pos.x);
					spriteText.transform.localPosition = newTxtPos;
					PositionText(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
			}
		}
	}

	protected void PositionCaret()
	{
		PositionCaret(true);
	}

	// recur tells us if we want to do a single
	// recursive positioning because our text
	// object moved as a result of an attempt
	// to keep the caret in the viewable area:
	protected void PositionCaret(bool recur)
	{
		if (spriteText == null)
			return;
		if (caret == null)
		{
			PositionText(true);
			return;
		}

		//insert = Mathf.Min(insert, spriteText.DisplayString.Length);

		Vector3 pos = transform.InverseTransformPoint(spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert)));

		// See if the current character is in our viewable area:
		Vector3 top = pos + Vector3.up * spriteText.BaseHeight * spriteText.transform.localScale.y;
		
		if(recur)
		{
			if(multiline) // Only check top/bottom if we're multiline
			{
				if (top.y > marginTopLeft.y)
				{
					spriteText.transform.localPosition -= Vector3.up * spriteText.LineSpan;
					PositionCaret(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
				else if (pos.y < marginBottomRight.y)
				{
					spriteText.transform.localPosition += Vector3.up * spriteText.LineSpan;
					PositionCaret(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
			}
			else // Only check left/right if we're not multiline
			{
				if (pos.x < marginTopLeft.x)
				{
					Vector3 center = GetCenterPoint();
					// Move it so that the current character is in the middle:
					Vector3 newTxtPos = spriteText.transform.localPosition + Vector3.right * Mathf.Abs(center.x - pos.x);
					// Don't move right of its starting position:
					newTxtPos.x = Mathf.Min(newTxtPos.x, origTextPos.x);
					spriteText.transform.localPosition = newTxtPos;
					PositionCaret(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
				else if (pos.x > marginBottomRight.x)
				{
					Vector3 center = GetCenterPoint();
					// Move it so that the current character is in the middle:
					Vector3 newTxtPos = spriteText.transform.localPosition - Vector3.right * Mathf.Abs(center.x - pos.x);
					spriteText.transform.localPosition = newTxtPos;
					PositionCaret(false);
					// Re-clip our text:
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
			}
		}

		transitions[1].list[0].StopSafe();

		caret.transform.localPosition = pos;

		transitions[1].list[0].Start();
		if(caret.animations.Length > 0)
			caret.DoAnim(0);

		// Re-clip:
		caret.ClippingRect = clientClippingRect;
	}

	// Accepts a point in world space and finds which
	// text character is nearest to it
	protected void PositionInsertionPoint(Vector3 pt)
	{
		if (caret == null || spriteText == null)
			return;

		insert = spriteText.DisplayIndexToPlainIndex(spriteText.GetNearestInsertionPoint(pt));
		UIManager.instance.InsertionPoint = insert;
		PositionCaret(true);

		/*
				pt = spriteText.transform.InverseTransformPoint(pt);

				Vector3[] verts = txtMesh.vertices;

				// Find the nearest vertex to the right of our point:
				float dist;
				float minDist = 99999f;
				int nearest;

				for(int i=0; i<txtMesh.vertexCount; ++i)
				{
					dist = verts[i].x - pt.x;
					if (dist < minDist)
					{
						minDist = dist;
						nearest = i;
					}
				}

				int[] tris = txtMesh.triangles;

				// Now find the triangle that contains this vertex:
				for(int i=0; i<tris.Length; ++i)
				{

				}
		*/
	}

	/// <summary>
	/// Moves the insertion point up one line (called when the
	/// "up" arrow is pressed).
	/// </summary>
	public void GoUp()
	{
		Vector3 pos = spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert));
		pos += spriteText.transform.up * spriteText.LineSpan * spriteText.transform.lossyScale.y;
		insert = spriteText.DisplayIndexToPlainIndex(spriteText.GetNearestInsertionPoint(pos));
		UIManager.instance.InsertionPoint = insert;
		PositionCaret(true);
	}

	/// <summary>
	/// Moves the insertion point up one line (called when the
	/// "up" arrow is pressed).
	/// </summary>
	public void GoDown()
	{
		Vector3 pos = spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert));
		pos -= spriteText.transform.up * spriteText.LineSpan * spriteText.transform.lossyScale.y;
		insert = spriteText.DisplayIndexToPlainIndex(spriteText.GetNearestInsertionPoint(pos));
		UIManager.instance.InsertionPoint = insert;
		PositionCaret(true);
	}

	/// <summary>
	/// Sets the delegate to be called when the content of the field
	/// is "committed" (when the return key is pressed on a single-line
	/// field, or when the "Done" button is pressed on an iOS keyboard).
	/// NOTE: This will unset any previously assigned commit delegate.
	/// </summary>
	/// <param name="del">The delegate to be called.</param>
	public void SetCommitDelegate(EZKeyboardCommitDelegate del)
	{
		commitDelegate = del;
	}

	/// <summary>
	/// Adds a delegate to be called when the content of the field
	/// is "committed" (when the return key is pressed on a single-line
	/// field, or when the "Done" button is pressed on an iOS keyboard).
	/// </summary>
	/// <param name="del">The delegate to be called.</param>
	public void AddCommitDelegate(EZKeyboardCommitDelegate del)
	{
		commitDelegate += del;
	}

	/// <summary>
	/// Removes a delegate previously added or set by AddCommitDelegate()
	/// or SetCommitDelegate().
	/// </summary>
	/// <param name="del">The delegate to be removed.</param>
	public void RemoveCommitDelegate(EZKeyboardCommitDelegate del)
	{
		commitDelegate -= del;
	}

	/// <summary>
	/// Sets the delegate to be called when the field receives the focus
	/// as determined by the customFocusEvent setting.
	/// NOTE: This will unset any previously assigned focus delegate.
	/// </summary>
	/// <param name="del">The delegate to be called.</param>
	public void SetFocusDelegate(FocusDelegate del)
	{
		focusDelegate = del;
	}

	/// <summary>
	/// Adds a delegate to be called when the field receives the focus
	/// as determined by the customFocusEvent setting.
	/// </summary>
	/// <param name="del">The delegate to be called.</param>
	public void AddFocusDelegate(FocusDelegate del)
	{
		focusDelegate += del;
	}

	/// <summary>
	/// Removes a delegate previously added or set by AddFocusDelegate()
	/// or SetFocusDelegate().
	/// </summary>
	/// <param name="del">The delegate to be removed.</param>
	public void RemoveFocusDelegate(FocusDelegate del)
	{
		focusDelegate -= del;
	}

	/// <summary>
	/// Sets the delegate to be called when the field receives input.
	/// This delegate validates the input text.
	/// NOTE: This will unset any previously assigned validation delegate.
	/// </summary>
	/// <param name="del">The delegate to be called.</param>
	public void SetValidationDelegate(ValidationDelegate del)
	{
		validationDelegate = del;
	}

	/// <summary>
	/// Adds a delegate to be called when the field receives input.
	/// This delegate validates the input text.
	/// </summary>
	/// <param name="del">The delegate to be called.</param>
	public void AddValidationDelegate(ValidationDelegate del)
	{
		validationDelegate += del;
	}

	/// <summary>
	/// Removes a delegate previously added or set by AddValidationDelegate()
	/// or SetValidationDelegate().
	/// </summary>
	/// <param name="del">The delegate to be removed.</param>
	public void RemoveValidationDelegate(ValidationDelegate del)
	{
		validationDelegate -= del;
	}


	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected override void Awake()
	{
		base.Awake();

		defaultTextAlignment = SpriteText.Alignment_Type.Left;
		defaultTextAnchor = SpriteText.Anchor_Pos.Upper_Left;
	}

	public override void Start()
	{
		if (m_started)
			return;

		base.Start();

		// Create a TextMesh object if none exists:
		if (spriteText == null)
		{
			Text = " ";
			Text = "";
		}

		if(spriteText != null)
		{
			spriteText.password = password;
			spriteText.maskingCharacter = maskingCharacter;
			spriteText.multiline = multiline;
			origTextPos = spriteText.transform.localPosition;
			SetMargins(margins);
		}

		// Set the insertion point to the end by default:
		insert = Text.Length;

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
			// See if we should create our caret:
			if (Application.platform == RuntimePlatform.IPhonePlayer ||
				Application.platform == RuntimePlatform.Android)
			{
				if (showCaretOnMobile)
					CreateCaret();
			}
			else
#endif
				CreateCaret();
		}

		cachedPos = transform.position;
		cachedRot = transform.rotation;
		cachedScale = transform.lossyScale;
		CalcClippingRect();

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	protected void CreateCaret()
	{
		// Create our caret and hide it by default:
		GameObject go = new GameObject();
		go.name = name + " - caret";
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		go.layer = gameObject.layer;
		caret = (AutoSprite)go.AddComponent(typeof(AutoSprite));
		caret.plane = plane;
		caret.offset = caretOffset;
		caret.SetAnchor(caretAnchor);
		caret.persistent = persistent;
		if (!managed)
		{
			if (caret.spriteMesh != null)
				((SpriteMesh)caret.spriteMesh).material = renderer.sharedMaterial;
		}
		else
		{
			if (manager != null)
			{
				caret.Managed = managed;
				manager.AddSprite(caret);
				caret.SetDrawLayer(drawLayer + 1);	// Caret should be drawn in front of the field graphic
			}
			else
				Debug.LogError("Sprite on object \"" + name + "\" not assigned to a SpriteManager!");
		}
		caret.autoResize = autoResize;
		if (pixelPerfect)
			caret.pixelPerfect = pixelPerfect;
		else
			caret.SetSize(caretSize.x, caretSize.y);

		if (states[1].spriteFrames.Length != 0)
		{
			caret.animations = new UVAnimation[1];
			caret.animations[0] = new UVAnimation();
			caret.animations[0].SetAnim(states[1], 0);
			caret.PlayAnim(0, 0);
		}
		caret.renderCamera = renderCamera;
		caret.SetCamera(renderCamera);
		caret.Hide(true);
		transitions[1].list[0].MainSubject = caret.gameObject;

		PositionCaret();

		if (container != null)
			container.AddSubject(caret.gameObject);

		if (autoResize)
		{
			// Force start and resize since it would not have
			// taken effect earlier since the UVs were not
			// yet initialized:
			caret.Start();
			caret.SetSize(caretSize.x, caretSize.y);
		}
	}

	// Calculates the clipping rect for the text
	public void CalcClippingRect()
	{
		if (spriteText == null)
			return;

		Vector3 tl = marginTopLeft;
		Vector3 br = marginBottomRight;

		// Clamp the client rect to any clipping rect we may have:
		if(clipped)
		{
			Vector3 origTL = tl;
			Vector3 origBR = br;
			tl.x = Mathf.Clamp(localClipRect.x, origTL.x, origBR.x);
			br.x = Mathf.Clamp(localClipRect.xMax, origTL.x, origBR.x);
			tl.y = Mathf.Clamp(localClipRect.yMax, origBR.y, origTL.y);
			br.y = Mathf.Clamp(localClipRect.y, origBR.y, origTL.y);
		}

		clientClippingRect.FromRect(Rect.MinMaxRect(tl.x, br.y, br.x, tl.y));
		clientClippingRect.MultFast(transform.localToWorldMatrix);

		spriteText.ClippingRect = clientClippingRect;
		if (caret != null)
			caret.ClippingRect = clientClippingRect;
	}

	// Handles an EZAnimation tween
	public void OnEZTranslated()
	{
		CalcClippingRect();
	}

	// Handles an EZAnimation tween
	public void OnEZRotated()
	{
		CalcClippingRect();
	}

	// Handles an EZAnimation tween
	public void OnEZScaled()
	{
		CalcClippingRect();
	}

	/// <summary>
	/// Sets the margins to the specified values.
	/// </summary>
	/// <param name="marg">The distance, in local units, in from the edges of
	/// the control where the text within should be clipped.</param>
	public void SetMargins(Vector2 marg)
	{
		margins = marg;
		Vector3 center = GetCenterPoint();
		marginTopLeft = new Vector3(center.x + margins.x - width * 0.5f, center.y - margins.y + height * 0.5f, 0);
		marginBottomRight = new Vector3(center.x - margins.x + width * 0.5f, center.y + margins.y - height * 0.5f, 0);

		if (multiline)
		{
			float distanceToMargin = 0;

			switch (spriteText.anchor)
			{
				case SpriteText.Anchor_Pos.Upper_Left:
				case SpriteText.Anchor_Pos.Middle_Left:
				case SpriteText.Anchor_Pos.Lower_Left:
					distanceToMargin = marginBottomRight.x - origTextPos.x;
					break;
				case SpriteText.Anchor_Pos.Upper_Center:
				case SpriteText.Anchor_Pos.Middle_Center:
				case SpriteText.Anchor_Pos.Lower_Center:
					distanceToMargin = ((marginBottomRight.x - marginTopLeft.x) * 2f) - 2f * Mathf.Abs(origTextPos.x);
					break;
				case SpriteText.Anchor_Pos.Upper_Right:
				case SpriteText.Anchor_Pos.Middle_Right:
				case SpriteText.Anchor_Pos.Lower_Right:
					distanceToMargin = origTextPos.x - marginTopLeft.x;
					break;
			}

			// Adjust for scale:
			spriteText.maxWidth = (1f / spriteText.transform.localScale.x) * distanceToMargin;
		}
		else
		{
			if (spriteText != null)
				spriteText.maxWidth = 0;
		}
	}


	// Sets the default UVs:
	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
			frameInfo.Copy(states[0].spriteFrames[0]);

		base.InitUVs();
	}


	/// <summary>
	/// When true, indicates that the user has typed more characters
	/// than the max length setting will allow.  This is automatically
	/// unset when the user removes characters.
	/// This flag is only used if maxLength is non-zero.
	/// </summary>
	public bool MaxLengthExceeded
	{
		get { return maxLengthExceeded; }
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
				if (container != null && caret != null)
					container.RemoveChild(caret.gameObject);

				if (value != null && caret != null)
					value.AddChild(caret.gameObject);
			}

			base.Container = value;
		}
	}


	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool newText = (spriteText == null);

			if (Application.isPlaying && !m_started)
				Start();

			// See if the insertion point is at the end
			// of the text:
			bool caretAtEnd;
			if (text != null)
				caretAtEnd = insert == text.Length;
			else
				caretAtEnd = true;

			base.Text = value;

			// Check the length:
			if (maxLength > 0)
			{
				if (value.Length > maxLength)
					maxLengthExceeded = true;
				else
					maxLengthExceeded = false;
			}

			if (newText && spriteText != null)
			{
				spriteText.transform.localPosition = new Vector4(width * -0.5f + margins.x, height * 0.5f + margins.y);
				spriteText.removeUnsupportedCharacters = true;
				spriteText.parseColorTags = false;
				spriteText.multiline = multiline;
			}

			if (cachedPos != transform.position ||
				cachedRot != transform.rotation ||
				cachedScale != transform.lossyScale)
			{
				cachedPos = transform.position;
				cachedRot = transform.rotation;
				cachedScale = transform.lossyScale;
				CalcClippingRect();
			}

			// Keep the caret at the end:
			if (caretAtEnd)
				insert = Text.Length;

			PositionCaret();

			if (changeDelegate != null)
				changeDelegate(this);
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UITextField Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UITextField)go.AddComponent(typeof(UITextField));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UITextField Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UITextField)go.AddComponent(typeof(UITextField));
	}



	public override void Unclip()
	{
		if (ignoreClipping)
			return;

		base.Unclip();
		CalcClippingRect();
	}

	public override Rect3D ClippingRect
	{
		get { return base.ClippingRect; }
		set
		{
			if (ignoreClipping)
				return;

			base.ClippingRect = value;
			CalcClippingRect();
		}
	}


	public override bool Clipped
	{
		get { return base.Clipped; }
		set
		{
			if (ignoreClipping)
				return;

			base.Clipped = value;
			CalcClippingRect();
		}
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}



	public override void OnDrawGizmosSelected()
	{
 		base.OnDrawGizmosSelected();

//		Vector3 ul, ll, lr, ur;

// 		ul = (transform.position - transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));
// 		ll = (transform.position - transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));
// 		lr = (transform.position + transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));
// 		ur = (transform.position + transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));

		Gizmos.color = new Color(1f, 0, 0.5f, 1f);
// 		Gizmos.DrawLine(ul, ll);	// Left
// 		Gizmos.DrawLine(ll, lr);	// Bottom
// 		Gizmos.DrawLine(lr, ur);	// Right
// 		Gizmos.DrawLine(ur, ul);	// Top

		Gizmos.DrawLine(clientClippingRect.topLeft, clientClippingRect.bottomLeft);	// Left
		Gizmos.DrawLine(clientClippingRect.bottomLeft, clientClippingRect.bottomRight);	// Bottom
		Gizmos.DrawLine(clientClippingRect.bottomRight, clientClippingRect.topRight);	// Right
		Gizmos.DrawLine(clientClippingRect.topRight, clientClippingRect.topLeft);	// Top
	}


	public override void DoMirror()
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
			mirror = new UITextFieldMirror();
			mirror.Mirror(this);
		}

		mirror.Validate(this);

		// Compare our mirrored settings to the current settings
		// to see if something was changed:
		if (mirror.DidChange(this))
		{
			Init();
			mirror.Mirror(this);	// Update the mirror
		}
	}
}


public class UITextFieldMirror : AutoSpriteControlBaseMirror
{
	public Vector2 margins;
	public bool multiline;

	// Mirrors the specified SpriteText's settings
	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);
		UITextField tf = (UITextField)s;
		margins = tf.margins;
		multiline = tf.multiline;
	}

	// Validates certain settings:
	public override bool Validate(SpriteRoot s)
	{
		return base.Validate(s);
	}

	// Returns true if any of the settings do not match:
	public override bool DidChange(SpriteRoot s)
	{
		UITextField tf = (UITextField)s;
		if (margins.x != tf.margins.x ||
			margins.y != tf.margins.y ||
			width != tf.width ||
			height != tf.height)
		{
			tf.SetMargins(tf.margins);
			tf.CalcClippingRect();
			margins = tf.margins;
			// Keep it to ourselves since we handled it
			//return true;
		}
		if (multiline != tf.multiline)
		{
			if (tf.spriteText != null)
				tf.spriteText.multiline = tf.multiline;
			return true;
		}

		return base.DidChange(s);
	}
}