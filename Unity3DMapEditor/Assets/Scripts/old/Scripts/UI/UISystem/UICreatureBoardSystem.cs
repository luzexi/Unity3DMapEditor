/***********************************\
*									*
*	 角色头顶上的信息UI管理器		*
*									*
\************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

public class UICreatureBoardSystem
{
    // 更改为单例模式
    static readonly UICreatureBoardSystem sInstance = new UICreatureBoardSystem();

    static public UICreatureBoardSystem Instance { get { return sInstance; } }

    private UICreatureBoardSystem()
    {
        m_nMaxDistance = 324;
        m_nMaxDispearDistance = 625;
    }

    public void Initial()
    {

    }

    public void Release()
    {
        m_listAllBoard.Clear();
        m_listBoardDataPool.Clear();
    }

    //信息板池
    List<CreatureBoard> m_listBoardDataPool = new List<CreatureBoard>();
    //所有信息板的链表
    List<CreatureBoard> m_listAllBoard = new List<CreatureBoard>();

    static int ID = 0;

    public CreatureBoard CreateCreatureBoard()
    {
        CreatureBoard pNewBoard = null;

        //创建新的
        if (m_listBoardDataPool.Count == 0)
        {
            pNewBoard = new CreatureBoard(ID++);
        }
        else
        {
            //尝试从池中取得
            pNewBoard = m_listBoardDataPool[0];
            //pNewBoard.Show(true);

            m_listBoardDataPool.RemoveAt(0);
        }

        pNewBoard.Reset();

        m_listAllBoard.Add(pNewBoard);

        //CUICreatureBoardSystem::GetClientScreen()->addChildWindow( pNewBoard->GetMainWindow() );
        pNewBoard.Show(false);

        return pNewBoard;
    }

    public void DestroyCreatureBoard(CreatureBoard board)
    {
        if (board == null)
            return;

        //CUICreatureBoardSystem::GetClientScreen()->removeChildWindow( pBoard->GetMainWindow() );

        //从链表中删除，加入可用堆栈
        if (m_listAllBoard.Remove(board))
        {
            m_listBoardDataPool.Add(board);
            board.Show(false);
        }
    }

    float m_nMaxDistance; // 最大的显示范围
    public float MaxDistance
    {
        get { return m_nMaxDistance; }
        set { m_nMaxDistance = value; }
    }
    float m_nMaxDispearDistance;// 超过找个范围就消失
    public float MaxDispearDistance
    {
        get { return m_nMaxDispearDistance; }
        set { m_nMaxDispearDistance = value; }
    }
}
