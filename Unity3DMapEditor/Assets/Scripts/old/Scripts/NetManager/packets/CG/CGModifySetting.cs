using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGModifySetting : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MODIFYSETTING;
        }

        public override int getSize()
        {
            return sizeof(byte) + m_Value.getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteByte(m_Type);
            m_Value.writeToBuff(ref buff);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public byte Type
        {
            set { m_Type = value; }
        }
        public _OWN_SETTING Value
        {
            set { m_Value = value; }
            get { return m_Value; }
        }

        private byte m_Type;//enum SETTING_TYPE

        private _OWN_SETTING m_Value;
    }

    public class CGModifySettingFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGModifySetting(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MODIFYSETTING; }
        public override int GetPacketMaxSize() { return sizeof(byte) + _OWN_SETTING.getMaxSize(); }
    }
}