using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCRetSettingHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            GCRetSetting packet = pPacket as GCRetSetting;

	        //需要重写--Ivan
	        //SystemSetup::GetMe()->Set_GameData(pSetting[SETTING_TYPE_GAME].m_SettingData);
	
	        for(int i= (int)SETTING_TYPE.SETTING_TYPE_K0;i < (int)SETTING_TYPE.SETTING_TYPE_K19+1; i++)
	        {
		        CActionSystem.Instance.MainMenuBar_SetID(i-(int)SETTING_TYPE.SETTING_TYPE_K0,packet.OwnSetting[i].m_SettingType,packet.OwnSetting[i].m_SettingData);
	        }
	        // 为了防止刷新过多导致出错,在此处刷新快捷栏 [1/13/2011 ivan edit]
	        CActionSystem.Instance.UpdateToolBar();

	        //Talk::s_Talk.LoadTabSetting(pPacket);

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_RETSETTING;
        }
    }
}