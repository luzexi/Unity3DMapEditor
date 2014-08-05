public enum USEITEM_RESULT
{
	USEITEM_SUCCESS			        =	0,
	USEITEM_CANNT_USE		        =	1,
	USEITEM_LEVEL_FAIL		        =	2,
	USEITEM_TYPE_FAIL		        =	3,
	USEITEM_TARGET_TYPE_FAIL        =	4,
	USEITEM_SKILL_FAIL		        =	5,
	USEITEM_IDENT_TYPE_FAIL	        =	6, //卷轴类型错误
	USEITEM_IDENT_TARGET_TYPE_FAIL  =   7,//目标类型错误
	USEITEM_IDENT_LEVEL_FAIL	    =   8,//卷轴等级不够
    USEITEM_CANNT_USE_INBUS	        =   9,//在BUS上不能使用物品

	USEITEM_INVALID
};

public enum USEGEM_RESULT
{

	USEGEM_SUCCESS,
	USEGEM_CANNT_USE,
	USEGEM_LEVEL_FAIL,
	USEGEM_EQUIP_GEM_FULL,
	USEGEM_CANNT_COMBINE_GEM,
	USEGEM_FAIL
};

public enum REMOVEGEM_RESULT
{
	REMOVEGEM_SUCCESS,
	REMOVEGEM_NO_ITEM,
	REMOVEGEM_ERROR_GEMINDEX,
	REMOVEGEM_NO_GEM,
	REMOVEGEM_INVALID_EQUIP,
	REMOVEGEM_TOO_LARGE_GEMINDEX,
	REMOVEGEM_UNKNOW_ERROR,
	REMOVEGEM_BAG_FULL,
    REMOVEGEM_NO_MAT,       //8:需要宝石摘除符
    REMOVEGEM_SUCCESS1,     //9:成功（高级宝石摘除符）

};

//{----------------------------------------------------------------------------
// [2011-7-14] by: cfp+ 打孔结果
public enum ADDSLOT_RESULT
{
    ADDSLOT_ERROR   =   0,   //出错
    ADDSLOT_SUCCESS	=   1,
    ADDSLOT_EQUIP_FAIL,		//找不到装备
    ADDSLOT_ITEM_FAIL,
    ADDSLOT_ITEMNUM_FAIL,  //材料数量不满足
    ADDSLOT_FULL_FAIL,
    ADDSLOT_MONEY_FAIL,
    ADDSLOT_BAG_INVALID,
    ADDSLOT_FAIL,
};
//----------------------------------------------------------------------------}

public enum EQUIPLEVEUP_RESULT
{
    EQUIPLEVEUP_SUCCESS         =	0,  //成功
    EQUIPLEVEUP_ERROR           =   -1, //重大错误 
    EQUIPLEVEUP_EQUIP_FAIL      =   -2,//装备不可用
    EQUIPLEVEUP_COMMONEQUIP     =   -3,//强化精华不可用
    //EQUIPLEVEUP_FAIL_SUCCESS    =   -4,//
    //EQUIPLEVEUP_ITEMTYPE_FAIL   =   -5,
    //EQUIPLEVEUP_ITEMNUM_FAIL    =   -6,
    EQUIPLEVEUP_FULL_FAIL       =   -4,//该装备的强化等级已经最大
    EQUIPLEVEUP_MONEY_FAIL      =   -5,//金钱不足
    //EQUIPLEVEUP_BAG_INVALID     =   -6,
    EQUIPLEVEUP_FAIL            =   -6,//强化失败
};

public enum USE_EQUIPSKILL_RESULT
{
	USEEQUIPSKILL_SUCCESS			= 1,
	USEEQUIPSKILL_NO_ACTIVE_EFFECT,	
	USEEQUIPSKILL_FAIL
};


public enum UseEquipResultCode
{
	USEEQUIP_SUCCESS	=	1,
	USEEQUIP_LEVEL_FAIL,
	USEEQUIP_TYPE_FAIL,
	USEEQUIP_IDENT_FAIL,
	USEEQUIP_JOB_FAIL,
};

public enum LEVELUP_RESULT
{
	LEVELUP_SUCCESS,
	LEVELUP_NO_EXP,
};

//{----------------------------------------------------------------------------
// [2011-1-6] by: cfp+ GCLevelUp 升级类型
public enum LEVELUP_TYPE
{
    NORMAL_LEVELUP,
    AMBIT_LEVELUP,
    ALL_LEVELUP,
};
//----------------------------------------------------------------------------}


public enum ATTR_RESUlT
{
	ATTR_RESUlT_SUCCESS,
	ATTR_RESULT_NOT_ENOUGH_REMAIN_POINT,
	ATTR_RESULT_NO_SUCH_ATTR,
	ATTR_RESUlT_NO_POINT,
};


public enum PickResultCode
{
	PICK_SUCCESS,
	PICK_BAG_FULL,
	PICK_INVALID_OWNER,			
	PICK_INVALID_ITEM,			//拾取物品不存在
	PICK_TOO_FAR,				//角色距离太远
	PICK_DEAD,					//角色已经死亡
	PICK_CLOSED_BOX
};

public enum RATTR_RESULT
{
	RATTR_SUCCESS,
	RATTR_NO_POINT,
	RATTR_NO_LEVEL
};

public enum UnEquipResultCode
{
	UNEQUIP_SUCCESS	=	1,
	UNEQUIP_BAG_FULL,
	UNEQUIP_HAS_ITEM,

};

public enum DISCARDITEM_RESULT
{
	DISCARDITEM_SUCCESS,
	DISCARDITEM_FAIL
};


public enum LOGIN_CONNECT_RESULT
{
	LOGINCR_SUCCESS,			//成功
	LOGINCR_FULL,				//Login满
	LOGINCR_STOP_SERVICE,		//暂时停止服务
};

public enum LOGIN_RESULT
{
	LOGINR_SUCCESS,				//成功
	LOGINR_AUTH_FAIL,			//验证失败
	LOGINR_VERSION_FAIL,		//版本错误
	LOGINR_NOT_EN_POINT,		//点数不够
	LOGINR_STOP_SERVICE,		//暂时停止服务
	LOGINR_SERVER_BUSY,			//服务器忙，重试
	LOGINR_DBQUERY_FAIL,		//数据库无此人信息
};



public enum ASKCHARLIST_RESULT
{
	ASKCHARLIST_SUCCESS,		//成功
	ASKCHARLIST_OP_FAILS,		//操作失败
	ASKCHARLIST_SERVER_BUSY,	//服务器忙，重试
	ASKCHARLIST_OP_TIMES,		//操作过于频繁
};

public enum ASKCREATECHAR_RESULT
{
	ASKCREATECHAR_SUCCESS,			//成功
	ASKCREATECHAR_SERVER_BUSY,		//服务器忙，重试
	ASKCREATECHAR_OP_TIMES,			//操作过于频繁
	ASKCREATECHAR_FULL,				//角色已经满了
	ASKCREATECHAR_SAME_NAME,		//角色重名
	ASKCREATECHAR_OP_ERROR,			//错误操作流程
	ASKCREATECHAR_INTERNAL_ERROR,	//内部错误
	ASKCREATECHAR_INVALID_NAME,		//角色名错误
};

public enum ASKDELETECHAR_RESULT
{
	ASKDELETECHAR_SUCCESS,			//成功
	ASKDELETECHAR_SERVER_BUSY,		//服务器忙，重试
	ASKDELETECHAR_OP_TIMES,			//操作过于频繁
	ASKDELETECHARR_EMPTY,			//没有角色删除
	ASKDELETECHAR_OP_ERROR,			//错误操作流程
	ASKDELETECHAR_NOT_OWNER,		//不是角色的所有者
	ASKDELETECHAR_INTERNAL_ERROR,	//内部错误
	ASKDELETECHAR_ONLINE,			//角色在线
};


public enum ASKCHARLOGIN_RESULT
{
	ASKCHARLOGIN_SUCCESS,		//成功
	ASKCHARLIST_WORLD_FULL,		//世界满了
	ASKCHARLOGIN_SERVER_BUSY,	//服务器忙，重试
	ASKCHARLOGIN_LOADDB_ERROR,	//角色载入出错
	ASKCHARLOGIN_OP_TIMES,		//操作过于频繁
	ASKCHARLOGIN_NOT_OWNER,		//不是角色的所有者
	ASKCHARLOGIN_SERVER_STOP,	//服务器维护
	ASKCHARLOGIN_CHANGE_SCENE,	//切换场景
};


public enum CLIENT_TURN_STATUS
{
	CTS_TURN,					//排队状态
	CTS_NORMAL,					//排队完毕状态
};