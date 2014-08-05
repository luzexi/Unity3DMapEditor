using System;
using System.Collections.Generic;

// login server 状态.
public enum LOGIN_SERVER_STATUS
{
	LOGIN_SERVER_FULL	= 0, // 爆满
	LOGIN_SERVER_BUSY,		 // 拥挤
	LOGIN_SERVER_NORMAL,	 // 正常
	LOGIN_SERVER_IDLE,		 // 空闲
	LOGIN_SERVER_STOP,		 // 维护

};

// 网络接入商
public enum NET_PROVIDE
{
	NET_PROVIDE_CNII	= 0, // 电信
	NET_PROVIDE_CNC,		 // 网通
	NET_PROVIDE_OTHER,		 // 其他

};

// 推荐服务器
public enum SERVER_COMMEND
{
	NORMAL	= 0, // 正常
	COMMEND,	 // 推荐
};


// login server 结构信息
public struct LOGINSERVER_INOF
{
	public string szLoginServerName;						// login server的名字
    public string szIp;									// ip地址				
    public int iPort;										// 端口号
    public LOGIN_SERVER_STATUS iLoginServerStatus;		// login server的状态.
    public SERVER_COMMEND sCommend;						// 推荐服务器
};
//typedef __LOGINSERVER_INOF LOGINSERVER_INOF, *PLOGINSERVER_INOF;


// 区域服务器结构信息
public struct AREA_INFO	//区域服务器信息
{
	public string szAreaName;					// 区域名字
	public NET_PROVIDE iProvide;				// 网络供应商
    public List<LOGINSERVER_INOF> LoginInfo;	// login server的具体信息

};
//typedef __AREA_INFO AREA_INFO, *PAREA_INFO;



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// 自动选择服务器需要的数据
//

// login server 服务器的的坐标索引
public struct LOGIN_SERVER_POS
{
	int iAreaIndex;
	int iLoginServerIndex;
};
//typedef __LOGIN_SERVER_POS LOGIN_SERVER_POS, *PLOGIN_SERVER_POS;

// Login server 数组
//typedef List<LOGIN_SERVER_POS> LOGIN_SERVER_POS_VECTOR;

// 网络供应商信息
//struct __PROVIDE_INFO
//{
//    LOGIN_SERVER_POS_VECTOR	IdleLoginServerVector;
//    LOGIN_SERVER_POS_VECTOR	NorLoginServerVector;
//    LOGIN_SERVER_POS_VECTOR	BusyLoginServerVector;
//    LOGIN_SERVER_POS_VECTOR	FullLoginServerVector;		
//};

//typedef __PROVIDE_INFO PROVIDE_INFO, *PPROVIDE_INFO;