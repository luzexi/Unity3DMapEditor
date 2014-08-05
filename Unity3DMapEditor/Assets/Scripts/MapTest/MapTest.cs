using UnityEngine;
using System.Collections;
using Game.Resource;
using LitJson;


/// <summary>
/// Map test.
/// </summary>
public class MapTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool start = false;
	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,200,200) , "go"))
		{
			string path = "file://" + Application.dataPath + "/" +"Build/Maps/MapTest/map.p";
			ResourceMgr.RequestAssetBundle(path , loadBack ,null);
		}
	}

	private void loadBack( string name , object obj , object[] arg )
	{
		TextAsset ta = (obj as AssetBundle).mainAsset as TextAsset;
		Map map = JsonMapper.ToObject<Map>(ta.text);
		foreach( MapNode item in map.objects )
		{
			string dir = "file://"+Application.dataPath + "/" + "Build/Maps/"+map.name + "/";
			//Debug.Log("dir " + dir);
			ResourceMgr.RequestAssetBundle(dir+item.assetPath , loadNodeBack ,null , item.ToMapNodeEx() );
		}
	}

	private void loadNodeBack( string name , object obj , object[] arg )
	{
		MapNodeEx node = arg[0] as MapNodeEx;
		AssetBundle ab = obj as AssetBundle;
		GameObject go = GameObject.Instantiate( ab.Load(node.name)) as GameObject;
		go.transform.position = node.position;
		go.transform.rotation = node.rotation;
		go.transform.localScale = node.scale;
	}
}
