
using System.Collections.Generic;
/// <summary>
/// 打开的箱子，原CDataPool中ItemBox接口
/// </summary>
public class ItemBox
{
    //箱子ID
    private int m_idItemBox;
    //箱子物品实例数组
    private List<CObject_Item> m_vItemBox;

    //清空
	public void			Clear(){}
	//设置箱子ID
    public void         SetID(int id) { m_idItemBox = id; }
	//取得箱子ID
    public int          GetID() { return m_idItemBox; }
	//设置其中物品
    public void SetItem(int nBoxIndex, CObject_Item pItem, bool bClearOld) { }
	//取得其中物品
    public CObject_Item GetItem(int nBoxIndex)
    {
        //Todo:
        return null;
    }
	//按照ID取得物品
    public CObject_Item GetItem(short idWorld, short idServer, int idSerial, ref int nIndex)
    {
        //Todo:
        return null;
    }
	//返回目前箱子中物品个数
    public int          GetNumber()
    {
        //Todo:
        return 0;
    }

	//获得一个物品在当前商店的实际收购价格
    public int          GetCurNpcBuyPrice(int nBagIndex)
    {
        //Todo:
        return 0;
    }
}