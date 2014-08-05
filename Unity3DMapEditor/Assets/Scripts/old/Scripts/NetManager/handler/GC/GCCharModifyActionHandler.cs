using System;
using System.Collections.Generic;
using Network.Packets;
namespace Network.Handlers
{
    public class GCCharModifyActionHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LogManager.LogWarning("RECV GCCharModifyAction");
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCCharModifyAction packet = pPacket as GCCharModifyAction;
                CObject obj = CObjectManager.Instance.FindServerObject(packet.ObjectID);
                if (obj == null)
                {
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }
                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_MODIFY_ACTION;
                cmdTemp.SetValue<int>(0,packet.LogicCount);
                cmdTemp.SetValue<int>(1,packet.ModifyTime);
                obj.PushCommand(cmdTemp);
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARMODIFYACTION;
        }
    }
}