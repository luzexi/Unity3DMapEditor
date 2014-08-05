
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCNewItemBoxHandler 空壳
    /// </summary>
    public class GCNewItemBoxHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase packet, ref Peer pPlayer)
        {
            GCNewItemBox pPacket = (GCNewItemBox) packet;
	        //当前流程是主流程
	        if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
		        CObjectManager pObjectManager = CObjectManager.Instance;
        		
		        //检查位置是否合法
				fVector2 pos = new fVector2(pPacket.Position.m_fX, pPacket.Position.m_fZ);
		        if( !WorldManager.Instance.ActiveScene.IsValidPosition(ref pos) )
		        {
                    LogManager.LogError("Valid Position @GCNewItemBoxHandler.Execute");
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
		        }

		        if( (int)ITEMBOX_TYPE.ITYPE_DROPBOX == pPacket.ObjectType)
		        {
			        bool bMustCreater = true;
			        CObject pObj = (CObject)(pObjectManager.FindServerObject( (int)pPacket.MonsterID ));
			        if ( pObj != null && pObj is CObject_Character )
			        {
				        CObject_Character pCharacter		= (CObject_Character)pObj;
				        if(!pCharacter.IsDie())
				        {
					        uint		idItemBox		= (uint)pPacket.ObjectID;
					        uint		idOwner			= (uint)pPacket.OwnerID;
					        WORLD_POS	posCreate		= pPacket.Position;
					        pCharacter.AddDropBoxEvent(idItemBox, idOwner, ref posCreate);
					        bMustCreater = false;
				        }
			        }

			        if(bMustCreater)
			        {
				        //创建ItemBox
				        CTripperObject_ItemBox pBox = (CTripperObject_ItemBox)CObjectManager.Instance.NewTripperItemBox((int)pPacket.ObjectID);
				        pBox.Initial(null);	
				        //设置位置
				        pBox.SetMapPosition(pPacket.Position.m_fX, pPacket.Position.m_fZ);
				        //设置掉落箱的归属
				        pBox.SetOwnerGUID((uint)pPacket.OwnerID);

				        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_DROP_ITEMBOX);
			        }
		        }
		        else
		        {
			        //创建矿物生长点
			        CTripperObject_Resource pResource = (CTripperObject_Resource)CObjectManager.Instance.NewTripperResource((int) pPacket.ObjectID);
			        if(!(pResource.SetResourceID((int)pPacket.ObjectType)))
			        {
				        //非法的资源ID
				        CObjectManager.Instance.DestroyObject(pResource);
				        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
			        }
			        pResource.Initial(null);	
			        //设置位置
			        pResource.SetMapPosition( pPacket.Position.m_fX, pPacket.Position.m_fZ);
                    jhCount++;
		        }
	        }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        static int jhCount = 0;
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWITEMBOX;
        }
    }
}
