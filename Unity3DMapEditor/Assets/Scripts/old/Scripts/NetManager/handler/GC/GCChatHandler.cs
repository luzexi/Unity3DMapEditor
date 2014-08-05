// 用于测试与简易服务端相连 [12/15/2011 ZL]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCChatHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCChat msg = (GCChat)pPacket;

            //Talk.Instance.ReceiveTalk(msg);
            Interface.Talk.Instance.HandleRecvTalkPacket(msg);
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHAT;
        }
    }
}
