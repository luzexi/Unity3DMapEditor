using UnityEngine;
using System.Collections;

public class SpriteManagerCamera : MonoBehaviour {
	
	void Awake()
	{
		PackedSprite packedSprite = gameObject.GetComponent<PackedSprite>();
		if(packedSprite != null)
			packedSprite.RenderCamera = UISystem.Instance.UiCamrea;
	}
}
