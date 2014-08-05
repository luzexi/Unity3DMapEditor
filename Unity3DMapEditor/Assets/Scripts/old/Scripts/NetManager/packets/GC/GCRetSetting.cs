
using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCRetSetting : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            for (int i = 0; i < (int)SETTING_TYPE.SETTING_TYPE_NUMBER; i++ )
            {
                m_aSetting[i].readFromBuff(ref buff);
            }

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //todo
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_RETSETTING;
        }
        public override int getSize()
        {
            return _OWN_SETTING.getMaxSize() * (int)SETTING_TYPE.SETTING_TYPE_NUMBER;
        }

        //public interface

        public _OWN_SETTING[] OwnSetting
        {
            get { return m_aSetting; }
            set { m_aSetting = OwnSetting; }
        }

        //数据
		private _OWN_SETTING[]	m_aSetting = new _OWN_SETTING[(int)SETTING_TYPE.SETTING_TYPE_NUMBER] ;

    };
    public class GCRetSettingFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRetSetting(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_RETSETTING; }
        public override int GetPacketMaxSize()
        {
            return _OWN_SETTING.getMaxSize() * (int)SETTING_TYPE.SETTING_TYPE_NUMBER;
        }
    };
}