using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class LCRetLoginHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCRetLogin msg = (LCRetLogin)pPacket;
            //string count = Encoding.ASCII.GetString(msg.SzAccount);
            //Console.WriteLine("LCRetLogin Recv : packetId:" + msg.getPacketID() +
            //", packetSize:" + msg.getPacketSize() +
            //", 当前登录账号：" + count +
            //", 当前登录状态：" + msg.M_Result);

            //LogManager.Log("LCRetLogin Recv : packetId:" + msg.getPacketID() +
            //", packetSize:" + msg.getPacketSize() +
            //", 当前登录账号：" + count +
            //", 当前登录状态：" + msg.M_Result);

            LogManager.Log("RECV LCRetLogin");
            // 设定游戏状态 [12/21/2011 Administrator]
            GameProcedure.s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_BEGIN_REQUESTING);

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_RETLOGIN;
        }
    }
}
