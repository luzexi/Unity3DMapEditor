using System.Collections.Generic;

using UnityEngine;


public class VariableSystem : tVariableSystem
{

    private const string INEXISTS = "Inexistent";
    private const int    INT_INEXISTS = int.MaxValue;
    private const float  FLOAT_INEXISTS = float.MaxValue;

    private List<string> tempKey = new List<string>();

    //////////////////////////////////////////////////////////////////////////
    //from interface 
    //得到某个变量的值，如果不存在，则返回FALSE
    public override bool GetVariable(string szName, string strValue)
    {
        strValue = PlayerPrefs.GetString(szName, INEXISTS);
        if (strValue == INEXISTS) return false;
        return true;
    }
    //设置某个变量的值，如果不存在，则首先创建
    public  override void SetVariable(string szName, string szValue, bool bTemp, bool bFireEvent)
    {
        if(bFireEvent)
        {
            string old = PlayerPrefs.GetString(szName);
            if(old != szValue)
            {
                //fire event
            }
        }
       
        PlayerPrefs.SetString(szName, szValue);

        //for delete
        if(bTemp)
        {
            PushInTemp(szName);
        }
    }
    //默认 bTemp = true, bFireEvent = true
    public  override void SetVariable(string szName, string szValue)
    {
        SetVariable(szName, szValue, true, true);
    }
    //设置某个变量的值，但并不马上生效, 下一次启动时才会生效
    //咱不实现
    public  override void SetVariableDelay(string szName, string szValue){}

    //-------------------------------------------------------------------
    //快速设置通道

    //整数
    public override  void SetAs_Int(string szName, int nValue, bool bTemp)
    {
        PlayerPrefs.SetInt(szName, nValue);
        if(bTemp)
            PushInTemp(szName);
    }
    //bTemp= TRUE
    public override  void SetAs_Int(string szName, int nValue)
    {
        SetAs_Int(szName, nValue, true);
    }

    //浮点数
    public override  void SetAs_Float(string szName, float fValue, bool bTemp)
    {
        PlayerPrefs.SetFloat(szName,  fValue);
        if(bTemp)
            PushInTemp(szName);
    }
    // bTemp = TRUE;
    public override  void SetAs_Float(string szName, float fValue)
    {
        SetAs_Float(szName, fValue, true);
    }

    //Vector2
    public  override void SetAs_Vector2(string szName, float fX, float fY, bool bTemp)
    {
        PlayerPrefs.SetFloat(szName + "_fX", fX);
        PlayerPrefs.SetFloat(szName +"_fY", fY);
        if(bTemp)
        {
            PushInTemp(szName + "_fX");
            PushInTemp(szName + "_fY");
        }
    }
    //bTemp = TRUE
    public  override void SetAs_Vector2(string szName, float fX, float fY)
    {
        SetAs_Vector2(szName, fX, fY, true);
    }

    //-------------------------------------------------------------------
    //快速获取通道

    //字符串, DEF=""
    public override string GetAs_String(string szName)
    {
        return PlayerPrefs.GetString(szName, INEXISTS);
    }
    public override  string GetAs_String(string szName, ref bool bHave)
    {
        string value = GetAs_String(szName);
        if(szName == INEXISTS) 
            bHave = false;
        return value;
    }
    //整数, DEF=0
    public  override int GetAs_Int(string szName)
    {
        return PlayerPrefs.GetInt(szName, INT_INEXISTS);
    }
    public  override int GetAs_Int(string szName, ref bool bHave)
    {
        int value = GetAs_Int(szName);
        if(value == INT_INEXISTS)
            bHave = false;
        return value;
    }
    //浮点数, DEF=0.0f
    public override  float GetAs_Float(string szName)
    {
        return PlayerPrefs.GetFloat(szName, FLOAT_INEXISTS);
    }
    public override  float GetAs_Float(string szName, ref bool bHave)
    {
        float value = GetAs_Float(szName);
        if(value == FLOAT_INEXISTS)
            bHave = false;
        return value;
    }
    //Vector2, DEF=(0.0f, 0.0f)
    public override  fVector2 GetAs_Vector2(string szName)
    {
        float fx = PlayerPrefs.GetFloat(szName+"_fX", FLOAT_INEXISTS);
        float fy = PlayerPrefs.GetFloat(szName+"_fY", FLOAT_INEXISTS);

        return new fVector2(fx, fy);
    }
    public  override fVector2 GetAs_Vector2(string szName, ref bool bHave)
    {
        fVector2 fv = GetAs_Vector2(szName);
        if(fv.x == FLOAT_INEXISTS)
            bHave = false;
        return fv;
    }

    public  override void SetVariableDefault(string szName, string szValue)
    {
        if(PlayerPrefs.HasKey(szName))
            return;

        PlayerPrefs.SetString(szName, szValue);
    }
    public  override void Save()
    {
        DeleteTemp();

        PlayerPrefs.Save();
    }
    //
    //////////////////////////////////////////////////////////////////////////

   
    void DeleteTemp()
    {
        foreach (string key in tempKey)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    void PushInTemp(string key)
    {
        if(!tempKey.Contains(key))
            tempKey.Add(key);
    }
}