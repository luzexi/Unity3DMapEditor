using System;
using System.Collections.Generic;

using Network.Packets;
using UnityEngine;

namespace Network.Handlers
{
    /// <summary>
    /// GCMyBagListHandler 空壳
    /// </summary>
    public class GCMyBagListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                {
                    GCMyBagList myBag = (GCMyBagList)pPacket;
                    CDataPool.Instance.UserBag_Clear();

                    int nItemCount = (int)myBag.AskCount;
                    for (int i = 0; i < nItemCount; i++)
                    {
                        _BAG_ITEM pItem = myBag.GetItemData()[i];
                        // 			        if(pItem == null) 
                        //                         continue;

                        CObject_Item pItemObj = ObjectSystem.Instance.NewItem(pItem.m_ItemTableIndex);
                        if (pItemObj == null)
                            continue;

                        pItemObj.SetGUID((ushort)pItem.m_ItemID.m_World, (ushort)pItem.m_ItemID.m_Server, (uint)pItem.m_ItemID.m_Serial);
                        pItemObj.Num = pItem.m_Count;
                        pItemObj.TypeOwner = ITEM_OWNER.IO_MYSELF_PACKET;
                        pItemObj.PosIndex = pItem.m_nndex;

                        CDataPool.Instance.UserBag_SetItem(pItem.m_nndex, pItemObj, true, false);
                    }

                    //更新到ActionSystem
                    if (nItemCount > 0)
                        CActionSystem.Instance.UserBag_Update();

                    //UI
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED);
                }
            }
            catch(Exception ex)
            {
                LogManager.LogError(ex.ToString());
            }
            finally
            {

            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_MYBAGLIST;
        }
    }
}
