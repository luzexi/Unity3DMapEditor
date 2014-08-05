using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class SceneJsonHandler : JsonHandler {
	
	
	public override void Write(string file){
		
	}
	
	public override void Read(string file){
		
	}
	
	/// <summary>
	/// 成员对象 
	/// </summary>
	Scene mScene;
	SceneNode mLastNode;
}

public class SceneSerializer{
	
	
	
	public bool isLoaded
	{
		get
		{
			return mLoaded;
		}
	}
	public void Load(Scene scene, string file){
		
		
		//LoadString(scene, );
		
		mLoaded = true;
	}
	
	public void Save(Scene scene, string file){
		
		string json = JsonMapper.ToJson(scene);
		SaveString(file, json);
	}
	public string SceneNodeToJson(SceneNodeE node){
		string json = JsonMapper.ToJson(node);
		return json;
	}
    public string ObjectToJson(System.Object obj, Type type)
    {
        string json = JsonMapper.ToJson(obj);
        return json;
    }
	void LoadString(Scene scene, string strSource){
		
		//is OK?
		scene = JsonMapper.ToObject<Scene>(strSource);
		if(scene == null)
			Debug.Log("Can not conversion Json string to Scene Object");
		
	}
    string ReadString(string file)
    {
        if (!File.Exists(file))
        {
            Console.WriteLine("{0} does not exist.", file);
            return "";
        }
        using (StreamReader sr = File.OpenText(file))
        {
            string input = sr.ReadToEnd();
            
            sr.Close();

            return input;
        }
        
    }
	public void SaveString(string file, string strSource){
		
		StreamWriter Writer;
		try{
			Writer = File.CreateText(file);
            
		}
		catch
		{
			Debug.LogError("Can not create file:" + file);
			return;
		}
		Writer.WriteLine(strSource);
        Writer.Close();
		
	}

    
	bool mLoaded = false;
	
}