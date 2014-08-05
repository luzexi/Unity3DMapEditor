using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCScriptCommandHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            try
            {
                GCScriptCommand newScriptCommand = (GCScriptCommand)pPacket;

                SCommand_DPC cmdTemp = new SCommand_DPC();
                cmdTemp.m_wID = DPC_SCRIPT_DEFINE.DPC_SCRIPT_COMMAND;

                cmdTemp.SetValue<ENUM_SCRIPT_COMMAND>(0, newScriptCommand.GetCmdID());
                cmdTemp.SetValue<object>(1, newScriptCommand.getBuf());
                CUIDataPool.Instance.OnCommand_(cmdTemp);

                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            catch (Exception e)
            {
                LogManager.LogError("--------------------GCScriptCommandHandler的Execute()方法出错。--------------------");
                LogManager.LogError(e.ToString());
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
            }
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SCRIPTCOMMAND;
        }
    }
}