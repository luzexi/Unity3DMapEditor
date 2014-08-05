using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCDetailHealsAndDamagesHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                LogManager.Log("Receive GCDetailHealsAndDamages Packet");
                GCDetailHealsAndDamages Packet = (GCDetailHealsAndDamages)pPacket;
                CObject pObj = CObjectManager.Instance.FindServerObject((int)Packet.RecieverID);
                if(pObj!= null)
                {
                    _DAMAGE_INFO infoDamage = new _DAMAGE_INFO();
                    infoDamage.m_nSkillID = Packet.SkillID;

                    if (infoDamage.m_nSkillID != MacroDefine.INVALID_ID)
				    {
                        _DBC_SKILL_DATA skillData = CSkillDataMgr.Instance.GetSkillData((uint)infoDamage.m_nSkillID);
					    if(skillData != null)
					    {
                            infoDamage.m_nBulletID = skillData.m_nBulletID;
					    }
				    }

                    infoDamage.m_nTargetID = (uint)Packet.RecieverID;
                    infoDamage.m_nSenderID = (uint)Packet.SenderID;
                    infoDamage.m_nSenderLogicCount = Packet.SenderLogicCount;
                    infoDamage.m_nImpactID = MacroDefine.INVALID_ID;
                    infoDamage.m_nType = _DAMAGE_INFO.DAMAGETYPE.TYPE_HEAL_AND_DAMAGE;
                    if (Packet.IsHpModificationDirty())
                    {
                        infoDamage.m_bHealthDirty = true;
                        infoDamage.m_nHealthIncrement = Packet.HPModification;
                    }
                    if (Packet.IsMpModificationDirty())
                    {
                        infoDamage.m_bManaDirty = true;
                        infoDamage.m_nManaIncrement = Packet.MPModification;
                    }
                    if (Packet.IsRageModificationDirty())
                    {
                        infoDamage.m_bRageDirty = true;
                        infoDamage.m_nRageIncrement = Packet.RageModification;
                    }
                    if (Packet.IsStrikePointModificationDirty())
                    {
                        infoDamage.m_bStrikePointDirty = true;
                        infoDamage.m_nStrikePointIncrement = Packet.StrikePointModification;
                    }
                    infoDamage.m_bIsCriticalHit = Packet.CriticalHit;

                    _LOGIC_EVENT logicEvent = new _LOGIC_EVENT();
                    logicEvent.Init((uint)Packet.SenderID, Packet.SenderLogicCount,infoDamage);

                    SCommand_Object cmdTemp = new SCommand_Object();
                    cmdTemp.m_wID = (int)OBJECTCOMMANDDEF.OC_LOGIC_EVENT;
                    cmdTemp.SetValue<object>(0, logicEvent);
                    pObj.PushCommand(cmdTemp);
                }
                
            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_DETAIL_HEALS_AND_DAMAGES;
        }
    }
}