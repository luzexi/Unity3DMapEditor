
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public  class GCNewMonster_MoveHandler : HandlerBase
    {
    
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            
	        //当前流程是主流程
	        if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
		        CObjectManager pObjectManager = CObjectManager.Instance;
                GCNewMonster_Move newMonsterMovPacket = (GCNewMonster_Move)pPacket;

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
		        CObject_PlayerNPC pNPC = (CObject_PlayerNPC)pObjectManager.FindServerObject( (int)newMonsterMovPacket.getObjID() );

		        if ( pNPC == null )
		        {
			        pNPC = pObjectManager.NewPlayerNPC((int)newMonsterMovPacket.getObjID());
			        pNPC.Initial( null );
		        }
		        else
		        {

			        //pNPC.Enable( OSF_VISIABLE );
			        // pNPC.Disalbe( OSF_OUT_VISUAL_FIELD );
		        }
                //设置x z坐标
			    pNPC.SetMapPosition(newMonsterMovPacket.getWorldPos().m_fX, newMonsterMovPacket.getWorldPos().m_fZ);
                if( newMonsterMovPacket.getIsNPC() )
			        pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_NPC);
		        else
			        pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_MONSTER);

		        pNPC.GetCharacterData().Set_MoveSpeed(newMonsterMovPacket.getMoveSpeed());

		        // move to command
		        {
                    WORLD_POS[] posTarget = new WORLD_POS[1];
                    posTarget[0].m_fX = newMonsterMovPacket.getTargetPos().m_fX;
                    posTarget[0].m_fZ = newMonsterMovPacket.getTargetPos().m_fZ;

                    SCommand_Object cmdTemp = new SCommand_Object();
			        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_MOVE;
                    cmdTemp.SetValue<uint>(0, 0);
			        cmdTemp.SetValue(1, newMonsterMovPacket.getHandleID());
			        cmdTemp.SetValue(2,1);
			        cmdTemp.SetValue<WORLD_POS[]>(3, posTarget);
			        pNPC.PushCommand(cmdTemp );
		        }

                //放入Ask队列
                pObjectManager.LoadQueue.TryAddLoadTask(pNPC.ID);

		        //tempcode
//                 {
// 		            //此版不做服务器繁忙客户端延后发消息的处理
//                     CGCharAskBaseAttrib msgAskBaseAttrib = new CGCharAskBaseAttrib();
// 		            msgAskBaseAttrib.setTargetID( (uint)(int)newMonsterMovPacket.getObjID());
// 		            GameProcedure.s_NetManager.SendPacket(msgAskBaseAttrib );
// 
// 		         }

		        pNPC.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());

	        }

	        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE; ;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWMONSTER_MOVE;
        }
    }


    
}




