using UnityEngine;
using System.Collections;


//	MapNode.cs
//	Author: Lu Zexi
//	2014-08-05



/// <summary>
/// Map node.
/// </summary>
public class MapNode
{
	public string name;
	public string assetPath;
	public string texture;
	public double[] position;
	public double[] rotation;
	public double[] scale;
	
	public MapNode()
	{
		position = new double[3];
		rotation = new double[4];
		scale = new double[3];
	}

	public MapNodeEx ToMapNodeEx()
	{
		MapNodeEx node = new MapNodeEx();
		node.name = this.name;
		node.assetBundle = this.assetPath;
		node.textureBundle = this.texture;
		node.position = new Vector3((float)this.position[0],(float)this.position[1],(float)this.position[2]);
		node.rotation = new Quaternion((float)this.rotation[0],(float)this.rotation[1],(float)this.rotation[2],(float)this.rotation[3]);
		node.scale = new Vector3((float)this.scale[0],(float)this.scale[1],(float)this.scale[2]);
		return node;
	}
}

