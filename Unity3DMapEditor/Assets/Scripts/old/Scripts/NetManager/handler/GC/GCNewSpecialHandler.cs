using System;
using System.Collections.Generic;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCNewSpecialHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCNewSpecial Packet = (GCNewSpecial)pPacket;
                //LogManager.Log("Receive GCNewSpecial Packet ObjID:" + Packet.ObjID);
                fVector2 pos = new fVector2(Packet.posWorld.m_fX,Packet.posWorld.m_fZ);
                if(!WorldManager.Instance.ActiveScene.IsValidPosition(ref pos))
                {
                    LogManager.LogError("ERROR POSITION @ GCNewSpecialHandler");
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

		        CObject_Special pSpecial = (CObject_Special)CObjectManager.Instance.FindServerObject(Packet.ObjID);
		        if ( pSpecial == null )
		        {
			        //创建平台生长点
                    pSpecial = (CObject_Special)CObjectManager.Instance.NewSpecialObject(Packet.ObjID);
		        }
		        else
		        {
			        pSpecial.Release();
		        }

		        SObject_SpecialInit initSpecial = new SObject_SpecialInit();
                initSpecial.m_fvPos.x = Packet.posWorld.m_fX;
                initSpecial.m_fvPos.z = Packet.posWorld.m_fZ;
		        initSpecial.m_fvPos.y	= 0.0f;
		        initSpecial.m_fvRot		= new Vector3( 0.0f, Packet.Dir, 0.0f );
		        initSpecial.m_nDataID	= Packet.DataID;
		        pSpecial.Initial(initSpecial );
		        pSpecial.SetMapPosition(Packet.posWorld.m_fX, Packet.posWorld.m_fZ);
		        pSpecial.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());

            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_NEWSPECIAL;
        }
    }
}