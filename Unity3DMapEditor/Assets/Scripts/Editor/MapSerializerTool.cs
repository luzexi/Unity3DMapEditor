using UnityEngine;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;




/// <summary>
/// Map serializer tool.
/// </summary>
public class MapSerializerTool
{
	public static void SerializeObjectAndSave(System.Object obj, string file)
	{
		string json = JsonMapper.ToJson(obj);
		SaveStringToFile(file, json);
	}
	public static System.Object JsonToObject(string file)
	{
		string json = EditorHelpers.ReadString(file);
		System.Object obj = JsonMapper.ToObject(json);
		
		return obj;
	}

	public static List<MapAssets> JsonToAssetList(string file)
	{
		string json = ReadString(file);
		List<MapAssets> assets = JsonMapper.ToObject<List<MapAssets>>(json);
		if(assets == null )
			assets = new List<MapAssets>();
		return assets;
	}

	public static Map JsonToScene(string file)
	{
		string json = ReadString(file);
		Map scene = JsonMapper.ToObject<Map>(json);
		
		return scene;
	}

	public static string ObjectToJson(System.Object obj)
	{
		return JsonMapper.ToJson(obj);
	}

	/// <summary>
	/// Saves the string to file.
	/// </summary>
	/// <param name="path">Path.</param>
	/// <param name="strSource">String source.</param>
	public static void SaveStringToFile(string path, string strSource)
	{
		StreamWriter Writer;
		try
		{
			Writer = File.CreateText(path);
		}
		catch
		{
			//Debug.LogError("Can not create file:" + file);
			return;
		}
		Writer.WriteLine(strSource);
		Writer.Close();
	}

	/// <summary>
	/// Reads the string.
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="file">File.</param>
	public static string ReadString(string file)
	{
		if (!File.Exists(file))
		{
			Debug.Log(file + " does not exist.");
			return "";
		}
		using (StreamReader sr = File.OpenText(file))
		{
			string input = sr.ReadToEnd();
			
			sr.Close();
			
			return input;
		}
	}
}

