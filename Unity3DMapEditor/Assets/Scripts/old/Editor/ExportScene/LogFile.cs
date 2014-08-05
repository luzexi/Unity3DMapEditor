using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
public class fileUility
{
    [MenuItem("Tools/Log File")]
    public static void Logfile()
    {
        string dir = "Assets/Resources/Public";
        
        string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        string filstr = "";
        for(int i =0; i < files.Length; i++)
        {
            filstr += files[i] + "\n";
            
        }
        EditorHelpers.SaveStringToFile("public.log", filstr);
        dir = "Assets/Resources/Private";
        filstr = "";
        files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            filstr += files[i] + "\n";
            
        }
        EditorHelpers.SaveStringToFile("private.log", filstr);
    }
}