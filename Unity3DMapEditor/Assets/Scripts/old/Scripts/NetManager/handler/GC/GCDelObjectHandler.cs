using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCDelObjectHandler : HandlerBase
    {
	    public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
	        //当前流程是主流程
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
		        CObjectManager pObjectManager = CObjectManager.Instance;
                GCDelObject delObjPacket = (GCDelObject)pPacket;
		        //判断是否是本场景
		        // 将场景ID和资源ID分开，不再使用资源ID代替场景ID [10/24/2011 Sun]
		        if(WorldManager.Instance.GetActiveSceneID() != delObjPacket.getSceneID())
		        {
			        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		        }

		        //寻找obj
                CObject pDelObj = pObjectManager.FindServerObject((int)delObjPacket.getObjID());
                CObject_Character pObj = pDelObj as CObject_Character;
                LogManager.Log("GCDelObjectHandler ObjID:" + delObjPacket.getObjID());
                if ( pObj == null )
                {
			         CObjectManager.Instance.DestroyObject(pDelObj);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                if(CObjectManager.Instance.GetMainTarget() != null && pObj == CObjectManager.Instance.GetMainTarget())
                {
                    CObjectManager.Instance.SetMainTarget(-1,CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_OBJECT);
                }
                 // 判断obj是否在载具上面，在的话需要删除掉 [8/29/2011 Ivan edit] //todo
                if(pObj.GetCharacterType() == CHARACTER_TYPE.CT_PLAYEROTHER ||
                    pObj.GetCharacterType() == CHARACTER_TYPE.CT_PLAYERMYSELF) 
                {
                    if(pObj.GetCharacterData().Get_BusObjID() != MacroDefine.UINT_MAX)
                    {
                        CObject pBus = CObjectManager.Instance.FindServerObject((int)pObj.GetCharacterData().Get_BusObjID());
 				        if (pBus != null)
 				        {
 					        SCommand_Object cmdTemp = new SCommand_Object();
 					        cmdTemp.m_wID			= (int)OBJECTCOMMANDDEF.OC_BUS_REMOVE_PASSENGER;
 					        cmdTemp.SetValue<int>(0,pObj.ServerID);
 					        pBus.PushCommand(cmdTemp);
 				        }
                    }
                }

                //删除对象
                CObjectManager.Instance.DestroyObject(pObj);
		        //之后不能执行任何代码
	        }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DELOBJECT;
        }
    };


}

