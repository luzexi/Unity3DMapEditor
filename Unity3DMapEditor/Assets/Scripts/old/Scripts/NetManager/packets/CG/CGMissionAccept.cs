using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGMissionAccept : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONACCEPT;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(int) + sizeof(byte);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_idNPC) != sizeof(uint)) return false;
            if (buff.ReadInt(ref m_idScript) != sizeof(int)) return false;
            if (buff.ReadByte(ref ucIgnoreStop) != sizeof(byte)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_idNPC);
            buff.WriteInt(m_idScript);
            buff.WriteByte(ucIgnoreStop);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        
		private uint    m_idNPC;
		private int	    m_idScript;		// 任务的脚本ID
        private byte    ucIgnoreStop;   //是否忽略停止,当等于UCHAR_MAX，则使得Human停下来

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
        public byte IgnoreStop
        {
            get { return ucIgnoreStop; }
            set { ucIgnoreStop = value; }
        }
    }

    public class CGMissionAcceptFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGMissionAccept(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONACCEPT; }
        public override int GetPacketMaxSize() { return sizeof(int) + sizeof(uint) + sizeof(byte); }
    }
}