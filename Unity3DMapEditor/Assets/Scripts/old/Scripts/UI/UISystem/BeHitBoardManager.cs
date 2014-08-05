using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 文字运动类型 [4/19/2011 ivan edit]
public enum ENUM_DMG_MOVE_TYPE
{
    MOVE_INVALID = -1,
    MOVE_STATUS,
    MOVE_DAMAGE_OTHER,
    MOVE_DAMAGE_ME,
    MOVE_HEAL_HP,
    MOVE_HEAL_MP,
    MOVE_SCENE_NAME,
}

// 伤害显示类型，如暴击，普通攻击等 [12/28/2010 ivan edit]
public enum ENUM_DAMAGE_TYPE
{
    DAMAGE_INVALID = -1,
    DAMAGE_CRITICAL,
    DAMAGE_NORMAL,
    DAMAGE_MISS,
    DAMAGE_IMMU,
    DAMAGE_ABSORB,
    DAMAGE_COUNTERACT,
    DAMAGE_TRANSFERED,
    DAMAGE_ENTER_FIGHT,
    DAMAGE_LEAVE_FIGHT,
    DAMAGE_Exp,
};

public enum ENUM_DAMAGE_OPERATION
{
    OPERATION_INVALID = MacroDefine.INVALID_ID,
    OPERATION_ADD,
    OPERATION_SUB
};

public class BeHitBoardManager
{
    static readonly BeHitBoardManager sInstance = new BeHitBoardManager();

    static public BeHitBoardManager Instance { get { return sInstance; } }

    float m_fNormalSize;
    public float NormalSize
    {
        get { return m_fNormalSize; }
    }
    float m_fDoubleSize;
    public float DoubleSize
    {
        get { return m_fDoubleSize; }
    }
    float m_fDoubleEndSize;
    public float DoubleEndSize
    {
        get { return m_fDoubleEndSize; }
    }
    float m_fDoubleTime;
    public float DoubleTime
    {
        get { return m_fDoubleTime; }
    }
    private BeHitBoardManager()
    {
        InitAllMoveData();

        m_fNormalSize = 1;
        m_fDoubleSize = 2;
        m_fDoubleEndSize = 1;
        m_fDoubleTime = 1000;
    }

    Queue<BeHitBoard> freeBoards = new Queue<BeHitBoard>();
    //所有信息板的链表
    List<BeHitBoard> allBoards = new List<BeHitBoard>();

    public BeHitBoard GetFreeBoard()
    {
        if (freeBoards.Count != 0)
        {
            return freeBoards.Dequeue();
        }
        else
        {
            return CreateNewBoard();
        }
    }

    public void FreeBoard(BeHitBoard oldWindow)
    {
        freeBoards.Enqueue(oldWindow);
        allBoards.Remove(oldWindow);
        oldWindow.InUse = false;
    }

    static int WindowID = 100;
    private BeHitBoard CreateNewBoard()
    {
        BeHitBoard behit = new BeHitBoard(WindowID++);
        behit.InUse = true;
        return behit;
    }

    const int MaxHitCount = 100;
    internal void AddNewBeHit(bool bDouble, string dmgText, float x, float y, ENUM_DAMAGE_TYPE dmgType, ENUM_DMG_MOVE_TYPE moveType)
    {
        if (allBoards.Count + freeBoards.Count > MaxHitCount)
            return;

        BeHitBoard pBeHit = GetFreeBoard();
        if (pBeHit != null)
        {
            STRUCT_BEHIT_DATA PreDefine = DefineBehits[(int)moveType];
            pBeHit.m_bDoubleHit = bDouble;
            pBeHit.m_ColorType = PreDefine.m_ColorType;
            pBeHit.m_BackColorType = PreDefine.m_BackColorType;
            //STRUCT_BEHIT_DATA* pItem	= &PreDefine;
            pBeHit.m_nXSpeed = UnityEngine.Random.Range(-PreDefine.m_nXSpeed, PreDefine.m_nXSpeed);
            pBeHit.m_nYSpeed = PreDefine.m_nYSpeed + UnityEngine.Random.Range(3, 10); // Y象素级移动速度
            pBeHit.m_nXAcceleration = PreDefine.m_nXAcceleration;
            pBeHit.m_nYAcceleration = PreDefine.m_nYAcceleration; // 两个方向的加速度
            pBeHit.m_nMaxLife = PreDefine.m_nMaxLife;
            pBeHit.m_bAlphaMode = PreDefine.m_bAlphaMode;
            pBeHit.m_nMoveMode = PreDefine.m_nMoveMode;
            pBeHit.m_nUseTempPos = PreDefine.m_nUseTempPos;
            pBeHit.m_nStartX = x + PreDefine.m_fXPos;
            pBeHit.m_nStartY = y + PreDefine.m_fYPos;
            pBeHit.m_nMoveType = moveType;

            if (moveType == ENUM_DMG_MOVE_TYPE.MOVE_SCENE_NAME && dmgType == ENUM_DAMAGE_TYPE.DAMAGE_INVALID)
                pBeHit.ResetData(dmgText);
            else
                pBeHit.ResetData(moveType,dmgType, dmgText);

            pBeHit.Update();

            allBoards.Add(pBeHit);

            pBeHit.InUse = true;
        }
    }

    public void Update()
    {
        for (int i = 0; i < allBoards.Count; i++)
        {
            BeHitBoard curr = allBoards[i];
            curr.Update();

            if (curr.m_nLife < 0)
                FreeBoard(curr);
        }
    }

    STRUCT_BEHIT_DATA[] DefineBehits = new STRUCT_BEHIT_DATA[7];
    void InitAllMoveData()
    {
        //初始化对应ENUM_DMG_MOVE_TYPE
        STRUCT_BEHIT_DATA temp = DefineBehits[(int)ENUM_DMG_MOVE_TYPE.MOVE_STATUS] = new STRUCT_BEHIT_DATA();
        //状态的移动参数
        temp.m_nMaxLife = 3000;
        temp.m_nYSpeed = 100;
        temp.m_ColorType = new Color(1, 1, 1, 1);

        //别人的伤害
        temp = DefineBehits[(int)ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_OTHER] = new STRUCT_BEHIT_DATA();
        temp.m_nXSpeed = 80;
        temp.m_nYSpeed = 80;
        temp.m_nMaxLife = 3000;
        temp.m_ColorType = new Color(1, 1, 1, 1);
        //自己的伤害
        temp = DefineBehits[(int)ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_ME] = new STRUCT_BEHIT_DATA();
        //temp.m_nXSpeed = 30;
        temp.m_nYSpeed = 60;
        temp.m_nMaxLife = 3000;
        temp.m_nYAcceleration = 30;
        temp.m_ColorType = new Color(1, 0, 0, 1);
        //治疗HP
        temp = DefineBehits[(int)ENUM_DMG_MOVE_TYPE.MOVE_HEAL_HP] = new STRUCT_BEHIT_DATA();
        temp.m_nXSpeed = 40;
        temp.m_fYPos = 40;
        temp.m_nMaxLife = 3000;
        temp.m_ColorType = new Color(1, 1, 0, 1);
        //治疗MP
        temp = DefineBehits[(int)ENUM_DMG_MOVE_TYPE.MOVE_HEAL_MP] = new STRUCT_BEHIT_DATA();
        temp.m_nXSpeed = 40;
        temp.m_nMaxLife = 3000;
        temp.m_ColorType = new Color(1, 1, 0, 1);
        //显示场景名称
        temp = DefineBehits[(int)ENUM_DMG_MOVE_TYPE.MOVE_SCENE_NAME] = new STRUCT_BEHIT_DATA();
        temp.m_nMaxLife = 3000;
        temp.m_nYSpeed = 30;
        temp.m_ColorType = new Color(1, 1, 0, 1);
    }
}

public class STRUCT_BEHIT_DATA
{
    public float m_fXPos;
    public float m_fYPos;
    public float m_nXSpeed; // X象素级移动速度
    public float m_nYSpeed; // Y象素级移动速度

    public float m_nXAcceleration;
    public float m_nYAcceleration; // 两个方向的加速度

    public int m_nMaxLife;

    public bool m_bAlphaMode;	// 是否是alpha模式
    public int m_nMoveMode;
    public bool m_nUseTempPos;

    public Color m_ColorType;
    public Color m_BackColorType;

    public float m_fWidth;
    public float m_fHeight;
}

public class BeHitBoard
{
    // 底层用creatureboard控制显示 [2/7/2012 Ivan]
    CreatureBoard uiBoard;

    public BeHitBoard(int id)
    {
        uiBoard = new CreatureBoard(id);
        Reset();
    }

    public void Update()
    {
        m_nXSpeed += (m_nXAcceleration / 1000) * GameProcedure.s_pTimeSystem.GetDeltaTime();
        m_nYSpeed += (m_nYAcceleration / 1000) * GameProcedure.s_pTimeSystem.GetDeltaTime();

        m_nStartX += (m_nXSpeed / 1000) * GameProcedure.s_pTimeSystem.GetDeltaTime();
        m_nStartY += (m_nYSpeed / 1000) * GameProcedure.s_pTimeSystem.GetDeltaTime();

        m_nLife -= (int)GameProcedure.s_pTimeSystem.GetDeltaTime();

        if (m_bAlphaMode)
            m_fAlpha = (float)m_nLife / (float)m_nMaxLife;
        if (m_bDoubleHit)
        {
            // 更新缩放算法 [1/10/2011 ivan edit]
            float deltaScale = (GameProcedure.s_pTimeSystem.GetDeltaTime() / m_fDoubleTime) *
                (BeHitBoardManager.Instance.DoubleSize - BeHitBoardManager.Instance.DoubleEndSize);
            if (m_bZoomOn)
            {
                //m_fScale += m_fDoubleTime / g_pTimer.GetDeltaTime();
                m_fScale += deltaScale;

                if (m_fScale > BeHitBoardManager.Instance.DoubleSize)
                    m_bZoomOn = false;
            }
            else
            {
                //m_fScale -= m_fDoubleTime / g_pTimer.GetDeltaTime();
                m_fScale -= deltaScale;
            }

            if (m_fScale < BeHitBoardManager.Instance.DoubleEndSize)
            {
                m_fScale = BeHitBoardManager.Instance.DoubleEndSize;
                m_bDoubleHit = false;
            }
            uiBoard.SetScale(m_fScale);
        }
        if (m_nLife > 0)
        {
            Vector2 fPos = new Vector2(m_nStartX, m_nStartY);
            if (m_bAlphaMode)
                uiBoard.SetAlpha(m_fAlpha);
            uiBoard.SetPosition(new Vector2(m_nStartX, m_nStartY));
        }
    }

    void Reset()
    {
        m_nLife = 0;
        m_fAlpha = 1.0f;
        m_fDoubleTime = 1000;
        m_bInUse = false;
        m_nTempPosIndex = 0;
        m_bDoubleHit = false;

        if (m_bDoubleHit)
        {
            m_bZoomOn = true;
            m_fScale = BeHitBoardManager.Instance.DoubleEndSize;
        }
        else
        {
            m_bZoomOn = false;
            m_fScale = BeHitBoardManager.Instance.NormalSize;
        }

        m_nLife = m_nMaxLife;
        m_fDoubleTime = BeHitBoardManager.Instance.DoubleTime;

        uiBoard.SetScale(m_fScale);
        uiBoard.SetTextColor(m_ColorType);
        uiBoard.Reset();
    }

    public void ResetData(string szText)
    {
        Reset();

        uiBoard.SetElement_Name(szText);
        uiBoard.Show(true);
    }

    UnityEngine.GameObject winGo;

    string m_szWindowName;

    public bool m_bDoubleHit;

    public float m_nStartX;
    public float m_nStartY;

    public float m_fAlpha;
    public int m_nLife;			// 生存期
    public Color m_ColorType;
    public Color m_BackColorType;

    public float m_nXSpeed; // X象素级移动速度
    public float m_nYSpeed; // Y象素级移动速度
    public float m_nXAcceleration;
    public float m_nYAcceleration; // 两个方向的加速度
    public int m_nMaxLife;
    public bool m_bAlphaMode;	// 是否是alpha模式

    public float m_fScale;
    public STRUCT_BEHIT_DATA m_sData;

    public float m_fDoubleTime;
    public bool m_bZoomOn;
    public int m_nMoveMode;
    public bool m_nUseTempPos;
    public int m_nTempPosIndex;
    public ENUM_DMG_MOVE_TYPE m_nMoveType;

    bool m_bInUse; // 是否正在使用
    public bool InUse
    {
        get { return m_bInUse; }
        set
        {
            m_bInUse = value;
            uiBoard.Show(m_bInUse);
        }
    }

    /// <summary>
    /// 支持使用图片显示战斗信息
    /// </summary>
    /// <param name="moveType"></param>
    /// <param name="dmgType"></param>
    /// <param name="dmgText"></param>
    internal void ResetData(ENUM_DMG_MOVE_TYPE moveType, ENUM_DAMAGE_TYPE dmgType, string dmgText)
    {
        Reset();

        uiBoard.SetFightInfo(moveType,dmgType,dmgText);
        uiBoard.Show(true);
    }
}