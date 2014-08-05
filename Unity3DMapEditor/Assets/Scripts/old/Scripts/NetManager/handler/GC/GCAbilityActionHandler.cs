using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCAbilityActionHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCAbilityAction packet = pPacket as GCAbilityAction;
                CObjectManager pObjectManager = CObjectManager.Instance;
                if (pObjectManager == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
                CObject pObj = pObjectManager.FindServerObject((int)packet.ObjectID);
                if (pObj == null)
                {
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                if (packet.BeginOrEnd == (byte)GCAbilityAction.ActionState.ABILITY_BEGIN)
                {
                    SCommand_Object cmdTemp = new SCommand_Object();
                    cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_ABILITY;
                    cmdTemp.SetValue(0, 0);
                    cmdTemp.SetValue<int>(1, packet.LogicCount);
                    cmdTemp.SetValue<short>(2, packet.AbilityID);
                    cmdTemp.SetValue<int>(3, packet.PrescriptionID);
                    cmdTemp.SetValue<uint>(4, packet.TargetID);
                    pObj.PushCommand(cmdTemp);
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ABILITY_ACTION;
        }
    }
}