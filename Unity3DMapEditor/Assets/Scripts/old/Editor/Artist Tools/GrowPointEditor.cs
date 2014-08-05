using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DBSystem;
using DBC;
[CustomEditor(typeof(GrowPointEditorScript))]
public class GrowPointEditor : Editor
{
    void OnEnable()
    {
        mGrowPointEditorScript = (GrowPointEditorScript)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("生长点");
        mGrowPointEditorScript.mCurrentIndex = EditorGUILayout.Popup(mGrowPointEditorScript.mCurrentIndex, mGrowPointEditorScript.mAllGrwoPintInfo);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("打开文件"))
        {
            openGrwopointFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("保存到文件"))
        {
            saveToFile();
        }
        GUILayout.EndHorizontal();
    }
    public void OnSceneGUI()
    {
        if (mGrowPointEditorScript.mGUIDirty)
        {
            Repaint();
            mGrowPointEditorScript.mGUIDirty = false;
            
        }
            
    }
    void saveToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save growpoint", "", "scene_growpoint", "txt");

        if (path != null && path.Length > 0)
        {
            mGrowPointEditorScript.saveToFile(path);
        }
    }
    void openGrwopointFile()
    {
        string path = EditorUtility.OpenFilePanel("Open txt", "", "txt");

        if (path == null || path.Length == 0) return;
        mGrowPointEditorScript.clearAll();
        StreamReader sr = new StreamReader(path);
        sr.ReadLine();
        sr.ReadLine();
        while(sr.EndOfStream == false)
        {
            string tempStr = sr.ReadLine();
            string[] values = tempStr.Split('\t');
            int id = Int32.Parse(values[1]);
            float x = float.Parse(values[2]);
            float z = float.Parse(values[3]);
            float y = GFX.GfxUtility.getSceneHeight(x, z);
            mGrowPointEditorScript.addGrowPoint(id, new Vector3(x,y,z));
        }
    }

    GrowPointEditorScript mGrowPointEditorScript;
}
class GrowPointTool
{
    [MenuItem("SG Tools/Grow Point Editor")]
    static void Execute()
    {
        if (!EditorApplication.isPlaying) EditorApplication.isPlaying = true;
        EventTool.AddEditorScript("GrowPointEditorScript");
    }
}