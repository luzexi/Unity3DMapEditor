using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SceneFindPathStatus
{
	List<SceneTransferData>	vFindResult;
	int nCurrentPos;
	Vector2 vDestPos;
	int	nDestScene;
	bool bUseFlag;

    public HyperItemBase Hyper { get; set; }

	public void SetValue(int DestScene, Vector2 DestPos, List<SceneTransferData> findResult)
    {
        Clear();

		nCurrentPos = 0;
		nDestScene = DestScene;
		vDestPos = DestPos;
		vFindResult = findResult;
		bUseFlag = true;
	}

	public SceneFindPathStatus()
	{
		Clear();
	}
    public Vector2 GetNextPosition()
	{
        if (vFindResult != null)
        {
            if (nCurrentPos < vFindResult.Count)
            {
                Vector2 ret = new Vector2((float)vFindResult[nCurrentPos].xPos, (float)vFindResult[nCurrentPos].yPos);
                ++nCurrentPos;
                return ret;
            }
            if (bUseFlag)
            {
                bUseFlag = false;
                return vDestPos;
            }
        }
		return new Vector2(-1,-1);
	}
    public bool IsUse() { return bUseFlag; }

    public void Clear()
	{
		nCurrentPos = 0;
		nDestScene = -1;
		bUseFlag = false;
        if (vFindResult != null)
		    vFindResult.Clear();

        Hyper = null;
	}
}

//场景传送点结构
public class SceneTransferData
{
    public int nSceneID;
    public float xPos;
    public float yPos;
    public int nDestSceneID;
}

//每个场景的路信息,场景传送点的集合
public class SceneWayInfo 
{
    //定义每个场景最多的通路个数
    public const int MAX_WAY_ONE_SCENE = 8;

    public int[] nWayID = new int[SceneWayInfo.MAX_WAY_ONE_SCENE];
    public bool bValidFlag;

	public SceneWayInfo()
	{
        for (int i = 0; i < SceneWayInfo.MAX_WAY_ONE_SCENE; i++)
            nWayID[i] = -1;
		bValidFlag = false;
	}

	public bool Add(int id)
	{
		for (int i=0; i<MAX_WAY_ONE_SCENE; i++)
		{
			if (nWayID[i] == -1)
			{
				nWayID[i] = id;
				return true;
			}
		}
		return false;
	}
	public bool IsValid()
	{
		return bValidFlag;
	}
	public void	SetValid(bool setting)
	{
		bValidFlag = setting;
	}
}

public struct SearchNodeData
{
    public int Weight { get; set; }
    public int Pos { get; set; }
}

public class SearchPathNode
{
    //一条搜索路径的最大结点长度
    public const int MAX_PATH_ID_ARRAY = 256;

    public int[] nPathIDArray = new int[SearchPathNode.MAX_PATH_ID_ARRAY];
    public int nWeight;
    int nCurrentPos;

    public void CopyOf(SearchPathNode old)
    {
        Array.Copy(old.nPathIDArray, this.nPathIDArray, old.nPathIDArray.Length);
        this.nWeight = old.nWeight;
        this.nCurrentPos = old.nCurrentPos;
    }

    public SearchPathNode()
    {
        Clear();
    }
    public void Clear()
    {
        for (int i = 0; i < SearchPathNode.MAX_PATH_ID_ARRAY; i++)
            nPathIDArray[i] = -1;
        nWeight = int.MaxValue;
        nCurrentPos = 0;
    }
    public bool IsInUse()
    {
        return nWeight != int.MaxValue;
    }
    public void MarkUnUse()
    {
        nWeight = int.MaxValue;
    }
    public void MarkUse()
    {
        nWeight = 0;
        nCurrentPos = 0;
    }
    public void AddWeight(int w)
    {
        nWeight += w;
    }
    public void AddWeight()
    {
        nWeight += 1;
    }
    public bool AddPathID(int nID)
    {
        for (int i = 0; i < MAX_PATH_ID_ARRAY; i++)
        {
            //出现循环错误
            if (nPathIDArray[i] == nID)
            {
                return false;
            }
            //找到空白的,然后添加进去
            if (nPathIDArray[i] == -1)
            {
                nPathIDArray[i] = nID;
                ++nCurrentPos;
                return true;
            }
        }
        return false;
    }
    public void RemoveLastAddID()
    {
        --nCurrentPos;
        if (nCurrentPos >= 0)
        {
            nPathIDArray[nCurrentPos] = -1;
        }
    }
    public int GetLastID()
    {
        if (nCurrentPos < 1 || nCurrentPos > MAX_PATH_ID_ARRAY)
        {
            return -1;
        }
        return nPathIDArray[nCurrentPos-1];
    }
}

public class SceneTransportPath
{
    static readonly SceneTransportPath sInstance = new SceneTransportPath();
    static public SceneTransportPath Instance { get { return sInstance; } }

    public SceneTransportPath()
    {
        m_pTransferData = null;
        m_pSceneWayInfo = null;
	    m_nMaxScene = 0;
	    m_nMaxData =0;
	    m_nCurrentSearchPos = 0;
    }

	//接口函数
	public void	Initailize(int maxData, int maxScene)
    {
	    if (maxData>0 && maxScene > 0)
	    {
		    m_pTransferData = new SceneTransferData[maxData];
		    m_pSceneWayInfo = new SceneWayInfo[maxScene];
		    m_nMaxScene = maxScene;
		    m_nMaxData = maxData;
	    }
	    else
	    {
            LogManager.LogError("ShortestPath::Initailize Invalid Params");
	    }
    }

    static int nPos = 0;
	public void	AddSceneTransData( SceneTransferData data)
    {
	    if( nPos < m_nMaxData)
	    {
		    m_pTransferData[nPos++] = data;
		    if(data.nSceneID < m_nMaxScene)
		    {
                if (m_pSceneWayInfo[data.nSceneID] == null)
                    m_pSceneWayInfo[data.nSceneID] = new SceneWayInfo();

			    if( !m_pSceneWayInfo[data.nSceneID].Add(nPos-1) )
			    {
                    LogManager.LogError("ShortestPath::AddSceneTransData m_pSceneWayInfo[data.nSceneID].Add return false");
			    }
			    m_pSceneWayInfo[data.nSceneID].SetValid(true);
		    }
		    else
		    {
                LogManager.LogError("ShortestPath::AddSceneTransData ");
		    }
	    }
	    else
	    {
            LogManager.LogError("ShortestPath::AddSceneTransData out bound of m_pTransferData");
	    }
    }

	public bool	FindPath(int nDestScene,  int nSrcScene, out List<SceneTransferData> rOutPath)
    {
        rOutPath = new List<SceneTransferData>();
        //不是有效的场景,那么返回失败
	    if ( !IsValidSceneID(nDestScene) || !IsValidSceneID(nSrcScene))
	    {
		    return false;
	    }

	    //  [8/3/2011 ivan edit]
	    ClearPathData();

	    //添加原场景到OpenList里
	    AddToOpenList(nSrcScene);
	    //添加
	    AddFirstSearchPathNode(nSrcScene);

	    //循环查找,直到找到或遍历完了为止
	    bool bFind = false;
	    int nShortesID = -1;
	    while( !IsOpenListEmpty() && !bFind && m_aSearchPathD.Count > 0)
	    {
		    //找到代价最小的
            SearchNodeData node = GetSWeightNode();
            nShortesID = node.Pos;
		    //然后删除之
            m_aSearchPathD.Remove(node);

		    //检查是否是终点
		    if(  IsEnd(nShortesID, nDestScene) )
		    {
			    bFind = true;
			    break;
		    }

		    //添加最佳路线进入
		    int nID = GetSceneWayInfoIDFromSearchPathNodeID(nShortesID);
		    if( nID < 0 ) break;
		    AddSearchPathNode(nShortesID, nID);
	    }
	    if (bFind)
	    {
		    if(nShortesID >=0 && nShortesID < m_nCurrentSearchPos )
		    {
			    rOutPath.Clear();
                for (int i = 0; i < SearchPathNode.MAX_PATH_ID_ARRAY; i++)
			    {
				    int nID = m_aSearchPathNode[nShortesID].nPathIDArray[i];
				    if (nID<0)
				    {
					    break;
				    }
				    //添加路径
				    rOutPath.Add(m_pTransferData[nID]);
			    }
		    }
	    }
	    return bFind;
    }

	public bool	ClearPathData()
    {
	    m_aOpenList.Clear();
	    m_aCloseList.Clear();

	    return true;
    }

	// 删除原始数据 [8/3/2011 ivan edit]
    //public bool ClearOrgData();

	//算法函数
    //把某个场景的所有的路径加入到列表里面
	protected int	AddSearchPathNode(int nSearchPathNodeID, int nSceneWayInfoID)
    {
	    //在已经使用了的范围内
	    if(nSearchPathNodeID >=0 && nSearchPathNodeID < m_nCurrentSearchPos && IsValidSceneID(nSceneWayInfoID))
	    {
		    //拷贝一份
            SearchPathNode node = new SearchPathNode();
            node.CopyOf(m_aSearchPathNode[nSearchPathNodeID]);
		    //删除并加入到Close里面
		    RemoveFromOpenList(nSceneWayInfoID);
		    AddToCloseList(nSceneWayInfoID);
		    //八个方向
		    for (int i=0; i<SceneWayInfo.MAX_WAY_ONE_SCENE; i++)
		    {
			    if( m_pSceneWayInfo[nSceneWayInfoID].nWayID[i] != -1 )
			    {
				    node.AddPathID(m_pSceneWayInfo[nSceneWayInfoID].nWayID[i] );
				    node.AddWeight();
				    if (m_nCurrentSearchPos < MAX_PATH_NODE)
                    {
                        SearchPathNode newNode = new SearchPathNode();
                        newNode.CopyOf(node);
                        m_aSearchPathNode[m_nCurrentSearchPos] = newNode;
				    }
				    //加入到OpenList里
				    int wayID = GetSceneWayInfoIDFromSceneTransferDataID(m_pSceneWayInfo[nSceneWayInfoID].nWayID[i]);
				    AddToOpenList(wayID);
				    //添加到权重列表里,目的是为了排序,找到权重最小的就是第一个
				    AddSearchNode(node.nWeight, m_nCurrentSearchPos++);
				    //删除刚才添加的,然后继续使用
                    node.RemoveLastAddID();
                    node.AddWeight(-1);
			    }
			    else
			    {
				    return i;
			    }
		    }
	    }
	    return 0;
    }
	protected void	AddFirstSearchPathNode(int nSceneWayInfoID)
    {
	    m_nCurrentSearchPos = 0;
        if(m_aSearchPathNode[m_nCurrentSearchPos] == null)
            m_aSearchPathNode[m_nCurrentSearchPos] = new SearchPathNode();
        else
	        m_aSearchPathNode[m_nCurrentSearchPos].Clear();
	    m_aSearchPathD.Clear();
	    m_aSearchPathNode[m_nCurrentSearchPos].MarkUse();
	    //m_aSearchPathD.insert(std::make_pair(m_aSearchPathNode[m_nCurrentSearchPos].nWeight, m_nCurrentSearchPos));
	    ++m_nCurrentSearchPos;
	    AddSearchPathNode(0,nSceneWayInfoID);
    }

	protected void	AddToOpenList(int nSceneWayInfoID)
    {
	    //不能重复添加
	    if( m_aOpenList.Contains(nSceneWayInfoID) )
	    {
		    return;
	    }
	    m_aOpenList.Add(nSceneWayInfoID);
    }
	protected void	RemoveFromOpenList(int nSceneWayInfoID)
    {
        m_aOpenList.Remove(nSceneWayInfoID);
    }

    protected bool IsInOpenList(int nSceneWayInfoID)
    {
        return m_aOpenList.Contains(nSceneWayInfoID);
    }

	protected bool	IsOpenListEmpty() { return m_aOpenList.Count == 0; }
	protected void	AddToCloseList(int nSceneWayInfoID)
    {
	    //不能重复添加
        if (m_aCloseList.Contains(nSceneWayInfoID))
            return;
	    m_aCloseList.Add(nSceneWayInfoID);
    }
    protected bool IsInCloseList(int nSceneWayInfoID)
    {
        return m_aCloseList.Contains(nSceneWayInfoID);
    }
	protected bool	IsCloseListEmpty() { return m_aCloseList.Count > 0; }
    protected int GetSceneWayInfoIDFromSearchPathNodeID(int nID)
    {
        if (nID >= 0 && nID < SearchPathNode.MAX_PATH_ID_ARRAY)
        {
            int nTransID = m_aSearchPathNode[nID].GetLastID();
            if (nTransID > -1 && nTransID < m_nMaxData)
            {
                return m_pTransferData[nTransID].nDestSceneID;
            }
        }
        return -1;
    }
    protected int GetSceneWayInfoIDFromSceneTransferDataID(int nID)
    {
        //m_pSceneWayInfo以SceneID为下标,所以可以这样使用
        if (nID >= 0 && nID < m_nMaxData)
        {
            return m_pTransferData[nID].nDestSceneID;
        }
        return -1;
    }
	protected bool	IsValidSceneID(int nSceneID)
    {
	    if (nSceneID >-1 && nSceneID <m_nMaxScene)
	    {
		    return true;
	    }
	    return false;
    }
    protected bool IsEnd(int nShortesID, int nDestScene)
    {
        int nLastID = m_aSearchPathNode[nShortesID].GetLastID();
        if (nLastID < 0)
        {
            return false;
        }
        return m_pTransferData[nLastID].nDestSceneID == nDestScene;
    }

	//定义基本信息数据
	private SceneTransferData[]	m_pTransferData;
	private SceneWayInfo[]			m_pSceneWayInfo;
	private int m_nMaxScene, m_nMaxData;
	//定义算法数据
    //系统里每次检索最多有多少的条通路
    const int MAX_PATH_NODE	=1024;
	private SearchPathNode[]		m_aSearchPathNode = new SearchPathNode[MAX_PATH_NODE];
    //first代表权重,越小,表示是最短路径,second代表SearchPathNode的索引,是一个通路的索引
    List<SearchNodeData> m_aSearchPathD = new List<SearchNodeData>();
    void AddSearchNode(int weight, int pos)
    {
        SearchNodeData node = new SearchNodeData();
        node.Weight = weight;
        node.Pos = pos;
        m_aSearchPathD.Add(node);
    }

    SearchNodeData GetSWeightNode()
    {
        SearchNodeData node = new SearchNodeData();
        if (m_aSearchPathD.Count == 0)
            return node;
        else
            node = m_aSearchPathD[0];
        for (int i = 1; i < m_aSearchPathD.Count; i++)
        {
            if (m_aSearchPathD[i].Weight < node.Weight)
            {
                node = m_aSearchPathD[i];
            }
        }
        return node;
    }

    //状态集合
	private List<int> m_aOpenList = new List<int>();
	private List<int> m_aCloseList = new List<int>();
	private int m_nCurrentSearchPos;
}