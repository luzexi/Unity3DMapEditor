
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
      public class GCNewPetHandler : HandlerBase
      {
            public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase packet, ref Peer pPlayer)
            {
	            //当前流程是主流程
	            if(GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
	            {
                    GCNewPet pPacket = (GCNewPet)packet;
		            CObjectManager pObjectManager = CObjectManager.Instance;
		            //检查位置是否合法
                    fVector2 pos = new fVector2(pPacket.getWorldPos().m_fX, pPacket.getWorldPos().m_fZ);
		            if(!WorldManager.Instance.ActiveScene.IsValidPosition(ref pos))
		            {
			            LogManager.LogError("Valid Position @GCNewPetHandler.Execute");
			            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_ERROR;
		            }
		            //创建宠物
		            CObject_PlayerNPC pNPC = (CObject_PlayerNPC)pObjectManager.FindServerObject( (int)pPacket.getObjID() );

		            float fFaceDir = pPacket.getDir();
		            if(pNPC==null)
		            {
			            pNPC = pObjectManager.NewPlayerNPC( (int)pPacket.getObjID() );

			            SObjectInit initPlayerNPC = new SObjectInit();

			            initPlayerNPC.m_fvPos = new Vector3(pos.x, 0, pos.y);
			            initPlayerNPC.m_fvRot = new Vector3( 0.0f, fFaceDir, 0.0f );

			            pNPC.Initial( initPlayerNPC );
		            }
		            else
		            {
			            pNPC.SetMapPosition(pos.x, pos.y);
			            pNPC.SetFaceDir( fFaceDir );
                        pNPC.Enable((uint)ObjectStatusFlags.OSF_VISIABLE);
                        pNPC.Disalbe((uint)ObjectStatusFlags.OSF_OUT_VISUAL_FIELD);
		            }

		            pNPC.SetNpcType(ENUM_NPC_TYPE.NPC_TYPE_PET);
		            pNPC.GetCharacterData().Set_MoveSpeed(pPacket.getMoveSpeed());

                    SCommand_Object cmdTemp = new SCommand_Object();
                    cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_IDLE;
		            cmdTemp.SetValue<float>(0,pos.x);
		            cmdTemp.SetValue<float>(1, pos.y);
		            cmdTemp.SetValue<bool>(2,false);	
		            pNPC.PushCommand(cmdTemp );

		            //放入Ask队列
		            pObjectManager.LoadQueue.TryAddLoadTask(pNPC.ID);


		            //同步渲染层
		            string szTemp = "GCNewPet("+pos.x.ToString()+","+pos.y.ToString() + ")";
		            pNPC.PushDebugString(szTemp);
                    pNPC.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());

	            }

	        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE; ;
            }

            public override int GetPacketID()
            {
                return (int)PACKET_DEFINE.PACKET_GC_NEWPET;
            }
      }


}

