using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class SkillTools
{
    static BuildAssetBundleOptions options = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;

    [MenuItem("SG Tools/ExportSkillFiles")]
    static void Execute()
    {
        string skillResPublicPath = "Assets/Resources/Skill";
        string packName = "Build/Skill/skilltemp.config";
        string filePath = "Build/Skill";
        if (Directory.Exists(filePath))
        {
            Directory.Delete(filePath, true);
        }

        Directory.CreateDirectory(filePath);

        // 打包所有的配置文件 [3/16/2012 Ivan]
        BuildPipeline.PushAssetDependencies();
        List<UnityEngine.Object> skills = EditorHelpers.CollectAll<UnityEngine.Object>(skillResPublicPath);
        LogManager.Log("curSkill "+ skills.Count);
        BuildPipeline.BuildAssetBundle(
                null, skills.ToArray(), packName, options);
        BuildPipeline.PopAssetDependencies();

        // 加密后并重新打包 [3/27/2012 Ivan]
        EncryptFile(packName);
    }

    static void EncryptFile(string orgPath)
    {
        string tempFileName = "Assets/Temps/skill.bytes";
        string resultFileName = "Build/Skill/skill.config";
        byte[] content = GetFile(orgPath);
        if (content != null)
        {
            // 加密二进制文件 [3/27/2012 Ivan]
            EncryptBinary(ref content);
            // 写入文件 [3/27/2012 Ivan]
            Write2File(tempFileName, content);
            // 重新打包 [3/27/2012 Ivan]
            ExportEncryptFile(resultFileName, tempFileName);
            // 清理中间文件 [3/27/2012 Ivan]
            if (File.Exists(orgPath))
                File.Delete(orgPath);
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);

        }
    }

    private static void ExportEncryptFile(string resultFileName, string tempFileName)
    {
        UnityEngine.Object[] objs = new UnityEngine.Object[1];
        AssetDatabase.Refresh();
        objs[0] = AssetDatabase.LoadMainAssetAtPath(tempFileName);
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundle(
                null, objs, resultFileName, options);
        BuildPipeline.PopAssetDependencies();
    }

    private static void Write2File(string path, byte[] content)
    {
        BinaryWriter binWriter = null;
        FileStream fs = null;
        try
        {
            if (File.Exists(path))
                File.Delete(path);
            fs = File.Open(path, FileMode.Create);
            binWriter = new BinaryWriter(fs);
            binWriter.Write(content);
        }
        catch (Exception e)
        {
            LogManager.LogError(e.Message);
        }
        finally
        {
            binWriter.Close();
            fs.Close();
        }
    }

    private static void EncryptBinary(ref byte[] content)
    {
        content = Encrypt.SGEncrypt.EncryptByte(content);
    }

    static byte[] GetFile(string path)
    {
        FileStream fs = File.Open(path, FileMode.Open);
        BinaryReader binReader = new BinaryReader(fs);
        try
        {
            byte[] content = new byte[fs.Length];
            int count = binReader.Read(content, 0, (int)fs.Length);

            if (count != 0)
            {
                return content;
            }
        }
        catch (EndOfStreamException e)
        {
            LogManager.LogError(e.Message);
        }
        finally
        {
            binReader.Close();
            fs.Close();
        }
        return null;
    }
}