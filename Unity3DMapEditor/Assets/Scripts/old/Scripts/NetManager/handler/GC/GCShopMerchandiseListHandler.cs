using System;
using System.Collections.Generic;


using Network.Packets;

namespace Network.Handlers
{
    public class GCShopMerchandiseListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LogManager.LogWarning("RECV GCShopMerchandiseList");
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCShopMerchandiseList packet = pPacket as GCShopMerchandiseList;
                //清空原有商品列表
                CDataPool.Instance.Booth_Clear();

                //清空原有商品列表
                CDataPool.Instance.Booth_Sold_Clear();

                //商品个数
                int nNum = packet.MerchadiseNum;

                //添加到数据池中
                for (short i = 0; i < nNum; i++)
                {
                    //创建商品实例
                    CObject_Item pItem = ObjectSystem.Instance.NewItem(packet.MerchadiseList[i].idTable);
                    if (pItem == null) continue;

                    //设置该物品在货架上的位置
                    pItem.PosIndex = i;
                    //设置该物品的数量（每一组的数量）
                    pItem.Num = (packet.MerchadiseList[i].byNumber);
                    //设置有限商品的最大数量（显示用）
                    pItem.SetMax(packet.MerchadiseList[i].MaxNumber);
                    //价格放入数据池
                    CDataPool.Instance.Booth_SetItemPrice(i, packet.MerchadiseList[i].nPrice);
                    //加入数据池
                    CDataPool.Instance.Booth_SetItem(i, pItem);
                }
                //设置数量
                CDataPool.Instance.Booth_SetNumber(nNum);
                CDataPool.Instance.Booth_SetBuyType(packet.BuyType);
                CDataPool.Instance.Booth_SetRepairType(packet.RepairType);
                CDataPool.Instance.Booth_SetRepairLevel(packet.RepairLevel);
                CDataPool.Instance.Booth_SetRepairSpend(packet.RepairSpend);
                CDataPool.Instance.Booth_SetRepairOkProb(packet.RepairOkProb);
                CDataPool.Instance.Booth_SetBuyLevel(packet.BuyLevel);
                CDataPool.Instance.Booth_SetScale(packet.fScale);
                CDataPool.Instance.Booth_SetCallBack(packet.bBuyBack == 1);
                CDataPool.Instance.Booth_SetCurrencyUnit(packet.CurrencyUnit);
                CDataPool.Instance.Booth_SetSerialNum(packet.SerialNum);
                CDataPool.Instance.Booth_SetbCanBuyMult(packet.CanBuyMulti);
                CDataPool.Instance.Booth_SetShopType(packet.ShopType);

                //更新到ActionSystem
                if (nNum > 0)
                    CActionSystem.Instance.Booth_Update();

                //通知UI
                CDataPool.Instance.Booth_Open();

                //得到商人Obj
                int nID = -1;

                CObject pObj = CObjectManager.Instance.FindServerObject((int)packet.ObjectID);
                if (pObj == null)
                {
                    nID = pObj.ID;
                    CUIDataPool.Instance.SetCurShopNpcId(pObj.ServerID);
                }
                else
                {
                    nID = -1;
                }

                CDataPool.Instance.Booth_SetShopNpcId(nID);
                CDataPool.Instance.Booth_SetShopUniqueId(packet.UniqueID);

                //CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_OPEN_BOOTH);//, nID);
                UIWindowMng.Instance.ShowWindow("ShopWindow");
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SHOPMERCHANDISELIST;
        }
    }
};
