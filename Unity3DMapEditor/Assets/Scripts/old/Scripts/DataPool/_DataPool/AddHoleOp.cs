////////////////////////////////////////////////////////////////////////////
//打孔操作，源自：CDataPool
public class AddHoleOperation
{
    struct ADD_HOLE_EQUIP
    {
        //INT m_nContainer;    //装备或者背包
        //INT m_nPosition;	 //装备点	
        //INT m_nItemPos;		 //消耗道具所在位置
        //INT m_ItemId;        //道具id
        //INT m_ItemNum;		 //道具数量	
        //INT m_Money;        //花费金钱数目
        //INT m_NpcId;		//相关npc

        //VOID CleanUp()
        //{
        //    m_nContainer = -1;
        //    m_nPosition = -1;
        //    m_ItemId = -1;
        //    m_ItemNum = -1;
        //    m_Money = -1;
        //    m_NpcId = -1;
        //}

        //ADD_HOLE_EQUIP()
        //{
        //    CleanUp();
        //}
    };
    ADD_HOLE_EQUIP m_CurAddHoleEquip;

    ///** 当前正在打孔的物品 */
    //VOID AddHole_CleanUP() { m_CurAddHoleEquip.CleanUp(); }
    //INT AddHole_GetEquipConta() { return m_CurAddHoleEquip.m_nContainer; }
    //INT AddHole_GetEquipPos() { return m_CurAddHoleEquip.m_nPosition; }
    //INT AddHole_GetNeedItemPos() { return m_CurAddHoleEquip.m_nItemPos; }
    //INT AddHole_GetNeedItemId() { return m_CurAddHoleEquip.m_ItemId; }
    //INT AddHole_GetNeedItemNum() { return m_CurAddHoleEquip.m_ItemNum; }
    //INT AddHole_GetNeedMoney() { return m_CurAddHoleEquip.m_Money; }
    //INT AddHole_GetNpcId() { return m_CurAddHoleEquip.m_NpcId; }


    //VOID AddHole_SetEquipConta(INT nContainer) { m_CurAddHoleEquip.m_nContainer = nContainer; }
    //VOID AddHole_SetEquipPos(INT nPosition) { m_CurAddHoleEquip.m_nPosition = nPosition; }
    //VOID AddHole_SetNeedItemPos(INT nPosition) { m_CurAddHoleEquip.m_nItemPos = nPosition; }
    //VOID AddHole_SetNeedItemId(INT nNeedID) { m_CurAddHoleEquip.m_ItemId = nNeedID; }
    //VOID AddHole_SetNeedItemNum(INT nNeedNum) { m_CurAddHoleEquip.m_ItemNum = nNeedNum; }
    //VOID AddHole_SetNeedMoney(INT nNeedMoney) { m_CurAddHoleEquip.m_Money = nNeedMoney; }
    //VOID AddHole_SetNpcId(INT npc) { m_CurAddHoleEquip.m_NpcId = npc; }
}