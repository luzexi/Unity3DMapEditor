using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCAbilitySuccHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCAbilitySucc packet = pPacket as GCAbilitySucc;
                bool bLog = (GameProcedure.s_pUISystem != null) ? true : false;
                //生成一个临时的道具
                CObject_Item pItem = null;

                if (packet.ObjectID != MacroDefine.UINT_MAX)
                {
                    if (bLog)
                        pItem = ObjectSystem.Instance.NewItem(packet.ObjectID);
                }

                //如果不是钓鱼成功，就隐藏进度条。
                if (packet.AbilityID != 9)
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_PROGRESSBAR_HIDE);

                ////判断是哪种技能，播放相应声音
                //switch(packet.AbilityID)
                //{
                //case 9://11
                //    CSoundSystemFMod::_PlayUISoundFunc(11+59);
                //    break;
                //case 8://13
                //    CSoundSystemFMod::_PlayUISoundFunc(13+59);
                //    break;
                //case 10://15
                //    CSoundSystemFMod::_PlayUISoundFunc(15+59);
                //    break;
                //    //19
                //case 4:			//铸剑
                //case 5:			//制衣
                //case 6:			//艺术制作
                //case 7:			//工程学
                //    CSoundSystemFMod::_PlayUISoundFunc(19+59);
                //    break;
                //case 3:	//23 //21
                //    CSoundSystemFMod::_PlayUISoundFunc(23+59);
                //    break;
                //default :
                //    break;
                //}

                if (bLog)
                {
                    string strTemp = "";
                    if (packet.AbilityID == 3)
                    {
                        // 				ADDTALKMSG("你成功的镶嵌了一颗宝石");
                        // 				CGameProcedure::s_pEventSystem->PushEvent( GE_INFO_SELF, "镶嵌成功");
                    }
                    else if (pItem != null)
                    {
                        //strTemp = COLORMSGFUNC("CREATE_ONE_ITEM", pItem->GetName());
                        //ADDTALKMSG(strTemp);
                        //CObject_Item::DestroyItem(pItem);
                        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "获得 " + pItem.GetName());
                    }
                    // 因为使用技能都会触发这个条件，暂时屏蔽 [1/11/2011 ivan edit]
                    // 			else if( pPacket->GetPrescriptionID() == INVALID_ID )// != --> == [11/15/2010 Sun]
                    // 			{ // 如果是配方合成失败
                    // 				strTemp = "合成失败";//COLORMSGFUNC("ability_create_failed");
                    // 				ADDTALKMSG(strTemp);
                    // 			}
                }

                // 必须检测，上面逻辑会有泄露内存可能 [10/26/2011 Ivan edit]
                if (pItem != null)
                {
                    ObjectSystem.Instance.DestroyItem(pItem);
                }

                //GameProcedure.s_pGameInterface.Player_UseLifeAbility(packet.PrescriptionID);
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_ABILITY_SUCC;
        }
    }
}