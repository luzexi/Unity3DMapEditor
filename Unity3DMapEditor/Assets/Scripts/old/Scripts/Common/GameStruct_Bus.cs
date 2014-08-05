using System;
using System.Collections.Generic;
using System.Text;

public class GameStruct_Bus
{
    // <=此值为无效高度
    public const int DEF_BUS_PATH_NODE_INVALID_Y = -100000;

    // 最大乘客数量
    public const int DEF_BUS_MAX_PASSENGER_COUNT = 8;

    // 提供给脚本用的参数数量
    public const int DEF_BUS_SCRIPT_PARAM_BUF_SIZE = 8;

    // 技能数量
    public const int BUS_MAX_SKILL = 12;
}

public enum ENUM_BUS_TYPE
{
    BUS_TYPE_INVALID = -1,
    BUS_TYPE_ONE_WAY,				// 单程车
    BUS_TYPE_SHUTTLE,				// 往返车

    BUS_TYPE_NUMBERS
};

public enum ENUM_BUS_OWNER_TYPE
{
    BUS_TYPE_OWNER_INVALID = -1,
    BUS_HUMAN,				// 属于玩家个人
    BUS_TEAM,				// 属于队伍
    BUS_GUILD,              // 属于公会

    BUS_OWNER_TYPE_NUMBERS
};

/////////////////////////////////////////////////////////
// BUS 载具（客户端用到结构）
/////////////////////////////////////////////////////////
public struct _BUS_SEAT_INFO
{
    public int		m_nActionSetIndex;			// 在CharModel.txt中的动作组索引
    public string m_pszLocator;				// 乘客坐位的绑定点名称
};

public class _BUS_INFO
{
    public int m_nDataID;					// 数据编号
    public string m_pszName;				// 名称
    public int m_nScriptID;				    // 脚本ID
    public int m_nType;					    // 类型 ENUM_BUS_TYPE
    public int m_nStopTime;				    // 停站时间
    public int m_nModelID;					// 模型数据ID
    public float m_fModelHeight;			// 模型高度
    public int m_nMaxPassengerCount;		// 准乘人数
    public float m_fMoveSpeed;				// 行驶速度
    public _BUS_SEAT_INFO[] m_aSeatInfo = new _BUS_SEAT_INFO[GameStruct_Bus.DEF_BUS_MAX_PASSENGER_COUNT];	// 乘客坐位的绑定点名称

    //////////////////////////////////////////////////////////////////////////
    //战斗载具 基本属性
    public byte ucIsFight;                  //是否战斗载具
    public int cOwnerType;                 //归属类型(-1为无归属限制，0:个人，1为组队、2为帮会)
    public int uRequireLevel;              //需求等级
    public int cRequireMenpai;             //需求五行
    public int ucRequireAmbit;             //需求境界
    //////////////////////////////////////////////////////////////////////////
    //战斗属性
    public int iMAXHP;                      //MAXHP
    public int iPhysicAttack;               //物理攻击
    public int iPhysicDefence;              //物理防御
    public int iMagicAttack;                //法术攻击
    public int iMagicDefence;               //法术防御
    public int iHit;                       //命中率
    public int iMiss;                      //闪避率
    public int iCritical;                  //会心率
    public int[] iSkilID = new int[GameStruct_Bus.BUS_MAX_SKILL];     //技能ID（前6个为主控，后6个为副控）

	public _BUS_INFO()
	{
		CleanUp();
	}

	void CleanUp()
	{
		m_nDataID					= MacroDefine.INVALID_ID;
		m_nScriptID					= MacroDefine.INVALID_ID;
		m_pszName					= "";
        m_nType                     = (int)ENUM_BUS_TYPE.BUS_TYPE_INVALID;
		m_nStopTime					= 0;
		m_nModelID					= MacroDefine.INVALID_ID;
		m_fModelHeight				= -1.0f;
		m_nMaxPassengerCount		= MacroDefine.INVALID_ID;
		m_fMoveSpeed				= -1.0f;
        Array.Clear(m_aSeatInfo, 0, m_aSeatInfo.Length);

        //////////////////////////////////////////////////////////////////////////
        //战斗载具 基本属性
        ucIsFight                   = 0;
        cOwnerType                  = MacroDefine.INVALID_ID;
        uRequireLevel               = MacroDefine.INVALID_ID;
        cRequireMenpai              = MacroDefine.INVALID_ID;
        ucRequireAmbit              = MacroDefine.INVALID_ID;
        //////////////////////////////////////////////////////////////////////////
        //战斗属性
        iMAXHP                      = MacroDefine.INVALID_ID;
        iPhysicAttack               = MacroDefine.INVALID_ID;
        iPhysicDefence              = MacroDefine.INVALID_ID;
        iMagicAttack                = MacroDefine.INVALID_ID;
        iMagicDefence               = MacroDefine.INVALID_ID;
        iHit                        = MacroDefine.INVALID_ID;
        iMiss                       = MacroDefine.INVALID_ID;
        iCritical                   = MacroDefine.INVALID_ID;

        for (int i = 0; i < GameStruct_Bus.BUS_MAX_SKILL; i++)
        {
            iSkilID[i] = MacroDefine.INVALID_ID;
        }
	}
};