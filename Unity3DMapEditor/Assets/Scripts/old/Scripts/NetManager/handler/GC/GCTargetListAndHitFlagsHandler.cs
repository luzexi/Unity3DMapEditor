using System;
using System.Collections.Generic;

using Network.Packets;
using DBSystem;
namespace Network.Handlers
{
    public class GCTargetListAndHitFlagsHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
               
                GCTargetListAndHitFlags Packet = (GCTargetListAndHitFlags)pPacket;
                CObject pObj = CObjectManager.Instance.FindServerObject(Packet.ObjID);
                LogManager.Log("Receive GCTargetListAndHitFlags Packet " + Packet.ObjID + " BulletID: " + Packet.SkillOrSpecialObjDataID + " TargetID: " + Packet.TargetID);
                if (pObj == null)
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

                string pszSenderLocator = "";
                bool bHitCharacter = false;
                int nBulletID = MacroDefine.INVALID_ID;
                if ((byte)GCTargetListAndHitFlags.TARGETTYPE.SPECIAL_OBJ_ACTIVATE == Packet.DataType)
                {
                    _DBC_SPECIAL_OBJ_DATA pSpecialObjData = CDataBaseSystem.Instance.GetDataBase<_DBC_SPECIAL_OBJ_DATA>((int)DataBaseStruct.DBC_SPECIAL_OBJ_DATA).Search_Index_EQU((int)Packet.SkillOrSpecialObjDataID);
                    if (pSpecialObjData != null)
                    {
                        nBulletID = pSpecialObjData.m_nBulletID;
                        bHitCharacter = true;
                        pszSenderLocator = "";
                    }
                }
                else
                {
                    _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)Packet.SkillOrSpecialObjDataID);
                    if (skillData != null)
                    {
                        nBulletID = skillData.m_nBulletID;
                        pszSenderLocator = skillData.m_pszBulletSendLocator;
                        switch ((ENUM_SELECT_TYPE)skillData.m_nSelectType)
                        {
                            case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
                            case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
                                bHitCharacter = false;
                                break;
                            case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
                            case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
                            case ENUM_SELECT_TYPE.SELECT_TYPE_SELF:
                            default:
                                bHitCharacter = true;
                                break;
                        }
                    }
                }

                if (nBulletID != MacroDefine.INVALID_ID)
                {
                    _LOGIC_EVENT_BULLET infoBullet = null;
                    if (bHitCharacter)
                    {

                        CObject_Character pCharacter = (CObject_Character)pObj;
                        // CObject_Special   pSpecial = (CObject_Special)pObj;
                        if (pCharacter != null)
                        {
                            for (byte i = 0; i < Packet.TargetNum; i++)
                            {
                                infoBullet = new _LOGIC_EVENT_BULLET();
                                infoBullet.m_nBulletID = nBulletID;
                                infoBullet.m_pszSenderLocator = pszSenderLocator;
                                infoBullet.m_bHitTargetObj = bHitCharacter;
                                _LOGIC_EVENT logicEvent = new _LOGIC_EVENT();
                                logicEvent.Init((uint)Packet.ObjID, Packet.LogicCount, infoBullet);
                                SCommand_Object cmdTemp = new SCommand_Object();
                                cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_LOGIC_EVENT;
                                logicEvent.m_bullet.m_nTargetID = (uint)Packet.TargetList[i];
                                cmdTemp.SetValue<object>(0, (object)logicEvent);
                                pCharacter.PushCommand(cmdTemp);
                            }
                        }
                        //else if(pSpecial != null)
                        //{

                        //}
                    }
                    else
                    {
                        infoBullet = new _LOGIC_EVENT_BULLET();
                        infoBullet.m_nBulletID = nBulletID;
                        infoBullet.m_pszSenderLocator = pszSenderLocator;
                        infoBullet.m_bHitTargetObj = bHitCharacter;

                        infoBullet.m_fTargetX = Packet.PosTarget.m_fX;
                        infoBullet.m_fTargetZ = Packet.PosTarget.m_fZ;
                        _LOGIC_EVENT logicEvent = new _LOGIC_EVENT();
                        logicEvent.Init((uint)Packet.ObjID, Packet.LogicCount, infoBullet);
                        SCommand_Object cmdTemp = new SCommand_Object();
                        cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_LOGIC_EVENT;
                        cmdTemp.SetValue<object>(0, (object)logicEvent);

                        CObject_Character pCharacter = (CObject_Character)pObj;
                        // CObject_Special   pSpecial   = (CObject_Special)pObj;
                        if (pCharacter != null)
                        {
                            pCharacter.PushCommand(cmdTemp);
                        }
                        //  else if(pSpecial != null)
                        // {
                        // CObject_Special *pSpecial = (CObject_Special*)pObj;
                        // }
                    }
                }
                pObj.SetMsgTime(GameProcedure.s_pTimeSystem.GetTimeNow());
            }
		    
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_TARGET_LIST_AND_HIT_FLAGS;
        }
    }
}