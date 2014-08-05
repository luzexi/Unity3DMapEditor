

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Network.Packets;

namespace Network.Handlers
{
    public class GCRelationHandler : HandlerBase
    {
    public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
    {

	    if(GameProcedure.GetActiveProcedure() == (GameProcedure)GameProcedure.s_ProcMain)
	    {
            GCRelation packet = (GCRelation) pPacket;
		    if(GameProcedure.s_pUISystem == null) return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

		    GC_RELATION pReturn = packet.GetRelation();
		    string szText = "其他好友操作。";

		    Relation pRelation = CDataPool.Instance.GetRelation();
		    if ( pRelation == null )
		    {
			    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
		    }
            
		    switch( (RELATION_RETURN_TYPE )pReturn.m_Type )
		    {
		    case RELATION_RETURN_TYPE.RET_RELATIONLIST:
			    {
				    GC_RELATIONLIST pRelationList = (GC_RELATIONLIST)pReturn.mRelation;

				    pRelation.SetMood( UIString.Instance.GetUnicodeString(pRelationList.GetMood()) );

				    for( int i=0; i<pRelationList.GetFriendCount(); ++i )
				    {
					    pRelation.AddRelation( pRelationList.GetFriend(i) );
				    }

				    for( int i=0; i<pRelationList.GetBlackCount(); ++i )
				    {
					    pRelation.AddRelation( pRelationList.GetBlackName(i) );
				    }

				    // 需要 push 一个事件通知 UI
                    pRelation.UpdateUIList(RELATION_GROUP.RELATION_GROUP_FRIEND_ALL);
				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_RELATIONINFO:
			    {
				    pRelation.UpdateRelationInfo( (RETURN_RELATION_INFO )pReturn.mRelation );
                    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_TEMPFRIEND_TO_FRIEND:
		    case RELATION_RETURN_TYPE.RET_TEMPFRIEND_ADDTO_BLACKLIST:
		    case RELATION_RETURN_TYPE.RET_ADDFRIEND:
		    case RELATION_RETURN_TYPE.RET_ADDTOBLACKLIST:
			    {
				    RETURN_ADD_RELATION pAddRelation =(RETURN_ADD_RELATION)pReturn.mRelation;
				    RELATION_GROUP Drg;
                    string msg = UIString.Instance.GetUnicodeString(pAddRelation.GetRelationData().GetName());
				    if ( pAddRelation.GetRelationType() == (byte)RELATION_TYPE.RELATION_TYPE_BLACKNAME )
				    {
    					szText = "你将"+msg+"加入黑名单。";
		//			    strTemp = NOCOLORMSGFUNC("GCRelationHandler_Info_Add_Black_List", pAddRelation.GetRelationData().GetName());
		//			    _snprintf(szText, _MAX_PATH, "%s", strTemp.c_str());
					    Drg = RELATION_GROUP.RELATION_GROUP_BLACK;
				    }
				    else if ( pAddRelation.GetRelationType() == (byte)RELATION_TYPE.RELATION_TYPE_FRIEND )
				    {

                        szText = "你将" + msg + "添加为好友。";
		//			    strTemp = NOCOLORMSGFUNC("GCRelationHandler_Info_Add_Firend_List", pAddRelation.GetRelationData().GetName());
		//			    _snprintf(szText, _MAX_PATH, "%s", strTemp.c_str());
					    Drg = (RELATION_GROUP)pAddRelation.GetGroup();
				    }
				    else
				    {
    					szText = "意外的关系类型："+pAddRelation.GetRelationType();
	//				    strTemp = NOCOLORMSGFUNC("GCRelationHandler_Info_Err_Relation", pAddRelation.GetRelationData().GetName());
	//				    _snprintf(szText, _MAX_PATH, "%s", strTemp.c_str());
	//				    AssertEx(FALSE, szText);
					    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
				    }

				    _RELATION pRelationData;
				    pRelationData = pAddRelation.GetRelationData();

				    // 如果是临时好友，则移除
				    pRelation.RemoveRelation( pRelationData.GetGUID() );
				    pRelation.RemoveRelation( RELATION_GROUP.RELATION_GROUP_TEMPFRIEND, UIString.Instance.GetUnicodeString(pRelationData.GetName()) );

				    SDATA_RELATION_MEMBER Member = new SDATA_RELATION_MEMBER();

				    Member.m_GUID = pRelationData.GetGUID();
				    Member.m_szName = UIString.Instance.GetUnicodeString( pRelationData.GetName() );
				    Member.m_RelationType = (RELATION_TYPE)pAddRelation.GetRelationType();
				    Member.m_nLevel = pRelationData.GetLevel();
				    Member.m_nMenPai = pRelationData.GetMenPai();
				    Member.m_nPortrait = pRelationData.GetPortrait();
				    Member.m_GuildID = pRelationData.GetGuildID();
				    Member.m_szGuildName = UIString.Instance.GetUnicodeString( pRelationData.GetGuildName() );
				    Member.m_bOnlineFlag = pRelationData.GetOnlineFlag() != 0;

				    if( Member.m_bOnlineFlag )
				    {
					    Member.m_szMood =  UIString.Instance.GetUnicodeString(pRelationData.GetMood());
					    Member.m_szTitle =  UIString.Instance.GetUnicodeString(pRelationData.GetTitle());
					    Member.m_SceneID = pRelationData.GetSceneID();
					    Member.m_nTeamSize = pRelationData.GetTeamSize();
				    }

				    pRelation.AddRelation( Drg, Member );

				    // 更新好友信息 [9/26/2011 Ivan edit]
				    CObject_Character player = (CObject_Character)(CObjectManager.Instance.FindCharacterByName(Member.m_szName));
				    if (player != null)
				    {
					    player.GetCharacterData().RefreshName();
				    }
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_TRANSITION:
			    {
// 				    RETURN_ADD_RELATION pAddRelation = (RETURN_ADD_RELATION)(pReturn.mRelation);
// 				    RELATION_GROUP Drg;
// 
// 				    // 将好友分组到黑名单才提示 [6/23/2011 edit by ZL]
// 				    if ( pAddRelation.GetRelationType() == (byte)RELATION_TYPE_BLACKNAME )
// 				    {
// 					    string name = UIString.Instance.GetUnicodeString(pAddRelation.GetRelationData().GetName());
//                         szText = "你将"+name+"加入黑名单";
// 					    Drg = (RELATION_GROUP)RELATION_GROUP_BLACK;
// 					    GameProcedure.s_pEventSystem.PushEvent( GAME_EVENT_ID.GE_INFO_SELF, szText );
// 				    }

				    RELATION_GUID_UCHAR_UCHAR pRelationInfo = (RELATION_GUID_UCHAR_UCHAR)(pReturn.mRelation);

				    RELATION_GROUP Srg = RELATION_GROUP.RELATION_GROUP_FRIEND_ALL;
				    int  idx = 0;

				    pRelation.GetRelationPosition( pRelationInfo.GetTargetGUID(), ref Srg, ref idx );
				    pRelation.MoveRelation( Srg, (RELATION_TYPE)pRelationInfo.GetRelationType(),
					    (RELATION_GROUP)pRelationInfo.GetGroup(), pRelationInfo.GetTargetGUID() );

				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_DELFRIEND:
		    case RELATION_RETURN_TYPE.RET_DELFROMBLACKLIST:
			    {
				    pRelation.RemoveRelation( ((RELATION_GUID)pReturn.mRelation).GetTargetGUID() );
				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_ADDFRIENDNOTIFY:
			    {
    //				_snprintf(szText, _MAX_PATH, "%s已经将你添加为好友了。", pReturn.m_NotifyFriend.GetName() );
//     				ADDTALKMSG(szText);
// 				    				    strTemp = NOCOLORMSGFUNC("GCRelationHandler_Info_Add_You_To_Firend_List", pReturn.m_NotifyFriend.GetName());
// 				    				    _snprintf(szText, _MAX_PATH, "%s", strTemp.c_str());
// 				    				    CGameProcedure::s_pEventSystem.PushEvent( GE_INFO_SELF, szText );
// 				    				    // 询问是否加对方为好友 [6/22/2011 edit by ZL]
// 				    				    CGameProcedure::s_pEventSystem.PushEvent( GE_FRIEND_MSG_CONFIRM, "201", pReturn.m_NotifyFriend.GetName(), pReturn.m_NotifyFriend.GetGUID() );
				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_ONLINELIST:
			    {
				    RETURN_ONLINE_LIST pRecv = (RETURN_ONLINE_LIST)(pReturn.mRelation);

				    for( byte i=0; i<pRecv.GetOnlineCount(); ++i )
				    {
					    pRelation.UpdateOnlineFriend( pRecv.GetOnlineRelation(i) );
				    }

				    //CObjectManager::GetMe().GetMySelf().GetCharacterData().Get_IsMinorPwdSetup(TRUE);

				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_RELATIONONLINE:
			    {
				    RETURN_NOTIFY_ONLINE pRecv = (RETURN_NOTIFY_ONLINE)(pReturn.mRelation);
				    pRelation.RelationOnline(UIString.Instance.GetUnicodeString( pRecv.GetTargetName()), UIString.Instance.GetUnicodeString(pRecv.GetMood()) );
				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_RELATIONOFFLINE:
			    {
				    pRelation.RelationOffLine( ((RELATION_GUID )pReturn.mRelation).GetTargetGUID() );
				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_NEWMOOD:
			    {
                    REQUEST_MODIFY_MOOD mood = (REQUEST_MODIFY_MOOD)pReturn.mRelation;
                    pRelation.SetMood((UIString.Instance.GetUnicodeString(mood.GetMood())));
				    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;;
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_ERR_PASSWDMISMATCH:
			    {
				    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_MINORPASSWORD_OPEN_UNLOCK_PASSWORD_DLG);
			    }
                break;
		    case RELATION_RETURN_TYPE.RET_ERR_TARGETNOTEXIST:
		    case RELATION_RETURN_TYPE.RET_ERR_GROUPISFULL:
		    case RELATION_RETURN_TYPE.RET_ERR_ISFRIEND:
		    case RELATION_RETURN_TYPE.RET_ERR_ISBLACKNAME:
		    case RELATION_RETURN_TYPE.RET_ERR_CANNOTTRANSITION:
		    case RELATION_RETURN_TYPE.RET_ERR_ISNOTFRIEND:
		    case RELATION_RETURN_TYPE.RET_ERR_ISNOTINBLACKLIST:
		    case RELATION_RETURN_TYPE.RET_ERR_SPOUSETOBLACKLIST:
		    case RELATION_RETURN_TYPE.RET_ERR_MASTERTOBLACKLIST:
		    case RELATION_RETURN_TYPE.RET_ERR_PRENTICETOBLACKLIST:
		    case RELATION_RETURN_TYPE.RET_ERR_BROTHERTOBLACKLIST:
		    case RELATION_RETURN_TYPE.RET_ERR_DELSPOUSE:
		    case RELATION_RETURN_TYPE.RET_ERR_DELMASTER:
		    case RELATION_RETURN_TYPE.RET_ERR_DELPRENTICE:
		    case RELATION_RETURN_TYPE.RET_ERR_DELBROTHER:
		    case RELATION_RETURN_TYPE.RET_ERR_CANNOT_ADDFRIEND:
		    case RELATION_RETURN_TYPE.RET_ERR_CANNOTRECEIVEMAIL:
		    case RELATION_RETURN_TYPE.RET_ERR_NOTRECVSTRANGEMAIL:
		    case RELATION_RETURN_TYPE.RET_ERR_RELATIONUNKNOWN:
			    {
// 				    STRING strTemp = "";
// 				    strTemp = NOCOLORMSGFUNC(pRelationString[ pReturn.m_Type - RET_ERR_START ]);
// 				    strncpy(szText, strTemp.c_str(), MAX_PATH-1);
// 				    //strncpy(szText, pRelationString[ pReturn.m_Type - RET_ERR_START ], MAX_PATH-1);
                     szText = "错误";
			    }
			    break;
		    case RELATION_RETURN_TYPE.RET_ERR_ISENEMY:
			    {
				    szText = "阵营错误";
			    }
			    break;
		    default :
			    return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;;
		    }

		    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, szText );
	    }

        return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE; ;
    }

        public override int GetPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_RELATION;
        }
    }
}

