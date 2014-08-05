using System;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Packets;

public struct SCRIPT_DEFINE
{
    public static readonly int MAX_PARAM_SZIE = 16;
    public static readonly int MAX_STR_SIZE   = 512;
    public static readonly int MAX_FUNC_NAME_SIZE = 64;
    public static readonly int MAX_INT_PARAM_COUNT = 6;
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class X_PARAM : ClassCanbeSerialized
{
    byte m_IntCount;//整型参数数量
    byte m_StrCount;//字符型参数数量
    int[] m_aIntValue = new int[SCRIPT_DEFINE.MAX_PARAM_SZIE];//整形数据
    short[] m_aStrOffset = new short[SCRIPT_DEFINE.MAX_PARAM_SZIE];//字符型参数间隔（\0）在字符缓存中的偏移位置
    byte[] m_aStrValue = new byte[SCRIPT_DEFINE.MAX_STR_SIZE];//字符缓存

    public X_PARAM()
    {
        CleanUp();
    }


    void CleanUp()
    {

    }

    public int getSize()
    {
        return 0;
    }
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        return true;
    }
    public int writeToBuff(ref NetOutputBuffer buff)
    {
        return getSize();
    }

    //public Properties
    public byte IntCount//取得X_PARAM中的整数型参数数量
    {
        get { return m_IntCount; }
    }
    public byte StrCount//取得X_PARAM中的字符串参数个数
    {
        get { return m_StrCount; }
    }

    public int GetIntValue(int nIndex) //取得第nIndex个X_PARAM中的整数型参数
    {
        if (nIndex < 0 || nIndex >= SCRIPT_DEFINE.MAX_PARAM_SZIE)
            return 0;
        if (nIndex >= IntCount)
            return 0;

        return m_aIntValue[nIndex];
    }

    //CHAR*		GetStrValue( INT nIndex ) ;//取得X_PARAM中的第nIndex个字符串参数

    public int			AddIntValue( int nValue )//向X_PARAM中添加一个整型参数
    {
        	if( m_IntCount>= SCRIPT_DEFINE.MAX_PARAM_SZIE-1 )
		        return -1 ;

	        m_IntCount ++ ;

	        m_aIntValue[m_IntCount-1]=nValue ;

	        return m_IntCount-1 ;
    }
                                           //返回值为此参数的序号
    public int			SetIntValue( int nIndex, int nValue )//修改X_PARAM中的第nIndex个整形参数
    {
        if( nIndex<0 || nIndex>=SCRIPT_DEFINE.MAX_PARAM_SZIE )
		    return -1 ;
	    if( nIndex>=m_IntCount )
		    return -1;

	    m_aIntValue[nIndex]=nValue ;

	    return nIndex ;
    }

    //public int			AddStrValue( const CHAR* szIn ) ;//向X_PARAM中添加一个字符串型参数
    //                                       //返回值为此参数的序号
    //public int			SetStrValue( INT nIndex, CHAR* szIn ) ;//修改X_PARAM中的第nIndex个字符串型参数
};


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class X_SCRIPT : ClassCanbeSerialized
{
    int	        m_ScriptID ;
    byte		m_uFunNameSize ;
    byte[]		m_szFunName = new byte[SCRIPT_DEFINE.MAX_FUNC_NAME_SIZE];
//	BYTE		m_uStrParamSize ;
//	CHAR		m_szStrParam[MAX_STRING_PARAM_SIZE] ;
    byte		m_uParamCount ;
    int[]		m_aParam = new int[SCRIPT_DEFINE.MAX_INT_PARAM_COUNT] ;


    public int getSize()
    {
        return sizeof(int) + sizeof(byte) * (2 + m_uFunNameSize) + sizeof(int) * m_uParamCount;
    }
    public static int getMaxSize()
    {
        return sizeof(int) + sizeof(byte) * (2 + SCRIPT_DEFINE.MAX_FUNC_NAME_SIZE) + sizeof(int) * SCRIPT_DEFINE.MAX_INT_PARAM_COUNT;
    }
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        return true;
    }
    public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteInt(m_ScriptID);
        buff.WriteByte(m_uFunNameSize);
        if (FunNameSize > 0)
        {
            buff.Write(ref m_szFunName, sizeof(byte) * (int)m_uFunNameSize);
        }
        buff.WriteByte(m_uParamCount);
        if (m_uParamCount > 0)
        {
            for (byte i = 0; i < m_uParamCount; i++ )
            {
                buff.WriteInt(m_aParam[i]);
            }
        }
        return getSize();
    }
    public void CleanUp()
    {
        m_ScriptID = MacroDefine.INVALID_ID;
        m_uFunNameSize = 0;
        //m_uStrParamSize = 0 ;
        m_uParamCount = 0;

    }

    public int ScriptID
    {
        get{return m_ScriptID;}
        set{m_ScriptID = value;}
    }
    public byte[] FunName
    {
        get{return m_szFunName;}
        set{
            Array.Copy(value, m_szFunName, value.Length);
            m_uFunNameSize = (byte)value.Length;
        }
    }
    public byte FunNameSize
    {
        get{return m_uFunNameSize;}
    }


    //VOID		SetStrParam( char* szstrparam )
    //{
    //	strncpy( m_szStrParam, szstrparam, MAX_STRING_PARAM_SIZE-1 ) ;
    //	m_uStrParamSize = (BYTE)strlen(m_szStrParam) ;
    //};
    //CHAR*		GetStrParam( ){ return m_szStrParam ; }
    //INT			GetStrParamSize( ){ return m_uStrParamSize ; }

    public byte ParamCount
    {
        get { return m_uParamCount; }
        set { m_uParamCount = value; }
    }
    public int this[int index]
    {
        get
        {
            if (index >= SCRIPT_DEFINE.MAX_INT_PARAM_COUNT)
                return 0;
            return m_aParam[index];
        }
        set
        {
            if (index >= SCRIPT_DEFINE.MAX_INT_PARAM_COUNT)
                return;
            m_aParam[index] = value;
        }
    }

};
