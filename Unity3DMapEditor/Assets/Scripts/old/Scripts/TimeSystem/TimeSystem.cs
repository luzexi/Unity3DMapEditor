/********************************************************************
	created:	2011/12/28
	created:	28:12:2011   14:04
	filename: 	TimeSystem.cs
	file path:	SGWeb\Assets\Scripts\TimeSystem
	file base:	TimeSystem
	file ext:	cs
	author:		Ivan
	
	purpose:	封装了unity3d的时间系统
*********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

// namespace SGWEB
// {
	public class TimeSystem
	{
        private float timeNow;		    //当前时间
        private float loopTime;		    //最近20ms以来每祯平均花费的时间
        private float deltaTime;		//上一桢所花费的时间
        private float realTime;		    //取得根据loopTime累积的系统运行时间
        private float fps;			    //当前的桢率
        private uint  tickCount;		//桢总数

        private float MAXTIME_CAL_LOOPTIME = 0.02F;//每20ms计算一次每桢花费的时间
        private static uint timeLoopCnt = 0;		//桢累计计数
        private static float timeLoopHop = 0;		//时间累计计数

        private float MAXTIME_CAL_FPS = 1;          //每1秒计算一次fps
        private static uint frameLoopCnt = 0;		//桢累计计数
        private static float frameLoopHop = 0;		//时间累计计数

        //取得最近20ms以来每祯平均花费的时间
        public float GetLoopTime() { return loopTime; }
        //取得上一祯花费的时间
        public uint GetDeltaTime() { return (uint)(deltaTime*1000.0f); }
        //取得累积的系统运行时间
        public float GetRealTime() { return realTime; }
        //取得当前时间
        public uint GetTimeNow() { return (uint)(timeNow * 1000.0f); }
        //取得桢率
        public float GetFPS() { return fps; }
        //得到桢数
        public uint GetTickCount() { return tickCount; }

        public void Initial()
        {
            loopTime = 0.0f;
            deltaTime = 0.0f;
            fps = 0.0f;
            tickCount = 0;

            realTime = timeNow = Time.realtimeSinceStartup;
        }

        public void Tick()
        {
            ++tickCount;
            // 全部使用unity内置的时间系统 [12/28/2011 Ivan]
            timeNow = Time.time;
            deltaTime = Time.deltaTime;
            realTime = Time.realtimeSinceStartup;

            //-------------------------------------------------------------
            //计算最近20ms以来每祯平均花费的时间
	        {
                timeLoopCnt++;
                timeLoopHop += deltaTime;
                if (timeLoopHop > MAXTIME_CAL_LOOPTIME)		
		        {
                    loopTime = (timeLoopHop / (float)timeLoopCnt) ;

                    timeLoopCnt = 0;
                    timeLoopHop = 0;
		        }
	        }
            //-------------------------------------------------------------
            //计算桢率
            {
                frameLoopCnt++;
                frameLoopHop += deltaTime;
                if (frameLoopHop > MAXTIME_CAL_FPS)
                {
                    fps = (float)frameLoopCnt / frameLoopHop;

                    frameLoopCnt = 0;
                    frameLoopHop = 0;
                }
            }
        }

        public uint CalSubTime(uint timeStart, uint timeEnd)
        {
	        if(timeEnd < timeStart) 
		        return ((uint)0xffffffff-timeStart) + timeEnd;
	        else 
		        return timeEnd-timeStart;
        }
	}
/*}*/
