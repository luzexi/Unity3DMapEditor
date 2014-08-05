using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DBSystem;
using DBC;
[CustomEditor(typeof(NPCEditorScript))]
public class NPCEditor:Editor
{
    void OnEnable () 
    {
		
        mNPCScript = (NPCEditorScript)(target);
    }

	void OnGUI()
	{
        if (NPCEditorScript.NPCEditorScriptEnable== false) return;
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("怪物NPC");
        int oldIndex = mNPCScript.mCurrentSelectMonsterIndex;
        mNPCScript.mCurrentSelectMonsterIndex = EditorGUILayout.Popup(mNPCScript.mCurrentSelectMonsterIndex, mNPCScript.AllMonsterNpcNames);
        if (oldIndex != mNPCScript.mCurrentSelectMonsterIndex || mNPCScript.mCurrentMonsterInfo == null)
        {
            mNPCScript.resetCurrentMonsterInfo();
        }
        GUILayout.EndHorizontal();

        _DBC_CREATURE_ATT monsterAttr = mNPCScript.mMonsterTable.Search_Index_EQU(mNPCScript.AllMonsterNpcID[mNPCScript.mCurrentSelectMonsterIndex]);
        if (monsterAttr == null) return;
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("打开文件"))
        {
            openINIFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("名字(不能修改)");
        EditorGUILayout.TextField(monsterAttr.pName);

        GUILayout.Label("等级");
        mNPCScript.mCurrentMonsterInfo.m_iLevel = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iLevel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("复活时间");
        mNPCScript.mCurrentMonsterInfo.m_iRefreshTime = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iRefreshTime);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("阵营ID");
        mNPCScript.mCurrentMonsterInfo.m_iCampId = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iCampId);
        GUILayout.Label("名誉ID");
        mNPCScript.mCurrentMonsterInfo.m_iReputationID = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iReputationID);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("基础AI类型");
        mNPCScript.mCurrentMonsterInfo.m_iBaseAIId = EditorGUILayout.Popup(mNPCScript.mCurrentMonsterInfo.m_iBaseAIId,mNPCScript.mBaseAIString);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("AI文件");
        mNPCScript.mCurrentMonsterInfo.m_iAdvanceAIId = EditorGUILayout.Popup(mNPCScript.mCurrentMonsterInfo.m_iAdvanceAIId, mNPCScript.mAdvanceAIString);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("脚本文件");
        mNPCScript.mCurrentMonsterInfo.m_EvenId = EditorGUILayout.Popup(mNPCScript.mCurrentMonsterInfo.m_EvenId+1, mNPCScript.mScriptFileString)-1;
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal("box");
        GUILayout.Label("群ID");
        mNPCScript.mCurrentMonsterInfo.m_iGroupId = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iGroupId);
        GUILayout.Label("队伍ID");
        mNPCScript.mCurrentMonsterInfo.m_iTeamId = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iTeamId);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("实例名");
        mNPCScript.mCurrentMonsterInfo.m_strInstanceName = EditorGUILayout.TextField(mNPCScript.mCurrentMonsterInfo.m_strInstanceName);
//         GUILayout.Label("GUID");
//         mNPCScript.mCurrentMonsterInfo.m_dwMonsterGUID = (uint)EditorGUILayout.IntField((int)mNPCScript.mCurrentMonsterInfo.m_dwMonsterGUID);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("巡逻路线ID");
        mNPCScript.mCurrentMonsterInfo.m_iLineid = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iLineid);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("头衔");
        mNPCScript.mCurrentMonsterInfo.m_strTitleName = EditorGUILayout.TextField(mNPCScript.mCurrentMonsterInfo.m_strTitleName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("NPC类型");
        mNPCScript.mCurrentMonsterInfo.m_iType = EditorGUILayout.Popup(mNPCScript.mCurrentMonsterInfo.m_iType, mNPCScript.mNPCType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("店铺1");
        mNPCScript.mCurrentMonsterInfo.m_iShopArray[0] = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iShopArray[0]);
        GUILayout.Label("店铺2");
        mNPCScript.mCurrentMonsterInfo.m_iShopArray[1] = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iShopArray[1]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("店铺3");
        mNPCScript.mCurrentMonsterInfo.m_iShopArray[2] = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iShopArray[2]);
        GUILayout.Label("店铺4");
        mNPCScript.mCurrentMonsterInfo.m_iShopArray[3] = EditorGUILayout.IntField(mNPCScript.mCurrentMonsterInfo.m_iShopArray[3]);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");

        if(GUILayout.Button("修改"))
        {
            save();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("保存到文件"))
        {
            saveToFile();
        }

        GUILayout.EndHorizontal();
	}

    public override void OnInspectorGUI()
    {
        OnGUI();
    }

    public void OnSceneGUI()
    {
 		if(mNPCScript.mInspectorDirty)
        {
            Repaint();
            mNPCScript.mInspectorDirty = false;
        }
    }


    void save()
    {
        mNPCScript.save();
    }
    void saveToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save INI", "", "scene_monster", "ini");

        if (path != null && path.Length >0)
        {
            mNPCScript.saveToFile(path);
        }
       
    }
    void openINIFile()
    {
        string path = EditorUtility.OpenFilePanel("Open INI", "", "ini");

        if (path == null || path.Length == 0) return ;
        mNPCScript.clear();
        /* mNPCScript.readFromIni(path);*/
        IniFile ini = new IniFile(path, "info");
        int monsterCount = System.Int32.Parse(ini["monstercount"]);
        for (int i = 0; i < monsterCount; ++i )
        {
            CMonsterInstanceInfo monsterInfo = new CMonsterInstanceInfo();
            ini = new IniFile(path, "monster"+i.ToString());
            monsterInfo.m_dwMonsterGUID = UInt32.Parse(ini["guid"]);
            monsterInfo.m_dwObjectId = UInt64.Parse(ini["type"]);
            monsterInfo.m_strInstanceName = ini["name"];
            monsterInfo.m_strTitleName = ini["title"];
            float posX = Single.Parse(ini["pos_x"]);
            float posZ = Single.Parse(ini["pos_z"]);
            float posY = GFX.GfxUtility.getSceneHeight(posX, posZ);
            int dir = Int32.Parse(ini["dir"]);
            dir += 9;
            dir = dir % 36;
            float fDir = (dir * 1.0f / 36) * (6.28f);
            monsterInfo.m_EvenId =Int32.Parse( ini["script_id"]);
            monsterInfo.m_iRefreshTime = Int32.Parse(ini["respawn_time"]);
            monsterInfo.m_iGroupId = Int32.Parse(ini["group_id"]);
            monsterInfo.m_iTeamId = Int32.Parse(ini["team_id"]);
            monsterInfo.m_iBaseAIId = Int32.Parse(ini["base_ai"]);
            monsterInfo.m_iAdvanceAIId = Int32.Parse(ini["ai_file"]);
            monsterInfo.m_iLineid = Int32.Parse(ini["patrol_id"]);
            monsterInfo.m_iShopArray[0] = Int32.Parse(ini["shop0"]);
            monsterInfo.m_iShopArray[1] = Int32.Parse(ini["shop1"]);
            monsterInfo.m_iShopArray[2] = Int32.Parse(ini["shop2"]);
            monsterInfo.m_iShopArray[3] = Int32.Parse(ini["shop3"]);
            monsterInfo.m_iReputationID = Int32.Parse(ini["ReputationID"]);
            monsterInfo.m_iLevel = Int32.Parse(ini["level"]);
            monsterInfo.m_iType = Int32.Parse(ini["npc"]);
            monsterInfo.m_iCampId = Int32.Parse(ini["camp"]);

            monsterInfo.m_popupIndex = mNPCScript.IDToPopupIndex[(int)monsterInfo.m_dwObjectId];
            mNPCScript.AddActor((int)monsterInfo.m_dwObjectId,
                                monsterInfo,
                                new Vector3(posX, posY, posZ),
                                Quaternion.AngleAxis(Mathf.Rad2Deg * fDir, Vector3.up));
        }
    }
    public NPCEditorScript mNPCScript;
 }

//创建玩家装备资源包 [2011/12/14 ZZY]
class NPCTool
{
    [MenuItem("SG Tools/Monster Editor")]
    static void Execute()
    {
        if (!EditorApplication.isPlaying) EditorApplication.isPlaying = true;
        EventTool.AddEditorScript("NPCEditorScript");
    }
}