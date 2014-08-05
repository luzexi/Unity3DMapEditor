using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class LCStatusHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCStatus msg = (LCStatus)pPacket;
            //Console.WriteLine("LCStatus Recv : packetId:" + msg.getPacketID() +
            //", packetSize:" + msg.getPacketSize() +
            //", 当前排队序列：" + msg.M_TurnNumber +
            //", 当前排队状态：" + msg.M_ClientStatus);

            //LogManager.Log("LCStatus Recv : packetId:" + msg.getPacketID() +
            //", packetSize:" + msg.getPacketSize() +
            //", 当前排队序列：" + msg.M_TurnNumber +
            //", 当前排队状态：" + msg.M_ClientStatus);
            LogManager.Log("RECV LCStatus: " + msg.M_ClientStatus);

            if (null == pPacket)
            {
                return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            NET_RESULT_DEFINE.CLIENT_TURN_STATUS res = msg.M_ClientStatus;
            switch (res)
            {
                case NET_RESULT_DEFINE.CLIENT_TURN_STATUS.CTS_NORMAL:
                    {
                        GameProcedure.s_ProcLogIn.ChangeToCharacterSel();
                        break;
                    }
                case NET_RESULT_DEFINE.CLIENT_TURN_STATUS.CTS_TURN:
                    {
                        int iTurn = msg.M_TurnNumber;
                        char[] bufInfo = new char[1024];

                        //memset(bufInfo, 0, sizeof(bufInfo));
                        //_stprintf(bufInfo, "服务器已满,你被排在 %d 名", iTurn);
                        //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO_NO_BUTTON, bufInfo);
                        break;
                    }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_STATUS;
        }
    }
}