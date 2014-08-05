using UnityEngine;
using System.Collections;

public class ShowHitPositionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        	RaycastHit hitInfo;
        	bool hit = Physics.Raycast(ray, out hitInfo);
        	if (!hit) return;
			Debug.Log("Position:"+ "x="+hitInfo.point.x + " z="+hitInfo.point.z);
		}

	}
}
