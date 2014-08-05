using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

class EditorHelpers
{
    // This method loads all files at a certain path and
    // returns a list of specific assets.
    public static List<T> CollectAll<T>(string path) where T : Object
    {
		try{
			
        List<T> l = new List<T>();
        string[] files = Directory.GetFiles(path,"*.*", SearchOption.AllDirectories);
		foreach (string file in files)
        {
            if (file.Contains(".meta") || file.Contains(".db")) continue;
            T asset = (T) AssetDatabase.LoadAssetAtPath(file, typeof(T));
            if (asset == null)
            {
                Debug.LogError("Asset is not " + typeof(T) + ": " + file);
                continue;
            }
            l.Add(asset);
        }
        return l;
		}
		catch (Exception ex)
		{
    		Debug.LogError("exception caught here:" +  ex.GetType().ToString());
    		Debug.LogError(ex.Message);
		}

		return null;
       
    }

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
        //TextAsset text = AssetDatabase.LoadAssetAtPath(file, typeof(Object)) as TextAsset;
        //if (!text)
        //{
        //    Debug.Log(file + " does not exist.");
        //    return "";
        //}
        //return text.text;

    }
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
}