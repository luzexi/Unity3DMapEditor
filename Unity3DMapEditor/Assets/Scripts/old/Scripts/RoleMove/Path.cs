using System;
using System.Collections.Generic;
using UnityEngine;
using NavMesh;

public struct PathUnit
{
    public Vector2 fvStart;		//起始点
    public Vector2 fvTarget;	//目标点
    public int dwRequestIndex;	//发往服务器的路径编号，用于和服务器之间交流的标示
};

public class CPath
{
    private static readonly CPath instance = new CPath();
    public static CPath Instance
    {
        get { return instance; }
    }

    List<PathUnit> m_vPosStack = new List<PathUnit>();		//关键点的集合，每两个点之间是一条路径单位
    public List<PathUnit> PosStack
    {
        get { return m_vPosStack; }
    }
    Vector2 m_TargetPos;		        // 走路的终点

    // 建立导航网格路径
    int BuildNavMeshPath(Vector2 fvCurrent, Vector2 fvTarget, int nPassLevel)
    {
        // 先清空路径.
        m_vPosStack.Clear();
        List<Vector2> pathList = null;

        PathResCode pathResult = PathFinder.Instance.FindPath(fvCurrent, fvTarget, out pathList, 0);
        if (pathList.Count >= 2)
        {
            PathUnit pu = new PathUnit();
            pu.fvStart = pathList[0];
            for (int i = 1; i < pathList.Count; i++)
            {
                Vector2 pt = pathList[i];
                pu.fvTarget = pt;
                m_vPosStack.Add(pu);
                pu.fvStart = pu.fvTarget;
            }
        }
        return m_vPosStack.Count;
    }

    // 计算从开始点到结束点的路径。
    public bool CreateMovePath(Vector2 fvCurrent, Vector2 fvTarget)
    {
        //得到当前人物坐骑等级
        int nPassLevel = -1;

        // 找到目标点。
        m_TargetPos = fvTarget;

        int pathSize = BuildNavMeshPath(fvCurrent, fvTarget, nPassLevel);
        
        return pathSize > 0 ? true : false;
    }
    public static bool IsPointInUnreachRegion(float x, float z) // 当前点是否在不可行走区域之内
    {
        return false;//todo
    }
}
