using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCNewBusHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
	        if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	        {
		        //检查位置是否合法
                //if(!CWorldManager::GetMe()->GetActiveScene()->IsValidPosition(fVector2(pPacket->GetCurPos()->m_fX, pPacket->GetCurPos()->m_fZ)))
                //{
                //    TDThrow("ERROR POSITION @ GCNewBusHandler");
                //}
                GCNewBus busPacket = pPacket as GCNewBus;
                if (busPacket == null)
                {
                    LogManager.LogError("GCNewBus 收包错误。");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
                }

		        //创建玩家
			    CObject pBus = CObjectManager.Instance.FindServerObject((int)busPacket.ObjID);
                Vector3 fvGame = new Vector3(busPacket.PosWorld.m_fX,0,busPacket.PosWorld.m_fZ);
		        if(pBus == null)
		        {
			        pBus = CObjectManager.Instance.NewBus((int)busPacket.ObjID);

			        SObject_BusInit tBusInit = new SObject_BusInit();
			        tBusInit.m_fvPos	= fvGame;
			        tBusInit.m_fvRot	= new Vector3(0.0f, busPacket.Dir, 0.0f);
			        tBusInit.m_nDataID	= busPacket.DataID;
			        pBus.Initial(tBusInit);

			        pBus.SetMapPosition(busPacket.PosWorld.m_fX,busPacket.PosWorld.m_fZ);
		        }
		        else
		        {
			        if(pBus.isVisible())
			        {
				        if(Math.Abs(pBus.GetPosition().x - fvGame.x) + Math.Abs(pBus.GetPosition().z - fvGame.z) > 
                            ObjectDef.DEF_CHARACTER_POS_ADJUST_DIST)
				        {
					        pBus.SetMapPosition(fvGame.x, fvGame.z);
				        }
			        }
			        else
			        {
					    pBus.SetMapPosition(fvGame.x, fvGame.z);
				        pBus.SetFaceDir(busPacket.Dir);
				        //pBus->setVisible(true);
			        }
		        }

		        SCommand_Object cmdTemp = new SCommand_Object();
		        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_BUS_ADD_PASSENGER;
		        int i;
		        for(i = 0; i < busPacket.PassengerCount; i++)
		        {
			        cmdTemp.SetValue(0,i);
			        cmdTemp.SetValue(1,busPacket.PassengerIDs[i]);
			        pBus.PushCommand(cmdTemp);
		        }

		        //放入Ask队列
                GameProcedure.s_pObjectManager.LoadQueue.TryAddLoadTask(pBus.ID);
	        }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWBUS;
        }
    }
}