using System;
using System.Collections.Generic;

using LitJson;


public class EditorSerializer
{

    public static void SerializeObjectAndSave(System.Object obj, string file)
    {
        string json = JsonMapper.ToJson(obj);
        EditorHelpers.SaveStringToFile(file, json);
    }
    public static System.Object JsonToObject(string file)
    {
        string json = EditorHelpers.ReadString(file);
        System.Object obj = JsonMapper.ToObject(json);

        return obj;
    }
    public static List<Assets> JsonToAssetList(string file)
    {
        string json = EditorHelpers.ReadString(file);
        List<Assets> assets = JsonMapper.ToObject<List<Assets>>(json);

        return assets;
        
    }
    public static Scene JsonToScene(string file)
    {
        string json = EditorHelpers.ReadString(file);
        Scene scene = JsonMapper.ToObject<Scene>(json);

        return scene;
    }
    public static string ObjectToJson(System.Object obj)
    {
        return JsonMapper.ToJson(obj);
    }
}