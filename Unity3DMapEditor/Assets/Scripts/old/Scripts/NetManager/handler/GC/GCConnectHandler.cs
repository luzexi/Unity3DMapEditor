// 用于测试与简易服务端相连 [12/15/2011 ZL]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCConnectHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
           
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcChangeScene)
            {
                //LogManager.Log("Recv GCConnect!");

                GCConnect Packet = (GCConnect)pPacket;
                //设置要进入的场景的数据

                //!!Import 
                //只有第一次连接某服务器，该消息发回来的场景id和进入位置才有意义
                //其他情况（从别的场景切换过来，从别的服务器切换过来）该消息
                //发回来的这两个值是不可信的，需要使用GCNotifyChangeScene通知的值
                if (Packet.Estate == 1)
                {
                    CGConnect msg = (CGConnect)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_CONNECT);
                    msg.Key = GameProcedure.s_pVariableSystem.GetAs_Int("GameServer_Key");
                    msg.ServerID = -1;
                    msg.GUID = GameProcedure.s_pVariableSystem.GetAs_Int("User_GUID");
                    byte[] temp = Encoding.ASCII.GetBytes(GameProcedure.s_pVariableSystem.GetAs_String("User_NAME"));
                    Array.Copy(temp, msg.SzAccount, temp.Length);
                    msg.Gender = GameProcedure.s_pVariableSystem.GetAs_Int("User_GENDER");
                    NetManager.GetNetManager().SendPacket(msg);
                    GameProcedure.s_ProcChangeScene.SetStatus(GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_SENDING_CGCONNECT);

                }
                else
                {

                    //场景id	
                    bool Had = false ;
                    GameProcedure.s_pVariableSystem.GetAs_Int("Scene_ID", ref Had);
                    if (!Had)
                    {
                        GameProcedure.s_pVariableSystem.SetAs_Int("Scene_ID", Packet.SceneID);
                    }

                    //场景位置
                   
                    GameProcedure.s_pVariableSystem.GetAs_Vector2("Scene_EnterPos", ref Had);
                    if (!Had)
                    {
                        GameProcedure.s_pVariableSystem.SetAs_Vector2("Scene_EnterPos", Packet.Position.m_fX, Packet.Position.m_fZ);
                        //LogManager.Log("Scene_EnterPos" + pos);

                    }

                    if (GameProcedure.s_ProcChangeScene != null)
                    {
                        GameProcedure.s_ProcChangeScene.SetStatus(GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_RECEIVE_CGCONNECT_SUCCESS);
                    }
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CONNECT;
        }
    }
}
