using UnityEngine;
using System.Collections;

public class BillboardScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
    void OnWillRenderObject()
    {
        if (Camera.mainCamera != null)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(-Camera.mainCamera.gameObject.transform.forward);//和摄像机的朝向相同
        }
    }
}
