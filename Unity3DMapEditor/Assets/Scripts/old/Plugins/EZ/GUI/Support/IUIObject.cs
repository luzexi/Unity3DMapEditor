//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------



using UnityEngine;
using System.Collections;


/// <remarks>
/// Definition of a delegate type that is used as a pattern for methods that can be called when input is received.
/// </remarks>
/// <param name="ptr">Reference to the POINTER_INFO object that contains informaiton about the input event.</param>
public delegate void EZInputDelegate(ref POINTER_INFO ptr);

/// <remarks>
/// Definition of a delegate type that is used as a pattern for methods that can be called when the value of a control is changed.
/// </remarks>
/// <param name="obj">Reference to the control whose value changed.</param>
public delegate void EZValueChangedDelegate(IUIObject obj);


/// <remarks>
/// A generic interface for a UI element.
/// </remarks>
public interface IUIObject : IEZDragDrop
{
	/// <summary>
	/// Controls whether this control is in an enabled
	/// state or not. If it is not, input is not processed.
	/// This can also be used to cause a control to take on
	/// a "grayed out" appearance when disabled.
	/// </summary>
	bool controlIsEnabled
	{
		get;
		set;
	}

	/// <summary>
	/// When set to true, the control will instruct any
	/// pointers which have it as their target to
	/// de-target them.  Use this if you are deactivating
	/// a control and want no input to go to it.
	/// It is strongly recommended NOT to use this feature
	/// on any control that appears in a scroll list, or
	/// else you may be unable to scroll past the edge of
	/// the list's viewable area.
	/// </summary>
	bool DetargetOnDisable
	{
		get;
		set;
	}

	// Allows an object to act as a proxy for other
	// controls - i.e. a UIVirtualScreen
	IUIObject GetControl(ref POINTER_INFO ptr);

	// Accessor for getting/setting a reference to the
	// object that contains this one.
	IUIContainer Container
	{
		get;
		set;
	}

	GameObject gameObject
	{
		get;
	}

	Transform transform
	{
		get;
	}

	string ToString();

	string name
	{
		get;
	}

	// Requests that the IUIObject accept the specified
	// container as its container.  It should search
	// up the hierarchy of parent objects first to see
	// if a more immediate parent is a container, and
	// if so, reject the request.
	// <param name="cont">A reference to the object that is requesting containership.</param>
	// <returns>True if succeeded, false if a closer container was found.</returns>
	bool RequestContainership(IUIContainer cont);

	// <summary>
	// Is called when a control receives the keyboard focus.
	// </summary>
	// <returns>The object should return true if it can process 
	// keyboard input (will result on displaying an on-screen 
	// keyboard on iPhone OS devices), or false if it cannot.</returns>
	bool GotFocus();

	/// <summary>
	/// This is where input handling code should go in any derived class.
	/// </summary>
	/// <param name="ptr">POINTER_INFO struct that contains information on the pointer that caused the event, as well as the event that occurred.</param>
	void OnInput(POINTER_INFO ptr);

	/// <summary>
	/// Sets the method to be called when input occurs (input is forwarded from OnInput()).
	/// NOTE: This will replace any and all delegates which have been set or added previously.  If you are unsure
	/// if any delegates are already registered, use AddInputDelegate() instead, or RemoveInputDelegate() to unset
	/// a previously registered delegate.  Only use this when you are sure you want to replace all previously registered delegates.
	/// </summary>
	/// <param name="del">A method that conforms to the EZInputDelegate pattern.</param>
	void SetInputDelegate(EZInputDelegate del);

	/// <summary>
	/// Adds a method to be called when input occurs (input is forwarded from OnInput()).
	/// </summary>
	/// <param name="del">A method that conforms to the EZInputDelegate pattern.</param>
	void AddInputDelegate(EZInputDelegate del);

	/// <summary>
	/// Removes a method added with AddInputDelegate().
	/// </summary>
	/// <param name="del">A method that conforms to the EZInputDelegate pattern.</param>
	void RemoveInputDelegate(EZInputDelegate del);

	/// <summary>
	/// Sets the method to be called when the value of a control changes (such as a checkbox changing from false to true, or a slider being moved).
	/// NOTE: This will replace any and all delegates which have been set or added previously.  If you are unsure
	/// if any delegates are already registered, use AddValueChangedDelegate() instead, or RemoveValueChangedDelegate() to unset
	/// a previously registered delegate.  Only use this when you are sure you want to replace all previously registered delegates.
	/// </summary>
	/// <param name="del">A method that conforms to the EZValueChangedDelegate pattern.</param>
	void SetValueChangedDelegate(EZValueChangedDelegate del);

	/// <summary>
	/// Adds a method to be called when the value of a control changes (such as a checkbox changing from false to true, or a slider being moved).
	/// </summary>
	/// <param name="del">A method that conforms to the EZValueChangedDelegate pattern.</param>
	void AddValueChangedDelegate(EZValueChangedDelegate del);

	/// <summary>
	/// Removes a method added with AddValueChangedDelegate().
	/// </summary>
	/// <param name="del">A method that conforms to the EZValueChangedDelegate pattern.</param>
	void RemoveValueChangedDelegate(EZValueChangedDelegate del);
}
