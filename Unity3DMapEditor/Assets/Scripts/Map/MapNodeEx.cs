using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//	MapNode.cs
//	Author: Lu Zexi
//	2017-08-05



/// <summary>
/// Map node.
/// </summary>
public class MapNodeEx
{
	public string name;
	public string assetPath;
	public string assetBundle;
	public string textureBundle;
	public string textures;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	public bool hasTexture = false;

	public MapNode ToMapNode()
	{
		MapNode node = new MapNode();
		node.name = this.name;
		node.assetPath = this.assetBundle;
		node.texture = this.textureBundle;
		if (this.textures != null)
			node.texture += "@" + this.textures;
		
//		string Value = this.position.ToString();
//		string pos = Value.Substring(1, Value.Length - 2);
//		string[] axis = pos.Split(',');
//		node.position[0] = System.Double.Parse(axis[0]);
//		node.position[1] = System.Double.Parse(axis[1]);
//		node.position[2] = System.Double.Parse(axis[2]);
		node.position[0] = this.position.x;
		node.position[1] = this.position.y;
		node.position[2] = this.position.z;
		
//		Value = this.rotation.ToString();
//		pos = Value.Substring(1, Value.Length - 2);
//		axis = pos.Split(',');
//		node.rotation[0] = System.Double.Parse(axis[0]);
//		node.rotation[1] = System.Double.Parse(axis[1]);
//		node.rotation[2] = System.Double.Parse(axis[2]);
//		node.rotation[3] = System.Double.Parse(axis[3]);
		node.rotation[0] = this.rotation.x;
		node.rotation[1] = this.rotation.y;
		node.rotation[2] = this.rotation.z;
		node.rotation[3] = this.rotation.w;
		
//		Value = this.scale.ToString();
//		pos = Value.Substring(1, Value.Length - 2);
//		axis = pos.Split(',');
//		node.scale[0] = System.Double.Parse(axis[0]);
//		node.scale[1] = System.Double.Parse(axis[1]);
//		node.scale[2] = System.Double.Parse(axis[2]);
		node.scale[0] = this.scale.x;
		node.scale[1] = this.scale.y;
		node.scale[2] = this.scale.z;

		return node;
	}
}
