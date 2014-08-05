
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
    public class GCNewPlayer_DeathHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                LogManager.Log("RECV GCNewPlayer_Death");
            //当前流程是主流程
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCNewPlayer_Death newPlayerPacket = (GCNewPlayer_Death)pPacket;
                CObjectManager pObjectManager = GameProcedure.s_pObjectManager;
                                ////检查位置是否合法
                                //if(!CWorldManager::GetMe().GetActiveScene().IsValidPosition(fVector2(newPlayerPacket.getWorldPos().m_fX, newPlayerPacket.getWorldPos().m_fZ)))
                                //{
                                //    TDThrow("ERROR POSITION @ GCNewMonsterHandler");
                                //}

                //创建玩家
                CObject_PlayerOther pOther = (CObject_PlayerOther)pObjectManager.FindServerObject((int)newPlayerPacket.ObjectID);

                if (pOther == null)
                {
                    pOther = pObjectManager.NewPlayerOther((int)newPlayerPacket.ObjectID);
                    pOther.Initial(null);

                }
                else
                {
                    // pOther.Enable( OSF_VISIABLE );
                    // pOther.Disalbe( OSF_OUT_VISUAL_FIELD );
                }
                //设置怪物位置和面向
                pOther.SetMapPosition(newPlayerPacket.Position.m_fX, newPlayerPacket.Position.m_fZ);

                pOther.SetFaceDir(newPlayerPacket.Dir);

                //更新装备信息
                pOther.GetCharacterData().Set_EquipVer(newPlayerPacket.EquipVer);
                pOther.GetCharacterData().Set_MoveSpeed(newPlayerPacket.MoveSpeed);

                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_DEATH;
                cmdTemp.SetValue<bool>(0, false);
                pOther.PushCommand(cmdTemp);

                //放入Ask队列
                pObjectManager.LoadQueue.TryAddLoadTask(pOther.ID);
                //pObjectManager.GetLoadQueue().TryAddLoadTask(pOther.GetID(), CObject_Character::CT_MONSTER);

                //此版不做服务器繁忙客户端延后发消息的处理
                // 		        CGCharAskBaseAttrib msgAskBaseAttrib = new CGCharAskBaseAttrib();
                // 		        msgAskBaseAttrib.setTargetID( (uint)newPlayerPacket.ObjectID);
                // 		        GameProcedure.s_NetManager.SendPacket(msgAskBaseAttrib );

                // 		        char szTemp[MAX_PATH];
                // 		        _snprintf(szTemp, MAX_PATH, "GCNewMonster(%.1f,%.1f)", 
                // 			        newPlayerPacket.getWorldPos().m_fX, newPlayerPacket.getWorldPos().m_fZ);
                // 		        pOther.PushDebugString(szTemp);
                pOther.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());

            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWPLAYER_DEATH;
        }
    }
}
