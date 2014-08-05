using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DBSystem;
using DBC;
[CustomEditor(typeof(PatrolEditorScript))]
public class PatrolEditor : Editor
{
    void OnEnable()
    {
        mPatrolEditorScript = (PatrolEditorScript)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("停留时间");
        mPatrolEditorScript.mCurrentSettleTime = EditorGUILayout.IntField(mPatrolEditorScript.mCurrentSettleTime);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("打开文件"))
        {
            openFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("完成路线"))
        {
            mPatrolEditorScript.finishCurrentLine();
        }
        if (GUILayout.Button("修改路线"))
        {
            mPatrolEditorScript.modifyCurrent();
        }
        if (GUILayout.Button("刷新"))
        {
            mPatrolEditorScript.refresh();
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
        if (mPatrolEditorScript.mGUIDirty)
        {
            Repaint();
            mPatrolEditorScript.mGUIDirty = false;
           
        }

    }
    void saveToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save Patrol", "", "scene_patrolpoint", "ini");

        if (path != null && path.Length > 0)
        {
            mPatrolEditorScript.saveToFile(path);
        }
    }
    void openFile()
    {
        string path = EditorUtility.OpenFilePanel("Open INI", "", "ini");

        if (path == null || path.Length == 0) return;
        mPatrolEditorScript.clearAll();
        IniFile ini = new IniFile(path, "INFO");
        int numPatrol = int.Parse(ini["PATROLNUMBER"]);
        for (int i = 0; i < numPatrol; ++i)
        {
            mPatrolEditorScript.finishCurrentLine();
            ini = new IniFile(path, "PATROL"+i);
            int patrolPointNum = int.Parse(ini["PATROLPOINTNUM"]);
            for (int j = 0; j < patrolPointNum; ++j)
            {
                float posx = float.Parse(ini["POSX" + j]);
                float posy = float.Parse(ini["POSY" + j]);
                float posz = float.Parse(ini["POSZ" + j]);
                int settletime = int.Parse(ini["settletime" + j]);
                mPatrolEditorScript.add(new Vector3(posx, posy, posz), settletime);
            }
        }
    }

    PatrolEditorScript mPatrolEditorScript;
}

class PatrolTool
{
    [MenuItem("SG Tools/Patrol Editor")]
    static void Execute()
    {
        if (!EditorApplication.isPlaying) EditorApplication.isPlaying = true;
        EventTool.AddEditorScript("PatrolEditorScript");
    }
}