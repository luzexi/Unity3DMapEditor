using UnityEngine;
using System.Collections;
public class FrameUpdateScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GFX.GfxSystem.Instance.Tick();
	}
	
}
