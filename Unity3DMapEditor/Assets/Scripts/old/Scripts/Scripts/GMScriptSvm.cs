using System.Collections.Generic;
using System;
using UnityEngine;

public class CScriptSvm
{
    static readonly CScriptSvm instance = new CScriptSvm();
    public static CScriptSvm Instance
    {
        get
        {
            return instance;
        }
    }

    protected Dictionary<int, string> m_mapQuestFile = new Dictionary<int, string>();

    private int IndexForLog = 0;

    public void Initial()
    {
        LoadMissionScriptFile();
    }

    public string GetMissionScriptFile(int nID)
    {
        if (m_mapQuestFile.ContainsKey(nID))
        {
            return m_mapQuestFile[nID];
        }
        return string.Empty;
    }

    private bool LoadMissionScriptFile()
    {
        try
        {
            //UnityEngine.Object MissionFileObj = Resources.Load("Private/Script");

            //if (MissionFileObj == null)
            //{
            //    LogManager.LogError("\r\n----------------------------------------------------\r\n");
            //    LogManager.LogError("Load Resources Script.txt 出错。");
            //    LogManager.LogError("\r\n----------------------------------------------------\r\n");
            //    return false;
            //}

            //TextAsset MissionFileAsset = (TextAsset)MissionFileObj;

            string scriptData = DBStruct.GetResources("Private/Script");
            string[] MissionFiles = scriptData.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < MissionFiles.Length; i++)
            {
                IndexForLog++;
                MissionFiles[i].Trim();
                if (MissionFiles[i].Equals("") || MissionFiles[i].StartsWith(";"))
                {
                    continue;
                }
                else
                {
                    int npos = MissionFiles[i].IndexOf("=");
                    if (npos == -1)
                    {
                        continue;
                    }
                    int ID = Convert.ToInt32(MissionFiles[i].Substring(0, npos).Trim());
                    string MissionFilePath = MissionFiles[i].Substring(npos + 1).Trim();
                    MissionFilePath = "Private" + MissionFilePath.Replace(".lua", "");
                    MissionFilePath = MissionFilePath.Replace("\\", "/");
                    m_mapQuestFile.Add(ID, MissionFilePath);
                }
            }

            return true;
        }
        catch(Exception ex)
        {
            LogManager.LogError("--------------------出错行ID为:" + IndexForLog + " ,第二次出现该ID。--------------------");
            LogManager.LogError(ex.ToString());
            return false;
        }
    }
}