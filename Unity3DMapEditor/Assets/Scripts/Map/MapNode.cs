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
}

