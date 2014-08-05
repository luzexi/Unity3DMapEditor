using System;

using Network.Packets;
namespace Network.Handlers
{
    public class GCShopBuyHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCShopBuy packet = pPacket as GCShopBuy;
                GCShopBuy.BuyResult		bIsBuyOk	=	(GCShopBuy.BuyResult)packet.IsBuyOk; 
		        uint		ItemIndex	=	packet.ItemIndex;
		        byte		ItemNum		=	packet.ItemNum;

		        //已经关上了就算了
		        if(CDataPool.Instance.Booth_IsClose())
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE ;

		        if(bIsBuyOk == GCShopBuy.BuyResult.BUY_MONEY_FAIL)
		        {
			        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"金钱不足.");
		        }
		        else if(bIsBuyOk == GCShopBuy.BuyResult.BUY_RMB_FAIL)
		        {
			        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"元宝不足.");
		        }
		        else if(bIsBuyOk == GCShopBuy.BuyResult.BUY_BAG_FULL)
		        {
			        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"包裹已满");
		        }
		        else if(bIsBuyOk == GCShopBuy.BuyResult.BUY_NO_MERCH)
		        {
			        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF,"商品已改变，请重新打开");
		        }
		        else if(bIsBuyOk == GCShopBuy.BuyResult.BUY_OK)
		        {
	                //CObject_Item pItem = ObjectSystem.Instance.NewItem( ItemIndex );
                    //CHAR szMsg[256];
        			
                    //_snprintf(szMsg, 256, "您购买了%d个%s",	ItemNum, pItem->GetName());

                    //ADDTALKMSG(szMsg);
			        //ObjectSystem.Instance.DestroyItem(pItem);
			        //CGameProcedure::s_pEventSystem->PushEvent(GE_INFO_SELF,"交易成功");
			        //CSoundSystemFMod::_PlayUISoundFunc(25+59);
		        }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SHOPBUY;
        }
    }
};
