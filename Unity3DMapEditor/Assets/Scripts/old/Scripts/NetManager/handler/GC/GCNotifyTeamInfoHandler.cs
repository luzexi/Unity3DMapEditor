
using System;

using Network.Packets;

namespace Network.Handlers
{

    public class GCNotifyTeamInfoHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCNotifyTeamInfo packet = pPacket as GCNotifyTeamInfo;
                CObject pObj = CObjectManager.Instance.FindServerObject((int)packet.ObjectId);
                if (pObj == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                SCommand_Object cmdTemp = new SCommand_Object();
                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_UPDATE_TEAM_FLAG;
                cmdTemp.SetValue<byte>(0, packet.TeamFlag);
                cmdTemp.SetValue<byte>(1, packet.TeamLeaderFlag);
                cmdTemp.SetValue<byte>(2, packet.TeamFullFlag);
    
                pObj.PushCommand(cmdTemp);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NOTIFYTEAMINFO;
        }
    }
}
