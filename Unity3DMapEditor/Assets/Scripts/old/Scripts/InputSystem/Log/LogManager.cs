using System;
using System.Collections.Generic;
using UnityEngine;

public class LogManager
{
    enum LogLevel
    {
        Disable = -1,
        Info = 0,
        Warning = 1,
        Error = 2
    }

    static LogLevel outLevel = LogLevel.Info;

    /// <summary>
    /// 打印所有的消息
    /// </summary>
    public static void EnableInfo()
    {
        outLevel = LogLevel.Info;
    }

    /// <summary>
    /// 打印Warning和Error
    /// </summary>
    public static void EnableWarning()
    {
        outLevel = LogLevel.Warning;
    }

    /// <summary>
    /// 只打印Error
    /// </summary>
    public static void EnableError()
    {
        outLevel = LogLevel.Error;
    }

    /// <summary>
    /// 禁用所有打印
    /// </summary>
    public static void DisableAll()
    {
        outLevel = LogLevel.Disable;
    }

    static int currDebugState = -1;

    static bool IsLevelEnable(LogLevel current)
    {
        if (currDebugState == -1)
        {
            if (Debug.isDebugBuild)
                currDebugState = 1;
            else
                currDebugState = 0;
        }

        if (currDebugState != 1)
            return false;
        if (outLevel == LogLevel.Disable)
            return false;

        if ((int)current >= (int)outLevel)
            return true;
        
        return false;
    }

    public static void Log(string msg)
    {
        if (IsLevelEnable(LogLevel.Info))
            Debug.Log(msg);
    }
    
    public static void LogWarning(string msg)
    {
        if (IsLevelEnable(LogLevel.Warning))
		{
           
			Debug.LogWarning(GameProcedure.s_pTimeSystem.GetTimeNow() + " : "+msg);
		}
    }

    public static void LogError(string msg)
    {
        if (IsLevelEnable(LogLevel.Error))
            Debug.LogError(msg);
    }
}