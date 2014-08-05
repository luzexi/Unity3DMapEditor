//===================================================
//游戏系统配置变量管理器

public abstract class tVariableSystem  : CNode
{


	//得到某个变量的值，如果不存在，则返回FALSE
	public abstract bool GetVariable(string szName, string strValue);
	//设置某个变量的值，如果不存在，则首先创建
    public abstract void SetVariable(string szName, string szValue, bool bTemp, bool bFireEvent);
    //默认 bTemp = true, bFireEvent = true
    public abstract void SetVariable(string szName, string szValue);
	//设置某个变量的值，但并不马上生效, 下一次启动时才会生效
    public abstract void SetVariableDelay(string szName, string szValue);

	//-------------------------------------------------------------------
	//快速设置通道

	//整数
    public abstract void SetAs_Int(string szName, int nValue, bool bTemp);
    //bTemp= TRUE
    public abstract void SetAs_Int(string szName, int nValue);

	//浮点数
    public abstract void SetAs_Float(string szName, float fValue, bool bTemp);
    // bTemp = TRUE;
    public abstract void SetAs_Float(string szName, float fValue);

	//Vector2
    public abstract void SetAs_Vector2(string szName, float fX, float fY, bool bTemp);
    //bTemp = TRUE
    public abstract void SetAs_Vector2(string szName, float fX, float fY);

	//-------------------------------------------------------------------
	//快速获取通道

	//字符串, DEF=""
    public abstract string GetAs_String(string szName);
    public abstract string GetAs_String(string szName, ref bool bHave);
	//整数, DEF=0
    public abstract int GetAs_Int(string szName);
    public abstract int GetAs_Int(string szName, ref bool bHave);
	//浮点数, DEF=0.0f
    public abstract float GetAs_Float(string szName);
    public abstract float GetAs_Float(string szName, ref bool bHave);
	//Vector2, DEF=(0.0f, 0.0f)
    public abstract fVector2 GetAs_Vector2(string szName);
    public abstract fVector2 GetAs_Vector2(string szName, ref bool bHave);

    public abstract void SetVariableDefault(string szName, string szValue);

    //save
    public abstract void Save();


};
