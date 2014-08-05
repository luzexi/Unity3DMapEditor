using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class BuildBatch  {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [MenuItem("SG Tools/Build WebPlayer")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "WebPlayer");

        string[] levels = {"Assets/Res/Map/Scene/load/load.unity"}; 
        BuildPipeline.BuildPlayer(levels, path + "/WebPlayer", BuildTarget.WebPlayerStreamed, BuildOptions.WebPlayerOfflineDeployment);
        if (!Directory.Exists(path + "/WebPlayer/Build"))
            Directory.CreateDirectory(path + "/WebPlayer/Build");
        string[] dirs = Directory.GetDirectories("Build");
        for (int i = 0; i < dirs.Length; i++ )
        {
            string dir = dirs[i].Replace("\\", "/");
            FileUtil.CopyFileOrDirectory(dir, path + "/WebPlayer/" + dir);
        }
    }
}
