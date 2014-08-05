/********************************************************************
	created:	2011/12/28
	created:	28:12:2011   17:10
	filename: 	SGWeb\Assets\Scripts\CursorManager\ICursorSystem.cs
	file path:	SGWeb\Assets\Scripts\CursorManager
	file base:	ICursorSystem
	file ext:	cs
	author:		Ivan
	
	purpose:	鼠标接口类，定义了鼠标类型和操作
*********************************************************************/
using System;

// namespace SGWEB
// {
    public enum ENUM_CURSOR_TYPE
    {
        CURSOR_WINBASE = 0,

        CURSOR_NORMAL,			//普通
        CURSOR_ATTACK,			//攻击
        CURSOR_AUTORUN,			//自动行走中
        CURSOR_PICKUP,			//拾取物品
        CURSOR_UNREACHABLE,		//无法到达的区域
        CURSOR_MINE,			//采矿
        CURSOR_HERBS,			//采药
        CURSOR_FISH,			//钓鱼
        CURSOR_SPEAK,			//谈话
        CURSOR_INTERACT,		//齿轮
        CURSOR_REPAIR,			//修理
        CURSOR_HOVER,			//鼠标激活(挂接物品...)
        CURSOR_IDENTIFY,		//鼠标鉴定
        CURSOR_ADDFRIEND,		//添加好友
        CURSOR_EXCHANGE,		//添加好友
        CURSOR_CATCH_PET,		//捉充
        CURSOR_HYPERLINK_HOVER, //悬浮于超链接之上

        CURSOR_NUMBER,

    }

// 	public class ICursorSystem
//     {
//         //设置鼠标光标
//         public virtual void SetCursor(ENUM_CURSOR_TYPE nType){}
//         //得到当前光标
//         public virtual ENUM_CURSOR_TYPE GetCursor() { return ENUM_CURSOR_TYPE.CURSOR_WINBASE; }
// 
// 	    //进入UI控制模式
//         public virtual void EnterUICursor(ENUM_CURSOR_TYPE hCursor) { }
//         public virtual void LeaveUICursor() { }
// 
// 	    //响应WM_SETCURSOR
//         public virtual void OnSetCursor() { }
// 
// 	    //显示/隐藏鼠标
//         public virtual void ShowCursor(bool bShow) { }
// 	}
/*}*/
