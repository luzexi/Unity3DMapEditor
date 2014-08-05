
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public  class GCNewPlayer_MoveHandler : HandlerBase
    {
    
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LogManager.Log("RECV GCNewPlayer_Move");
	        //当前流程是主流程
	        if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
		        CObjectManager pObjectManager = CObjectManager.Instance;
                GCNewPlayer_Move newPlayerMovPacket = (GCNewPlayer_Move)pPacket;

// 		        //检查位置是否合法
// 		        if(!CWorldManager::GetMe().GetActiveScene().IsValidPosition(fVector2(pPacket.getWorldPos().m_fX, pPacket.getWorldPos().m_fZ)))
// 		        {
// 			        TDThrow("ERROR POSITION @ GCNewMonster_MoveHandler");
// 		        }
// 		        if(!CWorldManager::GetMe().GetActiveScene().IsValidPosition(fVector2(pPacket.getTargetPos().m_fX, pPacket.getTargetPos().m_fZ)))
// 		        {
// 			        TDThrow("ERROR POSITION @ GCNewMonster_MoveHandler target");
// 		        }

		        //创建玩家
		        CObject_PlayerNPC pOther = (CObject_PlayerNPC)pObjectManager.FindServerObject( (int)newPlayerMovPacket.ObjID );

		        if ( pOther == null )
		        {
                    pOther = pObjectManager.NewPlayerOther((int)newPlayerMovPacket.ObjID);
			        pOther.Initial( null );
		        }
		        else
		        {

			        //pOther.Enable( OSF_VISIABLE );
			        // pOther.Disalbe( OSF_OUT_VISUAL_FIELD );
		        }
                //设置x z坐标
                pOther.SetMapPosition(newPlayerMovPacket.PosWorld.m_fX, newPlayerMovPacket.PosWorld.m_fZ);

                pOther.GetCharacterData().Set_EquipVer(newPlayerMovPacket.EquipVer);
		        pOther.GetCharacterData().Set_MoveSpeed(newPlayerMovPacket.MoveSpeed);

		        // move to command
		        {
                    WORLD_POS[] posTarget = new WORLD_POS[1];
                    posTarget[0].m_fX = newPlayerMovPacket.PosTarget.m_fX;
                    posTarget[0].m_fZ = newPlayerMovPacket.PosTarget.m_fZ;

                    SCommand_Object cmdTemp = new SCommand_Object();
			        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_MOVE;
                    cmdTemp.SetValue<uint>(0, 0);
			        cmdTemp.SetValue(1, newPlayerMovPacket.HandleID);
			        cmdTemp.SetValue(2,1);
			        cmdTemp.SetValue<WORLD_POS[]>(3, posTarget);
			        pOther.PushCommand(cmdTemp );
		        }

                //放入Ask队列
                pObjectManager.LoadQueue.TryAddLoadTask(pOther.ID);
		       // CObjectManager::GetMe().GetLoadQueue().TryAddLoadTask(pOther.GetID(), CObject_Character::CT_MONSTER);

		        //tempcode
//                 {
// 		            //此版不做服务器繁忙客户端延后发消息的处理
//                     CGCharAskBaseAttrib msgAskBaseAttrib = new CGCharAskBaseAttrib();
// 		            msgAskBaseAttrib.setTargetID( (uint)(int)newPlayerMovPacket.ObjID);
// 		            GameProcedure.s_NetManager.SendPacket(msgAskBaseAttrib );
// 
// 		         }

		        pOther.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());

	        }

	        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE; ;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWPLAYER_MOVE;
        }
    }


    
}




