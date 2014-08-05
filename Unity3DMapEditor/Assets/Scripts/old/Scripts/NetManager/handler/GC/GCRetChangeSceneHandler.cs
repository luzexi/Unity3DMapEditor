
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCRetChangeSceneHanlder 空壳
    /// </summary>
    public class GCRetChangeSceneHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {

            GCRetChangeScene Packet = (GCRetChangeScene)pPacket;

            LogManager.Log("RECV GCRetChangeSceneHanlder : return =" + Packet.Return);

            
            //当前流程是登录流程
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain &&
                (WorldManager.Instance.GetStation() == WorldManager.WORLD_STATION.WS_ASK_CHANGE_SCANE || WorldManager.Instance.GetStation() == WorldManager.WORLD_STATION.WS_RELIVE))
            {
                //不允许进入
                if (Packet.Return == (byte)GCRetChangeScene.CHANGESCENERETURN.CSR_ERROR)
                {
                    //GameProcedure.s_pGfxSystem->PushDebugString("ChangeScene Denied");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                //不需要切换服务器
                if (Packet.Return == (byte)GCRetChangeScene.CHANGESCENERETURN.CSR_SUCCESS)
                {
                    //设置即将进入的场景的数据
                    //CGameProcedure::s_pVariableSystem->SetAs_Int("Scene_ID", CWorldManager::GetMe()->GetNextSenceID()),
                    //CGameProcedure::s_pVariableSystem->SetAs_Vector2("Scene_EnterPos", 
                    //    CWorldManager::GetMe()->GetNextScenePos().x,
                    //    CWorldManager::GetMe()->GetNextScenePos().y	),

                    //CGameProcedure::s_pEventSystem->PushEvent(GE_ON_SCENE_TRANS);
                    GameProcedure.s_pVariableSystem.SetAs_Int("Scene_ID", WorldManager.Instance.GetNextSenceID());
                    GameProcedure.s_pVariableSystem.SetAs_Vector2("Scene_EnterPos", WorldManager.Instance.GetNextScenePos().x, WorldManager.Instance.GetNextScenePos().y);
                    //通知开始切换场景
                    //WorldManager.OnSceneTransEvent(GAME_EVENT_ID.GE_ON_SCENE_TRANS);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_ON_SCENE_TRANS);

                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                //需要切换服务器
                if (Packet.Return == (byte)GCRetChangeScene.CHANGESCENERETURN.CSR_SUCCESS_DIFFSERVER)
                {
                    //设置即将进入的场景的数据
                    //CGameProcedure::s_pVariableSystem->SetVariable("GameServer_Address", pPacket->GetIP()),
                    //CGameProcedure::s_pVariableSystem->SetAs_Int("GameServer_Port", pPacket->GetPort()),
                    //CGameProcedure::s_pVariableSystem->SetAs_Int("GameServer_Key", pPacket->GetKey()),
                    //CGameProcedure::s_pVariableSystem->SetAs_Int("Scene_ID", CWorldManager::GetMe()->GetNextSenceID()),
                    //CGameProcedure::s_pVariableSystem->SetAs_Vector2("Scene_EnterPos", 
                    //    CWorldManager::GetMe()->GetNextScenePos().x,
                    //    CWorldManager::GetMe()->GetNextScenePos().y	),

                    //CGameProcedure::s_pEventSystem->PushEvent(GE_ON_SERVER_TRANS);
                    string ip = Encoding.ASCII.GetString(Packet.IP);
                    GameProcedure.s_pVariableSystem.SetVariable("GameServer_Address", ip);
                    GameProcedure.s_pVariableSystem.SetAs_Int("GameServer_Port", Packet.Port);
                    GameProcedure.s_pVariableSystem.SetAs_Int("GameServer_Key", Packet.Key);
                    GameProcedure.s_pVariableSystem.SetAs_Int("Scene_ID", WorldManager.Instance.GetNextSenceID());
                    GameProcedure.s_pVariableSystem.SetAs_Vector2("Scene_EnterPos", WorldManager.Instance.GetNextScenePos().x, WorldManager.Instance.GetNextScenePos().y);

                    //通知切换服务器
                    //WorldManager.OnSceneTransEvent(GAME_EVENT_ID.GE_ON_SERVER_TRANS);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_ON_SERVER_TRANS);

                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
            }


            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_RETCHANGESCENE;
        }
    }
}
