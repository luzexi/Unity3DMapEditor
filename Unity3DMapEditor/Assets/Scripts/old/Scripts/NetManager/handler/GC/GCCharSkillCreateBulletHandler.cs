using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCharSkillCreateBulletHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCCharSkillCreateBullet Packet = (GCCharSkillCreateBullet)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
                LogManager.Log("RECV GCCharSkillCreateBullet");
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARSKILL_CREATEBULLET;
        }
    }
};
