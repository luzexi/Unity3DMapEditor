using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCCharMoveResultHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                //LogManager.Log("Receive GCCharMoveResult Packet");
                GCCharMoveResult charMovePacket = (GCCharMoveResult)pPacket;
          
		        if(charMovePacket.Result < 0)
		        {
			        if( charMovePacket.Result ==(int) OPERATE_RESULT.OR_CUT_PATHROUTE )
			        {
				
				        byte numPos = charMovePacket.NumTargetPos;
                        WORLD_POS[] targetPos = charMovePacket.TargetPos; 
				        if( numPos > 0 )
				        {
                            if (CObjectManager.Instance.getPlayerMySelf().ModifyMoveCommand(charMovePacket.HandleID, numPos, targetPos) == false)
                            {
                                CObjectManager.Instance.getPlayerMySelf().OnMoveStop();

                                SCommand_Object cmdTemp = new SCommand_Object();
                                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_TELEPORT;
                                cmdTemp.SetValue<float>(0, targetPos[numPos - 1].m_fX);
                                cmdTemp.SetValue<float>(1, targetPos[numPos - 1].m_fZ);

                                CObjectManager.Instance.getPlayerMySelf().PushCommand(cmdTemp);

                                //////////////////////////////////////////////////////////////////////////
                                // 注意，这里被强制传送了 [3/23/2012 Ivan]
                                LogManager.LogWarning("强制传送 x:" + targetPos[numPos - 1].m_fX +
                                                        " z:" + targetPos[numPos - 1].m_fZ);
                                LogManager.LogWarning("注意！如果此时寻路不正常的话，很有可能是服务器和客户端的导航数据不一致，被强制传送了");
                                //////////////////////////////////////////////////////////////////////////
                            }

                            CObjectManager.Instance.getPlayerMySelf().StopLogic();
				        }
				        else
				        {
					        CObjectManager.Instance.getPlayerMySelf().OnMoveStop( );
				        }
			        }
			        else
			        {
				        CObjectManager.Instance.getPlayerMySelf().OnMoveStop( );
			        }
		        }


            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_CHARMOVERESULT;
        }
    }
}