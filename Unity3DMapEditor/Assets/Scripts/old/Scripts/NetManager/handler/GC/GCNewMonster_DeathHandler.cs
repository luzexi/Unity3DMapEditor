
using System;
using Network.Packets;

namespace Network.Handlers
{

    public class GCNewMonster_DeathHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            //当前流程是主流程
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCNewMonster_Death newMonsterPacket = (GCNewMonster_Death)pPacket;
                CObjectManager pObjectManager = GameProcedure.s_pObjectManager;
                // 		        //检查位置是否合法
                // 		        if(!CWorldManager::GetMe().GetActiveScene().IsValidPosition(fVector2(newMonsterPacket.getWorldPos().m_fX, newMonsterPacket.getWorldPos().m_fZ)))
                // 		        {
                // 			        TDThrow("ERROR POSITION @ GCNewMonsterHandler");
                // 		        }

                //创建玩家
                CObject_PlayerNPC pNPC = (CObject_PlayerNPC)pObjectManager.FindServerObject((int)newMonsterPacket.ObjectID);

                if (pNPC == null)
                {
                    pNPC = pObjectManager.NewPlayerNPC((int)newMonsterPacket.ObjectID);
                    pNPC.Initial(null);

                }
                else
                {
                    // pNPC.Enable( OSF_VISIABLE );
                    // pNPC.Disalbe( OSF_OUT_VISUAL_FIELD );
                }
                //设置怪物位置和面向
                pNPC.SetMapPosition(newMonsterPacket.Position.m_fX, newMonsterPacket.Position.m_fZ);
                pNPC.SetFaceDir(newMonsterPacket.Dir);

                if (newMonsterPacket.IsNpc != 0)
                    pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_NPC);
                else
                    pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_MONSTER);

                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_DEATH;
                cmdTemp.SetValue<bool>(0, false);
                pNPC.PushCommand(cmdTemp);

                //清除界面选中NPC
		        if (CObjectManager.Instance.GetMainTarget() != null && pNPC != null &&
			        (CObjectManager.Instance.GetMainTarget().ID == pNPC.ID))
		        {
			        GameProcedure.s_pGameInterface.Object_SelectAsMainTarget(MacroDefine.INVALID_ID);
		        }

                pNPC.GetCharacterData().Set_MoveSpeed(newMonsterPacket.MoveSpeed);

  
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
            return (int)PACKET_DEFINE.PACKET_GC_NEWMONSTER_DEATH;
        }
    }
}
