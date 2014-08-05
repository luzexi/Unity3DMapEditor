//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Provides an interface for containing dynamically-created
/// IUIObjects so that they can be properly added to the
/// parent UIView's list for processing.
/// UIContainers are passive in that, unlike UIObjects, they 
/// accept no input directly.
/// </remarks>
public interface IUIContainer : IUIObject
{
	/// <summary>
	/// Reference to the parent UIView
	/// </summary>
	//protected UIView parentView;

/*
	public void SetParentView(UIView v)
	{
		parentView = v;
	}
*/

	/// <summary>
	/// Adds a child to the container.
	/// </summary>
	/// <param name="go">GameObject to be added as a child of the container.</param>
	void AddChild(GameObject go);

	/// <summary>
	/// Removes an object as a child of the container.
	/// </summary>
	/// <param name="go">Object to be removed.</param>
	void RemoveChild(GameObject go);

	/// <summary>
	/// Adds an object as a subject of the container's transitions.
	/// </summary>
	/// <param name="go">GameObject that is subject to the container's transitions.</param>
	void AddSubject(GameObject go);

	/// <summary>
	/// Removes an object from being a subject of the container's transitions.
	/// </summary>
	/// <param name="go">GameObject that should no longer be subject to the container's transitions.</param>
	void RemoveSubject(GameObject go);
}
