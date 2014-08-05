using System;

using Network.Packets;
namespace Network.Handlers
{
    public class GCShopUpdateMerchandiseListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCShopUpdateMerchandiseList packet = pPacket as GCShopUpdateMerchandiseList;
                //已经关上了就算了
                if (CDataPool.Instance.Booth_IsClose())
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                //更新的商品的个数
                int nNum = packet.MerchadiseNum;
                //添加到数据池中
                for (int i = 0; i < nNum; i++)
                {
                    //查找商品实例
                    CObject_Item pItem = CDataPool.Instance.Booth_GetItemByID((int)packet.MerchadiseList[i].idTable);
                    //TDAssert(pItem);
                    if (pItem == null)
                        continue;

                    //设置新的数量
                    int num =  packet.MerchadiseList[i].byNumber;
                    pItem.SetMax(num);
                }
                //更新到ActionSystem
                if (nNum > 0) CActionSystem.Instance.Booth_Update();
                //更新到UI
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BOOTH);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SHOPUPDATEMERCHANDISELIST;
        }
    }
};
