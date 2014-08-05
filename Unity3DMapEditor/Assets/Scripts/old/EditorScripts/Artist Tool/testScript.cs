using UnityEngine;
using System.Collections;

public class testScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("Cube/Sphere");
		if(go)
		{
			Debug.Log("find");
		}
		else 
			Debug.Log("not find");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
