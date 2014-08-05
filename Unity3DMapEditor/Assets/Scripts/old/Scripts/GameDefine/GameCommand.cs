/********************************************************************
	created:	2011/12/29
	created:	29:12:2011   10:11
	filename: 	SGWeb\Assets\Scripts\GameDefine\GameCommand.cs
	file path:	SGWeb\Assets\Scripts\GameDefine
	file base:	GameCommand
	file ext:	cs
	author:		Ivan
	
	purpose:	这个类定义了游戏的基本命令类型，包括鼠标命令等
*********************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

// namespace SGWEB
// {
    // 命令类型
    public enum ENUM_GAME_COMMAD_TYPE
    {
        GAME_COMMAD_TYPE_INVALID = -1,
        GAME_COMMAD_TYPE_WORLD,
        GAME_COMMAD_TYPE_OBJECT,
        GAME_COMMAD_TYPE_AI,
        GAME_COMMAD_TYPE_DPC,			// Data pool
        GAME_COMMAD_TYPE_UI,
        GAME_COMMAD_TYPE_MOUSE,
    }

    // AI命令
    public enum AICommandDef
    {
        // 无效的ID
        AIC_NONE = 0,

        // 寻路移动
        // F0	:	目标点X坐标
        // F1	:	目标点Z坐标
        AIC_MOVE,

        // 使用技能
        // D0	:	技能ID
        // D1	:	目标ID
        // F2	:	目标点X坐标
        // F3	:	目标点Z坐标
        // F4	:	方向
        AIC_USE_SKILL,

        // 对某个Tripper点进行操作(箱子，矿物...)
        // D0	:	目标ID
        AIC_TRIPPER_ACTIVE,

        // 缺省事件
        // D0	:	目标ID
        AIC_DEFAULT_EVENT,

        // 使用某项合成技能(厨艺，制造...)
        // D0	:	目标ID
        AIC_COMPOSE_ITEM,

        // 跳跃
        AIC_JUMP,

        // 跟随
        AIC_FOLLOW,

        //挂机
        // D0	:	1为开始挂机
        AIC_GUAJI,

        //直线行走
        // D0 : 每次前进距离，+向前走，-向后走
        AIC_MOVE_ALONG,

        // 增加延时命令，用于移动到目的后再执行命令 [4/27/2011 ivan edit]
        AIC_CMD_AFTERMOVE
    }

    // Run Command 的返回值
    public enum RC_RESULT
    {
        RC_OK = 0,
        RC_ERROR,
        RC_SKIP,
        RC_WAIT, // 等待
    }

    // 鼠标命令类型
    public enum MOUSE_CMD_TYPE
    {
        MCT_NULL,

        MCT_PLAYER_MOVETO,	//移动到目标点
        //m_afParam[0][1] = The position of target area

        MCT_PLAYER_SELECT,	//选中物体
        //m_adwParam[0] = The ID of target object

        MCT_SKILL_OBJ,		//点选目标的技能
        //m_adwParam[0] = The ID of skill
        //m_adwParam[1] = The ID of target object
        MCT_SKILL_AREA,		//范围技能
        //m_adwParam[0] = The ID of skill
        //m_afParam[1][2] = The position of target area
        MCT_CANCEL_SKILL,	//取消当前Action技能，设置为缺省技能
        MCT_SKILL_DIR,		//方向技能
		MCT_SKILL_SELF,//只针对自己的技能
		MCT_SKILL_NONE,//只无需选择技能
        //m_adwParam[0] = The ID of skill
        //m_afParam[1] = The direction

        MCT_HIT_TRIPPEROBJ,	//放在能够操作的TripperObj上(矿物，箱子...)
        //m_adwParam[0] = The ID of Tripper obj
        //...

        MCT_SPEAK,			//和NPC谈话
        //m_adwParam[0] = The ID of NPC

        MCT_CONTEXMENU,		//显示右键菜单
        //m_adwParam[0] = The ID of NPC

        MCT_REPAIR,			//修理指针
        MCT_CANCLE_REPAIR,	//修理指针

        MCT_USE_ITEM,		//物品使用
        //m_apParam[0] = Action Item
        //m_adwParam[1] = The ID of target object
        //m_afParam[2] = Pos X
        //m_afParam[3] = Pos Z
        //m_adwParam[4] = Is area

        MCT_CANCEL_USE_ITEM,//取消物品使用

        MCT_ENTER_BUS,// 进入载具 [8/26/2011 ivan edit]

        MCT_EXIT_BUS,// 离开载具 [8/26/2011 ivan edit]

        //
        // 鼠标在界面上的命令状态 2006－3－29
        //
        MCT_UI_USE_IDENTIFY,			// 使用鉴定卷轴 	
        MCT_UI_USE_CANCEL_IDENTIFY,		// 取消使用鉴定卷轴 		

        MCT_UI_USE_ADDFRIEND,			//添加好友
        MCT_UI_USE_CANCEL_ADDFRIEND,	//取消添加好友

        MCT_UI_USE_EXCHANGE,			//添加好友
        MCT_UI_USE_CANCEL_EXCHANGE,	//取消添加好友

        MCT_LEAP,						//轻功

        MCT_CATCH_PET,				// 捉宠

        MCT_SALE,					// 卖东西
        MCT_CANCEL_SALE,			// 取消买东西

        MCT_BUYMULT,				// 买东西
        MCT_CANCEL_BUYMULT,			// 取消买东西

        MCT_HOVER,					// 悬浮
        MCT_HyperLink,              // UI超链接
    };

    //游戏中逻辑指令基类
    //[StructLayout(LayoutKind.Explicit, Size = 64)]
//     public struct Params
//     {
//         //[FieldOffset(0)]
// //         public uint[] m_adwParam;
// //         //[FieldOffset(0)]
// //         public uint[] m_auParam;
// //        // [FieldOffset(0)]
// //         public float[] m_afParam;
// //         //[FieldOffset(0)]
// //         public int[] m_anParam;
// //         //[FieldOffset(0)]
// //         public bool[] m_abParam;
// //         //[FieldOffset(0)]
// //         public UInt64[] m_auqParam;
// //        // [FieldOffset(0)]
// //         public Int64[] m_aqParam;
// //         // 引用类型分开放置
//          private object[] m_apParam;
// 
//         public Params(int paSize)
//         {
// //             m_adwParam = new uint[paSize];
// //             m_auParam = new uint[paSize];
// //             m_afParam = new float[paSize];
// //             m_anParam = new int[paSize];
// //             m_abParam = new bool[paSize];
// //             m_auqParam = new UInt64[paSize / 2];
// //             m_aqParam = new Int64[paSize / 2];
//              m_apParam = new object[paSize];
//         }
// 
//         public void SetValue( int index, int iValue)
//         {
//             m_apParam[index] = (object)iValue;
//         }
// 
//         public void SetValue(int index, float fValue)
//         {
//             m_apParam[index] = (object)fValue;
//         }
// 
//         public void SetValue(int index, object obj)
//         {
//             m_apParam[index] = obj;
//         }
// 
//         public object GetValue<>(int index)
//         {
//             return m_apParam[index];
//         }
// 
//         public void CleanUp()
//         {
//             Array.Clear(m_apParam, 0, m_apParam.Length);
//         }
//     };

    //游戏中逻辑指令基类
    public class SCommand
    {
        //////////////////////////////////////////////////////////////////////////////
        // 通用的函数模板
        //////////////////////////////////////////////////////////////////////////////
        private object[] m_apParam;

        public void SetValue<Type>(int index, Type fValue)
        {
            m_apParam[index] = fValue;
        }

        public T GetValue<T>(int index)
        {
            try
            {
                T value = (T)m_apParam[index];
                return value;
            }
            catch (InvalidCastException ex)
            {
                LogManager.LogError(ex.ToString());
                throw ex;
            }
        }
        //////////////////////////////////////////////////////////////////////////////
        public const int MAX_OBJ_CMD_PARAM_NUM = 16;
        //命令类型
        public int m_wID;
        protected ENUM_GAME_COMMAD_TYPE eType;

        public SCommand()
        {
            eType = ENUM_GAME_COMMAD_TYPE.GAME_COMMAD_TYPE_INVALID;

            m_apParam = new object[MAX_OBJ_CMD_PARAM_NUM];
        }

        public ENUM_GAME_COMMAD_TYPE Type()
        {
            return eType;
        }

        public void CleanUp()
        {
            m_wID = 0;

            Array.Clear(m_apParam, 0, m_apParam.Length);
        }
    }


    //应用于object的命令
    public class SCommand_Object : SCommand
    {
        public SCommand_Object()
        {
            eType = ENUM_GAME_COMMAD_TYPE.GAME_COMMAD_TYPE_OBJECT;
        }
    };

    //应用于AI的命令
    public class SCommand_AI : SCommand
    {
        public SCommand_AI()
        {
            eType = ENUM_GAME_COMMAD_TYPE.GAME_COMMAD_TYPE_AI;
        }
    };

    //应用于UI的命令
    public class SCommand_DPC : SCommand
    {
        public SCommand_DPC()
        {
            eType = ENUM_GAME_COMMAD_TYPE.GAME_COMMAD_TYPE_DPC;
        }
    };

    //挂在鼠标上的命令
    public class SCommand_Mouse : SCommand
    {

        public MOUSE_CMD_TYPE m_typeMouse;
        public SCommand_Mouse()
        {
            eType = ENUM_GAME_COMMAD_TYPE.GAME_COMMAD_TYPE_MOUSE;
        }
    };


/*}*/
