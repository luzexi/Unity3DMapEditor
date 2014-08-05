using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{

    public class LCRetConnectHandler : HandlerBase
    {
        public string inputString = "oooosadf";
        public string szCount = "aaa";
        public string password = "bbb";
        public static int verson = 1015;

        public static void sendCLAskLogin(string szCount, string password, int verson)
        {
            CLAskLogin msg = (CLAskLogin)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CL_ASKLOGIN);
            byte[] temp = Encoding.ASCII.GetBytes(szCount);
            Array.Copy(temp, msg.SzAccount, temp.Length);
            temp = Encoding.ASCII.GetBytes(password);
            Array.Copy(temp, msg.SzPassWord, temp.Length);
            msg.UVersion = verson;
            NetManager.GetNetManager().SendPacket(msg);
        }

        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCRetConnect msg = (LCRetConnect)pPacket;
//             string ip = Encoding.ASCII.GetString(msg.M_LoginIp);
//             Console.WriteLine("LCRetConnect : packetId:" + msg.getPacketID() +
//             ", packetSize:" + msg.getPacketSize() +
//             "\nLOGIN_CONNECT_RESULT: 0成功 1人满 2暂停, 当前连接状态：" + msg.M_Result +
//             "\n服务器返回负载IP：" + ip +
//             "\n服务器返回负载端口：" + msg.M_LoginPort);
//             Console.WriteLine("GOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOAL!");
// 
//             LogManager.Log("LCRetConnect : packetId:" + msg.getPacketID() +
//             ", packetSize:" + msg.getPacketSize() +
//             ", LOGIN_CONNECT_RESULT: 0成功 1人满 2暂停, 当前连接状态：" + msg.M_Result +
//             ", 服务器返回负载IP：" + ip +
//             ", 服务器返回负载端口：" + msg.M_LoginPort);

            //发送CLAskLogin登陆消息包
            //sendCLAskLogin(szCount, password, verson);
            // askLogin放到login流程中 [12/21/2011 Administrator]
            LogManager.Log("RECV LCRetConnect");
            GameProcedure.s_ProcLogIn.SendCheckAccountMsg();
			GameProcedure.s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_REQUESTING);

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_RETCONNECT;
        }
    }
}