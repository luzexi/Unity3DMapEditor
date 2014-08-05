using Network;
using Network.Packets;
using System;
using UnityEngine;
using Interface;
public class CActionItem_Talisman : CActionItem
{
    //得到操作类型
    public override ACTION_OPTYPE GetType() { return ACTION_OPTYPE.AOT_TALISMAN; }
    //类型字符串
    public override ActionNameType GetType_String() 
    {
        ActionNameType typeName = ActionNameType.UnName;
        if (ItemImpl != null)
        {
            switch (ItemImpl.TypeOwner)
            {
                case ITEM_OWNER.IO_TALISMAN_PACKET:
                    typeName = ActionNameType.talismanItem;
                    break;
                case ITEM_OWNER.IO_TALISMAN_EQUIPT:
                    typeName = ActionNameType.talismanEquip;
                    break;
            }
        }
        return typeName; 
    }
    //得到定义ID
    public override int GetDefineID()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null) return -1;
        return item.Define.nTableIndex;
    }

    public CTalisman_Item ItemImpl { get; set; }
    //得到数量
    public override int GetNum() { return 1; }
    //得到内部数据
    public override object GetImpl() { return (object)ItemImpl; }
    //得到解释
    public override string GetDesc()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null) return "ERROR";

        return item.GetDesc();
    }
    //得到冷却状组ID
    public override int GetCoolDownID() { return -1; }
    //得到所在容器的索引
    //	技能			- 第几个技能
    public override int GetPosIndex()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null) return -1;

        return item.GetPosIndex();
    }
    //是否能够自动继续进行
    public virtual bool AutoKeepOn() { return false; }
    //激活动作
    public override void DoAction() 
    {
        base.DoAction();

        if (ItemImpl == null)
            return;
        switch (ItemImpl.TypeOwner)
        {
            case ITEM_OWNER.IO_TALISMAN_PACKET:
                {
                    CGOperateTalisman operateTalisman = new CGOperateTalisman();
                    int posIndex = ItemImpl.GetPosIndex();
                    int equiptIndex = CDataPool.Instance.FindTalismanEquiptEmptyPlace();
                    if (equiptIndex == -1)
                    {
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "装备栏没有空位");
                        return;
                    }
                    operateTalisman.Type = 0;
                    operateTalisman.SrcIndex = (byte)(posIndex + GAMEDEFINE.MAX_BAG_SIZE);
                    operateTalisman.DstIndex = (byte)(equiptIndex);
                    NetManager.GetNetManager().SendPacket(operateTalisman);
                }
                break;
            case ITEM_OWNER.IO_TALISMAN_EQUIPT:
                {
                    CGOperateTalisman operateTalisman = new CGOperateTalisman();
                    int equiptIndex = ItemImpl.GetPosIndex();
                    int posIndex = CDataPool.Instance.FindTalismanInventoryEmptyPlace();
                    if (posIndex == -1)
                    {
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "法宝栏没有空位");
                        return;
                    }
                    operateTalisman.Type = 1;
                    operateTalisman.SrcIndex = (byte)(posIndex + GAMEDEFINE.MAX_BAG_SIZE);
                    operateTalisman.DstIndex = (byte)(equiptIndex);
                    NetManager.GetNetManager().SendPacket(operateTalisman);
                }
                break;
        }
    }
    //刷新
    public virtual void Update(tActionReference pRef) { }
    //检查冷却是否结束
    public override bool CoolDownIsOver() { return true; }
    //拖动结束
    public override void NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName)
    {
        Debug.Log("src " + szSourceName + " dst " + szTargetName);
        if (bDestory)
        {
            DestoryItem(szSourceName);
            return;
        }
        //拖动到空白地方
        if (szTargetName == "")
        {
            return;
        }

        char cSourceName = szSourceName[0];
        char cTargetType = szTargetName[0];
        //如果是法宝装备栏 就不需要拖
        //T 法宝物品栏 E 法宝装备栏
        if (cSourceName == 'A')
        {
            if (cTargetType == 'B')
            {
                NotifyDragEquipToPackage(szSourceName, szTargetName);
            }
        }
        else if (cSourceName == 'B')
        {
            if (cTargetType == 'B')
            {
                NotifyDragPackageToPackage(szSourceName, szTargetName);
            }
        }
    }

    void DestoryItem(string szSourceName)
    {

    }

    void NotifyDragEquipToPackage(string szSourceName, string szTargetName)
    {
        char equiptIndex  = szSourceName[1];
        char packageIndex = szTargetName[1];
        Debug.Log("src " + szSourceName + " dst " + szTargetName);

    }

    void NotifyDragPackageToEquip(string szSourceName, string szTargetName)
    {
        char packageIndex = szSourceName[1];
        char equiptIndex  = szTargetName[1];
        Debug.Log("src " + szSourceName + " dst " + szTargetName);
    }

    void NotifyDragPackageToPackage(string szSourceName, string szTargetName)
    {		
		int srcPackageIndex = Convert.ToInt32(szSourceName.Substring(1, szSourceName.Length-1)) -1;
        int desPackageIndex = Convert.ToInt32(szTargetName.Substring(1, szTargetName.Length-1)) -1;
        Debug.Log("src " + szSourceName + " dst " + szTargetName);
        CTalisman_Item item = ItemImpl;
        if (item != null)
        {
            switch (item.TypeOwner)
            {
                //Package -> Package
                case ITEM_OWNER.IO_TALISMAN_PACKET:
                    {
                        //先不做能不能放入的检测
						int srcindex = (int)srcPackageIndex + GAMEDEFINE.MAX_BAG_SIZE;
                    	int dstindex = (int)desPackageIndex + GAMEDEFINE.MAX_BAG_SIZE;    
					
                        
                        if (srcindex == dstindex)
                            return;
                      
                        CTalisman_Item desItem = CDataPool.Instance.TalismanInventory_GetItem(srcPackageIndex);
                        CTalisman_Item srcItem = CDataPool.Instance.TalismanInventory_GetItem(desPackageIndex);
                        if (desItem != null && srcItem != null)
                        {
                            CGOperateTalisman operateTalisman = new CGOperateTalisman();
                            operateTalisman.Type = 2;
                            operateTalisman.SrcIndex = (byte)srcindex;
                            operateTalisman.DstIndex = (byte)dstindex;
                            NetManager.GetNetManager().SendPacket(operateTalisman);
                        }
                        else
                        {
                            //不同格
                            CGPackage_SwapItem msg = new CGPackage_SwapItem();
                            msg.PIndex1 = (byte)srcindex;
                            msg.PIndex2 = (byte)dstindex;
                            NetManager.GetNetManager().SendPacket(msg);
                        }
                    }
                    break;
            }
        }

    }


    // 得到等级
    public virtual int GetLevel()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null)
        {
            return 0;
        };
        return item.GetLV();
    }

    public CActionItem_Talisman(int nID)
        : base(nID)
    {
    }
	
	public uint GetCurExp()
	{
		CTalisman_Item item = ItemImpl;
        if (item == null)
        {
            return 0;
        };
        return item.Exp;
	}
    public uint GetNextLVExp()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null)
        {
            return 0;
        };
        return item.NextExp;
    }
    public int GetSellPrice()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null)
        {
            return 0;
        };
        return item.GetItemPrice();
    }

    public string GetFontColor()
    {
        CTalisman_Item item = ItemImpl;
        if (item == null)
        {
            return "";
        };
        return item.getFontColor();
    }

    //更新数据
    public void Update_Item(CTalisman_Item item)
    {
        m_id = item.GetID();
        //名称
        m_strName = item.Define.szName;
        //图标
        m_strIconName = item.Define.szIcon;
        ItemImpl = item;
        //通知UI
        UpdateToRefrence();
    }

    //法宝ID
    protected int m_id;

};