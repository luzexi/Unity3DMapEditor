using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankAcquireListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankAcquireList packet = pPacket as GCBankAcquireList;
                //清空原有商品列表
			    CDataPool.Instance.UserBank_Clear();

			    //银行里面的钱数
			    CDataPool.Instance.UserBank_SetBankMoney(packet.Money);

			    //当前银行的大小
			    CDataPool.Instance.UserBank_SetBankEndIndex(packet.CurBankSize);

			    GCBankAcquireList._BANK_ITEM[]	ItemList = packet.ItemList;
			    int nNum = packet.ItemNum;
			    int bankpos = 0;
			    CObject_Item pItem = null;

			    //添加到数据池中
			    for(int i=0; i<nNum; i++)
			    {
				    bankpos = ItemList[i].bankindex;

				    if(ItemList[i].isBlueEquip != 0)
				    {
					    pItem = ObjectSystem.Instance.NewItem( ItemList[i].item_data.m_ItemIndex);
					    if(pItem == null) 
                            throw new NullReferenceException("Bank Item Create failed:" + ItemList[i].item_data.m_ItemIndex);
					    pItem.SetGUID(
						    (ushort)ItemList[i].item_data.m_ItemGUID.m_World, 
						    ItemList[i].item_data.m_ItemGUID.m_Server,
						    (uint)ItemList[i].item_data.m_ItemGUID.m_Serial);
					    CDataPool.Instance.UserBank_SetItem(bankpos, pItem, true);
					    CDataPool.Instance.UserBank_SetItemExtraInfo(bankpos, false, ItemList[i].item_data);
				    }
				    else
				    {
					    pItem = ObjectSystem.Instance.NewItem( ItemList[i].item_guid);
					    if(pItem == null) 
                            throw new NullReferenceException("Bank Item Create failed:" + ItemList[i].item_data.m_ItemIndex);
					    CDataPool.Instance.UserBank_SetItem(bankpos, pItem , true);
				    }
				    pItem.PosIndex =(short) bankpos;
				    pItem.SetNumber(ItemList[i].byNumber);
			    }
    			
			    CActionSystem.Instance.UserBank_Update();

                int nBankNpcId = CDataPool.Instance.UserBank_GetNpcId();
                CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_TOGLE_BANK, nBankNpcId);
                UIWindowMng.Instance.GetWindow("StoreWindow");
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKACQUIRELIST;
        }
    }
}