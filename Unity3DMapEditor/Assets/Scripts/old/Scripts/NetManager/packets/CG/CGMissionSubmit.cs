using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGMissionSubmit : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONSUBMIT;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(int) + sizeof(uint) + sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_idNPC) != sizeof(uint)) return false;
            if (buff.ReadInt(ref m_idScript) != sizeof(int)) return false;
            if (buff.ReadUint(ref m_idSelectRadio) != sizeof(uint)) return false;
            if (buff.ReadByte(ref ucIgnoreStop) != sizeof(byte)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_idNPC);
            buff.WriteInt(m_idScript);
            buff.WriteUint(m_idSelectRadio);
            buff.WriteByte(ucIgnoreStop);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private uint    m_idNPC;
        private int     m_idScript;		    // 任务的脚本ID
        private uint    m_idSelectRadio;    // 多选一物品的选择ID
        private byte    ucIgnoreStop;       // 是否忽略停止,当等于UCHAR_MAX，则使得Human停下来

        public uint NPCID
        {
            get { return m_idNPC; }
            set { m_idNPC = value; }
        }
        public int ScriptID
        {
            get { return m_idScript; }
            set { m_idScript = value; }
        }
        public uint SelectRadio
        {
            get { return m_idSelectRadio; }
            set { m_idSelectRadio = value; }
        }
        public byte IgnoreStop
        {
            get { return ucIgnoreStop; }
            set { ucIgnoreStop = value; }
        }
    }

    public class CGMissionSubmitFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGMissionSubmit(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONSUBMIT; }
        public override int GetPacketMaxSize() { return sizeof(uint) + sizeof(int) + sizeof(uint) + sizeof(byte); }
    }
}