using System.Collections;
using System.IO;
public class IniFile
{
    private Hashtable iniFile = new Hashtable();
    public int Count { get { return iniFile.Count; } }
    public string this[string IndexKey] { get { return iniFile[IndexKey].ToString(); } }

    public IniFile(string file, string section)
    {
       string Section = "[" + section + "]";
       LoadIniFile(file, Section);
       if (iniFile.Count>0 && iniFile.Contains("SpecialIniFilePath"))
       {
             string path = this["SpecialIniFilePath"].Trim();
             if (path != "") LoadIniFile(path, Section);
       }
    }
    private void LoadIniFile(string filePath, string Section)
    {
        try  
        {
           
            StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default);
            string readLine = null;
            bool readEnd = false;
            string[] keyWord;
            while ((readLine = sr.ReadLine()) != null)
            {
                if (readLine == Section)   //是指定的节，则开始读取配置信息                 
                {
                    while ((readLine = sr.ReadLine()) != null)
                    {
                       if (readLine != "")   //跳过空行                         
                       {
                            if (readLine.Substring(0, 1) == "[")   //是另一新节，则结束本次的读取                          
                            {
                                readEnd = true;
                                break;
                            }
                            keyWord = readLine.Split('=');
                            iniFile[keyWord[0].Trim()] = keyWord[1];
                        }
                    }
                }
                if (readEnd == true) break;
            }
            sr.Close();
        }
        catch       
        {
            iniFile.Clear();
        }
    }
}