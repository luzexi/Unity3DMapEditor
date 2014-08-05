using System;

using Network.Packets;
namespace Network.Handlers
{
    public class GCShopSoldListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCShopSoldList packet = pPacket as GCShopSoldList;

                int nNum = packet.MerchadiseNum;
                if (nNum == 0)
                {//空的没东西
                    if (CDataPool.Instance.Booth_GetSoldNumber() != 0)
                    {
                        CDataPool.Instance.Booth_Sold_Clear();
                    }
                }
                else
                {
                    //清空原有商品列表
                    CDataPool.Instance.Booth_Sold_Clear();

                    //CSoundSystemFMod::_PlayUISoundFunc(25+59);

                    //添加到数据池中
                    for (short i = 0; i < nNum; i++)
                    {
                        GCShopSoldList._MERCHANDISE_ITEM pNewItem = packet.MerchadiseList[i];
                        CObject_Item pItem = null;

                        pItem = ObjectSystem.Instance.NewItem(pNewItem.item_data.m_ItemIndex);
                        pItem.SetGUID(
                            (ushort)pNewItem.item_data.m_ItemGUID.m_World,
                            (ushort)pNewItem.item_data.m_ItemGUID.m_Server,
                            (uint)pNewItem.item_data.m_ItemGUID.m_Serial);

                        pItem.SetExtraInfo(ref pNewItem.item_data);


                        //设置该物品在货架上的位置因为是回购物品它的位置应该是200
                        pItem.PosIndex = i;
                        //AxTrace(0,0,"pItem->SetPosIndex ＝ %d",i);
                        //设置该物品的数量（每一组的数量）
                        pItem.SetNumber(pNewItem.item_data.GetItemCount());
                        //回购物品当然只有一个
                        pItem.SetMax(1);
                        //加入数据池
                        CDataPool.Instance.Booth_SetSoldItem(i, pItem);
                        CDataPool.Instance.Booth_SetSoldPrice(i, pNewItem.iPrice);
                    }
                    //设置回购商品的数量
                    CDataPool.Instance.Booth_SetSoldNumber(nNum);
                }

                //更新到ActionSystem
                CActionSystem.Instance.Booth_Update();
                //通知UI
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BOOTH);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SHOPSOLDLIST;
        }
    }
};
