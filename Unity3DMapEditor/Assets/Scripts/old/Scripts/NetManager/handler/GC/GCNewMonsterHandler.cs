
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCNewMonsterHandler 空壳
    /// </summary>
    public class GCNewMonsterHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            //if(GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            //    LogManager.Log("RECV GCNewMonster");
            //当前流程是主流程
	        if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
                GCNewMonster newMonsterPacket = (GCNewMonster) pPacket;
		        CObjectManager pObjectManager = GameProcedure.s_pObjectManager;
// 		        //检查位置是否合法
// 		        if(!CWorldManager::GetMe().GetActiveScene().IsValidPosition(fVector2(newMonsterPacket.getWorldPos().m_fX, newMonsterPacket.getWorldPos().m_fZ)))
// 		        {
// 			        TDThrow("ERROR POSITION @ GCNewMonsterHandler");
// 		        }

		        //创建玩家
		        CObject_PlayerNPC  pNPC = (CObject_PlayerNPC )pObjectManager.FindServerObject( (int)newMonsterPacket.ObjectID);

		        if(pNPC==null)
		        {
			        pNPC = pObjectManager.NewPlayerNPC( (int)newMonsterPacket.ObjectID );
			        pNPC.Initial(null);

		        }
		        else
		        {
			       // pNPC.Enable( OSF_VISIABLE );
			       // pNPC.Disalbe( OSF_OUT_VISUAL_FIELD );
		        }
                //设置怪物位置和面向
			    pNPC.SetMapPosition( newMonsterPacket.Position.m_fX, newMonsterPacket.Position.m_fZ);

                pNPC.SetFaceDir(newMonsterPacket.Dir);

		        if( newMonsterPacket.IsNpc!=0 )
			        pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_NPC);
		        else
			        pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_MONSTER);

		        pNPC.GetCharacterData().Set_MoveSpeed(newMonsterPacket.MoveSpeed);

// 		        SGWEB.SCommand_Object cmdTemp;
// 		        cmdTemp.m_wID			= (SGWEB.AICommandDef)ObjectCommandDef.OC_IDLE;
// 		        cmdTemp.m_afParam[0]	= newMonsterPacket.Position.m_fX;
// 		        cmdTemp.m_afParam[1]	= newMonsterPacket.Position.m_fZ;
// 		        cmdTemp.m_abParam[2]	= false;	
		       // pNPC.PushCommand(cmdTemp );

		        //放入Ask队列
		        pObjectManager.LoadQueue.TryAddLoadTask(pNPC.ID);

		        //此版不做服务器繁忙客户端延后发消息的处理
// 		        CGCharAskBaseAttrib msgAskBaseAttrib = new CGCharAskBaseAttrib();
// 		        msgAskBaseAttrib.setTargetID( (uint)newMonsterPacket.ObjectID);
// 		        GameProcedure.s_NetManager.SendPacket(msgAskBaseAttrib );

// 		        char szTemp[MAX_PATH];
// 		        _snprintf(szTemp, MAX_PATH, "GCNewMonster(%.1f,%.1f)", 
// 			        newMonsterPacket.getWorldPos().m_fX, newMonsterPacket.getWorldPos().m_fZ);
// 		        pNPC.PushDebugString(szTemp);
 		        pNPC.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
					
			}
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWMONSTER;
        }
    }
}
