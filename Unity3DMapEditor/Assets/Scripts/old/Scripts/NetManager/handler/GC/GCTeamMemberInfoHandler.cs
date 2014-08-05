using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCTeamMemberInfoHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
            {
                GCTeamMemberInfo packet = pPacket as GCTeamMemberInfo;
                CUIDataPool pDataPool = (GameProcedure.s_pUIDataPool);
                CTeamOrGroup team = pDataPool.GetTeamOrGroup();
                TeamMemberInfo pTMInfo = team.GetMember(packet.GUID);

                if (pTMInfo == null)
                { // 组队切换场景时会遇到这个问题
                    // TDAssert(FALSE);
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_FAMILY))
                {
                    pTMInfo.m_uFamily = (int)packet.Family;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_LEVEL))
                {
                    pTMInfo.m_uLevel = (int)packet.Level;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_POSITION))
                {
                    GameProcedure.m_bWaitNeedFreshMinimap = true;
                    pTMInfo.m_WorldPos.m_fX = packet.WorldPos.m_fX;
                    pTMInfo.m_WorldPos.m_fZ = packet.WorldPos.m_fZ;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HP))
                {
                    pTMInfo.m_nHP = packet.HP;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MAX_HP))
                {
                    pTMInfo.m_dwMaxHP = (int)packet.MaxHP;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MP))
                {
                    pTMInfo.m_nMP = packet.MP;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MAX_MP))
                {
                    pTMInfo.m_dwMaxMP = (int)packet.MaxMP;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_ANGER))
                {
                    pTMInfo.m_nAnger = packet.Anger;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_WEAPON))
                {
                    // 武器id
                    pTMInfo.m_WeaponID = (int)packet.WeaponID;

                    // 设置ui模型
                    pTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_WEAPON, pTMInfo.m_WeaponID);

                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_CAP))
                {
                    // 帽子id
                    pTMInfo.m_CapID = (int)packet.CapID;

                    // 设置ui模型
                    pTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_CAP, pTMInfo.m_CapID);
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_ARMOR))
                {
                    // 身体
                    pTMInfo.m_ArmourID = (int)packet.ArmourID;

                    // 设置ui模型
                    pTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_ARMOR, pTMInfo.m_ArmourID);
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_CUFF))
                {
                    // 腕
                    pTMInfo.m_CuffID = (int)packet.CuffID;

                    // 设置ui模型
                    pTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_CUFF, pTMInfo.m_CuffID);
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_BOOT))
                {
                    // 脚
                    pTMInfo.m_FootID = (int)packet.FootID;

                    // 设置ui模型
                    pTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_BOOT, pTMInfo.m_FootID);
                }

                /*
                if( pPacket.IsUpdateAttrib(TEAM_MEMBER_ATT_BUFF) )
                {
                }
                */

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_DEADLINK))
                {
                    pTMInfo.m_bDeadLink = packet.DeadLinkFlag > 0;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_DEAD))
                {
                    pTMInfo.m_bDead = packet.DeadFlag > 0;
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_FACEMESH))
                {
                    pTMInfo.m_uFaceMeshID = (int)packet.FaceMeshID;
                    if (pTMInfo.m_uFaceMeshID < 255)
                    {
                        // 设置脸形
                        pTMInfo.m_UIModel.SetFaceMeshId(pTMInfo.m_uFaceMeshID);
                    }
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HAIRMESH))
                {
                    pTMInfo.m_uHairMeshID = (int)packet.HairMeshID;

                    if (pTMInfo.m_uHairMeshID < 255)
                    {
                        // 设置发型
                        pTMInfo.m_UIModel.SetFaceHairId(pTMInfo.m_uHairMeshID);
                    }
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HAIRCOLOR))
                {
                    pTMInfo.m_uHairColor = packet.HairColor;

                    //if(pTMInfo.m_uHairColor < 255)
                    //{
                    //	// 设置颜色
                    //	pTMInfo.m_UIModel.SetHairColor(pTMInfo.m_uHairColor);
                    //}
                    //else
                    //{
                    //	// 设置颜色
                    //	pTMInfo.m_UIModel.SetHairColor(0);
                    //}//

                    // 设置颜色
                    pTMInfo.m_UIModel.SetHairColor(pTMInfo.m_uHairColor);
                }

                if (packet.IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_BACK))
                {
                    // 背饰
                    pTMInfo.m_uBackID = (int)packet.BackID;

                    // 设置ui模型
                    pTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_BACK, pTMInfo.m_uBackID);
                }

                int idx = team.GetMemberUIIndex(pTMInfo.m_GUID);
                GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_UPTEDATA_MEMBER_INFO, idx);
                for (idx = 0; idx < team.GetTeamMemberCount(); ++idx)
                {
                    if (team.GetMemberByIndex(idx).m_GUID == pTMInfo.m_GUID)
                    {
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_REFRESH_MEMBER, idx);
                    }
                }

            }
            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_TEAMMEMBERINFO;
        }
    }
}