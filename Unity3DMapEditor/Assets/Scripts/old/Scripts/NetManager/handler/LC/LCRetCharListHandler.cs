using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{

    /// <summary>
    /// CLAskCharListHandler
    /// </summary>
    public class LCRetCharListHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            LCRetCharList msg = (LCRetCharList)pPacket;
            //LogManager.Log("Receive LCRetCharList" );
            if (msg != null && (msg.Result == NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_SUCCESS))
            {

                int iCharacterCount = msg.CharNumber;
                try
                {
                    GameProcedure.s_ProcCharSel.ClearUIModel();//清除已存在的角色  [8/29/2011 zzy]
                    GameProcedure.s_ProcCharSel.m_bClearUIModel = false;
                    if (iCharacterCount > 0)
                    {
                        DB_CHAR_BASE_INFO pInfo = new DB_CHAR_BASE_INFO();
                        //LogManager.Log("Create DB_CHAR_BASE_INFO");
                        for (int i = 0; i < iCharacterCount; i++)
                        {
                            pInfo = msg.GetCharBaseInfo(i);
                            //LogManager.Log("Get DB_CHAR_BASE_INFO");
                            // 添加一个角色
                            GameProcedure.s_ProcCharSel.AddCharacter(ref pInfo);
                            //LogManager.Log("algin DB_CHAR_BASE_INFO");

                            GameProcedure.s_ProcCharCreate.m_iCamp = pInfo.m_Camp;

                            string name = Encoding.Default.GetString(pInfo.m_Name);

                            //LogManager.Log("name:" + name);
                            //LogManager.Log("name:" + name);

                        }

                    }
                    else
                    {
                        GameProcedure.s_ProcCharCreate.m_iCamp = 0;
                    }
                }
                catch (Exception er)
                {
                    LogManager.Log(er.ToString());
                }
                LogManager.Log("Receive char count: " + iCharacterCount);

                GameProcedure.s_ProcCharSel.resetSel();
                GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_CLOSE_SYSTEM_INFO);
                GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR);	


                // 转换到人物选择界面。
                if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcCharCreate)
                {
                    if (GameProcedure.s_ProcCharCreate.Status == GamePro_CharCreate.PLAYER_CHAR_CREATE_STATUS.CHAR_CREATE_CREATE_OK)
                    {
                        GameProcedure.s_ProcCharCreate.ChangeToRoleSel();
                    }
                }


            }
            else
            {
                NET_RESULT_DEFINE.ASKCHARLIST_RESULT ListResult;
                ListResult = msg.Result;

                switch (ListResult)
                {
                    case NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_OP_FAILS://操作失败
                        {
                            LogManager.Log("得到角色信息出错");
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "得到角色信息出错! ");	
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_SERVER_BUSY://服务器忙，重试
                        {
                            LogManager.Log("得到角色信息出错! 服务器繁忙.");
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "得到角色信息出错! 服务器繁忙.");	
                            break;
                        }
                    case NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_OP_TIMES://操作过于频繁
                        {
                            LogManager.Log("得到角色信息出错! 操作过于频繁.");
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "得到角色信息出错! 操作过于频繁.");	
                            break;
                        }
                    default:
                        {
                            LogManager.Log("未知错误");
                            //CGameProcedure::s_pEventSystem->PushEvent( GE_GAMELOGIN_SHOW_SYSTEM_INFO, "未知错误!");	
                            break;
                        }
                }

            }
            //LogManager.Log("Receive Char List: Result" + msg.Result);

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_LC_RETCHARLIST;
        }
    }
}