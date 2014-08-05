using UnityEngine;
using System.Collections;

public class RotationScript : MonoBehaviour {
    public float RotateSpeed = 60.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float rotAngle = Time.deltaTime*RotateSpeed;
        gameObject.transform.Rotate(Vector3.up, rotAngle, Space.World);
	}
    void OnWillRenderObject()
    {
        
    }
}
