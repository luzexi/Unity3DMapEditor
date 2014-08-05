using System;
using System.Collections;
using System.Collections.Generic;
using Network.Packets;
using UnityEngine;

//物体加载队列，防止一桢内同时加载过多物体
public class CObjectLoadQueue
{
    //继承接口IComparer,支持重复键
    public class SortDistance : IComparer<float>
    {

        #region IComparer<float> Members

        int IComparer<float>.Compare(float x, float y)
        {
            //排序
            float compRes = x - y;
            if (compRes == 0)
            {
                return -1;
            }
            else if (compRes > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        #endregion
    }

    //按照距离远近的工作队列
    SortedList<float, int> m_mapLoadTask = new SortedList<float, int>(new SortDistance());
	bool    m_LoadDirect;

    public CObjectLoadQueue()
    {
        m_LoadDirect = false;
    }
    
	//将一个需要Ask的obj放入
	public bool TryAddLoadTask(int idObj)
    {
	    //玩家自己不放入队列
	    CObject pObj = CObjectManager.Instance.FindObject(idObj);
	    if(pObj==null || pObj==CObjectManager.Instance.getPlayerMySelf()) 
            return false;

	    if( m_LoadDirect )
	    { 
            //立即请求
	        CGCharAskBaseAttrib msgAskBaseAttrib = new CGCharAskBaseAttrib();
	        msgAskBaseAttrib.setTargetID( (uint)pObj.ServerID);
	        GameProcedure.s_NetManager.SendPacket(msgAskBaseAttrib );
            //LogManager.LogWarning("Ask Char BaseAttrib id=" + pObj.ServerID);
	    }
	    else
	    {
		    //自己位置
		    Vector3 fvPosMySelf = CObjectManager.Instance.getPlayerMySelf().GetPosition();
		    float fDistanceSq = Utility.TDU_GetDistSq(pObj.GetPosition(), fvPosMySelf);

            if (m_mapLoadTask.ContainsValue(idObj))
                LogManager.LogWarning("该角色ID<" + idObj + ">已经在加载队列中");
            else
            {
                m_mapLoadTask.Add(fDistanceSq, idObj);

                //LogManager.Log("Ask Char BaseAttrib in queue id" + idObj);
            }
	    }

	    return true;
    }

    //每桢处理的个数
    const int WORK_SPEED = 1;
	//工作桢
	public void		Tick()
    {
	    if(m_mapLoadTask.Count != 0)
	    {
		    int nDoRealWork = 0;
		    do
		    {
			    //存在需要加载的OBJ
   			    int idObj = m_mapLoadTask.Values[0];

	            CObject pObj = CObjectManager.Instance.FindObject(idObj);
			    if(pObj != null)
			    {
                    //处理消息
                    CGCharAskBaseAttrib msgAskBaseAttrib = new CGCharAskBaseAttrib();
                    msgAskBaseAttrib.setTargetID((uint)pObj.ServerID);
                    GameProcedure.s_NetManager.SendPacket(msgAskBaseAttrib);
                    //LogManager.LogWarning("Ask Char BaseAttrib id=" + pObj.ServerID);
				    nDoRealWork++;
			    }
			    else
			    {
				    //该Object已经被删除
			    }

			    //从队列中删除
                m_mapLoadTask.RemoveAt(0);

			    //做过真实工作，退出
			    if(nDoRealWork>=WORK_SPEED || m_mapLoadTask.Count == 0) break;
		    }while(true);
	    }
    }

	//清空工作队列（切换场景时）
	public void		ClearAllTask()
    {
        m_mapLoadTask.Clear();
    }
	//是否立即加载NPC
	public void		SetLoadNPCDirect(bool isDirect)
    {
        m_LoadDirect = isDirect;
    }
}
