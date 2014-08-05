//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// UIPanel provides functionality as a container for controls.
/// Controls can be organized into panels and manipulated as a
/// group.
/// 
/// To organize controls into a panel, simply make them children
/// of a GameObject containing a UIPanel component.
/// </remarks>
[System.Serializable]
[AddComponentMenu("EZ GUI/Panels/Panel")]
public class UIPanel : UIPanelBase
{
	//------------------------------------------
	// Transition stuff:
	//------------------------------------------
	[HideInInspector]
	public EZTransitionList transitions = new EZTransitionList( new EZTransition[]
		{
			new EZTransition("Bring In Forward"),
			new EZTransition("Bring In Back"),
			new EZTransition("Dismiss Forward"),
			new EZTransition("Dismiss Back")
		});

	public override EZTransitionList Transitions
	{
		get { return transitions; }
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIPanel Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIPanel)go.AddComponent(typeof(UIPanel));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIPanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIPanel)go.AddComponent(typeof(UIPanel));
	}
}