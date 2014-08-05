using UnityEngine;
using UnityEditor;


public class VersionInfoWizard: ScriptableWizard
{
    public int nVersionDBC   = 0;
    public int nVersionUI    = 0;
    public int nVersionScene = 0;

    [MenuItem("SG Tools/Create Version info")]
    static void CreateVersionInfo()
    {
        ScriptableWizard.DisplayWizard("Version Info", typeof(VersionInfoWizard), "Create");
    }

    void OnWizardCreate()
    {
        ResourceManager.VersionInfo version = new ResourceManager.VersionInfo();
        version.nVersionDBC = nVersionDBC;
        version.nVersionUI = nVersionUI;
        version.nVersionScene = nVersionScene;

        EditorSerializer.SerializeObjectAndSave(version, "version.txt");
    }
}