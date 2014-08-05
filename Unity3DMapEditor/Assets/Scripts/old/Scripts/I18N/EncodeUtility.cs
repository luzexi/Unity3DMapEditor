using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DBSystem;

public class EncodeUtility
{
    static readonly EncodeUtility instance = new EncodeUtility();
    public static EncodeUtility Instance
    {
        get { return instance; }
    }

    DBC.COMMON_DBC<EncodeBase> gb2Unicode;

    Dictionary<int, int> uni2gbTable = new Dictionary<int, int>();
    private EncodeUtility()
    {
        gb2Unicode = CDataBaseSystem.Instance.GetDataBase<EncodeBase>((int)DataBaseStruct.DBC_CODE_GB2UNI);

        foreach (EncodeBase code in gb2Unicode.StructDict.Values)
        {
            uni2gbTable.Add(code.tarCode, code.orgCode);
        }
    }

    char ChangeCharFromUni2Gb(char uniChar)
    {
        int gbChar = 0;
         if (!uni2gbTable.TryGetValue(uniChar, out gbChar))
         {
             //LogManager.LogWarning("字符集转换表缺少Unicode字符："+ uniChar);
         }
        return Convert.ToChar(gbChar);
    }

    bool IsGbCode(byte[] bytes)
    {
        if (bytes.Length >= 2)
        {
            return IsGbCode(bytes[0], bytes[1]);
        }
        return false;
    }

    bool IsGbCode(byte b1, byte b2)
    {
        if (b1 >= 129 && b1 <= 254 && b2 >= 64 && b2 <= 254)
            return true;
        else
            return false;
    }

    string ChangeGb2Unicode(byte[] gbSource)
    {
        if (gb2Unicode == null)
        {
            LogManager.LogError("没有加载gb2Unicode编码表");
            return "";
        }
        StringBuilder sb = new StringBuilder();
        ushort gbCode = 0;
        for (int i = 0; i < gbSource.Length; )
        {
            if (i < gbSource.Length - 1 &&
                (IsGbCode(gbSource[i], gbSource[i + 1])))
            {
                gbCode = 0;
                gbCode |= (ushort)(gbSource[i] << 8);
                gbCode |= gbSource[i + 1];

                EncodeBase code = gb2Unicode.Search_Index_EQU(gbCode);
                if (code != null)
                    sb.Append((char)code.tarCode);
                else
                    sb.Append("_");

                i += 2;
            }
            else
            {
                sb.Append((char)gbSource[i]);

                i += 1;
            }
        }
        return sb.ToString();
    }

    public string GetUnicodeString(byte[] source)
    {
        try
        {
            //if (IsGbCode(source))
            {
                return ChangeGb2Unicode(source);
            }
            //else
            //{
            //    return Encoding.UTF8.GetString(source);
            //}
        }
        catch (Exception ex)
        {
            LogManager.LogError(ex.ToString());
            return "";
        }
    }

    public byte[] GetBytes(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// 只接受Unicode编码的字符串
    /// </summary>
    /// <param name="unicodeString"></param>
    /// <returns></returns>
    public byte[] GetGbBytes(string unicodeString)
    {
        byte[] gbBytes = new byte[unicodeString.Length*2];
        char[] unicodeChars = unicodeString.ToCharArray();
        int j = 0;
        for (int i = 0; i < unicodeChars.Length; i++)
        {
            char gbChar = ChangeCharFromUni2Gb(unicodeChars[i]);

            // 转换失败,强制使用原有编码
            if (gbChar == char.MinValue)
                gbChar = unicodeChars[i];

            byte highByte = Convert.ToByte(gbChar >> 8);
            byte lowByte = Convert.ToByte((byte)gbChar);
            if (highByte != byte.MinValue)
                gbBytes[j++] = highByte;
            if (lowByte != byte.MinValue)
                gbBytes[j++] = lowByte;
        }
        return gbBytes;
    }
}

