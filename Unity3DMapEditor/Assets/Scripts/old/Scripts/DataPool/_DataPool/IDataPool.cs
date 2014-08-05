//////////////////////////////////////////////////////////////////////////
// name: IDataPool
// desc: 原DataPool的接口定义
// auth: sun
// date: 2012-2-2
//////////////////////////////////////////////////////////////////////////
public interface IPlayerEquip
{
    //清空
    void Clear();
    //设置装备
    void SetItem(short ptEquip, CObject_Item item, bool bClearOld);
    //获取装备
    CObject_Item GetItem(int ptEquip);
    //套装是否组合成功，返回套装编号，否则返回-1
    int IsUnion();
}
