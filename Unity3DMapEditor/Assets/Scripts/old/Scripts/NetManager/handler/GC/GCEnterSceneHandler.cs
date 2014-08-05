
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    /// <summary>
    /// GCEnterSceneHandler 空壳
    /// </summary>
    public class GCEnterSceneHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCEnterScene Packet = (GCEnterScene)pPacket;

            LogManager.Log("RECV GCEnterScene : result=" + Packet.Result);
            //允许进入
            if (Packet.Result == 0)
            {
                //----------------------------------------------   --------------------
                //保存自身数据
                GameProcedure.s_pVariableSystem.SetAs_Int("MySelf_ID", Packet.ObjectID);
              
                GameProcedure.s_pVariableSystem.SetAs_Vector2("MySelf_Pos", Packet.Position.m_fX , Packet.Position.m_fZ);


                //------------------------------------------------------------------
                //设置要进入的场景
                // 增加一个资源ID，暂时resid和id相同 [10/24/2011 Sun]
                if (Packet.City == 1)
                {

                    //CGameProcedure::s_pProcEnter->SetSceneID(nCitySceneID, nCityLevel);
                    GameProcedure.s_ProcEnter.SetSceneID(Packet.SceneID, Packet.ResID, Packet.CityLevel);
                }
                else
                {
                    //普通场景，第二个参数（城市等级）必须为-1
                    //CGameProcedure::s_pProcEnter->SetSceneID(pPacket->getSceneID(), -1);
                    GameProcedure.s_ProcEnter.SetSceneID(Packet.SceneID, Packet.ResID, -1);
                }
                //设置登录流程状态,使之进入下一个状态
                GameProcedure.s_ProcEnter.SetStatus(GamePro_Enter.ENTER_STATUS.ENTERSCENE_OK);
                //进入场景 
                GameProcedure.s_ProcEnter.EnterScene();
            }
            else
            {
                //不允许进入
                GameProcedure.s_ProcEnter.SetStatus(GamePro_Enter.ENTER_STATUS.ENTERSCENE_FAILED);
                //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO_CLOSE_NET, "进入场景的请求被服务器拒绝");	
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ENTERSCENE;
        }
    }
}
