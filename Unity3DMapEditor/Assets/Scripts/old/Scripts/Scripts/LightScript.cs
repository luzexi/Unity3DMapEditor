using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {

	// Use this for initialization
	public float MinIntensity = 1.0f;
	public float MaxIntensity = 2.0f;
	public float CycleTime = 0.3f;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(light == null) return;
		light.intensity = (Mathf.Cos(Time.time*Mathf.PI*2/CycleTime)+1)*0.5f *(MaxIntensity-MinIntensity) + MinIntensity;
	}
}
