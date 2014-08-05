//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;

// Only compile if not using Unity iPhone
#if !UNITY_IPHONE || (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9)
[CustomEditor(typeof(UIPanel))]
#endif
public class UIPanelInspector : UIPanelBaseInspector
{
}