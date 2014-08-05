//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <summary>
/// Button taht opens the specified URL when clicked.
/// </summary>
[AddComponentMenu("EZ GUI/Controls/Weblink Button")]
public class UIBtnWWW : UIButton
{
	/// <summary>
	/// URL to be opened when the button is clicked.
	/// </summary>
	public string URL;

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		base.OnInput(ref ptr);

		if (!m_controlIsEnabled || IsHidden())
		{
			return;
		}

		if (ptr.evt == whenToInvoke)
		{
			Invoke("DoURL", delay);
		}
	}

	// Method that does the actual work (opens a URL)
	protected void DoURL()
	{
#if UNITY_IPHONE || UNITY_ANDROID
	#if UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
			if(Application.internetReachability != NetworkReachability.NotReachable)
	#else
			if(iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
	#endif
#endif
		Application.OpenURL(URL);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIBtnWWW))
			return;

		UIBtnWWW b = (UIBtnWWW)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			URL = b.URL;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIBtnWWW Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIBtnWWW)go.AddComponent(typeof(UIBtnWWW));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIBtnWWW Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIBtnWWW)go.AddComponent(typeof(UIBtnWWW));
	}
}
