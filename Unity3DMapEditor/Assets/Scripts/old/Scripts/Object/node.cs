using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CNode
{

	//返回子节点数目
	public virtual int							GetChildNum() 	{ return mChildren.Count; }

	//返回父节点
    public virtual CNode GetParent() { return mParent; }
    public  void SetParent(CNode parent) {  mParent = parent; }
	//节点初始化
	public virtual void						Initial(object pInit)			{ }
    public virtual void Release()
    {
        foreach (CObject obj in mChildren)
        {
            obj.Release(); 
        }
        EraseAllChild();
    }
	//逻辑轮循函数
	public virtual void						Tick()
	{
        // Why Copy?? [5/3/2012 SUN]
        List<CNode> childrenCopy = new List<CNode>();
        childrenCopy.AddRange(mChildren);
        foreach (CObject obj in childrenCopy)
		{
            obj.Tick();
		}
	}

	//添加子节点到该节点上
	public virtual void AddChild(CObject newObject)
    {
        if (newObject==null)
        {
            return;
        }
        newObject.SetParent(this);
        mChildren.Add(newObject);
    }

	//删除某个子节点,当该节点存在时返回TRUE,否则返回FALSE
	public virtual bool EraseChild(CObject newObject)
    {
       return mChildren.Remove(newObject);
    }

	//删除所有子节点
	public virtual void EraseAllChild( )
    {
        mChildren.Clear();
    }

    //层次
    protected List<CNode> mChildren = new List<CNode>();
    protected CNode mParent;
}	


