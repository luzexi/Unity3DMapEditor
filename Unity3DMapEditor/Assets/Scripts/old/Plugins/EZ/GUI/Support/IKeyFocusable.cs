//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


/// <summary>
/// Definition of a delegate that is called when the user
/// "commits" the text value entered (usually either by pressing the
/// enter key when control is set to single-line, or when
/// the "Done" button is pressed on the iOS keyboard).
/// </summary>
/// <param name="content"></param>
public delegate void EZKeyboardCommitDelegate(IKeyFocusable control);


public interface IKeyFocusable 
{
	// Is called to inform a control that it has lost the
	// keyboard focus.
	void LostFocus();

	// For internal use only.
	// Sets the input text of the control as well as
	// the insertion point.
	// <param name="text">The input text of the control.</param>
	// <param name="insert">The index of the insertion point.</param>
	// <returns>Returns the text accepted which may be different from the text sent in the "text" argument.</returns>
	string SetInputText(string inputText, ref int insertPt);

	// For internal use only.
	// Gets the input text of the control (if any)
	// and returns the insertion point in the
	// reference variable "insert".
	// <param name="info">Will contain information about how the keyboard should be displayed (if iPhone) as well as the index of the insertion point.</param>
	// <returns>Returns the input text of the control.</returns>
	string GetInputText(ref KEYBOARD_INFO info);

	// For internal use only. Instructs the control to commit
	// the current input and invoke any methods or call any
	// delegates that want to know.
	void Commit();

	/// <summary>
	/// Sets a delegate to be called when the object's
	/// content is committed (usually either by pressing the
	/// enter key when control is set to single-line, or when
	/// the "Done" button is pressed on the iOS keyboard).
	/// </summary>
	/// <param name="del">Delegate to be called when the content is committed.</param>
	void SetCommitDelegate(EZKeyboardCommitDelegate del);

	/// <summary>
	/// Accessor that returns the textual content of
	/// the control.
	/// </summary>
	string Content
	{
		get;
	}

	/// <summary>
	/// Is called when the "up" arrow is pressed with the intent
	/// to move the insertion point upward.
	/// </summary>
	void GoUp();

	/// <summary>
	/// Is called when the "down" arrow is pressed with the intent
	/// to move the insertion point downward.
	/// </summary>
	void GoDown();
}
