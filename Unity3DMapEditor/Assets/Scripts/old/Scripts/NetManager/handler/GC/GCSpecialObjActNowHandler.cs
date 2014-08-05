using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;
namespace Network.Handlers
{
    public class GCSpecialObjActNowHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCSpecialObjActNow Packet = (GCSpecialObjActNow)pPacket;

            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                LogManager.Log("RECV GCSpecialObj_ActNowHandler");
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.ObjID);
                if (pObj != null)
                {
                    _ObjID_List listObjID	= Packet.ObjIDList;

		            SCommand_Object cmdTemp = new SCommand_Object();
                    cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_SPECIAL_OBJ_TRIGGER;
		            cmdTemp.SetValue<int>(0,Packet.LogicCount);
                    cmdTemp.SetValue<int>(1,listObjID.m_nCount);
                    cmdTemp.SetValue<object>(2,(object)(listObjID.m_aIDs));
		            pObj.PushCommand(cmdTemp );
                    pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
                }
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_SPECIAL_OBJ_ACT_NOW;
        }
    }
};
