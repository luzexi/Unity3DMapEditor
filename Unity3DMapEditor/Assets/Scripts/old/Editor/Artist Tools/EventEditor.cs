using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DBSystem;
using DBC;
[CustomEditor(typeof(EventEditorScript))]
public class EventEditor : Editor
{
    void OnEnable()
    {
		mEventEditorScript = (EventEditorScript)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("打开文件"))
        {
            openINIFile();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("区域ID：");
        mEventEditorScript.mCurrentEventInfo.mGuid = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mGuid);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("脚本ID：");
        mEventEditorScript.mCurrentEventInfo.mScript_id = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mScript_id);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("传送目的地ID：");
        mEventEditorScript.mCurrentEventInfo.mTransSceneId = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mTransSceneId);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("目的地X坐标：");
        mEventEditorScript.mCurrentEventInfo.mTransX = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mTransX);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("目的地Z坐标：");
        mEventEditorScript.mCurrentEventInfo.mTransY = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mTransY);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("最低等级：");
        mEventEditorScript.mCurrentEventInfo.mTransMinLevel = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mTransMinLevel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("最高等级：");
        mEventEditorScript.mCurrentEventInfo.mTransMaxLevel = EditorGUILayout.IntField(mEventEditorScript.mCurrentEventInfo.mTransMaxLevel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("修改"))
        {
            mEventEditorScript.modifyCurrent();
        }
        if (GUILayout.Button("保存到文件"))
        {
            saveToFile();
        }
        GUILayout.EndHorizontal();
    }

    public void OnSceneGUI()
    {
        if (mEventEditorScript.mUIDirty)
        {
            Repaint();
            mEventEditorScript.mUIDirty = false;
        }
       
    }

    void saveToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save INI", "", "scene_area", "ini");

        if (path != null && path.Length > 0)
        {
            mEventEditorScript.saveToFile(path);
        }
    }
    void openINIFile()
    {
        string path = EditorUtility.OpenFilePanel("Open INI", "", "ini");

        if (path == null || path.Length == 0) return;

        mEventEditorScript.clearAll();
        IniFile ini = new IniFile(path, "area_info");
        int areaCount = Int32.Parse(ini["area_count"]);
        for (int i = 0; i < areaCount; ++i )
        {
            ini = new IniFile(path, "area"+i);
            mEventEditorScript.mCurrentEventInfo.mGuid = Int32.Parse(ini["guid"]);
            mEventEditorScript.mCurrentEventInfo.mScript_id = Int32.Parse(ini["script_id"]);

            mEventEditorScript.mCurrentEventInfo.mTransSceneId = Int32.Parse(ini["transSceneId"]);
            mEventEditorScript.mCurrentEventInfo.mTransX = Int32.Parse(ini["transX"]);
            mEventEditorScript.mCurrentEventInfo.mTransY = Int32.Parse(ini["transY"]);
            mEventEditorScript.mCurrentEventInfo.mTransMinLevel = Int32.Parse(ini["transMinLevel"]);
            mEventEditorScript.mCurrentEventInfo.mTransMaxLevel = Int32.Parse(ini["transMaxLevel"]);

            float left = (float)float.Parse(ini["left"]);
            float top = (float)float.Parse(ini["top"]);
            float y1 = GFX.GfxUtility.getSceneHeight(left, top);
            float right = (float)float.Parse(ini["right"]);
            float bottom = (float)float.Parse(ini["bottom"]);
            float y2 = GFX.GfxUtility.getSceneHeight(right, bottom);

            mEventEditorScript.createArea(new Vector3(left, y1, top), new Vector3(right, y2, bottom));
        }
    }
	EventEditorScript mEventEditorScript;
}

class EventTool
{
    [MenuItem("SG Tools/Event Editor")]
    static void Execute()
    {
        if (!EditorApplication.isPlaying) EditorApplication.isPlaying = true;

        AddEditorScript("EventEditorScript");
        
    }
    public static void AddEditorScript(string scriptName)
    {
        GameObject go = GameObject.Find("SGEditor");
        if (go != null)
        {
            if (go.GetComponent(scriptName) != null)
            {
                Selection.activeGameObject = go;
                return;
            }
            UnityEngine.Object.DestroyImmediate(go);
        }

        //go = new GameObject("SGEditor");
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "SGEditor";
        go.AddComponent("GameCameraScript");
		go.AddComponent("ShowHitPositionScript");
        go.AddComponent(scriptName);
        Selection.activeGameObject = go;
    }
}