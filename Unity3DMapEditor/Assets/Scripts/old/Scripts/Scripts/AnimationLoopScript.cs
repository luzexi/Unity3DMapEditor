using UnityEngine;
using System.Collections;

public class AnimationLoopScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	if(gameObject.animation != null)
	{
		gameObject.animation.wrapMode = WrapMode.Loop;		
	}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
