using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class SceneJsonSerializer
{
    public static Scene JsonToObject(string strJson)
    {
        //todo 异常处理
        Scene scene = JsonMapper.ToObject<Scene>(strJson);

        return scene;
    }

    public static string ObjectToJson(Scene scene)
    {
        string json = JsonMapper.ToJson(scene);

        return json;
    }


}