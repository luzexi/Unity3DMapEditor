using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {
	
	//event
	public delegate void EventHandler(GameObject go);
	
	public event EventHandler UpdateObjects;
	public event EventHandler UpdateTextures;
	
	public string NextSceneName;
	

	void Awake(){
	
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(UpdateObjects != null)
		{
			UpdateObjects(this.gameObject);
		}
		if(UpdateTextures != null)
		{
			UpdateTextures(this.gameObject);
		}
	}
	
	//GUI
    //void OnGUI(){

    //    if (GUI.Button(new Rect(20, 100, 120, 25), "Load Scene"))
    //    {
    //        if (Application.loadedLevelName != NextSceneName)
    //            EnterScene(NextSceneName);
    //    }

    //}
	public  void EnterScene(string Name){
		
		//todo: 根据场景信息文件，查找这个场景对应的资源文件
		//启动资源下载
		//StartCoroutine(SceneInfo.Instance().LoadScene(Name));
       //StartCoroutine(SceneInfo.Instance().DownLoadNew(Name));
         SceneInfo.Instance().EnterScene(Name);
	}

    public void EnterSceneWithoutNav(string loginSceneName)
    {
        SceneInfo.Instance().EnterSceneWithoutNav(loginSceneName);
    }
}
