using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGMissionRefuse : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_MISSIONREFUSE;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(int);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_idNPC) != sizeof(uint)) return false;
            if (buff.ReadInt(ref m_idScript) != sizeof(int)) return false;
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_idNPC);
            buff.WriteInt(m_idScript);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        private uint m_idNPC;
        private int m_idScript;		// 任务的脚本ID

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
    }

    public class CGMissionRefuseFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGMissionRefuse(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_CG_MISSIONREFUSE; }
        public override int GetPacketMaxSize() { return sizeof(uint) + sizeof(int); }
    }
}