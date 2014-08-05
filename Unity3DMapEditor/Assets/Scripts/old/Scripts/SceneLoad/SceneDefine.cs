using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneNode
{

    public string name;
    public string assetPath;
    public string texture;
    public double[] position;
    public double[] rotation;
    public double[] scale;

    public SceneNode()
    {
        position = new double[3];
        rotation = new double[4];
        scale = new double[3];
    }
}

public class Scene {
	public string name;
	public string terrainFile;
	public List<SceneNode> objects;
}