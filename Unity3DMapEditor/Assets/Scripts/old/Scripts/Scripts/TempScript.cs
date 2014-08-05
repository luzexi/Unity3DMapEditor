using UnityEngine;
using System.Collections;

public class TempScript : MonoBehaviour {
	public Texture2D backgroundTex;
	// Use this for initialization
	public GameObject[] allGO;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnGUI()
	{
		GUI.DrawTexture( new Rect(0,0, 400,400),backgroundTex);
		GUI.Button(new Rect(0,0,100,100),"按钮");
	}
}
