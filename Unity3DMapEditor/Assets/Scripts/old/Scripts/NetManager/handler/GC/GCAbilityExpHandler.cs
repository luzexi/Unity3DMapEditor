using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCAbilityExpHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCAbilityExp packet = pPacket as GCAbilityExp;
                CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_LifeAbility(packet.AbilityID, -1, packet.Exp);

        ////添加提示信息
        //BOOL bLog = (CGameProcedure::s_pUISystem != NULL)?TRUE:FALSE;
        //if(bLog && CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_LifeAbility(pPacket->getAbilityId()))
        //{
        //    LPCTSTR pAbilityName = CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_LifeAbility(
        //        pPacket->getAbilityId())->m_pDefine->szName;
        //    UINT exp = pPacket->getExp()/100;
        //    CHAR szText[_MAX_PATH];
        //    _snprintf( szText, _MAX_PATH, "你的#cCCFFCC%s#W技能的熟练度增加到#R%d#W", pAbilityName, exp );
        //    ADDTALKMSG(szText);
        //}
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ABILITYEXP;
        }
    }
}