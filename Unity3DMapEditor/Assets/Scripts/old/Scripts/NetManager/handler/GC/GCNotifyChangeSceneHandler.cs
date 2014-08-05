
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{

    public class GCNotifyChangeSceneHanlder : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                // 场景资源ID和服务器ID分开，暂时使用相同ID [10/24/2011 Sun]
                GCNotifyChangeScene Packet = (GCNotifyChangeScene)pPacket;

                LogManager.Log("RECV GCNotifyChangeScene");
                LogManager.Log("RECV GCNotifyChangeScene: id=" + Packet.TargetSceneID);
                
                int nSourceID = WorldManager.Instance.GetActiveSceneID();
                if (nSourceID != Packet.CurrentSceneID) return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                fVector2 pos = new fVector2(Packet.Position.m_fX, Packet.Position.m_fZ);
                WorldManager.Instance.ChangeScene(
                    Packet.TargetSceneID,
                    Packet.ResID,
                    ref pos,
                    Packet.TargetDir,
                    Packet.Flag);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NOTIFYCHANGESCENE;
        }
    }
}
