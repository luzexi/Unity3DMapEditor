using UnityEngine;
using System.Collections;

public class textureAdressMode : MonoBehaviour 
{
	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
	void Start () 
	{
		renderer.material.mainTexture.wrapMode = wrapMode;
	}
	
	// Update is called once per frame
	
}
