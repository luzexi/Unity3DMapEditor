using UnityEngine;
using System.Collections;



// Interface to help us draw our script selection GUI.
public interface IGUIScriptSelector
{
	MonoBehaviour DrawScriptSelection(MonoBehaviour script, ref string method);
}



/// <remarks>
/// Flags that indicate which portion(s)
/// of the control should be copied.
/// </remarks>
public enum ControlCopyFlags
{
	/// <summary>
	/// Copies those attributes which determine
	/// the control's appearance.
	/// </summary>
	Appearance = 0x01,

	/// <summary>
	/// Copies the sound settings of the control.
	/// </summary>
	Sound = 0x02,

	/// <summary>
	/// Copies the text of the control.
	/// </summary>
	Text = 0x04,

	/// <summary>
	/// Copies the associated data reference of
	/// the control.
	/// </summary>
	Data = 0x08,

	/// <summary>
	/// Copies the state of the control.
	/// </summary>
	State = 0x10,

	/// <summary>
	/// Copies the transitions of the control.
	/// </summary>
	Transitions = 0x20,

	/// <summary>
	/// Copies those attributes which relate to
	/// invocation and delegates.
	/// </summary>
	Invocation = 0x40,

	/// <summary>
	/// Copies those attributes which relate to
	/// all other settings.
	/// </summary>
	Settings = 0x80,

	/// <summary>
	/// Copies all attributes related to drag & drop.
	/// </summary>
	DragDrop = 0x100,

	/// <summary>
	/// Copies all attributes of the control
	/// </summary>
	All = 0xFFFF
}


// Interface for accessing controls in things
// such as editors and inspectors.
public interface IControl
{
	string Text
	{
		get;
		set;
	}

	/// <summary>
	/// Determines whether any associated text
	/// should be taken into account when generating
	/// a collider for the control automatically.
	/// </summary>
	bool IncludeTextInAutoCollider
	{
		get;
		set;
	}

	/// <summary>
	/// Updates any auto-generated collider for the control.
	/// </summary>
	void UpdateCollider();

	/// <summary>
	/// Copies the the specified control.
	/// </summary>
	/// <param name="c">The control to be copied.</param>
	void Copy(IControl c);

	/// <summary>
	/// Copies the specified parts of the specified control.
	/// </summary>
	/// <param name="c">The control to be copied.</param>
	/// <param name="flags">Flags specifying which attributes of the control are to be copied.</param>
	void Copy(IControl c, ControlCopyFlags flags);

	int DrawPreStateSelectGUI(int selState, bool inspector);

	int DrawPostStateSelectGUI(int selState);

	void DrawPreTransitionUI(int selState, IGUIScriptSelector gui);

	string[] EnumStateElements();

	EZTransitionList GetTransitions(int index);

	EZTransitionList[] Transitions
	{
		get;
		set;
	}

	string GetStateLabel(int index);

	void SetStateLabel(int index, string label);

	ASCSEInfo GetStateElementInfo(int stateNum);


	/// <summary>
	/// Holds "boxed" data for the control.
	/// This can be used to associate any
	/// object or value with the control
	/// for later reference and use.
	/// </summary>
	object Data
	{
		get;
		set;
	}
}
