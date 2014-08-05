using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankBeginHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankBegin packet = pPacket as GCBankBegin;
                LogManager.LogWarning("Bank npc id=" + packet.NPCID);
                //将银行NPC的ID保存到数据池
			    int ObjectID = (CObjectManager.Instance.FindServerObject((int)packet.NPCID)).ID;
			    CDataPool.Instance.UserBank_SetNpcId(ObjectID);

			    CGBankAcquireList Msg = new CGBankAcquireList();
                NetManager.GetNetManager().SendPacket(Msg);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKBEGIN;
        }
    }
}