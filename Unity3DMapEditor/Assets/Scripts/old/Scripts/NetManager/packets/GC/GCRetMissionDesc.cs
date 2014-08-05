using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCRetMissionDesc : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_MISSIONDESC;
        }

        public override int getSize()
        {
            return sizeof(int) + sizeof(byte) * (3 + 260 * 3);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_nMissionIndex) != sizeof(int))
            {
                return false;
            }
            if (buff.ReadByte(ref m_nMissionName) != sizeof(byte))
            {
                return false;
            }
            for (int i = 0; i < 260; i++)
            {
                if (buff.ReadByte(ref m_szMissionName[i]) != sizeof(byte))
                {
                    return false;
                }
            }
            if (buff.ReadByte(ref m_nMissionTarget) != sizeof(byte))
            {
                return false;
            }
            for (int i = 0; i < 260; i++)
            {
                if (buff.ReadByte(ref m_szMissionTarget[i]) != sizeof(byte))
                {
                    return false;
                }
            }
            if (buff.ReadByte(ref m_nMissionDesc) != sizeof(byte))
            {
                return false;
            }
            for (int i = 0; i < 260; i++)
            {
                if (buff.ReadByte(ref m_szMissionDesc[i]) != sizeof(byte))
                {
                    return false;
                }
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_nMissionIndex);
            buff.WriteByte(m_nMissionName);
            buff.Write(ref m_szMissionName, 260);
            buff.WriteByte(m_nMissionTarget);
            buff.Write(ref m_szMissionTarget, 260);
            buff.WriteByte(m_nMissionDesc);
            buff.Write(ref m_szMissionDesc, 260);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        private int m_nMissionIndex;
        private byte m_nMissionName;
        private byte[] m_szMissionName = new byte[260];
        private byte m_nMissionTarget;
        private byte[] m_szMissionTarget = new byte[260];
        private byte m_nMissionDesc;
        private byte[] m_szMissionDesc = new byte[260];

        public int GetMissionIndex() { return m_nMissionIndex; }
        public string GetMissionName() { return EncodeUtility.Instance.GetUnicodeString(m_szMissionName); }
        public string GetMissionTarget() { return EncodeUtility.Instance.GetUnicodeString(m_szMissionTarget); }
        public string GetMissionDesc() { return EncodeUtility.Instance.GetUnicodeString(m_szMissionDesc); }
        public string GetMissionNameLen() { return Convert.ToString(m_nMissionName); }
        public string GetMissionDescLen() { return Convert.ToString(m_nMissionDesc); }
        public string GetMissionTargetLen() { return Convert.ToString(m_nMissionTarget); }
    }

    public class GCRetMissionDescFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRetMissionDesc(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_MISSIONDESC; }
        public override int GetPacketMaxSize() { return sizeof(int) + sizeof(byte) * (3 + 260 * 3); }
    }
}